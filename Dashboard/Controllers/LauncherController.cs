using Dashboard.Config;
using Dashboard.ServiceProviders;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Dashboard.Controllers
{
    public class LauncherController : DashboardController
    {
        [RequireService]
        private SystemService System { get; set; }

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

        public ICommand LaunchCommand
        {
            get
            {
                return launchCommand ?? (launchCommand = new RelayCommand(
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
            }
        }

        public LauncherController()
        {
            Loaded = true;
        }
    }
}
