using ExamDesk.UI.Shared;
using ExamDesk.UI.Student;

namespace ExamDesk.UI.Administration;

// AUTH-01 • Bạn 1 phụ trách: Màn hình chọn vai trò (Giảng viên / Sinh viên).
public partial class RoleSelectionControl : UserControl
{
    public event EventHandler? LecturerLoggedIn;
    public event EventHandler? OpenHelpRequested;

    public string LoggedInUsername { get; private set; } = string.Empty;

    public RoleSelectionControl()
    {
        InitializeComponent();
        UiStyle.StyleCard(_lecturerCardBorder, 12);
        UiStyle.StyleCard(_studentCardBorder, 12);
        _lecturerCardContent.SizeChanged += (_, _) => AdjustCardLabelWidths();
        _studentCardContent.SizeChanged += (_, _) => AdjustCardLabelWidths();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        AdjustCardLabelWidths();
    }

    private void AdjustCardLabelWidths()
    {
        ApplyCardWidths(_lecturerCardContent, _lblLecturerTag, _lblLecturerTitle, _lblLecturerDesc);
        ApplyCardWidths(_studentCardContent, _lblStudentTag, _lblStudentTitle, _lblStudentDesc);
    }

    private static void ApplyCardWidths(Control container, params Label[] labels)
    {
        if (container.ClientSize.Width <= 0)
        {
            return;
        }

        var maxWidth = Math.Max(180, container.ClientSize.Width - container.Padding.Horizontal);
        foreach (var label in labels)
        {
            label.MaximumSize = new Size(maxWidth, 0);
        }
    }

    private void OnLecturerLoginClick(object? sender, EventArgs e)
    {
        try
        {
            using var loginForm = new LoginForm();
            var owner = FindForm();
            var result = owner is not null ? loginForm.ShowDialog(owner) : loginForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoggedInUsername = loginForm.LoggedInUsername;
                LecturerLoggedIn?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            if (FindForm() is MainForm mainForm)
            {
                mainForm.ShowScreen(MainForm.CreateErrorScreen("Đăng nhập giảng viên", ex.Message));
            }
        }
    }

    private void OnStudentAreaClick(object? sender, EventArgs e)
    {
        try
        {
            using var studentForm = new StudentExamForm();
            var owner = FindForm();
            if (owner is not null)
            {
                studentForm.ShowDialog(owner);
            }
            else
            {
                studentForm.ShowDialog();
            }
        }
        catch (Exception ex)
        {
            if (FindForm() is MainForm mainForm)
            {
                mainForm.ShowScreen(MainForm.CreateErrorScreen("Khu vực sinh viên", ex.Message));
            }
        }
    }

    private void OnHelpLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        OpenHelpRequested?.Invoke(this, EventArgs.Empty);
    }
}
