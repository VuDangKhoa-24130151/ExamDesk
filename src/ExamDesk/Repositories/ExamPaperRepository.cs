using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách. Stub cho giai đoạn nghiệp vụ; UI hiện tại chưa gọi các hàm này.
public sealed class ExamPaperRepository
{
    // TODO (Bạn 2): Rút ngẫu nhiên ID theo độ khó.
    public List<int> PickRandomQuestionIds(int subjectId, string difficulty, int count)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): Lưu đề và thứ tự câu trong một transaction.
    public int InsertExamPaper(ExamPaper paper, List<int> questionIds)
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");

    // TODO (Bạn 2): SELECT đề để tạo ca.
    public List<ExamPaper> FindPapers()
        => throw new NotImplementedException("Bạn 2: chưa triển khai.");
}
