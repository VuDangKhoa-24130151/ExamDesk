using ExamDesk.UI;
using ExamDesk.Data;

namespace ExamDesk;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
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
#if DEBUG
        // Chỉ chuẩn bị đề/ca thử khi người phát triển yêu cầu qua tham số chạy.
        if (args.Contains("--prepare-student-test"))
        {
            try
            {
                var session = new Services.StudentTestDataService().PrepareExamSession();
                MessageBox.Show(
                    $"Đã lưu đề TEST_UI30: 30 câu, 45 phút.\nCa mới đang mở: {session.SessionCode}\n\nNhập MSSV 24130151 hoặc 24130152, chọn ca này rồi xem hướng dẫn để bắt đầu.\n\nDatabase: {SqliteContext.DatabaseFilePath}",
                    "Đã chuẩn bị bài thi thử", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Run(new UI.Student.StudentExamForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không chuẩn bị được bài thi thử", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
#endif
        Application.Run(new MainForm());
    }
}
