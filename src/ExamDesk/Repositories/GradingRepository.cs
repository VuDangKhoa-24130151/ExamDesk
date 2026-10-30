using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class GradingRepository
{
    // TODO (Bạn 2): SELECT có tham số theo lớp/ca.
    public List<ExamAttempt> FindAttempts(int? sessionId = null, string? className = null)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Đọc chi tiết các câu đã nộp.
    public List<AttemptAnswer> FindAnswersByAttemptId(int attemptId)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
