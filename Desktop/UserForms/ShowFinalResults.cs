// <copyright file="ShowFinalResults.cs">
// Copyright В© 2016 All Rights Reserved
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
// (Р­С‚РѕС‚ С„Р°Р№Р» вЂ” С‡Р°СЃС‚СЊ ClimbingCompetition.
// 
// ClimbingCompetition - СЃРІРѕР±РѕРґРЅР°СЏ РїСЂРѕРіСЂР°РјРјР°: РІС‹ РјРѕР¶РµС‚Рµ РїРµСЂРµСЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏС‚СЊ РµРµ Рё/РёР»Рё
// РёР·РјРµРЅСЏС‚СЊ РµРµ РЅР° СѓСЃР»РѕРІРёСЏС… РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU РІ С‚РѕРј РІРёРґРµ,
// РІ РєР°РєРѕРј РѕРЅР° Р±С‹Р»Р° РѕРїСѓР±Р»РёРєРѕРІР°РЅР° Р¤РѕРЅРґРѕРј СЃРІРѕР±РѕРґРЅРѕРіРѕ РїСЂРѕРіСЂР°РјРјРЅРѕРіРѕ РѕР±РµСЃРїРµС‡РµРЅРёСЏ;
// Р»РёР±Рѕ РІРµСЂСЃРёРё 3 Р»РёС†РµРЅР·РёРё, Р»РёР±Рѕ (РїРѕ РІР°С€РµРјСѓ РІС‹Р±РѕСЂСѓ) Р»СЋР±РѕР№ Р±РѕР»РµРµ РїРѕР·РґРЅРµР№
// РІРµСЂСЃРёРё.
// 
// ClimbingCompetition СЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏРµС‚СЃСЏ РІ РЅР°РґРµР¶РґРµ, С‡С‚Рѕ РѕРЅР° Р±СѓРґРµС‚ РїРѕР»РµР·РЅРѕР№,
// РЅРѕ Р‘Р•Р—Рћ Р’РЎРЇРљРРҐ Р“РђР РђРќРўРР™; РґР°Р¶Рµ Р±РµР· РЅРµСЏРІРЅРѕР№ РіР°СЂР°РЅС‚РёРё РўРћР’РђР РќРћР“Рћ Р’РР”Рђ
// РёР»Рё РџР РР“РћР”РќРћРЎРўР Р”Р›РЇ РћРџР Р•Р”Р•Р›Р•РќРќР«РҐ Р¦Р•Р›Р•Р™. РџРѕРґСЂРѕР±РЅРµРµ СЃРј. РІ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№
// РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU.
// 
// Р’С‹ РґРѕР»Р¶РЅС‹ Р±С‹Р»Рё РїРѕР»СѓС‡РёС‚СЊ РєРѕРїРёСЋ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU
// РІРјРµСЃС‚Рµ СЃ СЌС‚РѕР№ РїСЂРѕРіСЂР°РјРјРѕР№. Р•СЃР»Рё СЌС‚Рѕ РЅРµ С‚Р°Рє, СЃРј. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;
using System.Globalization;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для отбражения итогового протокола результатов
    /// </summary>
    public partial class ShowFinalResults : BaseForm
    {
        string style = "", gName = "";
        SqlDataAdapter da;
        DataTable dtRes = new DataTable();
        int listID, groupID, judgeID;
        RoundElem[] roundList;
        
        FirstRoundType fr;

        double[] pts = null;
        enum FirstRoundType { QualificationFlash, TwoRoutes, OneRoute }
        SqlDataReader rd;

        class RoundElem
        {
            public RoundElem()
            {
                lists = new KeyValuePair<int, bool>[0];
                timeRoutes = 0;
            }
            public char start;
            public char stop;
            public int iid;
            public string name;
            public int count;
            public int countPar;
            public ListTypeEnum listType;
            public bool hasLeadTime;
            public int timeRoutes;
            public KeyValuePair<int, bool>[] lists;
        }

        int yOld, yYoung;
        Boolean preQfClimb;

        public DataTable getTable
        { get { return dtRes; } }

        readonly bool recalcConstructor;
        public ShowFinalResults(string competitionTitle, int listID, SqlConnection baseCon)
            : base(baseCon, competitionTitle)
        {
#if !DEBUG
            try
            {
#endif
                //if (cn.State != ConnectionState.Open)
                //    cn.Open();

                style = "";
                preQfClimb = SettingsForm.GetPreQfClimb(this.cn, listID);
                AccountForm.AlterParticipantsTable(cn, null);
                //this.restrict = restrict;
                if (competitionTitle != "NOT_FOR_SHOW")
                {
                    InitializeComponent();
                    //rbPts.Checked = usePts;
                    //rbPlc.Checked = !usePts;
                    //cbRestrict.Checked = this.restrict;
                    dg.DataSource = dtRes;
                }

                this.listID = listID;
                
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = cn;

                //cmd.CommandText = "SELECT l.group_id, l.style, g.name, l.judge_id, l.restrictPoints FROM lists l(NOLOCK) JOIN Groups g(NOLOCK) ON l.group_id=g.iid WHERE l.iid=" +
                cmd.CommandText = "SELECT l.group_id, l.style, g.name, l.judge_id FROM lists l(NOLOCK) JOIN Groups g(NOLOCK) ON l.group_id=g.iid WHERE l.iid=" +
                    this.listID.ToString();
                rd = cmd.ExecuteReader();

                try
                {
                    if (rd.Read())
                    {
                        groupID = Convert.ToInt32(rd[0]);
                        style = rd[1].ToString();
                        gName = rd[2].ToString();
                        try { judgeID = Convert.ToInt32(rd[3]); }
                        catch { judgeID = 0; }
                        /*this.restrict = Convert.ToBoolean(rd["restrictPoints"]);
                        try
                        {
                            if (cbRestrict != null)
                                cbRestrict.Checked = this.restrict;
                        }
                        catch { }*/
                    }
                }
                finally { rd.Close(); }
                cmd.CommandText = "SELECT pos FROM generalResults(NOLOCK) WHERE 1=0";
                try { cmd.ExecuteScalar(); }
                catch {
                    cmd.CommandText = "ALTER TABLE generalResults ADD pos INT NULL";
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                }
                da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = cn;

                cmd.CommandText = "SELECT count(*) FROM generalResults WHERE list_id = " + listID.ToString();

                recalcConstructor = this.competitionTitle != "NOT_FOR_SHOW" && Convert.ToInt32(cmd.ExecuteScalar()) < 1;

                RefreshTable(recalcConstructor);
                //allowEvent = true;
#if !DEBUG
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
#endif
            this.Text = StaticClass.GetListName(listID, cn);
        }

        double[] loadPoints(int maxQ, SqlTransaction tran)
        {
            AccountForm.CheckPointsTable(cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT pts FROM Points WHERE pos > 0 ORDER BY pos";
            cmd.Transaction = tran;
            List<double> pointsList = new List<double>();
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                    if (rdr[0] != DBNull.Value)
                        pointsList.Add(Convert.ToDouble(rdr[0]));
            }
            finally { rdr.Close(); }
            cmd.CommandText = "SELECT pts FROM Points WHERE pos = 0";
            object oTmp = cmd.ExecuteScalar();
            if (oTmp != null && oTmp != DBNull.Value)
            {
                double dPoint = Convert.ToDouble(oTmp);
                if (dPoint > 0.0)
                    while (pointsList.Count < maxQ)
                        pointsList.Add(dPoint);
            }
            return pointsList.ToArray();
        }

        private void RefreshTable(Boolean recalculate)
        {
#if !DEBUG
            try
            {
#endif
            SqlTransaction tran = cn.BeginTransaction();
            bool transactionSuccess = false;
            try
            {
                dtRes.Clear();
                dtRes.Columns.Clear();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;


                RefreshData(tran);
                if (this.competitionTitle != "NOT_FOR_SHOW" && dg != null && dg.DataSource != dtRes)
                    dg.DataSource = dtRes;
                if (this.competitionTitle != "NOT_FOR_SHOW" && !recalculate)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM generalResults(nolock) WHERE list_id = " + listID.ToString();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                        recalculate = true;
                }
                if (recalculate && this.competitionTitle != "NOT_FOR_SHOW")
                {
                    cmd.CommandText = "DELETE FROM generalResults WHERE list_id = " + listID.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO generalResults (list_id,climber_id,pts, pos) VALUES (@l,@c,@p,@ps)";
                    cmd.Parameters.Add("@l", SqlDbType.Int);
                    cmd.Parameters[0].Value = this.listID;
                    cmd.Parameters.Add("@c", SqlDbType.Int);
                    cmd.Parameters.Add("@p", SqlDbType.Float);
                    cmd.Parameters.Add("@ps", SqlDbType.Int);
                    foreach (DataRow dr in dtRes.Rows)
                    {
                        int climber;
                        object ptts;
                        try
                        {
                            if (dr["Балл"] == null || dr["Балл"] == DBNull.Value)
                                ptts = 0.0;
                            else
                                ptts = Convert.ToDouble(dr["Балл"]);
                            climber = Convert.ToInt32(dr["№"]);
                        }
                        catch { continue; }
                        if (climber == 0)
                            continue;
                        cmd.Parameters[1].Value = climber;
                        cmd.Parameters[2].Value = ptts;
                        int posParamVal;
                        if (int.TryParse((dr["Место"] ?? DBNull.Value).ToString(), out posParamVal))
                            cmd.Parameters[3].Value = posParamVal;
                        else
                            cmd.Parameters[3].Value = DBNull.Value;
                        cmd.ExecuteNonQuery();
                    }

                }
                else
                {
                    cmd.CommandText = "SELECT climber_id, IsNull(pts,0.0) pts, IsNull(pos, -1) pos" +
                                      "  FROM generalResults(nolock)" +
                                      " WHERE list_id = @listID";
                    cmd.Parameters.Add("@listID", SqlDbType.Int).Value = listID;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int clm = Convert.ToInt32(rdr["climber_id"]);
                            foreach (DataRow dr in dtRes.Rows)
                                if (Convert.ToInt32(dr["№"]) == clm)
                                {
                                    dr["Балл"] = rdr["pts"];
                                    if (Convert.ToInt32(rdr["pos"]) > -1)
                                        dr["Место"] = Convert.ToInt32(rdr["pos"]);
                                }
                        }
                    }
                    int cnt = dtRes.Rows.Count;
                    int sorted = 0;
                    for (int i = sorted; i < cnt; i++)
                    {
                        if (dtRes.Rows[i]["Место"] == null || dtRes.Rows[i]["Место"] == DBNull.Value)
                            continue;
                        int minValue;
                        if (!int.TryParse(dtRes.Rows[i]["Место"].ToString(), out minValue))
                            minValue = 1;
                        int minIndex = i;
                        for (int j = i + 1; j < cnt; j++)
                        {
                            if (dtRes.Rows[j]["Место"] == null || dtRes.Rows[j]["Место"] == DBNull.Value)
                                continue;
                            int curPos;
                            if (int.TryParse(dtRes.Rows[j]["Место"].ToString(), out curPos) && curPos < minValue)
                            {
                                minIndex = j;
                                minValue = Convert.ToInt32(dtRes.Rows[j]["Место"]);
                            }
                        }
                        if (minIndex > i)
                        {
                            var tmp = dtRes.Rows[i].ItemArray;
                            dtRes.Rows[i].ItemArray = dtRes.Rows[minIndex].ItemArray;
                            dtRes.Rows[minIndex].ItemArray = tmp;
                        }
                        sorted++;
                    }

                }
                if (this.competitionTitle != "NOT_FOR_SHOW")
                {
                    try
                    {
                        dg.ReadOnly = false;
                        foreach (DataGridViewColumn dc in dg.Columns)
                        {
                            if (dc.Name.Equals("место", StringComparison.OrdinalIgnoreCase) || dc.Name.Equals("балл", StringComparison.OrdinalIgnoreCase))
                                dc.ReadOnly = false;
                            else
                                dc.ReadOnly = true;
                        }
                    }
                    catch { }
                }
                transactionSuccess = true;
            }
            finally
            {
                if (transactionSuccess)
                    tran.Commit();
                else
                    try { tran.Rollback(); }
                    catch { }
            }
#if !DEBUG
            }
            catch (Exception ex) { MessageBox.Show("Ошибка создания итогового протокола:\r\n" + ex.Message); }
#endif
        }

        List<int> speedFalls = new List<int>();
        void MakeResultList(FirstRoundType fr, SqlTransaction tran)
        {
            speedFalls.Clear();
            String sPidarasyTable;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = this.cn;
                cmd.Transaction = tran;
                cmd.CommandText = @"
  set @sTableName = '##PIDARASY_' + convert(varchar(30), @listID) + '_' + convert(varchar(20), @@spid)
  if exists(select 1
              from tempdb..sysobjects(nolock)
             where name = @sTableName)
    exec('begin tran drop table ' + @sTableName + ' commit tran')
  exec('begin tran ' +
       '  create table ' + @sTableName + ' (' +
       '    iid int ' +
       '  ) ' +
       'commit tran')";
                var p = cmd.Parameters.Add("@sTableName", SqlDbType.VarChar, 255);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@listID", SqlDbType.Int).Value = listID;
                cmd.ExecuteNonQuery();
                sPidarasyTable = (String)p.Value;
            }
            dtRes.Clear();
            dtRes.Columns.Clear();
            if (cn.State != ConnectionState.Open)
                cn.Open();
            DataTable dtTmp = new DataTable();
            switch (fr)
            {//abcdefghij
                case FirstRoundType.QualificationFlash:
                    int routeNumber, timeRoutes;
                    KeyValuePair<int, bool>[] listsLL;
                    dtRes = StaticClass.FillResFlash(roundList[0].iid, cn, true, tran, out routeNumber, true, out timeRoutes, out listsLL);
                    if (dtRes.Columns.Contains("Кв."))
                        dtRes.Columns.Remove("Кв.");
                    foreach (DataRow drr in dtRes.Rows)
                    {
                        if (drr["Тр.1"].ToString() == "")
                            drr["Рез-т"] = DBNull.Value;
                    }
                    roundList[0].stop = (char)('F' + (2 * routeNumber) + timeRoutes);
                    roundList[0].timeRoutes = timeRoutes;
                    roundList[0].lists = listsLL;
                    break;
                case FirstRoundType.TwoRoutes:
                    dtRes = StaticClass.FillResOnsight(roundList[0].iid, cn, true, tran);
                    dtRes.Columns.Remove("Кв.");
                    //dtRes.Columns["Место"].ColumnName = "posText";
                    dtRes.Columns.Remove("pos");
                    dtRes.Columns["posT"].ColumnName = "pos";
                    if (style == "Трудность")
                        roundList[0].stop = 'G';
                    else
                        roundList[0].stop = 'M';
                    break;
                case FirstRoundType.OneRoute:
                    switch (style)
                    {
                        case "Боулдеринг":
                            da.SelectCommand.CommandText = "SELECT CASE br.preQf WHEN 0 THEN br.pos ELSE 0 END pos," +
                                "br.posText AS Место,br.climber_id AS [№]," +
                                "(p.surname+' '+p.name) AS [Фамилия, Имя],p.age AS [Г.р.],p.qf AS Разряд,dbo.fn_getTeamName(p.iid, br.list_id) AS Команда,br.disq, br.nya, br.tops AS T,br.topAttempts AS Ta," +
                                "br.bonuses AS B,br.bonusAttempts AS Ba FROM boulderResults br(NOLOCK) INNER JOIN Participants p(NOLOCK) ON " +
                                "br.climber_id=p.iid INNER JOIN Teams t(NOLOCK) ON p.team_id=t.iid WHERE br.list_id=" +
                                roundList[0].iid.ToString() + " ORDER BY pos, br.res DESC";
                            roundList[0].stop = 'I';
                            break;
                        case "Трудность":
                            da.SelectCommand.CommandText = "SELECT CASE br.preQf WHEN 0 THEN br.pos ELSE 0 END pos," +
                                "br.posText AS Место,br.climber_id AS [№]," +
                                "(p.surname+' '+p.name) AS [Фамилия, Имя],p.age AS [Г.р.],p.qf AS Разряд,"+
                                "dbo.fn_getTeamName(p.iid, br.list_id) AS Команда,br.resText, ISNULL(br.timeText,'') timeText " +
                                "FROM routeResults br(NOLOCK) INNER JOIN Participants p(NOLOCK) ON " +
                                "br.climber_id=p.iid INNER JOIN Teams t(NOLOCK) ON p.team_id=t.iid WHERE br.list_id=" +
                                roundList[0].iid.ToString() + " ORDER BY pos, br.res DESC";
                            roundList[0].stop = (roundList[0].hasLeadTime ? 'G' : 'F');
                            break;
                        case "Скорость":
                            int minPos, offset;
                            if (preQfClimb && roundList.Length > 1)
                            {
                                using (var cmd = new SqlCommand())
                                {
                                    cmd.Connection = cn;
                                    cmd.Transaction = tran;
                                    SqlParameter pListID, pNextID;
                                    cmd.CommandText = String.Format("INSERT INTO {0} (iid) " +
                                                      "SELECT DISTINCT SR.climber_id" +
                                                      "  FROM speedResults SR(nolock)" +
                                                      "  JOIN speedResults SN(nolock) on SN.climber_id = SR.climber_id" +
                                                      "                              and SN.list_id = @nextID" +
                                                      " WHERE SR.list_id = @listID" +
                                                      "   AND SR.preQf = 1" +
                                                      "   AND (SR.resText like '%дискв%' OR SR.resText like '%срыв%' OR SR.resText like '%н/я%')" +
                                                      "   AND (SN.resText like '%дискв%' OR SN.resText like '%срыв%' OR SN.resText like '%н/я%')"
                                                      , sPidarasyTable);
                                    (pListID = cmd.Parameters.Add("@listID", SqlDbType.Int)).Value = roundList[0].iid;
                                    (pNextID = cmd.Parameters.Add("@nextID", SqlDbType.Int)).Value = roundList[1].iid;

                                    int pidarasyCount = cmd.ExecuteNonQuery();

                                    cmd.CommandText = "SELECT COUNT(*)" +
                                                      "  FROM speedResults SR(nolock)" +
                                                      " WHERE SR.list_id = @nextID";
                                    
                                    
                                    int nextRoundClimbers = Convert.ToInt32(cmd.ExecuteScalar()) - pidarasyCount;

                                    cmd.CommandText = "SELECT min(SR.pos)" +
                                                      "  FROM speedResults SR(nolock)" +
                                                  " LEFT JOIN speedResults SN(nolock) on SN.climber_id = SR.climber_id AND SN.list_id = @nextID" +
                                                  "     WHERE SR.list_id = @listID" +
                                                  "       AND SN.climber_id IS NULL";
                                    

                                    object val = cmd.ExecuteScalar();
                                    minPos = (val == null) ? -1 : Convert.ToInt32(val);

                                    if (minPos > 0 && minPos <= nextRoundClimbers)
                                        offset = nextRoundClimbers + 1 - minPos;
                                    else
                                        offset = 0;
                                }
                            }
                            else
                                minPos = offset = 0;

                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = cn;
                                cmd.Transaction = tran;
                                StringBuilder sb = new StringBuilder("SELECT distinct SR.climber_id" +
                                                                     "  FROM speedResults SR(nolock)" +
                                                                 " LEFT JOIN ").Append(sPidarasyTable).Append(" PDR(nolock) on PDR.iid = SR.climber_id");
                                if (roundList.Length > 1)
                                    sb.Append(" LEFT JOIN speedResults SN(nolock) on SN.climber_id = SR.climber_id AND SN.list_id = ").Append(roundList[1].iid);
                                sb.Append(" WHERE SR.list_id = ").Append(roundList[0].iid)
                                  .Append(" AND (SR.resText like '%дискв%' OR SR.resText like '%срыв%' OR SR.resText like '%н/я%')")
                                  .Append(" AND (PDR.iid is not null ");
                                if (roundList.Length > 1)
                                    sb.Append(" OR SN.climber_id is null ");
                                sb.Append(")");
                                cmd.CommandText = sb.ToString();
                                using (var rdr = cmd.ExecuteReader())
                                {
                                    while (rdr.Read())
                                        speedFalls.Add(Convert.ToInt32(rdr[0]));
                                }
                            }

                            da.SelectCommand.CommandText = String.Format(
                                "SELECT CASE WHEN br.preQf = 0 OR PDR.iid is not null THEN CASE WHEN br.pos < {0} or br.resText like '%срыв%' THEN br.pos ELSE br.pos + {1} END ELSE 0 END pos," +
                                      " CASE WHEN br.posText <> '' AND br.resText not like '%срыв%' AND IsNumeric(br.posText) = 1 THEN " +
                                      "        case when br.pos >= {0} then convert(varchar(max), convert(int, br.posText) + {1}) " +
                                      "             else br.posText " +
                                      "        end " +
                                      "      ELSE br.posText " +
                                      " END AS Место, " +
                                      " br.climber_id AS [№]," +
                                      " (p.surname+' '+p.name) AS [Фамилия, Имя],p.age AS [Г.р.],p.qf AS Разряд,dbo.fn_getTeamName(p.iid, br.list_id) AS Команда," +
                                      " br.resText " +
                                 " FROM speedResults br(NOLOCK)" +
                                 " JOIN Participants p(NOLOCK) ON br.climber_id=p.iid" +
                                 " JOIN Teams t(NOLOCK) ON p.team_id=t.iid" +
                            " LEFT JOIN {3} PDR(nolock) on PDR.iid = P.iid" +
                                " WHERE br.list_id = {2}" +
                             " ORDER BY pos", minPos, offset, roundList[0].iid, sPidarasyTable);
                            roundList[0].stop = 'F';
                            break;
                    }
                    da.SelectCommand.Transaction = tran;
                    dtRes.Clear();
                    da.Fill(dtRes);
                    if (!roundList[0].hasLeadTime && dtRes.Columns.Contains("timeText"))
                        dtRes.Columns.Remove("timeText");
                    int nullCnt = 0;
                    if(!preQfClimb)
                        foreach (DataRow drr in dtRes.Rows)
                        {
                            if (Convert.ToInt32(drr["pos"]) == 0)
                            {
                                nullCnt++;
                                drr["pos"] = nullCnt;
                                drr["Место"] = nullCnt.ToString();
                            }
                            else
                            {
                                int iTmp;
                                if (nullCnt > 0)
                                {
                                    drr["pos"] = Convert.ToInt32(drr["pos"]) + nullCnt;
                                    if (int.TryParse(drr["Место"].ToString(), out iTmp))
                                        drr["Место"] = (iTmp + nullCnt).ToString();
                                }
                            }
                        }
                    break;
            }
            roundList[0].count = dtRes.Rows.Count;
            pts = loadPoints(dtRes.Rows.Count, tran);
            if (bParallel)
                foreach (DataRow drrt in dtRes.Rows)
                    try
                    {
                        int yB = Convert.ToInt32(drrt["Г.р."]);
                        if (yB >= yOld && yB <= yYoung)
                            roundList[0].countPar++;
                    }
                    catch { }
            //try { dg.DataSource = dtRes; }
            //catch { }
            for (int i = 1; i < roundList.Length; i++)
            {
                DataTable dtPidorasy = null;
                int roundRouteNumber = -1;
                int roundTimeColumns = 0;
                char cTmp;
                bool hasResBest = false;
                switch (style)
                {
                    case "Боулдеринг":
                        if (roundList[i].name == "Суперфинал")
                            goto case "Трудность";
                        da.SelectCommand.CommandText = "SELECT CASE preQf WHEN 0 THEN pos ELSE 0 END pos, " +
                            "CASE preQf WHEN 0 THEN posText ELSE '0' END posText,climber_id AS [№],tops,topAttempts,bonuses," +
                            "bonusAttempts, disq, nya FROM boulderResults(NOLOCK) WHERE list_id=" + roundList[i].iid.ToString() + " ORDER BY " +
                            "pos, res DESC";

                        dtRes.Columns.Add("disq" + i.ToString(), typeof(bool));
                        dtRes.Columns.Add("nya" + i.ToString(), typeof(bool));
                        dtRes.Columns.Add("T" + i.ToString(), typeof(int));
                        dtRes.Columns.Add("Ta" + i.ToString(), typeof(int));
                        dtRes.Columns.Add("B" + i.ToString(), typeof(int));
                        dtRes.Columns.Add("Ba" + i.ToString(), typeof(int));

                        cTmp = roundList[i - 1].stop;
                        roundList[i].start = ++cTmp;
                        for (int ii = 0; ii < 3; ii++)
                            cTmp++;
                        roundList[i].stop = cTmp;
                        break;
                    case "Трудность":
                        cTmp = roundList[i - 1].stop;
                        roundList[i].start = ++cTmp;
                        if (roundList[i].listType == ListTypeEnum.LeadFlash)
                        {
                            KeyValuePair<int, bool>[] lists;
                            dtTmp = StaticClass.FillResFlash(roundList[i].iid, cn, true, tran, out roundRouteNumber, true, out roundTimeColumns,out lists);
                            roundList[i].timeRoutes = roundRouteNumber;
                            roundList[i].lists = lists;
                            string rndN = "res" + i.ToString();
                            for (int ij = 1; ij <= roundRouteNumber; ij++)
                            {
                                cTmp += (char)2;
                                dtRes.Columns.Add(rndN + "R" + ij.ToString(), typeof(string));
                                if (ij <= lists.Length)
                                    if (lists[ij - 1].Value)
                                    {
                                        cTmp++;
                                        dtRes.Columns.Add(rndN + "T" + ij.ToString(), typeof(string));
                                    }
                                dtRes.Columns.Add(rndN + "P" + ij.ToString(), typeof(string));
                            }
                            dtRes.Columns.Add(rndN, typeof(string));
                            roundList[i].stop = cTmp;
                            if (dtTmp.Columns.IndexOf("Место") > -1 && dtTmp.Columns.IndexOf("posText") < 0)
                                dtTmp.Columns["Место"].ColumnName = "posText";
                            //if (dtTmp.Columns.IndexOf("№") > -1 && dtTmp.Columns.IndexOf("iid") < 0)
                            //    dtTmp.Columns["№"].ColumnName = "iid";
                        }
                        else
                        {
                            da.SelectCommand.CommandText = "SELECT CASE preQf WHEN 0 THEN pos ELSE 0 END pos," +
                                "CASE preQf WHEN 0 THEN posText ELSE '0' END posText,climber_id AS [№],resText, " +
                                "     ISNULL(timeText,'') timeText " +
                                "FROM routeResults(NOLOCK) WHERE list_id=" + roundList[i].iid.ToString() + " ORDER BY " +
                                "pos, res DESC";
                            dtRes.Columns.Add("res" + i.ToString(), typeof(string));
                            if (roundList[i].hasLeadTime)
                            {
                                dtRes.Columns.Add("time" + i.ToString(), typeof(string));
                                roundList[i].stop = (char)(roundList[i].start + 1);
                            }
                            else
                                roundList[i].stop = roundList[i].start;
                        }
                        
                        
                        break;
                    case "Скорость":
                        string sAdditionalSelect;
                        SpeedRules rules = SettingsForm.GetSpeedRules(cn, tran);
                        bool useBestQf = (rules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds;
                        if (roundList[i].name == "Квалификация 2" && useBestQf)
                        {
                            hasResBest = true;
                            sAdditionalSelect = ", dbo.fn_getBest(SR.iid) AS resBest ";
                        }
                        else
                            sAdditionalSelect = String.Empty;
                        da.SelectCommand.CommandText =
                            "SELECT CASE WHEN SP.climber_id is not null then SP.pos "+
                            "            WHEN SR.preQf = 0 THEN SR.pos " +
                            "            ELSE 0 " +
                            "       END pos, " +
                            "       CASE WHEN SP.climber_id is not null then SP.posText " +
                            "            WHEN SR.preQf = 0 THEN SR.posText " +
                            "            ELSE '0' " +
                            "       END posText, " +
                            //"       CASE SR.preQf WHEN 0 THEN SR.pos ELSE 0 END pos," +
                            //"       CASE SR.preQf WHEN 0 THEN SR.posText ELSE '0' END posText," +
                            "       SR.climber_id AS [№], SR.resText " + sAdditionalSelect +
                            "  FROM speedResults SR(NOLOCK)" +
                        " LEFT JOIN " + sPidarasyTable + " PDR(nolock) on PDR.iid = SR.climber_id" +
                        " LEFT JOIN speedResults SP(nolock) on SP.climber_id = PDR.iid and SP.list_id = " + roundList[i - 1].iid.ToString() +
                            " WHERE SR.list_id=" + roundList[i].iid.ToString() +
                            "   AND PDR.iid is null" +
                         " ORDER BY pos";
                        SqlDataAdapter daPidorasy = new SqlDataAdapter();
                        daPidorasy.SelectCommand = new SqlCommand();
                        daPidorasy.SelectCommand.Connection = cn;
                        daPidorasy.SelectCommand.Transaction = tran;

                        daPidorasy.SelectCommand.CommandText = String.Format(@"
SELECT SR.climber_id, SR.resText {0}
  FROM speedResults SR(nolock)
  JOIN {1} PDR(nolock) on PDR.iid = SR.climber_id
 WHERE SR.list_id = {2}
", sAdditionalSelect, sPidarasyTable, roundList[i].iid);
                        dtPidorasy = new DataTable();
                        daPidorasy.Fill(dtPidorasy);

                        dtRes.Columns.Add("res" + i.ToString(), typeof(string));
                        cTmp = roundList[i - 1].stop;
                        roundList[i].stop = roundList[i].start = ++cTmp;
                        if (hasResBest)
                        {
                            roundList[i].stop++;
                            dtRes.Columns.Add("resBest", typeof(string));
                        }
                        break;
                }
                da.SelectCommand.Transaction = tran;
                if (roundList[i].listType != ListTypeEnum.LeadFlash)
                {
                    dtTmp.Clear();
                    da.Fill(dtTmp);
                    int nullCnt = 0;
                    foreach (DataRow drr in dtTmp.Rows)
                    {
                        if (Convert.ToInt32(drr["pos"]) == 0)
                            nullCnt++;
                        if (nullCnt > 0)
                        {
                            drr["pos"] = Convert.ToInt32(drr["pos"]) + nullCnt;
                            int iTmp;
                            if (int.TryParse(drr["posText"].ToString(), out iTmp))
                                drr["posText"] = (iTmp + nullCnt).ToString();
                        }
                    }
                }
                if (bParallel)
                {
                    SqlCommand cmdp = new SqlCommand("SELECT age FROM Participants(NOLOCK) WHERE iid = @id", cn);
                    cmdp.Parameters.Add("@id", SqlDbType.Int);
                    cmdp.Transaction = tran;
                    foreach (DataRow drrt in dtTmp.Rows)
                        try
                        {
                            cmdp.Parameters[0].Value = Convert.ToInt32(drrt["№"]);
                            int age = Convert.ToInt32(cmdp.ExecuteScalar());
                            if (age >= yOld && age <= yYoung)
                                roundList[i].countPar++;
                        }
                        catch { }
                }

                roundList[i].count = dtTmp.Rows.Count;
                for (int j = 0; j < dtTmp.Rows.Count; j++)
                {
                    DataRow dr = dtTmp.Rows[j];
                    //bool b = true;
                    for (int k = 0; k < dtRes.Rows.Count; k++)
                    {
                        DataRow drInner = dtRes.Rows[k];
                        if (drInner["№"].ToString() == dr["№"].ToString())
                        {
                            drInner["Место"] = dr["posText"];
                            drInner["pos"] = dr["pos"];
                            switch (style)
                            {
                                case "Боулдеринг":
                                    if (roundList[i].name == "Суперфинал")
                                        goto case "Трудность";
                                    drInner["T" + i.ToString()] = dr["tops"];
                                    drInner["Ta" + i.ToString()] = dr["topAttempts"];
                                    drInner["B" + i.ToString()] = dr["bonuses"];
                                    drInner["Ba" + i.ToString()] = dr["bonusAttempts"];
                                    drInner["disq" + i.ToString()] = dr["disq"];
                                    drInner["nya" + i.ToString()] = dr["nya"];
                                    break;
                                case "Трудность":
                                    if (roundList[i].listType == ListTypeEnum.LeadFlash)
                                    {
                                        int resColInd = dtTmp.Columns.IndexOf("Рез-т");
                                        if (resColInd > -1)
                                            for (int iC = 0; iC <= roundRouteNumber * 2 + roundTimeColumns; iC++)
                                            {
                                                try { drInner[dtRes.Columns.Count - 1 - iC] = dr[resColInd - iC]; }
                                                catch { }
                                            }
                                    }
                                    else
                                    {
                                        drInner["res" + i.ToString()] = dr["resText"];
                                        if (hasResBest)
                                            drInner["resBest"] = dr["resBest"];
                                        if (roundList[i].hasLeadTime)
                                            drInner["time" + i.ToString()] = dr["timeText"];
                                    }
                                    break;
                                case "Скорость":
                                    goto case "Трудность";
                            }
                            if (k != j)
                            {
                                object[] tmpRow = dtRes.Rows[k].ItemArray;
                                dtRes.Rows[k].ItemArray = dtRes.Rows[j].ItemArray;
                                dtRes.Rows[j].ItemArray = tmpRow;
                            }
                            //b = false;
                            break;
                        }
                    }
                    //if (b)
                    //{
                    //    AddRowToPos(j, dtRes);
                    //    object[] row = dtRes.Rows[j].ItemArray;
                    //    SqlCommand cmd = new SqlCommand(
                    //        "SELECT (p.surname+' '+p.name), p.age, p.qf, t.name FROM " +
                    //        "Participants p INNER JOIN Teams t ON p.team_id=g.iid WHERE " +
                    //        "p.iid = " + dr["№"].ToString(), cn);
                    //}
                }
                if (dtPidorasy != null)
                {
                    int lastRowP = -1, currentRow = 0;
                    foreach(DataRow drRes in dtRes.Rows)
                    {
                        currentRow++;
                        foreach (DataRow drPidoras in dtPidorasy.Rows)
                        {
                            try
                            {
                                if (Convert.ToInt32(drRes["№"]) == Convert.ToInt32(drPidoras["climber_id"]))
                                {
                                    lastRowP = currentRow;
                                    drRes["res" + i.ToString()] = drPidoras["resText"];
                                    if (dtPidorasy.Columns.Contains("resBest") && dtRes.Columns.Contains("resBest"))
                                        drRes["resBest"] = drPidoras["resBest"];
                                }
                            }
                            catch { }
                        }
                    }
                    if (lastRowP > roundList[i].count)
                        roundList[i].count = lastRowP;
                }
            }// на след. раунд
            //dtRes.Columns.Remove("pos");
        }

        void AddRowToPos(int pos, DataTable dt)
        {
            int k = dt.Rows.Count - 1;
            object[] tmp = dt.Rows[k--].ItemArray;
            dt.Rows.Add(tmp);
            for (; k >= pos; k--)
            {
                tmp = dt.Rows[k].ItemArray;
                dt.Rows[k + 1].ItemArray = tmp;
            }
        }

        int cntClm = -1;

        void SetPts(SqlTransaction tran)
        {
            int cnt, i, head, tail, lastWithPoints;
            double avgPts = 0.0;

            bool vk = false, np = false;
            if (dtRes.Columns[0].ColumnName == "pos")
                head = 7;
            else
                head = 6;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT vk, noPoints FROM Participants(NOLOCK) WHERE iid = @iid";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            noPoits = new List<int>();
            vKk = new List<int>();
            try { dtRes.Columns.Add("Балл", typeof(double)); }
            catch { }
            foreach (DataRow qwer in dtRes.Rows)
                qwer["Балл"] = 0.0;
            
            #region Sorting NoPoints
            dtRes.Columns.Add("clmType", typeof(int));
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT style FROM lists(nolock) WHERE iid = " + listID.ToString();
            string col, res = cmd.ExecuteScalar() as string;
            if (String.IsNullOrEmpty(res))
                col = " P.noPoints ";
            else if (res.Equals("Трудность", StringComparison.InvariantCultureIgnoreCase))
                col = " CONVERT(BIT, CASE P.lead WHEN 2 THEN 1 ELSE 0 END) noPoints ";
            else if (res.Equals("Скорость", StringComparison.InvariantCultureIgnoreCase))
                col = " CONVERT(BIT, CASE P.speed WHEN 2 THEN 1 ELSE 0 END) noPoints ";
            else if (res.Equals("Боулдеринг", StringComparison.InvariantCultureIgnoreCase))
                col = " CONVERT(BIT, CASE P.boulder WHEN 2 THEN 1 ELSE 0 END) noPoints ";
            else
                col = " P.noPoints ";

            cmd.CommandText = "SELECT P.vk," + col +
                              "  FROM participants P(NOLOCK)" +
                              " WHERE P.iid = @id";
            cmd.Parameters.Add("@id", SqlDbType.Int);
            foreach (DataRow qwer in dtRes.Rows)
                try
                {
                    cmd.Parameters[0].Value = Convert.ToInt32(qwer["№"]);
                    SqlDataReader qRd = cmd.ExecuteReader();
                    try
                    {
                        if (qRd.Read())
                        {
                            if (Convert.ToBoolean(qRd["vk"]))
                                qwer["clmType"] = 2;
                            else if (Convert.ToBoolean(qRd["noPoints"]))
                                qwer["clmType"] = 1;
                            else
                                qwer["clmType"] = 0;
                        }
                        else
                            qwer["clmType"] = 4;
                    }
                    finally { qRd.Close(); }
                }
                catch { }
            int lstPos = -1;
            foreach (DataRow dr in dtRes.Rows)
            {
                int cP = Convert.ToInt32(dr["pos"]);
                if (cP >= lstPos)
                    lstPos = cP;
                else
                    dr["pos"] = lstPos;
            }
            try { SortingClass.SortTwoCases(dtRes, "pos", "clmType"); }
            catch { }
            //int col1 = dtRes.Columns.IndexOf("pos");
            //if (col1 >= 0)
            //{
            //    int col2 = dtRes.Columns.Count - 1;
            //    SortingClass.SortTwoCases(dtRes, col1, col2);
            //}

            
            #endregion

            dtRes.Columns.Add("posCol", typeof(int));
            dtRes.Columns.Add("posDiff", typeof(int));
            for (int ij = 0; ij < dtRes.Rows.Count; ij++)
            {
                dtRes.Rows[ij]["posCol"] = ij;
                dtRes.Rows[ij]["posDiff"] = 0;
            }
            List<object[]> vkNpLst = new List<object[]>();
            int ijj = 0;
            while (ijj < dtRes.Rows.Count)
                if ((int)dtRes.Rows[ijj]["clmType"] > 0)
                {
                    //if ((int)dtRes.Rows[ijj]["clmType"] == 1)
                    //{
                    //if(style!="Скорость")
                    for (int kk = ijj + 1; kk < dtRes.Rows.Count; kk++)
                    {
                        dtRes.Rows[kk]["pos"] = Convert.ToInt32(dtRes.Rows[kk]["pos"]) - 1;
                        dtRes.Rows[kk]["posDiff"] = (int)dtRes.Rows[kk]["posDiff"] + 1;
                    }
                    //}
                    vkNpLst.Add(dtRes.Rows[ijj].ItemArray);
                    dtRes.Rows.RemoveAt(ijj);

                }
                else
                    ijj++;
            cmd.Parameters.Clear();

            if (fr == FirstRoundType.QualificationFlash)
            {
                for (cnt = 0; cnt < dtRes.Rows.Count; cnt++)
                {
                    if ((dtRes.Rows[cnt][head].ToString().IndexOf("н/я") > -1) &&
                        (dtRes.Rows[cnt][head + 2].ToString().IndexOf("н/я") > -1))
                        break;
                }
            }
            else
            {
                var dtResColumns = dtRes.Columns.Cast<DataColumn>().ToList();
                for (cnt = 0; cnt < dtRes.Rows.Count; cnt++)
                {
                    if (dtRes.Rows[cnt][head].ToString().IndexOf("н/я") > -1)
                    {
                        int nRound = head + 1;
                        if (dtRes.Columns.Count > nRound)
                        {
                            string sResN = dtRes.Rows[cnt][nRound] as string;
                            if (String.IsNullOrEmpty(sResN) || sResN.IndexOf("н/я") > -1)
                                break;
                        }
                        else
                            break;
                    }
                    try
                    {
                        if (dtRes.Rows[cnt]["Место"].ToString().IndexOf("н/я") > -1)
                            break;
                    }
                    catch { }
                    try
                    {
                        if (dtResColumns.Any(c => c.ColumnName == "nyaA") && dtResColumns.Any(c => c.ColumnName == "nyaB"))
                        {
                            if ((Convert.ToInt32(dtRes.Rows[cnt]["nyaA"]) > 0) ||
                               (Convert.ToInt32(dtRes.Rows[cnt]["nyaB"]) > 0))
                                break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (dtResColumns.Any(c => c.ColumnName == "nya"))
                            if (Convert.ToInt32(dtRes.Rows[cnt]["nya"]) > 0)
                                break;
                    }
                    catch { }
                }
            }
            cntClm = cnt;
            bool restrict = SettingsForm.GetOnly75(cn, tran);
            bool sharePoints = SettingsForm.GetSharePoints(cn, tran);
            if (restrict)
                lastWithPoints = (int)(0.75 * (cnt));
            else
                lastWithPoints = (int)(cnt);
            if (lastWithPoints > pts.Length)
                lastWithPoints = pts.Length;
            //dtRes.Columns.Add("Балл", typeof(double));
            int k = -1, curPos = 1, curID = 0;
            do
            {
                k++;
                curID = (int)dtRes.Rows[k]["№"];
                CheckID(curID, out vk, out np);

            } while (np || vk);


            //for(i = 0; i < dtRes.Rows.Count; i++)

            int posIndx = curPos;
            int curPosIndx = k;
            if (sharePoints)
            {
                if (pts.Length >= posIndx)
                {
                    avgPts = pts[posIndx - 1];
                    cnt = dtRes.Rows.Count;
                    for (i = k + 1; i < cnt; i++)
                    {
                        DataRow dRow = dtRes.Rows[i];
                        int nxtId = (int)dRow["№"];
                        int nxtPos;
                        nxtPos = Convert.ToInt32(dRow["pos"]);
                        //if (nxtPos > int.MaxValue - 1000)
                        //    nxtPos = i + 1;
                        if (nxtPos > i + 1)
                            nxtPos = i + 1;
                        if (nxtPos > curPos)
                        {
                            avgPts /= (nxtPos - curPos);
                            avgPts = (double)((int)(avgPts * 10.0)) / 10.0;
                            head = lastWithPoints + 1 - curPos;
                            tail = nxtPos - 1 - lastWithPoints;
                            if (head >= tail)
                                for (int j = curPosIndx; j < i; j++)
                                    dtRes.Rows[j]["Балл"] = avgPts;
                            else
                                break;

                            //posIndx = nxtPos;
                            curPos = Convert.ToInt32(dRow["pos"]);// nxtPos;
                            if (curPos > int.MaxValue - 1000)
                                curPos = i + 1;
                            posIndx = curPos;
                            if (posIndx <= lastWithPoints)
                                avgPts = pts[posIndx - 1];
                            else
                                //avgPts = 0.0;
                                break;

                            curPosIndx = i;
                        }
                        else
                        {
                            posIndx++;
                            if (posIndx <= lastWithPoints)
                                avgPts += pts[posIndx - 1];
                        }
                    }
                    if (cnt - lastWithPoints > 1)
                        avgPts /= (cnt - curPos);
                    else
                        avgPts /= (cnt + 1 - curPos);
                    avgPts = (double)((int)(avgPts * 10.0)) / 10.0;
                    head = lastWithPoints + 1 - curPos;
                    tail = cnt - lastWithPoints;
                    if (head >= tail)
                        for (int j = curPosIndx; j < i; j++)
                            dtRes.Rows[j]["Балл"] = avgPts;
                }

            }
            else
                for (i = k; i < dtRes.Rows.Count; i++)
                {
                    posIndx = Convert.ToInt32(dtRes.Rows[i]["pos"]);
                    if (posIndx > lastWithPoints || posIndx >= pts.Length)
                        break;
                    dtRes.Rows[i]["Балл"] = pts[posIndx - 1];
                }


            foreach (object[] oLst in vkNpLst)
                dtRes.Rows.Add(oLst);
            SortingClass.SortByColumn(dtRes, 0, dtRes.Rows.Count, dtRes.Columns.IndexOf("posCol"));
            foreach (DataRow drdd in dtRes.Rows)
                drdd["pos"] = Convert.ToInt32(drdd["pos"]) + (int)drdd["posDiff"];
            dtRes.Columns.Remove("posDiff");
            dtRes.Columns.Remove("clmType");
            dtRes.Columns.Remove("posCol");

            bool speedFallsNoPoints = !SettingsForm.GetPointsForFalls(cn, tran);
            foreach (DataRow qwer in dtRes.Rows)
            {
                int iid = Convert.ToInt32(qwer["№"]);
                CheckID(iid, out vk, out np);
                if (np || vk || (speedFallsNoPoints && speedFalls.Contains(iid)))
                    qwer["Балл"] = 0.0;
                if ((qwer["Место"].ToString() == "н/я") ||
                    (qwer["Место"].ToString() == "дискв.") ||
                    (qwer["Место"].ToString() == ""))
                    qwer["Балл"] = 0.0;
            }

            if (!SettingsForm.GetLessOnePoint(cn, tran))
                foreach (DataRow qwer in dtRes.Rows)
                    if (Convert.ToDouble(qwer["Балл"]) < 1.0)
                        qwer["Балл"] = 0.0;
        }

        List<int> vKk, noPoits;

        void CheckID(int iid, out bool vk, out bool np)
        {
            vk = np = false;
            foreach (int i in noPoits)
                if (i == iid)
                {
                    np = true;
                    break;
                }
            if (!np)
                foreach (int i in vKk)
                    if (i == iid)
                    {
                        vk = true;
                        break;
                    }
        }

        void RefreshData(SqlTransaction tran)
        {

            List<RoundElem> rounds = new List<RoundElem>();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT iid,round,listType FROM lists(NOLOCK) WHERE (style='" +
                style + "') AND (group_id=" + groupID.ToString() + ") AND (prev_round IS NULL) AND " +
                "((round != 'Квалификация 1') OR (style!='Трудность')) AND  (round != 'Квалификация 2') AND " +
                "(round != '1/4 финала Трасса 1') AND (round != '1/4 финала Трасса 2') AND " +
                "(round != 'Квалификация Группа А') AND (round != 'Квалификация Группа Б') AND " +
                "(round != 'Итоговый протокол') AND (iid_parent IS NULL)";
            rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                RoundElem rEl = new RoundElem();
                rEl.name = rd[1].ToString();
                rEl.iid = Convert.ToInt32(rd[0]);
                rEl.start = 'F';
                rEl.stop = 'A';
                rEl.count = 0;
                rEl.countPar = 0;
                try { rEl.listType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), rd["listType"].ToString(), true); }
                catch { rEl.listType = ListTypeEnum.Unknown; }
                rEl.hasLeadTime = false;
                rounds.Add(rEl);
                //iids.Add(Convert.ToInt32(rd[0]));
                //rounds.Add(rd[1].ToString());
                break;
            }
            rd.Close();
            
            if (rounds.Count > 0)
            {
                switch (rounds[0].listType)
                {
                    case ListTypeEnum.LeadFlash:
                        fr = FirstRoundType.QualificationFlash;
                        break;
                    case ListTypeEnum.LeadGroups:
                        fr = FirstRoundType.TwoRoutes;
                        break;
                    case ListTypeEnum.BoulderGroups:
                        fr = FirstRoundType.TwoRoutes;
                        break;
                    default:
                        fr = FirstRoundType.OneRoute;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Вид не проведён.");
                return;
            }
            bool b;
            int i = 0;
            do
            {
                cmd.CommandText = "SELECT ln.iid, ln.round, ln.listType FROM lists ln(NOLOCK) JOIN lists l(NOLOCK) ON ln.iid=" +
                    "l.next_round WHERE (l.style='" + style + "') AND (l.group_id=" + groupID.ToString() +
                    ") AND (l.iid=" + rounds[i].iid.ToString() + ")";
                rd = cmd.ExecuteReader();
                b = false;
                while (rd.Read())
                {
                    try
                    {
                        RoundElem rEl = new RoundElem();
                        rEl.name = rd[1].ToString();
                        rEl.iid = Convert.ToInt32(rd[0]);
                        rEl.start = 'A';
                        rEl.stop = 'A';
                        rEl.count = rEl.countPar = 0;
                        try { rEl.listType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), rd["listType"].ToString(), true); }
                        catch { rEl.listType = ListTypeEnum.Unknown; }
                        rEl.hasLeadTime = false;
                        rounds.Add(rEl);
                        //rounds.Add(rd[1].ToString());
                        //iids.Add(Convert.ToInt32(rd[0]));
                        i++;
                        b = true;
                    }
                    catch { b = false; }
                    break;
                }
                rd.Close();
            } while (b);
            roundList = rounds.ToArray();

            if (SettingsForm.GetLeadTime(cmd.Connection, cmd.Transaction))
            {
                for (int iooo = 0; iooo < roundList.Length; iooo++)
                    roundList[iooo].hasLeadTime = StaticClass.ListHasLeadTime(roundList[iooo].iid, cmd.Connection, cmd.Transaction);
            }

            MakeResultList(fr, tran);
            SetPts(tran);
            /*if (dg != null && dg.DataSource != dtRes)
                dg.DataSource = dtRes;*/
            try { dtRes.Columns.Remove("pos"); }
            catch { }
        }

        private void xlExport_Click(object sender, EventArgs e)
        {
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;
            DataTable dtPrint;
            if (bParallel)
                dtPrint = dtParallel;
            else
                dtPrint = dtRes;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, true, cn))
                return;
            try
            {
                int i = 0;
                int frst_row;

                frst_row = 7;
                foreach (DataColumn cl in dtPrint.Columns)
                {
                    i++;
                    if (cl.ColumnName == "№" || cl.ColumnName.IndexOf("disq") > -1 || cl.ColumnName.IndexOf("nya") > -1)
                    {
                        i--;
                        continue;
                    }
                    else
                        if (cl.ColumnName == "ФамилияИмя")
                            ws.Cells[frst_row, i] = "Фамилия, Имя";
                        else if (cl.ColumnName.Equals("Разряд", StringComparison.InvariantCultureIgnoreCase))
                            ws.Cells[frst_row, i] = "Разр.";
                        else
                            ws.Cells[frst_row, i] = cl.ColumnName;
                }
                Excel.Range dt;

                if (SettingsForm.GetShowNewQf(cn))
                {
                    ws.Cells[frst_row, i + 1] = "Вып. разр.";
                    dt = ws.get_Range(ws.Cells[frst_row, i + 1], ws.Cells[frst_row, i + 1]);
                    dt.WrapText = true;
                    dt = ws.get_Range(ws.Cells[frst_row, i + 1], ws.Cells[2000, i + 1]);
                    dt.ColumnWidth = 4.57;
                    dt.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }
                i = frst_row;
                int lastPts = frst_row;
                int lastRow = -1, lastRowTot = -1;
                
                foreach (DataRow rd in dtPrint.Rows)
                {
                    i++;
                    try
                    {
                        if (rd["Балл"].ToString() != "0")
                            lastPts = i;
                    }
                    catch { }
                    char[] qaz = { 'T', 'B' };
                    int k = 1, j = 0;

                    int teamIndex = int.MaxValue;
                    int pointsIndex = int.MaxValue;

                    while (k <= dtPrint.Columns.Count)
                    {
                        if (dtPrint.Columns[k - 1].ColumnName.Equals("команда", StringComparison.OrdinalIgnoreCase))
                            teamIndex = k;
                        else if (dtPrint.Columns[k - 1].ColumnName.Equals("балл", StringComparison.OrdinalIgnoreCase))
                            pointsIndex = k;
                        if (dtPrint.Columns[k - 1].ColumnName == "№")
                        {
                            j++;
                            k++;
                            continue;
                        }
                        if ((dtPrint.Columns[k - 1].ColumnName == "Балл") && (rd[k - 1].ToString() == "0"))
                        {
                            k++;
                            continue;
                        }
                        if (dtPrint.Columns[k - 1].ColumnName.IndexOf("nya") > -1 ||
                            dtPrint.Columns[k - 1].ColumnName.IndexOf("disq") > -1)
                        {
                            j++;
                            bool b;
                            try { b = Convert.ToInt32(rd[k - 1]) > 0; }
                            catch { b = false; }
                            if (b)
                            {
                                dt = ws.get_Range(ws.Cells[i, k - j + 1], ws.Cells[i, k - j + 4]);
                                dt.Merge(Type.Missing);
                                if (dtPrint.Columns[k - 1].ColumnName.IndexOf("nya") > -1)
                                {
                                    dt.Cells[1, 1] = "н/я";
                                    k += 5;
                                }
                                else
                                {
                                    dt.Cells[1, 1] = "дискв.";
                                    k += 6;
                                    j++;
                                }
                            }
                            else
                                k++;
                            continue;
                        }
                        if (rd[k - 1] == null || rd[k - 1] == DBNull.Value)
                            ws.Cells[i, k - j] = string.Empty;
                        else if (k > teamIndex && k < pointsIndex)
                            ws.Cells[i, k - j] = string.Format(" {0}", rd[k - 1]);
                        else
                            ws.Cells[i, k - j] = rd[k - 1];
                        k++;
                    }
                    if ((rd["Г.р."].ToString() == "0"))
                        ws.Cells[i, 4 - j] = "";
                    string str = ws.get_Range(ws.Cells[i, 1], ws.Cells[i, 1]).Text.ToString().Trim();
                    if (str.Equals("1") || str.Equals("2") || str.Equals("3"))
                        lastRow = i;
                    lastRowTot = i;
                }

                dt = ws.get_Range("A" + (frst_row + 1).ToString(), "E" + (frst_row + dtPrint.Rows.Count).ToString());
                dt.Style = "MyStyle";
                dt.Columns.AutoFit();

                dt = ws.get_Range("B" + (frst_row + 1).ToString(), "B" + (frst_row + dtPrint.Rows.Count).ToString());
                dt.Style = "StyleLA";

                dt = ws.get_Range("A5", "A5");
                if (judgeID > 0)
                    dt.Cells[1, 1] = "Зам. гл. судьи по виду: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    dt.Cells[1, 1] = "Зам. гл. судьи по виду:";

                int emptyRounds = 0;

                for (int ii = 0; ii < roundList.Length; ii++)
                {
                    if (bParallel)
                    {
                        if (roundList[ii].countPar > 0)
                            dt = ws.get_Range(roundList[ii].start + frst_row.ToString(), roundList[ii].stop + (frst_row + roundList[ii].countPar).ToString());
                        else
                        {
                            emptyRounds++;
                            continue;
                        }
                    }
                    else
                        dt = ws.get_Range(roundList[ii].start + frst_row.ToString(), roundList[ii].stop + (frst_row + roundList[ii].count).ToString());
                    dt.Style = "MyStyle";
                    dt.Columns.AutoFit();

                    if (roundList[ii].name == "Квалификация (2 группы)")
                    {
                        char cT = roundList[ii].start;
                        for (int tty = 0; tty < 3; tty++)
                            cT++;
                        dt = ws.get_Range(roundList[ii].start + (frst_row - 1).ToString(),
                            cT + (frst_row - 1).ToString());
                        dt.Style = "MyStyle";
                        dt.Merge(Type.Missing);
                        dt.Cells[1, 1] = "Квал. Группа А";
                        dt = ws.get_Range(++cT + (frst_row - 1).ToString(),
                            roundList[ii].stop + (frst_row - 1).ToString());
                        dt.Style = "MyStyle";
                        dt.Merge(Type.Missing);
                        dt.Cells[1, 1] = "Квал. Группа Б";
                    }
                    else
                    {
                        string strtTmp = roundList[ii].name.Replace("инал", "ин.");
                        strtTmp = strtTmp.Replace(".а", ".");
                        strtTmp = strtTmp.Replace("упер", ".");
                        strtTmp = strtTmp.Replace(" (2 трассы)", "");
                        strtTmp = strtTmp.Replace("валификация", "вал.");

                        if (roundList[ii].start == roundList[ii].stop)
                        {
                            //dt = ws.get_Range(roundList[ii].start + frst_row.ToString(), roundList[ii].stop + frst_row.ToString());
                            dt = ws.get_Range(roundList[ii].start + (frst_row - 1).ToString(), roundList[ii].stop + frst_row.ToString());
                            dt.Style = "MyStyle";
                            dt.Merge(Type.Missing);
                        }
                        else if (
                            roundList[ii].stop == (char)(roundList[ii].start + 1)
                            &&
                            roundList[ii].name == "Квалификация 2"
                            &&
                            style == "Скорость"
                            &&
                            (SettingsForm.GetSpeedRules(cn) & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds
                            )
                        {
                            //dt = ws.get_Range(roundList[ii].start + frst_row.ToString(), roundList[ii].stop + frst_row.ToString());
                            dt = ws.get_Range(roundList[ii].stop + (frst_row - 1).ToString(), roundList[ii].stop + frst_row.ToString());
                            dt.Style = "MyStyle";
                            dt.Merge(Type.Missing);
                            dt.Cells[1, 1] = "Квал.Итог";
                            dt.Columns.AutoFit();

                            dt = ws.get_Range(roundList[ii].start + (frst_row - 1).ToString(), roundList[ii].start + frst_row.ToString());
                            dt.Style = "MyStyle";
                            dt.Merge(Type.Missing);
                            
                        }
                        else
                        {
                            dt = ws.get_Range(roundList[ii].start + (frst_row - 1).ToString(), roundList[ii].stop + (frst_row - 1).ToString());
                            dt.Style = "MyStyle";
                            dt.Merge(Type.Missing);
                            Excel.Range dtL= ws.get_Range(roundList[ii].start + frst_row.ToString(), roundList[ii].stop + frst_row.ToString());
                            if (roundList[ii].listType == ListTypeEnum.LeadFlash)
                            {
                                int rCurNum = roundList[ii].lists.Length;// (roundList[ii].stop - roundList[ii].start) / 2;
                                int iCell = 0;
                                for (int iRN = 1; iRN <= rCurNum; iRN++)
                                {
                                    dtL.Cells[1, ++iCell] = "Тр. " + iRN.ToString();
                                    if (roundList[ii].lists[iRN - 1].Value)
                                        dtL.Cells[1, ++iCell] = "Время " + iRN.ToString();
                                    dtL.Cells[1, ++iCell] = "Балл " + iRN.ToString();
                                }
                                dtL.Cells[1, ++iCell] = "Рез-т";
                            }
                            else if (roundList[ii].hasLeadTime)
                            {
                                dtL.Cells[1, 1] = "Рез-т";
                                dtL.Cells[1, 2] = "Время";
                            }
                        }
                        dt.Cells[1, 1] = strtTmp;

                        dt.Columns.AutoFit();
                    }
                }

                char ptsI = (char)(roundList[roundList.Length - 1].stop - emptyRounds);
                if (SettingsForm.GetShowNewQf(cn))
                    ptsI++;
                //if(!bParallel)
                ptsI++;
                for (char cc = 'F'; cc != ptsI; cc++)
                {
                    dt = ws.get_Range(cc + frst_row.ToString(), cc + frst_row.ToString());
                    if (dt.Text.ToString().IndexOf("T") > -1)
                    {
                        if (dt.Text.ToString().IndexOf("a") > -1)
                            dt.Cells[1, 1] = "Поп.";
                        else
                            dt.Cells[1, 1] = "Top";
                        dt.Columns.AutoFit();
                    }
                    else
                        if (dt.Text.ToString().IndexOf("B") > -1)
                        {
                            if (dt.Text.ToString().IndexOf("a") > -1)
                                dt.Cells[1, 1] = "Поп.";
                            else
                                dt.Cells[1, 1] = "Бон.";
                            dt.Columns.AutoFit();
                        }

                }
                if (bParallel)
                    ptsI--;
                else
                {
                    try
                    {
                        if (SettingsForm.GetShowNewQf(cn))
                            ptsI--;
                        dt = ws.get_Range(ptsI + frst_row.ToString(), ptsI + lastPts.ToString());
                        dt.Style = "MyStyle";
                        dt.Columns.AutoFit();
                        if (SettingsForm.GetShowNewQf(cn))
                        {
                            ptsI++;
                            dt = ws.get_Range(ptsI + frst_row.ToString(), ptsI + frst_row.ToString());
                            dt.Style = "MyStyle";
                        }
                    }
                    catch { }
                }

                for (char cc = 'A'; cc < 'F'; cc++)
                {
                    dt = ws.get_Range(cc + frst_row.ToString(), cc + frst_row.ToString());
                    string strTTmp = dt.Text.ToString();
                    dt = ws.get_Range(cc + (frst_row - 1).ToString(), cc + frst_row.ToString());
                    if (cc == 'B')
                        dt.Style = "StyleLA";
                    else
                        dt.Style = "MyStyle";
                    dt.Merge(Type.Missing);
                    dt.Cells[1, 1] = strTTmp;

                }
                String strTmp;
                char ptsStrt = SettingsForm.GetShowNewQf(cn) ? (char)(ptsI - 1) : ptsI;
                for (char cTm = ptsStrt; cTm <= ptsI; cTm++)
                {
                    dt = ws.get_Range(cTm + frst_row.ToString(), cTm + frst_row.ToString());
                    strTmp = dt.Text.ToString();
                    if (!bParallel)
                    {

                        dt = ws.get_Range(cTm + (frst_row - 1).ToString(), cTm + frst_row.ToString());
                        dt.Style = "MyStyle";
                        dt.Merge(Type.Missing);
                        dt.Cells[1, 1] = strTmp;
                    }
                }
                if (bParallel)
                    dt = ws.get_Range("A" + (frst_row - 1).ToString(), ptsI + (roundList[0].countPar + frst_row).ToString());
                else
                    dt = ws.get_Range("A" + (frst_row - 1).ToString(), ptsI + (roundList[0].count + frst_row).ToString());
                dt.Columns.AutoFit();

                dt = ws.get_Range("A1", ptsI + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

                dt = ws.get_Range(ptsI + "2", ptsI + "2");
                dt.Style = "StyleRA";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];

                dt = ws.get_Range("A4", ptsI + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                if (bParallel)
                    strTmp = this.style + ". " + tbGName.Text;
                else
                    strTmp = this.style + ". " + gName;
                dt.Cells[1, 1] = strTmp;

                if (lastRow > frst_row)
                {
                    try
                    {
                        dt = ws.get_Range(ws.Cells[frst_row + 1, 1], ws.Cells[lastRow, 255]);
                        dt.Font.Bold = true;

                        dt = ws.get_Range(ws.Cells[frst_row - 1, 1], ws.Cells[lastRowTot, 255]);
                        dt.Columns.AutoFit();
                    }
                    catch { }
                }

                strTmp = "ИТОГОВЫЙ ПРОТОКОЛ РЕЗУЛЬТАТОВ";
                dt = ws.get_Range("A3", ptsI + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "ИТ_";
                switch (this.style)
                {
                    case "Боулдеринг":
                        strTmp += "Б_";
                        break;
                    case "Трудность":
                        strTmp += "ТР_";
                        break;
                    case "Скорость":
                        strTmp += "С_";
                        break;
                }
                if (bParallel)
                    strTmp += StaticClass.CreateSheetName(tbGName.Text, "");
                else
                    strTmp += StaticClass.CreateSheetName(gName, "");

                try { ws.Name = strTmp; }
                catch { }

                if (SettingsForm.GetShowNewQf(cn))
                {
                    dt = ws.get_Range(ptsI + (frst_row - 1).ToString(), ptsI + frst_row.ToString());
                    dt.WrapText = true;
                    dt.ColumnWidth = 4.57;
                }

                if (this.style == "Скорость" && !bParallel)
                {
                    Excel.Worksheet ws2 = (Excel.Worksheet)wb.Worksheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);

                    if (ws2 == null)
                    {
                        MessageBox.Show("Лист Excel не может быть создан.");
                        return;
                    }

                    ((Excel._Worksheet)ws2).Activate();
                    DrawSchema(ws2);
                    ws2.Name = strTmp + "_СХЕМА";

                    StaticClass.PrintCopyright(xlApp, ws2);

                    StaticClass.PrintFooter(ws2, cn);

                    ws2.PageSetup.Zoom = false;
                    ws2.PageSetup.FitToPagesWide = 1;
                    if (SettingsForm.GetOneListExcel(cn))
                        ws2.PageSetup.FitToPagesWide = 1;
                    else
                    {
                        int im = 32767;
                        while (im > 2)
                            try
                            {
                                ws2.PageSetup.FitToPagesTall = im;
                                break;
                            }
                            catch { im /= 2; }
                    }
                    ws2.PageSetup.CenterHorizontally = ws2.PageSetup.CenterVertically = true;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта в Excel:\r\n" + 
#if DEBUG
                ex.ToString()
#else
                ex.Message
#endif
                ); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void DrawSchema(Excel.Worksheet ws)
        {
            if (roundList.Length == 0)
                return;
            int stRow = 5, stColumn = 1, p = 3, Dp = 4;
            string strTmp = "";
            foreach (RoundElem rEl in roundList)
                if (rEl.name.IndexOf("инал") > -1)
                    DrawRound(rEl.iid, ws, ref p, ref Dp, ref stRow, ref stColumn, rEl.name == "Финал");
            strTmp = roundList[roundList.Length - 1].name;
            if (strTmp != "Финал")
            {
                int nTmp = strTmp.IndexOf('/');
                if (nTmp > 0)
                {
                    int n2 = strTmp.IndexOf(' ', nTmp);
                    if (n2 > 0)
                    {
                        int q = int.Parse(strTmp.Substring(nTmp + 1, n2 - n2 + 1));
                        while (q > 2)
                        {
                            DrawBlankRound(q, ws, ref p, ref Dp, ref stRow, ref stColumn, false);
                            q /= 2;
                        }
                        DrawBlankRound(4, ws, ref p, ref Dp, ref stRow, ref stColumn, true);
                    }
                }
            }

            ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
            char c = 'A';
            for (int i = 2; i < stColumn; i++)
                c++;
            Excel.Range dt = ws.get_Range("A1", c + "1");
            dt.Style = "CompTitle";
            dt.Merge(Type.Missing);

            strTmp = "";
            dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
            ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];
            dt = ws.get_Range(c + "2", c + "2");
            dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
            dt.Style = "StyleRA";

            dt = ws.get_Range("A3", c + "3");
            dt.Style = "Title";
            dt.Merge(Type.Missing);
            strTmp = this.style + ". " + gName;
            dt.Cells[1, 1] = strTmp;

            strTmp = "ФИНАЛЬНАЯ ЧАСТЬ";
            dt = ws.get_Range("A4", c + "4");
            dt.Style = "Title";
            dt.Merge(Type.Missing);
            dt.Cells[1, 1] = strTmp;

        }

        private void DrawBox(Excel.Range range)
        {
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex =
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex =
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex =
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight =
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight =
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight =
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
        }

        private void DrawRound(int iid, Excel.Worksheet ws, ref int pairDif, ref int difBetween, ref int startRow, ref int startColumn, bool final)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                Excel.Range rnge;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT sr.start,sr.route1_text,sr.resText,sr.posText,sp.posText,(p.surname+' '+p.name),sr.qf,sr.route2_text " +
                    "FROM speedResults sr(NOLOCK) INNER JOIN lists l(NOLOCK) ON sr.list_id=l.iid INNER JOIN speedResults sp(NOLOCK) ON " +
                    "(l.prev_round=sp.list_id) AND (sr.climber_id=sp.climber_id) INNER JOIN Participants p ON " +
                    "sr.climber_id=p.iid WHERE sr.list_id=" + iid.ToString() + " ORDER BY sr.start";
                rd = cmd.ExecuteReader();
                int row = startRow - difBetween + 1;
                int curStart = 0;
                int rTmp = -1;
                char cSt = 'A';
                for (int i = 1; i < startColumn; i++)
                    cSt++;
                char cSt2 = cSt;
                cSt2++;
                string rt = "";
                while (rd.Read())
                {
                    int curS = Convert.ToInt32(rd[0]);
                    if (curS % 2 == 1)
                        rt = rd[1].ToString();
                    else
                        rt = rd[7].ToString();
                    if (final)
                    {
                        char cT = cSt2;
                        cT++; cT++;
                        if (curS < 3)
                            if (rTmp == -1)
                            {
                                rTmp = row;
                                row += (pairDif + difBetween + 4);
                                if (rd[3].ToString() == "3")
                                {
                                    ws.Cells[row + 1, startColumn + 3] = "III - " + rd[5].ToString();
                                    rnge = ws.get_Range(cT.ToString() + (row + 1).ToString(),
                                        cT.ToString() + (row + 1).ToString());
                                    rnge.Style = "MyStyle";
                                    DrawBox(rnge);
                                }
                            }
                            else
                            {
                                if (rd[3].ToString() == "3")
                                {
                                    ws.Cells[row + 1, startColumn + 3] = "III - " + rd[5].ToString();
                                    rnge = ws.get_Range(cT.ToString() + (row + 1).ToString(),
                                        cT.ToString() + (row + 1).ToString());
                                    rnge.Style = "MyStyle";
                                    DrawBox(rnge);
                                }
                                row += 3;
                            }
                        else
                        {
                            if (rTmp != -1)
                            {
                                row = rTmp;
                                rTmp = -1;
                            }
                            if ((curS % 2) == 1)
                            {
                                row += difBetween;
                                if (rd[3].ToString() == "1")
                                {
                                    ws.Cells[row + (int)(pairDif / 2), startColumn + 3] = "I - " + rd[5].ToString();
                                    rnge = ws.get_Range(cT.ToString() + (row + (int)(pairDif / 2)).ToString(),
                                        cT.ToString() + (row + (int)(pairDif / 2)).ToString());
                                    rnge.Style = "MyStyle";
                                    DrawBox(rnge);
                                }
                            }
                            else
                            {
                                if (rd[3].ToString() == "1")
                                {
                                    ws.Cells[row + (int)(pairDif / 2), startColumn + 3] = "I - " + rd[5].ToString();
                                    rnge = ws.get_Range(cT.ToString() + (row + (int)(pairDif / 2)).ToString(),
                                        cT.ToString() + (row + (int)(pairDif / 2)).ToString());
                                    rnge.Style = "MyStyle";
                                    DrawBox(rnge);
                                }
                                row += pairDif;
                            }
                        }
                    }
                    else
                        if ((curS - curStart) > 1)
                            row += (pairDif + difBetween);
                        else
                        {
                            if ((curS % 2) == 1)
                                row += difBetween;
                            else
                                row += pairDif;
                        }
                    ws.Cells[row, startColumn] = rd[4].ToString() + " " + rd[5].ToString();
                    ws.Cells[row, startColumn + 1] = rd[2].ToString();

                    string strTmp = cSt.ToString() + row.ToString();
                    rnge = ws.get_Range(strTmp, strTmp);
                    rnge.Style = "al_left";
                    strTmp = cSt2.ToString() + row.ToString();
                    rnge = ws.get_Range(strTmp, strTmp);
                    rnge.Style = "al_right";

                    DrawBox(ws.get_Range(ws.Cells[row, startColumn], ws.Cells[row, startColumn + 1]));

                    ws.Cells[row - 1, startColumn + 1] = rt;
                    if (rd[6].ToString() != "Q")
                        ws.Cells[row, startColumn + 2] = rd[3].ToString();

                    curStart = curS;
                }
                rd.Close();
                if (final)
                    row += 7;
                for (int i = 0; i < 3; i++)
                {
                    rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                    rnge.Columns.AutoFit();
                    cSt++;
                }
                if (final)
                {
                    rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                    rnge.Columns.AutoFit();
                }
                else
                {
                    rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                    rnge.ColumnWidth = 1.5;
                }
                int nTmp = difBetween + 2;
                startRow += (pairDif - 1);
                difBetween = pairDif * 2 + difBetween - 2;
                pairDif = nTmp;
                startColumn += 4;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DrawBlankRound(int cnt, Excel.Worksheet ws, ref int pairDif, ref int difBetween, ref int startRow, ref int startColumn, bool final)
        {
            Excel.Range rnge;

            int row = startRow - difBetween + 1;
            int rTmp = -1;
            char cSt = 'A';
            for (int i = 1; i < startColumn; i++)
                cSt++;
            char cSt2 = cSt;
            cSt2++;
            //string rt = "";
            for (int curS = 1; curS <= cnt; curS++)
            {
                //int curS = Convert.ToInt32(rd[0]);

                if (final)
                {
                    char cT = cSt2;
                    cT++; cT++;
                    if (curS < 3)
                        if (rTmp == -1)
                        {
                            rTmp = row;
                            row += (pairDif + difBetween + 4);
                            ws.Cells[row + 1, startColumn + 3] = "III -                               ";
                            rnge = ws.get_Range(cT.ToString() + (row + 1).ToString(),
                                cT.ToString() + (row + 1).ToString());
                            rnge.Style = "MyStyle";
                            DrawBox(rnge);
                        }
                        else
                        {
                            row += 3;
                        }
                    else
                    {
                        if (rTmp != -1)
                        {
                            row = rTmp;
                            rTmp = -1;
                        }
                        if ((curS % 2) == 1)
                        {
                            row += difBetween;
                            ws.Cells[row + (int)(pairDif / 2), startColumn + 3] = "I -                               ";
                            rnge = ws.get_Range(cT.ToString() + (row + (int)(pairDif / 2)).ToString(),
                                cT.ToString() + (row + (int)(pairDif / 2)).ToString());
                            rnge.Style = "MyStyle";
                            DrawBox(rnge);
                        }
                        else
                        {
                            row += pairDif;
                        }
                    }
                }
                else
                {
                    if ((curS % 2) == 1)
                        row += difBetween;
                    else
                        row += pairDif;
                }

                string strTmp = cSt.ToString() + row.ToString();
                ws.Cells[row, startColumn] = "                              ";
                rnge = ws.get_Range(strTmp, strTmp);
                rnge.Style = "al_left";
                strTmp = cSt2.ToString() + row.ToString();
                rnge = ws.get_Range(strTmp, strTmp);
                rnge.Style = "al_right";
                DrawBox(ws.get_Range(ws.Cells[row, startColumn], ws.Cells[row, startColumn + 1]));

            }

            if (final)
                row += 7;
            for (int i = 0; i < 3; i++)
            {
                rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                rnge.Columns.AutoFit();
                cSt++;
            }
            if (final)
            {
                rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                rnge.Columns.AutoFit();
            }
            else
            {
                rnge = ws.get_Range(cSt.ToString() + "1", cSt.ToString() + row.ToString());
                rnge.ColumnWidth = 1.5;
            }
            int nTmp = difBetween + 2;
            startRow += (pairDif - 1);
            difBetween = pairDif * 2 + difBetween - 2;
            pairDif = nTmp;
            startColumn += 4;
        }

        bool inEvent = false;
        private void cbRestrict_CheckedChanged(object sender, EventArgs e)
        {
            if(inEvent)
                return;
            inEvent = true;
            try
            {
                if (MessageBox.Show(this, "Данное действие затронет все протоколы. Продолжить?", "", MessageBoxButtons.YesNo,
                     MessageBoxIcon.Warning) == DialogResult.No)
                {
                    cbRestrict.Checked = !cbRestrict.Checked;
                    return;
                }
                SettingsForm.SetOnly75(cbRestrict.Checked, cn);
                RefreshTable(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения данных:\r\n" + ex.Message);
                cbRestrict.Checked = !cbRestrict.Checked;
            }
            finally { inEvent = false; }
            /*
            restrict = cbRestrict.Checked;
            if (allowEvent)
            {
                StaticClass.DoSimpleWork(UpdateRestrict);
                RefreshTable();
            }*/
        }

        /*private void UpdateRestrict()
        {
            try
            {
                SqlConnection cnI = new SqlConnection(cn.ConnectionString);
                cnI.Open();
                try
                {
                    SqlCommand cmdl = new SqlCommand();
                    cmdl.Connection = cnI;
                    cmdl.CommandText = "UPDATE lists SET restrictPoints=@r WHERE iid=" + listID.ToString();
                    cmdl.Parameters.Add("@r", SqlDbType.Bit);
                    cmdl.Parameters[0].Value = cbRestrict.Checked;
                    cmdl.ExecuteNonQuery();
                }
                finally { cnI.Close(); }
            }
            catch { }
        }*/

        private void cbParallel_CheckedChanged(object sender, EventArgs e)
        {
            gbParallel.Enabled = cbParallel.Checked;
            bParallel = false;
            if (!cbParallel.Checked)
                RefreshData(null);
        }

        bool bParallel = false;

        DataTable dtParallel = new DataTable();

        private void btnCount_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    yOld = int.Parse(tbOld.Text);
                    if (yOld < 15)
                        yOld += 2000;
                    else
                        if (yOld < 100)
                            yOld += 1900;
                }
                catch
                {
                    if (MessageBox.Show("Старший г.р. введён неправильно. Будет использован 0.", "",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                    yOld = 0;
                }


                try
                {
                    yYoung = int.Parse(tbYoung.Text);
                    if (yYoung < 15)
                        yYoung += 2000;
                    else
                        if (yYoung < 100)
                            yYoung += 1900;
                }
                catch
                {
                    if (MessageBox.Show("Младший г.р. введён неправильно. Будет использован " + DateTime.Now.Year.ToString() +
                        ".", "",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                    yYoung = DateTime.Now.Year;
                }

                if (yOld > yYoung)
                {
                    int nTmp = yOld;
                    yOld = yYoung;
                    yYoung = nTmp;
                }
                tbOld.Text = yOld.ToString();
                tbYoung.Text = yYoung.ToString();

                bParallel = true;

                RefreshData(null);

                dtParallel.Clear();
                dtParallel.Columns.Clear();
                foreach (DataColumn dcl in dtRes.Columns)
                    dtParallel.Columns.Add(dcl.ColumnName, dcl.DataType);

                foreach (DataRow dr in dtRes.Rows)
                {
                    int year;
                    try { year = Convert.ToInt32(dr["Г.р."]); }
                    catch { year = 0; }
                    if (year >= yOld && year <= yYoung)
                        dtParallel.Rows.Add(dr.ItemArray);
                }

                int curPts = -1;
                int curInd = -1;
                while (curInd < (dtParallel.Rows.Count - 1) && curPts == -1)
                {
                    try { curPts = Convert.ToInt32(dtParallel.Rows[++curInd]["Место"]); }
                    catch { }
                }
                if (curPts > -1)
                {
                    int nVk = 0, curPl = 1;
                    dtParallel.Rows[curInd]["Место"] = "1";
                    for (int i = curInd + 1; i < dtParallel.Rows.Count; i++)
                        try
                        {
                            int cPts = Convert.ToInt32(dtParallel.Rows[i]["Место"]);
                            if (cPts != curPts)
                            {
                                curPts = cPts;
                                curPl = i - curInd + 1 - nVk;
                            }
                            dtParallel.Rows[i]["Место"] = curPl.ToString();
                        }
                        catch { nVk++; }
                    for (int i = dtParallel.Columns.Count - 1; i >= 0; i--)
                    {
                        bool b = false;
                        foreach (DataRow dr in dtParallel.Rows)
                        {
                            b = (dr[i].ToString() != "");
                            if (b)
                                break;
                        }
                        if (!b)
                            dtParallel.Columns.RemoveAt(i);
                    }
                }



                try { dtParallel.Columns.Remove("Балл"); }
                catch { }
                dg.DataSource = dtParallel;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка подсчёта параллельного зачёта:\r\n" + ex.Message); }
        }

        private void ShowFinalResults_Load(object sender, EventArgs e)
        {
            inEvent = true;
            try { cbRestrict.Checked = SettingsForm.GetOnly75(cn); }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                this.Close();
            }
            finally { inEvent = false; }
        }

        private void btnCalcQfs_Click(object sender, EventArgs e)
        {
            string res = CalculateQfList(bParallel ? dtParallel : dtRes, cn, null);
            if (String.IsNullOrEmpty(res))
                res = "Нет данных для подсчета";
            MessageBox.Show(this, res,"Cписок по разрядам");
        }

        private sealed class QVP
        {
            public string Qf;
            public int Qt;
            public QVP(string Qf, int Qt)
            {
                this.Qf = Qf;
                this.Qt = Qt;
            }
        }
        private static string CalculateQfList(DataTable dt, SqlConnection cn, SqlTransaction tran)
        {
            if (dt == null)
                return String.Empty;
            DataColumn dcQf = null, dcPlace = null;
            foreach (DataColumn dc in dt.Columns)
            {
                if (dcQf == null && dc.ColumnName.ToLower().IndexOf("разр") > -1)
                    dcQf = dc;
                else if (dcPlace == null && dc.ColumnName.Equals("Место", StringComparison.InvariantCultureIgnoreCase))
                    dcPlace = dc;
                if (dcQf != null && dcPlace != null)
                    break;
            }
            if (dcPlace == null || dcQf == null)
                return String.Empty;
            List<QVP> qfs = new List<QVP>();

            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;

            cmd.CommandText = "SELECT iid, qf FROM qfList(NOLOCK) WHERE qf <> '' AND qf IS NOT NULL ORDER BY iid";
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    qfs.Add(new QVP(rdr["qf"].ToString().Trim(), 0));
            }

            foreach (DataRow row in dt.Rows)
            {
                string qf = row[dcQf] as string;
                if (String.IsNullOrEmpty(qf))
                    continue;
                qf = qf.Trim();

                string pos = row[dcPlace] as string;
                if (pos == null)
                    pos = String.Empty;
                else
                    pos = pos.ToLower();
                if (pos.IndexOf("в/к") > -1)
                    continue;
                if (String.IsNullOrEmpty(pos))
                {
                    int fallInd = int.MaxValue, dsqInd = int.MaxValue, nYaInd = int.MaxValue;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string sR = row[i] as string;
                        if (String.IsNullOrEmpty(sR))
                            continue;
                        sR = sR.ToLower();
                        if (nYaInd == int.MaxValue && sR.IndexOf("н/я") > -1)
                            nYaInd = i;
                        else if (dsqInd == int.MaxValue && sR.IndexOf("дискв.") > -1)
                            dsqInd = i;
                        else if (fallInd == int.MaxValue && sR.IndexOf("срыв") > -1)
                            fallInd = i;
                    }
                    if (nYaInd < fallInd && nYaInd < dsqInd)
                        continue;
                }
                bool updated = false;
                for (int i = 0; i < qfs.Count; i++)
                    if (qfs[i].Qf.Equals(qf, StringComparison.InvariantCultureIgnoreCase))
                    {
                        qfs[i].Qt++;
                        updated = true;
                        break;
                    }
                if (!updated)
                    qfs.Add(new QVP(qf, 1));

            }
            string res = String.Empty;
            foreach (var v in qfs)
            {
                if (!String.IsNullOrEmpty(res))
                    res += "\r\n";
                res += v.Qf + " - " + v.Qt.ToString();
            }
            return res;
        }

        private void btnRecalc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Все измененные вручную места будут пересчитаны. Продолжить?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;
            RefreshTable(true);
        }

        private void dg_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dtRes.Rows.Count)
                return;
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE generalResults SET pos=@pos, pts=@pts WHERE list_id=@listID and climber_id=@clm";
                cmd.Parameters.Add("@listID", SqlDbType.Int).Value = listID;
                cmd.Parameters.Add("@clm", SqlDbType.Int).Value = dtRes.Rows[e.RowIndex]["№"];
                var prPos = cmd.Parameters.Add("@pos", SqlDbType.Int);
                int nTmp;
                if (int.TryParse(dtRes.Rows[e.RowIndex]["Место"] as String, out nTmp))
                    prPos.Value = nTmp;
                else
                    prPos.Value = DBNull.Value;
                var prPts = cmd.Parameters.Add("@pts", SqlDbType.Float);
                var sPts = dtRes.Rows[e.RowIndex]["Балл"] == null ? null : dtRes.Rows[e.RowIndex]["Балл"].ToString();
                if (String.IsNullOrEmpty(sPts))
                    prPts.Value = 0.0;
                else
                {
                    Double dTmp;
                    sPts = sPts.Trim().Replace(',', '.');
                    if (Double.TryParse(sPts.Trim(), NumberStyles.Any, new CultureInfo("en-US"), out dTmp))
                        prPts.Value = dTmp;
                    else
                        prPts.Value = 0.0;
                }

                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения данных: " + ex.Message);
                    return;
                }

            }
            RefreshTable(false);
        }
    }
}
