#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class StudentExamControl
{
    private Label _title = null!, _identity = null!, _clock = null!, _progress = null!,
        _questionNumber = null!, _deadlineBanner = null!;
    private QuestionText _question = null!;
    private SaveIndicator _saveStatus = null!;
    private Panel _questionViewport = null!, _gridViewport = null!;
    private readonly OptionRow[] _options = new OptionRow[4];
    private StudentButton _flag = null!, _previous = null!, _next = null!, _clearChoice = null!, _submit = null!;
    private StudentButton _viewImage = null!;
    private PictureBox _image = null!;

    private TableLayoutPanel _examShell = null!, _examHeader = null!, _layout = null!, _right = null!,
        _questionHeading = null!, _actionBar = null!;
    private FlowLayoutPanel _body = null!, _questionLegend = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        DoubleBuffered = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Padding = Padding.Empty;

        // 1. Header gọn: Tiêu đề đề thi, đồng hồ đếm ngược, thông tin sinh viên và nút Nộp bài
        _title = new Label
        {
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            Font = StudentLayout.GetLabelFont(StudentLayout.HeadingFontSize, FontStyle.Bold),
            ForeColor = UiStyle.Text,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 0, StudentLayout.Space16, 0),
            UseMnemonic = false
        };

        _clock = new ClockPill
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false,
            Font = StudentLayout.GetLabelFont(StudentLayout.ClockFontSize, FontStyle.Bold),
            Margin = new Padding(StudentLayout.Space8, 0, StudentLayout.Space8, 0)
        };

        _identity = new Label
        {
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Surface,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(StudentLayout.Space16, 0, StudentLayout.Space16, 0),
            UseMnemonic = false
        };

        _submit = new StudentButton("&Nộp bài", StudentButtonKind.Primary)
        {
            AccessibleName = "Nộp bài",
            AccessibleDescription = "Mở kiểm tra bài trước khi xác nhận nộp.",
            Anchor = AnchorStyles.Right,
            TabIndex = 0
        };

        var identityActions = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            TabIndex = 0,
            TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.AutoSize)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        identityActions.Controls.Add(_identity, 0, 0);
        identityActions.Controls.Add(_submit, 1, 0);

        _examHeader = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = UiStyle.Surface,
            Margin = Padding.Empty,
            TabIndex = 1,
            TabStop = false,
            Padding = new Padding(StudentLayout.Space24, StudentLayout.Space8, StudentLayout.Space24, StudentLayout.Space8),
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 50),
                new ColumnStyle(SizeType.Absolute, StudentLayout.ClockWidth),
                new ColumnStyle(SizeType.Percent, 50)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _examHeader.Paint += (_, e) => StudentLayout.DrawHeaderBorder(_examHeader, e.Graphics);
        _examHeader.Controls.Add(_title, 0, 0);
        _examHeader.Controls.Add(_clock, 1, 0);
        _examHeader.Controls.Add(identityActions, 2, 0);

        // 2. Card câu hỏi bên trái
        _deadlineBanner = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            Visible = false,
            Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold),
            BackColor = UiStyle.WarningBack,
            ForeColor = UiStyle.WarningText,
            Padding = new Padding(StudentLayout.Space12, StudentLayout.Space8, StudentLayout.Space12, StudentLayout.Space8),
            Margin = new Padding(StudentLayout.Space24, StudentLayout.Space16, StudentLayout.Space24, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false,
            TabIndex = 0
        };

        _questionNumber = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            ForeColor = UiStyle.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty,
            UseMnemonic = false
        };

        _saveStatus = new SaveIndicator
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            BackColor = UiStyle.Surface,
            UseMnemonic = false,
            TabIndex = 0,
            TabStop = true,
            Margin = Padding.Empty,
            AccessibleName = "Trạng thái lưu tiến độ"
        };

        _questionHeading = new BufferedStudentTable
        {
            ColumnCount = 2,
            RowCount = 1,
            Height = StudentLayout.HeadingHeight,
            Margin = new Padding(0, 0, 0, StudentLayout.Space12),
            TabIndex = 0,
            TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 50),
                new ColumnStyle(SizeType.Percent, 50)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _questionHeading.Controls.Add(_questionNumber, 0, 0);
        _questionHeading.Controls.Add(_saveStatus, 1, 0);

        _question = new QuestionText
        {
            AutoSize = false,
            UseMnemonic = false,
            ForeColor = UiStyle.Text,
            Font = StudentLayout.GetLabelFont(StudentLayout.HeadingFontSize, FontStyle.Regular),
            Margin = new Padding(0, StudentLayout.Space4, 0, StudentLayout.Space24),
            TabIndex = 1
        };

        _image = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.Zoom,
            Visible = false,
            Margin = new Padding(0, 0, 0, StudentLayout.Space12),
            Cursor = Cursors.Hand,
            AccessibleName = "Ảnh minh họa câu hỏi",
            TabIndex = 2
        };

        _viewImage = StudentLayout.CreateButton("Xem ảnh &lớn");
        _viewImage.Visible = false;
        _viewImage.TabIndex = 3;

        _body = new BufferedStudentFlow
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = UiStyle.Surface,
            Margin = Padding.Empty
        };
        _body.Controls.Add(_questionHeading);
        _body.Controls.Add(_question);
        _body.Controls.Add(_image);
        _body.Controls.Add(_viewImage);
        for (int i = 0; i < _options.Length; i++)
        {
            var option = new OptionRow((char)('A' + i)) { TabIndex = 4 + i, TabStop = i == 0 };
            option.Margin = new Padding(0, 0, 0, StudentLayout.Space8);
            _options[i] = option;
            _body.Controls.Add(option);
        }

        _questionViewport = new BufferedStudentPanel
        {
            Name = "QuestionViewport",
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = UiStyle.Surface,
            Margin = new Padding(StudentLayout.Space24, StudentLayout.Space24, StudentLayout.Space24, StudentLayout.Space16),
            TabIndex = 1
        };
        _questionViewport.Controls.Add(_body);

        // 4. Thanh nút thao tác Trước / Bỏ chọn / Đánh dấu / Tiếp
        _previous = new StudentButton(PreviousText, StudentButtonKind.Secondary, StudentArrow.Left)
        {
            AccessibleName = "Câu trước",
            Anchor = AnchorStyles.Left,
            TabIndex = 0,
            Margin = Padding.Empty
        };

        _clearChoice = new StudentButton(ClearChoiceText, StudentButtonKind.Ghost)
        {
            AccessibleName = "Bỏ chọn",
            TabIndex = 0,
            Margin = new Padding(0, 0, StudentLayout.Space8, 0)
        };

        _flag = new StudentButton(FlagText, StudentButtonKind.FlagToggle)
        {
            AccessibleName = "Đánh dấu xem lại",
            TabIndex = 1,
            Margin = Padding.Empty
        };

        _next = new StudentButton(NextText, StudentButtonKind.Secondary, StudentArrow.Right)
        {
            AccessibleName = "Câu tiếp",
            Anchor = AnchorStyles.Right,
            TabIndex = 2,
            Margin = Padding.Empty
        };

        var centerActions = new BufferedStudentFlow
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Anchor = AnchorStyles.None,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = UiStyle.Surface,
            TabIndex = 1,
            TabStop = false
        };
        centerActions.Controls.AddRange([_clearChoice, _flag]);

        _actionBar = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(StudentLayout.Space24, StudentLayout.Stroke1, StudentLayout.Space24, 0),
            BackColor = Color.Transparent,
            TabIndex = 2,
            TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.AutoSize),
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.AutoSize)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _actionBar.Paint += (_, e) => StudentLayout.DrawTopBorder(_actionBar, e.Graphics);
        _actionBar.Controls.Add(_previous, 0, 0);
        _actionBar.Controls.Add(centerActions, 1, 0);
        _actionBar.Controls.Add(_next, 2, 0);

        var cardLayout = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            // Giữ nền trong suốt ở lớp sát viền bo của thẻ.
            BackColor = Color.Transparent,
            TabIndex = 0,
            TabStop = false,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100),
                new RowStyle(SizeType.Absolute, StudentLayout.BarHeight)
            }
        };
        cardLayout.Controls.Add(_deadlineBanner, 0, 0);
        cardLayout.Controls.Add(_questionViewport, 0, 1);
        cardLayout.Controls.Add(_actionBar, 0, 2);

        var questionCard = new QuestionCard
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, StudentLayout.Space24, 0),
            Padding = new Padding(StudentLayout.Stroke1),
            TabIndex = 0,
            TabStop = false
        };
        questionCard.Controls.Add(cardLayout);

        // 5. Sidebar danh sách câu hỏi & chú giải bên phải
        var progressTitle = StudentLayout.Label("Danh sách câu hỏi", StudentLayout.BodyFontSize, FontStyle.Bold);
        progressTitle.Margin = new Padding(0, 0, 0, StudentLayout.Space4);
        progressTitle.TabIndex = 0;

        _progress = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            AutoEllipsis = true,
            Height = StudentLayout.HeadingHeight,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            ForeColor = UiStyle.TextMuted,
            Margin = new Padding(0, 0, 0, StudentLayout.Space12),
            TabIndex = 1
        };

        _gridViewport = new BufferedStudentPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Margin = Padding.Empty,
            TabIndex = 2
        };

        _questionLegend = new BufferedStudentFlow
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            Margin = new Padding(0, StudentLayout.Space12, 0, 0),
            BackColor = UiStyle.Surface,
            TabIndex = 3,
            TabStop = false
        };
        foreach (var (label, state) in new[]
        {
            ("Đã trả lời", QuestionTileState.Answered),
            ("Đánh dấu", QuestionTileState.Flagged),
            ("Đang xem", QuestionTileState.Current),
            ("Chưa trả lời", QuestionTileState.Unanswered)
        })
        {
            _questionLegend.Controls.Add(new LegendItem(label, state));
        }

        _right = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            BackColor = UiStyle.Surface,
            Margin = Padding.Empty,
            TabIndex = 1,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100),
                new RowStyle(SizeType.AutoSize)
            }
        };
        _right.Controls.Add(progressTitle, 0, 0);
        _right.Controls.Add(_progress, 0, 1);
        _right.Controls.Add(_gridViewport, 0, 2);
        _right.Controls.Add(_questionLegend, 0, 3);

        var sidebar = new CardPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(StudentLayout.Space16),
            TabIndex = 1
        };
        sidebar.Controls.Add(_right);

        // 6. Bố cục 2 cột chính: Thẻ làm bài (trái) và Cột câu hỏi (phải)
        _layout = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(StudentLayout.Space24),
            TabIndex = 0,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.Absolute, StudentLayout.SidebarWidth)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _layout.Controls.Add(questionCard, 0, 0);
        _layout.Controls.Add(sidebar, 1, 0);

        // 6. Khung tổng thể 2 hàng: Header -> Bố cục làm bài
        _examShell = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            TabStop = false,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.Absolute, StudentLayout.BarHeight),
                new RowStyle(SizeType.Percent, 100)
            }
        };
        _examShell.Controls.Add(_examHeader, 0, 0);
        _examShell.Controls.Add(_layout, 0, 1);
        Controls.Add(_examShell);

        ResumeLayout(true);
    }
}
