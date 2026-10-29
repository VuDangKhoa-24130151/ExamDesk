using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Khung dành cho Bạn 2. Chưa triển khai nghiệp vụ hoặc SQL.
public sealed class ExamSessionRepository
{
    // TODO (Bạn 2): SELECT các ca mở.
    public List<ExamSession> FindOpenSessions()
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Đọc đúng thứ tự đề; không hiển thị đáp án chuẩn trong lúc thi.
    public List<Question> FindQuestionsByPaperId(int paperId)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): SELECT theo ca/MSSV có tham số.
    public bool HasStudentSubmitted(int sessionId, string studentCode)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
