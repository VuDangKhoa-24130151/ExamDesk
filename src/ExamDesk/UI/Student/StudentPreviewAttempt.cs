using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// State UI chỉ sống trong cửa sổ này; chưa lưu tệp hoặc gọi Service/Repository.
internal sealed class StudentPreviewAttempt
{
    public ExamDesk.Models.StudentDraft? Draft { get; private set; }
    private readonly ExamDesk.Services.StudentDraftService _service = new();
    private readonly int?[] _demoAnswers = new int?[30];
    private readonly bool[] _demoFlags = new bool[30];
    private readonly DateTime _demoDeadline = DateTime.Now.AddMinutes(45);
    public StudentPreviewAttempt() { }
    public StudentPreviewAttempt(ExamDesk.Models.StudentDraft draft) { Draft = draft; }
    public int?[] Answers => Draft?.Answers ?? _demoAnswers;
    public bool[] Flags => Draft?.Flags ?? _demoFlags;
    public string Title => Draft?.Title ?? "Làm bài demo (không lưu)";
    public int Current { get; set; }
    public DateTime Deadline => Draft?.DeadlineUtc.LocalDateTime ?? _demoDeadline;
    public bool Submitted { get; set; }
    public int Answered => Answers.Count(answer => answer.HasValue);
    public void Choose(int? choice)
    {
        if(Draft is null) _demoAnswers[Current]=choice;
        else Draft=_service.Change(Draft,Current,choice);
    }
    public void Flag(bool value)
    {
        if(Draft is null) _demoFlags[Current]=value;
        else Draft=_service.Change(Draft,Current,Answers[Current],value);
    }
    public void Lock() { if(Draft is not null) Draft=_service.Lock(Draft); }
    public string Remaining
    {
        get { int seconds=Math.Max(0,(int)(Deadline-DateTime.Now).TotalSeconds); return $"{seconds/60:00}:{seconds%60:00}"; }
    }
}
// Bố cục dùng riêng trong phân hệ Student, giữ bảng màu chung.
internal static class StudentLayout
{
    public static FlowLayoutPanel Page(UserControl control, string title)
    {
        control.AutoScaleMode = AutoScaleMode.Font;
        control.Font = new Font("Segoe UI", 10);
        control.BackColor = UiStyle.Background;
        control.ForeColor = UiStyle.Text;
        control.AutoScroll = true;
        var page = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(24) };
        page.Controls.Add(Label(title, 18, FontStyle.Bold));
        control.Controls.Add(page);
        page.SizeChanged += (_, _) => {
            int width = Math.Max(100, page.ClientSize.Width - page.Padding.Horizontal - 24);
            foreach (Control child in page.Controls)
            {
                if (child is Label label) label.MaximumSize = new Size(width, 0);
                else child.Width = width;
            }
        };
        return page;
    }
    public static Label Label(string text, float size = 10, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, Font = new Font("Segoe UI", size, style),
        Margin = new Padding(0, 8, 0, 12), ForeColor = UiStyle.Text
    };
    public static Button Button(string text, EventHandler click, bool primary = false)
    {
        var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(160, 40),
            Padding = new Padding(12, 4, 12, 4), Margin = new Padding(0, 8, 12, 8),
            FlatStyle = FlatStyle.Flat, BackColor = primary ? UiStyle.Primary : Color.White,
            ForeColor = primary ? Color.White : UiStyle.Text };
        button.FlatAppearance.BorderColor = Color.FromArgb(220, 227, 237);
        button.Click += click;
        return button;
    }
}
