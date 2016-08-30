// <copyright file="TmeWorkForm.cs">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма отображается при выполнении операции резервного копирования БД и т.п. Тупо идёт секундомер
    /// </summary>
    public partial class TmeWorkForm : Form
    {
        AccountForm.TimeConsumingWork func;
        Thread thr = null;
        int min = 0, sec = 0;
        bool autoHide;
        public TmeWorkForm(string caption, AccountForm.TimeConsumingWork func, bool autoHide)
        {
            this.autoHide = autoHide;
            InitializeComponent();
            this.Text = caption;
            this.func = func;
            thr = new Thread(ThreadFunc);
            try { thr.TrySetApartmentState(ApartmentState.STA); }
            catch { }
            thr.Start();
        }

        private void ThreadFunc()
        {
            func();
            timer.Stop();
            if (!autoHide)
                MessageBox.Show("Процесс завершён");
            try
            {
                this.Invoke(new EventHandler(delegate
                {
                    try { this.Close(); }
                    catch { }
                }));
            }
            catch { }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sec == 59)
            {
                min++;
                sec = 0;
            }
            else
                sec++;
            label2.Invoke(new EventHandler(delegate
            {
                label2.Text = min.ToString("00") + ":" + sec.ToString("00");
            }));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (thr != null)
                    thr.Abort();
                timer.Stop();
                MessageBox.Show("Процесс отменён");
                this.Invoke(new EventHandler(delegate { this.Close(); }));
            }
            catch (Exception ex)
            { MessageBox.Show("Отмена невозможна:\r\n" + ex.Message); }
        }
    }
}
