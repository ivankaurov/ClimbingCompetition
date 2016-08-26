using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using XmlApiData;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма входа в систему
    /// </summary>
    public class AccountForm : System.Windows.Forms.Form
    {
#if FULL
        public const string DB_ID = "DB_CLIMBING_COMPETITION";
#else
        public const string DB_ID = "DB_SPEED_COMPETITION";
#endif
        private System.Windows.Forms.Label m_textUser;
        private System.Windows.Forms.Label m_textPassword;
        private System.Windows.Forms.Label m_textServer;
        private System.Windows.Forms.Button m_bOK;
        private System.Windows.Forms.Button m_bCancel;
        private System.Windows.Forms.TextBox m_tbUser;
        private System.Windows.Forms.TextBox m_tbServer;
        private System.Windows.Forms.TextBox m_tbPassword;
        private Label label1;
        private string cnString = "";
        private Label label2;
        private TextBox compTitle;
        private string dir, dbName, srvName, adminPwd;
        private ComboBox cbDatabase;
        private Label label3;
        private TextBox m_tbPlace;
        private Label label4;
        private TextBox m_tbDate;
        private Button btnCreateDB;
        private Button bntDropDB;
        private TextBox tbAdminPwd;
        private Label label5;
        private Label label6;
        private TextBox textBox1;
        private const string LOADING_STR = "Загрузка...";
        public const string NO_DB = "<базы данных не найдены>";

        string sOnl = "";

        public string ConnectionString
        {
            get
            {
                if (nc)
                    return "";

                return CreateConnection(cbDatabase.Text).ConnectionString;
            }
        }

        SqlConnection cn;
        private bool alertAbout;
        private bool intlOld;
        private Button btnShowData;
        private Button btnbackup;
        private Button btnRestore;
        private RadioButton rbUseWin;
        private RadioButton rbUseSQL;
        private GroupBox gb;
        private ComboBox cbProjNum;
        private CheckBox cbOwn;
        private GroupBox gbSpeed;
        private RadioButton rbIntl;
        private RadioButton rbNational;
        private Button btnExit;

        public string CompetitionTitle
        {
            get { return compTitle.Text + "\r\n" + m_tbPlace.Text + "\r\n" + m_tbDate.Text; }
        }
        public string GetDir
        {
            get { return dir; }
        }
        public string GetDB { get { return dbName; } }
        public string GetServ { get { return srvName; } }
        public string GetAdminPwd { get { return adminPwd; } }
        public int ProjNum
        {
            get {
                int nTmp;
                if (int.TryParse(cbProjNum.Text, out nTmp))
                    return nTmp;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public AccountForm(SqlConnection cn)
        {
            this.cn = cn;
            InitializeComponent();
            SetAppearance(false);
            m_bOK.Text = "Обновить";
            m_bCancel.Text = "Отмена";
            btnCreateDB.Visible = bntDropDB.Visible = btnbackup.Visible = btnRestore.Visible = false;
            SetEditMode(true);
            GetData();
            GetCompData();
            srvName = m_tbServer.Text;
            dbName = cbDatabase.Text;
            m_tbServer.ReadOnly = m_tbUser.ReadOnly = m_tbPassword.ReadOnly = true;
            cbDatabase.Enabled = false;
            alertAbout = true;
            intlOld = rbIntl.Checked;
            btnExit.Visible = false;
        }

        public AccountForm(string dir)
            : this(dir, false)
        {
        }

        public AccountForm(string dir, bool showProjNum)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            SetAppearance(showProjNum);
            this.dir = dir;
            SetEditMode(false);
            GetData();
            alertAbout = false;
            m_bCancel.Text = "Войти в систему\r\nне поключаясь к БД";
            //m_bOK.Text = "Подключиться к БД";
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void SetAppearance(bool showProjNum)
        {
            if (showProjNum)
            {
                label5.Text = "Номер проектора:";
                cbProjNum.Visible = true;
            }
            else
            {
                label5.Text = "Пароль администратора";
                cbProjNum.Visible = false;
            }
            tbAdminPwd.Visible = !showProjNum;
            gbSpeed.Visible = !showProjNum;
        }
        
        string lastDb = "";
        
        private void SaveData()
        {
        	SaveDataToRegistry();
        }
        
        private void SaveDataToRegistry() {
        	//RegistryKey rk = null, inner = null;
            appSettings aSet = appSettings.Default;
            try
            {
                //rk = Registry.LocalMachine.OpenSubKey("Software", true);

                //try{
                //    inner = rk.OpenSubKey("ClimbingCompetition", true);
                //}
                //catch {
                //    inner = null;
                //}
                //if(inner == null)
                //    inner = rk.CreateSubKey("ClimbingCompetition",
                //                            RegistryKeyPermissionCheck.ReadWriteSubTree);
                aSet.Server = m_tbServer.Text;
                //inner.SetValue("Server", m_tbServer.Text);
                aSet.Database = cbDatabase.Text;
                //inner.SetValue("Database", cbDatabase.Text);
                aSet.User = m_tbUser.Text;
                //inner.SetValue("User", m_tbUser.Text);
                aSet.Password = m_tbPassword.Text;
                //inner.SetValue("password", m_tbPassword.Text);
                aSet.UseWinAuth = rbUseWin.Checked;
                //string str;
                //if(rbUseWin.Checked)
                //    str = "WINDOWS";
                //else
                //    str = "SQL";
                //inner.SetValue("Auth_Type", str);
                aSet.OnlineString = sOnl;
                aSet.UseOwnDb = cbOwn.Checked;
                //inner.SetValue("Online", sOnl);
                //inner.Close();
                //rk.Close();
            }
            catch { }
            finally { aSet.Save(); }
        }

        private void GetDataFromRegistry() {
            appSettings aSet = appSettings.Default;
        	//RegistryKey rk = null, inner = null;
        	try{
                //rk = Registry.LocalMachine.OpenSubKey("Software", true);
        		
                //try{
                //    inner = rk.OpenSubKey("ClimbingCompetition", true);
                //}
                //catch {
                //    inner = null;
                //}
                //if(inner == null)
                //    inner = rk.CreateSubKey("ClimbingCompetition",
                //                            RegistryKeyPermissionCheck.ReadWriteSubTree);
                m_tbServer.Text = aSet.Server;
        		//m_tbServer.Text = inner.GetValue("Server", "").ToString();
                lastDb = aSet.Database;
        		//lastDb = inner.GetValue("Database", "").ToString();
        		cbDatabase.Text = lastDb;
                m_tbUser.Text = aSet.User;
        		//m_tbUser.Text = inner.GetValue("User", "").ToString();
                m_tbPassword.Text = aSet.Password;
        		//m_tbPassword.Text = inner.GetValue("password", "").ToString();
        		//string str = inner.GetValue("Auth_Type", "").ToString();
                //bool useWin = (str == "WINDOWS");
                bool useWin = aSet.UseWinAuth;
        		rbUseWin.Checked = useWin;
        		rbUseSQL.Checked = gb.Enabled = !useWin;
                sOnl = aSet.OnlineString;
                cbOwn.Checked = aSet.UseOwnDb;
        		//sOnl = inner.GetValue("Online", "").ToString();
        		//inner.Close();
        		//rk.Close();
        	}	
        	catch{}
        }
        
        private void GetData()
        {
        	GetDataFromRegistry();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_textUser = new System.Windows.Forms.Label();
            this.m_textPassword = new System.Windows.Forms.Label();
            this.m_textServer = new System.Windows.Forms.Label();
            this.m_bOK = new System.Windows.Forms.Button();
            this.m_bCancel = new System.Windows.Forms.Button();
            this.m_tbUser = new System.Windows.Forms.TextBox();
            this.m_tbServer = new System.Windows.Forms.TextBox();
            this.m_tbPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.compTitle = new System.Windows.Forms.TextBox();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_tbPlace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_tbDate = new System.Windows.Forms.TextBox();
            this.btnCreateDB = new System.Windows.Forms.Button();
            this.bntDropDB = new System.Windows.Forms.Button();
            this.tbAdminPwd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnShowData = new System.Windows.Forms.Button();
            this.btnbackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.rbUseWin = new System.Windows.Forms.RadioButton();
            this.rbUseSQL = new System.Windows.Forms.RadioButton();
            this.gb = new System.Windows.Forms.GroupBox();
            this.cbProjNum = new System.Windows.Forms.ComboBox();
            this.cbOwn = new System.Windows.Forms.CheckBox();
            this.gbSpeed = new System.Windows.Forms.GroupBox();
            this.rbNational = new System.Windows.Forms.RadioButton();
            this.rbIntl = new System.Windows.Forms.RadioButton();
            this.btnExit = new System.Windows.Forms.Button();
            this.gb.SuspendLayout();
            this.gbSpeed.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_textUser
            // 
            this.m_textUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_textUser.Location = new System.Drawing.Point(6, 16);
            this.m_textUser.Name = "m_textUser";
            this.m_textUser.Size = new System.Drawing.Size(128, 16);
            this.m_textUser.TabIndex = 0;
            this.m_textUser.Text = "Имя пользователя";
            // 
            // m_textPassword
            // 
            this.m_textPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_textPassword.Location = new System.Drawing.Point(6, 66);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.Size = new System.Drawing.Size(128, 16);
            this.m_textPassword.TabIndex = 0;
            this.m_textPassword.Text = "Пароль";
            // 
            // m_textServer
            // 
            this.m_textServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_textServer.Location = new System.Drawing.Point(12, 165);
            this.m_textServer.Name = "m_textServer";
            this.m_textServer.Size = new System.Drawing.Size(128, 16);
            this.m_textServer.TabIndex = 0;
            this.m_textServer.Text = "Сервер";
            // 
            // m_bOK
            // 
            this.m_bOK.Location = new System.Drawing.Point(8, 324);
            this.m_bOK.Name = "m_bOK";
            this.m_bOK.Size = new System.Drawing.Size(131, 37);
            this.m_bOK.TabIndex = 1;
            this.m_bOK.Text = "Подключиться к БД";
            this.m_bOK.Click += new System.EventHandler(this.m_bOK_Click);
            // 
            // m_bCancel
            // 
            this.m_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_bCancel.Location = new System.Drawing.Point(145, 324);
            this.m_bCancel.Name = "m_bCancel";
            this.m_bCancel.Size = new System.Drawing.Size(135, 38);
            this.m_bCancel.TabIndex = 1;
            this.m_bCancel.Text = "Отмена";
            this.m_bCancel.Click += new System.EventHandler(this.m_bCancel_Click);
            // 
            // m_tbUser
            // 
            this.m_tbUser.Location = new System.Drawing.Point(6, 35);
            this.m_tbUser.Name = "m_tbUser";
            this.m_tbUser.Size = new System.Drawing.Size(272, 20);
            this.m_tbUser.TabIndex = 3;
            this.m_tbUser.Text = "sa";
            // 
            // m_tbServer
            // 
            this.m_tbServer.Location = new System.Drawing.Point(12, 189);
            this.m_tbServer.Name = "m_tbServer";
            this.m_tbServer.Size = new System.Drawing.Size(272, 20);
            this.m_tbServer.TabIndex = 5;
            this.m_tbServer.Text = "toshiba-kaurov\\sql2000";
            // 
            // m_tbPassword
            // 
            this.m_tbPassword.Location = new System.Drawing.Point(6, 85);
            this.m_tbPassword.Name = "m_tbPassword";
            this.m_tbPassword.PasswordChar = '*';
            this.m_tbPassword.Size = new System.Drawing.Size(272, 20);
            this.m_tbPassword.TabIndex = 4;
            this.m_tbPassword.Text = "sa";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "База данных";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(307, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Полное название соревнований";
            // 
            // compTitle
            // 
            this.compTitle.Location = new System.Drawing.Point(307, 55);
            this.compTitle.Name = "compTitle";
            this.compTitle.Size = new System.Drawing.Size(272, 20);
            this.compTitle.TabIndex = 8;
            // 
            // cbDatabase
            // 
            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new System.Drawing.Point(11, 253);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(272, 21);
            this.cbDatabase.TabIndex = 6;
            this.cbDatabase.SelectedIndexChanged += new System.EventHandler(this.cbDatabase_SelectedIndexChanged);
            this.cbDatabase.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(307, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Место проведения";
            // 
            // m_tbPlace
            // 
            this.m_tbPlace.Location = new System.Drawing.Point(307, 165);
            this.m_tbPlace.Name = "m_tbPlace";
            this.m_tbPlace.Size = new System.Drawing.Size(272, 20);
            this.m_tbPlace.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(304, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Дата";
            // 
            // m_tbDate
            // 
            this.m_tbDate.Location = new System.Drawing.Point(304, 207);
            this.m_tbDate.Name = "m_tbDate";
            this.m_tbDate.Size = new System.Drawing.Size(272, 20);
            this.m_tbDate.TabIndex = 11;
            this.m_tbDate.Text = " ";
            // 
            // btnCreateDB
            // 
            this.btnCreateDB.Location = new System.Drawing.Point(304, 358);
            this.btnCreateDB.Name = "btnCreateDB";
            this.btnCreateDB.Size = new System.Drawing.Size(131, 37);
            this.btnCreateDB.TabIndex = 13;
            this.btnCreateDB.Text = "Создать новую БД";
            this.btnCreateDB.UseVisualStyleBackColor = true;
            this.btnCreateDB.Click += new System.EventHandler(this.btnCreateDB_Click);
            // 
            // bntDropDB
            // 
            this.bntDropDB.Location = new System.Drawing.Point(441, 357);
            this.bntDropDB.Name = "bntDropDB";
            this.bntDropDB.Size = new System.Drawing.Size(135, 38);
            this.bntDropDB.TabIndex = 14;
            this.bntDropDB.Text = "Удалить выбранную БД";
            this.bntDropDB.Click += new System.EventHandler(this.bntDropDB_Click);
            // 
            // tbAdminPwd
            // 
            this.tbAdminPwd.Location = new System.Drawing.Point(304, 254);
            this.tbAdminPwd.Name = "tbAdminPwd";
            this.tbAdminPwd.PasswordChar = '*';
            this.tbAdminPwd.Size = new System.Drawing.Size(272, 20);
            this.tbAdminPwd.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(304, 230);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 21);
            this.label5.TabIndex = 5;
            this.label5.Text = "Пароль администратора";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(307, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(242, 31);
            this.label6.TabIndex = 0;
            this.label6.Text = "Сокращённое название соревнований (для печати на номерах)";
            // 
            // tbHeight
            // 
            this.textBox1.Location = new System.Drawing.Point(307, 112);
            this.textBox1.Name = "tbHeight";
            this.textBox1.Size = new System.Drawing.Size(272, 20);
            this.textBox1.TabIndex = 9;
            // 
            // btnShowData
            // 
            this.btnShowData.Location = new System.Drawing.Point(310, 4);
            this.btnShowData.Name = "btnShowData";
            this.btnShowData.Size = new System.Drawing.Size(266, 32);
            this.btnShowData.TabIndex = 7;
            this.btnShowData.Text = "Показать данные о соревнованиях";
            this.btnShowData.UseVisualStyleBackColor = true;
            this.btnShowData.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // btnbackup
            // 
            this.btnbackup.Location = new System.Drawing.Point(304, 401);
            this.btnbackup.Name = "btnbackup";
            this.btnbackup.Size = new System.Drawing.Size(131, 37);
            this.btnbackup.TabIndex = 15;
            this.btnbackup.Text = "Создать копию БД";
            this.btnbackup.UseVisualStyleBackColor = true;
            this.btnbackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(441, 401);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(134, 37);
            this.btnRestore.TabIndex = 16;
            this.btnRestore.Text = "Восстановить БД";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // rbUseWin
            // 
            this.rbUseWin.AutoSize = true;
            this.rbUseWin.Location = new System.Drawing.Point(11, 4);
            this.rbUseWin.Name = "rbUseWin";
            this.rbUseWin.Size = new System.Drawing.Size(267, 17);
            this.rbUseWin.TabIndex = 1;
            this.rbUseWin.Text = "Использовать данные учётной записи Windows";
            this.rbUseWin.UseVisualStyleBackColor = true;
            // 
            // rbUseSQL
            // 
            this.rbUseSQL.AutoSize = true;
            this.rbUseSQL.Checked = true;
            this.rbUseSQL.Location = new System.Drawing.Point(11, 26);
            this.rbUseSQL.Name = "rbUseSQL";
            this.rbUseSQL.Size = new System.Drawing.Size(236, 17);
            this.rbUseSQL.TabIndex = 2;
            this.rbUseSQL.TabStop = true;
            this.rbUseSQL.Text = "Использовать учётную запись SQL server";
            this.rbUseSQL.UseVisualStyleBackColor = true;
            this.rbUseSQL.CheckedChanged += new System.EventHandler(this.rbUseSQL_CheckedChanged);
            // 
            // gb
            // 
            this.gb.Controls.Add(this.m_textUser);
            this.gb.Controls.Add(this.m_tbUser);
            this.gb.Controls.Add(this.m_textPassword);
            this.gb.Controls.Add(this.m_tbPassword);
            this.gb.Location = new System.Drawing.Point(5, 42);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(284, 112);
            this.gb.TabIndex = 11;
            this.gb.TabStop = false;
            // 
            // cbProjNum
            // 
            this.cbProjNum.FormattingEnabled = true;
            this.cbProjNum.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cbProjNum.Location = new System.Drawing.Point(304, 254);
            this.cbProjNum.Name = "cbProjNum";
            this.cbProjNum.Size = new System.Drawing.Size(54, 21);
            this.cbProjNum.TabIndex = 12;
            this.cbProjNum.Text = "1";
            // 
            // cbOwn
            // 
            this.cbOwn.AutoSize = true;
            this.cbOwn.Checked = true;
            this.cbOwn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOwn.Location = new System.Drawing.Point(15, 281);
            this.cbOwn.Name = "cbOwn";
            this.cbOwn.Size = new System.Drawing.Size(183, 17);
            this.cbOwn.TabIndex = 17;
            this.cbOwn.Text = "Показывать только \"свои\" БД";
            this.cbOwn.UseVisualStyleBackColor = true;
            // 
            // gbSpeed
            // 
            this.gbSpeed.Controls.Add(this.rbNational);
            this.gbSpeed.Controls.Add(this.rbIntl);
            this.gbSpeed.Location = new System.Drawing.Point(307, 282);
            this.gbSpeed.Name = "gbSpeed";
            this.gbSpeed.Size = new System.Drawing.Size(268, 69);
            this.gbSpeed.TabIndex = 18;
            this.gbSpeed.TabStop = false;
            this.gbSpeed.Text = "Правила проведения соревнований";
            // 
            // rbNational
            // 
            this.rbNational.AutoSize = true;
            this.rbNational.Location = new System.Drawing.Point(7, 42);
            this.rbNational.Name = "rbNational";
            this.rbNational.Size = new System.Drawing.Size(131, 17);
            this.rbNational.TabIndex = 0;
            this.rbNational.TabStop = true;
            this.rbNational.Text = "Российские правила";
            this.rbNational.UseVisualStyleBackColor = true;
            // 
            // rbIntl
            // 
            this.rbIntl.AutoSize = true;
            this.rbIntl.Location = new System.Drawing.Point(7, 20);
            this.rbIntl.Name = "rbIntl";
            this.rbIntl.Size = new System.Drawing.Size(154, 17);
            this.rbIntl.TabIndex = 0;
            this.rbIntl.TabStop = true;
            this.rbIntl.Text = "Международные правила";
            this.rbIntl.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(8, 371);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(132, 37);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.m_bCancel_Click);
            // 
            // AccountForm
            // 
            this.AcceptButton = this.m_bOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_bCancel;
            this.ClientSize = new System.Drawing.Size(591, 447);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.gbSpeed);
            this.Controls.Add(this.cbOwn);
            this.Controls.Add(this.cbProjNum);
            this.Controls.Add(this.gb);
            this.Controls.Add(this.rbUseSQL);
            this.Controls.Add(this.rbUseWin);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnbackup);
            this.Controls.Add(this.btnShowData);
            this.Controls.Add(this.tbAdminPwd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCreateDB);
            this.Controls.Add(this.cbDatabase);
            this.Controls.Add(this.m_tbDate);
            this.Controls.Add(this.m_tbPlace);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.compTitle);
            this.Controls.Add(this.m_tbServer);
            this.Controls.Add(this.m_bOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_textServer);
            this.Controls.Add(this.bntDropDB);
            this.Controls.Add(this.m_bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Авторизация";
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.gbSpeed.ResumeLayout(false);
            this.gbSpeed.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        bool nc = true;
        private bool exitPressed = false;
        public bool ExitPressed { get { return exitPressed; } }
        private void m_bCancel_Click(object sender, System.EventArgs e)
        {
            if (m_bOK.Text != "Обновить")
            {
                cnString = "";
                nc = true;
            }
            if (sender == btnExit)
                exitPressed = true;
            this.Close();
        }

        private void m_bOK_Click(object sender, System.EventArgs e)
        {
            //workstation id="CLIMBING-SERVER";
            if (m_bOK.Text == "Обновить")
            {
                if (UpdateCompetitonData())
                    this.Close();
                return;
            }
            if (cbDatabase.Text == "" || cbDatabase.Text == NO_DB || cbDatabase.Text == LOADING_STR)
            {
                MessageBox.Show("База данных не выбрана");
                return;
            }
            cnString = ConnectionString;
            dbName = cbDatabase.Text;
            srvName = m_tbServer.Text;

            ConnectToDB();
        }

        private void ConnectToDB()
        {
            nc = false;
            SaveData();

            adminPwd = tbAdminPwd.Text;


            this.Close();
        }



        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (m_tbServer.Text != "" && ((m_tbUser.Text != "" && m_tbPassword.Text != "") || rbUseWin.Checked))
            {
                //try { FillDatabases(); }
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //    cbDatabase.DroppedDown = false;
                //}
                cbDatabase.Items.Clear();
                cbDatabase.Items.Add(LOADING_STR);
                System.Threading.Thread thr = new System.Threading.Thread(FillDatabases);
                thr.Start(cbOwn.Checked);
            }
        }

        private void FillDatabases(object obCH)
        {
            if (!(obCH is bool))
                return;
            bool bCh = (bool)obCH;
            SqlConnection cn = null;
            try
            {
                cn = CreateConnection("master");
                SqlCommand cmd = new SqlCommand("SELECT name FROM sysdatabases(NOLOCK) ORDER BY name", cn);
                SqlDataReader rdr;

                cn.Open();
                List<string> dbList = new List<string>();
                rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                        dbList.Add(rdr[0].ToString());
                }
                finally { rdr.Close(); }

                foreach (string str in dbList)
                {
                    try
                    {
                        if (bCh)
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
                            if (oTmp == null || oTmp == DBNull.Value || oTmp.ToString() != DB_ID)
                                continue;
                        }
                        if (cbDatabase.InvokeRequired)
                            cbDatabase.Invoke(new EventHandler(delegate
                            {
                                cbDatabase.Items.Add(str);
                            }));
                        else
                            cbDatabase.Items.Add(str);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new EventHandler(delegate
                {
                    MessageBox.Show(this, ex.Message);
                }));
            }
            finally
            {
                if (cn != null && cn.State != System.Data.ConnectionState.Closed)
                    cn.Close();
                try
                {
                    if (cbDatabase.InvokeRequired)
                        cbDatabase.Invoke(new EventHandler(delegate
                        {
                            cbDatabase.Items.Remove(LOADING_STR);
                            if (cbDatabase.Items.Count < 1)
                                cbDatabase.Items.Add(NO_DB);
                        }));
                    else
                    {
                        cbDatabase.Items.Remove(LOADING_STR);
                        if (cbDatabase.Items.Count < 1)
                            cbDatabase.Items.Add(NO_DB);
                    }
                }
                catch { }
            }
        }


        void SetEditMode(bool edit)
        {
            compTitle.ReadOnly = textBox1.ReadOnly = m_tbPlace.ReadOnly = m_tbDate.ReadOnly = !edit;
            gbSpeed.Enabled = edit;
            if (edit)
                btnCreateDB.Text = "Подтвердить";
            else
                btnCreateDB.Text = "Создать новую БД";
            if (m_bOK.Text == "Обновить")
                m_bOK.Enabled = edit;
            else
                m_bOK.Enabled = bntDropDB.Enabled = !edit;
        }

        private void btnCreateDB_Click(object sender, EventArgs e)
        {
            if (btnCreateDB.Text != "Подтвердить")
            {
                rbIntl.Checked = rbNational.Checked = false;
                SetEditMode(true);
                return;
            }
            if (cbDatabase.Text.Length < 1 || cbDatabase.Text == LOADING_STR || cbDatabase.Text == NO_DB)
            {
                MessageBox.Show("Название БД не введено");
                return;
            }
            if (!rbIntl.Checked && !rbNational.Checked)
            {
                MessageBox.Show(this, "Правила проведения соревнований не выбраны.");
                return;
            }
            cn = CreateConnection("master");
            string dbName = cbDatabase.Text;
            srvName = m_tbServer.Text;
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM sysdatabases WHERE name = '" + dbName + "'", cn);

            try { cn.Open(); }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nОшибка подключения к серверу БД"); return; }

            int n = 0;
            try { n = (int)cmd.ExecuteScalar(); }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nВведено неверное имя БД"); return; }

            if (n > 0)
            {
                if (MessageBox.Show("Такая БД уже существует.\r\nВы уверены, что хотите её заменить?\r\nВсе существующие данные в этой БД будут удалены", "Удалить БД?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                
                try { DropDB(dbName); }
                catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nОшибка удаления БД"); return; }
            }

            cmd.CommandText = "CREATE DATABASE " + dbName;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nОшибка создания БД"); return; }

            try
            {
                cmd.CommandText = "ALTER DATABASE [" + dbName + "] SET RECOVERY SIMPLE WITH NO_WAIT";
                cmd.ExecuteNonQuery();
            }
            catch { }


            try
            {
                cn.Close();
                cn = CreateConnection(dbName);
                cn.Open();
                cmd.Connection = cn;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nОшибка соединения с новой БД"); return; }
            //cmd.Connection = cn;

            try
            {
                #region DataTables
                CreateLogData(cmd.Connection);

                BaseForm.FormGenerateSaveTables(cmd.Connection, cmd.Transaction);

                cmd.CommandText = "CREATE TABLE Groups (" +
    "[iid] [int] NOT NULL primary key," +
    "[name] [varchar] (50)  NOT NULL ," +
    "[oldYear] [int] NOT NULL ," +
    "[youngYear] [int] NOT NULL ," +
    "[minQf] [int] NOT NULL DEFAULT 12," +
    @"[genderFemale] [bit] NOT NULL );
   create unique index Groups_grName on Groups(name);
   create unique index Groups_Constr on Groups(oldYear, youngYear, genderFemale)";
                cmd.ExecuteNonQuery();

                CreateQfTable(cmd.Connection);

                cmd.CommandText = "CREATE TABLE Teams (" +
    "[iid] [int] NOT NULL primary key," +
    "[Guest] [bit] NOT NULL CONSTRAINT [DF_Teams_Guest] DEFAULT (0)," +
    "[name] [varchar] (255) COLLATE Cyrillic_General_CI_AS NOT NULL," +
    "[pos] [int] NULL," +
    "[chief] [varchar] (255) COLLATE Cyrillic_General_CI_AS not null default ''," +
    "[chief_ord] [varchar] (255) COLLATE Cyrillic_General_CI_AS not null default ''" +
                @");
    create unique index Teams_TeamName on Teams(name);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dbo].[Participants] (" +
    "[iid] [int] NOT NULL ," +
    "[license] [bigint] NULL ," +
    "[name] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[surname] [varchar] (255) COLLATE Cyrillic_General_CI_AS NOT NULL ," +
    "[name_ord] [varchar] (255) COLLATE Cyrillic_General_CI_AS NOT NULL default ''," +
    "[age] [int] NULL ," +
    "[genderFemale] [bit] NOT NULL ," +
    "[qf] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[team_id] [int] NOT NULL ," +
    "[group_id] [int] NOT NULL ," +
    "[rankingLead] [int] NULL ," +
    "[rankingSpeed] [int] NULL ," +
    "[rankingBoulder] [int] NULL ," +
    "[vk] [bit] NOT NULL CONSTRAINT [DF_Participants_vk] DEFAULT (0)," +
    "[noPoints] [bit] NOT NULL CONSTRAINT [DF_Participants_noPoints] DEFAULT (0)," +
    "[lateAppl] [bit] NOT NULL CONSTRAINT [DF_Participants_lateAppl] DEFAULT (0)," +
    "[lead] [smallint] NOT NULL DEFAULT (1)," +
    "[speed] [smallint] NOT NULL DEFAULT (1)," +
    "[boulder] [smallint] NOT NULL DEFAULT (1)," +
    "[photo] [image] NULL ," +
    "[changed] BIT NOT NULL DEFAULT 1, " +
    "CONSTRAINT [PK_Participants] PRIMARY KEY  CLUSTERED " +
    "(" +
    "	[iid]" +
    ")  ON [PRIMARY] ," +
    "CONSTRAINT [FK_Participants_Groups] FOREIGN KEY " +
    "(" +
    "	[group_id]" +
    ") REFERENCES [dbo].[Groups] (" +
    "	[iid]" +
    ") ON DELETE CASCADE  ON UPDATE CASCADE ," +
    "CONSTRAINT [FK_Participants_Teams1] FOREIGN KEY " +
    "(" +
    "	[team_id]" +
    ") REFERENCES [dbo].[Teams] (" +
    "	[iid]" +
    ") ON DELETE CASCADE  ON UPDATE CASCADE " +
@");
    create index Participants_team on Participants(team_id)
    create index Participants_group on Participants(group_id)";
                cmd.ExecuteNonQuery();

                CreateTeamSecondLinkTable(cmd.Connection, cmd.Transaction);

                cmd.CommandText = "CREATE TABLE [dbo].[judges] (" +
"	[iid] [int] NOT NULL ," +
"	[name] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[surname] [varchar] (255) COLLATE Cyrillic_General_CI_AS NOT NULL ," +
"	[patronimic] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[city] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
                    //"	[pos] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[category] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"   [photo] [image] NULL," +
"   [changed] [bit] NOT NULL DEFAULT 1," +
"	CONSTRAINT [PK_judges] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[iid]" +
"	)" +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dbo].[CompetitionData] (" +
"	[NAME] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[SHORT_NAME] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[DATES] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[PLACE] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[ADMIN] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL, " +
"   [SynchroRequest] [bit] NOT NULL CONSTRAINT [DF_CompetitionData_SynchroRequest]  DEFAULT (0), " +
"	[ClimbingTime] [int] NULL, " +
"	[MinElapsed] [int] NULL, " +
"	[SecElapsed] [int] NULL, " +
"   [remoteString] [varchar] (8000) NULL DEFAULT '', " +
"   [remoteTime] [int] NULL DEFAULT 0, " +
"   [DB_ID] [varchar] (255) NOT NULL DEFAULT '" + DB_ID + "' " +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dbo].[lists] (" +
    "[iid] [int] NOT NULL ," +
    "[group_id] [int] NULL ," +
    "[listType] [varchar] (255) COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT '" + ListTypeEnum.Unknown.ToString() + "', " +
    "[style] [varchar] (50) COLLATE Cyrillic_General_CI_AS NOT NULL ," +
    "[round] [varchar] (50) COLLATE Cyrillic_General_CI_AS NOT NULL ," +
    "[topo] [image] NULL ," +
    "[quote] [int] NULL ," +
    "[allowView] [bit] NOT NULL CONSTRAINT [DF_lists_allowView] DEFAULT (0)," +
    "[workFinished] [bit] NOT NULL CONSTRAINT [DF_lists_workFinished] DEFAULT (0)," +
    "[prev_round] [int] NULL ," +
    "[next_round] [int] NULL ," +
    "[nowClimbing] [int] NULL ," +
    "[nowClimbingTmp] [int] NULL, " +
    "[nowClimbing3] [int] NULL, " +
    "[climbingTime] [int] NULL ," +
    "[routeNumber] [int] NULL ," +
    "[showPhoto] [bit] NOT NULL CONSTRAINT [DF_lists_showPhoto] DEFAULT (0)," +
    "[online] [bit] NOT NULL CONSTRAINT [DF_lists_online] DEFAULT (0)," +
    "[judge_id] [int] NULL ," +
    "[isolationOpen] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[isolationClose] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[observation] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[start] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
    "[routesetter_id] [int] NULL ," +
    "[iid_parent] [int] NULL ," +
    "[IsLocked] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL , " +
    "[changed] BIT NOT NULL DEFAULT 1, " +
                    //"[usePts] BIT NOT NULL DEFAULT 1, " +
                    //"[restrictPoints] BIT NOT NULL DEFAULT 1, " +
    "CONSTRAINT [PK_lists] PRIMARY KEY  CLUSTERED " +
    "(" +
    "	[iid]" +
    ")  ON [PRIMARY] ," +
    "CONSTRAINT [FK_lists_Groups] FOREIGN KEY " +
    "(" +
    "	[group_id]" +
    ") REFERENCES [dbo].[Groups] (" +
    "	[iid]" +
    ")," +

    "CONSTRAINT [FK_lists_judges2] FOREIGN KEY " +
    "(" +
    "	[routesetter_id]" +
    ") REFERENCES [dbo].[judges] (" +
    "	[iid]" +
    ")," +

    "CONSTRAINT [FK_lists_judges] FOREIGN KEY " +
    "(" +
    "	[judge_id]" +
    ") REFERENCES [dbo].[judges] (" +
    "	[iid]" +
    ") ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    CREATE TABLE positions(
                      iid  INT PRIMARY KEY,
                      name VARCHAR(255) NOT NULL,
                   changed BIT NOT NULL DEFAULT 1,
                  forBadge BIT NOT NULL DEFAULT 0
                    )";
                cmd.ExecuteNonQuery();
                cmd.CommandText=@"
                    CREATE TABLE JudgePos(
                      iid BIGINT PRIMARY KEY,
                      judge_id INT NOT NULL FOREIGN KEY REFERENCES judges(iid) ON DELETE CASCADE ON UPDATE CASCADE,
                      pos_id INT NOT NULL FOREIGN KEY REFERENCES positions(iid) ON DELETE CASCADE ON UPDATE CASCADE,
                     changed BIT NOT NULL DEFAULT 1
                    )";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"
                    CREATE VIEW judgeView AS
                      SELECT j.iid, j.name, j.surname, j.patronimic, city, category, ISNULL(p.name,'') pos
                        FROM judges j(NOLOCK)
                   LEFT JOIN JudgePos jp(NOLOCK) ON jp.judge_id = j.iid
                   LEFT JOIN positions p(NOLOCK) ON p.iid = jp.pos_id";
                cmd.ExecuteNonQuery();

                Judges.InitPosList(cn, null);

                CreateOrderTables(cn, null);

                CreateQueryData(cn);
                

                #endregion

                #region ResultTables
                cmd.CommandText = "CREATE TABLE [dbo].[routeResults] (" +
"	[iid] [bigint] NOT NULL ," +
"	[list_id] [int] NOT NULL ," +
"	[climber_id] [int] NOT NULL ," +
"	[start] [int] NOT NULL ," +
"	[resText] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[res] [bigint] NULL ," +
"   [timeText] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"   [timeValue] [bigint] NULL ," +
"	[posText] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[pos] [int] NULL  DEFAULT (" + int.MaxValue.ToString() + ")," +
"	[qf] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[pts] [float] NULL ," +
"	[ptsText] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[prevRoundRoute] [tinyint] NOT NULL CONSTRAINT [DF_routeResults_prevRoundRoute] DEFAULT (0)," +
"	[prevRound] [int] NULL ," +
"   [preQf] [bit] NOT NULL DEFAULT 0, " +
"[changed] BIT NOT NULL DEFAULT 1, " +
"	CONSTRAINT [PK_routeResults] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[iid]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_routeResults_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE ," +
"	CONSTRAINT [FK_routeResults_Participants] FOREIGN KEY " +
"	(" +
"		[climber_id]" +
"	) REFERENCES [dbo].[Participants] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
CREATE TABLE [dbo].[speedResults](
	[iid] [bigint] NOT NULL,
	[climber_id] [int] NOT NULL,
	[list_id] [int] NOT NULL,
	[start] [int] NULL,
	[pos] [int] NULL,
	[posText] [varchar](50) NULL,
	[res] [bigint] NULL,
	[resText] [varchar](50) NULL,
	[route1] [bigint] NULL,
	[route1_text] [varchar](50) NULL,
	[route2] [bigint] NULL,
	[route2_text] [varchar](50) NULL,
	[start2route] [bit] NOT NULL,
	[qf] [varchar](50) NULL,
	[preQf] [bit] NOT NULL,
	[changed] [bit] NOT NULL,
 CONSTRAINT [PK_speedResults_1] PRIMARY KEY CLUSTERED 
(
	[iid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

ALTER TABLE [dbo].[speedResults]  WITH CHECK ADD  CONSTRAINT [FK_speedResults_lists] FOREIGN KEY([list_id])
REFERENCES [dbo].[lists] ([iid])
ON UPDATE CASCADE
ON DELETE CASCADE


ALTER TABLE [dbo].[speedResults] CHECK CONSTRAINT [FK_speedResults_lists]


ALTER TABLE [dbo].[speedResults]  WITH CHECK ADD  CONSTRAINT [FK_speedResults_Participants] FOREIGN KEY([climber_id])
REFERENCES [dbo].[Participants] ([iid])
ON UPDATE CASCADE
ON DELETE CASCADE


ALTER TABLE [dbo].[speedResults] CHECK CONSTRAINT [FK_speedResults_Participants]


ALTER TABLE [dbo].[speedResults] ADD  CONSTRAINT [DF_speedResults_pos]  DEFAULT ((2147483647)) FOR [pos]


ALTER TABLE [dbo].[speedResults] ADD  CONSTRAINT [DF_speedResults_res]  DEFAULT ((9223372036854775807.)) FOR [res]


ALTER TABLE [dbo].[speedResults] ADD  CONSTRAINT [DF_speedResults_start2route]  DEFAULT ((0)) FOR [start2route]


ALTER TABLE [dbo].[speedResults] ADD  DEFAULT ((0)) FOR [preQf]


ALTER TABLE [dbo].[speedResults] ADD  DEFAULT ((1)) FOR [changed]
";
                cmd.ExecuteNonQuery();

                CreateSpeedAdvancedFormatTable(cmd.Connection, cmd.Transaction);

                cmd.CommandText = "CREATE TABLE [dbo].[boulderResults] (" +
"	[iid] [bigint] NOT NULL ," +
"	[climber_id] [int] NOT NULL ," +
"	[list_id] [int] NOT NULL ," +
"	[start] [int] NOT NULL ," +
"	[pos] [int] NULL   DEFAULT (" + int.MaxValue.ToString() + ")," +
"	[posText] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"	[pts] [float] NULL ," +
"	[res] [bigint] NULL ," +
"	[tops] [int] NOT NULL CONSTRAINT [DF_boulderResults_tops] DEFAULT (0)," +
"	[topAttempts] [int] NOT NULL CONSTRAINT [DF_boulderResults_topAttempts] DEFAULT (0)," +
"	[bonuses] [int] NOT NULL CONSTRAINT [DF_boulderResults_bonuses] DEFAULT (0)," +
"	[bonusAttempts] [int] NOT NULL CONSTRAINT [DF_boulderResults_bonusAttempts] DEFAULT (0)," +
"	[qf] [varchar] (50) COLLATE Cyrillic_General_CI_AS NULL ," +
"   [preQf] [bit] NOT NULL DEFAULT 0, " +
"	[top1] [int] NULL ," +
"	[bonus1] [int] NULL ," +
"	[top2] [int] NULL ," +
"	[bonus2] [int] NULL ," +
"	[top3] [int] NULL ," +
"	[bonus3] [int] NULL ," +
"	[top4] [int] NULL ," +
"	[bonus4] [int] NULL ," +
"	[top5] [int] NULL ," +
"	[bonus5] [int] NULL ," +
"	[top6] [int] NULL ," +
"	[bonus6] [int] NULL ," +
"	[top7] [int] NULL ," +
"	[bonus7] [int] NULL ," +
"	[top8] [int] NULL ," +
"	[bonus8] [int] NULL ," +
"	[nya] [bit] NOT NULL CONSTRAINT [DF_boulderResults_nya]  DEFAULT (0), " +
"	[disq] [bit] NOT NULL CONSTRAINT [DF_boulderResults_disq]  DEFAULT (0), " +
"[changed] BIT NOT NULL DEFAULT 1, " +
"	CONSTRAINT [PK_boulderResults] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[iid]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_boulderResults_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE ," +
"	CONSTRAINT [FK_boulderResults_Participants] FOREIGN KEY " +
"	(" +
"		[climber_id]" +
"	) REFERENCES [dbo].[Participants] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();

                MultiRouteBoulder.CheckTableExists(cmd.Connection);

                cmd.CommandText = "CREATE TABLE [dbo].[generalResults] (" +
"	[list_id] [int] NOT NULL ," +
"	[climber_id] [int] NOT NULL ," +
"	[pos] [int] NULL ," +
"	[pts] [real] NOT NULL ," +
"	CONSTRAINT [PK_generalResults] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[list_id]," +
"		[climber_id]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_generalResults_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE ," +
"	CONSTRAINT [FK_generalResults_Participants] FOREIGN KEY " +
"	(" +
"		[climber_id]" +
"	) REFERENCES [dbo].[Participants] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dbo].[teamResults] (" +
"	[list_id] [int] NOT NULL ," +
"	[team_id] [int] NOT NULL ," +
"	[pos] [int] NULL ," +
"	CONSTRAINT [PK_teamResults] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[list_id]," +
"		[team_id]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_teamResults_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE ," +
"	CONSTRAINT [FK_teamResults_Teams] FOREIGN KEY " +
"	(" +
"		[team_id]" +
"	) REFERENCES [dbo].[Teams] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE " +
")";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dbo].[lists_hdr2] (" +
"	[list_id] [int] NOT NULL ," +
"	[use_group] [bit] NOT NULL ," +
"	[male] [int] NOT NULL ," +
"	[female] [int] NULL ," +
"	CONSTRAINT [PK_lists_hdr2] PRIMARY KEY  CLUSTERED " +
"	(" +
"		[list_id]" +
"	)  ON [PRIMARY] ," +
"	CONSTRAINT [FK_lists_hdr2_lists] FOREIGN KEY " +
"	(" +
"		[list_id]" +
"	) REFERENCES [dbo].[lists] (" +
"		[iid]" +
"	) ON DELETE CASCADE  ON UPDATE CASCADE" +
")";
                cmd.ExecuteNonQuery();
                #endregion

                #region StoredProcedures

                SortingClass.CreateTableCompareProc(cmd.Connection, cmd.Transaction);

                cmd.CommandText = "CREATE PROCEDURE [dbo].[DeleteClimber]\r\n" +
"	@iid int\r\n" +
"AS\r\n" +
"DECLARE @riid bigint;\r\n" +
"SET @riid = 0;\r\n" +
"DECLARE	curs CURSOR FOR SELECT iid FROM routeResults WHERE climber_id = @iid;\r\n" +

"OPEN curs;\r\n" +

"FETCH NEXT FROM curs INTO @riid;\r\n" +
"WHILE @@FETCH_STATUS = 0\r\n" +
"BEGIN\r\n" +
"	DELETE FROM result_2routes WHERE(route1_id = @riid) OR (route2_id = @riid);\r\n" +
"	FETCH NEXT FROM curs INTO @riid;\r\n" +
"END\r\n" +

"CLOSE curs;\r\n" +
"DEALLOCATE curs;\r\n" +

"DELETE FROM routeResults WHERE climber_id = @iid;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE PROCEDURE [dbo].[DeleteGroup]\r\n" +
"	@giid int\r\n" +
"AS\r\n" +
"DECLARE @piid int;\r\n" +

"DECLARE cur CURSOR FOR SELECT iid FROM Participants WHERE group_id = @giid;\r\n" +
"OPEN cur;\r\n" +
"FETCH NEXT FROM cur INTO @piid;\r\n" +
"WHILE @@FETCH_STATUS = 0\r\n" +
"BEGIN\r\n" +
"	EXEC DeleteClimber @iid = @piid;\r\n" +
"	FETCH NEXT FROM cur INTO @piid;\r\n" +
"END;\r\n" +
"CLOSE cur;\r\n" +
"DEALLOCATE cur;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE PROCEDURE [dbo].[leadQf]\r\n" +
    "@list_id int\r\n" +
"AS\r\n" +
"SELECT r.posText AS Место, p.surname+' '+p.name AS [Фамилия, Имя], p.age AS [Г.р.],\r\n" +
"p.qf AS Разряд, t.name AS Команда, r1.resText AS [Тр.1], r1.ptsText AS [Балл 1],\r\n" +
"r2.resText AS [Тр.2], r2.ptsText AS [Балл 2],\r\n" +
"r.resText AS [Итог], r.qf AS [Кв.]\r\n" +
"FROM result_2routes r INNER JOIN routeResults r1 ON r.route1_id = r1.iid\r\n" +
"INNER JOIN routeResults r2 ON r.route2_id=r2.iid INNER JOIN Participants p ON\r\n" +
"r1.climber_id=p.iid INNER JOIN teams t ON p.team_id=t.iid\r\n" +
"WHERE (r1.pts IS NOT NULL) AND (r2.pts IS NOT NULL) AND (r.list_id = @list_id)\r\n" +
"ORDER BY (r1.pts*r2.pts), p.vk, t.name, p.surname, p.name";
                cmd.ExecuteNonQuery();
                #endregion

                #region Triggers
                cmd.CommandText = "CREATE  TRIGGER [checkQuote] ON [dbo].[lists]\r\n" +
"AFTER INSERT,UPDATE\r\n" +
"AS\r\n" +
"DECLARE @round varchar(50),\r\n" +
"		@quote real,\r\n" +
"		@giid int,\r\n" +
"		@style varchar(50)\r\n" +
"SELECT @round = i.round, @quote = i.quote, @giid=i.group_id, @style=i.style FROM inserted i;\r\n" +

"IF ((@round = '1/4 финала (2 трассы)') AND (@quote IS NOT NULL))\r\n" +
"BEGIN\r\n" +
"	UPDATE lists SET quote = (@quote/2) WHERE (style=@style) AND (group_id=@giid) AND \r\n" +
"		((round='1/4 финала Трасса 1') OR (round='1/4 финала Трасса 2'))\r\n" +
"END\r\n";
                cmd.ExecuteNonQuery();

                StaticClass.CheckQualyTrigger(cmd.Connection);

                StaticClass.CheckSpeedFunctions(cmd.Connection);

                CheckPointsTable(cmd.Connection, null);

                cmd.CommandText = "CREATE TRIGGER [checkYear] ON [dbo].[Groups]\r\n" +
"AFTER INSERT,UPDATE\r\n" +
"AS\r\n" +
"DECLARE @yearOld int,\r\n" +
"		@yearYoung int\r\n" +
"SELECT @yearOld = oldYear, @yearYoung = youngYear FROM inserted;\r\n" +
"IF @yearOld > @yearYoung\r\n" +
"BEGIN\r\n" +
"   RAISERROR ('Старший год должен быть <= младшего', 16, 1)\r\n" +
"ROLLBACK TRANSACTION\r\n" +
"END";
                try
                {
                    cmd.CommandText = "sp_dboption '" + dbName + "', 'recursive triggers', 'false'";
                    cmd.ExecuteNonQuery();
                }
                catch { }
                cmd.CommandText = "sp_configure 'nested triggers', 0";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "RECONFIGURE";
                cmd.ExecuteNonQuery();

                CreateAllChangedTriggers(cmd.Connection, true);

                cmd.CommandText = "CREATE TRIGGER [ControlRoutes] ON [dbo].[routeResults]\r\n" +
"AFTER INSERT\r\n" +
"AS\r\n" +
"DECLARE @exists int\r\n" +
"SELECT @exists = (SELECT COUNT(r.iid) FROM routeResults r, inserted i WHERE \r\n" +
"i.climber_id = r.climber_id AND i.list_id = r.list_id)\r\n" +
"IF @exists > 1\r\n" +
"BEGIN\r\n" +
"   RAISERROR ('Участник уже есть в протоколе', 16, 1)\r\n" +
"ROLLBACK TRANSACTION\r\n" +
"END";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TRIGGER [listDel] ON [dbo].[lists]\r\n" +
"AFTER DELETE\r\n" +
"AS\r\n" +
"DECLARE @listNext int,\r\n" +
"		@listPrev int,\r\n" +
"		@round varchar(50),\r\n" +
"		@style varchar(50),\r\n" +
"		@giid int,\r\n" +
"		@message varchar(50),\r\n" +
"		@iid int\r\n," +
"		@cnt int\r\n" +
"SELECT @listPrev = d.prev_round, @listNext = d.next_round, @round = round, \r\n" +
"	@giid = group_id, @style = style, @iid = d.iid FROM deleted d;\r\n" +
"UPDATE lists SET next_round = NULL WHERE iid = @listPrev;\r\n" +
"UPDATE lists SET prev_round = NULL WHERE iid = @listNext;\r\n" +
"SELECT @cnt = COUNT(*) FROM lists WHERE iid_parent = @iid;\r\n" +
"IF (@cnt > 0) BEGIN \r\n" +
"DELETE FROM lists WHERE iid_parent = @iid;\r\n" +
"END;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TRIGGER [listSeq] ON [dbo].[lists]\r\n" +
"AFTER INSERT,UPDATE\r\n" +
"AS\r\n" +
"DECLARE @listNext int,\r\n" +
"		@iid int,\r\n" +
"		@tmp int\r\n" +
"SELECT @listNext = i.next_round, @iid = i.iid FROM inserted i;\r\n" +
"IF (@listNext IS NOT NULL)\r\n" +
"BEGIN\r\n" +
"	IF(NOT EXISTS(SELECT * FROM lists WHERE iid = @listNext))\r\n" +
"	BEGIN\r\n" +
"		RAISERROR ('No such list', 16, 1);\r\n" +
"		ROLLBACK TRANSACTION;\r\n" +
"	END\r\n" +
"	ELSE\r\n" +
"		UPDATE lists SET prev_round=@iid WHERE iid = @listNext;\r\n" +
"END";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TRIGGER ParticipantsNP " +
"ON Participants AFTER INSERT " +
"AS " +
"BEGIN " +
"  DECLARE @iid INT, " +
"          @team INT, " +
"          @np BIT, " +
"          @npSelect BIT; " +
"  SELECT @iid = iid, @team = team_id, @npSelect = noPoints FROM inserted; " +
"  IF (SELECT @npSelect) = 0 " +
"  BEGIN " +
"    SELECT @np = Guest FROM teams(NOLOCK) WHERE iid = @team; " +
"    UPDATE participants SET noPoints = @np WHERE iid = @iid;  " +
"  END; " +
"END";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TRIGGER TeamsNP " +
"ON Teams AFTER UPDATE AS " +
"BEGIN " +
"  DECLARE @iid INT, " +
"          @np BIT; " +
"  SELECT @iid = iid, @np = Guest FROM inserted; " +
"  UPDATE participants SET noPoints = @np WHERE team_id = @iid; " +
"END;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TRIGGER ListDel2 ON lists AFTER DELETE AS " +
"BEGIN " +
"  DECLARE @gr_id INT, " +
"          @style VARCHAR(255), " +
"          @iid INT, " +
"          @cnt INT, " +
"          @round VARCHAR(255); " +
"  SELECT @gr_id = group_id, @style = style, @round = round, @iid = iid FROM deleted; " +
"  SELECT @cnt = COUNT(*) FROM lists WHERE iid_parent = @iid; " +
"  IF (@cnt > 0) BEGIN " +
"    DELETE FROM lists WHERE iid_parent = @iid;" +
"  END;" +
"END;";
                cmd.ExecuteNonQuery();



                #endregion

                CreateTeamFunction(cn);

                CreateTopoTable(cn);

                UpdateCompetitonData();

                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nОшибка при созднии БД");
                //try
                //{ cn.Close(); }
                //catch { }
                //cn = CreateConnection("master");
                //try { cn.Open(); }
                //catch { return; }
                //cmd.Connection = cn;
                //cmd.CommandText = "DROP DATABASE " + dbName;
                try
                {
                    DropDB(dbName);
                }
                catch { return; }
            }
            this.cnString = cn.ConnectionString;
            this.dbName = dbName;
            SetEditMode(false);
            ConnectToDB();
        }

        public static void CreateTeamFunction(SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*)" +
                              "  FROM sysobjects(nolock)" +
                              " WHERE name = 'fn_getTeamName'" +
                              "   AND type = 'FN'";
            object o = cmd.ExecuteScalar();
            int cnt = 0;
            if(o != null && o!= DBNull.Value)
                try { cnt = Convert.ToInt32(o); }
                catch { }
            if (cnt > 0){
#if DEBUG
                cmd.CommandText = "DROP FUNCTION dbo.fn_getTeamName";
                cmd.ExecuteNonQuery();
#else
                return;
#endif
            }
            cmd.CommandText = @"
create function dbo.fn_getTeamName(@P_nClimberID int, @P_nListID int) returns varchar(4000)
as begin
  declare @res varchar(4000), @style varchar(50), @appType smallint
  set @res = ''
  set @style = null
  set @appType = null
  select @style = l.style
    from lists l(nolock)
   where iid = @P_nListID
  
  if @style = 'Трудность'
  begin
    select @appType = p.lead
      from Participants p(nolock)
     where p.iid = @P_nClimberID
  end
  else if @style = 'Скорость'
  begin
    select @appType = p.speed
      from Participants p(nolock)
     where p.iid = @P_nClimberID
  end
  else if @style = 'Боулдеринг'
  begin
    select @appType = p.boulder
      from Participants p(nolock)
     where p.iid = @P_nClimberID
  end

  declare @nTeamID int
  set @nTeamID = null  

  select @res = t.name, @nTeamID = t.iid
    from Participants p(nolock)
    join Teams t(nolock) on t.iid = p.team_id
   where p.iid = @P_nClimberID
   
  if @res is null
    set @res = ''
  else begin
      select @res = @res + '-' + t.name
        from teamsLink TL(nolock)
        join Teams T(nolock) on T.iid = TL.team_id
       where TL.climber_id = @P_nClimberID
         and TL.team_id <> @nTeamID
    order by T.name;
  end

  if (@appType is not null and @appType = 2)
    set @res = @res + ' (Л)'
  return @res
end
";
            cmd.ExecuteNonQuery();
        }

        public static void CreateQueryData(SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = @"
  IF OBJECT_ID('prg_Qs') IS NULL
  BEGIN
    CREATE TABLE prg_Qs (
      MACHINE VARCHAR(4096) NOT NULL DEFAULT host_name(),
      REQUEST VARCHAR(255)  NOT NULL,
      PARAM   VARCHAR(255)  NOT NULL,
      sys_date DATETIME     NOT NULL DEFAULT GetDate()
    );
  END;";
            cmd.ExecuteNonQuery();
        }

        public static void CreateLogData(SqlConnection _cn)
        {
            SqlConnection cn = new SqlConnection(_cn.ConnectionString);
            cn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = @"
IF OBJECT_ID('logic_transactions') IS NULL
BEGIN
  CREATE TABLE dbo.logic_transactions(
    iid       BIGINT NOT NULL PRIMARY KEY,
    tran_date DATETIME NOT NULL,
    obj_table VARCHAR(255) NOT NULL,
    obj_id    BIGINT NOT NULL,
    host      VARCHAR(255) NOT NULL DEFAULT host_name(),
    is_active BIT NOT NULL,
    is_ghost  BIT NOT NULL DEFAULT 0
  );
END";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
IF OBJECT_ID('logic_transaction_data') IS NULL
BEGIN
  CREATE TABLE dbo.logic_transaction_data(
    iid                BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.logic_transactions(iid) ON DELETE CASCADE ON UPDATE CASCADE,
    iid_line           BIGINT NOT NULL PRIMARY KEY,
    action             char(1) NOT NULL,
    table_name         varchar(255) NOT NULL,
    table_id           BIGINT       NOT NULL,
    mod_col            varchar(255) NOT NULL,
    iid_col            varchar(255) NOT NULL,
    old_value          varchar(MAX) NULL,
    new_value          varchar(MAX) NULL,
    iid_modified       BIT NOT NULL,
    default_conversion varchar(max) NULL,
    def_conv_new       varchar(max) NULL,
    order_insert       INT NOT NULL
  );
END";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT OBJECT_ID('rollback_logic_transaction') objid";
                    object oTmp = cmd.ExecuteScalar();

#if !DEBUG
                    bool needCreate = true;
#endif
                    if (oTmp != null && oTmp != DBNull.Value)
                    {
#if DEBUG
                        cmd.CommandText = "DROP PROCEDURE dbo.rollback_logic_transaction";
                        cmd.ExecuteNonQuery();
#else
                        needCreate = false;
#endif
                    }

#if !DEBUG
                    if (needCreate)
                    {
#endif
                        cmd.CommandText = @"
  CREATE PROCEDURE dbo.rollback_logic_transaction (@P_nTranID BIGINT, @RP_bSuccess BIT OUTPUT) AS
  BEGIN
    SET @RP_bSuccess = 1;
    DECLARE @b BIT;
    SET @b = NULL;
    SELECT @b = is_active
      FROM logic_transactions(NOLOCK)
     WHERE iid = @P_nTranID;
    IF (@b IS NULL OR @b = 0) BEGIN
      SET @RP_bSuccess = 0
      RETURN; 
    END

    BEGIN TRAN
    DECLARE @action char(1), @table_name varchar(255), @table_id BIGINT, @mod_col VARCHAR(255),@iid_col VARCHAR(255),
            @old_value varchar(MAX), @new_value varchar(MAX), @iid_modified BIT, @def_conv VARCHAR(MAX),
            @sQry VARCHAR(8000)
    DECLARE hc_TRANS CURSOR FAST_FORWARD LOCAL FOR
       SELECT action, table_name, table_id, mod_col, iid_col, old_value, new_value, iid_modified, default_conversion
         FROM logic_transaction_data(NOLOCK)
        WHERE iid = @P_nTranID
     ORDER BY order_insert DESC;
    OPEN hc_TRANS
    WHILE(1=1) BEGIN
      FETCH NEXT FROM hc_TRANS INTO @action, @table_name, @table_id, @mod_col, @iid_col, @old_value,
                                    @new_value, @iid_modified, @def_conv;
      IF @@FETCH_STATUS = -1 BREAK;
      IF @@FETCH_STATUS = -2 CONTINUE;
      SET @sQry = NULL
      /*IF @action = 'A' BEGIN
        SET @sQry = 'DELETE FROM ' + @table_name + ' WHERE ' + @iid_col + ' = ' + CONVERT(VARCHAR,@table_id);
      END
      ELSE */IF @action = 'U' BEGIN
        SET @sQry = 'UPDATE ' + @table_name + ' SET ' + @mod_col + ' = '
        IF @def_conv IS NOT NULL AND @def_conv <> ''
          SET @sQry = @sQry + @def_conv;
        ELSE
          SET @sQry = @sQry + @old_value;
        SET @sQry = @sQry + ' WHERE ' + @iid_col + ' = ';
        IF @iid_modified = 1
          SET @sQry = @sQry + @new_value;
        ELSE
          SET @sQry = @sQry + CONVERT(VARCHAR,@table_id);
      END
      ELSE BEGIN
        SET @RP_bSuccess = 0;
        BREAK;
      END
      IF @sQry IS NULL
        CONTINUE;
      BEGIN TRY
        EXEC(@sQry);
      END TRY
      BEGIN CATCH
        SET @RP_bSuccess = 0;
        BREAK;
      END CATCH;
    END
    CLOSE hc_TRANS;
    DEALLOCATE hc_TRANS;
    IF (@RP_bSuccess = 0) BEGIN
      ROLLBACK TRAN;
      RETURN;
    END;

    DECLARE @prevTranID BIGINT
    SET @prevTranID = NULL
      SELECT TOP 1 @prevTranID = PT.iid
        FROM logic_transactions CT(NOLOCK)
        JOIN logic_transactions PT(NOLOCK) ON PT.obj_table = CT.obj_table
                                          AND PT.obj_id    = CT.obj_id
                                          AND PT.tran_date < CT.tran_date
       WHERE CT.iid = @P_nTranID
    ORDER BY PT.tran_date DESC
    IF @prevTranID IS NOT NULL
      UPDATE logic_transactions SET is_active = 1 WHERE iid = @prevTranID;

    UPDATE logic_transactions SET is_active = 0 WHERE iid = @P_nTranID;

    SET @RP_bSuccess = 1

    COMMIT TRAN;
  END
";
                        cmd.ExecuteNonQuery();
#if !DEBUG
                    }
#endif

                    cmd.CommandText = "SELECT OBJECT_ID('restore_logic_transaction') objid";
                    oTmp = cmd.ExecuteScalar();

#if !DEBUG
                    needCreate = true;
#endif
                    if (oTmp != null && oTmp != DBNull.Value)
                    {
#if DEBUG
                            cmd.CommandText = "DROP PROCEDURE dbo.restore_logic_transaction";
                            cmd.ExecuteNonQuery();
#else
                        needCreate = false;
#endif
                    }

#if !DEBUG
                    if (needCreate)
                    {
#endif
                        cmd.CommandText = @"
CREATE PROC dbo.restore_logic_transaction(@P_nTranID BIGINT, @RP_bSuccess BIT OUTPUT)
AS BEGIN
  SET @RP_bSuccess = 1  
    DECLARE @objId bigint, @objTable varchar(255)
    SELECT @objId = NULL, @objTable = NULL
    
    SELECT @objId = obj_id, @objTable = obj_table
      FROM logic_transactions(NOLOCK)
     WHERE iid = @P_nTranID;
     
    IF (@objId IS NULL OR @objTable IS NULL) BEGIN
      SET @RP_bSuccess = 0;
      RETURN;
    END;

    BEGIN TRAN
    DECLARE @action char(1), @table_name varchar(255), @table_id BIGINT, @mod_col VARCHAR(255),@iid_col VARCHAR(255),
            @old_value varchar(MAX), @new_value varchar(MAX), @iid_modified BIT, @def_conv VARCHAR(MAX),
            @sQry VARCHAR(8000)
    DECLARE hc_TRANS CURSOR FAST_FORWARD LOCAL FOR
       SELECT action, table_name, table_id, mod_col, iid_col, old_value, new_value, iid_modified, def_conv_new
         FROM logic_transaction_data(NOLOCK)
        WHERE iid = @P_nTranID
     ORDER BY order_insert;
    OPEN hc_TRANS
    WHILE(1=1) BEGIN
      FETCH NEXT FROM hc_TRANS INTO @action, @table_name, @table_id, @mod_col, @iid_col, @old_value,
                                    @new_value, @iid_modified, @def_conv;
      IF @@FETCH_STATUS = -1 BREAK;
      IF @@FETCH_STATUS = -2 CONTINUE;
      SET @sQry = NULL
      IF @action = 'U' BEGIN
        SET @sQry = 'UPDATE ' + @table_name + ' SET ' + @mod_col + ' = '
        IF @def_conv IS NOT NULL AND @def_conv <> ''
          SET @sQry = @sQry + @def_conv;
        ELSE
          SET @sQry = @sQry + @new_value;
        SET @sQry = @sQry + ' WHERE ' + @iid_col + ' = ';
        IF @iid_modified = 1
          SET @sQry = @sQry + @old_value;
        ELSE
          SET @sQry = @sQry + CONVERT(VARCHAR,@table_id);
      END
      ELSE BEGIN
        SET @RP_bSuccess = 0;
        BREAK;
      END
      IF @sQry IS NULL
        CONTINUE;
      BEGIN TRY
        EXEC(@sQry);
      END TRY
      BEGIN CATCH
        SET @RP_bSuccess = 0;
        BREAK;
      END CATCH;
    END
    CLOSE hc_TRANS;
    DEALLOCATE hc_TRANS;
    IF (@RP_bSuccess = 0) BEGIN
      ROLLBACK TRAN;
      RETURN;
    END;

    UPDATE logic_transactions SET is_active = 0 WHERE obj_id = @objId AND obj_table = @objTable

    UPDATE logic_transactions SET is_active = 1 WHERE iid = @P_nTranID;

    SET @RP_bSuccess = 1

    COMMIT TRAN;
  END
";
                        cmd.ExecuteNonQuery();
#if !DEBUG
                    }
#endif

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
            finally { cn.Close(); }
        }

        public static void CreateQfTable(SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) FROM sysobjects WHERE name = 'qfList' AND type = 'U'";
            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                return;
            cmd.CommandText = @"
                CREATE TABLE qfList(
                  iid INT NOT NULL PRIMARY KEY,
                  qf VARCHAR(4) NOT NULL
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO qfList(iid,qf) VALUES(@iid, @qf)";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@qf", SqlDbType.VarChar, 4);
            cmd.Parameters[0].Value = 1;
            cmd.Parameters[1].Value = "ЗМС";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "МСМК";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "МС";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "КМС";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "1";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "2";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "3";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "1ю";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "2ю";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "3ю";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "б/р";
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = (int)cmd.Parameters[0].Value + 1;
            cmd.Parameters[1].Value = "";
            cmd.ExecuteNonQuery();
        }

        public static void CreateAllChangedTriggers(SqlConnection cn)
        {
            CreateAllChangedTriggers(cn, false);
        }

        private static void CreateAllChangedTriggers(SqlConnection cn, bool allowExceptions)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                if (cn.State != System.Data.ConnectionState.Open)
                    cn.Open();
                cmd.Connection = cn;
                CreateChangedTrigger("lists", cmd);
                CreateListTrigger("routeResults", cmd);
                CreateListTrigger("speedResults", cmd);
                CreateListTrigger("boulderResults", cmd);
                CreateChangedTrigger("Participants", cmd);
                CreateChangedTrigger("Judges", cmd);
                CreateChangedTrigger("positions", cmd);
                CreateChangedTrigger("JudgePos", cmd);
                CreateChangedTrigger("Groups", cmd);
                CreateChangedTrigger("Teams", cmd);
            }
            catch (Exception ex)
            {
                if (allowExceptions)
                    throw;
                else
                    MessageBox.Show("Ошибка создания триггеров\r\n" + ex.Message);
            }
        }

        private static void CreateListTrigger(string tableName, SqlCommand cmd)
        {
            SortingClass.CheckColumn(tableName, "changed", "BIT NOT NULL DEFAULT 1", cmd.Connection);
            cmd.CommandText = "SELECT COUNT(*) cnt FROM sysobjects WHERE name='setChanged" + tableName + "' AND type='TR'";
            int k = Convert.ToInt32(cmd.ExecuteScalar());
            if (k > 0)
                return;
            cmd.CommandText = @"
  CREATE TRIGGER [setChanged" + tableName + "] ON [dbo].[" + tableName + @"]
  AFTER UPDATE
  AS
";
            string sUpd = "Update(res)";
            if (tableName.ToLower().IndexOf("boulder") > -1)
            {
                sUpd = "(" + sUpd;
                for (int i = 1; i < 9; i++)
                    sUpd += " OR Update(top" + i.ToString() + ") OR Update(bonus" + i.ToString() + ")";
                sUpd += ")";
            }
                cmd.CommandText += " IF ("+sUpd+" AND NOT Update(changed))";
            cmd.CommandText += @" BEGIN
    UPDATE " + tableName + @" SET changed=1 WHERE iid IN (SELECT iid FROM inserted)
  END";
            cmd.ExecuteNonQuery();
        }

        private static void CreateChangedTrigger(string tableName, SqlCommand cmd)
        {
            SortingClass.CheckColumn(tableName, "changed", "BIT NOT NULL DEFAULT 1", cmd.Connection);
            cmd.CommandText = "SELECT COUNT(*) cnt FROM sysobjects WHERE name='setChanged" + tableName + "' AND type='TR'";
            int k = Convert.ToInt32(cmd.ExecuteScalar());
            if (k > 0)
                return;
  cmd.CommandText = @"
  CREATE TRIGGER [setChanged" + tableName + "] ON [dbo].[" + tableName + @"]
  AFTER UPDATE
  AS
  IF NOT Update(changed) BEGIN
    UPDATE " + tableName + @" SET changed=1 WHERE iid IN (SELECT iid FROM inserted)
  END";
            cmd.ExecuteNonQuery();
        }

        public static void CheckPointsTable(SqlConnection cn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (tran == null)
                cmd.Transaction = cn.BeginTransaction();
            else
                cmd.Transaction = tran;
            bool transactionSuccess = false;
            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM sysobjects WHERE name='Points' AND type = 'U'";
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    return;
                cmd.CommandText = @"
                  CREATE TABLE Points(
                     pos INT PRIMARY KEY,
                     pts FLOAT NOT NULL DEFAULT 0.0
                  )";
                cmd.ExecuteNonQuery();
                LoadStandartPoints(cn, cmd.Transaction);
                transactionSuccess = true;
            }
            finally
            {
                if (tran == null)
                {
                    if (transactionSuccess)
                        cmd.Transaction.Commit();
                    else
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                }
            }
        }

        public static void OrderPoints(SqlConnection cn, SqlTransaction tran)
        {
            CheckPointsTable(cn, tran);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (tran == null)
                cmd.Transaction = cn.BeginTransaction();
            else
                cmd.Transaction = tran;
            bool trSuccess = false;
            try
            {
                cmd.CommandText = "SELECT pts FROM Points WHERE pos = 0";
                object oTmp = cmd.ExecuteScalar();
                double zeroPts;
                if (oTmp == null || oTmp == DBNull.Value)
                    zeroPts = -1.0;
                else
                {
                    zeroPts = Convert.ToDouble(oTmp);
                    cmd.CommandText = "DELETE FROM Points WHERE pos = 0";
                    cmd.ExecuteNonQuery();
                }
                List<double> pointsList = new List<double>();
                cmd.CommandText = "SELECT pts FROM Points WHERE pts > 0 ORDER BY pos";
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    while (rdr.Read())
                        if (rdr[0] != DBNull.Value)
                            pointsList.Add(Convert.ToDouble(rdr[0]));
                }
                finally { rdr.Close(); }
                cmd.CommandText = "DELETE FROM Points";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Points(pos,pts) VALUES (@pos, @pts)";
                cmd.Parameters.Add("@pos", SqlDbType.Int);
                cmd.Parameters.Add("@pts", SqlDbType.Float);
                if (zeroPts > 0.0)
                {
                    cmd.Parameters[0].Value = 0;
                    cmd.Parameters[1].Value = zeroPts;
                    cmd.ExecuteNonQuery();
                }
                for (int i = 0; i < pointsList.Count; i++)
                {
                    cmd.Parameters[0].Value = (i + 1);
                    cmd.Parameters[1].Value = pointsList[i];
                    cmd.ExecuteNonQuery();
                }
                trSuccess = true;
            }
            finally
            {
                if (tran == null)
                {
                    if (trSuccess)
                        cmd.Transaction.Commit();
                    else
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                }
            }
        }

        public static void LoadStandartPoints(SqlConnection cn, SqlTransaction tran)
        {
            double[] pts = new double[30];
            pts[0] = 100.0;
            pts[1] = 80.0;
            pts[2] = 65.0;
            pts[3] = 55.0;
            pts[4] = 51.0;
            pts[5] = 47.0;
            for (int i = 6; i < 12; i++)
                pts[i] = 43 - 3 * (i - 6);
            for (int i = 12; i < 21; i++)
                pts[i] = 26 - 2 * (i - 12);
            for (int i = 21; i < 30; i++)
                pts[i] = (double)(30 - i);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (tran == null)
                cmd.Transaction = cn.BeginTransaction();
            else
                cmd.Transaction = tran;
            bool transactionSuccess = false;
            try
            {
                cmd.CommandText = "DELETE FROM Points";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Points(pos,pts) VALUES(@pos,@pts)";
                cmd.Parameters.Add("@pos", SqlDbType.Int);
                cmd.Parameters.Add("@pts", SqlDbType.Float);
                for (int i = 0; i < pts.Length; i++)
                {
                    cmd.Parameters[0].Value = (i + 1);
                    cmd.Parameters[1].Value = pts[i];
                    cmd.ExecuteNonQuery();
                }
                transactionSuccess = true;
            }
            finally
            {
                if (tran == null)
                {
                    if (transactionSuccess)
                        cmd.Transaction.Commit();
                    else
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                }
            }
        }

        public static void AlterParticipantsTable(SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = (tran == null) ? cn.BeginTransaction() : tran;
            try
            {
                cmd.CommandText = "SELECT c.name, t.name xtype" +
                                  "  FROM sysobjects o(nolock)" +
                                  "  JOIN syscolumns c(nolock) on c.id = o.id" +
                                  "  JOIN systypes t(nolock) on t.xtype = c.xtype" +
                                  " WHERE o.name = 'Participants'" +
                                  "   AND c.name IN('lead', 'speed', 'boulder')" +
                                  "   AND t.name <> 'smallint'";
                List<string> columns = new List<string>(), constraints = new List<string>();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        columns.Add(rdr["name"] as string);
                foreach (var s in columns)
                {
                    constraints.Clear();
                    cmd.CommandText = "SELECT name" +
                                      "  FROM sys.default_constraints(nolock)" +
                                      " WHERE name LIKE '%Participants%" + s + "%'";
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            constraints.Add(rdr["name"] as string);
                    foreach (var c in constraints)
                    {
                        cmd.CommandText = "ALTER TABLE Participants DROP CONSTRAINT " + c;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "ALTER TABLE Participants ALTER COLUMN " + s + " SMALLINT NOT NULL";
                    cmd.ExecuteNonQuery();
                }
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (tran == null)
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                throw ex;
            }
        }

        public static void CreateTopoTable(SqlConnection cn, SqlTransaction _tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction tran = (_tran == null ? cn.BeginTransaction() : _tran);
            try
            {
                SortingClass.CheckColumn("lists", "topo", "image null", cn, tran);
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    Transaction = tran
                };
                cmd.CommandText = @"
if not exists(select 1
                from sysobjects o(nolock)
               where o.name = 'topo_holds'
                 and o.type = 'U') begin
  create table dbo.topo_holds(
    iid int primary key,
    list_id int foreign key references Lists(iid) on delete cascade on update cascade,
    hold_id varchar(20) not null,
    x int not null,
    y int not null
  )

  create index topo_holds_XI1 on topo_holds(list_id, hold_id)
end
";
                cmd.ExecuteNonQuery();
                if (_tran == null)
                    tran.Commit();
            }
            catch
            {
                if (_tran == null)
                    tran.Rollback();
                throw;
            }
        }

        public static void CreateOrderTables(SqlConnection cn, SqlTransaction _tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = (_tran == null ? cn.BeginTransaction() : _tran);
            try
            {
                cmd.CommandText = @"
if not exists(select 1
                from sysobjects(nolock)
               where name = 'orders'
                 and type = 'U') begin
  create table dbo.orders(
      iid int primary key,
      team_id int not null foreign key references Teams(iid) on delete cascade on update cascade,
      climber_id int not null,
      order_number int not null,
      order_date datetime not null,
      order_amount decimal(18,2) not null,
      order_file varchar(4096) COLLATE Cyrillic_General_CI_AS not null,
      order_data image not null)

  create index orders_number on orders(order_number)

  create unique index orders_clm on orders(climber_id)

  create index fk_team on orders(team_id)

  create index of_team on orders(team_id, order_file)
end";
                cmd.ExecuteNonQuery();
#if DEBUG
                cmd.CommandText = @"
if exists(select 1
            from sysindexes(nolock)
           where name = 'orders_num') begin
  drop index orders.orders_num
  create index orders_number on orders(order_number)
end";
                cmd.ExecuteNonQuery();
#endif
                cmd.CommandText = @"
if not exists(select 1
                from sysobjects(nolock)
               where name = 'order_params'
                 and type = 'U') begin
  create table dbo.order_params(
      param_id varchar(20) primary key,
      param_value varchar(4096) COLLATE Cyrillic_General_CI_AS null
  )
end";
                cmd.ExecuteNonQuery();

                if (_tran == null)
                    cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (_tran == null)
                    cmd.Transaction.Rollback();
                throw ex;
            }
        }

        public static void CreateTeamSecondLinkTable(SqlConnection cn, SqlTransaction _tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran;
            cmd.CommandText = @"
  IF NOT EXISTS(SELECT 1
                  FROM sysobjects(NOLOCK)
                 WHERE name = 'teamsLink'
                   AND type = 'U') BEGIN
    CREATE TABLE dbo.teamsLink (
      iid INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
      team_id INT NOT NULL FOREIGN KEY REFERENCES Teams(iid) ON DELETE NO ACTION ON UPDATE NO ACTION,
      climber_id INT NOT NULL FOREIGN KEY REFERENCES Participants(iid) ON DELETE CASCADE ON UPDATE CASCADE,
    );
    CREATE UNIQUE INDEX teamLink_T_C ON teamsLink(team_id, climber_id);
    CREATE INDEX teamLink_T ON teamsLink(team_id);
    CREATE INDEX teamLink_C ON teamsLink(climber_id);
  END";
            cmd.ExecuteNonQuery();
        }

        public static void CreateSpeedAdvancedFormatTable(SqlConnection cn, SqlTransaction _tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran;

            cmd.CommandText = @"
if not exists(select 1
                from sysobjects o(nolock)
               where o.name = 'speedResults_Runs'
                 and type = 'U') begin
  create table speedResults_Runs(
     iid         bigint not null primary key,
     iid_parent  bigint not null foreign key references speedResults(iid) on delete no action on update no action,
     iid_parent_comp bigint null foreign key references speedResults(iid) on delete no action on update no action,
     run_no      int not null,
     result      bigint,
     res_text    varchar(50),
     start2route bit not null
  )
end";
            cmd.ExecuteNonQuery();

            if (!CheckObjectExistense(cn, "dbo.update_SpeedRun", "P", _tran))
            {
                cmd.CommandText = @"
create proc dbo.update_SpeedRun(@P_nClimberLeft int,
								@P_nClimberRight int,
								@P_nResLeft bigint,
								@P_sResLeft varchar(50),
								@P_nResRight bigint,
								@P_sResRight varchar(50),
								@P_nListID int,
								@P_nRunNo int)
AS BEGIN
  if (@P_nClimberLeft is not null) begin
    update speedResults_Runs
       set result = case when ISNULL(@P_sResLeft, '') = '' then NULL else @P_nResLeft end,
           res_text = case when ISNULL(@P_sResLeft, '') = '' then NULL else @P_sResLeft end
      from speedResults_Runs SR(nolock)
      join speedResults S(nolock) on S.iid = SR.iid_parent
     where S.list_id = @P_nListID
       and S.climber_id = @P_nClimberLeft
       and SR.run_no = @P_nRunNo
  end
  
  if(@P_nClimberRight is not null) begin
    update speedResults_Runs
       set result = case when ISNULL(@P_sResRight, '') = '' then NULL else @P_nResRight end,
           res_text = case when ISNULL(@P_sResRight, '') = '' then NULL else @P_sResRight end
      from speedResults_Runs SR(nolock)
      join speedResults S(nolock) on S.iid = SR.iid_parent
     where S.list_id = @P_nListID
       and S.climber_id = @P_nClimberRight
       and SR.run_no = @P_nRunNo
  end
  
  declare @resLeft bigint, @resRight bigint, @pointsLeft bigint, @pointsRight bigint, @hasRes bit
  declare @textLeft varchar(50), @textRight varchar(50), @sTotalLeft varchar(max), @sTotalRight varchar(max)
  declare @fallsLeft bit, @fallsRight bit
  declare @bestLeft varchar(50), @bestRight varchar(50), @bestNLeft bigint, @bestNRight bigint
  set @bestLeft = null
  set @bestRight = null
  set @bestNLeft = null
  set @bestNRight = null
  set @fallsLeft = 1
  set @fallsRight = 1
  set @pointsLeft = 0
  set @pointsRight = 0
  set @hasRes = 0
  set @sTotalLeft = null
  set @sTotalRight = null
  
  declare crs_Results cursor fast_forward local
  for select R1.result, R2.result, LTRIM(RTRIM(R1.res_text)), LTRIM(RTRIM(R2.res_text))
        from lists L(nolock)
   left join speedResults S1(nolock) on S1.list_id = L.iid and S1.climber_id = @P_nCLimberLeft
   left join speedResults_Runs R1(nolock) on R1.iid_parent = S1.iid
   left join speedResults S2(nolock) on S2.list_id = L.iid and S2.climber_id = @P_nCLimberRight
   left join speedResults_Runs R2(nolock) on R2.iid_parent = S2.iid
       where L.iid = @P_nListID
         and (R1.run_no = R2.run_no
             or R1.iid is null
             or R2.iid is null)
         and (R1.iid is not null or R2.iid is not null)
    order by ISNULL(R1.run_no, R2.run_no)
  open crs_Results
  while(1=1) begin
    fetch next from crs_Results into @resLeft, @resRight, @textLeft, @textRight
    if @@FETCH_STATUS = -2 continue
    if @@FETCH_STATUS < 0 break
    if ((@resLeft is null) AND (@resRight is null))
      continue
    if (@hasRes = 0)
      set @hasRes = 1
    if(@resLeft is not null AND @resLeft >= 6000000)
      set @resLeft = 6000000
    if(@resRight is not null AND @resRight >= 6000000)
       set @resRight = 6000000
    if(@resLeft is null)
      set @pointsRight = @pointsRight + 1
    else if(@resRight is null)
      set @pointsLeft = @pointsLeft + 1
    else if (@resLeft < @resRight)
      set @pointsLeft = @pointsLeft + 1
    else if (@resRight < @resLeft)
      set @pointsRight = @pointsRight + 1
    if(@textLeft is not null) begin
      if(@sTotalLeft is null)
        set @sTotalLeft = ' ' + @textLeft
      else
        set @sTotalLeft = @sTotalLeft + ', ' + @textLeft
    end  
    if(@textRight is not null) begin
      if(@sTotalRight is null)
        set @sTotalRight = ' ' + @textRight
      else
        set @sTotalRight = @sTotalRight + ', ' + @textRight
    end
    
    if(@resLeft is not null) begin
      if(@fallsLeft = 1 and @resLeft < 6000000)
        set @fallsLeft = 0
      if (@bestNLeft is null OR @resLeft < @bestNLeft)
      begin
        set @bestLeft = @textLeft
        set @bestNLeft = @resLeft
      end
    end
    if(@resRight is not null) begin
      if (@fallsRight = 1 and @resRight < 6000000)
        set @fallsRight = 0
      if(@bestNRight is null OR @resRight < @bestNRight)
      begin
        set @bestRight = @textRight
        set @bestNRight = @resRight
      end
    end
  end
  close crs_Results
  deallocate crs_Results
  
  if(@sTotalLeft is not null and LEN(@sTotalLeft) > 50)
    set @sTotalLeft = LEFT(@sTotalLeft, 50)
  if(@sTotalRight is not null and LEN(@sTotalRight) > 50)
    set @sTotalRight = LEFT(@sTotalRight, 50)
    
  if(@hasRes = 0) begin
    set @pointsLeft = " + long.MaxValue.ToString() + @"
    set @pointsRight = @pointsLeft
    set @textLeft = null
    set @textRight = null
  end
  else
  begin
    set @textLeft = ' ' + CONVERT(varchar, @pointsLeft) + ':' + CONVERT(varchar, @pointsRight)
    set @textRight = ' ' + CONVERT(varchar,@pointsRight) + ':' + CONVERT(varchar, @pointsLeft)
    set @pointsLeft = 100 - @pointsLeft
    set @pointsRight = 100 - @pointsRight
  end
  if(@P_nClimberLeft is not null) begin
    update speedResults
       set res = case when (@hasRes=1 AND @fallsLeft=1) then 12100000 else @pointsLeft end,
           resText = @textLeft,
           route1 = @pointsLeft,
           route1_text = case when (@P_nRunNo % 2) = 1 then @sTotalLeft else '-' end,
           route2 = 0,
           route2_text = case when (@P_nRunNo % 2) = 0 then @sTotalLeft else '-' end
     where list_id = @P_nListID
       and climber_id = @P_nClimberLeft
  end
  if(@P_nClimberRight is not null) begin
    update speedResults
       set res = case when (@hasRes=1 AND @fallsRight=1) then 12100000 else @pointsRight end,
           resText = @textRight,
           route1 = 0,
           route1_text = case when (@P_nRunNo % 2) = 0 then @sTotalRight ELSE '-' end,
           route2 = @pointsRight,
           route2_text = case when (@P_nRunNo % 2) = 1 then @sTotalRight ELSE '-' end
     where list_id = @P_nListID
       and climber_id = @P_nClimberRight
  end
  
  if ((@P_nClimberLeft is not null) and (@P_nCLimberRight is not null) and(@pointsLeft = @pointsRight) and (@P_nRunNo = 3))
  begin
    update speedResults
       set res = @bestNLeft, resText = @bestLeft
     where climber_id = @P_nClimberLeft and list_id = @P_nListID

    update speedResults
       set res = @bestNRight, resText = @bestRight
     where climber_id = @P_nClimberRight and list_id = @P_nListID
  end

  if ((@P_nClimberLeft is not null) and (@P_nClimberRight is not null) and (@pointsLeft <> @pointsRight))
  begin
    declare @nUpdate int
    if(@pointsLeft > @pointsRight) begin
      set @bestRight = @bestLeft
      set @bestNRight = @bestNLeft
      set @nUpdate = @P_nClimberLeft
    end
    else
      set @nUpdate = @P_nClimberRight

   declare @nCorr bigint
   set @nCorr = 0
   select @nCorr = ISNULL(SP.pos,0)
     from lists L(nolock) 
     join speedResults SP(nolock) on SP.list_id = L.iid and SP.climber_id = @nUpdate
    where L.next_round = @P_nListID

   if(isnull(@nCorr,10000) > 1000)
     set @nCorr = 0
    
      
     update speedResults
        set res = @bestNRight - @nCorr,
            resText= @bestRight
       from speedResults SR(nolock)
      where SR.list_id = @P_nListID
        and SR.climber_id = @nUpdate
  end
END
";
                cmd.ExecuteNonQuery();
            }

//            cmd.CommandText = "select count(*) cnt"+
//                              "  from sysobjects o(nolock)"+
//                              " where name = 'fn_GetResTextSpeed'"+
//                              "   and type = 'FN'";
//            object o = cmd.ExecuteScalar();
//            bool exists;
//            if (o == null || o == DBNull.Value || (Convert.ToInt32(o) < 1))
//                exists = false;
//            else
//                exists = true;
//#if DEBUG
//            if (exists)
//            {
//                cmd.CommandText = "drop function dbo.fn_GetResTextSpeed";
//                cmd.ExecuteNonQuery();
//            }
//#else
//            if(exists)
//                return;
//#endif

        }

        private static bool CheckObjectExistense(SqlConnection cn, string objectName, string objectType, SqlTransaction _tran = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran;
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM sysobjects O(nolock)" +
                              " WHERE O.name = @P_sName" +
                              "   AND O.type = @P_sType";
            cmd.Parameters.Add("@P_sName", SqlDbType.VarChar, 255).Value = objectName.Replace("dbo.", String.Empty);
            cmd.Parameters.Add("@P_sType", SqlDbType.VarChar, 255).Value = objectType;
            object res = cmd.ExecuteScalar();
            if (res == null || res == DBNull.Value || Convert.ToInt32(res) < 1)
                return false;
#if DEBUG
            cmd.CommandText = "DROP ";
            switch (objectType.ToUpper())
            {
                case "P":
                    cmd.CommandText += "PROCEDURE";
                    break;
                case "U":
                    cmd.CommandText += "TABLE";
                    break;
                case "FN":
                    cmd.CommandText += "FUNCTION";
                    break;
                default:
                    return true;
            }
            cmd.CommandText += " " + objectName;
            cmd.ExecuteNonQuery();
            return false;
#else
            return true;
#endif
        }

        private SqlConnection CreateConnection(string dbName)
        {
            if (rbUseSQL.Checked)
                return CreateConnection(m_tbServer.Text, m_tbUser.Text, m_tbPassword.Text, dbName);
            else
                return CreateConnection(m_tbServer.Text, dbName);
        }

        public static SqlConnection CreateConnection(string server, string dbName)
        {
            SqlConnection cnt;
            cnt = new SqlConnection(
            "Data Source=" + server + ";" +
            "Initial Catalog=" + dbName + ";" +
            "Integrated Security=True");
            return cnt;
        }

        public static SqlConnection CreateConnection(string server, string user, string password, string dbName)
        {
            SqlConnection cnt;
            cnt = new SqlConnection("packet size=4096;" +
            "user id=" + user + ";" +
            "data source=" + server + ";" +
            "persist security info=True;" +
            "initial catalog=" + dbName + ";" +
            "password=" + password);
            return cnt;
        }

        private void DropDB(string dbName)
        {
            using (SqlConnection cnL = CreateConnection("master"))
            {
                if (cnL.State != ConnectionState.Open)
                    cnL.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnL;
                cmd.CommandText = "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'" + dbName + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER DATABASE [" + dbName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DROP DATABASE [" + dbName + "]";
                cmd.ExecuteNonQuery();
            }
        }

        private void bntDropDB_Click(object sender, EventArgs e)
        {
            if (cbDatabase.Text == "" || cbDatabase.Text == LOADING_STR || cbDatabase.Text == NO_DB)
            {
                MessageBox.Show("База данных не выбрана");
                return;
            }
            string db = cbDatabase.Text;
            if (this.cn != null)
            { try { this.cn.Close(); } catch { } }

            if (db == "")
            {
                MessageBox.Show("База данных для удаления не выбрана");
                return;
            }
            SqlCommand cmd = new SqlCommand();
            //cn = new SqlConnection(ConnectionString);
            //SqlCommand cmd = new SqlCommand("SELECT ADMIN FROM CompetitionData", cn);
            //try
            //{
            //    cn.Open();
            //    if (cmd.ExecuteScalar().ToString() != tbAdminPwd.Text)
            //    {
            //        MessageBox.Show("Введён неверный пароль администратора");
            //        cn.Close();
            //        return;
            //    }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    try { cn.Close(); }
            //    catch { }
            //    return;
            //}
            if (MessageBox.Show("Вы уверены, что хотите удалить Базу Данных \"" + db +
                "\"?\r\nВсе данные из этой БД будут удалены бех возможности восстановления!!!", "Удаление Базы Данных",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;
            try
            {
                DropDB(db);
                MessageBox.Show("База данных успешно удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nОшибка удаления БД");
                return;
            }
            cbDatabase.DroppedDown = false;
            cbDatabase.Text = "";
        }

        private bool UpdateCompetitonData()
        {
            if (alertAbout && (rbIntl.Checked != intlOld))
            {
                if (MessageBox.Show(this, "Вы уверены, что хотите изменить правила проведения сорвевнований?\r\n" +
                    "ВНИМАНИЕ!!! Результаты уже проведённых раундов могут оказаться недействительными!!!",
                    "Изменить правила проведения соревнований", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                    == DialogResult.No)
                {
                    rbIntl.Checked = intlOld;
                    rbNational.Checked = !rbIntl.Checked;
                    return false;
                }
            }
            SqlConnection cn;
            bool b = true;
            if (this.cn == null)
                cn = new SqlConnection(ConnectionString);
            else
            {
                b = false;
                cn = this.cn;
            }
            try { dbName = cbDatabase.SelectedItem.ToString(); }
            catch { dbName = cbDatabase.Text; }
            srvName = m_tbServer.Text;
            if (!rbIntl.Checked && !rbNational.Checked)
            {
                MessageBox.Show(this, "Правила проведения соревнований не выбраны.");
                return false;
            }
            MessageBox.Show("Пароль администратора: \"" + tbAdminPwd.Text + "\"", "Пароль",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            //if (MessageBox.Show("Пароль администратора: \"" + tbAdminPwd.Text + "\"\r\nВы согласны?", "Подтверждение пароля",
            //    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            //    return;
            try
            {
                if (cn.State != System.Data.ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cmd.Connection.BeginTransaction();
                try
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM CompetitionData";
                    object o = cmd.ExecuteScalar();
                    bool exists;
                    if (o == null || o.Equals(DBNull.Value))
                        exists = false;
                    else
                        try { exists = Convert.ToInt32(o) > 0; }
                        catch { exists = false; }
                    //cmd.CommandText = "DELETE FROM CompetitionData";
                    //cmd.ExecuteNonQuery();
                    if (exists)
                        cmd.CommandText = "UPDATE CompetitionData SET DATES=@dates, PLACE=@place, NAME=@name," +
                            "SHORT_NAME=@shortName,ADMIN=@admin";
                    else
                        cmd.CommandText = "INSERT INTO CompetitionData (DATES,PLACE,NAME,SHORT_NAME,ADMIN) " +
                            "VALUES(@dates,@place,@name,@shortName,@admin)";
                    cmd.Parameters.Add("@dates", SqlDbType.VarChar, 255).Value = m_tbDate.Text;
                    cmd.Parameters.Add("@place", SqlDbType.VarChar, 255).Value = m_tbPlace.Text;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 255).Value = compTitle.Text;
                    cmd.Parameters.Add("@shortName", SqlDbType.VarChar, 255).Value = textBox1.Text;
                    cmd.Parameters.Add("@admin", SqlDbType.VarChar, 255).Value = tbAdminPwd.Text;

                    cmd.ExecuteNonQuery();

                    SpeedRules rulesC = SortingClass.GetCompRules(cmd.Connection, cmd.Transaction);
                    if (rbIntl.Checked)
                        rulesC = rulesC | (SpeedRules.InternationalRules | SpeedRules.InternationalSchema);
                    else
                        rulesC = rulesC & (~(SpeedRules.InternationalRules | SpeedRules.InternationalSchema));
                    SortingClass.SetCompRules(rulesC, cmd.Connection, cmd.Transaction);

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw ex;
                }
                if (b)
                    cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления данных:\r\n" + ex.Message);
                if (cn.State == System.Data.ConnectionState.Open && b)
                    cn.Close();
                return false;
            }
            return true;
        }

        private void cbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            GetCompData();
        }

        private void GetCompData()
        {
            if (cbDatabase.SelectedIndex < 0 && cbDatabase.Text == "")
                return;
            nc = false;
            cn = new SqlConnection(ConnectionString);
            nc = true;
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM CompetitionData", cn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        m_tbDate.Text = rdr[2].ToString();
                        m_tbPlace.Text = rdr[3].ToString();
                        compTitle.Text = rdr[0].ToString();
                        textBox1.Text = rdr[1].ToString();
                    }
                }
                SpeedRules cRules = SortingClass.GetCompRules(cmd.Connection, cmd.Transaction);
                rbIntl.Checked = (cRules & SpeedRules.InternationalRules) == SpeedRules.InternationalRules;
                rbNational.Checked = !rbIntl.Checked;
                cn.Close();
            }
            catch
            {
                if (cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
                m_tbDate.Text = m_tbPlace.Text = compTitle.Text = textBox1.Text = "";
            }
        }

        string bakFileName;

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (cbDatabase.Text == "")
            {
                MessageBox.Show("БД не выбрана");
                return;
            }
            try
            {
                if (cn != null)
                    cn.Close();
            }
            catch { }
            if (CheckServerName())
                return;
            if (MessageBox.Show("Внимание! Данная операция может занять много времени. Продолжить?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Файлы резервной копии БД (*.bak)|*.bak";
            sfd.FilterIndex = 0;
            sfd.FileName = cbDatabase.Text + ".bak";
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;
            if (sfd.FileName == "")
                return;
            bakFileName = sfd.FileName;
            //BackupDB_Delegate();
            thrDbName = cbDatabase.Text;
            thrServName = m_tbServer.Text;
            cn = CreateConnection("master");
            //BackupDB_Delegate();
            TmeWorkForm tf = new TmeWorkForm("Создание резервной копии", BackupDB_Delegate, false);
            tf.ShowDialog();
        }

        string thrDbName, thrServName;

        private void BackupDB_Delegate()
        {
            
            string dbName = thrDbName;
            string srvName = thrServName;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandTimeout = 600;
            try
            {
                cn.Open();

                if (File.Exists(bakFileName))
                    File.Delete(bakFileName);

                //cmd.CommandText = "SELECT COUNT(*) FROM sysdevices WHERE name = '#cBakData'";
                //if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                //{
                //    cmd.CommandText = "EXEC sp_dropdevice '#cBakData'";
                //    cmd.ExecuteNonQuery();
                //}

                //cmd.CommandText = "EXEC sp_addumpdevice 'disk', '#cBakData', '" + bakFileName + "'";
                //cmd.ExecuteNonQuery();
                cmd.CommandText = "BACKUP DATABASE " + dbName + " TO DISK='" + bakFileName + "' WITH COMPRESSION";

                try { cmd.ExecuteNonQuery(); }
                catch
                {
                    cmd.CommandText = "BACKUP DATABASE " + dbName + " TO DISK='" + bakFileName + "'";
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Резервная копия создана успешно в файл\r\n" +
                    bakFileName);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка резервного копирования БД\r\n" + ex.Message); }
            //try
            //{
            //    cmd.CommandText = "EXEC sp_dropdevice '#cBakData'";

            //    cmd.ExecuteNonQuery();
            //}
            //catch { }

            try { cn.Close(); }
            catch { }
        }

        private bool CheckServerName()
        {
            if (m_tbServer.Text.ToLower().IndexOf("localhost") == 0 || m_tbServer.Text.ToLower().IndexOf("(local)") == 0)
                return false;
            try
            {
                if (m_tbServer.Text.ToUpper().IndexOf(System.Net.Dns.GetHostName().ToUpper()) < 0)
                    return (MessageBox.Show("Данный компьютер скорее всего не является сервером.\r\n" +
                        "Настоятельно рекомендуется проводить операции, связанные с копированием БД с СЕРВЕРА.\r\n" +
                        "Вы уверены, что хотите продолжить?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.No);
                else
                    return false;
            }
            catch { return false; }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (cbDatabase.Text == "")
            {
                MessageBox.Show("Введите название восстанавливаемой БД");
                return;
            }

            try
            {
                if (cn != null)
                    cn.Close();
            }
            catch { }

            if (CheckServerName())
                return;
            if (MessageBox.Show("Внимание! Данная операция может занять много времени. Продолжить?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы резервной копии БД (*.bak)|*.bak|Все файлы (*.*)|*.*";
            ofd.FilterIndex = 0;
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            if (ofd.FileName == "")
                return;
            if (!File.Exists(ofd.FileName))
            {
                MessageBox.Show("Файл не найден.");
                return;
            }
            bakFileName = ofd.FileName;
            thrDbName = cbDatabase.Text;
            cn = CreateConnection("master");
            TmeWorkForm tf = new TmeWorkForm("Восстановление БД", RestoreDB_Delegate, false);
            tf.ShowDialog();
        }

        private void RestoreDB_Delegate()
        {

            string dbName = thrDbName;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            try
            {

                cn.Open();
                string fName;
                cmd.CommandText = "SELECT COUNT(*) FROM sysdatabases(NOLOCK) WHERE name = '" + dbName + "'";
                cmd.CommandTimeout = 600;
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    if (MessageBox.Show("Такая БД существует. Заменить?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cmd.CommandText = "SELECT filename FROM sysdatabases(NOLOCK) WHERE name = '" + dbName + "'";
                        fName = cmd.ExecuteScalar().ToString();
                    }
                    else
                    {
                        try { cn.Close(); }
                        catch { }
                        return;
                    }
                }
                //else
                //{
                //sfd.ShowDialog();
                //if (sfd.FileName == "")
                //{
                //    try { cn.Close(); }
                //    catch { }
                //    return;
                //}
                //if (File.Exists(sfd.FileName))
                //    File.Delete(sfd.FileName);
                //fName = sfd.FileName;
                //}

                cmd.CommandText = "RESTORE DATABASE " + dbName +
                                  "   FROM DISK = '" + bakFileName + "'" +
                                  "   WITH REPLACE";
                try { cmd.ExecuteNonQuery(); }
                catch
                {
                    cmd.CommandText = "RESTORE FILELISTONLY FROM DISK = '" + bakFileName + "'";
                    List<string> files = new List<string>();
                    List<string> types = new List<string>();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    //string lfname = "";
                    try
                    {
                        while (rdr.Read())
                        {
                            files.Add(rdr["LogicalName"].ToString());
                            try
                            {
                                string fType = rdr["PhysicalName"].ToString();
                                int k = fType.LastIndexOf('.');
                                if (k > -1)
                                    types.Add("." + fType.Substring(k + 1).ToLower());
                                else
                                    types.Add("");
                            }
                            catch { types.Add(""); }
                        }
                    }
                    finally { rdr.Close(); }
                    MessageBox.Show("Сейчас будет создан основной файл Вашей восстанавливаемой БД.\r\n" +
                        "Пожалуйста, укажите место расположения данного файла.\r\n\r\n" +
                        "ВНИМАНИЕ!!! Не располагайте основной файл БД на сменных дисках!!!\r\n" +
                        "Для стабильной работы программы данный файл должен быть расположен\r\n" +
                        "на жёстком диске Вашего сервера т.к. доступ к нему производится ПОСТОЯННО!!!");
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Укажите файл для хранения данных";
                    sfd.Filter = "Файл БД (*.mdf)|*.mdf";
                    sfd.FilterIndex = 0;
                    sfd.FileName = thrDbName + ".mdf";

                    if (sfd.ShowDialog() == DialogResult.Cancel)
                    {
                        cn.Close();
                        return;
                    }
                    if (sfd.FileName == "")
                    {
                        cn.Close();
                        return;
                    }
                    string pthName;
                    int nn = sfd.FileName.LastIndexOf('\\');
                    if (nn > -1)
                        pthName = sfd.FileName.Substring(0, nn) + '\\';
                    else
                    {
                        cn.Close();
                        return;
                    }
                    //string logFile = sfd.FileName.Substring(0, sfd.FileName.Length - 4) + "_log.ldf";

                    cmd.CommandText = "RESTORE DATABASE " + dbName +
                                      "   FROM DISK = '" + bakFileName + "'" +
                                      "   WITH ";
                    for (nn = 0; nn < files.Count; nn++)
                    {
                        string cFile = pthName;
                        if (types[nn] == ".mdf" || types[nn] == ".ldf")
                            cFile += dbName + types[nn];
                        else
                            cFile += dbName + "_" + files[nn] + types[nn];

                        cmd.CommandText += " MOVE '" + files[nn] + "' TO '" + cFile + "', ";
                        //MessageBox.Show(cmd.CommandText);
                    }
                    cmd.CommandText += " REPLACE";
                    //"MOVE '" + lfname + "' TO '" + sfd.FileName + "'," +
                    //                  "        MOVE '" + lfname + "_log' TO '" + logFile + "'," +
                    //                  "        REPLACE";

                    cmd.ExecuteNonQuery();

                }

                try
                {
                    cmd.CommandText = "ALTER DATABASE [" + dbName + "] SET RECOVERY SIMPLE WITH NO_WAIT";
                    cmd.ExecuteNonQuery();
                }
                catch { }

                MessageBox.Show("База данных " + dbName + " успешно восстановлена");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка восстановления БД.\r\n" + ex.Message); }
        }

        public delegate void TimeConsumingWork();

        private void rbUseSQL_CheckedChanged(object sender, EventArgs e)
        {
            gb.Enabled = rbUseSQL.Checked;
        }
    }
}
