// <copyright file="login.aspx.cs">
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
using System.Linq;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class _Plogin : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = true;
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;
            cbUsers.Items.Clear();
            var usrList = (from u in dc.ONLuserRoles
                           where u.comp_id == compID
                           || u.role_id == Constants.ROLE_ADMIN_ROOT
                           select new { Iid = u.ONLuser.iid, Name = u.ONLuser.name }).Distinct().OrderBy(a => a.Name);

            cbUsers.Items.Clear();
            foreach (var p in usrList)
                cbUsers.Items.Add(new ListItem(p.Name, p.Iid));
        }

        private ONLuser selectedUser()
        {
            var uList = from u in dc.ONLusers
                        where u.iid == cbUsers.SelectedValue
                        select u;
            if (uList.Count() < 1)
                return null;
            return uList.First();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;
            if (Membership.ValidateUser(cbUsers.SelectedValue, tbPassword.Text))
            {
                try
                {
                    var u = selectedUser();
                    u.loginDate = DateTime.UtcNow;
                    dc.SaveChanges();
                }
                catch { }
                if (compID < 1)
                {
                    FormsAuthentication.SetAuthCookie(cbUsers.SelectedValue, false);
                    Response.Redirect("~/Apps/UserAccountmanagement/DBManagement/CompetitionManagement.aspx");
                }
                else
                    FormsAuthentication.RedirectFromLoginPage(cbUsers.SelectedValue, false);
            }
            else
                lblStatus.Text = "Неверный пароль";
        }

        protected void getPassword_Click(object sender, EventArgs e)
        {
            try
            {
                var oTmp = Membership.GetUser(cbUsers.SelectedValue, false);
                if (!(oTmp is ClmUser))
                {
                    lblStatus.Text = "Неверный пользователь";
                    return;
                }
                ClmUser usr = (ClmUser)oTmp;
                if (usr.Usr.lastForgPassword != null)
                {
                    TimeSpan ts = DateTime.Now.ToUniversalTime() - usr.Usr.lastForgPassword.Value;
                    if (ts.Minutes < 5)
                    {
                        lblStatus.Text = "Пароль был отправлен на Ваш e-mail. " +
                            "Повторная отправка пароля возможна не менее чем через 5 минут";
                        return;
                    }
                }
                if (String.IsNullOrEmpty(usr.Email))
                {
                    lblStatus.Text = "Невозможно восстановить пароль для данного пользователя";
                    return;
                }
                string erMsg;
                if (mailService.SendMail(usr.Email, "Ваш пароль в системе онлайн подачи заявок",
                    "Ваш пароль в системе подачи заявок: " + usr.Usr.password, MailPriority.High,
                    out erMsg))
                {
                    lblStatus.Text = "Ваш пароль выслан на Ваш e-mail";
                    try
                    {
                        cbUsers.SelectedValue = usr.Usr.iid;
                        var u = selectedUser();
                        u.lastForgPassword = DateTime.UtcNow;
                        dc.SaveChanges();
                    }
                    catch { }
                }
                else
                    lblStatus.Text = "Ошибка отправки пароля на e-mail";
            }
            catch { }
        }
    }
}
