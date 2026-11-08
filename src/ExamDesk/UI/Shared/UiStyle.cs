namespace ExamDesk.UI.Shared;

// Bạn 1 điều phối. Các phân hệ dùng cùng màu; không thêm thư viện UI bên ngoài.
public static class UiStyle
{
    public static readonly Color Background = ColorTranslator.FromHtml("#F3F6FA");
    public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
    public static readonly Color Text = ColorTranslator.FromHtml("#172B4D");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#53647D");
    public static readonly Color Primary = ColorTranslator.FromHtml("#185ABD");
    public static readonly Color Sidebar = ColorTranslator.FromHtml("#142D50");
    public static readonly Color Border = ColorTranslator.FromHtml("#DCE3ED");

    public static readonly Color SuccessText = ColorTranslator.FromHtml("#146C43");
    public static readonly Color SuccessBack = ColorTranslator.FromHtml("#EAF6EF");
    public static readonly Color WarningText = ColorTranslator.FromHtml("#845300");
    public static readonly Color WarningBack = ColorTranslator.FromHtml("#FFF5DB");
    public static readonly Color ErrorText = ColorTranslator.FromHtml("#B42318");
    public static readonly Color ErrorBack = ColorTranslator.FromHtml("#FFF0EE");

    public static readonly Font BodyFont = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font SmallFont = new("Segoe UI", 9F, FontStyle.Regular);
    public static readonly Font TitleFont = new("Segoe UI", 16F, FontStyle.Bold);

    public static void StylePrimaryButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Primary;
        button.ForeColor = Color.White;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        if (button.Height < 38)
        {
            button.Height = 38;
        }

        ApplyResponsiveButtonSizing(button);
    }

    public static void StyleSecondaryButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.BackColor = Surface;
        button.ForeColor = Text;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        if (button.Height < 38)
        {
            button.Height = 38;
        }

        ApplyResponsiveButtonSizing(button);
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

        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Border;
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

        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Background;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Background;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E3EEFF");
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.RowTemplate.Height = 36;
    }
}
