using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// Bạn 1: phát triển giao diện trong control này; dữ liệu minh họa riêng của phân hệ.
public partial class QuestionBankControl : UserControl
{
    private bool _suppressFilterEvents;

    public QuestionBankControl()
    {
        InitializeComponent();
        InitializeFilterOptions();
        RefreshGrid();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.F))
        {
            _txtSearch.Focus();
            _txtSearch.SelectAll();
            return true;
        }

        if (keyData == Keys.Delete && !_txtSearch.Focused && _btnDelete.Enabled)
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
        foreach (var subject in AdminPreviewData.Subjects)
        {
            _cboSubject.Items.Add(new FilterOption<int?>(subject.Id, $"{subject.Code} – {subject.Name}"));
        }
        _cboSubject.SelectedIndex = 0;

        _cboChapter.Items.Clear();
        _cboChapter.Items.Add(new FilterOption<int?>(null, "Tất cả"));
        for (var ch = 1; ch <= 4; ch++)
        {
            _cboChapter.Items.Add(new FilterOption<int?>(ch, $"Chương {ch}"));
        }
        _cboChapter.SelectedIndex = 0;

        _cboDifficulty.Items.Clear();
        _cboDifficulty.Items.Add(new FilterOption<string?>(null, "Tất cả"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Easy", "Dễ"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Medium", "Trung bình"));
        _cboDifficulty.Items.Add(new FilterOption<string?>("Hard", "Khó"));
        _cboDifficulty.SelectedIndex = 0;

        _suppressFilterEvents = false;
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        if (_suppressFilterEvents)
        {
            return;
        }

        RefreshGrid();
    }

    private void OnClearFilterClick(object? sender, EventArgs e)
    {
        _suppressFilterEvents = true;
        _cboSubject.SelectedIndex = 0;
        _cboChapter.SelectedIndex = 0;
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

        IEnumerable<Question> query = AdminPreviewData.Questions;

        if (selectedSubjectId.HasValue)
        {
            query = query.Where(q => q.SubjectId == selectedSubjectId.Value);
        }

        if (selectedChapter.HasValue)
        {
            query = query.Where(q => q.Chapter == selectedChapter.Value);
        }

        if (!string.IsNullOrEmpty(selectedDifficulty))
        {
            query = query.Where(q => string.Equals(q.Difficulty, selectedDifficulty, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(q => q.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query.ToList();
        var totalCount = AdminPreviewData.Questions.Count;
        _lblCount.Text = $"Hiển thị {filtered.Count} / {totalCount} câu";

        _dgvQuestions.SuspendLayout();
        _dgvQuestions.Rows.Clear();

        foreach (var question in filtered)
        {
            var rowIndex = _dgvQuestions.Rows.Add(
                $"#{question.Id}",
                AdminPreviewData.FormatSubjectCode(question.SubjectId),
                $"Chương {question.Chapter}",
                AdminPreviewData.FormatDifficulty(question.Difficulty),
                AdminPreviewData.ShortenContent(question.Content),
                question.CorrectOption);

            var row = _dgvQuestions.Rows[rowIndex];
            row.Tag = question;

            var fullTooltip =
                $"[#{question.Id}] {AdminPreviewData.FormatSubjectCode(question.SubjectId)} • Chương {question.Chapter} • {AdminPreviewData.FormatDifficulty(question.Difficulty)}\n\n" +
                $"{question.Content}\n\n" +
                $"A. {question.OptionA}\n" +
                $"B. {question.OptionB}\n" +
                $"C. {question.OptionC}\n" +
                $"D. {question.OptionD}\n" +
                $"Đáp án đúng: {question.CorrectOption}";

            row.Cells[_colContent.Index].ToolTipText = fullTooltip;
        }

        _dgvQuestions.ResumeLayout();

        var isEmpty = filtered.Count == 0;
        _lblEmptyState.Visible = isEmpty;
        _dgvQuestions.Visible = !isEmpty;

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

    private void OnAddClick(object? sender, EventArgs e)
    {
        using var dialog = new QuestionEditorDialog();
        var owner = FindForm();
        var dialogResult = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (dialogResult != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var nextId = AdminPreviewData.Questions.Count > 0
            ? AdminPreviewData.Questions.Max(q => q.Id) + 1
            : 101;

        dialog.Result.Id = nextId;
        AdminPreviewData.Questions.Add(dialog.Result);
        EnsureQuestionVisibleInFilter(dialog.Result);
        RefreshGrid(nextId);

        MessageBox.Show(
            this,
            "Đã lưu (minh họa – chưa lưu CSDL)",
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

        using var dialog = new QuestionEditorDialog();
        dialog.LoadQuestion(selected);
        var owner = FindForm();
        var dialogResult = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (dialogResult != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var index = AdminPreviewData.Questions.FindIndex(q => q.Id == selected.Id);
        if (index >= 0)
        {
            AdminPreviewData.Questions[index] = dialog.Result;
        }

        EnsureQuestionVisibleInFilter(dialog.Result);
        RefreshGrid(dialog.Result.Id);

        MessageBox.Show(
            this,
            "Đã lưu (minh họa – chưa lưu CSDL)",
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
        if (selectedChapter.HasValue && selectedChapter.Value != savedQuestion.Chapter)
        {
            _cboChapter.SelectedIndex = 0;
        }

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

        using var previewDialog = new QuestionPreviewDialog
        {
            AllowEdit = true
        };
        previewDialog.ShowQuestion(selected, AdminPreviewData.FormatSubjectCode(selected.SubjectId));

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

        var shortened = AdminPreviewData.ShortenContent(selected.Content, 90);
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

        AdminPreviewData.Questions.Remove(selected);
        RefreshGrid();

        MessageBox.Show(
            this,
            "Đã xóa (minh họa – chưa lưu CSDL)",
            "Xóa câu hỏi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private sealed record FilterOption<T>(T Value, string Text)
    {
        public override string ToString() => Text;
    }
}
