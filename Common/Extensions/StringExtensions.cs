// <copyright file="StringExtensions.cs">
// Copyright © 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Extensions
{
    public static class StringExtensions
    {
        public const String DEFAULT_DATE_FORMAT = "d";

        public static CultureInfo DefaultCulture { get { return CultureInfo.GetCultureInfo("ru-RU"); } }

        public static DateTime? GetFormalizedDate(this String value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            return DateTime.ParseExact(value, DEFAULT_DATE_FORMAT, DefaultCulture);
        }

        public static String GetFormalizedDateString(this DateTime? value)
        {
            if (!value.HasValue)
                return String.Empty;
            return value.Value.ToString(DEFAULT_DATE_FORMAT, DefaultCulture);
        }

        public static Double? GetNumericValue(this String value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            return Double.Parse(value, DefaultCulture);
        }

        public static String GetNumericValueString(this Double value)
        {
            return value.ToString(DefaultCulture);
        }

        public static String GetNumericValueString(this Double? value)
        {
            return value.HasValue ? value.GetNumericValueString() : String.Empty;
        }

        public static T? GetEnumValue<T>(this String value) where T : struct
        {
            if (String.IsNullOrEmpty(value))
                return null;
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static String GetEnumValueString<T>(this T value) where T : struct
        {
            return Enum.GetName(typeof(T), value);
        }

        public static String GetEnumValueString<T>(this T? value) where T : struct
        {
            return value.HasValue ? value.Value.GetEnumValueString() : String.Empty;
        }
        
        public static T GetEnumByStringValue<T>(string stringValue, T defaultValue, CultureInfo lanugage = null) where T : struct
        {
            return StringExtensions.GetEnumByStringValue<T>(stringValue, lanugage) ?? defaultValue;
        }

        public static T? GetEnumByStringValue<T>(string stringValue, CultureInfo language = null) where T : struct
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var resultField = fields.FirstOrDefault(f =>
            {
                var attr = (EnumDisplayAttribute)Attribute.GetCustomAttribute(f, typeof(EnumDisplayAttribute));
                if (attr == null)
                    return false;
                return string.Equals(attr.GetName(language), stringValue ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            });

            return resultField == null ? null : new T?((T)resultField.GetValue(null));
        }

        public static String EnumFriendlyValue<T>(this T value, CultureInfo language = null) where T : struct
        {
            var tField = typeof(T).GetField(value.ToString(), BindingFlags.Public | BindingFlags.Static);
            if (tField != null)
            {
                var attr = (EnumDisplayAttribute)Attribute.GetCustomAttribute(tField, typeof(EnumDisplayAttribute));
                if (attr != null)
                    return attr.GetName(language);
            }
            return value.ToString();
        }

        public static String EnumFriendlyValue<T>(this T? value) where T : struct
        {
            if (value.HasValue)
                return value.Value.EnumFriendlyValue();
            var attr = (EnumDisplayAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(EnumDisplayAttribute));
            if (attr != null)
                return attr.Name;
            return String.Empty;
        }

        public static String FormatParameterString(this String value, params String[] args)
        {
            if (args == null || args.Length < 1)
                return value.Replace("&", String.Empty);
            StringBuilder resultString = new StringBuilder();
            String currentString = value;
            int n, argIndex = 0;
            while ((n = currentString.IndexOf('&')) >= 0)
            {
                if (n > 0)
                    resultString.Append(currentString.Substring(0, n));
                if (argIndex < args.Length)
                    resultString.Append(args[argIndex++]);
                currentString = (n >= currentString.Length - 1) ? String.Empty : currentString.Substring(n + 1);
            }
            if (currentString.Length > 0)
                resultString.Append(currentString);
            return resultString.ToString();
        }

        readonly static MyConcurrentDictionary<Type, MyThreadLocal<ResourceManager>> resourceManagerCache = new MyConcurrentDictionary<Type, MyThreadLocal<ResourceManager>>();
        public static String GetLocalizedResource(this string resourceName, Type resourceType, CultureInfo language = null, String valueIfEmpty = null)
        {
            if (String.IsNullOrEmpty(resourceName))
                return valueIfEmpty ?? string.Empty;
            if (resourceType == null)
                return resourceName;
            try
            {
                return resourceManagerCache.GetOrAdd(resourceType, tp => new MyThreadLocal<ResourceManager>(() => new ResourceManager(tp)))
                                            .Value
                                            .GetString(resourceName, language ?? /* Thread.CurrentThread.CurrentUICulture*/ DefaultCulture);
            }
            catch(MissingManifestResourceException)
            {
                return resourceName;
            }
        }

        public static String Cut(this String value, int length)
        {
            if (length <= 0 || value == null)
                return String.Empty;
            return value.Length > length ? value.Substring(0, length) : value;
        }

        public static String CutString<T>(this T value, int length)
            where T : class
        {
            return value == null ? String.Empty : value.ToString().Cut(length);
        }

        public static String XORString(this String value)
        {
            if (String.IsNullOrEmpty(value))
                return "0000";
            UInt16 result = 0, val;
            var bytes = Encoding.ASCII.GetBytes(value.ToUpperInvariant());
            for (int i = 0; i < bytes.Length; i += 2)
            {
                val = (ushort)(bytes[i] << 8);
                if (i < (bytes.Length - 1))
                    val += bytes[i + 1];
                result = (ushort)(result ^ val);
            }
            return result.ToString("X4");
        }
    }
}