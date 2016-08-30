// <copyright file="MultiRouteBoulder.cs">
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
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ClimbingCompetition.DsMultiBouderTableAdapters;

namespace ClimbingCompetition
{
    public
#if FULL
    sealed 
#else
    static
#endif
        class MultiRouteBoulder
    {
#if FULL
        public const string NYA = "н/я";
        public const string DSQ = "дискв.";
        public const string ClmNUM = "ClimberID";
        public const string ResNUM = "ResIid";
        public const int NYARES = 5, DSQRES = 8, SIMPLERES = 9, MAXAT = 9999;
        public enum ListType { Start, Res, Print, Full }
        private readonly int listID;
        private readonly int routeNumber;
        private readonly boulderResultsTableAdapter bresTA = new boulderResultsTableAdapter();
        private readonly BoulderRoutesTableAdapter bRoutesTA = new BoulderRoutesTableAdapter();
        private readonly ParticipantsTableAdapter partTA = new ParticipantsTableAdapter();
        private readonly TeamsTableAdapter teamTA = new TeamsTableAdapter();

        private readonly DsMultiBouder ds = new DsMultiBouder();

        public MultiRouteBoulder(int listID, int routeNumber)
        {
            this.cn = null;
            this.routeNumber = routeNumber;
            this.listID = listID;
        }

        public SqlConnection cn
        {
            get { return bresTA.Connection; }
            set { bresTA.Connection = bRoutesTA.Connection = partTA.Connection = teamTA.Connection = value; }
        }

        public MultiRouteBoulder(int listID, SqlConnection cn)
        {
            this.listID = listID;
            this.cn = cn;
            SqlCommand cmd = new SqlCommand("SELECT routeNumber FROM lists WHERE iid = " + listID.ToString(),
                this.cn);
            object res = cmd.ExecuteScalar();
            if(res == null || res == DBNull.Value)
                throw new ArgumentNullException("Число трасс не введено");
            routeNumber = Convert.ToInt32(res);
            if (routeNumber <= 0)
                throw new ArgumentOutOfRangeException("Число трасс не может быть меньше или равным нулю");
            if (routeNumber > 99)
                throw new ArgumentOutOfRangeException("Число трасс не может быть больше 99");
        }

        public void FillData()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            ds.BoulderRoutes.Clear();
            ds.boulderResults.Clear();
            ds.Participants.Clear();
            teamTA.Fill(ds.Teams);
            partTA.FillByBoulderListID(ds.Participants,listID);
            bresTA.FillByListID(ds.boulderResults,listID);
            bRoutesTA.Fill(ds.BoulderRoutes);
            
        }

        private DataTable CreateStructure(bool createStart)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(ResNUM, typeof(long));
            if (createStart)
                dt.Columns.Add("Ст.№",typeof(int));
            else
                dt.Columns.Add("Место", typeof(string));
            dt.Columns.Add("№", typeof(int));
            dt.Columns.Add("ФамилияИмя", typeof(string));
            dt.Columns.Add("Г.р.", typeof(int));
            dt.Columns.Add("Разряд", typeof(string));
            dt.Columns.Add("Команда", typeof(string));
            for (int i = 1; i <= routeNumber; i++)
            {
                dt.Columns.Add("T" + i.ToString(), typeof(int));
                dt.Columns.Add("B" + i.ToString(), typeof(int));
            }
            dt.Columns.Add("Тр.", typeof(int));
            dt.Columns.Add("Попытки на трассы", typeof(int));
            dt.Columns.Add("Бон.", typeof(int));
            dt.Columns.Add("Попытки на бонусы", typeof(int));
            if (!createStart)
                dt.Columns.Add("Кв.", typeof(string));
            dt.Columns.Add(NYA, typeof(bool));
            dt.Columns.Add(DSQ, typeof(bool));
            dt.Columns.Add(ClmNUM, typeof(int));
            return dt;
        }

        private int StartComparison(DsMultiBouder.boulderResultsRow r1, DsMultiBouder.boulderResultsRow r2)
        {
            return r1.start.CompareTo(r2.start);
        }

        private int PosComparison(DsMultiBouder.boulderResultsRow r1, DsMultiBouder.boulderResultsRow r2)
        {
            if (r1.iid == r2.iid)
                return r1.iid.CompareTo(r2.iid);
            long res1 = (r1.IsresNull() ? 0 : r1.res);
            long res2 = (r2.IsresNull() ? 0 : r2.res);
            if (res1 != res2)
                return res2.CompareTo(res1);
            if (r1.pos != r2.pos)
                return r1.pos.CompareTo(r2.pos);
            if (r1.nya != r2.nya)
                return r1.nya.CompareTo(r2.nya);
            if (r1.disq != r2.disq)
                return r1.disq.CompareTo(r2.disq);
            if (r1.ParticipantsRow.vk != r2.ParticipantsRow.vk)
                return r1.ParticipantsRow.vk.CompareTo(r2.ParticipantsRow.vk);
            if (r1.ParticipantsRow.TeamsRow.name != r2.ParticipantsRow.TeamsRow.name)
                return r1.ParticipantsRow.TeamsRow.name.CompareTo(r2.ParticipantsRow.TeamsRow.name);
            if (r1.ParticipantsRow.surname != r2.ParticipantsRow.surname)
                return r1.ParticipantsRow.surname.CompareTo(r2.ParticipantsRow.surname);
            string sName1 = (r1.ParticipantsRow.IsnameNull() ? "" : r1.ParticipantsRow.name);
            string sName2 = (r2.ParticipantsRow.IsnameNull() ? "" : r2.ParticipantsRow.name);
            return sName1.CompareTo(sName2);
        }

        private int PosStartComparison(DsMultiBouder.boulderResultsRow r1, DsMultiBouder.boulderResultsRow r2)
        {
            if (r1.iid == r2.iid)
                return 0;
            long res1 = (r1.IsresNull() ? 0 : r1.res);
            long res2 = (r2.IsresNull() ? 0 : r2.res);
            if (res1 != res2)
                return res2.CompareTo(res1);
            if (r1.pos != r2.pos)
                return r1.pos.CompareTo(r2.pos);
            if (r1.nya != r2.nya)
                return r1.nya.CompareTo(r2.nya);
            if (r1.disq != r2.disq)
                return r1.disq.CompareTo(r2.disq);
            if (r1.ParticipantsRow.vk != r2.ParticipantsRow.vk)
                return r1.ParticipantsRow.vk.CompareTo(r2.ParticipantsRow.vk);
            return r1.start.CompareTo(r2.start);
        }

        public DataTable GetListFiltered(ListType lt)
        {
            DataTable dt = GetList(lt);
            if (dt.Columns.IndexOf(ClmNUM) > -1)
                dt.Columns.Remove(ClmNUM);
            if (dt.Columns.IndexOf(ResNUM) > -1)
                dt.Columns.Remove(ResNUM);
            if (dt.Columns.IndexOf("№") > -1)
                dt.Columns.Remove("№");
            if (dt.Columns.IndexOf(DSQ) > -1)
                dt.Columns.Remove(DSQ);
            if (dt.Columns.IndexOf(NYA) > -1)
                dt.Columns.Remove(NYA);
            return dt;
        }

        public DataTable GetList(ListType listType)
        {
            bool start = (listType == ListType.Start);
            DataTable dt = CreateStructure(listType == ListType.Start);
            List<DsMultiBouder.boulderResultsRow> rowList = new List<DsMultiBouder.boulderResultsRow>();
            foreach (DsMultiBouder.boulderResultsRow row in ds.boulderResults)
                rowList.Add(row);
            Comparison<DsMultiBouder.boulderResultsRow> cmp;
            if (start)
                cmp = StartComparison;
            else if (listType == ListType.Full)
                cmp = PosStartComparison;
            else
                cmp = PosComparison;
            rowList.Sort(cmp);
            int i = 0;
            List<DsMultiBouder.boulderResultsRow> emptyList = new List<DsMultiBouder.boulderResultsRow>();
            ListType lToFull;
            if (listType == ListType.Print)
                lToFull = ListType.Print;
            else
                lToFull = ListType.Res;
            SqlTransaction tran = cn.BeginTransaction();
            bool tranSuccess = false;
            try
            {
                while (i < rowList.Count)
                {
                    ListType lRes = CheckRow(rowList[i], tran);
                    if (lRes == ListType.Res || lRes == lToFull || listType == ListType.Full)
                        i++;
                    else
                    {
                        emptyList.Add(rowList[i]);
                        rowList.RemoveAt(i);
                    }
                }
                tranSuccess = true;
            }
            finally
            {
                if (tranSuccess)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            if (start)
                rowList = emptyList;
            foreach (var row in rowList)
            {
                int curI = 0;
                DataRow dr = dt.NewRow();
                dr[0] = row.iid;
                if (start)
                    dr[++curI] = row.start;
                else
                    dr[++curI] = (row.IsposTextNull() ? (object)DBNull.Value : (object)row.posText);
                dr[++curI] = row.ParticipantsRow.iid;
                dr[++curI] = row.ParticipantsRow.surname + (row.ParticipantsRow.IsnameNull() ? "" : " " + row.ParticipantsRow.name);
                dr[++curI] = (row.ParticipantsRow.IsageNull() ? (object)DBNull.Value : (object)row.ParticipantsRow.age);
                dr[++curI] = (row.ParticipantsRow.IsqfNull() ? (object)DBNull.Value : (object)row.ParticipantsRow.qf);
                dr[++curI] = row.teamName;//.ParticipantsRow.TeamsRow.name;
                for (i = 1; i <= routeNumber; i++)
                {
                    var rR = row.GetByRouteNumber(i);
                    if (rR == null)
                    {
                        dr[++curI] = DBNull.Value;
                        dr[++curI] = DBNull.Value;
                    }
                    else
                    {
                        dr[++curI] = (rR.IstopANull() ? (object)DBNull.Value : (object)rR.topA);
                        dr[++curI] = (rR.IsbonusANull() ? (object)DBNull.Value : (object)rR.bonusA);
                    }
                }
                dr[++curI] = row.tops;
                dr[++curI] = row.topAttempts;
                dr[++curI] = row.bonuses;
                dr[++curI] = row.bonusAttempts;
                if (!start)
                    dr[++curI] = (row.IsqfNull() ? (object)DBNull.Value : (object)row.qf);
                dr[++curI] = row.nya;
                dr[++curI] = row.disq;
                dr[++curI] = row.climber_id;
                dt.Rows.Add(dr);
                //dt.Rows.Add(row);
            }
            return dt;
        }

        //private enum ResType { NOTHING, NYA, DISQ, RES }

        public bool SaveRes(DataTable dt, SqlTransaction tr)
        {
            if (dt == null || dt.Rows.Count < 1)
                return true;
            int iidCol = dt.Columns.IndexOf(ResNUM),
                nyaCol = dt.Columns.IndexOf(NYA), dsqCol = dt.Columns.IndexOf(DSQ),
                clmNumCol = dt.Columns.IndexOf(ClmNUM);
            if (iidCol < 0)
                throw new ArgumentException("Неверная струткура таблицы. Столбец ResIid не найден.");
            if (nyaCol < 0)
                throw new ArgumentException("Неверная струткура таблицы. Столбец " + NYA + " не найден.");
            if (dsqCol < 0)
                throw new ArgumentException("Неверная струткура таблицы. Столбец " + DSQ + " не найден.");
            if (clmNumCol < 0)
                throw new ArgumentException("Неверная струткура таблицы. Столбец с номером участника не найден.");
            int[] topCol = new int[routeNumber], bonusCol = new int[routeNumber];
            for (int i = 1; i <= routeNumber; i++)
            {
                topCol[i - 1] = dt.Columns.IndexOf("T" + i.ToString());
                bonusCol[i - 1] = dt.Columns.IndexOf("B" + i.ToString());
                if (topCol[i - 1] < 0 || bonusCol[i - 1] < 0)
                    throw new ArgumentException("Неверная структура таблицы. Данные по трассе " + i.ToString() + " не найдены");
            }

            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction tran;
            if (tr == null)
                tran = cn.BeginTransaction();
            else
                tran = tr;
            bool transactionSuccess = false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.Parameters.Add("@iid", SqlDbType.BigInt);
            cmd.Parameters[0].Value = 0;
            cmd.Parameters.Add("@nya", SqlDbType.Bit);
            cmd.Parameters[1].Value = false;
            cmd.Parameters.Add("@dsq", SqlDbType.Bit);
            cmd.Parameters[2].Value = false;
            cmd.Parameters.Add("@t", SqlDbType.Int);
            cmd.Parameters.Add("@ta", SqlDbType.Int);
            cmd.Parameters.Add("@b", SqlDbType.Int);
            cmd.Parameters.Add("@ba", SqlDbType.Int);
            cmd.Parameters.Add("@res", SqlDbType.BigInt);
            for (int i = 3; i < cmd.Parameters.Count; i++)
                cmd.Parameters[i].Value = 0;
            string cmdDel = "DELETE FROM BoulderRoutes WHERE iid_parent = @iid";
            string cmdUpdate = "UPDATE boulderResults SET nya=@nya, disq=@dsq, changed = 1," +
                "tops = @t, topAttempts = @ta, bonuses = @b, bonusAttempts = @ba,res = @res WHERE iid=@iid";
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[iidCol] == DBNull.Value)
                        continue;
                    //ResType r;
                    bool nya = (dr[nyaCol] == DBNull.Value ? false : Convert.ToBoolean(dr[nyaCol]));
                    bool dsq = (dr[dsqCol] == DBNull.Value ? false : Convert.ToBoolean(dr[dsqCol]));
                    long id = Convert.ToInt64(dr[iidCol]);
                    cmd.Parameters[0].Value = id;
                    int t = 0, tA = 0, b = 0, bA = 0;
                    int? top, bonus;
                    if (nya)
                    {
                        cmd.CommandText = cmdDel;
                        cmd.ExecuteNonQuery();
                        //r = ResType.NYA;
                        cmd.Parameters[1].Value = true;
                        cmd.Parameters[2].Value = false;
                        cmd.Parameters[7].Value = NYARES;
                    }
                    else if (dsq)
                    {
                        cmd.CommandText = cmdDel;
                        cmd.ExecuteNonQuery();
                        //r = ResType.DISQ;
                        cmd.Parameters[1].Value = false;
                        cmd.Parameters[2].Value = true;
                        cmd.Parameters[7].Value = DSQRES;
                    }
                    else
                    {
                        cmd.Parameters[1].Value = cmd.Parameters[2].Value = false;
                        bool hasClimbed = false;
                        //r = ResType.RES;
                        for (int i = 0; i < topCol.Length; i++)
                        {
                            top = (dr[topCol[i]] == DBNull.Value ? null : (int?)Convert.ToInt32(dr[topCol[i]]));
                            bonus = (dr[bonusCol[i]] == DBNull.Value ? null : (int?)Convert.ToInt32(dr[bonusCol[i]]));
                            if (top != null && top > 0)
                            {
                                if (bonus == null)
                                    throw new ArgumentNullException("Бонус у участника №" + dr[ClmNUM].ToString() + " не введён.");
                                else if (bonus > top)
                                    throw new ArgumentOutOfRangeException("Бонус у участника №" + dr[ClmNUM].ToString() + " больше ТОПа.");
                            }
                            else if (top == null && bonus != null)
                                top = 0;
                            if (!hasClimbed && top != null && bonus != null)
                                hasClimbed = true;
                            t += (top != null && top > 0 ? 1 : 0);
                            tA += (top != null && top > 0 ? (int)top : 0);
                            b += (bonus != null && bonus > 0 ? 1 : 0);
                            bA += (bonus != null && bonus > 0 ? (int)bonus : 0);
                            SaveRes(i + 1, top, bonus, id, tran);
                        }
                        if (hasClimbed)
                        {
                            long res = t * 100000000000 + (MAXAT - tA) * 10000000 + b * 100000 + (MAXAT - bA) * 10 + SIMPLERES;
                            cmd.Parameters[7].Value = res;
                        }
                        else
                            cmd.Parameters[7].Value = DBNull.Value;
                    }
                    cmd.Parameters[3].Value = t;
                    cmd.Parameters[4].Value = tA;
                    cmd.Parameters[5].Value = b;
                    cmd.Parameters[6].Value = bA;

                    cmd.CommandText = cmdUpdate;
                    cmd.ExecuteNonQuery();
                }
                transactionSuccess = true;
            }
            finally
            {
                if (tr == null)
                {
                    if (transactionSuccess)
                        tran.Commit();
                    else
                        tran.Rollback();
                }
            }
            return true;
        }

        private void SaveRes(int routeNumber, int? top, int? bonus, long iid, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction trLocal;
            if (tran == null)
                trLocal = cn.BeginTransaction();
            else
                trLocal = tran;
            bool transactionSuccess = false;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trLocal;
                cmd.Connection = cn;
                if (top == null || bonus == null)
                {
                    cmd.CommandText = "DELETE FROM BoulderRoutes WHERE iid_parent = " + iid.ToString() +
                        " AND routeN = " + routeNumber.ToString();
                    cmd.ExecuteNonQuery();
                    transactionSuccess = true;
                    return;
                }
                cmd.CommandText = "SELECT COUNT(*) FROM BoulderRoutes WHERE iid_parent = " + iid.ToString() +
                    " AND routeN = " + routeNumber.ToString();
                int nTmp = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Add("@t", SqlDbType.Int);
                cmd.Parameters.Add("@b", SqlDbType.Int);
                cmd.Parameters[0].Value = (top == null ? DBNull.Value : (object)top);
                cmd.Parameters[1].Value = (bonus == null ? DBNull.Value : (object)bonus);
                switch (nTmp)
                {
                    case 0:
                        cmd.CommandText = "INSERT INTO BoulderRoutes(iid, iid_parent, routeN, topA, bonusA, changed)" +
                            " VALUES(@iid," + iid.ToString() + "," + routeNumber.ToString() + ",@t,@b,1)";
                        cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = StaticClass.GetNextIID("BoulderRoutes", cn, "iid", trLocal);
                        break;
                    case 1:
                        cmd.CommandText = "UPDATE BoulderRoutes SET topA=@t, bonusA=@b,changed=1 " +
                            "WHERE iid_parent = " + iid.ToString() + " AND routeN = " + routeNumber.ToString();
                        break;
                    default:
                        cmd.CommandText = "DELETE FROM BoulderRoutes WHERE iid_parent = " + iid.ToString() +
                        " AND routeN = " + routeNumber.ToString();
                        cmd.ExecuteNonQuery();
                        goto case 0;
                }
                cmd.ExecuteNonQuery();
                transactionSuccess = true;
            }
            finally
            {
                if (tran == null)
                {
                    if (transactionSuccess)
                        trLocal.Commit();
                    else
                        trLocal.Rollback();
                }
            }
        }

        private ListType CheckRow(DsMultiBouder.boulderResultsRow row, SqlTransaction tran)
        {
            DsMultiBouder.BoulderRoutesRow[] rLst = row.GetBoulderRoutesRows();
            if (row.nya || row.disq)
                return ListType.Res;
            if (rLst == null || rLst.Length < 1)
                return ListType.Start;
            List<DsMultiBouder.BoulderRoutesRow> rowL = new List<DsMultiBouder.BoulderRoutesRow>(rLst);
            rowL.Sort(new Comparison<DsMultiBouder.BoulderRoutesRow>(
                delegate(DsMultiBouder.BoulderRoutesRow r1, DsMultiBouder.BoulderRoutesRow r2)
                {
                    return r1.routeN.CompareTo(r2.routeN);
                }));
            bool hasNothing = true;
            bool hasEverything = (routeNumber <= rowL.Count);
            
            for (int i = 1; i <= routeNumber && i <= rowL.Count; i++)
            {
                if (rowL[i - 1].IsbonusANull() || rowL[i - 1].IstopANull())
                    hasEverything = false;
                else
                    hasNothing = false;
                if (rowL[i - 1].routeN != i)
                    hasEverything = false;
            }
            if (routeNumber < rowL.Count)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "DELETE FROM BoulderRoutes WHERE iid_parent=" + row.iid.ToString() +
                    " AND routeN > " + routeNumber.ToString();
                cmd.ExecuteNonQuery();
            }
            if (hasNothing)
                return ListType.Start;
            if (hasEverything)
                return ListType.Res;
            return ListType.Print;
        }
#endif

        public static void CheckTableExists(SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = @"IF NOT EXISTS(SELECT * 
                                                FROM sysobjects
                                               WHERE name='BoulderRoutes'
                                                 AND type = 'U')
                                   CREATE TABLE BoulderRoutes(
                                      iid BIGINT PRIMARY KEY,
                               iid_parent BIGINT FOREIGN KEY REFERENCES boulderResults(iid) ON DELETE CASCADE ON UPDATE CASCADE,
                                   routeN INT    NOT NULL,
                                     topA INT    NULL,
                                   bonusA INT    NULL,
                                  changed BIT    NOT NULL DEFAULT 1)";
            cmd.ExecuteNonQuery();
        }
    }
}
