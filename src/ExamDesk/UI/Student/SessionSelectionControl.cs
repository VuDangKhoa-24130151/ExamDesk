using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public event EventHandler? InstructionsRequested;
    public event EventHandler? CloseRequested;
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

    private void OpenInstructions(object? sender, EventArgs e)
    {
        InstructionsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}



