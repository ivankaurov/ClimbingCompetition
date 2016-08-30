// <copyright file="ListCreator.cs">
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ClimbingCompetition;
using System.ComponentModel;
using XmlApiData;

namespace ClimbingCompetition.Online
{
    /// <summary>
    /// ������������ ��� ������-���������� ��� ���������� ����������� �� �����
    /// </summary>
    public sealed class ListCreator : IDisposable
    {
        private class ListCrLst
        {
            public DataTable dt = null;
            public RoundType rt = RoundType.NOTHING;
            public bool hasPreQf = false;
            public int iid = -1;
        }
        private List<ListCrLst> loadedLists;
        private enum RoundType { ORDINARY, TWO_ROUTES, FLASH, NOTHING, GENERAL }
        private SqlConnection cn;
        private int listID;
        private SqlCommand cmd;
        private SpeedRules cRules;
        private long compID;
        private ListCreator(SqlConnection cn, int listID, List<ListCrLst> loadedLists, long compID)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            this.compID = compID;
            this.cn = cn;
            this.listID = listID;
            cmd = new SqlCommand();
            cmd.Connection = this.cn;
            if (loadedLists == null)
                this.loadedLists = new List<ListCrLst>();
            else
                this.loadedLists = loadedLists;
        }
        //~ListCreator()
        //{
        //    try { cn.Close(); }
        //    catch { }
        //}

        /// <summary>
        /// �������� �������� �� ��������� iid'�
        /// </summary>
        /// <param name="listID">iid ���������</param>
        /// <param name="cn">������������ �����������</param>
        /// <returns>DataTable - ������� ��������</returns>
        [Description("�������� �������� �� ��������� iid'�")]
        public static DataTable GetResultList(int listID, SqlConnection cn, long compID)
        {
            ListCreator lc = new ListCreator(cn, listID, null, compID);
            lc.cRules = SortingClass.GetCompRules(cn, true, compID);
            RoundType rt;
            DataTable dt = lc.GetResultList(out rt, true, false);
            if (dt != null)
                try
                {
                    if (dt.Columns.IndexOf("iid") > -1)
                        dt.Columns["iid"].ColumnName = "�";
                    if (dt.Columns.IndexOf("pos") > -1)
                        dt.Columns.Remove("pos");
                    if (dt.Columns.IndexOf("pts") > -1)
                        dt.Columns.Remove("pts");
                    if (dt.Columns.IndexOf("vk") > -1)
                        dt.Columns.Remove("vk");
                    if (dt.Columns.IndexOf("nya") > -1)
                        dt.Columns.Remove("nya");
                    if (dt.Columns.IndexOf("disq") > -1)
                        dt.Columns.Remove("disq");
                    if (dt.Columns.IndexOf("NpreQf") > -1)
                    {
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        //    if (Convert.ToInt32(dt.Rows[i]["NpreQf"]) < 1)
                        //    {
                        //        dt.Rows.RemoveAt(i);
                        //        i--;
                        //    }
                        dt.Columns.Remove("NpreQf");
                    }
                    if (dt.Columns.IndexOf("�") > 0)
                    {
                        bool needToClose = false;
                        if (cn.State != ConnectionState.Open)
                        {
                            needToClose = true;
                            cn.Open();
                        }
                        try
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = "SELECT secretary_id" +
                                              "  FROM ONLClimberCompLink(NOLOCK)" +
                                              " WHERE iid = @iid";
                            cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    cmd.Parameters[0].Value = Convert.ToInt64(dr["�"]);
                                    object oRes = cmd.ExecuteScalar();
                                    dr["�"] = oRes == null ? DBNull.Value : oRes;
                                }
                                catch { }
                            }
                        }
                        finally
                        {
                            if (needToClose)
                                cn.Close();
                        }
                    }
                }
                catch { }
            return dt;
        }

        private DataTable GetResultList(out RoundType rt, bool fullList, bool inclPreQf)
        {
            bool bTmp;
            return GetResultList(out rt, fullList, inclPreQf, out bTmp);
        }

        private ListTypeEnum listType = ListTypeEnum.Unknown;

        private DataTable GetResultList(out RoundType rt, bool fullList, bool inclPreQf, out bool hasPreQf)
        {
            foreach (ListCrLst lst in loadedLists)
                if (lst.iid == this.listID)
                {
                    rt = lst.rt;
                    hasPreQf = lst.hasPreQf;
                    return lst.dt;
                }
            hasPreQf = (checkPreQf(listID) > 0);
            rt = RoundType.NOTHING;
            DataTable dtRet = null;
            cmd.CommandText = "SELECT ISNULL(style,'') style, ISNULL(round,'') round," +
                              "       ISNULL(listType,'" + ListTypeEnum.Unknown + "') listType" +
                              "  FROM ONLlists(NOLOCK)" +
                              " WHERE iid=" + listID.ToString();
            string style, round;
            if (cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr.Read())
                {
                    style = dr["style"].ToString().ToLower().Trim();
                    round = dr["round"].ToString().ToLower().Trim();
                    try { listType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), dr["listType"].ToString().Trim(), true); }
                    catch { listType = ListTypeEnum.Unknown; }
                }
                else
                    return null;
            }
            finally { dr.Close(); }

            switch (listType)
            {
                case ListTypeEnum.General:
                    rt = RoundType.GENERAL;
                    dtRet = CreateGeneralResults();
                    break;
                case ListTypeEnum.LeadFlash:
                    rt = RoundType.FLASH;
                    dtRet = CreateFlash(inclPreQf);
                    break;
                case ListTypeEnum.LeadGroups:
                    rt = RoundType.TWO_ROUTES;
                    dtRet = Create2Routes(true, inclPreQf);
                    break;
                case ListTypeEnum.LeadSimple:
                    rt = RoundType.ORDINARY;
                    dtRet = CreateLead(round.IndexOf("����") > -1, inclPreQf);
                    break;
                case ListTypeEnum.BoulderGroups:
                    rt = RoundType.TWO_ROUTES;
                    dtRet = Create2Routes(false, inclPreQf);
                    break;
                case ListTypeEnum.BoulderSimple:
                    rt = RoundType.ORDINARY;
                    dtRet = CreateBoulder(fullList, inclPreQf);
                    break;
                case ListTypeEnum.BoulderSuper:
                    goto case ListTypeEnum.LeadSimple;
                case ListTypeEnum.SpeedFinal:
                    rt = RoundType.ORDINARY;
                    roundT rnd = round.Equals("�����") ? roundT.FINAL : roundT.PAIRING;
                    dtRet = CreateSpeed(rnd, fullList, inclPreQf);
                    break;
                case ListTypeEnum.SpeedQualy:
                    rt = RoundType.ORDINARY;
                    dtRet = CreateSpeed(roundT.QUALI, fullList, inclPreQf);
                    break;
                case ListTypeEnum.SpeedQualy2:
                    try
                    {
                        cmd.CommandText = "SELECT ParamValue" +
                                          "  FROM ONLCompetitionParams" +
                                          " WHERE comp_id = " + compID.ToString() +
                                          "   AND ParamName = 'Speed2ndQualy'";
                        bool res;
                        try { res = bool.Parse(cmd.ExecuteScalar().ToString()); }
                        catch { res = false; }
                        if (!res)
                            listType = ListTypeEnum.SpeedQualy;
                    }
                    catch { }
                    goto case ListTypeEnum.SpeedQualy;
            }

            
            if (dtRet != null)
            {
                ListCrLst lst = new ListCrLst();
                lst.iid = listID;
                lst.dt = dtRet;
                lst.hasPreQf = hasPreQf;
                lst.rt = rt;
                loadedLists.Add(lst);
            }
            return dtRet;
        }

        CompetitionRules CompRules { get { return ((cRules & SpeedRules.InternationalRules) == SpeedRules.InternationalRules) ? CompetitionRules.International : CompetitionRules.Russian; } }

        private DataTable Create2Routes(bool lead, bool inclPreQf)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = "SELECT iid, round FROM ONLlists(NOLOCK) WHERE iid_parent = " + listID.ToString();
            int r1 = -1, r2 = -1;
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                {
                    string rnd = rdr["round"].ToString().ToLower();
                    if (rnd.IndexOf("������ 1") > -1 || rnd.IndexOf(" �") > -1)
                        r1 = Convert.ToInt32(rdr["iid"]);
                    else
                        r2 = Convert.ToInt32(rdr["iid"]);
                }
            }
            finally { rdr.Close(); }

            if (r1 * r2 <= 0)
                return null;
            DataTable dtR1;
            DataTable dtR2;
            if (lead)
                GetCntLead(r1, r2, out dtR1, out dtR2, inclPreQf);
            else
                GetCntBoulder(r1, r2, out dtR1, out dtR2, inclPreQf);

            if (dtR1.Rows.Count < 1)
                return null;
            foreach (DataRow dr in dtR1.Rows)
                if (Convert.ToInt32(dr["pos"]) == 1)
                    dr["pts"] = 1.0;
            SortingClass.SortByColumn(dtR1, 0, dtR1.Rows.Count, dtR1.Columns.IndexOf("pts"));
            
            int curPos = 1, posCnt = 1;
            double curPts = Convert.ToDouble(dtR1.Rows[0]["pts"]);
            dtR1.Rows[0]["pos"] = curPos;
            for (int i = 1; i < dtR1.Rows.Count; i++)
            {
                double fPts = Convert.ToDouble(dtR1.Rows[i]["pts"]);
                if (fPts == curPts)
                    posCnt++;
                else
                {
                    curPos += posCnt;
                    curPts = fPts;
                    posCnt = 1;
                }
                dtR1.Rows[i]["pos"] = curPos;
            }
            dtR2 = SortingClass.CreateStructure();
            dtR2.Columns.Add("pos0", typeof(int));
            foreach (DataRow dr in dtR1.Rows)
            {
                //cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                object[] row = new object[9];
                row[0] = Convert.ToBoolean(dr["vk"]);
                row[1] = 0;
                row[2] = "";
                row[3] = Convert.ToInt32(dr["iid"]);
                row[4] = "";
                row[5] = 0.0;
                row[6] = "";
                string str;
                if (lead)
                    str = (dr["������ 1"].ToString() + dr["������ 2"].ToString()).ToLower();
                else
                    str = dr["�����"].ToString();
                if (str.IndexOf("�����") > -1)
                    row[7] = 1;
                else if (str.IndexOf("�/�") > -1)
                    row[7] = 2;
                else
                    row[7] = 0;
                row[8] = Convert.ToInt32(dr["pos"]);
                dtR2.Rows.Add(row);
            }
            SortingClass.SortResults(dtR2, true, false);
            foreach(DataRow dr1 in dtR1.Rows)
                foreach(DataRow dr2 in dtR2.Rows)
                    if (dr1["iid"].ToString() == dr2["iid"].ToString())
                    {
                        string strTmp = dr1["�����"].ToString().ToLower();
                        if (strTmp.IndexOf("�/�") < 0 && strTmp.IndexOf("�����") < 0)
                            dr1["�����"] = dr2["posText"].ToString();
                        dr1["pos"] = Convert.ToInt32(dr2["pos"]);
                        dr1["pts"] = Convert.ToDouble(dr2["pts"]);
                        break;
                    }
            SortingClass.SortTwoCases(dtR1, "pts", "vk");
            dtR1.Columns.Remove("��.�");
            return dtR1;
        }

        private void GetCntLead(int r1, int r2, out DataTable dtR1, out DataTable dtR2, bool inclPreQf)
        {
            ListCreator lcTmp = new ListCreator(cn, r1, loadedLists, compID);
            dtR1 = lcTmp.CreateLead(false, inclPreQf);
            lcTmp = new ListCreator(cn, r2, loadedLists, compID);
            dtR2 = lcTmp.CreateLead(false, inclPreQf);
            if (cn.State != ConnectionState.Open)
                cn.Open();
            RemoveEmptyRows(dtR1);
            RemoveEmptyRows(dtR2);
            //List<DataRow> toRemove = new List<DataRow>();
            //foreach (DataRow dr in dtR1.Rows)
            //    if (dr["���-�"].ToString().Length > 0)
            //        dr["����"] = "";
            //    else
            //        toRemove.Add(dr);
            //foreach (DataRow dr in toRemove)
            //    dtR1.Rows.Remove(dr);
            foreach (DataRow dr in dtR1.Rows)
                dr["����"] = "";
            foreach (DataRow dr in dtR2.Rows)
            {
                dr["����"] = dr["���-�"].ToString();
                dr["���-�"] = "";
                dtR1.Rows.Add(dr.ItemArray);
            }
            dtR2.Rows.Clear();

            dtR1.Columns["����"].ColumnName = "������ 2";
            dtR1.Columns["���-�"].ColumnName = "������ 1";
        }

        private void GetCntBoulder(int r1, int r2, out DataTable dtR1, out DataTable dtR2, bool inclPreQf)
        {
            ListCreator lcTmp = new ListCreator(cn, r1, loadedLists, compID);
            dtR1 = lcTmp.CreateBoulder(false, inclPreQf);
            lcTmp = new ListCreator(cn, r2, loadedLists, compID);
            dtR2 = lcTmp.CreateBoulder(false, inclPreQf);
            if (cn.State != ConnectionState.Open)
                cn.Open();
            //List<DataRow> toRemove = new List<DataRow>();
            //foreach (DataRow dr in dtR1.Rows)
            //    if (dr["�����"].ToString().Length < 1)
            //        toRemove.Add(dr);
            //foreach (DataRow dr in toRemove)
            //    dtR1.Rows.Remove(dr);
            RemoveEmptyRows(dtR2);
            RemoveEmptyRows(dtR1);
            dtR1.Columns.Add("��.(�)", typeof(int));
            dtR1.Columns.Add("���.�� ��.(�)", typeof(int));
            dtR1.Columns.Add("���.(�)", typeof(int));
            dtR1.Columns.Add("���.�� ���.(�)", typeof(int));
            dtR2.Columns.Add("��.(�)", typeof(int));
            dtR2.Columns.Add("���.�� ��.(�)", typeof(int));
            dtR2.Columns.Add("���.(�)", typeof(int));
            dtR2.Columns.Add("���.�� ���.(�)", typeof(int));
            foreach (DataRow dr in dtR2.Rows)
            //if (dr["�����"].ToString().Length > 0)
            {
                foreach (DataColumn dc in dtR2.Columns)
                    if ((dc.ColumnName.ToLower().IndexOf("��.") > -1 ||
                        dc.ColumnName.IndexOf("��.") > -1) && dc.ColumnName.IndexOf("(�)") < 0)
                    {
                        dr[dc.ColumnName + "(�)"] = dr[dc];
                        dr[dc] = DBNull.Value;
                    }
                dtR1.Rows.Add(dr.ItemArray);
            }
            dtR2.Rows.Clear();
            foreach (DataColumn dc in dtR1.Columns)
                if ((dc.ColumnName.ToLower().IndexOf("��.") > -1 ||
                    dc.ColumnName.IndexOf("��.") > -1) && dc.ColumnName.IndexOf("(�)") < 0)
                    dc.ColumnName += "(�)";
            dtR1.Columns.Add("qf", typeof(string));
            foreach (DataRow dr in dtR1.Rows)
                dr["qf"] = dr["��."].ToString();
            dtR1.Columns.Remove("��.");
            dtR1.Columns["qf"].ColumnName = "��.";
            //dtR1.Columns["����"].ColumnName = "������ 2";
            //dtR1.Columns["���-�"].ColumnName = "������ 1";
        }

        public class RoundData
        {
            public int id { get; private set; }
            public string roundName { get; private set; }
            public int prQ = 0;
            public string ptsCol { get; private set; }
            public string resCol { get; private set; }
            public string ptsTextCol { get; private set; }
            public RoundData(int id, string roundName, string ptsCol, string resCol, string ptsTextCol)
            {
                this.id = id; 
                this.roundName = roundName;
                this.ptsCol = ptsCol;
                this.resCol = resCol;
                this.ptsTextCol = ptsTextCol;
            }
        }

        private DataTable CreateFlash(bool inclPreQf)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = "SELECT iid, round FROM ONLlists(NOLOCK) WHERE iid_parent = " + listID.ToString() +
                " ORDER BY round, iid";
            List<RoundData> rounds = new List<RoundData>();
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                int iCnt = 0;
                while (rdr.Read())
                {
                    iCnt++;
                    rounds.Add(new RoundData(Convert.ToInt32(rdr["iid"]), rdr["round"].ToString(),
                        "pts" + iCnt.ToString(), "��." + iCnt.ToString(), "���� " + iCnt.ToString()));
                }
            }
            finally { rdr.Close(); }
            if (rounds.Count < 1)
                return null;
            cmd.CommandText = "SELECT quote FROM ONLlists(NOLOCK) WHERE iid = " + listID.ToString();
            int q;
            try
            {
                q = Convert.ToInt32(cmd.ExecuteScalar());
                if (inclPreQf)
                {
                    int maxPrQ = int.MinValue;
                    cmd.CommandText = "SELECT COUNT(*) FROM ONLlistdata(NOLOCK) WHERE preQf = 1 AND iid = @iid";
                    cmd.Parameters.Add("@iid", SqlDbType.Int);
                    for (int ij = 0; ij < rounds.Count; ij++)
                    {
                        cmd.Parameters[0].Value = rounds[ij].id;
                        try { rounds[ij].prQ = Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch { rounds[ij].prQ = 0; }
                        if (maxPrQ < rounds[ij].prQ)
                            maxPrQ = rounds[ij].prQ;
                    }
                    q += maxPrQ;
                }
            }
            catch { q = 0; }

            ListCreator lcTmp = new ListCreator(cn, rounds[0].id, loadedLists, compID);
            DataTable dtR1 = lcTmp.CreateLead(true, inclPreQf);
            RemoveEmptyRows(dtR1);
            if (dtR1.Rows.Count < 1)
                return null;
            dtR1.Columns["���-�"].ColumnName = rounds[0].resCol;
            dtR1.Columns["pts"].ColumnName = rounds[0].ptsCol;
            dtR1.Columns["����"].ColumnName = rounds[0].ptsTextCol;

            for (int i = 1; i < rounds.Count; i++)
            {
                dtR1.Columns.Add(rounds[i].resCol, typeof(string));
                dtR1.Columns.Add(rounds[i].ptsCol, typeof(double));
                dtR1.Columns.Add(rounds[i].ptsTextCol, typeof(string));
                lcTmp.listID = rounds[i].id;
                DataTable dt2 = lcTmp.CreateLead(true, inclPreQf);
                RemoveEmptyRows(dt2);
                List<DataRow> toRemove = new List<DataRow>();
                foreach (DataRow dr1 in dtR1.Rows)
                {
                    bool exists = false;
                    
                    foreach (DataRow dr2 in dt2.Rows)
                        if (dr1["iid"].ToString() == dr2["iid"].ToString())
                        {
                            dr1[rounds[i].resCol] = dr2["���-�"].ToString();
                            dr1[rounds[i].ptsCol] = Convert.ToDouble(dr2["pts"]);
                            dr1[rounds[i].ptsTextCol] = dr2["����"].ToString();
                            exists = true;
                            break;
                        }
                    if (!exists)
                        toRemove.Add(dr1);
                }
                foreach (DataRow dr in toRemove)
                    dtR1.Rows.Remove(dr);
            }
            if (dtR1.Rows.Count < 1)
                return null;
            dtR1.Columns.Add("total", typeof(double));
            dtR1.Columns.Add("totalTxt", typeof(string));
            dtR1.Columns.Add("totalInt", typeof(long));
            dtR1.Columns.Add("totalPts", typeof(double));
            foreach (DataRow dr in dtR1.Rows)
            {
                double val = 1.0;
                foreach (RoundData rd in rounds)
                    val *= Convert.ToDouble(dr[rd.ptsCol]);
                val = Math.Pow(val, 1.0 / ((double)rounds.Count));
                dr["total"] = val;
                long vl = long.Parse((val * 100.0).ToString("0"));
                dr["totalInt"] = vl;
                dr["totalTxt"] = val.ToString("0.00");
            }

            SortingClass.SortByColumn(dtR1, 0, dtR1.Rows.Count, dtR1.Columns.IndexOf("total"));
            dtR1.Columns.Add("posTotal", typeof(int));
            int curPos = 1;
            long curPts = Convert.ToInt64(dtR1.Rows[0]["totalInt"]);
            dtR1.Rows[0]["posTotal"] = curPos;
            int posCnt = 1;
            for (int i = 1; i < dtR1.Rows.Count; i++)
            {
                long fPts = Convert.ToInt64(dtR1.Rows[i]["totalInt"]);
                if (fPts == curPts)
                    posCnt++;
                else
                {
                    curPts = fPts;
                    curPos += posCnt;
                    posCnt = 1;
                }
                dtR1.Rows[i]["posTotal"] = curPos;
            }

            DataTable dtR2 = SortingClass.CreateStructure();
            dtR2.Columns.Add("pos0", typeof(int));
            dtR1.Columns.Add("sp", typeof(int));
            foreach (DataRow dr in dtR1.Rows)
            {
                bool nYa = true;
                bool dsq = true;
                foreach (RoundData rd in rounds)
                {
                    string res = dr[rd.resCol].ToString().ToLower();
                    if (res == "�/�")
                        continue;
                    else if (res == "�����.")
                    {
                        if (nYa)
                        {
                            nYa = false;
                            dsq = true;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        nYa = dsq = false;
                        break;
                    }
                }
                if (nYa)
                {
                    dr["sp"] = 2;
                    dr["totalTxt"] = "�/�";
                }
                else if (dsq)
                {
                    dr["sp"] = 1;
                    dr["totalTxt"] = "�����.";
                }
                else
                    dr["sp"] = 0;
                if (nYa || dsq)
                    foreach (RoundData rd in rounds)
                        dr[rd.ptsTextCol] = "";
                object[] row = new object[9];
                //cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                row[0] = Convert.ToBoolean(dr["vk"]);
                row[1] = 0;
                row[2] = "";
                row[3] = Convert.ToInt32(dr["iid"]);
                row[4] = "";
                row[5] = 0.0;
                row[6] = "";
                row[7] = Convert.ToInt32(dr["sp"]);
                row[8] = Convert.ToInt32(dr["posTotal"]);
                dtR2.Rows.Add(row);
            }
            //cmd.Parameters.Clear();
            
            SortingClass.SortResults(dtR2, q, true, false, CompRules);

            //while (dtR1.Columns.Count > 6)
            //    dtR1.Columns.RemoveAt(dtR1.Columns.Count - 1);
            dtR1.Columns.Add("qf", typeof(string));
            foreach(DataRow dr1 in dtR1.Rows)
                foreach (DataRow dr2 in dtR2.Rows)
                    if (dr1["iid"].ToString() == dr2["iid"].ToString())
                    {
                        dr1["posTotal"] = Convert.ToInt32(dr2["pos"]);
                        dr1["�����"] = dr2["posText"].ToString();
                        dr1["qf"] = dr2["qf"].ToString();
                        dr1["totalPts"] = dr2["pts"];
                        break;
                    }
            SortingClass.SortByColumn(dtR1, 0, dtR1.Rows.Count, dtR1.Columns.IndexOf("totalPts"));

            dtR1.Columns.Remove("pos");
            //dtR1.Columns.Remove("pts");
            dtR1.Columns.Remove("��.�");
            //dtR1.Columns.Remove("pts2");
            dtR1.Columns.Remove("total");
            dtR1.Columns.Remove("totalInt");
            dtR1.Columns.Remove("sp");
            dtR1.Columns.Remove("��.");
            foreach (RoundData rd in rounds)
                dtR1.Columns.Remove(rd.ptsCol);

            //dtR1.Columns["���-�"].ColumnName = "��.1";
            //dtR1.Columns["����"].ColumnName = "���� 1";
            //dtR1.Columns["res2"].ColumnName = "��.2";
            //dtR1.Columns["ptsText2"].ColumnName = "���� 2";
            dtR1.Columns["totalTxt"].ColumnName = "���-�";
            dtR1.Columns["totalPts"].ColumnName = "pts";
            dtR1.Columns["posTotal"].ColumnName = "pos";
            dtR1.Columns["qf"].ColumnName = "��.";
            foreach (DataRow dr in dtR1.Rows)
                if (Convert.ToInt32(dr["NpreQf"]) < 1)
                    dr["���-�"] = "";

            return dtR1;
        }

        private DataTable CreateLead(bool isQuali, bool inclPreQf)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandText = @"SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText,  
                                       CASE ll.res WHEN '�����.' THEN 1
                                                   WHEN '�/�' THEN 2
                                                   ELSE 0 END sp, 
                                       CASE l.preQf WHEN 0 THEN l.pos ELSE 0 END pos0
                                  FROM ONLlistdata l(NOLOCK)
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLlead ll(NOLOCK) ON ll.iid_line = l.iid_line
                                 WHERE (ISNULL(ll.res,'') <> '' ";
            if(inclPreQf)
                cmd.CommandText+= " OR l.preQf = 1 ";
            cmd.CommandText += ")  AND l.iid=" + listID.ToString();
            if(!inclPreQf)
                cmd.CommandText+=" AND l.preQf = 0 ";
            cmd.CommandText += " ORDER BY l.pos";
            DataTable dt = new DataTable();
            DataTable dtRes = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                int roundCnt = 0;
                List<int> lsts = CreateListSeq();
                foreach (int i in lsts)
                {
                    RoundType rt;
                    ListCreator lcTmp = new ListCreator(cn, i, loadedLists, compID);
                    bool hasPreQf;
                    DataTable dtI = lcTmp.GetResultList(out rt, false, true, out hasPreQf);
                    this.loadedLists = lcTmp.loadedLists;
                    if (dtI == null || rt == RoundType.NOTHING)
                        continue;
                    roundCnt++;
                    DataColumn dc1I = null, dc2I = null, dPQ = null;
                    if (rt == RoundType.TWO_ROUTES || (hasPreQf && dtI.Columns.IndexOf("NpreQf") > -1))
                    {
                        dt.Columns.Add("rNum" + roundCnt.ToString());
                        if (rt == RoundType.TWO_ROUTES)
                        {

                            foreach (DataColumn dcI in dtI.Columns)
                                if (dcI.ColumnName.ToLower().IndexOf("������") > -1)
                                {
                                    if (dcI.ColumnName.IndexOf("1") > -1)
                                        dc1I = dcI;
                                    else
                                        dc2I = dcI;
                                }
                            if (dc1I == null || dc2I == null)
                                continue;
                        }
                        dPQ = dtI.Columns["NpreQf"];
                    }
                    dt.Columns.Add("pos" + roundCnt.ToString());
                    foreach (DataRow dr in dt.Rows)
                        foreach (DataRow drI in dtI.Rows)
                            if (dr["iid"].ToString() == drI["iid"].ToString())
                            {
                                if (dPQ != null)
                                {
                                    if (Convert.ToInt32(drI[dPQ]) < 1)
                                        dr[dt.Columns.Count - 2] = 3;
                                    else
                                        dr[dt.Columns.Count - 2] = 1;
                                }
                                if (rt == RoundType.TWO_ROUTES)
                                {
                                    if (dPQ != null && Convert.ToInt32(drI[dPQ]) < 1)
                                        dr[dt.Columns.Count - 2] = 3;
                                    else if (drI[dc1I].ToString().Length > 0)
                                        dr[dt.Columns.Count - 2] = 1;
                                    else
                                        dr[dt.Columns.Count - 2] = 2;
                                }

                                dr[dt.Columns.Count - 1] = Convert.ToInt32(drI["pos"]);
                                break;
                            }
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT ISNULL(quote,0) quote FROM ONLlists(NOLOCK) WHERE iid=" + listID.ToString();
                int quote = Convert.ToInt32(cmd.ExecuteScalar());
                //if (inclPreQf)
                //    quote += checkPreQf(listID);
                SortingClass.SortResults(dt, quote, lsts.Count < 1, isQuali, CompRules);
                if (inclPreQf)
                {
                    cmd.CommandText = "SELECT l.preQf FROM ONLlistdata l(NOLOCK) " +
                                      " WHERE l.climber_id = @c AND l.iid=" + listID.ToString();
                    cmd.Parameters.Add("@c", SqlDbType.Int);
                    dt.Columns.Add("preQf", typeof(bool));
                    foreach (DataRow dr in dt.Rows)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                        try { dr["preQf"] = Convert.ToBoolean(cmd.ExecuteScalar()); }
                        catch { dr["preQf"] = false; }
                    }
                    cmd.Parameters.Clear();
                    int pqC = 0;
                    foreach (DataRow dr in dt.Rows)
                        if ((bool)dr["preQf"])
                        {
                            pqC++;
                            dr["pts"] = 0.1;
                            dr["ptsText"] = "";
                        }
                        else
                        {
                            dr["pts"] = Convert.ToDouble(dr["pts"]) - (double)pqC;
                            double d;
                            if (double.TryParse(dr["ptsText"].ToString(), out d))
                                dr["ptsText"] = (d - (double)pqC).ToString("0.00");
                        }
                }
                cmd.CommandText = @"SELECT '' �����, c.iid, cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������, ll.res [���-�], '' ����, '' [��.], 0 pos, 0.0 pts,
                                       l.start [��.�], c.vk, CASE l.preQf WHEN 0 THEN 1 ELSE 0 END NpreQf
                                  FROM ONLlead ll(NOLOCK)
                                  JOIN ONLlistdata        l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id
                                 WHERE (ISNULL(ll.res,'') <> '' ";
                if (inclPreQf)
                    cmd.CommandText += " OR l.preQf = 1 ";
                cmd.CommandText += ")  AND l.iid=" + listID.ToString();
                if (!inclPreQf)
                    cmd.CommandText += " AND l.preQf = 0 ";
                da.SelectCommand = cmd;
                if (da.SelectCommand.Connection.State != ConnectionState.Open)
                    da.SelectCommand.Connection.Open();
                da.Fill(dtRes);

            }
            foreach (DataRow dtR in dtRes.Rows)
                foreach (DataRow dr in dt.Rows)
                    if (dr["iid"].ToString() == dtR["iid"].ToString())
                    {
                        dtR["�����"] = dr["posText"].ToString();
                        dtR["����"] = dr["ptsText"].ToString();
                        dtR["��."] = dr["qf"].ToString();
                        if (dr["pos"] != DBNull.Value && dr["pos"] != null)
                            dtR["pos"] = Convert.ToInt32(dr["pos"]);
                        else
                            dtR["pos"] = DBNull.Value;
                        if (dr["pts"] != DBNull.Value && dr["pts"] != null)
                            dtR["pts"] = Convert.ToDouble(dr["pts"]);
                        else
                            dtR["pts"] = DBNull.Value;
                        break;
                    }
            if (dtRes.Rows.Count > 0)
                SortingClass.SortTwoCases(dtRes, "pts", "vk");
            //SortingClass.SortByColumn(dtRes, 0, dtRes.Rows.Count, dtRes.Columns.IndexOf("pos"));

            cmd.CommandText = @"SELECT '' �����,c.iid,cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������, ll.res [���-�], '' ����, '' [��.], 0 pos, 0.0 pts,
                                       l.start [��.�], c.vk, 1 NpreQf
                                  FROM ONLlead ll(NOLOCK)
                                  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id
                                 WHERE l.iid = " + listID.ToString() +
                                @" AND ISNULL(ll.res,'') = ''
                                   AND l.preQf = 0
                              ORDER BY l.start";
            if (cn.State != ConnectionState.Open)
                cn.Open();
            da.SelectCommand = cmd;
            DataTable dtStart = new DataTable();
            da.Fill(dtStart);
            SortingClass.SortTwoCases(dtRes, "NpreQf", "pts");
            MergeResStart(ref dtRes, ref dtStart);
            
            //foreach (DataRow dr in dtRes.Rows)
            //    if (Convert.ToInt32(dr["NpreQf"]) < 1)
            //    {
            //        dr["pts"] = 0.2;
            //        dr["pos"] = 1;
            //    }
            return dtRes;
        }

        private DataTable CreateBoulder(bool fullList, bool inclPreQf)
        {
            bool fRound = true;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandText = @"SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText,
                                       CASE WHEN ll.nya = 1 THEN 2
                                            WHEN ll.disq = 1 THEN 1
                                            ELSE 0 END sp, CASE l.preQf WHEN 0 THEN l.pos ELSE 0 END pos0
                                  FROM ONLlistdata l(NOLOCK)
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLboulder ll(NOLOCK) ON ll.iid_line = l.iid_line
                                 WHERE (EXISTS(SELECT * FROM ONLboulderRoutes br(NOLOCK) WHERE br.iid_parent = ll.idPK) OR ll.disq = 1 OR ll.nya = 1";
            if (inclPreQf)
                cmd.CommandText += "OR l.preQf = 1";
            cmd.CommandText += ") AND l.iid=" + listID.ToString();
            if (!inclPreQf)
                cmd.CommandText += " AND l.preQf = 0 ";
            cmd.CommandText += " ORDER BY l.pos";
            DataTable dt = new DataTable();
            DataTable dtRes = new DataTable();
            da.Fill(dt);
            cmd.CommandText = "SELECT ISNULL(routeNumber,1) rN FROM ONLlists(NOLOCK) WHERE iid=" +
                listID.ToString();
            int rN;
            try { rN = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { rN = -1; }
            if (rN < 1)
                return null;
            if (dt.Rows.Count > 0)
            {
                int roundCnt = 0;
                List<int> lsts = CreateListSeq();
                fRound = lsts.Count < 1;
                foreach (int i in lsts)
                {
                    RoundType rt;
                    ListCreator lcTmp = new ListCreator(cn, i, loadedLists, compID);
                    bool hasPreQf;
                    DataTable dtI = lcTmp.GetResultList(out rt, false, true, out hasPreQf);
                    this.loadedLists = lcTmp.loadedLists;
                    if (dtI == null || rt == RoundType.NOTHING)
                        continue;
                    roundCnt++;
                    DataColumn dc1I = null, dc2I = null, dcPq = null;
                    if (rt == RoundType.TWO_ROUTES || (hasPreQf && dtI.Columns.IndexOf("NpreQf") > -1))
                    {
                        dt.Columns.Add("rNum" + roundCnt.ToString());
                        if (rt == RoundType.TWO_ROUTES)
                        {
                            foreach (DataColumn dcI in dtI.Columns)
                                if (dcI.ColumnName.ToLower().IndexOf("��.") > -1)
                                {
                                    if (dcI.ColumnName.IndexOf("�") > -1)
                                        dc1I = dcI;
                                    else
                                        dc2I = dcI;
                                }
                            if (dc1I == null || dc2I == null)
                                continue;
                        }
                        dcPq = dtI.Columns["NpreQf"];
                    }
                    dt.Columns.Add("pos" + roundCnt.ToString());
                    foreach (DataRow dr in dt.Rows)
                        foreach (DataRow drI in dtI.Rows)
                            if (dr["iid"].ToString() == drI["iid"].ToString())
                            {
                                if (dcPq != null)
                                {
                                    if (Convert.ToInt32(drI[dcPq]) < 1)
                                        dr[dt.Columns.Count - 2] = 3;
                                    else
                                        dr[dt.Columns.Count - 2] = 1;
                                }
                                if (rt == RoundType.TWO_ROUTES)
                                {
                                    if (dcPq != null && Convert.ToInt32(drI[dcPq]) < 1)
                                        dr[dt.Columns.Count - 2] = 3;
                                    else if (drI[dc1I] != DBNull.Value && drI[dc1I] != null)
                                        dr[dt.Columns.Count - 2] = 1;
                                    else
                                        dr[dt.Columns.Count - 2] = 2;
                                }
                                dr[dt.Columns.Count - 1] = Convert.ToInt32(drI["pos"]);
                                break;
                            }
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT ISNULL(quote,0) quote FROM ONLlists(NOLOCK) WHERE iid=" + listID.ToString();
                int quote = Convert.ToInt32(cmd.ExecuteScalar());
                if (inclPreQf)
                    quote += checkPreQf(listID);

                SortingClass.SortResults(dt, quote, lsts.Count < 1, false, CompRules);
                if (inclPreQf)
                {
                    cmd.CommandText = "SELECT l.preQf FROM ONLlistdata l(NOLOCK) " +
                                      " WHERE l.climber_id = @c AND l.iid=" + listID.ToString();
                    cmd.Parameters.Add("@c", SqlDbType.Int);
                    dt.Columns.Add("preQf", typeof(bool));
                    foreach (DataRow dr in dt.Rows)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(dr["iid"]);
                        try { dr["preQf"] = Convert.ToBoolean(cmd.ExecuteScalar()); }
                        catch { dr["preQf"] = false; }
                    }
                    cmd.Parameters.Clear();
                    int pqC = 0;
                    foreach (DataRow dr in dt.Rows)
                        if ((bool)dr["preQf"])
                        {
                            pqC++;
                            dr["pts"] = 0.1;
                            dr["ptsText"] = "";
                        }
                        else
                        {
                            dr["pts"] = Convert.ToDouble(dr["pts"]) - (double)pqC;
                            double d;
                            if (double.TryParse(dr["ptsText"].ToString(), out d))
                                dr["ptsText"] = (d - (double)pqC).ToString("0.00");
                        }
                }


                cmd.CommandText = @"SELECT ll.idPK, '' �����, c.iid, cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������,";
                if (fullList)
                    for (int i = 1; i <= rN; i++)
                        cmd.CommandText += "NULL T" + i.ToString() + ",NULL B" + i.ToString() + ",";
                cmd.CommandText += "     ll.T [��.], ll.Ta [���.�� ��.], ll.B [���.], ll.Ba [���.�� ���.],";
                cmd.CommandText += @"   '' [��.], 0 pos, 0.0 pts,
                                       l.start [��.�], c.vk, ll.disq, ll.nya,CASE l.preQf WHEN 0 THEN 1 ELSE 0 END NpreQf
                                  FROM ONLboulder ll(NOLOCK)
                                  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id
                                 WHERE (EXISTS(SELECT * FROM ONLboulderRoutes br(NOLOCK) WHERE br.iid_parent = ll.idPK) OR ll.disq = 1 OR ll.nya = 1";
                if (inclPreQf)
                    cmd.CommandText += "OR l.preQf = 1";
                cmd.CommandText += ") AND l.iid=" + listID.ToString();
                if (!inclPreQf)
                    cmd.CommandText += " AND l.preQf = 0 ";
                cmd.CommandText += " ORDER BY ll.T DESC, ll.Ta, ll.B DESC, ll.Ba, ll.nya, ll.disq, c.vk";
                da.SelectCommand = cmd;
                if (da.SelectCommand.Connection.State != ConnectionState.Open)
                    da.SelectCommand.Connection.Open();
                da.Fill(dtRes);
                if (fullList)
                {
                    SqlCommand cmdG = new SqlCommand(
                        @"SELECT topA, bonusA
                            FROM ONLboulderRoutes(NOLOCK)
                           WHERE iid_parent = @ip
                             AND routeN = @rN", da.SelectCommand.Connection);
                    cmdG.Parameters.Add("@ip", SqlDbType.BigInt);
                    cmdG.Parameters.Add("@rN", SqlDbType.Int);
                    foreach (DataRow drN in dtRes.Rows)
                    {
                        cmdG.Parameters[0].Value = drN["idPK"];
                        for (int cR = 1; cR <= rN; cR++)
                        {
                            cmdG.Parameters[1].Value = cR;
                            SqlDataReader drG = cmdG.ExecuteReader();
                            try
                            {
                                if (drG.Read())
                                {
                                    drN["T" + cR.ToString()] = drG["topA"];
                                    drN["B" + cR.ToString()] = drG["bonusA"];
                                }
                            }
                            catch { }
                            finally { drG.Close(); }
                        }
                    }
                }
                dtRes.Columns.Remove("idPK");
            }
            foreach (DataRow dtR in dtRes.Rows)
                foreach (DataRow dr in dt.Rows)
                    if (dr["iid"].ToString() == dtR["iid"].ToString())
                    {
                        dtR["�����"] = dr["posText"].ToString();
                        dtR["��."] = dr["qf"].ToString();
                        if (dr["pos"] != DBNull.Value && dr["pos"] != null)
                            dtR["pos"] = Convert.ToInt32(dr["pos"]);
                        else
                            dtR["pos"] = DBNull.Value;
                        if (dr["pts"] != DBNull.Value && dr["pts"] != null)
                            dtR["pts"] = Convert.ToDouble(dr["pts"]);
                        else
                            dtR["pts"] = DBNull.Value;
                        break;
                    }

            //SortingClass.SortByColumn(dtRes, 0, dtRes.Rows.Count, dtRes.Columns.IndexOf("pos"));
            if (fRound)
                foreach (DataRow dr in dtRes.Rows)
                    if (Convert.ToBoolean(dr["disq"]))
                        dr["�����"] = "�����.";
                    else if (Convert.ToBoolean(dr["nya"]))
                        dr["�����"] = "�/�";

            if (dtRes.Rows.Count > 0)
                SortingClass.SortTwoCases(dtRes, "pts", "vk");

            cmd.CommandText = @"SELECT '' �����, c.iid, cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������,";
            if (fullList)
                for (int i = 1; i <= rN; i++)
                    cmd.CommandText += "NULL T" + i.ToString() + ",NULL B" + i.ToString() + ",";
            cmd.CommandText += "     ll.T [��.], ll.Ta [���.�� ��.], ll.B [���.], ll.Ba [���.�� ���.],";
            cmd.CommandText += @"   '' [��.], 0 pos, 0.0 pts,
                                       l.start [��.�], c.vk, 0 disq, 0 nya, 1 NpreQf
                                  FROM ONLboulder ll(NOLOCK)
                                  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id
                                 WHERE NOT EXISTS(SELECT * FROM ONLboulderRoutes br(NOLOCK) WHERE br.iid_parent = ll.idPK) AND ll.nya = 0 AND ll.disq = 0 AND l.preQf = 0
                                   AND l.iid = " + listID.ToString() + " ORDER BY l.start";
            if (cn.State != ConnectionState.Open)
                cn.Open();
            da.SelectCommand = cmd;

            DataTable dtStart = new DataTable();
            da.Fill(dtStart);
            MergeResStart(ref dtRes, ref dtStart);
            return dtRes;
        }

        private static void MergeResStart(ref DataTable dtRes, ref DataTable dtStart)
        {
            object[] obj = new object[dtRes.Columns.Count];
            for (int i = 0; i < obj.Length; i++)
                obj[i] = DBNull.Value;
            if (dtRes.Rows.Count < 1)
            {
                dtRes = dtStart;
                dtRes.Rows.Add(obj);
                dtRes.Rows[dtRes.Rows.Count - 1]["�������, ���"] = "-��������� ��������-";

                dtRes.Columns.Add("ghh", typeof(int));
                for (int i = 0; i < dtRes.Rows.Count - 1; i++)
                    dtRes.Rows[i]["ghh"] = i;
                dtRes.Rows[dtRes.Rows.Count - 1]["ghh"] = -1;
                SortingClass.SortByColumn(dtRes, "ghh");
                dtRes.Columns.Remove("ghh");
                return;
            }
            if (dtStart.Rows.Count > 0)
            {
                try
                {
                    
                    dtRes.Rows.Add(obj);

                    int cI = -1;
                    for (int i = 0; i < dtRes.Columns.Count; i++)
                        if (dtRes.Columns[i].ColumnName.ToLower().IndexOf("�������") > -1)
                        {
                            cI = i;
                            break;
                        }
                    if (cI > -1)
                        dtRes.Rows[dtRes.Rows.Count - 1]["�������, ���"] = "-��������� ��������-";
                    foreach (DataRow dr in dtStart.Rows)
                        dtRes.Rows.Add(dr.ItemArray);
                }
                catch { }

            }
        }

        private enum roundT { QUALI, PAIRING, FINAL }

        private DataTable CreateSpeed(roundT round, bool fullRes, bool inclPreQf)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            if (inclPreQf && !fullRes && round == roundT.QUALI)
                round = roundT.QUALI;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            DataTable dtRes = new DataTable();
            string sWhere1 = " WHERE l.iid = " + listID.ToString() + " ";
            if (listType != ListTypeEnum.SpeedQualy2)
            {
                sWhere1 += " AND (ll.res IS NOT NULL " +
                           "     AND LTRIM(RTRIM(ll.res)) <> '' ";
                if (inclPreQf)
                    sWhere1 += " OR l.preQf = 1 ";
                sWhere1 += ") ";
            }
            cmd.CommandText = @"SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText,
                                           CASE ll.r1 WHEN '�/�' THEN 2 WHEN '�����.' THEN 1 ELSE 0 END sp, 
                                           CASE l.preQf WHEN 0 THEN l.pos ELSE 0 END pos0
                                      FROM ONLlistdata l(NOLOCK)
                                      JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                      JOIN ONLspeed ll(NOLOCK) ON ll.iid_line = l.iid_line" + sWhere1;
            if (round == roundT.FINAL)
            {
                cmd.CommandText += " ORDER BY (CASE WHEN l.start < 3 THEN 2 ELSE 1 END), l.res";
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["pos0"] = (i + 1);
            }
            else
            {
                cmd.CommandText += "ORDER BY l.pos";
                da.Fill(dt);
            }
            List<int> lsts = null;
            if (dt.Rows.Count > 0)
            {
                int roundCnt = 0;
                lsts = CreateListSeq();
                foreach (int i in lsts)
                {
                    RoundType rt;
                    ListCreator lcTmp = new ListCreator(cn, i, loadedLists, compID);
                    DataTable dtI = lcTmp.GetResultList(out rt, false, true);
                    this.loadedLists = lcTmp.loadedLists;
                    if (dtI == null || rt == RoundType.NOTHING)
                        continue;
                    roundCnt++;
                    dt.Columns.Add("pos" + roundCnt.ToString());
                    foreach (DataRow dr in dt.Rows)
                        foreach (DataRow drI in dtI.Rows)
                            if (dr["iid"].ToString() == drI["iid"].ToString())
                            {
                                dr[dt.Columns.Count - 1] = Convert.ToInt32(drI["pos"]);
                                break;
                            }
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT ISNULL(quote,0) quote FROM ONLlists(NOLOCK) WHERE iid=" + listID.ToString();
                int quote = Convert.ToInt32(cmd.ExecuteScalar());

                SortingClass.SortResults(dt, quote, lsts.Count < 1, false, CompRules);
                foreach (DataRow dr in dt.Rows)
                    if (Convert.ToBoolean(dr["vk"]) && dr["qf"].ToString().ToLower().IndexOf("q") > -1)
                        dr["qf"] = " q";

                if (round == roundT.QUALI || round == roundT.PAIRING)
                {
                    cmd.CommandText = "SELECT l.climber_id, ll.res " +
                                  "  FROM ONLspeed ll(NOLOCK) " +
                                  "  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line " +
                                  " WHERE l.iid = " + listID.ToString();
                    da.SelectCommand = cmd;
                    DataTable dtF = new DataTable();
                    da.Fill(dtF);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string str = "";
                        foreach (DataRow drInner in dtF.Rows)
                            if (drInner["climber_id"].ToString() == dr["iid"].ToString())
                            {
                                str = drInner["res"].ToString().ToLower();
                                break;
                            }
                        if ((str.IndexOf("����") > -1 || str.IndexOf("*") > -1) && str.IndexOf("(1)") < 0 && lsts != null && lsts.Count > 0)
                            dr["qf"] = "";
                    }
                }

                if (inclPreQf)
                {

                    int vkQ = 0;
                    int qQ = 0;
                    foreach (DataRow dr in dt.Rows)
                        if (dr["qf"].ToString() == "Q")
                            qQ++;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToBoolean(dr["vk"]) && dr["qf"].ToString() == " q")
                        {
                            vkQ++;
                            dr["pos0"] = qQ + vkQ;
                        }
                        else if (dr["qf"].ToString().Length < 1)
                            dr["pos0"] = Convert.ToInt32(dr["pos"]) + vkQ;
                        else
                            dr["pos0"] = Convert.ToInt32(dr["pos"]) - vkQ;
                    }
                    SortingClass.SortResults(dt, quote, lsts.Count < 1, false, CompRules);
                    foreach (DataRow dr in dt.Rows)
                        if (Convert.ToBoolean(dr["vk"]) && dr["qf"].ToString().Length > 0)
                            dr["qf"] = "";
                }


                cmd.CommandText = @"SELECT '' �����, c.iid, cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������,";
                if (fullRes)
                    cmd.CommandText += "ll.r1 [������ 1], ll.r2 [������ 2],";
                cmd.CommandText += " ll.res [�����], ";
                if (fullRes && listType == ListTypeEnum.SpeedQualy2)
                    cmd.CommandText += "ISNULL(resFirst.res,'') [����.1]," +
                        " CASE WHEN ISNULL(prevLD.res,0) = l.res THEN ISNULL(resFirst.res,'') " +
                        "      ELSE ll.res END [������], ";
                cmd.CommandText += @" ll.qf [��.], 0 pos, 0.0 pts,l.start [��.�], c.vk,
                                       CASE l.preQf WHEN 0 THEN 1 ELSE 0 END NpreQf
                                  FROM ONLspeed ll(NOLOCK)
                                  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id ";
                if (fullRes && listType == ListTypeEnum.SpeedQualy2)
                    cmd.CommandText += " JOIN ONLlists curL(NOLOCK) ON l.iid = curL.iid" +
                                  " LEFT JOIN ONLlists prevL(NOLOCK) ON prevL.iid = curL.prevRound" +
                                  " LEFT JOIN ONLlistdata prevLD(NOLOCK) ON prevLD.iid = prevL.iid" +
                                  "                                     AND prevLD.climber_id = l.climber_id" +
                                  " LEFT JOIN ONLspeed resFirst(NOLOCK) ON resFirst.iid_line = prevLD.iid_line ";
                cmd.CommandText += sWhere1 +
                    " ORDER BY l.pos";
                da.SelectCommand = cmd;
                if (da.SelectCommand.Connection.State != ConnectionState.Open)
                    da.SelectCommand.Connection.Open();
                da.Fill(dtRes);

            }
            foreach (DataRow dtR in dtRes.Rows)
                foreach (DataRow dr in dt.Rows)
                    if (dr["iid"].ToString() == dtR["iid"].ToString())
                    {
                        dtR["�����"] = dr["posText"].ToString();
                        switch (round)
                        {
                            case roundT.FINAL:
                                dtR["��."] = "";
                                break;
                            case roundT.QUALI:
                                dtR["��."] = dr["qf"].ToString();
                                break;
                        }
                        if (dr["pos"] != DBNull.Value && dr["pos"] != null)
                            dtR["pos"] = Convert.ToInt32(dr["pos"]);
                        else
                            dtR["pos"] = DBNull.Value;
                        if (dr["pts"] != DBNull.Value && dr["pts"] != null)
                            dtR["pts"] = Convert.ToDouble(dr["pts"]);
                        else
                            dtR["pts"] = DBNull.Value;
                        break;
                    }
            if (dtRes.Rows.Count > 0)
            {
                SortingClass.SortTwoCases(dtRes, "pos", "vk");
                if (lsts == null || lsts.Count == 0)
                    foreach (DataRow dr in dtRes.Rows)
                        if (dr["�����"].ToString().ToLower().IndexOf("����") > -1)
                            dr["�����"] = "";
            }
            
            //SortingClass.SortByColumn(dtRes, 0, dtRes.Rows.Count, dtRes.Columns.IndexOf("pos"));

            if (listType != ListTypeEnum.SpeedQualy2)
            {
                cmd.CommandText = @"SELECT '' �����, c.iid, cl.surname+' '+cl.name [�������, ���], cl.age [�.�.], 
                                       t.name �������, c.qf ������,";
                if (fullRes)
                    cmd.CommandText += "ll.r1 [������ 1], ll.r2 [������ 2],";
                cmd.CommandText += " ll.res [�����], ";
                if (fullRes && listType == ListTypeEnum.SpeedQualy2)
                    cmd.CommandText += "ISNULL(resFirst.res,'') [����.1]," +
                        " CASE WHEN ISNULL(prevLD.res,0) = l.res THEN ISNULL(resFirst.res,'') " +
                        "      ELSE ll.res END [������], ";
                cmd.CommandText += @"'' [��.], 0 pos, 0.0 pts,l.start [��.�], c.vk, 1 NpreQf
                                  FROM ONLspeed ll(NOLOCK)
                                  JOIN ONLlistdata l(NOLOCK) ON l.iid_line = ll.iid_line
                                  JOIN ONLClimberCompLink c(NOLOCK) ON c.iid = l.climber_id
                                  JOIN ONLClimbers       cl(NOLOCK) ON cl.iid = c.climber_id
                                  JOIN ONLteams t(NOLOCK) ON t.iid = c.team_id ";
                if (fullRes && listType == ListTypeEnum.SpeedQualy2)
                    cmd.CommandText += " JOIN ONLlists curL(NOLOCK) ON l.iid = curL.iid" +
                                  " LEFT JOIN ONLlists prevL(NOLOCK) ON prevL.iid = curL.iid_parent" +
                                  " LEFT JOIN ONLlistdata prevLD(NOLOCK) ON prevLD.iid = prevL.iid" +
                                  "                                     AND prevLD.climber_id = l.climber_id" +
                                  " LEFT JOIN ONLspeed resFirst(NOLOCK) ON resFirst.iid_line = prevLD.iid_line ";
                cmd.CommandText += @" WHERE (ll.res IS NULL OR LTRIM(RTRIM(ll.res)) = '')
                                   AND l.preQf = 0
                                   AND l.iid = " + listID.ToString() + " ORDER BY l.start";
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                da.SelectCommand = cmd;
                DataTable dtStart = new DataTable();
                da.Fill(dtStart);
                MergeResStart(ref dtRes, ref dtStart);
            }
            if (round == roundT.FINAL)
            {
                if (dtRes.Rows.Count == 5)
                {
                    foreach (DataRow dr in dtRes.Rows)
                    {
                        try
                        {
                            if (dr["�����"].ToString().Length < 1)
                                continue;
                            if (Convert.ToInt32(dr["pos"]) < 3 && Convert.ToInt32(dr["��.�"]) < 3)
                            {
                                int nTmp;
                                if (int.TryParse(dr["�����"].ToString(), out nTmp))
                                    dr["�����"] = (nTmp + 2).ToString();
                            }
                        }
                        catch { }
                    }
                }
            }
            return dtRes;
        }

        private List<int> CreateListSeq()
        {
            List<int> res = new List<int>();
            int curID = listID;
            cmd.CommandText = "SELECT prevRound FROM ONLlists(NOLOCK) WHERE iid=@id";
            cmd.Parameters.Add("@id", SqlDbType.Int);
            while (true)
            {
                cmd.Parameters[0].Value = curID;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                object obj = cmd.ExecuteScalar();
                if (obj == null || obj == DBNull.Value)
                    break;
                else
                {
                    curID = Convert.ToInt32(obj);
                    if (curID > 0)
                        res.Add(curID);
                    else
                        break;
                }
            }
            cmd.Parameters.Clear();
            return res;
        }

        

        private DataTable CreateGeneralResults()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText =
              @"SELECT l.iid 
                  FROM ONLlists l(NOLOCK)
                  JOIN ONLlists ll(NOLOCK) ON l.style = ll.style
                                          AND l.group_id = ll.group_id
                                          AND l.iid <> ll.iid
                                          AND l.comp_id = ll.comp_id
                 WHERE l.prevRound IS NULL
                   AND l.iid_parent IS NULL
                   AND ll.iid=" + listID.ToString();
            int fId;
            fId = getIntFromCmd();
            if (fId < 1)
                return null;
            RoundType rt;
            ListCreator lc = new ListCreator(cn, fId, loadedLists, compID);
            DataTable dtRes = lc.GetResultList(out rt, false, true);
            this.loadedLists = lc.loadedLists;
            if (rt == RoundType.NOTHING || dtRes == null)
                return null;
            RemoveEmptyRows(dtRes);
            string style;
            string round = getRoundName(fId, out style);
            if (round.Length == 0 || style.Length == 0)
                return null;
            if (style.ToLower() == "����������")
            {
                if (rt == RoundType.TWO_ROUTES)
                {
                    dtRes.Columns.Add("����.(��.�)", typeof(string));
                    dtRes.Columns.Add("����.(��.�)", typeof(string));
                    int cA = dtRes.Columns.IndexOf("��.(�)");
                    int cB = dtRes.Columns.IndexOf("��.(�)");
                    if(cA * cB <=0)
                        return null;
                    foreach (DataRow dr in dtRes.Rows)
                    {
                        string strTmp = dr["�����"].ToString().ToLower();
                        bool nDisq = (strTmp.IndexOf("�/�") > -1 || strTmp.IndexOf("�����") > -1);
                        DataColumn dc;
                        int dcI;
                        if (dr[cA] != null && dr[cA] != DBNull.Value)
                        {
                            dc = dtRes.Columns[dtRes.Columns.Count - 2];
                            dcI = cA;
                        }
                        else if (dr[cB] != null && dr[cB] != DBNull.Value)
                        {
                            dc = dtRes.Columns[dtRes.Columns.Count - 1];
                            dcI = cB;
                        }
                        else
                            continue;
                        if (nDisq)
                        {
                            dr[dc] = dr["�����"].ToString();
                            dr["�����"] = "";
                        }
                        else if (Convert.ToInt32(dr["nPreQf"]) < 1)
                            dr[dc] = "";
                        else
                            dr[dc] = dr[dcI].ToString() + "/" + dr[dcI + 1].ToString() + " " + dr[dcI + 2].ToString() + "/" + dr[dcI + 3].ToString();
                    }
                    List<DataColumn> toRemove = new List<DataColumn>();
                    for (int i = 0; i < 4; i++)
                    {
                        toRemove.Add(dtRes.Columns[cA + i]);
                        toRemove.Add(dtRes.Columns[cB + i]);
                    }
                    foreach (DataColumn dc in toRemove)
                        dtRes.Columns.Remove(dc);
                }
                else
                {
                    dtRes.Columns.Add(round, typeof(string));
                    int dcI = dtRes.Columns.IndexOf("��.");
                    if (dcI < 0)
                        return null;
                    foreach (DataRow dr in dtRes.Rows)
                    {
                        string strTmp = dr["�����"].ToString().ToLower();
                        bool bND = (strTmp.IndexOf("�/�") > -1 || strTmp.IndexOf("�����") > -1);
                        bool pQf;
                        try { pQf = (Convert.ToInt32(dr["NpreQf"]) < 1); }
                        catch { pQf = false; }
                        if (bND)
                        {
                            dr[round] = dr["�����"].ToString();
                            dr["�����"] = "";
                        }
                        else if (pQf)
                            dr[round] = "";
                        else if (dr[dcI] != DBNull.Value && dr[dcI] != null)
                            dr[round] = dr[dcI].ToString() + "/" + dr[dcI + 1].ToString() + " " + dr[dcI + 2].ToString() + "/" + dr[dcI + 3].ToString();
                        else
                            dr[round] = "";
                    }
                    for (int i = 0; i < 4; i++)
                        dtRes.Columns.RemoveAt(dcI);
                }
            }
            if (dtRes.Columns.IndexOf("�����") > -1)
                dtRes.Columns["�����"].ColumnName = "����.";
            if (dtRes.Columns.IndexOf("���-�") > -1)
                dtRes.Columns["���-�"].ColumnName = round;
            int curId = fId;
            while (true)
            {
                cmd.CommandText = "SELECT iid FROM ONLlists(NOLOCK) WHERE prevRound=" + curId.ToString();
                curId = getIntFromCmd();
                if (curId < 0)
                    break;
                lc = new ListCreator(cn, curId, loadedLists, compID);
                round = getRoundName(curId, out style);
                if (round.Length == 0 || style.Length == 0)
                    continue;
                DataTable dtInner = lc.GetResultList(out rt, false, true);
                this.loadedLists = lc.loadedLists;
                if (rt == RoundType.NOTHING || dtInner == null)
                    continue;
                int stRow = -1;
                for(int j=0;j<dtInner.Rows.Count;j++)
                    if (dtInner.Rows[j]["�������, ���"].ToString().ToLower().IndexOf("��������� ��������") > -1)
                    {
                        stRow = j;
                        break;
                    }
                if (stRow > -1)
                {
                    for (int i = stRow + 1; i < dtInner.Rows.Count; i++)
                        dtInner.Rows[i]["pos"] = i;
                    dtInner.Rows.RemoveAt(stRow);
                }
                dtRes.Columns.Add(round, typeof(string));
                if (style.ToLower() == "����������" && round.ToLower().IndexOf("��") < 0)
                {
                    //dtRes.Columns.Add(round, typeof(string));
                    DataColumn[] dc = new DataColumn[4];
                    foreach(DataColumn dcc in dtInner.Columns)
                        if (dcc.ColumnName.ToLower().IndexOf("��.") > -1)
                        {
                            if (dcc.ColumnName.ToLower().IndexOf("���") > -1)
                                dc[1] = dcc;
                            else
                                dc[0] = dcc;
                        }
                        else if (dcc.ColumnName.ToLower().IndexOf("���.") > -1)
                        {
                            if (dcc.ColumnName.ToLower().IndexOf("���") > -1)
                                dc[3] = dcc;
                            else
                                dc[2] = dcc;
                        }
                    foreach (DataRow dr in dtInner.Rows)
                        foreach (DataRow drO in dtRes.Rows)
                            if (dr["iid"].ToString() == drO["iid"].ToString())
                            {
                                drO["�����"] = dr["�����"].ToString();
                                drO["pos"] = Convert.ToInt32(dr["pos"]);
                                if (Convert.ToBoolean(dr["nya"]))
                                    drO[round] = "�/�";
                                else if (Convert.ToBoolean(dr["disq"]))
                                    drO[round] = "�����.";
                                else
                                    drO[round] = dr[dc[0]].ToString() + "/" + dr[dc[1]].ToString() + " " + dr[dc[2]].ToString() + "/" + dr[dc[3]].ToString();
                            }
                }
                else
                {
                    string rndColName;
                    if (style.ToLower() == "���������" || round.ToLower().IndexOf("��") > -1)
                        rndColName = "���-�";
                    else
                        rndColName = "�����";
                    foreach(DataRow dr in dtInner.Rows)
                        foreach(DataRow drO in dtRes.Rows)
                            if (dr["iid"].ToString() == drO["iid"].ToString())
                            {
                                drO["�����"] = dr["�����"].ToString();
                                drO["pos"] = Convert.ToInt32(dr["pos"]);
                                drO[round] = dr[rndColName].ToString();
                            }
                }
            }
            if (dtRes.Columns.IndexOf("��.") > -1)
                dtRes.Columns.Remove("��.");
            if (dtRes.Columns.IndexOf("����") > -1)
                dtRes.Columns.Remove("����");
            if (dtRes.Columns.IndexOf("��.�") > -1)
                dtRes.Columns.Remove("��.�");
            SortingClass.SortTwoCases(dtRes, "pos", "vk");
            return dtRes;
        }

        private static void RemoveEmptyRows(DataTable dtRes)
        {
            List<DataRow> toDelete = new List<DataRow>();
            foreach (DataRow dr in dtRes.Rows)
                if (dr["pos"] == null || dr["pos"] == DBNull.Value || (Convert.ToInt32(dr["pos"]) < 1))
                    toDelete.Add(dr);
            foreach (DataRow dr in toDelete)
                dtRes.Rows.Remove(dr);
        }

        private int getIntFromCmd()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            int fId;
            object obj = cmd.ExecuteScalar();
            if (obj != null && obj != DBNull.Value)
                try { fId = Convert.ToInt32(obj); }
                catch { fId = -1; }
            else
                fId = -1;
            return fId;
        }

        private string getRoundName(int id, out string style)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = "SELECT round, style FROM ONLlists WHERE iid = " + id.ToString();
            string rName;
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr.Read())
                {
                    if (dr["round"] == DBNull.Value || dr["round"] == null ||
                        dr["style"] == DBNull.Value || dr["style"] == null)
                    {
                        style = "";
                        return "";
                    }
                    rName = dr["round"].ToString();
                    style = dr["style"].ToString();
                }
                else
                {
                    style = "";
                    return "";
                }
            }
            finally { dr.Close(); }
            return GetShortRoundName(rName);
        }

        public static string GetShortRoundName(string longName)
        {
            string round = longName;
            if (round.IndexOf("�����") > -1)
                round = round.Replace("�����������", "���");
            if (round.IndexOf("�����") > -1)
                round = round.Replace("�����", "��");
            if (round.IndexOf("����") > -1)
                round = round.Replace("����", "��");
            if (round.IndexOf("����") > -1)
                round = round.Replace("����", "�");
            if (round.IndexOf("����") > -1)
                round = round.Replace("����", "�");
            round = round.Replace("�", "");
            return round;
        }

        private int checkPreQf(int id)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = String.Format("SELECT round FROM ONLlists(NOLOCK) WHERE iid={0}", id);
            string round;
            object obj = cmd.ExecuteScalar();
            if (obj == null || obj == DBNull.Value)
                return 0;
            else
                round = obj.ToString().ToLower();
            List<int> routes = new List<int>();
            cmd.CommandText = String.Format("SELECT iid FROM ONLlists(NOLOCK) WHERE iid_parent = {0}", id);
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                while (dr.Read())
                    if (dr["iid"] != DBNull.Value && dr["iid"] != null)
                        routes.Add(Convert.ToInt32(dr["iid"]));
            }
            finally { dr.Close(); }
            List<int> lst = new List<int>();
            foreach (int i in routes)
                lst.Add(checkPreQf(i));
            lst.Sort();
            if (round.IndexOf("�����") > -1 && round.IndexOf("2 ��") > -1)
            {
                if (lst.Count > 0)
                    return lst[lst.Count - 1];
                else
                    return 0;
            }
            else if (round.IndexOf("���") > -1 && (round.IndexOf("2 ��") > -1 || round.IndexOf("2 ��") > -1))
            {
                int r = 0;
                foreach (int i in lst)
                    r += i;
                return r;
            }
            else
            {
                cmd.CommandText = String.Format("SELECT COUNT(*) cnt FROM ONLlistdata WHERE preQf = 1 AND iid={0}", id);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Dispose()
        {
            if (this.cmd != null)
                this.cmd.Dispose();
        }
    }
}
