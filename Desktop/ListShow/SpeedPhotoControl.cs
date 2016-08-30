// <copyright file="SpeedPhotoControl.cs">
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

namespace ListShow
{
    public partial class SpeedPhotoControl : ListShow.BasePhotoControl
    {
        public SpeedPhotoControl()
        {
            AllowEvent = false;
            InitializeComponent();
        }

        protected override void DoLayout()
        {
            this.spcMain.SuspendLayout();
            this.spcLeft.SuspendLayout();
            this.spcLeftBottom.SuspendLayout();

            this.Controls.Remove(namesPanel);
            this.spcLeft.Panel1.Controls.Add(namesPanel);
            this.namesPanel.Dock = DockStyle.Fill;

            this.Controls.Remove(pbPhoto);
            this.spcLeftBottom.Panel1.Controls.Add(pbPhoto);
            this.pbPhoto.Dock = DockStyle.Fill;

            this.Controls.Remove(listBox1);
            this.spcLeftBottom.Panel2.Controls.Add(listBox1);
            this.listBox1.Dock = DockStyle.Fill;

            this.spcMain.Dock = DockStyle.Fill;

            this.spcLeftBottom.ResumeLayout(false);
            this.spcLeftBottom.PerformLayout();
            this.spcLeft.ResumeLayout(false);
            this.spcLeft.PerformLayout();
            this.spcMain.ResumeLayout(false);
            this.spcMain.PerformLayout();
            this.ResumeLayout(false);
        }

        private ClimberData clRight = ClimberData.Empty;

        protected override bool SetCurrentList()
        {
            if (currentList < 0 || currentList >= ShowingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing l, nowClimbingTmp r FROM lists(NOLOCK) WHERE iid=" + ShowingLists[currentList].iid.ToString();

            int iRight, iLeft;
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                if (rdr.Read())
                {
                    if (rdr["l"] != DBNull.Value)
                        iLeft = Convert.ToInt32(rdr["l"]);
                    else
                        iLeft = int.MinValue;
                    if (rdr["r"] != DBNull.Value)
                        iRight = Convert.ToInt32(rdr["r"]);
                    else
                        iRight = int.MinValue;
                }
                else
                    return false;
            }
            finally { rdr.Close(); }
            if (iLeft <= 0 && iRight <= 0)
                return false;
            ClimberData newLeft, newRight;
            bool leftSet = true, rightSet = true;
            if (iLeft <= 0)
                newLeft = ClimberData.Empty;
            else if (iLeft == currentClimber.Iid)
            {
                leftSet = false;
                newLeft = currentClimber;
            }
            else if (iLeft == clRight.Iid)
                newLeft = clRight;
            else
            {
                newLeft = new ClimberData(iLeft);
                if (!newLeft.LoadData(cn))
                    return false;
            }

            if (iRight <= 0)
                newRight = ClimberData.Empty;
            else if (iRight == clRight.Iid)
            {
                rightSet = false;
                newRight = clRight;
            }
            else if (iRight == currentClimber.Iid)
                newRight = currentClimber;
            else
            {
                newRight = new ClimberData(iRight);
                if (!newRight.LoadData(cn))
                    return false;
            }
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate
                {
                    if (leftSet)
                        SetClimberData(newLeft);
                    if (rightSet)
                        SetClimberToRight(newRight);
                }));
            else
            {
                if (leftSet)
                    SetClimberData(newLeft);
                if (rightSet)
                    SetClimberToRight(newRight);
            }
            currentClimber = newLeft;
            clRight = newRight;
            return true;
        }

        private void SetClimberToRight(ClimberData clm)
        {
            ClimberData c;
            if (clm == null)
                c = ClimberData.Empty;
            else
                c = clm;
            tbRightAge.Text = c.AgeStr;
            tbRightGroup.Text = c.Group;
            tbRightNum.Text = c.IidStr;
            tbRightQf.Text = c.Qf;
            tbRightSurname.Text = c.Name;
            tbRightTeam.Text = c.Team;
            pbRightPhoto.Image = c.Photo;
            c.SetToListBox(listBoxRight);
        }

        private void spcLeftBottom_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (sender == null || !(sender is SplitContainer))
                return;
            if (EnterEvent())
                try
                {
                    SplitContainer sp = (SplitContainer)sender;
                    sp.SplitterDistance = sp.Width / 2;
                }
                catch { }
                finally { ExitEvent(); }
        }

        private void spcLeft_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (EnterEvent())
                try
                {
                    if (sender == null || !(sender is SplitContainer))
                        return;
                    SplitContainer spc = (SplitContainer)sender;
                    try
                    {
                        if (spc.SplitterDistance > 230)
                            spc.SplitterDistance = 230;
                    }
                    catch { }
                    if (sender == spcLeft)
                        spcRight.SplitterDistance = spcLeft.SplitterDistance;
                    else
                        spcLeft.SplitterDistance = spcRight.SplitterDistance;
                }
                catch { }
                finally { ExitEvent(); }
        }

        private void spcMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (EnterEvent())
                try { spcMain.SplitterDistance = spcMain.Width / 2; }
                catch { }
                finally { ExitEvent(); }
        }
    }
}
