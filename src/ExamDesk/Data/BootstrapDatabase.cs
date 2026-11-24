namespace ExamDesk.Data;

internal static class BootstrapDatabase
{
    internal static void RestoreIfMissing(string databaseFilePath, string sourceDirectory)
    {
        // Không ghi đè database đã có, kể cả khi bộ dữ liệu trong bản build mới thay đổi.
        if (File.Exists(databaseFilePath))
        {
            return;
        }

        var sourceDatabase = Path.Combine(sourceDirectory, "examdesk.db");
        if (!File.Exists(sourceDatabase))
        {
            throw new FileNotFoundException(
                "Thiếu bộ database ban đầu. Hãy build lại hoặc sao chép đầy đủ thư mục ứng dụng.",
                sourceDatabase);
        }

        var targetDirectory = Path.GetDirectoryName(databaseFilePath)!;
        Directory.CreateDirectory(targetDirectory);
        var pendingDatabase = Path.Combine(targetDirectory, $".bootstrap-{Guid.NewGuid():N}.tmp");
        try
        {
            File.Copy(sourceDatabase, pendingDatabase);
            foreach (var sourceFile in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                if (string.Equals(sourceFile, sourceDatabase, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var destination = Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory, sourceFile));
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                // Có thể còn tệp phụ từ lần khởi tạo bị gián đoạn; database chưa tồn tại.
                File.Copy(sourceFile, destination, overwrite: true);
            }

            // Chỉ công bố database sau khi đã chép đủ metadata và ảnh.
            File.Move(pendingDatabase, databaseFilePath, overwrite: false);
        }
        finally
        {
            if (File.Exists(pendingDatabase))
            {
                File.Delete(pendingDatabase);
            }
        }
    }
}
