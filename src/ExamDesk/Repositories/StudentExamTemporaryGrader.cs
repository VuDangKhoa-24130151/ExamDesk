using ExamDesk.Data;
using ExamDesk.Models;
using System.Text.Json;

namespace ExamDesk.Repositories;

internal interface ISubmissionGrader
{
    // Chỉ trả về sau khi kết quả đã được lưu.
    void GradeAndSave(StudentDraft saved);
}

// Chấm tạm: đọc đáp án đúng trực tiếp từ Questions. Bạn 2 sẽ thay bằng SubmissionService.
internal sealed class StudentExamTemporaryGrader : ISubmissionGrader
{
    public void GradeAndSave(StudentDraft lockedDraft)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var tx = connection.BeginTransaction();
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.Parameters.AddWithValue("$session", lockedDraft.SessionId);
        cmd.Parameters.AddWithValue("$student", lockedDraft.StudentCode);
        // Gửi lại cùng lượt không tạo thêm kết quả đã commit.
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM ExamAttempts WHERE SessionId=$session AND StudentCode=$student)";
        if (Convert.ToInt32(cmd.ExecuteScalar()) != 0)
        {
            tx.Commit();
            return;
        }

        // Đọc lại bản đã khóa để tránh gửi dữ liệu cũ nếu lượt thi bị đặt lại.
        cmd.CommandText = "SELECT Payload FROM StudentDrafts WHERE SessionId=$session AND StudentCode=$student AND Revision=$revision";
        cmd.Parameters.AddWithValue("$revision", lockedDraft.Revision);
        var payload = cmd.ExecuteScalar() as string;
        var saved = payload is null ? null : JsonSerializer.Deserialize<StudentDraft>(payload);
        if (saved is not { PendingSubmission: true } || saved.StartedUtc != lockedDraft.StartedUtc)
            throw new InvalidOperationException("Bản nháp đã khóa không còn khớp với lượt thi.");
        cmd.Parameters.RemoveAt("$revision");

        cmd.Parameters.AddWithValue("$paper", saved.PaperId);
        cmd.CommandText = """
            SELECT q.Id, q.CorrectOption
            FROM ExamSessions s
            JOIN ExamPaperQuestions pq ON pq.PaperId = s.PaperId
            JOIN Questions q ON q.Id = pq.QuestionId
            WHERE s.Id = $session AND s.PaperId = $paper
            """;
        var answerKey = new Dictionary<int, string>();
        using (var reader = cmd.ExecuteReader())
            while (reader.Read())
                answerKey.Add(reader.GetInt32(0), reader.GetString(1));

        if (saved.Questions.Count == 0 || saved.Questions.Count != saved.Answers.Length ||
            saved.Questions.Any(question => !answerKey.ContainsKey(question.Id)) ||
            saved.Answers.Any(answer => answer is < 0 or > 3))
            throw new InvalidOperationException("Đề thi hoặc đáp án đã lưu không hợp lệ. Chưa thể nộp bài.");

        var answers = saved.Questions.Select((question, index) =>
        {
            var selected = saved.Answers[index] is int choice ? ((char)('A' + choice)).ToString() : "";
            var correct = answerKey[question.Id];
            return (QuestionId: question.Id, Order: index + 1, Selected: selected, Correct: correct);
        }).ToArray();
        var correctCount = answers.Count(answer => answer.Selected == answer.Correct);
        var submittedAt = saved.LockedAtUtc ?? DateTimeOffset.UtcNow;

        cmd.CommandText = """
            INSERT INTO ExamAttempts (SessionId, PaperId, StudentCode, StudentName, ClassName,
                StartedAt, SubmittedAt, TotalQuestions, CorrectCount, Score, IsAutoSubmitted)
            VALUES ($session, $paper, $student, $name, $class, $started, $submitted,
                $total, $correctCount, $score, $auto)
            RETURNING Id
            """;
        cmd.Parameters.AddWithValue("$name", saved.StudentName);
        cmd.Parameters.AddWithValue("$class", saved.ClassName);
        cmd.Parameters.AddWithValue("$started", saved.StartedUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$submitted", submittedAt.ToString("O"));
        cmd.Parameters.AddWithValue("$total", answers.Length);
        cmd.Parameters.AddWithValue("$correctCount", correctCount);
        cmd.Parameters.AddWithValue("$score", Math.Round(10.0 * correctCount / answers.Length, 2, MidpointRounding.AwayFromZero));
        cmd.Parameters.AddWithValue("$auto", saved.AutoSubmitted);
        var attemptId = Convert.ToInt64(cmd.ExecuteScalar());

        cmd.Parameters.Clear();
        cmd.CommandText = """
            INSERT INTO AttemptAnswers (AttemptId, QuestionId, QuestionOrder, SelectedOption, CorrectOption, IsCorrect)
            VALUES ($attempt, $question, $order, $selected, $correct, $isCorrect)
            """;
        cmd.Parameters.AddWithValue("$attempt", attemptId);
        var questionId = cmd.Parameters.Add("$question", Microsoft.Data.Sqlite.SqliteType.Integer);
        var order = cmd.Parameters.Add("$order", Microsoft.Data.Sqlite.SqliteType.Integer);
        var selectedOption = cmd.Parameters.Add("$selected", Microsoft.Data.Sqlite.SqliteType.Text);
        var correctOption = cmd.Parameters.Add("$correct", Microsoft.Data.Sqlite.SqliteType.Text);
        var isCorrect = cmd.Parameters.Add("$isCorrect", Microsoft.Data.Sqlite.SqliteType.Integer);
        foreach (var answer in answers)
        {
            questionId.Value = answer.QuestionId;
            order.Value = answer.Order;
            selectedOption.Value = answer.Selected;
            correctOption.Value = answer.Correct;
            isCorrect.Value = answer.Selected == answer.Correct;
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
    }
}
