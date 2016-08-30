// <copyright file="NextRoundForm.cs">
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
    public partial class NextRoundForm : Form
    {
        private readonly string[] rounds = new string[] { "1/2 финала", "Финал", "Суперфинал" };

        private string round = "";
        private int routeNumber = -1;
        private bool cancel = false;

        private NextRoundForm(string prevRound)
        {
            InitializeComponent();
            int startInd;
            switch (prevRound.ToLower())
            {
                case "1/2 финала":
                    startInd = 1;
                    break;
                case "финал":
                    startInd = 2;
                    break;
                default:
                    startInd = 0;
                    break;
            }
            for (int i = startInd; i < rounds.Length; i++)
                cbRound.Items.Add(rounds[i]);
        }
        private bool validateData()
        {
            if (cbRound.SelectedIndex < 0 || cbRound.SelectedItem == null)
            {
                MessageBox.Show(this, "Раунд выбран неверно");
                return false;
            }
            round = cbRound.SelectedItem.ToString();
            if (cbRound.SelectedItem.ToString() != rounds[rounds.Length - 1])
            {
                if (!int.TryParse(textBox1.Text, out routeNumber))
                {
                    MessageBox.Show(this, "Число трасс введено неверно");
                    return false;
                }
            }
            else
                routeNumber = -1;
            return true;
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            if (validateData())
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cancel = true;
            this.Close();
        }

        public static bool GetNextRoundData(string prevRound, IWin32Window owner, out string round, out int routeNumber)
        {
            NextRoundForm nf = new NextRoundForm(prevRound);
            if (owner == null)
                nf.ShowDialog();
            else
                nf.ShowDialog(owner);
            round = nf.round;
            routeNumber = nf.routeNumber;
            return nf.cancel;
        }
    }
}
