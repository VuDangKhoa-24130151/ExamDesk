using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ExamDesk.UI.Shared;

// Bạn 1 điều phối. Các phân hệ dùng cùng màu; không thêm thư viện UI bên ngoài.
public static class UiStyle
{
    private sealed class RoundedStyleState
    {
        public int Radius { get; set; }
        public Color? BorderColor { get; set; }
    }

    private static readonly ConditionalWeakTable<Control, RoundedStyleState> RoundedStates = new();

    public static readonly Color Background = ColorTranslator.FromHtml("#F3F6FA");
    public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
    public static readonly Color Text = ColorTranslator.FromHtml("#172B4D");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#53647D");
    public static readonly Color Primary = ColorTranslator.FromHtml("#185ABD");
    public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#144CA0");
    public static readonly Color PrimaryPressed = ColorTranslator.FromHtml("#0F3D82");
    public static readonly Color SecondaryHover = ColorTranslator.FromHtml("#EEF3FA");
    public static readonly Color SecondaryPressed = ColorTranslator.FromHtml("#E1EAF6");
    public static readonly Color CardHoverBack = ColorTranslator.FromHtml("#F7FAFF");
    public static readonly Color Sidebar = ColorTranslator.FromHtml("#142D50");
    public static readonly Color Border = ColorTranslator.FromHtml("#DCE3ED");
    public static readonly Color GridLine = ColorTranslator.FromHtml("#E6ECF4");
    public static readonly Color RowAltBack = ColorTranslator.FromHtml("#F9FBFD");
    public static readonly Color RowSelectionBack = ColorTranslator.FromHtml("#E8F1FD");

    public static readonly Color SuccessText = ColorTranslator.FromHtml("#146C43");
    public static readonly Color SuccessBack = ColorTranslator.FromHtml("#EAF6EF");
    public static readonly Color WarningText = ColorTranslator.FromHtml("#845300");
    public static readonly Color WarningBack = ColorTranslator.FromHtml("#FFF5DB");
    public static readonly Color ErrorText = ColorTranslator.FromHtml("#B42318");
    public static readonly Color ErrorBack = ColorTranslator.FromHtml("#FFF0EE");
    public static readonly Color DangerBorder = ColorTranslator.FromHtml("#F1B8B3");
    public static readonly Color DangerPressed = ColorTranslator.FromHtml("#FDE0DD");

    public static readonly Font BodyFont = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font BodyBoldFont = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font SmallFont = new("Segoe UI", 9F, FontStyle.Regular);
    public static readonly Font SmallBoldFont = new("Segoe UI", 9.5F, FontStyle.Bold);
    public static readonly Font SectionTitleFont = new("Segoe UI", 11F, FontStyle.Bold);
    public static readonly Font InputLargeFont = new("Segoe UI", 11F, FontStyle.Regular);
    public static readonly Font QuestionBodyFont = new("Segoe UI", 10.5F, FontStyle.Regular);
    public static readonly Font TitleFont = new("Segoe UI", 16F, FontStyle.Bold);
    public static readonly Font MetricValueFont = new("Segoe UI", 20F, FontStyle.Bold);

    public static void StylePrimaryButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = PrimaryHover;
        button.FlatAppearance.MouseDownBackColor = PrimaryPressed;
        button.BackColor = Primary;
        button.ForeColor = Color.White;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        if (button.Height < 38)
        {
            button.Height = 38;
        }

        ApplyResponsiveButtonSizing(button);
        ApplyRoundedCorners(button, 6);
    }

    public static void StyleSecondaryButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = SecondaryHover;
        button.FlatAppearance.MouseDownBackColor = SecondaryPressed;
        button.BackColor = Surface;
        button.ForeColor = Text;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        if (button.Height < 38)
        {
            button.Height = 38;
        }

        ApplyResponsiveButtonSizing(button);
        ApplyRoundedCorners(button, 6, Border);
    }

    public static void StyleDangerButton(Button button)
    {
        StyleSecondaryButton(button);
        button.ForeColor = ErrorText;
        button.FlatAppearance.MouseOverBackColor = ErrorBack;
        button.FlatAppearance.MouseDownBackColor = DangerPressed;
        ApplyRoundedCorners(button, 6, DangerBorder);
    }

    public static void StyleCompactSecondaryButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = SecondaryHover;
        button.FlatAppearance.MouseDownBackColor = SecondaryPressed;
        button.BackColor = Surface;
        button.ForeColor = Text;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        button.Height = 30;
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowOnly;
        button.Padding = new Padding(10, 0, 10, 0);
        ApplyRoundedCorners(button, 6, Border);
    }

    public static void StyleCard(Panel cardBorder, int radius = 10)
    {
        ArgumentNullException.ThrowIfNull(cardBorder);

        cardBorder.BackColor = Surface;
        ApplyRoundedCorners(cardBorder, radius, Border);
        if (cardBorder.Controls.Count > 0)
        {
            ApplyRoundedCorners(cardBorder.Controls[0], Math.Max(4, radius - 1));
        }
    }

    public static void StyleBadge(Control badge, int radius = 6)
    {
        ArgumentNullException.ThrowIfNull(badge);
        ApplyRoundedCorners(badge, radius);
    }

    public static void ApplyRoundedCorners(Control control, int radius = 8, Color? borderColor = null)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (RoundedStates.TryGetValue(control, out var existing))
        {
            existing.Radius = Math.Max(0, radius);
            existing.BorderColor = borderColor;
            UpdateRoundedRegion(control, existing);
            control.Invalidate();
            return;
        }

        var state = new RoundedStyleState
        {
            Radius = Math.Max(0, radius),
            BorderColor = borderColor
        };
        RoundedStates.Add(control, state);

        control.HandleCreated += (_, _) => UpdateRoundedRegion(control, state);
        control.SizeChanged += (_, _) =>
        {
            UpdateRoundedRegion(control, state);
            if (state.BorderColor.HasValue)
            {
                control.Invalidate();
            }
        };
        control.Paint += (_, e) =>
        {
            if (state.BorderColor is not { } border || control.Width < 2 || control.Height < 2)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = CreateRoundedRectPath(new Rectangle(0, 0, control.Width - 1, control.Height - 1), state.Radius);
            using var pen = new Pen(border, 1F);
            e.Graphics.DrawPath(pen, path);
        };

        if (control.IsHandleCreated)
        {
            UpdateRoundedRegion(control, state);
        }
    }

    public static GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return path;
        }

        var maxRadius = Math.Min(bounds.Width, bounds.Height) / 2;
        var r = Math.Clamp(radius, 0, maxRadius);
        if (r == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        var diameter = r * 2;
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void UpdateRoundedRegion(Control control, RoundedStyleState state)
    {
        if (control.IsDisposed || control.Width < 2 || control.Height < 2)
        {
            return;
        }

        using var path = CreateRoundedRectPath(new Rectangle(0, 0, control.Width, control.Height), state.Radius);
        var oldRegion = control.Region;
        control.Region = new Region(path);
        oldRegion?.Dispose();
    }

    private static void ApplyResponsiveButtonSizing(Button button)
    {
        if (button.Dock != DockStyle.None)
        {
            return;
        }

        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowOnly;
        if (button.Padding.Horizontal < 16)
        {
            button.Padding = new Padding(8, button.Padding.Top, 8, button.Padding.Bottom);
        }
    }

    public static void ClampToWorkingArea(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var screen = form.Owner is not null
            ? Screen.FromControl(form.Owner)
            : Screen.FromControl(form);
        var workingArea = screen.WorkingArea;

        var maxWidth = Math.Max(320, workingArea.Width - 16);
        var maxHeight = Math.Max(260, workingArea.Height - 16);

        if (form.MinimumSize.Width > maxWidth || form.MinimumSize.Height > maxHeight)
        {
            form.MinimumSize = new Size(
                Math.Min(form.MinimumSize.Width, maxWidth),
                Math.Min(form.MinimumSize.Height, maxHeight));
        }

        var targetWidth = Math.Min(form.Width, maxWidth);
        var targetHeight = Math.Min(form.Height, maxHeight);
        if (targetWidth != form.Width || targetHeight != form.Height)
        {
            form.Size = new Size(targetWidth, targetHeight);
        }

        var left = Math.Clamp(form.Left, workingArea.Left, Math.Max(workingArea.Left, workingArea.Right - form.Width));
        var top = Math.Clamp(form.Top, workingArea.Top, Math.Max(workingArea.Top, workingArea.Bottom - form.Height));
        if (left != form.Left || top != form.Top)
        {
            form.Location = new Point(left, top);
        }
    }

    public static void FitClientHeightWithinWorkingArea(Form form, int preferredClientHeight)
    {
        ArgumentNullException.ThrowIfNull(form);

        var screen = form.Owner is not null
            ? Screen.FromControl(form.Owner)
            : Screen.FromControl(form);
        var workingArea = screen.WorkingArea;

        var nonClientHeight = Math.Max(0, form.Height - form.ClientSize.Height);
        var maxClientHeight = Math.Max(220, workingArea.Height - nonClientHeight - 24);
        var targetClientHeight = Math.Min(preferredClientHeight, maxClientHeight);

        if (targetClientHeight > form.ClientSize.Height)
        {
            form.ClientSize = new Size(form.ClientSize.Width, targetClientHeight);
        }

        ClampToWorkingArea(form);
    }

    public static void StyleGrid(DataGridView grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        typeof(DataGridView)
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?
            .SetValue(grid, true);

        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = GridLine;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.EnableHeadersVisualStyles = false;
        grid.ScrollBars = ScrollBars.Both;

        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;

        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Background;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Font = BodyBoldFont;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Background;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        grid.DefaultCellStyle.SelectionBackColor = RowSelectionBack;
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.AlternatingRowsDefaultCellStyle.BackColor = RowAltBack;
        grid.RowTemplate.Height = Math.Max(36, BodyFont.Height + 16);
    }
}
