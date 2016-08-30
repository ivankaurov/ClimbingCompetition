// <copyright file="Enums.cs">
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
