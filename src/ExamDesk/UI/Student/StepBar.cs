using System.ComponentModel;
using ExamDesk.UI.Shared;

namespace ExamDesk.UI.Student;

public sealed class StepBar : UserControl
{
    private static readonly string[] StepNames =
        ["Nhận diện", "Chọn ca", "Hướng dẫn", "Làm bài", "Kết quả"];

    private readonly Label[] _steps = new Label[StepNames.Length];
    private int _currentStep = 1;

    public StepBar()
    {
        AutoScaleMode = AutoScaleMode.Font;
        Font = UiStyle.BodyFont;
        BackColor = UiStyle.Surface;
        Size = new Size(560, 40);
        TabStop = false;
        AccessibleName = "Các bước làm bài thi";

        var layout = new TableLayoutPanel
        {
            ColumnCount = StepNames.Length,
            RowCount = 1,
            Dock = DockStyle.Fill,
            TabStop = false
        };
        for (var index = 0; index < StepNames.Length; index++)
        {
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / StepNames.Length));
            var label = new Label
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 4, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false
            };
            _steps[index] = label;
            layout.Controls.Add(label, index, 0);
        }

        Controls.Add(layout);
        UpdateSteps();
    }

    // Đánh số từ 1 (Nhận diện) đến 5 (Kết quả).
    [DefaultValue(1)]
    public int CurrentStep
    {
        get => _currentStep;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, StepNames.Length);
            if (_currentStep == value)
            {
                return;
            }

            _currentStep = value;
            UpdateSteps();
        }
    }

    private void UpdateSteps()
    {
        for (var index = 0; index < _steps.Length; index++)
        {
            var label = _steps[index];
            var completed = index + 1 < _currentStep;
            var current = index + 1 == _currentStep;
            var name = StepNames[index];

            label.Text = completed ? $"✓ {name}" : name;
            label.ForeColor = completed ? UiStyle.SuccessText : current ? UiStyle.Primary : UiStyle.TextMuted;
            label.Font = current ? StudentLayout.GetLabelFont(10, FontStyle.Bold) : UiStyle.BodyFont;
            label.AccessibleName = completed ? $"{name}, đã hoàn thành"
                : current ? $"{name}, bước hiện tại" : $"{name}, chưa thực hiện";
        }
    }
}
