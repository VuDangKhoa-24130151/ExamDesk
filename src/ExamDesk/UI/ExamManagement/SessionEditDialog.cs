#nullable enable
using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

public partial class SessionEditDialog : Form
{
    private readonly ExamSessionService _service;
    private int _sessionId;
    private string _initialName = string.Empty;
    private string _initialRoom = string.Empty;
    private int _initialPaperId;

    public SessionEditDialog() : this(new ExamSessionService())
    {
    }

    public SessionEditDialog(ExamSessionService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        InitializeComponent();
        _editor.SizeChanged += (_, _) =>
            _lblStatus.MaximumSize = new Size(Math.Max(1, _editor.ClientSize.Width - _editor.Padding.Horizontal), 0);
        _btnSave.Click += OnSaveClick;
    }

    public SessionEditDialog(ExamSession session, IEnumerable<ExamPaper> papers, ExamSessionService? service = null)
        : this(service ?? new ExamSessionService())
    {
        LoadSession(session, papers);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
    }

    public void LoadSession(ExamSession session, IEnumerable<ExamPaper> papers)
    {
        ArgumentNullException.ThrowIfNull(session);
        _sessionId = session.Id;
        _initialName = session.SessionName ?? string.Empty;
        _initialRoom = session.RoomName ?? string.Empty;
        _initialPaperId = session.PaperId;

        Text = $"Sửa ca {session.SessionCode}";
        _txtName.Text = _initialName;
        _txtRoom.Text = _initialRoom;

        _cboPaper.Items.Clear();
        if (papers is not null)
        {
            foreach (var item in papers)
            {
                var choice = new PaperChoice(item.Id, $"{item.PaperCode} — {item.Title}");
                _cboPaper.Items.Add(choice);
                if (choice.Id == session.PaperId)
                    _cboPaper.SelectedItem = choice;
            }
        }
        _lblStatus.Text = string.Empty;
    }

    private void OnSaveClick(object? sender, EventArgs e)
    {
        try
        {
            var paperId = (_cboPaper.SelectedItem as PaperChoice)?.Id ?? 0;
            _service.UpdateSession(_sessionId, _txtName.Text, _txtRoom.Text, paperId);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
            _lblStatus.Text = ex is ExamSessionInputException input
                ? input.Message
                : "Không lưu được thay đổi. Hãy kiểm tra thông tin và thử lại.";
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult != DialogResult.OK && HasUnsavedChanges())
        {
            var confirm = MessageBox.Show(this, "Bỏ thay đổi chưa lưu của ca này?", "Hủy sửa ca",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (confirm != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }
        base.OnFormClosing(e);
    }

    private bool HasUnsavedChanges()
    {
        var currentPaperId = (_cboPaper.SelectedItem as PaperChoice)?.Id ?? 0;
        return _txtName.Text != _initialName ||
               _txtRoom.Text != _initialRoom ||
               currentPaperId != _initialPaperId;
    }
}
