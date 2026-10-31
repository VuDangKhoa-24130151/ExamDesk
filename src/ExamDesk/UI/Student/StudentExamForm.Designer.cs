#nullable enable
namespace ExamDesk.UI.Student;

partial class StudentExamForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterParent;
        WindowState = FormWindowState.Maximized;
        // Thanh trên cùng giữ tên kỳ thi và sinh viên khi chuyển giữa các màn.
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 64, ColumnCount = 2, RowCount = 1,
            BackColor = Color.White, Padding = new Padding(24, 12, 24, 12)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var examTitle = new Label
        {
            Text = "KỲ THI CUỐI KỲ / LẬP TRÌNH .NET",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = ExamDesk.UI.Shared.UiStyle.Text, AutoEllipsis = true,
            Margin = new Padding(0, 0, 16, 0)
        };
        var studentName = new Label
        {
            Text = "Vũ Đăng Khoa", AutoSize = true,
            Anchor = AnchorStyles.Right, Padding = new Padding(12, 6, 12, 6),
            MinimumSize = new Size(190, 0), Margin = Padding.Empty,
            BackColor = Color.FromArgb(234, 246, 239),
            ForeColor = Color.FromArgb(20, 108, 67),
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(examTitle, 0, 0);
        header.Controls.Add(studentName, 1, 0);

        // Thêm vùng nội dung trước để Dock.Fill nằm dưới thanh tiêu đề.
        Controls.Add(new SessionSelectionControl());
        Controls.Add(header);
        Text = "UniExam — Chọn ca thi";
        ResumeLayout(false);
    }
}



