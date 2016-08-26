using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebClimbing.src;
using ClimbingCompetition;
using ClimbingCompetition.Online;

namespace WebClimbing.Apps
{
    public partial class _PAppConfirm : BasePageRestrictedUser
    {
        

        private void RefreshTable()
        {
            List<ONLClimberCompLink> lnkList = new List<ONLClimberCompLink>();
            var lnkL = from l in dc.ONLClimberCompLinks
                       where l.comp_id == compID
                       && (l.state == Constants.CLIMBER_PENDING_UPDATE
                       || l.state == Constants.CLIMBER_PENDING_DELETE)
                       select l;
            if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                lnkL = from l in lnkL
                       where l.updOpIid != null
                       && l.ONLoperation.state == Constants.OP_STATE_SENT
                       && l.ONLoperation.user_id == User.Identity.Name
                       select l;
            var lnkLS = from l in lnkL
                        orderby l.ONLteam.name, l.ONLteam.iid,
                        l.ONLGroup.oldYear, l.ONLGroup.genderFemale,
                        l.ONLclimber.surname, l.ONLclimber.name
                        select l;
            foreach (var l in lnkLS)
                lnkList.Add(l);
            SetCList(lnkList);
        }

        private void SetCList(List<ONLClimberCompLink> listToSet)
        {
            var lstToSet = from l in listToSet
                           select new
                           {
                               iid = l.iid,
                               secretary_id = l.secretary_id,
                               name = l.ONLclimber.surname + " " + l.ONLclimber.name,
                               team = l.ONLteam.name,
                               age = (l.ONLclimber.age == null ? String.Empty : l.ONLclimber.age.Value.ToString()),
                               qf = l.qf,
                               grp = l.ONLGroup.name,
                               todo = l.state == Constants.CLIMBER_PENDING_DELETE ? "Удалить" :
                               (l.ONLClimberCompLink1.Count > 0 ? "Заменить " +
                               l.ONLClimberCompLink1.FirstOrDefault().ONLclimber.surname + " " +
                               l.ONLClimberCompLink1.FirstOrDefault().ONLclimber.name
                               : "Добавить"
                               ),
                               op_state = (l.ONLoperation.state == Constants.OP_STATE_NEW ? "Не отправлен" : "Не подтвержден"),
                               lead = (l.lead < 1 ? "-" : (l.lead == 1 ? "+" : "Лично")),
                               speed = (l.speed < 1 ? "-" : (l.speed == 1 ? "+" : "Лично")),
                               boulder = (l.boulder < 1 ? "-" : (l.boulder == 1 ? "+" : "Лично"))
                           };
            gvApps.DataSource = lstToSet;
            gvApps.DataBind();
            foreach (DataControlField f in gvApps.Columns)
            {
                var vR = f as CheckBoxField;
                if (vR == null)
                    continue;
                switch (vR.DataField)
                {
                    case "lead":
                        vR.Visible = clmEdit.Lead;
                        break;
                    case "speed":
                        vR.Visible = clmEdit.Speed;
                        break;
                    case "boulder":
                        vR.Visible = clmEdit.Boulder;
                        break;
                }
            }
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);


            if (!Page.IsPostBack)
            {
                try
                {
                    if (!CheckDeadline(true))
                    {
                        LblErrorMessage.Text = "Приём изменений закончен";
                        return;
                    }
                    if (!CheckDeadline(false))
                        LblErrorMessage.Text = "Приём заявок закончен. Все заявки далее будут приниматься только по согласованию с организаторами.";

                    if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                    {
                        long opIdL = Request.GetLongParam(Constants.PARAM_IID);
                        if (opIdL < 1)
                        {
                            Response.Redirect(Constants.REDIRECT_NOACCESS);
                            return;
                        }
                        var opUserList = from u in dc.ONLoperations
                                         where u.iid == opIdL
                                         && u.comp_id == compID
                                         && u.state == Constants.OP_STATE_SENT
                                         select u.user_id;
                        if (opUserList.Count() < 1)
                        {
                            Response.Redirect(Constants.REDIRECT_NOACCESS);
                            return;
                        }
                        if (opUserList.First() != User.Identity.Name)
                        {
                            Response.Redirect(Constants.REDIRECT_NOACCESS);
                            return;
                        }
                    }
                    RefreshTable();
                }
                catch (Exception ex)
                {
                    LblErrorMessage.Text = "Ошибка загрузки данных";
                    if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                        LblErrorMessage.Text += ": " + ex.Message;
                }
            }
        }

        private string getClmString(long iid)
        {
            try
            {
                var q = dc.ONLClimberCompLinks.First(l => l.iid == iid);
                return q.ONLteam.name + "|" +
                    q.ONLclimber.surname + " " + q.ONLclimber.name + "|" +
                    q.ONLclimber.age.ToString() + "|" + q.qf.ToString() + "|" +
                    q.ONLGroup.name + "| - ";
            }
            catch { return String.Empty; }
        }

        private string MessageString
        {
            get
            {
                try
                {
                    if (!User.IsInRole(Constants.ROLE_ADMIN, compID))
                        return (from u in dc.ONLusers
                                where u.iid == User.Identity.Name
                                select u.messageToSend).First();
                    else
                        return String.Empty;
                }
                catch { return String.Empty; }
            }
            set
            {
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    return;
                try
                {
                    var u = dc.ONLusers.First(us => us.iid == User.Identity.Name);
                    u.messageToSend = value;
                    dc.SaveChanges();
                }
                catch { }
            }
        }

        protected void gvApps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int rowN = Convert.ToInt32(e.CommandArgument);
                gvApps.SelectRow(rowN);

                long iid = Convert.ToInt64(gvApps.SelectedValue);

                string clmName = gvApps.SelectedRow.Cells[2].Text;
                ClimbingConfirmations cfs = new ClimbingConfirmations();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                bool reloadApps = false;
                string clmStr = getClmString(iid);
                switch (e.CommandName)
                {
                    case "ConfirmLine":
                        bool cDel, qfErr, cEdit;
                        reloadApps = true;
                        string message;
                        switch (cfs.ConfirmClimber(iid, out cDel, out cEdit,out qfErr, out message))
                        {
                            case InsertingResult.SUCCESS:
                                if (cDel)
                                {
                                    LblErrorMessage.Text = "Участник " + clmName + " успешно удалён";
                                   MessageString = MessageString +"\r\n" + clmStr + "Удаление";
                                }
                                else
                                {
                                    MessageString=MessageString+ "\r\n" + clmStr + (cEdit ? "Правка" : "Добавление");
                                    LblErrorMessage.Text = "Участник " + clmName + " успешно " + (cEdit ? "изменён" : "внесён в базу данных");
                                }
                                break;
                            case InsertingResult.QUOTA_EXCEED:
                                LblErrorMessage.Text = "Участник " + clmName + " не был добавлен по причине превышения квоты";
                                break;
                            case InsertingResult.TO_QUEUE:
                                MessageString = MessageString + "\r\n" + clmStr + "В очередь";
                                LblErrorMessage.Text = "Участник " + clmName + " был поставлен в очередь.";
                                break;
                            case InsertingResult.DUPLICATE:
                                reloadApps = false;
                                LblErrorMessage.Text = "Такой участник уже есть в БД";
                                break;
                            default:
                                reloadApps = false;
                                LblErrorMessage.Text = "Ошибка зявки учатника " + clmName;
                                break;
                        }
                        if (qfErr)
                            LblErrorMessage.Text += "; " + ClimbingConfirmations.RAZR_NOTE;
                        if (!String.IsNullOrEmpty(message))
                            LblErrorMessage.Text += "<br />" + message;
                        break;
                    case "DeleteLine":
                        var toDel = dc.ONLClimberCompLinks.First(l => l.iid == iid);
                        List<ONLClimberCompLink> tUpd = new List<ONLClimberCompLink>();
                        foreach (var v in toDel.ONLClimberCompLink1)
                            tUpd.Add(v);
                        foreach (var v in tUpd)
                            v.replacementID = null;
                        dc.SaveChanges();
                        dc.ONLClimberCompLinks.DeleteObject(toDel);
                        dc.SaveChanges();
                        LblErrorMessage.Text = "Заявка на участника " + clmName + " отменена.";
                        reloadApps = true;
                        break;
                    case "EditLine":
                        SetToEdit(iid);
                        return;
                }
                if (reloadApps)
                {
                    LongStringClass lstr = new LongStringClass(compID, MessageString);
                    Thread thr = new Thread(RepreshOp);
                    thr.Start(lstr);
                }
                RefreshTable();
            }
            catch (Exception ex)
            {
                LblErrorMessage.Text = "Ошибка правки";
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    LblErrorMessage.Text += ": " + ex.Message;
            }
        }

        class LongStringClass
        {
            public long CompID { get; private set; }
            public string Message { get; private set; }
            public LongStringClass(long compID, string message)
            {
                this.CompID = compID;
                this.Message = message;
            }
        }
        private static Mutex RefreshOpMutex = new Mutex();
        private static void RepreshOp(object prm)
        {
            RefreshOpMutex.WaitOne();
            try
            {
                LongStringClass msg = prm as LongStringClass;
                if (msg == null)
                    return;

                //List<ONLoperation> closedOp = new List<ONLoperation>();
                var closedOp = from o in staticDc.ONLoperations
                               where o.comp_id == msg.CompID
                               && o.state == Constants.OP_STATE_SENT
                               && o.ONLClimberCompLinks.Count(lnk=>lnk.state!=Constants.CLIMBER_CONFIRMED) == 0
                               select o;
                if (closedOp.Count() > 0)
                {
                    if (!String.IsNullOrEmpty(msg.Message))
                    {
                        try
                        {
                            var dbAdminList = (from ur in staticDc.ONLuserRoles
                                               where (ur.comp_id == msg.CompID && ur.role_id == Constants.ROLE_ADMIN)
                                               || ur.role_id == Constants.ROLE_ADMIN_ROOT
                                               select ur.ONLuser.email).Distinct();
                            MailService ms = new MailService(staticDc, msg.CompID);
                            foreach (var s in dbAdminList)
                            {
                                    try
                                    {
                                        string sErr;
                                        ms.SendMail(s, "Получена заявка",
                                            msg.Message, System.Net.Mail.MailPriority.Normal, out sErr);
                                    }
                                    catch { }
                            }
                        }
                        catch { }
                    }
                    string inL = "";
                    Dictionary<string, ONLuser> uList = new Dictionary<string, ONLuser>();
                    foreach (var n in closedOp)
                    {
                        n.state = Constants.OP_STATE_CONFIRMED;
                        if (inL != "")
                            inL += ",";
                        inL += n.ToString();
                        if (!uList.ContainsKey(n.user_id))
                            uList.Add(n.user_id, n.ONLuser);
                    }
                    foreach (var v in uList)
                        v.Value.messageToSend = String.Empty;
                    staticDc.SaveChanges();
                }
            }
            catch { }
            finally { RefreshOpMutex.ReleaseMutex(); }
        }

        protected void SetToEdit(long iid)
        {
            clmEdit.ClearForm();
            clmEdit.SetClimber(iid);
            panelConfApps.Enabled = false;
            panelEditClimber.Visible = true;
            hfIid.Value = iid.ToString();
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                long iid = long.Parse(hfIid.Value);
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                var uOld = dc.ONLClimberCompLinks.First(lnkClm => lnkClm.iid == iid);
                bool newClimberCreated;
                ONLclimber nCl;
                var uE = clmEdit.createClimber(uOld.team_id.ToString(), out newClimberCreated, out nCl);
                if (uE == null)
                    return;

                if (newClimberCreated && nCl.EntityState != EntityState.Detached)
                    dc.ONLclimbers.Detach(nCl);

                if (uE.EntityState != EntityState.Detached)
                    dc.ONLClimberCompLinks.Detach(uE);

                if ((uOld.ONLClimberCompLink1.Count > 0) && uOld.group_id != uE.group_id)
                {
                    LblErrorMessage.Text = "Невозможно изменить возрастную группу для этого участника";
                    return;
                }

                if (newClimberCreated)
                {
                    nCl.iid = (dc.ONLclimbers.Count() < 1) ? 1 : (dc.ONLclimbers.OrderByDescending(c => c.iid).First().iid + 1);
                    dc.ONLclimbers.AddObject(nCl);
                    uOld.climber_id = nCl.iid;
                    uOld.ONLclimber = nCl;
                }

                uOld.qf = uE.qf;
                uOld.lead = uE.lead;
                uOld.speed = uE.speed;
                uOld.boulder = uE.boulder;
                if (uE.EntityState != EntityState.Detached)
                    dc.ONLClimberCompLinks.Detach(uE);
                dc.SaveChanges();
                panelEditClimber.Visible = false;
                panelConfApps.Enabled = true;
                RefreshTable();
            }
            catch (Exception ex)
            {
                LblErrorMessage.Text = "Ошибка сохранения";
                if (User.IsInRole(Constants.ROLE_ADMIN, compID))
                    LblErrorMessage.Text += ": " + ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            panelEditClimber.Visible = false;
            panelConfApps.Enabled = true;
            RefreshTable();
        }
    }
}