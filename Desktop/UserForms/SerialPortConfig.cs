// <copyright file="SerialPortConfig.cs">
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
using System.IO.Ports;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки СОМ порта для подключения скоростной системы
    /// </summary>
    public partial class SerialPortConfig : Form
    {
        private string portName;
        private SerialPort sp = null;
        private SerialPortConfig(string portName)
        {
            InitializeComponent();
            this.Text = "Настройка порта " + portName;
            this.portName = portName;
            cmbBaudRate.SelectedIndex = appSettings.Default.cmbBaudRate;
            cmbDataBits.SelectedIndex = appSettings.Default.cmbDataBits;
            cmbParity.SelectedIndex = appSettings.Default.cmbParity;
            cmbStopBits.SelectedIndex = appSettings.Default.cmbStopBits;
            cmbHandShake.SelectedIndex = appSettings.Default.cmdHandShake;
            cbShowInfo.Checked = appSettings.Default.cbShowInfo;
            tbAutoClose.Text = appSettings.Default.AutoCloseTimer.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sp = new SerialPort(portName);
            int tmp;
            if (cmbBaudRate.SelectedIndex > 0)
            {
                if (int.TryParse(cmbBaudRate.Text, out tmp))
                    sp.BaudRate = tmp;
                else
                {
                    MessageBox.Show("Скорость введена не верно");
                    return;
                }
            }
            if (cmbDataBits.SelectedIndex > 0)
            {
                if (int.TryParse(cmbDataBits.Text, out tmp))
                    sp.DataBits = tmp;
                else
                {
                    MessageBox.Show("Биты данных введены не верно");
                    return;
                }
            }
            if (cmbHandShake.SelectedIndex > 0)
            {
                try
                {
                    Handshake hsh = (Handshake)Enum.Parse(typeof(Handshake), cmbHandShake.Text, true);
                    sp.Handshake = hsh;
                }
                catch
                {
                    MessageBox.Show("Handshake протокол выбран неправильно;");
                    return;
                }
            }
            if (cmbStopBits.SelectedIndex > 0)
            {
                StopBits sBits;
                switch (cmbStopBits.Text)
                {
                    case "(нет)":
                        sBits = StopBits.None;
                        break;
                    case "1":
                        sBits = StopBits.One;
                        break;
                    case "1.5":
                        sBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        sBits = StopBits.Two;
                        break;
                    default:
                        MessageBox.Show("Стоповые биты введены неправильно");
                        return;
                }
                sp.StopBits = sBits;
            }
            if (cmbParity.SelectedIndex > 0)
            {
                Parity p;
                switch (cmbParity.Text)
                {
                    case "(нет)":
                        p = Parity.None;
                        break;
                    case "Чёт":
                        p = Parity.Even;
                        break;
                    case "Нечёт":
                        p = Parity.Odd;
                        break;
                    case "Маркер":
                        p = Parity.Mark;
                        break;
                    case "Пробел":
                        p = Parity.Space;
                        break;
                    default:
                        MessageBox.Show("Чётность введена неправильно");
                        return;
                }
                sp.Parity = p;
            }
            int autoClose;
            if (!int.TryParse(tbAutoClose.Text, out autoClose))
            {
                MessageBox.Show("Время закрытия окна введено неверно");
                return;
            }
            if (autoClose < 0 || (autoClose > 0 && autoClose < 5))
            {
                MessageBox.Show("Время закрытия окна должно быть более 4 секунд");
                return;
            }
            appSettings aset = appSettings.Default;
            aset.cmbBaudRate = cmbBaudRate.SelectedIndex;
            aset.cmbDataBits = cmbDataBits.SelectedIndex;
            aset.cmbParity = cmbParity.SelectedIndex;
            aset.cmbStopBits = cmbStopBits.SelectedIndex;
            aset.cmdHandShake = cmbHandShake.SelectedIndex;
            aset.systemPort = sp;
            aset.cbShowInfo = cbShowInfo.Checked;
            aset.AutoCloseTimer = autoClose;
            aset.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            sp = null;
            this.Close();
        }

        public static SerialPort ConfigurePort(string portName, IWin32Window owner)
        {
            SerialPortConfig spc = new SerialPortConfig(portName);
            spc.ShowDialog(owner);
            return spc.sp;
        }
    }
}
