using System.Text.RegularExpressions;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-03 • Bạn 1 phụ trách: Hộp thoại thêm / sửa lớp học (mốc UI, dữ liệu minh họa).
public partial class ClassEditDialog : Form
{
    private static readonly Regex ValidClassCodeRegex = new("^[A-Z0-9]{2,15}$", RegexOptions.Compiled);
    private static readonly Regex SchoolYearRegex = new(@"^(\d{4})-(\d{4})$", RegexOptions.Compiled);

    private int _editingId;
    private string _initialCode = string.Empty;
    private string _initialName = string.Empty;
    private string _initialSchoolYear = string.Empty;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    internal ClassPreview? Result { get; private set; }

    public ClassEditDialog()
    {
        InitializeComponent();
        CaptureInitialSnapshot();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
    }

    internal void LoadClass(ClassPreview classItem)
    {
        ArgumentNullException.ThrowIfNull(classItem);

        _suppressDirtyTracking = true;
        _editingId = classItem.Id;

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

        if (string.IsNullOrWhiteSpace(code))
        {
            _lblCodeError.Text = "Vui lòng nhập mã lớp.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (!ValidClassCodeRegex.IsMatch(code))
        {
            _lblCodeError.Text = "Mã lớp chỉ gồm chữ in hoa không dấu (A–Z) hoặc số (0–9), độ dài 2–15 ký tự.";
            _lblCodeError.Visible = true;
            firstInvalidControl ??= _txtCode;
        }
        else if (AdminPreviewData.Classes.Any(c =>
                     c.Id != _editingId &&
                     string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)))
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

        Result = new ClassPreview
        {
            Id = _editingId,
            Code = code,
            Name = name,
            SchoolYear = schoolYear
        };

        _allowCloseWithoutPrompt = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void EnsureHeightFitsContent()
    {
        _rootLayout.PerformLayout();
        var preferredHeight = _rootLayout.GetPreferredSize(new Size(ClientSize.Width, 0)).Height + 24;
        UiStyle.FitClientHeightWithinWorkingArea(this, preferredHeight);
    }
}
