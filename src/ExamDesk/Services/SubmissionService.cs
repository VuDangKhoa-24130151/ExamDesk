using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

// Bạn 2 phụ trách: Nhận bài nộp, đối chiếu đáp án chuẩn từ CSDL, chấm điểm và lưu kết quả.
public sealed class SubmissionService : ISubmissionGrader
{
    private readonly SubmissionRepository _submissionRepository;
    private readonly ExamPaperService _examPaperService;

    public SubmissionService()
        : this(new SubmissionRepository(), new ExamPaperService())
    {
    }

    public SubmissionService(SubmissionRepository submissionRepository, ExamPaperService examPaperService)
    {
        _submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
        _examPaperService = examPaperService ?? throw new ArgumentNullException(nameof(examPaperService));
    }

    public void GradeAndSave(StudentDraft lockedDraft)
        => GradeAndSaveAttempt(lockedDraft);

    public ExamAttempt GradeAndSaveAttempt(StudentDraft lockedDraft)
    {
        ArgumentNullException.ThrowIfNull(lockedDraft);
        return _submissionRepository.GradeAndSaveLockedDraft(
            lockedDraft,
            paperId => _examPaperService.GetAnswerKey(paperId));
    }

    public ExamAttempt GradeAndSaveAttempt(ExamSubmission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        string studentCode = (submission.StudentCode ?? string.Empty).Trim();
        if (submission.SessionId <= 0 || submission.PaperId <= 0 || string.IsNullOrWhiteSpace(studentCode))
            throw new ArgumentException("Thông tin ca thi, đề thi hoặc mã sinh viên không hợp lệ.");

        // Chống nộp lặp: nếu đã có kết quả của sinh viên ở ca này thì trả về bản cũ luôn.
        var existing = _submissionRepository.FindExistingAttempt(submission.SessionId, studentCode);
        if (existing is not null)
            return existing;

        if (!_submissionRepository.IsSessionMatchingPaper(submission.SessionId, submission.PaperId))
            throw new InvalidOperationException("Đề thi không khớp với ca thi hoặc ca thi không tồn tại.");

        // Lấy bảng đáp án chuẩn từ ExamPaperService (không tin dữ liệu đáp án từ UI).
        var answerKey = _examPaperService.GetAnswerKey(submission.PaperId);
        if (answerKey.Count == 0)
            throw new InvalidOperationException("Không tìm thấy đáp án chuẩn của đề thi để chấm điểm.");

        var attemptAnswers = new List<AttemptAnswer>(answerKey.Count);
        int correctCount = 0;

        foreach (var keyItem in answerKey.OrderBy(k => k.OrderIndex))
        {
            string selected = string.Empty;
            // Chỉ tra cứu theo QuestionId, không fallback theo OrderIndex để tránh lệch câu khi trộn đề.
            if (submission.SelectedAnswers is not null &&
                submission.SelectedAnswers.TryGetValue(keyItem.QuestionId, out var byQuestionId) &&
                !string.IsNullOrWhiteSpace(byQuestionId))
            {
                selected = SubmissionRepository.NormalizeOption(byQuestionId);
            }

            string correct = SubmissionRepository.NormalizeOption(keyItem.CorrectOption);
            bool isCorrect = selected.Length > 0 && selected == correct;
            if (isCorrect)
                correctCount++;

            attemptAnswers.Add(new AttemptAnswer
            {
                QuestionId = keyItem.QuestionId,
                QuestionOrder = keyItem.OrderIndex,
                SelectedOption = selected,
                CorrectOption = correct,
                IsCorrect = isCorrect
            });
        }

        int totalQuestions = answerKey.Count;
        double score = Math.Round(correctCount * 10.0 / totalQuestions, 2, MidpointRounding.AwayFromZero);

        var submittedAt = DateTimeOffset.UtcNow;
        if (submission.IsAutoSubmitted && submission.DeadlineUtc != default && submittedAt > submission.DeadlineUtc)
            submittedAt = submission.DeadlineUtc;

        var attempt = new ExamAttempt
        {
            SessionId = submission.SessionId,
            PaperId = submission.PaperId,
            StudentCode = studentCode,
            StudentName = (submission.StudentName ?? string.Empty).Trim(),
            ClassName = (submission.ClassName ?? string.Empty).Trim(),
            StartedAt = submission.StartedAt == default ? submittedAt : submission.StartedAt,
            SubmittedAt = submittedAt,
            TotalQuestions = totalQuestions,
            CorrectCount = correctCount,
            Score = score,
            IsAutoSubmitted = submission.IsAutoSubmitted,
            ReviewNote = string.Empty
        };

        _submissionRepository.SaveAttemptWithAnswers(attempt, attemptAnswers);
        return _submissionRepository.FindExistingAttempt(submission.SessionId, studentCode) ?? attempt;
    }
}
