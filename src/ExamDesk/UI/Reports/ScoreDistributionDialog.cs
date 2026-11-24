#nullable enable
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

// Bạn 2: Hộp thoại thống kê phổ điểm theo danh sách bài thi đang lọc.
internal sealed class ScoreDistributionDialog : Form
{
    public ScoreDistributionDialog(ScoreDistributionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        AutoScaleMode = AutoScaleMode.Font;
        Text = "Thống kê phổ điểm";
        ClientSize = new Size(760, 460);
        MinimumSize = new Size(640, 400);
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

        // Hàng 0: Tóm tắt các chỉ số thống kê
        var summaryPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            BackColor = UiStyle.Surface,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 14)
        };
        var lblSummary = new Label
        {
            AutoSize = true,
            ForeColor = UiStyle.Text,
            Text = summary.TotalCount == 0
                ? $"Phạm vi thống kê: {summary.FilterSummary}   |   Tổng số bài: 0"
                : $"Phạm vi thống kê: {summary.FilterSummary}   |   Tổng số bài: {summary.TotalCount}\n" +
                  $"Điểm trung bình: {summary.AverageScore:0.00}   |   Cao nhất: {summary.MaxScore:0.00}   |   " +
                  $"Thấp nhất: {summary.MinScore:0.00}   |   Đạt (>= 5.0): {summary.PassCount}/{summary.TotalCount} ({summary.PassRatePercent:0.0}%)"
        };
        summaryPanel.Controls.Add(lblSummary);

        // Hàng 1: Bảng phân bố phổ điểm theo 4 khoảng hoặc trạng thái trống
        var contentHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiStyle.Surface
        };

        if (summary.TotalCount == 0)
        {
            var lblEmpty = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = UiStyle.TextMuted,
                Text = "Chưa có bài thi nào trong phạm vi đã chọn để thống kê phổ điểm."
            };
            contentHost.Controls.Add(lblEmpty);
        }
        else
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
            UiStyle.StyleGrid(dgv);
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScoreBucketItem.Label),
                HeaderText = "Khoảng điểm",
                FillWeight = 140
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScoreBucketItem.Count),
                HeaderText = "Số bài",
                FillWeight = 65
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScoreBucketItem.PercentageText),
                HeaderText = "Tỷ lệ",
                FillWeight = 75
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScoreBucketItem.VisualBar),
                HeaderText = "Biểu đồ trực quan",
                FillWeight = 160
            });
            dgv.DataSource = summary.Buckets.ToList();
            contentHost.Controls.Add(dgv);
        }

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

        root.Controls.Add(summaryPanel, 0, 0);
        root.Controls.Add(contentHost, 0, 1);
        root.Controls.Add(bottomPanel, 0, 2);
        Controls.Add(root);

        Shown += (_, _) => UiStyle.ClampToWorkingArea(this);
    }
}
