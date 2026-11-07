namespace ExamDesk.UI.Reports;

// dữ liệu tạm cho UI
internal static class ReportsPreviewData
{
    public const string ManualSubmit = "Nộp bài";
    public const string AutoSubmit = "Hết giờ";
    public const string CA01 = "CA01 - Cơ sở dữ liệu - 07:00 06/11";
    public const string CA02 = "CA02 - Lập trình .NET - 09:35 06/11";
    public static List<string> Session() => new() { CA01, CA02 };

    
}

