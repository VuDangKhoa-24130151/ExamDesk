using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SubmissionStatusControl : UserControl
{
    public event EventHandler? CloseRequested;

    public SubmissionStatusControl()
    {
        InitializeComponent();
        Resize += (_, _) => CenterCard();
        CenterCard();
    }

    internal void SetBusy(bool busy)
    {
        _actions.Enabled = !busy;
        UseWaitCursor = busy;
        _busyProgress.MarqueeAnimationSpeed = busy ? 30 : 0;
        _busyProgress.Visible = busy;
        if (busy) _status.Text = "Đang nộp bài…";
    }
    internal void AddAction(Control action) => _actions.Controls.Add(action);

    internal void SetAttempt(StudentAttemptState attempt, string? error)
    {
        if (attempt.Draft is not { } draft) return;
        bool hasError = !string.IsNullOrWhiteSpace(error);
        // Bài thật chưa hiển thị điểm ở màn này; kết quả có màn riêng sau khi tải xong.
        _status.BackColor = hasError ? UiStyle.ErrorBack : UiStyle.WarningBack;
        _status.ForeColor = hasError ? UiStyle.ErrorText : UiStyle.WarningText;
        _lockStatus.Text = draft.PendingSubmission ? "Bài đã khóa. Bạn không thể sửa đáp án." : "Chưa khóa được bài thi.";
        _status.Text = attempt.Submitted
            ? "Đã nộp bài. Chưa hiển thị được kết quả."
            : "Chưa nộp bài xong. Bạn có thể thử nộp bài lại.";
        if (hasError) _status.Text += $"\n{error}";
        _summary.Text=$"Sinh viên: {draft.StudentName} · {draft.StudentCode}\nLớp: {draft.ClassName}\n\n{attempt.Title}\nĐã trả lời: {attempt.Answered}/{attempt.Answers.Length} câu";
        if (draft.PendingSubmission)
            _summary.Text += $"\nLý do: {(draft.AutoSubmitted ? "Hết giờ" : "Nộp bài chủ động")}";
    }

    private void CenterCard()
    {
        int side = Math.Max(StudentLayout.Space24,
            (ClientSize.Width - StudentLayout.Scale(this, StudentLayout.StatusCardMaxWidth)) / 2);
        _cardHost.Padding = new Padding(side, StudentLayout.Space16, side, StudentLayout.Space24);
    }

}

