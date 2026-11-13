using ExamDesk.Data;
using ExamDesk.Models;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Repositories;

/// <summary>
/// Repository thao tác với bảng ExamPapers và ExamPaperQuestions trong SQLite.
/// </summary>
public sealed class ExamPaperRepository
{
    /// <summary>
    /// Rút ngẫu nhiên danh sách ID câu hỏi không trùng theo môn học và độ khó.
    /// </summary>
    public List<int> PickRandomQuestionIds(int subjectId, string difficulty, int count)
    {
        if (count <= 0)
        {
            return [];
        }

        var ids = new List<int>(count);
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Dùng ORDER BY RANDOM() trên tập khóa chính Id của bảng Questions để rút ngẫu nhiên không trùng.
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id
            FROM Questions
            WHERE SubjectId = $subjectId AND Difficulty = $difficulty
            ORDER BY RANDOM()
            LIMIT $count;";
        command.Parameters.AddWithValue("$subjectId", subjectId);
        command.Parameters.AddWithValue("$difficulty", difficulty);
        command.Parameters.AddWithValue("$count", count);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ids.Add(reader.GetInt32(0));
        }

        return ids;
    }

    /// <summary>
    /// Lưu đề thi mới và danh sách câu hỏi kèm thứ tự OrderIndex trong một SqliteTransaction duy nhất.
    /// </summary>
    public int InsertExamPaper(ExamPaper paper, List<int> questionIds)
    {
        ArgumentNullException.ThrowIfNull(paper);
        ArgumentNullException.ThrowIfNull(questionIds);

        if (questionIds.Distinct().Count() != questionIds.Count)
        {
            throw new InvalidOperationException("Danh sách câu hỏi bị trùng lặp, không thể lưu đề thi.");
        }

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Bảo toàn dữ liệu: chèn ExamPapers và toàn bộ ExamPaperQuestions trong cùng 1 transaction.
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var insertPaperCmd = connection.CreateCommand())
            {
                insertPaperCmd.Transaction = transaction;
                insertPaperCmd.CommandText = @"
                    INSERT INTO ExamPapers (PaperCode, Title, SubjectId, DurationMinutes, EasyCount, MediumCount, HardCount)
                    VALUES ($paperCode, $title, $subjectId, $durationMinutes, $easyCount, $mediumCount, $hardCount);
                    SELECT last_insert_rowid();";
                insertPaperCmd.Parameters.AddWithValue("$paperCode", paper.PaperCode);
                insertPaperCmd.Parameters.AddWithValue("$title", paper.Title);
                insertPaperCmd.Parameters.AddWithValue("$subjectId", paper.SubjectId);
                insertPaperCmd.Parameters.AddWithValue("$durationMinutes", paper.DurationMinutes);
                insertPaperCmd.Parameters.AddWithValue("$easyCount", paper.EasyCount);
                insertPaperCmd.Parameters.AddWithValue("$mediumCount", paper.MediumCount);
                insertPaperCmd.Parameters.AddWithValue("$hardCount", paper.HardCount);

                paper.Id = Convert.ToInt32(insertPaperCmd.ExecuteScalar());
            }

            InsertPaperQuestionsInternal(connection, transaction, paper.Id, questionIds);

            transaction.Commit();
            return paper.Id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Lấy danh sách đề thi để chọn khi tạo ca thi.
    /// </summary>
    public List<ExamPaper> FindPapers()
    {
        var list = new List<ExamPaper>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, PaperCode, Title, SubjectId, DurationMinutes, EasyCount, MediumCount, HardCount
            FROM ExamPapers
            ORDER BY Id DESC;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(MapExamPaper(reader));
        }

        return list;
    }

    /// <summary>
    /// Đếm số câu hỏi hiện có theo từng mức độ khó (Easy, Medium, Hard) của một môn học.
    /// </summary>
    public ExamPaperDifficultyCounts GetAvailableCountsByDifficulty(int subjectId)
    {
        var counts = new ExamPaperDifficultyCounts();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Difficulty, COUNT(*)
            FROM Questions
            WHERE SubjectId = $subjectId
            GROUP BY Difficulty;";
        command.Parameters.AddWithValue("$subjectId", subjectId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var difficulty = reader.GetString(0);
            var total = reader.GetInt32(1);
            switch (difficulty)
            {
                case "Easy":
                    counts.EasyCount = total;
                    break;
                case "Medium":
                    counts.MediumCount = total;
                    break;
                case "Hard":
                    counts.HardCount = total;
                    break;
            }
        }

        return counts;
    }

    /// <summary>
    /// Kiểm tra mã đề đã tồn tại trong CSDL hay chưa.
    /// </summary>
    public bool IsPaperCodeExists(string paperCode, int? excludePaperId = null)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        if (excludePaperId.HasValue)
        {
            command.CommandText = @"
                SELECT EXISTS(
                    SELECT 1 FROM ExamPapers
                    WHERE LOWER(PaperCode) = LOWER($paperCode) AND Id <> $excludeId
                );";
            command.Parameters.AddWithValue("$excludeId", excludePaperId.Value);
        }
        else
        {
            command.CommandText = @"
                SELECT EXISTS(
                    SELECT 1 FROM ExamPapers
                    WHERE LOWER(PaperCode) = LOWER($paperCode)
                );";
        }

        command.Parameters.AddWithValue("$paperCode", paperCode.Trim());
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    /// <summary>
    /// Lấy danh sách tóm tắt đề thi kèm môn học, số câu đã gắn và số ca thi đang dùng.
    /// </summary>
    public List<ExamPaperSummary> FindPaperSummaries(int? subjectId = null, string? keyword = null)
    {
        var list = new List<ExamPaperSummary>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        var sql = @"
            SELECT
                p.Id,
                p.PaperCode,
                p.Title,
                p.SubjectId,
                p.DurationMinutes,
                p.EasyCount,
                p.MediumCount,
                p.HardCount,
                s.Code AS SubjectCode,
                s.Name AS SubjectName,
                (SELECT COUNT(*) FROM ExamPaperQuestions epq WHERE epq.PaperId = p.Id) AS AssignedQuestionCount,
                (SELECT COUNT(*) FROM ExamSessions es WHERE es.PaperId = p.Id) AS SessionCount
            FROM ExamPapers p
            INNER JOIN Subjects s ON s.Id = p.SubjectId
            WHERE 1 = 1";

        if (subjectId.HasValue && subjectId.Value > 0)
        {
            sql += " AND p.SubjectId = $subjectId";
            command.Parameters.AddWithValue("$subjectId", subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += " AND (p.PaperCode LIKE $keyword OR p.Title LIKE $keyword)";
            command.Parameters.AddWithValue("$keyword", $"%{keyword.Trim()}%");
        }

        sql += " ORDER BY p.Id DESC;";
        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var paper = MapExamPaper(reader);
            list.Add(new ExamPaperSummary
            {
                Paper = paper,
                SubjectCode = reader.GetString(8),
                SubjectName = reader.GetString(9),
                AssignedQuestionCount = reader.GetInt32(10),
                SessionCount = reader.GetInt32(11)
            });
        }

        return list;
    }

    /// <summary>
    /// Lấy thông tin một đề thi theo Id.
    /// </summary>
    public ExamPaper? GetPaperById(int paperId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, PaperCode, Title, SubjectId, DurationMinutes, EasyCount, MediumCount, HardCount
            FROM ExamPapers
            WHERE Id = $paperId;";
        command.Parameters.AddWithValue("$paperId", paperId);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapExamPaper(reader) : null;
    }

    /// <summary>
    /// Lấy danh sách chi tiết câu hỏi thuộc một đề thi theo đúng thứ tự OrderIndex.
    /// </summary>
    public List<ExamPaperQuestionDetail> GetPaperQuestions(int paperId)
    {
        var list = new List<ExamPaperQuestionDetail>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                epq.OrderIndex,
                q.Id,
                q.Difficulty,
                q.Chapter,
                q.Content,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.CorrectOption,
                q.ImagePath
            FROM ExamPaperQuestions epq
            INNER JOIN Questions q ON q.Id = epq.QuestionId
            WHERE epq.PaperId = $paperId
            ORDER BY epq.OrderIndex;";
        command.Parameters.AddWithValue("$paperId", paperId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ExamPaperQuestionDetail
            {
                OrderIndex = reader.GetInt32(0),
                QuestionId = reader.GetInt32(1),
                Difficulty = reader.GetString(2),
                Chapter = reader.GetInt32(3).ToString(),
                Content = reader.GetString(4),
                OptionA = reader.GetString(5),
                OptionB = reader.GetString(6),
                OptionC = reader.GetString(7),
                OptionD = reader.GetString(8),
                CorrectOption = reader.GetString(9),
                ImagePath = reader.IsDBNull(10) ? null : reader.GetString(10)
            });
        }

        return list;
    }

    /// <summary>
    /// Kiểm tra đề thi đã được sử dụng trong bất kỳ ca thi hoặc lượt làm bài nào chưa.
    /// </summary>
    public bool IsPaperUsedInSession(int paperId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CASE
                WHEN EXISTS(SELECT 1 FROM ExamSessions WHERE PaperId = $paperId)
                  OR EXISTS(SELECT 1 FROM ExamAttempts WHERE PaperId = $paperId)
                THEN 1 ELSE 0
            END;";
        command.Parameters.AddWithValue("$paperId", paperId);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    /// <summary>
    /// Thay thế toàn bộ câu hỏi của đề thi (khi đề chưa có ca thi) trong một SqliteTransaction.
    /// </summary>
    public void ReplacePaperQuestions(int paperId, List<int> newQuestionIds)
    {
        ArgumentNullException.ThrowIfNull(newQuestionIds);

        if (newQuestionIds.Distinct().Count() != newQuestionIds.Count)
        {
            throw new InvalidOperationException("Danh sách câu hỏi rút lại bị trùng lặp, không thể cập nhật đề thi.");
        }

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Bảo toàn dữ liệu: xóa bộ câu cũ và ghi bộ câu mới trong cùng 1 transaction.
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var deleteCmd = connection.CreateCommand())
            {
                deleteCmd.Transaction = transaction;
                deleteCmd.CommandText = "DELETE FROM ExamPaperQuestions WHERE PaperId = $paperId;";
                deleteCmd.Parameters.AddWithValue("$paperId", paperId);
                deleteCmd.ExecuteNonQuery();
            }

            InsertPaperQuestionsInternal(connection, transaction, paperId, newQuestionIds);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Xóa đề thi và các liên kết câu hỏi của đề trong một SqliteTransaction.
    /// </summary>
    public void DeletePaper(int paperId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Bảo toàn dữ liệu: xóa ExamPaperQuestions trước rồi mới xóa ExamPapers trong cùng 1 transaction.
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var deleteQuestionsCmd = connection.CreateCommand())
            {
                deleteQuestionsCmd.Transaction = transaction;
                deleteQuestionsCmd.CommandText = "DELETE FROM ExamPaperQuestions WHERE PaperId = $paperId;";
                deleteQuestionsCmd.Parameters.AddWithValue("$paperId", paperId);
                deleteQuestionsCmd.ExecuteNonQuery();
            }

            using (var deletePaperCmd = connection.CreateCommand())
            {
                deletePaperCmd.Transaction = transaction;
                deletePaperCmd.CommandText = "DELETE FROM ExamPapers WHERE Id = $paperId;";
                deletePaperCmd.Parameters.AddWithValue("$paperId", paperId);
                deletePaperCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void InsertPaperQuestionsInternal(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int paperId,
        List<int> questionIds)
    {
        using var insertCmd = connection.CreateCommand();
        insertCmd.Transaction = transaction;
        insertCmd.CommandText = @"
            INSERT INTO ExamPaperQuestions (PaperId, QuestionId, OrderIndex)
            VALUES ($paperId, $questionId, $orderIndex);";

        var paperIdParam = insertCmd.Parameters.Add("$paperId", SqliteType.Integer);
        var questionIdParam = insertCmd.Parameters.Add("$questionId", SqliteType.Integer);
        var orderIndexParam = insertCmd.Parameters.Add("$orderIndex", SqliteType.Integer);

        paperIdParam.Value = paperId;

        for (var i = 0; i < questionIds.Count; i++)
        {
            questionIdParam.Value = questionIds[i];
            orderIndexParam.Value = i + 1;
            insertCmd.ExecuteNonQuery();
        }
    }

    private static ExamPaper MapExamPaper(SqliteDataReader reader)
    {
        return new ExamPaper
        {
            Id = reader.GetInt32(0),
            PaperCode = reader.GetString(1),
            Title = reader.GetString(2),
            SubjectId = reader.GetInt32(3),
            DurationMinutes = reader.GetInt32(4),
            EasyCount = reader.GetInt32(5),
            MediumCount = reader.GetInt32(6),
            HardCount = reader.GetInt32(7)
        };
    }
}
