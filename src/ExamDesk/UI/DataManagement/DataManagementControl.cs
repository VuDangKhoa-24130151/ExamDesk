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
        _btnExportCsv.Click += (_, _) => ExportResultsCsv();
    }

    private void ExportResultsCsv()
    {
        // Đọc thẳng từ SQLite mỗi lần bấm xuất để luôn có dữ liệu mới nhất
        var attempts = _gradingService.GetAttemptSummaries();
        if (attempts.Count == 0)
        {
            SetStatus("Chưa có bài thi nào trong cơ sở dữ liệu để xuất.", UiStyle.WarningText, UiStyle.WarningBack);
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
                // Dùng InvariantCulture để dấu thập phân của điểm luôn là dấu chấm (.) thay vì bị đổi theo máy Windows
                string submittedAtText = a.SubmittedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string scoreText = a.Score.ToString("0.00", CultureInfo.InvariantCulture);

                sb.AppendLine(
                    $"{EscapeCsv(a.StudentCode)},{EscapeCsv(a.StudentName)},{EscapeCsv(a.ClassName)}," +
                    $"{EscapeCsv(a.SessionName)},{EscapeCsv(a.PaperTitle)},{a.TotalQuestions},{a.CorrectCount},{a.WrongCount},{a.BlankCount}," +
                    $"{scoreText},{EscapeCsv(a.SubmitMethodText)},{EscapeCsv(submittedAtText)}");
            }

            // Ghi UTF-8 có BOM (true) để mở bằng Excel không bị lỗi font tiếng Việt
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

        // Bọc ngoặc kép và nhân đôi dấu " bên trong để không vỡ cột khi tên/đề thi có dấu phẩy
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
