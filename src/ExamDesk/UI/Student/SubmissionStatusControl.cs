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

    internal Label StatusLabel => _status;
    internal void AddAction(Control action) => _actions.Controls.Add(action);

    internal void SetAttempt(StudentPreviewAttempt attempt)
    {
        if(attempt.Draft is { } draft)
        {
            string reason = draft.AutoSubmitted ? "Lượt thi đã hết giờ." : "Bạn đã nộp bài cho ca này.";
            _status.Text = $"{reason} Bài đã khóa và đang chờ chấm điểm. Bạn có thể quay lại để chọn ca thi khác.";
            _summary.Text=$"Sinh viên: {draft.StudentName} · {draft.StudentCode}\nLớp: {draft.ClassName}\n\n{attempt.Title}\nĐã trả lời: {attempt.Answered}/{attempt.Answers.Length} câu\nLý do: {(draft.AutoSubmitted ? "Hết giờ" : "Nộp chủ động")}";
            return;
        }
        string previewReason = DateTime.Now >= attempt.Deadline ? "Hết giờ" : "Nộp chủ động";
        _summary.Text = $"Sinh viên: Vũ Đăng Khoa · 24130151\n\nĐã chốt: {attempt.Answered}/{attempt.Answers.Length} câu có đáp án · Lý do: {previewReason}";
    }

    private void CenterCard()
    {
        int side = Math.Max(24, (ClientSize.Width - (int)(820 * DeviceDpi / 96f)) / 2);
        _cardHost.Padding = new Padding(side, 16, side, 24);
    }

}

