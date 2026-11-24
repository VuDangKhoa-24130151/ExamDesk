using ExamDesk.Services;

namespace ExamDesk.UI.Reports;

// Bạn 2: Màn hình tra cứu kết quả thi, phúc khảo và thống kê phổ điểm từ SQLite.
public partial class GradingReviewControl : UserControl
{
    private readonly GradingReviewService _gradingService = new();
    private readonly ExamSessionService _sessionService = new();
    private readonly AdministrationService _adminService = new();
    private List<AttemptResultItem> _allAttempts = [];
    private bool _isInitializing;

    public GradingReviewControl()
    {
        InitializeComponent();
        LoadDataFromDatabase();
        WireEvents();
        RefreshGrid();
    }

    private void LoadDataFromDatabase()
    {
        var sessions = _sessionService.GetSessions();
        var sessionMap = sessions.ToDictionary(
            s => s.Id,
            s => $"{s.SessionCode} - {s.SessionName}");

        var attempts = _gradingService.GetAttempts();
        var items = new List<AttemptResultItem>(attempts.Count);

        foreach (var attempt in attempts)
        {
            var answers = _gradingService.GetAttemptAnswers(attempt.Id);
            int answeredCount = answers.Count(a => !string.IsNullOrWhiteSpace(a.SelectedOption));
            int blankCount = Math.Max(0, attempt.TotalQuestions - answeredCount);

            string sessionLabel = sessionMap.TryGetValue(attempt.SessionId, out var name)
                ? name
                : $"Ca #{attempt.SessionId}";

            items.Add(new AttemptResultItem
            {
                AttemptId = attempt.Id,
                SessionId = attempt.SessionId,
                Mssv = attempt.StudentCode,
                HoTen = attempt.StudentName,
                Lop = attempt.ClassName,
                Ca = sessionLabel,
                SoCau = attempt.TotalQuestions,
                CauDung = attempt.CorrectCount,
                CauBoTrong = blankCount,
                CachNop = attempt.IsAutoSubmitted ? ReportsPreviewData.AutoSubmit : ReportsPreviewData.ManualSubmit,
                NopLuc = attempt.SubmittedAt.ToLocalTime().DateTime,
                DiemLuu = attempt.Score
            });
        }

        _allAttempts = items;
        InitFilters(sessions.Select(s => $"{s.SessionCode} - {s.SessionName}"));
    }

    private void InitFilters(IEnumerable<string> dbSessions)
    {
        _isInitializing = true;

        var selectedSession = _cboSession.SelectedItem as string;
        _cboSession.Items.Clear();
        _cboSession.Items.Add("Tất cả ca thi");

        var sessionNames = dbSessions
            .Concat(_allAttempts.Select(a => a.Ca))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var s in sessionNames)
            _cboSession.Items.Add(s);

        _cboSession.SelectedIndex = selectedSession is not null && _cboSession.Items.Contains(selectedSession)
            ? _cboSession.Items.IndexOf(selectedSession)
            : 0;

        var selectedClass = _cboClass.SelectedItem as string;
        _cboClass.Items.Clear();
        _cboClass.Items.Add("Tất cả lớp");

        var classNames = _adminService.GetClasses().Select(c => c.Code)
            .Concat(_allAttempts.Select(a => a.Lop))
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase);

        foreach (var c in classNames)
            _cboClass.Items.Add(c);

        _cboClass.SelectedIndex = selectedClass is not null && _cboClass.Items.Contains(selectedClass)
            ? _cboClass.Items.IndexOf(selectedClass)
            : 0;

        _isInitializing = false;
    }
        private void RefreshGrid()
    {
        IEnumerable<AttemptResultItem> query = _allAttempts;

        // Mục 0 của ComboBox là "Tất cả", nên chỉ lọc khi chọn mục từ 1 trở đi
        if (_cboSession.SelectedIndex > 0 && _cboSession.SelectedItem is string session)
            query = query.Where(a => a.Ca == session);

        if (_cboClass.SelectedIndex > 0 && _cboClass.SelectedItem is string className)
            query = query.Where(a => a.Lop == className);

        string keyword = _txtSearch.Text.Trim();
        if (keyword.Length > 0)
            query = query.Where(a =>
                a.Mssv.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                a.HoTen.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        var list = query.ToList();
        _dgvResults.DataSource = list;
        _lblCount.Text = $"Hiển thị {list.Count} / {_allAttempts.Count} bài";

        bool isEmpty = list.Count == 0;
        _lblEmpty.Visible = isEmpty;
        _dgvResults.Visible = !isEmpty;
        _btnViewDetail.Enabled = !isEmpty;
    }
        private void WireEvents()
    {
        // Đang nạp ComboBox thì bỏ qua, tránh lọc lại nhiều lần
        _cboSession.SelectedIndexChanged += (_, _) => { if (!_isInitializing) RefreshGrid(); };
        _cboClass.SelectedIndexChanged += (_, _) => { if (!_isInitializing) RefreshGrid(); };
        _txtSearch.TextChanged += (_, _) => { if (!_isInitializing) RefreshGrid(); };

        _btnClearFilter.Click += (_, _) =>
        {
            _isInitializing = true;
            _cboSession.SelectedIndex = 0;
            _cboClass.SelectedIndex = 0;
            _txtSearch.Clear();
            _isInitializing = false;
            RefreshGrid();
        };

        _btnViewDetail.Click += (_, _) => ShowSelectedDetail();
        _dgvResults.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) ShowSelectedDetail(); };

        //GV-14
        _btnStatistics.Click += (_, _) => ShowStatistics();
    }

    private void ShowSelectedDetail()
    {
        if (_dgvResults.CurrentRow?.DataBoundItem is not AttemptResultItem item)
        {
            MessageBox.Show(this, "Hãy chọn một bài thi trong bảng.", "Chưa chọn bài",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dialog = new AttemptDetailDialog(item);
        dialog.ShowDialog(this);
    }
        private void ShowStatistics()
    {
        var currentList = _dgvResults.DataSource as List<AttemptResultItem> ?? _allAttempts;
        string sessionText = _cboSession.SelectedItem as string ?? "Tất cả ca thi";
        string classText = _cboClass.SelectedItem as string ?? "Tất cả lớp";

        using var dialog = new ScoreDistributionDialog(currentList, $"{sessionText} • {classText}");
        dialog.ShowDialog(this);
    }

}   
