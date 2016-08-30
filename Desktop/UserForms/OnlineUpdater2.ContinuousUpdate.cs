// <copyright file="OnlineUpdater2.ContinuousUpdate.cs">
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

﻿using ClimbingCompetition.Client;
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
