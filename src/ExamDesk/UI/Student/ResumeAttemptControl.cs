namespace ExamDesk.UI.Student;
public partial class ResumeAttemptControl : UserControl
{
    public event EventHandler? ResumeRequested;
    public ResumeAttemptControl() => InitializeComponent();
    internal void SetAttempt(StudentPreviewAttempt attempt)
    {
        _summary.Text = $"Cuối kỳ .NET – Ca 01 · Vũ Đăng Khoa · 24130151\n\nĐã trả lời: {attempt.Answered}/30 · Đánh dấu: {attempt.Flags.Count(flag => flag)}\nHạn nộp: {attempt.Deadline:HH:mm:ss dd/MM/yyyy}\n\nDữ liệu chỉ được giữ trong cửa sổ hiện tại. Chưa hỗ trợ khôi phục sau khi đóng ứng dụng.\nĐồng hồ tiếp tục tính theo hạn cũ, không cấp lại 45 phút.";
    }
}
