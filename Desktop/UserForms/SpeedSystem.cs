// <copyright file="SpeedSystem.cs">
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
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UserForms;
using ClimbingCompetition.SpeedData;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� ����� ����������� ������������ �������� � ������ ���������� ������ �� ���������� �������
    /// </summary>
    public partial class SpeedSystem : SpeedFinals
    {
        private int baseIID = 0;
        
        bool IsQualyAndNeed1st = false, preQfClimb;

        protected bool UseBestQfFromTwo
        {
            get { return (Rules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds; }
        }

        protected bool UseBestRoute
        {
            get { return (Rules & SpeedRules.BestRouteInQfRound) == SpeedRules.BestRouteInQfRound; }
        }

        public SpeedSystem(int listID, SqlConnection baseCon, string competitionTitle, bool trial)
            :
        base(listID, baseCon, competitionTitle, false)
        {

            InitializeComponent();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (cn.State != ConnectionState.Open)
                cn.Open();


            preQfClimb = SettingsForm.GetPreQfClimb(this.cn, listID);
            cmd.CommandText = "SELECT round FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            IsQualyAndNeed1st = (cmd.ExecuteScalar() as string).Equals("������������ 2", StringComparison.InvariantCultureIgnoreCase) && UseBestQfFromTwo;

            gbWorkMode.Visible = gbWorkMode.Enabled = false;
            //if (this.speedAdvancedSystem)
            //{
            //    cbShowView.SelectedIndex = 0;
            //}
            //else
            //    cbShowView.Visible = cbShowView.Enabled = false;

            string select1st, select2nd, sOrderBy;
            if (IsQualyAndNeed1st)
            {
                select1st = " dbo.fn_get1st(l.climber_id, l.list_id) AS [����.1], ";
                select2nd = " dbo.fn_getBest(l.iid) AS [����.����], ";
                sOrderBy = " dbo.fn_getBestRes(l.iid), ";
            }
            else
            {
                select2nd = select1st = String.Empty;
                sOrderBy = " l.res, ";
            }

            rbInternational.Visible = rbNational.Visible = false;
            this.Text = StaticClass.GetListName(listID, cn);
            daResW.SelectCommand.CommandText =
                    "   SELECT l.posText AS [�����], l.climber_id AS [�], " +
                    "          p.surname + ' ' + p.name AS [����������], p.age AS [�.�.], " +
                    "          p.qf AS [������], dbo.fn_getTeamName(p.iid, l.list_id) AS [�������], " + select1st +
                    "          l.route1_text AS [������ 1], l.route2_text AS [������ 2]," +
                    "          l.resText AS [�����], " + select2nd + "l.qf AS [��.], l.start AS [��.�] " +
                    "     FROM speedResults l(NOLOCK) " +
                    "     JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "    WHERE l.list_id = " + listID.ToString();
            if (UseBestRoute)
                daResW.SelectCommand.CommandText +=
                        "      AND (ISNULL(route1_text,'') <> ''" +
                        "      AND ISNULL(route2_text,'') <> ''" +
                        "       OR CHARINDEX('�/�',resText) > 0)";
            else
                daResW.SelectCommand.CommandText +=
                    "      AND ISNULL(resText,'') <> '' ";
            daResW.SelectCommand.CommandText +=
                    " ORDER BY " + sOrderBy + " l.pos, p.vk, l.route1, [�������], p.surname, p.name";

            daStart.SelectCommand.CommandText =
                "   SELECT l.start AS [��.�], l.climber_id AS [�], (p.surname+' '+p.name) AS [����������]," +
                "          p.age AS [�.�.], p.qf AS [������], dbo.fn_getTeamName(p.iid, l.list_id) AS [�������], " + select1st +
                "          l.route1_text AS [������ 1], l.route2_text AS [������ 2], " +
                "          CASE WHEN l.route1_text IS NULL THEN 1 " +
                "               WHEN l.route1_text = '' THEN 1 " +
                "               ELSE 0 END ps " +
                "     FROM speedResults l(NOLOCK) " +
                "     JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                "    WHERE l.list_id = " + listID.ToString();
            if (UseBestRoute)
                daStart.SelectCommand.CommandText +=
                "      AND (ISNULL(route1_text,'') = ''" +
                "       OR ISNULL(route2_text,'') = '')" +
                "      AND (CHARINDEX('�/�', resText) < 1 " +
                "       OR restext IS NULL)";
            else
                daStart.SelectCommand.CommandText +=
                "      AND ISNULL(l.resText,'') = '' ";
            daStart.SelectCommand.CommandText +=
                (preQfClimb ? String.Empty : "      AND l.preQf = 0 ") +
                //"      AND l.route1_text IS NOT NULL "+
                " ORDER BY ps, l.start";
            SetNext();

            cmd.CommandText = "SELECT l.prev_round FROM lists l(NOLOCK) WHERE l.iid=" + this.listID.ToString();

            try
            {
                if (this.cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                baseIID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch { baseIID = 0; }
            try { SetEditMode(false); }
            catch { }
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                try { btnNextRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                catch { btnNextRound.Enabled = true; }
            }
            else
                btnNextRound.Enabled = false;
            cbNxtRound.Enabled = btnNextRound.Enabled;
        }

        protected override void RefreshData()
        {
            if (daResW == null || dgRes == null || daStart == null || dgStart == null)
                return;
            try
            {
                dtStart.Clear();
                dtStart.Columns.Clear();
                daStart.Fill(dtStart);
                dgStart.DataSource = dtStart;
                try { dgStart.Columns["ps"].Visible = false; }
                catch { }
                dtRes.Clear();
                dtRes.Columns.Clear();
                daResW.Fill(dtRes);
                dgRes.DataSource = dtRes;
                try { dgRes.Columns["��.�"].Visible = false; }
                catch { }
                if (btnNextRound != null && cbNxtRound != null)
                {
                    bool isQ = false;
                    foreach (DataRow row in dtRes.Rows)
                        if (row["��."].ToString().ToLower().IndexOf("q") > -1)
                        {
                            isQ = true;
                            break;
                        }
                    btnNextRound.Enabled = cbNxtRound.Enabled = isQ;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ �������� ������\r\n" + ex.Message);
            }
        }


        public ResultListSpeed.WorkMode WM
        {
            get
            {
                if (rbRoute1.Checked)
                    return ResultListSpeed.WorkMode.ROUTE_1;
                if (rbRoute2.Checked)
                    return ResultListSpeed.WorkMode.ROUTE_2;
                if (rbSystem.Checked)
                    return ResultListSpeed.WorkMode.SYSTEM;
                return ResultListSpeed.WorkMode.BOTH_ROUTES;
            }
        }

        private void rbSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbSystem.Checked)
            {
                StaticClass.mSpeed.WaitOne(1000, true);
                try
                {
                    ResultListSpeed sr = new ResultListSpeed(listID, cn, competitionTitle);
                    sr.WM = this.WM;
                    sr.MdiParent = this.MdiParent;
                    sr.Show();
                    this.Invoke(new EventHandler(CloseF));
                }
                finally { StaticClass.mSpeed.ReleaseMutex(); }
            }
        }
        void CloseF(object sender, EventArgs e) {
            StaticClass.mSpeed.WaitOne(1000, true);
            try { this.Close(); }
            finally { StaticClass.mSpeed.ReleaseMutex(); }
        }
        protected override void btnNextRound_Click(object sender, EventArgs e)
        {
            if (dtStart.Rows.Count > 0)
                if (MessageBox.Show("���� ��� ���������������� ����������. �� ����� ������� ��������� ��������?",
                   "������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            try
            {
                switch (cbNxtRound.SelectedIndex)
                {
                    case 0:
                        ResultListSpeed.SecondQf(listID, cn);
                        return;
                    case 1:
                        ResultListSpeed.Pairing(listID, cn, false, true);
                        return;
                    default:
                        MessageBox.Show("����������, �������� �����");
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ �������� ���������� ��������� �� ��������� �����.\r\n" + ex.Message);
            }
        }

        protected override DataFromSystem.ResStr[] PutData(DataFromSystem ds, string r1, string r2, out bool cancel)
        {
            ds.SetResMode(1, false, true);
            ds.SetResMode(2, this.sl is IvanovoListener, true);
            return ds.ShowForm(out cancel);
        }

        private bool fromEventObj = false;
        private System.Threading.Mutex mFE = new System.Threading.Mutex();
        private bool fromEvent
        {
            get
            {
                mFE.WaitOne();
                try { return fromEventObj; }
                finally { mFE.ReleaseMutex(); }
            }
            set
            {
                mFE.WaitOne();
                try { fromEventObj = value; }
                finally { mFE.ReleaseMutex(); }
            }
        }

        protected override void SetEditMode(bool edit)
        {
            bool fLoaded = leftIID.Text.Length > 0;
            bool sLoaded = lblIID.Text.Length > 0;

            btnRefresh.Enabled = !edit;

            btnCancel.Enabled = btnShowSystem.Enabled = edit && (fLoaded || sLoaded);
            if (btnNext != null)
                btnNext.Enabled = !btnCancel.Enabled;
            xlExport.Enabled = !btnCancel.Enabled;
            if (cbNxtRound != null && cbNxtRound != null)
            {
                cbNxtRound.Enabled = !btnCancel.Enabled;
                btnNextRound.Enabled = !btnCancel.Enabled && (cbNxtRound.SelectedIndex >= 0);
            }
            else
                btnNextRound.Enabled = false;
            if (btnCancel.Enabled)
                UpdateNowCl();

            if (edit && (fLoaded || sLoaded))
                btnEdit.Text = "�����������";
            else
                btnEdit.Text = "������";
            leftRoute1.Enabled = edit && fLoaded;
            rightRoute2.Enabled = rightSum.Enabled = edit && sLoaded;

            if (edit)
                btnRollBack.Enabled = btnRestore.Enabled = btnRollBackSecond.Enabled = btnRestoreSecond.Enabled = false;
            else
                try { SetTransactionButtonsEnabled(); }
                catch { }

            if (btnCancel.Enabled)
                if (!fromEvent)
                    EnterDataFromSystem();
            if (rightSum.Enabled && UseBestRoute)
                rightSum.Enabled = false;
        }

        private void UpdateNowCl()
        {
            try
            {
                string lR, rR;
                if (leftIID.Text.Length > 0)
                    lR = leftIID.Text;
                else
                    lR = "NULL";
                if (lblIID.Text.Length > 0)
                    rR = lblIID.Text;
                else
                    rR = "NULL";
                cmd.CommandText = "UPDATE lists SET nowClimbing = " + lR + ", nowClimbingTmp = " + rR + " WHERE iid = " + listID.ToString();
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        protected override void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "������")
            {
                SetEditMode(true);
                return;
            }
            bool error = false;
            bool br = false;

            Dictionary<string, string> valLeft = null, valRight = null;
            int nTmp;
            long? resIdLeft, resIdRight;
            try
            {
                if (int.TryParse(leftIID.Text, out nTmp))
                    resIdLeft = ResultListSpeed.GetClimbersResSpeed(nTmp, listID, cn, out valLeft);
                else
                    resIdLeft = null;
            }
            catch { resIdLeft = null; }

            try
            {
                if (int.TryParse(lblIID.Text, out nTmp))
                    resIdRight = ResultListSpeed.GetClimbersResSpeed(nTmp, listID, cn, out valRight);
                else
                    resIdRight = null;
            }
            catch { resIdRight = null; }

            try
            {
                if (leftIID.Text.Length > 0)
                {
                    //if (leftRoute1.Text.Length > 0)
                    //{
                    if (leftRoute1.Text.IndexOf("�/�") > -1 ||
                        !UseBestRoute && (leftRoute1.Text.IndexOf("����") > -1 ||
                                leftRoute1.Text.IndexOf("�����") > -1))
                        leftRoute2.Text = leftSum.Text = "";
                    if (leftRoute2.Text.Length > 0 || leftSum.Text.Length > 0)
                    {
                        if (leftSum.Text.IndexOf("����") > -1 ||
                            leftSum.Text.IndexOf("�/�") > -1 ||
                            leftSum.Text.IndexOf("�����") > -1)
                        {
                            leftSum.Text = "";
                            ResultListSpeed.ClearClimber(3, leftIID.Text, listID, cn);
                            error = !EnterFirstRoute();
                        }
                        else
                        {
                            if (leftRoute2.Text.Length > 0 && leftSum.Text.Length > 0)
                                leftSum.Text = "";
                            error = !EnterBothRoutes(leftIID, leftRoute1, leftRoute2, leftSum);
                        }
                    }
                    else
                        error = !EnterFirstRoute();
                    //}
                    //else
                    //{
                    //    error = true;
                    //    MessageBox.Show("������� ��������� �� 1 ������");
                    //}
                }
                if (lblIID.Text.Length > 0)
                {
                    //if (rightRoute2.Text.Length > 0 || rightSum.Text.Length > 0)
                        error = error || (!(br = EnterBothRoutes()));
                    //else
                    //{
                    //    error = true;
                    //    MessageBox.Show("������� ��������� 2 ������ ��� �����");
                    //}
                }
            }
            catch (Exception ex)
            {
                error = true;
                MessageBox.Show("������ ����� �����������\r\n" + ex.Message);
            }
            if (br && (!error))
            {
                SqlCommand cmd = new SqlCommand();
                
                
            }
            if (!error)
            {
                SetEditMode(false);
                ResultListSpeed.RefreshTable(listID, baseIID, RefreshData, cn, CR);

                if (resIdLeft != null && resIdLeft.HasValue || resIdRight != null && resIdRight.HasValue)
                {
                    SqlTransaction tran = cn.BeginTransaction();
                    try
                    {
                        Dictionary<string, string> newVals;
                        if (resIdLeft != null && resIdLeft.HasValue)
                        {
                            ResultListSpeed.GetClimbersResSpeed(resIdLeft.Value, cn, out newVals, tran);
                            ResultListSpeed.AddTransaction(resIid: resIdLeft.Value,
                                                           oldValues: valLeft,
                                                           newValues: newVals,
                                                           cn: cn,
                                                           tran: tran);
                        }
                        if (resIdRight != null && resIdRight.HasValue)
                        {
                            ResultListSpeed.GetClimbersResSpeed(resIdRight.Value, cn, out newVals, tran);
                            ResultListSpeed.AddTransaction(resIid: resIdRight.Value,
                                                           oldValues: valRight,
                                                           newValues: newVals,
                                                           cn: cn,
                                                           tran: tran);
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tran.Rollback(); }
                        catch { }
                        MessageBox.Show(this, "������ ���������� ������� ����������:\r\n" + ex.Message);
                    }
                }

                RefreshData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SetNextClimber();
        }

        protected override void SetNextClimber()
        {
            SetNext();
            SetEditMode(true);
        }
        private void CalculateStartersToSet(out int left, out int right)
        {
            int frst2;
            CalculateStartersToSet(listID, cn, out left, out right, out frst2);
        }
        public static void CalculateStartersToSet(int listID, SqlConnection cn, out int left, out int right, out int firstInSecondGroup)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM speedResults (NOLOCK)" +
                              " WHERE list_id=" + listID.ToString() +
                              "   AND preQf = 0";
            int cnt = (int)cmd.ExecuteScalar();
            firstInSecondGroup = 1 + (cnt / 2) + (cnt % 2);

            cmd.CommandText = "SELECT MIN(start) start" +
                              "  FROM speedResults (NOLOCK)" +
                              " WHERE list_id=" + listID.ToString() +
                              "   AND preQf = 0" +
                              "   AND ISNULL(route1_text,'') = '' " +
                              "   AND (CHARINDEX('�/�', resText) < 1" +
                              "    OR resText IS NULL)";
            object oTmp = cmd.ExecuteScalar();
            if (oTmp != null && oTmp != DBNull.Value)
                left = Convert.ToInt32(oTmp); //���, ��� ������ ������� �� 1�� ������
            else
                left = 0;
            right = GetNextClimberOnRightRoute(listID, firstInSecondGroup, cmd.Connection);
        }

        private static int GetNextClimberOnRightRoute(int listID, int firstInGroup, SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();
            cmd.CommandText = "SELECT MIN(start) strt" +
                              "  FROM speedResults (NOLOCK)" +
                              " WHERE list_id=" + listID.ToString() +
                              "   AND preQf = 0" +
                              "   AND ISNULL(route2_text,'') = '' " +
                              "   AND (CHARINDEX('�/�', resText) < 1" +
                              "    OR resText IS NULL)" +
                              "   AND start >= @sTmp";
            cmd.Parameters.Add("@sTmp", SqlDbType.Int);
            cmd.Parameters[0].Value = firstInGroup;
            object oTmpo = cmd.ExecuteScalar();
            if (oTmpo != null && oTmpo != DBNull.Value)
                return Convert.ToInt32(oTmpo);
            cmd.Parameters[0].Value = -1;
            oTmpo = cmd.ExecuteScalar();
            if (oTmpo != null && oTmpo != DBNull.Value)
                return Convert.ToInt32(oTmpo);
            else
                return 0;
        }

        private void SetNext()
        {
            try
            {
                RefreshData();
                bool sLoaded = false, fLoaded = false;
                int nR = -1, nL = -1;
                if ((Rules & SpeedRules.IFSC_WR) == SpeedRules.IFSC_WR)
                    CalculateStartersToSet(out nL, out nR);
                foreach (DataRow dr in dtStart.Rows)
                {
                    if (nR < 0 && dr["ps"].ToString() == "0" || nR > 0 && (int)dr["��.�"] == nR)
                    {
                        if (!sLoaded)
                        {
                            sLoaded = true;
                            lblStart.Text = dr["��.�"].ToString();
                            lblIID.Text = dr["�"].ToString();
                            lblName.Text = dr["����������"].ToString();
                            lblAge.Text = dr["�.�."].ToString();
                            lblQf.Text = dr["������"].ToString();
                            lblTeam.Text = dr["�������"].ToString();
                            rightRoute1.Text = dr["������ 1"].ToString();
                        }
                    }
                    else if (!fLoaded && nL < 0 || nL > 0 && (int)dr["��.�"] == nL)
                    {
                        leftStart.Text = dr["��.�"].ToString();
                        leftIID.Text = dr["�"].ToString();
                        leftName.Text = dr["����������"].ToString();
                        leftAge.Text = dr["�.�."].ToString();
                        leftQf.Text = dr["������"].ToString();
                        leftTeam.Text = dr["�������"].ToString();
                        leftRoute2.Text = dr["������ 2"].ToString();
                        fLoaded = true;
                    }
                    if (fLoaded && sLoaded)
                        break;
                }
                if (!sLoaded)
                {
                    lblStart.Text = "";
                    lblIID.Text = "";
                    lblName.Text = "";
                    lblAge.Text = "";
                    lblQf.Text = "";
                    lblTeam.Text = "";
                    rightRoute1.Text = "";
                }
                if (!fLoaded)
                {
                    leftStart.Text = "";
                    leftIID.Text = "";
                    leftName.Text = "";
                    leftAge.Text = "";
                    leftQf.Text = "";
                    leftTeam.Text = "";
                    leftRoute2.Text = String.Empty;
                }
                leftRoute1.Text = /*leftRoute2.Text =*/ leftSum.Text = rightRoute2.Text = rightSum.Text = "";
            }
            finally
            {
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
        }

        private bool EnterFirstRoute(bool swapRoutes = false)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ��������� ���������� � ��\r\n" + ex.Message);
                return false;
            }
            string route1 = leftRoute1.Text;
            if (route1.Length < 1)
            {
                string cNme = StaticClass.GetClimberName(leftIID.Text, this.cn);
                if (cNme.Length < 1)
                    return false;
                if (MessageBox.Show("�� ������������� ������ ������� ��������� 1�� ������ � ��������� " +
                    cNme + "?", "������� ���������", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                    DialogResult.No)
                    return false;
                return ResultListSpeed.ClearClimber(1, leftIID.Text, this.listID, cn);
            }
            long r1 = ResultListSpeed.ParseString(ref route1);
            leftRoute1.Text = route1;
            SqlCommand cmd = new SqlCommand();
            if (r1 >= 6000000 && (!UseBestRoute || route1.ToLower().IndexOf("�/�") > -1))
            {
                string rs2 = "";
                string rsSum = route1;
                string toSet1, toSet2;
                if (swapRoutes)
                {
                    toSet1 = rs2;
                    toSet2 = route1;
                }
                else
                {
                    toSet1 = route1;
                    toSet2 = rs2;
                }
                return EnterBothRoutes(ref toSet1, ref toSet2, ref rsSum, leftIID.Text);
            }
            string sRouteN = swapRoutes ? "route2" : "route1";
            cmd.CommandText = "UPDATE speedResults SET " + sRouteN + "=@r1," + sRouteN + "_text=@r1t ";
            if (UseBestRoute)
                cmd.CommandText += ",res=@r1, resText=@r1t ";
            cmd.CommandText+=
                " WHERE (list_id=" + listID.ToString() + ") AND (climber_id=" + leftIID.Text + ")";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@r1", SqlDbType.BigInt);
            cmd.Parameters[0].Value = r1;
            cmd.Parameters.Add("@r1t", SqlDbType.VarChar);
            cmd.Parameters[1].Value = route1;

            try
            {
                cmd.Connection = cn;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ����� ���������� �� 1 ������\r\n" + ex.Message);
                return false;
            }
            sRouteN = swapRoutes ? "Tmp" : String.Empty;
            cmd.CommandText = "UPDATE lists SET nowClimbing" + sRouteN + " = NULL WHERE iid = " + listID.ToString();
            try { cmd.ExecuteNonQuery(); }
            catch { }
            return true;
        }

        private bool EnterBothRoutes(Label lIid, TextBox r1l, TextBox r2l, TextBox suml)
        {
            string r1 = r1l.Text;
            string r2 = r2l.Text;
            string rSum = suml.Text;
            bool bRes = EnterBothRoutes(ref r1, ref r2, ref rSum, lIid.Text);
            if (bRes)
            {
                r1l.Text = r1;
                r2l.Text = r2;
                suml.Text = rSum;
            }
            return bRes;
        }

        private bool EnterBothRoutes()
        {
            return EnterBothRoutes(lblIID, rightRoute1, rightRoute2, rightSum);
        }

        private bool EnterBothRoutes(ref string route1, ref string route2, ref string sum, string climberID)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
            string cNme = StaticClass.GetClimberName(climberID, this.cn);
            if (cNme.Length < 1)
                return false;
            if (route1.Length > 0 && route2.Length == 0 && sum.Length == 0)
            {
                if (MessageBox.Show("�� ������������� ������ ������� ��������� 2�� ������ � ��������� " +
                    cNme + "?", "������� ���������", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                    DialogResult.No)
                    return false;
                return ResultListSpeed.ClearClimber(2, climberID, listID, cn);
            }

            //string route1 = tbRoute1.Text, route2 = tbRoute2.Text, sum = tbSum.Text;
            long r1 = 0, r2 = 0, nSum = 0;

            if ((!String.IsNullOrEmpty(route1) || UseBestRoute) && !String.IsNullOrEmpty(route2))
            {
                r1 = ResultListSpeed.ParseString(ref route1);
                //if (r1 >= 6000000)
                //    return EnterFirstRoute();
                r2 = ResultListSpeed.ParseString(ref route2, true);
                if (UseBestRoute)
                {
                    nSum = Math.Min(r1, r2);
                    sum = (r1 < r2) ? route1 : route2;
                }
                else if (r2 < 6000000)
                {
                    nSum = r1 + r2;
                    sum = ResultListSpeed.CreateString(nSum / 100);
                }
                else
                {
                    if (baseIID > 0)
                        r2 = 6000000;
                    nSum = r1 + r2;
                    sum = "*" + route1;
                }
            }
            else
            {
                bool b = false;
                if (route2 != "" && sum != "")
                {
                    route1 = route2;
                    b = true;
                }

                if (route1 != "" && sum != "")
                {
                    r1 = ResultListSpeed.ParseString(ref route1);
                    nSum = ResultListSpeed.ParseString(ref sum, true);
                    if (r1 < 6000000)
                    {
                        if (nSum < 6000000)
                        {
                            r2 = nSum - r1;
                            route2 = ResultListSpeed.CreateString(r2 / 100);
                        }
                        else
                        {
                            if (baseIID > 0)
                                nSum = 6000000;
                            r2 = nSum;
                            route2 = sum;
                            nSum = r1 + r2;
                            sum = "*" + route1;
                        }
                    }
                    else
                    {
                        if (baseIID > 0)
                            r1 = 6000000;
                        nSum = 2 * r1;
                        r2 = r1;
                        sum = route1;
                        route2 = "";
                    }
                    if (b)
                    {
                        long nTmp = r1;
                        string rTmp = route1;
                        route1 = route2;
                        r1 = r2;
                        route2 = rTmp;
                        r2 = nTmp;
                    }
                }
            }

            /*tbRoute1.Text = route1;
            tbRoute2.Text = route2;
            tbSum.Text = sum;*/

            if (!UseBestRoute && (nSum < r1 || nSum < r2))
            {
                MessageBox.Show("��������� ������������ ����� �����");
                return false;
            }

            int prevPos = 0;
            cmd.Parameters.Clear();
            //if (baseIID > 0)
            //{
            //    cmd.CommandText = "SELECT pos FROM speedResults WHERE (list_id=" + baseIID.ToString() + ") AND (climber_id=" +
            //        climberID + ")";
            //    try { prevPos = Convert.ToInt32(cmd.ExecuteScalar()); }
            //    catch { prevPos = 99; }
            //}

            cmd.CommandText = "UPDATE speedResults SET res=@res,route1=@r1,route1_text=@r1t,route2=@r2," +
                "route2_text=@r2t, resText=@rt WHERE (list_id=" + listID.ToString() + ") AND (climber_id=" + climberID + ")";

            cmd.Parameters.Add("@res", SqlDbType.BigInt);

            cmd.Parameters[0].Value = nSum + prevPos;
            cmd.Parameters.Add("@r1", SqlDbType.BigInt);
            cmd.Parameters[1].Value = r1;
            cmd.Parameters.Add("@r2", SqlDbType.BigInt);
            cmd.Parameters[2].Value = r2;
            cmd.Parameters.Add("@rt", SqlDbType.VarChar);
            cmd.Parameters[3].Value = sum;
            cmd.Parameters.Add("@r1t", SqlDbType.VarChar);
            cmd.Parameters[4].Value = route1;
            cmd.Parameters.Add("@r2t", SqlDbType.VarChar);
            cmd.Parameters[5].Value = route2;

            try
            {
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET nowClimbing3 = " + climberID + ",nowClimbingTmp = NULL WHERE iid = " + listID.ToString();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void cbNxtRound_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnNextRound.Enabled = (cbNxtRound.SelectedIndex > -1);
        }

        protected override void dgStart_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (leftRoute1.Enabled || rightRoute2.Enabled)
                    return;
                int curRow = e.RowIndex;
                if (curRow >= dtStart.Rows.Count)
                    curRow = -1;
                if (curRow < 0)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            SetEmpty(false);
                            break;
                        case MouseButtons.Right:
                            SetEmpty(true);
                            break;
                    }
                    return;
                }
                DataRow dr = dtStart.Rows[curRow];
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (dr["�"].ToString() == lblIID.Text)
                            SetEmpty(true);
                        leftAge.Text = dr["�.�."].ToString();
                        leftIID.Text = dr["�"].ToString();
                        leftName.Text = dr["����������"].ToString();
                        leftQf.Text = dr["������"].ToString();
                        leftRoute1.Text = dr["������ 1"].ToString();
                        leftRoute2.Text = dr["������ 2"].ToString();
                        leftStart.Text = dr["��.�"].ToString();
                        leftTeam.Text = dr["�������"].ToString();
                        break;
                    case MouseButtons.Right:
                        if (dr["������ 1"].ToString().Length == 0 && (Rules & SpeedRules.BestRouteInQfRound) != SpeedRules.BestRouteInQfRound)
                        {
                            MessageBox.Show("���� �������� ��� �� ��� �� 1 ������.");
                            return;
                        }
                        if (dr["�"].ToString() == leftIID.Text)
                            SetEmpty(false);
                        lblAge.Text = dr["�.�."].ToString();
                        lblIID.Text = dr["�"].ToString();
                        lblName.Text = dr["����������"].ToString();
                        lblQf.Text = dr["������"].ToString();
                        rightRoute1.Text = dr["������ 1"].ToString();
                        rightRoute2.Text = dr["������ 2"].ToString();
                        lblStart.Text = dr["��.�"].ToString();
                        lblTeam.Text = dr["�������"].ToString();
                        break;
                }
            }
            finally
            {
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
        }

        protected override void dgRes_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (leftRoute1.Enabled || rightRoute2.Enabled)
                    return;
                int curRow = e.RowIndex;
                if (curRow >= dtRes.Rows.Count)
                    curRow = -1;
                if (curRow < 0)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            SetEmpty(false);
                            break;
                        case MouseButtons.Right:
                            SetEmpty(true);
                            break;
                    }
                    return;
                }
                DataRow dr = dtRes.Rows[curRow];
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (dr["�"].ToString() == lblIID.Text)
                            SetEmpty(true);
                        leftAge.Text = dr["�.�."].ToString();
                        leftIID.Text = dr["�"].ToString();
                        leftName.Text = dr["����������"].ToString();
                        leftQf.Text = dr["������"].ToString();
                        leftRoute1.Text = dr["������ 1"].ToString();
                        leftRoute2.Text = dr["������ 2"].ToString();
                        leftSum.Text = dr["�����"].ToString();
                        leftStart.Text = dr["��.�"].ToString();
                        leftTeam.Text = dr["�������"].ToString();
                        break;
                    case MouseButtons.Right:
                        if (dr["������ 1"].ToString().Length == 0 && (Rules & SpeedRules.BestRouteInQfRound) != SpeedRules.BestRouteInQfRound )
                        {
                            MessageBox.Show("���� �������� ��� �� ��� �� 1 ������.");
                            return;
                        }
                        if (dr["�"].ToString() == leftIID.Text)
                            SetEmpty(false);
                        lblAge.Text = dr["�.�."].ToString();
                        lblIID.Text = dr["�"].ToString();
                        lblName.Text = dr["����������"].ToString();
                        lblQf.Text = dr["������"].ToString();
                        rightRoute1.Text = dr["������ 1"].ToString();
                        rightRoute2.Text = dr["������ 2"].ToString();
                        rightSum.Text = dr["�����"].ToString();
                        lblStart.Text = dr["��.�"].ToString();
                        lblTeam.Text = dr["�������"].ToString();
                        break;
                }
            }
            finally
            {
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
        }

        protected override void ds_SetOneRouteEvent(object sender, SetOneRouteResultEventArgs e)
        {
            if (sender == null || !(sender is DataFromSystem))
                return;
            DataFromSystem ds = (DataFromSystem)sender;
            if (e.Route == 1)
            {
                leftRoute1.Text = e.Res;
                if (EnterFirstRoute())
                {
                    SetEditMode(false);
                    ResultListSpeed.RefreshTable(listID, baseIID, RefreshData, cn, CR);
                    fromEvent = true;
                    try { SetNextClimber(); }
                    finally { fromEvent = false; }
                    if (leftIID.Text.Length > 0 && leftName.Text.Length > 0)
                        ds.Route1 = leftIID.Text + " " + leftName.Text + " (��.� " + leftStart.Text + ")";
                    else
                        ds.Route1 = "";
                }
            }
            else if (e.Route == 2 && IFSC_WR && (e.Res.IndexOf("�/�") > -1))
            {
                string iidLeft = leftIID.Text,
                    resLeft = leftRoute1.Text,
                    res2left = leftRoute2.Text,
                    sumLeft = leftSum.Text;
                
                leftIID.Text = lblIID.Text;
                leftRoute1.Text = e.Res;
                if (EnterFirstRoute(true))
                {
                    SetEditMode(false);
                    ResultListSpeed.RefreshTable(listID, baseIID, RefreshData, cn, CR);
                    fromEvent = true;
                    try { SetNextClimber(); }
                    finally { fromEvent = false; }
                    if (!String.IsNullOrEmpty(lblIID.Text) && !String.IsNullOrEmpty(lblName.Text))
                        ds.Route2 = lblIID.Text + " " + lblName.Text + " (��.� " + lblStart.Text + ")";
                    else
                        ds.Route2 = String.Empty;
                }
            }
        }

        private void SpeedSystem_Load(object sender, EventArgs e)
        {
            if(needToClose)
                try { this.Close(); }
                catch { }
        }

        protected override void SetTransactionButtonsEnabled(SqlTransaction tran = null)
        {
            long? l1, l2;
            SetTransactionButtonsEnabled(out l1, out l2);
            SetTransactionSecondButtonsEnabled(out l1, out l2);
        }

        private void SetTransactionSecondButtonsEnabled(out long? activeTranID, out long? nextTranID, SqlTransaction tran = null)
        {
            SetTransactionButtonsEnabled(lblIID, btnRollBackSecond, btnRestoreSecond, out activeTranID, out nextTranID, cn, listID, tran);
        }

        protected override void SetTransactionButtonsEnabled(out long? activeTranID, out long? nextTranID, SqlTransaction tran = null)
        {
            SetTransactionButtonsEnabled(leftIID, btnRollBack, btnRestore, out activeTranID, out nextTranID,
                cn, listID, tran);
        }

        private static void SetTransactionButtonsEnabled(Control labelControl, Control rollback, Control restore,
            out long? activeTranID, out long? nextTranID, SqlConnection cn, int listID, SqlTransaction tran = null)
        {
            activeTranID = null;
            nextTranID = null;
            rollback.Enabled = restore.Enabled = false;
            int clmID;
            if (!int.TryParse(labelControl.Text, out clmID))
                return;
            ResultListSpeed.GetSpeedTransactions(clmID, listID, out activeTranID, out nextTranID, cn, tran);
            rollback.Enabled = (activeTranID != null && activeTranID.HasValue);
            restore.Enabled = (nextTranID != null && nextTranID.HasValue);
        }

        protected override void btnRollBack_Click(object sender, EventArgs e)
        {
            int climberID;
            string climberName;
            TransactionAction action;
            byte routeToClean;
            if (sender == btnRollBack || sender == btnRestore)
            {
                if (!int.TryParse(leftIID.Text, out climberID))
                    return;
                climberName = leftName.Text;
                action = (sender == btnRollBack) ? TransactionAction.ROLLBACK : TransactionAction.RESTORE;
                routeToClean = 1;
            }
            else if (sender == btnRollBackSecond || sender == btnRestoreSecond)
            {
                if (!int.TryParse(lblIID.Text, out climberID))
                    return;
                climberName = lblName.Text;
                action = (sender == btnRollBackSecond) ? TransactionAction.ROLLBACK : TransactionAction.RESTORE;
                routeToClean = 2;
            }
            else
            {
                MessageBox.Show(this, "����������� ������ - �� ������ �������� �������");
                return;
            }
            DoTranAction(climberID, climberName, action, routeToClean);
        }

        private enum TransactionAction { ROLLBACK, RESTORE }
        private void DoTranAction(int climberID, string climberName, TransactionAction action, byte routeToClean)
        {
            long? activeTranID, nextTranID, toCheck;
            try
            {
                ResultListSpeed.GetSpeedTransactions(climberID, listID, out activeTranID, out nextTranID, cn);
                toCheck = (action == TransactionAction.ROLLBACK) ? activeTranID : nextTranID;
                string message = (action == TransactionAction.ROLLBACK) ? "��������" : "�������";
                if (toCheck == null || !toCheck.HasValue)
                {
                    MessageBox.Show(this, "���������� " + message + " ���������");
                    return;
                }
                if (MessageBox.Show(this, "�� �������, ��� ������ " + message + " ��������� �������� ��������� (" +
                    climberName + ")?", message + " ���������?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    return;
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    bool res;
                    if (action == TransactionAction.ROLLBACK)
                        res = StaticClass.RollbackTransaction(toCheck.Value, cn, tran);
                    else
                        res = StaticClass.RestoreLogicTransaction(toCheck.Value, cn, tran);
                    if (res)
                    {
                        ResultListSpeed.RefreshTable(listID, baseIID, RefreshData, cn, CR, tran, false);
                        tran.Commit();
                        RefreshData();
                        ClearClimberOnRoute(routeToClean);
                    }
                    else
                    {
                        MessageBox.Show("���������� ������� ����������� ���������� ������� SQL Server");
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
            catch (Exception ex) { MessageBox.Show("������ ���������� �������:\r\n" + ex.Message); }
        }
    }
}