// <copyright file="IvanovoSet.cs">
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
using System.Net;

namespace ClimbingCompetition
{
    public partial class IvanovoSet : Form
    {
        private int port;
        private IPAddress address;
        private bool cancel;

        public int Port { get { return port; } }
        public IPAddress Address { get { return address; } }
        public bool Cancel { get { return cancel; } }

        public IvanovoSet()
        {
            InitializeComponent();
            cancel = false;
            tbIP.ValidatingType = typeof(System.Net.IPAddress);
        }

        private void IvanovoSet_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void tbIP_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (!e.IsValidInput)
            {
                ipTbToolTip.ToolTipTitle = "Неверный IP";
                ipTbToolTip.Show("Неверный ввод IP адреса системы", tbIP, 0, -20, 5000);
            }
        }
        private int autoClose = -1;
        private bool ValidateData()
        {
            if (!int.TryParse(tbPort.Text, out port))
                return false;
            try { address = (IPAddress)tbIP.ValidateText(); }
            catch { return false; }

            if (!int.TryParse(tbAutoClose.Text, out autoClose))
                autoClose = -1;

            if (autoClose < 0 || (autoClose > 0 && autoClose < 5))
            {
                MessageBox.Show("Время закрытия окна должно быть более 4 секунд");
                return false;
            }

            return (address != null && port > 0);
        }

        private void LoadData()
        {
            appSettings aSet = appSettings.Default;
            tbIP.Text = aSet.IvanovoSystemIP;
            tbPort.Text = aSet.IvanovoSystemPort.ToString();
            tbAutoClose.Text = aSet.AutoCloseTimer.ToString();
            autoClose = aSet.AutoCloseTimer;
            cbShowInfo.Checked = aSet.cbShowInfo;
        }

        private void SaveData()
        {
            appSettings aSet = appSettings.Default;
            aSet.IvanovoSystemIP = address.ToString();
            aSet.IvanovoSystemPort = port;
            aSet.AutoCloseTimer = autoClose;
            aSet.cbShowInfo = cbShowInfo.Checked;
            aSet.Save();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
            {
                ipTbToolTip.ToolTipTitle = "Неверный ввод данных";
                ipTbToolTip.Show("Неверный ввод данных", tbIP, 0, -20, 5000);
                return;
            }
            SaveData();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
        }
    }
}
