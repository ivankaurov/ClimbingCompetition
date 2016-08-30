// <copyright file="AddClimber.cs">
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� ��� ���������� ��������� � ��������� ��������
    /// </summary>
    public partial class AddClimber : BaseForm
    {
        //private SqlConnection cn;
        private int num;
        public AddClimber(SqlConnection baseCon)
            : base(baseCon,"")
        {
            InitializeComponent();
            //this.cn = cn;
            SqlCommand cmd = new SqlCommand("SELECT name FROM Teams ORDER BY name", this.cn);
            SqlDataReader rd = cmd.ExecuteReader();
            cbPartTeams.Items.Add("-��� �������-");
            while (rd.Read())
                cbPartTeams.Items.Add(rd[0].ToString());
            rd.Close();
            cmd.CommandText = "SELECT name FROM Groups ORDER BY name";
            rd = cmd.ExecuteReader();
            cbPartGroups.Items.Add("-��� ������-");
            while (rd.Read())
                cbPartGroups.Items.Add(rd[0].ToString());
            rd.Close();
        }

        public string SelectedGroup
        {
            get { return cbPartGroups.SelectedItem.ToString(); }
            set { cbPartGroups.SelectedIndex = cbPartGroups.Items.IndexOf(value); }
        }

        public int Number
        {
            get
            {
                return num;
            }
        }

        private void SetDataGrid()
        {
            if (cbPartGroups.SelectedIndex < 0)
                cbPartGroups.SelectedIndex = 0;
            if (cbPartTeams.SelectedIndex < 0)
                cbPartTeams.SelectedIndex = 0;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand();
            da.SelectCommand.Connection = cn;
            if (cbPartGroups.SelectedIndex == 0)
            {
                if (cbPartTeams.SelectedIndex == 0)
                    da.SelectCommand.CommandText = "SELECT p.iid AS �����, p.surname AS �������, p.name " +
                        "AS ���, t.name AS �������, p.age AS [�.�.], p.Qf AS ������, g.name AS ������ FROM " +
                        "Participants p INNER JOIN Teams t ON p.team_id = t.iid INNER JOIN Groups g " +
                        "ON p.group_id = g.iid";
                else
                {
                    da.SelectCommand.CommandText = "SELECT p.iid AS �����, p.surname AS �������, p.name " +
                        "AS ���, t.name AS �������, p.age AS [�.�.], p.Qf AS ������, g.name AS ������ FROM " +
                        "Participants p INNER JOIN Teams t ON p.team_id = t.iid INNER JOIN Groups g " +
                        "ON p.group_id = g.iid WHERE t.name = @name";
                    da.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar, 50);
                    da.SelectCommand.Parameters[0].Value = cbPartTeams.SelectedItem.ToString();
                }
            }
            else
            {
                if (cbPartTeams.SelectedIndex == 0)
                {
                    da.SelectCommand.CommandText = "SELECT p.iid AS �����, p.surname AS �������, p.name " +
                        "AS ���, t.name AS �������, p.age AS [�.�.], p.Qf AS ������, g.name AS ������ FROM " +
                        "Participants p INNER JOIN Teams t ON p.team_id = t.iid INNER JOIN Groups g " +
                        "ON p.group_id = g.iid WHERE g.name = @name";
                    da.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar, 50);
                    da.SelectCommand.Parameters[0].Value = cbPartGroups.SelectedItem.ToString();
                }
                else
                {
                    da.SelectCommand.CommandText = "SELECT p.iid AS �����, p.surname AS �������, p.name " +
                        "AS ���, t.name AS �������, p.age AS [�.�.], p.Qf AS ������, g.name AS ������ FROM " +
                        "Participants p INNER JOIN Teams t ON p.team_id = t.iid INNER JOIN Groups g " +
                        "ON p.group_id = g.iid WHERE (g.name = @gname) AND (t.name = @tname)";
                    da.SelectCommand.Parameters.Add("@gname", SqlDbType.VarChar, 50);
                    da.SelectCommand.Parameters[0].Value = cbPartGroups.SelectedItem.ToString();
                    da.SelectCommand.Parameters.Add("@tname", SqlDbType.VarChar, 50);
                    da.SelectCommand.Parameters[1].Value = cbPartTeams.SelectedItem.ToString();
                }
            }

            da.SelectCommand.CommandText += " ORDER BY t.name, g.genderFemale, g.oldYear, p.surname, p.name";
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                dg.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            dg.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        private void cbPartGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDataGrid();
        }

        private void cbPartTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDataGrid();
        }

        private void dg_SelectionChanged(object sender, EventArgs e)
        {
            int iid;
            try { iid = Convert.ToInt32(dg.CurrentRow.Cells[0].Value); }
            catch { return; }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT p.*, t.name AS tname, g.name AS gname FROM Participants p " +
                "INNER JOIN Teams t ON p.team_id=t.iid INNER JOIN Groups g ON p.group_id=g.iid " +
                "WHERE p.iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters[0].Value = iid;
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                tbPartNumber.Text = r["iid"].ToString();
                if (r["Age"].ToString() == "0")
                    tbPartAge.Text = "";
                else
                    tbPartAge.Text = r["Age"].ToString();
                tbPartGroup.Text = r["gname"].ToString();
                tbPartName.Text = r["name"].ToString();
                tbPartSurname.Text = r["surname"].ToString();
                tbPartTeam.Text = r["tname"].ToString();
                bPartLateAppl.Checked = Convert.ToBoolean(r["lateAppl"]);
                bPartNoPoints.Checked = Convert.ToBoolean(r["noPoints"]);
                bPartVk.Checked = Convert.ToBoolean(r["vk"]);
                string qq = r["Qf"].ToString();
                cbPartQf.SelectedIndex = cbPartQf.Items.IndexOf(qq);
                if (Convert.ToBoolean(r["genderFemale"]))
                    cbPartSex.SelectedIndex = cbPartSex.Items.IndexOf("�");
                else
                    cbPartSex.SelectedIndex = cbPartSex.Items.IndexOf("�");
                tbPartBoulder.Text = r["rankingBoulder"].ToString();
                tbPartLead.Text = r["rankingLead"].ToString();
                tbPartSpeed.Text = r["rankingSpeed"].ToString();
                break;
            }
            r.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                num = Convert.ToInt32(tbPartNumber.Text);
                this.Close();
            }
            catch
            { MessageBox.Show("�������� �� ������"); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            num = 0;
            this.Close();
        }
    }
}