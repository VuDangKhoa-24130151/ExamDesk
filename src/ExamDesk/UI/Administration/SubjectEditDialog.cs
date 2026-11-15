using System.Text.RegularExpressions;
using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-02 • Bạn 1 phụ trách: Hộp thoại thêm / sửa môn học (kết nối SQLite qua QuestionBankService).
public partial class SubjectEditDialog : Form
{
    private static readonly Regex ValidCodeRegex = new("^[A-Z0-9]{2,10}$", RegexOptions.Compiled);
    private readonly QuestionBankService _service;

    private int _editingId;
    private string _initialCode = string.Empty;
    private string _initialName = string.Empty;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    public Subject? Result { get; private set; }

    public SubjectEditDialog() : this(new QuestionBankService())
    {
    }

    public SubjectEditDialog(QuestionBankService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();
        CaptureInitialSnapshot();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
    }

    public void LoadSubject(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        _suppressDirtyTracking = true;
        _editingId = subject.Id;

        var titleText = $"Sửa môn học: {subject.Code}";
        Text = titleText;
        _lblTitle.Text = titleText;

        _txtCode.Text = subject.Code;
        _txtName.Text = subject.Name;

        ClearErrors();
        _suppressDirtyTracking = false;
        CaptureInitialSnapshot();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.S))
        {
            TrySave();
            return true;
        }

        if (keyData == Keys.Escape)
        {
            RequestCancelClose();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_allowCloseWithoutPrompt && DialogResult != DialogResult.OK && HasUnsavedChanges())
        {
            var confirm = MessageBox.Show(
                this,
                "Bỏ các thay đổi chưa lưu?",
                "Xác nhận hủy thay đổi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        base.OnFormClosing(e);
    }

    private void CaptureInitialSnapshot()
    {
        _initialCode = _txtCode.Text.Trim();
        _initialName = _txtName.Text.Trim();
    }

    private bool HasUnsavedChanges()
    {
        if (_suppressDirtyTracking)
        {
            return false;
        }

        return !string.Equals(_txtCode.Text.Trim(), _initialCode, StringComparison.Ordinal) ||
               !string.Equals(_txtName.Text.Trim(), _initialName, StringComparison.Ordinal);
    }

    private void OnCodeTextChanged(object? sender, EventArgs e)
    {
        if (_lblCodeError.Visible)
        {
            _lblCodeError.Visible = false;
        }
    }

    private void OnNameTextChanged(object? sender, EventArgs e)
    {
        if (_lblNameError.Visible)
        {
            _lblNameError.Visible = false;
        }
    }

    private void ClearErrors()
    {
        _lblCodeError.Visible = false;
        _lblNameError.Visible = false;
    }

    private void OnCancelClick(object? sender, EventArgs e)
    {
        RequestCancelClose();
    }

    private void RequestCancelClose()
    {
        if (HasUnsavedChanges())
        {
            var confirm = MessageBox.Show(
                this,
                "Bỏ các thay đổi chưa lưu?",
                "Xác nhận hủy thay đổi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
            {
                return;
            }
        }

        _allowCloseWithoutPrompt = true;
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void OnSaveClick(object? sender, EventArgs e)
    {
        TrySave();
    }

    private void TrySave()
    {
        ClearErrors();

        Control? firstInvalidControl = null;

        var code = _txtCode.Text.Trim().ToUpperInvariant();
        var name = _txtName.Text.Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            _lblCodeError.Text = "Vui lòng nhập mã môn học.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (!ValidCodeRegex.IsMatch(code))
        {
            _lblCodeError.Text = "Mã môn chỉ gồm chữ in hoa không dấu (A–Z) hoặc số (0–9), độ dài 2–10 ký tự.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (_service.IsSubjectCodeExists(code, _editingId))
        {
            _lblCodeError.Text = $"Mã môn \"{code}\" đã tồn tại. Vui lòng nhập mã khác.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            _lblNameError.Text = "Vui lòng nhập tên môn học.";
            _lblNameError.Visible = true;
            firstInvalidControl ??= _txtName;
        }
        else if (name.Length > 100)
        {
            _lblNameError.Text = $"Tên môn không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).";
            _lblNameError.Visible = true;
            firstInvalidControl ??= _txtName;
        }

        EnsureHeightFitsContent();

        if (firstInvalidControl is not null)
        {
            firstInvalidControl.Focus();
            return;
        }

        try
        {
            var subject = new Subject
            {
                Id = _editingId,
                Code = code,
                Name = name
            };

            _service.SaveSubject(subject);
            Result = subject;

            _allowCloseWithoutPrompt = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi lưu môn học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            firstInvalidControl?.Focus();
        }
    }

    private void EnsureHeightFitsContent()
    {
        _rootLayout.PerformLayout();
        var preferredHeight = _rootLayout.GetPreferredSize(new Size(ClientSize.Width, 0)).Height + 24;
        UiStyle.FitClientHeightWithinWorkingArea(this, preferredHeight);
    }
}
