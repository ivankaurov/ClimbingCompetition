// <copyright file="ONLCompGrid.cs">
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClimbingCompetition.Online;
using System.IO;

namespace ClimbingCompetition
{
    public partial class ONLCompGrid : UserControl
    {
        public ClimbingService Service { get; set; }

        public long compID { get; set; }

        public ONLCompGrid()
        {
            InitializeComponent();
#if DEBUG
            paramID.Visible = true;
#endif
        }

        private Dictionary<string, string> LoadParams()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var paramVals = Service.GetAllBinaryParamsStringParamValues(compID);

            foreach (var p in paramVals)
                res.Add(p.Value1, p.Value2);
            return res;
        }

        public void LoadData()
        {
            var v = LoadParams();
            SetLoadedData(v);
        }

        private const string EmptyValue = "<Пусто>";

        public class BinaryValue
        {
            public byte[] binaryValue = null;
            public bool changed = false;
            public string fileName = String.Empty;
            public string firendlyName = String.Empty;
            public string linkText = String.Empty;
        }
        private Dictionary<string, BinaryValue> values = new Dictionary<string, BinaryValue>();

        bool rowEventAllowed = true;
        private void SetLoadedData(Dictionary<string, string> data)
        {
            rowEventAllowed = false;
            try
            {
                gvParams.Rows.Clear();
                values.Clear();
                foreach (var v in data)
                {
                    string paramID = v.Key;
                    string friendlyName;
                    switch (paramID)
                    {
                        case Constants.PDB_COMP_BIN_ACCOMODATION:
                            friendlyName = "Информация по проживанию";
                            break;
                        case Constants.PDB_COMP_BIN_POLOGENIE:
                            friendlyName = "Положение";
                            break;
                        case Constants.PDB_COMP_BIN_POLOGENIE_TITUL:
                            friendlyName = "Титул положения";
                            break;
                        case Constants.PDB_COMP_BIN_REGLAMENT:
                            friendlyName = "Регламент";
                            break;
                        case Constants.PDB_COMP_BIN_REGLAMENT_TITUL:
                            friendlyName = "Титул регламента";
                            break;
                        case Constants.PDB_COMP_BIN_RASP:
                            friendlyName = "Расписание соревнований";
                            break;
                        case Constants.PDB_COMP_BIN_RESULTS:
                            friendlyName = "Результаты";
                            break;
                        case Constants.PDB_COMP_BIN_LOGO_LEFT:
                            friendlyName = "Логотип на сайте слева";
                            break;
                        case Constants.PDB_COMP_BIN_LOGO_RIGHT:
                            friendlyName = "Логотип на сайте справа";
                            break;
                        default:
                            if (paramID.IndexOf(Constants.PDB_COMP_SPONSORS) == 0)
                                friendlyName = "Лого спонсора";
                            else if (paramID.IndexOf(Constants.PDB_COMP_PARTNERS) == 0)
                                friendlyName = "Лого партнера ФСР";
                            else if (paramID.IndexOf(Constants.PDB_COMP_BIN_ADD_FILE) == 0)
                                friendlyName = "Доп.инфо";
                            else
                                continue;
                            break;
                    }
                    if (!values.ContainsKey(paramID))
                        values.Add(paramID, new BinaryValue());
                    BinaryValue bvSet = values[paramID];
                    bvSet.binaryValue = null;
                    bvSet.changed = false;
                    bvSet.fileName = v.Value;
                    bvSet.firendlyName = friendlyName;
                    values[paramID] = bvSet;
                    

                    string value = String.IsNullOrEmpty(v.Value) ? EmptyValue : v.Value;


                    gvParams.Rows.Add();
                    var addedRow = gvParams.Rows[gvParams.Rows.Count - 2];
                    addedRow.Cells[0].Value = paramID;
                    addedRow.Cells[1].Value = friendlyName;
                    addedRow.Cells[2].Value = value;
                }
            }
            finally { rowEventAllowed = true; }
        }

        private void gvParams_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!rowEventAllowed)
                return;
            try
            {
                string str = gvParams.Rows[e.RowIndex].Cells[FirendlyName.Name].Value as string;
                string paramIDS;
                string linkText = str;
                switch (str)
                {
                    case "Информация по проживанию":
                        paramIDS = Constants.PDB_COMP_BIN_ACCOMODATION;
                        break;
                    case "Положение":
                        paramIDS = Constants.PDB_COMP_BIN_POLOGENIE;
                        break;
                    case "Титул положения":
                        paramIDS = Constants.PDB_COMP_BIN_POLOGENIE_TITUL;
                        break;
                    case "Регламент":
                        paramIDS = Constants.PDB_COMP_BIN_REGLAMENT;
                        break;
                    case "Титул регламента":
                        paramIDS = Constants.PDB_COMP_BIN_REGLAMENT_TITUL;
                        break;
                    case "Расписание соревнований":
                        paramIDS = Constants.PDB_COMP_BIN_RASP;
                        break;
                    case "Лого спонсора":
                        paramIDS = Constants.PDB_COMP_SPONSORS;
                        break;
                    case "Лого партнера ФСР":
                        paramIDS = Constants.PDB_COMP_PARTNERS;
                        break;
                    case "Результаты":
                        paramIDS = Constants.PDB_COMP_BIN_RESULTS;
                        break;
                    case "Логотип на сайте слева":
                        paramIDS = Constants.PDB_COMP_BIN_LOGO_LEFT;
                        linkText = gvParams.Rows[e.RowIndex].Cells[this.linkText.Name].Value as string;
                        if (linkText == null)
                            linkText = String.Empty;
                        break;
                    case "Логотип на сайте справа":
                        paramIDS = Constants.PDB_COMP_BIN_LOGO_RIGHT;
                        linkText = gvParams.Rows[e.RowIndex].Cells[this.linkText.Name].Value as string;
                        if (linkText == null)
                            linkText = String.Empty;
                        break;
                    case "Доп.инфо":
                        paramIDS = Constants.PDB_COMP_BIN_ADD_FILE;
                        string sTmp = gvParams.Rows[e.RowIndex].Cells[this.linkText.Name].Value as string;
                        if (!String.IsNullOrEmpty(sTmp))
                            linkText = sTmp;
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
                int curPI = 0;

                bool isLogo = paramIDS.Equals(Constants.PDB_COMP_SPONSORS) || paramIDS.Equals(Constants.PDB_COMP_PARTNERS)
                              || paramIDS.Equals(Constants.PDB_COMP_BIN_ADD_FILE);
                string checkPID = paramIDS;

                for (int i = 0; i < gvParams.Rows.Count - 1; i++)
                {
                    if (i != e.RowIndex && checkPID.Equals(gvParams.Rows[i].Cells[paramID.Name].Value as string))
                    {
                        if (!isLogo)
                        {
                            e.Cancel = true;
                            MessageBox.Show("Такой параметр уже есть");
                            return;
                        }
                        else
                            checkPID = paramIDS + "_" + (++curPI).ToString();
                    }
                }
                if (!values.ContainsKey(checkPID))
                {
                    BinaryValue bv = new BinaryValue();
                    bv.changed = true;
                    bv.firendlyName = str;
                    values.Add(checkPID, bv);
                }
                values[checkPID].linkText = linkText;
                if (!(paramIDS.Equals(Constants.PDB_COMP_BIN_ADD_FILE) || paramIDS.Equals(Constants.PDB_COMP_BIN_LOGO_LEFT)
                   || paramIDS.Equals(Constants.PDB_COMP_BIN_LOGO_RIGHT)))
                    linkText = String.Empty;
                gvParams.Rows[e.RowIndex].Cells[this.linkText.Name].Value = linkText;
                gvParams.Rows[e.RowIndex].Cells[paramID.Name].Value = checkPID;
            }
            catch (Exception)
            {
                e.Cancel = true;
            }
        }

        private void gvParams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.RowIndex >= gvParams.Rows.Count)
                    return;
                if (e.ColumnIndex < 0 || e.ColumnIndex >= gvParams.Columns.Count)
                    return;
                if (gvParams.Columns[e.ColumnIndex].Equals(setValue))
                    gvParams_SetValueButtonClicked(e.RowIndex);
                else if (gvParams.Columns[e.ColumnIndex].Equals(clearVal))
                    gvParams_ClearValueButtonClicked(e.RowIndex);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки:\r\n" + ex.Message); }
        }

        private void gvParams_SetValueButtonClicked(int rowIndex)
        {
            DataGridViewCellCancelEventArgs eArgs = new DataGridViewCellCancelEventArgs(0, rowIndex);
            gvParams_RowValidating(null, eArgs);
            if (eArgs.Cancel)
                return;
            var dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel || dr == DialogResult.No)
                return;

            string pID = gvParams.Rows[rowIndex].Cells[paramID.Name].Value as string;
            MemoryStream mstr = new MemoryStream();
            try
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                string fName = fi.Name;
                var stream = ofd.OpenFile();
                
                try
                {
                    byte[] buffer = new byte[102400];
                    int n;
                    while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
                        mstr.Write(buffer, 0, n);
                    if (!values.ContainsKey(pID))
                    {
                        values.Add(pID, new BinaryValue());
                        values[pID].firendlyName = gvParams.Rows[rowIndex].Cells[FirendlyName.Name].Value as string;
                    }
                    values[pID].binaryValue = mstr.ToArray();
                    values[pID].changed = true;
                    values[pID].fileName = fName;
                    
                    gvParams.Rows[rowIndex].Cells[ParamValue.Name].Value = fName;
                }
                finally { stream.Close(); }
            }
            finally { mstr.Close(); }
        }

        private void gvParams_ClearValueButtonClicked(int rowIndex)
        {
            DataGridViewCellCancelEventArgs eArgs = new DataGridViewCellCancelEventArgs(0, rowIndex);
            gvParams_RowValidating(null, eArgs);
            if (eArgs.Cancel)
                return;
            string pID = gvParams.Rows[rowIndex].Cells[paramID.Name].Value as string;
            if (!values.ContainsKey(pID))
            {
                values.Add(pID, new BinaryValue());
                values[pID].firendlyName = gvParams.Rows[rowIndex].Cells[FirendlyName.Name].Value as string;
            }
            values[pID].changed = true;
            values[pID].binaryValue = null;
            values[pID].fileName = String.Empty;
            gvParams.Rows[rowIndex].Cells[ParamValue.Name].Value = EmptyValue;
        }

        public Dictionary<string, BinaryValue> GetLoadedValues()
        {
            Dictionary<string, BinaryValue> retVals = new Dictionary<string, BinaryValue>();
            foreach (var v in this.values)
                retVals.Add(v.Key, v.Value);
            List<string> toDelete = new List<string>();
            foreach (var v in retVals)
            {
                if (!v.Value.changed)
                {
                    toDelete.Add(v.Key);
                    continue;
                }
                bool exists = false;
                foreach (DataGridViewRow row in gvParams.Rows)
                    if (v.Key.Equals(row.Cells[paramID.Name].Value as string))
                    {
                        exists = true;
                        break;
                    }
                if (!exists)
                    toDelete.Add(v.Key);
            }

            foreach (var v in toDelete)
                retVals.Remove(v);
            return retVals;
        }
    }
}
