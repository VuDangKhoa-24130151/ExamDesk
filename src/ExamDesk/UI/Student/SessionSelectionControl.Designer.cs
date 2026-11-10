#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SessionSelectionControl
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeAppearance()
    {
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Dock = DockStyle.Fill;
        AutoScroll = true;
    }

    private static TableLayoutPanel SessionList()
    {
        var list = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = new Padding(0, 12, 0, 12)
        };
        list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return list;
    }

    // Tạo khung trắng dùng chung; các nội dung bên trong xếp từ trên xuống.
    private static FlowLayoutPanel Surface() => new()
    {
        AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Top, BackColor = UiStyle.Surface,
        Padding = new Padding(16), FlowDirection = FlowDirection.TopDown,
        WrapContents = false, BorderStyle = BorderStyle.FixedSingle, TabStop = false
    };

    // Tạo nhãn theo cùng font và khoảng cách để không phải viết lặp lại.
    private static Label TextLabel(string text, float size = 10, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, UseMnemonic = false, Font = StudentLayout.GetLabelFont(size, style),
        Margin = new Padding(0, 0, 0, 8)
    };

    // Nhãn trạng thái có màu nền riêng, ví dụ đang mở hoặc chưa đến giờ.
    private static Label Badge(string text, Color background, Color foreground) => new()
    {
        Text = text, AutoSize = true, BackColor = background, ForeColor = foreground, Font = UiStyle.SmallFont,
        Anchor = AnchorStyles.None, Padding = new Padding(8, 4, 8, 4), Margin = new Padding(0, 0, 12, 0)
    };
}





