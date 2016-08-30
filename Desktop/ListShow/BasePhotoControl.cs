// <copyright file="BasePhotoControl.cs">
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
using System.Data.SqlClient;
using ListShow.DsListShowTableAdapters;

namespace ListShow
{
    public partial class BasePhotoControl : BaseControl
    {
        public struct ListStruct
        {
            public int iid;
            public string name;
        }
        protected List<ListStruct> ShowingLists = new List<ListStruct>();
        protected int currentList = 0;
        public enum ClimbingStyleType { Трудность, Скорость }
        public ClimbingStyleType ClimbingStyle { get; set; }

        public BasePhotoControl()
        {
            InitializeComponent();
        }

        protected virtual void DoLayout() { }

        public override bool LoadData()
        {
            ShowingLists = LoadData(cn, ClimbingStyle, projNum);
            currentList = 0;
            return (ShowingLists.Count > 0);
        }

        public static List<ListStruct> LoadData(SqlConnection cn, ClimbingStyleType style, int projNum)
        {
            List<ListStruct> rV = new List<ListStruct>();
            ListsForShowTableAdapter lta = new ListsForShowTableAdapter();
            lta.Connection = cn;
            DsListShow.ListsForShowDataTable dt = lta.GetData(projNum);
            foreach (DsListShow.ListsForShowRow row in dt.Rows)
                if (row.showPhoto && row.style == style.ToString())
                {
                    ListStruct lstr;
                    lstr.iid = row.iid;
                    lstr.name = row.grp + " " + row.round + " " + row.style;
                    rV.Add(lstr);
                }
            return rV;
        }

        protected bool lastPassed = false;

        public override bool MoveNext()
        {
            if (lastPassed)
            {
                lastPassed = false;
                return false;
            }
            if (ShowingLists.Count == 0 || currentList >= ShowingLists.Count)
                if (!LoadData())
                    return false;

            while (currentList < ShowingLists.Count)
                if (SetCurrentList())
                {
                    if (currentList == (ShowingLists.Count - 1))
                        lastPassed = true;
                    currentList++;
                    return true;
                }
                else
                    currentList++;
            return false;


        }

        protected ClimberData currentClimber = ClimberData.Empty;
        protected virtual bool SetCurrentList()
        {
            if (currentList < 0 || currentList >= ShowingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing FROM lists(NOLOCK) WHERE iid=" + ShowingLists[currentList].iid.ToString();
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return false;
            int curClm = Convert.ToInt32(oTmp);
            if (currentClimber != null && curClm == currentClimber.Iid)
                return true;
            ClimberData c = new ClimberData(curClm);
            if (!c.LoadData(cn))
                return false;
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate { SetClimberData(c); }));
            else
                SetClimberData(c);
            currentClimber = c;
            return true;
        }

        protected virtual void SetClimberData(ClimberData clm)
        {
            ClimberData c;
            if (clm == null)
                c = ClimberData.Empty;
            else
                c = clm;
            tbPartAge.Text = c.AgeStr;
            tbPartGroup.Text = c.Group;
            tbPartNumber.Text = c.IidStr;
            tbPArtQf.Text = c.Qf;
            tbPartSurname.Text = c.Name;
            tbPartTeam.Text = c.Team;
            pbPhoto.Image = c.Photo;
            c.SetToListBox(listBox1);
        }

        public override bool RefreshCurrent()
        {
            return true;
        }

        private void BasePhotoControl_Load(object sender, EventArgs e)
        {
            DoLayout();
        }
    }
}
