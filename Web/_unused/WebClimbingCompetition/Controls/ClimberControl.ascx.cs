using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;
using WebClimbing.Apps;

namespace WebClimbing.Controls
{
    public partial class ClimberControl : BaseControl
    {
        private bool? _lead = null, _speed = null, _boulder = null;

        public bool Lead
        {
            get
            {
                if(_lead == null)
                    try { _lead = this.BasePage.CurrentCompetition.Lead(); }
                    catch { return false; }
                return _lead.Value;
            }
        }
        public bool Speed
        {
            get
            {
                if (_speed == null)
                    try { _speed = this.BasePage.CurrentCompetition.Speed(); }
                    catch { return false; }
                return _speed.Value;
            }
        }
        public bool Boulder
        {
            get
            {
                if (_boulder == null)
                    try { _boulder = this.BasePage.CurrentCompetition.Boulder(); }
                    catch { return false; }
                return _boulder.Value;
            }
        }

        public int? CurrentClimber
        {
            get
            {
                int val;
                if (int.TryParse(hfIid.Value, out val))
                    return val;
                return null;
            }
        }

        private void SetEnabled()
        {
            try
            {
                cbSelectLead.Enabled = Lead;
                cbSelectSpeed.Enabled = Speed;
                cbSelectBoulder.Enabled = Boulder;
                //cbl1.Items[0].Enabled = Lead;
                //cbl1.Items[1].Enabled = Speed;
                //cbl1.Items[2].Enabled = Boulder;
            }
            catch { }
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request.Browser.EcmaScriptVersion.Major >= 1)
            {
                if (String.IsNullOrEmpty(tbName1.Attributes["onkeyup"]))
                    tbName1.Attributes.Add("onkeyup", 
                        "try{ suggestClimber('" + tbName1.ClientID + "','" + tbAge1.ClientID + "','" + cbGender1.ClientID + "','" + cbQf1.ClientID + "'); } catch(err44) { }");
                
                AddOnBlurAttr(tbName1);
                AddOnBlurAttr(tbAge1);
                AddOnBlurAttr(cbGender1);
            }
            if (!Page.IsPostBack)
                SetEnabled();
        }

        private void AddOnBlurAttr(WebControl c)
        {
            if (String.IsNullOrEmpty(c.Attributes["onblur"]))
            {
                int nThreshold = this.Page is _PApplications ? 0 : 1;
                c.Attributes.Add("onblur",
                    " try { " +
                    "   ValidateForm('" + TbName_ClientID + "', '" + TbAge_ClientID + "', '" + CbGender_ClientID + "', '" + LblError_ClientID + "'," + nThreshold.ToString() + "); " +
                    " } catch(errBlur) { }");
            }
        }

        public string ErrMessage
        {
            get { return lblMessage1.Text; }
            set { lblMessage1.Text = value; }
        }

        private void ClearCbList()
        {
            try
            {
                cbSelectLead.SelectedIndex = Lead ? 1 : 0;
                cbSelectSpeed.SelectedIndex = Speed ? 1 : 0;
                cbSelectBoulder.SelectedIndex = Boulder ? 1 : 0;
                //cbl1.Items[0].Selected = Lead;
                //cbl1.Items[1].Selected = Speed;
                //cbl1.Items[2].Selected = Boulder;
                SetEnabled();
            }
            catch { }
        }

        public void ClearForm()
        {
            hfTeamID.Value = hfIid.Value = tbName1.Text = tbAge1.Text = lblMessage1.Text = lblGroup.Text = "";
            cbGender1.SelectedValue = "0";
            cbQf1.SelectedIndex = 0;
            ClearCbList();
        }

        public bool IsEmpty()
        {
            return (String.IsNullOrEmpty(tbName1.Text));
        }

        public bool NameEnabled { get { return tbName1.Enabled; } set { tbName1.Enabled = value; } }
        public bool AgeEnabled { get { return tbAge1.Enabled; } set { tbAge1.Enabled = value; } }
        public bool GenderEnabled { get { return cbGender1.Enabled; } set { cbGender1.Enabled = value; } }
        public bool QfEnabled { get { return cbQf1.Enabled; } set { cbQf1.Enabled = value; } }
        public bool StylesEnabled { get { return pnlStyles.Enabled; } set { pnlStyles.Enabled = value; } }

        public TextBox NameInputBox { get { return tbName1; } }

        public ONLClimberCompLink GetClimberByIid(long iid)
        {
            try { return dc.ONLClimberCompLinks.First(c => c.iid == iid); }
            catch { return null; }
        }
        public void SetClimber(long link_id)
        {
            SetClimber(GetClimberByIid(link_id));
        }
        public void SetClimber(ONLClimberCompLink lnk)
        {
            try
            {
                if (lnk == null)
                {
                    lblMessage1.Text = "Участник не найден";
                    return;
                }
                ONLclimber clm = lnk.ONLclimber;
                hfIid.Value = lnk.iid.ToString();
                hfTeamID.Value = lnk.team_id.ToString();
                tbName1.Text = clm.surname + " " + clm.name;
                tbAge1.Text = clm.age.ToString();
                cbGender1.SelectedValue = (clm.genderFemale ? "1" : "0");
                cbQf1.SelectedValue = lnk.qf;
                cbSelectLead.SelectedIndex = lnk.lead;
                cbSelectSpeed.SelectedIndex = lnk.speed;
                cbSelectBoulder.SelectedIndex = lnk.boulder;
                //cbl1.Items[0].Selected = Lead && lnk.lead;
                //cbl1.Items[1].Selected = Speed && lnk.speed;
                //cbl1.Items[2].Selected = Boulder && lnk.boulder;
                lblGroup.Text = lnk.ONLGroup.name;
                SetEnabled();
            }
            catch (Exception ex)
            {
                lblMessage1.Text = "Ошибка установки участника";
                if (Page.User.IsInRole("ADM"))
                    lblMessage1.Text += ": " + ex.Message;
            }
        }

        public ONLClimberCompLink createClimber(string teamId, out bool newClimber)
        {
            ONLclimber clmTmp;
            return createClimber(teamId, out newClimber, out clmTmp);
        }

        public ONLClimberCompLink createClimber(string teamId, out bool newClimber, out ONLclimber createdClm)
        {
            createdClm = null;
            newClimber = false;
            string errMessage = "";
            try
            {
                int age;
                if (!int.TryParse(tbAge1.Text, out age))
                {
                    errMessage = "Год введён неправильно.";
                    return null;
                }
                if (age < 0)
                {
                    errMessage = "Год введён неправильно.";
                    return null;
                }
                if (age <= 20)
                    age += 2000;
                if (age <= 99)
                    age += 1900;
                if (age > DateTime.Now.Year)
                {
                    errMessage = "Год введён неправильно.";
                    return null;
                }
                string surname, name, srcName;
                srcName = tbName1.Text.Trim();
                while (srcName.IndexOf("  ") > -1)
                    srcName = srcName.Replace("  ", " ");
                srcName = srcName.Replace('ё', 'е');
                tbName1.Text = srcName;
                if (String.IsNullOrEmpty(srcName))
                {
                    errMessage = "Фамилия не введена";
                    return null;
                }
                int n = srcName.IndexOf(' ');
                if (n < 0)
                {
                    surname = srcName;
                    name = String.Empty;
                }
                else
                {
                    surname = srcName.Substring(0, n);
                    try { name = srcName.Substring(n + 1); }
                    catch { name = String.Empty; }
                }
                bool genderFemale = (cbGender1.SelectedValue == "1");
                var bp = Page as BasePage;
                long? grpID;
                if (bp != null)
                    grpID = bp.CurrentCompetition.GetClimbersGroup(age, genderFemale);
                else
                    grpID = dc.ONLCompetitions.Where(c => c.iid == compID).First().GetClimbersGroup(age, genderFemale);

                if (grpID == null || !grpID.HasValue)
                {
                    errMessage = "Участник не входит ни в одну возрастную группу";
                    return null;
                }
                ONLGroup gr = dc.ONLGroupsCompLinks.Where(grl => grl.iid == grpID.Value).First().ONLGroup;
                lblGroup.Text = gr.name;

                string t_id = (String.IsNullOrEmpty(hfTeamID.Value)  ? teamId : hfTeamID.Value);

                int teamIDToSet;
                if (!int.TryParse(t_id, out teamIDToSet))
                    teamIDToSet = -1;

                ONLteam teamToSet;
                try { teamToSet = dc.ONLteams.First(t => t.iid == teamIDToSet); }
                catch { teamToSet = null; }
                if (teamToSet == null)
                {
                    errMessage = "Команда выбрана не верно";
                    return null;
                }
                try
                {
                    createdClm = dc.ONLclimbers.First(c =>
                    c.surname == surname && c.name == name && c.age == age);
                    createdClm.surname = surname;
                    createdClm.name = name;
                    dc.SaveChanges();
                }
                catch
                {
                    createdClm = ONLclimber.CreateONLclimber(0, genderFemale, new DateTime(age, 1, 5), false, false);
                    createdClm.name = name;
                    createdClm.surname = surname;
                    newClimber = true;
                }

                if (cbSelectLead.SelectedIndex < 0)
                    cbSelectLead.SelectedIndex = 0;
                if (cbSelectSpeed.SelectedIndex < 0)
                    cbSelectSpeed.SelectedIndex = 0;
                if (cbSelectBoulder.SelectedIndex < 0)
                    cbSelectBoulder.SelectedIndex = 0;

                string sQf = cbQf1.SelectedValue == null ? String.Empty : cbQf1.SelectedValue;
                ONLClimberCompLink lnk = ONLClimberCompLink.CreateONLClimberCompLink(
                iid: 0,
                    climber_id: createdClm.iid,
                    comp_id: compID,
                    secretary_id: 0,
                    group_id: gr.iid,
                    team_id: teamIDToSet,
                    qf: sQf,
                    lead: (short)(Lead ? cbSelectLead.SelectedIndex : 0),
                    speed: (short)(Speed ? cbSelectSpeed.SelectedIndex : 0),// (Speed && cbl1.Items[1].Selected),
                    boulder: (short)(Boulder ? cbSelectBoulder.SelectedIndex : 0),// (Boulder && cbl1.Items[2].Selected),
                    vk: false,
                    nopoints: false,
                    appl_type: String.Empty,
                    is_changeble: true,
                    queue_pos: 0,
                    sys_date_create: DateTime.UtcNow,
                    sys_date_update: DateTime.UtcNow,
                    state: Constants.CLIMBER_PENDING_UPDATE,
                    queue_Lead: 0,
                    queue_Speed: 0,
                    queue_Boulder: 0);
                lnk.ONLclimber = createdClm;
                try
                {
                    if (lnk.EntityState != EntityState.Detached)
                        dc.ONLClimberCompLinks.Detach(lnk);
                }
                catch { }
                try
                {
                    if (newClimber && lnk.ONLclimber.EntityState != EntityState.Detached)
                        dc.ONLclimbers.Detach(lnk.ONLclimber);
                }
                catch { }

                

                //if (!String.IsNullOrEmpty(hfIid.Value))
                //{
                //    int nIid;
                //    if (int.TryParse(hfIid.Value, out nIid))
                //        lnk.iid = nIid;
                //}

                if (lnk.lead == 0 && lnk.speed == 0 && lnk.boulder == 0)
                {
                    errMessage = "Не выбраны виды для участия.";
                    return null;
                }
                return lnk;
            }
            catch (Exception ex)
            {
                errMessage = "Ошибка создания объекта";
                if (Page.User.IsInRole(Constants.ROLE_ADMIN, compID))
                    errMessage += ": " + ex.Message;
                return null;
            }
            finally { lblMessage1.Text = errMessage; }
        }

        public string TbName_ClientID { get { return tbName1.ClientID; } }
        public string TbAge_ClientID { get { return tbAge1.ClientID; } }
        public string CbGender_ClientID { get { return cbGender1.ClientID; } }
        public string LblError_ClientID { get { return lblMessage1.ClientID; } }
    }
}