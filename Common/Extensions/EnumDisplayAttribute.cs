using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Resources;

namespace Extensions
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDisplayAttribute : Attribute
    {
        readonly String resourceName;
        public String ResourceName { get { return resourceName; } }

        readonly Type resourceType;
        public Type ResourceType { get { return resourceType; } }

        public EnumDisplayAttribute() { }

        public EnumDisplayAttribute(String elementName)
        {
            this.resourceName = elementName;
        }

        public EnumDisplayAttribute(String resourceName, Type resourceType)
        {
            this.resourceName = resourceName;
            this.resourceType = resourceType;
        }

        public String Name { get { return GetName(Thread.CurrentThread.CurrentUICulture); } }

        public String GetName(CultureInfo language)
        {
            return StringExtensions.GetLocalizedResource(resourceName, resourceType, language, string.Empty);
        }
    }
}