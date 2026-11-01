using ExamDesk.UI.Student;

namespace ExamDesk.UI.Administration;

// AUTH-01 • Bạn 1 phụ trách: Màn hình chọn vai trò (Giảng viên / Sinh viên).
public partial class RoleSelectionControl : UserControl
{
    public event EventHandler? LecturerLoggedIn;

    public RoleSelectionControl()
    {
        InitializeComponent();
    }

    private void OnLecturerLoginClick(object? sender, EventArgs e)
    {
        using var loginForm = new LoginForm();
        var owner = FindForm();
        var result = owner is not null ? loginForm.ShowDialog(owner) : loginForm.ShowDialog();
        if (result == DialogResult.OK)
        {
            LecturerLoggedIn?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnStudentAreaClick(object? sender, EventArgs e)
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

    private void OnHelpLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        MessageBox.Show(
            this,
            "Trợ giúp đang được dựng UI",
            "Trợ giúp (HELP-01)",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
