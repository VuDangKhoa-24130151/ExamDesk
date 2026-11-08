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
        if (busy) _status.Text = "Đang gửi bài…";
    }
    internal void AddAction(Control action) => _actions.Controls.Add(action);

    internal void SetAttempt(StudentAttemptState attempt, string? error)
    {
        if(attempt.Draft is { } draft)
        {
            _lockStatus.Text = draft.PendingSubmission ? "Bài đã khóa. Bạn không thể sửa đáp án." : "Chưa khóa được bài thi.";
            _status.Text = attempt.Submitted
                ? $"Bài đã gửi. Chưa hiển thị được kết quả.\n{error}"
                : $"Chưa gửi xong bài thi. Bạn có thể thử gửi lại.\n{error}";
            _summary.Text=$"Sinh viên: {draft.StudentName} · {draft.StudentCode}\nLớp: {draft.ClassName}\n\n{attempt.Title}\nĐã trả lời: {attempt.Answered}/{attempt.Answers.Length} câu";
            if (draft.PendingSubmission)
                _summary.Text += $"\nLý do: {(draft.AutoSubmitted ? "Hết giờ" : "Nộp chủ động")}";
            return;
        }
        string previewReason = attempt.IsExpired ? "Hết giờ" : "Nộp chủ động";
        _summary.Text = $"Sinh viên: Sinh viên mẫu · SV0001\n\nĐã chốt: {attempt.Answered}/{attempt.Answers.Length} câu có đáp án · Lý do: {previewReason}";
    }

    private void CenterCard()
    {
        int side = Math.Max(24, (ClientSize.Width - (int)(820 * DeviceDpi / 96f)) / 2);
        _cardHost.Padding = new Padding(side, 16, side, 24);
    }

}

