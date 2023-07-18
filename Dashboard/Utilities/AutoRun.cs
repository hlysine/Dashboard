using Microsoft.Win32;

namespace Dashboard.Utilities;

public static class AutoRun
{
    private static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    /// <summary>
    /// Check if the application is registered to autostart
    /// </summary>
    /// <returns>Returns true if the app autostarts</returns>
    public static bool Check()
    {
        return rkApp.GetValue(Helper.GetProductName()) != null;
    }

    public static void Register()
    {
        rkApp.SetValue(Helper.GetProductName(), Helper.GetAppExePath());
    }

    public static void Unregister()
    {
        rkApp.DeleteValue(Helper.GetProductName(), false);
    }
}