using System.Text.Json;
using ExamDesk.Data;
using ExamDesk.Models;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Repositories;

// Bạn 1 phụ trách: Quản lý tài khoản giảng viên (AUTH-02), lớp học và sinh viên (GV-03) qua SQLite.
public sealed class AdministrationRepository
{
    private static readonly object MetadataLock = new();

    private sealed class ClassMetadataEntry
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
    }

    private static string ClassMetadataFilePath => Path.Combine(
        Path.GetDirectoryName(SqliteContext.DatabaseFilePath)!,
        "class_metadata.json");

    #region Lecturer Auth Operations

    public bool HasAnyLecturer()
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM Users WHERE Role = 'Lecturer');";
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    public void EnsureDefaultLecturerAccount(string username, string passwordHash, string fullName)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, StudentCode, ClassName)
            SELECT $username, $passwordHash, $fullName, 'Lecturer', '', ''
            WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Role = 'Lecturer')
              AND NOT EXISTS (SELECT 1 FROM Users WHERE LOWER(TRIM(Username)) = LOWER(TRIM($username)));";
        command.Parameters.AddWithValue("$username", username.Trim());
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        command.Parameters.AddWithValue("$fullName", fullName.Trim());
        command.ExecuteNonQuery();
    }

    public User? FindLecturerByCredentials(string username, string passwordHash)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Username, FullName, StudentCode, ClassName
            FROM Users
            WHERE Role = 'Lecturer'
              AND LOWER(TRIM(Username)) = LOWER(TRIM($username))
              AND PasswordHash = $passwordHash
            LIMIT 1;";
        command.Parameters.AddWithValue("$username", username.Trim());
        command.Parameters.AddWithValue("$passwordHash", passwordHash);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            FullName = reader.GetString(2),
            StudentCode = reader.GetString(3),
            ClassName = reader.GetString(4),
            Role = UserRole.Lecturer
        };
    }

    #endregion

    #region Class Operations

    public List<ClassInfo> GetClasses()
    {
        var studentCounts = GetStudentCountsByClassFromDb();

        lock (MetadataLock)
        {
            var entries = LoadAndSyncMetadataEntries(studentCounts);
            return entries
                .OrderBy(e => e.Code, StringComparer.OrdinalIgnoreCase)
                .Select(e => new ClassInfo
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    SchoolYear = e.SchoolYear,
                    StudentCount = studentCounts.TryGetValue(e.Code, out var count) ? count : 0
                })
                .ToList();
        }
    }

    public bool IsClassCodeExists(string code, int? excludeClassId = null)
    {
        var normalized = code.Trim();
        return GetClasses().Any(c =>
            string.Equals(c.Code, normalized, StringComparison.OrdinalIgnoreCase) &&
            (!excludeClassId.HasValue || c.Id != excludeClassId.Value));
    }

    public int CountStudentsByClass(string classCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*)
            FROM Users
            WHERE Role = 'Student'
              AND UPPER(TRIM(ClassName)) = UPPER(TRIM($classCode));";
        command.Parameters.AddWithValue("$classCode", classCode.Trim());

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int AddClass(ClassInfo classInfo)
    {
        ArgumentNullException.ThrowIfNull(classInfo);
        var studentCounts = GetStudentCountsByClassFromDb();

        lock (MetadataLock)
        {
            var entries = LoadAndSyncMetadataEntries(studentCounts);
            var normalizedCode = classInfo.Code.Trim().ToUpperInvariant();

            if (entries.Any(e => string.Equals(e.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Mã lớp \"{normalizedCode}\" đã tồn tại. Vui lòng nhập mã khác.");
            }

            var nextId = entries.Count > 0 ? entries.Max(e => e.Id) + 1 : 1;
            entries.Add(new ClassMetadataEntry
            {
                Id = nextId,
                Code = normalizedCode,
                Name = classInfo.Name.Trim(),
                SchoolYear = classInfo.SchoolYear.Trim()
            });

            SaveMetadata(entries);
            classInfo.Id = nextId;
            classInfo.Code = normalizedCode;
            return nextId;
        }
    }

    public void UpdateClass(ClassInfo classInfo, string? oldCode = null)
    {
        ArgumentNullException.ThrowIfNull(classInfo);
        var studentCounts = GetStudentCountsByClassFromDb();

        lock (MetadataLock)
        {
            var entries = LoadAndSyncMetadataEntries(studentCounts);
            var existing = entries.FirstOrDefault(e => e.Id == classInfo.Id);
            if (existing is null && !string.IsNullOrWhiteSpace(oldCode))
            {
                existing = entries.FirstOrDefault(e =>
                    string.Equals(e.Code, oldCode.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (existing is null)
            {
                throw new InvalidOperationException("Không tìm thấy lớp học cần cập nhật.");
            }

            var newCode = classInfo.Code.Trim().ToUpperInvariant();
            if (entries.Any(e => e.Id != existing.Id && string.Equals(e.Code, newCode, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Mã lớp \"{newCode}\" đã tồn tại. Vui lòng nhập mã khác.");
            }

            var actualOldCode = existing.Code;
            if (!string.Equals(actualOldCode, newCode, StringComparison.Ordinal))
            {
                UpdateStudentClassNameInDb(actualOldCode, newCode);
            }

            existing.Code = newCode;
            existing.Name = classInfo.Name.Trim();
            existing.SchoolYear = classInfo.SchoolYear.Trim();
            SaveMetadata(entries);

            classInfo.Id = existing.Id;
            classInfo.Code = newCode;
        }
    }

    public void DeleteClass(int classId)
    {
        var studentCounts = GetStudentCountsByClassFromDb();

        lock (MetadataLock)
        {
            var entries = LoadAndSyncMetadataEntries(studentCounts);
            var existing = entries.FirstOrDefault(e => e.Id == classId)
                ?? throw new InvalidOperationException("Không tìm thấy lớp học để xóa.");

            var studentCount = CountStudentsByClass(existing.Code);
            if (studentCount > 0)
            {
                throw new InvalidOperationException(
                    $"Không thể xóa lớp \"{existing.Code} – {existing.Name}\" vì lớp đang có {studentCount} sinh viên.");
            }

            entries.Remove(existing);
            SaveMetadata(entries);
        }
    }

    private static Dictionary<string, int> GetStudentCountsByClassFromDb()
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TRIM(ClassName) AS ClassCode, COUNT(*) AS StudentCount
            FROM Users
            WHERE Role = 'Student' AND TRIM(ClassName) <> ''
            GROUP BY TRIM(ClassName)
            ORDER BY TRIM(ClassName);";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var code = reader.GetString(0);
            var count = reader.GetInt32(1);
            counts[code] = counts.TryGetValue(code, out var existingCount) ? existingCount + count : count;
        }

        return counts;
    }

    private static void UpdateStudentClassNameInDb(string oldCode, string newCode)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                UPDATE Users
                SET ClassName = $newCode
                WHERE Role = 'Student'
                  AND UPPER(TRIM(ClassName)) = UPPER(TRIM($oldCode));";
            command.Parameters.AddWithValue("$newCode", newCode);
            command.Parameters.AddWithValue("$oldCode", oldCode);
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static List<ClassMetadataEntry> LoadAndSyncMetadataEntries(Dictionary<string, int> studentCounts)
    {
        var path = ClassMetadataFilePath;
        List<ClassMetadataEntry> entries;
        var needsSave = false;

        if (!File.Exists(path))
        {
            entries = studentCounts.Count == 0 ? CreateDefaultClasses() : [];
            needsSave = true;
        }
        else
        {
            entries = LoadMetadata(path);
        }

        // Gom các ClassName có trong bảng Users (Role = 'Student') để không bỏ sót lớp nào (kể cả TEST_LOP01).
        foreach (var dbClassCode in studentCounts.Keys)
        {
            if (entries.Any(e => string.Equals(e.Code, dbClassCode, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var nextId = entries.Count > 0 ? entries.Max(e => e.Id) + 1 : 1;
            entries.Add(new ClassMetadataEntry
            {
                Id = nextId,
                Code = dbClassCode,
                Name = string.Equals(dbClassCode, "TEST_LOP01", StringComparison.OrdinalIgnoreCase)
                    ? "Lớp kiểm thử sinh viên (TEST_LOP01)"
                    : $"Lớp {dbClassCode}",
                SchoolYear = "2024-2028"
            });
            needsSave = true;
        }

        if (needsSave)
        {
            SaveMetadata(entries);
        }

        return entries;
    }

    private static List<ClassMetadataEntry> LoadMetadata(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<ClassMetadataEntry>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static void SaveMetadata(List<ClassMetadataEntry> entries)
    {
        var path = ClassMetadataFilePath;
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private static List<ClassMetadataEntry> CreateDefaultClasses() =>
    [
        new ClassMetadataEntry { Id = 1, Code = "DH23DT", Name = "Đại học Công nghệ thông tin K23A", SchoolYear = "2023-2027" },
        new ClassMetadataEntry { Id = 2, Code = "DH23TH", Name = "Đại học Công nghệ thông tin K23B", SchoolYear = "2023-2027" },
        new ClassMetadataEntry { Id = 3, Code = "DH24PM", Name = "Đại học Kỹ thuật phần mềm K24", SchoolYear = "2024-2028" }
    ];

    #endregion

    #region Student Operations

    public List<User> GetStudentsByClass(string classCode, string? keyword = null)
    {
        var list = new List<User>();
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        var sql = @"
            SELECT Id, Username, FullName, StudentCode, ClassName
            FROM Users
            WHERE Role = 'Student'
              AND UPPER(TRIM(ClassName)) = UPPER(TRIM($classCode))";
        command.Parameters.AddWithValue("$classCode", classCode.Trim());

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += " AND (StudentCode LIKE $keyword OR FullName LIKE $keyword)";
            command.Parameters.AddWithValue("$keyword", $"%{keyword.Trim()}%");
        }

        sql += " ORDER BY StudentCode ASC;";
        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                FullName = reader.GetString(2),
                StudentCode = reader.GetString(3),
                ClassName = reader.GetString(4),
                Role = UserRole.Student
            });
        }

        return list;
    }

    public User? FindUserConflictByStudentCode(string studentCode, int? excludeUserId = null)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Username, FullName, StudentCode, ClassName, Role
            FROM Users
            WHERE (LOWER(TRIM(StudentCode)) = LOWER(TRIM($code))
                OR LOWER(TRIM(Username)) = LOWER(TRIM($code)))
              AND ($excludeId IS NULL OR Id <> $excludeId)
            LIMIT 1;";
        command.Parameters.AddWithValue("$code", studentCode.Trim());
        command.Parameters.AddWithValue("$excludeId", excludeUserId.HasValue ? excludeUserId.Value : DBNull.Value);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var roleText = reader.GetString(5);
        var role = Enum.TryParse<UserRole>(roleText, ignoreCase: true, out var parsed)
            ? parsed
            : UserRole.Student;

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            FullName = reader.GetString(2),
            StudentCode = reader.GetString(3),
            ClassName = reader.GetString(4),
            Role = role
        };
    }

    public int AddStudent(User student)
    {
        ArgumentNullException.ThrowIfNull(student);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, StudentCode, ClassName)
            VALUES ($username, $passwordHash, $fullName, 'Student', $studentCode, $className);
            SELECT last_insert_rowid();";
        var code = student.StudentCode.Trim();
        command.Parameters.AddWithValue("$username", code);
        command.Parameters.AddWithValue("$passwordHash", "!STUDENT_NO_PASSWORD!");
        command.Parameters.AddWithValue("$fullName", student.FullName.Trim());
        command.Parameters.AddWithValue("$studentCode", code);
        command.Parameters.AddWithValue("$className", student.ClassName.Trim());

        var newId = Convert.ToInt32(command.ExecuteScalar());
        student.Id = newId;
        student.Username = code;
        student.StudentCode = code;
        student.Role = UserRole.Student;
        return newId;
    }

    public void UpdateStudent(User student)
    {
        ArgumentNullException.ThrowIfNull(student);

        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            string oldStudentCode;
            using (var selectCmd = connection.CreateCommand())
            {
                selectCmd.Transaction = transaction;
                selectCmd.CommandText = @"
                    SELECT StudentCode
                    FROM Users
                    WHERE Id = $id AND Role = 'Student'
                    LIMIT 1;";
                selectCmd.Parameters.AddWithValue("$id", student.Id);

                var result = selectCmd.ExecuteScalar() as string;
                if (result is null)
                {
                    throw new InvalidOperationException("Không tìm thấy sinh viên cần cập nhật.");
                }

                oldStudentCode = result;
            }

            var newStudentCode = student.StudentCode.Trim();
            if (!string.Equals(oldStudentCode, newStudentCode, StringComparison.OrdinalIgnoreCase))
            {
                var (attemptCount, draftCount) = CountStudentExamUsage(connection, transaction, oldStudentCode);
                if (attemptCount > 0 || draftCount > 0)
                {
                    throw new InvalidOperationException(
                        $"Sinh viên {oldStudentCode} đã có dữ liệu bài thi/lượt làm bài ({attemptCount} bài nộp, {draftCount} bản nháp), không thể thay đổi MSSV.");
                }
            }

            using (var updateCmd = connection.CreateCommand())
            {
                updateCmd.Transaction = transaction;
                updateCmd.CommandText = @"
                    UPDATE Users
                    SET Username = $username,
                        FullName = $fullName,
                        StudentCode = $studentCode,
                        ClassName = $className
                    WHERE Id = $id AND Role = 'Student';";
                updateCmd.Parameters.AddWithValue("$username", newStudentCode);
                updateCmd.Parameters.AddWithValue("$fullName", student.FullName.Trim());
                updateCmd.Parameters.AddWithValue("$studentCode", newStudentCode);
                updateCmd.Parameters.AddWithValue("$className", student.ClassName.Trim());
                updateCmd.Parameters.AddWithValue("$id", student.Id);
                updateCmd.ExecuteNonQuery();
            }

            transaction.Commit();
            student.Username = newStudentCode;
            student.StudentCode = newStudentCode;
            student.Role = UserRole.Student;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DeleteStudent(int userId)
    {
        using var connection = SqliteContext.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            string studentCode;
            string fullName;
            using (var selectCmd = connection.CreateCommand())
            {
                selectCmd.Transaction = transaction;
                selectCmd.CommandText = @"
                    SELECT StudentCode, FullName
                    FROM Users
                    WHERE Id = $id AND Role = 'Student'
                    LIMIT 1;";
                selectCmd.Parameters.AddWithValue("$id", userId);

                using var reader = selectCmd.ExecuteReader();
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Không tìm thấy sinh viên để xóa.");
                }

                studentCode = reader.GetString(0);
                fullName = reader.GetString(1);
            }

            var (attemptCount, draftCount) = CountStudentExamUsage(connection, transaction, studentCode);
            if (attemptCount > 0 && draftCount > 0)
            {
                throw new InvalidOperationException(
                    $"Không thể xóa sinh viên {studentCode} – {fullName} vì đã có {attemptCount} bài thi đã nộp (ExamAttempts) và {draftCount} lượt làm bài đang lưu (StudentDrafts).");
            }

            if (attemptCount > 0)
            {
                throw new InvalidOperationException(
                    $"Không thể xóa sinh viên {studentCode} – {fullName} vì đã có {attemptCount} bài thi đã nộp trong hệ thống (ExamAttempts).");
            }

            if (draftCount > 0)
            {
                throw new InvalidOperationException(
                    $"Không thể xóa sinh viên {studentCode} – {fullName} vì đang có {draftCount} lượt làm bài trong hệ thống (StudentDrafts).");
            }

            using (var deleteCmd = connection.CreateCommand())
            {
                deleteCmd.Transaction = transaction;
                deleteCmd.CommandText = "DELETE FROM Users WHERE Id = $id AND Role = 'Student';";
                deleteCmd.Parameters.AddWithValue("$id", userId);
                deleteCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static (int AttemptCount, int DraftCount) CountStudentExamUsage(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string studentCode)
    {
        using var checkCmd = connection.CreateCommand();
        checkCmd.Transaction = transaction;
        checkCmd.CommandText = @"
            SELECT
                (SELECT COUNT(*) FROM ExamAttempts WHERE StudentCode = $studentCode),
                (SELECT COUNT(*) FROM StudentDrafts WHERE StudentCode = $studentCode);";
        checkCmd.Parameters.AddWithValue("$studentCode", studentCode);

        using var reader = checkCmd.ExecuteReader();
        if (!reader.Read())
        {
            return (0, 0);
        }

        return (reader.GetInt32(0), reader.GetInt32(1));
    }

    #endregion
}
