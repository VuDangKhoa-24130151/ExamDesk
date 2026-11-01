using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 1 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class QuestionRepository
{
    // TODO (Bạn 1): SELECT có tham số.
    public List<Question> FindQuestions(int? subjectId = null, int? chapter = null)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");

    // TODO (Bạn 1): INSERT/UPDATE có tham số.
    public void SaveQuestion(Question question)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");

    // TODO (Bạn 1): DELETE có tham số và kiểm tra ràng buộc.
    public void DeleteQuestion(int questionId)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");
}
