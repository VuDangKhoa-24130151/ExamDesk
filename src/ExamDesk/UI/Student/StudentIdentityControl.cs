using System.Drawing.Drawing2D;
using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Bạn 3: nhận diện sinh viên trước khi mở danh sách ca thi.
public sealed class StudentIdentityControl : UserControl
{
    private const int CardWidth = 640;
    private const int CardPadding = 32;
    private const int FieldHeight = 40;
    private const string CheckingMessage = "Đang kiểm tra mã số sinh viên…";
    private const string EmptyCodeMessage = "Hãy nhập mã số sinh viên để tra cứu hồ sơ.";
    private const string LookupErrorMessage = "Không tra cứu được. Hãy thử lại hoặc hỏi giám thị coi thi.";
    private const string ErrorPrefix = "Lỗi: ";
    private const string ViewText = "Tra cứu hồ sơ";

    private readonly StudentExamService _service = new();
    private readonly Panel _viewport = new BufferedStudentPanel() { Dock = DockStyle.Fill, AutoScroll = true };
    private readonly TableLayoutPanel _center = new BufferedStudentTable() { Anchor = AnchorStyles.Top | AnchorStyles.Left, ColumnCount = 3, RowCount = 3 };
    private readonly TableLayoutPanel _content = new BufferedStudentTable() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
    private readonly IdentityCard _card = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1 };
    private readonly TableLayoutPanel _form = new BufferedStudentTable() { Dock = DockStyle.Fill, ColumnCount = 1, BackColor = Color.Transparent };
    private readonly TableLayoutPanel _lookupRow = new BufferedStudentTable() { ColumnCount = 3, RowCount = 1, TabStop = false };
    private readonly Panel _input = new() { Dock = DockStyle.Fill, BackColor = UiStyle.Surface };
    private readonly TextBox _code = new()
    {
        BorderStyle = BorderStyle.None, Anchor = AnchorStyles.Left | AnchorStyles.Right,
        BackColor = UiStyle.Surface, ForeColor = UiStyle.Text,
        AccessibleName = "Mã số sinh viên", PlaceholderText = "Ví dụ: 24130151",
        Font = StudentLayout.GetLabelFont(StudentLayout.SubheadingFontSize, FontStyle.Regular), TabIndex = 0
    };
    private readonly Panel _messageHost = new() { Dock = DockStyle.Fill, TabStop = false };
    private readonly MessageLabel _message = new()
    {
        AutoSize = false, Dock = DockStyle.Fill, Font = UiStyle.SmallFont, ForeColor = UiStyle.TextMuted,
        AccessibleName = "Gợi ý hoặc lỗi mã số sinh viên", UseMnemonic = false
    };
    private readonly StudentButton _view = StudentLayout.CreateButton(ViewText,
        kind: StudentButtonKind.Primary, arrow: StudentArrow.Right);
    private readonly StudentButton _close = StudentLayout.CreateButton("Đóng cửa sổ", kind: StudentButtonKind.Ghost);
    private readonly Label _help = StudentLayout.Label("Cần hỗ trợ? Hãy hỏi giám thị coi thi.", StudentLayout.SmallFontSize);
    private bool _checking;
    private bool _hasError;
    private bool _layingOut;

    public event Action<User>? StudentIdentified;
    public event EventHandler? CloseRequested;

    public StudentIdentityControl()
    {
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.None;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        AccessibleName = "Bắt đầu làm bài thi";
        foreach (var panel in new TableLayoutPanel[] { _center, _content, _card, _form })
        {
            panel.Margin = Padding.Empty;
            panel.TabStop = false;
        }
        _center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _center.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute));
        _center.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _center.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        _center.RowStyles.Add(new RowStyle(SizeType.Absolute));
        _center.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        _content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _content.RowStyles.Add(new RowStyle(SizeType.Absolute));
        _content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _card.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(_form, new AlignedLabel("Nhập mã số sinh viên", StudentLayout.GetLabelFont(StudentLayout.TitleFontSize, FontStyle.Bold)) { TextAlign = ContentAlignment.TopCenter });
        var subtitle = new AlignedLabel("Tra cứu và xác nhận hồ sơ trước khi chọn ca thi.", UiStyle.BodyFont) { TextAlign = ContentAlignment.TopCenter };
        subtitle.ForeColor = UiStyle.TextMuted;
        AddRow(_form, subtitle);
        var codeLabel = new AlignedLabel("Mã số sinh viên", AlignedLabel.Semibold) { Anchor = AnchorStyles.Left, TabStop = false };
        _lookupRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _lookupRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _lookupRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute));
        _lookupRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _input.Margin = Padding.Empty;
        _input.TabIndex = 0;
        _view.Dock = DockStyle.Fill;
        _view.TabIndex = 1;
        _lookupRow.Controls.Add(codeLabel, 0, 0);
        _lookupRow.Controls.Add(_input, 1, 0);
        _lookupRow.Controls.Add(_view, 2, 0);
        AddRow(_form, _lookupRow, SizeType.Absolute);
        AddRow(_form, _messageHost, SizeType.Absolute);
        AddRow(_form, _close, SizeType.Absolute);
        _close.Dock = DockStyle.None;
        _close.Anchor = AnchorStyles.Left;
        _view.AccessibleName = ViewText;
        _close.AccessibleName = "Đóng cửa sổ";
        _help.ForeColor = UiStyle.TextMuted;
        _help.TextAlign = ContentAlignment.MiddleCenter;
        _help.Dock = DockStyle.Fill;
        _input.Controls.Add(_code);
        _messageHost.Controls.Add(_message);
        _card.HeaderDescription = subtitle;
        _card.Controls.Add(_form, 0, 0);
        _content.Controls.Add(_card, 0, 0);
        _content.Controls.Add(_help, 0, 1);
        _center.Controls.Add(_content, 1, 1);
        _viewport.Controls.Add(_center);
        Controls.Add(_viewport);

        _input.Paint += (_, e) =>
        {
            int thickness = S(_hasError || _code.Focused ? StudentLayout.Stroke2 : StudentLayout.Stroke1);
            var color = _hasError ? UiStyle.ErrorText : _code.Focused ? UiStyle.Primary : UiStyle.Border;
            int inset = (thickness + 1) / 2;
            var bounds = Rectangle.Inflate(_input.ClientRectangle, -inset, -inset);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            using var outline = StudentLayout.RoundedRectangle(this, bounds, StudentLayout.ButtonRadius);
            using var pen = new Pen(color, thickness);
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(pen, outline);
            e.Graphics.Restore(state);
        };
        _input.Click += (_, _) => _code.Focus();
        _code.GotFocus += (_, _) => _input.Invalidate();
        _code.LostFocus += (_, _) => _input.Invalidate();
        _code.TextChanged += (_, _) => SetMessage(string.Empty, false);
        _view.Click += (_, _) => Identify();
        _close.Click += (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty);
        _viewport.SizeChanged += (_, _) => LayoutCard();
        _input.SizeChanged += (_, _) =>
        {
            PositionInput();
            _input.Invalidate();
        };
        DpiChangedAfterParent += (_, _) => LayoutCard();
        SetMessage(string.Empty, false);
    }

    private int S(int value) => StudentLayout.Scale(this, value);

    private static void AddRow(TableLayoutPanel table, Control control, SizeType type = SizeType.AutoSize)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(type));
        control.Dock = DockStyle.Fill;
        control.Margin = Padding.Empty;
        control.TabIndex = row;
        table.Controls.Add(control, 0, row);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LayoutCard();
        _code.Focus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape) return true;
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        // Xử lý Enter từ ô nhập trước khi Form chuyển phím tới nút mặc định.
        if (keyData == Keys.Enter && (_code.Focused || _checking))
        {
            Identify();
            return true;
        }
        return base.ProcessDialogKey(keyData);
    }

    private void LayoutCard()
    {
        if (_layingOut) return;
        _layingOut = true;
        _viewport.SuspendLayout();
        _viewport.AutoScroll = false;
        _center.SuspendLayout();
        _card.SuspendLayout();
        _form.SuspendLayout();
        try
        {
            int inset = S(StudentLayout.Space24);
            int width = Math.Max(S(280), Math.Min(S(CardWidth), _viewport.ClientSize.Width - inset * 2));
            int edge = S(StudentLayout.Stroke1);
            _card.Padding = new Padding(edge);
            _form.Padding = new Padding(S(CardPadding), S(StudentLayout.Space8),
                S(CardPadding), S(StudentLayout.Space12));
            int formWidth = width - edge * 2;
            int textWidth = Math.Max(1, formWidth - _form.Padding.Horizontal);
            foreach (var label in _form.Controls.OfType<Label>())
                label.MaximumSize = new Size(textWidth, 0);
            _form.Controls[0].Margin = new Padding(0, 0, 0, S(StudentLayout.Space4));
            _form.Controls[1].Margin = new Padding(0, 0, 0, S(StudentLayout.Space24 + StudentLayout.Space12));
            _lookupRow.Controls[0].Margin = new Padding(0, 0, S(StudentLayout.Space12), 0);
            _form.RowStyles[2].Height = S(FieldHeight);
            _messageHost.Margin = new Padding(0, S(StudentLayout.Space4), 0, 0);
            int messagePadding = S(StudentLayout.Space8);
            _message.Padding = new Padding(messagePadding, S(StudentLayout.Stroke2), messagePadding, S(StudentLayout.Stroke2));
            _form.RowStyles[3].Height = MeasureMessageHeight(textWidth - messagePadding * 2)
                + _message.Padding.Vertical + _messageHost.Margin.Vertical;
            int buttonGap = S(StudentLayout.Space12);
            _view.Margin = new Padding(buttonGap, 0, 0, 0);
            _lookupRow.ColumnStyles[2].Width = Math.Max(S(152), _view.MinimumSize.Width) + buttonGap;
            _close.Margin = Padding.Empty;
            _form.RowStyles[4].Height = S(StudentLayout.ButtonHeight);
            int height = _form.GetPreferredSize(new Size(formWidth, 0)).Height + edge * 2;
            _content.RowStyles[0].Height = height;
            _help.MaximumSize = new Size(width, 0);
            _help.Margin = new Padding(0, S(StudentLayout.Space16), 0, 0);
            int contentHeight = height + _help.GetPreferredSize(new Size(width, 0)).Height + _help.Margin.Vertical;
            _center.Padding = new Padding(inset);
            _center.ColumnStyles[1].Width = width;
            _center.RowStyles[1].Height = contentHeight;
            _center.Width = Math.Max(_viewport.Width - (contentHeight + inset * 2 > _viewport.Height ? SystemInformation.VerticalScrollBarWidth : 0), width + inset * 2);
            _center.Height = Math.Max(_viewport.Height, contentHeight + inset * 2);
            _viewport.AutoScrollMinSize = new Size(width + inset * 2, contentHeight + inset * 2);
            _card.Invalidate();
        }
        finally
        {
            _form.ResumeLayout(true);
            _card.ResumeLayout(true);
            _center.ResumeLayout(true);
            _viewport.AutoScroll = true;
            _viewport.ResumeLayout(true);
            _layingOut = false;
        }
        PositionInput();
    }

    private void PositionInput()
    {
        // Căn giữa chữ theo chiều cao font thực tế, giữ lề ô nhập theo DPI.
        _code.SetBounds(S(StudentLayout.Space12), Math.Max(0, (_input.Height - _code.PreferredHeight) / 2),
            Math.Max(1, _input.Width - S(StudentLayout.Space12) * 2), _code.PreferredHeight);
    }

    private async void Identify()
    {
        if (_checking) return;
        string code = _code.Text.Trim();
        if (code.Length == 0)
        {
            ShowError(EmptyCodeMessage);
            return;
        }
        _checking = true;
        // Giữ focus ở ô nhập để Enter lặp không kích hoạt nút đóng.
        _code.Focus();
        _code.ReadOnly = true;
        _view.Enabled = false;
        _close.Enabled = false;
        _view.Text = "Đang kiểm tra…";
        UseWaitCursor = true;
        SetMessage(CheckingMessage, false);
        User? student = null;
        string? error = null;
        try
        {
            (student, error) = await Task.Run(() => _service.LookupStudent(code));
        }
        catch (Exception ex)
        {
            error = StudentError.Describe(ex, LookupErrorMessage);
        }
        finally
        {
            _checking = false;
            if (!IsDisposed)
            {
                UseWaitCursor = false;
                _code.ReadOnly = false;
                _view.Text = ViewText;
                _view.Enabled = true;
                _close.Enabled = true;
            }
        }
        // Form thay và Dispose màn hiện tại ngay trong sự kiện thành công.
        if (IsDisposed) return;
        if (error is not null) ShowError(error);
        else if (student is not null) StudentIdentified?.Invoke(student);
    }

    private void SetMessage(string text, bool error)
    {
        _hasError = error;
        _messageHost.Visible = text.Length > 0;
        _message.Text = error ? ErrorPrefix + text : text;
        _message.ForeColor = error ? UiStyle.ErrorText : UiStyle.TextMuted;
        _message.BackColor = error ? UiStyle.ErrorBack : StudentLayout.SelectionBack;
        _input.Invalidate();
        AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
    }

    private void ShowError(string message)
    {
        SetMessage(message, true);
        _code.Focus();
        _code.SelectAll();
    }

    private int MeasureMessageHeight(int width)
    {
        using var graphics = _message.CreateGraphics();
        var available = new Size(Math.Max(1, width), int.MaxValue);
        int Measure(string text) => TextRenderer.MeasureText(graphics, text, _message.Font,
            available, MessageLabel.DrawFlags).Height;

        // Dành chỗ cho lỗi dài ngay khi bố cục được tính, trước khi trạng thái thay đổi.
        return new[]
        {
            Measure(CheckingMessage),
            Measure(ErrorPrefix + EmptyCodeMessage), Measure(ErrorPrefix + LookupErrorMessage),
            Measure(ErrorPrefix + StudentExamService.StudentNotFoundMessage),
            Measure(ErrorPrefix + StudentError.DuplicateStudentCodeMessage)
        }.Max();
    }

    private sealed class MessageLabel : Label
    {
        internal const TextFormatFlags DrawFlags =
            TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(UiStyle.Surface);
            if (Width <= 0 || Height <= 0) return;
            using var shape = StudentLayout.RoundedRectangle(this, ClientRectangle, StudentLayout.ButtonRadius);
            using var fill = new SolidBrush(BackColor);
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(fill, shape);
            e.Graphics.Restore(state);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var bounds = new Rectangle(Padding.Left, Padding.Top,
                Width - Padding.Horizontal, Height - Padding.Vertical);
            if (bounds.Width > 0 && bounds.Height > 0)
                TextRenderer.DrawText(e.Graphics, Text, Font, bounds, ForeColor, DrawFlags);
        }
    }

    private sealed class AlignedLabel : Label
    {
        internal static readonly Font Semibold = new("Segoe UI Semibold", StudentLayout.SubheadingFontSize);
        private const TextFormatFlags DrawFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;


        public AlignedLabel(string caption, Font font)
        {
            Text = caption;
            Font = font;
            AutoSize = true;
            UseMnemonic = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var alignment = TextAlign == ContentAlignment.TopCenter ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                DrawFlags | TextFormatFlags.WordBreak | alignment);
        }
    }

    private sealed class IdentityCard : TableLayoutPanel
    {
        internal Control? HeaderDescription;

        public IdentityCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            int stroke = StudentLayout.Scale(this, StudentLayout.Stroke1);
            var bounds = new Rectangle(stroke, stroke, Width - stroke * 2, Height - stroke * 2);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            using var path = StudentLayout.RoundedRectangle(this, bounds);
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var surface = new SolidBrush(UiStyle.Surface);
            using var border = new Pen(UiStyle.TextMuted, stroke);
            e.Graphics.FillPath(surface, path);
            if (HeaderDescription is { Parent: Control form } description)
            {
                int bandHeight = form.Top + description.Bottom
                    + StudentLayout.Scale(this, StudentLayout.Space4);
                var clip = e.Graphics.Save();
                e.Graphics.SetClip(new Rectangle(0, 0, Width, bandHeight), CombineMode.Intersect);
                using var tint = new SolidBrush(StudentLayout.CardHeaderBack);
                e.Graphics.FillPath(tint, path);
                e.Graphics.Restore(clip);
            }
            e.Graphics.DrawPath(border, path);
            e.Graphics.Restore(state);
        }
    }
}
