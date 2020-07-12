using Dashboard.Components.Containers;
using Dashboard.Config;
using Dashboard.Utilities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Dashboard.Components
{
    public class WindowContainer : DashboardContainer
    {
        private bool initialized = false;

        public Action<object> WindowInitialize { get; private set; }

        public Action<object> LostFocus { get; private set; }

        [PersistentConfig]
        public bool Autostart { get; set; } = true;

        [PersistentConfig]
        public HotKey HotKey { get; set; } = new HotKey();

        private WindowBlur.BlurType blurType = WindowBlur.BlurType.Acrylic;
        [PersistentConfig]
        public WindowBlur.BlurType BlurType { get => blurType; set => SetAndNotify(ref blurType, value); }

        private ColorScheme colorScheme = new ColorScheme();
        [PersistentConfig]
        public ColorScheme ColorScheme { get => colorScheme; set => SetAndNotify(ref colorScheme, value); }

        private double backgroundOpacity = 0.4d;
        [PersistentConfig]
        public double BackgroundOpacity { get => backgroundOpacity; set => SetAndNotify(ref backgroundOpacity, value); }

        private RelayCommand showWindowCommand;

        public ICommand ShowWindowCommand
        {
            get
            {
                return showWindowCommand ??= new RelayCommand(
                    // execute
                    () =>
                    {
                        ThisForeground = true;
                    },
                    // can execute
                    () => !ThisForeground
                );
            }
        }

        private RelayCommand quitAppCommand;

        public ICommand QuitAppCommand
        {
            get
            {
                return quitAppCommand ??= new RelayCommand(
                    // execute
                    () =>
                    {
                        Application.Current.Shutdown();
                    },
                    // can execute
                    () => true
                );
            }
        }

        private KeyboardHook keyHook = new KeyboardHook();

        public WindowContainer()
        {
            ThisForeground = false;

            WindowInitialize = _ =>
            {
                if (initialized) return;
                initialized = true;

                if (Autostart)
                    AutoRun.Register();
                else
                    AutoRun.Unregister();

                keyHook.RegisterHotKey(HotKey.ModifierKeys, HotKey.Key);
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
