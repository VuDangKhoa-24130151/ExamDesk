using ExamDesk.Data;
using ExamDesk.Models;

namespace ExamDesk.Repositories;

public sealed class StudentExamRepository
{
    public User? FindStudent(string studentCode)
    {
        using var connection = SqliteContext.CreateConnection(); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, StudentCode, ClassName FROM Users WHERE Role='Student' AND StudentCode=$code LIMIT 2";
        command.Parameters.AddWithValue("$code", studentCode);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            System.Diagnostics.Trace.WriteLine($"Không tìm thấy MSSV trong {connection.DataSource}");
            return null;
        }
        var student = new User { Id=reader.GetInt32(0), FullName=reader.GetString(1), StudentCode=reader.GetString(2), ClassName=reader.GetString(3) };
        if (reader.Read()) throw new ExamDesk.Services.StudentExamInputException("MSSV bị trùng trong danh mục. Vui lòng liên hệ giảng viên.");
        return student;
    }

    public List<ExamSession> FindOpenSessions() => new ExamSessionRepository().FindSessions().Where(s => s.IsOpen).ToList();

    // Có khóa nghĩa là đã bắt đầu, giá trị true nghĩa là đã lưu bài nộp.
    internal Dictionary<int, bool> FindSessionProgress(string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.Parameters.AddWithValue("$student", studentCode);
        command.CommandText = """
            SELECT SessionId, MAX(Submitted)
            FROM (
                SELECT SessionId, 0 AS Submitted FROM StudentDrafts WHERE StudentCode = $student
                UNION ALL
                SELECT SessionId, 1 AS Submitted FROM ExamAttempts WHERE StudentCode = $student
            )
            GROUP BY SessionId
            """;
        var progress = new Dictionary<int, bool>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            progress.Add(reader.GetInt32(0), reader.GetBoolean(1));
        return progress;
    }


    internal StudentResult? FindResult(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT a.Score, a.TotalQuestions, a.CorrectCount, a.SubmittedAt, a.IsAutoSubmitted,
                a.TotalQuestions - COUNT(CASE WHEN aa.SelectedOption <> '' THEN 1 END),
                a.StudentName, a.StudentCode,
                COALESCE(s.SessionName, 'Ca #' || a.SessionId),
                COALESCE(p.Title, 'Đề #' || a.PaperId)
            FROM ExamAttempts a
            LEFT JOIN AttemptAnswers aa ON aa.AttemptId = a.Id
            LEFT JOIN ExamSessions s ON s.Id = a.SessionId
            LEFT JOIN ExamPapers p ON p.Id = a.PaperId
            WHERE a.SessionId = $session AND a.StudentCode = $student
            GROUP BY a.Id
            """;
        command.Parameters.AddWithValue("$session", sessionId);
        command.Parameters.AddWithValue("$student", studentCode);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new StudentResult(reader.GetDouble(0), reader.GetInt32(1), reader.GetInt32(2),
            reader.GetInt32(5), DateTimeOffset.Parse(reader.GetString(3),
                System.Globalization.CultureInfo.InvariantCulture), reader.GetBoolean(4),
            reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
    }
}

internal sealed record StudentResult(double Score, int Total, int Correct, int Blank,
    DateTimeOffset SubmittedAt, bool IsAutoSubmitted,
    string StudentName, string StudentCode, string SessionName, string PaperTitle);
