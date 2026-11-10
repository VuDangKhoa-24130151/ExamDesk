namespace ExamDesk.UI.Student;
public partial class SubmitReviewDialog : Form
{
    public int? QuestionToReview { get; private set; }
    public SubmitReviewDialog()
    {
        InitializeComponent();
#if DEBUG
        ShowAttempt(new StudentAttemptState());
#else
        _confirmButton.Enabled = false;
#endif
    }

    internal SubmitReviewDialog(StudentAttemptState attempt)
    {
        InitializeComponent();
        ShowAttempt(attempt);
    }

    private void ShowAttempt(StudentAttemptState attempt)
    {
        _answered.Text = attempt.Answered.ToString();
        _unanswered.Text = (attempt.Answers.Length - attempt.Answered).ToString();
        _flagged.Text = attempt.Flags.Count(flag => flag).ToString();

        for (int index = 0; index < attempt.Answers.Length; index++)
        {
            bool unanswered = !attempt.Answers[index].HasValue;
            bool flagged = attempt.Flags[index];
            if (!unanswered && !flagged) continue;

            string status = unanswered
                ? (flagged ? "Chưa trả lời · Đánh dấu" : "Chưa trả lời")
                : "Đánh dấu";
            AddQuestionButton(index, status);
        }

        if (_numbers.Controls.Count == 0)
            _numbers.Controls.Add(StudentLayout.Label("Không có câu chưa trả lời hoặc đã đánh dấu."));

        void UpdateClock() =>
            _warning.Text = $"Còn {attempt.Remaining} · Sau khi nộp bài, bạn không thể sửa đáp án.";

        UpdateClock();
        _clock.Tick += (_, _) => UpdateClock();
        Shown += (_, _) =>
        {
            Shared.UiStyle.ClampToWorkingArea(this);
            _continueButton.Focus();
            _clock.Start();
        };
    }

    private void AddQuestionButton(int index, string description)
    {
        var number = StudentLayout.Button($"Câu {index + 1} · {description}", (_, _) =>
        {
            QuestionToReview = index;
            DialogResult = DialogResult.Cancel;
        });
        number.Margin = new Padding(0, 0, 8, 8);
        number.AccessibleName = $"Quay lại câu {index + 1}";
        number.AccessibleDescription = description;
        number.ForeColor = Shared.UiStyle.Primary;
        _numbers.Controls.Add(number);
    }
}
