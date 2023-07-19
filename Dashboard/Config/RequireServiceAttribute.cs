using System;
using Dashboard.Services;

namespace Dashboard.Config;

/// <summary>
/// Specify that the <see cref="DashboardManager"/> should provide an instance of the required <see cref="Service"/> through this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequireServiceAttribute : Attribute
{
    public string ServiceIdProperty { get; set; }

    public RequireServiceAttribute(string serviceIdProperty)
    {
        ServiceIdProperty = serviceIdProperty;
    }
}
