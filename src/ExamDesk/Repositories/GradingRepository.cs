using System.Globalization;
using System.Text.Json;
using ExamDesk.Data;
using ExamDesk.Models;
using ExamDesk.Services;

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

    public int CountAllAttempts()
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM ExamAttempts;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<AttemptSummaryItem> FindAttemptSummaries(int? sessionId = null, string? className = null, string? keyword = null)
    {
        var list = new List<AttemptSummaryItem>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        var sql = @"
            SELECT a.Id, a.SessionId, a.PaperId, a.StudentCode, a.StudentName, a.ClassName,
                   COALESCE(s.SessionCode || ' - ' || s.SessionName, 'Ca #' || a.SessionId) AS SessionDisplay,
                   COALESCE(p.PaperCode || ' - ' || p.Title, 'Đề #' || a.PaperId) AS PaperDisplay,
                   a.TotalQuestions, a.CorrectCount,
                   a.TotalQuestions - COUNT(CASE WHEN aa.SelectedOption <> '' THEN 1 END) AS BlankCount,
                   a.Score, a.IsAutoSubmitted, a.SubmittedAt
            FROM ExamAttempts a
            LEFT JOIN AttemptAnswers aa ON aa.AttemptId = a.Id
            LEFT JOIN ExamSessions s ON s.Id = a.SessionId
            LEFT JOIN ExamPapers p ON p.Id = a.PaperId
            WHERE 1 = 1";

        if (sessionId.HasValue && sessionId.Value > 0)
        {
            sql += " AND a.SessionId = $sessionId";
            command.Parameters.AddWithValue("$sessionId", sessionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(className))
        {
            sql += " AND UPPER(TRIM(a.ClassName)) = UPPER(TRIM($className))";
            command.Parameters.AddWithValue("$className", className.Trim());
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += " AND (UPPER(a.StudentCode) LIKE UPPER($keyword) OR UPPER(a.StudentName) LIKE UPPER($keyword))";
            command.Parameters.AddWithValue("$keyword", $"%{keyword.Trim()}%");
        }

        sql += " GROUP BY a.Id ORDER BY a.SubmittedAt DESC, a.Id DESC;";
        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new AttemptSummaryItem
            {
                AttemptId = reader.GetInt32(0),
                SessionId = reader.GetInt32(1),
                PaperId = reader.GetInt32(2),
                StudentCode = reader.GetString(3),
                StudentName = reader.GetString(4),
                ClassName = reader.GetString(5),
                SessionName = reader.GetString(6),
                PaperTitle = reader.GetString(7),
                TotalQuestions = reader.GetInt32(8),
                CorrectCount = reader.GetInt32(9),
                BlankCount = Math.Max(0, reader.GetInt32(10)),
                Score = reader.GetDouble(11),
                IsAutoSubmitted = reader.GetInt32(12) == 1,
                SubmittedAt = DateTimeOffset.Parse(reader.GetString(13), CultureInfo.InvariantCulture)
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

    public List<AttemptReviewDetailItem> FindAttemptReviewDetails(int attemptId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        int sessionId;
        string studentCode;
        using (var attemptCmd = connection.CreateCommand())
        {
            attemptCmd.CommandText = "SELECT SessionId, StudentCode FROM ExamAttempts WHERE Id = $attemptId LIMIT 1;";
            attemptCmd.Parameters.AddWithValue("$attemptId", attemptId);
            using var reader = attemptCmd.ExecuteReader();
            if (!reader.Read())
                return [];

            sessionId = reader.GetInt32(0);
            studentCode = reader.GetString(1);
        }

        var snapshotByQuestionId = new Dictionary<int, StudentQuestion>();
        var historyByQuestionId = new Dictionary<int, string>();

        using (var draftCmd = connection.CreateCommand())
        {
            draftCmd.CommandText = @"
                SELECT Payload
                FROM StudentDrafts
                WHERE SessionId = $sessionId
                  AND UPPER(TRIM(StudentCode)) = UPPER(TRIM($studentCode))
                LIMIT 1;";
            draftCmd.Parameters.AddWithValue("$sessionId", sessionId);
            draftCmd.Parameters.AddWithValue("$studentCode", studentCode);

            if (draftCmd.ExecuteScalar() is string payload && !string.IsNullOrWhiteSpace(payload))
            {
                try
                {
                    var draft = JsonSerializer.Deserialize<StudentDraft>(payload);
                    if (draft is not null)
                    {
                        foreach (var q in draft.Questions)
                            snapshotByQuestionId[q.Id] = q;

                        foreach (var group in draft.History.GroupBy(h => h.QuestionId))
                        {
                            var steps = group
                                .Select(h => h.Choice is >= 0 and <= 3 ? ((char)('A' + h.Choice.Value)).ToString() : "(bỏ chọn)")
                                .ToList();

                            if (steps.Count > 0)
                                historyByQuestionId[group.Key] = string.Join(" → ", steps);
                        }
                    }
                }
                catch (JsonException)
                {
                    // Bỏ qua nếu Payload cũ không đọc được; fallback sang dữ liệu bảng Questions.
                }
            }
        }

        var details = new List<AttemptReviewDetailItem>();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT aa.QuestionOrder, aa.QuestionId,
                       aa.SelectedOption, aa.CorrectOption, aa.IsCorrect,
                       COALESCE(q.Content, ''),
                       COALESCE(q.OptionA, ''), COALESCE(q.OptionB, ''),
                       COALESCE(q.OptionC, ''), COALESCE(q.OptionD, '')
                FROM AttemptAnswers aa
                LEFT JOIN Questions q ON q.Id = aa.QuestionId
                WHERE aa.AttemptId = $attemptId
                ORDER BY aa.QuestionOrder;";
            cmd.Parameters.AddWithValue("$attemptId", attemptId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int questionOrder = reader.GetInt32(0);
                int questionId = reader.GetInt32(1);
                string selected = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                string correct = reader.GetString(3);
                bool isCorrect = reader.GetInt32(4) == 1;

                string content = reader.GetString(5);
                string optA = reader.GetString(6);
                string optB = reader.GetString(7);
                string optC = reader.GetString(8);
                string optD = reader.GetString(9);

                // Ưu tiên snapshot câu hỏi trong StudentDrafts để bảo toàn nội dung lúc thi
                if (snapshotByQuestionId.TryGetValue(questionId, out var snap))
                {
                    if (!string.IsNullOrWhiteSpace(snap.Content))
                        content = snap.Content;
                    if (snap.Options.Length > 0) optA = snap.Options[0];
                    if (snap.Options.Length > 1) optB = snap.Options[1];
                    if (snap.Options.Length > 2) optC = snap.Options[2];
                    if (snap.Options.Length > 3) optD = snap.Options[3];
                }

                if (!historyByQuestionId.TryGetValue(questionId, out var historyText) || string.IsNullOrWhiteSpace(historyText))
                {
                    historyText = string.IsNullOrWhiteSpace(selected) ? "(không chọn)" : selected;
                }

                details.Add(new AttemptReviewDetailItem
                {
                    QuestionOrder = questionOrder,
                    QuestionId = questionId,
                    QuestionContent = content,
                    OptionA = optA,
                    OptionB = optB,
                    OptionC = optC,
                    OptionD = optD,
                    SelectedOption = selected,
                    CorrectOption = correct,
                    IsCorrect = isCorrect,
                    ChangeHistory = historyText
                });
            }
        }

        return details;
    }
}
