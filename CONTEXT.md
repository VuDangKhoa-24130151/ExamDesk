# UniExam — Phân công và cách bắt đầu

## 1. Làm UI trước

Ứng dụng thi trắc nghiệm C# WinForms, .NET 10, SQLite, offline. Giữ một project `src/ExamDesk/ExamDesk.csproj`, namespace `ExamDesk`, tên hiển thị UniExam.

**Ba bạn bắt đầu UI song song, chưa cần hoàn thiện database hoặc seed.** Mỗi phân hệ có thể dùng danh sách minh họa riêng để dựng bảng, form và trạng thái. Không coi thao tác minh họa là lưu/nộp dữ liệu thật.

Hiện có cửa sổ tích hợp mở các UserControl giảng viên, Form đăng nhập GV và Form sinh viên. Các màn hình chỉ là khung có tiêu đề, chưa dựng sẵn 26 màn hình và chưa có xác thực/phân quyền. Services/Repositories còn ném NotImplementedException; khởi động UI không gọi chúng, không tạo SQLite.

## 2. Phân công đã chốt

| Người | Màn hình và công việc | Thư mục UI |
|---|---|---|
| Bạn 1 — Quản trị và câu hỏi | AUTH-01/02, GV-01 → GV-06, HELP-01. Chọn vai trò, đăng nhập GV, tổng quan, môn/chương, lớp/SV, ngân hàng câu hỏi, trợ giúp | Administration |
| Bạn 2 — Đề, ca và báo cáo | GV-07 → GV-15. Đề thi, ca thi, kết quả/phúc khảo chỉ đọc, phổ điểm, xuất/nhập, sao lưu | ExamManagement, Reports, DataManagement |
| Bạn 3 — Sinh viên và làm bài | AUTH-03, SV-01 → SV-07. Nhận diện, chọn ca, hướng dẫn, làm bài, lưu/khôi phục, nộp/chấm điểm, xem điểm | Student |

Bạn 3 tạo/lưu kết quả; Bạn 2 đọc kết quả để báo cáo. Khi đến nghiệp vụ, mỗi người làm cả UI → Service → Repository của phần mình; không có một người phải viết toàn bộ database cho hai người còn lại.

## 3. Cấu trúc

```text
uniexam/                         Repository Git; mở terminal tại đây
├── CONTEXT.md                   Phân công và cách làm — bản chính
├── DESIGN.md                    Quy ước UI — bản chính
├── database/sql/                Schema/seed dự kiến, hoàn thiện sau
├── installer/                   Đóng gói cuối dự án
└── src/ExamDesk/
    ├── ExamDesk.csproj
    ├── Program.cs               Bạn 1 điều phối
    ├── UI/
    │   ├── MainForm.cs          Bạn 1: menu và ghép màn hình
    │   ├── Shared/UiStyle.cs    Màu chung, Bạn 1 điều phối
    │   ├── Administration/     Bạn 1
    │   ├── ExamManagement/      Bạn 2: đề và ca
    │   ├── Reports/             Bạn 2: kết quả, phúc khảo, thống kê
    │   ├── DataManagement/      Bạn 2: tệp và sao lưu
    │   └── Student/             Bạn 3
    ├── Models/                  Kiểu dữ liệu chung
    ├── Services/                Nghiệp vụ; mốc UI chưa cần gọi
    ├── Repositories/            SQL/transaction, làm sau
    └── Data/SqliteContext.cs    Kết nối, chưa khởi tạo database
```

CONTEXT.md và DESIGN.md ở thư mục cha là bản sao tiện đọc. Bản trong `uniexam/` là bản chính chia sẻ qua Git. Khi cập nhật tài liệu hãy đồng bộ bản sao nếu còn sử dụng.

## 4. Bắt tay vào việc

1. Build và mở khung, đọc DESIGN.md, xác định thư mục mình sở hữu.
2. Bạn 1 bắt đầu `Administration/QuestionBankControl`; Bạn 2 bắt đầu `ExamManagement/ExamPaperControl`; Bạn 3 bắt đầu `Student/StudentExamForm`.
3. Starter đã có `.cs` và `.Designer.cs`, constructor không tham số. Thêm control bằng Designer nếu IDE hỗ trợ; giữ sự kiện trong `.cs`.
4. Thêm UserControl/Dialog cho các màn hình còn lại trong thư mục của mình. Dùng dữ liệu minh họa nhỏ hoặc `PreviewData.cs` riêng của phân hệ, chưa cần seed SQLite.
5. Khi có màn hình mới, gửi tên class và cách mở cho Bạn 1 gắn vào MainForm. Hai bạn còn lại không cùng sửa menu chung.
6. Ghép từng nhóm màn hình hoàn thành; kiểm tra mở/đóng, quay lại, resize và bàn phím. Không đợi cuối kỳ mới ghép.

Khung hiện tại chuyển menu sẽ Dispose control cũ, nên dữ liệu UI tạm không được giữ qua menu. Các bước thêm/sửa/xem trước trong phân hệ cần tự giữ state hoặc hỏi bỏ thay đổi. Trước khi ghép bản thật, Bạn 1 bổ sung điều hướng bảo toàn dữ liệu khi cần. Đóng Form sinh viên trở về khung; chưa có phiên đăng nhập thật.

## 5. Quyền sửa file để ít xung đột

- Mỗi người sở hữu cả bộ `.cs/.Designer.cs/.resx` của màn hình mình. Không cùng sửa một Form.
- Bạn 1 điều phối MainForm, Program, UI/Shared và csproj. Người khác đề xuất thay đổi thay vì cùng sửa. Project SDK tự nhận file .cs mới, không cần thêm từng file vào csproj.
- Models, Data và schema.sql giữ nguyên ở mốc UI; khi đến nghiệp vụ cả nhóm thống nhất trước, Bạn 1 điều phối ghép.
- Dùng nhánh `feature/admin-ui`, `feature/exam-management-ui`, `feature/student-ui`. Giữ main build được, ghép phần nhỏ thường xuyên. Không commit bin/obj, database chạy thật hoặc cấu hình IDE cá nhân.

## 6. Phân công tầng dưới khi làm nghiệp vụ

| Người | Services | Repositories |
|---|---|---|
| Bạn 1 | QuestionBankService; bổ sung danh mục/đăng nhập khi cần | QuestionRepository |
| Bạn 2 | ExamPaperService, ExamSessionService, GradingReviewService; bổ sung dữ liệu/sao lưu | ExamPaperRepository, ExamSessionRepository, GradingRepository |
| Bạn 3 | StudentExamService, SubmissionService | StudentExamRepository, SubmissionRepository |

Các chữ ký chỉ là gợi ý ban đầu, chưa phải hợp đồng dữ liệu hoàn chỉnh. Đã tách chấm/lưu của Bạn 3 khỏi đọc báo cáo và tạo ca của Bạn 2. Phúc khảo chỉ đọc, không thêm sửa điểm/ghi kết luận ở mốc này.

Trước khi nối nghiệp vụ, chốt danh mục chương/lớp/SV; đề nháp/phát hành và snapshot; danh sách được thi; khung giờ ca; trạng thái lượt thi, deadline, câu trả lời đang làm và lịch sử thay đổi. Schema hiện tại chưa đủ các luồng đó và chưa tự chạy lúc mở app. Không xóa dữ liệu cũ để nâng cấp schema.

Quy tắc triển khai sau: rút đủ câu không trùng; một lượt/SV/ca; timer theo deadline gốc; hết giờ tự chốt/nộp một lần; lỗi lưu giữ bài; chỉ hiện điểm sau commit; đáp án chuẩn lấy từ dữ liệu đáng tin cậy; sửa ngân hàng không đổi đề đã thi. Điểm = số đúng/tổng ×10, hai chữ số thập phân; sai và bỏ trống không có điểm nhưng thống kê riêng.

## 7. Chạy project

Yêu cầu Windows và .NET 10 SDK. Mở `src/ExamDesk/ExamDesk.csproj` bằng IDE hỗ trợ WinForms. Chạy từ thư mục `uniexam/`:

```powershell
dotnet build src/ExamDesk/ExamDesk.csproj
dotnet run --project src/ExamDesk/ExamDesk.csproj
```

Restore lần đầu có thể cần mạng nếu máy chưa có gói Microsoft.Data.Sqlite. Ứng dụng hoàn thiện vẫn chạy offline. Khung hiện không có UniExam.sln; không dùng đường dẫn src/UniExam/UniExam.csproj cũ.

Đường dẫn database dự kiến theo code: `%LocalAppData%/ExamDesk/database/examdesk.db`. Chưa cần tạo database hoặc tài khoản để mở khung UI.

## 8. Nghiệm thu mốc UI

- Mở được màn hình các nhóm từ khung, không gọi stub gây lỗi.
- Đúng bố cục; nút chuyển/quay lại và dialog minh họa hoạt động.
- Có empty/error/loading theo thiết kế; không coi dữ liệu minh họa là kết quả lưu thật.
- Không mất nút/cắt chữ khi resize; build thành công trước khi ghép.

Mốc tiếp theo mới chốt Models/schema, nối SQLite và kiểm thử luồng đề → ca → làm bài → nộp → kết quả.
