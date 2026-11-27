using System.Globalization;
using System.Text.Json;
using ExamDesk.Data;
using ExamDesk.Models;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Repositories;

// Bạn 2 phụ trách: Lưu kết quả lượt thi và chi tiết từng câu vào SQLite trong cùng một transaction.
public sealed class SubmissionRepository
{
    public ExamAttempt? FindExistingAttempt(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        return FindExistingAttemptCore(command, sessionId, studentCode);
    }

    public bool IsSessionMatchingPaper(int sessionId, int paperId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        return IsSessionMatchingPaperCore(command, sessionId, paperId);
    }

    internal ExamAttempt GradeAndSaveLockedDraft(
        StudentDraft lockedDraft,
        Func<int, List<AnswerKeyItem>> getAnswerKey)
    {
        ArgumentNullException.ThrowIfNull(lockedDraft);
        ArgumentNullException.ThrowIfNull(getAnswerKey);

        string studentCode = (lockedDraft.StudentCode ?? string.Empty).Trim();
        if (lockedDraft.SessionId <= 0 || lockedDraft.PaperId <= 0 || string.IsNullOrWhiteSpace(studentCode))
            throw new ArgumentException("Thông tin ca thi, đề thi hoặc mã sinh viên không hợp lệ.");

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        // Dùng BEGIN IMMEDIATE (deferred: false) để giữ khóa ghi ngay khi bắt đầu transaction,
        // tránh trường hợp bấm nộp tay trùng đúng lúc hết giờ tự nộp gây ghi 2 lần.
        using var transaction = connection.BeginTransaction(deferred: false);

        try
        {
            // 1. Nếu (SessionId, StudentCode) đã có trong ExamAttempts thì trả về kết quả cũ ngay.
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.Transaction = transaction;
                var existing = FindExistingAttemptCore(checkCmd, lockedDraft.SessionId, studentCode);
                if (existing is not null)
                {
                    transaction.Commit();
                    return existing;
                }
            }

            // 2. Đọc lại bản nháp từ DB theo đúng Revision đã khóa để chắc chắn dữ liệu không bị cũ/lệch lượt.
            var saved = ReadLockedDraftCore(connection, transaction, lockedDraft.SessionId, studentCode, lockedDraft.Revision);
            if (saved is not { PendingSubmission: true } || saved.StartedUtc != lockedDraft.StartedUtc)
                throw new InvalidOperationException("Bản nháp đã khóa không còn khớp với lượt thi.");

            // 3. Kiểm tra ca thi và mã đề trong ExamSessions phải khớp nhau.
            using (var verifyCmd = connection.CreateCommand())
            {
                verifyCmd.Transaction = transaction;
                if (!IsSessionMatchingPaperCore(verifyCmd, saved.SessionId, saved.PaperId))
                    throw new InvalidOperationException("Ca thi không tồn tại hoặc mã đề thi không khớp với ca thi.");
            }

            // 4. Chấm điểm dựa trên bảng đáp án chuẩn từ ExamPaperService, không dùng đáp án phía UI.
            var answerKey = getAnswerKey(saved.PaperId);
            var (attemptAnswers, correctCount) = GradeDraftAgainstAnswerKey(saved, answerKey);

            int totalQuestions = answerKey.Count;
            double score = Math.Round(correctCount * 10.0 / totalQuestions, 2, MidpointRounding.AwayFromZero);

            // Nếu hết giờ tự nộp thì không để thời điểm nộp vượt quá DeadlineUtc.
            var submittedAt = saved.LockedAtUtc ?? (saved.AutoSubmitted ? saved.DeadlineUtc : DateTimeOffset.UtcNow);
            if (saved.AutoSubmitted && saved.DeadlineUtc != default && submittedAt > saved.DeadlineUtc)
                submittedAt = saved.DeadlineUtc;

            // Tự động nộp trước hạn (AutoSubmitted == true nhưng SubmittedAt < DeadlineUtc) là do vi phạm 3 lần.
            bool isViolationAutoSubmit = saved.AutoSubmitted && saved.DeadlineUtc != default && submittedAt < saved.DeadlineUtc;

            var attempt = new ExamAttempt
            {
                SessionId = saved.SessionId,
                PaperId = saved.PaperId,
                StudentCode = saved.StudentCode.Trim(),
                StudentName = (saved.StudentName ?? string.Empty).Trim(),
                ClassName = (saved.ClassName ?? string.Empty).Trim(),
                StartedAt = saved.StartedUtc == default ? submittedAt : saved.StartedUtc,
                SubmittedAt = submittedAt,
                TotalQuestions = totalQuestions,
                CorrectCount = correctCount,
                Score = score,
                IsAutoSubmitted = saved.AutoSubmitted,
                ReviewNote = isViolationAutoSubmit ? "VIOLATION_3" : string.Empty
            };

            InsertAttemptAndAnswers(connection, transaction, attempt, attemptAnswers);

            // Cố tình giữ lại dòng StudentDrafts (không DELETE) để màn Phúc khảo đọc được snapshot câu hỏi và History đổi đáp án.
            transaction.Commit();
            return attempt;
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            // Mã lỗi 2067 = SQLITE_CONSTRAINT_UNIQUE: nếu vẫn đụng trùng (SessionId, StudentCode) thì trả về bản đã lưu.
            transaction.Rollback();
            var existing = FindExistingAttempt(lockedDraft.SessionId, studentCode);
            if (existing is not null)
                return existing;
            throw;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public int SaveAttemptWithAnswers(ExamAttempt attempt, List<AttemptAnswer> answers)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        ArgumentNullException.ThrowIfNull(answers);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction(deferred: false);

        try
        {
            using (var verifyCmd = connection.CreateCommand())
            {
                verifyCmd.Transaction = transaction;
                if (!IsSessionMatchingPaperCore(verifyCmd, attempt.SessionId, attempt.PaperId))
                    throw new InvalidOperationException("Ca thi không tồn tại hoặc mã đề thi không khớp với ca thi.");
            }

            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.Transaction = transaction;
                var existing = FindExistingAttemptCore(checkCmd, attempt.SessionId, attempt.StudentCode);
                if (existing is not null)
                {
                    transaction.Commit();
                    attempt.Id = existing.Id;
                    return attempt.Id;
                }
            }

            InsertAttemptAndAnswers(connection, transaction, attempt, answers);
            transaction.Commit();
            return attempt.Id;
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            transaction.Rollback();
            var existing = FindExistingAttempt(attempt.SessionId, attempt.StudentCode);
            if (existing is not null)
            {
                attempt.Id = existing.Id;
                return attempt.Id;
            }
            throw;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    internal static string NormalizeOption(string? option)
    {
        if (string.IsNullOrWhiteSpace(option))
            return string.Empty;

        string upper = option.Trim().ToUpperInvariant();
        return upper is "A" or "B" or "C" or "D" ? upper : string.Empty;
    }

    private static bool IsSessionMatchingPaperCore(SqliteCommand command, int sessionId, int paperId)
    {
        command.Parameters.Clear();
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM ExamSessions WHERE Id = $sessionId AND PaperId = $paperId);";
        command.Parameters.AddWithValue("$sessionId", sessionId);
        command.Parameters.AddWithValue("$paperId", paperId);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    private static StudentDraft? ReadLockedDraftCore(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int sessionId,
        string studentCode,
        int revision)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = @"
            SELECT Payload
            FROM StudentDrafts
            WHERE SessionId = $sessionId
              AND UPPER(TRIM(StudentCode)) = UPPER(TRIM($studentCode))
              AND Revision = $revision
            LIMIT 1;";
        cmd.Parameters.AddWithValue("$sessionId", sessionId);
        cmd.Parameters.AddWithValue("$studentCode", studentCode);
        cmd.Parameters.AddWithValue("$revision", revision);

        var payload = cmd.ExecuteScalar() as string;
        return string.IsNullOrWhiteSpace(payload) ? null : JsonSerializer.Deserialize<StudentDraft>(payload);
    }

    private static (List<AttemptAnswer> Answers, int CorrectCount) GradeDraftAgainstAnswerKey(
        StudentDraft saved,
        List<AnswerKeyItem> answerKey)
    {
        if (answerKey.Count == 0)
            throw new InvalidOperationException("Không tìm thấy đáp án chuẩn của đề thi để chấm điểm.");

        if (saved.Questions.Count == 0 ||
            saved.Questions.Count != saved.Answers.Length ||
            saved.Questions.Count != answerKey.Count)
        {
            throw new InvalidOperationException("Đề thi hoặc danh sách câu trả lời đã lưu không hợp lệ. Chưa thể nộp bài.");
        }

        // Map lựa chọn từ chỉ số 0..3 sang "A".."D" theo QuestionId (câu bỏ trống lưu "").
        var selectedByQuestionId = new Dictionary<int, string>(saved.Questions.Count);
        for (int i = 0; i < saved.Questions.Count; i++)
        {
            int? choice = saved.Answers[i];
            if (choice is < 0 or > 3)
                throw new InvalidOperationException("Đáp án đã lưu không hợp lệ.");

            selectedByQuestionId[saved.Questions[i].Id] = choice.HasValue ? ((char)('A' + choice.Value)).ToString() : string.Empty;
        }

        var answerKeyIds = answerKey.Select(k => k.QuestionId).ToHashSet();
        if (saved.Questions.Any(q => !answerKeyIds.Contains(q.Id)))
            throw new InvalidOperationException("Danh sách câu hỏi trong bản nháp không khớp với đề thi chuẩn.");

        var attemptAnswers = new List<AttemptAnswer>(answerKey.Count);
        int correctCount = 0;

        foreach (var keyItem in answerKey.OrderBy(k => k.OrderIndex))
        {
            string selected = selectedByQuestionId.TryGetValue(keyItem.QuestionId, out var opt) ? opt : string.Empty;
            string correct = NormalizeOption(keyItem.CorrectOption);
            bool isCorrect = selected.Length > 0 && selected == correct;
            if (isCorrect)
                correctCount++;

            attemptAnswers.Add(new AttemptAnswer
            {
                QuestionId = keyItem.QuestionId,
                QuestionOrder = keyItem.OrderIndex,
                SelectedOption = selected,
                CorrectOption = correct,
                IsCorrect = isCorrect
            });
        }

        return (attemptAnswers, correctCount);
    }

    private static ExamAttempt? FindExistingAttemptCore(SqliteCommand command, int sessionId, string studentCode)
    {
        command.CommandText = @"
            SELECT Id, SessionId, PaperId, StudentCode, StudentName, ClassName,
                   StartedAt, SubmittedAt, TotalQuestions, CorrectCount, Score,
                   IsAutoSubmitted, ReviewNote
            FROM ExamAttempts
            WHERE SessionId = $sessionId
              AND UPPER(TRIM(StudentCode)) = UPPER(TRIM($studentCode))
            LIMIT 1;";
        command.Parameters.Clear();
        command.Parameters.AddWithValue("$sessionId", sessionId);
        command.Parameters.AddWithValue("$studentCode", (studentCode ?? string.Empty).Trim());

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

    private static void InsertAttemptAndAnswers(
        SqliteConnection connection,
        SqliteTransaction transaction,
        ExamAttempt attempt,
        List<AttemptAnswer> answers)
    {
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

        using var insertAnswerCmd = connection.CreateCommand();
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
}
