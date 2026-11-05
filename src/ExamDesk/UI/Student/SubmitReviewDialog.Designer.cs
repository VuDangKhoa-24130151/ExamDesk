using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SubmitReviewDialog
{
    private readonly System.Windows.Forms.Timer _clock = new() { Interval = 250 };
    private Label _answered = null!, _unanswered = null!, _warning = null!;
    private FlowLayoutPanel _numbers = null!;
    private Button _continueButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) _clock.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        Text = "Kiểm tra trước khi nộp";
        Font = new Font("Segoe UI", 10);
        AutoScaleMode = AutoScaleMode.Font;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(680, 500);
        MinimumSize = new Size(540, 360);
        BackColor = UiStyle.Surface;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var body = new Panel { Dock = DockStyle.Fill, AutoScroll = true, TabIndex = 0 };
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(24, 16, 24, 0)
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        content.Controls.Add(StudentLayout.Label("Bạn muốn nộp bài?", 18, FontStyle.Bold));
        content.Controls.Add(StudentLayout.Label("Bạn vẫn có thể nộp bài khi còn câu bỏ trống."));

        var counts = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 8, 0, 12)
        };
        for (int i = 0; i < 2; i++)
            counts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        counts.Controls.Add(Counter("Đã trả lời", UiStyle.Background, UiStyle.Primary, out _answered), 0, 0);
        counts.Controls.Add(Counter("Chưa trả lời", Color.FromArgb(255, 245, 219), Color.FromArgb(132, 83, 0), out _unanswered), 1, 0);
        content.Controls.Add(counts);

        content.Controls.Add(StudentLayout.Label("Chọn số câu để quay lại — ô xanh đã trả lời, ô trắng chưa trả lời:"));
        _numbers = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 138, AutoScroll = true, Margin = Padding.Empty };
        content.Controls.Add(_numbers);
        _warning = StudentLayout.Label("");
        _warning.ForeColor = Color.FromArgb(180, 35, 24);
        content.Controls.Add(_warning);
        body.Controls.Add(content);

        // Giữ nút thao tác ở đáy; phần nội dung có thể cuộn khi thu nhỏ.
        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom, AutoSize = true, ColumnCount = 2,
            Padding = new Padding(24, 0, 24, 16), TabIndex = 1
        };
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _continueButton = StudentLayout.Button("Tiếp tục &làm bài", (_, _) => { });
        _continueButton.DialogResult = DialogResult.Cancel;
        _continueButton.Dock = DockStyle.Fill;
        var confirm = StudentLayout.Button("&Xác nhận nộp bài", (_, _) => { }, true);
        confirm.DialogResult = DialogResult.OK;
        confirm.Dock = DockStyle.Fill;
        actions.Controls.Add(_continueButton, 0, 0);
        actions.Controls.Add(confirm, 1, 0);
        Controls.Add(body);
        Controls.Add(actions);
        CancelButton = _continueButton;
        ResumeLayout(true);
    }

    private static Control Counter(string title, Color background, Color color, out Label value)
    {
        var card = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, BackColor = background, Padding = new Padding(12),
            Margin = new Padding(0, 0, 8, 0)
        };
        var caption = StudentLayout.Label(title, 9);
        caption.Margin = Padding.Empty;
        value = StudentLayout.Label("0", 18, FontStyle.Bold);
        value.Margin = new Padding(0, 4, 0, 0);
        value.ForeColor = color;
        card.Controls.Add(caption);
        card.Controls.Add(value);
        return card;
    }
}
