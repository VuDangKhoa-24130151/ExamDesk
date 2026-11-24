using System.Text.RegularExpressions;
using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-03 • Bạn 1 phụ trách: Hộp thoại thêm / sửa lớp học (kết nối SQLite qua AdministrationService).
public partial class ClassEditDialog : Form
{
    private static readonly Regex ValidClassCodeRegex = new("^[A-Z0-9]{2,15}$", RegexOptions.Compiled);
    private static readonly Regex SchoolYearRegex = new(@"^(\d{4})-(\d{4})$", RegexOptions.Compiled);

    private readonly AdministrationService _service;

    private int _editingId;
    private string _editingOriginalCode = string.Empty;
    private string _initialCode = string.Empty;
    private string _initialName = string.Empty;
    private string _initialSchoolYear = string.Empty;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    public ClassInfo? Result { get; private set; }

    public ClassEditDialog() : this(new AdministrationService())
    {
    }

    public ClassEditDialog(AdministrationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();
        _lblDemoBadge.Visible = false;
        _cardPanel.SizeChanged += (_, _) => UpdateWrappedLabelWidths();
        CaptureInitialSnapshot();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateWrappedLabelWidths();
        EnsureHeightFitsContent();
    }

    public void LoadClass(ClassInfo classItem)
    {
        ArgumentNullException.ThrowIfNull(classItem);

        _suppressDirtyTracking = true;
        _editingId = classItem.Id;
        _editingOriginalCode = classItem.Code;

        var titleText = $"Sửa lớp: {classItem.Code}";
        Text = titleText;
        _lblTitle.Text = titleText;

        _txtCode.Text = classItem.Code;
        _txtName.Text = classItem.Name;
        _txtSchoolYear.Text = classItem.SchoolYear;

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
        _initialSchoolYear = _txtSchoolYear.Text.Trim();
    }

    private bool HasUnsavedChanges()
    {
        if (_suppressDirtyTracking)
        {
            return false;
        }

        return !string.Equals(_txtCode.Text.Trim(), _initialCode, StringComparison.Ordinal) ||
               !string.Equals(_txtName.Text.Trim(), _initialName, StringComparison.Ordinal) ||
               !string.Equals(_txtSchoolYear.Text.Trim(), _initialSchoolYear, StringComparison.Ordinal);
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

    private void OnSchoolYearTextChanged(object? sender, EventArgs e)
    {
        if (_lblSchoolYearError.Visible)
        {
            _lblSchoolYearError.Visible = false;
        }
    }

    private void ClearErrors()
    {
        _lblCodeError.Visible = false;
        _lblNameError.Visible = false;
        _lblSchoolYearError.Visible = false;
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
        var schoolYear = _txtSchoolYear.Text.Trim();

        var isUnchangedExistingCode = !string.IsNullOrWhiteSpace(_editingOriginalCode) &&
            string.Equals(code, _editingOriginalCode, StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(code))
        {
            _lblCodeError.Text = "Vui lòng nhập mã lớp.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (!isUnchangedExistingCode && !ValidClassCodeRegex.IsMatch(code))
        {
            _lblCodeError.Text = "Mã lớp chỉ gồm chữ in hoa không dấu (A–Z) hoặc số (0–9), độ dài 2–15 ký tự.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (_service.IsClassCodeExists(code, _editingId > 0 ? _editingId : null))
        {
            _lblCodeError.Text = $"Mã lớp \"{code}\" đã tồn tại. Vui lòng nhập mã khác.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            _lblNameError.Text = "Vui lòng nhập tên lớp.";
            _lblNameError.Visible = true;
            firstInvalidControl ??= _txtName;
        }
        else if (name.Length > 100)
        {
            _lblNameError.Text = $"Tên lớp không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).";
            _lblNameError.Visible = true;
            firstInvalidControl ??= _txtName;
        }

        if (string.IsNullOrWhiteSpace(schoolYear))
        {
            _lblSchoolYearError.Text = "Vui lòng nhập niên khóa (ví dụ: 2024-2028).";
            _lblSchoolYearError.Visible = true;
            firstInvalidControl ??= _txtSchoolYear;
        }
        else
        {
            var match = SchoolYearRegex.Match(schoolYear);
            if (!match.Success)
            {
                _lblSchoolYearError.Text = "Niên khóa phải đúng định dạng YYYY-YYYY (ví dụ: 2024-2028).";
                _lblSchoolYearError.Visible = true;
                firstInvalidControl ??= _txtSchoolYear;
            }
            else
            {
                var startYear = int.Parse(match.Groups[1].Value);
                var endYear = int.Parse(match.Groups[2].Value);
                if (endYear <= startYear)
                {
                    _lblSchoolYearError.Text = $"Năm kết thúc ({endYear}) phải lớn hơn năm bắt đầu ({startYear}).";
                    _lblSchoolYearError.Visible = true;
                    firstInvalidControl ??= _txtSchoolYear;
                }
            }
        }

        EnsureHeightFitsContent();

        if (firstInvalidControl is not null)
        {
            firstInvalidControl.Focus();
            return;
        }

        try
        {
            var classInfo = new ClassInfo
            {
                Id = _editingId,
                Code = code,
                Name = name,
                SchoolYear = schoolYear
            };

            _service.SaveClass(classInfo, string.IsNullOrEmpty(_editingOriginalCode) ? null : _editingOriginalCode);
            Result = classInfo;

            _allowCloseWithoutPrompt = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi lưu lớp học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void UpdateWrappedLabelWidths()
    {
        if (_cardPanel.ClientSize.Width <= 0)
        {
            return;
        }

        var maxWidth = Math.Max(220, _cardPanel.ClientSize.Width - _cardPanel.Padding.Horizontal - 4);
        _lblCodeHint.MaximumSize = new Size(maxWidth, 0);
        _lblCodeError.MaximumSize = new Size(maxWidth, 0);
        _lblNameError.MaximumSize = new Size(maxWidth, 0);
        _lblSchoolYearHint.MaximumSize = new Size(maxWidth, 0);
        _lblSchoolYearError.MaximumSize = new Size(maxWidth, 0);
    }

    private void EnsureHeightFitsContent()
    {
        _rootLayout.PerformLayout();
        var preferredHeight = _rootLayout.GetPreferredSize(new Size(ClientSize.Width, 0)).Height + 24;
        UiStyle.FitClientHeightWithinWorkingArea(this, preferredHeight);
    }
}
