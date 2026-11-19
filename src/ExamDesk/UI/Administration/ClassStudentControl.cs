using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-03 • Bạn 1 phụ trách: Quản lý lớp và sinh viên (kết nối SQLite qua AdministrationService).
public partial class ClassStudentControl : UserControl, IUnsavedChangesGuard
{
    private readonly AdministrationService _service;
    private List<ClassInfo> _classes = [];
    private bool _suppressSelectionEvents;

    // Thêm/sửa lớp và sinh viên thực hiện qua hộp thoại modal ClassEditDialog / StudentEditDialog.
    public bool HasUnsavedChanges => false;

    public bool ConfirmLeave() => true;

    public ClassStudentControl() : this(new AdministrationService())
    {
    }

    public ClassStudentControl(AdministrationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();

        // Đã kết nối SQLite thật: ẩn nhãn dữ liệu minh họa
        _lblDemoBadge.Visible = false;

        RefreshClassesGrid();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.F))
        {
            _txtSearchStudent.Focus();
            _txtSearchStudent.SelectAll();
            return true;
        }

        if (keyData == Keys.Delete && !_txtSearchStudent.Focused)
        {
            if (_dgvStudents.ContainsFocus && _btnDeleteStudent.Enabled)
            {
                DeleteSelectedStudent();
                return true;
            }

            if (_dgvClasses.ContainsFocus && _btnDeleteClass.Enabled)
            {
                DeleteSelectedClass();
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OnStudentSearchTextChanged(object? sender, EventArgs e)
    {
        var currentStudentId = GetSelectedStudent()?.Id;
        RefreshStudentsGrid(preferredStudentId: currentStudentId);
    }

    private void OnClearStudentSearchClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_txtSearchStudent.Text))
        {
            return;
        }

        var currentStudentId = GetSelectedStudent()?.Id;
        _txtSearchStudent.Clear();
        RefreshStudentsGrid(preferredStudentId: currentStudentId);
    }

    private void RefreshClassesGrid(int? preferredClassId = null, int? preferredStudentId = null)
    {
        var targetClassId = preferredClassId ?? GetSelectedClass()?.Id;

        try
        {
            _classes = _service.GetClasses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách lớp học: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            _classes = [];
        }

        _lblClassCount.Text = $"{_classes.Count} lớp";

        _suppressSelectionEvents = true;
        _dgvClasses.SuspendLayout();
        _dgvClasses.Rows.Clear();

        foreach (var cls in _classes)
        {
            var rowIndex = _dgvClasses.Rows.Add(
                cls.Code,
                cls.Name,
                cls.SchoolYear,
                cls.StudentCount);

            var row = _dgvClasses.Rows[rowIndex];
            row.Tag = cls;
            row.Cells[_colClassName.Index].ToolTipText =
                $"{cls.Code} – {cls.Name}\nNiên khóa: {cls.SchoolYear} • Sĩ số: {cls.StudentCount} sinh viên";
        }

        _dgvClasses.ResumeLayout();

        var isEmpty = _classes.Count == 0;
        _lblClassEmptyState.Visible = isEmpty;
        _dgvClasses.Visible = !isEmpty;

        if (!isEmpty)
        {
            var targetRowIndex = 0;
            if (targetClassId.HasValue)
            {
                for (var i = 0; i < _dgvClasses.Rows.Count; i++)
                {
                    if (_dgvClasses.Rows[i].Tag is ClassInfo c && c.Id == targetClassId.Value)
                    {
                        targetRowIndex = i;
                        break;
                    }
                }
            }

            _dgvClasses.ClearSelection();
            _dgvClasses.Rows[targetRowIndex].Selected = true;
            _dgvClasses.CurrentCell = _dgvClasses.Rows[targetRowIndex].Cells[0];
        }

        _suppressSelectionEvents = false;

        RefreshStudentsGrid(preferredStudentId);
        UpdateActionButtonsState();
    }

    private void RefreshStudentsGrid(int? preferredStudentId = null)
    {
        var selectedClass = GetSelectedClass();
        if (selectedClass is null)
        {
            _lblStudentSectionTitle.Text = "Sinh viên lớp (chưa chọn)";
            _toolTip.SetToolTip(_lblStudentSectionTitle, string.Empty);
            _lblStudentCount.Text = "Hiển thị 0 / 0 sinh viên";
            _dgvStudents.Rows.Clear();
            _dgvStudents.Visible = false;
            _lblStudentEmptyState.Text = "Chọn một lớp để xem danh sách sinh viên";
            _lblStudentEmptyState.Visible = true;
            UpdateActionButtonsState();
            return;
        }

        var targetStudentId = preferredStudentId ?? GetSelectedStudent()?.Id;

        _lblStudentSectionTitle.Text = $"Sinh viên lớp {selectedClass.Code}";
        _toolTip.SetToolTip(
            _lblStudentSectionTitle,
            $"{selectedClass.Code} – {selectedClass.Name} ({selectedClass.SchoolYear})");

        var keyword = _txtSearchStudent.Text.Trim();
        List<User> filtered;
        try
        {
            filtered = _service.GetStudentsByClass(selectedClass.Code, keyword);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Lỗi khi tải danh sách sinh viên: {ex.Message}",
                "Lỗi cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            filtered = [];
        }

        var totalInClass = string.IsNullOrEmpty(keyword) ? filtered.Count : selectedClass.StudentCount;
        _lblStudentCount.Text = $"Hiển thị {filtered.Count} / {totalInClass} sinh viên";

        _suppressSelectionEvents = true;
        _dgvStudents.SuspendLayout();
        _dgvStudents.Rows.Clear();

        foreach (var student in filtered)
        {
            var rowIndex = _dgvStudents.Rows.Add(
                student.StudentCode,
                student.FullName,
                student.ClassName);

            var row = _dgvStudents.Rows[rowIndex];
            row.Tag = student;
            row.Cells[_colFullName.Index].ToolTipText =
                $"MSSV: {student.StudentCode}\nHọ và tên: {student.FullName}\nLớp: {student.ClassName}";
        }

        _dgvStudents.ResumeLayout();

        var isEmpty = filtered.Count == 0;
        _lblStudentEmptyState.Text = totalInClass == 0
            ? $"Lớp {selectedClass.Code} chưa có sinh viên nào.\nBấm \"Thêm sinh viên\" để tạo mới."
            : "Không có sinh viên phù hợp. Thử đổi từ khóa tìm kiếm.";
        _lblStudentEmptyState.Visible = isEmpty;
        _dgvStudents.Visible = !isEmpty;

        if (!isEmpty)
        {
            var targetRowIndex = 0;
            if (targetStudentId.HasValue)
            {
                for (var i = 0; i < _dgvStudents.Rows.Count; i++)
                {
                    if (_dgvStudents.Rows[i].Tag is User st && st.Id == targetStudentId.Value)
                    {
                        targetRowIndex = i;
                        break;
                    }
                }
            }

            _dgvStudents.ClearSelection();
            _dgvStudents.Rows[targetRowIndex].Selected = true;
            _dgvStudents.CurrentCell = _dgvStudents.Rows[targetRowIndex].Cells[0];
        }

        _suppressSelectionEvents = false;
        UpdateActionButtonsState();
    }

    private ClassInfo? GetSelectedClass()
    {
        if (!_dgvClasses.Visible || _dgvClasses.SelectedRows.Count == 0)
        {
            return null;
        }

        return _dgvClasses.SelectedRows[0].Tag as ClassInfo;
    }

    private User? GetSelectedStudent()
    {
        if (!_dgvStudents.Visible || _dgvStudents.SelectedRows.Count == 0)
        {
            return null;
        }

        return _dgvStudents.SelectedRows[0].Tag as User;
    }

    private void OnClassGridSelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionEvents)
        {
            return;
        }

        RefreshStudentsGrid();
    }

    private void OnStudentGridSelectionChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionEvents)
        {
            return;
        }

        UpdateActionButtonsState();
    }

    private void UpdateActionButtonsState()
    {
        var hasClass = GetSelectedClass() is not null;
        _btnEditClass.Enabled = hasClass;
        _btnDeleteClass.Enabled = hasClass;
        _btnDeleteClass.ForeColor = hasClass ? UiStyle.ErrorText : UiStyle.TextMuted;

        var hasAnyClass = _classes.Count > 0;
        _btnAddStudent.Enabled = hasAnyClass;
        _txtSearchStudent.Enabled = hasClass;
        _btnClearStudentSearch.Enabled = hasClass;

        var hasStudent = hasClass && GetSelectedStudent() is not null;
        _btnEditStudent.Enabled = hasStudent;
        _btnDeleteStudent.Enabled = hasStudent;
        _btnDeleteStudent.ForeColor = hasStudent ? UiStyle.ErrorText : UiStyle.TextMuted;
    }

    private void OnClassGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        OnEditClassClick(sender, EventArgs.Empty);
    }

    private void OnStudentGridCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        OnEditStudentClick(sender, EventArgs.Empty);
    }

    private void OnClassGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && _btnDeleteClass.Enabled)
        {
            e.Handled = true;
            DeleteSelectedClass();
        }
    }

    private void OnStudentGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && _btnDeleteStudent.Enabled)
        {
            e.Handled = true;
            DeleteSelectedStudent();
        }
    }

    private void OnAddClassClick(object? sender, EventArgs e)
    {
        using var dialog = new ClassEditDialog(_service);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        RefreshClassesGrid(preferredClassId: dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu lớp học thành công.",
            "Lưu lớp học",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnEditClassClick(object? sender, EventArgs e)
    {
        var selected = GetSelectedClass();
        if (selected is null)
        {
            return;
        }

        using var dialog = new ClassEditDialog(_service);
        dialog.LoadClass(selected);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var currentStudentId = GetSelectedStudent()?.Id;
        RefreshClassesGrid(preferredClassId: dialog.Result.Id, preferredStudentId: currentStudentId);

        MessageBox.Show(
            this,
            "Lưu lớp học thành công.",
            "Lưu lớp học",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnDeleteClassClick(object? sender, EventArgs e)
    {
        DeleteSelectedClass();
    }

    private void DeleteSelectedClass()
    {
        var selected = GetSelectedClass();
        if (selected is null)
        {
            return;
        }

        var studentCount = _service.CountStudentsByClass(selected.Code);
        if (studentCount > 0)
        {
            MessageBox.Show(
                this,
                $"Không thể xóa lớp \"{selected.Code} – {selected.Name}\" vì lớp đang có {studentCount} sinh viên.\n\n" +
                $"Cách xử lý: Chuyển sang lớp khác hoặc xóa {studentCount} sinh viên của lớp này trước khi xóa lớp.",
                "Không thể xóa lớp học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn xóa lớp \"{selected.Code} – {selected.Name}\" ({selected.SchoolYear})?",
            "Xác nhận xóa lớp học",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.DeleteClass(selected.Id);
            RefreshClassesGrid();

            MessageBox.Show(
                this,
                "Đã xóa lớp học thành công.",
                "Xóa lớp học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa lớp học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void OnAddStudentClick(object? sender, EventArgs e)
    {
        if (_classes.Count == 0)
        {
            return;
        }

        var selectedClass = GetSelectedClass();

        using var dialog = new StudentEditDialog(_service);
        dialog.InitializeForAdd(selectedClass?.Code);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var targetClass = _classes.FirstOrDefault(c =>
            string.Equals(c.Code, dialog.Result.ClassName, StringComparison.OrdinalIgnoreCase));

        EnsureStudentVisibleInSearch(dialog.Result);
        RefreshClassesGrid(
            preferredClassId: targetClass?.Id ?? selectedClass?.Id,
            preferredStudentId: dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu sinh viên thành công.",
            "Lưu sinh viên",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnEditStudentClick(object? sender, EventArgs e)
    {
        var selectedStudent = GetSelectedStudent();
        if (selectedStudent is null)
        {
            return;
        }

        using var dialog = new StudentEditDialog(_service);
        dialog.LoadStudent(selectedStudent);
        var owner = FindForm();
        var result = owner is not null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
        if (result != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        var targetClass = _classes.FirstOrDefault(c =>
            string.Equals(c.Code, dialog.Result.ClassName, StringComparison.OrdinalIgnoreCase));

        EnsureStudentVisibleInSearch(dialog.Result);
        RefreshClassesGrid(
            preferredClassId: targetClass?.Id,
            preferredStudentId: dialog.Result.Id);

        MessageBox.Show(
            this,
            "Lưu sinh viên thành công.",
            "Lưu sinh viên",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void EnsureStudentVisibleInSearch(User savedStudent)
    {
        var keyword = _txtSearchStudent.Text.Trim();
        if (!string.IsNullOrEmpty(keyword) &&
            !savedStudent.StudentCode.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
            !savedStudent.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            _txtSearchStudent.Clear();
        }
    }

    private void OnDeleteStudentClick(object? sender, EventArgs e)
    {
        DeleteSelectedStudent();
    }

    private void DeleteSelectedStudent()
    {
        var selectedClass = GetSelectedClass();
        var selectedStudent = GetSelectedStudent();
        if (selectedStudent is null)
        {
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn xóa sinh viên {selectedStudent.StudentCode} – {selectedStudent.FullName} (Lớp {selectedStudent.ClassName})?",
            "Xác nhận xóa sinh viên",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.DeleteStudent(selectedStudent.Id);
            RefreshClassesGrid(preferredClassId: selectedClass?.Id);

            MessageBox.Show(
                this,
                "Đã xóa sinh viên thành công.",
                "Xóa sinh viên",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa sinh viên",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
