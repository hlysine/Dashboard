using Dashboard.Components;
using Dashboard.Config;
using Dashboard.Services;
using Dashboard.Utilities;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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
    [ContainsConfig]
    public class DashboardManager : NotifyPropertyChanged
    {
        private DashboardComponent rootComponent;
        [PersistentConfig]
        public DashboardComponent RootComponent
        {
            get => rootComponent;
            set => SetAndNotify(ref rootComponent, value);
        }

        private ObservableCollection<Service> services = new ObservableCollection<Service>();
        [PersistentConfig]
        public ObservableCollection<Service> Services
        {
            get => services;
            set => SetAndNotify(ref services, value);
        }

        private const string configPath = "config.xml";

        private XmlSerializer xmlSerializer;

        /// <summary>
        /// Initialize this manager. To be called by the view of the root <see cref="DashboardComponent"/> (which is likely a <see cref="WindowView"/>).
        /// </summary>
        /// <returns></returns>
        public DashboardComponent Initialize()
        {
            //Prepare an XmlSerializer to be used when saving config
            var configXmlOverrides = new XmlAttributeOverrides();
            var attributes = new XmlAttributes();
            attributes.XmlIgnore = true;
            var classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from assemblyType in domainAssembly.GetTypes()
                             where assemblyType.IsDefined(typeof(ContainsConfigAttribute))
                             select assemblyType).ToArray();
            var props = new List<PropertyInfo>();
            foreach (Type configType in classList)
            {
                foreach (var prop in configType.GetProperties())
                {
                    if (!prop.IsDefined(typeof(PersistentConfigAttribute), true) && !props.Any(x => x.PropertyType == prop.PropertyType && x.Name == prop.Name))
                    {
                        props.Add(prop);
                        configXmlOverrides.Add(prop.DeclaringType, prop.Name, attributes);
                    }
                }
            }

            xmlSerializer = new XmlSerializer(typeof(DashboardManager), configXmlOverrides, classList, new XmlRootAttribute("DashboardConfig"), "");

            if (File.Exists(configPath.ToAbsolutePath()))
                LoadConfig();
            else
            {
                // Generate default config for now
                // TODO: FUTURE: launch setup guide when no config.xml
                var root = new WindowContainer();

                var clock = new ClockComponent();
                var spotify = new SpotifyComponent();
                var tasks = new GoogleTasksComponent();
                var calendar = new GoogleCalendarComponent();
                var gmail = new GoogleGmailComponent();
                var osu = new OsuComponent();
                var launcher = new LauncherComponent();
                var weather = new WeatherComponent();

                var spotifyService = new SpotifyService() { Id = Helper.RandomString(10) };
                var googleService = new GoogleService() { Id = Helper.RandomString(10) };
                var calendarService = new GoogleCalendarService() { Id = googleService.Id };
                var tasksService = new GoogleTasksService() { Id = googleService.Id };
                var gmailService = new GoogleGmailService() { Id = googleService.Id };
                var osuService = new OsuService() { Id = Helper.RandomString(10) };
                var systemService = new SystemService() { Id = Helper.RandomString(10) };
                var locationService = new LocationService() { Id = Helper.RandomString(10) };
                var weatherService = new OpenWeatherMapService() { Id = Helper.RandomString(10) };

                weatherService.LocationServiceId = locationService.Id;

                spotify.SpotifyAccountId = spotifyService.Id;
                tasks.GoogleAccountId = tasksService.Id;
                calendar.GoogleAccountId = calendarService.Id;
                gmail.GoogleAccountId = gmailService.Id;
                osu.OsuAccountId = osuService.Id;
                launcher.SystemServiceId = systemService.Id;
                weather.OpenWeatherMapServiceId = weatherService.Id;

                Services.Add(spotifyService);
                Services.Add(googleService);
                Services.Add(calendarService);
                Services.Add(tasksService);
                Services.Add(gmailService);
                Services.Add(osuService);
                Services.Add(systemService);
                Services.Add(locationService);
                Services.Add(weatherService);

                root.Children.Add(clock);
                root.Children.Add(spotify);
                root.Children.Add(tasks);
                root.Children.Add(calendar);
                root.Children.Add(gmail);
                root.Children.Add(osu);
                root.Children.Add(launcher);
                root.Children.Add(weather);

                RootComponent = root;
            }

            Services.ForEach(x => InitializeService(x));
            InitializeComponent(RootComponent);

            RootComponent.InitializationComplete();

            SaveConfig();

            return RootComponent;
        }

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public DashboardManager()
        {
        }

        public Service GetService(Type type, string serviceId)
        {
            Contract.Requires(!serviceId.IsNullOrEmpty());
            var service = Services.FirstOrDefault(x => x.GetType() == type && x.Id == serviceId);
            if (service == null)
            {
                service = (Service)Activator.CreateInstance(type);
                // fill in required services
                InitializeService(service);
                service.Id = serviceId;
                Services.Add(service);
            }
            return service;
        }

        private void Service_ConfigUpdated(object sender, EventArgs e)
        {
            SaveConfig();
        }

        public T GetService<T>(string serviceId) where T : Service, new()
        {
            return (T)GetService(typeof(T), serviceId);
        }

        public void SaveConfig()
        {
            using (var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Create))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        private void InitializeService(Service service)
        {
            service.GetServices(this);
            service.ConfigUpdated += Service_ConfigUpdated;
        }

        private void InitializeComponent(DashboardComponent component)
        {
            component.GetServices(this);
            component.Initialize();
        }

        public void LoadConfig()
        {
            if (File.Exists(configPath.ToAbsolutePath()))
                using (var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Open))
                {
                    var tmpManager = (DashboardManager)xmlSerializer.Deserialize(fs);
                    RootComponent = tmpManager.RootComponent;
                    Services = tmpManager.Services;
                }
        }
    }
}
