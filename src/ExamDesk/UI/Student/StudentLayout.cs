using System.ComponentModel;
using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

internal enum StudentButtonKind
{
    Secondary,
    Primary,
    Ghost,
    FlagToggle
}

internal enum StudentArrow
{
    None,
    Left,
    Right
}

// Bố cục dùng riêng trong phân hệ Student, giữ bảng màu chung.
internal static class StudentLayout
{
    internal const int Space4 = 4;
    internal const int Space8 = 8;
    internal const int Space12 = 12;
    internal const int Space16 = 16;
    internal const int Space24 = 24;

    internal const int Stroke1 = 1;
    internal const int Stroke2 = 2;

    internal const int SmallRadius = 4;
    internal const int ButtonRadius = 6;
    internal const int CardRadius = 8;
    internal const int FocusRadius = 12;
    internal const int ChipRadius = 16;
    internal const int PillRadius = 20;

    internal const int ButtonHeight = 36;
    internal const int ButtonPaddingX = 12;
    internal const int ChipHeight = 32;
    internal const int ChipMinWidth = 64;
    internal const int BarHeight = 56;
    internal const int DotSize = 8;
    internal const int ArrowSpace = 16;

    internal const int HeadingHeight = 24;
    internal const int LegendBoxSize = 12;
    internal const int LegendRowHeight = 20;
    internal const int LegendTextLeft = 20;

    internal const int OptionFocusInset = Space4;
    internal const int OptionCardPaddingY = Space12;
    internal const int OptionVerticalPadding = (OptionFocusInset + OptionCardPaddingY) * 2;
    internal const int OptionMinHeight = 60;
    internal const int OptionBadgeLeft = 20;
    internal const int OptionBadgeSize = 28;
    internal const int OptionTextLeft = 64;
    internal const int OptionTextReservedWidth = 104;
    internal const int OptionCheckRightInset = 28;

    internal const int TileSize = 40;
    internal const int TileGap = Space8;
    internal const int TileStep = TileSize + TileGap;
    internal const int GridColumnsWide = 5;
    internal const int GridColumnsNarrow = 4;

    internal const int SidebarWidth = 320;
    internal const int ClockWidth = 184;
    internal const int ImageMaxWidth = 480;
    internal const int ImageMaxHeight = 220;
    internal const int ImageDialogWidth = 840;
    internal const int ImageDialogHeight = 600;
    internal const int ImageDialogMinWidth = 320;
    internal const int ImageDialogMinHeight = 240;
    internal const int HelpDialogWidth = 520;
    internal const int HelpDialogHeight = 380;

    internal const int SubmitDialogWidth = 560;
    internal const int SubmitDialogHeight = 440;
    internal const int SubmitDialogMinWidth = 460;
    internal const int SubmitDialogMinHeight = 340;
    internal const int DialogClockWidth = 112;
    internal const int DialogHeaderRightReserve = 176;

    internal const int IdentityCardWidth = 460;
    internal const int SessionPageMaxWidth = 1040;
    internal const int SessionRowBreakpoint = 640;
    internal const int InstructionsMaxWidth = 1120;
    internal const int StatusCardMaxWidth = 820;

    internal const int FormClientWidth = 1280;
    internal const int FormClientHeight = 680;
    internal const int FormMinWidth = 1000;
    internal const int FormMinHeight = 600;

    internal const float SmallFontSize = 9f;
    internal const float BodyFontSize = 10f;
    internal const float SubheadingFontSize = 11f;
    internal const float HeadingFontSize = 12f;
    internal const float ClockFontSize = 14f;
    internal const float TitleFontSize = 16f;
    internal const float PageTitleFontSize = 18f;

    // Màu bổ sung để Bạn 1 duyệt: 8% Primary trên trắng (#EDF2FA với theme hiện tại).
    internal static readonly Color SelectionBack = Blend(UiStyle.Primary, Color.White, 0.08);
    // Màu bổ sung để Bạn 1 duyệt: nền ô đã trả lời 16% Primary trên trắng (#DAE5F4 với theme hiện tại).
    internal static readonly Color AnsweredBack = Blend(UiStyle.Primary, Color.White, 0.16);
    // Viền ô đã trả lời để Bạn 1 duyệt: viền xanh vừa 45% Primary trên trắng (#97B5E1 với theme hiện tại).
    internal static readonly Color AnsweredBorder = Blend(UiStyle.Primary, Color.White, 0.45);
    // Nền vàng rõ hơn cho ô đánh dấu xem lại để Bạn 1 duyệt (#FFE8A3 với theme hiện tại).
    internal static readonly Color FlaggedBack = ColorTranslator.FromHtml("#FFE8A3");
    // Viền hover: 15% TextMuted trên Border (#C7D0DC với theme hiện tại).
    internal static readonly Color OptionHoverBorder = Blend(UiStyle.TextMuted, UiStyle.Border, 0.15);
    // Nền nhấn nút phụ/ghost: 12% TextMuted trên Background (#E0E6EF với theme hiện tại).
    internal static readonly Color ButtonPressedBack = Blend(UiStyle.TextMuted, UiStyle.Background, 0.12);
    // Nền hover/pressed nút chính: trộn đen 12%/22% trên Primary (#154FA6 / #12448F).
    internal static readonly Color PrimaryHover = Blend(Color.Black, UiStyle.Primary, 0.12);
    internal static readonly Color PrimaryPressed = Blend(Color.Black, UiStyle.Primary, 0.22);
    // Nền hover/pressed và viền nút đánh dấu khi bật trên nền cảnh báo (#F3E5C5 / #EAD7B3 / #BDAA83).
    internal static readonly Color WarningHoverBack = Blend(UiStyle.WarningText, UiStyle.WarningBack, 0.10);
    internal static readonly Color WarningPressedBack = Blend(UiStyle.WarningText, UiStyle.WarningBack, 0.18);
    internal static readonly Color WarningBorder = Blend(UiStyle.WarningText, UiStyle.Border, 0.35);

    private static Color Blend(Color foreground, Color background, double amount) => Color.FromArgb(
        (int)Math.Round(foreground.R * amount + background.R * (1 - amount)),
        (int)Math.Round(foreground.G * amount + background.G * (1 - amount)),
        (int)Math.Round(foreground.B * amount + background.B * (1 - amount)));

    internal static void DrawHeaderBorder(Control control, Graphics graphics)
    {
        int thickness = Scale(control, Stroke1);
        using var border = new SolidBrush(UiStyle.Border);
        graphics.FillRectangle(border, 0, control.Height - thickness, control.Width, thickness);
    }

    internal static void DrawTopBorder(Control control, Graphics graphics)
    {
        int thickness = Scale(control, Stroke1);
        using var border = new SolidBrush(UiStyle.Border);
        graphics.FillRectangle(border, 0, 0, control.Width, thickness);
    }

    internal static void DrawDot(Graphics graphics, Rectangle bounds, Color color)
    {
        using var path = new GraphicsPath();
        path.AddEllipse(bounds);
        using var fill = new SolidBrush(color);
        graphics.FillPath(fill, path);
    }

    internal static GraphicsPath RoundedRectangle(Control control, Rectangle bounds, int radius = CardRadius)
    {
        var path = new GraphicsPath();
        int diameter = Math.Min(Scale(control, radius) * 2, Math.Min(bounds.Width, bounds.Height));
        if (diameter <= 0) return path;
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    internal static void DrawCheck(Graphics graphics, Control control, Point origin, Color color)
    {
        int S(int value) => Scale(control, value);
        using var pen = new Pen(color, S(Stroke2))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };
        graphics.DrawLines(pen,
        [
            origin,
            new Point(origin.X + S(Space4), origin.Y + S(Space4)),
            new Point(origin.X + S(Space12), origin.Y - S(Space4))
        ]);
    }

    internal static void DrawArrow(Graphics graphics, Control control, Point center, bool right, Color color)
    {
        int S(int value) => Scale(control, value);
        int dir = right ? 1 : -1;
        using var pen = new Pen(color, S(Stroke2))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };
        graphics.DrawLine(pen, center.X - dir * S(Space4), center.Y, center.X + dir * S(Space4), center.Y);
        graphics.DrawLines(pen,
        [
            new Point(center.X, center.Y - S(Space4)),
            new Point(center.X + dir * S(Space4), center.Y),
            new Point(center.X, center.Y + S(Space4))
        ]);
    }

    private static readonly Dictionary<(float Size, FontStyle Style), Font> LabelFonts = new();

    internal static Font GetLabelFont(float size, FontStyle style)
    {
        if (style == FontStyle.Regular && size == BodyFontSize) return UiStyle.BodyFont;
        if (style == FontStyle.Regular && size == SmallFontSize) return UiStyle.SmallFont;
        if (style == FontStyle.Bold && size == TitleFontSize) return UiStyle.TitleFont;

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

    public static Label Label(string text, float size = BodyFontSize, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text,
        AutoSize = true,
        UseMnemonic = false,
        Font = GetLabelFont(size, style),
        Margin = new Padding(0, Space8, 0, Space12),
        ForeColor = UiStyle.Text
    };

    public static Button Button(string text, EventHandler? click = null, bool primary = false) =>
        CreateButton(text, click, primary ? StudentButtonKind.Primary : StudentButtonKind.Secondary, StudentArrow.None);

    internal static StudentButton CreateButton(string text, EventHandler? click = null,
        StudentButtonKind kind = StudentButtonKind.Secondary, StudentArrow arrow = StudentArrow.None)
    {
        var button = new StudentButton(text, kind, arrow)
        {
            Margin = new Padding(0, Space8, Space12, Space8)
        };
        if (click is not null) button.Click += click;
        return button;
    }

    internal static void SetButtonPrimary(Button button, bool primary)
    {
        if (button is StudentButton studentButton)
        {
            studentButton.Kind = primary ? StudentButtonKind.Primary : StudentButtonKind.Secondary;
            return;
        }
        if (primary) UiStyle.StylePrimaryButton(button);
        else UiStyle.StyleSecondaryButton(button);
    }

    internal static int Scale(Control control, int pixels)
        => (int)Math.Round(pixels * control.DeviceDpi / 96d);
}

internal sealed class StudentButton : Button
{
    private StudentButtonKind _kind;
    private StudentArrow _arrow;
    private bool _toggled;
    private bool _hovered;
    private bool _pressed;

    internal StudentButton(string text, StudentButtonKind kind = StudentButtonKind.Secondary,
        StudentArrow arrow = StudentArrow.None)
    {
        _kind = kind;
        _arrow = arrow;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        AutoSize = false;
        UseMnemonic = true;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Surface;
        ForeColor = UiStyle.Text;
        Cursor = Cursors.Hand;
        Text = text;
        HandleCreated += (_, _) => FitSize();
        DpiChangedAfterParent += (_, _) => FitSize();
        FitSize();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal StudentButtonKind Kind
    {
        get => _kind;
        set
        {
            if (_kind == value) return;
            _kind = value;
            FitSize();
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal StudentArrow Arrow
    {
        get => _arrow;
        set
        {
            if (_arrow == value) return;
            _arrow = value;
            FitSize();
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool Toggled
    {
        get => _toggled;
        set
        {
            if (_toggled == value) return;
            _toggled = value;
            Invalidate();
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        }
    }

    internal void FitSize()
    {
        int S(int value) => StudentLayout.Scale(this, value);
        Padding = new Padding(S(StudentLayout.ButtonPaddingX), 0, S(StudentLayout.ButtonPaddingX), 0);
        int textWidth = TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix | TextFormatFlags.NoPadding).Width;
        int iconSpace = _arrow != StudentArrow.None ? S(StudentLayout.ArrowSpace) : 0;
        int height = S(StudentLayout.ButtonHeight);
        int width = Math.Max(height, textWidth + Padding.Horizontal + iconSpace + S(StudentLayout.Space4));
        MinimumSize = Size = new Size(width, height);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        FitSize();
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _hovered = Enabled;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = false;
        _pressed = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left && Enabled)
        {
            _pressed = true;
            Invalidate();
        }
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        if (_pressed)
        {
            _pressed = false;
            Invalidate();
        }
        base.OnMouseUp(mevent);
    }

    protected override bool IsInputKey(Keys keyData) =>
        keyData == Keys.Enter || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs kevent)
    {
        if (kevent.KeyCode == Keys.Enter && Enabled)
        {
            kevent.SuppressKeyPress = true;
            PerformClick();
            return;
        }
        if (kevent.KeyCode == Keys.Space && Enabled)
        {
            _pressed = true;
            Invalidate();
        }
        base.OnKeyDown(kevent);
    }

    protected override void OnKeyUp(KeyEventArgs kevent)
    {
        if (kevent.KeyCode == Keys.Space && _pressed)
        {
            _pressed = false;
            Invalidate();
        }
        base.OnKeyUp(kevent);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Cursor = Enabled ? Cursors.Hand : Cursors.Default;
        if (!Enabled) _hovered = _pressed = false;
        Invalidate();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        _pressed = false;
        Invalidate();
    }

    protected override void OnChangeUICues(UICuesEventArgs e)
    {
        base.OnChangeUICues(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        int S(int value) => StudentLayout.Scale(this, value);
        if (Width <= S(StudentLayout.Space4) || Height <= S(StudentLayout.Space4)) return;

        var clearColor = Parent?.BackColor is { A: > 0 } parentBack ? parentBack : UiStyle.Surface;
        e.Graphics.Clear(clearColor);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Color backColor, borderColor, foreColor;
        if (!Enabled)
        {
            backColor = _kind switch
            {
                StudentButtonKind.Primary => UiStyle.Border,
                StudentButtonKind.Ghost => clearColor,
                _ => UiStyle.Background
            };
            borderColor = _kind is StudentButtonKind.Primary or StudentButtonKind.Ghost
                ? Color.Empty : UiStyle.Border;
            foreColor = UiStyle.TextMuted;
        }
        else
        {
            switch (_kind)
            {
                case StudentButtonKind.Primary:
                    backColor = _pressed ? StudentLayout.PrimaryPressed
                        : _hovered ? StudentLayout.PrimaryHover : UiStyle.Primary;
                    borderColor = Color.Empty;
                    foreColor = Color.White;
                    break;
                case StudentButtonKind.Ghost:
                    backColor = _pressed ? StudentLayout.ButtonPressedBack
                        : _hovered ? UiStyle.Background : clearColor;
                    borderColor = Color.Empty;
                    foreColor = _pressed || _hovered ? UiStyle.Text : UiStyle.TextMuted;
                    break;
                case StudentButtonKind.FlagToggle when _toggled:
                    backColor = _pressed ? StudentLayout.WarningPressedBack
                        : _hovered ? StudentLayout.WarningHoverBack : UiStyle.WarningBack;
                    borderColor = StudentLayout.WarningBorder;
                    foreColor = UiStyle.WarningText;
                    break;
                default:
                    backColor = _pressed ? StudentLayout.ButtonPressedBack
                        : _hovered ? UiStyle.Background : UiStyle.Surface;
                    borderColor = _pressed || _hovered ? StudentLayout.OptionHoverBorder : UiStyle.Border;
                    foreColor = UiStyle.Text;
                    break;
            }
        }

        var bounds = Rectangle.Inflate(ClientRectangle, -S(StudentLayout.Stroke1), -S(StudentLayout.Stroke1));
        using var path = StudentLayout.RoundedRectangle(this, bounds, StudentLayout.ButtonRadius);
        using var fill = new SolidBrush(backColor);
        e.Graphics.FillPath(fill, path);

        if (borderColor != Color.Empty)
        {
            using var border = new Pen(borderColor, S(StudentLayout.Stroke1));
            e.Graphics.DrawPath(border, path);
        }

        if (Enabled && Focused && ShowFocusCues)
        {
            if (_kind == StudentButtonKind.Primary)
            {
                using var focusPath = StudentLayout.RoundedRectangle(this,
                    Rectangle.Inflate(bounds, -S(StudentLayout.Stroke2), -S(StudentLayout.Stroke2)),
                    StudentLayout.SmallRadius);
                using var focusPen = new Pen(Color.White, S(StudentLayout.Stroke1));
                e.Graphics.DrawPath(focusPen, focusPath);
            }
            else
            {
                using var focusPen = new Pen(UiStyle.Primary, S(StudentLayout.Stroke2));
                e.Graphics.DrawPath(focusPen, path);
            }
        }

        var prefixFlag = ShowKeyboardCues ? TextFormatFlags.Default : TextFormatFlags.HidePrefix;
        int textWidth = TextRenderer.MeasureText(e.Graphics, Text, Font, new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix | TextFormatFlags.NoPadding).Width;

        if (_arrow != StudentArrow.None)
        {
            int arrowWidth = S(StudentLayout.Space8);
            int gap = S(StudentLayout.Space8);
            int totalWidth = arrowWidth + gap + textWidth;
            int startX = Math.Max(Padding.Left, (Width - totalWidth) / 2);
            if (_arrow == StudentArrow.Left)
            {
                StudentLayout.DrawArrow(e.Graphics, this,
                    new Point(startX + arrowWidth / 2, Height / 2), right: false, foreColor);
                var textBounds = new Rectangle(startX + arrowWidth + gap, 0,
                    Math.Max(1, Width - (startX + arrowWidth + gap) - S(StudentLayout.Space4)), Height);
                TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, foreColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
                    | TextFormatFlags.NoPadding | prefixFlag);
            }
            else
            {
                var textBounds = new Rectangle(startX, 0, textWidth + S(StudentLayout.Space4), Height);
                TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, foreColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
                    | TextFormatFlags.NoPadding | prefixFlag);
                StudentLayout.DrawArrow(e.Graphics, this,
                    new Point(startX + textWidth + gap + arrowWidth / 2, Height / 2), right: true, foreColor);
            }
        }
        else
        {
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine | prefixFlag);
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new StudentButtonAccessibleObject(this);

    private sealed class StudentButtonAccessibleObject(StudentButton owner) : ButtonBaseAccessibleObject(owner)
    {
        public override AccessibleStates State =>
            base.State | (owner._kind == StudentButtonKind.FlagToggle && owner._toggled
                ? AccessibleStates.Checked | AccessibleStates.Pressed
                : AccessibleStates.None);
    }
}
