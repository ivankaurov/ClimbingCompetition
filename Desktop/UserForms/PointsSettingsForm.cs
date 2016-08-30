// <copyright file="PointsSettingsForm.cs">
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
    public partial class PointsSettingsForm : BaseForm
    {
        public PointsSettingsForm(SqlConnection baseCn, string compTitle) :
            base(baseCn, compTitle)
        {
            InitializeComponent();
            this.pointsTableAdapter.Connection = cn;
        }

        private void PointsSettingsForm_Load(object sender, EventArgs e)
        {
            if (!LoadData())
                this.Close();
        }

        private bool LoadData()
        {
            bool res;
            try
            {
                AccountForm.CheckPointsTable(cn, null);
                AccountForm.OrderPoints(cn, null);
                // TODO: This line of code loads data into the 'dsClimbing.Points' table. You can move, or remove it, as needed.
                this.pointsTableAdapter.Fill(this.dsClimbing.Points);
                SqlCommand cmd = new SqlCommand("SELECT pts FROM Points WHERE pos = 0", cn);
                object oTmp = cmd.ExecuteScalar();
                if (oTmp == null || oTmp == DBNull.Value)
                    tbLastPoints.Text = "0";
                else
                    tbLastPoints.Text = Convert.ToDouble(oTmp).ToString();
                res = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                res = false;
            }
            return res;
        }

        private static bool getDoubleFromString(string str, out double d)
        {
            d = -1.0;
            if (double.TryParse(str, out d))
                return true;
            if (double.TryParse(str.Replace('.', ','), out d))
                return true;
            if (double.TryParse(str.Replace(',', '.'), out d))
                return true;
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.pointsBindingSource.EndEdit();
                SqlTransaction tranOld = this.pointsTableAdapter.Transaction;
                this.pointsTableAdapter.Transaction = cn.BeginTransaction();
                bool transactionSuccess = false;
                try
                {
                    this.pointsTableAdapter.Update(dsClimbing);
                    AccountForm.OrderPoints(cn, this.pointsTableAdapter.Transaction);
                    double lastPts;
                    if (tbLastPoints.Text.Length < 1)
                        lastPts = -1.0;
                    else if (!getDoubleFromString(tbLastPoints.Text, out lastPts))
                    {
                        MessageBox.Show(this, "Очки последним участникам введены неверно");
                        return;
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.Transaction = this.pointsTableAdapter.Transaction;
                    cmd.CommandText = "DELETE FROM Points WHERE pos = 0";
                    cmd.ExecuteNonQuery();
                    if (lastPts > 0.0)
                    {
                        cmd.CommandText = "INSERT INTO Points(pos,pts) VALUES (0,@pts)";
                        cmd.Parameters.Add("@pts", SqlDbType.Float);
                        cmd.Parameters[0].Value = lastPts;
                        cmd.ExecuteNonQuery();
                    }
                    transactionSuccess = true;
                }
                finally
                {
                    if (transactionSuccess)
                    {
                        this.pointsTableAdapter.Transaction.Commit();
                        this.Close();
                    }
                    else
                        try { this.pointsTableAdapter.Transaction.Rollback(); }
                        catch { }
                    this.pointsTableAdapter.Transaction = tranOld;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка сохранения данных:\r\n" + ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотите отменить изменения?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
                this.Close();
        }

        private void btnLoadDefault_Click(object sender, EventArgs e)
        {
            try
            {
                AccountForm.LoadStandartPoints(cn, null);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки таблицы очков:\r\n" + ex.Message); }
        }
    }
}
