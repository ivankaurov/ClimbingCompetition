// <copyright file="GroupsManagement.aspx.cs">
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
using WebClimbing.src;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using System.Globalization;


namespace WebClimbing.Apps.UserAccountManagement
{
    public partial class GroupsManagement : BasePageRestrictedAdminComp
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = true;
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;
            LoadGroups();
            btnDelete.Enabled = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
            btnDelete.Visible = btnDelete.Enabled;
        }

        private void LoadGroups()
        {
            //var gList = from g in dc.ONLGroups
            //            orderby g.oldYear, g.genderFemale
            //            select g;
                        /*select new
                        {
                            Iid = g.iid,
                            Name = g.name,
                            Ages = g.oldYear + " - " + g.youngYear,
                            Gender = g.genderFemale ? "ж" : "м",
                            UsingNow = (g.ONLGroupsCompLinks.Count(gl => gl.comp_id == compID) > 0)
                        };*/
            List<ONLGroup> gList = new List<ONLGroup>();
            foreach (var g in dc.ONLGroups)
                gList.Add(g);
            gList.Sort((a, b) => (a.oldYear == b.oldYear ? a.genderFemale.CompareTo(b.genderFemale) : a.oldYear.CompareTo(b.oldYear)));
            var gLst = from g in gList
                       select new
                       {
                           Iid = g.iid,
                           Name = g.name,
                           Ages = g.youngYear + " - " + g.oldYear + " лет",
                           Gender = g.genderFemale ? "ж" : "м",
                           UsingNow = (g.ONLGroupsCompLinks.Count(gl => gl.comp_id == compID) > 0)
                       };

            gvGroups.DataSource = gLst;
            gvGroups.DataBind();

            var qfListS = from q in dc.ONLqfLists
                         orderby q.iid
                         select q;
            qfList.Items.Clear();
            foreach (var q in qfListS)
                qfList.Items.Add(new ListItem(q.qf, q.iid.ToString()));
        }

        private ONLGroup GetGroupByIid(int iid)
        {
            try { return dc.ONLGroups.First(t => t.iid == iid); }
            catch { return null; }
        }

        private bool SetGroup(ONLGroup t)
        {
            if (t == null)
                return false;
            panelEdit.Visible = true;
            SetCompVisible();
            lblIid.Text = t.iid.ToString();
            tbName.Text = t.name;
            tbOld.Text = t.oldYear.ToString();
            tbYoung.Text = t.youngYear.ToString();
            if (t.genderFemale)
                cbGender.SelectedValue = "F";
            else
                cbGender.SelectedValue = "M";
            if (cbUsedNow.Visible)
            {
                cbUsedNow.Checked = (t.ONLGroupsCompLinks.Count(lnk => lnk.comp_id == compID) > 0);
                if (cbUsedNow.Checked)
                {
                    var lnk = t.ONLGroupsCompLinks.First(ln => ln.comp_id == compID);
                    qfList.SelectedValue = lnk.minQf.ToString();
                }
                else if (qfList.Items.Count > 0)
                    qfList.SelectedIndex = qfList.Items.Count - 1;
            }
            lblError.Text = String.Empty;
            return true;
        }

        private void ClearGroupEdit()
        {
            tbOld.Text = tbYoung.Text = lblIid.Text = tbName.Text = lblError.Text = String.Empty;
            if (cbUsedNow.Visible)
            {
                cbUsedNow.Checked = false;
                if (qfList.Items.Count > 0)
                    qfList.SelectedIndex = qfList.Items.Count - 1;
            }
        }

        private void SetCompVisible()
        {
            lblGroupQF.Visible = lblCompUsage.Visible = qfList.Visible = cbUsedNow.Visible = (CurrentCompetition != null);
            if (cbUsedNow.Visible)
                lblCompUsage.Text = "Доступ в соревнования \"" + CurrentCompetition.short_name + "\":";
            btnDelete.Enabled = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
        }

        protected void btnNewGroup_Click(object sender, EventArgs e)
        {
            panelSelect.Enabled = false;
            panelEdit.Visible = true;
            SetCompVisible();
            ClearGroupEdit();
        }

        protected void btnCanel_Click(object sender, EventArgs e)
        {
            panelEdit.Visible = false;
            panelSelect.Enabled = true;
            lblError.Text = String.Empty;
        }

        protected void gvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            object oKey = gvGroups.SelectedValue;
            if (!(oKey is int))
                return;
            if (SetGroup(GetGroupByIid((int)oKey)))
            {
                panelEdit.Visible = true;
                panelSelect.Enabled = false;
                SetCompVisible();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(tbName.Text))
                {
                    lblError.Text = "Название группы не введено";
                    return;
                }
                int oldYear, youngYear;
                if (String.IsNullOrWhiteSpace(tbOld.Text))
                {
                    oldYear = 99;
                    tbOld.Text = "99";
                }
                else if (!int.TryParse(tbOld.Text, out oldYear))
                {
                    lblError.Text = "Максимальный возраст введён неправильно";
                    return;
                }
                if (String.IsNullOrWhiteSpace(tbYoung.Text))
                {
                    youngYear = 0;
                    tbYoung.Text = "0";
                }
                else if (!int.TryParse(tbYoung.Text, out youngYear))
                {
                    lblError.Text = "Минимальный возраст введён неправильно";
                    return;
                }

                bool genderFemale = cbGender.SelectedValue == "F";

                if (oldYear < youngYear)
                {
                    lblError.Text = "Максимальный возраст меньше минимального";
                    return;
                }

                ONLGroup gNew;
                if (String.IsNullOrEmpty(lblIid.Text))
                    gNew = ONLGroup.CreateONLGroup((int)SortingClass.GetNextIID("ONLGroups", "iid", cn, null),
                        oldYear, youngYear, true);
                else
                    gNew = GetGroupByIid(int.Parse(lblIid.Text));
                gNew.name = tbName.Text;
                gNew.oldYear = oldYear;
                gNew.youngYear = youngYear;
                gNew.genderFemale = genderFemale;

                if (cbUsedNow.Visible)
                {
                    int qf = int.Parse(qfList.SelectedValue);
                    if (cbUsedNow.Checked)
                    {
                        if (gNew.ONLGroupsCompLinks.Count(tlnk => tlnk.comp_id == compID) < 1)
                        {
                            ONLGroupsCompLink lnkNew = ONLGroupsCompLink.CreateONLGroupsCompLink(
                                SortingClass.GetNextIID("ONLGroupsCompLink", "iid", cn, null),
                                gNew.iid, compID, qf);
                            gNew.ONLGroupsCompLinks.Add(lnkNew);
                        }
                        else
                            gNew.ONLGroupsCompLinks.First(tlk => tlk.comp_id == compID).minQf = qf;
                    }
                    else
                    {
                        try
                        {
                            var lToDel = gNew.ONLGroupsCompLinks.First(tl => tl.comp_id == compID);
                            dc.ONLGroupsCompLinks.DeleteObject(lToDel);
                        }
                        catch { }
                    }
                }
                if (String.IsNullOrEmpty(lblIid.Text))
                    dc.ONLGroups.AddObject(gNew);
                dc.SaveChanges();
                panelEdit.Visible = false;
                panelSelect.Enabled = true;
                LoadGroups();
                lblError.Text = "Группа создана/обновлена";
            }
            catch (Exception ex)
            {
                lblError.Text = "Ошибка добавления/обновления группы:\r\n" +
                   ex.Message;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(lblIid.Text))
                {
                    panelEdit.Visible = false;
                    panelSelect.Enabled = true;
                    lblError.Text = String.Empty;
                    return;
                }
                var tToDel = GetGroupByIid(int.Parse(lblIid.Text));
                if (tToDel != null)
                {
                    dc.ONLGroups.DeleteObject(tToDel);
                    dc.SaveChanges();
                }
                lblError.Text = "Группа " + tbName.Text + " удалена";
                LoadGroups();
                panelEdit.Visible = false;
                panelSelect.Enabled = true;
            }
            catch (Exception ex) { lblError.Text = "Ошибка удаления:<br />" + ex.ToString(); }
        }
    }
}