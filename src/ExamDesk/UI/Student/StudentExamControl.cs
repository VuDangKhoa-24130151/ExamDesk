using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private StudentAttemptState _attempt;
    private bool _rendering;
    private int _imageQuestion = -1;
    private DateTime? _savedAt;
    private string? _saveError;
    private string? _saveNotice;
    public event EventHandler? SubmitRequested;
    internal event EventHandler? ExitReviewRequested;
    internal event EventHandler? SaveExpired;
    internal event EventHandler? DraftLocked;
    public StudentExamControl() : this(new StudentAttemptState()) { }
    internal StudentExamControl(StudentAttemptState attempt)
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
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                        _question.Text += "\n[Không tải được ảnh minh họa. Hãy báo giảng viên.]";
                    }
                }
            }
            for(int i=0;i<4;i++) _options[i].Text = $"{(char)('A'+i)}. {question.Options[i]}";
        }
        for (int i = 0; i < _options.Length; i++)
        {
            var option = _options[i];
            option.Checked = _attempt.Answers[_attempt.Current] == i;
            option.BackColor = option.Checked ? UiStyle.Background : UiStyle.Surface;
            option.Invalidate();
        }
        _flag.Checked = _attempt.Flags[_attempt.Current];
        _flag.Text = _flag.Checked ? "⚑ Bỏ đánh dấu &xem lại" : "⚑ Đánh dấu &xem lại";
        _previous.Enabled = _attempt.Current > 0;
        _next.Enabled = _attempt.Current < _attempt.Answers.Length-1;
        bool locked = _attempt.Draft?.PendingSubmission == true && !_attempt.Submitted;
        foreach (var option in _options) option.Enabled = !locked;
        _flag.Enabled = !_attempt.Submitted && !locked;
        _clearChoice.Enabled = !_attempt.Submitted && !locked && _attempt.Answers[_attempt.Current].HasValue;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/{_attempt.Answers.Length} · Đánh dấu {_attempt.Flags.Count(flag => flag)}";
        _progressTrack.Invalidate();
        _saveStatus.Text = _saveError ?? _saveNotice ?? (_savedAt is DateTime savedAt ? $"Đã lưu lúc {savedAt:HH:mm:ss}" : "");
        _saveStatus.BackColor = _saveError is not null ? UiStyle.ErrorBack
            : _saveNotice is not null ? UiStyle.WarningBack : UiStyle.SuccessBack;
        _saveStatus.ForeColor = _saveError is not null ? UiStyle.ErrorText
            : _saveNotice is not null ? UiStyle.WarningText : UiStyle.SuccessText;
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
    internal bool IsSaving { get; private set; }

    private void ChangeChoice(int? choice) => SaveChange(() => _attempt.Choose(choice));
    private void ChangeFlag()
    {
        bool flagged = _flag.Checked;
        SaveChange(() => _attempt.Flag(flagged));
    }

    private async void SaveChange(Action change)
    {
        if (_rendering || _attempt.Submitted || IsSaving) return;
        IsSaving = true;
        Enabled = false;
        UseWaitCursor = true;
        _timer.Stop();
        _saveStatus.Text = "Đang lưu tiến độ…";
        _saveStatus.BackColor = UiStyle.Background;
        _saveStatus.ForeColor = UiStyle.Primary;
        _saveStatus.Visible = true;
        _fitContent?.Invoke();
        Action afterSave = RenderQuestion;

        try
        {
            await Task.Run(change);
            MarkSaved();
        }
        catch (ExamDesk.Models.StudentDraftExpiredException)
        {
            afterSave = () => SaveExpired?.Invoke(this, EventArgs.Empty);
        }
        catch (ExamDesk.Models.StudentDraftConflictException)
        {
            afterSave = ReloadAfterConflict;
        }
        catch (Exception ex)
        {
            _saveError = StudentError.Describe(ex,
                "Chưa lưu được thay đổi. Hãy chọn lại hoặc báo giảng viên nếu lỗi tiếp diễn.");
        }
        finally
        {
            IsSaving = false;
            if (!IsDisposed)
            {
                UseWaitCursor = false;
                Enabled = true;
                _timer.Start();
                afterSave();
            }
        }
    }

    private void ReloadAfterConflict()
    {
        _savedAt = null;
        _saveNotice = null;
        _saveError = "Chưa tải lại được tiến độ. Hãy đóng và mở lại lượt thi.";
        try
        {
            // Chỉ hiển thị dữ liệu mới nhất, không ghi lại lựa chọn vừa bị từ chối.
            if (_attempt.ReloadDraft())
            {
                _saveError = null;
                _saveNotice = "Lượt thi đã thay đổi ở cửa sổ khác. Đã tải lại tiến độ mới nhất.";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.ToString());
        }
        if (_saveError is null && _attempt.Draft is { } draft)
        {
            if (draft.PendingSubmission)
            {
                DraftLocked?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (_attempt.IsExpired)
            {
                SaveExpired?.Invoke(this, EventArgs.Empty);
                return;
            }
            _imageQuestion = -1;
            if (_numbers.Length != _attempt.Answers.Length) BuildQuestionGrid();
        }
        RenderQuestion();
    }

    private void MarkSaved()
    {
        _saveError = null;
        _saveNotice = null;
        if (_attempt.Draft is not null) _savedAt = DateTimeOffset.UtcNow.LocalDateTime;
    }
    private void UpdateClock()
    {
        if (_attempt.Submitted)
        {
            _clock.Text = "Đã nộp · Chỉ xem";
            return;
        }

        var remaining = _attempt.TimeRemaining;
        bool lowTime = remaining <= TimeSpan.FromMinutes(5);
        _clock.Text = (lowTime ? "Sắp hết giờ · " : "Còn lại · ") + _attempt.Remaining;
        _clock.ForeColor = lowTime ? UiStyle.WarningText : UiStyle.Primary;
        _clock.BackColor = lowTime ? UiStyle.WarningBack : UiStyle.Background;
    }

    private void Tick(object? sender, EventArgs e)
    {
        UpdateClock();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _timer.Dispose(); _image.Image?.Dispose(); }
        base.Dispose(disposing);
    }
}
