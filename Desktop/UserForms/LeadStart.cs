// <copyright file="LeadStart.cs">
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

namespace ClimbingCompetition
{
    public partial class LeadStart : Form
    {
        protected LeadStart()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        public static bool StartClimber(string climberName)
        {
            LeadStart ls = new LeadStart();
            ls.Text = climberName;
            return (ls.ShowDialog() == DialogResult.OK);
        }

        System.Timers.Timer t = null;

        int remainingSeconds = 0;

        private void LeadStart_Load(object sender, EventArgs e)
        {
            t = new System.Timers.Timer(1000);
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            remainingSeconds = 40;
            updateLabel();
            t.Start();
        }

        private void updateLabel()
        {
            string sToSet = "00:" + remainingSeconds.ToString("00");
            if (label1.InvokeRequired)
                label1.Invoke(new EventHandler(delegate { label1.Text = sToSet; }));
            else
                label1.Text = sToSet;
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (remainingSeconds > 0)
            {
                remainingSeconds--;
                updateLabel();
            }
            else
            {
                System.Timers.Timer _t = sender as System.Timers.Timer;
                if (_t != null)
                    _t.Stop();
                MessageBox.Show("Время истекло", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            t.Stop();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            t.Stop();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
