using ExamDesk.Models;

namespace ExamDesk.Services;

// Bạn 3 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class StudentExamService
{
    // TODO (Bạn 3): Đọc các ca mở qua Repository.
    public List<ExamSession> GetOpenSessions()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Kiểm tra đề đủ câu trước khi cho bắt đầu.
    public List<Question> GetQuestionsForPaper(int paperId)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Kiểm tra điều kiện thi; không thay thế unique index khi lưu.
    public bool HasAlreadySubmitted(int sessionId, string studentCode)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");
}
