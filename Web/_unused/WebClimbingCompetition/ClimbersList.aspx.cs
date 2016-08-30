// <copyright file="ClimbersList.aspx.cs">
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
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class _PClimbersList : WebClimbing.src.BasePage
    {
        private string getTeamRestr()
        {
            string res = String.Empty;
            if (User.Identity.IsAuthenticated(compID))
                if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                {
                    MembershipUser usr = Membership.GetUser();
                    if (usr is ClmUser)
                    {
                        ClmUser u = (ClmUser)usr;
                        if (u.Usr.team_id != null)
                            return u.Usr.team_id.ToString();
                    }
                }
            return res;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request.Browser.EcmaScriptVersion.Major >= 1 && User.Identity.IsAuthenticated(compID))
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered(Constants.SCRIPT_TEAM_SEL_ID))
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Constants.SCRIPT_TEAM_SEL_ID,
                        " var teamSelID = '" + teamList.ClientID + "'; " +
                        " var compID = " + compID.ToString() + "; ", true);
                if (!Page.ClientScript.IsClientScriptBlockRegistered(Constants.SCRIPT_CLIMBER_VALIDATE))
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Constants.SCRIPT_CLIMBER_VALIDATE, "<script src=\"Scripts/MyScripts.js\" type=\"text/javascript\" ></script>");
            }
            if (!IsPostBack)
            {
                string sRes = getTeamRestr();
                if (!String.IsNullOrEmpty(sRes))
                {
                    teamList.SelectedValue = sRes;
                    teamList.Enabled = false;
                }
                else
                    teamList.Enabled = true;
                if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                    lblAdmMessage.Visible = false;
                lblAdmMessage.Text = String.Empty;
                try
                {
                    teamList.DataSourceID = groupsList.DataSourceID = null;
                    var tLst = from tl in dc.ONLteams
                               where tl.ONLTeamsCompLinks.Count(l => l.comp_id == compID) > 0
                               orderby tl.name
                               select new
                               {
                                   iid = tl.iid,
                                   name = tl.name
                               };
                    teamList.Items.Clear();
                    teamList.Items.Add(new ListItem("<все команды>", String.Empty));
                    foreach (var v in tLst)
                        teamList.Items.Add(new ListItem(v.name, v.iid.ToString()));
                }
                catch { }
                try
                {
                    var gLst = from gl in dc.ONLGroups
                               where gl.ONLGroupsCompLinks.Count(gln => gln.comp_id == compID) > 0
                               orderby gl.oldYear, gl.genderFemale
                               select new
                               {
                                   iid = gl.iid,
                                   name = gl.name
                               };
                    groupsList.Items.Clear();
                    groupsList.Items.Add(new ListItem("<все группы>", String.Empty));
                    foreach (var v in gLst)
                        groupsList.Items.Add(new ListItem(v.name, v.iid.ToString()));
                }
                catch { }
                panelLoginButtons.Visible = User.Identity.IsAuthenticated(compID);
                if (panelLoginButtons.Visible)
                    panelLoginButtonsAdmin.Visible = User.IsInRole(Constants.ROLE_ADMIN, compID);
                FillLists();
            }
        }

        protected void teamList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLists();
        }

        private void FillLists()
        {

            //string sWhere = "";
            //DataTable dt = new DataTable();
            //DataTable dtQ = new DataTable();
            if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                lblAdmMessage.Visible = false;
            lblAdmMessage.Text = "";
            //string sOrderBy, sCmd;



            var climberNormList = from lnk in dc.ONLClimberCompLinks
                                  where lnk.comp_id == compID
                                  && lnk.state == Constants.CLIMBER_CONFIRMED
                                  orderby lnk.ONLteam.name, lnk.ONLGroup.oldYear, lnk.ONLGroup.genderFemale
                                  select new
                                  {
                                      num = lnk.queue_pos,
                                      iid = lnk.iid,
                                      secretary_id = lnk.secretary_id,
                                      name = lnk.ONLclimber.surname + " " + lnk.ONLclimber.name +
                                        (lnk.vk ? " (в/к)" : String.Empty),
                                      age = lnk.ONLclimber.age.Value,
                                      qf = lnk.qf,
                                      team = lnk.ONLteam.name,
                                      grp = lnk.ONLGroup.name,
                                      lead = lnk.queue_Lead < 1 ? (lnk.lead == 1 ? "+" : (lnk.lead == 2 ? "Лично" : "-")) : "Оч."/* + lnk.queue_Lead.ToString()*/,//lnk.lead,
                                      speed = lnk.queue_Speed < 1 ? (lnk.speed == 1 ? "+" : (lnk.speed == 2 ? "Лично" : "-")) : "Оч."/* + lnk.queue_Speed.ToString()*/,//lnk.speed,
                                      boulder = lnk.queue_Boulder < 1 ? (lnk.boulder == 1 ? "+" : (lnk.boulder == 2 ? "Лично" : "-")) : "Оч."/* + lnk.queue_Boulder.ToString()*/,//lnk.boulder,
                                      appl_type = lnk.appl_type,
                                      is_changeble = lnk.is_changeble,
                                      ONLlink = lnk,
                                      queue_pos = lnk.queue_pos,
                                      group_p = lnk.ONLGroup
                                  };
            string sF = getTeamRestr();
            int teamF = -1;
            try
            {
                if (!String.IsNullOrEmpty(sF))
                    teamF = int.Parse(sF);
                else if (!String.IsNullOrEmpty(teamList.SelectedValue))
                    teamF = int.Parse(teamList.SelectedValue);
            }
            catch { }
            if (teamF > 0)
            {
                climberNormList = climberNormList.Where(a => a.ONLlink.team_id == teamF);
            }
            if (!String.IsNullOrEmpty(sF))
            {
                climberNormList = climberNormList.Where(a => a.ONLlink.replacementID == null);
            }
            int groupF = String.IsNullOrEmpty(groupsList.SelectedValue) ? -1 : int.Parse(groupsList.SelectedValue);
            if (groupF > 0)
            {
                climberNormList = climberNormList.Where(a=>a.ONLlink.group_id == groupF);
            }

           

            var comp = (from c in dc.ONLCompetitions
                        where c.iid == compID
                        select c).First();
            DateTime? lastUpd;
            try { lastUpd = dc.ONLClimberCompLinks.Where(lnk => lnk.comp_id == compID).OrderByDescending(l => l.sys_date_update).First().sys_date_update; }
            catch { lastUpd = null; }
            lbl1.Text = (lastUpd == null) ? String.Empty : ("Список создан в " + lastUpd.Value.ToUniversalTime().ToShortDateString() + " " +
                lastUpd.Value.ToUniversalTime().ToLongTimeString());

            if (User.Identity.IsAuthenticated(compID) && !User.IsInRole(Constants.ROLE_ADMIN, compID))
            {
                uncList.ReloadData();
                uncList.Visible = true;
            }

            clList.DataSource = (from t in climberNormList
                                 orderby t.team, t.group_p.genderFemale,
                                 t.group_p.oldYear, t.name, t.num
                                 select t);
            clList.DataBind();

            for (int i = 0; i < clList.Rows.Count; i++)
                clList.Rows[i].Cells[1].Text = (i + 1).ToString();

            var climberQueueList = climberNormList.Where(a => a.queue_pos > 0);
            climberNormList = climberNormList.Where(b => b.queue_pos < 1);

            panelQueue.Visible = (climberQueueList.Count() > 0);
            if (panelQueue.Visible)
            {
                gvQueue.DataSource = (from t in climberQueueList
                                      orderby t.group_p.genderFemale, t.group_p.oldYear,
                                      t.group_p.iid, t.queue_pos, t.team, t.name, t.num
                                      select t);
                gvQueue.DataBind();
            }
            try
            {
                bool lead = CurrentCompetition.Lead(), speed = CurrentCompetition.Speed(), boulder = CurrentCompetition.Boulder();
                if (!User.Identity.IsAuthenticated(compID))
                {
                    foreach (var dfc in clList.Columns)
                    {
                        ButtonField bf = dfc as ButtonField;
                        if (bf != null)
                            bf.Visible = false;
                        BoundField cf = dfc as BoundField;
                        if (cf != null)
                        {
                            if (!lead && cf.DataField.Equals("lead", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                            else if (!speed && cf.DataField.Equals("speed", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                            else if (!boulder && cf.DataField.Equals("boulder", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                        }
                        CheckBoxField cbf = dfc as CheckBoxField;
                        if (cbf != null && cbf.DataField.Equals("is_changeble", StringComparison.InvariantCultureIgnoreCase))
                            cbf.Visible = false;
                    }
                    foreach (var dfc in gvQueue.Columns)
                    {
                        ButtonField bf = dfc as ButtonField;
                        if (bf != null)
                            bf.Visible = false;
                        BoundField cf = dfc as BoundField;
                        if (cf != null)
                        {
                            if (!lead && cf.DataField.Equals("lead", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                            else if (!speed && cf.DataField.Equals("speed", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                            else if (!boulder && cf.DataField.Equals("boulder", StringComparison.InvariantCultureIgnoreCase))
                                cf.Visible = false;
                        }
                        CheckBoxField cbf = dfc as CheckBoxField;
                        if (cbf != null && cbf.DataField.Equals("is_changeble", StringComparison.InvariantCultureIgnoreCase))
                            cbf.Visible = false;
                    }
                }
            }
            catch { }
            /*
            try
            {
                cn.Close();
                clList.DataSource = dt;
                clList.DataBind();

                panelQueue.Visible = (dtQ.Rows.Count > 0);
                if (panelQueue.Visible)
                {
                    gvQueue.DataSource = dtQ;
                    gvQueue.DataBind();
                }

            }
            catch { }*/


        }

        protected void clList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView snd;
            if (sender == gvQueue)
                snd = gvQueue;
            else
                snd = clList;
            try
            {
                long nIid;
                try
                {
                    int line = Convert.ToInt32(e.CommandArgument);
                    snd.SelectRow(line);
                    nIid = Convert.ToInt64(snd.SelectedValue);
                }
                catch { return; }
                if (!CheckDeadline(true))
                {
                    lblAdmMessage.Visible = true;
                    lblAdmMessage.Text = "Приём изменений закончен";
                    return;
                }
                //SqlCommand cmd = new SqlCommand();
                //cmd.Connection = cn;
                ONLClimberCompLink c, cTC;
                switch (e.CommandName)
                {
                    case "MoveDown":
                        c = dc.ONLClimberCompLinks.First(l => l.iid == nIid);
                        if (!c.is_changeble)
                        {
                            lblAdmMessage.Text = "Участника невозможно заменить";
                            lblAdmMessage.Visible = true;
                            return;
                        }
                        var toChangeList = from climberToChange in dc.ONLClimberCompLinks
                                           where climberToChange.comp_id == c.comp_id
                                           && climberToChange.group_id == c.group_id
                                           && climberToChange.queue_pos > c.queue_pos
                                           && climberToChange.climber_id != c.climber_id
                                           && climberToChange.state == Constants.CLIMBER_CONFIRMED
                                           orderby climberToChange.queue_pos
                                           select climberToChange;
                        if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                            toChangeList = from clm in toChangeList
                                           where clm.team_id == c.team_id
                                           orderby clm.queue_pos
                                           select clm;
                        if (toChangeList.Count() < 1)
                        {
                            lblAdmMessage.Visible = true;
                            lblAdmMessage.Text = (c.queue_pos < 1) ? "В очереди нет участников" :
                                "Участник находится на последней позиции в очереди";
                            return;
                        }
                        cTC = toChangeList.First();

                        int p2Old = cTC.queue_pos;
                        bool chOld = cTC.is_changeble;
                        cTC.queue_pos = c.queue_pos;
                        cTC.is_changeble = c.is_changeble;
                        c.queue_pos = p2Old;
                        c.is_changeble = chOld;
                        dc.SaveChanges();

                        FillLists();
                        break;
                    case "MoveUp":
                        c = dc.ONLClimberCompLinks.First(l => l.iid == nIid);
                        var toChangeL = from cToChange in dc.ONLClimberCompLinks
                                        where cToChange.comp_id == compID
                                        && cToChange.group_id == c.group_id
                                        && cToChange.queue_pos < c.queue_pos
                                        && cToChange.queue_pos > 0
                                        && cToChange.climber_id != c.climber_id
                                        orderby cToChange.queue_pos descending
                                        select cToChange;
                        if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                            toChangeL = from tc in toChangeL
                                        where tc.team_id == c.team_id
                                        orderby tc.queue_pos descending
                                        select tc;
                        if (toChangeL.Count() < 1)
                        {
                            lblAdmMessage.Visible = true;
                            lblAdmMessage.Text = "Участник находится на первой позиции в очереди";
                            return;
                        }
                        cTC = toChangeL.First();

                        int p1Old = c.queue_pos;
                        bool isChOld = c.is_changeble;
                        c.queue_pos = cTC.queue_pos;
                        c.is_changeble = cTC.is_changeble;
                        cTC.queue_pos = p1Old;
                        cTC.is_changeble = isChOld;
                        dc.SaveChanges();
                        FillLists();
                        break;
                    case "EditLine":
                        climberEditControl.SetClimber(nIid);
                        hfEditingClimber.Value = nIid.ToString();
                        panelView.Enabled = false;
                        panelQueue.Enabled = false;
                        panelEdit.Visible = true;
                        panelAdminEdit.Visible = (User.IsInRole(Constants.ROLE_ADMIN, compID));
                        if (panelAdminEdit.Visible)
                        {
                            var cl = dc.ONLClimberCompLinks.First(l => l.iid == nIid);
                            cbQueue.Checked = (cl.queue_pos > 0);
                            cbChangeble.Checked = cl.is_changeble;
                            tbComment.Text = cl.appl_type;
                            tbRnkLead.Text = (cl.rankingLead == null ? "" : cl.rankingLead.Value.ToString());
                            tbRnkSpeed.Text = (cl.rankingSpeed == null ? "" : cl.rankingSpeed.Value.ToString());
                            tbRnkBoulder.Text = (cl.rankingBoulder == null ? "" : cl.rankingBoulder.Value.ToString());
                        }
                        climberEditControl.Visible = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    climberEditControl.ErrMessage = "Ошибка правки";
                    if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                        climberEditControl.ErrMessage += ": " + ex.Message;
                }
                catch { }
            }
        }

        protected void btnCamcelEditingClimber_Click(object sender, EventArgs e)
        {
            panelEdit.Visible = false;
            panelQueue.Enabled = true;
            panelView.Enabled = true;
        }

        protected void btnConfirmEditingClimber_Click(object sender, EventArgs e)
        {
            try
            {
                if (User.Identity.IsAuthenticated(compID))
                    ConfirmCLimber();
                else
                    climberEditControl.ErrMessage = "Вы не имеете прав на это действие";
            }
            catch (Exception ex)
            {
                climberEditControl.ErrMessage = "Ошибка правки";
                if (User.IsInRole(Constants.ROLE_ADMIN,compID))
                    climberEditControl.ErrMessage += ": " + ex.Message;
            }
        }

        private void ConfirmCLimber()
        {
            if(String.IsNullOrEmpty(hfEditingClimber.Value))
                return;
            long curClmId;
            if(!long.TryParse(hfEditingClimber.Value, out curClmId))
                return;
            ONLClimberCompLink old;
            try { old = dc.ONLClimberCompLinks.First(l => l.iid == curClmId); }
            catch { return; }
            bool isUser = !User.IsInRole(Constants.ROLE_ADMIN, compID);
            if (isUser && !old.is_changeble)
            {
                climberEditControl.ErrMessage = "Изменение данного участника невозможно";
                return;
            }
            bool newClm;
            ONLclimber createdClm;
            var uCl = climberEditControl.createClimber(old.team_id.ToString(), out newClm, out createdClm);
            if (uCl == null)
                return;
            if (isUser && uCl.group_id != old.group_id)
            {
                climberEditControl.ErrMessage = "Для изменения возрастной группы подайте новую заявку";
                if (newClm && createdClm.EntityState != EntityState.Detached)
                    dc.ONLclimbers.Detach(createdClm);
                if (uCl.EntityState != EntityState.Detached)
                    dc.ONLClimberCompLinks.Detach(uCl);
                return;
            }
            if (newClm)
            {
                int newClmId;
                if (dc.ONLclimbers.Count() < 1)
                    newClmId = 1;
                else
                    newClmId = dc.ONLclimbers.OrderByDescending(c => c.iid).First().iid + 1;
                createdClm.iid = newClmId;
                dc.ONLclimbers.AddObject(createdClm);
                if (uCl.EntityState != EntityState.Detached)
                    dc.ONLClimberCompLinks.Detach(uCl);
                dc.SaveChanges();
            }

            if (User.IsInRole(Constants.ROLE_ADMIN, compID))
            {
                old.climber_id = createdClm.iid;

                ConfirmAdmin(uCl, old);
            }
            else
            {
                uCl.iid = dc.ONLClimberCompLinks.OrderByDescending(l => l.iid).First().iid + 1;

                ONLoperation nextOp = GetOperation();
                uCl.updOpIid = nextOp.iid;
                uCl.ONLoperation = nextOp;
                uCl.climber_id = createdClm.iid;
                uCl.ONLclimber = createdClm;
                uCl.secretary_id = old.secretary_id;
                dc.ONLClimberCompLinks.AddObject(uCl);
                dc.SaveChanges();

                old.replacementID = uCl.iid;
                old.ONLClimberCompLink2 = uCl;
                old.sys_date_update = DateTime.UtcNow;
                dc.SaveChanges();

                uncList.LblMessageText = "Изменение принято. Для отправки изменений старшему тренеру Вашего региона нажмите на \"Подтвердить все заявки через e-mail\"";
            }

            ClimbingConfirmations.DeleteDeadClimbers(dc);

            panelEdit.Visible = false;
            panelView.Enabled = true;
            panelQueue.Enabled = true;
            FillLists();
        }

        private void ConfirmAdmin(ONLClimberCompLink uCl, ONLClimberCompLink old)
        {
            int? rLead, rSpeed, rBoulder;
            int nTmp;
            if (String.IsNullOrEmpty(tbRnkLead.Text))
                rLead = null;
            else if (int.TryParse(tbRnkLead.Text.Trim(), out nTmp))
                rLead = nTmp;
            else
            {
                climberEditControl.ErrMessage = "Рейтинг в трудности введён неправильно";
                return;
            }

            if (String.IsNullOrEmpty(tbRnkSpeed.Text))
                rSpeed = null;
            else if (int.TryParse(tbRnkSpeed.Text.Trim(), out nTmp))
                rSpeed = nTmp;
            else
            {
                climberEditControl.ErrMessage = "Рейтинг в скорости введён неправильно";
                return;
            }

            if (String.IsNullOrEmpty(tbRnkBoulder.Text))
                rBoulder = null;
            else if (int.TryParse(tbRnkBoulder.Text.Trim(), out nTmp))
                rBoulder = nTmp;
            else
            {
                climberEditControl.ErrMessage = "Рейтинг в боулдеринге введён неправильно";
                return;
            }

            old.appl_type = tbComment.Text;
            old.boulder = uCl.boulder;
            old.is_changeble = cbChangeble.Checked;
            old.lead = uCl.lead;
            old.qf = uCl.qf;
            if (!cbQueue.Checked)
                old.queue_pos = 0;
            else if (old.queue_pos < 1)
                old.queue_pos = 1;
            old.rankingBoulder = rBoulder;
            old.rankingLead = rLead;
            old.rankingSpeed = rSpeed;
            old.replacementID = null;
            old.speed = uCl.speed;
            old.sys_date_update = DateTime.UtcNow;

            if (uCl.EntityState != EntityState.Detached)
                dc.ONLClimberCompLinks.Detach(uCl);

            dc.SaveChanges();
        }

        private ONLoperation GetOperation()
        {
            var opList = from o in dc.ONLoperations
                         where o.user_id == User.Identity.Name
                         && o.comp_id == compID
                         && o.state == Constants.OP_STATE_NEW
                         orderby o.op_date descending
                         select o;
            if (opList.Count() < 1)
            {
                ONLoperation opToRet = ONLoperation.CreateONLoperation(
                    iid: (dc.ONLoperations.Count() < 1) ? 1 : (dc.ONLoperations.OrderByDescending(o => o.iid).First().iid + 1),
                    comp_id: compID,
                    user_id: User.Identity.Name,
                    op_date: DateTime.UtcNow,
                    state: Constants.OP_STATE_NEW);
                return opToRet;
            }
            else
                return opList.First();
        }

        protected void btnDelEditingClimber_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(hfEditingClimber.Value))
                return;
            long delId;
            if (!long.TryParse(hfEditingClimber.Value, out delId))
                return;
            ONLClimberCompLink lToDel;
            try { lToDel = dc.ONLClimberCompLinks.First(l => l.iid == delId); }
            catch { return; }
            try
            {
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    DeleteAdmin(lToDel);
                else if (User.Identity.IsAuthenticated(compID))
                    DeleteUser(lToDel);
                else
                    climberEditControl.ErrMessage = "У вас нет прав на даное действие";
            }
            catch (Exception ex)
            {
                climberEditControl.ErrMessage = "Ошибка удаления";
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    climberEditControl.ErrMessage += ": " + ex.Message;
            }
        }
        private void DeleteAdmin(ONLClimberCompLink lnk)
        {
            try
            {
                foreach (var c in lnk.ONLClimberCompLink1)
                    c.replacementID = null;
                dc.ONLClimberCompLinks.DeleteObject(lnk);
                dc.SaveChanges();
                FillLists();
            }
            catch (Exception ex)
            {
                lblAdmMessage.Text = "Ошибка удаления: " + ex.Message;
            }
        }

        private void DeleteUser(ONLClimberCompLink lnk)
        {
            long oldIid = lnk.iid;
            ONLClimberCompLink lnkDel = dc.ONLClimberCompLinks.First(l => l.iid == lnk.iid);
            if (lnkDel.EntityState != EntityState.Detached)
                dc.ONLClimberCompLinks.Detach(lnkDel);
            long newIid = dc.ONLClimberCompLinks.OrderByDescending(l => l.iid).First().iid + 1;
            lnkDel.iid = newIid;
            lnkDel.state = Constants.CLIMBER_PENDING_DELETE;
            var op = GetOperation();
            lnkDel.updOpIid = op.iid;
            lnkDel.ONLoperation = op;
            dc.ONLClimberCompLinks.AddObject(lnkDel);
            dc.SaveChanges();

            lnk = dc.ONLClimberCompLinks.First(i => i.iid == oldIid);
            lnk.replacementID = newIid;

            dc.SaveChanges();

            panelView.Enabled = true;
            panelEdit.Visible = false;
            panelQueue.Enabled = true;

            try
            {
                uncList.LblMessageText = "Удаление принято. Для отправки изменений старшему тренеру Вашего региона нажмите на \"Подтвердить все заявки через e-mail\"";
                FillLists();
            }
            catch { }
        }

        protected void btnRecalculateQueue_Click(object sender, EventArgs e)
        {
            try
            {
                List<long> groups = new List<long>();
                //ClimbingClassesDataContext dc = new ClimbingClassesDataContext(cn);
                var gList = from g in dc.ONLGroupsCompLinks
                            select g.iid;
                foreach (var gg in gList)
                    groups.Add(gg);
                foreach (long n in groups)
                    WebClimbing.src.ClimbingConfirmations.RecalculateQueue(n, dc);
                dc.SaveChanges();
                FillLists();
                lblAdmMessage.Text = "Очередь пересчитана";
            }
            catch (Exception ex)
            {
                lblAdmMessage.Text = "Ошибка пересчёта очереди: " + ex.Message;
            }
        }

        protected void btnRecalculateRanking_Click(object sender, EventArgs e)
        {
            try
            {
                WebClimbing.src.ClimbingConfirmations.LoadAllRankings(dc);
                lblAdmMessage.Text = "Рейтинг загружен. Если требуется, пересчитайте очередь";
            }
            catch (Exception ex)
            {
                lblAdmMessage.Text = "Ошибка пересчёта рейтинга: " + ex.Message;
            }
        }

        protected void uncList_rowDeleted(object sender, EventArgs e)
        {
            try { FillLists(); }
            catch { }
        }

        protected void btnSendClimbersList_Click(object sender, EventArgs e)
        {
            if(!User.Identity.IsAuthenticated(compID))
                return;
            lblAdmMessage.Visible = true;
            lblAdmMessage.Text = Tasks.SendClimbersListForUser(User.Identity.Name, compID);
        }
    }
}