using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

internal sealed class ExamViolationDialog : Form
{
    internal bool Acknowledged { get; private set; }
    // Giữ lại màu ban đầu để đổi về khi người dùng tắt High Contrast.
    private readonly List<(Label Label, Color Fore, Color Back)> textColors = [];
    // Các control này được tạo trong BuildLayout trước khi dùng ở các hàm bên dưới.
    private FlowLayoutPanel footer = null!;
    private ConfirmationButton confirm = null!;
    private Panel viewport = null!;
    private TableLayoutPanel body = null!;

    internal ExamViolationDialog(int count, string reason)
    {
        Text = "UniExam — Cảnh cáo vi phạm";
        Font = UiStyle.BodyFont;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(640, 440);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ControlBox = false;
        ShowInTaskbar = false;
        BackColor = UiStyle.Surface;

        BuildLayout(count, reason);
        ApplyContrastColors();
        SystemColorsChanged += (_, _) => ApplyContrastColors();
        Load += (_, _) => FitToContent();
        // Chờ WinForms đổi DPI xong rồi mới đo lại, tránh lấy kích thước cũ.
        DpiChanged += (_, _) => BeginInvoke((Action)FitToContent);
        Shown += (_, _) => confirm.Focus();
        // Không cho Alt+F4 bỏ qua xác nhận ở lần 3.
        FormClosing += (_, e) =>
        {
            if (count >= 3 && !Acknowledged && e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        };
    }

    private void BuildLayout(int count, string reason)
    {
        footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(24, 10, 24, 10), BackColor = UiStyle.Surface
        };
        footer.Paint += (_, e) =>
        {
            using var pen = new Pen(SystemInformation.HighContrast ? SystemColors.WindowText : UiStyle.Border);
            e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
        };
        confirm = new ConfirmationButton
        {
            Text = count >= 3 ? "Xác nhận và xem điểm" : "Xác nhận, đã hiểu", AutoSize = true,
            MinimumSize = new Size(190, 42), Margin = Padding.Empty,
            Padding = new Padding(16, 8, 16, 8)
        };
        UiStyle.StylePrimaryButton(confirm);
        confirm.FlatAppearance.BorderSize = 1;
        confirm.Click += (_, _) => { Acknowledged = true; Close(); };
        footer.Controls.Add(confirm);

        // Nếu màn hình nhỏ hoặc chữ phóng to thì cuộn nội dung, nút xác nhận vẫn nằm dưới.
        viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        body = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1,
            Padding = new Padding(24, 20, 24, 10), Margin = Padding.Empty
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        body.RowCount = 0;
        AddText(count >= 3 ? "⚠  Bạn đã vi phạm 3 lần" : $"⚠  Cảnh cáo lần {count}", UiStyle.TitleFont, UiStyle.Text,
            new Padding(0, 0, 0, 12));
        if (count >= 2)
            AddText(count >= 3 ? "ĐÃ KHÓA BÀI LÀM" : "CẢNH CÁO CUỐI CÙNG", StudentLayout.GetLabelFont(10, FontStyle.Bold), UiStyle.WarningText,
                new Padding(0, 0, 0, 20), UiStyle.WarningBack, new Padding(12, 8, 12, 8));

        string description = reason switch
        {
            "Rời khỏi cửa sổ làm bài thi" => "Hệ thống phát hiện bạn rời cửa sổ làm bài.",
            "Sử dụng phím chụp màn hình" => "Hệ thống phát hiện bạn sử dụng phím chụp màn hình.",
            "Yêu cầu thoát cửa sổ làm bài thi" => "Hệ thống phát hiện bạn yêu cầu thoát cửa sổ làm bài.",
            _ => $"Hành vi được phát hiện: {reason}."
        };
        AddText(description, UiStyle.BodyFont, UiStyle.Text, new Padding(0, 0, 0, 20));
        AddText(count >= 3
                ? "Bạn không được tiếp tục làm bài. Bấm xác nhận để nộp bài và xem điểm."
                : count >= 2
                ? "Nếu tiếp tục vi phạm, bài làm của bạn sẽ được nộp tự động."
                : "Đến lần vi phạm thứ 3, bài làm của bạn sẽ được nộp tự động.",
            UiStyle.BodyFont, UiStyle.WarningText, new Padding(0, 0, 0, 10),
            UiStyle.WarningBack, new Padding(16));
        AddText(count >= 3 ? "Bài làm sẽ được chấm theo các đáp án đã lưu."
                : "Thời gian làm bài vẫn tiếp tục chạy.", UiStyle.SmallFont,
            UiStyle.TextMuted, Padding.Empty);

        viewport.Controls.Add(body);
        Controls.Add(viewport);
        Controls.Add(footer);
        AcceptButton = confirm;
    }

    private void AddText(string text, Font font, Color color, Padding margin, Color? background = null,
        Padding? padding = null)
    {
        // Khung có màu nền dùng label tự vẽ để bo góc; chữ thường dùng Label có sẵn.
        Label label = background.HasValue ? new WarningLabel() : new Label();
        label.Text = text;
        label.Font = font;
        label.ForeColor = color;
        label.BackColor = background ?? UiStyle.Surface;
        label.AutoSize = true;
        label.Dock = DockStyle.Top;
        label.Margin = margin;
        label.Padding = padding ?? Padding.Empty;
        label.UseMnemonic = false;
        textColors.Add((label, color, label.BackColor));
        body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        body.Controls.Add(label, 0, body.RowCount++);
    }

    private void ApplyContrastColors()
    {
        bool highContrast = SystemInformation.HighContrast;
        BackColor = footer.BackColor = viewport.BackColor = body.BackColor =
            highContrast ? SystemColors.Window : UiStyle.Surface;
        foreach (var item in textColors)
        {
            item.Label.ForeColor = highContrast ? SystemColors.WindowText : item.Fore;
            item.Label.BackColor = highContrast ? SystemColors.Window : item.Back;
        }
        confirm.BackColor = highContrast ? SystemColors.Control : UiStyle.Primary;
        confirm.ForeColor = highContrast ? SystemColors.ControlText : UiStyle.Surface;
        confirm.FlatAppearance.BorderColor = highContrast ? SystemColors.ControlText : UiStyle.Primary;
        // Bật High Contrast thì để Windows vẽ nút, tránh màu tự chọn làm khó đọc.
        confirm.FlatStyle = highContrast ? FlatStyle.Standard : FlatStyle.Flat;
        Invalidate(true);
    }

    private void FitToContent()
    {
        // Lấy dòng dài nhất cộng thêm lề để hộp thoại vừa chữ, không dư một đoạn bên phải.
        int textWidth = textColors.Max(item =>
            TextRenderer.MeasureText(item.Label.Text, item.Label.Font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix).Width
            + item.Label.Padding.Horizontal + item.Label.Margin.Horizontal);
        int desiredWidth = Math.Max(textWidth + body.Padding.Horizontal,
            confirm.PreferredSize.Width + footer.Padding.Horizontal);
        ClientSize = new Size(desiredWidth, ClientSize.Height);
        UiStyle.ClampToWorkingArea(this);
        // Chốt chiều rộng trước vì chữ có thể xuống dòng, lúc đó chiều cao cũng thay đổi.
        PerformLayout();
        // Cộng chiều cao nội dung với thanh nút, lần 1 sẽ tự thấp hơn lần 2.
        int contentHeight = body.GetPreferredSize(new Size(viewport.ClientSize.Width, 0)).Height;
        ClientSize = new Size(ClientSize.Width, contentHeight + footer.Height);
        UiStyle.ClampToWorkingArea(this);
    }

    private static Rectangle DrawRoundedSurface(Control control, Graphics graphics, Color background, Color outline)
    {
        int stroke = Math.Max(1, StudentLayout.Scale(control, 1));
        // Lùi viền vào trong một chút để nét vẽ không bị cắt ở mép control.
        var bounds = Rectangle.Inflate(control.ClientRectangle, -stroke, -stroke);
        if (bounds.Width <= 0 || bounds.Height <= 0) return Rectangle.Empty;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var path = StudentLayout.RoundedRectangle(control, bounds, 8);
        using var fill = new SolidBrush(background);
        using var border = new Pen(outline, stroke);
        graphics.FillPath(fill, path);
        graphics.DrawPath(border, path);
        return bounds;
    }

    private sealed class ConfirmationButton : Button
    {
        private bool _hovered;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (SystemInformation.HighContrast) { base.OnPaint(e); return; }
            // Xóa nền theo màu của khung cha để bốn góc ngoài nút không bị vuông màu xanh.
            e.Graphics.Clear(Parent?.BackColor ?? UiStyle.Surface);
            var bounds = DrawRoundedSurface(this, e.Graphics,
                _hovered ? StudentLayout.PrimaryHover : BackColor, UiStyle.Primary);
            if (bounds.IsEmpty) return;
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            // Vẫn hiện dấu focus để người dùng Tab biết mình đang chọn nút này.
            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(e.Graphics,
                    Rectangle.Inflate(bounds, -StudentLayout.Scale(this, 5), -StudentLayout.Scale(this, 5)),
                    ForeColor, BackColor);
        }
    }

    private sealed class WarningLabel : Label
    {
        // Tự vẽ nền và viền; bật bộ đệm để bớt nhấp nháy khi vẽ lại.
        internal WarningLabel() => SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw, true);

        protected override void OnPaintBackground(PaintEventArgs e) =>
            e.Graphics.Clear(Parent?.BackColor ?? BackColor);

        protected override void OnPaint(PaintEventArgs e)
        {
            var bounds = DrawRoundedSurface(this, e.Graphics, BackColor,
                SystemInformation.HighContrast ? SystemColors.WindowText : UiStyle.WarningText);
            if (bounds.IsEmpty) return;
            // Chừa padding cho chữ để không đè lên viền bo góc.
            var textBounds = new Rectangle(Padding.Left, Padding.Top,
                Math.Max(0, Width - Padding.Horizontal), Math.Max(0, Height - Padding.Vertical));
            TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, ForeColor,
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.TextBoxControl);
        }
    }
}
