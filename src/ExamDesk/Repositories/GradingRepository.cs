using System.Globalization;
using ExamDesk.Data;
using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách: Đọc danh sách kết quả thi và chi tiết câu trả lời từ SQLite để báo cáo và phúc khảo.
public sealed class GradingRepository
{
    public List<ExamAttempt> FindAttempts(int? sessionId = null, string? className = null)
    {
        var list = new List<ExamAttempt>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        var sql = @"
            SELECT Id, SessionId, PaperId, StudentCode, StudentName, ClassName,
                   StartedAt, SubmittedAt, TotalQuestions, CorrectCount, Score,
                   IsAutoSubmitted, ReviewNote
            FROM ExamAttempts
            WHERE 1 = 1";

        if (sessionId.HasValue && sessionId.Value > 0)
        {
            sql += " AND SessionId = $sessionId";
            command.Parameters.AddWithValue("$sessionId", sessionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(className))
        {
            sql += " AND UPPER(TRIM(ClassName)) = UPPER(TRIM($className))";
            command.Parameters.AddWithValue("$className", className.Trim());
        }

        sql += " ORDER BY SubmittedAt DESC, Id DESC;";
        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ExamAttempt
            {
                Id = reader.GetInt32(0),
                SessionId = reader.GetInt32(1),
                PaperId = reader.GetInt32(2),
                StudentCode = reader.GetString(3),
                StudentName = reader.GetString(4),
                ClassName = reader.GetString(5),
                StartedAt = DateTimeOffset.Parse(reader.GetString(6), CultureInfo.InvariantCulture),
                SubmittedAt = DateTimeOffset.Parse(reader.GetString(7), CultureInfo.InvariantCulture),
                TotalQuestions = reader.GetInt32(8),
                CorrectCount = reader.GetInt32(9),
                Score = reader.GetDouble(10),
                IsAutoSubmitted = reader.GetInt32(11) == 1,
                ReviewNote = reader.IsDBNull(12) ? string.Empty : reader.GetString(12)
            });
        }

        return list;
    }

    public List<AttemptAnswer> FindAnswersByAttemptId(int attemptId)
    {
        var list = new List<AttemptAnswer>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT AttemptId, QuestionId, QuestionOrder,
                   SelectedOption, CorrectOption, IsCorrect
            FROM AttemptAnswers
            WHERE AttemptId = $attemptId
            ORDER BY QuestionOrder;";
        command.Parameters.AddWithValue("$attemptId", attemptId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new AttemptAnswer
            {
                AttemptId = reader.GetInt32(0),
                QuestionId = reader.GetInt32(1),
                QuestionOrder = reader.GetInt32(2),
                SelectedOption = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                CorrectOption = reader.GetString(4),
                IsCorrect = reader.GetInt32(5) == 1
            });
        }

        return list;
    }
}