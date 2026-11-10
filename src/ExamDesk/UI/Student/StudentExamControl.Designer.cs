#nullable enable
using ExamDesk.UI.Shared;
namespace ExamDesk.UI.Student;
partial class StudentExamControl
{
    private Label _question = null!, _questionNumber = null!, _clock = null!, _progress = null!,
        _saveStatus = null!;
    private Panel _questionViewport = null!, _gridViewport = null!, _progressTrack = null!;
    private readonly OptionRow[] _options = new OptionRow[4];
    private CheckBox _flag = null!;
    private PictureBox _image = null!;
    private Button _previous = null!, _next = null!, _clearChoice = null!, _submit = null!, _viewImage = null!;

    private TableLayoutPanel _layout = null!, _right = null!;
    private FlowLayoutPanel _body = null!, _actions = null!;
    private Label _section = null!, _title = null!, _identity = null!, _hint = null!, _advice = null!, _legend = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7, 17);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Padding = new Padding(24);
        _layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 2,
            Margin = new Padding(0, 0, 16, 8),
            ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.Percent, 100) },
            RowStyles = { new RowStyle(SizeType.Absolute, 24), new RowStyle(SizeType.Absolute, 32) }
        };
        _section = new Label
        {
            Text = "BÀI THI TRẮC NGHIỆM", AutoSize = true, Font = StudentLayout.GetLabelFont(9, FontStyle.Bold),
            ForeColor = UiStyle.Primary, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 12, 0)
        };
        _identity = new Label
        {
            Dock = DockStyle.Fill, AutoEllipsis = true, Font = UiStyle.SmallFont,
            ForeColor = UiStyle.TextMuted, TextAlign = ContentAlignment.MiddleLeft, Margin = Padding.Empty,
            UseMnemonic = false
        };
        _title = new Label
        {
            Dock = DockStyle.Fill, AutoEllipsis = true, Font = UiStyle.TitleFont,
            TextAlign = ContentAlignment.MiddleLeft, Margin = Padding.Empty, UseMnemonic = false
        };
        header.Controls.Add(_section, 0, 0);
        header.Controls.Add(_identity, 1, 0);
        header.Controls.Add(_title, 0, 1);
        header.SetColumnSpan(_title, 2);
        _layout.Controls.Add(header, 0, 0);

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 16, 0),
            RowStyles = { new RowStyle(SizeType.Percent, 100), new RowStyle(SizeType.AutoSize) }
        };
        var questionCard = new CardPanel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        var cardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles = { new RowStyle(SizeType.Absolute, 40), new RowStyle(SizeType.Percent, 100) }
        };
        _saveStatus = new Label
        {
            Dock = DockStyle.Fill, Font = UiStyle.SmallFont, AutoEllipsis = true,
            Padding = new Padding(8, 4, 8, 4), Margin = new Padding(0, 0, 0, 8),
            TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false
        };
        _questionViewport = new Panel
        {
            Name = "QuestionViewport", Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty
        };
        _body = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = Padding.Empty
        };
        _questionNumber = StudentLayout.Label("", 9, FontStyle.Bold);
        _questionNumber.ForeColor = UiStyle.TextMuted;
        _questionNumber.Margin = new Padding(0, 0, 0, 4);
        _body.Controls.Add(_questionNumber);
        _hint = StudentLayout.Label("Chọn một đáp án phù hợp nhất", 9);
        _hint.ForeColor = UiStyle.TextMuted;
        _hint.Margin = new Padding(0, 0, 0, 4);
        _body.Controls.Add(_hint);
        _question = StudentLayout.Label("", 11);
        _question.Margin = new Padding(0, 4, 0, 12);
        _body.Controls.Add(_question);
        _image = new PictureBox { SizeMode=PictureBoxSizeMode.Zoom, Visible=false, Margin=new Padding(0,0,0,12),
            Cursor = Cursors.Hand, AccessibleName = "Ảnh minh họa câu hỏi" };
        _body.Controls.Add(_image);
        _viewImage = StudentLayout.Button("Xem ảnh &lớn");
        _viewImage.Visible = false;
        _body.Controls.Add(_viewImage);
        for (int i = 0; i < _options.Length; i++)
        {
            var option = new OptionRow((char)('A' + i)) { TabIndex = i, TabStop = i == 0 };
            _options[i] = option;
            _body.Controls.Add(option);
        }
        _viewImage.TabIndex = _options.Length;
        _advice = StudentLayout.Label("Dùng phím mũi tên trong nhóm đáp án để đổi lựa chọn. Bạn vẫn có thể xem lại bài trước khi xác nhận nộp bài.", 9);
        _advice.ForeColor = UiStyle.TextMuted;
        _advice.Margin = Padding.Empty;
        _body.Controls.Add(_advice);
        _questionViewport.Controls.Add(_body);
        cardLayout.Controls.Add(_saveStatus, 0, 0);
        cardLayout.Controls.Add(_questionViewport, 0, 1);
        questionCard.Controls.Add(cardLayout);
        left.Controls.Add(questionCard, 0, 0);

        _actions = new FlowLayoutPanel
        {
            AutoSize = true, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 8, 0, 0)
        };
        _previous = StudentLayout.Button("← &Trước");
        _previous.AccessibleName = "Câu trước";
        _next = StudentLayout.Button("T&iếp →");
        _next.AccessibleName = "Câu tiếp";
        _clearChoice = StudentLayout.Button("&Bỏ chọn");
        _flag = new CheckBox { Appearance = Appearance.Button, Text = FlagText, AutoSize = true,
            FlatStyle = FlatStyle.Flat, Padding = new Padding(8, 4, 8, 4),
            BackColor = UiStyle.Surface, MinimumSize = new Size(0, 38) };
        _flag.FlatAppearance.BorderColor = UiStyle.Border;
        _flag.FlatAppearance.CheckedBackColor = UiStyle.Background;
        // Giữ thứ tự Tab cũ khi đổi vị trí các nút.
        _previous.TabIndex = 0;
        _next.TabIndex = 1;
        _flag.TabIndex = 2;
        _clearChoice.TabIndex = 3;
        _previous.Padding = _next.Padding = _clearChoice.Padding = new Padding(8, 4, 8, 4);
        _previous.Margin = _clearChoice.Margin = _flag.Margin = new Padding(0, 0, 4, 4);
        _next.Margin = new Padding(0, 0, 0, 4);
        _actions.Controls.AddRange([_previous, _clearChoice, _flag, _next]);
        left.Controls.Add(_actions, 0, 1);
        _layout.Controls.Add(left, 0, 1);

        _right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            Margin = Padding.Empty,
            RowStyles =
            {
                new RowStyle(SizeType.Absolute, 48),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize)
            }
        };
        _right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _clock = new Label
        {
            Dock = DockStyle.Fill, Font = StudentLayout.GetLabelFont(18, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter, ForeColor = UiStyle.Primary, BackColor = UiStyle.Background,
            Margin = new Padding(0, 0, 0, 8)
        };
        _right.Controls.Add(_clock, 0, 0);
        var progressTitle = StudentLayout.Label("Tiến độ làm bài", 10, FontStyle.Bold);
        progressTitle.Margin = new Padding(0, 0, 0, 4);
        _right.Controls.Add(progressTitle, 0, 1);
        _progress = StudentLayout.Label("");
        _progress.ForeColor = UiStyle.TextMuted;
        _progress.Margin = new Padding(0, 0, 0, 8);
        _right.Controls.Add(_progress, 0, 2);
        _progressTrack = new Panel { Dock = DockStyle.Fill, Height = 8, BackColor = UiStyle.Background, Margin = new Padding(0, 0, 0, 8) };
        _right.Controls.Add(_progressTrack, 0, 3);
        _gridViewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        _right.Controls.Add(_gridViewport, 0, 4);
        _legend = StudentLayout.Label("Chưa trả lời: viền mảnh   ✓ Đã trả lời\nĐang xem: viền đậm   ⚑ Đánh dấu", 9);
        _legend.ForeColor = UiStyle.TextMuted;
        _legend.Margin = new Padding(0, 8, 0, 8);
        _right.Controls.Add(_legend, 0, 5);
        _submit = StudentLayout.Button("Nộp bài", primary: true);
        _submit.Dock = DockStyle.Fill;
        _submit.Margin = Padding.Empty;
        _right.Controls.Add(_submit, 0, 6);
        var sidebar = new CardPanel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        sidebar.Controls.Add(_right);
        _layout.Controls.Add(sidebar, 1, 0);
        _layout.SetRowSpan(sidebar, 2);
        Controls.Add(_layout);

        ResumeLayout(true);
    }
}
