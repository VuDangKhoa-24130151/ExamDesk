namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    private StudentPreviewAttempt? _attempt;
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
        screen.PreviewRequested += (_, _) => ShowSessionSelection();
        screen.CloseRequested += (_, _) => Close();
        screen.SessionSelected += (student, session, paper) =>
        {
            var info = new UserControl();
            var page = StudentLayout.Page(info, session.SessionName);
            page.Controls.Add(StudentLayout.Label($"{student.FullName} · {student.StudentCode} · Lớp {student.ClassName}"));
            page.Controls.Add(StudentLayout.Label($"Đề {paper.PaperCode} — {paper.Title}\nPhòng {session.RoomName} · {paper.TotalQuestions} câu · {paper.DurationMinutes} phút"));
            page.Controls.Add(StudentLayout.Label("Mỗi câu chọn một đáp án A–D, có thể đổi/bỏ lựa chọn và đánh dấu xem lại. Tiến độ được lưu sau mỗi thay đổi. Khi hết giờ bài sẽ khóa để gửi nộp. Chấm điểm đang chờ tích hợp từ Bạn 2."));
                        var agree = new CheckBox { Text = "Tôi đã đọc hướng dẫn và xác nhận thông tin.", AutoSize = true };
            var start = StudentLayout.Button("Bắt đầu / Khôi phục lượt thi", (_, _) =>
            {
                try
                {
                    _attempt = new StudentPreviewAttempt(new ExamDesk.Services.StudentDraftService().StartOrResume(session.Id, student));
                    if (_attempt.Draft!.PendingSubmission) SubmitAttempt(); else ShowExam();
                }
                catch(Exception ex) { MessageBox.Show(this, ex.Message, "Không mở được lượt thi"); }
            }, true);
            start.Enabled = false;
            agree.CheckedChanged += (_, _) => start.Enabled = agree.Checked;
            page.Controls.Add(agree); page.Controls.Add(start);
            page.Controls.Add(StudentLayout.Button("Quay lại chọn ca", (_, _) => ShowIdentity()));
            ShowScreen(info, "Thông tin ca thi");
        };
        ShowScreen(screen, "Nhận diện sinh viên");
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
        if (_attempt.Draft is not null)
        {
            try { _attempt.Lock(); }
            catch(Exception ex)
            {
                _deadlineTimer.Stop();
                _studentContent.Enabled = false;
                MessageBox.Show(this, "Chưa lưu được trạng thái nộp. Bài vẫn được giữ trong database; đóng và mở lại để thử tiếp.\n" + ex.Message, "Lỗi lưu");
                return;
            }
            _attempt.Submitted = true;
            ShowPendingSubmission();
            return;
        }
        // Chốt một lần và hiển thị thông tin kết thúc lượt UI.
        _attempt.Submitted = true;
        var screen = new SubmissionStatusControl();
        screen.SetAttempt(_attempt);
        screen.CloseRequested += (_, _) => Close();
        ShowScreen(screen, "Trạng thái nộp bài");
    }

    private void ShowPendingSubmission()
    {
        var draft = _attempt!.Draft!;
        var pageControl = new UserControl();
        var page = StudentLayout.Page(pageControl, "Bài đã khóa — chờ chấm/lưu kết quả");
        page.Controls.Add(StudentLayout.Label($"{draft.StudentName} · {draft.StudentCode}\n{draft.Title}\nĐã trả lời {_attempt.Answered}/{draft.Questions.Count} câu. Lựa chọn đã lưu trong SQLite."));
        var status = StudentLayout.Label("Dịch vụ chấm điểm chưa hoàn thiện. Bài được giữ lại để gửi khi tích hợp xong; chưa có điểm chính thức.");
        page.Controls.Add(status);
        page.Controls.Add(StudentLayout.Button("Thử gửi bài", (_, _) =>
        {
            try
            {
                var submission = new ExamDesk.Models.ExamSubmission {
                    SessionId=draft.SessionId, PaperId=draft.PaperId, StudentCode=draft.StudentCode,
                    StudentName=draft.StudentName, ClassName=draft.ClassName, StartedAt=draft.StartedUtc,
                    DeadlineUtc=draft.DeadlineUtc, IsAutoSubmitted=draft.AutoSubmitted,
                    SelectedAnswers=draft.Questions.Select((q,i)=>(q.Id, Choice:draft.Answers[i]))
                        .ToDictionary(x=>x.Id,x=>x.Choice.HasValue ? ((char)('A'+x.Choice.Value)).ToString() : "") };
                var result = new ExamDesk.Services.SubmissionService().GradeAndSaveAttempt(submission);
                status.Text = $"Đã lưu kết quả. Điểm: {result.Score:0.00}";
            }
            catch(NotImplementedException) { status.Text = "Dịch vụ chấm điểm của Bạn 2 chưa triển khai. Bài vẫn được giữ trong database."; }
            catch(Exception ex) { status.Text = "Chưa gửi được bài, có thể thử lại. " + ex.Message; }
        }, true));
        page.Controls.Add(StudentLayout.Button("Đóng cửa sổ", (_, _) => Close()));
        ShowScreen(pageControl, "Chờ gửi bài");
    }

    private void ConfirmClose(object? sender, FormClosingEventArgs e)
    {
        if (_attempt is null) return;
        e.Cancel = MessageBox.Show(this,
            _attempt.Draft is null ? "Bài demo sẽ mất khi đóng. Bạn muốn đóng?" : "Tiến độ đã lưu. Đóng cửa sổ không dừng đồng hồ; mở lại cùng MSSV và ca để khôi phục. Bạn muốn đóng?",
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
