namespace ExamDesk.UI.Student;

public partial class ExamInstructionsControl : UserControl
{
    public event EventHandler? BackRequested;
    public event EventHandler? StartRequested;

    public ExamInstructionsControl()
    {
        InitializeComponent();
        _readCheckBox.CheckedChanged += (_, _) => _startButton.Enabled = _readCheckBox.Checked;
        _startButton.Click += (_, _) => StartRequested?.Invoke(this, EventArgs.Empty);
        Resize += (_, _) => CenterContent();
        CenterContent();
    }

    public ExamInstructionsControl(ExamDesk.Models.User student, ExamDesk.Models.ExamSession session, ExamDesk.Models.ExamPaper paper) : this()
    {
        // Giữ bố cục hai thẻ; thay toàn bộ dữ liệu demo trước khi hiển thị.
        var values=new Dictionary<string,string>
        {
            ["Cuối kỳ .NET – Ca 01"] = StudentPreviewAttempt.DisplayTitle(session.SessionName),
            ["Vũ Đăng Khoa · 24130151"]=$"{student.FullName} · {student.StudentCode}",
            ["Lớp DH23DT"]=$"Lớp {student.ClassName} · Phòng {session.RoomName}",
            ["30 câu · 45 phút · Thang điểm 10"]=$"{paper.TotalQuestions} câu · {paper.DurationMinutes} phút · Thang điểm 10",
            ["01  Bài thi có 30 câu trắc nghiệm, thời gian 45 phút."]=$"01  Bài thi có {paper.TotalQuestions} câu trắc nghiệm, thời gian {paper.DurationMinutes} phút.",
            ["05  Lựa chọn chỉ giữ trong cửa sổ hiện tại, chưa lưu xuống máy."]="05  Mỗi thay đổi đáp án được lưu. Đóng cửa sổ không dừng đồng hồ.",
            ["06  Hết giờ, vùng làm bài sẽ khóa và bài được tự động nộp."]="06  Hết giờ, bài tự khóa để gửi nộp. Nếu chưa gửi được, bài vẫn được giữ lại.",
            ["Điểm = số câu đúng / 30 × 10.\nLàm tròn hai chữ số thập phân; câu sai và bỏ trống không có điểm."]=$"Điểm = số câu đúng / {paper.TotalQuestions} × 10.\nCâu sai và bỏ trống không có điểm. Chỉ hiện điểm sau khi chấm và lưu thành công.",
            ["Chưa bắt đầu đếm giờ\nĐồng hồ chạy sau khi tạo lượt thi thành công."]="Lượt mới bắt đầu tính giờ khi bấm Bắt đầu.\nNếu khôi phục, bài tiếp tục theo hạn nộp cũ."
        };
        void ReplaceLabels(Control parent)
        {
            foreach(Control control in parent.Controls)
            {
                if(control is Label label && values.TryGetValue(label.Text,out var text)) label.Text=text;
                ReplaceLabels(control);
            }
        }
        ReplaceLabels(this);
        _startButton.Text="Bắt đầu / Khôi phục lượt thi →";
    }
    private void CenterContent()
    {
        int maxWidth = (int)(1120 * DeviceDpi / 96f);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    // Nhãn dài tự xuống dòng theo bề ngang của khung chứa nó.
    private static void WrapLabels(FlowLayoutPanel panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;
        foreach (Control control in panel.Controls)
        {
            if (control is Label label)
                label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
        }
    }
}
