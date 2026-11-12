using System.ComponentModel;
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
        Margin = new Padding(0, 0, 0, 8);
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
        int textWidth = Math.Max(1, width - ScalePixels(80));
        int textHeight = TextRenderer.MeasureText(Text, Font, new Size(textWidth, int.MaxValue),
            AnswerTextFlags).Height;
        return Math.Max(ScalePixels(40), textHeight + ScalePixels(16));
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
        if (e is not MouseEventArgs mouse || mouse.Button == MouseButtons.Left) base.OnClick(e);
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
        if (ClientSize.Width <= 1 || ClientSize.Height <= 1) return;

        if (Checked || (Enabled && _hovered))
        {
            using var fill = new SolidBrush(UiStyle.Background);
            e.Graphics.FillRectangle(fill, ClientRectangle);
        }

        using var border = new Pen(Checked ? UiStyle.Primary : UiStyle.Border);
        e.Graphics.DrawRectangle(border, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

        int inset = ScalePixels(8);
        int tileSize = ScalePixels(24);
        var tile = new Rectangle(inset, inset, tileSize, tileSize);
        using var tileFill = new SolidBrush(Checked ? UiStyle.Primary : UiStyle.Background);
        e.Graphics.FillRectangle(tileFill, tile);
        TextRenderer.DrawText(e.Graphics, _letter, Font, tile, Checked ? UiStyle.Surface : UiStyle.TextMuted,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

        int textLeft = ScalePixels(48);
        // Chừa riêng chỗ cho dấu chọn để không đè lên đáp án dài khi xem lại.
        var textBounds = new Rectangle(textLeft, inset, Math.Max(1, Width - ScalePixels(80)),
            Math.Max(1, Height - inset * 2));
        TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, Enabled ? UiStyle.Text : UiStyle.TextMuted,
            AnswerTextFlags);

        if (Checked)
        {
            var checkBounds = new Rectangle(Width - inset - tileSize, inset, tileSize, tileSize);
            TextRenderer.DrawText(e.Graphics, "✓", Font, checkBounds, UiStyle.Primary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        if (Focused && ShowFocusCues)
        {
            var focusBounds = Rectangle.Inflate(ClientRectangle, -2, -2);
            if (focusBounds.Width > 0 && focusBounds.Height > 0)
                ControlPaint.DrawFocusRectangle(e.Graphics, focusBounds);
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
