// <copyright file="ResultListSpeed.cs">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;

namespace ClimbingCompetition
{
    /// <summary>
    /// Ввод результатов квалификации скорости без подключения скоростной системы
    /// </summary>
    public partial class ResultListSpeed : BaseListForm
    {
        //SqlConnection cn;
        int judgeID;
        string gName = "", round = "";
        SqlDataAdapter daStart = new SqlDataAdapter(), daRes = new SqlDataAdapter();
        DataTable dtRes = new DataTable();
        DataTable dtStart = new DataTable();
        SqlCommand cmd = new SqlCommand();
        bool allowEvent = true, wrk_done, preQfClimb;
        int baseIID = 0;
        bool IsEditing = false;

        bool IsSecondAndNeed1st = false;
        SpeedRules Rules;

        bool UseBestRoute { get { return (Rules & SpeedRules.BestRouteInQfRound) == SpeedRules.BestRouteInQfRound; } }
        bool UseBestQfFromTwo { get { return (Rules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds; } }

        public ResultListSpeed(int listID, SqlConnection baseCon, string competitionTitle)
            : base(baseCon, competitionTitle,listID)
        {
            Rules = SettingsForm.GetSpeedRules(cn);
            preQfClimb = SettingsForm.GetPreQfClimb(cn, listID);
            AccountForm.CreateSpeedAdvancedFormatTable(cn);
            InitializeComponent();
            this.ToolTipDefault.SetToolTip(btnRestore, ResultListLead.RES_RESTORE_STRING);
            this.ToolTipDefault.SetToolTip(btnRollBack, ResultListLead.RES_ROLLBACK_STRING);
            //this.cn = cn;
            cmd.Connection = this.cn;
            cmd.CommandText = "SELECT g.name, l.round, l.prev_round, l.judge_id FROM lists l(NOLOCK) JOIN groups g(NOLOCK) ON l.group_id=" +
                "g.iid WHERE l.iid=" + this.listID.ToString();
            try
            {
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    gName = rdr[0].ToString();
                    round = rdr[1].ToString();
                    try { baseIID = Convert.ToInt32(rdr[2]); }
                    catch { baseIID = 0; }
                    try { judgeID = Convert.ToInt32(rdr["judge_id"]); }
                    catch { judgeID = 0; }
                    break;
                }
                rdr.Close();
                bool getUseBestQf = (Rules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds;
                IsSecondAndNeed1st = round.Equals("Квалификация 2", StringComparison.InvariantCultureIgnoreCase) &&
                    getUseBestQf;
            }
            catch { }
            daStart.SelectCommand = new SqlCommand();
            daStart.SelectCommand.Connection = this.cn;
            daRes.SelectCommand = new SqlCommand();
            daRes.SelectCommand.Connection = this.cn;
            dgRes.DataSource = dtRes;
            dgStart.DataSource = dtStart;
            rbBothRoutes.Checked = true;
            SetEditMode(false);
            this.Text = StaticClass.GetListName(listID, cn);
#if !DEBUG
            btnPrint.Visible = btnPrintPreview.Visible = false;
#endif
            RefreshTable();
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                try { btnNextRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                catch { btnNextRound.Enabled = true; }
            }
            else
                btnNextRound.Enabled = false;
            rbQf2.Enabled = rdPairs.Enabled = btnNextRound.Enabled;
            try
            {
                rbSystem.Enabled = (SettingsForm.GetCurrentSystem(cn) != SettingsForm.SpeedSystemType.NON);
            }
            catch { rbSystem.Enabled = false; }
        }

        /// <summary>
        /// Sets work mode for this list
        /// </summary>
        public WorkMode WM
        {
            set
            {
                wrk_done = false;
                switch (value)
                {
                    case WorkMode.ROUTE_1:
                        rbRoute1.Checked = true;
                        break;
                    case WorkMode.ROUTE_2:
                        rbRoute2.Checked = true;
                        break;
                    case WorkMode.BOTH_ROUTES:
                        rbBothRoutes.Checked = true;
                        break;
                    case WorkMode.SYSTEM:
                        rbSystem.Checked = true;
                        break;
                }
                Thread.Sleep(100);
                if (!wrk_done)
                {
                    if (rbSystem.Checked)
                        rbSystem_CheckedChanged(null, null);
                    else
                        rbRoute1_CheckedChanged(null, null);
                }
            }
        }

        private void SetTransactionButtonsEnabled(SqlTransaction tran = null)
        {
            long? l1, l2;
            SetTransactionButtonsEnabled(out l1, out l2, tran);
        }

        private void SetTransactionButtonsEnabled(out long? activeTranID, out long? nextTranID, SqlTransaction tran = null)
        {
            activeTranID = null;
            nextTranID = null;
            btnRollBack.Enabled = btnRestore.Enabled = false;
            int climberID;
            if (!int.TryParse(lblIID.Text, out climberID))
                return;
            GetSpeedTransactions(climberID, listID, out activeTranID, out nextTranID, cn, null);
            btnRollBack.Enabled = (activeTranID != null && activeTranID.HasValue);
            btnRestore.Enabled = (nextTranID != null && nextTranID.HasValue);
        }

        public static void GetSpeedTransactions(int climber_id, int list_id, out long? activeTranID, out long? nextTranID,
            SqlConnection cn, SqlTransaction tran = null)
        {
            activeTranID = null;
            nextTranID = null;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT iid FROM speedResults(NOLOCK) WHERE climber_id = " + climber_id.ToString() + " AND list_id=" + list_id.ToString();
            
            object res = cmd.ExecuteScalar();
            if (res == null || res == DBNull.Value)
                return;
            long resId = Convert.ToInt64(res);
            activeTranID = StaticClass.GetActiveTransaction(resId, "speedResults", cn, tran);
            nextTranID = StaticClass.GetNextTransaction(resId, "speedResults", cn, tran);
        }

        public static long? GetClimbersResSpeed(int climberID, int listID, SqlConnection cn,
            out Dictionary<string, string> oldValues, SqlTransaction tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT iid FROM speedresults(NOLOCK) WHERE climber_id=" + climberID.ToString() +
                " AND list_id=" + listID.ToString();
            object res = cmd.ExecuteScalar();
            if (res == null || res == DBNull.Value)
            {
                oldValues = new Dictionary<string, string>();
                return null;
            }
            GetClimbersResSpeed(Convert.ToInt64(res), cn, out oldValues, tran);
            return new long?(Convert.ToInt64(res));
        }

        public static void GetClimbersResSpeed(long resID, SqlConnection cn,
            out Dictionary<string,string> oldValues, SqlTransaction tran = null)
        {
            StaticClass.GetTableData("speedResults", "iid", resID, cn, out oldValues, tran);
        }


        private void RefreshData()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                dtRes.Clear();
                dtStart.Clear();
                dtStart.Columns.Clear();
                daRes.Fill(dtRes);
                daStart.Fill(dtStart);
                if (rbRoute2.Checked)
                {
                    try { dgStart.Columns["ps"].Visible = false; }
                    catch { }
                }
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static long ParseString(ref string res)
        {
            return ParseString(ref res, false);
        }

        public static long ParseString(ref string res, bool secRoute)
        {
            if (res == null)
                res = String.Empty;
            else
                res = res.ToLower().Trim();
            if (res.IndexOf("срыв") > -1)
            {
                res = "срыв";
                return 6000000;
            }
            if (res.IndexOf("дискв") > -1)
            {
                res = "дискв.";
                if (secRoute)
                    return 6000000;
                else
                    return 7000000;
            }
            if (res.IndexOf("н/я") > -1 || String.IsNullOrEmpty(res))
            {
                if (!String.IsNullOrEmpty(res))
                    res = "н/я";
                return 8000000;
            }
            int i;
            int[] results = new int[3];
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            for (i = 0; i < 3; i++)
                results[i] = 0;
            string strTmp = "";
            int indx, indxNxt;
            for (i = 2; i > -1; i--)
            {
                indx = res.LastIndexOfAny(digits);
                if (indx >= 0)
                    strTmp = res[indx].ToString();
                else
                    break;
                res = res.Substring(0, indx);
                while (res.Length >= 0)
                {
                    indxNxt = res.LastIndexOfAny(digits);
                    if ((indxNxt >= 0) && (indxNxt == (indx - 1)))
                    {
                        strTmp += res[indxNxt].ToString();
                        res = res.Substring(0, indxNxt);
                        indx--;
                    }
                    else
                        break;
                }
                int k = 1;
                foreach (char c in strTmp)
                    for (int kk = 0; kk < digits.Length; kk++)
                        if (digits[kk] == c)
                        {
                            results[i] += k * kk;
                            k *= 10;
                            break;
                        }
            }
            long resTotal = (results[2] + results[1] * 100 + results[0] * 6000);
            res = CreateString(resTotal);
            return resTotal * 100;
        }

        private void rbRoute1_CheckedChanged(object sender, EventArgs e)
        {
            wrk_done = true;
            tbRoute1.Text = tbRoute2.Text = tbSum.Text = "";
            string sSelect1st, sSelect2nd, sOrderBy;
            if (IsSecondAndNeed1st)
            {
                sSelect1st = " dbo.fn_get1st(l.climber_id, l.list_id) AS [Квал.1], ";
                sSelect2nd = " dbo.fn_getBest(l.iid) AS [Квал.итог], ";
                sOrderBy = " dbo.fn_getBestRes(l.iid), ";
            }
            else
            {
                sSelect2nd = sSelect1st = String.Empty;
                sOrderBy = " l.res, ";
            }
            if (rbRoute1.Checked)
            {
                daStart.SelectCommand.CommandText =
                    "  SELECT l.start AS [Ст.№], l.climber_id AS [№], " +
                    "         p.surname+' '+p.name AS [ФамилияИмя], p.age AS [Г.р.], p.qf AS [Разряд], " +
                    "         dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " + sSelect1st +
                    "         l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2] " +
                    "    FROM speedResults l(NOLOCK) " +
                    "    JOIN Participants p(NOLOCK) ON l.climber_id = p.iid" +
                    "   WHERE l.list_id = " + listID.ToString() +
                    (preQfClimb ? String.Empty : "     AND l.preQf = 0 ") +
                    "     AND ISNULL(route1_text,'') = '' " +
                    "ORDER BY l.start";
                daRes.SelectCommand.CommandText =
                    "  SELECT l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                            " p.name) AS [ФамилияИмя], p.age AS [Г.р.], p.qf AS [Разряд], "+
                            " dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " + sSelect1st +
                            " l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2]," +
                            " l.resText AS [Сумма], " + sSelect2nd + " l.qf AS [Кв.]" +
                    "    FROM speedResults l(NOLOCK)" +
                    "    JOIN Participants p(NOLOCK) ON l.climber_id = p.iid" +
                   "    WHERE (l.list_id = " + listID.ToString() + ")" +
                   "      AND ISNULL(route1_text,'') <> '' " +
                   " ORDER BY " + sOrderBy + " l.pos, p.vk, [Команда], p.surname, p.name";
            }
            else
            {
                daRes.SelectCommand.CommandText =
                    "   SELECT l.posText AS [Место], l.climber_id AS [№], " +
                    "          p.surname + ' ' + p.name AS [ФамилияИмя], p.age AS [Г.р.], " +
                    "          p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " + sSelect1st +
                    "          l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2]," +
                    "          l.resText AS [Сумма], " + sSelect2nd + " l.qf AS [Кв.] " +
                    "     FROM speedResults l(NOLOCK) " +
                    "     JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "    WHERE l.list_id = " + listID.ToString();
                if (UseBestRoute)
                {
                    daRes.SelectCommand.CommandText +=
                        "      AND (ISNULL(l.route2_text,'') <> '' ";
                    if (rbBothRoutes.Checked)
                        daRes.SelectCommand.CommandText +=
                            " AND ISNULL(l.route1_text,'') <> '' ";
                    daRes.SelectCommand.CommandText +=
                        "       OR CHARINDEX('н/я', l.resText) > 0) ";
                }
                else
                    daRes.SelectCommand.CommandText +=
                        "      AND ISNULL(l.resText,'') <> '' ";
                daRes.SelectCommand.CommandText +=
                " ORDER BY " + sOrderBy + " l.pos, p.vk, l.route1, [Команда], p.surname, p.name";

                if (rbRoute2.Checked)
                {
                    daStart.SelectCommand.CommandText =
                        "   SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+p.name) AS [ФамилияИмя]," +
                        "          p.age AS [Г.р.], p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " + sSelect1st +
                        "          l.route1_text AS [Трасса 1], " +
                        "          CASE WHEN l.route1_text IS NULL THEN 1 " +
                        "               WHEN l.route1_text = '' THEN 1 " +
                        "               ELSE 0 END ps, l.route2_text AS [Трасса 2] " +
                        "     FROM speedResults l(NOLOCK) " +
                        "     JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                        "    WHERE l.list_id = " + listID.ToString();
                    if (UseBestRoute)
                        daStart.SelectCommand.CommandText +=
                            "      AND ISNULL(l.route2_text,'') = '' " +
                            "      AND (l.resText IS NULL OR CHARINDEX('н/я', l.resText) < 1) ";
                    else
                        daStart.SelectCommand.CommandText +=
                            "      AND ISNULL(l.resText,'') = '' ";
                    daStart.SelectCommand.CommandText +=
                        (preQfClimb ? String.Empty : "      AND l.preQf = 0 ") +
                        //"      AND l.route1_text IS NOT NULL "+
                            " ORDER BY ps, l.start";
                }
                else
                {
                    daStart.SelectCommand.CommandText =
                        "  SELECT l.start AS [Ст.№], l.climber_id AS [№], " +
                        "         p.surname + ' ' + p.name AS [ФамилияИмя], p.age AS [Г.р.], " +
                        "         p.qf AS [Разряд], dbo.fn_getTeamName(p.iid, l.list_id) AS [Команда], " + sSelect1st +
                        "         l.route1_text AS [Трасса 1], l.route2_text AS [Трасса 2] " +
                        "    FROM speedResults l(NOLOCK) " +
                        "    JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                        "   WHERE l.list_id = " + listID.ToString();
                    if (UseBestRoute)
                        daStart.SelectCommand.CommandText +=
                            "      AND (ISNULL(l.route2_text,'') = '' " +
                            "       OR ISNULL(l.route1_text,'') = '') " +
                            "      AND (l.resText IS NULL OR CHARINDEX('н/я', l.resText) < 1) ";
                    else
                        daStart.SelectCommand.CommandText +=
                            "      AND ISNULL(l.resText,'') = '' ";
                    daStart.SelectCommand.CommandText +=
                        (preQfClimb ? String.Empty : "     AND l.preQf = 0 ") +
                    "ORDER BY CASE WHEN l.route1 IS NULL THEN 1 ELSE 0 END, l.start";
                }
            }
            RefreshData();
        }

        private enum typeEnter { FIRST, SECOND, BOTH }

        private bool EnterFirstRoute()
        {
            string route1 = tbRoute1.Text;
            long r1 = ParseString(ref route1);
            tbRoute1.Text = route1;
            if (r1 >= 6000000 && !UseBestRoute)
            {
                tbRoute2.Text = "";
                tbSum.Text = route1;
                return EnterBothRoutes();
            }
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = "UPDATE speedResults SET route1=@r1,route1_text=@r1t WHERE " +
     "(list_id=" + listID.ToString() + ") AND (climber_id=" + lblIID.Text + ")";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@r1", SqlDbType.BigInt);
                    cmd.Parameters[0].Value = r1;
                    cmd.Parameters.Add("@r1t", SqlDbType.VarChar);
                    cmd.Parameters[1].Value = route1;
                    var pR = cmd.ExecuteNonQuery();

                    if (UseBestRoute && pR > 0)
                    {
                        long resToSet = r1;
                        string restextToSet = route1;
                        cmd.CommandText = "SELECT ISNULL(route2,-1) route2, ISNULL(route2_text,'') route2_text" +
                                          "  FROM speedResults(NOLOCK)" +
                                          " WHERE list_id=" + listID.ToString() +
                                          "   AND climber_id=" + lblIID.Text;
                        long res2 = -1;
                        string res2t = String.Empty;
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                res2 = Convert.ToInt64(dr["route2"]);
                                if (res2 > 0)
                                    res2t = (string)dr["route2_text"];
                            }
                        }
                        if (res2 > 0 && res2 < resToSet)
                        {
                            resToSet = res2;
                            restextToSet = res2t;
                        }
                        cmd.CommandText = "UPDATE speedResults SET res=@r1, resText=@r1t WHERE list_id=" + listID.ToString() +
                            " AND climber_id=" + lblIID.Text;
                        cmd.Parameters[0].Value = resToSet;
                        cmd.Parameters[1].Value = restextToSet;
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                    throw ex;
                }
                finally { cmd.Transaction = null; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода результата: " + ex.Message);
                return false;
            }
            return true;
        }

        private bool EnterBothRoutes()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
            string route1 = tbRoute1.Text, route2 = tbRoute2.Text, sum = tbSum.Text;
            long r1 = 0, r2 = 0, nSum = 0;

            if ((!String.IsNullOrEmpty(route1) || UseBestRoute) && !String.IsNullOrEmpty(route2))
            {
                r1 = ParseString(ref route1);
                //if (r1 >= 6000000)
                //    return EnterFirstRoute();
                r2 = ParseString(ref route2, true);
                if (UseBestRoute)
                {
                    nSum = Math.Min(r1, r2);
                    sum = (r1 < r2) ? route1 : route2;
                }
                else if (r2 < 6000000)
                {
                    nSum = r1 + r2;
                    sum = CreateString(nSum / 100);
                }
                else
                {
                    /*if (baseIID > 0)----------------------------------------------------------*/
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
                    r1 = ParseString(ref route1);
                    nSum = ParseString(ref sum, true);
                    if (r1 < 6000000)
                    {
                        if (nSum < 6000000)
                        {
                            r2 = nSum - r1;
                            route2 = CreateString(r2 / 100);
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

            tbRoute1.Text = route1;
            tbRoute2.Text = route2;
            tbSum.Text = sum;

            if (!UseBestRoute && (nSum < r1 || nSum < r2))
            {
                MessageBox.Show("Проверьте правильность ввода суммы");
                return false;
            }

            int prevPos = 0;
            cmd.Parameters.Clear();
            //if (baseIID > 0)
            //{
            //    cmd.CommandText = "SELECT pos FROM speedResults WHERE (list_id=" + baseIID.ToString() + ") AND (climber_id=" +
            //        lblIID.Text + ")";
            //    try { prevPos = Convert.ToInt32(cmd.ExecuteScalar()); }
            //    catch { prevPos = 99; }
            //}

            cmd.CommandText = "UPDATE speedResults SET res=@res,route1=@r1,route1_text=@r1t,route2=@r2," +
                "route2_text=@r2t, resText=@rt WHERE (list_id=" + listID.ToString() + ") AND (climber_id=" + lblIID.Text + ")";

            cmd.Parameters.Add("@res", SqlDbType.BigInt);

            cmd.Parameters[0].Value = nSum + prevPos;
            cmd.Parameters.Add("@r1", SqlDbType.BigInt);
            cmd.Parameters[1].Value = r1;
            cmd.Parameters.Add("@r2", SqlDbType.BigInt);
            cmd.Parameters[2].Value = r2;
            cmd.Parameters.Add("@rt", SqlDbType.VarChar);
            cmd.Parameters[3].Value = tbSum.Text;
            cmd.Parameters.Add("@r1t", SqlDbType.VarChar);
            cmd.Parameters[4].Value = tbRoute1.Text;
            cmd.Parameters.Add("@r2t", SqlDbType.VarChar);
            cmd.Parameters[5].Value = tbRoute2.Text;

            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public static bool ClearClimber(int route, string id, int listID, SqlConnection cn)
        {
            if (String.IsNullOrEmpty(id))
                return false;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    switch (route)
                    {
                        case 1:
                            cmd.CommandText = "UPDATE speedResults SET pos=" + int.MaxValue + ",posText=NULL,res=" +
                                long.MaxValue + ",resText=NULL,route1=NULL," +
                                "route1_text=NULL,qf=NULL WHERE list_id=" + listID.ToString() +
                                " AND climber_id=" + id;
                            break;
                        case 2:
                            cmd.CommandText = "UPDATE speedResults SET pos=" + int.MaxValue + ",posText=NULL,res=" +
                                long.MaxValue + ",resText=NULL,route2=NULL," +
                                "route2_text=NULL,qf=NULL WHERE list_id=" + listID.ToString() +
                                " AND climber_id=" + id;
                            break;
                        case 3:
                            cmd.CommandText = "UPDATE speedResults SET pos=" + int.MaxValue + ",posText=NULL,res=" +
                                long.MaxValue + ",resText=NULL,route1=NULL,route2=NULL,route1_text=NULL," +
                                "route2_text=NULL,qf=NULL WHERE list_id=" + listID.ToString() +
                                " AND climber_id=" + id;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Неверный параметр route");
                    }
                    if (cmd.CommandText.Length > 0)
                        cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception exInner)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                    throw exInner;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления БД\r\n" + ex.Message);
                return false;
            }
        }
        private delegate bool func();
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "Правка")
            {
                //if (IsEditing && !(rbBothRoutes.Checked || rbRoute2.Checked))
                //{
                //    MessageBox.Show("Для правки результатов выберите режим \"Обе трассы / правка\"");
                //    return;
                //}
                SetEditMode(true, !IsEditing);
                return;

            }
            Dictionary<string, string> oldVals, newVals;
            long? resID = null;
            try
            {
                resID = GetClimbersResSpeed(int.Parse(lblIID.Text),
                    listID, cmd.Connection, out oldVals, cmd.Transaction);
            }
            catch { return; }

            try
            {



                if (tbRoute1.Text.IndexOf("н/я") > -1 ||
                    !UseBestRoute && (
                    !UseBestQfFromTwo && (tbRoute1.Text.IndexOf("дискв.") > -1 ||
                    tbRoute1.Text.IndexOf("срыв") > -1)))
                {
                    tbRoute2.Text = tbSum.Text = "";
                    if (EnterFirstRoute())
                    {
                        RefreshTable();
                        SetEditMode(false);
                        RefreshData();
                    }
                    return;
                }
                if (rbRoute1.Checked)
                {
                    //tbSum.Text = "";
                    //tbRoute2.Text = "";
                    if (tbRoute1.Text == "")
                    {
                        try
                        {
                            if (MessageBox.Show("Вы действительно хотите удалить результат участника " +
                                StaticClass.GetClimberName(lblIID.Text, cn) + " на 1 трассе?", "Удалить результат",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                DialogResult.Yes)
                            {
                                if (ClearClimber(1, lblIID.Text, this.listID, cn))
                                {
                                    RefreshTable();
                                    SetEditMode(false);
                                    RefreshData();
                                }
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка удаления результата\r\n" + ex.Message);
                            return;
                        }
                        MessageBox.Show("Пожалуйста, введите время на трассе 1.");
                        return;
                    }
                    func f;
                    tbRoute1.Text = tbRoute1.Text.ToLower();
                    bool bN = (tbRoute1.Text.IndexOf("срыв") > -1 ||
                               tbRoute1.Text.IndexOf("н/я") > -1 ||
                               tbRoute1.Text.IndexOf("дискв.") > -1);
                    if (bN)
                        tbRoute2.Text = tbSum.Text = "";
                    if (!UseBestRoute && (tbRoute2.Text.Length > 0 || tbSum.Text.Length > 0))
                    {
                        if (tbSum.Text.IndexOf("срыв") > -1 ||
                               tbSum.Text.IndexOf("н/я") > -1 ||
                               tbSum.Text.IndexOf("дискв.") > -1)
                        {
                            tbSum.Text = "";
                            ClearClimber(3, lblIID.Text, listID, cn);
                            f = EnterFirstRoute;
                        }
                        else
                        {
                            f = EnterBothRoutes;
                            if (tbSum.Text.Length > 0 && tbRoute2.Text.Length > 0)
                                tbSum.Text = "";
                        }
                    }
                    else
                        f = EnterFirstRoute;
                    if (f())
                    {
                        RefreshTable();
                        SetEditMode(false);
                        RefreshData();
                    }
                }
                else
                {
                    if ((rbRoute2.Checked && (tbRoute2.Text == "") && (tbSum.Text == "")) ||
                        (tbRoute1.Text.Length > 0) && tbRoute2.Text.Length < 1 && tbSum.Text.Length < 1)
                    {
                        if (MessageBox.Show("Удалить результат участника " + lblName.Text + "на ВТОРОЙ трассе?",
                            "Удалить результат?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                        {
                            ClearClimber(2, lblIID.Text, listID, cn);
                            RefreshTable();
                            SetEditMode(false);
                            RefreshData();
                            return;
                        }
                        else
                            MessageBox.Show("Пожалуйста, введите либо сумму, либо время на трассе 2.");
                        return;
                    }
                    long prevPos = 0;
                    if (rbBothRoutes.Checked)
                    {
                        if (tbRoute1.Text == "" && tbRoute2.Text == "" && tbSum.Text == "")
                            if (MessageBox.Show("Удалить результат участника " + lblName.Text + "?", "Удалить результат?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                            {
                                ClearClimber(3, lblIID.Text, listID, cn);
                                RefreshTable();
                                SetEditMode(false);
                                RefreshData();
                                return;
                            }
                            else
                            {
                                if ((tbRoute1.Text == "срыв") || (tbRoute1.Text == "дискв.") || (tbRoute1.Text == "н/я"))
                                {
                                    prevPos = 10;
                                    tbSum.Text = tbRoute1.Text;
                                }
                                if (tbRoute1.Text != "")
                                    prevPos++;
                                if (tbRoute2.Text != "")
                                    prevPos++;
                                if (tbSum.Text != "")
                                    prevPos++;
                                if (prevPos < 2)
                                {
                                    MessageBox.Show("Пожалуйста, введите 2 любых результата.");
                                    return;
                                }
                            }
                    }
                    if (EnterBothRoutes())
                    {
                        cmd.CommandText = "UPDATE lists SET nowClimbing3 = " + lblIID.Text + " WHERE iid = " + listID.ToString();
                        try { cmd.ExecuteNonQuery(); }
                        catch { }
                        if (rbRoute2.Checked)
                        {
                            cmd.CommandText = "UPDATE lists SET nowClimbingTmp = NULL WHERE iid = " + listID.ToString();
                            try { cmd.ExecuteNonQuery(); }
                            catch { }
                        }
                        RefreshTable();
                        SetEditMode(false);
                        RefreshData();
                    }
                }
            }
            catch { }
            finally
            {
                try
                {
                    if (resID != null && resID.HasValue)
                    {
                        GetClimbersResSpeed(resID.Value, cmd.Connection, out newVals, cmd.Transaction);

                        AddTransaction(resID.Value, oldVals, newVals, cn, -1, cmd.Transaction, true);
                    }

                }
                catch { }
            }
        }

        private void UpdateNowCl()
        {
            if (rbRoute1.Checked)
                cmd.CommandText = "UPDATE lists SET nowClimbing = " + lblIID.Text + " WHERE iid=" + listID.ToString();
            if (rbRoute2.Checked)
                cmd.CommandText = "UPDATE lists SET nowClimbingTmp = " + lblIID.Text + " WHERE iid=" + listID.ToString();
            if (rbBothRoutes.Checked)
                cmd.CommandText = "UPDATE lists SET nowClimbing = NULL, nowClimbingTmp = NULL WHERE iid = " + listID.ToString();
            try { cmd.ExecuteNonQuery(); }
            catch { }
        }

        private void RefreshTable()
        {
            RefreshTable(listID, baseIID, RefreshData, cn, CR);
        }
        public delegate void RefreshDataDelegate();

        public static DataTable getPrevRound(SqlConnection cn, int listID, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT ISNULL(prev_round,0) prev_round FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
            int prvR;
            try { prvR = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { return null; }
            if (prvR < 1)
                return null;
            cmd.CommandText = "SELECT climber_id, preQf, ISNULL(pos," + int.MaxValue + ") pos " +
                               " FROM speedResults(NOLOCK) WHERE list_id=" + prvR.ToString();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count < 1)
                return null;
            else
                return dt;
        }

        public static void RefreshTable(int listID, int baseIID, RefreshDataDelegate RefreshData, SqlConnection cn, SpeedRules cRules,
            SqlTransaction tran = null, bool needRefresh = true)
        {
#if !DEBUG
            try
            {
#endif
            bool preQfClimb = SettingsForm.GetPreQfClimb(cn, listID, tran);
                SqlCommand cmd = new SqlCommand();
                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT round FROM lists(NOLOCK) WHERE iid=" + listID.ToString();
                string rnd = cmd.ExecuteScalar() as string;

                bool useBestQf = (SettingsForm.GetSpeedRules(cn, tran) & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds;

                bool GetBestQf = (rnd.Equals("Квалификация 2", StringComparison.InvariantCultureIgnoreCase) && useBestQf);

                bool ForceQf = (rnd.Equals("Квалификация", StringComparison.InvariantCultureIgnoreCase) && useBestQf);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = cn;
                da.SelectCommand.Transaction = tran;
                string sSel, sOrderBy, sWhere;
                if (GetBestQf)
                {
                    sSel = " dbo.fn_getBestRes(l.iid) AS res ";
                    sOrderBy = " dbo.fn_getBestRes(l.iid) ";
                    sWhere = " OR dbo.fn_getBest(l.iid) <> '' ";
                }
                else
                {
                    sSel = " l.res ";
                    sOrderBy = " l.res ";
                    sWhere = String.Empty;
                }

            da.SelectCommand.CommandText = @"
                                    SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText," +
                                       (GetBestQf ? "0" : "CASE l.resText WHEN 'н/я' THEN 2 WHEN 'дискв.' THEN 1 ELSE 0 END") + " sp, " +
                                       (preQfClimb ? "l.pos" : "CASE l.preQf WHEN 0 THEN l.pos ELSE 0 END") + " pos0, " + sSel + @"
                                      FROM speedResults l(NOLOCK)
                                      JOIN Participants c(NOLOCK) ON c.iid = l.climber_id " +
                              " WHERE (l.resText IS NOT NULL " + sWhere + @")
                                       AND l.list_id = " + listID.ToString() +
                            " ORDER BY " + sOrderBy;
                DataTable dt = new DataTable();
                da.Fill(dt);



                if (dt.Rows.Count < 1)
                    return;
                long curRes = Convert.ToInt64(dt.Rows[0]["res"]);
                int curPos = 1;
                int curPosCnt = 1;
                dt.Rows[0]["pos0"] = curPos;
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    long fRes = Convert.ToInt64(dt.Rows[i]["res"]);
                    if (fRes == curRes)
                        curPosCnt++;
                    else
                    {
                        curPos += curPosCnt;
                        curPosCnt = 1;
                        curRes = fRes;
                    }
                    dt.Rows[i]["pos0"] = curPos;
                }
                DataTable dtPr;
                if (GetBestQf)
                    dtPr = null;
                else
                    dtPr = getPrevRound(cn, listID, tran);
                if (dtPr != null)
                {
                    dt.Columns.Add("rNum1", typeof(int));
                    dt.Columns.Add("pos1", typeof(int));

                    foreach (DataRow drDt in dt.Rows)
                        foreach (DataRow dr in dtPr.Rows)
                            if (dr["climber_id"].ToString() == drDt["iid"].ToString())
                            {
                                drDt["pos1"] = Convert.ToInt32(dr["pos"]);
                                drDt["rNum1"] = preQfClimb ? 0 : Convert.ToInt32(dr["preQf"]);
                                break;
                            }
                }

                dt.Columns.Remove("res");

                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT ISNULL(quote,0) quote FROM lists(NOLOCK) WHERE iid = " + listID.ToString();
                int quote;
                try { quote = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch { quote = 0; }
                CompetitionRules compRules = ((cRules & SpeedRules.InternationalRules) == SpeedRules.InternationalRules) ? CompetitionRules.International : CompetitionRules.Russian;
                SortingClass.SortResults(dt, quote, dtPr == null, false, compRules, ForceQf);

                int qfCnt = 0;
                int lstPs = 0;

                foreach (DataRow dr in dt.Rows)
                    if (dr["qf"].ToString().Length > 0)
                    {
                        if (Convert.ToBoolean(dr["vk"]))
                            dr["qf"] = " q";
                        else
                        {
                            lstPs = Convert.ToInt32(dr["pos"]);
                            qfCnt++;
                        }
                    }
                if (qfCnt > quote)
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToInt32(dr["pos"]) == lstPs)
                            dr["qf"] = "";
                    }

                da.UpdateCommand = new SqlCommand("UPDATE speedResults SET " +
                    "pos=@pos, posText=@posText, qf=@qf " +
                    "WHERE (climber_id=@cid) AND (list_id=@lid)", cn);
                da.UpdateCommand.Transaction = tran;
                da.UpdateCommand.Parameters.Add("@lid", SqlDbType.Int);
                da.UpdateCommand.Parameters[0].Value = listID;
                da.UpdateCommand.Parameters.Add("@cid", SqlDbType.Int, 4, "iid");
                da.UpdateCommand.Parameters.Add("@pos", SqlDbType.Int, 8, "pos");
                da.UpdateCommand.Parameters.Add("@posText", SqlDbType.VarChar, 50, "posText");
                da.UpdateCommand.Parameters.Add("@qf", SqlDbType.VarChar, 50, "qf");


                da.InsertCommand = da.UpdateCommand;
                da.Update(dt);
                if (GetBestQf)
                    sWhere = "ltrim(rtrim(dbo.fn_getBest(SR.iid)))";
                else
                    sWhere = "ltrim(rtrim(SR.resText))";
                bool setPosToFalls = SettingsForm.GetPosToFalls(cmd.Connection, cmd.Transaction);
                int totalCount;
                if (setPosToFalls)
                {
                    cmd.CommandText = @"SELECT count(*) cnt
                                      FROM speedResults SR(nolock)
                                      JOIN Participants P(nolock) on P.iid = SR.climber_id
                                     WHERE SR.list_id = " + listID.ToString() +
                                       "   AND P.vk = 0" +
                                       "   AND (" + sWhere + " <> 'н/я' " + (preQfClimb ? " OR SR.preQf = 1" : String.Empty) + ")";
                    object oTmp = cmd.ExecuteScalar();
                    if (oTmp != null && oTmp != DBNull.Value)
                        totalCount = Convert.ToInt32(oTmp);
                    else
                        totalCount = -1;
                }
                else
                    totalCount = -1;
                if (dtPr == null)
                {

                    cmd.CommandText = "UPDATE speedResults SET posText = '' FROM speedResults SR(NOLOCK) WHERE list_id = " + listID.ToString() +
                        " AND " + sWhere + " IN('срыв','дискв.','н/я') ";
                    cmd.ExecuteNonQuery();
                    if (setPosToFalls && totalCount > 0)
                    {
                        cmd.CommandText = "UPDATE speedResults" +
                                          "   SET posText = CASE P.vk WHEN 0 then '" + totalCount.ToString() + "'" +
                                          "                           ELSE 'в/к' END," +
                                          "       pos = " + totalCount.ToString() +
                                          "  FROM speedResults SR(nolock)" +
                                          "  JOIN Participants P(nolock) on P.iid = SR.climber_id" +
                                          " WHERE SR.list_id = " + listID.ToString() +
                                          "   AND " + sWhere + " IN('срыв','дискв.')";
                        cmd.ExecuteNonQuery();
                    }
                }
                if (!ForceQf)
                {
                    cmd.CommandText = "UPDATE speedResults SET qf = '' FROM speedResults SR(NOLOCK) WHERE SR.list_id = " + listID.ToString() +
                       " AND " + sWhere + " IN('срыв','дискв.','н/я') ";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE speedResults SET qf = '' FROM speedResults SR(NOLOCK) WHERE SR.list_id = " + listID.ToString() +
                        " AND CHARINDEX('*'," + sWhere + ") > 0 ";
                    cmd.ExecuteNonQuery();
                }

                if (preQfClimb)
                {
                    cmd.CommandText = @"UPDATE speedResults SET qf = ' q'
                                         WHERE list_id = @listIDDD
                                           AND preQf = 1
                                           AND IsNull(qf,'') = ''";
                    cmd.Parameters.Add("@listIDDD", SqlDbType.Int).Value = listID;
                    cmd.ExecuteNonQuery();
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления БД\r\n" + ex.Message);
            }
#endif
            if (needRefresh)
                RefreshData();
        }

        public static string CreateString(long resTotal)
        {
            bool neg = false;
            if (resTotal < 0)
            {
                resTotal = -resTotal;
                neg = true;
            }
            switch (resTotal)
            {
                case 60000:
                    return "срыв";
                case 120000:
                    return "срыв";
                case 70000:
                    return "дискв.";
                case 140000:
                    return "дискв.";
                case 80000:
                    return "н/я";
                case 160000:
                    return "н/я";
            }
            int i;
            string res;
            i = (int)(resTotal / 6000);
            res = i.ToString("0") + ":";
            resTotal -= (i * 6000);
            i = (int)(resTotal / 100);
            res += i.ToString("00") + ".";
            resTotal -= (i * 100);
            res += resTotal.ToString("00");
            if (neg)
                res = "-" + res;
            return " " + res;
        }

        private void SetEditMode(bool edit)
        {
            SetEditMode(edit, true);
        }
        private void SetEditMode(bool edit, bool reset)
        {
            if (edit)
            {
                if (lblIID.Text.Length < 1)
                    return;
                if (rbRoute2.Checked && (tbRoute1.Text.Length == 0) && !UseBestRoute)
                {
                    MessageBox.Show("Результат первой трассы ещё не введён. Пожалуйста, подождите.");
                    return;
                }
                btnEdit.Text = "Подтвердить";
                if (rbRoute1.Checked)
                {
                    tbRoute1.ReadOnly = false;
                    if (reset)
                        tbRoute1.Text = "";
                    tbRoute2.ReadOnly = tbSum.ReadOnly = true;
                }
                else
                {
                    tbRoute2.ReadOnly = tbSum.ReadOnly = false;
                    if (reset)
                        tbRoute2.Text = tbSum.Text = "";
                    if (rbRoute2.Checked)
                        tbRoute1.ReadOnly = true;
                    else
                    {
                        tbRoute1.ReadOnly = false;
                        if (reset)
                            tbRoute1.Text = "";
                    }
                }
                if (UseBestRoute)
                    tbSum.ReadOnly = true;
                UpdateNowCl();

                btnRollBack.Enabled = btnRestore.Enabled = false;
            }
            else
            {
                btnEdit.Text = "Правка";
                tbRoute1.ReadOnly = tbRoute2.ReadOnly = tbSum.ReadOnly = true;
                if (rbRoute1.Checked)
                {
                    cmd.CommandText = "UPDATE lists SET nowClimbing = NULL WHERE iid = " + listID.ToString();
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                }
                try { SetTransactionButtonsEnabled(); }
                catch { }
            }
            btnNext.Enabled = allowEvent = gbMode.Enabled = !edit;
            btnCancel.Enabled = edit;
            if (!tbSum.ReadOnly && UseBestQfFromTwo)
                tbSum.ReadOnly = true;
        }

        private void dgStart_SelectionChanged(object sender, EventArgs e)
        {
            if (allowEvent)
            {
                try
                {
                    IsEditing = false;
                    //tbRes.Text = "";
                    lblPos.Text = "";
                    lblTeam.Text = dgStart.CurrentRow.Cells[5].Value.ToString();
                    lblStart.Text = dgStart.CurrentRow.Cells[0].Value.ToString();
                    lblIID.Text = dgStart.CurrentRow.Cells[1].Value.ToString();
                    lblName.Text = dgStart.CurrentRow.Cells[2].Value.ToString();
                    lblAge.Text = dgStart.CurrentRow.Cells[3].Value.ToString();
                    lblQf.Text = dgStart.CurrentRow.Cells[4].Value.ToString();
                    //if (rbRoute1.Checked)
                    //    tbRoute1.Text = tbRoute2.Text = tbSum.Text = "";
                    //else
                    FillResFromDB();
                    //if (rbRoute2.Checked)
                    //    tbRoute1.Text = dgStart.CurrentRow.Cells[6].Value.ToString();

                    try { SetTransactionButtonsEnabled(); }
                    catch { }
                }
                catch { lblTeam.Text = lblStart.Text = lblQf.Text = lblName.Text = lblIID.Text = lblAge.Text = ""; }
            }
        }

        private void FillResFromDB()
        {
            Thread thr = new Thread(FillResFromDBFunc);
            thr.Start();
        }

        private void FillResFromDBFunc()
        {
            try
            {
                if (lblIID.Text.Length > 0)
                {
                    SqlDataReader dr = null;
                    SqlConnection cn = null;
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cn = new SqlConnection(this.cn.ConnectionString);
                        if (cn.State != ConnectionState.Open)
                            cn.Open();
                        cmd.Connection = cn;
                        cmd.CommandText = "SELECT route1_text r1, route2_text r2, resText [sum] " +
                                          "  FROM speedResults(NOLOCK) " +
                                          " WHERE list_id=" + listID.ToString() +
                                          "   AND climber_id = " + lblIID.Text;
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            string r1, r2, sum;
                            r1 = dr["r1"].ToString();
                            r2 = dr["r2"].ToString();
                            sum = dr["sum"].ToString();
                            tbRoute1.Invoke(new EventHandler(delegate { tbRoute1.Text = r1; }));
                            tbRoute2.Invoke(new EventHandler(delegate { tbRoute2.Text = r2; }));
                            tbSum.Invoke(new EventHandler(delegate { tbSum.Text = sum; }));
                        }
                    }
                    catch { }
                    finally
                    {
                        if (dr != null)
                            try { dr.Close(); }
                            catch { }
                        if (cn != null)
                            try { cn.Close(); }
                            catch { }
                    }
                }
            }
            catch { }
        }

        private void dgRes_DoubleClick(object sender, EventArgs e)
        {
            if (allowEvent)
            {
                try
                {
                    IsEditing = true;
                    //lblStart.Text = dgRes.CurrentRow.Cells[8].Value.ToString();
                    lblIID.Text = dgRes.CurrentRow.Cells[1].Value.ToString();
                    FillResFromDB();
                    try
                    {
                        int iid = Convert.ToInt32(lblIID.Text);
                        SqlCommand cmd = new SqlCommand("SELECT start FROM speedResults WHERE (" +
                            "list_id = " + listID.ToString() + ") AND (" +
                            "climber_id = " + iid.ToString() + ")", cn);
                        lblStart.Text = cmd.ExecuteScalar().ToString();
                    }
                    catch { lblStart.Text = ""; }

                    lblName.Text = dgRes.CurrentRow.Cells[2].Value.ToString();
                    lblAge.Text = dgRes.CurrentRow.Cells[3].Value.ToString();
                    lblQf.Text = dgRes.CurrentRow.Cells[4].Value.ToString();
                    lblTeam.Text = dgRes.CurrentRow.Cells[5].Value.ToString();
                    //tbRoute1.Text = dgRes.CurrentRow.Cells[6].Value.ToString();
                    lblPos.Text = dgRes.CurrentRow.Cells[0].Value.ToString();

                    //tbRoute2.Text = dgRes.CurrentRow.Cells[7].Value.ToString();
                    //tbSum.Text = dgRes.CurrentRow.Cells[8].Value.ToString();
                    try { SetTransactionButtonsEnabled(); }
                    catch { }
                }
                catch { lblTeam.Text = lblStart.Text = lblQf.Text = lblName.Text = lblIID.Text = lblAge.Text = ""; }
            }
        }

        private void xlExport_Click(object sender, EventArgs e)
        {
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                bool isQf = false;
                foreach (DataRow dataRow in dtRes.Rows)
                    if (dataRow["Кв."].ToString().Trim().Equals("Q", StringComparison.OrdinalIgnoreCase))
                    {
                        isQf = true;
                        break;
                    }

                int i = 0;
                const int frst_row = 6;

                foreach (DataColumn cl in dtRes.Columns)
                    if ((isQf) || (cl.ColumnName != "Кв."))
                    {
                        i++;
                        if (cl.ColumnName == "ФамилияИмя")
                            ws.Cells[frst_row, i] = "Фамилия, Имя";
                        else if (cl.ColumnName == "Сумма" && UseBestRoute)
                            ws.Cells[frst_row, i] = "Лучшее";
                        else
                            ws.Cells[frst_row, i] = cl.ColumnName;
                    }

                char lastColumnChar = (char)('A' + i - 1);
                char lastNotQualifiedC = isQf ? (char)(lastColumnChar - 1) : lastColumnChar;

                i = frst_row;
                int lastFin = -1;
                foreach (DataRow rd in dtRes.Rows)
                {
                    i++;
                    for (int k = 1; k <= dtRes.Columns.Count; k++)
                        ws.Cells[i, k] = rd[k - 1];
                    if ((rd["Г.р."].ToString() == "0"))
                        ws.Cells[i, 4] = "";
                    if ((!rd["Кв."].ToString().Trim().Equals("Q", StringComparison.OrdinalIgnoreCase)) && (lastFin == -1) && (i > 5))
                        lastFin = i - 1;
                    if ((rd["Кв."].ToString().Trim().Equals("Q", StringComparison.OrdinalIgnoreCase)) && (lastFin != -1))
                        lastFin = -1;
                }
                Excel.Range dt;

                if (isQf)
                {
                    if (lastFin < 0)
                        lastFin = i;
                    dt = ws.get_Range("A" + frst_row.ToString(), lastColumnChar + lastFin.ToString());
                    dt.Style = "MyStyle";
                    if (lastFin < i)
                    {
                        dt = ws.get_Range("A" + (lastFin + 1).ToString(), lastNotQualifiedC + i.ToString());
                        dt.Style = "MyStyle";
                    }
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

                char cTmp = lastColumnChar;
                //if (isQf)
                //    cTmp = lastColumnChar;
                //else
                //    cTmp = 'I';

                dt = ws.get_Range("A1", cTmp + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                string strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

                if (judgeID > 0)
                    ws.Cells[frst_row - 1, 1] = "Зам. гл. судьи по виду: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[frst_row - 1, 1] = "Зам. гл. судьи по виду:";


                dt = ws.get_Range(cTmp + "2", cTmp + "2");
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                dt.Style = "StyleRA";

                dt = ws.get_Range("A3", cTmp + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);

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

        private void btnNextRound_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbQf2.Checked)
                    SecondQf();
                else
                    if (rdPairs.Checked)
                        Pairing(listID, cn, false, true);
                    else
                        MessageBox.Show("Выберите следующий раунд");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void Pairing(int listID, SqlConnection cn, bool createFinal, bool fromQual)
        {
            SqlCommand cmd = new SqlCommand("SELECT group_id FROM lists WHERE iid=" + listID.ToString(), cn);
            int group_id, i;
            try { group_id = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            cmd.CommandText = "SELECT COUNT(*) FROM speedResults WHERE (res<15000000)&&(list_id=" +
                listID.ToString() + ")";
            if (createFinal)
                cmd.CommandText = "SELECT climber_id, pos FROM speedResults " +
                    "WHERE (list_id=" + listID.ToString() + ") AND (res<15000000) ORDER BY qf,pos";
            else
            {
                cmd.CommandText = "SELECT climber_id, pos FROM speedResults " +
                    "WHERE (list_id=" + listID.ToString() + ") AND ((qf='Q')";
                if (!fromQual)
                    cmd.CommandText += " OR (qf=' q')";
                cmd.CommandText += " OR (preQf = 1)) ORDER BY preQf, pos";
            }
            List<StartListMember> starters = new List<StartListMember>();
            Random randCl = new Random();
            try
            {
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    i = 1;
                    while (rdr.Read())
                    {
                        StartListMember stm = new StartListMember();
                        
                        stm.iid = Convert.ToInt32(rdr[0]);
                        try
                        {
                            if (rdr[1] != DBNull.Value)
                                stm.prevPos = Convert.ToInt32(rdr[1]);
                            else
                                stm.prevPos = 0;
                        }
                        catch { stm.prevPos = 0; }
                        stm.ranking = i;
                        stm.rndDouble = randCl.NextDouble();
                        starters.Add(stm);
                        i++;
                    }
                }
                finally { rdr.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            i = 2;
            while (i < starters.Count)
                i *= 2;
            if (!createFinal)
                starters.Sort();
            StartListMember[] stList = new StartListMember[i];
            int stInd = 0;
            for (i = 0; i < stList.Length; i++)
            {
                StartListMember curStarter = StartListMember.Empty;
                bool needEmpty = true;
                if (stInd < starters.Count)
                {
                    if (starters[stInd].ranking == (i + 1))
                    {
                        curStarter = starters[stInd];
                        needEmpty = false;
                        stInd++;
                    }
                }
                if (needEmpty)
                {
                    curStarter.prevPos = (i + 1);
                    curStarter.ranking = i + 1;
                }
                stList[i] = curStarter;
            }

            //for (i = 0; i < stList.Length; i++)
            //{
            //    if (i < starters.Count)
            //        stList[i] = (StartListMember)starters[i];
            //    else
            //    {
            //        stList[i].iid = 0;
            //        stList[i].prevPos = 0;
            //        stList[i].ranking = i + 1;
            //        stList[i].rndDouble = 0.0;
            //    }
            //}
            
            if (!createFinal)
                starters = GetPairs(stList);
            else if (starters.Count == 3)
            {
                starters.RemoveAt(0);
            }
            string round;
            if (createFinal || starters.Count < 3)
                round = "Финал";
            else
                round = "1/" + (stList.Length / 2).ToString() + " финала";
            cmd.CommandText = "SELECT iid FROM lists WHERE (group_id=" + group_id.ToString() + ") AND " +
                "(style='Скорость') AND (round='" + round + "')";
            int iid;
            try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { iid = 0; }

            try
            {
                cmd.Transaction = cn.BeginTransaction();
                if (iid == 0)
                {
                    cmd.CommandText = "SELECT (MAX(iid)+1) FROM lists";
                    try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch { iid = 1; }
                }
                else
                {
                    if (MessageBox.Show("Такой протокол существует. Заменить?", "Replace protocol",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                    cmd.CommandText = "DELETE FROM lists WHERE iid=" + iid.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,online,listType) VALUES (@i,@g,@s,@r,@o,@ltp)";

                cmd.Parameters.Add("@i", SqlDbType.Int);
                cmd.Parameters[0].Value = iid;

                cmd.Parameters.Add("@g", SqlDbType.Int);
                cmd.Parameters[1].Value = group_id;

                cmd.Parameters.Add("@s", SqlDbType.VarChar);
                cmd.Parameters[2].Value = "Скорость";

                cmd.Parameters.Add("@r", SqlDbType.VarChar);
                cmd.Parameters[3].Value = round;

                cmd.Parameters.Add("@o", SqlDbType.Bit);
                cmd.Parameters[4].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);

                cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                cmd.Parameters[5].Value = ListTypeEnum.SpeedFinal.ToString();


                cmd.ExecuteNonQuery();

                cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                cmd.Parameters[1].Value = listID;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                cmd.ExecuteNonQuery();

                long nxtIID;
                cmd.CommandText = "SELECT MAX(iid) FROM speedResults";
                try { nxtIID = Convert.ToInt64(cmd.ExecuteScalar()); }
                catch { nxtIID = 0; }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@list_id", SqlDbType.Int);
                cmd.Parameters[0].Value = iid;
                cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                cmd.Parameters.Add("@climber_id", SqlDbType.Int);
                cmd.Parameters.Add("@start", SqlDbType.Int);
                cmd.Parameters.Add("@s2r", SqlDbType.Bit);
                cmd.Parameters.Add("@p", SqlDbType.VarChar);
                cmd.Parameters[5].Value = int.MaxValue;
                cmd.CommandText = "INSERT INTO speedResults(iid, list_id, climber_id, start, start2route, pos) " +
                    "VALUES (@iid, @list_id, @climber_id, @start, @s2r, @p)";
                for (i = 0; i < starters.Count; i++)
                {
                    if (((StartListMember)starters[i]).iid == 0)
                        continue;
                    cmd.Parameters[1].Value = (++nxtIID);
                    cmd.Parameters[2].Value = ((StartListMember)starters[i]).iid;
                    cmd.Parameters[3].Value = (i + 1);
                    cmd.Parameters[4].Value = (i % 2) == 1;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();

                MessageBox.Show("Протокол создан. iid = " + iid.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                try { cmd.Transaction.Rollback(); }
                catch { }
            }
        }
        public static void Pairing(int listID, SqlConnection cn, bool createFinal)
        {
            Pairing(listID, cn, createFinal, false);
        }

        static List<StartListMember> GetPairs(StartListMember[] starters)
        {
            int i = 2;
            while (i < starters.Length)
                i *= 2;
            if (i > starters.Length)
                throw new ArgumentException("Кол-во стартующих должно быть степенью 2");
            if (starters.Length == 2)
                return new List<StartListMember>(starters);
            StartListMember[] stHalf = new StartListMember[starters.Length / 2];
            for (i = 0; i < stHalf.Length; i++)
                stHalf[i] = starters[i];
            List<StartListMember> retVal = GetPairs(stHalf);

            i = 0;
            while (i < retVal.Count)
            {
                int nTmp = ((StartListMember)retVal[i]).ranking;
                nTmp = starters.Length - nTmp;
                retVal.Insert(++i, starters[nTmp]);
                i++;
            }
            return retVal;
        }

        public static void SecondQf(int listID, SqlConnection cn)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                var preQfClimb = SettingsForm.GetPreQfClimb(cn, listID);
                SqlCommand cmd = new SqlCommand("SELECT group_id FROM lists(NOLOCK) WHERE iid=" + listID.ToString(), cn);
                int group_id, i;
                try { group_id = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                cmd.CommandText = "SELECT R.climber_id iid, ISNULL(R.pos,0) pos, ISNULL(R.start,0) start, ISNULL(P.rankingSpeed, 0) rankingSpeed " +
                                    "FROM speedResults R(NOLOCK) " +
                                    "JOIN Participants P(NOLOCK) ON P.iid = R.climber_id " +
                                   "WHERE R.list_id=" + listID.ToString() + " " +
                                     "AND (R.qf='Q' " +
                                     " OR R.qf = ' q' " +
                                     " OR R.preQf = 1) " +
                                     "ORDER BY " + (!preQfClimb ? "R.preQf, " : String.Empty) + "R.start";
                List<StartListMember> starters = new List<StartListMember>();
                List<Starter> data = new List<Starter>();
                try
                {
                    SqlDataReader rdr = cmd.ExecuteReader();
                    Random rnd = new Random();
                    while (rdr.Read())
                    {
                        StartListMember stm = new StartListMember();
                        stm.iid = Convert.ToInt32(rdr[0]);
                        stm.prevPos = Convert.ToInt32(rdr[1]);
                        stm.ranking = 0;
                        stm.rndDouble = 0.0;
                        starters.Add(stm);
                        Starter strt = new Starter();
                        strt.iid = Convert.ToInt32(rdr["iid"]);
                        strt.lateAppl = false;
                        strt.prevPos = Convert.ToInt32(rdr["pos"]);
                        strt.prevStart = Convert.ToInt32(rdr["start"]);
                        strt.random = rnd.NextDouble();
                        try { strt.ranking = Convert.ToInt32(rdr["rankingSpeed"]); }
                        catch { strt.ranking = 0; }
                        data.Add(strt);
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                Sorting sort = new Sorting(data, StartListMode.NotFirstRound);
                sort.ShowDialog();
                if (sort.Cancel)
                    return;
                cmd.CommandText = "SELECT iid FROM lists(NOLOCK) WHERE (group_id=" + group_id.ToString() + ") AND " +
                    "(style='Скорость') AND (round='Квалификация 2')";
                int iid;
                try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch { iid = 0; }
                try
                {
                    cmd.Transaction = cn.BeginTransaction();
                    if (iid == 0)
                    {
                        cmd.CommandText = "SELECT (MAX(iid)+1) FROM lists(NOLOCK)";
                        try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch { iid = 1; }
                    }
                    else
                    {
                        if (MessageBox.Show("Такой протокол существует. Заменить?", "Replace protocol",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                        cmd.CommandText = "DELETE FROM lists WHERE iid=" + iid.ToString();
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,online,listType) VALUES (@i,@g,@s,@r,@o,@ltp)";

                    cmd.Parameters.Add("@i", SqlDbType.Int);
                    cmd.Parameters[0].Value = iid;

                    cmd.Parameters.Add("@g", SqlDbType.Int);
                    cmd.Parameters[1].Value = group_id;

                    cmd.Parameters.Add("@s", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = "Скорость";

                    cmd.Parameters.Add("@r", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = "Квалификация 2";

                    cmd.Parameters.Add("@o", SqlDbType.Bit);
                    cmd.Parameters[4].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);

                    cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                    cmd.Parameters[5].Value = ListTypeEnum.SpeedQualy2.ToString();

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                    cmd.Parameters[1].Value = listID;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                    cmd.ExecuteNonQuery();

                    long nxtIID;
                    cmd.CommandText = "SELECT MAX(iid) FROM speedResults(NOLOCK)";
                    try { nxtIID = Convert.ToInt64(cmd.ExecuteScalar()); }
                    catch { nxtIID = 0; }

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@list_id", SqlDbType.Int);
                    cmd.Parameters[0].Value = iid;
                    cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                    cmd.Parameters.Add("@climber_id", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.Int);
                    cmd.Parameters.Add("@s2r", SqlDbType.Bit);
                    cmd.Parameters[4].Value = false;
                    cmd.Parameters.Add("@p", SqlDbType.VarChar);
                    cmd.Parameters[5].Value = int.MaxValue;
                    cmd.CommandText = "INSERT INTO speedResults(iid, list_id, climber_id, start, start2route, pos) " +
                        "VALUES (@iid, @list_id, @climber_id, @start, @s2r, @p)";
                    //for (i = 0; i < starters.Count; i++)
                    i = 0;
                    foreach (Starter ste in sort.Starters)
                    {
                        if (ste.iid == 0)
                            continue;
                        cmd.Parameters[1].Value = (++nxtIID);
                        cmd.Parameters[2].Value = ste.iid;
                        cmd.Parameters[3].Value = ++i;
                        //cmd.Parameters[4].Value = (i % 2) == 1;

                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    MessageBox.Show("Протокол создан. iid = " + iid.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }

        private void SecondQf()
        {
            SecondQf(this.listID, this.cn);
        }

        private void dgStart_DoubleClick(object sender, EventArgs e)
        {
            if (allowEvent)
            {
                try
                {
                    //lblStart.Text = dgRes.CurrentRow.Cells[8].Value.ToString();
                    lblIID.Text = dgStart.CurrentRow.Cells[0].Value.ToString();
                    try
                    {
                        int iid = Convert.ToInt32(lblIID.Text);
                        SqlCommand cmd = new SqlCommand("SELECT start FROM routeResults WHERE (" +
                            "list_id = " + listID.ToString() + ") AND (" +
                            "climber_id = " + iid.ToString() + ")", cn);
                        lblStart.Text = cmd.ExecuteScalar().ToString();
                    }
                    catch { lblStart.Text = ""; }
                    lblName.Text = dgRes.CurrentRow.Cells[2].Value.ToString();
                    lblAge.Text = dgRes.CurrentRow.Cells[3].Value.ToString();
                    lblQf.Text = dgRes.CurrentRow.Cells[4].Value.ToString();
                    lblTeam.Text = dgRes.CurrentRow.Cells[5].Value.ToString();
                    tbRoute1.Text = dgRes.CurrentRow.Cells[6].Value.ToString();
                    lblPos.Text = dgRes.CurrentRow.Cells[0].Value.ToString();
                    tbRoute2.Text = dgRes.CurrentRow.Cells[7].Value.ToString();
                    tbSum.Text = dgRes.CurrentRow.Cells[8].Value.ToString();
                }
                catch { lblTeam.Text = lblStart.Text = lblQf.Text = lblName.Text = lblIID.Text = lblAge.Text = ""; }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            RefreshData();
            if (dtStart.Rows.Count > 0)
            {
                IsEditing = false;
                SetEditMode(true);
            }
            else
                MessageBox.Show("Вся группа введена");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintOrdinaryList printList = new PrintOrdinaryList(dtRes, competitionTitle, "Скорость", gName,
                round, "Протокол результатов", "");
            printList.Print();
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            PrintOrdinaryList printList = new PrintOrdinaryList(dtRes, competitionTitle, "Скорость", gName,
                round, "Протокол результатов", "");
            printList.PrintPreview();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StaticClass.SetNowClimbingNull(cn, listID);
            base.OnClosing(e);
        }

        private void rbSystem_CheckedChanged(object sender, EventArgs e)
        {
            wrk_done = true;
            if (rbSystem.Checked)
            {
                StaticClass.mSpeed.WaitOne(1000, true);
                try
                {
                    SpeedSystem sr = new SpeedSystem(listID, cn, competitionTitle, trial);
                    sr.MdiParent = this.MdiParent;
                    sr.Show();
                    this.Invoke(new EventHandler(CloseF));
                }
                finally { StaticClass.mSpeed.ReleaseMutex(); }
            }
        }
        void CloseF(object sender, EventArgs e)
        {
            StaticClass.mSpeed.WaitOne(1000, true);
            try { this.Close(); }
            finally { StaticClass.mSpeed.ReleaseMutex(); }
        }
        public enum WorkMode { ROUTE_1, ROUTE_2, BOTH_ROUTES, SYSTEM }

        private void ResultListSpeed_Load(object sender, EventArgs e)
        {
            if (needToClose)
                try { this.Close(); }
                catch { }
        }

        private void btnRollBack_Click(object sender, EventArgs e)
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
                if (MessageBox.Show(this, "Вы уверены, что хотите " + message + " результат текущего участника (" +
                    lblName.Text + ")?", message + " результат?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
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
                        RefreshTable();
                        RefreshData();
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


        public static long AddTransaction(long resIid, Dictionary<string, string> oldValues, Dictionary<string, string> newValues,
            SqlConnection cn, long existingTran = -1, SqlTransaction tran = null, bool checkData=true)
        {
            return StaticClass.AddTransaction("speedResults", "iid", resIid, oldValues, newValues, cn,
                existingTran, tran, checkData);
        }
    }
}
