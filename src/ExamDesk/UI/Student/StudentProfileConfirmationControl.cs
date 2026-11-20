using System.Drawing.Drawing2D;
using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

internal sealed class StudentProfileConfirmationControl : UserControl
{
    private const int CardWidth = 680;
    private readonly Panel _viewport = new BufferedStudentPanel() { Dock = DockStyle.Fill, AutoScroll = true, TabStop = false };
    private readonly ProfileCard _page = new()
    {
        Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
        ColumnCount = 1, TabStop = false
    };
    private readonly Label _status;
    private readonly TableLayoutPanel _actions = new BufferedStudentTable()
    {
        Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, TabStop = false
    };

    internal event EventHandler? Confirmed;
    internal event EventHandler? RetryRequested;

    internal StudentProfileConfirmationControl(User student)
    {
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.None;
        BackColor = UiStyle.Background;
        AccessibleName = "Xác nhận hồ sơ sinh viên";
        _page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var title = StudentLayout.Label("Xác nhận hồ sơ sinh viên", StudentLayout.TitleFontSize, FontStyle.Bold);
        title.TextAlign = ContentAlignment.MiddleCenter;
        AddRow(title);
        _page.HeaderTitle = title;
        _status = StudentLayout.Label("Đã tìm thấy hồ sơ sinh viên. Hãy kiểm tra thông tin bên dưới.");
        _status.BackColor = UiStyle.SuccessBack;
        _status.ForeColor = UiStyle.SuccessText;
        AddRow(_status);
        AddRow(StudentLayout.Label(student.FullName, StudentLayout.PageTitleFontSize, FontStyle.Bold));
        AddRow(StudentLayout.Label($"Mã số sinh viên: {student.StudentCode}"));
        string className = string.IsNullOrWhiteSpace(student.ClassName) ? "Chưa có thông tin" : student.ClassName;
        AddRow(StudentLayout.Label($"Lớp: {className}"));

        var confirm = StudentLayout.CreateButton("Đúng thông tin, tiếp tục",
            (_, _) => Confirmed?.Invoke(this, EventArgs.Empty), StudentButtonKind.Primary, StudentArrow.Right);
        var retry = StudentLayout.CreateButton("Nhập lại mã",
            (_, _) => RetryRequested?.Invoke(this, EventArgs.Empty), StudentButtonKind.Secondary);
        retry.TabIndex = 0;
        confirm.TabIndex = 1;
        _actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        retry.Anchor = AnchorStyles.Left;
        confirm.Anchor = AnchorStyles.Right;
        _actions.Controls.Add(retry, 0, 0);
        _actions.Controls.Add(confirm, 1, 0);
        AddRow(_actions);

        _viewport.Controls.Add(_page);
        Controls.Add(_viewport);
        _viewport.SizeChanged += (_, _) => LayoutPage();
        DpiChangedAfterParent += (_, _) => LayoutPage();
        Load += (_, _) => LayoutPage();
    }

    private void AddRow(Control control)
    {
        int row = _page.RowCount++;
        _page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        control.Dock = DockStyle.Fill;
        _page.Controls.Add(control, 0, row);
    }

    private void LayoutPage()
    {
        int S(int value) => StudentLayout.Scale(this, value);
        int inset = S(StudentLayout.Space24);
        int side = Math.Max(inset, (_viewport.Width - S(CardWidth)) / 2);
        _page.Padding = new Padding(inset, S(StudentLayout.Space16), inset, inset);
        _status.Padding = new Padding(S(StudentLayout.Space12));
        int width = Math.Max(1, _viewport.Width - 2 * (side + inset));
        foreach (Control row in _page.Controls)
        {
            int bottomGap = row == _page.HeaderTitle ? StudentLayout.Space12 + StudentLayout.Space8 : StudentLayout.Space12;
            row.Margin = new Padding(0, 0, 0, S(bottomGap));
            if (row is Label label) label.MaximumSize = new Size(width, 0);
        }
        _actions.Margin = new Padding(0, S(StudentLayout.Space8), 0, 0);
        foreach (Control button in _actions.Controls)
            button.Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));

        int cardWidth = Math.Max(1, _viewport.Width - 2 * side);
        int cardHeight = _page.GetPreferredSize(new Size(cardWidth, 0)).Height;
        // Giữ lề tối thiểu để vùng nội dung vẫn cuộn được khi cửa sổ thấp.
        int upwardOffset = S(72);
        int top = Math.Max(inset, (_viewport.Height - cardHeight) / 2 - upwardOffset);
        _viewport.Padding = new Padding(side, top, side, inset);
    }

    private sealed class ProfileCard : TableLayoutPanel
    {
        internal Control? HeaderTitle;

        public ProfileCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            int stroke = StudentLayout.Scale(this, StudentLayout.Stroke1);
            var bounds = Rectangle.Inflate(ClientRectangle, -stroke, -stroke);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using var shape = StudentLayout.RoundedRectangle(this, bounds);
            using var fill = new SolidBrush(UiStyle.Surface);
            using var border = new Pen(UiStyle.TextMuted, stroke);
            var state = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(fill, shape);
            if (HeaderTitle is { } title)
            {
                int bandHeight = title.Bottom + title.Top;
                var clip = e.Graphics.Save();
                e.Graphics.SetClip(new Rectangle(0, 0, Width, bandHeight), CombineMode.Intersect);
                using var tint = new SolidBrush(StudentLayout.CardHeaderBack);
                e.Graphics.FillPath(tint, shape);
                e.Graphics.Restore(clip);
            }
            e.Graphics.DrawPath(border, shape);
            e.Graphics.Restore(state);
        }
    }

}
