using Microsoft.Data.Sqlite;

namespace ExamDesk.Data;

// File chung: Bạn 1 điều phối kết nối và khởi tạo schema, không nạp dữ liệu mẫu.
public static class SqliteContext
{
    public static string DatabaseFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ExamDesk", "database", "examdesk.db");

    public static SqliteConnection CreateConnection()
    {
        // Trả kết nối CHƯA mở. Repository chịu trách nhiệm Open/Dispose.
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = DatabaseFilePath,
            ForeignKeys = true
        };
        return new SqliteConnection(builder.ToString());
    }

    public static void InitializeDatabase()
    {
        // Nhúng schema vào assembly để chạy được cả khi đổi thư mục hoặc đóng gói app.
        using var stream = typeof(SqliteContext).Assembly
            .GetManifestResourceStream("ExamDesk.Data.schema.sql")
            ?? throw new InvalidOperationException("Không tìm thấy schema SQLite trong ứng dụng.");
        using var reader = new StreamReader(stream);
        var schema = reader.ReadToEnd();

        Directory.CreateDirectory(Path.GetDirectoryName(DatabaseFilePath)!);
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = schema;
        command.ExecuteNonQuery();
        transaction.Commit();
        // IF NOT EXISTS giữ dữ liệu cũ; thay đổi cấu trúc về sau cần migration riêng.
    }
}
