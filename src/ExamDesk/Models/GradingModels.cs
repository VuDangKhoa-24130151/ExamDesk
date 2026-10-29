namespace ExamDesk.Models;

public sealed class ExamAttempt
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int PaperId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public double Score { get; set; }
    public bool IsAutoSubmitted { get; set; }
    public string ReviewNote { get; set; } = string.Empty;
}

public sealed class AttemptAnswer
{
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public int QuestionOrder { get; set; }
    public string SelectedOption { get; set; } = string.Empty; // Rỗng = bỏ trống
    public string CorrectOption { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
