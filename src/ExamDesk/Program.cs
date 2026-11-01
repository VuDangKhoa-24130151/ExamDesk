using ExamDesk.UI;

namespace ExamDesk;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        // Bạn 1 điều phối. Mở khung tích hợp UI; chưa tạo database hoặc gọi nghiệp vụ.
        Application.Run(new MainForm());
    }
}
