using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Dashboard.Utilities;

public static partial class Helper
{
    public static readonly Random Rnd = new();

    public static bool ParseXamlBoolean(object parameter) => parameter as bool? ?? Convert.ToBoolean((string)parameter);

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        IEnumerable<T> forEach = enumeration.ToList();
        foreach (T item in forEach)
        {
            action(item);
        }

        return forEach;
    }

    public static IEnumerable ForEach(this IEnumerable enumeration, Action<object> action)
    {
        IEnumerable forEach = enumeration.Cast<object>().ToList();
        foreach (object item in forEach)
        {
            action(item);
        }

        return forEach;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Rnd.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    public static void OpenUri(Uri uri)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string uriStr = uri.ToString().Replace("&", "^&");

            Process.Start(new ProcessStartInfo($"cmd", $"/c start {uriStr}") { WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", uri.ToString());
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", uri.ToString());
        }
    }

    public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
    {
        return !a.Except(b).Any();
    }

    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string ToReadableDateString(this DateTime date)
    {
        if (date.Date == DateTime.Today)
        {
            return "Today";
        }

        if (date.Date == DateTime.Today - TimeSpan.FromDays(1))
        {
            return "Yesterday";
        }

        if (date.Date == DateTime.Today + TimeSpan.FromDays(1))
        {
            return "Tomorrow";
        }

        return date.ToString(date.Year == DateTime.Now.Year ? "MMM dd" : "MMM dd, yyyy");
    }

    [GeneratedRegex(".*(?= \\+0000| GMT| UTC| \\(UTC\\))")]
    private static partial Regex excludeUTCRegex();

    [GeneratedRegex(".*(?= [A-Z]{3}| \\([A-Z]{3}\\))")]
    private static partial Regex excludeTimeZoneRegex();

    public static DateTime ParseDateTime(this string str)
    {
        if (DateTime.TryParse(str, out DateTime res1))
        {
            return res1;
        }

        Match match = excludeUTCRegex().Match(str);
        if (match.Success)
        {
            if (DateTime.TryParseExact(match.Value, "ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime res2))
            {
                return res2;
            }
        }

        Match match2 = excludeTimeZoneRegex().Match(str);
        if (DateTime.TryParseExact(match2.Value, "ddd, d MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime res3))
        {
            return res3;
        }

        if (DateTime.TryParseExact(str, "ddd, d MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime res4))
        {
            return res4;
        }

        if (DateTime.TryParseExact(str, "ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime res5))
        {
            return res5;
        }

        throw new ArgumentException("Failed to parse the provided string");
    }

    public static DateTime GetDateTime(this EventDateTime eventDateTime)
    {
        if (eventDateTime.DateTimeRaw == null)
        {
            return DateTime.ParseExact(
                eventDateTime.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None
            );
        }
        else
        {
            return DateTime.Parse(eventDateTime.DateTimeRaw);
            // return eventDateTime.DateTimeDateTimeOffset.GetValueOrDefault().DateTime;
        }
    }

    public static string GetProgramVersion()
    {
        // return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        return System.Windows.Forms.Application.ProductVersion;
    }

    public static string GetAppExeFolder()
    {
        // return System.AppDomain.CurrentDomain.BaseDirectory;
        return System.Windows.Forms.Application.StartupPath;
    }

    public static string GetAppExePath()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().Location;
        // return Process.GetCurrentProcess().MainModule.FileName;
    }

    public static string GetProductName()
    {
        // return Application.Current.MainWindow.GetType().Assembly.GetName().Name;
        return System.Windows.Forms.Application.ProductName;
    }

    public static string GetCompanyName()
    {
        return System.Windows.Forms.Application.CompanyName;
        // Assembly currentAssem = Application.Current.MainWindow.GetType().Assembly;
        // object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
        // if (attribs.Length > 0)
        // {
        //     return ((AssemblyCompanyAttribute)attribs[0]).Company;
        // }
        //
        // return "";
    }

    public static string ToAbsolutePath(this string relativePath)
    {
        return Path.Combine(GetAppExeFolder(), relativePath);
    }
}
