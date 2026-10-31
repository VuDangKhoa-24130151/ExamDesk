using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public SessionSelectionControl()
    {
        InitializeComponent();
        Resize += (_, _) => CenterContent();
        CenterContent();
        Load += (_, _) => SelectNextControl(null, true, true, true, false);
    }

    // Màn này ít thông tin nên giới hạn bề ngang, phần dư chia đều sang hai bên.
    private void CenterContent()
    {
        int maxWidth = (int)(1040 * DeviceDpi / 96f);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    // Giới hạn chiều rộng nhãn để chữ dài xuống dòng, không tràn sang khung bên cạnh.
    private static void WrapLabels(FlowLayoutPanel panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;

        foreach (Control control in panel.Controls)
        {
            if (control is Label label)
                label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
        }
    }

    // Chưa có SV-02 nên tạm thông báo, sau này đổi thành mở màn hướng dẫn.
    private void OpenInstructions(object? sender, EventArgs e)
    {
        MessageBox.Show(this,
            "Màn hướng dẫn thi đang được xây dựng. Bạn chưa bắt đầu làm bài.",
            "Hướng dẫn thi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Khi có màn nhận diện, nút này sẽ đưa sinh viên quay về nhập mã.
    private void ChangeIdentity(object? sender, EventArgs e)
    {
        MessageBox.Show(this,
            "Chức năng nhập lại mã sinh viên đang được xây dựng.",
            "Nhập lại mã sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}



