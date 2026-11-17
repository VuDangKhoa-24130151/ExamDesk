using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public event EventHandler? CloseRequested;
    public event Action<ExamSession, ExamPaper>? SessionSelected;
    internal event Action<StudentResult>? ResultSelected;

    private readonly User? _student;
    private bool _reloading;
    private bool _selecting;
    private bool _shellBuilt;
    private bool _initialLoadDone;
    private bool _layingOut;
    private bool? _submittedExpanded;
    private BufferedStudentTable _header = null!;
    private SessionList _list = null!;
    private Label _summary = null!;
    private ProgressBar _loading = null!;
    private Label _status = null!;
    private BufferedStudentTable _notice = null!;
    private Button _reload = null!;
    private Button _retry = null!;

    private sealed class SessionList : FlowLayoutPanel
    {
        internal SessionList() => DoubleBuffered = true;

        protected override Point ScrollToControl(Control activeControl) =>
            ClientSize.Height <= 0 || !activeControl.Visible || activeControl.Width <= 0 || activeControl.Height <= 0
                ? AutoScrollPosition
                : base.ScrollToControl(activeControl);
    }

    // Chỉ mô tả giao diện; điều kiện vào ca giữ nguyên từ dịch vụ hiện có.
    private sealed record SessionEntry(ExamSession Session, ExamPaper? Paper,
        bool HasProgress, bool Submitted, bool CanEnter)
    {
        internal string State => Submitted ? "Đã nộp bài"
            : !CanEnter ? Paper is null ? "Đã khóa" : "Ca đã đóng"
            : HasProgress ? "Tiếp tục lượt thi" : "Đang mở";
    }

    public SessionSelectionControl()
    {
        InitializeAppearance();
    }

    public SessionSelectionControl(User student) : this()
    {
        _student = student;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
#if DEBUG
        DumpState(nameof(OnHandleCreated));
#endif
        if (_shellBuilt) return;
        // S() đã dùng DPI thật; đặt mốc hiện tại để lần autoscale đầu không nhân lần nữa.
        AutoScaleDimensions = CurrentAutoScaleDimensions;
        BuildShell();
        _shellBuilt = true;
#if DEBUG
        if (_student is null) BuildPreviewPage();
#endif
        UpdateLayout();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
#if DEBUG
        DumpState(nameof(OnLoad));
#endif
        if (_initialLoadDone) return;
        _initialLoadDone = true;
        if (_student is not null) _ = Reload();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateLayout();
    }

#if DEBUG
    private void DumpState(string tag)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[SessionSelectionControl:{tag}] Controls.Count={_list?.Controls.Count}, " +
            $"ClientSize={_list?.ClientSize}, DisplayRectangle={_list?.DisplayRectangle}, " +
            $"AutoScrollMinSize={_list?.AutoScrollMinSize}, AutoScrollPosition={_list?.AutoScrollPosition}, " +
            $"VerticalScroll.Visible={_list?.VerticalScroll.Visible}, DeviceDpi={DeviceDpi}");
        if (_list is not null)
            foreach (Control row in _list.Controls)
                System.Diagnostics.Debug.WriteLine(
                    $"  row={row.TabIndex}:{row.GetType().Name}, Bounds={row.Bounds}, " +
                    $"Visible={row.Visible}, Height={row.Height}");
    }
#endif

    private string ClassCaption
    {
        get
        {
            if (_student is not null) return $"Lớp {_student.ClassName}";
#if DEBUG
            return "Lớp DH23DT";
#else
            return "Chưa chọn sinh viên";
#endif
        }
    }

    private void BuildShell()
    {
        SuspendLayout();
        _header = Table();
        _header.TabIndex = 0;

        var toolbar = Table(2);
        toolbar.ColumnStyles.Clear();
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var back = StudentLayout.CreateButton("&Quay lại nhập MSSV",
            (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty),
            StudentButtonKind.Secondary, StudentArrow.Left);
        back.AccessibleName = "Quay lại nhập MSSV";
        back.TabIndex = 0;
        back.Margin = Padding.Empty;
        _reload = StudentLayout.CreateButton("&Tải lại", async (_, _) => await Reload());
        _reload.AccessibleName = "Tải lại danh sách ca thi (F5 hoặc Ctrl+R)";
        _reload.TabIndex = 1;
        _reload.Margin = new Padding(S(StudentLayout.Space12), 0, S(StudentLayout.Space12), 0);
        toolbar.Controls.Add(back, 0, 0);
        toolbar.Controls.Add(_reload, 1, 0);
        AddRow(_header, toolbar);
        var title = TextLabel("Chọn ca thi", StudentLayout.PageTitleFontSize, FontStyle.Bold);
        title.Margin = new Padding(0, S(StudentLayout.Space12), 0, S(StudentLayout.Space8));
        AddRow(_header, title);
        AddRow(_header, TextLabel("Chọn ca đang mở để bắt đầu làm bài."));
        _summary = TextLabel($"{ClassCaption} · Đang chuẩn bị danh sách");
        AddRow(_header, _summary);

        _notice = Table();
        _notice.Padding = new Padding(S(StudentLayout.Space12));
        _status = TextLabel("");
        _status.AccessibleRole = AccessibleRole.StaticText;
        _retry = StudentLayout.CreateButton("Thử &lại", async (_, _) => await Reload());
        _retry.AccessibleName = "Thử tải lại danh sách ca thi";
        _retry.Margin = Padding.Empty;
        AddRow(_notice, _status);
        _loading = new ProgressBar
        {
            Dock = DockStyle.Top, Height = S(StudentLayout.Space4),
            Style = ProgressBarStyle.Marquee, MarqueeAnimationSpeed = 30,
            Margin = new Padding(0, 0, 0, S(StudentLayout.Space8)), TabStop = false,
            AccessibleName = "Đang tải danh sách ca thi"
        };
        AddRow(_notice, _loading);
        AddRow(_notice, _retry);
        _notice.Visible = false;
        AddRow(_header, _notice);
        var help = TextLabel("Cần hỗ trợ? Hãy hỏi giám thị coi thi.");
        help.ForeColor = UiStyle.TextMuted;
        AddRow(_header, help);

        _list = new SessionList
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            TabIndex = 1,
            TabStop = false,
            Margin = Padding.Empty
        };
        Controls.Add(_list);
        Controls.Add(_header);
        SizeChanged += (_, _) => UpdateLayout();
        _list.SizeChanged += (_, _) => UpdateLayout();
#if DEBUG
        _list.Scroll += (_, _) => DumpState("list.Scroll");
#endif
        Load += (_, _) => SelectNextControl(null, true, true, true, false);
        UpdateLayout();
        ResumeLayout(true);
    }

    private async Task Reload()
    {
        if (_reloading || _selecting || _student is null || !_shellBuilt || IsDisposed || Disposing) return;
        _reloading = true;
        _reload.Enabled = _retry.Enabled = false;
        _reload.Text = "Đang tải…";
        _list.Enabled = false;
        ShowNotice("Đang tải danh sách ca thi…", false, false);
        try
        {
            var entries = await Task.Run(() =>
            {
                var service = new ExamSessionService();
                var papers = service.GetExamPapers();
                var progress = new StudentExamService().GetSessionProgress(_student.StudentCode);
                return service.GetSessions().Select(session =>
                {
                    var paper = papers.FirstOrDefault(p => p.Id == session.PaperId);
                    bool hasProgress = progress.TryGetValue(session.Id, out bool submitted);
                    bool canEnter = submitted || (paper is not null && (session.IsOpen || hasProgress));
                    return new SessionEntry(session, paper, hasProgress, submitted, canEnter);
                }).ToList();
            });
            if (IsDisposed || Disposing) return;
            RenderSessions(entries);
        }
        catch (Exception ex)
        {
            if (IsDisposed || Disposing) return;
            ClearRows();
            _summary.Text = $"Lớp {_student.ClassName} · Chưa tải được danh sách";
            ShowNotice(StudentError.Describe(ex,
                "Không tải được danh sách ca thi. Hãy thử lại hoặc hỏi giám thị coi thi."), true, true);
        }
        finally
        {
            _reloading = false;
            if (!IsDisposed && !Disposing)
            {
                _reload.Text = "&Tải lại";
                _reload.Enabled = _retry.Enabled = true;
                _list.Enabled = true;
            }
        }
    }

    private void ShowNotice(string text, bool error, bool retry)
    {
        _status.Text = text;
        _loading.Visible = _reloading && !error && !retry;
        bool empty = text.StartsWith("Chưa có ca thi", StringComparison.Ordinal);
        _status.AutoSize = !empty;
        _status.Dock = empty ? DockStyle.Top : DockStyle.None;
        _status.TextAlign = empty ? ContentAlignment.MiddleCenter : ContentAlignment.TopLeft;
        if (empty) _status.Height = S(StudentLayout.EmptyNoticeHeight);
        AccessibleDescription = text;
        _notice.BackColor = error ? UiStyle.ErrorBack : UiStyle.Background;
        _status.ForeColor = error ? UiStyle.ErrorText : UiStyle.TextMuted;
        _retry.Visible = retry;
        _notice.Visible = true;
        UpdateLayout();
        AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
    }

    private void ClearRows()
    {
        while (_list.Controls.Count > 0) _list.Controls[0].Dispose();
        _list.AutoScrollPosition = Point.Empty;
    }

    private void RenderSessions(IReadOnlyList<SessionEntry> entries)
    {
        _summary.Text = ClassCaption;
        _notice.Visible = false;
        RenderSessionRows(entries);
        if (entries.Count == 0)
            ShowNotice("Chưa có ca thi trong danh sách…\nHãy hỏi giám thị coi thi để được hỗ trợ.", false, true);
#if DEBUG
        DumpState("RenderSessions.End");
#endif
    }

    private void RenderSessionRows(IReadOnlyList<SessionEntry> entries)
    {
        _list.SuspendLayout();
        try
        {
            ClearRows();
            var todo = entries.Where(e => e.CanEnter && !e.Submitted).ToList();
            var submitted = entries.Where(e => e.Submitted).ToList();
            var unavailable = entries.Where(e => !e.CanEnter && !e.Submitted).ToList();
            AddGroup("CẦN LÀM", todo);
            AddGroup("ĐÃ NỘP BÀI", submitted, canCollapse: true);
            AddGroup("CHƯA THỂ VÀO", unavailable);
        }
        finally
        {
            _list.ResumeLayout(true);
            UpdateLayout();
        }
    }

    private void AddGroup(string title, List<SessionEntry> entries, bool canCollapse = false)
    {
        if (entries.Count == 0) return;
        var heading = Table(2);
        heading.ColumnStyles.Clear();
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var label = TextLabel($"{title} ({entries.Count})", StudentLayout.SmallFontSize, FontStyle.Bold);
        label.ForeColor = UiStyle.TextMuted;
        label.Anchor = AnchorStyles.Left;
        heading.Controls.Add(label, 0, 0);
        heading.Padding = new Padding(0, S(StudentLayout.Space12), 0, S(StudentLayout.Space8));
        heading.TabIndex = _list.Controls.Count;
        heading.Dock = DockStyle.None;
        _list.Controls.Add(heading);
        var cards = new List<Control>();
        foreach (var entry in entries)
        {
            var card = SessionRow(entry);
            card.Dock = DockStyle.None;
            card.TabIndex = _list.Controls.Count;
            cards.Add(card);
            _list.Controls.Add(card);
        }
        if (canCollapse)
        {
            bool expanded = _submittedExpanded ?? entries.Count <= 3;
            void SetExpanded()
            {
                _list.SuspendLayout();
                try
                {
                    foreach (var card in cards) card.Visible = expanded;
                }
                finally { _list.ResumeLayout(true); }
            }
            SetExpanded();
            var toggle = StudentLayout.CreateButton(expanded ? "Thu &gọn" : "&Mở rộng");
            toggle.Margin = Padding.Empty;
            toggle.AccessibleName = $"{(expanded ? "Thu gọn" : "Mở rộng")} {entries.Count} ca đã nộp bài";
            toggle.Click += (_, _) =>
            {
                expanded = !expanded;
                _submittedExpanded = expanded;
                SetExpanded();
                toggle.Text = expanded ? "Thu &gọn" : "&Mở rộng";
                toggle.AccessibleName = $"{(expanded ? "Thu gọn" : "Mở rộng")} {entries.Count} ca đã nộp bài";
                UpdateLayout();
            };
            heading.Controls.Add(toggle, 1, 0);
        }
    }

    private Control SessionRow(SessionEntry entry)
    {
        string reason = !entry.CanEnter
            ? entry.Paper is null
                ? "Đề thi chưa sẵn sàng. Hãy hỏi giám thị coi thi."
                : "Ca đã đóng và bạn chưa bắt đầu lượt thi. Hãy hỏi giám thị coi thi."
            : !entry.Session.IsOpen && !entry.Submitted
                ? "Ca đã đóng; bạn vẫn có thể tiếp tục lượt thi đã lưu." : "";
        var action = StudentLayout.CreateButton(entry.Submitted ? "&Xem kết quả"
            : entry.HasProgress ? "&Tiếp tục lượt thi" : "Xem &hướng dẫn",
            async (sender, _) =>
            {
                if (sender is not Button button || _selecting || _reloading) return;
                string text = button.Text;
                button.Text = "Đang tải…";
                button.Enabled = false;
                button.Refresh();
                try { await SelectSession(entry); }
                finally
                {
                    if (!button.IsDisposed)
                    {
                        button.Text = text;
                        button.Enabled = entry.CanEnter;
                    }
                }
            },
            entry.CanEnter && !entry.Submitted ? StudentButtonKind.Primary : StudentButtonKind.Secondary,
            entry.Submitted || !entry.CanEnter ? StudentArrow.None : StudentArrow.Right);
        action.Enabled = entry.CanEnter;
        action.AccessibleName = $"{action.Text.Replace("&", "")}: {entry.Session.SessionName}";
        action.AccessibleDescription = !entry.CanEnter ? "Chưa thể vào ca. Hãy hỏi giám thị coi thi." : entry.State;
        return new SessionCard(string.IsNullOrWhiteSpace(entry.Paper?.Title)
                ? entry.Session.SessionName : entry.Paper.Title, PaperText(entry.Paper),
            $"Mã ca {entry.Session.SessionCode} · Phòng {entry.Session.RoomName}",
            entry.State, reason, entry.Submitted, action)
        {
            AccessibleName = $"{entry.Session.SessionName} · {entry.State}"
        };
    }

    private async Task SelectSession(SessionEntry entry)
    {
        if (_student is null || !entry.CanEnter || _reloading || _selecting || IsDisposed || Disposing) return;
        _selecting = true;
        _list.Enabled = _reload.Enabled = _retry.Enabled = false;
        try
        {
            // Đọc trạng thái hiện tại, không quyết định bằng danh sách đã tải trước đó.
            var selection = await Task.Run(() =>
            {
                var service = new StudentExamService();
                var result = service.GetResult(entry.Session.Id, _student.StudentCode);
                if (result is not null)
                    return (Result: result, Session: (ExamSession?)null, Paper: (ExamPaper?)null);
                var current = service.GetSelectedSession(entry.Session.Id, _student.StudentCode);
                return (Result: (StudentResult?)null, Session: (ExamSession?)current.Session, Paper: (ExamPaper?)current.Paper);
            });
            if (IsDisposed || Disposing) return;
            if (selection.Result is not null)
                ResultSelected?.Invoke(selection.Result);
            else
                SessionSelected?.Invoke(selection.Session!, selection.Paper!);
        }
        catch (Exception ex)
        {
            if (IsDisposed || Disposing) return;
            ShowNotice(StudentError.Describe(ex, entry.Submitted
                ? "Không tải được kết quả đã lưu. Hãy thử lại hoặc hỏi giám thị coi thi."
                : "Không mở được ca thi. Hãy tải lại hoặc hỏi giám thị coi thi."), true, true);
        }
        finally
        {
            _selecting = false;
            if (!IsDisposed && !Disposing)
                _list.Enabled = _reload.Enabled = _retry.Enabled = true;
        }
    }

    private static string PaperText(ExamPaper? paper)
    {
        if (paper is null) return "Đề thi chưa sẵn sàng";
        return $"{paper.TotalQuestions} câu · {paper.DurationMinutes} phút";
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData is Keys.F5 or (Keys.Control | Keys.R))
        {
            _ = Reload();
            return true;
        }
        if (keyData is Keys.Escape or (Keys.Alt | Keys.Left))
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private int S(int value) => StudentLayout.Scale(this, value);

    private void UpdateLayout()
    {
        if (_layingOut || !_shellBuilt || _list is null || _header is null || IsDisposed || Disposing) return;
        // Không tính lại bố cục/wrap chữ khi cửa sổ đang thu nhỏ về 0x0.
        if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

        _layingOut = true;
        try
        {
            // Giữ bề rộng cố định theo phần chừa thanh cuộn để tránh nhảy chiều ngang.
            int available = Math.Max(1, ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            int side = Math.Max(0, (available - S(StudentLayout.SessionPageMaxWidth)) / 2)
                + S(StudentLayout.Space24);
            int contentWidth = Math.Max(1, available - side * 2);
            int wrapWidth = Math.Max(S(StudentLayout.SessionTitleMinWidth), contentWidth);

            var headerPadding = new Padding(side, S(StudentLayout.Space16),
                Math.Max(0, ClientSize.Width - available) + side, S(StudentLayout.Space8));
            bool headerChanged = false;
            _header.SuspendLayout();
            try
            {
                if (_header.Padding != headerPadding)
                {
                    _header.Padding = headerPadding;
                    headerChanged = true;
                }
                if (WrapLabels(_header, wrapWidth)) headerChanged = true;
                if (_notice.Visible)
                {
                    int noticeWidth = Math.Max(S(StudentLayout.SessionTitleMinWidth),
                        contentWidth - _notice.Padding.Horizontal);
                    if (WrapLabels(_notice, noticeWidth)) headerChanged = true;
                }
            }
            finally { _header.ResumeLayout(headerChanged); }
            if (headerChanged)
            {
                _header.PerformLayout();
                PerformLayout();
            }

            var padding = new Padding(side, 0, side, S(StudentLayout.Space16));
            _list.SuspendLayout();
            try
            {
                if (_list.Padding != padding) _list.Padding = padding;
                foreach (Control item in _list.Controls)
                {
                    int width = Math.Max(1, contentWidth - item.Margin.Horizontal);
                    // Group headings keep natural height with a fixed wrapping width.
                    if (item is not SessionCard)
                    {
                        var constraint = new Size(width, 0);
                        if (item.MinimumSize != constraint) item.MinimumSize = constraint;
                        if (item.MaximumSize != constraint) item.MaximumSize = constraint;
                    }
                    if (item.Width != width) item.Width = width;
                }
            }
            finally { _list.ResumeLayout(true); }
            // Flow layout has now placed the rows; update the scroll extent from those bounds.
            _list.PerformLayout();
#if DEBUG
            DumpState("UpdateLayout.End");
#endif
        }
        finally { _layingOut = false; }
    }

    private static BufferedStudentTable Table(int columns = 1)
    {
        var table = new BufferedStudentTable
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = columns,
            RowCount = 0,
            Margin = Padding.Empty,
            TabStop = false
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return table;
    }

    private static void AddRow(TableLayoutPanel table, Control control)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        control.TabIndex = row;
        table.Controls.Add(control, 0, row);
    }

    private Label TextLabel(string text, float size = StudentLayout.BodyFontSize,
        FontStyle style = FontStyle.Regular) => new()
        {
            Text = text,
            AutoSize = true,
            UseMnemonic = false,
            Font = StudentLayout.GetLabelFont(size, style),
            ForeColor = UiStyle.Text,
            Margin = new Padding(0, 0, 0, S(StudentLayout.Space8))
        };

    private static bool WrapLabels(Control panel, int maxWidth)
    {
        if (maxWidth <= 0) return false;
        bool changed = false;
        foreach (var label in panel.Controls.OfType<Label>())
        {
            var target = label.AutoSize
                ? new Size(Math.Max(1, maxWidth - label.Margin.Horizontal), 0)
                : Size.Empty;
            if (label.MaximumSize != target)
            {
                label.MaximumSize = target;
                changed = true;
            }
        }
        return changed;
    }

#if DEBUG
    private void BuildPreviewPage()
    {
        var paper = new ExamPaper { Title = "C# và SQLite · 30 câu", EasyCount = 30, DurationMinutes = 45 };
        var entries = new List<SessionEntry>
        {
            new(new ExamSession { SessionName = "Lập trình .NET – Ca sáng", SessionCode = "NET01",
                RoomName = "P101", IsOpen = true }, paper, false, false, true),
            new(new ExamSession { SessionName = "Cơ sở dữ liệu – Ca chiều", SessionCode = "SQL02",
                RoomName = "P102" }, paper, true, false, true)
        };
        for (int i = 1; i <= 8; i++)
            entries.Add(new(new ExamSession { SessionName = $"Kiểm tra thực hành – Ca {i}",
                SessionCode = $"TH{i:00}", RoomName = "P201" }, paper, true, true, true));
        entries.Add(new(new ExamSession { SessionName = "Ca thi đã kết thúc", SessionCode = "OLD",
            RoomName = "P301" }, paper, false, false, false));
        entries.Add(new(new ExamSession { SessionName = "Ca chờ chuẩn bị đề", SessionCode = "WAIT",
            RoomName = "P302", IsOpen = true }, null, false, false, false));
        RenderSessions(entries);
        _reload.Enabled = false;
    }
#endif
}
