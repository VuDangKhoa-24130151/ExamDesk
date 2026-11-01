using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 3 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class StudentExamRepository
{
    // TODO (Bạn 3): SELECT các ca mở.
    public List<ExamSession> FindOpenSessions()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Đọc đúng thứ tự đề; không hiển thị đáp án chuẩn trong lúc thi.
    public List<Question> FindQuestionsByPaperId(int paperId)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): SELECT theo ca/MSSV có tham số.
    public bool HasStudentSubmitted(int sessionId, string studentCode)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");
}
