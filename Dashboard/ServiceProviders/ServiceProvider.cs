using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.ServiceProviders
{
    public abstract class ServiceProvider
    {
        public abstract bool IsAuthorized { get; }
        public abstract bool CanAuthorize { get; }
        public event EventHandler ConfigUpdated;

        protected void RaiseConfigUpdated(EventArgs e)
        {
            ConfigUpdated?.Invoke(this, e);
        }
    }
}
