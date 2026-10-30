using ExamDesk.Models;

namespace ExamDesk.Services;

// Bạn 2 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class GradingReviewService
{
    // TODO (Bạn 2): Tra cứu/lọc dữ liệu cho bảng và thống kê.
    public List<ExamAttempt> GetAttempts(int? sessionId = null, string? className = null)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Đọc lịch sử từng câu để phúc khảo.
    public List<AttemptAnswer> GetAttemptAnswers(int attemptId)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
