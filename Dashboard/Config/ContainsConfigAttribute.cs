using System;

namespace Dashboard.Config
{
    /// <summary>
    /// Signals that this class contains <see cref="PersistentConfigAttribute"/> properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ContainsConfigAttribute : System.Attribute
    {
        public ContainsConfigAttribute()
        {
        }
    }
}
