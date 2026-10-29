namespace ExamDesk.Models;

public enum UserRole { Lecturer, Student, Examiner }

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
}
