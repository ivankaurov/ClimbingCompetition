// <copyright file="ChangePassword.aspx.cs">
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ClimbingCompetition;
using WebClimbing.src;

namespace WebClimbing.Apps
{
    public partial class _PChangePassword : System.Web.UI.Page
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if(!Page.IsValid)
                return;
            if (!PasswordWorkingClass.CheckStrength(newPwd.Text))
            {
                ErrorMessage.Text = "Пароль не соответствует требованиям безопасности (не менее 8 символов, не менее 1 строчной буквы, прописной буквы и цифры)";
                return;
            }
            try
            {
                string userId = User.Identity.Name;
                MembershipUser mUsr = Membership.GetUser(userId);
                if (!(mUsr is ClmUser))
                {
                    ErrorMessage.Text = "Ошибка определения пользователя";
                    return;
                }
                ClmUser usr = (ClmUser)mUsr;
                if (usr.ChangePassword(oldPwd.Text, newPwd.Text))
                    ErrorMessage.Text = "Пароль изменён успешно";
                else
                    ErrorMessage.Text = "Ошибка смены пароля";
            }
            catch { ErrorMessage.Text = "Ошибка смены пароля"; }
        }
    }
}
