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

    // Một câu SELECT giữ ca, đề và tiến độ trong cùng một snapshot SQLite.
    internal (ExamSession Session, ExamPaper Paper) FindSelectedSession(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT s.Id, s.SessionCode, s.SessionName, s.PaperId, s.RoomName, s.IsOpen,
                p.Id, p.PaperCode, p.Title, p.SubjectId, p.DurationMinutes,
                p.EasyCount, p.MediumCount, p.HardCount,
                EXISTS(SELECT 1 FROM StudentDrafts d WHERE d.SessionId=s.Id AND d.StudentCode=$student),
                EXISTS(SELECT 1 FROM ExamAttempts a WHERE a.SessionId=s.Id AND a.StudentCode=$student)
            FROM ExamSessions s LEFT JOIN ExamPapers p ON p.Id=s.PaperId
            WHERE s.Id=$session
            """;
        command.Parameters.AddWithValue("$session", sessionId);
        command.Parameters.AddWithValue("$student", studentCode);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            throw new ExamDesk.Services.StudentExamInputException("Ca thi không còn tồn tại. Hãy tải lại danh sách.");
        if (reader.GetBoolean(15))
            throw new ExamDesk.Services.StudentExamInputException("Bạn đã nộp bài ở ca này. Hãy tải lại danh sách ca.");
        if (!reader.GetBoolean(5) && !reader.GetBoolean(14))
            throw new ExamDesk.Services.StudentExamInputException("Ca thi đã đóng và bạn chưa bắt đầu lượt thi. Hãy chọn ca đang mở hoặc hỏi giám thị coi thi.");
        if (reader.IsDBNull(6))
            throw new ExamDesk.Services.StudentExamInputException("Ca thi chưa có đề. Hãy tải lại danh sách hoặc hỏi giám thị coi thi.");
        var session = new ExamSession
        {
            Id = reader.GetInt32(0), SessionCode = reader.GetString(1), SessionName = reader.GetString(2),
            PaperId = reader.GetInt32(3), RoomName = reader.GetString(4), IsOpen = reader.GetBoolean(5)
        };
        var paper = new ExamPaper
        {
            Id = reader.GetInt32(6), PaperCode = reader.GetString(7), Title = reader.GetString(8),
            SubjectId = reader.GetInt32(9), DurationMinutes = reader.GetInt32(10),
            EasyCount = reader.GetInt32(11), MediumCount = reader.GetInt32(12), HardCount = reader.GetInt32(13)
        };
        return (session, paper);
    }

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
