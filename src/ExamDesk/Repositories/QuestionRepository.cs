using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Khung dành cho Bạn 1. Chưa triển khai nghiệp vụ hoặc SQL.
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

    // TODO (Bạn 1): Rút ngẫu nhiên ID theo độ khó.
    public List<int> PickRandomQuestionIds(int subjectId, string difficulty, int count)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");

    // TODO (Bạn 1): Lưu đề và thứ tự câu trong một transaction.
    public int InsertExamPaper(ExamPaper paper, List<int> questionIds)
        => throw new NotImplementedException("Bạn 1: chưa triển khai.");
}
