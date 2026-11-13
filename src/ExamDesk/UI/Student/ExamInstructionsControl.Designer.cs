using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class ExamInstructionsControl
{
    private CheckBox _readCheckBox = null!;
    private Button _backButton = null!, _startButton = null!;
    private Label _sessionTitle = null!, _studentInfo = null!, _paperInfo = null!, _resumeInfo = null!;
    private Label _questionCount = null!, _duration = null!, _room = null!, _sessionCode = null!, _durationRule = null!;
    private TableLayoutPanel _page = null!, _rules = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.SmallFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;

        var viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        _page = Column();
        _page.Padding = new Padding(StudentLayout.Space24, StudentLayout.Space24, StudentLayout.Space24, StudentLayout.Space16);
        _page.Controls.Add(TextLabel("Hướng dẫn trước khi thi", StudentLayout.PageTitleFontSize, FontStyle.Bold));
        var stats = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, RowCount = 1,
            Margin = new Padding(0, StudentLayout.Space8, 0, StudentLayout.Space16)
        };
        TableLayoutPanel[] statTiles =
        [
            StatTile("Số câu", out _questionCount),
            StatTile("Thời gian", out _duration),
            StatTile("Phòng", out _room),
            StatTile("Mã ca", out _sessionCode)
        ];
        for (int i = 0; i < statTiles.Length; i++)
        {
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            stats.Controls.Add(statTiles[i], i, 0);
        }
        statTiles[^1].Margin = Padding.Empty;
        _page.Controls.Add(stats);

        _sessionTitle = TextLabel("Thông tin ca thi", StudentLayout.SubheadingFontSize, FontStyle.Bold);
        _studentInfo = TextLabel("", StudentLayout.SmallFontSize);
        _studentInfo.ForeColor = UiStyle.TextMuted;
        _paperInfo = TextLabel("");
        _page.Controls.Add(_sessionTitle);
        _page.Controls.Add(_studentInfo);
        _page.Controls.Add(_paperInfo);

        _resumeInfo = TextLabel("");
        _resumeInfo.ForeColor = UiStyle.WarningText;
        _resumeInfo.Visible = false;
        _page.Controls.Add(_resumeInfo);

        _rules = Column();
        _rules.BackColor = UiStyle.Surface;
        _rules.Padding = new Padding(StudentLayout.Space16, StudentLayout.Space16, StudentLayout.Space16, StudentLayout.Space8);
        _rules.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, _rules.ClientRectangle,
            UiStyle.Border, ButtonBorderStyle.Solid);
        _rules.Controls.Add(TextLabel("Quy tắc làm bài", StudentLayout.SubheadingFontSize, FontStyle.Bold));
        _durationRule = TextLabel("");
        Label[] rules =
        [
            _durationRule,
            TextLabel("Mỗi câu có bốn đáp án A–D. Chọn một đáp án, có thể đổi hoặc bỏ lựa chọn."),
            TextLabel("Chọn đáp án bằng A/B/C/D hoặc 1–4. Chuyển câu bằng N/P hoặc PageDown/PageUp."),
            TextLabel("Có thể đánh dấu để quay lại xem sau."),
            TextLabel("Tiến độ được lưu khi thay đổi. Đóng cửa sổ không dừng đồng hồ."),
            TextLabel("Hết giờ, hệ thống tự động nộp bài. Nếu có lỗi, bạn có thể thử nộp bài lại."),
            TextLabel("Sau khi nộp bài, bạn không thể thay đổi đáp án.")
        ];
        for (int i = 0; i < rules.Length; i++)
        {
            var ruleRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1,
                Margin = Padding.Empty,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.Percent, 100) }
            };
            var number = TextLabel($"{i + 1}.");
            number.Margin = new Padding(0, 0, StudentLayout.Space8, StudentLayout.Space8);
            var content = rules[i];
            ruleRow.Controls.Add(number, 0, 0);
            ruleRow.Controls.Add(content, 1, 0);
            ruleRow.SizeChanged += (_, _) => content.MaximumSize = new Size(
                Math.Max(1, ruleRow.ClientSize.Width - number.PreferredWidth - number.Margin.Horizontal), 0);
            _rules.Controls.Add(ruleRow);
        }
        _page.Controls.Add(_rules);
        viewport.Controls.Add(_page);

        // Chỉ vùng hướng dẫn cuộn; thanh xác nhận luôn ở dưới cùng.
        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom, AutoSize = true, ColumnCount = 2, RowCount = 2,
            Padding = new Padding(StudentLayout.Space24, StudentLayout.Space12, StudentLayout.Space24, StudentLayout.Space12),
            BackColor = UiStyle.Surface
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _readCheckBox = new CheckBox
        {
            Text = "Tôi đã đọc và &hiểu hướng dẫn làm bài.", AutoSize = true,
            Font = UiStyle.BodyFont, TabIndex = 0, Margin = new Padding(0, 0, 0, StudentLayout.Space8)
        };
        footer.Controls.Add(_readCheckBox, 0, 0);
        footer.SetColumnSpan(_readCheckBox, 2);

        _backButton = StudentLayout.CreateButton("&Quay lại chọn ca", arrow: StudentArrow.Left);
        _backButton.TabIndex = 1;
        _backButton.Margin = Padding.Empty;
        footer.Controls.Add(_backButton, 0, 1);
        _startButton = StudentLayout.Button("&Bắt đầu làm bài", primary: true);
        _startButton.Enabled = false;
        _startButton.TabIndex = 2;
        _startButton.Anchor = AnchorStyles.Right;
        _startButton.Margin = Padding.Empty;
        footer.Controls.Add(_startButton, 1, 1);

        Controls.Add(viewport);
        Controls.Add(footer);
        ResumeLayout(true);
    }

    private static TableLayoutPanel Column()
    {
        var column = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = Padding.Empty
        };
        column.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return column;
    }

    private static TableLayoutPanel StatTile(string caption, out Label value)
    {
        var tile = Column();
        tile.Dock = DockStyle.Fill;
        tile.BackColor = UiStyle.Surface;
        tile.Padding = new Padding(StudentLayout.Space12, StudentLayout.Space8, StudentLayout.Space12, StudentLayout.Space4);
        tile.Margin = new Padding(0, 0, StudentLayout.Space8, 0);
        tile.RowCount = 2;
        tile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var label = TextLabel(caption, StudentLayout.SmallFontSize);
        label.ForeColor = UiStyle.TextMuted;
        label.Margin = new Padding(0, 0, 0, StudentLayout.Space4);
        value = new Label
        {
            Text = "—", Dock = DockStyle.Fill, AutoSize = true, UseMnemonic = false,
            Font = UiStyle.TitleFont, Margin = Padding.Empty, TextAlign = ContentAlignment.MiddleLeft
        };
        tile.Controls.Add(label);
        tile.Controls.Add(value);
        return tile;
    }

    private static Label TextLabel(string text, float size = StudentLayout.BodyFontSize, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, AutoSize = true, UseMnemonic = false, Font = StudentLayout.GetLabelFont(size, style),
        Margin = new Padding(0, 0, 0, StudentLayout.Space8)
    };
}
