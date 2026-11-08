namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    private StudentPreviewAttempt? _attempt;
    private ExamDesk.Models.User? _student;
    private readonly System.Windows.Forms.Timer _deadlineTimer = new() { Interval = 1000 };
    public StudentExamForm()
    {
        InitializeComponent();
        ShowIdentity();
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

    private void ShowIdentity()
    {
        var screen = new StudentIdentityControl();
        _examLabel.Text = "UNIEXAM — SINH VIÊN";
        _studentLabel.Text = "Nhận diện bằng MSSV";
        screen.CloseRequested += (_, _) => Close();
        screen.StudentIdentified += student => ShowRealSessions(student);
        ShowScreen(screen, "Nhận diện sinh viên");
    }
    private void ShowRealSessions(ExamDesk.Models.User student)
    {
        _student = student;
        var screen=new SessionSelectionControl(student);
        screen.CloseRequested+=(_,_)=>ShowIdentity();
        screen.SessionSelected+=(session,paper)=>ShowRealInstructions(student,session,paper);
        _studentLabel.Text=$"{student.FullName} · {student.StudentCode}";
        _examLabel.Text="UNIEXAM — CHỌN CA THI";
        ShowScreen(screen,"Chọn ca thi");
    }

    private void ShowRealInstructions(ExamDesk.Models.User student, ExamDesk.Models.ExamSession session, ExamDesk.Models.ExamPaper paper)
    {
        var screen=new ExamInstructionsControl(student,session,paper);
        screen.BackRequested+=(_,_)=>ShowRealSessions(student);
        screen.StartRequested+=(_,_)=>
        {
            try
            {
                _attempt=new StudentPreviewAttempt(new ExamDesk.Services.StudentDraftService().StartOrResume(session.Id,student));
                _examLabel.Text = _attempt.Title;
                if(_attempt.Draft!.PendingSubmission) SubmitAttempt(); else ShowExam();
            }
            catch(Exception ex) { MessageBox.Show(this,ex.Message,"Không mở được lượt thi"); }
        };
        ShowScreen(screen,"Hướng dẫn trước khi thi");
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
        try { _attempt.Lock(); }
        catch (Exception ex)
        {
            _deadlineTimer.Stop();
            _studentContent.Enabled = false;
            MessageBox.Show(this, "Chưa lưu được trạng thái nộp. Bài vẫn được giữ trong database; đóng và mở lại để thử tiếp.\n" + ex.Message, "Lỗi lưu");
            return;
        }
        _attempt.Submitted = true;
        ShowSubmissionStatus();
    }

    private void ShowSubmissionStatus()
    {
        if (_attempt is not { Submitted: true } attempt) return;
        var screen = new SubmissionStatusControl();
        screen.SetAttempt(attempt);
#if DEBUG
        screen.AddAction(StudentLayout.Button("Xem lại bài đã nộp", (_, _) =>
        {
            var review = new StudentExamControl(attempt);
            review.ExitReviewRequested += (_, _) => ShowSubmissionStatus();
            ShowScreen(review, "Xem lại bài đã nộp");
        }));
#endif
        if (_student is { } student)
        {
            screen.AddAction(StudentLayout.Button("← Quay lại chọn ca", (_, _) =>
            {
                _attempt = null;
                ShowRealSessions(student);
            }, true));
        }
        screen.CloseRequested += (_, _) => Close();
        ShowScreen(screen, "Trạng thái nộp bài");
    }

    private void ConfirmClose(object? sender, FormClosingEventArgs e)
    {
        if (_attempt is null || _attempt.Submitted) return;
        e.Cancel = MessageBox.Show(this,
            "Tiến độ đã lưu. Đóng cửa sổ không dừng đồng hồ; mở lại cùng MSSV và ca để khôi phục. Bạn muốn đóng?",
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
