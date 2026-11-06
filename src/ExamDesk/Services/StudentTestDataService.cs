using ExamDesk.Data;

namespace ExamDesk.Services;

// Tiện ích thử thủ công, không gọi khi khởi động hoặc tra cứu thông thường.
public sealed class StudentTestDataService
{
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
        transaction.Commit();
    }
}
