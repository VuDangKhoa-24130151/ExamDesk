#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

partial class GradingReviewControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _lblTitle = new Label();
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