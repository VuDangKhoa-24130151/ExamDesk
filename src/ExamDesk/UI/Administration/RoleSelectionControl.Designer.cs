#nullable enable
using ExamDesk.UI.Shared;
using ExamDesk.UI.Student;

namespace ExamDesk.UI.Administration;

partial class RoleSelectionControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _topBar = null!;
    private Label _logoBadge = null!;
    private Panel _viewport = null!;
    private TableLayoutPanel _contentLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private TableLayoutPanel _cardsLayout = null!;
    private CardPanel _lecturerCard = null!;
    private CardPanel _studentCard = null!;
    private RoundedButton _btnLecturerLogin = null!;
    private RoundedButton _btnStudentArea = null!;
    private TableLayoutPanel _footer = null!;
    private Label _loginStatus = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        Font = UiStyle.BodyFont;
        AutoScaleDimensions = new SizeF(7F, 19F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        DoubleBuffered = true;
        Name = "RoleSelectionControl";
        Size = new Size(1100, 620);

        _topBar = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 48, ColumnCount = 2, RowCount = 1,
            Padding = new Padding(24, 8, 24, 8), BackColor = UiStyle.Surface,
            TabStop = false, TabIndex = 1
        };
        _topBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _topBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _logoBadge = new Label { Size = new Size(28, 28), Anchor = AnchorStyles.Left, Margin = Padding.Empty };
        _logoBadge.Paint += OnLogoBadgePaint;

        var brand = CreateLabel("UniExam", UiStyle.SectionTitleFont, UiStyle.Text);
        brand.Anchor = AnchorStyles.Left;
        brand.Margin = new Padding(10, 0, 0, 0);

        _topBar.Controls.Add(_logoBadge, 0, 0);
        _topBar.Controls.Add(brand, 1, 0);

        _viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = UiStyle.Background, TabStop = false, TabIndex = 0 };
        _contentLayout = new TableLayoutPanel
        {
            ColumnCount = 1, RowCount = 3, BackColor = UiStyle.Background,
            Margin = Padding.Empty, TabStop = false
        };
        _contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) _contentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblTitle = CreateLabel("Chào mừng đến với UniExam", UiStyle.TitleFont, UiStyle.Text);
        _lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        _lblTitle.Dock = DockStyle.Fill;
        _lblTitle.Margin = new Padding(0, 0, 0, 10);

        _lblSubtitle = CreateLabel("Chọn vai trò của bạn để bắt đầu", UiStyle.BodyFont, UiStyle.TextMuted);
        _lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        _lblSubtitle.Dock = DockStyle.Fill;
        _lblSubtitle.Margin = new Padding(0, 0, 0, 32);

        _cardsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            BackColor = UiStyle.Background, Margin = Padding.Empty, TabStop = false
        };
        _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _cardsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _btnLecturerLogin = CreateRoleButton("Đăng nhập giảng viên", false);
        _btnLecturerLogin.Click += OnLecturerLoginClick;
        _btnStudentArea = CreateRoleButton("Đăng nhập sinh viên", true);
        _btnStudentArea.Click += OnStudentAreaClick;

        _lecturerCard = CreateRoleCard("QUẢN TRỊ & TỔ CHỨC THI", "Giảng viên",
            "Quản lý câu hỏi, đề thi, ca thi và theo dõi kết quả sinh viên.", _btnLecturerLogin);
        _lecturerCard.TabIndex = 0;
        _studentCard = CreateRoleCard("HỌC TẬP & KIỂM TRA", "Sinh viên",
            "Xác nhận thông tin, chọn ca thi và bắt đầu làm bài trắc nghiệm.", _btnStudentArea);
        _studentCard.TabIndex = 1;

        _cardsLayout.Controls.Add(_lecturerCard, 0, 0);
        _cardsLayout.Controls.Add(_studentCard, 1, 0);
        _contentLayout.Controls.Add(_lblTitle, 0, 0);
        _contentLayout.Controls.Add(_lblSubtitle, 0, 1);
        _contentLayout.Controls.Add(_cardsLayout, 0, 2);
        _viewport.Controls.Add(_contentLayout);

        _footer = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 40, ColumnCount = 2, RowCount = 1,
            BackColor = UiStyle.Surface, Padding = new Padding(24, 0, 24, 0), TabStop = false, TabIndex = 2
        };
        _footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var footerText = CreateLabel("UniExam • Hệ thống thi trắc nghiệm", UiStyle.SmallFont, UiStyle.TextMuted);
        footerText.Anchor = AnchorStyles.Left;
        _loginStatus = CreateLabel("Chưa đăng nhập", UiStyle.SmallFont, UiStyle.TextMuted);
        _loginStatus.Anchor = AnchorStyles.Right;
        _footer.Controls.Add(footerText, 0, 0);
        _footer.Controls.Add(_loginStatus, 1, 0);

        Controls.Add(_viewport);
        Controls.Add(_topBar);
        Controls.Add(_footer);
        ResumeLayout(false);
    }

    private static Label CreateLabel(string text, Font font, Color color) => new()
    {
        Text = text, Font = font, ForeColor = color, AutoSize = true,
        UseMnemonic = false, Margin = Padding.Empty
    };

    private static RoundedButton CreateRoleButton(string text, bool primary) => new(text, primary)
    {
        Dock = DockStyle.Fill, Height = 40, MinimumSize = new Size(0, 40),
        Margin = Padding.Empty, Font = UiStyle.BodyBoldFont, TabIndex = 0
    };

    private static CardPanel CreateRoleCard(string category, string title, string description, RoundedButton button)
    {
        // Dùng CardPanel có sẵn và để bảng con nền trong suốt khỏi đè lên viền bo góc.
        var card = new CardPanel { Dock = DockStyle.Fill, Padding = new Padding(24), TabStop = false };
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = Padding.Empty,
            Margin = Padding.Empty, ColumnCount = 1, RowCount = 4, TabStop = false
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var tag = CreateLabel(category, UiStyle.SmallBoldFont, UiStyle.TextMuted);
        tag.Margin = new Padding(0, 0, 0, 10);
        var heading = CreateLabel(title, UiStyle.TitleFont, UiStyle.Text);
        heading.Margin = new Padding(0, 0, 0, 10);
        var detail = CreateLabel(description, UiStyle.BodyFont, UiStyle.TextMuted);
        detail.Margin = new Padding(0, 0, 0, 24);

        content.Controls.Add(tag, 0, 0);
        content.Controls.Add(heading, 0, 1);
        content.Controls.Add(detail, 0, 2);
        content.Controls.Add(button, 0, 3);
        card.Controls.Add(content);
        return card;
    }
}
