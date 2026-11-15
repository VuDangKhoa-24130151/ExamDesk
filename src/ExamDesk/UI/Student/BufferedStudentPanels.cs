namespace ExamDesk.UI.Student;

// Vẽ nền và nội dung của từng panel vào cùng bộ đệm khi đổi bố cục.
internal sealed class BufferedStudentPanel : Panel
{
    public BufferedStudentPanel() => DoubleBuffered = true;
}

internal class BufferedStudentTable : TableLayoutPanel
{
    public BufferedStudentTable() => DoubleBuffered = true;
}

internal sealed class BufferedStudentFlow : FlowLayoutPanel
{
    public BufferedStudentFlow() => DoubleBuffered = true;
}

// Mỗi control tự double-buffer; không ép WS_EX_COMPOSITED lên cả cây control.
// Compositing lồng nhau dễ để lại vùng cuộn chưa được vẽ khi khôi phục cửa sổ.
internal sealed class StudentScreenHost : Panel
{
    public StudentScreenHost() => DoubleBuffered = true;
}
