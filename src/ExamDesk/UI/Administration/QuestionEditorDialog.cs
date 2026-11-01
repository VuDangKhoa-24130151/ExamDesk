using ExamDesk.Models;

namespace ExamDesk.UI.Administration;

// GV-05 • Bạn 1 phụ trách: Hộp thoại thêm / sửa câu hỏi (mốc UI, dữ liệu minh họa).
public partial class QuestionEditorDialog : Form
{
    private int _editingId;
    private string _editingImagePath = string.Empty;
    private QuestionSnapshot _initialSnapshot;
    private bool _suppressDirtyTracking;
    private bool _allowCloseWithoutPrompt;

    public Question? Result { get; private set; }

    public QuestionEditorDialog()
    {
        InitializeComponent();
        PopulateSubjects();
        CaptureInitialSnapshot();
    }

    public void LoadQuestion(Question q)
    {
        ArgumentNullException.ThrowIfNull(q);

        _suppressDirtyTracking = true;

        _editingId = q.Id;
        _editingImagePath = q.ImagePath;

        var titleText = $"Sửa câu hỏi #{q.Id}";
        Text = titleText;
        _lblTitle.Text = titleText;

        for (var i = 0; i < _cboSubject.Items.Count; i++)
        {
            if (_cboSubject.Items[i] is SubjectOption item && item.Id == q.SubjectId)
            {
                _cboSubject.SelectedIndex = i;
                break;
            }
        }

        var chapterVal = Math.Clamp(q.Chapter, (int)_numChapter.Minimum, (int)_numChapter.Maximum);
        _numChapter.Value = chapterVal;

        _radEasy.Checked = string.Equals(q.Difficulty, "Easy", StringComparison.OrdinalIgnoreCase);
        _radMedium.Checked = string.Equals(q.Difficulty, "Medium", StringComparison.OrdinalIgnoreCase);
        _radHard.Checked = string.Equals(q.Difficulty, "Hard", StringComparison.OrdinalIgnoreCase);
        if (!_radEasy.Checked && !_radMedium.Checked && !_radHard.Checked)
        {
            _radEasy.Checked = true;
        }

        _txtContent.Text = q.Content;
        _txtOptionA.Text = q.OptionA;
        _txtOptionB.Text = q.OptionB;
        _txtOptionC.Text = q.OptionC;
        _txtOptionD.Text = q.OptionD;

        _radCorrectA.Checked = string.Equals(q.CorrectOption, "A", StringComparison.OrdinalIgnoreCase);
        _radCorrectB.Checked = string.Equals(q.CorrectOption, "B", StringComparison.OrdinalIgnoreCase);
        _radCorrectC.Checked = string.Equals(q.CorrectOption, "C", StringComparison.OrdinalIgnoreCase);
        _radCorrectD.Checked = string.Equals(q.CorrectOption, "D", StringComparison.OrdinalIgnoreCase);

        UpdateCharCount();
        ClearAllErrors();

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

    private void PopulateSubjects()
    {
        _suppressDirtyTracking = true;
        _cboSubject.Items.Clear();
        foreach (var subject in AdminPreviewData.Subjects)
        {
            _cboSubject.Items.Add(new SubjectOption(subject.Id, $"{subject.Code} – {subject.Name}"));
        }

        if (_cboSubject.Items.Count > 0)
        {
            _cboSubject.SelectedIndex = 0;
        }
        _suppressDirtyTracking = false;
    }

    private void CaptureInitialSnapshot()
    {
        _initialSnapshot = GetCurrentSnapshot();
    }

    private QuestionSnapshot GetCurrentSnapshot() => new(
        SubjectId: (_cboSubject.SelectedItem as SubjectOption)?.Id ?? 0,
        Chapter: (int)_numChapter.Value,
        Difficulty: GetSelectedDifficulty(),
        Content: _txtContent.Text,
        OptionA: _txtOptionA.Text,
        OptionB: _txtOptionB.Text,
        OptionC: _txtOptionC.Text,
        OptionD: _txtOptionD.Text,
        CorrectOption: GetSelectedCorrectOption());

    private bool HasUnsavedChanges() => !_suppressDirtyTracking && !Equals(_initialSnapshot, GetCurrentSnapshot());

    private string GetSelectedDifficulty()
    {
        if (_radEasy.Checked) return "Easy";
        if (_radMedium.Checked) return "Medium";
        if (_radHard.Checked) return "Hard";
        return string.Empty;
    }

    private string GetSelectedCorrectOption()
    {
        if (_radCorrectA.Checked) return "A";
        if (_radCorrectB.Checked) return "B";
        if (_radCorrectC.Checked) return "C";
        if (_radCorrectD.Checked) return "D";
        return string.Empty;
    }

    private void OnSubjectChanged(object? sender, EventArgs e)
    {
        _lblSubjectError.Visible = false;
    }

    private void OnChapterChanged(object? sender, EventArgs e)
    {
        _lblChapterError.Visible = false;
    }

    private void OnDifficultyChanged(object? sender, EventArgs e)
    {
        _lblDifficultyError.Visible = false;
    }

    private void OnContentTextChanged(object? sender, EventArgs e)
    {
        UpdateCharCount();
        if (_lblContentError.Visible)
        {
            _lblContentError.Visible = false;
        }
    }

    private void UpdateCharCount()
    {
        _lblCharCount.Text = $"{_txtContent.Text.Length} ký tự";
    }

    private void OnOptionTextChanged(object? sender, EventArgs e)
    {
        if (ReferenceEquals(sender, _txtOptionA)) _lblOptionAError.Visible = false;
        else if (ReferenceEquals(sender, _txtOptionB)) _lblOptionBError.Visible = false;
        else if (ReferenceEquals(sender, _txtOptionC)) _lblOptionCError.Visible = false;
        else if (ReferenceEquals(sender, _txtOptionD)) _lblOptionDError.Visible = false;
    }

    private void OnCorrectOptionCheckedChanged(object? sender, EventArgs e)
    {
        if (sender is not RadioButton changed || !changed.Checked)
        {
            return;
        }

        if (!ReferenceEquals(changed, _radCorrectA)) _radCorrectA.Checked = false;
        if (!ReferenceEquals(changed, _radCorrectB)) _radCorrectB.Checked = false;
        if (!ReferenceEquals(changed, _radCorrectC)) _radCorrectC.Checked = false;
        if (!ReferenceEquals(changed, _radCorrectD)) _radCorrectD.Checked = false;

        _lblCorrectOptionError.Visible = false;
    }

    private void ClearAllErrors()
    {
        _lblSubjectError.Visible = false;
        _lblChapterError.Visible = false;
        _lblDifficultyError.Visible = false;
        _lblContentError.Visible = false;
        _lblOptionAError.Visible = false;
        _lblOptionBError.Visible = false;
        _lblOptionCError.Visible = false;
        _lblOptionDError.Visible = false;
        _lblCorrectOptionError.Visible = false;
    }

    private void OnPreviewClick(object? sender, EventArgs e)
    {
        MessageBox.Show(
            this,
            "GV-06 Xem trước câu hỏi sẽ dựng ở bước sau.",
            "Xem trước câu hỏi (GV-06)",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
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
        ClearAllErrors();

        Control? firstInvalidControl = null;

        var selectedSubject = _cboSubject.SelectedItem as SubjectOption;
        if (selectedSubject is null)
        {
            _lblSubjectError.Text = "Vui lòng chọn môn học.";
            _lblSubjectError.Visible = true;
            firstInvalidControl ??= _cboSubject;
        }

        var chapter = (int)_numChapter.Value;
        if (chapter < 1 || chapter > 20)
        {
            _lblChapterError.Text = "Chương phải từ 1 đến 20.";
            _lblChapterError.Visible = true;
            firstInvalidControl ??= _numChapter;
        }

        var difficulty = GetSelectedDifficulty();
        if (string.IsNullOrEmpty(difficulty))
        {
            _lblDifficultyError.Text = "Vui lòng chọn độ khó.";
            _lblDifficultyError.Visible = true;
            firstInvalidControl ??= _radEasy;
        }

        var content = _txtContent.Text.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            _lblContentError.Text = "Vui lòng nhập nội dung câu hỏi.";
            _lblContentError.Visible = true;
            firstInvalidControl ??= _txtContent;
        }

        var optA = _txtOptionA.Text.Trim();
        var optB = _txtOptionB.Text.Trim();
        var optC = _txtOptionC.Text.Trim();
        var optD = _txtOptionD.Text.Trim();

        if (string.IsNullOrWhiteSpace(optA))
        {
            _lblOptionAError.Text = "Vui lòng nhập đáp án A.";
            _lblOptionAError.Visible = true;
            firstInvalidControl ??= _txtOptionA;
        }

        if (string.IsNullOrWhiteSpace(optB))
        {
            _lblOptionBError.Text = "Vui lòng nhập đáp án B.";
            _lblOptionBError.Visible = true;
            firstInvalidControl ??= _txtOptionB;
        }

        if (string.IsNullOrWhiteSpace(optC))
        {
            _lblOptionCError.Text = "Vui lòng nhập đáp án C.";
            _lblOptionCError.Visible = true;
            firstInvalidControl ??= _txtOptionC;
        }

        if (string.IsNullOrWhiteSpace(optD))
        {
            _lblOptionDError.Text = "Vui lòng nhập đáp án D.";
            _lblOptionDError.Visible = true;
            firstInvalidControl ??= _txtOptionD;
        }

        // Kiểm tra trùng nhau giữa các đáp án đã nhập (bỏ qua hoa/thường và khoảng trắng hai đầu)
        var optionEntries = new (string Label, string Text, TextBox Box, Label ErrorLabel)[]
        {
            ("A", optA, _txtOptionA, _lblOptionAError),
            ("B", optB, _txtOptionB, _lblOptionBError),
            ("C", optC, _txtOptionC, _lblOptionCError),
            ("D", optD, _txtOptionD, _lblOptionDError)
        };

        for (var i = 0; i < optionEntries.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(optionEntries[i].Text))
            {
                continue;
            }

            for (var j = i + 1; j < optionEntries.Length; j++)
            {
                if (string.IsNullOrWhiteSpace(optionEntries[j].Text))
                {
                    continue;
                }

                if (string.Equals(optionEntries[i].Text, optionEntries[j].Text, StringComparison.OrdinalIgnoreCase))
                {
                    optionEntries[j].ErrorLabel.Text = $"Đáp án {optionEntries[j].Label} trùng nội dung với đáp án {optionEntries[i].Label}.";
                    optionEntries[j].ErrorLabel.Visible = true;
                    firstInvalidControl ??= optionEntries[j].Box;
                }
            }
        }

        var correctOption = GetSelectedCorrectOption();
        if (string.IsNullOrEmpty(correctOption))
        {
            _lblCorrectOptionError.Text = "Vui lòng chọn 1 đáp án đúng (A, B, C hoặc D).";
            _lblCorrectOptionError.Visible = true;
            firstInvalidControl ??= _radCorrectA;
        }

        if (firstInvalidControl is not null)
        {
            _scrollPanel.ScrollControlIntoView(firstInvalidControl);
            firstInvalidControl.Focus();
            return;
        }

        Result = new Question
        {
            Id = _editingId,
            SubjectId = selectedSubject!.Id,
            Chapter = chapter,
            Difficulty = difficulty,
            Content = content,
            OptionA = optA,
            OptionB = optB,
            OptionC = optC,
            OptionD = optD,
            CorrectOption = correctOption,
            ImagePath = _editingImagePath
        };

        _allowCloseWithoutPrompt = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record SubjectOption(int Id, string DisplayText)
    {
        public override string ToString() => DisplayText;
    }

    private readonly record struct QuestionSnapshot(
        int SubjectId,
        int Chapter,
        string Difficulty,
        string Content,
        string OptionA,
        string OptionB,
        string OptionC,
        string OptionD,
        string CorrectOption);
}
