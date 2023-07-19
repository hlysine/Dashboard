using System;

namespace Dashboard.Config;

/// <summary>
/// Signals that this class contains <see cref="PersistentConfigAttribute"/> properties
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ContainsConfigAttribute : Attribute
{
}
