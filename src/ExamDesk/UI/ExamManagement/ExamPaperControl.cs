using ExamDesk.Models;
using ExamDesk.Services;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.ExamManagement;

public partial class ExamPaperControl : UserControl
{
    private readonly ExamPaperService _examPaperService;
    private readonly QuestionBankService _questionBankService;

    private bool _isLoadingFilters;
    private List<ExamPaperRow> _currentRows = [];

    public ExamPaperControl()
    {
        _examPaperService = new ExamPaperService();
        _questionBankService = new QuestionBankService();

        InitializeComponent();
        ApplyUiStyle();
        ConfigureGrid();
        WireEvents();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshAllData();
    }

    private void ApplyUiStyle()
    {
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Font = UiStyle.BodyFont;

        _lblTitle.Font = UiStyle.TitleFont;
        _lblTitle.ForeColor = UiStyle.Text;

        _lblSubjectFilter.Font = UiStyle.BodyFont;
        _lblSubjectFilter.ForeColor = UiStyle.Text;
        _lblSearch.Font = UiStyle.BodyFont;
        _lblSearch.ForeColor = UiStyle.Text;

        _lblResultCount.Font = UiStyle.SmallFont;
        _lblResultCount.ForeColor = UiStyle.TextMuted;

        _emptyStatePanel.BackColor = UiStyle.Surface;
        _emptyStatePanel.BorderStyle = BorderStyle.FixedSingle;
        _lblEmptyState.Font = UiStyle.BodyFont;
        _lblEmptyState.ForeColor = UiStyle.TextMuted;

        UiStyle.StyleGrid(_dgvPapers);
        UiStyle.StyleSecondaryButton(_btnClearFilter);
        UiStyle.StylePrimaryButton(_btnCreate);
        UiStyle.StyleSecondaryButton(_btnPreview);
        UiStyle.StyleSecondaryButton(_btnRegenerate);
        UiStyle.StyleSecondaryButton(_btnDelete);
    }

    private void ConfigureGrid()
    {
        _dgvPapers.Columns.Clear();
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colPaperCode",
            HeaderText = "Mã đề",
            DataPropertyName = nameof(ExamPaperRow.PaperCode),
            FillWeight = 12
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTitle",
            HeaderText = "Tiêu đề",
            DataPropertyName = nameof(ExamPaperRow.Title),
            FillWeight = 28
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSubject",
            HeaderText = "Môn",
            DataPropertyName = nameof(ExamPaperRow.SubjectDisplay),
            FillWeight = 20
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDuration",
            HeaderText = "Thời lượng (phút)",
            DataPropertyName = nameof(ExamPaperRow.DurationMinutes),
            FillWeight = 11,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colStructure",
            HeaderText = "Cơ cấu",
            DataPropertyName = nameof(ExamPaperRow.StructureDisplay),
            FillWeight = 14,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTotal",
            HeaderText = "Tổng câu",
            DataPropertyName = nameof(ExamPaperRow.TotalQuestions),
            FillWeight = 8,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        _dgvPapers.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colStatus",
            HeaderText = "Trạng thái",
            DataPropertyName = nameof(ExamPaperRow.StatusDisplay),
            FillWeight = 16,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
    }

    private void WireEvents()
    {
        VisibleChanged += (_, _) =>
        {
            if (Visible && IsHandleCreated)
            {
                RefreshAllData();
            }
        };

        _cboSubjectFilter.SelectedIndexChanged += (_, _) =>
        {
            if (!_isLoadingFilters)
            {
                LoadPapers();
            }
        };

        _txtSearch.TextChanged += (_, _) => LoadPapers();

        _btnClearFilter.Click += (_, _) =>
        {
            _isLoadingFilters = true;
            try
            {
                if (_cboSubjectFilter.Items.Count > 0)
                {
                    _cboSubjectFilter.SelectedIndex = 0;
                }

                _txtSearch.Clear();
            }
            finally
            {
                _isLoadingFilters = false;
            }

            LoadPapers();
        };

        _dgvPapers.SelectionChanged += (_, _) => UpdateActionButtons();
        _dgvPapers.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
            {
                OpenPreviewDialog();
            }
        };
        _dgvPapers.CellFormatting += OnGridCellFormatting;

        _btnCreate.Click += (_, _) => OpenCreateDialog();
        _btnPreview.Click += (_, _) => OpenPreviewDialog();
        _btnRegenerate.Click += (_, _) => RegenerateSelectedPaper();
        _btnDelete.Click += (_, _) => DeleteSelectedPaper();
    }

    private void RefreshAllData()
    {
        var previousSubjectId = GetSelectedSubjectFilterId();
        LoadSubjectFilter(previousSubjectId);
        LoadPapers();
    }

    private void LoadSubjectFilter(int? selectedSubjectId)
    {
        _isLoadingFilters = true;
        try
        {
            var items = new List<SubjectFilterOption>
            {
                new(0, "Tất cả")
            };

            foreach (var subject in _questionBankService.GetSubjects())
            {
                items.Add(new SubjectFilterOption(subject.Id, $"{subject.Code} - {subject.Name}"));
            }

            _cboSubjectFilter.DisplayMember = nameof(SubjectFilterOption.DisplayName);
            _cboSubjectFilter.ValueMember = nameof(SubjectFilterOption.Id);
            _cboSubjectFilter.DataSource = items;

            if (selectedSubjectId.HasValue && items.Any(x => x.Id == selectedSubjectId.Value))
            {
                _cboSubjectFilter.SelectedValue = selectedSubjectId.Value;
            }
            else
            {
                _cboSubjectFilter.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi tải danh sách môn học",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _isLoadingFilters = false;
        }
    }

    private void LoadPapers(int? selectPaperId = null)
    {
        var targetSelectId = selectPaperId ?? GetSelectedPaper()?.Id;
        var subjectId = GetSelectedSubjectFilterId();
        var keyword = _txtSearch.Text;

        try
        {
            var summaries = _examPaperService.GetPaperSummaries(subjectId, keyword);
            _currentRows = summaries.Select(s => new ExamPaperRow(s)).ToList();

            _dgvPapers.DataSource = null;
            _dgvPapers.DataSource = _currentRows;

            _lblResultCount.Text = $"Hiển thị {_currentRows.Count} đề thi";

            var isEmpty = _currentRows.Count == 0;
            _emptyStatePanel.Visible = isEmpty;
            _dgvPapers.Visible = !isEmpty;

            if (!isEmpty)
            {
                if (targetSelectId.HasValue)
                {
                    SelectRowByPaperId(targetSelectId.Value);
                }
                else if (_dgvPapers.Rows.Count > 0)
                {
                    _dgvPapers.Rows[0].Selected = true;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Lỗi tải danh sách đề thi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        UpdateActionButtons();
    }

    private int? GetSelectedSubjectFilterId()
    {
        if (_cboSubjectFilter.SelectedItem is SubjectFilterOption opt && opt.Id > 0)
        {
            return opt.Id;
        }

        return null;
    }

    private ExamPaperSummary? GetSelectedPaper()
    {
        if (_dgvPapers.CurrentRow?.DataBoundItem is ExamPaperRow row)
        {
            return row.Summary;
        }

        if (_dgvPapers.SelectedRows.Count > 0 && _dgvPapers.SelectedRows[0].DataBoundItem is ExamPaperRow selectedRow)
        {
            return selectedRow.Summary;
        }

        return null;
    }

    private void SelectRowByPaperId(int paperId)
    {
        foreach (DataGridViewRow row in _dgvPapers.Rows)
        {
            if (row.DataBoundItem is ExamPaperRow item && item.Id == paperId)
            {
                row.Selected = true;
                if (row.Cells.Count > 0)
                {
                    _dgvPapers.CurrentCell = row.Cells[0];
                }
                return;
            }
        }

        if (_dgvPapers.Rows.Count > 0)
        {
            _dgvPapers.Rows[0].Selected = true;
        }
    }

    private void UpdateActionButtons()
    {
        var selected = GetSelectedPaper();
        var hasSelection = selected is not null;
        var canModify = selected is not null && !selected.IsLocked;

        _btnPreview.Enabled = hasSelection;
        _btnRegenerate.Enabled = canModify;
        _btnDelete.Enabled = canModify;
    }

    private void OpenCreateDialog()
    {
        var initialSubjectId = GetSelectedSubjectFilterId();
        using var dialog = new ExamPaperEditorDialog(initialSubjectId);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadPapers(dialog.CreatedPaperId);
        }
    }

    private void OpenPreviewDialog()
    {
        var selected = GetSelectedPaper();
        if (selected is null)
        {
            return;
        }

        using var dialog = new ExamPaperPreviewDialog(selected);
        dialog.ShowDialog(this);
        if (dialog.QuestionsRegenerated)
        {
            LoadPapers(selected.Id);
        }
    }

    private void RegenerateSelectedPaper()
    {
        var selected = GetSelectedPaper();
        if (selected is null)
        {
            return;
        }

        if (selected.IsLocked)
        {
            MessageBox.Show(
                this,
                $"Đề thi '{selected.PaperCode}' đã được gán vào ca thi, không thể rút lại câu hỏi.",
                "Đề thi đã khóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc muốn rút lại ngẫu nhiên bộ câu hỏi cho đề '{selected.PaperCode}'?\nDanh sách câu hỏi cũ của đề sẽ bị thay thế.",
            "Xác nhận rút lại câu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _examPaperService.RegenerateQuestions(selected.Id);
            LoadPapers(selected.Id);

            MessageBox.Show(
                this,
                $"Đã rút lại bộ câu hỏi mới cho đề '{selected.PaperCode}'.",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể rút lại câu hỏi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void DeleteSelectedPaper()
    {
        var selected = GetSelectedPaper();
        if (selected is null)
        {
            return;
        }

        if (selected.IsLocked)
        {
            MessageBox.Show(
                this,
                $"Đề thi '{selected.PaperCode}' đã được gán vào ca thi, không thể xóa.",
                "Đề thi đã khóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc muốn xóa đề thi '{selected.PaperCode} - {selected.Title}'?\nThao tác này không thể hoàn tác.",
            "Xác nhận xóa đề thi",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _examPaperService.DeletePaper(selected.Id);
            LoadPapers();

            MessageBox.Show(
                this,
                $"Đã xóa đề thi '{selected.PaperCode}'.",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Không thể xóa đề thi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void OnGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _dgvPapers.Rows.Count)
        {
            return;
        }

        if (_dgvPapers.Columns[e.ColumnIndex].Name != "colStatus")
        {
            return;
        }

        if (_dgvPapers.Rows[e.RowIndex].DataBoundItem is not ExamPaperRow row || e.CellStyle is null)
        {
            return;
        }

        if (row.IsLocked)
        {
            e.CellStyle.BackColor = UiStyle.WarningBack;
            e.CellStyle.ForeColor = UiStyle.WarningText;
        }
        else
        {
            e.CellStyle.BackColor = UiStyle.SuccessBack;
            e.CellStyle.ForeColor = UiStyle.SuccessText;
        }
    }

    private sealed record SubjectFilterOption(int Id, string DisplayName);

    private sealed class ExamPaperRow(ExamPaperSummary summary)
    {
        public ExamPaperSummary Summary { get; } = summary;
        public int Id => Summary.Id;
        public string PaperCode => Summary.PaperCode;
        public string Title => Summary.Title;
        public string SubjectDisplay => $"{Summary.SubjectCode} - {Summary.SubjectName}";
        public int DurationMinutes => Summary.DurationMinutes;
        public string StructureDisplay => $"{Summary.EasyCount}D / {Summary.MediumCount}TB / {Summary.HardCount}K";
        public int TotalQuestions => Summary.TotalQuestions;
        public bool IsLocked => Summary.IsLocked;
        public string StatusDisplay => Summary.IsLocked ? "🔒 Đã gán ca (Khóa)" : "Chưa gán ca";
    }
}
