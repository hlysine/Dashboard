using Microsoft.Win32;

namespace Dashboard.Utilities;

public static class AutoRun
{
    private static readonly RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    /// <summary>
    /// Check if the application is registered to autostart
    /// </summary>
    /// <returns>Returns true if the app auto-starts</returns>
    public static bool Check()
    {
        return registryKey.GetValue(Helper.GetProductName()) != null;
    }

    public static void Register()
    {
        registryKey.SetValue(Helper.GetProductName(), Helper.GetAppExePath());
    }

    public static void Unregister()
    {
        registryKey.DeleteValue(Helper.GetProductName(), false);
    }
}
