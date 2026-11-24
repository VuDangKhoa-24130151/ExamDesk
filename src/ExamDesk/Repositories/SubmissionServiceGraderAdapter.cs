using ExamDesk.Models;
using ExamDesk.Services;

namespace ExamDesk.Repositories;

// Adapter nối luồng nộp bài của Bạn 3 (ISubmissionGrader) vào SubmissionService của Bạn 2.
internal sealed class SubmissionServiceGraderAdapter : ISubmissionGrader
{
    private readonly SubmissionService _submissionService;

    public SubmissionServiceGraderAdapter()
        : this(new SubmissionService())
    {
    }

    public SubmissionServiceGraderAdapter(SubmissionService submissionService)
    {
        _submissionService = submissionService ?? throw new ArgumentNullException(nameof(submissionService));
    }

    public void GradeAndSave(StudentDraft saved)
        => _submissionService.GradeAndSaveAttempt(saved);
}
