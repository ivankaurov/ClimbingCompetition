#if !DEBUG && !MARS
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