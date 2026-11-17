#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class StudentExamForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label _studentLabel = null!, _examLabel = null!;
    private readonly Panel _studentContent = new StudentScreenHost { Dock = DockStyle.Fill };

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private const int HeaderBadgeSize = 28;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.SmallFont;
        ClientSize = new Size(StudentLayout.FormClientWidth, StudentLayout.FormClientHeight);
        MinimumSize = new Size(StudentLayout.FormMinWidth, StudentLayout.FormMinHeight);
        StartPosition = FormStartPosition.CenterParent;
        // Thanh trên cùng giữ tên kỳ thi và sinh viên khi chuyển giữa các màn.
        var header = new BufferedStudentTable
        {
            Dock = DockStyle.Top, Height = StudentLayout.BarHeight, ColumnCount = 2, RowCount = 1,
            BackColor = UiStyle.Surface,
            Padding = new Padding(StudentLayout.Space24, StudentLayout.Space8, StudentLayout.Space24, StudentLayout.Space8)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var examTitle = new Label
        {
            Text = "UniExam",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
            Font = StudentLayout.GetLabelFont(StudentLayout.HeadingFontSize, FontStyle.Bold),
            ForeColor = UiStyle.Text, AutoEllipsis = true,
            Margin = new Padding(0, 0, StudentLayout.Space16, 0)
        };
        var studentName = new Label
        {
            AutoSize = false, AutoEllipsis = true,
            Dock = DockStyle.Fill,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            BackColor = UiStyle.Surface,
            ForeColor = UiStyle.TextMuted,
            TextAlign = ContentAlignment.MiddleRight
        };
        void SizeBadge()
        {
            int left = StudentLayout.Scale(examTitle, HeaderBadgeSize)
                + StudentLayout.Scale(examTitle, StudentLayout.Space8);
            examTitle.Padding = new Padding(left, 0, 0, 0);
        }
        SizeBadge();
        examTitle.HandleCreated += (_, _) => SizeBadge();
        examTitle.DpiChangedAfterParent += (_, _) => SizeBadge();
        examTitle.Paint += (_, e) =>
        {
            int size = StudentLayout.Scale(examTitle, HeaderBadgeSize);
            var bounds = new Rectangle(0, (examTitle.Height - size) / 2, size, size);
            using var badge = StudentLayout.RoundedRectangle(examTitle, bounds, StudentLayout.ButtonRadius);
            using var fill = new SolidBrush(UiStyle.Primary);
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillPath(fill, badge);
            e.Graphics.Restore(state);
            TextRenderer.DrawText(e.Graphics, "U", examTitle.Font, bounds, UiStyle.Surface,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
        studentName.Paint += (_, e) =>
        {
            if (studentName.Text.Length == 0)
                TextRenderer.DrawText(e.Graphics, "Cổng sinh viên", studentName.Font,
                    studentName.ClientRectangle, UiStyle.TextMuted,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        };

        _examLabel=examTitle; _studentLabel=studentName;
        header.Controls.Add(examTitle, 0, 0);
        header.Controls.Add(studentName, 1, 0);

        // Thêm vùng nội dung trước để Dock.Fill nằm dưới thanh tiêu đề.
        Controls.Add(_studentContent);
        Controls.Add(header);
        Text = "UniExam — Nhập mã sinh viên";
        ResumeLayout(false);
    }
}



