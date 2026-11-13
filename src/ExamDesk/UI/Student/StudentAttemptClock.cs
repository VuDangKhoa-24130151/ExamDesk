using System.Diagnostics;

namespace ExamDesk.UI.Student;

internal sealed class StudentAttemptClock
{
    private readonly object _syncLock = new();
    private readonly Stopwatch _elapsed = Stopwatch.StartNew();
    private TimeSpan _lastElapsed;
    private DateTimeOffset _lastEffectiveUtc;

    internal StudentAttemptClock(DateTimeOffset startedUtc, DateTimeOffset deadlineUtc)
    {
        DeadlineUtc = deadlineUtc;
        var now = DateTimeOffset.UtcNow;
        _lastEffectiveUtc = now > startedUtc ? now : startedUtc;
    }

    internal DateTimeOffset DeadlineUtc { get; }

    // Stopwatch giữ thời gian không chạy lùi trong tiến trình. SQLite lưu riêng từng máy,
    // nên không thể chống tuyệt đối việc đổi giờ, nhất là khi tắt rồi mở lại ứng dụng.
    internal DateTimeOffset ReadUtc()
    {
        lock (_syncLock)
        {
            var elapsed = _elapsed.Elapsed;
            var wallUtc = DateTimeOffset.UtcNow;
            var monotonicUtc = _lastEffectiveUtc + (elapsed - _lastElapsed);
            _lastElapsed = elapsed;
            _lastEffectiveUtc = wallUtc > monotonicUtc ? wallUtc : monotonicUtc;
            return _lastEffectiveUtc;
        }
    }

    internal TimeSpan Remaining => DeadlineUtc - ReadUtc();
    internal bool IsExpired => Remaining <= TimeSpan.Zero;

    internal string FormatRemaining()
    {
        var seconds = Math.Max(0, (long)Math.Ceiling(Remaining.TotalSeconds));
        return $"{seconds / 60:00}:{seconds % 60:00}";
    }
}
