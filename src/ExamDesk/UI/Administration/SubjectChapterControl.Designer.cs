#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class SubjectChapterControl
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private TableLayoutPanel _splitLayout = null!;

    // Left column: Subjects
    private Panel _subjectCardBorder = null!;
    private TableLayoutPanel _subjectCardLayout = null!;
    private TableLayoutPanel _subjectHeaderRow = null!;
    private Label _lblSubjectSectionTitle = null!;
    private Label _lblSubjectCount = null!;
    private TableLayoutPanel _subjectSearchLayout = null!;
    private Label _lblSearchSubject = null!;
    private TextBox _txtSearchSubject = null!;
    private Button _btnClearSubjectSearch = null!;
    private Panel _subjectGridBorder = null!;
    private Panel _subjectGridHost = null!;
    private DataGridView _dgvSubjects = null!;
    private DataGridViewTextBoxColumn _colSubjectCode = null!;
    private DataGridViewTextBoxColumn _colSubjectName = null!;
    private DataGridViewTextBoxColumn _colSubjectChapterCount = null!;
    private DataGridViewTextBoxColumn _colSubjectQuestionCount = null!;
    private Label _lblSubjectEmptyState = null!;
    private FlowLayoutPanel _subjectActionsPanel = null!;
    private Button _btnAddSubject = null!;
    private Button _btnEditSubject = null!;
    private Button _btnDeleteSubject = null!;

    // Right column: Chapters of selected Subject
    private Panel _chapterCardBorder = null!;
    private TableLayoutPanel _chapterCardLayout = null!;
    private TableLayoutPanel _chapterHeaderRow = null!;
    private Label _lblChapterSectionTitle = null!;
    private Label _lblChapterCount = null!;
    private Panel _chapterGridBorder = null!;
    private Panel _chapterGridHost = null!;
    private DataGridView _dgvChapters = null!;
    private DataGridViewTextBoxColumn _colChapterNumber = null!;
    private DataGridViewTextBoxColumn _colChapterName = null!;
    private DataGridViewTextBoxColumn _colChapterQuestionCount = null!;
    private Label _lblChapterEmptyState = null!;
    private FlowLayoutPanel _chapterActionsPanel = null!;
    private Button _btnAddChapter = null!;
    private Button _btnEditChapter = null!;
    private Button _btnDeleteChapter = null!;

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
        _splitLayout = new TableLayoutPanel();

        _subjectCardBorder = new Panel();
        _subjectCardLayout = new TableLayoutPanel();
        _subjectHeaderRow = new TableLayoutPanel();
        _lblSubjectSectionTitle = new Label();
        _lblSubjectCount = new Label();
        _subjectSearchLayout = new TableLayoutPanel();
        _lblSearchSubject = new Label();
        _txtSearchSubject = new TextBox();
        _btnClearSubjectSearch = new Button();
        _subjectGridBorder = new Panel();
        _subjectGridHost = new Panel();
        _dgvSubjects = new DataGridView();
        _colSubjectCode = new DataGridViewTextBoxColumn();
        _colSubjectName = new DataGridViewTextBoxColumn();
        _colSubjectChapterCount = new DataGridViewTextBoxColumn();
        _colSubjectQuestionCount = new DataGridViewTextBoxColumn();
        _lblSubjectEmptyState = new Label();
        _subjectActionsPanel = new FlowLayoutPanel();
        _btnAddSubject = new Button();
        _btnEditSubject = new Button();
        _btnDeleteSubject = new Button();

        _chapterCardBorder = new Panel();
        _chapterCardLayout = new TableLayoutPanel();
        _chapterHeaderRow = new TableLayoutPanel();
        _lblChapterSectionTitle = new Label();
        _lblChapterCount = new Label();
        _chapterGridBorder = new Panel();
        _chapterGridHost = new Panel();
        _dgvChapters = new DataGridView();
        _colChapterNumber = new DataGridViewTextBoxColumn();
        _colChapterName = new DataGridViewTextBoxColumn();
        _colChapterQuestionCount = new DataGridViewTextBoxColumn();
        _lblChapterEmptyState = new Label();
        _chapterActionsPanel = new FlowLayoutPanel();
        _btnAddChapter = new Button();
        _btnEditChapter = new Button();
        _btnDeleteChapter = new Button();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _splitLayout.SuspendLayout();
        _subjectCardBorder.SuspendLayout();
        _subjectCardLayout.SuspendLayout();
        _subjectHeaderRow.SuspendLayout();
        _subjectSearchLayout.SuspendLayout();
        _subjectGridBorder.SuspendLayout();
        _subjectGridHost.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvSubjects).BeginInit();
        _subjectActionsPanel.SuspendLayout();
        _chapterCardBorder.SuspendLayout();
        _chapterCardLayout.SuspendLayout();
        _chapterHeaderRow.SuspendLayout();
        _chapterGridBorder.SuspendLayout();
        _chapterGridHost.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvChapters).BeginInit();
        _chapterActionsPanel.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 2;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: 40/60 Split content

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
        _lblTitle.Text = "Môn và chương";

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

        // _splitLayout (40% Left / 60% Right)
        _splitLayout.Dock = DockStyle.Fill;
        _splitLayout.Margin = new Padding(0);
        _splitLayout.ColumnCount = 2;
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        _splitLayout.RowCount = 1;
        _splitLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // ================= LEFT COLUMN: SUBJECTS =================
        _subjectCardBorder.Dock = DockStyle.Fill;
        _subjectCardBorder.BackColor = UiStyle.Border;
        _subjectCardBorder.Padding = new Padding(1);
        _subjectCardBorder.Margin = new Padding(0, 0, 8, 0);

        _subjectCardLayout.Dock = DockStyle.Fill;
        _subjectCardLayout.BackColor = UiStyle.Surface;
        _subjectCardLayout.Padding = new Padding(16);
        _subjectCardLayout.ColumnCount = 1;
        _subjectCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _subjectCardLayout.RowCount = 4;
        _subjectCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Section title
        _subjectCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 1: Search bar
        _subjectCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 2: Grid
        _subjectCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 3: Action buttons

        // _subjectHeaderRow
        _subjectHeaderRow.Dock = DockStyle.Fill;
        _subjectHeaderRow.AutoSize = true;
        _subjectHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _subjectHeaderRow.ColumnCount = 2;
        _subjectHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _subjectHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _subjectHeaderRow.RowCount = 1;
        _subjectHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _subjectHeaderRow.Margin = new Padding(0, 0, 0, 8);

        _lblSubjectSectionTitle.AutoSize = true;
        _lblSubjectSectionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblSubjectSectionTitle.ForeColor = UiStyle.Text;
        _lblSubjectSectionTitle.Margin = new Padding(0);
        _lblSubjectSectionTitle.Text = "Môn học";

        _lblSubjectCount.AutoSize = true;
        _lblSubjectCount.Font = UiStyle.SmallFont;
        _lblSubjectCount.ForeColor = UiStyle.TextMuted;
        _lblSubjectCount.Margin = new Padding(8, 3, 0, 0);
        _lblSubjectCount.Text = "0 môn";

        _subjectHeaderRow.Controls.Add(_lblSubjectSectionTitle, 0, 0);
        _subjectHeaderRow.Controls.Add(_lblSubjectCount, 1, 0);

        // _subjectSearchLayout
        _subjectSearchLayout.Dock = DockStyle.Fill;
        _subjectSearchLayout.AutoSize = true;
        _subjectSearchLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _subjectSearchLayout.ColumnCount = 2;
        _subjectSearchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _subjectSearchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _subjectSearchLayout.RowCount = 2;
        _subjectSearchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _subjectSearchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _subjectSearchLayout.Margin = new Padding(0, 0, 0, 12);

        _lblSearchSubject.AutoSize = true;
        _lblSearchSubject.Font = UiStyle.SmallFont;
        _lblSearchSubject.ForeColor = UiStyle.TextMuted;
        _lblSearchSubject.Margin = new Padding(0, 0, 0, 4);
        _lblSearchSubject.Text = "Tìm theo mã hoặc tên môn (Ctrl+F)";

        _txtSearchSubject.Dock = DockStyle.Fill;
        _txtSearchSubject.Font = UiStyle.BodyFont;
        _txtSearchSubject.PlaceholderText = "Nhập mã hoặc tên môn…";
        _txtSearchSubject.Margin = new Padding(0, 4, 8, 0);
        _txtSearchSubject.TabIndex = 0;
        _txtSearchSubject.TextChanged += OnSubjectSearchTextChanged;

        _btnClearSubjectSearch.Width = 84;
        _btnClearSubjectSearch.Height = 34;
        _btnClearSubjectSearch.Margin = new Padding(0);
        _btnClearSubjectSearch.TabIndex = 1;
        _btnClearSubjectSearch.Text = "Xóa lọc";
        UiStyle.StyleSecondaryButton(_btnClearSubjectSearch);
        _btnClearSubjectSearch.Height = 34;
        _btnClearSubjectSearch.Click += OnClearSubjectSearchClick;

        _subjectSearchLayout.Controls.Add(_lblSearchSubject, 0, 0);
        _subjectSearchLayout.SetColumnSpan(_lblSearchSubject, 2);
        _subjectSearchLayout.Controls.Add(_txtSearchSubject, 0, 1);
        _subjectSearchLayout.Controls.Add(_btnClearSubjectSearch, 1, 1);

        // _subjectGridBorder
        _subjectGridBorder.Dock = DockStyle.Fill;
        _subjectGridBorder.BackColor = UiStyle.Border;
        _subjectGridBorder.Padding = new Padding(1);
        _subjectGridBorder.Margin = new Padding(0, 0, 0, 12);

        _subjectGridHost.Dock = DockStyle.Fill;
        _subjectGridHost.BackColor = UiStyle.Surface;

        UiStyle.StyleGrid(_dgvSubjects);
        _dgvSubjects.Dock = DockStyle.Fill;
        _dgvSubjects.BorderStyle = BorderStyle.None;
        _dgvSubjects.ShowCellToolTips = true;
        _dgvSubjects.TabIndex = 2;

        _colSubjectCode.HeaderText = "Mã môn";
        _colSubjectCode.Name = "_colSubjectCode";
        _colSubjectCode.FillWeight = 22F;
        _colSubjectCode.MinimumWidth = 70;

        _colSubjectName.HeaderText = "Tên môn";
        _colSubjectName.Name = "_colSubjectName";
        _colSubjectName.FillWeight = 42F;
        _colSubjectName.MinimumWidth = 110;

        _colSubjectChapterCount.HeaderText = "Số chương";
        _colSubjectChapterCount.Name = "_colSubjectChapterCount";
        _colSubjectChapterCount.FillWeight = 18F;
        _colSubjectChapterCount.MinimumWidth = 72;

        _colSubjectQuestionCount.HeaderText = "Số câu hỏi";
        _colSubjectQuestionCount.Name = "_colSubjectQuestionCount";
        _colSubjectQuestionCount.FillWeight = 18F;
        _colSubjectQuestionCount.MinimumWidth = 72;

        _dgvSubjects.Columns.AddRange(
            _colSubjectCode,
            _colSubjectName,
            _colSubjectChapterCount,
            _colSubjectQuestionCount);

        _dgvSubjects.SelectionChanged += OnSubjectGridSelectionChanged;
        _dgvSubjects.CellDoubleClick += OnSubjectGridCellDoubleClick;
        _dgvSubjects.KeyDown += OnSubjectGridKeyDown;

        _lblSubjectEmptyState.Dock = DockStyle.Fill;
        _lblSubjectEmptyState.BackColor = UiStyle.Surface;
        _lblSubjectEmptyState.ForeColor = UiStyle.TextMuted;
        _lblSubjectEmptyState.Font = UiStyle.BodyFont;
        _lblSubjectEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblSubjectEmptyState.Text = "Không có môn học phù hợp.";
        _lblSubjectEmptyState.Visible = false;

        _subjectGridHost.Controls.Add(_lblSubjectEmptyState);
        _subjectGridHost.Controls.Add(_dgvSubjects);
        _subjectGridBorder.Controls.Add(_subjectGridHost);

        // _subjectActionsPanel
        _subjectActionsPanel.Dock = DockStyle.Fill;
        _subjectActionsPanel.AutoSize = true;
        _subjectActionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _subjectActionsPanel.FlowDirection = FlowDirection.LeftToRight;
        _subjectActionsPanel.WrapContents = true;
        _subjectActionsPanel.Margin = new Padding(0);

        _btnAddSubject.Width = 116;
        _btnAddSubject.Height = 38;
        _btnAddSubject.Margin = new Padding(0, 0, 8, 0);
        _btnAddSubject.TabIndex = 3;
        _btnAddSubject.Text = "Thêm môn";
        UiStyle.StylePrimaryButton(_btnAddSubject);
        _btnAddSubject.Click += OnAddSubjectClick;

        _btnEditSubject.Width = 86;
        _btnEditSubject.Height = 38;
        _btnEditSubject.Margin = new Padding(0, 0, 8, 0);
        _btnEditSubject.TabIndex = 4;
        _btnEditSubject.Text = "Sửa";
        UiStyle.StyleSecondaryButton(_btnEditSubject);
        _btnEditSubject.Click += OnEditSubjectClick;

        _btnDeleteSubject.Width = 86;
        _btnDeleteSubject.Height = 38;
        _btnDeleteSubject.Margin = new Padding(0);
        _btnDeleteSubject.TabIndex = 5;
        _btnDeleteSubject.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnDeleteSubject);
        _btnDeleteSubject.ForeColor = UiStyle.ErrorText;
        _btnDeleteSubject.Click += OnDeleteSubjectClick;

        _subjectActionsPanel.Controls.Add(_btnAddSubject);
        _subjectActionsPanel.Controls.Add(_btnEditSubject);
        _subjectActionsPanel.Controls.Add(_btnDeleteSubject);

        _subjectCardLayout.Controls.Add(_subjectHeaderRow, 0, 0);
        _subjectCardLayout.Controls.Add(_subjectSearchLayout, 0, 1);
        _subjectCardLayout.Controls.Add(_subjectGridBorder, 0, 2);
        _subjectCardLayout.Controls.Add(_subjectActionsPanel, 0, 3);
        _subjectCardBorder.Controls.Add(_subjectCardLayout);

        // ================= RIGHT COLUMN: CHAPTERS =================
        _chapterCardBorder.Dock = DockStyle.Fill;
        _chapterCardBorder.BackColor = UiStyle.Border;
        _chapterCardBorder.Padding = new Padding(1);
        _chapterCardBorder.Margin = new Padding(8, 0, 0, 0);

        _chapterCardLayout.Dock = DockStyle.Fill;
        _chapterCardLayout.BackColor = UiStyle.Surface;
        _chapterCardLayout.Padding = new Padding(16);
        _chapterCardLayout.ColumnCount = 1;
        _chapterCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _chapterCardLayout.RowCount = 3;
        _chapterCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Section title
        _chapterCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Grid
        _chapterCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 2: Action buttons

        // _chapterHeaderRow
        _chapterHeaderRow.Dock = DockStyle.Fill;
        _chapterHeaderRow.AutoSize = true;
        _chapterHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _chapterHeaderRow.ColumnCount = 2;
        _chapterHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _chapterHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _chapterHeaderRow.RowCount = 1;
        _chapterHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _chapterHeaderRow.Margin = new Padding(0, 0, 0, 12);

        _lblChapterSectionTitle.AutoSize = false;
        _lblChapterSectionTitle.Dock = DockStyle.Fill;
        _lblChapterSectionTitle.Height = 26;
        _lblChapterSectionTitle.AutoEllipsis = true;
        _lblChapterSectionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblChapterSectionTitle.ForeColor = UiStyle.Text;
        _lblChapterSectionTitle.TextAlign = ContentAlignment.MiddleLeft;
        _lblChapterSectionTitle.Margin = new Padding(0);
        _lblChapterSectionTitle.Text = "Chương của môn: (chưa chọn)";

        _lblChapterCount.AutoSize = true;
        _lblChapterCount.Font = UiStyle.SmallFont;
        _lblChapterCount.ForeColor = UiStyle.TextMuted;
        _lblChapterCount.Margin = new Padding(8, 4, 0, 0);
        _lblChapterCount.Text = "0 chương";

        _chapterHeaderRow.Controls.Add(_lblChapterSectionTitle, 0, 0);
        _chapterHeaderRow.Controls.Add(_lblChapterCount, 1, 0);

        // _chapterGridBorder
        _chapterGridBorder.Dock = DockStyle.Fill;
        _chapterGridBorder.BackColor = UiStyle.Border;
        _chapterGridBorder.Padding = new Padding(1);
        _chapterGridBorder.Margin = new Padding(0, 0, 0, 12);

        _chapterGridHost.Dock = DockStyle.Fill;
        _chapterGridHost.BackColor = UiStyle.Surface;

        UiStyle.StyleGrid(_dgvChapters);
        _dgvChapters.Dock = DockStyle.Fill;
        _dgvChapters.BorderStyle = BorderStyle.None;
        _dgvChapters.ShowCellToolTips = true;
        _dgvChapters.TabIndex = 6;

        _colChapterNumber.HeaderText = "Chương số";
        _colChapterNumber.Name = "_colChapterNumber";
        _colChapterNumber.FillWeight = 20F;
        _colChapterNumber.MinimumWidth = 85;

        _colChapterName.HeaderText = "Tên chương";
        _colChapterName.Name = "_colChapterName";
        _colChapterName.FillWeight = 58F;
        _colChapterName.MinimumWidth = 180;

        _colChapterQuestionCount.HeaderText = "Số câu hỏi";
        _colChapterQuestionCount.Name = "_colChapterQuestionCount";
        _colChapterQuestionCount.FillWeight = 22F;
        _colChapterQuestionCount.MinimumWidth = 90;

        _dgvChapters.Columns.AddRange(
            _colChapterNumber,
            _colChapterName,
            _colChapterQuestionCount);

        _dgvChapters.SelectionChanged += OnChapterGridSelectionChanged;
        _dgvChapters.CellDoubleClick += OnChapterGridCellDoubleClick;
        _dgvChapters.KeyDown += OnChapterGridKeyDown;

        _lblChapterEmptyState.Dock = DockStyle.Fill;
        _lblChapterEmptyState.BackColor = UiStyle.Surface;
        _lblChapterEmptyState.ForeColor = UiStyle.TextMuted;
        _lblChapterEmptyState.Font = UiStyle.BodyFont;
        _lblChapterEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblChapterEmptyState.Text = "Chọn một môn để xem chương";
        _lblChapterEmptyState.Visible = false;

        _chapterGridHost.Controls.Add(_lblChapterEmptyState);
        _chapterGridHost.Controls.Add(_dgvChapters);
        _chapterGridBorder.Controls.Add(_chapterGridHost);

        // _chapterActionsPanel
        _chapterActionsPanel.Dock = DockStyle.Fill;
        _chapterActionsPanel.AutoSize = true;
        _chapterActionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _chapterActionsPanel.FlowDirection = FlowDirection.LeftToRight;
        _chapterActionsPanel.WrapContents = true;
        _chapterActionsPanel.Margin = new Padding(0);

        _btnAddChapter.Width = 132;
        _btnAddChapter.Height = 38;
        _btnAddChapter.Margin = new Padding(0, 0, 8, 0);
        _btnAddChapter.TabIndex = 7;
        _btnAddChapter.Text = "Thêm chương";
        UiStyle.StylePrimaryButton(_btnAddChapter);
        _btnAddChapter.Click += OnAddChapterClick;

        _btnEditChapter.Width = 86;
        _btnEditChapter.Height = 38;
        _btnEditChapter.Margin = new Padding(0, 0, 8, 0);
        _btnEditChapter.TabIndex = 8;
        _btnEditChapter.Text = "Sửa";
        UiStyle.StyleSecondaryButton(_btnEditChapter);
        _btnEditChapter.Click += OnEditChapterClick;

        _btnDeleteChapter.Width = 86;
        _btnDeleteChapter.Height = 38;
        _btnDeleteChapter.Margin = new Padding(0);
        _btnDeleteChapter.TabIndex = 9;
        _btnDeleteChapter.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnDeleteChapter);
        _btnDeleteChapter.ForeColor = UiStyle.ErrorText;
        _btnDeleteChapter.Click += OnDeleteChapterClick;

        _chapterActionsPanel.Controls.Add(_btnAddChapter);
        _chapterActionsPanel.Controls.Add(_btnEditChapter);
        _chapterActionsPanel.Controls.Add(_btnDeleteChapter);

        _chapterCardLayout.Controls.Add(_chapterHeaderRow, 0, 0);
        _chapterCardLayout.Controls.Add(_chapterGridBorder, 0, 1);
        _chapterCardLayout.Controls.Add(_chapterActionsPanel, 0, 2);
        _chapterCardBorder.Controls.Add(_chapterCardLayout);

        _splitLayout.Controls.Add(_subjectCardBorder, 0, 0);
        _splitLayout.Controls.Add(_chapterCardBorder, 1, 0);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_splitLayout, 0, 1);

        // SubjectChapterControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "SubjectChapterControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _splitLayout.ResumeLayout(false);
        _subjectCardBorder.ResumeLayout(false);
        _subjectCardLayout.ResumeLayout(false);
        _subjectCardLayout.PerformLayout();
        _subjectHeaderRow.ResumeLayout(false);
        _subjectHeaderRow.PerformLayout();
        _subjectSearchLayout.ResumeLayout(false);
        _subjectSearchLayout.PerformLayout();
        _subjectGridBorder.ResumeLayout(false);
        _subjectGridHost.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dgvSubjects).EndInit();
        _subjectActionsPanel.ResumeLayout(false);
        _subjectActionsPanel.PerformLayout();
        _chapterCardBorder.ResumeLayout(false);
        _chapterCardLayout.ResumeLayout(false);
        _chapterCardLayout.PerformLayout();
        _chapterHeaderRow.ResumeLayout(false);
        _chapterHeaderRow.PerformLayout();
        _chapterGridBorder.ResumeLayout(false);
        _chapterGridHost.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dgvChapters).EndInit();
        _chapterActionsPanel.ResumeLayout(false);
        _chapterActionsPanel.PerformLayout();
        ResumeLayout(false);
    }
}
