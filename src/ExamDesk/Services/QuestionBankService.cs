using ExamDesk.Models;

namespace ExamDesk.Services;

// Khung dành cho Bạn 1. Chưa triển khai nghiệp vụ hoặc SQL.
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

    // TODO (Bạn 1): Rút đủ số câu từng mức, không trùng; thiếu câu thì không lưu.
    public int GenerateExamPaper(ExamPaper configuration)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");
}
