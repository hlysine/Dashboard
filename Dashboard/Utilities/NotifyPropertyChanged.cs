using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Dashboard.Utilities
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyChanged(string propertyName)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, new(propertyName));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => NotifyChanged(propertyName));
            }
        }

        protected void NotifyChanged(IEnumerable<string> propertyName)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                propertyName.ForEach(NotifyChanged);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => NotifyChanged(propertyName));
            }
        }

        protected void SetAndNotify<T>(ref T variable, T value, string[] calculatedProperties = null, [CallerMemberName] string memberName = null)
        {
            Contract.Requires(memberName != null);
            variable = value;
            NotifyChanged(memberName);
            if (calculatedProperties != null) NotifyChanged(calculatedProperties);
        }
    }
}