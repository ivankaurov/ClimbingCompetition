namespace ClimbingCompetition
{
    partial class SetConnectionString
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCreateDB = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.rbCString = new System.Windows.Forms.RadioButton();
            this.rbServer = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDataBase = new System.Windows.Forms.ComboBox();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.rbUseSQL = new System.Windows.Forms.RadioButton();
            this.rbUseWin = new System.Windows.Forms.RadioButton();
            this.btnChangeAdmPassword = new System.Windows.Forms.Button();
            this.rbUseService = new System.Windows.Forms.RadioButton();
            this.cbCompSelect = new System.Windows.Forms.ComboBox();
            this.tbAdminPasswordService = new System.Windows.Forms.TextBox();
            this.gbUseWebServices = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbRemoteDBServices = new System.Windows.Forms.GroupBox();
            this.rbMVC = new System.Windows.Forms.RadioButton();
            this.btnSetAuth = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gbUseWebServices.SuspendLayout();
            this.gbRemoteDBServices.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(14, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(95, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(530, 20);
            this.textBox1.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(6, 19);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(144, 23);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Проверить соедининие";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCreateDB
            // 
            this.btnCreateDB.Enabled = false;
            this.btnCreateDB.Location = new System.Drawing.Point(156, 19);
            this.btnCreateDB.Name = "btnCreateDB";
            this.btnCreateDB.Size = new System.Drawing.Size(83, 23);
            this.btnCreateDB.TabIndex = 0;
            this.btnCreateDB.Text = "Создать БД";
            this.btnCreateDB.UseVisualStyleBackColor = true;
            this.btnCreateDB.Click += new System.EventHandler(this.btnCreateDB_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "30 сек.",
            "1 мин.",
            "2 мин.",
            "3 мин.",
            "5 мин.",
            "10 мин."});
            this.comboBox1.Location = new System.Drawing.Point(14, 275);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(155, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.Text = "Частота обновления";
            // 
            // rbCString
            // 
            this.rbCString.AutoSize = true;
            this.rbCString.Checked = true;
            this.rbCString.Location = new System.Drawing.Point(14, 50);
            this.rbCString.Name = "rbCString";
            this.rbCString.Size = new System.Drawing.Size(161, 17);
            this.rbCString.TabIndex = 4;
            this.rbCString.TabStop = true;
            this.rbCString.Text = "Ввести строку соединения";
            this.rbCString.UseVisualStyleBackColor = true;
            this.rbCString.CheckedChanged += new System.EventHandler(this.rbCString_CheckedChanged);
            // 
            // rbServer
            // 
            this.rbServer.AutoSize = true;
            this.rbServer.Location = new System.Drawing.Point(14, 73);
            this.rbServer.Name = "rbServer";
            this.rbServer.Size = new System.Drawing.Size(156, 17);
            this.rbServer.TabIndex = 4;
            this.rbServer.Text = "Ввести данные о сервере";
            this.rbServer.UseVisualStyleBackColor = true;
            this.rbServer.Visible = false;
            this.rbServer.CheckedChanged += new System.EventHandler(this.rbCString_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbDataBase);
            this.groupBox1.Controls.Add(this.tbServer);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.tbUser);
            this.groupBox1.Controls.Add(this.rbUseSQL);
            this.groupBox1.Controls.Add(this.rbUseWin);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(182, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 168);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сервер";
            this.groupBox1.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "База данных:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Сервер:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Имя пользователя:";
            // 
            // cbDataBase
            // 
            this.cbDataBase.FormattingEnabled = true;
            this.cbDataBase.Location = new System.Drawing.Point(116, 144);
            this.cbDataBase.Name = "cbDataBase";
            this.cbDataBase.Size = new System.Drawing.Size(203, 21);
            this.cbDataBase.TabIndex = 2;
            this.cbDataBase.DropDown += new System.EventHandler(this.cbDataBase_DropDown);
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(116, 122);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(203, 20);
            this.tbServer.TabIndex = 1;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(116, 96);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(203, 20);
            this.tbPassword.TabIndex = 1;
            // 
            // tbUser
            // 
            this.tbUser.Location = new System.Drawing.Point(116, 70);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(203, 20);
            this.tbUser.TabIndex = 1;
            // 
            // rbUseSQL
            // 
            this.rbUseSQL.AutoSize = true;
            this.rbUseSQL.Checked = true;
            this.rbUseSQL.Location = new System.Drawing.Point(7, 43);
            this.rbUseSQL.Name = "rbUseSQL";
            this.rbUseSQL.Size = new System.Drawing.Size(238, 17);
            this.rbUseSQL.TabIndex = 0;
            this.rbUseSQL.TabStop = true;
            this.rbUseSQL.Text = "Использовать учётную запись SQL Server";
            this.rbUseSQL.UseVisualStyleBackColor = true;
            // 
            // rbUseWin
            // 
            this.rbUseWin.AutoSize = true;
            this.rbUseWin.Location = new System.Drawing.Point(7, 20);
            this.rbUseWin.Name = "rbUseWin";
            this.rbUseWin.Size = new System.Drawing.Size(267, 17);
            this.rbUseWin.TabIndex = 0;
            this.rbUseWin.Text = "Использовать данные учётной записи Windows";
            this.rbUseWin.UseVisualStyleBackColor = true;
            this.rbUseWin.CheckedChanged += new System.EventHandler(this.rbUseWin_CheckedChanged);
            // 
            // btnChangeAdmPassword
            // 
            this.btnChangeAdmPassword.Enabled = false;
            this.btnChangeAdmPassword.Location = new System.Drawing.Point(245, 19);
            this.btnChangeAdmPassword.Name = "btnChangeAdmPassword";
            this.btnChangeAdmPassword.Size = new System.Drawing.Size(100, 40);
            this.btnChangeAdmPassword.TabIndex = 6;
            this.btnChangeAdmPassword.Text = "Сменить пароль администратора";
            this.btnChangeAdmPassword.UseVisualStyleBackColor = true;
            this.btnChangeAdmPassword.Click += new System.EventHandler(this.btnChangeAdmPassword_Click);
            // 
            // rbUseService
            // 
            this.rbUseService.AutoSize = true;
            this.rbUseService.Location = new System.Drawing.Point(13, 97);
            this.rbUseService.Name = "rbUseService";
            this.rbUseService.Size = new System.Drawing.Size(166, 17);
            this.rbUseService.TabIndex = 7;
            this.rbUseService.TabStop = true;
            this.rbUseService.Text = "Использовать Web службы";
            this.rbUseService.UseVisualStyleBackColor = true;
            this.rbUseService.Visible = false;
            this.rbUseService.CheckedChanged += new System.EventHandler(this.rbCString_CheckedChanged);
            // 
            // cbCompSelect
            // 
            this.cbCompSelect.FormattingEnabled = true;
            this.cbCompSelect.Location = new System.Drawing.Point(6, 19);
            this.cbCompSelect.Name = "cbCompSelect";
            this.cbCompSelect.Size = new System.Drawing.Size(151, 21);
            this.cbCompSelect.TabIndex = 8;
            this.cbCompSelect.DropDown += new System.EventHandler(this.cbCompSelect_DropDown);
            // 
            // tbAdminPasswordService
            // 
            this.tbAdminPasswordService.Location = new System.Drawing.Point(6, 72);
            this.tbAdminPasswordService.Name = "tbAdminPasswordService";
            this.tbAdminPasswordService.PasswordChar = '*';
            this.tbAdminPasswordService.Size = new System.Drawing.Size(152, 20);
            this.tbAdminPasswordService.TabIndex = 9;
            // 
            // gbUseWebServices
            // 
            this.gbUseWebServices.Controls.Add(this.label5);
            this.gbUseWebServices.Controls.Add(this.tbAdminPasswordService);
            this.gbUseWebServices.Controls.Add(this.cbCompSelect);
            this.gbUseWebServices.Location = new System.Drawing.Point(12, 141);
            this.gbUseWebServices.Name = "gbUseWebServices";
            this.gbUseWebServices.Size = new System.Drawing.Size(163, 99);
            this.gbUseWebServices.TabIndex = 10;
            this.gbUseWebServices.TabStop = false;
            this.gbUseWebServices.Text = "Службы";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Пароль администратора:";
            // 
            // gbRemoteDBServices
            // 
            this.gbRemoteDBServices.Controls.Add(this.btnTest);
            this.gbRemoteDBServices.Controls.Add(this.btnCreateDB);
            this.gbRemoteDBServices.Controls.Add(this.btnChangeAdmPassword);
            this.gbRemoteDBServices.Location = new System.Drawing.Point(182, 228);
            this.gbRemoteDBServices.Name = "gbRemoteDBServices";
            this.gbRemoteDBServices.Size = new System.Drawing.Size(349, 68);
            this.gbRemoteDBServices.TabIndex = 11;
            this.gbRemoteDBServices.TabStop = false;
            this.gbRemoteDBServices.Text = "Управление удалённой БД";
            this.gbRemoteDBServices.Visible = false;
            // 
            // rbMVC
            // 
            this.rbMVC.AutoSize = true;
            this.rbMVC.Location = new System.Drawing.Point(13, 118);
            this.rbMVC.Name = "rbMVC";
            this.rbMVC.Size = new System.Drawing.Size(125, 17);
            this.rbMVC.TabIndex = 7;
            this.rbMVC.TabStop = true;
            this.rbMVC.Text = "Новые WebСлужбы";
            this.rbMVC.UseVisualStyleBackColor = true;
            this.rbMVC.CheckedChanged += new System.EventHandler(this.rbCString_CheckedChanged);
            // 
            // btnSetAuth
            // 
            this.btnSetAuth.Location = new System.Drawing.Point(14, 303);
            this.btnSetAuth.Name = "btnSetAuth";
            this.btnSetAuth.Size = new System.Drawing.Size(155, 37);
            this.btnSetAuth.TabIndex = 12;
            this.btnSetAuth.Text = "Настройки аутентификации";
            this.btnSetAuth.UseVisualStyleBackColor = true;
            this.btnSetAuth.Click += new System.EventHandler(this.btnSetAuth_Click);
            // 
            // SetConnectionString
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 341);
            this.Controls.Add(this.btnSetAuth);
            this.Controls.Add(this.gbRemoteDBServices);
            this.Controls.Add(this.gbUseWebServices);
            this.Controls.Add(this.rbMVC);
            this.Controls.Add(this.rbUseService);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rbServer);
            this.Controls.Add(this.rbCString);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "SetConnectionString";
            this.Text = "Установка соединения с удалённым сервером";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbUseWebServices.ResumeLayout(false);
            this.gbUseWebServices.PerformLayout();
            this.gbRemoteDBServices.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCreateDB;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RadioButton rbCString;
        private System.Windows.Forms.RadioButton rbServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDataBase;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.RadioButton rbUseSQL;
        private System.Windows.Forms.RadioButton rbUseWin;
        private System.Windows.Forms.Button btnChangeAdmPassword;
        private System.Windows.Forms.RadioButton rbUseService;
        private System.Windows.Forms.ComboBox cbCompSelect;
        private System.Windows.Forms.TextBox tbAdminPasswordService;
        private System.Windows.Forms.GroupBox gbUseWebServices;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gbRemoteDBServices;
        private System.Windows.Forms.RadioButton rbMVC;
        private System.Windows.Forms.Button btnSetAuth;
    }
}