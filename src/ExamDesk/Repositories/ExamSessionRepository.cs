using ExamDesk.Data;
using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 3: SQL phục vụ quản lý ca; không thay đổi dữ liệu đề của Bạn 1.
public sealed class ExamSessionRepository
{
    public List<ExamPaper> FindPapers()
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PaperCode, Title, SubjectId, DurationMinutes, EasyCount, MediumCount, HardCount FROM ExamPapers ORDER BY Id DESC";
        using var reader = command.ExecuteReader();
        var result = new List<ExamPaper>();
        while (reader.Read()) result.Add(new ExamPaper { Id = reader.GetInt32(0), PaperCode = reader.GetString(1), Title = reader.GetString(2), SubjectId = reader.GetInt32(3), DurationMinutes = reader.GetInt32(4), EasyCount = reader.GetInt32(5), MediumCount = reader.GetInt32(6), HardCount = reader.GetInt32(7) });
        return result;
    }

    public List<ExamSession> FindSessions()
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, SessionCode, SessionName, PaperId, RoomName, IsOpen FROM ExamSessions ORDER BY Id DESC";
        using var reader = command.ExecuteReader();
        var result = new List<ExamSession>();
        while (reader.Read()) result.Add(new ExamSession { Id = reader.GetInt32(0), SessionCode = reader.GetString(1), SessionName = reader.GetString(2), PaperId = reader.GetInt32(3), RoomName = reader.GetString(4), IsOpen = reader.GetBoolean(5) });
        return result;
    }

    public int InsertSession(ExamSession session)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        // Ca mới luôn đóng; chỉ mở sau khi kiểm tra đề đủ câu.
        command.CommandText = "INSERT INTO ExamSessions(SessionCode, SessionName, PaperId, RoomName, IsOpen) VALUES ($code, $name, $paper, $room, 0); SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$code", session.SessionCode);
        command.Parameters.AddWithValue("$name", session.SessionName);
        command.Parameters.AddWithValue("$paper", session.PaperId);
        command.Parameters.AddWithValue("$room", session.RoomName);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void SetSessionOpen(int sessionId, bool isOpen)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        // Kiểm tra và cập nhật trong cùng câu SQL, không mở ca có đề thiếu câu.
        command.CommandText = """
            UPDATE ExamSessions SET IsOpen = $open WHERE Id = $id AND
            ($open = 0 OR EXISTS (
                SELECT 1 FROM ExamPapers p WHERE p.Id = ExamSessions.PaperId
                AND p.EasyCount + p.MediumCount + p.HardCount > 0
                AND (SELECT COUNT(*) FROM ExamPaperQuestions q WHERE q.PaperId = p.Id)
                    = p.EasyCount + p.MediumCount + p.HardCount))
            """;
        command.Parameters.AddWithValue("$id", sessionId);
        command.Parameters.AddWithValue("$open", isOpen ? 1 : 0);
        if (command.ExecuteNonQuery() != 1)
            throw new ExamDesk.Services.ExamSessionInputException("Ca không còn tồn tại hoặc đề chưa có đủ số câu. Hãy tải lại và kiểm tra đề.");
    }

    internal int CountInProgress(int sessionId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        return CountInProgress(command, sessionId);
    }

    // Giữ khóa ghi trong lúc đếm và đóng ca để một lượt mới không chen vào sau xác nhận.
    internal int CloseSession(int sessionId, bool allowInProgress)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        if (!allowInProgress)
        {
            int count = CountInProgress(command, sessionId);
            if (count > 0) return count;
        }

        command.Parameters.Clear();
        command.Parameters.AddWithValue("$session", sessionId);
        command.CommandText = "UPDATE ExamSessions SET IsOpen = 0 WHERE Id = $session";
        if (command.ExecuteNonQuery() != 1)
            throw new ExamDesk.Services.ExamSessionInputException("Ca thi không còn tồn tại. Hãy tải lại danh sách.");

        transaction.Commit();
        return 0;
    }

    private static int CountInProgress(Microsoft.Data.Sqlite.SqliteCommand command, int sessionId)
    {
        command.CommandText = """
            SELECT d.Payload
            FROM StudentDrafts d
            WHERE d.SessionId = $session AND NOT EXISTS (
                SELECT 1 FROM ExamAttempts a
                WHERE a.SessionId = d.SessionId AND a.StudentCode = d.StudentCode
            )
            """;
        command.Parameters.AddWithValue("$session", sessionId);
        var now = DateTimeOffset.UtcNow;
        int count = 0;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var draft = System.Text.Json.JsonSerializer.Deserialize<StudentDraft>(reader.GetString(0));
            if (draft is { PendingSubmission: false } && draft.DeadlineUtc > now) count++;
        }
        return count;
    }
}
