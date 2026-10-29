namespace ExamDesk.Models;

public sealed class Subject
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class Question
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int Chapter { get; set; }
    public string Difficulty { get; set; } = string.Empty; // Easy / Medium / Hard
    public string Content { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = string.Empty; // A / B / C / D
    public string ImagePath { get; set; } = string.Empty;
}

public sealed class ExamPaper
{
    public int Id { get; set; }
    public string PaperCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int DurationMinutes { get; set; }
    public int EasyCount { get; set; }
    public int MediumCount { get; set; }
    public int HardCount { get; set; }
    public int TotalQuestions => EasyCount + MediumCount + HardCount;
}
