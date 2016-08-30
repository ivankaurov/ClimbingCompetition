// <copyright file="NewUser.aspx.cs">
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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing.Apps.UserAccountManagement
{
    public partial class _PNewUser : BasePageRestrictedAdminComp
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }

        private void SetAdminButtonsVisible()
        {
            rbListCompProperties.Items.Clear();
            rbListAdminProperties.Items.Clear();
            rbListAdminProperties.Visible = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
            rbListCompProperties.Visible = (CurrentCompetition != null && User.IsInRole(Constants.ROLE_ADMIN, CurrentCompetition.iid));

            lblSetAdminRoles.Visible = rbListAdminProperties.Visible;
            lblSetCompRoles.Visible = rbListCompProperties.Visible;

            if (rbListAdminProperties.Visible)
            {
                rbListAdminProperties.Items.Add(new ListItem("Администратор Базы Данных", Constants.ROLE_ADMIN_ROOT));
                rbListAdminProperties.Items.Add(new ListItem("Пользователь Базы Данных", String.Empty));
            }
            if (rbListCompProperties.Visible)
            {
                rbListCompProperties.Items.Add(new ListItem("Администратор соревнований \"" + CurrentCompetition.short_name + "\"",
                    Constants.ROLE_ADMIN));
                rbListCompProperties.Items.Add(new ListItem("Пользователь (тренер) для \"" + CurrentCompetition.short_name + "\"",
                    Constants.ROLE_USER));
                rbListCompProperties.Items.Add(new ListItem("Нет доступа в соревнования \"" + CurrentCompetition.short_name + "\"",
                    String.Empty));
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
            base.Page_Load(sender, e);
            if (Page.IsPostBack)
                return;

            SetAdminButtonsVisible();

            cbRegSelect.Items.Clear();
            cbRegSelect.Items.Add(new ListItem(String.Empty, String.Empty));

            List<ONLteam> teamList = new List<ONLteam>();
            foreach (var t in dc.ONLteams)
            {
                if (compID <= 0 || (t.ONLTeamsCompLinks.Count(lnk => lnk.comp_id == compID) > 0))
                    teamList.Add(t);
            }
            teamList.OrderBy(tl => tl.name);

            foreach (var t in teamList)
                cbRegSelect.Items.Add(new ListItem(t.name, t.iid.ToString()));
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;
            if (UserName.Text.Length != 3)
            {
                ErrorMessage.Text = "Имя пользователя должно состоять из трёх символов";
                return;
            }
            if (!PasswordWorkingClass.CheckStrength(Password.Text))
            {
                ErrorMessage.Text = "Пароль не соответствует требованиям безопасности (не менее 8 символов, не менее 1 строчной буквы, прописной буквы и цифры)";
                return;
            }
            MembershipCreateStatus status;
            Membership.CreateUser(Region.Text,
                Password.Text, Email.Text,
                (String.IsNullOrEmpty(cbRegSelect.SelectedValue) ? "Question" : cbRegSelect.SelectedValue),
                "Answer", true, UserName.Text, out status);
            if (status == MembershipCreateStatus.Success)
            {
                try
                {
                    if (rbListAdminProperties.Visible && rbListAdminProperties.SelectedValue == Constants.ROLE_ADMIN_ROOT)
                        Roles.AddUserToRole(UserName.Text, Constants.ROLE_ADMIN_ROOT);
                    if (rbListCompProperties.Visible)
                    {
                        var u = dc.ONLusers.First(uL => uL.iid == UserName.Text);
                        switch (rbListCompProperties.SelectedValue)
                        {
                            case Constants.ROLE_ADMIN:
                                u.AddUserToCompetition(compID, Constants.ROLE_ADMIN);
                                if (u.team_id != null)
                                    u.ONLteam.AddTeamToCompetition(compID);
                                break;
                            case Constants.ROLE_USER:
                                u.AddUserToCompetition(compID, Constants.ROLE_USER);
                                if (u.team_id != null)
                                    u.ONLteam.AddTeamToCompetition(compID);
                                break;
                        }
                    }
                    ErrorMessage.Text = "Пользователь создан";
                }
                catch (Exception ex)
                {
                    Membership.DeleteUser(UserName.Text);
                    ErrorMessage.Text = "Ошибка создания пользователя: " + ex.Message;
                }
            }
            else
                ErrorMessage.Text = "Ошибка создания пользователя";
        }

        protected void btnGeneratePassword_Click(object sender, EventArgs e)
        {
            Password.Text = ConfirmPassword.Text = PasswordWorkingClass.generatePassword(8);
            ErrorMessage.Text = "Пароль: " + Password.Text;
        }
    }
}
