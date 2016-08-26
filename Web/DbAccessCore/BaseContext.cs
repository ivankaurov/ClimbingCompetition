using System;
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
