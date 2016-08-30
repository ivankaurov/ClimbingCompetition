// <copyright file="BaseControl.cs">
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
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebClimbing.src
{
    public class BaseControl : System.Web.UI.UserControl
    {
        private SqlConnection _cn = null;
        protected SqlConnection cn
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.cn;
                if (_cn == null)
                    _cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                if (_cn.State != System.Data.ConnectionState.Open)
                    _cn.Open();
                return _cn;
            }
        }

        private Entities _dc = null;
        public Entities dc
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.dc;
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            _compID = this.GetCompID();
        }

        private long _compID = -1;
        public long compID
        {
            get
            {
                if (this.BasePage != null)
                    return this.BasePage.compID;
                return _compID;
            }
        }

        ~BaseControl()
        {
            try
            {
                if (_cn != null && _cn.State != System.Data.ConnectionState.Closed)
                    _cn.Close();
            }
            catch { }
            try
            {
                if (_dc != null && _dc.Connection != null && _dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Close();
            }
            catch { }
            try { Dispose(); }
            catch { }
        }

        public BasePage BasePage { get { return this.Page as BasePage; } }
    }
}