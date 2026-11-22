using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

// Bạn 2 phụ trách: Dịch vụ tra cứu danh sách kết quả thi, chi tiết từng câu để phúc khảo và thống kê.
public sealed class GradingReviewService
{
    private readonly GradingRepository _gradingRepository;

    public GradingReviewService()
        : this(new GradingRepository())
    {
    }

    public GradingReviewService(GradingRepository gradingRepository)
    {
        _gradingRepository = gradingRepository ?? throw new ArgumentNullException(nameof(gradingRepository));
    }

    public List<ExamAttempt> GetAttempts(int? sessionId = null, string? className = null)
        => _gradingRepository.FindAttempts(sessionId, className);

    public List<AttemptAnswer> GetAttemptAnswers(int attemptId)
    {
        if (attemptId <= 0)
            throw new ArgumentException("Mã lượt thi không hợp lệ.", nameof(attemptId));

        return _gradingRepository.FindAnswersByAttemptId(attemptId);
    }
}