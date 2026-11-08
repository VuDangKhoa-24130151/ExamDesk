using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Administration;

// HELP-01 • Bạn 1 phụ trách: Màn hình trợ giúp offline theo tác vụ (H10).
public partial class HelpControl : UserControl, IUnsavedChangesGuard
{
    private static readonly Font GroupNodeFont = new("Segoe UI", 10F, FontStyle.Bold);

    private bool _suppressSearchEvents;
    private bool _suppressTreeEvents;

    public bool HasUnsavedChanges => false;

    public bool ConfirmLeave() => true;

    public HelpControl()
    {
        InitializeComponent();
        PopulateTopicTree();
    }

    public void ShowTopic(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            PopulateTopicTree();
            return;
        }

        if (!string.IsNullOrWhiteSpace(_txtSearch.Text))
        {
            _suppressSearchEvents = true;
            _txtSearch.Clear();
            _suppressSearchEvents = false;
        }

        PopulateTopicTree(preferredTopicId: id.Trim());
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.F))
        {
            _txtSearch.Focus();
            _txtSearch.SelectAll();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void OnSearchTextChanged(object? sender, EventArgs e)
    {
        if (_suppressSearchEvents)
        {
            return;
        }

        var currentTopicId = GetSelectedTopic()?.Id;
        PopulateTopicTree(preferredTopicId: currentTopicId);
    }

    private void OnClearSearchClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_txtSearch.Text))
        {
            return;
        }

        var currentTopicId = GetSelectedTopic()?.Id;
        _suppressSearchEvents = true;
        _txtSearch.Clear();
        _suppressSearchEvents = false;

        PopulateTopicTree(preferredTopicId: currentTopicId);
    }

    private void PopulateTopicTree(string? preferredTopicId = null)
    {
        var normalizedQuery = HelpContent.NormalizeForSearch(_txtSearch.Text);
        var allTopics = HelpContent.Topics;

        IReadOnlyList<HelpTopic> filtered;
        if (string.IsNullOrEmpty(normalizedQuery))
        {
            filtered = allTopics;
        }
        else
        {
            filtered = allTopics
                .Where(t =>
                    HelpContent.NormalizeForSearch(t.Title).Contains(normalizedQuery, StringComparison.Ordinal) ||
                    HelpContent.NormalizeForSearch(t.Keywords).Contains(normalizedQuery, StringComparison.Ordinal) ||
                    HelpContent.NormalizeForSearch(t.Group).Contains(normalizedQuery, StringComparison.Ordinal))
                .ToList();
        }

        _lblTopicCount.Text = $"Hiển thị {filtered.Count} / {allTopics.Count} chủ đề";

        _suppressTreeEvents = true;
        _tvTopics.BeginUpdate();
        _tvTopics.Nodes.Clear();

        TreeNode? firstTopicNode = null;
        TreeNode? matchedTopicNode = null;

        foreach (var group in filtered.GroupBy(t => t.Group))
        {
            var groupNode = new TreeNode(group.Key)
            {
                NodeFont = GroupNodeFont,
                ForeColor = UiStyle.Primary
            };

            foreach (var topic in group)
            {
                var topicNode = new TreeNode(topic.Title)
                {
                    Tag = topic,
                    ForeColor = UiStyle.Text
                };

                groupNode.Nodes.Add(topicNode);
                firstTopicNode ??= topicNode;

                if (!string.IsNullOrWhiteSpace(preferredTopicId) &&
                    string.Equals(topic.Id, preferredTopicId, StringComparison.OrdinalIgnoreCase))
                {
                    matchedTopicNode = topicNode;
                }
            }

            _tvTopics.Nodes.Add(groupNode);
        }

        _tvTopics.ExpandAll();
        _tvTopics.EndUpdate();

        var isEmpty = filtered.Count == 0;
        _lblTreeEmptyState.Visible = isEmpty;
        _tvTopics.Visible = !isEmpty;

        _lblRightEmptyState.Visible = isEmpty;
        _topicScrollPanel.Visible = !isEmpty;

        var nodeToSelect = matchedTopicNode ?? firstTopicNode;
        if (nodeToSelect is not null)
        {
            _tvTopics.SelectedNode = nodeToSelect;
            nodeToSelect.EnsureVisible();
        }

        _suppressTreeEvents = false;

        if (nodeToSelect?.Tag is HelpTopic selectedTopic)
        {
            DisplayTopic(selectedTopic);
        }
    }

    private HelpTopic? GetSelectedTopic()
    {
        var node = _tvTopics.SelectedNode;
        if (node is null)
        {
            return null;
        }

        if (node.Tag is HelpTopic topic)
        {
            return topic;
        }

        if (node.Nodes.Count > 0 && node.Nodes[0].Tag is HelpTopic firstChildTopic)
        {
            return firstChildTopic;
        }

        return null;
    }

    private void OnTreeTopicAfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (_suppressTreeEvents || e.Node is null)
        {
            return;
        }

        if (e.Node.Tag is HelpTopic topic)
        {
            DisplayTopic(topic);
        }
        else if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag is HelpTopic firstChildTopic)
        {
            DisplayTopic(firstChildTopic);
        }
    }

    private void DisplayTopic(HelpTopic topic)
    {
        _lblTopicGroup.Text = $"NHÓM: {topic.Group.ToUpperInvariant()}";
        _lblTopicTitle.Text = topic.Title;
        _lblTopicKeywords.Text = $"Từ khóa: {topic.Keywords}";

        _stepsFlow.SuspendLayout();
        while (_stepsFlow.Controls.Count > 0)
        {
            var old = _stepsFlow.Controls[0];
            _stepsFlow.Controls.RemoveAt(0);
            old.Dispose();
        }

        for (var i = 0; i < topic.Steps.Count; i++)
        {
            var rawStep = topic.Steps[i];
            var stepText = rawStep.StartsWith("Bước ", StringComparison.OrdinalIgnoreCase) ||
                           rawStep.StartsWith("•", StringComparison.Ordinal)
                ? rawStep
                : $"{i + 1}. {rawStep}";

            var stepCard = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = UiStyle.Background,
                Padding = new Padding(12, 10, 12, 10),
                Margin = new Padding(0, 0, 0, 8)
            };

            var lblStep = new Label
            {
                AutoSize = true,
                Font = UiStyle.BodyFont,
                ForeColor = UiStyle.Text,
                BackColor = UiStyle.Background,
                Margin = new Padding(0),
                Text = stepText
            };

            stepCard.Controls.Add(lblStep);
            _stepsFlow.Controls.Add(stepCard);
        }

        _stepsFlow.ResumeLayout();

        var hasNote = !string.IsNullOrWhiteSpace(topic.Note);
        _lblNoteContent.Text = topic.Note;
        _noteBox.Visible = hasNote;

        AdjustContentWidths();
        _topicScrollPanel.AutoScrollPosition = new Point(0, 0);
    }

    private void OnTopicScrollPanelSizeChanged(object? sender, EventArgs e)
    {
        AdjustContentWidths();
    }

    private void AdjustContentWidths()
    {
        var availableWidth = Math.Max(
            260,
            _topicScrollPanel.ClientSize.Width - _topicScrollPanel.Padding.Horizontal - 8);

        _topicContentFlow.MaximumSize = new Size(availableWidth, 0);
        _lblTopicTitle.MaximumSize = new Size(availableWidth, 0);
        _lblTopicKeywords.MaximumSize = new Size(availableWidth, 0);
        _topicDivider.Width = availableWidth;

        foreach (Control control in _stepsFlow.Controls)
        {
            if (control is Panel stepCard && stepCard.Controls.Count > 0 && stepCard.Controls[0] is Label lblStep)
            {
                stepCard.MinimumSize = new Size(availableWidth, 0);
                stepCard.MaximumSize = new Size(availableWidth, 0);
                var stepInnerWidth = Math.Max(200, availableWidth - stepCard.Padding.Horizontal);
                lblStep.MaximumSize = new Size(stepInnerWidth, 0);
            }
        }

        _noteBox.MinimumSize = new Size(availableWidth, 0);
        _noteBox.MaximumSize = new Size(availableWidth, 0);
        var noteInnerWidth = Math.Max(200, availableWidth - _noteBox.Padding.Horizontal);
        _lblNoteContent.MaximumSize = new Size(noteInnerWidth, 0);
    }
}
