using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Dashboard.Utilities
{
    public static class Helper
    {
        public static readonly Random Rnd = new();

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }

            return enumeration;
        }

        public static IEnumerable ForEach(this IEnumerable enumeration, Action<object> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }

            return enumeration;
        }

        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[Rnd.Next(chars.Length)];
            }

            return new(stringChars);
        }

        public static void OpenUri(Uri uri)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var uriStr = uri.ToString().Replace("&", "^&");

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
            else if (date.Date == DateTime.Today - TimeSpan.FromDays(1))
            {
                return "Yesterday";
            }
            else if (date.Date == DateTime.Today + TimeSpan.FromDays(1))
            {
                return "Tomorrow";
            }
            else if (date.Year == DateTime.Now.Year)
            {
                return date.ToString("MMM dd");
            }
            else
            {
                return date.ToString("MMM dd, yyyy");
            }
        }

        public static DateTime ParseDateTime(this string str)
        {
            if (DateTime.TryParse(str, out var res1))
            {
                return res1;
            }

            var match = Regex.Match(str, @".*(?= \+0000| GMT| UTC| \(UTC\))");
            if (match.Success)
            {
                if (DateTime.TryParseExact(match.Value, "ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out var res2))
                {
                    return res2;
                }
            }

            var match2 = Regex.Match(str, @".*(?= [A-Z]{3}| \([A-Z]{3}\))");
            if (DateTime.TryParseExact(match2.Value, "ddd, d MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out var res3))
            {
                return res3;
            }

            if (DateTime.TryParseExact(str, "ddd, d MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out var res4))
            {
                return res4;
            }

            if (DateTime.TryParseExact(str, "ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out var res5))
            {
                return res5;
            }

            throw new ArgumentException("Failed to parse the provided string");
        }

        public static DateTime GetDateTime(this EventDateTime eventDateTime)
        {
            if (eventDateTime.DateTimeRaw == null)
            {
                return DateTime.ParseExact(eventDateTime.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
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
}