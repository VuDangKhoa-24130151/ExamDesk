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
}   
