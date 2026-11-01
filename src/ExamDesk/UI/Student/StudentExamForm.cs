namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    public StudentExamForm()
    {
        InitializeComponent();
        ShowSessionSelection();
    }

    private void ShowSessionSelection()
    {
        var screen = new SessionSelectionControl();
        screen.InstructionsRequested += (_, _) => ShowInstructions();
        ShowScreen(screen, "Chọn ca thi");
    }

    private void ShowInstructions()
    {
        var screen = new ExamInstructionsControl();
        screen.BackRequested += (_, _) => ShowSessionSelection();
        ShowScreen(screen, "Hướng dẫn trước khi thi");
    }

    // Chỉ thay phần nội dung, thanh tên kỳ thi và sinh viên vẫn giữ nguyên.
    private void ShowScreen(UserControl screen, string title)
    {
        while (_studentContent.Controls.Count > 0)
            _studentContent.Controls[0].Dispose();

        screen.Dock = DockStyle.Fill;
        _studentContent.Controls.Add(screen);
        Text = "UniExam — " + title;
        screen.SelectNextControl(null, true, true, true, false);
    }
}
