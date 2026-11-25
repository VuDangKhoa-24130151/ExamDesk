#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

partial class DataManagementControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblSubtitle = null!;
    private Panel _exportCardBorder = null!;
    private TableLayoutPanel _exportCard = null!;
    private Label _lblExportHeading = null!;
    private Label _lblExportInfo = null!;
    private Label _lblSummary = null!;
    private FlowLayoutPanel _exportActions = null!;
    private Button _btnExportCsv = null!;
    private Button _btnRefresh = null!;
    private Panel _statusBanner = null!;
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
        _lblSubtitle = new Label();
        _exportCardBorder = new Panel();
        _exportCard = new TableLayoutPanel();
        _lblExportHeading = new Label();
        _lblExportInfo = new Label();
        _lblSummary = new Label();
        _exportActions = new FlowLayoutPanel();
        _btnExportCsv = new Button();
        _btnRefresh = new Button();
        _statusBanner = new Panel();
        _lblStatus = new Label();

        _rootLayout.SuspendLayout();
        _exportCardBorder.SuspendLayout();
        _exportCard.SuspendLayout();
        _exportActions.SuspendLayout();
        _statusBanner.SuspendLayout();
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
        _lblTitle.Margin = new Padding(0, 0, 0, 4);
        _lblTitle.Text = "Xuất dữ liệu kết quả thi";

        // Hàng 1: Mô tả ngắn (H2/H8)
        _lblSubtitle.AutoSize = true;
        _lblSubtitle.Font = UiStyle.BodyFont;
        _lblSubtitle.ForeColor = UiStyle.TextMuted;
        _lblSubtitle.Margin = new Padding(0, 0, 0, 16);
        _lblSubtitle.Text = "Xuất bảng điểm tổng hợp các ca thi đã chấm từ cơ sở dữ liệu cục bộ ra tệp CSV chuẩn UTF-8.";

        // Hàng 2: Thẻ Xuất kết quả thi có viền mảnh (co giãn theo DPI)
        _exportCardBorder.Dock = DockStyle.Fill;
        _exportCardBorder.AutoSize = true;
        _exportCardBorder.BackColor = UiStyle.Border;
        _exportCardBorder.Padding = new Padding(1);
        _exportCardBorder.Margin = new Padding(0, 0, 0, 16);

        _exportCard.Dock = DockStyle.Fill;
        _exportCard.AutoSize = true;
        _exportCard.BackColor = UiStyle.Surface;
        _exportCard.Padding = new Padding(20);
        _exportCard.ColumnCount = 1;
        _exportCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _exportCard.RowCount = 4;
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _exportCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblExportHeading.AutoSize = true;
        _lblExportHeading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblExportHeading.ForeColor = UiStyle.Text;
        _lblExportHeading.Margin = new Padding(0, 0, 0, 8);
        _lblExportHeading.Text = "Xuất bảng điểm ra tệp CSV";

        _lblExportInfo.AutoSize = true;
        _lblExportInfo.ForeColor = UiStyle.TextMuted;
        _lblExportInfo.Margin = new Padding(0, 0, 0, 12);
        _lblExportInfo.Text = "Các cột được xuất: MSSV, Họ tên, Lớp, Ca thi, Đề thi, Số câu, Đúng, Sai, Bỏ trống, Điểm, Hình thức nộp, Thời điểm nộp.\nTệp sử dụng mã hóa UTF-8 (BOM) để hiển thị đúng tiếng Việt khi mở bằng Microsoft Excel.";

        _lblSummary.AutoSize = true;
        _lblSummary.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        _lblSummary.ForeColor = UiStyle.Primary;
        _lblSummary.BackColor = UiStyle.Background;
        _lblSummary.Padding = new Padding(10, 6, 10, 6);
        _lblSummary.Margin = new Padding(0, 0, 0, 16);

        _exportActions.Dock = DockStyle.Fill;
        _exportActions.AutoSize = true;
        _exportActions.WrapContents = true;
        _exportActions.Margin = new Padding(0);

        _btnExportCsv.Text = "Xuất kết quả (.csv)";
        _btnExportCsv.TabIndex = 0;
        _btnExportCsv.Margin = new Padding(0, 0, 12, 4);
        UiStyle.StylePrimaryButton(_btnExportCsv);

        _btnRefresh.Text = "Tải lại số liệu";
        _btnRefresh.TabIndex = 1;
        _btnRefresh.Margin = new Padding(0, 0, 0, 4);
        UiStyle.StyleSecondaryButton(_btnRefresh);

        _exportActions.Controls.Add(_btnExportCsv);
        _exportActions.Controls.Add(_btnRefresh);

        _exportCard.Controls.Add(_lblExportHeading, 0, 0);
        _exportCard.Controls.Add(_lblExportInfo, 0, 1);
        _exportCard.Controls.Add(_lblSummary, 0, 2);
        _exportCard.Controls.Add(_exportActions, 0, 3);
        _exportCardBorder.Controls.Add(_exportCard);

        // Hàng 3: Khung phản hồi trạng thái rõ ràng (G3/H1)
        _statusBanner.Dock = DockStyle.Fill;
        _statusBanner.AutoSize = true;
        _statusBanner.BackColor = UiStyle.Surface;
        _statusBanner.Padding = new Padding(14, 10, 14, 10);

        _lblStatus.AutoSize = true;
        _lblStatus.Dock = DockStyle.Fill;
        _lblStatus.ForeColor = UiStyle.TextMuted;
        _lblStatus.Text = "Trạng thái: Sẵn sàng xuất dữ liệu.";
        _statusBanner.Controls.Add(_lblStatus);

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_lblSubtitle, 0, 1);
        _rootLayout.Controls.Add(_exportCardBorder, 0, 2);
        _rootLayout.Controls.Add(_statusBanner, 0, 3);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "DataManagementControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _exportCardBorder.ResumeLayout(false);
        _exportCardBorder.PerformLayout();
        _exportCard.ResumeLayout(false);
        _exportCard.PerformLayout();
        _exportActions.ResumeLayout(false);
        _exportActions.PerformLayout();
        _statusBanner.ResumeLayout(false);
        _statusBanner.PerformLayout();
        ResumeLayout(false);
    }
}
