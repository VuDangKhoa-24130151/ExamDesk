using ExamDesk.Models;

namespace ExamDesk.Services;

// Bạn 1 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class QuestionBankService
{
    // TODO (Bạn 1): Truy vấn qua QuestionRepository.
    public List<Question> GetQuestions(int? subjectId = null, int? chapter = null)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");

    // TODO (Bạn 1): Kiểm tra dữ liệu/quyền; khóa sửa câu đã gắn đề.
    public void SaveQuestion(Question question)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");

    // TODO (Bạn 1): Từ chối xóa câu đã gắn đề.
    public void DeleteQuestion(int questionId)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");
}
