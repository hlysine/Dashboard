using Dashboard.Components;
using Dashboard.Config;
using Dashboard.Controllers;
using Dashboard.ServiceProviders;
using Dashboard.Utilities;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Dashboard
{
    /// <summary>
    /// The view model for the main window. Also manages all components, controllers and service providers.
    /// </summary>
    public class ComponentManager : NotifyPropertyChanged
    {
        public ObservableCollection<DashboardController> Controllers { get; set; } = new ObservableCollection<DashboardController>();

        public ObservableCollection<UserControl> Components { get; set; } = new ObservableCollection<UserControl>();

        public ObservableCollection<ServiceProvider> Services { get; set; } = new ObservableCollection<ServiceProvider>();

        private bool windowShown = false;

        public bool WindowShown
        {
            get => windowShown;
            set => SetAndNotify(ref windowShown, value);
        }

        /// <summary>
        /// Action to be invoked when the main window is initialized.
        /// </summary>
        public Action<object> Initialize { get; private set; }

        public Action<object> QuitApplication { get; private set; }

        public Action<object> LostFocus { get; private set; }

        private RelayCommand showWindowCommand;

        public ICommand ShowWindowCommand
        {
            get
            {
                return showWindowCommand ?? (showWindowCommand = new RelayCommand(
                    // execute
                    () =>
                    {
                        WindowShown = true;
                    },
                    // can execute
                    () => !WindowShown
                ));
            }
        }

        private const string configPath = "config.xml";

        private XmlSerializer xmlSerializer;

        private KeyboardHook keyHook = new KeyboardHook();

        private bool initialized = false;

        private List<(DashboardController, DispatcherTimer)> refreshTimers = new List<(DashboardController, DispatcherTimer)>();

        public ComponentManager()
        {
            Initialize = _ =>
            {
                if (initialized) return;
                initialized = true;

                // TODO: only register if user set autostart config
                AutoRun.Register();

                keyHook.RegisterHotKey(Utilities.ModifierKeys.Alt, System.Windows.Forms.Keys.D);
                keyHook.KeyPressed += KeyHook_KeyPressed;

                ClockComponent clock = new ClockComponent(this);
                SpotifyComponent spotify = new SpotifyComponent(this);
                GoogleCalendarComponent calendar = new GoogleCalendarComponent(this);
                GoogleTasksComponent tasks = new GoogleTasksComponent(this);
                GoogleGmailComponent gmail = new GoogleGmailComponent(this);
                OsuComponent osu = new OsuComponent(this);
                LauncherComponent launcher = new LauncherComponent(this);
                Components.Add(clock);
                Components.Add(spotify);
                Components.Add(calendar);
                Components.Add(tasks);
                Components.Add(gmail);
                Components.Add(osu);
                Components.Add(launcher);

                foreach (var controller in Controllers)
                {
                    var timer = new DispatcherTimer() { Interval = controller.BackgroundRefreshRate, IsEnabled = false, Tag = controller };
                    timer.Tick += Timer_Tick;
                    refreshTimers.Add((controller, timer));

                    // Start after a random delay to avoid lag spikes
                    Task.Delay(Helper.Rnd.Next(10000)).ContinueWith(_ => timer.Start());
                }

                Controllers.ForEach(x => x.OnInitializationComplete());

                SaveConfig();
            };

            QuitApplication = _ =>
            {
                Application.Current.Shutdown();
            };

            LostFocus = _ =>
            {
                SetWindowState(false);
            };

            //Prepare an XmlSerializer to be used when saving config
            var configXmlOverrides = new XmlAttributeOverrides();
            var attributes = new XmlAttributes();
            attributes.XmlIgnore = true;
            var classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from assemblyType in domainAssembly.GetTypes()
                             where assemblyType.IsSubclassOf(typeof(ServiceProvider))
                             select assemblyType).ToArray();
            var props = new List<PropertyInfo>();
            foreach (Type serviceType in classList)
            {
                foreach (var prop in serviceType.GetProperties())
                {
                    if (!prop.IsDefined(typeof(PersistentConfigAttribute), true) && !props.Any(x => x.PropertyType == prop.PropertyType && x.Name == prop.Name))
                    {
                        props.Add(prop);
                        configXmlOverrides.Add(prop.DeclaringType, prop.Name, attributes);
                    }
                }
            }

            xmlSerializer = new XmlSerializer(typeof(ObservableCollection<ServiceProvider>), configXmlOverrides, classList, new XmlRootAttribute("DashboardConfig"), "");

            if (File.Exists(configPath.ToAbsolutePath()))
                LoadConfig();
        }

        private void SetWindowState(bool shown)
        {
            WindowShown = shown;
            foreach (var pair in refreshTimers)
            {
                pair.Item2.Interval = shown ? pair.Item1.ForegroundRefreshRate : pair.Item1.BackgroundRefreshRate;
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ((DashboardController)((DispatcherTimer)sender).Tag).OnRefresh();
        }

        private void KeyHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            SetWindowState(!WindowShown);
        }

        public ServiceProvider GetService(Type type)
        {
            ServiceProvider service = Services.FirstOrDefault(x => x.GetType() == type);
            if (service == null)
            {
                service = (ServiceProvider)Activator.CreateInstance(type);
                // fill in required services
                var properties = service.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsDefined(typeof(RequireServiceAttribute), true));
                properties.ForEach(x => x.SetValue(service, GetService(x.PropertyType)));
                Services.Add(service);
                service.ConfigUpdated += Service_ConfigUpdated;
            }
            return service;
        }

        private void Service_ConfigUpdated(object sender, EventArgs e)
        {
            SaveConfig();
        }

        public T GetService<T>() where T : ServiceProvider, new()
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        /// Get an instance of a <see cref="DashboardController"/>. Will return the same instance for duplicate calls.
        /// </summary>
        /// <typeparam name="T">A <see cref="DashboardController"/> type.</typeparam>
        /// <returns></returns>
        public T GetController<T>() where T : DashboardController, new()
        {
            DashboardController controller = Controllers.FirstOrDefault(x => x.GetType() == typeof(T));
            if (controller == null)
            {
                controller = new T();
                // fill in required servicess
                var properties = controller.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsDefined(typeof(RequireServiceAttribute), true));
                properties.ForEach(x => x.SetValue(controller, GetService(x.PropertyType)));
                controller.OnInitialize();
                Controllers.Add(controller);
            }
            return (T)controller;
        }

        public void SaveConfig()
        {
            using (var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Create))
            {
                xmlSerializer.Serialize(fs, Services);
            }

        }

        public void LoadConfig()
        {
            if (File.Exists(configPath.ToAbsolutePath()))
                using (var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Open))
                {
                    Services = (ObservableCollection<ServiceProvider>)xmlSerializer.Deserialize(fs);
                }

            //Subscribe to the events manually
            foreach (ServiceProvider service in Services)
            {
                var properties = service.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsDefined(typeof(RequireServiceAttribute), true));
                properties.ForEach(x => x.SetValue(service, GetService(x.PropertyType)));
                service.ConfigUpdated += Service_ConfigUpdated;
            }
        }
    }
}
