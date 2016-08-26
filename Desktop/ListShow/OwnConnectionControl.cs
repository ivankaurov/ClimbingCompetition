using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ListShow
{
    public partial class OwnConnectionControl : UserControl, IDisposable
    {
        private AsyncConnectionOpener asO = null;

        public OwnConnectionControl()
        {
            InitializeComponent();
        }

        protected SqlConnection cn { get; private set; }

        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                if (value == null)
                {
                    CloseCon();
                    return;
                }
                string oldString = (cn == null) ? "" : cn.ConnectionString;
                if (oldString != value.ConnectionString)
                    OpenConnection(value.ConnectionString);
            }
        }

        public ShowForm ParentShowForm
        {
            get
            {
                return this.ParentForm as ShowForm;
            }
        }

        protected virtual void DoAfterOpenSuccess() { }

        protected virtual void DoAfterOpenFailure(string message)
        {
            if (this.ParentShowForm != null)
                this.ParentShowForm.WriteLog(message, false);
        }

        private void ConnectionOpened(SqlConnection cn, bool res, string errorMsg)
        {
            if (res)
            {
                this.cn = cn;
                DoAfterOpenSuccess();
            }
            else
                DoAfterOpenFailure(errorMsg);
        }

        protected void OpenConnection(string connectionString)
        {
            CloseCon();
            this.curConStr = connectionString;
            asO = new AsyncConnectionOpener(connectionString);
            asO.DoOpen(ConnectionOpened);
        }
        protected string curConStr = "";
        protected virtual void StopTimer() { }
        protected virtual void DoBeforeClose() { }

        public void CloseCon()
        {
            try
            {
                if (asO != null && asO.IsAlive)
                    asO.AbortOperation();
                asO = null;
            }
            catch { }
            try { StopTimer(); }
            catch { }
            try { DoBeforeClose(); }
            catch { }
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    cn.Close();
            }
            catch { }
            cn = null;
            GC.Collect();
        }
    }
}
