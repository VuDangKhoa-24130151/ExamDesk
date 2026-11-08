namespace ExamDesk.UI.Student;
partial class ResumeAttemptControl
{
    private Label _summary = null!;
    private void InitializeComponent()
    {
        var page = StudentLayout.Page(this, "Khôi phục lượt thi");
        page.Controls.Add(StudentLayout.Label("Bạn có một lượt thi chưa hoàn tất", 12, FontStyle.Bold));
        _summary = StudentLayout.Label(""); page.Controls.Add(_summary);
        page.Controls.Add(StudentLayout.Button("&Tiếp tục làm bài →", (_, _) => ResumeRequested?.Invoke(this, EventArgs.Empty), true));
    }
}
