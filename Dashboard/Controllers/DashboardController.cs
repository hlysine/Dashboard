using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dashboard.Controllers
{
    public abstract class DashboardController : NotifyPropertyChanged
    {
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

        public virtual TimeSpan BackgroundRefreshRate { get => TimeSpan.FromMinutes(30); }
        public virtual TimeSpan ForegroundRefreshRate { get => TimeSpan.FromMinutes(2); }

        /// <summary>
        /// Called periodically by <see cref="ComponentManager"/> to update component content. 
        /// The frequency is decided by <see cref="ForegroundRefreshRate"/> and <see cref="BackgroundRefreshRate"/>
        /// </summary>
        public virtual void OnRefresh()
        {

        }

        private bool loaded = false;

        /// <summary>
        /// Whether the controller has finished the initial load.
        /// <para>Will invoke <see cref="FinishedLoading"/> when this is set from false to true.</para>
        /// </summary>
        public bool Loaded
        {
            get => loaded;
            protected set
            {
                if (value && !loaded)
                {
                    loaded = value;
                    FinishedLoading?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    loaded = value;
                }
            }
        }

        /// <summary>
        /// When the controller finished the initial load (including authentication and initial data load).
        /// </summary>
        public event EventHandler FinishedLoading;
    }
}
