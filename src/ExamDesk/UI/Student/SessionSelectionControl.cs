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
            Padding = new Padding(24), TabIndex = 0, TabStop = false
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
            WrapContents = false, Margin = new Padding(0, 0, 0, 12)
        };
        heading.Controls.Add(TextLabel("Chọn ca thi", 18, FontStyle.Bold));
        heading.Controls.Add(TextLabel("Chỉ hiển thị các ca thi có tên bạn trong danh sách.", 10));
        page.Controls.Add(heading, 0, 0);

        var identity = Surface();
        identity.Margin = new Padding(0, 0, 0, 12);
        identity.Controls.Add(TextLabel("Sinh viên mẫu · SV0001", 12, FontStyle.Bold));
        identity.Controls.Add(TextLabel("Lớp DH23DT • Chọn ca thi trong danh sách được phân công", 10));
        page.Controls.Add(identity, 0, 1);

        var sessions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            MinimumSize = new Size(0, 0), Margin = Padding.Empty, AutoSize = true, TabIndex = 0, TabStop = false
        };
        // Ca đang mở rộng hơn để có chỗ cho thông tin và nút vào thi.
        sessions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        sessions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        sessions.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var open = Surface();
        open.Dock = DockStyle.Fill;
        open.Margin = new Padding(0, 0, 12, 16);
        open.Controls.Add(TextLabel("Cuối kỳ .NET – Ca 01", 12, FontStyle.Bold));
        open.Controls.Add(TextLabel("Lập trình .NET • 30 câu • 45 phút", 10));
        open.Controls.Add(Badge("● Đang mở", UiStyle.SuccessBack, UiStyle.SuccessText));
        open.Controls.Add(TextLabel("Khung giờ được bắt đầu: 30/10/2026, 08:00–08:30", 10));
        open.Controls.Add(TextLabel("Lượt thi: Chưa bắt đầu", 10));
        var enter = new Button
        {
            Text = "&Xem hướng dẫn →", AutoSize = true,
            BackColor = UiStyle.Primary,
            ForeColor = UiStyle.Surface, FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 8, 0, 0), Padding = new Padding(12, 4, 12, 4),
            Cursor = Cursors.Hand, TabIndex = 0, AccessibleName = "Xem hướng dẫn ca thi 01"
        };
        UiStyle.StylePrimaryButton(enter);
        open.Controls.Add(enter);
        sessions.Controls.Add(open, 0, 0);

        var upcoming = Surface();
        upcoming.Dock = DockStyle.Fill;
        upcoming.Margin = new Padding(8, 0, 0, 16);
        upcoming.Controls.Add(TextLabel("Cuối kỳ .NET – Ca 02", 12, FontStyle.Bold));
        upcoming.Controls.Add(TextLabel("Lập trình .NET • 30 câu • 45 phút", 10));
        upcoming.Controls.Add(Badge("◷ Chưa đến giờ", UiStyle.WarningBack, UiStyle.WarningText));
        upcoming.Controls.Add(TextLabel("Được bắt đầu lúc 13:30 ngày 30/10/2026.\nBạn chưa thể vào ca thi này.", 10));
        sessions.Controls.Add(upcoming, 1, 0);
        page.Controls.Add(sessions, 0, 2);

        var back = new Button
        {
            Text = "← &Quay lại cửa sổ chính", AutoSize = true, TabIndex = 1,
            BackColor = UiStyle.Surface,
            ForeColor = UiStyle.Text, FlatStyle = FlatStyle.Flat,
            Padding = new Padding(12, 4, 12, 4), Margin = new Padding(0, 12, 0, 0)
        };
        UiStyle.StyleSecondaryButton(back);
        back.Click += (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty);
        page.Controls.Add(back, 0, 3);
        // Cho chữ tự xuống dòng theo chiều rộng từng khung khi phóng to hoặc thu nhỏ.
        heading.SizeChanged += (_, _) => WrapLabels(heading);
        identity.SizeChanged += (_, _) => WrapLabels(identity);
        open.SizeChanged += (_, _) => WrapLabels(open);
        upcoming.SizeChanged += (_, _) => WrapLabels(upcoming);
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

    private void BuildSessionPage(User student)
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(24)
        };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddRow(Control control)
        {
            int row = page.RowCount++;
            page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            page.Controls.Add(control, 0, row);
        }

        AddRow(TextLabel("Chọn ca thi", 18, FontStyle.Bold));
        AddRow(TextLabel("Chọn ca đang mở để bắt đầu, ca có tiến độ để tiếp tục hoặc ca đã nộp bài để xem kết quả."));
        var identity = Surface();
        identity.Controls.Add(TextLabel($"{student.FullName} · {student.StudentCode}", 12, FontStyle.Bold));
        identity.Controls.Add(TextLabel($"Lớp {student.ClassName}"));
        identity.SizeChanged += (_, _) => WrapLabels(identity);
        AddRow(identity);

        var list = new TableLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 12, 0, 12)
        };
        AddRow(list);
        var status = TextLabel("");
        AddRow(status);
        bool loading = false;

        void ShowError(Exception error, string message)
        {
            status.Text = StudentError.Describe(error, message);
            status.ForeColor = UiStyle.ErrorText;
        }

        void FitCards()
        {
            if (loading) return;
            int columns = list.ClientSize.Width < StudentLayout.Scale(this, 800) ? 1 : 2;
            var cards = list.Controls.Cast<Control>().ToArray();
            int rows = (cards.Length + columns - 1) / columns;
            if (list.ColumnCount == columns && list.RowCount == rows) return;

            list.SuspendLayout();
            list.ColumnCount = columns;
            list.RowCount = rows;
            list.ColumnStyles.Clear();
            list.RowStyles.Clear();
            for (int i = 0; i < columns; i++)
                list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            for (int i = 0; i < rows; i++)
                list.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < cards.Length; i++)
                list.SetCellPosition(cards[i], new TableLayoutPanelCellPosition(i % columns, i / columns));
            list.ResumeLayout(true);
        }

        list.SizeChanged += (_, _) => FitCards();
        void Reload()
        {
            loading = true;
            list.SuspendLayout();
            try
            {
                while (list.Controls.Count > 0) list.Controls[0].Dispose();
                list.ColumnCount = 2;
                list.ColumnStyles.Clear();
                list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                list.RowStyles.Clear();
                list.RowCount = 0;

                var sessionService = new ExamSessionService();
                var examService = new StudentExamService();
                var papers = sessionService.GetExamPapers();
                var progress = examService.GetSessionProgress(student.StudentCode);
                // Ưu tiên theo trạng thái sinh viên thấy trên thẻ, kể cả ca đã nộp còn mở.
                var sessions = sessionService.GetSessions().OrderBy(session =>
                {
                    bool hasProgress = progress.TryGetValue(session.Id, out bool submitted);
                    return submitted ? 2 : session.IsOpen ? 0 : hasProgress ? 1 : 3;
                }).ToList();
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    var paper = papers.FirstOrDefault(p => p.Id == session.PaperId);
                    bool hasProgress = progress.TryGetValue(session.Id, out bool submitted);
                    var card = Surface();
                    card.Margin = new Padding(0, 0, 12, 12);
                    card.Controls.Add(TextLabel(session.SessionName, 12, FontStyle.Bold));
                    card.Controls.Add(TextLabel(paper is null ? "Đề thi chưa sẵn sàng" :
                        $"{paper.Title} · {paper.TotalQuestions} câu · {paper.DurationMinutes} phút"));

                    // Ca đã nộp tra cứu điểm kể cả khi ca đóng hoặc thiếu thông tin đề.
                    var state = submitted ? "Đã nộp bài" : session.IsOpen ? "● Đang mở"
                        : hasProgress ? "Ca đã đóng · Có tiến độ" : "Ca đã đóng · Khóa";
                    card.Controls.Add(session.IsOpen && !submitted
                        ? Badge(state, UiStyle.SuccessBack, UiStyle.SuccessText)
                        : Badge(state, UiStyle.WarningBack, UiStyle.WarningText));
                    card.Controls.Add(TextLabel($"Mã ca: {session.SessionCode} · Phòng {session.RoomName}"));
                    var enter = StudentLayout.Button(submitted ? "Xem kết quả"
                        : hasProgress ? "Tiếp tục lượt thi →" : "Xem hướng dẫn →",
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
                        }, true);
                    enter.Enabled = submitted || (paper is not null && (session.IsOpen || hasProgress));
                    card.Controls.Add(enter);
                    card.SizeChanged += (_, _) => WrapLabels(card);

                    int row = i / 2;
                    if (row >= list.RowCount)
                    {
                        list.RowCount = row + 1;
                        list.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }
                    list.Controls.Add(card, i % 2, row);
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
                loading = false;
            }
            FitCards();
        }

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
        actions.Controls.Add(StudentLayout.Button("← Quay lại nhập MSSV", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty)));
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
        int maxWidth = StudentLayout.Scale(this, 1040);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private static void WrapLabels(FlowLayoutPanel panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;

        foreach (var label in panel.Controls.OfType<Label>())
            label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
    }
}
