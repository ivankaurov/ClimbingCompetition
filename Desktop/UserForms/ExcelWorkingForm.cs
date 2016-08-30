// <copyright file="ExcelWorkingForm.cs">
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
    public partial class ExcelWorkingForm : Form
    {
        public ExcelWorkingForm()
        {
            InitializeComponent();
        }
        private bool directionUp = true;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (directionUp)
            {
                if (progressBar1.Value == 99)
                    directionUp = false;
                progressBar1.Value++;
            }
            else
            {
                if (progressBar1.Value == 1)
                    directionUp = true;
                progressBar1.Value--;
            }
        }

        private void ExcelWorkingForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            StaticClass.ExecutionFinished += new StaticClass.ExecutionFinishedEventHandler(StaticClass_ExecutionFinished);
        }

        void StaticClass_ExecutionFinished(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(delegate(object o, EventArgs ec)
            {
                this.Close();
            }));
        }

        private void ExcelWorkingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            StaticClass.ExecutionFinished -= this.StaticClass_ExecutionFinished;
        }
    }
}
