using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SubmitReviewDialog
{
    private readonly System.Windows.Forms.Timer _clock = new() { Interval = 250 };
    private Label _answered = null!, _unanswered = null!, _flagged = null!, _warning = null!;
    private FlowLayoutPanel _numbers = null!;
    private Button _continueButton = null!, _confirmButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) _clock.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        Text = "Kiểm tra trước khi nộp bài";
        Font = UiStyle.BodyFont;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(680, 530);
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
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, Margin = new Padding(0, 8, 0, 12)
        };
        for (int i = 0; i < 3; i++)
            counts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
        counts.Controls.Add(Counter("Đã trả lời", UiStyle.Background, UiStyle.Primary, out _answered), 0, 0);
        counts.Controls.Add(Counter("Chưa trả lời", UiStyle.WarningBack, UiStyle.WarningText, out _unanswered), 1, 0);
        counts.Controls.Add(Counter("Đánh dấu", UiStyle.Background, UiStyle.Primary, out _flagged), 2, 0);
        content.Controls.Add(counts);

        content.Controls.Add(StudentLayout.Label("Câu cần xem lại — chọn câu để quay lại", 10, FontStyle.Bold));
        _numbers = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Margin = Padding.Empty
        };
        content.Controls.Add(_numbers);
        _warning = StudentLayout.Label("");
        _warning.ForeColor = UiStyle.ErrorText;
        body.Controls.Add(content);
        content.SizeChanged+=(_,_)=>
        {
            int width=Math.Max(1,content.ClientSize.Width-content.Padding.Horizontal-12);
            foreach(Control control in content.Controls)
                if(control is Label label) label.MaximumSize=new Size(width,0);
        };

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
        _confirmButton = StudentLayout.Button("&Xác nhận nộp bài", primary: true);
        _confirmButton.DialogResult = DialogResult.OK;
        _confirmButton.Dock = DockStyle.Fill;
        actions.Controls.Add(_warning, 0, 0);
        actions.SetColumnSpan(_warning, 2);
        actions.Controls.Add(_continueButton, 0, 1);
        actions.Controls.Add(_confirmButton, 1, 1);
        actions.SizeChanged += (_, _) =>
            _warning.MaximumSize = new Size(Math.Max(1, actions.ClientSize.Width - actions.Padding.Horizontal), 0);
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
