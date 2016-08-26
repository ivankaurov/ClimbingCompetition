using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Linq.Expressions;

namespace WebClimbing.ServiceClasses
{
    public static class AdditionalHelpers
    {
        private class ModTuple<T1, T2>
        {
            public T1 Item1 { get; set; }
            public T2 Item2 { get; set; }
            public ModTuple(T1 Item1, T2 Item2)
            {
                this.Item1 = Item1;
                this.Item2 = Item2;
            }
            public ModTuple()
            {
                this.Item1 = default(T1);
                this.Item2 = default(T2);
            }
        }

        public static T GetEnumValue<T>(this String str, bool ignoreCase = true)
            where T : struct
        {
            Type t = typeof(T);
            if (!t.IsEnum)
                throw new ArgumentException("Enum expected", "T");
            if (Attribute.IsDefined(t, typeof(EnumCustomDisplayAttribute)))
            {
                var enumValues = ((T[])Enum.GetValues(t)).Select(v => new { Text = v.GetFriendlyValue(), Value = v });
                StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                foreach (var s in enumValues)
                    if (s.Text.Equals(str, comparison))
                        return s.Value;
            }
            return (T)Enum.Parse(t, str, ignoreCase);
        }

        public static String GetFriendlyValue<T>(this T? eValue)
            where T: struct
        {
            if (eValue == null)
            {
                EnumCustomDisplayAttribute eA = (EnumCustomDisplayAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(EnumCustomDisplayAttribute));
                if (eA != null)
                    return eA.NullName ?? String.Empty;

                return String.Empty;
            }
            else
                return eValue.Value.GetFriendlyValue();
        }

        public static String GetFriendlyValue<T>(this T eValue)
            where T : struct
        {
            Type t = typeof(T);
            if (!t.IsEnum)
                throw new ArgumentException("Enum expected", "eValue");
            if (Attribute.IsDefined(t, typeof(EnumCustomDisplayAttribute)))
            {

                var field = t.GetField(eValue.ToString(), BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                {
                    var dAtr = ((DisplayAttribute[])Attribute.GetCustomAttributes(field, typeof(DisplayAttribute))).FirstOrDefault();
                    if (dAtr != null)
                        return dAtr.Name;
                }
            }
            return eValue.ToString();
        }
        
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper helper, string name, TEnum selectedValue, String nullValueString = null, Object htmlAttr = null) where TEnum : struct
        {
            return helper.EnumDropDownList(name, new Nullable<TEnum>(selectedValue), nullValueString, htmlAttr);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper helper, string name, Nullable<TEnum> selectedValue, String nullValueString = null, Object htmlAttr = null)
            where TEnum: struct
        {
            var values = ((TEnum[])Enum.GetValues(typeof(TEnum))).Select(v => new ModTuple<TEnum, String>(v, v.ToString())).ToList();

            if (Attribute.IsDefined(typeof(TEnum), typeof(EnumCustomDisplayAttribute)))
                values.ForEach(tpl => tpl.Item2 = tpl.Item1.GetFriendlyValue());


            var items = values.Select(i => new SelectListItem() { Text = i.Item2, Value = i.Item1.ToString(), Selected = (i.Item1.Equals(selectedValue)) });
            return helper.DropDownList(name, items, nullValueString, htmlAttr);
        }

        public static String GetStringValue<T>(this T? value, String nullValueText = "<без ограничения>")
            where T : struct
        {
            return (value == null) ? (nullValueText?? String.Empty) : value.Value.ToString();
        }

        public static String GetUnifiedName(this String src)
        {
            if (String.IsNullOrEmpty(src))
                return String.Empty;
            src = src.Trim().ToLowerInvariant().Replace('ё', 'е');
            while (src.Contains("  "))
                src = src.Replace("  ", " ");
            var words = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 1)
                return String.Empty;
            StringBuilder sbFull = new StringBuilder(src.Length);
            for (int i = 0; i < words.Length; i++)
            {
                if (sbFull.Length > 0)
                    sbFull.Append(' ');

                bool started = false;
                var defisSplit = words[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in defisSplit)
                {
                    if (started)
                        sbFull.Append('-');
                    else
                        started = true;
                    sbFull.AppendFormat("{0}{1}", s.Substring(0, 1).ToUpperInvariant(), s.Substring(1).ToLowerInvariant());
                }
                
            }
            return sbFull.ToString();
        }

        private static readonly CultureInfo ciParse = new CultureInfo("ru-RU");
        private const String dateFormat = "d";
        public static String GetDateString(this DateTime? value)
        {
            if (value == null)
                return String.Empty;
            return value.Value.ToString(dateFormat, ciParse);
        }
        public static DateTime? GetDateValue(this String srcString)
        {
            if (srcString == null)
                return null;
            srcString = srcString.Trim();
            if (String.IsNullOrEmpty(srcString))
                return null;
            return DateTime.ParseExact(srcString, dateFormat, ciParse);
        }
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class EnumCustomDisplayAttribute : Attribute
    {
        public EnumCustomDisplayAttribute() { }
        public String NullName { get; set; }
    }
}