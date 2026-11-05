using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SubmissionStatusControl
{
    private Label _status = null!, _summary = null!;
    private Panel _cardHost = null!;

    private void InitializeComponent()
    {
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10);
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        var heading = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(24, 16, 24, 0)
        };
        heading.Controls.Add(StudentLayout.Label("Kết thúc lượt làm bài", 18, FontStyle.Bold));
        heading.Controls.Add(StudentLayout.Label("Bạn có thể đóng cửa sổ để trở về màn hình chính."));
        _cardHost = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var card = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1,
            BackColor = UiStyle.Surface, Padding = new Padding(24)
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        card.Controls.Add(StudentLayout.Label("Thông tin lượt làm bài", 12, FontStyle.Bold));
        card.Controls.Add(StudentLayout.Label("Bài đã được chốt. Bạn không thể sửa đáp án."));
        _status = StudentLayout.Label("Đã kết thúc lượt UI. Dữ liệu bài thi chỉ tồn tại trong phiên hiện tại.");
        _status.Dock = DockStyle.Top;
        _status.Padding = new Padding(16);
        card.Controls.Add(_status);
        _summary = StudentLayout.Label("Chưa có lượt thi.");
        card.Controls.Add(_summary);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.Button("&Đóng cửa sổ", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty)));
        card.Controls.Add(actions);
        card.SizeChanged += (_, _) =>
        {
            int width = Math.Max(1, card.ClientSize.Width - card.Padding.Horizontal);
            foreach (Control control in card.Controls)
            {
                if (control is Label label) label.MaximumSize = new Size(width, 0);
            }
        };
        _cardHost.Controls.Add(card);
        Controls.Add(_cardHost);
        Controls.Add(heading);
    }
}

