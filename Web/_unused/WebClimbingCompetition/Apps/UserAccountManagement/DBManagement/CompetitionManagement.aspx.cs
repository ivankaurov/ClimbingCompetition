using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing.Apps.UserAccountManagement.DBManagement
{
    public partial class CompetitionManagement : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.NoRedirecting = true;
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;
            LoadCompList();
        }

        private void LoadCompList()
        {
            List<ONLCompetition> cList = new List<ONLCompetition>();
            foreach (var c in dc.ONLCompetitions)
                cList.Add(c);

            cList.Sort((a, b) => b.GetDateParam(Constants.PDB_COMP_START_DATE).CompareTo(a.GetDateParam(Constants.PDB_COMP_START_DATE)));
            gvComps.DataSource = cList;
            gvComps.DataBind();
        }

        private ONLCompetition GetCompByIid(long iid)
        {
            try { return dc.ONLCompetitions.First(c => c.iid == iid); }
            catch { return null; }
        }

        private void ClearCompData()
        {
            
            lblErrMsg.Text = lblCompIid.Text = tbCompPlace.Text = tbCompTitle.Text = tbCompShort.Text = String.Empty;
            clndCorrectionDeadLine.SelectedDate = clndDeadLine.SelectedDate = clndFrom.SelectedDate = clndTo.SelectedDate = Constants.NowDate;
            cbAllowAfterDeadline.Checked = false;
            foreach (ListItem v in cbListStyles.Items)
                v.Selected = false;
        }

        protected void gvComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            var oKey = gvComps.SelectedValue;
            long curCompSel;
            try { curCompSel = Convert.ToInt64(oKey); }
            catch { return; }
            var curComp = GetCompByIid(curCompSel);
            if (curComp == null)
                return;
            panelComps.Visible = false;
            panelSelectedComp.Visible = true;

            lblCompIid.Text = curComp.iid.ToString();
            tbCompTitle.Text = curComp.name;
            tbCompShort.Text = curComp.short_name;
            tbCompPlace.Text = curComp.place;
            lblErrMsg.Text = String.Empty;

            clndFrom.SelectedDate = curComp.GetDateParam(Constants.PDB_COMP_START_DATE).ToUniversalTime();
            clndFrom.DataBind();
            clndTo.SelectedDate = curComp.GetDateParam(Constants.PDB_COMP_END_DATE).ToUniversalTime();
            clndFrom.DataBind();

            clndDeadLine.SelectedDate = curComp.GetDateParam(Constants.PDB_COMP_DEADLINE).ToUniversalTime();
            clndCorrectionDeadLine.SelectedDate = curComp.GetDateParam(Constants.PDB_COMP_DEADLINE_CHANGE).ToUniversalTime();

            cbAllowAfterDeadline.Checked = curComp.GetBooleanParam(Constants.PDB_COMP_ALLOW_AFTER_DEADLINE);

            string styles = curComp.GetStringParam(Constants.PDB_COMP_STYLES);
            foreach (ListItem li in cbListStyles.Items)
                li.Selected = (styles.IndexOf(li.Value) > -1);
        }

        protected void btnNewComp_Click(object sender, EventArgs e)
        {
            ClearCompData();
            panelComps.Visible = false;
            panelSelectedComp.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            panelComps.Visible = true;
            panelSelectedComp.Visible = false;
            ClearCompData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string sStyles = String.Empty;
                ONLCompetition comp;
                if (String.IsNullOrEmpty(lblCompIid.Text))
                    comp = ONLCompetition.CreateONLCompetition(SortingClass.GetNextIID("ONLCompetitions", "iid", cn, null));
                else
                    comp = GetCompByIid(long.Parse(lblCompIid.Text));
                if (comp == null)
                {
                    lblErrMsg.Text = "Ошибка создания объекта";
                    return;
                }
                if (String.IsNullOrWhiteSpace(tbCompPlace.Text))
                {
                    lblErrMsg.Text = "Место проведения не введено";
                    return;
                }
                if (String.IsNullOrWhiteSpace(tbCompShort.Text))
                {
                    lblErrMsg.Text = "Краткое наименование соревнований не введено";
                    return;
                }
                if (String.IsNullOrWhiteSpace(tbCompTitle.Text))
                {
                    lblErrMsg.Text = "Наименование соревнований не введено";
                    return;
                }
                foreach (ListItem li in cbListStyles.Items)
                    if (li.Selected)
                        sStyles += li.Value;
                if (sStyles.Equals(String.Empty))
                {
                    lblErrMsg.Text = "Виды не выбраны";
                    return;
                }
                CultureInfo ci = new CultureInfo("ru-RU");
                string dateTotal = String.Empty;
                DateTime from = clndFrom.SelectedDate, to = clndTo.SelectedDate;
                if (from.Equals(to))
                    dateTotal = from.ToString("d MMMM yyyy", ci);
                else if (from.Year.Equals(to.Year))
                {
                    if (from.Month.Equals(to.Month))
                        dateTotal = from.Day.ToString() + " - " + to.ToString("d MMMM yyyy", ci) + " г.";
                    else
                        dateTotal = from.ToString("d MMMM", ci) + " - " +
                            to.ToString("d MMMM yyyy", ci) + " г.";
                }
                else
                    dateTotal = from.ToString("d MMMM yyyy", ci) + " г. - " +
                        to.ToString("d MMMM yyyy", ci) + " г.";

                comp.name = tbCompTitle.Text;
                comp.short_name = tbCompShort.Text;
                comp.place = tbCompPlace.Text;
                comp.date = dateTotal;

                long? nextIid = null;
                nextIid = comp.SetDateParam(Constants.PDB_COMP_START_DATE, from, nextIid);
                nextIid = (nextIid <= 0 ? null : new long?(nextIid.Value + 1));

                nextIid = comp.SetDateParam(Constants.PDB_COMP_END_DATE, to, nextIid);
                nextIid = (nextIid <= 0 ? null : new long?(nextIid.Value + 1));

                nextIid = comp.SetDateParam(Constants.PDB_COMP_DEADLINE, clndDeadLine.SelectedDate, nextIid);
                nextIid = (nextIid <= 0 ? null : new long?(nextIid.Value + 1));

                nextIid = comp.SetDateParam(Constants.PDB_COMP_DEADLINE_CHANGE, clndCorrectionDeadLine.SelectedDate, nextIid);
                nextIid = (nextIid <= 0 ? null : new long?(nextIid.Value + 1));

                nextIid = comp.SetStringParam(Constants.PDB_COMP_STYLES, sStyles, nextIid);
                nextIid = (nextIid <= 0 ? null : new long?(nextIid.Value + 1));

                nextIid = comp.SetObjectParam(Constants.PDB_COMP_ALLOW_AFTER_DEADLINE,
                    cbAllowAfterDeadline.Checked, (o => ((bool)o).ToString()), nextIid);

                if (String.IsNullOrWhiteSpace(lblCompIid.Text))
                    dc.ONLCompetitions.AddObject(comp);
                dc.SaveChanges();
                LoadCompList();
                lblErrMsg.Text = "Соревнования созданы/обновлены";
                panelComps.Visible = !(panelSelectedComp.Visible = false);
            }
            catch (Exception ex)
            {
                lblErrMsg.Text = "Ошибка обновления соревнований:\r\n" + ex.ToString();
            }
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(lblCompIid.Text))
                return;
            try
            {
                long cIid = long.Parse(lblCompIid.Text);
                var curComp = dc.ONLCompetitions.First(c => c.iid == cIid);
                if (curComp == null)
                    throw new Exception("Нет таких соренований");
                dc.ONLCompetitions.DeleteObject(curComp);
                dc.SaveChanges();
                lblErrMsg.Text = "Соревнований удалены";
                LoadCompList();
                panelSelectedComp.Visible = !(panelComps.Visible = true);
            }
            catch (Exception ex)
            {
                lblErrMsg.Text = "Ошибка удаления соревнований: " + ex.Message;
            }
        }
    }
}