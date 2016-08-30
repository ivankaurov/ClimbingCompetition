// <copyright file="BaseContext.Init.cs">
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

﻿using Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbAccessCore
{
    partial class BaseContext
    {
        public const String ADMIN_GROUP = "0055AM991663452298";

        public enum WhatToDo { LeaveAsIs, CreateOrUpdate, DropAndCreate, CreateNew }

        protected virtual void SetUserAsAdmin(Users.DbUser user)
        {
            var adminGroup = this.UserGroups.FirstOrDefault(grp => grp.Iid.Equals(ADMIN_GROUP, StringComparison.OrdinalIgnoreCase));
            if (adminGroup == null)
                adminGroup = this.UserGroups.Add(new Users.DbUserGroup(this, ADMIN_GROUP)
                {
                    Name = CoreTranslations.Admins
                });
            adminGroup.AddUser(user, this);
        }

        protected virtual IEnumerable<String> InitWindows(Log.LogicTransaction ltr) { yield break; }

        protected abstract void SetInitializer(WhatToDo whatToDo);

        void DoInitWindows(Log.LogicTransaction ltr)
        {
            var _ltr = ltr == null ? this.BeginLtr("INIT_WND") : ltr;
            var existingWindows = new HashSet<String>(InitWindows(_ltr));
            this.Windows.Where(wnd => !existingWindows.Contains(wnd.Iid))
                        .ToList()
                        .ForEach(wnd => wnd.RemoveObject(this, _ltr));
            if (ltr == null)
                _ltr.Commit(this);
        }

        Windows.ActionDescriptor AddAction(Windows.WindowDescriptor window, String actionKey, String actionName, Log.LogicTransaction ltr)
        {
            var action = window.ChildActions.FirstOrDefault(act => act.ActionKey.Equals(actionKey, StringComparison.OrdinalIgnoreCase));
            if(action == null)
            {
                window.ChildActions.Add(action = new Windows.ActionDescriptor(this, actionKey, window)
                {
                    ActionName = actionName
                });
                if (ltr != null)
                    ltr.AddCreatedObject(action, this);
            }
            else if(!String.Equals(action.ActionName, actionName, StringComparison.Ordinal))
            {
                if (ltr != null)
                    ltr.AddUpdatedObjectBefore(action, this);
                action.ActionName = actionName;
                if (ltr != null)
                    ltr.AddUpatedObjectAfter(action, this);
            }
            return action;
        }

        protected Windows.WindowDescriptor AddWindow(String windowId, String windowName, Windows.WindowDescriptor parentWindow, IEnumerable<Tuple<String,String>> actions, Log.LogicTransaction ltr)
        {
            var window = this.Windows.FirstOrDefault(wnd => wnd.Iid.Equals(windowId, StringComparison.OrdinalIgnoreCase));
            if (window == null)
            {
                window = this.Windows.Add(new Windows.WindowDescriptor(this, windowId, windowName)
                {
                    ChildActions = new List<Windows.ActionDescriptor>(),
                    ChildWindows = new List<Windows.WindowDescriptor>()
                });
                if (ltr != null)
                    ltr.AddCreatedObject(window, this);
            }
            else if(!window.Name.Equals(windowName, StringComparison.Ordinal) || window.ParentWindow!= parentWindow)
            {
                if (ltr != null)
                    ltr.AddUpdatedObjectBefore(window, this);
                window.ParentWindow = parentWindow;
                if (parentWindow != null && !parentWindow.ChildWindows.Contains(window))
                    parentWindow.ChildWindows.Add(window);
                window.Name = windowName;
                if (ltr != null)
                    ltr.AddUpatedObjectAfter(window, this);
            }
            
            if(actions == null && window.ChildActions.Count > 0)
            {
                foreach (var action in window.ChildActions)
                    action.RemoveObject(this, ltr);
                window.ChildActions.Clear();
            }
            else if(actions!= null)
            {
                var addedActions = new HashSet<String>(actions.Select(act =>
                {
                    AddAction(window, act.Item1, act.Item2, ltr);
                    return act.Item1;
                }));

                foreach(var action in window.ChildActions.Where(act => !addedActions.Contains(act.ActionKey)))
                {
                    action.RemoveObject(this, ltr);
                    window.ChildActions.Remove(action);
                }
            }

            return window;
        }

        void Init(ProgressNotifier notifier, String applicationAdminUser, String applicationAdminPassword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            notifier.OnInitProgress(55, CoreTranslations.InitDbAdmin);
            var user = this.Users.FirstOrDefault(u => u.UserName.Equals(applicationAdminUser, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                user = this.Users.Add(new Users.DbUser(this)
                {
                    UserName = applicationAdminUser
                });
            user.LogicallyDeleted = false;
            user.AllowMultipleLogins = false;
            user.NeedChangePassword = false;
            user.SetPassword(applicationAdminPassword);
            this.SetCurrentUser(user);
            token.ThrowIfCancellationRequested();
            this.SaveChanges();

            notifier.OnInitProgress(60, CoreTranslations.InitDbFill);
            DoInitWindows(null);
            token.ThrowIfCancellationRequested();
            this.SaveChanges();

            notifier.OnInitProgress(70, CoreTranslations.InitDbAdmin);
            SetUserAsAdmin(user);
            token.ThrowIfCancellationRequested();
            this.SaveChanges();

        }

        protected static Task<BaseContext> InitContextOnExistingDBAsync(string contextConnectionString, WhatToDo whatToDo,
                                                             Func<String, BaseContext> contextInitConstructor,
                                                             Func<String, BaseContext> contextJobConstructor,
                                                             String applicationAdminUser, String applicationAdminPassword,
                                                             ProgressNotifier notifier, CancellationToken token)
        {
            if (whatToDo == WhatToDo.CreateNew || whatToDo == WhatToDo.DropAndCreate)
                throw new ArgumentOutOfRangeException("whatToDo", whatToDo, "Value not supported");
            if (notifier == null)
                notifier = new ProgressNotifier();
            return Task<BaseContext>.Factory.StartNew(() =>
            {
                notifier.OnInitProgress(10, CoreTranslations.InitDbCreation);
                using (var result = contextInitConstructor(contextConnectionString))
                {
                    if (whatToDo == WhatToDo.LeaveAsIs)
                        result.SetInitializer(WhatToDo.LeaveAsIs);
                    else
                    {
                        result.SetInitializer(WhatToDo.CreateOrUpdate);
                    }
                    result.Database.ExecuteSqlCommand("select top 1 * from sys.tables");
                }
                BaseContext context = null;
                try
                {
                    context = contextJobConstructor(contextConnectionString);
                    context.CreateCreateNewIidProc();
                    context.Init(notifier, applicationAdminUser, applicationAdminPassword, token);
                    return context;
                }
                catch
                {
                    if (context != null)
                        context.Dispose();
                    throw;
                }
            }, token);
        }

        protected static Task InitDatabaseAsync(String serverName, String databaseName, String projectName,
                                                             String sqlServerUser, String sqlServerPassword,
                                                             String applicationAdminUser, String applicationAdminPassword,
                                                             WhatToDo whatToDo,
                                                             Func<String, BaseContext> contextInitConstructor,
                                                             Func<String, BaseContext> contextJobConstructor,
                                                             ProgressNotifier notifier, CancellationToken token)
        {
            if (String.IsNullOrEmpty(serverName))
                throw new ArgumentNullException("serverName");
            if (String.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException("databaseName");
            if (String.IsNullOrEmpty(projectName))
                throw new ArgumentNullException("projectName");
            if (String.IsNullOrEmpty(applicationAdminUser))
                throw new ArgumentNullException(applicationAdminUser);
            if (String.IsNullOrEmpty(applicationAdminPassword))
                throw new ArgumentNullException("applicationAdminPassword");

            if (notifier == null)
                notifier = new ProgressNotifier();

            var systemConnectionString =
                String.IsNullOrEmpty(sqlServerUser) ? SqlCore.Connector.CreateConnectionString(serverName) :
                SqlCore.Connector.CreateConnectionString(serverName, sqlServerUser, sqlServerPassword);

            var contextConnectionString = SqlCore.Connector.CreateInternalConnectionString(serverName, databaseName);

            notifier.OnInitProgress(0, CoreTranslations.InitServer);            

            Task tskInitSql;

            if(whatToDo == WhatToDo.CreateNew || whatToDo == WhatToDo.DropAndCreate)
            {
                var cmd = new SqlCommand("select count(*) from master..sysdatabases where name = @dbName", new SqlConnection(systemConnectionString));
                cmd.Parameters.Add("@dbName", System.Data.SqlDbType.NVarChar, 255).Value = databaseName;

                tskInitSql = cmd.ExecuteScalarAsync(res => Convert.ToInt32(res) > 0)
                                .ContinueWith(tsk =>
                                {
                                    if (tsk.Result)
                                    {
                                        if (whatToDo == WhatToDo.CreateNew)
                                            throw new ApplicationException(String.Format(CoreTranslations.DatabaseExists, databaseName));
                                        cmd.CommandText = "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = @dbName";
                                        cmd.ExecuteNonQuery();
                                        cmd.Connection.ChangeDatabase("master");
                                        cmd.CommandText = String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseName);
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = String.Format("DROP DATABASE [{0}]", databaseName);
                                        cmd.ExecuteNonQuery();
                                    }
                                    return SqlCore.Initializer.InitServerAsync(systemConnectionString, projectName, databaseName, token);
                                }, token)
                                .Unwrap()
                                .AttachAction(tsk =>
                                {
                                    cmd.Connection.Dispose();
                                    cmd.Dispose();
                                }, TaskContinuationOptions.ExecuteSynchronously);
            }
            else
                tskInitSql = SqlCore.Initializer.InitServerAsync(systemConnectionString, projectName, databaseName, token);

            var tskCreateContext = tskInitSql.ContinueWith(tsk =>
            {
                notifier.OnInitProgress(10, CoreTranslations.InitDbCreation);
                using (var result = contextInitConstructor(contextConnectionString))
                {
                    if (whatToDo == WhatToDo.LeaveAsIs)
                        result.SetInitializer(WhatToDo.LeaveAsIs);
                    else
                    {
                        result.SetInitializer(WhatToDo.CreateOrUpdate);
                        result.Database.CreateIfNotExists();
                    }
                    result.Database.ExecuteSqlCommand("select top 1 * from sys.tables");
                }
                return contextJobConstructor(contextConnectionString);
            }, token, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);

            // сюда токен отмены не передаем. если до сюда дошли, надо завершить создание БД
            var tskFixDatabase = tskCreateContext.ContinueWith(tsk =>
            {
                notifier.OnInitProgress(50, CoreTranslations.InitServer);
                tskCreateContext.Result.CreateCreateNewIidProc();
                return SqlCore.Initializer.FixDatabaseAsync(contextConnectionString);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current)
            .Unwrap();

            var tskInitContext = tskFixDatabase.ContinueWith(tsk => tskCreateContext.Result.Init(notifier, applicationAdminUser, applicationAdminPassword, token),
                token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

            // токен-сорс для отмены возвращаемой задачи
            var resultCts = new CancellationTokenSource();

            return Task.Factory.ContinueWhenAll(new Task[] { tskInitSql, tskCreateContext, tskFixDatabase, tskInitContext },
                allTasks =>
                {
                    try
                    {
                        var firstFaultedTask = allTasks.FirstOrDefault(t => t.IsFaulted);
                        if (firstFaultedTask != null)
                            firstFaultedTask.Exception.Flatten().Handle(ex => ex is OperationCanceledException);

                        // чтобы метод вернул отмененную задачу, дождавшись завершения задач
                        if (token.IsCancellationRequested)
                            resultCts.Cancel();

                        notifier.OnInitProgress(100, CoreTranslations.InitCompleted);
                    }
                    finally
                    {
                        if (tskCreateContext.Status == TaskStatus.RanToCompletion)
                            tskCreateContext.Result.Dispose();
                    }
                }, TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith(tsk =>
                {
                    if (tsk.IsFaulted)
                        throw tsk.Exception;
                }, resultCts.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }
}