namespace ExamDesk.Data;

internal static class BootstrapDatabase
{
    private const string ResourcePrefix = "ExamDesk.Bootstrap.";

    internal static void RestoreIfMissing(string databaseFilePath, string sourceDirectory)
    {
        // Không ghi đè database đã có, kể cả khi bộ dữ liệu trong bản build mới thay đổi.
        if (File.Exists(databaseFilePath))
        {
            return;
        }

        var sourceDatabase = Path.Combine(sourceDirectory, "examdesk.db");
        if (File.Exists(sourceDatabase))
        {
            // Nếu có thư mục database/bootstrap nằm cạnh app thì ưu tiên chép từ đó
            RestoreFromDirectory(databaseFilePath, sourceDirectory, sourceDatabase);
            return;
        }

        // Không có thư mục ngoài (bản đóng gói 1 file exe) thì tự giải nén từ resource nhúng sẵn trong app
        RestoreFromEmbeddedResources(databaseFilePath);
    }

    private static void RestoreFromDirectory(string databaseFilePath, string sourceDirectory, string sourceDatabase)
    {
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

    // Giải nén bộ db + ảnh + metadata đã nhúng trong file exe ra máy người dùng ở lần chạy đầu
    private static void RestoreFromEmbeddedResources(string databaseFilePath)
    {
        var assembly = typeof(BootstrapDatabase).Assembly;
        var resources = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal))
            .ToArray();

        if (resources.Length == 0)
        {
            throw new FileNotFoundException(
                "Thiếu bộ database ban đầu. Hãy build lại hoặc sao chép đầy đủ thư mục ứng dụng.",
                databaseFilePath);
        }

        var targetDirectory = Path.GetDirectoryName(databaseFilePath)!;
        Directory.CreateDirectory(targetDirectory);
        // Chép examdesk.db ra file tạm trước, giải nén xong hết ảnh và json mới đổi tên thành file chính
        var pendingDatabase = Path.Combine(targetDirectory, $".bootstrap-{Guid.NewGuid():N}.tmp");
        try
        {
            foreach (var resourceName in resources)
            {
                // Cắt bỏ phần prefix "ExamDesk.Bootstrap." để lấy lại đúng đường dẫn gốc (vd: images\ten_anh.jpg)
                var relativePath = resourceName[ResourcePrefix.Length..]
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);

                var destination = string.Equals(relativePath, "examdesk.db", StringComparison.OrdinalIgnoreCase)
                    ? pendingDatabase
                    : Path.Combine(targetDirectory, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                using var input = assembly.GetManifestResourceStream(resourceName)!;
                using var output = File.Create(destination);
                input.CopyTo(output);
            }

            if (!File.Exists(pendingDatabase))
            {
                throw new FileNotFoundException("Thiếu tệp examdesk.db nhúng trong ứng dụng.", databaseFilePath);
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
