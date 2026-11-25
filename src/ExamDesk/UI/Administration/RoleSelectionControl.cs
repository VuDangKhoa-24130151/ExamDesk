using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;
using ExamDesk.UI.Student;

namespace ExamDesk.UI.Administration;

// AUTH-01 • Bạn 1 phụ trách: Màn hình chọn vai trò (Giảng viên / Sinh viên).
public partial class RoleSelectionControl : UserControl
{
    private bool _layingOut;
    private bool _layoutPending;
    public event EventHandler? LecturerLoggedIn;

    public string LoggedInUsername { get; private set; } = string.Empty;

    public RoleSelectionControl()
    {
        InitializeComponent();
        _viewport.ClientSizeChanged += (_, _) => QueueLayout();
        DpiChangedAfterParent += (_, _) => LayoutWelcome();
        _topBar.Paint += (_, e) => DrawDivider(e.Graphics, _topBar, _topBar.Height - 1);
        _footer.Paint += (_, e) => DrawDivider(e.Graphics, _footer, 0);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LayoutWelcome();
    }

    internal void SetLoginStatus(string username)
    {
        _loginStatus.Text = string.IsNullOrWhiteSpace(username) ? "Chưa đăng nhập" : $"Giảng viên: {username}";
    }

    private static void DrawDivider(Graphics graphics, Control control, int y)
    {
        using var pen = new Pen(UiStyle.Border);
        graphics.DrawLine(pen, 0, y, control.Width, y);
    }

    private void OnLogoBadgePaint(object? sender, PaintEventArgs e)
    {
        if (_logoBadge.Width < 2 || _logoBadge.Height < 2) return;
        // Tự vẽ bo góc thay vì dùng Region để góc chữ U không bị răng cưa.
        e.Graphics.Clear(UiStyle.Surface);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = CreateRoundedPath(new RectangleF(0.5f, 0.5f, _logoBadge.Width - 1f, _logoBadge.Height - 1f), 6f * DeviceDpi / 96f);
        using var fill = new SolidBrush(UiStyle.Primary);
        e.Graphics.FillPath(fill, path);
        TextRenderer.DrawText(e.Graphics, "U", UiStyle.SectionTitleFont, _logoBadge.ClientRectangle, Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private void QueueLayout()
    {
        if (_layoutPending || !IsHandleCreated || IsDisposed) return;
        _layoutPending = true;
        // Đợi WinForms cập nhật vùng cuộn xong rồi mới căn lại thẻ.
        BeginInvoke(() =>
        {
            _layoutPending = false;
            if (!IsDisposed) LayoutWelcome();
        });
    }

    private void LayoutWelcome()
    {
        if (_layingOut || _viewport.ClientSize.Width <= 0) return;
        _layingOut = true;
        _viewport.SuspendLayout();
        _contentLayout.SuspendLayout();
        _cardsLayout.SuspendLayout();
        try
        {
            int S(int value) => (int)Math.Round(value * DeviceDpi / 96f);
            _topBar.Height = S(48);
            _topBar.Padding = new Padding(S(24), S(8), S(24), S(8));
            _logoBadge.Size = new Size(S(28), S(28));
            _footer.Height = S(40);
            _footer.Padding = new Padding(S(24), 0, S(24), 0);
            _lblTitle.Margin = new Padding(0, 0, 0, S(10));
            _lblSubtitle.Margin = new Padding(0, 0, 0, S(32));

            int inset = S(24), gap = S(24);
            int width = Math.Max(S(280), Math.Min(S(800), _viewport.ClientSize.Width - inset * 2));
            bool stacked = width < S(680);

            // Giới hạn chiều ngang để thẻ không bị kéo quá to khi phóng cửa sổ.
            _contentLayout.Width = width;
            _cardsLayout.ColumnCount = stacked ? 1 : 2;
            _cardsLayout.RowCount = stacked ? 2 : 1;
            _cardsLayout.ColumnStyles.Clear();
            _cardsLayout.RowStyles.Clear();
            for (int i = 0; i < _cardsLayout.ColumnCount; i++)
                _cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, stacked ? 100 : 50));
            for (int i = 0; i < _cardsLayout.RowCount; i++)
                _cardsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, stacked ? 50 : 100));
            _cardsLayout.SetCellPosition(_studentCard, stacked ? new TableLayoutPanelCellPosition(0, 1) : new(1, 0));
            _lecturerCard.Margin = stacked ? new Padding(0, 0, 0, gap / 2) : new Padding(0, 0, gap / 2, 0);
            _studentCard.Margin = stacked ? new Padding(0, gap / 2, 0, 0) : new Padding(gap / 2, 0, 0, 0);

            int cardWidth = stacked ? width : (width - gap) / 2;
            int cardHeight = S(236), buttonHeight = S(40);
            _btnLecturerLogin.Height = _btnStudentArea.Height = buttonHeight;
            _btnLecturerLogin.MinimumSize = _btnStudentArea.MinimumSize = new Size(0, buttonHeight);

            foreach (var card in new[] { _lecturerCard, _studentCard })
            {
                card.Padding = new Padding(S(24));
                var content = (TableLayoutPanel)card.Controls[0];
                int textWidth = Math.Max(1, cardWidth - card.Padding.Horizontal - content.Padding.Horizontal);
                int preferredHeight = card.Padding.Vertical + content.Padding.Vertical;
                foreach (Control item in content.Controls)
                {
                    if (item is Label label)
                    {
                        label.MaximumSize = new Size(textWidth, 0);
                        preferredHeight += label.GetPreferredSize(new Size(textWidth, 0)).Height;
                    }
                    else preferredHeight += Math.Max(buttonHeight, item.MinimumSize.Height);
                    preferredHeight += item.Margin.Vertical;
                }
                cardHeight = Math.Max(cardHeight, preferredHeight);
            }

            // Thiếu chỗ thì xếp hai thẻ dọc và cho cuộn, tránh mất nút đăng nhập.
            int cardsHeight = stacked ? cardHeight * 2 + gap : cardHeight;
            _cardsLayout.Height = cardsHeight;
            int height = cardsHeight;
            foreach (var label in _contentLayout.Controls.OfType<Label>())
            {
                label.MaximumSize = new Size(width, 0);
                height += label.GetPreferredSize(new Size(width, 0)).Height + label.Margin.Vertical;
            }
            _contentLayout.Height = height;
            _viewport.AutoScrollMinSize = new Size(0, height + inset * 2);
            int left = Math.Max(inset, (_viewport.ClientSize.Width - width) / 2);
            int top = Math.Max(inset, (int)((_viewport.ClientSize.Height - height) * 0.35));
            _contentLayout.Location = new Point(left + _viewport.AutoScrollPosition.X, top + _viewport.AutoScrollPosition.Y);
        }
        finally
        {
            _cardsLayout.ResumeLayout(true);
            _contentLayout.ResumeLayout(true);
            _viewport.ResumeLayout(true);
            _layingOut = false;
        }
    }

    private void OnLecturerLoginClick(object? sender, EventArgs e)
    {
        try
        {
            using var loginForm = new LoginForm();
            ShowStandaloneForm(loginForm, matchMaximized: false, result =>
            {
                if (result == DialogResult.OK)
                {
                    LoggedInUsername = loginForm.LoggedInUsername;
                    LecturerLoggedIn?.Invoke(this, EventArgs.Empty);
                }
            });
        }
        catch (Exception ex)
        {
            if (FindForm() is MainForm mainForm)
                mainForm.ShowScreen(MainForm.CreateErrorScreen("Đăng nhập giảng viên", ex.Message));
        }
    }

    private void OnStudentAreaClick(object? sender, EventArgs e)
    {
        try
        {
            using var studentForm = new StudentExamForm();
            ShowStandaloneForm(studentForm, matchMaximized: true);
        }
        catch (Exception ex)
        {
            if (FindForm() is MainForm mainForm)
                mainForm.ShowScreen(MainForm.CreateErrorScreen("Khu vực sinh viên", ex.Message));
        }
    }

    private void ShowStandaloneForm(Form form, bool matchMaximized, Action<DialogResult>? onClosed = null)
    {
        var owner = FindForm();
        if (owner is null)
        {
            onClosed?.Invoke(form.ShowDialog());
            return;
        }

        // Ẩn cửa sổ chính khi qua màn đăng nhập/sinh viên để khỏi lộ màn chọn vai trò ở dưới.
        form.StartPosition = FormStartPosition.CenterScreen;
        form.ShowInTaskbar = true;
        if (matchMaximized && owner.WindowState == FormWindowState.Maximized)
        {
            form.WindowState = FormWindowState.Maximized;
        }

        owner.Hide();
        try
        {
            var result = form.ShowDialog();
            // Đổi màn hình (nếu đăng nhập xong) trước khi hiện lại Form chính để không bị nháy màn cũ.
            onClosed?.Invoke(result);
        }
        finally
        {
            if (!owner.IsDisposed)
            {
                owner.Show();
                owner.Activate();
            }
        }
    }

    private static GraphicsPath CreateRoundedPath(RectangleF bounds, float radius)
    {
        var path = new GraphicsPath();
        float r = Math.Clamp(radius, 0f, Math.Min(bounds.Width, bounds.Height) / 2f);
        if (r <= 0.1f) { path.AddRectangle(bounds); return path; }
        float d = r * 2f;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    // Nút tự vẽ bo góc bằng AntiAlias để viền không bị cắt góc như khi dùng Region.
    private sealed class RoundedButton : Button
    {
        private readonly bool _primary;
        private bool _hover, _down;

        public RoundedButton(string text, bool primary)
        {
            _primary = primary;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            AutoSize = false;
            UseMnemonic = false;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Text = AccessibleName = text;
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = _down = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _down = true; Invalidate(); } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (_down) { _down = false; Invalidate(); } }
        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (ClientSize.Width < 4 || ClientSize.Height < 4) return;

            float scale = DeviceDpi / 96f;
            float stroke = Math.Max(1f, 1.25f * scale);
            float inset = _primary ? 0.5f : stroke / 2f;
            float w = ClientSize.Width - (_primary ? 1f : stroke);
            float h = ClientSize.Height - (_primary ? 1f : stroke);
            if (w <= 0 || h <= 0) return;

            Color fillColor = _primary
                ? (_down ? UiStyle.PrimaryPressed : (_hover ? UiStyle.PrimaryHover : UiStyle.Primary))
                : (_down ? UiStyle.SecondaryPressed : (_hover ? UiStyle.SecondaryHover : UiStyle.Surface));
            Color borderColor = _down ? UiStyle.PrimaryPressed : (_hover ? UiStyle.PrimaryHover : UiStyle.Primary);
            Color textColor = _primary ? Color.White : borderColor;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedPath(new RectangleF(inset, inset, w, h), 6f * scale))
            using (var fill = new SolidBrush(fillColor))
            {
                e.Graphics.FillPath(fill, path);
                if (!_primary)
                {
                    using var border = new Pen(borderColor, stroke);
                    e.Graphics.DrawPath(border, path);
                }
            }

            if (Focused && ShowFocusCues)
            {
                float fi = inset + 2.5f * scale;
                using var fp = CreateRoundedPath(new RectangleF(fi, fi, ClientSize.Width - fi * 2f, ClientSize.Height - fi * 2f), 4f * scale);
                using var pen = new Pen(_primary ? Color.FromArgb(200, Color.White) : UiStyle.Primary, Math.Max(1f, scale));
                e.Graphics.DrawPath(pen, fp);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }
}
