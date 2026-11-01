#nullable enable
namespace ExamDesk.UI.Administration;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label _lblHint = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _lblHint = new Label();
        SuspendLayout();
        _lblHint.AutoSize = true;
        _lblHint.Location = new Point(24, 24);
        _lblHint.Text = "AUTH-02 • Bạn 1 phụ trách\nKhung UI chưa triển khai. Có thể làm giao diện với dữ liệu minh họa riêng.";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterParent;
        Controls.Add(_lblHint);
        Text = "UniExam — Đăng nhập giảng viên (khung UI)";
        ResumeLayout(false);
        PerformLayout();
    }
}

