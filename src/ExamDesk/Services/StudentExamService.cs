using ExamDesk.Models;

namespace ExamDesk.Services;

// Khung dành cho Bạn 2. Chưa triển khai nghiệp vụ hoặc SQL.
public sealed class StudentExamService
{
    // TODO (Bạn 2): Đọc các ca mở qua Repository.
    public List<ExamSession> GetOpenSessions()
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Kiểm tra đề đủ câu trước khi cho bắt đầu.
    public List<Question> GetQuestionsForPaper(int paperId)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Kiểm tra điều kiện thi; không thay thế unique index khi lưu.
    public bool HasAlreadySubmitted(int sessionId, string studentCode)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
