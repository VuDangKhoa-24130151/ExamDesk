namespace ExamDesk.UI.Student;

// Bạn 3: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class StudentExamForm : Form
{
    private const int WsExComposited = 0x02000000;
    private static readonly bool UseCompositedPainting =
        Environment.GetEnvironmentVariable("UNIEXAM_STUDENT_COMPOSITED") == "1";

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            // Bật riêng khi so sánh vẽ lại của TextBox và thanh marquee.
            if (UseCompositedPainting) parameters.ExStyle |= WsExComposited;
            return parameters;
        }
    }

    private StudentAttemptState? _attempt;
    private ExamDesk.Models.User? _student;
    private bool _submitting;
    private bool _submitAfterSave;
    private bool _handlingExpiredAttempt;
    private ExamViolationMonitor? _violationMonitor;
    private ExamViolationDialog? _violationDialog;
    private bool _violationSubmitPending;
    private readonly System.Windows.Forms.Timer _deadlineTimer = new() { Interval = 1000 };
    public StudentExamForm()
    {
        DoubleBuffered = true;
        InitializeComponent();
        ApplyCompactHeader();
        DpiChanged += (_, _) => ApplyCompactHeader();
        if (_examLabel.Parent is Control header)
            header.Paint += (_, e) => StudentLayout.DrawHeaderBorder(header, e.Graphics);
        ShowIdentity();
        FormClosing += ConfirmClose;
        FormClosed += (_, _) => _deadlineTimer.Dispose();
        _deadlineTimer.Tick += (_, _) => TrySubmitExpiredAttempt();
    }

    private void ApplyCompactHeader()
    {
        int S(int value) => StudentLayout.Scale(this, value);
        if (_examLabel.Parent is not Control header) return;
        header.Height = S(StudentLayout.BarHeight);
        header.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space8),
            S(StudentLayout.Space24), S(StudentLayout.Space8));
        _examLabel.Margin = new Padding(0, 0, S(StudentLayout.Space16), 0);
    }

    private void ShowIdentity()
    {
        _student = null;
        var screen = new StudentIdentityControl();
        _examLabel.Text = "UniExam";
        _studentLabel.Text = string.Empty;
        screen.CloseRequested += (_, _) => Close();
        screen.StudentIdentified += ShowProfileConfirmation;
        ShowScreen(screen, "Nhập mã sinh viên");
    }
    private void ShowProfileConfirmation(ExamDesk.Models.User student)
    {
        var screen = new StudentProfileConfirmationControl(student);
        screen.Confirmed += (_, _) => ShowRealSessions(student);
        screen.RetryRequested += (_, _) => ShowIdentity();
        ShowScreen(screen, "Xác nhận hồ sơ sinh viên");
    }

    private void ShowRealSessions(ExamDesk.Models.User student)
    {
        _student = student;
        var screen=new SessionSelectionControl(student);
        screen.CloseRequested+=(_,_)=>ShowIdentity();
        screen.SessionSelected+=(session,paper)=>ShowRealInstructions(student,session,paper);
        screen.ResultSelected += ShowResult;
        _studentLabel.Text=$"{student.FullName} · {student.StudentCode}";
        _examLabel.Text="UniExam";
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
        _violationMonitor = new ExamViolationMonitor(this, ReportViolation);
        var screen = new StudentExamControl(_attempt);
        screen.SubmitRequested += (_, _) => ReviewSubmission(screen);
        screen.SaveExpired += (_, _) => TrySubmitExpiredAttempt(expiredDuringSave: true);
        screen.SaveCompleted += (_, _) =>
        {
            if (_violationSubmitPending) SubmitForViolation();
            else TrySubmitExpiredAttempt();
        };
        screen.DraftLocked += (_, _) => SubmitAttempt();
        ShowScreen(screen, "Làm bài");
        _deadlineTimer.Start();
    }

    private void ReportViolation(string reason)
    {
        if (_submitting || _handlingExpiredAttempt || _violationSubmitPending ||
            _violationDialog is not null || _attempt is not { Submitted: false } attempt ||
            _studentContent.Controls.OfType<StudentExamControl>().FirstOrDefault() is not { } exam)
            return;
        if (TrySubmitExpiredAttempt()) return;
        attempt.ViolationCount++;
        _violationSubmitPending = attempt.ViolationCount >= 3;

        // Không dùng MessageBox: giữ timer chạy và có thể đóng cảnh cáo khi hết giờ.
        exam.Enabled = false;
        foreach (Form dialog in OwnedForms) dialog.Close();
        var warning = new ExamViolationDialog(attempt.ViolationCount, reason);
        _violationDialog = warning;
        warning.FormClosed += (_, _) =>
        {
            _violationDialog = null;
            if (_violationSubmitPending && warning.Acknowledged)
                SubmitForViolation();
            else if (!exam.IsDisposed && !_violationSubmitPending && !_submitting)
                exam.Enabled = true;
        };
        warning.Show(this);
    }

    private void SubmitForViolation()
    {
        // Lần 3 phải chờ bấm xác nhận; lưu xong hoặc timer tick cũng không được bỏ qua màn này.
        if (_violationDialog is not null) return;
        if (_studentContent.Controls.OfType<StudentExamControl>().Any(exam => exam.IsSaving)) return;
        SubmitAttempt();
    }

    private bool TrySubmitExpiredAttempt(bool expiredDuringSave = false)
    {
        if (_violationSubmitPending)
        {
            SubmitForViolation();
            return true;
        }
        if (_handlingExpiredAttempt) return true;
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

        // MessageBox vẫn xử lý sự kiện UI; chặn timer/sự kiện lưu báo hết giờ lần nữa.
        _handlingExpiredAttempt = true;
        _deadlineTimer.Stop();
        try
        {
            MessageBox.Show(this, "Đã hết giờ. Hệ thống sẽ tự động nộp bài. Vui lòng chờ trạng thái xử lý.",
                "Hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            foreach (Form dialog in OwnedForms) dialog.Close();
            SubmitAttempt();
        }
        finally
        {
            _handlingExpiredAttempt = false;
        }
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
        if (_violationSubmitPending && _violationDialog is not null) return;
        if (_submitting || _attempt is not { } attempt) return;
        _submitting = true;
        StopViolationMonitoring();
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
                    : "Không nộp bài được. Hãy thử nộp bài lại hoặc liên hệ giảng viên.");
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

    private void ShowResult(ExamDesk.Models.StudentResult result)
    {
        var screen = new StudentResultControl(result);
        screen.EndSessionRequested += (_, _) =>
        {
            _deadlineTimer.Stop();
            _submitAfterSave = false;
            _attempt = null;
            _student = null;
            ShowIdentity();
        };
        screen.CloseRequested += (_, _) => Close();
        screen.BackRequested += (_, _) =>
        {
            _attempt = null;
            if (_student is { } student) ShowRealSessions(student);
        };
        ShowScreen(screen, "Kết quả bài thi");
        if (_attempt?.ViolationCount >= 3)
            Text = "UniExam — Bài đã tự động nộp do vi phạm lần thứ 3";
    }

    private (SubmissionStatusControl Screen, Button? Retry) ShowSubmissionStatus(bool busy = false)
    {
        var attempt = _attempt!;
        var screen = new SubmissionStatusControl();
        screen.SetAttempt(attempt, null);
        Button? retry = null;
        if (attempt.Draft is not null)
        {
            retry = StudentLayout.Button(attempt.Submitted ? "Tải lại kết quả" : "Thử nộp bài lại",
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
            screen.AddAction(StudentLayout.CreateButton("Quay lại chọn ca", (_, _) =>
            {
                _attempt = null;
                ShowRealSessions(student);
            }, StudentButtonKind.Primary, StudentArrow.Left));
        }
        screen.CloseRequested += (_, _) => Close();
        screen.SetBusy(busy);
        ShowScreen(screen, "Trạng thái nộp bài");
        return (screen, retry);
    }

    private void ConfirmClose(object? sender, FormClosingEventArgs e)
    {
        if (_violationMonitor is not null)
        {
            e.Cancel = true;
            ReportViolation("Yêu cầu thoát cửa sổ làm bài thi");
            return;
        }
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

    private void ShowScreen(UserControl screen, string title)
    {
        if (screen is not StudentExamControl) StopViolationMonitoring();
        var previous = _studentContent.Controls.OfType<UserControl>().SingleOrDefault();
        screen.Visible = false;
        screen.Size = _studentContent.ClientSize;
        screen.Dock = DockStyle.Fill;
        // Đổi nội dung và chiều cao vùng Dock.Fill trong cùng lượt layout.
        SuspendLayout();
        _studentContent.SuspendLayout();
        try
        {
            _examLabel.Text = "UniExam";
            if (_examLabel.Parent is Control header)
                header.Visible = true;
            _studentContent.Controls.Add(screen);
            screen.BringToFront();
        }
        finally
        {
            ResumeLayout(true);
            _studentContent.ResumeLayout(true);
        }
        // Load và bố cục ban đầu hoàn tất trước khi bỏ màn cũ phía sau.
        screen.Visible = true;
        screen.PerformLayout();
        previous?.Dispose();
        Text = "UniExam — " + title;
        if (screen is StudentExamControl exam) exam.FocusInitialControl();
        else screen.SelectNextControl(null, true, true, true, false);
    }

    private void StopViolationMonitoring()
    {
        _violationMonitor?.Dispose();
        _violationMonitor = null;
        _violationDialog?.Close();
        _violationDialog = null;
        _violationSubmitPending = false;
    }
}
