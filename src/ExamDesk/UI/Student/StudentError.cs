using System.Diagnostics;
using ExamDesk.Services;

namespace ExamDesk.UI.Student;

internal static class StudentError
{
    internal static string DescribeStart(Exception error, string fallback)
    {
        // Kho lỗi cũ dùng chung InvalidOperationException, chỉ cho qua thông báo đã biết.
        if (error.GetType() == typeof(InvalidOperationException) &&
            error.Message is "Ca đã đóng hoặc không có đề thi." or "Đề chưa có đủ câu hỏi." or "Bạn đã nộp bài ca này.")
            return error.Message;

        return Describe(error, fallback);
    }

    internal static string Describe(Exception error, string fallback)
    {
        Trace.WriteLine(error);
        return error is StudentExamInputException input ? input.Message : fallback;
    }
}
