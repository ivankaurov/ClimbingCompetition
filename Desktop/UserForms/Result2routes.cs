using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using XmlApiData;

namespace ClimbingCompetition
{
    /// <summary>
    /// Сведенеие 2х трасс трудности и боулдеринга
    /// </summary>
    public partial class Result2routes
#if FULL
        : BaseForm
#endif
    {
#if FULL
        string gName = "", round = "";
        //SqlConnection cn;
        int listID, judgeID;
        DataTable dt;
        static bool boulder;
        public Result2routes(string competitionTitle, SqlConnection baseCon, int listID)
            : base(baseCon, competitionTitle)
        {
            InitializeComponent();
            //this.cn = cn;
            this.listID = listID;
            RefreshData();

            //SqlCommand cmd = new SqlCommand("SELECT g.name,l.round, l.judge_id,l.usePts FROM groups g(NOLOCK) JOIN lists l(NOLOCK) ON g.iid = l.group_id WHERE l.iid = " + listID.ToString(), this.cn);
            SqlCommand cmd = new SqlCommand("SELECT g.name,l.round, l.judge_id,l.style FROM groups g(NOLOCK) JOIN lists l(NOLOCK) ON g.iid = l.group_id WHERE l.iid = " + listID.ToString(), this.cn);
            SqlDataReader rd = cmd.ExecuteReader();
            //bool usePts = rbPts.Checked;
            try
            {
                if (rd.Read())
                {
                    gName = rd[0].ToString();
                    round = rd[1].ToString();
                    try { judgeID = Convert.ToInt32(rd[2]); }
                    catch { judgeID = 0; }
                    try { cbRound.Visible = (rd[3].ToString().ToLower().IndexOf("боулд") < 0); }
                    catch { }
                    /*try { usePts = Convert.ToBoolean(rd["usePts"]); }
                    catch { }*/
                }
            }
            finally { rd.Close(); }
            if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                try { btnNxtRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                catch { btnNxtRound.Enabled = true; }
            }
            else
                btnNxtRound.Enabled = cbRound.Enabled = false;
            /*if (usePts != rbPts.Checked)
                try
                {
                    rbPts.Checked = usePts;
                    rbPl.Checked = !usePts;
                }
                catch { }*/
            this.Text = StaticClass.GetListName(listID, cn);
        }

        private void RefreshData()
        {
            try
            {
                dt = StaticClass.FillResOnsight(this.listID, this.cn, false);
                boulder = StaticClass.boulder;
                dt.Columns.Remove("posT");
                dt.Columns.Remove("pos");
                if (dt.Columns.IndexOf("posText1") > -1)
                    dt.Columns.Remove("posText1");

                dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
                dg.DataSource = dt;
            }
            catch { }
        }

        private void xlExport_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;
            char total;

            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                int i = 0;
                int cnt = 0;
                int startRow = 6;
                foreach (DataRow rd in dt.Rows)
                {
                    if (rd["Кв."].ToString() == "Q")
                        cnt++;
                    else
                        break;
                }
                if (boulder)
                    startRow++;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    i++;
                    DataColumn cl = dt.Columns[j];
                    if ((cl.ColumnName.IndexOf("T_") > -1))
                        ws.Cells[startRow, i] = "Тр.";
                    else
                    {
                        if ((cl.ColumnName.IndexOf("B_") > -1))
                            ws.Cells[startRow, i] = "Бон.";
                        else
                        {
                            if ((cl.ColumnName.IndexOf("a_") > -1))
                                ws.Cells[startRow, i] = "Поп.";
                            else
                            {
                                if (cl.ColumnName == "№" || cl.ColumnName.IndexOf("nya") > -1 || cl.ColumnName.IndexOf("disq") > -1)
                                {
                                    i--;
                                    continue;
                                }
                                if ((cl.ColumnName == "Кв.") && (cnt == 0))
                                    continue;
                                else
                                {
                                    if (cl.ColumnName == "ФамилияИмя")
                                        ws.Cells[startRow, i] = "Фамилия, Имя";
                                    else
                                        ws.Cells[startRow, i] = cl.ColumnName;
                                }
                            }
                        }
                    }
                }
                Excel.Range dtR;
                i = startRow;
                int lastFin = -1;
                foreach (DataRow rd in dt.Rows)
                {
                    i++;
                    char[] qaz = { 'T', 'B' };
                    int k = 1, j = 0;
                    while (k <= dt.Columns.Count)
                    {
                        if (dt.Columns[k - 1].ColumnName == "№")
                        {
                            j = 1;
                            k++;
                            continue;
                        }
                        if (dt.Columns[k - 1].ColumnName.IndexOf("nya") > -1 ||
                            dt.Columns[k - 1].ColumnName.IndexOf("disq") > -1)
                        {
                            j++;
                            bool b;
                            try { b = Convert.ToInt32(rd[k - 1]) > 0; }
                            catch { b = false; }
                            if (b)
                            {
                                dtR = ws.get_Range(ws.Cells[i, k - j + 1], ws.Cells[i, k - j + 4]);
                                dtR.Merge(Type.Missing);
                                if (dt.Columns[k - 1].ColumnName.IndexOf("nya") > -1)
                                {
                                    dtR.Cells[1, 1] = "н/я";
                                    k += 5;
                                }
                                else
                                {
                                    dtR.Cells[1, 1] = "дискв.";
                                    k += 6;
                                }
                            }
                            else
                                k++;
                            continue;
                        }
                        //if ((dt.Columns[k - 1].ColumnName.IndexOfAny(qaz) > -1) && (rd[k - 1].ToString() == "0"))
                        //    ws.Cells[i, k - j] = "-";
                        //else
                        ws.Cells[i, k - j] = rd[k - 1];
                        k++;
                    }
                    if ((rd["Г.р."].ToString() == "0"))
                        ws.Cells[i, 4 - j] = "";
                    if (rd["Кв."].ToString() == "Q")
                        lastFin = i;
                }
                if (boulder)
                {
                    if (lastFin > -1)
                    {
                        dtR = ws.get_Range("A" + (startRow - 1).ToString(), "N" + lastFin.ToString());
                        dtR.Style = "MyStyle";
                        if (i != lastFin)
                            dtR = ws.get_Range("A" + (lastFin + 1).ToString(), "M" + i.ToString());
                        dtR.Style = "MyStyle";
                    }
                    else
                    {
                        dtR = ws.get_Range("A" + (startRow - 1).ToString(), "M" + i.ToString());
                        dtR.Style = "MyStyle";
                    }
                    dtR = ws.get_Range("A" + (startRow - 1).ToString(), "N" + i.ToString());
                }
                else
                {
                    if (lastFin > -1)
                    {
                        dtR = ws.get_Range("A" + startRow.ToString(), "H" + lastFin.ToString());
                        dtR.Style = "MyStyle";
                        if (i != lastFin)
                            dtR = ws.get_Range("A" + (lastFin + 1).ToString(), "G" + i.ToString());
                        dtR.Style = "MyStyle";
                    }
                    else
                    {
                        dtR = ws.get_Range("A" + startRow.ToString(), "G" + i.ToString());
                        dtR.Style = "MyStyle";
                    }


                    dtR = ws.get_Range("A" + startRow.ToString(), "H" + i.ToString());
                }
                dtR.Columns.AutoFit();

                if (boulder)
                {
                    if (cnt == 0)
                        total = 'M';
                    else
                        total = 'N';
                }
                else
                {
                    if (cnt == 0)
                        total = 'G';
                    else
                        total = 'H';
                }

                dtR = ws.get_Range("B" + startRow.ToString(), "B" + i.ToString());
                dtR.Style = "StyleLA";

                dtR = ws.get_Range("A1", total + "1");
                dtR.Style = "CompTitle";
                dtR.Merge(Type.Missing);
                string strTmp = "";
                dtR.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];

                if (judgeID > 0)
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду:";

                dtR = ws.get_Range(total + "2", total + "2");
                dtR.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];

                dtR.Style = "StyleRA";

                dtR = ws.get_Range("A3", total + "3");
                dtR.Style = "Title";
                dtR.Merge(Type.Missing);
                if (boulder)
                    strTmp = "Боулдеринг. " + gName;
                else
                    strTmp = "Трудность. " + gName;
                dtR.Cells[1, 1] = strTmp;

                strTmp = "Протокол результатов. " + round;
                dtR = ws.get_Range("A4", total + "4");
                dtR.Style = "Title";
                dtR.Merge(Type.Missing);
                dtR.Cells[1, 1] = strTmp;
                strTmp = "рез_";
                if (boulder)
                    strTmp += "Б_";
                else
                    strTmp += "ТР_";

                strTmp += StaticClass.CreateSheetName(gName, round);

                strTmp = strTmp.Replace(' ', '_');
                try { ws.Name = strTmp; }
                catch { }

                if (boulder)
                {
                    dtR = ws.get_Range("F" + (startRow - 1).ToString(), "I" + (startRow - 1).ToString());
                    dtR.Cells[1, 1] = "Группа А";
                    dtR.Merge(Type.Missing);
                    dtR.Style = "MyStyle";
                    dtR = ws.get_Range("J" + (startRow - 1).ToString(), "M" + (startRow - 1).ToString());
                    dtR.Cells[1, 1] = "Группа Б";
                    dtR.Merge(Type.Missing);
                    dtR.Style = "MyStyle";
                    for (char ct = 'A'; ct != 'F'; ct++)
                    {
                        dtR = ws.get_Range(ct + (startRow - 1).ToString(), ct + startRow.ToString());
                        dtR.Merge(Type.Missing);
                    }
                    if (total == 'N')
                    {
                        dtR = ws.get_Range(total + (startRow - 1).ToString(), total + startRow.ToString());
                        dtR.Merge(Type.Missing);
                    }
                }
                StaticClass.ExcelUnderlineQf(ws);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }
        }

        private void btnNxtRound_Click(object sender, EventArgs e)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            //NewStartList nl;
            string nxtRound;
            int routeN;
            if (boulder)
            {
                if (NextRoundForm.GetNextRoundData(this.round, this, out nxtRound, out routeN))
                    return;
            }
            else
            {
                if (cbRound.SelectedIndex < 0 || cbRound.SelectedItem == null)
                {
                    MessageBox.Show("Раунд не выбран");
                    return;
                }
                routeN = -1;
                nxtRound = cbRound.SelectedItem.ToString();
            }
            int nextID;
            //bool b;
            SqlCommand cmd = new SqlCommand("SELECT l.iid FROM lists l, lists l1 WHERE " +
                "(l.style=l1.style) AND (l.group_id=l1.group_id) AND (l1.iid=" + listID.ToString() +
                ") AND (l.round = '" + nxtRound + "')", cn);
            try { nextID = Convert.ToInt32(cmd.ExecuteScalar()); }
            catch { nextID = 0; }
            if (nextID > 0)
            {
                DialogResult dgr = MessageBox.Show("Такой протокол уже существует.\nЗаменить его?",
                    "Протокол существует", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dgr == DialogResult.No)
                    return;
                cmd.CommandText = "DELETE FROM lists WHERE iid=" + nextID.ToString();
                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
                nextID = (int)StaticClass.GetNextIID("lists", cn, "iid", null);
            List<StartListMember> starters = new List<StartListMember>();
            DataTable dtTmp = new DataTable();
            Random rnd = new Random();
            SqlDataAdapter da = new SqlDataAdapter();
            if (boulder)
                da.SelectCommand = new SqlCommand("SELECT r.climber_id,r.pos,p.rankingBoulder AS rank, r.start FROM " +
                    "boulderResults r(NOLOCK) JOIN lists l(NOLOCK) ON l.iid = r.list_id JOIN Participants p(NOLOCK) ON " +
                    "r.climber_id = p.iid JOIN lists l2(NOLOCK) ON (l.group_id=l2.group_id) AND (l.style=l2.style) " +
                    "WHERE ((l.round = 'Квалификация Группа А') OR (l.round = 'Квалификация Группа Б')) AND " +
                    "(l2.iid = " + listID.ToString() + ") AND (r.qf = 'Q' OR r.preQf = 1) ORDER BY r.start", cn);
            else
                da.SelectCommand = new SqlCommand("SELECT r.climber_id,r.pos,p.rankingLead AS rank, r.start FROM " +
                    "routeResults r(NOLOCK) JOIN lists l(NOLOCK) ON l.iid = r.list_id JOIN Participants p(NOLOCK) ON " +
                    "r.climber_id = p.iid JOIN lists l2(NOLOCK) ON (l.group_id=l2.group_id) AND (l.style=l2.style) " +
                    "WHERE ((l.round = '1/4 финала Трасса 1') OR (l.round = '1/4 финала Трасса 2')) AND " +
                    "(l2.iid = " + listID.ToString() + ") AND (r.qf = 'Q' OR r.preQf = 1) ORDER BY r.start", cn);
            da.Fill(dtTmp);

            List<Starter> data = new List<Starter>();

            foreach (DataRow rdd in dtTmp.Rows)
            {
                StartListMember stm = new StartListMember();
                Starter strt = new Starter();
                stm.iid = Convert.ToInt32(rdd["climber_id"]);
                stm.prevPos = Convert.ToInt32(rdd["pos"]);
                try { stm.ranking = Convert.ToInt32(rdd["rank"]); }
                catch { stm.ranking = int.MaxValue; }
                stm.rndDouble = rnd.NextDouble();
                starters.Add(stm);

                strt.iid = stm.iid;
                strt.lateAppl = false;
                strt.prevPos = stm.prevPos;
                try { strt.prevStart = Convert.ToInt32(rdd["start"]); }
                catch { strt.prevStart = 0; }
                strt.random = stm.rndDouble;
                strt.ranking = stm.ranking;
                data.Add(strt);
                //if (rdd["Кв."].ToString() == "Q")
                //{
                //    StartListMember stm;
                //    stm.iid = Convert.ToInt32(rdd["№"]);
                //    stm.prevPos = Convert.ToInt32(rdd["pos"]);
                //    //if (rdd["Трасса 1"].ToString() == "")
                //    //    stm.prevPos += 2000;
                //    //else
                //    //    stm.prevPos += 1000;
                //    stm.ranking = 9999;
                //    stm.rndDouble = rnd.NextDouble();
                //    stNum.Add(stm);
                //}
                //else
                //    break;
            }

            cmd.Parameters.Add("@i", SqlDbType.Int);
            cmd.Parameters[0].Value = listID;
            //foreach (StartListMember stm in stNum)
            //{
            //    StartListMember stmQ;
            //    stmQ.iid = stm.iid;
            //    stmQ.prevPos = stm.prevPos;
            //    cmd.CommandText = "SELECT rankingLead FROM Participants WHERE iid = " + stm.iid.ToString();
            //    try { stmQ.ranking = Convert.ToInt32(cmd.ExecuteScalar()); }
            //    catch { stmQ.ranking = 9999; }
            //    stmQ.rndDouble = stm.rndDouble;
            //    starters.Add(stmQ);
            //}

            cmd.CommandText = "SELECT g.name, g.iid FROM lists l INNER JOIN Groups g ON l.group_id=g.iid WHERE l.iid=@i";
            SqlDataReader rd = cmd.ExecuteReader();
            int group_id = 0;
            string group_name = "";
            while (rd.Read())
            {
                group_id = Convert.ToInt32(rd[1]);
                group_name = rd[0].ToString();
                break;
            }
            rd.Close();
            //int nextID = (int)StaticMethods.GetNextIID("lists", cn);

            try
            {
                cmd.Transaction = cn.BeginTransaction();
                cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round,online,routeNumber,listType) VALUES (@i,@g,@s,@r,@o,@rn,@ltp)";
                cmd.Parameters[0].Value = nextID;

                cmd.Parameters.Add("@g", SqlDbType.Int);
                cmd.Parameters[1].Value = group_id;

                cmd.Parameters.Add("@s", SqlDbType.VarChar);
                if (boulder)
                    cmd.Parameters[2].Value = "Боулдеринг";
                else
                    cmd.Parameters[2].Value = "Трудность";

                cmd.Parameters.Add("@r", SqlDbType.VarChar);
                cmd.Parameters[3].Value = nxtRound;

                cmd.Parameters.Add("@o", SqlDbType.Bit);
                cmd.Parameters[4].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);

                cmd.Parameters.Add("@rn", SqlDbType.Int);
                cmd.Parameters[5].Value = (routeN > 0 ? routeN : (object)DBNull.Value);

                cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                cmd.Parameters[6].Value = ((boulder && nxtRound.ToLower() != "суперфинал") ? ListTypeEnum.BoulderSimple : ListTypeEnum.LeadSimple).ToString();

                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                cmd.Parameters[1].Value = listID;
                cmd.ExecuteNonQuery();

                cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                cmd.ExecuteNonQuery();

                //if (boulder)
                //    nl = new NewStartList(false, group_name, "Боулдеринг", cbRound.SelectedItem.ToString());
                //else
                //    nl = new NewStartList(false, group_name, "Трудность", cbRound.SelectedItem.ToString());
                //nl.ShowDialog();
                Sorting sorting = new Sorting(data, StartListMode.NotFirstRound);
                sorting.ShowDialog();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                if (sorting.Cancel)
                //if (nl.type == StartListType.Cancel)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch
                    {
                        cmd.CommandText = "DELETE FROM lists WHERE iid = " + nextID.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    return;
                }
                #region OLD
                /* //bool b;
                do
                {
                    switch (nl.type)
                    {
                        case StartListType.RankBased:
                            starters.Sort(new RankingComparer());
                            starters.Reverse();
                            b = false;
                            break;
                        case StartListType.General:
                            starters.Sort(new RandomListComparer());
                            b = false;
                            break;
                        case StartListType.Reverse:
                            starters.Sort(new NextRoundComparer());
                            b = false;
                            break;
                        default:
                            MessageBox.Show("Тип жеребьёвки неподдерживается. Выберите другой тип.");
                            nl.ShowDialog();
                            b = true;
                            break;
                    }
                } while (b);*/
                #endregion
                long nxtIID;
                string taleName;
                if (boulder)
                    taleName = "boulderResults";
                else
                    taleName = "routeResults";
                cmd.CommandText = "SELECT MAX(iid) FROM " + taleName;
                try { nxtIID = Convert.ToInt64(cmd.ExecuteScalar()); }
                catch { nxtIID = 1; }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@list_id", SqlDbType.Int);
                cmd.Parameters[0].Value = nextID;
                cmd.Parameters.Add("@iid", SqlDbType.BigInt);
                cmd.Parameters.Add("@climber_id", SqlDbType.Int);
                cmd.Parameters.Add("@start", SqlDbType.Int);

                cmd.Parameters.Add("@pos", SqlDbType.Int);
                cmd.Parameters[4].Value = int.MaxValue;

                cmd.CommandText = "INSERT INTO " + taleName + "(iid, list_id, climber_id, start, pos) " +
                    "VALUES (@iid, @list_id, @climber_id, @start, @pos)";
                int i = 0;
                foreach (Starter stss in sorting.Starters)
                //for (int i = 0; i < sorting.Starters.Count; i++)
                {
                    cmd.Parameters[1].Value = (++nxtIID);
                    cmd.Parameters[2].Value = /*((StartListMember)starters[i]).iid*/stss.iid;
                    cmd.Parameters[3].Value = ++i;
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Протокол создан. iid = " + nextID.ToString());
                cmd.Transaction.Commit();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                try { cmd.Transaction.Rollback(); }
                catch
                {
                    cmd.CommandText = "DELETE FROM lists WHERE iid=" + nextID.ToString();
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                }
                //cmd.Transaction.Rollback();
                return;
            }
        }

        private void rbPl_CheckedChanged(object sender, EventArgs e)
        {
            SaveUsePlcState();
        }

        bool inEvent = false;
        private void SaveUsePlcState()
        {
            if (inEvent)
                return;
            inEvent = true;
            try
            {
                if (MessageBox.Show(this, "Данное действие затронет все протоколы. Продолжить?", "", MessageBoxButtons.YesNo,
                     MessageBoxIcon.Warning) == DialogResult.No)
                {
                    rbPl.Checked = !rbPl.Checked;
                    rbPts.Checked = !rbPl.Checked;
                    return;
                }
                SettingsForm.SetOSQfMode(rbPl.Checked ? SettingsForm.OSQfMode.PLC : SettingsForm.OSQfMode.PTS, cn);
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения данных.\r\n" + ex.Message);
            }
            finally { inEvent = false; }
        }

        private void Result2routes_Load(object sender, EventArgs e)
        {
            inEvent = true;
            try
            {
                rbPl.Checked = (SettingsForm.GetOSQfMode(cn) == SettingsForm.OSQfMode.PLC);
                rbPts.Checked = !rbPl.Checked;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message); }
            finally { inEvent = false; }
        }

#endif
    }
}