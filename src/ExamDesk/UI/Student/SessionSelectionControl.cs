using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public event EventHandler? CloseRequested;
    public event Action<ExamSession, ExamPaper>? SessionSelected;
    internal event Action<StudentResult>? ResultSelected;

    public SessionSelectionControl()
    {
        InitializeAppearance();
#if DEBUG
        BuildPreviewPage();
#endif
        ConfigureViewport();
    }

    public SessionSelectionControl(User student)
    {
        InitializeAppearance();
        ConfigureViewport();
        BuildSessionPage(student);
    }

#if DEBUG
    private void BuildPreviewPage()
    {
        SuspendLayout();

        // Chia màn hình thành 4 hàng: tiêu đề, thông tin sinh viên, ca thi và nút quay lại.
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 4,
            Padding = new Padding(StudentLayout.Space24), TabIndex = 0, TabStop = false
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // Các hàng tự tăng chiều cao khi chữ xuống dòng; màn hình thiếu chỗ sẽ có thanh cuộn.
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        page.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var heading = new FlowLayoutPanel
        {
            AutoSize = true, Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Margin = new Padding(0, 0, 0, StudentLayout.Space12)
        };
        heading.Controls.Add(TextLabel("Chọn ca thi", StudentLayout.PageTitleFontSize, FontStyle.Bold));
        heading.Controls.Add(TextLabel("Chỉ hiển thị các ca thi có tên bạn trong danh sách.", StudentLayout.BodyFontSize));
        page.Controls.Add(heading, 0, 0);

        var identity = Surface();
        identity.Margin = new Padding(0, 0, 0, StudentLayout.Space12);
        identity.Controls.Add(TextLabel("Sinh viên mẫu · SV0001", StudentLayout.HeadingFontSize, FontStyle.Bold));
        identity.Controls.Add(TextLabel("Lớp DH23DT • Chọn ca thi trong danh sách được phân công", StudentLayout.BodyFontSize));
        page.Controls.Add(identity, 0, 1);

        var sessions = SessionList();
        var enter = StudentLayout.CreateButton("&Xem hướng dẫn", kind: StudentButtonKind.Primary, arrow: StudentArrow.Right);
        enter.TabIndex = 0;
        sessions.Controls.Add(SessionRow("Cuối kỳ .NET – Ca 01", "Lập trình .NET · 30 câu · 45 phút",
            "Ca: CA01\nPhòng: P101",
            "Đang mở", enter));
        var unavailable = StudentLayout.CreateButton("Xem hướng dẫn", kind: StudentButtonKind.Primary, arrow: StudentArrow.Right);
        unavailable.Enabled = false;
        sessions.Controls.Add(SessionRow("Cuối kỳ .NET – Ca 02", "Lập trình .NET · 30 câu · 45 phút",
            "Ca: CA02\nPhòng: P102",
            "Ca đã đóng", unavailable));
        page.Controls.Add(sessions, 0, 2);

        var back = StudentLayout.CreateButton("&Quay lại cửa sổ chính",
            (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty),
            StudentButtonKind.Secondary, StudentArrow.Left);
        back.TabIndex = 1;
        back.Margin = new Padding(0, StudentLayout.Space12, 0, 0);
        page.Controls.Add(back, 0, 3);
        // Cho chữ tự xuống dòng theo chiều rộng từng khung khi phóng to hoặc thu nhỏ.
        heading.SizeChanged += (_, _) => WrapLabels(heading);
        identity.SizeChanged += (_, _) => WrapLabels(identity);
        Controls.Add(page);
        ResumeLayout(false);
    }
#endif

    private void ConfigureViewport()
    {
        Resize += (_, _) => CenterContent();
        CenterContent();
        Load += (_, _) => SelectNextControl(null, true, true, true, false);
    }

    private static TableLayoutPanel SessionRow(string title, string paper, string details, string state,
        Button action)
    {
        var row = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, RowCount = 1,
            BackColor = UiStyle.Surface,
            Padding = new Padding(StudentLayout.Space16, StudentLayout.Space8, StudentLayout.Space16, StudentLayout.Space8),
            Margin = new Padding(0, 0, 0, StudentLayout.Space8)
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, row.ClientRectangle,
            UiStyle.Border, ButtonBorderStyle.Solid);

        var info = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Margin = new Padding(0, 0, StudentLayout.Space12, 0)
        };
        info.Controls.Add(TextLabel(title, StudentLayout.SubheadingFontSize, FontStyle.Bold));
        info.Controls.Add(TextLabel($"Đề: {paper}"));
        var meta = TextLabel(details, StudentLayout.SmallFontSize);
        meta.ForeColor = UiStyle.TextMuted;
        meta.Margin = Padding.Empty;
        info.Controls.Add(meta);
        info.Controls.Add(TextLabel($"Trạng thái: {state}", StudentLayout.SmallFontSize));
        // Tên ca và đề dài xuống dòng trong cột thông tin.
        info.SizeChanged += (_, _) => WrapLabels(info);

        var badge = Badge(state);
        action.AccessibleName = $"{action.Text.Replace("&", "").Trim()}: {title}";
        action.Margin = Padding.Empty;
        row.Controls.Add(info, 0, 0);
        row.Controls.Add(badge, 1, 0);
        row.Controls.Add(action, 2, 0);
        bool? stacked = null;
        void ArrangeRow()
        {
            bool narrow = row.ClientSize.Width < StudentLayout.Scale(row, StudentLayout.SessionRowBreakpoint);
            if (stacked == narrow) return;
            stacked = narrow;
            row.SuspendLayout();
            // Đưa thông tin lên trên, badge và nút thành các hàng riêng ở màn hình hẹp.
            row.ColumnCount = 3;
            row.RowCount = narrow ? 3 : 1;
            row.SetCellPosition(badge, new TableLayoutPanelCellPosition(narrow ? 0 : 1, narrow ? 1 : 0));
            row.SetCellPosition(action, new TableLayoutPanelCellPosition(narrow ? 0 : 2, narrow ? 2 : 0));
            row.ColumnCount = narrow ? 1 : 3;
            row.RowStyles.Clear();
            for (int i = 0; i < row.RowCount; i++) row.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            badge.Anchor = narrow ? AnchorStyles.Left : AnchorStyles.None;
            badge.Margin = narrow
                ? new Padding(0, StudentLayout.Space8, 0, StudentLayout.Space8)
                : new Padding(0, 0, StudentLayout.Space12, 0);
            action.Anchor = narrow ? AnchorStyles.Left : AnchorStyles.Right;
            row.ResumeLayout(true);
            WrapLabels(info);
        }
        row.SizeChanged += (_, _) => ArrangeRow();
        row.DpiChangedAfterParent += (_, _) => ArrangeRow();
        ArrangeRow();
        return row;
    }

    private void BuildSessionPage(User student)
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(StudentLayout.Space24)
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddRow(Control control)
        {
            int row = page.RowCount++;
            page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            page.Controls.Add(control, 0, row);
        }

        AddRow(TextLabel("Chọn ca thi", StudentLayout.PageTitleFontSize, FontStyle.Bold));
        AddRow(TextLabel("Chọn ca đang mở để bắt đầu, ca có tiến độ để tiếp tục hoặc ca đã nộp bài để xem kết quả."));
        var identity = Surface();
        identity.Controls.Add(TextLabel($"{student.FullName} · {student.StudentCode}", StudentLayout.HeadingFontSize, FontStyle.Bold));
        identity.Controls.Add(TextLabel($"Lớp {student.ClassName}"));
        identity.SizeChanged += (_, _) => WrapLabels(identity);
        AddRow(identity);

        var list = SessionList();
        AddRow(list);
        var status = TextLabel("");
        AddRow(status);

        void ShowError(Exception error, string message)
        {
            status.Text = StudentError.Describe(error, message);
            status.ForeColor = UiStyle.ErrorText;
        }

        void Reload()
        {
            list.SuspendLayout();
            try
            {
                while (list.Controls.Count > 0) list.Controls[0].Dispose();
                var sessionService = new ExamSessionService();
                var examService = new StudentExamService();
                var papers = sessionService.GetExamPapers();
                var progress = examService.GetSessionProgress(student.StudentCode);
                var sessions = sessionService.GetSessions().Select(session =>
                {
                    var paper = papers.FirstOrDefault(p => p.Id == session.PaperId);
                    bool hasProgress = progress.TryGetValue(session.Id, out bool submitted);
                    bool canEnter = submitted || (paper is not null && (session.IsOpen || hasProgress));
                    return (session, paper, hasProgress, submitted, canEnter);
                })
                // Ca mở dùng được đứng đầu, ca không có thao tác xuống cuối.
                .OrderBy(entry => !entry.canEnter ? 2 : entry.session.IsOpen ? 0 : 1)
                .ThenBy(entry => entry.submitted)
                .ToList();
                foreach (var (session, paper, hasProgress, submitted, canEnter) in sessions)
                {
                    var enter = StudentLayout.CreateButton(submitted ? "Xem kết quả"
                        : hasProgress ? "Tiếp tục lượt thi" : "Xem hướng dẫn",
                        (_, _) =>
                        {
                            try
                            {
                                if (submitted)
                                {
                                    var result = examService.GetResult(session.Id, student.StudentCode);
                                    if (result is null)
                                    {
                                        MessageBox.Show(this,
                                            "Bạn đã nộp bài ở ca này nhưng chưa có điểm. Vui lòng liên hệ giảng viên.",
                                            "Chưa có kết quả", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    ResultSelected?.Invoke(result);
                                    return;
                                }

                                var selected = examService.GetSelectedPaper(session.Id, student.StudentCode);
                                SessionSelected?.Invoke(session, selected);
                            }
                            catch (Exception ex)
                            {
                                ShowError(ex, submitted
                                    ? "Không tải được kết quả đã lưu. Hãy thử lại hoặc liên hệ giảng viên."
                                    : "Không mở được thông tin ca thi. Hãy tải lại danh sách hoặc liên hệ giảng viên.");
                            }
                        }, StudentButtonKind.Primary, submitted ? StudentArrow.None : StudentArrow.Right);
                    enter.Enabled = canEnter;
                    var state = submitted ? "Đã nộp bài" : !enter.Enabled ? "Đã khóa"
                        : session.IsOpen ? "Đang mở" : "Ca đã đóng";
                    var details = $"Ca: {session.SessionCode}\nPhòng: {session.RoomName}";
                    if (!enter.Enabled)
                    {
                        details += !session.IsOpen && !hasProgress
                            ? "\nCa đã đóng và bạn chưa bắt đầu lượt thi."
                            : "\nĐề thi chưa sẵn sàng. Hãy liên hệ giảng viên.";
                    }
                    else if (!session.IsOpen && !submitted)
                    {
                        details += "\nBạn có thể tiếp tục lượt thi đã lưu.";
                    }
                    var paperText = paper is null ? "Đề thi chưa sẵn sàng"
                        : $"{paper.Title} · {paper.TotalQuestions} câu · {paper.DurationMinutes} phút";
                    list.Controls.Add(SessionRow(session.SessionName, paperText, details, state, enter));
                }

                status.ForeColor = UiStyle.TextMuted;
                status.Text = sessions.Count == 0
                    ? "Chưa có ca thi. Hãy liên hệ giảng viên hoặc tải lại danh sách." : "";
            }
            catch (Exception ex)
            {
                ShowError(ex, "Không truy cập được danh sách ca thi. Hãy bấm Tải lại ca thi hoặc liên hệ giảng viên.");
            }
            finally
            {
                list.ResumeLayout(true);
            }
        }

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.CreateButton("Quay lại nhập MSSV",
            (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty),
            StudentButtonKind.Secondary, StudentArrow.Left));
        actions.Controls.Add(StudentLayout.Button("Tải lại ca thi", (_, _) => Reload()));
        AddRow(actions);
        page.SizeChanged += (_, _) =>
        {
            int width = Math.Max(1, page.ClientSize.Width - page.Padding.Horizontal);
            foreach (var label in page.Controls.OfType<Label>())
                label.MaximumSize = new Size(width, 0);
        };
        Controls.Add(page);
        Reload();
    }

    // Màn này ít thông tin nên giới hạn bề ngang, phần dư chia đều sang hai bên.
    private void CenterContent()
    {
        int maxWidth = StudentLayout.Scale(this, StudentLayout.SessionPageMaxWidth);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private static void WrapLabels(Control panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;

        foreach (var label in panel.Controls.OfType<Label>())
            label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
    }
}
