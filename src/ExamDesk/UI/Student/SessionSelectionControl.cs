using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public partial class SessionSelectionControl : UserControl
{
    public event EventHandler? InstructionsRequested;
    public event EventHandler? CloseRequested;
    public SessionSelectionControl()
    {
        InitializeComponent();
        Resize += (_, _) => CenterContent();
        CenterContent();
        Load += (_, _) => SelectNextControl(null, true, true, true, false);
    }

    public event Action<ExamDesk.Models.ExamSession, ExamDesk.Models.ExamPaper>? SessionSelected;

    public SessionSelectionControl(ExamDesk.Models.User student) : this()
    {
        while(Controls.Count > 0) Controls[0].Dispose();
        var page = new TableLayoutPanel { Dock=DockStyle.Top,AutoSize=true,ColumnCount=1,Padding=new Padding(24) };
        page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        void Add(Control control) { int row=page.RowCount++;page.RowStyles.Add(new RowStyle(SizeType.AutoSize));page.Controls.Add(control,0,row); }
        Add(TextLabel("Chọn ca thi",18,FontStyle.Bold));
        Add(TextLabel("Chọn ca đang mở để xem hướng dẫn và bắt đầu hoặc khôi phục bài thi."));
        var identity=Surface();
        identity.Controls.Add(TextLabel($"{student.FullName} · {student.StudentCode}",12,FontStyle.Bold));
        identity.Controls.Add(TextLabel($"Lớp {student.ClassName}")); Add(identity);
        var list=new TableLayoutPanel { Dock=DockStyle.Top,AutoSize=true,ColumnCount=2,Margin=new Padding(0,12,0,12) };
        list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
        Add(list);
        var status=TextLabel("");Add(status);
        bool loading=false;
        void FitCards()
        {
            if(loading) return;
            int columns=list.ClientSize.Width < (int)(800*DeviceDpi/96f) ? 1 : 2;
            var cards=list.Controls.Cast<Control>().ToArray();
            if(list.ColumnCount==columns && list.RowCount==(cards.Length+columns-1)/columns) return;
            list.SuspendLayout();
            list.ColumnCount=columns;list.RowCount=(cards.Length+columns-1)/columns;
            list.ColumnStyles.Clear();list.RowStyles.Clear();
            for(int i=0;i<columns;i++) list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100f/columns));
            for(int i=0;i<list.RowCount;i++) list.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for(int i=0;i<cards.Length;i++) list.SetCellPosition(cards[i],new TableLayoutPanelCellPosition(i%columns,i/columns));
            list.ResumeLayout(true);
        }
        list.SizeChanged+=(_,_)=>FitCards();
        void Reload()
        {
            loading=true;
            while(list.Controls.Count>0) list.Controls[0].Dispose();
            list.ColumnCount=2;list.ColumnStyles.Clear();list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));list.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
            list.RowStyles.Clear();list.RowCount=0;
            try
            {
                var service=new ExamDesk.Services.ExamSessionService();
                var sessions = service.GetSessions().Where(session => session.IsOpen).ToList();
                var papers = service.GetExamPapers();
                for(int i=0;i<sessions.Count;i++)
                {
                    var session=sessions[i];var paper=papers.FirstOrDefault(p=>p.Id==session.PaperId);
                    var card=Surface(); card.Margin=new Padding(0,0,12,12);
                    card.Controls.Add(TextLabel(StudentPreviewAttempt.DisplayTitle(session.SessionName), 12, FontStyle.Bold));
                    card.Controls.Add(TextLabel(paper is null ? "Đề thi chưa sẵn sàng" : $"{paper.Title} · {paper.TotalQuestions} câu · {paper.DurationMinutes} phút"));
                    card.Controls.Add(Badge("● Đang mở", UiStyle.SuccessBack, UiStyle.SuccessText));
                    card.Controls.Add(TextLabel($"Mã ca: {session.SessionCode} · Phòng {session.RoomName}"));
                    var enter=StudentLayout.Button("Xem hướng dẫn →",(_,_)=>
                    {
                        try
                        {
                            var selected=new ExamDesk.Services.StudentExamService().GetSelectedPaper(session.Id,student.StudentCode);
                            SessionSelected?.Invoke(session,selected);
                        }
                        catch(Exception ex) { status.Text=ex.Message;status.ForeColor=UiStyle.ErrorText; }
                    },true);
                    enter.Enabled=paper is not null;card.Controls.Add(enter);
                    int row=i/2;if(row>=list.RowCount) {list.RowCount=row+1;list.RowStyles.Add(new RowStyle(SizeType.AutoSize));}
                    list.Controls.Add(card,i%2,row);
                    card.SizeChanged+=(_,_)=>WrapLabels(card);
                }
                loading=false; FitCards();
                status.ForeColor=UiStyle.TextMuted;
                status.Text = sessions.Count == 0 ? "Chưa có ca thi đang mở. Hãy liên hệ giảng viên hoặc tải lại danh sách." : "";
            }
            catch(Exception ex) { status.Text="Không tải được ca thi. "+ex.Message;status.ForeColor=UiStyle.ErrorText; }
            finally { loading=false; }
        }
        var actions=new FlowLayoutPanel { Dock=DockStyle.Top,AutoSize=true,WrapContents=true };
        actions.Controls.Add(StudentLayout.Button("← Quay lại nhập MSSV",(_,_)=>CloseRequested?.Invoke(this,EventArgs.Empty)));
        actions.Controls.Add(StudentLayout.Button("Tải lại ca thi",(_,_)=>Reload()));Add(actions);
        Controls.Add(page);Reload();
        page.SizeChanged+=(_,_)=> { int width=Math.Max(1,page.ClientSize.Width-page.Padding.Horizontal); foreach(Control c in page.Controls) if(c is Label l) l.MaximumSize=new Size(width,0); };
    }
    // Màn này ít thông tin nên giới hạn bề ngang, phần dư chia đều sang hai bên.
    private void CenterContent()
    {
        int maxWidth = (int)(1040 * DeviceDpi / 96f);
        int side = Math.Max(0, (ClientSize.Width - maxWidth) / 2);
        Padding = new Padding(side, 0, side, 0);
    }

    // Giới hạn chiều rộng nhãn để chữ dài xuống dòng, không tràn sang khung bên cạnh.
    private static void WrapLabels(FlowLayoutPanel panel)
    {
        int width = panel.ClientSize.Width - panel.Padding.Horizontal;
        if (width <= 0) return;

        foreach (Control control in panel.Controls)
        {
            if (control is Label label)
                label.MaximumSize = new Size(Math.Max(1, width - label.Margin.Horizontal), 0);
        }
    }

    private void OpenInstructions(object? sender, EventArgs e)
    {
        InstructionsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void GoBack(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}



