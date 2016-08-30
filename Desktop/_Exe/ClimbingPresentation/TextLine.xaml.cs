// <copyright file="TextLine.xaml.cs">
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Threading;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for TextLine.xaml
    /// </summary>
    public partial class TextLine : UserControl
    {
        private const double DEFAULT_INTERVAL = 60;
        System.Timers.Timer tShift = new System.Timers.Timer(DEFAULT_INTERVAL),
            tReload = new System.Timers.Timer(5000);


        Mutex mEvent = new Mutex();

        public TextLine()
        {
            InitializeComponent();
            tShift.AutoReset = false;
            tShift.Stop();
            tReload.AutoReset = false;
            tReload.Stop();
            tShift.Elapsed += new System.Timers.ElapsedEventHandler(tShift_Elapsed);
            tReload.Elapsed += new System.Timers.ElapsedEventHandler(tReload_Elapsed);
        }

        void tReload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mEvent.WaitOne();
            try
            {
                tReload.Stop();
                tShift.Start();
            }
            finally { mEvent.ReleaseMutex(); }
        }

        void tShift_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mEvent.WaitOne();
            bool needStart = true;
            try
            {
                tShift.Stop();
                bool needWait = false;
                if (cn == null)
                    return;
                if (currentString == string.Empty)
                {
                    if (cn.State != System.Data.ConnectionState.Open)
                        cn.Open();
                    needWait = true;
                    currentString = ListShow.TextLine.FillFullNowClimbing(cn);
                    try { tShift.Interval = ListShow.TextLine.LoadTimerInterval(cn); }
                    catch { }

                    if (this.Dispatcher.CheckAccess())
                        this.Visibility = (String.IsNullOrEmpty(currentString) ? Visibility.Collapsed : Visibility.Visible);
                    else
                        this.Dispatcher.Invoke(DispatcherPriority.Send,
                            (ParameterizedThreadStart)delegate(object obj)
                        {
                            string s = obj as string;
                            this.Visibility = (String.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible);
                        }, currentString );

                    if (currentString == string.Empty)
                    {
                        needStart = false;
                        tReload.Start();
                        return;
                    }
                }
                else
                    currentString = currentString.Substring(1);
                if (lblText.Dispatcher.CheckAccess())
                    lblText.Content = currentString;
                else
                    lblText.Dispatcher.Invoke(DispatcherPriority.Send,
                        (ParameterizedThreadStart)delegate(object p)
                    {
                        lblText.Content = p as string;
                    }, currentString);
                if (needWait)
                    Thread.Sleep(500);
            }
            catch { }
            finally
            {
                if (needStart)
                    tShift.Start();
                mEvent.ReleaseMutex();
            }
        }

        private string currentString = string.Empty;

        private SqlConnection cn = null;
        public string ConnectionString
        {
            get
            {
                return (cn == null ? String.Empty : cn.ConnectionString);
            }
            set
            {
                if (cn != null && cn.State != System.Data.ConnectionState.Closed)
                    cn.Close();
                try
                {
                    cn = new SqlConnection(value);
                    cn.Open();
                }
                catch { }
            }
        }

        public bool IsRunning
        {
            get
            {
                mEvent.WaitOne();
                try { return (tShift.Enabled || tReload.Enabled); }
                finally { mEvent.ReleaseMutex(); }
            }
            set
            {
                mEvent.WaitOne();
                try
                {
                    if (value)
                    {
                        if (tReload.Enabled)
                            tReload.Stop();
                        if (!tShift.Enabled)
                            tShift.Start();
                    }
                    else
                    {
                        tShift.Stop();
                        tReload.Stop();
                    }
                }
                finally { mEvent.ReleaseMutex(); }
            }
        }

        public void DisposeConnection()
        {
            try
            {
                if (cn != null && cn.State != System.Data.ConnectionState.Closed)
                    cn.Close();
            }
            catch { }
            try
            {
                cn = null;
                GC.Collect();
            }
            catch { }
        }

        ~TextLine() { DisposeConnection(); }
    }
}
