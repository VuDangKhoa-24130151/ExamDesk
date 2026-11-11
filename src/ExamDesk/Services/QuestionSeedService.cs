using System.Text.Json;
using ExamDesk.Data;

namespace ExamDesk.Services;

// Bạn 1 phụ trách: Dịch vụ nạp dữ liệu câu hỏi và môn học mẫu vào SQLite thật.
public sealed class QuestionSeedService
{
    private sealed class ChapterMetadataEntry
    {
        public int SubjectId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private static string MetadataFilePath => Path.Combine(
        Path.GetDirectoryName(SqliteContext.DatabaseFilePath)!,
        "chapter_metadata.json");

    public (int subjectsAdded, int questionsAdded) SeedQuestionBank()
    {
        // 1. Đọc nội dung SQL từ EmbeddedResource
        using var stream = typeof(SqliteContext).Assembly.GetManifestResourceStream("ExamDesk.Data.seed_questions.sql")
            ?? throw new InvalidOperationException("Không tìm thấy tệp dữ liệu mẫu seed_questions.sql trong assembly. Hãy build lại ứng dụng.");

        using var reader = new StreamReader(stream);
        var sql = reader.ReadToEnd();

        // 2. Thực thi toàn bộ lệnh trong 1 Transaction trên SqliteContext.CreateConnection()
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Subjects; SELECT COUNT(*) FROM Questions;";
        var initialSubjects = 0;
        var initialQuestions = 0;
        using (var countReader = countCmd.ExecuteReader())
        {
            if (countReader.Read()) initialSubjects = countReader.GetInt32(0);
            if (countReader.NextResult() && countReader.Read()) initialQuestions = countReader.GetInt32(0);
        }

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
        transaction.Commit();

        // 3. Đếm số lượng sau khi nạp
        using var afterCmd = connection.CreateCommand();
        afterCmd.CommandText = "SELECT COUNT(*) FROM Subjects; SELECT COUNT(*) FROM Questions;";
        var afterSubjects = 0;
        var afterQuestions = 0;
        using (var afterReader = afterCmd.ExecuteReader())
        {
            if (afterReader.Read()) afterSubjects = afterReader.GetInt32(0);
            if (afterReader.NextResult() && afterReader.Read()) afterQuestions = afterReader.GetInt32(0);
        }

        var subjectsAdded = afterSubjects - initialSubjects;
        var questionsAdded = afterQuestions - initialQuestions;

        // 4. Đồng bộ tên chương cho các môn được nạp vào metadata
        SeedChapterMetadata(connection);

        return (subjectsAdded, questionsAdded);
    }

    private static void SeedChapterMetadata(Microsoft.Data.Sqlite.SqliteConnection connection)
    {
        try
        {
            var subjectIds = new Dictionary<string, int>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Code, Id FROM Subjects WHERE Code IN ('LTNET', 'CSDL', 'MMT');";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    subjectIds[reader.GetString(0)] = reader.GetInt32(1);
                }
            }

            var path = MetadataFilePath;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            List<ChapterMetadataEntry> entries = [];
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    entries = JsonSerializer.Deserialize<List<ChapterMetadataEntry>>(json) ?? [];
                }
                catch
                {
                    entries = [];
                }
            }

            void EnsureChapter(int subId, int num, string name)
            {
                if (!entries.Any(e => e.SubjectId == subId && e.Number == num))
                {
                    entries.Add(new ChapterMetadataEntry { SubjectId = subId, Number = num, Name = name });
                }
            }

            if (subjectIds.TryGetValue("LTNET", out var ltnetId))
            {
                EnsureChapter(ltnetId, 1, "Nền tảng .NET và ngôn ngữ C#");
                EnsureChapter(ltnetId, 2, "Thiết kế giao diện với WinForms");
                EnsureChapter(ltnetId, 3, "Truy vấn LINQ và lập trình bất đồng bộ");
                EnsureChapter(ltnetId, 4, "Truy cập dữ liệu với ADO.NET và SQLite");
            }

            if (subjectIds.TryGetValue("CSDL", out var csdlId))
            {
                EnsureChapter(csdlId, 1, "Mô hình cơ sở dữ liệu quan hệ");
                EnsureChapter(csdlId, 2, "Ngôn ngữ truy vấn có cấu trúc (SQL)");
                EnsureChapter(csdlId, 3, "Chuẩn hóa dữ liệu và giao dịch (Transaction)");
                EnsureChapter(csdlId, 4, "Chỉ mục, tối ưu hóa và an toàn dữ liệu");
            }

            if (subjectIds.TryGetValue("MMT", out var mmtId))
            {
                EnsureChapter(mmtId, 1, "Tổng quan mạng máy tính và mô hình OSI");
                EnsureChapter(mmtId, 2, "Tầng ứng dụng và các giao thức mạng");
                EnsureChapter(mmtId, 3, "Tầng giao vận (TCP/UDP)");
                EnsureChapter(mmtId, 4, "Địa chỉ IPv4 và chia mạng con (Subnetting)");
            }

            var serialized = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, serialized);
        }
        catch
        {
            // Bỏ qua lỗi ghi phụ nếu không ghi được tệp
        }
    }
}
