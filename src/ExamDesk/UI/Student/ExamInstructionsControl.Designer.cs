#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class ExamInstructionsControl
{
    private CheckBox _readCheckBox = null!;
    private Button _startButton = null!;
    private Label _sessionTitle = null!, _studentInfo = null!, _sessionInfo = null!;
    private Label _paperInfo = null!, _durationRule = null!, _resumeInfo = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.SmallFont;
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
        _sessionTitle = TextLabel("Thông tin ca thi", 12, FontStyle.Bold);
        instructions.Controls.Add(_sessionTitle);
        instructions.Controls.Add(TextLabel("Đọc kỹ trước khi bắt đầu lượt thi", 9));
        _durationRule = TextLabel("");
        instructions.Controls.Add(_durationRule);
        string[] rules =
        {
            "02  Mỗi câu có bốn đáp án A–D. Chọn một đáp án, có thể đổi hoặc bỏ lựa chọn.",
            "03  Chọn đáp án bằng A/B/C/D hoặc 1–4. Chuyển câu bằng N/P hoặc PageDown/PageUp.",
            "04  Có thể đánh dấu để quay lại xem sau.",
            "05  Tiến độ được lưu khi thay đổi. Đóng cửa sổ không dừng đồng hồ.",
            "06  Hết giờ, hệ thống tự động nộp bài. Nếu có lỗi, bạn có thể thử nộp bài lại.",
            "07  Sau khi nộp bài, bạn không thể thay đổi đáp án."
        };
        foreach (string rule in rules)
            instructions.Controls.Add(TextLabel(rule));
        details.Controls.Add(instructions, 0, 0);

        var attempt = Surface();
        attempt.Margin = new Padding(8, 0, 0, 0);
        attempt.Controls.Add(TextLabel("Thông tin lượt thi", 12, FontStyle.Bold));
        _studentInfo = TextLabel("");
        _sessionInfo = TextLabel("");
        _paperInfo = TextLabel("");
        attempt.Controls.Add(_studentInfo);
        attempt.Controls.Add(_sessionInfo);
        attempt.Controls.Add(_paperInfo);
        _resumeInfo = TextLabel("");
        _resumeInfo.Visible = false;
        attempt.Controls.Add(_resumeInfo);
        details.Controls.Add(attempt, 1, 0);
        page.Controls.Add(details, 0, 1);

        _readCheckBox = new CheckBox
        {
            Text = "Tôi đã đọc và &hiểu hướng dẫn làm bài.", AutoSize = true,
            Margin = new Padding(0, 12, 0, 8), TabIndex = 0
        };
        page.Controls.Add(_readCheckBox, 0, 2);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, WrapContents = true,
            Margin = Padding.Empty, TabIndex = 1, TabStop = false
        };


        var back = new Button
        {
            Text = "← &Quay lại chọn ca", AutoSize = true, BackColor = UiStyle.Surface, ForeColor = UiStyle.Text,
            FlatStyle = FlatStyle.Flat, TabIndex = 0, Margin = new Padding(0, 0, 12, 8)
        };
        UiStyle.StyleSecondaryButton(back);
        back.Click += GoBack;
        _startButton = new Button
        {
            Text = "&Bắt đầu làm bài", AutoSize = true, Anchor = AnchorStyles.Right, BackColor = UiStyle.Primary, ForeColor = UiStyle.Surface,
            FlatStyle = FlatStyle.Flat, Enabled = false, TabIndex = 1, Margin = new Padding(0, 0, 0, 8)
        };
        UiStyle.StylePrimaryButton(_startButton);
        // Nút bật sau khi sinh viên xác nhận đã đọc hướng dẫn.
        actions.Controls.Add(back);
        actions.Controls.Add(_startButton);
        page.Controls.Add(actions, 0, 3);

        // Khi phần nội dung hẹp, xếp hai thẻ theo chiều dọc để chữ không bị bó hẹp.
        void FitDetails()
        {
            bool stacked=details.ClientSize.Width < (int)(820*DeviceDpi/96f);
            int columns=stacked ? 1 : 2;
            if(details.ColumnCount==columns) return;
            details.SuspendLayout();
            details.ColumnCount=columns; details.RowCount=stacked ? 2 : 1;
            details.ColumnStyles.Clear(); details.RowStyles.Clear();
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,stacked ? 100 : 60));
            if(!stacked) details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,40));
            details.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            if(stacked) details.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            details.SetCellPosition(instructions,new TableLayoutPanelCellPosition(0,0));
            details.SetCellPosition(attempt,new TableLayoutPanelCellPosition(stacked ? 0 : 1,stacked ? 1 : 0));
            instructions.Margin=new Padding(0,0,stacked ? 0 : 12,stacked ? 12 : 0);
            attempt.Margin=new Padding(stacked ? 0 : 8,0,0,0);
            details.ResumeLayout(true);
        }
        details.SizeChanged+=(_,_)=>FitDetails();
        Load+=(_,_)=>FitDetails();
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
        Text = text, AutoSize = true, UseMnemonic = false, Font = StudentLayout.GetLabelFont(size, style),
        Margin = new Padding(0, 0, 0, 8)
    };
}
