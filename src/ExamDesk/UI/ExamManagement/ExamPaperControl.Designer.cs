#nullable enable
namespace ExamDesk.UI.ExamManagement;

partial class ExamPaperControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblTitle = null!;
    private Panel _filterCardBorder = null!;
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
        _filterCardBorder = new Panel();
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
        _filterCardBorder.SuspendLayout();
        _filterPanel.SuspendLayout();
        SuspendLayout();

        // Root layout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 4;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Title
        _lblTitle.AutoSize = true;
        _lblTitle.Text = "Quản lý đề thi";
        _lblTitle.Margin = new Padding(0, 0, 0, 12);

        // Filter card + bar
        _filterCardBorder.Dock = DockStyle.Fill;
        _filterCardBorder.AutoSize = true;
        _filterCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _filterCardBorder.Padding = new Padding(1);
        _filterCardBorder.Margin = new Padding(0, 0, 0, 12);

        _filterPanel.Dock = DockStyle.Fill;
        _filterPanel.AutoSize = true;
        _filterPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _filterPanel.Padding = new Padding(12, 8, 12, 8);
        _filterPanel.FlowDirection = FlowDirection.LeftToRight;
        _filterPanel.WrapContents = true;
        _filterPanel.Margin = Padding.Empty;

        _lblSubjectFilter.AutoSize = true;
        _lblSubjectFilter.Text = "Môn học:";
        _lblSubjectFilter.Margin = new Padding(0, 7, 6, 4);

        _cboSubjectFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSubjectFilter.Width = 230;
        _cboSubjectFilter.Margin = new Padding(0, 2, 16, 4);
        _cboSubjectFilter.TabIndex = 0;

        _lblSearch.AutoSize = true;
        _lblSearch.Text = "Tìm kiếm:";
        _lblSearch.Margin = new Padding(0, 7, 6, 4);

        _txtSearch.Width = 220;
        _txtSearch.PlaceholderText = "Mã đề hoặc tiêu đề...";
        _txtSearch.Margin = new Padding(0, 2, 12, 4);
        _txtSearch.TabIndex = 1;

        _btnClearFilter.Text = "Xóa lọc";
        _btnClearFilter.Margin = new Padding(0, 1, 12, 4);
        _btnClearFilter.TabIndex = 2;

        _lblResultCount.AutoSize = true;
        _lblResultCount.Text = "Hiển thị 0 đề thi";
        _lblResultCount.Margin = new Padding(0, 7, 0, 4);

        _filterPanel.Controls.Add(_lblSubjectFilter);
        _filterPanel.Controls.Add(_cboSubjectFilter);
        _filterPanel.Controls.Add(_lblSearch);
        _filterPanel.Controls.Add(_txtSearch);
        _filterPanel.Controls.Add(_btnClearFilter);
        _filterPanel.Controls.Add(_lblResultCount);
        _filterCardBorder.Controls.Add(_filterPanel);

        // Grid container
        _gridContainer.Dock = DockStyle.Fill;
        _gridContainer.Margin = new Padding(0, 0, 0, 14);

        _dgvPapers.Dock = DockStyle.Fill;
        _dgvPapers.AutoGenerateColumns = false;
        _dgvPapers.TabIndex = 3;

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
        _btnCreate.TabIndex = 4;

        _btnPreview.Text = "Xem chi tiết";
        _btnPreview.Margin = new Padding(0, 0, 10, 0);
        _btnPreview.TabIndex = 5;

        _btnRegenerate.Text = "Rút lại câu";
        _btnRegenerate.Margin = new Padding(0, 0, 10, 0);
        _btnRegenerate.TabIndex = 6;

        _btnDelete.Text = "Xóa đề";
        _btnDelete.Margin = new Padding(0);
        _btnDelete.TabIndex = 7;

        _actionPanel.Controls.Add(_btnCreate);
        _actionPanel.Controls.Add(_btnPreview);
        _actionPanel.Controls.Add(_btnRegenerate);
        _actionPanel.Controls.Add(_btnDelete);

        _rootLayout.Controls.Add(_lblTitle, 0, 0);
        _rootLayout.Controls.Add(_filterCardBorder, 0, 1);
        _rootLayout.Controls.Add(_gridContainer, 0, 2);
        _rootLayout.Controls.Add(_actionPanel, 0, 3);

        Controls.Add(_rootLayout);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Name = "ExamPaperControl";
        Size = new Size(1000, 600);

        _filterCardBorder.ResumeLayout(false);
        _filterCardBorder.PerformLayout();
        _filterPanel.ResumeLayout(false);
        _filterPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvPapers).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
