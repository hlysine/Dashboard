using Dashboard.Components;
using Dashboard.Config;
using Dashboard.Services;
using Dashboard.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using Dashboard.Components.Containers;
using Dashboard.Views.Components;

namespace Dashboard;

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

    private ObservableCollection<Service> services = new();

    [PersistentConfig]
    public ObservableCollection<Service> Services
    {
        get => services;
        set => SetAndNotify(ref services, value);
    }

    [PersistentConfig]
    public bool Autostart { get; set; } = true;

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
        var classList = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
            from assemblyType in domainAssembly.GetTypes()
            where assemblyType.IsDefined(typeof(ContainsConfigAttribute), true) && !assemblyType.IsAbstract
            select assemblyType).ToArray();
        var props = new List<PropertyInfo>();
        foreach (var configType in classList)
        {
            foreach (var prop in configType.GetProperties())
            {
                if (!props.Any(x => x.PropertyType == prop.PropertyType && x.Name == prop.Name))
                {
                    props.Add(prop);
                    if (!prop.IsDefined(typeof(PersistentConfigAttribute), false))
                    {
                        configXmlOverrides.Add(prop.DeclaringType, prop.Name, attributes);
                    }
                    else
                    {
                        var attribute = (PersistentConfigAttribute)prop.GetCustomAttribute(typeof(PersistentConfigAttribute));
                        var xmlElem = new XmlAttributes();
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && !typeof(string).IsAssignableFrom(prop.PropertyType))
                        {
                            xmlElem.XmlArray = new XmlArrayAttribute((attribute.Generated ? "_" : "") + prop.Name);
                            var arrTypeList = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                                from assemblyType in domainAssembly.GetTypes()
                                where prop.PropertyType.GetGenericArguments()[0].IsAssignableFrom(assemblyType)
                                select assemblyType).ToArray();
                            foreach (var t in arrTypeList)
                            {
                                xmlElem.XmlArrayItems.Add(new XmlArrayItemAttribute(t));
                            }
                        }
                        else if (attribute.Generated)
                        {
                            xmlElem.XmlElements.Add(new XmlElementAttribute("_" + prop.Name));
                        }

                        configXmlOverrides.Add(prop.DeclaringType, prop.Name, xmlElem);
                    }
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

            var spotifyService = new SpotifyService
                { Id = Helper.RandomString(10) };
            var googleService = new GoogleService
                { Id = Helper.RandomString(10) };
            var calendarService = new GoogleCalendarService
                { Id = googleService.Id };
            var tasksService = new GoogleTasksService
                { Id = googleService.Id };
            var gmailService = new GoogleGmailService
                { Id = googleService.Id };
            var osuService = new OsuService
                { Id = Helper.RandomString(10) };
            var systemService = new SystemService
                { Id = Helper.RandomString(10) };
            var locationService = new LocationService
                { Id = Helper.RandomString(10) };
            var weatherService = new OpenWeatherMapService
            {
                Id = Helper.RandomString(10),
                LocationServiceId = locationService.Id,
            };

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

        Services.ForEach(InitializeService);

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
        using var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Create);
        xmlSerializer.Serialize(fs, this);
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
        if (!File.Exists(configPath.ToAbsolutePath()))
            return;

        using var fs = new FileStream(configPath.ToAbsolutePath(), FileMode.Open);
        using StreamReader reader = new(fs);
        var document = reader.ReadToEnd();
        var xDoc = XDocument.Parse(document);
        foreach (var node in xDoc.Descendants().Where(x => !x.Elements().Any()))
        {
            node.Value = node.Value.Trim();
        }

        document = xDoc.ToString();

        using StringReader xmlReader = new(document);
        var tmpManager = (DashboardManager)xmlSerializer.Deserialize(xmlReader);
        RootComponent = tmpManager.RootComponent;
        Services = tmpManager.Services;
        Autostart = tmpManager.Autostart;
    }
}