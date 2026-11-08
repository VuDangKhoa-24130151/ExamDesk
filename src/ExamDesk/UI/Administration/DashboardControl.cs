using System.Globalization;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-01 • Bạn 1 phụ trách: Trang tổng quan giảng viên (mốc UI, dữ liệu minh họa).
public partial class DashboardControl : UserControl, IUnsavedChangesGuard
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

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
    {
        InitializeComponent();
        RefreshDashboard();
    }

    public void RefreshDashboard()
    {
        UpdateGreeting();
        _lblDate.Text = FormatVietnameseDate(DateTime.Now);

        // 1. Thẻ số liệu tổng hợp từ AdminPreviewData
        var subjectCount = AdminPreviewData.Subjects.Count;
        var chapterCount = AdminPreviewData.Chapters.Count;
        var questionCount = AdminPreviewData.Questions.Count;
        var classCount = AdminPreviewData.Classes.Count;
        var studentCount = AdminPreviewData.Students.Count;

        _lblSubjectsValue.Text = subjectCount.ToString(ViCulture);
        _lblChaptersValue.Text = chapterCount.ToString(ViCulture);
        _lblQuestionsValue.Text = questionCount.ToString(ViCulture);
        _lblClassesValue.Text = classCount.ToString(ViCulture);
        _lblStudentsValue.Text = studentCount.ToString(ViCulture);

        // 2. Phân bố câu hỏi theo độ khó
        var easyCount = AdminPreviewData.Questions.Count(q =>
            string.Equals(q.Difficulty, "Easy", StringComparison.OrdinalIgnoreCase));
        var mediumCount = AdminPreviewData.Questions.Count(q =>
            string.Equals(q.Difficulty, "Medium", StringComparison.OrdinalIgnoreCase));
        var hardCount = AdminPreviewData.Questions.Count(q =>
            string.Equals(q.Difficulty, "Hard", StringComparison.OrdinalIgnoreCase));

        _lblDifficultyTotal.Text = $"Tổng: {questionCount} câu";

        UpdateDifficultyRow(_barEasy, _lblEasyStat, easyCount, questionCount, UiStyle.SuccessText);
        UpdateDifficultyRow(_barMedium, _lblMediumStat, mediumCount, questionCount, UiStyle.Primary);
        UpdateDifficultyRow(_barHard, _lblHardStat, hardCount, questionCount, UiStyle.WarningText);

        // 3. Danh sách "Cần chú ý"
        PopulateAttentionWarnings();
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

    private void PopulateAttentionWarnings()
    {
        var warnings = new List<string>();

        // 1. Chương chưa có câu hỏi
        foreach (var chapter in AdminPreviewData.Chapters
                     .OrderBy(c => c.SubjectId)
                     .ThenBy(c => c.Number))
        {
            var count = AdminPreviewData.CountQuestionsByChapter(chapter.SubjectId, chapter.Number);
            if (count == 0)
            {
                var subjectText = AdminPreviewData.FormatSubjectCode(chapter.SubjectId);
                warnings.Add($"{subjectText} – Chương {chapter.Number} ({chapter.Name}): chưa có câu hỏi nào.");
            }
        }

        // 2. Môn có < 5 câu ở một mức độ khó
        foreach (var subject in AdminPreviewData.Subjects)
        {
            var easy = AdminPreviewData.Questions.Count(q =>
                q.SubjectId == subject.Id &&
                string.Equals(q.Difficulty, "Easy", StringComparison.OrdinalIgnoreCase));
            var medium = AdminPreviewData.Questions.Count(q =>
                q.SubjectId == subject.Id &&
                string.Equals(q.Difficulty, "Medium", StringComparison.OrdinalIgnoreCase));
            var hard = AdminPreviewData.Questions.Count(q =>
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

        // 3. Lớp chưa có sinh viên
        foreach (var cls in AdminPreviewData.Classes)
        {
            var studentCount = AdminPreviewData.CountStudentsByClass(cls.Code);
            if (studentCount == 0)
            {
                warnings.Add($"Lớp {cls.Code} – {cls.Name}: chưa có sinh viên nào.");
            }
        }

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
            _lblAttentionCount.Text = $"{warnings.Count} mục";
            foreach (var warning in warnings)
            {
                var warnLabel = CreateAlertBannerLabel(
                    $"• {warning}",
                    UiStyle.WarningBack,
                    UiStyle.WarningText);
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
