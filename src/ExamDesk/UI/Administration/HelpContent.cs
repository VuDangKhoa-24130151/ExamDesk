using System.Globalization;
using System.Text;

namespace ExamDesk.UI.Administration;

// HELP-01 • Bạn 1 phụ trách: Nội dung trợ giúp offline theo tác vụ (tĩnh trong bộ nhớ, không đọc mạng/tệp).
internal sealed class HelpTopic
{
    public string Id { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Keywords { get; init; } = string.Empty;
    public IReadOnlyList<string> Steps { get; init; } = [];
    public string Note { get; init; } = string.Empty;
}

internal static class HelpContent
{
    public static IReadOnlyList<HelpTopic> Topics { get; } =
    [
        // ==================== 1. BẮT ĐẦU ====================
        new HelpTopic
        {
            Id = "start-role",
            Group = "Bắt đầu",
            Title = "Chọn vai trò",
            Keywords = "chon vai tro, bat dau, giang vien, sinh vien, auth-01",
            Steps =
            [
                "Bước 1: Mở ứng dụng UniExam hoặc bấm mục \"Chọn vai trò\" ở đầu thanh menu bên trái.",
                "Bước 2: Nếu là giảng viên, bấm nút \"Đăng nhập giảng viên\" ở thẻ bên trái để mở hộp thoại xác thực.",
                "Bước 3: Nếu là sinh viên dự thi, bấm nút \"Vào khu sinh viên\" ở thẻ bên phải để mở cửa sổ làm bài thi."
            ],
            Note = "Trong chế độ dựng UI, bạn vẫn có thể bấm trực tiếp các mục trên thanh menu bên trái để kiểm tra từng màn hình mà chưa cần đăng nhập thật."
        },
        new HelpTopic
        {
            Id = "start-login",
            Group = "Bắt đầu",
            Title = "Đăng nhập giảng viên",
            Keywords = "dang nhap giang vien, mat khau, tai khoan, gv01, auth-02",
            Steps =
            [
                "Bước 1: Tại màn hình \"Chọn vai trò\", bấm nút \"Đăng nhập giảng viên\" (hoặc chọn \"Mở UI đăng nhập GV\" ở cuối menu).",
                "Bước 2: Nhập Tên đăng nhập * (tài khoản minh họa: gv01) và Mật khẩu * (mật khẩu minh họa: 123456).",
                "Bước 3: Có thể tích \"Hiện mật khẩu\" để kiểm tra lại ký tự vừa nhập.",
                "Bước 4: Nhấn phím Enter hoặc bấm nút \"Đăng nhập\". Hệ thống sẽ chuyển tới màn hình \"Tổng quan\" và hiển thị tên giảng viên ở cuối thanh menu."
            ],
            Note = "Nếu để trống trường bắt buộc hoặc nhập sai tài khoản minh họa, thông báo lỗi sẽ xuất hiện ngay dưới ô nhập và giữ nguyên nội dung để bạn sửa lại."
        },
        new HelpTopic
        {
            Id = "start-logout",
            Group = "Bắt đầu",
            Title = "Đăng xuất",
            Keywords = "dang xuat, thoat phien, doi tai khoan",
            Steps =
            [
                "Bước 1: Quan sát khu vực trạng thái tài khoản ở góc dưới cùng của thanh menu bên trái (hiển thị \"GV: <tên>\").",
                "Bước 2: Bấm nút \"Đăng xuất\" ngay bên dưới tên giảng viên.",
                "Bước 3: Nếu màn hình hiện tại có thay đổi chưa lưu, hệ thống sẽ hỏi xác nhận bỏ thay đổi trước.",
                "Bước 4: Chọn \"Yes\" (Có) trong hộp thoại \"Xác nhận đăng xuất\" để xóa phiên hiện tại và quay về màn hình \"Chọn vai trò\"."
            ],
            Note = "Dữ liệu minh họa đã thêm/sửa trong bộ nhớ vẫn được giữ nguyên trong suốt lần chạy ứng dụng cho tới khi đóng chương trình."
        },

        // ==================== 2. QUẢN TRỊ ====================
        new HelpTopic
        {
            Id = "admin-subjects",
            Group = "Quản trị",
            Title = "Thêm/sửa môn và chương",
            Keywords = "mon hoc, chuong, them mon, sua mon, xoa mon, gv-02, ctrl+2",
            Steps =
            [
                "Bước 1: Bấm mục \"Môn và chương\" ở nhóm Quản trị trên menu trái (hoặc nhấn phím tắt Ctrl+2).",
                "Bước 2: Để thêm hoặc sửa môn học, dùng nút \"Thêm môn\" hoặc \"Sửa\" (hoặc nhấp đúp vào dòng môn học) ở bảng bên trái; nhập Mã môn * (2–10 ký tự in hoa/số, không trùng) và Tên môn *, sau đó nhấn Ctrl+S để lưu.",
                "Bước 3: Chọn một môn học ở bảng bên trái để xem danh sách chương tương ứng ở bảng bên phải.",
                "Bước 4: Dùng các nút \"Thêm chương\", \"Sửa\", \"Xóa\" ở bảng bên phải để quản lý chương của môn đang chọn (Chương số * từ 1 đến 50, không trùng trong cùng môn)."
            ],
            Note = "Hệ thống không cho phép xóa môn học đang còn chương hoặc câu hỏi, và không cho phép xóa chương đang còn câu hỏi. Hãy chuyển hoặc xóa các câu hỏi liên quan trong \"Ngân hàng câu hỏi\" trước."
        },
        new HelpTopic
        {
            Id = "admin-classes",
            Group = "Quản trị",
            Title = "Quản lý lớp và sinh viên",
            Keywords = "lop hoc, sinh vien, mssv, nien khoa, si so, chuyen lop, gv-03, ctrl+3",
            Steps =
            [
                "Bước 1: Bấm mục \"Lớp và sinh viên\" trên menu trái (hoặc nhấn phím tắt Ctrl+3).",
                "Bước 2: Ở cột trái \"Lớp\", bấm \"Thêm lớp\" hoặc \"Sửa\" để cập nhật Mã lớp *, Tên lớp * và Niên khóa * (định dạng YYYY-YYYY, năm kết thúc lớn hơn năm bắt đầu).",
                "Bước 3: Chọn một lớp ở cột trái để hiển thị danh sách sinh viên của lớp đó ở cột phải. Dùng ô tìm kiếm (Ctrl+F) để lọc nhanh theo MSSV hoặc họ và tên.",
                "Bước 4: Bấm \"Thêm sinh viên\" hoặc nhấp đúp vào một sinh viên để sửa thông tin (MSSV * gồm 8–10 chữ số không trùng toàn hệ thống, Họ và tên *, Lớp *). Khi đổi Lớp trong hộp thoại Sửa, sĩ số hai lớp sẽ tự động cập nhật."
            ],
            Note = "Không thể xóa lớp học đang còn sinh viên. Chức năng \"Nhập từ tệp…\" được thực hiện tại màn hình \"Xuất/nhập và sao lưu\" (GV-15 – Bạn 2 phụ trách)."
        },
        new HelpTopic
        {
            Id = "admin-question-add",
            Group = "Quản trị",
            Title = "Thêm câu hỏi",
            Keywords = "them cau hoi, soan cau hoi, ngan hang cau hoi, dap an, do kho, gv-04, gv-05, ctrl+4",
            Steps =
            [
                "Bước 1: Bấm mục \"Ngân hàng câu hỏi\" trên menu trái (Ctrl+4) rồi bấm nút \"Thêm câu hỏi\" (hoặc bấm \"Thêm câu hỏi\" tại mục Thao tác nhanh của màn hình Tổng quan).",
                "Bước 2: Trong hộp thoại soạn câu hỏi, chọn Môn *, Chương * (1–20) và Độ khó * (Dễ / Trung bình / Khó).",
                "Bước 3: Nhập Nội dung câu hỏi * và đầy đủ 4 phương án trả lời A, B, C, D * (nội dung 4 phương án không được trùng nhau).",
                "Bước 4: Đánh dấu tròn \"Đúng\" tại đúng 1 phương án chuẩn, sau đó bấm \"Lưu\" hoặc nhấn Ctrl+S."
            ],
            Note = "Bạn có thể bấm nút \"Xem trước\" ngay trong hộp thoại soạn thảo để kiểm tra giao diện hiển thị câu hỏi trước khi lưu."
        },
        new HelpTopic
        {
            Id = "admin-question-edit",
            Group = "Quản trị",
            Title = "Sửa/xóa câu hỏi",
            Keywords = "sua cau hoi, xoa cau hoi, tim kiem cau hoi, bo loc, delete, gv-04",
            Steps =
            [
                "Bước 1: Tại màn hình \"Ngân hàng câu hỏi\" (Ctrl+4), sử dụng bộ lọc Môn học, Chương, Độ khó hoặc ô tìm kiếm từ khóa (Ctrl+F) để tìm câu hỏi.",
                "Bước 2: Chọn dòng câu hỏi trên bảng rồi bấm nút \"Sửa\" để cập nhật nội dung, phương án hoặc đáp án đúng.",
                "Bước 3: Để xóa câu hỏi đang chọn, bấm nút \"Xóa\" hoặc nhấn phím Delete trên bàn phím, sau đó chọn \"Yes\" trong hộp thoại xác nhận."
            ],
            Note = "Nếu bạn đang sửa dở câu hỏi trong hộp thoại mà bấm \"Hủy\" hoặc nhấn phím Esc, hệ thống sẽ hỏi xác nhận trước khi bỏ các thay đổi chưa lưu."
        },
        new HelpTopic
        {
            Id = "admin-question-preview",
            Group = "Quản trị",
            Title = "Xem trước câu hỏi",
            Keywords = "xem truoc cau hoi, preview, dap an dung, gv-06",
            Steps =
            [
                "Bước 1: Tại bảng \"Ngân hàng câu hỏi\", chọn một câu hỏi và bấm nút \"Xem trước\" (hoặc nhấp đúp chuột vào dòng câu hỏi).",
                "Bước 2: Kiểm tra bố cục câu hỏi và 4 phương án A, B, C, D dưới góc nhìn của sinh viên dự thi.",
                "Bước 3: Bật/tắt ô kiểm \"Hiển thị đáp án đúng (chỉ dành cho giảng viên)\" để kiểm tra phương án đúng được tô nổi bật.",
                "Bước 4: Có thể bấm nút \"Sửa câu hỏi này\" ngay trên hộp thoại xem trước để chuyển sang chế độ chỉnh sửa."
            ],
            Note = "Trong khu vực thi của sinh viên, đáp án đúng luôn được ẩn hoàn toàn để đảm bảo tính bảo mật của kỳ thi."
        },

        // ==================== 3. THI ====================
        new HelpTopic
        {
            Id = "exam-paper",
            Group = "Thi",
            Title = "Tạo đề thi",
            Keywords = "tao de thi, ma tran de, phat hanh de, gv-07, gv-08, gv-09",
            Steps =
            [
                "Bước 1: Bấm mục \"Đề thi\" trong nhóm \"Thi\" trên thanh menu bên trái.",
                "Bước 2: Chọn môn học, thời gian làm bài và cấu hình số lượng câu hỏi theo từng mức độ khó (Dễ, Trung bình, Khó).",
                "Bước 3: Kiểm tra số lượng câu hỏi khả dụng trong ngân hàng, xem trước đề và phát hành đề thi cho ca thi."
            ],
            Note = "Chi tiết thao tác sẽ được cập nhật khi hoàn thiện màn hình Đề thi (GV-07 → GV-09)."
        },
        new HelpTopic
        {
            Id = "exam-session",
            Group = "Thi",
            Title = "Tạo ca thi",
            Keywords = "tao ca thi, mo ca thi, phong thi, danh sach du thi, gv-10, gv-11",
            Steps =
            [
                "Bước 1: Bấm mục \"Ca thi\" trong nhóm \"Thi\" trên thanh menu bên trái.",
                "Bước 2: Tạo ca thi mới gắn với đề thi đã phát hành, thiết lập mã ca, tên ca, phòng thi và khung giờ mở ca.",
                "Bước 3: Mở hoặc đóng ca thi để cho phép sinh viên trong danh sách bắt đầu vào làm bài."
            ],
            Note = "Chi tiết do phân hệ phụ trách cập nhật (Bạn 3 – Quản lý ca thi GV-10 → GV-11)."
        },
        new HelpTopic
        {
            Id = "exam-result",
            Group = "Thi",
            Title = "Xem kết quả và phúc khảo",
            Keywords = "xem ket qua, thong ke, phuc khao, pho diem, sao luu, gv-12, gv-13, gv-14, gv-15",
            Steps =
            [
                "Bước 1: Bấm mục \"Kết quả và phúc khảo\" trong nhóm \"Thi\" trên thanh menu bên trái.",
                "Bước 2: Lọc kết quả theo ca thi hoặc lớp học để xem bảng điểm, số câu đúng và trạng thái nộp bài của từng sinh viên.",
                "Bước 3: Chọn một bài thi để đối chiếu chi tiết từng câu trả lời (phúc khảo chỉ đọc) hoặc xem biểu đồ phổ điểm của ca thi."
            ],
            Note = "Chi tiết do phân hệ phụ trách cập nhật (Bạn 2 – Kết quả, phúc khảo và phổ điểm GV-12 → GV-14)."
        },

        // ==================== 4. SINH VIÊN ====================
        new HelpTopic
        {
            Id = "student-session",
            Group = "Sinh viên",
            Title = "Chọn ca thi",
            Keywords = "sinh vien chon ca thi, nhan dien, huong dan truoc khi thi, sv-01, sv-02",
            Steps =
            [
                "Bước 1: Từ màn hình \"Chọn vai trò\", bấm \"Vào khu sinh viên\" (hoặc mục \"Mở UI sinh viên\" trên menu).",
                "Bước 2: Kiểm tra thông tin họ tên, MSSV, lớp học và chọn ca thi có trạng thái \"Đang mở\" bằng cách bấm nút \"Xem hướng dẫn →\".",
                "Bước 3: Đọc kỹ quy chế phòng thi và cách tính điểm tại màn hình \"Hướng dẫn trước khi thi\".",
                "Bước 4: Tích vào ô \"Tôi đã đọc và hiểu hướng dẫn làm bài\" rồi bấm \"Bắt đầu làm bài →\"."
            ],
            Note = "Các ca thi ở trạng thái \"Chưa đến giờ\" sẽ hiển thị thông báo khung giờ mở ca và chưa cho phép bấm vào thi."
        },
        new HelpTopic
        {
            Id = "student-attempt",
            Group = "Sinh viên",
            Title = "Làm bài và gắn cờ",
            Keywords = "lam bai thi, chon dap an, gan co xem lai, luoi cau hoi, dong ho dem nguoc, sv-03",
            Steps =
            [
                "Bước 1: Trong màn hình làm bài, đọc nội dung câu hỏi và chọn 1 trong 4 phương án A, B, C hoặc D (có thể đổi hoặc bỏ lựa chọn trước khi nộp).",
                "Bước 2: Nếu muốn đánh dấu câu hỏi để kiểm tra lại sau, bật nút \"Gắn cờ xem lại\" trên câu hỏi đó.",
                "Bước 3: Sử dụng nút \"Câu trước\" / \"Câu tiếp\" hoặc bấm trực tiếp vào số thứ tự câu trên bảng lưới bên phải để di chuyển nhanh giữa các câu."
            ],
            Note = "Lưới câu hỏi phân biệt rõ trạng thái Chưa trả lời, Đã trả lời, Đang xem và Đã gắn cờ bằng cả màu sắc lẫn ký hiệu/viền."
        },
        new HelpTopic
        {
            Id = "student-submit",
            Group = "Sinh viên",
            Title = "Nộp bài",
            Keywords = "nop bai, ket thuc bai thi, het gio tu nop, xem diem, sv-04, sv-05, sv-06",
            Steps =
            [
                "Bước 1: Sau khi hoàn thành các câu hỏi, bấm nút \"Nộp bài\" ở khu vực điều hướng bài thi.",
                "Bước 2: Kiểm tra lại số câu đã trả lời, số câu còn bỏ trống và số câu đang gắn cờ trong hộp thoại xác nhận nộp bài.",
                "Bước 3: Bấm xác nhận nộp bài để hệ thống khóa bài làm, chấm điểm và hiển thị kết quả sau khi lưu thành công."
            ],
            Note = "Phím Esc không bao giờ tự nộp bài thi. Khi đồng hồ đếm ngược về 00:00, hệ thống sẽ tự động khóa vùng làm bài và nộp bài một lần duy nhất."
        },

        // ==================== 5. PHÍM TẮT ====================
        new HelpTopic
        {
            Id = "shortcuts-all",
            Group = "Phím tắt",
            Title = "Bảng tổng hợp phím tắt",
            Keywords = "phim tat, ban phim, ctrl+f, ctrl+s, delete, esc, enter, ctrl+1, ctrl+2, ctrl+3, ctrl+4, f1",
            Steps =
            [
                "• Ctrl+1 : Mở nhanh màn hình \"Tổng quan\" (GV-01).",
                "• Ctrl+2 : Mở nhanh màn hình \"Môn và chương\" (GV-02).",
                "• Ctrl+3 : Mở nhanh màn hình \"Lớp và sinh viên\" (GV-03).",
                "• Ctrl+4 : Mở nhanh màn hình \"Ngân hàng câu hỏi\" (GV-04).",
                "• F1     : Mở màn hình \"Trợ giúp\" (HELP-01) từ bất kỳ vị trí nào.",
                "• Ctrl+F : Đưa con trỏ vào ô tìm kiếm tại màn hình hiện tại (Môn và chương, Lớp và sinh viên, Ngân hàng câu hỏi, Trợ giúp).",
                "• Ctrl+S : Lưu dữ liệu trong các hộp thoại Thêm/Sửa (Môn học, Chương, Lớp học, Sinh viên, Câu hỏi).",
                "• Delete : Xóa dòng đang chọn trên bảng (luôn có hộp thoại kiểm tra ràng buộc và xác nhận trước khi xóa).",
                "• Enter  : Kích hoạt nút hành động chính (Đăng nhập, Lưu trong hộp thoại).",
                "• Esc    : Đóng hoặc hủy hộp thoại đang mở (hỏi xác nhận nếu có thay đổi chưa lưu; không tự nộp bài thi)."
            ],
            Note = "Bạn cũng có thể nhấp đúp chuột (Double-click) vào một dòng trên bảng Môn học, Chương, Lớp, Sinh viên hoặc Câu hỏi để mở nhanh hộp thoại Sửa / Xem trước."
        }
    ];

    public static string NormalizeForSearch(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var decomposed = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var ch in decomposed)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(ch switch
            {
                'đ' => 'd',
                'Đ' => 'd',
                _ => ch
            });
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
