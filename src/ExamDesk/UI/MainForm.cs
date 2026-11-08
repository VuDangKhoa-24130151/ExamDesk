using ExamDesk.UI.Administration;
using ExamDesk.UI.ExamManagement;
using ExamDesk.UI.Reports;
using ExamDesk.UI.DataManagement;
using ExamDesk.UI.Student;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI;

// File chung: Bạn 1 điều phối. Đây là cửa sổ tích hợp UI, chưa phải đăng nhập/phân quyền.
public sealed class MainForm : Form
{
    private static readonly Font ActiveMenuFont = new("Segoe UI", 10F, FontStyle.Bold);
    private static readonly Font GroupHeaderFont = new("Segoe UI", 8.5F, FontStyle.Bold);
    private static readonly Color MenuHoverColor = ColorTranslator.FromHtml("#1E4276");
    private static readonly Color SidebarMutedTextColor = ControlPaint.Light(UiStyle.TextMuted, 0.7F);
    private static readonly Color SidebarDividerColor = ColorTranslator.FromHtml("#234575");

    private readonly Panel _content = new() { Dock = DockStyle.Fill, Padding = new Padding(0) };
    private readonly FlowLayoutPanel _menu;
    private readonly List<Button> _menuButtons = [];
    private readonly ToolTip _toolTip = new();

    private readonly Label _lblLoginStatus;
    private readonly Button _btnLogout;
    private readonly Label _lblBottomNote;

    private readonly Button _roleSelectionButton;
    private readonly Button _dashboardButton;
    private readonly Button _subjectChapterButton;
    private readonly Button _classStudentButton;
    private readonly Button _questionBankButton;
    private readonly Button _examPaperButton;
    private readonly Button _helpButton;

    private Button? _activeMenuButton;
    private string _loggedInUsername = string.Empty;
    private int _nextTabIndex;

    public MainForm()
    {
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        KeyPreview = true;
        Text = "UniExam — Khung tích hợp UI";
        ClientSize = new Size(1280, 680);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Background;
        ForeColor = UiStyle.Text;

        var sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 236,
            BackColor = UiStyle.Sidebar,
            ForeColor = Color.White
        };

        _menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 10, 10, 10),
            BackColor = UiStyle.Sidebar,
            ForeColor = Color.White,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        _menu.ClientSizeChanged += (_, _) => AdjustMenuChildWidths();

        var brandLabel = new Label
        {
            Text = "UniExam",
            Font = UiStyle.TitleFont,
            ForeColor = Color.White,
            BackColor = UiStyle.Sidebar,
            AutoSize = false,
            Width = 200,
            Height = 42,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0),
            Margin = new Padding(0, 2, 0, 8)
        };
        _menu.Controls.Add(brandLabel);

        // 1. Chọn vai trò
        _roleSelectionButton = AddMenu(_menu, "Chọn vai trò", CreateRoleSelectionScreen);

        // 2. Nhóm "Quản trị"
        AddMenuGroupHeader(_menu, "Quản trị");
        _dashboardButton = AddMenu(_menu, "Tổng quan", () => CreateDashboardScreen(), "Ctrl+1");
        _subjectChapterButton = AddMenu(_menu, "Môn và chương", () => new SubjectChapterControl(), "Ctrl+2");
        _classStudentButton = AddMenu(_menu, "Lớp và sinh viên", () => new ClassStudentControl(), "Ctrl+3");
        _questionBankButton = AddMenu(_menu, "Ngân hàng câu hỏi", () => new QuestionBankControl(), "Ctrl+4");

        // 3. Nhóm "Thi"
        AddMenuGroupHeader(_menu, "Thi");
        _examPaperButton = AddMenu(_menu, "Đề thi", () => new ExamPaperControl());
        AddMenu(_menu, "Ca thi", () => new SessionManagementControl());
        AddMenu(_menu, "Kết quả và phúc khảo", () => new GradingReviewControl());
        AddMenu(_menu, "Xuất/nhập và sao lưu", () => new DataManagementControl());

        // 4. Trợ giúp
        AddMenuSeparator(_menu);
        _helpButton = AddMenu(_menu, "Trợ giúp", () => new HelpControl(), "F1");

        // 5. Nhóm "Công cụ dựng UI"
        AddMenuGroupHeader(_menu, "Công cụ dựng UI");
        AddActionMenu(_menu, "Mở UI đăng nhập GV", "Đăng nhập giảng viên", OpenLoginDialogTool);
        AddActionMenu(_menu, "Mở UI sinh viên", "Khu vực sinh viên", OpenStudentDialogTool);

        // Vùng trạng thái đăng nhập ở cuối sidebar
        var authFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            Padding = new Padding(12, 10, 12, 12),
            BackColor = UiStyle.Sidebar
        };

        var footerDivider = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = SidebarDividerColor
        };

        var authLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiStyle.Sidebar,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0)
        };
        authLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        authLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        authLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _lblLoginStatus = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Font = UiStyle.SmallFont,
            ForeColor = SidebarMutedTextColor,
            BackColor = UiStyle.Sidebar,
            Margin = new Padding(4, 0, 4, 6),
            Text = "Chưa đăng nhập – chế độ xem UI"
        };

        _btnLogout = new Button
        {
            Text = "Đăng xuất",
            Dock = DockStyle.Top,
            Height = 34,
            Margin = new Padding(0, 2, 0, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = MenuHoverColor,
            ForeColor = Color.White,
            Font = UiStyle.SmallFont,
            Cursor = Cursors.Hand,
            Visible = false,
            TabStop = true,
            TabIndex = _nextTabIndex++
        };
        _btnLogout.FlatAppearance.BorderSize = 1;
        _btnLogout.FlatAppearance.BorderColor = SidebarDividerColor;
        _btnLogout.FlatAppearance.MouseOverBackColor = UiStyle.Primary;
        _btnLogout.Click += OnLogoutClick;

        authLayout.Controls.Add(_lblLoginStatus, 0, 0);
        authLayout.Controls.Add(_btnLogout, 0, 1);
        authFooter.Controls.Add(authLayout);
        authFooter.Controls.Add(footerDivider);

        sidebarPanel.Controls.Add(_menu);
        sidebarPanel.Controls.Add(authFooter);

        _lblBottomNote = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 32,
            AutoEllipsis = true,
            Padding = new Padding(16, 0, 16, 0),
            Font = UiStyle.SmallFont,
            ForeColor = UiStyle.TextMuted,
            BackColor = UiStyle.Background,
<<<<<<< HEAD
            TextAlign = ContentAlignment.MiddleLeft
=======
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "CHẾ ĐỘ DỰNG UI • Quản lý ca đã lưu SQLite; các phân hệ khác đang dùng dữ liệu demo."
>>>>>>> d4ca63f48c4afdb6294511f56159ceb56920be5d
        };

        Controls.Add(_content);
        Controls.Add(sidebarPanel);
        Controls.Add(_lblBottomNote);

        UpdateLoginStatusUi();
        AdjustMenuChildWidths();
        TryOpenScreen("Chọn vai trò", CreateRoleSelectionScreen, _roleSelectionButton);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UiStyle.ClampToWorkingArea(this);
        AdjustMenuChildWidths();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.D1:
            case Keys.Control | Keys.NumPad1:
                _dashboardButton.PerformClick();
                return true;

            case Keys.Control | Keys.D2:
            case Keys.Control | Keys.NumPad2:
                _subjectChapterButton.PerformClick();
                return true;

            case Keys.Control | Keys.D3:
            case Keys.Control | Keys.NumPad3:
                _classStudentButton.PerformClick();
                return true;

            case Keys.Control | Keys.D4:
            case Keys.Control | Keys.NumPad4:
                _questionBankButton.PerformClick();
                return true;

            case Keys.F1:
                _helpButton.PerformClick();
                return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!CanLeaveCurrentScreen())
        {
            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
    }

    private UserControl CreateRoleSelectionScreen()
    {
        var roleControl = new RoleSelectionControl();
        roleControl.LecturerLoggedIn += (_, _) =>
        {
            HandleLecturerLoggedIn(roleControl.LoggedInUsername);
        };
        roleControl.OpenHelpRequested += (_, _) =>
        {
            _helpButton.PerformClick();
        };
        return roleControl;
    }

    private DashboardControl CreateDashboardScreen(string? overrideLecturerName = null)
    {
        var effectiveName = overrideLecturerName ??
                            (string.IsNullOrWhiteSpace(_loggedInUsername) ? null : _loggedInUsername);
        var dashboard = new DashboardControl
        {
            LecturerName = effectiveName
        };
        dashboard.NavigateRequested += OnDashboardNavigateRequested;
        return dashboard;
    }

    private void OnDashboardNavigateRequested(object? sender, string key)
    {
        switch (key)
        {
            case "questions":
                _questionBankButton.PerformClick();
                break;

            case "subjects":
                _subjectChapterButton.PerformClick();
                break;

            case "classes":
                _classStudentButton.PerformClick();
                break;

            case "exams":
                _examPaperButton.PerformClick();
                break;

            case "question-new":
                TryOpenScreen(
                    "Ngân hàng câu hỏi",
                    () => new QuestionBankControl(),
                    _questionBankButton,
                    screen =>
                    {
                        if (screen is QuestionBankControl questionBank)
                        {
                            BeginInvoke(() =>
                            {
                                try
                                {
                                    questionBank.StartAddQuestion();
                                }
                                catch (Exception ex)
                                {
                                    ReplaceContentScreen(CreateErrorScreen("Thêm câu hỏi", ex.Message));
                                }
                            });
                        }
                    });
                break;
        }
    }

    private void HandleLecturerLoggedIn(string? username)
    {
        var resolvedUsername = string.IsNullOrWhiteSpace(username) ? "gv01" : username.Trim();
        if (TryOpenScreen("Tổng quan", () => CreateDashboardScreen(resolvedUsername), _dashboardButton))
        {
            _loggedInUsername = resolvedUsername;
            UpdateLoginStatusUi();
        }
    }

    private void OpenLoginDialogTool()
    {
        using var form = new LoginForm();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            HandleLecturerLoggedIn(form.LoggedInUsername);
        }
    }

    private void OpenStudentDialogTool()
    {
        using var form = new StudentExamForm();
        form.ShowDialog(this);
    }

    private void OnLogoutClick(object? sender, EventArgs e)
    {
        if (!CanLeaveCurrentScreen())
        {
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Bạn có chắc chắn muốn đăng xuất khỏi tài khoản \"{_loggedInUsername}\"?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _loggedInUsername = string.Empty;
        UpdateLoginStatusUi();
        TryOpenScreen("Chọn vai trò", CreateRoleSelectionScreen, _roleSelectionButton, skipLeaveCheck: true);
    }

    private void UpdateLoginStatusUi()
    {
        var isLoggedIn = !string.IsNullOrWhiteSpace(_loggedInUsername);
        if (isLoggedIn)
        {
            _lblLoginStatus.Text = $"GV: {_loggedInUsername}";
            _lblLoginStatus.ForeColor = Color.White;
            _lblLoginStatus.Font = ActiveMenuFont;
            _btnLogout.Visible = true;
            _lblBottomNote.Text =
                $"CHẾ ĐỘ DỰNG UI • Đang đăng nhập minh họa: GV {_loggedInUsername} (chưa kết nối SQLite).";
        }
        else
        {
            _lblLoginStatus.Text = "Chưa đăng nhập – chế độ xem UI";
            _lblLoginStatus.ForeColor = SidebarMutedTextColor;
            _lblLoginStatus.Font = UiStyle.SmallFont;
            _btnLogout.Visible = false;
            _lblBottomNote.Text =
                "CHẾ ĐỘ DỰNG UI • Chưa đăng nhập – chế độ xem UI (các màn hình mở tự do để dựng giao diện, chưa kết nối SQLite).";
        }
    }

    private void AddMenuGroupHeader(FlowLayoutPanel menu, string title)
    {
        var label = new Label
        {
            Text = title,
            Font = GroupHeaderFont,
            ForeColor = SidebarMutedTextColor,
            BackColor = UiStyle.Sidebar,
            AutoSize = false,
            Width = 200,
            Height = 22,
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(8, 0, 4, 0),
            Margin = new Padding(0, 8, 0, 2)
        };
        menu.Controls.Add(label);
    }

    private static void AddMenuSeparator(FlowLayoutPanel menu)
    {
        var line = new Panel
        {
            Width = 200,
            Height = 1,
            BackColor = SidebarDividerColor,
            Margin = new Padding(0, 6, 0, 6)
        };
        menu.Controls.Add(line);
    }

    private Button AddMenu(FlowLayoutPanel menu, string text, Func<UserControl> createScreen, string? shortcutHint = null)
    {
        var button = CreateSidebarButton(text, shortcutHint);
        button.Click += (_, _) =>
        {
            TryOpenScreen(text, createScreen, button);
        };

        _menuButtons.Add(button);
        menu.Controls.Add(button);
        return button;
    }

    private Button AddActionMenu(FlowLayoutPanel menu, string text, string screenNameForError, Action action)
    {
        var button = CreateSidebarButton(text, shortcutHint: null);
        button.Click += (_, _) =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ReplaceContentScreen(CreateErrorScreen(screenNameForError, ex.Message));
            }
        };
        menu.Controls.Add(button);
        return button;
    }

    private Button CreateSidebarButton(string text, string? shortcutHint)
    {
        var button = new Button
        {
            Tag = text,
            Text = "   " + text,
            Width = 200,
            Height = 38,
            AutoEllipsis = true,
            Margin = new Padding(0, 0, 0, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = UiStyle.Sidebar,
            ForeColor = Color.White,
            Font = UiStyle.BodyFont,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(6, 0, 4, 0),
            Cursor = Cursors.Hand,
            TabStop = true,
            TabIndex = _nextTabIndex++
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = MenuHoverColor;
        button.FlatAppearance.MouseDownBackColor = UiStyle.Primary;

        if (!string.IsNullOrWhiteSpace(shortcutHint))
        {
            _toolTip.SetToolTip(button, $"{text} ({shortcutHint})");
        }
        else
        {
            _toolTip.SetToolTip(button, text);
        }

        return button;
    }

    private void AdjustMenuChildWidths()
    {
        var targetWidth = Math.Max(140, _menu.ClientSize.Width - _menu.Padding.Horizontal - 2);
        _menu.SuspendLayout();
        foreach (Control child in _menu.Controls)
        {
            if (child.Width != targetWidth)
            {
                child.Width = targetWidth;
            }
        }
        _menu.ResumeLayout();
    }

    private void SetActiveMenuButton(Button activeButton)
    {
        _activeMenuButton = activeButton;
        foreach (var btn in _menuButtons)
        {
            var baseText = btn.Tag as string ?? string.Empty;
            var isSelected = ReferenceEquals(btn, activeButton);
            btn.BackColor = isSelected ? UiStyle.Primary : UiStyle.Sidebar;
            btn.Font = isSelected ? ActiveMenuFont : UiStyle.BodyFont;
            btn.Text = isSelected ? $"▶ {baseText}" : $"   {baseText}";
        }
    }

    private bool CanLeaveCurrentScreen()
    {
        if (_content.Controls.Count > 0 &&
            _content.Controls[0] is IUnsavedChangesGuard guard &&
            guard.HasUnsavedChanges)
        {
            return guard.ConfirmLeave();
        }

        return true;
    }

    private bool TryOpenScreen(
        string screenName,
        Func<UserControl> createScreen,
        Button? menuButton = null,
        Action<UserControl>? afterShown = null,
        bool skipLeaveCheck = false)
    {
        if (!skipLeaveCheck && !CanLeaveCurrentScreen())
        {
            return false;
        }

        try
        {
            var screen = createScreen();
            ReplaceContentScreen(screen);
            if (menuButton is not null)
            {
                SetActiveMenuButton(menuButton);
            }
            else if (_activeMenuButton is not null)
            {
                SetActiveMenuButton(_activeMenuButton);
            }

            afterShown?.Invoke(screen);
            return true;
        }
        catch (Exception ex)
        {
            ReplaceContentScreen(CreateErrorScreen(screenName, ex.Message));
            if (menuButton is not null)
            {
                SetActiveMenuButton(menuButton);
            }
            return false;
        }
    }

    public bool ShowScreen(UserControl screen, Button? menuButton = null)
    {
        ArgumentNullException.ThrowIfNull(screen);

        // Hỏi xác nhận nếu màn hình hiện tại có dữ liệu đang nhập dở chưa lưu.
        if (!CanLeaveCurrentScreen())
        {
            screen.Dispose();
            return false;
        }

        var screenName = menuButton?.Tag as string ?? screen.Name;
        try
        {
            ReplaceContentScreen(screen);
            if (menuButton is not null)
            {
                SetActiveMenuButton(menuButton);
            }
            else if (_activeMenuButton is not null)
            {
                SetActiveMenuButton(_activeMenuButton);
            }

            return true;
        }
        catch (Exception ex)
        {
            screen.Dispose();
            ReplaceContentScreen(CreateErrorScreen(screenName, ex.Message));
            return false;
        }
    }

    private void ReplaceContentScreen(UserControl screen)
    {
        while (_content.Controls.Count > 0)
        {
            var previous = _content.Controls[0];
            _content.Controls.Remove(previous);
            previous.Dispose();
        }

        screen.Dock = DockStyle.Fill;
        _content.Controls.Add(screen);
    }

    internal static UserControl CreateErrorScreen(string screenName, string errorMessage)
    {
        var displayScreenName = string.IsNullOrWhiteSpace(screenName) ? "màn hình" : screenName.Trim();
        var detail = string.IsNullOrWhiteSpace(errorMessage) ? "Lỗi không xác định" : errorMessage.Trim();

        var control = new UserControl
        {
            AutoScaleDimensions = new SizeF(7F, 15F),
            AutoScaleMode = AutoScaleMode.Font,
            BackColor = UiStyle.Background,
            ForeColor = UiStyle.Text,
            Font = UiStyle.BodyFont,
            Dock = DockStyle.Fill
        };

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(24),
            BackColor = UiStyle.Background
        };
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var lblTitle = new Label
        {
            AutoSize = true,
            Font = UiStyle.TitleFont,
            ForeColor = UiStyle.Text,
            Text = displayScreenName,
            Margin = new Padding(0, 0, 0, 16)
        };

        var errorBorder = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(1),
            BackColor = UiStyle.ErrorText,
            Margin = Padding.Empty
        };

        var errorPanel = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(20, 16, 20, 16),
            BackColor = UiStyle.ErrorBack
        };

        var lblError = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Font = UiStyle.BodyFont,
            ForeColor = UiStyle.ErrorText,
            BackColor = UiStyle.ErrorBack,
            Margin = Padding.Empty,
            Text = $"Không mở được {displayScreenName}: {detail}. Báo cho bạn phụ trách."
        };

        errorPanel.SizeChanged += (_, _) =>
        {
            var maxWidth = Math.Max(200, errorPanel.ClientSize.Width - errorPanel.Padding.Horizontal);
            lblError.MaximumSize = new Size(maxWidth, 0);
        };

        errorPanel.Controls.Add(lblError);
        errorBorder.Controls.Add(errorPanel);

        rootLayout.Controls.Add(lblTitle, 0, 0);
        rootLayout.Controls.Add(errorBorder, 0, 1);
        control.Controls.Add(rootLayout);

        return control;
    }
}
