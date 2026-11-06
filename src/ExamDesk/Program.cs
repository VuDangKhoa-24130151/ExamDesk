using ExamDesk.UI;
using ExamDesk.Data;

namespace ExamDesk;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            SqliteContext.InitializeDatabase();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Không thể khởi tạo SQLite.\n\nĐường dẫn: {SqliteContext.DatabaseFilePath}\n\n{ex.Message}",
                "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        Application.Run(new MainForm());
    }
}
