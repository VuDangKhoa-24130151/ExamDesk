#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

partial class DataManagementControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private TableLayoutPanel _exportCard = null!;
    private Label _lblExportInfo = null!;
    private FlowLayoutPanel _exportActions = null!;
    private Button _btnExportCsv = null!;
    private TableLayoutPanel _backupCard = null!;
    private Label _lblBackupInfo = null!;
    private FlowLayoutPanel _backupActions = null!;
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
        _exportCard = new TableLayoutPanel();
        _lblExportInfo = new Label();
        _exportActions = new FlowLayoutPanel();
        _btnExportCsv = new Button();
        _backupCard = new TableLayoutPanel();
        _lblBackupInfo = new Label();
        _backupActions = new FlowLayoutPanel();
        _btnBackup = new Button();
        _btnRestore = new Button();
        _lblStatus = new Label();

        _rootLayout.SuspendLayout();
        _exportCard.SuspendLayout();
        _exportActions.SuspendLayout();
        _backupCard.SuspendLayout();
        _backupActions.SuspendLayout();
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
        _lblTitle.Text = "Xuất/nhập dữ liệu và sao lưu";

        // Hàng 1: Khung Xuất kết quả thi (dùng TableLayoutPanel + FlowLayoutPanel co giãn theo DPI)
        _exportCard.Dock = DockStyle.Fill;
        _exportCard.AutoSize = true;
        _exportCard.BackColor = UiStyle.Surface;
        _exportCard.Padding = new Padding(16);
        _exportCard.Margin = new Padding(0, 0, 0, 16);
        _exportCard.ColumnCount = 1;
        _exportCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _exportCard.RowCount = 2;
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblExportInfo.AutoSize = true;
        _lblExportInfo.ForeColor = UiStyle.Text;
        _lblExportInfo.Margin = new Padding(0, 0, 0, 12);
        _lblExportInfo.Text = "1. Xuất danh sách kết quả thi\nXuất bảng điểm các ca thi từ SQLite ra tệp CSV để lưu trữ hoặc mở bằng Excel.";

        _exportActions.Dock = DockStyle.Fill;
        _exportActions.AutoSize = true;
        _exportActions.WrapContents = true;
        _exportActions.Margin = new Padding(0);

        _btnExportCsv.Text = "Xuất kết quả (.csv)";
        _btnExportCsv.Margin = new Padding(0, 0, 12, 4);
        UiStyle.StylePrimaryButton(_btnExportCsv);

        _exportActions.Controls.Add(_btnExportCsv);
        _exportCard.Controls.Add(_lblExportInfo, 0, 0);
        _exportCard.Controls.Add(_exportActions, 0, 1);

        // Hàng 2: Khung Sao lưu & Khôi phục dữ liệu
        _backupCard.Dock = DockStyle.Fill;
        _backupCard.AutoSize = true;
        _backupCard.BackColor = UiStyle.Surface;
        _backupCard.Padding = new Padding(16);
        _backupCard.Margin = new Padding(0, 0, 0, 16);
        _backupCard.ColumnCount = 1;
        _backupCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _backupCard.RowCount = 2;
        _backupCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _backupCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblBackupInfo.AutoSize = true;
        _lblBackupInfo.ForeColor = UiStyle.Text;
        _lblBackupInfo.Margin = new Padding(0, 0, 0, 12);
        _lblBackupInfo.Text = "2. Sao lưu & Khôi phục cơ sở dữ liệu cục bộ (SQLite, ảnh câu hỏi và metadata)\nTạo gói sao lưu (.zip) chứa examdesk.db, thư mục images và tệp metadata JSON, hoặc khôi phục từ gói đã sao lưu.";

        _backupActions.Dock = DockStyle.Fill;
        _backupActions.AutoSize = true;
        _backupActions.WrapContents = true;
        _backupActions.Margin = new Padding(0);

        _btnBackup.Text = "Sao lưu dữ liệu (.zip)";
        _btnBackup.Margin = new Padding(0, 0, 12, 4);
        UiStyle.StylePrimaryButton(_btnBackup);

        _btnRestore.Text = "Khôi phục từ bản sao lưu...";
        _btnRestore.Margin = new Padding(0, 0, 12, 4);
        UiStyle.StyleSecondaryButton(_btnRestore);

        _backupActions.Controls.Add(_btnBackup);
        _backupActions.Controls.Add(_btnRestore);
        _backupCard.Controls.Add(_lblBackupInfo, 0, 0);
        _backupCard.Controls.Add(_backupActions, 0, 1);

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
        _exportActions.ResumeLayout(false);
        _exportActions.PerformLayout();
        _backupCard.ResumeLayout(false);
        _backupCard.PerformLayout();
        _backupActions.ResumeLayout(false);
        _backupActions.PerformLayout();
        ResumeLayout(false);
    }
}
