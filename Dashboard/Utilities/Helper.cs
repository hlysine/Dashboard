using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dashboard.Utilities
{
    public static class Helper
    {
        public static readonly Random Rnd = new Random();

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
            return enumeration;
        }

        public static IEnumerable ForEach(this IEnumerable enumeration, Action<object> action)
        {
            foreach (object item in enumeration)
            {
                action(item);
            }
            return enumeration;
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

        public static DateTime ParseDateTime(this string str)
        {
            if (DateTime.TryParse(str,out DateTime res1))
            {
                return res1;
            }
            var match = Regex.Match(str, @".*(?= \+0000| GMT| UTC| \(UTC\))");
            if (match.Success)
            {
                if (DateTime.TryParseExact(match.Value, "ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime res2))
                {
                    return res2;
                }
            }
            var match2 = Regex.Match(str, @".*(?= [A-Z]{3}| \([A-Z]{3}\))");
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
            if (eventDateTime.DateTime == null)
            {
                return DateTime.ParseExact(eventDateTime.Date,
                                        "yyyy-MM-dd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
            }
            else
            {
                return eventDateTime.DateTime.GetValueOrDefault();
            }
        }

        public static string GetProgramVersion()
        {
            //return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return System.Windows.Forms.Application.ProductVersion;
        }

        public static string GetAppExeFolder()
        {
            //return System.AppDomain.CurrentDomain.BaseDirectory;
            return System.Windows.Forms.Application.StartupPath;
        }

        public static string GetAppExePath()
        {
            //return System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.Windows.Forms.Application.ExecutablePath;
        }

        public static string GetProductName()
        {
            //return Application.Current.MainWindow.GetType().Assembly.GetName().Name;
            return System.Windows.Forms.Application.ProductName;
        }

        public static string GetCompanyName()
        {
            return System.Windows.Forms.Application.CompanyName;
        }

        public static string ToAbsolutePath(this string relativePath)
        {
            return Path.Combine(System.Windows.Forms.Application.StartupPath, relativePath);
        }
    }
}
