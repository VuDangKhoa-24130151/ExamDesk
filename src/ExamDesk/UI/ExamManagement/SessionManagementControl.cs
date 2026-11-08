using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

// Bạn 3: quản lý ca thật qua Service; không dùng dữ liệu đề demo.
public partial class SessionManagementControl : UserControl, IUnsavedChangesGuard
{
    private readonly ExamSessionService _service = new();
    public SessionManagementControl() => InitializeComponent();
    private int? _defaultPaperId;
    public bool HasUnsavedChanges =>
        _code.TextLength > 0 || _name.TextLength > 0 || _room.TextLength > 0 ||
        (_paper.SelectedItem as PaperChoice)?.Id != _defaultPaperId;

    public bool ConfirmLeave() => !HasUnsavedChanges || MessageBox.Show(this,
        "Thông tin tạo ca chưa được lưu. Bạn có muốn bỏ thông tin đang nhập và rời màn hình?",
        "Rời màn quản lý ca", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
        MessageBoxDefaultButton.Button2) == DialogResult.Yes;

    private ExamSession? SelectedSession => _grid.CurrentRow?.Tag as ExamSession;
    private sealed record PaperChoice(int Id, string Label)
    {
        public override string ToString() => Label;
    }

    private void Reload(int? selectedId = null, string? notice = null)
    {
        try
        {
            var paperId = (_paper.SelectedItem as PaperChoice)?.Id;
            var papers = _service.GetExamPapers();
            var sessions = _service.GetSessions();
            _paper.Items.Clear();
            foreach (var paper in papers)
                _paper.Items.Add(new PaperChoice(paper.Id, $"{paper.PaperCode} — {paper.Title}"));
            if (_paper.Items.Count > 0)
            {
                _paper.SelectedIndex = 0;
                foreach (PaperChoice choice in _paper.Items)
                    if (choice.Id == paperId) { _paper.SelectedItem = choice; break; }
            }
            _defaultPaperId ??= (_paper.SelectedItem as PaperChoice)?.Id;
            if (_defaultPaperId.HasValue && !papers.Any(p => p.Id == _defaultPaperId))
                _defaultPaperId = (_paper.SelectedItem as PaperChoice)?.Id;
            _create.Enabled = papers.Count > 0;
            _grid.Rows.Clear();
            foreach (var session in sessions)
            {
                var paper = papers.FirstOrDefault(p => p.Id == session.PaperId);
                var index = _grid.Rows.Add(session.SessionCode, session.SessionName,
                    paper is null ? $"Đề #{session.PaperId}" : $"{paper.PaperCode} — {paper.Title}",
                    session.RoomName, session.IsOpen ? "Đang mở" : "Đóng");
                _grid.Rows[index].Tag = session;
                if (session.Id == selectedId) _grid.CurrentCell = _grid.Rows[index].Cells[0];
            }
            UpdateSelection();
            _status.ForeColor = UiStyle.TextMuted;
            _status.Text = notice ?? (papers.Count == 0
                ? "Chưa có đề thi để gán cho ca. Hãy lưu đề thi trước khi tạo ca."
                : sessions.Count == 0 ? "Chưa có ca thi. Nhập thông tin ở trên để tạo ca đầu tiên."
                : $"Có {sessions.Count} ca thi. Chọn một ca để mở hoặc đóng.");
        }
        catch (Exception ex)
        {
            _create.Enabled = false;
            _toggle.Enabled = false;
            ShowFailure(ex, "Không tải được thông tin ca và đề thi. Hãy bấm Tải lại để thử lại.");
        }
    }

    private void UpdateSelection()
    {
        _toggle.Enabled = SelectedSession is not null;
        _toggle.Text = SelectedSession?.IsOpen == true ? "Đóng ca" : "Mở ca";
    }

    private void CreateSession()
    {
        if (string.IsNullOrWhiteSpace(_code.Text)) { ShowError("Vui lòng nhập mã ca."); _code.Focus(); return; }
        if (string.IsNullOrWhiteSpace(_name.Text)) { ShowError("Vui lòng nhập tên ca."); _name.Focus(); return; }
        if (string.IsNullOrWhiteSpace(_room.Text)) { ShowError("Vui lòng nhập phòng thi."); _room.Focus(); return; }
        if (_paper.SelectedItem is not PaperChoice paper) { ShowError("Vui lòng chọn đề thi."); _paper.Focus(); return; }
        try
        {
            var id = _service.CreateSession(new ExamSession { SessionCode = _code.Text, SessionName = _name.Text, RoomName = _room.Text, PaperId = paper.Id });
            _code.Clear(); _name.Clear(); _room.Clear();
            _defaultPaperId = paper.Id;
            Reload(id, "Đã lưu ca mới ở trạng thái đóng.");
        }
        catch (Exception ex) { ShowFailure(ex, "Không lưu được ca thi. Hãy kiểm tra thông tin và thử lại."); }
    }

    private void ToggleSession()
    {
        var session = SelectedSession;
        if (session is null) return;
        var action = session.IsOpen ? "Đóng" : "Mở";
        try
        {
            int inProgress = session.IsOpen ? _service.CountInProgress(session.Id) : 0;
            if (!ConfirmToggle(session, action, inProgress)) return;
            if (session.IsOpen)
            {
                int activeAtClose = _service.CloseSession(session.Id, allowInProgress: inProgress > 0);
                if (activeAtClose > 0)
                {
                    if (!ConfirmToggle(session, action, activeAtClose)) return;
                    _service.CloseSession(session.Id, allowInProgress: true);
                }
            }
            else
            {
                _service.SetSessionOpen(session.Id, true);
            }
            Reload(session.Id, $"Đã {action.ToLowerInvariant()} ca {session.SessionCode}.");
        }
        catch (Exception ex)
        {
            ShowFailure(ex, "Không thể kiểm tra hoặc đổi trạng thái ca. Hãy tải lại danh sách và thử lại.");
        }
    }

    private bool ConfirmToggle(ExamSession session, string action, int inProgress)
    {
        string prompt = $"{action} ca {session.SessionCode} — {session.SessionName}?";
        if (inProgress > 0)
            prompt += $"\n\nCó {inProgress} lượt thi đang làm dở trên máy này. Đóng ca sẽ ngăn lượt mới; sinh viên đã bắt đầu vẫn có thể tiếp tục theo hạn nộp cũ.";
        return MessageBox.Show(this, prompt, "Xác nhận", MessageBoxButtons.YesNo,
            inProgress > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) == DialogResult.Yes;
    }

    private void ShowFailure(Exception error, string fallback)
    {
        System.Diagnostics.Trace.WriteLine(error);
        ShowError(error is ExamSessionInputException input ? input.Message : fallback);
    }

    private void ShowError(string message)
    {
        _status.ForeColor = UiStyle.ErrorText;
        _status.Text = message;
    }
}
