// <copyright file="Secretary.cs">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Linq;
using System.Windows.Forms;
using ClimbingCompetition.dsClimbingTableAdapters;
using Excel = Microsoft.Office.Interop.Excel;
using ClimbingCompetition.WebServices;
using System.Globalization;
using XmlApiClient;
using XmlApiData;
using System.Net;
using ClimbingCompetition.Client;
using Extensions;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма "приём заявок". Тут вводятся данные о группах/командах/участниках
    /// </summary>
    public partial class Secretary : BaseForm
    {
        private static DataTable RegTypeTable;
        private static DataGridViewCell DefCellTemplate;
        static Secretary()
        {
            RegTypeTable = new DataTable();
            RegTypeTable.Columns.Add("value", typeof(short));
            RegTypeTable.Columns.Add("text", typeof(string));
            RegTypeTable.Rows.Add(new object[] { 0, "-" });
            RegTypeTable.Rows.Add(new object[] { 2, "Лично" });
            RegTypeTable.Rows.Add(new object[] { 1, "+" });

            DataGridViewComboBoxColumn cbClmn = new DataGridViewComboBoxColumn();
            DefCellTemplate = cbClmn.CellTemplate;
        }

        public static string SCR_ALL_GROUPS = "-все группы-";
        public static string SCR_ALL_TEAMS = "-все команды-";
        public static string SCR_DEFAULT_GROUP = "Определить автоматически";
        public static string DELETE = "Удалить";
        public static string CANCEL = "Отмена";
        public static string SUBMIT = "Подтвердить";
        public static string EDIT = "Правка";
        public static string NYA = "н/я";
        public static string DSQ = "дискв.";

        public static string SCR_RANK_LEAD_COL = "Р.Тр.";
        public static string SCR_RANK_SPEED_COL = "Р.Ск.";
        public static string SCR_RANK_BOULDER_COL = "Р.Б.";
        public static string BIB = "Номер";
        public static string SURNAME = "Фамилия";
        public static string NAME = "Имя";
        public static string TEAM_ST = "Команда";
        public static string AGE_ST = "Г.р.";
        public static string QF_ST = "Разряд";
        public static string GROUP = "Группа";
        public static string LEAD = "Трудность";
        public static string SPEED = "Скорость";
        public static string BOULDER = "Боулдеринг";
        public static string LEAD_S = "Тр.";
        public static string SPEED_S = "Ск.";
        public static string BOULDER_S = "Боулд.";
        public static string SCR_NEW_BIB = "Нов.номер";
        public static string SCR_FEMALE = "ж";
        public static string BIB_F = "Инд.№";
        public static string CLIMBER_NAME = "Фамилия, Имя";
#if !FULL
        private const string rankingColumn = "Рейт.";
#endif

        //SqlConnection cn;
        string dir;
        DataTable dataTable = new DataTable();
        bool allowGuest, changed = false, allowRem = true;
        private SqlConnection remote = null;

        private long? compIDForService = null;

        public override void LoadLocalizedStrings(CultureInfo ci)
        {
            StaticClass.LoadInitialStrings("ClimbingCompetition.LocalizedStrings", this, ci);
        }

        XmlClient client;

        public Secretary(SqlConnection baseCon, string competitionTitle, string dir)
            : base(baseCon, competitionTitle, null)
        {

            InitializeComponent();

            AccountForm.CreateTeamSecondLinkTable(cn, null);

            //this.cn = cn;
            //try { this.cn.Open(); }
            //catch { }
            RefreshCbGroups();
            RefreshCbTeams();

            this.competitionTitle = competitionTitle;
            this.dir = dir;
            try
            {
                if (this.cn.State != ConnectionState.Open)
                    this.cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teams(NOLOCK) WHERE Guest = 0", this.cn);
                cmd.ExecuteScalar();
                allowGuest = true;
            }
            catch { allowGuest = false; }
            SetEditModeGroups(false);
            SetEditModeTeams(false);
            SetEditModeParts(false);
            cbGuest.Visible = allowGuest;
            AccountForm.CreateQfTable(cn);
            SortingClass.CheckColumn("Groups", "minQf", "INT NOT NULL DEFAULT 12", this.cn);
            SortingClass.CheckColumn("Groups", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("Teams", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("Participants", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("Teams", "chief", "VARCHAR(255) NOT NULL DEFAULT ''", this.cn);
            SortingClass.CheckColumn("Teams", "chief_ord", "VARCHAR(255) NOT NULL DEFAULT ''", this.cn);
            SortingClass.CheckColumn("Participants", "name_ord", "VARCHAR(255) NOT NULL DEFAULT ''", this.cn);
            SortingClass.CheckColumn("Participants", "license", "BIGINT NULL", this.cn);
            SortingClass.CheckGlobalIDColumns(this.cn);

            try
            {
                AccountForm.AlterParticipantsTable(cn, null);
                SqlCommand cm = new SqlCommand();
                cm.Connection = this.cn;
                cm.CommandText = "SELECT qf FROM qfList(NOLOCK) ORDER BY iid";
                SqlDataReader drq = cm.ExecuteReader();
                try
                {
                    while (drq.Read())
                    {
                        cbPartQf.Items.Add(drq[0].ToString());
                        cbGroupsQf.Items.Add(drq[0].ToString());
                    }
                }
                finally { drq.Close(); }
                cm.CommandText = "SELECT ISNULL(remoteString,'') rs FROM CompetitionData(NOLOCK)";
                string rcs = cm.ExecuteScalar().ToString();
                if (!String.IsNullOrEmpty(rcs))
                {
                    XmlClientWrapper wrapper;
                    compIDForService = StaticClass.ParseServString(rcs, out wrapper);
                    if (wrapper != null)
                    {
                        client = wrapper.CreateClient(cn);
                        compIDForService = wrapper.CompetitionId;
                    }
                    if (compIDForService == null || !compIDForService.HasValue)
                        remote = new SqlConnection(rcs);
                }
            }
            catch { }
            SortingClass.CheckColumn("teams", "pos", "INT NULL", cn);
            btnGroupsInet.Visible = btnTeamsInet.Visible = btnInetImport.Visible = gbInetPhoto.Visible = ServiceClient.GetInstance(cn).ConnectionSet;

            //cbPartGroups.SelectedIndex = 0;
#if !DEBUG
            btnGroupsInet.Enabled = btnImportRankings.Enabled =
                btnGroupsInet.Visible = btnImportRankings.Visible = false;
#endif
#if !FULL
            lblRankingBoulder.Visible = tbPartBoulder.Visible = lblRankingLead.Visible = tbPartLead.Visible = false;
#endif
        }

        #region Groups
        bool addGroups = false;

        private void SetEditModeGroups(bool edit)
        {
            cbGroupsQf.Enabled = cbGroupsSex.Enabled = edit;
            btnGroupsAdd.Enabled = cbGroupsSelect.Enabled =
                tbGroupsName.ReadOnly = tbGroupsOld.ReadOnly = tbGroupsYoung.ReadOnly =
                btnGroupsInet.Enabled = btnImportRankings.Enabled = !edit;
            if (edit)
            {
                btnGroupsDel.Text = CANCEL;
                btnGroupsEdit.Text = SUBMIT;
            }
            else
            {
                btnGroupsDel.Text = DELETE;
                btnGroupsEdit.Text = EDIT;
            }
        }

        private void RefreshCbGroups()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT name FROM groups ORDER BY oldYear, genderFemale";
            SqlDataReader rdr = cmd.ExecuteReader();
            cbGroupsSelect.Items.Clear();
            cbPartGroups.Items.Clear();
            cbPartGroups.Items.Add(SCR_ALL_GROUPS);
            while (rdr.Read())
            {
                cbGroupsSelect.Items.Add(rdr[0].ToString());
                cbPartGroups.Items.Add(rdr[0].ToString());
            }
            rdr.Close();
            cbGroupsSelect.SelectedIndex = -1;
        }

        private void ClearGroups()
        {
            tbGroupsName.Text = tbGroupsOld.Text = tbGroupsYoung.Text = tbGroupsIID.Text = "";
            cbGroupsQf.SelectedIndex = cbGroupsSex.SelectedIndex = cbGroupsSelect.SelectedIndex = -1;
            cbGroupsQf.Text = "";
            cbGroupsSelect.Text = "Выберите группу";
            cbGroupsSex.Text = "Пол";
        }

        private void cbGroupsSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGroupsSelect.SelectedIndex < 0)
            {
                ClearGroups();
                return;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.CommandText = @"
              SELECT g.iid, g.name, g.oldYear, g.youngYear, g.genderFemale,
                     ISNULL(q.qf,'') qf
                FROM Groups g(NOLOCK)
           LEFT JOIN qfList q(NOLOCK) ON q.iid = g.minQf
               WHERE g.name = @name";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
            cmd.Parameters[0].Value = cbGroupsSelect.SelectedItem.ToString();
            SqlDataReader rd = cmd.ExecuteReader();
            try
            {
                if (rd.Read())
                {
                    tbGroupsIID.Text = rd["iid"].ToString();
                    tbGroupsName.Text = rd["name"].ToString();
                    tbGroupsOld.Text = rd["oldYear"].ToString();
                    tbGroupsYoung.Text = rd["youngYear"].ToString();
                    cbGroupsSex.SelectedIndex =
                        (Convert.ToBoolean(rd["genderFemale"]) ? 1 : 0);
                    /*cbGroupsSex.SelectedIndex = cbGroupsSex.Items.IndexOf("Ж");
                else
                    cbGroupsSex.SelectedIndex = cbGroupsSex.Items.IndexOf("М");*/
                    cbGroupsQf.SelectedIndex = cbGroupsQf.Items.IndexOf(rd["qf"].ToString());
                }
            }
            finally { rd.Close(); }
        }

        private void btnGroupsAdd_Click(object sender, EventArgs e)
        {
            SetEditModeGroups(true);
            ClearGroups();
            addGroups = true;
        }

        private void btnGroupsEdit_Click(object sender, EventArgs e)
        {
            if (btnGroupsEdit.Text == EDIT)
            {
                SetEditModeGroups(true);
                addGroups = false;
            }
            else
            {
                int iid;
                if (addGroups)
                {
                    SqlCommand cmdW = new SqlCommand("SELECT (MAX(iid)+1) FROM Groups", cn);
                    try { iid = Convert.ToInt32(cmdW.ExecuteScalar()); }
                    catch { iid = 1; }
                }
                else
                {
                    try
                    {
                        iid = Convert.ToInt32(tbGroupsIID.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Группа не выбрана");
                        return;
                    }
                }
                if (tbGroupsName.Text == "")
                {
                    MessageBox.Show("Введите имя группы");
                    return;
                }
                int yearYoung, yearOld;

                if (tbGroupsOld.Text == "")
                    yearOld = 0;
                else
                {
                    try
                    {
                        yearOld = Convert.ToInt32(tbGroupsOld.Text);
                        if ((yearOld > 10) && (yearOld < 100))
                            yearOld += 1900;
                        if ((yearOld >= 0) && (yearOld <= 10))
                            yearOld += 2000;
                        if (yearOld < 0)
                            throw new Exception();
                    }
                    catch
                    {
                        MessageBox.Show("Старший год введён неверно");
                        return;
                    }
                }
                if (tbGroupsYoung.Text == "")
                    yearYoung = DateTime.Now.Year;
                else
                {
                    try
                    {
                        yearYoung = Convert.ToInt32(tbGroupsYoung.Text);
                        if ((yearYoung > 10) && (yearYoung < 100))
                            yearYoung += 1900;
                        if ((yearYoung >= 0) && (yearYoung <= 10))
                            yearYoung += 2000;
                        if (yearYoung < 0)
                            throw new Exception();
                    }
                    catch
                    {
                        MessageBox.Show("Младший год введён неверно");
                        SetEditModeGroups(false);
                        return;
                    }
                }
                bool gFemale;
                if (cbGroupsSex.SelectedIndex < 0)
                {
                    MessageBox.Show("Пол не выбран");
                    return;
                }
                gFemale = (cbGroupsSex.Text.Equals("ж", StringComparison.InvariantCultureIgnoreCase) ||
                    cbGroupsSex.Text.Equals("f", StringComparison.InvariantCultureIgnoreCase));

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;

                cmd.CommandText = "SELECT iid FROM qfList(NOLOCK) WHERE qf = @q";
                cmd.Parameters.Add("@q", SqlDbType.VarChar, 4);
                cmd.Parameters[0].Value = cbGroupsQf.Text;
                object oTmp = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                int iQ;
                if (oTmp == null || oTmp == DBNull.Value)
                {
                    cmd.CommandText = "SELECT ISNULL(MAX(iid),0) FROM qfList(NOLOCK)";
                    oTmp = cmd.ExecuteScalar();
                    if (oTmp == null || oTmp == DBNull.Value)
                        iQ = 0;
                    else
                        iQ = Convert.ToInt32(oTmp);
                }
                else
                    iQ = Convert.ToInt32(oTmp);

                if (addGroups)
                {
                    cmd.CommandText = "INSERT INTO Groups (name, oldYear, youngYear, genderFemale, minQf, iid) " +
                        "VALUES (@name, @yearOld, @yearYoung, @genderFemale, @minQf, @iid)";
                }
                else
                {
                    cmd.CommandText = "UPDATE Groups SET name=@name, oldYear=@yearOld, " +
                        "youngYear=@yearYoung, genderFemale=@genderFemale, minQf=@minQf WHERE iid=@iid";
                }

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                cmd.Parameters[0].Value = tbGroupsName.Text;

                cmd.Parameters.Add("@yearOld", SqlDbType.Int);
                cmd.Parameters[1].Value = yearOld;

                cmd.Parameters.Add("@yearYoung", SqlDbType.Int);
                cmd.Parameters[2].Value = yearYoung;

                cmd.Parameters.Add("@genderFemale", SqlDbType.Bit);
                cmd.Parameters[3].Value = gFemale;

                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[4].Value = iid;

                cmd.Parameters.Add("@minQf", SqlDbType.Int);
                cmd.Parameters[5].Value = iQ;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                SetEditModeGroups(false);
                RefreshCbGroups();
            }
        }

        private void btnGroupsDel_Click(object sender, EventArgs e)
        {
            if (btnGroupsDel.Text == CANCEL)
            {
                SetEditModeGroups(false);
                return;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            if (cbGroupsSelect.SelectedIndex < 0)
            {
                MessageBox.Show("Группа не выбрана");
                return;
            }
            DialogResult dg = MessageBox.Show("Вы уверены что хотите удалить группу '" +
                cbGroupsSelect.SelectedItem.ToString() + "'?",
                "Удаление группы", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dg == DialogResult.No)
                return;
            cmd.CommandText = "DELETE FROM Groups WHERE iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            try
            {
                cmd.Parameters[0].Value = Convert.ToInt32(tbGroupsIID.Text);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            SetEditModeGroups(false);
            RefreshCbGroups();
            ClearGroups();
        }
        #endregion

        #region Teams
        bool addTeams = false;

        private void SetEditModeTeams(bool edit)
        {
            if (allowGuest)
                cbGuest.Enabled = edit;
            btnTeamsPrintTB.Enabled = btnDelEmptyTeams.Enabled = btnTeamsInet.Enabled = btnTeamPrintOrders.Enabled =
                tbTeamsChiefOrd.ReadOnly = tbTeamsChief.ReadOnly = tbTeamPos.ReadOnly = tbTeamsName.ReadOnly = btnTeamsAdd.Enabled = cbTeamsSelect.Enabled = !edit;
            if (edit)
            {
                btnTeamsDel.Text = CANCEL;
                btnTeamsEdit.Text = SUBMIT;
            }
            else
            {
                btnTeamsDel.Text = DELETE;
                btnTeamsEdit.Text = EDIT;
            }
        }

        private void RefreshCbTeams()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT name FROM Teams ORDER BY name";
            SqlDataReader rdr = cmd.ExecuteReader();
            cbTeamsSelect.Items.Clear();
            cbPartTeams.Items.Clear();
            cbPartTeams.Items.Add(SCR_ALL_TEAMS);
            while (rdr.Read())
            {
                cbTeamsSelect.Items.Add(rdr[0].ToString());
                cbPartTeams.Items.Add(rdr[0].ToString());
            }
            rdr.Close();
            cbTeamsSelect.SelectedIndex = -1;
        }

        private void ClearTeams()
        {
            cbTeamsSelect.SelectedIndex = -1;
            tbTeamsName.Text = "";
            cbTeamsSelect.Text = "Выберите команду";
            tbTeamsIID.Text = "";
            tbTeamPos.Text = "";
            tbTeamsChief.Text = tbTeamsChiefOrd.Text = String.Empty;
            cbGuest.Checked = false;
        }

        private void cbTeamsSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTeamsSelect.SelectedIndex < 0)
            {
                ClearTeams();
                return;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (allowGuest)
                cmd.CommandText = "SELECT iid, name, Guest, pos, chief, chief_ord FROM Teams(NOLOCK) WHERE name = @name";
            else
                cmd.CommandText = "SELECT iid, name, pos, chief, chief_ord FROM Teams(NOLOCK) WHERE name = @name";
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
            cmd.Parameters[0].Value = cbTeamsSelect.SelectedItem.ToString();
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                if (rdr.Read())
                {
                    tbTeamsIID.Text = rdr["iid"].ToString();
                    tbTeamsName.Text = rdr["name"].ToString();
                    if (allowGuest)
                    {
                        tbTeamPos.Text = rdr["pos"].ToString();
                        cbGuest.Checked = Convert.ToBoolean(rdr["Guest"]);
                    }
                    else
                        tbTeamPos.Text = rdr["pos"].ToString();
                    tbTeamsChief.Text = rdr["chief"].ToString();
                    tbTeamsChiefOrd.Text = rdr["chief_ord"].ToString();
                }
            }
            finally
            {
                rdr.Close();
            }
        }

        private void btnTeamsAdd_Click(object sender, EventArgs e)
        {
            SetEditModeTeams(true);
            ClearTeams();
            addTeams = true;
        }

        private void btnTeamsEdit_Click(object sender, EventArgs e)
        {
            if (btnTeamsEdit.Text == EDIT)
            {
                SetEditModeTeams(true);
                addTeams = false;
            }
            else
            {
                int iid;
                if (addTeams)
                {
                    SqlCommand cmdW = new SqlCommand("SELECT (MAX (iid)+1) FROM Teams", cn);
                    try { iid = Convert.ToInt32(cmdW.ExecuteScalar()); }
                    catch { iid = 1; }
                }
                else
                {
                    try
                    {
                        iid = Convert.ToInt32(tbTeamsIID.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Команда не выбрана");
                        SetEditModeTeams(false);
                        return;
                    }
                }
                if (tbTeamsName.Text == "")
                {
                    MessageBox.Show("Введите команду");
                    return;
                }
                int? pos;
                int nTmp;
                if (String.IsNullOrEmpty(tbTeamPos.Text))
                    pos = null;
                else
                    if (int.TryParse(tbTeamPos.Text, out nTmp))
                        pos = nTmp;
                    else
                    {
                        MessageBox.Show("Рейтинг введён неправильно");
                        return;
                    }

                SqlCommand cmd = new SqlCommand();
                if (allowGuest)
                {
                    if (addTeams)
                        cmd.CommandText = "INSERT INTO Teams (name, Guest, iid, pos, chief, chief_ord) VALUES (@name, @guest, @iid, @pos, @chief, @co)";
                    else
                        cmd.CommandText = "UPDATE Teams SET name=@name, Guest = @guest, pos=@pos, chief = @chief,chief_ord = @co WHERE iid=@iid";
                }
                else
                {
                    if (addTeams)
                        cmd.CommandText = "INSERT INTO Teams (name, iid, pos, chief,chief_ord) VALUES (@name, @iid, @pos, @chief, @co)";
                    else
                        cmd.CommandText = "UPDATE Teams SET name=@name, pos=@pos, chief = @chief,chief_ord = @co WHERE iid=@iid";
                }
                cmd.Connection = cn;

                cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                cmd.Parameters[0].Value = tbTeamsName.Text;

                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[1].Value = iid;

                if (allowGuest)
                {
                    cmd.Parameters.Add("@guest", SqlDbType.Bit);
                    cmd.Parameters[2].Value = cbGuest.Checked;
                }
                cmd.Parameters.Add("@pos", SqlDbType.Int);
                if (pos == null)
                    cmd.Parameters[cmd.Parameters.Count - 1].Value = DBNull.Value;
                else
                    cmd.Parameters[cmd.Parameters.Count - 1].Value = pos.Value;

                cmd.Parameters.Add("@chief", SqlDbType.VarChar, 255);
                cmd.Parameters[cmd.Parameters.Count - 1].Value = tbTeamsChief.Text;

                cmd.Parameters.Add("@co", SqlDbType.VarChar, 255);
                cmd.Parameters[cmd.Parameters.Count - 1].Value = tbTeamsChiefOrd.Text;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                SetEditModeTeams(false);
                RefreshCbTeams();
            }
        }

        private void btnTeamsDel_Click(object sender, EventArgs e)
        {
            if (btnTeamsDel.Text == CANCEL)
            {
                SetEditModeTeams(false);
                return;
            }
            int iid;
            try
            {
                if (cbTeamsSelect.SelectedIndex < 0)
                    throw new Exception("Команда не выбрана");
                iid = Convert.ToInt32(tbTeamsIID.Text);
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
                return;
            }
            DialogResult dg = MessageBox.Show("Вы уверены что хотите удалить команду '" +
                cbTeamsSelect.SelectedItem.ToString() + "'?",
                "Удаление команды", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dg == DialogResult.No)
                return;
            SqlCommand cmd = new SqlCommand("DELETE FROM Teams WHERE iid = @iid", cn);
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters[0].Value = iid;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            SetEditModeTeams(false);
            RefreshCbTeams();
            ClearTeams();
        }
        #endregion

        #region Participants
        bool cancelEvent = false;
        bool addParts = false;

        private void SetEditModeParts(bool edit)
        {
            cancelEvent = cbPartQf.Enabled = cbPartSex.Enabled = bPartLateAppl.Enabled =
                bPartNoPoints.Enabled = bPartVk.Enabled = 
                button1.Enabled = btnDelPhoto.Enabled = btnGetPhotoFromLocal.Enabled = edit;
            tbPartAge.ReadOnly = tbPartBoulder.ReadOnly = tbPartLead.ReadOnly = tbPartName.ReadOnly =
                tbPartSpeed.ReadOnly = tbPartSurname.ReadOnly = /*cbPartGroups.Enabled =*/
                btnPartAdd.Enabled = xlExport.Enabled = xlExport.Enabled = btnInetImport.Enabled = btnAddTeams.Enabled =
                btnSaveAllPhotos.Enabled = btnLoadFromFolder.Enabled = btnOrderNum.Enabled = !edit;

            btnGetFromInet.Enabled = gbInetPhoto.Visible && edit;

            if (edit)
            {
                btnPartEdit.Text = SUBMIT;
                btnPartDel.Text = CANCEL;
                cbPartGroups.Items.RemoveAt(0);
                cbPartTeams.Items.RemoveAt(0);
                if (cbPartGroups.Items.IndexOf(SCR_DEFAULT_GROUP) < 0)
                    cbPartGroups.Items.Insert(0, SCR_DEFAULT_GROUP);
                cbPartGroups.SelectedIndex = 0;
            }
            else
            {
                btnPartDel.Text = DELETE;
                btnPartEdit.Text = EDIT;
                try { cbPartGroups.Items.Remove(SCR_DEFAULT_GROUP); }
                catch { }
                try
                {
                    if (cbPartTeams.Items.Count < 1 || cbPartTeams.Items[0].ToString() != SCR_ALL_TEAMS)
                        cbPartTeams.Items.Insert(0, SCR_ALL_TEAMS);
                    if (cbPartGroups.Items.Count < 1 || cbPartGroups.Items[0].ToString() != SCR_ALL_GROUPS)
                        cbPartGroups.Items.Insert(0, SCR_ALL_GROUPS);
                }
                catch { }
            }
        }

        private sealed class LoadDataCn
        {
            public XmlClient Client { get; private set; }
            public bool LoadPhoto { get; private set; }
            public long? compID { get; private set; }
            public SqlConnection local { get; private set; }
            public SqlConnection remote { get; private set; }
            public Secretary form { get; private set; }
            public MainForm.AfetrStopCallBack Callback { get; private set; }
            public LoadDataCn(SqlConnection local, SqlConnection remote, long? compID, Secretary form, MainForm.AfetrStopCallBack callback, XmlClient client, bool loadPhoto = true)
            {
                this.local = local;
                this.remote = remote;
                this.form = form;
                this.Callback = callback;
                this.compID = compID;
                this.LoadPhoto = loadPhoto;
                this.Client = client;
            }
        }

        private void LoadDataFromSite2()
        {
            var serviceClient = ServiceClient.GetInstance(this.cn);
            if(serviceClient.Competition == null)
            {
                MessageBox.Show("Соединение с веб-службой не настроено.");
                return;
            }

            if (MessageBox.Show("Все данные будут удалены. Продолжить?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.No)
                return;

            var tran = cn.BeginTransaction();
            try
            {
                int elementCount;
                using(var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.Transaction = tran;
                    
                    cmd.CommandText = "DELETE FROM Participants";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM Groups";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM Teams";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO Groups (iid, name, oldYear, youngYear, genderFemale, " + ServiceHelper.REMOTE_ID_COLUMN + ")" +
                                      "VALUES(@iid, @name, @oldYear, @youngYear, @genderFemale, @remoteIid)";
                    cmd.Parameters.Add("@iid", SqlDbType.Int);
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@oldYear", SqlDbType.Int);
                    cmd.Parameters.Add("@youngYear", SqlDbType.Int);
                    cmd.Parameters.Add("@genderFemale", SqlDbType.Bit);
                    cmd.Parameters.Add("@remoteIid", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE);

                    ServiceHelper.CheckRemoteIdColumn("Groups", cmd.Connection, cmd.Transaction);

                    elementCount = 0;
                    var groups = new Dictionary<string, int>();
                    foreach (var g in serviceClient.GetAgeGroups()
                                                   .OrderBy(g => g.YearOld * 10 + g.Gender == Common.Gender.Female ? 1 : 0))
                    {
                        cmd.Parameters["@iid"].Value = ++elementCount;
                        cmd.Parameters["@name"].Value = g.Name;
                        cmd.Parameters["@oldYear"].Value = g.YearOld;
                        cmd.Parameters["@youngYear"].Value = g.YearYoung;
                        cmd.Parameters["@genderFemale"].Value = g.Gender == Common.Gender.Female;
                        cmd.Parameters["@remoteIid"].Value = g.AgeGroupInCompId;
                        cmd.ExecuteNonQuery();

                        groups.Add(g.AgeGroupInCompId, elementCount);
                    }
                    MessageBox.Show(string.Format("Загружено групп: {0}", elementCount));


                    elementCount = 0;
                    ServiceHelper.CheckRemoteIdColumn("Teams", cmd.Connection, cmd.Transaction);
                    cmd.CommandText = "INSERT INTO Teams(iid, name, " + ServiceHelper.REMOTE_ID_COLUMN + ") " +
                                      "VALUES (@iid, @name, @remoteIid)";
                    var teams = new Dictionary<string, int>();
                    foreach(var t in serviceClient.GetTeams().OrderBy(t => t.Name))
                    {
                        cmd.Parameters["@iid"].Value = ++elementCount;
                        cmd.Parameters["@name"].Value = t.Name;
                        cmd.Parameters["@remoteIid"].Value = t.Iid;
                        cmd.ExecuteNonQuery();

                        teams.Add(t.Iid, elementCount);
                    }
                    MessageBox.Show(string.Format("Загружено команд: {0}", elementCount));
                    
                    cmd.Parameters.Clear();
                    elementCount = 0;
                    ServiceHelper.CheckRemoteIdColumn("Participants", cmd.Connection, cmd.Transaction);
                    cmd.CommandText = "INSERT INTO Participants(iid, " + ServiceHelper.REMOTE_ID_COLUMN + ", surname, name, age, genderFemale, qf, team_id, group_id," +
                                      "lead, speed, boulder, changed)" +
                                      "VALUES(@iid, @remoteIid, @surname, @name, @age, @genderFemale, @qf, @team, @group," +
                                      "@l, @s, @b, 0)";
                    cmd.Parameters.Add("@iid", SqlDbType.Int);
                    cmd.Parameters.Add("@remoteIid", SqlDbType.VarChar, ServiceHelper.REMOTE_ID_COL_SIZE);
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                    cmd.Parameters.Add("@age", SqlDbType.Int);
                    cmd.Parameters.Add("@genderFemale", SqlDbType.Bit);
                    cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50);
                    cmd.Parameters.Add("@team", SqlDbType.Int);
                    cmd.Parameters.Add("@group", SqlDbType.Int);
                    cmd.Parameters.Add("@l", SqlDbType.Bit);
                    cmd.Parameters.Add("@s", SqlDbType.Bit);
                    cmd.Parameters.Add("@b", SqlDbType.Bit);

                    foreach (var p in serviceClient.GetParticipants())
                    {
                        cmd.Parameters["@iid"].Value = ++elementCount;
                        cmd.Parameters["@remoteIid"].Value = p.RecordUniqueId;
                        cmd.Parameters["@surname"].Value = p.Surname;
                        cmd.Parameters["@name"].Value = p.Name;
                        cmd.Parameters["@age"].Value = p.YearOfBirth;
                        cmd.Parameters["@genderFemale"].Value = p.Gender == Common.Gender.Female;
                        cmd.Parameters["@qf"].Value = p.Qf.EnumFriendlyValue();
                        cmd.Parameters["@team"].Value = teams[p.TeamsIDs.First()];
                        cmd.Parameters["@group"].Value = groups[p.AgeGroupInCompId];
                        cmd.Parameters["@l"].Value = (p.Styles & Common.ClimbingStyles.Lead) == Common.ClimbingStyles.Lead;
                        cmd.Parameters["@s"].Value = (p.Styles & Common.ClimbingStyles.Speed) == Common.ClimbingStyles.Speed;
                        cmd.Parameters["@b"].Value = (p.Styles & Common.ClimbingStyles.Bouldering) == Common.ClimbingStyles.Bouldering;

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(string.Format("Загружено участников: {0}", elementCount));
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private static void CopyDataFromWebService(XmlClient client, long compId, SqlConnection cn)
        {
            var dgRes = MessageBox.Show("Все данные будут удалены. Продолжить?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
            if (dgRes == DialogResult.No)
                return;
            var groupList = client.AgeGroups;
            var teamList = client.Regions;
            var cL = client.Climbers;
            int teams = 0, groups = 0, climbers = 0;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction tran = cn.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;
                cmd.CommandText = "DELETE FROM Participants";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM Groups";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM Teams";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO Groups (iid, name, oldYear, youngYear, genderFemale)" +
                                  "VALUES(@iid, @name, @oldYear, @youngYear, @genderFemale)";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@oldYear", SqlDbType.Int);
                cmd.Parameters.Add("@youngYear", SqlDbType.Int);
                cmd.Parameters.Add("@genderFemale", SqlDbType.Bit);
                foreach (var g in groupList.Data)
                {
                    cmd.Parameters["@iid"].Value = g.Iid;
                    cmd.Parameters["@name"].Value = g.Name;
                    cmd.Parameters["@oldYear"].Value = g.YearOld;
                    cmd.Parameters["@youngYear"].Value = g.YearYoung;
                    cmd.Parameters["@genderFemale"].Value = g.Female;
                    cmd.ExecuteNonQuery();
                    groups++;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO Teams(iid, name) VALUES (@iid, @name)";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                foreach (var t in teamList.Data)
                {
                    cmd.Parameters["@iid"].Value = t.Iid;
                    cmd.Parameters["@name"].Value = t.Name;
                    cmd.ExecuteNonQuery();
                    teams++;
                }

                SqlCommand cmdBibCheck = new SqlCommand()
                {
                    Connection = cn,
                    Transaction = tran,
                    CommandText = "SELECT COUNT(*) cnt FROM Participants P(nolock) WHERE P.iid = @bib"
                };
                cmdBibCheck.Parameters.Add("@bib", SqlDbType.Int);

                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO Participants(iid, license, surname, name, age, genderFemale, qf, team_id, group_id," +
                                  "rankingLead, rankingSpeed, rankingBoulder,lead, speed, boulder, changed)" +
                                  "VALUES(@iid, @license, @surname, @name, @age, @genderFemale, @qf, @team, @group," +
                                  "@rL, @rS, @rB, @l, @s, @b, 0)";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@license", SqlDbType.BigInt);
                cmd.Parameters.Add("@surname", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@age", SqlDbType.Int);
                cmd.Parameters.Add("@genderFemale", SqlDbType.Bit);
                cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@team", SqlDbType.Int);
                cmd.Parameters.Add("@group", SqlDbType.Int);
                cmd.Parameters.Add("@rL", SqlDbType.Int);
                cmd.Parameters.Add("@rS", SqlDbType.Int);
                cmd.Parameters.Add("@rB", SqlDbType.Int);
                cmd.Parameters.Add("@l", SqlDbType.Bit);
                cmd.Parameters.Add("@s", SqlDbType.Bit);
                cmd.Parameters.Add("@b", SqlDbType.Bit);
                SqlCommand cmdTeams = new SqlCommand{Connection=cn, Transaction=tran};
                cmdTeams.CommandText = "INSERT INTO teamsLink (team_id, climber_id) VALUES (@tm, @clm)";
                cmdTeams.Parameters.Add("@tm", SqlDbType.Int);
                cmdTeams.Parameters.Add("@clm", SqlDbType.Int);
                foreach (var clm in cL.Data)
                {
                    int newBib;
                    if (clm.Bib != null)
                    {
                        cmdBibCheck.Parameters[0].Value = clm.Bib;
                        object oRes = cmdBibCheck.ExecuteScalar();
                        if (oRes != null && Convert.ToInt32(oRes) > 0)
                            newBib = GetNextNumber(clm.GroupID, tran, cn);
                        else
                            newBib = clm.Bib.Value;
                    }
                    else
                        newBib = GetNextNumber(clm.GroupID, tran, cn);
                    cmd.Parameters["@iid"].Value = newBib;
                    cmd.Parameters["@license"].Value = clm.License;
                    cmd.Parameters["@surname"].Value = clm.Surname;
                    cmd.Parameters["@name"].Value = clm.Name;
                    cmd.Parameters["@age"].Value = clm.YearOfBirth;
                    cmd.Parameters["@genderFemale"].Value = clm.Female;
                    cmd.Parameters["@qf"].Value = clm.Razr;
                    cmd.Parameters["@team"].Value = clm.TeamID;
                    cmd.Parameters["@group"].Value = clm.GroupID;
                    cmd.Parameters["@rL"].Value = clm.RankingLead == null ? DBNull.Value:(object)clm.RankingLead.Value;
                    cmd.Parameters["@rS"].Value = clm.RankingSpeed == null ? DBNull.Value : (object)clm.RankingSpeed.Value;
                    cmd.Parameters["@rB"].Value = clm.RankingBoulder == null ? DBNull.Value : (object)clm.RankingBoulder.Value;
                    cmd.Parameters["@l"].Value = clm.Lead;
                    cmd.Parameters["@s"].Value = clm.Speed;
                    cmd.Parameters["@b"].Value = clm.Boulder;
                    cmd.ExecuteNonQuery();
                    var mt = clm as Comp_MultipleTeamsClimber;
                    if (mt != null)
                    {
                        cmdTeams.Parameters["@clm"].Value = newBib;
                        foreach (var t in mt.Teams)
                        {
                            if (clm.TeamID == (int)t)
                                continue;
                            cmdTeams.Parameters["@tm"].Value = (int)t;
                            cmdTeams.ExecuteNonQuery();
                        }
                    }
                    climbers++;
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            MessageBox.Show(
                String.Format("Данные получены.{0}Импортировано Групп: {1}{0}             Команд: {2}{0}         Участников: {3}",
                Environment.NewLine, groups, teams, climbers));
        }

        private static void CopyDataFromOnline(object objP)
        {
            LoadDataCn lcn = objP as LoadDataCn;
            if (lcn == null)
                return;
            SqlConnection cn = null, remote = null;
            bool threadAbort = false;
            try
            {
                if ((lcn.remote == null && lcn.compID == null) || lcn.local == null || lcn.form == null)
                    return;
                cn = new SqlConnection(lcn.local.ConnectionString);
                

                List<int> insP = new List<int>();
                dsClimbing ds = new dsClimbing();
                ClimbingService service = null;
                if (lcn.compID == null || !lcn.compID.HasValue)
                {
                    remote = new SqlConnection(lcn.remote.ConnectionString);
                    if (remote.State != ConnectionState.Open)
                        remote.Open();
                    service = null;
                }
                else if (lcn.Client != null)
                {
                    try
                    {
                        CopyDataFromWebService(lcn.Client, lcn.compID.Value, cn);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(String.Format("Ошибка сохранения данных:{0}{1}", Environment.NewLine, ex.Message));
                    }
                    catch (WebException ex)
                    {
                        MessageBox.Show(String.Format("Ошибка загрузки данных:{0}{1}", Environment.NewLine, ex.Message));
                    }
                    return;
                }
                else
                {
                    service = new ClimbingService();
                    remote = null;
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                ONLGroupsTableAdapter gta = new ONLGroupsTableAdapter();
                if (remote != null)
                {
                    gta.Connection = remote;
                    gta.Fill(ds.ONLGroups);
                }
                ONLteamsTableAdapter tta = new ONLteamsTableAdapter();
                if (remote != null)
                {
                    tta.Connection = remote;
                    tta.Fill(ds.ONLteams);
                }
                ONLclimbersTableAdapter cta = new ONLclimbersTableAdapter();
                if (remote != null)
                {
                    cta.Connection = remote;
                    cta.FillByIsDel(ds.ONLclimbers, false);
                }
                SqlCommand cmdCheck = new SqlCommand();
                cmdCheck.Connection = cn;
                cmdCheck.CommandText = "SELECT COUNT(*) cnt FROM Teams(NOLOCK) WHERE iid=@iid";
                cmdCheck.Parameters.Add("@iid", SqlDbType.Int);
                SqlCommand cmdAdd = new SqlCommand();
                string sInsert = "INSERT INTO Teams(iid,name,pos, chief, chief_ord) VALUES(@iid, @name,@pos,'','')";
                string sUpdate = "UPDATE Teams SET name=@name, pos=@pos WHERE iid= @iid";
                cmdAdd.Connection = cn;
                cmdAdd.Parameters.Add("@iid", SqlDbType.Int);
                cmdAdd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmdAdd.Parameters.Add("@pos", SqlDbType.Int);
                cmdAdd.Parameters[2].Value = DBNull.Value;

                List<int> teams = new List<int>();
                if(service == null)
                foreach (dsClimbing.ONLteamsRow row in ds.ONLteams)
                {
                    cmdCheck.Parameters[0].Value = row.iid;
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        cmdAdd.CommandText = sUpdate;
                    else
                        cmdAdd.CommandText = sInsert;
                    cmdAdd.Parameters[0].Value = row.iid;
                    cmdAdd.Parameters[1].Value = row.name;
                    cmdAdd.ExecuteNonQuery();
                    teams.Add(row.iid);
                }
                else
                    foreach (var t in service.GetTeamsForCompetition(lcn.compID.Value))
                    {
                        cmdCheck.Parameters[0].Value = t.SecretaryId;
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                            cmdAdd.CommandText = sUpdate;
                        else
                            cmdAdd.CommandText = sInsert;
                        cmdAdd.Parameters[0].Value = t.SecretaryId;
                        cmdAdd.Parameters[1].Value = t.Name;
                        cmdAdd.Parameters[2].Value = (t.RankingPos < int.MaxValue) ? (object)t.RankingPos : DBNull.Value;
                        cmdAdd.ExecuteNonQuery();
                        teams.Add(t.SecretaryId);
                    }

                cmdCheck.CommandText = "SELECT COUNT(*) cnt FROM Groups(NOLOCK) WHERE iid=@iid";

                sInsert = "INSERT INTO Groups(iid,name, oldYear, youngYear, genderFemale) VALUES(@iid, @name, @yO, @yY, @gF)";
                sUpdate = "UPDATE Groups SET name=@name, oldYear=@yO, youngYear=@yY, genderFemale = @gF WHERE iid=@iid";
                cmdAdd.Parameters.Clear();

                cmdAdd.Parameters.Add("@iid", SqlDbType.Int);
                cmdAdd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmdAdd.Parameters.Add("@yO", SqlDbType.Int);
                cmdAdd.Parameters.Add("@yY", SqlDbType.Int);
                cmdAdd.Parameters.Add("@gF", SqlDbType.Bit);
                cmdAdd.Parameters.Add("@minQF", SqlDbType.Int);
                cmdAdd.Parameters[5].Value = 12;

                List<int> groups = new List<int>();
                if (service == null)
                    foreach (dsClimbing.ONLGroupsRow row in ds.ONLGroups.Rows)
                    {
                        cmdCheck.Parameters[0].Value = row.iid;
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                            cmdAdd.CommandText = sUpdate;
                        else
                            cmdAdd.CommandText = sInsert;
                        cmdAdd.Parameters[0].Value = row.iid;
                        cmdAdd.Parameters[1].Value = row.name;
                        cmdAdd.Parameters[2].Value = row.oldYear;
                        cmdAdd.Parameters[3].Value = row.youngYear;
                        cmdAdd.Parameters[4].Value = row.genderFemale;
                        cmdAdd.ExecuteNonQuery();
                        groups.Add(row.iid);
                    }
                else
                {
                    SqlCommand cmc2 = new SqlCommand(), cm3 = new SqlCommand();
                    cmc2.Connection = cm3.Connection=cmdCheck.Connection;
                    cmc2.Transaction = cm3.Transaction = cmdCheck.Transaction;
                    cmc2.CommandText = "SELECT iid FROM Groups(NOLOCK) WHERE oldYear = @old AND youngYear=@young AND genderFemale=@gender";
                    cmc2.Parameters.Add("@old", SqlDbType.Int);
                    cmc2.Parameters.Add("@young", SqlDbType.Int);
                    cmc2.Parameters.Add("@gender", SqlDbType.Bit);
                    sUpdate = "UPDATE Groups SET iid=@iidNew, name=@name WHERE iid=@iid";
                    cm3.CommandText = "UPDATE Groups SET iid=@iidNew WHERE iid=@iid";
                    cm3.Parameters.Add("@iidNew", SqlDbType.Int);
                    cm3.Parameters.Add("@iid", SqlDbType.Int);
                    cmdAdd.Parameters.Add("@iidNew", SqlDbType.Int);
                    foreach (var g in service.GetGroupsForCompetition(lcn.compID.Value))
                    {
                        cmc2.Parameters[0].Value = g.yearOld;
                        cmc2.Parameters[1].Value = g.yearYoung;
                        cmc2.Parameters[2].Value = g.GenderFemale;
                        object obj = cmc2.ExecuteScalar();
                        int toUpd = -1;
                        try
                        {
                            if (obj != null && obj != DBNull.Value)
                                toUpd = Convert.ToInt32(obj);
                        }
                        catch { }

                        if (toUpd > 0)
                            cmdAdd.CommandText = sUpdate;
                        else
                            cmdAdd.CommandText = sInsert;
                        int iOld = toUpd < 1 ? g.Iid : toUpd;
                        cmdAdd.Parameters[0].Value = iOld;
                        cmdAdd.Parameters[1].Value = g.Name;
                        cmdAdd.Parameters[2].Value = g.yearOld;
                        cmdAdd.Parameters[3].Value = g.yearYoung;
                        cmdAdd.Parameters[4].Value = g.GenderFemale;
                        cmdAdd.Parameters[5].Value = g.minQf;
                        cmdAdd.Parameters[6].Value = g.Iid;

                        cm3.Parameters[0].Value = int.MaxValue;
                        cm3.Parameters[1].Value = g.Iid;
                        bool toUpdBack = (iOld != g.Iid) ? (cm3.ExecuteNonQuery() > 0) : false;


                        cmdAdd.ExecuteNonQuery();
                        if (toUpdBack)
                        {
                            cm3.Parameters[0].Value = iOld;
                            cm3.Parameters[1].Value = int.MaxValue;
                            cm3.ExecuteNonQuery();
                        }
                        groups.Add(g.Iid);
                    }
                }

                
                cmdCheck.CommandText = "SELECT COUNT(*) cnt FROM Participants(NOLOCK) WHERE iid=@iid";

                sInsert = "INSERT INTO Participants(iid,name,surname,age,genderFemale, qf,team_id,group_id," +
                    "rankingLead, rankingSpeed, rankingBoulder, vk, noPoints, photo, lead, speed, boulder, name_ord)" +
                    "VALUES(@iid, @name, @surname, @age, @gF, @qf, @t,@g, @rL, @rS, @rB, @vk, @nP, @photo, @lead, @speed, @boulder,'')";
                sUpdate = @"UPDATE Participants SET name=@name, surname=@surname, age=@age, genderFemale=@gF,
                                 qf=@qf, team_id=@t, group_id=@g,rankingLead=@rL,rankingSpeed=@rS,
                                 rankingBoulder=@rB,vk=@vk,noPoints=@nP, photo=@photo, lead=@lead, speed=@speed, boulder=@boulder WHERE iid=@iid";

                cmdAdd.Parameters.Clear();
                cmdAdd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmdAdd.Parameters.Add("@surname", SqlDbType.VarChar, 255);
                cmdAdd.Parameters.Add("@age", SqlDbType.Int);
                cmdAdd.Parameters.Add("@gF", SqlDbType.Bit);
                cmdAdd.Parameters.Add("@t", SqlDbType.Int);
                cmdAdd.Parameters.Add("@g", SqlDbType.Int);
                cmdAdd.Parameters.Add("@qf", SqlDbType.VarChar, 50);
                cmdAdd.Parameters.Add("@rL", SqlDbType.Int);
                cmdAdd.Parameters.Add("@rS", SqlDbType.Int);
                cmdAdd.Parameters.Add("@rB", SqlDbType.Int);
                cmdAdd.Parameters.Add("@vk", SqlDbType.Bit);
                cmdAdd.Parameters.Add("@nP", SqlDbType.Bit);
                cmdAdd.Parameters.Add("@photo", SqlDbType.Image);
                cmdAdd.Parameters.Add("@iid", SqlDbType.Int);
                cmdAdd.Parameters.Add("@lead", SqlDbType.SmallInt);
                cmdAdd.Parameters.Add("@speed", SqlDbType.SmallInt);
                cmdAdd.Parameters.Add("@boulder", SqlDbType.SmallInt);
                if(service == null)
                    foreach (dsClimbing.ONLclimbersRow row in ds.ONLclimbers)
                    {
                        cmdCheck.Parameters[0].Value = row.iid;
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                            cmdAdd.CommandText = sUpdate;
                        else
                            cmdAdd.CommandText = sInsert;
                        cmdAdd.Parameters[0].Value = row.name;
                        cmdAdd.Parameters[1].Value = row.surname;
                        cmdAdd.Parameters[2].Value = row.age;
                        cmdAdd.Parameters[3].Value = row.genderFemale;
                        cmdAdd.Parameters[4].Value = row.team_id;
                        cmdAdd.Parameters[5].Value = row.group_id;
                        if (row.IsqfNull())
                            cmdAdd.Parameters[6].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[6].Value = row.qf;
                        if (row.IsrankingLeadNull())
                            cmdAdd.Parameters[7].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[7].Value = row.rankingLead;
                        if (row.IsrankingSpeedNull())
                            cmdAdd.Parameters[8].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[8].Value = row.rankingSpeed;
                        if (row.IsrankingBoulderNull())
                            cmdAdd.Parameters[9].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[9].Value = row.rankingBoulder;
                        cmdAdd.Parameters[10].Value = row.vk;
                        cmdAdd.Parameters[11].Value = row.nopoints;
                        if (row.IsphotoNull())
                            cmdAdd.Parameters[12].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[12].Value = row.photo;
                        cmdAdd.Parameters[13].Value = row.iid;
                        cmdAdd.Parameters[14].Value = row.lead;
                        cmdAdd.Parameters[15].Value = row.speed;
                        cmdAdd.Parameters[16].Value = row.boulder;
                        cmdAdd.ExecuteNonQuery();
                        insP.Add(row.iid);
                    }
                else
                    foreach (var row in service.GetClimbersForCompetition(lcn.compID.Value,true,lcn.LoadPhoto,
                        null, null))
                    {
                        if (row.Link.queue_pos > 0)
                            continue;
                        if (row.Link.queue_Lead > 0 && row.Link.queue_Speed > 0 && row.Link.queue_Boulder > 0)
                            continue;
                        cmdCheck.Parameters[0].Value = row.Link.secretary_id;
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                            cmdAdd.CommandText = sUpdate;
                        else
                            cmdAdd.CommandText = sInsert;
                        cmdAdd.Parameters[0].Value = row.Climber.name;
                        cmdAdd.Parameters[1].Value = row.Climber.surname;
                        cmdAdd.Parameters[2].Value = row.Climber.age.Value;
                        cmdAdd.Parameters[3].Value = row.Climber.genderFemale;
                        cmdAdd.Parameters[4].Value = row.Team.SecretaryId;
                        cmdAdd.Parameters[5].Value = row.Group.Iid;
                        if (row.Link.qf == null)
                            cmdAdd.Parameters[6].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[6].Value = row.Link.qf;
                        if (row.Link.rankingLead == null)
                            cmdAdd.Parameters[7].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[7].Value = row.Link.rankingLead;
                        if (row.Link.rankingSpeed == null)
                            cmdAdd.Parameters[8].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[8].Value = row.Link.rankingSpeed;
                        if (row.Link.rankingBoulder == null)
                            cmdAdd.Parameters[9].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[9].Value = row.Link.rankingBoulder;
                        cmdAdd.Parameters[10].Value = row.Link.vk;
                        cmdAdd.Parameters[11].Value = row.Link.nopoints;
                        if (row.Climber.photo == null)
                            cmdAdd.Parameters[12].Value = DBNull.Value;
                        else
                            cmdAdd.Parameters[12].Value = row.Climber.photo;
                        cmdAdd.Parameters[13].Value = row.Link.secretary_id;
                        cmdAdd.Parameters[14].Value = row.Link.queue_Lead < 1 ? row.Link.lead : 0;
                        cmdAdd.Parameters[15].Value = row.Link.queue_Speed < 1 ? row.Link.speed : 0;
                        cmdAdd.Parameters[16].Value = row.Link.queue_Boulder < 1 ? row.Link.boulder : 0;
                        cmdAdd.ExecuteNonQuery();
                        insP.Add(row.Link.secretary_id);
                    }

                bool confRemove = true;
                lcn.form.Invoke(new EventHandler(delegate
                {
                    confRemove = (MessageBox.Show(lcn.form, "Данные успешно загружены. Удалить остальные данные из БД?", "",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                }));
                if (confRemove)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "";
                    string inGr = "";
                    if (groups.Count > 0)
                    {
                        foreach (int gd in groups)
                            inGr += "," + gd.ToString();
                        inGr = " NOT IN (" + inGr.Substring(1) + ")";
                    }
                    string inT = "";
                    if (teams.Count > 0)
                    {
                        foreach (int gd in teams)
                            inT += "," + gd.ToString();
                        inT = " NOT IN (" + inT.Substring(1) + ")";
                    }
                    string inP = "";
                    if (insP.Count > 0)
                    {
                        foreach (int n in insP)
                            inP += "," + n.ToString();
                        inP = " NOT IN (" + inP.Substring(1) + ")";
                    }
                    cmd.Transaction = cn.BeginTransaction();
                    try
                    {
                        int tmC, grC, pC;
                        cmd.CommandText = "DELETE FROM generalResults ";
                        if (inP.Length > 0)
                            cmd.CommandText += "WHERE climber_id " + inP;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM routeResults ";
                        if (inP.Length > 0)
                            cmd.CommandText += "WHERE climber_id " + inP;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM speedResults ";
                        if (inP.Length > 0)
                            cmd.CommandText += "WHERE climber_id " + inP;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM boulderResults ";
                        if (inP.Length > 0)
                            cmd.CommandText += "WHERE climber_id " + inP;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM Participants ";
                        if (inP.Length > 0)
                            cmd.CommandText += "WHERE iid " + inP;
                        pC = cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM teamResults ";
                        if (inT.Length > 0)
                            cmd.CommandText += " WHERE team_id " + inT;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM Teams ";
                        if (inT.Length > 0)
                            cmd.CommandText += "WHERE iid " + inT;
                        tmC = cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM lists_hdr2 WHERE list_id IN(SELECT iid FROM lists ";
                        if (inGr.Length > 0)
                            cmd.CommandText += " WHERE group_id " + inGr;
                        cmd.CommandText += ")";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM lists ";
                        if (inGr.Length > 0)
                            cmd.CommandText += "WHERE group_id " + inGr;
                        cmd.CommandText = "DELETE FROM Groups ";
                        if (inGr.Length > 0)
                            cmd.CommandText += " WHERE iid " + inGr;
                        grC = cmd.ExecuteNonQuery();
                        if (pC + tmC + grC == 0)
                            cmd.Transaction.Rollback();
                        else
                        {
                            lcn.form.Invoke(new EventHandler(delegate
                            {
                                if (MessageBox.Show(lcn.form, "Будет удалено " + pC.ToString() + " участников, " +
                                    tmC.ToString() + " команд и " + grC.ToString() + " возрастных групп. Продолжить?",
                                    "Удаление лишних данных", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) ==
                                     DialogResult.Yes)
                                    cmd.Transaction.Commit();
                                else
                                    cmd.Transaction.Rollback();
                            }));
                        }

                    }
                    catch (Exception ex)
                    {
                        if (cmd.Transaction != null)
                            try { cmd.Transaction.Rollback(); }
                            catch { }
                        throw ex;
                    }
                }
                lcn.form.Invoke(new EventHandler(delegate { lcn.form.RefreshCbTeams(); }));
            }
            catch (ThreadAbortException) { threadAbort = true; }
            catch (Exception ex)
            {
                try
                {
                    lcn.form.Invoke(new EventHandler(delegate
                    {
                        MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                    }));
                }
                catch { }
            }
            finally
            {
                try
                {
                    if (remote != null)
                        remote.Close();
                }
                catch { }
                try
                {
                    if (cn != null)
                        cn.Close();
                }
                catch { }
                if (!threadAbort)
                    try
                    {
                        if (lcn.Callback != null)
                            lcn.Callback(MainForm.ThreadStats.IMG_LOAD_REM);
                    }
                    catch { }
            }
        }

        

        private DataTable SetDataGrid()
        {
            try
            {
                if (cancelEvent)
                    return null;
                cancelEvent = true;
                btnLoadToInet.Enabled = gbInetPhoto.Visible && (tbPartNumber.Text.Length > 0);
                if (cbPartGroups.SelectedIndex < 0)
                    cbPartGroups.SelectedIndex = 0;
                if (cbPartTeams.SelectedIndex < 0)
                    cbPartTeams.SelectedIndex = 0;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = cn;
                string initSelect = "SELECT p.iid AS [" + BIB + "], p.surname AS [" + SURNAME + "], p.name " +
                            "AS [" + NAME + "], dbo.fn_getTeamName(p.iid,NULL) AS [" + TEAM_ST + "], p.age AS [" + AGE_ST + "], p.Qf AS [" + QF_ST + "], g.name AS [" + GROUP + "], " +
                            "p.rankingLead AS [" + LEAD + "], p.rankingSpeed AS [" + SPEED + "], p.rankingBoulder AS [" + BOULDER + "], " +
                            "p.lead AS [" + LEAD_S + "], p.speed AS [" + SPEED_S + "], p.boulder AS [" + BOULDER_S + "],p.iid AS [" + SCR_NEW_BIB + "] " +
                            "FROM Participants p INNER JOIN Teams t ON p.team_id = t.iid INNER JOIN Groups g " +
                            "ON p.group_id = g.iid ";
                if (cbPartGroups.SelectedIndex == 0)
                {
                    if (cbPartTeams.SelectedIndex == 0)
                        da.SelectCommand.CommandText = initSelect;
                    else
                    {
                        da.SelectCommand.CommandText = initSelect + "WHERE t.name = @name";
                        da.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        da.SelectCommand.Parameters[0].Value = cbPartTeams.SelectedItem.ToString();
                    }
                }
                else
                {
                    if (cbPartTeams.SelectedIndex == 0)
                    {
                        da.SelectCommand.CommandText = initSelect + "WHERE g.name = @name";
                        da.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        da.SelectCommand.Parameters[0].Value = cbPartGroups.SelectedItem.ToString();
                    }
                    else
                    {
                        da.SelectCommand.CommandText = initSelect + "WHERE (g.name = @gname) AND (t.name = @tname)";
                        da.SelectCommand.Parameters.Add("@gname", SqlDbType.VarChar, 50);
                        da.SelectCommand.Parameters[0].Value = cbPartGroups.SelectedItem.ToString();
                        da.SelectCommand.Parameters.Add("@tname", SqlDbType.VarChar, 50);
                        da.SelectCommand.Parameters[1].Value = cbPartTeams.SelectedItem.ToString();
                    }
                }

                da.SelectCommand.CommandText += " ORDER BY t.name, g.genderFemale, g.oldYear, p.surname, p.name";
                dataTable = new DataTable();
                try
                {
                    da.Fill(dataTable);
                    try { dataTable.Columns[LEAD].ColumnName = SCR_RANK_LEAD_COL; }
                    catch { }
                    try { dataTable.Columns[SPEED].ColumnName = SCR_RANK_SPEED_COL; }
                    catch { }
                    try { dataTable.Columns[BOULDER].ColumnName = SCR_RANK_BOULDER_COL; }
                    catch { }
                    dg.Columns.Clear();
                    //foreach (DataColumn dc in dataTable)
                    //{
                    //    DataGridViewColumn c = new DataGridViewTextBoxColumn();
                    //    c.
                    //}
                    dg.DataSource = dataTable;
                    for (int i = 0; i < dg.Columns.Count; i++)
                    {
                        DataGridViewColumn clmn = dg.Columns[i];
                        if ((clmn.HeaderText.Equals(LEAD_S, StringComparison.InvariantCultureIgnoreCase) ||
                            clmn.HeaderText.Equals(SPEED_S, StringComparison.InvariantCultureIgnoreCase) ||
                            clmn.HeaderText.Equals(BOULDER_S, StringComparison.InvariantCultureIgnoreCase))
                            && !(clmn is DataGridViewComboBoxColumn))
                        {
                            DataGridViewComboBoxColumn l = new DataGridViewComboBoxColumn();
                            l.DataPropertyName = clmn.DataPropertyName;
                            l.HeaderText = clmn.HeaderText;
                            l.DataSource = RegTypeTable;
                            l.DisplayMember = "text";
                            l.ValueMember = "value";
                            l.DisplayIndex = clmn.DisplayIndex;
                            dg.Columns.Add(l);
                            dg.Columns.Remove(clmn);
                            i--;
                        }
                    }
                    dg.DataSource = dataTable;
                    /*cbClmn.DataSource = RegTypeTable;
                    cbClmn.DisplayMember = "text";
                    cbClmn.ValueMember = "value";
                    dg.Columns.Add(cbClmn);*/
                    changed = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Ошибка обновления списка:\r\n" + ex.Message);
                    return dataTable;
                }
                //dg.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                cancelEvent = false;
                try { dg.Columns[0].ReadOnly = true; }
                catch { }
#if !FULL
                try
                {
                    dg.Columns[rankLeadCol].Visible = dg.Columns[rankBoulderCol].Visible =
                        dg.Columns["Тр."].Visible = dg.Columns["Ск."].Visible = dg.Columns["Боулд."].Visible
                        = false;
                    dg.Columns[rankSpeedCol].HeaderText = rankingColumn;
                }
                catch { }
#endif

                return dataTable;
            }
            catch { return new DataTable(); }
        }

        private void SetNumbersInARow()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = cn.BeginTransaction();
            try
            {
                cmd.CommandText = @"SELECT p.iid, t.iid team, g.iid grp
                                  FROM Participants p
                                  JOIN teams t ON t.iid = p.team_id
                                  JOIN groups g ON g.iid = p.group_id
                              ORDER BY ";
                AutoNum an = SettingsForm.GetAutoNum(cn, cmd.Transaction);
                switch (an)
                {
                    case AutoNum.GRP:
                        cmd.CommandText += "g.iid, t.name, p.surname, p.name, p.age, p.qf";
                        break;
                    case AutoNum.ROW:
                        cmd.CommandText += "t.name, g.genderFemale, g.oldYear, p.surname, p.name, p.age, p.qf";
                        break;
                    default:
                        goto case AutoNum.ROW;
                }
                List<NumChange> changeList = new List<NumChange>();
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                        changeList.Add(new NumChange(Convert.ToInt32(rdr["iid"]),
                            Convert.ToInt32(rdr["grp"]), Convert.ToInt32(rdr["team"])));
                }
                finally { rdr.Close(); }
                int nxtNum = (int)StaticClass.GetNextIID("Participants", cn, "iid", cmd.Transaction);
                if (nxtNum < 20000)
                    nxtNum = 20000;
                foreach (NumChange nc in changeList)
                {
                    while (checkNum(++nxtNum, cmd.Transaction)) ;
                    nc.Intermediate = nxtNum;
                }
                cmd.CommandText = "UPDATE Participants SET iid = @n WHERE iid = @o";
                cmd.Parameters.Add("@n", SqlDbType.Int);
                cmd.Parameters.Add("@o", SqlDbType.Int);
                foreach (NumChange nc in changeList)
                {
                    cmd.Parameters[0].Value = nc.Intermediate;
                    cmd.Parameters[1].Value = nc.Old;
                    cmd.ExecuteNonQuery();
                }
                switch (SettingsForm.GetAutoNum(cn, cmd.Transaction))
                {
                    case AutoNum.ROW:
                        int curNum = 0;
                        foreach (NumChange nc in changeList)
                            nc.New = ++curNum;
                        break;
                    case AutoNum.GRP:
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT MAX(iid) FROM Groups";
                        int grpCount = Convert.ToInt32(cmd.ExecuteScalar());
                        GrDataCollection grList = new GrDataCollection();
                        foreach (NumChange nc in changeList)
                        {
                            if (grList[nc.Group.ToString()] == null)
                            {
                                grList.Add(new GrData(nc.Group));
                                nc.New = (nc.Group - 1) * 100 + 1;
                            }
                            else
                            {
                                int lastNum = grList[nc.Group.ToString()].LastNum;
                                if (lastNum % 100 == 99)
                                {
                                    int ser;
                                    if (grList[nc.Group.ToString()].Series == (nc.Group - 1) * 100)
                                        ser = (grpCount - 1) * 100;
                                    else
                                        ser = grList[nc.Group.ToString()].Series;
                                    bool serOcc;
                                    do
                                    {
                                        ser += 100;
                                        serOcc = false;
                                        foreach (GrData gd in grList)
                                            if (gd.Series >= ser)
                                            {
                                                serOcc = true;
                                                break;
                                            }
                                    } while (serOcc);
                                    lastNum = ser + 1;
                                }
                                else
                                    lastNum++;
                                nc.New = lastNum;
                            }
                            grList[grList.Count - 1].LastNum = nc.New;
                        }
                        break;
                    default: goto case AutoNum.ROW;
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE Participants SET iid = @n WHERE iid = @o";
                cmd.Parameters.Add("@n", SqlDbType.Int);
                cmd.Parameters.Add("@o", SqlDbType.Int);
                foreach (NumChange nc in changeList)
                {
                    cmd.Parameters[0].Value = nc.New;
                    cmd.Parameters[1].Value = nc.Intermediate;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                try
                {
                    MessageBox.Show(this, "Номера упорядочены");
                    SetDataGrid();
                }
                catch (Exception ex) { MessageBox.Show(this, "Ошибка выборки данных:\r\n" + ex.Message); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка упорядочивания номеров:\r\n" + ex.Message);
                try { cmd.Transaction.Rollback(); }
                catch { }
            }
        }

        private bool checkNum(int num, SqlTransaction tran)
        {
            return SortingClass.checkNum(num, "Participants", cn, tran);
        }

        private class GrDataCollection : List<GrData>
        {
            public GrData this[string iid]
            {
                get
                {
                    int id = int.Parse(iid);
                    foreach (GrData gd in this)
                        if (gd.Iid == id)
                            return gd;
                    return null;
                }
            }
        }

        private class GrData
        {
            private int iid;
            public int Iid { get { return iid; } }

            public int Series { get { return (LastNum - (LastNum % 100)); } }

            private int lastNum;
            public int LastNum
            {
                get { return lastNum; }
                set { lastNum = value; }
            }

            public GrData(int iid)
            {
                this.iid = iid;
                this.lastNum = 0;
            }
        }

        private class NumChange
        {
            private int old, intermediate, final, team, group;
            public int Old { get { return old; } }
            public int Team { get { return team; } }
            public int Group { get { return group; } }
            public int Intermediate
            {
                get { return intermediate; }
                set
                {
                    if (value < 1)
                        throw new ArgumentOutOfRangeException("Номер должен быть >= 1");
                    intermediate = value;
                }
            }
            public int New
            {
                get { return final; }
                set
                {
                    if (value < 1)
                        throw new ArgumentOutOfRangeException("Номер должен быть >= 1");
                    final = value;
                }
            }

            public NumChange(int old, int group, int team)
            {
                this.intermediate = this.final = -1;
                this.old = old;
                this.group = group;
                this.team = team;
            }
        }

        private void btnGetTable_Click(object sender, EventArgs e)
        {
            if (SaveData())
            {
                MessageBox.Show(this, "Изменения успешно применены");
                SetDataGrid();
            }
        }

        private bool SaveData()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE Participants SET iid=@n_i, rankingBoulder = @b, rankingLead=@l, " +
                    "rankingSpeed=@s, lead=@lead, speed=@speed, boulder=@boulder WHERE iid=@iid";
                cmd.Parameters.Add("@b", SqlDbType.Int, 4, SCR_RANK_BOULDER_COL);
                cmd.Parameters.Add("@s", SqlDbType.Int, 4, SCR_RANK_SPEED_COL);
                cmd.Parameters.Add("@l", SqlDbType.Int, 4, SCR_RANK_LEAD_COL);
                cmd.Parameters.Add("@iid", SqlDbType.Int, 4, "Номер");
                cmd.Parameters.Add("@lead", SqlDbType.SmallInt, 2, "Тр.");
                cmd.Parameters.Add("@speed", SqlDbType.SmallInt, 2, "Ск.");
                cmd.Parameters.Add("@boulder", SqlDbType.SmallInt, 2, "Боулд.");
                cmd.Parameters.Add("@n_i", SqlDbType.Int, 4, "Нов.номер");
                cmd.Transaction = cn.BeginTransaction();
                string sError = "";
                try
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        for (int i = 0; i < cmd.Parameters.Count; i++)
                            cmd.Parameters[i].Value = dr[cmd.Parameters[i].SourceColumn];
                        try { cmd.ExecuteNonQuery(); }
                        catch (Exception exc)
                        {
                            sError += "Ошибка обновления данных участника " + dr[1].ToString() + " " + dr[2].ToString() +
                                " из команды " + dr[3].ToString() + ": " + exc.Message + "\r\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    sError += "Ошибка обновления списка участников: " + ex.Message + "\r\n";
                }
                finally
                {
                    if (sError.Length > 0)
                    {
                        cmd.Transaction.Rollback();
                        MessageBox.Show(this, "При обновлении списка участников произошли следующие ошибки:\r\n" +
                            sError);
                    }
                    else
                        cmd.Transaction.Commit();
                }
                return (sError.Length == 0);
            }
            catch (Exception exE)
            {
                MessageBox.Show(this, "Ошибка обновления списка участников:\r\n" +
                    exE.Message);
                return false;
            }
        }

        private void cbPartGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDataGrid();
        }

        public override void RefreshAndReload()
        {
            SetDataGrid();
        }

        private void cbPartTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            SetDataGrid();
        }

        private void btnPartAdd_Click(object sender, EventArgs e)
        {
            grIndx = cbPartGroups.SelectedIndex;
            tmIndx = cbPartTeams.SelectedIndex;
            string str = "";
            if (cbPartTeams.SelectedIndex > 0)
                str = cbPartTeams.SelectedItem.ToString();
            SetEditModeParts(true);
            ClearParts();
            addParts = true;
            if (str != "")
                cbPartTeams.SelectedIndex = cbPartTeams.Items.IndexOf(str);
            cbPartGroups.SelectedIndex = cbPartGroups.Items.IndexOf(SCR_DEFAULT_GROUP);
        }

        private void ClearParts()
        {
            cancelEvent = true;
            cbPartTeams.SelectedIndex = cbPartGroups.SelectedIndex = cbPartQf.SelectedIndex =
                cbPartSex.SelectedIndex = -1;
            cbPartTeams.Text = "Выберите команду";
            tbPartAge.Text = tbPartBoulder.Text = tbPartGroup.Text = tbPartLead.Text =
                tbPartName.Text = tbPartNumber.Text = tbPartSpeed.Text = tbPartSurname.Text =
                tbPartTeam.Text = "";
            bPartLateAppl.Checked = bPartNoPoints.Checked = bPartVk.Checked = false;
            cbPartSex.Text = "Пол";
            cbPartQf.Text = "Разряд";
        }

        private void dg_SelectionChanged(object sender, EventArgs e)
        {
            if (cancelEvent)
                return;
            int iid;
            try { iid = Convert.ToInt32(dg.CurrentRow.Cells[0].Value); }
            catch { return; }
            btnLoadToInet.Enabled = gbInetPhoto.Visible;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT p.*, t.name AS tname, g.name AS gname FROM Participants p " +
                "INNER JOIN Teams t ON p.team_id=t.iid INNER JOIN Groups g ON p.group_id=g.iid " +
                "WHERE p.iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters[0].Value = iid;
            SqlDataReader r = cmd.ExecuteReader();
            try
            {
                //int phIn = r.GetOrdinal("photo"), bufSize = int.MaxValue;
                if (r.Read())
                {
                    tbPartNumber.Text = r["iid"].ToString();
                    if (r["Age"].ToString() == "0")
                        tbPartAge.Text = "";
                    else
                        tbPartAge.Text = r["Age"].ToString();
                    tbPartGroup.Text = r["gname"].ToString();
                    tbPartName.Text = r["name"].ToString();
                    tbPartSurname.Text = r["surname"].ToString();
                    tbPartTeam.Text = r["tname"].ToString();
                    bPartLateAppl.Checked = Convert.ToBoolean(r["lateAppl"]);
                    bPartNoPoints.Checked = Convert.ToBoolean(r["noPoints"]);
                    bPartVk.Checked = Convert.ToBoolean(r["vk"]);
                    string qq = r["Qf"].ToString();
                    cbPartQf.SelectedIndex = cbPartQf.Items.IndexOf(qq);
                    cbPartSex.SelectedIndex =
                        (Convert.ToBoolean(r["genderFemale"]) ? 1 : 0);
                    /*cbPartSex.SelectedIndex = cbPartSex.Items.IndexOf("Ж");
                else
                    cbPartSex.SelectedIndex = cbPartSex.Items.IndexOf("М");*/
                    tbPartBoulder.Text = r["rankingBoulder"].ToString();
                    tbPartLead.Text = r["rankingLead"].ToString();
                    tbPartSpeed.Text = r["rankingSpeed"].ToString();
                    /*if (r.IsDBNull(phIn))
                        pbPhoto.Image = null;
                    else
                    {
                        try
                        {
                            byte[] photo = new byte[100];
                            bool b = true;
                            do
                            {
                                try { photo = new byte[bufSize]; b = false; }
                                catch (OutOfMemoryException)
                                { bufSize /= 2; }
                            } while (b);
                            // Create a file to hold the output.
                            //FileStream stream = new FileStream(
                            //  "logo.bmp", FileMode.OpenOrCreate, FileAccess.Write);
                            MemoryStream stream = new MemoryStream();
                            BinaryWriter writer = new BinaryWriter(stream);

                            // Reset the starting byte for the new BLOB.
                            long startIndex = 0;

                            // Read bytes into outByte[] and retain the number of bytes returned.
                            long retval = r.GetBytes(phIn, startIndex, photo, 0, bufSize);

                            // Continue while there are bytes beyond the size of the buffer.
                            while (retval == bufSize)
                            {
                                writer.Write(photo);
                                writer.Flush();

                                // Reposition start index to end of last buffer and fill buffer.
                                startIndex += bufSize;
                                retval = r.GetBytes(phIn, startIndex, photo, 0, bufSize);
                            }

                            // Write the remaining buffer.
                            writer.Write(photo, 0, (int)retval - 1);
                            writer.Flush();


                            pbPhoto.Image = Image.FromStream(stream);
                            // Close the output file.
                            writer.Close();
                            stream.Close();
                        }
                        catch { }
                    }*/
                }
            }
            finally { r.Close(); }
            try
            { btnSavePhoto.Enabled = ((pbPhoto.Image = LoadImageFromDB()) != null); }
            catch (Exception ex)
            {
                pbPhoto.Image = null;
                btnSavePhoto.Enabled = false;
                MessageBox.Show("Ошибка загрузки фотографии:\r\n" + ex.Message);
            }
        }

        private void LoadImage()
        {
            try
            {
                OpenFileDialog dg = new OpenFileDialog();
                dg.ShowDialog();
                Image img = Image.FromFile(dg.FileName);
                pbPhoto.Image = img;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            btnSavePhoto.Enabled = (pbPhoto.Image != null);
        }

        private Image LoadImageFromDB()
        {
            int iid;
            if (!int.TryParse(tbPartNumber.Text, out iid))
                return null;
            return LoadImageFromDB(iid, cn);
        }

        private static Image LoadImageFromDB(int iid, SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            PhotoLocalTableAdapter ta = new PhotoLocalTableAdapter();
            ta.Connection = cn;
            foreach (dsClimbing.PhotoLocalRow row in ta.GetDataByIid(iid))
            {
                if (row.IsphotoNull())
                    return null;
                return ImageWorker.GetFromBytes(row.photo);
            }
            return null;
        }

        private static bool SaveImageToLocal(Image img, int iid, SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            cmd.Connection = cn;
            cmd.CommandText = "UPDATE Participants SET photo=@photo WHERE iid=" + iid.ToString();
            cmd.Parameters.Add("@photo", SqlDbType.Image);
            if (img == null)
                cmd.Parameters[0].Value = DBNull.Value;
            else
                cmd.Parameters[0].Value = ImageWorker.GetBytesFromImage(img);
            return (cmd.ExecuteNonQuery() > 0);
        }

        private void SaveImageToLocal(Image img, SqlTransaction tran)
        {
            try
            {
                int iid;
                if (!int.TryParse(tbPartNumber.Text, out iid))
                    return;
                SaveImageToLocal(img, iid, cn, tran);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения фотографии:\r\n" +
                    ex.Message);
            }
        }

        private void SaveImageToRemote(Image img)
        {
            int iid;
            if (!int.TryParse(tbPartNumber.Text, out iid))
                return;
            if (compIDForService == null)
            {
                if (remote == null)
                    return;
                dsClimbing.PhotoRemoteDataTable dt = new dsClimbing.PhotoRemoteDataTable();
                dsClimbing.PhotoRemoteRow row = dt.NewPhotoRemoteRow();
                row.iid = iid;
                if (img == null)
                    row.SetphotoNull();
                else
                    row.photo = ImageWorker.GetBytesFromImage(img);
                Thread thr = new Thread(DbUpdate);
                thr.Start(row);
            }
            else
            {
                PartImage pi = new PartImage(img, iid, compIDForService);
                Thread thr = new Thread(SavePhotoViaService);
                thr.Start(pi);
            }
        }
        private void SavePhotoViaService(object obj)
        {
            PartImage p = obj as PartImage;
            if (p == null)
                return;
            try
            {
                if (p.compID == null || !p.compID.HasValue)
                    return;
                ClimbingService service = new ClimbingService();
                byte[] imageToPost =
                    p.img == null ? null :
                    ImageWorker.GetBytesFromImage(p.img);
                service.PostPhotoForClimber(imageToPost, p.iid, p.compID.Value);
                MessageBox.Show("Фото участника №" + p.iid.ToString() + " успешно загружено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки фото участника №" + p.iid.ToString() + ":\r\n" +
                    ex.Message);
            }
        }
        private class PartImage
        {
            public PartImage(Image image, int iid, long? compID = null)
            {
                this.iid = iid;
                this.img = image;
                this.compID = compID;
            }
            public Image img { get; private set; }
            public int iid { get; private set; }
            public long? compID { get; private set; }
        }

        private void LoadImagesFromFolder(object obj)
        {
            if (!(obj is LoadSaveImagesThreadArg))
                return;
            LoadSaveImagesThreadArg ta = (LoadSaveImagesThreadArg)obj;
            SqlConnection cnL = new SqlConnection(cn.ConnectionString);
            bool ThreadAbort = false;
            try
            {
                string folder = ta.Path;
                if (cnL.State != ConnectionState.Open)
                    cnL.Open();
                DirectoryInfo dirI = new DirectoryInfo(folder);
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo fi in dirI.GetFiles("*", SearchOption.AllDirectories))
                    if (fi.Name.ToLower().IndexOf(".jpeg") > -1 || fi.Name.ToLower().IndexOf(".jpg") > -1 || fi.Name.ToLower().IndexOf(".png") > -1
                        || fi.Name.ToLower().IndexOf(".bmp") > -1)
                        files.Add(fi);

                List<PartImage> images = new List<PartImage>();
                foreach (FileInfo fi in files)
                    try
                    {
                        List<int> idL = StaticClass.GetNumberSeq(fi.Name);
                        if (idL.Count < 1)
                            continue;
                        images.Add(new PartImage(Image.FromFile(fi.FullName), idL[0]));
                    }
                    catch (ThreadAbortException ext) { throw ext; }
                    catch { }
                SqlTransaction tran = cnL.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    int cnt = 0;
                    foreach (PartImage pi in images)
                        if (SaveImageToLocal(pi.img, pi.iid, cnL, tran))
                            cnt++;
                    this.Invoke(new EventHandler(delegate
                    {
                        if (MessageBox.Show(this, "Фотографии у " + cnt.ToString() + " участников будут обновлены. Продолжить?",
                            "Обновить фотографии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                             DialogResult.Yes)
                            tran.Commit();
                        else
                            tran.Rollback();
                    }));
                }
                catch (Exception exIn)
                {
                    try { tran.Rollback(); }
                    catch { }
                    throw exIn;
                }
            }
            catch (ThreadAbortException) { ThreadAbort = true; }
            catch (Exception ex)
            {
                this.Invoke(new EventHandler(delegate
                {
                    MessageBox.Show(this, "Ошибка загрузки фотографий:\r\n" + ex.Message);
                }));
            }
            finally
            {
                try { cnL.Close(); }
                catch { }
                if (!ThreadAbort)
                    try
                    {
                        if (ta.Callback != null)
                            ta.Callback(MainForm.ThreadStats.IMG_LOAD_F);
                        //this.Invoke(new EventHandler(delegate
                        //{
                        //    MainForm mf = (MainForm)this.MdiParent;
                        //    mf.RemThrData(MainForm.ThreadStats.IMG_LOAD_F);
                        //}));
                    }
                    catch { }
            }
        }

        private void SaveImagesToFolder(object obj)
        {
            LoadSaveImagesThreadArg tta = obj as LoadSaveImagesThreadArg;
            if (tta == null)
                return;
            bool threadAbort = false;
            SqlConnection cnL = new SqlConnection(cn.ConnectionString);
            try
            {
                string path = tta.Path;
                string pathTmp;
                if (cnL.State != ConnectionState.Open)
                    cnL.Open();
                PhotoLocalTableAdapter ta = new PhotoLocalTableAdapter();
                ta.Connection = cnL;
                dsClimbing.PhotoLocalDataTable dt = ta.GetDataWhereNotNull();
                string curGroup = "";
                DirectoryInfo bDir = new DirectoryInfo(path);
                DirectoryInfo cDir = null;
                int saved = 0;
                foreach (dsClimbing.PhotoLocalRow row in dt.Rows)
                {
                    if (curGroup != row.gr_name || cDir == null)
                    {
                        curGroup = row.gr_name;
                        pathTmp = Path.Combine(bDir.FullName, curGroup);
                        if (!Directory.Exists(pathTmp))
                            cDir = bDir.CreateSubdirectory(curGroup);
                        else
                            cDir = new DirectoryInfo(pathTmp);
                    }
                    Bitmap btm = new Bitmap(ImageWorker.GetFromBytes(row.photo));
                    string fName = cDir.FullName + "\\" + row.iid.ToString("000") + "_" + row.surname + "_" + row.name + ".jpg";
                    if (File.Exists(fName))
                        try { File.Delete(fName); }
                        catch { }
                    try
                    {
                        btm.Save(fName, ImageFormat.Jpeg);
                        saved++;
                    }
                    catch (ThreadAbortException exT) { throw exT; }
                    catch (Exception ex)
                    {
                        bool bCont = true;
                        this.Invoke(new EventHandler(delegate
                        {
                            bCont = (MessageBox.Show(this, "Ошибка при сохранении фото в файл " + fName + ":\r\n" +
                                ex.Message + "\r\nПродолжить?", "Ошибка сохранения", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                            == DialogResult.Yes);
                        }));
                        if (!bCont)
                            break;
                    }
                }
                this.Invoke(new EventHandler(delegate { MessageBox.Show(this, "Сохранение закончено. Всего сохранено " + saved.ToString() + " фотографий."); }));
            }
            catch (ThreadAbortException) { threadAbort = true; }
            catch (Exception ex)
            {
                try
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        MessageBox.Show(this, "Ошибка сохранения фотографий:\r\n" + ex.Message);
                    }));
                }
                catch { }
            }
            finally
            {
                try { cnL.Close(); }
                catch { }
                if (!threadAbort)
                    try
                    {
                        if (tta.Callback != null)
                            tta.Callback(MainForm.ThreadStats.IMG_SAV_F);
                        //this.Invoke(new EventHandler(delegate
                        //{
                        //    MainForm mf = (MainForm)this.MdiParent;
                        //    mf.RemThrData(MainForm.ThreadStats.IMG_SAV_F);
                        //}));
                    }
                    catch { }
            }
        }

        private Mutex mRem = new Mutex();
        private void DbUpdate(object obj)
        {
            dsClimbing.PhotoRemoteRow row = obj as dsClimbing.PhotoRemoteRow;
            if (row == null)
                return;
            try
            {
                mRem.WaitOne();
                try
                {
                    if (!allowRem)
                        return;
                    if (remote == null)
                        return;
                    if (remote.State != ConnectionState.Open)
                        remote.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = remote;
                    cmd.CommandText = "UPDATE ONLclimbers SET photo=@photo WHERE iid=" + row.iid.ToString();
                    cmd.Parameters.Add("@photo", SqlDbType.Image);
                    if (row.IsphotoNull())
                        cmd.Parameters[0].Value = DBNull.Value;
                    else
                        cmd.Parameters[0].Value = row.photo;
                    cmd.ExecuteNonQuery();
                }
                finally { mRem.ReleaseMutex(); }
                MessageBox.Show("Фото участника №" + row.iid.ToString() + " успешно загружено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки фото участника №" + row.iid.ToString() + ":\r\n" +
                    ex.Message);
            }
        }

        private int grIndx = 0, tmIndx = 0;

        private static object getRanking(string str, out string txt)
        {
            txt = str;
            double dTmp;
            int res;
            if (double.TryParse(txt, out dTmp))
                res = (int)dTmp;
            else
            {
                txt = txt.Replace(',', '.');
                if (double.TryParse(txt, out dTmp))
                    res = (int)dTmp;
                else
                {
                    txt = txt.Replace('.', ',');
                    if (double.TryParse(txt, out dTmp))
                        res = (int)dTmp;
                    else
                        res = -1;
                }
            }
            if (res <= 0)
            {
                txt = "";
                return DBNull.Value;
            }
            else
            {
                txt = res.ToString();
                return res;
            }
        }

        private static object getRanking(TextBox tb)
        {
            string modStr;
            object res = getRanking(tb.Text, out modStr);
            tb.Text = modStr;
            return res;
        }

        private void btnPartEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnPartEdit.Text == EDIT)
                {
                    if (tbPartNumber.Text == "")
                    {
                        MessageBox.Show("Участник не выбран");
                        return;
                    }
                    grIndx = cbPartGroups.SelectedIndex;
                    tmIndx = cbPartTeams.SelectedIndex;
                    string str = tbPartTeam.Text;
                    SetEditModeParts(true);
                    addParts = false;
                    cbPartTeams.SelectedIndex = cbPartTeams.Items.IndexOf(str);
                }
                else
                {
                    if (tbPartSurname.Text == "")
                    {
                        MessageBox.Show("Введите фамилию участника");
                        return;
                    }
                    if (cbPartSex.SelectedIndex == -1)
                    {
                        MessageBox.Show("Пол не выбран");
                        return;
                    }
                    if (cbPartQf.SelectedIndex == -1)
                    {
                        MessageBox.Show("Разряд не выбран");
                        return;
                    }
                    if (cbPartTeams.SelectedIndex == -1)
                    {
                        MessageBox.Show("Команда не выбрана");
                        return;
                    }
                    int age;
                    try
                    {
                        if (tbPartAge.Text == "")
                            age = 0;
                        else
                        {
                            age = Convert.ToInt32(tbPartAge.Text);
                            if (age < 0)
                                throw new Exception("Неправильный возраст");
                            if (age >= 0 && age <= 10)
                                age += 2000;
                            if (age > 10 && age < 100)
                                age += 1900;
                            tbPartAge.Text = age.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    object lead, speed, boulder;
                    lead = getRanking(tbPartLead);
                    speed = getRanking(tbPartSpeed);
                    boulder = getRanking(tbPartBoulder);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.Transaction = cn.BeginTransaction();
                    bool transactionSucces = false;
                    bool gender = cbPartSex.Text.Equals("f", StringComparison.InvariantCultureIgnoreCase) ||
                        cbPartSex.Text.Equals("ж", StringComparison.InvariantCultureIgnoreCase);
                    int iid = -1, g_id = -1;
                    try
                    {
                        cmd.CommandText = "SELECT iid FROM Teams WHERE name = @name";
                        cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        cmd.Parameters[0].Value = cbPartTeams.SelectedItem.ToString();
                        int t_id;
                        try { t_id = Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch
                        {
                            MessageBox.Show("Ошибка выбора команды");
                            return;
                        }
                        cmd.Parameters.Clear();

                        cmd.CommandText = "SELECT iid FROM Groups WHERE (genderFemale = @gf) AND " +
                            "(oldYear <= @age) AND (youngYear >= @age)";
                        cmd.Parameters.Add("@gf", SqlDbType.Bit);
                        cmd.Parameters[0].Value = gender;
                        cmd.Parameters.Add("@age", SqlDbType.Int);
                        cmd.Parameters[1].Value = age;
                        try
                        {
                            object res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                                g_id = Convert.ToInt32(cmd.ExecuteScalar());
                            else
                                g_id = 0;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        if (cbPartGroups.Text == SCR_DEFAULT_GROUP && g_id == 0)
                        {
                            MessageBox.Show(this, "Участник не входит ни в одну возрастную группу.\r\n" +
                                "Проверьте пол и возраст или укажите нужную возрастную группу вручную");
                            return;
                        }
                        else if (cbPartGroups.Text != SCR_DEFAULT_GROUP)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "SELECT iid FROM Groups(NOLOCK) WHERE name=@nme";
                            cmd.Parameters.Add("@nme", SqlDbType.VarChar, 255);
                            cmd.Parameters[0].Value = cbPartGroups.Text;
                            int newId;
                            object oTmp = cmd.ExecuteScalar();
                            if (oTmp != null && oTmp != DBNull.Value)
                                newId = Convert.ToInt32(oTmp);
                            else
                            {
                                MessageBox.Show(this, "Выбранной Вами группы не существует.\r\n" +
                                    "Такое может произойти, если кто-нибудь удалил нужную Вам группу.\r\n" +
                                    "Попробуйте закрыть окно \"Приём заявок\" и открыть его снова.");
                                return;
                            }
                            if (g_id != 0 && newId != g_id)
                                if (MessageBox.Show(this, "Данный участник по полу/возрасту должен выступать в другой возрастной группе.\r\n" +
                                    "Вы уверены, что хотите добавить данного участника в выбранную Вами группу?",
                                    "Разные группы", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                                     DialogResult.No)
                                    return;
                            g_id = newId;
                        }


                        if (addParts)
                            iid = GetNextNumber(g_id, cmd.Transaction);
                        else
                        {
                            try { iid = Convert.ToInt32(tbPartNumber.Text); }
                            catch
                            {
                                MessageBox.Show("Участник не выбран");
                                SetEditModeParts(false);
                                return;
                            }
                        }

                        if (addParts)
                            cmd.CommandText = "INSERT INTO Participants(name, surname, age, genderFemale, " +
                                "qf, team_id, group_id, rankingLead, rankingBoulder, rankingSpeed, " +
                                "vk, noPoints, lateAppl, iid) VALUES (@name, @surname, @age, @gF, @qf, " +
                                "@t_id, @g_id, @r_l, @r_b, @r_s, @vk, @nP, @l_ap, @iid)";
                        else
                            cmd.CommandText = "UPDATE Participants SET name=@name, surname=@surname, " +
                                "age=@age, genderFemale=@gF, qf=@qf, team_id=@t_id, group_id=@g_id, " +
                                "rankingLead=@r_l, rankingBoulder=@r_b, rankingSpeed=@r_s, " +
                                "vk=@vk, noPoints=@nP, lateAppl=@l_ap, changed=1 WHERE iid=@iid";
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        cmd.Parameters[0].Value = tbPartName.Text;

                        cmd.Parameters.Add("@surname", SqlDbType.VarChar, 50);
                        cmd.Parameters[1].Value = tbPartSurname.Text;

                        cmd.Parameters.Add("@age", SqlDbType.Int);
                        cmd.Parameters[2].Value = age;

                        cmd.Parameters.Add("@gF", SqlDbType.Bit);
                        cmd.Parameters[3].Value = (cbPartSex.SelectedItem.ToString() == "Ж");

                        cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50);
                        cmd.Parameters[4].Value = cbPartQf.SelectedItem.ToString();

                        cmd.Parameters.Add("@t_id", SqlDbType.Int);
                        cmd.Parameters[5].Value = t_id;

                        cmd.Parameters.Add("@g_id", SqlDbType.Int);
                        cmd.Parameters[6].Value = g_id;

                        cmd.Parameters.Add("@r_l", SqlDbType.Int);
                        cmd.Parameters[7].Value = lead;

                        cmd.Parameters.Add("@r_b", SqlDbType.Int);
                        cmd.Parameters[8].Value = boulder;

                        cmd.Parameters.Add("@r_s", SqlDbType.Int);
                        cmd.Parameters[9].Value = speed;

                        cmd.Parameters.Add("@vk", SqlDbType.Bit);
                        cmd.Parameters[10].Value = bPartVk.Checked;

                        cmd.Parameters.Add("@nP", SqlDbType.Bit);
                        cmd.Parameters[11].Value = bPartNoPoints.Checked;

                        cmd.Parameters.Add("@l_ap", SqlDbType.Bit);
                        cmd.Parameters[12].Value = bPartLateAppl.Checked;

                        cmd.Parameters.Add("@iid", SqlDbType.Int);
                        cmd.Parameters[13].Value = iid;
                        /*
                        cmd.Parameters.Add("@photo", SqlDbType.Image);
                        if (pbPhoto.Image != null)
                        {
                            MemoryStream mStream = new MemoryStream();
                            pbPhoto.Image.Save(mStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            cmd.Parameters[14].Value = mStream.ToArray();
                            mStream.Close();
                        }
                        else
                            cmd.Parameters[14].Value = DBNull.Value;
                         * */
                        try
                        {
                            cmd.ExecuteNonQuery();
                            SaveImageToLocal(pbPhoto.Image, cmd.Transaction);
                            transactionSucces = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Ошибка добавления/правки участника:\r\n" + ex.Message);
                            return;
                        }
                    }
                    finally
                    {
                        if (transactionSucces)
                            cmd.Transaction.Commit();
                        else
                            cmd.Transaction.Rollback();
                    }
                    if (iid < 1 || g_id < 1)
                        return;
                    string str = cbPartTeams.SelectedItem.ToString();
                    SetEditModeParts(false);
                    try
                    {
                        cbPartTeams.SelectedIndex = cbPartTeams.Items.IndexOf(str);
                        tbPartTeam.Text = str;
                        tbPartNumber.Text = iid.ToString();
                        cmd.CommandText = "SELECT name FROM Groups WHERE iid = " + g_id.ToString();
                        cbPartGroups.SelectedIndex = cbPartGroups.Items.IndexOf(cmd.ExecuteScalar().ToString());
                        tbPartGroup.Text = cbPartGroups.SelectedItem.ToString();
                        cbPartTeams.SelectedIndex = tmIndx;
                        cbPartGroups.SelectedIndex = grIndx;
                    }
                    catch { }
                    try { SetDataGrid(); }
                    catch { }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка при добавлении/обновлении участника:\r\n" + ex.Message);
            }
        }

        private static int GetNextNumber(int groupID, SqlTransaction tran, SqlConnection cn)
        {
            return SortingClass.GetNextNumber(groupID, cn, tran, SettingsForm.GetAutoNum(cn, tran), false);
        }

        private int GetNextNumber(int groupID, SqlTransaction tran)
        {
            return GetNextNumber(groupID, tran, cn);
        }

        private void btnPartDel_Click(object sender, EventArgs e)
        {
            if (btnPartDel.Text == CANCEL)
            {
                SetEditModeParts(false);
                return;
            }
            int iid;
            try
            {
                iid = Convert.ToInt32(tbPartNumber.Text);
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
                return;
            }
            DialogResult dg = MessageBox.Show("Вы уверены что хотите удалить участника '" +
                tbPartNumber.Text + " - " + tbPartSurname.Text + ' ' + tbPartName.Text + "'?",
                "Удаление участника", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dg == DialogResult.No)
                return;
            SqlCommand cmd = new SqlCommand("DELETE FROM Participants WHERE iid = @iid", cn);
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters[0].Value = iid;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            //SetEditModeParts(false);
            SetDataGrid();
        }

        private void xlImport_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                List<int> inserted = new List<int>();

                bool getNumbers = (MessageBox.Show("Импортировать индивидуальные номера из Excel?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                OpenFileDialog dg = new OpenFileDialog();
                dg.CheckFileExists = true;
                dg.CheckPathExists = true;
                DialogResult dgRes = dg.ShowDialog();
                if (dgRes == DialogResult.Cancel)
                    return;
                //MessageBox.Show(dg.FileName);

                Excel.Application xlApp = new Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("Excel не может быть запущен.");
                    return;
                }
                xlApp.Visible = true;
                Excel.Workbook wb;
                try
                {
                    wb = xlApp.Workbooks.Open(dg.FileName, Type.Missing, false, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (wb == null)
                {
                    MessageBox.Show("Ошибка открытия файла.");
                    return;
                }
                Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
                if (ws == null)
                {
                    MessageBox.Show("Лист Excel не может быть открыт.");
                    return;
                }


                ((Excel._Worksheet)ws).Activate();
                Excel.Range tmpRange;

                #region DetectionRankingColumns
                char[] styles = new char[7];
                for (int iii = 0; iii < styles.Length; iii++)
                    styles[iii] = (char)0;
                for (char cc = 'A'; cc <= 'Z'; cc++)
                {
                    tmpRange = ws.get_Range(cc + "5", cc + "5");
#if FULL
                    int nI = tmpRange.Text.ToString().ToLower().IndexOf("тр");
                    if (nI > -1)
                    {
                        if (tmpRange.Text.ToString().ToLower().IndexOf("ре") > -1)
                            styles[1] = cc;
                        else
                            if (nI == 0)
                                styles[0] = cc;
                    }
                    else
                    {
                        nI = tmpRange.Text.ToString().ToLower().IndexOf("ск");
                        if (nI > -1)
                        {
                            if (tmpRange.Text.ToString().ToLower().IndexOf("ре") > -1)
                                styles[3] = cc;
                            else
                                if (nI == 0)
                                    styles[2] = cc;
                        }
                        else
                        {
                            nI = tmpRange.Text.ToString().ToLower().IndexOf("бо");
                            if (nI > -1)
                            {
                                if (tmpRange.Text.ToString().ToLower().IndexOf("ре") > -1)
                                    styles[5] = cc;
                                else
                                    if (nI == 0)
                                        styles[4] = cc;
                            }
                            else
                                if (tmpRange.Text.ToString().ToLower().IndexOf("поз") == 0)
                                    styles[6] = cc;
                        }
                    }
#else
                    if (tmpRange.Text.ToString().ToLower().IndexOf("рейт") > -1)
                        styles[3] = cc;
#endif
                }
                #endregion
                int i = 6, stp;
                char nameCol = 'C', ageCol = 'D', razrCol = 'E', teamCol = 'F', sexCol = 'G', numCol = 'B', vkCol = (char)0;
                bool bName = false, bAge = false, bRazr = false, bTeam = false, bSex = false, bNum = false, bVk = false;
                for (char cCur = 'A'; (cCur <= 'Z') && !(bName && bAge && bRazr && bTeam && bSex && bVk); cCur++)
                {
                    tmpRange = ws.get_Range(cCur + "5", cCur + "5");
                    string stm = tmpRange.Text.ToString().ToLower();
                    if (!bName && (stm.IndexOf("фамил") > -1) || stm.IndexOf("ф.и.о") > -1)
                    {
                        bName = true;
                        nameCol = cCur;
                    }
                    else if (!bAge && (stm.IndexOf("г.р.") > -1 || stm.IndexOf("год") > -1))
                    {
                        bAge = true;
                        ageCol = cCur;
                    }
                    else if (!bRazr && stm.IndexOf("разр") > -1)
                    {
                        bRazr = true;
                        razrCol = cCur;
                    }
                    else if (!bTeam && (stm.IndexOf("команд") > -1 || stm.IndexOf("город") > -1 || stm.IndexOf("регион") > -1 || stm.IndexOf("субъект") > -1 || stm.IndexOf("страна") > -1))
                    {
                        bTeam = true;
                        teamCol = cCur;
                    }
                    else if (!bSex && stm.IndexOf("пол") > -1)
                    {
                        bSex = true;
                        sexCol = cCur;
                    }
                    else if (!bNum && ((stm.IndexOf("инд") > -1 && (stm.IndexOf("№") > -1 || stm.IndexOf("n") > -1)) || stm.IndexOf("номер") > -1))
                    {
                        bNum = true;
                        numCol = cCur;
                    }
                    else if (!bVk && (stm.IndexOf("вне кон") > -1 || stm.IndexOf("в/к") > -1) || stm.IndexOf("в\\к") > -1)
                    {
                        bVk = true;
                        vkCol = cCur;
                    }
                }
                string sMsg = "";
                if (!bName)
                    sMsg += "Столбец \"Фамилия, Имя\" не найден. Будет использован столбец " + nameCol + "\r\n";
                if (!bAge)
                    sMsg += "Столбец \"Г.р.\" не найден. Будет использован столбец " + ageCol + "\r\n";
                if (!bRazr)
                    sMsg += "Столбец \"Разряд\" не найден. Будет использован столбец " + razrCol + "\r\n";
                if (!bTeam)
                    sMsg += "Столбец \"Команда\" не найден. Будет использован столбец " + teamCol + "\r\n";
                if (!bSex)
                    sMsg += "Столбец \"Пол\" не найден. Будет использован столбец " + sexCol + "\r\n";
                if (!bNum)
                    sMsg += "Столбец \"Инд.№\" не найден. Будет использован столбец " + numCol + "\r\n";
                if (!bVk)
                    sMsg += "Столбец \"в/к\" не найден. Участникам. выступающим вне конкурса признак надо будет проставить вручную.\r\n";
                if (sMsg.Length > 0)
                    if (MessageBox.Show(this, sMsg + "\r\nВы согласны?", "Несоответствие столбцов",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        MessageBox.Show(this, "Импорт участников отменён пользователем.");
                        return;
                    }
                string strName, strSurname, strQf, strTeam, strAge, sErrors = "";
                int nAge, nTeamID, nGroupID, iid;
                bool gFemale;
                do
                {
                    tmpRange = ws.get_Range(nameCol + i.ToString(), nameCol + i.ToString());
                    string nameSurnameText = tmpRange.Text.ToString().Trim();
                    while (nameSurnameText.IndexOf("  ") > -1)
                        nameSurnameText = nameSurnameText.Replace("  ", " ");
                    stp = nameSurnameText.IndexOf(' ');
                    if (stp > 0)
                    {
                        strSurname = nameSurnameText.Substring(0, stp);
                        strName = nameSurnameText.Substring(stp + 1, nameSurnameText.Length - stp - 1);
                    }
                    else
                    {
                        strSurname = nameSurnameText;
                        strName = "";
                    }
                    if (strName == "" && strSurname == "")
                    {
                        i++;
                        continue;
                    }

                    tmpRange = ws.get_Range(ageCol + i.ToString(), ageCol + i.ToString());
                    strAge = tmpRange.Text.ToString().Trim();
                    try
                    {
                        nAge = Convert.ToInt32(strAge);
                        if (nAge < 100 && nAge > 10)
                            nAge += 1900;
                        if (nAge >= 0 && nAge <= 10)
                            nAge += 2000;
                        strAge = nAge.ToString();
                    }
                    catch { strAge = ""; nAge = 0; }
                    tmpRange.Cells[1, 1] = strAge.ToString();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.Transaction = cn.BeginTransaction();
                    bool transactionSuccess = false;
                    strTeam = "<Команда не определена>";
                    try
                    {
                        tmpRange = ws.get_Range(sexCol + i.ToString(), sexCol + i.ToString());
                        gFemale = (tmpRange.Text.ToString().ToUpper() == "Ж");

                        tmpRange = ws.get_Range(razrCol + i.ToString(), razrCol + i.ToString());
                        strQf = tmpRange.Text.ToString();

                        tmpRange = ws.get_Range(teamCol + i.ToString(), teamCol + i.ToString());
                        strTeam = tmpRange.Text.ToString().Trim();
                        while (strTeam.IndexOf("  ") > -1)
                            strTeam = strTeam.Replace("  ", " ");
                        cmd.CommandText = "SELECT iid FROM Teams WHERE name = @name";
                        cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        cmd.Parameters[0].Value = strTeam;
                        bool vk = false;
                        if (vkCol > (char)0)
                            try
                            {
                                tmpRange = ws.get_Range(vkCol + i.ToString(), vkCol + i.ToString());
                                string sttmp = tmpRange.Text.ToString().ToLower();
                                vk = (sttmp.IndexOf('+') > -1 || sttmp.IndexOf("в/к") > -1);
                            }
                            catch { }

                        try { nTeamID = Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch { nTeamID = 0; }
                        if (nTeamID == 0)
                        {
                            cmd.CommandText = "SELECT (MAX(iid)+1) FROM Teams";
                            try { nTeamID = Convert.ToInt32(cmd.ExecuteScalar()); }
                            catch { nTeamID = 1; }
                            cmd.CommandText = "INSERT INTO Teams (name, iid) VALUES (@name, @iid)";
                            cmd.Parameters.Add("@iid", SqlDbType.Int);
                            cmd.Parameters[1].Value = nTeamID;
                            cmd.ExecuteNonQuery();
                        }

                        cmd.CommandText = "SELECT iid FROM Groups WHERE (oldYear<=@age)" +
                            "AND (youngYear>=@age) AND (genderFemale = @gF)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@age", SqlDbType.Int);
                        cmd.Parameters[0].Value = nAge;
                        cmd.Parameters.Add("@gF", SqlDbType.Bit);
                        cmd.Parameters[1].Value = gFemale;
                        try
                        {
                            nGroupID = Convert.ToInt32(cmd.ExecuteScalar());
                            if (nGroupID == 0)
                                throw new Exception();
                        }
                        catch
                        {
                            sErrors += "Участник " + strSurname + ' ' + strName + " из команды " + strTeam + " не входит ни в одну возрастную группу.\r\n";
                            i++;
                            continue;
                        }

                        //int[] strt = new int[7];
                        object[] strt = new object[7];
                        for (int iii = 0; iii < strt.Length; iii++)
                            if (iii % 2 == 0)
                                strt[iii] = false;
                            else
                                strt[iii] = DBNull.Value;

                        for (int iii = 0; iii < styles.Length; iii++)
                        {
                            if (styles[iii] == (char)0)
                                continue;
                            string stt = ws.get_Range(styles[iii] + i.ToString(), styles[iii] + i.ToString()).Text.ToString().Trim().ToLower();
                            if (iii % 2 == 0)
                            {
                                if (String.IsNullOrEmpty(stt))
                                    strt[iii] = (short)0;
                                else if (stt.Equals("+"))
                                    strt[iii] = (short)1;
                                else if (stt.Equals("л") || stt.Equals("лично"))
                                    strt[iii] = (short)2;
                                else
                                    strt[iii] = (short)0;
                                /*if (stt != "" && stt.ToUpper().IndexOf('Н') < 0 && stt.IndexOf('-') < 0)
                                    strt[iii] = true;
                                else
                                    strt[iii] = false;*/
                                //strt[iii] = 1;
                            }
                            else
                            {
                                string sNewRank;
                                strt[iii] = getRanking(stt, out sNewRank);
                                ws.get_Range(styles[iii] + i.ToString(), styles[iii] + i.ToString()).Cells[1, 1] = sNewRank;
                                //if (stt != "")
                                //    try { strt[iii] = int.Parse(stt); }
                                //    catch { }
                            }

                        }

                        cmd.CommandText = "SELECT ISNULL (iid,0) iid FROM Participants WHERE (" +
                            "name = '" + strName + "') AND (surname = '" + strSurname + "') AND (age = " + strAge +
                            ") AND (team_id = " + nTeamID.ToString() + ") AND (group_id = " + nGroupID.ToString() + ')';
                        try { iid = Convert.ToInt32(cmd.ExecuteScalar()); }
                        catch { iid = 0; }
                        if (iid > 0)
                        {
                            //MessageBox.Show("Участник "+strSurname+' '+strName+" уже есть в списке.");
                            cmd.CommandText = "UPDATE Participants SET qf = '" + strQf +
                                "', lead = @l, speed = @s, boulder = @b, rankingLead = @rl, rankingSpeed = @rs, " +
                                "rankingBoulder = @rb, lateAppl = @la, vk = @vk WHERE iid = " + iid.ToString();
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add("@l", SqlDbType.SmallInt);

                            cmd.Parameters.Add("@s", SqlDbType.SmallInt);

                            cmd.Parameters.Add("@b", SqlDbType.SmallInt);

#if FULL
                            cmd.Parameters[0].Value = strt[0];
                            cmd.Parameters[1].Value = strt[2];
                            cmd.Parameters[2].Value = strt[4];
#else
                        cmd.Parameters[0].Value = false;
                        cmd.Parameters[2].Value = false;
                        cmd.Parameters[1].Value = true;
#endif

                            cmd.Parameters.Add("@rl", SqlDbType.Int);
                            cmd.Parameters[3].Value = strt[1];
                            //if (strt[1] == 0)
                            //    cmd.Parameters[3].Value = DBNull.Value;
                            //else
                            //    cmd.Parameters[3].Value = strt[1];

                            cmd.Parameters.Add("@rs", SqlDbType.Int);
                            cmd.Parameters[4].Value = strt[3];
                            //if (strt[3] == 0)
                            //    cmd.Parameters[4].Value = DBNull.Value;
                            //else
                            //    cmd.Parameters[4].Value = strt[3];

                            cmd.Parameters.Add("@rb", SqlDbType.Int);
                            cmd.Parameters[5].Value = strt[5];
                            //if (strt[5] == 0)
                            //    cmd.Parameters[5].Value = DBNull.Value;
                            //else
                            //    cmd.Parameters[5].Value = strt[5];

                            cmd.Parameters.Add("@la", SqlDbType.Bit);
                            cmd.Parameters[6].Value = strt[6];

                            cmd.Parameters.Add("@vk", SqlDbType.Bit);
                            cmd.Parameters[7].Value = vk;
                            try
                            {
                                cmd.ExecuteNonQuery();
                                inserted.Add(iid);
                                transactionSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                sErrors += "Ошибка обновления данных участника " + strSurname + " " + strName +
                                  " из команды " + strTeam + ": " + ex.Message + "\r\n";
                                transactionSuccess = false;
                            }
                            tmpRange = ws.get_Range(numCol + i.ToString(), numCol + i.ToString());
                            tmpRange.Cells[1, 1] = iid.ToString();
                            i++;

                            tmpRange = ws.get_Range(nameCol + i.ToString(), nameCol + i.ToString());
                            continue;
                        }
                        tmpRange = ws.get_Range(numCol + i.ToString(), numCol + i.ToString());
                        int oldN = -1;
                        try
                        {
                            if (!getNumbers)
                                throw new Exception();
                            iid = Convert.ToInt32(tmpRange.Text.ToString());
                            if (checkNum(iid, cmd.Transaction))
                            {
                                oldN = iid;
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            iid = GetNextNumber(nGroupID, cmd.Transaction);
                        }
                        if (oldN > 0)
                            sErrors += "Участнику " + strSurname + " " + strName + " из команды " + strTeam +
                                " вместо указанного номера " + oldN.ToString() +
                                        " будет присвоен новый номер " + iid.ToString() + "\r\n";

                        tmpRange.Cells[1, 1] = iid.ToString();

                        //tmpRange = ws.get_Range('I' + i.ToString(), 'I' + i.ToString());

                        //int ranking;
                        //try { ranking = Convert.ToInt32(tmpRange.Text); }
                        //catch { ranking = 0; }



                        //tmpRange = ws.get_Range('J' + i.ToString(), 'J' + i.ToString());
                        //bool lateAppl = (tmpRange.Text.ToString() != "");

                        cmd.CommandText = "INSERT INTO Participants(name, surname, age, genderFemale, " +
                            "qf, team_id, group_id, lead, speed, boulder, rankingLead, rankingSpeed, rankingBoulder, lateAppl, vk, name_ord, iid) " +
                            "VALUES (@name, @surname, @age, @gF, @qf, @t_id, @g_id, @l, @s, @b, @rl, @rs, @rb, @la, @vk, '', @iid)";
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@name", SqlDbType.VarChar, 50);
                        cmd.Parameters[0].Value = strName;

                        cmd.Parameters.Add("@surname", SqlDbType.VarChar, 50);
                        cmd.Parameters[1].Value = strSurname;

                        cmd.Parameters.Add("@age", SqlDbType.Int);
                        cmd.Parameters[2].Value = nAge;

                        cmd.Parameters.Add("@gF", SqlDbType.Bit);
                        cmd.Parameters[3].Value = gFemale;

                        cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50);
                        cmd.Parameters[4].Value = strQf.Trim();

                        cmd.Parameters.Add("@t_id", SqlDbType.Int);
                        cmd.Parameters[5].Value = nTeamID;

                        cmd.Parameters.Add("@g_id", SqlDbType.Int);
                        cmd.Parameters[6].Value = nGroupID;
                        cmd.Parameters.Add("@l", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@s", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@b", SqlDbType.SmallInt);
#if FULL
                        cmd.Parameters[7].Value = strt[0];

                        cmd.Parameters[8].Value = strt[2];

                        cmd.Parameters[9].Value = strt[4];
#else
                    cmd.Parameters[7].Value = false;
                    cmd.Parameters[9].Value = false;
                    cmd.Parameters[8].Value = true;
#endif
                        cmd.Parameters.Add("@rl", SqlDbType.Int);
                        cmd.Parameters[10].Value = strt[1];
                        //if (strt[1] == 0)
                        //    cmd.Parameters[10].Value = DBNull.Value;
                        //else
                        //    cmd.Parameters[10].Value = strt[1];

                        cmd.Parameters.Add("@rs", SqlDbType.Int);
                        cmd.Parameters[11].Value = strt[3];
                        //if (strt[3] == 0)
                        //    cmd.Parameters[11].Value = DBNull.Value;
                        //else
                        //    cmd.Parameters[11].Value = strt[3];

                        cmd.Parameters.Add("@rb", SqlDbType.Int);
                        cmd.Parameters[12].Value = strt[5];
                        //if (strt[5] == 0)
                        //    cmd.Parameters[12].Value = DBNull.Value;
                        //else
                        //    cmd.Parameters[12].Value = strt[5];

                        cmd.Parameters.Add("@la", SqlDbType.Bit);
                        cmd.Parameters[13].Value = strt[6];

                        cmd.Parameters.Add("@vk", SqlDbType.Bit);
                        cmd.Parameters[14].Value = vk;

                        cmd.Parameters.Add("@iid", SqlDbType.Int);
                        cmd.Parameters[15].Value = iid;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            inserted.Add(iid);
                            transactionSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            sErrors += "Ошибка добавления участника " + strSurname + ' ' + strName + ": " + ex.Message + "\r\n";
                            transactionSuccess = false;
                        }
                    }
                    finally
                    {
                        if (transactionSuccess)
                            try { cmd.Transaction.Commit(); }
                            catch (Exception ex)
                            { sErrors += "Ошибка добавления участника " + strSurname + ' ' + strName + ": " + ex.Message + "\r\n"; }
                        else
                            try { cmd.Transaction.Rollback(); }
                            catch { }
                    }
                    i++;
                    tmpRange = ws.get_Range(nameCol + i.ToString(), nameCol + i.ToString());
                } while (tmpRange.Text.ToString() != "");
                if (sErrors.Length > 0)
                    MessageBox.Show(this, "При импорте списка участников произошли следующие ошибки:\r\n" + sErrors);

                if (MessageBox.Show(this, "Импорт завершён. Добавлено " + inserted.Count.ToString() +
                    " участников. Удалить из БД участников, не входящих в список добавленных?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (inserted.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = cn;
                        cmd.Transaction = cn.BeginTransaction();
                        try
                        {
                            cmd.CommandText = "DELETE FROM Participants WHERE iid NOT IN(";
                            foreach (int iiii in inserted)
                                cmd.CommandText += iiii.ToString() + ",";
                            cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);
                            cmd.CommandText += ")";
                            int ijk = cmd.ExecuteNonQuery();
                            if (ijk > 0)
                            {
                                if (MessageBox.Show(ijk.ToString() + " участников будет удалено. Продолжить?", "Удаление лишних участников", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    cmd.Transaction.Commit();
                                else
                                    cmd.Transaction.Rollback();
                            }
                            else
                                cmd.Transaction.Rollback();
                        }
                        catch
                        {
                            try { cmd.Transaction.Rollback(); }
                            catch { }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    RefreshCbTeams();
                    cbPartGroups.SelectedIndex = 0;
                    cbPartTeams.SelectedIndex = 0;
                    SetDataGrid();
                }
                catch { }
            }
            //			string t = tmpRange.Text.ToString();
            //			MessageBox.Show(t);
        }

        public static string GENDER = "Пол";
        public static string SCR_RANK_ALL = "Рейтинг (место)";
        public static string SCR_PARTICIPATIONS = "Участие в видах";
        public static string SCR_LATE_APPL = "Поздн. Заявка";
        public static string OUT_OF_COMP = "в/к";
        public static string SCR_LIST_TITLE = "Список участников";

        private void xlExport_Click(object sender, EventArgs e)
        {

            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;


            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, true, false, cn))
                return;
            try
            {
                DataTable dt = SetDataGrid();

                //da.Fill(dt);

                int i = 0;

                SqlCommand cmd = new SqlCommand("SELECT g.genderFemale, p.lateAppl, p.vk " +
                                                "  FROM Participants p(NOLOCK) " +
                                                "  JOIN Groups g(NOLOCK) ON g.iid = p.group_id " +
                                                " WHERE p.iid = @iid", cn);
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                int iLAppl;
#if FULL
                iLAppl = 15;
#else
            iLAppl = 10;
#endif
                foreach (DataRow rd in dt.Rows)
                {
                    i++;
                    ws.Cells[i + 5, 1] = i.ToString();
                    ws.Cells[i + 5, 2] = rd[0].ToString();
                    ws.Cells[i + 5, 3] = rd[1].ToString() + ' ' + rd[2].ToString();
                    ws.Cells[i + 5, 4] = rd[4].ToString();
                    ws.Cells[i + 5, 5] = rd[5].ToString();
                    ws.Cells[i + 5, 6] = rd[3].ToString();

                    cmd.Parameters[0].Value = Convert.ToInt32(rd[0]);
                    SqlDataReader drr = cmd.ExecuteReader();

                    try
                    {
                        if (drr.Read())
                        {
                            if (Convert.ToBoolean(drr[0]))
                                ws.Cells[i + 5, 7] = SCR_FEMALE;
                            if (Convert.ToBoolean(drr[1]))
                                ws.Cells[i + 5, iLAppl] = "+";
                            if (Convert.ToBoolean(drr[2]))
                                ws.Cells[i + 5, iLAppl + 1] = "+";
                        }
                    }
                    finally { drr.Close(); }
                    //if (Convert.ToBoolean(cmd.ExecuteScalar()))
                    //    ws.Cells[i + 5, 7] = "ж";
                    //else
                    //    ws.Cells[i + 5, 7] = "";
                    ws.Cells[i + 5, 8] = rd[6].ToString();
#if FULL
                    //if (Convert.ToBoolean(rd["Тр."]))
                    ws.Cells[i + 5, 9] = GetRowValue(rd, LEAD_S);
                    //if (Convert.ToBoolean(rd["Ск."]))
                    ws.Cells[i + 5, 10] = GetRowValue(rd, SPEED_S);
                    //if (Convert.ToBoolean(rd["Боулд."]))
                    ws.Cells[i + 5, 11] = GetRowValue(rd, BOULDER_S);

                    ws.Cells[i + 5, 12] = rd[SCR_RANK_LEAD_COL].ToString();
                    ws.Cells[i + 5, 13] = rd[SCR_RANK_SPEED_COL].ToString();
                    ws.Cells[i + 5, 14] = rd[SCR_RANK_BOULDER_COL].ToString();
#else
                ws.Cells[i + 5, 9] = rd[rankSpeedCol].ToString();
#endif
                }
                ws.Cells[5, 1] = "№";
                ws.Cells[5, 2] = BIB_F;
                ws.Cells[5, 3] = CLIMBER_NAME;
                ws.Cells[5, 4] = AGE_ST;
                ws.Cells[5, 5] = QF_ST;
                ws.Cells[5, 6] = TEAM_ST;
                ws.Cells[5, 7] = GENDER;
                ws.Cells[5, 8] = GROUP;
                Excel.Range dta;
#if FULL
                dta = ws.get_Range("I4", "K4");
                dta.Merge(Type.Missing);
                dta.Cells[1, 1] = SCR_PARTICIPATIONS;
                ws.Cells[5, 9] = LEAD;
                ws.Cells[5, 10] = SPEED;
                ws.Cells[5, 11] = BOULDER;

                dta = ws.get_Range("L4", "N4");
                dta.Merge(Type.Missing);
                dta.Cells[1, 1] = SCR_RANK_ALL;
                ws.Cells[5, 12] = SCR_RANK_LEAD_COL;
                ws.Cells[5, 13] = SCR_RANK_SPEED_COL;
                ws.Cells[5, 14] = SCR_RANK_BOULDER_COL;
#else
            ws.Cells[5, iLAppl - 1] = "Рейтинг (место)";
#endif
                ws.Cells[5, iLAppl] = SCR_LATE_APPL;
                ws.Cells[5, iLAppl + 1] = OUT_OF_COMP;
#if FULL
                dta = ws.get_Range("A5", 'H' + (i + 5).ToString());
                dta.Style = "MyStyle";
                dta.Columns.AutoFit();

                dta = ws.get_Range("I4", 'N' + (i + 5).ToString());
                dta.Style = "MyStyle";
                dta.Columns.AutoFit();

                dta = ws.get_Range("O5", 'P' + (i + 5).ToString());
                dta.Style = "MyStyle";
                dta.Columns.AutoFit();
#else
            dta = ws.get_Range(ws.Cells[5, 1], ws.Cells[i + 5, iLAppl + 1]);
            dta.Style = "MyStyle";
            dta.Columns.AutoFit();
#endif
                dta = ws.get_Range("C5", 'C' + (i + 5).ToString());
                dta.Style = "MyStyle2";

                try { ws.Name = SCR_LIST_TITLE; }
                catch { }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private static string GetRowValue(DataRow rd, string stle)
        {
            string s;
            switch (Convert.ToInt16(rd[stle]))
            {
                case 1: s = "+";
                    break;
                case 2: s = "Лично";
                    break;
                default: s = String.Empty;
                    break;
            }
            return s;
        }

        private void printAllCards_Click(object sender, EventArgs e)
        {
            SelectCardType sct = new SelectCardType();
            sct.ShowDialog();
            SelectCardType.CardPrint data = sct.GetWhatToPrint;
            if (data.CANCEL)
                return;
            #region ExcelLaunching
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;

            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, false, cn))
                return;
            try
            {
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;


            #endregion
                Excel.Range dt = ws.get_Range("A1", "A1");
                dt.ColumnWidth = 7.0;
                dt = ws.get_Range("B1", "B1");
                dt.ColumnWidth = 10.29;
                dt = ws.get_Range("C1", "C1");
                dt.ColumnWidth = 12.14;
                dt = ws.get_Range("D1", "D1");
                dt.ColumnWidth = 12.29;
                dt = ws.get_Range("E1", "E1");
                dt.ColumnWidth = 12.0;
                dt = ws.get_Range("F1", "F1");
                dt.ColumnWidth = 11.0;
                dt = ws.get_Range("G1", "G1");
                dt.ColumnWidth = 0.75;
                dt = ws.get_Range("H1", "H1");
                dt.ColumnWidth = 7.0;
                dt = ws.get_Range("I1", "I1");
                dt.ColumnWidth = 10.29;
                dt = ws.get_Range("J1", "J1");
                dt.ColumnWidth = 12.14;
                dt = ws.get_Range("K1", "K1");
                dt.ColumnWidth = 12.29;
                dt = ws.get_Range("L1", "L1");
                dt.ColumnWidth = 12.0;
                dt = ws.get_Range("M1", "M1");
                dt.ColumnWidth = 11.0;

                int i, k = 0, Length = 1;
                //DataRow rd = dsClimbing.Participants.Rows[cm.Position];
                DataTable dT = SetDataGrid();
                foreach (DataRow rd in dT.Rows)
                {
                    string num = rd[0].ToString();

                    if (!CheckClimberCard(num, data))
                        continue;
                    bool fr = ((k % 2) == 0);
                    if (fr)
                        i = k / 2;
                    else
                        i = (k - 1) / 2;


                    if (num == "")
                        num = "0";

                    ShowOneCard(ref data, ws, ref dt, k, ref Length, ref num);

                    k++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private bool CheckClimberCard(string num, SelectCardType.CardPrint data)
        {
            if (!data.OnlyParticipants)
                return true;
            foreach (var v in data.RoundsToCheck)
                if (CheckClimber(num, v))
                    return true;
            return false;
        }



        private bool CheckClimber(string num, SelectCardType.RoundToCheck round)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText =
                @"SELECT COUNT(*) cnt
                    FROM lists L(NOLOCK)
                    JOIN " + round.Table + @" R(NOLOCK) ON R.list_id = L.iid
                   WHERE L.round = @rn
                     AND L.style = @st
                     AND R.climber_id = " + num;
            cmd.Parameters.Add(new SqlParameter("@rn", SqlDbType.VarChar));
            cmd.Parameters[0].Value = round.Round;
            cmd.Parameters.Add(new SqlParameter("@st", SqlDbType.VarChar));
            cmd.Parameters[1].Value = round.Style;
            object res = cmd.ExecuteScalar();
            if (res is int)
                return (((int)res) > 0);
            if (res == null || res == DBNull.Value)
                return false;
            try { return (Convert.ToInt32(res) > 0); }
            catch { return false; }
        }

        private void printCard_Click(object sender, EventArgs e)
        {
            int iid;
            try { iid = Convert.ToInt32(tbPartNumber.Text); }
            catch { iid = 0; }
            if (iid != 0)
            {
                SelectCardType sct = new SelectCardType();
                sct.ShowDialog();
                SelectCardType.CardPrint data = sct.GetWhatToPrint;
                if (data.CANCEL)
                    return;
                #region ExcelLaunching
                Excel.Worksheet ws;
                Excel.Workbook wb;
                Excel.Application xlApp;
                if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, false, cn))
                    return;
                try
                {

                    Excel.Range dt = ws.get_Range("A1", "A1");
                    dt.ColumnWidth = 6.57;
                    dt = ws.get_Range("B1", "B1");
                    dt.ColumnWidth = 3.29;
                    dt = ws.get_Range("C1", "C1");
                    dt.ColumnWidth = 11.57;
                    for (char c = 'D'; c != 'P'; c++)
                    {
                        dt = ws.get_Range(c + "1", c + "1");
                        dt.ColumnWidth = 3.0;
                    }
                    dt = ws.get_Range("P1", "P1");
                    dt.ColumnWidth = 9.0;
                #endregion

                    dt = ws.get_Range("A1", "A1");
                    dt.ColumnWidth = 7.0;
                    dt = ws.get_Range("B1", "B1");
                    dt.ColumnWidth = 10.29;
                    dt = ws.get_Range("C1", "C1");
                    dt.ColumnWidth = 12.14;
                    dt = ws.get_Range("D1", "D1");
                    dt.ColumnWidth = 12.29;
                    dt = ws.get_Range("E1", "E1");
                    dt.ColumnWidth = 12.0;
                    dt = ws.get_Range("F1", "F1");
                    dt.ColumnWidth = 11.0;
                    dt = ws.get_Range("G1", "G1");
                    dt.ColumnWidth = 3.43;
                    dt = ws.get_Range("H1", "H1");
                    dt.ColumnWidth = 7.0;
                    dt = ws.get_Range("I1", "I1");
                    dt.ColumnWidth = 10.29;
                    dt = ws.get_Range("J1", "J1");
                    dt.ColumnWidth = 12.14;
                    dt = ws.get_Range("K1", "K1");
                    dt.ColumnWidth = 12.29;
                    dt = ws.get_Range("L1", "L1");
                    dt.ColumnWidth = 12.0;
                    dt = ws.get_Range("M1", "M1");
                    dt.ColumnWidth = 11.0;


                    int k = 0;
                    int Length = 1;
                    string num = iid.ToString();
                    ShowOneCard(ref data, ws, ref dt, k, ref Length, ref num);
                }
                catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
                finally { StaticClass.SetExcelVisible(xlApp); }
            }
        }

        private void ShowOneCard(ref SelectCardType.CardPrint data, Excel.Worksheet ws, ref Excel.Range dt, int k, ref int Length, ref string num)
        {
            bool fr = ((k % 2) == 0);
            int i;
            if (fr)
                i = k / 2;
            else
                i = (k - 1) / 2;

            if (num == "")
                num = "0";

            if (fr)
                dt = ws.get_Range("A" + (1 + i * Length).ToString(), "F" + (1 + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (1 + i * Length).ToString(), "M" + (1 + i * Length).ToString());
            dt.Merge(Type.Missing);
            dt.Style = "topc";
            dt.Cells[1, 1] = "КАРТОЧКА УЧАСТНИКА";
            dt.RowHeight = 15.0;

            if (fr)
                dt = ws.get_Range("C" + (2 + i * Length).ToString(), "F" + (2 + i * Length).ToString());
            else
                dt = ws.get_Range("J" + (2 + i * Length).ToString(), "M" + (2 + i * Length).ToString());
            dt.Merge(Type.Missing);
            dt.RowHeight = 22.5;
            dt.Style = "font11c";
            SqlCommand cmd = new SqlCommand("SELECT surname+' '+name FROM Participants WHERE iid=" + num, cn);
            try { dt.Cells[1, 1] = "Фамилия, Имя: " + cmd.ExecuteScalar().ToString(); }
            catch { dt.Cells[1, 1] = "Фамилия, Имя: "; }

            if (fr)
                dt = ws.get_Range("A" + (2 + i * Length).ToString(), "B" + (2 + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (2 + i * Length).ToString(), "I" + (2 + i * Length).ToString());
            dt.Merge(Type.Missing);
            dt.RowHeight = 22.5;
            dt.Style = "font11c";
            dt.Cells[1, 1] = "Номер: " + num;

            if (fr)
                dt = ws.get_Range("A" + (3 + i * Length).ToString(), "B" + (3 + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (3 + i * Length).ToString(), "I" + (3 + i * Length).ToString());
            dt.Style = "font10c";
            dt.Merge(Type.Missing);
            dt.RowHeight = 21.0;
            cmd.CommandText = "SELECT age FROM Participants WHERE iid=" + num;
            try { dt.Cells[1, 1] = "Г.р.: " + cmd.ExecuteScalar().ToString(); }
            catch { dt.Cells[1, 1] = "Г.р.: "; }

            string strTmp = tbPartTeam.Text;
            if (fr)
                dt = ws.get_Range("C" + (3 + i * Length).ToString(), "D" + (3 + i * Length).ToString());
            else
                dt = ws.get_Range("J" + (3 + i * Length).ToString(), "K" + (3 + i * Length).ToString());
            dt.Style = "font10c";
            dt.Merge(Type.Missing);
            cmd.CommandText = "SELECT t.name FROM Participants p INNER JOIN Teams t ON p.team_id=t.iid " +
                "WHERE p.iid=" + num;
            try { dt.Cells[1, 1] = "Команда: " + cmd.ExecuteScalar().ToString(); }
            catch { dt.Cells[1, 1] = "Команда: "; }

            if (fr)
                dt = ws.get_Range("E" + (3 + i * Length).ToString(), "F" + (3 + i * Length).ToString());
            else
                dt = ws.get_Range("L" + (3 + i * Length).ToString(), "M" + (3 + i * Length).ToString());
            dt.Style = "font10c";
            dt.Merge(Type.Missing);
            cmd.CommandText = "SELECT qf FROM Participants WHERE iid=" + num;
            try { dt.Cells[1, 1] = "Разряд: " + cmd.ExecuteScalar().ToString(); }
            catch { dt.Cells[1, 1] = "Разряд: "; }

            if (fr)
            {
                dt = ws.get_Range("A" + (4 + i * Length).ToString(), "F" + (4 + i * Length).ToString());
                dt.RowHeight = 2.25;
            }

            int curRow = 5;


            string round;

            if (data.lead != null)
            {
                #region Lead
                if (fr)
                    dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                else
                    dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                dt.Style = "font11_CAc";
                dt.RowHeight = 15.0;
                dt.Cells[1, 1] = "Ст. №";
                dt.Cells[1, 2] = "Трудность";
                dt.Cells[1, 6] = "Место";
                if (fr)
                    dt = ws.get_Range("C" + (curRow + i * Length).ToString(), "E" + (curRow + i * Length).ToString());
                else
                    dt = ws.get_Range("J" + (curRow + i * Length).ToString(), "L" + (curRow + i * Length).ToString());
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = "Результат";

                //if (fr)
                //    dt = ws.get_Range("A" + (6 + i * Length).ToString(), "F" + (9 + i * Length).ToString());
                //else
                //    dt = ws.get_Range("H" + (6 + i * Length).ToString(), "M" + (9 + i * Length).ToString());
                //dt.Style = "font11";
                //dt.RowHeight = 21.0;
                //dt.Cells[1, 2] = "Трасса 1";
                //dt.Cells[2, 2] = "Трасса 2";
                //dt.Cells[3, 2] = "Финал";
                //dt.Cells[4, 2] = "С.финал";
                int stRow = ++curRow;
                if (data.lead.quali)
                {
                    #region Quaterfinal
                    if (data.lead.q1g)
                    {
                        if (fr)
                            dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                        else
                            dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                        dt.Style = "font11c";
                        dt.RowHeight = 21.0;
                        dt.Cells[1, 2] = "1/4 фин.";

                        round = "1/4 финала";

                        PrintRoundLead(dt, num, cmd, round, false, false);
                        curRow++;
                    }
                    if (data.lead.q2g)
                    {
                        if (fr)
                            dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                        else
                            dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                        dt.Style = "font11c";
                        dt.RowHeight = 21.0;
                        dt.Cells[1, 2] = "1/4 фин.";
                        string rTmp = "1/4 финала";

                        round = rTmp + " Трасса 1";
                        if (!PrintRoundLead(dt, num, cmd, round, true, false))
                        {
                            round = rTmp + " Трасса 2";
                            PrintRoundLead(dt, num, cmd, round, true, true);
                        }

                        curRow++;
                    }
                    #endregion
                    #region QFFlash

                    if (data.lead.qFlash)
                        for (int qw = 1; qw <= data.lead.qFlashRN; qw++)
                        {
                            if (fr)
                                dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                            else
                                dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                            dt.Style = "font11c";
                            dt.RowHeight = 21.0;
                            dt.Cells[1, 2] = "Трасса " + qw.ToString();

                            round = "Квалификация " + qw;

                            PrintRoundLead(dt, num, cmd, round, false, false);

                            curRow++;
                            /*
                            if (fr)
                                dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                            else
                                dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                            dt.Style = "font11c";
                            dt.RowHeight = 21.0;
                            dt.Cells[1, 2] = "Трасса 2";

                            round = "Квалификация 2";

                            PrintRoundLead(dt, num, cmd, round, false, false);

                            curRow++;*/
                        }

                    #endregion
                }
                #region Semifinal
                if (data.lead.semi)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "1/2 фин.";

                    round = "1/2 финала";

                    PrintRoundLead(dt, num, cmd, round, false, false);

                    curRow++;
                }
                #endregion
                #region Final
                if (data.lead.final)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "Финал";

                    round = "Финал";

                    PrintRoundLead(dt, num, cmd, round, false, false);

                    curRow++;
                }
                #endregion
                #region SuperFinal
                if (data.lead.super)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "С.финал";

                    round = "Суперфинал";

                    PrintRoundLead(dt, num, cmd, round, false, false);

                    curRow++;
                }
                #endregion


                for (int hhh = stRow; hhh < curRow; hhh++)
                {
                    if (fr)
                        dt = ws.get_Range("C" + (hhh + i * Length).ToString(), "E" + (hhh + i * Length).ToString());
                    else
                        dt = ws.get_Range("J" + (hhh + i * Length).ToString(), "L" + (hhh + i * Length).ToString());
                    dt.Merge(Type.Missing);
                }

                if (fr)
                {
                    dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    dt.RowHeight = 2.25;
                }
                curRow++;
                #endregion
            }


            if (data.speed != null)
            {
                #region Speed
                SelectCardType.Speed dss = data.speed;
                if (fr)
                    dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                else
                    dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                dt.Style = "font11_CAc";
                dt.RowHeight = 15.0;
                dt.Cells[1, 1] = "Ст. №";
                dt.Cells[1, 2] = "Скорость";
                dt.Cells[1, 3] = "I Трасса";
                dt.Cells[1, 4] = "II Трасса";
                dt.Cells[1, 5] = "Сумма";
                dt.Cells[1, 6] = "Место";
                curRow++;

                if (dss.q1)
                {
                    #region quali1
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "Квал.";
                    round = "Квалификация";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;
                    #endregion
                }

                if (dss.q2)
                {
                    #region quali2
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "Квал.2";
                    round = "Квалификация 2";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;
                    #endregion
                }

                if (dss.f16)
                {
                    #region f16
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "1/8 фин.";
                    round = "1/8 финала";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;
                    #endregion
                }

                if (dss.f8)
                {
                    #region f8
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "1/4 фин.";
                    round = "1/4 финала";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;
                    #endregion
                }

                if (dss.f4)
                {
                    #region f4
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "1/2 фин.";
                    round = "1/2 финала";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;

                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "Финал";
                    round = "Финал";
                    PrintSpeedRound(dt, num, cmd, round);
                    curRow++;
                    #endregion
                }

                if (fr)
                {
                    dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    dt.RowHeight = 2.25;
                }
                curRow++;
                #endregion
            }




            if (data.boulder != null)
            {
                #region Boulder
                SelectCardType.Boulder db = data.boulder;
                if (fr)
                    dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                else
                    dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                dt.Style = "font11_CAc";
                dt.RowHeight = 15.0;
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = "Боулдеринг";
                curRow++;
                if (db.quali)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11_CAc";
                    dt.RowHeight = 15.0;
                    dt.Merge(Type.Missing);
                    dt.Cells[1, 1] = round = (db.q2g ? "Квалификация" : "1/4 финала");

                    curRow++;

                    PrintBoulderRound(db.q2g, ws, ref dt, i, fr, num, cmd, ref curRow, round, db.rQuali, Length);
                }

                if (db.semi)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11_CAc";
                    dt.RowHeight = 15.0;
                    dt.Merge(Type.Missing);
                    dt.Cells[1, 1] = round = "1/2 финала";

                    curRow++;

                    PrintBoulderRound(false, ws, ref dt, i, fr, num, cmd, ref curRow, round, db.rSemi, Length);
                }

                if (db.final)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11_CAc";
                    dt.RowHeight = 15.0;
                    dt.Merge(Type.Missing);
                    dt.Cells[1, 1] = round = "Финал";

                    curRow++;

                    PrintBoulderRound(false, ws, ref dt, i, fr, num, cmd, ref curRow, round, db.rFinal, Length);
                }

                if (db.super)
                {
                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11_CAc";
                    dt.RowHeight = 15.0;
                    dt.Cells[1, 1] = "Ст. №";
                    dt.Cells[1, 6] = "Место";
                    if (fr)
                        dt = ws.get_Range("C" + (curRow + i * Length).ToString(), "E" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("J" + (curRow + i * Length).ToString(), "L" + (curRow + i * Length).ToString());
                    dt.Merge(Type.Missing);
                    dt.Cells[1, 1] = "Результат";

                    curRow++;

                    if (fr)
                        dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
                    dt.Style = "font11c";
                    dt.RowHeight = 21.0;
                    dt.Cells[1, 2] = "С.финал";

                    round = "Суперфинал";

                    cmd.CommandText = "SELECT ld.Start,ld.ResText,ld.pos FROM lists al, routeResults ld " +
                        "WHERE (al.iid=ld.list_id) AND (al.Round='" + round + "') AND (ld.climber_id=" +
                        num + ") AND (al.style='Боулдеринг')";
                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool tmp = false;
                    while (rdr.Read())
                    {
                        dt.Cells[1, 1] = rdr[0];
                        dt.Cells[1, 3] = rdr[1];
                        strTmp = rdr[1].ToString();
                        if (strTmp.IndexOf("н/я") > 0 || strTmp == "" || strTmp.IndexOf("дискв.") > 0)
                            tmp = true;
                        if (!tmp)
                            dt.Cells[1, 6] = rdr[2];
                        break;
                    }
                    rdr.Close();


                    if (fr)
                        dt = ws.get_Range("C" + (curRow + i * Length).ToString(), "E" + (curRow + i * Length).ToString());
                    else
                        dt = ws.get_Range("J" + (curRow + i * Length).ToString(), "L" + (curRow + i * Length).ToString());
                    dt.Merge(Type.Missing);

                    curRow++;
                }

                #endregion
            }

            Length = curRow;
        }

        private static bool PrintRoundLead(Excel.Range dt, string num, SqlCommand cmd, string round, bool twoRoutes, bool route2)
        {
            cmd.CommandText = "SELECT ld.Start,ld.ResText,ld.pos FROM lists al, routeResults ld " +
                "WHERE (al.iid=ld.list_id) AND (al.Round='" + round + "') AND (ld.climber_id=" +
                num + ") AND (al.style='Трудность')";
            SqlDataReader rdr = cmd.ExecuteReader();
            bool tmp = false;
            string strTmp = "";
            bool retVal = false;
            while (rdr.Read())
            {
                if (twoRoutes)
                {
                    if (route2)
                        dt.Cells[1, 1] = " 2: " + rdr[0].ToString();
                    else
                        dt.Cells[1, 1] = " 1: " + rdr[0].ToString();
                }
                else
                    dt.Cells[1, 1] = rdr[0];
                dt.Cells[1, 3] = rdr[1];
                strTmp = rdr[1].ToString();
                if (strTmp.IndexOf("н/я") > 0 || strTmp == "" || strTmp.IndexOf("дискв.") > 0)
                    tmp = true;
                if (!tmp)
                    dt.Cells[1, 6] = rdr[2];
                retVal = true;
                break;
            }
            rdr.Close();
            return retVal;
        }

        private const string NO_RESULT = "NO_RESULT";

        private static void PrintBoulderRound(bool twoGr, Excel.Worksheet ws, ref Excel.Range dt, int i, bool fr, string num, SqlCommand cmd, ref int curRow, string round, int rNum, int Length)
        {
            if (fr)
                dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (curRow + i * Length).ToString(), "M" + (curRow + i * Length).ToString());
            dt.Style = "font11_CAc";
            dt.RowHeight = 15.0;
            dt.Cells[1, 1] = "Ст.№";
            dt.Cells[1, 3] = "TOP";
            dt.Cells[1, 4] = "Bonus";
            dt.Cells[1, 5] = "Подп.судьи";
            dt.Cells[1, 6] = "Подп.уч-ка";

            string rTmp = round;

            if (twoGr)
                round += " Группа А";
            else if (round.IndexOf("валиф") > -1)
                round = "1/4 финала";
            bool grB = false;
            bool resSucc;
            int[] resList;
            string psText;
            bool printRes = true, hasRoutes;

            hasRoutes = GetRoundRes(twoGr, dt, num, cmd, round, rNum, grB, out resSucc, out resList, out psText);

            if (twoGr && !resSucc)
            {
                round = rTmp + " Группа Б";
                grB = true;
                hasRoutes = GetRoundRes(twoGr, dt, num, cmd, round, rNum, grB, out resSucc, out resList, out psText);
            }
            curRow++;
            if (fr)
                dt = ws.get_Range("A" + (curRow + i * Length).ToString(),
                    "F" + (curRow + rNum + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (curRow + i * Length).ToString(),
                    "M" + (curRow + rNum + i * Length).ToString());
            dt.Style = "font11_CAc";
            dt.RowHeight = 21.0;
            for (int rn = 1; rn <= rNum; rn++)
            {
                if (fr)
                    dt = ws.get_Range("A" + (curRow + rn - 1 + i * Length).ToString(),
                        "B" + (curRow + rn - 1 + i * Length).ToString());
                else
                    dt = ws.get_Range("H" + (curRow + rn - 1 + i * Length).ToString(),
                        "I" + (curRow + rn - 1 + i * Length).ToString());
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = "Трасса " + rn.ToString();

                if (psText != NO_RESULT)
                {
                    if (fr)
                        dt = ws.get_Range("C" + (curRow + rn - 1 + i * Length).ToString(),
                            "D" + (curRow + rn - 1 + i * Length).ToString());
                    else
                        dt = ws.get_Range("J" + (curRow + rn - 1 + i * Length).ToString(),
                            "K" + (curRow + rn - 1 + i * Length).ToString());
                    if (resList[2 * (rn - 1) + 4] > -1)
                        dt.Cells[1, 1] = resList[2 * (rn - 1) + 4].ToString();
                    if (resList[2 * (rn - 1) + 5] > -1)
                        dt.Cells[1, 2] = resList[2 * (rn - 1) + 5].ToString();
                    if (rn == 1 && resList[2 * (rn - 1) + 4].ToString() == "")
                        printRes = false;
                }
                else if (rn == 1)
                    printRes = false;
            }

            curRow += rNum;

            if (fr)
                dt = ws.get_Range("A" + (curRow + i * Length).ToString(),
                    "B" + (curRow + i * Length).ToString());
            else
                dt = ws.get_Range("H" + (curRow + i * Length).ToString(),
                    "I" + (curRow + i * Length).ToString());
            dt.Merge(Type.Missing);
            dt.Cells[1, 1] = "Результат";

            if (psText != NO_RESULT)
            {
                if (fr)
                    dt = ws.get_Range("C" + (curRow + i * Length).ToString(),
                        "D" + (curRow + i * Length).ToString());
                else
                    dt = ws.get_Range("J" + (curRow + i * Length).ToString(),
                        "K" + (curRow + i * Length).ToString());
                if (printRes && hasRoutes)
                {
                    if (psText == NYA)
                    {
                        dt.Cells[1, 1] = NYA;
                        dt.Cells[1, 2] = "";
                    }
                    else if (psText == DSQ)
                    {
                        dt.Cells[1, 1] = DSQ;
                        dt.Cells[1, 2] = "";
                    }
                    else
                    {
                        if (resList[0] > -1 && resList[1] > -1)
                            dt.Cells[1, 1] = " " + resList[0].ToString() + " / " + resList[1].ToString();
                        if (resList[2] > -1 && resList[3] > -1)
                            dt.Cells[1, 2] = " " + resList[2].ToString() + " / " + resList[3].ToString();
                    }
                }
            }
            curRow++;
            if (fr)
            {
                dt = ws.get_Range("A" + (curRow + i * Length).ToString(), "F" + (curRow + i * Length).ToString());
                dt.RowHeight = 2.25;
            }
            curRow++;
        }

        private static bool GetRoundRes(bool twoGr, Excel.Range dt, string num, SqlCommand cmd, string round, int rNum, bool grB, out bool resSucc, out int[] resList, out string psText)
        {
            bool toReturn = false;
            resSucc = false;
            cmd.CommandText = @"
                SELECT ld.iid, ld.start, ld.posText, ISNULL(tops, -1) tops, ISNULL(topAttempts, -1) topAttempts, 
                       ISNULL(bonuses, -1) bonuses, ISNULL(bonusAttempts, -1) bonusAttempts, ld.nya, ld.disq
                  FROM lists al, boulderResults ld
                 WHERE al.iid = ld.list_id " +
                  "AND al.Round='" + round + "' " +
                  "AND ld.climber_id = " + num + " " +
                  "AND al.style = 'Боулдеринг'";
            resList = new int[2 * rNum + 4];
            for (int i = 0; i < resList.Length; i++)
                resList[i] = -1;
            psText = NO_RESULT;
            long resIid;
            bool nya = false, disq = false;
            try
            {
                SqlDataReader rdb = cmd.ExecuteReader();
                try
                {
                    if (rdb.Read())
                    {
                        if (!twoGr)
                            dt.Cells[1, 2] = rdb["start"];
                        else
                        {
                            if (grB)
                                dt.Cells[1, 2] = "Б: " + rdb["start"].ToString();
                            else
                                dt.Cells[1, 2] = "А: " + rdb["start"].ToString();
                        }

                        psText = rdb["posText"].ToString();
                        for (int rn = 0; rn < 4; rn++)
                            try { resList[rn] = Convert.ToInt32(rdb[3 + rn]); }
                            catch { resList[rn] = -1; }
                        resIid = Convert.ToInt64(rdb["iid"]);
                        nya = Convert.ToBoolean(rdb["nya"]);
                        disq = Convert.ToBoolean(rdb["disq"]);
                        resSucc = true;
                    }
                    else
                    {
                        psText = NO_RESULT;
                        return false;
                    }
                }
                finally { rdb.Close(); }
                if (nya || disq)
                {
                    if (nya)
                        psText = NYA;
                    else
                        psText = DSQ;
                    return true;
                }
                else
                {
                    cmd.CommandText = @"SELECT routeN, ISNULL(topA,-1) topA, ISNULL(bonusA,-1)bonusA
                                      FROM BoulderRoutes(NOLOCK)
                                     WHERE iid_parent = " + resIid.ToString() +
                                       "   AND routeN <= " + rNum.ToString() +
                                    " ORDER BY routeN";
                    rdb = cmd.ExecuteReader();
                    try
                    {
                        while (rdb.Read())
                        {
                            int rn = Convert.ToInt32(rdb["routeN"]);
                            resList[2 + 2 * rn] = Convert.ToInt32(rdb["topA"]);
                            resList[3 + 2 * rn] = Convert.ToInt32(rdb["bonusA"]);
                            toReturn = true;
                        }
                    }
                    finally { rdb.Close(); }
                }
            }
            catch { psText = NO_RESULT; }
            return toReturn;
        }

        private static void PrintSpeedRound(Excel.Range dt, string num, SqlCommand cmd, string round)
        {
            try
            {
                cmd.CommandText = "SELECT ld.start,ld.route1_text,ld.route2_text,ld.resText,ld.posText FROM lists al, speedResults ld " +
                    "WHERE (al.iid=ld.list_id) AND (al.Round='" + round + "') AND (ld.climber_id=" +
                    num + ") AND (al.style='Скорость')";
                SqlDataReader rdr = cmd.ExecuteReader();
                bool tmp = false;
                while (rdr.Read())
                {
                    dt.Cells[1, 1] = rdr[0];
                    dt.Cells[1, 3] = rdr[1].ToString().Replace(",", ".");
                    dt.Cells[1, 4] = rdr[2].ToString().Replace(",", ".");
                    dt.Cells[1, 5] = rdr[3].ToString().Replace(",", ".");
                    string strTmp = rdr[1].ToString();
                    if (strTmp.IndexOf("н/я") > -1 || strTmp == "" || strTmp.IndexOf("дискв.") > 1
                        || strTmp.IndexOf("срыв") > 1)
                        tmp = true;
                    if (!tmp)
                        dt.Cells[1, 6] = rdr[4];
                }
                rdr.Close();
            }
            catch { }
        }

        private string[] GetLogosFromRegistry(bool getFromReg)
        {
            string[] res = new string[2];
            try
            {
                if (getFromReg)
                {
                    res[0] = ClComp.Default.Logo;
                }
                if (res[0] == null || res[0] == "")
                    throw new Exception();

            }
            catch
            {
                OpenFileDialog dg = new OpenFileDialog();
                dg.Title = "Выберите логотип, указанный на номере (300 x 40)";
                DialogResult dgr = dg.ShowDialog();
                if (dgr == DialogResult.Cancel)
                    res[0] = "no_logo";
                else
                    res[0] = dg.FileName;


                ClComp clCs = ClComp.Default;
                clCs.Logo = res[0];
                clCs.Save();
            }
            return res;
        }

        private string[] GetLogos()
        {
            return GetLogosFromRegistry(true);
        }

        private bool GetLogoName(out string[] logos)
        {

            logos = GetLogosFromRegistry(true);
            DialogResult dgr = MessageBox.Show("логотип: " + logos[0] + '\n' + "Это правильно?",
                "Проверьте выбранные логотипы", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            while (dgr == DialogResult.No)
            {
                logos = GetLogosFromRegistry(false);
                dgr = MessageBox.Show("логотип: " + logos[0] + '\n' + "Это правильно?",
                "Проверьте выбранные логотипы", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            return (dgr != DialogResult.Cancel);
        }

        private void printAllNumbers_Click(object sender, EventArgs e)
        {
            string[] logos;
            if (!GetLogoName(out logos))
                return;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, false, cn))
                return;
            try
            {
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;




                Excel.Range titles = ws.get_Range("C1", "C1");
                titles.ColumnWidth = 3.0;

                titles = ws.get_Range("A1", "A1");
                titles.ColumnWidth = 44;
                titles = ws.get_Range("D1", "D1");
                titles.ColumnWidth = 44;

                titles = ws.get_Range("B1", "B1");
                titles.ColumnWidth = 23;
                titles = ws.get_Range("E1", "E1");
                titles.ColumnWidth = 23;

                int i = 0;

                DataTable dTable = SetDataGrid();

                string competitionTitle = StaticClass.ParseCompTitle(this.competitionTitle)[3];

                foreach (DataRow rd in dTable.Rows)
                {
                    if (i % 2 == 0)
                        titles = ws.get_Range('A' + (3 * i / 2 + 1).ToString(), 'A' + (3 * i / 2 + 1).ToString());
                    else
                        titles = ws.get_Range('D' + (3 * (i - 1) / 2 + 1).ToString(), 'D' + (3 * (i - 1) / 2 + 1).ToString());
                    titles.Style = "StyleName";
                    //titles.ColumnWidth=44;
                    titles.RowHeight = 30;
                    titles.Cells[1, 1] = "  " + rd[2].ToString() + ' ' + rd[1].ToString();

                    if (i % 2 == 0)
                        titles = ws.get_Range('B' + (3 * i / 2 + 1).ToString(), 'B' + (3 * i / 2 + 1).ToString());
                    else
                        titles = ws.get_Range('E' + (3 * (i - 1) / 2 + 1).ToString(), 'E' + (3 * (i - 1) / 2 + 1).ToString());
                    titles.Style = "styleGroup";
                    //titles.ColumnWidth=23;

                    titles.Cells[1, 1] = rd[3].ToString() + "  ";

                    if (i % 2 == 0)
                        titles = ws.get_Range('A' + (3 * i / 2 + 2).ToString(), 'B' + (3 * i / 2 + 2).ToString());
                    else
                        titles = ws.get_Range('D' + (3 * (i - 1) / 2 + 2).ToString(), 'E' + (3 * (i - 1) / 2 + 2).ToString());
                    titles.Merge(Type.Missing);
                    titles.Style = "StyleNumber";
                    //titles.ColumnWidth=30;
                    titles.RowHeight = 190;
                    titles.Cells[1, 1] = rd[0].ToString();

                    if (i % 2 == 0)
                        titles = ws.get_Range('A' + (3 * i / 2 + 3).ToString(), 'B' + (3 * i / 2 + 3).ToString());
                    else
                        titles = ws.get_Range('D' + (3 * (i - 1) / 2 + 3).ToString(), 'E' + (3 * (i - 1) / 2 + 3).ToString());
                    titles.Merge(Type.Missing);
                    titles.Style = "StyleComp";
                    //titles.ColumnWidth=30;
                    titles.RowHeight = 60;
                    titles.Cells[1, 1] = competitionTitle;

                    try
                    {
                        if (i % 2 == 0)
                        {
                            if (logos[0] != "no_logo")
                            {
                                //ws.Shapes.AddPicture(logos[0], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 5, (float)(279.75 * i / 2 + 194.0), 175, 40);
                                ws.Shapes.AddPicture(logos[0], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 5, (float)(279.75 * i / 2 + 194.0), 350, 40);
                            }
                            //if (logos[1] != "no_logo")
                            //ws.Shapes.AddPicture(logos[1], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 155, (float)(279.75 * i / 2 + 194.0), 175, 40);
                        }
                        else
                        {
                            if (logos[0] != "no_logo")
                            {
                                //ws.Shapes.AddPicture(logos[0], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 385, (float)(279.75 * (i - 1) / 2 + 194.0), 175, 40);
                                ws.Shapes.AddPicture(logos[0], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 385, (float)(279.75 * (i - 1) / 2 + 194.0), 350, 40);
                            }
                            //if (logos[1] != "no_logo")
                            //ws.Shapes.AddPicture(logos[1], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 535, (float)(279.75 * (i - 1) / 2 + 194.0), 175, 40);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show("Bib creation error.\r\n" + ex.Message +
                                          "\r\nContinue?", "Bib creation error",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Error) ==
                                         DialogResult.No)
                            break;
                        /*StreamWriter sw = new StreamWriter(dir + '\\' + "logos.txt");
                        sw.WriteLine("");
                        sw.Close();*/
                    }

                    i++;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void printOneNumber_Click(object sender, EventArgs e)
        {
            if (tbPartNumber.Text == "")
                return;



            string[] logos;

            if (!GetLogoName(out logos))
                return;

            Excel.Worksheet ws;
            Excel.Workbook wb;
            Excel.Application xlApp;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, false, cn))
                return;
            try
            {
                ws.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;

                try
                {
                    ws.PageSetup.LeftMargin = xlApp.InchesToPoints(0.196850393700787);
                    ws.PageSetup.RightMargin  = xlApp.InchesToPoints(0.196850393700787);
                    ws.PageSetup.TopMargin = xlApp.InchesToPoints(0.196850393700787);
                    ws.PageSetup.BottomMargin = xlApp.InchesToPoints(0.196850393700787);
                    ws.PageSetup.HeaderMargin = xlApp.InchesToPoints(0.0);
                    ws.PageSetup.FooterMargin = xlApp.InchesToPoints(0.0);
                    ws.PageSetup.CenterHorizontally = ws.PageSetup.CenterVertically = true;
                    ws.PageSetup.PaperSize = (Excel.XlPaperSize)70;
                    ws.PageSetup.Zoom = false;
                    ws.PageSetup.FitToPagesTall = 1;
                    ws.PageSetup.FitToPagesWide = 1;
                }
                catch { }

                Excel.Range titles;
                titles = ws.get_Range("C1", "C1");
                titles.ColumnWidth = 3.0;

                titles = ws.get_Range("A1", "A1");
                titles.Style = "StyleName";
                titles.ColumnWidth = 44;
                titles.RowHeight = 30;
                titles.Cells[1, 1] = "  " + tbPartSurname.Text + ' ' + tbPartName.Text;

                titles = ws.get_Range("B1", "B1");
                titles.Style = "styleGroup";
                titles.ColumnWidth = 23;
                titles.Cells[1, 1] = tbPartTeam.Text + " ";

                titles = ws.get_Range("A2", "B2");
                titles.Merge(Type.Missing);
                titles.Style = "StyleNumber";
                titles.RowHeight = 190;
                titles.Cells[1, 1] = tbPartNumber.Text;
                if (logos[0] != "no_logo")
                {
                    ws.Shapes.AddPicture(logos[0], Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, 5, 194, 350, 40);
                }


                titles = ws.get_Range("A3", "B3");
                titles.Merge(Type.Missing);
                titles.Style = "StyleComp";
                titles.RowHeight = 60;
                titles.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[3];
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void btnLoadToInet_Click(object sender, EventArgs e)
        {
            SaveImageToRemote(pbPhoto.Image);
        }

        private void btnGetFromInet_Click(object sender, EventArgs e)
        {
            
            int iid;
            if (!(int.TryParse(tbPartNumber.Text, out iid)))
                return;

            try
            {
                if (compIDForService == null)
                {
                    if (remote == null)
                        return;
                    if (remote.State != ConnectionState.Open)
                        remote.Open();
                    PhotoRemoteTableAdapter ta = new PhotoRemoteTableAdapter();
                    ta.Connection = remote;
                    foreach (dsClimbing.PhotoRemoteRow row in ta.GetDataByIid(iid))
                    {
                        if (row.IsphotoNull())
                            pbPhoto.Image = null;
                        else
                            pbPhoto.Image = ImageWorker.GetFromBytes(row.photo);
                        return;
                    }
                }
                else
                {
                    ClimbingService service = new ClimbingService();
                    var v = service.GetClimber(iid, compIDForService.Value, true);
                    var photo = v.Climber.photo;
                    if (photo == null || photo.Length < 1)
                        pbPhoto.Image = null;
                    else
                        pbPhoto.Image = ImageWorker.GetFromBytes(photo);
                }
            }
            catch (Exception ex) { MessageBox.Show("Не удалось загрузить фото:\r\n" + ex.Message); }
            finally { btnSavePhoto.Enabled = (pbPhoto.Image != null); }
        }

        private void btnGetPhotoFromLocal_Click(object sender, EventArgs e)
        {
            try { pbPhoto.Image = LoadImageFromDB(); }
            catch (Exception ex) { MessageBox.Show(this, "Не удалось загрузить фото:\r\n" + ex.Message); }
            finally { btnSavePhoto.Enabled = (pbPhoto.Image != null); }
        }

        private void btnSavePhoto_Click(object sender, EventArgs e)
        {
            if (pbPhoto.Image == null)
                return;
            if (tbPartNumber.Text.Length < 1)
                return;
            int iid;
            if (!int.TryParse(tbPartNumber.Text, out iid))
                return;
            try
            {
                SaveFileDialog dg = new SaveFileDialog();
                dg.AddExtension = true;
                dg.Filter = "Изображение JPEG (*.jpg)|*.jpg";
                dg.FilterIndex = 0;
                dg.FileName = iid.ToString("000") + "_" + tbPartSurname.Text + "_" + tbPartName.Text + ".jpg";
                dg.FileName = dg.FileName.Replace(' ', '_');
                if (dg.ShowDialog() == DialogResult.Cancel || dg.FileName.Length < 1)
                    return;
                Bitmap bmp = new Bitmap(pbPhoto.Image);
                bmp.Save(dg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex) { MessageBox.Show("Не удалось сохранить фотографию:\r\n" + ex.Message); }
        }

        private void btnDelPhoto_Click(object sender, EventArgs e)
        {
            pbPhoto.Image = null;
            btnSavePhoto.Enabled = false;
        }

        private void btnLoadFromFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            string path = ofd.SelectedPath;
            MainForm.ThrData thrD;
            if (sender == btnLoadFromFolder)
                thrD = new MainForm.ThrData(new Thread(LoadImagesFromFolder), MainForm.ThreadStats.IMG_LOAD_F);
            else if (sender == btnSaveAllPhotos)
                thrD = new MainForm.ThrData(new Thread(SaveImagesToFolder), MainForm.ThreadStats.IMG_SAV_F);
            else
                return;
            MainForm mf = (MainForm)this.MdiParent;
            if (mf.AddThrData(thrD))
                thrD.thr.Start(new LoadSaveImagesThreadArg(path, new MainForm.AfetrStopCallBack(mf.RemThrData)));
            else
                MessageBox.Show(this, "Загрузка уже выполняется. Пожалуйста, попробуйте позже.");
        }

        private sealed class LoadSaveImagesThreadArg
        {
            public string Path { get; private set; }
            public MainForm.AfetrStopCallBack Callback { get; private set; }
            public LoadSaveImagesThreadArg(string path, MainForm.AfetrStopCallBack callback)
            {
                this.Path = path;
                this.Callback = callback;
            }
        }

        private void btnOrderNum_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотите упорядочить номера участников?\r\n" +
                "Существующие номера будут изменены!", "Упорядочить номера", MessageBoxButtons.YesNo,
                 MessageBoxIcon.Warning) == DialogResult.Yes)
                SetNumbersInARow();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            LoadImage();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (changed)
            {
                switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        SaveData();
                        e.Cancel = false;
                        break;
                    case DialogResult.No:
                        e.Cancel = false;
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            base.OnClosing(e);
        }

        private void dg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changed = true;
        }

        private void Secretary_FormClosing(object sender, FormClosingEventArgs e)
        {
            mRem.WaitOne();
            allowRem = false;
            try
            {
                if (remote != null)
                    if (remote.State != ConnectionState.Closed)
                        remote.Close();
            }
            catch { }
            finally { mRem.ReleaseMutex(); }
        }

        private void btnInetImport_Click(object sender, EventArgs e)
        {

            //CopyDataFromOnline();
            //MessageBox.Show("Импорт завершён.");
            //RefreshCbTeams();
            //cbPartGroups.SelectedIndex = 0;
            //cbPartTeams.SelectedIndex = 0;
            //SetDataGrid();
            //MainForm.ThrData td = new MainForm.ThrData(new Thread(CopyDataFromOnline), MainForm.ThreadStats.IMG_LOAD_REM);
            //if (((MainForm)this.MdiParent).AddThrData(td))
            //{

            //LoadDataCn lcn = new LoadDataCn(cn, remote, compIDForService, this,
            //    new MainForm.AfetrStopCallBack(((MainForm)this.MdiParent).RemThrData), client);
            // CopyDataFromOnline(lcn);

            bool result = false;
            try
            {
                this.LoadDataFromSite2();
                result = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Ошибка загрузки данных: {0}", ex.Message));
            }

            RefreshCbGroups();
            RefreshCbTeams();
            cbPartGroups.SelectedIndex = 0;
            cbPartTeams.SelectedIndex = 0;

            if (result)
            {
                if (MessageBox.Show("Упорядочить индивидуальные номера?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    SetNumbersInARow();
            }
            SetDataGrid();
            //    td.thr.Start(lcn);
            //}
        }

        private void btnTeamsInetService(long compID)
        {
            try
            {
                ClimbingService service = new ClimbingService();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                cmd.CommandText = "SELECT iid, name, pos FROM teams(NOLOCK)";
                List<ONLTeamSerializable> teamToUpdate = new List<ONLTeamSerializable>();
                SqlDataReader dr = cmd.ExecuteReader();
                try
                {
                    while (dr.Read())
                    {
                        ONLTeamSerializable serT = new ONLTeamSerializable();
                        serT.CompID = compID;
                        serT.SecretaryId = StaticClass.GetValueTypeValue<int>(dr["iid"], -1);
                        if (serT.SecretaryId < 0)
                            continue;
                        serT.Name = StaticClass.GetNullableString(dr["name"]);
                        if (serT.Name == null)
                            serT.Name = String.Empty;
                        serT.RankingPos = StaticClass.GetValueTypeValue<int>(dr["pos"], int.MaxValue);
                        teamToUpdate.Add(serT);
                    }
                }
                finally { dr.Close(); }
                var upd = service.PostTeams(teamToUpdate.ToArray(), true, compID);
                if (upd.Length == teamToUpdate.Count)
                    MessageBox.Show(this, "Список успешно обновлён");
                else
                    MessageBox.Show(this, "Произошли ошибки при обновлении " +
                        (teamToUpdate.Count - upd.Length).ToString() + " команд(ы).");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки команд:\r\n" + ex.Message); }
        }

        private void btnTeamsInet_Click(object sender, EventArgs e)
        {
            if (compIDForService != null && compIDForService.HasValue)
            {
                btnTeamsInetService(compIDForService.Value);
                return;
            }
            if (remote == null)
                return;
            try
            {
                if (remote.State != ConnectionState.Open)
                    remote.Open();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                SqlCommand cmdR = new SqlCommand();
                cmdR.Connection = remote;
                cmdR.Transaction = remote.BeginTransaction();
                bool trSuccess = false;
                try
                {
                    string inList = "";
                    cmd.CommandText = "SELECT iid, name, pos FROM teams(NOLOCK)";
                    cmdR.CommandText = "InsertTeamGroup @iid, @name, 1, @pos";
                    cmdR.Parameters.Add("@iid", SqlDbType.Int);
                    cmdR.Parameters.Add("@name", SqlDbType.VarChar, 255);
                    cmdR.Parameters.Add("@pos", SqlDbType.Int);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                        {
                            cmdR.Parameters[0].Value = Convert.ToInt32(rdr["iid"]);
                            cmdR.Parameters[1].Value = rdr["name"].ToString();
                            cmdR.Parameters[2].Value = (rdr["pos"] == null ? DBNull.Value : rdr["pos"]);
                            cmdR.ExecuteNonQuery();
                            if (!String.IsNullOrEmpty(inList))
                                inList += ",";
                            inList += cmdR.Parameters[0].Value.ToString();
                        }
                    }
                    finally { rdr.Close(); }
                    if (!string.IsNullOrEmpty(inList))
                    {
                        cmdR.CommandText = "DELETE FROM ONLclimbersUnc WHERE team_id NOT IN(" + inList + ")";
                        cmdR.ExecuteNonQuery();
                        cmdR.CommandText = "DELETE FROM ONLteams WHERE iid NOT IN(" + inList + ")";
                        cmdR.ExecuteNonQuery();
                    }
                    trSuccess = true;
                }
                finally
                {
                    if (trSuccess)
                    {
                        cmdR.Transaction.Commit();
                        MessageBox.Show("Синхронизация завершена");
                    }
                    else
                        cmdR.Transaction.Rollback();
                }

            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка синхронизации:\r\n" + ex.Message); }
            finally
            {
                try { remote.Close(); }
                catch { }
            }
        }

        private void btnGroupsInet_Click(object sender, EventArgs e)
        {
            if (remote == null)
                return;
            try
            {
                if (remote.State != ConnectionState.Open)
                    remote.Open();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                SqlCommand cmdR = new SqlCommand();
                cmdR.Connection = remote;
                cmdR.Transaction = remote.BeginTransaction();
                bool trSuccess = false;
                try
                {
                    string inList = "";
                    cmd.CommandText = "SELECT iid, name, oldYear, youngYear, genderFemale, minQf FROM groups(NOLOCK)";
                    cmdR.CommandText = "InsertTeamGroup @iid, @name, 0, @yO, @yY, @gF, @minQf";
                    cmdR.Parameters.Add("@iid", SqlDbType.Int);
                    cmdR.Parameters.Add("@name", SqlDbType.VarChar, 255);
                    cmdR.Parameters.Add("@yO", SqlDbType.Int);
                    cmdR.Parameters.Add("@yY", SqlDbType.Int);
                    cmdR.Parameters.Add("@gF", SqlDbType.Bit);
                    cmdR.Parameters.Add("@minQf", SqlDbType.Int);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                        {
                            cmdR.Parameters[0].Value = Convert.ToInt32(rdr["iid"]);
                            cmdR.Parameters[1].Value = rdr["name"].ToString();
                            cmdR.Parameters[2].Value = rdr["oldYear"];
                            cmdR.Parameters[3].Value = rdr["youngYear"];
                            cmdR.Parameters[4].Value = rdr["genderFemale"];
                            cmdR.Parameters[5].Value = rdr["minQf"];
                            cmdR.ExecuteNonQuery();
                            if (!String.IsNullOrEmpty(inList))
                                inList += ",";
                            inList += cmdR.Parameters[0].Value.ToString();
                        }
                    }
                    finally { rdr.Close(); }
                    if (!string.IsNullOrEmpty(inList))
                    {
                        cmdR.CommandText = "DELETE FROM ONLclimbersUnc WHERE group_id NOT IN(" + inList + ")";
                        cmdR.ExecuteNonQuery();
                        cmdR.CommandText = "DELETE FROM ONLgroups WHERE iid NOT IN(" + inList + ")";
                        cmdR.ExecuteNonQuery();
                    }
                    trSuccess = true;
                }
                finally
                {
                    if (trSuccess)
                    {
                        cmdR.Transaction.Commit();
                        MessageBox.Show("Синхронизация завершена");
                    }
                    else
                        cmdR.Transaction.Rollback();
                }

            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка синхронизации:\r\n" + ex.Message); }
            finally
            {
                try { remote.Close(); }
                catch { }
            }
        }

        private void ImportRanking()
        {
            try
            {
                if (remote == null)
                {
                    MessageBox.Show("Соединение с удалённой БД не настроено");
                    return;
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT iid,name FROM groups(NOLOCK) ORDER BY iid";
                string[] styles = { "Трудность", "Скорость", "Боулдеринг" };
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                    {
                        int iid = Convert.ToInt32(rdr["iid"]);
                        string name = rdr["name"].ToString();
                        foreach(string s in styles)
                            try { ImportRanking(iid, s, cn.ConnectionString, remote.ConnectionString); }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка импорта рейтинга (" + name +
                                   ", " + s + "):\r\n" + ex.Message);
                            }
                    }
                }
                finally { rdr.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка импорта рейтинга: " + ex.ToString());
            }
        }

        private static void ImportRanking(int groupID, string style, string connectionString,
            string remoteString)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlConnection remote = new SqlConnection(remoteString);
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT name FROM groups(NOLOCK) WHERE iid=" + groupID.ToString();
                string grName = cmd.ExecuteScalar().ToString();
                OpenFileDialog dg = new OpenFileDialog();
                dg.CheckFileExists = true;
                dg.CheckPathExists = true;
                dg.Title = grName + " " + style;
                DialogResult dgRes = dg.ShowDialog();
                if (dgRes == DialogResult.Cancel)
                    return;
                //MessageBox.Show(dg.FileName);

                Excel.Application xlApp = new Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("Excel не может быть запущен.");
                    return;
                }
                xlApp.Visible = true;
                Excel.Workbook wb;
                try
                {
                    wb = xlApp.Workbooks.Open(dg.FileName, Type.Missing, false, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (wb == null)
                {
                    MessageBox.Show("Ошибка открытия файла.");
                    return;
                }
                Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
                if (ws == null)
                {
                    MessageBox.Show("Лист Excel не может быть открыт.");
                    return;
                }


                ((Excel._Worksheet)ws).Activate();

                int emptyRows = 0;
                int nRow = 0;
                List<clmRankingStruct> listToInsert = new List<clmRankingStruct>();
                do
                {
                    //A - место
                    //B - Фамилия Имя
                    //C - Команда
                    //D - Г.р.
                    nRow++;
                    Excel.Range rng = ws.get_Range("B" + nRow.ToString(), "B" + nRow.ToString());
                    string name = rng.Text.ToString().Trim();
                    if (String.IsNullOrEmpty(name))
                    {
                        emptyRows++;
                        continue;
                    }
                    else if (name.ToLower().IndexOf("Фамилия") > -1)
                    {
                        emptyRows = 0;
                        continue;
                    }
                    while (name.IndexOf("  ") > -1)
                        name.Replace("  ", " ");
                    name = name.Trim();
                    emptyRows = 0;

                    rng = ws.get_Range("D" + nRow.ToString(), "D" + nRow.ToString());
                    string strAge = rng.Text.ToString().Trim();
                    int age;
                    if (!int.TryParse(strAge, out age))
                        continue;
                    if (age < 50)
                        age += 2000;
                    else if (age < 100)
                        age += 1900;
                    int nPos;
                    rng = ws.get_Range("A" + nRow.ToString(), "A" + nRow.ToString());
                    string strPos = rng.Text.ToString().Trim();
                    if (!int.TryParse(strPos, out nPos))
                        continue;

                    int nTeam;
                    rng = ws.get_Range("C" + nRow.ToString(), "C" + nRow.ToString());
                    string strTeam = rng.Text.ToString().Trim();
                    cmd.CommandText = "SELECT iid FROM teams(NOLOCK) WHERE name=@team";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@team", SqlDbType.VarChar, 255);
                    cmd.Parameters[0].Value = strTeam;
                    object oTmp = cmd.ExecuteScalar();
                    if (oTmp == null || oTmp == DBNull.Value)
                    {
                        nTeam = (int)SortingClass.GetNextIID("teams", "iid", cn, null);
                        cmd.CommandText = "INSERT INTO teams(iid,name) VALUES (@iid,@team)";
                        cmd.Parameters.Add("@iid", SqlDbType.Int);
                        cmd.Parameters[1].Value = nTeam;
                        cmd.ExecuteNonQuery();
                    }
                    else
                        nTeam = Convert.ToInt32(oTmp);

                    listToInsert.Add(new clmRankingStruct(
                        name, age, nTeam, groupID, style, nPos));

                } while (emptyRows < 5);
                remote.Open();
                SqlTransaction tran = remote.BeginTransaction();
                bool tranSuccess = false;
                try
                {
                    cmd.Connection = remote;
                    cmd.Transaction = tran;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM ONLrankings WHERE style=@style " +
                        " AND group_id=@groupID";
                    cmd.Parameters.Add("@style", SqlDbType.VarChar, 255);
                    cmd.Parameters[0].Value = style;
                    cmd.Parameters.Add("@groupID", SqlDbType.Int);
                    cmd.Parameters[1].Value = groupID;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO ONLrankings(climber,age,style,group_id,team_id,pos) " +
                        "VALUES(@climber,@age,@style,@groupID,@team,@pos)";
                    cmd.Parameters.Add("@climber", SqlDbType.VarChar, 4000);
                    cmd.Parameters.Add("@age", SqlDbType.Int);
                    cmd.Parameters.Add("@team", SqlDbType.Int);
                    cmd.Parameters.Add("@pos", SqlDbType.Int);
                    foreach (var p in listToInsert)
                    {
                        cmd.Parameters[2].Value = p.climber;
                        cmd.Parameters[3].Value = p.age;
                        cmd.Parameters[4].Value = p.team_id;
                        cmd.Parameters[5].Value = p.pos;
                        cmd.ExecuteNonQuery();
                    }
                    tranSuccess = true;
                }
                finally
                {
                    if (tranSuccess)
                        tran.Commit();
                    else
                        tran.Rollback();
                }
                xlApp.Quit();
                MessageBox.Show(style + " " + grName + ": Импортировано " + listToInsert.Count.ToString() +
                    " участников");
            }
            finally
            {
                try { cn.Close(); }
                catch { }
                try { remote.Close(); }
                catch { }
            }
        }

        private class clmRankingStruct
        {
            public string climber { get; private set; }
            public int pos { get; private set; }
            public int age { get; private set; }
            public int group_id { get; private set; }
            public int team_id { get; private set; }
            public string style { get; private set; }
            public clmRankingStruct(string climber, int age, int team_id,
                int group_id, string style, int pos)
            {
                this.climber = climber;
                this.age = age;
                this.group_id = group_id;
                this.pos = pos;
                this.style = style;
                this.team_id = team_id;
            }
        }

        private void btnImportRankings_Click(object sender, EventArgs e)
        {
            ImportRanking();
        }

        private void btnDelEmptyTeams_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотите удалить команды без участников?", "", MessageBoxButtons.YesNo,
                 MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                return;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = @"SELECT t.name 
                                      FROM teams t(NOLOCK)
                                     WHERE not exists(select *
                                                        from Participants p(nolock)
                                                       where p.team_id = t.iid)
                                  ORDER BY t.name";
                cmd.Transaction = cn.BeginTransaction();
                try
                {

                    string sMessage = string.Empty;
                    int nCnt = 0;
                    var rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                        {
                            nCnt++;
                            if (sMessage != string.Empty)
                                sMessage += "\r\n";
                            sMessage += nCnt.ToString() + ". " + (rdr[0] == DBNull.Value ? String.Empty : rdr[0].ToString());
                        }
                    }
                    finally { rdr.Close(); }
                    if (nCnt < 1)
                    {
                        cmd.Transaction.Rollback();
                        MessageBox.Show(this, "Нет команд для удаления");
                        return;
                    }
                    sMessage = "Будут удалены следующие команды (всего " + nCnt.ToString() + ")\r\n\r\n" + sMessage +
                        "\r\n\r\nПродолжить?";
                    if (MessageBox.Show(this, sMessage, String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        == System.Windows.Forms.DialogResult.No)
                    {
                        cmd.Transaction.Rollback();
                        return;
                    }
                    cmd.CommandText = "DELETE FROM teams " +
                        "WHERE not exists(select * from participants p(nolock) where p.team_id = teams.iid)";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception exIn)
                {
                    cmd.Transaction.Rollback();
                    throw exIn;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка удаления пустых команд:\r\n" +
                    ex.Message);
            }
        }

        private void btnTeamsPrintTB_Click(object sender, EventArgs e)
        {
            int? currentTeam;
            int nTmp;
            if (int.TryParse(tbTeamsIID.Text, out nTmp))
                currentTeam = nTmp;
            else
                currentTeam = null;
            SafetyRepParams sRep = new SafetyRepParams(cn, competitionTitle, currentTeam);
            sRep.ShowDialog(this);
        }

        private void btnTeamPrintOrders_Click(object sender, EventArgs e)
        {
            int nTeam;
            if (!int.TryParse(tbTeamsIID.Text, out nTeam))
            {
                MessageBox.Show(this, "Команда не выбрана. Печать ордеров невозможна.");
                return;
            }
            P_Kord ordForm = new P_Kord(cn, competitionTitle, nTeam);
            ordForm.ShowDialog(this);
        }

        private void btnAddTeams_Click(object sender, EventArgs e)
        {
            int nIid;
            if (!int.TryParse(tbPartNumber.Text, out nIid))
            {
                MessageBox.Show(this, "Участник не выбран");
                return;
            }
#if !DEBUG
            try
            {
#endif
                TableDataChange.FillAdditionalTeams(nIid, cn, null, this);
                SetDataGrid();
#if !DEBUG
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка обновления списка доп.команд:\r\n" + ex.Message); }
#endif
        }
    }
}