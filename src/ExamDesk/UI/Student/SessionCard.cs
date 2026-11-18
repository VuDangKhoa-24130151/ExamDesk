using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Thẻ ca thi tự vẽ; nút hành động đặt bằng tọa độ nên không bị cắt ở mép phải.
internal sealed class SessionCard : Panel
{
    private readonly string _title;
    private readonly string _paper;
    private readonly string _meta;
    private readonly string _state;
    private readonly string _reason;
    private readonly bool _compact;
    private readonly Button _action;
    private readonly ToolTip _tooltip = new();
    private bool _hovered;
    private int _measuredWidth;
    private int _measuredDpi;

    private const TextFormatFlags WrapFlags = TextFormatFlags.NoPrefix
        | TextFormatFlags.NoPadding | TextFormatFlags.WordBreak;
    private const TextFormatFlags SingleFlags = TextFormatFlags.NoPrefix
        | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

    internal SessionCard(string title, string paper, string meta, string state,
        string reason, bool compact, Button action)
    {
        _title = title;
        _paper = paper;
        _meta = meta;
        _state = state;
        _reason = reason;
        _compact = compact;
        _action = action;

        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
        BackColor = StateColors(state).Back;
        Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));
        Dock = DockStyle.None;
        TabStop = false;
        AccessibleRole = AccessibleRole.Grouping;
        AccessibleName = $"{title} · {state}";
        AccessibleDescription = $"{paper} · {meta}" + (reason.Length > 0 ? $". {reason}" : "");
        action.AccessibleDescription = $"{state}. {AccessibleDescription}";

        Controls.Add(action);
        action.MouseEnter += (_, _) => SetHovered(true);
        action.MouseLeave += (_, _) => SetHovered(ClientRectangle.Contains(PointToClient(Cursor.Position)));
        _tooltip.SetToolTip(this, $"{title}\n{state}\n{AccessibleDescription}");
        _tooltip.SetToolTip(action, $"{action.AccessibleName}\n{AccessibleDescription}");
        ApplyLayout();
    }

    private int S(int value) => StudentLayout.Scale(this, value);
    private string BadgeText => _state == "Đang mở" ? "● Đang mở" : _state;
    private string PaperText => _compact ? $"{_paper} · {_meta}" : _paper;
    private Font TitleFont => StudentLayout.GetLabelFont(StudentLayout.SubheadingFontSize, FontStyle.Bold);
    private Font BodyFont => StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Regular);

    private static (Color Fore, Color Back) StateColors(string state) => state switch
    {
        "Đang mở" => (UiStyle.SuccessText, UiStyle.SuccessBack),
        "Tiếp tục lượt thi" => (UiStyle.Primary, StudentLayout.SelectionBack),
        "Ca đã đóng" => (UiStyle.WarningText, UiStyle.WarningBack),
        "Đã khóa" => (UiStyle.WarningText, UiStyle.WarningBack),
        _ => (UiStyle.TextMuted, UiStyle.Background)
    };

    private sealed record Metrics(Rectangle Badge, Rectangle Title, Rectangle Paper,
        Rectangle Meta, Rectangle Reason, Rectangle Action, int Height);

    private int TextWidth(string text, Font font) =>
        TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), SingleFlags).Width;

    private int TextHeight(string text, Font font, int width) =>
        TextRenderer.MeasureText(text, font, new Size(Math.Max(1, width), int.MaxValue), WrapFlags).Height;

    private Metrics Measure(int width)
    {
        int pad = S(StudentLayout.Space16);
        int gap = S(StudentLayout.Space8);
        int right = width - pad;

        int minActionWidth = _state == "Tiếp tục lượt thi" ? S(StudentLayout.SessionContinueWidth) : 0;
        int actionW = Math.Max(Math.Max(_action.Width, _action.MinimumSize.Width), minActionWidth);
        int actionH = Math.Max(_action.Height, _state == "Tiếp tục lượt thi" ? S(StudentLayout.SessionContinueHeight) : 0);

        // Badge đo bằng font thật, không dùng Size.Empty (sẽ làm chữ bị cắt).
        int badgeW = Math.Min(TextWidth(BadgeText, BodyFont) + S(StudentLayout.Space16),
            Math.Max(S(StudentLayout.Space24), width - 2 * pad));
        int badgeH = BodyFont.Height + gap;
        var badge = new Rectangle(right - badgeW, pad, badgeW, badgeH);

        bool narrow = width < S(StudentLayout.SessionRowBreakpoint);
        int titleW = badge.Left - gap - pad;
        bool stacked = narrow || titleW < S(StudentLayout.SessionTitleMinWidth);
        if (stacked) titleW = Math.Max(1, right - pad);
        int titleY = stacked ? badge.Bottom + gap : pad;
        var title = new Rectangle(pad, titleY, titleW, TextHeight(_title, TitleFont, titleW));
        int textTop = Math.Max(title.Bottom, badge.Bottom) + gap;

        int bodyW = narrow
            ? width - 2 * pad
            : width - 2 * pad - actionW - S(StudentLayout.Space16);
        bodyW = Math.Max(S(StudentLayout.Space24), bodyW);

        var paper = new Rectangle(pad, textTop, bodyW, TextHeight(PaperText, BodyFont, bodyW));
        int y = paper.Bottom;
        var meta = Rectangle.Empty;
        var reason = Rectangle.Empty;
        if (!_compact)
        {
            meta = new Rectangle(pad, y + gap, bodyW, TextHeight(_meta, BodyFont, bodyW));
            y = meta.Bottom;
            if (_reason.Length > 0)
            {
                reason = new Rectangle(pad, y + gap, bodyW, TextHeight(_reason, BodyFont, bodyW));
                y = reason.Bottom;
            }
        }
        int textBottom = y;

        Rectangle actionRect;
        int height;
        if (narrow)
        {
            // Màn hẹp: nút nằm dưới nội dung, căn trái.
            actionRect = new Rectangle(pad, textBottom + gap, actionW, actionH);
            height = actionRect.Bottom + pad;
        }
        else
        {
            // Màn rộng: nút nằm bên phải, căn giữa theo khối chữ.
            int ay = textTop + Math.Max(0, (textBottom - textTop - actionH) / 2);
            actionRect = new Rectangle(right - actionW, ay, actionW, actionH);
            height = Math.Max(textBottom, actionRect.Bottom) + pad;
        }

        return new Metrics(badge, title, paper, meta, reason, actionRect, height);
    }

    private void ApplyLayout()
    {
        if (_action is null || ClientSize.Width <= 0
            || (_measuredWidth == ClientSize.Width && _measuredDpi == DeviceDpi)) return;
        // Cache first: nested Layout/Resize sees the completed measurement.
        _measuredWidth = ClientSize.Width;
        _measuredDpi = DeviceDpi;
        var m = Measure(_measuredWidth);
        if (_action.Bounds != m.Action) _action.Bounds = m.Action;
        if (Height != m.Height) Height = m.Height;
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        int width = proposedSize.Width > 0 && proposedSize.Width < int.MaxValue
            ? proposedSize.Width
            : Width;
        return new Size(width, Measure(Math.Max(1, width)).Height);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        ApplyLayout();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(UiStyle.Background);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (ClientSize.Width <= 0) return;
        var m = Measure(ClientSize.Width);
        var colors = StateColors(_state);
        var g = e.Graphics;
        int stroke = S(StudentLayout.Stroke1);

        g.Clear(UiStyle.Background);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var bounds = Rectangle.Inflate(ClientRectangle, -stroke, -stroke);
        using var path = StudentLayout.RoundedRectangle(this, bounds);
        using (var fill = new SolidBrush(colors.Back))
            g.FillPath(fill, path);

        var saved = g.Save();
        g.SetClip(path, CombineMode.Intersect);
        using (var accent = new SolidBrush(colors.Fore))
            g.FillRectangle(accent, bounds.Left, bounds.Top, S(StudentLayout.Space4), bounds.Height);
        g.Restore(saved);

        using (var border = new Pen(_hovered ? StudentLayout.SessionCardHoverBorder : StudentLayout.SessionCardBorder, stroke))
            g.DrawPath(border, path);

        // Badge trắng viền màu để nổi trên nền trạng thái.
        using (var badgeFill = new SolidBrush(UiStyle.Surface))
            g.FillRectangle(badgeFill, m.Badge);
        TextRenderer.DrawText(g, BadgeText, BodyFont, m.Badge, colors.Fore,
            SingleFlags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        TextRenderer.DrawText(g, _title, TitleFont, m.Title, UiStyle.Text, WrapFlags);
        TextRenderer.DrawText(g, PaperText, BodyFont, m.Paper, UiStyle.Text, WrapFlags);
        if (!_compact)
        {
            TextRenderer.DrawText(g, _meta, BodyFont, m.Meta, UiStyle.TextMuted, WrapFlags);
            if (_reason.Length > 0)
                TextRenderer.DrawText(g, _reason, BodyFont, m.Reason,
                    _action.Enabled ? UiStyle.TextMuted : colors.Fore, WrapFlags);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        SetHovered(true);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        SetHovered(ClientRectangle.Contains(PointToClient(Cursor.Position)));
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));
        (_action as StudentButton)?.FitSize();
        ApplyLayout();
    }

    private void SetHovered(bool hovered)
    {
        if (_hovered == hovered) return;
        _hovered = hovered;
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _tooltip.Dispose();
        base.Dispose(disposing);
    }
}
