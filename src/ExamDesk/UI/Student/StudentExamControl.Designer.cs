#nullable enable
using ExamDesk.UI.Shared;
namespace ExamDesk.UI.Student;
partial class StudentExamControl
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private Label _question = null!, _questionNumber = null!, _clock = null!, _progress = null!,
        _saveStatus = null!;
    private Panel _questionViewport = null!, _gridViewport = null!, _progressTrack = null!;
    private readonly RadioButton[] _options = new RadioButton[4];
    private Button[] _numbers = [];
    private Action? _fitContent;
    private CheckBox _flag = null!;
    private PictureBox _image = null!;
    private Button _previous = null!, _next = null!, _clearChoice = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions=new SizeF(7,15);
        AutoScaleMode=AutoScaleMode.Font;
        Font=UiStyle.BodyFont;
        BackColor=UiStyle.Background;
        ForeColor=UiStyle.Text;
        Padding=new Padding(24);
        var layout=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=2,RowCount=2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        var header = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        header.Margin = new Padding(0, 0, 16, 12);
        var section = StudentLayout.Label(_attempt.Submitted ? "XEM LẠI BÀI ĐÃ NỘP" : "BÀI THI TRẮC NGHIỆM", 9, FontStyle.Bold);
        section.ForeColor = UiStyle.Primary;
        section.Margin = new Padding(0, 0, 0, 4);
        header.Controls.Add(section);
        var title = StudentLayout.Label(_attempt.Title, 18, FontStyle.Bold);
        title.Margin = new Padding(0, 0, 0, 4);
        header.Controls.Add(title);
        var draft = _attempt.Draft;
        var identity = StudentLayout.Label(draft is null ? $"{_attempt.Answers.Length} câu" :
            $"{draft.StudentName} · {draft.StudentCode} · {draft.ClassName} · {draft.Questions.Count} câu", 9);
        identity.ForeColor = UiStyle.TextMuted;
        identity.Margin = new Padding(0, 0, 0, 8);
        header.Controls.Add(identity);
        _clock = new Label { Dock = DockStyle.Top, Height = 42, Font = StudentLayout.GetLabelFont(12, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter, ForeColor = UiStyle.Primary, BackColor = UiStyle.Background };
        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(_clock, 1, 0);

        var scroll = _questionViewport = new Panel
        {
            Name = "QuestionViewport", Dock = DockStyle.Fill, AutoScroll = true, Margin = new Padding(0, 0, 16, 0)
        };
        var body = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = Padding.Empty
        };
        var questionCard = ExamCard();
        _questionNumber = StudentLayout.Label("", 12, FontStyle.Bold);
        _questionNumber.ForeColor = UiStyle.Primary;
        _saveStatus = StudentLayout.Label("", 9);
        _questionNumber.Margin = _saveStatus.Margin = new Padding(0, 0, 0, 4);
        _saveStatus.Padding = new Padding(8, 4, 8, 4);
        var questionHeader = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Margin = Padding.Empty };
        questionHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        questionHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        questionHeader.Controls.Add(_questionNumber, 0, 0);
        questionHeader.Controls.Add(_saveStatus, 1, 0);
        questionCard.Controls.Add(questionHeader);
        var hint = StudentLayout.Label(_attempt.Submitted ? "Lựa chọn đã lưu khi nộp bài" : "Chọn một đáp án phù hợp nhất", 9);
        hint.ForeColor = UiStyle.TextMuted;
        hint.Margin = new Padding(0, 0, 0, 4);
        questionCard.Controls.Add(hint);
        _question = StudentLayout.Label("", 12, FontStyle.Bold);
        _question.Margin = new Padding(0, 4, 0, 12);
        questionCard.Controls.Add(_question);
        _image = new PictureBox { SizeMode=PictureBoxSizeMode.Zoom, Visible=false, Margin=new Padding(0,0,0,12) };
        questionCard.Controls.Add(_image);
        string[] answers=["A. int","B. bool","C. string","D. double"];
        for(int i=0;i<4;i++)
        {
            int choice=i;
            var option = new RadioButton { Text=answers[i], AutoSize=false, Margin=new Padding(0,3,0,3),
                Padding = new Padding(10, 0, 8, 0), UseMnemonic = false, UseVisualStyleBackColor = false,
                AutoCheck = !_attempt.Submitted, TabStop = !_attempt.Submitted,
                TabIndex = i, AccessibleName=$"Đáp án {(char)('A'+i)}" };
            option.Paint += (_, e) =>
            {
                using var border = new Pen(option.Checked || option.Focused ? UiStyle.Primary : UiStyle.Border);
                e.Graphics.DrawRectangle(border, 0, 0, option.ClientSize.Width - 1, option.ClientSize.Height - 1);
            };
            option.Enter += (_, _) => option.Invalidate();
            option.Leave += (_, _) => option.Invalidate();
            option.CheckedChanged += (_, _) => Choose(choice);
            _options[i] = option;
            questionCard.Controls.Add(option);
        }
        body.Controls.Add(questionCard);
        var actions=new FlowLayoutPanel { AutoSize=true,WrapContents=true,Margin=new Padding(0,6,0,0) };
        _previous=StudentLayout.Button("← Câu &trước",(_,_)=>GoToQuestion(_attempt.Current-1));
        _next=StudentLayout.Button("Câu t&iếp →",(_,_)=>GoToQuestion(_attempt.Current+1));
        actions.Controls.Add(_previous);actions.Controls.Add(_next);
        _flag = new CheckBox { Appearance = Appearance.Button, Text = "⚑ Đánh dấu &xem lại", AutoSize = true,
            FlatStyle = FlatStyle.Flat, Padding = new Padding(12, 4, 12, 4),
            BackColor = UiStyle.Surface, Margin = new Padding(0, 8, 12, 8) };
        _flag.FlatAppearance.BorderColor = UiStyle.Border;
        _flag.FlatAppearance.CheckedBackColor = UiStyle.Background;
        _flag.Enabled = !_attempt.Submitted;
        _flag.CheckedChanged += (_, _) => ChangeFlag();
        actions.Controls.Add(_flag);
        _clearChoice = StudentLayout.Button("&Bỏ lựa chọn", (_, _) => ChangeChoice(null));
        actions.Controls.Add(_clearChoice);
        body.Controls.Add(actions);
        var notes = ExamCard();
        notes.Margin = new Padding(0, 12, 0, 0);
        var advice = StudentLayout.Label(_attempt.Submitted
            ? "Chọn số câu hoặc dùng Câu trước và Câu tiếp để xem lại bài."
            : "Dùng phím mũi tên trong nhóm đáp án để đổi lựa chọn. Bạn vẫn có thể xem lại bài trước khi xác nhận nộp.", 9);
        advice.ForeColor = UiStyle.TextMuted;
        advice.Margin = Padding.Empty;
        notes.Controls.Add(advice);
        body.Controls.Add(notes);
        scroll.Controls.Add(body);layout.Controls.Add(scroll,0,1);

        var right=new TableLayoutPanel { Dock=DockStyle.Fill,ColumnCount=1,RowCount=6,BackColor=UiStyle.Surface,Padding=new Padding(14) };
        right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.Percent,100));right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var progressTitle = StudentLayout.Label("Tiến độ làm bài", 12, FontStyle.Bold);
        progressTitle.Margin = new Padding(0, 4, 0, 4);
        right.Controls.Add(progressTitle, 0, 0);
        _progress = StudentLayout.Label("");
        _progress.ForeColor = UiStyle.TextMuted;
        _progress.Margin = new Padding(0, 0, 0, 6);
        right.Controls.Add(_progress, 0, 1);
        _progressTrack = new Panel { Dock = DockStyle.Fill, Height = 6, BackColor = UiStyle.Background, Margin = new Padding(0, 0, 0, 10) };
        _progressTrack.Paint += (_, e) =>
        {
            using var fill = new SolidBrush(UiStyle.Primary);
            float width = _progressTrack.ClientSize.Width * _attempt.Answered / (float)_attempt.Answers.Length;
            e.Graphics.FillRectangle(fill, 0, 0, width, _progressTrack.ClientSize.Height);
        };
        right.Controls.Add(_progressTrack, 0, 2);
        var gridScroll = _gridViewport = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        BuildQuestionGrid();
        right.Controls.Add(gridScroll,0,3);
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var legend = StudentLayout.Label("□ Chưa trả lời   ✓ Đã trả lời\n▣ Đang xem (viền đậm)   ⚑ Đánh dấu", 9);
        legend.ForeColor = UiStyle.TextMuted;
        right.Controls.Add(legend, 0, 4);
        var submit = _attempt.Submitted
            ? StudentLayout.Button("← Về trạng thái nộp", (_, _) => ExitReviewRequested?.Invoke(this, EventArgs.Empty), true)
            : StudentLayout.Button("&Nộp bài", (_, _) => SubmitRequested?.Invoke(this, EventArgs.Empty), true);
        submit.Dock = DockStyle.Fill;
        submit.Margin = new Padding(0, 4, 0, 0);
        right.Controls.Add(submit,0,5);layout.Controls.Add(right,1,1);
        Controls.Add(layout);

        bool fitting=false;
        void FitContent()
        {
            if(fitting) return;
            fitting=true;
            try
            {
                int width = Math.Max(80, scroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
                int innerWidth = Math.Max(40, width - questionCard.Padding.Horizontal - 2);
                _question.MaximumSize = new Size(innerWidth, 0);
                identity.MaximumSize = new Size(Math.Max(80, scroll.ClientSize.Width), 0);
                title.MaximumSize = identity.MaximumSize;
                body.SuspendLayout();
                try
                {
                    questionCard.MinimumSize = new Size(width, 0);
                    notes.MinimumSize = new Size(width, 0);
                    questionHeader.MinimumSize = new Size(innerWidth, 0);
                    questionHeader.MaximumSize = new Size(innerWidth, 0);
                    _saveStatus.MaximumSize = new Size(Math.Max(1, innerWidth * 2 / 3), 0);
                    _questionNumber.MaximumSize = new Size(Math.Max(1, innerWidth - _saveStatus.PreferredWidth - 12), 0);
                    advice.MaximumSize = new Size(innerWidth, 0);
                    foreach (var option in _options)
                    {
                        int textWidth = Math.Max(1, innerWidth - (int)(40 * DeviceDpi / 96f));
                        var measure = TextRenderer.MeasureText(option.Text, option.Font, new Size(textWidth, 100000), TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                        int height = Math.Max((int)(32 * DeviceDpi / 96f), measure.Height + (int)(12 * DeviceDpi / 96f));
                        option.Size = new Size(innerWidth, height);
                    }
                    actions.MaximumSize = new Size(width, 0);
                }
                finally { body.ResumeLayout(true); }
                _image.Width = Math.Min(innerWidth, StudentLayout.Scale(this, 480));
                _image.Height = Math.Min(StudentLayout.Scale(this, 220), _image.Width * 220 / 480);
                foreach (var number in _numbers) number.MinimumSize = new Size(0, StudentLayout.Scale(this, 34));
                int sideWidth=Math.Max(80,right.ClientSize.Width-right.Padding.Horizontal-12);
                _progress.MaximumSize = legend.MaximumSize = new Size(sideWidth, 0);
            }
            finally { fitting=false; }
        }
        void FitColumns()
        {
            if (layout.ClientSize.Width <= 0) return;
            var sideWidth = Math.Min(StudentLayout.Scale(this, 300), (int)(layout.ClientSize.Width * 0.42f));
            if (layout.ColumnStyles[1].Width != sideWidth)
                layout.ColumnStyles[1].Width = sideWidth;
            FitContent();
        }

        _fitContent = FitContent;
        layout.SizeChanged += (_, _) => FitColumns();
        scroll.SizeChanged += (_, _) => FitContent();
        right.SizeChanged += (_, _) => FitContent();
        Load += (_, _) => FitColumns();
        DpiChangedAfterParent += (_, _) => FitColumns();
        if (!_attempt.Submitted)
        {
            _timer.Tick += Tick;
            _timer.Start();
        }
        ResumeLayout(true);
    }

    private static FlowLayoutPanel ExamCard()
    {
        var card = new FlowLayoutPanel
        {
            AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(16), Margin = Padding.Empty, BackColor = UiStyle.Surface
        };
        card.Paint += (_, e) =>
        {
            using var border = new Pen(UiStyle.Border);
            e.Graphics.DrawRectangle(border, 0, 0, card.ClientSize.Width - 1, card.ClientSize.Height - 1);
        };
        return card;
    }
    private void BuildQuestionGrid()
    {
        while (_gridViewport.Controls.Count > 0) _gridViewport.Controls[0].Dispose();
        var grid=new TableLayoutPanel { Dock=DockStyle.Top,AutoSize=true,ColumnCount=5,RowCount=(_attempt.Answers.Length+4)/5 };
        for(int i=0;i<5;i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,20));
        for(int row=0;row<grid.RowCount;row++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _numbers=new Button[_attempt.Answers.Length];
        for(int i=0;i<_numbers.Length;i++)
        {
            int index=i;
            _numbers[i]=new Button { Dock=DockStyle.Fill,FlatStyle=FlatStyle.Flat,Padding=Padding.Empty,Margin=new Padding(2),AccessibleName=$"Câu {i+1}" };
            var number = _numbers[i];
            number.Paint += (_, e) =>
            {
                var bounds = Rectangle.Inflate(number.ClientRectangle, -3, -2);
                if (_attempt.Answers[index].HasValue)
                    TextRenderer.DrawText(e.Graphics, "✓", UiStyle.SmallFont, bounds, UiStyle.SuccessText,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
                if (_attempt.Flags[index])
                    TextRenderer.DrawText(e.Graphics, "⚑", UiStyle.SmallFont, bounds, UiStyle.WarningText,
                        TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.NoPadding);
            };
            _numbers[i].Click+=(_,_)=>GoToQuestion(index);grid.Controls.Add(_numbers[i],i%5,i/5);
        }
        _gridViewport.Controls.Add(grid);
    }
}
