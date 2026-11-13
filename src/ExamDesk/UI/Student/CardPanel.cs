using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public sealed class CardPanel : Panel
{
    public CardPanel()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Padding = new Padding(StudentLayout.Space16);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);

        var borderWidth = Math.Max(1, StudentLayout.Scale(this, StudentLayout.Stroke1));
        var width = ClientSize.Width - borderWidth;
        var height = ClientSize.Height - borderWidth;
        if (width <= 0 || height <= 0)
        {
            return;
        }

        // Thu bán kính khi thẻ quá nhỏ để các cung không chồng nhau.
        var inset = borderWidth / 2f;
        var diameter = Math.Min(StudentLayout.Scale(this, StudentLayout.Space24), Math.Min(width, height));
        using var outline = new GraphicsPath();
        outline.AddArc(inset, inset, diameter, diameter, 180, 90);
        outline.AddArc(inset + width - diameter, inset, diameter, diameter, 270, 90);
        outline.AddArc(inset + width - diameter, inset + height - diameter, diameter, diameter, 0, 90);
        outline.AddArc(inset, inset + height - diameter, diameter, diameter, 90, 90);
        outline.CloseFigure();

        using var surface = new SolidBrush(UiStyle.Surface);
        using var border = new Pen(UiStyle.Border, borderWidth);
        var graphicsState = e.Graphics.Save();
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.FillPath(surface, outline);
        e.Graphics.DrawPath(border, outline);
        e.Graphics.Restore(graphicsState);
    }
}
