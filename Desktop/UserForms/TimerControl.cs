// <copyright file="TimerControl.cs">
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
using System.IO;
using System.Media;
using System.Windows.Forms;
using ClimbingCompetition;
using System.Drawing;
using System.Threading;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace ListShow
{
    public partial class TimerControl : UserControl
    {
        public enum TimerState { STOPPED, PENDING, RUNNING, ERROR }
        //private BoulderingTimer bt = null;
        public string ConnectionString { get; set; }
        private TimerState state = TimerState.STOPPED;
        public TimerState State
        {
            get { return state; }
            private set
            {
                if (value != state)
                {
                    state = value;
                    if (TimerStateChanged != null)
                        TimerStateChanged(this, new TimerStateChangedEventArgs(state));
                }
            }
        }

        public TimerControl()
        {
            InitializeComponent();
            this.ConnectionString = "";
            this.State = TimerState.STOPPED;
            SetMenuEnabled();
        }
        ClimbingTimer _bt = null;
        ClimbingTimer bt
        {
            get
            {
                if (_bt == null)
                {
                    _bt = new ClimbingTimer(this.ConnectionString, 1);
                    _bt.timerEvent += new EventHandler<ClimbingTimerEventArgs>(bt_timerEvent);
                    _bt.timerStopped += new EventHandler(bt_timerStopped);
                }
                return _bt;
            }
        }
        public void StartTimer()
        {
            try
            {
                if (this.ConnectionString.Length < 1)
                    return;

                //if (bt == null)
                //{
                //    bt = new ClimbingTimer(this.ConnectionString, 1);
                //    bt.timerEvent += new EventHandler<ClimbingTimerEventArgs>(bt_timerEvent);
                //    bt.timerStopped += new EventHandler(bt_timerStopped);
                //}

                bt.Start();
                this.State = TimerState.RUNNING;

            }
            finally
            {
                SetMenuEnabled();
            }
        }

        void bt_timerStopped(object sender, EventArgs e)
        {
            SetMenuEnabled(false);
        }

        void bt_timerEvent(object sender, ClimbingTimerEventArgs e)
        {
            try
            {
                string sTime = e.Minute.ToString("00") + ":" + e.Second.ToString("00");
                if (lblTime.InvokeRequired)
                    lblTime.Invoke(new EventHandler(delegate { lblTime.Text = sTime; }));
                else
                    lblTime.Text = sTime;
                try
                {
                    if (e.Minute == 1 && e.Second == 3 && oneMinute != null)
                        oneMinute.Play();
                    else if (e.Minute == 0 && e.Second == 12 && getReady != null)
                        getReady.Play();
                    else if (e.Minute == 0 && e.Second == 2 && rotate != null)
                        rotate.Play();
                }
                catch { }
            }
            catch { }
        }

        private bool ctxMenu = true;
        public bool ContextMenuEnabled
        {
            get { return ctxMenu; }
            set
            {
                ctxMenu = value;
                SetFunc<bool> f = SetCtxMenu;
                if (lblTime.InvokeRequired)
                    lblTime.Invoke(f, new object[] { ctxMenu });
                else
                    f(ctxMenu);
            }
        }

        private void SetCtxMenu(bool enable)
        {
            ContextMenuStrip valToSet = enable ? timerContextMenuStrip : null;
            lblTime.ContextMenuStrip = valToSet;
        }

        private void SetMenuEnabled(bool? isNowRunning = null)
        {
            try
            {
                bool isRunning = (isNowRunning != null && isNowRunning.HasValue) ? isNowRunning.Value : (State == TimerState.RUNNING || State == TimerState.PENDING);
                if (timerContextMenuStrip.InvokeRequired)
                    timerContextMenuStrip.Invoke(new SetFunc<bool>(delegate(bool p)
                    {
                        startTimerToolStripMenuItem.Enabled = !(stopTimerToolStripMenuItem.Enabled = p);
                    }), new object[] { isRunning });
                else
                    startTimerToolStripMenuItem.Enabled = !(stopTimerToolStripMenuItem.Enabled = isRunning);
            }
            catch { }
        }

        public IExceptionLogger ParentLogger { get { return this.ParentForm as IExceptionLogger; } }

        void bt_RotationEvent(object sender, RotationEventArgs e)
        {
            try
            {
                if (lblTime.InvokeRequired)
                    lblTime.Invoke(new EventHandler(delegate { lblTime.Text = e.Time; }));
                else
                    lblTime.Text = e.Time;
            }
            catch { }
        }

        public void StopTimer()
        {
            if (bt != null)
                try { bt.Stop(); }
                catch { }
            SetMenuEnabled(new bool?(false));
            State = TimerState.STOPPED;
        }

        public void ResetTimer()
        {
            if(bt != null)
                try { bt.Reset(); }
                catch { }
        }

        public class TimerStateChangedEventArgs : EventArgs
        {
            public TimerState State { get; private set; }
            public TimerStateChangedEventArgs(TimerState state) { this.State = state; }
        }
        public event EventHandler<TimerStateChangedEventArgs> TimerStateChanged;

        SoundPlayer oneMinute = null, getReady = null, rotate = null;

        public void SetTimerSignals()
        {
            if (bt == null)
                return;

            bool res = false;
            ClimbingCompetition.Properties.Settings aSet = ClimbingCompetition.Properties.Settings.Default;
            string sOneM = aSet.OneMinute, sGetReady = aSet.GetReady, sRotate = aSet.Rotate;


            do
            {
                oneMinute = GetPath("Выберите файл \"Осталась одна минута\"", sOneM, out sOneM);
                getReady = GetPath("Выберите файл \"Приготовиться к переходу\"", sGetReady, out sGetReady);
                rotate = GetPath("Выберите файл \"Переход!\"", sRotate, out sRotate);
                string sMsg = "Осталась одна минута: " + (sOneM.Length > 0 ? sOneM : "<Без сигнала>") + "\r\n" +
                    "Приготовиться к переходу: " + (sGetReady.Length > 0 ? sGetReady : "<Без сигнала>") + "\r\n" +
                    "Переход: " + (sRotate.Length > 0 ? sRotate : "<Без сигнала>") + "\r\n" +
                    "Это правильно?";
                DialogResult dgRes;
                if (this.ParentForm == null || this.ParentForm.InvokeRequired)
                    dgRes = MessageBox.Show(sMsg, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                else
                    dgRes = MessageBox.Show(this.ParentForm, sMsg, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dgRes)
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        res = true;
                        break;
                }
            } while (!res);
            try
            {
                aSet.OneMinute = sOneM;
                aSet.GetReady = sGetReady;
                aSet.Rotate = sRotate;
                aSet.Save();
            }
            catch (Exception ex)
            {
                if (ParentLogger != null)
                    ParentLogger.WriteLog("Ошибка установки сигналов:\r\n" + ex.Message, true);
            }
        }

        private delegate T GetFunc<T>();

        private delegate void SetFunc<T>(T value);

        public override Font Font
        {
            get
            {
                if (lblTime.InvokeRequired)
                    return (Font)lblTime.Invoke(new GetFunc<Font>(delegate { return lblTime.Font; }));
                else
                    return lblTime.Font;
            }
            set
            {
                if (lblTime.InvokeRequired)
                    lblTime.Invoke(new SetFunc<Font>(delegate(Font f)
                    {
                        lblTime.Font = f;
                    }), new object[] { value });
                else
                    lblTime.Font = value;
            }
        }

        private SoundPlayer GetPath(string messageToShow, string defaultPath, out string path)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (defaultPath.Length > 0)
                try { ofd.FileName = defaultPath; }
                catch { }
            else
                try { ofd.InitialDirectory = Directory.GetCurrentDirectory(); }
                catch { }
            ofd.Title = messageToShow;
            DialogResult dgRes;
            if (this.ParentForm == null || this.ParentForm.InvokeRequired)
                dgRes = ofd.ShowDialog();
            else
                dgRes = ofd.ShowDialog(this.ParentForm);
            if (dgRes == DialogResult.Abort || dgRes == DialogResult.Cancel)
            {
                path = "";
                return null;
            }
            try
            {
                SoundPlayer res = new SoundPlayer(ofd.FileName);
                path = ofd.FileName;
                return res;
            }
            catch (Exception ex)
            {
                if (ParentLogger != null)
                    ParentLogger.WriteLog("Ошибка загрузки звукового файла:\r\n" + ex.Message, true);
                path = "";
                return null;
            }
        }

        private void startTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void stopTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void настроитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTimerSignals();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread thr = new Thread(SetSelectFont);
            thr.Start();
        }

        private void SetSelectFont()
        {
            try
            {
                fontDialog.Font = this.Font;
                var a = fontDialog.ShowDialog();
                if (a == DialogResult.Cancel || a == DialogResult.No || fontDialog.Font == null)
                    return;
                this.Font = fontDialog.Font;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка установки шрифта:\r\n" + ex.Message); }
        }

        private void autoresetToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            try { bt.AutoReset = autoresetToolStripMenuItem.Checked; }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно установить параметр:\r\n" + ex.Message);
                autoresetToolStripMenuItem.Checked = !autoresetToolStripMenuItem.Checked;
            }
        }

        private void autoSynchroToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            try { bt.AutoSynchro = autoSynchroToolStripMenuItem.Checked; }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно установить параметр:\r\n" + ex.Message);
                autoSynchroToolStripMenuItem.Checked = !autoSynchroToolStripMenuItem.Checked;
            }
        }

        private void сброситьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTimer();
        }
    }
}