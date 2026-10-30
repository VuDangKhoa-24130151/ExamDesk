using ExamDesk.UI.Administration;
using ExamDesk.UI.ExamManagement;
using ExamDesk.UI.Reports;
using ExamDesk.UI.DataManagement;
using ExamDesk.UI.Student;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI;

// File chung: Bạn 1 điều phối. Đây là cửa sổ tích hợp UI, chưa phải đăng nhập/phân quyền.
public sealed class MainForm : Form
{
    private readonly Panel _content = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        Text = "UniExam — Khung tích hợp UI";
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;

        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Left, Width = 224, Padding = new Padding(12),
            FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true
        };
        var note = new Label
        {
            Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(12),
            Text = "CHẾ ĐỘ DỰNG UI • Các màn hình là khung trống, chưa đăng nhập hoặc kết nối SQLite."
        };
        AddMenu(menu, "Tổng quan", () => ShowScreen(new DashboardControl()));
        AddMenu(menu, "Ngân hàng câu hỏi", () => ShowScreen(new QuestionBankControl()));
        AddMenu(menu, "Đề thi", () => ShowScreen(new ExamPaperControl()));
        AddMenu(menu, "Ca thi", () => ShowScreen(new SessionManagementControl()));
        AddMenu(menu, "Kết quả và thống kê", () => ShowScreen(new GradingReviewControl()));
        AddMenu(menu, "Dữ liệu và sao lưu", () => ShowScreen(new DataManagementControl()));
        AddMenu(menu, "Mở UI đăng nhập GV", () => { using var form = new LoginForm(); form.ShowDialog(this); });
        AddMenu(menu, "Mở UI sinh viên", () => { using var form = new StudentExamForm(); form.ShowDialog(this); });
        Controls.Add(_content);
        Controls.Add(menu);
        Controls.Add(note);
        ShowScreen(new DashboardControl());
    }

    private static void AddMenu(FlowLayoutPanel menu, string text, Action open)
    {
        var button = new Button { Text = text, Width = 188, Height = 40, Margin = new Padding(0, 0, 0, 8) };
        button.Click += (_, _) => open();
        menu.Controls.Add(button);
    }

    private void ShowScreen(UserControl screen)
    {
        // Khung xem UI: chuyển màn hình hủy control cũ; form thật cần hỏi khi có sửa chưa lưu.
        while (_content.Controls.Count > 0)
        {
            var previous = _content.Controls[0];
            _content.Controls.Remove(previous);
            previous.Dispose();
        }
        screen.Dock = DockStyle.Fill;
        _content.Controls.Add(screen);
    }
}
