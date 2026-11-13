#nullable enable
namespace ExamDesk.UI.ExamManagement;

partial class ExamPaperPreviewDialog
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel _rootLayout = null!;
    private Panel _headerCard = null!;
    private TableLayoutPanel _headerTable = null!;
    private Label _lblPaperTitle = null!;
    private Label _lblMetaInfo = null!;
    private Label _lblStatusBadge = null!;
    private CheckBox _chkShowAnswerKey = null!;
    private FlowLayoutPanel _questionsPanel = null!;
    private FlowLayoutPanel _footerPanel = null!;
    private Button _btnRegenerate = null!;
    private Button _btnClose = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerCard = new Panel();
        _headerTable = new TableLayoutPanel();
        _lblPaperTitle = new Label();
        _lblMetaInfo = new Label();
        _lblStatusBadge = new Label();
        _chkShowAnswerKey = new CheckBox();
        _questionsPanel = new FlowLayoutPanel();
        _footerPanel = new FlowLayoutPanel();
        _btnRegenerate = new Button();
        _btnClose = new Button();

        SuspendLayout();

        // Root layout
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.ColumnCount = 1;
        _rootLayout.RowCount = 3;
        _rootLayout.Padding = new Padding(24);
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Header card
        _headerCard.Dock = DockStyle.Top;
        _headerCard.AutoSize = true;
        _headerCard.Padding = new Padding(16);
        _headerCard.Margin = new Padding(0, 0, 0, 14);

        _headerTable.Dock = DockStyle.Fill;
        _headerTable.AutoSize = true;
        _headerTable.ColumnCount = 2;
        _headerTable.RowCount = 3;
        _headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _lblPaperTitle.AutoSize = true;
        _lblPaperTitle.Anchor = AnchorStyles.Left;
        _lblPaperTitle.Margin = new Padding(0, 0, 12, 6);

        _lblStatusBadge.AutoSize = true;
        _lblStatusBadge.Anchor = AnchorStyles.Right;
        _lblStatusBadge.Padding = new Padding(10, 4, 10, 4);
        _lblStatusBadge.Margin = new Padding(0, 0, 0, 6);

        _lblMetaInfo.AutoSize = true;
        _lblMetaInfo.Anchor = AnchorStyles.Left;
        _lblMetaInfo.Margin = new Padding(0, 0, 0, 8);
        _headerTable.SetColumnSpan(_lblMetaInfo, 2);

        _chkShowAnswerKey.AutoSize = true;
        _chkShowAnswerKey.Text = "Hiện đáp án đúng";
        _chkShowAnswerKey.Checked = false;
        _chkShowAnswerKey.Cursor = Cursors.Hand;
        _chkShowAnswerKey.Margin = new Padding(0);
        _headerTable.SetColumnSpan(_chkShowAnswerKey, 2);

        _headerTable.Controls.Add(_lblPaperTitle, 0, 0);
        _headerTable.Controls.Add(_lblStatusBadge, 1, 0);
        _headerTable.Controls.Add(_lblMetaInfo, 0, 1);
        _headerTable.Controls.Add(_chkShowAnswerKey, 0, 2);

        _headerCard.Controls.Add(_headerTable);

        // Questions scrollable list
        _questionsPanel.Dock = DockStyle.Fill;
        _questionsPanel.AutoScroll = true;
        _questionsPanel.FlowDirection = FlowDirection.TopDown;
        _questionsPanel.WrapContents = false;
        _questionsPanel.Padding = new Padding(0, 0, 6, 0);
        _questionsPanel.Margin = new Padding(0);

        // Footer buttons
        _footerPanel.Dock = DockStyle.Fill;
        _footerPanel.AutoSize = true;
        _footerPanel.FlowDirection = FlowDirection.RightToLeft;
        _footerPanel.WrapContents = false;
        _footerPanel.Margin = new Padding(0, 14, 0, 0);

        _btnClose.Text = "Đóng";
        _btnClose.Margin = new Padding(8, 0, 0, 0);

        _btnRegenerate.Text = "Rút lại câu ngẫu nhiên";
        _btnRegenerate.Margin = new Padding(0);

        _footerPanel.Controls.Add(_btnClose);
        _footerPanel.Controls.Add(_btnRegenerate);

        _rootLayout.Controls.Add(_headerCard, 0, 0);
        _rootLayout.Controls.Add(_questionsPanel, 0, 1);
        _rootLayout.Controls.Add(_footerPanel, 0, 2);

        Controls.Add(_rootLayout);

        CancelButton = _btnClose;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(780, 620);
        MinimumSize = new Size(640, 480);
        Name = "ExamPaperPreviewDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Xem trước đề thi";

        ResumeLayout(false);
        PerformLayout();
    }
}
