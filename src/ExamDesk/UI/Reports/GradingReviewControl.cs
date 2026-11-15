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
        WireEvents();
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
        _btnStatistics.Click += (_, _) => MessageBox.Show(this,"Màn hình phổ điểm (GV-14) đang được dựng.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowSelectedDetail()
    {
        if (_dgvResults.CurrentRow?.DataBoundItem is not AttemptResultItem item)
        {
            MessageBox.Show(this, "Hãy chọn một bài thi trong bảng.", "Chưa chọn bài",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Tạm thời: (hộp thoại chi tiết từng câu) sẽ thay chỗ này ở bước sau
        MessageBox.Show(this,
            $"{item.Mssv} - {item.HoTen}\nĐiểm: {item.Diem:0.00} ({item.CauDung}/{item.SoCau} câu đúng)",
            "Chi tiết bài thi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

}   
