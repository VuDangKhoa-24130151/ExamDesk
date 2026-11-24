using System.Globalization;
using ExamDesk.Data;
using ExamDesk.Models;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách: Lưu kết quả lượt thi và chi tiết từng câu vào SQLite trong cùng một transaction.
public sealed class SubmissionRepository
{
    // Kiểm tra sinh viên đã có kết quả đã chốt ở ca thi này chưa (chống nộp lặp)
    public ExamAttempt? FindExistingAttempt(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, SessionId, PaperId, StudentCode, StudentName, ClassName,
                   StartedAt, SubmittedAt, TotalQuestions, CorrectCount, Score,
                   IsAutoSubmitted, ReviewNote
            FROM ExamAttempts
            WHERE SessionId = $sessionId AND StudentCode = $studentCode
            LIMIT 1;";
        command.Parameters.AddWithValue("$sessionId", sessionId);
        command.Parameters.AddWithValue("$studentCode", studentCode.Trim());

        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;

        return new ExamAttempt
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
        };
    }

    // Kiểm tra ca thi có tồn tại và khớp với mã đề thi được gửi lên hay không (schema.sql TODO Bạn 2)
    public bool IsSessionMatchingPaper(int sessionId, int paperId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EXISTS(
                SELECT 1 FROM ExamSessions
                WHERE Id = $sessionId AND PaperId = $paperId
            );";
        command.Parameters.AddWithValue("$sessionId", sessionId);
        command.Parameters.AddWithValue("$paperId", paperId);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    // Lưu bài nộp và toàn bộ câu trả lời trong 1 transaction duy nhất
    public int SaveAttemptWithAnswers(ExamAttempt attempt, List<AttemptAnswer> answers)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(answers);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 0. Kiểm tra ca thi và đề thi phải khớp nhau ngay trong transaction
            using (var verifySessionCmd = connection.CreateCommand())
            {
                verifySessionCmd.Transaction = transaction;
                verifySessionCmd.CommandText = @"
                    SELECT EXISTS(
                        SELECT 1 FROM ExamSessions
                        WHERE Id = $sessionId AND PaperId = $paperId
                    );";
                verifySessionCmd.Parameters.AddWithValue("$sessionId", attempt.SessionId);
                verifySessionCmd.Parameters.AddWithValue("$paperId", attempt.PaperId);
                if (Convert.ToInt32(verifySessionCmd.ExecuteScalar()) != 1)
                {
                    throw new InvalidOperationException("Ca thi không tồn tại hoặc mã đề thi không khớp với ca thi.");
                }
            }

            // 1. Kiểm tra chống nộp lặp ngay trong transaction
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = @"
                    SELECT Id FROM ExamAttempts
                    WHERE SessionId = $sessionId AND StudentCode = $studentCode
                    LIMIT 1;";
                checkCmd.Parameters.AddWithValue("$sessionId", attempt.SessionId);
                checkCmd.Parameters.AddWithValue("$studentCode", attempt.StudentCode.Trim());
                var existingId = checkCmd.ExecuteScalar();
                if (existingId is not null && existingId != DBNull.Value)
                {
                    transaction.Commit();
                    attempt.Id = Convert.ToInt32(existingId);
                    return attempt.Id;
                }
            }

            // 2. Thêm dòng kết quả tổng vào bảng ExamAttempts
            using (var insertAttemptCmd = connection.CreateCommand())
            {
                insertAttemptCmd.Transaction = transaction;
                insertAttemptCmd.CommandText = @"
                    INSERT INTO ExamAttempts (
                        SessionId, PaperId, StudentCode, StudentName, ClassName,
                        StartedAt, SubmittedAt, TotalQuestions, CorrectCount, Score,
                        IsAutoSubmitted, ReviewNote
                    )
                    VALUES (
                        $sessionId, $paperId, $studentCode, $studentName, $className,
                        $startedAt, $submittedAt, $totalQuestions, $correctCount, $score,
                        $isAutoSubmitted, $reviewNote
                    );
                    SELECT last_insert_rowid();";
                insertAttemptCmd.Parameters.AddWithValue("$sessionId", attempt.SessionId);
                insertAttemptCmd.Parameters.AddWithValue("$paperId", attempt.PaperId);
                insertAttemptCmd.Parameters.AddWithValue("$studentCode", attempt.StudentCode.Trim());
                insertAttemptCmd.Parameters.AddWithValue("$studentName", attempt.StudentName.Trim());
                insertAttemptCmd.Parameters.AddWithValue("$className", attempt.ClassName.Trim());
                insertAttemptCmd.Parameters.AddWithValue("$startedAt", attempt.StartedAt.ToString("O", CultureInfo.InvariantCulture));
                insertAttemptCmd.Parameters.AddWithValue("$submittedAt", attempt.SubmittedAt.ToString("O", CultureInfo.InvariantCulture));
                insertAttemptCmd.Parameters.AddWithValue("$totalQuestions", attempt.TotalQuestions);
                insertAttemptCmd.Parameters.AddWithValue("$correctCount", attempt.CorrectCount);
                insertAttemptCmd.Parameters.AddWithValue("$score", attempt.Score);
                insertAttemptCmd.Parameters.AddWithValue("$isAutoSubmitted", attempt.IsAutoSubmitted ? 1 : 0);
                insertAttemptCmd.Parameters.AddWithValue("$reviewNote", attempt.ReviewNote ?? string.Empty);

                attempt.Id = Convert.ToInt32(insertAttemptCmd.ExecuteScalar());
            }

            // 3. Thêm chi tiết từng câu vào bảng AttemptAnswers
            using (var insertAnswerCmd = connection.CreateCommand())
            {
                insertAnswerCmd.Transaction = transaction;
                insertAnswerCmd.CommandText = @"
                    INSERT INTO AttemptAnswers (
                        AttemptId, QuestionId, QuestionOrder,
                        SelectedOption, CorrectOption, IsCorrect
                    )
                    VALUES (
                        $attemptId, $questionId, $questionOrder,
                        $selectedOption, $correctOption, $isCorrect
                    );";

                var pAttemptId = insertAnswerCmd.Parameters.Add("$attemptId", SqliteType.Integer);
                var pQuestionId = insertAnswerCmd.Parameters.Add("$questionId", SqliteType.Integer);
                var pOrder = insertAnswerCmd.Parameters.Add("$questionOrder", SqliteType.Integer);
                var pSelected = insertAnswerCmd.Parameters.Add("$selectedOption", SqliteType.Text);
                var pCorrect = insertAnswerCmd.Parameters.Add("$correctOption", SqliteType.Text);
                var pIsCorrect = insertAnswerCmd.Parameters.Add("$isCorrect", SqliteType.Integer);

                pAttemptId.Value = attempt.Id;
                foreach (var ans in answers)
                {
                    ans.AttemptId = attempt.Id;
                    pQuestionId.Value = ans.QuestionId;
                    pOrder.Value = ans.QuestionOrder;
                    pSelected.Value = ans.SelectedOption ?? string.Empty;
                    pCorrect.Value = ans.CorrectOption;
                    pIsCorrect.Value = ans.IsCorrect ? 1 : 0;
                    insertAnswerCmd.ExecuteNonQuery();
                }
            }

            // Giữ lại bản ghi trong StudentDrafts (đã khóa PendingSubmission = true)
            // để bảo toàn lịch sử đổi đáp án (History) phục vụ tra cứu và phúc khảo.
            transaction.Commit();
            return attempt.Id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}