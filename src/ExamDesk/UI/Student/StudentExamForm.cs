namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    private StudentPreviewAttempt? _attempt;
    private readonly System.Windows.Forms.Timer _deadlineTimer = new() { Interval = 1000 };
    public StudentExamForm()
    {
        InitializeComponent();
        ShowSessionSelection();
        FormClosing += ConfirmClose;
        FormClosed += (_, _) => _deadlineTimer.Dispose();
        _deadlineTimer.Tick += (_, _) =>
        {
            if (_attempt is { Submitted: false } && DateTime.Now >= _attempt.Deadline)
            {
                foreach (Form dialog in OwnedForms) dialog.Close();
                SubmitAttempt();
            }
        };
        _deadlineTimer.Start();
    }

    private void ShowSessionSelection()
    {
        var screen = new SessionSelectionControl();
        screen.InstructionsRequested += (_, _) => ShowInstructions();
        screen.CloseRequested += (_, _) => Close();
        ShowScreen(screen, "Chọn ca thi");
    }

    private void ShowInstructions()
    {
        var screen = new ExamInstructionsControl();
        screen.BackRequested += (_, _) => ShowSessionSelection();
        screen.StartRequested += (_, _) => { _attempt = new StudentPreviewAttempt(); ShowExam(); };
        ShowScreen(screen, "Hướng dẫn trước khi thi");
    }

    private void ShowExam()
    {
        if (_attempt is null || _attempt.Submitted) return;
        if (DateTime.Now >= _attempt.Deadline) { SubmitAttempt(); return; }
        var screen = new StudentExamControl(_attempt);
        screen.SubmitRequested += (_, _) => ReviewSubmission(screen);
        screen.TimeExpired += (_, _) => SubmitAttempt();
        ShowScreen(screen, "Làm bài");
    }

    private void ReviewSubmission(StudentExamControl exam)
    {
        if (_attempt is null || _attempt.Submitted) return;
        using var dialog = new SubmitReviewDialog(_attempt);
        var result = dialog.ShowDialog(this);
        if (_attempt.Submitted) return;
        if (result == DialogResult.OK) SubmitAttempt();
        else if (dialog.QuestionToReview is int index) exam.GoToQuestion(index);
    }

    private void SubmitAttempt()
    {
        if (_attempt is null || _attempt.Submitted) return;
        // Chốt một lần và hiển thị thông tin kết thúc lượt UI.
        _attempt.Submitted = true;
        var screen = new SubmissionStatusControl();
        screen.SetAttempt(_attempt);
        screen.CloseRequested += (_, _) => Close();
        ShowScreen(screen, "Trạng thái nộp bài");
    }

    private void ConfirmClose(object? sender, FormClosingEventArgs e)
    {
        if (_attempt is null) return;
        e.Cancel = MessageBox.Show(this,
            "Dữ liệu UI chỉ nằm trong cửa sổ này và sẽ mất khi đóng. Bạn muốn đóng?",
            "Đóng lượt thi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes;
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
