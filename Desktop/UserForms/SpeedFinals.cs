// <copyright file="SpeedFinals.cs">
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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Windows.Forms;
using UserForms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Threading;
using ClimbingCompetition.SpeedData;

namespace ClimbingCompetition
{
    /// <summary>
    /// Ввод результатов финалов скорости
    /// родитель для формы ввода результатов квалификации скорости 
    /// в режиме считывания данных со скоростной системы
    /// </summary>
    public partial class SpeedFinals : BaseListForm
    {
        protected SettingsForm.SpeedSystemType currentSystem;
        private readonly bool speedAdvancedSystemVar;

        protected bool speedAdvancedSystem { get { return this.speedAdvancedSystemVar && (this.GetType() == typeof(SpeedFinals)); } }

        private bool IsLimitedFinal = false;
        protected long leftRoute = 0;
        //protected SqlConnection cn;
        protected SqlCommand cmd;
        protected int judgeID = 0;
        protected SqlDataAdapter daStart, daResQ, daResF, daResW, daResL;
        protected DataTable dtStart = new DataTable(), dtRes = new DataTable();
        protected bool seconRoute = false, finals, allowEvent = false, semi = false;
        protected SystemListener sl = null;
        private int climberCount = 0;
        private int? runNo = null;
        protected int? RunNo { get { return runNo; } set { runNo = value; } }


        protected SpeedRules Rules;

        protected SpeedFinals()
            : base(new SqlConnection(), "",EMPTY_LIST)
        {
            InitializeComponent();
            this.ToolTipDefault.SetToolTip(btnRollBack, ResultListLead.RES_ROLLBACK_STRING);
            this.ToolTipDefault.SetToolTip(btnRestore, ResultListLead.RES_RESTORE_STRING);
        }
        public SpeedFinals(SqlConnection baseCon, string compTitle, int listID) : base(baseCon, compTitle, listID)
        {
            AccountForm.CreateSpeedAdvancedFormatTable(cn);
            
            InitializeComponent();
            for (int i = 0; i < tmpData.Length; i++)
                tmpData[i] = "";
            SetEmpty(true);
            SetEmpty(false);
            this.ToolTipDefault.SetToolTip(btnRollBack, ResultListLead.RES_ROLLBACK_STRING);
            this.ToolTipDefault.SetToolTip(btnRestore, ResultListLead.RES_RESTORE_STRING);
            this.Rules = SettingsForm.GetSpeedRules(cn);
            this.speedAdvancedSystemVar = (this.Rules & SpeedRules.SpeedAdvancedSystem) == SpeedRules.SpeedAdvancedSystem;
            if (!this.speedAdvancedSystem)
                gbWorkMode.Visible = gbWorkMode.Enabled = false;
            //if (this.speedAdvancedSystem)
            //{
            //    cbShowView.SelectedIndex = 0;
            //    if (this.GetType() == typeof(SpeedFinals))
            //    {
            //        cbShowView.Items.Add("1 забег");
            //        cbShowView.Items.Add("2 забег");
            //        cbShowView.Items.Add("3 забег");
            //    }
            //}
            //else
            //    cbShowView.Visible = cbShowView.Enabled = false;
        }

        protected bool IsBestQualy()
        {
            try
            {
                if(!this.GetType().Equals(typeof(SpeedSystem)))
                    return false;
                return (Rules & SpeedRules.BestRouteInQfRound) == SpeedRules.BestRouteInQfRound;
            }
            catch { return false; }
        }

        void SpeedFinals_Load(object sender, EventArgs e)
        {
            if (needToClose)
            {
                try { this.Close(); }
                catch { }
                return;
            }
            try
            {
                if (cn == null || cn.ConnectionString == null || cn.ConnectionString == "")
                    currentSystem = SettingsForm.SpeedSystemType.NON;
                else
                    currentSystem = SettingsForm.GetCurrentSystem(cn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно загрузить тип используемой системы:\r\n" + ex.Message);
                currentSystem = SettingsForm.SpeedSystemType.NON;
            }
            if (currentSystem == SettingsForm.SpeedSystemType.NON)
            {
                cbUseSystem.Checked = false;
                tbComPort.Enabled = btnShowSystem.Enabled = btnConfigPort.Enabled =
                    cbUseSystem.Enabled = cbAutoSystem.Enabled = false;
                return;
            }
            switch (currentSystem)
            {
                case SettingsForm.SpeedSystemType.IVN:
                    tbComPort.Enabled = false;
                    btnConfigPort.Text = "Настроить систему";
                    break;
                case SettingsForm.SpeedSystemType.EKB:
                    tbComPort.Enabled = true;
                    btnConfigPort.Text = "Настроить порт";
                    break;
            }
        }
        public SpeedFinals(int listID, SqlConnection baseCon, string competitionTitle, bool finals)
            : this(baseCon, competitionTitle,listID)
        {
            rbInternational.Visible = rbInternational.Checked = (Rules & SpeedRules.InternationalSchema) == SpeedRules.InternationalSchema;
            rbNational.Checked = !rbInternational.Checked;
            rbNational.Visible = rbNational.Checked;
            this.finals = finals;
            //try
            //{
            //    if (cn.State != ConnectionState.Open)
            //        cn.Open();
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
            //this.cn = cn;
            cmd = new SqlCommand();
            cmd.Connection = cn;

            this.runNo = null;
            CreateDataAdapters();
            
            string strround = "";
            cmd.CommandText = "SELECT round, judge_id FROM lists(NOLOCK) WHERE iid=" + listID.ToString();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                try
                {
                    if (rd.Read())
                    {
                        strround = rd["round"].ToString();
                        btnNextRound.Enabled = (strround != "Финал");
                        semi = (rd["round"].ToString().IndexOf("1/2") > -1);
                        try { judgeID = Convert.ToInt32(rd["judge_id"]); }
                        catch { judgeID = 0; }
                    }
                }
                finally { rd.Close(); }

                //btnNextRound.Enabled = (cmd.ExecuteScalar().ToString() != "Финал");
            }
            catch { }
            this.Text = StaticClass.GetListName(listID, cn);
            cbPrevRound.Enabled = (this.Text.IndexOf("Финал") > -1) || (this.Text.IndexOf("1/2 финала") > -1);

            cmd.CommandText = "SELECT COUNT(*) FROM speedResults(NOLOCK) WHERE list_id = " + listID.ToString();
            try
            {
                int tmp = Convert.ToInt32(cmd.ExecuteScalar());
                IsLimitedFinal = (tmp < 3);
                int tmpCn = 1;
                while ((tmpCn *= 2) < tmp) ;
                climberCount = tmpCn;
            }
            catch { IsLimitedFinal = false; }
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
            {
                if (strround == "Финал")
                    btnNextRound.Enabled = false;
                else
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                    try { btnNextRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                    catch { btnNextRound.Enabled = true; }
                }
            }
            else
                btnNextRound.Enabled = false;
        }

        private void CreateDataAdapters()
        {
            if (this.RunNo == null)
            {
                #region обычные_адаптеры
                daStart = new SqlDataAdapter(
                    new SqlCommand("SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+" +
                        "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " +
                        "l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2], l.resText AS [Сумма],l.qf AS [Кв.] " +
                        "FROM speedResults l(NOLOCK) INNER JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                        "WHERE (l.list_id = " + listID.ToString() +
                        ") AND (l.resText IS NULL) ORDER BY l.start", this.cn));
                CreateWinAdapters();
                daResF = new SqlDataAdapter(
                    new SqlCommand("SELECT l.start AS [Ст.№],l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                        "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда],  " +
                        "l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2], l.resText AS [Сумма], " +
                        "l.qf AS [Кв.],l.res FROM speedResults l(NOLOCK) INNER JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                        "WHERE (l.list_id = " + listID.ToString() +
                        ") AND (l.resText IS NOT NULL) AND (((l.qf<>'Q')AND(l.qf<>' q')) OR (l.qf IS NULL)) ORDER BY l.pos", this.cn));
                daResL = new SqlDataAdapter(
                    new SqlCommand("SELECT l.start AS [Ст.№],l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                        "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда],  " +
                        "l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2], l.resText AS [Сумма], " +
                        "l.qf AS [Кв.],l.res FROM speedResults l(NOLOCK) INNER JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                        "WHERE (l.list_id = " + listID.ToString() +
                        ") AND (l.resText IS NOT NULL) AND ((l.qf<>'Q') OR (l.qf IS NULL)) ORDER BY l.res, l.pos", this.cn));
                daResF.UpdateCommand = new SqlCommand(
                    "UPDATE speedResults SET pos=@pos,posText=@pt,qf=@qf WHERE (climber_id=@id) AND (list_id=" +
                    listID.ToString() + ")", this.cn);
                daResF.UpdateCommand.Parameters.Add("@pos", SqlDbType.Int, 4, "pos");
                daResF.UpdateCommand.Parameters.Add("@pt", SqlDbType.VarChar, 50, "Место");
                daResF.UpdateCommand.Parameters.Add("@qf", SqlDbType.VarChar, 50, "Кв.");
                daResF.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 4, "№");
                #endregion
            }
            else
            {
                daStart = new SqlDataAdapter();
                daStart.SelectCommand = new SqlCommand();
                daStart.SelectCommand.Connection = cn;
                daStart.SelectCommand.CommandText =
                    "SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+ p.name) AS [Фамилия, Имя], p.age AS [Г.р.]," +
                    "       p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " +
                    "       CASE sr.start2route when 0 then sr.res_text else '-' end AS [Трасса 1]," +
                    "       CASE sr.start2route when 1 then sr.res_text else '-' end AS [Трасса 2]," +
                    "       l.resText AS [Сумма], l.qf AS [Кв.] " +
                    "  FROM speedResults l(NOLOCK)" +
                    "  JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "  JOIN speedResults_Runs sr(NOLOCK) ON sr.iid_parent = l.iid" +
                    " WHERE l.list_id = " + listID.ToString() +
                    "   AND sr.res_text IS NULL" +
                    "   AND sr.run_no = " + this.runNo.Value.ToString() +
                 " ORDER BY l.start";
                daResQ = null;
                daResF = new SqlDataAdapter();
                daResF.SelectCommand = new SqlCommand();
                daResF.SelectCommand.Connection = cn;
                daResF.SelectCommand.CommandText=
                    "SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+ p.name) AS [Фамилия, Имя], p.age AS [Г.р.]," +
                    "       p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " +
                    "       CASE sr.start2route when 0 then sr.res_text else '-' end AS [Трасса 1]," +
                    "       CASE sr.start2route when 1 then sr.res_text else '-' end AS [Трасса 2]," +
                    "       l.resText AS [Сумма], l.qf AS [Кв.] " +
                    "  FROM speedResults l(NOLOCK)" +
                    "  JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "  JOIN speedResults_Runs sr(NOLOCK) ON sr.iid_parent = l.iid" +
                    " WHERE l.list_id = " + listID.ToString() +
                    "   AND sr.res_text IS NOT NULL" +
                    "   AND sr.run_no = " + this.runNo.Value.ToString() +
                 " ORDER BY l.start";
            }
            dgStart.DataSource = dtStart;
            dgRes.DataSource = dtRes;
            RefreshData();
            SetCurrentPair();
            SetEditMode(false);
        }

        //меняем забег на форме и заодно создаём его протокол
        bool inChangeRun = false;
        private void ChangeRunNo(int? newRunNo)
        {
            if (inChangeRun)
                return;
            try
            {
                inChangeRun = true;
                ChangeRun(newRunNo);
            }
            finally { inChangeRun = false; }
        }

        protected virtual void ChangeRun(int? newRunNo)
        {
            if (newRunNo == null || !newRunNo.HasValue || newRunNo.Value < 1)
            {
                this.runNo = null;
                CreateDataAdapters();
                return;
            }
            SqlCommand cmdCheck = new SqlCommand(), cmdInsert = new SqlCommand(), cmdUpdate = new SqlCommand();
            cmdCheck.Connection = cmdInsert.Connection = cmdUpdate.Connection = cn;
            if (newRunNo.Value > 1)
            {
                cmdCheck.CommandText = "SELECT COUNT(*) cnt" +
                                       "  FROM speedResults S(nolock)" +
                                       "  JOIN speedResults_Runs SR(nolock) on SR.iid_parent = S.iid" +
                                       " WHERE S.list_id = " + this.listID.ToString() +
                                       "   AND SR.run_no < " + newRunNo.Value.ToString() +
                                       "   AND SR.res_text is null";
                object oTmp = cmdCheck.ExecuteScalar();
                bool disallow = (oTmp != null && oTmp != DBNull.Value && Convert.ToInt32(oTmp) > 0);
                if (!disallow)
                {
                    cmdCheck.CommandText = "SELECT COUNT(*) cnt" +
                                           "  FROM speedResults S(nolock)" +
                                           "  JOIN speedResults_Runs SR(nolock) on SR.iid_parent = S.iid" +
                                           " WHERE S.list_id = " + this.listID.ToString() +
                                           "   AND SR.run_no = " + (newRunNo.Value - 1).ToString();
                    oTmp = cmdCheck.ExecuteScalar();
                    disallow = (oTmp == null || oTmp == DBNull.Value || Convert.ToInt32(oTmp) < 1);
                }
                if (disallow)
                {
                    MessageBox.Show(this, "Есть незаконченные предыдущие забеги.");
                    if (runNo == null || runNo.Value<1)
                    {
                        rbGeneral.Checked = true;
                        //cbShowView.SelectedIndex = 0;
                    }
                    else
                    {
                        switch (runNo.Value)
                        {
                            case 1:
                                rb1stRun.Checked = true;
                                break;
                            case 2:
                                rb2ndRun.Checked = true;
                                break;
                            default:
                                rb3rdRun.Checked = true;
                                break;
                        }
                        //cbShowView.SelectedIndex = runNo.Value;
                    }
                    return;
                }
            }

            cmdUpdate.CommandText = "SELECT COUNT(*) cnt" +
                                   "  FROM speedResults SR(nolock)" +
                                   " WHERE SR.list_id = " + this.listID.ToString();
            object o = cmdUpdate.ExecuteScalar();
            int nCnt;
            if (o == null || o == DBNull.Value)
                nCnt = 0;
            else
                nCnt = Convert.ToInt32(o);
            if (nCnt < 1)
            {
                MessageBox.Show("В протоколе нет участников");
                return;
            }
            int total2Pow = 1;
            while (total2Pow < nCnt)
                total2Pow *= 2;

            List<int> climbersToRun = new List<int>();
            if (newRunNo.Value < 3)
                for (int i = 1; i <= (total2Pow); i += 2)
                    climbersToRun.Add(i);
            else
            {
                cmdCheck.CommandText = @"
select distinct SR.start
  from speedResults SR(nolock)
  join speedResults_Runs R1(nolock) on R1.iid_parent = SR.iid
  join speedResults_Runs R2(nolock) on R2.iid_parent_comp = SR.iid and R2.run_no = R1.run_no
  join speedResults SR2(nolock) on SR2.iid = R2.iid_parent
 where SR.start % 2 = 1
   and SR.list_id = " + this.listID.ToString() + @"
   and R1.run_no = " + (newRunNo.Value - 1).ToString() + @"
   and SR2.res = SR.res
order by SR.start
";
                using (var rdr = cmdCheck.ExecuteReader())
                {
                    while (rdr.Read())
                        climbersToRun.Add(Convert.ToInt32(rdr["start"]));
                }
            }

            cmdCheck.CommandText =
                       "SELECT r.iid, sr.iid iParent" +
                       "  FROM speedResults sr(nolock)" +
                   " LEFT JOIN speedResults_Runs r(nolock) on r.iid_parent = sr.iid and r.run_no = " + newRunNo.Value.ToString() +
                       " WHERE sr.list_id = " + this.listID.ToString() +
                       "   AND sr.start = @start";
            cmdCheck.Parameters.Add("@start", SqlDbType.Int);

            cmdInsert.CommandText = "INSERT INTO speedResults_Runs(iid, iid_parent, iid_parent_comp, run_no, start2route) " +
                        "VALUES(@iid, @iid_parent, @iid_parent_comp, " + newRunNo.Value.ToString() + ",@start2route)";
            cmdInsert.Parameters.Add("@iid", SqlDbType.BigInt);
            cmdInsert.Parameters.Add("@iid_parent", SqlDbType.BigInt);
            cmdInsert.Parameters.Add("@iid_parent_comp", SqlDbType.BigInt);
            cmdInsert.Parameters.Add("@start2route", SqlDbType.Bit);

            cmdUpdate.CommandText = "UPDATE speedResults_Runs SET start2route=@start2route, iid_parent_comp=@iid_parent_comp" +
                                   " WHERE iid=@iid";
            cmdUpdate.Parameters.Clear();
            cmdUpdate.Parameters.Add("@iid", SqlDbType.BigInt);
            cmdUpdate.Parameters.Add("@iid_parent_comp", SqlDbType.BigInt);
            cmdUpdate.Parameters.Add("@start2route", SqlDbType.Bit);

            SqlTransaction tran = cn.BeginTransaction();
            bool scs = false;
            try
            {
                cmdUpdate.Transaction= cmdInsert.Transaction = cmdCheck.Transaction = tran;
                long? result1, result2, run1, run2;

                List<long> insertedData = new List<long>();
                foreach (var i in climbersToRun)
                {
                    GetRunAnClimber(cmdCheck, i, out result1, out run1);
                    if (run1 != null)
                        insertedData.Add(run1.Value);
                    GetRunAnClimber(cmdCheck, (i + 1), out result2, out run2);
                    if (run2 != null)
                        insertedData.Add(run2.Value);

                    bool reverseRoutes = (newRunNo.Value == 2);


                    if (run1 == null)
                        insertedData.Add(UpdateRuns(cmdInsert, cmdUpdate, tran, result1, result2, run1, reverseRoutes));
                    if (run2 == null)
                        insertedData.Add(UpdateRuns(cmdInsert, cmdUpdate, tran, result2, result1, run2, !reverseRoutes));
                }

                insertedData.RemoveAll((a => (a <= 0)));

                if (insertedData.Count > 0)
                {
                    cmdUpdate.CommandText = "DELETE speedResults_Runs" +
                                            "  FROM speedResults_Runs SR(nolock)" +
                                            "  JOIN speedResults S(nolock) on S.iid = SR.iid_parent" +
                                            " WHERE S.list_id=" + this.listID.ToString() +
                                            "   AND SR.run_no = " + newRunNo.Value.ToString() +
                                            "   AND SR.iid NOT IN(";
                    foreach (var s in insertedData)
                        cmdUpdate.CommandText += s.ToString() + ",";
                    cmdUpdate.Parameters.Clear();
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Substring(0, cmdUpdate.CommandText.Length - 1) + ")";
                    cmdUpdate.ExecuteNonQuery();
                }

                scs = true;
            }
            finally
            {
                if (scs)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            if (scs)
            {
                this.runNo = newRunNo;

                CreateDataAdapters();
            }
        }

        private long UpdateRuns(SqlCommand cmdInsert, SqlCommand cmdUpdate, SqlTransaction tran, long? result1, long? result2, long? run1, bool secondRoute)
        {
            long retVal;
            if (result1 != null)
            {
                if (run1 != null)
                {
                    cmdUpdate.Parameters[0].Value = run1.Value;
                    cmdUpdate.Parameters[1].Value = (result2 == null) ? DBNull.Value : (object)result2.Value;
                    cmdUpdate.Parameters[2].Value = secondRoute;
                    cmdUpdate.ExecuteNonQuery();
                    retVal = run1.Value;
                }
                else
                {
                    retVal = SortingClass.GetNextIID("speedResults_Runs", "iid", cn, tran);
                    cmdInsert.Parameters[0].Value = retVal;
                    cmdInsert.Parameters[1].Value = result1.Value;
                    cmdInsert.Parameters[2].Value = (result2 == null) ? DBNull.Value : (object)result2.Value;
                    cmdInsert.Parameters[3].Value = secondRoute;
                    cmdInsert.ExecuteNonQuery();
                }
            }
            else
                retVal = -1;
            return retVal;
        }

        private static void GetRunAnClimber(SqlCommand cmdCheck, int startNo, out long? speedResultIid, out long? runIid)
        {
            cmdCheck.Parameters[0].Value = startNo;
            using (var rdr = cmdCheck.ExecuteReader())
            {
                if (rdr.Read())
                {
                    if (rdr["iid"] == null || rdr["iid"] == DBNull.Value)
                        runIid = null;
                    else
                        runIid = new long?(Convert.ToInt64(rdr["iid"]));
                    if (rdr["iParent"] == null || rdr["iParent"] == DBNull.Value)
                        speedResultIid = null;
                    else
                        speedResultIid = new long?(Convert.ToInt64(rdr["iParent"]));
                }
                else
                {
                    speedResultIid = null;
                    runIid = null;
                }
            }
        }

        private void CreateWinAdapters()
        {
            string obs;
            if (rbInternational.Checked)
                obs = " ORDER BY l.pos";
            else
                obs = " ORDER BY l.res, l.pos";
            daResQ = new SqlDataAdapter(
                new SqlCommand("SELECT l.start AS [Ст.№],l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                    "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " +
                    "l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2], l.resText AS [Сумма], " +
                    "l.qf AS [Кв.],l.res FROM speedResults l(NOLOCK) INNER JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "WHERE (l.list_id = " + this.listID.ToString() +
                    ") AND (l.resText IS NOT NULL) AND ((l.qf='Q') OR (l.qf=' q'))" + obs, this.cn));
            daResW = new SqlDataAdapter(
                new SqlCommand("SELECT l.start AS [Ст.№],l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                    "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " +
                    "l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2], l.resText AS [Сумма], " +
                    "l.qf AS [Кв.],l.res FROM speedResults l(NOLOCK) INNER JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "WHERE (l.list_id = " + this.listID.ToString() +
                    ") AND (l.resText IS NOT NULL) AND (l.qf='Q')" + obs, this.cn));
        }

        protected virtual void SetTransactionButtonsEnabled(SqlTransaction tran = null)
        {
            long? l1, l2;
            SetTransactionButtonsEnabled(out l1, out l2, tran);
        }

        protected virtual void SetTransactionButtonsEnabled(out long? activeTranID, out long? nextTranID, SqlTransaction tran = null)
        {
            activeTranID = null;
            nextTranID = null;
            btnRollBack.Enabled = btnRestore.Enabled = false;
            string toCheck = seconRoute ? lblIID.Text : leftIID.Text;
            int climberID;
            if (!int.TryParse(toCheck, out climberID))
                return;
            ResultListSpeed.GetSpeedTransactions(climberID, listID, out activeTranID, out nextTranID, cn, tran);
            btnRollBack.Enabled = (activeTranID != null && activeTranID.HasValue);
            btnRestore.Enabled = (nextTranID != null && nextTranID.HasValue);
        }

        protected virtual void SetPair(int num, bool scRoute)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                int secondIndx = -1;
                int frstIndx = -1;
                DataTable dt = new DataTable();
                daStart.Fill(dt);
                for (int i = 0; i < dtStart.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtStart.Rows[i]["Ст.№"]) == (num + 1))
                        secondIndx = i;
                    if (Convert.ToInt32(dtStart.Rows[i]["Ст.№"]) == num)
                        frstIndx = i;
                    if ((secondIndx > -1) && (frstIndx > -1))
                        break;
                }
                if (frstIndx == -1)
                {
                    dt.Clear();
                    if (daResQ != null)
                        daResQ.Fill(dt);
                    if (daResF != null)
                        daResF.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dt.Rows[i]["Ст.№"]) == (num + 1))
                            secondIndx = i;
                        if (Convert.ToInt32(dt.Rows[i]["Ст.№"]) == num)
                            frstIndx = i;
                        if ((secondIndx > -1) && (frstIndx > -1))
                            break;
                    }
                }
                leftRoute1.Text = leftRoute2.Text = leftSum.Text = rightRoute1.Text = rightRoute2.Text = rightSum.Text = "";
                if (scRoute)
                {
                    lblAge.Text = dt.Rows[frstIndx]["Г.р."].ToString();
                    lblIID.Text = dt.Rows[frstIndx]["№"].ToString();
                    lblName.Text = dt.Rows[frstIndx]["Фамилия, Имя"].ToString();
                    lblQf.Text = dt.Rows[frstIndx]["Разряд"].ToString();
                    lblStart.Text = num.ToString();
                    lblTeam.Text = dt.Rows[frstIndx]["Команда"].ToString();
                    rightRoute1.Text = dt.Rows[frstIndx]["Трасса 1"].ToString();
                    rightRoute2.Text = dt.Rows[frstIndx]["Трасса 2"].ToString();
                    rightSum.Text = dt.Rows[frstIndx]["Сумма"].ToString();

                    
                    if (secondIndx > -1)
                    {
                        leftAge.Text = dt.Rows[secondIndx]["Г.р."].ToString();
                        leftIID.Text = dt.Rows[secondIndx]["№"].ToString();
                        leftName.Text = dt.Rows[secondIndx]["Фамилия, Имя"].ToString();
                        leftQf.Text = dt.Rows[secondIndx]["Разряд"].ToString();
                        leftStart.Text = (num + 1).ToString();
                        leftTeam.Text = dt.Rows[secondIndx]["Команда"].ToString();
                        leftRoute1.Text = dt.Rows[secondIndx]["Трасса 1"].ToString();
                        leftRoute2.Text = dt.Rows[secondIndx]["Трасса 2"].ToString();
                        leftSum.Text = dt.Rows[secondIndx]["Сумма"].ToString();
                    }
                    else
                        leftStart.Text = leftTeam.Text = leftAge.Text = leftIID.Text = leftName.Text = leftQf.Text = "";
                }
                else
                {
                    leftRoute = 0;
                    leftAge.Text = dt.Rows[frstIndx]["Г.р."].ToString();
                    leftIID.Text = dt.Rows[frstIndx]["№"].ToString();
                    leftName.Text = dt.Rows[frstIndx]["Фамилия, Имя"].ToString();
                    leftQf.Text = dt.Rows[frstIndx]["Разряд"].ToString();
                    leftStart.Text = num.ToString();
                    leftTeam.Text = dt.Rows[frstIndx]["Команда"].ToString();
                    leftRoute1.Text = dt.Rows[frstIndx]["Трасса 1"].ToString();
                    leftRoute2.Text = dt.Rows[frstIndx]["Трасса 2"].ToString();
                    leftSum.Text = dt.Rows[frstIndx]["Сумма"].ToString();

                    if (secondIndx > -1)
                    {
                        lblAge.Text = dt.Rows[secondIndx]["Г.р."].ToString();
                        lblIID.Text = dt.Rows[secondIndx]["№"].ToString();
                        lblName.Text = dt.Rows[secondIndx]["Фамилия, Имя"].ToString();
                        lblQf.Text = dt.Rows[secondIndx]["Разряд"].ToString();
                        lblStart.Text = (num + 1).ToString();
                        lblTeam.Text = dt.Rows[secondIndx]["Команда"].ToString();
                        rightRoute1.Text = dt.Rows[secondIndx]["Трасса 1"].ToString();
                        rightRoute2.Text = dt.Rows[secondIndx]["Трасса 2"].ToString();
                        rightSum.Text = dt.Rows[secondIndx]["Сумма"].ToString();
                    }
                    else
                        lblStart.Text = lblTeam.Text = lblAge.Text = lblIID.Text = lblName.Text = lblQf.Text = "";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally
            {
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
        }

        protected virtual void SetCurrentPair()
        {
            try
            {
                if (seconRoute)
                {
                    int n;
                    if (int.TryParse(leftStart.Text, out n))
                        SetPair(n, seconRoute);
                    else
                        clearCLimbers();
                }
                else
                    if (dtStart.Rows.Count > 0)
                    {
                        int num = Convert.ToInt32(dtStart.Rows[0]["Ст.№"]);
                        SetPair(num, speedAdvancedSystem && runNo != null && runNo.Value > 0 && (runNo.Value % 2 == 0));
                    }
                    else
                        clearCLimbers();
                try { SetTransactionButtonsEnabled(); }
                catch { }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void clearCLimbers()
        {
            lblIID.Text = lblName.Text = lblQf.Text = lblStart.Text = lblTeam.Text = lblAge.Text =
                rightRoute1.Text = rightRoute2.Text = rightSum.Text =
                leftAge.Text = leftIID.Text = leftName.Text = leftQf.Text = leftTeam.Text =
                leftRoute1.Text = leftRoute2.Text = leftSum.Text = "";
        }

        protected virtual void RefreshData()
        {

            dtStart.Clear();
            dtRes.Clear();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                daStart.Fill(dtStart);
                if (daResQ != null)
                    daResQ.Fill(dtRes);
                if (daResF != null)
                    daResF.Fill(dtRes);
                if (dtRes.Columns.Contains("res"))
                    dtRes.Columns.Remove("res");
                if (dtRes.Columns.Contains("Ст.№"))
                    dtRes.Columns.Remove("Ст.№");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected virtual void SetEditMode(bool edit)
        {
            bool lEn = (leftIID.Text.Length > 0);
            bool rEn = (lblIID.Text.Length > 0);
            if (!lEn && !rEn && edit)
                return;
            leftRoute1.Enabled = rightRoute2.Enabled = btnCancel.Enabled = btnShowSystem.Enabled = edit;
            leftSum.Enabled = rightSum.Enabled = (edit && seconRoute);
            leftRoute2.Enabled = rightRoute1.Enabled = false;
            allowEvent = !edit;

            leftRoute1.Enabled = leftRoute1.Enabled && lEn;
            leftRoute2.Enabled = leftRoute2.Enabled && lEn;
            leftSum.Enabled = leftSum.Enabled && lEn;

            rightRoute1.Enabled = rightRoute1.Enabled && rEn;
            rightRoute2.Enabled = rightRoute2.Enabled && rEn;
            rightSum.Enabled = rightSum.Enabled && rEn;

            this.btnRefresh.Enabled = !edit;

            if(gbWorkMode.Visible)
                gbWorkMode.Enabled = !edit;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE lists SET nowClimbing = @r1, nowClimbingTmp = @r2 WHERE iid = " + listID.ToString();

            cmd.Parameters.Add("@r1", SqlDbType.Int);
            cmd.Parameters.Add("@r2", SqlDbType.Int);
            if (edit)
            {
                SaveData();
                btnEdit.Text = "Подтвердить";
                btnRestore.Enabled = btnRollBack.Enabled = false;
                int iTmp;
                if (int.TryParse(leftIID.Text, out iTmp))
                    cmd.Parameters[0].Value = iTmp;
                else
                    cmd.Parameters[0].Value = DBNull.Value;
                if (int.TryParse(lblIID.Text, out iTmp))
                    cmd.Parameters[1].Value = iTmp;
                else
                    cmd.Parameters[1].Value = DBNull.Value;
                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch { }
                EnterDataFromSystem();

            }
            else
            {
                btnEdit.Text = "Правка";
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
            
        }

        string[] tmpData = new string[6];
        bool dataSaved = false;
        bool needToExchangeSystem = false; //для Баурока
        private void SaveData()
        {
            tmpData[0] = leftRoute1.Text;
            tmpData[1] = leftRoute2.Text;
            tmpData[2] = leftSum.Text;
            tmpData[3] = rightRoute1.Text;
            tmpData[4] = rightRoute2.Text;
            tmpData[5] = rightSum.Text;
            dataSaved = true;
        }
        private void loadData()
        {
            if (!dataSaved)
                return;
            leftRoute1.Text = tmpData[0];
            leftRoute2.Text = tmpData[1];
            leftSum.Text = tmpData[2];
            rightRoute1.Text = tmpData[3];
            rightRoute2.Text = tmpData[4];
            rightSum.Text = tmpData[5];
        }

        protected virtual DataFromSystem.ResStr[] PutData(DataFromSystem ds, string r1, string r2, out bool cancel)
        {
            bool val = (this.needToExchangeSystem && this.sl is IvanovoListener);
            for (int i = 1; i <= 2; i++)
                ds.SetResMode(i, val, false);
            return ds.ShowForm(out cancel);
        }

        protected void EnterDataFromSystem()
        {
            try
            {
                if (!cbUseSystem.Checked || sl == null)
                    return;
                if (!sl.StartListening())
                    return;
                string r1 = "", r2 = "";
                if (leftStart.Text.Length > 0)
                    r1 = leftIID.Text + " " + leftName.Text + " (Ст.№ " + leftStart.Text + ")";
                else
                    r1 = "";
                if (lblStart.Text.Length > 0)
                    r2 = lblIID.Text + " " + lblName.Text + " (Ст.№ " + lblStart.Text + ")";
                else
                    r2 = "";
                bool cancelData;
                bool needToChangeLocal;
                if (this.sl is IvanovoListener)
                {
                    needToChangeLocal = needToExchangeSystem;
                }
                else
                    needToChangeLocal = false;
                DataFromSystem ds = new DataFromSystem(r1, r2, this.sl, this, cbAutoSystem.Checked, (this.GetType() == typeof(SpeedSystem)), needToChangeLocal);
                if (this is SpeedSystem)
                    ds.SetOneRouteEvent += new EventHandler<SetOneRouteResultEventArgs>(ds_SetOneRouteEvent); //new DataFromSystem.SetOneRouteResultEventHandler(ds_SetOneRouteEvent);
                DataFromSystem.ResStr[] rs = PutData(ds, r1, r2, out cancelData);
                cbAutoSystem.Checked = !ds.StopAuto;

                if (this.sl is IvanovoListener && !cancelData && !ds.ExchangeOccured)
                    this.needToExchangeSystem = !ds.NeedToExchange;

                if (rs.Length > 0 && !cancelData)
                    switch (rs[0].resType)
                    {
                        case DataFromSystem.resTypeE.ROUTE:
                            if (leftRoute1.Enabled)
                                leftRoute1.Text = rs[0].res;
                            else if (leftRoute2.Enabled)
                                leftRoute2.Text = rs[0].res;
                            if (leftSum.Enabled)
                                leftSum.Text = "";
                            break;
                        case DataFromSystem.resTypeE.SUM:
                            if (leftSum.Enabled)
                                leftSum.Text = rs[0].res;
                            if (leftRoute1.Enabled)
                                leftRoute1.Text = "";
                            else if (leftRoute2.Enabled)
                                leftRoute2.Text = "";
                            break;
                    }
                if (rs.Length > 1 && !cancelData)
                    switch (rs[1].resType)
                    {
                        case DataFromSystem.resTypeE.ROUTE:
                            if (rightRoute1.Enabled)
                                rightRoute1.Text = rs[1].res;
                            else if (rightRoute2.Enabled)
                                rightRoute2.Text = rs[1].res;
                            if (rightSum.Enabled)
                                rightSum.Text = "";
                            break;
                        case DataFromSystem.resTypeE.SUM:
                            if (rightSum.Enabled)
                                rightSum.Text = rs[1].res;
                            if (rightRoute2.Enabled)
                                rightRoute2.Text = "";
                            else if (rightRoute1.Enabled)
                                rightRoute1.Text = "";
                            break;
                    }
                if (cbAutoSystem.Checked && !cancelData && btnEdit.Enabled && btnEdit.Text.ToLower() == "подтвердить") //если не нажата кнопка Cancel, то автоматом сохраним результат
                {
                    btnEdit_Click(null, null);
                    SetNextClimber();
                }
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка считывания/сохранения данных:\r\n" + ex.Message); }
        }

        protected virtual void ds_SetOneRouteEvent(object sender, SetOneRouteResultEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetNextClimber()
        {
            if (lblIID.Text.Length + leftIID.Text.Length > 0)
                SetEditMode(true);  
        }
        protected virtual void parseTimes(ref string[] times, ref long[] res)
        {
            if (seconRoute)
            {
                res[0] = ResultListSpeed.ParseString(ref times[0]);
                if (res[0] >= 6000000)
                {
                    times[2] = times[0];
                    times[1] = "";
                    res[2] = 12000000;
                    res[1] = res[2] - res[0];
                    return;
                }
                if (times[2] != "")
                {
                    res[2] = ResultListSpeed.ParseString(ref times[2]);
                    if (res[2] >= 6000000)
                    {
                        res[2] = 12000000;
                        res[1] = res[2] - res[0];
                        times[1] = times[2];
                    }
                    else
                    {
                        res[1] = res[2] - res[0];
                        if (res[1] < 0)
                            throw new ArgumentException();
                        times[1] = ResultListSpeed.CreateString(res[1] / 100);
                    }
                }
                else
                {
                    if (times[1] == "")
                        times[1] = "срыв";
                    res[1] = ResultListSpeed.ParseString(ref times[1]);
                    if (res[1] < 6000000)
                    {
                        res[2] = res[1] + res[0];
                        times[2] = ResultListSpeed.CreateString(res[2] / 100);
                    }
                    else
                    {
                        res[2] = 12000000;
                        res[1] = res[2] - res[0];
                        times[2] = times[1];
                    }
                }
            }
            else
                res[0] = ResultListSpeed.ParseString(ref times[0]);
        }

        private class TableIdRow
        {
            public int ClimberId { get; set; }
            public long Result { get; set; }
            public object[] RowData { get; set; }
        }
        protected virtual int RefreshTable(SqlDataAdapter da, int prevCount, DataTable dt = null)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                bool recount = (dt != null);
                if (dt == null)
                {
                    dt = new DataTable();
                    da.Fill(dt);
                }

                if (prevCount > 0)
                    foreach (DataRow dr in dt.Rows)
                        if (dr["Кв."].ToString() == " q")
                            dr["Кв."] = "";

                if (dt.Rows.Count < 1)
                    return 0;
                if (!dt.Columns.Contains("pos"))
                    dt.Columns.Add("pos", typeof(string));
                dt.Rows[0]["pos"] = prevCount + 1;
                dt.Rows[0]["Место"] = (prevCount + 1).ToString();
                int prevPos = prevCount + 1;

                long resPrev;
                try { resPrev = Convert.ToInt64(dt.Rows[0]["res"]); }
                catch { RefreshData(); return 0; }
                long resCur = resPrev;
                int quote = 0;
                if(!rbInternational.Checked)
                    if (prevCount > 0)
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM speedResults WHERE (resText IS NOT NULL AND resText <> '') AND (list_id=" +
                            listID.ToString() + ")";
                        quote = Convert.ToInt32(cmd.ExecuteScalar());
                        if ((quote % 2) == 1)
                            quote = (quote + 1) / 2;
                        else
                            quote /= 2;
                        cmd.CommandText = "SELECT COUNT(*) FROM speedResults WHERE (resText IS NOT NULL AND resText <>'') AND (list_id=" +
                            listID.ToString() + ") AND (qf='Q')";
                        quote -= Convert.ToInt32(cmd.ExecuteScalar());
                    }
                double curPts = (double)prevPos;
                List<ResultCompare> sameRes = new List<ResultCompare>();
                DataColumn idColumn;
                bool hasRecount = false;
                if (!recount && dt.Columns.Contains("№"))
                {
                    idColumn = dt.Columns["№"];
                    sameRes.Add(new ResultCompare { ClimberId = Convert.ToInt32(dt.Rows[0][idColumn]), Result = resCur });
                }
                else
                    idColumn = null;
                for (int rowIndexx = 1; rowIndexx < dt.Rows.Count; rowIndexx++)
                //foreach (DataRow rd in dt.Rows)
                {
                    DataRow rd = dt.Rows[rowIndexx];

                    resCur = Convert.ToInt64(rd["res"]);

                    if (resCur == resPrev)
                    {
                        rd["pos"] = prevPos;
                        rd["Место"] = prevPos.ToString();
                        curPts += 0.5;
                        if (idColumn != null)
                        {
                            sameRes.Add(new ResultCompare { ClimberId = Convert.ToInt32(rd[idColumn]), Result = resCur });
                            if (!hasRecount)
                                hasRecount = true;
                        }
                        continue;
                    }
                    else
                    {
                        double dd = curPts - (double)prevCount - 0.1;
                        if ((curPts - (double)prevCount - 0.1 <= (double)quote) && resPrev < 12000000)
                            for (int i = prevPos - prevCount - 1; i < rowIndexx; i++)
                                dt.Rows[i]["Кв."] = " q";
                        prevPos = prevCount + 1 + rowIndexx;
                        rd["pos"] = prevPos;
                        rd["Место"] = prevPos.ToString();
                        resPrev = resCur;
                        curPts = (double)prevPos;
                        if (idColumn != null)
                        {
                            if (sameRes.Count > 1)
                            {
                                CompareAndUpdateRes(this.listID, sameRes, da.SelectCommand);
                                foreach (DataRow dataRow in dt.Rows)
                                {
                                    foreach(var rc in sameRes)
                                        if (Convert.ToInt32(dataRow[idColumn]) == rc.ClimberId)
                                        {
                                            dataRow["res"] = rc.Result;
                                            break;
                                        }
                                }
                            }
                            sameRes.Clear();
                            sameRes.Add(new ResultCompare { ClimberId = Convert.ToInt32(rd[idColumn]), Result = resCur });
                        }
                    }
                }

                if (idColumn != null && sameRes.Count > 1)
                {
                    CompareAndUpdateRes(this.listID, sameRes, da.SelectCommand);
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        foreach (var rc in sameRes)
                            if (Convert.ToInt32(dataRow[idColumn]) == rc.ClimberId)
                            {
                                dataRow["res"] = rc.Result;
                                break;
                            }
                    }
                }

                if (hasRecount)
                {
                    List<TableIdRow> lstRow = new List<TableIdRow>();
                    foreach (DataRow dRow in dt.Rows)
                        lstRow.Add(new TableIdRow
                        {
                            ClimberId = Convert.ToInt32(dRow[idColumn]),
                            Result = Convert.ToInt64(dRow["res"]),
                            RowData = dRow.ItemArray
                        });
                    lstRow.Sort((a, b) => a.Result.CompareTo(b.Result));
                    dt.Rows.Clear();
                    foreach (var r in lstRow)
                        dt.Rows.Add(r.RowData);
                    return RefreshTable(da, prevCount, dt);
                }

                double d = curPts - (double)prevCount - 0.1;
                if ((curPts - (double)prevCount - 0.1 <= (double)quote) && resCur < 12000000 && !cbPrevRound.Enabled)
                    for (int i = prevPos - prevCount - 1; i < dt.Rows.Count; i++)
                        dt.Rows[i]["Кв."] = " q";

                try
                {
                    if (daResF.InsertCommand == null)
                        daResF.InsertCommand = daResF.UpdateCommand;
                    daResF.Update(dt);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try
                {
                    (new SqlCommand("UPDATE speedResults SET changed=1 WHERE list_id=" +
                       listID.ToString(), cn)).ExecuteNonQuery();
                }
                catch { }
                return dt.Rows.Count;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return 0; }

        }

        protected bool IFSC_WR { get { return (Rules & SpeedRules.IFSC_WR) == SpeedRules.IFSC_WR; } }

        private CheckAndDeleteRes ClearResult(int climberID, int runNo, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            CheckAndDeleteRes retVal = CheckAndDeleteRes.Error;
            cmd.Transaction = (tran == null) ? cn.BeginTransaction() : tran;
            try
            {
                cmd.CommandText = "SELECT COUNT(*)" +
                                  "  FROM speedResults_Runs SR(nolock)" +
                                  "  JOIN speedResults S(nolock) on S.iid = SR.iid_parent" +
                                  " WHERE S.list_id = " + listID.ToString() +
                                  "   AND S.climber_id = " + climberID.ToString() +
                                  "   AND SR.run_no > " + runNo.ToString() +
                                  "   AND SR.result IS NOT NULL";
                object oRes = cmd.ExecuteScalar();
                if (oRes != null && oRes != DBNull.Value && (Convert.ToInt32(oRes) > 0))
                {
                    cmd.CommandText = "SELECT P.surname + ' ' + P.name FROM Participants P(nolock) where P.iid = " + climberID.ToString();

                    MessageBox.Show("Для участника " + cmd.ExecuteScalar().ToString().ToUpper() + " есть результаты в следующих забегах. Очистка результата невозможна");
                    return CheckAndDeleteRes.Error;
                }
                cmd.CommandText = "UPDATE speedResults_Runs SET result = NULL, res_text = NULL" +
                                  "  FROM speedResults_Runs SR(nolock)" +
                                  "  JOIN speedResults S(nolock) on S.iid = SR.iid_parent" +
                                  " WHERE S.climber_id = " + climberID.ToString() +
                                  "   AND S.list_id = " + listID.ToString() +
                                  "   AND SR.run_no = " + runNo.ToString();
                cmd.ExecuteNonQuery();

                retVal = CheckAndDeleteRes.CommitAndExit;

                return retVal;
            }
            finally
            {
                if (tran == null)
                {
                    if (retVal == CheckAndDeleteRes.CommitAndExit)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                }
            }
        }

        private enum CheckAndDeleteRes
        {
            Error, Continue, CommitAndExit
        }

        //true если надо закончить обновление результата
        private CheckAndDeleteRes CheckAndDelete()
        {
            if (!speedAdvancedSystem)
                return CheckAndDeleteRes.Continue;
            if (runNo == null || !runNo.HasValue || (String.IsNullOrEmpty(lblIID.Text) && String.IsNullOrEmpty(leftIID.Text)))
                return CheckAndDeleteRes.Continue;
            if (String.IsNullOrEmpty(leftIID.Text) && !String.IsNullOrEmpty(lblIID.Text))
            {
                if (string.IsNullOrEmpty(rightRoute2.Text.Trim()))
                {
                    if (MessageBox.Show("Удалить результат участника " + lblName.Text.ToUpper() + "?", String.Empty, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        return ClearResult(int.Parse(lblIID.Text), RunNo.Value);
                    return CheckAndDeleteRes.CommitAndExit;
                }
                else
                    return CheckAndDeleteRes.Continue;
            }
            if (String.IsNullOrEmpty(lblIID.Text) && !String.IsNullOrEmpty(leftIID.Text))
            {
                if (string.IsNullOrEmpty(leftRoute1.Text.Trim()))
                {
                    if (MessageBox.Show("Удалить результат участника " + leftName.Text.ToUpper() + "?", String.Empty, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        return ClearResult(int.Parse(leftIID.Text), runNo.Value);
                    return CheckAndDeleteRes.CommitAndExit;
                }
                else
                    return CheckAndDeleteRes.Continue;
            }

            if (String.IsNullOrEmpty(leftRoute1.Text.Trim()) && String.IsNullOrEmpty(rightRoute2.Text.Trim()))
            {
                if (MessageBox.Show("Удалить результат пары " + leftName.Text.ToUpper() + " - " + lblName.Text.ToUpper() + "?", String.Empty, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    SqlTransaction tran = cn.BeginTransaction();
                    bool scs = false;
                    try
                    {
                        var res = ClearResult(int.Parse(leftIID.Text), runNo.Value, tran);
                        if (res != CheckAndDeleteRes.CommitAndExit)
                            return res;
                        res = ClearResult(int.Parse(lblIID.Text), runNo.Value, tran);
                        if (res != CheckAndDeleteRes.CommitAndExit)
                            return res;
                        scs = true;
                    }
                    finally
                    {
                        if (scs)
                            tran.Commit();
                        else
                            tran.Rollback();
                    }
                }
                return CheckAndDeleteRes.CommitAndExit;
            }
            if (String.IsNullOrEmpty(leftRoute1.Text.Trim()))
            {
                MessageBox.Show("Не введён результат участника " + leftName.Text.ToUpper() + " (СЛЕВА)");
                return CheckAndDeleteRes.Error;
            }
            if (String.IsNullOrEmpty(rightRoute2.Text.Trim()))
            {
                MessageBox.Show("Не введён результат учасника " + lblName.Text.ToUpper() + " (СПРАВА)");
                return CheckAndDeleteRes.Error;
            }
            return CheckAndDeleteRes.Continue;
        }

        private class ResultCompare
        {
            public int ClimberId { get; set; }
            public long Result { get; set; }
            private int posMod = 0;
            public int PosMod { get { return posMod; } set { posMod = value; } }
        }

        private void CompareAndUpdateRes(int initialList, List<ResultCompare> sourceResults, SqlCommand cmd)
        {
            var cmdSelect = new SqlCommand { Connection = cmd.Connection, Transaction = cmd.Transaction };
            cmdSelect.CommandText = String.Format("SELECT LPrev.iid" +
                                                  "  FROM Lists L(nolock)" +
                                                  "  JOIN Lists LPrev(nolock) on LPrev.iid = L.prev_round" +
                                                  " WHERE L.iid={0}", initialList);
            int currentList;
            using (var rdr = cmdSelect.ExecuteReader())
            {
                if (rdr.Read())
                {
                    if (rdr[0] != DBNull.Value)
                        currentList = Convert.ToInt32(rdr[0]);
                    else
                        return;
                }
                else
                    return;
            }
            cmdSelect.CommandText = "SELECT R.res" +
                                  "  FROM speedResults R(nolock)" +
                                  " WHERE R.list_id = @listId" +
                                  "   AND R.climber_id = @climberId";
            cmdSelect.Parameters.Add("@listId", SqlDbType.Int).Value = currentList;
            cmdSelect.Parameters.Add("@climberId", SqlDbType.Int);


            List<ResultCompare> roundResults = new List<ResultCompare>();
            bool completed = true;
            foreach (var rc in sourceResults)
            {
                cmdSelect.Parameters[1].Value = rc.ClimberId;
                using (var rdr = cmdSelect.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        if (rdr[0] != DBNull.Value)
                            roundResults.Add(new ResultCompare { ClimberId = rc.ClimberId, Result = Convert.ToInt64(rdr[0]) });
                        else
                        {
                            completed = false;
                            break;
                        }
                    }
                    else
                    {
                        completed = false;
                        break;
                    }
                }
            }
            if (!completed)
            {
                CompareAndUpdateRes(currentList, sourceResults, cmd);
                return;
            }
            roundResults.Sort((a, b) => a.Result.CompareTo(b.Result));
            int cnt = roundResults.Count;
            long? lastRes = null;
            List<ResultCompare> sameResults = new List<ResultCompare>();
            int pos = 0;
            for (int i = 0; i < cnt; i++)
            {
                if (lastRes == null || lastRes.Value != roundResults[i].Result)
                {
                    if (sameResults.Count > 1)
                        CompareAndUpdateRes(currentList, sameResults, cmd);
                    sameResults.Clear();
                    pos = i;
                    lastRes = roundResults[i].Result;
                }
                ResultCompare rcSrc = null;
                foreach (var rs in sourceResults)
                    if (rs.ClimberId == roundResults[i].ClimberId)
                    {
                        rcSrc = rs;
                        break;
                    }
                rcSrc.PosMod += pos;
                rcSrc.Result += pos;
                sameResults.Add(rcSrc);
            }
            if (sameResults.Count > 1)
                CompareAndUpdateRes(currentList, sameResults, cmd);


        }
        
        private void CompareAndUpdateRes(ref long rLeft, int iidLeft, ref long rRight, int iidRight, SqlCommand cmd)
        {
            ResultCompare resLeft = new ResultCompare { ClimberId = iidLeft, Result = rLeft };
            ResultCompare resRight = new ResultCompare { ClimberId = iidRight, Result = rRight };
            List<ResultCompare> tempList = new List<ResultCompare>();
            tempList.Add(resLeft);
            tempList.Add(resRight);
            CompareAndUpdateRes(this.listID, tempList, cmd);
            var cmdU = new SqlCommand { Connection = cmd.Connection, Transaction = cmd.Transaction };
            cmdU.CommandText = "UPDATE speedResults SET res=@res WHERE list_id=@listId AND climber_id=@climberId";
            cmdU.Parameters.Add("@listId", SqlDbType.Int).Value = this.listID;
            cmdU.Parameters.Add("@climberId", SqlDbType.Int);
            cmdU.Parameters.Add("@res", SqlDbType.BigInt);
            if (resLeft.Result != rLeft)
            {
                cmdU.Parameters[1].Value = iidLeft;
                cmdU.Parameters[2].Value = resLeft.Result;
                cmdU.ExecuteNonQuery();

                rLeft = resLeft.Result;
            }
            if (resRight.Result != rRight)
            {
                cmdU.Parameters[1].Value = iidRight;
                cmdU.Parameters[2].Value = resRight.Result;
                cmdU.ExecuteNonQuery();

                rRight = resRight.Result;
            }
            
        }

        protected virtual void btnEdit_Click(object sender, EventArgs e)
        {
            if (speedAdvancedSystem && runNo == null)
            {
                MessageBox.Show(this, "Выберите забег для редактирования");
                return;
            }
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (btnEdit.Text == "Правка")
            {
                SetEditMode(true);
                return;
            }
            switch (CheckAndDelete())
            {
                case CheckAndDeleteRes.CommitAndExit:
                    SetEditMode(false);
                    return;
                case CheckAndDeleteRes.Error:
                    return;
            }
            SetEditMode(false);

            Dictionary<string, string> oldVals, oldValsOther;
            long mainId, secondId;
            int mainStart;
            SqlCommand cmdL2 = new SqlCommand();
            cmdL2.Connection = cn;
            string mainIdToCheckStr = (seconRoute || speedAdvancedSystem && runNo != null && runNo.Value > 0 && (runNo.Value % 2 == 0)) ? lblIID.Text : leftIID.Text;
            if (!long.TryParse(mainIdToCheckStr, out mainId))
                return;
            cmdL2.CommandText = "SELECT iid, start FROM speedResults(NOLOCK) WHERE list_id=" + listID.ToString() +
                " AND climber_id=" + mainId.ToString();
            var dr1 = cmdL2.ExecuteReader();
            try
            {
                if (dr1.Read())
                {
                    mainId = Convert.ToInt64(dr1["iid"]);
                    mainStart = Convert.ToInt32(dr1["start"]);
                }
                else
                    return;
            }
            finally { dr1.Close(); }
            int startToCheck = (mainStart % 2 == 1) ? mainStart + 1 : mainStart - 1;
            cmdL2.CommandText = "SELECT iid FROM speedResults(NOLOCK) WHERE list_id=" + listID.ToString() +
                " AND start=" + startToCheck.ToString();
            dr1 = cmdL2.ExecuteReader();
            try
            {
                if (dr1.Read())
                    secondId = Convert.ToInt64(dr1["iid"]);
                else
                    secondId = -1;
            }
            finally { dr1.Close(); }

            ResultListSpeed.GetClimbersResSpeed(mainId, cn, out oldVals);
            if (secondId > 0)
                ResultListSpeed.GetClimbersResSpeed(secondId, cn, out oldValsOther);
            else
                oldValsOther = new Dictionary<string, string>();

            try
            {
                long[] rightRouteNum = new long[3];
                string[] rightRouteStr = new string[3];
                for (int i = 0; i < 2; i++)
                {
                    rightRouteNum[i] = 0;
                    rightRouteStr[i] = "";
                }

                if (speedAdvancedSystem && !seconRoute)
                    seconRoute = true;

                if (seconRoute)
                {
                    #region 2 трасса
                    int posToEnter = 0;
                    long[] otherLong;
                    string[] otherString;
                    if (rbInternational.Checked)
                    {
                        posToEnter = GetIntlPosToEnter();
                    }
                    bool oneRoute = ((Rules & SpeedRules.OneRouteToRunInFinal) == SpeedRules.OneRouteToRunInFinal) || speedAdvancedSystem;


                    if (speedAdvancedSystem)
                    {
                        if (String.IsNullOrEmpty(leftIID.Text))
                        {
                            rightRouteStr = null;
                            rightRouteNum = null;
                        }
                        else
                        {
                            rightRouteStr[0] = rightRouteStr[2] = leftRoute1.Text;
                            rightRouteStr[1] = "0";
                        }
                        if (String.IsNullOrEmpty(lblIID.Text))
                        {
                            otherLong = null;
                            otherString = null;
                        }
                        else
                        {
                            otherString = new string[] { "0", rightRoute2.Text, rightRoute2.Text };
                            otherLong = new long[3];
                            try { parseTimes(ref otherString, ref otherLong); }
                            catch
                            {
                                MessageBox.Show("Неверный ввод результатов на правой трассе");
                                return;
                            }
                        }
                    }
                    else
                    {
                        rightRouteStr[0] = rightRoute1.Text;
                        rightRouteStr[1] = oneRoute ? "0" : rightRoute2.Text;
                        rightRouteStr[2] = oneRoute ? rightRoute1.Text : rightSum.Text;

                        otherLong = null;
                        otherString = null;
                    }
                    if(rightRouteStr != null)
                        try { parseTimes(ref rightRouteStr, ref rightRouteNum); }
                        catch
                        {
                            MessageBox.Show("Неверный ввод рез-тов на " + (speedAdvancedSystem ? "левой" : "правой") + " трассе");
                            return;
                        }
                    if (speedAdvancedSystem)
                    {
                        leftRoute1.Text = leftSum.Text = (rightRouteStr == null ? String.Empty : rightRouteStr[0]);
                        leftRoute2.Text = rightRoute1.Text = String.Empty;
                        rightRoute2.Text = rightSum.Text = (otherString == null ? String.Empty : otherString[1]);
                    }
                    else
                    {
                        rightRoute2.Text = rightRouteStr[1];
                        rightSum.Text = rightRouteStr[2];
                    }
                    if (oneRoute && rightRouteStr != null)
                        rightRouteStr[1] = "-";

                    long rLeft = long.MaxValue, rRight;
                    if (speedAdvancedSystem)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "EXEC dbo.update_SpeedRun @P_nClimberLeft = @nClimberLeft," +
                                                                 "@P_nClimberRight = @nClimberRight," +
                                                                 "@P_nResLeft = @nResLeft," +
                                                                 "@P_sResLeft = @sResLeft," +
                                                                 "@P_nResRight = @nResRight," +
                                                                 "@P_sResRight = @sResRight," +
                                                                 "@P_nListID = " + listID.ToString() + "," +
                                                                 "@P_nRunNo = " + runNo.Value.ToString();
                        /*
                        object clmLeft = String.IsNullOrEmpty(leftIID.Text)?DBNull.Value: (object)int.Parse(leftIID.Text);
                        object clmRight = String.IsNullOrEmpty(lblIID.Text) ? DBNull.Value : (object)int.Parse(lblIID.Text);
                        object nResLeft = resLong == null ? DBNull.Value : (object)resLong[0];
                        object nResRight = otherLong == null ? DBNull.Value : (object)otherLong[1];
                        */


                        cmd.Parameters.Add("@nClimberLeft", SqlDbType.Int).Value = String.IsNullOrEmpty(leftIID.Text) ? DBNull.Value : (object)int.Parse(leftIID.Text);
                        cmd.Parameters.Add("@nClimberRight", SqlDbType.Int).Value = (String.IsNullOrEmpty(lblIID.Text) ? DBNull.Value : (object)int.Parse(lblIID.Text));
                        cmd.Parameters.Add("@nResLeft", SqlDbType.BigInt).Value = (rightRouteNum==null) ? DBNull.Value : (object)rightRouteNum[0];
                        cmd.Parameters.Add("@sResLeft", SqlDbType.VarChar, 50).Value = rightRouteStr == null ? DBNull.Value :  (object)rightRouteStr[0];
                        cmd.Parameters.Add("@nResRight", SqlDbType.BigInt).Value = (otherLong == null ? DBNull.Value : (object)otherLong[1]);
                        cmd.Parameters.Add("@sResRight", SqlDbType.VarChar, 50).Value = (otherString == null ? DBNull.Value : (object)otherString[1]);

                        bool scs = false;
                        cmd.Transaction = cmd.Connection.BeginTransaction();
                        try
                        {
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "SELECT res FROM speedResults(nolock) WHERE climber_id = @clm AND list_id = " + listID.ToString();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@clm", SqlDbType.Int);
                            if (!String.IsNullOrEmpty(leftIID.Text))
                            {
                                cmd.Parameters[0].Value = int.Parse(leftIID.Text);
                                rLeft = Convert.ToInt64(cmd.ExecuteScalar());
                            }
                            else
                                rLeft = -1;
                            if (!String.IsNullOrEmpty(lblIID.Text))
                            {
                                cmd.Parameters[0].Value = int.Parse(lblIID.Text);
                                rRight = Convert.ToInt64(cmd.ExecuteScalar());
                            }
                            else
                                rRight = -1;
                            scs = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка записи данных:\r\n" + ex.Message);
                            return;
                        }
                        finally
                        {
                            if (scs)
                                cmd.Transaction.Commit();
                            else
                                cmd.Transaction.Rollback();
                            cmd.Transaction = null;
                            cmd.Parameters.Clear();
                        }

                    }
                    else
                    {
                        //cmd.CommandText = "SELECT sr.pos FROM speedResults sr(nolock) INNER JOIN lists l(nolock) ON sr.list_id = " +
                        //    "l.iid WHERE (l.next_round = " + listID.ToString() + ") AND (sr.climber_id = " +
                        //    lblIID.Text + ")";
                        //try { rightRouteNum[2] += Convert.ToInt32(cmd.ExecuteScalar()); }
                        //catch { rightRouteNum[2] += 99; }
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@r", SqlDbType.BigInt);
                        cmd.Parameters.Add("@rt", SqlDbType.VarChar);
                        cmd.Parameters.Add("@r1", SqlDbType.BigInt);
                        cmd.Parameters.Add("@r1t", SqlDbType.VarChar);
                        cmd.Parameters[0].Value = rightRouteNum[2];
                        cmd.Parameters[1].Value = rightRouteStr[2];
                        cmd.Parameters[2].Value = rightRouteNum[1];
                        cmd.Parameters[3].Value = rightRouteStr[1];
                        rRight = rightRouteNum[2];
                        cmd.CommandText = "UPDATE speedResults SET res=@r,resText=@rt,route2=@r1,route2_text=@r1t,qf='' " +
                            "WHERE (climber_id=" + lblIID.Text + ") AND (list_id=" + listID.ToString() + ")";
                        try { cmd.ExecuteNonQuery(); }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    } // not speedAdvancedSystem - ввод правой трассы

                    string sToCheck;
                    if (speedAdvancedSystem)
                    {
                        if (runNo != null && runNo.Value > 0 && (runNo.Value % 2 == 0))
                            sToCheck = leftIID.Text;
                        else
                            sToCheck = lblIID.Text;
                    }
                    else
                        sToCheck = leftIID.Text;

                    if (!String.IsNullOrEmpty(sToCheck /*speedAdvancedSystem ? lblIID.Text : leftIID.Text*/))
                    {
                        #region Оба участника есть
                        if (!speedAdvancedSystem)
                        {
                            rightRouteStr[0] = leftRoute2.Text;
                            rightRouteStr[1] = oneRoute ? "0" : leftRoute1.Text;
                            rightRouteStr[2] = oneRoute ? leftRoute2.Text : leftSum.Text;
                            try { parseTimes(ref rightRouteStr, ref rightRouteNum); }
                            catch
                            {
                                MessageBox.Show("Неверный ввод рез-тов на левой трассе");
                                return;
                            }

                            rLeft = rightRouteNum[2];

                            if (oneRoute)
                                rightRouteStr[1] = "-";
                            leftSum.Text = rightRouteStr[2];
                            leftRoute1.Text = rightRouteStr[1];
                            //cmd.CommandText = "SELECT sr.pos FROM speedResults sr INNER JOIN lists l ON sr.list_id = " +
                            //    "l.iid WHERE (l.next_round = " + listID.ToString() + ") AND (sr.climber_id = " +
                            //    leftIID.Text + ")";
                            //try { rightRouteNum[2] += Convert.ToInt32(cmd.ExecuteScalar()); }
                            //catch { rightRouteNum[2] += 99; }

                            cmd.Parameters[0].Value = rightRouteNum[2];
                            cmd.Parameters[1].Value = rightRouteStr[2];
                            cmd.Parameters[2].Value = rightRouteNum[1];
                            cmd.Parameters[3].Value = rightRouteStr[1];
                            cmd.CommandText = "UPDATE speedResults SET res=@r,resText=@rt,route1=@r1,route1_text=@r1t,qf='' " +
                                "WHERE (climber_id=" + leftIID.Text + ") AND (list_id=" + listID.ToString() + ")";
                            try { cmd.ExecuteNonQuery(); }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return;
                            }


                        } // not speedAdvancedSystem - ввод 2ой трассы

                        cmd.CommandText = String.Empty;

                        if (speedAdvancedSystem)
                        {
                            cmd.CommandText = "UPDATE speedResults SET qf='', pos = NULL, posText = NULL WHERE climber_id=@clm AND list_id=" + listID.ToString();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@clm", SqlDbType.Int);
                            if (!String.IsNullOrEmpty(lblIID.Text))
                            {
                                cmd.Parameters[0].Value = int.Parse(lblIID.Text);
                                cmd.ExecuteNonQuery();
                            }
                            if (!String.IsNullOrEmpty(leftIID.Text))
                            {
                                cmd.Parameters[0].Value = int.Parse(leftIID.Text);
                                cmd.ExecuteNonQuery();
                            }
                            cmd.Parameters.Clear();
                            cmd.CommandText = String.Empty;
                        }

                        if (finals || (semi && rbInternational.Checked))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ci", SqlDbType.Int);
                            cmd.Parameters.Add("@ps", SqlDbType.Int);
                            cmd.Parameters.Add("@pt", SqlDbType.VarChar);
                            cmd.Parameters.Add("@q", SqlDbType.VarChar, 50);

                            cmd.CommandText = "UPDATE speedResults SET pos=@ps,posText=@pt,qf=@q WHERE " +
                                    "(climber_id=@ci) AND (list_id=" + listID.ToString() + ")";
                            bool exch = speedAdvancedSystem && runNo != null && (runNo.Value % 2 == 1);
                            string sCheck = exch ? leftStart.Text : lblStart.Text;
                            if ((sCheck/*lblStart.Text*/ == "3" && finals) || (sCheck/*lblStart.Text*/ == "1" && semi) || IsLimitedFinal)
                            {
                                cmd.Parameters[1].Value = 1;
                                cmd.Parameters[2].Value = "1";
                            }
                            else
                            {
                                if (finals)
                                {
                                    cmd.Parameters[1].Value = 3;
                                    cmd.Parameters[2].Value = "3";
                                }
                                else if (semi)
                                {
                                    cmd.Parameters[1].Value = 2;
                                    cmd.Parameters[2].Value = "2";
                                }
                            }
                            if (semi)
                                cmd.Parameters[3].Value = "Q";
                            else
                                cmd.Parameters[3].Value = "";
                            if (rLeft < rRight)
                                cmd.Parameters[0].Value = int.Parse(leftIID.Text);
                            else
                                cmd.Parameters[0].Value = int.Parse(lblIID.Text);
                            cmd.ExecuteNonQuery();

                            if ((sCheck/*lblStart.Text*/ == "3" && finals) || (sCheck /*lblStart.Text*/ == "1" && semi) || IsLimitedFinal)
                            {
                                if (finals)
                                {
                                    cmd.Parameters[1].Value = 2;
                                    cmd.Parameters[2].Value = "2";
                                }
                                else if (semi)
                                {
                                    cmd.Parameters[1].Value = 3;
                                    cmd.Parameters[2].Value = "3";
                                }
                            }
                            else
                            {
                                cmd.Parameters[1].Value = 4;
                                cmd.Parameters[2].Value = "4";
                            }
                            cmd.Parameters[3].Value = "";
                            if (rLeft < rRight)
                                cmd.Parameters[0].Value = int.Parse(lblIID.Text);
                            else
                                cmd.Parameters[0].Value = int.Parse(leftIID.Text);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand cmdL = new SqlCommand();
                            cmdL.Connection = cmd.Connection;
                            cmdL.CommandText = "UPDATE speedResults SET pos=" + posToEnter.ToString() + ",posText=@pt " +
                                "WHERE climber_id = @cid AND list_id=" + listID.ToString();
                            cmdL.Parameters.Add("@pt", SqlDbType.VarChar, 50);
                            cmdL.Parameters[0].Value = posToEnter.ToString();
                            cmdL.Parameters.Add("@cid", SqlDbType.Int);

                            if (rLeft == rRight && rLeft < 12000000)
                                CompareAndUpdateRes(ref rLeft, int.Parse(leftIID.Text), ref rRight, int.Parse(lblIID.Text), cmd);

                            if ((rLeft < rRight) && (rLeft < 12000000))
                            {
                                cmd.CommandText = "UPDATE speedResults SET qf='Q' WHERE (climber_id=" + leftIID.Text +
                                    ") AND (list_id=" + listID.ToString() + ")";
                                cmdL.Parameters[1].Value = int.Parse(leftIID.Text);
                            }
                            else if (rRight < 12000000 || cbPrevRound.Checked)
                            {
                                cmd.CommandText = "UPDATE speedResults SET qf='Q' WHERE (climber_id=" + lblIID.Text +
                                    ") AND (list_id=" + listID.ToString() + ")";
                                cmdL.Parameters[1].Value = int.Parse(lblIID.Text);
                            }
                            if (!String.IsNullOrEmpty(cmd.CommandText))
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    if (rbInternational.Checked)
                                        cmdL.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Ошибка обновления БД:\r\n" + ex.Message);
                                    return;
                                }
                        }
                        cmd.CommandText = "UPDATE lists SET nowClimbing = NULL, nowClimbingTmp = NULL WHERE iid = " + listID.ToString();
                        try { cmd.ExecuteNonQuery(); }
                        catch { }
                        #endregion
                    }
                    else
                    {
                        bool exchange = (speedAdvancedSystem && runNo != null && (runNo.Value % 2 == 1));
                        long resCur = exchange ? rLeft : rRight;
                        string iidCur = exchange ? leftIID.Text : lblIID.Text;
                        string startCur = exchange ? leftStart.Text : lblStart.Text;
                        if (finals || (semi && rbInternational.Checked))
                        {
                            cmd.CommandText = "UPDATE speedResults SET pos=";
                            if ((/*lblStart.Text*/startCur == "3" && finals) || (/*lblStart.Text*/startCur == "1" && semi))
                                cmd.CommandText += "1,posText='1'";
                            else
                            {
                                if (finals)
                                    cmd.CommandText += "3,posText='3'";
                                else if (semi)
                                    cmd.CommandText += "2,posText='2'";
                            }
                            if (semi)
                                cmd.CommandText += ", qf='Q' ";
                            cmd.CommandText += " WHERE " +
                                "(climber_id=" + /*lblIID.Text*/iidCur + ") AND (list_id=" + listID.ToString() + ")";
                            cmd.ExecuteNonQuery();
                        }
                        else
                            if (/*rRight*/resCur < 12000000)
                            {
                                cmd.CommandText = "UPDATE speedResults SET qf='Q' WHERE (climber_id=" + /*lblIID.Text*/iidCur +
                                    ") AND (list_id=" + listID.ToString() + ")";
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    if (rbInternational.Checked)
                                    {
                                        cmd.CommandText = "UPDATE speedResults SET pos=" + posToEnter.ToString() +
                                            ", posText='" + posToEnter.ToString() + "' WHERE (climber_id=" + /*lblIID.Text*/iidCur +
                                            ") AND (list_id=" + listID.ToString() + ")";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Ошибка обновления БД:\r\n" + ex.Message);
                                    return;
                                }
                            }
                    }
                    RefreshShownTables();
                    #endregion
                }
                else
                {
                    #region 1 трасса
                    if (leftRoute1.Text == "")
                    {
                        MessageBox.Show("Результат 1ой трассы не введён");
                        return;
                    }
                    if ((lblIID.Text != "") && (rightRoute2.Text == ""))
                    {
                        MessageBox.Show("Результат 2ой трассы не введён");
                        return;
                    }
                    string strTmp = leftRoute1.Text;
                    leftRoute = ResultListSpeed.ParseString(ref strTmp);
                    leftRoute1.Text = strTmp;
                    cmd.CommandText = "UPDATE speedResults SET route1=@r,route1_text=@rt WHERE " +
                        "(climber_id=" + leftIID.Text + ") AND (list_id=" + listID.ToString() + ")";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@r", SqlDbType.BigInt);
                    cmd.Parameters[0].Value = leftRoute;
                    cmd.Parameters.Add("@rt", SqlDbType.VarChar);
                    cmd.Parameters[1].Value = strTmp;
                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    if (lblIID.Text != "")
                    {
                        strTmp = rightRoute2.Text;
                        leftRoute = ResultListSpeed.ParseString(ref strTmp);
                        rightRoute2.Text = strTmp;
                        cmd.CommandText = "UPDATE speedResults SET route2=@r,route2_text=@rt WHERE " +
                            "(climber_id=" + lblIID.Text + ") AND (list_id=" + listID.ToString() + ")";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@r", SqlDbType.BigInt);
                        cmd.Parameters[0].Value = leftRoute;
                        cmd.Parameters.Add("@rt", SqlDbType.VarChar);
                        cmd.Parameters[1].Value = strTmp;
                        try { cmd.ExecuteNonQuery(); }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                    #endregion
                }
            }
            finally
            {
                Dictionary<string, string> newVals = new Dictionary<string, string>();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    ResultListSpeed.GetClimbersResSpeed(mainId, cn, out newVals, tran);
                    long nextTranId = ResultListSpeed.AddTransaction(mainId, oldVals, newVals, cn, -1, tran, (secondId < 1));
                    if (secondId > 0)
                    {
                        ResultListSpeed.GetClimbersResSpeed(secondId, cn, out newVals, tran);
                        ResultListSpeed.AddTransaction(secondId, oldValsOther, newVals, cn, nextTranId, tran, true);
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); }
                    catch { }
                    MessageBox.Show(this, "Ошибка сохранения журнала транзакций:\r\n" + ex.Message);
                }

            }

            seconRoute = !seconRoute;

            RefreshData();
            SetCurrentPair();

            if (seconRoute && (Rules & SpeedRules.OneRouteToRunInFinal) == SpeedRules.OneRouteToRunInFinal)
            {
                btnEdit.Text = "Подтвердить";
                btnEdit_Click(sender, e);
                return;
            }
            
        }

        private void RefreshShownTables()
        {
            if (speedAdvancedSystem && this.runNo != null && this.runNo.Value > 0)
            {
                var backupRunNo = this.runNo.Value;
                try
                {
                    this.runNo = null;
                    CreateDataAdapters();
                    RefreshShownTables();
                }
                finally
                {
                    this.runNo = new int?(backupRunNo);
                    CreateDataAdapters();
                }
            }
            else
            {
                if (!(finals || (rbInternational.Checked && semi)))
                {
                    if (rbInternational.Checked)
                    {
                        cmd.Parameters.Clear();
                        //cmd.CommandText = "SELECT MAX(pos) FROM speedResults(NOLOCK) WHERE list_id=" + listID.ToString() +
                        //    " AND qf = 'Q'";
                        cmd.CommandText = "SELECT COUNT(*) FROM speedResults(NOLOCK) WHERE list_id=" + listID.ToString() +
                            " AND qf='Q'";
                        RefreshTable(daResL, Convert.ToInt32(cmd.ExecuteScalar()));
                    }
                    else
                    {
                        RefreshTable(daResL, RefreshTable(daResW, 0));
                        RefreshTable(daResQ, 0);
                    }
                }
            }
        }

        private int GetIntlPosToEnter()
        {
            int posToEnter;
            SqlCommand cmdl = new SqlCommand();
            cmdl.Connection = cn;
            cmdl.CommandText = @"
                    SELECT sr.pos 
                      FROM speedResults sr(NOLOCK)
                      JOIN lists lPrev(NOLOCK) ON sr.list_id = lPrev.iid
                     WHERE sr.climber_id = @clm
                       AND lPrev.next_round = " + listID.ToString();
            cmdl.Parameters.Add("@clm", SqlDbType.Int);
            int idc, p1 = -1, p2 = -1;
            object oTmp;
            if (int.TryParse(leftIID.Text, out idc))
            {
                cmdl.Parameters[0].Value = idc;
                oTmp = cmdl.ExecuteScalar();
                if (oTmp != null && oTmp != DBNull.Value)
                    p1 = Convert.ToInt32(oTmp);
            }
            if (int.TryParse(lblIID.Text, out idc))
            {
                cmdl.Parameters[0].Value = idc;
                oTmp = cmdl.ExecuteScalar();
                if (oTmp != null && oTmp != DBNull.Value)
                    p2 = Convert.ToInt32(oTmp);
            }
            if (p1 > 0 && p2 > 0)
                posToEnter = Math.Min(p1, p2);
            else if (p1 < 0 && p2 > 0)
                posToEnter = p2;
            else if (p1 > 0 && p2 < 0)
                posToEnter = p1;
            else
                posToEnter = int.MaxValue - 100;

            return posToEnter;
        }

        protected virtual void btnNextRound_Click(object sender, EventArgs e)
        {
            if (dtStart.Rows.Count > 0)
                if (MessageBox.Show("Ещё есть неотстартовавшие. Всё равно создать протокол?",
                    "Следующий раунд", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            ResultListSpeed.Pairing(listID, cn, (dtRes.Rows.Count < 5));
        }

        protected virtual void dgRes_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (!allowEvent)
                return;
            try
            {
                string n = dgRes.CurrentRow.Cells[1].Value.ToString();
                cmd.CommandText = "SELECT start FROM speedResults WHERE (climber_id=" + n + ") AND (list_id=" +
                    listID.ToString() + ")";
                int strt = Convert.ToInt32(cmd.ExecuteScalar());
                if ((strt % 2) == 0)
                    strt--;
                SetPair(strt, speedAdvancedSystem && runNo != null && runNo.Value > 0 && (runNo.Value % 2 == 0));
                seconRoute = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected virtual void dgStart_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (!allowEvent)
                return;
            try
            {
                int strt = Convert.ToInt32(dgStart.CurrentRow.Cells[0].Value);
                if ((strt % 2) == 0)
                    strt--;
                SetPair(strt, speedAdvancedSystem && runNo != null && runNo.Value > 0 && (runNo.Value % 2 == 0));
                seconRoute = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected void xlExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                bool isQf = false;
                const int frst_row = 6;
                foreach (DataRow dataRow in dtRes.Rows)
                    if (dataRow["Кв."].ToString() == "Q")
                    {
                        isQf = true;
                        break;
                    }

                int i = 0;

                foreach (DataColumn cl in dtRes.Columns)
                    if (((isQf) || (cl.ColumnName != "Кв.")) && cl.ColumnName != "Ст.№")
                    {
                        i++;
                        if (cl.ColumnName.ToLower() == "фамилияимя")
                            ws.Cells[frst_row, i] = "Фамилия, Имя";
                        else if (cl.ColumnName == "Сумма" && IsBestQualy())
                            ws.Cells[frst_row, i] = "Лучшее";
                        else
                            ws.Cells[frst_row, i] = cl.ColumnName;
                    }
                char lastColumnChar = (char)('A' + i - 1);
                char lastNotQualified = isQf ? (char)(lastColumnChar - 1) : lastColumnChar;

                i = frst_row;
                int lastFin = -1;
                int iG;
                if (dtRes.Columns[dtRes.Columns.Count - 1].ColumnName == "Ст.№")
                    iG = 1;
                else
                    iG = 0;
                foreach (DataRow rd in dtRes.Rows)
                {
                    i++;
                    for (int k = 1; k <= dtRes.Columns.Count - iG; k++)
                        ws.Cells[i, k] = rd[k - 1];
                    if ((rd["Г.р."].ToString() == "0"))
                        ws.Cells[i, 4] = "";
                    if (((rd["Кв."].ToString() != "Q") && ((rd["Кв."].ToString() != " q"))) && (lastFin == -1) && (i > 5))
                        lastFin = i - 1;
                    if (((rd["Кв."].ToString() == "Q") || ((rd["Кв."].ToString() == " q"))) && (lastFin != -1))
                        lastFin = -1;
                }
                Excel.Range dt;

                if ((lastFin > -1) && isQf)
                {
                    dt = ws.get_Range("A" + frst_row.ToString(), lastColumnChar + lastFin.ToString());
                    dt.Style = "MyStyle";
                    dt = ws.get_Range("A" + (lastFin + 1).ToString(), lastNotQualified + i.ToString());
                    dt.Style = "MyStyle";
                }
                else
                {
                    dt = ws.get_Range("A" + frst_row.ToString(), lastColumnChar + i.ToString());
                    dt.Style = "MyStyle";
                }
                dt = ws.get_Range("A" + frst_row.ToString(), lastColumnChar + i.ToString());
                dt.Columns.AutoFit();

                dt = ws.get_Range("C" + frst_row.ToString(), "C" + i.ToString());
                dt.Style = "StyleLA";

                char cTmp;
                cTmp = lastColumnChar;
                //if (isQf)
                //    cTmp = 'J';
                //else
                //    cTmp = 'I';

                dt = ws.get_Range("A1", cTmp + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                string strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];

                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];
                if (judgeID > 0)
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду:";

                dt = ws.get_Range(cTmp + "2", cTmp + "2");
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                dt.Style = "StyleRA";

                dt = ws.get_Range("A3", cTmp + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                string gName = "", round = "";
                cmd.CommandText = "SELECT g.name, l.round FROM lists l INNER JOIN groups g ON (l.group_id=g.iid) " +
                    "WHERE (l.iid = " + listID.ToString() + ")";
                try
                {
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        gName = rdr[0].ToString();
                        round = rdr[1].ToString();
                        break;
                    }
                    rdr.Close();
                }
                catch { }

                strTmp = "Скорость. " + gName;
                dt.Cells[1, 1] = strTmp;

                strTmp = "Протокол результатов. " + round;
                dt = ws.get_Range("A4", cTmp + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "рез_";

                strTmp += "СК_";

                try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, round); }
                catch { }

                StaticClass.ExcelUnderlineQf(ws);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StaticClass.SetNowClimbingNull(cn, listID);
            //if (sl != null)
            //    try { sl.StopListening(); }
            //    catch { }
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            loadData();
        }

        public static SystemListener getParentListener(Form current)
        {
            if (current.MdiParent != null && current.MdiParent is StaticClass.ExtendedForm)
            {
                StaticClass.ExtendedForm ef = (StaticClass.ExtendedForm)current.MdiParent;
                if (ef.SystListener != null)
                    return ef.SystListener;
            }
            return null;
        }

        public static void setParentListener(SystemListener slL, Form current)
        {
            if (current.MdiParent != null && current.MdiParent is StaticClass.ExtendedForm)
                ((StaticClass.ExtendedForm)current.MdiParent).SystListener = slL;
        }

        private void cbUseSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseSystem.Checked)
            {
                if(currentSystem == SettingsForm.SpeedSystemType.EKB || currentSystem == SettingsForm.SpeedSystemType.EKO)
                    if (tbComPort.Text == "")
                    {
                        MessageBox.Show("Введите номер последовательного порта");
                        cbUseSystem.Checked = false;
                        return;
                    }
                SystemListener prnL = getParentListener(this);
                bool crtNew = true;
                if(prnL != null)
                    switch (currentSystem)
                    {
                        case SettingsForm.SpeedSystemType.EKO:
                            goto case SettingsForm.SpeedSystemType.EKB;
                        case SettingsForm.SpeedSystemType.EKB:
                            if (!(prnL is EKBListener))
                            {
                                prnL.StopListening();
                                crtNew = true;
                                break;
                            }
                            EKBListener ekL = (EKBListener)prnL;
                            if (ekL.PortName != tbComPort.Text)
                            {
                                crtNew = (MessageBox.Show(this, "Уже создано подключение к скоростной системе на другом порту.\r\n" +
                                    "Вы уверены, что хотите изменить подключение?", "Изменить подключение", MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question) == DialogResult.Yes);
                            }
                            else
                                crtNew = (prnL == null);
                            break;
                        case SettingsForm.SpeedSystemType.IVN:
                            if (!(prnL is IvanovoListener))
                            {
                                prnL.StopListening();
                                crtNew = true;
                                break;
                            }
                            IvanovoListener ivL = (IvanovoListener)prnL;
                            crtNew = (MessageBox.Show(this, "Уже открыть подключение к системе по адресу " +
                                ivL.IP.ToString() + " на " + ivL.Port.ToString() + " порту. Создать новое подключение?",
                                "Изменить подключение", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.Yes);
                            break;
                    }
                if (crtNew)
                {
                    //sl = new RandomSystemListener(tbComPort.Text);
                    bool useOld = false;
                    switch (currentSystem)
                    {
                        case SettingsForm.SpeedSystemType.EKO:
                            useOld = true;
                            goto case SettingsForm.SpeedSystemType.EKB;
                        case SettingsForm.SpeedSystemType.EKB:
                            SerialPort sp = null;
                            appSettings apSet = appSettings.Default;
                            try
                            {
                                if (apSet.systemPort != null)
                                {
                                    sp = apSet.systemPort;
                                    if (sp.PortName != tbComPort.Text)
                                        sp = null;
                                }
                            }
                            catch { sp = null; }

                            if (sp == null)
                                sp = SerialPortConfig.ConfigurePort(tbComPort.Text, this);
                            if (sp == null)
                                return;
                            sl = new EKBListener(sp, useOld, this.MdiParent, SpeedSystemHelper.ShowErrors, SpeedSystemHelper.PersistShowErrors);
                            break;
                        case SettingsForm.SpeedSystemType.IVN:
                            IvanovoSet iSet = new IvanovoSet();
                            iSet.ShowDialog(this);
                            if (iSet.Cancel)
                                sl = null;
                            else
                                sl = new IvanovoListener(iSet.Address, iSet.Port, this.MdiParent, SpeedSystemHelper.ShowErrors, SpeedSystemHelper.PersistShowErrors);
                            break;
                    }
                }
                else
                    sl = prnL;
                if (sl != null && sl.StartListening())
                {
                    btnConfigPort.Enabled = tbComPort.Enabled = false;
                    appSettings aSet = appSettings.Default;
                    aSet.SysPort = tbComPort.Text;
                    aSet.Save();
                    //StaticClass.SaveDataToRegistry(StaticClass.SYS_PORT, tbComPort.Text);
                    btnShowSystem.Visible = true;
                    setParentListener(sl, this);
                }
                else
                    cbUseSystem.Checked = false;
                
            }
            else
            {
                btnConfigPort.Enabled = true;
                tbComPort.Enabled = (currentSystem == SettingsForm.SpeedSystemType.EKB);
                btnShowSystem.Visible = false;
                cbAutoSystem.Checked = false;
                sl = null;
            }
            cbAutoSystem.Enabled = cbUseSystem.Checked;
        }

        private void btnShowSystem_Click(object sender, EventArgs e)
        {
            if (speedAdvancedSystem && (runNo == null))
            {
                MessageBox.Show(this, "Выберите забег");
                return;
            }
            EnterDataFromSystem();
        }

        protected void SetEmpty(bool right)
        {
            if (right)
                lblAge.Text = lblIID.Text = lblName.Text = lblQf.Text = rightRoute1.Text = rightRoute2.Text =
                            lblStart.Text = rightSum.Text = lblTeam.Text = "";
            else
                leftAge.Text = leftIID.Text = leftName.Text = leftQf.Text = leftRoute1.Text = leftRoute2.Text =
                    leftStart.Text = leftSum.Text = leftTeam.Text = "";
        }

        protected virtual void dgRes_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void btnConfigPort_Click(object sender, EventArgs e)
        {
            switch (currentSystem)
            {
                case SettingsForm.SpeedSystemType.EKB:
                    if (tbComPort.Text.Length > 0)
                        SerialPortConfig.ConfigurePort(tbComPort.Text, this);
                    else
                        MessageBox.Show("Введите имя СОМ порта");
                    break;
                case SettingsForm.SpeedSystemType.IVN:
                    IvanovoSet iSet = new IvanovoSet();
                    iSet.ShowDialog();
                    if (iSet.Cancel)
                        return;
                    if (sl == null)
                        sl = new IvanovoListener(iSet.Address, iSet.Port, this.MdiParent, SpeedSystemHelper.ShowErrors, SpeedSystemHelper.PersistShowErrors);
                    else if (sl is IvanovoListener)
                    {
                        sl.StopListening();
                        ((IvanovoListener)sl).IP = iSet.Address;
                        ((IvanovoListener)sl).Port = iSet.Port;
                    }
                    sl.StartListening();
                    break;
            }

        }

        private void cbAutoSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoSystem.Checked)
            {
                if (speedAdvancedSystem && runNo == null)
                {
                    MessageBox.Show(this, "Выберите забег");
                    cbAutoSystem.Checked = false;
                    return;
                }
                if (sl == null)
                {
                    cbAutoSystem.Checked = false;
                    return;
                }
                if (MessageBox.Show(this, "Внимание! Вы собираетесь включить режим полностью автоматического ввода результатов\r\n" +
                    "Поиск случайных ошибок может занять дольше времени, чем подтверждение результата каждого участника.\r\n" +
                    "Сначала ОБЯЗАТЕЛЬНО проставьте все неявки в протоколе.\r\n" +
                    "Если Вы всё ещё хотите использовать полностью автоматический режим, то подтвердите Ваш выбор.",
                    "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    cbAutoSystem.Checked = false;
                    return;
                }
                if (btnEdit.Text.ToLower() == "правка")
                {
                    SetNextClimber();
                    //SetEditMode(true);
                }
            }
        }

        System.Threading.Mutex mSL = new System.Threading.Mutex();

        private void SpeedFinals_Shown(object sender, EventArgs e)
        {
            SystemListener prn = getParentListener(this);
            if (prn == null || prn is EKBListener)
            {
                if (prn != null)
                    tbComPort.Text = ((EKBListener)prn).PortName;
                else
                    tbComPort.Text = appSettings.Default.SysPort;
            }
        }

        protected virtual void btnRollBack_Click(object sender, EventArgs e)
        {
            long? activeTranID, nextTranID, toCheck;
            try
            {
                SetTransactionButtonsEnabled(out activeTranID, out nextTranID);
                toCheck = (sender == btnRollBack) ? activeTranID : nextTranID;
                string message = (sender == btnRollBack) ? "откатить" : "вернуть";
                if (toCheck == null || !toCheck.HasValue)
                {
                    MessageBox.Show(this, "Невозможно " + message + " результат");
                    return;
                }
                string climbersStr = leftName.Text;
                bool hasBothCLimbers = false;
                if (!leftName.Text.Equals(String.Empty) && !lblName.Text.Equals(String.Empty))
                {
                    hasBothCLimbers = true;
                    climbersStr = climbersStr + " и " + lblName.Text;
                }
                else if (!lblName.Text.Equals(String.Empty))
                    climbersStr = lblName.Text;
                if (MessageBox.Show(this, "Вы уверены, что хотите " + message + " участник" +
                    (hasBothCLimbers ? "ов" : "а") + " " + climbersStr + "?"
                    , message + " результат?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    return;
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    bool res;
                    if (sender == btnRollBack)
                        res = StaticClass.RollbackTransaction(toCheck.Value, cn, tran);
                    else
                        res = StaticClass.RestoreLogicTransaction(toCheck.Value, cn, tran);
                    if (res)
                    {
                        tran.Commit();
                        try { RefreshShownTables(); }
                        catch { }
                        RefreshData();
                        ClearAllClimbers();
                    }
                    else
                    {
                        MessageBox.Show("Выполнение команды завершилось внутренней ошибкой SQL Server");
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
            catch (Exception ex) { MessageBox.Show("Ошибка выполнения команды:\r\n" + ex.Message); }
        }

        protected virtual void ClearAllClimbers()
        {
            ClearClimberOnRoute(1);
            ClearClimberOnRoute(2);
        }

        protected virtual void ClearClimberOnRoute(byte route)
        {
            if (route < 1 || route > 2)
                throw new ArgumentOutOfRangeException("route");
            if (route == 1)
                leftAge.Text = leftIID.Text = leftName.Text = leftQf.Text =
                    leftRoute1.Text = leftRoute2.Text = leftStart.Text = leftSum.Text = leftTeam.Text = String.Empty;
            else
                lblAge.Text = lblIID.Text = lblName.Text = lblQf.Text =
                    rightRoute1.Text = rightRoute2.Text = lblStart.Text = rightSum.Text = lblTeam.Text = String.Empty;
        }

        //private void cbShowView_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (cbShowView.SelectedIndex < 0)
        //        return;
        //    int? newView = cbShowView.SelectedIndex > 0 ? new int?(cbShowView.SelectedIndex) : null;
        //    ChangeRunNo(newView);

        //    btnNextRound.Enabled = xlExport.Enabled = (this.runNo == null || this.runNo.Value < 1);
        //}

        protected virtual void dgStart_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void rb1stRun_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                int? newView;
                if (rb1stRun.Checked)
                    newView = 1;
                else if (rb2ndRun.Checked)
                    newView = 2;
                else if (rb3rdRun.Checked)
                    newView = 3;
                else
                    newView = null;
                ChangeRunNo(newView);

                btnNextRound.Enabled = xlExport.Enabled = (this.runNo == null || this.runNo.Value < 1);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
