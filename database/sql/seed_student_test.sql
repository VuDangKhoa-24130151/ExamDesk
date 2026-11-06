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
