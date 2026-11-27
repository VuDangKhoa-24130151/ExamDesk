#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.DataManagement;

partial class DataManagementControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private Panel _exportCardBorder = null!;
    private TableLayoutPanel _exportCard = null!;
    private Label _lblExportHeading = null!;
    private Label _lblExportInfo = null!;
    private Button _btnExportCsv = null!;
    private Panel _statusBannerBorder = null!;
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
        _exportCardBorder = new Panel();
        _exportCard = new TableLayoutPanel();
        _lblExportHeading = new Label();
        _lblExportInfo = new Label();
        _btnExportCsv = new Button();
        _statusBannerBorder = new Panel();
        _statusBanner = new Panel();
        _lblStatus = new Label();

        _rootLayout.SuspendLayout();
        _exportCardBorder.SuspendLayout();
        _exportCard.SuspendLayout();
        _statusBannerBorder.SuspendLayout();
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
        // Thêm hàng đệm 100% ở dưới cùng để WinForms không kéo giãn khung trạng thái chiếm hết phần trống.
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Hàng 0: Tiêu đề
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 16);
        _lblTitle.Text = "Xuất dữ liệu kết quả thi";

        // Hàng 1: Thẻ Xuất kết quả thi (bọc viền 1px bằng Panel ngoài để nổi trên nền xám nhạt)
        _exportCardBorder.Dock = DockStyle.Fill;
        _exportCardBorder.AutoSize = true;
        _exportCardBorder.BackColor = UiStyle.Border;
        _exportCardBorder.Padding = new Padding(1);
        _exportCardBorder.Margin = new Padding(0, 0, 0, 14);

        _exportCard.Dock = DockStyle.Fill;
        _exportCard.AutoSize = true;
        _exportCard.BackColor = UiStyle.Surface;
        _exportCard.Padding = new Padding(20);
        _exportCard.ColumnCount = 1;
        _exportCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _exportCard.RowCount = 3;
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
        _lblExportInfo.Margin = new Padding(0, 0, 0, 16);
        _lblExportInfo.Text = "Các cột được xuất: MSSV, Họ tên, Lớp, Ca thi, Đề thi, Số câu, Đúng, Sai, Bỏ trống, Điểm, Hình thức nộp, Thời điểm nộp.";

        _btnExportCsv.Text = "Xuất kết quả (.csv)";
        _btnExportCsv.TabIndex = 0;
        _btnExportCsv.Margin = new Padding(0);
        UiStyle.StylePrimaryButton(_btnExportCsv);

        _exportCard.Controls.Add(_lblExportHeading, 0, 0);
        _exportCard.Controls.Add(_lblExportInfo, 0, 1);
        _exportCard.Controls.Add(_btnExportCsv, 0, 2);
        _exportCardBorder.Controls.Add(_exportCard);

        // Hàng 2: Thanh trạng thái gọn 1 dòng
        _statusBannerBorder.Dock = DockStyle.Top;
        _statusBannerBorder.AutoSize = true;
        _statusBannerBorder.BackColor = UiStyle.Border;
        _statusBannerBorder.Padding = new Padding(1);
        _statusBannerBorder.Margin = new Padding(0);

        _statusBanner.Dock = DockStyle.Top;
        _statusBanner.AutoSize = true;
        _statusBanner.BackColor = UiStyle.Surface;
        _statusBanner.Padding = new Padding(14, 10, 14, 10);

        _lblStatus.AutoSize = true;
        _lblStatus.ForeColor = UiStyle.TextMuted;
        _lblStatus.Text = "Trạng thái: Sẵn sàng xuất dữ liệu.";
        _statusBanner.Controls.Add(_lblStatus);
        _statusBannerBorder.Controls.Add(_statusBanner);

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_exportCardBorder, 0, 1);
        _rootLayout.Controls.Add(_statusBannerBorder, 0, 2);

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
        _statusBannerBorder.ResumeLayout(false);
        _statusBannerBorder.PerformLayout();
        _statusBanner.ResumeLayout(false);
        _statusBanner.PerformLayout();
        ResumeLayout(false);
    }
}
