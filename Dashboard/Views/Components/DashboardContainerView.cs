using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Dashboard.Components;
using Dashboard.Components.Containers;
using Dashboard.Utilities;

namespace Dashboard.Views.Components
{
    public abstract class DashboardContainerView<TComponent> : DashboardView<TComponent> where TComponent : DashboardContainer, new()
    {
        private Dictionary<DashboardComponent, DashboardViewBase> viewBindings = new();

        protected DashboardContainerView(TComponent component) : base(component)
        {
            Loaded += DashboardContainerView_Loaded;
        }

        private void DashboardContainerView_Loaded(object sender, RoutedEventArgs e)
        {
            Children_CollectionChanged(Component.Children, new(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            Component.Children.CollectionChanged += Children_CollectionChanged;
        }

        protected DashboardContainerView() : this(null)
        {

        }

        protected abstract void AddView(DashboardViewBase element);

        protected abstract void RemoveView(DashboardViewBase element);

        protected abstract void ClearView();

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        var elem = GetNewViewFor(comp);
                        if (elem == null) return;  //DEBUG
                        viewBindings.Add(comp, elem);
                        AddView(elem);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        RemoveView(viewBindings[comp]);
                        viewBindings.Remove(comp);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    e.OldItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        RemoveView(viewBindings[comp]);
                        viewBindings.Remove(comp);
                    });
                    e.NewItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        var elem = GetNewViewFor(comp);
                        viewBindings.Add(comp, elem);
                        AddView(elem);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    viewBindings.Clear();
                    ClearView();
                    Component.Children.ForEach(x =>
                    {
                        var elem = GetNewViewFor(x);
                        if (elem == null) return;  //DEBUG
                        viewBindings.Add(x, elem);
                        AddView(elem);
                    });
                    break;
            }
        }

        private DashboardViewBase GetNewViewFor(DashboardComponent component)
        {
            // TODO: remove BaseType? chain
            var classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from assemblyType in domainAssembly.GetTypes()
                             where assemblyType.IsSubclassOf(typeof(DashboardViewBase))
                                && (assemblyType.BaseType?.BaseType?.GenericTypeArguments.Contains(component.GetType())).GetValueOrDefault()
                                && !assemblyType.IsAbstract
                             select assemblyType).ToArray();
            var target = classList.FirstOrDefault();
            if (target == null)
                return null;
            else
                return (DashboardViewBase)Activator.CreateInstance(target, component);
        }
    }
}
