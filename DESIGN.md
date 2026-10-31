# UniExam — Quy ước UI cho ba người

## 1. Làm UI trước

Theo phân công CONTEXT.md. Bộ thiết kế có 26 màn hình chính; dialog/trạng thái không bắt buộc thành Form riêng. Dùng WinForms chuẩn; chưa cần thư viện UI, hiệu ứng hay theme.

Ảnh tham chiếu nằm tại `../ui-design-26/png/` khi đứng trong repository, hoặc nhận ZIP UniExam-chi-anh-PNG.zip từ nhóm. Ảnh không nằm trong Git nguồn; vẫn có thể làm theo bảng dưới đây.

## 2. Tổ chức giao diện

- Giảng viên: MainForm chứa UserControl, Dock Fill. Màn hình lớn dùng UserControl, thêm/sửa đơn giản có thể dùng dialog.
- Sinh viên: StudentExamForm riêng; Bạn 3 tổ chức các bước nhận diện/chọn ca/làm bài/kết quả bên trong.
- LoginForm chỉ dành cho GV; nhận diện SV nằm trong Student.
- Starter có constructor không tham số và file Designer; màn hình mới nên theo cùng kiểu để mở bằng Designer.
- `.cs` chứa sự kiện, `.Designer.cs` chứa bố cục, `.resx` chứa tài nguyên nếu cần. Cả bộ thuộc cùng một người.
- UI không viết SQL. Trong mốc UI dùng dữ liệu minh họa của phân hệ, chưa gọi Services/Repositories đang ném NotImplementedException.

## 3. Màn hình và điểm bắt đầu

Tên chưa có được thêm dần, không cần tạo đủ file rỗng trước.

| Người | ID | Chức năng | File có sẵn / tên gợi ý |
|---|---|---|---|
| 1 | AUTH-01 | Chọn vai trò | Thêm RoleSelectionControl |
| 1 | AUTH-02 | Đăng nhập GV | Có LoginForm |
| 1 | GV-01 | Tổng quan | Có DashboardControl |
| 1 | GV-02 | Môn và chương | Thêm SubjectChapterControl |
| 1 | GV-03 | Lớp và sinh viên | Thêm ClassStudentControl |
| 1 | GV-04/05/06 | Danh sách, soạn, xem trước câu hỏi | Có QuestionBankControl; thêm editor/preview |
| 1 | HELP-01 | Trợ giúp offline | Thêm HelpControl |
| 2 | GV-07/08/09 | Danh sách, cấu hình, phát hành đề | Có ExamPaperControl; thêm editor/preview |
| 2 | GV-10/11 | Danh sách và tạo ca | Có SessionManagementControl; thêm editor |
| 2 | GV-12/13/14 | Kết quả, phúc khảo, phổ điểm | Có GradingReviewControl; thêm detail/statistics |
| 2 | GV-15 | Xuất/nhập, sao lưu | Có DataManagementControl |
| 3 | AUTH-03 | Nhận diện SV | Thêm StudentIdentityControl |
| 3 | SV-01/02 | Chọn ca và hướng dẫn | Thêm SessionSelectionControl, ExamInstructionsControl |
| 3 | SV-03 | Làm bài | Có khung StudentExamForm; thêm vùng làm bài |
| 3 | SV-04/05 | Kiểm tra nộp, đang lưu/lỗi | Thêm SubmitReviewDialog, SubmissionStatusControl |
| 3 | SV-06/07 | Điểm và khôi phục | Thêm StudentResultControl, ResumeAttemptControl |

Bạn 1 làm lỗi đăng nhập/câu hỏi. Bạn 2 làm thiếu câu, empty báo cáo, preview nhập, xác nhận khôi phục. Bạn 3 làm trạng thái timer, lựa chọn, nộp, lỗi lưu và khôi phục lượt. Không giao cùng một dialog cho hai người.

## 4. Màu, chữ và bố cục

| Thành phần | Quy ước |
|---|---|
| Nền / bề mặt | #F3F6FA / #FFFFFF |
| Chữ chính / phụ | #172B4D / #53647D |
| Primary / sidebar | #185ABD / #142D50 |
| Viền | #DCE3ED |
| Thành công | #146C43 trên #EAF6EF |
| Cảnh báo | #845300 trên #FFF5DB |
| Lỗi | #B42318 trên #FFF0EE |
| Font | Segoe UI; nội dung 10–11pt; phụ 9pt; tiêu đề 16–18pt |
| Khoảng cách | 4/8/12/16/24/32 px ở scale 100%; lề ngoài 24px |
| Nút / ô nhập | Khoảng 38–40px, tăng theo font/DPI khi cần |

Màu cốt lõi trong UI/Shared/UiStyle.cs, Bạn 1 điều phối thay đổi. Không tạo bảng màu khác nhau ở mỗi phân hệ; control chuẩn có thể giữ màu hệ thống nếu dễ đọc hơn.

Ảnh tham chiếu 1366×768 là kích thước thiết kế; WinForms còn title bar/viền. Khung dùng client 1280×680 để vừa laptop. Không ép client 1366×768 làm cửa sổ vượt màn hình. Dùng Dock/Anchor/TableLayoutPanel, vùng cuộn, không đặt toàn bộ bằng tọa độ tuyệt đối. Sidebar khoảng 224px; cột điều hướng câu khoảng 300–328px; nội dung còn lại co giãn.

Starter dùng AutoScaleMode.Font; giữ nhất quán và kiểm tra scale 100/125/150%. Font WinForms dùng pt, không sao chép số px từ ảnh. Text dài Wrap, bảng có cuộn, nút chính không bị khuất. Form mở giữa cha hoặc màn hình.

## 5. Hành vi UI

- Nhãn luôn hiện; trường bắt buộc có dấu *. Lỗi gần trường, nêu cách sửa, giữ nội dung đã nhập.
- Một hành động chính rõ trong mỗi vùng. Có Hủy/Quay lại; hỏi trước khi bỏ thay đổi hoặc thay dữ liệu.
- Bảng có tiêu đề tiếng Việt, hàng chọn, bộ lọc và empty state; có cách xem đầy đủ text dài.
- Tab order hợp lý, focus rõ; Enter theo ngữ cảnh; Esc không nộp bài; Ctrl+S dành cho form quản trị khi có lưu.
- Phản hồi đang xử lý/xong/lỗi. Dữ liệu tạm và chức năng chưa hoàn thành được ghi rõ trong tài liệu bàn giao hoặc comment code; không cần gắn chữ “minh họa” vào tiêu đề, tên người dùng hay trạng thái trên mọi màn hình. Không báo đã lưu/nộp thành công khi chưa thực hiện; nút chưa hoàn thành phải Disabled hoặc thông báo rõ khi bấm.
- Nút chưa làm dùng Disabled hoặc phản hồi rõ đang dựng UI; không bấm im lặng.

Màn hình thi: một lựa chọn A–D, đổi/bỏ được; chuyển câu giữ lựa chọn trong phiên demo, không tự chuyển khi chọn đáp án. Cờ xem lại độc lập. Lưới dùng ký hiệu/viền/chữ bên cạnh màu cho chưa trả lời/đã trả lời/đang xem/gắn cờ. Nộp cách xa chuyển câu và có dialog kiểm tra. Không lộ đáp án chuẩn trong khu SV.

Timer/lưu ở mốc UI có thể mô phỏng, không coi là khôi phục dữ liệu thật. Khi nối nghiệp vụ: deadline cố định, hết giờ tự nộp, ngăn nộp lặp, lỗi lưu giữ bài, hiện điểm sau commit. Phúc khảo chỉ đọc, không thêm sửa điểm.

## 6. Nguyên tắc cần thể hiện

| Nguyên tắc | Áp dụng |
|---|---|
| G1/H4 — nhất quán | Cùng từ ngữ, màu, nút và hành vi desktop |
| G2/H7 — đa dạng, hiệu quả | Bàn phím, focus, tìm kiếm/lọc |
| G3/H1 — phản hồi, trạng thái | Tiến độ, timer, đang xử lý/lỗi lưu |
| G4 — kết thúc rõ | Xác nhận hoàn tất và bước tiếp |
| G5/H5 — ngăn lỗi | Validation, thiếu câu, xác nhận nộp |
| G6/G7/H3 — đảo ngược, kiểm soát | Hủy, quay lại, đổi đáp án trước chốt |
| G8/H6 — giảm ghi nhớ | Thông tin ca, lựa chọn, lưới và chú giải |
| H2 — ngôn ngữ thực tế | Môn/chương/đề/ca; tránh thuật ngữ CSDL trong tác vụ |
| H8 — tối giản | Màn thi chỉ có công cụ làm bài |
| H9 — sửa lỗi | Nêu nguyên nhân và hành động sửa |
| H10 — trợ giúp | Hướng dẫn theo tác vụ có sẵn offline |

## 7. Bàn giao để ghép

Gửi tên class, ID, cách mở, đường quay lại và trạng thái đã demo. UserControl GV mở được bằng constructor không tham số, Dock Fill. Không tự đổi Program.cs để test; nhờ Bạn 1 thêm mục vào khung. Dữ liệu tạm tự quản lý trong phân hệ; khung hiện Dispose control cũ khi chuyển menu.

Trước khi ghép: build thành công, mở/đóng không lỗi, không gọi stub, không cắt chữ/nút; Form riêng đóng trở về khung. Chưa yêu cầu database, thuật toán thi thật hoặc bộ cài để nghiệm thu UI.

## 8. Checklist trước khi bàn giao UI

Mỗi người áp dụng ngay khi làm từng màn và tự rà trước khi ghép. Checklist này dùng để kiểm tra thực tế các nguyên tắc ở mục 6; không yêu cầu mỗi màn phải thể hiện đủ cả 8 nguyên tắc vàng và 10 Heuristic.

- [ ] **Bố cục và chữ:** dùng Dock/Anchor/TableLayoutPanel phù hợp; kiểm tra ở kích thước cửa sổ tối thiểu. Không mất nút hoặc chồng nội dung; chữ dài xuống dòng, cuộn hoặc có cách xem đầy đủ. Thiếu chiều cao phải cuộn đến được các thao tác.
- [ ] **Nhất quán (G1/H4):** dùng màu, font, khoảng cách và thuật ngữ chung. Mỗi vùng có một hành động chính rõ ràng; tên nút diễn tả đúng bước sẽ xảy ra, ví dụ “Xem hướng dẫn” nếu chưa bắt đầu thi.
- [ ] **Bàn phím (G2/H7):** Tab/Shift+Tab theo thứ tự thao tác, bỏ qua thành phần chỉ hiển thị; focus dễ thấy. Enter đúng ngữ cảnh; Esc không tự lưu hoặc nộp bài. Nếu thêm phím tắt, dùng nhất quán và kiểm tra không trùng trong cùng màn.
- [ ] **Phản hồi (G3/H1):** nút bấm có phản hồi; trạng thái dùng chữ hoặc ký hiệu bên cạnh màu. Tác vụ đang xử lý có phản hồi và ngăn bấm lặp khi cần. Chức năng chưa làm phải Disabled hoặc thông báo rõ, không báo thành công giả.
- [ ] **Ngăn lỗi và sửa lỗi (G5/H5/H9):** kiểm tra dữ liệu đầu vào, báo lỗi gần trường và nêu cách sửa; giữ nội dung đã nhập khi lỗi. Xác nhận trước thao tác có thể mất dữ liệu hoặc chốt bài thi.
- [ ] **Điều hướng và kiểm soát (G6/G7/H3):** có đường quay lại hoặc hủy phù hợp. Chuyển màn không âm thầm mất dữ liệu đang làm; giữ trạng thái hoặc hỏi trước khi bỏ thay đổi.
- [ ] **Giảm ghi nhớ, hướng dẫn và kết thúc (G4/G8/H6/H10):** hiển thị thông tin cần cho tác vụ, có hướng dẫn khi cần. Khi hoàn tất tác vụ phải cho biết kết quả và bước tiếp theo; chỉ thông báo lưu/nộp thành công khi xử lý thật đã thành công.
- [ ] **Kiểm tra và bàn giao:** build thành công, mở/đóng/quay lại không lỗi, không gọi stub. Chạy thử DPI 100%/125%/150%; ghi rõ mức đã kiểm tra, lỗi còn lại và chức năng đang dùng dữ liệu tạm. Không đánh dấu đạt chỉ vì build thành công.

Chỉ kiểm tra các trạng thái có liên quan đến màn hình: danh sách trống, dữ liệu không hợp lệ, đang tải, lỗi tải/lưu. Ở mốc UI có thể cho xem trước các trạng thái bằng dữ liệu tạm trong phân hệ; khi nối nghiệp vụ phải kiểm tra lại bằng luồng thật. Không thêm trạng thái hoặc thao tác giả chỉ để đủ checklist.

Sau khi ghép các màn, cả nhóm rà lại toàn bộ luồng theo mục 6: xác định nguyên tắc được thể hiện ở màn nào, thao tác nào và còn thiếu gì. Đặc biệt kiểm tra quay lại, dữ liệu đang làm, lỗi lưu, hết giờ và nộp bài; đánh giá một màn riêng chưa thay thế được kiểm tra cả luồng.

### Comment và cách viết code

- Viết code đơn giản, tên biến/hàm rõ nghĩa, theo cấu trúc hiện có; chỉ tách hàm khi giúp dễ đọc hoặc tránh lặp.
- Comment tiếng Việt ngắn ở đoạn khó: giải thích mục đích hoặc lý do xử lý, ví dụ cách chia hàng/cột, giữ đáp án khi chuyển câu, chặn nộp lặp.
- Không comment lại từng dòng code đã rõ nghĩa. Cập nhật hoặc bỏ comment khi logic thay đổi.
- Dữ liệu ghi sẵn và phần xử lý tạm cần comment rõ để người sau biết chỗ nối Service/Repository; mỗi thành viên phải hiểu và giải thích được phần mình phụ trách.

## 9. Khi nhờ AI hỗ trợ

“Đọc CONTEXT.md và DESIGN.md trong repository. Tôi là Bạn [1/2/3], làm màn hình [ID] trong thư mục [đường dẫn]. Dựng UI và tương tác minh họa bằng WinForms chuẩn, chưa kết nối database. Chỉ sửa file thuộc phần này; đề xuất cho người điều phối nếu cần sửa MainForm/Shared/Models. Không triển khai thay phần hai bạn còn lại.”

