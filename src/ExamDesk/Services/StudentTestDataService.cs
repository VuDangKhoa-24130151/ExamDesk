using ExamDesk.Data;

namespace ExamDesk.Services;

// Dữ liệu test cục bộ được nạp thủ công bằng tham số chạy.
public sealed class StudentTestDataService
{
    public ExamDesk.Models.ExamSession PrepareExamSession()
    {
        Load();
        var service = new ExamSessionService();
        var paper = service.GetExamPapers().Single(p => p.PaperCode == "TEST_UI30");
        var session = new ExamDesk.Models.ExamSession
        {
            SessionCode = "TEST_CA_" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
            SessionName = "Bài thi thử C# và SQLite",
            PaperId = paper.Id,
            RoomName = "Phòng thử 01"
        };
        session.Id = service.CreateSession(session);
        service.SetSessionOpen(session.Id, true);
        session.IsOpen = true;
        return session;
    }

    public void Load()
    {
        using var stream = typeof(SqliteContext).Assembly.GetManifestResourceStream("ExamDesk.Data.seed_student_test.sql")
            ?? throw new InvalidOperationException("Không tìm thấy dữ liệu thử. Hãy build lại ứng dụng.");
        using var reader = new StreamReader(stream);
        var sql = reader.ReadToEnd().Replace("BEGIN IMMEDIATE;", "").Replace("COMMIT;", "");
        using var connection = SqliteContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Role='Student' AND StudentCode IN ('24130151','24130152')";
        if (Convert.ToInt32(command.ExecuteScalar()) != 2)
            throw new InvalidOperationException("Dữ liệu mẫu xung đột mã tài khoản hoặc MSSV. Chưa lưu thay đổi.");
        command.CommandText = "SELECT COUNT(*) FROM ExamPaperQuestions ep JOIN ExamPapers p ON p.Id=ep.PaperId JOIN Questions q ON q.Id=ep.QuestionId WHERE p.PaperCode='TEST_UI30' AND length(trim(q.OptionA))>0 AND length(trim(q.OptionB))>0 AND length(trim(q.OptionC))>0 AND length(trim(q.OptionD))>0";
        if (Convert.ToInt32(command.ExecuteScalar()) != 30)
            throw new InvalidOperationException("Đề TEST_UI30 chưa đủ 30 câu với 4 đáp án. Chưa lưu dữ liệu thử.");
        transaction.Commit();
    }
}
