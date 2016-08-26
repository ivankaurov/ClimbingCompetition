using ClimbingCompetition.Client;
using Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    partial class OnlineUpdater2
    {
        readonly object runningLocker = new object();
        volatile bool isRunning = false;

        volatile int updateInterval = 30;

		public bool IsRunning { get { return this.isRunning; } }

		public int UpdateInterval
        {
            get { return this.updateInterval; }
            set { this.updateInterval = Math.Max(1, value); }
        }

		public void Cancel()
        {
            lock (runningLocker)
            {
                isRunning = false;
            }
        }

		public void Start()
        {
            lock (runningLocker)
            {
                if (isRunning)
                    return;
                if (!ServiceClient.GetInstance(this.cn).ConnectionSet)
                {
                    MessageBox.Show("Соединение не установлено");
                    return;
                }

                this.isRunning = true;
                ThreadPool.QueueUserWorkItem(obj => this.UpdateCycle());
            }
        }

        public event EventHandler UpdateStarted;
        public event EventHandler UpdateFinished;
        public event EventHandler<ExceptionEventArgs> UpdateFailed;

		public class ExceptionEventArgs : DataEventArgs<Exception>
        {
            public bool Handled { get; private set; }


			public void Handle()
            {
                if (!this.Handled)
                    this.Handled = true;
            }

            public ExceptionEventArgs(Exception e) : base(e) { }
        }

		private void RaiseUpdateStarted()
        {
            var e = Interlocked.CompareExchange(ref UpdateStarted, null, null);
            if (e != null)
                e(this, new EventArgs());
        }

        private void RaiseUpdateFinished()
        {
            var e = Interlocked.CompareExchange(ref UpdateFinished, null, null);
            if (e != null)
                e(this, new EventArgs());
        }

		private bool RaiseUpdateFailed(Exception e)
        {
            var eA = Interlocked.CompareExchange(ref UpdateFailed, null, null);
            if (eA == null)
                return false;

            var args = new ExceptionEventArgs(e);
            eA(this, args);
            return args.Handled;
        }

        private bool UpdateFunction()
        {
            try
            {
                this.RaiseUpdateStarted();
                if (!this.DoActionWithExceptionCheck(() => this.PostUpdatedGroups()))
                    return false;

                if (!this.DoActionWithExceptionCheck(() => this.PostChangedClimbers()))
                    return false;

                if (!this.DoActionWithExceptionCheck(() => this.LoadChangedLists()))
                    return false;
                return this.isRunning;
            }
            finally
            {
                this.RaiseUpdateFinished();
            }
        }

		private void UpdateCycle()
        {
            while (this.UpdateFunction())
                Thread.Sleep(this.updateInterval * 1000);
        }

        private bool DoActionWithExceptionCheck(Action action)
        {
            if (!isRunning)
                return false;
            try
            {
                try { action(); }
                catch (SqlException)
                {
                    this.cn.Close();
                    action();
                }
                return true;
            }
            catch (ThreadAbortException) { return false; }
			catch(Exception ex)
            {
                if (this.RaiseUpdateFailed(ex))
                    return true;
                else
                {
                    isRunning = false;
                    return false;
                }
            }
        }
    }
}
