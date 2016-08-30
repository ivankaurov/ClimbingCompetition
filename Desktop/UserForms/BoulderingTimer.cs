// <copyright file="BoulderingTimer.cs">
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
using System.Text;
using System.Threading;
using System.Media;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using UserForms;
using System.Diagnostics;
using ClimbingCompetition.SpeedData;


namespace ClimbingCompetition
{
    /// <summary>
    /// Таймер для боулдеринга. Возможна работа не более одного таймера
    /// Есть задел под синхронизацию с ёбургской системой, пока изучаю протокол
    /// </summary>
    public class oBoulderingTimer : IDisposable
    {
        System.Timers.Timer timer, timer1;
        Mutex mTimer;
        Mutex mReqUpd = new Mutex();
        private double SEC_INT = 1000.0;
        private const int UPD_INT = 20;
        private EKBListener sl = null;
        public EKBListener SL
        {
            get { return sl; }
            set
            {
                if (sl != null)
                {
                    //sl.SystemListenerEvent -= this.sl_SystemListenerEvent;
                    sl.UnsubscribeEvent(sl_SystemListenerEvent);
                    //sl.StopListening();
                }
                if (value == null)
                {
                    sl = null;
                    //timer.Start();
                }
                else
                {
                    //timer.Stop();
                    sl = value;
                    //sl.StartListening();
                    //sl.SystemListenerEvent += new SystemListener.SystemListenerEventHandler(sl_SystemListenerEvent);
                    sl.SubscribeEvent(sl_SystemListenerEvent);
                }
            }
        }
        private oBoulderingTimer(EKBListener sl)
        {
            timer = new System.Timers.Timer();
            timer.Interval = SEC_INT;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            mTimer = new Mutex();

            timer1 = new System.Timers.Timer();
            timer1.AutoReset = false;
            timer1.Interval = UPD_INT * SEC_INT;
            timer1.Stop();
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(t2_Tick);
            if (sl != null)
            {
                this.sl = sl;
                //this.sl.SystemListenerEvent += new SystemListener.SystemListenerEventHandler(sl_SystemListenerEvent);
                //this.sl.StartListening();
                sl.SubscribeEvent(sl_SystemListenerEvent);
            }
        }

        void sl_SystemListenerEvent(object sender, SystemListenerEventArgs e)
        {
            if (e.EvT != SystemListenerEventArgs.EventType.TIMER)
                return;
            mTimer.WaitOne();
            try
            {
                secElapsed = e.Sec;
                minElapsed = e.Min;
                TimerUpdate(true);
            }
            catch { }
            finally { mTimer.ReleaseMutex(); }
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            if (!timer.Enabled)
                timer.Start();
            Thread thr = new Thread(TimerElapsedT);
            thr.Start();
        }
        private void TimerElapsedT()
        {
            mTimer.WaitOne();
            TimerUpdate(false);
            mTimer.ReleaseMutex();
        }

        public oBoulderingTimer(int time, SqlConnection baseCon, bool mainTimer, EKBListener sl) :
            this(time, baseCon.ConnectionString, mainTimer, sl) { }

        public oBoulderingTimer(int time, string connectionString, bool mainTimer, EKBListener sl)
            : this(sl)
        {
            this.time = time;
            minElapsed = this.time;
            secElapsed = 0;
            this.cn = new SqlConnection(connectionString);
            this.mainTimer = mainTimer;
            //if(sl == null)
            timer.Start();
            watch.Start();
        }

        public oBoulderingTimer(int time,
            SoundPlayer oneMinute, SoundPlayer getReady, SoundPlayer rotate,
            SqlConnection baseCon, bool mainTimer, EKBListener sl)
            : this(sl)
        {
            this.time = time;
            minElapsed = this.time;
            secElapsed = 0;

            this.oneMinute = oneMinute;
            this.getReady = getReady;
            this.rotate = rotate;
            this.cn = new SqlConnection(baseCon.ConnectionString);
            this.mainTimer = mainTimer;
            //if(sl == null)
            timer.Start();
            watch.Start();
        }

        SoundPlayer oneMinute = null, getReady = null, rotate = null;


        public SoundPlayer[] signals
        {
            get
            {
                SoundPlayer[] res = new SoundPlayer[3];
                res[0] = oneMinute;
                res[1] = getReady;
                res[2] = rotate;
                return res;
            }
            set
            {
                if (value.Length < 3)
                    throw new ArgumentException("Incorrect Length");
                oneMinute = value[0];
                getReady = value[1];
                rotate = value[2];
            }
        }

        int time, minElapsed = 0, secElapsed = 0;
        SqlConnection cn;

        ~oBoulderingTimer()
        {
            if (cn != null)
                try
                {
                    if (cn.State != ConnectionState.Closed)
                        try { cn.Close(); }
                        catch { }
                }
                catch { }
            if (sl != null)
                sl.UnsubscribeEvent(sl_SystemListenerEvent);
        }
        public bool Dispose()
        {
            return Dispose(false);
        }
        public bool Dispose(bool unconditional)
        {
            bool b;
            if (!unconditional)
                b = (
                    MessageBox.Show("Остановить таймер?", "Остановка таймера",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes);
            else
                b = true;
            if (b)
            {
                timer.Stop();
                if (sl != null)
                    sl.UnsubscribeEvent(sl_SystemListenerEvent);
            }
            return b;
        }

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        public void Reset()
        {
            minElapsed = time;
            secElapsed = 0;
            try
            {
                if (watch.IsRunning)
                    watch.Stop();
                watch.Reset();
            }
            catch { }
            if (RotationEvent != null)
                RotationEvent(this, new RotationEventArgs(RotationEventArgs.EventType.RESET, 0, 0));
        }

        public void Start()
        {
            //if (sl == null)
            //{
            if (!timer.Enabled)
                timer.Start();
            try
            {
                if (minElapsed == time && secElapsed == 0)
                {
                    if (watch.IsRunning)
                        watch.Stop();
                    watch.Reset();
                    watch.Start();
                }
            }
            catch { }
            //}
            if (sl != null)
                sl.SubscribeEvent(sl_SystemListenerEvent);
            if (RotationEvent != null)
                RotationEvent(this, new RotationEventArgs(RotationEventArgs.EventType.START, minElapsed, secElapsed));
        }
        public bool Stop(bool askForReset)
        {
            //if (sl == null)
            timer.Stop();
            try
            {
                watch.Stop();
                watch.Reset();
            }
            catch { }
            if (sl != null)
                sl.UnsubscribeEvent(sl_SystemListenerEvent);
            if (RotationEvent != null)
                RotationEvent(this, new RotationEventArgs(RotationEventArgs.EventType.STOP, minElapsed, secElapsed));
            if (askForReset)
            {
                try
                {
                    if (MessageBox.Show("Сбросить таймер?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Reset();
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }
        public override string ToString()
        {
            return minElapsed.ToString("00") + ":" + secElapsed.ToString("00");
        }
        public bool Stop()
        {
            return Stop(RotationEvent == null);
        }

        /// <summary>
        /// Retrieves or sets the value of Timer.Enabled
        /// </summary>
        public bool TimerEnabled
        {
            get { return timer.Enabled; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        public delegate void RotationEventHandle(object sender, RotationEventArgs e);

        public event RotationEventHandle RotationEvent;

        public event EventHandler<BoolEventArgs> SynchroCompletedEvent;

        private Stopwatch watch = new Stopwatch();

        private void TimerUpdate(bool fromSL)
        {
            if (!fromSL)
            {
                if (secElapsed == 0)
                {
                    secElapsed = 59;
                    minElapsed--;
                }
                else
                    secElapsed--;
            }
            if (minElapsed == 0 && (secElapsed == 0 && sl == null || secElapsed == 1 && sl != null))
            {

                if (watch.IsRunning)
                {
                    watch.Stop();
                    TimeSpan tsEl = watch.Elapsed;
                    if (tsEl.Minutes != time || tsEl.Seconds != 0 || tsEl.Milliseconds > 10 || tsEl.Milliseconds < 990)
                    {
                        timer.Stop();
                        //Калибровка таймера
                        double mSecTotal = tsEl.Milliseconds + tsEl.Seconds * 1000 + tsEl.Minutes * 60 * 1000;
                        double mSecNeeded = time * 60 * 1000;
                        var dTmp = Math.Abs(1.0 - mSecNeeded / mSecTotal);
                        if (dTmp < 0.1)
                        {
                            double newVal = (double)timer.Interval * (mSecNeeded / mSecTotal);
                            timer.Interval = newVal;
                        }
                        if (!timer.Enabled)
                            timer.Start();
                    }
                    watch.Reset();
                    watch.Start();
                }
                else
                    watch.Start();
   
                if (!fromSL)
                {
                    minElapsed = time;
                    secElapsed = 0;
                }
                if (RotationEvent != null)
                    RotationEvent(this, new RotationEventArgs(RotationEventArgs.EventType.ROTATE, minElapsed, secElapsed));
                try
                {
                    if (rotate != null)
                        rotate.Play();
                }
                catch { }
                return;
            }
            else
                if (RotationEvent != null)
                    RotationEvent(this, new RotationEventArgs(RotationEventArgs.EventType.ONE_SEC, minElapsed, secElapsed));
            if (minElapsed == 1 && secElapsed == 2)
            {
                try
                {
                    if (oneMinute != null)
                        oneMinute.Play();
                }
                catch { }
            }
            else
                if (minElapsed == 0 && secElapsed == 12)
                {
                    try
                    {
                        if (getReady != null)
                            getReady.Play();
                    }
                    catch { }
                }


            if ((secElapsed % 20 == 3) && mainTimer)
            {
                Thread thread = new Thread(new ThreadStart(SynchroUpdate));
                thread.Start();
            }
            if ((secElapsed % 20 == 6) && !mainTimer && !requesting)
            {
                Thread thread = new Thread(new ThreadStart(SynchroRequest));
                thread.Start();
            }
        }

        bool mainTimer;

        bool requesting = false;

        private void SynchroUpdate()
        {
            mReqUpd.WaitOne();
            try
            {
                if (cn.State != ConnectionState.Open)
                    try { cn.Open(); }
                    catch { return; }
                SqlCommand cmd = new SqlCommand("SELECT SynchroRequest FROM CompetitionData", cn);
                if (Convert.ToBoolean(cmd.ExecuteScalar()))
                {
                    cmd.CommandText = "UPDATE CompetitionData SET " +
                        "SynchroRequest=0, " +
                        "ClimbingTime=" + time.ToString() + ", " +
                        "MinElapsed=" + minElapsed.ToString() + ", " +
                        "SecElapsed=" + secElapsed.ToString();
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                }
                try { cn.Close(); }
                catch { }
            }
            finally { mReqUpd.ReleaseMutex(); }
        }

        Thread listener;

        public void SynchroRequest()
        {
            mReqUpd.WaitOne();
            try
            {
                if (cn.State != ConnectionState.Open)
                    try { cn.Open(); }
                    catch { return; }
                SqlCommand cmd = new SqlCommand("UPDATE CompetitionData SET SynchroRequest=1", cn);
                try
                {
                    cmd.ExecuteNonQuery();
                    listener = new Thread(new ThreadStart(RequestListener));
                    requesting = true;
                    listener.Start();
                }
                catch { }
            }
            catch { }
            finally { mReqUpd.ReleaseMutex(); }
        }

        void RequestListener()
        {
            bool result = false;
            Exception exM = null;
            try
            {
                mReqUpd.WaitOne();
                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                }
                finally { mReqUpd.ReleaseMutex(); }
                bool b;
                timer1.Start();
                SqlCommand cmd = new SqlCommand("SELECT SynchroRequest FROM CompetitionData", cn);
                do
                {
                    Thread.Sleep(50);
                    mReqUpd.WaitOne();
                    try { b = Convert.ToBoolean(cmd.ExecuteScalar()); }
                    finally { mReqUpd.ReleaseMutex(); }
                } while (b);
                timer1.Stop();
                cmd.CommandText = "SELECT ClimbingTime,MinElapsed,SecElapsed FROM CompetitionData";
                mReqUpd.WaitOne();
                try
                {
                    SqlDataReader rd = cmd.ExecuteReader();
                    try
                    {
                        if (rd.Read())
                        {
                            secElapsed = (int)rd[2];
                            minElapsed = (int)rd[1];
                            time = (int)rd[0];
                            this.Start();
                            result = true;
                        }
                    }
                    finally { rd.Close(); }
                }
                finally
                {
                    mReqUpd.ReleaseMutex();
                    requesting = false;
                }
            }
            catch (ThreadAbortException)
            { exM = new TimeoutException("Превышен лимит времени ожидания синхронизации таймера"); }
            catch (Exception ex) { exM = ex; }
            finally
            {
                try
                {
                    if (cn != null && cn.State != ConnectionState.Closed)
                        cn.Close();
                }
                catch { }
                if (SynchroCompletedEvent != null)
                    SynchroCompletedEvent(this, new BoolEventArgs(result, exM));
            }
            //MessageBox.Show("Синхронизация завершена");
        }

        void t2_Tick(object sender, EventArgs e)
        {
            try { listener.Abort(); }
            catch { }
            finally { timer1.Stop(); }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try { this.Dispose(true); }
            catch { }
        }

        #endregion
    }

    public class BoolEventArgs : EventArgs
    {
        public Exception Ex { get; private set; }
        public bool Result { get; private set; }
        public BoolEventArgs(bool res, Exception ex)
        {
            this.Result = res;
            this.Ex = ex;
        }
    }

    public class RotationEventArgs : EventArgs
    {
        public enum EventType { ROTATE, START, STOP, RESET, ONE_SEC }
        EventType evT;
        public EventType GetEvType { get { return evT; } }
        int min, sec;
        public RotationEventArgs(EventType eventType, int min, int sec)
            : base()
        {
            evT = eventType;
            this.min = min;
            this.sec = sec;
        }
        public string Time { get { return min.ToString("00") + ":" + sec.ToString("00"); } }
    }
}
