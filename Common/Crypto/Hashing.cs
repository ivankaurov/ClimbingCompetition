// <copyright file="Hashing.cs">
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
using System.Security.Cryptography;

namespace Crypto
{
    public static class Hashing
    {
        private static Random rnd = new Random();
        private static Encoding defaultEncoding = Encoding.Unicode;
        
        public static byte[] GetBytes(this String value, Byte[] salt = null)
        {
            if (salt == null || salt.Length < 1)
                return defaultEncoding.GetBytes(value ?? String.Empty);
            List<Byte> data = new List<byte>(value.GetBytes());
            data.AddRange(salt);
            return data.ToArray();
        }

        public static String GetString(this byte[] byteValue)
        {
            return defaultEncoding.GetString(byteValue);
        }

        public static byte[] ComputeHash(this byte[] value)
        {
            using (var provider = new SHA512CryptoServiceProvider())
            {
                return provider.ComputeHash(value);
            }
        }

        public static byte[] ComputeHash(this String value, byte[] salt = null)
        {
            return value.GetBytes(salt).ComputeHash();
        }

        public static string ComputeHashString(this string value, byte[] salt = null)
        {
            return Encoding.ASCII.GetString(value.ComputeHash(salt));
        }

        public static bool CheckHash(this byte[] openValue, byte[] hashValue)
        {
            if (openValue == null && hashValue == null)
                return true;
            else if (openValue == null || hashValue == null)
                return false;
            return openValue.ComputeHash()
                            .SequenceEqual(hashValue);
        }

        public static bool CheckHash(this String value, byte[] hashValue, byte[] salt = null)
        {
            return value.GetBytes(salt).CheckHash(hashValue);
        }

        public static byte[] GenerateSalt(int length = 20)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException("length");
            byte[] res = new byte[length];
            lock (rnd)
            {
                rnd.NextBytes(res);
            }
            return res;
        }
    }
}
