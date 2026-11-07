#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class HelpControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblOfflineBadge = null!;
    private TableLayoutPanel _splitLayout = null!;

    // Left column (~300px): Search + TreeView
    private Panel _leftCardBorder = null!;
    private TableLayoutPanel _leftCardLayout = null!;
    private TableLayoutPanel _searchLayout = null!;
    private Label _lblSearch = null!;
    private TextBox _txtSearch = null!;
    private Button _btnClearSearch = null!;
    private Label _lblTopicCount = null!;
    private Panel _treeBorderPanel = null!;
    private Panel _treeHostPanel = null!;
    private TreeView _tvTopics = null!;
    private Label _lblTreeEmptyState = null!;

    // Right column: Topic content
    private Panel _rightCardBorder = null!;
    private Panel _rightCardHost = null!;
    private Panel _topicScrollPanel = null!;
    private FlowLayoutPanel _topicContentFlow = null!;
    private Label _lblTopicGroup = null!;
    private Label _lblTopicTitle = null!;
    private Label _lblTopicKeywords = null!;
    private Panel _topicDivider = null!;
    private Label _lblStepsHeader = null!;
    private FlowLayoutPanel _stepsFlow = null!;
    private FlowLayoutPanel _noteBox = null!;
    private Label _lblNoteHeader = null!;
    private Label _lblNoteContent = null!;
    private Label _lblRightEmptyState = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _lblOfflineBadge = new Label();
        _splitLayout = new TableLayoutPanel();

        _leftCardBorder = new Panel();
        _leftCardLayout = new TableLayoutPanel();
        _searchLayout = new TableLayoutPanel();
        _lblSearch = new Label();
        _txtSearch = new TextBox();
        _btnClearSearch = new Button();
        _lblTopicCount = new Label();
        _treeBorderPanel = new Panel();
        _treeHostPanel = new Panel();
        _tvTopics = new TreeView();
        _lblTreeEmptyState = new Label();

        _rightCardBorder = new Panel();
        _rightCardHost = new Panel();
        _topicScrollPanel = new Panel();
        _topicContentFlow = new FlowLayoutPanel();
        _lblTopicGroup = new Label();
        _lblTopicTitle = new Label();
        _lblTopicKeywords = new Label();
        _topicDivider = new Panel();
        _lblStepsHeader = new Label();
        _stepsFlow = new FlowLayoutPanel();
        _noteBox = new FlowLayoutPanel();
        _lblNoteHeader = new Label();
        _lblNoteContent = new Label();
        _lblRightEmptyState = new Label();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _splitLayout.SuspendLayout();
        _leftCardBorder.SuspendLayout();
        _leftCardLayout.SuspendLayout();
        _searchLayout.SuspendLayout();
        _treeBorderPanel.SuspendLayout();
        _treeHostPanel.SuspendLayout();
        _rightCardBorder.SuspendLayout();
        _rightCardHost.SuspendLayout();
        _topicScrollPanel.SuspendLayout();
        _topicContentFlow.SuspendLayout();
        _noteBox.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 2;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Left Tree + Right Content

        // _headerLayout
        _headerLayout.Dock = DockStyle.Fill;
        _headerLayout.AutoSize = true;
        _headerLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _headerLayout.ColumnCount = 2;
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _headerLayout.RowCount = 1;
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _headerLayout.Margin = new Padding(0, 0, 0, 12);

        // _lblTitle
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0);
        _lblTitle.Text = "Trợ giúp";

        // _lblOfflineBadge
        _lblOfflineBadge.AutoSize = true;
        _lblOfflineBadge.BackColor = UiStyle.SuccessBack;
        _lblOfflineBadge.ForeColor = UiStyle.SuccessText;
        _lblOfflineBadge.Font = UiStyle.SmallFont;
        _lblOfflineBadge.Padding = new Padding(10, 6, 10, 6);
        _lblOfflineBadge.Margin = new Padding(8, 4, 0, 0);
        _lblOfflineBadge.Text = "Hướng dẫn offline theo tác vụ";

        _headerLayout.Controls.Add(_lblTitle, 0, 0);
        _headerLayout.Controls.Add(_lblOfflineBadge, 1, 0);

        // _splitLayout (Left 310px | Right 100%)
        _splitLayout.Dock = DockStyle.Fill;
        _splitLayout.Margin = new Padding(0);
        _splitLayout.ColumnCount = 2;
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _splitLayout.RowCount = 1;
        _splitLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // ================= LEFT COLUMN (~300px) =================
        _leftCardBorder.Dock = DockStyle.Fill;
        _leftCardBorder.BackColor = UiStyle.Border;
        _leftCardBorder.Padding = new Padding(1);
        _leftCardBorder.Margin = new Padding(0, 0, 8, 0);

        _leftCardLayout.Dock = DockStyle.Fill;
        _leftCardLayout.BackColor = UiStyle.Surface;
        _leftCardLayout.Padding = new Padding(14);
        _leftCardLayout.ColumnCount = 1;
        _leftCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _leftCardLayout.RowCount = 3;
        _leftCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Search bar
        _leftCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 1: Count label
        _leftCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 2: TreeView

        // _searchLayout
        _searchLayout.Dock = DockStyle.Fill;
        _searchLayout.AutoSize = true;
        _searchLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _searchLayout.ColumnCount = 2;
        _searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _searchLayout.RowCount = 2;
        _searchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _searchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _searchLayout.Margin = new Padding(0, 0, 0, 6);

        _lblSearch.AutoSize = true;
        _lblSearch.Font = UiStyle.SmallFont;
        _lblSearch.ForeColor = UiStyle.TextMuted;
        _lblSearch.Margin = new Padding(0, 0, 0, 4);
        _lblSearch.Text = "Tìm chủ đề hoặc từ khóa (Ctrl+F)";

        _txtSearch.Dock = DockStyle.Fill;
        _txtSearch.Font = UiStyle.BodyFont;
        _txtSearch.PlaceholderText = "Nhập tiêu đề, từ khóa…";
        _txtSearch.Margin = new Padding(0, 4, 6, 0);
        _txtSearch.TabIndex = 0;
        _txtSearch.TextChanged += OnSearchTextChanged;

        _btnClearSearch.Width = 72;
        _btnClearSearch.Height = 32;
        _btnClearSearch.Margin = new Padding(0);
        _btnClearSearch.TabIndex = 1;
        _btnClearSearch.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnClearSearch);
        _btnClearSearch.Height = 32;
        _btnClearSearch.Click += OnClearSearchClick;

        _searchLayout.Controls.Add(_lblSearch, 0, 0);
        _searchLayout.SetColumnSpan(_lblSearch, 2);
        _searchLayout.Controls.Add(_txtSearch, 0, 1);
        _searchLayout.Controls.Add(_btnClearSearch, 1, 1);

        // _lblTopicCount
        _lblTopicCount.AutoSize = true;
        _lblTopicCount.Font = UiStyle.SmallFont;
        _lblTopicCount.ForeColor = UiStyle.TextMuted;
        _lblTopicCount.Margin = new Padding(0, 0, 0, 8);
        _lblTopicCount.Text = "Hiển thị 0 chủ đề";

        // _treeBorderPanel
        _treeBorderPanel.Dock = DockStyle.Fill;
        _treeBorderPanel.BackColor = UiStyle.Border;
        _treeBorderPanel.Padding = new Padding(1);
        _treeBorderPanel.Margin = new Padding(0);

        _treeHostPanel.Dock = DockStyle.Fill;
        _treeHostPanel.BackColor = UiStyle.Surface;

        _tvTopics.Dock = DockStyle.Fill;
        _tvTopics.BorderStyle = BorderStyle.None;
        _tvTopics.BackColor = UiStyle.Surface;
        _tvTopics.ForeColor = UiStyle.Text;
        _tvTopics.Font = UiStyle.BodyFont;
        _tvTopics.FullRowSelect = true;
        _tvTopics.HideSelection = false;
        _tvTopics.ShowLines = false;
        _tvTopics.ItemHeight = 28;
        _tvTopics.Indent = 18;
        _tvTopics.TabIndex = 2;
        _tvTopics.AfterSelect += OnTreeTopicAfterSelect;

        _lblTreeEmptyState.Dock = DockStyle.Fill;
        _lblTreeEmptyState.BackColor = UiStyle.Surface;
        _lblTreeEmptyState.ForeColor = UiStyle.TextMuted;
        _lblTreeEmptyState.Font = UiStyle.BodyFont;
        _lblTreeEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblTreeEmptyState.Padding = new Padding(12);
        _lblTreeEmptyState.Text = "Không tìm thấy chủ đề phù hợp";
        _lblTreeEmptyState.Visible = false;

        _treeHostPanel.Controls.Add(_lblTreeEmptyState);
        _treeHostPanel.Controls.Add(_tvTopics);
        _treeBorderPanel.Controls.Add(_treeHostPanel);

        _leftCardLayout.Controls.Add(_searchLayout, 0, 0);
        _leftCardLayout.Controls.Add(_lblTopicCount, 0, 1);
        _leftCardLayout.Controls.Add(_treeBorderPanel, 0, 2);
        _leftCardBorder.Controls.Add(_leftCardLayout);

        // ================= RIGHT COLUMN: TOPIC CONTENT =================
        _rightCardBorder.Dock = DockStyle.Fill;
        _rightCardBorder.BackColor = UiStyle.Border;
        _rightCardBorder.Padding = new Padding(1);
        _rightCardBorder.Margin = new Padding(8, 0, 0, 0);

        _rightCardHost.Dock = DockStyle.Fill;
        _rightCardHost.BackColor = UiStyle.Surface;

        _topicScrollPanel.Dock = DockStyle.Fill;
        _topicScrollPanel.AutoScroll = true;
        _topicScrollPanel.BackColor = UiStyle.Surface;
        _topicScrollPanel.Padding = new Padding(24, 20, 24, 20);
        _topicScrollPanel.SizeChanged += OnTopicScrollPanelSizeChanged;

        _topicContentFlow.Dock = DockStyle.Top;
        _topicContentFlow.AutoSize = true;
        _topicContentFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _topicContentFlow.FlowDirection = FlowDirection.TopDown;
        _topicContentFlow.WrapContents = false;
        _topicContentFlow.BackColor = UiStyle.Surface;
        _topicContentFlow.Margin = new Padding(0);

        // _lblTopicGroup
        _lblTopicGroup.AutoSize = true;
        _lblTopicGroup.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        _lblTopicGroup.ForeColor = UiStyle.Primary;
        _lblTopicGroup.Margin = new Padding(0, 0, 0, 4);
        _lblTopicGroup.Text = "NHÓM CHỦ ĐỀ";

        // _lblTopicTitle
        _lblTopicTitle.AutoSize = true;
        _lblTopicTitle.Font = UiStyle.TitleFont;
        _lblTopicTitle.ForeColor = UiStyle.Text;
        _lblTopicTitle.Margin = new Padding(0, 0, 0, 6);
        _lblTopicTitle.Text = "Tiêu đề chủ đề";

        // _lblTopicKeywords
        _lblTopicKeywords.AutoSize = true;
        _lblTopicKeywords.Font = UiStyle.SmallFont;
        _lblTopicKeywords.ForeColor = UiStyle.TextMuted;
        _lblTopicKeywords.Margin = new Padding(0, 0, 0, 12);
        _lblTopicKeywords.Text = "Từ khóa: —";

        // _topicDivider
        _topicDivider.Height = 1;
        _topicDivider.Width = 500;
        _topicDivider.BackColor = UiStyle.Border;
        _topicDivider.Margin = new Padding(0, 0, 0, 16);

        // _lblStepsHeader
        _lblStepsHeader.AutoSize = true;
        _lblStepsHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblStepsHeader.ForeColor = UiStyle.Text;
        _lblStepsHeader.Margin = new Padding(0, 0, 0, 10);
        _lblStepsHeader.Text = "Các bước thực hiện";

        // _stepsFlow
        _stepsFlow.AutoSize = true;
        _stepsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _stepsFlow.FlowDirection = FlowDirection.TopDown;
        _stepsFlow.WrapContents = false;
        _stepsFlow.BackColor = UiStyle.Surface;
        _stepsFlow.Margin = new Padding(0, 0, 0, 16);

        // _noteBox (WarningBack)
        _noteBox.AutoSize = true;
        _noteBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _noteBox.FlowDirection = FlowDirection.TopDown;
        _noteBox.WrapContents = false;
        _noteBox.BackColor = UiStyle.WarningBack;
        _noteBox.Padding = new Padding(14, 12, 14, 12);
        _noteBox.Margin = new Padding(0);

        _lblNoteHeader.AutoSize = true;
        _lblNoteHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _lblNoteHeader.ForeColor = UiStyle.WarningText;
        _lblNoteHeader.Margin = new Padding(0, 0, 0, 4);
        _lblNoteHeader.Text = "Lưu ý";

        _lblNoteContent.AutoSize = true;
        _lblNoteContent.Font = UiStyle.BodyFont;
        _lblNoteContent.ForeColor = UiStyle.WarningText;
        _lblNoteContent.Margin = new Padding(0);
        _lblNoteContent.Text = string.Empty;

        _noteBox.Controls.Add(_lblNoteHeader);
        _noteBox.Controls.Add(_lblNoteContent);

        _topicContentFlow.Controls.Add(_lblTopicGroup);
        _topicContentFlow.Controls.Add(_lblTopicTitle);
        _topicContentFlow.Controls.Add(_lblTopicKeywords);
        _topicContentFlow.Controls.Add(_topicDivider);
        _topicContentFlow.Controls.Add(_lblStepsHeader);
        _topicContentFlow.Controls.Add(_stepsFlow);
        _topicContentFlow.Controls.Add(_noteBox);

        _topicScrollPanel.Controls.Add(_topicContentFlow);

        _lblRightEmptyState.Dock = DockStyle.Fill;
        _lblRightEmptyState.BackColor = UiStyle.Surface;
        _lblRightEmptyState.ForeColor = UiStyle.TextMuted;
        _lblRightEmptyState.Font = UiStyle.BodyFont;
        _lblRightEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblRightEmptyState.Text = "Không tìm thấy chủ đề phù hợp";
        _lblRightEmptyState.Visible = false;

        _rightCardHost.Controls.Add(_lblRightEmptyState);
        _rightCardHost.Controls.Add(_topicScrollPanel);
        _rightCardBorder.Controls.Add(_rightCardHost);

        _splitLayout.Controls.Add(_leftCardBorder, 0, 0);
        _splitLayout.Controls.Add(_rightCardBorder, 1, 0);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_splitLayout, 0, 1);

        // HelpControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "HelpControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _splitLayout.ResumeLayout(false);
        _leftCardBorder.ResumeLayout(false);
        _leftCardLayout.ResumeLayout(false);
        _leftCardLayout.PerformLayout();
        _searchLayout.ResumeLayout(false);
        _searchLayout.PerformLayout();
        _treeBorderPanel.ResumeLayout(false);
        _treeHostPanel.ResumeLayout(false);
        _rightCardBorder.ResumeLayout(false);
        _rightCardHost.ResumeLayout(false);
        _topicScrollPanel.ResumeLayout(false);
        _topicScrollPanel.PerformLayout();
        _topicContentFlow.ResumeLayout(false);
        _topicContentFlow.PerformLayout();
        _noteBox.ResumeLayout(false);
        _noteBox.PerformLayout();
        ResumeLayout(false);
    }
}
