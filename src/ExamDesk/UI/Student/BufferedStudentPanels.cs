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

// Ghép việc vẽ các control con trong vùng chuyển màn, kể cả TextBox native.
internal sealed class StudentScreenHost : Panel
{
    public StudentScreenHost() => DoubleBuffered = true;

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            parameters.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
            return parameters;
        }
    }
}