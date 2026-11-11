using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    private const int QuestionGridColumns = 5;
    private const string FlagText = "⚑ Đánh dấu &xem lại";
    private const string UnflagText = "⚑ Bỏ đánh dấu &xem lại";
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private Button[] _numbers = [];
    private bool _fittingContent;
    private readonly StudentAttemptState _attempt = null!;
    private bool _rendering;
    private int _imageQuestion = -1;
    private bool _imageLoadFailed;
    private DateTime? _savedAt;
    private string? _saveError;
    private string? _saveNotice;
    private StudentAttemptClock? _saveClock;
    public event EventHandler? SubmitRequested;
    internal event EventHandler? ExitReviewRequested;
    internal event EventHandler? SaveExpired;
    internal event EventHandler? SaveCompleted;
    internal event EventHandler? DraftLocked;
#if DEBUG
    public StudentExamControl() : this(new StudentAttemptState()) { }
#else
    public StudentExamControl()
    {
        InitializeComponent();
        Enabled = false;
    }
#endif

    internal StudentExamControl(StudentAttemptState attempt)
    {
        _attempt = attempt;
        _savedAt = attempt.Draft?.History.LastOrDefault()?.AtUtc.LocalDateTime;
        InitializeComponent();
        ConfigureExam();
        RenderQuestion();
    }
    private void ConfigureExam()
    {
        _title.Text = _attempt.Title;
        _identity.Text = _attempt.Draft is { } draft
            ? $"{draft.StudentName} · {draft.StudentCode} · {draft.ClassName} · {draft.Questions.Count} câu"
            : $"{_attempt.Answers.Length} câu";
        if (_attempt.Submitted)
        {
            _section.Text = "XEM LẠI BÀI ĐÃ NỘP";
            _hint.Text = "Lựa chọn đã lưu khi nộp bài";
            _advice.Text = "Chọn số câu hoặc dùng Câu trước và Câu tiếp để xem lại bài.";
            _submit.Text = "← Về trạng thái nộp bài";
            _submit.Click += (_, _) => ExitReviewRequested?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _submit.Click += (_, _) => SubmitRequested?.Invoke(this, EventArgs.Empty);
            _submit.KeyDown += (_, e) =>
            {
                if (e.KeyCode is Keys.Space or Keys.Enter) e.SuppressKeyPress = true;
            };
        }

        _image.Click += (_, _) => ShowQuestionImage();
        _viewImage.Click += (_, _) => ShowQuestionImage();
        _previous.Click += (_, _) => GoToQuestion(_attempt.Current - 1);
        _next.Click += (_, _) => GoToQuestion(_attempt.Current + 1);
        _flag.CheckedChanged += (_, _) => ChangeFlag();
        _clearChoice.Click += (_, _) => ChangeChoice(null, _clearChoice);

        for (int i = 0; i < _options.Length; i++)
        {
            int choice = i;
            var option = _options[i];
            option.Click += (_, _) => Choose(choice);
            option.PreviewKeyDown += KeepArrowKeys;
            option.KeyDown += (_, e) => HandleOptionKey(choice, e);
        }

        _progressTrack.Paint += (_, e) =>
        {
            using var fill = new SolidBrush(UiStyle.Primary);
            float width = _progressTrack.ClientSize.Width * _attempt.Answered / (float)_attempt.Answers.Length;
            e.Graphics.FillRectangle(fill, 0, 0, width, _progressTrack.ClientSize.Height);
        };
        BuildQuestionGrid();

        _layout.SizeChanged += (_, _) => FitColumns();
        _questionViewport.SizeChanged += (_, _) => FitContent();
        _right.SizeChanged += (_, _) => FitContent();
        Load += (_, _) => FitColumns();
        DpiChangedAfterParent += (_, _) => FitColumns();
        if (!_attempt.Submitted)
        {
            _timer.Tick += Tick;
            _timer.Start();
        }
    }

    private void FitContent()
    {
        if (_fittingContent) return;
        _fittingContent = true;
        try
        {
            // Chừa chỗ cho thanh cuộn để độ rộng đáp án không đổi khi nội dung dài.
            int width = Math.Max(1, _questionViewport.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            _body.SuspendLayout();
            try
            {
                var textLimit = new Size(width, 0);
                _question.MaximumSize = _questionNumber.MaximumSize = _hint.MaximumSize =
                    _advice.MaximumSize = textLimit;
                foreach (var option in _options)
                    option.Size = new Size(width, option.MeasureHeight(width));
                _image.Width = Math.Min(width, StudentLayout.Scale(this, 480));
                _image.Height = Math.Min(StudentLayout.Scale(this, 220), _image.Width * 220 / 480);
            }
            finally { _body.ResumeLayout(true); }
            // Dành chỗ cho cả hai nhãn để thanh nút không đổi hàng khi đánh dấu câu.
            var flagTextWidth = TextRenderer.MeasureText(UnflagText, _flag.Font).Width;
            _flag.MinimumSize = new Size(flagTextWidth + _flag.Padding.Horizontal + StudentLayout.Scale(this, 8),
                StudentLayout.Scale(this, 38));
            foreach (var number in _numbers) number.MinimumSize = new Size(0, StudentLayout.Scale(this, 34));
            int sideWidth = Math.Max(1, _right.ClientSize.Width);
            _progress.MaximumSize = _legend.MaximumSize = new Size(sideWidth, 0);
        }
        finally { _fittingContent = false; }
    }

    private void FitColumns()
    {
        if (_layout.ClientSize.Width <= 0) return;
        var sideWidth = Math.Min(StudentLayout.Scale(this, 300), (int)(_layout.ClientSize.Width * 0.42f));
        if (_layout.ColumnStyles[1].Width != sideWidth)
            _layout.ColumnStyles[1].Width = sideWidth;
        FitContent();
    }

    private void BuildQuestionGrid()
    {
        while (_gridViewport.Controls.Count > 0) _gridViewport.Controls[0].Dispose();
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = QuestionGridColumns,
            RowCount = (_attempt.Answers.Length + QuestionGridColumns - 1) / QuestionGridColumns
        };
        for (int i = 0; i < QuestionGridColumns; i++)
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / QuestionGridColumns));
        for (int row = 0; row < grid.RowCount; row++)
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _numbers = new Button[_attempt.Answers.Length];
        for (int i = 0; i < _numbers.Length; i++)
        {
            int index = i;
            var number = new Button
            {
                Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, Padding = Padding.Empty,
                Margin = new Padding(2), AccessibleName = $"Câu {i + 1}"
            };
            _numbers[i] = number;
            number.Paint += (_, e) =>
            {
                var bounds = Rectangle.Inflate(number.ClientRectangle, -3, -2);
                if (_attempt.Answers[index].HasValue)
                    TextRenderer.DrawText(e.Graphics, "✓", UiStyle.SmallFont, bounds, UiStyle.SuccessText,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
                if (_attempt.Flags[index])
                    TextRenderer.DrawText(e.Graphics, "⚑", UiStyle.SmallFont, bounds, UiStyle.WarningText,
                        TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.NoPadding);
            };
            number.Click += (_, _) => GoToQuestion(index);
            number.PreviewKeyDown += KeepArrowKeys;
            number.KeyDown += (_, e) => HandleQuestionKey(index, e);
            grid.Controls.Add(number, i % QuestionGridColumns, i / QuestionGridColumns);
        }
        _gridViewport.Controls.Add(grid);
    }

    private void RenderQuestion()
    {
        _rendering = true;
        _questionNumber.Text = $"Câu {_attempt.Current + 1:00} / {_attempt.Answers.Length}";
        if (_attempt.Draft is { } draft)
        {
            var question = draft.Questions[_attempt.Current];
            _question.Text = question.Content;
            if(_imageQuestion != _attempt.Current)
            {
                _image.Image?.Dispose();
                _image.Image = null;
                _imageLoadFailed = false;
                _imageQuestion=_attempt.Current;
                if(!string.IsNullOrWhiteSpace(question.ImagePath))
                {
                    try
                    {
                        using var original=Image.FromFile(question.ImagePath);
                        _image.Image = new Bitmap(original);
                    }
                    catch(Exception ex) when(ex is IOException or ArgumentException or OutOfMemoryException or UnauthorizedAccessException)
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                        _imageLoadFailed = true;
                    }
                }
            }
            if (_imageLoadFailed)
                _question.Text += "\n[Không tải được ảnh minh họa. Hãy báo giảng viên.]";
            _image.Visible = _viewImage.Visible = _image.Image is not null;
            for (int i = 0; i < _options.Length; i++) _options[i].Text = question.Options[i];
        }
#if DEBUG
        else
        {
            _question.Text = "Kiểu dữ liệu nào trong C# dùng để lưu giá trị đúng / sai?";
            string[] answers = ["int", "bool", "string", "double"];
            for (int i = 0; i < _options.Length; i++) _options[i].Text = answers[i];
        }
#endif
        int? selected = _attempt.Answers[_attempt.Current];
        bool locked = _attempt.Draft?.PendingSubmission == true && !_attempt.Submitted;
        for (int i = 0; i < _options.Length; i++)
        {
            var option = _options[i];
            option.Checked = selected == i;
            option.Enabled = !locked;
            option.TabStop = !_attempt.Submitted && !locked && i == (selected ?? 0);
        }
        _flag.Checked = _attempt.Flags[_attempt.Current];
        _flag.Text = _flag.Checked ? UnflagText : FlagText;
        _previous.Enabled = _attempt.Current > 0;
        _next.Enabled = _attempt.Current < _attempt.Answers.Length-1;
        _flag.Enabled = !_attempt.Submitted && !locked;
        _clearChoice.Enabled = !_attempt.Submitted && !locked && selected.HasValue;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/{_attempt.Answers.Length} · Đánh dấu {_attempt.Flags.Count(flag => flag)}";
        _progressTrack.Invalidate();
        _saveStatus.Text = _saveError ?? _saveNotice ?? (_savedAt is DateTime savedAt ? $"Đã lưu lúc {savedAt:HH:mm:ss}" : "");
        _saveStatus.BackColor = _saveError is not null ? UiStyle.ErrorBack
            : _saveNotice is not null ? UiStyle.WarningBack : UiStyle.SuccessBack;
        _saveStatus.ForeColor = _saveError is not null ? UiStyle.ErrorText
            : _saveNotice is not null ? UiStyle.WarningText : UiStyle.SuccessText;
        _saveStatus.Visible = _saveStatus.Text.Length > 0;
        UpdateClock();
        for (int i = 0; i < _numbers.Length; i++)
        {
            bool answered = _attempt.Answers[i].HasValue;
            bool flagged = _attempt.Flags[i];
            _numbers[i].Text = (i + 1).ToString();
            _numbers[i].TabStop = i == _attempt.Current;
            _numbers[i].AccessibleDescription = $"{(answered ? "Đã trả lời" : "Chưa trả lời")}{(flagged ? ", đánh dấu xem lại" : "")}{(i == _attempt.Current ? ", đang xem" : "")}";
            _numbers[i].BackColor = answered ? UiStyle.SuccessBack : flagged ? UiStyle.WarningBack : UiStyle.Surface;
            _numbers[i].ForeColor = answered ? UiStyle.SuccessText : flagged ? UiStyle.WarningText : UiStyle.Text;
            _numbers[i].FlatAppearance.BorderColor = i == _attempt.Current ? UiStyle.Primary : UiStyle.Border;
            _numbers[i].FlatAppearance.BorderSize = i == _attempt.Current ? 2 : 1;
            _numbers[i].Invalidate();
        }
        FitContent();
        _rendering = false;
    }

    private void ShowQuestionImage()
    {
        if (_image.Image is not { } image || FindForm() is not { } owner) return;

        using var dialog = new Form
        {
            Text = $"Ảnh câu {_attempt.Current + 1}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(StudentLayout.Scale(this, 850), StudentLayout.Scale(this, 600)),
            MinimumSize = new Size(StudentLayout.Scale(this, 320), StudentLayout.Scale(this, 240)),
            MinimizeBox = false,
            ShowInTaskbar = false,
            BackColor = UiStyle.Surface
        };
        var viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var preview = new PictureBox { Image = image, Size = image.Size, SizeMode = PictureBoxSizeMode.Zoom };
        viewport.Controls.Add(preview);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        var close = StudentLayout.Button("&Đóng", (_, _) => { }, true);
        close.DialogResult = DialogResult.Cancel;
        actions.Controls.Add(close);
        dialog.Controls.Add(viewport);
        dialog.Controls.Add(actions);
        dialog.CancelButton = close;
        dialog.Load += (_, _) =>
        {
            UiStyle.ClampToWorkingArea(dialog);
            // Phóng ảnh nhỏ vừa vùng xem, giữ ảnh lớn ở kích thước gốc để cuộn.
            double scale = Math.Max(1d, Math.Min(
                (double)viewport.ClientSize.Width / image.Width, (double)viewport.ClientSize.Height / image.Height));
            preview.Size = new Size((int)(image.Width * scale), (int)(image.Height * scale));
        };
        dialog.ShowDialog(owner);
    }
    internal void GoToQuestion(int index)
    {
        if (IsSaving || index < 0 || index >= _attempt.Answers.Length) return;
        _attempt.Current = index;
        RenderQuestion();
        _questionViewport.AutoScrollPosition = Point.Empty;
        _gridViewport.ScrollControlIntoView(_numbers[index]);
    }

    internal void FocusInitialControl()
    {
        if (_next.CanSelect) _next.Select();
        else _numbers[_attempt.Current].Select();
    }

    private void Choose(int index)
    {
        if (_attempt.Submitted || IsSaving || !_options[index].Enabled ||
            _attempt.Answers[_attempt.Current] == index) return;

        ChangeChoice(index, _options[index]);
    }

    private void HandleOptionKey(int index, KeyEventArgs e)
    {
        int offset = e.KeyData switch
        {
            Keys.Left or Keys.Up => -1,
            Keys.Right or Keys.Down => 1,
            Keys.Space => 0,
            _ => int.MinValue
        };
        if (offset == int.MinValue) return;

        e.SuppressKeyPress = true;
        if (_attempt.Submitted || IsSaving) return;

        int choice = (index + offset + _options.Length) % _options.Length;
        _options[choice].Select();
        Choose(choice);
    }

    private static void KeepArrowKeys(object? sender, PreviewKeyDownEventArgs e)
    {
        // Để KeyDown xử lý mũi tên trong nhóm thay vì điều hướng mặc định.
        if (e.KeyData is Keys.Left or Keys.Right or Keys.Up or Keys.Down) e.IsInputKey = true;
    }

    private void HandleQuestionKey(int index, KeyEventArgs e)
    {
        int target = e.KeyData switch
        {
            Keys.Left => index - 1,
            Keys.Right => index + 1,
            Keys.Up => index - QuestionGridColumns,
            Keys.Down => index + QuestionGridColumns,
            _ => -1
        };
        if (target < 0 || target >= _numbers.Length) return;

        e.SuppressKeyPress = true;
        if (IsSaving) return;

        GoToQuestion(target);
        _numbers[target].Select();
    }
    internal bool IsSaving { get; private set; }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Phím tắt chỉ thao tác bài đang mở, không kích hoạt nút nộp đang có focus.
        if (keyData is Keys.Escape or Keys.Enter || keyData == (Keys.Alt | Keys.N))
            return true;
        int choice = keyData switch
        {
            Keys.A or Keys.D1 or Keys.NumPad1 => 0,
            Keys.B or Keys.D2 or Keys.NumPad2 => 1,
            Keys.C or Keys.D3 or Keys.NumPad3 => 2,
            Keys.D or Keys.D4 or Keys.NumPad4 => 3,
            _ => -1
        };
        int direction = keyData switch
        {
            Keys.N or Keys.PageDown => 1,
            Keys.P or Keys.PageUp => -1,
            _ => 0
        };
        if (choice < 0 && direction == 0)
        {
            if (keyData == Keys.Space && _submit.Focused) return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }
        if (IsSaving) return true;
        if (choice >= 0)
        {
            if (!_attempt.Submitted)
            {
                _options[choice].Select();
                Choose(choice);
            }
        }
        else GoToQuestion(_attempt.Current + direction);
        return true;
    }
    internal bool IsDeadlineReached => _saveClock?.IsExpired ?? _attempt.IsExpired;

    private void ChangeChoice(int? choice, Control source) => SaveChange(() => _attempt.Choose(choice), source);
    private void ChangeFlag()
    {
        bool flagged = _flag.Checked;
        SaveChange(() => _attempt.Flag(flagged), _flag);
    }

    private async void SaveChange(Action change, Control source)
    {
        if (_rendering || _attempt.Submitted || IsSaving || _attempt.Draft?.PendingSubmission == true) return;
        var remaining = _attempt.TimeRemaining;
        if (remaining <= TimeSpan.Zero)
        {
            SaveExpired?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Luồng lưu dùng đồng hồ lượt thi. UI giữ mốc riêng theo cùng hạn để tránh đọc đồng thời.
        var deadline = _attempt.Draft?.DeadlineUtc ?? DateTimeOffset.UtcNow + remaining;
        _saveClock = new StudentAttemptClock(deadline - remaining, deadline);
        IsSaving = true;
        foreach (var option in _options) option.Enabled = false;
        _flag.Enabled = _clearChoice.Enabled = _previous.Enabled = _next.Enabled = false;
        _saveStatus.Text = "Đang lưu tiến độ…";
        _saveStatus.BackColor = UiStyle.Background;
        _saveStatus.ForeColor = UiStyle.Primary;
        _saveStatus.Visible = true;
        FitContent();
        Action afterSave = RenderQuestion;

        try
        {
            await Task.Run(change);
            MarkSaved();
        }
        catch (ExamDesk.Models.StudentDraftExpiredException)
        {
            afterSave = () => SaveExpired?.Invoke(this, EventArgs.Empty);
        }
        catch (ExamDesk.Models.StudentDraftConflictException)
        {
            afterSave = ReloadAfterConflict;
        }
        catch (Exception ex)
        {
            _saveError = StudentError.Describe(ex,
                "Chưa lưu được thay đổi. Hãy chọn lại hoặc báo giảng viên nếu lỗi tiếp diễn.");
        }
        finally
        {
            _saveClock = null;
            IsSaving = false;
            if (!IsDisposed)
            {
                SaveCompleted?.Invoke(this, EventArgs.Empty);
                if (!IsDisposed) afterSave();
                if (!IsDisposed)
                {
                    var focusTarget = source.CanFocus ? source : _options[_attempt.Answers[_attempt.Current] ?? 0];
                    if (focusTarget.CanFocus) focusTarget.Focus();
                }
            }
        }
    }

    internal void ShowDeadlinePending()
    {
        _saveStatus.Text = "Đã hết giờ. Sẽ tự nộp bài khi lưu xong.";
        _saveStatus.BackColor = UiStyle.WarningBack;
        _saveStatus.ForeColor = UiStyle.WarningText;
        _saveStatus.Visible = true;
    }

    private void ReloadAfterConflict()
    {
        _savedAt = null;
        _saveNotice = null;
        _saveError = "Chưa tải lại được tiến độ. Hãy đóng và mở lại lượt thi.";
        try
        {
            // Chỉ hiển thị dữ liệu mới nhất, không ghi lại lựa chọn vừa bị từ chối.
            if (_attempt.ReloadDraft())
            {
                _saveError = null;
                _saveNotice = "Lượt thi đã thay đổi ở cửa sổ khác. Đã tải lại tiến độ mới nhất.";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.ToString());
        }
        if (_saveError is null && _attempt.Draft is { } draft)
        {
            if (draft.PendingSubmission)
            {
                DraftLocked?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (_attempt.IsExpired)
            {
                SaveExpired?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (_numbers.Length != _attempt.Answers.Length) BuildQuestionGrid();
        }
        RenderQuestion();
    }

    private void MarkSaved()
    {
        _saveError = null;
        _saveNotice = null;
        if (_attempt.Draft is not null) _savedAt = DateTimeOffset.UtcNow.LocalDateTime;
    }
    private void UpdateClock()
    {
        if (_attempt.Submitted)
        {
            _clock.Text = "Đã nộp bài · Chỉ xem";
            return;
        }

        var remaining = _saveClock?.Remaining ?? _attempt.TimeRemaining;
        bool lowTime = remaining <= TimeSpan.FromMinutes(5);
        var seconds = Math.Max(0, (long)Math.Ceiling(remaining.TotalSeconds));
        _clock.Text = seconds == 0 ? "Đã hết giờ · 00:00"
            : $"{(lowTime ? "Sắp hết giờ" : "Còn lại")} · {seconds / 60:00}:{seconds % 60:00}";
        _clock.ForeColor = lowTime ? UiStyle.WarningText : UiStyle.Primary;
        _clock.BackColor = lowTime ? UiStyle.WarningBack : UiStyle.Background;
    }

    private void Tick(object? sender, EventArgs e)
    {
        UpdateClock();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _timer.Dispose(); _image.Image?.Dispose(); }
        base.Dispose(disposing);
    }
}
