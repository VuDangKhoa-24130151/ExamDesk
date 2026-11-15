using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentExamService
{
    internal const string StudentNotFoundMessage = "Không tìm thấy mã số sinh viên. Hãy kiểm tra lại mã hoặc liên hệ với giảng viên.";

    private readonly StudentExamRepository _repository = new();
    public User IdentifyStudent(string studentCode)
    {
        var (student, error) = LookupStudent(studentCode);
        return student ?? throw new StudentExamInputException(error!);
    }

    internal (User? Student, string? Error) LookupStudent(string studentCode)
    {
        studentCode = studentCode.Trim();
        if (studentCode.Length == 0) return (null, "Vui lòng nhập mã số sinh viên.");

        var student = _repository.FindStudent(studentCode);
        return student is null
            ? (null, StudentNotFoundMessage)
            : (student, null);
    }

    internal StudentResult? GetResult(StudentDraft draft)
        => GetResult(draft.SessionId, draft.StudentCode);

    internal StudentResult? GetResult(int sessionId, string studentCode)
        => _repository.FindResult(sessionId, studentCode.Trim());

    internal Dictionary<int, bool> GetSessionProgress(string studentCode)
        => _repository.FindSessionProgress(studentCode.Trim());

    public ExamPaper GetSelectedPaper(int sessionId, string studentCode)
        => GetSelectedSession(sessionId, studentCode).Paper;

    internal (ExamSession Session, ExamPaper Paper) GetSelectedSession(int sessionId, string studentCode)
        => _repository.FindSelectedSession(sessionId, studentCode.Trim());

}

internal sealed class StudentExamInputException(string message) : Exception(message);
