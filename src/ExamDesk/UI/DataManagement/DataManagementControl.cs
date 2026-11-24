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

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
