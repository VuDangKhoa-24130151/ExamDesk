#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class ClassStudentControl
{
    private System.ComponentModel.IContainer? components = null;
    private ToolTip _toolTip = null!;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private TableLayoutPanel _splitLayout = null!;

    // Left column: Classes (35%)
    private Panel _classCardBorder = null!;
    private TableLayoutPanel _classCardLayout = null!;
    private TableLayoutPanel _classHeaderRow = null!;
    private Label _lblClassSectionTitle = null!;
    private Label _lblClassCount = null!;
    private Panel _classGridBorder = null!;
    private Panel _classGridHost = null!;
    private DataGridView _dgvClasses = null!;
    private DataGridViewTextBoxColumn _colClassCode = null!;
    private DataGridViewTextBoxColumn _colClassName = null!;
    private DataGridViewTextBoxColumn _colSchoolYear = null!;
    private DataGridViewTextBoxColumn _colStudentCount = null!;
    private Label _lblClassEmptyState = null!;
    private FlowLayoutPanel _classActionsPanel = null!;
    private Button _btnAddClass = null!;
    private Button _btnEditClass = null!;
    private Button _btnDeleteClass = null!;

    // Right column: Students of selected Class (65%)
    private Panel _studentCardBorder = null!;
    private TableLayoutPanel _studentCardLayout = null!;
    private TableLayoutPanel _studentHeaderRow = null!;
    private Label _lblStudentSectionTitle = null!;
    private Label _lblStudentCount = null!;
    private TableLayoutPanel _studentSearchLayout = null!;
    private Label _lblSearchStudent = null!;
    private TextBox _txtSearchStudent = null!;
    private Button _btnClearStudentSearch = null!;
    private Panel _studentGridBorder = null!;
    private Panel _studentGridHost = null!;
    private DataGridView _dgvStudents = null!;
    private DataGridViewTextBoxColumn _colStudentCode = null!;
    private DataGridViewTextBoxColumn _colFullName = null!;
    private DataGridViewTextBoxColumn _colStudentClass = null!;
    private Label _lblStudentEmptyState = null!;
    private FlowLayoutPanel _studentActionsPanel = null!;
    private Button _btnAddStudent = null!;
    private Button _btnEditStudent = null!;
    private Button _btnDeleteStudent = null!;
    private Panel _importButtonHost = null!;
    private Button _btnImportFile = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _toolTip = new ToolTip(components);
        _rootLayout = new TableLayoutPanel();
        _headerLayout = new TableLayoutPanel();
        _lblTitle = new Label();
        _lblDemoBadge = new Label();
        _splitLayout = new TableLayoutPanel();

        _classCardBorder = new Panel();
        _classCardLayout = new TableLayoutPanel();
        _classHeaderRow = new TableLayoutPanel();
        _lblClassSectionTitle = new Label();
        _lblClassCount = new Label();
        _classGridBorder = new Panel();
        _classGridHost = new Panel();
        _dgvClasses = new DataGridView();
        _colClassCode = new DataGridViewTextBoxColumn();
        _colClassName = new DataGridViewTextBoxColumn();
        _colSchoolYear = new DataGridViewTextBoxColumn();
        _colStudentCount = new DataGridViewTextBoxColumn();
        _lblClassEmptyState = new Label();
        _classActionsPanel = new FlowLayoutPanel();
        _btnAddClass = new Button();
        _btnEditClass = new Button();
        _btnDeleteClass = new Button();

        _studentCardBorder = new Panel();
        _studentCardLayout = new TableLayoutPanel();
        _studentHeaderRow = new TableLayoutPanel();
        _lblStudentSectionTitle = new Label();
        _lblStudentCount = new Label();
        _studentSearchLayout = new TableLayoutPanel();
        _lblSearchStudent = new Label();
        _txtSearchStudent = new TextBox();
        _btnClearStudentSearch = new Button();
        _studentGridBorder = new Panel();
        _studentGridHost = new Panel();
        _dgvStudents = new DataGridView();
        _colStudentCode = new DataGridViewTextBoxColumn();
        _colFullName = new DataGridViewTextBoxColumn();
        _colStudentClass = new DataGridViewTextBoxColumn();
        _lblStudentEmptyState = new Label();
        _studentActionsPanel = new FlowLayoutPanel();
        _btnAddStudent = new Button();
        _btnEditStudent = new Button();
        _btnDeleteStudent = new Button();
        _importButtonHost = new Panel();
        _btnImportFile = new Button();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _splitLayout.SuspendLayout();
        _classCardBorder.SuspendLayout();
        _classCardLayout.SuspendLayout();
        _classHeaderRow.SuspendLayout();
        _classGridBorder.SuspendLayout();
        _classGridHost.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvClasses).BeginInit();
        _classActionsPanel.SuspendLayout();
        _studentCardBorder.SuspendLayout();
        _studentCardLayout.SuspendLayout();
        _studentHeaderRow.SuspendLayout();
        _studentSearchLayout.SuspendLayout();
        _studentGridBorder.SuspendLayout();
        _studentGridHost.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dgvStudents).BeginInit();
        _studentActionsPanel.SuspendLayout();
        _importButtonHost.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 2;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: 35/65 Split content

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
        _lblTitle.Text = "Lớp và sinh viên";

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

        // _splitLayout (35% Left / 65% Right)
        _splitLayout.Dock = DockStyle.Fill;
        _splitLayout.Margin = new Padding(0);
        _splitLayout.ColumnCount = 2;
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        _splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        _splitLayout.RowCount = 1;
        _splitLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // ================= LEFT COLUMN: CLASSES (35%) =================
        _classCardBorder.Dock = DockStyle.Fill;
        _classCardBorder.BackColor = UiStyle.Border;
        _classCardBorder.Padding = new Padding(1);
        _classCardBorder.Margin = new Padding(0, 0, 8, 0);

        _classCardLayout.Dock = DockStyle.Fill;
        _classCardLayout.BackColor = UiStyle.Surface;
        _classCardLayout.Padding = new Padding(16);
        _classCardLayout.ColumnCount = 1;
        _classCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _classCardLayout.RowCount = 3;
        _classCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Section title
        _classCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Grid
        _classCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 2: Action buttons

        // _classHeaderRow
        _classHeaderRow.Dock = DockStyle.Fill;
        _classHeaderRow.AutoSize = true;
        _classHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _classHeaderRow.ColumnCount = 2;
        _classHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _classHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _classHeaderRow.RowCount = 1;
        _classHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _classHeaderRow.Margin = new Padding(0, 0, 0, 12);

        _lblClassSectionTitle.AutoSize = true;
        _lblClassSectionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblClassSectionTitle.ForeColor = UiStyle.Text;
        _lblClassSectionTitle.Margin = new Padding(0);
        _lblClassSectionTitle.Text = "Lớp";

        _lblClassCount.AutoSize = true;
        _lblClassCount.Font = UiStyle.SmallFont;
        _lblClassCount.ForeColor = UiStyle.TextMuted;
        _lblClassCount.Margin = new Padding(8, 3, 0, 0);
        _lblClassCount.Text = "0 lớp";

        _classHeaderRow.Controls.Add(_lblClassSectionTitle, 0, 0);
        _classHeaderRow.Controls.Add(_lblClassCount, 1, 0);

        // _classGridBorder
        _classGridBorder.Dock = DockStyle.Fill;
        _classGridBorder.BackColor = UiStyle.Border;
        _classGridBorder.Padding = new Padding(1);
        _classGridBorder.Margin = new Padding(0, 0, 0, 12);

        _classGridHost.Dock = DockStyle.Fill;
        _classGridHost.BackColor = UiStyle.Surface;

        UiStyle.StyleGrid(_dgvClasses);
        _dgvClasses.Dock = DockStyle.Fill;
        _dgvClasses.BorderStyle = BorderStyle.None;
        _dgvClasses.ShowCellToolTips = true;
        _dgvClasses.TabIndex = 0;

        _colClassCode.HeaderText = "Mã lớp";
        _colClassCode.Name = "_colClassCode";
        _colClassCode.FillWeight = 23F;
        _colClassCode.MinimumWidth = 68;

        _colClassName.HeaderText = "Tên lớp";
        _colClassName.Name = "_colClassName";
        _colClassName.FillWeight = 39F;
        _colClassName.MinimumWidth = 95;

        _colSchoolYear.HeaderText = "Niên khóa";
        _colSchoolYear.Name = "_colSchoolYear";
        _colSchoolYear.FillWeight = 23F;
        _colSchoolYear.MinimumWidth = 76;

        _colStudentCount.HeaderText = "Sĩ số";
        _colStudentCount.Name = "_colStudentCount";
        _colStudentCount.FillWeight = 15F;
        _colStudentCount.MinimumWidth = 54;

        _dgvClasses.Columns.AddRange(
            _colClassCode,
            _colClassName,
            _colSchoolYear,
            _colStudentCount);

        _dgvClasses.SelectionChanged += OnClassGridSelectionChanged;
        _dgvClasses.CellDoubleClick += OnClassGridCellDoubleClick;
        _dgvClasses.KeyDown += OnClassGridKeyDown;

        _lblClassEmptyState.Dock = DockStyle.Fill;
        _lblClassEmptyState.BackColor = UiStyle.Surface;
        _lblClassEmptyState.ForeColor = UiStyle.TextMuted;
        _lblClassEmptyState.Font = UiStyle.BodyFont;
        _lblClassEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblClassEmptyState.Text = "Chưa có lớp học nào.\nBấm \"Thêm lớp\" để tạo mới.";
        _lblClassEmptyState.Visible = false;

        _classGridHost.Controls.Add(_lblClassEmptyState);
        _classGridHost.Controls.Add(_dgvClasses);
        _classGridBorder.Controls.Add(_classGridHost);

        // _classActionsPanel
        _classActionsPanel.Dock = DockStyle.Fill;
        _classActionsPanel.AutoSize = true;
        _classActionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _classActionsPanel.FlowDirection = FlowDirection.LeftToRight;
        _classActionsPanel.WrapContents = true;
        _classActionsPanel.Margin = new Padding(0);

        _btnAddClass.Width = 110;
        _btnAddClass.Height = 38;
        _btnAddClass.Margin = new Padding(0, 0, 8, 0);
        _btnAddClass.TabIndex = 1;
        _btnAddClass.Text = "Thêm lớp";
        UiStyle.StylePrimaryButton(_btnAddClass);
        _btnAddClass.Click += OnAddClassClick;

        _btnEditClass.Width = 80;
        _btnEditClass.Height = 38;
        _btnEditClass.Margin = new Padding(0, 0, 8, 0);
        _btnEditClass.TabIndex = 2;
        _btnEditClass.Text = "Sửa";
        UiStyle.StyleSecondaryButton(_btnEditClass);
        _btnEditClass.Click += OnEditClassClick;

        _btnDeleteClass.Width = 80;
        _btnDeleteClass.Height = 38;
        _btnDeleteClass.Margin = new Padding(0);
        _btnDeleteClass.TabIndex = 3;
        _btnDeleteClass.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnDeleteClass);
        _btnDeleteClass.ForeColor = UiStyle.ErrorText;
        _btnDeleteClass.Click += OnDeleteClassClick;

        _classActionsPanel.Controls.Add(_btnAddClass);
        _classActionsPanel.Controls.Add(_btnEditClass);
        _classActionsPanel.Controls.Add(_btnDeleteClass);

        _classCardLayout.Controls.Add(_classHeaderRow, 0, 0);
        _classCardLayout.Controls.Add(_classGridBorder, 0, 1);
        _classCardLayout.Controls.Add(_classActionsPanel, 0, 2);
        _classCardBorder.Controls.Add(_classCardLayout);

        // ================= RIGHT COLUMN: STUDENTS (65%) =================
        _studentCardBorder.Dock = DockStyle.Fill;
        _studentCardBorder.BackColor = UiStyle.Border;
        _studentCardBorder.Padding = new Padding(1);
        _studentCardBorder.Margin = new Padding(8, 0, 0, 0);

        _studentCardLayout.Dock = DockStyle.Fill;
        _studentCardLayout.BackColor = UiStyle.Surface;
        _studentCardLayout.Padding = new Padding(16);
        _studentCardLayout.ColumnCount = 1;
        _studentCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _studentCardLayout.RowCount = 4;
        _studentCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Section title + count
        _studentCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 1: Search bar
        _studentCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 2: Grid
        _studentCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 3: Action buttons

        // _studentHeaderRow
        _studentHeaderRow.Dock = DockStyle.Fill;
        _studentHeaderRow.AutoSize = true;
        _studentHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentHeaderRow.ColumnCount = 2;
        _studentHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _studentHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _studentHeaderRow.RowCount = 1;
        _studentHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _studentHeaderRow.Margin = new Padding(0, 0, 0, 8);

        _lblStudentSectionTitle.AutoSize = false;
        _lblStudentSectionTitle.Dock = DockStyle.Fill;
        _lblStudentSectionTitle.Height = 26;
        _lblStudentSectionTitle.AutoEllipsis = true;
        _lblStudentSectionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblStudentSectionTitle.ForeColor = UiStyle.Text;
        _lblStudentSectionTitle.TextAlign = ContentAlignment.MiddleLeft;
        _lblStudentSectionTitle.Margin = new Padding(0);
        _lblStudentSectionTitle.Text = "Sinh viên lớp (chưa chọn)";

        _lblStudentCount.AutoSize = true;
        _lblStudentCount.Font = UiStyle.SmallFont;
        _lblStudentCount.ForeColor = UiStyle.TextMuted;
        _lblStudentCount.Margin = new Padding(8, 4, 0, 0);
        _lblStudentCount.Text = "Hiển thị 0 / 0 sinh viên";

        _studentHeaderRow.Controls.Add(_lblStudentSectionTitle, 0, 0);
        _studentHeaderRow.Controls.Add(_lblStudentCount, 1, 0);

        // _studentSearchLayout
        _studentSearchLayout.Dock = DockStyle.Fill;
        _studentSearchLayout.AutoSize = true;
        _studentSearchLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentSearchLayout.ColumnCount = 2;
        _studentSearchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _studentSearchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _studentSearchLayout.RowCount = 2;
        _studentSearchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _studentSearchLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _studentSearchLayout.Margin = new Padding(0, 0, 0, 12);

        _lblSearchStudent.AutoSize = true;
        _lblSearchStudent.Font = UiStyle.SmallFont;
        _lblSearchStudent.ForeColor = UiStyle.TextMuted;
        _lblSearchStudent.Margin = new Padding(0, 0, 0, 4);
        _lblSearchStudent.Text = "Tìm theo MSSV hoặc họ và tên (Ctrl+F)";

        _txtSearchStudent.Dock = DockStyle.Fill;
        _txtSearchStudent.Font = UiStyle.BodyFont;
        _txtSearchStudent.PlaceholderText = "Nhập MSSV hoặc họ và tên sinh viên…";
        _txtSearchStudent.Margin = new Padding(0, 4, 8, 0);
        _txtSearchStudent.TabIndex = 4;
        _txtSearchStudent.TextChanged += OnStudentSearchTextChanged;

        _btnClearStudentSearch.Width = 88;
        _btnClearStudentSearch.Height = 34;
        _btnClearStudentSearch.Margin = new Padding(0);
        _btnClearStudentSearch.TabIndex = 5;
        _btnClearStudentSearch.Text = "Xóa lọc";
        UiStyle.StyleSecondaryButton(_btnClearStudentSearch);
        _btnClearStudentSearch.Height = 34;
        _btnClearStudentSearch.Click += OnClearStudentSearchClick;

        _studentSearchLayout.Controls.Add(_lblSearchStudent, 0, 0);
        _studentSearchLayout.SetColumnSpan(_lblSearchStudent, 2);
        _studentSearchLayout.Controls.Add(_txtSearchStudent, 0, 1);
        _studentSearchLayout.Controls.Add(_btnClearStudentSearch, 1, 1);

        // _studentGridBorder
        _studentGridBorder.Dock = DockStyle.Fill;
        _studentGridBorder.BackColor = UiStyle.Border;
        _studentGridBorder.Padding = new Padding(1);
        _studentGridBorder.Margin = new Padding(0, 0, 0, 12);

        _studentGridHost.Dock = DockStyle.Fill;
        _studentGridHost.BackColor = UiStyle.Surface;

        UiStyle.StyleGrid(_dgvStudents);
        _dgvStudents.Dock = DockStyle.Fill;
        _dgvStudents.BorderStyle = BorderStyle.None;
        _dgvStudents.ShowCellToolTips = true;
        _dgvStudents.TabIndex = 6;

        _colStudentCode.HeaderText = "MSSV";
        _colStudentCode.Name = "_colStudentCode";
        _colStudentCode.FillWeight = 24F;
        _colStudentCode.MinimumWidth = 95;

        _colFullName.HeaderText = "Họ và tên";
        _colFullName.Name = "_colFullName";
        _colFullName.FillWeight = 56F;
        _colFullName.MinimumWidth = 180;

        _colStudentClass.HeaderText = "Lớp";
        _colStudentClass.Name = "_colStudentClass";
        _colStudentClass.FillWeight = 20F;
        _colStudentClass.MinimumWidth = 80;

        _dgvStudents.Columns.AddRange(
            _colStudentCode,
            _colFullName,
            _colStudentClass);

        _dgvStudents.SelectionChanged += OnStudentGridSelectionChanged;
        _dgvStudents.CellDoubleClick += OnStudentGridCellDoubleClick;
        _dgvStudents.KeyDown += OnStudentGridKeyDown;

        _lblStudentEmptyState.Dock = DockStyle.Fill;
        _lblStudentEmptyState.BackColor = UiStyle.Surface;
        _lblStudentEmptyState.ForeColor = UiStyle.TextMuted;
        _lblStudentEmptyState.Font = UiStyle.BodyFont;
        _lblStudentEmptyState.TextAlign = ContentAlignment.MiddleCenter;
        _lblStudentEmptyState.Text = "Chọn một lớp để xem danh sách sinh viên";
        _lblStudentEmptyState.Visible = false;

        _studentGridHost.Controls.Add(_lblStudentEmptyState);
        _studentGridHost.Controls.Add(_dgvStudents);
        _studentGridBorder.Controls.Add(_studentGridHost);

        // _studentActionsPanel
        _studentActionsPanel.Dock = DockStyle.Fill;
        _studentActionsPanel.AutoSize = true;
        _studentActionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentActionsPanel.FlowDirection = FlowDirection.LeftToRight;
        _studentActionsPanel.WrapContents = true;
        _studentActionsPanel.Margin = new Padding(0);

        _btnAddStudent.Width = 138;
        _btnAddStudent.Height = 38;
        _btnAddStudent.Margin = new Padding(0, 0, 8, 0);
        _btnAddStudent.TabIndex = 7;
        _btnAddStudent.Text = "Thêm sinh viên";
        UiStyle.StylePrimaryButton(_btnAddStudent);
        _btnAddStudent.Click += OnAddStudentClick;

        _btnEditStudent.Width = 86;
        _btnEditStudent.Height = 38;
        _btnEditStudent.Margin = new Padding(0, 0, 8, 0);
        _btnEditStudent.TabIndex = 8;
        _btnEditStudent.Text = "Sửa";
        UiStyle.StyleSecondaryButton(_btnEditStudent);
        _btnEditStudent.Click += OnEditStudentClick;

        _btnDeleteStudent.Width = 86;
        _btnDeleteStudent.Height = 38;
        _btnDeleteStudent.Margin = new Padding(0, 0, 8, 0);
        _btnDeleteStudent.TabIndex = 9;
        _btnDeleteStudent.Text = "Xóa";
        UiStyle.StyleSecondaryButton(_btnDeleteStudent);
        _btnDeleteStudent.ForeColor = UiStyle.ErrorText;
        _btnDeleteStudent.Click += OnDeleteStudentClick;

        // Wrap disabled _btnImportFile in _importButtonHost so tooltip shows reliably on hover
        _importButtonHost.AutoSize = true;
        _importButtonHost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _importButtonHost.Margin = new Padding(0);
        _importButtonHost.Padding = new Padding(0);

        _btnImportFile.Width = 128;
        _btnImportFile.Height = 38;
        _btnImportFile.Margin = new Padding(0);
        _btnImportFile.TabIndex = 10;
        _btnImportFile.Text = "Nhập từ tệp…";
        UiStyle.StyleSecondaryButton(_btnImportFile);
        _btnImportFile.Enabled = false;
        _btnImportFile.ForeColor = UiStyle.TextMuted;

        const string importTooltipText = "Nhập tệp ở màn hình Dữ liệu và sao lưu – Bạn 2 phụ trách";
        _toolTip.ShowAlways = true;
        _toolTip.SetToolTip(_btnImportFile, importTooltipText);
        _toolTip.SetToolTip(_importButtonHost, importTooltipText);

        _importButtonHost.Controls.Add(_btnImportFile);

        _studentActionsPanel.Controls.Add(_btnAddStudent);
        _studentActionsPanel.Controls.Add(_btnEditStudent);
        _studentActionsPanel.Controls.Add(_btnDeleteStudent);
        _studentActionsPanel.Controls.Add(_importButtonHost);

        _studentCardLayout.Controls.Add(_studentHeaderRow, 0, 0);
        _studentCardLayout.Controls.Add(_studentSearchLayout, 0, 1);
        _studentCardLayout.Controls.Add(_studentGridBorder, 0, 2);
        _studentCardLayout.Controls.Add(_studentActionsPanel, 0, 3);
        _studentCardBorder.Controls.Add(_studentCardLayout);

        _splitLayout.Controls.Add(_classCardBorder, 0, 0);
        _splitLayout.Controls.Add(_studentCardBorder, 1, 0);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_splitLayout, 0, 1);

        // ClassStudentControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_rootLayout);
        Name = "ClassStudentControl";
        Size = new Size(1000, 600);

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _splitLayout.ResumeLayout(false);
        _classCardBorder.ResumeLayout(false);
        _classCardLayout.ResumeLayout(false);
        _classCardLayout.PerformLayout();
        _classHeaderRow.ResumeLayout(false);
        _classHeaderRow.PerformLayout();
        _classGridBorder.ResumeLayout(false);
        _classGridHost.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dgvClasses).EndInit();
        _classActionsPanel.ResumeLayout(false);
        _classActionsPanel.PerformLayout();
        _studentCardBorder.ResumeLayout(false);
        _studentCardLayout.ResumeLayout(false);
        _studentCardLayout.PerformLayout();
        _studentHeaderRow.ResumeLayout(false);
        _studentHeaderRow.PerformLayout();
        _studentSearchLayout.ResumeLayout(false);
        _studentSearchLayout.PerformLayout();
        _studentGridBorder.ResumeLayout(false);
        _studentGridHost.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dgvStudents).EndInit();
        _studentActionsPanel.ResumeLayout(false);
        _studentActionsPanel.PerformLayout();
        _importButtonHost.ResumeLayout(false);
        ResumeLayout(false);
    }
}
