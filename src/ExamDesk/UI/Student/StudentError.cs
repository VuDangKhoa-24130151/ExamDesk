using System.Diagnostics;
using ExamDesk.Models;
using ExamDesk.Services;

namespace ExamDesk.UI.Student;

internal static class StudentError
{
    internal const string DuplicateStudentCodeMessage = "Mã số sinh viên bị trùng. Hãy hỏi giám thị coi thi.";

    internal static string DescribeStart(Exception error, string fallback)
    {
        if (error is StudentDraftStartException startError)
            return startError.Message;

        return Describe(error, fallback);
    }

    internal static string Describe(Exception error, string fallback)
    {
        Trace.WriteLine(error);
        return error is StudentExamInputException input
            ? input.Message.Replace("MSSV bị trùng trong danh mục. Vui lòng liên hệ giảng viên.", DuplicateStudentCodeMessage)
            : fallback;
    }
}
