// <copyright file="SetAdminPassword.cs">
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

namespace ClimbingCompetition
{
    public sealed partial class SetAdminPassword : Form
    {
        private string oldPwd;
        private bool oldExists;
        private bool cancel;
        private IWin32Window owner;
        private SetAdminPassword(string oldPwd, bool oldExists, IWin32Window owner)
        {
            InitializeComponent();
            this.oldExists = oldExists;
            this.oldPwd = oldPwd;
            if (this.oldExists)
                this.Text = "Смена пароля администратора";
            else
                this.Text = "Введите пароль администратора";
            this.lblOld.Visible = this.lblOld.Enabled =
                this.tbOld.Visible = this.tbOld.Enabled = oldExists;
            this.cancel = false;
            this.owner = owner;
        }

        

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(oldExists)
                if (tbOld.Text != oldPwd)
                {
                    MessageBox.Show(owner, "Старый пароль введён неправильно.");
                    return;
                }
            if (tbConf.Text != tbNew.Text)
            {
                MessageBox.Show(owner, "Пароли не совпадают.");
                return;
            }
            if (!PasswordWorkingClass.CheckStrength(tbNew.Text))
            {
                MessageBox.Show(owner, "Пароль не соответствует требованиям безопасности.\r\n" +
                    "Длина пароля должна быть не менее 8 символов,\r\n" +
                    "пароль должен содержать не менее одной заглавной буквы, " +
                    "не менее одной строчной буквы и не менее одной цифры.");
                return;
            }
            this.cancel = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cancel = true;
            this.Close();
        }

        private static bool CreateNewPassword(string old, bool oldExists, IWin32Window owner, out string newPassword)
        {
            SetAdminPassword sap = new SetAdminPassword(old, oldExists, owner);
            sap.ShowDialog(owner);
            bool res = !sap.cancel;
            if (res)
                newPassword = sap.tbNew.Text;
            else
                newPassword = "";
            return res;
        }

        public static bool ChangePassword(string oldPassword, IWin32Window owner, out string newPassword)
        {
            return CreateNewPassword(oldPassword, true, owner, out newPassword);
        }

        public static bool CreateNewPassword(IWin32Window owner, out string newPassword)
        {
            return CreateNewPassword("", false, owner, out newPassword);
        }
    }
}
