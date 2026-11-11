namespace ExamDesk.UI.Reports;

// Bạn 2: phát triển giao diện trong control này; dữ liệu minh họa riêng của phân hệ.
public partial class GradingReviewControl : UserControl
{
    private readonly List<AttemptResultItem> _allAttempts = ReportsPreviewData.Attempts();
    private bool _isInitializing;
    public GradingReviewControl()
    {
        InitializeComponent();
        InitFilters();
        RefreshGrid();
    }
    private void InitFilters()
    {
        _isInitializing = true;
        _cboSession.Items.Clear();
        _cboSession.Items.Add("Tất cả ca thi");
        foreach (var s in ReportsPreviewData.Session())
            _cboSession.Items.Add(s);
        _cboSession.SelectedIndex = 0;
        _cboClass.Items.Clear();
        _cboClass.Items.Add("Tất cả lớp");
        foreach (var c in ReportsPreviewData.Classes())
            _cboClass.Items.Add(c);
        _cboClass.SelectedIndex = 0;
        _isInitializing = false;
    }
    private void RefreshGrid()
    {
        var list = _allAttempts.ToList();
        _dgvResults.DataSource = list;
        _lblCount.Text = $"Hiển thị {list.Count} / {_allAttempts.Count} bài";
        bool isEmpty = list.Count == 0;
        _lblEmpty.Visible = isEmpty;
        _dgvResults.Visible = !isEmpty;
        _btnViewDetail.Enabled = !isEmpty && _dgvResults.CurrentRow != null;
    }

}   
