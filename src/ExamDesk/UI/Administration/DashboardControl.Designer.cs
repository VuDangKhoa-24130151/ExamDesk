#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class DashboardControl
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _scrollPanel = null!;
    private TableLayoutPanel _rootLayout = null!;

    // 1. Header
    private TableLayoutPanel _headerLayout = null!;
    private FlowLayoutPanel _titleGroupFlow = null!;
    private Label _lblTitle = null!;
    private Label _lblGreeting = null!;
    private Label _lblDate = null!;
    private Label _lblDemoBadge = null!;

    // 2. Metric cards (5 cards)
    private FlowLayoutPanel _metricsFlow = null!;
    private Panel _cardSubjectsBorder = null!;
    private Label _lblSubjectsValue = null!;
    private Label _lblSubjectsLabel = null!;
    private Panel _cardChaptersBorder = null!;
    private Label _lblChaptersValue = null!;
    private Label _lblChaptersLabel = null!;
    private Panel _cardQuestionsBorder = null!;
    private Label _lblQuestionsValue = null!;
    private Label _lblQuestionsLabel = null!;
    private Panel _cardClassesBorder = null!;
    private Label _lblClassesValue = null!;
    private Label _lblClassesLabel = null!;
    private Panel _cardStudentsBorder = null!;
    private Label _lblStudentsValue = null!;
    private Label _lblStudentsLabel = null!;

    // 3. Middle section: Difficulty distribution (Left) + Attention warnings (Right)
    private TableLayoutPanel _middleLayout = null!;

    private Panel _difficultyCardBorder = null!;
    private TableLayoutPanel _difficultyCardLayout = null!;
    private TableLayoutPanel _difficultyHeaderRow = null!;
    private Label _lblDifficultyTitle = null!;
    private Label _lblDifficultyTotal = null!;
    private TableLayoutPanel _difficultyBarsTable = null!;
    private Label _lblEasyName = null!;
    private Panel _barEasy = null!;
    private Label _lblEasyStat = null!;
    private Label _lblMediumName = null!;
    private Panel _barMedium = null!;
    private Label _lblMediumStat = null!;
    private Label _lblHardName = null!;
    private Panel _barHard = null!;
    private Label _lblHardStat = null!;

    private Panel _attentionCardBorder = null!;
    private TableLayoutPanel _attentionCardLayout = null!;
    private TableLayoutPanel _attentionHeaderRow = null!;
    private Label _lblAttentionTitle = null!;
    private Label _lblAttentionCount = null!;
    private FlowLayoutPanel _attentionListFlow = null!;

    // 4. Quick actions
    private Panel _quickActionsCardBorder = null!;
    private TableLayoutPanel _quickActionsCardLayout = null!;
    private Label _lblQuickActionsTitle = null!;
    private FlowLayoutPanel _quickActionsFlow = null!;
    private Button _btnQuickAddQuestion = null!;
    private Button _btnQuickSubjects = null!;
    private Button _btnQuickClasses = null!;
    private Button _btnQuickExams = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _scrollPanel = new Panel();
        _rootLayout = new TableLayoutPanel();
        _headerLayout = new TableLayoutPanel();
        _titleGroupFlow = new FlowLayoutPanel();
        _lblTitle = new Label();
        _lblGreeting = new Label();
        _lblDate = new Label();
        _lblDemoBadge = new Label();

        _metricsFlow = new FlowLayoutPanel();
        _cardSubjectsBorder = new Panel();
        _lblSubjectsValue = new Label();
        _lblSubjectsLabel = new Label();
        _cardChaptersBorder = new Panel();
        _lblChaptersValue = new Label();
        _lblChaptersLabel = new Label();
        _cardQuestionsBorder = new Panel();
        _lblQuestionsValue = new Label();
        _lblQuestionsLabel = new Label();
        _cardClassesBorder = new Panel();
        _lblClassesValue = new Label();
        _lblClassesLabel = new Label();
        _cardStudentsBorder = new Panel();
        _lblStudentsValue = new Label();
        _lblStudentsLabel = new Label();

        _middleLayout = new TableLayoutPanel();
        _difficultyCardBorder = new Panel();
        _difficultyCardLayout = new TableLayoutPanel();
        _difficultyHeaderRow = new TableLayoutPanel();
        _lblDifficultyTitle = new Label();
        _lblDifficultyTotal = new Label();
        _difficultyBarsTable = new TableLayoutPanel();
        _lblEasyName = new Label();
        _barEasy = new Panel();
        _lblEasyStat = new Label();
        _lblMediumName = new Label();
        _barMedium = new Panel();
        _lblMediumStat = new Label();
        _lblHardName = new Label();
        _barHard = new Panel();
        _lblHardStat = new Label();

        _attentionCardBorder = new Panel();
        _attentionCardLayout = new TableLayoutPanel();
        _attentionHeaderRow = new TableLayoutPanel();
        _lblAttentionTitle = new Label();
        _lblAttentionCount = new Label();
        _attentionListFlow = new FlowLayoutPanel();

        _quickActionsCardBorder = new Panel();
        _quickActionsCardLayout = new TableLayoutPanel();
        _lblQuickActionsTitle = new Label();
        _quickActionsFlow = new FlowLayoutPanel();
        _btnQuickAddQuestion = new Button();
        _btnQuickSubjects = new Button();
        _btnQuickClasses = new Button();
        _btnQuickExams = new Button();

        _scrollPanel.SuspendLayout();
        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _titleGroupFlow.SuspendLayout();
        _metricsFlow.SuspendLayout();
        _middleLayout.SuspendLayout();
        _difficultyCardBorder.SuspendLayout();
        _difficultyCardLayout.SuspendLayout();
        _difficultyHeaderRow.SuspendLayout();
        _difficultyBarsTable.SuspendLayout();
        _attentionCardBorder.SuspendLayout();
        _attentionCardLayout.SuspendLayout();
        _attentionHeaderRow.SuspendLayout();
        _quickActionsCardBorder.SuspendLayout();
        _quickActionsCardLayout.SuspendLayout();
        _quickActionsFlow.SuspendLayout();
        SuspendLayout();

        // _scrollPanel
        _scrollPanel.Dock = DockStyle.Fill;
        _scrollPanel.AutoScroll = true;
        _scrollPanel.BackColor = UiStyle.Background;
        _scrollPanel.Padding = new Padding(0);

        // _rootLayout
        _rootLayout.Dock = DockStyle.Top;
        _rootLayout.AutoSize = true;
        _rootLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 4;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Metric cards
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: Difficulty + Attention
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3: Quick actions

        // ================= 1. HEADER =================
        _headerLayout.Dock = DockStyle.Fill;
        _headerLayout.AutoSize = true;
        _headerLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _headerLayout.ColumnCount = 2;
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _headerLayout.RowCount = 1;
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _headerLayout.Margin = new Padding(0, 0, 0, 16);

        _titleGroupFlow.Dock = DockStyle.Fill;
        _titleGroupFlow.AutoSize = true;
        _titleGroupFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _titleGroupFlow.FlowDirection = FlowDirection.TopDown;
        _titleGroupFlow.WrapContents = false;
        _titleGroupFlow.Margin = new Padding(0);

        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 4);
        _lblTitle.Text = "Tổng quan";

        _lblGreeting.AutoSize = true;
        _lblGreeting.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblGreeting.ForeColor = UiStyle.Primary;
        _lblGreeting.Margin = new Padding(0, 0, 0, 2);
        _lblGreeting.Text = "Xin chào, Giảng viên";

        _lblDate.AutoSize = true;
        _lblDate.Font = UiStyle.SmallFont;
        _lblDate.ForeColor = UiStyle.TextMuted;
        _lblDate.Margin = new Padding(0);
        _lblDate.Text = "Thứ Hai, 02/11/2026";

        _titleGroupFlow.Controls.Add(_lblTitle);
        _titleGroupFlow.Controls.Add(_lblGreeting);
        _titleGroupFlow.Controls.Add(_lblDate);

        _lblDemoBadge.AutoSize = true;
        _lblDemoBadge.BackColor = UiStyle.WarningBack;
        _lblDemoBadge.ForeColor = UiStyle.WarningText;
        _lblDemoBadge.Font = UiStyle.SmallFont;
        _lblDemoBadge.Padding = new Padding(10, 6, 10, 6);
        _lblDemoBadge.Margin = new Padding(8, 4, 0, 0);
        _lblDemoBadge.Text = "Số liệu minh họa";

        _headerLayout.Controls.Add(_titleGroupFlow, 0, 0);
        _headerLayout.Controls.Add(_lblDemoBadge, 1, 0);

        // ================= 2. METRIC CARDS =================
        _metricsFlow.Dock = DockStyle.Fill;
        _metricsFlow.AutoSize = true;
        _metricsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _metricsFlow.FlowDirection = FlowDirection.LeftToRight;
        _metricsFlow.WrapContents = true;
        _metricsFlow.Margin = new Padding(0, 0, 0, 16);

        ConfigureMetricCard(_cardSubjectsBorder, _lblSubjectsValue, _lblSubjectsLabel, "Môn học", "subjects");
        ConfigureMetricCard(_cardChaptersBorder, _lblChaptersValue, _lblChaptersLabel, "Chương", "subjects");
        ConfigureMetricCard(_cardQuestionsBorder, _lblQuestionsValue, _lblQuestionsLabel, "Câu hỏi", "questions");
        ConfigureMetricCard(_cardClassesBorder, _lblClassesValue, _lblClassesLabel, "Lớp", "classes");
        ConfigureMetricCard(_cardStudentsBorder, _lblStudentsValue, _lblStudentsLabel, "Sinh viên", "classes");

        _metricsFlow.Controls.Add(_cardSubjectsBorder);
        _metricsFlow.Controls.Add(_cardChaptersBorder);
        _metricsFlow.Controls.Add(_cardQuestionsBorder);
        _metricsFlow.Controls.Add(_cardClassesBorder);
        _metricsFlow.Controls.Add(_cardStudentsBorder);

        // ================= 3. MIDDLE SECTION =================
        _middleLayout.Dock = DockStyle.Fill;
        _middleLayout.AutoSize = true;
        _middleLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _middleLayout.ColumnCount = 2;
        _middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
        _middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
        _middleLayout.RowCount = 1;
        _middleLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _middleLayout.Margin = new Padding(0, 0, 0, 16);

        // 3A. Difficulty card
        _difficultyCardBorder.Dock = DockStyle.Fill;
        _difficultyCardBorder.AutoSize = true;
        _difficultyCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _difficultyCardBorder.BackColor = UiStyle.Border;
        _difficultyCardBorder.Padding = new Padding(1);
        _difficultyCardBorder.Margin = new Padding(0, 0, 8, 0);

        _difficultyCardLayout.Dock = DockStyle.Fill;
        _difficultyCardLayout.AutoSize = true;
        _difficultyCardLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _difficultyCardLayout.BackColor = UiStyle.Surface;
        _difficultyCardLayout.Padding = new Padding(16);
        _difficultyCardLayout.ColumnCount = 1;
        _difficultyCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _difficultyCardLayout.RowCount = 2;
        _difficultyCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _difficultyCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _difficultyHeaderRow.Dock = DockStyle.Fill;
        _difficultyHeaderRow.AutoSize = true;
        _difficultyHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _difficultyHeaderRow.ColumnCount = 2;
        _difficultyHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _difficultyHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _difficultyHeaderRow.RowCount = 1;
        _difficultyHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _difficultyHeaderRow.Margin = new Padding(0, 0, 0, 12);

        _lblDifficultyTitle.AutoSize = true;
        _lblDifficultyTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblDifficultyTitle.ForeColor = UiStyle.Text;
        _lblDifficultyTitle.Margin = new Padding(0);
        _lblDifficultyTitle.Text = "Phân bố câu hỏi theo độ khó";

        _lblDifficultyTotal.AutoSize = true;
        _lblDifficultyTotal.Font = UiStyle.SmallFont;
        _lblDifficultyTotal.ForeColor = UiStyle.TextMuted;
        _lblDifficultyTotal.Margin = new Padding(8, 3, 0, 0);
        _lblDifficultyTotal.Text = "Tổng: 0 câu";

        _difficultyHeaderRow.Controls.Add(_lblDifficultyTitle, 0, 0);
        _difficultyHeaderRow.Controls.Add(_lblDifficultyTotal, 1, 0);

        _difficultyBarsTable.Dock = DockStyle.Fill;
        _difficultyBarsTable.AutoSize = true;
        _difficultyBarsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _difficultyBarsTable.ColumnCount = 3;
        _difficultyBarsTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _difficultyBarsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _difficultyBarsTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _difficultyBarsTable.RowCount = 3;
        _difficultyBarsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _difficultyBarsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _difficultyBarsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _difficultyBarsTable.Margin = new Padding(0);

        ConfigureDifficultyRow(_lblEasyName, "Dễ", _barEasy, _lblEasyStat, rowIndex: 0);
        ConfigureDifficultyRow(_lblMediumName, "Trung bình", _barMedium, _lblMediumStat, rowIndex: 1);
        ConfigureDifficultyRow(_lblHardName, "Khó", _barHard, _lblHardStat, rowIndex: 2);

        _difficultyCardLayout.Controls.Add(_difficultyHeaderRow, 0, 0);
        _difficultyCardLayout.Controls.Add(_difficultyBarsTable, 0, 1);
        _difficultyCardBorder.Controls.Add(_difficultyCardLayout);

        // 3B. Attention card
        _attentionCardBorder.Dock = DockStyle.Fill;
        _attentionCardBorder.AutoSize = true;
        _attentionCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _attentionCardBorder.BackColor = UiStyle.Border;
        _attentionCardBorder.Padding = new Padding(1);
        _attentionCardBorder.Margin = new Padding(8, 0, 0, 0);

        _attentionCardLayout.Dock = DockStyle.Fill;
        _attentionCardLayout.AutoSize = true;
        _attentionCardLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _attentionCardLayout.BackColor = UiStyle.Surface;
        _attentionCardLayout.Padding = new Padding(16);
        _attentionCardLayout.ColumnCount = 1;
        _attentionCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _attentionCardLayout.RowCount = 2;
        _attentionCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _attentionCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _attentionHeaderRow.Dock = DockStyle.Fill;
        _attentionHeaderRow.AutoSize = true;
        _attentionHeaderRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _attentionHeaderRow.ColumnCount = 2;
        _attentionHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _attentionHeaderRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _attentionHeaderRow.RowCount = 1;
        _attentionHeaderRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _attentionHeaderRow.Margin = new Padding(0, 0, 0, 12);

        _lblAttentionTitle.AutoSize = true;
        _lblAttentionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblAttentionTitle.ForeColor = UiStyle.Text;
        _lblAttentionTitle.Margin = new Padding(0);
        _lblAttentionTitle.Text = "Cần chú ý";

        _lblAttentionCount.AutoSize = true;
        _lblAttentionCount.Font = UiStyle.SmallFont;
        _lblAttentionCount.ForeColor = UiStyle.TextMuted;
        _lblAttentionCount.Margin = new Padding(8, 3, 0, 0);
        _lblAttentionCount.Text = "0 mục";

        _attentionHeaderRow.Controls.Add(_lblAttentionTitle, 0, 0);
        _attentionHeaderRow.Controls.Add(_lblAttentionCount, 1, 0);

        _attentionListFlow.Dock = DockStyle.Fill;
        _attentionListFlow.AutoSize = true;
        _attentionListFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _attentionListFlow.FlowDirection = FlowDirection.TopDown;
        _attentionListFlow.WrapContents = false;
        _attentionListFlow.Margin = new Padding(0);
        _attentionListFlow.SizeChanged += OnAttentionListSizeChanged;

        _attentionCardLayout.Controls.Add(_attentionHeaderRow, 0, 0);
        _attentionCardLayout.Controls.Add(_attentionListFlow, 0, 1);
        _attentionCardBorder.Controls.Add(_attentionCardLayout);

        _middleLayout.Controls.Add(_difficultyCardBorder, 0, 0);
        _middleLayout.Controls.Add(_attentionCardBorder, 1, 0);

        // ================= 4. QUICK ACTIONS =================
        _quickActionsCardBorder.Dock = DockStyle.Fill;
        _quickActionsCardBorder.AutoSize = true;
        _quickActionsCardBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _quickActionsCardBorder.BackColor = UiStyle.Border;
        _quickActionsCardBorder.Padding = new Padding(1);
        _quickActionsCardBorder.Margin = new Padding(0);

        _quickActionsCardLayout.Dock = DockStyle.Fill;
        _quickActionsCardLayout.AutoSize = true;
        _quickActionsCardLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _quickActionsCardLayout.BackColor = UiStyle.Surface;
        _quickActionsCardLayout.Padding = new Padding(16);
        _quickActionsCardLayout.ColumnCount = 1;
        _quickActionsCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _quickActionsCardLayout.RowCount = 2;
        _quickActionsCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _quickActionsCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblQuickActionsTitle.AutoSize = true;
        _lblQuickActionsTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblQuickActionsTitle.ForeColor = UiStyle.Text;
        _lblQuickActionsTitle.Margin = new Padding(0, 0, 0, 12);
        _lblQuickActionsTitle.Text = "Thao tác nhanh";

        _quickActionsFlow.Dock = DockStyle.Fill;
        _quickActionsFlow.AutoSize = true;
        _quickActionsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _quickActionsFlow.FlowDirection = FlowDirection.LeftToRight;
        _quickActionsFlow.WrapContents = true;
        _quickActionsFlow.Margin = new Padding(0);

        _btnQuickAddQuestion.Width = 150;
        _btnQuickAddQuestion.Height = 40;
        _btnQuickAddQuestion.Margin = new Padding(0, 0, 12, 8);
        _btnQuickAddQuestion.TabIndex = 0;
        _btnQuickAddQuestion.Text = "Thêm câu hỏi";
        UiStyle.StylePrimaryButton(_btnQuickAddQuestion);
        _btnQuickAddQuestion.Click += (_, _) => RequestNavigation("question-new");

        _btnQuickSubjects.Width = 190;
        _btnQuickSubjects.Height = 40;
        _btnQuickSubjects.Margin = new Padding(0, 0, 12, 8);
        _btnQuickSubjects.TabIndex = 1;
        _btnQuickSubjects.Text = "Quản lý môn và chương";
        UiStyle.StyleSecondaryButton(_btnQuickSubjects);
        _btnQuickSubjects.Click += (_, _) => RequestNavigation("subjects");

        _btnQuickClasses.Width = 195;
        _btnQuickClasses.Height = 40;
        _btnQuickClasses.Margin = new Padding(0, 0, 12, 8);
        _btnQuickClasses.TabIndex = 2;
        _btnQuickClasses.Text = "Quản lý lớp và sinh viên";
        UiStyle.StyleSecondaryButton(_btnQuickClasses);
        _btnQuickClasses.Click += (_, _) => RequestNavigation("classes");

        _btnQuickExams.Width = 150;
        _btnQuickExams.Height = 40;
        _btnQuickExams.Margin = new Padding(0, 0, 0, 8);
        _btnQuickExams.TabIndex = 3;
        _btnQuickExams.Text = "Đề thi";
        UiStyle.StyleSecondaryButton(_btnQuickExams);
        _btnQuickExams.Click += (_, _) => RequestNavigation("exams");

        _quickActionsFlow.Controls.Add(_btnQuickAddQuestion);
        _quickActionsFlow.Controls.Add(_btnQuickSubjects);
        _quickActionsFlow.Controls.Add(_btnQuickClasses);
        _quickActionsFlow.Controls.Add(_btnQuickExams);

        _quickActionsCardLayout.Controls.Add(_lblQuickActionsTitle, 0, 0);
        _quickActionsCardLayout.Controls.Add(_quickActionsFlow, 0, 1);
        _quickActionsCardBorder.Controls.Add(_quickActionsCardLayout);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_metricsFlow, 0, 1);
        _rootLayout.Controls.Add(_middleLayout, 0, 2);
        _rootLayout.Controls.Add(_quickActionsCardBorder, 0, 3);
        _scrollPanel.Controls.Add(_rootLayout);

        // DashboardControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        Controls.Add(_scrollPanel);
        Name = "DashboardControl";
        Size = new Size(1000, 600);

        _scrollPanel.ResumeLayout(false);
        _scrollPanel.PerformLayout();
        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _titleGroupFlow.ResumeLayout(false);
        _titleGroupFlow.PerformLayout();
        _metricsFlow.ResumeLayout(false);
        _metricsFlow.PerformLayout();
        _middleLayout.ResumeLayout(false);
        _middleLayout.PerformLayout();
        _difficultyCardBorder.ResumeLayout(false);
        _difficultyCardBorder.PerformLayout();
        _difficultyCardLayout.ResumeLayout(false);
        _difficultyCardLayout.PerformLayout();
        _difficultyHeaderRow.ResumeLayout(false);
        _difficultyHeaderRow.PerformLayout();
        _difficultyBarsTable.ResumeLayout(false);
        _difficultyBarsTable.PerformLayout();
        _attentionCardBorder.ResumeLayout(false);
        _attentionCardBorder.PerformLayout();
        _attentionCardLayout.ResumeLayout(false);
        _attentionCardLayout.PerformLayout();
        _attentionHeaderRow.ResumeLayout(false);
        _attentionHeaderRow.PerformLayout();
        _quickActionsCardBorder.ResumeLayout(false);
        _quickActionsCardBorder.PerformLayout();
        _quickActionsCardLayout.ResumeLayout(false);
        _quickActionsCardLayout.PerformLayout();
        _quickActionsFlow.ResumeLayout(false);
        _quickActionsFlow.PerformLayout();
        ResumeLayout(false);
    }

    private void ConfigureMetricCard(
        Panel borderPanel,
        Label valueLabel,
        Label titleLabel,
        string labelText,
        string navigationKey)
    {
        borderPanel.Width = 178;
        borderPanel.Height = 96;
        borderPanel.BackColor = UiStyle.Border;
        borderPanel.Padding = new Padding(1);
        borderPanel.Margin = new Padding(0, 0, 12, 12);
        borderPanel.Cursor = Cursors.Hand;

        var surfaceLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiStyle.Surface,
            Padding = new Padding(16, 12, 16, 12),
            ColumnCount = 1,
            RowCount = 2,
            Cursor = Cursors.Hand
        };
        surfaceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        surfaceLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        surfaceLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

        valueLabel.Dock = DockStyle.Fill;
        valueLabel.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        valueLabel.ForeColor = UiStyle.Primary;
        valueLabel.TextAlign = ContentAlignment.MiddleLeft;
        valueLabel.Margin = new Padding(0);
        valueLabel.Text = "0";
        valueLabel.Cursor = Cursors.Hand;

        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = UiStyle.BodyFont;
        titleLabel.ForeColor = UiStyle.TextMuted;
        titleLabel.TextAlign = ContentAlignment.BottomLeft;
        titleLabel.Margin = new Padding(0);
        titleLabel.Text = labelText;
        titleLabel.Cursor = Cursors.Hand;

        void OnCardClick(object? s, EventArgs e) => RequestNavigation(navigationKey);
        borderPanel.Click += OnCardClick;
        surfaceLayout.Click += OnCardClick;
        valueLabel.Click += OnCardClick;
        titleLabel.Click += OnCardClick;

        surfaceLayout.Controls.Add(valueLabel, 0, 0);
        surfaceLayout.Controls.Add(titleLabel, 0, 1);
        borderPanel.Controls.Add(surfaceLayout);
    }

    private void ConfigureDifficultyRow(
        Label nameLabel,
        string difficultyName,
        Panel barPanel,
        Label statLabel,
        int rowIndex)
    {
        nameLabel.AutoSize = true;
        nameLabel.Dock = DockStyle.Fill;
        nameLabel.MinimumSize = new Size(80, 0);
        nameLabel.Font = UiStyle.BodyFont;
        nameLabel.ForeColor = UiStyle.Text;
        nameLabel.TextAlign = ContentAlignment.MiddleLeft;
        nameLabel.Margin = new Padding(0, 6, 8, 6);
        nameLabel.Text = difficultyName;

        barPanel.Dock = DockStyle.Fill;
        barPanel.Height = 22;
        barPanel.BackColor = UiStyle.Background;
        barPanel.Margin = new Padding(0, 8, 10, 8);
        barPanel.Paint += OnDifficultyBarPaint;

        statLabel.AutoSize = true;
        statLabel.Dock = DockStyle.Fill;
        statLabel.MinimumSize = new Size(96, 0);
        statLabel.Font = UiStyle.SmallFont;
        statLabel.ForeColor = UiStyle.TextMuted;
        statLabel.TextAlign = ContentAlignment.MiddleRight;
        statLabel.Margin = new Padding(0, 6, 0, 6);
        statLabel.Text = "0 câu (0%)";

        _difficultyBarsTable.Controls.Add(nameLabel, 0, rowIndex);
        _difficultyBarsTable.Controls.Add(barPanel, 1, rowIndex);
        _difficultyBarsTable.Controls.Add(statLabel, 2, rowIndex);
    }
}
