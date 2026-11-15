#nullable enable
namespace ExamDesk.UI.ExamManagement;

partial class ExamPaperControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private FlowLayoutPanel _filterPanel = null!;
    private Label _lblSubjectFilter = null!;
    private ComboBox _cboSubjectFilter = null!;
    private Label _lblSearch = null!;
    private TextBox _txtSearch = null!;
    private Button _btnClearFilter = null!;
    private Label _lblResultCount = null!;
    private Panel _gridContainer = null!;
    private DataGridView _dgvPapers = null!;
    private Panel _emptyStatePanel = null!;
    private Label _lblEmptyState = null!;
    private FlowLayoutPanel _actionPanel = null!;
    private Button _btnCreate = null!;
    private Button _btnPreview = null!;
    private Button _btnRegenerate = null!;
    private Button _btnDelete = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _filterPanel = new FlowLayoutPanel();
        _lblSubjectFilter = new Label();
        _cboSubjectFilter = new ComboBox();
        _lblSearch = new Label();
        _txtSearch = new TextBox();
        _btnClearFilter = new Button();
        _lblResultCount = new Label();
        _gridContainer = new Panel();
        _dgvPapers = new DataGridView();
        _emptyStatePanel = new Panel();
        _lblEmptyState = new Label();
        _actionPanel = new FlowLayoutPanel();
        _btnCreate = new Button();
        _btnPreview = new Button();
        _btnRegenerate = new Button();
        _btnDelete = new Button();

        ((System.ComponentModel.ISupportInitialize)_dgvPapers).BeginInit();
        SuspendLayout();

        // Root layout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.ColumnCount = 1;
        _rootLayout.RowCount = 4;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Title
        _lblTitle.AutoSize = true;
        _lblTitle.Text = "Quản lý đề thi";
        _lblTitle.Margin = new Padding(0, 0, 0, 14);

        // Filter bar
        _filterPanel.Dock = DockStyle.Fill;
        _filterPanel.AutoSize = true;
        _filterPanel.FlowDirection = FlowDirection.LeftToRight;
        _filterPanel.WrapContents = true;
        _filterPanel.Margin = new Padding(0, 0, 0, 12);

        _lblSubjectFilter.AutoSize = true;
        _lblSubjectFilter.Text = "Môn học:";
        _lblSubjectFilter.Margin = new Padding(0, 9, 6, 6);

        _cboSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSubjectFilter.Width = 240;
        _cboSubjectFilter.Margin = new Padding(0, 4, 16, 6);

        _lblSearch.AutoSize = true;
        _lblSearch.Text = "Tìm kiếm:";
        _lblSearch.Margin = new Padding(0, 9, 6, 6);

        _txtSearch.Width = 240;
        _txtSearch.PlaceholderText = "Mã đề hoặc tiêu đề...";
        _txtSearch.Margin = new Padding(0, 4, 12, 6);

        _btnClearFilter.Text = "Xóa lọc";
        _btnClearFilter.Margin = new Padding(0, 0, 16, 6);

        _lblResultCount.AutoSize = true;
        _lblResultCount.Text = "Hiển thị 0 đề thi";
        _lblResultCount.Margin = new Padding(0, 10, 0, 6);

        _filterPanel.Controls.Add(_lblSubjectFilter);
        _filterPanel.Controls.Add(_cboSubjectFilter);
        _filterPanel.Controls.Add(_lblSearch);
        _filterPanel.Controls.Add(_txtSearch);
        _filterPanel.Controls.Add(_btnClearFilter);
        _filterPanel.Controls.Add(_lblResultCount);

        // Grid container
        _gridContainer.Dock = DockStyle.Fill;
        _gridContainer.Margin = new Padding(0, 0, 0, 14);

        _dgvPapers.Dock = DockStyle.Fill;
        _dgvPapers.AutoGenerateColumns = false;

        _emptyStatePanel.Dock = DockStyle.Fill;
        _emptyStatePanel.Visible = false;

        _lblEmptyState.Dock = DockStyle.Fill;
        _lblEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblEmptyState.Text = "Chưa có đề thi nào.\nBấm \"Tạo đề mới\" ở thanh bên dưới để tạo đề thi ngẫu nhiên.";

        _emptyStatePanel.Controls.Add(_lblEmptyState);
        _gridContainer.Controls.Add(_dgvPapers);
        _gridContainer.Controls.Add(_emptyStatePanel);
        _emptyStatePanel.BringToFront();

        // Action bar
        _actionPanel.Dock = DockStyle.Fill;
        _actionPanel.AutoSize = true;
        _actionPanel.FlowDirection = FlowDirection.LeftToRight;
        _actionPanel.WrapContents = true;
        _actionPanel.Margin = new Padding(0);

        _btnCreate.Text = "Tạo đề mới";
        _btnCreate.Margin = new Padding(0, 0, 10, 0);

        _btnPreview.Text = "Xem chi tiết";
        _btnPreview.Margin = new Padding(0, 0, 10, 0);

        _btnRegenerate.Text = "Rút lại câu";
        _btnRegenerate.Margin = new Padding(0, 0, 10, 0);

        _btnDelete.Text = "Xóa đề";
        _btnDelete.Margin = new Padding(0);

        _actionPanel.Controls.Add(_btnCreate);
        _actionPanel.Controls.Add(_btnPreview);
        _actionPanel.Controls.Add(_btnRegenerate);
        _actionPanel.Controls.Add(_btnDelete);

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_filterPanel, 0, 1);
        _rootLayout.Controls.Add(_gridContainer, 0, 2);
        _rootLayout.Controls.Add(_actionPanel, 0, 3);

        Controls.Add(_rootLayout);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Name = "ExamPaperControl";
        Size = new Size(1000, 600);

        ((System.ComponentModel.ISupportInitialize)_dgvPapers).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
