using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ClimbingCompetition.WebServices;
using XmlApiClient;
using System.Net;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� ��������� ���������� ��� ������ ����������
    /// </summary>
    public partial class OnlineBroadcast : BaseForm
    {
        SqlConnection remoteConnection;
        SqlDataAdapter da;
        MainForm parent;
        DataTable dt = new DataTable();
        long? compForService = null;
        XmlClient client;
        
        public OnlineBroadcast(/*string remoteConn,*/ SqlConnection localBaseCon, MainForm parent)
            : base(localBaseCon, parent.competitionTitle, null)
        {
            InitializeComponent();
            this.parent = parent;
            //this.cn = cn;
            //XmlClientWrapper wrapper;
            //compForService = StaticClass.ParseServString(remoteConn, out wrapper);
            //if (wrapper != null)
            //{
            //    client = wrapper.CreateClient(cn);
            //    compForService = wrapper.CompetitionId;
            //    remoteConnection = null;
            //}
            //else if (compForService == null)
            //    remoteConnection = new SqlConnection(remoteConn);
            //else
            //    remoteConn = null;
            ////if (this.cn.State != ConnectionState.Open)
            ////    this.cn.Open();

            SortingClass.CheckColumn("Participants", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("lists", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("routeResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("speedResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            SortingClass.CheckColumn("boulderResults", "changed", "BIT NOT NULL DEFAULT 1", this.cn);
            StaticClass.CheckChangedTriggers(this.cn);

            da = new SqlDataAdapter(new SqlCommand(
                "SELECT l.iid AS [�],l.style AS ���,g.name AS ������,l.round AS �����, " +
                "l.routeNumber AS [����� �����], " +
                "l.online AS [������ �� �����],l.allowView AS [������ ����������], g.iid gr_id " +
                "FROM lists l(NOLOCK) INNER JOIN groups g(NOLOCK) ON l.group_id=g.iid "+
#if FULL
                " WHERE l.style IN ('���������','��������','����������') "+
#else
                " WHERE l.style = '��������' " +
#endif
                "ORDER BY " +
                "l.iid DESC", this.cn));
            try { da.Fill(dt); }
            catch { }
            dg.DataSource = dt;
            dg.Columns["gr_id"].Visible = false;
            da.UpdateCommand = new SqlCommand("UPDATE lists SET online=@onl WHERE iid=@iid", this.cn);
            da.UpdateCommand.Parameters.Add("@onl", SqlDbType.Bit, 1, "������ �� �����");
            da.UpdateCommand.Parameters.Add("@iid", SqlDbType.Int, 4, "�");
            try { dg.Columns["������ ����������"].ReadOnly = true; }
            catch { }
#if !DEBUG
            cbLoadJudges.Visible = false;
#endif
        }

        bool startBroadcast = false;
        public bool StartBroadcast
        {
            get { return startBroadcast; }
            private set { startBroadcast = value; }
        }

        private static void OnlineStarted(RequestResult result, Exception ex, object data)
        {
            var message = data as string;
            if (result == RequestResult.Success)
            {
                if (!String.IsNullOrEmpty(message))
                    MessageBox.Show(message);
            }
            else if (result == RequestResult.Error)
                StaticClass.ShowExceptionMessageBox("������ ���������� �����", ex);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DataTable dtL = null;
            try
            {
                if (da.UpdateCommand.Connection.State != ConnectionState.Open)
                    da.UpdateCommand.Connection.Open();
                da.Update(this.dt);

                startBroadcast = OnlineUpdater2.GetUpdater(cn).LoadLiveLists() > 0;
                if (startBroadcast)
                {
                    this.Close();
                    return;
                }

                if (MessageBox.Show("��������� ��� �������� �� ���� �� �������. ������� ���������?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    this.Close();
                return;
                //SqlCommand cmd = new SqlCommand("UPDATE lists SET changed = 0", da.UpdateCommand.Connection);
                //cmd.ExecuteNonQuery();

                dtL = dt.Clone();
                dtL.Rows.Clear();
                foreach (DataRow dr in dt.Rows)
                    if (Convert.ToBoolean(dr["������ ����������"]) &&
                        Convert.ToBoolean(dr["������ �� �����"]))
                        dtL.Rows.Add(dr.ItemArray);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (client != null)
            {
                OnlineUpdater.BeginFullUpdate(false, cn, client, OnlineStarted, "���������� ������� ��������", OnlineUpdater.UpdateStartMode.AskForWait);
                this.Close();
                startBroadcast = true;
                return;
            }
            if (dtL != null && dtL.Rows.Count > 0)
                startBroadcast = StaticClass.ReloadListsFromTable(false, dtL, cn, remoteConnection, compForService, client);
            else
                startBroadcast = false;
            try
            {
                if (startBroadcast)
                    StaticClass.UpdateListData(false, cn, remoteConnection, true, compForService, client);
            }
            catch { }
            if (!startBroadcast)
            {
                if (MessageBox.Show("��������� ��� �������� �� ���� �� �������. ������� ���������?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    this.Close();
            }
            else
                this.Close();
        }

        //public bool fullReload = false;

        private void UpdateCompleted(RequestResult result, Exception ex, object state)
        {
            try
            {
                switch (result)
                {
                    case RequestResult.Exit:
                        return;
                    case RequestResult.Error:
                        StaticClass.ShowExceptionMessageBox("������ ����������", ex, this);
                        return;
                    default:
                        StaticClass.ShowExceptionMessageBox("������ ���������� ������� ��������", owner: this);
                        return;
                }
            }
            finally
            {
                lock (loaderLocker)
                {
                    climbersLoadingResult = null;
                }
            }
        }
        AsyncRequestResult climbersLoadingResult = null;
        private static object loaderLocker = new object();
        
        private void btnLoadClimbers_Click(object sender, EventArgs e)
        {
            try
            {
                this.ReloadClimbersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("������ ����������: {0}", ex));
            }
            return;

            if (client != null)
            {
                if (cbLoadInDiffThread.Checked)
                {
                    lock(loaderLocker)
                    {
                        if (climbersLoadingResult != null)
                        {
                            if (MessageBox.Show(this, "�������� ��������. ��������?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == System.Windows.Forms.DialogResult.Yes)
                                climbersLoadingResult.Abort();
                            else
                                return;
                        }
                        climbersLoadingResult = OnlineUpdater.BeginRefreshClimbers(true, cn, client, UpdateCompleted, null);
                    }
                }
                else
                {
                    try
                    {
                        OnlineUpdater.RefreshClimbers(true, cn, client, true);
                        StaticClass.ShowExceptionMessageBox("������ ���������� ������� ��������", owner: this);
                    }
                    catch (WebException wex) { StaticClass.ShowExceptionMessageBox("������ ���������� �����", wex, this); }
                    catch (SqlException s) { StaticClass.ShowExceptionMessageBox("������ ������ � �������", s, this); }
                    catch (Exception ex) { StaticClass.ShowExceptionMessageBox("����������� ������", ex, this); }
                }
                return;
            }
            loadAgr la = new loadAgr(!cbLoadInDiffThread.Checked, cbLoadPhoto.Checked, cbLoadJudges.Checked,
                compForService, client, new MainForm.AfetrStopCallBack(parent.RemThrData), cn);
            if (la.useCurrent)
                LoadClimbers(la);
            else
            {
                if (parent.LoadingThread != null && parent.LoadingThread.IsAlive)
                    if (MessageBox.Show("�������� ������ ���������� ��� ��������. ��������?", "�������� ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                        return;
                parent.LoadingThread = new Thread(LoadClimbers);
                MainForm.ThrData td = new MainForm.ThrData(parent.LoadingThread, MainForm.ThreadStats.IMG_SAV_REM);
                parent.AddThrData(td);
                parent.LoadingThread.Start(la);
            }
        }

        private sealed class loadAgr
        {
            public XmlClient Client { get; private set; }
            public long? compForService { get; private set; }
            public loadAgr(bool useCurrent, bool loadPhoto, bool loadJudges, long? compForService, XmlClient client, MainForm.AfetrStopCallBack callback, SqlConnection _cn)
            {
                SqlConnection cn = new SqlConnection(_cn.ConnectionString);
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                this.Client = new XmlClient(client.BaseUrl, client.CompId, SecurityOperations.CreateSigningDelegate(cn), SecurityOperations.GetPassword(cn)) { Proxy = client.Proxy, Timeout = client.Timeout };
                this.useCurrent = useCurrent;
                this.loadJudges = loadJudges;
                this.loadPhoto = loadPhoto;
                this.Callback = callback;
                this.compForService = compForService;
            }
            public bool useCurrent { get; private set; }
            public bool loadPhoto { get; private set; }
            public bool loadJudges { get; private set; }
            public MainForm.AfetrStopCallBack Callback { get; private set; }
        }

        private void LoadClimbers(object obj)
        {
            bool threadAbort = false;
            loadAgr la = obj as loadAgr;
            if (la == null)
                return;
            try
            {
                SqlConnection lCn, rCn;
                if (la.compForService == null)
                {
                    if (la.useCurrent)
                        rCn = remoteConnection;
                    else
                        rCn = new SqlConnection(remoteConnection.ConnectionString);
                }
                else
                    rCn = null;
                if (la.useCurrent)
                    lCn = cn;
                else
                    lCn = new SqlConnection(cn.ConnectionString);
                try
                {
                    StaticClass.RefreshClimberList(true, true, la.loadPhoto, la.compForService,
                        lCn, rCn, la.Client);
                    //if (la.loadJudges)
                    //    StaticClass.RefreshJudgesList(true, true, la.loadPhoto, lCn, rCn);
                }
                catch (System.Threading.ThreadAbortException) { threadAbort = true; }
                finally
                {
                    if (!la.useCurrent)
                    {
                        try { lCn.Close(); }
                        catch { }
                        if(rCn != null)
                            try { rCn.Close(); }
                            catch { }
                    }
                }
            }
            catch (ThreadAbortException) { threadAbort = true; }
#if !DEBUG
            catch
            { }
#endif
            finally
            {
                if (!threadAbort)
                    try
                    {
                        if (la != null && la.Callback != null)
                            la.Callback(MainForm.ThreadStats.IMG_SAV_REM);
                    }
                    catch { }
            }
        }

        private void ReloadClimbersList()
        {
            OnlineUpdater2.GetUpdater(this.cn).PostAllGroups();
            OnlineUpdater2.Instance.PostAllClimbers();
            MessageBox.Show("������ ���������� ������� ��������");
        }

        private void btnReloadLists_Click(object sender, EventArgs e)
        {
            try
            {
                if (da.UpdateCommand.Connection.State != ConnectionState.Open)
                    da.UpdateCommand.Connection.Open();
                da.Update(this.dt);
                //SqlCommand cmd = new SqlCommand("UPDATE lists SET changed = 0", da.UpdateCommand.Connection);
                //cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                OnlineUpdater2.GetUpdater(this.cn).PostUpdatedGroups();
                OnlineUpdater2.Instance.PostChangedClimbers();
                if (OnlineUpdater2.Instance.LoadAllLists() > 0)
                {
                    MessageBox.Show("��������� ���������");
                    startBroadcast = true;
                    this.Close();
                    return;
                }
                startBroadcast = false;
                if (MessageBox.Show("��������� ��� �������� �� ���� �� �������. ������� ���������?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("������ ��������: {0}", ex));
                return;
            }

            return;

            if (client != null)
            {
                OnlineUpdater.BeginFullUpdate(true, cn, client, OnlineStarted,
                    String.Format("������ �� ����� �������������{0}���������� ������� ��������", Environment.NewLine),
                     OnlineUpdater.UpdateStartMode.AskForWait);
                this.Close();
                startBroadcast = true;
                return;
            }
            startBroadcast = StaticClass.ReloadListsFromTable(true, dt, cn, remoteConnection, compForService, client);
            try
            {
                if (startBroadcast)
                    StaticClass.UpdateListData(true, cn, remoteConnection, true, compForService, client);
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ������� ����������\r\n" + ex.Message);
                return;
            }
            if (!startBroadcast)
            {
                if (MessageBox.Show("��������� ��� �������� �� ���� �� �������. ������� ���������?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    this.Close();
            }
            else
                this.Close();
        }

        private void btnLoadOneList_Click(object sender, EventArgs e)
        {
            int iid;
            string style, round;
            DataTable dtL = this.dt.Clone();
            dtL.Rows.Clear();
            try
            {
                int curRInd = dg.Rows.IndexOf(dg.CurrentRow);
                if (curRInd < 0 || curRInd >= dt.Rows.Count)
                    return;
                object[] row = dt.Rows[curRInd].ItemArray;
                dtL.Rows.Add(row);
                iid = Convert.ToInt32(dg.CurrentRow.Cells["�"].Value);

                style = dg.CurrentRow.Cells["���"].Value.ToString();
                round = dg.CurrentRow.Cells["�����"].Value.ToString();
                dg.CurrentRow.Cells["������ �� �����"].Value = true;
                dtL.Rows[0]["������ �� �����"] = true;
            }
            catch
            {
                MessageBox.Show("�������� �� ������");
                return;
            }
            OnlineUpdater2.GetUpdater(cn).LoadSingleList(iid, true);
            MessageBox.Show("�������� ��������");
            return;
            if (client != null)
            {
                
                OnlineUpdater.ReloadOneList(iid, cn, client);
                return;
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "UPDATE lists SET online = 1, changed = 0 WHERE iid = " + iid.ToString();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                startBroadcast = StaticClass.ReloadListsFromTable(false, dtL, cn, remoteConnection, compForService, client);
                if (startBroadcast)
                {
                    StaticClass.UpdateListData(iid, cn, remoteConnection, true, true, compForService, client);
                    MessageBox.Show("�������� ��������");
                }
                else
                    throw new Exception();
            }
            catch (Exception ex) { MessageBox.Show("������ ���������� ���������\r\n" + ex.Message); }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            ONLCompetitionSettings settingsForm = new ONLCompetitionSettings(cn, competitionTitle);
            settingsForm.ShowDialog(this);
        }
    }
}