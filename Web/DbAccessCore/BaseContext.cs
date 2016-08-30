// <copyright file="BaseContext.cs">
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
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace DbAccessCore
{
    public abstract partial class BaseContext : DbContext
    {
        readonly SqlConnection connection;
        readonly bool ownConnection;
        protected String ConnectionString { get { return Connection.ConnectionString; } }
        protected SqlConnection Connection { get { return connection; } }

        readonly IPAddress clrIP;
        readonly String clrHostName = Dns.GetHostName();
        readonly Lazy<String> clrHostNameForSQL;
        readonly Lazy<String> clrIpAddressForSQL;

        public String CLRHostName { get { return clrHostName; } }
        public IPAddress ClrIP { get { return clrIP; } }
        public String CLRHostNameForSQL { get { return clrHostNameForSQL.Value; } }
        public String CLRIPAddressForSQL { get { return clrIpAddressForSQL.Value; } }

        readonly Boolean web = false;
        readonly Boolean isClone = false;

        readonly IPAddress clientIp;
        readonly Lazy<String> clientIpForSql;
        public IPAddress ClientIP { get { return clientIp; } }
        public String ClientIPForSQL { get { return clientIpForSql.Value; } }

        readonly String clientHostName;
        readonly Lazy<String> clientHostNameForSQL;
        public String ClientHostName { get { return clientHostName; } }
        public String ClientHostNameForSQL { get { return clientHostNameForSQL.Value; } }        

        public Boolean IsWebContext { get { return web; } }
        protected internal virtual Boolean AdminAllowAll { get { return true; } }

        protected sealed override void Dispose(bool disposing)
        {
            if (!createdForInitOnly && disposing)
            {
                OnDispose();
                LogOut(!(isClone || web));
                if (ownConnection)
                    connection.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void OnDispose()
        {
        }

        public Log.LogicTransaction BeginLtr(String name = null)
        {
            return this.LogicTransactions1.Add(new Log.LogicTransaction(name, this));
        }
    }
}
