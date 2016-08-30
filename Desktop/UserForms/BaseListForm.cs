// <copyright file="BaseListForm.cs">
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
using System.Net;
using System.Globalization;

namespace ClimbingCompetition
{
    public partial class BaseListForm : ClimbingCompetition.BaseForm
    {
        private bool lockSet = false;
        public const int EMPTY_LIST = -1;

        private readonly int list_id;
        private readonly string hostName;

        protected int listID { get { return list_id; } }
        protected string HostName { get { return hostName; } }

        protected ToolTip ToolTipDefault { get { return formToolTip; } }

        public BaseListForm() : this(null) { }
        public BaseListForm(String baseResourceFile/* = "ClimbingCompetition.ListHeaderStrings"*/)
            : base(null, "", baseResourceFile)
        {
            InitializeComponent();
            this.list_id = EMPTY_LIST;
            this.hostName = "";
        }

        
        public BaseListForm(SqlConnection cn, string competitionTitle, int listID, String baseResourceFile = "ClimbingCompetition.ListHeaderStrings")
            : base(cn, competitionTitle, baseResourceFile)
        {
            InitializeComponent();
            this.list_id = listID;
            try { hostName = Dns.GetHostName().ToUpper(); }
            catch { hostName = ""; }
            CheckLock();
        }

        private void CheckLock()
        {
            string sLockedBy = GetListOwner();
            if (sLockedBy != HostName)
            {
                if (sLockedBy.Length < 1)
                    SetLock(true);
                else
                    switch (MessageBox.Show("Данный протокол, возможно, редактируется с компьютера " + sLockedBy + ".\r\n" +
                        "Изменить владельца протокола?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        case DialogResult.Yes:
                            SetLock(true);
                            break;
                        case DialogResult.Cancel:
                            needToClose = true;
                            break;
                    }
            }
            //else
            //    MessageBox.Show("Данный протокол, возможно, уже открыт на Вашем компьютере.");
        }

        //protected override void GetFormDataKeys(out string typeName, out string hostName)
        //{
        //    String sTypeName;
        //    base.GetFormDataKeys(out sTypeName, out hostName);
        //    typeName = String.Format("{0}.{1}", sTypeName, list_id);
        //    if (typeName.Length > BaseForm.FORM_CLASS_FIELD_LENGTH)
        //        typeName = typeName.Substring(0, BaseForm.FORM_CLASS_FIELD_LENGTH);
        //}

        protected string GetListOwner()
        {
            if (cn == null || cn.ConnectionString.Length < 1)
                return "";
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SortingClass.CheckColumn("lists", "IsLocked", "VARCHAR(255) NULL", cn);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT IsLocked FROM lists WHERE iid=" + listID.ToString();
                object oTmp = cmd.ExecuteScalar();
                if (oTmp == null || oTmp == DBNull.Value)
                    return "";
                return oTmp.ToString();
            }
            catch { return ""; }
        }

        protected void SetLock(bool setLock)
        {
            if (cn == null || cn.ConnectionString.Length < 1)
                return;
            try
            {
                if (listID < 1 || listID == EMPTY_LIST)
                    return;
                SortingClass.CheckColumn("lists", "IsLocked", "VARCHAR(255) NULL", cn);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE lists SET IsLocked = " + (setLock ? "'" + HostName + "'" : "NULL") +
                    " WHERE iid = " + listID.ToString();
                cmd.ExecuteNonQuery();
                this.lockSet = setLock;
            }
            catch { }
        }

        private void BaseListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(lockSet)
                try { SetLock(false); }
                catch { }
        }

        public string AGE, CLIMBER, IID, QF, TIME, RESULT, TEAM, TIMERES, START, POS, PTS, NEXTROUND, ROUTEJUDGE,
            LEADNAME, SPEEDNAME, BOULDERNAME, RESULTLIST, STARTLIST, RANKING, PREQF, ROUTE,
            TIMEISOOPEN, TIMEISOCLOSE, TIMEOBSERVATION, TIMESTART, TIMECLIMB, MINUTE;
    }
}
