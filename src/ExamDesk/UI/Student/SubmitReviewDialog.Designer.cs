using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SubmitReviewDialog
{
    private readonly System.Windows.Forms.Timer _clock = new() { Interval = 1000 };
    private TableLayoutPanel _shell = null!;
    private TableLayoutPanel _headerBar = null!;
    private Panel _scrollViewport = null!;
    private TableLayoutPanel _scrollContent = null!;
    private FlowLayoutPanel _unansweredGroup = null!;
    private FlowLayoutPanel _flaggedGroup = null!;
    private TableLayoutPanel _footerBar = null!;
    private Label _titleLabel = null!;
    private DialogClockPill _warning = null!;
    private Label _unansweredSummary = null!;
    private Label _unansweredTitle = null!;
    private Label _flaggedTitle = null!;
    private StudentButton _continueButton = null!, _confirmButton = null!;

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
        ClientSize = new Size(StudentLayout.SubmitDialogWidth, StudentLayout.SubmitDialogHeight);
        MinimumSize = new Size(StudentLayout.SubmitDialogMinWidth, StudentLayout.SubmitDialogMinHeight);
        BackColor = UiStyle.Surface;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        _titleLabel = new Label
        {
            Text = "Nộp bài?",
            AutoSize = true,
            UseMnemonic = false,
            ForeColor = UiStyle.Text,
            Margin = Padding.Empty
        };

        _warning = new DialogClockPill
        {
            AutoSize = false,
            UseMnemonic = false,
            Anchor = AnchorStyles.Right,
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Background,
            AccessibleName = "Thời gian còn lại"
        };

        _unansweredSummary = new Label
        {
            AutoSize = true,
            UseMnemonic = false,
            ForeColor = UiStyle.TextMuted,
            Margin = Padding.Empty
        };

        _headerBar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.AutoSize)
            },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize)
            }
        };
        _headerBar.Paint += (_, e) => StudentLayout.DrawHeaderBorder(_headerBar, e.Graphics);
        _headerBar.Controls.Add(_titleLabel, 0, 0);
        _headerBar.Controls.Add(_warning, 1, 0);
        _headerBar.SetRowSpan(_warning, 2);
        _headerBar.Controls.Add(_unansweredSummary, 0, 1);

        _unansweredTitle = new Label
        {
            Text = "Chưa trả lời",
            AutoSize = true,
            UseMnemonic = false,
            ForeColor = UiStyle.Text
        };
        _unansweredGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UiStyle.Surface,
            Padding = Padding.Empty,
            TabStop = false
        };

        _flaggedTitle = new Label
        {
            Text = "Đánh dấu xem lại",
            AutoSize = true,
            UseMnemonic = false,
            ForeColor = UiStyle.Text
        };
        _flaggedGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = UiStyle.Surface,
            Padding = Padding.Empty,
            TabStop = false
        };

        _scrollContent = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize)
            }
        };
        _scrollContent.Controls.Add(_unansweredTitle, 0, 0);
        _scrollContent.Controls.Add(_unansweredGroup, 0, 1);
        _scrollContent.Controls.Add(_flaggedTitle, 0, 2);
        _scrollContent.Controls.Add(_flaggedGroup, 0, 3);

        _scrollViewport = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            TabIndex = 0,
            TabStop = false
        };
        _scrollViewport.Controls.Add(_scrollContent);

        _continueButton = new StudentButton("Tiếp tục &làm bài", StudentButtonKind.Secondary)
        {
            AccessibleName = "Tiếp tục làm bài",
            Anchor = AnchorStyles.Left,
            TabIndex = 0
        };
        _continueButton.Click += (_, _) =>
        {
            if (_isSubmitting) return;
            DialogResult = DialogResult.Cancel;
            Close();
        };

        _confirmButton = new StudentButton("&Nộp bài", StudentButtonKind.Primary)
        {
            AccessibleName = "Nộp bài",
            Anchor = AnchorStyles.Right,
            TabIndex = 1
        };
        _confirmButton.Click += (_, _) =>
        {
            if (_isSubmitting) return;
            IsSubmitting = true;
            DialogResult = DialogResult.OK;
            Close();
        };

        _footerBar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            TabIndex = 1,
            TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.AutoSize),
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.AutoSize)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _footerBar.Paint += (_, e) => StudentLayout.DrawTopBorder(_footerBar, e.Graphics);
        _footerBar.Controls.Add(_continueButton, 0, 0);
        _footerBar.Controls.Add(_confirmButton, 2, 0);

        _shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100),
                new RowStyle(SizeType.Absolute, StudentLayout.BarHeight)
            }
        };
        _shell.Controls.Add(_headerBar, 0, 0);
        _shell.Controls.Add(_scrollViewport, 0, 1);
        _shell.Controls.Add(_footerBar, 0, 2);
        Controls.Add(_shell);

        CancelButton = _continueButton;

        ApplyDialogLayout();
        _scrollViewport.SizeChanged += (_, _) => FitScrollContent();
        DpiChanged += (_, _) => { ApplyDialogLayout(); FitScrollContent(); };
        ResumeLayout(true);
    }
}
