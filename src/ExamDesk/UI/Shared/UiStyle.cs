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
    }

    public static void StyleGrid(DataGridView grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Border;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.EnableHeadersVisualStyles = false;

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
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E3EEFF");
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.RowTemplate.Height = 36;
    }
}
