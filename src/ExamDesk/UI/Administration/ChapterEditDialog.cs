using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-02 • Bạn 1 phụ trách: Hộp thoại thêm / sửa chương của môn học (kết nối SQLite qua QuestionBankService).
public partial class ChapterEditDialog : Form
{
    private readonly QuestionBankService _service;

    private int _subjectId;
    private int? _editingOriginalNumber;
    private int _initialNumber = 1;
    private string _initialName = string.Empty;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    public ChapterInfo? Result { get; private set; }

    public ChapterEditDialog() : this(new QuestionBankService())
    {
    }

    public ChapterEditDialog(QuestionBankService service)
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

    public void InitializeForAdd(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        _suppressDirtyTracking = true;
        _subjectId = subject.Id;
        _editingOriginalNumber = null;

        Text = "Thêm chương";
        _lblTitle.Text = "Thêm chương";
        _lblSubjectInfo.Text = $"Môn học: {subject.Code} – {subject.Name}";

        var existingNumbers = _service.GetChaptersBySubject(subject.Id)
            .Select(c => c.Number)
            .ToHashSet();

        var suggestedNumber = 1;
        while (suggestedNumber < 50 && existingNumbers.Contains(suggestedNumber))
        {
            suggestedNumber++;
        }

        _numChapterNumber.Value = suggestedNumber;
        _txtName.Clear();

        ClearErrors();
        _suppressDirtyTracking = false;
        CaptureInitialSnapshot();
    }

    public void LoadChapter(Subject subject, ChapterInfo chapter)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(chapter);

        _suppressDirtyTracking = true;
        _subjectId = subject.Id;
        _editingOriginalNumber = chapter.Number;

        var titleText = $"Sửa Chương {chapter.Number}";
        Text = titleText;
        _lblTitle.Text = titleText;
        _lblSubjectInfo.Text = $"Môn học: {subject.Code} – {subject.Name}";

        var clampedNumber = Math.Clamp(chapter.Number, (int)_numChapterNumber.Minimum, (int)_numChapterNumber.Maximum);
        _numChapterNumber.Value = clampedNumber;
        _txtName.Text = chapter.Name;

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
        _initialNumber = (int)_numChapterNumber.Value;
        _initialName = _txtName.Text.Trim();
    }

    private bool HasUnsavedChanges()
    {
        if (_suppressDirtyTracking)
        {
            return false;
        }

        return (int)_numChapterNumber.Value != _initialNumber ||
               !string.Equals(_txtName.Text.Trim(), _initialName, StringComparison.Ordinal);
    }

    private void OnNumberChanged(object? sender, EventArgs e)
    {
        if (_lblNumberError.Visible)
        {
            _lblNumberError.Visible = false;
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
        _lblNumberError.Visible = false;
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

        var number = (int)_numChapterNumber.Value;
        var name = _txtName.Text.Trim();

        if (number < 1 || number > 50)
        {
            _lblNumberError.Text = "Chương số phải nằm trong khoảng từ 1 đến 50.";
            _lblNumberError.Visible = true;
            firstInvalidControl ??= _numChapterNumber;
        }
        else
        {
            var existingChapters = _service.GetChaptersBySubject(_subjectId);
            if (existingChapters.Any(c => c.Number == number && c.Number != _editingOriginalNumber))
            {
                _lblNumberError.Text = $"Chương số {number} đã tồn tại trong môn học này. Vui lòng chọn số khác.";
                _lblNumberError.Visible = true;
                firstInvalidControl ??= _numChapterNumber;
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            _lblNameError.Text = "Vui lòng nhập tên chương.";
            _lblNameError.Visible = true;
            firstInvalidControl ??= _txtName;
        }
        else if (name.Length > 100)
        {
            _lblNameError.Text = $"Tên chương không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).";
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
            var chapter = new ChapterInfo
            {
                SubjectId = _subjectId,
                Number = number,
                Name = name
            };

            _service.SaveChapter(chapter, _editingOriginalNumber);
            Result = chapter;

            _allowCloseWithoutPrompt = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi lưu chương",
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
