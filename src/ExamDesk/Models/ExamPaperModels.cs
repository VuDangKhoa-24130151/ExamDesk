namespace ExamDesk.Models;

/// <summary>
/// Số lượng câu hỏi hiện có theo từng mức độ khó của một môn học.
/// </summary>
public sealed class ExamPaperDifficultyCounts
{
    public int EasyCount { get; set; }
    public int MediumCount { get; set; }
    public int HardCount { get; set; }
    public int TotalCount => EasyCount + MediumCount + HardCount;
}

/// <summary>
/// Thông tin tóm tắt đề thi dùng cho danh sách và kiểm tra trạng thái khóa.
/// Bọc ExamPaper (do ExamPaper là sealed) kèm tên môn, số câu đã gắn và số ca thi đang dùng.
/// </summary>
public sealed class ExamPaperSummary
{
    public ExamPaper Paper { get; set; } = new();

    public int Id => Paper.Id;
    public string PaperCode => Paper.PaperCode;
    public string Title => Paper.Title;
    public int SubjectId => Paper.SubjectId;
    public int DurationMinutes => Paper.DurationMinutes;
    public int EasyCount => Paper.EasyCount;
    public int MediumCount => Paper.MediumCount;
    public int HardCount => Paper.HardCount;
    public int TotalQuestions => Paper.TotalQuestions;

    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int AssignedQuestionCount { get; set; }
    public int SessionCount { get; set; }
    public bool IsLocked => SessionCount > 0;
}

/// <summary>
/// Chi tiết một câu hỏi đã gắn vào đề thi theo đúng thứ tự OrderIndex.
/// </summary>
public sealed class ExamPaperQuestionDetail
{
    public int OrderIndex { get; set; }
    public int QuestionId { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Chapter { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
