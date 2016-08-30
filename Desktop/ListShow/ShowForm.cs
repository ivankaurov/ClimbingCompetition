// <copyright file="ShowForm.cs">
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

﻿#if DEBUG
//#define NOTIMER
//#define NOLINE
#endif

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ClimbingCompetition;
using CTimer = System.Timers.Timer;

namespace ListShow
{
    public partial class ShowForm : Form, IExceptionLogger
    {
        private const double REFRESH_TIME = 10000.0;
        private const int ITER_COUNT = 3;

        private int IterToMoveNext = ITER_COUNT;

        private SqlConnection cn = null;
        private int projNum;
        private int currentControl = 0;
        private int eventCount = 0;

        private List<Thread> RefreshThreadsList = new List<Thread>();
        Mutex mThrList = new Mutex();
        private List<BaseControl> cList = new List<BaseControl>();

        public static string LogFile
        {
            get { return CExceptionLogger.LogFile; }
            set { CExceptionLogger.LogFile = value; }
        }

        CTimer tRefresh;
        private bool closeImmediately = 
#if DEBUG
            true;
#else
            false;
#endif
        public ShowForm()
        {
            InitializeComponent();
            this.Text = 
#if FULL
                "ListShow for ClimbingCompetition " 
#else
                "ListShow for SpeedCompetition "
#endif
                + StaticClass.GetVersion();
            this.toolStripStatusLabel1.Text = this.Text + "        Для смены группы нажмите <Пробел>";
            tRefresh = new CTimer(REFRESH_TIME);
            tRefresh.AutoReset = true;
            tRefresh.Elapsed += new System.Timers.ElapsedEventHandler(tRefresh_Elapsed);

            listShowControl.Dock = DockStyle.Fill;
            cList.Add(listShowControl);

            leadPhotoControl.ClimbingStyle = BasePhotoControl.ClimbingStyleType.Трудность;
            leadPhotoControl.Dock = DockStyle.Fill;
            cList.Add(leadPhotoControl);

            speedPhotoControl.ClimbingStyle = BasePhotoControl.ClimbingStyleType.Скорость;
            speedPhotoControl.Dock = DockStyle.Fill;
            cList.Add(speedPhotoControl);

#if !DEBUG
            обновитьToolStripMenuItem.Enabled = следующийЭкранToolStripMenuItem.Enabled =
                обновитьToolStripMenuItem.Visible = следующийЭкранToolStripMenuItem.Visible =
                сброситьОчередьToolStripMenuItem.Visible = false;
#endif
        }

        void tRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SetNextEvent();
        }

        private void ShowForm_Load(object sender, EventArgs e)
        {
#if !DEBUG
            обновитьToolStripMenuItem.Visible = обновитьToolStripMenuItem.Enabled = false;
#endif
            Connect(true);
#if NOLINE
            try
            {
                this.Controls.Remove(textLine);
                textLine.CloseCon();
                textLine = null;
                GC.Collect();
            }
            catch { }
#endif
            SetErrorMenus();
            SetTimerMenus(TimerControl.TimerState.STOPPED);
            //tRefresh.Start();
        }

        AsyncConnectionOpener acn = null;
        Mutex mSetNextEvent = new Mutex();
        

        private void SetNextEvent()
        {
            Thread thr = new Thread(SetNextEventThreadFunction);
            mThrList.WaitOne();
            try
            {
                RefreshThreadsList.Add(thr);
                thr.Start(thr);
            }
            finally { mThrList.ReleaseMutex(); }
        }

        private void SetNextEventThreadFunction(object obj)
        {
            try
            {
                Thread thr;
                if (obj is Thread)
                    thr = (Thread)obj;
                else
                    thr = null;
                mSetNextEvent.WaitOne();
                try
                {
                    if (!Connected)
                    {
                        foreach (BaseControl bc in cList)
                            if (bc.InvokeRequired)
                                bc.Invoke(new EventHandler(delegate { bc.Visible = bc.AllowEvent = false; }));
                            else
                                bc.Visible = bc.AllowEvent = false;
                        return;
                    }

                    if (cn.State != ConnectionState.Open)
                    {
                        if (acn == null || !acn.IsAlive)
                        {
                            acn = new AsyncConnectionOpener(cn);
                            acn.DoOpen(new AsyncConnectionOpener.ConnectionOpenedDelegate(delegate(SqlConnection cnL, bool res, string erMsg)
                            {
                                if (res)
                                    this.cn = cnL;
                                else
                                    WriteLog(erMsg, false);
                                SetNextEvent();
                            }));
                        }
                        return;
                    }
                    if (eventCount == 0)
                    {
                        SetNextView();
                        LoadRefreshSpeed();
                    }
                    else
                    {
                        RefreshFormData();
                    }
                    eventCount = (eventCount >= IterToMoveNext) ? 0 : (eventCount + 1);
                }
                finally
                {
                    mSetNextEvent.ReleaseMutex();
                    if (thr != null)
                    {
                        mThrList.WaitOne();
                        try { RefreshThreadsList.Remove(thr); }
                        catch { }
                        finally { mThrList.ReleaseMutex(); }
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex) { WriteLog(ex, false); }
        }

        public void WriteLog(Exception ex, bool needToShow)
        {
            WriteLog((ex == null) ? "" : ex.ToString(), needToShow);
        }

        public void WriteLog(string message, bool needToShow)
        {
            try
            {
                if (this.InvokeRequired)
                    this.Invoke(new EventHandler(delegate { WriteLogStatic("Window Title: " + this.Text + "; Message: " + message, this, needToShow); }));
                else
                    WriteLogStatic("Window Title: " + this.Text + "; Message: " + message, this, needToShow);
            }
            catch { }
        }

        public static void WriteLogStatic(string message, Form owner, bool needToShow)
        {
            CExceptionLogger.WriteLogStatic(message, owner, needToShow);
        }

        private void RefreshFormData()
        {
            if (cList.Count < 1)
                return;
            if (currentControl >= cList.Count)
                currentControl = 0;
            if (!cList[currentControl].RefreshCurrent() || !cList[currentControl].Visible)
                SetNextView();
        }

        private void SetNextView()
        {
            if (cList.Count < 1)
                return;
            bool somethingSet = false;
            try
            {
                if (currentControl < cList.Count)
                {
                    if (cList[currentControl].MoveNext())
                    {
                        somethingSet = true;
                        return;
                    }
                }

                currentControl = (currentControl >= cList.Count - 1) ? 0 : (currentControl + 1);
                int cStart = currentControl;
                do
                {
                    if (cList[currentControl].LoadData())
                        if (cList[currentControl].MoveNext())
                        {
                            somethingSet = true;
                            break;
                        }
                    currentControl = (currentControl >= cList.Count - 1) ? 0 : (currentControl + 1);
                } while (currentControl != cStart);
            }
            catch (Exception ex)
            {
                currentControl = (currentControl >= cList.Count - 1) ? 0 : (currentControl + 1);
                throw ex;
            }
            finally
            {
                for (int i = 0; i < cList.Count; i++)
                    if (cList[i].InvokeRequired)
                        cList[i].Invoke(new EventHandler(delegate
                        {
                            if (somethingSet)
                                cList[i].AllowEvent = cList[i].Visible = (i == currentControl);
                            else
                                cList[i].Visible = cList[i].AllowEvent = false;
                        }));
                    else
                    {
                        if (somethingSet)
                            cList[i].AllowEvent = cList[i].Visible = (i == currentControl);
                        else
                            cList[i].Visible = cList[i].AllowEvent = false;
                    }
            }
        }

        private const string NOT_CONNECTED = "Нет соединения с БД";
        private const string ConnectLabel = "Подключиться к БД";
        private const string ChangeLabel = "Сменить БД";
        private bool Connected { get { return connectToolStripMenuItem.Text == ChangeLabel; } }

        private void Connect(bool connect)
        {
            try
            {
                if (connect)
                {
                    try
                    {
                        if (cn != null && cn.State != ConnectionState.Closed)
                            cn.Close();
                    }
                    catch { }
                    cn = null;
                    GC.Collect();
                    AccountForm fr = new AccountForm("", true);
                    fr.ShowDialog();
                    if (fr.ExitPressed)
                    {
                        closeImmediately = true;
                        this.Close();
                    }
                    projNum = fr.ProjNum;
                    if (fr.ConnectionString == "")
                    {
                        Connect(false);
                        return;
                    }
                    else
                    {
                        cn = new SqlConnection();
                        cn.ConnectionString = fr.ConnectionString;
                        try
                        {
                            cn.Open();
                            try { SortingClass.CheckColumn("lists", "projNum", "INT NOT NULL DEFAULT 1", cn); }
                            catch { }
                            connectToolStripMenuItem.Text = ChangeLabel;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка соединения:\r\n" + ex.Message);
                            this.Text = NOT_CONNECTED;
                            return;
                        }
                        foreach (BaseControl bc in cList)
                        {
                            bc.ProjNum = projNum;
                            bc.cn = cn;
                        }
#if !NOLINE
                        textLine.Connection = cn;
                        timerControl.ConnectionString = cn.ConnectionString;
#endif
                        
                        LoadRefreshSpeed();
#if !NOTIMER
                        tRefresh.Start();
#endif
                    }
                }
                else
                {
                    tRefresh.Stop();
                    this.Text = NOT_CONNECTED;
                    try
                    {
                        if (cn != null && cn.State != ConnectionState.Closed)
                            cn.Close();
                    }
                    catch { }
                    connectToolStripMenuItem.Text = ConnectLabel;
                }
            }
            finally
            {
#if DEBUG
                следующийЭкранToolStripMenuItem.Enabled = обновитьToolStripMenuItem.Enabled = Connected;
#endif
                таймерToolStripMenuItem.Enabled = Connected;
                if (Connected)
                    OnKeyPressP(true);
                else
                {
#if !NOLINE
                    textLine.CloseCon();
#endif
                    try
                    {
                        timerControl.Visible = false;
                        timerControl.StopTimer();
                    }
                    catch { }
                    foreach (BaseControl bc in cList)
                        bc.Visible = false;
                }
            }
        }

        private void LoadRefreshSpeed()
        {
            bool tRunning = tRefresh.Enabled;
            if (tRunning)
                tRefresh.Stop();
            try { tRefresh.Interval = (double)SettingsForm.GetRefreshSpeed(cn) * 1000; }
            catch { tRefresh.Interval = REFRESH_TIME; }
            try { IterToMoveNext = SettingsForm.GetMoveNext(cn) - 1; }
            catch { IterToMoveNext = ITER_COUNT; }
            if (tRunning)
                tRefresh.Start();
        }

        public void OnKeyPressP(bool immediateRefresh)
        {
            bool timerRunning = tRefresh.Enabled;
            tRefresh.Stop();
            if (immediateRefresh)
                eventCount = 0;
            SetNextEvent();
            if (timerRunning)
                tRefresh.Start();
        }

        private void ShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!closeImmediately)
                if (MessageBox.Show(this, "Вы уверены, что хотите выйти?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            tRefresh.Stop();
            try
            {
                if (acn != null)
                    acn.AbortOperation();
            }
            catch { }
            ClearQueue();
#if !NOLINE
            textLine.CloseCon();
#endif
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    cn.Close();
            }
            catch { }
        }

        private void ClearQueue()
        {
            mThrList.WaitOne();
            try
            {
                foreach (var thr in RefreshThreadsList)
                    if (thr.IsAlive)
                        thr.Abort();
                RefreshThreadsList.Clear();
            }
            catch { }
            finally { mThrList.ReleaseMutex(); }
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnKeyPressP(false);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connectToolStripMenuItem.Text == ChangeLabel)
                Connect(false);
            Connect(true);
        }

        private void следующийЭкранToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnKeyPressP(true);
        }

        private void файлЖурналаОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLogFile();
        }

        private void SetLogFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            try
            {
                if (LogFile.Length > 0)
                    sfd.FileName = LogFile;
                else
                    sfd.FileName = "";
            }
            catch { }
            sfd.Filter = "Текстовый файл (*.txt)|*.txt";
            DialogResult dgRes = sfd.ShowDialog(this);
            if (dgRes == DialogResult.Cancel || dgRes == DialogResult.Abort)
                return;
            LogFile = sfd.FileName;
            SetErrorMenus();
        }

        private void установитьВыводОшибокНаЭкранToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотитите установить вывод ошибок на экран?", "",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            LogFile = "";
            SetErrorMenus();
        }

        private void SetErrorMenus()
        {
            установитьВыводОшибокНаЭкранToolStripMenuItem.Enabled =
                   очиститьЖурналОшибокToolStripMenuItem.Enabled = (LogFile.Length > 0);
        }

        private void очиститьЖурналОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LogFile.Length < 1)
                return;
            if (MessageBox.Show(this, "Вы уверены, что хотите очистить журнал ошибок?", "",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            try
            {
                StreamWriter swr = new StreamWriter(LogFile, false);
                swr.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка очистки файла журнала ошибок:\r\n" + ex.Message); }
        }

        private void SetTimerMenus(TimerControl.TimerState state)
        {
            try
            {
                bool allowStop = (state == TimerControl.TimerState.RUNNING ||
                    state == TimerControl.TimerState.PENDING);
                остановитьToolStripMenuItem.Enabled = настройкаСигналовToolStripMenuItem.Enabled = allowStop;
                запуститьToolStripMenuItem.Enabled = !allowStop;
                timerControl.Visible = (state == TimerControl.TimerState.RUNNING);
            }
            catch { }
        }

        private void timerControl_TimerStateChanged(object sender, TimerControl.TimerStateChangedEventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate { SetTimerMenus(e.State); }));
            else
                SetTimerMenus(e.State);
        }

        private void запуститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timerControl.ConnectionString.Length < 1 && Connected)
                timerControl.ConnectionString = cn.ConnectionString;
            timerControl.StartTimer();
        }

        private void остановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerControl.StopTimer();
        }

        private void настройкаСигналовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerControl.SetTimerSignals();
        }

        private void ShowForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                OnKeyPressP(true);
        }

        private void сброситьОчередьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearQueue();
        } 
    }
}
