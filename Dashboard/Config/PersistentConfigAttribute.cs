using System;

namespace Dashboard.Config
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class PersistentConfigAttribute : System.Attribute
    {
        /// <summary>
        /// Specifies whether this config is generated and should not be edited by the user
        /// </summary>
        public bool Generated { get; set; } = false;

        public PersistentConfigAttribute()
        {
        }
    }
}
