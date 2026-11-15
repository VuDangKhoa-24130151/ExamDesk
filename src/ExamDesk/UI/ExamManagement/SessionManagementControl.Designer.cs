#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

partial class SessionManagementControl
{
    private System.ComponentModel.IContainer? components = null;
    private readonly DataGridView _grid = new();
    private readonly TextBox _code = new();
    private readonly TextBox _name = new();
    private readonly TextBox _room = new();
    private readonly ComboBox _paper = new();
    private readonly Button _create = new() { Text = "Lưu ca mới", AutoSize = true };
    private readonly Button _edit = new() { Text = "Sửa ca", AutoSize = true, Enabled = false };
    private readonly Button _toggle = new() { Text = "Mở ca", AutoSize = true, Enabled = false };
    private readonly Button _refresh = new() { Text = "Tải lại", AutoSize = true };
    private readonly Label _status = new() { AutoSize = true, ForeColor = UiStyle.TextMuted };
    private readonly TextBox _search = new() { AccessibleName = "Tìm ca theo mã, tên hoặc phòng", PlaceholderText = "Tìm theo mã ca, tên ca hoặc phòng" };
    private readonly ComboBox _stateFilter = new() { AccessibleName = "Lọc theo trạng thái", DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _filterStatus = new() { AutoSize = true, ForeColor = UiStyle.TextMuted };

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        Padding = new Padding(24);
        AutoScroll = true;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, MinimumSize = new Size(540, 480) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 7; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(new Label { Text = "Quản lý ca thi", Font = UiStyle.TitleFont, AutoSize = true, Margin = new Padding(0, 0, 0, 16) }, 0, 0);

        var editor = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Top, ColumnCount = 2, RowCount = 4 };
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        string[] labels = ["Mã ca *", "Tên ca *", "Phòng thi *", "Đề thi *"];
        Control[] fields = [_code, _name, _room, _paper];
        for (int i = 0; i < fields.Length; i++)
        {
            editor.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            editor.Controls.Add(new Label { Text = labels[i], AutoSize = true, Margin = new Padding(0, 8, 16, 8) }, 0, i);
            fields[i].Dock = DockStyle.Fill;
            fields[i].Margin = new Padding(0, 4, 0, 4);
            fields[i].TabIndex = i;
            fields[i].AccessibleName = labels[i];
            editor.Controls.Add(fields[i], 1, i);
        }
        _paper.DropDownStyle = ComboBoxStyle.DropDownList;
        _code.MaxLength = 50;
        _name.MaxLength = 200;
        _room.MaxLength = 100;
        layout.Controls.Add(editor, 0, 1);
        layout.Controls.Add(new Label { Text = "Ca mới luôn ở trạng thái đóng. Lưu bằng Enter hoặc Ctrl+S.",
            AutoSize = true, ForeColor = UiStyle.TextMuted }, 0, 2);

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = new Padding(0, 12, 0, 12) };
        UiStyle.StylePrimaryButton(_create);
        UiStyle.StyleSecondaryButton(_edit);
        UiStyle.StyleSecondaryButton(_toggle);
        UiStyle.StyleSecondaryButton(_refresh);
        actions.Controls.AddRange([_create, _edit, _toggle, _refresh]);
        layout.Controls.Add(actions, 0, 3);
        _status.Margin = new Padding(0, 0, 0, 12);
        layout.Controls.Add(_status, 0, 4);
        var filters = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Top, ColumnCount = 4, RowCount = 1 };
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        filters.Controls.Add(new Label { Text = "Tìm ca", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _search.Dock = DockStyle.Fill;
        filters.Controls.Add(_search, 1, 0);
        filters.Controls.Add(new Label { Text = "Trạng thái", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        _stateFilter.Dock = DockStyle.Fill;
        _stateFilter.Items.AddRange(["Tất cả", "Đang mở", "Đóng"]);
        _stateFilter.SelectedIndex = 0;
        filters.Controls.Add(_stateFilter, 3, 0);
        layout.Controls.Add(filters, 0, 5);
        layout.Controls.Add(_filterStatus, 0, 6);
        UiStyle.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        foreach (var column in new[] { ("Code", "Mã ca"), ("Name", "Tên ca"), ("Paper", "Đề thi"), ("Room", "Phòng"), ("State", "Trạng thái") })
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = column.Item1, HeaderText = column.Item2, MinimumWidth = 90 });
        layout.Controls.Add(_grid, 0, 7);
        Controls.Add(layout);
        Name = "SessionManagementControl";
        Size = new Size(1000, 600);
        ResumeLayout(false);
    }
}
