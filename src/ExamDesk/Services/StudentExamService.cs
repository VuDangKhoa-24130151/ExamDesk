using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentExamService
{
    private readonly StudentExamRepository _repository = new();
    public User IdentifyStudent(string studentCode)
    {
        studentCode = studentCode.Trim();
        if (studentCode.Length == 0) throw new ArgumentException("Vui lòng nhập mã sinh viên.");
        return _repository.FindStudent(studentCode)
            ?? throw new InvalidOperationException("Không tìm thấy MSSV trong danh mục sinh viên. Vui lòng kiểm tra lại hoặc liên hệ giảng viên.");
    }
    public List<ExamSession> GetOpenSessions() => _repository.FindOpenSessions();
    public List<Question> GetQuestionsForPaper(int paperId)
        => throw new NotImplementedException("Chưa nối đề thật vào màn làm bài.");
    public bool HasAlreadySubmitted(int sessionId, string studentCode)
        => _repository.HasStudentSubmitted(sessionId, studentCode.Trim());
    public ExamPaper GetSelectedPaper(int sessionId, string studentCode)
    {
        var session = GetOpenSessions().FirstOrDefault(s => s.Id == sessionId)
            ?? throw new InvalidOperationException("Ca thi đã đóng hoặc không còn tồn tại. Hãy tải lại danh sách.");
        if (HasAlreadySubmitted(sessionId, studentCode)) throw new InvalidOperationException("Bạn đã nộp bài ở ca này.");
        return new ExamSessionRepository().FindPapers().FirstOrDefault(p => p.Id == session.PaperId)
            ?? throw new InvalidOperationException("Không tìm thấy đề của ca thi.");
    }
}
