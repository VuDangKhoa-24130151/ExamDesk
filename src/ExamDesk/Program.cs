using ExamDesk.UI;

namespace ExamDesk;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        // Khung chỉ mở cửa sổ trống; chưa đăng nhập, tạo database hay nạp dữ liệu.
        Application.Run(new MainForm());
    }
}
