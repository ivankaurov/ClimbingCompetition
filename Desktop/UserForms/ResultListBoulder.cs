// <copyright file="ResultListBoulder.cs">
// Copyright В© 2016 All Rights Reserved
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
// (Р­С‚РѕС‚ С„Р°Р№Р» вЂ” С‡Р°СЃС‚СЊ ClimbingCompetition.
// 
// ClimbingCompetition - СЃРІРѕР±РѕРґРЅР°СЏ РїСЂРѕРіСЂР°РјРјР°: РІС‹ РјРѕР¶РµС‚Рµ РїРµСЂРµСЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏС‚СЊ РµРµ Рё/РёР»Рё
// РёР·РјРµРЅСЏС‚СЊ РµРµ РЅР° СѓСЃР»РѕРІРёСЏС… РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU РІ С‚РѕРј РІРёРґРµ,
// РІ РєР°РєРѕРј РѕРЅР° Р±С‹Р»Р° РѕРїСѓР±Р»РёРєРѕРІР°РЅР° Р¤РѕРЅРґРѕРј СЃРІРѕР±РѕРґРЅРѕРіРѕ РїСЂРѕРіСЂР°РјРјРЅРѕРіРѕ РѕР±РµСЃРїРµС‡РµРЅРёСЏ;
// Р»РёР±Рѕ РІРµСЂСЃРёРё 3 Р»РёС†РµРЅР·РёРё, Р»РёР±Рѕ (РїРѕ РІР°С€РµРјСѓ РІС‹Р±РѕСЂСѓ) Р»СЋР±РѕР№ Р±РѕР»РµРµ РїРѕР·РґРЅРµР№
// РІРµСЂСЃРёРё.
// 
// ClimbingCompetition СЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏРµС‚СЃСЏ РІ РЅР°РґРµР¶РґРµ, С‡С‚Рѕ РѕРЅР° Р±СѓРґРµС‚ РїРѕР»РµР·РЅРѕР№,
// РЅРѕ Р‘Р•Р—Рћ Р’РЎРЇРљРРҐ Р“РђР РђРќРўРР™; РґР°Р¶Рµ Р±РµР· РЅРµСЏРІРЅРѕР№ РіР°СЂР°РЅС‚РёРё РўРћР’РђР РќРћР“Рћ Р’РР”Рђ
// РёР»Рё РџР РР“РћР”РќРћРЎРўР Р”Р›РЇ РћРџР Р•Р”Р•Р›Р•РќРќР«РҐ Р¦Р•Р›Р•Р™. РџРѕРґСЂРѕР±РЅРµРµ СЃРј. РІ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№
// РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU.
// 
// Р’С‹ РґРѕР»Р¶РЅС‹ Р±С‹Р»Рё РїРѕР»СѓС‡РёС‚СЊ РєРѕРїРёСЋ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU
// РІРјРµСЃС‚Рµ СЃ СЌС‚РѕР№ РїСЂРѕРіСЂР°РјРјРѕР№. Р•СЃР»Рё СЌС‚Рѕ РЅРµ С‚Р°Рє, СЃРј. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

#if FULL
#define MULTIBOULDER
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
#if DEBUG
using System.Media;
#endif
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.IO.Ports;
using UserForms;
using XmlApiData;
using ClimbingCompetition.SpeedData;

namespace ClimbingCompetition
{
    /// <summary>
    /// Ввод результатов боулдеринга
    /// </summary>
    public partial class ResultListBoulder
#if FULL
 : BaseListForm
#endif
    {
#if FULL
        Thread thr = null;
        SerialPort sp = null;
        string gName, round, path;
        //SqlConnection cn;
        int routeNumber, baseIID, climbingTime, minElapsed = 0, secElapsed = 0, frstRoute = 0, judgeID;
        bool baseTwoRoutes, lowChanged = false, highChanged = false;
#if DEBUG
        SoundPlayer oneMinute, getReady, rotate;
#endif
        DataTable dtStart = new DataTable(), dtRes = new DataTable();
        Mutex mStart = new Mutex(false);
        Mutex mRes = new Mutex(false);
        private bool isListening = false;

#if MULTIBOULDER
        private MultiRouteBoulder mrb;
#else
        SqlDataAdapter daStart, daRes, daPrint;
#endif

        private void dgRes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string dg;
            if (sender == dgRes)
                dg = "протоколе результатов";
            else
                dg = "стартовом протоколе";
            MessageBox.Show("Неверный ввод текста  в " + dg + ", в строке " + e.RowIndex.ToString() +
                ", столбце " + e.ColumnIndex.ToString() + ":\r\n" +
                e.Exception.Message);
        }

        public ResultListBoulder(SqlConnection baseCon, string competitionTitle, int listID, bool isPrevRound, string path)
            : base(baseCon, competitionTitle, listID)
        {
            InitializeComponent();

            tbCOMportSys.Text = appSettings.Default.SysPort;

            dgRes.DataError += new DataGridViewDataErrorEventHandler(dgRes_DataError);
            dgStart.DataError += new DataGridViewDataErrorEventHandler(dgRes_DataError);

            GetDataFromRegistry();
            this.path = path;
            try { StartList.ArrangeStartNumbers(listID, cn, null); }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка участников:\r\n" + ex.Message);
                needToClose = true;
            }
#if MULTIBOULDER
            mrb = new MultiRouteBoulder(this.listID, cn);
#endif
            try
            {
#if DEBUG

                try { oneMinute = new SoundPlayer(path + "Sound\\01-осталась одна минута.wav"); }
                catch { }
                try { getReady = new SoundPlayer(path + "Sound\\02-приготовиться к переходу.wav"); }
                catch { }
                try { rotate = new SoundPlayer(path + "Sound\\03-переход.wav"); }
                catch { }
#endif
                //btnNxtRound.Enabled = isPrevRound;
                //this.cn = cn;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = this.cn;
                cmd.CommandText = "SELECT g.name, lists.round, lists.climbingTime, lists.prev_round, lists.routeNumber, lists.judge_id FROM lists(NOLOCK) JOIN Groups g(NOLOCK)" +
                    " ON lists.group_id = g.iid WHERE lists.iid = @iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[0].Value = this.listID;
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    if (rdr.Read())
                    {
                        gName = rdr[0].ToString();

                        round = rdr[1].ToString();
                        this.Text = "Протокол результатов " + gName + " Боулдеринг " + round;

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

                        try { baseIID = Convert.ToInt32(rdr[3]); }
                        catch { baseIID = -1; }

                        try { routeNumber = Convert.ToInt32(rdr[4]); }
                        catch { routeNumber = 1; }

                        try { judgeID = Convert.ToInt32(rdr[5]); }
                        catch { judgeID = 0; }
                    }
                }
                finally { rdr.Close(); }


                if (baseIID > 0)
                {
                    cmd.CommandText = "SELECT round FROM lists(NOLOCK) WHERE iid = " + baseIID.ToString();
                    try
                    { baseTwoRoutes = (cmd.ExecuteScalar().ToString().IndexOf("2 трассы") >= 0); }
                    catch
                    { baseTwoRoutes = false; }
                }
                if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE prev_round = " + listID.ToString();
                    try { btnNxtRound.Enabled = Convert.ToInt32(cmd.ExecuteScalar()) < 1; }
                    catch { btnNxtRound.Enabled = true; }
                }
                else
                    btnNxtRound.Enabled = false;
#if !MULTIBOULDER
                //daStart = new SqlDataAdapter();
                //daStart.SelectCommand = new SqlCommand();
                //daStart.SelectCommand.Connection = this.cn;

                //daStart.SelectCommand.CommandText = "SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+" +
                //    "p.name) AS [Фамилия, Имя], p.age AS [Г.р.], p.qf AS [Разряд], t.name AS [Команда] " +
                //    "FROM boulderResults l INNER JOIN Participants p ON l.climber_id = p.iid INNER JOIN " +
                //    "Teams t ON p.team_id = t.iid WHERE (l.list_id = @listID) AND (top1 IS NULL) ORDER BY l.start";
                //daStart.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                //daStart.SelectCommand.Parameters[0].Value = this.listID;

                daRes = new SqlDataAdapter();
                daRes.SelectCommand = new SqlCommand();
                daRes.SelectCommand.Connection = this.cn;

                daRes.SelectCommand.CommandText = "SELECT l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                    "p.name) AS [ФамилияИмя], p.age AS [Г.р.], p.qf AS [Разряд], t.name AS [Команда], ";
                for (int i = 1; i <= routeNumber; i++)
                    daRes.SelectCommand.CommandText += "l.top" + i.ToString() + " AS [T" + i.ToString() +
                        "], l.bonus" + i.ToString() + " AS [B" + i.ToString() + "], ";
                daRes.SelectCommand.CommandText +=
                    "l.tops AS [Тр.], l.topAttempts AS [Попытки на трассы], " +
                    "l.bonuses AS [Бон.], l.bonusAttempts AS [Попытки на бонусы], " +
                    "l.qf AS [Кв.], l.nya AS [н/я], l.disq AS [дискв.] " +
                    "FROM boulderResults l(NOLOCK) JOIN Participants p(NOLOCK) ON " +
                    "l.climber_id = p.iid JOIN Teams t(NOLOCK) ON p.team_id = t.iid " +
                    "WHERE (l.list_id = @listID) AND (res IS NOT NULL) " +
                    "  AND ((l.top" + routeNumber.ToString() + " IS NOT NULL) OR (l.disq = 1) OR (l.nya = 1)) " +
                    "ORDER BY l.res DESC, l.pos, l.nya, l.disq, p.vk, t.name, p.surname, p.name";

                daRes.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                daRes.SelectCommand.Parameters[0].Value = this.listID;

                daPrint = new SqlDataAdapter(new SqlCommand(
                    "", cn));
                daPrint.SelectCommand.CommandText = "SELECT l.posText AS [Место], l.climber_id AS [№], (p.surname+' '+" +
                    "p.name) AS [ФамилияИмя], p.age AS [Г.р.], p.qf AS [Разряд], t.name AS [Команда], ";
                for (int i = 1; i <= routeNumber; i++)
                    daPrint.SelectCommand.CommandText += "l.top" + i.ToString() + " AS [T" + i.ToString() +
                        "], l.bonus" + i.ToString() + " AS [B" + i.ToString() + "], ";
                daPrint.SelectCommand.CommandText +=
                    "l.tops AS [Тр.], l.topAttempts AS [Попытки на трассы], " +
                    "l.bonuses AS [Бон.], l.bonusAttempts AS [Попытки на бонусы], " +
                    "l.qf AS [Кв.], l.nya AS [н/я], l.disq AS [дискв.] " +
                    "FROM boulderResults l(NOLOCK) JOIN Participants p(NOLOCK) ON " +
                    "l.climber_id = p.iid JOIN Teams t(NOLOCK) ON p.team_id = t.iid " +
                    "WHERE (l.list_id = @listID) AND (res IS NOT NULL) " +
                    //"  AND ((l.top" + routeNumber.ToString() + " IS NOT NULL) OR (l.disq = 1) OR (l.nya = 1)) " +
                    "ORDER BY l.res DESC, l.pos, l.nya, l.disq, p.vk, t.name, p.surname, p.name";

                daPrint.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                daPrint.SelectCommand.Parameters[0].Value = listID;

                daStart = new SqlDataAdapter();
                daStart.SelectCommand = new SqlCommand();
                daStart.SelectCommand.Connection = this.cn;

                daStart.SelectCommand.CommandText = "SELECT l.start AS [Ст.№], l.climber_id AS [№], (p.surname+' '+" +
                    "p.name) AS [ФамилияИмя], p.age AS [Г.р.], p.qf AS [Разряд], t.name AS [Команда], ";
                for (int i = 1; i <= routeNumber; i++)
                    daStart.SelectCommand.CommandText += "l.top" + i.ToString() + " AS [T" + i.ToString() +
                        "], l.bonus" + i.ToString() + " AS [B" + i.ToString() + "], ";
                daStart.SelectCommand.CommandText +=
                    "l.tops AS [Тр.], l.topAttempts AS [Попытки на трассы], " +
                    "l.bonuses AS [Бон.], l.bonusAttempts AS [Попытки на бонусы], " +
                    "l.nya AS [н/я], l.disq AS [дискв.] " +
                    "FROM boulderResults l INNER JOIN Participants p ON " +
                    "l.climber_id = p.iid INNER JOIN Teams t ON p.team_id = t.iid " +
                    "WHERE (l.preQf = 0) AND (l.list_id = @listID) AND (top" + routeNumber.ToString() + " IS NULL) AND (l.disq=0)AND(l.nya=0)" +
                    "ORDER BY l.start";
                //string strTmp = daStart.SelectCommand.CommandText.Replace("top1 IS NOT NULL", "top1 IS NULL");

                //daStart.SelectCommand.CommandText += " UNION " + strTmp + " ORDER BY l.start";

                daStart.SelectCommand.Parameters.Add("@listID", SqlDbType.Int);
                daStart.SelectCommand.Parameters[0].Value = this.listID;
#endif


                //SetQf();
                //RefreshData();
                try { UpdateDB(); }
                catch { }
#if !DEBUG
                btnStartListening.Visible = lblComPort.Visible = tbComPortNum.Visible = false;
                cbUseSystem.Visible = lblUseSystem.Visible = tbCOMportSys.Visible = false;
                btnStartP.Visible = tbPocketPort.Visible = false;
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                needToClose = true;
            }
        }

        private void GetDataFromRegistry()
        {
            appSettings aSet = appSettings.Default;
            aSet.Reload();
            tbComPortNum.Text = aSet.BTport;
            //tbComPortNum.Text = StaticClass.GetDataFromRegistry("BTport");
        }

        private void SaveDataToRegistry()
        {
            appSettings aSet = appSettings.Default;
            aSet.BTport = tbComPortNum.Text;
            aSet.Save();
            //StaticClass.SaveDataToRegistry("BTport", tbComPortNum.Text);
        }

        void RefreshData()
        {
#if MULTIBOULDER
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                mrb.FillData();
                mStart.WaitOne();
                try
                {
                    dtStart = mrb.GetList(MultiRouteBoulder.ListType.Start);
                    if (dgStart.InvokeRequired)
                        dgStart.Invoke(new EventHandler(delegate
                        {
                            dgStart.DataSource = dtStart;
                            try
                            {
                                dgStart.Columns[MultiRouteBoulder.ResNUM].Visible =
                                dgStart.Columns[MultiRouteBoulder.ClmNUM].Visible = false;
                            }
                            catch { }
                        }));
                    else
                    {
                        dgStart.DataSource = dtStart;
                        try
                        {
                            dgStart.Columns[MultiRouteBoulder.ResNUM].Visible =
                            dgStart.Columns[MultiRouteBoulder.ClmNUM].Visible = false;
                        }
                        catch { }
                    }
                    lowChanged = false;
                }
                finally { mStart.ReleaseMutex(); }

                mRes.WaitOne();
                try
                {
                    dtRes = mrb.GetList(MultiRouteBoulder.ListType.Res);
                    if (dgRes.InvokeRequired)
                        dgRes.Invoke(new EventHandler(delegate
                        {
                            dgRes.DataSource = dtRes;
                            try
                            {
                                dgRes.Columns[MultiRouteBoulder.ResNUM].Visible =
                                dgRes.Columns[MultiRouteBoulder.ClmNUM].Visible = false;
                            }
                            catch { }
                        }));
                    else
                    {
                        dgRes.DataSource = dtRes;
                        try
                        {
                            dgRes.Columns[MultiRouteBoulder.ResNUM].Visible =
                            dgRes.Columns[MultiRouteBoulder.ClmNUM].Visible = false;
                        }
                        catch { }
                    }
                    highChanged = false;
                }
                finally { mRes.ReleaseMutex(); }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message); }
#else
            mStart.WaitOne();
            dtStart.Clear();
            daStart.Fill(dtStart);
            /*try{
            dgStart.Invoke(new EventHandler(delegate{
        	                                	dgStart.DataSource = dtStart;
        	                                }));
            }
            catch{*/
            dgStart.DataSource = dtStart;//}
            mStart.ReleaseMutex();
            //dgStart.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            mRes.WaitOne();

            dtRes.Clear();
            daRes.Fill(dtRes);
            /*try{ dgRes.Invoke(new EventHandler(delegate{
        	                                	dgRes.DataSource = dtRes;
        	                                }));
            } catch{*/
            dgRes.DataSource = dtRes;//}
            mRes.ReleaseMutex();
            //dgRes.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
#endif
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (climbingTime < 0)
                return;
            if (secElapsed == 0)
            {
                secElapsed = 59;
                minElapsed--;
            }
            else
                secElapsed--;
            if (lblTime.InvokeRequired)
                lblTime.Invoke(new EventHandler(delegate
                {
                    lblTime.Text = minElapsed.ToString("00") + ":" + secElapsed.ToString("00");
                }));
            else
                lblTime.Text = minElapsed.ToString("00") + ":" + secElapsed.ToString("00");
            if ((minElapsed == 1) && (secElapsed == 2))
            {
#if DEBUG
                try { oneMinute.Play(); }
                catch { }
#endif
                //MessageBox.Show("Осталась одна минута.", "One minute left", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if ((minElapsed == 0) && (secElapsed == 0))
                {
#if DEBUG
                    try { rotate.Play(); }
                    catch { }
#endif
                    minElapsed = climbingTime;
                    secElapsed = 1;
                    if (frstRoute > 0)
                    {
                        frstRoute++;
                        tb1stRoute.Text = "";
                        foreach (DataRow rd in dtStart.Rows)
                            if (Convert.ToInt32(rd[0]) == frstRoute)
                            {
                                tb1stRoute.Text = frstRoute.ToString() + " - " + rd[1].ToString() + " - " +
                                        rd[2].ToString();
                                break;
                            }
                        SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=" +
                                        frstRoute.ToString() + "WHERE iid=" + listID.ToString(), cn);
                        try { cmd.ExecuteNonQuery(); }
                        catch { }
                    }
                    //MessageBox.Show("Переход.", "Time is up", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    if ((minElapsed == 0) && (secElapsed == 12))
                    {
#if DEBUG
                        try { getReady.Play(); }
                        catch { }
#endif
                        //MessageBox.Show("Приготовиться к переходу.", "Get ready for rotation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStart.Text == "Старт")
                {
                    if (climbingTime < 1)
                    {
                        MessageBox.Show("Время ротации не введено");
                        return;
                    }


                    if (StaticClass.TheOnlyTimer != null)
                    {
                        //cbUseSystem.Checked = (bt.SL != null);
                        if (StaticClass.TheOnlyTimer.Minutes != climbingTime)
                        {
                            if (MessageBox.Show("Уже запущен таймер на " + StaticClass.TheOnlyTimer.Minutes.ToString() + " минут." +
                                "\r\nЗаменить время?", "Замена таймера",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                                return;
                        }
                        StaticClass.TheOnlyTimer.RotationInterval = new TimeSpan(0, climbingTime, 0);
                        //if (cbUseSystem.Checked)
                        //{
                        //    UserForms.EKBListener el = GetSystemListener();
                        //    if (el == null)
                        //    {
                        //        MessageBox.Show("Не удалось подключиться к системе");
                        //        btnStart.Text = "Старт";
                        //        this.bt = null;
                        //        return;
                        //    }
                        //    else
                        //        bt.SL = el;
                        //}
                        //bt.Reset();


                    }
                    else
                        StaticClass.TheOnlyTimer = new ClimbingTimer(cn.ConnectionString, new TimeSpan(0, climbingTime, 0), true);
                    StaticClass.TheOnlyTimer.timerEvent += new EventHandler<ClimbingTimerEventArgs>(ResultListBoulder_RotationEvent);
                    StaticClass.TheOnlyTimer.Start();
                    cbUseSystem.Enabled = tbCOMportSys.Enabled = false;
                    btnStart.Text = "Стоп";
                    /*UserForms.EKBListener sl;
                    if (cbUseSystem.Checked)
                    {
                        sl = GetSystemListener();
                        if (sl == null)
                        {
                            MessageBox.Show("Не удалось подключиться к системе");
                            btnStart.Text = "Старт";
                            this.bt = null;
                            return;
                        }
                    }
                    else
                        sl = null;
                    bt = new BoulderingTimer(climbingTime, cn, true, sl);
                    if (bt != null)
                    {
                        mfr.bTimer = bt;
                        bt.RotationEvent += new BoulderingTimer.RotationEventHandle(ResultListBoulder_RotationEvent);
                        cbUseSystem.Enabled = tbCOMportSys.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show(this, "Не удалось запустить таймер");
                        btnStart.Text = "Старт";
                    }*/
                }
                else
                {
                    if (StaticClass.TheOnlyTimer == null)
                        return;
                    if (MessageBox.Show("Остановить таймер?\r\nВнимание!!! Таймер будет остановлен во всех группах!",
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.No)
                        return;
                    bool reset = MessageBox.Show("Сбросить таймер?", String.Empty, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
                    if (reset)
                    {
                        StaticClass.TheOnlyTimer.Dispose();
                        StaticClass.TheOnlyTimer = null;
                    }
                    else
                    {
                        StaticClass.TheOnlyTimer.Stop();
                    }
                    btnStart.Text = "Старт";

                    SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=NULL WHERE iid=" + listID.ToString(), cn);
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                    tb1stRoute.Text = "";
                    cbUseSystem.Enabled = true;
                    tbCOMportSys.Enabled = !cbUseSystem.Checked;
                    return;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка обновления состояния таймера:\r\n" + ex.Message); }
        }

        private EKBListener GetSystemListener()
        {
            SystemListener slI = SpeedFinals.getParentListener(this);
            if (!(slI is EKBListener))
                return null;
            EKBListener sl = (EKBListener)slI;
            bool createNew;
            if (sl != null && sl.PortName != tbCOMportSys.Text)
                createNew = (MessageBox.Show(this, "Уже запущена система считывания данных на другом порту.\r\n" +
                    "Вы уверена, что хотите изменить порт?", "Изменить порт", MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question) == DialogResult.Yes);
            else
                createNew = (sl == null);
            if (createNew)
            {
                SerialPort sp;
                appSettings aset = appSettings.Default;
                if (aset.systemPort == null || (aset.systemPort != null && aset.systemPort.PortName.ToLower() != tbCOMportSys.Text.ToLower()))
                    sp = SerialPortConfig.ConfigurePort(tbCOMportSys.Text, this);
                else
                    sp = aset.systemPort;
                if (sp == null)
                    return null;
                try { sl = new EKBListener(sp, false, this.MdiParent, SpeedSystemHelper.ShowErrors, SpeedSystemHelper.PersistShowErrors); }
                catch { return null; }
            }
            try
            {
                if (sl.StartListening())
                    SpeedFinals.setParentListener(sl, this);
                else
                    sl = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка соединения с системой:\r\n" + ex.Message);
                sl = null;
            }
            return sl;
        }

        void ResultListBoulder_RotationEvent(object sender, ClimbingTimerEventArgs e)
        {
            //lblTime.Text = e.Time;
            if (isClosed)
                return;
            var t = sender as ClimbingTimer;
            if (t == null || t.AllowDispose)
                return;
            using (SqlConnection cn = new SqlConnection(this.cn.ConnectionString))
            {
                string sTime = e.Minute.ToString("00") + ":" + e.Second.ToString("00");
                if (lblTime.InvokeRequired)
                    lblTime.Invoke(new EventHandler(delegate
                    {
                        lblTime.Text = sTime;
                    }));
                else
                    lblTime.Text = sTime;
                bool isRotate = e.Second == 0 && e.Minute == 0;
                if ((frstRoute > 0) && isRotate)
                {
                    //return;
                    bool inc;
                    int cCur = GetClimberAtRoute(1, listID, out inc, cn), nTmp;
                    if (inc)
                        nTmp = cCur;
                    else
                        nTmp = GetNextClimber(cCur, this.listID, cn);

                    if (nTmp > 0)
                        frstRoute = nTmp;
                    else
                        frstRoute++;
                    //tb1stRoute.Invoke(new EventHandler(delegate { tb1stRoute.Text = ""; }));
                    //foreach (DataRow rd in dtStart.Rows)
                    //    if (Convert.ToInt32(rd[0]) == frstRoute)
                    //    {
                    //        tb1stRoute.Text = frstRoute.ToString() + " - " + rd[1].ToString() + " - " +
                    //                rd[2].ToString();
                    //        break;
                    //    }
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=" +
                                    frstRoute.ToString() + "WHERE iid=" + listID.ToString(), cn);
                    try { cmd.ExecuteNonQuery(); }
                    catch { }
                    int p1, p2, rn;
                    GetPauses(listID, cn, out p1, out p2, out rn);
                    if (p1 > 10)
                    {
                        p1 = p1 - 10 + 1;
                        if (p1 > rn)
                            p1 = -1;
                    }
                    else if (p1 == routeNumber)
                        p1 = -1;
                    else if (p1 > 0)
                        p1 += 10;
                    else
                        p1 = -1;
                    if (p2 > 10)
                    {
                        p2 = p2 - 10 + 1;
                        if (p2 > rn)
                            p2 = -1;
                    }
                    else if (p2 == routeNumber)
                        p2 = -1;
                    else if (p2 > 0)
                        p2 += 10;
                    else
                        p2 = -1;
                    cmd.CommandText = "UPDATE lists SET nowClimbingTmp=" + p1.ToString() + ",nowClimbing3=" + p2.ToString() + " WHERE iid=" + listID.ToString();
                    cmd.ExecuteNonQuery();
                    setNowClStr(cn);

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private bool UpdateDB()
        {
#if MULTIBOULDER
            try
            {

                if (cn.State != ConnectionState.Open)
                    cn.Open();
                mStart.WaitOne();
                try
                {
                    mRes.WaitOne();
                    try
                    {
                        bool tranSuccess = false;
                        SqlTransaction tran = cn.BeginTransaction();
                        try
                        {
                            mrb.SaveRes(dtStart, tran);
                            mrb.SaveRes(dtRes, tran);
                            GetResultList(tran);
                            tranSuccess = true;
                        }
                        finally
                        {
                            if (tranSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();
                        }
                    }
                    finally { mRes.ReleaseMutex(); }
                }
                finally { mStart.ReleaseMutex(); }
                lowChanged = highChanged = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения данных:\r\n" +
                    ex.Message);
                return false;
            }
            finally { RefreshData(); }
#else
            mStart.WaitOne();
            try
            {
                string erStr = "";
                SqlTransaction tran = null;
                try
                {
                    tran = cn.BeginTransaction();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("iid", typeof(int));
                    for (int i = 1; i <= routeNumber; i++)
                    {
                        dt.Columns.Add("T" + i.ToString(), typeof(int));
                        dt.Columns.Add("B" + i.ToString(), typeof(int));
                    }
                    dt.Columns.Add("T", typeof(int));
                    dt.Columns.Add("Ta", typeof(int));
                    dt.Columns.Add("B", typeof(int));
                    dt.Columns.Add("Ba", typeof(int));
                    dt.Columns.Add("disq", typeof(bool));
                    dt.Columns.Add("nya", typeof(bool));
                    dt.Columns.Add("res", typeof(long));
                    List<int> errorList = new List<int>();
                    AddRowToUpdateTable(dt, dtStart, errorList);
                    AddRowToUpdateTable(dt, dtRes, errorList);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.UpdateCommand = new SqlCommand();
                    da.UpdateCommand.Connection = cn;
                    da.UpdateCommand.CommandText = @"
            UPDATE boulderResults SET tops = @t, topAttempts = @ta, bonuses = @b, bonusAttempts = @ba,
                                      res = @res, disq = @disq, nya = @nya";
                    for (int i = 1; i <= routeNumber; i++)
                        da.UpdateCommand.CommandText += ", top" + i.ToString() + "=@t" + i.ToString() +
                            ", bonus" + i.ToString() + "=@b" + i.ToString();
                    da.UpdateCommand.CommandText += " WHERE list_id=" + listID.ToString() + " AND climber_id=@iid";
                    da.UpdateCommand.Parameters.Add("@t", SqlDbType.Int, 4, "T");
                    da.UpdateCommand.Parameters.Add("@ta", SqlDbType.Int, 4, "Ta");
                    da.UpdateCommand.Parameters.Add("@b", SqlDbType.Int, 4, "B");
                    da.UpdateCommand.Parameters.Add("@ba", SqlDbType.Int, 4, "Ba");
                    da.UpdateCommand.Parameters.Add("@res", SqlDbType.BigInt, 8, "res");
                    da.UpdateCommand.Parameters.Add("@disq", SqlDbType.Bit, 1, "disq");
                    da.UpdateCommand.Parameters.Add("@nya", SqlDbType.Bit, 1, "nya");
                    da.UpdateCommand.Parameters.Add("@iid", SqlDbType.Int, 4, "iid");
                    for (int i = 1; i <= routeNumber; i++)
                    {
                        da.UpdateCommand.Parameters.Add("@t" + i.ToString(), SqlDbType.Int, 4, "T" + i.ToString());
                        da.UpdateCommand.Parameters.Add("@b" + i.ToString(), SqlDbType.Int, 4, "B" + i.ToString());
                    }
                    da.UpdateCommand.Transaction = tran;
                    da.InsertCommand = da.UpdateCommand;
                    if (da.UpdateCommand.Connection.State != ConnectionState.Open)
                        da.UpdateCommand.Connection.Open();
                    da.Update(dt);

                    GetResultList(tran);


                    foreach (int i in errorList)
                    {
                        if (erStr.Length > 0)
                            erStr += ", ";
                        erStr += i.ToString();
                    }
                    bool commit = true;
                    if (erStr.Length > 0)
                        if (MessageBox.Show("Были исправлены ошибки/опечатки при вводе результатов следующих участников:\r\n" +
                            erStr + "\r\nПрименить изменения?", "Ошибки", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                            commit = false;
                    if (commit)
                    {
                        tran.Commit();
                        lowChanged = highChanged = false;
                    }
                    else
                        tran.Rollback();
                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        try { tran.Rollback(); }
                        catch { }
                    throw ex;
                }
                finally { RefreshData(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления БД\r\n" + ex.Message);
                return false;
            }
            finally { mStart.ReleaseMutex(); }
#endif
        }

        

        private void GetResultList(SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            cmd.Connection = cn;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.CommandText = @"SELECT c.vk, 0 pos, ''posText, c.iid, '' qf, 0.0 pts, '' ptsText,
                                       CASE WHEN l.nya = 1 THEN 2
                                            WHEN l.disq = 1 THEN 1
                                            ELSE 0 END sp, l.pos pos0, l.res
                                  FROM boulderresults l(NOLOCK)
                                  JOIN participants c(NOLOCK) ON c.iid = l.climber_id
                                 WHERE (ISNULL(l.res,0)>0 OR l.disq = 1 OR l.nya = 1)
                                   AND l.list_id=" + listID.ToString() + @"
                                   AND l.preQf = 0
                              ORDER BY l.res DESC, l.pos";
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
            foreach (int i in CreateListSeq(listID, cn, tran))
            {
                DataTable dtI = GetList(i, tran);
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
            CompetitionRules compRules = ((CR & SpeedRules.InternationalRules) == SpeedRules.InternationalRules) ? CompetitionRules.International : CompetitionRules.Russian;
            SortingClass.SortResults(dt, q, cnt < 1, false, compRules);
            if (cnt < 1)
                foreach (DataRow dr in dt.Rows)
                    switch (Convert.ToInt32(dr["sp"]))
                    {
                        case 1:
                            dr["posText"] = "дискв.";
                            break;
                        case 2:
                            dr["posText"] = "н/я";
                            break;
                    }
            cmd.CommandText = "UPDATE boulderResults SET pos=@p, posText=@pt, pts=@pts, qf = @qf " +
                              " WHERE list_id = " + listID.ToString() + " AND climber_id = @cid";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@p", SqlDbType.Int, 4, "pos");
            cmd.Parameters.Add("@pt", SqlDbType.VarChar, 50, "posText");
            cmd.Parameters.Add("@pts", SqlDbType.Float, sizeof(float), "pts");
            cmd.Parameters.Add("@cid", SqlDbType.Int, 4, "iid");
            cmd.Parameters.Add("@qf", SqlDbType.VarChar, 50, "qf");
            da.UpdateCommand = cmd;
            da.InsertCommand = cmd;
            da.Update(dt);
            if (SettingsForm.GetPosToFalls(cmd.Connection, cmd.Transaction))
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT COUNT(*) cnt" +
                                  "  FROM boulderResults br(nolock)" +
                                  "  JOIN lists l(nolock) on l.iid = br.list_id" +
                                  "  JOIN Participants p(nolock) on p.iid = br.climber_id" +
                                  " WHERE l.iid = " + listID.ToString() +
                                  "   AND l.prev_round IS NULL" +
                                  "   AND p.vk = 0" +
                                  "   AND br.nya = 0";
                int ptCount;
                object oTmp = cmd.ExecuteScalar();
                if (oTmp != null && oTmp != DBNull.Value)
                    ptCount = Convert.ToInt32(oTmp);
                else
                    ptCount = -1;
                if (ptCount > 0)
                {
                    cmd.CommandText = "UPDATE boulderResults" +
                                      "   SET pos = @pos," +
                                      "       posText = CASE P.vk WHEN 0 THEN CONVERT(VARCHAR,@pos) ELSE @posVK END" +
                                      "  FROM boulderResults BR(nolock)" +
                                      "  JOIN Participants P(nolock) on P.iid = BR.climber_id" +
                                      " WHERE BR.list_id = " + listID.ToString() +
                                      "   AND BR.res IS NOT NULL" +
                                      "   AND BR.bonuses = 0" +
                                      "   AND BR.tops = 0";
                    cmd.Parameters.Add("@pos", SqlDbType.Int).Value = ptCount;
                    cmd.Parameters.Add("@posVK", SqlDbType.VarChar, 3).Value = "в/к";
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
        }

        public static List<int> CreateListSeq(int startID, SqlConnection cn, SqlTransaction tran)
        {
            List<int> res = new List<int>();
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            int curID = startID;
            cmd.CommandText = "SELECT prev_round FROM lists(NOLOCK) WHERE iid = @id";
            cmd.Parameters.Add("@id", SqlDbType.Int);
            while (true)
            {
                cmd.Parameters[0].Value = curID;
                object r = cmd.ExecuteScalar();
                if (r != null && r != DBNull.Value)
                {
                    curID = Convert.ToInt32(r);
                    res.Add(curID);
                }
                else
                    break;
            }
            return res;
        }

        private DataTable GetList(int iid, SqlTransaction tran)
        {
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
            if (round.IndexOf("2 гр") > -1)
            {
                dt = StaticClass.FillResOnsight(iid, cn, true, tran);
                if (dt.Columns.IndexOf("№") > -1)
                    dt.Columns["№"].ColumnName = "iid";
                dt.Columns.Add("rNum");
                DataColumn dcA = null, dcB = null;
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ColumnName.ToLower().IndexOf("та") > -1)
                        dcA = dc;
                    else if (dc.ColumnName.ToLower().IndexOf("тб") > -1)
                        dcB = dc;
                if (dcB == null || dcA == null)
                    return null;
                foreach (DataRow dr in dt.Rows)
                    if (dr["pos"] != null && dr["pos"] != DBNull.Value && Convert.ToInt32(dr["pos"]) < 1)
                        dr["rNum"] = 3;
                    else if (dr[dcA] != null && dr[dcA] != DBNull.Value)
                        dr["rNum"] = 1;
                    else if (dr[dcB] != null && dr[dcB] != DBNull.Value)
                        dr["rNum"] = 2;
                    else
                        dr["rNum"] = 3;
                for (int i = 0; i < dt.Columns.Count; i++)
                    if (dt.Columns[i].ColumnName != "pos" &&
                        dt.Columns[i].ColumnName != "rNum" &&
                        dt.Columns[i].ColumnName != "iid")
                    {
                        dt.Columns.RemoveAt(i);
                        i--;
                    }
            }
            else
            {
                cmd.CommandText = "SELECT climber_id iid, CASE preQf WHEN 0 THEN 1 ELSE 3 END rNum, ISNULL(pos, " + int.MaxValue.ToString() + ") pos" +
                                  "  FROM boulderResults(NOLOCK) " +
                                  " WHERE list_id = " + iid.ToString();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.Transaction = tran;
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }

#if !MULTIBOULDER
        private enum resTpe { RESULT, DISQ, NYA, NOTHING }
        private void AddRowToUpdateTable(DataTable dt, DataTable dtSource, List<int> errorList)
        {
            if (errorList == null)
                errorList = new List<int>();
            object[] obj = new object[dt.Columns.Count];
            for (int i = 0; i < obj.Length; i++)
                obj[i] = DBNull.Value;
            foreach (DataRow dr in dtSource.Rows)
            {
                //bool nd = Convert.ToBoolean(dr["н/я"]) || Convert.ToBoolean(dr["дискв."]);

                resTpe rt = resTpe.NOTHING;
                if (Convert.ToBoolean(dr["дискв."]))
                    rt = resTpe.DISQ;
                else if (Convert.ToBoolean(dr["н/я"]))
                    rt = resTpe.NYA;
                if (rt == resTpe.NOTHING)
                {
                    bool errorAdded = false;
                    for (int i = 1; i <= routeNumber; i++)
                    {
                        int nt = -1, nb = -1;
                        DataColumn dcT = dtSource.Columns["T" + i.ToString()];
                        DataColumn dcB = dtSource.Columns["B" + i.ToString()];
                        if (dr[dcT] != null && dr[dcT] != DBNull.Value)
                            nt = Convert.ToInt32(dr[dcT]);
                        if (dr[dcB] != null && dr[dcB] != DBNull.Value)
                            nb = Convert.ToInt32(dr[dcB]);
                        if (nt >= 0 && nb >= 0 && nt >= nb)
                        {
                            if (nt > 0 && nb == 0)
                            {
                                if (!errorAdded)
                                {
                                    errorList.Add(Convert.ToInt32(dr["№"]));
                                    errorAdded = true;
                                }
                                dr[dcB] = nt;
                            }
                            rt = resTpe.RESULT;
                        }
                        else if (nt >= 0 && nb >= 0 && nt < nb)
                        {
                            if (nt > 0)
                            {
                                if (!errorAdded)
                                {
                                    errorList.Add(Convert.ToInt32(dr["№"]));
                                    errorAdded = true;
                                }
                                dr[dcT] = nb;
                            }
                            rt = resTpe.RESULT;
                        }
                        else if (nt < 0 && nb >= 0)
                        {
                            dr[dcT] = 0;
                            rt = resTpe.RESULT;
                        }
                        else if (nt >= 0 && nb < 0)
                        {
                            dr[dcB] = nt;
                            if (!errorAdded)
                            {
                                errorList.Add(Convert.ToInt32(dr["№"]));
                                errorAdded = true;
                            }
                            rt = resTpe.RESULT;
                        }
                    }
                }
                dt.Rows.Add(obj);
                DataRow drI = dt.Rows[dt.Rows.Count - 1];
                //object[] obj = new object[dt.Columns.Count];
                drI["iid"] = Convert.ToInt32(dr["№"]);
                drI["disq"] = (rt == resTpe.DISQ);
                drI["nya"] = (rt == resTpe.NYA);
                //nd = (bool)drI["disq"] || (bool)drI["nya"];
                int t = 0, ta = 0, b = 0, ba = 0;
                if (rt == resTpe.RESULT)
                {
                    for (int i = 1; i <= routeNumber; i++)
                    {
                        if (dr["T" + i.ToString()] == DBNull.Value || dr["T" + i.ToString()] == null)
                            drI["T" + i.ToString()] = DBNull.Value;
                        else
                        {

                            drI["T" + i.ToString()] = Convert.ToInt32(dr["T" + i.ToString()]);
                            if ((int)drI["T" + i.ToString()] > 0)
                            {
                                t++;
                                ta += (int)drI["T" + i.ToString()];
                            }
                        }
                        if (dr["B" + i.ToString()] == DBNull.Value || dr["B" + i.ToString()] == null)
                            drI["B" + i.ToString()] = DBNull.Value;
                        else
                        {
                            drI["B" + i.ToString()] = Convert.ToInt32(dr["B" + i.ToString()]);
                            if ((int)drI["B" + i.ToString()] > 0)
                            {
                                b++;
                                ba += (int)drI["B" + i.ToString()];
                            }
                        }
                    }
                }
                drI["T"] = t;
                drI["Ta"] = ta;
                drI["B"] = b;
                drI["Ba"] = ba;
                switch (rt)
                {
                    case resTpe.NYA:
                        if (baseIID > 0)
                            drI["res"] = 8;
                        else
                            drI["res"] = 5;
                        break;
                    case resTpe.DISQ:
                        drI["res"] = 8;
                        break;
                    case resTpe.RESULT:
                        drI["res"] = (t * 1000 + (900 - ta)) * 10000 + (b * 1000 + (900 - ba));
                        break;
                    default:
                        drI["res"] = 0;
                        break;
                }
            }
        }
#endif

        private void xlExport_Click(object sender, EventArgs e)
        {

            Excel.Workbook wb;
            Excel.Worksheet ws;
            Excel.Application xlApp;
            char total = 'J';
            for (int qwe = 0; qwe < routeNumber; qwe++)
            { total++; total++; }

            if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                return;
            try
            {
                DataTable dtPrint;
#if MULTIBOULDER
                mrb.FillData();
                dtPrint = mrb.GetList(MultiRouteBoulder.ListType.Print);
                try
                {
                    int ijklh = dtPrint.Columns.IndexOf(MultiRouteBoulder.ClmNUM);
                    if (ijklh > -1)
                        dtPrint.Columns.RemoveAt(ijklh);
                    ijklh = dtPrint.Columns.IndexOf(MultiRouteBoulder.ResNUM);
                    if (ijklh > -1)
                        dtPrint.Columns.RemoveAt(ijklh);
                }
                catch { }
#else
                dtPrint = new DataTable();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                daPrint.Fill(dtPrint);
#endif
                int i = 0;
                int cnt;
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM boulderResults WHERE (list_id=" +
                    listID.ToString() + ") AND (qf='Q')", cn);
                try { cnt = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch { cnt = 0; }
                const int frst_row = 6;
                for (int clInd = 0; clInd < dtPrint.Columns.Count - 2; clInd++)
                {
                    DataColumn cl = dtPrint.Columns[clInd];
                    i++;
                    if ((cl.ColumnName.IndexOf("опытк") > -1))
                        ws.Cells[frst_row, i] = "Поп.";
                    else
                    {
                        if (cl.ColumnName == "№")
                        {
                            i--;
                            continue;
                        }
                        if ((cl.ColumnName == "Кв.") && (cnt == 0))
                            continue;
                        else
                        {
                            if (cl.ColumnName == "ФамилияИмя")
                                ws.Cells[frst_row, i] = "Фамилия, Имя";
                            else
                                ws.Cells[frst_row, i] = cl.ColumnName;
                        }
                    }
                }
                i = frst_row;
                int lastFin = -1;
                Excel.Range dt;
                foreach (DataRow rd in dtPrint.Rows)
                {
                    i++;
                    char[] qaz = { 'T', 'B' };


                    int k = 1, j = 0;
                    while (k <= dtPrint.Columns.Count - 7)
                    {
                        if (dtPrint.Columns[k - 1].ColumnName == "№")
                        {
                            j = 1;
                            k++;
                            continue;
                        }
                        if ((dtPrint.Columns[k - 1].ColumnName.IndexOfAny(qaz) > -1) && (rd[k - 1].ToString() == "0"))
                            ws.Cells[i, k - j] = "-";
                        else
                            ws.Cells[i, k - j] = rd[k - 1];
                        k++;
                    }

                    if ((bool)rd["н/я"] || (bool)rd["дискв."])
                    {
                        char c = 'A';
                        for (int kk = 0; kk < k - 1 - j; kk++)
                            c++;
                        char d = c;
                        for (int kk = 0; kk < 3; kk++)
                            d++;
                        dt = ws.get_Range(c + i.ToString(), d + i.ToString());
                        dt.Merge(Type.Missing);
                        dt.Style = "MyStyle";

                        if ((bool)rd["н/я"])
                            dt.Cells[1, 1] = "н/я";
                        else
                            dt.Cells[1, 1] = "дискв.";
                        ws.Cells[i, 1] = "";
                        k += 4;
                    }
                    else
                    {
                        while (k <= dtPrint.Columns.Count - 3)
                        {
                            ws.Cells[i, k - j] = rd[k - 1];
                            k++;
                        }
                    }
                    ws.Cells[i, k - j] = rd[k - 1];

                    if ((rd["Г.р."].ToString() == "0"))
                        ws.Cells[i, 4 - j] = "";
                    if (rd["Кв."].ToString() == "Q")
                        lastFin = i;

                }
                if (lastFin > -1)
                {
                    dt = ws.get_Range("A" + frst_row.ToString(), total + lastFin.ToString());
                    dt.Style = "MyStyle";
                    total--;
                    if (i != lastFin)
                        dt = ws.get_Range("A" + (lastFin + 1).ToString(), total + i.ToString());
                    dt.Style = "MyStyle";
                    total++;
                    dt = ws.get_Range("A" + frst_row.ToString(), total + lastFin.ToString());
                }
                else
                {
                    total--;
                    dt = ws.get_Range("A" + frst_row.ToString(), total + i.ToString());
                    dt.Style = "MyStyle";
                    total++;
                }

                dt = ws.get_Range("A" + frst_row.ToString(), total + i.ToString());
                dt.Columns.AutoFit();


                for (int fgh = 0; fgh < 5; fgh++)
                    total--;

                dt = ws.get_Range(total + frst_row.ToString(), total + i.ToString());
                dt.Style = "MyStyle";

                for (int fgh = 0; fgh < 5; fgh++)
                    total++;

                if (cnt == 0)
                    total--;

                dt = ws.get_Range("B" + frst_row.ToString(), "B" + i.ToString());
                dt.Style = "StyleLA";

                dt = ws.get_Range("A1", total + "1");
                dt.Style = "CompTitle";
                dt.Merge(Type.Missing);
                string strTmp = "";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];

                dt = ws.get_Range("A3", total + "3");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                strTmp = "Боулдеринг. " + gName;
                dt.Cells[1, 1] = strTmp;

                ws.Cells[2, 1] = StaticClass.ParseCompTitle(competitionTitle)[1];
                dt = ws.get_Range(total + "2", total + "2");
                dt.Style = "StyleRA";
                dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[2];

                if (judgeID > 0)
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду: " + StaticClass.GetJudgeData(judgeID, cn, true);
                else
                    ws.Cells[5, 1] = "Зам. гл. судьи по виду:";

                strTmp = "Протокол результатов. " + round;
                dt = ws.get_Range("A4", total + "4");
                dt.Style = "Title";
                dt.Merge(Type.Missing);
                dt.Cells[1, 1] = strTmp;
                strTmp = "рез_";
                strTmp += "Б_";

                try { ws.Name = strTmp + StaticClass.CreateSheetName(gName, round); }
                catch { }
                StaticClass.ExcelUnderlineQf(ws);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка экспорта данных в Excel:\r\n" + ex.Message); }
            finally { StaticClass.SetExcelVisible(xlApp); }

        }

        private void SaveData()
        {
            try
            {

                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=NULL WHERE iid=" + listID.ToString(), cn);
                cmd.ExecuteNonQuery();
                UpdateDB();
            }
            catch { }
            //if (MessageBox.Show("Вы уверены? Секундомер будет ОСТАНОВЛЕН!", "Exit form", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.No)
            //    this.Activate();
        }

        private void btnNxtRound_Click(object sender, EventArgs e)
        {
            //NewStartList nl;
            string tableName;
            string nxtRount;
            ListTypeEnum nxtLTp;
            int routeN;
            if (NextRoundForm.GetNextRoundData(this.round, this, out nxtRount, out routeN))
                return;
            if (nxtRount.ToLower() == "суперфинал")
            {
                tableName = "routeResults";
                nxtLTp = ListTypeEnum.BoulderSuper;
            }
            else
            {
                nxtLTp = ListTypeEnum.BoulderSimple;
                tableName = "boulderResults";
            }
            int nextID;
            bool b = false;
            SqlCommand cmd = new SqlCommand("SELECT l.iid FROM lists l(NOLOCK), lists l1(NOLOCK) WHERE " +
                "(l.style=l1.style) AND (l.group_id=l1.group_id) AND (l1.iid=" + listID.ToString() +
                ") AND (l.round = @nr)", cn);
            cmd.Parameters.Add("@nr", SqlDbType.VarChar);
            cmd.Parameters[0].Value = nxtRount;
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
            cmd.CommandText = "SELECT r.climber_id, r.pos, p.rankingBoulder, r.start FROM " +
                "boulderResults r(NOLOCK) JOIN Participants p(NOLOCK) ON r.climber_id=p.iid " +
                "WHERE (r.qf='Q' OR r.preQf = 1) AND (r.list_id=@i) ORDER BY r.start";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@i", SqlDbType.Int);
            cmd.Parameters[0].Value = listID;
            SqlDataReader rd = cmd.ExecuteReader();
            Random rnd = new Random();
            List<Starter> data = new List<Starter>();
            while (rd.Read())
            {
                StartListMember stm = new StartListMember();
                Starter strt = new Starter();
                stm.iid = Convert.ToInt32(rd[0]);
                try { stm.ranking = Convert.ToInt32(rd[2]); }
                catch { stm.ranking = int.MaxValue; }
                stm.rndDouble = rnd.NextDouble();
                stm.prevPos = Convert.ToInt32(rd[1]);
                strt.iid = stm.iid;
                strt.lateAppl = false;
                strt.prevPos = stm.prevPos;
                try { strt.prevStart = Convert.ToInt32(rd[3]); }
                catch { strt.prevStart = 0; }
                strt.random = stm.rndDouble;
                strt.ranking = stm.ranking;
                data.Add(strt);
                starters.Add(stm);
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
                    cmd.CommandText = "DELETE FROM " + tableName + " WHERE list_id = @i";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE lists SET next_round=NULL, prev_round=NULL " +
                        "WHERE iid=@i";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "INSERT INTO lists(iid,group_id,style,round, routeNumber,listType) VALUES (@i,@g,@s,@r,@rn,@ltp)";

                    cmd.Parameters.Add("@s", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = "Боулдеринг";

                    cmd.Parameters.Add("@r", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = nxtRount;

                    cmd.Parameters.Add("@rn", SqlDbType.Int);
                    cmd.Parameters[4].Value = (routeN > 0 ? routeN : (object)DBNull.Value);

                    cmd.Parameters.Add("@ltp", SqlDbType.VarChar, 255);
                    cmd.Parameters[5].Value = nxtLTp.ToString();

                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = "UPDATE lists SET next_round=@i WHERE iid=@g";
                cmd.Parameters[1].Value = listID;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE lists SET prev_round=@g WHERE iid=@i";
                cmd.ExecuteNonQuery();

                //nl = new NewStartList(false, group_name, "Боулдеринг", cbRound.SelectedItem.ToString());
                //nl.ShowDialog();
                Sorting sorting = new Sorting(data, StartListMode.NotFirstRound);
                sorting.ShowDialog();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                //if (nl.type == StartListType.Cancel)
                if (sorting.Cancel)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch
                    {
                        cmd.Parameters[0].Value = nextID;
                        cmd.CommandText = "DELETE FROM " + tableName + " WHERE list_id = @i";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "UPDATE lists SET next_round=NULL, prev_round=NULL " +
                            "WHERE iid=@i";
                        cmd.ExecuteNonQuery();
                    }
                    return;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE lists SET online=@o, routeNumber = @rn WHERE iid=" + nextID.ToString();
                cmd.Parameters.Add("@o", SqlDbType.Bit);
                cmd.Parameters[0].Value = StaticClass.IsListOnline(listID, cn, cmd.Transaction);
                cmd.Parameters.Add("@rn", SqlDbType.Int);
                cmd.Parameters[1].Value = (routeN > 0 ? routeN : (object)DBNull.Value);
                cmd.ExecuteNonQuery();

                long nxtIID;
                cmd.CommandText = "SELECT MAX(iid) FROM " + tableName;
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
                cmd.CommandText = "INSERT INTO " + tableName + "(iid, list_id, climber_id, start, pos) " +
                    "VALUES (@iid, @list_id, @climber_id, @start, @pos)";
                //for (int i = 0; i < starters.Count; i++)
                int i = 0;
                foreach (Starter sts in sorting.Starters)
                {
                    cmd.Parameters[1].Value = (++nxtIID);
                    //cmd.Parameters[2].Value = ((StartListMember)starters[i]).iid;
                    cmd.Parameters[2].Value = sts.iid;
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

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataRow rd in dtStart.Rows)
                if (rd["B1"].ToString() == "")
                {
                    try
                    {
                        frstRoute = Convert.ToInt32(rd[0]);
                        SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=" +
                            frstRoute.ToString() + "WHERE iid=" + listID.ToString(), cn);
                        try { cmd.ExecuteNonQuery(); }
                        catch { }
                        setNowClStr(cn);
                        return;
                    }
                    catch { frstRoute = 0; }
                    break;
                }
        }

        private void btnCorrect_Click(object sender, EventArgs e)
        {
            char[] dig = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string route = "";
            string number = "";
            int i = tb1stRoute.Text.IndexOfAny(dig);
            if (i == -1)
            {
                MessageBox.Show("Данные не введены");
                return;
            }
            bool nowRoute = true;
            while (true)
            {
                if (nowRoute)
                    route += tb1stRoute.Text[i];
                else
                    number += tb1stRoute.Text[i];
                int nTmp = tb1stRoute.Text.IndexOfAny(dig, i + 1);
                if (nTmp != (i + 1))
                {
                    if (nowRoute)
                    {
                        if (nTmp == -1)
                        {
                            number = route;
                            route = "1";
                            break;
                        }
                        nowRoute = false;
                    }
                    else
                        break;
                }
                i = nTmp;
            }

            int nR, nS;
            if (!int.TryParse(route, out nR))
                return;
            if (!int.TryParse(number, out nS))
                return;
            setClimber(nR, nS);
            setNowClStr(cn);
            return;

            //int nRoute = 0, nNum = 0;
            //int mult = 1;
            //for (i = route.Length - 1; i >= 0; i--)
            //{
            //    for (int k = 0; k < dig.Length; k++)
            //        if (dig[k] == route[i])
            //        {
            //            nRoute += mult * k;
            //            break;
            //        }
            //    mult *= 10;
            //}
            //mult = 1;
            //for (i = number.Length - 1; i >= 0; i--)
            //{
            //    for (int k = 0; k < dig.Length; k++)
            //        if (dig[k] == number[i])
            //        {
            //            nNum += mult * k;
            //            break;
            //        }
            //    mult *= 10;
            //}
            //frstRoute = nNum + 2 * (nRoute - 1);

            //tb1stRoute.Text = "";
            //foreach (DataRow rd in dtStart.Rows)
            //    if (Convert.ToInt32(rd[0]) == frstRoute)
            //    {
            //        tb1stRoute.Text = frstRoute.ToString() + " - " + rd[1].ToString() + " - " +
            //                rd[2].ToString();
            //        break;
            //    }
            //SqlCommand cmd = new SqlCommand("UPDATE lists SET nowClimbing=" +
            //                frstRoute.ToString() + "WHERE iid=" + listID.ToString(), cn);
            //try { cmd.ExecuteNonQuery(); }
            //catch { }
        }

        private void setNowClStr(SqlConnection cn)
        {
            tb1stRoute.Invoke(new EventHandler(delegate
            {
                try { tb1stRoute.Text = getNowClStr(listID, cn); }
                catch { tb1stRoute.Text = ""; }
            }));
            try
            {
                int p1, p2, rn;
                GetPauses(listID, cn, out p1, out p2, out rn);
                tbP1.Invoke(new EventHandler(delegate
                {
                    if (p1 > 10)
                        tbP1.Text = "После трассы " + (p1 - 10).ToString();
                    else if (p1 > 0)
                        tbP1.Text = "На трассе " + p1.ToString();
                    else
                        tbP1.Text = "";
                }));
                tbP2.Invoke(new EventHandler(delegate
                {
                    if (p2 > 10)
                        tbP2.Text = "После трассы " + (p2 - 10).ToString();
                    else if (p2 > 0)
                        tbP2.Text = "На трассе " + p2.ToString();
                    else
                        tbP2.Text = "";
                }));
            }
            catch { }
        }

        private void setClimber(int route, int start)
        {
            try
            {
                int r = route, p1, p2, rn;
                int curS = start;
                bool needOneStep = false;
                GetPauses(this.listID, cn, out p1, out p2, out rn);
                while (r > 1)
                {
                    r--;
                    int tmp;
                    if (p1 == 10 + r || p2 == 10 + r || needOneStep)
                    {
                        tmp = GetNextClimber(curS, listID, cn);
                        if (tmp < 0)
                            curS++;
                        else
                            curS = tmp;
                    }
                    else
                    {
                        tmp = GetNextClimber(GetNextClimber(curS, listID, cn), listID, cn);
                        if (tmp < 0)
                            curS += 2;
                        else
                            curS = tmp;
                    }
                    needOneStep = (p1 == r || p2 == r);
                }
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE lists SET nowClimbing = " + curS.ToString() + " WHERE iid=" + listID.ToString();
                cmd.ExecuteNonQuery();
                frstRoute = curS;
            }
            catch (Exception ex) { MessageBox.Show("Не удалось установить параметры бегущей строки:\r\n" + ex.Message); }
        }

        bool isClosed = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!stopServer())
            {
                e.Cancel = true;
                return;
            }
            if (lowChanged || highChanged)
            {
                DialogResult dgr = MessageBox.Show("Сохранить изменения?",
                    this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dgr)
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
                        return;
                }
            }
            isClosed = !e.Cancel;
            if (!e.Cancel && StaticClass.TheOnlyTimer != null)
                StaticClass.TheOnlyTimer.timerEvent -= ResultListBoulder_RotationEvent;

            if (!e.Cancel)
                StaticClass.SetNowClimbingNull(cn, listID);
            if (!e.Cancel)
            {
                if (StaticClass.TheOnlyTimer != null && StaticClass.TheOnlyTimer.AllowDispose)
                {
                    StaticClass.TheOnlyTimer.Dispose();
                    StaticClass.TheOnlyTimer = null;
                }
            }
            if (hoster != null)
                try { hoster.Stop(); }
                catch { }
            hoster = null;
            GC.Collect();
            base.OnClosing(e);
        }

        private void dgRes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            highChanged = true;
        }

        private void dgStart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            lowChanged = true;
        }

        private void btnStartListening_Click(object sender, EventArgs e)
        {
            if (isListening)
                stopServer();
            else
                startServer();
        }

        private bool stopServer()
        {
            if (isListening)
            {
                if (MessageBox.Show("Вы уверены что хотите остановить Bluetooth сервер?",
                                "Выключение сервера",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Exclamation) ==
                                DialogResult.No)
                    return false;
                btnStartListening.Text = "Запустить сервер";
                tbComPortNum.ReadOnly = false;
                isListening = false;
                sp.Close();
                try { thr.Join(1000); }
                catch { }
            }
            return true;
        }
        private void startServer()
        {
            if (tbComPortNum.Text == "")
            {
                MessageBox.Show("Введите номер СОМ порта");
                return;
            }
            try
            {
                sp = new SerialPort("COM" + tbComPortNum.Text);
                sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия СОМ порта\r\n" + ex.Message);
                return;
            }
            btnStartListening.Text = "Остановить сервер";
            tbComPortNum.ReadOnly = true;
            isListening = true;
            thr = new Thread(listen);
            thr.Start();
            SaveDataToRegistry();
        }

        string remaining = "";

        private void listen()
        {
            while (isListening)
            {
                try
                {
                    if (sp.BytesToRead > 0 || remaining.Length > 0)
                    {
                        if (sp.BytesToRead > 0)
                            remaining += sp.ReadExisting();
                        string str;
                        int i = remaining.IndexOf('@');
                        if (i < 0)
                        {
                            str = remaining;
                            remaining = "";
                        }
                        else
                        {
                            str = remaining.Substring(0, i);
                            remaining = remaining.Substring(i + 1);
                        }
                        str = str.Substring(1);
                        str = StaticClass.DecodeString(str);
                        //MessageBox.Show("received: " + str);
                        byte[] b = process(str);
                        if (b != null)
                            sp.Write(b, 0, b.Length);
                    }
                    else
                        Thread.Sleep(100);
                }
                catch { Thread.Sleep(100); }

            }
        }

        int clmCount = 0;

        private byte[] process(string s)
        {
            int route;
            try
            {
                route = Climber.getFieldInt(s, "ROUTE");
                if (route < 1)
                    return null;
            }
            catch { return null; }
            int i = s.IndexOf("PUT");
            string str = "";
            if (i > -1)
            {
                str = processPUT(s, route);
            }
            else
            {
                i = s.IndexOf("GET");
                if (i > -1)
                    str = processGET(s, route);
                else
                {
                    i = s.IndexOf("TEST");
                    if (i > -1)
                        str = "PASSED";
                }
            }
            if (str == "")
                return null;
            str = StaticClass.EncodeString(str);
            byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] retVal = new byte[b.Length + 1];
            retVal[0] = (byte)b.Length;

            for (int k = 0; k < b.Length; k++)
            {
                retVal[1 + k] = b[k];
            }
            return retVal;
        }

        private void insertToStart(object top, object bonus,
                                   bool nya, bool disq, int route, int index)
        {
            mStart.WaitOne();
            DataRow dr = dtStart.Rows[index];
            dr["T" + route.ToString()] = top;
            dr["B" + route.ToString()] = bonus;
            int lst = dtStart.Columns.Count - 1;
            dr[lst] = disq;
            dr[lst - 1] = nya;
            dgStart.Invoke(new EventHandler(delegate
            {
                dgStart.DataSource = dtStart;
            }));
            mStart.ReleaseMutex();
            dgStart.Invoke(new EventHandler(delegate
            {
                dgRes.Invoke(new EventHandler(delegate
                {
                    UpdateDB();
                }));
            }));
        }
        private void insertToRes(object top, object bonus,
                                   bool nya, bool disq, int route, int index)
        {
            mRes.WaitOne();
            DataRow dr = dtRes.Rows[index];
            dr["T" + route.ToString()] = top;
            dr["B" + route.ToString()] = bonus;
            int lst = dtRes.Columns.Count - 1;
            dr[lst] = disq;
            dr[lst - 1] = nya;
            dgRes.Invoke(new EventHandler(delegate
            {
                dgRes.DataSource = dtRes;
            }));
            mRes.ReleaseMutex();
            dgRes.Invoke(new EventHandler(delegate
            {
                dgStart.Invoke(new EventHandler(delegate
                {
                    UpdateDB();
                }));
            }));

        }

        private string processPUT(string s, int route)
        {
            //MessageBox.Show("data to put: " + s);
            int id = Climber.getFieldInt(s, "IID");
            int ind = 0;
            object top = getObject(s, "TOP");
            object bonus = getObject(s, "BONUS");
            bool nya = Climber.getFieldBool(s, "NYA");
            bool disq = Climber.getFieldBool(s, "DISQ");
            try
            {
                foreach (DataRow dr in dtStart.Rows)
                {
                    if (Convert.ToInt32(dr[1]) == id)
                    {
                        insertToStart(top, bonus, nya, disq, route, ind);
                        return "SUCCESS";
                    }
                    ind++;
                }
                ind = 0;
                foreach (DataRow dr in dtRes.Rows)
                {
                    if (Convert.ToInt32(dr[1]) == id)
                    {
                        insertToRes(top, bonus, nya, disq, route, ind);
                        return "SUCCESS";
                    }
                    ind++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording. route=" +
                                route.ToString() + "; climber=" +
                                id.ToString() + "\r\n" + ex.Message);
            }
            return "FAILURE";
            /*SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            
            string iid = Climber.getFieldInt(s, "IID").ToString();
            cmd.CommandText = "UPDATE boulderResults SET "+
            	"nya = @n, disq = @d, top"+route.ToString() + " = @t, bonus"+
            	route.ToString() + " =@b WHERE climber_id = "+iid+ 
            	" AND list_id = " + listID.ToString();
            cmd.Parameters.Add("@t", SqlDbType.Int);
            cmd.Parameters[0].Value = top;
            cmd.Parameters.Add("@b", SqlDbType.Int);
            cmd.Parameters[1].Value = bonus;
            cmd.Parameters.Add("@n", SqlDbType.Int);
            cmd.Parameters[2].Value = nya;
            cmd.Parameters.Add("@d", SqlDbType.Int);
            cmd.Parameters[3].Value = disq;
            try {
            	if(cn.State != ConnectionState.Open)
            		cn.Open();
            	cmd.ExecuteNonQuery();
            	
            } catch(Exception ex) {
            	MessageBox.Show("Error in updating data. Route="+
            	                route.ToString()+"; Climber="+iid+"\r\n"+
            	                ex.Message);
            }*/
        }

        private object getObject(string s, string f)
        {
            int n = Climber.getFieldInt(s, f);
            if (n >= 0)
                return (object)n;
            else
                return DBNull.Value;
        }

        private Climber getWithParam(string param, int route)
        {
            Climber c = new Climber();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.CommandText =
                "SELECT TOP 1 B.climber_id, B.start, B.top" + route.ToString() + ", " +
                "       B.bonus" + route.ToString() + ", B.nya, B.disq, " +
                "       P.name, P.surname, T.name team" +
                "  FROM boulderResults B(NOLOCK) " +
                "  JOIN Participants P(NOLOCK) ON P.iid = B.climber_id " +
                "  JOIN Teams T(NOLOCK) ON T.iid = P.team_id " +
                " WHERE B.list_id = " + listID.ToString() +
                "   AND " + param;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    if (rdr.Read())
                    {
                        c.iid = Convert.ToInt32(rdr["climber_id"]);
                        c.start = Convert.ToInt32(rdr["start"]);
                        if (rdr["top" + route.ToString()] != DBNull.Value)
                        {
                            try { c.top = Convert.ToInt32(rdr["top" + route.ToString()]); }
                            catch
                            {
                                c.top = 0;
                                c.notClimbed = true;
                            }
                        }
                        else
                        {
                            c.top = 0;
                            c.notClimbed = true;
                        }
                        if (rdr["bonus" + route.ToString()] != DBNull.Value)
                        {
                            try { c.bonus = Convert.ToInt32(rdr["bonus" + route.ToString()]); }
                            catch
                            {
                                c.bonus = 0;
                                c.notClimbed = true;
                            }
                        }
                        else
                        {
                            c.bonus = 0;
                            c.notClimbed = true;
                        }

                        c.nya = Convert.ToBoolean(rdr["nya"]);
                        c.disq = Convert.ToBoolean(rdr["disq"]);
                        if (c.nya || c.disq)
                            c.notClimbed = false;
                        c.name = rdr["name"].ToString();
                        c.surname = rdr["surname"].ToString();
                        c.team = rdr["team"].ToString();
                    }
                    else
                    {
                        c.name = "Нет такого участника";
                        c.empty = true;
                    }
                }
                finally
                {
                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                c.name = ex.Message;
                c.empty = true;
            }
            return c;
        }

        private Climber getByIid(int iid, int route)
        {
            return getWithParam("B.climber_id = " + iid.ToString(), route);
        }

        private Climber getByStart(int start, int route)
        {
            return getWithParam("B.start = " + start.ToString(), route);
        }

        private Climber getNext(int route)
        {
            Climber c = getWithParam("B.top" + route.ToString() + " IS NULL " +
                                "      AND B.nya = 0 AND B.disq = 0 " +
                                " ORDER BY B.start ", route);
            if (c.iid == 0 && c.start == 0)
                c = getByStart(1, route);
            return c;
        }

        private string processGET(string s, int route)
        {
            int i = s.IndexOf("NEXT");
            string str = null;
            Climber c = new Climber();
            clmCount++;
            c.iid = clmCount;
            c.start = clmCount;
            if (i > -1)
            {
                c = getNext(route);
            }
            else
            {
                i = Climber.getFieldInt(s, "IID");
                if (i > 0)
                    c = getByIid(i, route);
                else
                {
                    i = Climber.getFieldInt(s, "START");
                    if (i > 0)
                        c = getByStart(i, route);
                    else
                        str = "INCORRECT_ENTRY";
                }
            }
            if (str == null)
                str = c.getResponse(true);
            return str;
        }

        public static int GetNextClimber(int current, int list_id, SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT MIN(start) FROM boulderResults(NOLOCK) " +
                                  " WHERE list_id=" + list_id.ToString() +
                                  "   AND start > " + current.ToString() +
                                  "   AND nya=0 AND disq=0";
                object oRes = cmd.ExecuteScalar();
                if (oRes == null || oRes == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(oRes);
            }
            catch { return -1; }
        }

        public static int GetPrevClimber(int current, int list_id, SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT MAX(start) FROM boulderResults(NOLOCK) " +
                                  " WHERE list_id=" + list_id.ToString() +
                                  "   AND start < " + current.ToString() +
                                  "   AND nya=0 AND disq=0";
                object oRes = cmd.ExecuteScalar();
                if (oRes == null || oRes == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(oRes);
            }
            catch { return -1; }
        }

        private static void GetPauses(int list_id, SqlConnection cn, out int p1, out int p2, out int n)
        {
            p1 = p2 = n = -1;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT ISNULL(routeNumber,-1) rn FROM lists(NOLOCK) WHERE iid=" + list_id.ToString();
                n = Convert.ToInt32(cmd.ExecuteScalar());
                if (n > 0)
                {
                    cmd.CommandText = "SELECT ISNULL(nowClimbingTmp,-1) p1, ISNULL(nowClimbing3,-1) p2 FROM lists(NOLOCK) WHERE iid=" + list_id.ToString();
                    SqlDataReader dr = cmd.ExecuteReader();
                    try
                    {
                        if (dr.Read())
                        {
                            p1 = Convert.ToInt32(dr["p1"]);
                            if (p1 < 1 || (p1 > n && p1 < 11) || p1 > (10 + n))
                                p1 = -1;
                            p2 = Convert.ToInt32(dr["p2"]);
                            if (p2 < 1 || (p2 > n && p2 < 11) || p2 > (10 + n))
                                p2 = -1;
                        }
                    }
                    finally { dr.Close(); }
                }
            }
            catch { }
        }

        public static int GetClimberAtRoute(int route, int list_id, out bool incedent, SqlConnection cn)
        {
            incedent = false;
            if (route < 1)
                return -1;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                int n, p1, p2, rN;
                GetPauses(list_id, cn, out p1, out p2, out rN);
                if (route == 1)
                {
                    cmd.CommandText = "SELECT ISNULL(nowClimbing,-1) ncl FROM lists(NOLOCK) WHERE iid=" + list_id.ToString();
                    n = Convert.ToInt32(cmd.ExecuteScalar());
                    incedent = (route == p1 || route == p2);
                    return n;
                }
                else if (route > 1 && route <= rN)
                {
                    bool bTmp;
                    n = GetClimberAtRoute(route - 1, list_id, out bTmp, cn);
                    if (n < 0)
                        return -1;
                    if (p1 == (route - 1) + 10 || p2 == (route - 1) + 10)
                        n = GetPrevClimber(n, list_id, cn);
                    else if (p1 == route || p2 == route)
                    {
                        n = GetPrevClimber(n, list_id, cn);
                        incedent = true;
                    }
                    else
                        n = GetPrevClimber(GetPrevClimber(n, list_id, cn), list_id, cn);
                    return n;
                }
                else
                    return -1;
            }
            catch { return -1; }
        }

        public static string GetClimberData(int list_id, int start, SqlConnection cn)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT p.name + ' ' + p.surname " +
                                  "  FROM Participants p(NOLOCK) " +
                                  "  JOIN boulderResults br(NOLOCK) ON br.climber_id = p.iid " +
                                  " WHERE br.list_id = " + list_id.ToString() +
                                  "   AND br.start = " + start.ToString();
                return cmd.ExecuteScalar().ToString();
            }
            catch { return ""; }
        }

        public static string getNowClStr(int list_id, SqlConnection cn)
        {
            string str = "";
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT ISNULL(routeNumber,-1) rn FROM lists(NOLOCK) WHERE iid = " + list_id.ToString();
                int rN = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= rN; i++)
                {
                    bool inc;
                    int curCl = GetClimberAtRoute(i, list_id, out inc, cn);
                    if (i > 0 && !inc)
                    {
                        string strClm = GetClimberData(list_id, curCl, cn);
                        if (strClm.Length > 0)
                        {
                            if (str.Length > 0)
                                str += "; ";
                            str += "Трасса " + i.ToString() + ": " + strClm + " (Ст.№ " + curCl.ToString() + ")";
                        }
                    }
                }
            }
            catch { }
            return str;
        }

        private void btnAddP1_Click(object sender, EventArgs e)
        {
            string val, col;
            if (sender == btnAddP1)
            {
                val = tbP1.Text;
                col = "nowClimbingTmp";
            }
            else
            {
                val = tbP2.Text;
                col = "nowClimbing3";
            }
            int route;
            if (val.Length == 0)
                route = -1;
            else
            {
                int diff;
                if (val.ToLower().IndexOf("после") > -1)
                    diff = 10;
                else
                    diff = 0;
                if (!int.TryParse(getOnlyDigits(val), out route))
                {
                    MessageBox.Show("Трасса введена неверно");
                    return;
                }
                if (route < 1 || route > routeNumber)
                {
                    MessageBox.Show("Трасса введена неверно");
                    return;
                }
                route += diff;
            }
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE lists SET " + col + " = " + route.ToString();
                cmd.ExecuteNonQuery();
                setNowClStr(cn);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка добавления скользящей паузы:\r\n" + ex.Message); }
        }
        private static string getOnlyDigits(string src)
        {
            string retVal = "";
            for (int i = 0; i < src.Length; i++)
                if (char.IsDigit(src, i))
                    retVal += src[i];
            return retVal;
        }

        private void cbUseSystem_CheckedChanged(object sender, EventArgs e)
        {
            tbCOMportSys.Enabled = !cbUseSystem.Checked;
        }

        private bool portOpen = false;
        private PBHoster hoster = null;
        private void btnStartP_Click(object sender, EventArgs e)
        {
            if (!portOpen)
            {
                int portP;
                if (!int.TryParse(tbPocketPort.Text, out portP))
                    portP = -1;
                if (portP <= 0)
                {
                    MessageBox.Show(this, "Порт введён неверно");
                    return;
                }
                try
                {
                    if (hoster != null)
                        try { hoster.Stop(); }
                        catch { }
                    hoster = new PBHoster(portP);
                    hoster.PBHosterEvent += new PBHoster.PBHosterEventHandler(hoster_PBHosterEvent);
                    portOpen = true;
                    btnStartP.Text = "Stop";
                    tbPocketPort.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        "Ошибка запуска системы считывания данных с КПК:\r\n" +
                        ex.Message);
                }
            }
            else
            {
                try { hoster.Stop(); }
                catch { }
                hoster = null;
                GC.Collect();
                btnStartP.Text = "Start";
                portOpen = false;
            }
        }

        void hoster_PBHosterEvent(object sender, PBHoster.PBHosterEventArgs e)
        {
            PBHoster h = (PBHoster)sender;
            switch (e.Cmd)
            {
                case PBCommandType.GETLIST:
                    h.SetString(this.gName + " " + round, e);
                    break;
                case PBCommandType.GETNEXT:
                    h.SetClimber((PBClimber)getNext(e.Route), e);
                    break;
                case PBCommandType.GETBYIID:
                    h.SetClimber((PBClimber)getByIid(e.Climber.Iid, e.Route), e);
                    break;
                case PBCommandType.GETBYSTART:
                    h.SetClimber((PBClimber)getByStart(e.Climber.Start, e.Route), e);
                    break;
                case PBCommandType.SETRES:
                    h.SetBoolean(processPUT(e.Climber.ToString(), e.Route) == "SUCCESS",
                        e);
                    break;
            }
        }

        private void ResultListBoulder_Load(object sender, EventArgs e)
        {
            if (needToClose)
                try { this.Close(); }
                catch { }
        }

#endif
    }

    public class Climber
    {
#if FULL
        public bool empty = false;
        public bool notClimbed = false;
        public string surname = "";
        public string name = "";
        public string team = "";
        public int start = 0;
        public int iid = 0;
        public int top = 0;
        public int bonus = 0;
        public bool disq = false;
        public bool nya = false;

        public static Climber createClimber(string response)
        {
            Climber retVal = new Climber();
            retVal.surname = Climber.getField(response, "LAST");
            retVal.name = Climber.getField(response, "FIRST");
            retVal.team = Climber.getField(response, "TEAM");
            retVal.iid = Climber.getFieldInt(response, "IID");
            retVal.start = Climber.getFieldInt(response, "START");
            retVal.top = Climber.getFieldInt(response, "TOP");
            retVal.bonus = Climber.getFieldInt(response, "BONUS");
            retVal.disq = Climber.getFieldBool(response, "DISQ");
            retVal.nya = Climber.getFieldBool(response, "NYA");
            return retVal;
        }

        public string getResponse(bool fullResponse)
        {
            string s = Climber.setField("IID", iid) + Climber.setField("TOP", top) +
                    Climber.setField("BONUS", bonus) + Climber.setField("NYA", nya) +
                    Climber.setField("DISQ", disq);
            if (fullResponse)
            {
                s += Climber.setField("LAST", surname) +
                        Climber.setField("FIRST", name) + Climber.setField("TEAM", team) +
                        Climber.setField("START", start);
            }
            return s;
        }

        public static string getField(string response, string field)
        {
            int i;
            int j;
            string s;
            string f = field + "=";
            i = response.IndexOf(f);
            if (i > -1)
            {

                j = response.IndexOf(";", i);
                if (j > -1)
                {
                    try
                    {
                        s = response.Substring(i + f.Length, j - (i + f.Length));
                    }
                    catch
                    {
                        s = "";
                    }
                }
                else
                    s = "";
            }
            else
                s = "";
            return s;
        }

        public static int getFieldInt(string response, string f)
        {
            string s = getField(response, f);
            if (s != "")
            {
                try
                {
                    return int.Parse(s);
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }

        public static bool getFieldBool(string response, string f)
        {
            return (getFieldInt(response, f) > 0);
        }

        public static string setField(string field, string value)
        {
            return field + "=" + value + ";";
        }

        public static string setField(string field, int value)
        {
            return setField(field, value.ToString());
        }

        public static string setField(string field, bool value)
        {
            int k;
            if (value)
                k = 1;
            else
                k = 0;
            return setField(field, k);
        }

        public static explicit operator PBClimber(Climber src)
        {
            PBClimber clm;
            if (src.name.Length < 1 && src.surname.Length < 1 || src.empty)
                clm = PBClimber.GetEmpty();
            else
            {
                clm = new PBClimber((ushort)src.iid, src.surname + (src.name.Length > 0 ? " " + src.name : ""),
                     src.team, (ushort)src.start);
                if (src.nya)
                    clm.Res = PBClimber.ResType.NYA;
                else if (src.disq)
                    clm.Res = PBClimber.ResType.DSQ;
                else if (src.notClimbed)
                    clm.Res = PBClimber.ResType.NOT_CLIMBED;
                else
                {
                    clm.Res = PBClimber.ResType.RES;
                    clm.Top = src.top;
                    clm.Bonus = src.bonus;
                }
            }
            return clm;
        }
#endif
    }
}