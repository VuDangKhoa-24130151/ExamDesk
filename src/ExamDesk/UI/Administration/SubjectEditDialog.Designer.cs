#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class SubjectEditDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private Panel _cardPanel = null!;
    private TableLayoutPanel _formLayout = null!;
    private Label _lblCode = null!;
    private TextBox _txtCode = null!;
    private Label _lblCodeHint = null!;
    private Label _lblCodeError = null!;
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
        _lblCode = new Label();
        _txtCode = new TextBox();
        _lblCodeHint = new Label();
        _lblCodeError = new Label();
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
        _lblTitle.Text = "Thêm môn học";

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
        _formLayout.RowCount = 7;
        for (var i = 0; i < 7; i++)
        {
            _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        // _lblCode
        _lblCode.AutoSize = true;
        _lblCode.Font = UiStyle.BodyFont;
        _lblCode.ForeColor = UiStyle.Text;
        _lblCode.Margin = new Padding(0, 0, 0, 6);
        _lblCode.Text = "Mã môn *";

        // _txtCode
        _txtCode.Dock = DockStyle.Fill;
        _txtCode.Font = UiStyle.BodyFont;
        _txtCode.ForeColor = UiStyle.Text;
        _txtCode.BackColor = UiStyle.Surface;
        _txtCode.CharacterCasing = CharacterCasing.Upper;
        _txtCode.MaxLength = 20;
        _txtCode.PlaceholderText = "Ví dụ: LTNET, CSDL, KTPM…";
        _txtCode.Margin = new Padding(0, 0, 0, 4);
        _txtCode.TabIndex = 0;
        _txtCode.TextChanged += OnCodeTextChanged;

        // _lblCodeHint
        _lblCodeHint.AutoSize = true;
        _lblCodeHint.Font = UiStyle.SmallFont;
        _lblCodeHint.ForeColor = UiStyle.TextMuted;
        _lblCodeHint.Margin = new Padding(0, 0, 0, 4);
        _lblCodeHint.Text = "2–10 ký tự chữ in hoa (A–Z) hoặc chữ số (0–9), không khoảng trắng, không trùng.";

        // _lblCodeError
        _lblCodeError.AutoSize = true;
        _lblCodeError.Dock = DockStyle.Fill;
        _lblCodeError.Font = UiStyle.SmallFont;
        _lblCodeError.ForeColor = UiStyle.ErrorText;
        _lblCodeError.Margin = new Padding(0, 0, 0, 10);
        _lblCodeError.Visible = false;

        // _lblName
        _lblName.AutoSize = true;
        _lblName.Font = UiStyle.BodyFont;
        _lblName.ForeColor = UiStyle.Text;
        _lblName.Margin = new Padding(0, 8, 0, 6);
        _lblName.Text = "Tên môn *";

        // _txtName
        _txtName.Dock = DockStyle.Fill;
        _txtName.Font = UiStyle.BodyFont;
        _txtName.ForeColor = UiStyle.Text;
        _txtName.BackColor = UiStyle.Surface;
        _txtName.MaxLength = 150;
        _txtName.PlaceholderText = "Ví dụ: Lập trình .NET";
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

        _formLayout.Controls.Add(_lblCode, 0, 0);
        _formLayout.Controls.Add(_txtCode, 0, 1);
        _formLayout.Controls.Add(_lblCodeHint, 0, 2);
        _formLayout.Controls.Add(_lblCodeError, 0, 3);
        _formLayout.Controls.Add(_lblName, 0, 4);
        _formLayout.Controls.Add(_txtName, 0, 5);
        _formLayout.Controls.Add(_lblNameError, 0, 6);

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

        // SubjectEditDialog
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(520, 360);
        MinimumSize = new Size(460, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
        Controls.Add(_rootLayout);
        Name = "SubjectEditDialog";
        Text = "Thêm môn học";

        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _headerLayout.ResumeLayout(false);
        _headerLayout.PerformLayout();
        _cardPanel.ResumeLayout(false);
        _cardPanel.PerformLayout();
        _formLayout.ResumeLayout(false);
        _formLayout.PerformLayout();
        _footerFlow.ResumeLayout(false);
        _footerFlow.PerformLayout();
        ResumeLayout(false);
    }
}
