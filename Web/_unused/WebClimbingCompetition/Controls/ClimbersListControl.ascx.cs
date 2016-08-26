//#define DBG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing.Controls
{
    public partial class ClimbersListControl : BaseControl
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected void btnConfirmEmail_Click(object sender, EventArgs e)
        {
            SendConfirmation(Constants.OP_STATE_NEW);
        }

        public string LblMessageText
        {
            get { return lblMessageTop.Text; }
            set { lblMessageTop.Text = value; }
        }

        private void GetClimbersList(out object l1, out object l2)
        {
            var listUnconfirmed = from l in dc.ONLClimberCompLinks
                                  where l.comp_id == compID
                                  && (l.state == Constants.CLIMBER_PENDING_DELETE
                                  || l.state == Constants.CLIMBER_PENDING_UPDATE)
                                  && l.updOpIid != null
                                  select l;
            if (!Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                listUnconfirmed = from l in listUnconfirmed
                                  where l.ONLoperation.user_id == Page.User.Identity.Name
                                  select l;

            var resListFull = from l in listUnconfirmed
                              orderby l.ONLGroup.oldYear, l.ONLGroup.genderFemale,
                              l.ONLteam.name, l.ONLclimber.surname, l.ONLclimber.name
                              select new
                              {
                                  Iid = l.iid,
                                  Name =
                                  ((l.state == Constants.CLIMBER_PENDING_UPDATE && l.ONLClimberCompLink1.Count > 0 &&
                                  l.ONLClimberCompLink1.FirstOrDefault().climber_id != l.climber_id) ?
                                  l.ONLClimberCompLink1.FirstOrDefault().ONLclimber.surname + " " +
                                  l.ONLClimberCompLink1.FirstOrDefault().ONLclimber.name + " => " : String.Empty) +
                                  l.ONLclimber.surname + " " + l.ONLclimber.name,
                                  Age = l.ONLclimber.age,
                                  Qf = l.qf,
                                  Team = l.ONLteam.name,
                                  Grp = l.ONLGroup.name,
                                  Lead = l.queue_Lead < 1 ? (l.lead == 1 ? "+" : (l.lead == 2 ? "Лично" : "-")) : "Оч."/* + l.queue_Lead.ToString()*/,//l.lead,
                                  Speed = l.queue_Speed < 1 ? (l.speed == 1 ? "+" : (l.speed == 2 ? "Лично" : "-")) : "Оч."/* + l.queue_Speed.ToString()*/,//l.speed,
                                  Boulder = l.queue_Boulder < 1 ? (l.boulder == 1 ? "+" : (l.boulder == 2 ? "Лично" : "-")) : "Оч."/* + l.queue_Boulder.ToString()*/,//l.boulder,
                                  State = (l.state == Constants.CLIMBER_PENDING_DELETE ? "Удаление"
                                  : (l.ONLClimberCompLink1.Count == 0 ? "Добавление" : "Изменение")),
                                  OpState = l.ONLoperation.state
                              };

            var lstClmUnSent = from l in resListFull
                               where l.OpState == Constants.OP_STATE_NEW
                               select l;
            var lstClmSent = from l in resListFull
                             where l.OpState == Constants.OP_STATE_SENT
                             select l;
            l1 = lstClmUnSent;
            l2 = lstClmSent;
        }

        public void ReloadData()
        {

            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                object clmList, clmList2;

                GetClimbersList(out clmList, out clmList2);
                //gvUnsent.Columns.Clear();
                gvUnsent.DataSource = clmList;
                gvUnsent.DataBind();

                //ButtonField bf = new ButtonField();
                //bf.CommandName = "Delete";
                //bf.Text = "Отмена";
                //bf.ButtonType = ButtonType.Button;
                //gvUnsent.Columns.Insert(gvUnsent.Columns.Count, bf);

                //gvUnsent.Columns[gvUnconfirmed.Columns.Count - 2].Visible = false;

                btnConfirmEmail.Enabled = btnCancelAll.Enabled = (gvUnsent.Rows.Count > 0);

                
               
                gvUnconfirmed.DataSource = clmList2;
                gvUnconfirmed.DataBind();
                if (!Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                    try { gvUnconfirmed.Columns[gvUnconfirmed.Columns.Count - 1].Visible = false; }
                    catch { }
                //int nTmp = 0;
                //if (Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                //{
                //    nTmp = 1;
                //    gvUnconfirmed.Columns.Insert(gvUnconfirmed.Columns.Count, bf);
                //}
                //gvUnconfirmed.Columns[gvUnconfirmed.Columns.Count - 1 - nTmp].Visible = false;
                btnConfirmAgain.Visible = (gvUnconfirmed.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                if (Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                    lblMessageTop.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void SendConfirmation(int opState)
        {
            lblMessageTop.Text = "";
            try
            {
                var uList = from r in dc.ONLoperations
                            where r.ONLuser.iid == Page.User.Identity.Name &&
                            r.comp_id == compID &&
                            r.state == opState
                            orderby r.op_date descending
                            select r;
                if (uList.Count() < 1)
                {
                    lblMessageTop.Text = "Нет заявок для отправки";
                    return;
                }
                var opId = uList.First();
                if (string.IsNullOrEmpty(opId.ONLuser.email))
                {
                    lblMessageTop.Text = "Не указан контактный E-mail. Обратитесь к Администратору.";
                    return;
                }
                string url = "http://" + Request.Url.Authority + "/Apps/AppConfirm.aspx?" + Constants.PARAM_IID + "=" + opId.iid.ToString() +
                    "&" + Constants.PARAM_COMP_ID + "=" + compID.ToString();
                

                
#if !DBG
                string erMsg;
                if (this.BasePage.mailService.SendMail(opId.ONLuser.email, "Новая заявка",
                    "Добрый день,\r\n" +
                    "От Вашего региона была подана заявка на " + BasePage.CompetitionName + ".\r\n" +
                    "Для подтверждения или отмены заявки пожалуйста, пройдите по указанной в письме ссылке.\r\n" +
                    " " + url + " ", System.Net.Mail.MailPriority.Normal, out erMsg))
                {
#endif
                if (opState == Constants.OP_STATE_NEW)
                {
                    var opList = from o in dc.ONLoperations
                                 where o.comp_id == compID
                                 && o.user_id == Page.User.Identity.Name
                                 && o.state == Constants.OP_STATE_NEW
                                 select o;
                    foreach (var v in opList.ToArray())
                        v.state = Constants.OP_STATE_SENT;
                    dc.SaveChanges();
                }
                
#if !DBG
                }
#endif
                ReloadData();


                lblMessageTop.Text = "Подтверждение отправлено успешно.";
                if (ConfirmationSent != null)
                    ConfirmationSent(this, new StringEventArgs(lblMessageTop.Text));
            }
            catch
            {
                lblMessageTop.Text = "Ошибка отправки подтверждения";
            }
        }

        protected void btnConfirmAgain_Click(object sender, EventArgs e)
        {
            SendConfirmation(Constants.OP_STATE_SENT);
        }

        public bool btnAddAppEnabled
        {
            get { return btnAddApp.Enabled; }
            set { btnAddApp.Enabled = value; }
        }

        public event EventHandler<EventArgs> btnAddAppClick;

        public event EventHandler<StringEventArgs> ConfirmationSent;

        protected void btnAddApp_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAddAppClick != null)
                    btnAddAppClick(this, e);
            }
            catch { }
        }

        protected void btnCancelAll_Click(object sender, EventArgs e)
        {
            try
            {
                var uncOpList = from o in dc.ONLoperations
                                where (o.ONLuser.iid == Page.User.Identity.Name &&
                                       o.comp_id == compID &&
                                       o.state == Constants.OP_STATE_NEW)
                                select o;
                List<ONLoperation> idList = new List<ONLoperation>();
                foreach (var l in uncOpList)
                    idList.Add(l);
                foreach (var op in idList)
                {
                    while (op.ONLClimberCompLinks.Count > 0)
                    {
                        var lnk = op.ONLClimberCompLinks.First();
                        RemoveUnconfirmedLink(lnk);
                    }
                    dc.ONLoperations.DeleteObject(op);
                }
                dc.SaveChanges();

                lblMessageTop.Text = "Неотправленные заявки удалены.";
                ReloadData();
                if (rowDeleted != null)
                    rowDeleted(gvUnsent, new EventArgs());
            }
            catch
            {
                lblMessageTop.Text = "Ошибка отмены заявок";
            }
        }

        private void RemoveUnconfirmedLink(ONLClimberCompLink lnk)
        {
            while (lnk.ONLClimberCompLink1.Count > 0)
            {
                var lInner = lnk.ONLClimberCompLink1.First();
                lInner.replacementID = null;
                lInner.ONLClimberCompLink2 = null;
                lnk.ONLClimberCompLink1.Remove(lInner);
            }
            dc.ONLClimberCompLinks.DeleteObject(lnk);
        }

        public event EventHandler<EventArgs> rowDeleted;

        protected void gvUnsent_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                if (e.Keys.Count < 1)
                    return;
                foreach (object obj in e.Keys)
                    if (obj is DictionaryEntry)
                    {
                        DictionaryEntry de = (DictionaryEntry)obj;
                        if (de.Key.ToString().ToLower() != "iid")
                            continue;
                        long nIid;
                        if (!long.TryParse(de.Value.ToString(), out nIid))
                            continue;
                        ONLClimberCompLink lnkToDel;
                        try { lnkToDel = dc.ONLClimberCompLinks.First(l => l.iid == nIid); }
                        catch { continue; }
                        RemoveUnconfirmedLink(lnkToDel);
                    }
                dc.SaveChanges();
                ReloadData();
                lblMessageTop.Text = "";
                if (rowDeleted != null)
                    rowDeleted(gvUnsent, new EventArgs());
            }
            catch (Exception ex)
            {
                if (Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                    lblMessageTop.Text = "Ошибка удаления участника: " + ex.Message;
            }
        }

        protected void gvUnconfirmed_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (!Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                return;
            lblMessageTop.Text = "";
            if (rowDeleted != null)
                rowDeleted(gvUnconfirmed, new EventArgs());
        }
    }

    public sealed class StringEventArgs : EventArgs
    {
        private string msg;
        public string Message { get { return msg; } }
        public StringEventArgs(string message)
        {
            this.msg = message;
        }
    }
}