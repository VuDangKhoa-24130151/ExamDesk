using System.Diagnostics;
using ExamDesk.Services;

namespace ExamDesk.UI.Student;

internal static class StudentError
{
    internal static string Describe(Exception error, string fallback)
    {
        Trace.WriteLine(error);
        return error is StudentExamInputException input ? input.Message : fallback;
    }
}
