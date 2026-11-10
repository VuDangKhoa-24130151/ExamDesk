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
    private List<ExamSession> _sessions = [];
    private List<ExamPaper> _papers = [];
    private HashSet<int> _editableIds = [];
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

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.S) ||
            (keyData == Keys.Enter && (_code.Focused || _name.Focused || _room.Focused)))
        {
            if (_create.Enabled) CreateSession();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void Reload(int? selectedId = null, string? notice = null)
    {
        try
        {
            var paperId = (_paper.SelectedItem as PaperChoice)?.Id;
            var papers = _service.GetExamPapers();
            var sessions = _service.GetSessions();
            _editableIds = _service.GetEditableSessionIds();
            _papers = papers;
            _sessions = sessions;
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
            ApplyFilters(selectedId);
            _status.ForeColor = UiStyle.TextMuted;
            _status.Text = notice ?? (papers.Count == 0
                ? "Chưa có đề thi để gán cho ca. Hãy lưu đề thi trước khi tạo ca."
                : sessions.Count == 0 ? "Chưa có ca thi. Nhập thông tin ở trên để tạo ca đầu tiên."
                : "Chọn một ca để sửa, mở hoặc đóng.");
        }
        catch (Exception ex)
        {
            _create.Enabled = false;
            _sessions = [];
            ApplyFilters(null);
            ShowFailure(ex, "Không tải được thông tin ca và đề thi. Hãy bấm Tải lại để thử lại.");
        }
    }

    private void ApplyFilters(int? selectedId)
    {
        var search = _search.Text.Trim();
        _grid.Rows.Clear();
        DataGridViewCell? selectedCell = null;
        foreach (var session in _sessions)
        {
            if ((_stateFilter.SelectedIndex == 1 && !session.IsOpen) ||
                (_stateFilter.SelectedIndex == 2 && session.IsOpen)) continue;
            if (!session.SessionCode.Contains(search, StringComparison.CurrentCultureIgnoreCase) &&
                !session.SessionName.Contains(search, StringComparison.CurrentCultureIgnoreCase) &&
                !session.RoomName.Contains(search, StringComparison.CurrentCultureIgnoreCase)) continue;

            var paper = _papers.FirstOrDefault(p => p.Id == session.PaperId);
            var index = _grid.Rows.Add(session.SessionCode, session.SessionName,
                paper is null ? $"Đề #{session.PaperId}" : $"{paper.PaperCode} — {paper.Title}",
                session.RoomName, session.IsOpen ? "Đang mở" : "Đóng");
            var row = _grid.Rows[index];
            row.Tag = session;
            if (session.Id == selectedId) selectedCell = row.Cells[0];
        }

        if (_grid.Rows.Count > 0) _grid.CurrentCell = selectedCell ?? _grid.Rows[0].Cells[0];
        _filterStatus.Text = _grid.Rows.Count == 0 && (search.Length > 0 || _stateFilter.SelectedIndex > 0)
            ? "Không có ca thi phù hợp với bộ lọc."
            : $"Hiển thị {_grid.Rows.Count}/{_sessions.Count} ca thi.";
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        var session = SelectedSession;
        _toggle.Enabled = session is not null;
        _toggle.Text = session?.IsOpen == true ? "Đóng ca" : "Mở ca";
        _edit.Enabled = session is { IsOpen: false } && _editableIds.Contains(session.Id);
    }

    private void EditSession()
    {
        if (SelectedSession is not { } session || !_edit.Enabled) return;

        using var dialog = new Form
        {
            Text = $"Sửa ca {session.SessionCode}",
            StartPosition = FormStartPosition.CenterParent, ShowInTaskbar = false,
            ClientSize = new Size(560, 240), MinimumSize = new Size(400, 250),
            AutoScaleDimensions = new SizeF(7F, 15F), AutoScaleMode = AutoScaleMode.Font,
            Font = UiStyle.BodyFont, BackColor = UiStyle.Surface,
            MinimizeBox = false, MaximizeBox = false, AutoScroll = true
        };
        var name = new TextBox { Text = session.SessionName, MaxLength = 200 };
        var room = new TextBox { Text = session.RoomName, MaxLength = 100 };
        var paper = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        foreach (var item in _papers)
        {
            var choice = new PaperChoice(item.Id, $"{item.PaperCode} — {item.Title}");
            paper.Items.Add(choice);
            if (choice.Id == session.PaperId) paper.SelectedItem = choice;
        }

        var editor = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Top, ColumnCount = 2, Padding = new Padding(16) };
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        string[] labels = ["Tên ca *", "Phòng thi *", "Đề thi *"];
        Control[] fields = [name, room, paper];
        for (int i = 0; i < fields.Length; i++)
        {
            editor.Controls.Add(new Label { Text = labels[i], AutoSize = true, Anchor = AnchorStyles.Left }, 0, i);
            fields[i].AccessibleName = labels[i];
            fields[i].Dock = DockStyle.Fill;
            editor.Controls.Add(fields[i], 1, i);
        }
        var status = new Label { AutoSize = true, ForeColor = UiStyle.ErrorText, Margin = new Padding(0, 8, 0, 8) };
        editor.Controls.Add(status, 0, 3);
        editor.SetColumnSpan(status, 2);
        editor.SizeChanged += (_, _) =>
            status.MaximumSize = new Size(Math.Max(1, editor.ClientSize.Width - editor.Padding.Horizontal), 0);

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true };
        var save = new Button { Text = "Lưu thay đổi" };
        var cancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel };
        UiStyle.StylePrimaryButton(save);
        UiStyle.StyleSecondaryButton(cancel);
        actions.Controls.AddRange([save, cancel]);
        editor.Controls.Add(actions, 0, 4);
        editor.SetColumnSpan(actions, 2);
        dialog.Controls.Add(editor);
        dialog.AcceptButton = save;
        dialog.CancelButton = cancel;
        dialog.Load += (_, _) => UiStyle.ClampToWorkingArea(dialog);

        save.Click += (_, _) =>
        {
            try
            {
                _service.UpdateSession(session.Id, name.Text, room.Text, (paper.SelectedItem as PaperChoice)?.Id ?? 0);
                dialog.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                status.Text = ex is ExamSessionInputException input ? input.Message
                    : "Không lưu được thay đổi. Hãy kiểm tra thông tin và thử lại.";
            }
        };
        dialog.FormClosing += (_, e) =>
        {
            if (dialog.DialogResult == DialogResult.OK ||
                (name.Text == session.SessionName && room.Text == session.RoomName &&
                 (paper.SelectedItem as PaperChoice)?.Id == session.PaperId)) return;
            e.Cancel = MessageBox.Show(dialog, "Bỏ thay đổi chưa lưu của ca này?", "Hủy sửa ca",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes;
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload(session.Id, $"Đã lưu thay đổi của ca {session.SessionCode}.");
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
