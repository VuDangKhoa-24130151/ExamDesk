#nullable enable
using ExamDesk.Services;

namespace ExamDesk.UI.Reports;

// Bạn 2: Màn hình tra cứu kết quả thi, phúc khảo và thống kê phổ điểm từ SQLite.
public partial class GradingReviewControl : UserControl
{
    private readonly GradingReviewService _gradingService = new();
    private readonly ExamSessionService _sessionService = new();
    private readonly AdministrationService _adminService = new();
    private List<AttemptSummaryItem> _currentAttempts = [];
    private bool _isInitializing;

    public GradingReviewControl()
    {
        InitializeComponent();
        var initialAttempts = _gradingService.GetAttemptSummaries();
        LoadFilters(initialAttempts);
        WireEvents();
        RefreshGrid(initialAttempts);
    }

    private void LoadFilters(IReadOnlyList<AttemptSummaryItem>? preloadedAttempts = null)
    {
        _isInitializing = true;

        int? selectedSessionId = (_cboSession.SelectedItem as SessionFilterOption)?.SessionId;
        _cboSession.Items.Clear();
        _cboSession.Items.Add(new SessionFilterOption(null, "Tất cả ca thi"));

        foreach (var session in _sessionService.GetSessions())
        {
            _cboSession.Items.Add(new SessionFilterOption(
                session.Id,
                $"{session.SessionCode} - {session.SessionName}"));
        }

        int sessionIndex = 0;
        if (selectedSessionId.HasValue)
        {
            for (int i = 0; i < _cboSession.Items.Count; i++)
            {
                if (_cboSession.Items[i] is SessionFilterOption opt && opt.SessionId == selectedSessionId.Value)
                {
                    sessionIndex = i;
                    break;
                }
            }
        }
        _cboSession.SelectedIndex = sessionIndex;

        string? selectedClass = _cboClass.SelectedItem as string;
        _cboClass.Items.Clear();
        _cboClass.Items.Add("Tất cả lớp");

        // Gộp danh sách lớp từ danh mục quản trị và các lớp đã có bài thi trong CSDL
        var dbAttempts = preloadedAttempts ?? _gradingService.GetAttemptSummaries();
        var classNames = _adminService.GetClasses().Select(c => c.Code)
            .Concat(dbAttempts.Select(a => a.ClassName))
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase);

        foreach (var className in classNames)
            _cboClass.Items.Add(className);

        _cboClass.SelectedIndex = selectedClass is not null && _cboClass.Items.Contains(selectedClass)
            ? _cboClass.Items.IndexOf(selectedClass)
            : 0;

        _isInitializing = false;
    }

    private void RefreshGrid(List<AttemptSummaryItem>? preloadedAttempts = null)
    {
        int? sessionId = (_cboSession.SelectedItem as SessionFilterOption)?.SessionId;
        string? className = _cboClass.SelectedIndex > 0 ? _cboClass.SelectedItem as string : null;
        string keyword = _txtSearch.Text.Trim();

        int totalInDb = preloadedAttempts?.Count ?? _gradingService.GetTotalAttemptCount();
        _currentAttempts = preloadedAttempts is not null && !sessionId.HasValue && className is null && keyword.Length == 0
            ? preloadedAttempts
            : _gradingService.GetAttemptSummaries(sessionId, className, keyword);

        _dgvResults.DataSource = _currentAttempts;
        _lblCount.Text = $"Hiển thị {_currentAttempts.Count} / {totalInDb} bài";

        bool isEmpty = _currentAttempts.Count == 0;
        // Phân biệt rõ giữa CSDL chưa có bài nào và lọc không ra kết quả
        _lblEmpty.Text = totalInDb == 0
            ? "Chưa có bài thi nào được nộp trong cơ sở dữ liệu."
            : "Không có bài thi nào khớp với điều kiện lọc.";
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

        _btnRefresh.Click += (_, _) =>
        {
            var latest = _gradingService.GetAttemptSummaries();
            LoadFilters(latest);
            RefreshGrid(latest);
        };

        _btnViewDetail.Click += (_, _) => ShowSelectedDetail();
        _dgvResults.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) ShowSelectedDetail(); };
        _dgvResults.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ShowSelectedDetail();
            }
        };

        _btnStatistics.Click += (_, _) => ShowStatistics();
    }

    private void ShowSelectedDetail()
    {
        if (_dgvResults.CurrentRow?.DataBoundItem is not AttemptSummaryItem item)
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
        string sessionText = (_cboSession.SelectedItem as SessionFilterOption)?.Label ?? "Tất cả ca thi";
        string classText = _cboClass.SelectedItem as string ?? "Tất cả lớp";
        var summary = _gradingService.CalculateScoreDistribution(_currentAttempts, $"{sessionText} • {classText}");

        using var dialog = new ScoreDistributionDialog(summary);
        dialog.ShowDialog(this);
    }

    private sealed record SessionFilterOption(int? SessionId, string Label)
    {
        public override string ToString() => Label;
    }
}
