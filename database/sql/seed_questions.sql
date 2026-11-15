-- SEED DỮ LIỆU MẪU CHO NGÂN HÀNG CÂU HỎI & MÔN HỌC
-- Bao gồm 3 môn: LTNET (Lập trình .NET), CSDL (Cơ sở dữ liệu), MMT (Mạng máy tính)
-- Mỗi môn 24 câu hỏi mẫu phân bổ đều theo 4 Chương (mỗi chương 6 câu) và 3 mức Độ khó (Easy, Medium, Hard).
-- Cú pháp an toàn (idempotent), không gây trùng lặp dữ liệu khi nạp nhiều lần.

PRAGMA foreign_keys = ON;

-- 1. TẠO CÁC MÔN HỌC NẾU CHƯA TỒN TẠI
INSERT INTO Subjects (Code, Name) VALUES ('LTNET', 'Lập trình .NET') ON CONFLICT(Code) DO NOTHING;
INSERT INTO Subjects (Code, Name) VALUES ('CSDL', 'Cơ sở dữ liệu') ON CONFLICT(Code) DO NOTHING;
INSERT INTO Subjects (Code, Name) VALUES ('MMT', 'Mạng máy tính') ON CONFLICT(Code) DO NOTHING;

-- 2. CÂU HỎI MÔN LTNET: LẬP TRÌNH .NET (24 CÂU, CHƯƠNG 1–4)
WITH Data_LTNET(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Nền tảng .NET và ngôn ngữ C# (6 câu)
(1, 'Easy', 'Trong nền tảng .NET, thành phần nào chịu trách nhiệm quản lý bộ nhớ tự động (Garbage Collection) và thực thi mã trung gian IL?',
 'CLR (Common Language Runtime)', 'BCL (Base Class Library)', 'CTS (Common Type System)', 'CLS (Common Language Specification)', 'A'),

(1, 'Easy', 'Trong C#, kiểu dữ liệu nào dưới đây thuộc nhóm kiểu tham trị (Value Type)?',
 'string', 'struct', 'class', 'interface', 'B'),

(1, 'Medium', 'Từ khóa nào trong C# dùng để khai báo một lớp không thể bị kế thừa bởi bất kỳ lớp nào khác?',
 'abstract', 'static', 'sealed', 'readonly', 'C'),

(1, 'Medium', 'Khi khai báo phương thức trong lớp cơ sở, từ khóa nào cho phép lớp dẫn xuất có thể ghi đè (override) hành vi của phương thức đó?',
 'static', 'new', 'sealed', 'virtual', 'D'),

(1, 'Hard', 'Điểm khác biệt cốt lõi giữa kiểu dữ liệu tham trị (Value Type) và kiểu tham chiếu (Reference Type) khi truyền tham số vào phương thức mà không có ref/out là gì?',
 'Kiểu tham trị truyền bản sao giá trị, còn kiểu tham chiếu truyền bản sao địa chỉ tham chiếu tới đối tượng trên Heap.',
 'Cả hai đều cấp phát vùng nhớ mới trên vùng nhớ Heap trước khi gọi hàm.',
 'Kiểu tham trị luôn bị đóng gói (boxing) khi truyền vào mọi phương thức.',
 'Kiểu tham chiếu không cho phép thay đổi thuộc tính bên trong đối tượng được truyền vào.', 'A'),

(1, 'Hard', 'Trong C# 8.0 trở lên, cơ chế Pattern Matching với cú pháp switch expression trả về giá trị dựa trên kiểu hoặc thuộc tính của đối tượng giúp ích gì cho lập trình viên?',
 'Tự động chuyển đổi toàn bộ mã nguồn sang ngôn ngữ C++ để tăng tốc độ.',
 'Viết biểu thức rẽ nhánh ngắn gọn, an toàn kiểu dữ liệu tại thời điểm biên dịch và giảm thiểu lỗi null.',
 'Bỏ qua hoàn toàn việc kiểm tra điều kiện logic khi chạy ứng dụng.',
 'Tự động ghi log mọi ngoại lệ ra tệp văn bản cục bộ.', 'B'),

-- Chương 2: Thiết kế giao diện với WinForms (6 câu)
(2, 'Easy', 'Trong C# WinForms, thuộc tính nào của Control xác định văn bản hiển thị trực tiếp trên bề mặt của điều khiển đó?',
 'Name', 'Tag', 'Text', 'Value', 'C'),

(2, 'Easy', 'Thuộc tính nào giúp một điều khiển WinForms tự động neo các cạnh của nó vào các cạnh tương ứng của vùng chứa cha khi cửa sổ thay đổi kích thước?',
 'Dock', 'AutoSize', 'Margin', 'Anchor', 'D'),

(2, 'Medium', 'Khi thiết kế giao diện WinForms hỗ trợ nhiều mức phóng đại màn hình (DPI 100%, 125%, 150%) với phông chữ Segoe UI, thuộc tính AutoScaleMode của Form và UserControl nên đặt giá trị nào?',
 'AutoScaleMode.Font', 'AutoScaleMode.None', 'AutoScaleMode.Dpi', 'AutoScaleMode.Inherit', 'A'),

(2, 'Medium', 'Để ngăn chặn hiện tượng nhấp nháy màn hình (flickering) khi vẽ lại hoặc cuộn các điều khiển phức tạp như DataGridView hay Panel trong WinForms, kỹ thuật nào sau đây được áp dụng?',
 'Giảm kích thước cửa sổ xuống dưới 800x600.',
 'Bật thuộc tính DoubleBuffered = true trên điều khiển.',
 'Gọi Application.Exit() trước khi vẽ lại.',
 'Đặt thuộc tính Visible = false vĩnh viễn.', 'B'),

(2, 'Hard', 'Xét kịch bản người dùng đang nhập dở dữ liệu trên UserControl nhưng bấm chuyển sang màn hình khác: giải pháp kiến trúc nào đảm bảo ngăn ngừa mất mát dữ liệu?',
 'Gọi GC.Collect() ngay khi người dùng bấm chuyển tab.',
 'Tự động lưu dữ liệu đè lên cơ sở dữ liệu mà không cần hỏi người dùng.',
 'Triển khai giao diện IUnsavedChangesGuard để kiểm tra cờ HasUnsavedChanges và hiển thị hộp thoại xác nhận trước khi rời màn hình.',
 'Khóa toàn bộ bàn phím và chuột của hệ điều hành.', 'C'),

(2, 'Hard', 'Trong WinForms, phương thức nào dùng để ủy thác việc cập nhật giao diện người dùng từ một luồng nền (Background Thread) về đúng luồng giao diện chính (UI Thread)?',
 'Thread.Sleep()', 'Process.Start()', 'GC.WaitForPendingFinalizers()', 'Control.Invoke() hoặc Control.BeginInvoke()', 'D'),

-- Chương 3: Truy vấn LINQ và lập trình bất đồng bộ (6 câu)
(3, 'Easy', 'Từ khóa nào trong C# biểu thị phương thức có chứa các thao tác bất đồng bộ và cho phép sử dụng toán tử await bên trong thân hàm?',
 'async', 'yield', 'unsafe', 'volatile', 'A'),

(3, 'Easy', 'Trong LINQ to Objects, phương thức mở rộng nào được sử dụng để lọc các phần tử của tập hợp theo một biểu thức điều kiện (Predicate)?',
 'Select()', 'Where()', 'OrderBy()', 'GroupBy()', 'B'),

(3, 'Medium', 'Khi sử dụng async/await trên luồng giao diện WinForms, vì sao nên dùng await Task.Delay(500) thay vì Thread.Sleep(500)?',
 'Thread.Sleep không được hỗ trợ trong phiên bản .NET hiện đại.',
 'Task.Delay tự động lưu toàn bộ dữ liệu form vào cơ sở dữ liệu.',
 'Task.Delay giải phóng luồng giao diện để ứng dụng không bị treo (đơ giao diện) trong thời gian chờ.',
 'Thread.Sleep sẽ tự động đóng ứng dụng ngay lập tức.', 'C'),

(3, 'Medium', 'Trong LINQ, cơ chế "thực thi trì hoãn" (Deferred Execution) có ý nghĩa như thế nào đối với các phương thức như Where(), Select()?',
 'Câu lệnh chỉ thực thi duy nhất một lần khi ứng dụng khởi động.',
 'Dữ liệu được tính toán ngay lập tức tại thời điểm viết dòng lệnh LINQ.',
 'Kết quả truy vấn luôn được ghi thẳng vào đĩa cứng.',
 'Truy vấn chỉ thực sự được thực thi khi duyệt qua các phần tử (ví dụ qua vòng lặp foreach hoặc gọi ToList()).', 'D'),

(3, 'Hard', 'Trong lập trình bất đồng bộ .NET, việc gọi thuộc tính .Result hoặc phương thức .Wait() trực tiếp trên một Task đang chạy từ UI Thread có thể dẫn tới hậu quả nghiêm trọng nào?',
 'Gây ra hiện tượng khóa chết (Deadlock) làm ứng dụng bị treo hoàn toàn.',
 'Làm tràn bộ nhớ Stack (StackOverflowException).',
 'Làm mất toàn bộ dữ liệu trong bảng SQLite.',
 'Tự động khởi động lại hệ điều hành Windows.', 'A'),

(3, 'Hard', 'Phương thức LINQ nào sau đây cho phép chuyển đổi tập hợp các phần tử thành một cấu trúc Lookup nhóm theo khóa mà vẫn giữ nguyên tính chất chỉ đọc?',
 'ToList()', 'ToLookup()', 'AsParallel()', 'SingleOrDefault()', 'B'),

-- Chương 4: Truy cập dữ liệu với ADO.NET và SQLite (6 câu)
(4, 'Easy', 'Khi làm việc với ADO.NET trong C#, từ khóa nào được khuyến nghị bao quanh các đối tượng SqliteConnection, SqliteCommand để tự động giải phóng tài nguyên sau khi sử dụng?',
 'lock', 'checked', 'using', 'fixed', 'C'),

(4, 'Easy', 'Phương thức nào của đối tượng SqliteCommand được sử dụng để thực thi các câu lệnh INSERT, UPDATE, DELETE và trả về số dòng bị ảnh hưởng?',
 'ExecuteReader()', 'ExecuteScalar()', 'ExecuteXmlReader()', 'ExecuteNonQuery()', 'D'),

(4, 'Medium', 'Để lấy về một giá trị đơn lẻ đầu tiên của dòng đầu tiên trong kết quả truy vấn (ví dụ SELECT COUNT(*) hoặc SELECT last_insert_rowid()), phương thức nào là tối ưu nhất?',
 'ExecuteScalar()', 'ExecuteReader()', 'ExecuteNonQuery()', 'FillSchema()', 'A'),

(4, 'Medium', 'Để ngăn chặn triệt để lỗ hổng bảo mật SQL Injection khi truy vấn theo từ khóa người dùng nhập vào, giải pháp chuẩn xác nhất là gì?',
 'Ghép chuỗi trực tiếp bằng toán tử + hoặc nội suy chuỗi.',
 'Sử dụng câu lệnh SQL có tham số (Parameterized Query) thông qua Parameters.AddWithValue.',
 'Chỉ kiểm tra độ dài chuỗi ký tự trên TextBox giao diện.',
 'Chuyển toàn bộ câu lệnh SQL sang chữ in hoa.', 'B'),

(4, 'Hard', 'Khi thực hiện chuỗi thao tác thêm đề thi và thêm danh sách câu hỏi chi tiết của đề thi vào cơ sở dữ liệu SQLite, cơ chế nào đảm bảo toàn bộ thành công hoặc nếu có lỗi thì hoàn tác sạch sẽ?',
 'Sử dụng nhiều khối try...catch lồng nhau mà không cần kết nối chung.',
 'Lưu dữ liệu vào biến static của ứng dụng.',
 'Sử dụng SqliteTransaction (BeginTransaction, Commit và Rollback) trên cùng một kết nối mở.',
 'Tắt hoàn toàn cờ ForeignKeys trong chuỗi kết nối SQLite.', 'C'),

(4, 'Hard', 'Trong tệp cấu hình dự án .NET (csproj), cách cấu hình nào giúp nhúng tệp script SQL (như schema.sql, seed_questions.sql) vào trực tiếp tệp assembly .dll của ứng dụng?',
 'Khai báo thẻ <None Include="..." CopyToOutputDirectory="Never" />',
 'Đặt thuộc tính Build Action là Page trong Visual Studio.',
 'Nén tệp SQL thành định dạng .zip rồi đặt cùng thư mục cài đặt.',
 'Khai báo thẻ <EmbeddedResource Include="..." LogicalName="..." /> trong ItemGroup.', 'D')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_LTNET q
JOIN Subjects s ON s.Code = 'LTNET'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);

-- 3. CÂU HỎI MÔN CSDL: CƠ SỞ DỮ LIỆU (24 CÂU, CHƯƠNG 1–4)
WITH Data_CSDL(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Mô hình cơ sở dữ liệu quan hệ (6 câu)
(1, 'Easy', 'Trong mô hình cơ sở dữ liệu quan hệ, khóa chính (Primary Key) của một bảng bắt buộc phải thỏa mãn những tính chất nào?',
 'Có giá trị duy nhất (Unique) trên mỗi dòng và không được phép chứa giá trị rỗng (NOT NULL).',
 'Phải mang kiểu dữ liệu số nguyên tự tăng và có tiền tố PK_.',
 'Luôn luôn phải tham chiếu trực tiếp đến một bảng khác trong cùng CSDL.',
 'Có thể chứa nhiều dòng mang giá trị NULL nếu các cột còn lại khác nhau.', 'A'),

(1, 'Easy', 'Ràng buộc khóa ngoại (Foreign Key) giữa bảng Questions và bảng Subjects nhằm đảm bảo nguyên tắc toàn vẹn nào trong CSDL?',
 'Toàn vẹn thực thể (Entity Integrity)',
 'Toàn vẹn tham chiếu (Referential Integrity)',
 'Toàn vẹn ngữ nghĩa người dùng (Domain Integrity)',
 'Toàn vẹn giao dịch (Transaction Integrity)', 'B'),

(1, 'Medium', 'Trong bảng Subjects, việc đặt chỉ mục duy nhất (UNIQUE) trên cột Code (Mã môn học) có mục đích chính là gì?',
 'Cho phép nhập nhiều môn học trùng mã nhưng khác tên.',
 'Tự động đổi chữ thường thành chữ in hoa khi lưu vào bảng.',
 'Đảm bảo không thể có hai môn học nào có mã trùng nhau trong toàn hệ thống.',
 'Xóa tự động các câu hỏi thuộc môn học cũ khi tạo môn học mới.', 'C'),

(1, 'Medium', 'Trong thiết kế CSDL hệ thống thi trắc nghiệm, vì sao không nên xóa cứng một câu hỏi khi câu hỏi đó đã được gán vào đề thi của các ca thi trước?',
 'Vì SQLite không hỗ trợ cú pháp lệnh DELETE khi có chỉ mục.',
 'Vì thao tác xóa câu hỏi sẽ làm thay đổi mật khẩu của tài khoản quản trị.',
 'Vì việc xóa câu hỏi sẽ làm giảm kích thước tệp cơ sở dữ liệu trên đĩa.',
 'Để bảo toàn lịch sử đề thi, kết quả chấm thi và phục vụ việc tra cứu, phúc khảo của thí sinh.', 'D'),

(1, 'Hard', 'Hành vi nào xảy ra khi thực hiện lệnh DELETE trên bảng cha (ví dụ Subjects) nếu khóa ngoại ở bảng con (Questions) được thiết lập với tùy chọn ON DELETE RESTRICT trong SQLite?',
 'Hệ thống từ chối xóa dòng ở bảng cha và báo lỗi vi phạm ràng buộc khóa ngoại nếu đang tồn tại dòng con tham chiếu tới.',
 'Hệ thống tự động xóa tất cả các dòng ở bảng con có liên quan rồi mới xóa dòng ở bảng cha.',
 'Hệ thống tự động gán giá trị NULL cho cột khóa ngoại ở tất cả các dòng con.',
 'Dòng ở bảng cha bị xóa bình thường, để lại các dòng con không có cha (mồ côi).', 'A'),

(1, 'Hard', 'Trong SQLite, mệnh đề PRAGMA foreign_keys = ON; có vai trò quan trọng như thế nào đối với ứng dụng?',
 'Tự động tạo các bảng mới theo quy ước đặt tên của C#.',
 'Kích hoạt cơ chế kiểm tra và thực thi các ràng buộc khóa ngoại (Foreign Key Constraints) vốn mặc định bị tắt trong SQLite.',
 'Tăng tốc độ đọc dữ liệu từ tệp cơ sở dữ liệu lên gấp đôi.',
 'Mã hóa tệp cơ sở dữ liệu bằng thuật toán AES-256.', 'B'),

-- Chương 2: Ngôn ngữ truy vấn có cấu trúc - SQL (6 câu)
(2, 'Easy', 'Mệnh đề nào trong câu lệnh SELECT của SQL được sử dụng để sắp xếp thứ tự kết quả trả về theo chiều tăng dần hoặc giảm dần?',
 'GROUP BY', 'WHERE', 'ORDER BY', 'HAVING', 'C'),

(2, 'Easy', 'Ký tự đại diện nào trong phép toán so sánh mẫu LIKE của SQL biểu diễn cho một chuỗi gồm không hoặc nhiều ký tự bất kỳ?',
 'Ký tự dấu hỏi chấm (?)', 'Ký tự dấu hoa thị (*)', 'Ký tự dấu gạch dưới (_)', 'Ký tự dấu phần trăm (%)', 'D'),

(2, 'Medium', 'Để lấy danh sách tất cả môn học cùng số lượng câu hỏi hiện có của từng môn (kể cả những môn mới tạo chưa có câu hỏi nào, hiển thị số lượng câu = 0), phép nối nào cần sử dụng?',
 'LEFT JOIN từ Subjects sang Questions kết hợp với hàm COUNT(Questions.Id) và GROUP BY Subjects.Id.',
 'INNER JOIN giữa Subjects và Questions kết hợp GROUP BY.',
 'CROSS JOIN giữa hai bảng mà không cần điều kiện kết nối.',
 'RIGHT JOIN từ Questions sang Subjects mà không cần gom nhóm.', 'A'),

(2, 'Medium', 'Mệnh đề HAVING trong câu lệnh SQL khác với mệnh đề WHERE ở điểm mấu chốt nào?',
 'WHERE chỉ dùng cho câu lệnh UPDATE, còn HAVING chỉ dùng cho SELECT.',
 'WHERE lọc dữ liệu trên từng dòng đơn lẻ trước khi gom nhóm, còn HAVING lọc kết quả sau khi đã áp dụng hàm tổng hợp và GROUP BY.',
 'HAVING không thể sử dụng kết hợp với câu lệnh có chứa GROUP BY.',
 'WHERE chỉ lọc được các cột có kiểu dữ liệu chuỗi ký tự.', 'B'),

(2, 'Hard', 'Cho truy vấn: SELECT SubjectId, COUNT(*) AS Total FROM Questions GROUP BY SubjectId HAVING Total > 10; Truy vấn này trả về kết quả gì?',
 'Danh sách tất cả câu hỏi của 10 môn học đầu tiên trong cơ sở dữ liệu.',
 'Tổng số câu hỏi của các môn học có SubjectId lớn hơn 10.',
 'Danh sách các mã định danh môn học (SubjectId) đang có nhiều hơn 10 câu hỏi trong ngân hàng câu hỏi.',
 'Số lượng câu hỏi trung bình của mỗi chương trong từng môn học.', 'C'),

(2, 'Hard', 'Cú pháp INSERT INTO ... SELECT ... WHERE NOT EXISTS (...) mang lại lợi ích gì đặc biệt khi nạp dữ liệu mẫu vào cơ sở dữ liệu?',
 'Tự động xóa sạch dữ liệu cũ trước khi nạp dữ liệu mới.',
 'Tăng tốc độ ghi dữ liệu bằng cách tắt cơ chế ghi nhật ký phục hồi (WAL).',
 'Cho phép chèn dữ liệu mà không cần quan tâm đến kiểu dữ liệu của các cột.',
 'Đảm bảo tính chất idempotent (không sinh ra dữ liệu trùng lặp ngay cả khi chạy lại script nạp dữ liệu nhiều lần).', 'D'),

-- Chương 3: Chuẩn hóa dữ liệu và giao dịch - Transaction (6 câu)
(3, 'Easy', 'Một bảng trong cơ sở dữ liệu quan hệ được coi là đạt Dạng chuẩn 1 (1NF) khi thỏa mãn điều kiện tiên quyết nào?',
 'Mọi thuộc tính (cột) đều chứa giá trị nguyên tử (nguyên tố, không phân chia được nữa) và không chứa nhóm lặp.',
 'Bảng phải có ít nhất hai khóa ngoại tham chiếu sang hai bảng khác nhau.',
 'Tất cả các cột dữ liệu đều phải mang kiểu số nguyên (INTEGER).',
 'Khóa chính của bảng bắt buộc phải là khóa tổ hợp gồm từ hai cột trở lên.', 'A'),

(3, 'Easy', 'Trong bốn thuộc tính ACID của Giao dịch (Transaction), chữ viết tắt "A" đại diện cho thuộc tính nào?',
 'Availability (Tính khả dụng)',
 'Atomicity (Tính nguyên tố - Tất cả hoặc không có gì)',
 'Accuracy (Tính chính xác)',
 'Authentication (Tính xác thực)', 'B'),

(3, 'Medium', 'Một bảng đạt Dạng chuẩn 2 (2NF) khi nó đã thỏa mãn Dạng chuẩn 1 (1NF) và thỏa mãn thêm điều kiện nào sau đây?',
 'Mọi cột trong bảng đều phải có chỉ mục duy nhất (UNIQUE Index).',
 'Không có bất kỳ dòng nào chứa giá trị rỗng (NULL).',
 'Mọi thuộc tính không khóa đều phụ thuộc hàm đầy đủ vào khóa chính (không phụ thuộc vào một phần của khóa chính tổ hợp).',
 'Tên các bảng phải tuân theo chuẩn quy ước chữ thường.', 'C'),

(3, 'Medium', 'Thuộc tính nào trong bộ nguyên tắc ACID đảm bảo rằng các thay đổi dữ liệu sau khi đã Commit thành công sẽ không bị mất kể cả khi hệ thống gặp sự cố mất điện đột ngột?',
 'Atomicity', 'Consistency', 'Isolation', 'Durability (Tính bền vững)', 'D'),

(3, 'Hard', 'Hiện tượng "Dirty Read" (Đọc dữ liệu bẩn) trong hệ thống cơ sở dữ liệu xảy ra khi nào?',
 'Khi một giao dịch đọc phải dữ liệu đã bị sửa đổi bởi một giao dịch khác nhưng giao dịch sửa đổi đó chưa được Commit.',
 'Khi hai giao dịch cùng thực hiện ghi đè lên cùng một dòng dữ liệu tại cùng một thời điểm.',
 'Khi câu truy vấn đọc dữ liệu từ một bảng không có khóa chính.',
 'Khi tệp cơ sở dữ liệu bị hỏng do phần cứng lưu trữ gặp lỗi vật lý.', 'A'),

(3, 'Hard', 'Giả sử bảng Questions lưu trực tiếp tên môn học (SubjectName) lặp đi lặp lại trên từng dòng câu hỏi thay vì lưu SubjectId tham chiếu tới bảng Subjects. Thiết kế này vi phạm dạng chuẩn nào và gây ra dị thường gì?',
 'Vi phạm 1NF và làm mất toàn bộ khóa chính của bảng.',
 'Vi phạm chuẩn hóa gây dị thường cập nhật (Update Anomaly): khi sửa tên một môn học phải cập nhật hàng nghìn câu hỏi liên quan.',
 'Tự động làm hỏng cấu trúc tệp cơ sở dữ liệu SQLite.',
 'Làm cho câu lệnh SELECT không thể thực thi được trên bảng Questions.', 'B'),

-- Chương 4: Chỉ mục, tối ưu hóa và an toàn dữ liệu (6 câu)
(4, 'Easy', 'Cấu trúc dữ liệu phổ biến nhất được các hệ quản trị cơ sở dữ liệu (bao gồm cả SQLite) sử dụng để hiện thực hóa Chỉ mục (Index) là gì?',
 'Hàng đợi (Queue)',
 'Danh sách liên kết đơn (Singly Linked List)',
 'Cây B-Tree (hoặc B+Tree)',
 'Bảng băm một chiều (Hash Table đơn giản)', 'C'),

(4, 'Easy', 'Câu lệnh SQL nào dùng để tạo chỉ mục trên cột SubjectId và Chapter của bảng Questions nhằm tối ưu hóa bộ lọc ngân hàng câu hỏi?',
 'ADD INDEX idx_q_sub ON Questions(SubjectId, Chapter);',
 'INSERT INDEX idx_q_sub INTO Questions(SubjectId, Chapter);',
 'ALTER TABLE Questions CREATE INDEX(SubjectId, Chapter);',
 'CREATE INDEX IF NOT EXISTS idx_questions_sub_chap ON Questions(SubjectId, Chapter);', 'D'),

(4, 'Medium', 'Mặc dù chỉ mục (Index) giúp tăng tốc độ đáng kể cho các câu lệnh truy vấn tìm kiếm SELECT, nhược điểm chính của việc lạm dụng quá nhiều chỉ mục là gì?',
 'Làm chậm tốc độ của các thao tác ghi (INSERT, UPDATE, DELETE) và tiêu tốn thêm dung lượng lưu trữ trên đĩa.',
 'Làm sai lệch kết quả của các hàm tính toán tổng hợp như SUM, COUNT.',
 'Khiến cho các ràng buộc khóa ngoại bị vô hiệu hóa.',
 'Không cho phép sử dụng câu lệnh SELECT có chứa mệnh đề WHERE nữa.', 'A'),

(4, 'Medium', 'Lệnh EXPLAIN QUERY PLAN trong SQLite cung cấp thông tin giá trị nào cho lập trình viên khi tối ưu hóa hiệu năng ứng dụng?',
 'Hiển thị mật khẩu đã được mã hóa của người dùng hiện tại.',
 'Mô tả chi tiết chiến lược thực thi truy vấn của SQLite (quét toàn bộ bảng SCAN TABLE hay sử dụng chỉ mục SEARCH TABLE USING INDEX).',
 'Tự động viết lại mã C# của tầng Repository sang ngôn ngữ LINQ.',
 'Đếm số lượng kết nối đang mở tới tệp cơ sở dữ liệu.', 'B'),

(4, 'Hard', 'Khi sao lưu (backup) cơ sở dữ liệu SQLite trong ứng dụng desktop, giải pháp nào đảm bảo dữ liệu sao lưu toàn vẹn ngay cả khi ứng dụng đang chạy?',
 'Dùng lệnh sao chép tệp File.Copy trực tiếp mà không cần quan tâm đến trạng thái của kết nối.',
 'Tắt máy tính bằng cách ngắt nguồn điện rồi tháo ổ đĩa sao chép tệp .db sang máy khác.',
 'Sử dụng SQLite Online Backup API (hoặc đảm bảo mọi transaction ghi đã được commit và đồng bộ qua VACUUM INTO).',
 'Đổi tên bảng trong tệp CSDL thành chữ hoa trước khi copy.', 'C'),

(4, 'Hard', 'Câu lệnh SQLite PRAGMA integrity_check có chức năng gì trong việc bảo trì và kiểm tra sức khỏe của cơ sở dữ liệu?',
 'Tự động thêm dữ liệu mẫu vào tất cả các bảng rỗng.',
 'Nâng cấp phiên bản SQLite từ 3 lên phiên bản cao hơn mà không cần cài đặt lại.',
 'Xóa vĩnh viễn toàn bộ các bản ghi bị đánh dấu là trùng lặp.',
 'Quét toàn bộ tệp cơ sở dữ liệu để phát hiện các trang dữ liệu bị hỏng, lỗi cấu trúc B-Tree hoặc sai lệch chỉ mục.', 'D')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_CSDL q
JOIN Subjects s ON s.Code = 'CSDL'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);

-- 4. CÂU HỎI MÔN MMT: MẠNG MÁY TÍNH (24 CÂU, CHƯƠNG 1–4)
WITH Data_MMT(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Tổng quan mạng máy tính và mô hình OSI (6 câu)
(1, 'Easy', 'Mô hình tham chiếu kết nối các hệ thống mở (OSI) do tổ chức ISO ban hành bao gồm bao nhiêu tầng chức năng?',
 '7 tầng', '4 tầng', '5 tầng', '8 tầng', 'A'),

(1, 'Easy', 'Đơn vị dữ liệu giao thức (PDU) tại tầng Liên kết dữ liệu (Data Link Layer - Tầng 2) được gọi là gì?',
 'Bit', 'Frame (Khung tin)', 'Packet (Gói tin)', 'Segment (Đoạn tin)', 'B'),

(1, 'Medium', 'Trong mô hình OSI, tầng nào chịu trách nhiệm mã hóa dữ liệu, nén dữ liệu và định dạng cú pháp dữ liệu (ví dụ JSON, XML, JPEG) trước khi chuyển lên tầng ứng dụng?',
 'Tầng Mạng (Network Layer)',
 'Tầng Phiên (Session Layer)',
 'Tầng Trình diễn (Presentation Layer)',
 'Tầng Giao vận (Transport Layer)', 'C'),

(1, 'Medium', 'Thiết bị mạng Router (Bộ định tuyến) hoạt động chủ yếu ở tầng nào của mô hình OSI để chuyển tiếp các gói tin dựa trên địa chỉ IP đích?',
 'Tầng Vật lý (Physical Layer)',
 'Tầng Liên kết dữ liệu (Data Link Layer)',
 'Tầng Giao vận (Transport Layer)',
 'Tầng Mạng (Network Layer)', 'D'),

(1, 'Hard', 'Quá trình đóng gói dữ liệu (Encapsulation) khi dữ liệu truyền từ tầng Ứng dụng xuống tầng Vật lý trong ngăn xếp giao thức mạng diễn ra theo trình tự nào?',
 'Dữ liệu (Data) → Đoạn tin (Segment) → Gói tin (Packet) → Khung tin (Frame) → Dãy bit nhị phân.',
 'Dữ liệu → Khung tin → Đoạn tin → Gói tin → Dãy bit nhị phân.',
 'Dãy bit nhị phân → Gói tin → Khung tin → Đoạn tin → Dữ liệu.',
 'Gói tin → Đoạn tin → Dữ liệu → Khung tin → Dãy bit nhị phân.', 'A'),

(1, 'Hard', 'Trong kiến trúc mạng LAN phòng thi trắc nghiệm, vì sao việc sử dụng Switch lại tối ưu hơn việc dùng Hub truyền thống?',
 'Hub tự động mã hóa gói tin bằng thuật toán SSL, còn Switch thì không.',
 'Switch phân chia miền đụng độ (Collision Domain) độc lập cho từng cổng dựa trên bảng địa chỉ MAC, giúp giảm thiểu tắc nghẽn và xung đột dữ liệu.',
 'Hub có khả năng định tuyến gói tin qua mạng Internet, còn Switch chỉ hoạt động nội bộ.',
 'Switch không cần cấp nguồn điện khi hoạt động trong mạng phòng máy.', 'B'),

-- Chương 2: Tầng ứng dụng và các giao thức mạng (6 câu)
(2, 'Easy', 'Giao thức nào ở tầng Ứng dụng chịu trách nhiệm phân giải tên miền dạng ký tự (ví dụ uniexam.edu.vn) thành địa chỉ IP tương ứng của máy chủ?',
 'DHCP', 'FTP', 'DNS (Domain Name System)', 'SNMP', 'C'),

(2, 'Easy', 'Cặp số hiệu cổng mặc định (Default Port) được sử dụng cho giao thức truyền tải siêu văn bản bảo mật HTTPS và giao thức gửi thư điện tử SMTP lần lượt là gì?',
 '80 và 21', '22 và 23', '53 và 110', '443 và 25', 'D'),

(2, 'Medium', 'Giao thức DHCP (Dynamic Host Configuration Protocol) thực hiện việc cấp phát địa chỉ IP tự động cho máy trạm thông qua chu trình 4 thông điệp theo thứ tự nào?',
 'DORA: Discover → Offer → Request → Acknowledge.',
 'ARPA: Ask → Reply → Provide → Accept.',
 'SYNC: Start → Yield → Network → Confirm.',
 'POST: Ping → Open → Send → Terminate.', 'A'),

(2, 'Medium', 'Phương thức HTTP nào được khuyến nghị sử dụng khi gửi dữ liệu nhạy cảm (như thông tin đăng nhập tài khoản hoặc bài thi đã hoàn thành của sinh viên) từ client lên server?',
 'GET', 'POST', 'OPTIONS', 'TRACE', 'B'),

(2, 'Hard', 'Trong các mã trạng thái phản hồi HTTP (HTTP Status Code), mã phản hồi 403 Forbidden mang ý nghĩa gì?',
 'Máy chủ không tìm thấy tài nguyên theo đường dẫn URI được yêu cầu.',
 'Yêu cầu thành công và dữ liệu được trả về trong phần thân thông điệp.',
 'Máy chủ đã hiểu yêu cầu nhưng từ chối thực hiện do máy khách không có đủ quyền hạn truy cập tài nguyên.',
 'Máy chủ gặp lỗi nội bộ không mong muốn khi xử lý yêu cầu.', 'C'),

(2, 'Hard', 'Khi xây dựng ứng dụng giám sát thi trực tuyến thời gian thực giữa giảng viên và hàng trăm máy thi của sinh viên, giao thức nào sau đây cho phép giao tiếp hai chiều liên tục (Full-duplex) với độ trễ thấp trên một kết nối TCP duy nhất?',
 'FTP truyền tệp chủ động.',
 'POP3 nhận thư định kỳ.',
 'HTTP/1.0 với cơ chế đóng kết nối sau mỗi truy vấn.',
 'WebSocket (kết nối ws:// hoặc wss://).', 'D'),

-- Chương 3: Tầng giao vận - TCP/UDP (6 câu)
(3, 'Easy', 'Giao thức nào tại tầng Giao vận cung cấp dịch vụ truyền dữ liệu hướng kết nối (Connection-oriented), đảm bảo dữ liệu đến đích tin cậy và đúng thứ tự?',
 'TCP (Transmission Control Protocol)',
 'UDP (User Datagram Protocol)',
 'IP (Internet Protocol)',
 'ICMP (Internet Control Message Protocol)', 'A'),

(3, 'Easy', 'Giao thức UDP (User Datagram Protocol) thường được ưu tiên lựa chọn sử dụng cho loại hình ứng dụng nào sau đây?',
 'Ứng dụng truyền tệp ngân hàng yêu cầu tuyệt đối không mất dữ liệu.',
 'Ứng dụng truyền phát âm thanh/hình ảnh trực tiếp (Live Streaming), VoIP hoặc trò chơi trực tuyến ưu tiên tốc độ và độ trễ thấp.',
 'Hệ thống xác thực mật khẩu người dùng qua web.',
 'Ứng dụng gửi email quan trọng của doanh nghiệp.', 'B'),

(3, 'Medium', 'Trong quá trình bắt tay 3 bước (Three-way Handshake) để thiết lập kết nối TCP, trình tự các gói tin điều khiển được gửi giữa Client và Server là gì?',
 'Client gửi ACK → Server gửi SYN → Client gửi FIN.',
 'Server gửi SYN → Client gửi RST → Server gửi ACK.',
 'Client gửi SYN → Server gửi SYN-ACK → Client gửi ACK.',
 'Client gửi FIN → Server gửi ACK → Server gửi FIN-ACK.', 'C'),

(3, 'Medium', 'Kỹ thuật Cửa sổ trượt (Sliding Window) trong giao thức TCP được thiết kế nhằm mục đích chính nào?',
 'Thay đổi địa chỉ IP của máy gửi theo chu kỳ.',
 'Tự động mã hóa nội dung các gói tin truyền qua mạng công cộng.',
 'Đóng kết nối mạng ngay khi phát hiện có độ trễ cao.',
 'Kiểm soát luồng (Flow Control) nhằm ngăn không cho bên gửi truyền dữ liệu quá nhanh khiến bên nhận bị quá tải bộ đệm.', 'D'),

(3, 'Hard', 'Khi một kết nối TCP đang hoạt động gặp sự cố mất gói tin do mạng bị tắc nghẽn, thuật toán Kiểm soát tắc nghẽn (Congestion Control) của TCP sẽ phản ứng như thế nào?',
 'Giảm kích thước cửa sổ tắc nghẽn (Congestion Window), quay về pha Slow Start hoặc Congestion Avoidance và truyền lại gói tin bị mất.',
 'Tự động chuyển toàn bộ dữ liệu còn lại sang sử dụng giao thức UDP.',
 'Gửi liên tục các gói tin RST để ngắt toàn bộ các máy tính trong mạng LAN.',
 'Tăng gấp đôi tốc độ phát gói tin để bù đắp lượng dữ liệu bị thất thoát.', 'A'),

(3, 'Hard', 'Cổng (Port) tại tầng Giao vận kết hợp với địa chỉ IP tại tầng Mạng tạo thành khái niệm cơ bản nào trong lập trình mạng socket?',
 'MAC Address',
 'Socket Address (Điểm cuối kết nối mạng)',
 'Subnet Mask',
 'Default Gateway', 'B'),

-- Chương 4: Địa chỉ IPv4 và chia mạng con - Subnetting (6 câu)
(4, 'Easy', 'Một địa chỉ IPv4 tiêu chuẩn có tổng chiều dài là bao nhiêu bit và được biểu diễn bằng bao nhiêu số thập phân ngăn cách bởi dấu chấm?',
 '16 bit, chia thành 2 phần.',
 '64 bit, chia thành 8 phần.',
 '32 bit, chia thành 4 octet (mỗi octet 8 bit).',
 '128 bit, chia thành 8 nhóm hexa.', 'C'),

(4, 'Easy', 'Dải địa chỉ IP nào dưới đây thuộc khối địa chỉ IP riêng (Private IP Address) dùng tự do trong mạng nội bộ theo tiêu chuẩn RFC 1918?',
 '8.8.8.0/24', '1.1.1.0/24', '203.162.0.0/16', '192.168.1.0/24', 'D'),

(4, 'Medium', 'Với một dải mạng có địa chỉ 192.168.10.0 và mặt nạ mạng con Subnet Mask là 255.255.255.0 (/24), số lượng địa chỉ IP tối đa có thể gán cho các máy tính (Host) là bao nhiêu?',
 '254 địa chỉ (loại trừ địa chỉ mạng .0 và địa chỉ quảng bá .255)',
 '256 địa chỉ',
 '128 địa chỉ',
 '512 địa chỉ', 'A'),

(4, 'Medium', 'Địa chỉ IP nào sau đây là địa chỉ vòng lặp cục bộ (Loopback Address) dùng để máy tính tự kiểm tra ngăn xếp TCP/IP của chính mình?',
 '0.0.0.0', '127.0.0.1', '255.255.255.255', '192.168.0.1', 'B'),

(4, 'Hard', 'Quản trị viên cần chia mạng 192.168.1.0/24 thành 4 mạng con (Subnet) bằng nhau cho 4 phòng thi. Mặt nạ mạng con mới (Subnet Mask) theo ký hiệu CIDR và số lượng địa chỉ Host khả dụng trên mỗi phòng thi là bao nhiêu?',
 '25 (255.255.255.128) với 126 host khả dụng mỗi phòng thi.',
 '/27 (255.255.255.224) với 30 host khả dụng mỗi phòng thi.',
 '/26 (255.255.255.192) với 62 host khả dụng mỗi phòng thi.',
 '/28 (255.255.255.240) với 14 host khả dụng mỗi phòng thi.', 'C'),

(4, 'Hard', 'Khi một máy tính trong mạng LAN gửi gói tin ra một máy chủ trên Internet, kỹ thuật nào tại Router giúp chuyển đổi địa chỉ IP nội bộ (Private IP) thành một địa chỉ IP công cộng (Public IP)?',
 'ARP (Address Resolution Protocol)',
 'ICMP Redirect',
 'VLAN Trunking (802.1Q)',
 'NAT (Network Address Translation) / PAT', 'D')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_MMT q
JOIN Subjects s ON s.Code = 'MMT'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);
