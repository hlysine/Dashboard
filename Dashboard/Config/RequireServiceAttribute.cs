using System;
using System.Collections.Generic;
using System.Text;
using Dashboard.Services;

namespace Dashboard.Config
{
    /// <summary>
    /// Specify that the <see cref="DashboardManager"/> should provide an instance of the required <see cref="Service"/> through this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireServiceAttribute : System.Attribute
    {
        public string ServiceIdProperty { get; set; }

        public RequireServiceAttribute(string serviceIdProperty)
        {
            ServiceIdProperty = serviceIdProperty;
        }
    }
}
