using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dashboard.Tools
{
    public static class Helper
    {
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
