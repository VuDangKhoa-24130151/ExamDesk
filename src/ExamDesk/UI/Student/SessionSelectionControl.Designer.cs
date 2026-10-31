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

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10F);
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Dock = DockStyle.Fill;

        // Chia màn hình thành 4 hàng: tiêu đề, thông tin sinh viên, ca thi và nút quay lại.
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 4,
            Padding = new Padding(24), TabIndex = 0, TabStop = false
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // Các hàng tự tăng chiều cao khi chữ xuống dòng; màn hình thiếu chỗ sẽ có thanh cuộn.
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var heading = new FlowLayoutPanel
        {
            AutoSize = true, Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Margin = new Padding(0, 0, 0, 12)
        };
        heading.Controls.Add(TextLabel("Chọn ca thi", 18, FontStyle.Bold));
        heading.Controls.Add(TextLabel("Chỉ hiển thị các ca thi có tên bạn trong danh sách.", 10));
        page.Controls.Add(heading, 0, 0);

        // Tạm ghi sẵn thông tin, sau này lấy từ màn nhận diện sinh viên.
        var identity = Surface();
        identity.Margin = new Padding(0, 0, 0, 12);
        identity.Controls.Add(TextLabel("Vũ Đăng Khoa · 24130151", 12, FontStyle.Bold));
        identity.Controls.Add(TextLabel("Lớp DH23DT • Chọn ca thi trong danh sách được phân công", 10));
        page.Controls.Add(identity, 0, 1);

        var sessions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            MinimumSize = new Size(0, 0), Margin = Padding.Empty, AutoSize = true, TabIndex = 0, TabStop = false
        };
        // Ca đang mở rộng hơn để có chỗ cho thông tin và nút vào thi.
        sessions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        sessions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        sessions.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var open = Surface();
        open.Dock = DockStyle.Fill;
        open.Margin = new Padding(0, 0, 12, 16);
        open.Controls.Add(TextLabel("Cuối kỳ .NET – Ca 01", 12, FontStyle.Bold));
        open.Controls.Add(TextLabel("Lập trình .NET • 30 câu • 45 phút", 10));
        open.Controls.Add(Badge("● Đang mở", Color.FromArgb(234, 246, 239), Color.FromArgb(20, 108, 67)));
        open.Controls.Add(TextLabel("Khung giờ được bắt đầu: 30/10/2026, 08:00–08:30", 10));
        open.Controls.Add(TextLabel("Lượt thi: Chưa bắt đầu", 10));
        var enter = new Button
        {
            Text = "&Xem hướng dẫn →", AutoSize = true,
            MinimumSize = new Size(220, 40), BackColor = UiStyle.Primary,
            ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 8, 0, 0), Padding = new Padding(12, 4, 12, 4),
            Cursor = Cursors.Hand, TabIndex = 0, AccessibleName = "Xem hướng dẫn ca thi 01"
        };
        enter.FlatAppearance.BorderSize = 0;
        enter.Click += OpenInstructions;
        open.Controls.Add(enter);
        sessions.Controls.Add(open, 0, 0);

        var upcoming = Surface();
        upcoming.Dock = DockStyle.Fill;
        upcoming.Margin = new Padding(8, 0, 0, 16);
        upcoming.Controls.Add(TextLabel("Cuối kỳ .NET – Ca 02", 12, FontStyle.Bold));
        upcoming.Controls.Add(TextLabel("Lập trình .NET • 30 câu • 45 phút", 10));
        upcoming.Controls.Add(Badge("◷ Chưa đến giờ", Color.FromArgb(255, 245, 219), Color.FromArgb(132, 83, 0)));
        upcoming.Controls.Add(TextLabel("Được bắt đầu lúc 13:30 ngày 30/10/2026.\nBạn chưa thể vào ca thi này.", 10));
        sessions.Controls.Add(upcoming, 1, 0);
        page.Controls.Add(sessions, 0, 2);

        var back = new Button
        {
            Text = "← &Nhập lại mã sinh viên", AutoSize = true, TabIndex = 1,
            MinimumSize = new Size(224, 40), BackColor = UiStyle.Surface,
            ForeColor = UiStyle.Text, FlatStyle = FlatStyle.Flat,
            Padding = new Padding(12, 4, 12, 4), Margin = new Padding(0, 12, 0, 0)
        };
        back.FlatAppearance.BorderColor = Color.FromArgb(220, 227, 237);
        back.Click += ChangeIdentity;
        page.Controls.Add(back, 0, 3);
        // Cho chữ tự xuống dòng theo chiều rộng từng khung khi phóng to hoặc thu nhỏ.
        heading.SizeChanged += (_, _) => WrapLabels(heading);
        identity.SizeChanged += (_, _) => WrapLabels(identity);
        open.SizeChanged += (_, _) => WrapLabels(open);
        upcoming.SizeChanged += (_, _) => WrapLabels(upcoming);
        AutoScroll = true;
        Controls.Add(page);
        ResumeLayout(false);
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
    private static Label TextLabel(string text, float size, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, Font = new Font("Segoe UI", size, style),
        Margin = new Padding(0, 0, 0, 8)
    };

    // Nhãn trạng thái có màu nền riêng, ví dụ đang mở hoặc chưa đến giờ.
    private static Label Badge(string text, Color background, Color foreground) => new()
    {
        Text = text, AutoSize = true, BackColor = background, ForeColor = foreground,
        Padding = new Padding(10, 6, 10, 6), Margin = new Padding(0, 8, 0, 16)
    };
}





