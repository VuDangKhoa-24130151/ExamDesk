using System.Text;
using ExamDesk.UI.Reports;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

// Bạn 2: Màn hình GV-15 xuất kết quả thi và sao lưu/khôi phục CSDL.
public partial class DataManagementControl : UserControl
{
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

        var rows = ReportsPreviewData.Attempts();
        var sb = new StringBuilder();
        sb.AppendLine("MSSV,HoTen,Lop,CaThi,SoCau,CauDung,CauSai,CauBoTrong,Diem,CachNop");
        foreach (var r in rows)
            sb.AppendLine($"{r.Mssv},\"{r.HoTen}\",{r.Lop},\"{r.Ca}\",{r.SoCau},{r.CauDung},{r.CauSai},{r.CauBoTrong},{r.Diem:0.00},\"{r.CachNop}\"");

        File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
        _lblStatus.ForeColor = UiStyle.SuccessText;
        _lblStatus.Text = $"Trạng thái: Đã xuất {rows.Count} kết quả ra tệp {Path.GetFileName(dialog.FileName)}.";
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

        _lblStatus.ForeColor = UiStyle.SuccessText;
        _lblStatus.Text = $"Trạng thái: Đã chọn vị trí sao lưu {Path.GetFileName(dialog.FileName)} (chế độ minh họa UI).";
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

        _lblStatus.ForeColor = UiStyle.WarningText;
        _lblStatus.Text = $"Trạng thái: Đã xác nhận khôi phục từ {Path.GetFileName(openDialog.FileName)} (chờ nối nghiệp vụ CSDL).";
    }
}