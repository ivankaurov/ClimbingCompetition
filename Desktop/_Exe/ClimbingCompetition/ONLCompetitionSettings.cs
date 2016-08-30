// <copyright file="ONLCompetitionSettings.cs">
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
using ClimbingCompetition.Online;
using System.Globalization;
using XmlApiClient;

namespace ClimbingCompetition
{
    public partial class ONLCompetitionSettings : ClimbingCompetition.BaseForm
    {
        long compID = -1;
        ClimbingService service = null;
        XmlClient client;
        bool cbVideoOld = false;

        public ONLCompetitionSettings(SqlConnection _cn, string _competitionTitle)
            : base(_cn, _competitionTitle, null)
        {
            InitializeComponent();
        }

        private void ONLCompetitionSettings_Load(object sender, EventArgs e)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT remoteString FROM CompetitionData(NOLOCK)";
                cmd.Connection = cn;
                XmlClientWrapper wrapper;
                long? compIDforService = StaticClass.ParseServString(cmd.ExecuteScalar() as string,out wrapper);
                if (wrapper != null)
                {
                    compIDforService = wrapper.CompetitionId;
                    client = wrapper.CreateClient(cn);
                }
                if (compIDforService == null || !compIDforService.HasValue)
                {
                    MessageBox.Show("Не настроен доступ к веб-службам или не выбрано соревнование");
                    this.Close();
                    return;
                }
                compID = compIDforService.Value;
                service = new ClimbingService();

                paramsGrid.Service = service;
                paramsGrid.compID = compID;
                paramsGrid.LoadData();

                string str = service.GetParamValue(Constants.PDB_COMP_VIDEO, compID);
                str = (str == null) ? String.Empty : str.Trim().ToLower();
                cbVideoOld = str.Equals("true") || str.Equals("1");
                cbVideo.Checked = cbVideoOld;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось запустить форму настройки соревнований:\r\n" + ex.Message);
                this.Close();
                return;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string toSave = String.Empty;
            var dictToSave = paramsGrid.GetLoadedValues();
            if (cbVideo.Checked == cbVideoOld && (dictToSave == null || dictToSave.Count < 1))
            {
                MessageBox.Show(this, "Нет параметров для сохранения");
                return;
            }
            if (cbVideoOld != cbVideo.Checked)
                toSave = "Видео трансляция";
            foreach (var v in dictToSave)
            {
                if (!toSave.Equals(String.Empty))
                    toSave += ", ";
                toSave += v.Value.firendlyName;
            }

            if (MessageBox.Show(this, "Будут сохранены следующие параметры:\r\n" + toSave + "\r\nПродолжить?",
                String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;
            if (cbVideoOld != cbVideo.Checked)
                service.PostCompetitionParam(Constants.PDB_COMP_VIDEO, cbVideo.Checked.ToString(new CultureInfo("en-US")), compID);
            foreach (var v in dictToSave)
            {
#if !DEBUG
                try
                {
#endif
                bool isAddFile = v.Key.IndexOf(Constants.PDB_COMP_BIN_ADD_FILE) == 0 ||
                    v.Key.Equals(Constants.PDB_COMP_BIN_LOGO_LEFT) || v.Key.Equals(Constants.PDB_COMP_BIN_LOGO_RIGHT);
                if (v.Value.binaryValue == null || v.Value.binaryValue.Length < 1)
                    service.RemoveCompetitionParam(v.Key, compID);
                else
                {
                    service.PostCompetitionBinaryParam(v.Key, v.Value.fileName, v.Value.binaryValue, compID);
                    if (isAddFile)
                        service.PostCompetitionParam(v.Key + Constants.PDB_PARAM_ADD_INFO, v.Value.linkText, compID);
                }
#if !DEBUG
                }
                catch (Exception exI)
                {
                    var dr = MessageBox.Show(this, "Ошибка обновления параметра \"" + v.Value.firendlyName + "\":\r\n" +
                        exI.ToString() + "\r\n" +
                        "Продолжить?\r\n", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dr == System.Windows.Forms.DialogResult.No)
                        return;
                }
#endif
            }
            MessageBox.Show(this, "Параметры успешно обновлены.");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Выйти не сохраняя?",
               String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;
            this.Close();
        }
    }
}
