#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

partial class SessionEditDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _editor = null!;
    private Label _lblName = null!;
    private TextBox _txtName = null!;
    private Label _lblRoom = null!;
    private TextBox _txtRoom = null!;
    private Label _lblPaper = null!;
    private ComboBox _cboPaper = null!;
    private Label _lblStatus = null!;
    private FlowLayoutPanel _actions = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        Text = "Sửa ca thi";
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        ClientSize = new Size(560, 240);
        MinimumSize = new Size(400, 250);
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Surface;
        MinimizeBox = false;
        MaximizeBox = false;
        AutoScroll = true;

        _txtName = new TextBox { MaxLength = 200 };
        _txtRoom = new TextBox { MaxLength = 100 };
        _cboPaper = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

        _editor = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            ColumnCount = 2,
            Padding = new Padding(16)
        };
        _editor.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _lblName = new Label { Text = "Tên ca *", AutoSize = true, Anchor = AnchorStyles.Left };
        _txtName.AccessibleName = "Tên ca *";
        _txtName.Dock = DockStyle.Fill;

        _lblRoom = new Label { Text = "Phòng thi *", AutoSize = true, Anchor = AnchorStyles.Left };
        _txtRoom.AccessibleName = "Phòng thi *";
        _txtRoom.Dock = DockStyle.Fill;

        _lblPaper = new Label { Text = "Đề thi *", AutoSize = true, Anchor = AnchorStyles.Left };
        _cboPaper.AccessibleName = "Đề thi *";
        _cboPaper.Dock = DockStyle.Fill;

        _editor.Controls.Add(_lblName, 0, 0);
        _editor.Controls.Add(_txtName, 1, 0);
        _editor.Controls.Add(_lblRoom, 0, 1);
        _editor.Controls.Add(_txtRoom, 1, 1);
        _editor.Controls.Add(_lblPaper, 0, 2);
        _editor.Controls.Add(_cboPaper, 1, 2);

        _lblStatus = new Label
        {
            AutoSize = true,
            ForeColor = UiStyle.ErrorText,
            Margin = new Padding(0, 8, 0, 8)
        };
        _editor.Controls.Add(_lblStatus, 0, 3);
        _editor.SetColumnSpan(_lblStatus, 2);

        _actions = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            WrapContents = true
        };
        _btnSave = new Button { Text = "Lưu thay đổi" };
        _btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel };
        UiStyle.StylePrimaryButton(_btnSave);
        UiStyle.StyleSecondaryButton(_btnCancel);
        _actions.Controls.AddRange([_btnSave, _btnCancel]);
        _editor.Controls.Add(_actions, 0, 4);
        _editor.SetColumnSpan(_actions, 2);

        Controls.Add(_editor);
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
        ResumeLayout(false);
    }
}
