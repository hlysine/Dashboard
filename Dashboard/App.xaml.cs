using System;
using System.Threading;
using System.Windows;
using Dashboard.Views.Components;

namespace Dashboard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Constants and Fields

    /// <summary>The event mutex name.</summary>
    private const string unique_event_name = "{798498E1-F78E-4A7B-A36E-A3B595AAF376}";

    /// <summary>The unique mutex name.</summary>
    private const string unique_mutex_name = "{9FEBFF08-B20D-4CAC-B6AE-ADA856530C81}";

    /// <summary>The event wait handle.</summary>
    private EventWaitHandle eventWaitHandle;

    /// <summary>The mutex.</summary>
    private Mutex mutex;

    #endregion

    #region Methods

    /// <summary>The app on startup.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void AppOnStartup(object sender, StartupEventArgs e)
    {
        mutex = new Mutex(true, unique_mutex_name, out bool isOwned);
        eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, unique_event_name);

        // So, R# would not give a warning that this variable is not used.
        GC.KeepAlive(mutex);

        if (isOwned)
        {
            // Spawn a thread which will be waiting for our event
            var thread = new Thread(
                () =>
                {
                    while (eventWaitHandle.WaitOne())
                    {
                        Current.Dispatcher.BeginInvoke(
                            (Action)(() => ((WindowView)Current.MainWindow).BringToForeground()));
                    }
                })
            {
                // It is important mark it as background otherwise it will prevent app from exiting.
                IsBackground = true,
            };

            thread.Start();
            return;
        }

        // Notify other instance so it could bring itself to foreground.
        eventWaitHandle.Set();

        // Terminate this instance.
        Shutdown();
    }

    #endregion
}
