using ExamDesk.Data;
using ExamDesk.Models;

namespace ExamDesk.Repositories;

public sealed class StudentExamRepository
{
    public User? FindStudent(string studentCode)
    {
        using var connection = SqliteContext.CreateConnection(); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, StudentCode, ClassName FROM Users WHERE Role='Student' AND StudentCode=$code LIMIT 2";
        command.Parameters.AddWithValue("$code", studentCode);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            // Ghi chẩn đoán cục bộ để đối chiếu database của tiến trình đang chạy.
            System.Diagnostics.Trace.WriteLine($"Không thấy MSSV '{studentCode}' trong {connection.DataSource}");
            throw new InvalidOperationException($"Không tìm thấy MSSV {studentCode}.\nDatabase đang đọc: {connection.DataSource}");
        }
        var student = new User { Id=reader.GetInt32(0), FullName=reader.GetString(1), StudentCode=reader.GetString(2), ClassName=reader.GetString(3) };
        if (reader.Read()) throw new InvalidOperationException("MSSV bị trùng trong danh mục. Vui lòng liên hệ giảng viên.");
        return student;
    }

    public List<ExamSession> FindOpenSessions() => new ExamSessionRepository().FindSessions().Where(s => s.IsOpen).ToList();

    // Đọc câu hỏi thật sẽ triển khai cùng hợp đồng đề; không trả đáp án chuẩn lên UI.
    public List<Question> FindQuestionsByPaperId(int paperId)
        => throw new NotImplementedException("Chưa nối đề thật vào màn làm bài.");

    public bool HasStudentSubmitted(int sessionId, string studentCode)
    {
        using var connection = SqliteContext.CreateConnection(); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM ExamAttempts WHERE SessionId=$session AND StudentCode=$student)";
        command.Parameters.AddWithValue("$session", sessionId);
        command.Parameters.AddWithValue("$student", studentCode);
        return Convert.ToInt32(command.ExecuteScalar()) != 0;
    }
}
