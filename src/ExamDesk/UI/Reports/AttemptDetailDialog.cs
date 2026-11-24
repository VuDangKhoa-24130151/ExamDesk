#nullable enable
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

// Bạn 2: Hộp thoại tra cứu chi tiết bài làm và lịch sử chọn đáp án (chỉ đọc)
internal sealed class AttemptDetailDialog : Form
{
    private readonly GradingReviewService _gradingService = new();

    public AttemptDetailDialog(AttemptSummaryItem item)
    {
        AutoScaleMode = AutoScaleMode.Font;
        Text = $"Chi tiết bài làm & Phúc khảo — {item.StudentCode} ({item.StudentName})";
        ClientSize = new Size(1040, 620);
        MinimumSize = new Size(760, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = UiStyle.Background;
        Font = UiStyle.BodyFont;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 1,
            RowCount = 3
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Hàng 0: Thông tin tóm tắt bài thi
        var infoPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            BackColor = UiStyle.Surface,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 14)
        };
        var lblInfo = new Label
        {
            AutoSize = true,
            ForeColor = UiStyle.Text,
            Text = $"Sinh viên: {item.StudentCode} — {item.StudentName}   |   Lớp: {item.ClassName}\n" +
                   $"Ca thi: {item.SessionName}   |   Đề thi: {item.PaperTitle}\n" +
                   $"Nộp lúc: {item.SubmittedAtText} ({item.SubmitMethodText})   |   " +
                   $"Kết quả: {item.Score:0.00} điểm — Đúng: {item.CorrectCount}/{item.TotalQuestions}, Sai: {item.WrongCount}, Bỏ trống: {item.BlankCount}   (Chế độ tra cứu chỉ đọc)"
        };
        infoPanel.Controls.Add(lblInfo);

        // Hàng 1: Bảng chi tiết từng câu, đáp án A–D và lịch sử đổi đáp án
        var details = _gradingService.GetAttemptReviewDetails(item.AttemptId);
        var dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
        UiStyle.StyleGrid(dgv);
        dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        dgv.DefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.QuestionOrder),
            HeaderText = "Câu",
            FillWeight = 35
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.QuestionContent),
            HeaderText = "Nội dung câu hỏi",
            FillWeight = 160
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.OptionsFormatted),
            HeaderText = "Các lựa chọn (A–D)",
            FillWeight = 160
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.SelectedOption),
            HeaderText = "SV chọn",
            FillWeight = 55
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.CorrectOption),
            HeaderText = "Đáp án đúng",
            FillWeight = 60
        });
        var resultCol = new DataGridViewTextBoxColumn
        {
            Name = "ResultStatusColumn",
            DataPropertyName = nameof(AttemptReviewDetailItem.ResultStatus),
            HeaderText = "Kết quả",
            FillWeight = 65
        };
        dgv.Columns.Add(resultCol);
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.ChangeHistory),
            HeaderText = "Lịch sử đổi đáp án",
            FillWeight = 110
        });

        dgv.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex != resultCol.Index || e.CellStyle is null)
                return;

            string? status = e.Value as string;
            if (status == "Đúng")
            {
                e.CellStyle.ForeColor = UiStyle.SuccessText;
                e.CellStyle.BackColor = UiStyle.SuccessBack;
            }
            else if (status == "Sai")
            {
                e.CellStyle.ForeColor = UiStyle.ErrorText;
                e.CellStyle.BackColor = UiStyle.ErrorBack;
            }
            else
            {
                e.CellStyle.ForeColor = UiStyle.WarningText;
                e.CellStyle.BackColor = UiStyle.WarningBack;
            }
        };

        dgv.DataSource = details;

        // Hàng 2: Nút Đóng
        var bottomPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0, 12, 0, 0)
        };
        var btnClose = new Button { Text = "Đóng", Width = 100 };
        UiStyle.StyleSecondaryButton(btnClose);
        btnClose.Click += (_, _) => Close();
        CancelButton = btnClose;
        bottomPanel.Controls.Add(btnClose);

        root.Controls.Add(infoPanel, 0, 0);
        root.Controls.Add(dgv, 0, 1);
        root.Controls.Add(bottomPanel, 0, 2);
        Controls.Add(root);

        Shown += (_, _) => UiStyle.ClampToWorkingArea(this);
    }
}
