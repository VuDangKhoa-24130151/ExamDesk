using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class SubmissionRepository
{
    // TODO (Bạn 2): Một transaction lưu bài và mọi câu; giữ unique ca/MSSV.
    public int SaveAttemptWithAnswers(ExamAttempt attempt, List<AttemptAnswer> answers)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
