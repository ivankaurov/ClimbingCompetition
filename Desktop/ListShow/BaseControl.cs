using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace ListShow
{
    public class BaseControl : UserControl
    {
        private SqlConnection con = null;
        public SqlConnection cn { get { return con; } set { con = value; } }

        public bool AllowEvent { get; set; }

        public BaseControl()
            : base()
        {
            this.KeyPress += new KeyPressEventHandler(BaseControl_KeyPress);
            this.AllowEvent = false;
        }

        void BaseControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                if (ParentF != null)
                {
                    if (ParentF.InvokeRequired)
                        ParentF.Invoke(new EventHandler(delegate { ParentF.OnKeyPressP(true); }));
                    else
                        ParentF.OnKeyPressP(true);
                }
        }

        protected int projNum;
        public int ProjNum { get { return projNum; } set { projNum = value; } }

        public ShowForm ParentF
        {
            get
            {
                return this.ParentForm as ShowForm;
            }
        }

        public virtual bool LoadData() { return false; }
        public virtual bool MoveNext() { return false; }
        public virtual bool RefreshCurrent() { return false; }

        private Mutex mEvent = new Mutex();
        protected bool EnterEvent()
        {
            return mEvent.WaitOne(0, false);
        }

        protected void ExitEvent()
        {
            mEvent.ReleaseMutex();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseControl
            // 
            this.Name = "BaseControl";
            this.Resize += new System.EventHandler(this.BaseControl_Resize);
            this.ResumeLayout(false);

        }

        protected virtual void BaseControl_Resize(object sender, EventArgs e)
        {
        }
    }

    public class AsyncConnectionOpener
    {
        private SqlConnection cn;
        private Thread thr = null;



        public AsyncConnectionOpener(SqlConnection cn)
        {
            this.cn = cn;
        }

        public AsyncConnectionOpener(string connectionString)
        {
            this.cn = new SqlConnection(connectionString);
        }

        public void DoOpen(ConnectionOpenedDelegate res)
        {
            thr = new Thread(OpenConnection);
            thr.Start(res);
        }

        public bool AbortOperation()
        {
            if (thr != null)
                try
                {
                    if (thr.IsAlive)
                        thr.Abort();
                    return true;
                }
                catch { return false; }
            else
                return true;
        }

        public bool IsAlive { get { return (thr != null && thr.IsAlive); } }

        private void OpenConnection(object obj)
        {
            ConnectionOpenedDelegate res = (ConnectionOpenedDelegate)obj;
            bool r;
            string erMsg;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                r = true;
                erMsg = "";
            }
            catch (Exception ex)
            {
                r = false;
                erMsg = ex.ToString();
            }
            res(cn, r, erMsg);
        }

        public delegate void ConnectionOpenedDelegate(SqlConnection cn, bool result, string errorMessage);
    }
}
