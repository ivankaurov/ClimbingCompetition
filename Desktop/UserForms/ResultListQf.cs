// <copyright file="ResultListQf.cs">
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
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;

namespace ClimbingCompetition
{
    /// <summary>
    /// �������� 2� ������������ ��������� (��� �� ������� �������������)
    /// </summary>
    public partial class ResultListQf
#if FULL
        : BaseForm
#endif
    {
#if FULL
        //SqlConnection cn;
        int listID, judgeID;
        string gName;
        DataTable dtRes;
        int routeNumber;
        string roundThis;
        public ResultListQf(int listID, SqlConnection baseCon, string competitionTitle, string gName)
            : base(baseCon, competitionTitle)
        {
            InitializeComponent();
            this.listID = listID;
            //this.cn = cn;
            this.gName = gName;
            //this.Text = "�������� ����������� " + gName + " ��������� ";

           
            this.Text = StaticClass.GetListName(listID, cn);
            SqlCommand cmd = new SqlCommand("SELECT judge_id FROM lists(NOLOCK) WHERE iid = " + listID.ToString(), cn);
            try { judgeID = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { judgeID = 0; }
            cmd.CommandText = "SELECT ISNULL(round,'') rnd FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            try { roundThis= cmd.ExecuteScalar().ToString(); }
            catch { roundThis = String.Empty; }
            
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                try { btnNxtRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                catch { btnNxtRound.Enabled = true; }
            }
            else
                btnNxtRound.Enabled = false;
            cbRound.Enabled = btnNxtRound.Enabled;
            //dtRes.Columns["posText"].ColumnName = "�����";
        }

        private int timeLists = 0;
        private KeyValuePair<int, bool>[] lists = new KeyValuePair<int, bool>[0];

        private void RefreshData()
        {
            SettingsForm.FlashShowMode fs = SettingsForm.GetFlashShowMode(cn);
            bool showEverybody = (fs == SettingsForm.FlashShowMode.BTH || fs == SettingsForm.FlashShowMode.ADM);
            dtRes = StaticClass.FillResFlash(this.listID, this.cn, false, out routeNumber, showEverybody, out timeLists, out lists);
            try
            {
                dtRes.Columns.Remove("�");
            }
            catch { }
            try
            {
                dtRes.Columns.Remove("pos");
            }
            catch { }
            try { dg.DataSource = dtRes; }
            catch { }
            //dtRes.Columns["posText"].ColumnName = "�����";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void xlExport_Click(object sender, EventArgs e)
        {

            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                int i = 0, resCol = -1;
                const int frst_row = 6;
                try { dtRes.Columns.Remove("pos"); }
                catch { }
                foreach (DataColumn cl in dtRes.Columns)
                {
                    i++;
                    if (cl.ColumnName == "����������")
                        ws.Cells[frst_row, i] = "�������, ���";
                    else
                    {
                        ws.Cells[frst_row, i] = cl.ColumnName;
                        if (cl.ColumnName == "���-�")
                            resCol = i;
                    }
                }
                i = frst_row;
                int lastFin = -1;
                char lstC = (char)('G' + (2 * routeNumber) + timeLists);
                foreach (DataRow rd in dtRes.Rows)
                {
                    i++;
                    for (int k = 1; k < 6; k++)
                        ws.Cells[i, k] = rd[k - 1];
                    ws.Cells[i, 6] = " " + rd[5].ToString();
                    ws.Cells[i, 7] = rd[6].ToString();
                    ws.Cells[i, 8] = " " + rd[7].ToString();
                    for (int k = 9; k <= dtRes.Columns.Count; k++)
                    {
                        if (resCol >= 9 && k == resCol)
                            ws.Cells[i, k] = rd[k - 1].ToString();
                        else
                            ws.Cells[i, k] = rd[k - 1];
                    }
                    if ((rd["�.�."].ToString() == "0"))
                        ws.Cells[i, 3] = "";
                    if ((rd["��."].ToString() != "Q") && (lastFin == -1) && (i > 5))
                        lastFin = i - 1;
                }
                Excel.Range dt;
                if (lastFin > -1)
                {
                    dt = ws.get_Range("A" + frst_row, lstC + lastFin.ToString());
                    dt.Style = "MyStyle";
                    dt = ws.get_Range("A" + (lastFin + 1).ToString(), (char)(lstC - 1) + i.ToString());
                    dt.Style = "MyStyle";
                }
                else
                {
                    dt = ws.get_Range("A" + frst_row.ToString(), lstC + i.ToString());
                    dt.Style = "MyStyle";
                }
                dt = ws.get_Range("A" + frst_row.ToString(), lstC + i.ToString());
                dt.Columns.AutoFit();

                dt = ws.get_Range("B" + frst_row.ToString(), "B" + i.ToString());
                dt.Style = "StyleLA";

                dt = ws.get_Range("A1", lstC + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                string strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

                dt = ws.get_Range(lstC + "2", lstC + "2");
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                dt.Style = "StyleRA";
                if (judgeID > 0)
                    ws.Cells[5, 1] = "���. ��. ����� �� ����: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[5, 1] = "���. ��. ����� �� ����:";

                dt = ws.get_Range("A3", lstC + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                strTmp = "���������. " + gName;
                dt.Cells[1, 1] = strTmp;

                strTmp = "�������� �����������. " + roundThis;
                dt = ws.get_Range("A4", lstC + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "���_";
                strTmp += "��_";

                try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, roundThis) + "_��"; }
                catch { }

                StaticClass.ExcelUnderlineQf(ws);
            }
            catch (Exception ex) { MessageBox.Show("������ �������� ������ � Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void btnNxtRound_Click(object sender, EventArgs e)
        {
            //NewStartList nl;
            if (cbRound.SelectedIndex < 0)
            {
                MessageBox.Show("����� �� ������");
                return;
            }

            int nextID;
            //bool b = false;
            SqlCommand cmd = new SqlCommand("SELECT l.iid FROM lists l(NOLOCK), lists l1(NOLOCK) WHERE " +
                "(l.style=l1.style) AND (l.group_id=l1.group_id) AND (l1.iid=" + listID.ToString() +
                ") AND (l.round = '" + cbRound.SelectedItem.ToString() + "')", cn);
            try { nextID = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { nextID = 0; }
            if (nextID > 0)
            {
                DialogResult dgr = MessageBox.Show("����� �������� ��� ����������.\n�������� ���?",
                    "�������� ����������", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dgr == DialogResult.No)
                    return;
                //b = true;
            }
            else
                nextID = (int)StaticClass.GetNextIID("lists", cn, "iid", null);
            DataTable dtTmp = StaticClass.FillResFlash(listID, cn, false, out routeNumber, true, out timeLists, out lists);
            Random rnd = new Random();
            cmd.CommandText = "SELECT rankingLead, lateAppl FROM Participants P(NOLOCK) WHERE iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            List<Starter> data = new List<Starter>();
            foreach (DataRow rdd in dtTmp.Rows)
            {
                if (rdd["��."].ToString() == "Q")
                {
                    Starter st = new Starter();
                    st.iid = Convert.ToInt32(rdd["�"]);
                    st.prevPos = Convert.ToInt32(rdd["pos"]);
                    st.prevStart = 1;
                    st.random = rnd.NextDouble();
                    st.ranking = int.MaxValue;
                    st.lateAppl = false;
                    cmd.Parameters[0].Value = st.iid;
                    try
                    {
                        SqlDataReader rdss = cmd.ExecuteReader();
                        while (rdss.Read())
                        {
                            try { st.lateAppl = (bool)rdss["lateAppl"]; }
                            catch { }
                            try { st.ranking = (int)rdss["rankingLead"]; }
                            catch { }
                            break;
                        }
                        rdss.Close();
                    }
                    catch { }
                    data.Add(st);
                }
                else
                    break;
            }
            //cmd.CommandText = "SELECT LQ.iid " +
            //                  "  FROM lists LQ(NOLOCK) " +
            //                  "  JOIN lists LF(NOLOCK) ON LQ.style = LF.style " +
            //                  "                       AND LQ.group_id = LF.group_id " +
            //                  "                       AND LQ.round = '������������ 1'" +
            //                  " WHERE LF.iid = " + listID.ToString();

            try
            {
                cmd.CommandText = "SELECT P.iid, P.lateAppl, P.rankingLead " +
                                  "  FROM Participants P(NOLOCK) " +
                                  "  JOIN routeResults R(NOLOCK) ON R.climber_id = P.iid " +
                                  "  JOIN lists        L(NOLOCK) ON L.iid = R.list_id " +
                                  " WHERE L.iid_parent = " + listID.ToString() +
                                  "   AND R.preQf = 1 ";
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Starter s = new Starter();
                    s.iid = Convert.ToInt32(rdr["iid"]);
                    bool exists = false;
                    foreach (Starter st in data)
                        if (st.iid == s.iid)
                        {
                            exists = true;
                            break;
                        }
                    if (!exists)
                    {
                        s.lateAppl = Convert.ToBoolean(rdr["lateAppl"]);
                        s.prevPos = 0;
                        s.prevStart = 0;
                        s.random = rnd.NextDouble();
                        try { s.ranking = Convert.ToInt32(rdr["rankingLead"]); }
                        catch { s.ranking = int.MaxValue; }
                        data.Add(s);
                    }
                }
                rdr.Close();
            }
            catch { }


            //foreach(StartListMember stm in stNum)
            //{
            //    StartListMember stmQ;
            //    stmQ.iid = stm.iid;
            //    stmQ.prevPos = stm.prevPos;
            //    cmd.CommandText = "SELECT rankingLead FROM Participants WHERE iid = " + stm.iid.ToString();
            //    try { stmQ.ranking = Convert.ToInt32(cmd.ExecuteScalar()); }
            //    catch { stmQ.ranking = 9999; }
            //    stmQ.rndDouble = stm.rndDouble;
            //    starters.Add(stmQ);
            //}
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@i", SqlDbType.Int);
            cmd.Parameters[0].Value = listID;
            cmd.CommandText = "SELECT g.name, g.iid FROM lists l INNER JOIN Groups g ON l.group_id=g.iid WHERE l.iid=@i";
            SqlDataReader rd = cmd.ExecuteReader();
            int group_id = 0;
            string group_name = "";
            while (rd.Read())
            {
                group_id = Convert.ToInt32(rd[1]);
                group_name = rd[0].ToString();
                break;
            }
            rd.Close();
            //int nextID = (int)StaticMethods.GetNextIID("lists", cn);

            ListTypeEnum newListType = ListTypeEnum.LeadSimple;
            List<int> childLists = new List<int>();
            try
            {
                //cmd.Transaction = cn.BeginTransaction();
                cmd.Transaction = cn.BeginTransaction();

                cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,online) VALUES (@i,@g,@s,@r,@o)";
                cmd.Parameters[0].Value = nextID;

                cmd.Parameters.Add("@g", SqlDbType.Int);
                cmd.Parameters[1].Value = group_id;

                cmd.Parameters.Add("@s", SqlDbType.VarChar);
                cmd.Parameters[2].Value = "���������";

                cmd.Parameters.Add("@r", SqlDbType.VarChar);
                cmd.Parameters[3].Value = cbRound.SelectedItem.ToString();

                cmd.Parameters.Add("@o", SqlDbType.Bit);
                cmd.Parameters[4].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);

                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                cmd.Parameters[1].Value = listID;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                cmd.ExecuteNonQuery();

                //nl = new NewStartList(false, group_name, "���������", cbRound.SelectedItem.ToString());
                //nl.ShowDialog();
                Sorting sorting = new Sorting(data, StartListMode.NotFirstRound, true);
                sorting.ShowDialog();
                if (sorting.Cancel)
                //if (nl.type == StartListType.Cancel)
                {
                    //cmd.Transaction.Rollback();

                    try { cmd.Transaction.Rollback(); }
                    catch
                    {
                        cmd.CommandText = "DELETE FROM lists WHERE iid = " + nextID.ToString();
                        try { cmd.ExecuteNonQuery(); }
                        catch { }
                    }
                    return;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE lists SET listType = @ltp WHERE iid = " + nextID.ToString();
                cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                newListType = sorting.RoundFlash ? ListTypeEnum.LeadFlash : ListTypeEnum.LeadSimple;
                cmd.Parameters[0].Value = newListType.ToString();
                cmd.ExecuteNonQuery();

                if (newListType == ListTypeEnum.LeadFlash)
                {
                    cmd.CommandText = "UPDATE lists SET routeNumber = " + sorting.SettedRouteNumber.ToString() +
                        " WHERE iid = " + nextID.ToString();
                    cmd.ExecuteNonQuery();
                    for (int i = 1; i <= sorting.SettedRouteNumber; i++)
                    {
                        cmd.Parameters.Clear();
                        int curId = (int)StaticClass.GetNextIID("lists", cn, "iid", cmd.Transaction);
                        cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,online,listType,iid_parent) VALUES (@i,@g,@s,@r,@o,@lt,@ip)";
                        cmd.Parameters.Add("@i", SqlDbType.Int);
                        cmd.Parameters[0].Value = curId;
                        childLists.Add(curId);

                        cmd.Parameters.Add("@g", SqlDbType.Int);
                        cmd.Parameters[1].Value = group_id;

                        cmd.Parameters.Add("@s", SqlDbType.VarChar);
                        cmd.Parameters[2].Value = "���������";

                        cmd.Parameters.Add("@r", SqlDbType.VarChar);
                        cmd.Parameters[3].Value = cbRound.SelectedItem.ToString() + " ������ " + i.ToString();

                        cmd.Parameters.Add("@o", SqlDbType.Bit);
                        cmd.Parameters[4].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);

                        cmd.Parameters.Add("@lt", SqlDbType.VarChar, 255);
                        cmd.Parameters[5].Value = ListTypeEnum.LeadSimple.ToString();

                        cmd.Parameters.Add("@ip", SqlDbType.Int);
                        cmd.Parameters[6].Value = nextID;

                        cmd.ExecuteNonQuery();
                    }
                }
                //bool b;
                /*
                do
                {
                    switch (nl.type)
                    {
                        case StartListType.General:
                            starters.Sort(new RandomListComparer());
                            b = false;
                            break;
                        case StartListType.Reverse:
                            starters.Sort(new NextRoundComparer());
                            b = false;
                            break;
                        case StartListType.Cancel:
                            cmd.Transaction.Rollback();
                            return;
                        default:
                            MessageBox.Show("��� ���������� ����������������. �������� ������ ���.");
                            nl.ShowDialog();
                            b = true;
                            break;
                    }
                } while (b);*/

                //sorting.Starters
                if (newListType == ListTypeEnum.LeadSimple)
                {
                    childLists.Clear();
                    childLists.Add(nextID);
                }
                for (int i = 0; i < sorting.Count && i < childLists.Count; i++)
                {
                    InsertToList(childLists[i], sorting[i], cn, cmd.Transaction);
                }
                cmd.Transaction.Commit();
                MessageBox.Show("�������� ������. iid = " + nextID.ToString());
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                try { cmd.Transaction.Rollback(); }
                catch { }
                return;
            }
        }

        public static void InsertToList(int list_id, List<Starter> list, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@list_id", SqlDbType.Int);
            cmd.Parameters[0].Value = list_id;
            cmd.Parameters.Add("@iid", SqlDbType.BigInt);
            cmd.Parameters.Add("@climber_id", SqlDbType.Int);
            cmd.Parameters.Add("@start", SqlDbType.Int);
            cmd.Parameters.Add("@p", SqlDbType.VarChar);
            cmd.Parameters[4].Value = int.MaxValue;
            cmd.CommandText = "INSERT INTO routeResults(iid, list_id, climber_id, start, pos) " +
                "VALUES (@iid, @list_id, @climber_id, @start, @p)";
            int i = 0;
            foreach (Starter strte in list)
            //for (int i = 0; i < starters.Count; i++)
            {
                cmd.Parameters[1].Value = StaticClass.GetNextIID("routeResults", cn, "iid", cmd.Transaction);
                cmd.Parameters[2].Value = strte.iid;
                cmd.Parameters[3].Value = ++i;
                cmd.ExecuteNonQuery();
            }
        }

        private void cbRound_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ResultListQf_Load(object sender, EventArgs e)
        {
            try { RefreshData(); }
            catch (Exception ex) { MessageBox.Show(this, "������ �������� ������:\r\n" + ex.Message); }
        }
#endif
    }
}