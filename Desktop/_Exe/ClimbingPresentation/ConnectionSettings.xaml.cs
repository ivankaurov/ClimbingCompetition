using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClimbingCompetition;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Windows.Threading;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings : Window
    {
        public ConnectionSettings()
        {
            InitializeComponent();
        }

        private void LoadSettings()
        {
            var aSet = ClimbingPresentation.Properties.Settings.Default;
            rbUseSQL.IsChecked = aSet.sqlUseSqlAuth;
            rbUseWin.IsChecked = !rbUseSQL.IsChecked;
            tbServer.Text = aSet.sqlServer;
            if (aSet.sqlUseSqlAuth)
            {
                tbUser.Text = aSet.sqlUser;
                tbPassword.Password = aSet.sqlPassword;
            }
            else
                tbUser.Text = tbPassword.Password = "";
            cbDatabase.Items.Clear();
            if (!String.IsNullOrEmpty(aSet.sqlDatabase))
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = aSet.sqlDatabase;
                cbDatabase.Items.Add(cbi);
                cbDatabase.SelectedIndex = 0;
            }
            cbShowSelf.IsChecked = aSet.sqlShowSelf;

            int curP = aSet.sqlCurPrj;
            if (curP > 5)
                curP = 5;
            else if (curP < 1)
                curP = 1;
            cbProjNum.SelectedIndex = 0;
            foreach(var n in cbProjNum.Items)
                if (n is ComboBoxItem)
                {
                    ComboBoxItem cbI = (ComboBoxItem)n;
                    if (cbI.Content != null && cbI.Content.ToString() == curP.ToString())
                    {
                        cbProjNum.SelectedItem = cbI;
                        break;
                    }
                }

            SetSqlEnabled();
        }

        private void SaveSettings()
        {
            var aSet = ClimbingPresentation.Properties.Settings.Default;
            aSet.sqlUseSqlAuth = (rbUseSQL.IsChecked == null ? false : rbUseSQL.IsChecked.Value);
            aSet.sqlServer = tbServer.Text;
            aSet.sqlDatabase = cbDatabase.Text;
            if (aSet.sqlUseSqlAuth)
            {
                aSet.sqlUser = tbUser.Text;
                aSet.sqlPassword = tbPassword.Password;
            }
            else
                aSet.sqlUser = aSet.sqlPassword = "";
            aSet.sqlShowSelf = (cbShowSelf.IsChecked == null ? false : cbShowSelf.IsChecked.Value);
            int n;
            if (!int.TryParse(cbProjNum.Text, out n))
                n = 1;
            aSet.sqlCurPrj = n;
            aSet.Save();
        }

        private void rbUseWin_Checked(object sender, RoutedEventArgs e)
        {
            SetSqlEnabled();
        }

        private void SetSqlEnabled()
        {
            gridSql.IsEnabled = (rbUseSQL.IsChecked == null ? false : rbUseSQL.IsChecked.Value);
        }

        private void rbUseSQL_Checked(object sender, RoutedEventArgs e)
        {
            SetSqlEnabled();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        Thread thr = null;

        private void cbDatabase_DropDownOpened(object sender, EventArgs e)
        {
            if (thr != null && thr.IsAlive)
                return;
            cbDatabase.Items.Clear();
            CnData cd;
            bool checkDb = (cbShowSelf.IsChecked == null ? false : cbShowSelf.IsChecked.Value);
            if (rbUseSQL.IsChecked != null && rbUseSQL.IsChecked.Value)
                cd = new CnData(tbServer.Text, checkDb, tbUser.Text, tbPassword.Password, String.Empty);
            else
                cd = new CnData(tbServer.Text, checkDb, String.Empty);
            thr = new Thread(FillDatabases);
            thr.Start(cd);
        }


        private void FillDatabases(object o)
        {
            if (!(o is CnData))
                return;
            CnData cnData = (CnData)o;
            SqlConnection cn = null;
            try
            {
                if (cnData.UseSql)
                    cn = AccountForm.CreateConnection(cnData.Server, cnData.User, cnData.Password, "master");
                else
                    cn = AccountForm.CreateConnection(cnData.Server, "master");
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "SELECT name FROM sysdatabases(NOLOCK) ORDER BY name";
                    List<string> dbList = new List<string>();
                    var rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                            dbList.Add(rdr[0].ToString());
                    }
                    finally { rdr.Close(); }
                    foreach (string str in dbList)
                        try
                        {
                            if (cnData.CheckDB)
                            {
                                cmd.Connection.ChangeDatabase(str);
                                cmd.CommandText = "SELECT COUNT(*) FROM sysobjects(NOLOCK) WHERE name='CompetitionData' AND type='U'";
                                object oTmp = cmd.ExecuteScalar();
                                if (oTmp == null || oTmp == DBNull.Value)
                                    continue;
                                if (Convert.ToInt32(oTmp) < 1)
                                    continue;
                                cmd.CommandText = @"SELECT COUNT(*) FROM syscolumns c(NOLOCK)
                                          JOIN sysobjects obj(NOLOCK) ON obj.id = c.id
                                         WHERE obj.name = 'CompetitionData'
                                           AND obj.type = 'U'
                                           AND c.name = 'DB_ID'";
                                oTmp = cmd.ExecuteScalar();
                                if (oTmp == null || oTmp == DBNull.Value)
                                    continue;
                                if (Convert.ToInt32(oTmp) < 1)
                                    continue;
                                cmd.CommandText = "SELECT DB_ID FROM CompetitionData(NOLOCK)";
                                oTmp = cmd.ExecuteScalar();
                                if (oTmp == null || oTmp == DBNull.Value || oTmp.ToString() != AccountForm.DB_ID)
                                    continue;
                            }

                            if (cbDatabase.Dispatcher.CheckAccess())
                            {
                                ComboBoxItem item = new ComboBoxItem();
                                item.Content = str;
                                cbDatabase.Items.Add(item);
                            }
                            else
                                cbDatabase.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                    (ParameterizedThreadStart)delegate(object obj)
                                    {
                                        if (!(obj is string))
                                            return;
                                        ComboBoxItem it = new ComboBoxItem();
                                        it.BeginInit();
                                        it.Content = obj as string;
                                        it.EndInit();
                                        cbDatabase.Items.Add(it);
                                    }, str);
                                 
                        }
                        catch { }
                }
                finally { cn.Close(); }
            }
            catch (Exception ex)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    MessageBox.Show(this, "Ошибка загрузки списка баз данных:\r\n" +
                        ex.Message, "Ошибка загрузки");
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate
                    {
                        MessageBox.Show(this, "Ошибка загрузки списка баз данных:\r\n" +
                            ex.Message, "Ошибка загрузки");
                    });
                }
            }
        }

        private sealed class CnData
        {
            public bool UseSql { get; private set; }
            public string Server { get; private set; }
            public string User { get; private set; }
            public string Password { get; private set; }
            public bool CheckDB { get; private set; }
            public string Database { get; private set; }
            public CnData(string server, bool checkDB, string database) :
                this(false, server, checkDB, String.Empty, String.Empty, database)
            { }

            public CnData(string server, bool checkDB, string user, string password, string database) :
                this(true, server, checkDB, user, password, database) { }

            public CnData(bool useSql, string server, bool checkDB, string user, string password, string database)
            {
                this.UseSql = useSql;
                this.Server = server;
                this.User = user;
                this.Password = password;
                this.CheckDB = checkDB;
            }
        }

        private SqlConnection cnToRet = null;
        public SqlConnection CreatedConnection { get { return cnToRet; } }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SaveAndExit();
        }

        private void SaveAndExit()
        {
            if (String.IsNullOrEmpty(tbServer.Text))
            {
                MessageBox.Show("Сервер не введён");
                return;
            }
            if (rbUseSQL.IsChecked == true)
            {
                if (String.IsNullOrEmpty(tbUser.Text))
                {
                    MessageBox.Show("Имя пользователя не введено");
                    return;
                }
                if (String.IsNullOrEmpty(tbPassword.Password))
                {
                    MessageBox.Show("Пароль не введён");
                    return;
                }
            }
            if (String.IsNullOrEmpty(cbDatabase.Text))
            {
                MessageBox.Show("База данных не выбрана");
                return;
            }

            if (rbUseSQL.IsChecked == true)
                cnToRet = AccountForm.CreateConnection(tbServer.Text, tbUser.Text, tbPassword.Password, cbDatabase.Text);
            else
                cnToRet = AccountForm.CreateConnection(tbServer.Text, cbDatabase.Text);

            SaveSettings();
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearAndExit();
        }

        private void ClearAndExit()
        {
            cnToRet = null;
            this.DialogResult = false;
            this.Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ClearAndExit();
                    return;
                case Key.Enter:
                    SaveAndExit();
                    return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thr != null && thr.IsAlive)
                thr.Join();
        }
    }
}
