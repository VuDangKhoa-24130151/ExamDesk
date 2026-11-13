using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public sealed class StudentResultControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? CloseRequested;
    internal event EventHandler? EndSessionRequested;

    internal StudentResultControl(StudentResult result)
    {
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        DoubleBuffered = true;
        AutoScroll = true;
        var page = new BufferedStudentTable
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(StudentLayout.Space24)
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        page.Controls.Add(StudentLayout.Label("Kết quả bài thi", StudentLayout.PageTitleFontSize, FontStyle.Bold));
        page.Controls.Add(StudentLayout.Label($"Sinh viên: {result.StudentName} · MSSV: {result.StudentCode}"));
        page.Controls.Add(StudentLayout.Label($"Ca thi: {result.SessionName}"));
        page.Controls.Add(StudentLayout.Label($"Đề thi: {result.PaperTitle}"));

        var metrics = new BufferedStudentTable
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, RowCount = 1,
            Margin = new Padding(0, StudentLayout.Space8, 0, StudentLayout.Space12)
        };
        for (int i = 0; i < metrics.ColumnCount; i++)
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        metrics.Controls.Add(Metric("Điểm", $"{result.Score:F2}/10", isScore: true), 0, 0);
        metrics.Controls.Add(Metric("Đúng", $"{result.Correct}/{result.Total}"), 1, 0);
        metrics.Controls.Add(Metric("Sai", $"{result.Total - result.Correct - result.Blank}"), 2, 0);
        var blank = Metric("Bỏ trống", $"{result.Blank}");
        blank.Margin = Padding.Empty;
        metrics.Controls.Add(blank, 3, 0);
        page.Controls.Add(metrics);

        page.Controls.Add(StudentLayout.Label($"Thời điểm nộp bài: {result.SubmittedAt.ToLocalTime():dd/MM/yyyy HH:mm:ss}"));
        page.Controls.Add(StudentLayout.Label($"Lý do: {(result.IsAutoSubmitted ? "Hết giờ" : "Nộp bài chủ động")}"));
        var actions = new BufferedStudentFlow { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.CreateButton("&Quay lại chọn ca",
            (_, _) => BackRequested?.Invoke(this, EventArgs.Empty),
            StudentButtonKind.Secondary, StudentArrow.Left));
        actions.Controls.Add(StudentLayout.Button("Kết thúc phiên sinh viên", (_, _) => EndSessionRequested?.Invoke(this, EventArgs.Empty)));
        actions.Controls.Add(StudentLayout.Button("&Đóng", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty), true));
        page.Controls.Add(actions);
        page.SizeChanged += (_, _) =>
        {
            int width = page.ClientSize.Width - page.Padding.Horizontal;
            foreach (var label in page.Controls.OfType<Label>())
                label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
        };
        Controls.Add(page);
    }

    private static TableLayoutPanel Metric(string title, string value, bool isScore = false)
    {
        var tile = new BufferedStudentTable
        {
            AutoSize = true, Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
            BackColor = UiStyle.Surface,
            Padding = new Padding(StudentLayout.Space12, StudentLayout.Space8, StudentLayout.Space12, StudentLayout.Space8),
            Margin = new Padding(0, 0, StudentLayout.Space8, 0)
        };
        tile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var caption = StudentLayout.Label(title, StudentLayout.SmallFontSize);
        caption.ForeColor = UiStyle.TextMuted;
        caption.Margin = new Padding(0, 0, 0, StudentLayout.Space4);
        tile.Controls.Add(caption, 0, 0);
        tile.Controls.Add(new Label
        {
            Text = value, Dock = DockStyle.Fill, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft,
            Font = isScore ? StudentLayout.GetLabelFont(StudentLayout.PageTitleFontSize, FontStyle.Bold) : UiStyle.TitleFont,
            ForeColor = isScore ? UiStyle.Primary : UiStyle.Text, Margin = Padding.Empty
        }, 0, 1);
        return tile;
    }
}
