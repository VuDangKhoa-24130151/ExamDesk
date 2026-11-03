namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private StudentPreviewAttempt _attempt = new();
    public event EventHandler? SubmitRequested;
    public event EventHandler? PauseRequested;
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
            _numbers[i].Text = $"{i + 1}{(_attempt.Answers[i].HasValue ? "+" : "−")}{(_attempt.Flags[i] ? "*" : "")}";
            _numbers[i].AccessibleDescription = $"{(_attempt.Answers[i].HasValue ? "Đã trả lời" : "Chưa trả lời")}{(_attempt.Flags[i] ? ", đánh dấu xem lại" : "")}";
            _numbers[i].BackColor = i == _attempt.Current ? Color.FromArgb(220, 234, 255) : Color.White;
            _numbers[i].FlatAppearance.BorderSize = i == _attempt.Current ? 2 : 1;
        }
    }
    internal void GoToQuestion(int index) { _attempt.Current = index; RenderQuestion(); }
    private void Choose(int index)
    {
        if (!_options[index].Checked) return;
        _attempt.Answers[_attempt.Current] = index;
        _notice.Text = "Lựa chọn đang được giữ trong phiên UI; chưa lưu xuống máy.";
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
