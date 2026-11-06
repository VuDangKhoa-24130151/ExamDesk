namespace ExamDesk.UI.ExamManagement;

// Dữ liệu tạm cho mốc UI (Bạn 2). Khi nối nghiệp vụ thay bằng ExamPaperService.
internal static class ExamPreviewData
{
    public const string StatusDraft = "Nháp";
    public const string StatusPublished = "Đã phát hành";

    public static List<string> Subjects() => new()
    {
        // TODO: 3 tên môn
    };

    // Trả về danh sách MỚI mỗi lần gọi để control tự giữ bản của mình
    public static List<ExamPaperItem> Papers() => new()
    {
        new ExamPaperItem
        {
            MaDe = "DE01",
            TenDe = "Giữa kỳ Cơ sở dữ liệu",
            Mon = "Cơ sở dữ liệu",
            SoPhut = 45,
            SoCauDe = 10, SoCauTB = 20, SoCauKho = 10,
            TrangThai = StatusDraft
        },
        // TODO: thêm 3–4 đề nữa, có cả Nháp lẫn Đã phát hành, nhiều môn khác nhau
    };

    // Số câu có trong ngân hàng của một môn: (dễ, trung bình, khó)
    public static (int De, int TB, int Kho) AvailableQuestions(string mon) => mon switch
    {
        "Cơ sở dữ liệu" => (30, 40, 20),
        // TODO: thêm cho 2 môn còn lại
        _ => (0, 0, 0)
    };
}

internal sealed class ExamPaperItem
{
    public string MaDe { get; set; } = string.Empty;
    public string TenDe { get; set; } = string.Empty;
    public string Mon { get; set; } = string.Empty;
    public int SoPhut { get; set; }
    public int SoCauDe { get; set; }
    public int SoCauTB { get; set; }
    public int SoCauKho { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int TongSoCau => SoCauDe + SoCauTB + SoCauKho;
}
