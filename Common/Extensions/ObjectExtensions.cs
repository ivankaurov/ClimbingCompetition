using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static Boolean ArrayEquals<T>(this T[] array1, T[] array2)
        {
            if (Object.ReferenceEquals(array1, array2))
                return true;
            if (array1 == null || array2 == null)
                return false;
            return array1.SequenceEqual(array2);
        }

        readonly static object typesSyncRoot = new object();
        readonly static MyConcurrentDictionary<String, Type> types = new MyConcurrentDictionary<string, Type>();
        public static Type GetTypeByName(String name)
        {
            if (name == null)
                return null;
            return types.GetOrAdd(name, key => AppDomain.CurrentDomain.GetAssemblies()
                                       .Select(asm => asm.GetType(key, false))
                                       .FirstOrDefault(t => t != null));
        }
    }
}
