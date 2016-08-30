// <copyright file="TextLine.cs">
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
using System.Data;
using System.Threading;
using ListShow.DsListShowTableAdapters;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ListShow
{
    public partial class TextLine : ListShow.OwnConnectionControl
    {
        public TextLine()
        {
            InitializeComponent();
            t = new System.Timers.Timer(timerInterval);
            t.AutoReset = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            tRestart = new System.Timers.Timer(10000.0);
            tRestart.Enabled = false;
            tRestart.AutoReset = false;
            tRestart.Elapsed += new System.Timers.ElapsedEventHandler(tRestart_Elapsed);
        }

        void tRestart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string sOpen;
            if (cn == null)
                sOpen = curConStr;
            else
                sOpen = cn.ConnectionString;
            OpenConnection(sOpen);
        }

        private System.Timers.Timer t, tRestart;

        private Mutex timerIntervalMutex = new Mutex();

        private int timerInterval = 500;

        Mutex mThrList = new Mutex();
        List<Thread> thrList = new List<Thread>();

        private void LoadInterval()
        {
            try { TextSpeed = ClimbingCompetition.SettingsForm.GetTextLineSpeed(cn); }
            catch (ThreadAbortException ex) { throw ex; }
            catch { }
        }

        public static double LoadTimerInterval(SqlConnection cn)
        {
            int n = ClimbingCompetition.SettingsForm.GetTextLineSpeed(cn);
            if (n < 1)
                n = 1;
            else if (n > 20)
                n = 20;
            return 1050.0 - ((double)n * 50.0);
        }

        public int TextSpeed
        {
            get { return (1050 - timerInterval) / 50; }
            set
            {
                if (value < 1 || value > 20)
                    throw new ArgumentOutOfRangeException("Неверная скорость бегущей строки");
                timerInterval = 1050 - (value * 50);
                timerIntervalMutex.WaitOne();
                try
                {
                    bool isR = (t != null && t.Enabled);
                    if (isR)
                        t.Stop();
                    t.Interval = timerInterval;
                    if (isR)
                        t.Start();
                }
                finally { timerIntervalMutex.ReleaseMutex(); }
            }
        }

        protected override void DoAfterOpenSuccess()
        {
            LoadInterval();
            t.Start();
        }

        protected override void DoAfterOpenFailure(string message)
        {
            if (this.ParentShowForm != null)
                this.ParentShowForm.WriteLog(message, false);
            else
                ShowForm.WriteLogStatic(message, null, false);
            tRestart.Start();
        }

        private string currentString = "";

        protected override void  DoBeforeClose()
        {
            mThrList.WaitOne();
            try
            {
                foreach (Thread thr in thrList)
                    if (thr.IsAlive)
                        thr.Abort();
                thrList.Clear();
            }
            finally { mThrList.ReleaseMutex(); }
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Thread thr = new Thread(t_elapsedThrF);
            mThrList.WaitOne();
            try
            {
                thrList.Add(thr);
                thr.Start(thr);
            }
            finally { mThrList.ReleaseMutex(); }
        }
        Mutex mThrF = new Mutex();
        private void t_elapsedThrF(object obj)
        {
            Thread thr;
            if (obj is Thread)
                thr = (Thread)obj;
            else
                thr = null;
            mThrF.WaitOne();
            try
            {
                bool needToReload = false;
                timerIntervalMutex.WaitOne();
                try
                {
                    if (currentString.Length < 1)
                    {
                        if (!ReloadString(out currentString))
                            return;
                        needToReload = true;
                    }
                    else
                        currentString = currentString.Substring(1);
                    if (textBox1.InvokeRequired)
                        textBox1.Invoke(new EventHandler(delegate { textBox1.Text = currentString; }));
                    else
                        textBox1.Text = currentString;
                }
                finally { timerIntervalMutex.ReleaseMutex(); }
                if (needToReload)
                    LoadInterval();
                try { t.Start(); }
                catch { }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (this.ParentShowForm != null)
                    this.ParentShowForm.WriteLog(ex, false);
                else
                    ShowForm.WriteLogStatic(ex.ToString(), null, false);
            }
            finally
            {
                mThrF.ReleaseMutex();
                if (thr != null)
                {
                    mThrList.WaitOne();
                    try { thrList.Remove(thr); }
                    catch { }
                    finally { mThrList.ReleaseMutex(); }
                }
            }
        }

        private bool ReloadString(out string strNowCl)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    OpenConnection(cn.ConnectionString);
                    strNowCl = "";
                    return false;
                }

                try
                {
                    strNowCl = FillFullNowClimbing(cn);
                }
                catch (ThreadAbortException exTr) { throw exTr; }
                catch
                {
                    OpenConnection(cn.ConnectionString);
                    strNowCl = "";
                    return false;
                }
                return true;
            }
            catch (ThreadAbortException exT) { throw exT; }
            catch (Exception ex)
            {
                if (this.ParentShowForm != null)
                    this.ParentShowForm.WriteLog(ex, false);
                else
                    ShowForm.WriteLogStatic(ex.ToString(), null, false);
                strNowCl = "";
                return false;
            }
        }

        public static string FillFullNowClimbing(SqlConnection cn)
        {
            string strNowCl;
#if FULL
            strNowCl = FillLead(cn);
            strNowCl = AppendToNowClimbing(strNowCl, FillSpeed(cn));
            strNowCl = AppendToNowClimbing(strNowCl, FillBoulder(cn));
#else
            strNowCl = FillSpeed(cn);
#endif
            return strNowCl;
        }

        private static string AppendToNowClimbing(string strNowCl, string sTmp)
        {
            if (sTmp.Length > 0)
            {
                if (strNowCl.Length > 0)
                    strNowCl += "; ";
                strNowCl += sTmp;
            }
            return strNowCl;
        }
#if FULL
        
        public static string FillLead(SqlConnection cn)
        {
            string res = "";
            NowClimbingListsTableAdapter ta = new NowClimbingListsTableAdapter();
            LeadNowClimbingDataTableAdapter lta = new LeadNowClimbingDataTableAdapter();
            lta.Connection = ta.Connection = cn;
            DsListShow.NowClimbingListsDataTable table = ta.GetDataByStyle("Трудность");

            foreach (DsListShow.NowClimbingListsRow row in table.Rows)
            {
                if (row.IsnowClimbingNull() && row.IsnowClimbingTmpNull())
                    continue;
                if (res.Length > 0)
                    res += "; ";
                res += row.grp;
                string rnd = row.round.ToLower();
                if (rnd.IndexOf("трасса 1") > -1)
                    rnd = "Трасса 1";
                else if (rnd.IndexOf("трасса 2") > -1)
                    rnd = "Трасса 2";
                else
                    rnd = "";
                if (rnd.Length > 0)
                    res += " " + rnd;
                res += ": ";
                bool stIns = false;
                if (!row.IsnowClimbingNull())
                    foreach (DsListShow.LeadNowClimbingDataRow row2 in lta.GetDataByIdClm(row.iid, row.nowClimbing))
                    {
                        res += "На трассе " + row2.surname.ToUpper() + ((row2.IsnameNull()) ? "" : ((row2.name.Length > 0) ? " " + row2.name : "")) +
                            " (Ст.№ " + row2.start.ToString() + ")";
                        stIns = true;
                        break;
                    }
                if (!row.IsnowClimbingTmpNull())
                    foreach (DsListShow.LeadNowClimbingDataRow row2 in lta.GetDataByIdClm(row.iid, row.nowClimbingTmp))
                    {
                        if (row2.IsresNull())
                            break;
                        if (stIns)
                            res += ", ";
                        string pos;
                        if (row2.IsposNull() || row2.pos == "в/к" || row2.res == "н/я" || row2.pos == "н/я")
                            pos = "";
                        else
                            pos = row2.pos;
                        res += row2.surname.ToUpper() + ((row2.IsnameNull()) ? "" : ((row2.name.Length > 0) ? " " + row2.name : "")) + ": " +
                            "рез-т " + row2.res + ((pos.Length > 0) ? ", " + pos + " место" : "");
                        break;
                    }
            }

            return res;
        }
#endif
        public static string FillSpeed(SqlConnection cn)
        {
            string res = "";
            NowClimbingListsTableAdapter ta = new NowClimbingListsTableAdapter();
            SpeedNowClimbingDataTableAdapter sta = new SpeedNowClimbingDataTableAdapter();
            sta.Connection = ta.Connection = cn;
            DsListShow.NowClimbingListsDataTable table = ta.GetDataByStyle("Скорость");

            foreach (DsListShow.NowClimbingListsRow row in table.Rows)
            {
                if (row.IsnowClimbingNull() && row.IsnowClimbingTmpNull() && row.IsnowClimbing3Null())
                    continue;
                if (res.Length > 0)
                    res += "; ";
                res += row.grp + ": ";
                bool stIns = false;
                if (!row.IsnowClimbingNull())
                    foreach (DsListShow.SpeedNowClimbingDataRow row2 in sta.GetDataByIdClm(row.iid, row.nowClimbing))
                    {
                        res += "Трасса 1: " + GetClimberName(row2) + " (Ст.№ " + row2.start.ToString() + ")";
                        stIns = true;
                        break;
                    }
                if (!row.IsnowClimbingTmpNull())
                    foreach (DsListShow.SpeedNowClimbingDataRow row2 in sta.GetDataByIdClm(row.iid, row.nowClimbingTmp))
                    {
                        if (stIns)
                            res += ", ";
                        else
                            stIns = true;
                        res += "Трасса 2: " + GetClimberName(row2) + " (Ст.№ " + row2.start.ToString() + ")";
                        break;
                    }
                if (!row.IsnowClimbing3Null())
                    foreach (DsListShow.SpeedNowClimbingDataRow row2 in sta.GetDataByIdClm(row.iid, row.nowClimbing3))
                    {
                        if (row2.IsresNull())
                            break;
                        if (stIns)
                            res += ", ";
                        string pos;
                        if (row2.IsposNull() || row2.pos == "в/к" || row2.res == "н/я" || row2.pos == "н/я")
                            pos = "";
                        else
                            pos = row2.pos;
                        res += GetClimberName(row2) + ": " +
                            "рез-т " + row2.res + ((pos.Length > 0) ? ", " + pos + " место" : "");
                        break;
                    }
            }

            return res;
        }

        public static string GetClimberName(DsListShow.SpeedNowClimbingDataRow row)
        {
            return row.surname.ToUpper() + (row.IsnameNull() ? "" : ((row.name.Length > 0) ? " " + row.name : ""));
        }
#if FULL
        public static string FillBoulder(SqlConnection cn)
        {
            string res = "";
            NowClimbingListsTableAdapter ta = new NowClimbingListsTableAdapter();
            ta.Connection = cn;
            DsListShow.NowClimbingListsDataTable table = ta.GetDataByStyle("Боулдеринг");

            foreach (DsListShow.NowClimbingListsRow row in table.Rows)
            {
                string nowClCur = ClimbingCompetition.ResultListBoulder.getNowClStr(row.iid, cn);
                if (nowClCur.Length > 0)
                {
                    if (res.Length > 0)
                        res += "; ";
                    res += row.grp + ": " + nowClCur;
                }
            }
            return res;
        }
#endif
        private class ClimbingElem
        {
            private const int EMPTY = -1;
            public int Iid { get; private set; }
            public int NowClimbing { get; set; }
            public int NowClimbingTmp { get; set; }
            public int NowClimbing3 { get; set; }
            public string Round { get; private set; }
            public string Group { get; private set; }

            public bool Empty { get { return (NowClimbing == EMPTY && NowClimbing3 == EMPTY && NowClimbingTmp == EMPTY); } }

            public ClimbingElem(int iid, string round, string group)
            {
                this.Iid = iid;
                this.Round = round;
                this.Group = group;
                this.NowClimbing = this.NowClimbingTmp = this.NowClimbing3 = EMPTY;
            }
        }

        protected override void StopTimer()
        {
            t.Stop();
        }
    }
}
