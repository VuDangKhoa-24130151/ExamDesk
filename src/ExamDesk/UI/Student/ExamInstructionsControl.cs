namespace ExamDesk.UI.Student;

public partial class ExamInstructionsControl : UserControl
{
    public event EventHandler? BackRequested;

    public ExamInstructionsControl()
    {
        InitializeComponent();
        Resize += (_, _) => CenterContent();
        CenterContent();
    }

    private void CenterContent()
    {
        int maxWidth = (int)(1120 * DeviceDpi / 96f);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    // Nhãn dài tự xuống dòng theo bề ngang của khung chứa nó.
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
}
