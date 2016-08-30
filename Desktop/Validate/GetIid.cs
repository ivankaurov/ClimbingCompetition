// <copyright file="GetIid.cs">
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
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class GetIid : Form
    {
        private int n;
        private GetIid()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out n))
                this.Close();
        }

        public static int GetInt()
        {
            GetIid gtId = new GetIid();
            gtId.pictureBox.Visible = gtId.btnShowPhoto.Visible = false;
            gtId.ShowDialog();
            return gtId.n;
        }

        private SqlConnection cn = null;

        public GetIid(SqlConnection baseCn)
            : this()
        {
            cn = new SqlConnection(baseCn.ConnectionString);
            cn.Open();
        }

        private void GetIid_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (cn != null)
                    if (cn.State != ConnectionState.Closed)
                        cn.Close();
            }
            catch { }
        }

        private void btnShowPhoto_Click(object sender, EventArgs e)
        {
            int iid;
            if (!int.TryParse(textBox1.Text, out iid))
                return;
            bool judge = (sender == btnShowPhoto);
                
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT remoteString FROM CompetitionData(NOLOCK)";
            string remS = cmd.ExecuteScalar().ToString();
            if (remS.Length < 1)
                return;
            SqlConnection remote = new SqlConnection(remS);
            try
            {
                string path = ImageWorker.GetOnlineImg(remote, iid,judge);
                if (path.Length < 1)
                {
                    pictureBox.Image = null;
                    return;
                }
                Image img = Image.FromFile(System.Threading.Thread.GetDomain().BaseDirectory+ path);
                pictureBox.Image = img;
            }
            finally { remote.Close(); }
        }
        
    }
}