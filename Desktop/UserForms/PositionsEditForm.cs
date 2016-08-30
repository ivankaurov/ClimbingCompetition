// <copyright file="PositionsEditForm.cs">
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
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для редактирования справочника судейских должностей
    /// </summary>
    public partial class PositionsEditForm : BaseForm
    {
        public PositionsEditForm(SqlConnection baseCn, string competitionTitle):
            base(baseCn, competitionTitle)
        {
            InitializeComponent();
            this.positionsTableAdapter.Connection = cn;
        }

        private void PositionsEditForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsClimbing.positions' table. You can move, or remove it, as needed.
            this.positionsTableAdapter.Fill(this.dsClimbing.positions);
            // TODO: This line of code loads data into the 'dsClimbing.positions' table. You can move, or remove it, as needed.
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                positionsBindingSource.EndEdit();
                positionsTableAdapter.Update(dsClimbing);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка обновления списка должностей:\r\n" + ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgPosView.SelectedRows.Count < 1)
                return;
            if (MessageBox.Show(this, "Вы уверены, что хотите удалить выбранную должность?",
                "Удалить должность", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;
            try
            {
                positionsBindingSource.RemoveCurrent();
                positionsBindingSource.EndEdit();
                positionsTableAdapter.Update(dsClimbing);
                //positionsBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка удаления должностей:\r\n" + ex.Message);
            }
        }
    }
}
