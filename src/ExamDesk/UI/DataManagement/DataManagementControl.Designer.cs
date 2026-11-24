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
        _lblStatus = new Label();

        _rootLayout.SuspendLayout();
        _exportCard.SuspendLayout();
        _exportActions.SuspendLayout();
        SuspendLayout();

        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 3;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Hàng 0: Tiêu đề
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 16);
        _lblTitle.Text = "Xuất dữ liệu kết quả thi";

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
        _lblExportInfo.Text = "Xuất danh sách kết quả thi\nXuất bảng điểm các ca thi từ SQLite ra tệp CSV để lưu trữ hoặc mở bằng Excel.";

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

        // Hàng 2: Nhãn phản hồi trạng thái
        _lblStatus.AutoSize = true;
        _lblStatus.ForeColor = UiStyle.TextMuted;
        _lblStatus.Text = "Trạng thái: Sẵn sàng.";

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_exportCard, 0, 1);
        _rootLayout.Controls.Add(_lblStatus, 0, 2);

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
        ResumeLayout(false);
    }
}
