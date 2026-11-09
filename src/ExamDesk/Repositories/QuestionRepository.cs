using System.Text.Json;
using ExamDesk.Data;
using ExamDesk.Models;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Repositories;

// Bạn 1 phụ trách: Quản lý môn học, chương và câu hỏi qua SQLite.
public sealed class QuestionRepository
{
    private static readonly object MetadataLock = new();

    private sealed class ChapterMetadataEntry
    {
        public int SubjectId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private static string MetadataFilePath => Path.Combine(
        Path.GetDirectoryName(SqliteContext.DatabaseFilePath)!,
        "chapter_metadata.json");

    #region Subject Operations

    public List<Subject> GetSubjects()
    {
        var list = new List<Subject>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.Id, s.Code, s.Name,
                   COUNT(DISTINCT q.Chapter) AS DbChapterCount,
                   COUNT(q.Id) AS DbQuestionCount
            FROM Subjects s
            LEFT JOIN Questions q ON s.Id = q.SubjectId
            GROUP BY s.Id, s.Code, s.Name
            ORDER BY s.Code;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var code = reader.GetString(1);
            var name = reader.GetString(2);
            var dbChapterCount = reader.GetInt32(3);
            var dbQuestionCount = reader.GetInt32(4);

            var metaChapters = GetMetadataChapters(id);
            var chapterCount = Math.Max(dbChapterCount, metaChapters.Count);

            list.Add(new Subject
            {
                Id = id,
                Code = code,
                Name = name,
                ChapterCount = chapterCount,
                QuestionCount = dbQuestionCount
            });
        }

        return list;
    }

    public Subject? GetSubjectById(int id)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.Id, s.Code, s.Name,
                   COUNT(DISTINCT q.Chapter) AS DbChapterCount,
                   COUNT(q.Id) AS DbQuestionCount
            FROM Subjects s
            LEFT JOIN Questions q ON s.Id = q.SubjectId
            WHERE s.Id = $id
            GROUP BY s.Id, s.Code, s.Name;";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var subjectId = reader.GetInt32(0);
        var code = reader.GetString(1);
        var name = reader.GetString(2);
        var dbChapterCount = reader.GetInt32(3);
        var dbQuestionCount = reader.GetInt32(4);

        var metaChapters = GetMetadataChapters(subjectId);
        var chapterCount = Math.Max(dbChapterCount, metaChapters.Count);

        return new Subject
        {
            Id = subjectId,
            Code = code,
            Name = name,
            ChapterCount = chapterCount,
            QuestionCount = dbQuestionCount
        };
    }

    public bool IsSubjectCodeExists(string code, int? excludeId = null)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*)
            FROM Subjects
            WHERE UPPER(TRIM(Code)) = UPPER(TRIM($code))
              AND ($excludeId IS NULL OR Id != $excludeId);";
        command.Parameters.AddWithValue("$code", code.Trim());
        command.Parameters.AddWithValue("$excludeId", excludeId.HasValue ? (object)excludeId.Value : DBNull.Value);

        var count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    public int AddSubject(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Subjects (Code, Name)
            VALUES ($code, $name);
            SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$code", subject.Code.Trim());
        command.Parameters.AddWithValue("$name", subject.Name.Trim());

        var newId = Convert.ToInt32(command.ExecuteScalar());
        subject.Id = newId;
        return newId;
    }

    public void UpdateSubject(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Subjects
            SET Code = $code, Name = $name
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$code", subject.Code.Trim());
        command.Parameters.AddWithValue("$name", subject.Name.Trim());
        command.Parameters.AddWithValue("$id", subject.Id);

        command.ExecuteNonQuery();
    }

    public void DeleteSubject(int subjectId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Subjects WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", subjectId);

        command.ExecuteNonQuery();

        RemoveMetadataForSubject(subjectId);
    }

    public int CountQuestionsBySubject(int subjectId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Questions WHERE SubjectId = $subjectId;";
        command.Parameters.AddWithValue("$subjectId", subjectId);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int CountExamPapersBySubject(int subjectId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM ExamPapers WHERE SubjectId = $subjectId;";
        command.Parameters.AddWithValue("$subjectId", subjectId);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    #endregion

    #region Chapter Operations

    public List<ChapterInfo> GetChaptersBySubject(int subjectId)
    {
        var questionCounts = new Dictionary<int, int>();

        using (var connection = SqliteContext.CreateConnection())
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Chapter, COUNT(Id) AS QuestionCount
                FROM Questions
                WHERE SubjectId = $subjectId
                GROUP BY Chapter
                ORDER BY Chapter;";
            command.Parameters.AddWithValue("$subjectId", subjectId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var chapterNum = reader.GetInt32(0);
                var qCount = reader.GetInt32(1);
                questionCounts[chapterNum] = qCount;
            }
        }

        var metadata = GetMetadataChapters(subjectId);
        var allNumbers = new SortedSet<int>(questionCounts.Keys.Concat(metadata.Keys));

        var result = new List<ChapterInfo>();
        foreach (var num in allNumbers)
        {
            metadata.TryGetValue(num, out var name);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Chương {num}";
            }

            questionCounts.TryGetValue(num, out var count);

            result.Add(new ChapterInfo
            {
                SubjectId = subjectId,
                Number = num,
                Name = name,
                QuestionCount = count
            });
        }

        return result;
    }

    public int CountQuestionsByChapter(int subjectId, int chapterNumber)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*)
            FROM Questions
            WHERE SubjectId = $subjectId AND Chapter = $chapter;";
        command.Parameters.AddWithValue("$subjectId", subjectId);
        command.Parameters.AddWithValue("$chapter", chapterNumber);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void UpdateChapterNumber(int subjectId, int oldNumber, int newNumber)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Questions
            SET Chapter = $newNumber
            WHERE SubjectId = $subjectId AND Chapter = $oldNumber;";
        command.Parameters.AddWithValue("$newNumber", newNumber);
        command.Parameters.AddWithValue("$subjectId", subjectId);
        command.Parameters.AddWithValue("$oldNumber", oldNumber);

        command.ExecuteNonQuery();

        UpdateMetadataChapterNumber(subjectId, oldNumber, newNumber);
    }

    public void SaveChapter(ChapterInfo chapter, int? originalNumber = null)
    {
        ArgumentNullException.ThrowIfNull(chapter);

        if (originalNumber.HasValue && originalNumber.Value != chapter.Number)
        {
            UpdateChapterNumber(chapter.SubjectId, originalNumber.Value, chapter.Number);
        }

        SaveMetadataChapter(chapter.SubjectId, chapter.Number, chapter.Name);
    }

    public void DeleteChapter(int subjectId, int chapterNumber)
    {
        var count = CountQuestionsByChapter(subjectId, chapterNumber);
        if (count > 0)
        {
            throw new InvalidOperationException($"Chương {chapterNumber} đang có {count} câu hỏi, không thể xóa.");
        }

        RemoveMetadataChapter(subjectId, chapterNumber);
    }

    #endregion

    #region Chapter Metadata Persistence

    private static Dictionary<int, string> GetMetadataChapters(int subjectId)
    {
        lock (MetadataLock)
        {
            var entries = LoadMetadata();
            return entries
                .Where(e => e.SubjectId == subjectId)
                .ToDictionary(e => e.Number, e => e.Name);
        }
    }

    private static void SaveMetadataChapter(int subjectId, int number, string name)
    {
        lock (MetadataLock)
        {
            var entries = LoadMetadata();
            var existing = entries.FirstOrDefault(e => e.SubjectId == subjectId && e.Number == number);
            if (existing is not null)
            {
                existing.Name = name.Trim();
            }
            else
            {
                entries.Add(new ChapterMetadataEntry
                {
                    SubjectId = subjectId,
                    Number = number,
                    Name = name.Trim()
                });
            }

            SaveMetadata(entries);
        }
    }

    private static void UpdateMetadataChapterNumber(int subjectId, int oldNumber, int newNumber)
    {
        lock (MetadataLock)
        {
            var entries = LoadMetadata();
            var existing = entries.FirstOrDefault(e => e.SubjectId == subjectId && e.Number == oldNumber);
            if (existing is not null)
            {
                existing.Number = newNumber;
            }

            SaveMetadata(entries);
        }
    }

    private static void RemoveMetadataChapter(int subjectId, int number)
    {
        lock (MetadataLock)
        {
            var entries = LoadMetadata();
            entries.RemoveAll(e => e.SubjectId == subjectId && e.Number == number);
            SaveMetadata(entries);
        }
    }

    private static void RemoveMetadataForSubject(int subjectId)
    {
        lock (MetadataLock)
        {
            var entries = LoadMetadata();
            entries.RemoveAll(e => e.SubjectId == subjectId);
            SaveMetadata(entries);
        }
    }

    private static List<ChapterMetadataEntry> LoadMetadata()
    {
        var path = MetadataFilePath;
        if (!File.Exists(path))
        {
            var defaultEntries = CreateInitialChapterMetadata();
            SaveMetadata(defaultEntries);
            return defaultEntries;
        }

        try
        {
            var json = File.ReadAllText(path);
            var result = JsonSerializer.Deserialize<List<ChapterMetadataEntry>>(json);
            return result ?? CreateInitialChapterMetadata();
        }
        catch
        {
            return CreateInitialChapterMetadata();
        }
    }

    private static void SaveMetadata(List<ChapterMetadataEntry> entries)
    {
        try
        {
            var path = MetadataFilePath;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch
        {
            // Bỏ qua lỗi ghi phụ nếu không ảnh hưởng hoạt động chính
        }
    }

    private static List<ChapterMetadataEntry> CreateInitialChapterMetadata() => [];

    #endregion

    #region Question Operations

    public List<Question> FindQuestions(int? subjectId = null, int? chapter = null, string? difficulty = null, string? keyword = null)
    {
        var list = new List<Question>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        var sql = @"
            SELECT Id, SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath
            FROM Questions
            WHERE 1=1";

        if (subjectId.HasValue)
        {
            sql += " AND SubjectId = $subjectId";
            command.Parameters.AddWithValue("$subjectId", subjectId.Value);
        }

        if (chapter.HasValue)
        {
            sql += " AND Chapter = $chapter";
            command.Parameters.AddWithValue("$chapter", chapter.Value);
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            sql += " AND UPPER(Difficulty) = UPPER($difficulty)";
            command.Parameters.AddWithValue("$difficulty", difficulty.Trim());
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += " AND Content LIKE $keyword";
            command.Parameters.AddWithValue("$keyword", $"%{keyword.Trim()}%");
        }

        sql += " ORDER BY Id DESC;";
        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Question
            {
                Id = reader.GetInt32(0),
                SubjectId = reader.GetInt32(1),
                Chapter = reader.GetInt32(2),
                Difficulty = reader.GetString(3),
                Content = reader.GetString(4),
                OptionA = reader.GetString(5),
                OptionB = reader.GetString(6),
                OptionC = reader.GetString(7),
                OptionD = reader.GetString(8),
                CorrectOption = reader.GetString(9),
                ImagePath = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
            });
        }

        return list;
    }

    public Question? GetQuestionById(int id)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath
            FROM Questions
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Question
        {
            Id = reader.GetInt32(0),
            SubjectId = reader.GetInt32(1),
            Chapter = reader.GetInt32(2),
            Difficulty = reader.GetString(3),
            Content = reader.GetString(4),
            OptionA = reader.GetString(5),
            OptionB = reader.GetString(6),
            OptionC = reader.GetString(7),
            OptionD = reader.GetString(8),
            CorrectOption = reader.GetString(9),
            ImagePath = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
        };
    }

    public int InsertQuestion(Question q)
    {
        ArgumentNullException.ThrowIfNull(q);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
            VALUES ($subjectId, $chapter, $difficulty, $content, $optA, $optB, $optC, $optD, $correct, $imagePath);
            SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$subjectId", q.SubjectId);
        command.Parameters.AddWithValue("$chapter", q.Chapter);
        command.Parameters.AddWithValue("$difficulty", q.Difficulty.Trim());
        command.Parameters.AddWithValue("$content", q.Content.Trim());
        command.Parameters.AddWithValue("$optA", q.OptionA.Trim());
        command.Parameters.AddWithValue("$optB", q.OptionB.Trim());
        command.Parameters.AddWithValue("$optC", q.OptionC.Trim());
        command.Parameters.AddWithValue("$optD", q.OptionD.Trim());
        command.Parameters.AddWithValue("$correct", q.CorrectOption.Trim().ToUpperInvariant());
        command.Parameters.AddWithValue("$imagePath", q.ImagePath ?? string.Empty);

        var newId = Convert.ToInt32(command.ExecuteScalar());
        q.Id = newId;
        return newId;
    }

    public void UpdateQuestion(Question q)
    {
        ArgumentNullException.ThrowIfNull(q);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Questions
            SET SubjectId = $subjectId,
                Chapter = $chapter,
                Difficulty = $difficulty,
                Content = $content,
                OptionA = $optA,
                OptionB = $optB,
                OptionC = $optC,
                OptionD = $optD,
                CorrectOption = $correct,
                ImagePath = $imagePath
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$subjectId", q.SubjectId);
        command.Parameters.AddWithValue("$chapter", q.Chapter);
        command.Parameters.AddWithValue("$difficulty", q.Difficulty.Trim());
        command.Parameters.AddWithValue("$content", q.Content.Trim());
        command.Parameters.AddWithValue("$optA", q.OptionA.Trim());
        command.Parameters.AddWithValue("$optB", q.OptionB.Trim());
        command.Parameters.AddWithValue("$optC", q.OptionC.Trim());
        command.Parameters.AddWithValue("$optD", q.OptionD.Trim());
        command.Parameters.AddWithValue("$correct", q.CorrectOption.Trim().ToUpperInvariant());
        command.Parameters.AddWithValue("$imagePath", q.ImagePath ?? string.Empty);
        command.Parameters.AddWithValue("$id", q.Id);

        command.ExecuteNonQuery();
    }

    public void SaveQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        if (question.Id <= 0)
        {
            InsertQuestion(question);
        }
        else
        {
            UpdateQuestion(question);
        }
    }

    public void DeleteQuestion(int questionId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Ràng buộc nghiệp vụ: kiểm tra câu hỏi đã được gán vào đề thi chưa
        using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM ExamPaperQuestions WHERE QuestionId = $id;";
        checkCmd.Parameters.AddWithValue("$id", questionId);
        var paperCount = Convert.ToInt32(checkCmd.ExecuteScalar());
        if (paperCount > 0)
        {
            throw new InvalidOperationException("Câu hỏi đã nằm trong đề thi, không thể xóa.");
        }

        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Questions WHERE Id = $id;";
        deleteCmd.Parameters.AddWithValue("$id", questionId);
        deleteCmd.ExecuteNonQuery();
    }

    public int CountAllQuestions()
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Questions;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<int> GetDistinctChapters(int? subjectId = null)
    {
        var list = new List<int>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        if (subjectId.HasValue)
        {
            command.CommandText = "SELECT DISTINCT Chapter FROM Questions WHERE SubjectId = $subjectId ORDER BY Chapter;";
            command.Parameters.AddWithValue("$subjectId", subjectId.Value);
        }
        else
        {
            command.CommandText = "SELECT DISTINCT Chapter FROM Questions ORDER BY Chapter;";
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(reader.GetInt32(0));
        }

        return list;
    }

    #endregion
}
