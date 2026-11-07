using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 3 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class ExamSessionRepository
{
    // TODO (Bạn 3): SELECT ca thi.
    public List<ExamSession> FindSessions()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): INSERT ca thi có tham số.
    public int InsertSession(ExamSession session)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): UPDATE trạng thái ca.
    public void SetSessionOpen(int sessionId, bool isOpen)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");
}
