namespace ExamDesk.UI.Student;

// Vẽ nền và nội dung của từng panel vào cùng bộ đệm khi đổi bố cục.
internal sealed class BufferedStudentPanel : Panel
{
    public BufferedStudentPanel() => DoubleBuffered = true;
}

internal sealed class BufferedStudentTable : TableLayoutPanel
{
    public BufferedStudentTable() => DoubleBuffered = true;
}

internal sealed class BufferedStudentFlow : FlowLayoutPanel
{
    public BufferedStudentFlow() => DoubleBuffered = true;
}
