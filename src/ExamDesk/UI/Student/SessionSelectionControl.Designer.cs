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
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Dock = DockStyle.Fill;
        AutoScroll = true;
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
        Text = text, AutoSize = true, BackColor = background, ForeColor = foreground,
        Padding = new Padding(10, 6, 10, 6), Margin = new Padding(0, 8, 0, 16)
    };
}





