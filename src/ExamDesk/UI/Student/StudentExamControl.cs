using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private StudentPreviewAttempt _attempt;
    private bool _rendering;
    private int _imageQuestion = -1;
    private DateTime? _savedAt;
    private string? _saveError;
    public event EventHandler? SubmitRequested;
    internal event EventHandler? ExitReviewRequested;
    public event EventHandler? TimeExpired;
    public StudentExamControl() : this(new StudentPreviewAttempt()) { }
    internal StudentExamControl(StudentPreviewAttempt attempt)
    {
        _attempt = attempt;
        _savedAt = attempt.Draft?.History.LastOrDefault()?.AtUtc.LocalDateTime;
        InitializeComponent();
        RenderQuestion();
    }
    private void RenderQuestion()
    {
        _rendering = true;
        _questionNumber.Text = $"Câu {_attempt.Current + 1:00} / {_attempt.Answers.Length}";
        _question.Text = "Kiểu dữ liệu nào trong C# dùng để lưu giá trị đúng / sai?";
        if (_attempt.Draft is { } draft)
        {
            var question = draft.Questions[_attempt.Current];
            _question.Text = question.Content;
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
        for (int i = 0; i < _options.Length; i++)
        {
            var option = _options[i];
            option.Checked = _attempt.Answers[_attempt.Current] == i;
            option.BackColor = option.Checked ? Color.FromArgb(234, 242, 255) : Color.White;
            option.Invalidate();
        }
        _flag.Checked = _attempt.Flags[_attempt.Current];
        _flag.Text = _flag.Checked ? "⚑ Bỏ đánh dấu &xem lại" : "⚑ Đánh dấu &xem lại";
        _previous.Enabled = _attempt.Current > 0;
        _next.Enabled = _attempt.Current < _attempt.Answers.Length-1;
        _clearChoice.Enabled = !_attempt.Submitted && _attempt.Answers[_attempt.Current].HasValue;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/{_attempt.Answers.Length} · Đánh dấu {_attempt.Flags.Count(flag => flag)}";
        _progressTrack.Invalidate();
        _saveStatus.Text = _saveError ?? (_savedAt is DateTime savedAt ? $"Đã lưu lúc {savedAt:HH:mm:ss}" : "");
        _saveStatus.BackColor = _saveError is null ? UiStyle.SuccessBack : UiStyle.ErrorBack;
        _saveStatus.ForeColor = _saveError is null ? UiStyle.SuccessText : UiStyle.ErrorText;
        _saveStatus.Visible = _saveStatus.Text.Length > 0;
        UpdateClock();
        for (int i = 0; i < _numbers.Length; i++)
        {
            bool answered = _attempt.Answers[i].HasValue;
            bool flagged = _attempt.Flags[i];
            _numbers[i].Text = (i + 1).ToString();
            _numbers[i].AccessibleDescription = $"{(answered ? "Đã trả lời" : "Chưa trả lời")}{(flagged ? ", đánh dấu xem lại" : "")}{(i == _attempt.Current ? ", đang xem" : "")}";
            _numbers[i].BackColor = answered ? UiStyle.SuccessBack : flagged ? UiStyle.WarningBack : UiStyle.Surface;
            _numbers[i].ForeColor = answered ? UiStyle.SuccessText : flagged ? UiStyle.WarningText : UiStyle.Text;
            _numbers[i].FlatAppearance.BorderColor = i == _attempt.Current ? UiStyle.Primary : UiStyle.Border;
            _numbers[i].FlatAppearance.BorderSize = i == _attempt.Current ? 2 : 1;
            _numbers[i].Invalidate();
        }
        _fitContent?.Invoke();
        _rendering = false;
    }
    internal void GoToQuestion(int index)
    {
        if (index < 0 || index >= _attempt.Answers.Length) return;
        _attempt.Current = index;
        RenderQuestion();
        _questionViewport.AutoScrollPosition = Point.Empty;
        _gridViewport.ScrollControlIntoView(_numbers[index]);
    }
    private void Choose(int index)
    {
        if (_rendering || !_options[index].Checked) return;
        ChangeChoice(index);
    }
    private void ChangeChoice(int? choice)
    {
        if (_rendering || _attempt.Submitted) return;
        try { _attempt.Choose(choice); MarkSaved(); }
        catch(Exception ex)
        {
            _saveError = "Chưa lưu được lựa chọn";
            MessageBox.Show(this, ex.Message, _saveError);
        }
        RenderQuestion();
    }
    private void ChangeFlag()
    {
        if (_rendering || _attempt.Submitted) return;
        try { _attempt.Flag(_flag.Checked); MarkSaved(); }
        catch(Exception ex)
        {
            _saveError = "Chưa lưu được cờ xem lại";
            MessageBox.Show(this, ex.Message, _saveError);
        }
        RenderQuestion();
    }

    private void MarkSaved()
    {
        _saveError = null;
        if (_attempt.Draft is not null) _savedAt = DateTime.Now;
    }
    private void UpdateClock()
    {
        if (_attempt.Submitted)
        {
            _clock.Text = "Đã nộp · Chỉ xem";
            return;
        }

        var remaining = _attempt.Deadline - DateTime.Now;
        bool lowTime = remaining <= TimeSpan.FromMinutes(5);
        _clock.Text = (lowTime ? "Sắp hết giờ · " : "Còn lại · ") + _attempt.Remaining;
        _clock.ForeColor = lowTime ? UiStyle.WarningText : UiStyle.Primary;
        _clock.BackColor = lowTime ? UiStyle.WarningBack : Color.FromArgb(234, 242, 255);
    }

    private void Tick(object? sender, EventArgs e)
    {
        UpdateClock();
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
