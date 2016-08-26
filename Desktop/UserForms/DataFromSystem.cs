using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ClimbingCompetition;
using ClimbingCompetition.SpeedData;

namespace UserForms
{
    /// <summary>
    /// Форма тображает данные, пришедшие со скоростной системы
    /// </summary>
    public partial class DataFromSystem : Form
    {
        private IWin32Window owner;
        private int CLOSE_TIMER;
        private int autoCloseRemaining ;
        private ResStr[] results = null;
        private bool cancel = false;
        private bool autoMode, eventOccured = false, dialogShown = false;
        private Mutex mAS = new Mutex();
        private System.Timers.Timer tEnableClose = new System.Timers.Timer(4990);
        private bool dataCame = false;

        public struct ResStr
        {
            public string res;
            public resTypeE resType;
        }
        public enum resTypeE { ROUTE, SUM, NONE }
        SystemListener sl;
        private bool needToExchange = false;

        private Mutex needToExchangeMutex = new Mutex();
        public bool NeedToExchange
        {
            get
            {
                needToExchangeMutex.WaitOne();
                try { return needToExchange; }
                finally { needToExchangeMutex.ReleaseMutex(); }
            }
            set
            {
                needToExchangeMutex.WaitOne();
                try { needToExchange = value; }
                finally { needToExchangeMutex.ReleaseMutex(); }
            }
        }
        public DataFromSystem(string r1, string r2, SystemListener sl, IWin32Window owner, bool autoMode, bool isQualy,bool needToExchange = false)
        {
            InitializeComponent();
            AllowClose = true;
            tEnableClose.Elapsed += new System.Timers.ElapsedEventHandler(tEnableClose_Elapsed);
            CLOSE_TIMER = appSettings.Default.AutoCloseTimer;
            autoCloseRemaining = CLOSE_TIMER;
            Route1 = r1;
            Route2 = r2;
            this.sl = sl;
            //this.sl.SystemListenerEvent += new SystemListener.SystemListenerEventHandler(sl_SystemListenerEvent);
            this.sl.SubscribeEvent(sl_SystemListenerEvent);

            this.owner = owner;
            this.autoMode = autoMode;
            lblTimer.Text = "";
            autoCloseTimer.Enabled = false;
            btnStopAuto.Visible = this.autoMode;
            btnSetNext2.Visible = btnEnterAndSetNext.Visible = isQualy;
            if (this.autoMode && (CLOSE_TIMER > 0))
                btnSaveRes.Enabled = false;


            if (sl is IvanovoListener)
            {
                this.needToExchange = needToExchange;
            }
            else
            {
                btnExchange.Visible = btnExchange.Enabled = false;
                this.needToExchange = false;
                rbS1.Visible = rbS2.Visible = false;
            }
        }

        bool acl;
        Mutex mAcl = new Mutex();
        bool AllowClose
        {
            get
            {
                mAcl.WaitOne();
                try { return acl; }
                finally { mAcl.ReleaseMutex(); }
            }
            set
            {
                mAcl.WaitOne();
                try { acl = value; }
                finally { mAcl.ReleaseMutex(); }
            }
        }

        void waitForClose()
        {
            while (!AllowClose)
                Thread.Sleep(50);
        }

        void tEnableClose_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AllowClose = true;
            try
            {
                if (sender != null && sender is System.Timers.Timer)
                    ((System.Timers.Timer)sender).Stop();
            }
            catch { }
        }

        public string Route1
        {
            get { return lblClimber1.Text; }
            set
            {
                lblClimber1.Text = value;
                gbR1.Visible = (lblClimber1.Text.Length > 0);
                rbTime1.Checked = true;
            }
        }

        public string Route2
        {
            get { return lblClimber2.Text; }
            set
            {
                lblClimber2.Text = value;
                gbR2.Visible = (lblClimber2.Text.Length > 0);
                rbTime2.Checked = true;
            }
        }

        public void SetResults(string route1, string route2)
        {
            if (lblClimber1.Text.Length > 0)
            {
                rbTime1.Text = route1;
                rbTime1.Checked = true;
            }
            if (lblClimber2.Text.Length > 0)
            {
                rbTime2.Text = route2;
                rbTime2.Checked = true;
            }
        }
        public void SetResMode(int routeN, bool sumB, bool readOld)
        {
            //if (readOld)
            //    try
            //    {
            //        appSettings aSet = appSettings.Default;
            //        aSet.Reload();
            //        //rbR1.Checked = Convert.ToBoolean(StaticClass.GetDataFromRegistry("SysR1"));
            //        rbR1.Checked = aSet.SysR1;
            //        //rbR2.Checked = Convert.ToBoolean(StaticClass.GetDataFromRegistry("SysR2"));
            //        rbR2.Checked = aSet.SysR2;
            //        rbS1.Checked = !rbR1.Checked;
            //        rbS2.Checked = !rbR2.Checked;
            //    }
            //    catch { SetResModeInner(route, sum); }
            //else
            //    SetResModeInner(route, sum);
            RadioButton route, sum;
            if (routeN == 1)
            {
                route = rbR1;
                sum = rbS1;
            }
            else
            {
                route = rbR2;
                sum = rbS2;
            }
            route.Checked = !(sum.Checked = sumB);
            //rbS1.Checked = rbS2.Checked = !(rbR1.Checked = rbR2.Checked = true);
        }

        private void SetResModeInner(int route, bool sum)
        {
            if (route == 1)
            {
                if (sum)
                    rbS1.Checked = true;
                else
                    rbR1.Checked = true;
            }
            else if (route == 2)
            {
                if (sum)
                    rbS2.Checked = true;
                else
                    rbR2.Checked = true;
            }
            else
                throw new ArgumentOutOfRangeException("Incorrect route number");
        }

        private void sl_SystemListenerEvent(object sender, SystemListenerEventArgs e)
        {
            System.Threading.Thread thr = new System.Threading.Thread(ProcessEvent);
            thr.Start(e);
        }

        bool fStOccured = false;

        private void ProcessEvent(object obj)
        {
            try
            {
                if (!(obj is SystemListenerEventArgs))
                    return;
                SystemListenerEventArgs e = (SystemListenerEventArgs)obj;
                if (autoMode && CLOSE_TIMER > 0)
                {
                    mAS.WaitOne();
                    try
                    {
                        eventOccured = (e.EvT == SystemListenerEventArgs.EventType.RESULT ||
                            e.EvT == SystemListenerEventArgs.EventType.FALL);
                        if (dialogShown && !autoCloseTimer.Enabled && eventOccured && !fStOccured)
                            this.Invoke(new EventHandler(delegate { autoCloseTimer.Start(); }));
                    }
                    finally { mAS.ReleaseMutex(); }
                }
                if (e.EvT == SystemListenerEventArgs.EventType.FALSTART)
                    this.Invoke(new EventHandler(delegate
                    {
                        if (autoCloseTimer.Enabled)
                            autoCloseTimer.Stop();
                        fStOccured = true;
                    }));

                if (e.RouteNumber < 1 || e.RouteNumber > 2)
                    return;

                if (this.NeedToExchange)
                    e.RouteNumber = (e.RouteNumber == 1) ? 2 : 1;

                //if (e.RouteNumber == 1 && !gbR1.Visible || e.RouteNumber == 2 && !gbR2.Visible)
                //    return;
                if (!dataCame && eventOccured)
                {
                    dataCame = true;
                    AllowClose = false;
                    tEnableClose.Start();
                }
                switch (e.EvT)
                {
                    case SystemListenerEventArgs.EventType.MESSAGE:
                        MessageBox.Show(e.Res);
                        break;
                    case SystemListenerEventArgs.EventType.FALL:
                        if (e.RouteNumber == 1)
                            rbFall1.Invoke(new EventHandler(delegate { rbFall1.Checked = true; }));
                        else
                            rbFall2.Invoke(new EventHandler(delegate { rbFall2.Checked = true; }));
                        break;
                    case SystemListenerEventArgs.EventType.FALSTART:
                        RadioButton rb;
                        if (e.RouteNumber == 1)
                            rb = rbTime1;
                        else
                            rb = rbTime2;
                        rb.Invoke(new EventHandler(delegate { rb.Text = "FALST"; }));
                        this.Invoke(new EventHandler(delegate { MessageBox.Show(this, "Фальстарт на " + e.RouteNumber.ToString() + " трассе"); }));
                        break;
                    case SystemListenerEventArgs.EventType.RESULT:
                        if (e.RouteNumber == 1)
                        {
                            if (rbTime1.InvokeRequired)
                                rbTime1.Invoke(new EventHandler(delegate { rbTime1.Text = e.Res; }));
                            else
                                rbTime1.Text = e.Res;
                        }
                        else
                        {
                            if (rbTime2.InvokeRequired)
                                rbTime2.Invoke(new EventHandler(delegate { rbTime2.Text = e.Res; }));
                            else
                                rbTime2.Text = e.Res;
                        }
                        break;
                }
            }
            catch { }
        }

        public ResStr[] ShowForm(out bool cancel)
        {
            if (autoMode && CLOSE_TIMER > 0)
            {
                mAS.WaitOne();
                try
                {
                    dialogShown = true;
                    if (eventOccured && !autoCloseTimer.Enabled)
                        autoCloseTimer.Start();
                }
                finally { mAS.ReleaseMutex(); }
            }
            base.ShowDialog(owner);
            cancel = this.cancel;
            return results;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            results = new ResStr[2];
            for (int i = 0; i < results.Length; i++)
            {
                results[i].res = "";
                results[i].resType = resTypeE.NONE;
            }
            cancel = true;
            this.Close();
        }

        private string ResRoute1
        {
            get
            {
                if (rbTime1.Checked)
                    return  rbTime1.Text;
                else if (rbFall1.Checked)
                    return rbFall1.Text;
                else if (rbDisq1.Checked)
                    return rbDisq1.Text;
                else 
                    return rbNya1.Text;
            }
        }

        private string ResRoute2
        {
            get
            {
                if (rbTime2.Checked)
                    return rbTime2.Text;
                else if (rbFall2.Checked)
                    return rbFall2.Text;
                else if (rbDisq2.Checked)
                    return rbDisq2.Text;
                else
                    return rbNya2.Text;
            }
        }

        private void btnSaveRes_Click(object sender, EventArgs e)
        {
            if (!CheckRes())
            {
                MessageBox.Show("Выберите куда вводить результат");
                return;
            }
            results = new ResStr[2];
            if (gbR1.Visible)
            {
                results[0].res = ResRoute1;
                if (rbR1.Checked)
                    results[0].resType = resTypeE.ROUTE;
                else
                    results[0].resType = resTypeE.SUM;
            }
            else
            {
                results[0].res = "";
                results[0].resType = resTypeE.NONE;
            }
            if (gbR2.Visible)
            {
                if (rbTime2.Checked)
                    results[1].res = rbTime2.Text;
                else if (rbFall2.Checked)
                    results[1].res = rbFall2.Text;
                else if (rbDisq2.Checked)
                    results[1].res = rbDisq2.Text;
                else results[1].res = rbNya2.Text;
                if (rbR2.Checked)
                    results[1].resType = resTypeE.ROUTE;
                else
                    results[1].resType = resTypeE.SUM;
            }
            else
            {
                results[1].resType = resTypeE.NONE;
                results[1].res = "";
            }
            appSettings aSet = appSettings.Default;
            aSet.SysR1 = rbR1.Checked;
            aSet.SysR2 = rbR2.Checked;
            aSet.Save();
            //StaticClass.SaveDataToRegistry("SysR1", rbR1.Checked.ToString());
            //StaticClass.SaveDataToRegistry("SysR2", rbR2.Checked.ToString());
            cancel = false;
            this.Close();
        }

        private bool CheckRes()
        {
            return (gbR1.Visible && (rbR1.Checked || rbS1.Checked) || !gbR1.Visible) &&
                (gbR2.Visible && (rbR2.Checked || rbS2.Checked) || !gbR2.Visible);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (autoCloseTimer.Enabled)
                autoCloseTimer.Stop();
            if (sl != null)
                sl.UnsubscribeEvent(sl_SystemListenerEvent);
            waitForClose();
            base.OnClosing(e);
        }

        private void autoCloseTimer_Tick(object sender, EventArgs e)
        {
            if (autoCloseRemaining > 1)
            {
                autoCloseRemaining -= 1;
                lblTimer.Invoke(new EventHandler(delegate
                {
                    lblTimer.Text = "Окно будет закрыто через " + autoCloseRemaining.ToString() + " секунд";
                }));
            }
            else
            {
                if(autoCloseTimer.Enabled)
                    autoCloseTimer.Stop();
                btnSaveRes_Click(null, null);
            }
        }

        public bool StopAuto { get { return !autoMode; } }

        private void btnStopAuto_Click(object sender, EventArgs e)
        {
            btnStopAuto.Enabled = false;
            btnStopAuto.Text = "Автоматический режим будет отключен";
            autoMode = false;
            btnSaveRes.Enabled = true;
        }

        //public delegate void SetOneRouteResultEventHandler(object sender, SetOneRouteResultEventArgs e);

        public event /*SetOneRouteResultEventHandler*/ EventHandler<SetOneRouteResultEventArgs> SetOneRouteEvent;

        private void OnSetOneRouteResultEventHandler(SetOneRouteResultEventArgs e)
        {
            if (SetOneRouteEvent != null)
            {
                SetOneRouteEvent(this, e);
            }
        }

        private void btnEnterAndSetNext_Click(object sender, EventArgs e)
        {
            string res, clm;
            int routeNum;
            if (sender == btnEnterAndSetNext)
            {
                clm = lblClimber1.Text;
                routeNum = 1;
                res = ResRoute1;
            }
            else
            {
                clm = lblClimber2.Text;
                routeNum = 2;
                res = ResRoute2;
            }
            if (MessageBox.Show(this, "Вы уверены, что хотите установить участнку " + clm +
                " на "+routeNum.ToString()+" трассе результат " + res + "?", "", MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question) == DialogResult.Yes)
                OnSetOneRouteResultEventHandler(new SetOneRouteResultEventArgs(routeNum, res));
            //SetOneRouteEvent(this, new SetOneRouteResultEventArgs(1, ResRoute1));
        }

        private void DataFromSystem_Load(object sender, EventArgs e)
        {

        }

        private bool exchangeOccured = false;
        public bool ExchangeOccured { get { return exchangeOccured; } }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbTime1.InvokeRequired)
                    rbTime1.Invoke(new EventHandler(delegate
                    {
                        string str = rbTime1.Text;
                        rbTime1.Text = rbTime2.Text;
                        rbTime2.Text = str;
                    }));
                else
                {
                    string str2 = rbTime1.Text;
                    rbTime1.Text = rbTime2.Text;
                    rbTime2.Text = str2;
                }
                this.exchangeOccured = !this.exchangeOccured;
            }
            catch { }
        }

    }

    public class SetOneRouteResultEventArgs : EventArgs
    {
        private int route;
        private string res;
        public int Route { get { return route; } }
        public string Res { get { return res; } }
        public SetOneRouteResultEventArgs(int _route, string _res)
        {
            this.res = _res;
            this.route = _route;
        }
    }
}