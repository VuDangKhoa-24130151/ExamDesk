namespace ExamDesk.Models;

internal sealed record StudentResult(double Score, int Total, int Correct, int Blank,
    DateTimeOffset SubmittedAt, bool IsAutoSubmitted,
    string StudentName, string StudentCode, string SessionName, string PaperTitle);
