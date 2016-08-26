using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace Extensions
{
    public static class SqlExtensions
    {
        public static Task<SqlDataReader> ExecuteReaderAsync(this SqlCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            return Task.Factory.FromAsync((ac, s) => cmd.BeginExecuteReader(ac, s),
                ar => cmd.EndExecuteReader(ar), null);
        }

        public static Task<T> ExecuteReaderAsync<T>(this SqlCommand cmd, Func<SqlDataReader, T> resultFactory)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            if (resultFactory == null)
                throw new ArgumentNullException("resultFactory");


            var executeReaderTask = cmd.Connection.State == ConnectionState.Open ? cmd.ExecuteReaderAsync() :
                Task.Factory.StartNew(() => cmd.Connection.Open())
                            .ContinueWith(tsk =>
                            {
                                if (tsk.IsFaulted)
                                    throw tsk.Exception;
                                return cmd.ExecuteReaderAsync();
                            }).Unwrap();
            return executeReaderTask.ContinueWith(tsk =>
            {
                try { return resultFactory(tsk.Result); }
                finally { tsk.Result.Close(); }
            }, TaskContinuationOptions.OnlyOnRanToCompletion)
            .ContinueWith(tsk =>
            {
                if (executeReaderTask.IsFaulted)
                    throw executeReaderTask.Exception;
                return tsk.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<T[]> ExecuteReaderByLineAsync<T>(this SqlCommand cmd, Func<SqlDataReader, T> lineFactory)
        {
            if (lineFactory == null)
                throw new ArgumentNullException("lineFactory");
            return cmd.ExecuteReaderAsync(rdr =>
            {
                var res = new LinkedList<T>();
                while (rdr.Read())
                    res.AddLast(lineFactory(rdr));
                return res.ToArray();
            });
        }

        public static Task<object> ExecuteScalarAsync(this SqlCommand cmd)
        {
            return cmd.ExecuteReaderAsync(rdr =>
            {
                if (rdr.Read())
                    return rdr[0] == DBNull.Value ? null : rdr[0];
                else
                    return null;
            });
        }

        public static Task<T> ExecuteScalarAsync<T>(this SqlCommand cmd, Func<object, T> conversion = null)
        {
            return cmd.ExecuteReaderAsync(rdr =>
            {
                object result = rdr.Read() ? (rdr[0] == DBNull.Value ? null : rdr[0]) : null;
                if (conversion == null)
                    return result == null ? default(T) : (T)result;
                else
                    return conversion(result);
            });
        }

        public static Task<Int32> ExecuteNonQueryAsync(this SqlCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            if (cmd.Connection.State == ConnectionState.Open)
                return Task.Factory.FromAsync((ac, s) => cmd.BeginExecuteNonQuery(ac, s), ar => cmd.EndExecuteNonQuery(ar), null);
            var open = Task.Factory.StartNew(() => cmd.Connection.Open());

            return open.ContinueWith(tsk =>
                Task.Factory.FromAsync((ac, s) => cmd.BeginExecuteNonQuery(ac, s), ar => cmd.EndExecuteNonQuery(ar), null), TaskContinuationOptions.OnlyOnRanToCompletion)
                .Unwrap()
                .ContinueWith(tsk =>
                {
                    if (open.IsFaulted)
                        throw open.Exception;
                    return tsk.Result;
                }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
