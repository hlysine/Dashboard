﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Dashboard.Components;
using Dashboard.Components.Containers;
using Dashboard.Utilities;
using Dashboard.Views.Components;

namespace Dashboard.Views;

public partial class RootView : Window
{
    private readonly DashboardManager manager = new DashboardManager();
    private readonly Dictionary<DashboardComponent, Window> viewBindings = new();

    public RootView()
    {
        DataContext = manager;
        InitializeComponent();

        // TODO: check that Initialize() does return a WindowContainer
        var window = (WindowContainer)manager.Initialize();
        
        if (manager.Autostart)
            AutoRun.Register();
        else
            AutoRun.Unregister();
            
        var view = GetNewViewFor(window);
        viewBindings.Add(window, view);
        // TODO: extract to helper method: load window without showing
        var helper = new WindowInteropHelper(view);
        helper.EnsureHandle();
    }

    private Window GetNewViewFor(DashboardComponent component)
    {
        // TODO: accomodate types other than WindowContainer and Window
        var classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            from assemblyType in domainAssembly.GetTypes()
            where assemblyType.GetInterface($"{nameof(IDashboardView<WindowContainer>)}`1") != null
                  && (assemblyType.GetInterface($"{nameof(IDashboardView<WindowContainer>)}`1")?.GenericTypeArguments.Contains(component.GetType())).GetValueOrDefault()
                  && !assemblyType.IsAbstract
            select assemblyType).ToArray();
        var target = classList.FirstOrDefault();
        if (target == null)
            return null;
        else
            return (Window)Activator.CreateInstance(target, component);
    }

    // Command binding is not working in tray icon menu for some reason
    private void menuExit_Click(object sender, RoutedEventArgs e)
    {
        if (manager.QuitAppCommand.CanExecute(null))
            manager.QuitAppCommand.Execute(null);
    }
}