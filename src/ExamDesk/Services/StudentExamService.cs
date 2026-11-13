using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentExamService
{
    internal const string StudentNotFoundMessage = "Không tìm thấy mã số sinh viên. Hãy kiểm tra lại mã hoặc liên hệ với giảng viên.";

    private readonly StudentExamRepository _repository = new();
    private readonly ExamSessionService _sessionService = new();
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
    {
        var session = _sessionService.GetSessions().FirstOrDefault(s => s.Id == sessionId)
            ?? throw new StudentExamInputException("Ca thi không còn tồn tại. Hãy tải lại danh sách.");
        var hasProgress = GetSessionProgress(studentCode).TryGetValue(sessionId, out var submitted);
        if (submitted) throw new StudentExamInputException("Bạn đã nộp bài ở ca này. Hãy tải lại danh sách ca.");
        if (!session.IsOpen && !hasProgress)
            throw new StudentExamInputException("Ca thi đã đóng và bạn chưa bắt đầu lượt thi. Hãy chọn ca đang mở hoặc hỏi giám thị coi thi.");
        return _sessionService.GetExamPapers().FirstOrDefault(p => p.Id == session.PaperId)
            ?? throw new StudentExamInputException("Ca thi chưa có đề. Hãy tải lại danh sách hoặc hỏi giám thị coi thi.");
    }
}

internal sealed class StudentExamInputException(string message) : Exception(message);
