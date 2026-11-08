namespace ExamDesk.UI.Student;
public partial class SubmitReviewDialog : Form
{
    public int? QuestionToReview { get; private set; }
    public SubmitReviewDialog() : this(new StudentPreviewAttempt()) { }
    internal SubmitReviewDialog(StudentPreviewAttempt attempt)
    {
        InitializeComponent();
        _answered.Text = attempt.Answered.ToString();
        _unanswered.Text = (attempt.Answers.Length - attempt.Answered).ToString();
        _flagged.Text = attempt.Flags.Count(flag => flag).ToString();

        for (int index = 0; index < attempt.Answers.Length; index++)
        {
            if (!attempt.Answers[index].HasValue) AddQuestionButton(index);
        }

        if (_numbers.Controls.Count == 0)
            _numbers.Controls.Add(StudentLayout.Label("Bạn đã trả lời tất cả các câu."));

        void UpdateClock() =>
            _warning.Text = $"Còn {attempt.Remaining} · Sau khi nộp, bạn không thể sửa đáp án.";

        UpdateClock();
        _clock.Tick += (_, _) => UpdateClock();
        Shown += (_, _) =>
        {
            _continueButton.Focus();
            _clock.Start();
        };
    }

    private void AddQuestionButton(int index)
    {
        var number = StudentLayout.Button((index + 1).ToString(), (_, _) =>
        {
            QuestionToReview = index;
            DialogResult = DialogResult.Cancel;
        });
        number.AutoSize = false;
        number.MinimumSize = Size.Empty;
        number.Padding = Padding.Empty;
        number.Size = new Size(48, 38);
        number.Margin = new Padding(0, 0, 8, 8);
        number.AccessibleName = $"Quay lại câu {index + 1}";
        number.AccessibleDescription = "Chưa trả lời";
        number.ForeColor = Shared.UiStyle.Primary;
        _numbers.Controls.Add(number);
    }
}
