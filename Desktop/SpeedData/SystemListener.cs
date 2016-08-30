// <copyright file="SystemListener.cs">
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
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace ClimbingCompetition.SpeedData
{
    /// <summary>
    /// ������� ����� ��� �������� ������������ ������ � ������ ���������� ������
    /// </summary>
    public abstract class SystemListener
    {
        private readonly Action<bool> persistShowErrors;

        private readonly Form _owner;

        private bool _working = false;

        private bool showErrors = false;
        private bool showErrorMessages
        {
            get { return showErrors; }
            set
            {
                if (value == showErrors)
                    return;
                showErrors = value;
                if (this.persistShowErrors != null)
                    this.persistShowErrors(showErrors);
            }
        }
        protected SystemListener(bool showErrors, Action<bool> persistShowErrors, Form owner)
        {
            this.showErrors = showErrors;
            this.persistShowErrors = persistShowErrors;
            this._owner = owner;
        }
        public delegate void SystemListenerEventHandler(object sender, SystemListenerEventArgs e);

        private event SystemListenerEventHandler SystemListenerEvent;

        public void SubscribeEvent(SystemListenerEventHandler func)
        {
            bool needToStart = (SystemListenerEvent == null);
            SystemListenerEvent += new SystemListenerEventHandler(func);
            if (needToStart)
                StartListening();
        }

        protected void ShowMessage(string message) { ShowMessage(message, false); }

        protected void ShowMessage(string message, bool always)
        {
            if (!always && !showErrorMessages)
                return;
            string msg = DateTime.Now.ToLongTimeString() + ": " + message;
            string postfix;
            if (showErrorMessages)
                postfix = "\r\n���������� ���������� �������������?";
            else
                postfix = "";
            DialogResult dgRes;
            if (owner == null)
                dgRes = MessageBox.Show(msg + postfix, "", (showErrorMessages ? MessageBoxButtons.YesNo : MessageBoxButtons.OK), MessageBoxIcon.Information);
            else if (owner.InvokeRequired)
            {
                owner.Invoke(new EventHandler(delegate
                {
                    DialogResult ds = MessageBox.Show(owner, msg + postfix, "", (showErrorMessages ? MessageBoxButtons.YesNo : MessageBoxButtons.OK), MessageBoxIcon.Information);
                    if (showErrorMessages)
                        showErrorMessages = (ds == DialogResult.Yes);
                }));
                return;
            }
            else
                dgRes = MessageBox.Show(owner, msg + postfix, "", (showErrorMessages ? MessageBoxButtons.YesNo : MessageBoxButtons.OK), MessageBoxIcon.Information);
            if (showErrorMessages)
                showErrorMessages = (dgRes == DialogResult.Yes);
        }

        public void UnsubscribeEvent(SystemListenerEventHandler func)
        {
            SystemListenerEvent -= func;
        }
        protected void CommitEvent(object sender, SystemListenerEventArgs e)
        {
            if (SystemListenerEvent != null)
                SystemListenerEvent(sender, e);
        }

        public bool working
        {
            get { return this._working; }
            protected set { this._working = value; }
        }

        public Form owner
        {
            get
            {
                return this._owner;
            }
        }

        public bool StartListening()
        {
            if (!working)
                working = StartWorking();
            if (!working)
                MessageBox.Show("�� ������� ��������� ������� ���������� ������");
            return working;
        }

        public bool StopListening()
        {
            if (working)
                working = !StopWorking();
            if (working)
                MessageBox.Show("��������� ������� ����� ������ �� �������");
            return !working;
        }

        ~SystemListener()
        {
            try { StopWorking(); }
            catch { }
        }

        protected abstract bool StartWorking();

        protected abstract bool StopWorking();
    }

    public class SystemListenerEventArgs : EventArgs
    {
        private int routeNumber;
        private int min, sec;
        private EventType evT;
        private string res;
        public enum EventType { MESSAGE, RESULT, FALL, FALSTART, TIMER }
        public int RouteNumber { get { return routeNumber; } set { routeNumber = value; } }
        public EventType EvT { get { return evT; } }
        public string Res { get { return res; } }
        public int Min { get { return min; } private set { min = value; } }
        public int Sec { get { return sec; } private set { sec = value; } }

        public SystemListenerEventArgs(EventType evT, int routeNumber, string res)
        {
            this.evT = evT;
            this.routeNumber = routeNumber;
            this.res = res;
        }
        public SystemListenerEventArgs(int min, int sec)
        {
            this.evT = EventType.TIMER;
            this.min = min;
            this.sec = sec;
        }
    }
#if DEBUG
    /// <summary>
    /// �������� �����, ������������ ��� �������� ��������� ����������� � ��������
    /// </summary>
    public class RandomSystemListener : SystemListener
    {
        //public override event SystemListenerEventHandler SystemListenerEvent;

        System.Timers.Timer t;
        Random r;
        public RandomSystemListener(string s, bool showErrors, Action<bool> persistShowErrors)
            : base(showErrors, persistShowErrors, null)
        {
            t = new System.Timers.Timer(5000);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            r = new Random();
        }
        protected void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!working)
                return;
            for (int i = 0; i < 2; i++)
            {
                string s;
                SystemListenerEventArgs.EventType evT = SystemListenerEventArgs.EventType.RESULT;
                int k = r.Next(10);
                if (k < 3)
                {
                    evT = SystemListenerEventArgs.EventType.FALL;
                    s = "FALL";
                }
                else if (k < 4)
                {
                    evT = SystemListenerEventArgs.EventType.FALSTART;
                    s = "FALSTART";
                }
                else
                    s = "0:" + (r.Next(49) + 10).ToString("00") + "." + r.Next(99).ToString("00");
                SystemListenerEventArgs args = new SystemListenerEventArgs(
                    evT, (i + 1), s);
                CommitEvent(this, args);
            }
            if (!t.Enabled)
                t.Start();
        }

        protected override bool StartWorking()
        {
            t.Start();
            MessageBox.Show("���� ������");
            return true;
        }

        protected override bool StopWorking()
        {
            t.Stop();
            MessageBox.Show("���� ������");
            return true;
        }


    }

#endif

}
