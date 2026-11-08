using System.Text.RegularExpressions;
using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-03 • Bạn 1 phụ trách: Hộp thoại thêm / sửa sinh viên (mốc UI, dữ liệu minh họa).
public partial class StudentEditDialog : Form
{
    private static readonly Regex ValidStudentCodeRegex = new(@"^\d{8,10}$", RegexOptions.Compiled);

    private int _editingId;
    private string _initialStudentCode = string.Empty;
    private string _initialFullName = string.Empty;
    private string _initialClassCode = string.Empty;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    public User? Result { get; private set; }

    public StudentEditDialog()
    {
        InitializeComponent();
        PopulateClasses();
        CaptureInitialSnapshot();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
    }

    public void InitializeForAdd(string? defaultClassCode)
    {
        _suppressDirtyTracking = true;
        _editingId = 0;

        Text = "Thêm sinh viên";
        _lblTitle.Text = "Thêm sinh viên";

        _txtStudentCode.Clear();
        _txtFullName.Clear();
        SelectClassInCombo(defaultClassCode);

        ClearErrors();
        _suppressDirtyTracking = false;
        CaptureInitialSnapshot();
    }

    public void LoadStudent(User student)
    {
        ArgumentNullException.ThrowIfNull(student);

        _suppressDirtyTracking = true;
        _editingId = student.Id;

        var titleText = $"Sửa sinh viên: {student.StudentCode}";
        Text = titleText;
        _lblTitle.Text = titleText;

        _txtStudentCode.Text = student.StudentCode;
        _txtFullName.Text = student.FullName;
        SelectClassInCombo(student.ClassName);

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

    private void PopulateClasses()
    {
        _suppressDirtyTracking = true;
        _cboClass.Items.Clear();

        foreach (var cls in AdminPreviewData.Classes)
        {
            _cboClass.Items.Add(new ClassOption(cls.Code, $"{cls.Code} – {cls.Name}"));
        }

        if (_cboClass.Items.Count > 0)
        {
            _cboClass.SelectedIndex = 0;
        }

        _suppressDirtyTracking = false;
    }

    private void SelectClassInCombo(string? classCode)
    {
        if (!string.IsNullOrWhiteSpace(classCode))
        {
            for (var i = 0; i < _cboClass.Items.Count; i++)
            {
                if (_cboClass.Items[i] is ClassOption option &&
                    string.Equals(option.Code, classCode, StringComparison.OrdinalIgnoreCase))
                {
                    _cboClass.SelectedIndex = i;
                    return;
                }
            }
        }

        if (_cboClass.Items.Count > 0 && _cboClass.SelectedIndex < 0)
        {
            _cboClass.SelectedIndex = 0;
        }
    }

    private void CaptureInitialSnapshot()
    {
        _initialStudentCode = _txtStudentCode.Text.Trim();
        _initialFullName = _txtFullName.Text.Trim();
        _initialClassCode = (_cboClass.SelectedItem as ClassOption)?.Code ?? string.Empty;
    }

    private bool HasUnsavedChanges()
    {
        if (_suppressDirtyTracking)
        {
            return false;
        }

        var currentClassCode = (_cboClass.SelectedItem as ClassOption)?.Code ?? string.Empty;
        return !string.Equals(_txtStudentCode.Text.Trim(), _initialStudentCode, StringComparison.Ordinal) ||
               !string.Equals(_txtFullName.Text.Trim(), _initialFullName, StringComparison.Ordinal) ||
               !string.Equals(currentClassCode, _initialClassCode, StringComparison.OrdinalIgnoreCase);
    }

    private void OnStudentCodeTextChanged(object? sender, EventArgs e)
    {
        if (_lblStudentCodeError.Visible)
        {
            _lblStudentCodeError.Visible = false;
        }
    }

    private void OnFullNameTextChanged(object? sender, EventArgs e)
    {
        if (_lblFullNameError.Visible)
        {
            _lblFullNameError.Visible = false;
        }
    }

    private void OnClassSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_lblClassError.Visible)
        {
            _lblClassError.Visible = false;
        }
    }

    private void ClearErrors()
    {
        _lblStudentCodeError.Visible = false;
        _lblFullNameError.Visible = false;
        _lblClassError.Visible = false;
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

        var studentCode = _txtStudentCode.Text.Trim();
        var fullName = _txtFullName.Text.Trim();
        var selectedClass = _cboClass.SelectedItem as ClassOption;

        if (string.IsNullOrWhiteSpace(studentCode))
        {
            _lblStudentCodeError.Text = "Vui lòng nhập mã số sinh viên (MSSV).";
            _lblStudentCodeError.Visible = true;
            firstInvalidControl ??= _txtStudentCode;
        }
        else if (!ValidStudentCodeRegex.IsMatch(studentCode))
        {
            _lblStudentCodeError.Text = "MSSV chỉ gồm chữ số (0–9) và có độ dài từ 8 đến 10 ký tự.";
            _lblStudentCodeError.Visible = true;
            firstInvalidControl ??= _txtStudentCode;
        }
        else
        {
            var duplicate = AdminPreviewData.Students.FirstOrDefault(s =>
                s.Id != _editingId &&
                string.Equals(s.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase));

            if (duplicate is not null)
            {
                _lblStudentCodeError.Text =
                    $"MSSV \"{studentCode}\" đã tồn tại ({duplicate.FullName} – Lớp {duplicate.ClassName}).";
                _lblStudentCodeError.Visible = true;
                firstInvalidControl ??= _txtStudentCode;
            }
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            _lblFullNameError.Text = "Vui lòng nhập họ và tên sinh viên.";
            _lblFullNameError.Visible = true;
            firstInvalidControl ??= _txtFullName;
        }
        else if (fullName.Length > 100)
        {
            _lblFullNameError.Text = $"Họ và tên không được vượt quá 100 ký tự (hiện tại {fullName.Length} ký tự).";
            _lblFullNameError.Visible = true;
            firstInvalidControl ??= _txtFullName;
        }

        if (selectedClass is null)
        {
            _lblClassError.Text = "Vui lòng chọn lớp học.";
            _lblClassError.Visible = true;
            firstInvalidControl ??= _cboClass;
        }

        EnsureHeightFitsContent();

        if (firstInvalidControl is not null)
        {
            firstInvalidControl.Focus();
            return;
        }

        Result = new User
        {
            Id = _editingId,
            Username = studentCode,
            StudentCode = studentCode,
            FullName = fullName,
            ClassName = selectedClass!.Code,
            Role = UserRole.Student
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

    private sealed record ClassOption(string Code, string DisplayText)
    {
        public override string ToString() => DisplayText;
    }
}
