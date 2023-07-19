// Taken from https://github.com/sourcechord/FluentWPF/blob/master/FluentWPF/Utility/SystemInfo.cs

using System;
using Microsoft.Win32;

namespace Dashboard.Utilities;

internal class SystemInfo
{
    public static Lazy<VersionInfo> Version { get; private set; } = new(getVersionInfo);

    private static VersionInfo getVersionInfo()
    {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", false);

        if (registryKey == null) return default;

        object majorValue = registryKey.GetValue("CurrentMajorVersionNumber");
        object minorValue = registryKey.GetValue("CurrentMinorVersionNumber");
        var buildValue = (string)registryKey.GetValue("CurrentBuild", 7600);
        bool canReadBuild = int.TryParse(buildValue, out int build);

        Version defaultVersion = Environment.OSVersion.Version;

        if (majorValue is int major && minorValue is int minor && canReadBuild)
        {
            return new VersionInfo(major, minor, build);
        }
        else
        {
            return new VersionInfo(defaultVersion.Major, defaultVersion.Minor, defaultVersion.Revision);
        }
    }

    internal static bool IsWin10()
    {
        return Version.Value.Major == 10;
    }


    internal static bool IsWin7()
    {
        return Version.Value is { Major: 6, Minor: 1 };
    }

    internal static bool IsWin8X()
    {
        return Version.Value is { Major: 6, Minor: 2 or 3 };
    }
}
