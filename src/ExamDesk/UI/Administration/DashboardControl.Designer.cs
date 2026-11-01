#nullable enable
namespace ExamDesk.UI.Administration;

partial class DashboardControl
{
    private System.ComponentModel.IContainer? components = null;
    private Label _lblTitle = null!;
    private Label _lblHint = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _lblTitle = new Label();
        _lblHint = new Label();
        SuspendLayout();
        _lblTitle.AutoSize = true;
        _lblTitle.Location = new Point(24, 24);
        _lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        _lblTitle.Text = "Tổng quan";
        _lblHint.AutoSize = true;
        _lblHint.Location = new Point(24, 72);
        _lblHint.Text = "GV-01 • Bạn 1 phụ trách\nKhung UI chưa triển khai. Thêm control tại đây; chưa gọi Service/SQLite.";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);
        Controls.Add(_lblHint);
        Controls.Add(_lblTitle);
        Name = "DashboardControl";
        Size = new Size(1000, 600);
        ResumeLayout(false);
        PerformLayout();
    }
}

