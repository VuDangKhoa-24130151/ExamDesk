#nullable enable
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Reports;

// Bạn 2: Hộp thoại tra cứu chi tiết bài làm để phúc khảo (chỉ đọc)
internal sealed class AttemptDetailDialog : Form
{
    private readonly GradingReviewService _gradingService = new();
    private readonly List<AttemptReviewDetailItem> _allDetails;
    private readonly DataGridView _dgv = new();
    private readonly Label _lblEmpty = new();
    private readonly ComboBox _cboFilter = new();
    private readonly Label _lblFilterCount = new();
    private DataGridViewTextBoxColumn _selectedCol = null!;
    private DataGridViewTextBoxColumn _resultCol = null!;

    public AttemptDetailDialog(AttemptSummaryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        AutoScaleMode = AutoScaleMode.Font;
        Text = $"Chi tiết bài làm & Phúc khảo — {item.StudentCode} ({item.StudentName})";
        ClientSize = new Size(1060, 640);
        MinimumSize = new Size(780, 500);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = UiStyle.Background;
        Font = UiStyle.BodyFont;

        _allDetails = _gradingService.GetAttemptReviewDetails(item.AttemptId);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            ColumnCount = 1,
            RowCount = 4
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Hàng 0: Thẻ thông tin tóm tắt bài thi (có viền mảnh và phân tầng thông tin rõ)
        root.Controls.Add(BuildSummaryCard(item), 0, 0);

        // Hàng 1: Thanh lọc nhanh theo trạng thái câu (Đúng / Sai / Bỏ trống)
        root.Controls.Add(BuildFilterBar(), 0, 1);

        // Hàng 2: Bảng chi tiết từng câu và đáp án A–D
        root.Controls.Add(BuildGridPanel(), 0, 2);

        // Hàng 3: Nút Đóng
        var bottomPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0, 12, 0, 0)
        };
        var btnClose = new Button { Text = "Đóng", Width = 100, TabIndex = 3 };
        UiStyle.StyleSecondaryButton(btnClose);
        btnClose.Click += (_, _) => Close();
        CancelButton = btnClose;
        bottomPanel.Controls.Add(btnClose);
        root.Controls.Add(bottomPanel, 0, 3);

        Controls.Add(root);
        ApplyFilter();

        Shown += (_, _) => UiStyle.ClampToWorkingArea(this);
    }

    private Control BuildSummaryCard(AttemptSummaryItem item)
    {
        var borderHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            BackColor = UiStyle.Border,
            Padding = new Padding(1),
            Margin = new Padding(0, 0, 0, 12)
        };

        var card = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            BackColor = UiStyle.Surface,
            Padding = new Padding(16, 12, 16, 12),
            ColumnCount = 2,
            RowCount = 3
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        card.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var lblStudent = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = UiStyle.Text,
            Margin = new Padding(0, 0, 12, 4),
            Text = $"{item.StudentName} ({item.StudentCode})  •  Lớp: {item.ClassName}"
        };

        var lblReadOnlyBadge = new Label
        {
            AutoSize = true,
            Font = UiStyle.SmallFont,
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Background,
            Padding = new Padding(8, 3, 8, 3),
            Margin = new Padding(0, 0, 0, 4),
            Text = "Tra cứu chỉ đọc"
        };

        var lblExamInfo = new Label
        {
            AutoSize = true,
            ForeColor = UiStyle.TextMuted,
            Margin = new Padding(0, 0, 0, 8),
            Text = $"Ca thi: {item.SessionName}   •   Đề thi: {item.PaperTitle}   •   Nộp lúc: {item.SubmittedAtText} ({item.SubmitMethodText})"
        };

        var statsBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            Margin = new Padding(0)
        };
        statsBar.Controls.Add(CreateStatPill($"Điểm: {item.Score:0.00}", UiStyle.Primary, UiStyle.Background));
        statsBar.Controls.Add(CreateStatPill($"Đúng: {item.CorrectCount}/{item.TotalQuestions}", UiStyle.SuccessText, UiStyle.SuccessBack));
        statsBar.Controls.Add(CreateStatPill($"Sai: {item.WrongCount}", UiStyle.ErrorText, UiStyle.ErrorBack));
        statsBar.Controls.Add(CreateStatPill($"Bỏ trống: {item.BlankCount}", UiStyle.WarningText, UiStyle.WarningBack));

        card.Controls.Add(lblStudent, 0, 0);
        card.Controls.Add(lblReadOnlyBadge, 1, 0);
        card.Controls.Add(lblExamInfo, 0, 1);
        card.SetColumnSpan(lblExamInfo, 2);
        card.Controls.Add(statsBar, 0, 2);
        card.SetColumnSpan(statsBar, 2);

        borderHost.Controls.Add(card);
        return borderHost;
    }

    private static Label CreateStatPill(string text, Color foreColor, Color backColor) => new()
    {
        AutoSize = true,
        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
        ForeColor = foreColor,
        BackColor = backColor,
        Padding = new Padding(10, 4, 10, 4),
        Margin = new Padding(0, 0, 8, 0),
        Text = text
    };

    private Control BuildFilterBar()
    {
        var borderHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            BackColor = UiStyle.Border,
            Padding = new Padding(1),
            Margin = new Padding(0, 0, 0, 12)
        };

        var bar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            BackColor = UiStyle.Surface,
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(0)
        };

        var lblFilter = new Label
        {
            AutoSize = true,
            ForeColor = UiStyle.Text,
            Margin = new Padding(0, 6, 8, 0),
            Text = "Lọc câu hỏi:"
        };

        _cboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboFilter.Width = 190;
        _cboFilter.TabIndex = 0;
        _cboFilter.Margin = new Padding(0, 2, 12, 2);
        _cboFilter.Items.AddRange(["Tất cả câu hỏi", "Chỉ câu sai", "Chỉ câu bỏ trống", "Chỉ câu đúng"]);
        _cboFilter.SelectedIndex = 0;
        _cboFilter.SelectedIndexChanged += (_, _) => ApplyFilter();

        _lblFilterCount.AutoSize = true;
        _lblFilterCount.ForeColor = UiStyle.TextMuted;
        _lblFilterCount.Margin = new Padding(0, 6, 0, 0);

        bar.Controls.Add(lblFilter);
        bar.Controls.Add(_cboFilter);
        bar.Controls.Add(_lblFilterCount);
        borderHost.Controls.Add(bar);
        return borderHost;
    }

    private Control BuildGridPanel()
    {
        var borderHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiStyle.Border,
            Padding = new Padding(1)
        };

        var host = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiStyle.Surface
        };

        UiStyle.StyleGrid(_dgv);
        _dgv.Dock = DockStyle.Fill;
        _dgv.BorderStyle = BorderStyle.None;
        _dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        _dgv.GridColor = UiStyle.Border;
        _dgv.TabIndex = 1;
        _dgv.AutoGenerateColumns = false;
        _dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        _dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FAFBFD");
        _dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _dgv.DefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
        _dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        var orderCol = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.QuestionOrder),
            HeaderText = "Câu",
            FillWeight = 36,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(6, 8, 6, 8) }
        };
        var contentCol = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.QuestionContent),
            HeaderText = "Nội dung câu hỏi",
            FillWeight = 185,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(14, 8, 14, 8) }
        };
        var optionsCol = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.OptionsFormatted),
            HeaderText = "Các lựa chọn (A–D)",
            FillWeight = 185,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(14, 8, 14, 8) }
        };
        _selectedCol = new DataGridViewTextBoxColumn
        {
            Name = "SelectedOptionColumn",
            DataPropertyName = nameof(AttemptReviewDetailItem.SelectedOption),
            HeaderText = "SV chọn",
            FillWeight = 55,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(6, 8, 6, 8) }
        };
        var correctCol = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AttemptReviewDetailItem.CorrectOption),
            HeaderText = "Đáp án đúng",
            FillWeight = 62,
            DefaultCellStyle =
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(6, 8, 6, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            }
        };
        _resultCol = new DataGridViewTextBoxColumn
        {
            Name = "ResultStatusColumn",
            DataPropertyName = nameof(AttemptReviewDetailItem.ResultStatus),
            HeaderText = "Kết quả",
            FillWeight = 68,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Padding = new Padding(6, 8, 6, 8) }
        };

        _dgv.Columns.AddRange(orderCol, contentCol, optionsCol, _selectedCol, correctCol, _resultCol);
        foreach (DataGridViewColumn col in _dgv.Columns)
        {
            // Tắt mũi tên sort mặc định để chữ tiêu đề không bị lệch sang trái, rồi căn giữa tuyệt đối.
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        _dgv.CellFormatting += OnGridCellFormatting;

        _lblEmpty.Dock = DockStyle.Fill;
        _lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
        _lblEmpty.ForeColor = UiStyle.TextMuted;
        _lblEmpty.Visible = false;

        host.Controls.Add(_lblEmpty);
        host.Controls.Add(_dgv);
        borderHost.Controls.Add(host);
        return borderHost;
    }

    private void OnGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.CellStyle is null)
            return;

        if (e.ColumnIndex == _selectedCol.Index)
        {
            string? selected = e.Value as string;
            if (string.IsNullOrWhiteSpace(selected))
            {
                e.Value = "—";
                e.CellStyle.ForeColor = UiStyle.TextMuted;
                e.FormattingApplied = true;
            }
            return;
        }

        if (e.ColumnIndex == _resultCol.Index)
        {
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
        }
    }

    private void ApplyFilter()
    {
        List<AttemptReviewDetailItem> filtered = _cboFilter.SelectedIndex switch
        {
            1 => _allDetails.Where(d => d.ResultStatus == "Sai").ToList(),
            2 => _allDetails.Where(d => d.ResultStatus == "Bỏ trống").ToList(),
            3 => _allDetails.Where(d => d.ResultStatus == "Đúng").ToList(),
            _ => _allDetails
        };

        _dgv.DataSource = filtered;
        _lblFilterCount.Text = $"Hiển thị {filtered.Count} / {_allDetails.Count} câu";

        bool isEmpty = filtered.Count == 0;
        _lblEmpty.Text = _allDetails.Count == 0
            ? "Không tìm thấy chi tiết câu trả lời cho lượt thi này."
            : "Không có câu hỏi nào thuộc nhóm trạng thái đã chọn.";
        _lblEmpty.Visible = isEmpty;
        _dgv.Visible = !isEmpty;
    }
}
