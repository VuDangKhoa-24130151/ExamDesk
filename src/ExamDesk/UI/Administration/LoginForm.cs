using System.Drawing.Drawing2D;
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
        UiStyle.ApplyRoundedCorners(_pnlErrorBanner, 6, UiStyle.ErrorText);

        _pnlUsernameBox.Paint += OnInputBoxPaint;
        _pnlPasswordBox.Paint += OnInputBoxPaint;
        _pnlUsernameBox.SizeChanged += (_, _) => _pnlUsernameBox.Invalidate();
        _pnlPasswordBox.SizeChanged += (_, _) => _pnlPasswordBox.Invalidate();
        _pnlUsernameBox.Click += (_, _) => _txtUsername.Focus();
        _pnlPasswordBox.Click += (_, _) => _txtPassword.Focus();
        _txtUsername.Enter += (_, _) => UpdateInputBoxBorders();
        _txtUsername.Leave += (_, _) => UpdateInputBoxBorders();
        _txtPassword.Enter += (_, _) => UpdateInputBoxBorders();
        _txtPassword.Leave += (_, _) => UpdateInputBoxBorders();
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
        var hadError = false;
        if (_lblUsernameError.Visible)
        {
            _lblUsernameError.Visible = false;
            hadError = true;
        }
        if (_pnlErrorBanner.Visible)
        {
            _pnlErrorBanner.Visible = false;
            hadError = true;
        }

        UpdateInputBoxBorders();
        if (hadError)
        {
            EnsureHeightFitsContent();
        }
    }

    private void OnPasswordTextChanged(object? sender, EventArgs e)
    {
        var hadError = false;
        if (_lblPasswordError.Visible)
        {
            _lblPasswordError.Visible = false;
            hadError = true;
        }
        if (_pnlErrorBanner.Visible)
        {
            _pnlErrorBanner.Visible = false;
            hadError = true;
        }

        UpdateInputBoxBorders();
        if (hadError)
        {
            EnsureHeightFitsContent();
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
        UpdateInputBoxBorders();

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

    private void OnInputBoxPaint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel box || box.Width < 4 || box.Height < 4)
        {
            return;
        }

        var isUsername = ReferenceEquals(box, _pnlUsernameBox);
        var hasError = isUsername ? _lblUsernameError.Visible : _lblPasswordError.Visible;
        var isFocused = isUsername ? _txtUsername.Focused : _txtPassword.Focused;

        Color borderColor;
        float borderWidth;
        if (hasError)
        {
            borderColor = UiStyle.ErrorText;
            borderWidth = 1.5F;
        }
        else if (isFocused)
        {
            borderColor = UiStyle.Primary;
            borderWidth = 1.75F;
        }
        else
        {
            borderColor = UiStyle.Border;
            borderWidth = 1F;
        }

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var inset = borderWidth > 1F ? 1 : 0;
        var bounds = new Rectangle(
            inset,
            inset,
            Math.Max(1, box.Width - 1 - inset * 2),
            Math.Max(1, box.Height - 1 - inset * 2));
        using var path = UiStyle.CreateRoundedRectPath(bounds, 6);
        using var pen = new Pen(borderColor, borderWidth);
        e.Graphics.DrawPath(pen, path);
    }

    private void UpdateInputBoxBorders()
    {
        _pnlUsernameBox.Invalidate();
        _pnlPasswordBox.Invalidate();
    }

    private void UpdateWrappedLabelWidths()
    {
        if (_rootLayout.ClientSize.Width <= 0)
        {
            return;
        }

        var rootContentWidth = Math.Max(240, _rootLayout.ClientSize.Width - _rootLayout.Padding.Horizontal - 4);
        _lblSubtitle.MaximumSize = new Size(rootContentWidth, 0);
        _lblUsernameError.MaximumSize = new Size(rootContentWidth, 0);
        _lblPasswordError.MaximumSize = new Size(rootContentWidth, 0);

        var bannerContentWidth = Math.Max(200, rootContentWidth - _pnlErrorBanner.Padding.Horizontal);
        _lblGeneralError.MaximumSize = new Size(bannerContentWidth, 0);
    }

    private void EnsureHeightFitsContent()
    {
        if (WindowState != FormWindowState.Normal)
        {
            return;
        }

        _rootLayout.PerformLayout();
        var width = ClientSize.Width;
        var preferredClientHeight = _pnlAccentBar.Height + _rootLayout.GetPreferredSize(new Size(width, 0)).Height;

        var screen = Owner is not null ? Screen.FromControl(Owner) : Screen.FromControl(this);
        var workingArea = screen.WorkingArea;
        var nonClientHeight = Math.Max(0, Height - ClientSize.Height);
        var maxClientHeight = Math.Max(220, workingArea.Height - nonClientHeight - 24);
        var targetClientHeight = Math.Min(preferredClientHeight, maxClientHeight);

        if (ClientSize.Height != targetClientHeight)
        {
            ClientSize = new Size(width, targetClientHeight);
        }

        UiStyle.ClampToWorkingArea(this);
    }
}
