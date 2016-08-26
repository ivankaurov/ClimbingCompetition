using DbAccessCore;
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