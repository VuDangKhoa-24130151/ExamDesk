#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

partial class GradingReviewControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;

    private FlowLayoutPanel _filterPanel = null!;
    private Label _lblSession = null!;
    private ComboBox _cboSession = null!;
    private Label _lblClass = null!;
    private ComboBox _cboClass = null!;
    private Label _lblSearch = null!;
    private TextBox _txtSearch = null!;
    private Button _btnClearFilter = null!;
    private Label _lblCount = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _filterPanel = new FlowLayoutPanel();
        _lblSession = new Label();
        _cboSession = new ComboBox();
        _lblClass = new Label();
        _cboClass = new ComboBox();
        _lblSearch = new Label();
        _txtSearch = new TextBox();
        _btnClearFilter = new Button();
        _lblCount = new Label();
        _rootLayout.SuspendLayout();
        SuspendLayout();

        // Khung chính chia 4 hàng(gồmm Tiêu đề, Bộ lọc, Bảng kết quả, Nút dưới)
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 4;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Hàng 0: Tiêu đề
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 16);
        _lblTitle.Text = "Kết quả thi và phúc khảo";

        _rootLayout.Controls.Add(_lblTitle, 0, 0);

                // Hàng 1: Thanh bộ lọc
        _filterPanel.Dock = DockStyle.Fill;
        _filterPanel.AutoSize = true;
        _filterPanel.BackColor = UiStyle.Surface;
        _filterPanel.Padding = new Padding(12);
        _filterPanel.Margin = new Padding(0, 0, 0, 16);
        _filterPanel.WrapContents = true;

        _lblSession.AutoSize = true;
        _lblSession.Text = "Ca thi:";
        _lblSession.Margin = new Padding(0, 8, 6, 0);

        _cboSession.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSession.Width = 240;
        _cboSession.Margin = new Padding(0, 4, 16, 4);

        _lblClass.AutoSize = true;
        _lblClass.Text = "Lớp:";
        _lblClass.Margin = new Padding(0, 8, 6, 0);

        _cboClass.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboClass.Width = 130;
        _cboClass.Margin = new Padding(0, 4, 16, 4);

        _lblSearch.AutoSize = true;
        _lblSearch.Text = "Tìm SV:";
        _lblSearch.Margin = new Padding(0, 8, 6, 0);

        _txtSearch.Width = 180;
        _txtSearch.PlaceholderText = "MSSV hoặc họ tên...";
        _txtSearch.Margin = new Padding(0, 4, 12, 4);

        _btnClearFilter.Text = "Xóa lọc";
        _btnClearFilter.Width = 90;
        UiStyle.StyleSecondaryButton(_btnClearFilter);
        _btnClearFilter.Margin = new Padding(0, 0, 16, 0);

        _lblCount.AutoSize = true;
        _lblCount.ForeColor = UiStyle.TextMuted;
        _lblCount.Margin = new Padding(0, 10, 0, 0);

        _filterPanel.Controls.Add(_lblSession);
        _filterPanel.Controls.Add(_cboSession);
        _filterPanel.Controls.Add(_lblClass);
        _filterPanel.Controls.Add(_cboClass);
        _filterPanel.Controls.Add(_lblSearch);
        _filterPanel.Controls.Add(_txtSearch);
        _filterPanel.Controls.Add(_btnClearFilter);
        _filterPanel.Controls.Add(_lblCount);

        _rootLayout.Controls.Add(_filterPanel, 0, 1);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "GradingReviewControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        ResumeLayout(false);
    }
}