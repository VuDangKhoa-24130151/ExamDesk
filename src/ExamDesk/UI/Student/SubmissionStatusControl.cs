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

    internal void SetAttempt(StudentPreviewAttempt attempt)
    {
        string reason = DateTime.Now >= attempt.Deadline ? "Hết giờ" : "Nộp chủ động";
        _summary.Text = $"Sinh viên: Vũ Đăng Khoa · 24130151\n\nĐã chốt: {attempt.Answered}/{attempt.Answers.Length} câu có đáp án · Lý do: {reason}";
    }

    private void CenterCard()
    {
        int side = Math.Max(24, (ClientSize.Width - (int)(820 * DeviceDpi / 96f)) / 2);
        _cardHost.Padding = new Padding(side, 16, side, 24);
    }

}

