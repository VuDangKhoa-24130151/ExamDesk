#nullable enable
using System.Globalization;
using System.IO.Compression;
using System.Text;
using ExamDesk.Data;
using ExamDesk.Services;
using ExamDesk.UI.Shared;
using Microsoft.Data.Sqlite;

namespace ExamDesk.UI.DataManagement;

// Bạn 2: Màn hình xuất kết quả thi và sao lưu/khôi phục dữ liệu SQLite + ảnh + metadata.
public partial class DataManagementControl : UserControl
{
    private readonly GradingReviewService _gradingService = new();

    public DataManagementControl()
    {
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        _btnExportCsv.Click += (_, _) => ExportResultsCsv();
        _btnBackup.Click += (_, _) => BackupDatabase();
        _btnRestore.Click += (_, _) => RestoreDatabase();
    }

    private void ExportResultsCsv()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Xuất danh sách kết quả thi",
            Filter = "CSV UTF-8 (*.csv)|*.csv",
            FileName = $"KetQuaThi_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var attempts = _gradingService.GetAttemptSummaries();

            var sb = new StringBuilder();
            sb.AppendLine("MSSV,HoTen,Lop,CaThi,DeThi,SoCau,CauDung,CauSai,CauBoTrong,Diem,CachNop,NopLuc");

            foreach (var a in attempts)
            {
                string submittedAtText = a.SubmittedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string scoreText = a.Score.ToString("0.00", CultureInfo.InvariantCulture);

                sb.AppendLine(
                    $"{EscapeCsv(a.StudentCode)},{EscapeCsv(a.StudentName)},{EscapeCsv(a.ClassName)}," +
                    $"{EscapeCsv(a.SessionName)},{EscapeCsv(a.PaperTitle)},{a.TotalQuestions},{a.CorrectCount},{a.WrongCount},{a.BlankCount}," +
                    $"{scoreText},{EscapeCsv(a.SubmitMethodText)},{EscapeCsv(submittedAtText)}");
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
            _lblStatus.ForeColor = UiStyle.SuccessText;
            _lblStatus.Text = $"Trạng thái: Đã xuất {attempts.Count} kết quả thi từ SQLite ra tệp {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception ex)
        {
            _lblStatus.ForeColor = UiStyle.ErrorText;
            _lblStatus.Text = $"Trạng thái: Không thể xuất tệp CSV ({ex.Message}).";
        }
    }

    private void BackupDatabase()
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Chọn nơi lưu gói sao lưu dữ liệu",
            Filter = "Gói sao lưu UniExam (*.zip)|*.zip",
            FileName = $"examdesk_backup_{DateTime.Now:yyyyMMdd_HHmm}.zip"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        string stagingDir = Path.Combine(Path.GetTempPath(), $"examdesk_backup_{Guid.NewGuid():N}");
        string tempZip = Path.Combine(Path.GetTempPath(), $"examdesk_backup_{Guid.NewGuid():N}.zip");

        try
        {
            Directory.CreateDirectory(stagingDir);

            // 1. Hot-backup tệp SQLite đang chạy sang tệp tạm
            string stagedDb = Path.Combine(stagingDir, "examdesk.db");
            using (var sourceConnection = SqliteContext.CreateConnection())
            {
                sourceConnection.Open();
                var destConnString = new SqliteConnectionStringBuilder
                {
                    DataSource = stagedDb,
                    Pooling = false
                }.ToString();

                using var destConnection = new SqliteConnection(destConnString);
                destConnection.Open();
                sourceConnection.BackupDatabase(destConnection);
            }

            // 2. Sao chép thư mục images và các tệp metadata JSON đi kèm (nếu có)
            string databaseDir = Path.GetDirectoryName(SqliteContext.DatabaseFilePath)!;
            string sourceImagesDir = Path.Combine(databaseDir, "images");
            if (Directory.Exists(sourceImagesDir))
                CopyDirectory(sourceImagesDir, Path.Combine(stagingDir, "images"));

            if (Directory.Exists(databaseDir))
            {
                foreach (var jsonFile in Directory.EnumerateFiles(databaseDir, "*.json", SearchOption.TopDirectoryOnly))
                {
                    File.Copy(jsonFile, Path.Combine(stagingDir, Path.GetFileName(jsonFile)), overwrite: true);
                }
            }

            // 3. Đóng gói thành tệp .zip và thay thế tệp đích an toàn
            ZipFile.CreateFromDirectory(stagingDir, tempZip, CompressionLevel.Optimal, includeBaseDirectory: false);
            File.Move(tempZip, dialog.FileName, overwrite: true);

            _lblStatus.ForeColor = UiStyle.SuccessText;
            _lblStatus.Text = $"Trạng thái: Đã sao lưu CSDL, ảnh câu hỏi và metadata ra tệp {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception ex)
        {
            _lblStatus.ForeColor = UiStyle.ErrorText;
            _lblStatus.Text = $"Trạng thái: Sao lưu dữ liệu thất bại ({ex.Message}).";
        }
        finally
        {
            TryDeleteFile(tempZip);
            TryDeleteDirectory(stagingDir);
        }
    }

    private void RestoreDatabase()
    {
        using var openDialog = new OpenFileDialog
        {
            Title = "Chọn gói sao lưu để khôi phục",
            Filter = "Gói sao lưu UniExam (*.zip)|*.zip"
        };

        if (openDialog.ShowDialog(this) != DialogResult.OK)
            return;

        // Xác nhận trước khi khôi phục theo quy định G6/H3 trong DESIGN.md
        var confirm = MessageBox.Show(this,
            $"Khôi phục từ gói '{Path.GetFileName(openDialog.FileName)}' sẽ thay thế toàn bộ CSDL, ảnh câu hỏi và metadata hiện tại.\n\nBạn có chắc chắn muốn tiếp tục không?",
            "Xác nhận khôi phục bản sao lưu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            _lblStatus.ForeColor = UiStyle.TextMuted;
            _lblStatus.Text = "Trạng thái: Đã hủy thao tác khôi phục.";
            return;
        }

        string stagingDir = Path.Combine(Path.GetTempPath(), $"examdesk_restore_{Guid.NewGuid():N}");
        string? rollbackDb = null;

        try
        {
            Directory.CreateDirectory(stagingDir);
            ZipFile.ExtractToDirectory(openDialog.FileName, stagingDir);

            string stagedDb = Path.Combine(stagingDir, "examdesk.db");
            if (!File.Exists(stagedDb))
                throw new InvalidOperationException("Gói sao lưu không chứa tệp examdesk.db.");

            // Kiểm tra tính toàn vẹn của SQLite trước khi ghi đè dữ liệu hiện tại
            var verifyConnString = new SqliteConnectionStringBuilder
            {
                DataSource = stagedDb,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString();

            using (var verifyConn = new SqliteConnection(verifyConnString))
            {
                verifyConn.Open();
                using var checkCmd = verifyConn.CreateCommand();
                checkCmd.CommandText = "PRAGMA integrity_check;";
                var checkResult = checkCmd.ExecuteScalar() as string;
                if (!string.Equals(checkResult, "ok", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Tệp cơ sở dữ liệu trong bản sao lưu bị hỏng.");
            }

            // Bắt buộc giải phóng toàn bộ connection pool trước khi ghi đè tệp SQLite trên Windows
            SqliteConnection.ClearAllPools();

            string targetDb = SqliteContext.DatabaseFilePath;
            string targetDir = Path.GetDirectoryName(targetDb)!;
            Directory.CreateDirectory(targetDir);

            if (File.Exists(targetDb))
            {
                rollbackDb = Path.Combine(targetDir, $".restore_bak_{Guid.NewGuid():N}.db");
                File.Copy(targetDb, rollbackDb, overwrite: true);
            }

            TryDeleteFile(targetDb + "-wal");
            TryDeleteFile(targetDb + "-shm");
            File.Copy(stagedDb, targetDb, overwrite: true);

            string stagedImages = Path.Combine(stagingDir, "images");
            string targetImages = Path.Combine(targetDir, "images");
            if (Directory.Exists(stagedImages))
            {
                TryDeleteDirectory(targetImages);
                CopyDirectory(stagedImages, targetImages);
            }

            foreach (var jsonFile in Directory.EnumerateFiles(stagingDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                File.Copy(jsonFile, Path.Combine(targetDir, Path.GetFileName(jsonFile)), overwrite: true);
            }

            SqliteContext.InitializeDatabase();
            if (rollbackDb is not null)
                TryDeleteFile(rollbackDb);

            _lblStatus.ForeColor = UiStyle.SuccessText;
            _lblStatus.Text = $"Trạng thái: Đã khôi phục CSDL, ảnh và metadata thành công từ gói {Path.GetFileName(openDialog.FileName)}.";
        }
        catch (Exception ex)
        {
            if (rollbackDb is not null && File.Exists(rollbackDb))
            {
                try
                {
                    SqliteConnection.ClearAllPools();
                    File.Copy(rollbackDb, SqliteContext.DatabaseFilePath, overwrite: true);
                    TryDeleteFile(rollbackDb);
                }
                catch
                {
                    // Giữ lại thông báo lỗi chính
                }
            }

            _lblStatus.ForeColor = UiStyle.ErrorText;
            _lblStatus.Text = $"Trạng thái: Khôi phục dữ liệu thất bại ({ex.Message}).";
        }
        finally
        {
            TryDeleteDirectory(stagingDir);
        }
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);
        foreach (var file in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDir, file);
            var destFile = Path.Combine(destinationDir, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(file, destFile, overwrite: true);
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Bỏ qua lỗi dọn tệp tạm
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch
        {
            // Bỏ qua lỗi dọn thư mục tạm
        }
    }
}
