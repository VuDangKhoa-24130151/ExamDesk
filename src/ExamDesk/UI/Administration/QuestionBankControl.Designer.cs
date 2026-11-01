#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class QuestionBankControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private Panel _filterCardBorder = null!;
    private FlowLayoutPanel _filterFlow = null!;
    private FlowLayoutPanel _grpSubject = null!;
    private Label _lblSubjectFilter = null!;
    private ComboBox _cboSubject = null!;
    private FlowLayoutPanel _grpChapter = null!;
    private Label _lblChapterFilter = null!;
    private ComboBox _cboChapter = null!;
    private FlowLayoutPanel _grpDifficulty = null!;
    private Label _lblDifficultyFilter = null!;
    private ComboBox _cboDifficulty = null!;
    private FlowLayoutPanel _grpSearch = null!;
    private Label _lblSearchFilter = null!;
    private TextBox _txtSearch = null!;
    private FlowLayoutPanel _grpFilterActions = null!;
    private Button _btnClearFilter = null!;
    private Label _lblCount = null!;
    private Panel _gridBorderPanel = null!;
    private Panel _gridHostPanel = null!;
    private DataGridView _dgvQuestions = null!;
    private DataGridViewTextBoxColumn _colId = null!;
    private DataGridViewTextBoxColumn _colSubject = null!;
    private DataGridViewTextBoxColumn _colChapter = null!;
    private DataGridViewTextBoxColumn _colDifficulty = null!;
    private DataGridViewTextBoxColumn _colContent = null!;
    private DataGridViewTextBoxColumn _colCorrectOption = null!;
    private Label _lblEmptyState = null!;
    private FlowLayoutPanel _actionsPanel = null!;
    private Button _btnAdd = null!;
    private Button _btnEdit = null!;
    private Button _btnPreview = null!;
    private Button _btnDelete = null!;

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
        _lblDemoBadge = new Label();
        _filterCardBorder = new Panel();
        _filterFlow = new FlowLayoutPanel();
        _grpSubject = new FlowLayoutPanel();
        _lblSubjectFilter = new Label();
        _cboSubject = new ComboBox();
        _grpChapter = new FlowLayoutPanel();
        _lblChapterFilter = new Label();
        _cboChapter = new ComboBox();
        _grpDifficulty = new FlowLayoutPanel();
        _lblDifficultyFilter = new Label();
        _cboDifficulty = new ComboBox();
        _grpSearch = new FlowLayoutPanel();
        _lblSearchFilter = new Label();
        _txtSearch = new TextBox();
        _grpFilterActions = new FlowLayoutPanel();
        _btnClearFilter = new Button();
        _lblCount = new Label();
        _gridBorderPanel = new Panel();
        _gridHostPanel = new Panel();
        _dgvQuestions = new DataGridView();
        _colId = new DataGridViewTextBoxColumn();
        _colSubject = new DataGridViewTextBoxColumn();
        _colChapter = new DataGridViewTextBoxColumn();
        _colDifficulty = new DataGridViewTextBoxColumn();
        _colContent = new DataGridViewTextBoxColumn();
        _colCorrectOption = new DataGridViewTextBoxColumn();
        _lblEmptyState = new Label();
        _actionsPanel = new FlowLayoutPanel();
        _btnAdd = new Button();
        _btnEdit = new Button();
        _btnPreview = new Button();
        _btnDelete = new Button();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _filterCardBorder.SuspendLayout();
        _filterFlow.SuspendLayout();
        _grpSubject.SuspendLayout();
        _grpChapter.SuspendLayout();
        _grpDifficulty.SuspendLayout();
        _grpSearch.SuspendLayout();
        _grpFilterActions.SuspendLayout();
        _gridBorderPanel.SuspendLayout();
        _gridHostPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvQuestions).BeginInit();
        _actionsPanel.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 4;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 1: Filter bar
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 2: DataGridView + Empty state
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 3: Action buttons

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
        _lblTitle.Text = "Ngân hàng câu hỏi";

        // _lblDemoBadge
        _lblDemoBadge.AutoSize = true;
        _lblDemoBadge.BackColor = UiStyle.WarningBack;
        _lblDemoBadge.ForeColor = UiStyle.WarningText;
        _lblDemoBadge.Font = UiStyle.SmallFont;
        _lblDemoBadge.Padding = new Padding(10, 6, 10, 6);
        _lblDemoBadge.Margin = new Padding(8, 4, 0, 0);
        _lblDemoBadge.Text = "Dữ liệu minh họa – chưa lưu CSDL";

        _headerLayout.Controls.Add(_lblTitle, 0, 0);
        _headerLayout.Controls.Add(_lblDemoBadge, 1, 0);

        // _filterCardBorder
        _filterCardBorder.Dock = DockStyle.Fill;
        _filterCardBorder.AutoSize = true;
        _filterCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _filterCardBorder.BackColor = UiStyle.Border;
        _filterCardBorder.Padding = new Padding(1);
        _filterCardBorder.Margin = new Padding(0, 0, 0, 12);

        // _filterFlow
        _filterFlow.Dock = DockStyle.Fill;
        _filterFlow.AutoSize = true;
        _filterFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _filterFlow.BackColor = UiStyle.Surface;
        _filterFlow.Padding = new Padding(12, 8, 12, 8);
        _filterFlow.FlowDirection = FlowDirection.LeftToRight;
        _filterFlow.WrapContents = true;
        _filterFlow.Margin = new Padding(0);

        // _grpSubject
        _grpSubject.AutoSize = true;
        _grpSubject.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _grpSubject.FlowDirection = FlowDirection.TopDown;
        _grpSubject.WrapContents = false;
        _grpSubject.Margin = new Padding(0, 0, 12, 4);

        _lblSubjectFilter.AutoSize = true;
        _lblSubjectFilter.Font = UiStyle.SmallFont;
        _lblSubjectFilter.ForeColor = UiStyle.TextMuted;
        _lblSubjectFilter.Margin = new Padding(0, 0, 0, 4);
        _lblSubjectFilter.Text = "Môn học";

        _cboSubject.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSubject.Font = UiStyle.BodyFont;
        _cboSubject.Width = 200;
        _cboSubject.Margin = new Padding(0);
        _cboSubject.TabIndex = 0;
        _cboSubject.SelectedIndexChanged += OnFilterChanged;

        _grpSubject.Controls.Add(_lblSubjectFilter);
        _grpSubject.Controls.Add(_cboSubject);

        // _grpChapter
        _grpChapter.AutoSize = true;
        _grpChapter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _grpChapter.FlowDirection = FlowDirection.TopDown;
        _grpChapter.WrapContents = false;
        _grpChapter.Margin = new Padding(0, 0, 12, 4);

        _lblChapterFilter.AutoSize = true;
        _lblChapterFilter.Font = UiStyle.SmallFont;
        _lblChapterFilter.ForeColor = UiStyle.TextMuted;
        _lblChapterFilter.Margin = new Padding(0, 0, 0, 4);
        _lblChapterFilter.Text = "Chương";

        _cboChapter.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboChapter.Font = UiStyle.BodyFont;
        _cboChapter.Width = 115;
        _cboChapter.Margin = new Padding(0);
        _cboChapter.TabIndex = 1;
        _cboChapter.SelectedIndexChanged += OnFilterChanged;

        _grpChapter.Controls.Add(_lblChapterFilter);
        _grpChapter.Controls.Add(_cboChapter);

        // _grpDifficulty
        _grpDifficulty.AutoSize = true;
        _grpDifficulty.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _grpDifficulty.FlowDirection = FlowDirection.TopDown;
        _grpDifficulty.WrapContents = false;
        _grpDifficulty.Margin = new Padding(0, 0, 12, 4);

        _lblDifficultyFilter.AutoSize = true;
        _lblDifficultyFilter.Font = UiStyle.SmallFont;
        _lblDifficultyFilter.ForeColor = UiStyle.TextMuted;
        _lblDifficultyFilter.Margin = new Padding(0, 0, 0, 4);
        _lblDifficultyFilter.Text = "Độ khó";

        _cboDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboDifficulty.Font = UiStyle.BodyFont;
        _cboDifficulty.Width = 125;
        _cboDifficulty.Margin = new Padding(0);
        _cboDifficulty.TabIndex = 2;
        _cboDifficulty.SelectedIndexChanged += OnFilterChanged;

        _grpDifficulty.Controls.Add(_lblDifficultyFilter);
        _grpDifficulty.Controls.Add(_cboDifficulty);

        // _grpSearch
        _grpSearch.AutoSize = true;
        _grpSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _grpSearch.FlowDirection = FlowDirection.TopDown;
        _grpSearch.WrapContents = false;
        _grpSearch.Margin = new Padding(0, 0, 12, 4);

        _lblSearchFilter.AutoSize = true;
        _lblSearchFilter.Font = UiStyle.SmallFont;
        _lblSearchFilter.ForeColor = UiStyle.TextMuted;
        _lblSearchFilter.Margin = new Padding(0, 0, 0, 4);
        _lblSearchFilter.Text = "Tìm theo nội dung (Ctrl+F)";

        _txtSearch.Font = UiStyle.BodyFont;
        _txtSearch.Width = 220;
        _txtSearch.PlaceholderText = "Nhập từ khóa nội dung…";
        _txtSearch.Margin = new Padding(0);
        _txtSearch.TabIndex = 3;
        _txtSearch.TextChanged += OnFilterChanged;

        _grpSearch.Controls.Add(_lblSearchFilter);
        _grpSearch.Controls.Add(_txtSearch);

        // _grpFilterActions
        _grpFilterActions.AutoSize = true;
        _grpFilterActions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _grpFilterActions.FlowDirection = FlowDirection.LeftToRight;
        _grpFilterActions.WrapContents = false;
        _grpFilterActions.Margin = new Padding(0, 18, 0, 4);

        _btnClearFilter.Width = 96;
        _btnClearFilter.Height = 38;
        _btnClearFilter.Margin = new Padding(0, 0, 12, 0);
        _btnClearFilter.TabIndex = 4;
        _btnClearFilter.Text = "Xóa lọc";
        UiStyle.StyleSecondaryButton(_btnClearFilter);
        _btnClearFilter.Click += OnClearFilterClick;

        _lblCount.AutoSize = true;
        _lblCount.Font = UiStyle.SmallFont;
        _lblCount.ForeColor = UiStyle.TextMuted;
        _lblCount.Margin = new Padding(0, 10, 0, 0);
        _lblCount.Text = "Hiển thị 0 / 0 câu";

        _grpFilterActions.Controls.Add(_btnClearFilter);
        _grpFilterActions.Controls.Add(_lblCount);

        _filterFlow.Controls.Add(_grpSubject);
        _filterFlow.Controls.Add(_grpChapter);
        _filterFlow.Controls.Add(_grpDifficulty);
        _filterFlow.Controls.Add(_grpSearch);
        _filterFlow.Controls.Add(_grpFilterActions);
        _filterCardBorder.Controls.Add(_filterFlow);

        // _gridBorderPanel
        _gridBorderPanel.Dock = DockStyle.Fill;
        _gridBorderPanel.BackColor = UiStyle.Border;
        _gridBorderPanel.Padding = new Padding(1);
        _gridBorderPanel.Margin = new Padding(0, 0, 0, 12);

        // _gridHostPanel
        _gridHostPanel.Dock = DockStyle.Fill;
        _gridHostPanel.BackColor = UiStyle.Surface;

        // _dgvQuestions
        UiStyle.StyleGrid(_dgvQuestions);
        _dgvQuestions.Dock = DockStyle.Fill;
        _dgvQuestions.BorderStyle = BorderStyle.None;
        _dgvQuestions.ShowCellToolTips = true;
        _dgvQuestions.TabIndex = 5;

        _colId.HeaderText = "Mã";
        _colId.Name = "_colId";
        _colId.FillWeight = 9F;
        _colId.MinimumWidth = 60;

        _colSubject.HeaderText = "Môn";
        _colSubject.Name = "_colSubject";
        _colSubject.FillWeight = 20F;
        _colSubject.MinimumWidth = 120;

        _colChapter.HeaderText = "Chương";
        _colChapter.Name = "_colChapter";
        _colChapter.FillWeight = 11F;
        _colChapter.MinimumWidth = 75;

        _colDifficulty.HeaderText = "Độ khó";
        _colDifficulty.Name = "_colDifficulty";
        _colDifficulty.FillWeight = 12F;
        _colDifficulty.MinimumWidth = 85;

        _colContent.HeaderText = "Nội dung";
        _colContent.Name = "_colContent";
        _colContent.FillWeight = 48F;
        _colContent.MinimumWidth = 220;

        _colCorrectOption.HeaderText = "Đáp án đúng";
        _colCorrectOption.Name = "_colCorrectOption";
        _colCorrectOption.FillWeight = 13F;
        _colCorrectOption.MinimumWidth = 95;

        _dgvQuestions.Columns.AddRange(
            _colId,
            _colSubject,
            _colChapter,
            _colDifficulty,
            _colContent,
            _colCorrectOption);

        _dgvQuestions.SelectionChanged += OnGridSelectionChanged;
        _dgvQuestions.CellDoubleClick += OnGridCellDoubleClick;
        _dgvQuestions.KeyDown += OnGridKeyDown;

        // _lblEmptyState
        _lblEmptyState.Dock = DockStyle.Fill;
        _lblEmptyState.BackColor = UiStyle.Surface;
        _lblEmptyState.ForeColor = UiStyle.TextMuted;
        _lblEmptyState.Font = UiStyle.BodyFont;
        _lblEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblEmptyState.Text = "Không có câu hỏi phù hợp. Thử đổi bộ lọc.";
        _lblEmptyState.Visible = false;

        _gridHostPanel.Controls.Add(_lblEmptyState);
        _gridHostPanel.Controls.Add(_dgvQuestions);
        _gridBorderPanel.Controls.Add(_gridHostPanel);

        // _actionsPanel
        _actionsPanel.Dock = DockStyle.Fill;
        _actionsPanel.AutoSize = true;
        _actionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _actionsPanel.FlowDirection = FlowDirection.LeftToRight;
        _actionsPanel.WrapContents = true;
        _actionsPanel.Margin = new Padding(0);

        _btnAdd.Width = 140;
        _btnAdd.Height = 40;
        _btnAdd.Margin = new Padding(0, 0, 12, 0);
        _btnAdd.TabIndex = 6;
        _btnAdd.Text = "Thêm câu hỏi";
        UiStyle.StylePrimaryButton(_btnAdd);
        _btnAdd.Click += OnAddClick;

        _btnEdit.Width = 100;
        _btnEdit.Height = 40;
        _btnEdit.Margin = new Padding(0, 0, 12, 0);
        _btnEdit.TabIndex = 7;
        _btnEdit.Text = "Sửa";
        UiStyle.StyleSecondaryButton(_btnEdit);
        _btnEdit.Click += OnEditClick;

        _btnPreview.Width = 120;
        _btnPreview.Height = 40;
        _btnPreview.Margin = new Padding(0, 0, 12, 0);
        _btnPreview.TabIndex = 8;
        _btnPreview.Text = "Xem trước";
        UiStyle.StyleSecondaryButton(_btnPreview);
        _btnPreview.Click += OnPreviewClick;

        _btnDelete.Width = 100;
        _btnDelete.Height = 40;
        _btnDelete.Margin = new Padding(0);
        _btnDelete.TabIndex = 9;
        _btnDelete.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnDelete);
        _btnDelete.ForeColor = UiStyle.ErrorText;
        _btnDelete.Click += OnDeleteClick;

        _actionsPanel.Controls.Add(_btnAdd);
        _actionsPanel.Controls.Add(_btnEdit);
        _actionsPanel.Controls.Add(_btnPreview);
        _actionsPanel.Controls.Add(_btnDelete);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_filterCardBorder, 0, 1);
        _rootLayout.Controls.Add(_gridBorderPanel, 0, 2);
        _rootLayout.Controls.Add(_actionsPanel, 0, 3);

        // QuestionBankControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "QuestionBankControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _filterCardBorder.ResumeLayout(false);
        _filterCardBorder.PerformLayout();
        _filterFlow.ResumeLayout(false);
        _filterFlow.PerformLayout();
        _grpSubject.ResumeLayout(false);
        _grpSubject.PerformLayout();
        _grpChapter.ResumeLayout(false);
        _grpChapter.PerformLayout();
        _grpDifficulty.ResumeLayout(false);
        _grpDifficulty.PerformLayout();
        _grpSearch.ResumeLayout(false);
        _grpSearch.PerformLayout();
        _grpFilterActions.ResumeLayout(false);
        _grpFilterActions.PerformLayout();
        _gridBorderPanel.ResumeLayout(false);
        _gridHostPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dgvQuestions).EndInit();
        _actionsPanel.ResumeLayout(false);
        _actionsPanel.PerformLayout();
        ResumeLayout(false);
    }
}
