using System;
using System.Windows.Input;
using Dashboard.Config;
using Dashboard.Utilities;

namespace Dashboard.Components.Containers;

public class WindowContainer : DashboardContainer
{
    public override string DefaultName => "Dashboard";

    public Action<object> LostFocus { get; private set; }

    [PersistentConfig]
    public HotKey HotKey { get; set; } = new() { Key = Key.D, ModifierKeys = ModifierKeys.Alt };

    private WindowBlur.BlurType blurType = WindowBlur.BlurType.Acrylic;

    [PersistentConfig]
    public WindowBlur.BlurType BlurType { get => blurType; set => SetAndNotify(ref blurType, value); }

    private ColorScheme colorScheme = new();

    [PersistentConfig]
    public ColorScheme ColorScheme { get => colorScheme; set => SetAndNotify(ref colorScheme, value); }

    private double backgroundOpacity = 0.4d;

    [PersistentConfig]
    public double BackgroundOpacity { get => backgroundOpacity; set => SetAndNotify(ref backgroundOpacity, value); }

    private RelayCommand toggleWindowCommand;

    public ICommand ToggleWindowCommand
    {
        get
        {
            return toggleWindowCommand ??= new RelayCommand(
                // execute
                () =>
                {
                    ThisForeground = !ThisForeground;
                },
                // can execute
                () => true
            );
        }
    }

    public WindowContainer()
    {
        ThisForeground = false;

        LostFocus = _ =>
        {
            ThisForeground = false;
        };
    }
}