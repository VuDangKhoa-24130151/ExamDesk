using System.Globalization;
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

    public int GetTotalAttemptCount()
        => _gradingRepository.CountAllAttempts();

    public List<AttemptSummaryItem> GetAttemptSummaries(int? sessionId = null, string? className = null, string? keyword = null)
        => _gradingRepository.FindAttemptSummaries(sessionId, className, keyword);

    public List<AttemptAnswer> GetAttemptAnswers(int attemptId)
    {
        if (attemptId <= 0)
            throw new ArgumentException("Mã lượt thi không hợp lệ.", nameof(attemptId));

        return _gradingRepository.FindAnswersByAttemptId(attemptId);
    }

    public List<AttemptReviewDetailItem> GetAttemptReviewDetails(int attemptId)
    {
        if (attemptId <= 0)
            throw new ArgumentException("Mã lượt thi không hợp lệ.", nameof(attemptId));

        return _gradingRepository.FindAttemptReviewDetails(attemptId);
    }

    public ScoreDistributionSummary CalculateScoreDistribution(
        IReadOnlyList<AttemptSummaryItem> attempts,
        string filterSummary)
    {
        ArgumentNullException.ThrowIfNull(attempts);

        int total = attempts.Count;
        double avg = total == 0 ? 0 : Math.Round(attempts.Average(a => a.Score), 2, MidpointRounding.AwayFromZero);
        double max = total == 0 ? 0 : attempts.Max(a => a.Score);
        double min = total == 0 ? 0 : attempts.Min(a => a.Score);
        int passCount = attempts.Count(a => a.Score >= 5.0);
        double passRate = total == 0 ? 0 : Math.Round(passCount * 100.0 / total, 1, MidpointRounding.AwayFromZero);

        // Chia 4 khoảng điểm khớp đúng điều kiện lọc (dùng < 6.5 và < 8.0 để không sót điểm lẻ như 6.45)
        (string Label, int Count)[] groups =
        [
            ("Dưới 5.0 (Chưa đạt)", attempts.Count(a => a.Score < 5.0)),
            ("5.0 – < 6.5 (Trung bình)", attempts.Count(a => a.Score >= 5.0 && a.Score < 6.5)),
            ("6.5 – < 8.0 (Khá)", attempts.Count(a => a.Score >= 6.5 && a.Score < 8.0)),
            ("8.0 – 10.0 (Giỏi)", attempts.Count(a => a.Score >= 8.0))
        ];

        var buckets = groups.Select(g =>
        {
            double pct = total == 0 ? 0 : Math.Round(g.Count * 100.0 / total, 1, MidpointRounding.AwayFromZero);
            // Chuẩn hóa thanh biểu đồ về tối đa 20 ký tự '█' (mỗi 5% = 1 ô) để không bị tràn cột khi lớp đông
            int barLength = g.Count == 0 ? 0 : Math.Max(1, (int)Math.Round(pct / 5.0, MidpointRounding.AwayFromZero));
            return new ScoreBucketItem
            {
                Label = g.Label,
                Count = g.Count,
                Percentage = pct,
                VisualBar = new string('█', barLength)
            };
        }).ToList();

        return new ScoreDistributionSummary
        {
            FilterSummary = filterSummary,
            TotalCount = total,
            AverageScore = avg,
            MaxScore = max,
            MinScore = min,
            PassCount = passCount,
            PassRatePercent = passRate,
            Buckets = buckets
        };
    }
}

public sealed class AttemptSummaryItem
{
    public int AttemptId { get; set; }
    public int SessionId { get; set; }
    public int PaperId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SessionName { get; set; } = string.Empty;
    public string PaperTitle { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public int BlankCount { get; set; }
    public int WrongCount => Math.Max(0, TotalQuestions - CorrectCount - BlankCount);
    public double Score { get; set; }
    public bool IsAutoSubmitted { get; set; }
    public bool IsViolationSubmitted { get; set; }
    public string SubmitMethodText => !IsAutoSubmitted ? "Nộp bài"
        : IsViolationSubmitted ? "Vi phạm (3 lần)"
        : "Hết giờ";
    public DateTimeOffset SubmittedAt { get; set; }
    public string SubmittedAtText => SubmittedAt.ToLocalTime().ToString("HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);
}

public sealed class AttemptReviewDetailItem
{
    public int QuestionOrder { get; set; }
    public int QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string SelectedOption { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string ResultStatus => string.IsNullOrWhiteSpace(SelectedOption) ? "Bỏ trống"
                                : IsCorrect ? "Đúng" : "Sai";
    public string OptionsFormatted => $"A. {OptionA}\nB. {OptionB}\nC. {OptionC}\nD. {OptionD}";
}

public sealed class ScoreBucketItem
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public string PercentageText => $"{Percentage:0.0}%";
    public string VisualBar { get; set; } = string.Empty;
}

public sealed class ScoreDistributionSummary
{
    public string FilterSummary { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public double AverageScore { get; set; }
    public double MaxScore { get; set; }
    public double MinScore { get; set; }
    public int PassCount { get; set; }
    public double PassRatePercent { get; set; }
    public IReadOnlyList<ScoreBucketItem> Buckets { get; set; } = [];
}
