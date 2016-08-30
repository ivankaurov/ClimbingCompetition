// <copyright file="IExceptionLogger.cs">
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
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ClimbingCompetition
{
    public interface IExceptionLogger
    {
        void WriteLog(Exception ex, bool needToShow);
        void WriteLog(string message, bool needToShow);
    }

    public static class CExceptionLogger
    {
        public static string LogFile
        {
            get { return ClimbingCompetition.Properties.Settings.Default.LogFile; }
            set
            {
                ClimbingCompetition.Properties.Settings lsSet = ClimbingCompetition.Properties.Settings.Default;
                lsSet.LogFile = value;
                lsSet.Save();
            }
        }
        public static void WriteLogStatic(string message, Form owner, bool needToShow)
        {
            try
            {
                string fileName = ClimbingCompetition.Properties.Settings.Default.LogFile;
                string msg = DateTime.Now.ToString("G") + ": " + message;
                if (fileName.Length < 1 || needToShow)
                {
                    Thread thr = new Thread(new ParameterizedThreadStart(delegate(object arg)
                    {
                        try
                        {
                            if (arg == null || !(arg is string))
                                return;
                            string s = (string)arg;
                            if (owner == null)
                                MessageBox.Show(s);
                            else if (owner.InvokeRequired)
                                owner.Invoke(new EventHandler(delegate { MessageBox.Show(owner, s, "Ошибка!"); }));
                            else
                                MessageBox.Show(owner, s, "Ошибка!");
                        }
                        catch { }
                    }));
                    thr.Start(msg);
                }
                if (!String.IsNullOrEmpty(fileName))
                {
                    StreamWriter swr = new StreamWriter(fileName, true);
                    try { swr.WriteLine(DateTime.Now.ToString("G") + ": " + message); }
                    finally { swr.Close(); }
                }
            }
            catch { }
        }
    }
}
