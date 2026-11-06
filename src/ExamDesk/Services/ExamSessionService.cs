using ExamDesk.Models;

namespace ExamDesk.Services;

// Bạn 3 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class ExamSessionService
{
    // TODO (Bạn 3): Lấy đề để tạo ca.
    public List<ExamPaper> GetExamPapers()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Lấy các ca cho màn hình quản lý.
    public List<ExamSession> GetSessions()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Kiểm tra ca và lưu qua Repository.
    public int CreateSession(ExamSession session)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Mở/đóng ca qua Repository.
    public void SetSessionOpen(int sessionId, bool isOpen)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");
}
