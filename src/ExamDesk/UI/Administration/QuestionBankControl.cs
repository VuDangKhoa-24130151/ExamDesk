using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-04 • Bạn 1 phụ trách: Quản lý ngân hàng câu hỏi (kết nối SQLite qua QuestionBankService).
public partial class QuestionBankControl : UserControl, IUnsavedChangesGuard
{
    private readonly QuestionBankService _service;
    private Dictionary<int, Subject> _subjectMap = [];
    private bool _suppressFilterEvents;
    private TableLayoutPanel _emptyStateContainer = null!;
    private Button _btnSeedData = null!;

    // Thêm/sửa câu hỏi thực hiện qua hộp thoại modal QuestionEditorDialog (đã tự kiểm tra thay đổi chưa lưu).
    public bool HasUnsavedChanges => false;

    public bool ConfirmLeave() => true;

    public QuestionBankControl() : this(new QuestionBankService())
    {
    }

    public QuestionBankControl(QuestionBankService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();

        // Đã kết nối SQLite thật: ẩn nhãn dữ liệu minh họa
        _lblDemoBadge.Visible = false;

        InitializeEmptyStateContainer();
        _filterCardBorder.SizeChanged += (_, _) => UpdateFilterFlowMaxWidth();
        _gridHostPanel.SizeChanged += (_, _) => UpdateEmptyStateWrapWidth();

        InitializeFilterOptions();
        RefreshGrid();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateFilterFlowMaxWidth();
        UpdateEmptyStateWrapWidth();
    }

    private void InitializeEmptyStateContainer()
    {
        _gridHostPanel.Controls.Remove(_lblEmptyState);
        _lblEmptyState.Dock = DockStyle.None;
        _lblEmptyState.AutoSize = true;
        _lblEmptyState.Anchor = AnchorStyles.None;
        _lblEmptyState.Margin = Padding.Empty;
        _lblEmptyState.Visible = true;

        _btnSeedData = new Button
        {
            Text = "Nạp dữ liệu mẫu thử nghiệm",
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 12, 0, 0),
            Padding = new Padding(14, 6, 14, 6),
            Visible = false
        };
        UiStyle.StylePrimaryButton(_btnSeedData);
        _btnSeedData.Click += OnSeedDataClick;

        var centerFlow = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Anchor = AnchorStyles.None,
            BackColor = UiStyle.Surface,
            Margin = Padding.Empty
        };
        centerFlow.Controls.Add(_lblEmptyState);
        centerFlow.Controls.Add(_btnSeedData);

        _emptyStateContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiStyle.Surface,
            ColumnCount = 1,
            RowCount = 3,
            Visible = false
        };
        _emptyStateContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _emptyStateContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _emptyStateContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _emptyStateContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _emptyStateContainer.Controls.Add(centerFlow, 0, 1);

        _gridHostPanel.Controls.Add(_emptyStateContainer);
    }

    private void UpdateFilterFlowMaxWidth()
    {
        if (_filterCardBorder.ClientSize.Width <= 0)
        {
            return;
        }

        var maxWidth = Math.Max(240, _filterCardBorder.ClientSize.Width - _filterCardBorder.Padding.Horizontal);
        _filterFlow.MaximumSize = new Size(maxWidth, 0);
    }

    private void UpdateEmptyStateWrapWidth()
    {
        if (_gridHostPanel.ClientSize.Width <= 0)
        {
            return;
        }

        var maxWidth = Math.Max(220, _gridHostPanel.ClientSize.Width - 48);
        _lblEmptyState.MaximumSize = new Size(maxWidth, 0);
    }

    private void OnSeedDataClick(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            this,
            "Bạn có muốn nạp bộ câu hỏi mẫu cho 3 môn học (Lập trình .NET, Cơ sở dữ liệu, Mạng máy tính) vào cơ sở dữ liệu SQLite không?",
            "Xác nhận nạp dữ liệu mẫu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            var seedService = new QuestionSeedService();
            var (subjectsAdded, questionsAdded) = seedService.SeedQuestionBank();

            InitializeFilterOptions();
            RefreshGrid();

            var message = questionsAdded > 0
                ? $"Đã nạp thành công:\n• {subjectsAdded} môn học mới\n• {questionsAdded} câu hỏi mới vào cơ sở dữ liệu SQLite."
                : "Cơ sở dữ liệu đã có sẵn dữ liệu mẫu (không có bản ghi trùng lặp nào được thêm mới).";

            MessageBox.Show(
                this,
                message,
                "Nạp dữ liệu mẫu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi nạp dữ liệu: {ex.Message}",
                "Lỗi nạp dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.F))
        {
            _txtSearch.Focus();
            _txtSearch.SelectAll();
            return true;
        }

        if (keyData == Keys.Delete && _dgvQuestions.ContainsFocus && _btnDelete.Enabled)
        {
            DeleteSelectedQuestion();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void InitializeFilterOptions()
    {
        _suppressFilterEvents = true;

        _cboSubject.Items.Clear();
        _cboSubject.Items.Add(new FilterOption<int?>(null, "Tất cả môn học"));

        try
        {
            var subjects = _service.GetSubjects();
            _subjectMap = subjects.ToDictionary(s => s.Id);
            foreach (var subject in subjects)
            {
                _cboSubject.Items.Add(new FilterOption<int?>(subject.Id, $"{subject.Code} – {subject.Name}"));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách môn học: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        _cboSubject.SelectedIndex = 0;

        PopulateChapterFilterOptions(preferredChapter: null);

        _cboDifficulty.Items.Clear();
        _cboDifficulty.Items.Add(new FilterOption<string?>(null, "Tất cả"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Easy", "Dễ"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Medium", "Trung bình"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Hard", "Khó"));
        _cboDifficulty.SelectedIndex = 0;

        _suppressFilterEvents = false;
    }

    private void PopulateChapterFilterOptions(int? preferredChapter)
    {
        var selectedSubjectId = (_cboSubject.SelectedItem as FilterOption<int?>)?.Value;

        List<int> chapterNumbers;
        try
        {
            if (selectedSubjectId.HasValue)
            {
                var metaChapters = _service.GetChaptersBySubject(selectedSubjectId.Value).Select(c => c.Number);
                var distinctChapters = _service.GetDistinctChapters(selectedSubjectId.Value);
                chapterNumbers = metaChapters.Concat(distinctChapters)
                    .Where(n => n > 0)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();
            }
            else
            {
                chapterNumbers = _service.GetDistinctChapters()
                    .Where(n => n > 0)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();
            }
        }
        catch
        {
            chapterNumbers = [];
        }

        if (chapterNumbers.Count == 0)
        {
            chapterNumbers = [1, 2, 3, 4];
        }

        _cboChapter.Items.Clear();
        _cboChapter.Items.Add(new FilterOption<int?>(null, "Tất cả"));
        var targetIndex = 0;
        foreach (var ch in chapterNumbers)
        {
            var idx = _cboChapter.Items.Add(new FilterOption<int?>(ch, $"Chương {ch}"));
            if (preferredChapter.HasValue && ch == preferredChapter.Value)
            {
                targetIndex = idx;
            }
        }
        _cboChapter.SelectedIndex = targetIndex;
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        if (_suppressFilterEvents)
        {
            return;
        }

        if (ReferenceEquals(sender, _cboSubject))
        {
            var currentChapter = (_cboChapter.SelectedItem as FilterOption<int?>)?.Value;
            _suppressFilterEvents = true;
            PopulateChapterFilterOptions(currentChapter);
            _suppressFilterEvents = false;
        }

        RefreshGrid();
    }

    private void OnClearFilterClick(object? sender, EventArgs e)
    {
        _suppressFilterEvents = true;
        _cboSubject.SelectedIndex = 0;
        PopulateChapterFilterOptions(preferredChapter: null);
        _cboDifficulty.SelectedIndex = 0;
        _txtSearch.Clear();
        _suppressFilterEvents = false;

        RefreshGrid();
    }

    private void RefreshGrid(int? preferredSelectedId = null)
    {
        var selectedSubjectId = (_cboSubject.SelectedItem as FilterOption<int?>)?.Value;
        var selectedChapter = (_cboChapter.SelectedItem as FilterOption<int?>)?.Value;
        var selectedDifficulty = (_cboDifficulty.SelectedItem as FilterOption<string?>)?.Value;
        var keyword = _txtSearch.Text.Trim();

        List<Question> filtered;
        var totalCount = 0;

        try
        {
            filtered = _service.GetQuestions(selectedSubjectId, selectedChapter, selectedDifficulty, keyword);
            totalCount = _service.CountAllQuestions();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách câu hỏi: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            filtered = [];
        }

        _lblCount.Text = $"Hiển thị {filtered.Count} / {totalCount} câu";

        _dgvQuestions.SuspendLayout();
        _dgvQuestions.Rows.Clear();

        foreach (var question in filtered)
        {
            var subjectCode = FormatSubjectCode(question.SubjectId);
            var difficultyText = FormatDifficulty(question.Difficulty);
            var hasImage = !string.IsNullOrWhiteSpace(question.ImagePath);
            var shortContent = ShortenContent(question.Content);
            var displayContent = hasImage ? $"[🖼 Ảnh] {shortContent}" : shortContent;

            var rowIndex = _dgvQuestions.Rows.Add(
                $"#{question.Id}",
                subjectCode,
                $"Chương {question.Chapter}",
                difficultyText,
                displayContent,
                question.CorrectOption);

            var row = _dgvQuestions.Rows[rowIndex];
            row.Tag = question;

            var imageLine = hasImage
                ? $"\nẢnh minh họa: {Path.GetFileName(question.ImagePath)}\n"
                : string.Empty;

            var fullTooltip =
                $"[#{question.Id}] {subjectCode} • Chương {question.Chapter} • {difficultyText}\n\n" +
                $"{question.Content}\n" +
                imageLine +
                $"\nA. {question.OptionA}\n" +
                $"B. {question.OptionB}\n" +
                $"C. {question.OptionC}\n" +
                $"D. {question.OptionD}\n\n" +
                $"Đáp án đúng: {question.CorrectOption}";

            row.Cells[_colContent.Index].ToolTipText = fullTooltip;
        }

        _dgvQuestions.ResumeLayout();

        var isEmpty = filtered.Count == 0;
        if (totalCount == 0)
        {
            _lblEmptyState.Text = "Ngân hàng câu hỏi trong cơ sở dữ liệu hiện đang trống.\nBạn có thể tự bấm \"Thêm câu hỏi\" hoặc nạp nhanh bộ dữ liệu mẫu:";
            _btnSeedData.Visible = true;
            _emptyStateContainer.Visible = true;
            _emptyStateContainer.BringToFront();
            _dgvQuestions.Visible = false;
        }
        else if (isEmpty)
        {
            _lblEmptyState.Text = "Không có câu hỏi phù hợp với bộ lọc hiện tại. Thử đổi bộ lọc hoặc từ khóa tìm kiếm.";
            _btnSeedData.Visible = false;
            _emptyStateContainer.Visible = true;
            _emptyStateContainer.BringToFront();
            _dgvQuestions.Visible = false;
        }
        else
        {
            _emptyStateContainer.Visible = false;
            _btnSeedData.Visible = false;
            _dgvQuestions.Visible = true;
        }

        if (!isEmpty)
        {
            var targetRowIndex = 0;
            if (preferredSelectedId.HasValue)
            {
                for (var i = 0; i < _dgvQuestions.Rows.Count; i++)
                {
                    if (_dgvQuestions.Rows[i].Tag is Question q && q.Id == preferredSelectedId.Value)
                    {
                        targetRowIndex = i;
                        break;
                    }
                }
            }

            _dgvQuestions.ClearSelection();
            _dgvQuestions.Rows[targetRowIndex].Selected = true;
            _dgvQuestions.CurrentCell = _dgvQuestions.Rows[targetRowIndex].Cells[0];
        }

        UpdateActionButtonsState();
    }

    private string FormatSubjectCode(int subjectId)
    {
        if (_subjectMap.TryGetValue(subjectId, out var subject))
        {
            return subject.Code;
        }

        return $"Môn #{subjectId}";
    }

    private static string FormatDifficulty(string? diff) => diff switch
    {
        "Easy" => "Dễ",
        "Medium" => "Trung bình",
        "Hard" => "Khó",
        _ => diff ?? string.Empty
    };

    private static string ShortenContent(string? text, int maxLen = 60)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var trimmed = text.Trim();
        return trimmed.Length <= maxLen ? trimmed : trimmed[..maxLen] + "…";
    }

    private Question? GetSelectedQuestion()
    {
        if (_dgvQuestions.SelectedRows.Count == 0)
        {
            return null;
        }

        return _dgvQuestions.SelectedRows[0].Tag as Question;
    }

    private void OnGridSelectionChanged(object? sender, EventArgs e)
    {
        UpdateActionButtonsState();
    }

    private void UpdateActionButtonsState()
    {
        var hasSelection = _dgvQuestions.Visible && GetSelectedQuestion() is not null;
        _btnEdit.Enabled = hasSelection;
        _btnPreview.Enabled = hasSelection;
        _btnDelete.Enabled = hasSelection;
        _btnDelete.ForeColor = hasSelection ? UiStyle.ErrorText : UiStyle.TextMuted;
    }

    public void StartAddQuestion()
    {
        OnAddClick(this, EventArgs.Empty);
    }

    private void OnAddClick(object? sender, EventArgs e)
    {
        using var dialog = new QuestionEditorDialog(_service);
        var owner = FindForm();
        var dialogResult = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (dialogResult != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        EnsureQuestionVisibleInFilter(dialog.Result);
        RefreshGrid(dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu câu hỏi thành công.",
            "Lưu câu hỏi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnEditClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedQuestion();
        if (selected is null)
        {
            return;
        }

        try
        {
            if (_service.IsQuestionUsedInAnyPaper(selected.Id))
            {
                MessageBox.Show(
                    this,
                    "Câu hỏi này đã được gắn vào đề thi, không thể chỉnh sửa để bảo toàn đề thi.",
                    "Không thể sửa câu hỏi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi kiểm tra trạng thái câu hỏi: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        using var dialog = new QuestionEditorDialog(_service);
        dialog.LoadQuestion(selected);
        var owner = FindForm();
        var dialogResult = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (dialogResult != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        EnsureQuestionVisibleInFilter(dialog.Result);
        RefreshGrid(dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu câu hỏi thành công.",
            "Lưu câu hỏi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void EnsureQuestionVisibleInFilter(Question savedQuestion)
    {
        _suppressFilterEvents = true;

        var selectedSubjectId = (_cboSubject.SelectedItem as FilterOption<int?>)?.Value;
        if (selectedSubjectId.HasValue && selectedSubjectId.Value != savedQuestion.SubjectId)
        {
            _cboSubject.SelectedIndex = 0;
        }

        var selectedChapter = (_cboChapter.SelectedItem as FilterOption<int?>)?.Value;
        PopulateChapterFilterOptions(
            selectedChapter.HasValue && selectedChapter.Value == savedQuestion.Chapter
                ? selectedChapter
                : null);

        var selectedDifficulty = (_cboDifficulty.SelectedItem as FilterOption<string?>)?.Value;
        if (!string.IsNullOrEmpty(selectedDifficulty) &&
            !string.Equals(selectedDifficulty, savedQuestion.Difficulty, StringComparison.OrdinalIgnoreCase))
        {
            _cboDifficulty.SelectedIndex = 0;
        }

        var keyword = _txtSearch.Text.Trim();
        if (!string.IsNullOrEmpty(keyword) &&
            !savedQuestion.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            _txtSearch.Clear();
        }

        _suppressFilterEvents = false;
    }

    private void OnPreviewClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedQuestion();
        if (selected is null)
        {
            return;
        }

        var isLocked = false;
        try
        {
            isLocked = _service.IsQuestionUsedInAnyPaper(selected.Id);
        }
        catch
        {
            isLocked = false;
        }

        using var previewDialog = new QuestionPreviewDialog
        {
            AllowEdit = !isLocked
        };
        previewDialog.ShowQuestion(selected, FormatSubjectCode(selected.SubjectId));

        var owner = FindForm();
        if (owner is not null)
        {
            previewDialog.ShowDialog(owner);
        }
        else
        {
            previewDialog.ShowDialog();
        }

        if (previewDialog.EditRequested)
        {
            OnEditClick(sender, e);
        }
    }

    private void OnGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        OnPreviewClick(sender, EventArgs.Empty);
    }

    private void OnGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && _btnDelete.Enabled)
        {
            e.Handled = true;
            DeleteSelectedQuestion();
        }
    }

    private void OnDeleteClick(object? sender, EventArgs e)
    {
        DeleteSelectedQuestion();
    }

    private void DeleteSelectedQuestion()
    {
        var selected = GetSelectedQuestion();
        if (selected is null)
        {
            return;
        }

        try
        {
            if (_service.IsQuestionUsedInAnyPaper(selected.Id))
            {
                MessageBox.Show(
                    this,
                    "Câu hỏi đã nằm trong đề thi, không thể xóa.",
                    "Không thể xóa câu hỏi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi kiểm tra câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        var shortened = ShortenContent(selected.Content, 90);
        var confirmResult = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn xóa câu hỏi #{selected.Id}?\n\n\"{shortened}\"",
            "Xác nhận xóa câu hỏi",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirmResult != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.DeleteQuestion(selected.Id);
            RefreshGrid();

            MessageBox.Show(
                this,
                "Đã xóa câu hỏi thành công.",
                "Xóa câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private sealed record FilterOption<T>(T Value, string Text)
    {
        public override string ToString() => Text;
    }
}
