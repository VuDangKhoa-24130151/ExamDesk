#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class QuestionEditorDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private Panel _scrollPanel = null!;
    private TableLayoutPanel _formLayout = null!;
    private TableLayoutPanel _metaLayout = null!;
    private Label _lblSubject = null!;
    private ComboBox _cboSubject = null!;
    private Label _lblSubjectError = null!;
    private Label _lblChapter = null!;
    private NumericUpDown _numChapter = null!;
    private Label _lblChapterError = null!;
    private Label _lblDifficulty = null!;
    private FlowLayoutPanel _difficultyFlow = null!;
    private RadioButton _radEasy = null!;
    private RadioButton _radMedium = null!;
    private RadioButton _radHard = null!;
    private Label _lblDifficultyError = null!;
    private TableLayoutPanel _contentHeaderLayout = null!;
    private Label _lblContent = null!;
    private Label _lblCharCount = null!;
    private TextBox _txtContent = null!;
    private Label _lblContentError = null!;
    private TableLayoutPanel _optionsHeaderLayout = null!;
    private Label _lblOptionsSection = null!;
    private Label _lblCorrectColumnHint = null!;
    private TableLayoutPanel _optionsLayout = null!;
    private Label _lblOptionA = null!;
    private TextBox _txtOptionA = null!;
    private RadioButton _radCorrectA = null!;
    private Label _lblOptionAError = null!;
    private Label _lblOptionB = null!;
    private TextBox _txtOptionB = null!;
    private RadioButton _radCorrectB = null!;
    private Label _lblOptionBError = null!;
    private Label _lblOptionC = null!;
    private TextBox _txtOptionC = null!;
    private RadioButton _radCorrectC = null!;
    private Label _lblOptionCError = null!;
    private Label _lblOptionD = null!;
    private TextBox _txtOptionD = null!;
    private RadioButton _radCorrectD = null!;
    private Label _lblOptionDError = null!;
    private Label _lblCorrectOptionError = null!;
    private TableLayoutPanel _footerLayout = null!;
    private Button _btnPreview = null!;
    private Button _btnCancel = null!;
    private Button _btnSave = null!;

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
        _scrollPanel = new Panel();
        _formLayout = new TableLayoutPanel();
        _metaLayout = new TableLayoutPanel();
        _lblSubject = new Label();
        _cboSubject = new ComboBox();
        _lblSubjectError = new Label();
        _lblChapter = new Label();
        _numChapter = new NumericUpDown();
        _lblChapterError = new Label();
        _lblDifficulty = new Label();
        _difficultyFlow = new FlowLayoutPanel();
        _radEasy = new RadioButton();
        _radMedium = new RadioButton();
        _radHard = new RadioButton();
        _lblDifficultyError = new Label();
        _contentHeaderLayout = new TableLayoutPanel();
        _lblContent = new Label();
        _lblCharCount = new Label();
        _txtContent = new TextBox();
        _lblContentError = new Label();
        _optionsHeaderLayout = new TableLayoutPanel();
        _lblOptionsSection = new Label();
        _lblCorrectColumnHint = new Label();
        _optionsLayout = new TableLayoutPanel();
        _lblOptionA = new Label();
        _txtOptionA = new TextBox();
        _radCorrectA = new RadioButton();
        _lblOptionAError = new Label();
        _lblOptionB = new Label();
        _txtOptionB = new TextBox();
        _radCorrectB = new RadioButton();
        _lblOptionBError = new Label();
        _lblOptionC = new Label();
        _txtOptionC = new TextBox();
        _radCorrectC = new RadioButton();
        _lblOptionCError = new Label();
        _lblOptionD = new Label();
        _txtOptionD = new TextBox();
        _radCorrectD = new RadioButton();
        _lblOptionDError = new Label();
        _lblCorrectOptionError = new Label();
        _footerLayout = new TableLayoutPanel();
        _btnPreview = new Button();
        _btnCancel = new Button();
        _btnSave = new Button();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _scrollPanel.SuspendLayout();
        _formLayout.SuspendLayout();
        _metaLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_numChapter).BeginInit();
        _difficultyFlow.SuspendLayout();
        _contentHeaderLayout.SuspendLayout();
        _optionsHeaderLayout.SuspendLayout();
        _optionsLayout.SuspendLayout();
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
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Scrollable form body
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 2: Footer buttons

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
        _lblTitle.Text = "Thêm câu hỏi";

        // _lblDemoBadge
        _lblDemoBadge.AutoSize = true;
        _lblDemoBadge.BackColor = UiStyle.WarningBack;
        _lblDemoBadge.ForeColor = UiStyle.WarningText;
        _lblDemoBadge.Font = UiStyle.SmallFont;
        _lblDemoBadge.Padding = new Padding(10, 6, 10, 6);
        _lblDemoBadge.Margin = new Padding(8, 4, 0, 0);
        _lblDemoBadge.Text = "Minh họa – chưa lưu CSDL (Ctrl+S để lưu)";

        _headerLayout.Controls.Add(_lblTitle, 0, 0);
        _headerLayout.Controls.Add(_lblDemoBadge, 1, 0);

        // _scrollPanel
        _scrollPanel.Dock = DockStyle.Fill;
        _scrollPanel.AutoScroll = true;
        _scrollPanel.BackColor = UiStyle.Surface;
        _scrollPanel.BorderStyle = BorderStyle.FixedSingle;
        _scrollPanel.Padding = new Padding(20, 16, 20, 16);
        _scrollPanel.Margin = new Padding(0, 0, 0, 16);

        // _formLayout
        _formLayout.Dock = DockStyle.Top;
        _formLayout.AutoSize = true;
        _formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _formLayout.BackColor = UiStyle.Surface;
        _formLayout.ColumnCount = 1;
        _formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _formLayout.RowCount = 7;
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 0: Meta row (Subject, Chapter, Difficulty)
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 1: Content header
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 2: Content textbox
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 3: Content error
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 4: Options header
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 5: Options table
        _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // 6: Correct option error

        // _metaLayout (3 columns: Subject | Chapter | Difficulty)
        _metaLayout.Dock = DockStyle.Fill;
        _metaLayout.AutoSize = true;
        _metaLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _metaLayout.ColumnCount = 3;
        _metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        _metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        _metaLayout.RowCount = 3;
        _metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Labels
        _metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Inputs
        _metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Errors
        _metaLayout.Margin = new Padding(0, 0, 0, 12);

        // Subject
        _lblSubject.AutoSize = true;
        _lblSubject.Font = UiStyle.BodyFont;
        _lblSubject.ForeColor = UiStyle.Text;
        _lblSubject.Margin = new Padding(0, 0, 12, 6);
        _lblSubject.Text = "Môn *";

        _cboSubject.Dock = DockStyle.Fill;
        _cboSubject.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSubject.Font = UiStyle.BodyFont;
        _cboSubject.Margin = new Padding(0, 0, 12, 4);
        _cboSubject.TabIndex = 0;
        _cboSubject.SelectedIndexChanged += OnSubjectChanged;

        _lblSubjectError.AutoSize = true;
        _lblSubjectError.Font = UiStyle.SmallFont;
        _lblSubjectError.ForeColor = UiStyle.ErrorText;
        _lblSubjectError.Margin = new Padding(0, 0, 12, 0);
        _lblSubjectError.Visible = false;

        // Chapter
        _lblChapter.AutoSize = true;
        _lblChapter.Font = UiStyle.BodyFont;
        _lblChapter.ForeColor = UiStyle.Text;
        _lblChapter.Margin = new Padding(0, 0, 12, 6);
        _lblChapter.Text = "Chương *";

        _numChapter.Dock = DockStyle.Fill;
        _numChapter.Font = UiStyle.BodyFont;
        _numChapter.Minimum = 1;
        _numChapter.Maximum = 50;
        _numChapter.Value = 1;
        _numChapter.Margin = new Padding(0, 0, 12, 4);
        _numChapter.TabIndex = 1;
        _numChapter.ValueChanged += OnChapterChanged;

        _lblChapterError.AutoSize = true;
        _lblChapterError.Font = UiStyle.SmallFont;
        _lblChapterError.ForeColor = UiStyle.ErrorText;
        _lblChapterError.Margin = new Padding(0, 0, 12, 0);
        _lblChapterError.Visible = false;

        // Difficulty
        _lblDifficulty.AutoSize = true;
        _lblDifficulty.Font = UiStyle.BodyFont;
        _lblDifficulty.ForeColor = UiStyle.Text;
        _lblDifficulty.Margin = new Padding(0, 0, 0, 6);
        _lblDifficulty.Text = "Độ khó *";

        _difficultyFlow.Dock = DockStyle.Fill;
        _difficultyFlow.AutoSize = true;
        _difficultyFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _difficultyFlow.FlowDirection = FlowDirection.LeftToRight;
        _difficultyFlow.WrapContents = true;
        _difficultyFlow.Margin = new Padding(0, 0, 0, 4);

        _radEasy.AutoSize = true;
        _radEasy.Font = UiStyle.BodyFont;
        _radEasy.ForeColor = UiStyle.Text;
        _radEasy.Margin = new Padding(0, 2, 12, 2);
        _radEasy.TabIndex = 2;
        _radEasy.Text = "Dễ";
        _radEasy.Checked = true;
        _radEasy.CheckedChanged += OnDifficultyChanged;

        _radMedium.AutoSize = true;
        _radMedium.Font = UiStyle.BodyFont;
        _radMedium.ForeColor = UiStyle.Text;
        _radMedium.Margin = new Padding(0, 2, 12, 2);
        _radMedium.TabIndex = 3;
        _radMedium.Text = "Trung bình";
        _radMedium.CheckedChanged += OnDifficultyChanged;

        _radHard.AutoSize = true;
        _radHard.Font = UiStyle.BodyFont;
        _radHard.ForeColor = UiStyle.Text;
        _radHard.Margin = new Padding(0, 2, 0, 2);
        _radHard.TabIndex = 4;
        _radHard.Text = "Khó";
        _radHard.CheckedChanged += OnDifficultyChanged;

        _difficultyFlow.Controls.Add(_radEasy);
        _difficultyFlow.Controls.Add(_radMedium);
        _difficultyFlow.Controls.Add(_radHard);

        _lblDifficultyError.AutoSize = true;
        _lblDifficultyError.Font = UiStyle.SmallFont;
        _lblDifficultyError.ForeColor = UiStyle.ErrorText;
        _lblDifficultyError.Margin = new Padding(0);
        _lblDifficultyError.Visible = false;

        _metaLayout.Controls.Add(_lblSubject, 0, 0);
        _metaLayout.Controls.Add(_cboSubject, 0, 1);
        _metaLayout.Controls.Add(_lblSubjectError, 0, 2);
        _metaLayout.Controls.Add(_lblChapter, 1, 0);
        _metaLayout.Controls.Add(_numChapter, 1, 1);
        _metaLayout.Controls.Add(_lblChapterError, 1, 2);
        _metaLayout.Controls.Add(_lblDifficulty, 2, 0);
        _metaLayout.Controls.Add(_difficultyFlow, 2, 1);
        _metaLayout.Controls.Add(_lblDifficultyError, 2, 2);

        // _contentHeaderLayout
        _contentHeaderLayout.Dock = DockStyle.Fill;
        _contentHeaderLayout.AutoSize = true;
        _contentHeaderLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _contentHeaderLayout.ColumnCount = 2;
        _contentHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contentHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _contentHeaderLayout.RowCount = 1;
        _contentHeaderLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _contentHeaderLayout.Margin = new Padding(0, 0, 0, 6);

        _lblContent.AutoSize = true;
        _lblContent.Font = UiStyle.BodyFont;
        _lblContent.ForeColor = UiStyle.Text;
        _lblContent.Margin = new Padding(0);
        _lblContent.Text = "Nội dung câu hỏi *";

        _lblCharCount.AutoSize = true;
        _lblCharCount.Font = UiStyle.SmallFont;
        _lblCharCount.ForeColor = UiStyle.TextMuted;
        _lblCharCount.Margin = new Padding(0, 2, 0, 0);
        _lblCharCount.Text = "0 ký tự";

        _contentHeaderLayout.Controls.Add(_lblContent, 0, 0);
        _contentHeaderLayout.Controls.Add(_lblCharCount, 1, 0);

        // _txtContent
        _txtContent.Dock = DockStyle.Fill;
        _txtContent.Multiline = true;
        _txtContent.WordWrap = true;
        _txtContent.AcceptsReturn = true;
        _txtContent.ScrollBars = ScrollBars.Vertical;
        _txtContent.Font = UiStyle.BodyFont;
        _txtContent.ForeColor = UiStyle.Text;
        _txtContent.BackColor = UiStyle.Surface;
        _txtContent.BorderStyle = BorderStyle.FixedSingle;
        _txtContent.Height = 96;
        _txtContent.Margin = new Padding(0, 0, 0, 4);
        _txtContent.TabIndex = 5;
        _txtContent.TextChanged += OnContentTextChanged;

        // _lblContentError
        _lblContentError.AutoSize = true;
        _lblContentError.Dock = DockStyle.Fill;
        _lblContentError.Font = UiStyle.SmallFont;
        _lblContentError.ForeColor = UiStyle.ErrorText;
        _lblContentError.Margin = new Padding(0, 0, 0, 10);
        _lblContentError.Visible = false;

        // _optionsHeaderLayout
        _optionsHeaderLayout.Dock = DockStyle.Fill;
        _optionsHeaderLayout.AutoSize = true;
        _optionsHeaderLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _optionsHeaderLayout.ColumnCount = 2;
        _optionsHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _optionsHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _optionsHeaderLayout.RowCount = 1;
        _optionsHeaderLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _optionsHeaderLayout.Margin = new Padding(0, 4, 0, 8);

        _lblOptionsSection.AutoSize = true;
        _lblOptionsSection.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _lblOptionsSection.ForeColor = UiStyle.Text;
        _lblOptionsSection.Margin = new Padding(0);
        _lblOptionsSection.Text = "Các phương án trả lời (4 đáp án không được trùng nhau) *";

        _lblCorrectColumnHint.AutoSize = true;
        _lblCorrectColumnHint.Font = UiStyle.SmallFont;
        _lblCorrectColumnHint.ForeColor = UiStyle.TextMuted;
        _lblCorrectColumnHint.Margin = new Padding(0, 2, 8, 0);
        _lblCorrectColumnHint.Text = "Đánh dấu 1 đáp án đúng *";

        _optionsHeaderLayout.Controls.Add(_lblOptionsSection, 0, 0);
        _optionsHeaderLayout.Controls.Add(_lblCorrectColumnHint, 1, 0);

        // _optionsLayout (3 columns: Label | Multiline TextBox | RadioButton "Đúng")
        _optionsLayout.Dock = DockStyle.Fill;
        _optionsLayout.AutoSize = true;
        _optionsLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _optionsLayout.ColumnCount = 3;
        _optionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _optionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _optionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _optionsLayout.RowCount = 8;
        for (var i = 0; i < 8; i++)
        {
            _optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }
        _optionsLayout.Margin = new Padding(0);

        ConfigureOptionRow(
            _lblOptionA, "Đáp án A *",
            _txtOptionA, 6,
            _radCorrectA, 7,
            _lblOptionAError,
            rowIndex: 0);

        ConfigureOptionRow(
            _lblOptionB, "Đáp án B *",
            _txtOptionB, 8,
            _radCorrectB, 9,
            _lblOptionBError,
            rowIndex: 2);

        ConfigureOptionRow(
            _lblOptionC, "Đáp án C *",
            _txtOptionC, 10,
            _radCorrectC, 11,
            _lblOptionCError,
            rowIndex: 4);

        ConfigureOptionRow(
            _lblOptionD, "Đáp án D *",
            _txtOptionD, 12,
            _radCorrectD, 13,
            _lblOptionDError,
            rowIndex: 6);

        // _lblCorrectOptionError
        _lblCorrectOptionError.AutoSize = true;
        _lblCorrectOptionError.Dock = DockStyle.Fill;
        _lblCorrectOptionError.Font = UiStyle.SmallFont;
        _lblCorrectOptionError.ForeColor = UiStyle.ErrorText;
        _lblCorrectOptionError.Margin = new Padding(0, 4, 0, 0);
        _lblCorrectOptionError.Visible = false;

        _formLayout.Controls.Add(_metaLayout, 0, 0);
        _formLayout.Controls.Add(_contentHeaderLayout, 0, 1);
        _formLayout.Controls.Add(_txtContent, 0, 2);
        _formLayout.Controls.Add(_lblContentError, 0, 3);
        _formLayout.Controls.Add(_optionsHeaderLayout, 0, 4);
        _formLayout.Controls.Add(_optionsLayout, 0, 5);
        _formLayout.Controls.Add(_lblCorrectOptionError, 0, 6);

        _scrollPanel.Controls.Add(_formLayout);

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

        // _btnPreview
        _btnPreview.Width = 120;
        _btnPreview.Height = 40;
        _btnPreview.Margin = new Padding(0);
        _btnPreview.TabIndex = 14;
        _btnPreview.Text = "Xem trước";
        UiStyle.StyleSecondaryButton(_btnPreview);
        _btnPreview.Click += OnPreviewClick;

        // _btnCancel
        _btnCancel.Width = 100;
        _btnCancel.Height = 40;
        _btnCancel.Margin = new Padding(0, 0, 12, 0);
        _btnCancel.TabIndex = 15;
        _btnCancel.Text = "Hủy";
        UiStyle.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += OnCancelClick;

        // _btnSave
        _btnSave.Width = 140;
        _btnSave.Height = 40;
        _btnSave.Margin = new Padding(0);
        _btnSave.TabIndex = 16;
        _btnSave.Text = "Lưu";
        UiStyle.StylePrimaryButton(_btnSave);
        _btnSave.Click += OnSaveClick;

        _footerLayout.Controls.Add(_btnPreview, 0, 0);
        _footerLayout.Controls.Add(_btnCancel, 2, 0);
        _footerLayout.Controls.Add(_btnSave, 3, 0);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_scrollPanel, 0, 1);
        _rootLayout.Controls.Add(_footerLayout, 0, 2);

        // QuestionEditorDialog
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(760, 640);
        MinimumSize = new Size(640, 560);
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        Controls.Add(_rootLayout);
        Name = "QuestionEditorDialog";
        Text = "Thêm câu hỏi";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _scrollPanel.ResumeLayout(false);
        _scrollPanel.PerformLayout();
        _formLayout.ResumeLayout(false);
        _formLayout.PerformLayout();
        _metaLayout.ResumeLayout(false);
        _metaLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_numChapter).EndInit();
        _difficultyFlow.ResumeLayout(false);
        _difficultyFlow.PerformLayout();
        _contentHeaderLayout.ResumeLayout(false);
        _contentHeaderLayout.PerformLayout();
        _optionsHeaderLayout.ResumeLayout(false);
        _optionsHeaderLayout.PerformLayout();
        _optionsLayout.ResumeLayout(false);
        _optionsLayout.PerformLayout();
        _footerLayout.ResumeLayout(false);
        _footerLayout.PerformLayout();
        ResumeLayout(false);
    }

    private void ConfigureOptionRow(
        Label label,
        string labelText,
        TextBox textBox,
        int textBoxTabIndex,
        RadioButton correctRadio,
        int radioTabIndex,
        Label errorLabel,
        int rowIndex)
    {
        label.AutoSize = true;
        label.Font = UiStyle.BodyFont;
        label.ForeColor = UiStyle.Text;
        label.Margin = new Padding(0, 10, 8, 0);
        label.Text = labelText;

        textBox.Dock = DockStyle.Fill;
        textBox.Multiline = true;
        textBox.WordWrap = true;
        textBox.AcceptsReturn = true;
        textBox.ScrollBars = ScrollBars.Vertical;
        textBox.Font = UiStyle.BodyFont;
        textBox.ForeColor = UiStyle.Text;
        textBox.BackColor = UiStyle.Surface;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Height = 46;
        textBox.Margin = new Padding(0, 4, 12, 2);
        textBox.TabIndex = textBoxTabIndex;
        textBox.TextChanged += OnOptionTextChanged;

        correctRadio.AutoSize = true;
        correctRadio.Font = UiStyle.BodyFont;
        correctRadio.ForeColor = UiStyle.Text;
        correctRadio.Margin = new Padding(4, 12, 4, 0);
        correctRadio.TabIndex = radioTabIndex;
        correctRadio.Text = "Đúng";
        correctRadio.CheckedChanged += OnCorrectOptionCheckedChanged;

        errorLabel.AutoSize = true;
        errorLabel.Dock = DockStyle.Fill;
        errorLabel.Font = UiStyle.SmallFont;
        errorLabel.ForeColor = UiStyle.ErrorText;
        errorLabel.Margin = new Padding(0, 0, 12, 6);
        errorLabel.Visible = false;

        _optionsLayout.Controls.Add(label, 0, rowIndex);
        _optionsLayout.Controls.Add(textBox, 1, rowIndex);
        _optionsLayout.Controls.Add(correctRadio, 2, rowIndex);
        _optionsLayout.Controls.Add(errorLabel, 1, rowIndex + 1);
    }
}
