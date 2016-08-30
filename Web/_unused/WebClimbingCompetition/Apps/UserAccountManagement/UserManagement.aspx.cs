// <copyright file="UserManagement.aspx.cs">
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing.Apps.UserAccountManagement
{
    public partial class _PUserManagement : BasePage//RestrictedAdminDatabase
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }

        //private int? tDF = null;
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
            this.NoRedirecting = true;
            //this.FormName = "UserManagement";
            base.Page_Load(sender, e);
            if (Page.IsPostBack)
                return;
            SetAdminButtonsVisible();
            RefreshUserList();
            cbTeams.Items.Clear();
            cbTeams.Items.Add(new ListItem("", ""));

            var v = from t in dc.ONLteams
                    orderby t.name ascending
                    select new
                    {
                        Iid = t.iid,
                        Name = t.name
                    };
            foreach (var q in v)
                cbTeams.Items.Add(new ListItem(q.Name, q.Iid.ToString()));
        }

        private void RefreshUserList()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            try { Culture = "ru-RU"; }
            catch { }
            var uList = from u in dc.ONLusers
                        orderby u.name ascending
                        select new
                        {
                            Iid = u.iid,
                            Region = u.name,
                            Email = u.email,
                            LastLogin = u.loginDate,
                            Admin = (u.ONLuserRoles.Count(ur3 => ur3.role_id == Constants.ROLE_ADMIN_ROOT) > 0),
                            AdminComp = (u.ONLuserRoles.Count(ur => ur.comp_id == compID && ur.role_id == Constants.ROLE_ADMIN) > 0),
                            UserComp = (u.ONLuserRoles.Count(ur2 => ur2.comp_id == compID && ur2.role_id == Constants.ROLE_USER) > 0)
                        };
            gvUsers.DataSource = uList;
            gvUsers.DataBind();
        }

        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                object oKey = gvUsers.SelectedValue;
                if (!(oKey is string))
                    return;
                string sKey = (string)oKey;
                MembershipUser usr = Membership.GetUser(sKey);
                if (!(usr is ClmUser))
                    return;
                ClmUser cUser = (ClmUser)usr;
                lblIid.Text = cUser.Usr.iid;
                tbName.Text = cUser.Usr.name;
                tbEmail.Text = cUser.Usr.email;
                if (cUser.Usr.team_id == null)
                    cbTeams.SelectedValue = "";
                else
                    try { cbTeams.SelectedValue = cUser.Usr.team_id.ToString(); }
                    catch { }
                lblMessage.Text = "";
                var u = GetUserByIid(lblIid.Text);
                if (u == null)
                    return;

                if (rbListAdminProperties.Visible)
                {
                    if (Roles.IsUserInRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT))
                        rbListAdminProperties.SelectedValue = Constants.ROLE_ADMIN_ROOT;
                    else
                        rbListAdminProperties.SelectedValue = String.Empty;
                }
                if (rbListCompProperties.Visible)
                {
                    if (u.ONLuserRoles.Count(r => r.role_id == Constants.ROLE_ADMIN && r.comp_id == compID) > 0)
                        rbListCompProperties.SelectedValue = Constants.ROLE_ADMIN;
                    else if (u.ONLuserRoles.Count(r => r.role_id == Constants.ROLE_USER && r.comp_id == compID) > 0)
                        rbListCompProperties.SelectedValue = Constants.ROLE_USER;
                    else
                        rbListCompProperties.SelectedValue = String.Empty;
                }
            }
            catch { }
        }

        protected void ClearCurrent()
        {
            lblIid.Text = "";
            tbEmail.Text = "";
            tbName.Text = "";
            cbTeams.SelectedValue = "";
            if (rbListCompProperties.Visible)
                rbListCompProperties.SelectedValue = String.Empty;
            if (rbListAdminProperties.Visible)
                rbListAdminProperties.SelectedValue = String.Empty;
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(lblIid.Text))
            {
                lblMessage.Text = "Пользователь не выбран";
                return;
            }
            if (Roles.IsUserInRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT))
            {
                var usrList = Roles.GetUsersInRole(Constants.ROLE_ADMIN_ROOT);
                if (usrList.Length < 2)
                {
                    lblMessage.Text = "Невозможно удалить последнего администратора";
                    return;
                }
            }
            try
            {
                var u = dc.ONLusers.First(uF => uF.iid.Equals(lblIid.Text, StringComparison.InvariantCultureIgnoreCase));
                while (u.ONLuserRoles.Count() > 0)
                    dc.ONLuserRoles.DeleteObject(u.ONLuserRoles.First());
                dc.SaveChanges();
                dc.ONLusers.DeleteObject(u);
                dc.SaveChanges();
                ClearCurrent();
                lblMessage.Text = "Пользователь удалён";
                RefreshUserList();

                //if (Membership.DeleteUser(lblIid.Text))
                //{
                //    ClearCurrent();
                //    lblMessage.Text = "Пользователь удалён";
                //    RefreshUserList();
                //}
                //else
                //    lblMessage.Text = "Ошибка удаления пользователя";
            }
            catch(Exception ex)
            {
                lblMessage.Text = "Ошибка удаления пользователя:\r\n"+ex.ToString();
            }
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(lblIid.Text))
            {
                lblMessage.Text = "Пользователь не выбран";
                return;
            }
            int? teamId;
            if (cbTeams.SelectedValue == "")
                teamId = null;
            else
            {
                int nTmp;
                if (!int.TryParse(cbTeams.SelectedValue, out nTmp))
                {
                    lblMessage.Text = "Команда выбрана неверно.";
                    return;
                }
                teamId = nTmp;
            }
            try
            {
                MembershipUser usr = Membership.GetUser(lblIid.Text);
                if (!(usr is ClmUser))
                {
                    lblMessage.Text = "Ошибка обновления пользователя. Пользователь не найден";
                    return;
                }
                ClmUser cUser = (ClmUser)usr;
                cUser.Email = tbEmail.Text;
                cUser.Usr.name = tbName.Text;
                cUser.Usr.team_id = teamId;
                Membership.UpdateUser(cUser);

                lblMessage.Text = "Пользователь обновлён";

                if (!rbListCompProperties.SelectedValue.Equals(Constants.ROLE_ADMIN_ROOT) &&
                    dc.ONLuserRoles.Count(r => r.role_id == Constants.ROLE_ADMIN_ROOT) < 2)
                {
                    lblMessage.Text += "; Оставлен статус администратора БД";
                    RefreshUserList();
                }

                ONLuser u = GetUserByIid(lblIid.Text);
                if (u == null)
                    return;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.Add("@uid", SqlDbType.VarChar, 3);
                cmd.Parameters[0].Value = u.iid;
                if (User.IsInRole(Constants.ROLE_ADMIN_ROOT) && rbListCompProperties.SelectedValue.Equals(String.Empty) && CurrentCompetition == null)
                {
                    cmd.CommandText = "DELETE FROM ONLuserRoles WHERE user_id = @uid";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    if (rbListAdminProperties.Visible)
                        switch (rbListAdminProperties.SelectedValue)
                        {
                            case Constants.ROLE_ADMIN_ROOT:
                                if (!Roles.IsUserInRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT))
                                    Roles.AddUserToRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT);
                                break;
                            default:
                                if (Roles.IsUserInRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT))
                                    Roles.RemoveUserFromRole(lblIid.Text, Constants.ROLE_ADMIN_ROOT);
                                break;
                        }
                    if(rbListCompProperties.Visible)
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
                            default:
                                u.RemoveUserFromCompetition(compID);
                                break;
                        }
                }
                RefreshUserList();
                
            }
            catch
            {
                lblMessage.Text = "Ошибка обновления пользователя.";
            }
                
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            lblMessageMail.Text = "Отправка сообщений";
            tbMessage.Text = "";
            tbSubj.Text = "";
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbSubj.Text))
            {
                lblMessageMail.Text = "Тема не введена";
                return;
            }
            if (String.IsNullOrEmpty(tbMessage.Text))
            {
                lblMessageMail.Text = "Сообщение не введено";
                return;
            }
            string sError = "", sTmp;
            try
            {
                List<string> eList = new List<string>();
                if (String.IsNullOrEmpty(tbEmail.Text))
                {
                    var emlListL = from u in dc.ONLusers
                                   where u.ONLuserRoles.Count(r => r.comp_id == compID &&
                                                                 (r.role_id == Constants.ROLE_USER || r.role_id == Constants.ROLE_ADMIN)
                                                              ) > 0
                                   select u.email;

                    foreach (var s in emlListL)
                        if (!String.IsNullOrEmpty(s))
                            if (eList.IndexOf(s) < 0)
                                eList.Add(s);
                }
                else
                    eList.Add(tbEmail.Text);
                foreach (string eml in eList)
                {
                    if (!mailService.SendMail(eml, tbSubj.Text, tbMessage.Text, MailPriority.High, out sTmp))
                    {
                        if (!String.IsNullOrEmpty(sError))
                            sError += ", ";
                        sError += eml + " - " + sTmp;
                    }
                }
            }
            catch (Exception ex) { sError = ex.Message; }
            if (String.IsNullOrEmpty(sError))
                lblMessageMail.Text = "Сообщения успешно отправлены";
            else
                lblMessageMail.Text = "Ошибка рассылки сообщений: " + sError;
        }
    }
}
