using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Bạn 3: nhận diện MSSV và chọn ca thật. Chưa xác thực bằng mật khẩu hoặc phân công theo lớp.
public sealed class StudentIdentityControl : UserControl
{
    private readonly StudentExamService _service = new();
    private readonly TextBox _code = new() { Width = 280, MaxLength = 50, AccessibleName = "Mã sinh viên", PlaceholderText = "Ví dụ: 24130151", TabIndex = 0 };
    private readonly Label _identity = StudentLayout.Label("");
    private readonly Label _status = StudentLayout.Label("");
    private readonly Button _view;
    private readonly TableLayoutPanel _page = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(24), BackColor = UiStyle.Surface };
    private User? _student;
    public event Action<User>? StudentIdentified;
    public event EventHandler? CloseRequested;

    public StudentIdentityControl()
    {
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        AutoScroll = true;
        _page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(StudentLayout.Label("Nhận diện sinh viên và chọn ca", 18, FontStyle.Bold));
        AddRow(StudentLayout.Label("Nhập mã sinh viên để xác nhận thông tin trước khi chọn ca thi."));
        AddRow(StudentLayout.Label("Mã sinh viên *"));
        var lookup = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = Padding.Empty, TabIndex = 0 };
        _code.Margin = new Padding(0, 6, 12, 8);
        var find = StudentLayout.Button("Tra cứu sinh viên", (_, _) => Identify(), true);
        find.TabIndex = 1;
        lookup.Controls.Add(_code); lookup.Controls.Add(find);
        AddRow(lookup);
        _identity.Visible = false;
        _identity.ForeColor = UiStyle.SuccessText;
        AddRow(_identity);
        // Danh sách ca được hiển thị ở màn chọn ca riêng.

        AddRow(_status);
        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = Padding.Empty };
        _view = StudentLayout.Button("Tiếp tục chọn ca →", (_, _) => SelectSession(), true);
        actions.Controls.Add(_view);
        AddRow(actions);
        var footer = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = new Padding(0, 24, 0, 0) };
        footer.Controls.Add(StudentLayout.Button("Đóng khu sinh viên", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty)));
        AddRow(footer);
        Controls.Add(_page);
        ResetLookup();
        _code.TextChanged += (_, _) => ResetLookup();
        _code.KeyDown += (_, e) => { if(e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Identify(); } };
        _page.SizeChanged += (_, _) => WrapLabels();
        Resize += (_, _) => FitPage();
        Load += (_, _) => { FitPage(); _code.Focus(); };
    }

    private void AddRow(Control control)
    {
        int row = _page.RowCount++;
        _page.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        control.TabIndex = row;
        _page.Controls.Add(control, 0, row);
    }

    private void FitPage()
    {
        // Chỉ màn này có khung giới hạn chiều rộng; không thay layout chung của màn thi.
        int inset = (int)(24 * DeviceDpi / 96f);
        int side = Math.Max(inset, (ClientSize.Width - (int)(900 * DeviceDpi / 96f)) / 2);
        Padding = new Padding(side, inset, side, inset);
        _code.Width = Math.Min((int)(280 * DeviceDpi / 96f), Math.Max(120, ClientSize.Width - Padding.Horizontal - _page.Padding.Horizontal - 24));
        WrapLabels();
    }

    private void WrapLabels()
    {
        int width = Math.Max(1, _page.ClientSize.Width - _page.Padding.Horizontal - 12);
        foreach(Control control in _page.Controls)
            if(control is Label label) label.MaximumSize = new Size(width, 0);
    }

    private void ResetLookup()
    {
        _student = null;
        _identity.Text = ""; _identity.Visible = false;
        _view.Enabled = false;
        _status.ForeColor = UiStyle.TextMuted;
        _status.Text = "Nhập MSSV và bấm Tra cứu sinh viên hoặc nhấn Enter.";
    }

    private void Identify()
    {
        ResetLookup();
        try
        {
            var student = _service.IdentifyStudent(_code.Text);
            _student = student;
            _identity.Text = $"{student.FullName} · {student.StudentCode} · Lớp {student.ClassName}";
            _identity.Visible = true;
            _view.Enabled = true;
            _status.Text = "Đã tìm thấy sinh viên. Bấm Tiếp tục chọn ca để xem danh sách ca thi.";
            _view.Focus();
        }
        catch (Exception ex)
        {
            ShowError(StudentError.Describe(ex, "Không truy cập được thông tin thi. Hãy thử tra cứu lại hoặc liên hệ giảng viên."));
            _code.Focus();
        }
    }

    private void SelectSession()
    {
        if (_student is null) return;
        try
        {
            StudentIdentified?.Invoke(_student);
        }
        catch (Exception ex) { ShowError(StudentError.Describe(ex, "Không truy cập được thông tin thi. Hãy thử tra cứu lại hoặc liên hệ giảng viên.")); }
    }
    private void ShowError(string message) { _status.ForeColor = UiStyle.ErrorText; _status.Text = message; }
}
