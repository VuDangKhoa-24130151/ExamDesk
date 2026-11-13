#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

// Bạn 2: Hộp thoại tra cứu chi tiết bài làm và lịch sử chọn đáp án 
internal sealed class AttemptDetailDialog : Form
{
    public AttemptDetailDialog(AttemptResultItem item)
    {
        AutoScaleMode = AutoScaleMode.Font;
        Text = $"Chi tiết bài làm & Phúc khảo — {item.Mssv} ({item.HoTen})";
        ClientSize = new Size(820, 540);
        MinimumSize = new Size(700, 440);
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
            Text = $"Sinh viên: {item.Mssv} — {item.HoTen}   |   Lớp: {item.Lop}\n" +
                   $"Ca thi: {item.Ca}   |   Nộp lúc: {item.NopLuc:HH:mm dd/MM/yyyy} ({item.CachNop})\n" +
                   $"Kết quả: {item.Diem:0.00} điểm — Đúng: {item.CauDung}/{item.SoCau}, Sai: {item.CauSai}, Bỏ trống: {item.CauBoTrong}   (Chế độ tra cứu chỉ đọc)"
        };
        infoPanel.Controls.Add(lblInfo);

        // Hàng 1: Bảng chi tiết từng câu và lịch sử đổi đáp án
        var dgv = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false };
        UiStyle.StyleGrid(dgv);
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cau", HeaderText = "Câu", FillWeight = 50 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LuaChon", HeaderText = "SV chọn", FillWeight = 75 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DapAnDung", HeaderText = "Đáp án đúng", FillWeight = 85 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KetQua", HeaderText = "Kết quả", FillWeight = 85 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LichSu", HeaderText = "Lịch sử lựa chọn", FillWeight = 130 });
        dgv.DataSource = ReportsPreviewData.AnswerDetails(item);

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
    }
}