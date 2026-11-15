using ExamDesk.Models;
using ExamDesk.Repositories;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Services;

/// <summary>
/// Service xử lý nghiệp vụ tạo đề thi ngẫu nhiên, xem đề và bảo toàn đề thi.
/// </summary>
public sealed class ExamPaperService
{
    private readonly ExamPaperRepository _examPaperRepository;
    private readonly QuestionRepository _questionRepository;

    public ExamPaperService()
        : this(new ExamPaperRepository(), new QuestionRepository())
    {
    }

    public ExamPaperService(ExamPaperRepository examPaperRepository, QuestionRepository questionRepository)
    {
        _examPaperRepository = examPaperRepository ?? throw new ArgumentNullException(nameof(examPaperRepository));
        _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
    }

    /// <summary>
    /// Kiểm tra cấu hình, rút ngẫu nhiên câu hỏi không trùng theo từng độ khó và lưu đề thi vào SQLite.
    /// </summary>
    public int GenerateExamPaper(ExamPaper configuration)
    {
        ValidateConfiguration(configuration);

        if (_examPaperRepository.IsPaperCodeExists(configuration.PaperCode))
        {
            throw new InvalidOperationException(
                $"Mã đề '{configuration.PaperCode}' đã tồn tại. Vui lòng nhập mã đề khác.");
        }

        var selectedQuestionIds = PickUniqueRandomQuestions(
            configuration.SubjectId,
            configuration.EasyCount,
            configuration.MediumCount,
            configuration.HardCount);

        try
        {
            return _examPaperRepository.InsertExamPaper(configuration, selectedQuestionIds);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException(
                $"Mã đề '{configuration.PaperCode}' đã tồn tại hoặc dữ liệu đề thi không hợp lệ. Vui lòng đổi mã đề khác và thử lại.",
                ex);
        }
    }

    /// <summary>
    /// Lấy số lượng câu hỏi hiện có theo từng mức độ khó của môn học.
    /// </summary>
    public ExamPaperDifficultyCounts GetAvailableCounts(int subjectId)
    {
        if (subjectId <= 0 || _questionRepository.GetSubjectById(subjectId) is null)
        {
            throw new InvalidOperationException("Môn học không tồn tại trong hệ thống. Vui lòng chọn lại môn học.");
        }

        return _examPaperRepository.GetAvailableCountsByDifficulty(subjectId);
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của cấu hình đề thi trước khi tạo đề.
    /// </summary>
    public void ValidateConfiguration(ExamPaper configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.PaperCode = (configuration.PaperCode ?? string.Empty).Trim().ToUpperInvariant();
        configuration.Title = (configuration.Title ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(configuration.PaperCode))
        {
            throw new ArgumentException("Mã đề thi không được để trống. Vui lòng nhập mã đề (ví dụ: DE01).");
        }

        if (string.IsNullOrWhiteSpace(configuration.Title))
        {
            throw new ArgumentException("Tên đề thi không được để trống. Vui lòng nhập tên đề thi.");
        }

        if (configuration.SubjectId <= 0 || _questionRepository.GetSubjectById(configuration.SubjectId) is null)
        {
            throw new InvalidOperationException("Môn học được chọn không tồn tại. Vui lòng chọn môn học hợp lệ.");
        }

        if (configuration.DurationMinutes < 1)
        {
            throw new ArgumentException("Thời gian làm bài phải từ 1 phút trở lên.");
        }

        if (configuration.EasyCount < 0 || configuration.MediumCount < 0 || configuration.HardCount < 0)
        {
            throw new ArgumentException("Số lượng câu hỏi ở mỗi mức độ khó không được nhỏ hơn 0.");
        }

        if (configuration.TotalQuestions <= 0)
        {
            throw new ArgumentException("Tổng số câu hỏi của đề thi phải lớn hơn 0. Vui lòng chọn ít nhất 1 câu hỏi.");
        }
    }

    /// <summary>
    /// Lấy danh sách tóm tắt đề thi theo môn học và từ khóa tìm kiếm.
    /// </summary>
    public List<ExamPaperSummary> GetPaperSummaries(int? subjectId = null, string? keyword = null)
        => _examPaperRepository.FindPaperSummaries(subjectId, keyword);

    /// <summary>
    /// Lấy thông tin một đề thi theo Id.
    /// </summary>
    public ExamPaper? GetPaperById(int paperId)
        => _examPaperRepository.GetPaperById(paperId);

    /// <summary>
    /// Lấy danh sách chi tiết câu hỏi thuộc một đề thi.
    /// </summary>
    public List<ExamPaperQuestionDetail> GetPaperQuestions(int paperId)
        => _examPaperRepository.GetPaperQuestions(paperId);

    /// <summary>
    /// Rút lại bộ câu hỏi mới cho đề thi khi đề chưa được gắn vào bất kỳ ca thi nào.
    /// </summary>
    public void RegenerateQuestions(int paperId)
    {
        var paper = _examPaperRepository.GetPaperById(paperId)
            ?? throw new InvalidOperationException("Không tìm thấy đề thi cần rút lại câu hỏi.");

        // Bảo toàn đề thi: đề đã gắn vào ca thi hoặc đã có lượt thi thì tuyệt đối không cho rút lại câu.
        if (_examPaperRepository.IsPaperUsedInSession(paperId))
        {
            throw new InvalidOperationException(
                $"Đề thi '{paper.PaperCode}' đã được gán vào ca thi, không thể rút lại câu hỏi để bảo toàn dữ liệu thi.");
        }

        var newQuestionIds = PickUniqueRandomQuestions(
            paper.SubjectId,
            paper.EasyCount,
            paper.MediumCount,
            paper.HardCount);

        _examPaperRepository.ReplacePaperQuestions(paperId, newQuestionIds);
    }

    /// <summary>
    /// Xóa đề thi khi đề chưa được gắn vào bất kỳ ca thi nào.
    /// </summary>
    public void DeletePaper(int paperId)
    {
        var paper = _examPaperRepository.GetPaperById(paperId)
            ?? throw new InvalidOperationException("Không tìm thấy đề thi cần xóa.");

        // Bảo toàn đề thi: chặn xóa nếu đề đã tham gia ca thi hoặc có bài làm của sinh viên.
        if (_examPaperRepository.IsPaperUsedInSession(paperId))
        {
            throw new InvalidOperationException(
                $"Đề thi '{paper.PaperCode}' đã được gán vào ca thi, không thể xóa để bảo toàn dữ liệu thi.");
        }

        _examPaperRepository.DeletePaper(paperId);
    }

    /// <summary>
    /// Dành cho phân hệ Ca thi &amp; Màn thi (Bạn 3): lấy danh sách đề thi đã có đủ số lượng câu hỏi theo cấu hình
    /// (<c>COUNT(*) = EasyCount + MediumCount + HardCount</c> và tổng câu &gt; 0) để sẵn sàng mở ca thi.
    /// </summary>
    public List<ExamPaper> GetReadyPapers()
        => _examPaperRepository.GetReadyPapers();

    /// <summary>
    /// Dành cho phân hệ Màn thi sinh viên (Bạn 3): trả về danh sách câu hỏi theo đúng <c>OrderIndex</c>
    /// dưới dạng <see cref="ExamQuestionView"/> (đảm bảo tuyệt đối không chứa đáp án đúng <c>CorrectOption</c>).
    /// Nếu đề không tồn tại hoặc chưa đủ số câu theo cấu hình sẽ ném <see cref="InvalidOperationException"/>.
    /// </summary>
    public List<ExamQuestionView> GetQuestionsForExam(int paperId)
    {
        if (paperId <= 0)
        {
            throw new InvalidOperationException("Mã định danh đề thi không hợp lệ.");
        }

        var paper = _examPaperRepository.GetPaperById(paperId)
            ?? throw new InvalidOperationException($"Không tìm thấy đề thi có mã định danh {paperId}.");

        var questions = _examPaperRepository.GetQuestionsForExam(paperId);
        if (paper.TotalQuestions <= 0 || questions.Count != paper.TotalQuestions)
        {
            throw new InvalidOperationException(
                $"Đề thi '{paper.PaperCode}' chưa đủ số lượng câu hỏi theo cấu hình ({questions.Count}/{paper.TotalQuestions} câu). Vui lòng kiểm tra lại đề thi.");
        }

        return questions;
    }

    /// <summary>
    /// Dành cho phân hệ Chấm điểm (<c>SubmissionService</c> - Bạn 2): trả về danh sách đáp án chuẩn
    /// (<see cref="AnswerKeyItem"/> gồm PaperId, OrderIndex, QuestionId, CorrectOption) theo đúng <c>OrderIndex</c>.
    /// </summary>
    public List<AnswerKeyItem> GetAnswerKey(int paperId)
    {
        if (paperId <= 0)
        {
            throw new InvalidOperationException("Mã định danh đề thi không hợp lệ.");
        }

        var paper = _examPaperRepository.GetPaperById(paperId)
            ?? throw new InvalidOperationException($"Không tìm thấy đề thi có mã định danh {paperId} để lấy bảng đáp án.");

        var answerKey = _examPaperRepository.GetAnswerKey(paperId);
        if (paper.TotalQuestions <= 0 || answerKey.Count != paper.TotalQuestions)
        {
            throw new InvalidOperationException(
                $"Đề thi '{paper.PaperCode}' chưa đủ câu hỏi ({answerKey.Count}/{paper.TotalQuestions}), không thể lấy bảng đáp án chấm điểm.");
        }

        return answerKey;
    }

    private List<int> PickUniqueRandomQuestions(int subjectId, int easyCount, int mediumCount, int hardCount)
    {
        var available = _examPaperRepository.GetAvailableCountsByDifficulty(subjectId);
        var shortages = new List<string>();

        if (available.EasyCount < easyCount)
        {
            shortages.Add($"Dễ: cần {easyCount}, hiện có {available.EasyCount}");
        }

        if (available.MediumCount < mediumCount)
        {
            shortages.Add($"Trung bình: cần {mediumCount}, hiện có {available.MediumCount}");
        }

        if (available.HardCount < hardCount)
        {
            shortages.Add($"Khó: cần {hardCount}, hiện có {available.HardCount}");
        }

        if (shortages.Count > 0)
        {
            throw new InvalidOperationException(
                $"Ngân hàng câu hỏi không đủ số lượng để tạo đề ({string.Join("; ", shortages)}). Vui lòng bổ sung thêm câu hỏi hoặc giảm số câu yêu cầu.");
        }

        // Rút ngẫu nhiên độc lập từng mức độ khó (Easy, Medium, Hard), do tập câu hỏi ở mỗi mức rời nhau nên không thể trùng.
        var easyIds = _examPaperRepository.PickRandomQuestionIds(subjectId, "Easy", easyCount);
        var mediumIds = _examPaperRepository.PickRandomQuestionIds(subjectId, "Medium", mediumCount);
        var hardIds = _examPaperRepository.PickRandomQuestionIds(subjectId, "Hard", hardCount);

        if (easyIds.Count < easyCount || mediumIds.Count < mediumCount || hardIds.Count < hardCount)
        {
            throw new InvalidOperationException(
                "Số lượng câu hỏi trong ngân hàng vừa thay đổi và không còn đủ để tạo đề. Vui lòng kiểm tra lại ngân hàng câu hỏi.");
        }

        var combined = new int[easyIds.Count + mediumIds.Count + hardIds.Count];
        easyIds.CopyTo(combined, 0);
        mediumIds.CopyTo(combined, easyIds.Count);
        hardIds.CopyTo(combined, easyIds.Count + mediumIds.Count);

        // Trộn ngẫu nhiên thứ tự toàn bộ câu hỏi đã rút trước khi gán OrderIndex 1..N.
        Random.Shared.Shuffle(combined);

        return combined.Distinct().Count() == combined.Length
            ? [.. combined]
            : throw new InvalidOperationException("Phát hiện câu hỏi bị trùng khi rút đề. Vui lòng thử lại.");
    }
}
