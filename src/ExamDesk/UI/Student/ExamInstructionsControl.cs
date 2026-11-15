namespace ExamDesk.UI.Student;

public partial class ExamInstructionsControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? StartRequested;
    internal StudentAttemptClock? ResumeClock { get; }
    private bool _expired;

    public ExamInstructionsControl()
    {
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
        CenterContent();
    }

    public ExamInstructionsControl(ExamDesk.Models.User student, ExamDesk.Models.ExamSession session, ExamDesk.Models.ExamPaper paper, ExamDesk.Models.StudentDraft? draft = null) : this()
    {
        if (draft is not null)
        {
            _startButton.Text = "&Tiếp tục làm bài";
            _resumeInfo.Visible = true;
            var attemptClock = ResumeClock = new StudentAttemptClock(draft.StartedUtc, draft.DeadlineUtc);
            var clock = new System.Windows.Forms.Timer { Interval = 1000 };
            void UpdateProgress()
            {
                var remainingText = attemptClock.FormatRemaining();
                _expired = remainingText == "00:00";
                _startButton.Text = _expired ? "Nộp bài ngay" : "&Tiếp tục làm bài";
                _startButton.Enabled = _expired || _readCheckBox.Checked;
                _resumeInfo.Text = $"Đã trả lời: {draft.Answers.Count(a => a.HasValue)}/{draft.Answers.Length} câu"
                    + $"\nCòn lại: {remainingText}"
                    + (_expired ? "\nLượt này đã hết giờ. Bấm để nộp bài ngay." : "");
            }
            UpdateProgress();
            clock.Tick += (_, _) => UpdateProgress();
            Disposed += (_, _) => clock.Dispose();
            clock.Start();
        }

        _sessionTitle.Text = session.SessionName;
        _studentInfo.Text = $"{student.FullName} · {student.StudentCode} · Lớp {student.ClassName}";
        _paperInfo.Text = paper.Title;
        _questionCount.Text = $"{paper.TotalQuestions}";
        _duration.Text = $"{paper.DurationMinutes} phút";
        _room.Text = session.RoomName;
        _sessionCode.Text = session.SessionCode;
        _durationRule.Text = $"Bài thi có {paper.TotalQuestions} câu trắc nghiệm, thời gian {paper.DurationMinutes} phút.";
    }

    private void CenterContent()
    {
        int maxWidth = StudentLayout.Scale(this, StudentLayout.InstructionsMaxWidth);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
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
