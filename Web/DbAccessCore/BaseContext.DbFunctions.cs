// <copyright file="BaseContext.DbFunctions.cs">
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
using Crypto;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
#if NET40 || NET451
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
#endif

namespace DbAccessCore
{
    partial class BaseContext
    {
#if NET40 || NET451
        private DbContextTransaction currentTransaction;
#else
        private IDbContextTransaction currentTransaction;
#endif
        private readonly object physicalTransactionLocker = new object();

        public bool HasTransaction
        {
            get { return this.currentTransaction != null; }
        }

        public void BeginPhysicalTransaction()
        {
            lock (physicalTransactionLocker)
            {
                if (this.currentTransaction != null)
                    throw new InvalidOperationException("Physical transaction is already active.");

                this.currentTransaction = this.Database.BeginTransaction();
            }
        }

        public void CommitPhysicalTransaction()
        {
            lock (physicalTransactionLocker)
            {
                if (this.currentTransaction == null)
                    throw new InvalidOperationException("Physical transaction is inactive");
                this.currentTransaction.Commit();
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }

        public void RollbackPhysicalTransaction()
        {
            lock (physicalTransactionLocker)
            {
                if (this.currentTransaction == null)
                    throw new InvalidOperationException("Physical transaction is incative");
                this.currentTransaction.Rollback();
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }

        SqlCommand CreateCommand(String commandText = null)
        {
            lock (physicalTransactionLocker)
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                SqlCommand cmd = null;
                try
                {
                    cmd = new SqlCommand();
                    cmd.Connection = connection;
                    if (commandText != null)
                        cmd.CommandText = commandText;
                    if (this.currentTransaction != null)
#if NET40 || NET451
                        cmd.Transaction = this.currentTransaction.UnderlyingTransaction as SqlTransaction;
#else
                        cmd.Transaction = this.currentTransaction.GetDbTransaction() as SqlTransaction;
#endif
                    return cmd;
                }
                catch
                {
                    if (cmd != null)
                        cmd.Dispose();
                    throw;
                }
            }
        }

        Object SqlExecuteScalar(SqlCommand cmd)
        {
            var result = cmd.ExecuteScalar();
            return result == DBNull.Value ? null : result;
        }

        Object SqlExecuteScalar(String query)
        {
            return SqlExecuteScalar(CreateCommand(query));
        }

        T? SqlExecuteScalar<T>(SqlCommand cmd, Func<Object, T> converter)
            where T : struct
        {
            var result = SqlExecuteScalar(cmd);
            return result == null ? null : new T?(converter(result));
        }

        T? SqlExecuteScalar<T>(String query, Func<Object, T> converter)
            where T : struct
        {
            return SqlExecuteScalar(CreateCommand(query), converter);
        }

        readonly Lazy<int> spid;
        public int SPID { get { return spid.Value; } }

        readonly Lazy<String> dbHash;
        public String DB_HASH { get { return dbHash.Value; } }

        readonly TimeSpan timeDiff;
        public DateTime Now { get { return (DateTime.Now - timeDiff).ToUniversalTime(); } }

        DateTime? GetLoginTime(int spid)
        {
            using (var cmd = CreateCommand("select login_time from master.sys.sysprocesses(nolock) where spid=@spid"))
            {
                cmd.Parameters.Add("@spid", SqlDbType.Int).Value = spid;
                return SqlExecuteScalar(cmd, res => Convert.ToDateTime(res).ToUniversalTime());
            }
        }
        readonly Lazy<DateTime> loginTime;
        public DateTime LoginTime { get { return loginTime.Value; } }

        readonly Lazy<String> sqlHostName;
        public String SqlHostName { get { return sqlHostName.Value; } }

        Boolean ObjectExists(String objectName)
        {
            using (var cmd = CreateCommand("select count(*) from sysobjects(nolock) where name = @objName"))
            {
                cmd.Parameters.Add("@objName", SqlDbType.VarChar, -1).Value = objectName;
                return Convert.ToInt32(SqlExecuteScalar(cmd)) > 0;
            }
        }

        const String CREATE_IID_PROC = "createIid";
        void CreateCreateNewIidProc()
        {
            if (ObjectExists(CREATE_IID_PROC))
                return;
            using (var cmd = CreateCommand())
            {
                if (!ObjectExists("iid_gen_table"))
                {
                    cmd.CommandText = @"
                      create table iid_gen_table(
                        spid int not null primary key,
                        stub bit,
                        tms  rowversion)";
                    cmd.ExecuteNonQuery();
                }
                StringBuilder sbLeftData = new StringBuilder();
                for (int i = 0; i < BaseObject.IID_SIZE; i++)
                    sbLeftData.Append("0");

                cmd.CommandText = String.Format(@"
create proc dbo.{3}(@RP_sNewIid varchar({0}) out, @P_sDBHASH varchar(4) = '{1}')
as begin
  update iid_gen_table with(rowlock)
     set stub = 1
   where spid = @@spid

  if (@@rowcount = 0) begin
    begin tran
      insert into iid_gen_table(spid,stub)
      values(@@spid, 0)
    commit tran
  end

  select @RP_sNewIid = right('{2}' +
                             convert(varchar({0}), convert(bigint, T.tms)) +
                             right('0000' + @P_sDBHASH, 4) +
                             right('000' + convert(varchar(30), @@spid), 3), {0})
    from iid_gen_table T(nolock)
   where T.spid = @@spid
end", BaseObject.IID_SIZE, DB_HASH, sbLeftData, CREATE_IID_PROC);

                cmd.ExecuteNonQuery();
            }
        }

        public String CreateNewIid()
        {
            using (var cmd = CreateCommand(CREATE_IID_PROC))
            {
                cmd.Parameters.Add("@P_sDBHASH", SqlDbType.VarChar, 4).Value = DB_HASH;
                var result = cmd.Parameters.Add("@RP_sNewIid", SqlDbType.VarChar, BaseObject.IID_SIZE);
                result.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                return (String)result.Value;
            }
        }

        public void GetObjectCreationValues(out String newIid, out DateTime serverTime)
        {
            using (var cmd = CreateCommand(@"
    exec dbo." + CREATE_IID_PROC + @" @RP_sNewIid = @RP_sNewIid out, @P_sDBHASH = @sDBHASH
    set @RP_dtNow = GetDate()"))
            {
                cmd.Parameters.Add("@sDBHASH", SqlDbType.VarChar, 4).Value = DB_HASH;
                var newIidP = cmd.Parameters.Add("@RP_sNewIid", SqlDbType.VarChar, BaseObject.IID_SIZE);
                newIidP.Direction = ParameterDirection.Output;

                var currentDTP = cmd.Parameters.Add("@RP_dtNow", SqlDbType.DateTime);
                currentDTP.Direction = ParameterDirection.Output;

                do
                {
                    cmd.ExecuteNonQuery();
                } while ((newIid = newIidP.Value as String) == null || currentDTP.Value == DBNull.Value);
                serverTime = Convert.ToDateTime(currentDTP.Value).ToUniversalTime();
            }
        }

        public Task<String> CreateNewIidAsync()
        {
            return CreateCommand(String.Format(@"
                        declare @sResult varchar({0})
                        exec dbo.{1} @RP_sNewIid = @sResult out, @P_sDBHASH = '{2}'
                        select @sResult result
                        ", BaseObject.IID_SIZE, CREATE_IID_PROC, DB_HASH))
#if NET40 || NET451
                        .ExecuteScalarAsync(res => (String)res);
#else
                        .ExecuteScalarAsync().ContinueWith(t => t.Result?.ToString());
#endif
        }
    }
}
