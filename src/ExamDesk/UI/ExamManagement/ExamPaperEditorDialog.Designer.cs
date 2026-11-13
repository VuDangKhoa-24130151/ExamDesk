#nullable enable
namespace ExamDesk.UI.ExamManagement;

partial class ExamPaperEditorDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Label _lblDialogTitle = null!;
    private TableLayoutPanel _formTable = null!;

    private Label _lblPaperCode = null!;
    private TextBox _txtPaperCode = null!;

    private Label _lblTitle = null!;
    private TextBox _txtTitle = null!;

    private Label _lblSubject = null!;
    private ComboBox _cboSubject = null!;

    private Label _lblDuration = null!;
    private NumericUpDown _numDuration = null!;

    private Label _lblEasy = null!;
    private NumericUpDown _numEasy = null!;
    private Label _lblEasyAvailable = null!;
    private Label _lblEasyError = null!;

    private Label _lblMedium = null!;
    private NumericUpDown _numMedium = null!;
    private Label _lblMediumAvailable = null!;
    private Label _lblMediumError = null!;

    private Label _lblHard = null!;
    private NumericUpDown _numHard = null!;
    private Label _lblHardAvailable = null!;
    private Label _lblHardError = null!;

    private Label _lblTotalSummary = null!;
    private Label _lblFormError = null!;

    private FlowLayoutPanel _buttonPanel = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _lblDialogTitle = new Label();
        _formTable = new TableLayoutPanel();

        _lblPaperCode = new Label();
        _txtPaperCode = new TextBox();

        _lblTitle = new Label();
        _txtTitle = new TextBox();

        _lblSubject = new Label();
        _cboSubject = new ComboBox();

        _lblDuration = new Label();
        _numDuration = new NumericUpDown();

        _lblEasy = new Label();
        _numEasy = new NumericUpDown();
        _lblEasyAvailable = new Label();
        _lblEasyError = new Label();

        _lblMedium = new Label();
        _numMedium = new NumericUpDown();
        _lblMediumAvailable = new Label();
        _lblMediumError = new Label();

        _lblHard = new Label();
        _numHard = new NumericUpDown();
        _lblHardAvailable = new Label();
        _lblHardError = new Label();

        _lblTotalSummary = new Label();
        _lblFormError = new Label();

        _buttonPanel = new FlowLayoutPanel();
        _btnSave = new Button();
        _btnCancel = new Button();

        ((System.ComponentModel.ISupportInitialize)_numDuration).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_numEasy).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_numMedium).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_numHard).BeginInit();
        SuspendLayout();

        // Root layout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.ColumnCount = 1;
        _rootLayout.RowCount = 5;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Title
        _lblDialogTitle.AutoSize = true;
        _lblDialogTitle.Text = "Cấu hình tạo đề thi mới";
        _lblDialogTitle.Margin = new Padding(0, 0, 0, 16);

        // Form table (3 columns: Label | Input | Available info)
        _formTable.Dock = DockStyle.Top;
        _formTable.AutoSize = true;
        _formTable.ColumnCount = 3;
        _formTable.RowCount = 10;
        _formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165F));
        _formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _formTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        // Row 0: Mã đề
        _lblPaperCode.AutoSize = true;
        _lblPaperCode.Anchor = AnchorStyles.Left;
        _lblPaperCode.Text = "Mã đề *";
        _lblPaperCode.Margin = new Padding(0, 6, 12, 6);

        _txtPaperCode.Dock = DockStyle.Fill;
        _txtPaperCode.MaxLength = 50;
        _txtPaperCode.PlaceholderText = "Ví dụ: DE01";
        _txtPaperCode.Margin = new Padding(0, 4, 0, 8);
        _formTable.SetColumnSpan(_txtPaperCode, 2);

        // Row 1: Tiêu đề
        _lblTitle.AutoSize = true;
        _lblTitle.Anchor = AnchorStyles.Left;
        _lblTitle.Text = "Tiêu đề đề thi *";
        _lblTitle.Margin = new Padding(0, 6, 12, 6);

        _txtTitle.Dock = DockStyle.Fill;
        _txtTitle.MaxLength = 200;
        _txtTitle.PlaceholderText = "Ví dụ: Đề kiểm tra giữa kỳ";
        _txtTitle.Margin = new Padding(0, 4, 0, 8);
        _formTable.SetColumnSpan(_txtTitle, 2);

        // Row 2: Môn học
        _lblSubject.AutoSize = true;
        _lblSubject.Anchor = AnchorStyles.Left;
        _lblSubject.Text = "Môn học *";
        _lblSubject.Margin = new Padding(0, 6, 12, 6);

        _cboSubject.Dock = DockStyle.Fill;
        _cboSubject.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboSubject.Margin = new Padding(0, 4, 0, 8);
        _formTable.SetColumnSpan(_cboSubject, 2);

        // Row 3: Thời lượng
        _lblDuration.AutoSize = true;
        _lblDuration.Anchor = AnchorStyles.Left;
        _lblDuration.Text = "Thời lượng (phút) *";
        _lblDuration.Margin = new Padding(0, 6, 12, 6);

        _numDuration.Minimum = 5;
        _numDuration.Maximum = 180;
        _numDuration.Value = 45;
        _numDuration.Width = 140;
        _numDuration.Anchor = AnchorStyles.Left;
        _numDuration.Margin = new Padding(0, 4, 0, 8);

        // Row 4: Số câu Dễ
        _lblEasy.AutoSize = true;
        _lblEasy.Anchor = AnchorStyles.Left;
        _lblEasy.Text = "Số câu Dễ";
        _lblEasy.Margin = new Padding(0, 6, 12, 2);

        _numEasy.Minimum = 0;
        _numEasy.Maximum = 500;
        _numEasy.Value = 0;
        _numEasy.Width = 140;
        _numEasy.Anchor = AnchorStyles.Left;
        _numEasy.Margin = new Padding(0, 4, 12, 2);

        _lblEasyAvailable.AutoSize = true;
        _lblEasyAvailable.Anchor = AnchorStyles.Left;
        _lblEasyAvailable.Text = "Có sẵn: 0 câu";
        _lblEasyAvailable.Margin = new Padding(0, 6, 0, 2);

        // Row 5: Lỗi Dễ
        _lblEasyError.AutoSize = true;
        _lblEasyError.Anchor = AnchorStyles.Left;
        _lblEasyError.Text = string.Empty;
        _lblEasyError.Visible = false;
        _lblEasyError.Margin = new Padding(0, 0, 0, 6);
        _formTable.SetColumnSpan(_lblEasyError, 2);

        // Row 6: Số câu Trung bình
        _lblMedium.AutoSize = true;
        _lblMedium.Anchor = AnchorStyles.Left;
        _lblMedium.Text = "Số câu Trung bình";
        _lblMedium.Margin = new Padding(0, 6, 12, 2);

        _numMedium.Minimum = 0;
        _numMedium.Maximum = 500;
        _numMedium.Value = 0;
        _numMedium.Width = 140;
        _numMedium.Anchor = AnchorStyles.Left;
        _numMedium.Margin = new Padding(0, 4, 12, 2);

        _lblMediumAvailable.AutoSize = true;
        _lblMediumAvailable.Anchor = AnchorStyles.Left;
        _lblMediumAvailable.Text = "Có sẵn: 0 câu";
        _lblMediumAvailable.Margin = new Padding(0, 6, 0, 2);

        // Row 7: Lỗi Trung bình
        _lblMediumError.AutoSize = true;
        _lblMediumError.Anchor = AnchorStyles.Left;
        _lblMediumError.Text = string.Empty;
        _lblMediumError.Visible = false;
        _lblMediumError.Margin = new Padding(0, 0, 0, 6);
        _formTable.SetColumnSpan(_lblMediumError, 2);

        // Row 8: Số câu Khó
        _lblHard.AutoSize = true;
        _lblHard.Anchor = AnchorStyles.Left;
        _lblHard.Text = "Số câu Khó";
        _lblHard.Margin = new Padding(0, 6, 12, 2);

        _numHard.Minimum = 0;
        _numHard.Maximum = 500;
        _numHard.Value = 0;
        _numHard.Width = 140;
        _numHard.Anchor = AnchorStyles.Left;
        _numHard.Margin = new Padding(0, 4, 12, 2);

        _lblHardAvailable.AutoSize = true;
        _lblHardAvailable.Anchor = AnchorStyles.Left;
        _lblHardAvailable.Text = "Có sẵn: 0 câu";
        _lblHardAvailable.Margin = new Padding(0, 6, 0, 2);

        // Row 9: Lỗi Khó
        _lblHardError.AutoSize = true;
        _lblHardError.Anchor = AnchorStyles.Left;
        _lblHardError.Text = string.Empty;
        _lblHardError.Visible = false;
        _lblHardError.Margin = new Padding(0, 0, 0, 6);
        _formTable.SetColumnSpan(_lblHardError, 2);

        _formTable.Controls.Add(_lblPaperCode, 0, 0);
        _formTable.Controls.Add(_txtPaperCode, 1, 0);
        _formTable.Controls.Add(_lblTitle, 0, 1);
        _formTable.Controls.Add(_txtTitle, 1, 1);
        _formTable.Controls.Add(_lblSubject, 0, 2);
        _formTable.Controls.Add(_cboSubject, 1, 2);
        _formTable.Controls.Add(_lblDuration, 0, 3);
        _formTable.Controls.Add(_numDuration, 1, 3);
        _formTable.Controls.Add(_lblEasy, 0, 4);
        _formTable.Controls.Add(_numEasy, 1, 4);
        _formTable.Controls.Add(_lblEasyAvailable, 2, 4);
        _formTable.Controls.Add(_lblEasyError, 1, 5);
        _formTable.Controls.Add(_lblMedium, 0, 6);
        _formTable.Controls.Add(_numMedium, 1, 6);
        _formTable.Controls.Add(_lblMediumAvailable, 2, 6);
        _formTable.Controls.Add(_lblMediumError, 1, 7);
        _formTable.Controls.Add(_lblHard, 0, 8);
        _formTable.Controls.Add(_numHard, 1, 8);
        _formTable.Controls.Add(_lblHardAvailable, 2, 8);
        _formTable.Controls.Add(_lblHardError, 1, 9);

        // Total summary
        _lblTotalSummary.AutoSize = true;
        _lblTotalSummary.Text = "Tổng số câu: 0";
        _lblTotalSummary.Margin = new Padding(0, 12, 0, 8);

        // Form error message
        _lblFormError.Dock = DockStyle.Top;
        _lblFormError.AutoSize = true;
        _lblFormError.Padding = new Padding(10, 8, 10, 8);
        _lblFormError.Margin = new Padding(0, 4, 0, 12);
        _lblFormError.Visible = false;

        // Bottom buttons
        _buttonPanel.Dock = DockStyle.Fill;
        _buttonPanel.AutoSize = true;
        _buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        _buttonPanel.WrapContents = false;
        _buttonPanel.Margin = new Padding(0, 8, 0, 0);

        _btnCancel.Text = "Hủy";
        _btnCancel.Margin = new Padding(8, 0, 0, 0);

        _btnSave.Text = "Tạo đề";
        _btnSave.Margin = new Padding(0);

        _buttonPanel.Controls.Add(_btnCancel);
        _buttonPanel.Controls.Add(_btnSave);

        _rootLayout.Controls.Add(_lblDialogTitle, 0, 0);
        _rootLayout.Controls.Add(_formTable, 0, 1);
        _rootLayout.Controls.Add(_lblTotalSummary, 0, 2);
        _rootLayout.Controls.Add(_lblFormError, 0, 3);
        _rootLayout.Controls.Add(_buttonPanel, 0, 4);

        Controls.Add(_rootLayout);

        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(640, 540);
        MinimumSize = new Size(560, 480);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ExamPaperEditorDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Tạo đề thi mới";

        ((System.ComponentModel.ISupportInitialize)_numDuration).EndInit();
        ((System.ComponentModel.ISupportInitialize)_numEasy).EndInit();
        ((System.ComponentModel.ISupportInitialize)_numMedium).EndInit();
        ((System.ComponentModel.ISupportInitialize)_numHard).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
