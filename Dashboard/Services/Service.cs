using Dashboard.Config;
using Dashboard.Utilities;
using System;
using System.Linq;
using System.Reflection;

namespace Dashboard.Services;

[ContainsConfig]
public abstract class Service
{
    public abstract bool IsAuthorized { get; }
    public abstract bool CanAuthorize { get; }

    [PersistentConfig]
    public string Id { get; set; } = "";
    public event EventHandler ConfigUpdated;

    protected void RaiseConfigUpdated(EventArgs e)
    {
        ConfigUpdated?.Invoke(this, e);
    }

    // TODO: unify DashboardComponent.GetServices and this
    public virtual void GetServices(DashboardManager manager)
    {
        var properties = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                  .Where(x => x.IsDefined(typeof(RequireServiceAttribute), true))
                                  .Select(prop => (prop, (RequireServiceAttribute)Attribute.GetCustomAttribute(prop, typeof(RequireServiceAttribute))));
        properties.ForEach(x => x.prop.SetValue(this, manager.GetService(x.prop.PropertyType, (string)GetType().GetProperty(x.Item2.ServiceIdProperty).GetValue(this))));
        OnInitialized();
    }

    protected virtual void OnInitialized()
    {

    }
}