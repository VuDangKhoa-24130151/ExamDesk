-- SEED DỮ LIỆU MẪU CHO NGÂN HÀNG CÂU HỎI & MÔN HỌC
-- Bao gồm 3 môn: LTNET (Lập trình .NET), CSDL (Cơ sở dữ liệu), MMT (Mạng máy tính)
-- Mỗi môn 48 câu hỏi mẫu (16 Dễ, 20 Trung bình, 12 Khó) phân bổ đều theo 4 Chương (mỗi chương 12 câu: 4 Dễ, 5 Trung bình, 3 Khó)
-- và đáp án đúng phân bố đều A/B/C/D.
-- Cú pháp an toàn (idempotent), không gây trùng lặp dữ liệu khi nạp nhiều lần.

PRAGMA foreign_keys = ON;

-- 1. TẠO CÁC MÔN HỌC NẾU CHƯA TỒN TẠI
INSERT INTO Subjects (Code, Name) VALUES ('LTNET', 'Lập trình .NET') ON CONFLICT(Code) DO NOTHING;
INSERT INTO Subjects (Code, Name) VALUES ('CSDL', 'Cơ sở dữ liệu') ON CONFLICT(Code) DO NOTHING;
INSERT INTO Subjects (Code, Name) VALUES ('MMT', 'Mạng máy tính') ON CONFLICT(Code) DO NOTHING;

-- 2. CÂU HỎI MÔN LTNET: LẬP TRÌNH .NET (48 CÂU, CHƯƠNG 1–4)
WITH Data_LTNET(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Nền tảng .NET và ngôn ngữ C# (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(1, 'Easy', 'Trong ngôn ngữ C#, từ khóa nào được sử dụng để khai báo một hằng số mà giá trị phải được xác định ngay tại thời điểm biên dịch (compile-time)?',
 'readonly', 'static', 'const', 'volatile', 'C'),

(1, 'Easy', 'Trong C#, kiểu dữ liệu dựng sẵn nào phù hợp nhất để lưu trữ các giá trị tài chính, tiền tệ đòi hỏi độ chính xác thập phân cao và tránh sai số làm tròn nhị phân?',
 'float', 'double', 'long', 'decimal', 'D'),

(1, 'Medium', 'Trong C#, sự khác biệt chính giữa từ khóa ref và từ khóa out khi truyền tham số theo tham chiếu vào phương thức là gì?',
 'Tham số ref bắt buộc biến phải được khởi tạo giá trị trước khi truyền, còn out không yêu cầu khởi tạo trước nhưng phương thức được gọi phải gán giá trị cho nó trước khi kết thúc.',
 'Tham số out chỉ dùng được cho kiểu chuỗi string, còn ref chỉ dùng được cho kiểu số nguyên int.',
 'Tham số ref tự động tạo luồng xử lý mới, còn out chạy tuần tự trên luồng chính.',
 'Phương thức sử dụng ref không thể trả về kiểu void, còn out chỉ dùng trong hàm khởi tạo.', 'A'),

(1, 'Medium', 'Trong C#, giao diện IDisposable định nghĩa phương thức nào để giải phóng tường minh các tài nguyên không được quản lý (unmanaged resources) như kết nối CSDL, tệp tin hoặc luồng mạng?',
 'Finalize()', 'Dispose()', 'Destruct()', 'ReleaseHandle()', 'B'),

(1, 'Medium', 'Tính năng Nullable Reference Types (ví dụ string?) trong C# hiện đại giúp ích gì cho quá trình phát triển phần mềm?',
 'Cho phép biến kiểu số nguyên int nhận trực tiếp chuỗi rỗng mà không cần ép kiểu.',
 'Tự động chuyển đổi mọi ngoại lệ NullReferenceException thành giá trị số 0.',
 'Giúp trình biên dịch phân tích luồng dữ liệu và phát cảnh báo sớm khi biến tham chiếu có khả năng bị null trước lúc truy cập.',
 'Loại bỏ hoàn toàn nhu cầu cấp phát bộ nhớ trên Heap cho đối tượng.', 'C'),

(1, 'Hard', 'Xét kiểu record trong C# 9.0 trở lên (ví dụ public sealed record StudentDto(string Code, string Name);), đặc điểm mặc định nào khiến record khác biệt với class thông thường?',
 'Record luôn được cấp phát toàn bộ trên bộ nhớ Stack giống hệt kiểu int và không hỗ trợ kế thừa.',
 'Record không thể chứa phương thức hoặc thuộc tính tính toán bên trong thân khai báo.',
 'Record tự động bỏ qua cơ chế thu gom rác (Garbage Collector) của CLR.',
 'Record tự động sinh logic so sánh bình đẳng theo giá trị (value-based equality) thông qua Equals/GetHashCode và hỗ trợ biểu thức with để tạo bản sao bất biến.', 'D'),

-- Chương 2: Thiết kế giao diện với WinForms (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(2, 'Easy', 'Trong WinForms, phương thức nào của lớp Form được gọi để hiển thị một hộp thoại dạng Modal (chặn tương tác với cửa sổ cha cho đến khi hộp thoại được đóng lại)?',
 'ShowDialog()', 'Show()', 'Activate()', 'Focus()', 'A'),

(2, 'Easy', 'Khi muốn một điều khiển con (như TableLayoutPanel hoặc DataGridView) tự động lấp đầy toàn bộ không gian khả dụng bên trong vùng chứa cha của nó, thuộc tính Dock nên đặt giá trị nào?',
 'DockStyle.None', 'DockStyle.Fill', 'DockStyle.Top', 'DockStyle.Bottom', 'B'),

(2, 'Medium', 'Trong WinForms, khi gọi phương thức Form.ShowDialog(), nếu hộp thoại được đóng thông qua việc gán DialogResult = DialogResult.OK, lưu ý quan trọng nào về vòng đời tài nguyên cần thực hiện ở mã gọi?',
 'Form tự động bị hủy ngay lập tức nên không cần quan tâm đến việc giải phóng tài nguyên.',
 'Phải gọi Application.Restart() ngay sau khi hộp thoại đóng.',
 'Form hiển thị bằng ShowDialog() không tự động gọi Dispose() khi đóng, do đó mã gọi cần bọc đối tượng Form trong khối using.',
 'Phải gọi ShowDialog() thêm một lần nữa để xóa dữ liệu trên các TextBox.', 'C'),

(2, 'Medium', 'Sự kiện FormClosing trong WinForms cho phép lập trình viên ngăn không cho cửa sổ đóng lại (ví dụ khi người dùng chọn Không hủy thay đổi chưa lưu) bằng cách thao tác thuộc tính nào của FormClosingEventArgs?',
 'e.Handled = true;', 'e.SuppressKeyPress = true;', 'e.CloseReason = CloseReason.None;', 'e.Cancel = true;', 'D'),

(2, 'Medium', 'Trong WinForms, để ngăn rò rỉ khóa tệp ảnh trên đĩa và giải phóng bộ nhớ GDI+ khi hiển thị nhiều ảnh câu hỏi liên tiếp trong PictureBox, cách xử lý đúng nhất là gì?',
 'Đọc tệp ảnh vào MemoryStream (hoặc sao chép sang đối tượng Bitmap mới) để đóng FileStream ngay, đồng thời gọi Dispose() đối với Image cũ trước khi gán ảnh mới.',
 'Dùng trực tiếp Image.FromFile(path) và không bao giờ gọi Dispose() trên PictureBox.Image.',
 'Gọi pictureBox.ImageLocation = path liên tục trong sự kiện Paint của Form.',
 'Sao chép tệp ảnh vào thư mục System32 trước khi nạp lên giao diện.', 'A'),

(2, 'Hard', 'Vì sao khi xây dựng danh sách thẻ câu hỏi động bằng FlowLayoutPanel trong WinForms, việc tạo mới đối tượng Font bên trong vòng lặp vẽ từng thẻ và gọi Controls.Clear() mà không Dispose các control cũ lại gây suy giảm hiệu năng nghiêm trọng?',
 'Vì FlowLayoutPanel chỉ cho phép chứa tối đa 10 control con trong một lần chạy ứng dụng.',
 'Vì mỗi Font và Control đều nắm giữ GDI handle của Windows; không dùng lại Font tĩnh hoặc không gọi Dispose() control cũ sẽ gây rò rỉ GDI handle cho tới khi vượt giới hạn tiến trình.',
 'Vì phương thức Controls.Clear() tự động xóa toàn bộ mã nguồn trong tệp Designer.cs.',
 'Vì hệ điều hành Windows cấm tạo mới đối tượng Font sau khi Form đã gọi OnShown.', 'B'),

-- Chương 3: Truy vấn LINQ và lập trình bất đồng bộ (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(3, 'Easy', 'Trong LINQ, phương thức mở rộng nào được sử dụng để biến đổi (ánh xạ) từng phần tử của tập hợp nguồn sang một dạng dữ liệu mới (ví dụ trích xuất danh sách chỉ gồm Id hoặc chuỗi hiển thị)?',
 'Where()', 'Any()', 'Select()', 'Skip()', 'C'),

(3, 'Easy', 'Trong lập trình bất đồng bộ C#, kiểu trả về nào được khuyến nghị sử dụng cho một phương thức async không trả về giá trị dữ liệu cụ thể (trừ trường hợp event handler của giao diện)?',
 'void', 'Thread', 'Action', 'Task', 'D'),

(3, 'Medium', 'Sự khác biệt giữa FirstOrDefault() và SingleOrDefault() trong LINQ khi tập hợp kết quả có nhiều hơn một phần tử thỏa mãn điều kiện là gì?',
 'FirstOrDefault() trả về phần tử đầu tiên tìm thấy, còn SingleOrDefault() sẽ ném ra ngoại lệ InvalidOperationException.',
 'SingleOrDefault() trả về phần tử cuối cùng, còn FirstOrDefault() ném ngoại lệ.',
 'Cả hai phương thức đều tự động xóa các phần tử trùng lặp trong danh sách gốc.',
 'FirstOrDefault() chỉ hoạt động với kiểu mảng int[], còn SingleOrDefault() chỉ dùng cho Dictionary.', 'A'),

(3, 'Medium', 'Khi muốn kiểm tra nhanh xem một danh sách câu hỏi có tồn tại ít nhất một phần tử thỏa mãn điều kiện hay không mà không cần duyệt hết toàn bộ tập hợp, phương thức LINQ nào là tối ưu nhất?',
 'Count(predicate) > 0', 'Any(predicate)', 'ToList().Count > 0', 'All(predicate)', 'B'),

(3, 'Medium', 'Trong .NET, khi cần chạy đồng thời nhiều tác vụ bất đồng bộ độc lập và chờ tất cả các tác vụ đó hoàn thành trước khi tổng hợp kết quả, phương thức nào nên được sử dụng?',
 'Task.RunSynchronously()', 'Task.WhenAny()', 'Task.Yield()', 'Task.WhenAll()', 'D'),

(3, 'Hard', 'Mục đích chính của việc truyền CancellationToken vào các phương thức bất đồng bộ (như truy vấn hoặc tải dữ liệu dài) là gì?',
 'Mã hóa toàn bộ kết quả trả về của Task bằng khóa bí mật.',
 'Bắt buộc hệ điều hành tăng mức độ ưu tiên CPU cho luồng giao diện.',
 'Cho phép hủy bỏ tác vụ bất đồng bộ một cách hợp tác (cooperative cancellation) khi người dùng đóng màn hình hoặc hết thời gian chờ, tránh lãng phí tài nguyên.',
 'Tự động chuyển đổi hàm async thành hàm đồng bộ để tránh dùng từ khóa await.', 'C'),

-- Chương 4: Truy cập dữ liệu với ADO.NET và SQLite (12 câu: 4 Easy, 5 Medium, 3 Hard)
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
 'Khai báo thẻ <EmbeddedResource Include="..." LogicalName="..." /> trong ItemGroup.', 'D'),

(4, 'Easy', 'Trong ADO.NET với SQLite, phương thức nào của đối tượng SqliteCommand trả về một SqliteDataReader để đọc tuần tự từng dòng dữ liệu từ câu lệnh SELECT?',
 'ExecuteReader()', 'ExecuteNonQuery()', 'ExecuteScalar()', 'BeginTransaction()', 'A'),

(4, 'Easy', 'Khi đọc dữ liệu từ SqliteDataReader mà một cột trong bảng có thể chứa giá trị NULL (như cột ImagePath), phương thức nào giúp kiểm tra an toàn trước khi gọi reader.GetString(index)?',
 'reader.HasRows', 'reader.IsDBNull(index)', 'reader.Read()', 'reader.GetFieldType(index)', 'B'),

(4, 'Medium', 'Lớp SqliteConnectionStringBuilder trong thư viện Microsoft.Data.Sqlite mang lại lợi ích gì so với việc nối chuỗi kết nối thủ công?',
 'Tự động tạo toàn bộ các bảng dữ liệu trong ứng dụng mà không cần file schema.sql.',
 'Cho phép chia sẻ cùng một kết nối SQLite qua mạng Internet mà không cần dịch vụ máy chủ.',
 'Giúp xây dựng chuỗi kết nối đúng cú pháp, thoát ký tự đặc biệt an toàn và cấu hình tường minh các thuộc tính như DataSource hoặc ForeignKeys.',
 'Tự động nén dung lượng tệp cơ sở dữ liệu giảm đi 90 phần trăm sau mỗi lần mở kết nối.', 'C'),

(4, 'Medium', 'Vì sao trong kiến trúc ứng dụng desktop dùng SQLite, phương thức CreateConnection() nên trả về một kết nối ở trạng thái chưa mở để từng hàm Repository tự gọi Open() và giải phóng bằng using?',
 'Vì thư viện Microsoft.Data.Sqlite không cho phép mở hơn một câu lệnh SELECT trong toàn vòng đời ứng dụng.',
 'Vì kết nối chưa mở tự động chạy nhanh hơn kết nối đã mở khi đọc dữ liệu.',
 'Vì phương thức Open() tự động đóng toàn bộ các Form đang mở trên màn hình.',
 'Để giới hạn phạm vi sống của kết nối ngắn nhất có thể, tránh giữ kết nối mở lâu gây khóa ghi (database locked) và đảm bảo giải phóng tài nguyên ngay cả khi phát sinh ngoại lệ.', 'D'),

(4, 'Medium', 'Trong SQLite, ngoại lệ SqliteException với mã lỗi SqliteErrorCode = 19 (SQLITE_CONSTRAINT) thường phát sinh trong tình huống nào khi thực thi câu lệnh INSERT hoặc UPDATE?',
 'Đường dẫn tệp cơ sở dữ liệu không tồn tại trên ổ đĩa.',
 'Dữ liệu ghi vào vi phạm các ràng buộc toàn vẹn như UNIQUE, PRIMARY KEY, NOT NULL, CHECK hoặc FOREIGN KEY.',
 'Câu lệnh SQL bị thiếu dấu chấm phẩy ở cuối dòng.',
 'Bộ nhớ RAM của máy tính không đủ để mở tệp cơ sở dữ liệu.', 'B'),

(4, 'Hard', 'Khi thực thi hàm xóa hoặc thay thế danh sách câu hỏi của một đề thi trong ExamPaperRepository, vì sao việc kiểm tra điều kiện đề thi đã được dùng trong ca thi hay chưa phải được đặt ngay bên trong cùng SqliteTransaction trước lệnh DELETE?',
 'Để ngăn chặn lỗi tranh chấp dữ liệu (race condition / TOCTOU) giữa thời điểm UI hoặc Service kiểm tra và thời điểm Repository thực sự xóa dữ liệu trong CSDL.',
 'Vì SQLite không hỗ trợ chạy câu lệnh SELECT EXISTS bên ngoài khối BeginTransaction.',
 'Để tự động bỏ qua tất cả các ràng buộc khóa ngoại ON DELETE RESTRICT đã khai báo trong schema.sql.',
 'Vì câu lệnh DELETE trong SQLite chỉ hoạt động khi có ít nhất một câu lệnh SELECT chạy sau nó.', 'A')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_LTNET q
JOIN Subjects s ON s.Code = 'LTNET'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);

-- 3. CÂU HỎI MÔN CSDL: CƠ SỞ DỮ LIỆU (48 CÂU, CHƯƠNG 1–4)
WITH Data_CSDL(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Mô hình cơ sở dữ liệu quan hệ (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(1, 'Easy', 'Trong mô hình cơ sở dữ liệu quan hệ, một bảng (Relation) được tổ chức gồm các thành phần cơ bản nào?',
 'Các nút gốc, nút lá và con trỏ hai chiều.',
 'Các tệp nhị phân rời rạc không có cấu trúc cột cố định.',
 'Các cột biểu diễn thuộc tính (Attributes) và các dòng biểu diễn bộ dữ liệu (Tuples/Records).',
 'Các thẻ đánh dấu lồng nhau dạng cặp mở và đóng.', 'C'),

(1, 'Easy', 'Ràng buộc CHECK trong định nghĩa bảng SQL (ví dụ CHECK (CorrectOption IN (''A'', ''B'', ''C'', ''D''))) dùng để đảm bảo điều gì?',
 'Tự động tạo chỉ mục tìm kiếm cho cột được khai báo.',
 'Bắt buộc cột đó phải tham chiếu đến khóa chính của một bảng khác.',
 'Tự động tăng giá trị số nguyên mỗi khi thêm dòng mới.',
 'Kiểm tra và giới hạn miền giá trị hợp lệ mà dữ liệu của cột được phép nhận khi chèn hoặc cập nhật.', 'D'),

(1, 'Medium', 'Khóa ứng viên (Candidate Key) trong một bảng quan hệ được định nghĩa chính xác nhất là gì?',
 'Tập hợp tối thiểu các thuộc tính có khả năng xác định duy nhất mỗi dòng trong bảng (một trong số đó được chọn làm khóa chính).',
 'Thuộc tính luôn được phép mang giá trị NULL trên mọi dòng dữ liệu.',
 'Bất kỳ cột nào có kiểu dữ liệu văn bản TEXT trong bảng.',
 'Tập hợp tất cả các khóa ngoại đang trỏ tới các bảng khác.', 'A'),

(1, 'Medium', 'Trong bảng ExamPaperQuestions(PaperId, QuestionId, OrderIndex), việc đặt PRIMARY KEY (PaperId, QuestionId) kết hợp UNIQUE (PaperId, OrderIndex) mang lại tác dụng gì?',
 'Cho phép một câu hỏi xuất hiện nhiều lần liên tiếp trong cùng một đề thi.',
 'Ngăn một câu hỏi bị gắn trùng hai lần vào cùng một đề thi, đồng thời đảm bảo mỗi vị trí thứ tự câu hỏi trong đề là duy nhất.',
 'Bắt buộc mọi đề thi phải có tổng số câu hỏi bằng nhau.',
 'Tự động xóa câu hỏi khỏi bảng Questions khi đề thi bị xóa.', 'B'),

(1, 'Medium', 'Sự khác nhau giữa ràng buộc ON DELETE CASCADE và ON DELETE RESTRICT khi xóa một bản ghi ở bảng cha là gì?',
 'ON DELETE CASCADE luôn báo lỗi, còn ON DELETE RESTRICT tự động xóa hết các bảng liên quan.',
 'Cả hai tùy chọn đều gán giá trị NULL cho khóa ngoại của bảng con.',
 'ON DELETE CASCADE tự động xóa các bản ghi con đang tham chiếu tới dòng cha bị xóa, còn ON DELETE RESTRICT chặn không cho xóa dòng cha nếu vẫn còn dòng con tham chiếu.',
 'ON DELETE RESTRICT chỉ áp dụng cho bảng tạm, không dùng cho bảng thường.', 'C'),

(1, 'Hard', 'Trong bảng ExamAttempts, ràng buộc UNIQUE (SessionId, StudentCode) giúp bảo đảm nguyên tắc nghiệp vụ nào ở tầng cơ sở dữ liệu?',
 'Một sinh viên chỉ được phép làm tối đa một ca thi duy nhất trong toàn bộ khóa học.',
 'Một ca thi chỉ cho phép duy nhất một sinh viên tham gia làm bài.',
 'Sinh viên có thể nộp bài nhiều lần trong cùng một ca thi để lấy điểm cao nhất.',
 'Mỗi sinh viên chỉ có thể có tối đa một bài nộp kết quả chính thức trong cùng một ca thi, chống lỗi nộp lặp ở mức dữ liệu.', 'D'),

-- Chương 2: Ngôn ngữ truy vấn có cấu trúc - SQL (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(2, 'Easy', 'Từ khóa nào trong câu lệnh SELECT của SQL dùng để loại bỏ các giá trị trùng lặp trong tập kết quả trả về (ví dụ lấy danh sách các số chương phân biệt)?',
 'DISTINCT', 'UNIQUE', 'TOP', 'FILTER', 'A'),

(2, 'Easy', 'Trong SQL, để kiểm tra một cột có đang chứa giá trị rỗng (NULL) hay không, biểu thức điều kiện nào là đúng chuẩn?',
 'ColumnName = NULL', 'ColumnName IS NULL', 'ColumnName == ""', 'ColumnName LIKE NULL', 'B'),

(2, 'Medium', 'Hàm tổng hợp COUNT(*) khác với COUNT(TenCot) ở điểm nào khi cột TenCot có chứa giá trị NULL ở một số dòng?',
 'COUNT(*) bỏ qua các dòng có bất kỳ cột nào chứa NULL, còn COUNT(TenCot) đếm cả NULL.',
 'Cả hai hàm luôn trả về kết quả giống hệt nhau trong mọi trường hợp.',
 'COUNT(*) đếm tất cả các dòng thỏa điều kiện (kể cả dòng chứa NULL), còn COUNT(TenCot) chỉ đếm những dòng mà cột TenCot khác NULL.',
 'COUNT(TenCot) trả về tổng giá trị số học của cột thay vì số lượng dòng.', 'C'),

(2, 'Medium', 'Trong SQLite, toán tử nào sau đây dùng để nối chuỗi ký tự (ví dụ ghép tiền tố với mã câu hỏi)?',
 'Toán tử &', 'Toán tử +', 'Toán tử ::', 'Toán tử ||', 'D'),

(2, 'Medium', 'Mệnh đề LIMIT kết hợp OFFSET trong câu lệnh SELECT thường được dùng để giải quyết bài toán nào?',
 'Phân trang kết quả truy vấn (lấy một số lượng bản ghi nhất định bắt đầu từ vị trí chỉ định).',
 'Xóa toàn bộ các dòng dữ liệu trùng lặp trong bảng.',
 'Tạo mới khóa ngoại giữa hai bảng khác nhau.',
 'Chuyển đổi kiểu dữ liệu từ văn bản sang ngày tháng.', 'A'),

(2, 'Hard', 'Xét hai toán tử EXISTS (SELECT 1 FROM ...) và IN (SELECT ...) khi kiểm tra sự tồn tại bản ghi ở bảng con có kích thước rất lớn, ưu điểm của EXISTS là gì?',
 'EXISTS tự động sắp xếp lại dữ liệu bảng con theo thứ tự giảm dần.',
 'EXISTS có khả năng dừng tìm kiếm ngay khi gặp dòng đầu tiên thỏa mãn (short-circuit) và xử lý an toàn khi tập kết quả con có thể chứa giá trị NULL.',
 'EXISTS bắt buộc phải đi kèm mệnh đề GROUP BY mới thực thi được.',
 'IN luôn chạy nhanh hơn EXISTS trong mọi hệ quản trị CSDL kể cả khi thiếu chỉ mục.', 'B'),

-- Chương 3: Chuẩn hóa dữ liệu và giao dịch - Transaction (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(3, 'Easy', 'Trong giao dịch cơ sở dữ liệu (Transaction), lệnh COMMIT có tác dụng gì?',
 'Hủy bỏ toàn bộ các thay đổi đã thực hiện từ đầu giao dịch.',
 'Xóa cấu trúc các bảng được sử dụng trong giao dịch.',
 'Xác nhận và lưu vĩnh viễn tất cả các thay đổi dữ liệu của giao dịch vào cơ sở dữ liệu.',
 'Tạm dừng toàn bộ hệ quản trị cơ sở dữ liệu để sao lưu thủ công.', 'C'),

(3, 'Easy', 'Khi một thao tác ghi bên trong giao dịch gặp lỗi và hệ thống gọi lệnh ROLLBACK, trạng thái dữ liệu của cơ sở dữ liệu sẽ như thế nào?',
 'Các câu lệnh chạy trước lỗi vẫn được lưu lại, chỉ bỏ câu lệnh bị lỗi.',
 'Toàn bộ bảng dữ liệu bị xóa trống hoàn toàn.',
 'Dữ liệu bị lỗi được chuyển sang một bảng dự phòng mang tên Backup.',
 'Toàn bộ các thay đổi dữ liệu thuộc giao dịch đó được hoàn tác về đúng trạng thái ổn định trước khi giao dịch bắt đầu.', 'D'),

(3, 'Medium', 'Một bảng đạt Dạng chuẩn 3 (3NF) khi đã đạt Dạng chuẩn 2 (2NF) và thỏa mãn thêm yêu cầu nào?',
 'Không tồn tại phụ thuộc hàm bắc cầu (transitive dependency) từ khóa chính qua một thuộc tính không khóa sang một thuộc tính không khóa khác.',
 'Bảng chỉ được phép chứa tối đa 3 cột dữ liệu.',
 'Mọi cột trong bảng đều phải là khóa chính.',
 'Không được phép có bất kỳ khóa ngoại nào trong bảng.', 'A'),

(3, 'Medium', 'Thuộc tính Isolation (Tính cô lập) trong bộ nguyên tắc ACID đảm bảo điều gì khi nhiều giao dịch hoạt động đồng thời?',
 'Các giao dịch luôn chạy chậm hơn 10 lần để tiết kiệm điện năng.',
 'Kết quả thực thi đồng thời các giao dịch phải tương đương với việc các giao dịch thực thi tuần tự từng giao dịch một, không làm lộ dữ liệu trung gian chưa Commit.',
 'Tất cả người dùng không thể đăng nhập cùng lúc vào ứng dụng.',
 'Mỗi giao dịch tự động sao chép tệp cơ sở dữ liệu sang một thư mục riêng.', 'B'),

(3, 'Medium', 'Trong SQLite, lệnh SAVEPOINT và ROLLBACK TO SAVEPOINT giúp ích gì trong xử lý giao dịch?',
 'Tự động chuyển đổi cơ sở dữ liệu SQLite sang SQL Server.',
 'Tăng số lượng cột tối đa cho phép trong một bảng.',
 'Mã hóa toàn bộ chuỗi kết nối trong ứng dụng.',
 'Đánh dấu một điểm trung gian bên trong giao dịch lớn để có thể hoàn tác cục bộ về điểm đó mà không hủy bỏ toàn bộ giao dịch.', 'D'),

(3, 'Hard', 'Trong kiến trúc xử lý đồng thời của SQLite, sự khác biệt giữa chế độ giao dịch BEGIN DEFERRED và BEGIN IMMEDIATE là gì?',
 'BEGIN DEFERRED không hỗ trợ lệnh ROLLBACK, còn BEGIN IMMEDIATE thì có.',
 'BEGIN IMMEDIATE tự động xóa các kết nối đọc khác đang mở trên tệp cơ sở dữ liệu.',
 'BEGIN DEFERRED trì hoãn việc xin khóa cho đến câu lệnh đọc/ghi đầu tiên, còn BEGIN IMMEDIATE xin ngay khóa ghi (RESERVED lock) lúc bắt đầu để tránh deadlock khi nâng cấp từ khóa đọc lên khóa ghi.',
 'Cả hai chế độ chỉ khác nhau ở tên gọi và không ảnh hưởng tới cơ chế khóa tệp.', 'C'),

-- Chương 4: Chỉ mục, tối ưu hóa và an toàn dữ liệu (12 câu: 4 Easy, 5 Medium, 3 Hard)
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
 'Quét toàn bộ tệp cơ sở dữ liệu để phát hiện các trang dữ liệu bị hỏng, lỗi cấu trúc B-Tree hoặc sai lệch chỉ mục.', 'D'),

(4, 'Easy', 'Lý do chính cần tạo chỉ mục (Index) trên các cột thường xuyên xuất hiện trong mệnh đề WHERE hoặc JOIN là gì?',
 'Giúp hệ quản trị CSDL định vị nhanh các bản ghi thỏa điều kiện thay vì phải quét tuần tự toàn bộ bảng.',
 'Giúp tự động dịch nội dung văn bản từ tiếng Anh sang tiếng Việt.',
 'Giúp tự động tạo giao diện WinForms cho bảng dữ liệu đó.',
 'Giúp bỏ qua việc kiểm tra ràng buộc khóa chính của bảng.', 'A'),

(4, 'Easy', 'Tấn công SQL Injection thường xảy ra khi lập trình viên viết câu truy vấn bằng cách nào?',
 'Sử dụng câu lệnh SQL có tham số hóa (Parameterized Query).',
 'Nối trực tiếp chuỗi dữ liệu chưa kiểm soát từ người dùng vào bên trong chuỗi câu lệnh SQL.',
 'Gọi Stored Procedure với tham số có kiểu dữ liệu chặt chẽ.',
 'Sử dụng giao dịch SqliteTransaction cho các lệnh INSERT.', 'B'),

(4, 'Medium', 'Với chỉ mục ghép (Composite Index) được tạo trên hai cột (SubjectId, Chapter), truy vấn nào dưới đây khai thác hiệu quả nhất thứ tự cột của chỉ mục này (quy tắc tiền tố bên trái - Leftmost Prefix)?',
 'Truy vấn chỉ lọc điều kiện theo cột Content LIKE ''%SQL%''.',
 'Truy vấn chỉ lọc điều kiện theo cột Chapter = 2 mà không lọc SubjectId.',
 'Truy vấn lọc điều kiện theo cả SubjectId = 1 AND Chapter = 2 (hoặc chỉ theo SubjectId = 1).',
 'Truy vấn chỉ có mệnh đề SELECT COUNT(*) FROM Questions không có WHERE.', 'C'),

(4, 'Medium', 'Trong SQLite, chế độ nhật ký ghi trước WAL (Write-Ahead Logging: PRAGMA journal_mode = WAL;) mang lại ưu điểm nổi bật nào so với chế độ rollback journal mặc định?',
 'Tắt hoàn toàn mọi kiểm tra khóa ngoại để ghi dữ liệu nhanh hơn.',
 'Xóa tự động các bản ghi cũ hơn 30 ngày trong bảng.',
 'Chuyển toàn bộ dữ liệu trên ổ cứng lên bộ nhớ đệm đám mây.',
 'Cho phép các thao tác đọc và một thao tác ghi diễn ra đồng thời mà không chặn lẫn nhau, cải thiện đáng kể hiệu năng đa truy cập.', 'D'),

(4, 'Medium', 'Lệnh VACUUM trong SQLite thường được thực thi định kỳ nhằm mục đích gì?',
 'Xóa toàn bộ các bảng đang tồn tại trong cơ sở dữ liệu.',
 'Xây dựng lại tệp cơ sở dữ liệu để thu hồi dung lượng trống bị phân mảnh sau khi xóa nhiều bản ghi và tối ưu bố cục trang dữ liệu.',
 'Kiểm tra lỗi cú pháp của tất cả các tệp mã nguồn C# trong dự án.',
 'Đặt lại mật khẩu đăng nhập cho toàn bộ người dùng trong bảng Users.', 'B'),

(4, 'Hard', 'Trong các hệ thống thi trắc nghiệm trực tuyến, vì sao chỉ dựa vào kiểm tra hợp lệ ở tầng giao diện (UI) là chưa đủ để bảo vệ dữ liệu mà cần kết hợp cả kiểm tra nghiệp vụ ở tầng Service và ràng buộc/kiểm tra trong Transaction tại tầng Repository?',
 'Vì kiểm tra tại UI chỉ hỗ trợ trải nghiệm người dùng; tầng Service và kiểm tra trong Transaction ở Repository mới đảm bảo tính nhất quán, toàn vẹn dữ liệu và chống race condition dù dữ liệu đến từ bất kỳ luồng gọi nào.',
 'Vì WinForms không hỗ trợ kiểm tra độ dài chuỗi hoặc giá trị số trên các điều khiển nhập liệu.',
 'Vì SQLite bắt buộc mỗi bảng phải có ít nhất 3 lớp kiểm tra trùng lặp nhau mới cho phép mở kết nối.',
 'Vì câu lệnh SQL không thể thực thi được nếu không có ít nhất hai lớp ngoại lệ ném ra từ giao diện.', 'A')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_CSDL q
JOIN Subjects s ON s.Code = 'CSDL'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);

-- 4. CÂU HỎI MÔN MMT: MẠNG MÁY TÍNH (48 CÂU, CHƯƠNG 1–4)
WITH Data_MMT(Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption) AS (
VALUES
-- Chương 1: Tổng quan mạng máy tính và mô hình OSI (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(1, 'Easy', 'Mô hình giao thức TCP/IP thực tế được chia thành mấy tầng chức năng chính (theo chuẩn 4 tầng phổ biến)?',
 '2 tầng', '3 tầng', '4 tầng (Truy cập mạng, Internet, Giao vận, Ứng dụng)', '9 tầng', 'C'),

(1, 'Easy', 'Địa chỉ vật lý MAC (Media Access Control) được gắn cố định trên card mạng (NIC) có độ dài bao nhiêu bit và hoạt động ở tầng nào của mô hình OSI?',
 '16 bit, hoạt động ở tầng Vật lý (Layer 1).',
 '32 bit, hoạt động ở tầng Mạng (Layer 3).',
 '64 bit, hoạt động ở tầng Giao vận (Layer 4).',
 '48 bit (biểu diễn 12 ký tự hexa), hoạt động ở tầng Liên kết dữ liệu (Layer 2).', 'D'),

(1, 'Medium', 'Giao thức ARP (Address Resolution Protocol) trong mạng cục bộ Ethernet đảm nhiệm vai trò gì?',
 'Phân giải từ địa chỉ IP đã biết của một thiết bị trong cùng mạng LAN để tìm ra địa chỉ MAC tương ứng.',
 'Phân giải tên miền dạng văn bản sang địa chỉ IP qua mạng Internet.',
 'Cấp phát tự động địa chỉ IP, Subnet Mask và Default Gateway cho máy trạm.',
 'Mã hóa toàn bộ lưu lượng HTTP giữa trình duyệt và máy chủ.', 'A'),

(1, 'Medium', 'Sự khác biệt giữa Miền đụng độ (Collision Domain) và Miền quảng bá (Broadcast Domain) khi kết nối các máy tính qua thiết bị Switch tầng 2 là gì?',
 'Switch gộp toàn bộ mạng thành một miền đụng độ chung nhưng tách mỗi cổng thành một miền quảng bá riêng.',
 'Mỗi cổng của Switch tạo ra một miền đụng độ riêng biệt, nhưng mặc định tất cả các cổng (khi chưa chia VLAN) vẫn thuộc cùng một miền quảng bá.',
 'Switch loại bỏ hoàn toàn gói tin Broadcast nên không có miền quảng bá nào tồn tại.',
 'Chỉ có Hub mới tạo được nhiều miền đụng độ riêng cho từng cổng.', 'B'),

(1, 'Medium', 'Chế độ truyền dẫn Song công toàn phần (Full-duplex) khác với Bán song công (Half-duplex) ở điểm nào?',
 'Full-duplex chỉ cho phép truyền dữ liệu một chiều từ máy chủ về máy trạm.',
 'Half-duplex cho phép gửi và nhận dữ liệu đồng thời trên hai cặp dây tách biệt.',
 'Full-duplex cho phép thiết bị vừa gửi vừa nhận dữ liệu đồng thời tại cùng một thời điểm mà không xảy ra xung đột, còn Half-duplex tại một thời điểm chỉ truyền theo một chiều.',
 'Full-duplex chỉ hoạt động được trên mạng không dây Wi-Fi cũ.', 'C'),

(1, 'Hard', 'Vì sao giao thức CSMA/CD được thiết kế cho mạng Ethernet dùng Hub/cáp đồng trục truyền thống lại không còn cần thiết khi tất cả máy tính kết nối trực tiếp tới Switch ở chế độ Full-duplex?',
 'Vì Switch sử dụng giao thức UDP thay cho Ethernet Frame.',
 'Vì chế độ Full-duplex tự động tắt toàn bộ địa chỉ MAC trên card mạng.',
 'Vì tốc độ truyền ở chế độ Full-duplex giảm xuống dưới 1 Mbps nên không thể đụng độ.',
 'Vì mỗi cổng Switch là một liên kết điểm-điểm riêng với đường truyền gửi và nhận tách biệt, loại bỏ hoàn toàn khả năng hai khung tin xung đột vật lý trên cùng đường truyền.', 'D'),

-- Chương 2: Tầng ứng dụng và các giao thức mạng (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(2, 'Easy', 'Giao thức SSH (Secure Shell) sử dụng cổng mặc định nào để thiết lập phiên điều khiển từ xa được mã hóa an toàn tới máy chủ?',
 'Cổng 22', 'Cổng 23', 'Cổng 25', 'Cổng 80', 'A'),

(2, 'Easy', 'Sự khác biệt cơ bản giữa hai giao thức nhận thư điện tử POP3 và IMAP là gì?',
 'POP3 chỉ gửi thư đi, còn IMAP chỉ dùng để nhận tệp đính kèm.',
 'POP3 thường tải thư về máy trạm và có thể xóa bản gốc trên máy chủ, còn IMAP giữ nguyên thư trên máy chủ và đồng bộ trạng thái đọc/thư mục giữa nhiều thiết bị.',
 'IMAP không hỗ trợ mã hóa TLS/SSL, còn POP3 bắt buộc dùng cổng 443.',
 'POP3 chỉ chạy trên trình duyệt web, còn IMAP chỉ chạy trong mạng nội bộ.', 'B'),

(2, 'Medium', 'Trong thông điệp phản hồi HTTP, nhóm mã trạng thái (Status Code) nào biểu thị rằng yêu cầu bị lỗi từ phía máy khách (Client Error, ví dụ 400 Bad Request, 401 Unauthorized, 404 Not Found)?',
 'Nhóm 2xx', 'Nhóm 3xx', 'Nhóm 4xx', 'Nhóm 5xx', 'C'),

(2, 'Medium', 'Trong kiến trúc hệ thống phân giải tên miền DNS, loại bản ghi (Record Type) nào dùng để ánh xạ trực tiếp một tên miền (hostname) sang địa chỉ IPv4?',
 'Bản ghi MX', 'Bản ghi PTR', 'Bản ghi AAAA', 'Bản ghi A', 'D'),

(2, 'Medium', 'Giao thức HTTPS bảo vệ nội dung truyền tải giữa trình duyệt và máy chủ web bằng cơ chế nào?',
 'Bọc giao thức HTTP bên trên tầng bảo mật TLS/SSL nhằm mã hóa dữ liệu, xác thực danh tính máy chủ qua chứng chỉ số và bảo đảm toàn vẹn gói tin.',
 'Thay thế toàn bộ giao thức TCP bằng UDP để giảm thời gian bắt tay.',
 'Ẩn địa chỉ IP của máy chủ khỏi bảng định tuyến DNS toàn cầu.',
 'Nén toàn bộ trang HTML thành tệp ảnh JPEG trước khi gửi qua mạng.', 'A'),

(2, 'Hard', 'Khi trình duyệt web gửi yêu cầu tới một máy chủ web đặt sau Reverse Proxy hoặc dùng HTTP Keep-Alive (Persistent Connection) trong HTTP/1.1, lợi ích hiệu năng lớn nhất đạt được là gì?',
 'Trình duyệt bỏ qua hoàn toàn bước phân giải tên miền DNS cho mọi trang web.',
 'Tái sử dụng một kết nối TCP đã thiết lập để gửi và nhận nhiều yêu cầu/phản hồi HTTP liên tiếp, giảm độ trễ bắt tay 3 bước TCP và chi phí thiết lập TLS lặp lại.',
 'Mỗi hình ảnh và tệp CSS được tự động gộp vào cùng một gói tin UDP duy nhất.',
 'Máy chủ không cần trả về bất kỳ tiêu đề HTTP Header nào trong phản hồi.', 'B'),

-- Chương 3: Tầng giao vận - TCP/UDP (12 câu: 4 Easy, 5 Medium, 3 Hard)
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

(3, 'Easy', 'Số hiệu cổng (Port Number) ở tầng Giao vận của bộ giao thức TCP/IP có phạm vi giá trị hợp lệ từ bao nhiêu đến bao nhiêu (số nguyên 16 bit)?',
 'Từ 0 đến 255', 'Từ 1 đến 1023', 'Từ 0 đến 65535', 'Từ 1024 đến 99999', 'C'),

(3, 'Easy', 'So với phần đầu (Header) của giao thức TCP, phần đầu của giao thức UDP có đặc điểm gì nổi bật?',
 'Có kích thước tối thiểu 60 byte để chứa thông tin điều khiển luồng.',
 'Bắt buộc chứa trường Sequence Number và Acknowledgment Number dài 32 bit.',
 'Thay đổi độ dài linh hoạt từ 20 đến 128 byte tùy vào số lượng cờ điều khiển.',
 'Có kích thước cố định rất gọn nhẹ là 8 byte (gồm Source Port, Destination Port, Length và Checksum).', 'D'),

(3, 'Medium', 'Chức năng Dồn kênh và Phân kênh (Multiplexing / Demultiplexing) tại tầng Giao vận dựa vào thông tin nào để chuyển đúng dữ liệu từ tầng Mạng lên đúng tiến trình ứng dụng đang chạy trên máy tính?',
 'Số hiệu cổng nguồn/đích (Source/Destination Port) kết hợp với địa chỉ IP trong Socket.',
 'Tên người dùng đăng nhập hệ điều hành Windows.',
 'Số thứ tự cổng vật lý cắm dây cáp trên thiết bị Switch.',
 'Địa chỉ MAC của máy chủ DNS mặc định.', 'A'),

(3, 'Medium', 'Trong giao thức TCP, trường Sequence Number (Số thứ tự) và Acknowledgment Number (Số xác nhận) được sử dụng cho mục đích chính nào?',
 'Đếm tổng số router trung gian mà gói tin đã đi qua.',
 'Đánh dấu thứ tự từng byte dữ liệu để bên nhận sắp xếp lại đúng trình tự, phát hiện mất gói hoặc gói trùng lặp và báo nhận dữ liệu đã nhận thành công.',
 'Xác định thời gian sống (TTL) tối đa của gói tin trên mạng Internet.',
 'Phân biệt các loại hệ điều hành khác nhau giữa Client và Server.', 'B'),

(3, 'Medium', 'Trong quá trình đóng kết nối TCP bình thường giữa hai bên (Four-way Termination), cờ điều khiển nào được gửi đi để thông báo một bên đã truyền xong dữ liệu và yêu cầu kết thúc chiều truyền đó?',
 'Cờ SYN', 'Cờ PSH', 'Cờ URG', 'Cờ FIN', 'D'),

(3, 'Hard', 'Trong giao thức TCP, vì sao sau khi đóng kết nối ở phía chủ động đóng (active close), socket thường phải chờ một khoảng thời gian ở trạng thái TIME_WAIT (thường bằng 2*MSL) trước khi giải phóng hoàn toàn?',
 'Để chờ card mạng tự động khởi động lại trình điều khiển.',
 'Để đảm bảo toàn bộ dữ liệu trong RAM được sao lưu ra ổ cứng.',
 'Để đảm bảo gói ACK cuối cùng đến được bên kia (có thể truyền lại nếu bị mất) và để mọi gói tin cũ đi lạc trên mạng hết hạn trước khi tái sử dụng cặp cổng đó.',
 'Để ép buộc hệ điều hành chuyển toàn bộ lưu lượng tiếp theo sang giao thức ICMP.', 'C'),

-- Chương 4: Địa chỉ IPv4 và chia mạng con - Subnetting (12 câu: 4 Easy, 5 Medium, 3 Hard)
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
 'NAT (Network Address Translation) / PAT', 'D'),

(4, 'Easy', 'Lệnh mạng phổ biến nào sử dụng giao thức ICMP (Echo Request / Echo Reply) để kiểm tra khả năng kết nối thông suốt và đo độ trễ khứ hồi (RTT) tới một địa chỉ IP đích?',
 'ping', 'ipconfig', 'nslookup', 'netstat', 'A'),

(4, 'Easy', 'Trong mạng LAN, khi một máy trạm cần gửi gói tin tới một địa chỉ IP nằm ngoài mạng con (Subnet) hiện tại của nó, gói tin đó sẽ được chuyển tiếp đầu tiên tới thành phần nào?',
 'Máy chủ DNS phụ',
 'Cổng mặc định (Default Gateway / Router)',
 'Địa chỉ Loopback 127.0.0.1',
 'Địa chỉ quảng bá cục bộ 255.255.255.255', 'B'),

(4, 'Medium', 'Cho địa chỉ IP máy trạm là 172.16.45.70 với mặt nạ mạng con 255.255.255.192 (/26). Địa chỉ mạng (Network Address) của mạng con chứa máy trạm này là bao nhiêu?',
 '172.16.45.0', '172.16.45.32', '172.16.45.64', '172.16.45.128', 'C'),

(4, 'Medium', 'Với dải mạng 10.20.30.0/28, địa chỉ quảng bá (Broadcast Address) của mạng con này và số lượng địa chỉ Host khả dụng tối đa là bao nhiêu?',
 '10.20.30.16 và 16 host khả dụng.',
 '10.20.30.31 và 30 host khả dụng.',
 '10.20.30.255 và 254 host khả dụng.',
 '10.20.30.15 và 14 host khả dụng.', 'D'),

(4, 'Medium', 'Trong ba dải địa chỉ IPv4 riêng (Private IP) theo chuẩn RFC 1918, dải mạng lớp B thuộc phạm vi nào sau đây?',
 '10.0.0.0 – 10.255.255.255 (10.0.0.0/8)',
 '172.16.0.0 – 172.31.255.255 (172.16.0.0/12)',
 '192.168.0.0 – 192.168.255.255 (192.168.0.0/16)',
 '169.254.0.0 – 169.254.255.255 (169.254.0.0/16)', 'B'),

(4, 'Hard', 'Một khoa đại học được cấp khối địa chỉ 192.168.50.0/24 và áp dụng kỹ thuật VLSM (Variable Length Subnet Mask) để chia cho 3 phòng: Phòng A cần 100 máy, Phòng B cần 50 máy, Phòng C cần 20 máy. Phương án phân bổ mặt nạ mạng con tối ưu không chồng lấn từ lớn đến nhỏ là gì?',
 'Phòng A dùng 192.168.50.0/25 (126 host); Phòng B dùng 192.168.50.128/26 (62 host); Phòng C dùng 192.168.50.192/27 (30 host).',
 'Phòng A dùng 192.168.50.0/26; Phòng B dùng 192.168.50.64/26; Phòng C dùng 192.168.50.128/26.',
 'Phòng A dùng 192.168.50.0/24; Phòng B dùng 192.168.50.128/25; Phòng C dùng 192.168.50.192/28.',
 'Phòng A dùng 192.168.50.0/25; Phòng B dùng 192.168.50.64/26; Phòng C dùng 192.168.50.128/27.', 'A')
)
INSERT INTO Questions (SubjectId, Chapter, Difficulty, Content, OptionA, OptionB, OptionC, OptionD, CorrectOption, ImagePath)
SELECT s.Id, q.Chapter, q.Difficulty, q.Content, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, ''
FROM Data_MMT q
JOIN Subjects s ON s.Code = 'MMT'
WHERE NOT EXISTS (
    SELECT 1 FROM Questions old
    WHERE old.SubjectId = s.Id AND old.Content = q.Content
);
