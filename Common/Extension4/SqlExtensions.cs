// <copyright file="SqlExtensions.cs">
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

#if NET40
﻿using System;
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
#endif