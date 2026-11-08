namespace ExamDesk.Models;

// Không chứa đáp án đúng; snapshot nội dung để giữ bài khi ngân hàng thay đổi.
public sealed class StudentQuestion
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string[] Options { get; set; } = [];
    public string ImagePath { get; set; } = "";
}
public sealed class AnswerChange
{
    public int QuestionId { get; set; }
    public int? Choice { get; set; }
    public DateTimeOffset AtUtc { get; set; }
}
public sealed class StudentDraft
{
    public int SessionId { get; set; }
    public int PaperId { get; set; }
    public string StudentCode { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTimeOffset StartedUtc { get; set; }
    public DateTimeOffset DeadlineUtc { get; set; }
    public List<StudentQuestion> Questions { get; set; } = [];
    public int?[] Answers { get; set; } = [];
    public bool[] Flags { get; set; } = [];
    public List<AnswerChange> History { get; set; } = [];
    public bool PendingSubmission { get; set; }
    public bool AutoSubmitted { get; set; }
    public int Revision { get; set; }
}
