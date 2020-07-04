using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dashboard.Controllers
{
    public abstract class DashboardController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetAndNotify<T>(ref T variable, T value, string[] calculatedProperties = null, [CallerMemberName] string memberName = null)
        {
            Contract.Requires(memberName != null);
            variable = value;
            NotifyChanged(memberName);
            if (calculatedProperties != null)
            {
                Array.ForEach(calculatedProperties, x => NotifyChanged(x));
            }
        }

        /// <summary>
        /// To be called when <see cref="ComponentManager"/> finished the initialization of this controller (filled in required services)
        /// <para>A good place to call <see cref="ServiceProviders.AuthCodeServiceProvider.Authorize(System.Threading.CancellationToken)"/>.</para>
        /// </summary>
        public virtual void OnInitialize()
        {

        }

        /// <summary>
        /// To be called when <see cref="ComponentManager"/> instantiated all components.
        /// <para>A good place to call <see cref="ServiceProviders.AuthCodeServiceProvider.RequireScopes(string[])"/>.</para>
        /// </summary>
        public virtual void OnInitializationComplete()
        {

        }
    }
}
