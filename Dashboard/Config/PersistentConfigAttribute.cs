using System;

namespace Dashboard.Config;

[AttributeUsage(AttributeTargets.Property)]
public class PersistentConfigAttribute : Attribute
{
    /// <summary>
    /// Specifies whether this config is generated and should not be edited by the user
    /// </summary>
    public bool Generated { get; set; }
}
