using System.Drawing.Drawing2D;
using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class StudentExamControl : UserControl
{
    // Trạng thái hiển thị dùng chung cho ô số câu hỏi và thanh chú giải
    private enum QuestionTileState
    {
        Answered,
        Flagged,
        Current,
        Unanswered
    }

    private int _questionGridColumns = StudentLayout.GridColumnsWide;
    private TableLayoutPanel _questionGrid = null!;
    private bool _fittingGrid;
    private const string PreviousText = "&Trước";
    private const string NextText = "T&iếp";
    private const string ReviewSubmitText = "Xem lại và &nộp";
    private const string ClearChoiceText = "&Bỏ chọn";
    private const string FlagText = "Đánh dấu &xem lại";
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private QuestionTile[] _numbers = [];
    private bool _fittingContent;
    private readonly StudentAttemptState _attempt = null!;
    private int _imageQuestion = -1;
    private bool _imageLoadFailed;
    private DateTime? _savedAt;
    private string? _saveError;
    private string? _saveNotice;
    private readonly ToolTip _headerTips = new();
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
        ApplyExamLayout();
        _title.Text = _attempt.Title;
        _identity.Text = _attempt.Draft is { } draft
            ? $"{draft.StudentName} · {draft.StudentCode}"
            : "Chưa có thông tin sinh viên";
        _headerTips.SetToolTip(_title, _title.Text);
        _headerTips.SetToolTip(_identity, _identity.Text);

        _saveStatus.Click += (_, _) => ShowSaveDetails();
        _saveStatus.KeyDown += (_, e) =>
        {
            if (e.KeyCode is not (Keys.Enter or Keys.Space)) return;
            e.SuppressKeyPress = true;
            ShowSaveDetails();
        };

        if (_attempt.Submitted)
        {
            _submit.Text = "Về trạng thái nộp bài";
            _submit.Arrow = StudentArrow.Left;
            _submit.AccessibleName = "Về trạng thái nộp bài";
            _submit.Click += (_, _) => ExitReviewRequested?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _submit.Click += (_, _) => SubmitRequested?.Invoke(this, EventArgs.Empty);
        }

        _image.Click += (_, _) => ShowQuestionImage();
        _viewImage.Click += (_, _) => ShowQuestionImage();
        _previous.Click += (_, _) => GoToQuestion(_attempt.Current - 1);
        _next.Click += (_, _) =>
        {
            if (IsSaving) return;
            if (!_attempt.Submitted && _attempt.Current >= _attempt.Answers.Length - 1)
                SubmitRequested?.Invoke(this, EventArgs.Empty);
            else
                GoToQuestion(_attempt.Current + 1);
        };
        _flag.Click += (_, _) => ChangeFlag();
        _clearChoice.Click += (_, _) => ChangeChoice(null);

        for (int i = 0; i < _options.Length; i++)
        {
            int choice = i;
            var option = _options[i];
            option.Click += (_, _) => Choose(choice);
            option.PreviewKeyDown += KeepArrowKeys;
            option.KeyDown += (_, e) => HandleOptionKey(choice, e);
        }

        BuildQuestionGrid();

        _layout.SizeChanged += (_, _) => FitColumns();
        _questionViewport.SizeChanged += (_, _) => FitContent();
        _right.SizeChanged += (_, _) => FitContent();
        _gridViewport.SizeChanged += (_, _) => FitQuestionGrid();
        Load += (_, _) => FitColumns();
        DpiChangedAfterParent += (_, _) => { ApplyExamLayout(); FitColumns(); };
        if (!_attempt.Submitted)
        {
            _timer.Tick += Tick;
            _timer.Start();
        }
    }

    private void ShowKeyboardHelp()
    {
        using var popup = new Form
        {
            Text = "Hướng dẫn làm bài", StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(StudentLayout.Scale(this, StudentLayout.HelpDialogWidth),
                StudentLayout.Scale(this, StudentLayout.HelpDialogHeight)),
            Font = UiStyle.BodyFont, BackColor = UiStyle.Surface,
            MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false,
            Padding = new Padding(StudentLayout.Scale(this, StudentLayout.Space24))
        };
        var instructions = new Label
        {
            Dock = DockStyle.Fill, AutoSize = false, UseMnemonic = false,
            Text = "A–D / 1–4: chọn đáp án\nN / P hoặc PageDown / PageUp: câu tiếp / trước\nTab / Shift+Tab: chuyển vùng thao tác\nMũi tên: đổi đáp án hoặc di chuyển trong lưới\nEnter trong lưới: mở câu đang có focus\nSpace: chọn đáp án đang có focus\nBỏ chọn: xóa lựa chọn; đánh dấu xem lại độc lập\nEsc: đóng trợ giúp, không nộp bài\n\nNộp bài luôn mở bước kiểm tra và xác nhận.\nĐồng hồ vẫn chạy khi mở trợ giúp."
        };
        var close = StudentLayout.Button("Đóng", (_, _) => popup.Close());
        close.Dock = DockStyle.Bottom;
        close.DialogResult = DialogResult.Cancel;
        popup.Controls.Add(instructions);
        popup.Controls.Add(close);
        popup.CancelButton = close;
        popup.Shown += (_, _) => { UiStyle.ClampToWorkingArea(popup); close.Focus(); };
        popup.ShowDialog(FindForm());
    }

    private void ShowSaveDetails()
    {
        if (_saveStatus.AccessibleDescription is { Length: > 0 } details)
            MessageBox.Show(FindForm(), details, "Trạng thái lưu tiến độ", MessageBoxButtons.OK,
                _saveError is null ? MessageBoxIcon.Information : MessageBoxIcon.Error);
    }

    private void SetSaveStatus(string text, Color color, string details)
    {
        _saveStatus.Text = text;
        _saveStatus.ForeColor = color;
        _saveStatus.AccessibleDescription = details;
        _headerTips.SetToolTip(_saveStatus, details);
        _saveStatus.Invalidate();
    }

    private void UpdateSaveStatus()
    {
        if (_saveError is not null) SetSaveStatus("Lỗi lưu", UiStyle.ErrorText, _saveError);
        else if (_saveNotice is not null) SetSaveStatus("Đã tải lại", UiStyle.TextMuted, _saveNotice);
        else if (_savedAt is DateTime savedAt)
            SetSaveStatus("Đã lưu", UiStyle.SuccessText, $"Đã lưu tiến độ lúc {savedAt:HH:mm:ss}.");
        else SetSaveStatus("Sẵn sàng", UiStyle.TextMuted, _attempt.Draft is null
            ? "Lượt demo không lưu tiến độ vào cơ sở dữ liệu." : "Tiến độ tự lưu sau mỗi thay đổi.");
    }

    // Giới hạn tùy biến trong màn thi; co giãn theo DPI và tỷ lệ màn hình.
    private void ApplyExamLayout()
    {
        int S(int value) => StudentLayout.Scale(this, value);
        _layout.Margin = new Padding(S(StudentLayout.Space24));
        if (_questionViewport.Parent is TableLayoutPanel content)
        {
            content.RowStyles[2].Height = S(StudentLayout.BarHeight);
            if (content.Parent is Control card) card.Padding = new Padding(S(StudentLayout.Stroke1));
        }
        _deadlineBanner.Margin = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space16), S(StudentLayout.Space24), 0);
        _questionViewport.Margin = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space24), S(StudentLayout.Space24), S(StudentLayout.Space16));
        _actionBar.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Stroke1), S(StudentLayout.Space24), 0);
        if (_right.Parent is Control sidebar) sidebar.Padding = new Padding(S(StudentLayout.Space16));
        if (_layout.GetControlFromPosition(0, 0) is Control left) left.Margin = new Padding(0, 0, S(StudentLayout.Space24), 0);
        _examShell.RowStyles[0].Height = S(StudentLayout.BarHeight);
        _examHeader.Padding = new Padding(S(StudentLayout.Space24), S(StudentLayout.Space8), S(StudentLayout.Space24), S(StudentLayout.Space8));
        _examHeader.ColumnStyles[1].Width = S(StudentLayout.ClockWidth);
        _title.Margin = new Padding(0, 0, S(StudentLayout.Space16), 0);
        _identity.Margin = new Padding(S(StudentLayout.Space16), 0, S(StudentLayout.Space16), 0);
        _questionHeading.Height = S(StudentLayout.HeadingHeight);
        _questionHeading.Margin = new Padding(0, 0, 0, S(StudentLayout.Space12));
        _question.Margin = new Padding(0, S(StudentLayout.Space4), 0, S(StudentLayout.Space24));
        _clock.Margin = new Padding(S(StudentLayout.Space8), 0, S(StudentLayout.Space8), 0);
        _progress.Height = S(StudentLayout.HeadingHeight);
        _progress.Margin = new Padding(0, 0, 0, S(StudentLayout.Space12));
        _questionLegend.Margin = new Padding(0, S(StudentLayout.Space12), 0, 0);
        foreach (LegendItem item in _questionLegend.Controls) item.FitSize();
        _clearChoice.Margin = new Padding(0, 0, S(StudentLayout.Space8), 0);
        foreach (var button in new[] { _previous, _clearChoice, _flag, _next, _submit })
            button.FitSize();
        foreach (var option in _options) option.Margin = new Padding(0, 0, 0, S(StudentLayout.Space8));
    }

    private void FitContent()
    {
        if (_fittingContent) return;
        _fittingContent = true;
        try
        {
            if (_deadlineBanner.Parent is { } statusContainer)
            {
                _deadlineBanner.MaximumSize = new Size(
                    Math.Max(1, statusContainer.DisplayRectangle.Width - _deadlineBanner.Margin.Horizontal), 0);
            }
            // Chừa chỗ cho thanh cuộn để độ rộng đáp án không đổi khi nội dung dài.
            int width = Math.Max(1, _questionViewport.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            _body.SuspendLayout();
            try
            {
                var textLimit = new Size(width, 0);
                _question.MaximumSize = _questionNumber.MaximumSize = textLimit;
                _questionHeading.Width = width;
                _question.Size = new Size(width, ((QuestionText)_question).MeasureHeight(width));
                foreach (var option in _options)
                    option.Size = new Size(width, option.MeasureHeight(width));
                _image.Width = Math.Min(width, StudentLayout.Scale(this, StudentLayout.ImageMaxWidth));
                _image.Height = Math.Min(StudentLayout.Scale(this, StudentLayout.ImageMaxHeight),
                    _image.Width * StudentLayout.ImageMaxHeight / StudentLayout.ImageMaxWidth);
            }
            finally { _body.ResumeLayout(true); }
            FitQuestionGrid();
            int sideWidth = Math.Max(1, _right.ClientSize.Width);
            _progress.MaximumSize = _questionLegend.MaximumSize = new Size(sideWidth, 0);
        }
        finally { _fittingContent = false; }
    }

    private void FitColumns()
    {
        if (_layout.ClientSize.Width <= 0) return;
        var sideWidth = Math.Min(StudentLayout.Scale(this, StudentLayout.SidebarWidth), (int)(_layout.ClientSize.Width * 0.42f));
        if (_layout.ColumnStyles[1].Width != sideWidth)
            _layout.ColumnStyles[1].Width = sideWidth;
        FitContent();
    }

    private void BuildQuestionGrid()
    {
        while (_gridViewport.Controls.Count > 0) _gridViewport.Controls[0].Dispose();
        _questionGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = false, Margin = Padding.Empty, Padding = Padding.Empty,
            BackColor = UiStyle.Surface, TabIndex = 0, TabStop = false
        };
        _numbers = new QuestionTile[_attempt.Answers.Length];
        for (int i = 0; i < _numbers.Length; i++)
        {
            int index = i;
            var number = new QuestionTile
            {
                Padding = Padding.Empty, AccessibleName = $"Câu {i + 1}", TabIndex = i,
                Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Regular)
            };
            _numbers[i] = number;
            number.Click += (_, _) => GoToQuestion(index);
            number.PreviewKeyDown += KeepArrowKeys;
            number.KeyDown += (_, e) => HandleQuestionKey(index, e);
        }
        _gridViewport.Controls.Add(_questionGrid);
        FitQuestionGrid();
    }

    private void FitQuestionGrid()
    {
        if (_questionGrid is null || _fittingGrid) return;
        _fittingGrid = true;
        try
        {
            int S(int value) => StudentLayout.Scale(this, value);
            int step = S(StudentLayout.TileStep);
            int gap = S(StudentLayout.TileGap);
            int tileSize = S(StudentLayout.TileSize);
            int available = Math.Max(1, _gridViewport.ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            int columns = available >= StudentLayout.GridColumnsWide * step - gap
                ? StudentLayout.GridColumnsWide
                : StudentLayout.GridColumnsNarrow;
            int rows = (_numbers.Length + columns - 1) / columns;
            var focused = _numbers.FirstOrDefault(number => number.Focused);
            _questionGrid.SuspendLayout();
            if (_questionGrid.ColumnCount != columns || _questionGrid.RowCount != rows)
            {
                _questionGrid.Controls.Clear();
                _questionGrid.ColumnStyles.Clear();
                _questionGrid.RowStyles.Clear();
                _questionGrid.ColumnCount = columns;
                _questionGrid.RowCount = rows;
                for (int i = 0; i < columns; i++) _questionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, step));
                for (int i = 0; i < rows; i++) _questionGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, step));
                for (int i = 0; i < _numbers.Length; i++) _questionGrid.Controls.Add(_numbers[i], i % columns, i / columns);
            }
            _questionGridColumns = columns;
            foreach (ColumnStyle column in _questionGrid.ColumnStyles) column.Width = step;
            foreach (RowStyle row in _questionGrid.RowStyles) row.Height = step;
            foreach (var number in _numbers)
            {
                number.Margin = new Padding(0, 0, gap, gap);
                number.Size = new Size(tileSize, tileSize);
            }
            _questionGrid.Height = Math.Max(0, rows * step - gap);
            _questionGrid.ResumeLayout(true);
            if (focused?.CanFocus == true) focused.Focus();
        }
        finally { _fittingGrid = false; }
    }

    private void RenderQuestion()
    {
        _questionNumber.Text = $"Câu {_attempt.Current + 1:00} / {_attempt.Answers.Length}";
        if (_attempt.Draft is { } draft)
        {
            var question = draft.Questions[_attempt.Current];
            _question.Text = question.Content;
            if (_imageQuestion != _attempt.Current)
            {
                _image.Image?.Dispose();
                _image.Image = null;
                _imageLoadFailed = false;
                _imageQuestion = _attempt.Current;
                if (!string.IsNullOrWhiteSpace(question.ImagePath))
                {
                    try
                    {
                        using var original = Image.FromFile(question.ImagePath);
                        _image.Image = new Bitmap(original);
                    }
                    catch (Exception ex) when (ex is IOException or ArgumentException or OutOfMemoryException or UnauthorizedAccessException)
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
            option.Enabled = !_attempt.Submitted && !locked;
            option.TabStop = !_attempt.Submitted && !locked && i == (selected ?? 0);
        }
        bool currentFlagged = _attempt.Flags[_attempt.Current];
        _flag.Toggled = currentFlagged;
        _flag.AccessibleDescription = currentFlagged ? "Đã bật đánh dấu xem lại" : "Chưa đánh dấu xem lại";
        _previous.Enabled = _attempt.Current > 0;
        bool isLastQuestion = _attempt.Current >= _attempt.Answers.Length - 1;
        bool canSubmitOnLast = isLastQuestion && !_attempt.Submitted;
        _next.Kind = canSubmitOnLast ? StudentButtonKind.Primary : StudentButtonKind.Secondary;
        _next.Arrow = canSubmitOnLast ? StudentArrow.None : StudentArrow.Right;
        _next.Text = canSubmitOnLast ? ReviewSubmitText : NextText;
        _next.AccessibleName = canSubmitOnLast ? "Xem lại và nộp" : "Câu tiếp";
        _next.AccessibleDescription = canSubmitOnLast ? "Mở kiểm tra bài trước khi xác nhận nộp." : "";
        _next.Enabled = canSubmitOnLast ? !locked : !isLastQuestion;
        _flag.Enabled = !_attempt.Submitted && !locked;
        bool canClearChoice = !_attempt.Submitted && !locked && selected.HasValue;
        _clearChoice.Visible = canClearChoice;
        _clearChoice.Enabled = canClearChoice;
        _submit.Enabled = !IsSaving;
        _progress.Text = $"Đã trả lời {_attempt.Answered}/{_attempt.Answers.Length} · Đánh dấu {_attempt.Flags.Count(flag => flag)}";
        _headerTips.SetToolTip(_progress, _progress.Text);
        UpdateSaveStatus();
        UpdateClock();
        for (int i = 0; i < _numbers.Length; i++)
        {
            bool answered = _attempt.Answers[i].HasValue;
            bool flagged = _attempt.Flags[i];
            _numbers[i].Text = (i + 1).ToString();
            _numbers[i].TabStop = i == _attempt.Current;
            _numbers[i].AccessibleDescription = $"{(answered ? "Đã trả lời" : "Chưa trả lời")}{(flagged ? ", đánh dấu xem lại" : "")}{(i == _attempt.Current ? ", đang xem" : "")}";
            _numbers[i].Answered = answered;
            _numbers[i].Flagged = flagged;
            _numbers[i].Current = i == _attempt.Current;
            _numbers[i].Invalidate();
        }
        FitContent();
    }

    private void ShowQuestionImage()
    {
        if (_image.Image is not { } image || FindForm() is not { } owner) return;

        using var dialog = new Form
        {
            Text = $"Ảnh câu {_attempt.Current + 1}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(StudentLayout.Scale(this, StudentLayout.ImageDialogWidth),
                StudentLayout.Scale(this, StudentLayout.ImageDialogHeight)),
            MinimumSize = new Size(StudentLayout.Scale(this, StudentLayout.ImageDialogMinWidth),
                StudentLayout.Scale(this, StudentLayout.ImageDialogMinHeight)),
            MinimizeBox = false,
            ShowInTaskbar = false,
            BackColor = UiStyle.Surface
        };
        var viewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var preview = new PictureBox { Image = image, Size = image.Size, SizeMode = PictureBoxSizeMode.Zoom };
        viewport.Controls.Add(preview);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        var close = StudentLayout.Button("&Đóng", primary: true);
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
            Keys.Up => index - _questionGridColumns,
            Keys.Down => index + _questionGridColumns,
            _ => -1
        };
        if (target < 0 || target >= _numbers.Length) return;

        e.SuppressKeyPress = true;
        if (IsSaving) return;

        _numbers[target].Select();
        _gridViewport.ScrollControlIntoView(_numbers[target]);
    }
    internal bool IsSaving { get; private set; }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F1) { ShowKeyboardHelp(); return true; }
        // Không chuyển Esc lên Form để tránh kích hoạt thao tác nộp bài.
        if ((keyData & Keys.KeyCode) == Keys.Escape) return true;
        if (keyData == Keys.Enter && _saveStatus.Focused)
        {
            ShowSaveDetails();
            return true;
        }
        if (keyData == Keys.Enter && _numbers.FirstOrDefault(number => number.Focused) is { } tile)
        {
            tile.Activate();
            return true;
        }

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
            return base.ProcessCmdKey(ref msg, keyData);
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
    internal bool IsDeadlineReached => _attempt.IsExpired;

    private void ChangeChoice(int? choice, Control? source = null) =>
        SaveChange(() => _attempt.Choose(choice), source ?? _clearChoice);
    private void ChangeFlag()
    {
        bool flagged = !_attempt.Flags[_attempt.Current];
        SaveChange(() => _attempt.Flag(flagged), _flag);
    }

    private async void SaveChange(Action change, Control source)
    {
        if (_attempt.Submitted || IsSaving || _attempt.Draft?.PendingSubmission == true) return;
        var remaining = _attempt.TimeRemaining;
        if (remaining <= TimeSpan.Zero)
        {
            SaveExpired?.Invoke(this, EventArgs.Empty);
            return;
        }

        IsSaving = true;
        foreach (var option in _options) option.Enabled = false;
        _flag.Enabled = _clearChoice.Enabled = _previous.Enabled = _next.Enabled = false;
        _submit.Enabled = false;
        SetSaveStatus("Đang lưu…", UiStyle.Primary, "Đang lưu tiến độ. Vui lòng chờ trước khi đổi đáp án.");
        FitContent();
        Action afterSave = RenderQuestion;

        try
        {
            await Task.Run(change);
            MarkSaved();
        }
        catch (StudentDraftExpiredException)
        {
            afterSave = () => SaveExpired?.Invoke(this, EventArgs.Empty);
        }
        catch (StudentDraftConflictException)
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
        SetSaveStatus("Đang lưu…", UiStyle.Primary, "Đã hết giờ. Sẽ tự nộp bài khi lưu xong.");
        _deadlineBanner.Text = "Đã hết giờ. Sẽ tự nộp bài khi lưu xong.";
        _deadlineBanner.Visible = true;
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
            _deadlineBanner.Visible = false;
            _clock.Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Bold);
            _clock.Text = "Đã nộp bài · Chỉ xem";
            return;
        }
        _clock.Font = StudentLayout.GetLabelFont(StudentLayout.ClockFontSize, FontStyle.Bold);

        var remaining = _attempt.TimeRemaining;
        bool lowTime = remaining < TimeSpan.FromMinutes(5);
        bool urgent = remaining < TimeSpan.FromMinutes(1);
        _deadlineBanner.Text = remaining <= TimeSpan.Zero
            ? "Đã hết giờ. Bài sẽ được nộp tự động."
            : urgent ? "Còn dưới 1 phút. Hãy kiểm tra câu trả lời trước khi hết giờ."
            : "Còn tối đa 5 phút. Hãy kiểm tra câu trả lời trước khi hết giờ.";
        _deadlineBanner.BackColor = urgent ? UiStyle.ErrorBack : UiStyle.WarningBack;
        _deadlineBanner.ForeColor = urgent ? UiStyle.ErrorText : UiStyle.WarningText;
        _deadlineBanner.Visible = lowTime;
        var seconds = Math.Max(0, (long)Math.Ceiling(remaining.TotalSeconds));
        _clock.Text = seconds == 0 ? "Hết giờ · 00:00"
            : $"Còn {seconds / 60:00}:{seconds % 60:00}";
        _clock.ForeColor = urgent ? UiStyle.ErrorText : lowTime ? UiStyle.WarningText : UiStyle.Primary;
        _clock.BackColor = urgent ? UiStyle.ErrorBack : lowTime ? UiStyle.WarningBack : UiStyle.Background;
    }

    private void Tick(object? sender, EventArgs e)
    {
        UpdateClock();
    }

    private sealed class QuestionCard : Panel
    {
        internal QuestionCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            BackColor = UiStyle.Background;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            int stroke = StudentLayout.Scale(this, StudentLayout.Stroke1);
            if (Width <= stroke * 2 || Height <= stroke * 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = StudentLayout.RoundedRectangle(this,
                Rectangle.Inflate(ClientRectangle, -stroke, -stroke), StudentLayout.CardRadius);
            using var fill = new SolidBrush(UiStyle.Surface);
            using var border = new Pen(UiStyle.Border, stroke);
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(border, path);
        }
    }

    private sealed class SaveIndicator : Label
    {
        internal SaveIndicator()
        {
            SetStyle(ControlStyles.Selectable | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }

        protected override bool IsInputKey(Keys keyData) =>
            keyData is Keys.Enter or Keys.Space || base.IsInputKey(keyData);

        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
        protected override void OnPaint(PaintEventArgs e)
        {
            int S(int value) => StudentLayout.Scale(this, value);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int dotSize = S(StudentLayout.DotSize);
            int gap = S(StudentLayout.Space8);
            int reserve = dotSize + gap + S(StudentLayout.Space4);
            int textWidth = Math.Min(TextRenderer.MeasureText(Text, Font).Width, Math.Max(0, Width - reserve));
            int textLeft = Width - textWidth;
            StudentLayout.DrawDot(e.Graphics,
                new Rectangle(Math.Max(0, textLeft - dotSize - gap), (Height - dotSize) / 2, dotSize, dotSize),
                ForeColor);
            TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(textLeft, 0, textWidth, Height), ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(e.Graphics,
                    Rectangle.Inflate(ClientRectangle, -S(StudentLayout.Stroke1), -S(StudentLayout.Stroke1)));
        }
    }

    // Label chuẩn không có line-height; đo và vẽ cùng một cách để chữ dài không bị cắt.
    private sealed class QuestionText : Label
    {
        internal QuestionText() => SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw, true);

        private List<string> WrapLines(Graphics graphics, int width)
        {
            var lines = new List<string>();
            using var format = new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags = StringFormatFlags.LineLimit
            };
            foreach (var paragraph in Text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                if (paragraph.Length == 0) { lines.Add(""); continue; }
                int start = 0;
                while (start < paragraph.Length)
                {
                    string remaining = paragraph[start..];
                    graphics.MeasureString(remaining, Font, new SizeF(Math.Max(1, width), Font.GetHeight(graphics) * 1.1f),
                        format, out int count, out _);
                    if (count == 0) count = System.Globalization.StringInfo.GetNextTextElement(remaining).Length;
                    lines.Add(remaining[..count].TrimEnd());
                    start += count;
                }
            }
            return lines;
        }

        internal int MeasureHeight(int width)
        {
            using var graphics = CreateGraphics();
            return (int)Math.Ceiling(WrapLines(graphics, width).Count * Font.GetHeight(graphics) * 1.4f);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var brush = new SolidBrush(ForeColor);
            using var format = new StringFormat(StringFormat.GenericTypographic);
            float lineHeight = Font.GetHeight(e.Graphics) * 1.4f;
            var lines = WrapLines(e.Graphics, Width);
            for (int i = 0; i < lines.Count; i++)
                e.Graphics.DrawString(lines[i], Font, brush, new PointF(0, i * lineHeight), format);
        }
    }

    private sealed class ClockPill : Label
    {
        internal ClockPill() => SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw, true);

        protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(UiStyle.Surface);

        protected override void OnPaint(PaintEventArgs e)
        {
            int inset = StudentLayout.Scale(this, StudentLayout.Stroke1);
            if (Width <= inset * 2 || Height <= inset * 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pill = StudentLayout.RoundedRectangle(this,
                Rectangle.Inflate(ClientRectangle, -inset, -inset), StudentLayout.PillRadius);
            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, pill);
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    private sealed class LegendItem : Panel
    {
        private readonly QuestionTileState _state;
        internal LegendItem(string text, QuestionTileState state)
        {
            Text = text;
            _state = state;
            Font = StudentLayout.GetLabelFont(StudentLayout.SmallFontSize, FontStyle.Regular);
            AccessibleName = text;
            AccessibleRole = AccessibleRole.StaticText;
            DoubleBuffered = true;
            FitSize();
        }

        internal void FitSize()
        {
            int S(int value) => StudentLayout.Scale(this, value);
            Size = new Size(TextRenderer.MeasureText(Text, Font).Width + S(StudentLayout.LegendTextLeft),
                Math.Max(Font.Height, S(StudentLayout.LegendRowHeight)));
            Margin = new Padding(0, 0, S(StudentLayout.Space12), S(StudentLayout.Space8));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int S(int value) => StudentLayout.Scale(this, value);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = new Rectangle(S(StudentLayout.Stroke1), (Height - S(StudentLayout.LegendBoxSize)) / 2,
                S(StudentLayout.LegendBoxSize), S(StudentLayout.LegendBoxSize));
            using var path = StudentLayout.RoundedRectangle(this, bounds, StudentLayout.SmallRadius);

            Color fillCol;
            Color borderCol;
            int stroke;

            switch (_state)
            {
                case QuestionTileState.Answered: // Đã trả lời: nền xanh nhạt, viền xanh vừa 1px
                    fillCol = StudentLayout.AnsweredBack;
                    borderCol = StudentLayout.AnsweredBorder;
                    stroke = StudentLayout.Stroke1;
                    break;
                case QuestionTileState.Flagged: // Đánh dấu: nền vàng, viền WarningBorder 1px, không dấu chấm
                    fillCol = StudentLayout.FlaggedBack;
                    borderCol = StudentLayout.WarningBorder;
                    stroke = StudentLayout.Stroke1;
                    break;
                case QuestionTileState.Current: // Đang xem: nền trắng, viền Primary 2px
                    fillCol = UiStyle.Surface;
                    borderCol = UiStyle.Primary;
                    stroke = StudentLayout.Stroke2;
                    break;
                default: // Chưa trả lời: nền trắng, viền Border 1px
                    fillCol = UiStyle.Surface;
                    borderCol = UiStyle.Border;
                    stroke = StudentLayout.Stroke1;
                    break;
            }

            using var fill = new SolidBrush(fillCol);
            using var border = new Pen(borderCol, S(stroke));
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(border, path);

            TextRenderer.DrawText(e.Graphics, Text, Font,
                new Rectangle(S(StudentLayout.LegendTextLeft), 0, Width - S(StudentLayout.LegendTextLeft), Height),
                UiStyle.TextMuted, TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
    }

    private sealed class QuestionTile : Panel
    {
        internal bool Answered, Flagged, Current;

        internal QuestionTile()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable, true);
            AccessibleRole = AccessibleRole.PushButton;
            TabStop = true;
            BackColor = UiStyle.Surface;
            Cursor = Cursors.Hand;
        }

        internal void Activate() { if (Enabled) OnClick(EventArgs.Empty); }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Focus();
            base.OnMouseDown(e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (Enabled && (e is not MouseEventArgs mouse || mouse.Button == MouseButtons.Left)) base.OnClick(e);
        }
        protected override bool IsInputKey(Keys keyData) =>
            keyData is Keys.Enter or Keys.Space || base.IsInputKey(keyData);
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData is Keys.Enter or Keys.Space) { e.SuppressKeyPress = true; Activate(); }
            base.OnKeyDown(e);
        }
        protected override AccessibleObject CreateAccessibilityInstance() => new TileAccessibleObject(this);
        private sealed class TileAccessibleObject(QuestionTile owner) : ControlAccessibleObject(owner)
        {
            public override string DefaultAction => "Mở câu hỏi";
            public override void DoDefaultAction() { if (owner.Enabled) { owner.Focus(); owner.Activate(); } }
        }

        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            int S(int value) => StudentLayout.Scale(this, value);
            if (Width <= S(StudentLayout.Space4) || Height <= S(StudentLayout.Space4)) return;
            e.Graphics.Clear(UiStyle.Surface);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var outline = StudentLayout.RoundedRectangle(this,
                Rectangle.Inflate(ClientRectangle, -S(StudentLayout.Stroke2), -S(StudentLayout.Stroke2)),
                StudentLayout.ButtonRadius);

            Color backColor;
            if (Flagged)
                backColor = StudentLayout.FlaggedBack;
            else if (Answered)
                backColor = StudentLayout.AnsweredBack;
            else
                backColor = UiStyle.Surface;

            Color borderColor;
            int stroke;
            if (Current)
            {
                borderColor = UiStyle.Primary;
                stroke = StudentLayout.Stroke2;
            }
            else if (Flagged)
            {
                borderColor = StudentLayout.WarningBorder;
                stroke = StudentLayout.Stroke1;
            }
            else if (Answered)
            {
                borderColor = StudentLayout.AnsweredBorder;
                stroke = StudentLayout.Stroke1;
            }
            else
            {
                borderColor = UiStyle.Border;
                stroke = StudentLayout.Stroke1;
            }

            Color textColor;
            FontStyle fontStyle;
            if (Answered)
            {
                textColor = UiStyle.Primary;
                fontStyle = FontStyle.Bold;
            }
            else if (Flagged)
            {
                textColor = UiStyle.WarningText;
                fontStyle = FontStyle.Regular;
            }
            else
            {
                textColor = UiStyle.TextMuted;
                fontStyle = FontStyle.Regular;
            }

            using var fill = new SolidBrush(backColor);
            using var border = new Pen(borderColor, S(stroke));
            e.Graphics.FillPath(fill, outline);
            e.Graphics.DrawPath(border, outline);

            var textBounds = ClientRectangle;
            TextRenderer.DrawText(e.Graphics, Text,
                StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, fontStyle),
                textBounds, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(e.Graphics,
                    Rectangle.Inflate(ClientRectangle, -S(StudentLayout.Space4), -S(StudentLayout.Space4)));
        }
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _timer.Dispose(); _headerTips.Dispose(); _image.Image?.Dispose(); }
        base.Dispose(disposing);
    }
}
