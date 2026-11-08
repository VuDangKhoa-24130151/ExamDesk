-- Du lieu thu rieng cho Ban 3. Chay thu cong sau khi app da khoi tao schema.
-- Khong tu nap khi mo app, khong xoa/ghi de du lieu hien co.
-- MSSV: 24130151, 24130152. Khong dung cac tai khoan nay de dang nhap mat khau.
-- Sau khi nap: vao Ca thi, tao ca gan TEST_DE01, roi mo ca.
PRAGMA foreign_keys = ON;
BEGIN IMMEDIATE;
-- Nang MSSV mau cu sang dang so, chi sua tai khoan thu chua co bai nop.
UPDATE Users SET StudentCode='24130151'
WHERE Username='TEST_SV01' AND StudentCode='TEST_SV01' AND Role='Student'
AND PasswordHash='!DISABLED_TEST_ACCOUNT!'
AND NOT EXISTS (SELECT 1 FROM Users WHERE StudentCode='24130151')
AND NOT EXISTS (SELECT 1 FROM ExamAttempts WHERE StudentCode='TEST_SV01');
UPDATE Users SET StudentCode='24130152'
WHERE Username='TEST_SV02' AND StudentCode='TEST_SV02' AND Role='Student'
AND PasswordHash='!DISABLED_TEST_ACCOUNT!'
AND NOT EXISTS (SELECT 1 FROM Users WHERE StudentCode='24130152')
AND NOT EXISTS (SELECT 1 FROM ExamAttempts WHERE StudentCode='TEST_SV02');

INSERT INTO Users(Username,PasswordHash,FullName,Role,StudentCode,ClassName)
SELECT 'TEST_SV01','!DISABLED_TEST_ACCOUNT!','Sinh viên thử 01','Student','24130151','TEST_LOP01'
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username='TEST_SV01' OR StudentCode='24130151');
INSERT INTO Users(Username,PasswordHash,FullName,Role,StudentCode,ClassName)
SELECT 'TEST_SV02','!DISABLED_TEST_ACCOUNT!','Sinh viên thử 02','Student','24130152','TEST_LOP01'
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username='TEST_SV02' OR StudentCode='24130152');

INSERT INTO Subjects(Code,Name) VALUES ('TEST_NET','Lập trình .NET — dữ liệu thử') ON CONFLICT(Code) DO NOTHING;

WITH Samples(Chapter,Difficulty,Content,A,B,C,D,Answer) AS (VALUES
(1,'Easy','[TEST_Q01] Kiểu dữ liệu C# nào biểu diễn đúng hoặc sai?','int','bool','string','double','B'),
(1,'Easy','[TEST_Q02] Chỉ số phần tử đầu tiên của mảng C# là bao nhiêu?','0','1','-1','Tùy độ dài mảng','A'),
(2,'Medium','[TEST_Q03] Từ khóa nào giúp giải phóng đối tượng IDisposable khi kết thúc phạm vi?','namespace','return','using','public','C'),
(2,'Medium','[TEST_Q04] Mệnh đề SQL nào lọc các dòng theo điều kiện?','ORDER BY','WHERE','GROUP BY','SELECT DISTINCT','B'),
(3,'Hard','[TEST_Q05] Khi nhiều thao tác ghi phải cùng thành công hoặc cùng thất bại, nên dùng gì?','Một biến static','Một Label','Một vòng lặp','Transaction','D'),
(3,'Hard','[TEST_Q06] Để tránh ghép dữ liệu người dùng trực tiếp vào SQL, nên dùng gì?','SQL có tham số','Nối chuỗi','Đổi tên bảng','Ẩn ô nhập','A'))
INSERT INTO Questions(SubjectId,Chapter,Difficulty,Content,OptionA,OptionB,OptionC,OptionD,CorrectOption,ImagePath)
SELECT s.Id,q.Chapter,q.Difficulty,q.Content,q.A,q.B,q.C,q.D,q.Answer,''
FROM Samples q JOIN Subjects s ON s.Code='TEST_NET'
WHERE NOT EXISTS (SELECT 1 FROM Questions old WHERE old.SubjectId=s.Id AND old.Content=q.Content);

INSERT INTO ExamPapers(PaperCode,Title,SubjectId,DurationMinutes,EasyCount,MediumCount,HardCount)
SELECT 'TEST_DE01','Đề thử cho luồng sinh viên — 6 câu',Id,10,2,2,2 FROM Subjects WHERE Code='TEST_NET'
ON CONFLICT(PaperCode) DO NOTHING;

WITH Expected(Prefix,Position) AS (VALUES
('[TEST_Q01]%',1),('[TEST_Q02]%',2),('[TEST_Q03]%',3),
('[TEST_Q04]%',4),('[TEST_Q05]%',5),('[TEST_Q06]%',6))
INSERT INTO ExamPaperQuestions(PaperId,QuestionId,OrderIndex)
SELECT p.Id,q.Id,e.Position FROM Expected e
JOIN Questions q ON q.Content LIKE e.Prefix
JOIN Subjects s ON s.Id=q.SubjectId AND s.Code='TEST_NET'
JOIN ExamPapers p ON p.PaperCode='TEST_DE01' AND p.SubjectId=s.Id
WHERE NOT EXISTS (SELECT 1 FROM ExamPaperQuestions old WHERE old.PaperId=p.Id AND (old.QuestionId=q.Id OR old.OrderIndex=e.Position));

COMMIT;

-- De 30 cau de kiem thu day du luong lam bai. Khong thay doi de 6 cau cu.
BEGIN IMMEDIATE;
WITH Samples(N,Content,A,B,C,D,Answer) AS (VALUES
(1,'Từ khóa khai báo một lớp trong C# là gì?','class','struct','namespace','using','A'),
(2,'Kiểu nào lưu giá trị đúng hoặc sai?','int','bool','decimal','char','B'),
(3,'Chỉ số đầu tiên của mảng C# là bao nhiêu?','1','-1','0','2','C'),
(4,'Thuộc tính nào trả về số phần tử của một mảng?','Count() luôn bắt buộc','Size','Capacity','Length','D'),
(5,'Toán tử nào so sánh hai số nguyên bằng nhau?','==','=','!=','=>','A'),
(6,'Từ khóa nào tạo một đối tượng của lớp?','this','new','base','typeof','B'),
(7,'Vòng lặp nào phù hợp để duyệt từng phần tử của một collection?','switch','if','foreach','try','C'),
(8,'Từ khóa nào kết thúc phương thức và có thể trả về giá trị?','continue','break','throw','return','D'),
(9,'Kiểu nào thường dùng cho số tiền cần tính toán thập phân chính xác?','decimal','bool','char','byte','A'),
(10,'Khối nào xử lý ngoại lệ đã phát sinh trong try?','finally','catch','using','lock','B'),
(11,'Phạm vi private cho phép truy cập thành viên chủ yếu ở đâu?','Mọi assembly','Mọi lớp kế thừa','Trong kiểu khai báo thành viên','Mọi namespace','C'),
(12,'Từ khóa nào cho phép lớp con ghi đè một phương thức của lớp cha?','static','sealed','private','virtual','D'),
(13,'Từ khóa nào được lớp con dùng để ghi đè phương thức virtual?','override','new luôn tương đương override','const','readonly','A'),
(14,'List<int> biểu diễn cấu trúc nào?','Mảng cố định chỉ một phần tử','Danh sách số nguyên có thể thay đổi kích thước','Tập chuỗi duy nhất','Bảng dữ liệu SQL','B'),
(15,'Dictionary<TKey,TValue> tổ chức dữ liệu theo dạng nào?','Chỉ mục liên tiếp bắt buộc','Cây giao diện','Cặp khóa và giá trị','Chỉ các giá trị bool','C'),
(16,'using với đối tượng IDisposable có mục đích gì?','Chạy trên luồng mới','Bỏ qua ngoại lệ','Tự động lưu SQL','Gọi Dispose khi kết thúc phạm vi','D'),
(17,'Sự kiện nào thường xử lý thao tác nhấn Button trong WinForms?','Click','Load','Resize','FormClosing','A'),
(18,'Thuộc tính nào của TextBox cung cấp nội dung người dùng nhập?','Name','Text','Tag','Enabled','B'),
(19,'Đặt Enabled=false cho Button có tác dụng gì?','Xóa Button','Ẩn Form','Vô hiệu hóa tương tác với Button','Đổi tên Button','C'),
(20,'DockStyle.Fill làm control bố trí như thế nào?','Nằm ngoài Form','Giữ kích thước 1 pixel','Chỉ nằm ở góc dưới','Lấp đầy vùng bố trí còn lại của control cha','D'),
(21,'Câu lệnh SQL nào đọc dữ liệu từ bảng?','SELECT','INSERT','UPDATE','DELETE','A'),
(22,'Mệnh đề SQL nào lọc dòng theo điều kiện?','ORDER BY','WHERE','GROUP BY','LIMIT luôn thay thế WHERE','B'),
(23,'Câu lệnh nào thêm một dòng dữ liệu?','DROP TABLE','ALTER TABLE','INSERT INTO','SELECT','C'),
(24,'Mệnh đề nào sắp xếp kết quả truy vấn SQL?','HAVING','JOIN','WHERE','ORDER BY','D'),
(25,'PRIMARY KEY dùng để làm gì?','Định danh duy nhất một dòng','Mã hóa mọi cột','Sắp xếp mọi truy vấn','Tự tạo giao diện','A'),
(26,'FOREIGN KEY thể hiện điều gì?','Mật khẩu của database','Tham chiếu giữa dữ liệu ở các bảng','Độ dài tối đa chuỗi','Tên file ảnh','B'),
(27,'Vì sao dùng tham số SQL cho dữ liệu người dùng?','Để luôn bỏ qua lỗi','Để tự động tạo bảng','Để tách dữ liệu khỏi cú pháp SQL','Để tắt ràng buộc','C'),
(28,'Transaction giúp nhiều thao tác ghi đáp ứng yêu cầu nào?','Luôn chạy song song','Không bao giờ bị lỗi','Không cần kiểm tra dữ liệu','Cùng thành công hoặc cùng được hoàn tác','D'),
(29,'SQL kiểm tra một cột có giá trị NULL bằng biểu thức nào?','IS NULL','= NULL','== NULL','EQUALS NULL','A'),
(30,'SQLite trong ứng dụng này lưu dữ liệu chủ yếu ở đâu?','Chỉ trong Label','Trong file cơ sở dữ liệu trên máy','Bắt buộc trên máy chủ SQL Server','Chỉ trong biến của Form','B'))
INSERT INTO Questions(SubjectId,Chapter,Difficulty,Content,OptionA,OptionB,OptionC,OptionD,CorrectOption,ImagePath)
SELECT s.Id,1+(q.N-1)/10,CASE WHEN q.N<=10 THEN 'Easy' WHEN q.N<=25 THEN 'Medium' ELSE 'Hard' END,
'[UI30_' || printf('%02d',q.N) || '] ' || q.Content,q.A,q.B,q.C,q.D,q.Answer,''
FROM Samples q JOIN Subjects s ON s.Code='TEST_NET'
WHERE NOT EXISTS (SELECT 1 FROM Questions old WHERE old.SubjectId=s.Id AND old.Content LIKE '[UI30_' || printf('%02d',q.N) || ']%');
INSERT INTO ExamPapers(PaperCode,Title,SubjectId,DurationMinutes,EasyCount,MediumCount,HardCount)
SELECT 'TEST_UI30','Đề thi thử C# và SQLite — 30 câu',Id,45,10,15,5 FROM Subjects WHERE Code='TEST_NET'
ON CONFLICT(PaperCode) DO NOTHING;
INSERT INTO ExamPaperQuestions(PaperId,QuestionId,OrderIndex)
SELECT p.Id,q.Id,CAST(substr(q.Content,7,2) AS INTEGER)
FROM ExamPapers p JOIN Questions q ON q.SubjectId=p.SubjectId
WHERE p.PaperCode='TEST_UI30' AND q.Content GLOB '[[]UI30_[0-9][0-9]]*'
AND NOT EXISTS (SELECT 1 FROM ExamPaperQuestions old WHERE old.PaperId=p.Id AND (old.QuestionId=q.Id OR old.OrderIndex=CAST(substr(q.Content,7,2) AS INTEGER)));
INSERT INTO ExamSessions(SessionCode,SessionName,PaperId,RoomName,IsOpen)
SELECT 'TEST_UI30_CA01','Thi thử 30 câu — C# và SQLite',Id,'Phòng test UI',1 FROM ExamPapers
WHERE PaperCode='TEST_UI30' AND (SELECT COUNT(*) FROM ExamPaperQuestions WHERE PaperId=ExamPapers.Id)=30
ON CONFLICT(SessionCode) DO NOTHING;
COMMIT;
