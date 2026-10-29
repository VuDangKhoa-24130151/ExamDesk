using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Khung dành cho Bạn 3. Chưa triển khai nghiệp vụ hoặc SQL.
public sealed class GradingRepository
{
    // TODO (Bạn 3): Một transaction lưu bài và mọi câu; giữ unique ca/MSSV.
    public int SaveAttemptWithAnswers(ExamAttempt attempt, List<AttemptAnswer> answers)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): SELECT có tham số theo lớp/ca.
    public List<ExamAttempt> FindAttempts(int? sessionId = null, string? className = null)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Đọc chi tiết các câu đã nộp.
    public List<AttemptAnswer> FindAnswersByAttemptId(int attemptId)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): UPDATE ghi chú có tham số.
    public void UpdateReviewNote(int attemptId, string note)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): SELECT đề để tạo ca.
    public List<ExamPaper> FindPapers()
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

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
