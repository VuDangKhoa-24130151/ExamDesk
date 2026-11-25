using System.Runtime.InteropServices;

namespace ExamDesk.UI.Student;

// Chỉ theo dõi khi có màn làm bài; không đọc nội dung bàn phím hay clipboard.
internal sealed class ExamViolationMonitor : IDisposable
{
    private readonly Form _owner;
    private readonly Action<string> _report;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 150 };
    private readonly HookProc _callback;
    private readonly HashSet<int> _pressed = [];
    private nint _hook;
    private bool _wasInside = true;
    private bool _paused;
    private bool _disposed;

    internal ExamViolationMonitor(Form owner, Action<string> report)
    {
        _owner = owner;
        _report = report;
        _callback = KeyboardHook;
        _hook = SetWindowsHookEx(13, _callback, GetModuleHandle(null), 0);
        if (_hook == 0) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        _timer.Tick += (_, _) => CheckForeground();
        _timer.Start();
    }

    internal void Pause()
    {
        _paused = true;
        _timer.Stop();
    }

    internal void Resume()
    {
        if (_disposed) return;
        // Lấy lại cửa sổ hiện tại, tránh tính việc đóng cảnh báo là rời bài.
        _wasInside = IsInside();
        _paused = false;
        _timer.Start();
    }

    private bool IsInside()
    {
        var foreground = GetForegroundWindow();
        // Dialog xác nhận nộp/lỗi/cảnh cáo của chính cửa sổ thi không phải rời bài.
        for (var window = foreground; window != 0; window = GetWindow(window, 4))
            if (window == _owner.Handle) return true;
        return false;
    }

    private void CheckForeground()
    {
        if (_paused || _disposed) return;
        bool inside = IsInside();
        bool left = _wasInside && !inside;
        _wasInside = inside;
        if (left) _report("Rời khỏi cửa sổ làm bài thi");
    }

    private nint KeyboardHook(int code, nint message, nint data)
    {
        if (code >= 0 && !_disposed)
        {
            int key = Marshal.ReadInt32(data);
            bool down = message == 0x100 || message == 0x104;
            bool up = message == 0x101 || message == 0x105;
            if (up) _pressed.Remove(key);
            bool first = down && _pressed.Add(key);
            bool capture = key == (int)Keys.PrintScreen ||
                (key == (int)Keys.S && IsDown(Keys.ShiftKey) &&
                    (IsDown(Keys.LWin) || IsDown(Keys.RWin)));
            if (down && capture && IsInside())
            {
                if (first && !_paused)
                    _owner.BeginInvoke((Action)(() =>
                    {
                        if (!_disposed && !_paused) _report("Sử dụng phím chụp màn hình");
                    }));
                // Chặn phím chụp để cùng một thao tác không mở Snipping Tool rồi bị đếm hai lần.
                return 1;
            }
        }
        return CallNextHookEx(_hook, code, message, data);
    }

    private static bool IsDown(Keys key) => (GetAsyncKeyState((int)key) & 0x8000) != 0;

    public void Dispose()
    {
        _disposed = true;
        _timer.Dispose();
        if (_hook != 0) { UnhookWindowsHookEx(_hook); _hook = 0; }
    }

    private delegate nint HookProc(int code, nint message, nint data);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int id, HookProc callback, nint module, uint thread);
    [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(nint hook);
    [DllImport("user32.dll")] private static extern nint CallNextHookEx(nint hook, int code, nint message, nint data);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    [DllImport("user32.dll")] private static extern nint GetForegroundWindow();
    [DllImport("user32.dll")] private static extern nint GetWindow(nint window, uint command);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern nint GetModuleHandle(string? name);
}
