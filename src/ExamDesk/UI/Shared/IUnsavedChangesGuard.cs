namespace ExamDesk.UI.Shared;

// Bạn 1 điều phối. Các màn hình có thay đổi chưa lưu cài đặt interface này để MainForm hỏi trước khi rời.
public interface IUnsavedChangesGuard
{
    bool HasUnsavedChanges { get; }

    // Tự hiển thị hộp thoại xác nhận; trả về true nếu người dùng đồng ý bỏ thay đổi để rời màn hình.
    bool ConfirmLeave();
}
