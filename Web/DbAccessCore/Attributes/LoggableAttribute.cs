using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class LoggableAttribute : Attribute
    {
        readonly Boolean isLoggable;

        public LoggableAttribute(Boolean isLoggable) { this.isLoggable = isLoggable; }

        public Boolean Loggable { get { return isLoggable; } }
    }
}
