#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private Panel _pnlErrorBanner = null!;
    private Label _lblGeneralError = null!;
    private Label _lblUsername = null!;
    private TextBox _txtUsername = null!;
    private Label _lblUsernameError = null!;
    private Label _lblPassword = null!;
    private TextBox _txtPassword = null!;
    private Label _lblPasswordError = null!;
    private CheckBox _chkShowPassword = null!;
    private Label _lblDemoNote = null!;
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
        _rootLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _lblSubtitle = new Label();
        _pnlErrorBanner = new Panel();
        _lblGeneralError = new Label();
        _lblUsername = new Label();
        _txtUsername = new TextBox();
        _lblUsernameError = new Label();
        _lblPassword = new Label();
        _txtPassword = new TextBox();
        _lblPasswordError = new Label();
        _chkShowPassword = new CheckBox();
        _lblDemoNote = new Label();
        _buttonsLayout = new TableLayoutPanel();
        _btnCancel = new Button();
        _btnLogin = new Button();

        _rootLayout.SuspendLayout();
        _pnlErrorBanner.SuspendLayout();
        _buttonsLayout.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Surface;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 12;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Title
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Subtitle
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: General error banner
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3: Username label
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 4: Username textbox
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 5: Username error
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 6: Password label
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 7: Password textbox
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 8: Password error
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 9: Show password checkbox
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 10: Demo note
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 11: Buttons

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
        _lblSubtitle.Margin = new Padding(0, 0, 0, 12);
        _lblSubtitle.Text = "Nhập thông tin tài khoản để truy cập phân hệ quản trị và câu hỏi";

        // _pnlErrorBanner
        _pnlErrorBanner.Dock = DockStyle.Fill;
        _pnlErrorBanner.AutoSize = true;
        _pnlErrorBanner.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _pnlErrorBanner.BackColor = UiStyle.ErrorBack;
        _pnlErrorBanner.Padding = new Padding(12, 8, 12, 8);
        _pnlErrorBanner.Margin = new Padding(0, 0, 0, 12);
        _pnlErrorBanner.Visible = false;

        // _lblGeneralError
        _lblGeneralError.AutoSize = true;
        _lblGeneralError.Dock = DockStyle.Fill;
        _lblGeneralError.Font = UiStyle.SmallFont;
        _lblGeneralError.ForeColor = UiStyle.ErrorText;
        _lblGeneralError.Margin = new Padding(0);
        _lblGeneralError.Text = string.Empty;

        _pnlErrorBanner.Controls.Add(_lblGeneralError);

        // _lblUsername
        _lblUsername.AutoSize = true;
        _lblUsername.Font = UiStyle.BodyFont;
        _lblUsername.ForeColor = UiStyle.Text;
        _lblUsername.Margin = new Padding(0, 0, 0, 6);
        _lblUsername.Text = "Tên đăng nhập *";

        // _txtUsername
        _txtUsername.Dock = DockStyle.Fill;
        _txtUsername.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        _txtUsername.ForeColor = UiStyle.Text;
        _txtUsername.BackColor = UiStyle.Surface;
        _txtUsername.BorderStyle = BorderStyle.FixedSingle;
        _txtUsername.Margin = new Padding(0, 0, 0, 4);
        _txtUsername.TabIndex = 0;
        _txtUsername.TextChanged += OnUsernameTextChanged;

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
        _lblPassword.Font = UiStyle.BodyFont;
        _lblPassword.ForeColor = UiStyle.Text;
        _lblPassword.Margin = new Padding(0, 4, 0, 6);
        _lblPassword.Text = "Mật khẩu *";

        // _txtPassword
        _txtPassword.Dock = DockStyle.Fill;
        _txtPassword.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        _txtPassword.ForeColor = UiStyle.Text;
        _txtPassword.BackColor = UiStyle.Surface;
        _txtPassword.BorderStyle = BorderStyle.FixedSingle;
        _txtPassword.UseSystemPasswordChar = true;
        _txtPassword.Margin = new Padding(0, 0, 0, 4);
        _txtPassword.TabIndex = 1;
        _txtPassword.TextChanged += OnPasswordTextChanged;

        // _lblPasswordError
        _lblPasswordError.AutoSize = true;
        _lblPasswordError.Dock = DockStyle.Fill;
        _lblPasswordError.Font = UiStyle.SmallFont;
        _lblPasswordError.ForeColor = UiStyle.ErrorText;
        _lblPasswordError.Margin = new Padding(0, 0, 0, 8);
        _lblPasswordError.Text = string.Empty;
        _lblPasswordError.Visible = false;

        // _chkShowPassword
        _chkShowPassword.AutoSize = true;
        _chkShowPassword.Font = UiStyle.SmallFont;
        _chkShowPassword.ForeColor = UiStyle.Text;
        _chkShowPassword.Margin = new Padding(0, 4, 0, 12);
        _chkShowPassword.TabIndex = 2;
        _chkShowPassword.Text = "Hiện mật khẩu";
        _chkShowPassword.CheckedChanged += OnShowPasswordCheckedChanged;

        // _lblDemoNote
        _lblDemoNote.AutoSize = true;
        _lblDemoNote.Dock = DockStyle.Fill;
        _lblDemoNote.Font = UiStyle.SmallFont;
        _lblDemoNote.ForeColor = UiStyle.TextMuted;
        _lblDemoNote.TextAlign = ContentAlignment.BottomLeft;
        _lblDemoNote.Margin = new Padding(0, 4, 0, 12);
        _lblDemoNote.Text = "Bản dựng UI – tài khoản minh họa: gv01 / 123456";

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
        _buttonsLayout.Margin = new Padding(0);
        _buttonsLayout.TabIndex = 3;

        // _btnLogin
        _btnLogin.Width = 140;
        _btnLogin.Height = 40;
        _btnLogin.Margin = new Padding(8, 0, 0, 0);
        _btnLogin.TabIndex = 3;
        _btnLogin.Text = "Đăng nhập";
        UiStyle.StylePrimaryButton(_btnLogin);
        _btnLogin.Click += OnLoginClick;

        // _btnCancel
        _btnCancel.Width = 110;
        _btnCancel.Height = 40;
        _btnCancel.Margin = new Padding(0);
        _btnCancel.TabIndex = 4;
        _btnCancel.Text = "Quay lại";
        UiStyle.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += OnCancelClick;

        _buttonsLayout.Controls.Add(_btnCancel, 1, 0);
        _buttonsLayout.Controls.Add(_btnLogin, 2, 0);

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_lblSubtitle, 0, 1);
        _rootLayout.Controls.Add(_pnlErrorBanner, 0, 2);
        _rootLayout.Controls.Add(_lblUsername, 0, 3);
        _rootLayout.Controls.Add(_txtUsername, 0, 4);
        _rootLayout.Controls.Add(_lblUsernameError, 0, 5);
        _rootLayout.Controls.Add(_lblPassword, 0, 6);
        _rootLayout.Controls.Add(_txtPassword, 0, 7);
        _rootLayout.Controls.Add(_lblPasswordError, 0, 8);
        _rootLayout.Controls.Add(_chkShowPassword, 0, 9);
        _rootLayout.Controls.Add(_lblDemoNote, 0, 10);
        _rootLayout.Controls.Add(_buttonsLayout, 0, 11);

        // LoginForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Surface;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(460, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = _btnLogin;
        CancelButton = _btnCancel;
        Controls.Add(_rootLayout);
        Name = "LoginForm";
        Text = "UniExam – Đăng nhập giảng viên";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _pnlErrorBanner.ResumeLayout(false);
        _pnlErrorBanner.PerformLayout();
        _buttonsLayout.ResumeLayout(false);
        _buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
