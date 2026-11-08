#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class ChapterEditDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private Panel _cardPanel = null!;
    private TableLayoutPanel _formLayout = null!;
    private Label _lblSubjectInfo = null!;
    private Label _lblNumber = null!;
    private NumericUpDown _numChapterNumber = null!;
    private Label _lblNumberHint = null!;
    private Label _lblNumberError = null!;
    private Label _lblName = null!;
    private TextBox _txtName = null!;
    private Label _lblNameError = null!;
    private FlowLayoutPanel _footerFlow = null!;
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
        _cardPanel = new Panel();
        _formLayout = new TableLayoutPanel();
        _lblSubjectInfo = new Label();
        _lblNumber = new Label();
        _numChapterNumber = new NumericUpDown();
        _lblNumberHint = new Label();
        _lblNumberError = new Label();
        _lblName = new Label();
        _txtName = new TextBox();
        _lblNameError = new Label();
        _footerFlow = new FlowLayoutPanel();
        _btnCancel = new Button();
        _btnSave = new Button();

        _rootLayout.SuspendLayout();
        _headerLayout.SuspendLayout();
        _cardPanel.SuspendLayout();
        _formLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_numChapterNumber).BeginInit();
        _footerFlow.SuspendLayout();
        SuspendLayout();

        // _rootLayout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.BackColor = UiStyle.Background;
        _rootLayout.Padding = new Padding(24, 20, 24, 20);
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.RowCount = 3;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // 0: Header
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 1: Form card
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
        _lblTitle.Text = "Thêm chương";

        // _lblDemoBadge
        _lblDemoBadge.AutoSize = true;
        _lblDemoBadge.BackColor = UiStyle.WarningBack;
        _lblDemoBadge.ForeColor = UiStyle.WarningText;
        _lblDemoBadge.Font = UiStyle.SmallFont;
        _lblDemoBadge.Padding = new Padding(10, 6, 10, 6);
        _lblDemoBadge.Margin = new Padding(8, 4, 0, 0);
        _lblDemoBadge.Text = "Minh họa – chưa lưu CSDL";

        _headerLayout.Controls.Add(_lblTitle, 0, 0);
        _headerLayout.Controls.Add(_lblDemoBadge, 1, 0);

        // _cardPanel
        _cardPanel.Dock = DockStyle.Fill;
        _cardPanel.AutoScroll = true;
        _cardPanel.BackColor = UiStyle.Surface;
        _cardPanel.BorderStyle = BorderStyle.FixedSingle;
        _cardPanel.Padding = new Padding(20, 16, 20, 16);
        _cardPanel.Margin = new Padding(0, 0, 0, 16);

        // _formLayout
        _formLayout.Dock = DockStyle.Top;
        _formLayout.AutoSize = true;
        _formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _formLayout.BackColor = UiStyle.Surface;
        _formLayout.ColumnCount = 1;
        _formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _formLayout.RowCount = 8;
        for (var i = 0; i < 8; i++)
        {
            _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        // _lblSubjectInfo
        _lblSubjectInfo.AutoSize = true;
        _lblSubjectInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _lblSubjectInfo.ForeColor = UiStyle.Primary;
        _lblSubjectInfo.Margin = new Padding(0, 0, 0, 12);
        _lblSubjectInfo.Text = "Môn học: —";

        // _lblNumber
        _lblNumber.AutoSize = true;
        _lblNumber.Font = UiStyle.BodyFont;
        _lblNumber.ForeColor = UiStyle.Text;
        _lblNumber.Margin = new Padding(0, 0, 0, 6);
        _lblNumber.Text = "Chương số *";

        // _numChapterNumber
        _numChapterNumber.Width = 140;
        _numChapterNumber.Font = UiStyle.BodyFont;
        _numChapterNumber.ForeColor = UiStyle.Text;
        _numChapterNumber.BackColor = UiStyle.Surface;
        _numChapterNumber.Minimum = 1;
        _numChapterNumber.Maximum = 50;
        _numChapterNumber.Value = 1;
        _numChapterNumber.Margin = new Padding(0, 0, 0, 4);
        _numChapterNumber.TabIndex = 0;
        _numChapterNumber.ValueChanged += OnNumberChanged;

        // _lblNumberHint
        _lblNumberHint.AutoSize = true;
        _lblNumberHint.Font = UiStyle.SmallFont;
        _lblNumberHint.ForeColor = UiStyle.TextMuted;
        _lblNumberHint.Margin = new Padding(0, 0, 0, 4);
        _lblNumberHint.Text = "Số thứ tự chương từ 1 đến 50, không trùng trong cùng một môn học.";

        // _lblNumberError
        _lblNumberError.AutoSize = true;
        _lblNumberError.Dock = DockStyle.Fill;
        _lblNumberError.Font = UiStyle.SmallFont;
        _lblNumberError.ForeColor = UiStyle.ErrorText;
        _lblNumberError.Margin = new Padding(0, 0, 0, 10);
        _lblNumberError.Visible = false;

        // _lblName
        _lblName.AutoSize = true;
        _lblName.Font = UiStyle.BodyFont;
        _lblName.ForeColor = UiStyle.Text;
        _lblName.Margin = new Padding(0, 8, 0, 6);
        _lblName.Text = "Tên chương *";

        // _txtName
        _txtName.Dock = DockStyle.Fill;
        _txtName.Font = UiStyle.BodyFont;
        _txtName.ForeColor = UiStyle.Text;
        _txtName.BackColor = UiStyle.Surface;
        _txtName.MaxLength = 150;
        _txtName.PlaceholderText = "Ví dụ: Nền tảng .NET và ngôn ngữ C#";
        _txtName.Margin = new Padding(0, 0, 0, 4);
        _txtName.TabIndex = 1;
        _txtName.TextChanged += OnNameTextChanged;

        // _lblNameError
        _lblNameError.AutoSize = true;
        _lblNameError.Dock = DockStyle.Fill;
        _lblNameError.Font = UiStyle.SmallFont;
        _lblNameError.ForeColor = UiStyle.ErrorText;
        _lblNameError.Margin = new Padding(0, 0, 0, 4);
        _lblNameError.Visible = false;

        _formLayout.Controls.Add(_lblSubjectInfo, 0, 0);
        _formLayout.Controls.Add(_lblNumber, 0, 1);
        _formLayout.Controls.Add(_numChapterNumber, 0, 2);
        _formLayout.Controls.Add(_lblNumberHint, 0, 3);
        _formLayout.Controls.Add(_lblNumberError, 0, 4);
        _formLayout.Controls.Add(_lblName, 0, 5);
        _formLayout.Controls.Add(_txtName, 0, 6);
        _formLayout.Controls.Add(_lblNameError, 0, 7);

        _cardPanel.Controls.Add(_formLayout);

        // _footerFlow
        _footerFlow.Dock = DockStyle.Fill;
        _footerFlow.AutoSize = true;
        _footerFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _footerFlow.FlowDirection = FlowDirection.RightToLeft;
        _footerFlow.WrapContents = false;
        _footerFlow.Margin = new Padding(0);

        // _btnSave
        _btnSave.Width = 130;
        _btnSave.Height = 40;
        _btnSave.Margin = new Padding(0);
        _btnSave.TabIndex = 2;
        _btnSave.Text = "Lưu (Ctrl+S)";
        UiStyle.StylePrimaryButton(_btnSave);
        _btnSave.Click += OnSaveClick;

        // _btnCancel
        _btnCancel.Width = 100;
        _btnCancel.Height = 40;
        _btnCancel.Margin = new Padding(0, 0, 12, 0);
        _btnCancel.TabIndex = 3;
        _btnCancel.Text = "Hủy";
        UiStyle.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += OnCancelClick;

        _footerFlow.Controls.Add(_btnSave);
        _footerFlow.Controls.Add(_btnCancel);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_cardPanel, 0, 1);
        _rootLayout.Controls.Add(_footerFlow, 0, 2);

        // ChapterEditDialog
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(520, 380);
        MinimumSize = new Size(460, 360);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
        Controls.Add(_rootLayout);
        Name = "ChapterEditDialog";
        Text = "Thêm chương";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _cardPanel.ResumeLayout(false);
        _cardPanel.PerformLayout();
        _formLayout.ResumeLayout(false);
        _formLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_numChapterNumber).EndInit();
        _footerFlow.ResumeLayout(false);
        _footerFlow.PerformLayout();
        ResumeLayout(false);
    }
}
