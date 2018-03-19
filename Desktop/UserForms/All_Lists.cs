// <copyright file="All_Lists.cs">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using XmlApiData;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� - ������ ���� ����������
    /// </summary>
    public partial class All_Lists : BaseForm
    {
        //SqlConnection cn;
        const string NewList = "NEW_LIST";
        string routeNumberOld = NewList;
        string path;
        bool allowEvent;
        DataTable dt;
        SqlDataAdapter da;
        List<string> Rounds = new List<string>();

        bool AllowSecondEvent = false, changed = false;

        private void RepairDatabase()
        {
            SqlCommand cmd = new SqlCommand(), cmdU = new SqlCommand();
            cmd.Connection = cmdU.Connection = cn;

            cmd.CommandText = "SELECT DISTINCT L.iid" +
                              "  FROM lists L(nolock)" +
                              "  JOIN lists LC(nolock) on LC.iid_parent = L.iid" +
                              " WHERE LC.group_id <> L.group_id";
            List<int> brokenLists = new List<int>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    brokenLists.Add(Convert.ToInt32(rdr["iid"]));
            }
            if (brokenLists.Count < 1)
                return;
            cmd.CommandText = "SELECT L.group_id, COUNT(*) cnt" +
                              "  FROM lists L(nolock)" +
                              " WHERE L.iid = @iid" +
                              "    OR L.iid_parent = @iid "+
                            "GROUP BY L.group_id "+
                            "ORDER BY cnt DESC";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmdU.CommandText = "UPDATE lists" +
                               "   SET group_id = @grp" +
                               " WHERE iid = @iid" +
                               "    OR iid_parent = @iid";
            cmdU.Parameters.Add("@iid", SqlDbType.Int);
            cmdU.Parameters.Add("@grp", SqlDbType.Int);
            foreach (int n in brokenLists)
            {
                cmd.Parameters[0].Value = n;
                int groupId;
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        groupId = Convert.ToInt32(rdr[0]);
                    else
                        continue;
                }
                cmdU.Parameters[0].Value = n;
                cmdU.Parameters[1].Value = groupId;
                cmdU.ExecuteNonQuery();
            }
        }

        public All_Lists(SqlConnection baseCon, string path, string competitionTitle)
            : base(baseCon, competitionTitle)
        {
            //if (cn.State != ConnectionState.Open)
            //    cn.Open();
            Judges.CheckTable(cn);
            SortingClass.CheckColumn("lists", "projNum", "INT NOT NULL DEFAULT 1", cn);

            SortingClass.CheckColumn("lists", "listType", "VARCHAR(255) NOT NULL DEFAULT '" + ListTypeEnum.Unknown + "'", cn);
            RecalculateLists(cn, false, null);

            AccountForm.CreateTeamSecondLinkTable(cn, null);

            AccountForm.CreateTeamFunction(cn);

            StaticClass.CheckQualyTrigger(cn);
            StaticClass.CheckSpeedFunctions(cn);

            RepairDatabase();

            Rounds.Add("������������");
            Rounds.Add("������������ 1");
            Rounds.Add("������������ 2");
            Rounds.Add("������������ (2 ������)");
            Rounds.Add("������������ ������ �");
            Rounds.Add("������������ ������ �");
            Rounds.Add("1/8 ������");
            Rounds.Add("1/4 ������");
            Rounds.Add("1/4 ������ (2 ������)");
            Rounds.Add("1/4 ������ ������ 1");
            Rounds.Add("1/4 ������ ������ 2");
            Rounds.Add("1/2 ������");
            Rounds.Add("1/2 ������ (2 ������)");
            Rounds.Add("�����");
            Rounds.Add("����������");
            Rounds.Add("�������� ��������");
            InitializeComponent();
#if !FULL
            while (cbStyle.Items.Count > 1)
                cbStyle.Items.RemoveAt(1);
            cbStyle.Items.Add("��������");
            cbStyle.Items.Add("���������");
#endif
            viewStartList.Visible = btnEdit.Visible = btnAdd.Visible = btnDel.Visible = btnSubmitChanges.Visible = (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY);

            foreach (string str in Rounds)
                cbRound.Items.Add(str);

            this.path = path;
            //this.cn = cn;
            SqlCommand cmd = new SqlCommand("SELECT name FROM groups ORDER BY name", cn);
            SqlDataReader rd = cmd.ExecuteReader();
            cbGroup.Items.Add("-��� ������-");
            while (rd.Read())
                cbGroup.Items.Add(rd[0].ToString());
            rd.Close();
            cmd.CommandText = "UPDATE lists SET round='������������',routeNumber=2 WHERE round='������������ (2 ������)' AND style='���������'";
            cmd.ExecuteNonQuery();
            cbStyle.SelectedIndex = 0;
            SetEditMode(false);
            da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand();
            da.UpdateCommand = new SqlCommand();
            da.SelectCommand.Connection = da.UpdateCommand.Connection = cn;
            da.SelectCommand.Parameters.Add("@style", SqlDbType.VarChar, 50);
            da.SelectCommand.Parameters.Add("@gname", SqlDbType.VarChar, 50);
            da.UpdateCommand.CommandText = "UPDATE lists SET allowView=@aw, workFinished=@wf, quote=@q, " +
                "climbingTime=@ct, routeNumber=@rn, showPhoto=@ph WHERE iid=@iid";
            da.UpdateCommand.Parameters.Add("@aw", SqlDbType.Bit, 1, "��������");
            da.UpdateCommand.Parameters.Add("@wf", SqlDbType.Bit, 1, "���������");
            da.UpdateCommand.Parameters.Add("@q", SqlDbType.Int, 4, "�����");
            da.UpdateCommand.Parameters.Add("@ct", SqlDbType.Int, 4, "����� �� ������");
            da.UpdateCommand.Parameters.Add("@rn", SqlDbType.Int, 4, "���-�� �����");
            da.UpdateCommand.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
            da.UpdateCommand.Parameters.Add("@ph", SqlDbType.Bit, 1, "����� ����");
            dt = new DataTable();
            dgLists.DataSource = dt;

            ComboBox cb = cbJudge;
            cb.Items.Add("���. �� ����");
            string cmdText =  "  SELECT iid, name, surname, patronimic, category cat " +
                              "    FROM judgeView(NOLOCK) " +
                              "   WHERE pos = '���. ��.����� �� ����' " +
                              "ORDER BY name";

            FillJudgesData(cmd, cb, cmdText);

            cb = cbRouteSetter;
            cb.Items.Add("��������� ������");
            cmdText = "SELECT iid, name, surname, patronimic, category cat " +
                      "  FROM judgeView(NOLOCK) " +
                      " WHERE pos = '���. ��.����� �� �������' " +
                      "    OR pos = '����������� �����' " +
                    "ORDER BY name";

            FillJudgesData(cmd, cb, cmdText);
            try
            {
                SortingClass.CheckColumn("lists", "iid_parent", "INT", this.cn);
            }
            catch { }
#if FULL
            cmd.CommandText = "UPDATE lists SET style = '��������� - ��' WHERE style = '���������'";
#else
            cmd.CommandText = "UPDATE lists SET style = '���������' WHERE style = '��������� - ��'";
#endif
            try { cmd.ExecuteNonQuery(); }
            catch { }

            cmd.CommandText = "SELECT TOP 1 nowClimbing3 FROM lists(NOLOCK) ";
            try { cmd.ExecuteScalar(); }
            catch (SqlException) {
                cmd.CommandText = "ALTER TABLE lists ADD nowClimbing3 INT NULL";
                cmd.ExecuteNonQuery();
            }

            RefreshData();

            SortingClass.CheckColumn("lists", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("routeResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("speedResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("boulderResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("Participants", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            //SortingClass.CheckColumn("lists", "usePts", "BIT NOT NULL DEFAULT 1", this.cn);
            //SortingClass.CheckColumn("lists", "restrictPoints", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("Judges", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("positions", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("JudgePos", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            StaticClass.CheckChangedTriggers(this.cn);
            MultiRouteBoulder.CheckTableExists(this.cn);

            try { dgLists.ReadOnly = (StaticClass.currentAccount != StaticClass.AccountInfo.SECRETARY); }
            catch { }
#if !FULL
            lblRouteNumber.Visible = tbRouteNumber.Visible = false;
#endif
        }

        sealed class ListToRecalc
        {
            public int Iid { get; private set; }
            public string Round { get; private set; }
            public string Style { get; private set; }
            public ListToRecalc(int iid, string style, string round)
            {
                this.Iid = iid;
                this.Style = style;
                this.Round = round;
            }
        }

        public static void RecalculateLists(SqlConnection cn, bool recalculateAll, SqlTransaction tranP)
        {
            SqlTransaction tran;
            if (tranP == null)
                tran = cn.BeginTransaction();
            else
                tran = tranP;
            try
            {
                List<ListToRecalc> lists = new List<ListToRecalc>();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT iid, style, round FROM lists(NOLOCK)";
                if (!recalculateAll)
                    cmd.CommandText += " WHERE listType = '" + ListTypeEnum.Unknown.ToString() + "'";
                cmd.Transaction = tran;
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                    {
                        int locIid = Convert.ToInt32(rdr["iid"]);

                        string locStyle = String.Empty;
                        if (rdr["style"] != null && rdr["style"] != DBNull.Value)
                            locStyle = rdr["style"].ToString().Trim().ToLower();

                        string locRound = String.Empty;
                        if (rdr["round"] != null && rdr["round"] != DBNull.Value)
                            locRound = rdr["round"].ToString().Trim().ToLower();

                        lists.Add(new ListToRecalc(locIid, locStyle, locRound));
                    }
                }
                finally { rdr.Close(); }
                cmd.CommandText = "UPDATE lists SET listType = @lt WHERE iid = @iid";
                cmd.Parameters.Add("@lt", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                
                foreach (var l in lists)
                {
                    ListTypeEnum listType;
                    cmd.Parameters[1].Value=l.Iid;
                    switch (l.Style)
                    {
                        case "���������":
                            switch (l.Round)
                            {
                                case "������������":
                                    listType = ListTypeEnum.LeadFlash;
                                    break;
                                case "�������� ��������":
                                    listType = ListTypeEnum.General;
                                    break;
                                default:
                                    if (l.Round.IndexOf("(") > 0)
                                        listType = ListTypeEnum.LeadGroups;
                                    else
                                        listType = ListTypeEnum.LeadSimple;
                                    break;
                            }
                            break;
                        case "��������":
                            switch (l.Round)
                            {
                                case "������������":
                                    listType = ListTypeEnum.SpeedQualy;
                                    break;
                                case "������������ 2":
                                    listType = ListTypeEnum.SpeedQualy2;
                                    break;
                                case "�������� ��������":
                                    listType = ListTypeEnum.General;
                                    break;
                                default:
                                    listType = ListTypeEnum.SpeedFinal;
                                    break;
                            }
                            break;
                        case "����������":
                            switch (l.Round)
                            {
                                case "�������� ��������":
                                    listType = ListTypeEnum.General;
                                    break;
                                case "����������":
                                    listType = ListTypeEnum.BoulderSuper;
                                    break;
                                default:
                                    if (l.Round.IndexOf("(") > 0)
                                        listType = ListTypeEnum.BoulderGroups;
                                    else
                                        listType = ListTypeEnum.BoulderSimple;
                                    break;
                            }
                            break;
                        case "����������":
                            listType = ListTypeEnum.Combined;
                            break;
                        case "���������":
                            goto case "����������";
                        default:
                            if (l.Style.IndexOf("��") > 0)
                                listType = ListTypeEnum.TeamLead;
                            else if (l.Style.IndexOf("��") > 0)
                                listType = ListTypeEnum.TeamSpeed;
                            else if (l.Style.IndexOf("(�") > 0)
                                listType = ListTypeEnum.TeamBoulder;
                            else if (l.Style.IndexOf("��") > 0)
                                listType = ListTypeEnum.TeamGeneral;
                            else
                                listType = ListTypeEnum.Unknown;
                            break;
                    }
                    cmd.Parameters[0].Value = listType.ToString();
                    cmd.ExecuteNonQuery();
                }
                if (tranP == null)
                    tran.Commit();
            }
            catch (Exception ex)
            {
                if(tranP == null)
                    try { tran.Rollback(); }
                    catch { }
                throw ex;
            }
        }

        private void FillJudgesData(SqlCommand cmd, ComboBox cb, string cmdText)
        {
            cmd.CommandText = cmdText;
            cb.Items.Clear();
            
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                string strTmp = rd["surname"].ToString();
                if (rd["name"].ToString() != "")
                {
                    strTmp += " " + rd["name"].ToString()[0] + ".";
                    if (rd["patronimic"].ToString() != "")
                        strTmp += rd["patronimic"].ToString()[0] + ".";
                }
                strTmp += " (";
                if (rd["cat"].ToString() != "")
                    strTmp += rd["cat"].ToString() + "; ";
                strTmp += "iid = " + rd["iid"].ToString() + ")";
                cb.Items.Add(strTmp);
            }
            rd.Close();
        }

        private void RefreshData() { RefreshData(null); }

        private void RefreshData(SqlTransaction tran)
        {
            if (allowEvent)
            {
                allowEvent = false;
                if (cbStyle.SelectedIndex < 0)
                    cbStyle.SelectedIndex = 0;
                if (cbGroup.SelectedIndex < 0)
                    cbGroup.SelectedIndex = 0;
                string sWhere;
#if FULL
                sWhere = " WHERE 1=1 ";
#else
                sWhere = " WHERE l.style IN ('��������', '���������') ";
#endif
                SqlTransaction prevTr = da.SelectCommand.Transaction;
                try
                {
                    da.SelectCommand.Transaction = tran;
                    da.SelectCommand.Parameters[0].Value = cbStyle.Text;
                    if (cbGroup.SelectedIndex == 0)
                    {
                        da.SelectCommand.CommandText = "SELECT l.iid, l.style AS ���, g.name AS ������, " +
                            "l.round AS �����, l.quote AS �����, l.climbingTime AS [����� �� ������], " +
                            "l.routeNumber AS [���-�� �����], l.allowView AS ��������, l.showPhoto AS [����� ����], " +
                            "l.workFinished AS ���������, l.listType ltp FROM lists l(NOLOCK) LEFT JOIN Groups g ON l.group_id=g.iid " + sWhere;
                        if (cbStyle.SelectedIndex != 0)
                            da.SelectCommand.CommandText += " AND (l.style=@style) ";
                        da.SelectCommand.CommandText += "ORDER BY l.workFinished, l.style, l.round, g.name";
                    }
                    else
                    {
                        da.SelectCommand.CommandText = "SELECT l.iid, l.style AS ���, g.name AS ������, " +
                            "l.round AS �����, l.quote AS �����, l.climbingTime AS [����� �� ������], " +
                            "l.routeNumber AS [���-�� �����], l.allowView AS ��������, l.showPhoto AS [����� ����], " +
                            "l.workFinished AS ���������, l.listType ltp FROM lists l LEFT JOIN Groups g ON l.group_id=g.iid " + sWhere;
                        if (cbStyle.SelectedIndex != 0)
                            da.SelectCommand.CommandText += "AND (l.style=@style) ";
                        da.SelectCommand.CommandText += " AND (g.name=@gname) ORDER BY l.workFinished, l.style, l.round, g.name";
                    }
                    da.SelectCommand.Parameters[1].Value = cbGroup.SelectedItem.ToString();
                    dt.Clear();
                    dt.Columns.Clear();
                    da.Fill(dt);
                    dt.Columns.Add("ListType", typeof(ListTypeEnum));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["�����"].ToString() == "-1")
                            dt.Rows[i]["�����"] = DBNull.Value;
                        ListTypeEnum ltp;
                        try { ltp = (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), dt.Rows[i]["ltp"].ToString(), true); }
                        catch { ltp = ListTypeEnum.Unknown; }
                        dt.Rows[i]["ListType"] = ltp;
                    }
                    dt.Columns.Remove("ltp");
                    dgLists.DataSource = dt;
                    for (int i = 0; i < 4; i++)
                        dgLists.Columns[i].ReadOnly = true;
                    try { dgLists.Columns["���-�� �����"].ReadOnly = true; }
                    catch { }
                    try { dgLists.Columns["ListType"].Visible = false; }
                    catch { }
#if !FULL
                try
                {
                    dgLists.Columns["����� �� ������"].Visible = dgLists.Columns["���-�� �����"].Visible = false;
                }
                catch { }
#endif
                }
                finally { da.SelectCommand.Transaction = prevTr; }
                allowEvent = true;
            }
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tbRoutes.Enabled = cbStyle.Enabled && (cbRound.SelectedItem.ToString() == "����������");
            if (allowEvent)
                RefreshData();
            if(AllowSecondEvent)
            {
                cbRound.Items.Clear();
                switch (cbStyle.Text)
                {
#if FULL
                    case "���������":
                        cbRound.Items.Add("������������");
                        cbRound.Items.Add("1/4 ������ (2 ������)");
                        cbRound.Items.Add("1/4 ������");
                        cbRound.Items.Add("1/2 ������");
                        cbRound.Items.Add("�����");
                        cbRound.Items.Add("����������");
                        break;
                    case "����������":
                        cbRound.Items.Add("������������ (2 ������)");
                        cbRound.Items.Add("1/2 ������");
                        cbRound.Items.Add("�����");
                        cbRound.Items.Add("����������");
                        break;
#endif
                    case "��������":
                        cbRound.Items.Add("������������");
                        break;

                    case "����������":
                        cbRound.Items.Add("������������");
                        cbRound.Items.Add("�����");
                        break;
                }
                cbRound.Items.Add("�������� ��������");
            }
        }

        private void cbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void ClearData()
        {
            cbGroup.SelectedIndex = -1;
            cbGroup.Text = "������";
            cbRound.SelectedIndex = -1;
            cbRound.Text = "�����";
            cbJudge.Text = "���. �� ����";
            cbRouteSetter.Text = "��������� ������";
            cbProjNum.SelectedIndex = 0;
            tbIsoClose.Text = tbIsoOpen.Text = tbObserv.Text = tbStart.Text = "";
            tbRouteNumber.Text = "";
            tbIid.Text = String.Empty;
            routeNumberOld = NewList;
        }

        private void SetEditMode(bool edit, bool addNew = false)
        {
            allowEvent = btnAdd.Enabled = viewStartList.Enabled = viewResultList.Enabled =
                btnSubmitChanges.Enabled = dgLists.Enabled = !edit;
            AllowSecondEvent = cbRound.Enabled = cbJudge.Enabled = tbIsoClose.Enabled = tbIsoOpen.Enabled =
                tbObserv.Enabled = tbStart.Enabled = cbRouteSetter.Enabled = cbProjNum.Enabled =
                tbRouteNumber.Enabled = edit;
            if (edit)
            {
                routeNumberOld = tbRouteNumber.Text;
                if (addNew)
                {
                    cbRound.Items.Clear();
                    tbIsoOpen.Text = tbIsoClose.Text = tbObserv.Text = tbStart.Text = "";
                }
                btnEdit.Text = "�����������";
                btnDel.Text = "������";
                cbGroup.Items.RemoveAt(0);
                if (addNew)
                {
                    cbJudge.SelectedIndex = cbJudge.Items.IndexOf("���. �� ����");
                    cbRouteSetter.SelectedIndex = cbRouteSetter.Items.IndexOf("��������� ������");
                }
            }
            else
            {
                btnEdit.Text = "������";
                btnDel.Text = "�������";
                try
                {
                    if (cbGroup.Items[0].ToString() != "-��� ������-")
                        cbGroup.Items.Insert(0, "-��� ������-");
                }
                catch { cbGroup.Items.Insert(0, "-��� ������-"); }
                cbStyle.Enabled = cbGroup.Enabled = true;
                cbRound.Items.Clear();
                foreach (string str in Rounds)
                    cbRound.Items.Add(str);
            }
        }
        private static void setCbItem(ComboBox cb, string item)
        {
            int n = cb.Items.IndexOf(item);
            if (n >= 0)
                cb.SelectedIndex = n;
            else
                cb.Text = item;
        }

        ListTypeEnum selectedListType = ListTypeEnum.Unknown;

        private void dg_lead_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!allowEvent)
                    return;
                allowEvent = false;
                int iid;
                try
                {
                    iid = Convert.ToInt32(dgLists.CurrentRow.Cells[0].Value);
                    selectedListType = (ListTypeEnum)dgLists.CurrentRow.Cells["ListType"].Value;
                }
                catch
                {
                    allowEvent = true;
                    return;
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "   SELECT p.style, p.round, g.name, p.quote, p.allowView, " +
                                  "          p.workFinished, p.climbingTime, p.routeNumber, j.name jname, j.surname, j.patronimic, j.iid jiid, " +
                                  "          p.isolationOpen io, p.isolationClose ic, p.observation ob, p.start st, " +
                                  "          j.category jcat, jr.name rname, jr.surname rsurname, jr.patronimic rpatronimic, " +
                                  "          jr.iid riid, jr.category rcat, p.projNum, p.routeNumber " +
                                  "     FROM lists p(NOLOCK) " +
                                  "LEFT JOIN Groups g(NOLOCK) ON p.group_id = g.iid " +
                                  "LEFT JOIN judgeView j(NOLOCK) ON p.judge_id = j.iid " +
                                  "LEFT JOIN judgeView jr(NOLOCK) ON p.routesetter_id = jr.iid " +
                                  "    WHERE p.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[0].Value = iid;
                SqlDataReader rd = cmd.ExecuteReader();
                try
                {
                    if (rd.Read())
                    {
                        string style = rd["style"].ToString();
                        string group = rd["name"].ToString();
                        string round = rd["round"].ToString();
                        setCbItem(cbStyle, style);
                        setCbItem(cbGroup, group);
                        setCbItem(cbRound, round);
                        bool startListEnabled;
                        startListEnabled = (style == "���������" || style == "��������" || style == "����������");
                        startListEnabled = startListEnabled && round != "�������� ��������" && round != "������������ (2 ������)" &&
                            round != "1/4 ������ (2 ������)";
                        if (style == "���������" && round == "������������")
                            startListEnabled = false;
                        viewStartList.Enabled = startListEnabled;
                        tbIid.Text = iid.ToString();

                        string strTmp = "";
                        if (rd["surname"].ToString() == "")
                            cbJudge.SelectedIndex = cbJudge.Items.IndexOf("���. �� ����");
                        else
                        {
                            strTmp = rd["surname"].ToString();
                            if (rd["jname"].ToString() != "")
                            {
                                strTmp += " " + rd["jname"].ToString()[0] + ".";
                                if (rd["patronimic"].ToString() != "")
                                    strTmp += rd["patronimic"].ToString()[0] + ".";
                            }
                            strTmp += " (";
                            if (rd["jcat"].ToString() != "")
                                strTmp += rd["jcat"].ToString() + "; ";
                            strTmp += "iid = " + rd["jiid"].ToString() + ")";

                            cbJudge.SelectedIndex = cbJudge.Items.IndexOf(strTmp);
                        }

                        strTmp = "";
                        if (rd["rsurname"].ToString() == "")
                            cbRouteSetter.SelectedIndex = cbRouteSetter.Items.IndexOf("��������� ������");
                        else
                        {
                            strTmp = rd["rsurname"].ToString();
                            if (rd["rname"].ToString() != "")
                            {
                                strTmp += " " + rd["rname"].ToString()[0] + ".";
                                if (rd["rpatronimic"].ToString() != "")
                                    strTmp += rd["rpatronimic"].ToString()[0] + ".";
                            }
                            strTmp += " (";
                            if (rd["rcat"].ToString() != "")
                                strTmp += rd["rcat"].ToString() + "; ";
                            strTmp += "iid = " + rd["riid"].ToString() + ")";

                            cbRouteSetter.SelectedIndex = cbRouteSetter.Items.IndexOf(strTmp);
                        }

                        tbIsoClose.Text = rd["ic"].ToString();
                        tbIsoOpen.Text = rd["io"].ToString();
                        tbObserv.Text = rd["ob"].ToString();
                        tbStart.Text = rd["st"].ToString();

                        try { cbProjNum.SelectedIndex = cbProjNum.Items.IndexOf(rd["projNum"].ToString()); }
                        catch { cbProjNum.SelectedIndex = 0; }

                        if (rd["routeNumber"] != null && rd["routeNumber"] != DBNull.Value)
                            tbRouteNumber.Text = rd["routeNumber"].ToString();
                        else
                            tbRouteNumber.Text = "";
                    }
                }
                finally { rd.Close(); }
                allowEvent = true;
            }
            catch { }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SetEditMode(true, true);
            ClearData();
        }

        private int GetIID(string str)
        {
            int r = -1;
            int i = str.IndexOf('(');
            if (i >= 0)
            {
                i = str.IndexOf("= ");
                if (i >= 0)
                {
                    int k = str.IndexOf(')', i);
                    if (k > -1)
                    {
                        try
                        {
                            string strt = str.Substring(i + 2, k - i - 2);
                            r = int.Parse(strt);
                        }
                        catch { }
                    }
                }
            }
            return r;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "������")
            {
                if (String.IsNullOrEmpty(tbIid.Text))
                {
                    MessageBox.Show("�������� �� ������");
                    return;
                }
                SetEditMode(true, false);
                cbStyle.Enabled = cbGroup.Enabled = cbRound.Enabled = false;
                return;
            }
#if !FULL
            if (!(cbStyle.Text == "���������" || cbStyle.Text == "��������"))
            {
                MessageBox.Show(this, "��������� ��� �� �������� � ������������ ������ ���������.\r\n" +
                    "����������, ����������� ������ ������.", "�������� ������", MessageBoxButtons.OK,
                     MessageBoxIcon.Information);
                return;
            }
#endif
            bool addNew = String.IsNullOrEmpty(tbIid.Text);
            ListTypeEnum typeToSet = ListTypeEnum.Unknown;
            if (cbStyle.Text == "����������" || cbStyle.Text.IndexOf("������") > -1)
            {
                if (cbStyle.Text == "����������")
                    typeToSet = ListTypeEnum.Combined;
                else if (cbStyle.Text.ToLower().IndexOf("��") > 0)
                    typeToSet = ListTypeEnum.TeamGeneral;
                else if (cbStyle.Text.ToLower().IndexOf("��") > 0)
                    typeToSet = ListTypeEnum.TeamLead;
                else if (cbStyle.Text.ToLower().IndexOf("��") > 0)
                    typeToSet = ListTypeEnum.TeamSpeed;
                else
                    typeToSet = ListTypeEnum.TeamBoulder;
                cbRound.SelectedIndex = cbRound.Items.IndexOf("�������� ��������");
                if (cbStyle.Text.IndexOf("������") > -1)
                    cbGroup.SelectedIndex = 0;
            }
            if (addNew)
                if (cbRound.SelectedIndex == -1)
                {
                    MessageBox.Show("����� �� ������");
                    return;
                }
            if (typeToSet == ListTypeEnum.Unknown)
            {
                string roundT = cbRound.Text.Trim().ToLower();
                if (roundT == "�������� ��������")
                    typeToSet = ListTypeEnum.General;
                else
                    switch (cbStyle.Text.ToLower().Trim())
                    {
                        case "���������":
                            if (roundT == "������������")
                                typeToSet = ListTypeEnum.LeadFlash;
                            else if (roundT.IndexOf("(") > 0)
                                typeToSet = ListTypeEnum.LeadGroups;
                            else
                                typeToSet = ListTypeEnum.LeadSimple;
                            break;
                        case "��������":
                            typeToSet = ListTypeEnum.SpeedQualy;
                            break;
                        case "����������":
                            if (roundT.IndexOf("(") > 0)
                                typeToSet = ListTypeEnum.BoulderGroups;
                            else
                                typeToSet = ListTypeEnum.BoulderSimple;
                            break;
                    }
            }
            if (cbGroup.SelectedIndex == -1)
            {
                MessageBox.Show("������ �� �������");
                return;
            }
            if (cbStyle.Text.IndexOf("����") > 0 || cbStyle.Text == "")
            {
                MessageBox.Show("��� �� ������");
                return;
            }
            if (cbProjNum.SelectedIndex < 0)
            {
                cbProjNum.SelectedIndex = 0;
                cbProjNum.Text = "1";
            }
            if (routeNumberOld != NewList && routeNumberOld != tbRouteNumber.Text)
                if (MessageBox.Show(this, "�� �������, ��� ������ �������� ���������� ����� � ������ ������?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == DialogResult.No)
                {
                    tbRouteNumber.Text = routeNumberOld;
                    return;
                }
            int iid, giid = -1, jiid = -1, rn;

            object routeNumber;
            if (tbRouteNumber.Text.Length < 1)
                routeNumber = DBNull.Value;
            else
                if (int.TryParse(tbRouteNumber.Text, out rn))
                    routeNumber = rn;
                else
                {
                    MessageBox.Show(this, "���������� ����� ������� �������");
                    return;
                }
            SqlTransaction tran = cn.BeginTransaction();
            bool transactionSuccess = false;
            string gr = "";
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;
                if (cbStyle.Text.IndexOf("������") < 0)
                {
                    cmd.CommandText = "SELECT iid FROM Groups WHERE name = @name";
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                    cmd.Parameters[0].Value = cbGroup.SelectedItem.ToString();
                    try { giid = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch
                    {
                        MessageBox.Show("������ ������");
                        return;
                    }
                }
                try
                {
                    if (addNew && (cbStyle.Text == "���������" || cbStyle.Text == "��������" || cbStyle.Text == "����������") && cbRound.Text != "�������� ��������")
                    {
                        cmd.CommandText = @"SELECT COUNT(*) 
                                      FROM lists(NOLOCK)
                                     WHERE group_id = " + giid.ToString() + @"
                                       AND style = @style
                                       AND round <> '�������� ��������'
                                       AND prev_round IS NULL";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@style", SqlDbType.VarChar, 50);
                        cmd.Parameters[0].Value = cbStyle.Text;
                        object oTmp = cmd.ExecuteScalar();
                        if (oTmp != null && oTmp != DBNull.Value)
                            if (Convert.ToInt32(oTmp) > 0)
                            {
                                MessageBox.Show(this, "� ������ ���� � ������ ������ ��� ���� ���������. ������� ������� ���������.\r\n" +
                                    "��� �������� ���������� \"���������� ������\" �������������� ���� ������� ��������� \"��������� ������\"");
                                return;
                            }
                        cmd.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "�� ������� ��������� ������ ������������ ���������:\r\n" + ex.Message);
                    return;
                }
                int riid = -1;
                if (cbJudge.SelectedIndex > -1)
                    if (cbJudge.SelectedItem.ToString() != "-" && cbJudge.SelectedItem.ToString() != "���. �� ����")
                    {
                        try { jiid = GetIID(cbJudge.SelectedItem.ToString()); }
                        catch { jiid = -1; }
                    }
                if (cbRouteSetter.SelectedIndex > -1)
                    if (cbRouteSetter.Text != "-" && cbRouteSetter.Text != "��������� ������")
                    {
                        try { riid = GetIID(cbRouteSetter.Text); }
                        catch { riid = -1; }
                    }

                gr = cbGroup.Text;
                if (addNew)
                {
                    cmd.CommandText = "SELECT (MAX(iid)+1) FROM lists";
                    try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch { iid = 1; }
                }
                else
                {
                    try { iid = Convert.ToInt32(tbIid.Text); }
                    catch
                    {
                        MessageBox.Show("�������� �� ������");
                        SetEditMode(false);
                        RefreshData(tran);
                        return;
                    }
                }
                if (addNew)
                    cmd.CommandText = "INSERT INTO lists(group_id, style, round, judge_id, iid, " +
                        "isolationOpen, isolationClose, observation, start, routesetter_id, projNum, routeNumber,listType) " +
                        "VALUES(@gid, @style, @round, @jiid, @iid, @io, @ic, @ob, @st, @riid, @pjn, @rn, @ltp)";
                else
                    cmd.CommandText = "UPDATE lists SET /*group_id=@gid, style=@style, round=@round,*/ judge_id = @jiid, " +
                        "isolationOpen = @io, isolationClose = @ic, observation = @ob, start = @st, routesetter_id = @riid, " +
                        "projNum = @pjn, routeNumber = @rn/*, listType = @ltp*/ WHERE iid = @iid";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@gid", SqlDbType.Int);
                if (cbStyle.Text.IndexOf("������") > -1)
                    cmd.Parameters[0].Value = DBNull.Value;
                else
                    cmd.Parameters[0].Value = giid;

                cmd.Parameters.Add("@style", SqlDbType.VarChar, 50);
                cmd.Parameters[1].Value = cbStyle.Text;

                cmd.Parameters.Add("@round", SqlDbType.VarChar, 50);
                cmd.Parameters[2].Value = cbRound.Text;

                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[3].Value = iid;

                cmd.Parameters.Add("@jiid", SqlDbType.Int);
                if (jiid > 0)
                    cmd.Parameters[4].Value = jiid;
                else
                    cmd.Parameters[4].Value = DBNull.Value;

                cmd.Parameters.Add("@io", SqlDbType.VarChar, 50);
                cmd.Parameters[5].Value = tbIsoOpen.Text;

                cmd.Parameters.Add("@ic", SqlDbType.VarChar, 50);
                cmd.Parameters[6].Value = tbIsoClose.Text;

                cmd.Parameters.Add("@ob", SqlDbType.VarChar, 50);
                cmd.Parameters[7].Value = tbObserv.Text;

                cmd.Parameters.Add("@st", SqlDbType.VarChar, 50);
                cmd.Parameters[8].Value = tbStart.Text;

                cmd.Parameters.Add("@riid", SqlDbType.Int);
                if (riid > 0)
                    cmd.Parameters[9].Value = riid;
                else
                    cmd.Parameters[9].Value = DBNull.Value;

                cmd.Parameters.Add("@pjn", SqlDbType.Int);
                int nTmp;
                if (int.TryParse(cbProjNum.Text, out nTmp))
                    cmd.Parameters[10].Value = nTmp;
                else
                    cmd.Parameters[10].Value = 1;

                cmd.Parameters.Add("@rn", SqlDbType.Int);
                cmd.Parameters[11].Value = routeNumber;

                cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                cmd.Parameters[12].Value = typeToSet.ToString();

                cmd.ExecuteNonQuery();

                
                transactionSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "������ ����������/��������� ���������:\r\n" + ex.Message);
                return;
            }
            finally
            {
                if (!transactionSuccess)
                {
                    try { tran.Rollback(); }
                    catch { }
                }
            }
            if (!transactionSuccess)
                return;
            string round;
            round = cbRound.Text;
            switch (round)
            {
                case "�������� ��������":
#if FULL
                    if (cbStyle.Text == "����������")
                        goto case "����������";
#endif
                    if (cbStyle.Text.IndexOf("������") > -1)
                        goto case "���������";
                    tran.Commit();
                    ShowFinalResults sfr = new ShowFinalResults(competitionTitle, iid, cn);
                    sfr.MdiParent = this.MdiParent;
                    sfr.Show();
                    break;
                case "���������":
                    tran.Commit();
                    TeamResults tr = new TeamResults(cn, iid, competitionTitle);
                    tr.MdiParent = this.MdiParent;
                    tr.Show();
                    break;
#if FULL
                case "����������":
                    tran.Commit();
                    CombinedResults cr = new CombinedResults(giid, cn, competitionTitle, iid);
                    cr.MdiParent = this.MdiParent;
                    cr.Show();
                    break;
#endif
                default:
                    if (addNew)
                    {
                        try
                        {
                            StaticClass.CreateStartList(iid, tran);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch { }
                            MessageBox.Show("������ �������� ���������� ���������:\r\n" +
                                ex.Message);
                            return;
                        }
                        
                    }
                    else
                        tran.Commit();
                    break;
            }

            tbIid.Text = iid.ToString();
            SetEditMode(false);
            try
            {
                int k = cbGroup.Items.IndexOf(gr);
                if (k >= 0)
                    cbGroup.SelectedIndex = k;
            }
            catch { }
            RefreshData();
        }

        private void viewStartList_Click(object sender, EventArgs e)
        {
            int listID;
            try { listID = Convert.ToInt32(tbIid.Text); }
            catch
            {
                MessageBox.Show("�������� �� ������");
                return;
            }
            StartList stl = new StartList(listID, cn, competitionTitle);
            stl.MdiParent = this.MdiParent;
            stl.Show();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (btnDel.Text == "������")
            {
                SetEditMode(false);
                return;
            }
            int listID;
            try { listID = Convert.ToInt32(tbIid.Text); }
            catch
            {
                MessageBox.Show("�������� �� ������");
                return;
            }
            string sGr = "", sSt = "", sRnd = "";
            try
            {
                if (cbGroup.SelectedItem != null)
                    sGr = cbGroup.SelectedItem.ToString();
                if (cbRound.SelectedItem != null)
                    sRnd = cbRound.SelectedItem.ToString();
                if (cbStyle.Text != "" && cbStyle.Text.IndexOf("����") > 0)
                    sSt = cbStyle.Text;
            }
            catch { }
            DialogResult dg = MessageBox.Show("�� ������� ��� ������ ������� �������� " + '\n' +
                cbGroup.Text + " " + cbStyle.Text + " " +
                cbRound.Text + "?", "�������� ���������",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dg == DialogResult.No)
                return;
            try { DropList(listID, cn); }
            catch (Exception ex) { MessageBox.Show("������ �������� ���������:\r\n" + ex.Message); }
            tbIid.Text = "";
            RefreshData();
        }

        public static void DropList(int listID, SqlConnection cn)
        {
            int orderToDel = 0;
            DropList(listID, cn, ref orderToDel);
        }

        private static void DropList(int listID, SqlConnection cn, ref int orderToDel, SqlTransaction _tran = null, long _logicTranID = -1)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction tran = (_tran == null) ? cn.BeginTransaction() : _tran;
            try
            {
                long curLogicTran = (_logicTranID < 1) ?
                    StaticClass.AddLogicTransactionHeader(listID, "lists", cn, tran, false) : _logicTranID;
                

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;

                cmd.CommandText = "SELECT iid FROM lists(NOLOCK) WHERE iid_parent = " + listID.ToString();
                List<int> childLists = new List<int>();
                var dr = cmd.ExecuteReader();
                try
                {
                    while (dr.Read())
                    {
                        try { childLists.Add(Convert.ToInt32(dr["iid"])); }
                        catch { }
                    }
                }
                finally { dr.Close(); }

                foreach (var n in childLists)
                    DropList(n, cn, ref orderToDel, tran, curLogicTran);

                cmd.CommandText = "SELECT style FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
                string style;
                object oTmp = cmd.ExecuteScalar();
                if (oTmp == null || oTmp == DBNull.Value)
                    style = String.Empty;
                else
                    style = oTmp.ToString();

                string linesTableName;
                switch (style.ToLower().Trim())
                {
                    case "���������":
                        linesTableName = "routeResults";
                        break;
                    case "��������":
                        linesTableName = "speedResults";
                        break;
                    default:
                        linesTableName = String.Empty;
                        break;
                }

                if (linesTableName.Equals("speedResults"))
                {
                    string iidsToDel = String.Empty;
                    cmd.CommandText = "SELECT DISTINCT R.iid" +
                                      "  FROM speedResults SR(nolock)" +
                                      "  JOIN speedResults_Runs R(nolock) on R.iid_parent = SR.iid OR R.iid_parent_comp = SR.iid" +
                                      " WHERE SR.list_id = " + listID.ToString();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                            iidsToDel = iidsToDel + "," + rdr[0].ToString();
                    }
                    if (!String.IsNullOrEmpty(iidsToDel))
                    {
                        iidsToDel = iidsToDel.Substring(1);
                        cmd.CommandText = "DELETE FROM speedResults_Runs WHERE iid IN(" + iidsToDel + ")";
                        cmd.ExecuteNonQuery();
                    }
                }

                if (!linesTableName.Equals(String.Empty))
                {
                    List<long> linesIids = new List<long>();
                    cmd.CommandText = "SELECT iid FROM " + linesTableName + "(NOLOCK) WHERE list_id = " + listID.ToString();
                    dr = cmd.ExecuteReader();
                    try
                    {
                        while (dr.Read())
                            try { linesIids.Add(Convert.ToInt64(dr["iid"])); }
                            catch { }
                    }
                    finally { dr.Close(); }

                    foreach (var n in linesIids)
                        StaticClass.AddTableDataToDel(linesTableName, "iid", n, curLogicTran, cn, ref orderToDel, tran);
                }

                StaticClass.AddTableDataToDel("lists", "iid", listID, curLogicTran, cn, ref orderToDel, tran);


                cmd.CommandText = "DELETE FROM lists WHERE iid = " + listID.ToString();
                cmd.ExecuteNonQuery();


                if (_tran == null)
                    tran.Commit();
            }
            catch (Exception ex)
            {
                if(_tran == null)
                    try { tran.Rollback(); }
                    catch { }
                throw ex;
            }
        }

        private void viewResultList_Click(object sender, EventArgs e)
        {
            int listID;
            try { listID = Convert.ToInt32(tbIid.Text); }
            catch
            {
                MessageBox.Show("�������� �� ������");
                return;
            }
            Form frm;
            switch (selectedListType)
            {
                case ListTypeEnum.General:
                    frm = new ShowFinalResults(competitionTitle, listID, cn);
                    break;
#if FULL
                case ListTypeEnum.LeadFlash:
                    frm = new ResultListQf(listID, cn, competitionTitle, cbGroup.SelectedItem.ToString());
                    break;
                case ListTypeEnum.LeadSimple:
                    frm = new ResultListLead(cn, competitionTitle, listID, false, false);
                    break;
                case ListTypeEnum.LeadGroups:
                    frm = new Result2routes(competitionTitle, cn, listID);
                    break;
                case ListTypeEnum.BoulderGroups:
                    frm = new Result2routes(competitionTitle, cn, listID);
                    break;
                case ListTypeEnum.BoulderSuper:
                    frm = new ResultListLead(cn, competitionTitle, listID, false, true);
                    break;
                case ListTypeEnum.BoulderSimple:
                    frm = new ResultListBoulder(cn, competitionTitle, listID, false, path);
                    break;
#endif
                case ListTypeEnum.SpeedQualy:

                    frm = new ResultListSpeed(listID, cn, competitionTitle);
                    break;
                case ListTypeEnum.SpeedQualy2:
                    goto case ListTypeEnum.SpeedQualy;
                case ListTypeEnum.SpeedFinal:
                    frm = new SpeedFinals(listID, cn, competitionTitle, cbRound.Text == "�����");
                    break;
#if FULL
                case ListTypeEnum.Combined:
                    SqlCommand cmd = new SqlCommand("SELECT group_id FROM lists WHERE iid=" + listID.ToString(), cn);
                    int giid = 0;
                    try { giid = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                    frm = new CombinedResults(giid, cn, competitionTitle, listID);
                    break;
#endif
                case ListTypeEnum.TeamGeneral:
                    frm = new TeamResults(cn, listID, competitionTitle);
                    break;
                case ListTypeEnum.TeamBoulder:
                    goto case ListTypeEnum.TeamGeneral;
                case ListTypeEnum.TeamLead:
                    goto case ListTypeEnum.TeamGeneral;
                case ListTypeEnum.TeamSpeed:
                    goto case ListTypeEnum.TeamGeneral;
                default:
                    MessageBox.Show(this, "����������� ��� ���������");
                    return;
            }
            frm.MdiParent = this.MdiParent;
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
                try
                {
                    da.Update(dt);
                    RefreshData();
                    changed = false;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void btnPrintAct_Click(object sender, EventArgs e)
        {
            if(tbIid.Text!="")
                try
                {
                    int id = int.Parse(tbIid.Text);
                    CreateAct ct = new CreateAct(cn, id);
                    ct.ShowDialog();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
                
        protected override void OnClosing(CancelEventArgs e)
        {
            if (changed && (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY))
            {
                switch (MessageBox.Show("��������� ���������?", 
                    this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        SaveData();
                        e.Cancel = false;
                        break;
                    case DialogResult.No:
                        e.Cancel = false;
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            base.OnClosing(e);
        }

        private void dgLists_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changed = true;
        }

        private void cbRound_DropDown(object sender, EventArgs e)
        {
            cbRound.Items.Clear();
            switch (cbStyle.Text)
            {
#if FULL
                case "���������":
                    cbRound.Items.Add("������������");
                    cbRound.Items.Add("1/4 ������ (2 ������)");
                    cbRound.Items.Add("1/4 ������");
                    cbRound.Items.Add("1/2 ������");
                    cbRound.Items.Add("�����");
                    cbRound.Items.Add("����������");
                    break;
                case "����������":
                    cbRound.Items.Add("������������ (2 ������)");
                    cbRound.Items.Add("1/4 ������");
                    cbRound.Items.Add("1/2 ������");
                    cbRound.Items.Add("�����");
                    cbRound.Items.Add("����������");
                    break;

                case "����������":
                    cbRound.Items.Add("������������");
                    cbRound.Items.Add("�����");
                    break;
#endif
                case "��������":
                    cbRound.Items.Add("������������");
                    break;
            }
            cbRound.Items.Add("�������� ��������");
        }

        private void cbRound_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbRouteNumber.Enabled &&
                    cbStyle.Text == "���������")
                {
                    if (cbRound.Text == "������������" && tbRouteNumber.Text == "")
                        tbRouteNumber.Text = "2";
                    else if (cbRound.Text != "������������")
                        tbRouteNumber.Text = "";
                }
            }
            catch { }
        }
    }

    public class StartListMember:IComparable
    {
        public static StartListMember Empty;
        static StartListMember()
        {
            Empty = new StartListMember();
        }
        public int iid = 0;
        public int ranking = 0;
        public double rndDouble = 0.0;
        public int prevPos = 0;
        public StartListMember() { }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is StartListMember)
            {
                StartListMember other = (StartListMember)obj;
                if (this.iid == other.iid)
                    return this.iid.CompareTo(other.iid);
                if (this.prevPos == other.prevPos && this.ranking == other.ranking)
                    return this.rndDouble.CompareTo(other.rndDouble);
                else if (this.prevPos == other.prevPos)
                    return this.ranking.CompareTo(other.ranking);
                else
                    return this.prevPos.CompareTo(other.prevPos);
            }
            else
                throw new NotImplementedException();
        }

        #endregion
    }
}