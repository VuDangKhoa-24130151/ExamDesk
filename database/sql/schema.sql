-- THIET KE SQLITE BAN DAU - CHUA DUOC APP TU DONG THUC THI.
-- Moi may mot database cuc bo. Khong co du lieu mau.
-- Ban 1 dieu phoi schema; thay doi chung can thong nhat ca nhom.
-- CREATE TABLE IF NOT EXISTS khong nang cap database cu.
PRAGMA foreign_keys = ON;

-- Dung chung; Ban 2 hoan thien nhan dien/dang nhap neu can tai khoan.
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Role TEXT NOT NULL CHECK (Role IN ('Lecturer', 'Student', 'Examiner')),
    StudentCode TEXT NOT NULL DEFAULT '',
    ClassName TEXT NOT NULL DEFAULT ''
);

-- Ban 1: ngan hang cau hoi va de thi.
CREATE TABLE IF NOT EXISTS Subjects (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Questions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SubjectId INTEGER NOT NULL REFERENCES Subjects(Id) ON DELETE RESTRICT,
    Chapter INTEGER NOT NULL CHECK (Chapter > 0),
    Difficulty TEXT NOT NULL CHECK (Difficulty IN ('Easy', 'Medium', 'Hard')),
    Content TEXT NOT NULL,
    OptionA TEXT NOT NULL,
    OptionB TEXT NOT NULL,
    OptionC TEXT NOT NULL,
    OptionD TEXT NOT NULL,
    CorrectOption TEXT NOT NULL CHECK (CorrectOption IN ('A', 'B', 'C', 'D')),
    ImagePath TEXT NOT NULL DEFAULT ''
);
CREATE TABLE IF NOT EXISTS ExamPapers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PaperCode TEXT NOT NULL UNIQUE,
    Title TEXT NOT NULL,
    SubjectId INTEGER NOT NULL REFERENCES Subjects(Id) ON DELETE RESTRICT,
    DurationMinutes INTEGER NOT NULL CHECK (DurationMinutes > 0),
    EasyCount INTEGER NOT NULL CHECK (EasyCount >= 0),
    MediumCount INTEGER NOT NULL CHECK (MediumCount >= 0),
    HardCount INTEGER NOT NULL CHECK (HardCount >= 0),
    CHECK (EasyCount + MediumCount + HardCount > 0)
);
CREATE TABLE IF NOT EXISTS ExamPaperQuestions (
    PaperId INTEGER NOT NULL REFERENCES ExamPapers(Id) ON DELETE RESTRICT,
    QuestionId INTEGER NOT NULL REFERENCES Questions(Id) ON DELETE RESTRICT,
    OrderIndex INTEGER NOT NULL CHECK (OrderIndex > 0),
    PRIMARY KEY (PaperId, QuestionId),
    UNIQUE (PaperId, OrderIndex)
);
-- TODO Ban 1: khoa sua/xoa cau hoi da gan de, giu nguyen de/anh de bao toan lich su.
-- FK chi bao ve tham chieu, khong ngan UPDATE noi dung.

-- Ban 3 quan ly ca; Ban 2 doc ca de lam bai.
CREATE TABLE IF NOT EXISTS ExamSessions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionCode TEXT NOT NULL UNIQUE,
    SessionName TEXT NOT NULL,
    PaperId INTEGER NOT NULL REFERENCES ExamPapers(Id) ON DELETE RESTRICT,
    RoomName TEXT NOT NULL,
    IsOpen INTEGER NOT NULL DEFAULT 0 CHECK (IsOpen IN (0, 1))
);

-- Ban 3: ket qua va chi tiet tung cau, luu cung transaction.
CREATE TABLE IF NOT EXISTS ExamAttempts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionId INTEGER NOT NULL REFERENCES ExamSessions(Id) ON DELETE RESTRICT,
    PaperId INTEGER NOT NULL REFERENCES ExamPapers(Id) ON DELETE RESTRICT,
    StudentCode TEXT NOT NULL CHECK (length(trim(StudentCode)) > 0),
    StudentName TEXT NOT NULL,
    ClassName TEXT NOT NULL,
    StartedAt TEXT NOT NULL,
    SubmittedAt TEXT NOT NULL,
    TotalQuestions INTEGER NOT NULL CHECK (TotalQuestions > 0),
    CorrectCount INTEGER NOT NULL CHECK (CorrectCount BETWEEN 0 AND TotalQuestions),
    Score REAL NOT NULL CHECK (Score BETWEEN 0 AND 10),
    IsAutoSubmitted INTEGER NOT NULL CHECK (IsAutoSubmitted IN (0, 1)),
    ReviewNote TEXT NOT NULL DEFAULT '',
    UNIQUE (SessionId, StudentCode)
);
CREATE TABLE IF NOT EXISTS AttemptAnswers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AttemptId INTEGER NOT NULL REFERENCES ExamAttempts(Id) ON DELETE CASCADE,
    QuestionId INTEGER NOT NULL REFERENCES Questions(Id) ON DELETE RESTRICT,
    QuestionOrder INTEGER NOT NULL CHECK (QuestionOrder > 0),
    SelectedOption TEXT NOT NULL CHECK (SelectedOption IN ('', 'A', 'B', 'C', 'D')),
    CorrectOption TEXT NOT NULL CHECK (CorrectOption IN ('A', 'B', 'C', 'D')),
    IsCorrect INTEGER NOT NULL CHECK (IsCorrect IN (0, 1)),
    UNIQUE (AttemptId, QuestionId),
    UNIQUE (AttemptId, QuestionOrder)
);
-- TODO Ban 3: kiem tra ca/de khop nhau; dap an lay tu de chuan, khong tin UI.
-- Chinh diem co nhat ky va snapshot day du la mo rong, chua thiet ke tai day.
