namespace ClimbingCompetition
{
    partial class AddClimber
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
            this.dg = new System.Windows.Forms.DataGridView();
            this.bPartLateAppl = new System.Windows.Forms.CheckBox();
            this.bPartNoPoints = new System.Windows.Forms.CheckBox();
            this.bPartVk = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbPartTeam = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tbPartGroup = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPartNumber = new System.Windows.Forms.Label();
            this.cbPartSex = new System.Windows.Forms.ComboBox();
            this.cbPartQf = new System.Windows.Forms.ComboBox();
            this.tbPartBoulder = new System.Windows.Forms.TextBox();
            this.tbPartSpeed = new System.Windows.Forms.TextBox();
            this.tbPartLead = new System.Windows.Forms.TextBox();
            this.tbPartAge = new System.Windows.Forms.TextBox();
            this.tbPartName = new System.Windows.Forms.TextBox();
            this.tbPartSurname = new System.Windows.Forms.TextBox();
            this.cbPartTeams = new System.Windows.Forms.ComboBox();
            this.cbPartGroups = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.SuspendLayout();
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Right;
            this.dg.Location = new System.Drawing.Point(322, 0);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.Size = new System.Drawing.Size(416, 491);
            this.dg.TabIndex = 51;
            this.dg.SelectionChanged += new System.EventHandler(this.dg_SelectionChanged);
            // 
            // bPartLateAppl
            // 
            this.bPartLateAppl.AutoSize = true;
            this.bPartLateAppl.Enabled = false;
            this.bPartLateAppl.Location = new System.Drawing.Point(209, 225);
            this.bPartLateAppl.Name = "bPartLateAppl";
            this.bPartLateAppl.Size = new System.Drawing.Size(107, 17);
            this.bPartLateAppl.TabIndex = 48;
            this.bPartLateAppl.Text = "поздняя заявка";
            this.bPartLateAppl.UseVisualStyleBackColor = true;
            // 
            // bPartNoPoints
            // 
            this.bPartNoPoints.AutoSize = true;
            this.bPartNoPoints.Enabled = false;
            this.bPartNoPoints.Location = new System.Drawing.Point(209, 202);
            this.bPartNoPoints.Name = "bPartNoPoints";
            this.bPartNoPoints.Size = new System.Drawing.Size(83, 17);
            this.bPartNoPoints.TabIndex = 49;
            this.bPartNoPoints.Text = "без баллов";
            this.bPartNoPoints.UseVisualStyleBackColor = true;
            // 
            // bPartVk
            // 
            this.bPartVk.AutoSize = true;
            this.bPartVk.Enabled = false;
            this.bPartVk.Location = new System.Drawing.Point(209, 183);
            this.bPartVk.Name = "bPartVk";
            this.bPartVk.Size = new System.Drawing.Size(94, 17);
            this.bPartVk.TabIndex = 50;
            this.bPartVk.Text = "вне конкурса";
            this.bPartVk.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 299);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(127, 13);
            this.label19.TabIndex = 40;
            this.label19.Text = "Рейтинг в боулдеринге:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(28, 276);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(110, 13);
            this.label18.TabIndex = 41;
            this.label18.Text = "Рейтинг в скорости:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(24, 253);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(114, 13);
            this.label17.TabIndex = 39;
            this.label17.Text = "Рейтинг в трудности:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(175, 157);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 13);
            this.label10.TabIndex = 37;
            this.label10.Text = "Г.р.:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 183);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 13);
            this.label12.TabIndex = 38;
            this.label12.Text = "Разряд:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(36, 157);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(30, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "Пол:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 131);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Имя:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 45;
            this.label7.Text = "Фамилия:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 229);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 44;
            this.label16.Text = "Команда:";
            // 
            // tbPartTeam
            // 
            this.tbPartTeam.AutoSize = true;
            this.tbPartTeam.Location = new System.Drawing.Point(71, 229);
            this.tbPartTeam.Name = "tbPartTeam";
            this.tbPartTeam.Size = new System.Drawing.Size(0, 13);
            this.tbPartTeam.TabIndex = 34;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 206);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 43;
            this.label14.Text = "Группа:";
            // 
            // tbPartGroup
            // 
            this.tbPartGroup.AutoSize = true;
            this.tbPartGroup.Location = new System.Drawing.Point(71, 206);
            this.tbPartGroup.Name = "tbPartGroup";
            this.tbPartGroup.Size = new System.Drawing.Size(0, 13);
            this.tbPartGroup.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Номер:";
            // 
            // tbPartNumber
            // 
            this.tbPartNumber.AutoSize = true;
            this.tbPartNumber.Location = new System.Drawing.Point(71, 86);
            this.tbPartNumber.Name = "tbPartNumber";
            this.tbPartNumber.Size = new System.Drawing.Size(0, 13);
            this.tbPartNumber.TabIndex = 36;
            // 
            // cbPartSex
            // 
            this.cbPartSex.Enabled = false;
            this.cbPartSex.FormattingEnabled = true;
            this.cbPartSex.Items.AddRange(new object[] {
            "М",
            "Ж"});
            this.cbPartSex.Location = new System.Drawing.Point(85, 154);
            this.cbPartSex.Name = "cbPartSex";
            this.cbPartSex.Size = new System.Drawing.Size(67, 21);
            this.cbPartSex.TabIndex = 32;
            this.cbPartSex.Text = "Пол";
            // 
            // cbPartQf
            // 
            this.cbPartQf.Enabled = false;
            this.cbPartQf.FormattingEnabled = true;
            this.cbPartQf.Items.AddRange(new object[] {
            "МСМК",
            "МС",
            "КМС",
            "1",
            "2",
            "3",
            "1ю",
            "2ю",
            "3ю",
            "б/р"});
            this.cbPartQf.Location = new System.Drawing.Point(85, 180);
            this.cbPartQf.Name = "cbPartQf";
            this.cbPartQf.Size = new System.Drawing.Size(67, 21);
            this.cbPartQf.TabIndex = 33;
            this.cbPartQf.Text = "Разряд";
            // 
            // tbPartBoulder
            // 
            this.tbPartBoulder.Location = new System.Drawing.Point(155, 292);
            this.tbPartBoulder.Name = "tbPartBoulder";
            this.tbPartBoulder.ReadOnly = true;
            this.tbPartBoulder.Size = new System.Drawing.Size(47, 20);
            this.tbPartBoulder.TabIndex = 28;
            // 
            // tbPartSpeed
            // 
            this.tbPartSpeed.Location = new System.Drawing.Point(155, 273);
            this.tbPartSpeed.Name = "tbPartSpeed";
            this.tbPartSpeed.ReadOnly = true;
            this.tbPartSpeed.Size = new System.Drawing.Size(47, 20);
            this.tbPartSpeed.TabIndex = 27;
            // 
            // tbPartLead
            // 
            this.tbPartLead.Location = new System.Drawing.Point(155, 253);
            this.tbPartLead.Name = "tbPartLead";
            this.tbPartLead.ReadOnly = true;
            this.tbPartLead.Size = new System.Drawing.Size(47, 20);
            this.tbPartLead.TabIndex = 26;
            // 
            // tbPartAge
            // 
            this.tbPartAge.Location = new System.Drawing.Point(220, 154);
            this.tbPartAge.Name = "tbPartAge";
            this.tbPartAge.ReadOnly = true;
            this.tbPartAge.Size = new System.Drawing.Size(47, 20);
            this.tbPartAge.TabIndex = 31;
            // 
            // tbPartName
            // 
            this.tbPartName.Location = new System.Drawing.Point(85, 128);
            this.tbPartName.Name = "tbPartName";
            this.tbPartName.ReadOnly = true;
            this.tbPartName.Size = new System.Drawing.Size(181, 20);
            this.tbPartName.TabIndex = 30;
            // 
            // tbPartSurname
            // 
            this.tbPartSurname.Location = new System.Drawing.Point(85, 102);
            this.tbPartSurname.Name = "tbPartSurname";
            this.tbPartSurname.ReadOnly = true;
            this.tbPartSurname.Size = new System.Drawing.Size(181, 20);
            this.tbPartSurname.TabIndex = 29;
            // 
            // cbPartTeams
            // 
            this.cbPartTeams.FormattingEnabled = true;
            this.cbPartTeams.Location = new System.Drawing.Point(86, 44);
            this.cbPartTeams.Name = "cbPartTeams";
            this.cbPartTeams.Size = new System.Drawing.Size(181, 21);
            this.cbPartTeams.TabIndex = 25;
            this.cbPartTeams.Text = "Выберите команду";
            this.cbPartTeams.SelectedIndexChanged += new System.EventHandler(this.cbPartTeams_SelectedIndexChanged);
            // 
            // cbPartGroups
            // 
            this.cbPartGroups.FormattingEnabled = true;
            this.cbPartGroups.Location = new System.Drawing.Point(86, 17);
            this.cbPartGroups.Name = "cbPartGroups";
            this.cbPartGroups.Size = new System.Drawing.Size(181, 21);
            this.cbPartGroups.TabIndex = 24;
            this.cbPartGroups.Text = "Выберите группу";
            this.cbPartGroups.SelectedIndexChanged += new System.EventHandler(this.cbPartGroups_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(9, 345);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(99, 32);
            this.btnAdd.TabIndex = 52;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(114, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 32);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AddClimber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 491);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dg);
            this.Controls.Add(this.bPartLateAppl);
            this.Controls.Add(this.bPartNoPoints);
            this.Controls.Add(this.bPartVk);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.tbPartTeam);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbPartGroup);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbPartNumber);
            this.Controls.Add(this.cbPartSex);
            this.Controls.Add(this.cbPartQf);
            this.Controls.Add(this.tbPartBoulder);
            this.Controls.Add(this.tbPartSpeed);
            this.Controls.Add(this.tbPartLead);
            this.Controls.Add(this.tbPartAge);
            this.Controls.Add(this.tbPartName);
            this.Controls.Add(this.tbPartSurname);
            this.Controls.Add(this.cbPartTeams);
            this.Controls.Add(this.cbPartGroups);
            this.Name = "AddClimber";
            this.Text = "Добавление участника";
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.CheckBox bPartLateAppl;
        private System.Windows.Forms.CheckBox bPartNoPoints;
        private System.Windows.Forms.CheckBox bPartVk;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label tbPartTeam;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label tbPartGroup;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label tbPartNumber;
        private System.Windows.Forms.ComboBox cbPartSex;
        private System.Windows.Forms.ComboBox cbPartQf;
        private System.Windows.Forms.TextBox tbPartBoulder;
        private System.Windows.Forms.TextBox tbPartSpeed;
        private System.Windows.Forms.TextBox tbPartLead;
        private System.Windows.Forms.TextBox tbPartAge;
        private System.Windows.Forms.TextBox tbPartName;
        private System.Windows.Forms.TextBox tbPartSurname;
        private System.Windows.Forms.ComboBox cbPartTeams;
        private System.Windows.Forms.ComboBox cbPartGroups;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;

    }
}