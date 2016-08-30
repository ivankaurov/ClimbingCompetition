// <copyright file="OnlineTest.cs">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Validate
{
    public partial class OnlineTest : Form
    {
        SqlConnection cn;
        public OnlineTest(string style, SqlConnection local)
        {
            InitializeComponent();
            //try
            //{
            string rc;
            if (local.State != ConnectionState.Open)
                local.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = local;
            cmd.CommandText = "SELECT remoteString FROM CompetitionData(NOLOCK)";
            rc = cmd.ExecuteScalar().ToString();
            this.cn = new SqlConnection(rc/*"server=ivank\\sql2000;user=sa;password=sa;initial catalog=ONLclimbing"*/);
            if (this.cn.State != ConnectionState.Open)
                this.cn.Open();
            DataTable dt = ClimbingCompetition.Online.ListCreator.GetResultList(ClimbingCompetition.GetIid.GetInt(), this.cn, 1);
            if (dt == null)
                dt = new DataTable();
            dataGridView1.DataSource = dt;
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try { cn.Close(); }
            catch { }
            base.OnClosing(e);
        }
    }
}