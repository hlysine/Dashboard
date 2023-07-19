using Dashboard.Config;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dashboard.Components;

[ContainsConfig]
public abstract class DashboardComponent : NotifyPropertyChanged
{
    private ComponentPosition position = new();

    [PersistentConfig]
    public ComponentPosition Position
    {
        get => position;
        set => SetAndNotify(ref position, value);
    }

    private bool showTitle = true;

    [PersistentConfig]
    public bool ShowTitle
    {
        get => showTitle;
        set => SetAndNotify(ref showTitle, value);
    }

    public abstract string DefaultName { get; }

    private string customName = "";

    [PersistentConfig]
    public string CustomName
    {
        get => customName;
        set => SetAndNotify(ref customName, value, new[] { nameof(Name) });
    }

    public virtual string Name => CustomName.IsNullOrEmpty() ? DefaultName : CustomName;

    public virtual void InitializeDependencies()
    {
        OnInitializeDependencies();
    }

    public virtual void InitializeSelf()
    {
        OnInitializeSelf();
    }

    /// <summary>
    /// Called when the services required by this component have been filled in.
    /// <para>All dependencies should be configured here, such as calling <see cref="Services.AuthCodeService.RequireScopes(string[])"/>.</para>
    /// </summary>
    protected virtual void OnInitializeDependencies()
    {
    }

    /// <summary>
    /// Called when all components have initialized their dependencies.
    /// <para>All services are now safe to use, such as requesting authorization via <see cref="Services.AuthCodeService.Authorize(System.Threading.CancellationToken)"/>.</para>
    /// </summary>
    protected virtual void OnInitializeSelf()
    {
    }

    public virtual DashboardComponent Parent { get; set; }

    private bool foreground = true;

    public bool ThisForeground
    {
        get => foreground;
        set
        {
            SetAndNotify(ref foreground, value);
            ForegroundChanged();
        }
    }

    public bool Foreground => ThisForeground && (Parent?.Foreground ?? true);

    //var fg = ThisForeground;
    //var comp = this;
    //while (comp.Parent != null)
    //{
    //    fg &= comp.Parent.ThisForeground;
    //    comp = comp.Parent;
    //}
    //return fg;
    public virtual void ForegroundChanged()
    {
        OnForegroundChanged();
    }

    protected virtual void OnForegroundChanged()
    {
    }

    public virtual void GetServices(DashboardManager manager)
    {
        IEnumerable<(PropertyInfo prop, RequireServiceAttribute)> properties = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                                                                        .Where(x => x.IsDefined(typeof(RequireServiceAttribute), true))
                                                                                        .Select(prop => (prop, (RequireServiceAttribute)Attribute.GetCustomAttribute(prop, typeof(RequireServiceAttribute))));
        properties.ForEach(x => x.prop.SetValue(this, manager.GetService(x.prop.PropertyType, (string)GetType().GetProperty(x.Item2.ServiceIdProperty).GetValue(this))));
    }

    private bool loaded = false;

    /// <summary>
    /// Whether the Component has finished the initial load (including authentication and initial data load).
    /// <para>Will invoke <see cref="FinishedLoading"/> when this is set from false to true.</para>
    /// </summary>
    public bool Loaded
    {
        get => loaded;
        protected set
        {
            if (value && !loaded)
            {
                loaded = value;
                FinishedLoading?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                loaded = value;
            }
        }
    }

    /// <summary>
    /// When the Component finished the initial load (including authentication and initial data load).
    /// </summary>
    public event EventHandler FinishedLoading;
}
