using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Web.UI.WebControls;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using WebClimbing.Controls;
using WebClimbing.src;

namespace WebClimbing.Apps
{
    public partial class _PApplications : BasePageRestrictedUser
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                string sParam;
                try { sParam = CurrentCompetition.GetStringParam(Constants.PDB_COMP_ADD_APPL_INFO); }
                catch { sParam = String.Empty; }
                if (String.IsNullOrEmpty(sParam))
                    lblNotifAdd.Visible = false;
                else
                {
                    lblNotifAdd.Visible = true;
                    lblNotifAdd.Text = sParam;
                }

                bool needToSetMessage = !CheckDeadline(false);
                bool closeApp = !CheckDeadline(true) || needToSetMessage && !CurrentCompetition.GetBooleanParam(Constants.PDB_COMP_ALLOW_AFTER_DEADLINE);
                if (closeApp)
                {
                    panelClimbers.Visible = false;
                    lblMessageTop.Text = "Приём заявок на данные соревнования окончен.";
                    AppsData.Enabled = false;
                    lblNotifDeadline.Visible = false;
                    return;
                }
                else if (needToSetMessage)
                {
                    lblNotifDeadline.Visible = true;
                    lblNotifDeadline.Text = "<br />Приём заявок закончен. Дальнейшие заявки возможны только по согласованию с организаторами";
                }
                else
                    lblNotifDeadline.Visible = false;


                try
                {

                    panelConfirm.Visible = false;
                    panelClimbers.Enabled = true;

                    cbTeam.Items.Clear();
                    if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    {
                        var tList = from t in dc.ONLteams
                                    where t.ONLTeamsCompLinks.Count(l => l.comp_id == compID) > 0
                                    orderby t.name ascending
                                    select new
                                    {
                                        iid = t.iid,
                                        name = t.name
                                    };
                        cbTeam.Enabled = true;
                        foreach (var t in tList)
                            cbTeam.Items.Add(new ListItem(t.name, t.iid.ToString()));
                    }
                    else
                    {
                        MembershipUser mUser = Membership.GetUser(User.Identity.Name);
                        if (mUser is ClmUser)
                        {
                            ONLteam team = ((ClmUser)mUser).Usr.ONLteam;
                            if (team != null)
                            {
                                cbTeam.Items.Clear();
                                cbTeam.Items.Add(new ListItem(team.name, team.iid.ToString()));
                                cbTeam.SelectedValue = team.iid.ToString();
                                cbTeam.Enabled = false;
                            }
                        }
                    }
                    uncClm.btnAddAppEnabled = false;
                    ReloadData();

                    if (panelClimbers.Visible && panelClimbers.Enabled)
                    {
                        ClearForm();
                    }
                }
                catch { }
                finally
                {
                    try { cn.Close(); }
                    catch { }
                }
            }
            if (Request.Browser.EcmaScriptVersion.Major >= 1)
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered(Constants.SCRIPT_TEAM_SEL_ID))
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Constants.SCRIPT_TEAM_SEL_ID,
                        " var teamSelID = '" + cbTeam.ClientID + "'; " +
                        " var compID = " + compID.ToString() + "; ",
                        true);
                if (!Page.ClientScript.IsClientScriptBlockRegistered(Constants.SCRIPT_CLIMBER_VALIDATE))
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Constants.SCRIPT_CLIMBER_VALIDATE,
                        "<script src=\"../Scripts/MyScripts.js\" type=\"text/javascript\"></script>");
                if (!Page.ClientScript.IsClientScriptBlockRegistered("FORM_VALIDATESCRIPT"))
                {
                    string script = "<script type=\"text/javascript\"> \r\n" +
                        " function ValidateAllForms() { \r\n" +
                        "   var hasErrors = false; \r\n"+
                        "   var f; \r\n" +
                        "   var allEmpty = true; \r\n";
                    List<ClimberControl> clList = new List<ClimberControl>();
                    clList.Add(ClimberControl1);
                    clList.Add(ClimberControl2);
                    clList.Add(ClimberControl3);
                    clList.Add(ClimberControl4);
                    foreach (var c in clList)
                        script += " f = ValidateLabel('" + c.TbName_ClientID + "', '" + c.LblError_ClientID + "'); \r\n" +
                                  " if (!hasErrors && f > 0) \r\n" +
                                  "     hasErrors = true; \r\n" +
                                  " if (allEmpty && f > -1) \r\n" +
                                  "     allEmpty = false; \r\n";
                    script += " if(allEmpty) { \r\n" +
                              "     alert('Вы не ввели ни одного участника'); \r\n" +
                              "     return false; \r\n " +
                              " } \r\n" +
                              " if (hasErrors) \r\n" +
                              "     return confirm('Возможно, вы ввели ошибочные данные. Продолжить?'); \r\n" +
                              " return true; \r\n " +
                        " }; \r\n" +
                        "</script>";

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FORM_VALIDATESCRIPT", script);
                }
                if (String.IsNullOrEmpty(btnSubmitClimbers.Attributes["onclick"]))
                    btnSubmitClimbers.Attributes.Add("onclick", "return ValidateAllForms();");
            }
        }

        protected void btnSubmitClimbers_Click(object sender, EventArgs e)
        {
            List<ONLClimberCompLink> clmList = new List<ONLClimberCompLink>();
            bool res = true, newClm;
            ONLClimberCompLink u;
            if (!ClimberControl1.IsEmpty())
            {
                u = ClimberControl1.createClimber(cbTeam.SelectedValue, out newClm);
                if (u != null)
                    clmList.Add(u);
                res = res && (u != null);
            }
            if (!ClimberControl2.IsEmpty())
            {
                u = ClimberControl2.createClimber(cbTeam.SelectedValue, out newClm);
                if (u != null)
                    clmList.Add(u);
                res = res && (u != null);
            }
            if (!ClimberControl3.IsEmpty())
            {
                u = ClimberControl3.createClimber(cbTeam.SelectedValue, out newClm);
                if (u != null)
                    clmList.Add(u);
                res = res && (u != null);
            }
            if (!ClimberControl4.IsEmpty())
            {
                u = ClimberControl4.createClimber(cbTeam.SelectedValue, out newClm);
                if (u != null)
                    clmList.Add(u);
                res = res && (u != null);
            }
            
            if (res)
            {
                if (clmList.Count > 0)
                {
                    panelClimbers.Enabled = false;
                    panelConfirm.Visible = true;
                    lblMessageTop.Text = "Пожалуйста, проверьте данные и подтвердите отправку заявки";
                }
                else
                    lblMessageTop.Text = "Вы не ввели ни одного участника";
            }
        }

        private static Mutex mInsertClimber = new Mutex();
        public static void InsertClimberLink(List<ClimberLink> clmList, Entities dc, out int errCount, out string errMessage)
        {
            errCount = 0;
            errMessage = String.Empty;
            mInsertClimber.WaitOne();
            try
            {
                foreach (var clm in clmList)
                {
                    try
                    {
                        if (clm.NewClimber)
                        {
                            int newClimberIid;
                            if (dc.ONLclimbers.Count() < 1)
                                newClimberIid = 1;
                            else
                                newClimberIid = dc.ONLclimbers.OrderByDescending(c => c.iid).First().iid + 1;
                            clm.Climber.iid = newClimberIid;
                            dc.ONLclimbers.AddObject(clm.Climber);
                            clm.Link.climber_id = newClimberIid;
                            clm.Link.ONLclimber = clm.Climber;
                        }
                        long newLinkIid;
                        if (dc.ONLClimberCompLinks.Count() < 1)
                            newLinkIid = 1;
                        else
                            newLinkIid = dc.ONLClimberCompLinks.OrderByDescending(l => l.iid).First().iid + 1;
                        clm.Link.iid = newLinkIid;
                        clm.Link.secretary_id = GetNextNumber(dc, clm.Link.comp_id, clm.Link.group_id);
                        ONLGroupsCompLink lnkGr = dc.ONLGroupsCompLinks.First(l => l.comp_id == clm.Link.comp_id &&
                            l.group_id == clm.Link.group_id);
                        try { clm.Link.secretary_id = lnkGr.GetNextNumber(); }
                        catch { clm.Link.secretary_id = dc.ONLCompetitions.First(c => c.iid == clm.Link.comp_id).GetNextNumberGeneral(); }
                        dc.ONLClimberCompLinks.AddObject(clm.Link);
                        dc.SaveChanges();
                    }
                    catch
                    {
                        errCount++;
                        if (errMessage != String.Empty)
                            errMessage += ", ";
                        errMessage += clm.Link.ONLclimber.surname + " " + clm.Link.ONLclimber.name;
                    }
                }
            }
            finally { mInsertClimber.ReleaseMutex(); }
        }

        private static int GetNextNumber(Entities dc, long compID, int groupID)
        {
            var grpListQuery = from g in dc.ONLGroups
                               where g.ONLGroupsCompLinks.Count(gl => gl.comp_id == compID) > 0
                               orderby g.oldYear, g.genderFemale
                               select g;
            List<ONLGroup> gList = new List<ONLGroup>();
            foreach (var g in grpListQuery)
                gList.Add(g);
            int gPos;
            for (gPos = 0; gPos < gList.Count; gPos++)
                if (gList[gPos].iid == groupID)
                    break;
            if (gPos < 0)
                return (int)SortingClass.GetNextIID("ONLClimberCompLink", "secretary_id", staticCn, null);
            var lst = from l in dc.ONLClimberCompLinks
                      where l.comp_id == compID
                      && l.group_id == groupID
                      orderby l.comp_id descending
                      select l;
            int retVal;
            if (lst.Count() < 1)
                retVal = (1 + (gPos * 100));
            else
            {
                var lstPos = lst.First().secretary_id;
                if (lstPos % 100 == 99)
                    retVal = lstPos - 99 + gList.Count * 100;
                else
                    retVal = lstPos + 1;
            }
            if (dc.ONLClimberCompLinks.Count(ln => ln.comp_id == compID && ln.secretary_id == retVal) > 0)
                return (int)SortingClass.GetNextIID("ONLClimberCompLink", "secretary_id", staticCn, null);
            else
                return retVal;
        }

        protected void ClearForm()
        {
            lblConfMessage.Text = lblMessageTop.Text = uncClm.LblMessageText = "";
            ClimberControl1.ClearForm();
            ClimberControl2.ClearForm();
            ClimberControl3.ClearForm();
            ClimberControl4.ClearForm();
        }

        public class ClimberLink
        {
            public ONLClimberCompLink Link { get; private set; }
            public bool NewClimber { get; private set; }
            public ONLclimber Climber { get; private set; }
            public ClimberLink(ONLClimberCompLink lnk, ONLclimber clm, bool newClm)
            {
                this.Link = lnk;
                this.Climber = clm;
                this.NewClimber = newClm;
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            List<ClimberLink> uList = new List<ClimberLink>();
            ONLClimberCompLink c;
            ONLclimber outClimber;
            bool newClm;
            try
            {
                if (!ClimberControl1.IsEmpty())
                {
                    c = ClimberControl1.createClimber(cbTeam.SelectedValue, out newClm, out outClimber);
                    if (c != null)
                        uList.Add(new ClimberLink(c, outClimber, newClm));
                }
                if (!ClimberControl2.IsEmpty())
                {
                    c = ClimberControl2.createClimber(cbTeam.SelectedValue, out newClm, out outClimber);
                    if (c != null)
                        uList.Add(new ClimberLink(c, outClimber, newClm));
                }
                if (!ClimberControl3.IsEmpty())
                {
                    c = ClimberControl3.createClimber(cbTeam.SelectedValue, out newClm, out outClimber);
                    if (c != null)
                        uList.Add(new ClimberLink(c, outClimber, newClm));
                }
                if (!ClimberControl4.IsEmpty())
                {
                    c = ClimberControl4.createClimber(cbTeam.SelectedValue, out newClm, out outClimber);
                    if (c != null)
                        uList.Add(new ClimberLink(c, outClimber, newClm));
                }

                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    foreach (var v in uList)
                        v.Link.state = Constants.CLIMBER_CONFIRMED;
                else
                {
                    ONLoperation currentOp;
                    try { currentOp = dc.ONLoperations.First(op => op.user_id == User.Identity.Name && op.comp_id == compID && op.state == Constants.OP_STATE_NEW); }
                    catch
                    {
                        currentOp = ONLoperation.CreateONLoperation(
                            SortingClass.GetNextIID("ONLOperations", "iid", cn, null),
                            compID, User.Identity.Name, DateTime.UtcNow, Constants.OP_STATE_NEW);
                        dc.ONLoperations.AddObject(currentOp);
                        dc.SaveChanges();
                    }
                    foreach (var v in uList)
                    {
                        v.Link.updOpIid = currentOp.iid;
                        v.Link.ONLoperation = currentOp;
                    }
                }
                foreach (var v in uList)
                {
                    try
                    {
                        if (v.Link.EntityState != System.Data.EntityState.Detached)
                            dc.ONLClimberCompLinks.Detach(v.Link);
                    }
                    catch { }
                    try
                    {
                        if (v.NewClimber && v.Climber.EntityState != System.Data.EntityState.Detached)
                            dc.ONLclimbers.Detach(v.Climber);
                    }
                    catch { }
                }
                int nErr;
                string erMsg;
                InsertClimberLink(uList, dc, out nErr, out erMsg);

                lblMessageTop.Text = "Заявка на " + (uList.Count-nErr) + " человек принята.";
                if (nErr > 0)
                    lblMessageTop.Text += "<br />Произошли ошибки при добавлении следующих участников:<br />" + erMsg;

                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                {
                    panelConfirm.Visible = false;
                    ReloadData();
                    ClearForm();
                    panelClimbers.Enabled = true;
                    panelConfirm.Visible = false;
                }
                else
                {
                    lblMessageTop.Text += "<br />Для окончательного подтверждения заявки, после ввода всех участников, нажмите на кнопку \"" +
                                "Подтвердить все завки через e-mail\". Старший тренер Вашего региона подтвердит все введённые заявки.";
                    panelConfirm.Visible = false;
                    ReloadData();
                    uncClm.btnAddAppEnabled = true;
                    uncClm.LblMessageText = "";   
                }
            }
            catch (Exception ex)
            {
                lblConfMessage.Text = "Ошибка добавления участников";
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    lblConfMessage.Text += ": " + ex.Message;
            }
        }

        private void ReloadData()
        {
            try
            {
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                {
                    AppsData.Visible = false;
                    return;
                }
                uncClm.ReloadData();
            }
            catch { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            panelConfirm.Visible = false;
            panelClimbers.Enabled = true;
            lblMessageTop.Text = uncClm.LblMessageText = "";
        }

        protected void btnAddApp_Click(object sender, EventArgs e)
        {
            ClearForm();
            panelConfirm.Visible = false;
            panelClimbers.Enabled = true;
            uncClm.btnAddAppEnabled = false;
        }


        protected void uncClm_ConfSent(object sender, StringEventArgs e)
        {
            if (sender is ClimbersListControl)
                ((ClimbersListControl)sender).LblMessageText = "";
            lblMessageTop.Text = e.Message;
        }
    }
}
