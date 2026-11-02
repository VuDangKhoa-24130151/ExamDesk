namespace ExamDesk.UI.Student;
public partial class SubmitReviewDialog : Form
{
    public int? QuestionToReview { get; private set; }
    public SubmitReviewDialog() : this(new StudentPreviewAttempt()) { }
    internal SubmitReviewDialog(StudentPreviewAttempt attempt) => InitializeComponent(attempt);
}
