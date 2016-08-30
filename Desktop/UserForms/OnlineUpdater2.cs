// <copyright file="OnlineUpdater2.cs">
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

﻿using ClimbingCompetition.Client;
using ClimbingCompetition.Common.API;
using Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    public sealed partial class OnlineUpdater2 : IDisposable
    {
        private static readonly MyConcurrentDictionary<string, OnlineUpdater2> updaters = new MyConcurrentDictionary<string, OnlineUpdater2>();

        private readonly SqlConnection cn;
        private SqlTransaction currentTransaction = null;

        private OnlineUpdater2(string connectionString)
        {
            this.cn = new SqlConnection(connectionString);
            this.cn.Open();

            try { ServiceClient.GetInstance(cn); }
            catch
            {
                cn.Close();
                throw;
            }
        }

        public void Dispose() { this.cn.Dispose(); }

        public static OnlineUpdater2 GetUpdater(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");
            return OnlineUpdater2.updaters.GetOrAdd(cn.ConnectionString, connectionString => new OnlineUpdater2(connectionString));
        }

        public static OnlineUpdater2 Instance { get { return updaters.Values.FirstOrDefault(); } }

        private SqlCommand CreateCommand()
        {
            if (this.cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = this.cn;
                if (this.currentTransaction != null)
                    cmd.Transaction = this.currentTransaction;
                return cmd;
            }
            catch
            {
                if (cmd != null)
                    cmd.Dispose();
                throw;
            }
        }

        private void BeginTransaction()
        {
            if (this.currentTransaction != null)
                throw new InvalidOperationException("Transaction is active");
            this.currentTransaction = this.cn.BeginTransaction();
        }

        private void DoActionWithTransaction(Action<SqlTransaction> action)
        {
            if (this.currentTransaction == null)
                throw new InvalidOperationException("Transaction is inactive");
            if (action != null)
                action(this.currentTransaction);
            this.currentTransaction.Dispose();
            this.currentTransaction = null;
        }

        private void RollbackTransaction()
        {
            this.DoActionWithTransaction(tran => tran.Rollback());
        }

        private void CommitTransaction()
        {
            this.DoActionWithTransaction(tran => tran.Commit());
        }

        private void CleanInvalidTransaction()
        {
            if (this.currentTransaction != null)
                this.DoActionWithTransaction(null);
        }
    }
}