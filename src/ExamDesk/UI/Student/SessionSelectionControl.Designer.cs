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

    private static TableLayoutPanel SessionList()
    {
        var list = new BufferedStudentTable
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1,
            Margin = new Padding(0, StudentLayout.Space12, 0, StudentLayout.Space12)
        };
        list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return list;
    }

    // Tạo khung trắng dùng chung; các nội dung bên trong xếp từ trên xuống.
    private static FlowLayoutPanel Surface() => new()
    {
        AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Top, BackColor = UiStyle.Surface,
        Padding = new Padding(StudentLayout.Space16), FlowDirection = FlowDirection.TopDown,
        WrapContents = false, BorderStyle = BorderStyle.FixedSingle, TabStop = false
    };

    // Tạo nhãn theo cùng font và khoảng cách để không phải viết lặp lại.
    private static Label TextLabel(string text, float size = StudentLayout.BodyFontSize, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, UseMnemonic = false, Font = StudentLayout.GetLabelFont(size, style),
        Margin = new Padding(0, 0, 0, StudentLayout.Space8)
    };

    // Màu biểu thị trạng thái ca, không phụ thuộc khả năng bấm nút.
    private static Label Badge(string text) => new()
    {
        Text = text, AutoSize = true,
        BackColor = text == "Đang mở" ? UiStyle.SuccessBack
            : text == "Ca đã đóng" ? UiStyle.WarningBack : UiStyle.Background,
        ForeColor = text == "Đang mở" ? UiStyle.SuccessText
            : text == "Ca đã đóng" ? UiStyle.WarningText : UiStyle.TextMuted,
        Font = UiStyle.SmallFont,
        Anchor = AnchorStyles.None,
        Padding = new Padding(StudentLayout.Space8, StudentLayout.Space4, StudentLayout.Space8, StudentLayout.Space4),
        Margin = new Padding(0, 0, StudentLayout.Space12, 0)
    };
}





