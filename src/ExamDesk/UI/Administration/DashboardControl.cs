using System.Globalization;
using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-01 • Bạn 1 phụ trách: Trang tổng quan giảng viên (kết nối dữ liệu SQLite thật).
public partial class DashboardControl : UserControl, IUnsavedChangesGuard
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    private readonly QuestionBankService _questionBankService;
    private readonly ExamPaperService _examPaperService;
    private readonly AdministrationService _administrationService;

    private Panel _cardExamPapersBorder = null!;
    private Label _lblExamPapersValue = null!;
    private Label _lblExamPapersLabel = null!;

    private string? _lecturerName;

    public event EventHandler<string>? NavigateRequested;

    public bool HasUnsavedChanges => false;

    public bool ConfirmLeave() => true;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string? LecturerName
    {
        get => _lecturerName;
        set
        {
            _lecturerName = value;
            UpdateGreeting();
        }
    }

    public DashboardControl()
        : this(new QuestionBankService(), new ExamPaperService(), new AdministrationService())
    {
    }

    public DashboardControl(
        QuestionBankService questionBankService,
        ExamPaperService examPaperService,
        AdministrationService administrationService)
    {
        _questionBankService = questionBankService ?? throw new ArgumentNullException(nameof(questionBankService));
        _examPaperService = examPaperService ?? throw new ArgumentNullException(nameof(examPaperService));
        _administrationService = administrationService ?? throw new ArgumentNullException(nameof(administrationService));

        InitializeComponent();

        // Đã kết nối SQLite thật: ẩn nhãn số liệu minh họa
        _lblDemoBadge.Visible = false;

        InitializeExamPaperMetricCard();
        WireEvents();
        RefreshDashboard();
    }

    private void InitializeExamPaperMetricCard()
    {
        _cardExamPapersBorder = new Panel();
        _lblExamPapersValue = new Label();
        _lblExamPapersLabel = new Label();

        ConfigureMetricCard(_cardExamPapersBorder, _lblExamPapersValue, _lblExamPapersLabel, "Đề thi", "exams");

        // Đặt thẻ Đề thi ngay sau thẻ Câu hỏi và thu gọn độ rộng thẻ để 6 thẻ dàn đều trên 1 hàng ở 1280px.
        _metricsFlow.Controls.Add(_cardExamPapersBorder);
        _metricsFlow.Controls.SetChildIndex(_cardExamPapersBorder, 3);

        foreach (Control card in _metricsFlow.Controls)
        {
            card.Width = 150;
        }
    }

    private void WireEvents()
    {
        VisibleChanged += (_, _) =>
        {
            if (Visible && IsHandleCreated)
            {
                RefreshDashboard();
            }
        };
    }

    public void RefreshDashboard()
    {
        UpdateGreeting();
        _lblDate.Text = FormatVietnameseDate(DateTime.Now);

        List<Subject> subjects;
        List<(Subject Subject, List<ChapterInfo> Chapters)> chaptersBySubject = [];
        List<Question> questions;
        List<ExamPaperSummary> papers;
        List<ClassInfo> classes;

        try
        {
            subjects = _questionBankService.GetSubjects();
            var totalChapters = 0;
            foreach (var subject in subjects)
            {
                var chapters = _questionBankService.GetChaptersBySubject(subject.Id);
                totalChapters += chapters.Count;
                chaptersBySubject.Add((subject, chapters));
            }

            questions = _questionBankService.GetQuestions();
            papers = _examPaperService.GetPaperSummaries();
            classes = _administrationService.GetClasses();

            var subjectCount = subjects.Count;
            var questionCount = questions.Count;
            var examPaperCount = papers.Count;
            var classCount = classes.Count;
            var studentCount = classes.Sum(c => c.StudentCount);

            _lblSubjectsValue.Text = subjectCount.ToString(ViCulture);
            _lblChaptersValue.Text = totalChapters.ToString(ViCulture);
            _lblQuestionsValue.Text = questionCount.ToString(ViCulture);
            _lblExamPapersValue.Text = examPaperCount.ToString(ViCulture);
            _lblClassesValue.Text = classCount.ToString(ViCulture);
            _lblStudentsValue.Text = studentCount.ToString(ViCulture);

            // 2. Phân bố câu hỏi theo độ khó từ bảng Questions thật
            var easyCount = questions.Count(q =>
                string.Equals(q.Difficulty, "Easy", StringComparison.OrdinalIgnoreCase));
            var mediumCount = questions.Count(q =>
                string.Equals(q.Difficulty, "Medium", StringComparison.OrdinalIgnoreCase));
            var hardCount = questions.Count(q =>
                string.Equals(q.Difficulty, "Hard", StringComparison.OrdinalIgnoreCase));

            _lblDifficultyTotal.Text = $"Tổng: {questionCount} câu";

            UpdateDifficultyRow(_barEasy, _lblEasyStat, easyCount, questionCount, UiStyle.SuccessText);
            UpdateDifficultyRow(_barMedium, _lblMediumStat, mediumCount, questionCount, UiStyle.Primary);
            UpdateDifficultyRow(_barHard, _lblHardStat, hardCount, questionCount, UiStyle.WarningText);

            // 3. Danh sách "Cần chú ý"
            PopulateAttentionWarnings(subjects, chaptersBySubject, questions, classes);
        }
        catch (Exception ex)
        {
            _lblSubjectsValue.Text = "0";
            _lblChaptersValue.Text = "0";
            _lblQuestionsValue.Text = "0";
            _lblExamPapersValue.Text = "0";
            _lblClassesValue.Text = "0";
            _lblStudentsValue.Text = "0";
            _lblDifficultyTotal.Text = "Tổng: 0 câu";

            UpdateDifficultyRow(_barEasy, _lblEasyStat, 0, 0, UiStyle.SuccessText);
            UpdateDifficultyRow(_barMedium, _lblMediumStat, 0, 0, UiStyle.Primary);
            UpdateDifficultyRow(_barHard, _lblHardStat, 0, 0, UiStyle.WarningText);

            RenderAttentionMessages([$"Không thể tải số liệu từ SQLite: {ex.Message}"], isError: true);
        }
    }

    private void UpdateGreeting()
    {
        var displayName = string.IsNullOrWhiteSpace(_lecturerName)
            ? "Giảng viên"
            : _lecturerName.Trim();
        _lblGreeting.Text = $"Xin chào, {displayName}";
    }

    private static string FormatVietnameseDate(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba",
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ Nhật",
            _ => ViCulture.TextInfo.ToTitleCase(date.ToString("dddd", ViCulture))
        };

        return $"{dayOfWeek}, {date.ToString("dd/MM/yyyy", ViCulture)}";
    }

    private static void UpdateDifficultyRow(
        Panel barPanel,
        Label statLabel,
        int count,
        int total,
        Color fillColor)
    {
        var ratio = total > 0 ? Math.Clamp((double)count / total, 0.0, 1.0) : 0.0;
        var percentText = (ratio * 100.0).ToString("0.#", ViCulture);
        statLabel.Text = $"{count} câu ({percentText}%)";
        barPanel.Tag = new BarPaintState(ratio, fillColor);
        barPanel.Invalidate();
    }

    private void OnDifficultyBarPaint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        var bounds = panel.ClientRectangle;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using (var bgBrush = new SolidBrush(UiStyle.Background))
        {
            e.Graphics.FillRectangle(bgBrush, bounds);
        }

        if (panel.Tag is BarPaintState state && state.Ratio > 0)
        {
            var fillWidth = Math.Max(4, (int)Math.Round((bounds.Width - 2) * state.Ratio));
            fillWidth = Math.Min(bounds.Width - 2, fillWidth);
            var fillRect = new Rectangle(1, 1, fillWidth, Math.Max(1, bounds.Height - 2));
            using var fillBrush = new SolidBrush(state.FillColor);
            e.Graphics.FillRectangle(fillBrush, fillRect);
        }

        using var borderPen = new Pen(UiStyle.Border);
        e.Graphics.DrawRectangle(borderPen, 0, 0, bounds.Width - 1, bounds.Height - 1);
    }

    private void PopulateAttentionWarnings(
        List<Subject> subjects,
        List<(Subject Subject, List<ChapterInfo> Chapters)> chaptersBySubject,
        List<Question> questions,
        List<ClassInfo> classes)
    {
        var warnings = new List<string>();

        if (subjects.Count == 0)
        {
            warnings.Add("Chưa có môn học nào trong ngân hàng (hãy vào mục Môn và chương hoặc Ngân hàng câu hỏi để khởi tạo).");
        }

        // 1. Chương chưa có câu hỏi nào
        foreach (var (subject, chapters) in chaptersBySubject.OrderBy(x => x.Subject.Code, StringComparer.OrdinalIgnoreCase))
        {
            foreach (var chapter in chapters.OrderBy(c => c.Number))
            {
                if (chapter.QuestionCount == 0)
                {
                    warnings.Add($"{subject.Code} – {subject.Name} – Chương {chapter.Number} ({chapter.Name}): chưa có câu hỏi nào.");
                }
            }
        }

        // 2. Môn học có dưới 5 câu ở một mức độ khó (Dễ / Trung bình / Khó)
        foreach (var subject in subjects.OrderBy(s => s.Code, StringComparer.OrdinalIgnoreCase))
        {
            var easy = questions.Count(q =>
                q.SubjectId == subject.Id &&
                string.Equals(q.Difficulty, "Easy", StringComparison.OrdinalIgnoreCase));
            var medium = questions.Count(q =>
                q.SubjectId == subject.Id &&
                string.Equals(q.Difficulty, "Medium", StringComparison.OrdinalIgnoreCase));
            var hard = questions.Count(q =>
                q.SubjectId == subject.Id &&
                string.Equals(q.Difficulty, "Hard", StringComparison.OrdinalIgnoreCase));

            var lowLevels = new List<string>();
            if (easy < 5) lowLevels.Add($"Dễ ({easy}/5)");
            if (medium < 5) lowLevels.Add($"Trung bình ({medium}/5)");
            if (hard < 5) lowLevels.Add($"Khó ({hard}/5)");

            if (lowLevels.Count > 0)
            {
                warnings.Add(
                    $"Môn {subject.Code} – {subject.Name} có dưới 5 câu ở mức độ khó: {string.Join(", ", lowLevels)}.");
            }
        }

        // 3. Lớp học chưa có sinh viên nào
        foreach (var cls in classes.OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase))
        {
            if (cls.StudentCount == 0)
            {
                warnings.Add($"Lớp {cls.Code} – {cls.Name}: chưa có sinh viên nào.");
            }
        }

        RenderAttentionMessages(warnings, isError: false);
    }

    private void RenderAttentionMessages(List<string> warnings, bool isError)
    {
        _attentionListFlow.SuspendLayout();
        while (_attentionListFlow.Controls.Count > 0)
        {
            var old = _attentionListFlow.Controls[0];
            _attentionListFlow.Controls.RemoveAt(0);
            old.Dispose();
        }

        if (warnings.Count == 0)
        {
            _lblAttentionCount.Text = "Đạt yêu cầu";
            var okLabel = CreateAlertBannerLabel(
                "✓ Dữ liệu đầy đủ",
                UiStyle.SuccessBack,
                UiStyle.SuccessText);
            _attentionListFlow.Controls.Add(okLabel);
        }
        else
        {
            _lblAttentionCount.Text = isError ? "Lỗi tải dữ liệu" : $"{warnings.Count} mục";
            var backColor = isError ? UiStyle.ErrorBack : UiStyle.WarningBack;
            var foreColor = isError ? UiStyle.ErrorText : UiStyle.WarningText;

            foreach (var warning in warnings)
            {
                var warnLabel = CreateAlertBannerLabel(
                    $"• {warning}",
                    backColor,
                    foreColor);
                _attentionListFlow.Controls.Add(warnLabel);
            }
        }

        AdjustAttentionBannerWidths();
        _attentionListFlow.ResumeLayout();
    }

    private static Label CreateAlertBannerLabel(string text, Color backColor, Color foreColor)
    {
        return new Label
        {
            AutoSize = true,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = UiStyle.SmallFont,
            Padding = new Padding(10, 8, 10, 8),
            Margin = new Padding(0, 0, 0, 6),
            Text = text
        };
    }

    private void OnAttentionListSizeChanged(object? sender, EventArgs e)
    {
        AdjustAttentionBannerWidths();
    }

    private void AdjustAttentionBannerWidths()
    {
        var width = Math.Max(180, _attentionListFlow.ClientSize.Width - _attentionListFlow.Padding.Horizontal - 4);
        foreach (Control control in _attentionListFlow.Controls)
        {
            if (control is Label label)
            {
                label.MinimumSize = new Size(width, 0);
                label.MaximumSize = new Size(width, 0);
            }
        }
    }

    private void RequestNavigation(string key)
    {
        NavigateRequested?.Invoke(this, key);
    }

    private readonly record struct BarPaintState(double Ratio, Color FillColor);
}
