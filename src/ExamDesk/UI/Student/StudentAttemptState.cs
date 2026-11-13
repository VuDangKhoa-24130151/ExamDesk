namespace ExamDesk.UI.Student;

// Trạng thái hiển thị và tiến độ đã lưu của lượt thi.
internal sealed class StudentAttemptState
{
    public ExamDesk.Models.StudentDraft? Draft { get; private set; }
    private readonly ExamDesk.Services.StudentDraftService _service = new();
    private readonly StudentAttemptClock _clock;

#if DEBUG
    private readonly int?[] _demoAnswers = new int?[40];
    private readonly bool[] _demoFlags = new bool[40];

    public StudentAttemptState()
    {
        var now = DateTimeOffset.UtcNow;
        _clock = new StudentAttemptClock(now, now.AddMinutes(45));
    }

#endif

    public StudentAttemptState(ExamDesk.Models.StudentDraft draft, StudentAttemptClock? resumeClock = null)
    {
        Draft = draft;
        _clock = resumeClock is not null && resumeClock.DeadlineUtc == draft.DeadlineUtc
            ? resumeClock : new StudentAttemptClock(draft.StartedUtc, draft.DeadlineUtc);
    }
#if DEBUG
    public int?[] Answers => Draft?.Answers ?? _demoAnswers;
    public bool[] Flags => Draft?.Flags ?? _demoFlags;
    public string Title => Draft?.Title ?? "Làm bài demo (không lưu)";
#else
    public int?[] Answers => Draft!.Answers;
    public bool[] Flags => Draft!.Flags;
    public string Title => Draft!.Title;
#endif
    public int Current { get; set; }
    public bool IsExpired => _clock.IsExpired;
    public TimeSpan TimeRemaining => _clock.Remaining;
    public bool Submitted { get; private set; }
    public int Answered => Answers.Count(answer => answer.HasValue);
    public void Choose(int? choice)
    {
#if DEBUG
        if (Draft is null)
        {
            _demoAnswers[Current] = choice;
            return;
        }
#endif
        Draft = _service.Change(Draft!, Current, choice, null, _clock.ReadUtc);
    }
    public void Flag(bool value)
    {
#if DEBUG
        if (Draft is null)
        {
            _demoFlags[Current] = value;
            return;
        }
#endif
        Draft = _service.Change(Draft!, Current, Answers[Current], value, _clock.ReadUtc);
    }
    internal bool ReloadDraft()
    {
        if (Draft is not { } draft) return false;
        var latest = _service.FindDraft(draft.SessionId, draft.StudentCode);
        if (latest is null || latest.StartedUtc != draft.StartedUtc || latest.Answers.Length == 0) return false;

        Draft = latest;
        Current = Math.Clamp(Current, 0, Answers.Length - 1);
        return true;
    }
    public void Submit()
    {
        if (Draft is not null)
        {
            try
            {
                Draft = _service.LockForSubmission(Draft, _clock.ReadUtc);
            }
            catch (ExamDesk.Models.StudentDraftConflictException)
            {
                // Chỉ thử khóa lại một lần với bản mới nhất.
                if (!ReloadDraft()) throw;
                Draft = _service.LockForSubmission(Draft!, _clock.ReadUtc);
            }
            // Chỉ đánh dấu đã nộp sau khi bộ chấm lưu xong.
            _service.SendForGrading(Draft);
        }
        Submitted = true;
    }
    public string Remaining => _clock.FormatRemaining();
}
