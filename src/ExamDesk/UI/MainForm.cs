namespace ExamDesk.UI;

// File chung: Bạn 2 điều phối. Chưa triển khai menu hoặc phân quyền.
public sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "ExamDesk — Khung sườn";
        ClientSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(new Label
        {
            Text = "Khung dự án ExamDesk\n\nChưa triển khai chức năng. Xem CONTEXT.md để nhận phần việc.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });
        // TODO (Bạn 2, phối hợp nhóm): nối đăng nhập và điều hướng các Form.
    }
}
