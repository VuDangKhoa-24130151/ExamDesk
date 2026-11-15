#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

// Bạn 2: Hộp thoại thống kê phổ điểm theo danh sách bài thi đang lọc.
internal sealed class ScoreDistributionDialog : Form
{
    public ScoreDistributionDialog(IReadOnlyList<AttemptResultItem> attempts, string filterSummary)
    {
        AutoScaleMode = AutoScaleMode.Font;
        Text = "Thống kê phổ điểm (GV-14)";
        ClientSize = new Size(720, 460);
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

        int total = attempts.Count;
        double avg = total == 0 ? 0 : Math.Round(attempts.Average(a => a.Diem), 2);
        double max = total == 0 ? 0 : attempts.Max(a => a.Diem);
        double min = total == 0 ? 0 : attempts.Min(a => a.Diem);
        int passCount = attempts.Count(a => a.Diem >= 5.0);

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
            Text = $"Phạm vi thống kê: {filterSummary}   |   Tổng số bài: {total}\n" +
                   $"Điểm trung bình: {avg:0.00}   |   Cao nhất: {max:0.00}   |   Thấp nhất: {min:0.00}   |   Đạt (>= 5.0): {passCount}/{total}"
        };
        summaryPanel.Controls.Add(lblSummary);

        // Hàng 1: Bảng phân bố phổ điểm theo 4 khoảng
        var dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
        UiStyle.StyleGrid(dgv);
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MucDiem", HeaderText = "Khoảng điểm", FillWeight = 130 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", HeaderText = "Số bài", FillWeight = 70 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TyLe", HeaderText = "Tỷ lệ", FillWeight = 80 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BieuDo", HeaderText = "Biểu đồ trực quan", FillWeight = 160 });
        dgv.DataSource = BuildBuckets(attempts);

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
        root.Controls.Add(dgv, 0, 1);
        root.Controls.Add(bottomPanel, 0, 2);
        Controls.Add(root);
    }

    private static List<ScoreBucketRow> BuildBuckets(IReadOnlyList<AttemptResultItem> attempts)
    {
        int total = attempts.Count;
        (string Label, int Count)[] groups =
        {
            ("Dưới 5.0 (Chưa đạt)", attempts.Count(a => a.Diem < 5.0)),
            ("5.0 – 6.4 (Trung bình)", attempts.Count(a => a.Diem >= 5.0 && a.Diem < 6.5)),
            ("6.5 – 7.9 (Khá)", attempts.Count(a => a.Diem >= 6.5 && a.Diem < 8.0)),
            ("8.0 – 10.0 (Giỏi)", attempts.Count(a => a.Diem >= 8.0))
        };

        return groups.Select(g =>
        {
            double pct = total == 0 ? 0 : Math.Round(g.Count * 100.0 / total, 1);
            string bar = new string('█', g.Count * 3);
            return new ScoreBucketRow
            {
                MucDiem = g.Label,
                SoLuong = g.Count,
                TyLe = $"{pct:0.0}%",
                BieuDo = bar
            };
        }).ToList();
    }

    private sealed class ScoreBucketRow
    {
        public string MucDiem { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public string TyLe { get; set; } = string.Empty;
        public string BieuDo { get; set; } = string.Empty;
    }
}