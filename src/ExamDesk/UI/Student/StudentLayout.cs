using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Bố cục dùng riêng trong phân hệ Student, giữ bảng màu chung.
internal static class StudentLayout
{
    private static readonly Dictionary<(float Size, FontStyle Style), Font> LabelFonts = new();

    internal static Font GetLabelFont(float size, FontStyle style)
    {
        if (style == FontStyle.Regular && size == 10) return UiStyle.BodyFont;
        if (style == FontStyle.Regular && size == 9) return UiStyle.SmallFont;
        if (style == FontStyle.Bold && size == 16) return UiStyle.TitleFont;

        lock (LabelFonts)
        {
            var key = (size, style);
            if (!LabelFonts.TryGetValue(key, out var font))
            {
                font = new Font("Segoe UI", size, style);
                LabelFonts.Add(key, font);
            }
            return font;
        }
    }

    public static FlowLayoutPanel Page(UserControl control, string title)
    {
        control.AutoScaleMode = AutoScaleMode.Font;
        control.Font = UiStyle.BodyFont;
        control.BackColor = UiStyle.Background;
        control.ForeColor = UiStyle.Text;
        control.AutoScroll = true;
        var page = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(24) };
        page.Controls.Add(Label(title, 18, FontStyle.Bold));
        control.Controls.Add(page);
        page.SizeChanged += (_, _) => {
            int width = Math.Max(100, page.ClientSize.Width - page.Padding.Horizontal - 24);
            foreach (Control child in page.Controls)
            {
                if (child is Label label) label.MaximumSize = new Size(width, 0);
                else child.Width = width;
            }
        };
        return page;
    }
    public static Label Label(string text, float size = 10, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, UseMnemonic = false, Font = GetLabelFont(size, style),
        Margin = new Padding(0, 8, 0, 12), ForeColor = UiStyle.Text
    };
    public static Button Button(string text, EventHandler? click = null, bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            Padding = new Padding(12, 4, 12, 4),
            Margin = new Padding(0, 8, 12, 8)
        };
        if (primary) UiStyle.StylePrimaryButton(button);
        else UiStyle.StyleSecondaryButton(button);
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.HandleCreated += (_, _) => FitButton(button);
        button.DpiChangedAfterParent += (_, _) => FitButton(button);
        if (click is not null) button.Click += click;
        return button;
    }

    private static void FitButton(Button button)
        => button.MinimumSize = new Size(button.MinimumSize.Width, Scale(button, 38));

    internal static int Scale(Control control, int pixels)
        => (int)Math.Round(pixels * control.DeviceDpi / 96d);
}
