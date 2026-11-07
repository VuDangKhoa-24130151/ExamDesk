#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

partial class StudentEditDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private TableLayoutPanel _headerLayout = null!;
    private Label _lblTitle = null!;
    private Label _lblDemoBadge = null!;
    private Panel _cardPanel = null!;
    private TableLayoutPanel _formLayout = null!;
    private Label _lblStudentCode = null!;
    private TextBox _txtStudentCode = null!;
    private Label _lblStudentCodeHint = null!;
    private Label _lblStudentCodeError = null!;
    private Label _lblFullName = null!;
    private TextBox _txtFullName = null!;
    private Label _lblFullNameError = null!;
    private Label _lblClass = null!;
    private ComboBox _cboClass = null!;
    private Label _lblClassError = null!;
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
        _lblStudentCode = new Label();
        _txtStudentCode = new TextBox();
        _lblStudentCodeHint = new Label();
        _lblStudentCodeError = new Label();
        _lblFullName = new Label();
        _txtFullName = new TextBox();
        _lblFullNameError = new Label();
        _lblClass = new Label();
        _cboClass = new ComboBox();
        _lblClassError = new Label();
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
        _lblTitle.Text = "Thêm sinh viên";

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
        _formLayout.RowCount = 10;
        for (var i = 0; i < 10; i++)
        {
            _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        // _lblStudentCode
        _lblStudentCode.AutoSize = true;
        _lblStudentCode.Font = UiStyle.BodyFont;
        _lblStudentCode.ForeColor = UiStyle.Text;
        _lblStudentCode.Margin = new Padding(0, 0, 0, 6);
        _lblStudentCode.Text = "MSSV *";

        // _txtStudentCode
        _txtStudentCode.Dock = DockStyle.Fill;
        _txtStudentCode.Font = UiStyle.BodyFont;
        _txtStudentCode.ForeColor = UiStyle.Text;
        _txtStudentCode.BackColor = UiStyle.Surface;
        _txtStudentCode.MaxLength = 15;
        _txtStudentCode.PlaceholderText = "Ví dụ: 24130151";
        _txtStudentCode.Margin = new Padding(0, 0, 0, 4);
        _txtStudentCode.TabIndex = 0;
        _txtStudentCode.TextChanged += OnStudentCodeTextChanged;

        // _lblStudentCodeHint
        _lblStudentCodeHint.AutoSize = true;
        _lblStudentCodeHint.Font = UiStyle.SmallFont;
        _lblStudentCodeHint.ForeColor = UiStyle.TextMuted;
        _lblStudentCodeHint.Margin = new Padding(0, 0, 0, 4);
        _lblStudentCodeHint.Text = "Chỉ gồm chữ số (0–9), độ dài 8–10 ký tự, không trùng trong toàn hệ thống.";

        // _lblStudentCodeError
        _lblStudentCodeError.AutoSize = true;
        _lblStudentCodeError.Dock = DockStyle.Fill;
        _lblStudentCodeError.Font = UiStyle.SmallFont;
        _lblStudentCodeError.ForeColor = UiStyle.ErrorText;
        _lblStudentCodeError.Margin = new Padding(0, 0, 0, 8);
        _lblStudentCodeError.Visible = false;

        // _lblFullName
        _lblFullName.AutoSize = true;
        _lblFullName.Font = UiStyle.BodyFont;
        _lblFullName.ForeColor = UiStyle.Text;
        _lblFullName.Margin = new Padding(0, 8, 0, 6);
        _lblFullName.Text = "Họ và tên *";

        // _txtFullName
        _txtFullName.Dock = DockStyle.Fill;
        _txtFullName.Font = UiStyle.BodyFont;
        _txtFullName.ForeColor = UiStyle.Text;
        _txtFullName.BackColor = UiStyle.Surface;
        _txtFullName.MaxLength = 150;
        _txtFullName.PlaceholderText = "Ví dụ: Nguyễn Minh An";
        _txtFullName.Margin = new Padding(0, 0, 0, 4);
        _txtFullName.TabIndex = 1;
        _txtFullName.TextChanged += OnFullNameTextChanged;

        // _lblFullNameError
        _lblFullNameError.AutoSize = true;
        _lblFullNameError.Dock = DockStyle.Fill;
        _lblFullNameError.Font = UiStyle.SmallFont;
        _lblFullNameError.ForeColor = UiStyle.ErrorText;
        _lblFullNameError.Margin = new Padding(0, 0, 0, 8);
        _lblFullNameError.Visible = false;

        // _lblClass
        _lblClass.AutoSize = true;
        _lblClass.Font = UiStyle.BodyFont;
        _lblClass.ForeColor = UiStyle.Text;
        _lblClass.Margin = new Padding(0, 8, 0, 6);
        _lblClass.Text = "Lớp *";

        // _cboClass
        _cboClass.Dock = DockStyle.Fill;
        _cboClass.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboClass.Font = UiStyle.BodyFont;
        _cboClass.ForeColor = UiStyle.Text;
        _cboClass.BackColor = UiStyle.Surface;
        _cboClass.Margin = new Padding(0, 0, 0, 4);
        _cboClass.TabIndex = 2;
        _cboClass.SelectedIndexChanged += OnClassSelectedIndexChanged;

        // _lblClassError
        _lblClassError.AutoSize = true;
        _lblClassError.Dock = DockStyle.Fill;
        _lblClassError.Font = UiStyle.SmallFont;
        _lblClassError.ForeColor = UiStyle.ErrorText;
        _lblClassError.Margin = new Padding(0, 0, 0, 4);
        _lblClassError.Visible = false;

        _formLayout.Controls.Add(_lblStudentCode, 0, 0);
        _formLayout.Controls.Add(_txtStudentCode, 0, 1);
        _formLayout.Controls.Add(_lblStudentCodeHint, 0, 2);
        _formLayout.Controls.Add(_lblStudentCodeError, 0, 3);
        _formLayout.Controls.Add(_lblFullName, 0, 4);
        _formLayout.Controls.Add(_txtFullName, 0, 5);
        _formLayout.Controls.Add(_lblFullNameError, 0, 6);
        _formLayout.Controls.Add(_lblClass, 0, 7);
        _formLayout.Controls.Add(_cboClass, 0, 8);
        _formLayout.Controls.Add(_lblClassError, 0, 9);

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
        _btnSave.TabIndex = 3;
        _btnSave.Text = "Lưu (Ctrl+S)";
        UiStyle.StylePrimaryButton(_btnSave);
        _btnSave.Click += OnSaveClick;

        // _btnCancel
        _btnCancel.Width = 100;
        _btnCancel.Height = 40;
        _btnCancel.Margin = new Padding(0, 0, 12, 0);
        _btnCancel.TabIndex = 4;
        _btnCancel.Text = "Hủy";
        UiStyle.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += OnCancelClick;

        _footerFlow.Controls.Add(_btnSave);
        _footerFlow.Controls.Add(_btnCancel);

        _rootLayout.Controls.Add(_headerLayout, 0, 0);
        _rootLayout.Controls.Add(_cardPanel, 0, 1);
        _rootLayout.Controls.Add(_footerFlow, 0, 2);

        // StudentEditDialog
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;
        ClientSize = new Size(520, 430);
        MinimumSize = new Size(460, 400);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
        Controls.Add(_rootLayout);
        Name = "StudentEditDialog";
        Text = "Thêm sinh viên";

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
