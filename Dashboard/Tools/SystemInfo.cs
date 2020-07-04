// Taken from https://github.com/sourcechord/FluentWPF/blob/master/FluentWPF/Utility/SystemInfo.cs

using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.Tools
{
    class SystemInfo
    {
        public static Lazy<VersionInfo> Version { get; private set; } = new Lazy<VersionInfo>(() => GetVersionInfo());

        internal static VersionInfo GetVersionInfo()
        {
            var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", false);

            if (regkey == null) return default(VersionInfo);

            var majorValue = regkey.GetValue("CurrentMajorVersionNumber");
            var minorValue = regkey.GetValue("CurrentMinorVersionNumber");
            var buildValue = (string)regkey.GetValue("CurrentBuild", 7600);
            var canReadBuild = int.TryParse(buildValue, out var build);

            var defaultVersion = System.Environment.OSVersion.Version;

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
}
