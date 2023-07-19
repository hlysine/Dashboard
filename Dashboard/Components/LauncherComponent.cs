using Dashboard.Config;
using Dashboard.Services;
using Dashboard.Utilities;
using System.Windows.Input;

namespace Dashboard.Components;

public class LauncherComponent : DashboardComponent
{
    public override string DefaultName => "Run";

    [RequireService(nameof(SystemServiceId))]
    private SystemService System { get; set; }

    [PersistentConfig]
    public string SystemServiceId { get; set; }

    private string prompt;
    public string Prompt
    {
        get => prompt;
        set => SetAndNotify(ref prompt, value);
    }

    private string errorMessage;
    public string ErrorMessage
    {
        get => errorMessage;
        set => SetAndNotify(ref errorMessage, value);
    }

    private RelayCommand launchCommand;

    public ICommand LaunchCommand => launchCommand ?? (launchCommand = new RelayCommand(
        // execute
        () =>
        {
            ErrorMessage = System.Run(Prompt);
            Prompt = "";
        },
        // can execute
        () =>
        {
            return true;
        }
    ));

    public LauncherComponent()
    {
        Loaded = true;
    }
}