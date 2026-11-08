using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public event EventHandler? CloseRequested;
    public event Action<ExamSession, ExamPaper>? SessionSelected;

    public SessionSelectionControl()
    {
        InitializeComponent();
        ConfigureViewport();
    }

    public SessionSelectionControl(User student)
    {
        InitializeAppearance();
        ConfigureViewport();
        BuildSessionPage(student);
    }

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
        AddRow(TextLabel("Chọn ca đang mở để bắt đầu hoặc chọn ca có tiến độ để tiếp tục."));
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
                var sessions = sessionService.GetSessions();
                var papers = sessionService.GetExamPapers();
                var progress = examService.GetSessionProgress(student.StudentCode);
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

                    // Lượt đã nộp không mở lại; ca đóng vẫn cho khôi phục bản nháp hiện có.
                    var state = submitted ? "Đã nộp" : session.IsOpen ? "● Đang mở"
                        : hasProgress ? "Ca đã đóng · Có tiến độ" : "Ca đã đóng · Khóa";
                    card.Controls.Add(session.IsOpen && !submitted
                        ? Badge(state, UiStyle.SuccessBack, UiStyle.SuccessText)
                        : Badge(state, UiStyle.WarningBack, UiStyle.WarningText));
                    card.Controls.Add(TextLabel($"Mã ca: {session.SessionCode} · Phòng {session.RoomName}"));
                    var enter = StudentLayout.Button(hasProgress && !submitted ? "Tiếp tục lượt thi →" : "Xem hướng dẫn →",
                        (_, _) =>
                        {
                            try
                            {
                                var selected = examService.GetSelectedPaper(session.Id, student.StudentCode);
                                SessionSelected?.Invoke(session, selected);
                            }
                            catch (Exception ex)
                            {
                                ShowError(ex, "Không mở được thông tin ca thi. Hãy tải lại danh sách hoặc liên hệ giảng viên.");
                            }
                        }, true);
                    enter.Enabled = paper is not null && !submitted && (session.IsOpen || hasProgress);
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
