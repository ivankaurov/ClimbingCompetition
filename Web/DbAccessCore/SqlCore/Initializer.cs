// <copyright file="Initializer.cs">
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
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Extensions;
using System.Threading;

namespace DbAccessCore.SqlCore
{
    public sealed class Initializer : IDisposable
    {
        public const String SQL_LOGIN = "CLIMBING22C9ABF7A06A4A6482CC43A442AC3F35";
        internal const String SQL_PASSWORD = "{6FEDA7C1-5957-4BAE-p@$$!word-AE2D-AF917B83DC8A}";
        private const String PROJECTS_TABLE = "all_projects";

        SqlConnection cn;
        String projectName, database;

        Task CreateSystemLoginAsync()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.Connection = cn;
                cmd.CommandText = "select count(*) cnt" +
                                  "  from master.sys.sql_logins(nolock)" +
                                  " where name = '" + SQL_LOGIN + "'";


                var loginExists = cmd.ExecuteScalarAsync(obj => Convert.ToInt32(obj));

                var create = loginExists.ContinueWith(tsk =>
                {

                    if (cmd.Connection.Database != "master")
                        cmd.Connection.ChangeDatabase("master");
                    cmd.CommandText = tsk.Result > 0 ? " ALTER " : "CREATE ";

                    cmd.CommandText += "LOGIN [" + SQL_LOGIN + "] WITH PASSWORD=N'" + SQL_PASSWORD + "'" +
                                        ", DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON";
                    return cmd.ExecuteNonQueryAsync();

                }, TaskContinuationOptions.OnlyOnRanToCompletion)
                .Unwrap();

                return create.ContinueWith(tsk =>
                {
                    cmd.CommandText = "EXEC master..sp_addsrvrolemember @loginame = N'" + SQL_LOGIN + "', @rolename = N'sysadmin'";
                    // cmd.CommandText = "ALTER SERVER ROLE [sysadmin] ADD MEMBER [" + SQL_LOGIN + "]";
                    return cmd.ExecuteNonQueryAsync();
                }, TaskContinuationOptions.OnlyOnRanToCompletion)
                .Unwrap()
                .ContinueWith(tsk =>
                {
                    new Task[] { loginExists, create, tsk }.ProcessMultipleTasksResults(checkForCancelled: false);
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
            catch
            {
                cmd.Dispose();
                throw;
            }
        }

        Task InitProjectsTableAsync()
        {
            SqlCommand cmd = null;
            SqlTransaction tran = null;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = cn;
                if (cmd.Connection.Database != "master")
                    cmd.Connection.ChangeDatabase("master");



                cmd.CommandText = @"
                      if object_id('" + PROJECTS_TABLE + @"') is null
                          create table " + PROJECTS_TABLE + "(database_name varchar(255), project_name varchar(255))";
                tran = cmd.Transaction = cmd.Connection.BeginTransaction();

                var createTable = cmd.ExecuteNonQueryAsync();


                return createTable.ContinueWith(tsk =>
                {
                    cmd.Parameters.Add("@database", System.Data.SqlDbType.VarChar, 255).Value = database;
                    cmd.Parameters.Add("@project", System.Data.SqlDbType.VarChar, 255).Value = projectName;

                    cmd.CommandText = @"
                      delete " + PROJECTS_TABLE + @"
                        from " + PROJECTS_TABLE + @" PT(nolock)
                   left join sys.databases D(nolock) on D.name = PT.database_name
                       where D.name is null

                      if not exists(select 1
                                      from " + PROJECTS_TABLE + @"(nolock)
                                     where database_name=@database
                                       and project_name=@project)
                        insert into " + PROJECTS_TABLE + @"(database_name, project_name)
                        values(@database, @project)";
                    return cmd.ExecuteNonQueryAsync();
                }, TaskContinuationOptions.OnlyOnRanToCompletion)
                .Unwrap()
                .ContinueWith(tsk =>
                {
                    if (tsk.Status == TaskStatus.RanToCompletion)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                    if (createTable.IsFaulted)
                        throw createTable.Exception;
                    if (tsk.IsFaulted)
                        throw tsk.Exception;
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
            catch
            {
                if (tran != null)
                    tran.Rollback();
                if (cmd != null)
                    cmd.Dispose();
                throw;
            }
        }

        Task FixOwnerAsync()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.Connection = cn;
                if (cmd.Connection.Database != database)
                    cmd.Connection.ChangeDatabase(database);
                cmd.CommandText = "sp_changedbowner " + SQL_LOGIN;
                return cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                cmd.Dispose();
                throw;
            }
        }


        Task FixLogAsync()
        {
            var cmd = new SqlCommand();
            try
            {

                cmd.Connection = cn;
                if (cmd.Connection.Database != "master")
                    cmd.Connection.ChangeDatabase("master");
                cmd.CommandText = String.Format("ALTER DATABASE [{0}] SET RECOVERY SIMPLE WITH NO_WAIT", database);
                return cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                cmd.Dispose();
                throw;
            }
        }

        SqlCommand CreateEnumerateDatabasesCommand()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.Connection = cn;
                cmd.CommandText = "select distinct D.name" +
                                  "  from master.sys.databases D(nolock)" +
                                  "  join " + PROJECTS_TABLE + " P(nolock) on P.database_name = D.name" +
                                  " where P.project_name = @prj " +
                                "order by D.name";
                cmd.Parameters.Add("@prj", System.Data.SqlDbType.VarChar, 255).Value = projectName;
                return cmd;
            }
            catch
            {
                cmd.Dispose();
                throw;
            }
        }

        Task<String[]> EnumerateDatabasesAsync()
        {
            return CreateEnumerateDatabasesCommand().ExecuteReaderByLineAsync(rdr => (string)rdr[0]);
        }


        public void Dispose() { Dispose(true); }
        void Dispose(bool disposing)
        {
            if (disposing)
                cn.Dispose();
        }
        ~Initializer() { Dispose(false); }

        Initializer(String connectionString, String projectName, String database)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (projectName == null)
                throw new ArgumentNullException("projectName");
            this.cn = new SqlConnection(connectionString);
            this.cn.Open();
            this.database = database;
            this.projectName = projectName;
        }

        static Task<Initializer> CreateInitializerAsync(String connectionString, String projectName, String database)
        {
            return Task.Factory.StartNew(() => new Initializer(connectionString, projectName, database));
        }

        public static Task InitServerAsync(String connectionString, String projectName, String database, CancellationToken token)
        {
            var init = CreateInitializerAsync(connectionString, projectName, database);
            var createLogin = init.ContinueWith(tsk => tsk.Result.CreateSystemLoginAsync(), token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default)
                .Unwrap();

            return createLogin.ContinueWith(tsk => init.Result.InitProjectsTableAsync(), token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default)
                              .Unwrap()
                              .ContinueWith(tsk =>
                              {
                                  new Task[] { init, createLogin, tsk }.ProcessMultipleTasksResults();
                              }, token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public static Task<String[]> EnumerateDatabasesAsync(String server, String projectName)
        {
            var tskCreate = Task.Factory.StartNew(() => new Initializer(Connector.CreateInternalConnectionString(server), projectName, "master"));

            return tskCreate.ContinueWith(tsk => tsk.Result.EnumerateDatabasesAsync(), TaskContinuationOptions.OnlyOnRanToCompletion)
                            .Unwrap()
                            .ContinueWith(tsk =>
                            {
                                if (tskCreate.IsFaulted)
                                    throw tskCreate.Exception;
                                tskCreate.Result.Dispose();
                                return tsk.Result;
                            });
        }

        public static Task FixDatabaseAsync(String internalConnectionString)
        {
            SqlConnectionStringBuilder sBuilder = new SqlConnectionStringBuilder(internalConnectionString);
            var initializer = CreateInitializerAsync(sBuilder.ConnectionString, "tmp_project", sBuilder.InitialCatalog);

            return initializer.ContinueWith(tsk => tsk.Result.FixOwnerAsync())
                              .Unwrap()
                              .ContinueWith(tsk =>
                              {
                                  if (tsk.IsFaulted)
                                      throw tsk.Exception;
                                  return initializer.Result.FixLogAsync();
                              })
                              .Unwrap()
                              .ContinueWith(tsk =>
                              {
                                  if (initializer.Status == TaskStatus.RanToCompletion)
                                      initializer.Result.Dispose();
                                  if (tsk.IsFaulted)
                                      throw tsk.Exception.Flatten();
                              }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}