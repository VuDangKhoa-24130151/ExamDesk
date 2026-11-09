using System.Text.RegularExpressions;
using ExamDesk.Models;
using ExamDesk.Repositories;

namespace ExamDesk.Services;

// Bạn 1 phụ trách: Dịch vụ nghiệp vụ quản lý môn học, chương và ngân hàng câu hỏi.
public sealed class QuestionBankService
{
    private static readonly Regex ValidCodeRegex = new("^[A-Z0-9]{2,10}$", RegexOptions.Compiled);
    private readonly QuestionRepository _questionRepository;

    public QuestionBankService() : this(new QuestionRepository())
    {
    }

    public QuestionBankService(QuestionRepository questionRepository)
    {
        _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
    }

    #region Subject Business Logic

    public List<Subject> GetSubjects() => _questionRepository.GetSubjects();

    public Subject? GetSubjectById(int id) => _questionRepository.GetSubjectById(id);

    public bool IsSubjectCodeExists(string code, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return _questionRepository.IsSubjectCodeExists(code.Trim(), excludeId);
    }

    public void ValidateSubject(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        var code = (subject.Code ?? string.Empty).Trim().ToUpperInvariant();
        var name = (subject.Name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Vui lòng nhập mã môn học.");
        }

        if (!ValidCodeRegex.IsMatch(code))
        {
            throw new ArgumentException("Mã môn chỉ gồm chữ in hoa không dấu (A–Z) hoặc số (0–9), độ dài 2–10 ký tự.");
        }

        if (_questionRepository.IsSubjectCodeExists(code, subject.Id))
        {
            throw new InvalidOperationException($"Mã môn \"{code}\" đã tồn tại. Vui lòng nhập mã khác.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Vui lòng nhập tên môn học.");
        }

        if (name.Length > 100)
        {
            throw new ArgumentException($"Tên môn không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).");
        }
    }

    public void SaveSubject(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        subject.Code = (subject.Code ?? string.Empty).Trim().ToUpperInvariant();
        subject.Name = (subject.Name ?? string.Empty).Trim();

        ValidateSubject(subject);

        if (subject.Id <= 0)
        {
            var newId = _questionRepository.AddSubject(subject);
            subject.Id = newId;
        }
        else
        {
            _questionRepository.UpdateSubject(subject);
        }
    }

    public void DeleteSubject(int subjectId)
    {
        var subject = _questionRepository.GetSubjectById(subjectId)
            ?? throw new InvalidOperationException("Không tìm thấy môn học để xóa.");

        var questionCount = _questionRepository.CountQuestionsBySubject(subjectId);
        if (questionCount > 0)
        {
            throw new InvalidOperationException($"Môn đang có {questionCount} câu hỏi, vui lòng xóa câu hỏi trước.");
        }

        var examPaperCount = _questionRepository.CountExamPapersBySubject(subjectId);
        if (examPaperCount > 0)
        {
            throw new InvalidOperationException($"Môn học đang được sử dụng trong {examPaperCount} đề thi, vui lòng xóa đề thi trước.");
        }

        var chapters = _questionRepository.GetChaptersBySubject(subjectId);
        if (chapters.Count > 0)
        {
            throw new InvalidOperationException($"Không thể xóa môn \"{subject.Code} – {subject.Name}\" vì đang có {chapters.Count} chương. Vui lòng xóa các chương trước khi xóa môn.");
        }

        _questionRepository.DeleteSubject(subjectId);
    }

    #endregion

    #region Chapter Business Logic

    public List<ChapterInfo> GetChaptersBySubject(int subjectId)
        => _questionRepository.GetChaptersBySubject(subjectId);

    public void SaveChapter(ChapterInfo chapter, int? originalNumber = null)
    {
        ArgumentNullException.ThrowIfNull(chapter);

        var name = (chapter.Name ?? string.Empty).Trim();

        if (chapter.Number < 1 || chapter.Number > 50)
        {
            throw new ArgumentException("Chương số phải nằm trong khoảng từ 1 đến 50.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Vui lòng nhập tên chương.");
        }

        if (name.Length > 100)
        {
            throw new ArgumentException($"Tên chương không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).");
        }

        var existingChapters = _questionRepository.GetChaptersBySubject(chapter.SubjectId);
        if (existingChapters.Any(c => c.Number == chapter.Number && c.Number != originalNumber))
        {
            throw new InvalidOperationException($"Chương số {chapter.Number} đã tồn tại trong môn học này. Vui lòng chọn số khác.");
        }

        chapter.Name = name;
        _questionRepository.SaveChapter(chapter, originalNumber);
    }

    public void DeleteChapter(int subjectId, int chapterNumber)
    {
        var questionCount = _questionRepository.CountQuestionsByChapter(subjectId, chapterNumber);
        if (questionCount > 0)
        {
            throw new InvalidOperationException($"Không thể xóa Chương {chapterNumber} vì đang có {questionCount} câu hỏi. Vui lòng xóa hoặc chuyển câu hỏi trước.");
        }

        _questionRepository.DeleteChapter(subjectId, chapterNumber);
    }

    #endregion

    #region Question Business Logic

    public List<Question> GetQuestions(int? subjectId = null, int? chapter = null, string? difficulty = null, string? keyword = null)
        => _questionRepository.FindQuestions(subjectId, chapter, difficulty, keyword);

    public Question? GetQuestionById(int id)
        => _questionRepository.GetQuestionById(id);

    public int CountAllQuestions()
        => _questionRepository.CountAllQuestions();

    public List<int> GetDistinctChapters(int? subjectId = null)
        => _questionRepository.GetDistinctChapters(subjectId);

    public void ValidateQuestion(Question q)
    {
        ArgumentNullException.ThrowIfNull(q);

        if (q.SubjectId <= 0 || _questionRepository.GetSubjectById(q.SubjectId) is null)
        {
            throw new ArgumentException("Môn học không hợp lệ hoặc không tồn tại.");
        }

        if (q.Chapter <= 0)
        {
            throw new ArgumentException("Chương phải là số nguyên dương lớn hơn 0.");
        }

        if (q.Chapter > 50)
        {
            throw new ArgumentException("Chương số không được vượt quá 50.");
        }

        var diff = q.Difficulty?.Trim() ?? string.Empty;
        if (!string.Equals(diff, "Easy", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(diff, "Medium", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(diff, "Hard", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Độ khó phải là một trong 3 mức: Dễ (Easy), Trung bình (Medium), Khó (Hard).");
        }
        q.Difficulty = char.ToUpperInvariant(diff[0]) + diff[1..].ToLowerInvariant();

        var content = q.Content?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Nội dung câu hỏi không được để trống.");
        }
        q.Content = content;

        var optA = q.OptionA?.Trim() ?? string.Empty;
        var optB = q.OptionB?.Trim() ?? string.Empty;
        var optC = q.OptionC?.Trim() ?? string.Empty;
        var optD = q.OptionD?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(optA)) throw new ArgumentException("Đáp án A không được để trống.");
        if (string.IsNullOrWhiteSpace(optB)) throw new ArgumentException("Đáp án B không được để trống.");
        if (string.IsNullOrWhiteSpace(optC)) throw new ArgumentException("Đáp án C không được để trống.");
        if (string.IsNullOrWhiteSpace(optD)) throw new ArgumentException("Đáp án D không được để trống.");

        if (string.Equals(optA, optB, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án B không được trùng với đáp án A.");
        if (string.Equals(optA, optC, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án C không được trùng với đáp án A.");
        if (string.Equals(optA, optD, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án D không được trùng với đáp án A.");
        if (string.Equals(optB, optC, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án C không được trùng với đáp án B.");
        if (string.Equals(optB, optD, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án D không được trùng với đáp án B.");
        if (string.Equals(optC, optD, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Đáp án D không được trùng với đáp án C.");

        q.OptionA = optA;
        q.OptionB = optB;
        q.OptionC = optC;
        q.OptionD = optD;

        var correct = q.CorrectOption?.Trim().ToUpperInvariant() ?? string.Empty;
        if (correct is not ("A" or "B" or "C" or "D"))
        {
            throw new ArgumentException("Đáp án đúng phải là một trong các phương án: A, B, C hoặc D.");
        }
        q.CorrectOption = correct;
    }

    public void SaveQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        ValidateQuestion(question);

        if (question.Id <= 0)
        {
            var newId = _questionRepository.InsertQuestion(question);
            question.Id = newId;
        }
        else
        {
            _questionRepository.UpdateQuestion(question);
        }
    }

    public void DeleteQuestion(int questionId)
    {
        _questionRepository.DeleteQuestion(questionId);
    }

    #endregion
}
