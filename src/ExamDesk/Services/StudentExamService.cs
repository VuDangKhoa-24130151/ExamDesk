using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentExamService
{
    private readonly StudentExamRepository _repository = new();
    private readonly ExamSessionService _sessionService = new();
    public User IdentifyStudent(string studentCode)
    {
        studentCode = studentCode.Trim();
        if (studentCode.Length == 0) throw new StudentExamInputException("Vui lòng nhập mã sinh viên.");
        return _repository.FindStudent(studentCode)
            ?? throw new StudentExamInputException("Không tìm thấy MSSV trong danh mục sinh viên. Vui lòng kiểm tra lại hoặc liên hệ giảng viên.");
    }
    internal StudentResult? GetResult(StudentDraft draft)
        => GetResult(draft.SessionId, draft.StudentCode);

    internal StudentResult? GetResult(int sessionId, string studentCode)
        => _repository.FindResult(sessionId, studentCode.Trim());

    public List<ExamSession> GetOpenSessions() => _sessionService.GetSessions().Where(s => s.IsOpen).ToList();
    internal Dictionary<int, bool> GetSessionProgress(string studentCode)
        => _repository.FindSessionProgress(studentCode.Trim());

    public ExamPaper GetSelectedPaper(int sessionId, string studentCode)
    {
        var session = _sessionService.GetSessions().FirstOrDefault(s => s.Id == sessionId)
            ?? throw new StudentExamInputException("Ca thi không còn tồn tại. Hãy tải lại danh sách.");
        var hasProgress = GetSessionProgress(studentCode).TryGetValue(sessionId, out var submitted);
        if (submitted) throw new StudentExamInputException("Bạn đã nộp bài ở ca này. Hãy tải lại danh sách ca.");
        if (!session.IsOpen && !hasProgress)
            throw new StudentExamInputException("Ca thi đã đóng và bạn chưa bắt đầu lượt thi. Hãy chọn ca đang mở hoặc liên hệ giảng viên.");
        return _sessionService.GetExamPapers().FirstOrDefault(p => p.Id == session.PaperId)
            ?? throw new StudentExamInputException("Ca thi chưa có đề. Hãy tải lại danh sách hoặc liên hệ giảng viên.");
    }
}

internal sealed class StudentExamInputException(string message) : Exception(message);
