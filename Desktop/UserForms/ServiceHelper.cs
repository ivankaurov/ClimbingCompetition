// <copyright file="ServiceHelper.cs">
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    public static class ServiceHelper
    {
        public const string REMOTE_ID_COLUMN = "RemoteIID";
        public const int REMOTE_ID_COL_SIZE = 50;

        public static void CheckRemoteIdColumn(string tableName, SqlConnection cn, SqlTransaction tran)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (cn == null)
                throw new ArgumentNullException("cn");

            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();

            using(var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran ?? cmd.Connection.BeginTransaction();
                try
                {
                    cmd.CommandText = "select count(*)" +
                                      "  from sys.tables T(nolock)" +
                                      "  join sys.columns C(nolock) on C.object_id = T.object_id" +
                                      " where T.name = @tName" +
                                      "   and C.name = '" + ServiceHelper.REMOTE_ID_COLUMN + "'";
                    cmd.Parameters.Add("@tName", SqlDbType.VarChar, 255).Value = tableName;

                    var resultObj = cmd.ExecuteScalar();
                    if (resultObj == null || resultObj == DBNull.Value || Convert.ToInt32(resultObj) == 0)
                    {
                        cmd.CommandText = String.Format("ALTER TABLE {0} ADD {1} VARCHAR({2}) NULL", tableName, ServiceHelper.REMOTE_ID_COLUMN, ServiceHelper.REMOTE_ID_COL_SIZE);
                        cmd.ExecuteNonQuery();
                    }

                    if (tran == null)
                        cmd.Transaction.Commit();
                }
                catch
                {
                    if (tran == null)
                        cmd.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
