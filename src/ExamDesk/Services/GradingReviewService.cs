using ExamDesk.Models;

namespace ExamDesk.Services;

// Khung dành cho Bạn 3. Chưa triển khai nghiệp vụ hoặc SQL.
public sealed class GradingReviewService
{
    // TODO (Bạn 3): Đọc đề/đáp án chuẩn từ CSDL; chấm và lưu thật, không trả điểm giả.
    public ExamAttempt GradeAndSaveAttempt(ExamSubmission submission)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Tra cứu/lọc dữ liệu cho bảng và thống kê.
    public List<ExamAttempt> GetAttempts(int? sessionId = null, string? className = null)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Đọc lịch sử từng câu để phúc khảo.
    public List<AttemptAnswer> GetAttemptAnswers(int attemptId)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

    // TODO (Bạn 3): Lưu kết luận phúc khảo.
    public void SaveReviewNote(int attemptId, string note)
        => throw new NotImplementedException("Bạn 3: chưa triển khai.");

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
