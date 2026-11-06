namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private StudentPreviewAttempt _attempt;
    private bool _rendering;
    private int _imageQuestion = -1;
    public event EventHandler? SubmitRequested;
    public event EventHandler? TimeExpired;
    public StudentExamControl() : this(new StudentPreviewAttempt()) { }
    internal StudentExamControl(StudentPreviewAttempt attempt) { _attempt = attempt; InitializeComponent(); RenderQuestion(); }
    private void RenderQuestion()
    {
        _rendering = true;
        _question.Text = $"Câu {_attempt.Current + 1:00} / {_attempt.Answers.Length} · C# cơ bản\n\nKiểu dữ liệu nào trong C# dùng để lưu giá trị đúng / sai?";
        if (_attempt.Draft is { } draft)
        {
            var question = draft.Questions[_attempt.Current];
            _question.Text = $"Câu {_attempt.Current+1} / {draft.Questions.Count}\n\n{question.Content}";
            if(_imageQuestion != _attempt.Current)
            {
                _image.Image?.Dispose(); _image.Image=null; _image.Visible=false;
                _imageQuestion=_attempt.Current;
                if(!string.IsNullOrWhiteSpace(question.ImagePath))
                {
                    try
                    {
                        using var original=Image.FromFile(question.ImagePath);
                        _image.Image=new Bitmap(original); _image.Visible=true;
                    }
                    catch(Exception ex) when(ex is IOException or ArgumentException or OutOfMemoryException or UnauthorizedAccessException)
                    { _question.Text += "\n[Không tải được ảnh minh họa]"; }
                }
            }
            for(int i=0;i<4;i++) _options[i].Text = $"{(char)('A'+i)}. {question.Options[i]}";
        }
        // Câu hỏi lặp để dựng giao diện, sẽ thay bằng dữ liệu ca thi sau.
        for (int i = 0; i < 4; i++) _options[i].Checked = _attempt.Answers[_attempt.Current] == i;
        _flag.Checked = _attempt.Flags[_attempt.Current];
        _previous.Enabled = _attempt.Current > 0;
        _next.Enabled = _attempt.Current < _attempt.Answers.Length-1;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/{_attempt.Answers.Length} câu";
        _clock.Text = "Còn lại " + _attempt.Remaining;
        for (int i = 0; i < _numbers.Length; i++)
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
        _rendering = false;
    }
    internal void GoToQuestion(int index) { if(index<0 || index>=_attempt.Answers.Length) return; _attempt.Current = index; RenderQuestion(); }
    private void Choose(int index)
    {
        if (_rendering || !_options[index].Checked) return;
        ChangeChoice(index);
    }
    private void ChangeChoice(int? choice)
    {
        if (_rendering) return;
        try { _attempt.Choose(choice); }
        catch(Exception ex) { MessageBox.Show(this,ex.Message,"Không lưu được lựa chọn"); }
        RenderQuestion();
    }
    private void ChangeFlag()
    {
        if (_rendering) return;
        try { _attempt.Flag(_flag.Checked); }
        catch(Exception ex) { MessageBox.Show(this,ex.Message,"Không lưu được cờ xem lại"); }
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
        if (disposing) { _timer.Dispose(); _image.Image?.Dispose(); }
        base.Dispose(disposing);
    }
}
