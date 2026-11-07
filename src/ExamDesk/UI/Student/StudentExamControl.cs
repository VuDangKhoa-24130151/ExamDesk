namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private StudentPreviewAttempt _attempt = new();
    public event EventHandler? SubmitRequested;
    public event EventHandler? TimeExpired;
    public StudentExamControl() { InitializeComponent(); RenderQuestion(); }
    internal StudentExamControl(StudentPreviewAttempt attempt) : this() { _attempt = attempt; RenderQuestion(); }
    private void RenderQuestion()
    {
        _question.Text = $"Câu {_attempt.Current + 1:00} / 30 · C# cơ bản\n\nKiểu dữ liệu nào trong C# dùng để lưu giá trị đúng / sai?";
        // Câu hỏi lặp để dựng giao diện, sẽ thay bằng dữ liệu ca thi sau.
        for (int i = 0; i < 4; i++) _options[i].Checked = _attempt.Answers[_attempt.Current] == i;
        _flag.Checked = _attempt.Flags[_attempt.Current];
        _previous.Enabled = _attempt.Current > 0;
        _next.Enabled = _attempt.Current < 29;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/30 câu";
        _clock.Text = "Còn lại " + _attempt.Remaining;
        for (int i = 0; i < 30; i++)
        {
            bool answered = _attempt.Answers[i].HasValue;
            bool flagged = _attempt.Flags[i];
            string symbols = (answered ? "✓" : "") + (flagged ? "⚑" : "");
            _numbers[i].Text = symbols.Length == 0 ? (i + 1).ToString() : $"{i + 1}\n{symbols}";
            _numbers[i].AccessibleDescription = $"{(_attempt.Answers[i].HasValue ? "Đã trả lời" : "Chưa trả lời")}{(_attempt.Flags[i] ? ", đánh dấu xem lại" : "")}";
            _numbers[i].BackColor = _attempt.Flags[i] ? ExamDesk.UI.Shared.UiStyle.Primary
                : answered ? Color.FromArgb(234, 246, 239) : Color.White;
            _numbers[i].ForeColor = flagged ? Color.White
                : answered ? Color.FromArgb(20, 108, 67) : ExamDesk.UI.Shared.UiStyle.Text;
            _numbers[i].FlatAppearance.BorderColor = i == _attempt.Current ? Color.Black : Color.FromArgb(220, 227, 237);
            _numbers[i].FlatAppearance.BorderSize = i == _attempt.Current ? 2 : 1;
        }
    }
    internal void GoToQuestion(int index) { _attempt.Current = index; RenderQuestion(); }
    private void Choose(int index)
    {
        if (!_options[index].Checked) return;
        _attempt.Answers[_attempt.Current] = index;
        RenderQuestion();
    }
    private void Tick(object? sender, EventArgs e)
    {
        _clock.Text = "Còn lại " + _attempt.Remaining;
        if (DateTime.Now < _attempt.Deadline || _attempt.Submitted) return;
        _timer.Stop();
        TimeExpired?.Invoke(this, EventArgs.Empty);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }
}
