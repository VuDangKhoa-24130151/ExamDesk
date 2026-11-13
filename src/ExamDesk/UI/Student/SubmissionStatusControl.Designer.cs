using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SubmissionStatusControl
{
    private Label _status = null!, _summary = null!, _lockStatus = null!;
    private FlowLayoutPanel _actions = null!;
    private Panel _cardHost = null!;
    private ProgressBar _busyProgress = null!;

    private void InitializeComponent()
    {
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        var heading = new BufferedStudentTable
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1,
            Padding = new Padding(StudentLayout.Space24, StudentLayout.Space16, StudentLayout.Space24, 0)
        };
        heading.Controls.Add(StudentLayout.Label("Trạng thái nộp bài", StudentLayout.PageTitleFontSize, FontStyle.Bold));
        _cardHost = new BufferedStudentPanel { Dock = DockStyle.Fill, AutoScroll = true };
        var card = new BufferedStudentTable
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1,
            BackColor = UiStyle.Surface, Padding = new Padding(StudentLayout.Space24)
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        card.Controls.Add(StudentLayout.Label("Thông tin lượt làm bài", StudentLayout.HeadingFontSize, FontStyle.Bold));
        _lockStatus = StudentLayout.Label("");
        card.Controls.Add(_lockStatus);
        _status = StudentLayout.Label("Chưa có thông tin nộp bài.");
        _status.Dock = DockStyle.Top;
        _status.Padding = new Padding(StudentLayout.Space16);
        card.Controls.Add(_status);
        _busyProgress = new ProgressBar
        {
            Dock = DockStyle.Top, Height = StudentLayout.Scale(this, StudentLayout.Space8), Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 0, Visible = false, TabStop = false,
            AccessibleName = "Đang xử lý bài nộp"
        };
        card.Controls.Add(_busyProgress);
        _summary = StudentLayout.Label("Chưa có lượt thi.");
        card.Controls.Add(_summary);
        var actions = new BufferedStudentFlow { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.Button("&Đóng cửa sổ", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty)));
        _actions = actions;
        card.Controls.Add(actions);
        card.SizeChanged += (_, _) =>
        {
            int width = Math.Max(1, card.ClientSize.Width - card.Padding.Horizontal);
            foreach (Control control in card.Controls)
            {
                if (control is Label label) label.MaximumSize = new Size(width, 0);
            }
        };
        _cardHost.Controls.Add(card);
        Controls.Add(_cardHost);
        Controls.Add(heading);
        heading.SizeChanged += (_, _) =>
        {
            int width = Math.Max(1, heading.ClientSize.Width - heading.Padding.Horizontal - StudentLayout.Space12);
            foreach (Control control in heading.Controls)
                if (control is Label label) label.MaximumSize = new Size(width, 0);
        };
    }
}

