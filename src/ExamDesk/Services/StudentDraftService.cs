using System.Text.Json;
using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

public sealed class StudentDraftService
{
    private readonly StudentDraftRepository _repository = new();
    public StudentDraft StartOrResume(int sessionId, User student) => _repository.StartOrResume(sessionId, student);
    public StudentDraft Change(StudentDraft draft, int index, int? answer, bool? flag = null)
    {
        if(index<0 || index>=draft.Questions.Count || answer is <0 or >3) throw new ArgumentException("Lựa chọn không hợp lệ.");
        var next=JsonSerializer.Deserialize<StudentDraft>(JsonSerializer.Serialize(draft))!;
        if(flag.HasValue) next.Flags[index]=flag.Value;
        else if(next.Answers[index]!=answer)
        {
            next.Answers[index]=answer;
            next.History.Add(new AnswerChange { QuestionId=next.Questions[index].Id,Choice=answer,AtUtc=DateTimeOffset.UtcNow });
        }
        _repository.Save(next);return next;
    }
    public StudentDraft Lock(StudentDraft draft)
    {
        if(draft.PendingSubmission) return draft;
        var next=JsonSerializer.Deserialize<StudentDraft>(JsonSerializer.Serialize(draft))!;
        next.PendingSubmission=true;next.AutoSubmitted=DateTimeOffset.UtcNow>=draft.DeadlineUtc;
        _repository.Save(next,true);return next;
    }
}
