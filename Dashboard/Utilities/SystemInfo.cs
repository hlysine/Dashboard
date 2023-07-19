// Taken from https://github.com/sourcechord/FluentWPF/blob/master/FluentWPF/Utility/SystemInfo.cs

using System;
using Microsoft.Win32;

namespace Dashboard.Utilities;

internal class SystemInfo
{
    public static Lazy<VersionInfo> Version { get; private set; } = new(() => GetVersionInfo());

    internal static VersionInfo GetVersionInfo()
    {
        RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", false);

        if (regkey == null) return default(VersionInfo);

        object majorValue = regkey.GetValue("CurrentMajorVersionNumber");
        object minorValue = regkey.GetValue("CurrentMinorVersionNumber");
        var buildValue = (string)regkey.GetValue("CurrentBuild", 7600);
        bool canReadBuild = int.TryParse(buildValue, out int build);

        Version defaultVersion = System.Environment.OSVersion.Version;

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
        return Version.Value.Major == 6 && Version.Value.Minor == 1;
    }

    internal static bool IsWin8x()
    {
        return Version.Value.Major == 6 && (Version.Value.Minor == 2 || Version.Value.Minor == 3);
    }
}