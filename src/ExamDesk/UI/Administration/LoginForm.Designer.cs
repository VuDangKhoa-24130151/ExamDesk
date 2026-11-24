#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class LoginForm
{
    private static readonly Font BrandFont = new("Segoe UI", 8.5F, FontStyle.Bold);

    private System.ComponentModel.IContainer? components = null;
    private Panel _pnlAccentBar = null!;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblBrand = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private Panel _pnlErrorBanner = null!;
    private Label _lblGeneralError = null!;
    private Label _lblUsername = null!;
    private Panel _pnlUsernameBox = null!;
    private TextBox _txtUsername = null!;
    private Label _lblUsernameError = null!;
    private Label _lblPassword = null!;
    private Panel _pnlPasswordBox = null!;
    private TextBox _txtPassword = null!;
    private Label _lblPasswordError = null!;
    private CheckBox _chkShowPassword = null!;
    private Panel _pnlDivider = null!;
    private TableLayoutPanel _buttonsLayout = null!;
    private Button _btnCancel = null!;
    private Button _btnLogin = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _pnlAccentBar = new Panel();
        _rootLayout = new TableLayoutPanel();
        _lblBrand = new Label();
        _lblTitle = new Label();
        _lblSubtitle = new Label();
        _pnlErrorBanner = new Panel();
        _lblGeneralError = new Label();
        _lblUsername = new Label();
        _pnlUsernameBox = new Panel();
        _txtUsername = new TextBox();
        _lblUsernameError = new Label();
        _lblPassword = new Label();
        _pnlPasswordBox = new Panel();
        _txtPassword = new TextBox();
        _lblPasswordError = new Label();
        _chkShowPassword = new CheckBox();
        _pnlDivider = new Panel();
        _buttonsLayout = new TableLayoutPanel();
        _btnCancel = new Button();
        _btnLogin = new Button();

        _rootLayout.SuspendLayout();
        _pnlErrorBanner.SuspendLayout();
        _pnlUsernameBox.SuspendLayout();
        _pnlPasswordBox.SuspendLayout();
        _buttonsLayout.SuspendLayout();
        SuspendLayout();

        // _pnlAccentBar
        _pnlAccentBar.Dock = DockStyle.Top;
        _pnlAccentBar.Height = 4;
        _pnlAccentBar.BackColor = UiStyle.Primary;
        _pnlAccentBar.Margin = Padding.Empty;
        _pnlAccentBar.TabStop = false;

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.AutoScroll = true;
        _rootLayout.BackColor = UiStyle.Surface;
        _rootLayout.Padding = new Padding(30, 22, 30, 24);
        _rootLayout.Margin = Padding.Empty;
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 13;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Brand
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Title
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: Subtitle
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3: General error banner
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 4: Username label
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 5: Username box
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 6: Username error
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 7: Password label
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 8: Password box
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 9: Password error
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 10: Show password checkbox
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 11: Divider
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 12: Buttons

        // _lblBrand
        _lblBrand.AutoSize = true;
        _lblBrand.Font = BrandFont;
        _lblBrand.ForeColor = UiStyle.Primary;
        _lblBrand.Margin = new Padding(0, 0, 0, 2);
        _lblBrand.Text = "UNIEXAM";

        // _lblTitle
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 4);
        _lblTitle.Text = "Đăng nhập giảng viên";

        // _lblSubtitle
        _lblSubtitle.AutoSize = true;
        _lblSubtitle.Font = UiStyle.SmallFont;
        _lblSubtitle.ForeColor = UiStyle.TextMuted;
        _lblSubtitle.Margin = new Padding(0, 0, 0, 16);
        _lblSubtitle.Text = "Nhập thông tin tài khoản để truy cập phân hệ quản trị và thi.";

        // _pnlErrorBanner
        _pnlErrorBanner.Dock = DockStyle.Fill;
        _pnlErrorBanner.AutoSize = true;
        _pnlErrorBanner.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _pnlErrorBanner.BackColor = UiStyle.ErrorBack;
        _pnlErrorBanner.Padding = new Padding(12, 9, 12, 9);
        _pnlErrorBanner.Margin = new Padding(0, 0, 0, 14);
        _pnlErrorBanner.Visible = false;

        // _lblGeneralError
        _lblGeneralError.AutoSize = true;
        _lblGeneralError.Dock = DockStyle.Fill;
        _lblGeneralError.Font = UiStyle.SmallFont;
        _lblGeneralError.ForeColor = UiStyle.ErrorText;
        _lblGeneralError.Margin = Padding.Empty;
        _lblGeneralError.Text = string.Empty;

        _pnlErrorBanner.Controls.Add(_lblGeneralError);

        // _lblUsername
        _lblUsername.AutoSize = true;
        _lblUsername.Font = UiStyle.SmallBoldFont;
        _lblUsername.ForeColor = UiStyle.Text;
        _lblUsername.Margin = new Padding(0, 0, 0, 6);
        _lblUsername.Text = "Tên đăng nhập *";

        // _pnlUsernameBox
        _pnlUsernameBox.Dock = DockStyle.Fill;
        _pnlUsernameBox.AutoSize = true;
        _pnlUsernameBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _pnlUsernameBox.MinimumSize = new Size(0, 40);
        _pnlUsernameBox.BackColor = UiStyle.Surface;
        _pnlUsernameBox.Padding = new Padding(12, 10, 12, 10);
        _pnlUsernameBox.Margin = new Padding(0, 0, 0, 4);
        _pnlUsernameBox.Cursor = Cursors.IBeam;
        _pnlUsernameBox.TabIndex = 0;
        _pnlUsernameBox.TabStop = false;

        // _txtUsername
        _txtUsername.Dock = DockStyle.Top;
        _txtUsername.Font = UiStyle.InputLargeFont;
        _txtUsername.ForeColor = UiStyle.Text;
        _txtUsername.BackColor = UiStyle.Surface;
        _txtUsername.BorderStyle = BorderStyle.None;
        _txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        _txtUsername.Margin = Padding.Empty;
        _txtUsername.TabIndex = 0;
        _txtUsername.TextChanged += OnUsernameTextChanged;

        _pnlUsernameBox.Controls.Add(_txtUsername);

        // _lblUsernameError
        _lblUsernameError.AutoSize = true;
        _lblUsernameError.Dock = DockStyle.Fill;
        _lblUsernameError.Font = UiStyle.SmallFont;
        _lblUsernameError.ForeColor = UiStyle.ErrorText;
        _lblUsernameError.Margin = new Padding(0, 0, 0, 8);
        _lblUsernameError.Text = string.Empty;
        _lblUsernameError.Visible = false;

        // _lblPassword
        _lblPassword.AutoSize = true;
        _lblPassword.Font = UiStyle.SmallBoldFont;
        _lblPassword.ForeColor = UiStyle.Text;
        _lblPassword.Margin = new Padding(0, 6, 0, 6);
        _lblPassword.Text = "Mật khẩu *";

        // _pnlPasswordBox
        _pnlPasswordBox.Dock = DockStyle.Fill;
        _pnlPasswordBox.AutoSize = true;
        _pnlPasswordBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _pnlPasswordBox.MinimumSize = new Size(0, 40);
        _pnlPasswordBox.BackColor = UiStyle.Surface;
        _pnlPasswordBox.Padding = new Padding(12, 10, 12, 10);
        _pnlPasswordBox.Margin = new Padding(0, 0, 0, 4);
        _pnlPasswordBox.Cursor = Cursors.IBeam;
        _pnlPasswordBox.TabIndex = 1;
        _pnlPasswordBox.TabStop = false;

        // _txtPassword
        _txtPassword.Dock = DockStyle.Top;
        _txtPassword.Font = UiStyle.InputLargeFont;
        _txtPassword.ForeColor = UiStyle.Text;
        _txtPassword.BackColor = UiStyle.Surface;
        _txtPassword.BorderStyle = BorderStyle.None;
        _txtPassword.UseSystemPasswordChar = true;
        _txtPassword.PlaceholderText = "Nhập mật khẩu";
        _txtPassword.Margin = Padding.Empty;
        _txtPassword.TabIndex = 1;
        _txtPassword.TextChanged += OnPasswordTextChanged;

        _pnlPasswordBox.Controls.Add(_txtPassword);

        // _lblPasswordError
        _lblPasswordError.AutoSize = true;
        _lblPasswordError.Dock = DockStyle.Fill;
        _lblPasswordError.Font = UiStyle.SmallFont;
        _lblPasswordError.ForeColor = UiStyle.ErrorText;
        _lblPasswordError.Margin = new Padding(0, 0, 0, 6);
        _lblPasswordError.Text = string.Empty;
        _lblPasswordError.Visible = false;

        // _chkShowPassword
        _chkShowPassword.AutoSize = true;
        _chkShowPassword.Font = UiStyle.SmallFont;
        _chkShowPassword.ForeColor = UiStyle.TextMuted;
        _chkShowPassword.Cursor = Cursors.Hand;
        _chkShowPassword.Margin = new Padding(0, 6, 0, 8);
        _chkShowPassword.TabIndex = 2;
        _chkShowPassword.Text = "Hiện mật khẩu";
        _chkShowPassword.CheckedChanged += OnShowPasswordCheckedChanged;

        // _pnlDivider
        _pnlDivider.Dock = DockStyle.Top;
        _pnlDivider.Height = 1;
        _pnlDivider.BackColor = UiStyle.Border;
        _pnlDivider.Margin = new Padding(0, 8, 0, 16);
        _pnlDivider.TabStop = false;

        // _buttonsLayout
        _buttonsLayout.Dock = DockStyle.Fill;
        _buttonsLayout.AutoSize = true;
        _buttonsLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonsLayout.ColumnCount = 3;
        _buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _buttonsLayout.RowCount = 1;
        _buttonsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _buttonsLayout.Margin = Padding.Empty;
        _buttonsLayout.TabIndex = 3;
        _buttonsLayout.TabStop = false;

        // _btnLogin
        _btnLogin.Width = 140;
        _btnLogin.Height = 40;
        _btnLogin.Margin = new Padding(10, 0, 0, 0);
        _btnLogin.TabIndex = 3;
        _btnLogin.Text = "Đăng nhập";
        UiStyle.StylePrimaryButton(_btnLogin);
        _btnLogin.Font = UiStyle.BodyBoldFont;
        _btnLogin.Click += OnLoginClick;

        // _btnCancel
        _btnCancel.Width = 108;
        _btnCancel.Height = 40;
        _btnCancel.Margin = Padding.Empty;
        _btnCancel.TabIndex = 4;
        _btnCancel.Text = "Quay lại";
        UiStyle.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += OnCancelClick;

        _buttonsLayout.Controls.Add(_btnCancel, 1, 0);
        _buttonsLayout.Controls.Add(_btnLogin, 2, 0);

        _rootLayout.Controls.Add(_lblBrand, 0, 0);
        _rootLayout.Controls.Add(_lblTitle, 0, 1);
        _rootLayout.Controls.Add(_lblSubtitle, 0, 2);
        _rootLayout.Controls.Add(_pnlErrorBanner, 0, 3);
        _rootLayout.Controls.Add(_lblUsername, 0, 4);
        _rootLayout.Controls.Add(_pnlUsernameBox, 0, 5);
        _rootLayout.Controls.Add(_lblUsernameError, 0, 6);
        _rootLayout.Controls.Add(_lblPassword, 0, 7);
        _rootLayout.Controls.Add(_pnlPasswordBox, 0, 8);
        _rootLayout.Controls.Add(_lblPasswordError, 0, 9);
        _rootLayout.Controls.Add(_chkShowPassword, 0, 10);
        _rootLayout.Controls.Add(_pnlDivider, 0, 11);
        _rootLayout.Controls.Add(_buttonsLayout, 0, 12);

        // LoginForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Surface;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(440, 430);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = _btnLogin;
        CancelButton = _btnCancel;
        Controls.Add(_rootLayout);
        Controls.Add(_pnlAccentBar);
        Name = "LoginForm";
        Text = "UniExam – Đăng nhập giảng viên";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _pnlErrorBanner.ResumeLayout(false);
        _pnlErrorBanner.PerformLayout();
        _pnlUsernameBox.ResumeLayout(false);
        _pnlUsernameBox.PerformLayout();
        _pnlPasswordBox.ResumeLayout(false);
        _pnlPasswordBox.PerformLayout();
        _buttonsLayout.ResumeLayout(false);
        _buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
