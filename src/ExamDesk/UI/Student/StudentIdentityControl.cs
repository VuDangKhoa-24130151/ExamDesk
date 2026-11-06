using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

// Bạn 3: nhận diện MSSV và chọn ca thật. Chưa xác thực bằng mật khẩu hoặc phân công theo lớp.
public sealed class StudentIdentityControl : UserControl
{
    private readonly StudentExamService _service = new();
    private readonly TextBox _code = new() { Width = 320, MaxLength = 50 };
    private readonly Label _identity = StudentLayout.Label("");
    private readonly Label _status = StudentLayout.Label("");
    private readonly ComboBox _sessions = new() { Width = 520, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _view;
    private User? _student;
    public event Action<User, ExamSession, ExamPaper>? SessionSelected;
    public event EventHandler? CloseRequested;
    public event EventHandler? PreviewRequested;
    private sealed record Choice(ExamSession Session) { public override string ToString() => $"{Session.SessionCode} — {Session.SessionName} — Phòng {Session.RoomName}"; }

    public StudentIdentityControl()
    {
        var page = StudentLayout.Page(this, "Nhận diện sinh viên và chọn ca");
        page.Controls.Add(StudentLayout.Label("Mã sinh viên *"));
        _code.AccessibleName = "Mã sinh viên";
        page.Controls.Add(_code);
        page.Controls.Add(StudentLayout.Button("Tra cứu sinh viên / Tải lại ca", (_, _) => Identify(), true));
        page.Controls.Add(_identity);
        page.Controls.Add(StudentLayout.Label("Ca đang mở (chưa lọc theo lớp hoặc danh sách phân công)"));
        page.Controls.Add(_sessions);
        _view = StudentLayout.Button("Xem thông tin ca", (_, _) => SelectSession());
        _view.Enabled = false;
        page.Controls.Add(_view);
        page.Controls.Add(_status);
        page.Controls.Add(StudentLayout.Button("Xem thử UI làm bài (không lưu)", (_, _) => PreviewRequested?.Invoke(this, EventArgs.Empty)));
        page.Controls.Add(StudentLayout.Button("Quay lại", (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty)));
        _code.TextChanged += (_, _) => { _student = null; _identity.Text = ""; _sessions.Items.Clear(); _view.Enabled = false; };
    }

    private void Identify()
    {
        _student = null; _sessions.Items.Clear(); _view.Enabled = false; _identity.Text = "";
        try
        {
            var student = _service.IdentifyStudent(_code.Text);
            var sessions = _service.GetOpenSessions();
            _student = student;
            _identity.Text = $"{student.FullName} · {student.StudentCode} · Lớp {student.ClassName}";
            foreach (var session in sessions) _sessions.Items.Add(new Choice(session));
            if (_sessions.Items.Count > 0) { _sessions.SelectedIndex = 0; _view.Enabled = true; }
            _status.ForeColor = UiStyle.TextMuted;
            _status.Text = sessions.Count == 0 ? "Hiện chưa có ca mở. Hãy liên hệ giảng viên hoặc tải lại." : "Chọn ca để xem thông tin đề thi.";
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void SelectSession()
    {
        if (_student is null || _sessions.SelectedItem is not Choice choice) return;
        try
        {
            var paper = _service.GetSelectedPaper(choice.Session.Id, _student.StudentCode);
            SessionSelected?.Invoke(_student, choice.Session, paper);
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }
    private void ShowError(string message) { _status.ForeColor = UiStyle.ErrorText; _status.Text = message; }
}
