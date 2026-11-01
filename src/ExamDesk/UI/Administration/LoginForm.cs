namespace ExamDesk.UI.Administration;

// Bạn 1: Form độc lập. Thêm các UserControl/Dialog của phân hệ trong cùng thư mục.
public partial class LoginForm : Form
{
    private const string DemoUsername = "gv01";
    private const string DemoPassword = "123456";

    private bool _isSubmitting;

    public string LoggedInUsername { get; private set; } = string.Empty;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void OnUsernameTextChanged(object? sender, EventArgs e)
    {
        if (_lblUsernameError.Visible)
        {
            _lblUsernameError.Visible = false;
        }
        if (_pnlErrorBanner.Visible)
        {
            _pnlErrorBanner.Visible = false;
        }
    }

    private void OnPasswordTextChanged(object? sender, EventArgs e)
    {
        if (_lblPasswordError.Visible)
        {
            _lblPasswordError.Visible = false;
        }
        if (_pnlErrorBanner.Visible)
        {
            _pnlErrorBanner.Visible = false;
        }
    }

    private void OnShowPasswordCheckedChanged(object? sender, EventArgs e)
    {
        _txtPassword.UseSystemPasswordChar = !_chkShowPassword.Checked;
    }

    private async void OnLoginClick(object? sender, EventArgs e)
    {
        if (_isSubmitting)
        {
            return;
        }

        _pnlErrorBanner.Visible = false;
        _lblUsernameError.Visible = false;
        _lblPasswordError.Visible = false;

        var username = _txtUsername.Text.Trim();
        var password = _txtPassword.Text;

        Control? firstInvalidControl = null;

        if (string.IsNullOrWhiteSpace(username))
        {
            _lblUsernameError.Text = "Vui lòng nhập tên đăng nhập.";
            _lblUsernameError.Visible = true;
            firstInvalidControl ??= _txtUsername;
        }

        if (string.IsNullOrEmpty(password))
        {
            _lblPasswordError.Text = "Vui lòng nhập mật khẩu.";
            _lblPasswordError.Visible = true;
            firstInvalidControl ??= _txtPassword;
        }

        EnsureHeightFitsContent();

        if (firstInvalidControl is not null)
        {
            firstInvalidControl.Focus();
            return;
        }

        if (!string.Equals(username, DemoUsername, StringComparison.Ordinal) ||
            !string.Equals(password, DemoPassword, StringComparison.Ordinal))
        {
            _lblGeneralError.Text = "Tên đăng nhập hoặc mật khẩu không đúng. Kiểm tra lại và thử lần nữa.";
            _pnlErrorBanner.Visible = true;
            _txtPassword.Clear();
            EnsureHeightFitsContent();
            _txtPassword.Focus();
            return;
        }

        SetSubmittingState(true);
        await Task.Delay(500);

        if (IsDisposed || !IsHandleCreated)
        {
            return;
        }

        LoggedInUsername = username;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCancelClick(object? sender, EventArgs e)
    {
        if (_isSubmitting)
        {
            return;
        }

        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void SetSubmittingState(bool submitting)
    {
        _isSubmitting = submitting;
        _txtUsername.Enabled = !submitting;
        _txtPassword.Enabled = !submitting;
        _chkShowPassword.Enabled = !submitting;
        _btnLogin.Enabled = !submitting;
        _btnCancel.Enabled = !submitting;
        _btnLogin.Text = submitting ? "Đang đăng nhập…" : "Đăng nhập";
    }

    private void EnsureHeightFitsContent()
    {
        _rootLayout.PerformLayout();
        var preferredHeight = _rootLayout.GetPreferredSize(new Size(ClientSize.Width, 0)).Height;
        if (preferredHeight > ClientSize.Height)
        {
            ClientSize = new Size(ClientSize.Width, preferredHeight);
        }
    }
}
