using ExamDesk.Models;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-06 • Bạn 1 phụ trách: Hộp thoại xem trước câu hỏi (nhận dữ liệu câu hỏi từ SQLite).
public partial class QuestionPreviewDialog : Form
{
    private static readonly Font BoldOptionFont = new("Segoe UI", 10F, FontStyle.Bold);

    private Question _question = new();

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool AllowEdit
    {
        get => _btnEdit.Visible;
        set => _btnEdit.Visible = value;
    }

    public bool EditRequested { get; private set; }

    public QuestionPreviewDialog()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
    }

    public void ShowQuestion(Question q, string subjectName)
    {
        ArgumentNullException.ThrowIfNull(q);

        _question = q;

        var displaySubject = string.IsNullOrWhiteSpace(subjectName)
            ? $"Môn #{q.SubjectId}"
            : subjectName.Trim();

        var displayChapter = q.Chapter > 0 ? $"Chương {q.Chapter}" : "Chưa chọn chương";
        var displayDifficulty = string.IsNullOrWhiteSpace(q.Difficulty)
            ? "Chưa chọn độ khó"
            : FormatDifficulty(q.Difficulty);

        _lblMetaInfo.Text = $"{displaySubject} · {displayChapter} · {displayDifficulty}";

        _lblQuestionContent.Text = string.IsNullOrWhiteSpace(q.Content)
            ? "(Chưa nhập nội dung câu hỏi)"
            : q.Content;

        _lblOptAText.Text = string.IsNullOrWhiteSpace(q.OptionA) ? "(Chưa nhập đáp án A)" : q.OptionA;
        _lblOptBText.Text = string.IsNullOrWhiteSpace(q.OptionB) ? "(Chưa nhập đáp án B)" : q.OptionB;
        _lblOptCText.Text = string.IsNullOrWhiteSpace(q.OptionC) ? "(Chưa nhập đáp án C)" : q.OptionC;
        _lblOptDText.Text = string.IsNullOrWhiteSpace(q.OptionD) ? "(Chưa nhập đáp án D)" : q.OptionD;

        // Phần sinh viên nhìn thấy: luôn giữ RadioButton ở trạng thái chưa chọn và bị vô hiệu
        _radOptA.Checked = false;
        _radOptB.Checked = false;
        _radOptC.Checked = false;
        _radOptD.Checked = false;

        UpdateCorrectAnswerHighlight();
    }

    private static string FormatDifficulty(string? difficulty) => difficulty switch
    {
        "Easy" => "Dễ",
        "Medium" => "Trung bình",
        "Hard" => "Khó",
        _ => difficulty ?? string.Empty
    };

    private void OnShowCorrectCheckedChanged(object? sender, EventArgs e)
    {
        UpdateCorrectAnswerHighlight();
    }

    private void UpdateCorrectAnswerHighlight()
    {
        var showCorrect = _chkShowCorrect.Checked;
        var correct = _question.CorrectOption?.Trim() ?? string.Empty;

        ApplyOptionHighlight(_optABorder, _optARow, _radOptA, _lblOptAText, _lblOptABadge,
            showCorrect && string.Equals(correct, "A", StringComparison.OrdinalIgnoreCase));

        ApplyOptionHighlight(_optBBorder, _optBRow, _radOptB, _lblOptBText, _lblOptBBadge,
            showCorrect && string.Equals(correct, "B", StringComparison.OrdinalIgnoreCase));

        ApplyOptionHighlight(_optCBorder, _optCRow, _radOptC, _lblOptCText, _lblOptCBadge,
            showCorrect && string.Equals(correct, "C", StringComparison.OrdinalIgnoreCase));

        ApplyOptionHighlight(_optDBorder, _optDRow, _radOptD, _lblOptDText, _lblOptDBadge,
            showCorrect && string.Equals(correct, "D", StringComparison.OrdinalIgnoreCase));
    }

    private static void ApplyOptionHighlight(
        Panel borderPanel,
        TableLayoutPanel rowLayout,
        RadioButton radioButton,
        Label textLabel,
        Label badgeLabel,
        bool isHighlightedCorrect)
    {
        var rowBack = isHighlightedCorrect ? UiStyle.SuccessBack : UiStyle.Surface;
        borderPanel.BackColor = isHighlightedCorrect ? UiStyle.SuccessText : UiStyle.Border;
        rowLayout.BackColor = rowBack;
        radioButton.BackColor = rowBack;
        textLabel.BackColor = rowBack;
        textLabel.ForeColor = isHighlightedCorrect ? UiStyle.SuccessText : UiStyle.Text;
        textLabel.Font = isHighlightedCorrect ? BoldOptionFont : UiStyle.BodyFont;
        badgeLabel.BackColor = rowBack;
        badgeLabel.Visible = isHighlightedCorrect;
    }

    private void OnEditClick(object? sender, EventArgs e)
    {
        EditRequested = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCloseClick(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
