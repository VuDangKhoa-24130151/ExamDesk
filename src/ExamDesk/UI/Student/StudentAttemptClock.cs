using System.Diagnostics;

namespace ExamDesk.UI.Student;

internal sealed class StudentAttemptClock
{
    private readonly Stopwatch _elapsed = Stopwatch.StartNew();
    private TimeSpan _lastElapsed;
    private DateTimeOffset _lastEffectiveUtc;
    private DateTimeOffset _wallAnchorUtc = DateTimeOffset.UtcNow;
    private TimeSpan _wallAnchorElapsed;

    internal StudentAttemptClock(DateTimeOffset startedUtc, DateTimeOffset deadlineUtc)
    {
        DeadlineUtc = deadlineUtc;
        _lastEffectiveUtc = _wallAnchorUtc > startedUtc ? _wallAnchorUtc : startedUtc;
    }

    internal DateTimeOffset DeadlineUtc { get; }

    // Stopwatch giữ thời gian không chạy lùi trong tiến trình. SQLite lưu riêng từng máy,
    // nên không thể chống tuyệt đối việc đổi giờ, nhất là khi tắt rồi mở lại ứng dụng.
    internal DateTimeOffset ReadUtc()
    {
        var elapsed = _elapsed.Elapsed;
        var wallUtc = DateTimeOffset.UtcNow;
        var drift = wallUtc - (_wallAnchorUtc + (elapsed - _wallAnchorElapsed));
        if (drift.Duration() >= TimeSpan.FromSeconds(2))
        {
            Trace.WriteLine($"Giờ hệ thống thay đổi khoảng {drift.TotalSeconds:F1} giây trong lượt thi.");
            _wallAnchorUtc = wallUtc;
            _wallAnchorElapsed = elapsed;
        }

        var monotonicUtc = _lastEffectiveUtc + (elapsed - _lastElapsed);
        _lastElapsed = elapsed;
        _lastEffectiveUtc = wallUtc > monotonicUtc ? wallUtc : monotonicUtc;
        return _lastEffectiveUtc;
    }

    internal TimeSpan Remaining => DeadlineUtc - ReadUtc();
    internal bool IsExpired => Remaining <= TimeSpan.Zero;

    internal string FormatRemaining()
    {
        var seconds = Math.Max(0, (long)Math.Ceiling(Remaining.TotalSeconds));
        return $"{seconds / 60:00}:{seconds % 60:00}";
    }
}
