using System;
using System.Collections.Generic;
using System.Text;
using Dashboard.ServiceProviders;

namespace Dashboard.Config
{
    /// <summary>
    /// Specify that the <see cref="ComponentManager"/> should provide an instance of the required <see cref="ServiceProvider"/> through this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireServiceAttribute : System.Attribute
    {
        public RequireServiceAttribute()
        {
        }
    }
}
