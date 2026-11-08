namespace ExamDesk.UI.Student;
public partial class SubmitReviewDialog : Form
{
    public int? QuestionToReview { get; private set; }
    public SubmitReviewDialog() : this(new StudentAttemptState()) { }
    internal SubmitReviewDialog(StudentAttemptState attempt)
    {
        InitializeComponent();
        _answered.Text = attempt.Answered.ToString();
        _unanswered.Text = (attempt.Answers.Length - attempt.Answered).ToString();
        _flagged.Text = attempt.Flags.Count(flag => flag).ToString();

        for (int index = 0; index < attempt.Answers.Length; index++)
        {
            if (!attempt.Answers[index].HasValue) AddQuestionButton(_numbers, index, "Chưa trả lời");
            if (attempt.Flags[index]) AddQuestionButton(_flaggedNumbers, index, "Đã đánh dấu");
        }

        if (_numbers.Controls.Count == 0)
            _numbers.Controls.Add(StudentLayout.Label("Bạn đã trả lời tất cả các câu."));

        if (_flaggedNumbers.Controls.Count == 0)
            _flaggedNumbers.Controls.Add(StudentLayout.Label("Bạn chưa đánh dấu câu nào."));

        void UpdateClock() =>
            _warning.Text = $"Còn {attempt.Remaining} · Sau khi nộp, bạn không thể sửa đáp án.";

        UpdateClock();
        _clock.Tick += (_, _) => UpdateClock();
        Shown += (_, _) =>
        {
            Shared.UiStyle.ClampToWorkingArea(this);
            _continueButton.Focus();
            _clock.Start();
        };
    }

    private static void FitQuestionButton(Button button)
        => button.MinimumSize = new Size(StudentLayout.Scale(button, 48), StudentLayout.Scale(button, 38));

    private void AddQuestionButton(FlowLayoutPanel list, int index, string description)
    {
        var number = StudentLayout.Button((index + 1).ToString(), (_, _) =>
        {
            QuestionToReview = index;
            DialogResult = DialogResult.Cancel;
        });
        number.HandleCreated += (_, _) => FitQuestionButton(number);
        number.DpiChangedAfterParent += (_, _) => FitQuestionButton(number);
        number.Margin = new Padding(0, 0, 8, 8);
        number.AccessibleName = $"Quay lại câu {index + 1}";
        number.AccessibleDescription = description;
        number.ForeColor = Shared.UiStyle.Primary;
        list.Controls.Add(number);
    }
}
