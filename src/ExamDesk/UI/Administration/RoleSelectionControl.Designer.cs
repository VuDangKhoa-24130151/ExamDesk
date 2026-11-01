#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class RoleSelectionControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private FlowLayoutPanel _headerPanel = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private TableLayoutPanel _cardsLayout = null!;
    private Panel _lecturerCardBorder = null!;
    private TableLayoutPanel _lecturerCardContent = null!;
    private Label _lblLecturerTag = null!;
    private Label _lblLecturerTitle = null!;
    private Label _lblLecturerDesc = null!;
    private Button _btnLecturerLogin = null!;
    private Panel _studentCardBorder = null!;
    private TableLayoutPanel _studentCardContent = null!;
    private Label _lblStudentTag = null!;
    private Label _lblStudentTitle = null!;
    private Label _lblStudentDesc = null!;
    private Button _btnStudentArea = null!;
    private FlowLayoutPanel _footerPanel = null!;
    private LinkLabel _lnkHelp = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerPanel = new FlowLayoutPanel();
        _lblTitle = new Label();
        _lblSubtitle = new Label();
        _cardsLayout = new TableLayoutPanel();
        _lecturerCardBorder = new Panel();
        _lecturerCardContent = new TableLayoutPanel();
        _lblLecturerTag = new Label();
        _lblLecturerTitle = new Label();
        _lblLecturerDesc = new Label();
        _btnLecturerLogin = new Button();
        _studentCardBorder = new Panel();
        _studentCardContent = new TableLayoutPanel();
        _lblStudentTag = new Label();
        _lblStudentTitle = new Label();
        _lblStudentDesc = new Label();
        _btnStudentArea = new Button();
        _footerPanel = new FlowLayoutPanel();
        _lnkHelp = new LinkLabel();

        _rootLayout.SuspendLayout();
        _headerPanel.SuspendLayout();
        _cardsLayout.SuspendLayout();
        _lecturerCardBorder.SuspendLayout();
        _lecturerCardContent.SuspendLayout();
        _studentCardBorder.SuspendLayout();
        _studentCardContent.SuspendLayout();
        _footerPanel.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 3;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
        _rootLayout.RowCount = 5;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // _headerPanel
        _headerPanel.Dock = DockStyle.Fill;
        _headerPanel.AutoSize = true;
        _headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _headerPanel.FlowDirection = FlowDirection.TopDown;
        _headerPanel.WrapContents = false;
        _headerPanel.Margin = new Padding(8, 0, 8, 24);

        // _lblTitle
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 8);
        _lblTitle.Text = "Chào mừng đến với UniExam";

        // _lblSubtitle
        _lblSubtitle.AutoSize = true;
        _lblSubtitle.Font = UiStyle.BodyFont;
        _lblSubtitle.ForeColor = UiStyle.TextMuted;
        _lblSubtitle.Margin = new Padding(0);
        _lblSubtitle.Text = "Chọn vai trò để tiếp tục";

        _headerPanel.Controls.Add(_lblTitle);
        _headerPanel.Controls.Add(_lblSubtitle);

        // _cardsLayout
        _cardsLayout.Dock = DockStyle.Fill;
        _cardsLayout.AutoSize = true;
        _cardsLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _cardsLayout.ColumnCount = 2;
        _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _cardsLayout.RowCount = 1;
        _cardsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _cardsLayout.Margin = new Padding(0);

        // _lecturerCardBorder
        _lecturerCardBorder.Dock = DockStyle.Fill;
        _lecturerCardBorder.AutoSize = true;
        _lecturerCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _lecturerCardBorder.BackColor = UiStyle.Border;
        _lecturerCardBorder.Padding = new Padding(1);
        _lecturerCardBorder.Margin = new Padding(8, 0, 12, 0);

        // _lecturerCardContent
        _lecturerCardContent.Dock = DockStyle.Fill;
        _lecturerCardContent.AutoSize = true;
        _lecturerCardContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _lecturerCardContent.BackColor = UiStyle.Surface;
        _lecturerCardContent.Padding = new Padding(24);
        _lecturerCardContent.ColumnCount = 1;
        _lecturerCardContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _lecturerCardContent.RowCount = 4;
        _lecturerCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _lecturerCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _lecturerCardContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _lecturerCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // _lblLecturerTag
        _lblLecturerTag.AutoSize = true;
        _lblLecturerTag.Font = UiStyle.SmallFont;
        _lblLecturerTag.ForeColor = UiStyle.Primary;
        _lblLecturerTag.Margin = new Padding(0, 0, 0, 4);
        _lblLecturerTag.Text = "PHÂN HỆ QUẢN TRỊ & TỔ CHỨC THI";

        // _lblLecturerTitle
        _lblLecturerTitle.AutoSize = true;
        _lblLecturerTitle.Font = UiStyle.TitleFont;
        _lblLecturerTitle.ForeColor = UiStyle.Text;
        _lblLecturerTitle.Margin = new Padding(0, 0, 0, 12);
        _lblLecturerTitle.Text = "Giảng viên";

        // _lblLecturerDesc
        _lblLecturerDesc.AutoSize = true;
        _lblLecturerDesc.Dock = DockStyle.Fill;
        _lblLecturerDesc.Font = UiStyle.BodyFont;
        _lblLecturerDesc.ForeColor = UiStyle.TextMuted;
        _lblLecturerDesc.Margin = new Padding(0, 0, 0, 24);
        _lblLecturerDesc.Text = "Quản lý câu hỏi, đề thi, ca thi, kết quả và thống kê học phần.";

        // _btnLecturerLogin
        _btnLecturerLogin.Dock = DockStyle.Fill;
        _btnLecturerLogin.Height = 40;
        _btnLecturerLogin.Margin = new Padding(0);
        _btnLecturerLogin.TabIndex = 0;
        _btnLecturerLogin.Text = "Đăng nhập giảng viên";
        UiStyle.StylePrimaryButton(_btnLecturerLogin);
        _btnLecturerLogin.Click += OnLecturerLoginClick;

        _lecturerCardContent.Controls.Add(_lblLecturerTag, 0, 0);
        _lecturerCardContent.Controls.Add(_lblLecturerTitle, 0, 1);
        _lecturerCardContent.Controls.Add(_lblLecturerDesc, 0, 2);
        _lecturerCardContent.Controls.Add(_btnLecturerLogin, 0, 3);
        _lecturerCardBorder.Controls.Add(_lecturerCardContent);

        // _studentCardBorder
        _studentCardBorder.Dock = DockStyle.Fill;
        _studentCardBorder.AutoSize = true;
        _studentCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentCardBorder.BackColor = UiStyle.Border;
        _studentCardBorder.Padding = new Padding(1);
        _studentCardBorder.Margin = new Padding(12, 0, 8, 0);

        // _studentCardContent
        _studentCardContent.Dock = DockStyle.Fill;
        _studentCardContent.AutoSize = true;
        _studentCardContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentCardContent.BackColor = UiStyle.Surface;
        _studentCardContent.Padding = new Padding(24);
        _studentCardContent.ColumnCount = 1;
        _studentCardContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _studentCardContent.RowCount = 4;
        _studentCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _studentCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _studentCardContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _studentCardContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // _lblStudentTag
        _lblStudentTag.AutoSize = true;
        _lblStudentTag.Font = UiStyle.SmallFont;
        _lblStudentTag.ForeColor = UiStyle.TextMuted;
        _lblStudentTag.Margin = new Padding(0, 0, 0, 4);
        _lblStudentTag.Text = "PHÂN HỆ LÀM BÀI THI";

        // _lblStudentTitle
        _lblStudentTitle.AutoSize = true;
        _lblStudentTitle.Font = UiStyle.TitleFont;
        _lblStudentTitle.ForeColor = UiStyle.Text;
        _lblStudentTitle.Margin = new Padding(0, 0, 0, 12);
        _lblStudentTitle.Text = "Sinh viên";

        // _lblStudentDesc
        _lblStudentDesc.AutoSize = true;
        _lblStudentDesc.Dock = DockStyle.Fill;
        _lblStudentDesc.Font = UiStyle.BodyFont;
        _lblStudentDesc.ForeColor = UiStyle.TextMuted;
        _lblStudentDesc.Margin = new Padding(0, 0, 0, 24);
        _lblStudentDesc.Text = "Nhận diện thí sinh, chọn ca thi và thực hiện bài thi trắc nghiệm.";

        // _btnStudentArea
        _btnStudentArea.Dock = DockStyle.Fill;
        _btnStudentArea.Height = 40;
        _btnStudentArea.Margin = new Padding(0);
        _btnStudentArea.TabIndex = 1;
        _btnStudentArea.Text = "Vào khu sinh viên";
        UiStyle.StyleSecondaryButton(_btnStudentArea);
        _btnStudentArea.Click += OnStudentAreaClick;

        _studentCardContent.Controls.Add(_lblStudentTag, 0, 0);
        _studentCardContent.Controls.Add(_lblStudentTitle, 0, 1);
        _studentCardContent.Controls.Add(_lblStudentDesc, 0, 2);
        _studentCardContent.Controls.Add(_btnStudentArea, 0, 3);
        _studentCardBorder.Controls.Add(_studentCardContent);

        _cardsLayout.Controls.Add(_lecturerCardBorder, 0, 0);
        _cardsLayout.Controls.Add(_studentCardBorder, 1, 0);

        // _footerPanel
        _footerPanel.Dock = DockStyle.Fill;
        _footerPanel.AutoSize = true;
        _footerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _footerPanel.FlowDirection = FlowDirection.LeftToRight;
        _footerPanel.WrapContents = false;
        _footerPanel.Margin = new Padding(8, 12, 8, 0);

        // _lnkHelp
        _lnkHelp.AutoSize = true;
        _lnkHelp.Font = UiStyle.BodyFont;
        _lnkHelp.LinkColor = UiStyle.Primary;
        _lnkHelp.ActiveLinkColor = UiStyle.Primary;
        _lnkHelp.VisitedLinkColor = UiStyle.Primary;
        _lnkHelp.TabStop = true;
        _lnkHelp.TabIndex = 2;
        _lnkHelp.Text = "Trợ giúp";
        _lnkHelp.LinkClicked += OnHelpLinkClicked;

        _footerPanel.Controls.Add(_lnkHelp);

        _rootLayout.Controls.Add(_headerPanel, 1, 1);
        _rootLayout.Controls.Add(_cardsLayout, 1, 2);
        _rootLayout.Controls.Add(_footerPanel, 1, 4);

        // RoleSelectionControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "RoleSelectionControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerPanel.ResumeLayout(false);
        _headerPanel.PerformLayout();
        _cardsLayout.ResumeLayout(false);
        _cardsLayout.PerformLayout();
        _lecturerCardBorder.ResumeLayout(false);
        _lecturerCardBorder.PerformLayout();
        _lecturerCardContent.ResumeLayout(false);
        _lecturerCardContent.PerformLayout();
        _studentCardBorder.ResumeLayout(false);
        _studentCardBorder.PerformLayout();
        _studentCardContent.ResumeLayout(false);
        _studentCardContent.PerformLayout();
        _footerPanel.ResumeLayout(false);
        _footerPanel.PerformLayout();
        ResumeLayout(false);
    }
}
