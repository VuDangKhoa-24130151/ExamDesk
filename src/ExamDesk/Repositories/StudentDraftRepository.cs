using System.Text.Json;
using ExamDesk.Data;
using ExamDesk.Models;

namespace ExamDesk.Repositories;

// Bạn 3: một bản nháp/SV/ca. Revision ngăn hai cửa sổ ghi đè nhau.
public sealed class StudentDraftRepository
{
    public StudentDraft StartOrResume(int sessionId, User student)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.Parameters.AddWithValue("$session", sessionId);
        cmd.Parameters.AddWithValue("$student", student.StudentCode);
        cmd.CommandText = "SELECT COUNT(*) FROM ExamAttempts WHERE SessionId=$session AND StudentCode=$student";
        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            throw new InvalidOperationException("Bạn đã nộp bài ca này.");

        cmd.CommandText = "SELECT Payload, Revision FROM StudentDrafts WHERE SessionId=$session AND StudentCode=$student";
        StudentDraft? draft = null;
        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                draft = JsonSerializer.Deserialize<StudentDraft>(reader.GetString(0))!;
                draft.Revision = reader.GetInt32(1);
            }
        }

        // Bản nháp giữ nguyên đề và hạn nộp, kể cả khi ca đã đóng.
        if (draft is not null)
        {
            tx.Commit();
            return draft;
        }

        cmd.CommandText = """
            SELECT s.PaperId, s.SessionName, p.DurationMinutes, p.EasyCount + p.MediumCount + p.HardCount
            FROM ExamSessions s JOIN ExamPapers p ON p.Id = s.PaperId
            WHERE s.Id = $session AND s.IsOpen = 1
            """;
        int count, minutes, paperId;
        string title;
        using (var reader = cmd.ExecuteReader())
        {
            if (!reader.Read())
                throw new InvalidOperationException("Ca đã đóng hoặc không có đề thi.");
            paperId = reader.GetInt32(0);
            title = reader.GetString(1);
            minutes = reader.GetInt32(2);
            count = reader.GetInt32(3);
        }

        cmd.Parameters.AddWithValue("$paper", paperId);
        cmd.CommandText = """
            SELECT q.Id, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.ImagePath
            FROM ExamPaperQuestions pq JOIN Questions q ON q.Id = pq.QuestionId
            WHERE pq.PaperId = $paper ORDER BY pq.OrderIndex
            """;
        var questions = new List<StudentQuestion>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                questions.Add(new StudentQuestion
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Options = [reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)],
                    ImagePath = reader.GetString(6)
                });
            }
        }

        if (count <= 0 || count != questions.Count)
            throw new InvalidOperationException("Đề chưa có đủ câu hỏi.");

        var now = DateTimeOffset.UtcNow;
        draft = new StudentDraft
        {
            SessionId = sessionId,
            PaperId = paperId,
            StudentCode = student.StudentCode,
            StudentName = student.FullName,
            ClassName = student.ClassName,
            Title = title,
            StartedUtc = now,
            DeadlineUtc = now.AddMinutes(minutes),
            Questions = questions,
            Answers = new int?[count],
            Flags = new bool[count]
        };
        cmd.CommandText = "INSERT INTO StudentDrafts(SessionId, StudentCode, Payload) VALUES ($session, $student, $payload)";
        cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(draft));
        cmd.ExecuteNonQuery();
        tx.Commit();
        return draft;
    }

    public StudentDraft LockForSubmission(StudentDraft draft)
        => LockForSubmission(draft, () => DateTimeOffset.UtcNow);

    internal StudentDraft LockForSubmission(StudentDraft draft, Func<DateTimeOffset> readUtc)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.Parameters.AddWithValue("$session", draft.SessionId);
        cmd.Parameters.AddWithValue("$student", draft.StudentCode);
        cmd.CommandText = "SELECT Payload, Revision FROM StudentDrafts WHERE SessionId=$session AND StudentCode=$student";
        StudentDraft saved;
        using (var reader = cmd.ExecuteReader())
        {
            if (!reader.Read())
                throw new InvalidOperationException("Không tìm thấy tiến độ của lượt thi.");
            saved = JsonSerializer.Deserialize<StudentDraft>(reader.GetString(0))!;
            saved.Revision = reader.GetInt32(1);
        }

        if (saved.StartedUtc != draft.StartedUtc)
            throw new StudentDraftConflictException();

        // Bản nháp đã khóa có thể gửi lại, kể cả từ phiên bản cũ.
        if (saved.PendingSubmission)
        {
            tx.Commit();
            return saved;
        }

        if (saved.Revision != draft.Revision)
            throw new StudentDraftConflictException();

        var now = readUtc();
        // Khóa và lưu lý do nộp trước khi chuyển bản nháp cho bộ chấm.
        saved.PendingSubmission = true;
        saved.LockedAtUtc = now;
        saved.AutoSubmitted = now >= saved.DeadlineUtc;
        cmd.CommandText = "UPDATE StudentDrafts SET Payload=$payload, Revision=Revision+1 WHERE SessionId=$session AND StudentCode=$student";
        cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(saved));
        cmd.ExecuteNonQuery();
        tx.Commit();
        saved.Revision++;
        return saved;
    }

    internal StudentDraft? FindDraft(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.Parameters.AddWithValue("$session", sessionId);
        cmd.Parameters.AddWithValue("$student", studentCode);
        cmd.CommandText = "SELECT Payload, Revision FROM StudentDrafts WHERE SessionId=$session AND StudentCode=$student";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        var draft = JsonSerializer.Deserialize<StudentDraft>(reader.GetString(0))!;
        draft.Revision = reader.GetInt32(1);
        return draft;
    }

    public void Save(StudentDraft draft)
        => Save(draft, () => DateTimeOffset.UtcNow);

    internal void Save(StudentDraft draft, Func<DateTimeOffset> readUtc)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.Parameters.AddWithValue("$session", draft.SessionId);
        cmd.Parameters.AddWithValue("$student", draft.StudentCode);
        cmd.CommandText = "SELECT Payload, Revision FROM StudentDrafts WHERE SessionId=$session AND StudentCode=$student";

        StudentDraft saved;
        using (var reader = cmd.ExecuteReader())
        {
            if (!reader.Read())
                throw new InvalidOperationException("Không tìm thấy tiến độ của lượt thi.");
            // Revision cũ không được ghi đè lượt vừa thay đổi ở cửa sổ khác.
            if (reader.GetInt32(1) != draft.Revision)
                throw new StudentDraftConflictException();
            saved = JsonSerializer.Deserialize<StudentDraft>(reader.GetString(0))!;
        }

        if (saved.StartedUtc != draft.StartedUtc)
            throw new StudentDraftConflictException();
        if (saved.PendingSubmission)
            throw new InvalidOperationException("Bài đã khóa để nộp, không thể đổi lựa chọn.");
        if (readUtc() >= saved.DeadlineUtc)
            throw new StudentDraftExpiredException();

        cmd.CommandText = "SELECT COUNT(*) FROM ExamAttempts WHERE SessionId=$session AND StudentCode=$student";
        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
            throw new InvalidOperationException("Bài đã nộp, không thể sửa.");

        cmd.CommandText = """
            UPDATE StudentDrafts SET Payload=$payload, Revision=Revision+1
            WHERE SessionId=$session AND StudentCode=$student AND Revision=$revision
            """;
        cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(draft));
        cmd.Parameters.AddWithValue("$revision", draft.Revision);
        if (cmd.ExecuteNonQuery() != 1)
            throw new StudentDraftConflictException();

        tx.Commit();
        draft.Revision++;
    }
}
