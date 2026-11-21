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
    private FlowLayoutPanel _body = null!;
    private TableLayoutPanel _questionLegend = null!, _titleGroup = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        int S(int value) => StudentLayout.Scale(this, value);
        DoubleBuffered = true;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Padding = Padding.Empty;

        // 1. Đồng hồ ở giữa và nút nộp bài trên nền chung.
        _title = new Label
        {
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            Font = StudentLayout.GetLabelFont(StudentLayout.HeadingFontSize, FontStyle.Bold),
            ForeColor = UiStyle.Text,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 0, S(StudentLayout.Space16), 0),
            UseMnemonic = false
        };

        _clock = new ExamClockPill
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            UseMnemonic = false,
            Font = StudentLayout.GetLabelFont(StudentLayout.ClockFontSize, FontStyle.Bold),
            Margin = new Padding(S(StudentLayout.Space8), 0, S(StudentLayout.Space8), 0)
        };

        _identity = new Label
        {
            Visible = false,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Surface,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(S(StudentLayout.Space16), 0, S(StudentLayout.Space16), 0),
            UseMnemonic = false
        };

        _submit = new StudentButton("&Nộp bài", StudentButtonKind.Secondary)
        {
            AccessibleName = "Nộp bài",
            AccessibleDescription = "Mở kiểm tra bài trước khi xác nhận nộp.",
            Anchor = AnchorStyles.Right,
            TabIndex = 0
        };

        var identityActions = new BufferedStudentTable
        {
            BackColor = UiStyle.Background,
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
            BackColor = UiStyle.Background,
            Margin = Padding.Empty,
            TabIndex = 1,
            TabStop = false,
            Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space8), S(StudentLayout.Space24), S(StudentLayout.Space8)),
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 50),
                new ColumnStyle(SizeType.Absolute, S(StudentLayout.ClockWidth)),
                new ColumnStyle(SizeType.Percent, 50)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        var brandBadge = new ClockPill
        {
            Text = "U", Dock = DockStyle.Fill, Margin = Padding.Empty,
            Font = StudentLayout.GetLabelFont(StudentLayout.HeadingFontSize, FontStyle.Bold),
            ForeColor = UiStyle.Text, BackColor = UiStyle.Surface, TabStop = false
        };
        _titleGroup = new BufferedStudentTable
        {
            Visible = false,
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            Margin = Padding.Empty, TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Absolute, S(StudentLayout.ExamBadgeSize)),
                new ColumnStyle(SizeType.Percent, 100)
            },
            RowStyles = { new RowStyle(SizeType.Percent, 100) }
        };
        _titleGroup.Controls.Add(brandBadge, 0, 0);
        _titleGroup.Controls.Add(_title, 1, 0);
        _examHeader.Controls.Add(_titleGroup, 0, 0);
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
            Padding = new Padding(S(StudentLayout.Space12), S(StudentLayout.Space8), S(StudentLayout.Space12), S(StudentLayout.Space8)),
            Margin = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space16), S(StudentLayout.Space24), 0),
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
            Height = S(StudentLayout.HeadingHeight),
            Margin = new Padding(0, 0, 0, S(StudentLayout.Space4)),
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
            Font = StudentLayout.GetLabelFont(StudentLayout.QuestionFontSize, FontStyle.Bold),
            Margin = new Padding(0, S(StudentLayout.Space4), 0, S(StudentLayout.Space24)),
            TabIndex = 1
        };

        _image = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = UiStyle.Background,
            Padding = new Padding(S(StudentLayout.Space4)),
            Visible = false,
            Margin = new Padding(0, 0, 0, S(StudentLayout.Space12)),
            Cursor = Cursors.Hand,
            AccessibleName = "Ảnh minh họa câu hỏi",
            TabIndex = 2
        };

        _image.Paint += (_, e) =>
        {
            int stroke = S(StudentLayout.Stroke1);
            using var border = new Pen(UiStyle.Border, stroke);
            e.Graphics.DrawRectangle(border, stroke, stroke,
                Math.Max(0, _image.Width - stroke * 2), Math.Max(0, _image.Height - stroke * 2));
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
            option.Margin = new Padding(0, 0, 0, S(StudentLayout.Space12));
            _options[i] = option;
            _body.Controls.Add(option);
        }

        _questionViewport = new BufferedStudentPanel
        {
            Name = "QuestionViewport",
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = UiStyle.Surface,
            Margin = new Padding(S(StudentLayout.Space24)),
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
            Margin = new Padding(0, 0, S(StudentLayout.Space8), 0)
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
            Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Stroke1), S(StudentLayout.Space24), 0),
            BackColor = Color.Transparent,
            TabIndex = 2,
            TabStop = false,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 50),
                new ColumnStyle(SizeType.AutoSize),
                new ColumnStyle(SizeType.Percent, 50)
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
                new RowStyle(SizeType.Absolute, S(StudentLayout.BarHeight))
            }
        };
        cardLayout.Controls.Add(_deadlineBanner, 0, 0);
        cardLayout.Controls.Add(_questionViewport, 0, 1);
        cardLayout.Controls.Add(_actionBar, 0, 2);

        var questionCard = new QuestionCard
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, S(StudentLayout.Space24), 0),
            Padding = new Padding(S(StudentLayout.Stroke1)),
            TabIndex = 0,
            TabStop = false
        };
        questionCard.Controls.Add(cardLayout);

        // 5. Sidebar danh sách câu hỏi & chú giải bên phải
        var progressTitle = StudentLayout.Label("Danh sách câu hỏi", StudentLayout.BodyFontSize, FontStyle.Bold);
        progressTitle.Margin = new Padding(0, 0, 0, S(StudentLayout.Space4));
        progressTitle.TabIndex = 0;

        _progress = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoEllipsis = true,
            Height = S(StudentLayout.HeadingHeight),
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular),
            ForeColor = UiStyle.TextMuted,
            Margin = new Padding(0, 0, 0, S(StudentLayout.Space12)),
            TabIndex = 1
        };

        _gridViewport = new BufferedStudentPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Margin = Padding.Empty,
            TabIndex = 2
        };

        _questionLegend = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 2,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 50), new ColumnStyle(SizeType.Percent, 50) },
            RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.AutoSize) },
            Margin = new Padding(0, S(StudentLayout.Space16), 0, 0),
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
            var index = _questionLegend.Controls.Count;
            _questionLegend.Controls.Add(new LegendItem(label, state)
            {
                Dock = DockStyle.Fill,
                Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Bold)
            }, index % 2, index / 2);
        }

        _right = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            BackColor = UiStyle.Surface,
            Margin = Padding.Empty,
            TabIndex = 1,
            ColumnStyles = { new ColumnStyle(SizeType.Percent, 100) },
            RowStyles =
            {
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Absolute, 0),
                new RowStyle(SizeType.AutoSize),
                new RowStyle(SizeType.Percent, 100)
            }
        };
        _right.Controls.Add(progressTitle, 0, 0);
        _right.Controls.Add(_progress, 0, 1);
        _right.Controls.Add(_gridViewport, 0, 2);
        _right.Controls.Add(_questionLegend, 0, 3);

        var sidebar = new QuestionCard
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(S(StudentLayout.Space16)),
            TabIndex = 1
        };
        sidebar.Controls.Add(_right);

        // 6. Bố cục 2 cột chính: Thẻ làm bài (trái) và Cột câu hỏi (phải)
        _layout = new BufferedStudentTable
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space16), S(StudentLayout.Space24), S(StudentLayout.Space24)),
            TabIndex = 0,
            ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100),
                new ColumnStyle(SizeType.Absolute, S(StudentLayout.SidebarWidth))
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
                new RowStyle(SizeType.Absolute, S(StudentLayout.BarHeight)),
                new RowStyle(SizeType.Percent, 100)
            }
        };
        _examShell.Controls.Add(_examHeader, 0, 0);
        _examShell.Controls.Add(_layout, 0, 1);
        Controls.Add(_examShell);

        // Keep presentation overrides local when ApplyExamLayout runs again for DPI.
        void KeepMargin(Control control, Func<Padding> desired)
        {
            void Apply()
            {
                var margin = desired();
                if (control.Margin != margin) control.Margin = margin;
            }
            control.MarginChanged += (_, _) => Apply();
            control.DpiChangedAfterParent += (_, _) => Apply();
            Apply();
        }

        KeepMargin(_layout, () => new Padding(S(StudentLayout.Space24),
            S(StudentLayout.Space16), S(StudentLayout.Space24), S(StudentLayout.Space24)));
        KeepMargin(_questionHeading, () => new Padding(0, 0, 0, S(StudentLayout.Space4)));
        Padding LegendMargin()
        {
            if (_gridViewport.Controls.Count == 0
                || _gridViewport.Controls[0] is not TableLayoutPanel tiles)
                return new Padding(0, S(StudentLayout.Space16), 0, 0);
            int width = tiles.ColumnCount * S(StudentLayout.TileStep) - S(StudentLayout.TileGap);
            int left = Math.Max(0, (tiles.ClientSize.Width - width) / 2);
            int right = Math.Max(0, _right.ClientSize.Width - left - width);
            return new Padding(left, S(StudentLayout.Space16), right, 0);
        }
        KeepMargin(_questionLegend, LegendMargin);
        foreach (var option in _options)
            KeepMargin(option, () => new Padding(0, 0, 0, S(StudentLayout.Space12)));

        // Follow the grid's natural height; reserve the legend before allowing scrolling.
        bool sizingSidebar = false;
        void FitSidebarPresentation()
        {
            if (sizingSidebar || _gridViewport.Controls.Count == 0) return;
            sizingSidebar = true;
            try
            {
                var grid = _gridViewport.Controls[0];
                int legendHeight = _questionLegend.GetPreferredSize(
                    new Size(_right.ClientSize.Width, 0)).Height + _questionLegend.Margin.Vertical;
                int titleHeight = progressTitle.GetPreferredSize(
                    new Size(_right.ClientSize.Width, 0)).Height + progressTitle.Margin.Vertical;
                int available = Math.Max(0, _right.ClientSize.Height - titleHeight
                    - (int)_right.RowStyles[1].Height - legendHeight);
                int height = Math.Min(grid.Height, available);
                if (_right.RowStyles[2].Height != height)
                    _right.RowStyles[2].Height = height;
                if (grid is TableLayoutPanel tiles && tiles.ColumnCount > 0)
                {
                    int tilesWidth = tiles.ColumnCount * S(StudentLayout.TileStep)
                        - S(StudentLayout.TileGap);
                    int left = Math.Max(0, (tiles.ClientSize.Width - tilesWidth) / 2);
                    var padding = new Padding(left, 0, 0, 0);
                    if (tiles.Padding != padding) tiles.Padding = padding;
                    var legendMargin = LegendMargin();
                    if (_questionLegend.Margin != legendMargin) _questionLegend.Margin = legendMargin;
                    foreach (Control number in tiles.Controls)
                        number.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
                }
            }
            finally { sizingSidebar = false; }
        }
        _gridViewport.ControlAdded += (_, e) =>
        {
            if (e.Control is { } grid)
            {
                grid.SizeChanged += (_, _) => FitSidebarPresentation();
                grid.Layout += (_, _) => FitSidebarPresentation();
            }
            FitSidebarPresentation();
        };
        _right.Layout += (_, _) => FitSidebarPresentation();

        _questionNumber.ForeColor = _progress.ForeColor = StudentLayout.ExamMutedText;
        _saveStatus.ForeColorChanged += (_, _) =>
        {
            if (_saveStatus.ForeColor == UiStyle.TextMuted)
                _saveStatus.ForeColor = StudentLayout.ExamMutedText;
        };
        foreach (var button in new[] { _previous, _next, _clearChoice, _flag, _submit })
            button.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
        foreach (var card in new[] { questionCard, sidebar })
        {
            card.Paint += (_, e) =>
            {
                int stroke = S(StudentLayout.Stroke1);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var path = StudentLayout.RoundedRectangle(card,
                    Rectangle.Inflate(card.ClientRectangle, -stroke, -stroke), StudentLayout.CardRadius);
                using var border = new Pen(StudentLayout.ExamBorder, stroke);
                e.Graphics.DrawPath(border, path);
            };
        }

        ApplyExamLayout();
        ResumeLayout(true);
    }

    private sealed class ExamClockPill : Label
    {
        internal ExamClockPill() => SetStyle(ControlStyles.UserPaint
            | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        protected override void OnPaintBackground(PaintEventArgs e) =>
            e.Graphics.Clear(Parent?.BackColor ?? UiStyle.Background);

        protected override void OnPaint(PaintEventArgs e)
        {
            int S(int value) => StudentLayout.Scale(this, value);
            int inset = S(StudentLayout.Stroke1);
            if (Width <= inset * 2 || Height <= inset * 2) return;

            var state = e.Graphics.Save();
            try
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var pill = StudentLayout.RoundedRectangle(this,
                    Rectangle.Inflate(ClientRectangle, -inset, -inset), StudentLayout.PillRadius);
                using var fill = new SolidBrush(BackColor);
                e.Graphics.FillPath(fill, pill);

                int iconSize = S(StudentLayout.Space16);
                int gap = S(StudentLayout.Space8);
                var flags = TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix
                    | TextFormatFlags.NoPadding;
                int available = Math.Max(0, Width - inset * 2 - iconSize - gap);
                int textWidth = Math.Min(available, TextRenderer.MeasureText(e.Graphics,
                    Text, Font, new Size(int.MaxValue, int.MaxValue), flags).Width);
                int left = (Width - iconSize - gap - textWidth) / 2;
                var icon = new Rectangle(left, (Height - iconSize) / 2, iconSize, iconSize);
                using var pen = new Pen(ForeColor, inset)
                {
                    StartCap = System.Drawing.Drawing2D.LineCap.Round,
                    EndCap = System.Drawing.Drawing2D.LineCap.Round
                };
                e.Graphics.DrawEllipse(pen, icon);
                int centerX = icon.Left + icon.Width / 2;
                int centerY = icon.Top + icon.Height / 2;
                e.Graphics.DrawLine(pen, centerX, centerY,
                    centerX, centerY - S(StudentLayout.Space4));
                e.Graphics.DrawLine(pen, centerX, centerY,
                    centerX + S(StudentLayout.Space4), centerY);
                TextRenderer.DrawText(e.Graphics, Text, Font,
                    new Rectangle(icon.Right + gap, 0, textWidth, Height), ForeColor,
                    flags | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            finally { e.Graphics.Restore(state); }
        }
    }
}
