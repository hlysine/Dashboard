using Dashboard.Components.Containers;
using Dashboard.Config;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Dashboard.Components
{
    public class WindowContainer : DashboardContainer
    {
        private bool initialized = false;

        public Action<object> WindowInitialize { get; private set; }

        public Action<object> QuitApplication { get; private set; }

        public Action<object> LostFocus { get; private set; }

        [PersistentConfig]
        public bool Autostart { get; set; } = true;

        private RelayCommand showWindowCommand;

        public ICommand ShowWindowCommand
        {
            get
            {
                return showWindowCommand ?? (showWindowCommand = new RelayCommand(
                    // execute
                    () =>
                    {
                        ThisForeground = true;
                    },
                    // can execute
                    () => !ThisForeground
                ));
            }
        }

        private KeyboardHook keyHook = new KeyboardHook();

        public WindowContainer()
        {
            
            WindowInitialize = _ =>
            {
                if (initialized) return;
                initialized = true;

                // TODO: only register if user set autostart config
                if (Autostart)
                    AutoRun.Register();
                else
                    AutoRun.Unregister();

                keyHook.RegisterHotKey(Utilities.ModifierKeys.Alt, System.Windows.Forms.Keys.D);
                keyHook.KeyPressed += KeyHook_KeyPressed;

                //ClockComponent clock = new ClockComponent();
                //SpotifyComponent spotify = new SpotifyComponent(this);
                //GoogleCalendarComponent calendar = new GoogleCalendarComponent(this);
                //GoogleTasksComponent tasks = new GoogleTasksComponent(this);
                //GoogleGmailComponent gmail = new GoogleGmailComponent(this);
                //OsuComponent osu = new OsuComponent(this);
                //LauncherComponent launcher = new LauncherComponent(this);
                //Components.Add(clock);
                //Components.Add(spotify);
                //Components.Add(calendar);
                //Components.Add(tasks);
                //Components.Add(gmail);
                //Components.Add(osu);
                //Components.Add(launcher);
            };

            QuitApplication = _ =>
            {
                Application.Current.Shutdown();
            };

            LostFocus = _ =>
            {
                ThisForeground = false;
            };
        }

        private void KeyHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            ThisForeground = !ThisForeground;
        }
    }
}
