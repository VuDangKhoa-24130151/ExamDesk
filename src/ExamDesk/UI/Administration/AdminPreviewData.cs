using ExamDesk.Models;

namespace ExamDesk.UI.Administration;

// Dữ liệu minh họa trong bộ nhớ của phân hệ Bạn 1 (Quản trị & Ngân hàng câu hỏi).
internal sealed class ChapterPreview
{
    public int SubjectId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
}

internal sealed class ClassPreview
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
}

internal static class AdminPreviewData
{
    public static List<Subject> Subjects { get; } =
    [
        new Subject { Id = 1, Code = "LTNET", Name = "Lập trình .NET" },
        new Subject { Id = 2, Code = "CSDL", Name = "Cơ sở dữ liệu" },
        new Subject { Id = 3, Code = "MMT", Name = "Mạng máy tính" }
    ];

    public static List<ChapterPreview> Chapters { get; } = CreateInitialChapters();

    public static List<ClassPreview> Classes { get; } =
    [
        new ClassPreview { Id = 1, Code = "DH23DT", Name = "Đại học Công nghệ thông tin K23A", SchoolYear = "2023-2027" },
        new ClassPreview { Id = 2, Code = "DH23TH", Name = "Đại học Công nghệ thông tin K23B", SchoolYear = "2023-2027" },
        new ClassPreview { Id = 3, Code = "DH24PM", Name = "Đại học Kỹ thuật phần mềm K24", SchoolYear = "2024-2028" }
    ];

    public static List<User> Students { get; } = CreateInitialStudents();

    public static List<Question> Questions { get; } = CreateInitialQuestions();

    public static int CountChaptersBySubject(int subjectId)
        => Chapters.Count(c => c.SubjectId == subjectId);

    public static int CountQuestionsBySubject(int subjectId)
        => Questions.Count(q => q.SubjectId == subjectId);

    public static int CountQuestionsByChapter(int subjectId, int chapterNumber)
        => Questions.Count(q => q.SubjectId == subjectId && q.Chapter == chapterNumber);

    public static int CountStudentsByClass(string classCode)
        => Students.Count(s => string.Equals(s.ClassName, classCode, StringComparison.OrdinalIgnoreCase));

    private static List<User> CreateInitialStudents() =>
    [
        // === Lớp 1: DH23DT (14 sinh viên) ===
        CreateStudent(1, "24130151", "Vũ Đăng Khoa", "DH23DT"),
        CreateStudent(2, "23130001", "Nguyễn Minh An", "DH23DT"),
        CreateStudent(3, "23130002", "Trần Thị Bảo Châu", "DH23DT"),
        CreateStudent(4, "23130003", "Lê Hoàng Dũng", "DH23DT"),
        CreateStudent(5, "23130004", "Phạm Ngọc Ánh Dương", "DH23DT"),
        CreateStudent(6, "23130005", "Hoàng Gia Đạt", "DH23DT"),
        CreateStudent(7, "23130006", "Đỗ Thu Hà", "DH23DT"),
        CreateStudent(8, "23130007", "Bùi Quang Hải", "DH23DT"),
        CreateStudent(9, "23130008", "Đặng Thị Minh Hiền", "DH23DT"),
        CreateStudent(10, "23130009", "Ngô Quốc Huy", "DH23DT"),
        CreateStudent(11, "23130010", "Dương Khánh Linh", "DH23DT"),
        CreateStudent(12, "23130011", "Lý Thành Long", "DH23DT"),
        CreateStudent(13, "23130012", "Mai Phương Thảo", "DH23DT"),
        CreateStudent(14, "23130013", "Võ Quốc Tuấn", "DH23DT"),

        // === Lớp 2: DH23TH (13 sinh viên) ===
        CreateStudent(15, "23130101", "Nguyễn Hoàng Bách", "DH23TH"),
        CreateStudent(16, "23130102", "Trần Quốc Bảo", "DH23TH"),
        CreateStudent(17, "23130103", "Lê Thị Kim Chi", "DH23TH"),
        CreateStudent(18, "23130104", "Phạm Thành Đạt", "DH23TH"),
        CreateStudent(19, "23130105", "Hoàng Minh Đức", "DH23TH"),
        CreateStudent(20, "23130106", "Vũ Thị Hương Giang", "DH23TH"),
        CreateStudent(21, "23130107", "Đỗ Trung Hiếu", "DH23TH"),
        CreateStudent(22, "23130108", "Bùi Ngọc Khánh", "DH23TH"),
        CreateStudent(23, "23130109", "Đặng Tuấn Kiệt", "DH23TH"),
        CreateStudent(24, "23130110", "Ngô Thị Trúc Lam", "DH23TH"),
        CreateStudent(25, "23130111", "Dương Nhật Minh", "DH23TH"),
        CreateStudent(26, "23130112", "Lý Bảo Ngọc", "DH23TH"),
        CreateStudent(27, "23130113", "Phan Thanh Phong", "DH23TH"),

        // === Lớp 3: DH24PM (13 sinh viên) ===
        CreateStudent(28, "24140001", "Nguyễn Thị Quỳnh Anh", "DH24PM"),
        CreateStudent(29, "24140002", "Trần Đình Khôi", "DH24PM"),
        CreateStudent(30, "24140003", "Lê Minh Khang", "DH24PM"),
        CreateStudent(31, "24140004", "Phạm Hồng Nhung", "DH24PM"),
        CreateStudent(32, "24140005", "Hoàng Trọng Phúc", "DH24PM"),
        CreateStudent(33, "24140006", "Vũ Minh Quân", "DH24PM"),
        CreateStudent(34, "24140007", "Đỗ Thị Thanh Tâm", "DH24PM"),
        CreateStudent(35, "24140008", "Bùi Hữu Thắng", "DH24PM"),
        CreateStudent(36, "24140009", "Đặng Thùy Trang", "DH24PM"),
        CreateStudent(37, "24140010", "Ngô Đức Trọng", "DH24PM"),
        CreateStudent(38, "24140011", "Dương Thị Tuyết Vân", "DH24PM"),
        CreateStudent(39, "24140012", "Lương Quốc Việt", "DH24PM"),
        CreateStudent(40, "24140013", "Trịnh Hải Yến", "DH24PM")
    ];

    private static User CreateStudent(int id, string studentCode, string fullName, string classCode) => new()
    {
        Id = id,
        Username = studentCode,
        StudentCode = studentCode,
        FullName = fullName,
        ClassName = classCode,
        Role = UserRole.Student
    };

    private static List<ChapterPreview> CreateInitialChapters() =>
    [
        // Môn 1: LTNET – Lập trình .NET (Chương 1–4)
        new ChapterPreview { SubjectId = 1, Number = 1, Name = "Nền tảng .NET và ngôn ngữ C#" },
        new ChapterPreview { SubjectId = 1, Number = 2, Name = "Thiết kế giao diện với WinForms" },
        new ChapterPreview { SubjectId = 1, Number = 3, Name = "Truy vấn LINQ và lập trình bất đồng bộ" },
        new ChapterPreview { SubjectId = 1, Number = 4, Name = "Truy cập dữ liệu với ADO.NET và SQLite" },

        // Môn 2: CSDL – Cơ sở dữ liệu (Chương 1–4)
        new ChapterPreview { SubjectId = 2, Number = 1, Name = "Mô hình cơ sở dữ liệu quan hệ" },
        new ChapterPreview { SubjectId = 2, Number = 2, Name = "Ngôn ngữ truy vấn có cấu trúc (SQL)" },
        new ChapterPreview { SubjectId = 2, Number = 3, Name = "Chuẩn hóa dữ liệu và giao dịch (Transaction)" },
        new ChapterPreview { SubjectId = 2, Number = 4, Name = "Chỉ mục, tối ưu hóa và an toàn dữ liệu" },

        // Môn 3: MMT – Mạng máy tính (Chương 1–4)
        new ChapterPreview { SubjectId = 3, Number = 1, Name = "Tổng quan mạng máy tính và mô hình OSI" },
        new ChapterPreview { SubjectId = 3, Number = 2, Name = "Tầng ứng dụng và các giao thức mạng" },
        new ChapterPreview { SubjectId = 3, Number = 3, Name = "Tầng giao vận (TCP/UDP)" },
        new ChapterPreview { SubjectId = 3, Number = 4, Name = "Địa chỉ IPv4 và chia mạng con (Subnetting)" }
    ];

    public static string FormatDifficulty(string? difficulty) => difficulty switch
    {
        "Easy" => "Dễ",
        "Medium" => "Trung bình",
        "Hard" => "Khó",
        _ => difficulty ?? string.Empty
    };

    public static string FormatSubjectCode(int subjectId)
    {
        var subject = Subjects.FirstOrDefault(s => s.Id == subjectId);
        return subject is not null ? $"{subject.Code} – {subject.Name}" : $"Môn #{subjectId}";
    }

    public static string ShortenContent(string content, int maxLength = 85)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        var normalized = content.Replace("\r\n", " ").Replace('\n', ' ').Trim();
        return normalized.Length <= maxLength
            ? normalized
            : string.Concat(normalized.AsSpan(0, maxLength), "…");
    }

    private static List<Question> CreateInitialQuestions() =>
    [
        // === Môn 1: LTNET – Lập trình .NET (10 câu, Chương 1–4) ===
        new Question
        {
            Id = 101, SubjectId = 1, Chapter = 1, Difficulty = "Easy",
            Content = "Trong nền tảng .NET, thành phần nào chịu trách nhiệm quản lý bộ nhớ tự động (Garbage Collection) và thực thi mã trung gian IL?",
            OptionA = "CLR (Common Language Runtime)",
            OptionB = "BCL (Base Class Library)",
            OptionC = "CTS (Common Type System)",
            OptionD = "CLS (Common Language Specification)",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 102, SubjectId = 1, Chapter = 1, Difficulty = "Easy",
            Content = "Từ khóa nào trong C# dùng để khai báo một lớp không thể bị kế thừa bởi bất kỳ lớp nào khác?",
            OptionA = "abstract",
            OptionB = "static",
            OptionC = "sealed",
            OptionD = "readonly",
            CorrectOption = "C", ImagePath = string.Empty
        },
        new Question
        {
            Id = 103, SubjectId = 1, Chapter = 1, Difficulty = "Medium",
            Content = "Điểm khác biệt cốt lõi giữa kiểu dữ liệu tham trị (Value Type - struct) và kiểu tham chiếu (Reference Type - class) khi truyền tham số vào phương thức mà không có từ khóa ref/out là gì?",
            OptionA = "Kiểu tham trị truyền bản sao giá trị, còn kiểu tham chiếu truyền bản sao địa chỉ tham chiếu tới đối tượng trên Heap.",
            OptionB = "Cả hai đều cấp phát vùng nhớ mới trên vùng nhớ Heap trước khi gọi hàm.",
            OptionC = "Kiểu tham trị luôn bị đóng gói (boxing) khi truyền vào mọi phương thức.",
            OptionD = "Kiểu tham chiếu không cho phép thay đổi thuộc tính bên trong đối tượng được truyền vào.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 104, SubjectId = 1, Chapter = 2, Difficulty = "Easy",
            Content = "Trong C# WinForms, thuộc tính nào giúp một điều khiển (Control) tự động lấp đầy toàn bộ diện tích còn trống của vùng chứa cha?",
            OptionA = "Anchor = AnchorStyles.None",
            OptionB = "Dock = DockStyle.Fill",
            OptionC = "AutoSize = true",
            OptionD = "Margin = new Padding(0)",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 105, SubjectId = 1, Chapter = 2, Difficulty = "Medium",
            Content = "Khi thiết kế giao diện WinForms hỗ trợ nhiều mức phóng đại màn hình (DPI 100%, 125%, 150%) với phông chữ Segoe UI, thuộc tính AutoScaleMode của Form và UserControl nên đặt giá trị nào?",
            OptionA = "AutoScaleMode.None",
            OptionB = "AutoScaleMode.Font",
            OptionC = "AutoScaleMode.Inherit",
            OptionD = "AutoScaleMode.Dpi chỉ dành cho console",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 106, SubjectId = 1, Chapter = 2, Difficulty = "Hard",
            Content = "Giả sử một cửa sổ MainForm trong ứng dụng WinForms thực hiện chuyển đổi giữa các màn hình con bằng cách xóa UserControl cũ khỏi Panel chứa và gọi phương thức Dispose() trên UserControl đó. Nếu người dùng đang nhập dở dữ liệu ở màn hình soạn câu hỏi mà chuyển sang mục menu khác, giải pháp kiến trúc nào đảm bảo không mất dữ liệu ngoài ý muốn?",
            OptionA = "Kiểm tra cờ thay đổi chưa lưu (IsDirty) để hỏi xác nhận trước khi hủy control, đồng thời lưu dữ liệu minh họa ở tầng dùng chung ngoài vòng đời UserControl.",
            OptionB = "Gọi GC.SuppressFinalize() trong hàm khởi tạo của MainForm.",
            OptionC = "Đặt thuộc tính Visible = true sau khi đã gọi Dispose().",
            OptionD = "Bắt ngoại lệ ObjectDisposedException rồi tự động khôi phục các TextBox.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 107, SubjectId = 1, Chapter = 3, Difficulty = "Medium",
            Content = "Trong LINQ to Objects, phương thức mở rộng nào thực hiện lọc các phần tử thỏa mãn điều kiện predicate và áp dụng cơ chế thực thi trì hoãn (deferred execution)?",
            OptionA = "ToList()",
            OptionB = "Where()",
            OptionC = "Count()",
            OptionD = "FirstOrDefault()",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 108, SubjectId = 1, Chapter = 3, Difficulty = "Hard",
            Content = "Khi sử dụng async/await trong sự kiện nút bấm của WinForms (trên UI Thread), vì sao nên dùng await Task.Delay(500) thay vì Thread.Sleep(500) khi mô phỏng trạng thái chờ xử lý?",
            OptionA = "Task.Delay giải phóng luồng giao diện (UI Thread) trong lúc chờ giúp cửa sổ không bị treo cứng và kịp vẽ lại nhãn trạng thái 'Đang đăng nhập…'.",
            OptionB = "Thread.Sleep không biên dịch được trong dự án .NET 10.",
            OptionC = "Task.Delay tự động lưu dữ liệu vào cơ sở dữ liệu SQLite.",
            OptionD = "Thread.Sleep tự động đóng cửa sổ hiện tại ngay lập tức.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 109, SubjectId = 1, Chapter = 4, Difficulty = "Medium",
            Content = "Khi làm việc với Microsoft.Data.Sqlite trong C#, đối tượng nào được khuyến nghị sử dụng kèm câu lệnh using để tránh rò rỉ tài nguyên kết nối tệp cơ sở dữ liệu?",
            OptionA = "SqliteConnection và SqliteCommand",
            OptionB = "StringBuilder",
            OptionC = "DataGridViewRow",
            OptionD = "ColorTranslator",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 110, SubjectId = 1, Chapter = 4, Difficulty = "Hard",
            Content = "Để ngăn chặn triệt để lỗ hổng SQL Injection khi truy vấn câu hỏi theo từ khóa tìm kiếm do người dùng nhập vào, lập trình viên .NET cần thực hiện cách nào sau đây?",
            OptionA = "Sử dụng truy vấn tham số hóa (Parameterized Query) thông qua bộ sưu tập Parameters của SqliteCommand.",
            OptionB = "Nối chuỗi trực tiếp bằng toán tử + hoặc nội suy chuỗi $\"SELECT ... WHERE Content LIKE '%{keyword}%'\".",
            OptionC = "Chỉ kiểm tra độ dài chuỗi trên TextBox giao diện.",
            OptionD = "Đổi tên bảng trong cơ sở dữ liệu thành chữ in hoa.",
            CorrectOption = "A", ImagePath = string.Empty
        },

        // === Môn 2: CSDL – Cơ sở dữ liệu (10 câu, Chương 1–4) ===
        new Question
        {
            Id = 201, SubjectId = 2, Chapter = 1, Difficulty = "Easy",
            Content = "Trong mô hình cơ sở dữ liệu quan hệ, khóa chính (Primary Key) của một bảng có tính chất bắt buộc nào?",
            OptionA = "Giá trị duy nhất (Unique) và không được rỗng (NOT NULL).",
            OptionB = "Chỉ được phép dùng kiểu dữ liệu chuỗi ký tự.",
            OptionC = "Có thể chứa nhiều dòng mang giá trị NULL.",
            OptionD = "Bắt buộc phải tham chiếu tới bảng khác.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 202, SubjectId = 2, Chapter = 1, Difficulty = "Easy",
            Content = "Ràng buộc khóa ngoại (Foreign Key) giữa bảng Questions(SubjectId) và bảng Subjects(Id) nhằm đảm bảo tính chất nào của dữ liệu?",
            OptionA = "Toàn vẹn tham chiếu (Referential Integrity)",
            OptionB = "Tăng kích thước tệp lưu trữ",
            OptionC = "Tự động xóa bảng Subjects khi thêm câu hỏi",
            OptionD = "Cho phép nhập mã môn học không tồn tại",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 203, SubjectId = 2, Chapter = 1, Difficulty = "Medium",
            Content = "Trong thiết kế CSDL hệ thống thi trắc nghiệm, vì sao không nên cho phép xóa trực tiếp một câu hỏi trong ngân hàng khi câu hỏi đó đã nằm trong đề thi của ca thi đã diễn ra?",
            OptionA = "Để bảo toàn lịch sử đề thi, kết quả chấm điểm và dữ liệu đối chiếu phúc khảo của các lượt thi trước đó.",
            OptionB = "Vì SQLite không hỗ trợ câu lệnh DELETE.",
            OptionC = "Vì bảng Questions không có cột khóa chính Id.",
            OptionD = "Vì xóa câu hỏi sẽ làm thay đổi mật khẩu giảng viên.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 204, SubjectId = 2, Chapter = 2, Difficulty = "Easy",
            Content = "Mệnh đề nào trong câu lệnh SELECT của SQL được dùng để lọc các nhóm dữ liệu sau khi đã gom nhóm bằng GROUP BY?",
            OptionA = "WHERE",
            OptionB = "HAVING",
            OptionC = "ORDER BY",
            OptionD = "DISTINCT",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 205, SubjectId = 2, Chapter = 2, Difficulty = "Medium",
            Content = "Để lấy danh sách tất cả môn học cùng số lượng câu hỏi hiện có của từng môn (kể cả môn mới tạo chưa có câu hỏi nào, hiển thị số lượng = 0), phép kết nối nào cần được sử dụng?",
            OptionA = "INNER JOIN từ Questions sang Subjects",
            OptionB = "LEFT JOIN từ Subjects sang Questions kết hợp COUNT(Questions.Id)",
            OptionC = "CROSS JOIN không điều kiện",
            OptionD = "NATURAL JOIN chỉ lấy dòng trùng tên",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 206, SubjectId = 2, Chapter = 2, Difficulty = "Hard",
            Content = "Khi rút ngẫu nhiên N câu hỏi theo ma trận độ khó (Dễ, Trung bình, Khó) của một môn học để phát hành đề thi mà đảm bảo không trùng lặp câu hỏi trong cùng một đề, truy vấn và kiểm tra nghiệp vụ cần thỏa mãn điều kiện nào?",
            OptionA = "Kiểm tra số lượng câu khả dụng trong ngân hàng của từng mức độ khó phải lớn hơn hoặc bằng số câu yêu cầu trước khi lấy mẫu không hoàn lại.",
            OptionB = "Lấy ngẫu nhiên có hoàn lại để một câu hỏi có thể xuất hiện 3 lần trong cùng đề thi.",
            OptionC = "Bỏ qua điều kiện lọc theo SubjectId để lấy đủ số lượng từ các môn học khác.",
            OptionD = "Luôn chỉ lấy các câu hỏi có Id nhỏ nhất.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 207, SubjectId = 2, Chapter = 3, Difficulty = "Medium",
            Content = "Một quan hệ R đạt dạng chuẩn 3 (3NF) khi nó đã ở dạng chuẩn 2 (2NF) và thỏa mãn thêm điều kiện nào?",
            OptionA = "Mọi thuộc tính không khóa đều không phụ thuộc bắc cầu vào khóa chính.",
            OptionB = "Bảng phải có ít nhất 10 cột dữ liệu.",
            OptionC = "Tất cả các cột đều có kiểu số nguyên.",
            OptionD = "Không được sử dụng khóa chính đơn.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 208, SubjectId = 2, Chapter = 3, Difficulty = "Hard",
            Content = "Xét kịch bản sinh viên bấm nộp bài thi trắc nghiệm: hệ thống cần cập nhật trạng thái lượt thi sang 'Đã nộp', lưu chi tiết từng phương án đã chọn và ghi nhận điểm số cuối cùng. Nếu mất điện đột ngột giữa chừng, tính chất nào của Giao dịch (Transaction) đảm bảo cơ sở dữ liệu không rơi vào trạng thái dở dang?",
            OptionA = "Atomicity (Tính nguyên tố) và Durability (Tính bền vững) trong bộ nguyên tắc ACID.",
            OptionB = "Tắt hoàn toàn cơ chế khóa tệp của hệ quản trị CSDL.",
            OptionC = "Lưu điểm số trước rồi xóa câu trả lời của sinh viên.",
            OptionD = "Chỉ lưu kết quả trên thanh tiêu đề cửa sổ.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 209, SubjectId = 2, Chapter = 4, Difficulty = "Easy",
            Content = "Việc tạo chỉ mục (Index) trên các cột thường xuyên dùng để lọc như SubjectId, Chapter và Difficulty trong bảng Questions mang lại lợi ích chính nào?",
            OptionA = "Tăng tốc độ truy vấn tìm kiếm và lọc câu hỏi trên tập dữ liệu lớn.",
            OptionB = "Tự động dịch nội dung câu hỏi sang tiếng Anh.",
            OptionC = "Giúp bảng không cần khóa chính.",
            OptionD = "Loại bỏ hoàn toàn nhu cầu sao lưu cơ sở dữ liệu.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 210, SubjectId = 2, Chapter = 4, Difficulty = "Medium",
            Content = "Trong SQLite chạy cục bộ (Offline), thao tác sao lưu cơ sở dữ liệu an toàn cần lưu ý điều gì khi đang có kết nối đọc/ghi?",
            OptionA = "Đảm bảo đóng hoặc đồng bộ hóa các giao dịch đang mở trước khi tạo bản sao tệp cơ sở dữ liệu.",
            OptionB = "Chỉ sao chép tệp thực thi .exe mà bỏ qua tệp .db.",
            OptionC = "Đổi phần mở rộng tệp .db thành .txt khi đang ghi dữ liệu.",
            OptionD = "Xóa bảng cũ trước khi tạo tệp sao lưu.",
            CorrectOption = "A", ImagePath = string.Empty
        },

        // === Môn 3: MMT – Mạng máy tính (10 câu, Chương 1–4) ===
        new Question
        {
            Id = 301, SubjectId = 3, Chapter = 1, Difficulty = "Easy",
            Content = "Mô hình tham chiếu OSI (Open Systems Interconnection) bao gồm bao nhiêu tầng chức năng?",
            OptionA = "4 tầng",
            OptionB = "5 tầng",
            OptionC = "7 tầng",
            OptionD = "8 tầng",
            CorrectOption = "C", ImagePath = string.Empty
        },
        new Question
        {
            Id = 302, SubjectId = 3, Chapter = 1, Difficulty = "Easy",
            Content = "Đơn vị dữ liệu giao thức (PDU) tại tầng Giao vận (Transport Layer) khi sử dụng giao thức TCP được gọi là gì?",
            OptionA = "Bit",
            OptionB = "Frame (Khung tin)",
            OptionC = "Packet (Gói tin)",
            OptionD = "Segment (Đoạn tin)",
            CorrectOption = "D", ImagePath = string.Empty
        },
        new Question
        {
            Id = 303, SubjectId = 3, Chapter = 1, Difficulty = "Medium",
            Content = "Trong kiến trúc mạng phòng máy thi trắc nghiệm nội bộ (LAN), thiết bị Switch hoạt động chủ yếu ở tầng nào của mô hình OSI để chuyển tiếp khung tin dựa trên địa chỉ MAC?",
            OptionA = "Tầng Vật lý (Physical Layer - Tầng 1)",
            OptionB = "Tầng Liên kết dữ liệu (Data Link Layer - Tầng 2)",
            OptionC = "Tầng Phiên (Session Layer - Tầng 5)",
            OptionD = "Tầng Ứng dụng (Application Layer - Tầng 7)",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 304, SubjectId = 3, Chapter = 2, Difficulty = "Easy",
            Content = "Giao thức nào ở tầng Ứng dụng chịu trách nhiệm phân giải tên miền (ví dụ: uniexam.local) thành địa chỉ IP tương ứng?",
            OptionA = "DHCP",
            OptionB = "DNS",
            OptionC = "FTP",
            OptionD = "SMTP",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 305, SubjectId = 3, Chapter = 2, Difficulty = "Medium",
            Content = "Giao thức HTTP/HTTPS mặc định sử dụng cặp cổng (Port) chuẩn nào ở phía máy chủ?",
            OptionA = "20 và 21",
            OptionB = "22 và 23",
            OptionC = "80 và 443",
            OptionD = "110 và 143",
            CorrectOption = "C", ImagePath = string.Empty
        },
        new Question
        {
            Id = 306, SubjectId = 3, Chapter = 3, Difficulty = "Medium",
            Content = "Đặc điểm nào sau đây mô tả đúng giao thức TCP (Transmission Control Protocol) so với giao thức UDP?",
            OptionA = "Hướng kết nối (Connection-oriented), truyền tin cậy, có kiểm soát luồng và kiểm soát tắc nghẽn.",
            OptionB = "Không thiết lập kết nối trước khi truyền và không đảm bảo thứ tự gói tin.",
            OptionC = "Không có trường số thứ tự (Sequence Number) trong phần tiêu đề.",
            OptionD = "Chỉ dùng cho các bản tin quảng bá (Broadcast) trong mạng nội bộ.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 307, SubjectId = 3, Chapter = 3, Difficulty = "Hard",
            Content = "Trong quá trình bắt tay ba bước (Three-way Handshake) để thiết lập kết nối TCP giữa máy trạm (Client) và máy chủ (Server), trình tự các cờ điều khiển được trao đổi lần lượt như thế nào?",
            OptionA = "Client gửi SYN → Server phản hồi SYN-ACK → Client gửi ACK.",
            OptionB = "Client gửi ACK → Server phản hồi SYN → Client gửi FIN.",
            OptionC = "Server gửi SYN → Client gửi RST → Server gửi ACK.",
            OptionD = "Client gửi FIN-ACK → Server gửi SYN-ACK → Client gửi SYN.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 308, SubjectId = 3, Chapter = 4, Difficulty = "Easy",
            Content = "Một địa chỉ IPv4 có độ dài bao nhiêu bit và được chia thành bao nhiêu octet?",
            OptionA = "32 bit, chia thành 4 octet (mỗi octet 8 bit).",
            OptionB = "64 bit, chia thành 8 octet.",
            OptionC = "128 bit, biểu diễn bằng hệ thập lục phân.",
            OptionD = "48 bit, gắn cố định trên card mạng.",
            CorrectOption = "A", ImagePath = string.Empty
        },
        new Question
        {
            Id = 309, SubjectId = 3, Chapter = 4, Difficulty = "Medium",
            Content = "Với dải mạng nội bộ phòng thi có địa chỉ 192.168.10.0/24 (Subnet Mask 255.255.255.0), số lượng địa chỉ IP hợp lệ tối đa có thể gán cho các máy tính trạm (Host) là bao nhiêu?",
            OptionA = "256 địa chỉ",
            OptionB = "254 địa chỉ (trừ địa chỉ mạng .0 và địa chỉ quảng bá .255)",
            OptionC = "128 địa chỉ",
            OptionD = "512 địa chỉ",
            CorrectOption = "B", ImagePath = string.Empty
        },
        new Question
        {
            Id = 310, SubjectId = 3, Chapter = 4, Difficulty = "Hard",
            Content = "Một quản trị viên mạng cần chia mạng 192.168.1.0/24 thành 4 mạng con (Subnet) có kích thước bằng nhau cho 4 phòng thi độc lập. Mặt nạ mạng con (Subnet Mask) mới theo ký hiệu CIDR và số địa chỉ Host khả dụng trên mỗi phòng thi sẽ là bao nhiêu?",
            OptionA = "/26 (255.255.255.192) với 62 địa chỉ Host khả dụng cho mỗi phòng thi.",
            OptionB = "/25 (255.255.255.128) với 126 địa chỉ Host khả dụng cho mỗi phòng thi.",
            OptionC = "/27 (255.255.255.224) với 30 địa chỉ Host khả dụng cho mỗi phòng thi.",
            OptionD = "/28 (255.255.255.240) với 14 địa chỉ Host khả dụng cho mỗi phòng thi.",
            CorrectOption = "A", ImagePath = string.Empty
        }
    ];
}
