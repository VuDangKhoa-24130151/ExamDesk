using System.Globalization;
using System.Text;
using ExamDesk.Data;
using ExamDesk.Services;
using ExamDesk.UI.Reports;
using ExamDesk.UI.Shared;
using Microsoft.Data.Sqlite;

namespace ExamDesk.UI.DataManagement;

// Bạn 2: Màn hình GV-15 xuất kết quả thi và sao lưu/khôi phục CSDL.
public partial class DataManagementControl : UserControl
{
    private readonly GradingReviewService _gradingService = new();
    private readonly ExamSessionService _sessionService = new();

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
            var sessionMap = _sessionService.GetSessions()
                .ToDictionary(s => s.Id, s => $"{s.SessionCode} - {s.SessionName}");
            var attempts = _gradingService.GetAttempts();

            var sb = new StringBuilder();
            sb.AppendLine("MSSV,HoTen,Lop,CaThi,SoCau,CauDung,CauSai,CauBoTrong,Diem,CachNop,NopLuc");

            foreach (var a in attempts)
            {
                var answers = _gradingService.GetAttemptAnswers(a.Id);
                int answeredCount = answers.Count(ans => !string.IsNullOrWhiteSpace(ans.SelectedOption));
                int blankCount = Math.Max(0, a.TotalQuestions - answeredCount);
                int wrongCount = Math.Max(0, a.TotalQuestions - a.CorrectCount - blankCount);

                string sessionLabel = sessionMap.TryGetValue(a.SessionId, out var name)
                    ? name
                    : $"Ca #{a.SessionId}";
                string submitWay = a.IsAutoSubmitted ? ReportsPreviewData.AutoSubmit : ReportsPreviewData.ManualSubmit;
                string submittedAtText = a.SubmittedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string scoreText = a.Score.ToString("0.00", CultureInfo.InvariantCulture);

                sb.AppendLine(
                    $"{EscapeCsv(a.StudentCode)},{EscapeCsv(a.StudentName)},{EscapeCsv(a.ClassName)}," +
                    $"{EscapeCsv(sessionLabel)},{a.TotalQuestions},{a.CorrectCount},{wrongCount},{blankCount}," +
                    $"{scoreText},{EscapeCsv(submitWay)},{EscapeCsv(submittedAtText)}");
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
            Title = "Chọn nơi lưu bản sao lưu CSDL",
            Filter = "SQLite Database (*.db)|*.db",
            FileName = $"examdesk_backup_{DateTime.Now:yyyyMMdd_HHmm}.db"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        if (IsSameFilePath(dialog.FileName, SqliteContext.DatabaseFilePath))
        {
            _lblStatus.ForeColor = UiStyle.WarningText;
            _lblStatus.Text = "Trạng thái: Vui lòng chọn tệp đích khác với tệp CSDL đang chạy.";
            return;
        }

        try
        {
            if (File.Exists(dialog.FileName))
                File.Delete(dialog.FileName);

            using (var sourceConnection = SqliteContext.CreateConnection())
            {
                sourceConnection.Open();
                var destConnString = new SqliteConnectionStringBuilder
                {
                    DataSource = dialog.FileName,
                    Pooling = false
                }.ToString();

                using var destConnection = new SqliteConnection(destConnString);
                destConnection.Open();
                sourceConnection.BackupDatabase(destConnection);
            }

            _lblStatus.ForeColor = UiStyle.SuccessText;
            _lblStatus.Text = $"Trạng thái: Đã sao lưu CSDL thành công ra tệp {Path.GetFileName(dialog.FileName)}.";
        }
        catch (Exception ex)
        {
            _lblStatus.ForeColor = UiStyle.ErrorText;
            _lblStatus.Text = $"Trạng thái: Sao lưu CSDL thất bại ({ex.Message}).";
        }
    }

    private void RestoreDatabase()
    {
        using var openDialog = new OpenFileDialog
        {
            Title = "Chọn tệp sao lưu CSDL để khôi phục",
            Filter = "SQLite Database (*.db)|*.db"
        };

        if (openDialog.ShowDialog(this) != DialogResult.OK)
            return;

        if (IsSameFilePath(openDialog.FileName, SqliteContext.DatabaseFilePath))
        {
            _lblStatus.ForeColor = UiStyle.WarningText;
            _lblStatus.Text = "Trạng thái: Tệp đã chọn trùng với tệp CSDL đang sử dụng.";
            return;
        }

        // Xác nhận trước khi khôi phục theo quy định G6/H3 trong DESIGN.md
        var confirm = MessageBox.Show(this,
            $"Khôi phục từ tệp '{Path.GetFileName(openDialog.FileName)}' sẽ thay thế dữ liệu hiện tại.\n\nBạn có chắc chắn muốn tiếp tục không?",
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

        try
        {
            var sourceConnString = new SqliteConnectionStringBuilder
            {
                DataSource = openDialog.FileName,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString();

            using (var sourceConnection = new SqliteConnection(sourceConnString))
            {
                sourceConnection.Open();
                using (var checkCmd = sourceConnection.CreateCommand())
                {
                    checkCmd.CommandText = "PRAGMA integrity_check;";
                    var checkResult = checkCmd.ExecuteScalar() as string;
                    if (!string.Equals(checkResult, "ok", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Tệp sao lưu không hợp lệ hoặc bị hỏng.");
                }

                using var destConnection = SqliteContext.CreateConnection();
                destConnection.Open();
                sourceConnection.BackupDatabase(destConnection);
            }

            SqliteContext.InitializeDatabase();
            _lblStatus.ForeColor = UiStyle.SuccessText;
            _lblStatus.Text = $"Trạng thái: Đã khôi phục CSDL thành công từ tệp {Path.GetFileName(openDialog.FileName)}.";
        }
        catch (Exception ex)
        {
            _lblStatus.ForeColor = UiStyle.ErrorText;
            _lblStatus.Text = $"Trạng thái: Khôi phục CSDL thất bại ({ex.Message}).";
        }
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static bool IsSameFilePath(string pathA, string pathB)
        => string.Equals(Path.GetFullPath(pathA), Path.GetFullPath(pathB), StringComparison.OrdinalIgnoreCase);
}