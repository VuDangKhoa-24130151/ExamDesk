#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

partial class DataManagementControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private Panel _exportCard = null!;
    private Label _lblExportInfo = null!;
    private Button _btnExportCsv = null!;
    private Panel _backupCard = null!;
    private Label _lblBackupInfo = null!;
    private Button _btnBackup = null!;
    private Button _btnRestore = null!;
    private Label _lblStatus = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _exportCard = new Panel();
        _lblExportInfo = new Label();
        _btnExportCsv = new Button();
        _backupCard = new Panel();
        _lblBackupInfo = new Label();
        _btnBackup = new Button();
        _btnRestore = new Button();
        _lblStatus = new Label();

        _rootLayout.SuspendLayout();
        _exportCard.SuspendLayout();
        _backupCard.SuspendLayout();
        SuspendLayout();

        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 4;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Hàng 0: Tiêu đề
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 16);
        _lblTitle.Text = "Xuất/nhập dữ liệu và Sao lưu (GV-15)";

        // Hàng 1: Khung Xuất kết quả thi
        _exportCard.Dock = DockStyle.Fill;
        _exportCard.AutoSize = true;
        _exportCard.BackColor = UiStyle.Surface;
        _exportCard.Padding = new Padding(16);
        _exportCard.Margin = new Padding(0, 0, 0, 16);

        _lblExportInfo.AutoSize = true;
        _lblExportInfo.Location = new Point(16, 16);
        _lblExportInfo.Text = "1. Xuất danh sách kết quả thi\nXuất bảng điểm các ca thi ra tệp CSV để lưu trữ hoặc mở bằng Excel.";

        _btnExportCsv.Text = "Xuất kết quả (.csv)";
        _btnExportCsv.Width = 180;
        _btnExportCsv.Location = new Point(16, 68);
        UiStyle.StylePrimaryButton(_btnExportCsv);

        _exportCard.Controls.Add(_lblExportInfo);
        _exportCard.Controls.Add(_btnExportCsv);

        // Hàng 2: Khung Sao lưu & Khôi phục CSDL SQLite
        _backupCard.Dock = DockStyle.Fill;
        _backupCard.AutoSize = true;
        _backupCard.BackColor = UiStyle.Surface;
        _backupCard.Padding = new Padding(16);
        _backupCard.Margin = new Padding(0, 0, 0, 16);

        _lblBackupInfo.AutoSize = true;
        _lblBackupInfo.Location = new Point(16, 16);
        _lblBackupInfo.Text = "2. Sao lưu & Khôi phục cơ sở dữ liệu cục bộ (SQLite)\nTạo bản sao lưu tệp examdesk.db hoặc khôi phục từ bản sao lưu có sẵn.";

        _btnBackup.Text = "Sao lưu CSDL (.db)";
        _btnBackup.Width = 180;
        _btnBackup.Location = new Point(16, 68);
        UiStyle.StylePrimaryButton(_btnBackup);

        _btnRestore.Text = "Khôi phục từ bản sao lưu...";
        _btnRestore.Width = 210;
        _btnRestore.Location = new Point(210, 68);
        UiStyle.StyleSecondaryButton(_btnRestore);

        _backupCard.Controls.Add(_lblBackupInfo);
        _backupCard.Controls.Add(_btnBackup);
        _backupCard.Controls.Add(_btnRestore);

        // Hàng 3: Nhãn phản hồi trạng thái
        _lblStatus.AutoSize = true;
        _lblStatus.ForeColor = UiStyle.TextMuted;
        _lblStatus.Text = "Trạng thái: Sẵn sàng.";

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_exportCard, 0, 1);
        _rootLayout.Controls.Add(_backupCard, 0, 2);
        _rootLayout.Controls.Add(_lblStatus, 0, 3);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "DataManagementControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _exportCard.ResumeLayout(false);
        _exportCard.PerformLayout();
        _backupCard.ResumeLayout(false);
        _backupCard.PerformLayout();
        ResumeLayout(false);
    }
}