using ClimbingCompetition.Client;
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