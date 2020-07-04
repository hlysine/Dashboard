using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.Config
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class PersistentConfigAttribute : System.Attribute
    {
        public PersistentConfigAttribute()
        {
        }
    }
}
