using ExamDesk.UI.Shared;
namespace ExamDesk.UI.Student;
partial class StudentExamControl
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private Label _question = null!, _clock = null!, _progress = null!, _notice = null!;
    private readonly RadioButton[] _options = new RadioButton[4];
    private readonly Button[] _numbers = new Button[30];
    private CheckBox _flag = null!;
    private Button _previous = null!, _next = null!;
    private void InitializeComponent()
    {
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 10);
        BackColor = UiStyle.Background;
        Padding = new Padding(24);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(StudentLayout.Label("Cuối kỳ .NET – Ca 01", 18, FontStyle.Bold), 0, 0);
        _clock = StudentLayout.Label(""); layout.Controls.Add(_clock, 1, 0);
        var content = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White,
            Padding = new Padding(16), FlowDirection = FlowDirection.TopDown, WrapContents = false };
        _question = StudentLayout.Label(""); content.Controls.Add(_question);
        string[] answers = { "A. int", "B. bool", "C. string", "D. double" };
        for (int i = 0; i < 4; i++)
        {
            int option = i;
            _options[i] = new RadioButton { Text = answers[i], AutoSize = true, MinimumSize = new Size(180, 42), Margin = new Padding(0, 4, 0, 4) };
            _options[i].CheckedChanged += (_, _) => Choose(option);
            content.Controls.Add(_options[i]);
        }
        var actions = new FlowLayoutPanel { AutoSize = true, WrapContents = true, Width = 550 };
        _previous = StudentLayout.Button("← Câu &trước", (_, _) => GoToQuestion(_attempt.Current - 1));
        _next = StudentLayout.Button("Câu &tiếp →", (_, _) => GoToQuestion(_attempt.Current + 1));
        actions.Controls.Add(_previous); actions.Controls.Add(_next);
        actions.Controls.Add(StudentLayout.Button("&Bỏ lựa chọn", (_, _) => { _attempt.Answers[_attempt.Current] = null; RenderQuestion(); }));
        content.Controls.Add(actions);
        _flag = new CheckBox { Text = "⚑ Đánh dấu &xem lại", AutoSize = true, Margin = new Padding(0, 12, 0, 12) };
        _flag.CheckedChanged += (_, _) => { _attempt.Flags[_attempt.Current] = _flag.Checked; RenderQuestion(); };
        content.Controls.Add(_flag);
        _notice = StudentLayout.Label("Chọn đáp án không tự chuyển câu. Dữ liệu chỉ giữ trong cửa sổ hiện tại.");
        content.Controls.Add(_notice);
        content.Controls.Add(StudentLayout.Button("&Tạm dừng / khôi phục", (_, _) => PauseRequested?.Invoke(this, EventArgs.Empty)));
        content.SizeChanged += (_, _) => {
            int width = Math.Max(160, content.ClientSize.Width - 50);
            _question.MaximumSize = _notice.MaximumSize = new Size(width, 0);
            actions.Width = width;
        };
        layout.Controls.Add(content, 0, 1);
        var sidebar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(12), BackColor = Color.White, Margin = new Padding(16, 0, 0, 0) };
        sidebar.Controls.Add(StudentLayout.Label("Tiến độ làm bài", 12, FontStyle.Bold));
        _progress = StudentLayout.Label(""); sidebar.Controls.Add(_progress);
        var grid = new TableLayoutPanel { AutoSize = true, ColumnCount = 5, RowCount = 6, Width = 270 };
        for (int i = 0; i < 5; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        for (int i = 0; i < 30; i++)
        {
            int index = i;
            _numbers[i] = new Button { Width = 49, Height = 40, Font = new Font("Segoe UI", 9), FlatStyle = FlatStyle.Flat, Margin = new Padding(2), AccessibleName = $"Câu {i + 1}" };
            _numbers[i].Click += (_, _) => GoToQuestion(index);
            grid.Controls.Add(_numbers[i], i % 5, i / 5);
        }
        sidebar.Controls.Add(grid);
        var legend = StudentLayout.Label("+ Đã trả lời · − Chưa trả lời\n* Xem lại · Viền đậm: đang xem");
        legend.MaximumSize = new Size(270, 0); sidebar.Controls.Add(legend);
        // Nút nộp luôn hiện, còn lưới câu và chú giải được cuộn riêng.
        var right = new Panel { Dock = DockStyle.Fill };
        var submit = StudentLayout.Button("&Nộp bài", (_, _) => SubmitRequested?.Invoke(this, EventArgs.Empty), true);
        submit.Dock = DockStyle.Bottom;
        right.Controls.Add(sidebar);
        right.Controls.Add(submit);
        layout.Controls.Add(right, 1, 1);
        Controls.Add(layout);
        _timer.Tick += Tick; _timer.Start();
    }
}
