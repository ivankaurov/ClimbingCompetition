using ClimbingCompetition.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Validate;
using XmlApiClient;
using Word = Microsoft.Office.Interop.Word;

namespace ClimbingCompetition
{
    /// <summary>
    /// Основная форма приложения
    /// </summary>
    public partial class MainForm : StaticClass.ExtendedForm
    {
        private SqlConnection cn;
        private bool isConnected = false;
        private string dir = "", cTitle = "";

        public string MF_NOT_CONNECTED = "Нет соединения с БД";
        public string MF_DISCONNECT_MENU = "&Отсоединиться от базы данных" + '\n' + "Смена данных подключения";
        public string MF_CONNECT_MENU = "&Подключиться к базе данных";
        public string IMG_SAV_F = "Идёт загрузка фотографий в папку; ";
        public string IMG_LOAD_F = "Идёт загрузка фотографий из папки; ";
        public string IMG_SAV_REM = "Идёт загрузка фотографий на удалённый сервер; ";
        public string IMG_LOAD_REM = "Идёт загрузка фотографий с удалённого сервера; ";
        public string UPD_REM = "Идёт обновление сайта; ";
        public string MF_STOP_TR_MENU = "Остановить трансляцию";
        public string MF_START_TR_MENU = "Запустить трансляцию";

        int TimeRemaining = 30;

        bool AllowWork;
        bool needToExit = false;

        private void LoadInitialCulture()
        {
            CultureInfo ci;
            try
            {
#if DEBUG
                ci = new CultureInfo(ClComp.Default.Language);
#else
                ci = new CultureInfo("ru-RU");
#endif
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
            }
            catch { ci = Thread.CurrentThread.CurrentUICulture; }
            LoadLocalizedStrings(ci);
        }

        
        public override void  LoadLocalizedStrings(CultureInfo ci)
        {
            StaticClass.LoadInitialStrings("ClimbingCompetition.LocalizedStrings", this, ci);
        }

        public MainForm()
        {
            LoadInitialCulture();

            InitializeComponent();
            cn = new SqlConnection();
            SetButtons();
            AllowWork = ValidationClass.AllowExecute(out TimeRemaining, true);

            needToExit = CntNew(AllowWork);
            StaticClass.Trial = ValidationClass.IsTrial;
            StaticClass.TrialLeft = TimeRemaining;

#if !DEBUG

            onlineTestToolStripMenuItem.Visible = imageTestToolStripMenuItem.Visible =
                ukranianToolStripMenuItem.Visible = ukranianToolStripMenuItem.Enabled =
                englishToolStripMenuItem.Visible = englishToolStripMenuItem.Enabled =
                onlineTestToolStripMenuItem.Enabled = imageTestToolStripMenuItem.Enabled = false;
#endif
        }

        private void GetSerial()
        {
            return;
        }

        public static string GetDir()
        {
            return System.Threading.Thread.GetDomain().BaseDirectory;
        }
        
        private bool CntNew(bool connect)
        {
            bool needToExit = false;
            StaticClass.currentAccount = StaticClass.AccountInfo.NONE;
            if (connect)
            {
                AccountForm fr = new AccountForm(dir);
                fr.ShowDialog();
                needToExit = fr.ExitPressed;
                if (fr.ConnectionString == "")
                {
                    isConnected = false;
                    this.Text = MF_NOT_CONNECTED;
                }
                else
                {
                    cn.ConnectionString = fr.ConnectionString;
                    try
                    {
                        cn.Open();
                        isConnected = true;
                        //dir = fr.GetDir;
                        dir = GetDir();
                        CheckAdmin(fr.GetAdminPwd);
                        competitionTitle = GetCompTitle();
                        try
                        {
                            ServiceClient.GetInstance(cn);
                            OnlineUpdater2.GetUpdater(cn);

                            OnlineUpdater2.Instance.UpdateStarted += (s, e) =>
                            {
                                statusStrip1.Invoke(new EventHandler(delegate
                                {
                                    tsLabel.Text = "Идет обновление сайта";
                                }));
                            };

                            OnlineUpdater2.Instance.UpdateFinished += (s, e) =>
                            {
                                statusStrip1.Invoke(new EventHandler(delegate
                                {
                                    tsLabel.Text = string.Empty;
                                }));
                            };

                            OnlineUpdater2.Instance.UpdateFailed += (s, e) =>
                            {
                                this.Invoke(new EventHandler(delegate
                                {
                                    var dRes = MessageBox.Show(this,
                                        string.Format("Ошибка обновления сайта:{0}{1}{0}{0}Продолжить обновлять сайт?", Environment.NewLine, e.Data), String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (dRes == DialogResult.Yes)
                                        e.Handle();
                                }));
                            };

                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = cn;
                            bool needToBind = !SortingClass.CheckColumn("CompetitionData", "DB_ID", "VARCHAR(255) NOT NULL DEFAULT '" + AccountForm.DB_ID + "'", cn);
                            if (!needToBind)
                            {
                                cmd.CommandText = "SELECT DB_ID FROM CompetitionData(NOLOCK)";
                                needToBind = (cmd.ExecuteScalar().ToString() != AccountForm.DB_ID);
                            }
                            if (needToBind)
                                if (MessageBox.Show(this, "Данная БД предназначена для работы в другой версии программы.\r\n" +
                                    "Изменить привязку БД?", "Неправильная БД", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    cmd.CommandText = "UPDATE CompetitionData SET DB_ID='" + AccountForm.DB_ID + "'";
                                    cmd.ExecuteNonQuery();
                                }
                            AccountForm.CreateLogData(cn);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Не удалось проверить/создать/обновить привязку БД к версии программы\r\n" +
                                ex.Message + "\r\nПравильная работа программы не гарантируется");
                        }
                        try { this.Text = StaticClass.ParseCompTitle(competitionTitle)[0] + " [" + fr.GetServ + " // " + fr.GetDB + "]"; }
                        catch
                        {
                            this.Text = "Выбранная база данных не соответствует версии программы";
                            MessageBox.Show(this.Text);
                            //listsToolStripMenuItem.Enabled = false;
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        isConnected = false;
                        this.Text = MF_NOT_CONNECTED;
                    }
                }
            }
            else
            {
                this.Text = MF_NOT_CONNECTED;
                foreach (Form frm in MdiChildren)
                    frm.Close();
                cn.Close();

                isConnected = false;
            }
            SetButtons();
            return needToExit;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (AllowWork)
            {
                CntNew(false);
                if (CntNew(true))
                    this.Close();
            }
            else
                MessageBox.Show("Пробный период истёк.");
        }

        private void SetButtons()
        {
            listsToolStripMenuItem.Enabled = (this.Text.IndexOf("не соответствует") < 0);
            if (isConnected)
            {
                connectToolStripMenuItem.Text = MF_DISCONNECT_MENU;
                listsToolStripMenuItem.Enabled = true;
                dataToolStripMenuItem.Visible = judgesToolStripMenuItem.Visible = deleteAllToolStripMenuItem.Visible =
                    changeDataToolStripMenuItem.Visible = onlineToolStripMenuItem.Visible = reportsToolStripMenuItem.Visible =
                    settingsToolStripMenuItem.Visible =
                    (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY);
                if (StaticClass.currentAccount == StaticClass.AccountInfo.SECRETARY)
                {
                    SortingClass.CheckColumn("CompetitionData", "remoteString", "VARCHAR(255)", cn);
                    SortingClass.CheckColumn("CompetitionData", "remoteTime", "INT", cn);
                    SqlCommand cmd = new SqlCommand(
                        "SELECT remoteString, remoteTime FROM CompetitionData(NOLOCK)", cn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                        rcString = rdr["remoteString"].ToString();
                    rdr.Close();
                }
                clearFormDataToolStripMenuItem.Visible = true;
            }
            else
            {
                connectToolStripMenuItem.Text = MF_CONNECT_MENU;
                deleteAllToolStripMenuItem.Visible = changeDataToolStripMenuItem.Visible = listsToolStripMenuItem.Enabled =
                    judgesToolStripMenuItem.Visible = dataToolStripMenuItem.Visible = reportsToolStripMenuItem.Visible =
                    clearFormDataToolStripMenuItem.Visible =
                    onlineToolStripMenuItem.Visible = settingsToolStripMenuItem.Visible = false;
                StaticClass.currentAccount = StaticClass.AccountInfo.NONE;
            }

        }

        private void btnSecretary_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                if (frm is Secretary)
                {
                    frm.Activate();
                    return;
                }
            }
            Secretary sec = new Secretary(cn, competitionTitle, dir);
            sec.Show();
            sec.MdiParent = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //if (thrDataCol.HasAlive)
            //    e.Cancel = (MessageBox.Show("Имеются незавершённые задачи. Закрыть программу?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //        == DialogResult.No);
            if (!e.Cancel)
            {
                if (OnlineUpdater2.Instance != null)
                    OnlineUpdater2.Instance.Cancel();
                thrDataCol.AbortAll();
                try { StopBroadcast(); }
                catch { }
                try { cn.Close(); }
                catch { }
            }
            base.OnClosing(e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { cn.Close(); }
            catch { }
            this.Close();
        }

        private void listsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                if (frm is All_Lists)
                {
                    frm.Close();
                    break;
                }
            }
            All_Lists al = new All_Lists(cn, dir + '\\', competitionTitle);
            al.MdiParent = this;
            al.Show();
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dg = MessageBox.Show("Вы уверены, что хотите очистить базу данных?" +
                '\n' + "Все данные при этом будут уничтожены без возможности восстановления",
                "Очистить Базу Данных?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (dg == DialogResult.No)
                return;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.Transaction = cn.BeginTransaction();

            try
            {
                cmd.CommandText = "DELETE FROM speedResults_Runs";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM teamResults";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM generalResults";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM routeResults";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM speedResults";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM boulderResults";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM lists_hdr2";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM lists";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM Participants";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM Teams";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM judgePos";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM positions";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM judges";
                cmd.ExecuteNonQuery();

                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                try { cmd.Transaction.Rollback(); }
                catch { }
                MessageBox.Show("Ошибка удаления:\r\n" + ex.ToString());
            }
        }

        private void judgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Judges jdg = new Judges(this.cn, this.competitionTitle);
            jdg.MdiParent = this;
            jdg.Show();
        }

        string GetCompTitle()
        {
            string res = "\r\n\r\n\r\n";
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT NAME, SHORT_NAME, DATES, PLACE FROM CompetitionData", cn);
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                //string res = "";
                while (rdr.Read())
                {
                    res = rdr[0].ToString() + "\r\n" + rdr[3].ToString() + "\r\n" + rdr[2].ToString() + "\r\n" + rdr[1].ToString();
                    break;
                }
                rdr.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return res;
        }

        void CheckAdmin(string pwd)
        {
            SqlCommand smd = new SqlCommand("SELECT ADMIN FROM CompetitionData", cn);
            try
            {
                if (pwd == smd.ExecuteScalar().ToString())
                    StaticClass.currentAccount = StaticClass.AccountInfo.SECRETARY;
                else
                    StaticClass.currentAccount = StaticClass.AccountInfo.JUDGE;
            }
            catch { StaticClass.currentAccount = StaticClass.AccountInfo.JUDGE; }
        }

        public string competitionTitle
        {
            get { return cTitle; }
            protected set
            {
                cTitle = value;
                foreach (Form f in this.MdiChildren)
                    if (f is BaseForm)
                    {
                        BaseForm bf = (BaseForm)f;
                        bf.competitionTitle = cTitle;
                    }
            }
        }

        private void changeDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountForm fr = new AccountForm(this.cn);
            fr.ShowDialog();
            competitionTitle = GetCompTitle();
            try { this.Text = StaticClass.ParseCompTitle(competitionTitle)[0] + " [" + fr.GetServ + " // " + fr.GetDB + "]"; }
            catch
            {
                this.Text = "Выбранная база данных не соответствует версии программы";
                MessageBox.Show(this.Text);
                //listsToolStripMenuItem.Enabled = false;
            }
        }

        string rcString = "";
        //bool fullReload = true;
        private bool SetBroadcast(/*out bool fRel*/)
        {
            //fRel = false;
            if (!ServiceClient.GetInstance(cn).ConnectionSet)
            {
                MessageBox.Show("Соединение не настроено");
                return false;
            }
            OnlineBroadcast obc = new OnlineBroadcast(cn, this);
            obc.ShowDialog();
            //fRel = obc.fullReload;
            return obc.StartBroadcast;
        }
        private String remoteConnectionString = String.Empty;

        private void startBroadcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (startBroadcastToolStripMenuItem.Text.Equals(MF_START_TR_MENU))
                {
                    if (SetBroadcast(/*out fullReload*/))
                        StartBroadcast();
                }
                else
                {
                    try { StopBroadcast(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void StartBroadcast()
        {
            OnlineUpdater2.Instance.Start();
            return;
            if (rcString == "")
            {
                MessageBox.Show("Соединение не настрено");
                return;
            }
            startBroadcastToolStripMenuItem.Text = MF_STOP_TR_MENU;

            
            TmeWorkForm tm = new TmeWorkForm("Запуск трансляции", StartBroadcast_delegate, true);
            tm.MdiParent = this;
            tm.Show();
            broadcastTimer.Start();
            changeBroadcastToolStripMenuItem.Enabled = true;
        }

        private void StartBroadcast_delegate()
        {
            try
            {
                broadcastMutex.WaitOne();
                try
                {
                    if (String.IsNullOrEmpty(remoteConnectionString))
                        return;
                    SqlConnection remoteConnection;
                    XmlClientWrapper wrapper;
                    XmlClient client = null;
                    using (SqlConnection cn = new SqlConnection(this.cn.ConnectionString))
                    {
                        long? compIDForService = StaticClass.ParseServString(remoteConnectionString, out wrapper);
                        if (wrapper != null)
                        {
                            compIDForService = wrapper.CompetitionId;
                            client = wrapper.CreateClient(cn);
                            if (client != null)
                            {
                                OnlineUpdater.BeginFullUpdate(false, cn, client, null, null, OnlineUpdater.UpdateStartMode.CancelIfUpdating);
                                return;
                            }
                        }
                        if (compIDForService == null)
                            remoteConnection = new SqlConnection(remoteConnectionString);
                        else
                            remoteConnection = null;

                        try { StaticClass.UpdateOnline(cn, remoteConnection, compIDForService, client); }
                        finally
                        {
                            try
                            {
                                if (remoteConnection != null && remoteConnection.State != ConnectionState.Closed)
                                    remoteConnection.Close();
                            }
                            catch { }
                        }
                    }
                }
                finally { broadcastMutex.ReleaseMutex(); }
                
                //Thread thr = new Thread(new ThreadStart(delegate
                //{
                //    this.Invoke(new EventHandler(delegate
                //    {
                //        MessageBox.Show(this, "Трансляция успешно запущена.");
                //    }));
                //}));
                //thr.Start();
            }
            catch { }
        }

        private void StopBroadcast()
        {
            OnlineUpdater2.Instance.Cancel();
            return;
            try
            {
                if (!broadcastTimer.Enabled)
                    return;
                broadcastMutex.WaitOne(10000);
                try
                {
                    broadcastTimer.Stop();
                    if (String.IsNullOrEmpty(remoteConnectionString))
                        return;
                    XmlClientWrapper wrapper;
                    XmlClient client = null;
                    var compID = StaticClass.ParseServString(remoteConnectionString, out wrapper);
                    if (wrapper != null)
                    {
                        compID = wrapper.CompetitionId;
                        client = wrapper.CreateClient(cn);
                    }
                    if (compID == null)
                    {
                        SqlConnection remote = new SqlConnection(remoteConnectionString);
                        remote.Open();
                        try
                        {
                            SqlCommand cmd = new SqlCommand("UPDATE ONLlists SET live=0", remote);
                            cmd.ExecuteNonQuery();
                        }
                        finally { remote.Close(); }
                    }
                    else
                    {
                        //TODO: СЛУЖБАМИ
                    }
                }
                finally { broadcastMutex.ReleaseMutex(); }
                startBroadcastToolStripMenuItem.Text = MF_START_TR_MENU;
                changeBroadcastToolStripMenuItem.Enabled = false;
                MessageBox.Show("Online траснсляция успешно остановлена");
            }
            catch { }
        }
        Mutex broadcastMutex = new Mutex();
        private void broadcastTimer_Tick(object sender, EventArgs e)
        {
            //if (thr!= null && thr.IsAlive)
            //    try { thr.Join(60000); }
            //    catch { }
            //thr = new Thread(UpdateSite_Delegate);
            //thr.Start();
            //return;
            /*TmeWorkForm tm = new TmeWorkForm("Обновление сайта", UpdateSite_Delegate, true);
            this.Invoke(new EventHandler(delegate
            {
                tm.MdiParent = this;
                tm.Show();
            }));*/
            ThrData tdUpdOnl = new ThrData(new Thread(UpdateSite_Delegate), ThreadStats.UPD_REM);
            if (AddThrData(tdUpdOnl))
                tdUpdOnl.thr.Start(new AfetrStopCallBack(RemThrData));
        }

        private void UpdateSite_Delegate(object arg)
        {
            //Thread.Sleep(10000);
            AfetrStopCallBack callback;
            if (arg is AfetrStopCallBack)
                callback = (AfetrStopCallBack)arg;
            else
                callback = null;
            bool threadAbort = false;
            try
            {
                broadcastMutex.WaitOne();
                try
                {
                    SqlConnection local = new SqlConnection(cn.ConnectionString);
                    local.Open();
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = local;
                        cmd.CommandText = "SELECT ISNULL(remoteString,'') FROM CompetitionData(NOLOCK)";
                        remoteConnectionString = cmd.ExecuteScalar() as string;
                        if (String.IsNullOrEmpty(remoteConnectionString))
                            return;
                        XmlClientWrapper wrapper;
                        XmlClient client = null;
                        long? compID = StaticClass.ParseServString(remoteConnectionString, out wrapper);
                        if (wrapper != null)
                        {
                            compID = wrapper.CompetitionId;
                            client = wrapper.CreateClient(local);
                        }
                        SqlConnection remote;
                        if (compID == null)
                            remote = new SqlConnection(remoteConnectionString);
                        else
                            remote = null;
                        try { StaticClass.UpdateOnline(local, remote, compID, client); }
                        finally
                        {
                            try
                            {
                                if (remote != null && remote.State != ConnectionState.Closed)
                                    remote.Close();
                            }
                            catch { }
                        }
                    }
                    finally { local.Close(); }
                }
                catch (ThreadAbortException)
                {
                    threadAbort = true;
                    return;
                }
                catch (Exception ex)
                {
                    Thread thr = new Thread(new ThreadStart(delegate
                    {
                        this.Invoke(new EventHandler(delegate
                        {
                            MessageBox.Show("Ошибка обновления удалённой БД.\r\n" + ex.Message +
                                "\r\nВремя:" + DateTime.Now.ToLongTimeString());
                        }));
                    }));
                    thr.Start();
                }
                finally { broadcastMutex.ReleaseMutex(); }
            }
            catch (ThreadAbortException)
            {
                threadAbort = true;
                return;
            }
            catch { }
            finally
            {
                if (!threadAbort && callback != null)
                    try { callback(ThreadStats.UPD_REM); }
                    catch { }
            }
        }

        private void setConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            try { StopBroadcast(); }
            catch { }

            var newForm = new NewRemoteConnectionForm(competitionTitle, this.cn);

            if (newForm.ShowDialog(this) == DialogResult.OK)
            {
                startBroadcastToolStripMenuItem.Enabled = true;
            }
            else
            {
                startBroadcastToolStripMenuItem.Enabled = false;
            }
            /*
            SetConnectionString scs = new SetConnectionString(rcString, competitionTitle, cn);
            scs.ShowDialog();
            if (scs.GetConnectionString != "")
            {
                rcString = scs.GetConnectionString;
                startBroadcastToolStripMenuItem.Enabled = true;
            }
            else
                startBroadcastToolStripMenuItem.Enabled = false;*/
            /*if (scs.TimerInt != 0)
                broadcastTimer.Interval = scs.TimerInt * 1000;*/
            broadcastTimer.Interval = 30000;

        }

        bool allowGuest;
        bool adult;

        void GetGuest()
        {
            try
            {
                if (this.cn.State != ConnectionState.Open)
                    this.cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teams(NOLOCK) WHERE Guest = 0", this.cn);
                try
                {
                    cmd.ExecuteScalar();
                    allowGuest = true;
                }
                catch { allowGuest = false; }
                cmd.CommandText = "SELECT COUNT(*) FROM Groups(NOLOCK) WHERE name LIKE '%енщин%'";
                try
                { adult = (Convert.ToInt32(cmd.ExecuteScalar()) > 0); }
                catch { adult = false; }
            }
            catch { allowGuest = false; adult = false; }
        }

        private void applToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateSecretaryRep;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        private Word.Application CreateSecretaryRep(BackgroundWorker bw)
        {
            int i;
            GetGuest();
            Word.Document doc;
            Word.Range rng;
            Word.Application app = null;
            if (StaticClass.LaunchWord(dir + "\\Templates\\mand.dot", out doc, out app))
            {
                bw.ReportProgress(15);
                rng = doc.Paragraphs[3].Range;
                rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[0] + ".\r";
                rng = doc.Paragraphs[5].Range;
                rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[1] + ".\r";
                rng = doc.Paragraphs[7].Range;
                rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[2] + '\r';

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    cmd.CommandText = "  SELECT g.name, COUNT(*) cnt " +
                                      "    FROM Participants p(NOLOCK) " +
                                      "    JOIN groups g(NOLOCK) ON g.iid = p.group_id " +
                                      "GROUP BY g.iid, g.name " +
                                      "ORDER BY g.iid ";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rng = doc.Paragraphs[15].Range;
                    char c = 'а';
                    rng.Text = "";
                    for (i = 0; i < dt.Rows.Count; i++)
                        rng.Text += "                  " + c++ + ") " + dt.Rows[i][0] + " - " + dt.Rows[i][1] + "\r\n";
                    rng.Text += "\r";

                    bw.ReportProgress(25);

                    int parIndx = 16 + dt.Rows.Count;
                    rng = doc.Paragraphs[parIndx].Range;
                    if (allowGuest)
                        cmd.CommandText = "SELECT COUNT(*) " +
                                          "  FROM Participants p(NOLOCK) " +
                                          "  JOIN Teams t(NOLOCK) ON t.iid = p.team_id " +
                                          " WHERE t.Guest = 0 ";
                    else
                        cmd.CommandText = "SELECT COUNT(*) FROM Participants(NOLOCK)";
                    string strG = cmd.ExecuteScalar().ToString();
                    string str;
                    switch (GetPadeg(strG))
                    {
                        case Padeg.SINGLE:
                            str = "Всего " + strG + " участник из ";
                            break;
                        case Padeg.PLURAL_SMALL:
                            str = "Всего " + strG + " участника из ";
                            break;
                        default:
                            str = "Всего " + strG + " участников из ";
                            break;
                    }

                    bw.ReportProgress(45);
                    if (allowGuest)
                        cmd.CommandText = "SELECT COUNT(*) FROM Teams(NOLOCK) WHERE Guest = 0";
                    else
                        cmd.CommandText = "SELECT COUNT(*) FROM Teams(NOLOCK)";
                    strG = cmd.ExecuteScalar().ToString();

                    switch (GetPadeg(strG))
                    {
                        case Padeg.SINGLE:
                            str += strG + " региона России,";
                            break;
                        default:
                            str += strG + " регионов России,";
                            break;
                    }
                    bw.ReportProgress(55);

                    //rng.Text = str;

                    strG = "";
                    if (allowGuest)
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM Teams(NOLOCK) WHERE Guest = 1";
                        strG = cmd.ExecuteScalar().ToString();
                        if (strG != "0")
                        {
                            parIndx++;
                            switch (GetPadeg(strG))
                            {
                                case Padeg.SINGLE:
                                    strG = "\r\n" + strG + " команда гостей (";
                                    break;
                                case Padeg.PLURAL_SMALL:
                                    strG = "\r\n" + strG + " команды гостей (";
                                    break;
                                case Padeg.PLURAL_BIG:
                                    strG = "\r\n" + strG + " команд гостей (";
                                    break;
                            }
                            cmd.CommandText = "SELECT COUNT(*) " +
                                              "  FROM Participants p(NOLOCK) " +
                                              "  JOIN Teams t(NOLOCK) ON t.iid = p.team_id " +
                                              " WHERE t.Guest = 1 ";
                            string strTmp = cmd.ExecuteScalar().ToString();
                            switch (GetPadeg(strTmp))
                            {
                                case Padeg.PLURAL_SMALL:
                                    //strG += strTmp + " человека) из ";
                                    strG += strTmp + " человека): ";
                                    break;
                                default:
                                    //strG += strTmp + " человек) из ";
                                    strG += strTmp + " человек): ";
                                    break;
                            }
                            cmd.CommandText = "SELECT name FROM Teams(NOLOCK) WHERE Guest = 1";
                            dt.Clear();
                            da.Fill(dt);
                            strG += dt.Rows[0][0].ToString();
                            for (i = 1; i < dt.Rows.Count; i++)
                                strG += ", " + dt.Rows[i][0].ToString();
                        }
                        else
                            strG = "";
                    }
                    strG += "\r";
                    rng.Text = str + strG;

                    bw.ReportProgress(65);

                    Word.Table t = doc.Tables[1];
                    CreateQualiTable(t, true);

                    bw.ReportProgress(90);

                    t = doc.Tables[2];
                    int k = 1;
                    k = GetForJudge(cmd, "Председатель мандатной комиссии", t, k);
                    k = GetForJudge(cmd, "Врач соревнований", t, k);
                    k = GetForJudge(cmd, "Представитель проводящей организации", t, k);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            bw.ReportProgress(100);

            return app;
        }

        private void CreateQualiTable(Word.Table t, bool diff_g)
        {
            SqlDataAdapter dat = new SqlDataAdapter(new SqlCommand(
                              "  SELECT ISNULL(genderFemale, 0) gF, ISNULL(qf, '') qff, ISNULL(COUNT(iid), 0) cnt " +
                              "    FROM Participants(NOLOCK) " +
                              "GROUP BY genderFemale, ISNULL(qf,'') " +
                              "ORDER BY qff, gF ", cn));
            DataTable dt2 = new DataTable();
            dat.Fill(dt2);

            bool nFirst = false;

            nFirst = InsertQfToTable(dt2, "3ю", "3 юн. разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "2ю", "2 юн. разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "1ю", "1 юн. разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "3", "3 разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "2", "2 разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "1", "1 разряд", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "КМС", "КМС", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "МС", "МС", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "МСМК", "МСМК", t, nFirst, diff_g);
            nFirst = InsertQfToTable(dt2, "ЗМС", "ЗМС", t, nFirst, diff_g);
        }

        private int GetForJudge(SqlCommand cmd, string str, Word.Table t, int k)
        {
            cmd.CommandText = "SELECT iid FROM judgeView(NOLOCK) WHERE pos = '" + str + "'";
            try
            {
                int ii = Convert.ToInt32(cmd.ExecuteScalar());
                t.Rows[k].Cells[2].Range.Text = StaticClass.GetJudgeData(ii, cn, false);
            }
            catch { }
            k++;
            return k;
        }

        private bool InsertQfToTable(DataTable dt, string strQf, string strLongName, Word.Table t, bool nfirst, bool diff_g)
        {
            int m, f;

            GetDataFromTable(dt, strQf, out m, out f);
            if (m > 0 || f > 0)
            {
                if (nfirst)
                {
                    object o = t.Rows[1];
                    t.Rows.Add(ref o);
                }
                nfirst = true;
                t.Rows[1].Cells[1].Range.Text = strLongName + ":";
                if (diff_g)
                {
                    if (adult)
                    {
                        switch (GetPadeg(m))
                        {
                            case Padeg.SINGLE:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " мужчина";
                                break;
                            case Padeg.PLURAL_SMALL:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " мужчины";
                                break;
                            default:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " мужчин";
                                break;
                        }
                        switch (GetPadeg(f))
                        {
                            case Padeg.SINGLE:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " женщина";
                                break;
                            case Padeg.PLURAL_SMALL:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " женщины";
                                break;
                            default:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " женщин";
                                break;
                        }
                    }
                    else
                    {
                        switch (GetPadeg(m))
                        {
                            case Padeg.SINGLE:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " юноша";
                                break;
                            case Padeg.PLURAL_SMALL:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " юноши";
                                break;
                            default:
                                t.Rows[1].Cells[2].Range.Text = m.ToString() + " юношей";
                                break;
                        }
                        switch (GetPadeg(f))
                        {
                            case Padeg.SINGLE:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " девушка";
                                break;
                            case Padeg.PLURAL_SMALL:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " девушки";
                                break;
                            default:
                                t.Rows[1].Cells[3].Range.Text = f.ToString() + " девушек";
                                break;
                        }
                    }
                }
                else
                {
                    m += f;
                    switch (GetPadeg(m))
                    {
                        case Padeg.SINGLE:
                            t.Rows[1].Cells[2].Range.Text = m.ToString() + " участник";
                            break;
                        case Padeg.PLURAL_SMALL:
                            t.Rows[1].Cells[2].Range.Text = m.ToString() + " участника";
                            break;
                        default:
                            t.Rows[1].Cells[2].Range.Text = m.ToString() + " участников";
                            break;
                    }
                }
            }
            return nfirst;
        }

        static void GetDataFromTable(DataTable dt, string qf, out int male, out int female)
        {
            male = female = 0;
            foreach (DataRow dr in dt.Rows)
                if (dr[1].ToString() == qf)
                {
                    if (Convert.ToBoolean(dr[0]))
                    {
                        female = Convert.ToInt32(dr[2]);
                        break;
                    }
                    else
                        male = Convert.ToInt32(dr[2]);
                }
        }

        Padeg GetPadeg(string num)
        {
            return GetPadeg(Convert.ToInt32(num));
        }

        Padeg GetPadeg(int num)
        {
            if (num < 0)
                throw new ArgumentException("Неверное число");
            if (num < 10)
                return GetPadegOne(num);
            if (num < 20)
                return Padeg.PLURAL_BIG;
            string sTmp = num.ToString();
            num = Convert.ToInt32(sTmp[sTmp.Length - 1].ToString());
            return GetPadegOne(num);
        }

        private static Padeg GetPadegOne(int num)
        {
            if (num == 0)
                return Padeg.PLURAL_BIG;
            if (num == 1)
                return Padeg.SINGLE;
            if (num < 5)
                return Padeg.PLURAL_SMALL;
            return Padeg.PLURAL_BIG;
        }
        enum Padeg { SINGLE, PLURAL_SMALL, PLURAL_BIG }

        private void presRepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateJuryPresidentReportNew;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        private Word.Application CreateJuryPresidentReportNew(BackgroundWorker bw)
        {
            GetGuest();
            Word.Document doc;
            Word.Application app = null;
            if (StaticClass.LaunchWord(dir + "\\Templates\\jury_rep.dot", out doc, out app))
            {
                bw.ReportProgress(2);
                //app.Visible = true;

                ReplaceLabel(doc, app, "TITLE", StaticClass.ParseCompTitle(competitionTitle)[0]);
                ReplaceLabel(doc, app, "TIME", StaticClass.ParseCompTitle(competitionTitle)[2]);
                ReplaceLabel(doc, app, "PLACE", StaticClass.ParseCompTitle(competitionTitle)[1]);

                bw.ReportProgress(8);

                //try
                //{
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teams(NOLOCK)", cn);
                int teams = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "SELECT COUNT(*) FROM Participants(NOLOCK)";
                int pCount = Convert.ToInt32(cmd.ExecuteScalar());

                ReplaceLabel(doc, app, "TEAMS_COUNT", "Команд: " + teams.ToString() + ", участников: " + pCount.ToString());


                bw.ReportProgress(10);

                SqlDataAdapter da = new SqlDataAdapter(new SqlCommand(
                    "SELECT name FROM groups(NOLOCK) ORDER BY oldYear, genderFemale", cn));
                DataTable dt = new DataTable();
                da.Fill(dt);
                string curG = "";
                string groupRow = "";
                int tmp;
                foreach (DataRow dr in dt.Rows)
                {
                    string ttt = dr[0].ToString();
                    tmp = ttt.IndexOf(' ');
                    if (tmp > -1)
                    {
                        ttt = ttt.Substring(0, tmp);
                        if (curG == ttt)
                            groupRow += " и " + dr[0].ToString().Substring(tmp + 1, dr[0].ToString().Length - tmp - 1);
                        else
                        {
                            if (groupRow != "")
                                groupRow += ", ";
                            groupRow += dr[0].ToString();
                            curG = ttt;
                        }
                    }
                    else
                    {
                        curG = "";
                        if (groupRow != "")
                            groupRow += ", ";
                        groupRow += dr[0].ToString();
                    }
                }
                ReplaceLabel(doc, app, "AGE_GROUPS", groupRow);
                bw.ReportProgress(20);

                cmd.CommandText = "SELECT DISTINCT style FROM lists(NOLOCK) WHERE style NOT LIKE 'Команд%' ORDER BY style";
                groupRow = String.Empty;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (!String.IsNullOrEmpty(groupRow))
                            groupRow += ", ";
                        groupRow += rdr[0].ToString();
                    }
                }
                ReplaceLabel(doc, app, "KINDS", groupRow);
//#if FULL
//                da.SelectCommand.CommandText = "SELECT style FROM lists(NOLOCK) WHERE style LIKE 'Команд%' ORDER BY iid DESC";
//#else
//                da.SelectCommand.CommandText = "SELECT style FROM lists(NOLOCK) WHERE style = 'Командные' ORDER BY iid DESC";
//#endif
//                dt.Clear();
//                dt.Columns.Clear();
//                da.Fill(dt);
//                if (dt.Rows.Count > 0)
//                {
//                    foreach (DataRow dr in dt.Rows)
//                        CreateTeamRes(app, doc.Tables[2], dr[0].ToString(), da);
//                    doc.Tables[2].Rows[doc.Tables[2].Rows.Count].Delete();
//                    doc.Tables[2].Columns[doc.Tables[2].Columns.Count].Delete();
//                }
//                bw.ReportProgress(25);

//                int rowInd = 1;
//                da.SelectCommand.CommandText = "  SELECT style, MIN(iid) id " +
//                                              "    FROM lists(NOLOCK) " +
//#if FULL
// "   WHERE style <> '' " +
//                                              "     AND style NOT LIKE 'Командн%' " +
//#else
//                                              "   WHERE style = 'Скорость' " +
//#endif
// "GROUP BY style " +
//                                              "ORDER BY id ";
//                dt.Clear();
//                dt.Columns.Clear();
//                da.Fill(dt);

//                if (dt.Rows.Count > 0)
//                {
//                    tmp = 35 / dt.Rows.Count;
//                    pCount = 25;
//                    foreach (DataRow dr in dt.Rows)
//                    {
//                        CreateStyleMedals(app, doc.Tables[3], dr[0].ToString(), ref rowInd);
//                        pCount += tmp;
//                        bw.ReportProgress(pCount);
//                    }
//                    doc.Tables[3].Rows[doc.Tables[3].Rows.Count].Delete();
//                }
//                string sWhereTK;
//#if FULL
//                sWhereTK = " WHERE L.style <> 'Многоборье' ";
//#else
//                sWhereTK = " WHERE L.style = 'Скорость' ";
//#endif
//                da.SelectCommand.CommandText =
//                    " SELECT 0 vk, 0 pos, '' posText, TMP.team_id iid, '' qf, 0.0 pts, '' ptsText, 0 sp, " +
//                    "        10000 - SUM(TMP.gold) pos0, 10000 - SUM(TMP.silver) pos1, 10000 - SUM(TMP.bronze) pos2, " +
//                    "        t.name, SUM(TMP.gold) gold, SUM(TMP.silver) silver, SUM(Tmp.bronze) bronze " +
//                    "   FROM (SELECT P.team_id, COUNT(G.climber_id) gold, 0 silver, 0 bronze " +
//                    "           FROM Participants P(NOLOCK) " +
//                    "           JOIN generalResults G(NOLOCK) ON G.climber_id = P.iid " +
//                    "           JOIN lists L(NOLOCK) ON L.iid = G.list_id " +
//                    sWhereTK +
//                    "            AND G.pos = 1 " +
//                    "       GROUP BY P.team_id " +
//                    "          UNION ALL " +
//                    "         SELECT P.team_id, 0 gold, COUNT(G.climber_id) silver, 0 bronze " +
//                    "           FROM Participants P(NOLOCK) " +
//                    "           JOIN generalResults G(NOLOCK) ON G.climber_id = P.iid " +
//                    "           JOIN lists L(NOLOCK) ON L.iid = G.list_id " +
//                    sWhereTK +
//                    "            AND G.pos = 2 " +
//                    "       GROUP BY P.team_id " +
//                    "          UNION ALL " +
//                    "         SELECT P.team_id, 0 gold, 0 silver, COUNT(G.climber_id) bronze " +
//                    "           FROM Participants P(NOLOCK) " +
//                    "           JOIN generalResults G(NOLOCK) ON G.climber_id = P.iid " +
//                    "           JOIN lists L(NOLOCK) ON L.iid = G.list_id " +
//                    sWhereTK +
//                    "            AND G.pos = 3 " +
//                    "       GROUP BY P.team_id) TMP " +
//                    "   JOIN Teams T(NOLOCK) ON T.iid = TMP.team_id " +
//                    "  WHERE TMP.gold <> 0 " +
//                    "     OR TMP.silver <> 0 " +
//                    "     OR TMP.bronze <> 0 " +
//                   "GROUP BY TMP.team_id, T.name " +
//                   "ORDER BY gold DESC, silver DESC, bronze DESC, T.name ";
//                dt.Columns.Clear();
//                dt.Clear();
//                da.Fill(dt);
//                Word.Table t;
//                if (dt.Rows.Count > 0)
//                {
//                    SortingClass.SortResults(dt, false, false);
//                    t = doc.Tables[4];
//                    foreach (DataRow dr in dt.Rows)
//                    {
//                        object oo = t.Rows[t.Rows.Count];
//                        t.Rows.Add(ref oo);
//                        for (int jj = 2; jj < 6; jj++)
//                            t.Cell(t.Rows.Count - 1, jj).Range.Text = dr[9 + jj].ToString();
//                        t.Cell(t.Rows.Count - 1, 1).Range.Text = dr[2].ToString();
//                    }
//                    t.Rows[t.Rows.Count].Delete();
//                }
//                bw.ReportProgress(70);
                da.SelectCommand.CommandText = "  SELECT DISTINCT surname + ' '  + name + ' ' + patronimic full_name, pos, city, category " +
                                               "    FROM judgeView(NOLOCK) " +
                                               "   WHERE pos LIKE '%трасс%' " +
                                               "     AND pos NOT LIKE '%на%' " +
                                               "ORDER BY pos, city, full_name ";

                Word.Table t = doc.Tables[1];
                FillJudgeTable(da, t);
                cmd.CommandText = "SELECT DISTINCT L.style" +
                                  "  FROM lists L(nolock)" +
                                  " WHERE L.style IN('Трудность','Скорость','Боулдеринг')" +
                               " ORDER BY L.style DESC";
                groupRow = String.Empty;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (!String.IsNullOrEmpty(groupRow))
                            groupRow += "; ";
                        groupRow += rdr[0].ToString() + ": ";
                    }
                    if (!String.IsNullOrEmpty(groupRow))
                        groupRow += ".";
                }
                ReplaceLabel(doc, app, "ROUTES", groupRow);

//                tmp = GetParagraphIndex(doc, "трасс, замечания, комментарии к трассам");
//                if (tmp < 0)
//                    throw new ArgumentException("Шаблон отчёта повреждён");
//                string str = "";
//#if FULL
//                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE style = 'Трудность'";
//                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
//                    str = GetLeadStat();
//                bw.ReportProgress(75);
//#endif
//                cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE style = 'Скорость'";
//                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
//                    str += GetSpeedStat();

//                doc.Paragraphs[tmp + 2].Range.Text = str;
                bw.ReportProgress(80);

                da.SelectCommand.CommandText = "  SELECT surname + ' '  + name + ' ' + patronimic full_name, pos, city, category, " +
                                               "         CASE WHEN (pos LIKE '%удья%' AND pos LIKE 'Гл%') THEN 0 " +
                                               "              WHEN (pos LIKE '%екретарь%' AND pos LIKE 'Гл%') THEN 1 " +
                                               "              WHEN pos LIKE '%виду%' THEN 2 " +
                                               "              WHEN pos LIKE '%безоп%' THEN 3 " +
                                               "              WHEN pos LIKE '%на трассе%' THEN 4 " +
                                               "              WHEN pos LIKE '%участ%' THEN 5 " +
                                               "              WHEN pos LIKE '%старт%' THEN 6 " +
                                               "              WHEN pos LIKE '%видео%' THEN 7 " +
                                               "              WHEN pos LIKE '%екретарь%' THEN 9 " +
                                               "              ELSE 8 END ord " +
                                               "    FROM judgeView(NOLOCK) " +
                                               "   WHERE (pos NOT LIKE '%трасс%' OR POS LIKE '%на%')" +
                                               "     AND pos NOT LIKE '%траховщ%'" +
                                               "     AND pos NOT LIKE '%мандатной%'" +
                                               "     AND pos NOT LIKE '%редставитель%' " +
                                               "     AND pos NOT LIKE 'Врач%' " +
                                               "ORDER BY ord, pos, city, full_name ";
                t = doc.Tables[2];
                FillJudgeTable(da, t);
                bw.ReportProgress(85);
                da.SelectCommand.CommandText = "  SELECT surname + ' '  + name + ' ' + patronimic full_name, pos, city, category " +
                                               "    FROM judgeView(NOLOCK) " +
                                               "   WHERE pos LIKE '%траховщ%' " +
                                               "ORDER BY pos, city, full_name ";

                t = doc.Tables[3];
                FillJudgeTable(da, t);

                //da.SelectCommand.CommandText = "  SELECT T.name, COUNT(P.iid) cnt " +
                //                               "    FROM Participants P(NOLOCK) " +
                //                               "    JOIN Teams T(NOLOCK) ON T.iid = P.team_id " +
                //                               "GROUP BY P.team_id, T.name " +
                //                               "ORDER BY T.name ";
                //dt.Clear();
                //dt.Columns.Clear();
                //da.Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    t = doc.Tables[9];
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        object oo = t.Rows[t.Rows.Count];
                //        t.Rows.Add(ref oo);
                //        for (int i = 0; i < 2; i++)
                //            t.Cell(t.Rows.Count - 1, i + 1).Range.Text = dr[i].ToString();
                //    }
                //    t.Rows[t.Rows.Count].Delete();
                //}

                cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Главный судья'";
                ReplaceLabel(doc,app,"JUDGE",StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false));
                ReplaceLabel(doc, app, "DATE", DateTime.Now.ToShortDateString());


                //}
                //catch (Exception ex)
                //{ MessageBox.Show("Ошибка создания отчёта.\r\n" + ex.Message); }
            }
            bw.ReportProgress(100);
            return app;
        }

        private static void CreateTeamRes(Word.Application app, Word.Table t, string stle, SqlDataAdapter da)
        {
            CreateTeamRes(app, t, stle, da, false);
        }
        private static void CreateTeamRes(Word.Application app, Word.Table t, string stle, SqlDataAdapter da, bool medalsOnly)
        {

            da.SelectCommand.CommandText = "  SELECT R.pos, T.name " +
                                            "    FROM teamResults R(NOLOCK) " +
                                            "    JOIN teams T(NOLOCK) ON T.iid = R.team_id " +
                                            "    JOIN lists L(NOLOCK) ON L.iid = R.list_id " +
                                            "   WHERE L.style = '" + stle + "' " +
                                            "     AND T.Guest = 0 ";
            if (medalsOnly)
                da.SelectCommand.CommandText += " AND R.pos <= 3 ";
            da.SelectCommand.CommandText += "ORDER BY R.pos ";
            DataTable dt = new DataTable();
            da.Fill(dt);
            object o1 = t.Columns[1];
            t.Columns.Add(ref o1);
            t.Columns.Add(ref o1);
            int curRow = 1;
            int curCol = 1;
            if (curRow >= t.Rows.Count)
            {
                o1 = t.Rows[t.Rows.Count];
                t.Rows.Add(ref o1);
            }

            //o1 = Word.WdUnits.wdCharacter;
            //object o2 = 1;
            //object o3 = Word.WdMovementType.wdExtend;
            //t.Cell(curRow, curCol).Select();
            //app.Selection.MoveRight(ref o1, ref o2, ref o3);
            //app.Selection.Cells.Merge();

            if (stle.ToLower().IndexOf("тр") > -1)
                t.Cell(curRow, curCol + 1).Range.Text = "Трудность";
            else
            {
                if (stle.ToLower().IndexOf("ск") > -1)
                    t.Cell(curRow, curCol + 1).Range.Text = "Скорость";
                else
                {
                    if (stle.ToLower().IndexOf("б") > -1)
                        t.Cell(curRow, curCol + 1).Range.Text = "Боулдеринг";
                    else
                        t.Cell(curRow, curCol + 1).Range.Text = "Общий зачёт";
                }
            }
            t.Cell(curRow, curCol + 1).Range.Font.Bold = 1;
            foreach (DataRow dr in dt.Rows)
            {
                curRow++;
                if (curRow >= t.Rows.Count)
                {
                    o1 = t.Rows[t.Rows.Count];
                    t.Rows.Add(ref o1);
                }
                t.Cell(curRow, curCol).Range.Text = dr[0].ToString();
                t.Cell(curRow, curCol + 1).Range.Text = dr[1].ToString();
            }
        }

        private static void FillJudgeTable(SqlDataAdapter da, Word.Table t)
        {
            int tmp = 0;
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                tmp++;
                object oo = t.Rows[t.Rows.Count];
                t.Rows.Add(ref oo);
                for (int ii = 0; ii < 4; ii++)
                    t.Cell(t.Rows.Count - 1, ii + 2).Range.Text = dr[ii].ToString();
                t.Cell(t.Rows.Count - 1, 1).Range.Text = tmp.ToString();
            }
            t.Rows[t.Rows.Count].Delete();
        }

#if FULL
        private string GetLeadStat()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            SqlDataAdapter da = new SqlDataAdapter(
                new SqlCommand("SELECT iid, name FROM groups(NOLOCK) ORDER BY oldYear, genderFemale", cn));
            DataTable dtGr = new DataTable();
            da.Fill(dtGr);
            string str = "Трудность: количество результатов TOP:";
            foreach (DataRow drGr in dtGr.Rows)
            {
                str += "\r     ";
                str += drGr[1].ToString() + ": ";
                da.SelectCommand.CommandText = "  SELECT iid, round " +
                                               "    FROM lists(NOLOCK) " +
                                               "   WHERE group_id = " + drGr[0].ToString() +
                                               "     AND style = 'Трудность' " +
                                               "     AND round NOT LIKE '%2 трассы%' " +
                                               "     AND round <> 'Квалификация' " +
                                               "     AND round <> 'Итоговый протокол' " +
                                               "ORDER BY iid ";
                DataTable dtR = new DataTable();
                da.Fill(dtR);
                foreach (DataRow drR in dtR.Rows)
                {
                    cmd.CommandText = "SELECT COUNT(*) " +
                                      "  FROM routeResults(NOLOCK) " +
                                      " WHERE resText LIKE '%TOP%'" +
                                      "   AND list_id = " + drR[0].ToString();
                    str += drR[1].ToString() + " – " + cmd.ExecuteScalar().ToString() + "; ";
                }
            }
            return str + "\r";
        }
#endif

        private string GetSpeedStat()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            SqlDataAdapter da = new SqlDataAdapter(
                new SqlCommand("SELECT iid, name FROM groups(NOLOCK) ORDER BY oldYear, genderFemale", cn));
            DataTable dtGr = new DataTable();
            da.Fill(dtGr);
            string str = "Скорость: время прохождения дистанции:";
            string s = "";
            object o;
            foreach (DataRow drGr in dtGr.Rows)
            {
                cmd.CommandText = "SELECT COUNT(*) cnt " +
                                  "  FROM speedResults SR(NOLOCK) " +
                                  "  JOIN lists L(NOLOCK) ON L.iid = SR.list_id " +
                                  " WHERE L.group_id = " + drGr[0].ToString();
                int kk;
                try { kk = Convert.ToInt32(cmd.ExecuteScalar()); }
                catch { kk = 0; }
                if (kk < 1)
                    continue;
                str += "\r     " + drGr[1].ToString() + ":";
                cmd.CommandText = "  SELECT TOP 1 SR.resText " +
                                  "    FROM speedResults SR(NOLOCK) " +
                                  "    JOIN lists L(NOLOCK) ON L.iid = SR.list_id " +
                                  "   WHERE L.group_id = " + drGr[0].ToString() + " " +
                                  "ORDER BY SR.res";
                try
                {
                    s = "";
                    o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        s = o.ToString();
                }
                catch { }
                str += "\r          лучшее - " + s;

                cmd.CommandText = "  SELECT TOP 1 SR.resText " +
                                  "    FROM speedResults SR(NOLOCK) " +
                                  "    JOIN lists L(NOLOCK) ON L.iid = SR.list_id " +
                                  "   WHERE L.group_id = " + drGr[0].ToString() +
                                  "     AND SR.res < 6000000 " +
                                  "ORDER BY SR.res DESC";
                try
                {
                    s = "";
                    o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        s = o.ToString();
                }
                catch { }
                str += "\r          худшее - " + s;
                cmd.CommandText = "  SELECT COUNT(*) " +
                                  "    FROM speedResults SR(NOLOCK) " +
                                  "    JOIN lists L(NOLOCK) ON L.iid = SR.list_id " +
                                  "   WHERE L.round = 'Квалификация' " +
                                  "     AND SR.resText = 'срыв' " +
                                  "     AND L.group_id = " + drGr[0].ToString();
                try
                {
                    s = "";
                    o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        s = o.ToString();
                }
                catch { }
                str += "\r     количество участников, сорвавшихся в квалификации: " + s;
            }
            return str + "\r";
        }

        private static int GetParagraphIndex(Word.Document doc, string tmp)
        {
            int iPar;
            bool b;
            iPar = 0;
            do
            {
                iPar++;
                try { b = (doc.Paragraphs[iPar].Range.Text.IndexOf(tmp) < 0); }
                catch
                {
                    iPar = -1;
                    break;
                }
            } while (b);
            return iPar;
        }

        void CreateStyleMedals(Word.Application app, Word.Table t, string style, ref int rowInd)
        {
            object o1 = Word.WdUnits.wdCharacter;
            object o2 = 6;
            object o3 = Word.WdMovementType.wdExtend;
            int ri = 1;
            if (rowInd > 1)
            {
                object oo = t.Rows[t.Rows.Count];
                t.Rows.Add(ref oo);
                rowInd = t.Rows.Count - 1;
                t.Rows[rowInd].Cells[1].Select();
                app.Selection.MoveRight(ref o1, ref o2, ref o3);
                app.Selection.Cells.Merge();
                app.Selection.Font.Bold = 1;
                app.Selection.Font.Size = 12;
                app.Selection.Font.Name = "Times New Roman";
                ri = 2;
            }
            o2 = 2;
            t.Rows[rowInd].Cells[1].Range.Text = style;
            SqlDataAdapter da = new SqlDataAdapter(
                new SqlCommand("SELECT iid, name, genderFemale FROM groups(NOLOCK) ORDER BY oldYear, genderFemale DESC", cn));
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                bool fem = Convert.ToBoolean(dr[2]);
                int giid = Convert.ToInt32(dr[0]);

                if (ri > 1)
                {
                    object o = t.Rows[t.Rows.Count];
                    t.Rows.Add(ref o);
                    rowInd = t.Rows.Count - 2;

                    t.Rows[rowInd + 1].Cells[1].Select();
                    app.Selection.MoveRight(ref o1, ref o2, ref o3);
                    app.Selection.Cells.Merge();
                    t.Rows[rowInd + 1].Cells[3].Select();
                    app.Selection.MoveRight(ref o1, ref o2, ref o3);
                    app.Selection.Cells.Merge();
                    t.Rows[rowInd + 1].Range.Font.Name = "Times New Roman";
                    t.Rows[rowInd + 1].Range.Font.Size = 12;
                    t.Rows[rowInd + 1].Range.Font.Bold = 1;
                    ri = 1;
                }
                if (fem)
                    t.Rows[rowInd + ri].Cells[1].Range.Text = dr[1].ToString();
                else
                    t.Rows[rowInd + ri].Cells[3].Range.Text = dr[1].ToString();
                ri = 2;
                da.SelectCommand.CommandText = "  SELECT r.pos, p.surname + ' ' + p.name part, t.name team " +
                                               "    FROM generalResults R(NOLOCK) " +
                                               "    JOIN lists L(NOLOCK) ON L.iid = R.list_id " +
                                               "    JOIN participants P(NOLOCK) ON P.iid = R.climber_id " +
                                               "    JOIN teams T(NOLOCK) ON T.iid = P.team_id " +
                                               "   WHERE L.style = '" + style + "' " +
                                               "     AND P.group_id = " + giid.ToString() + " " +
                                               "     AND r.pos <= 3 " +
                                               "ORDER BY r.pos ";
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                int indx;
                if (fem)
                    indx = 0;
                else
                    indx = 4;

                foreach (DataRow drr in dt2.Rows)
                {
                    if (rowInd + ri == t.Rows.Count)
                    {
                        object oTmp = t.Rows[t.Rows.Count];
                        t.Rows.Add(ref oTmp);
                    }
                    for (int iy = 0; iy < 3; iy++)
                        t.Rows[rowInd + ri].Cells[iy + 1 + indx].Range.Text = drr[iy].ToString();
                    ri++;
                }
                if (fem)
                    ri = 1;
            }
            rowInd = t.Rows.Count;
        }

        Word.Application CreateDelegateReport(BackgroundWorker bw)
        {
            GetGuest();
            Word.Document doc;
            Word.Application app = null;
            if (StaticClass.LaunchWord(dir + "\\Templates\\deleg1.dot", out doc, out app))
            {
                bw.ReportProgress(15);
                ReplaceLabel(doc, app, "title", StaticClass.ParseCompTitle(competitionTitle)[0]);
                ReplaceLabel(doc, app, "time", StaticClass.ParseCompTitle(competitionTitle)[2]);
                ReplaceLabel(doc, app, "place", StaticClass.ParseCompTitle(competitionTitle)[1]);
                //    rng = doc.Paragraphs[3].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[0] + ".\r";
                //    rng = doc.Paragraphs[5].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[1] + ".\r";
                //    rng = doc.Paragraphs[7].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[2] + '\r';

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    string judge;
                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%л%' AND pos LIKE '%удья%'";
                    try { judge = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                    catch { judge = ""; }
                    ReplaceLabel(doc, app, "president", judge);

                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%л%' AND pos LIKE '%екрет%'";
                    try { judge = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                    catch { judge = ""; }
                    ReplaceLabel(doc, app, "secretary", judge);

                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%по трассам%' ";
                    try { judge = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                    catch { judge = ""; }
                    ReplaceLabel(doc, app, "routesetter", judge);

                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%езопасн%' ";
                    try { judge = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                    catch { judge = ""; }
                    ReplaceLabel(doc, app, "safety", judge);

                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%виду%' ";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    judge = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (judge != "")
                            judge += ", ";
                        judge += StaticClass.GetJudgeData(Convert.ToInt32(dr["iid"]), cn, false);
                    }
                    ReplaceLabel(doc, app, "judge", judge);

                    cmd.CommandText = "  SELECT iid FROM judgeView p(NOLOCK) WHERE pos LIKE '%редст%' AND pos NOT LIKE '%орг%'";
                    try { judge = StaticClass.GetJudgeData(Convert.ToInt32(cmd.ExecuteScalar()), cn, false); }
                    catch { judge = ""; }
                    ReplaceLabel(doc, app, "delegate", judge);

                    //string routes = "";
                    try
                    {
                        DataTable dtR = new DataTable();
                        SqlDataAdapter daR = new SqlDataAdapter(cmd);
#if FULL
                        cmd.CommandText = "  SELECT l.style + ', ' + g.name + ', ' + l.round txt, g.oldYear, g.genderFemale" +
                                          "    FROM lists l(NOLOCK) " +
                                          "    JOIN groups g(NOLOCK) ON g.iid = l.group_id " +
                                          "   WHERE l.style <> 'Многоборье' " +
                                          "     AND l.style <> 'Скорость' " +
                                          "     AND l.style NOT LIKE 'Команд%' " +
                                          "     AND l.round NOT LIKE '%(2%' " +
                                          "     AND l.round <> 'Итоговый протокол' " +
                                          "ORDER BY l.style DESC, g.oldYear DESC, g.genderFemale, l.iid ";
                        daR.Fill(dtR);
#endif
                        cmd.CommandText = "  SELECT DISTINCT l.style + ', ' + g.name txt, g.oldYear, g.genderFemale " +
                                          "    FROM lists l(NOLOCK) " +
                                          "    JOIN groups g(NOLOCK) ON g.iid = l.group_id " +
                                          "   WHERE l.round <> 'Итоговый протокол' " +
                                          "     AND l.style = 'Скорость' " +
                                          "ORDER BY g.oldYear DESC, g.genderFemale ";

                        daR.Fill(dtR);
                        for (int i = 0; i < dtR.Rows.Count - 1; i++)
                            ReplaceLabel(doc, app, "ROUTES", dtR.Rows[i]["txt"].ToString() + ":\r$[ROUTES]");
                        if (dtR.Rows.Count > 0)
                            ReplaceLabel(doc, app, "ROUTES", dtR.Rows[dtR.Rows.Count - 1]["txt"].ToString());
                    }
                    catch { }
                    //rdr = cmd.ExecuteReader();
                    //try{
                    //    while(rdr.Read()) {
                    //        if(routes != "")
                    //            routes += ":\r";
                    //        routes += rdr["txt"].ToString();
                    //    }
                    //} finally {rdr.Close();}
                    //if(routes != "")
                    //    routes += ":";
                    //ReplaceLabel(doc, app, "ROUTES", routes);
                    ReplaceLabel(doc, app, "date", DateTime.Now.ToShortDateString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            bw.ReportProgress(100);

            return app;
        }

        private void ReplaceLabel(Word.Document doc, Word.Application app, string label, string repl)
        {
            WordReportCreationBar.ReplaceLabel(doc, app, label, repl, true);
        }

        private void delegateRepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateDelegateReport;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        private void report1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateReport1;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        private void report2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateReport2;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        Word.Application CreateReport1(BackgroundWorker bw)
        {
            Word.Document doc;
            Word.Application app = null;
            int tmp;
            if (StaticClass.LaunchWord(dir + "\\Templates\\rep1.dot", out doc, out app))
            {
                bw.ReportProgress(15);
                ReplaceLabel(doc, app, "title", StaticClass.ParseCompTitle(competitionTitle)[0]);
                ReplaceLabel(doc, app, "time", StaticClass.ParseCompTitle(competitionTitle)[2]);
                ReplaceLabel(doc, app, "place", StaticClass.ParseCompTitle(competitionTitle)[1]);
                //    rng = doc.Paragraphs[3].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[0] + ".\r";
                //    rng = doc.Paragraphs[5].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[1] + ".\r";
                //    rng = doc.Paragraphs[7].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[2] + '\r';

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    cmd.CommandText = "  SELECT name, oldYear, youngYear " +
                                      "    FROM groups(NOLOCK) " +
                                      "ORDER BY oldYear, genderFemale";
                    SqlDataReader rdr = cmd.ExecuteReader();

                    string groupRow = "";
                    try
                    {
                        int yO, yY;
                        while (rdr.Read())
                        {
                            try { yO = Convert.ToInt32(rdr["oldYear"]); }
                            catch { yO = 0; }
                            try { yY = Convert.ToInt32(rdr["youngYear"]); }
                            catch { yY = DateTime.Now.Year; }
                            if (groupRow != "")
                                groupRow += ", ";
                            groupRow += rdr["name"].ToString();
                            if (yO == 0 && yY != DateTime.Now.Year)
                            {
                                groupRow += " (до " + yY.ToString() + ")";
                            }
                            else if (yO != 0 && yY == DateTime.Now.Year)
                            {
                                groupRow += " (с " + yO.ToString() + ")";
                            }
                            else if (yO != 0 && yY != DateTime.Now.Year)
                            {
                                groupRow += " (" + yO.ToString() + "-" + yY.ToString() + ")";
                            }
                        }
                    }
                    finally
                    {
                        rdr.Close();
                    }
                    ReplaceLabel(doc, app, "GROUPS", groupRow);
                    bw.ReportProgress(25);

                    cmd.CommandText = "SELECT COUNT(DISTINCT t.iid) " +
                                      "  FROM teams t(NOLOCK) " +
                                      "  JOIN participants p(NOLOCK) ON P.team_id = t.iid ";
                    SetData(app, doc, cmd, "TEAM", true);

                    cmd.CommandText = "SELECT COUNT(*) " +
                                      "  FROM participants p(NOLOCK) ";
                    SetData(app, doc, cmd, "CLM", true);

                    cmd.CommandText = "SELECT COUNT(DISTINCT iid) FROM judgeView(NOLOCK) WHERE pos NOT LIKE '%редседатель%' AND pos NOT LIKE '%редставитель%' AND pos NOT LIKE '%Врач%'";
                    SetData(app, doc, cmd, "JUDGES", false);

                    SetDataRazr(app, doc, "ЗМС", "ZMS");
                    SetDataRazr(app, doc, "МСМК", "MSK");
                    SetDataRazr(app, doc, "МС", "MS");
                    SetDataRazr(app, doc, "КМС", "KMS");
                    SetDataRazr(app, doc, "1", "1R");
                    SetDataRazr(app, doc, "2", "2R");
                    SetDataRazr(app, doc, "3", "3R");
                    SetDataRazr(app, doc, "1ю", "1J");

                    cmd.CommandText = "  SELECT R.pos, T.name " +
                                      "    FROM teamResults R(NOLOCK) " +
                                      "    JOIN teams T(NOLOCK) ON T.iid = R.team_id " +
                                      "    JOIN lists L(NOLOCK) ON L.iid = R.list_id " +
#if FULL
 "   WHERE L.style = 'Командные - Мн' " +
#else
                                      "   WHERE L.style = 'Командные' " +
#endif
 "     AND T.guest = 0 " +
                                      "ORDER BY R.pos ";
                    Word.Table t = doc.Tables[2];
                    rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                        {
                            object oo = t.Rows[t.Rows.Count];

                            t.Rows.Add(ref oo);
                            for (int ii = 0; ii < 2; ii++)
                                t.Cell(t.Rows.Count - 1, ii + 1).Range.Text = rdr[ii].ToString();
                        }
                        t.Rows[t.Rows.Count].Delete();
                    }
                    finally
                    {
                        rdr.Close();
                    }

                    cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Главный судья'";

                    try { tmp = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch { tmp = -1; }
                    string judge;
                    if (tmp > 0)
                        judge = StaticClass.GetJudgeData(tmp, cn, false);
                    else
                        judge = "";
                    ReplaceLabel(doc, app, "PR_J", judge);

                    ReplaceLabel(doc, app, "date", DateTime.Now.ToShortDateString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            bw.ReportProgress(100);

            return app;
        }

        Word.Application CreateReport2(BackgroundWorker bw)
        {
            Word.Document doc;
            Word.Application app = null;
            int tmp;
            if (StaticClass.LaunchWord(dir + "\\Templates\\rep2.dot", out doc, out app))
            {
                bw.ReportProgress(15);
                ReplaceLabel(doc, app, "title", StaticClass.ParseCompTitle(competitionTitle)[0]);
                ReplaceLabel(doc, app, "time", StaticClass.ParseCompTitle(competitionTitle)[2]);
                ReplaceLabel(doc, app, "place", StaticClass.ParseCompTitle(competitionTitle)[1]);
                //    rng = doc.Paragraphs[3].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[0] + ".\r";
                //    rng = doc.Paragraphs[5].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[1] + ".\r";
                //    rng = doc.Paragraphs[7].Range;
                //    rng.Text = rng.Text.Substring(0, rng.Text.Length - 1) + StaticClass.ParseCompTitle(competitionTitle)[2] + '\r';

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    cmd.CommandText = "  SELECT name, oldYear, youngYear " +
                                      "    FROM groups(NOLOCK) " +
                                      "ORDER BY oldYear, genderFemale";
                    SqlDataReader rdr = cmd.ExecuteReader();

                    string groupRow = "";
                    try
                    {
                        int yO, yY;
                        while (rdr.Read())
                        {
                            try { yO = Convert.ToInt32(rdr["oldYear"]); }
                            catch { yO = 0; }
                            try { yY = Convert.ToInt32(rdr["youngYear"]); }
                            catch { yY = DateTime.Now.Year; }
                            if (groupRow != "")
                                groupRow += ", ";
                            groupRow += rdr["name"].ToString();
                            if (yO == 0 && yY != DateTime.Now.Year)
                            {
                                groupRow += " (до " + yY.ToString() + ")";
                            }
                            else if (yO != 0 && yY == DateTime.Now.Year)
                            {
                                groupRow += " (с " + yO.ToString() + ")";
                            }
                            else if (yO != 0 && yY != DateTime.Now.Year)
                            {
                                groupRow += " (" + yO.ToString() + "-" + yY.ToString() + ")";
                            }
                        }
                    }
                    finally
                    {
                        rdr.Close();
                    }
                    ReplaceLabel(doc, app, "GROUPS", groupRow);
                    bw.ReportProgress(25);

                    cmd.CommandText = "SELECT COUNT(DISTINCT t.iid) " +
                                      "  FROM teams t(NOLOCK) " +
                                      "  JOIN participants p(NOLOCK) ON P.team_id = t.iid ";
                    SetData(app, doc, cmd, "TEAM", true);

                    cmd.CommandText = "SELECT COUNT(*) " +
                                      "  FROM participants p(NOLOCK) ";
                    SetData(app, doc, cmd, "CLM", true);

                    cmd.CommandText = "SELECT COUNT(DISTINCT iid) FROM judgeView(NOLOCK) WHERE pos NOT LIKE '%редседатель%' AND pos NOT LIKE '%редставитель%' AND pos NOT LIKE '%Врач%'";
                    SetData(app, doc, cmd, "JUDGES", false);
                    bw.ReportProgress(35);
                    cmd.CommandText = "  SELECT CASE category " +
                                      "         WHEN '' THEN '''-''' " +
                                      "         ELSE category END cat, " +
                                      "         COUNT(*) cnt " +
                                      "    FROM judgeView(NOLOCK) " +
                                      "   WHERE pos NOT LIKE '%редседатель%' " +
                                      "     AND pos NOT LIKE '%редставитель%' " +
                                      "     AND pos NOT LIKE '%Врач%' " +
                                      "GROUP BY category " +
                                      "ORDER BY cat DESC";
                    rdr = cmd.ExecuteReader();
                    groupRow = "";
                    try
                    {
                        while (rdr.Read())
                        {
                            if (groupRow != "")
                                groupRow += ", ";
                            groupRow += rdr["cat"].ToString() + " - " + rdr["cnt"];
                        }
                    }
                    finally
                    {
                        rdr.Close();
                    }
                    ReplaceLabel(doc, app, "JCAT", groupRow);
                    bw.ReportProgress(40);

                    SetDataRazr(app, doc, "ЗМС", "ZMS");
                    SetDataRazr(app, doc, "МСМК", "MSK");
                    bw.ReportProgress(45);
                    SetDataRazr(app, doc, "МС", "MS");
                    SetDataRazr(app, doc, "КМС", "KMS");
                    bw.ReportProgress(50);
                    SetDataRazr(app, doc, "1", "1R", true);
                    bw.ReportProgress(55);
                    SetDataRazr(app, doc, "2", "2R", true);
                    bw.ReportProgress(60);
                    SetDataRazr(app, doc, "3", "3R", true);
                    bw.ReportProgress(65);
                    SetDataRazr(app, doc, "1ю", "1J", true);
                    bw.ReportProgress(70);
                    SetDataRazr(app, doc, "2ю", "2J", true);
                    bw.ReportProgress(75);
                    SetDataRazr(app, doc, "3ю", "3J", true);
                    bw.ReportProgress(80);
                    SetDataRazr(app, doc, "б/р", "BR", true);
                    bw.ReportProgress(85);
                    cmd.CommandText = "SELECT iid FROM judgeView WHERE pos = 'Главный судья'";

                    try { tmp = Convert.ToInt32(cmd.ExecuteScalar()); }
                    catch { tmp = -1; }
                    string judge;
                    if (tmp > 0)
                        judge = StaticClass.GetJudgeData(tmp, cn, false);
                    else
                        judge = "";
                    ReplaceLabel(doc, app, "PR_J", judge);
                    bw.ReportProgress(90);
                    ReplaceLabel(doc, app, "date", DateTime.Now.ToShortDateString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            bw.ReportProgress(100);

            return app;
        }

        private void SetData(Word.Application app, Word.Document doc, SqlCommand cmd, string label, bool replGender)
        {
            SetData(app, doc, cmd, label, replGender, false);
        }

        private void SetData(Word.Application app, Word.Document doc, SqlCommand cmd, string label, bool replGender, bool replGroup)
        {
            string str = cmd.ExecuteScalar().ToString();
            ReplaceLabel(doc, app, label, str);
            if (replGender || replGroup)
            {
                String bckup = cmd.CommandText;
                String s;
                if (cmd.CommandText.IndexOf("WHERE") > -1)
                    s = " AND ";
                else
                    s = " WHERE ";
                if (replGender)
                {
                    cmd.CommandText = bckup + s + "p.genderFemale = 0 ";
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "M", str);

                    cmd.CommandText = bckup + s + "p.genderFemale = 1 ";
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "F", str);
                }
                if (replGroup)
                {
                    cmd.CommandText = bckup + s + createGroupFilter(18, 19);
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "J", str);

                    cmd.CommandText = bckup + s + createGroupFilter(16, 17);
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "A", str);

                    cmd.CommandText = bckup + s + createGroupFilter(14, 15);
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "B", str);


                    cmd.CommandText = bckup + s + createGroupFilter(0, 13);
                    try
                    {
                        str = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                        str = "";
                    }
                    ReplaceLabel(doc, app, label + "C", str);
                }
            }
        }

        private string createGroupFilter(int ageYoung, int ageOld)
        {
            int yearOld = DateTime.Now.Year - ageOld;
            int yearYoung = DateTime.Now.Year - ageYoung;
            return " p.age BETWEEN " + yearOld.ToString() + " AND " +
                yearYoung.ToString() + " ";
        }

        private void SetDataRazr(Word.Application app, Word.Document doc, String razr, string label)
        {
            SetDataRazr(app, doc, razr, label, false);
        }

        private void SetDataRazr(Word.Application app, Word.Document doc, String razr, string label, bool replGroup)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) " +
                "  FROM participants p(NOLOCK) " +
                " WHERE qf = @qf ", cn);
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cmd.Parameters.Add("@qf", SqlDbType.VarChar);
            cmd.Parameters[0].Value = razr;
            SetData(app, doc, cmd, label, true, replGroup);
        }

        Word.Application CreateMedalistList(BackgroundWorker bw)
        {
            Word.Document doc;
            Word.Application app = null;
            int tmp;
            if (StaticClass.LaunchWord(dir + "\\Templates\\medals.dot", out doc, out app))
            {
                bw.ReportProgress(15);
                ReplaceLabel(doc, app, "title", StaticClass.ParseCompTitle(competitionTitle)[0]);
                ReplaceLabel(doc, app, "time", StaticClass.ParseCompTitle(competitionTitle)[2]);
                ReplaceLabel(doc, app, "place", StaticClass.ParseCompTitle(competitionTitle)[1]);
                

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    cmd.CommandText = "  SELECT style, MIN(iid) id " +
                                      "    FROM lists(NOLOCK) " +
                                      "   WHERE style <> '' " +
#if FULL
 "     AND style NOT LIKE 'Командн%' " +
#else
                                      "     AND style = 'Скорость' " +
#endif
 "GROUP BY style " +
                                      "ORDER BY id ";
                    int rowInd = 1, pCount;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        pCount = 25;
                        tmp = (75 - pCount) / dt.Rows.Count;
                        foreach (DataRow dr in dt.Rows)
                        {
                            CreateStyleMedals(app, doc.Tables[1], dr[0].ToString(), ref rowInd);
                            pCount += tmp;
                            bw.ReportProgress(pCount);
                        }
                        doc.Tables[1].Rows[doc.Tables[1].Rows.Count].Delete();
                    }
#if FULL
                    da.SelectCommand.CommandText = "SELECT style FROM lists(NOLOCK) WHERE style LIKE 'Команд%' ORDER BY iid DESC";
#else
                    da.SelectCommand.CommandText = "SELECT style FROM lists(NOLOCK) WHERE style = 'Командные' ORDER BY iid DESC";
#endif
                    dt.Clear();
                    dt.Columns.Clear();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        pCount = 75;
                        tmp = (95 - pCount) / dt.Rows.Count;
                        foreach (DataRow dr in dt.Rows)
                        {
                            CreateTeamRes(app, doc.Tables[2], dr[0].ToString(), da, true);
                            bw.ReportProgress(pCount);
                        }
                        doc.Tables[2].Rows[doc.Tables[2].Rows.Count].Delete();
                        doc.Tables[2].Columns[doc.Tables[2].Columns.Count].Delete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            bw.ReportProgress(100);

            return app;
        }

        private void medListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WordReportCreationBar.CreateRep crp = CreateMedalistList;
            WordReportCreationBar rb = new WordReportCreationBar(crp);
            rb.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBoxMy ab = new AboutBoxMy();
            ab.ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = Thread.GetDomain().BaseDirectory;
                if (dir.Length > 0)
                    if (dir[dir.Length - 1] != '\\')
                        dir += "\\";
                dir += "Руководство пользователя.pdf";
                System.Diagnostics.Process prc = System.Diagnostics.Process.Start(dir);
                if (prc == null)
                    throw new Exception();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка открытия файла руководства:\r\n" + ex.Message); }
        }

        private void onlineTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cn != null)
            {
                foreach (Form f in this.MdiChildren)
                    if (f is OnlineTest)
                        f.Close();
                OnlineTest onl = new OnlineTest("Боулдеринг", cn);
                onl.MdiParent = this;
                onl.Show();
            }
        }

        private void changeBroadcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetBroadcast(/*out fullReload*/))
                    StartBroadcast();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка запуска трансляции:\r\n" + ex.Message); }
        }

        private void imageTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new GetIid(cn)).ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
#if !DEBUG
                if (!needToExit)
                {
                    if (MessageBox.Show(this, "Вы уверены, что хотите выйти из приложения?", "Выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
#endif
                if (this.LoadingThread != null && LoadingThread.IsAlive)
                    e.Cancel = (MessageBox.Show("Идёт загрузка списка участников. Прервать?", "Прервать загрузку?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No);
                if (e.Cancel)
                    return;
                if (thrDataCol.HasAlive)
                    e.Cancel = (MessageBox.Show("Есть незаврешённые задачи в фоновом потоке. Прервать?", "",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No);
                if (e.Cancel)
                    return;
            }
            thrDataCol.AbortAll();
            
            if (!e.Cancel && cn != null)
                if (cn.State != ConnectionState.Closed)
                    try { cn.Close(); }
                    catch { }

        }

        public enum ThreadStats { IMG_SAV_F, IMG_LOAD_F, IMG_SAV_REM, IMG_LOAD_REM, UPD_REM }

        public class ThrData
        {
            public ThrData(Thread thr, ThreadStats stats)
            {
                this.thr = thr;
                this.stats = stats;
            }
            public ThreadStats stats;
            public Thread thr;
        }

        private class ThrDataCollection : List<ThrData>
        {
            private Mutex m = new Mutex();
            public new void Add(ThrData item)
            {
                m.WaitOne();
                try { base.Add(item); }
                finally { m.ReleaseMutex(); }
            }
            public new bool Remove(ThrData item)
            {
                m.WaitOne();
                try { return base.Remove(item); }
                finally { m.ReleaseMutex(); }
            }
            public ThrData this[ThreadStats st]
            {
                get
                {
                    m.WaitOne();
                    try
                    {
                        foreach (ThrData td in this)
                            if (td.stats == st)
                                return td;
                        return null;
                    }
                    finally { m.ReleaseMutex(); }
                }
            }
            public bool HasAlive
            {
                get
                {
                    m.WaitOne();
                    try
                    {
                        foreach (ThrData td in this)
                            if (td.thr.IsAlive)
                                return true;
                    }
                    finally { m.ReleaseMutex(); }
                    return false;
                }
            }
            public void AbortAll()
            {
                m.WaitOne();
                try
                {
                    foreach (ThrData td in this)
                        if (td.thr.IsAlive)
                            td.thr.Abort();
                    this.Clear();
                }
                finally { m.ReleaseMutex(); }
            }
        }

        private ThrDataCollection thrDataCol = new ThrDataCollection();

        public bool AddThrData(ThrData td)
        {
            if (thrDataCol[td.stats] != null)
                return false;
            thrDataCol.Add(td);
            statusStrip1.Invoke(new EventHandler(delegate
            {
                switch (td.stats)
                {
                    case ThreadStats.IMG_LOAD_F:
                        tsLabel.Text += IMG_LOAD_F;
                        break;
                    case ThreadStats.IMG_LOAD_REM:
                        tsLabel.Text += IMG_LOAD_REM;
                        break;
                    case ThreadStats.IMG_SAV_F:
                        tsLabel.Text += IMG_SAV_F;
                        break;
                    case ThreadStats.IMG_SAV_REM:
                        tsLabel.Text += IMG_SAV_REM;
                        break;
                    case ThreadStats.UPD_REM:
                        tsLabel.Text += UPD_REM;
                        break;
                }
            }));
            return true;
        }

        public delegate void AfetrStopCallBack(ThreadStats stats);

        public void RemThrData(ThreadStats stats)
        {
            try
            {
                ThrData data = thrDataCol[stats];
                if (data == null)
                    return;
                thrDataCol.Remove(data);
                if (statusStrip1.InvokeRequired)
                    statusStrip1.Invoke(new EventHandler(delegate
                    {
                        switch (stats)
                        {
                            case ThreadStats.IMG_LOAD_F:
                                tsLabel.Text = tsLabel.Text.Replace(IMG_LOAD_F, "");
                                break;
                            case ThreadStats.IMG_LOAD_REM:
                                tsLabel.Text = tsLabel.Text.Replace(IMG_LOAD_REM, "");
                                break;
                            case ThreadStats.IMG_SAV_F:
                                tsLabel.Text = tsLabel.Text.Replace(IMG_SAV_F, "");
                                break;
                            case ThreadStats.IMG_SAV_REM:
                                tsLabel.Text = tsLabel.Text.Replace(IMG_SAV_REM, "");
                                break;
                            case ThreadStats.UPD_REM:
                                tsLabel.Text = tsLabel.Text.Replace(UPD_REM, "");
                                break;
                        }
                    }));
                else
                    switch (stats)
                    {
                        case ThreadStats.IMG_LOAD_F:
                            tsLabel.Text = tsLabel.Text.Replace(IMG_LOAD_F, "");
                            break;
                        case ThreadStats.IMG_LOAD_REM:
                            tsLabel.Text = tsLabel.Text.Replace(IMG_LOAD_REM, "");
                            break;
                        case ThreadStats.IMG_SAV_F:
                            tsLabel.Text = tsLabel.Text.Replace(IMG_SAV_F, "");
                            break;
                        case ThreadStats.IMG_SAV_REM:
                            tsLabel.Text = tsLabel.Text.Replace(IMG_SAV_REM, "");
                            break;
                        case ThreadStats.UPD_REM:
                            tsLabel.Text = tsLabel.Text.Replace(UPD_REM, "");
                            break;
                    }
            }
            catch { }
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = Thread.GetDomain().BaseDirectory;
                if (dir.Length > 0)
                    if (dir[dir.Length - 1] != '\\')
                        dir += "\\";
                dir += "Правила соревнований.pdf";
                System.Diagnostics.Process prc = System.Diagnostics.Process.Start(dir);
            }
            catch (Exception ex)
            { MessageBox.Show("Не удалось открыть правила соревнований:\r\n" + ex.Message); }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.needToExit)
                    this.Close();

                SetMenuLanguage();

            }
            catch { }
        }

        private void SetMenuLanguage()
        {
            
            switch (Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2))
            {
                case "en":
                    englishToolStripMenuItem.Checked = true;
                    russianToolStripMenuItem.Checked = ukranianToolStripMenuItem.Checked = false;
                    break;
                case "uk":
                    ukranianToolStripMenuItem.Checked = true;
                    russianToolStripMenuItem.Checked = englishToolStripMenuItem.Checked = false;
                    break;
                default:
                    russianToolStripMenuItem.Checked = true;
                    englishToolStripMenuItem.Checked = ukranianToolStripMenuItem.Checked = false;
                    break;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm(cn);
            sf.ShowDialog();
        }

        private void rulesIFSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = Thread.GetDomain().BaseDirectory;
                if (String.IsNullOrEmpty(dir))
                    dir = "IFSC_Rules.pdf";
                else
                    dir = Path.Combine(dir, "IFSC_Rules.pdf");
                System.Diagnostics.Process prc = System.Diagnostics.Process.Start(dir);
            }
            catch (Exception ex)
            { MessageBox.Show("Не удалось открыть международные правила соревнований:\r\n" + ex.Message); }
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsI = sender as ToolStripMenuItem;
            if (tsI == null)
                return;
            if (!tsI.Checked)
            {
                russianToolStripMenuItem.Checked = true;
                ukranianToolStripMenuItem.Checked = englishToolStripMenuItem.Checked = false;
            }
            if (tsI == ukranianToolStripMenuItem)
                russianToolStripMenuItem.Checked = englishToolStripMenuItem.Checked = false;
            else if (tsI == englishToolStripMenuItem)
                russianToolStripMenuItem.Checked = ukranianToolStripMenuItem.Checked = false;
            else
                englishToolStripMenuItem.Checked = ukranianToolStripMenuItem.Checked = false;

            CultureInfo ci;
            if (englishToolStripMenuItem.Checked)
                ci = new CultureInfo("en-US");
            else if (ukranianToolStripMenuItem.Checked)
                ci = new CultureInfo("uk-UA");
            else
                ci = new CultureInfo("ru-RU");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;

            ClComp set = ClComp.Default;
            set.Language = ci.Name;
            set.Save();

            ComponentResourceManager res = new ComponentResourceManager(this.GetType());
            StaticClass.ApplyResourceToControl(this, res, ci);

        }

        private void clearFormDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотите очистить настройки форм?", String.Empty, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                BaseForm.FormGenerateSaveTables(cn);
                var cmd = new SqlCommand { Connection = cn };
                String hostName = System.Net.Dns.GetHostName();
                if (hostName.Length > BaseForm.HOST_NAME_FIELD_LENGTH)
                    hostName = hostName.Substring(0, BaseForm.HOST_NAME_FIELD_LENGTH);
                cmd.CommandText = "DELETE FROM form_object_data WHERE host_name = @host";
                cmd.Parameters.Add("@host", SqlDbType.VarChar, BaseForm.HOST_NAME_FIELD_LENGTH).Value = hostName;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка: " + ex.Message); }
        }
    }
}