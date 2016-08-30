// <copyright file="FindOtherLists.cs">
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

namespace ClimbingCompetition
{
    public sealed partial class FindOtherLists : Form
    {
        private FindOtherLists(SqlConnection cn, SqlTransaction tran, int listId)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT G.name + ' ' + L.round list_name, L.iid" +
                              "  FROM lists L(nolock)" +
                              "  JOIN groups G(nolock) on G.iid = L.group_id" +
                              " WHERE L.iid <> @iid" +
                              "   AND L.topo IS NOT NULL" +
                           " ORDER BY G.name, L.round";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
            comboBox1.Items.Clear();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    comboBox1.Items.Add(new ListTopo { Iid = Convert.ToInt32(rdr["iid"]), Tag = (string)rdr["list_name"] });
                }
            }
            comboBox1.Text = "Выбрать протокол";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show(this, "Протокол не выбран");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public static ListTopo SelectList(IWin32Window owner, int listOwner, SqlConnection cn, SqlTransaction tran)
        {
            var window = new FindOtherLists(cn, tran, listOwner);
            var res = window.ShowDialog(owner);
            if (res == DialogResult.OK)
                return (ListTopo)window.comboBox1.SelectedItem;
            else
                return null;
        }

    }

    public class ListTopo
    {
        public int Iid { get; set; }
        public String Tag { get; set; }

        public override string ToString()
        {
            return (Tag ?? String.Empty);
        }
    }
}
