namespace ExamDesk.Models;

public sealed class ExamSession
{
    public int Id { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string SessionName { get; set; } = string.Empty;
    public int PaperId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
}

// Dữ liệu trao đổi giữa Bạn 2 và Bạn 3; chưa có logic làm bài hoặc chấm điểm.
public sealed class ExamSubmission
{
    public int SessionId { get; set; }
    public int PaperId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset DeadlineUtc { get; set; }
    public Dictionary<int, string> SelectedAnswers { get; set; } = new();
    public bool IsAutoSubmitted { get; set; }
}
