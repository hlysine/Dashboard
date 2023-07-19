using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Dashboard.Components;
using Dashboard.Components.Containers;
using Dashboard.Utilities;

namespace Dashboard.Views.Components;

public abstract class DashboardContainerView<TComponent> : DashboardView<TComponent> where TComponent : DashboardContainer, new()
{
    private readonly Dictionary<DashboardComponent, DashboardViewBase> viewBindings = new();

    protected DashboardContainerView(TComponent component) : base(component)
    {
        Loaded += DashboardContainerView_Loaded;
    }

    private void DashboardContainerView_Loaded(object sender, RoutedEventArgs e)
    {
        Children_CollectionChanged(Component.Children, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        Component.Children.CollectionChanged += Children_CollectionChanged;
    }

    protected DashboardContainerView() : this(null)
    {

    }

    protected abstract void AddView(DashboardViewBase element);

    protected abstract void RemoveView(DashboardViewBase element);

    protected abstract void ClearView();

    private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    DashboardViewBase elem = getNewViewFor(comp);
                    if (elem == null) return;  //DEBUG
                    viewBindings.Add(comp, elem);
                    AddView(elem);
                });
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    RemoveView(viewBindings[comp]);
                    viewBindings.Remove(comp);
                });
                break;
            case NotifyCollectionChangedAction.Replace:
                e.OldItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    RemoveView(viewBindings[comp]);
                    viewBindings.Remove(comp);
                });
                e.NewItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    DashboardViewBase elem = getNewViewFor(comp);
                    viewBindings.Add(comp, elem);
                    AddView(elem);
                });
                break;
            case NotifyCollectionChangedAction.Reset:
                viewBindings.Clear();
                ClearView();
                Component.Children.ForEach(x =>
                {
                    DashboardViewBase elem = getNewViewFor(x);
                    if (elem == null) return;  //DEBUG
                    viewBindings.Add(x, elem);
                    AddView(elem);
                });
                break;
        }
    }

    // TODO: cache types
    private static DashboardViewBase getNewViewFor(DashboardComponent component)
    {
        // TODO: remove BaseType? chain
        Type[] classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            from assemblyType in domainAssembly.GetTypes()
            where assemblyType.IsSubclassOf(typeof(DashboardViewBase))
                  && (assemblyType.BaseType?.BaseType?.GenericTypeArguments.Contains(component.GetType())).GetValueOrDefault()
                  && !assemblyType.IsAbstract
            select assemblyType).ToArray();
        Type target = classList.FirstOrDefault();
        if (target == null)
            return null;
        else
            return (DashboardViewBase)Activator.CreateInstance(target, component);
    }
}
