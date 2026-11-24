using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// AUTH-02 • Bạn 1 phụ trách: Đăng nhập giảng viên qua SQLite (AdministrationService).
public partial class LoginForm : Form
{
    private readonly AdministrationService _service;
    private bool _isSubmitting;

    public User? LoggedInUser { get; private set; }
    public string LoggedInUsername { get; private set; } = string.Empty;

    public LoginForm() : this(new AdministrationService())
    {
    }

    public LoginForm(AdministrationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();
        UiStyle.ApplyRoundedCorners(_pnlErrorBanner, 6, UiStyle.DangerBorder);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateWrappedLabelWidths();

        try
        {
            _service.EnsureDefaultLecturerAccount();
        }
        catch (Exception ex)
        {
            _lblGeneralError.Text = $"Không thể khởi tạo tài khoản mặc định: {ex.Message}";
            _pnlErrorBanner.Visible = true;
        }

        EnsureHeightFitsContent();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (IsHandleCreated)
        {
            UpdateWrappedLabelWidths();
        }
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

    private void OnLoginClick(object? sender, EventArgs e)
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

        SetSubmittingState(true);
        try
        {
            var lecturer = _service.AuthenticateLecturer(username, password);
            if (lecturer is null)
            {
                SetSubmittingState(false);
                _txtPassword.Clear();
                _lblGeneralError.Text = "Tên đăng nhập hoặc mật khẩu không đúng. Kiểm tra lại và thử lần nữa.";
                _pnlErrorBanner.Visible = true;
                EnsureHeightFitsContent();
                _txtPassword.Focus();
                return;
            }

            LoggedInUser = lecturer;
            LoggedInUsername = !string.IsNullOrWhiteSpace(lecturer.FullName)
                ? $"{lecturer.FullName} ({lecturer.Username})"
                : lecturer.Username;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _lblGeneralError.Text = $"Lỗi khi xác thực tài khoản: {ex.Message}";
            _pnlErrorBanner.Visible = true;
            EnsureHeightFitsContent();
        }
        finally
        {
            if (!IsDisposed)
            {
                SetSubmittingState(false);
            }
        }
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

    private void UpdateWrappedLabelWidths()
    {
        if (_rootLayout.ClientSize.Width <= 0)
        {
            return;
        }

        var rootContentWidth = Math.Max(240, _rootLayout.ClientSize.Width - _rootLayout.Padding.Horizontal - 4);
        _lblTitle.MaximumSize = new Size(rootContentWidth, 0);
        _lblSubtitle.MaximumSize = new Size(rootContentWidth, 0);
        _lblUsernameError.MaximumSize = new Size(rootContentWidth, 0);
        _lblPasswordError.MaximumSize = new Size(rootContentWidth, 0);
        _lblDemoNote.MaximumSize = new Size(rootContentWidth, 0);

        var bannerContentWidth = Math.Max(200, rootContentWidth - _pnlErrorBanner.Padding.Horizontal);
        _lblGeneralError.MaximumSize = new Size(bannerContentWidth, 0);
    }

    private void EnsureHeightFitsContent()
    {
        _rootLayout.PerformLayout();
        var preferredHeight = _rootLayout.GetPreferredSize(new Size(ClientSize.Width, 0)).Height;
        UiStyle.FitClientHeightWithinWorkingArea(this, preferredHeight);
    }
}
