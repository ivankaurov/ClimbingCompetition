// <copyright file="Connector.cs">
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

﻿#if !DEBUG && !MARS
#define MARS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DbAccessCore.SqlCore
{
    static class Connector
    {
        public static String CreateConnectionString(String server, String database = null)
        {
            var cb = new SqlConnectionStringBuilder
            {
                DataSource = server,
                IntegratedSecurity = true
#if MARS
                ,
                MultipleActiveResultSets = true
#endif
            };
            if (!String.IsNullOrEmpty(database))
                cb.InitialCatalog = database;
            return cb.ConnectionString;
        }

        public static String CreateConnectionString(String server, String sqlUser, String sqlPassword, String database = null)
        {
            var cb = new SqlConnectionStringBuilder
            {
                DataSource = server,
                UserID = sqlUser,
                Password = sqlPassword
#if MARS
                ,
                MultipleActiveResultSets = true
#endif
            };
            if (!String.IsNullOrEmpty(database))
                cb.InitialCatalog = database;
            return cb.ConnectionString;
        }

        public static String CreateInternalConnectionString(String server, String database = null, String wsid = null)
        {
            var cb = new SqlConnectionStringBuilder
            {
                DataSource = server,
                UserID = Initializer.SQL_LOGIN,
                Password = Initializer.SQL_PASSWORD
#if MARS
                ,
                MultipleActiveResultSets = true
#endif
            };
            if (!String.IsNullOrEmpty(database))
                cb.InitialCatalog = database;
            if (!String.IsNullOrEmpty(wsid))
                cb.WorkstationID = wsid;
            return cb.ConnectionString;
        }
    }
}