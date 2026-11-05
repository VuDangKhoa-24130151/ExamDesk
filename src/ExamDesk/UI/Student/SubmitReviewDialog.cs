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

        for (int index = 0; index < attempt.Answers.Length; index++)
        {
            AddQuestionButton(index, attempt.Answers[index].HasValue);
        }

        if (_numbers.Controls.Count == 0)
            _numbers.Controls.Add(StudentLayout.Label("Chưa có câu hỏi."));

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

    private void AddQuestionButton(int index, bool answered)
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
        number.AccessibleDescription = answered ? "Đã trả lời" : "Chưa trả lời";
        number.BackColor = answered ? Color.FromArgb(234, 246, 239) : Color.White;
        number.ForeColor = answered ? Color.FromArgb(20, 108, 67) : ExamDesk.UI.Shared.UiStyle.Text;
        _numbers.Controls.Add(number);
    }
}
