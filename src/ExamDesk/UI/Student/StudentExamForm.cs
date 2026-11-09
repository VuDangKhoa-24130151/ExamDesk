namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    private StudentAttemptState? _attempt;
    private ExamDesk.Models.User? _student;
    private bool _submitting;
    private bool _submitAfterSave;
    private readonly System.Windows.Forms.Timer _deadlineTimer = new() { Interval = 1000 };
    public StudentExamForm()
    {
        InitializeComponent();
        ShowIdentity();
        FormClosing += ConfirmClose;
        FormClosed += (_, _) => _deadlineTimer.Dispose();
        _deadlineTimer.Tick += (_, _) => TrySubmitExpiredAttempt();
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
        screen.ResultSelected += ShowResult;
        _studentLabel.Text=$"{student.FullName} · {student.StudentCode}";
        _examLabel.Text="UNIEXAM — CHỌN CA THI";
        ShowScreen(screen,"Chọn ca thi");
    }

    private void ShowRealInstructions(ExamDesk.Models.User student, ExamDesk.Models.ExamSession session, ExamDesk.Models.ExamPaper paper)
    {
        var draft = new ExamDesk.Services.StudentDraftService().FindDraft(session.Id, student.StudentCode);
        var screen = new ExamInstructionsControl(student, session, paper, draft);
        screen.BackRequested+=(_,_)=>ShowRealSessions(student);
        screen.StartRequested+=(_,_)=>
        {
            try
            {
                _attempt=new StudentAttemptState(new ExamDesk.Services.StudentDraftService().StartOrResume(session.Id, student), screen.ResumeClock);
                _examLabel.Text = _attempt.Title;
                // Hoàn tất lượt mà phiên bản cũ chỉ lưu trạng thái khóa.
                if (_attempt.Draft!.PendingSubmission)
                    SubmitAttempt();
                else
                    ShowExam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, StudentError.DescribeStart(ex,
                    "Không mở được lượt thi. Hãy quay lại, tải lại ca thi hoặc liên hệ giảng viên."),
                    "Không mở được lượt thi");
            }
        };
        ShowScreen(screen,"Hướng dẫn trước khi thi");
    }
    private void ShowExam()
    {
        if (_attempt is null || _attempt.Submitted) return;
        if (TrySubmitExpiredAttempt()) return;
        var screen = new StudentExamControl(_attempt);
        screen.SubmitRequested += (_, _) => ReviewSubmission(screen);
        screen.SaveExpired += (_, _) => TrySubmitExpiredAttempt(expiredDuringSave: true);
        screen.SaveCompleted += (_, _) => TrySubmitExpiredAttempt();
        screen.DraftLocked += (_, _) => SubmitAttempt();
        ShowScreen(screen, "Làm bài");
        _deadlineTimer.Start();
    }

    private bool TrySubmitExpiredAttempt(bool expiredDuringSave = false)
    {
        if (_submitting || _attempt is not { Submitted: false } attempt) return false;
        var exam = _studentContent.Controls.OfType<StudentExamControl>().FirstOrDefault();
        if (!_submitAfterSave && !expiredDuringSave && !(exam?.IsDeadlineReached ?? attempt.IsExpired))
            return false;

        if (exam is { IsSaving: true })
        {
            if (!_submitAfterSave) exam.ShowDeadlinePending();
            _submitAfterSave = true;
            return true;
        }

        foreach (Form dialog in OwnedForms) dialog.Close();
        SubmitAttempt();
        return true;
    }

    private void ReviewSubmission(StudentExamControl exam)
    {
        if (_attempt is null || _attempt.Submitted) return;
        if (TrySubmitExpiredAttempt() || exam.IsSaving) return;
        using var dialog = new SubmitReviewDialog(_attempt);
        var result = dialog.ShowDialog(this);
        if (_attempt.Submitted || exam.IsDisposed) return;
        if (result == DialogResult.OK) SubmitAttempt();
        else if (dialog.QuestionToReview is int index) exam.GoToQuestion(index);
    }

    private async void SubmitAttempt()
    {
        if (_submitting || _attempt is not { } attempt) return;
        _submitting = true;
        _submitAfterSave = false;
        _deadlineTimer.Stop();
        var (status, retry) = ShowSubmissionStatus(busy: true);
        UseWaitCursor = true;
        try
        {
            var result = await Task.Run(() =>
            {
                if (!attempt.Submitted) attempt.Submit();
                return attempt.Draft is { } draft ? new ExamDesk.Services.StudentExamService().GetResult(draft) : null;
            });
            if (IsDisposed) return;
            if (result is not null)
                ShowResult(result);
            else
                status.SetAttempt(attempt, "Chưa tìm thấy kết quả. Hãy tải lại kết quả hoặc liên hệ giảng viên.");
        }
        catch (Exception ex)
        {
            var error = StudentError.Describe(ex,
                attempt.Submitted
                    ? "Không tải được kết quả đã lưu. Hãy tải lại kết quả hoặc liên hệ giảng viên."
                    : "Không lưu được bài nộp. Hãy thử gửi lại hoặc liên hệ giảng viên.");
            if (!IsDisposed) status.SetAttempt(attempt, error);
        }
        finally
        {
            _submitting = false;
            if (!IsDisposed) UseWaitCursor = false;
            if (!status.IsDisposed)
            {
                if (attempt.Submitted && retry is not null) retry.Text = "Tải lại kết quả";
                status.SetBusy(false);
            }
        }
    }

    private void ShowResult(ExamDesk.Repositories.StudentResult result)
    {
        var screen = new StudentResultControl(result);
        screen.CloseRequested += (_, _) => Close();
        screen.BackRequested += (_, _) =>
        {
            _attempt = null;
            if (_student is { } student) ShowRealSessions(student);
        };
        ShowScreen(screen, "Kết quả bài thi");
    }

    private (SubmissionStatusControl Screen, Button? Retry) ShowSubmissionStatus(bool busy = false)
    {
        var attempt = _attempt!;
        var screen = new SubmissionStatusControl();
        screen.SetAttempt(attempt, null);
        Button? retry = null;
        if (attempt.Draft is not null)
        {
            retry = StudentLayout.Button(attempt.Submitted ? "Tải lại kết quả" : "Thử gửi lại",
                (_, _) => SubmitAttempt(), true);
            screen.AddAction(retry);
        }
#if DEBUG
        if (attempt.Submitted)
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
        screen.SetBusy(busy);
        ShowScreen(screen, "Trạng thái nộp bài");
        return (screen, retry);
    }

    private void ConfirmClose(object? sender, FormClosingEventArgs e)
    {
        if (_submitting || _studentContent.Controls.OfType<StudentExamControl>().Any(screen => screen.IsSaving))
        {
            e.Cancel = true;
            return;
        }
        if (_attempt is null || _attempt.Submitted || _attempt.Draft?.PendingSubmission == true) return;
        e.Cancel = MessageBox.Show(this,
            "Đóng cửa sổ không dừng đồng hồ. Mở lại cùng MSSV và ca để tiếp tục từ tiến độ đã lưu. Bạn muốn đóng?",
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
        if (screen is StudentExamControl exam) exam.FocusInitialControl();
        else screen.SelectNextControl(null, true, true, true, false);
    }
}
