using ExamDesk.Repositories;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public sealed class StudentResultControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? CloseRequested;
    internal event EventHandler? EndSessionRequested;

    internal StudentResultControl(StudentResult result)
    {
        var page = StudentLayout.Page(this, "Kết quả bài thi");
        page.Controls.Add(StudentLayout.Label($"Sinh viên: {result.StudentName} · MSSV: {result.StudentCode}"));
        page.Controls.Add(StudentLayout.Label($"Ca thi: {result.SessionName}"));
        page.Controls.Add(StudentLayout.Label($"Đề thi: {result.PaperTitle}"));

        var score = StudentLayout.Label($"Điểm: {result.Score:F2}/10", 18, FontStyle.Bold);
        score.Padding = new Padding(16, 8, 16, 8);
        score.BackColor = UiStyle.Background;
        score.ForeColor = UiStyle.Primary;
        page.Controls.Add(score);
        page.Controls.Add(StudentLayout.Label($"Số câu đúng: {result.Correct}/{result.Total}"));
        page.Controls.Add(StudentLayout.Label($"Số câu sai: {result.Total - result.Correct - result.Blank}"));
        page.Controls.Add(StudentLayout.Label($"Số câu bỏ trống: {result.Blank}"));
        page.Controls.Add(StudentLayout.Label($"Thời điểm nộp: {result.SubmittedAt.ToLocalTime():dd/MM/yyyy HH:mm:ss}"));
        page.Controls.Add(StudentLayout.Label($"Lý do: {(result.IsAutoSubmitted ? "Hết giờ" : "Nộp tay")}"));
        var actions = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.Button("&Quay lại chọn ca", (_, _) => BackRequested?.Invoke(this, EventArgs.Empty)));
        actions.Controls.Add(StudentLayout.Button("Kết thúc phiên sinh viên", (_, _) => EndSessionRequested?.Invoke(this, EventArgs.Empty)));
        actions.Controls.Add(StudentLayout.Button("&Đóng", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty), true));
        page.Controls.Add(actions);
    }
}
