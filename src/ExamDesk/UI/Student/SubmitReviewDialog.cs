using System.ComponentModel;
using System.Drawing.Drawing2D;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SubmitReviewDialog : Form
{
    private bool _isSubmitting;

    public int? QuestionToReview { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool IsSubmitting
    {
        get => _isSubmitting;
        set
        {
            _isSubmitting = value;
            if (_continueButton is not null) _continueButton.Enabled = !value;
            if (_confirmButton is not null) _confirmButton.Enabled = !value;
            CancelButton = value ? null : _continueButton;
        }
    }

    public SubmitReviewDialog()
    {
        InitializeComponent();
#if DEBUG
        ShowAttempt(new StudentAttemptState());
#else
        _confirmButton.Enabled = false;
#endif
    }

    internal SubmitReviewDialog(StudentAttemptState attempt)
    {
        InitializeComponent();
        ShowAttempt(attempt);
    }

    private void ApplyDialogLayout()
    {
        int S(int value) => StudentLayout.Scale(this, value);
        ClientSize = new Size(S(StudentLayout.SubmitDialogWidth), S(StudentLayout.SubmitDialogHeight));
        MinimumSize = new Size(S(StudentLayout.SubmitDialogMinWidth), S(StudentLayout.SubmitDialogMinHeight));

        _headerBar.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space16),
            S(StudentLayout.Space24), S(StudentLayout.Space16));
        _titleLabel.Font = StudentLayout.GetLabelFont(StudentLayout.TitleFontSize, FontStyle.Bold);
        _titleLabel.Margin = new Padding(0, 0, S(StudentLayout.Space16), S(StudentLayout.Space4));
        _unansweredSummary.Margin = Padding.Empty;

        _warning.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
        _warning.Size = new Size(S(StudentLayout.DialogClockWidth), S(StudentLayout.ChipHeight));
        _warning.Margin = new Padding(S(StudentLayout.Space12), 0, 0, 0);

        _scrollContent.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space16),
            S(StudentLayout.Space24), S(StudentLayout.Space16));
        _unansweredTitle.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
        _unansweredTitle.Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));
        _unansweredGroup.Margin = new Padding(0, 0, 0, S(StudentLayout.Space16));

        _flaggedTitle.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
        _flaggedTitle.Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));
        _flaggedGroup.Margin = Padding.Empty;

        _shell.RowStyles[2].Height = S(StudentLayout.BarHeight);
        _footerBar.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Stroke1),
            S(StudentLayout.Space24), 0);
        _continueButton.Margin = _confirmButton.Margin = Padding.Empty;
        _continueButton.FitSize();
        _confirmButton.FitSize();

        foreach (Control control in _unansweredGroup.Controls)
            if (control is QuestionChip chip) chip.FitSize();
        foreach (Control control in _flaggedGroup.Controls)
            if (control is QuestionChip chip) chip.FitSize();
    }

    private void FitScrollContent()
    {
        if (_scrollViewport.ClientSize.Width <= 0) return;
        int innerWidth = Math.Max(100, _scrollViewport.ClientSize.Width
            - _scrollContent.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth);
        _unansweredGroup.MaximumSize = new Size(innerWidth, 0);
        _flaggedGroup.MaximumSize = new Size(innerWidth, 0);
        _unansweredSummary.MaximumSize = new Size(
            Math.Max(100, _headerBar.ClientSize.Width - StudentLayout.Scale(this, StudentLayout.DialogHeaderRightReserve)), 0);
    }

    private void ShowAttempt(StudentAttemptState attempt)
    {
        int unansweredCount = attempt.Answers.Length - attempt.Answered;
        _unansweredSummary.Text = $"Còn {unansweredCount} câu chưa trả lời";
        _unansweredSummary.ForeColor = unansweredCount > 0 ? UiStyle.WarningText : UiStyle.TextMuted;
        _unansweredSummary.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize,
            unansweredCount > 0 ? FontStyle.Bold : FontStyle.Regular);

        _unansweredGroup.Controls.Clear();
        _flaggedGroup.Controls.Clear();

        for (int index = 0; index < attempt.Answers.Length; index++)
        {
            if (!attempt.Answers[index].HasValue)
                _unansweredGroup.Controls.Add(CreateChip(index, flagged: attempt.Flags[index], answered: false));
            if (attempt.Flags[index])
                _flaggedGroup.Controls.Add(CreateChip(index, flagged: true, answered: attempt.Answers[index].HasValue));
        }

        if (_unansweredGroup.Controls.Count == 0)
        {
            var empty = StudentLayout.Label("Đã trả lời tất cả câu hỏi.", StudentLayout.SmallFontSize);
            empty.ForeColor = UiStyle.TextMuted;
            empty.Margin = new Padding(0, 0, 0, StudentLayout.Scale(this, StudentLayout.Space4));
            _unansweredGroup.Controls.Add(empty);
        }

        if (_flaggedGroup.Controls.Count == 0)
        {
            var empty = StudentLayout.Label("Không có câu đánh dấu xem lại.", StudentLayout.SmallFontSize);
            empty.ForeColor = UiStyle.TextMuted;
            empty.Margin = new Padding(0, 0, 0, StudentLayout.Scale(this, StudentLayout.Space4));
            _flaggedGroup.Controls.Add(empty);
        }

        void UpdateClock()
        {
            var remaining = attempt.TimeRemaining;
            bool lowTime = remaining < TimeSpan.FromMinutes(5);
            _warning.Text = $"Còn {attempt.Remaining}";
            // Không dùng màu lỗi trong dialog xác nhận nộp.
            _warning.ForeColor = lowTime ? UiStyle.WarningText : UiStyle.Primary;
            _warning.BackColor = lowTime ? UiStyle.WarningBack : UiStyle.Background;
            _warning.Invalidate();
        }

        UpdateClock();
        _clock.Tick += (_, _) => UpdateClock();
        Shown += (_, _) =>
        {
            UiStyle.ClampToWorkingArea(this);
            FitScrollContent();
            _continueButton.Focus();
            _clock.Start();
        };
    }

    private QuestionChip CreateChip(int index, bool flagged, bool answered = false)
    {
        var chip = new QuestionChip(index, flagged, answered);
        chip.Click += (_, _) =>
        {
            if (_isSubmitting) return;
            QuestionToReview = index;
            DialogResult = DialogResult.Cancel;
            Close();
        };
        return chip;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if ((keyData & Keys.KeyCode) == Keys.Escape)
        {
            if (_isSubmitting) return true;
            DialogResult = DialogResult.Cancel;
            Close();
            return true;
        }
        if (keyData == Keys.Enter && ActiveControl is Button button)
        {
            button.PerformClick();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_isSubmitting && DialogResult != DialogResult.OK)
        {
            e.Cancel = true;
            return;
        }
        base.OnFormClosing(e);
    }

    private sealed class DialogClockPill : Label
    {
        internal DialogClockPill() => SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw, true);

        protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(UiStyle.Surface);

        protected override void OnPaint(PaintEventArgs e)
        {
            int inset = StudentLayout.Scale(this, StudentLayout.Stroke1);
            if (Width <= inset * 2 || Height <= inset * 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pill = StudentLayout.RoundedRectangle(this,
                Rectangle.Inflate(ClientRectangle, -inset, -inset), StudentLayout.ChipRadius);
            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, pill);
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
    }

    private sealed class QuestionChip : Button
    {
        private readonly bool _flagged;
        private readonly bool _answered;
        private bool _hovered;
        private bool _pressed;

        internal QuestionChip(int index, bool flagged, bool answered = false)
        {
            _flagged = flagged;
            _answered = answered;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            AutoSize = false;
            UseMnemonic = false;
            Cursor = Cursors.Hand;
            Text = $"Câu {index + 1}";
            AccessibleName = $"Quay lại câu {index + 1}";
            AccessibleDescription = answered
                ? (flagged ? "Đã trả lời, đánh dấu xem lại" : "Đã trả lời")
                : (flagged ? "Chưa trả lời, đánh dấu xem lại" : "Chưa trả lời");
            FitSize();
        }

        internal void FitSize()
        {
            int S(int value) => StudentLayout.Scale(this, value);
            Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Regular);
            Padding = new Padding(S(StudentLayout.ButtonPaddingX), 0, S(StudentLayout.ButtonPaddingX), 0);
            int textWidth = TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding).Width;
            int width = Math.Max(S(StudentLayout.ChipMinWidth), textWidth + Padding.Horizontal + S(StudentLayout.Space4));
            MinimumSize = Size = new Size(width, S(StudentLayout.ChipHeight));
            Margin = new Padding(0, 0, S(StudentLayout.Space8), S(StudentLayout.Space8));
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = Enabled; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = _pressed = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left && Enabled) { _pressed = true; Invalidate(); }
            base.OnMouseDown(mevent);
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (_pressed) { _pressed = false; Invalidate(); }
            base.OnMouseUp(mevent);
        }
        protected override bool IsInputKey(Keys keyData) =>
            keyData == Keys.Enter || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Enter && Enabled)
            {
                kevent.SuppressKeyPress = true;
                PerformClick();
                return;
            }
            if (kevent.KeyCode == Keys.Space && Enabled) { _pressed = true; Invalidate(); }
            base.OnKeyDown(kevent);
        }
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space && _pressed) { _pressed = false; Invalidate(); }
            base.OnKeyUp(kevent);
        }
        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); _pressed = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            int S(int value) => StudentLayout.Scale(this, value);
            if (Width <= S(StudentLayout.Space4) || Height <= S(StudentLayout.Space4)) return;

            e.Graphics.Clear(UiStyle.Surface);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color backColor, borderColor, foreColor;
            if (_answered)
            {
                backColor = _pressed ? StudentLayout.ButtonPressedBack
                    : _hovered ? StudentLayout.SelectionBack : StudentLayout.AnsweredBack;
                borderColor = _pressed || _hovered ? UiStyle.Primary : StudentLayout.AnsweredBorder;
                foreColor = UiStyle.Primary;
            }
            else if (_flagged)
            {
                backColor = _pressed ? StudentLayout.WarningPressedBack
                    : _hovered ? StudentLayout.WarningHoverBack : StudentLayout.FlaggedBack;
                borderColor = StudentLayout.WarningBorder;
                foreColor = UiStyle.WarningText;
            }
            else
            {
                backColor = _pressed ? StudentLayout.ButtonPressedBack
                    : _hovered ? StudentLayout.SelectionBack : UiStyle.Background;
                borderColor = _pressed || _hovered ? UiStyle.Primary : UiStyle.Border;
                foreColor = _pressed || _hovered ? UiStyle.Primary : UiStyle.Text;
            }

            var bounds = Rectangle.Inflate(ClientRectangle, -S(StudentLayout.Stroke1), -S(StudentLayout.Stroke1));
            using var path = StudentLayout.RoundedRectangle(this, bounds, StudentLayout.ChipRadius);
            using var fill = new SolidBrush(backColor);
            using var border = new Pen(borderColor, S(StudentLayout.Stroke1));
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(border, path);

            if (Enabled && Focused && ShowFocusCues)
            {
                using var focusPen = new Pen(UiStyle.Primary, S(StudentLayout.Stroke2));
                e.Graphics.DrawPath(focusPen, path);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }
}
