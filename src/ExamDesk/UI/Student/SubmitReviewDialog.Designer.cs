namespace ExamDesk.UI.Student;
partial class SubmitReviewDialog
{
    private void InitializeComponent(StudentPreviewAttempt attempt)
    {
        Text = "Kiểm tra trước khi nộp"; Font = new Font("Segoe UI", 10);
        AutoScaleMode = AutoScaleMode.Font; StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(680, 510); MinimumSize = new Size(600, 450);
        MaximizeBox = false; MinimizeBox = false;
        var body = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), AutoScroll = true,
            FlowDirection = FlowDirection.TopDown, WrapContents = false };
        body.Controls.Add(StudentLayout.Label("Bạn muốn nộp bài?", 18, FontStyle.Bold));
        body.Controls.Add(StudentLayout.Label($"Đã trả lời: {attempt.Answered}/30     Chưa trả lời: {30 - attempt.Answered}     Đánh dấu: {attempt.Flags.Count(flag => flag)}"));
        body.Controls.Add(StudentLayout.Label("Câu chưa trả lời — chọn số câu để quay lại:"));
        var numbers = new FlowLayoutPanel { Width = 610, Height = 160, AutoScroll = true };
        for (int i = 0; i < 30; i++)
        {
            if (attempt.Answers[i].HasValue) continue;
            int index = i;
            var number = new Button { Text = (i + 1).ToString(), Width = 48, Height = 36 };
            number.Click += (_, _) => { QuestionToReview = index; DialogResult = DialogResult.Cancel; };
            numbers.Controls.Add(number);
        }
        if (numbers.Controls.Count == 0) numbers.Controls.Add(StudentLayout.Label("Bạn đã trả lời tất cả các câu."));
        body.Controls.Add(numbers);
        body.Controls.Add(StudentLayout.Label("Đồng hồ vẫn chạy. Sau khi xác nhận, bạn không thể sửa đáp án."));
        var actions = new FlowLayoutPanel { AutoSize = true, Width = 610 };
        var cancel = StudentLayout.Button("Tiếp tục &làm bài", (_, _) => DialogResult = DialogResult.Cancel);
        actions.Controls.Add(cancel);
        actions.Controls.Add(StudentLayout.Button("&Xác nhận nộp bài", (_, _) => DialogResult = DialogResult.OK, true));
        body.Controls.Add(actions); Controls.Add(body); CancelButton = cancel;
        body.SizeChanged += (_, _) => { numbers.Width = actions.Width = Math.Max(200, body.ClientSize.Width - 60); };
    }
}
