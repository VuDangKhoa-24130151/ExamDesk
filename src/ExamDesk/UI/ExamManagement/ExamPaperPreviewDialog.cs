using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

public partial class ExamPaperPreviewDialog : Form
{
    private static readonly Font PaperTitleFont = new("Segoe UI", 13F, FontStyle.Bold);

    private readonly ExamPaperService _examPaperService;
    private readonly ExamPaperSummary _summary;
    private List<ExamPaperQuestionDetail> _questions = [];

    public bool QuestionsRegenerated { get; private set; }

    public ExamPaperPreviewDialog(ExamPaperSummary summary)
    {
        _summary = summary ?? throw new ArgumentNullException(nameof(summary));
        _examPaperService = new ExamPaperService();

        InitializeComponent();
        ApplyUiStyle();
        BindHeader();
        WireEvents();
        LoadQuestions();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        UiStyle.ClampToWorkingArea(this);
        UpdateHeaderWrapWidths();
        AdjustCardWidths();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        DisposeQuestionPanelControls();
        _questionsPanel.Controls.Clear();
        base.OnFormClosed(e);
    }

    private void ApplyUiStyle()
    {
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;

        _headerCard.BackColor = UiStyle.Surface;
        _headerCard.BorderStyle = BorderStyle.None;
        UiStyle.ApplyRoundedCorners(_headerCard, 10, UiStyle.Border);

        _lblPaperTitle.Font = PaperTitleFont;
        _lblPaperTitle.ForeColor = UiStyle.Text;

        _lblMetaInfo.Font = UiStyle.BodyFont;
        _lblMetaInfo.ForeColor = UiStyle.TextMuted;

        _lblStatusBadge.Font = UiStyle.SmallFont;
        UiStyle.StyleBadge(_lblStatusBadge, 6);
        _chkShowAnswerKey.Font = UiStyle.BodyFont;
        _chkShowAnswerKey.ForeColor = UiStyle.Text;

        UiStyle.StylePrimaryButton(_btnClose);
        UiStyle.StyleSecondaryButton(_btnRegenerate);
    }

    private void BindHeader()
    {
        Text = $"Xem trước đề thi - {_summary.PaperCode}";
        _lblPaperTitle.Text = $"{_summary.PaperCode} - {_summary.Title}";
        _lblMetaInfo.Text =
            $"Môn: {_summary.SubjectCode} - {_summary.SubjectName}   |   " +
            $"Thời lượng: {_summary.DurationMinutes} phút   |   " +
            $"Tổng câu: {_summary.TotalQuestions} ({_summary.EasyCount}D / {_summary.MediumCount}TB / {_summary.HardCount}K)";

        if (_summary.IsLocked)
        {
            _lblStatusBadge.Text = "🔒 Đã gán ca (Khóa)";
            _lblStatusBadge.BackColor = UiStyle.WarningBack;
            _lblStatusBadge.ForeColor = UiStyle.WarningText;
            _btnRegenerate.Visible = false;
            _btnRegenerate.Enabled = false;
        }
        else
        {
            _lblStatusBadge.Text = "Chưa gán ca";
            _lblStatusBadge.BackColor = UiStyle.SuccessBack;
            _lblStatusBadge.ForeColor = UiStyle.SuccessText;
            _btnRegenerate.Visible = true;
            _btnRegenerate.Enabled = true;
        }
    }

    private void WireEvents()
    {
        _headerCard.SizeChanged += (_, _) => UpdateHeaderWrapWidths();
        _chkShowAnswerKey.CheckedChanged += (_, _) => RenderQuestions();
        _btnRegenerate.Click += (_, _) => RegeneratePaperQuestions();
        _btnClose.Click += (_, _) => Close();
        _questionsPanel.Resize += (_, _) => AdjustCardWidths();
    }

    private void LoadQuestions()
    {
        try
        {
            _questions = _examPaperService.GetPaperQuestions(_summary.Id);
            RenderQuestions();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi tải danh sách câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void RenderQuestions()
    {
        _questionsPanel.SuspendLayout();
        try
        {
            DisposeQuestionPanelControls();
            _questionsPanel.Controls.Clear();

            if (_questions.Count == 0)
            {
                var emptyLabel = new Label
                {
                    AutoSize = true,
                    Font = UiStyle.BodyFont,
                    ForeColor = UiStyle.TextMuted,
                    Text = "Đề thi này chưa có câu hỏi nào.",
                    Margin = new Padding(0, 12, 0, 0)
                };
                _questionsPanel.Controls.Add(emptyLabel);
                return;
            }

            var showAnswer = _chkShowAnswerKey.Checked;
            var cardWidth = GetTargetCardWidth();

            foreach (var q in _questions)
            {
                var card = BuildQuestionCard(q, showAnswer, cardWidth);
                _questionsPanel.Controls.Add(card);
            }
        }
        finally
        {
            _questionsPanel.ResumeLayout();
        }
    }

    private Panel BuildQuestionCard(ExamPaperQuestionDetail q, bool showAnswer, int cardWidth)
    {
        var card = new Panel
        {
            Width = cardWidth,
            MinimumSize = new Size(cardWidth, 0),
            MaximumSize = new Size(cardWidth, 0),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = UiStyle.Surface,
            BorderStyle = BorderStyle.None,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 10)
        };
        UiStyle.ApplyRoundedCorners(card, 10, UiStyle.Border);

        var inner = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(0)
        };

        var textMaxWidth = Math.Max(240, cardWidth - 34);

        var headerLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(textMaxWidth, 0),
            Font = UiStyle.SmallBoldFont,
            ForeColor = UiStyle.Primary,
            Text = $"Câu {q.OrderIndex}  •  {FormatDifficulty(q.Difficulty)}  •  Chương {q.Chapter}",
            Margin = new Padding(0, 0, 0, 6)
        };

        var contentLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(textMaxWidth, 0),
            Font = UiStyle.BodyBoldFont,
            ForeColor = UiStyle.Text,
            Text = q.Content,
            Margin = new Padding(0, 0, 0, 8)
        };

        inner.Controls.Add(headerLabel);
        inner.Controls.Add(contentLabel);

        if (!string.IsNullOrWhiteSpace(q.ImagePath))
        {
            var questionImage = QuestionBankService.LoadImageUnlocked(q.ImagePath);
            if (questionImage is not null)
            {
                var pictureBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = textMaxWidth,
                    Height = 160,
                    BackColor = UiStyle.Background,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(0, 0, 0, 8),
                    Image = questionImage,
                    TabStop = false
                };
                pictureBox.Disposed += (_, _) =>
                {
                    var img = pictureBox.Image;
                    pictureBox.Image = null;
                    img?.Dispose();
                };
                inner.Controls.Add(pictureBox);
            }
            else
            {
                var missingLabel = new Label
                {
                    AutoSize = true,
                    MaximumSize = new Size(textMaxWidth, 0),
                    Font = UiStyle.SmallFont,
                    ForeColor = UiStyle.WarningText,
                    BackColor = UiStyle.WarningBack,
                    Padding = new Padding(8, 4, 8, 4),
                    Margin = new Padding(0, 0, 0, 8),
                    Text = "⚠ Không tìm thấy tệp ảnh đính kèm"
                };
                inner.Controls.Add(missingLabel);
            }
        }

        inner.Controls.Add(BuildOptionLabel("A", q.OptionA, q.CorrectOption, showAnswer, textMaxWidth));
        inner.Controls.Add(BuildOptionLabel("B", q.OptionB, q.CorrectOption, showAnswer, textMaxWidth));
        inner.Controls.Add(BuildOptionLabel("C", q.OptionC, q.CorrectOption, showAnswer, textMaxWidth));
        inner.Controls.Add(BuildOptionLabel("D", q.OptionD, q.CorrectOption, showAnswer, textMaxWidth));

        card.Controls.Add(inner);
        return card;
    }

    private static Label BuildOptionLabel(
        string key,
        string optionText,
        string correctOption,
        bool showAnswer,
        int maxWidth)
    {
        var isCorrect = showAnswer && string.Equals(key, correctOption, StringComparison.OrdinalIgnoreCase);

        var label = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(maxWidth, 0),
            Padding = new Padding(8, 5, 8, 5),
            Margin = new Padding(0, 2, 0, 2),
            Font = isCorrect ? UiStyle.BodyBoldFont : UiStyle.BodyFont,
            BackColor = isCorrect ? UiStyle.SuccessBack : UiStyle.Surface,
            ForeColor = isCorrect ? UiStyle.SuccessText : UiStyle.Text,
            Text = isCorrect
                ? $"{key}. {optionText}   ✓ Đáp án đúng"
                : $"{key}. {optionText}"
        };

        if (isCorrect)
        {
            UiStyle.StyleBadge(label, 6);
        }

        return label;
    }

    private void DisposeQuestionPanelControls()
    {
        if (_questionsPanel.Controls.Count == 0)
        {
            return;
        }

        // Giải phóng ảnh và toàn bộ control con cùng handle GDI trước khi xóa khỏi panel.
        var oldControls = _questionsPanel.Controls.Cast<Control>().ToArray();
        foreach (var ctrl in oldControls)
        {
            DisposeControlImages(ctrl);
            ctrl.Dispose();
        }
    }

    private static void DisposeControlImages(Control root)
    {
        if (root is PictureBox pictureBox)
        {
            var img = pictureBox.Image;
            pictureBox.Image = null;
            img?.Dispose();
        }

        foreach (Control child in root.Controls)
        {
            DisposeControlImages(child);
        }
    }

    private void UpdateHeaderWrapWidths()
    {
        if (_headerCard.ClientSize.Width <= 0)
        {
            return;
        }

        var innerWidth = _headerCard.ClientSize.Width - _headerCard.Padding.Horizontal;
        var badgeWidth = Math.Max(_lblStatusBadge.Width, _lblStatusBadge.PreferredWidth);
        _lblPaperTitle.MaximumSize = new Size(Math.Max(200, innerWidth - badgeWidth - 28), 0);
        _lblMetaInfo.MaximumSize = new Size(Math.Max(240, innerWidth - 4), 0);
    }

    private int GetTargetCardWidth()
    {
        var available = _questionsPanel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 8;
        return Math.Max(320, available);
    }

    private void AdjustCardWidths()
    {
        UpdateHeaderWrapWidths();
        if (_questionsPanel.Controls.Count == 0)
        {
            return;
        }

        var cardWidth = GetTargetCardWidth();
        var textMaxWidth = Math.Max(240, cardWidth - 34);

        _questionsPanel.SuspendLayout();
        try
        {
            foreach (Control ctrl in _questionsPanel.Controls)
            {
                if (ctrl is not Panel card)
                {
                    continue;
                }

                card.MinimumSize = new Size(cardWidth, 0);
                card.MaximumSize = new Size(cardWidth, 0);
                card.Width = cardWidth;
                foreach (Control innerCtrl in card.Controls)
                {
                    if (innerCtrl is FlowLayoutPanel flow)
                    {
                        foreach (Control child in flow.Controls)
                        {
                            if (child is Label lbl)
                            {
                                lbl.MaximumSize = new Size(textMaxWidth, 0);
                            }
                            else if (child is PictureBox pic)
                            {
                                pic.Width = textMaxWidth;
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            _questionsPanel.ResumeLayout();
        }
    }

    private void RegeneratePaperQuestions()
    {
        if (_summary.IsLocked)
        {
            MessageBox.Show(
                this,
                $"Đề thi '{_summary.PaperCode}' đã được gán vào ca thi, không thể rút lại câu hỏi.",
                "Đề thi đã khóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc muốn rút lại ngẫu nhiên toàn bộ {_summary.TotalQuestions} câu hỏi cho đề '{_summary.PaperCode}'?\nDanh sách câu hỏi hiện tại sẽ bị thay thế.",
            "Xác nhận rút lại câu ngẫu nhiên",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _examPaperService.RegenerateQuestions(_summary.Id);
            QuestionsRegenerated = true;
            LoadQuestions();

            MessageBox.Show(
                this,
                $"Đã rút lại bộ câu hỏi mới cho đề '{_summary.PaperCode}'.",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể rút lại câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static string FormatDifficulty(string difficulty) => difficulty switch
    {
        "Easy" => "Dễ",
        "Medium" => "Trung bình",
        "Hard" => "Khó",
        _ => difficulty
    };
}
