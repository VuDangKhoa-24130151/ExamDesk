#nullable enable
using System.Globalization;
using System.Text;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

// Bạn 2: Màn hình xuất danh sách kết quả thi ra tệp CSV từ dữ liệu SQLite.
public partial class DataManagementControl : UserControl
{
    private readonly GradingReviewService _gradingService = new();

    public DataManagementControl()
    {
        InitializeComponent();
        RefreshSummary();
        _btnExportCsv.Click += (_, _) => ExportResultsCsv();
        _btnRefresh.Click += (_, _) =>
        {
            RefreshSummary();
            SetStatus("Đã cập nhật số liệu mới nhất từ cơ sở dữ liệu.", UiStyle.TextMuted, UiStyle.Surface);
        };
    }

    private void RefreshSummary()
    {
        int totalCount = _gradingService.GetTotalAttemptCount();
        bool hasData = totalCount > 0;

        _btnExportCsv.Enabled = hasData;
        _lblSummary.ForeColor = hasData ? UiStyle.Primary : UiStyle.WarningText;
        _lblSummary.BackColor = hasData ? UiStyle.Background : UiStyle.WarningBack;
        _lblSummary.Text = hasData
            ? $"Hiện có {totalCount} bài thi đã chấm sẵn sàng để xuất."
            : "Chưa có bài thi nào trong cơ sở dữ liệu để xuất.";
    }

    private void ExportResultsCsv()
    {
        var attempts = _gradingService.GetAttemptSummaries();
        if (attempts.Count == 0)
        {
            RefreshSummary();
            SetStatus("Chưa có bài thi nào để xuất ra tệp CSV.", UiStyle.WarningText, UiStyle.WarningBack);
            return;
        }

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
            SetStatus(
                $"Đã xuất {attempts.Count} kết quả thi ra tệp {Path.GetFileName(dialog.FileName)}.",
                UiStyle.SuccessText,
                UiStyle.SuccessBack);
        }
        catch (Exception ex)
        {
            SetStatus(
                $"Không thể ghi tệp CSV ({ex.Message}). Hãy đóng tệp nếu đang mở bằng Excel và thử lại.",
                UiStyle.ErrorText,
                UiStyle.ErrorBack);
        }
    }

    private void SetStatus(string message, Color textColor, Color backColor)
    {
        _statusBanner.BackColor = backColor;
        _lblStatus.ForeColor = textColor;
        _lblStatus.Text = $"Trạng thái: {message}";
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
