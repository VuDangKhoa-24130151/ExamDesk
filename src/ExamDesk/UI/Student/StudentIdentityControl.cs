using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Bạn 3: nhận diện MSSV và chọn ca thật. Chưa xác thực bằng mật khẩu hoặc phân công theo lớp.
public sealed class StudentIdentityControl : UserControl
{
    private readonly StudentExamService _service = new();
    private readonly TextBox _code = new()
    {
        AutoSize = true, Dock = DockStyle.Top, MaxLength = 50,
        AccessibleName = "Mã sinh viên", PlaceholderText = "Ví dụ: 24130151", Margin = Padding.Empty
    };
    private readonly Label _identity = StudentLayout.Label("");
    private readonly Label _status = StudentLayout.Label("");
    private readonly Button _view;
    private readonly CardPanel _card = new() { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(24) };
    private readonly TableLayoutPanel _page = new()
    {
        Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = Padding.Empty
    };
    private User? _student;
    public event Action<User>? StudentIdentified;
    public event EventHandler? CloseRequested;

    public StudentIdentityControl()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7, 17);
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        AutoScroll = true;
        _page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var title = StudentLayout.Label("Nhận diện sinh viên và chọn ca", 18, FontStyle.Bold);
        title.Margin = new Padding(0, 0, 0, 8);
        AddRow(title);
        var subtitle = StudentLayout.Label("Nhập mã sinh viên để xác nhận thông tin trước khi chọn ca thi.");
        subtitle.Margin = new Padding(0, 0, 0, 12);
        AddRow(subtitle);
        var codeLabel = StudentLayout.Label("Mã sinh viên *");
        codeLabel.Margin = new Padding(0, 0, 0, 4);
        AddRow(codeLabel);
        AddRow(_code);
        _status.Font = UiStyle.SmallFont;
        _status.Margin = new Padding(0, 4, 0, 12);
        AddRow(_status);
        _identity.Visible = false;
        _identity.ForeColor = UiStyle.SuccessText;
        _identity.Margin = new Padding(0, 0, 0, 12);
        AddRow(_identity);

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = Padding.Empty };
        var find = StudentLayout.Button("Tra cứu sinh viên", (_, _) => Identify(), true);
        _view = StudentLayout.Button("Tiếp tục chọn ca →", (_, _) => SelectSession());
        find.Margin = new Padding(0, 0, 8, 8);
        _view.Margin = new Padding(0, 0, 0, 8);
        // Chuyển nút chính theo bước hiện tại, giữ cả hai thao tác.
        _view.EnabledChanged += (_, _) =>
        {
            if (_view.Enabled)
            {
                UiStyle.StyleSecondaryButton(find);
                UiStyle.StylePrimaryButton(_view);
            }
            else
            {
                UiStyle.StylePrimaryButton(find);
                UiStyle.StyleSecondaryButton(_view);
            }
        };
        actions.Controls.Add(find);
        actions.Controls.Add(_view);
        AddRow(actions);
        var close = StudentLayout.Button("Đóng khu sinh viên", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty));
        close.Margin = new Padding(0, 8, 0, 0);
        AddRow(close);
        _card.Controls.Add(_page);
        Controls.Add(_card);
        ResetLookup();
        _code.TextChanged += (_, _) => ResetLookup();
        _code.KeyDown += (_, e) => { if(e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Identify(); } };
        _page.SizeChanged += (_, _) => WrapLabels();
        _card.SizeChanged += (_, _) => FitPage();
        Resize += (_, _) => FitPage();
        DpiChangedAfterParent += (_, _) => FitPage();
        Load += (_, _) => { FitPage(); _code.Focus(); };
        ResumeLayout(true);
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
        int inset = StudentLayout.Scale(this, 24);
        int side = Math.Max(inset, (ClientSize.Width - StudentLayout.Scale(this, 460)) / 2);
        // Căn giữa theo chiều dọc nếu đủ chỗ, chừa lề cuộn khi thẻ cao hơn vùng xem.
        int vertical = _card.Height <= ClientSize.Height ? (ClientSize.Height - _card.Height) / 2 : inset;
        var padding = new Padding(side, vertical, side, vertical);
        if (Padding != padding) Padding = padding;
    }

    private void WrapLabels()
    {
        int width = Math.Max(1, _page.ClientSize.Width);
        foreach (var label in _page.Controls.OfType<Label>())
            label.MaximumSize = new Size(width, 0);
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
