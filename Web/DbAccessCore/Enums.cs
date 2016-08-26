using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace DbAccessCore
{
    public enum LogDataType : byte { String = 0, DateTime = 1, Numeric = 2, Binary = 3, Enum = 4, TimeSpan = 5, Reference = 99 }

    public enum LogValueType { New, Old }

    public enum LogAction { Add, Delete, Update }

    public enum LtrState { Begin, Commit, Rollback }

    public enum RightsEnum { Allow = 1, Deny = 0 }

    public enum RightsActionEnum { View = 0, Edit = 1 }

    static class EnumStorage<T>
        where T : struct
    {
        readonly static ConcurrentDictionary<T, string> firstLetterDictionary = new ConcurrentDictionary<T, string>();
        readonly static ConcurrentDictionary<Char, T> reverseValues = new ConcurrentDictionary<char, T>();

        internal static String GetFirstLetter(T value)
        {
            return firstLetterDictionary.GetOrAdd(value, vl => vl.ToString().Substring(0,1));
        }

        internal static T GetByFirstLetter(String firstLetter)
        {
            if (string.IsNullOrWhiteSpace(firstLetter))
                return default(T);
            return reverseValues.GetOrAdd(firstLetter[0], c =>
            {
                var name = Enum.GetNames(typeof(T)).OrderBy(s => s).FirstOrDefault(s => s.First().Equals(c));
                return name == null ? default(T) : (T)Enum.Parse(typeof(T), name);
            });
        }
    }

    public static class EnumExtensions
    {
        public static String GetFirstLetter<T>(this T value) where T : struct
        {
            return EnumStorage<T>.GetFirstLetter(value);
        }

        public static T GetByFirstLetter<T>(this String firstLetter) where T : struct
        {
            return EnumStorage<T>.GetByFirstLetter(firstLetter);
        }
    }
}
