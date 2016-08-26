using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.Text;
using System.Diagnostics;

namespace ClimbingCompetition
{
    /// <summary>
    /// Ввод результатов трудности
    /// </summary>
    public partial class ResultListLead
#if FULL
        : BaseListForm
#endif
    {
#if FULL
        //SqlConnection cn;
        string gName, round;
        int judgeID;
        int quote;
        SqlDataAdapter daStart, daRes;
        DataTable dtRes = new DataTable();
        DataTable dtStart = new DataTable();
        int climbingTime, minElapsed, secElapsed, baseIID, base2routesIID;
        bool allowEvent = true;
        bool baseTwoRoutes;
        bool baseFlash;
        bool bSfin;
        bool isBoulder;
        bool isQuali = false;
        bool needToErase = true;
        bool useLeadTime;
        bool toEnableNextRound;
        bool ___hasTimeVar = false;

        bool hasTime
        {
            get { return useLeadTime ? ___hasTimeVar : false; }
            set
            {
                if (useLeadTime)
                    ___hasTimeVar = value;
            }
        }

        int qfID = 0;
        public const string RES_ROLLBACK_STRING = "Откатить последний введенный результат";
        public const string RES_RESTORE_STRING = "Вернуть последний результат";

        public const string RES_NO_ROLLBACK = "Нет предыдущего результата для восстановления";
        public const string RES_NO_RESTORE = "Нет отмененных результатов для восстановления";
        
        public ResultListLead(SqlConnection baseCon, string competitionTitle, int listID, bool isPrevRound, bool bSfin)
            : base(baseCon, competitionTitle, listID)
        {
            InitializeComponent();

            //this.btnEditTopo.Visible = this.btnEditTopo.Enabled = false;
            try
            {
                this.ToolTipDefault.SetToolTip(btnRollBack, RES_NO_ROLLBACK);
                this.ToolTipDefault.SetToolTip(btnRestore, RES_NO_RESTORE);
                //if (cn.State != ConnectionState.Open)
                //    cn.Open();
                StartList.ArrangeStartNumbers(listID, cn, null);
                this.bSfin = bSfin;
                this.toEnableNextRound = isPrevRound;
                btnNxtRound.Enabled = isPrevRound;
                
                //this.cn = cn;

                this.remain40sec = SettingsForm.GetIncl40sec(this.cn);
                

                SortingClass.CheckColumn("routeResults", "timeText", "varchar(50)", this.cn);
                SortingClass.CheckColumn("routeResults", "timeValue", "bigint", this.cn);
                AccountForm.CreateTopoTable(this.cn);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = this.cn;
                cmd.CommandText = "SELECT g.name, lists.round, lists.climbingTime, lists.prev_round, lists.quote, lists.judge_id, lists.style " +
                                  "  FROM lists(NOLOCK) " +
                                  "  JOIN Groups g(NOLOCK)ON lists.group_id = g.iid " +
                                  " WHERE lists.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[0].Value = this.listID;
                SqlDataReader rdr = cmd.ExecuteReader();
                round = String.Empty;
                while (rdr.Read())
                {
                    gName = rdr[0].ToString();

                    round = rdr[1].ToString().Trim();
                    isQuali = (round.IndexOf("валификация") > -1);
                    if (bSfin)
                        this.Text = "Боулдеринг - " + gName + " - " + round;
                    else
                        this.Text = "Трудность - " + gName + " - " + round;

                    try
                    {
                        climbingTime = Convert.ToInt32(rdr[2]);
                        lblTime.Text = climbingTime.ToString("00") + ":00";
                    }
                    catch
                    {
                        climbingTime = -1;
                        lblTime.Text = "";
                    }
                    tbRes.ReadOnly = true;
                    if (this.useLeadTime)
                        tbTime.ReadOnly = true;

                    try { baseIID = Convert.ToInt32(rdr[3]); }
                    catch { baseIID = -1; }

                    //cmd.CommandText = "SELECT quote FROM lists WHERE iid = " + listID.ToString();

                    try { quote = Convert.ToInt32(rdr["quote"]); }
                    catch { quote = -1; }

                    try { judgeID = Convert.ToInt32(rdr["judge_id"]); }
                    catch { judgeID = 0; }

                    try { isBoulder = (rdr["style"].ToString() == "Боулдеринг"); }
                    catch { isBoulder = bSfin; }
                    break;
                }
                rdr.Close();

                this.useLeadTime = SettingsForm.GetLeadTime(this.cn) && (round.Equals("финал", StringComparison.OrdinalIgnoreCase) || round.Equals("суперфинал", StringComparison.OrdinalIgnoreCase));
                tbTime.Visible = lblTimeClimb.Visible = this.useLeadTime;

                baseTwoRoutes = false;
                int prvRound = baseIID;
                bool b = true;
                if (baseIID > 0)
                {
                    cmd.CommandText = "SELECT round,prev_round FROM lists WHERE iid = @iid";
                    while (b)
                    {
                        cmd.Parameters[0].Value = prvRound;
                        rdr = cmd.ExecuteReader();
                        try
                        {
                            if (rdr.Read())
                            {
                                baseTwoRoutes = (rdr[0].ToString() == "1/4 финала (2 трассы)");
                                if (baseTwoRoutes)
                                {
                                    base2routesIID = prvRound;
                                    b = false;
                                }
                                if (prvRound == baseIID)
                                    baseFlash = (rdr[0].ToString() == "Квалификация");
                                try
                                {
                                    prvRound = Convert.ToInt32(rdr[1]);
                                    cmd.Parameters[0].Value = prvRound;
                                }
                                catch { b = false; }
                            }
                        }
                        finally { rdr.Close(); }
                    }
                }

                daStart = new SqlDataAdapter();
                daStart.SelectCommand = new SqlCommand();
                daStart.SelectCommand.Connection = this.cn;

                daStart.SelectCommand.CommandText = String.Format("SELECT l.start AS [{0}], l.climber_id AS [{1}], (p.surname+' '+" +
                    "p.name) AS [{2}], p.age AS [{3}], p.qf AS [{4}], dbo.fn_getTeamName(p.iid, l.list_id) AS [{5}] " +
                    "FROM routeResults l(NOLOCK) JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "WHERE (l.preQf = 0) AND (l.list_id = @listID) AND (res IS NULL) ORDER BY l.start",
                     GetSortable(START), GetSortable(IID), GetSortable(CLIMBER), GetSortable(AGE), GetSortable(QF), GetSortable(TEAM));
                daStart.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                daStart.SelectCommand.Parameters[0].Value = this.listID;

                daRes = new SqlDataAdapter();
                daRes.SelectCommand = new SqlCommand();
                daRes.SelectCommand.Connection = cn;

                daRes.SelectCommand.CommandText = String.Format("SELECT l.posText AS [{0}], l.start AS [{1}], (p.surname+' '+" +
                    "p.name) AS [{2}], p.age AS [{3}], p.qf AS [{4}], dbo.fn_getTeamName(p.iid, l.list_id) AS [{5}], " +
                    "l.resText AS [{6}],ISNULL(l.timeText,'') AS [{7}], l.ptsText AS [{8}], l.qf AS [{9}], p.iid FROM routeResults l(NOLOCK) JOIN Participants p(NOLOCK) ON l.climber_id = p.iid " +
                    "WHERE (l.list_id = @listID) AND (l.preQf = 0) AND (res IS NOT NULL) ORDER BY l.res DESC, L.pos, p.vk, [{5}], p.surname, p.name",
                    GetSortable(POS), GetSortable(START), GetSortable(CLIMBER), GetSortable(AGE),
                    GetSortable(QF), GetSortable(TEAM), GetSortable(RESULT), GetSortable(TIMERES), GetSortable(PTS), GetSortable(NEXTROUND));

                daRes.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                daRes.SelectCommand.Parameters[0].Value = this.listID;
                //RefreshData();
                if (round.IndexOf("валификац") > -1)
                {
                    cmd.CommandText = "SELECT lq.iid FROM lists lq(NOLOCK) JOIN lists lt(NOLOCK) ON " +
                        "(lq.group_id = lt.group_id) AND (lq.style = lt.style) WHERE " +
                        "(lq.round = @lqRound) AND (lt.iid = @ltIID)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@lqRound", SqlDbType.VarChar, 50);
                    cmd.Parameters[0].Value = "Квалификация (2 трассы)";
                    cmd.Parameters.Add("ltIID", SqlDbType.Int);
                    cmd.Parameters[1].Value = this.listID;
                    try { qfID = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch { qfID = 0; }
                }
                //RefreshTable(baseTwoRoutes);
                GetResultList();
                RefreshData();
                if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                    try { btnNxtRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                    catch { btnNxtRound.Enabled = true; }
                }
                else
                    btnNxtRound.Enabled = false;
                
                if (btnNxtRound.Enabled)
                {
                    cmd.CommandText = "SELECT iid_parent FROM lists(NOLOCK) WHERE iid=" + listID.ToString();
                    try
                    {
                        object oTmp = cmd.ExecuteScalar();
                        btnNxtRound.Enabled = (oTmp == null || oTmp == DBNull.Value);
                    }
                    catch { }
                }

                if (!btnNxtRound.Enabled)
                    cbRound.Enabled = toEnableNextRound = false;
                LoadEnabled();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                needToClose = true;
            }
        }

        private void LoadEnabled()
        {
            if (!toEnableNextRound)
                return;
            try
            {
                SqlCommand cmd = new SqlCommand();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = @"SELECT COUNT(*) cnt
                                  FROM routeResults(NOLOCK) 
                                 WHERE qf IN('Q','q',' q',' Q')
                                   AND list_id = " + listID.ToString();
                bool res = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                if (this.InvokeRequired)
                    this.Invoke(new EventHandler(delegate { btnNxtRound.Enabled = cbRound.Enabled = res; }));
                else
                    btnNxtRound.Enabled = cbRound.Enabled = res;
            }
            catch { }
        }

        private void RefreshData()
        {
            try
            {
                needToErase = true;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                dtStart.Clear();
                daStart.Fill(dtStart);
                dgStart.DataSource = dtStart;
                dgStart.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                dtRes.Clear();
                daRes.Fill(dtRes);
                dgRes.DataSource = dtRes;
                dgRes.Columns["iid"].Visible = false;
                for (int i = 0; i < dgRes.Columns.Count; i++)
                    dgRes.Columns[i].ReadOnly = true;
                if (useLeadTime)
                {
                    hasTime = false;
                    foreach (DataRow r in dtRes.Rows)
                        if (!String.IsNullOrEmpty(r[TIMERES].ToString()))
                        {
                            hasTime = true;
                            break;
                        }

                    dgRes.Columns[TIMERES].Visible = hasTime;
                }
                else
                    dgRes.Columns[TIMERES].Visible = false;

                /*for (int i = 0; i < 6; i++)
                    dgRes.Columns[i].ReadOnly = true;
                dgRes.Columns[6].ReadOnly = false;
                dgRes.Columns[7].ReadOnly = true;*/
                dgRes.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally
            {
                try { LoadEnabled(); }
                catch { }
                try { SetTransactionButtons(); }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        SqlTransaction tran = null;

        private bool EnterRes(int iid, string result, string timeRes, out string sTime)
        {
            return EnterRes(iid, result, timeRes, true, out sTime);
        }

        private bool EnterRes(int iid, string result, string timeRes, bool updateNowClimbing, out string sTime)
        {
            bool success = false;

            sTime = (String.IsNullOrEmpty(timeRes) ? String.Empty : timeRes.Trim());

            long? lnTime;
            if (useLeadTime)
            {
                bool wasInitTime = !String.IsNullOrEmpty(sTime);
                lnTime = parseTime(ref sTime);
                if (wasInitTime && (lnTime == null || !lnTime.HasValue))
                {
                    MessageBox.Show(this, "Время введено неверно");
                    return false;
                }
            }
            else
                lnTime = null;

            tran = cn.BeginTransaction();
            long? resID = null;
            Dictionary<string, string> oldValues = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;

                cmd.CommandText = "SELECT iid FROM routeResults WHERE list_id=" + listID.ToString() + " AND climber_id=" + iid.ToString();
                object oTmp = cmd.ExecuteScalar();
                if (oTmp == null || oTmp == DBNull.Value)
                    return false;
                resID = new long?(Convert.ToInt64(oTmp));

                if (updateNowClimbing)
                    StaticClass.GetTableData((backupTableName ?? "routeResults"), "iid", resID.Value, cn, out oldValues, tran);

                long res = 0;
                if (String.IsNullOrEmpty(result.Trim()))
                {
                    if (updateNowClimbing)
                        success = ClearClimber(iid, listID, cn, tran);
                    return false;
                }
                else
                {
                    res = ParseResult(ref result, baseIID < 1);
                    if (res >= 9 && HasTopo(listID, cmd.Connection, cmd.Transaction))
                    {
                        if (!EditTopo.CheckTopoHold(result, listID, cmd.Connection, cmd.Transaction))
                            if (MessageBox.Show(this,
                                String.Format("Зацепки с номером {0} нет в схеме. Сохранить результат?", result.Trim()),
                                String.Empty,
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                == System.Windows.Forms.DialogResult.No)
                                return false;
                    }
                }


                cmd.CommandText = "UPDATE routeResults SET resText = @rt, res = @res, timeText = @timeText, timeValue = @timeVal WHERE (list_id=@lid) AND (climber_id=@cid)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@rt", SqlDbType.VarChar, 50);
                result = result.Trim();
                cmd.Parameters[0].Value = result;
                cmd.Parameters.Add("@res", SqlDbType.BigInt);
                cmd.Parameters[1].Value = res;
                cmd.Parameters.Add("@lid", SqlDbType.Int);
                cmd.Parameters[2].Value = listID;
                cmd.Parameters.Add("@cid", SqlDbType.Int);
                cmd.Parameters[3].Value = iid;
                cmd.Parameters.Add("@timeText", SqlDbType.VarChar, 50).Value =
                    (lnTime == null || !lnTime.HasValue || lnTime.Value < 1) ? String.Empty : " " + sTime;
                cmd.Parameters.Add("@timeVal", SqlDbType.BigInt).Value =
                    (lnTime == null || !lnTime.HasValue || lnTime.Value < 1) ? DBNull.Value : (object)lnTime.Value;
                cmd.ExecuteNonQuery();

                if (updateNowClimbing)
                {
                    cmd.CommandText = "UPDATE lists SET nowClimbingTmp = nowClimbing WHERE iid = " + listID.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE lists SET nowClimbing = NULL WHERE iid=@lid";
                    cmd.Parameters[3].Value = DBNull.Value;
                    cmd.ExecuteNonQuery();
                }
                GetResultList();

                success = true;
            }
#if !DEBUG
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения результата:\r\n" + ex.Message);
                return false;
            }
#endif
            finally
            {
                if (success && resID != null && resID.HasValue)
                {
                    if (updateNowClimbing)
                    {
                        Dictionary<string, string> newValues;
                        StaticClass.GetTableData("routeResults", "iid", resID.Value, cn, out newValues, tran);
                        StaticClass.AddTransaction("routeResults", "iid", resID.Value, oldValues, newValues, cn, -1, tran);
                    }
                    tran.Commit();
                }
                else
                {
                    try { tran.Rollback(); }
                    catch { }
                }
            }


            if (success)
                RefreshData();
            return success;
        }

        private void GetResultList()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            cmd.Connection = cn;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = @"SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText,
                                       CASE l.resText WHEN 'н/я' THEN 2
                                                      WHEN 'дискв.' THEN 1
                                            ELSE 0 END sp, l.pos pos0, ISNULL(l.res,0) res,
                                       ISNULL(l.timeValue," + int.MaxValue.ToString() + @") timeValueS
                                  FROM routeResults l(NOLOCK)
                                  JOIN participants c(NOLOCK) ON c.iid = l.climber_id
                                 WHERE l.resText <> '' AND l.resText IS NOT NULL
                                   AND l.list_id=" + listID.ToString() + @"
                                   AND l.preQf = 0
                              ORDER BY l.res DESC, l.pos, timeValueS";
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count < 1)
                return;
            long cRes = Convert.ToInt64(dt.Rows[0]["res"]);
            int curPos = 1;
            int posCnt = 1;
            dt.Rows[0]["pos0"] = curPos;
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                long fRes = Convert.ToInt64(dt.Rows[i]["res"]);
                if (fRes == cRes)
                    posCnt++;
                else
                {
                    curPos += posCnt;
                    posCnt = 1;
                    cRes = fRes;
                }
                dt.Rows[i]["pos0"] = curPos;
            }
            dt.Columns.Remove("res");
            int cnt = 0;
            foreach (int i in ResultListBoulder.CreateListSeq(listID, cn, tran))
            {
                DataTable dtI;
                dtI = GetList(i);
                if (dtI == null)
                    continue;
                cnt++;
                dt.Columns.Add("rNum" + cnt.ToString(), typeof(int));
                dt.Columns.Add("pos" + cnt.ToString(), typeof(int));
                foreach (DataRow dr in dt.Rows)
                {
                    dr[dt.Columns.Count - 1] = 0;
                    dr[dt.Columns.Count - 2] = 0;
                    foreach (DataRow drI in dtI.Rows)
                        if (drI["iid"].ToString() == dr["iid"].ToString())
                        {
                            dr[dt.Columns.Count - 1] = Convert.ToInt32(drI["pos"]);
                            dr[dt.Columns.Count - 2] = Convert.ToInt32(drI["rNum"]);
                            break;
                        }
                }
                //dtI.Columns["rNum"].ColumnName += cnt.ToString();
                //dtI.Columns["pos"].ColumnName += cnt.ToString();

            }
            if (useLeadTime)
            {
                string sColName = "rNum" + (++cnt).ToString();
                dt.Columns.Add(sColName, typeof(int));
                string sCol2Name = "pos" + cnt.ToString();
                dt.Columns.Add(sCol2Name, typeof(int));
                foreach (DataRow drI in dt.Rows)
                {
                    drI[sColName] = 1;
                    try { drI[sCol2Name] = Convert.ToInt32(drI["timeValueS"]); }
                    catch { drI[sCol2Name] = int.MaxValue; }
                }
            }
            dt.Columns.Remove("timeValueS");
            int q;
            cmd.CommandText = "SELECT ISNULL(quote,0) FROM lists(NOLOCK) WHERE iid = " + listID;
            try
            {
                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                    q = Convert.ToInt32(o);
                else
                    q = 0;
            }
            catch { q = 0; }
            bool isQualy = (round.ToLower().IndexOf("квалификация") == 0);
            CompetitionRules compRules = ((CR & SpeedRules.InternationalRules) == SpeedRules.InternationalRules) ? CompetitionRules.International : CompetitionRules.Russian;
            SortingClass.SortResults(dt, q, cnt < 1, isQualy, compRules);
            cmd.CommandText = "UPDATE routeResults SET pos=@p, posText=@pt, pts=@pts, ptsText = @ptt, qf = @qf " +
                              " WHERE list_id = " + listID.ToString() + " AND climber_id = @cid";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@p", SqlDbType.Int, 4, "pos");
            cmd.Parameters.Add("@pt", SqlDbType.VarChar, 50, "posText");
            cmd.Parameters.Add("@pts", SqlDbType.Float, sizeof(float), "pts");
            cmd.Parameters.Add("@cid", SqlDbType.Int, 4, "iid");
            cmd.Parameters.Add("@ptt", SqlDbType.VarChar, 50, "ptsText");
            cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50, "qf");
            da.UpdateCommand = cmd;
            da.InsertCommand = cmd;
            da.Update(dt);
        }

        private DataTable GetList(int iid)
        {
            ListTypeEnum ltpP = StaticClass.GetListType(iid, cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT round FROM lists(NOLOCK) WHERE iid=" + iid.ToString();
            string round;
            try { round = cmd.ExecuteScalar().ToString().ToLower(); }
            catch { return null; }
            DataTable dt;
            switch (ltpP)
            {
                case ListTypeEnum.LeadGroups:
                    dt = StaticClass.FillResOnsight(iid, cn, true, tran);
                    if (dt.Columns.IndexOf("№") > -1)
                        dt.Columns["№"].ColumnName = "iid";
                    dt.Columns.Add("rNum");
                    DataColumn dcA = null, dcB = null;
                    foreach (DataColumn dc in dt.Columns)
                        if (dc.ColumnName.ToLower().IndexOf("трасса 1") > -1)
                            dcA = dc;
                        else if (dc.ColumnName.ToLower().IndexOf("трасса 2") > -1)
                            dcB = dc;
                    if (dcB == null || dcA == null)
                        return null;
                    foreach (DataRow dr in dt.Rows)
                        if (dr["pos"] != null && dr["pos"] != DBNull.Value && Convert.ToInt32(dr["pos"]) < 1)
                            dr["rNum"] = 3;
                        else if (dr[dcA] != null && dr[dcA] != DBNull.Value && dr[dcA].ToString().Length > 0)
                            dr["rNum"] = 1;
                        else if (dr[dcB] != null && dr[dcB] != DBNull.Value && dr[dcB].ToString().Length > 0)
                            dr["rNum"] = 2;
                        else
                            dr["rNum"] = 3;

                    break;
                case ListTypeEnum.LeadFlash:
                    int routeNumber, nTmp;
                    dt = StaticClass.FillResFlash(iid, cn, true, tran, out routeNumber, true, out nTmp);
                    dt.Columns.Add("rNum", typeof(int));
                    dt.Columns.Add("ppd", typeof(int));
                    foreach (DataRow dr in dt.Rows)
                    {
                        bool aCr = true;
                        for (int i = 1; i <= routeNumber; i++)
                            if (dr["Тр." + i.ToString()].ToString().Length > 0)
                            {
                                aCr = false;
                                break;
                            }
                        if (aCr)
                            dr["rNum"] = 3;
                        else
                            dr["rNum"] = 1;
                        try { dr["ppd"] = Convert.ToInt32(dr["pos"]); }
                        catch { dr["ppd"] = int.MaxValue; }
                    }
                    try
                    {
                        if (dt.Columns.IndexOf("pos") > -1)
                            dt.Columns.Remove("pos");
                        dt.Columns["№"].ColumnName = "iid";
                        dt.Columns["ppd"].ColumnName = "pos";
                    }
                    catch { return null; }


                    for (int i = 0; i < dt.Columns.Count; i++)
                        if (dt.Columns[i].ColumnName != "pos" &&
                            dt.Columns[i].ColumnName != "rNum" &&
                            dt.Columns[i].ColumnName != "iid")
                        {
                            dt.Columns.RemoveAt(i);
                            i--;
                        }
                    break;
                default:
                    cmd.CommandText = "SELECT climber_id iid, CASE preQf WHEN 0 THEN 1 ELSE 3 END rNum, " +
                                      "       CASE preQf WHEN 0 THEN ISNULL(pos, " + int.MaxValue.ToString() + ") ELSE 0 END pos" +
                                      "  FROM routeResults(NOLOCK) " +
                                      " WHERE list_id = " + iid.ToString();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    cmd.Transaction = tran;
                    dt = new DataTable();
                    da.Fill(dt);
                    break;
            }
            return dt;
        }

        private static CultureInfo resultCulture = CultureInfo.GetCultureInfo("en-US");
        public static String GetUnifiedHoldTag(String holdTag, bool allowAdditionalResults, bool insertLeadingSpaces,out long resTotal)
        {
            if (holdTag == null)
                throw new ArgumentNullException("holdTag");
            String res = holdTag.Trim().ToUpperInvariant().Replace(" ", String.Empty);
            if (res.Equals(String.Empty, StringComparison.Ordinal))
                throw new ArgumentException("holdTag can\'t be empty", "holdTag");
            res = res.Replace('Т', 'T').Replace('О', 'O').Replace('П', 'P').Replace('Р', 'P').Replace('\\','/');
            if (res.Equals("TOP", StringComparison.Ordinal))
            {
                resTotal = 10000000;
                return res;
            }
            int modifier = 0;
            if (allowAdditionalResults)
            {
                if (res.Contains("Н/Я"))
                {
                    resTotal = 1;
                    return "н/я";
                }
                if (res.Contains("ДИСКВ"))
                {
                    resTotal = 2;
                    return "дискв.";
                }
                if (res.Contains("+"))
                {
                    modifier = 1;
                    res = res.Replace("+", String.Empty);
                }
                else if (res.Contains("-"))
                {
                    modifier = -1;
                    res = res.Replace("-", String.Empty);
                }
            }
            double resValue;
            resValue = ParseUnversalDouble(res);
            resTotal = (long)(Math.Round(resValue, 2) * 1000) + modifier;
            StringBuilder resBuilder = new StringBuilder();
            if (insertLeadingSpaces)
                resBuilder.Append(' ');
            resBuilder.Append(resValue.ToString("0.##", resultCulture));
            switch (modifier)
            {
                case -1:
                    resBuilder.Append("-");
                    break;
                case 1:
                    resBuilder.Append("+");
                    break;
            }
            return resBuilder.ToString();
        }

        public static double ParseUnversalDouble(String res)
        {
            double resValue;
            if (!double.TryParse(res, out resValue))
            {
                String cultureSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                String resultSeparator = null;
                if (res.Contains("."))
                    resultSeparator = ".";
                else if (res.Contains(","))
                    resultSeparator = ".";
                bool success = false;
                if (resultSeparator != null && !cultureSeparator.Equals(resultSeparator, StringComparison.Ordinal))
                    success = double.TryParse(res.Replace(resultSeparator, cultureSeparator), out resValue);
                if (success)
                    resValue = Math.Round(resValue, 2);
                if (!success || resValue < 0.01)
                    throw new ArgumentException("Invalid result value", "holdTag");
            }
            return resValue;
        }

        private static long ParseResult(ref string result, bool isFrstRound)
        {
            long resTotal;
            try { result = GetUnifiedHoldTag(result, true, true, out resTotal); }
            catch (ArgumentException)
            {
                throw new ArgumentException("Неправильный ввод результата");
            }
            if (resTotal == 1 && !isFrstRound)
                resTotal = 2;
            return resTotal;
        }

        private void SetTransactionButtons(SqlTransaction tran = null)
        {
            long? l1, l2;
            SetTransactionButtons(out l1, out l2, tran);
        }

        private static bool HasTopo(int listId, SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction=tran };
            cmd.CommandText = "SELECT CASE WHEN topo IS NULL THEN 0 ELSE 1 END FROM lists WHERE iid=@iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
            object res = cmd.ExecuteScalar();
            if (res == null)
                return false;
            return (Convert.ToInt32(res) > 0);
        }

        private void SetTransactionButtons(out long? activeTranID, out long? restoreTranID, SqlTransaction tran = null)
        {

            activeTranID = null;
            restoreTranID = null;
            btnRollBack.Enabled = btnRestore.Enabled = false;
            this.ToolTipDefault.SetToolTip(btnRollBack, RES_NO_ROLLBACK);
            this.ToolTipDefault.SetToolTip(btnRestore, RES_NO_RESTORE);

            int climberID;
            if (!int.TryParse(lblIID.Text, out climberID))
                return;
            SqlCommand cmd = new SqlCommand();
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT iid FROM routeResults(NOLOCK) WHERE list_id=" + listID.ToString() + " AND climber_id=" + climberID.ToString();
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return;
            long resID;
            try { resID = Convert.ToInt64(oTmp); }
            catch { return; }
            activeTranID = StaticClass.GetActiveTransaction(resID, "routeResults", cn, tran);
            restoreTranID = StaticClass.GetNextTransaction(resID, "routeResults", cn, tran);
            btnRollBack.Enabled = (activeTranID != null && activeTranID.HasValue);
            btnRestore.Enabled = (restoreTranID != null && restoreTranID.HasValue);

            if (btnRestore.Enabled)
                this.ToolTipDefault.SetToolTip(btnRestore, RES_RESTORE_STRING);
            if (btnRollBack.Enabled)
                this.ToolTipDefault.SetToolTip(btnRollBack, RES_ROLLBACK_STRING);
        }

        private void dgStart_SelectionChanged(object sender, EventArgs e)
        {
            if (allowEvent)
            {
                try
                {
                    tbRes.Text = "";
                    if (useLeadTime)
                        tbTime.Text = String.Empty;
                    lblPos.Text = "";
                    lblTeam.Text = dgStart.CurrentRow.Cells[5].Value.ToString();
                    lblStart.Text = dgStart.CurrentRow.Cells[0].Value.ToString();
                    lblIID.Text = dgStart.CurrentRow.Cells[1].Value.ToString();
                    lblName.Text = dgStart.CurrentRow.Cells[2].Value.ToString();
                    lblAge.Text = dgStart.CurrentRow.Cells[3].Value.ToString();
                    lblQf.Text = dgStart.CurrentRow.Cells[4].Value.ToString();
                    needToErase = true;
                    try { SetTransactionButtons(); }
                    catch { }
                }
                catch { lblTeam.Text = lblStart.Text = lblQf.Text = lblName.Text = lblIID.Text = lblAge.Text = ""; }
            }
        }

        //private void dgRes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = cn;
        //        if (cn.State != ConnectionState.Open)
        //            cn.Open();
        //        int start = Convert.ToInt32(dgRes.CurrentRow.Cells[1].Value);
        //        cmd.CommandText = "SELECT climber_id FROM routeResults(NOLOCK) WHERE list_id = " + listID.ToString() + " AND start = " + start.ToString();
        //        EnterRes(Convert.ToInt32(cmd.ExecuteScalar()),
        //            dgRes.CurrentRow.Cells[6].Value.ToString().ToUpper());
        //    }
        //    catch { }
        //}

        Stopwatch competitorStopwatch = new Stopwatch();
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (btnEdit.Text == "Старт")
            {
                if (string.IsNullOrEmpty(lblIID.Text))
                {
                    MessageBox.Show(this, "Участник не выбран");
                    return;
                }

                if (!this.remain40sec)
                    if (!LeadStart.StartClimber(lblName.Text))
                        return;

                backupTable(null);

                prevRes = tbRes.Text;
                allowEvent = false;
                btnEdit.Text = "Ввод результата";
                btnEditTopo.Text = "Открыть схему";
                btnRestore.Enabled = btnRollBack.Enabled = false;
                btnCancel.Visible = true;
                tbRes.ReadOnly = false;
                if (useLeadTime)
                    tbTime.ReadOnly = false;
                if (needToErase)
                    tbRes.Text = "";
                if (climbingTime > 0)
                {
                    minElapsed = climbingTime;
                    secElapsed = 0;
                    timer.Enabled = true;
                    lblTime.Text = minElapsed.ToString("00") + ":00";
                }
                SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing = @nc WHERE iid = @iid", cn);
                cmd.Parameters.Add("@nc", SqlDbType.Int);
                try { cmd.Parameters[0].Value = Convert.ToInt32(lblIID.Text); }
                catch { cmd.Parameters[0].Value = DBNull.Value; }
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[1].Value = listID;
                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                if (useLeadTime)
                {
                    if (competitorStopwatch.IsRunning)
                        competitorStopwatch.Stop();
                    competitorStopwatch.Reset();
                    competitorStopwatch.Start();
                }

                if (HasTopo(listID, cmd.Connection, cmd.Transaction))
                {
                    OpenTopoRes();
                }

            }
            else
            {
#if !DEBUG
                try
                {
#endif
                //EnterResultLead(Convert.ToInt32(lblIID.Text), tbRes.Text.ToUpper(), true);
                string sResTime;
                if (!EnterRes(Convert.ToInt32(lblIID.Text), tbRes.Text.ToUpper(), (useLeadTime ? tbTime.Text : String.Empty), out sResTime))
                    return;
                if (useLeadTime)
                    tbTime.Text = sResTime;
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
#endif
                SetTransactionButtons();
                btnCancel.Visible = false;
                btnEdit.Text = "Старт";
                btnEditTopo.Text = "Настройка схемы";
                tbRes.ReadOnly = true;
                if (useLeadTime)
                    tbTime.ReadOnly = true;
                timer.Enabled = false;
                allowEvent = true;
                ClearBackup(null);
                RefreshData();
            }
        }

        private void OpenTopoRes()
        {
            var et = EditTopo.EditClimber(this.listID, Convert.ToInt32(lblIID.Text), cn, this.competitionTitle, this, competitorStopwatch);
            et.TopoClick += new EventHandler<TopoClickEventArgs>(et_TopoClick);
            et.ShowDialog(this);
        }

        void et_TopoClick(object sender, TopoClickEventArgs e)
        {
            tbRes.Invoke(new EventHandler(delegate
            {
                tbRes.Text = e.Result;
                if (!String.IsNullOrEmpty(e.Result))
                {
                    string sTmp;
                    if (useLeadTime && e.Time!=null)
                    {
                        tbTime.Text = e.Time;
                    }
                    else
                        tbTime.Text = String.Empty;
                    EnterRes(int.Parse(lblIID.Text), e.Result, tbTime.Text, false, out sTmp);

                    btnEdit.Invoke(new EventHandler(delegate
                    {
                        if (e.AutoSave && !btnEdit.Text.Equals("Старт", StringComparison.OrdinalIgnoreCase))
                            btnEdit_Click(btnEdit, null);
                    }));
                }
            }));
        }

        bool remain40sec = false;

        public event EventHandler<TopoClickEventArgs> TimerTopoTick;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (secElapsed == 0)
            {
                secElapsed = 59;
                minElapsed--;
            }
            else
                secElapsed--;
            lblTime.Invoke(new EventHandler(delegate
            {
                lblTime.Text = minElapsed.ToString("00") + ":" + secElapsed.ToString("00");

                if (TimerTopoTick != null)
                    TimerTopoTick(this, new TopoClickEventArgs(lblTime.Text));
            }));

            if ((minElapsed == 1) && (secElapsed == 5))
                MessageBox.Show("Осталась одна минута.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if ((minElapsed == 0) && (secElapsed == 0))
                {
                    timer.Enabled = false;
                    MessageBox.Show("Время истекло.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    if (this.remain40sec && (minElapsed == (climbingTime - 1)) && (secElapsed == 30))
                        MessageBox.Show("40 секунд прошло. Надо стартовать.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgRes_DoubleClick(object sender, EventArgs e)
        {
            if (allowEvent)
            {
                try
                {
                    //lblStart.Text = dgRes.CurrentRow.Cells[8].Value.ToString();
                    lblIID.Text = dgRes.CurrentRow.Cells["iid"].Value.ToString();
                    try
                    {
                        int iid = Convert.ToInt32(lblIID.Text);
                        SqlCommand cmd = new SqlCommand("SELECT start FROM routeResults WHERE (" +
                            "list_id = " + listID.ToString() + ") AND (" +
                            "climber_id = " + iid.ToString() + ")", cn);
                        lblStart.Text = cmd.ExecuteScalar().ToString();
                    }
                    catch { lblStart.Text = ""; }
                    lblName.Text = dgRes.CurrentRow.Cells[2].Value.ToString();
                    lblAge.Text = dgRes.CurrentRow.Cells[3].Value.ToString();
                    lblQf.Text = dgRes.CurrentRow.Cells[4].Value.ToString();
                    lblTeam.Text = dgRes.CurrentRow.Cells[5].Value.ToString();
                    tbRes.Text = dgRes.CurrentRow.Cells[6].Value.ToString();
                    lblPos.Text = dgRes.CurrentRow.Cells[0].Value.ToString();
                    if (useLeadTime)
                        tbTime.Text = dgRes.CurrentRow.Cells[GetSortable(TIMERES)].Value.ToString();
                    needToErase = false;
                    SetTransactionButtons();
                }
                catch { lblTeam.Text = lblStart.Text = lblQf.Text = lblName.Text = lblIID.Text = lblAge.Text = ""; }
            }
        }

        private string backupTableName = null;
        private Random rand = new Random();
        private void backupTable(SqlTransaction tran)
        {
            String sBackupTable;
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = ((tran == null) ? cn.BeginTransaction() : tran)
            };
            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM tempdb..sysobjects(nolock) WHERE name=@name and type='U'";
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
                int n;
                do
                {
                    sBackupTable = String.Format("##LEAD{0}", rand.Next());
                    cmd.Parameters["@name"].Value = sBackupTable;
                    n = Convert.ToInt32(cmd.ExecuteScalar());
                } while (n > 0);
                cmd.CommandText = String.Format("SELECT * INTO {0} FROM routeResults(nolock) WHERE list_id={1}", sBackupTable, listID);
                cmd.ExecuteNonQuery();
                if (tran == null)
                    cmd.Transaction.Commit();
                backupTableName = sBackupTable;
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        private void RestoreTable(SqlTransaction tran)
        {
            if (backupTableName == null)
                return;
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = ((tran == null) ? cn.BeginTransaction() : tran) };
            try
            {
                cmd.CommandText = @"
UPDATE routeResults
   SET resText = BK.resText,
       res = BK.res,
       timeText = BK.timeText,
       timeValue = BK.timeValue,
       posText = BK.posText,
       pos = BK.pos,
       qf = BK.qf,
       pts = BK.pts,
       ptsText = BK.ptsText
  FROM routeResults R(nolock)
  JOIN " + backupTableName + " BK(NOLOCK) ON BK.iid = R.iid";
                
                cmd.ExecuteNonQuery();
                ClearBackup(cmd.Transaction);
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        private void ClearBackup(SqlTransaction tran)
        {
            if (backupTableName == null)
                return;
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                Transaction = ((tran == null) ? cn.BeginTransaction() : tran)
            };
            try
            {
                cmd.CommandText = String.Format("DROP TABLE {0}", backupTableName);
                cmd.ExecuteNonQuery();
                backupTableName = null;
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        private void xlExport_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                const int frst_row = 6;

                bool isQf = false;
                foreach (DataRow dataRow in dtRes.Rows)
                    if (dataRow[GetSortable(NEXTROUND)].ToString().Equals("Q", StringComparison.OrdinalIgnoreCase))
                    {
                        isQf = true;
                        break;
                    }

                int i = 0;

                foreach (DataColumn cl in dtRes.Columns)
                {
                    if (cl.ColumnName.Equals("iid", StringComparison.OrdinalIgnoreCase) ||
                        (!hasTime && cl.ColumnName.Equals(GetSortable(TIMERES), StringComparison.OrdinalIgnoreCase)))
                        continue;
                    if ((isQf) || (!cl.ColumnName.Equals(GetSortable(NEXTROUND), StringComparison.OrdinalIgnoreCase)))
                    {
                        i++;
                        ws.Cells[frst_row, i] = GetInitialValue(cl.ColumnName);
                    }
                }

                i = frst_row;
                int lastFin = -1;
                foreach (DataRow rd in dtRes.Rows)
                {
                    i++;
                    int iSkip = 0;
                    for (int k = 1; k <= dtRes.Columns.Count - 1; k++)
                    {
                        if (iSkip == 0 && !hasTime && dtRes.Columns[k - 1].ColumnName.Equals(GetSortable(TIMERES), StringComparison.InvariantCultureIgnoreCase))
                        {
                            iSkip++;
                            continue;
                        }
                        ws.Cells[i, k - iSkip] = rd[k - 1];
                    }
                    //for (int k = 8; k <= dtRes.Columns.Count; k++)
                    //    ws.Cells[i, k] = rd[k - 1];
                    //ws.Cells[i, 7] = " " + rd[6].ToString();
                    if ((rd[GetSortable(AGE)].ToString() == "0"))
                        ws.Cells[i, 4] = "";
                    if ((rd[GetSortable(NEXTROUND)].ToString() != "Q") && (lastFin == -1) && (i > 5))
                        lastFin = i - 1;
                }
                Excel.Range dt;

                string cI, cH;
                cI = (hasTime ? "J" : "I");
                cH = (hasTime ? "I" : "H");


                if ((lastFin > -1) && isQf)
                {
                    dt = ws.get_Range("A" + frst_row, cI + lastFin.ToString());
                    dt.Style = "MyStyle";
                    dt = ws.get_Range("A" + (lastFin + 1).ToString(), cH + i.ToString());
                    dt.Style = "MyStyle";
                    dt = ws.get_Range("A" + frst_row, cI + lastFin.ToString());
                }
                else
                {
                    dt = ws.get_Range("A" + frst_row, cH + i.ToString());
                    dt.Style = "MyStyle";
                }
                dt = ws.get_Range("A" + frst_row, cI + i.ToString());
                dt.Columns.AutoFit();

                dt = ws.get_Range("C" + frst_row, "C" + i.ToString());
                dt.Style = "StyleLA";


                string cTmp;
                if (isQf)
                    cTmp = cI;
                else
                    cTmp = cH;

                dt = ws.get_Range("A1", cTmp + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                string strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];

                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];
                dt = ws.get_Range(cTmp + "2", cTmp + "2");
                dt.Style = "StyleRA";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];
                if (judgeID > 0)
                    ws.Cells[5, 1] = ROUTEJUDGE + ": " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[5, 1] = ROUTEJUDGE + ":";

                dt = ws.get_Range("A3", cTmp + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                if (bSfin)
                    strTmp = BOULDERNAME + ". " + gName;
                else
                    strTmp = LEADNAME + ". " + gName;
                dt.Cells[1, 1] = strTmp;

                strTmp = RESULTLIST + ". " + round;
                dt = ws.get_Range("A4", cTmp + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "рез_";
                if (bSfin)
                    strTmp += "Б_";
                else
                    strTmp += "ТР_";

                try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, round); }
                catch { }

                StaticClass.ExcelUnderlineQf(ws);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private static long? parseTime(ref string timeString)
        {
            if (String.IsNullOrEmpty(timeString))
            {
                timeString = String.Empty;
                return null;
            }
            long min, sec;
            for (int i = 0; i < timeString.Length; i++)
                if (!(timeString[i].Equals(' ') || Char.IsDigit(timeString, i)))
                    timeString = timeString.Replace(timeString[i], ' ');
            while (timeString.IndexOf("  ") > -1)
                timeString = timeString.Replace("  ", " ");
            List<long> intList = new List<long>();
            timeString = timeString.Trim();
            while (!String.IsNullOrEmpty(timeString))
            {
                int nTmp = timeString.IndexOf(' ');
                if (nTmp < 0)
                {
                    intList.Add(long.Parse(timeString));
                    break;
                }
                else
                {
                    intList.Add(long.Parse(timeString.Substring(0, nTmp)));
                    timeString = timeString.Substring(nTmp).Trim();
                }
            }
            if (intList.Count < 1)
            {
                timeString = String.Empty;
                return null;
            }
            if (intList.Count < 2)
            {
                min = 0;
                sec = intList[0];
            }
            else
            {
                min = intList[0];
                sec = intList[1];
            }
            long res = min * 60 + sec;
            min = res / 60;
            sec = res % 60;
            timeString = min.ToString("00") + ":" + sec.ToString("00");
            return res;
        }

        private void btnNxtRound_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            //NewStartList nl;
            if (cbRound.SelectedIndex < 0)
            {
                MessageBox.Show("Раунд не выбран");
                return;
            }
            int nextID;
            bool b = false;
            SqlCommand cmd = new SqlCommand("SELECT l.iid FROM lists l, lists l1 WHERE " +
                "(l.style=l1.style) AND (l.group_id=l1.group_id) AND (l1.iid=" + listID.ToString() +
                ") AND (l.round = '" + cbRound.SelectedItem.ToString() + "')", cn);
            try { nextID = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { nextID = 0; }
            if (nextID > 0)
            {
                DialogResult dgr = MessageBox.Show("Такой протокол уже существует.\nЗаменить его?",
                    "Протокол существует", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dgr == DialogResult.No)
                    return;
                b = true;
            }
            else
                nextID = (int)StaticClass.GetNextIID("lists", cn, "iid", null);
            List<StartListMember> starters = new List<StartListMember>();
            cmd.CommandText = "SELECT r.climber_id, r.pos, p.rankingLead, r.start FROM " +
                "routeResults r(NOLOCK) JOIN Participants p(NOLOCK) ON r.climber_id=p.iid " +
                "WHERE (r.qf='Q' OR r.preQf = 1) AND (r.list_id=@i) ORDER BY r.start";
            cmd.Parameters.Add("@i", SqlDbType.Int);
            cmd.Parameters[0].Value = listID;
            SqlDataReader rd = cmd.ExecuteReader();
            Random rnd = new Random();
            List<Starter> data = new List<Starter>();
            while (rd.Read())
            {
                StartListMember stm = new StartListMember();
                stm.iid = Convert.ToInt32(rd[0]);
                try { stm.ranking = Convert.ToInt32(rd[2]); }
                catch { stm.ranking = 9999; }
                stm.rndDouble = rnd.NextDouble();
                stm.prevPos = Convert.ToInt32(rd[1]);
                starters.Add(stm);
                Starter strt = new Starter();
                strt.iid = stm.iid;
                strt.lateAppl = false;
                strt.prevPos = stm.prevPos;
                try { strt.prevStart = Convert.ToInt32(rd[3]); }
                catch { strt.prevStart = 0; }
                strt.random = stm.rndDouble;
                strt.ranking = stm.ranking;
                data.Add(strt);
            }
            rd.Close();

            cmd.CommandText = "SELECT g.name, g.iid FROM lists l(NOLOCK) JOIN Groups g(NOLOCK) ON l.group_id=g.iid WHERE l.iid=@i";
            rd = cmd.ExecuteReader();
            int group_id = 0;
            string group_name = "";
            while (rd.Read())
            {
                group_id = Convert.ToInt32(rd[1]);
                group_name = rd[0].ToString();
                break;
            }
            rd.Close();
            //cmd.Transaction = cn.BeginTransaction();
            try
            {
                cmd.Transaction = cn.BeginTransaction();
                cmd.Parameters[0].Value = nextID;//param_name="@i"
                cmd.Parameters.Add("@g", SqlDbType.Int);//cmd.Parameters[1]
                cmd.Parameters[1].Value = group_id;
                if (b)
                {
                    cmd.CommandText = "DELETE FROM routeResults WHERE list_id = @i";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE lists SET next_round=NULL, prev_round=NULL " +
                        "WHERE iid=@i";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,listType) VALUES (@i,@g,@s,@r,@ltp)";
                    
                    

                    cmd.Parameters.Add("@s", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = "Трудность";

                    cmd.Parameters.Add("@r", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = cbRound.SelectedItem.ToString();

                    cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                    cmd.Parameters[4].Value = ListTypeEnum.LeadSimple.ToString();

                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                cmd.Parameters[1].Value = listID;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                cmd.ExecuteNonQuery();

                //nl = new NewStartList(false, group_name, "Трудность", cbRound.SelectedItem.ToString());
                //nl.ShowDialog();
                Sorting sort = new Sorting(data, StartListMode.NotFirstRound);
                sort.ShowDialog();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                //if (nl.type == StartListType.Cancel)
                if(sort.Cancel)
                {
                    cmd.CommandText = "DELETE FROM lists WHERE iid = @i";
                    try
                    {
                        try { cmd.Transaction.Rollback(); }
                        catch
                        {
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "UPDATE lists SET next_round=NULL, prev_round=NULL " +
                                "WHERE iid=@i";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    //cmd.Transaction.Rollback();
                    return;
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE lists SET online=@o WHERE iid=" + nextID.ToString();
                cmd.Parameters.Add("@o", SqlDbType.Bit);
                cmd.Parameters[0].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);
                cmd.ExecuteNonQuery();
                //do
                //{
                //    switch (nl.type)
                //    {
                //        case StartListType.General:
                //            starters.Sort(new RandomListComparer());
                //            b = false;
                //            break;
                //        case StartListType.Reverse:
                //            starters.Sort(new NextRoundComparer());
                //            b = false;
                //            break;
                //        case StartListType.Cancel:
                //            cmd.Transaction.Rollback();
                //            return;
                //        case StartListType.SameOrder:
                //            b = false;
                //            break;
                //        default:
                //            MessageBox.Show("Тип жеребьёвки неподдерживается. Выберите другой тип.");
                //            nl.ShowDialog();
                //            b = true;
                //            break;
                //    }
                //} while (b);

                long nxtIID;
                cmd.CommandText = "SELECT MAX(iid) FROM routeResults(NOLOCK)";
                try { nxtIID = Convert.ToInt64(cmd.ExecuteScalar()); }
                catch { nxtIID = 1; }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@list_id", SqlDbType.Int);
                cmd.Parameters[0].Value = nextID;
                cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                cmd.Parameters.Add("@climber_id", SqlDbType.Int);
                cmd.Parameters.Add("@start", SqlDbType.Int);

                cmd.Parameters.Add("@p", SqlDbType.Int);
                cmd.Parameters[4].Value = int.MaxValue;
                
                cmd.CommandText = "INSERT INTO routeResults(iid, list_id, climber_id, start, pos) " +
                    "VALUES (@iid, @list_id, @climber_id, @start, @p)";
                int i = 0;
                foreach(Starter strte in sort.Starters)
                //for (int i = 0; i < starters.Count; i++)
                {
                    cmd.Parameters[1].Value = (++nxtIID);
                    cmd.Parameters[2].Value = strte.iid;
                    cmd.Parameters[3].Value = ++i;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                MessageBox.Show("Протокол создан. iid = " + nextID.ToString());
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                try { cmd.Transaction.Rollback(); }
                catch { }
                return;
            }
        }

        private void cbRound_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgRes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgRes_DoubleClick(sender, e);
        }

        private void ResultListLead_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE lists SET nowClimbing = NULL WHERE iid=" + listID.ToString();
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StaticClass.SetNowClimbingNull(cn, listID);
            base.OnClosing(e);
        }

        private static bool ClearClimber(int iid, int listID, SqlConnection cn, SqlTransaction tran)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = tran;
                string cName = StaticClass.GetClimberName(iid.ToString(), cn);
                if (cName.Length < 1)
                {
                    MessageBox.Show("Ошибка извлечения данных участника\r\n");
                    return false;
                }
                if (MessageBox.Show("Вы действительно хотите удалить результат участника " + cName + "?",
                    "Удаление результата", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                    DialogResult.No)
                    return false;
                cmd.CommandText = "UPDATE routeResults SET resText = NULL, res = NULL, posText = NULL, " +
                    "pos = " + int.MaxValue.ToString() + ", qf = NULL, pts = NULL, ptsText = NULL " +
                    "WHERE list_id = " + listID.ToString() + " AND climber_id = " + iid.ToString();
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET nowClimbing = NULL WHERE iid = " + listID.ToString();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления результата\r\n" + ex.Message);
                return false;
            }
        
        }

        private string prevRes = "";

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            btnEdit.Text = "Старт";
            tbRes.ReadOnly = true;
            tbTime.ReadOnly = true;
            timer.Enabled = false;
            allowEvent = true;
            if (climbingTime > 0)
                lblTime.Text = climbingTime.ToString("00") + ":00";
            tbRes.Text = prevRes;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            var tran = cn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE lists SET nowClimbing = NULL WHERE iid = " + listID.ToString(), cn);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();

                RestoreTable(tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
            }
            try { RefreshData(); }
            catch { }
        }

        private void lblStart_TextChanged(object sender, EventArgs e)
        {
            if (climbingTime > 0)
                lblTime.Invoke(new EventHandler(delegate { lblTime.Text = climbingTime.ToString("00") + ":00"; }));
        }

        private void ResultListLead_Load(object sender, EventArgs e)
        {
            if (needToClose)
                try { this.Close(); }
                catch { }
        }

        private void btnRollBack_Click(object sender, EventArgs e)
        {
            long? activeTranID, nextTranID, toCheck;
            try
            {
                SetTransactionButtons(out activeTranID, out nextTranID);
                toCheck = (sender == btnRollBack) ? activeTranID : nextTranID;
                string message = (sender == btnRollBack) ? "откатить" : "вернуть";
                if (toCheck == null || !toCheck.HasValue)
                {
                    MessageBox.Show(this, "Невозможно " + message + " результат");
                    return;
                }
                if (MessageBox.Show(this, "Вы уверены, что хотите " + message + " результат текущего участника (" +
                    lblName.Text + ")?", message + " результат?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    return;
                tran = cn.BeginTransaction();
                try
                {
                    bool res;
                    if (sender == btnRollBack)
                        res = StaticClass.RollbackTransaction(toCheck.Value, cn, tran);
                    else
                        res = StaticClass.RestoreLogicTransaction(toCheck.Value, cn, tran);
                    if (res)
                    {
                        GetResultList();
                        tran.Commit();
                        RefreshData();
                    }
                    else
                    {
                        MessageBox.Show("Выполнение команды завершилось внутренней ошибкой SQL Server");
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка выполнения команды:\r\n" + ex.Message); }
        }

        private void btnEditTopo_Click(object sender, EventArgs e)
        {
            if (btnEditTopo.Text.Equals("Настройка схемы", StringComparison.OrdinalIgnoreCase))
                EditTopo.CreateTopo(this.listID, this.cn, this.competitionTitle).ShowDialog(this);
            else
                OpenTopoRes();
        }

#endif
    }
}