// <copyright file="ServiceConnectionModel.cs">
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
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using ClimbingCompetition.Common.API;

namespace ClimbingCompetition.Client
{
    [Serializable]
    public sealed class ServiceConnectionModel
    {
        public ApiCompetition Competition { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public string Serialize()
        {
            var serializer = new JsonSerializer();
            var stringBuilder = new StringBuilder();
            using (var sr = new StringWriter(stringBuilder))
            {
                serializer.Serialize(sr, this);
            }

            return stringBuilder.ToString();
        }

        public static ServiceConnectionModel FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(value))
            {
                return (ServiceConnectionModel)serializer.Deserialize(stringReader, typeof(ServiceConnectionModel));
            }
        }

        private static void CheckOnlineStorage(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");

            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT count(*) FROM sys.tables(nolock) WHERE name='st0_online_settings'";

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    return;

                cmd.CommandText = "CREATE TABLE st0_online_settings (setting_value nvarchar(max))";
                cmd.ExecuteNonQuery();
            }
        }

        public static ServiceConnectionModel FromSql(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");

            CheckOnlineStorage(cn);

            string result = null;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT top 1 setting_value FROM st0_online_settings(nolock)";
                result = cmd.ExecuteScalar() as string;
            }

            return string.IsNullOrEmpty(result) ? null : ServiceConnectionModel.FromString(result);
        }

        public void Persist(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");
            CheckOnlineStorage(cn);
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = "DELETE FROM st0_online_settings";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO st0_online_settings (setting_value) VALUES (@value)";
                    cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar, -1).Value = this.Serialize();
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}