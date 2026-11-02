#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class ExamInstructionsControl
{
    private CheckBox _readCheckBox = null!;
    private Button _startButton = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        AutoScroll = true;

        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 4,
            Padding = new Padding(24), TabStop = false
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 4; i++)
            page.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var heading = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Margin = new Padding(0, 0, 0, 12), TabStop = false
        };
        heading.Controls.Add(TextLabel("Hướng dẫn trước khi thi", 18, FontStyle.Bold));
        heading.Controls.Add(TextLabel("Xác nhận đúng thông tin sinh viên và nội dung ca thi."));
        page.Controls.Add(heading, 0, 0);

        // Hai cột tự tăng chiều cao theo nội dung, không kéo thẻ dài hết màn hình.
        var details = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1,
            Margin = Padding.Empty, TabStop = false
        };
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        details.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var instructions = Surface();
        instructions.Margin = new Padding(0, 0, 12, 0);
        instructions.Controls.Add(TextLabel("Cuối kỳ .NET – Ca 01", 12, FontStyle.Bold));
        instructions.Controls.Add(TextLabel("Đọc kỹ trước khi bắt đầu lượt thi", 9));
        string[] rules =
        {
            "01  Bài thi có 30 câu trắc nghiệm, thời gian 45 phút.",
            "02  Mỗi câu chọn một đáp án. Có thể đổi hoặc bỏ lựa chọn.",
            "03  Chuyển câu bằng nút hoặc lưới số câu bên phải.",
            "04  Có thể đánh dấu để quay lại xem sau.",
            "05  Lựa chọn chỉ giữ trong cửa sổ hiện tại, chưa lưu xuống máy.",
            "06  Hết giờ, vùng làm bài sẽ khóa và bài được tự động nộp.",
            "07  Sau khi nộp, bạn không thể thay đổi đáp án."
        };
        // Nội dung mô tả luồng thi dự kiến; bước này chưa gọi Service/Repository.
        foreach (string rule in rules)
            instructions.Controls.Add(TextLabel(rule));
        details.Controls.Add(instructions, 0, 0);

        var attempt = Surface();
        attempt.Margin = new Padding(8, 0, 0, 0);
        attempt.Controls.Add(TextLabel("Thông tin lượt thi", 12, FontStyle.Bold));
        // Thông tin tạm cùng với SV-01, sau này lấy từ sinh viên và ca đã chọn.
        attempt.Controls.Add(TextLabel("Vũ Đăng Khoa · 24130151"));
        attempt.Controls.Add(TextLabel("Lớp DH23DT"));
        attempt.Controls.Add(TextLabel("30 câu · 45 phút · Thang điểm 10"));
        attempt.Controls.Add(TextLabel("Điểm = số câu đúng / 30 × 10.\nLàm tròn hai chữ số thập phân; câu sai và bỏ trống không có điểm."));
        var notice = TextLabel("Chưa bắt đầu đếm giờ\nĐồng hồ chạy sau khi tạo lượt thi thành công.");
        notice.BackColor = Color.FromArgb(234, 242, 255);
        notice.ForeColor = UiStyle.Primary;
        notice.Padding = new Padding(12);
        attempt.Controls.Add(notice);
        details.Controls.Add(attempt, 1, 0);
        page.Controls.Add(details, 0, 1);

        _readCheckBox = new CheckBox
        {
            Text = "Tôi đã đọc và &hiểu hướng dẫn làm bài.", AutoSize = true,
            Margin = new Padding(0, 12, 0, 8), TabIndex = 0
        };
        page.Controls.Add(_readCheckBox, 0, 2);

        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1,
            Margin = Padding.Empty, TabIndex = 1, TabStop = false
        };
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        var back = new Button
        {
            Text = "← &Quay lại chọn ca", AutoSize = true, MinimumSize = new Size(180, 40),
            BackColor = UiStyle.Surface, ForeColor = UiStyle.Text,
            FlatStyle = FlatStyle.Flat, TabIndex = 1, Margin = Padding.Empty
        };
        back.FlatAppearance.BorderColor = Color.FromArgb(220, 227, 237);
        back.Click += GoBack;
        _startButton = new Button
        {
            Text = "&Bắt đầu làm bài →", AutoSize = true, MinimumSize = new Size(200, 40),
            Anchor = AnchorStyles.Right, BackColor = UiStyle.Primary, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Enabled = false, TabIndex = 0, Margin = Padding.Empty
        };
        _startButton.FlatAppearance.BorderSize = 0;
        // Nút bật sau khi sinh viên xác nhận đã đọc hướng dẫn.
        actions.Controls.Add(back, 0, 0);
        actions.Controls.Add(_startButton, 1, 0);
        page.Controls.Add(actions, 0, 3);

        heading.SizeChanged += (_, _) => WrapLabels(heading);
        instructions.SizeChanged += (_, _) => WrapLabels(instructions);
        attempt.SizeChanged += (_, _) => WrapLabels(attempt);
        Controls.Add(page);
        ResumeLayout(false);
        PerformLayout();
    }

    private static FlowLayoutPanel Surface() => new()
    {
        Dock = DockStyle.Fill, AutoSize = true, BackColor = UiStyle.Surface,
        Padding = new Padding(16), FlowDirection = FlowDirection.TopDown,
        WrapContents = false, BorderStyle = BorderStyle.FixedSingle, TabStop = false
    };

    private static Label TextLabel(string text, float size = 10, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, Font = new Font("Segoe UI", size, style),
        Margin = new Padding(0, 0, 0, 8)
    };
}
