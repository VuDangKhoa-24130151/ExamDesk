using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// GV-06 • Bạn 1 phụ trách: Hộp thoại xem trước câu hỏi (nhận dữ liệu câu hỏi từ SQLite).
public partial class QuestionPreviewDialog : Form
{
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
        _rootLayout.SizeChanged += (_, _) => AdjustWrappedTextWidths();
        _scrollPanel.SizeChanged += (_, _) => AdjustWrappedTextWidths();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
        AdjustWrappedTextWidths();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AdjustWrappedTextWidths();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ReleasePreviewImage();
        base.OnFormClosed(e);
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

        UpdateQuestionImage(q.ImagePath);

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
        AdjustWrappedTextWidths();
    }

    private void AdjustWrappedTextWidths()
    {
        if (_rootLayout.ClientSize.Width > 0)
        {
            _lblMetaInfo.MaximumSize = new Size(
                Math.Max(240, _rootLayout.ClientSize.Width - _rootLayout.Padding.Horizontal - 4),
                0);
        }

        if (_scrollPanel.ClientSize.Width <= 0)
        {
            return;
        }

        var viewportWidth = Math.Max(
            280,
            _scrollPanel.ClientSize.Width - _scrollPanel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 6);

        _lblQuestionContent.MaximumSize = new Size(
            Math.Max(220, viewportWidth - _questionBoxPanel.Padding.Horizontal),
            0);

        var badgeReserved = _chkShowCorrect.Checked
            ? Math.Max(120, _lblOptABadge.PreferredSize.Width + 12)
            : 0;
        var optionWidth = Math.Max(180, viewportWidth - 95 - badgeReserved);
        _lblOptAText.MaximumSize = new Size(optionWidth, 0);
        _lblOptBText.MaximumSize = new Size(optionWidth, 0);
        _lblOptCText.MaximumSize = new Size(optionWidth, 0);
        _lblOptDText.MaximumSize = new Size(optionWidth, 0);
    }

    private void UpdateQuestionImage(string? imagePath)
    {
        ReleasePreviewImage();

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            _picQuestionImage.Visible = false;
            _lblImageMissing.Visible = false;
            return;
        }

        var loadedImage = QuestionBankService.LoadImageUnlocked(imagePath);
        if (loadedImage is not null)
        {
            _picQuestionImage.Image = loadedImage;
            _picQuestionImage.Visible = true;
            _lblImageMissing.Visible = false;
            return;
        }

        _picQuestionImage.Visible = false;
        _lblImageMissing.Text = "⚠ Không tìm thấy tệp ảnh đính kèm";
        _lblImageMissing.Visible = true;
    }

    private void ReleasePreviewImage()
    {
        var oldImage = _picQuestionImage.Image;
        _picQuestionImage.Image = null;
        oldImage?.Dispose();
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

        AdjustWrappedTextWidths();
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
        var borderColor = isHighlightedCorrect ? UiStyle.SuccessText : UiStyle.Border;
        borderPanel.BackColor = rowBack;
        UiStyle.ApplyRoundedCorners(borderPanel, 8, borderColor);
        rowLayout.BackColor = rowBack;
        radioButton.BackColor = rowBack;
        textLabel.BackColor = rowBack;
        textLabel.ForeColor = isHighlightedCorrect ? UiStyle.SuccessText : UiStyle.Text;
        textLabel.Font = isHighlightedCorrect ? UiStyle.BodyBoldFont : UiStyle.BodyFont;
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
