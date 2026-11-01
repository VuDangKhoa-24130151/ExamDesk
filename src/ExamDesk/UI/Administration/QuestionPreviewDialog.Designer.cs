#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class QuestionPreviewDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private FlowLayoutPanel _headerPanel = null!;
    private Label _lblTitle = null!;
    private Label _lblMetaInfo = null!;
    private Panel _cardBorderPanel = null!;
    private Panel _scrollPanel = null!;
    private TableLayoutPanel _studentViewLayout = null!;
    private Label _lblStudentSectionTitle = null!;
    private Panel _questionBoxPanel = null!;
    private Label _lblQuestionContent = null!;
    private TableLayoutPanel _optionsListLayout = null!;
    private Panel _optABorder = null!;
    private TableLayoutPanel _optARow = null!;
    private RadioButton _radOptA = null!;
    private Label _lblOptAText = null!;
    private Label _lblOptABadge = null!;
    private Panel _optBBorder = null!;
    private TableLayoutPanel _optBRow = null!;
    private RadioButton _radOptB = null!;
    private Label _lblOptBText = null!;
    private Label _lblOptBBadge = null!;
    private Panel _optCBorder = null!;
    private TableLayoutPanel _optCRow = null!;
    private RadioButton _radOptC = null!;
    private Label _lblOptCText = null!;
    private Label _lblOptCBadge = null!;
    private Panel _optDBorder = null!;
    private TableLayoutPanel _optDRow = null!;
    private RadioButton _radOptD = null!;
    private Label _lblOptDText = null!;
    private Label _lblOptDBadge = null!;
    private TableLayoutPanel _footerLayout = null!;
    private CheckBox _chkShowCorrect = null!;
    private Button _btnEdit = null!;
    private Button _btnClose = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerPanel = new FlowLayoutPanel();
        _lblTitle = new Label();
        _lblMetaInfo = new Label();
        _cardBorderPanel = new Panel();
        _scrollPanel = new Panel();
        _studentViewLayout = new TableLayoutPanel();
        _lblStudentSectionTitle = new Label();
        _questionBoxPanel = new Panel();
        _lblQuestionContent = new Label();
        _optionsListLayout = new TableLayoutPanel();
        _optABorder = new Panel();
        _optARow = new TableLayoutPanel();
        _radOptA = new RadioButton();
        _lblOptAText = new Label();
        _lblOptABadge = new Label();
        _optBBorder = new Panel();
        _optBRow = new TableLayoutPanel();
        _radOptB = new RadioButton();
        _lblOptBText = new Label();
        _lblOptBBadge = new Label();
        _optCBorder = new Panel();
        _optCRow = new TableLayoutPanel();
        _radOptC = new RadioButton();
        _lblOptCText = new Label();
        _lblOptCBadge = new Label();
        _optDBorder = new Panel();
        _optDRow = new TableLayoutPanel();
        _radOptD = new RadioButton();
        _lblOptDText = new Label();
        _lblOptDBadge = new Label();
        _footerLayout = new TableLayoutPanel();
        _chkShowCorrect = new CheckBox();
        _btnEdit = new Button();
        _btnClose = new Button();

        _rootLayout.SuspendLayout();
        _headerPanel.SuspendLayout();
        _cardBorderPanel.SuspendLayout();
        _scrollPanel.SuspendLayout();
        _studentViewLayout.SuspendLayout();
        _questionBoxPanel.SuspendLayout();
        _optionsListLayout.SuspendLayout();
        _footerLayout.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24, 20, 24, 20);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 3;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Student view card
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 2: Footer

        // _headerPanel
        _headerPanel.Dock = DockStyle.Fill;
        _headerPanel.AutoSize = true;
        _headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _headerPanel.FlowDirection = FlowDirection.TopDown;
        _headerPanel.WrapContents = false;
        _headerPanel.Margin = new Padding(0, 0, 0, 12);

        // _lblTitle
        _lblTitle.AutoSize = true;
        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;
        _lblTitle.Margin = new Padding(0, 0, 0, 4);
        _lblTitle.Text = "Xem trước câu hỏi";

        // _lblMetaInfo
        _lblMetaInfo.AutoSize = true;
        _lblMetaInfo.Font = UiStyle.BodyFont;
        _lblMetaInfo.ForeColor = UiStyle.TextMuted;
        _lblMetaInfo.Margin = new Padding(0);
        _lblMetaInfo.Text = "Môn · Chương · Độ khó";

        _headerPanel.Controls.Add(_lblTitle);
        _headerPanel.Controls.Add(_lblMetaInfo);

        // _cardBorderPanel
        _cardBorderPanel.Dock = DockStyle.Fill;
        _cardBorderPanel.BackColor = UiStyle.Border;
        _cardBorderPanel.Padding = new Padding(1);
        _cardBorderPanel.Margin = new Padding(0, 0, 0, 16);

        // _scrollPanel
        _scrollPanel.Dock = DockStyle.Fill;
        _scrollPanel.AutoScroll = true;
        _scrollPanel.BackColor = UiStyle.Surface;
        _scrollPanel.Padding = new Padding(20, 16, 20, 16);

        // _studentViewLayout
        _studentViewLayout.Dock = DockStyle.Top;
        _studentViewLayout.AutoSize = true;
        _studentViewLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _studentViewLayout.BackColor = UiStyle.Surface;
        _studentViewLayout.ColumnCount = 1;
        _studentViewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _studentViewLayout.RowCount = 3;
        _studentViewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Section label
        _studentViewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Question content
        _studentViewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: Options A-D

        // _lblStudentSectionTitle
        _lblStudentSectionTitle.AutoSize = true;
        _lblStudentSectionTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _lblStudentSectionTitle.ForeColor = UiStyle.Primary;
        _lblStudentSectionTitle.Margin = new Padding(0, 0, 0, 12);
        _lblStudentSectionTitle.Text = "Cách sinh viên nhìn thấy";

        // _questionBoxPanel
        _questionBoxPanel.Dock = DockStyle.Fill;
        _questionBoxPanel.AutoSize = true;
        _questionBoxPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _questionBoxPanel.BackColor = UiStyle.Background;
        _questionBoxPanel.Padding = new Padding(16, 14, 16, 14);
        _questionBoxPanel.Margin = new Padding(0, 0, 0, 16);

        // _lblQuestionContent
        _lblQuestionContent.AutoSize = true;
        _lblQuestionContent.Dock = DockStyle.Fill;
        _lblQuestionContent.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        _lblQuestionContent.ForeColor = UiStyle.Text;
        _lblQuestionContent.Margin = new Padding(0);
        _lblQuestionContent.Text = string.Empty;

        _questionBoxPanel.Controls.Add(_lblQuestionContent);

        // _optionsListLayout
        _optionsListLayout.Dock = DockStyle.Fill;
        _optionsListLayout.AutoSize = true;
        _optionsListLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _optionsListLayout.ColumnCount = 1;
        _optionsListLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _optionsListLayout.RowCount = 4;
        for (var i = 0; i < 4; i++)
        {
            _optionsListLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }
        _optionsListLayout.Margin = new Padding(0);

        ConfigurePreviewOptionRow(_optABorder, _optARow, _radOptA, "A.", _lblOptAText, _lblOptABadge, 0);
        ConfigurePreviewOptionRow(_optBBorder, _optBRow, _radOptB, "B.", _lblOptBText, _lblOptBBadge, 1);
        ConfigurePreviewOptionRow(_optCBorder, _optCRow, _radOptC, "C.", _lblOptCText, _lblOptCBadge, 2);
        ConfigurePreviewOptionRow(_optDBorder, _optDRow, _radOptD, "D.", _lblOptDText, _lblOptDBadge, 3);

        _studentViewLayout.Controls.Add(_lblStudentSectionTitle, 0, 0);
        _studentViewLayout.Controls.Add(_questionBoxPanel, 0, 1);
        _studentViewLayout.Controls.Add(_optionsListLayout, 0, 2);

        _scrollPanel.Controls.Add(_studentViewLayout);
        _cardBorderPanel.Controls.Add(_scrollPanel);

        // _footerLayout
        _footerLayout.Dock = DockStyle.Fill;
        _footerLayout.AutoSize = true;
        _footerLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _footerLayout.ColumnCount = 4;
        _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _footerLayout.RowCount = 1;
        _footerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _footerLayout.Margin = new Padding(0);

        // _chkShowCorrect
        _chkShowCorrect.AutoSize = true;
        _chkShowCorrect.Font = UiStyle.BodyFont;
        _chkShowCorrect.ForeColor = UiStyle.Text;
        _chkShowCorrect.Margin = new Padding(0, 8, 16, 0);
        _chkShowCorrect.TabIndex = 0;
        _chkShowCorrect.Text = "Hiện đáp án đúng";
        _chkShowCorrect.Checked = false;
        _chkShowCorrect.CheckedChanged += OnShowCorrectCheckedChanged;

        // _btnEdit
        _btnEdit.Width = 110;
        _btnEdit.Height = 40;
        _btnEdit.Margin = new Padding(0, 0, 12, 0);
        _btnEdit.TabIndex = 1;
        _btnEdit.Text = "Sửa";
        _btnEdit.Visible = false;
        UiStyle.StyleSecondaryButton(_btnEdit);
        _btnEdit.Click += OnEditClick;

        // _btnClose
        _btnClose.Width = 110;
        _btnClose.Height = 40;
        _btnClose.Margin = new Padding(0);
        _btnClose.TabIndex = 2;
        _btnClose.Text = "Đóng";
        UiStyle.StylePrimaryButton(_btnClose);
        _btnClose.Click += OnCloseClick;

        _footerLayout.Controls.Add(_chkShowCorrect, 0, 0);
        _footerLayout.Controls.Add(_btnEdit, 2, 0);
        _footerLayout.Controls.Add(_btnClose, 3, 0);

        _rootLayout.Controls.Add(_headerPanel, 0, 0);
        _rootLayout.Controls.Add(_cardBorderPanel, 0, 1);
        _rootLayout.Controls.Add(_footerLayout, 0, 2);

        // QuestionPreviewDialog
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(720, 560);
        MinimumSize = new Size(580, 440);
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = _btnClose;
        Controls.Add(_rootLayout);
        Name = "QuestionPreviewDialog";
        Text = "UniExam – Xem trước câu hỏi";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerPanel.ResumeLayout(false);
        _headerPanel.PerformLayout();
        _cardBorderPanel.ResumeLayout(false);
        _scrollPanel.ResumeLayout(false);
        _scrollPanel.PerformLayout();
        _studentViewLayout.ResumeLayout(false);
        _studentViewLayout.PerformLayout();
        _questionBoxPanel.ResumeLayout(false);
        _questionBoxPanel.PerformLayout();
        _optionsListLayout.ResumeLayout(false);
        _optionsListLayout.PerformLayout();
        _footerLayout.ResumeLayout(false);
        _footerLayout.PerformLayout();
        ResumeLayout(false);
    }

    private void ConfigurePreviewOptionRow(
        Panel borderPanel,
        TableLayoutPanel rowLayout,
        RadioButton radioButton,
        string prefix,
        Label textLabel,
        Label badgeLabel,
        int rowIndex)
    {
        borderPanel.SuspendLayout();
        rowLayout.SuspendLayout();

        borderPanel.Dock = DockStyle.Fill;
        borderPanel.AutoSize = true;
        borderPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        borderPanel.BackColor = UiStyle.Border;
        borderPanel.Padding = new Padding(1);
        borderPanel.Margin = new Padding(0, 0, 0, 10);

        rowLayout.Dock = DockStyle.Fill;
        rowLayout.AutoSize = true;
        rowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        rowLayout.BackColor = UiStyle.Surface;
        rowLayout.Padding = new Padding(12, 10, 12, 10);
        rowLayout.ColumnCount = 3;
        rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rowLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        rowLayout.RowCount = 1;
        rowLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rowLayout.Margin = new Padding(0);

        radioButton.AutoSize = true;
        radioButton.Enabled = false;
        radioButton.Checked = false;
        radioButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        radioButton.ForeColor = UiStyle.Text;
        radioButton.Margin = new Padding(0, 0, 8, 0);
        radioButton.Text = prefix;

        textLabel.AutoSize = true;
        textLabel.Dock = DockStyle.Fill;
        textLabel.Font = UiStyle.BodyFont;
        textLabel.ForeColor = UiStyle.Text;
        textLabel.Margin = new Padding(0, 2, 8, 0);
        textLabel.Text = string.Empty;

        badgeLabel.AutoSize = true;
        badgeLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        badgeLabel.ForeColor = UiStyle.SuccessText;
        badgeLabel.Margin = new Padding(8, 2, 0, 0);
        badgeLabel.Text = "✓ Đáp án đúng";
        badgeLabel.Visible = false;

        rowLayout.Controls.Add(radioButton, 0, 0);
        rowLayout.Controls.Add(textLabel, 1, 0);
        rowLayout.Controls.Add(badgeLabel, 2, 0);
        borderPanel.Controls.Add(rowLayout);

        _optionsListLayout.Controls.Add(borderPanel, 0, rowIndex);

        rowLayout.ResumeLayout(false);
        rowLayout.PerformLayout();
        borderPanel.ResumeLayout(false);
        borderPanel.PerformLayout();
    }
}
