using ExamDesk.Models;
using ExamDesk.Repositories;
using Microsoft.Data.Sqlite;

namespace ExamDesk.Services;

// Bạn 3: kiểm tra dữ liệu quản lý ca trước khi lưu.
public sealed class ExamSessionService
{
    private readonly ExamSessionRepository _repository = new();
    public List<ExamPaper> GetExamPapers() => _repository.FindPapers();
    public List<ExamSession> GetSessions() => _repository.FindSessions();

    public int CreateSession(ExamSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.SessionCode = session.SessionCode.Trim();
        session.SessionName = session.SessionName.Trim();
        session.RoomName = session.RoomName.Trim();
        if (session.SessionCode.Length == 0 || session.SessionName.Length == 0 || session.RoomName.Length == 0)
            throw new ArgumentException("Vui lòng nhập mã ca, tên ca và phòng thi.");
        if (session.PaperId <= 0) throw new ArgumentException("Vui lòng chọn đề thi.");
        try { return _repository.InsertSession(session); }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        { throw new ExamSessionInputException("Mã ca đã tồn tại. Hãy dùng mã khác.", ex); }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 787)
        { throw new ExamSessionInputException("Đề thi không còn tồn tại. Hãy tải lại danh sách đề.", ex); }
    }

    public int CountInProgress(int sessionId) => _repository.CountInProgress(sessionId);

    internal int CloseSession(int sessionId, bool allowInProgress)
        => _repository.CloseSession(sessionId, allowInProgress);

    public void SetSessionOpen(int sessionId, bool isOpen)
    {
        if (sessionId <= 0) throw new ArgumentException("Vui lòng chọn ca thi.");
        _repository.SetSessionOpen(sessionId, isOpen);
    }
}

internal sealed class ExamSessionInputException(string message, Exception? inner = null)
    : InvalidOperationException(message, inner);
