// <copyright file="TableDataChange.cs">
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
    public partial class TableDataChange : Form
    {

        private static string GetParticipantOrdName(int iid, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT surname, name, genderFemale" +
                              "  FROM Participants(NOLOCK)" +
                              " WHERE iid = " + iid.ToString();
            string res = String.Empty;
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    string surname = rdr["surname"].ToString().Trim();
                    string name = rdr["name"].ToString().Trim();
                    bool female = Convert.ToInt32(rdr["genderFemale"]) > 0;
                    if (!String.IsNullOrEmpty(name))
                        name = name.Substring(0, 1) + ".";
                    if (surname.Length >= 3)
                    {
                        if (female)
                        {
                            switch (surname.Substring(surname.Length - 2, 2))
                            {
                                case "ва":
                                    surname = surname.Substring(0, surname.Length - 1) + "ой";
                                    break;
                                case "ая":
                                    surname = surname.Substring(0, surname.Length - 2) + "ой";
                                    break;
                                default:
                                    if (surname.Substring(surname.Length - 3, 3).Equals("ина"))
                                        surname = surname.Substring(0, surname.Length - 1) + "ой";
                                    break;
                            }                                
                        }
                        else
                        {
                            if (surname.Substring(surname.Length - 1, 1).Equals("в"))
                                surname = surname + "а";
                            else if (surname.Substring(surname.Length - 1, 1).Equals("й"))
                                surname = surname.Substring(0, surname.Length - 2) + "ого";
                            else if (surname.Substring(surname.Length - 2, 2).Equals("ин"))
                                surname = surname + "а";
                        }
                    }
                    res = (surname + " " + name).Trim();
                }
            }
            return res;
        }

        public static bool FillAdditionalTeams(int climberID, SqlConnection cn, SqlTransaction _tran = null, IWin32Window owner = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran == null ? cn.BeginTransaction() : _tran;
            bool scs = false;
            try
            {
                cmd.CommandText = "SELECT p.surname + ' ' + p.name FROM Participants P(nolock) WHERE P.iid = @climberID";
                cmd.Parameters.Add("@climberID", SqlDbType.Int);
                cmd.Parameters[0].Value = climberID;
                string clmName = cmd.ExecuteScalar() as string;
                if (clmName == null)
                    clmName = String.Empty;
                else
                    clmName = clmName.Trim().ToUpper();
                cmd.CommandText = "SELECT T.iid, T.name, CONVERT(BIT," +
                                  "       CASE WHEN TL.iid IS NULL THEN 0 ELSE 1 END) has_value" +
                                  "  FROM Teams T(nolock)" +
                              " LEFT JOIN teamsLink TL(nolock) on TL.team_id = T.iid" +
                                  "                           and TL.climber_id = @climberID" +
                                  " WHERE T.iid <> (SELECT P.team_id" +
                                  "                   FROM Participants P(nolock)" +
                                  "                  WHERE P.iid = @climberID)" +
                               " ORDER BY T.name";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                TableDataChange dcTable = new TableDataChange(clmName + ": дополнительные команды",
                    dt, "Команда", "Использовать");
                DialogResult dr;
                if (owner == null)
                    dr = dcTable.ShowDialog();
                else
                    dr = dcTable.ShowDialog(owner);
                if (dr == DialogResult.No || dr == DialogResult.Cancel || !dcTable.IsOK)
                    return false;
                string cmdYes = @"IF NOT EXISTS(SELECT 1
                                                  FROM teamsLink(NOLOCK)
                                                 WHERE team_id = @teamID
                                                   AND climber_id = @climberID) BEGIN
                                    INSERT INTO teamsLink(team_id, climber_id) VALUES (@teamID, @climberID)
                                  END;";
                string cmdNo = "DELETE FROM teamsLink WHERE team_id = @teamID AND climber_id = @climberID";
                cmd.Parameters.Add("@teamID", SqlDbType.Int);
                foreach (DataRow drr in dcTable.Table.Rows)
                {
                    cmd.Parameters[1].Value = drr["iid"];
                    cmd.CommandText = Convert.ToBoolean(drr["has_value"]) ? cmdYes : cmdNo;
                    cmd.ExecuteNonQuery();
                }
                scs = true;
            }
            finally
            {
                if (_tran == null)
                {
                    if (scs)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                }
            }
            return scs;
        }

        public static bool FillClimbersRP(int team, SqlConnection cn, SqlTransaction _tran = null, IWin32Window owner = null, string sClimbersWhere = "")
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran == null ? cn.BeginTransaction() : _tran;
            bool scs = false;
            try
            {
                cmd.CommandText = "SELECT iid, LTRIM(RTRIM(P.surname + ' ' + P.name)) climber_name," +
                                  "       LTRIM(RTRIM(P.name_ord)) name_ord" +
                                  "  FROM Participants P(NOLOCK)" +
                                  " WHERE P.team_id = " + team.ToString() + sClimbersWhere +
                               " ORDER BY climber_name";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow drr in dt.Rows)
                {
                    if(String.IsNullOrEmpty(drr["name_ord"].ToString()))
                        drr["name_ord"] = GetParticipantOrdName(Convert.ToInt32(drr["iid"]), cn, cmd.Transaction);
                }
                TableDataChange dcTable = new TableDataChange("Проверьте ФИО участников в родительном падеже",
                    dt, "Фамилия, Имя", "ФИО в род.падеже");
                DialogResult dr;
                if (owner == null)
                    dr = dcTable.ShowDialog();
                else
                    dr = dcTable.ShowDialog(owner);
                if (dr == DialogResult.No || dr == DialogResult.Cancel || !dcTable.IsOK)
                    return false;
                cmd.CommandText = "UPDATE Participants SET name_ord = @oname WHERE iid = @iid";
                cmd.Parameters.Add("@oname", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                foreach (DataRow row in dcTable.Table.Rows)
                {
                    cmd.Parameters[0].Value = row["name_ord"];
                    cmd.Parameters[1].Value = row["iid"];
                    cmd.ExecuteNonQuery();
                }
                scs = true;
            }
            finally
            {
                if (_tran == null)
                {
                    if (scs)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                }
            }
            return scs;
        }

        public static int[] CreateClimbersListForOrders(int team, SqlConnection cn, SqlTransaction _tran = null, IWin32Window owner = null, int[] initSrc = null)
        {
            List<int> chk = new List<int>();
            if (initSrc != null)
                foreach (var n in initSrc)
                    chk.Add(n);
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            bool scs = false;
            cmd.Transaction = _tran == null ? cn.BeginTransaction() : _tran;
            try
            {
                bool needLoop;
                DataTable dcTRES;
                do
                {
                    needLoop = false;
                    cmd.CommandText = "SELECT P.iid, P.surname +' ' + P.name clm_name," +
                                      "       CONVERT(BIT,0) has_order," +
                                      "       O.order_number" +
                                      "  FROM Participants P(nolock)" +
                                  " LEFT JOIN orders O(nolock) ON O.climber_id = P.iid" +
                                      " WHERE P.team_id = " + team.ToString() +
                                   " ORDER BY O.order_number, P.surname, P.name";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                        if (chk.Contains(Convert.ToInt32(dr["iid"])))
                            dr["has_order"] = true;
                    TableDataChange dcT = new TableDataChange("Настройка ордеров", dt, "Фамилия, Имя", "Печать", "Номер ордера", true);
                    dcT.btnDelSelectedOrders.Visible = true;
                    DialogResult dgRes;
                    if (owner == null)
                        dgRes = dcT.ShowDialog();
                    else
                        dgRes = dcT.ShowDialog(owner);
                    dcTRES = dcT.Table;
                    if (dgRes == DialogResult.Cancel || dgRes == DialogResult.No)
                        return null;
                    if (dgRes == DialogResult.Abort) //Удаление ордеров
                    {
                        DropSelectedOrders(cmd, dcTRES);
                        needLoop = true;
                    }
                } while (needLoop);
                DropSelectedOrders(cmd, dcTRES);
                List<int> resList = new List<int>();
                foreach(DataRow dr in dcTRES.Rows)
                    if (Convert.ToBoolean(dr["has_order"]))
                        resList.Add(Convert.ToInt32(dr["iid"]));
                scs = true;
                return resList.ToArray();
            }
            finally
            {
                if (_tran == null)
                    if (scs)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
            }
        }

        private static void DropSelectedOrders(SqlCommand cmd, DataTable dcT)
        {
            cmd.CommandText = String.Empty;
            foreach (DataRow dr in dcT.Rows)
            {
                if (Convert.ToBoolean(dr["has_order"]) &&
                    dr["order_number"] != null && dr["order_number"] != DBNull.Value)
                {
                    if (String.IsNullOrEmpty(cmd.CommandText))
                        cmd.CommandText = "DELETE FROM orders WHERE order_number IN(";
                    else
                        cmd.CommandText += ",";
                    cmd.CommandText += dr["order_number"].ToString();
                }
            }
            if (!String.IsNullOrEmpty(cmd.CommandText))
            {
                cmd.CommandText += ")";
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable dt;
        public DataTable Table { get { return dt; } }

        public TableDataChange(string header, DataTable dt, string col1H, string col2H, string col3H = null, bool col3ReadOnly = true)
        {
            int colCnt = String.IsNullOrEmpty(col3H) ? 3 : 4;
            if (dt.Columns.Count != colCnt || !dt.Columns[0].ColumnName.Equals("iid"))
                throw new ArgumentException("Input table has incorrect format");
            this.dt = dt;
            InitializeComponent();

            dg.DataSource = this.dt;
            dg.Columns[0].Visible = false;
            dg.Columns[1].HeaderText = col1H;
            dg.Columns[1].ReadOnly = true;
            dg.Columns[2].HeaderText = col2H;
            if (!String.IsNullOrEmpty(col3H))
            {
                dg.Columns[3].HeaderText = col3H;
                dg.Columns[3].ReadOnly = col3ReadOnly;
            }

            this.Text = header;
        }

        bool hasChanges = false;
        private void dg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!hasChanges)
                hasChanges = true;
        }

        bool OKPressed = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!OKPressed && hasChanges)
            {
                if (MessageBox.Show(this, "Отменить изменения?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OKPressed = false;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public bool IsOK { get { return OKPressed; } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OKPressed = true;
            try { dg.EndEdit(); }
            catch { }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnDelSelectedOrders_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Удалить ордера?", "Удаление ордеров", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                == System.Windows.Forms.DialogResult.No)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            hasChanges = false;
            this.Close();
        }
    }
}
