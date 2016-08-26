using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using ClimbingCompetition.Common.API;

namespace ClimbingCompetition.Client
{
    [Serializable]
    public sealed class ServiceConnectionModel
    {
        public ApiCompetition Competition { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public string Serialize()
        {
            var serializer = new JsonSerializer();
            var stringBuilder = new StringBuilder();
            using (var sr = new StringWriter(stringBuilder))
            {
                serializer.Serialize(sr, this);
            }

            return stringBuilder.ToString();
        }

        public static ServiceConnectionModel FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(value))
            {
                return (ServiceConnectionModel)serializer.Deserialize(stringReader, typeof(ServiceConnectionModel));
            }
        }

        private static void CheckOnlineStorage(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");

            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT count(*) FROM sys.tables(nolock) WHERE name='st0_online_settings'";

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    return;

                cmd.CommandText = "CREATE TABLE st0_online_settings (setting_value nvarchar(max))";
                cmd.ExecuteNonQuery();
            }
        }

        public static ServiceConnectionModel FromSql(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");

            CheckOnlineStorage(cn);

            string result = null;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "SELECT top 1 setting_value FROM st0_online_settings(nolock)";
                result = cmd.ExecuteScalar() as string;
            }

            return string.IsNullOrEmpty(result) ? null : ServiceConnectionModel.FromString(result);
        }

        public void Persist(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");
            CheckOnlineStorage(cn);
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = "DELETE FROM st0_online_settings";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO st0_online_settings (setting_value) VALUES (@value)";
                    cmd.Parameters.Add("@value", System.Data.SqlDbType.NVarChar, -1).Value = this.Serialize();
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}