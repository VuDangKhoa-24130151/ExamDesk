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
    private bool _sizingList;
    private readonly HashSet<Control> _collapsedRows = new();
    private bool? _submittedExpanded;
    private BufferedStudentTable _headerContent = null!;
    private BufferedStudentPanel _listHost = null!;
    private BufferedStudentTable _header = null!;
    private BufferedStudentPanel _viewport = null!;
    private BufferedStudentPanel _list = null!;
    private Label _summary = null!;
    private ProgressBar _loading = null!;
    private Label _status = null!;
    private BufferedStudentTable _notice = null!;
    private Button _reload = null!;
    private Button _retry = null!;

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
        if (_shellBuilt) return;
        // S() đã dùng DPI thật; đặt mốc hiện tại để lần autoscale đầu không nhân lần nữa.
        AutoScaleDimensions = CurrentAutoScaleDimensions;
        _shellBuilt = true;
        BuildShell();
#if DEBUG
        if (_student is null) BuildPreviewPage();
#endif
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_student is not null) _ = Reload();
    }

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
        _headerContent = Table();
        AddRow(_header, _headerContent);
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
        AddRow(_headerContent, toolbar);
        var title = TextLabel("Chọn ca thi", StudentLayout.PageTitleFontSize, FontStyle.Bold);
        title.Margin = new Padding(0, S(StudentLayout.Space12), 0, S(StudentLayout.Space8));
        AddRow(_headerContent, title);
        AddRow(_headerContent, TextLabel("Chọn ca đang mở để bắt đầu làm bài."));
        _summary = TextLabel($"{ClassCaption} · Đang chuẩn bị danh sách");
        AddRow(_headerContent, _summary);

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
        AddRow(_headerContent, _notice);
        var help = TextLabel("Cần hỗ trợ? Hãy hỏi giám thị coi thi.");
        help.ForeColor = UiStyle.TextMuted;
        AddRow(_headerContent, help);

        _viewport = new BufferedStudentPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            TabIndex = 1,
            TabStop = false,
            Margin = Padding.Empty
        };
        _listHost = new BufferedStudentPanel
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            Margin = Padding.Empty,
            TabStop = false
        };
        _list = new BufferedStudentPanel
        {
            AutoSize = false, TabStop = false, Margin = Padding.Empty
        };
        _listHost.Controls.Add(_list);
        _viewport.Controls.Add(_listHost);
        Controls.Add(_viewport);
        Controls.Add(_header);
        Resize += (_, _) => CenterContent();
        DpiChangedAfterParent += (_, _) => CenterContent();
        _viewport.ClientSizeChanged += (_, _) => CenterContent();
        _headerContent.SizeChanged += (_, _) =>
        {
            WrapLabels(_headerContent);
            WrapLabels(_notice);
        };
        Load += (_, _) => SelectNextControl(null, true, true, true, false);
        CenterContent();
        ResumeLayout(true);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (!Visible || !_shellBuilt || _list is null) return;
        CenterContent();
        FitListHost();
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
        WrapLabels(_notice);
        AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
    }

    private void ClearRows()
    {
        while (_list.Controls.Count > 0) _list.Controls[0].Dispose();
        _collapsedRows.Clear();
        _viewport.AutoScrollPosition = Point.Empty;
    }

    private void RenderSessions(IReadOnlyList<SessionEntry> entries)
    {
        _list.SuspendLayout();
        try
        {
            ClearRows();
            var todo = entries.Where(e => e.CanEnter && !e.Submitted).ToList();
            var submitted = entries.Where(e => e.Submitted).ToList();
            var unavailable = entries.Where(e => !e.CanEnter && !e.Submitted).ToList();
            _summary.Text = ClassCaption;
            _notice.Visible = false;
            AddGroup("CẦN LÀM", todo);
            AddGroup("ĐÃ NỘP BÀI", submitted, canCollapse: true);
            AddGroup("CHƯA THỂ VÀO", unavailable);
            if (entries.Count == 0)
                ShowNotice("Chưa có ca thi trong danh sách…\nHãy hỏi giám thị coi thi để được hỗ trợ.", false, true);
        }
        finally
        {
            _list.ResumeLayout(true);
            FitListHost();
        }
    }

    private void AddGroup(string title, List<SessionEntry> entries, bool canCollapse = false)
    {
        if (entries.Count == 0) return;
        var heading = Table(2);
        heading.Dock = DockStyle.None;
        heading.ColumnStyles.Clear();
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        heading.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var label = TextLabel($"{title} ({entries.Count})", StudentLayout.SmallFontSize, FontStyle.Bold);
        label.ForeColor = UiStyle.TextMuted;
        label.Anchor = AnchorStyles.Left;
        heading.Controls.Add(label, 0, 0);
        heading.Padding = new Padding(0, S(StudentLayout.Space12), 0, S(StudentLayout.Space8));
        heading.TabIndex = _list.Controls.Count;
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
                foreach (var card in cards)
                {
                    if (expanded) _collapsedRows.Remove(card);
                    else _collapsedRows.Add(card);
                    card.Visible = expanded;
                }
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
                FitListHost();
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

    private void CenterContent()
    {
        if (!_shellBuilt || _viewport is null) return;
        // Chỉ container nội dung có lề; viewport luôn chạm mép phải.
        int width = _viewport.ClientSize.Width;
        int side = Math.Max(0, (width - S(StudentLayout.SessionPageMaxWidth)) / 2)
            + S(StudentLayout.Space24);
        int scrollbar = Math.Max(0, ClientSize.Width - width);
        var headerPadding = new Padding(side, S(StudentLayout.Space16),
            side + scrollbar, S(StudentLayout.Space8));
        var listPadding = new Padding(side, 0, side, S(StudentLayout.Space16));
        if (_header.Padding != headerPadding) _header.Padding = headerPadding;
        if (_listHost.Padding != listPadding) _listHost.Padding = listPadding;
        FitListHost();
    }

    private void FitListHost()
    {
        if (_sizingList || _list is null || _listHost is null || IsDisposed || Disposing) return;
        _sizingList = true;
        _list.SuspendLayout();
        try
        {
            // Một danh sách phẳng: không suy ra chiều cao từ chuỗi AutoSize/Dock.Top.
            // Không dùng Visible để đo vì nó trả false khi cửa sổ cha đang bị ẩn.
            for (int pass = 0; pass < 3; pass++)
            {
                int width = Math.Max(S(StudentLayout.Space24),
                    _viewport.ClientSize.Width - _listHost.Padding.Horizontal);
                int y = 0;
                foreach (Control row in _list.Controls)
                {
                    if (_collapsedRows.Contains(row)) continue;
                    int rowWidth = Math.Max(1, width - row.Margin.Horizontal);
                    row.Width = rowWidth;
                    int height = row.GetPreferredSize(new Size(rowWidth, 0)).Height;
                    row.SetBounds(row.Margin.Left, y + row.Margin.Top, rowWidth, height);
                    y += height + row.Margin.Vertical;
                }
                _list.SetBounds(_listHost.Padding.Left, _listHost.Padding.Top, width, y);
                _listHost.Height = y + _listHost.Padding.Vertical;
                if (width == Math.Max(S(StudentLayout.Space24),
                    _viewport.ClientSize.Width - _listHost.Padding.Horizontal)) break;
            }
        }
        finally
        {
            _list.ResumeLayout(false);
            _sizingList = false;
        }
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

    private static void WrapLabels(Control panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;
        foreach (var label in panel.Controls.OfType<Label>())
            label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
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
