// <copyright file="TimerShowForm.cs">
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
using System.Data.SqlClient;
using ClimbingCompetition;

namespace BoulderTimer
{
    public partial class TimerShowForm : Form
    {
        private SqlConnection cn = null;

        public TimerShowForm()
        {
            InitializeComponent();
        }

        private void TimerShowForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    CloseAll();
                AccountForm af = new AccountForm(String.Empty, true);
                af.ShowDialog();
                if (af.ExitPressed || String.IsNullOrEmpty(af.ConnectionString))
                    this.Close();
                cn = new SqlConnection(af.ConnectionString);
                cn.Open();
                this.timerControl.ConnectionString = cn.ConnectionString;
                try
                {
                    BoulderTimer.Properties.Settings set = BoulderTimer.Properties.Settings.Default;
                    this.timerControl.Font = new Font(set.FontFamily, set.FontSize);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка соединения:\r\n" + ex.Message);
                this.Close();
            }
        }

        private void CloseAll()
        {
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    cn.Close();
                if (cn != null)
                    cn = null;
                GC.Collect();
            }
            catch { }
        }

        private void TimerShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                BoulderTimer.Properties.Settings st = BoulderTimer.Properties.Settings.Default;
                st.FontFamily = this.timerControl.Font.FontFamily.Name;
                st.FontSize = this.timerControl.Font.Size;
                st.Save();
            }
            catch { }
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var r = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                    "Выйти из приложения?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
