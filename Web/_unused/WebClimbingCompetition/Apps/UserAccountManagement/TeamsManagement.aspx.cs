using System;
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
    public partial class TeamsManagement : BasePageRestrictedAdminComp
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = true;
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;
            LoadTeams();
            LoadTeamGroups();
            btnDelete.Enabled = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
        }

        private void LoadTeams()
        {
            var tList = from t in dc.ONLteams
                        orderby ((t.group_id == null ? String.Empty : t.ONLTeamGroup.name) + "_" + t.name)
                        select new
                        {
                            Iid = t.iid,
                            Region = t.name,
                            GroupName = (t.group_id == null ? String.Empty : t.ONLTeamGroup.name),
                            UsingNow = ((t.ONLTeamsCompLinks.Count(tl => tl.comp_id == compID)) > 0)
                        };
            gvTeams.DataSource = tList;
            gvTeams.DataBind();
        }

        private void LoadTeamGroups()
        {
            var tgList = from tg in dc.ONLTeamGroups
                         orderby tg.name
                         select new { Iid = tg.iid, GroupName = tg.name };
            gvTeamGroups.DataSource = tgList;
            gvTeamGroups.DataBind();

            cbGroupToSet.Items.Clear();
            cbGroupToSet.Items.Add(new ListItem("Без группы", String.Empty));
            foreach (var v in tgList)
                cbGroupToSet.Items.Add(new ListItem(v.GroupName, v.Iid.ToString()));
        }

        private ONLteam GetTeamByIid(int iid)
        {
            try { return dc.ONLteams.First(t => t.iid == iid); }
            catch { return null; }
        }

        private bool SetTeam(ONLteam t)
        {
            if (t == null)
                return false;
            panelEdit.Visible = true;
            SetCompVisible();
            lblIid.Text = t.iid.ToString();
            tbName.Text = t.name;
            if (cbUsedNow.Visible)
            {
                cbUsedNow.Checked = (t.ONLTeamsCompLinks.Count(lnk => lnk.comp_id == compID) > 0);
                if (cbUsedNow.Checked)
                {
                    ONLTeamsCompLink lnk = t.ONLTeamsCompLinks.First(ln => ln.comp_id == compID);
                    tbRanking.Text = (lnk.ranking_pos > 0 && lnk.ranking_pos < int.MaxValue) ? lnk.ranking_pos.ToString() : String.Empty;
                }
                else
                {
                    tbRanking.Text = String.Empty;
                }
            }
            try
            {
                if (t.group_id == null || !t.group_id.HasValue)
                    cbGroupToSet.SelectedIndex = 0;
                else
                    cbGroupToSet.SelectedValue = t.group_id.Value.ToString();
            }
            catch { cbGroupToSet.SelectedIndex = 0; }
            lblError.Text = String.Empty;
            return true;
        }

        private void ClearTeamEdit()
        {
            lblIid.Text = tbName.Text = lblError.Text = String.Empty;
            if (cbUsedNow.Visible)
            {
                cbUsedNow.Checked = false;
                tbRanking.Text = String.Empty;
            }
            cbGroupToSet.SelectedIndex = 0;
        }

        private void SetCompVisible()
        {
            lblCompRanking.Visible = lblCompUsage.Visible = tbRanking.Visible = cbUsedNow.Visible = (CurrentCompetition != null);
            if (cbUsedNow.Visible)
                lblCompUsage.Text = "Доступ в соревнования \"" + CurrentCompetition.short_name + "\":";
            btnDelete.Enabled = User.IsInRole(Constants.ROLE_ADMIN_ROOT);
        }

        protected void btnNewTeam_Click(object sender, EventArgs e)
        {
            panelSelect.Enabled = false;
            panelEdit.Visible = true;
            panelTeamGroups.Visible = false;
            SetCompVisible();
            ClearTeamEdit();
        }

        protected void btnCanel_Click(object sender, EventArgs e)
        {
            panelEdit.Visible = false;
            panelSelect.Enabled = true;
            panelTeamGroups.Visible = true;
            lblError.Text = String.Empty;
        }

        protected void gvTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            object oKey = gvTeams.SelectedValue;
            if (!(oKey is int))
                return;
            if (SetTeam(GetTeamByIid((int)oKey)))
            {
                panelEdit.Visible = true;
                panelSelect.Enabled = false;
                panelTeamGroups.Visible = false;
                SetCompVisible();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(tbName.Text))
                {
                    lblError.Text = "Название команды не введено";
                    return;
                }
                if (cbGroupToSet.SelectedIndex < 0 || cbGroupToSet.SelectedItem == null)
                {
                    lblError.Text = "Группа команд не выбрана";
                    return;
                }
                int? grIdToSet;
                if (cbGroupToSet.SelectedIndex == 0)
                    grIdToSet = null;
                else
                    grIdToSet = new int?(int.Parse(cbGroupToSet.SelectedItem.Value));
                ONLteam tNew;
                if (String.IsNullOrEmpty(lblIid.Text))
                    tNew = ONLteam.CreateONLteam((int)SortingClass.GetNextIID("ONLTeams", "iid", cn, null));
                else
                    tNew = GetTeamByIid(int.Parse(lblIid.Text));
                tNew.name = tbName.Text;
                
                if (String.IsNullOrEmpty(lblIid.Text))
                    dc.ONLteams.AddObject(tNew);
                tNew.group_id = grIdToSet;
                dc.SaveChanges();
                if (cbUsedNow.Visible)
                {
                    if (cbUsedNow.Checked)
                    {
                        int rankingPos;
                        if (!int.TryParse(tbRanking.Text, out rankingPos))
                            rankingPos = int.MaxValue;
                        tNew.AddTeamToCompetition(compID, new int?(rankingPos));
                        try
                        {
                            var uList = tNew.ONLusers.ToArray();
                            for (int i = 0; i < uList.Length; i++)
                                uList[i].AddUserToCompetition(compID);
                        }
                        catch { }
                    }
                    else
                        tNew.RemoveTeamFromCompetition(compID);
                }

                panelEdit.Visible = false;
                panelSelect.Enabled = true;
                panelTeamGroups.Visible = true;
                LoadTeams();
                lblError.Text = "Команда создана/обновлена";
            }
            catch (Exception ex)
            {
                lblError.Text = "Ошибка добавления/обновления команды:\r\n" +
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
                var tToDel = GetTeamByIid(int.Parse(lblIid.Text));
                if (tToDel != null)
                {
                    dc.ONLteams.DeleteObject(tToDel);
                    dc.SaveChanges();
                }
                lblError.Text = "Команда " + tbName.Text + " удалена";
                LoadTeams();
                panelEdit.Visible = false;
                panelSelect.Enabled = true;
                panelTeamGroups.Visible = true;
            }
            catch (Exception ex) { lblError.Text = "Ошибка удаления:<br />" + ex.ToString(); }
        }

        private void ClearTGEdit()
        {
            tbGroupQuota.Text = lblGroupIid.Text = lblTGEditError.Text = tbGroupName.Text = String.Empty;
            cbUseCompGroup.Checked = false;
        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            panelTeams.Visible = false;
            panelGroupsSelect.Enabled = false;
            panelGroupsEdit.Visible = true;
            ClearTGEdit();
        }

        bool allowTGEvent = true;
        protected void gvTeamGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!allowTGEvent)
                return;
            object oKey = gvTeamGroups.SelectedValue;
            if (!(oKey is int))
                return;
            int nIid = (int)oKey;
            try
            {
                var tg = dc.ONLTeamGroups.First(g => g.iid == nIid);
                ClearTGEdit();
                lblGroupIid.Text = tg.iid.ToString();
                tbGroupName.Text = tg.name;
                panelTeams.Visible = false;
                panelGroupsEdit.Visible = true;
                panelGroupsSelect.Enabled = false;
                cbUseCompGroup.Enabled = tbGroupQuota.Enabled = (CurrentCompetition!= null);
                if (CurrentCompetition != null)
                {
                    cbUseCompGroup.Checked = CurrentCompetition.ONLTeamGroupCompLinks.Count(gl => gl.teamgroup_id == tg.iid) > 0;
                    if (cbUseCompGroup.Checked)
                        tbGroupQuota.Text = CurrentCompetition.ONLTeamGroupCompLinks.First(gl => gl.teamgroup_id == tg.iid).quota_style_total.ToString();
                    else
                        tbGroupQuota.Text = "";
                }
                    
            }
            catch { }
        }

        protected void btnGroupCancel_Click(object sender, EventArgs e)
        {
            ClearTGEdit();
            panelTeams.Visible = true;
            panelGroupsSelect.Enabled = true;
            panelGroupsEdit.Visible = false;
            LoadTeamGroups();
        }

        protected void btnGroupDel_Click(object sender, EventArgs e)
        {
            try
            {
                int nIid;
                if (!int.TryParse(lblIid.Text, out nIid))
                    return;
                var toDel = dc.ONLTeamGroups.First(g => g.iid == nIid);

                foreach (var tl in toDel.ONLTeamGroupCompLinks.ToArray())
                    dc.ONLTeamGroupCompLinks.DeleteObject(tl);
                dc.SaveChanges();

                var setToUpd = toDel.ONLCompetitions.ToArray();
                for (int i = 0; i < setToUpd.Length; i++)
                    setToUpd[i].default_group = null;
                dc.SaveChanges();

                var ttu = toDel.ONLteams.ToArray();
                for (int i = 0; i < ttu.Length; i++)
                    ttu[i].group_id = null;
                dc.SaveChanges();

                dc.ONLTeamGroups.DeleteObject(toDel);
                dc.SaveChanges();

                ClearTGEdit();
                panelTeams.Visible = panelGroupsSelect.Enabled = true;
                panelGroupsEdit.Visible = false;
                LoadTeamGroups();
            }
            catch (Exception ex) { lblTGEditError.Text = "Ошибка удаления группы команд:<br />" + ex.Message; }
        }

        protected void btnGroupSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string nameToSet = tbGroupName.Text.Trim();
                if (String.IsNullOrEmpty(nameToSet))
                {
                    lblTGEditError.Text = "Название группы не введено";
                    return;
                }
                int nIid, nTmp;
                int? nQuota;
                if (int.TryParse(tbGroupQuota.Text, out nTmp))
                    nQuota = new int?(nTmp);
                else
                    nQuota = null;
                if (CurrentCompetition != null && cbUseCompGroup.Checked && nQuota == null)
                {
                    lblTGEditError.Text = "Квота не введена. Для отмены квоты введите 0";
                    return;
                }
                ONLTeamGroup gToUpdate;
                if (!int.TryParse(lblGroupIid.Text, out nIid))
                {
                    try { nIid = dc.ONLTeamGroups.OrderByDescending(g => g.iid).First().iid + 1; }
                    catch { nIid = 1; }
                    gToUpdate = ONLTeamGroup.CreateONLTeamGroup(nIid, nameToSet);
                    dc.ONLTeamGroups.AddObject(gToUpdate);
                }
                else
                {
                    gToUpdate = dc.ONLTeamGroups.First(g => g.iid == nIid);
                    gToUpdate.name = nameToSet;
                }
                
                dc.SaveChanges();

                if (this.CurrentCompetition != null && cbUseCompGroup.Checked)
                {
                    ONLTeamGroupCompLink lToUpdate;
                    try { lToUpdate = this.CurrentCompetition.ONLTeamGroupCompLinks.Where(l => l.teamgroup_id == gToUpdate.iid).First(); }
                    catch
                    {
                        long nIidL;
                        try { nIidL = dc.ONLTeamGroupCompLinks.OrderByDescending(l => l.iid).First().iid + 1; }
                        catch { nIidL = 1; }
                        lToUpdate = ONLTeamGroupCompLink.CreateONLTeamGroupCompLink(nQuota.Value, nIidL);
                        lToUpdate.comp_id = this.CurrentCompetition.iid;
                        lToUpdate.teamgroup_id = gToUpdate.iid;
                        dc.ONLTeamGroupCompLinks.AddObject(lToUpdate);
                    }
                    lToUpdate.quota_style_total = nQuota.Value;
                    dc.SaveChanges();
                    foreach (var v in gToUpdate.ONLteams.ToArray())
                    {
                        v.AddTeamToCompetition(compID);
                        foreach (var u in v.ONLusers)
                            u.AddUserToCompetition(compID);
                    }
                }
                else if (this.CurrentCompetition != null && !cbUseCompGroup.Checked)
                {
                    foreach (var v in gToUpdate.ONLteams.ToArray())
                    {
                        v.RemoveTeamFromCompetition(compID);
                        foreach (var u in v.ONLusers)
                            u.RemoveUserFromCompetition(compID);
                    }
                    foreach (var tl in CurrentCompetition.ONLTeamGroupCompLinks.Where(gl => gl.teamgroup_id == gToUpdate.iid).ToArray())
                        dc.ONLTeamGroupCompLinks.DeleteObject(tl);
                    dc.SaveChanges();
                }

                ClearTGEdit();
                panelTeams.Visible = panelGroupsSelect.Enabled = true;
                panelGroupsEdit.Visible = false;
                LoadTeamGroups();
            }
            catch (Exception ex) { lblTGEditError.Text = "Ошибка правки группы команд:<br />" + ex.Message; }
        }

        protected void gvTeamGroups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (compID < 0)
                return;
            ONLTeamGroup grp;
            try
            {
                int line = Convert.ToInt32(e.CommandArgument);
                allowTGEvent = false;
                gvTeamGroups.SelectRow(line);
                allowTGEvent = true;
                object oKey = gvTeamGroups.SelectedValue;
                if (!(oKey is int))
                    return;
                int nIid = (int)oKey;
                grp = dc.ONLTeamGroups.First(n => n.iid == nIid);
            }
            catch { return; }
            switch (e.CommandName)
            {
                case "AddToComp":
                    foreach (var v in grp.ONLteams.ToArray())
                    {
                        v.AddTeamToCompetition(compID);
                        foreach (var u in v.ONLusers.ToArray())
                            u.AddUserToCompetition(compID);
                    }
                    break;
                case "DelFromComp":
                    foreach (var v in grp.ONLteams.ToArray())
                    {
                        v.RemoveTeamFromCompetition(compID);
                        foreach (var u in v.ONLusers.ToArray())
                            u.RemoveUserFromCompetition(compID);
                    }
                    break;
            }
            LoadTeams();
        }
    }
}