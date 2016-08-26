using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    public static class ServiceHelper
    {
        public const string REMOTE_ID_COLUMN = "RemoteIID";
        public const int REMOTE_ID_COL_SIZE = 50;

        public static void CheckRemoteIdColumn(string tableName, SqlConnection cn, SqlTransaction tran)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (cn == null)
                throw new ArgumentNullException("cn");

            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();

            using(var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = tran ?? cmd.Connection.BeginTransaction();
                try
                {
                    cmd.CommandText = "select count(*)" +
                                      "  from sys.tables T(nolock)" +
                                      "  join sys.columns C(nolock) on C.object_id = T.object_id" +
                                      " where T.name = @tName" +
                                      "   and C.name = '" + ServiceHelper.REMOTE_ID_COLUMN + "'";
                    cmd.Parameters.Add("@tName", SqlDbType.VarChar, 255).Value = tableName;

                    var resultObj = cmd.ExecuteScalar();
                    if (resultObj == null || resultObj == DBNull.Value || Convert.ToInt32(resultObj) == 0)
                    {
                        cmd.CommandText = String.Format("ALTER TABLE {0} ADD {1} VARCHAR({2}) NULL", tableName, ServiceHelper.REMOTE_ID_COLUMN, ServiceHelper.REMOTE_ID_COL_SIZE);
                        cmd.ExecuteNonQuery();
                    }

                    if (tran == null)
                        cmd.Transaction.Commit();
                }
                catch
                {
                    if (tran == null)
                        cmd.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
