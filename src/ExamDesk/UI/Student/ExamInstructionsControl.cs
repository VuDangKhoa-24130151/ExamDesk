namespace ExamDesk.UI.Student;

public partial class ExamInstructionsControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? StartRequested;
    internal StudentAttemptClock? ResumeClock { get; }
    private bool _expired;

    public ExamInstructionsControl()
    {
        DoubleBuffered = true;
        InitializeComponent();
        _readCheckBox.CheckedChanged += (_, _) => _startButton.Enabled = _expired || _readCheckBox.Checked;
        var sessionToolTip = new ToolTip();
        _sessionCode.TextChanged += (_, _) => sessionToolTip.SetToolTip(_sessionCode, _sessionCode.Text);
        _sessionCode.AutoSize = false;
        _sessionCode.AutoEllipsis = true;
        _sessionCode.Height = _sessionCode.Font.Height + StudentLayout.Space8;
        Disposed += (_, _) => sessionToolTip.Dispose();
        _backButton.Click += GoBack;
        _startButton.Click += (_, _) => StartRequested?.Invoke(this, EventArgs.Empty);
        _page.SizeChanged += (_, _) => WrapLabels(_page);
        _rules.SizeChanged += (_, _) => WrapLabels(_rules);
        Resize += (_, _) => CenterContent();
        DpiChangedAfterParent += (_, _) => CenterContent();
        _resumeInfo.FontChanged += (_, _) => SizeResumeInfo();
        CenterContent();
    }

    public ExamInstructionsControl(ExamDesk.Models.User student, ExamDesk.Models.ExamSession session, ExamDesk.Models.ExamPaper paper, ExamDesk.Models.StudentDraft? draft = null) : this()
    {
        if (draft is not null)
        {
            var attemptClock = ResumeClock = new StudentAttemptClock(draft.StartedUtc, draft.DeadlineUtc);
            var clock = new System.Windows.Forms.Timer { Interval = 1000 };
            var progressText = $"Đã trả lời: {draft.Answers.Count(a => a.HasValue)}/{draft.Answers.Length} câu";
            string? lastRemainingText = null;
            void UpdateProgress()
            {
                var remainingText = attemptClock.FormatRemaining();
                if (remainingText == lastRemainingText) return;
                lastRemainingText = remainingText;
                _expired = remainingText == "00:00";
                var buttonText = _expired ? "Nộp bài ngay" : "&Tiếp tục làm bài";
                if (_startButton.Text != buttonText)
                {
                    _startButton.Text = buttonText;
                    _startButton.Enabled = _expired || _readCheckBox.Checked;
                    SizeResumeInfo();
                }
                _resumeInfo.Text = progressText + $"\nCòn lại: {remainingText}"
                    + (_expired ? "\nLượt này đã hết giờ. Bấm để nộp bài ngay." : "");
            }
            UpdateProgress();
            _resumeInfo.Visible = true;
            clock.Tick += (_, _) => UpdateProgress();
            Disposed += (_, _) => clock.Dispose();
            clock.Start();
        }

        _sessionTitle.Text = session.SessionName;
        _studentInfo.Text = $"{student.FullName} · {student.StudentCode}";
        _classInfo.Text = student.ClassName;
        _paperInfo.Text = paper.Title;
        _questionCount.Text = $"{paper.TotalQuestions}";
        _duration.Text = $"{paper.DurationMinutes} phút";
        _room.Text = session.RoomName;
        _sessionCode.Text = session.SessionCode;
        _durationRule.Text = $"Bài thi có {paper.TotalQuestions} câu trắc nghiệm, thời gian {paper.DurationMinutes} phút.";
    }

    // Giữ chiều cao theo số dòng để mỗi nhịp đồng hồ chỉ cần vẽ lại chữ.
    private void SizeResumeInfo() => _resumeInfo.Height = _resumeInfo.Font.Height * (_expired ? 3 : 2);

    private void CenterContent()
    {
        int maxWidth = StudentLayout.Scale(this, StudentLayout.InstructionsMaxWidth);
        int side = Math.Max(0, (Width - maxWidth) / 2);
        // Chỉ căn nội dung; nền footer vẫn chạy hết chiều rộng màn hình.
        int inset = side + StudentLayout.Scale(this, StudentLayout.Space24);
        var pagePadding = new Padding(inset, _page.Padding.Top, inset, _page.Padding.Bottom);
        if (_page.Padding != pagePadding) _page.Padding = pagePadding;
        var footerPadding = new Padding(inset, _footer.Padding.Top, inset, _footer.Padding.Bottom);
        if (_footer.Padding != footerPadding) _footer.Padding = footerPadding;
    }

    private void GoBack(object? sender, EventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    // Cắt cả nền và nội dung theo góc bo; dựng lại khi đổi kích thước/DPI.
    private static void RoundCard(Control card, Color border)
    {
        static System.Drawing.Drawing2D.GraphicsPath Outline(RectangleF bounds, float radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            float diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        void UpdateRegion()
        {
            if (card.Width < 2 || card.Height < 2) return;
            using var path = Outline(new RectangleF(0, 0, card.Width, card.Height), StudentLayout.Scale(card, 8));
            var previous = card.Region;
            card.Region = new Region(path);
            previous?.Dispose();
            card.Invalidate();
        }

        card.SizeChanged += (_, _) => UpdateRegion();
        card.DpiChangedAfterParent += (_, _) => UpdateRegion();
        card.Paint += (_, e) =>
        {
            if (card.Width < 2 || card.Height < 2) return;
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = Outline(new RectangleF(.5f, .5f, card.Width - 1, card.Height - 1), StudentLayout.Scale(card, 8));
            using var pen = new Pen(border);
            e.Graphics.DrawPath(pen, path);
            e.Graphics.Restore(state);
        };
        UpdateRegion();
    }

    // Nhãn dài tự xuống dòng theo bề ngang của khung chứa nó.
    private static void WrapLabels(TableLayoutPanel panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;
        foreach (Control control in panel.Controls)
        {
            if (control is Label label)
                label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
        }
    }
}
