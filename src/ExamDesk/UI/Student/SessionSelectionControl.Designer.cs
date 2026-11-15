#nullable enable
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

partial class SessionSelectionControl
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeAppearance()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = StudentLayout.GetLabelFont(StudentLayout.BodyFontSize, FontStyle.Regular);
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;
        Dock = DockStyle.Fill;
        AutoScroll = false;
        DoubleBuffered = true;
        AccessibleName = "Chọn ca thi";
    }

}
