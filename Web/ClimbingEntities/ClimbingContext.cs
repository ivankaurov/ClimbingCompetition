// <copyright file="ClimbingContext.cs">
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

﻿using DbAccessCore;
using Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ClimbingEntities
{
    public static class WindowTitles
    {
        public const String USERS = "wnd_Users";
        public const String SETTINGS = "wnd_Settings";
    }

    public partial class ClimbingContext2: BaseContext
    {
        public const string PROJECT_NAME = "ClimbingCompetition2";

        protected ClimbingContext2(String connectionString) : base(connectionString) { }

        protected ClimbingContext2(String connectionString, Boolean isClone, Boolean isWeb, IPAddress clientIP, String clientHostName)
            : base(connectionString, isClone, isWeb, clientIP, clientHostName)
        { }

        protected override void SetInitializer(WhatToDo whatToDo)
        {
            switch (whatToDo)
            {
                case WhatToDo.CreateOrUpdate:
                    Database.SetInitializer<ClimbingContext2>(new MigrateDatabaseToLatestVersion<ClimbingContext2, ClimbingContextMigration2>(true));
                    return;
                case WhatToDo.LeaveAsIs:
                    Database.SetInitializer<ClimbingContext2>(new NullDatabaseInitializer<ClimbingContext2>());
                    return;
                default:
                    Database.SetInitializer<ClimbingContext2>(new DropCreateDatabaseAlways<ClimbingContext2>());
                    return;
            }
        }

        public static Task<ClimbingContext2> InitExistingDatabase(string connectionString, String applicationAdminUser, String applicationAdminPassword,
                                                             WhatToDo whatToDo, ProgressNotifier notifier = null, CancellationToken? token = null)
        {
            return BaseContext.InitContextOnExistingDBAsync(connectionString, whatToDo,
                cnString => new ClimbingContext2(cnString),
                cnString => new ClimbingContext2(cnString, false, false, null, null),
                applicationAdminUser, applicationAdminPassword, notifier, token ?? CancellationToken.None)
                .ContinueWith(tsk => (ClimbingContext2)tsk.Result, token ?? CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
        }

        public static Task InitDatabaseAsync(String serverName, String databaseName,
                                                             String sqlServerUser, String sqlServerPassword,
                                                             String applicationAdminUser, String applicationAdminPassword,
                                                             WhatToDo whatToDo, ProgressNotifier notifier = null, CancellationToken? token = null)
        {
            return BaseContext.InitDatabaseAsync(serverName, databaseName, PROJECT_NAME,
                                                 sqlServerUser, sqlServerPassword, applicationAdminUser, applicationAdminPassword,
                                                 whatToDo,
                                                 cnString => new ClimbingContext2(cnString),
                                                 cnString => new ClimbingContext2(cnString, false, false, null, null),
                                                 notifier, token ?? CancellationToken.None);
        }

        public static ClimbingContext2 LoginOnDesktop(String server, String database, String userName, String password)
        {
            return (ClimbingContext2)BaseContext.LoginOnDesktop(server, database, userName, password,
                                              cnString => new ClimbingContext2(cnString, false, false, null, null));
        }

        public static ClimbingContext2 CreateContextForBrowser(String connectionString, String userIdNameOrEmail, IPAddress clientIP, String clientHostName)
        {
            return (ClimbingContext2)BaseContext.CreateContextForBrowser(userIdNameOrEmail,
                () => new ClimbingContext2(connectionString, false, true, clientIP, clientHostName));
        }

        protected override IEnumerable<string> InitWindows(DbAccessCore.Log.LogicTransaction ltr)
        {
            return base.InitWindows(ltr)
                       .Concat(typeof(WindowTitles).GetFields()
                                                   .Where(f => f.IsLiteral)
                                                   .Select(f => f.GetValue(null) as String)
                                                   .Where(s => !String.IsNullOrWhiteSpace(s))
                                                   .Select(wt => AddWindow(wt, wt, null, null, ltr).Iid));
        }
    }
}