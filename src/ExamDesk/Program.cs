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
        // Nạp dữ liệu mẫu ngân hàng câu hỏi khi người dùng yêu cầu qua tham số dòng lệnh
        if (args.Contains("--seed-questions") || args.Contains("--seed-question-bank"))
        {
            try
            {
                var seedService = new Services.QuestionSeedService();
                var (subjectsAdded, questionsAdded) = seedService.SeedQuestionBank();
                var msg = questionsAdded > 0
                    ? $"Đã nạp thành công dữ liệu mẫu vào SQLite:\n• {subjectsAdded} môn học mới\n• {questionsAdded} câu hỏi mới\n\nDatabase: {SqliteContext.DatabaseFilePath}"
                    : $"Cơ sở dữ liệu đã có sẵn dữ liệu mẫu (không có bản ghi trùng lặp nào được thêm mới).\n\nDatabase: {SqliteContext.DatabaseFilePath}";

                MessageBox.Show(msg, "Nạp dữ liệu mẫu ngân hàng câu hỏi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp dữ liệu mẫu: {ex.Message}", "Lỗi nạp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
