using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-02 • Bạn 1 phụ trách: Quản lý môn và chương (kết nối SQLite qua QuestionBankService).
public partial class SubjectChapterControl : UserControl, IUnsavedChangesGuard
{
    private readonly QuestionBankService _service;
    private bool _suppressSelectionEvents;

    // Thêm/sửa môn và chương thực hiện qua hộp thoại modal SubjectEditDialog / ChapterEditDialog.
    public bool HasUnsavedChanges => false;

    public bool ConfirmLeave() => true;

    public SubjectChapterControl() : this(new QuestionBankService())
    {
    }

    public SubjectChapterControl(QuestionBankService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();

        // Đã kết nối SQLite thật: ẩn nhãn dữ liệu minh họa
        _lblDemoBadge.Visible = false;

        RefreshSubjectsGrid();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.F))
        {
            _txtSearchSubject.Focus();
            _txtSearchSubject.SelectAll();
            return true;
        }

        if (keyData == Keys.Delete && !_txtSearchSubject.Focused)
        {
            if (_dgvChapters.ContainsFocus && _btnDeleteChapter.Enabled)
            {
                DeleteSelectedChapter();
                return true;
            }

            if (_dgvSubjects.ContainsFocus && _btnDeleteSubject.Enabled)
            {
                DeleteSelectedSubject();
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OnSubjectSearchTextChanged(object? sender, EventArgs e)
    {
        var currentSubjectId = GetSelectedSubject()?.Id;
        RefreshSubjectsGrid(preferredSubjectId: currentSubjectId);
    }

    private void OnClearSubjectSearchClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_txtSearchSubject.Text))
        {
            return;
        }

        var currentSubjectId = GetSelectedSubject()?.Id;
        _txtSearchSubject.Clear();
        RefreshSubjectsGrid(preferredSubjectId: currentSubjectId);
    }

    private void RefreshSubjectsGrid(int? preferredSubjectId = null, int? preferredChapterNumber = null)
    {
        var targetSubjectId = preferredSubjectId ?? GetSelectedSubject()?.Id;
        var keyword = _txtSearchSubject.Text.Trim();

        List<Subject> allSubjects;
        try
        {
            allSubjects = _service.GetSubjects();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách môn học: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            allSubjects = [];
        }

        IEnumerable<Subject> query = allSubjects;
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s =>
                s.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query.ToList();
        var totalSubjects = allSubjects.Count;
        _lblSubjectCount.Text = $"Hiển thị {filtered.Count} / {totalSubjects} môn";

        _suppressSelectionEvents = true;
        _dgvSubjects.SuspendLayout();
        _dgvSubjects.Rows.Clear();

        foreach (var subject in filtered)
        {
            var rowIndex = _dgvSubjects.Rows.Add(
                subject.Code,
                subject.Name,
                subject.ChapterCount,
                subject.QuestionCount);

            var row = _dgvSubjects.Rows[rowIndex];
            row.Tag = subject;
            row.Cells[_colSubjectName.Index].ToolTipText =
                $"{subject.Code} – {subject.Name}\nSố chương: {subject.ChapterCount} • Số câu hỏi: {subject.QuestionCount}";
        }

        _dgvSubjects.ResumeLayout();

        var isEmpty = filtered.Count == 0;
        _lblSubjectEmptyState.Text = totalSubjects == 0
            ? "Chưa có môn học nào. Bấm \"Thêm môn\" để tạo mới."
            : "Không có môn học phù hợp. Thử đổi từ khóa tìm kiếm.";
        _lblSubjectEmptyState.Visible = isEmpty;
        _dgvSubjects.Visible = !isEmpty;

        if (!isEmpty)
        {
            var targetRowIndex = 0;
            if (targetSubjectId.HasValue)
            {
                for (var i = 0; i < _dgvSubjects.Rows.Count; i++)
                {
                    if (_dgvSubjects.Rows[i].Tag is Subject s && s.Id == targetSubjectId.Value)
                    {
                        targetRowIndex = i;
                        break;
                    }
                }
            }

            _dgvSubjects.ClearSelection();
            _dgvSubjects.Rows[targetRowIndex].Selected = true;
            _dgvSubjects.CurrentCell = _dgvSubjects.Rows[targetRowIndex].Cells[0];
        }

        _suppressSelectionEvents = false;

        RefreshChaptersGrid(preferredChapterNumber);
        UpdateActionButtonsState();
    }

    private void RefreshChaptersGrid(int? preferredChapterNumber = null)
    {
        var selectedSubject = GetSelectedSubject();
        if (selectedSubject is null)
        {
            _lblChapterSectionTitle.Text = "Chương của môn: (chưa chọn)";
            _lblChapterCount.Text = "0 chương";
            _dgvChapters.Rows.Clear();
            _dgvChapters.Visible = false;
            _lblChapterEmptyState.Text = "Chọn một môn để xem chương";
            _lblChapterEmptyState.Visible = true;
            UpdateActionButtonsState();
            return;
        }

        var targetChapterNumber = preferredChapterNumber ?? GetSelectedChapter()?.Number;

        _lblChapterSectionTitle.Text = $"Chương của môn: {selectedSubject.Code} – {selectedSubject.Name}";

        List<ChapterInfo> chapters;
        try
        {
            chapters = _service.GetChaptersBySubject(selectedSubject.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách chương: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            chapters = [];
        }

        var totalSubjectQuestions = chapters.Sum(c => c.QuestionCount);
        _lblChapterCount.Text = $"{chapters.Count} chương · {totalSubjectQuestions} câu hỏi";

        _suppressSelectionEvents = true;
        _dgvChapters.SuspendLayout();
        _dgvChapters.Rows.Clear();

        foreach (var chapter in chapters)
        {
            var rowIndex = _dgvChapters.Rows.Add(
                $"Chương {chapter.Number}",
                chapter.Name,
                chapter.QuestionCount);

            var row = _dgvChapters.Rows[rowIndex];
            row.Tag = chapter;
            row.Cells[_colChapterName.Index].ToolTipText =
                $"Chương {chapter.Number}: {chapter.Name}\nSố câu hỏi hiện có: {chapter.QuestionCount}";
        }

        _dgvChapters.ResumeLayout();

        var isEmpty = chapters.Count == 0;
        _lblChapterEmptyState.Text =
            $"Môn \"{selectedSubject.Code} – {selectedSubject.Name}\" chưa có chương nào.\nBấm \"Thêm chương\" để tạo mới.";
        _lblChapterEmptyState.Visible = isEmpty;
        _dgvChapters.Visible = !isEmpty;

        if (!isEmpty)
        {
            var targetRowIndex = 0;
            if (targetChapterNumber.HasValue)
            {
                for (var i = 0; i < _dgvChapters.Rows.Count; i++)
                {
                    if (_dgvChapters.Rows[i].Tag is ChapterInfo ch && ch.Number == targetChapterNumber.Value)
                    {
                        targetRowIndex = i;
                        break;
                    }
                }
            }

            _dgvChapters.ClearSelection();
            _dgvChapters.Rows[targetRowIndex].Selected = true;
            _dgvChapters.CurrentCell = _dgvChapters.Rows[targetRowIndex].Cells[0];
        }

        _suppressSelectionEvents = false;
        UpdateActionButtonsState();
    }

    private Subject? GetSelectedSubject()
    {
        if (!_dgvSubjects.Visible || _dgvSubjects.SelectedRows.Count == 0)
        {
            return null;
        }

        return _dgvSubjects.SelectedRows[0].Tag as Subject;
    }

    private ChapterInfo? GetSelectedChapter()
    {
        if (!_dgvChapters.Visible || _dgvChapters.SelectedRows.Count == 0)
        {
            return null;
        }

        return _dgvChapters.SelectedRows[0].Tag as ChapterInfo;
    }

    private void OnSubjectGridSelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionEvents)
        {
            return;
        }

        RefreshChaptersGrid();
    }

    private void OnChapterGridSelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionEvents)
        {
            return;
        }

        UpdateActionButtonsState();
    }

    private void UpdateActionButtonsState()
    {
        var hasSubject = GetSelectedSubject() is not null;
        _btnEditSubject.Enabled = hasSubject;
        _btnDeleteSubject.Enabled = hasSubject;
        _btnDeleteSubject.ForeColor = hasSubject ? UiStyle.ErrorText : UiStyle.TextMuted;

        _btnAddChapter.Enabled = hasSubject;

        var hasChapter = hasSubject && GetSelectedChapter() is not null;
        _btnEditChapter.Enabled = hasChapter;
        _btnDeleteChapter.Enabled = hasChapter;
        _btnDeleteChapter.ForeColor = hasChapter ? UiStyle.ErrorText : UiStyle.TextMuted;
    }

    private void OnSubjectGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        OnEditSubjectClick(sender, EventArgs.Empty);
    }

    private void OnChapterGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        OnEditChapterClick(sender, EventArgs.Empty);
    }

    private void OnSubjectGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && _btnDeleteSubject.Enabled)
        {
            e.Handled = true;
            DeleteSelectedSubject();
        }
    }

    private void OnChapterGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && _btnDeleteChapter.Enabled)
        {
            e.Handled = true;
            DeleteSelectedChapter();
        }
    }

    private void OnAddSubjectClick(object? sender, EventArgs e)
    {
        using var dialog = new SubjectEditDialog(_service);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        EnsureSubjectVisibleInSearch(dialog.Result);
        RefreshSubjectsGrid(preferredSubjectId: dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu môn học thành công.",
            "Lưu môn học",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnEditSubjectClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedSubject();
        if (selected is null)
        {
            return;
        }

        using var dialog = new SubjectEditDialog(_service);
        dialog.LoadSubject(selected);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var currentChapterNumber = GetSelectedChapter()?.Number;
        EnsureSubjectVisibleInSearch(dialog.Result);
        RefreshSubjectsGrid(preferredSubjectId: dialog.Result.Id, preferredChapterNumber: currentChapterNumber);

        MessageBox.Show(
            this,
            "Lưu môn học thành công.",
            "Lưu môn học",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void EnsureSubjectVisibleInSearch(Subject savedSubject)
    {
        var keyword = _txtSearchSubject.Text.Trim();
        if (!string.IsNullOrEmpty(keyword) &&
            !savedSubject.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
            !savedSubject.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            _txtSearchSubject.Clear();
        }
    }

    private void OnDeleteSubjectClick(object? sender, EventArgs e)
    {
        DeleteSelectedSubject();
    }

    private void DeleteSelectedSubject()
    {
        var selected = GetSelectedSubject();
        if (selected is null)
        {
            return;
        }

        var chapterCount = selected.ChapterCount;
        var questionCount = selected.QuestionCount;

        if (questionCount > 0 || chapterCount > 0)
        {
            string message;
            if (questionCount > 0 && chapterCount > 0)
            {
                message =
                    $"Không thể xóa môn \"{selected.Code} – {selected.Name}\" vì đang có {chapterCount} chương và {questionCount} câu hỏi.\n\n" +
                    $"Cách xử lý: Chuyển hoặc xóa {questionCount} câu hỏi trong Ngân hàng câu hỏi trước, sau đó xóa {chapterCount} chương của môn học này.";
            }
            else if (questionCount > 0)
            {
                message =
                    $"Không thể xóa môn \"{selected.Code} – {selected.Name}\" vì đang có {questionCount} câu hỏi.\n\n" +
                    $"Cách xử lý: Chuyển hoặc xóa {questionCount} câu hỏi trong Ngân hàng câu hỏi trước.";
            }
            else
            {
                message =
                    $"Không thể xóa môn \"{selected.Code} – {selected.Name}\" vì đang có {chapterCount} chương.\n\n" +
                    $"Cách xử lý: Xóa {chapterCount} chương của môn học này ở bảng bên phải trước khi xóa môn.";
            }

            MessageBox.Show(
                this,
                message,
                "Không thể xóa môn học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn xóa môn \"{selected.Code} – {selected.Name}\"?",
            "Xác nhận xóa môn học",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.DeleteSubject(selected.Id);
            RefreshSubjectsGrid();

            MessageBox.Show(
                this,
                "Đã xóa môn học thành công.",
                "Xóa môn học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa môn học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void OnAddChapterClick(object? sender, EventArgs e)
    {
        var selectedSubject = GetSelectedSubject();
        if (selectedSubject is null)
        {
            return;
        }

        using var dialog = new ChapterEditDialog(_service);
        dialog.InitializeForAdd(selectedSubject);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        RefreshSubjectsGrid(
            preferredSubjectId: selectedSubject.Id,
            preferredChapterNumber: dialog.Result.Number);

        MessageBox.Show(
            this,
            "Lưu chương thành công.",
            "Lưu chương",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnEditChapterClick(object? sender, EventArgs e)
    {
        var selectedSubject = GetSelectedSubject();
        var selectedChapter = GetSelectedChapter();
        if (selectedSubject is null || selectedChapter is null)
        {
            return;
        }

        using var dialog = new ChapterEditDialog(_service);
        dialog.LoadChapter(selectedSubject, selectedChapter);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        RefreshSubjectsGrid(
            preferredSubjectId: selectedSubject.Id,
            preferredChapterNumber: dialog.Result.Number);

        MessageBox.Show(
            this,
            "Lưu chương thành công.",
            "Lưu chương",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnDeleteChapterClick(object? sender, EventArgs e)
    {
        DeleteSelectedChapter();
    }

    private void DeleteSelectedChapter()
    {
        var selectedSubject = GetSelectedSubject();
        var selectedChapter = GetSelectedChapter();
        if (selectedSubject is null || selectedChapter is null)
        {
            return;
        }

        var questionCount = selectedChapter.QuestionCount;
        if (questionCount > 0)
        {
            MessageBox.Show(
                this,
                $"Không thể xóa Chương {selectedChapter.Number} – \"{selectedChapter.Name}\" của môn \"{selectedSubject.Code}\" vì đang có {questionCount} câu hỏi.\n\n" +
                $"Cách xử lý: Chuyển hoặc xóa {questionCount} câu hỏi trong Ngân hàng câu hỏi trước.",
                "Không thể xóa chương",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn xóa Chương {selectedChapter.Number} – \"{selectedChapter.Name}\" của môn \"{selectedSubject.Code} – {selectedSubject.Name}\"?",
            "Xác nhận xóa chương",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.DeleteChapter(selectedSubject.Id, selectedChapter.Number);
            RefreshSubjectsGrid(preferredSubjectId: selectedSubject.Id);

            MessageBox.Show(
                this,
                "Đã xóa chương thành công.",
                "Xóa chương",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa chương",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
