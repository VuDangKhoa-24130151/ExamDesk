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

    // Nối với luồng nộp bài sinh viên qua giao diện ISubmissionGrader
    public void GradeAndSave(StudentDraft lockedDraft)
    {
        ArgumentNullException.ThrowIfNull(lockedDraft);

        if (lockedDraft.Questions.Count == 0 || lockedDraft.Questions.Count != lockedDraft.Answers.Length)
            throw new InvalidOperationException("Đề thi hoặc danh sách câu trả lời trong bản nháp không hợp lệ.");

        var selectedAnswers = new Dictionary<int, string>(lockedDraft.Questions.Count);
        for (int i = 0; i < lockedDraft.Questions.Count; i++)
        {
            int? choice = lockedDraft.Answers[i];
            if (choice is < 0 or > 3)
                throw new InvalidOperationException("Đáp án đã lưu không hợp lệ.");

            string option = choice.HasValue ? ((char)('A' + choice.Value)).ToString() : string.Empty;
            selectedAnswers[lockedDraft.Questions[i].Id] = option;
        }

        var submittedAt = lockedDraft.LockedAtUtc
            ?? (lockedDraft.AutoSubmitted ? lockedDraft.DeadlineUtc : DateTimeOffset.UtcNow);
        if (lockedDraft.AutoSubmitted && submittedAt > lockedDraft.DeadlineUtc)
            submittedAt = lockedDraft.DeadlineUtc;

        var submission = new ExamSubmission
        {
            SessionId = lockedDraft.SessionId,
            PaperId = lockedDraft.PaperId,
            StudentCode = lockedDraft.StudentCode,
            StudentName = lockedDraft.StudentName,
            ClassName = lockedDraft.ClassName,
            StartedAt = lockedDraft.StartedUtc,
            DeadlineUtc = lockedDraft.DeadlineUtc,
            SelectedAnswers = selectedAnswers,
            IsAutoSubmitted = lockedDraft.AutoSubmitted
        };

        GradeAndSaveAttemptCore(submission, submittedAt);
    }

    public ExamAttempt GradeAndSaveAttempt(ExamSubmission submission)
        => GradeAndSaveAttemptCore(submission, submittedAtOverride: null);

    private ExamAttempt GradeAndSaveAttemptCore(ExamSubmission submission, DateTimeOffset? submittedAtOverride)
    {
        ArgumentNullException.ThrowIfNull(submission);

        string studentCode = (submission.StudentCode ?? string.Empty).Trim();
        if (submission.SessionId <= 0 || submission.PaperId <= 0 || string.IsNullOrWhiteSpace(studentCode))
            throw new ArgumentException("Thông tin ca thi, đề thi hoặc mã sinh viên không hợp lệ.");

        // Nếu bài đã nộp trước đó thì trả về kết quả đã lưu, không chấm/lưu lần hai
        var existing = _submissionRepository.FindExistingAttempt(submission.SessionId, studentCode);
        if (existing is not null)
            return existing;

        // Lấy bảng đáp án chuẩn từ CSDL (Bạn 1 cung cấp qua ExamPaperService)
        var answerKey = _examPaperService.GetAnswerKey(submission.PaperId);
        if (answerKey.Count == 0)
            throw new InvalidOperationException("Không tìm thấy đáp án chuẩn của đề thi để chấm điểm.");

        var attemptAnswers = new List<AttemptAnswer>(answerKey.Count);
        int correctCount = 0;

        foreach (var keyItem in answerKey.OrderBy(k => k.OrderIndex))
        {
            string selected = string.Empty;
            if (submission.SelectedAnswers is not null)
            {
                if (submission.SelectedAnswers.TryGetValue(keyItem.QuestionId, out var byQuestionId) && !string.IsNullOrWhiteSpace(byQuestionId))
                    selected = NormalizeOption(byQuestionId);
                else if (submission.SelectedAnswers.TryGetValue(keyItem.OrderIndex, out var byOrder) && !string.IsNullOrWhiteSpace(byOrder))
                    selected = NormalizeOption(byOrder);
            }

            string correct = NormalizeOption(keyItem.CorrectOption);
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
        // Điểm = số câu đúng / tổng số câu × 10, làm tròn 2 chữ số thập phân
        double score = Math.Round(correctCount * 10.0 / totalQuestions, 2, MidpointRounding.AwayFromZero);

        var attempt = new ExamAttempt
        {
            SessionId = submission.SessionId,
            PaperId = submission.PaperId,
            StudentCode = studentCode,
            StudentName = (submission.StudentName ?? string.Empty).Trim(),
            ClassName = (submission.ClassName ?? string.Empty).Trim(),
            StartedAt = submission.StartedAt == default ? DateTimeOffset.UtcNow : submission.StartedAt,
            SubmittedAt = submittedAtOverride ?? DateTimeOffset.UtcNow,
            TotalQuestions = totalQuestions,
            CorrectCount = correctCount,
            Score = score,
            IsAutoSubmitted = submission.IsAutoSubmitted,
            ReviewNote = string.Empty
        };

        _submissionRepository.SaveAttemptWithAnswers(attempt, attemptAnswers);
        return _submissionRepository.FindExistingAttempt(submission.SessionId, studentCode) ?? attempt;
    }

    private static string NormalizeOption(string? option)
    {
        if (string.IsNullOrWhiteSpace(option))
            return string.Empty;

        string upper = option.Trim().ToUpperInvariant();
        return upper is "A" or "B" or "C" or "D" ? upper : string.Empty;
    }
}