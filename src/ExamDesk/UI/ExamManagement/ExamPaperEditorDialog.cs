using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

public partial class ExamPaperEditorDialog : Form
{
    private const int DefaultDurationMinutes = 45;
    private const int DefaultEasyCount = 10;
    private const int DefaultMediumCount = 15;
    private const int DefaultHardCount = 5;

    private readonly ExamPaperService _examPaperService;
    private readonly QuestionBankService _questionBankService;
    private readonly int? _initialSubjectId;

    private ExamPaperDifficultyCounts _availableCounts = new();
    private bool _hasSubjects;
    private bool _savedSuccessfully;
    private bool _suppressEvents;

    public int CreatedPaperId { get; private set; }

    public ExamPaperEditorDialog() : this(null)
    {
    }

    public ExamPaperEditorDialog(int? initialSubjectId)
    {
        _initialSubjectId = initialSubjectId;
        _examPaperService = new ExamPaperService();
        _questionBankService = new QuestionBankService();

        InitializeComponent();
        ApplyUiStyle();
        WireEvents();
        LoadSubjects();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
        UpdateWrappedLabelWidths();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        UiStyle.ClampToWorkingArea(this);
        UpdateWrappedLabelWidths();
        _txtPaperCode.Focus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.S))
        {
            if (_btnSave.Enabled)
            {
                SaveExamPaper();
            }

            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_savedSuccessfully && IsDirty())
        {
            var confirm = MessageBox.Show(
                this,
                "Bạn đã nhập thông tin đề thi. Bạn có chắc muốn hủy bỏ và đóng cửa sổ?",
                "Xác nhận hủy",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        base.OnFormClosing(e);
    }

    private void ApplyUiStyle()
    {
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;

        _scrollPanel.BackColor = UiStyle.Surface;
        _formTable.BackColor = UiStyle.Surface;

        _lblDialogTitle.Font = UiStyle.TitleFont;
        _lblDialogTitle.ForeColor = UiStyle.Text;

        foreach (Control control in _formTable.Controls)
        {
            if (control is Label lbl)
            {
                lbl.Font = UiStyle.BodyFont;
                lbl.ForeColor = UiStyle.Text;
            }
        }

        _lblEasyAvailable.Font = UiStyle.SmallFont;
        _lblEasyAvailable.ForeColor = UiStyle.TextMuted;
        _lblMediumAvailable.Font = UiStyle.SmallFont;
        _lblMediumAvailable.ForeColor = UiStyle.TextMuted;
        _lblHardAvailable.Font = UiStyle.SmallFont;
        _lblHardAvailable.ForeColor = UiStyle.TextMuted;

        _lblEasyError.Font = UiStyle.SmallFont;
        _lblEasyError.ForeColor = UiStyle.ErrorText;
        _lblMediumError.Font = UiStyle.SmallFont;
        _lblMediumError.ForeColor = UiStyle.ErrorText;
        _lblHardError.Font = UiStyle.SmallFont;
        _lblHardError.ForeColor = UiStyle.ErrorText;

        _lblTotalSummary.Font = UiStyle.BodyBoldFont;
        _lblTotalSummary.ForeColor = UiStyle.Text;

        _lblFormError.Font = UiStyle.SmallFont;
        _lblFormError.ForeColor = UiStyle.ErrorText;
        _lblFormError.BackColor = UiStyle.ErrorBack;

        UiStyle.StylePrimaryButton(_btnSave);
        UiStyle.StyleSecondaryButton(_btnCancel);
    }

    private void WireEvents()
    {
        SizeChanged += (_, _) => UpdateWrappedLabelWidths();
        _cboSubject.SelectedIndexChanged += (_, _) =>
        {
            if (!_suppressEvents)
            {
                OnSubjectChanged();
            }
        };
        _numEasy.ValueChanged += (_, _) =>
        {
            if (!_suppressEvents)
            {
                UpdateCountsAndValidation();
            }
        };
        _numMedium.ValueChanged += (_, _) =>
        {
            if (!_suppressEvents)
            {
                UpdateCountsAndValidation();
            }
        };
        _numHard.ValueChanged += (_, _) =>
        {
            if (!_suppressEvents)
            {
                UpdateCountsAndValidation();
            }
        };

        _txtPaperCode.TextChanged += (_, _) => ClearFormError();
        _txtTitle.TextChanged += (_, _) => ClearFormError();
        _numDuration.ValueChanged += (_, _) =>
        {
            if (!_suppressEvents)
            {
                ClearFormError();
            }
        };

        _btnSave.Click += (_, _) => SaveExamPaper();
        _btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
    }

    private void LoadSubjects()
    {
        List<SubjectOption> subjects;
        string? loadError = null;
        try
        {
            subjects = _questionBankService.GetSubjects()
                .Select(s => new SubjectOption(s.Id, $"{s.Code} - {s.Name}"))
                .ToList();
        }
        catch (Exception ex)
        {
            subjects = [];
            loadError = ex.Message;
        }

        _suppressEvents = true;
        try
        {
            _cboSubject.DisplayMember = nameof(SubjectOption.DisplayName);
            _cboSubject.ValueMember = nameof(SubjectOption.Id);
            _cboSubject.DataSource = subjects;

            _hasSubjects = subjects.Count > 0;
            if (_hasSubjects)
            {
                if (_initialSubjectId.HasValue && subjects.Any(s => s.Id == _initialSubjectId.Value))
                {
                    _cboSubject.SelectedValue = _initialSubjectId.Value;
                }
                else
                {
                    _cboSubject.SelectedIndex = 0;
                }
            }

            // Gán cấu hình mặc định (45 phút; 10 Dễ, 15 Trung bình, 5 Khó) khi khởi tạo để tránh kích hoạt validate lặp lại.
            _numDuration.Value = DefaultDurationMinutes;
            _numEasy.Value = DefaultEasyCount;
            _numMedium.Value = DefaultMediumCount;
            _numHard.Value = DefaultHardCount;
        }
        finally
        {
            _suppressEvents = false;
        }

        if (!_hasSubjects)
        {
            _availableCounts = new ExamPaperDifficultyCounts();
            _lblEasyAvailable.Text = "Có sẵn: 0 câu";
            _lblMediumAvailable.Text = "Có sẵn: 0 câu";
            _lblHardAvailable.Text = "Có sẵn: 0 câu";
            UpdateCountsAndValidation();
            ShowFormError(loadError ?? "Chưa có môn học nào trong hệ thống. Vui lòng tạo môn học và câu hỏi trước khi tạo đề thi.");
            return;
        }

        OnSubjectChanged();
    }

    private void OnSubjectChanged()
    {
        ClearFormError();
        if (_cboSubject.SelectedItem is not SubjectOption subject)
        {
            _availableCounts = new ExamPaperDifficultyCounts();
            UpdateCountsAndValidation();
            return;
        }

        string? loadError = null;
        try
        {
            _availableCounts = _examPaperService.GetAvailableCounts(subject.Id);
        }
        catch (Exception ex)
        {
            _availableCounts = new ExamPaperDifficultyCounts();
            loadError = ex.Message;
        }

        _lblEasyAvailable.Text = $"Có sẵn: {_availableCounts.EasyCount} câu";
        _lblMediumAvailable.Text = $"Có sẵn: {_availableCounts.MediumCount} câu";
        _lblHardAvailable.Text = $"Có sẵn: {_availableCounts.HardCount} câu";

        UpdateCountsAndValidation();

        if (!string.IsNullOrEmpty(loadError))
        {
            ShowFormError(loadError);
        }
    }

    private void UpdateCountsAndValidation()
    {
        ClearFormError();

        var easy = (int)_numEasy.Value;
        var medium = (int)_numMedium.Value;
        var hard = (int)_numHard.Value;
        var total = easy + medium + hard;

        _lblTotalSummary.Text =
            $"Cơ cấu cố định: {easy} Dễ – {medium} Trung bình – {hard} Khó (Tổng {total} câu, hệ thống tự rút ngẫu nhiên)";
        _lblTotalSummary.ForeColor = UiStyle.Text;

        var hasShortage = false;
        hasShortage |= UpdateDifficultyError(_lblEasyError, easy, _availableCounts.EasyCount);
        hasShortage |= UpdateDifficultyError(_lblMediumError, medium, _availableCounts.MediumCount);
        hasShortage |= UpdateDifficultyError(_lblHardError, hard, _availableCounts.HardCount);

        _btnSave.Enabled = _hasSubjects && !hasShortage && total > 0;
    }

    private static bool UpdateDifficultyError(Label errorLabel, int requested, int available)
    {
        if (requested > available)
        {
            errorLabel.Text = $"Thiếu {requested - available} câu";
            errorLabel.Visible = true;
            return true;
        }

        errorLabel.Text = string.Empty;
        errorLabel.Visible = false;
        return false;
    }

    private void SaveExamPaper()
    {
        ClearFormError();

        if (_cboSubject.SelectedItem is not SubjectOption subject)
        {
            ShowFormError("Vui lòng chọn môn học trước khi tạo đề thi.");
            return;
        }

        var paper = new ExamPaper
        {
            PaperCode = _txtPaperCode.Text,
            Title = _txtTitle.Text,
            SubjectId = subject.Id,
            DurationMinutes = (int)_numDuration.Value,
            EasyCount = (int)_numEasy.Value,
            MediumCount = (int)_numMedium.Value,
            HardCount = (int)_numHard.Value
        };

        try
        {
            CreatedPaperId = _examPaperService.GenerateExamPaper(paper);
            _savedSuccessfully = true;

            MessageBox.Show(
                this,
                $"Đã tạo đề thi '{paper.PaperCode}' thành công ({paper.TotalQuestions} câu hỏi).",
                "Tạo đề thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowFormError(ex.Message);
        }
    }

    private void UpdateWrappedLabelWidths()
    {
        if (_rootLayout.ClientSize.Width <= 0)
        {
            return;
        }

        var summaryWidth = Math.Max(260, _rootLayout.ClientSize.Width - _rootLayout.Padding.Horizontal - 4);
        _lblTotalSummary.MaximumSize = new Size(summaryWidth, 0);
        _lblFormError.MaximumSize = new Size(Math.Max(220, summaryWidth - _lblFormError.Padding.Horizontal), 0);
    }

    private void ShowFormError(string message)
    {
        _lblFormError.Text = message;
        _lblFormError.Visible = true;
    }

    private void ClearFormError()
    {
        if (_lblFormError.Visible)
        {
            _lblFormError.Text = string.Empty;
            _lblFormError.Visible = false;
        }
    }

    private bool IsDirty()
    {
        return !string.IsNullOrWhiteSpace(_txtPaperCode.Text)
            || !string.IsNullOrWhiteSpace(_txtTitle.Text)
            || _numDuration.Value != DefaultDurationMinutes
            || _numEasy.Value != DefaultEasyCount
            || _numMedium.Value != DefaultMediumCount
            || _numHard.Value != DefaultHardCount;
    }

    private sealed record SubjectOption(int Id, string DisplayName);
}
