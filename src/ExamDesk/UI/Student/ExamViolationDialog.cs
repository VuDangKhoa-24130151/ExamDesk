using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

internal sealed class ExamViolationDialog : Form
{
    internal ExamViolationDialog(int count, string reason)
    {
        Text = "Cảnh cáo vi phạm quy tắc thi";
        Font = new Font("Segoe UI", 11);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScaleDimensions = new SizeF(8, 20);
        ClientSize = new Size(560, 270);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ControlBox = false;
        ShowInTaskbar = false;
        BackColor = UiStyle.WarningBack;
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label
        {
            Text = $"Cảnh cáo lần {count}/3 · {reason}", AutoSize = true,
            MaximumSize = new Size(510, 0), ForeColor = UiStyle.WarningText,
            Margin = new Padding(0, 0, 0, 16)
        });
        layout.Controls.Add(new Label
        {
            Text = "Bạn đang vi phạm quy tắc làm bài thi, nếu bị cảnh cáo quá 2 lần bài làm của bạn sẽ được nộp tự động",
            Dock = DockStyle.Fill, ForeColor = UiStyle.Text
        });
        var confirm = new Button
        {
            Text = "Xác nhận, đã hiểu", AutoSize = true, MinimumSize = new Size(190, 42),
            Anchor = AnchorStyles.Right, Margin = new Padding(0, 16, 0, 0)
        };
        confirm.Click += (_, _) => Close();
        layout.Controls.Add(confirm);
        Controls.Add(layout);
        AcceptButton = confirm;
    }
}
