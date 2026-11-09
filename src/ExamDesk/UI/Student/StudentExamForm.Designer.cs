#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class StudentExamForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label _studentLabel = null!, _examLabel = null!;
    private Panel _studentContent = new() { Dock = DockStyle.Fill };

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
        Font = UiStyle.SmallFont;
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterParent;
        WindowState = FormWindowState.Maximized;
        Load += (_, _) => UiStyle.ClampToWorkingArea(this);
        // Thanh trên cùng giữ tên kỳ thi và sinh viên khi chuyển giữa các màn.
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 64, ColumnCount = 2, RowCount = 1,
            BackColor = UiStyle.Surface, Padding = new Padding(24, 12, 24, 12)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var examTitle = new Label
        {
            Text = "UNIEXAM — SINH VIÊN",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            Font = StudentLayout.GetLabelFont(12, FontStyle.Bold),
            ForeColor = UiStyle.Text, AutoEllipsis = true,
            Margin = new Padding(0, 0, 16, 0)
        };
        var studentName = new Label
        {
            Text = "Nhận diện bằng MSSV", AutoSize = false, AutoEllipsis = true,
            Dock = DockStyle.Fill, Padding = new Padding(12, 6, 12, 6), Margin = Padding.Empty,
            BackColor = UiStyle.Background,
            ForeColor = UiStyle.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft
        };
        _examLabel=examTitle; _studentLabel=studentName;
        header.Controls.Add(examTitle, 0, 0);
        header.Controls.Add(studentName, 1, 0);

        // Thêm vùng nội dung trước để Dock.Fill nằm dưới thanh tiêu đề.
        Controls.Add(_studentContent);
        Controls.Add(header);
        Text = "UniExam — Chọn ca thi";
        ResumeLayout(false);
    }
}



