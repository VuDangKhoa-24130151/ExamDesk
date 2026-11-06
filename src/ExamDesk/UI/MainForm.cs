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
    private static readonly Font ActiveMenuFont = new("Segoe UI", 10F, FontStyle.Bold);
    private static readonly Color MenuHoverColor = ColorTranslator.FromHtml("#1E4276");

    private readonly Panel _content = new() { Dock = DockStyle.Fill, Padding = new Padding(0) };
    private readonly List<Button> _menuButtons = [];
    private Button _dashboardButton = null!;
    private int _nextTabIndex;

    public MainForm()
    {
        AutoScaleMode = AutoScaleMode.Font;
        Text = "UniExam — Khung tích hợp UI";
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;

        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 224,
            Padding = new Padding(12),
            BackColor = UiStyle.Sidebar,
            ForeColor = Color.White,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        var brandLabel = new Label
        {
            Text = "UniExam",
            Font = UiStyle.TitleFont,
            ForeColor = Color.White,
            BackColor = UiStyle.Sidebar,
            AutoSize = false,
            Width = 200,
            Height = 48,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0),
            Margin = new Padding(0, 4, 0, 12)
        };
        menu.Controls.Add(brandLabel);

        var note = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(12),
            Font = UiStyle.SmallFont,
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Background,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "CHẾ ĐỘ DỰNG UI • SQLite đã khởi tạo; giao diện đang dùng dữ liệu demo, chưa lưu nghiệp vụ thật."
        };

        var roleSelectionButton = AddMenu(menu, "Chọn vai trò", OpenRoleSelection);
        _dashboardButton = AddMenu(menu, "Tổng quan", () => ShowScreen(new DashboardControl()));
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

        SetActiveMenuButton(roleSelectionButton);
        OpenRoleSelection();
    }

    private void OpenRoleSelection()
    {
        var roleControl = new RoleSelectionControl();
        roleControl.LecturerLoggedIn += (_, _) =>
        {
            SetActiveMenuButton(_dashboardButton);
            ShowScreen(new DashboardControl());
        };
        ShowScreen(roleControl);
    }

    private Button AddMenu(FlowLayoutPanel menu, string text, Action open)
    {
        var button = new Button
        {
            Tag = text,
            Text = "   " + text,
            Width = 200,
            Height = 40,
            Margin = new Padding(0, 0, 0, 8),
            FlatStyle = FlatStyle.Flat,
            BackColor = UiStyle.Sidebar,
            ForeColor = Color.White,
            Font = UiStyle.BodyFont,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 8, 0),
            Cursor = Cursors.Hand,
            TabStop = true,
            TabIndex = _nextTabIndex++
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = MenuHoverColor;
        button.FlatAppearance.MouseDownBackColor = UiStyle.Primary;
        button.Click += (_, _) =>
        {
            SetActiveMenuButton(button);
            open();
        };

        _menuButtons.Add(button);
        menu.Controls.Add(button);
        return button;
    }

    private void SetActiveMenuButton(Button activeButton)
    {
        foreach (var btn in _menuButtons)
        {
            var baseText = btn.Tag as string ?? string.Empty;
            var isSelected = ReferenceEquals(btn, activeButton);
            btn.BackColor = isSelected ? UiStyle.Primary : UiStyle.Sidebar;
            btn.Font = isSelected ? ActiveMenuFont : UiStyle.BodyFont;
            btn.Text = isSelected ? $"▶ {baseText}" : $"   {baseText}";
        }
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
