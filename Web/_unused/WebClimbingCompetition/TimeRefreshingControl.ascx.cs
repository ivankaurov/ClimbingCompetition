// <copyright file="TimeRefreshingControl.ascx.cs">
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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClimbing
{
    public partial class _CTimeRefreshingControl : System.Web.UI.UserControl,IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public int Time
        {
            get { return ParseLabel(Label1.Text); }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be greater than zero");
                Label1.Text = "Автоматическое обновление через " + value.ToString() + " секунд";
            }
        }

        public bool TimerEnabled
        {
            get { return Timer1.Enabled; }
            set
            {
                Timer1.Enabled = value;
                if (value)
                    Label1.Text = "Автоматическое обновление через 60 секунд";
                else
                    Label1.Text = "";
                this.Visible = value;
            }
        }

        static int ParseLabel(string str)
        {
            int i1 = str.IndexOf("через");
            int i2 = str.IndexOf("сек");
            if (i1 > 0 && i2 > 0)
            {
                int l = i2 - i1 - 7;
                string strr = str.Substring(i1 + 6, l);
                return int.Parse(strr);
            }
            else
                return -1;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            int tm = ParseLabel(Label1.Text);
            tm -= 10;
            if (tm <= 0)
            {
                if(RefreshListEvent!= null)
                    RefreshListEvent(this, new EventArgs());
                tm = 180;
            }
            Label1.Text = "Автоматическое обновление через " + tm.ToString() + " секунд";
        }

        public delegate void RefreshListEventHandler(object sender, EventArgs e);
        public event RefreshListEventHandler RefreshListEvent;

        #region Члены IPostBackEventHandler

        public void RaisePostBackEvent(string eventArgument)
        {
            if (RefreshListEvent != null)
                RefreshListEvent(null, new EventArgs());
        }

        #endregion
    }
}