using ExamDesk.Repositories;

namespace ExamDesk.UI.Student;

public sealed class StudentResultControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? CloseRequested;

    internal StudentResultControl(StudentResult result)
    {
        var page = StudentLayout.Page(this, "Kết quả bài thi");
        page.Controls.Add(StudentLayout.Label($"Điểm: {result.Score:F2}/10", 18, FontStyle.Bold));
        page.Controls.Add(StudentLayout.Label($"Số câu đúng: {result.Correct}/{result.Total}"));
        page.Controls.Add(StudentLayout.Label($"Số câu sai: {result.Total - result.Correct - result.Blank}"));
        page.Controls.Add(StudentLayout.Label($"Số câu bỏ trống: {result.Blank}"));
        page.Controls.Add(StudentLayout.Label($"Thời điểm nộp: {result.SubmittedAt.ToLocalTime():dd/MM/yyyy HH:mm:ss}"));
        page.Controls.Add(StudentLayout.Label($"Lý do: {(result.IsAutoSubmitted ? "Hết giờ" : "Nộp tay")}"));
        var actions = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.Button("&Quay lại chọn ca", (_, _) => BackRequested?.Invoke(this, EventArgs.Empty)));
        actions.Controls.Add(StudentLayout.Button("&Đóng", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty), true));
        page.Controls.Add(actions);
    }
}
