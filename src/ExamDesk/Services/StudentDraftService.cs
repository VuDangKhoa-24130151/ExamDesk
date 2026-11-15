using System.Text.Json;
using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentDraftService
{
    private readonly StudentDraftRepository _repository = new();
    private readonly ISubmissionGrader _grader;

    public StudentDraftService() : this(new StudentExamTemporaryGrader()) { }
    internal StudentDraftService(ISubmissionGrader grader) => _grader = grader;
    public StudentDraft StartOrResume(int sessionId, User student) => _repository.StartOrResume(sessionId, student);
    internal StudentDraft? FindDraft(int sessionId, string studentCode) => _repository.FindDraft(sessionId, studentCode);

    // Giữ thứ tự ghi nhận. Choice từ 0 đến 3 là A–D, null là bỏ lựa chọn.
    public IReadOnlyList<AnswerChange> GetAnswerHistory(int sessionId, string studentCode)
        => FindDraft(sessionId, studentCode.Trim())?.History is { } history ? history.AsReadOnly() : [];

    internal StudentDraft Change(StudentDraft draft, int index, int? answer, bool? flag, Func<DateTimeOffset> readUtc)
    {
        if (index < 0 || index >= draft.Questions.Count || answer is < 0 or > 3)
            throw new ArgumentException("Lựa chọn không hợp lệ.");

        // Chỉ thay bản đang hiển thị sau khi bản sao đã lưu thành công.
        var next = JsonSerializer.Deserialize<StudentDraft>(JsonSerializer.Serialize(draft))!;
        next.Revision = draft.Revision;
        if (flag.HasValue)
        {
            next.Flags[index] = flag.Value;
        }
        else if (next.Answers[index] != answer)
        {
            next.Answers[index] = answer;
            next.History.Add(new AnswerChange
            {
                QuestionId = next.Questions[index].Id,
                Choice = answer,
                AtUtc = readUtc()
            });
        }

        _repository.Save(next, readUtc);
        return next;
    }

    internal StudentDraft LockForSubmission(StudentDraft draft, Func<DateTimeOffset> readUtc)
        => _repository.LockForSubmission(draft, readUtc);
    public void SendForGrading(StudentDraft lockedDraft) => _grader.GradeAndSave(lockedDraft);
}
