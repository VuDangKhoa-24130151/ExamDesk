namespace ExamDesk.UI.Student;

public partial class ExamInstructionsControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? StartRequested;
    internal StudentAttemptClock? ResumeClock { get; }

    public ExamInstructionsControl()
    {
        InitializeComponent();
        _readCheckBox.CheckedChanged += (_, _) => _startButton.Enabled = _readCheckBox.Checked;
        _startButton.Click += (_, _) => StartRequested?.Invoke(this, EventArgs.Empty);
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
                _resumeInfo.Text = $"Đã trả lời: {draft.Answers.Count(a => a.HasValue)}/{draft.Answers.Length} câu"
                    + $"\nCòn lại: {attemptClock.FormatRemaining()}";
            }
            UpdateProgress();
            clock.Tick += (_, _) => UpdateProgress();
            Disposed += (_, _) => clock.Dispose();
            clock.Start();
        }

        _sessionTitle.Text = session.SessionName;
        _studentInfo.Text = $"{student.FullName} · {student.StudentCode}";
        _sessionInfo.Text = $"Lớp {student.ClassName} · Phòng {session.RoomName} · Mã ca {session.SessionCode}";
        _paperInfo.Text = $"{paper.Title} · {paper.TotalQuestions} câu · {paper.DurationMinutes} phút";
        _durationRule.Text = $"01  Bài thi có {paper.TotalQuestions} câu trắc nghiệm, thời gian {paper.DurationMinutes} phút.";
    }

    private void CenterContent()
    {
        int maxWidth = (int)(1120 * DeviceDpi / 96f);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    // Nhãn dài tự xuống dòng theo bề ngang của khung chứa nó.
    private static void WrapLabels(FlowLayoutPanel panel)
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
