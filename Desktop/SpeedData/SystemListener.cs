using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace ClimbingCompetition.SpeedData
{
    /// <summary>
    /// Базовый класс для создания считывателей данных с разных скоростных систем
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
                postfix = "\r\nПродолжить показывать инфосообщения?";
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
                MessageBox.Show("Не удалось запустить систему считывания данных");
            return working;
        }

        public bool StopListening()
        {
            if (working)
                working = !StopWorking();
            if (working)
                MessageBox.Show("Остановка системы ввода данных не удалась");
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
    /// Тестовый класс, используется для создания случайных результатов в скорости
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
            MessageBox.Show("Порт открыт");
            return true;
        }

        protected override bool StopWorking()
        {
            t.Stop();
            MessageBox.Show("Порт закрыт");
            return true;
        }


    }

#endif

}
