using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ExamDesk.Models;
using ExamDesk.Repositories;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Services;

// Bạn 1 phụ trách: Dịch vụ nghiệp vụ đăng nhập giảng viên, quản lý lớp và sinh viên.
public sealed class AdministrationService
{
    private static readonly Regex ValidClassCodeRegex = new("^[A-Z0-9]{2,15}$", RegexOptions.Compiled);
    private static readonly Regex SchoolYearRegex = new(@"^(\d{4})-(\d{4})$", RegexOptions.Compiled);
    private static readonly Regex ValidStudentCodeRegex = new(@"^\d{8,10}$", RegexOptions.Compiled);

    private readonly AdministrationRepository _repository;

    public AdministrationService() : this(new AdministrationRepository())
    {
    }

    public AdministrationService(AdministrationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #region Lecturer Authentication

    public static string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }

    public void EnsureDefaultLecturerAccount()
    {
        if (_repository.HasAnyLecturer())
        {
            return;
        }

        var defaultPasswordHash = HashPassword("123456");
        _repository.EnsureDefaultLecturerAccount("gv01", defaultPasswordHash, "Giảng viên quản trị");
    }

    public User? AuthenticateLecturer(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var passwordHash = HashPassword(password);
        return _repository.FindLecturerByCredentials(username.Trim(), passwordHash);
    }

    #endregion

    #region Class Business Logic

    public List<ClassInfo> GetClasses() => _repository.GetClasses();

    public bool IsClassCodeExists(string code, int? excludeClassId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return _repository.IsClassCodeExists(code.Trim(), excludeClassId);
    }

    public int CountStudentsByClass(string classCode)
    {
        if (string.IsNullOrWhiteSpace(classCode))
        {
            return 0;
        }

        return _repository.CountStudentsByClass(classCode.Trim());
    }

    public void ValidateClass(ClassInfo classInfo, string? originalCode = null)
    {
        ArgumentNullException.ThrowIfNull(classInfo);

        var code = (classInfo.Code ?? string.Empty).Trim().ToUpperInvariant();
        var name = (classInfo.Name ?? string.Empty).Trim();
        var schoolYear = (classInfo.SchoolYear ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Vui lòng nhập mã lớp.");
        }

        var isUnchangedExistingCode = !string.IsNullOrWhiteSpace(originalCode) &&
            string.Equals(code, originalCode.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!isUnchangedExistingCode && !ValidClassCodeRegex.IsMatch(code))
        {
            throw new ArgumentException("Mã lớp chỉ gồm chữ in hoa không dấu (A–Z) hoặc số (0–9), độ dài 2–15 ký tự.");
        }

        int? excludeId = classInfo.Id > 0 ? classInfo.Id : null;
        if (_repository.IsClassCodeExists(code, excludeId))
        {
            throw new InvalidOperationException($"Mã lớp \"{code}\" đã tồn tại. Vui lòng nhập mã khác.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Vui lòng nhập tên lớp.");
        }

        if (name.Length > 100)
        {
            throw new ArgumentException($"Tên lớp không được vượt quá 100 ký tự (hiện tại {name.Length} ký tự).");
        }

        if (string.IsNullOrWhiteSpace(schoolYear))
        {
            throw new ArgumentException("Vui lòng nhập niên khóa (ví dụ: 2024-2028).");
        }

        var match = SchoolYearRegex.Match(schoolYear);
        if (!match.Success)
        {
            throw new ArgumentException("Niên khóa phải đúng định dạng YYYY-YYYY (ví dụ: 2024-2028).");
        }

        var startYear = int.Parse(match.Groups[1].Value);
        var endYear = int.Parse(match.Groups[2].Value);
        if (endYear <= startYear)
        {
            throw new ArgumentException($"Năm kết thúc ({endYear}) phải lớn hơn năm bắt đầu ({startYear}).");
        }

        classInfo.Code = code;
        classInfo.Name = name;
        classInfo.SchoolYear = schoolYear;
    }

    public void SaveClass(ClassInfo classInfo, string? oldCode = null)
    {
        ArgumentNullException.ThrowIfNull(classInfo);
        ValidateClass(classInfo, oldCode);

        if (classInfo.Id <= 0)
        {
            _repository.AddClass(classInfo);
        }
        else
        {
            _repository.UpdateClass(classInfo, oldCode);
        }
    }

    public void DeleteClass(int classId)
    {
        _repository.DeleteClass(classId);
    }

    #endregion

    #region Student Business Logic

    public List<User> GetStudentsByClass(string classCode, string? keyword = null)
    {
        if (string.IsNullOrWhiteSpace(classCode))
        {
            return [];
        }

        return _repository.GetStudentsByClass(classCode.Trim(), keyword);
    }

    public User? FindStudentConflict(string studentCode, int? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(studentCode))
        {
            return null;
        }

        return _repository.FindUserConflictByStudentCode(studentCode.Trim(), excludeUserId);
    }

    public void ValidateStudent(User student)
    {
        ArgumentNullException.ThrowIfNull(student);

        var studentCode = (student.StudentCode ?? string.Empty).Trim();
        var fullName = (student.FullName ?? string.Empty).Trim();
        var className = (student.ClassName ?? string.Empty).Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(studentCode))
        {
            throw new ArgumentException("Vui lòng nhập mã số sinh viên (MSSV).");
        }

        if (!ValidStudentCodeRegex.IsMatch(studentCode))
        {
            throw new ArgumentException("MSSV chỉ gồm chữ số (0–9) và có độ dài từ 8 đến 10 ký tự.");
        }

        int? excludeId = student.Id > 0 ? student.Id : null;
        var conflict = _repository.FindUserConflictByStudentCode(studentCode, excludeId);
        if (conflict is not null)
        {
            var detail = conflict.Role == UserRole.Student
                ? $"{conflict.FullName} – Lớp {conflict.ClassName}"
                : $"tài khoản {conflict.Username}";
            throw new InvalidOperationException($"MSSV \"{studentCode}\" đã tồn tại ({detail}).");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Vui lòng nhập họ và tên sinh viên.");
        }

        if (fullName.Length > 100)
        {
            throw new ArgumentException($"Họ và tên không được vượt quá 100 ký tự (hiện tại {fullName.Length} ký tự).");
        }

        if (string.IsNullOrWhiteSpace(className))
        {
            throw new ArgumentException("Vui lòng chọn lớp học.");
        }

        var matchedClass = _repository.GetClasses().FirstOrDefault(c =>
            string.Equals(c.Code, className, StringComparison.OrdinalIgnoreCase));
        if (matchedClass is null)
        {
            throw new ArgumentException($"Lớp \"{className}\" không tồn tại trong hệ thống.");
        }

        student.StudentCode = studentCode;
        student.Username = studentCode;
        student.FullName = fullName;
        student.ClassName = matchedClass.Code;
        student.Role = UserRole.Student;
    }

    public void SaveStudent(User student)
    {
        ArgumentNullException.ThrowIfNull(student);
        ValidateStudent(student);

        try
        {
            if (student.Id <= 0)
            {
                _repository.AddStudent(student);
            }
            else
            {
                _repository.UpdateStudent(student);
            }
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException(
                $"MSSV \"{student.StudentCode}\" đã tồn tại trong bảng Users. Vui lòng kiểm tra lại.",
                ex);
        }
    }

    public void DeleteStudent(int userId)
    {
        _repository.DeleteStudent(userId);
    }

    #endregion
}
