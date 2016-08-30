// <copyright file="PhotoControl.xaml.cs">
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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for PhotoControl.xaml
    /// </summary>
    public partial class PhotoControl : UserControl
    {
        public PhotoControl()
        {
            InitializeComponent();
            ProjNum = 1;
        }
        private SqlConnection cn=null;
        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                if (value != null && value.State != System.Data.ConnectionState.Open)
                    value.Open();
                cn = value;
                clmResults.Connection = clmPhoto.Connection = clmPresentation.Connection = cn;
            }
        }
        private ListShow.ClimberData cClimber = null;

        private int CurrentClimber
        {
            get
            {
                if (cClimber == null)
                    return -1;
                return cClimber.Iid;
            }
            set
            {
                cClimber = new ListShow.ClimberData(value);
                if (cn != null)
                    cClimber.LoadData(cn);
                clmResults.CurrentClimber = clmPhoto.CurrentClimber = clmPresentation.CurrentClimber = cClimber;
            }
        }

        public int ProjNum { get; set; }
        public List<ListShow.BasePhotoControl.ListStruct> showingLists = new List<ListShow.BasePhotoControl.ListStruct>();
        int currentList = -1;
        public bool LoadData()
        {
            if (cn == null)
                return false;
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            showingLists = ListShow.BasePhotoControl.LoadData(cn, ListShow.BasePhotoControl.ClimbingStyleType.Трудность, ProjNum);
            currentList = -1;
            return ScrollToNext();
        }

        public bool ScrollToNext()
        {
            currentList++;
            if (currentList >= showingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing FROM lists(NOLOCK) WHERE iid=" + showingLists[currentList].iid.ToString();
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return ScrollToNext();
            CurrentClimber = Convert.ToInt32(oTmp);
            return true;
        }
    }
}
