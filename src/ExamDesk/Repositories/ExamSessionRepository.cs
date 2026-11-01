using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class ExamSessionRepository
{
    // TODO (Bạn 2): SELECT ca thi.
    public List<ExamSession> FindSessions()
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): INSERT ca thi có tham số.
    public int InsertSession(ExamSession session)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): UPDATE trạng thái ca.
    public void SetSessionOpen(int sessionId, bool isOpen)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
