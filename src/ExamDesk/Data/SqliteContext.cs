using Microsoft.Data.Sqlite;

namespace ExamDesk.Data;

// File chung: Bạn 1 điều phối. Chỉ cung cấp điểm kết nối, chưa tạo bảng/nạp dữ liệu.
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
        // TODO (Bạn 1): tạo thư mục, đọc schema.sql và khởi tạo bảng trong transaction.
        // Chỉ gọi sau khi nhóm triển khai; không xóa database cũ hoặc tự nạp tài khoản mẫu.
        throw new NotImplementedException("Chưa triển khai khởi tạo SQLite.");
    }
}
