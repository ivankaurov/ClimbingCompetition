// <copyright file="StartList.cs">
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
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;


namespace ClimbingCompetition
{
    /// <summary>
    /// Форма просмотра/печати/редактирования стартовых протоколов
    /// </summary>
    public partial class StartList : BaseListForm
    {
        public static void ArrangeStartNumbers(int list_id, SqlConnection cn, SqlTransaction tran)
        {
            bool preQfClimb = SettingsForm.GetPreQfClimb(cn, list_id, tran);
            SqlCommand cmd = new SqlCommand("SELECT style FROM lists(NOLOCK) WHERE iid=" + list_id.ToString(), cn);
            cmd.Transaction = tran;
            string tableName = "";
            SqlDataReader drdr = cmd.ExecuteReader();
            try
            {
                if (drdr.Read())
                {
                    switch (drdr["style"].ToString())
                    {
                        case "Трудность":
                            tableName = "routeResults";
                            break;
                        case "Боулдеринг":
                            tableName = "boulderResults";
                            break;
                        case "Скорость":
                            tableName = "speedResults";
                            break;
                        default:
                            return;
                    }
                }
            }
            finally { drdr.Close(); }
            if (tableName == "")
                return;


            cmd.CommandText = "SELECT climber_id,start FROM " + tableName + "(NOLOCK) WHERE list_id = " + list_id.ToString() + " ";
            if (!preQfClimb)
                cmd.CommandText += "AND preQf = 0 ";
            cmd.CommandText += "ORDER BY start";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.UpdateCommand = new SqlCommand("UPDATE " + tableName + " SET start=@s WHERE (climber_id=@c) " +
                "AND (list_id=" + list_id.ToString() + ")", cn);
            da.UpdateCommand.Transaction = tran;
            da.UpdateCommand.Parameters.Add("@s", SqlDbType.Int, 4, "start");
            da.UpdateCommand.Parameters.Add("@c", SqlDbType.Int, 4, "climber_id");
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["start"] = (i + 1);
            da.Update(dt);
        }

        string prevRound = "";
        //SqlConnection cn;
        int /*listID,*/ iid_parent = -1;
        string gName;
        int gr_id = -1;
        string tableName;
        string round;
        string cStyle;
        DataTable dt = new DataTable();
        DataTable dt2ndRoute = null;
        List<roundN> rounds;
        ListTypeEnum listType = ListTypeEnum.Unknown;
        //int roundCount = 0;
        int recalc2route = -1;
        bool preQfClimb;
        public StartList(int listID, SqlConnection baseCon, string competitionTitle)
            : base(baseCon, competitionTitle, listID)
        {
            try
            {
                //if (cn.State != ConnectionState.Open)
                //    cn.Open();
                InitializeComponent();
                this.preQfClimb = SettingsForm.GetPreQfClimb(this.cn, listID);
                
                //StartList.ArrangeStartNumbers(listID, cn);
                //this.listID = listID;
                //this.cn = cn;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = this.cn;
                cmd.CommandText = "SELECT g.name, lists.style, lists.round, lists.group_id, lists.iid_parent,lists.listType FROM lists(NOLOCK) INNER JOIN Groups g(NOLOCK)" +
                    " ON lists.group_id = g.iid WHERE lists.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[0].Value = this.listID;
                SqlDataReader rdr = cmd.ExecuteReader();
                
                try
                {
                    if (rdr.Read())
                    {
                        switch (rdr[1].ToString())
                        {
                            case "Трудность":
                                tableName = "routeResults";
                                break;
                            case "Боулдеринг":
                                if (rdr["round"].ToString() == "Суперфинал")
                                    goto case "Трудность";
                                tableName = "boulderResults";
                                break;
                            case "Скорость":
                                tableName = "speedResults";
                                break;
                            default:
                                rdr.Close();
                                this.Close();
                                return;
                        }
                        gName = rdr[0].ToString();
                        cStyle = rdr[1].ToString();
                        round = rdr[2].ToString();
                        this.Text = "Стартовый протокол " + gName + " " + cStyle + " " + round;
                        if (rdr["group_id"] != DBNull.Value)
                            gr_id = Convert.ToInt32(rdr["group_id"]);
                        if (rdr["iid_parent"] != DBNull.Value)
                            iid_parent = Convert.ToInt32(rdr["iid_parent"]);
                        try { listType = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), rdr["listType"].ToString(), true); }
                        catch { listType = ListTypeEnum.Unknown; }
                    }
                }
                finally { rdr.Close(); }
                cmd.CommandText = "SELECT lp.listType" +
                                  "  FROM lists l(NOLOCK)" +
                                  "  JOIN lists lp(NOLOCK) ON lp.iid = l.iid_parent" +
                                  " WHERE l.iid = " + listID.ToString();
                object obj = cmd.ExecuteScalar();
                ListTypeEnum lpt = ListTypeEnum.Unknown;
                if(obj != null && obj != DBNull.Value)
                    try { lpt = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), obj.ToString(), true); }
                    catch { lpt = ListTypeEnum.Unknown; }

                if (listType == ListTypeEnum.SpeedQualy || listType == ListTypeEnum.SpeedQualy2)
                {
                    SpeedRules rules = SettingsForm.GetSpeedRules(cn);
                    if ((rules & SpeedRules.IFSC_WR) == SpeedRules.IFSC_WR)
                    {
                        int n1, n2;
                        SpeedSystem.CalculateStartersToSet(listID, cn, out n1, out n2, out recalc2route);
                    }
                }

                tsReCreate2ndRoute.Enabled = (lpt == ListTypeEnum.LeadFlash);
                cmd.CommandText = "SELECT COUNT(*) cnt " +
                                  "  FROM syscolumns cl(NOLOCK) " +
                                  "  JOIN sysobjects t(NOLOCK) ON t.id = cl.id " +
                                  " WHERE t.name = '" + tableName + "' " +
                                  "   AND cl.name = 'preQf' ";
                try
                {
                    if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                    {
                        cmd.CommandText =
                            "ALTER TABLE " + tableName + " ADD preQf BIT NOT NULL DEFAULT 0";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                dg.DataSource = dt;
                StaticClass.BuildRoundRow(listID, this.cn, out rounds);
                cbPrev.Enabled = (rounds.Count > 1);
                RefreshData();
                if (preQfClimb)
                {
                    cbPreQf.Enabled = false;
                    cbPreQf.Checked = true;
                }
                else
                    cbPreQf.Enabled = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }

        private DataTable RefreshData()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                dt = new DataTable();
                string str;
                switch (tableName)
                {
                    case "routeResults":
                        str = "rankingLead";
                        break;
                    case "boulderResults":
                        str = "rankingBoulder";
                        break;
                    default:
                        str = "rankingSpeed";
                        break;
                }
                SqlCommand cmd = new SqlCommand();
                string sWhere, sShow;

                if (preQfClimb || cbPreQf.Checked)
                {
                    sShow = String.Format(", l.preQf [{0}] ", PREQF); ;
                    sWhere = "";
                }
                else
                {
                    sShow = "";
                    sWhere = " AND l.preQf = 0 ";
                }
                cmd.Connection = cn;
                cmd.CommandText = String.Format(
                                  "SELECT l.start AS [{0}], l.climber_id AS [{1}], (p.surname + ' ' + " +
                                         "p.name) [{2}], p.age AS [{3}], p.qf AS [{4}], " +
                                         "dbo.fn_getTeamName(p.iid, l.list_id) AS [{5}], " + str + " AS [{6}] " + sShow +
                                    "FROM " + tableName + " l(NOLOCK) " +
                                    "JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                                   "WHERE l.list_id = @listID " +
                                   sWhere +
                                   "ORDER BY {7}", START, IID, CLIMBER, AGE, QF, TEAM, RANKING, preQfClimb ? "l.start" : "l.preQf, l.start");
                cmd.Parameters.Add("@listID", SqlDbType.Int);
                cmd.Parameters[0].Value = listID;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (recalc2route > 0)
                {
                    dt2ndRoute = new DataTable();
                    da.Fill(dt2ndRoute);
                    List<object[]> dataList = new List<object[]>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        int stNum = Convert.ToInt32(dr[0]);
                        if (stNum >= recalc2route)
                            stNum = stNum - recalc2route + 1;
                        else
                            stNum = stNum + 1 + (dt.Rows.Count - recalc2route);
                        object[] obj = new object[dr.ItemArray.Length];
                        dr.ItemArray.CopyTo(obj, 0);
                        obj[0] = stNum;
                        dataList.Add(obj);
                    }
                    dataList.Sort((a, b) => Convert.ToInt32(a[0]).CompareTo(Convert.ToInt32(b[0])));
                    dt2ndRoute.Rows.Clear();
                    foreach (var a in dataList)
                        dt2ndRoute.Rows.Add(a);
                }

                prevRound = "";
                if (!cbRanking.Checked)
                    try { dt.Columns.Remove(RANKING); }
                    catch { }
                dg.DataSource = dt;
                try
                {
                    foreach (DataGridViewColumn dc in dg.Columns)
                        try { dc.ReadOnly = !(dc.Name == PREQF || dc.Name == START); }
                        catch { }
                }
                catch { }
                #region PrevRoundData
                List<roundN> rounds;
                if (cbPrev.Checked)
                {
                    StaticClass.BuildRoundRow(listID, cn, out rounds);
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@cid", SqlDbType.Int);
                    switch (tableName)
                    {
                        case "speedResults":
                            dt.Columns.Add(((roundN)rounds[1]).name.ToString(), typeof(string));
                            cmd.CommandText = "SELECT resText " +
                                                  "  FROM " + tableName + "(NOLOCK) " +
                                                  " WHERE list_id = " + ((roundN)rounds[1]).iid.ToString() +
                                                  "   AND climber_id = @cid";
                            foreach (DataRow dr in dt.Rows)
                            {
                                cmd.Parameters[0].Value = Convert.ToInt32(dr["№"]);
                                try { dr[dt.Columns.Count - 1] = cmd.ExecuteScalar().ToString(); }
                                catch { }
                            }
                            break;
                        case "routeResults":
                            if (((roundN)rounds[1]).qf)
                            {
                                prevRound = "Квалификация";
                                int routeNumber, nTmp;
                                KeyValuePair<int, bool>[] resLLL;
                                DataTable dtTmp = StaticClass.FillResFlash(((roundN)rounds[1]).iid, cn, false, out routeNumber, true, out nTmp, out resLLL);
                                for (int i = 1; i <= routeNumber; i++)
                                    dt.Columns.Add("Квал." + i.ToString(), typeof(string));
                                foreach (DataRow dr in dt.Rows)
                                    foreach (DataRow drQ in dtTmp.Rows)
                                        if (dr["№"].ToString() == drQ["№"].ToString())
                                        {
                                            for (int i = 1; i <= routeNumber; i++)
                                                dr["Квал." + i.ToString()] = drQ["Тр." + i.ToString()].ToString();
                                            break;
                                        }
                            }
                            else
                            {
                                if (((roundN)rounds[1]).TwoRoutes)
                                {
                                    prevRound = "1/4 финала";
                                    dt.Columns.Add("Трасса 1", typeof(string));
                                    dt.Columns.Add("Трасса 2", typeof(string));
                                    DataTable dtTmp = StaticClass.FillResOnsight(((roundN)rounds[1]).iid, cn, false);
                                    foreach (DataRow dr in dt.Rows)
                                        foreach (DataRow drQ in dtTmp.Rows)
                                            if (dr["№"].ToString() == drQ["№"].ToString())
                                            {
                                                dr["Трасса 1"] = drQ["Трасса 1"].ToString();
                                                dr["Трасса 2"] = drQ["Трасса 2"].ToString();
                                                break;
                                            }
                                }
                                else
                                    goto case "speedResults";
                            }
                            break;
                        case "boulderResults":
                            if (((roundN)rounds[1]).TwoRoutes)
                            {
                                prevRound = "Квалификация";
                                dt.Columns.Add("Т_грА", typeof(string));
                                dt.Columns.Add("Тп_грА", typeof(string));
                                dt.Columns.Add("Б_грА", typeof(string));
                                dt.Columns.Add("Бп_грА", typeof(string));

                                dt.Columns.Add("Т_грБ", typeof(string));
                                dt.Columns.Add("Тп_грБ", typeof(string));
                                dt.Columns.Add("Б_грБ", typeof(string));
                                dt.Columns.Add("Бп_грБ", typeof(string));
                                DataTable dtTmp = StaticClass.FillResOnsight(((roundN)rounds[1]).iid, cn, false);
                                foreach (DataRow dr in dt.Rows)
                                    foreach (DataRow drQ in dtTmp.Rows)
                                        if (dr["№"].ToString() == drQ["№"].ToString())
                                        {
                                            dr["Т_грА"] = drQ["ТА"].ToString();
                                            dr["Тп_грА"] = drQ["ПтА"].ToString();
                                            dr["Б_грА"] = drQ["БА"].ToString();
                                            dr["Бп_грА"] = drQ["ПбА"].ToString();

                                            dr["Т_грБ"] = drQ["ТБ"].ToString();
                                            dr["Тп_грБ"] = drQ["ПтБ"].ToString();
                                            dr["Б_грБ"] = drQ["ББ"].ToString();
                                            dr["Бп_грБ"] = drQ["ПбБ"].ToString();
                                            break;
                                        }
                            }
                            else
                            {
                                prevRound = ((roundN)rounds[1]).name;
                                dt.Columns.Add("Т", typeof(string));
                                dt.Columns.Add("Тп", typeof(string));
                                dt.Columns.Add("Б", typeof(string));
                                dt.Columns.Add("Бп", typeof(string));
                                cmd.CommandText = "SELECT tops, topAttempts, bonuses, bonusAttempts " +
                                                      "  FROM boulderResults(NOLOCK) " +
                                                      " WHERE list_id = " + ((roundN)rounds[1]).iid.ToString() +
                                                      "   AND climber_id = @cid";
                                foreach (DataRow dr in dt.Rows)
                                {
                                    cmd.Parameters[0].Value = Convert.ToInt32(dr["№"]);
                                    try
                                    {
                                        SqlDataReader rdr = cmd.ExecuteReader();
                                        while (rdr.Read())
                                        {
                                            dr["Т"] = rdr["tops"].ToString();
                                            dr["Тп"] = rdr["topAttempts"].ToString();
                                            dr["Б"] = rdr["bonuses"].ToString();
                                            dr["Бп"] = rdr["topAttempts"].ToString();
                                            break;
                                        }
                                        rdr.Close();
                                    }
                                    catch { }
                                }
                            }
                            break;
                    }
                #endregion

                    
                }
                //dg.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return dt;
        }


        private void btnRefreshStartNumbers_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            string sSet;
            da.UpdateCommand = new SqlCommand();
            da.UpdateCommand.Connection = cn;
            SqlTransaction tran = cn.BeginTransaction();
            try
            {
                da.UpdateCommand.Transaction = tran;
                da.UpdateCommand.Parameters.Add("@s", SqlDbType.Int, 4, START);
                da.UpdateCommand.Parameters.Add("@c", SqlDbType.Int, 4, IID);
                if (cbPreQf.Checked)
                {
                    sSet = ", preQf = @pq ";
                    da.UpdateCommand.Parameters.Add("@pq", SqlDbType.Bit, 1, PREQF);
                }
                else
                    sSet = "";
                da.UpdateCommand.CommandText = "UPDATE " + tableName + " SET start=@s" + sSet + " WHERE (climber_id=@c) AND " +
                    "(list_id=" + listID.ToString() + ")";
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                da.Update(dt);
                if (!preQfClimb)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE " + tableName +
                                                   "   SET start = 0, pos = 0, posText = '' " +
                                                   " WHERE list_id = " + listID.ToString() +
                                                   "   AND preQf = 1 ", cn);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                }
                ArrangeStartNumbers(listID, cn, tran);
                tran.Commit();
                MessageBox.Show(this, "Стартовые номера сохранены");
                try { RefreshData(); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Ошибка загрузки стартового протокола:\r\n" +
                        ex.Message);
                }
            }
            catch (Exception ex)
            {
                    try { tran.Rollback(); }
                    catch { }
                MessageBox.Show(this, "Ошибка сохранения стартовых номеров:\r\n" + ex.Message);
            }
            
        }

        private void pushFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addClimberAtPos(1);
        }

        /// <summary>
        /// Adding new climber at position
        /// </summary>
        /// <param name="pos">position</param>
        private void addClimberAtPos(int pos)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                AddClimber clm = new AddClimber(cn);
                clm.SelectedGroup = gName;
                clm.ShowDialog();
                int num = clm.Number;
                if (num == 0)
                    return;
                long iid;
                SqlCommand cmd = new SqlCommand();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                bool transactionSuccess = false;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM " + tableName + "(NOLOCK) WHERE list_id=" + listID.ToString() +
                        " AND climber_id = " + num.ToString();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show(this, "Выбранный участник уже есть в протоколе.");
                        return;
                    }
                    iid = StaticClass.GetNextIID(tableName, cn, "iid", cmd.Transaction);
                    
                    cmd.CommandText = "INSERT INTO " + tableName + "(list_id,start,climber_id,iid) VALUES " +
                        "(@listID, @pos, @num, @iid)";

                    cmd.Parameters.Add("@listID", SqlDbType.Int);
                    cmd.Parameters[0].Value = listID;

                    cmd.Parameters.Add("@pos", SqlDbType.Int);
                    cmd.Parameters[1].Value = -1;

                    cmd.Parameters.Add("@num", SqlDbType.Int);
                    cmd.Parameters[2].Value = num;

                    cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                    cmd.Parameters[3].Value = iid;

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE " + tableName + " SET start=start+1 WHERE (list_id=@listID) AND (start >= @pos)";
                    cmd.Parameters[1].Value = pos;

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE " + tableName + " SET start = @pos WHERE iid = @iid";
                    cmd.ExecuteNonQuery();

                    StartList.ArrangeStartNumbers(listID, cn, cmd.Transaction);
                    //cmd.Transaction.Commit();
                    transactionSuccess = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Ошибка добавления участника в протокол:\r\n" +
                        ex.Message);
                    transactionSuccess = false;
                }
                finally
                {
                    if (transactionSuccess)
                    {
                        cmd.Transaction.Commit();
                        MessageBox.Show(this, "Участник успешно добавлен на ст.№ " + pos.ToString());
                        try { RefreshData(); }
                        catch (Exception ex) { MessageBox.Show(this, "Ощибка загрузки данных:\r\n" + ex.Message); }
                    }
                    else
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка добавления участника в протокол:\r\n" +
                    ex.Message);
            }
        }

        private void pushBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                int pos;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT (COUNT(climber_id)+1) FROM " + tableName + " WHERE list_id=@l";
                cmd.Parameters.Add("@l", SqlDbType.Int);
                cmd.Parameters[0].Value = listID;
                try { pos = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch { pos = 1; }
                addClimberAtPos(pos);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            RefreshData();
        }

        private void addToPosToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int pos;
            try { pos = Convert.ToInt32(dg.CurrentRow.Cells[0].Value); }
            catch { MessageBox.Show("Выберите позицию"); return; }
            DialogResult dgr = MessageBox.Show("Участник будет добавлен на позицию " + pos.ToString(), "Добавление участника", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dgr == DialogResult.No)
                return;
            addClimberAtPos(pos);
        }

        private void btnDelClimber_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                int iid;
                int pos;
                string name;
                try
                {
                    pos = Convert.ToInt32(dg.CurrentRow.Cells[0].Value);
                    iid = Convert.ToInt32(dg.CurrentRow.Cells[1].Value);
                    name = dg.CurrentRow.Cells[2].Value.ToString();
                }
                catch { MessageBox.Show("Выберите позицию"); return; }
                DialogResult dgr = MessageBox.Show("Участник " + pos.ToString() + " - " + iid.ToString() + " - " + name +
                    " будет удалён из протокола.", "Удаление участника", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dgr == DialogResult.No)
                    return;
                SqlCommand cmd = new SqlCommand("DELETE FROM " + tableName + " WHERE (list_id = " +
                    "@lid) AND (climber_id = @cid)", cn);
                cmd.Parameters.Add("@lid", SqlDbType.Int);
                cmd.Parameters[0].Value = listID;
                cmd.Parameters.Add("@cid", SqlDbType.Int);
                cmd.Parameters[1].Value = iid;

                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE " + tableName + " SET start=start-1 WHERE (list_id = @lid)" +
                    " AND (start > @cid)";
                cmd.Parameters[1].Value = pos;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            RefreshData();
        }

        private void btnExcelExport_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                string sAddName = String.Empty, sAddListName = String.Empty;
                DataTable dTable = RefreshData();
                if (recalc2route > 0)
                {
                    FillListExcel(ws, ". " + ROUTE + " 1", "_ТР1", dTable);
                    ws = (Excel.Worksheet)wb.Worksheets.Add();
                    FillListExcel(ws, ". " + ROUTE + " 2", "_ТР2", dt2ndRoute);
                }
                else
                    FillListExcel(ws, String.Empty, String.Empty, dTable);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }

            RefreshData();
        }

        private void FillListExcel(Excel.Worksheet ws, string sAddName, string sAddListName, DataTable dTable)
        {
            Excel.Range dt;
            int i = 0;

            try { dTable.Columns.Remove(PREQF); }
            catch { }
            const int frst_row = 5;

            char lstC = (char)('A' + dTable.Columns.Count - 1);

            if (cbTechnical.Checked)
            {
                int iLastC;
                switch (tableName)
                {
                    case "routeResults":
                        lstC++;
                        iLastC = (int)(lstC - 'A' + 1);
                        ws.Cells[frst_row, iLastC] = RESULT;
                        break;
                    case "speedResults":
                        lstC += (char)2;
                        iLastC = (int)(lstC - 'A' + 1);
                        ws.Cells[frst_row, iLastC - 1] = ROUTE + " 1";
                        ws.Cells[frst_row, iLastC] = RESULT;
                        break;
                    case "boulderResults":
                        lstC += (char)4;
                        iLastC = (int)(lstC - 'A' + 1);
                        ws.Cells[frst_row, iLastC - 3] = "ТOP";
                        ws.Cells[frst_row, iLastC - 2] = "Поп.";
                        ws.Cells[frst_row, iLastC - 1] = "Bonus";
                        ws.Cells[frst_row, iLastC] = "Поп.";
                        break;
                }
            }
            if (prevRound == "")
            {
                foreach (DataColumn cl in dTable.Columns)
                {
                    i++;
                    /*if (cl.ColumnName == "ФамилияИмя")
                        ws.Cells[frst_row, i] = "Фамилия, Имя";
                    else*/
                    ws.Cells[frst_row, i] = cl.ColumnName;
                }
                i = frst_row;
            }
            else
            {
                int iLast = 6;
                if (dTable.Columns.Count > 6)
                    if (dTable.Columns[6].ColumnName == RANKING)
                        iLast = 7;
                for (int j = 0; j < iLast; j++)
                {
                    DataColumn cl = dTable.Columns[j];
                    dt = ws.get_Range(ws.Cells[frst_row, j + 1], ws.Cells[frst_row + 1, j + 1]);
                    dt.Merge(Type.Missing);
                    /*if (cl.ColumnName == "ФамилияИмя")
                        dt.Cells[1, 1] = "Фамилия, Имя";
                    else*/
                        dt.Cells[1, 1] = cl.ColumnName;
                }
                for (int j = iLast; j < dTable.Columns.Count; j++)
                    ws.Cells[frst_row + 1, j + 1] = dTable.Columns[j].ColumnName;
                dt = ws.get_Range(ws.Cells[frst_row, iLast + 1], ws.Cells[frst_row, dTable.Columns.Count]);
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = prevRound;

                i = frst_row + 1;
                if (cbTechnical.Checked)
                    for (char qw = (char)('A' + dTable.Columns.Count - 1); qw < lstC; qw++)
                    {
                        dt = ws.get_Range(ws.Cells[frst_row, qw - 'A' + 2], ws.Cells[i, qw - 'A' + 2]);
                        dt.Merge(Type.Missing);
                    }
            }

            foreach (DataRow rd in dTable.Rows)
            {
                i++;
                for (int k = 1; k <= dTable.Columns.Count; k++)
                    ws.Cells[i, k] = rd[k - 1];
                if ((rd["Г.р."].ToString() == "0"))
                    ws.Cells[i, 4] = "";
            }

            dt = ws.get_Range("A" + frst_row, lstC + i.ToString());
            dt.Style = "MyStyle";
            dt.Columns.AutoFit();

            dt = ws.get_Range("C" + frst_row, "C" + i.ToString());
            dt.Style = "StyleLA";

            if (cbPrintTime.Checked)
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT isolationOpen, isolationClose, observation, start, climbingTime " +
                    "  FROM lists(NOLOCK) " +
                    " WHERE iid = " + listID.ToString(), cn);
                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        i++;
                        if (rdr["isolationOpen"].ToString() != "")
                        {
                            i++;
                            ws.Cells[i, 1] = TIMEISOOPEN + ": " + rdr["isolationOpen"].ToString();
                        }
                        if (rdr["isolationClose"].ToString() != "")
                        {
                            i++;
                            ws.Cells[i, 1] = TIMEISOCLOSE + ": " + rdr["isolationClose"].ToString();
                        }
                        if (rdr["observation"].ToString() != "")
                        {
                            i++;
                            ws.Cells[i, 1] = TIMEOBSERVATION + ": " + rdr["observation"].ToString();
                        }
                        if (rdr["start"].ToString() != "")
                        {
                            i++;
                            ws.Cells[i, 1] = TIMESTART + ": " + rdr["start"].ToString();
                        }
                        if (rdr["climbingTime"].ToString() != "")
                        {
                            i++;
                            ws.Cells[i, 1] = TIMECLIMB + ": " + rdr["climbingTime"].ToString() + " " + MINUTE;
                        }
                        break;
                    }
                    rdr.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            dt = ws.get_Range("A1", lstC + "1");
            dt.Style = "CompTitle";
            dt.Merge(Type.Missing);
            string strTmp = "";
            dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
            dt.Rows.AutoFit();
            ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

            dt = ws.get_Range(lstC + "2", lstC + "2");
            dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
            dt.Style = "StyleRA";


            dt = ws.get_Range("A3", lstC + "3");
            dt.Style = "Title";
            dt.Merge(Type.Missing);
            strTmp = cStyle + ". " + gName;
            dt.Cells[1, 1] = strTmp;


            strTmp = STARTLIST + ". " + round + sAddName;
            dt = ws.get_Range("A4", lstC + "4");
            dt.Style = "Title";
            dt.Merge(Type.Missing);
            dt.Cells[1, 1] = strTmp;
            strTmp = "ст_";
            switch (cStyle)
            {
                case "Трудность":
                    strTmp += "ТР_";
                    break;
                case "Скорость":
                    strTmp += "СК_";
                    break;
                case "Боулдеринг":
                    strTmp += "Б_";
                    break;
            }
            try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, round) + sAddListName; }
            catch { }
        }

        private void cbPrev_CheckedChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void btnReCreate2ndRoute_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gr_id < 1 || this.iid_parent < 1)
                    return;

                SqlCommand cmd2 = new SqlCommand();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd2.Connection = cn;
                cmd2.CommandText = "SELECT routeNumber FROM lists(NOLOCK) WHERE iid = " + iid_parent;
                object oTm = cmd2.ExecuteScalar();
                int routeNumber;
                if (oTm != null && oTm != DBNull.Value)
                    routeNumber = Convert.ToInt32(oTm);
                else
                    throw new Exception("Проверьте введённое количество трасс для данного раунда");
                if (MessageBox.Show(this, "Вы уверены. что хотите переформировать протокол на другую трассу квалификации?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;


                cmd2.CommandText = "SELECT climber_id FROM routeResults(NOLOCK) WHERE list_id = " + listID.ToString() +
                    " AND preQf = 0 ORDER BY start ";
                List<int> frstRoute = new List<int>();
                SqlDataReader rd2 = cmd2.ExecuteReader();
                try
                {
                    while (rd2.Read())
                        frstRoute.Add(Convert.ToInt32(rd2[0]));
                }
                finally { rd2.Close(); }
                List<int> nmb = StaticClass.GetNumberSeq(round);
                if (nmb == null || nmb.Count < 1 || nmb[0] > routeNumber)
                    throw new Exception("Неправильно задан текущий раунд");
                ReCreateOtherRounds(frstRoute, nmb[0], routeNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка переформирования протокола:\r\n" + ex.Message);
            }
        }

        private void ReCreateOtherRounds(List<int> currentRound, int curRoute, int routeCount)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT LP.round" +
                              "  FROM lists L(nolock)" +
                              "  JOIN lists LP(nolock) on LP.iid = L.iid_parent" +
                              " WHERE L.iid=" + listID.ToString();
            object oTmp = cmd.ExecuteScalar();
            string roundP = (oTmp == null || oTmp == DBNull.Value) ? String.Empty : oTmp.ToString() + " ";
            if (!roundP.Equals("Квалификация ", StringComparison.InvariantCultureIgnoreCase))
                roundP += "трасса ";
            List<string> rounds = new List<string>();
            for (int i = curRoute + 1; i <= routeCount; i++)
                rounds.Add(roundP + i.ToString());
            for (int i = 1; i < curRoute; i++)
                rounds.Add(roundP + i.ToString());
            List<int>[] start = Sorting.CreateOtherRoutes(currentRound, this, routeCount);
            if (start == null)
                return;
            if (start.Length != rounds.Count + 1)
                throw new Exception("Неверные данные по трассам/раундам");
           
            SqlCommand cmdU = new SqlCommand();
            cmdU.Connection = cn;
            SqlTransaction tran = cn.BeginTransaction();
            try
            {
                cmd.Transaction = tran;
                cmdU.Transaction = tran;
                List<int> insertedRounds = new List<int>();
                for (int i = 1; i < start.Length; i++)
                {
                    string curRound = rounds[i - 1];
                    List<int> starters = start[i];

                    int cListID;
                    cmd.CommandText = "SELECT iid FROM lists(NOLOCK) WHERE iid_parent=" + iid_parent.ToString() +
                        " AND round='" + curRound + "'";
                    oTmp = cmd.ExecuteScalar();
                    if (oTmp == null || oTmp == DBNull.Value)
                    {
                        cListID = (int)StaticClass.GetNextIID("lists", cn, "iid", tran);
                        cmd.CommandText = "INSERT INTO lists(group_id, style, round, judge_id, iid, " +
                    "isolationOpen, isolationClose, observation, start, routesetter_id, projNum,iid_parent) " +
                    "VALUES(" + gr_id.ToString() + ", 'Трудность', '" + curRound + "', NULL, " + cListID.ToString() + ", " +
                    "'', '', '', '', NULL, 1, " + iid_parent.ToString() + ")";
                        cmd.ExecuteNonQuery();
                    }
                    else
                        cListID = Convert.ToInt32(oTmp);

                    insertedRounds.Add(cListID);

                    List<long> inserted = new List<long>();
                    cmd.CommandText = "SELECT iid FROM routeResults(NOLOCK) WHERE list_id=" + cListID.ToString() +
                        " AND climber_id=@cid";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@cid", SqlDbType.Int);
                    int curStart = 0;
                    foreach (int st in starters)
                    {
                        curStart++;
                        long id;
                        cmd.Parameters[0].Value = st;
                        oTmp = cmd.ExecuteScalar();
                        if (oTmp == null || oTmp == DBNull.Value)
                        {
                            id = StaticClass.GetNextIID("routeResults", cn, "iid", tran);
                            cmdU.CommandText = "INSERT INTO routeResults (list_id,start,climber_id,iid, preQf) VALUES " +
                                "(" + cListID.ToString() + ", " + curStart.ToString() + ", " + st.ToString() + ", " + id.ToString() + ", 0)";
                        }
                        else
                        {
                            id = Convert.ToInt64(oTmp);
                            cmdU.CommandText = "UPDATE routeResults SET start=" + curStart.ToString() + ", " +
                                " preQf = 0 WHERE iid = " + id.ToString();
                        }
                        inserted.Add(id);
                        cmdU.ExecuteNonQuery();
                    }

                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT climber_id FROM routeResults(NOLOCK) WHERE preQf = 1 " +
                        " AND list_id = " + listID.ToString();
                    starters.Clear();
                    SqlDataReader rd = cmd.ExecuteReader();
                    try
                    {
                        while (rd.Read())
                            starters.Add(Convert.ToInt32(rd[0]));
                    }
                    finally { rd.Close(); }

                    cmd.CommandText = "SELECT iid FROM routeResults(NOLOCK) WHERE list_id = " + cListID.ToString() +
                        " AND climber_id = @clm";
                    cmd.Parameters.Add("@clm", SqlDbType.Int);

                    foreach (int st in starters)
                    {
                        long id;
                        cmd.Parameters[0].Value = st;
                        oTmp = cmd.ExecuteScalar();
                        if (oTmp == null || oTmp == DBNull.Value)
                        {
                            id = StaticClass.GetNextIID("routeResults", cn, "iid", tran);
                            cmdU.CommandText = "INSERT INTO routeResults (list_id,start,climber_id,iid, preQf) VALUES " +
                                "(" + cListID.ToString() + ", 0, " + st.ToString() + ", " + id.ToString() + ", 1)";
                        }
                        else
                        {
                            id = Convert.ToInt64(oTmp);
                            cmdU.CommandText = "UPDATE routeResults SET start = 0," +
                                " preQf = 1 WHERE iid = " + id.ToString();
                        }
                        cmdU.ExecuteNonQuery();
                        inserted.Add(id);
                    }
                    if (inserted.Count > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM routeResults WHERE list_id = " + cListID.ToString() + " AND " +
                            " iid NOT IN (";
                        foreach (long l in inserted)
                            cmd.CommandText += l.ToString() + ",";
                        cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
                        cmd.CommandText += ")";
                        cmd.ExecuteNonQuery();
                    }
                }

                if (insertedRounds.Count > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM lists WHERE iid_parent = " + iid_parent.ToString() + " AND " +
                        " iid NOT IN (";
                    foreach (int l in insertedRounds)
                        cmd.CommandText += l.ToString() + ",";
                    cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
                    cmd.CommandText += ") AND iid <> " + listID.ToString();
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                string str = "";
                foreach (int nn in insertedRounds)
                {
                    if (str.Length > 0)
                        str += ", ";
                    str += nn.ToString();
                }
                MessageBox.Show("Были переформированы следующие протоколы (iid): " + str);
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); }
                catch { }
                throw ex;
            }
        }

        //private void ReCreateSECOND(List<int> frstRoute)
        //{
        //    string rnd2nd;
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    SqlDataReader rd;
        //    if (round == "Квалификация 1")
        //        rnd2nd = "Квалификация 2";
        //    else if (round == "Квалификация 2")
        //        rnd2nd = "Квалификация 1";
        //    else
        //        throw new Exception("Проверьте наименование раунда");


        //    List<long> inserted = new List<long>();

        //    List<int> secondRoute = Sorting.CreateSecondRoute(frstRoute, this);
        //    if (secondRoute != null)
        //    {
        //        int curStart = 0;

        //        SqlCommand cmdU = new SqlCommand();
        //        cmdU.Connection = cn;
        //        SqlTransaction tran = cn.BeginTransaction();
        //        try
        //        {
        //            cmd.Transaction = tran;
        //            cmdU.Transaction = tran;

        //            cmd.CommandText = "select l.iid " +
        //                              "  from lists l(nolock) " +
        //                              "  join lists lc(nolock) on lc.iid_parent = l.iid_parent and lc.iid <> l.iid " +
        //                              " where lc.iid = " + listID.ToString();
        //            object oTmp = cmd.ExecuteScalar();
        //            int l2id;
        //            if (oTmp != null && oTmp != DBNull.Value)
        //                l2id = Convert.ToInt32(oTmp);
        //            else
        //            {
        //                l2id = (int)StaticClass.GetNextIID("lists", cn, "iid", tran);
        //                cmd.CommandText = "INSERT INTO lists(group_id, style, round, judge_id, iid, " +
        //            "isolationOpen, isolationClose, observation, start, routesetter_id, projNum,iid_parent) " +
        //            "VALUES(" + gr_id.ToString() + ", 'Трудность', '" + rnd2nd + "', NULL, " + l2id.ToString() + ", " +
        //            "'', '', '', '', NULL, 1, " + iid_parent.ToString() + ")";
        //                cmd.ExecuteNonQuery();

        //            }

        //            cmd.CommandText = "SELECT iid FROM routeResults(NOLOCK) WHERE list_id = " + l2id.ToString() +
        //                " AND climber_id = @clm";
        //            cmd.Parameters.Add("@clm", SqlDbType.Int);

        //            foreach (int i in secondRoute)
        //            {
        //                curStart++;
        //                long id;
        //                cmd.Parameters[0].Value = i;
        //                oTmp = cmd.ExecuteScalar();
        //                if (oTmp == null || oTmp == DBNull.Value)
        //                {
        //                    id = StaticClass.GetNextIID("routeResults", cn, "iid", tran);
        //                    cmdU.CommandText = "INSERT INTO routeResults (list_id,start,climber_id,iid, preQf) VALUES " +
        //                        "(" + l2id.ToString() + ", " + curStart.ToString() + ", " + i.ToString() + ", " + id.ToString() + ", 0)";
        //                }
        //                else
        //                {
        //                    id = Convert.ToInt64(oTmp);
        //                    cmdU.CommandText = "UPDATE routeResults SET start=" + curStart.ToString() + ", " +
        //                        " preQf = 0 WHERE iid = " + id.ToString();
        //                }
        //                cmdU.ExecuteNonQuery();
        //                inserted.Add(id);
        //            }

        //            cmd.Parameters.Clear();
        //            cmd.CommandText = "SELECT climber_id FROM routeResults(NOLOCK) WHERE preQf = 1 " +
        //                " AND list_id = " + listID.ToString();
        //            frstRoute.Clear();
        //            rd = cmd.ExecuteReader();
        //            try
        //            {
        //                while (rd.Read())
        //                    frstRoute.Add(Convert.ToInt32(rd[0]));
        //            }
        //            finally { rd.Close(); }

        //            cmd.CommandText = "SELECT iid FROM routeResults(NOLOCK) WHERE list_id = " + l2id.ToString() +
        //                " AND climber_id = @clm";
        //            cmd.Parameters.Add("@clm", SqlDbType.Int);

        //            foreach (int i in frstRoute)
        //            {
        //                long id;
        //                cmd.Parameters[0].Value = i;
        //                oTmp = cmd.ExecuteScalar();
        //                if (oTmp == null || oTmp == DBNull.Value)
        //                {
        //                    id = StaticClass.GetNextIID("routeResults", cn, "iid", tran);
        //                    cmdU.CommandText = "INSERT INTO routeResults (list_id,start,climber_id,iid, preQf) VALUES " +
        //                        "(" + l2id.ToString() + ", 0, " + i.ToString() + ", " + id.ToString() + ", 1)";
        //                }
        //                else
        //                {
        //                    id = Convert.ToInt64(oTmp);
        //                    cmdU.CommandText = "UPDATE routeResults SET start = 0," +
        //                        " preQf = 1 WHERE iid = " + id.ToString();
        //                }
        //                cmdU.ExecuteNonQuery();
        //                inserted.Add(id);
        //            }
        //            if (inserted.Count > 0)
        //            {
        //                cmd.Parameters.Clear();
        //                cmd.CommandText = "DELETE FROM routeResults WHERE list_id = " + l2id.ToString() + " AND " +
        //                    " iid NOT IN (";
        //                foreach (long l in inserted)
        //                    cmd.CommandText += l.ToString() + ",";
        //                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
        //                cmd.CommandText += ")";
        //                int ijk = cmd.ExecuteNonQuery();
        //            }
        //            tran.Commit();
        //            MessageBox.Show("Протокол перформирован. iid=" + l2id.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            try { tran.Rollback(); }
        //            catch { }
        //            throw ex;
        //        }
        //    }
        //}

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }

    class PrintOrdinaryList : PrintList
    {
        string competitionTitle, group, round, typeOfList, style, judge;
        public PrintOrdinaryList(DataTable dt, string compTitle, string style, 
            string group, string round, string typeOfList, string judge)
            : base(dt)
        {
            competitionTitle = compTitle;
            this.group = group;
            this.round = round;
            this.typeOfList = typeOfList;
            this.style = style;
            this.judge = judge;
        }
        protected override float PrintHeader(Font f, ref PrintPageEventArgs ev)
        {
            Font fTtl = new Font(f.FontFamily, 14f, FontStyle.Bold);
            float fontH = fTtl.GetHeight(ev.Graphics);

            float xPos = ev.MarginBounds.Left;
            foreach (int i in columns)
                xPos += f.SizeInPoints * (float)i;

            string ttl = StaticClass.ParseCompTitle(competitionTitle)[0];
            float ttlSize = fTtl.SizeInPoints * (float)ttl.Length;
            float yPos = ev.MarginBounds.Top;

            float xSt = (xPos + ev.MarginBounds.Left - ttlSize) / 2;
            if (xSt < ev.MarginBounds.Left)
                xSt = ev.MarginBounds.Left;

            ev.Graphics.DrawString(ttl, fTtl, Brushes.Black, xSt, yPos);
            yPos += fontH;
            ev.Graphics.DrawString(
                StaticClass.ParseCompTitle(competitionTitle)[1], f, Brushes.Black, ev.MarginBounds.Left, yPos);

            ttl = StaticClass.ParseCompTitle(competitionTitle)[2];
            ttlSize = f.SizeInPoints * (float)(ttl.Length);

            ev.Graphics.DrawString(ttl, f, Brushes.Black, xPos - ttlSize, yPos);
            fontH = f.GetHeight(ev.Graphics);
            yPos += fontH;

            fTtl = new Font(f, FontStyle.Bold);
            ttl = typeOfList + ". " + round;
            ttlSize = f.SizeInPoints*(float)(ttl.Length);
            xSt = (xPos + ev.MarginBounds.Left - ttlSize) / 2;
            if (xSt < ev.MarginBounds.Left)
                xSt = ev.MarginBounds.Left;

            ev.Graphics.DrawString(ttl, fTtl, Brushes.Black, xSt, yPos);
            yPos += fontH;

            ttl = style + ". " + group;
            ttlSize = f.SizeInPoints * (float)(ttl.Length);
            xSt = (xPos + ev.MarginBounds.Left - ttlSize) / 2;
            if (xSt < ev.MarginBounds.Left)
                xSt = ev.MarginBounds.Left;
            ev.Graphics.DrawString(ttl, fTtl, Brushes.Black, xSt, yPos);
            yPos += fontH;

            ev.Graphics.DrawString("Зам гл. судьи по виду: " + judge, f, Brushes.Black, 
                ev.MarginBounds.Left, yPos);

            return PrintColumnsHeaders(yPos + fontH, f, ref ev);
        }
    }
}
