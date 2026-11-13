using System.ComponentModel;
using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

internal sealed class OptionRow : Panel
{
    private const TextFormatFlags AnswerTextFlags =
        TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix;
    private readonly string _letter;
    private bool _hovered;
    private bool _checked;

    internal OptionRow(char letter)
    {
        _letter = letter.ToString();
        SetStyle(ControlStyles.Selectable | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        TabStop = true;
        AccessibleRole = AccessibleRole.RadioButton;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Surface;
        Cursor = Cursors.Hand;
        Margin = new Padding(0, 0, 0, ScalePixels(StudentLayout.Space8));
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value) return;
            _checked = value;
            Invalidate();
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        }
    }

    internal int MeasureHeight(int width)
    {
        int textWidth = Math.Max(1, width - ScalePixels(StudentLayout.OptionTextReservedWidth));
        int textHeight = TextRenderer.MeasureText(Text, Font, new Size(textWidth, int.MaxValue),
            AnswerTextFlags).Height;
        // Chừa 4 px mỗi phía cho focus ring, phần thẻ bên trong cao ít nhất 52 px.
        return Math.Max(ScalePixels(StudentLayout.OptionMinHeight),
            textHeight + ScalePixels(StudentLayout.OptionVerticalPadding));
    }

    protected override bool IsInputKey(Keys keyData) =>
        keyData == Keys.Space || base.IsInputKey(keyData);

    protected override void OnMouseDown(MouseEventArgs e)
    {
        // Focus bằng chuột trước khi xử lý chọn đáp án qua Click.
        if (e.Button == MouseButtons.Left) Focus();
        base.OnMouseDown(e);
    }

    protected override void OnClick(EventArgs e)
    {
        if (Enabled && (e is not MouseEventArgs mouse || mouse.Button == MouseButtons.Left)) base.OnClick(e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _hovered = Enabled;
        Invalidate();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Cursor = Enabled ? Cursors.Hand : Cursors.Default;
        if (!Enabled) _hovered = false;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = false;
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
        Invalidate();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        AccessibleName = $"Đáp án {_letter}. {Text}";
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (ClientSize.Width <= ScalePixels(StudentLayout.Space12)
            || ClientSize.Height <= ScalePixels(StudentLayout.Space12)) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? UiStyle.Surface);
        var bounds = Rectangle.Inflate(ClientRectangle,
            -ScalePixels(StudentLayout.OptionFocusInset),
            -ScalePixels(StudentLayout.OptionFocusInset));
        using var outline = StudentLayout.RoundedRectangle(this, bounds, StudentLayout.CardRadius);
        using var fill = new SolidBrush(Checked ? StudentLayout.SelectionBack
            : Enabled && _hovered ? UiStyle.Background : UiStyle.Surface);
        e.Graphics.FillPath(fill, outline);
        using var border = new Pen(Checked ? UiStyle.Primary
            : Enabled && _hovered ? StudentLayout.OptionHoverBorder : UiStyle.Border,
            ScalePixels(Checked ? StudentLayout.Stroke2 : StudentLayout.Stroke1));
        e.Graphics.DrawPath(border, outline);

        int inset = ScalePixels(StudentLayout.OptionBadgeLeft);
        int tileSize = ScalePixels(StudentLayout.OptionBadgeSize);
        var tile = new Rectangle(inset, (Height - tileSize) / 2, tileSize, tileSize);
        StudentLayout.DrawDot(e.Graphics, tile, Checked ? UiStyle.Primary : UiStyle.Background);
        TextRenderer.DrawText(e.Graphics, _letter, Font, tile, Checked ? UiStyle.Surface : UiStyle.TextMuted,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

        int textLeft = ScalePixels(StudentLayout.OptionTextLeft);
        // Chừa riêng chỗ cho dấu chọn để không đè lên đáp án dài khi xem lại.
        int textWidth = Math.Max(1, Width - ScalePixels(StudentLayout.OptionTextReservedWidth));
        int textHeight = TextRenderer.MeasureText(Text, Font, new Size(textWidth, int.MaxValue), AnswerTextFlags).Height;
        int verticalInset = ScalePixels(StudentLayout.Space16);
        var textBounds = new Rectangle(textLeft, Math.Max(verticalInset, (Height - textHeight) / 2),
            textWidth, Math.Max(1, Height - ScalePixels(StudentLayout.OptionVerticalPadding)));
        TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, UiStyle.Text,
            AnswerTextFlags);

        if (Checked)
        {
            StudentLayout.DrawCheck(e.Graphics, this,
                new Point(Width - ScalePixels(StudentLayout.OptionCheckRightInset), Height / 2), UiStyle.Primary);
        }

        if (Enabled && Focused)
        {
            // Viền ngoài độc lập, cách viền chọn 2 px; không thay màu để giả focus.
            using var focusPath = StudentLayout.RoundedRectangle(this,
                Rectangle.Inflate(ClientRectangle,
                    -ScalePixels(StudentLayout.Stroke1),
                    -ScalePixels(StudentLayout.Stroke1)),
                StudentLayout.FocusRadius);
            using var focusPen = new Pen(UiStyle.Primary, ScalePixels(StudentLayout.Stroke2));
            e.Graphics.DrawPath(focusPen, focusPath);
        }
    }

    private int ScalePixels(int pixels) => StudentLayout.Scale(this, pixels);

    protected override AccessibleObject CreateAccessibilityInstance() => new OptionAccessibleObject(this);

    private sealed class OptionAccessibleObject(OptionRow owner) : ControlAccessibleObject(owner)
    {
        public override AccessibleStates State =>
            base.State | (owner.Checked ? AccessibleStates.Checked : AccessibleStates.None);

        public override string DefaultAction => "Chọn đáp án";

        public override void DoDefaultAction()
        {
            if (!owner.Enabled) return;
            owner.Focus();
            owner.OnClick(EventArgs.Empty);
        }
    }
}
