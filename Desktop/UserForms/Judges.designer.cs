namespace ClimbingCompetition
{
    partial class Judges
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cbPos = new System.Windows.Forms.ComboBox();
            this.tbPos = new System.Windows.Forms.TextBox();
            this.tbSurname = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbPatronimic = new System.Windows.Forms.TextBox();
            this.tbCity = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEditSubmit = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dg = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnPosListEdit = new System.Windows.Forms.Button();
            this.btnLoadFRinet = new System.Windows.Forms.Button();
            this.btnLoadToInet = new System.Windows.Forms.Button();
            this.btnDelPhoto = new System.Windows.Forms.Button();
            this.btnLoadPhoto = new System.Windows.Forms.Button();
            this.pbPhoto = new System.Windows.Forms.PictureBox();
            this.rbGroupPos = new System.Windows.Forms.RadioButton();
            this.rbGroupJudge = new System.Windows.Forms.RadioButton();
            this.btnPrintOneB = new System.Windows.Forms.Button();
            this.btnPrintBadges = new System.Windows.Forms.Button();
            this.btnExcelExport = new System.Windows.Forms.Button();
            this.lblPosID = new System.Windows.Forms.Label();
            this.btnDelPos = new System.Windows.Forms.Button();
            this.btnEditPos = new System.Windows.Forms.Button();
            this.btnAddPos = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbCat = new System.Windows.Forms.TextBox();
            this.lblIID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dgPos = new System.Windows.Forms.DataGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPos)).BeginInit();
            this.SuspendLayout();
            // 
            // cbPos
            // 
            this.cbPos.Enabled = false;
            this.cbPos.FormattingEnabled = true;
            this.cbPos.Items.AddRange(new object[] {
            "Главный судья",
            "Главный секретарь",
            "Зам. гл.судьи по виду",
            "Зам. гл.судьи по трассам",
            "Зам. гл.судьи по безопасности",
            "Врач соревнований",
            "Постановщик трасс",
            "Секретарь",
            "Судья на трассе",
            "Судья при участниках",
            "Страховщик",
            "Председатель мандатной комиссии",
            "Представитель проводящей организации",
            "Представитель ФСР",
            "Другое:"});
            this.cbPos.Location = new System.Drawing.Point(18, 15);
            this.cbPos.Name = "cbPos";
            this.cbPos.Size = new System.Drawing.Size(302, 21);
            this.cbPos.TabIndex = 0;
            this.cbPos.Text = "Должность";
            this.cbPos.DropDown += new System.EventHandler(this.cbPos_DropDown);
            this.cbPos.SelectedIndexChanged += new System.EventHandler(this.cbPos_SelectedIndexChanged);
            // 
            // tbPos
            // 
            this.tbPos.Enabled = false;
            this.tbPos.Location = new System.Drawing.Point(326, 14);
            this.tbPos.Name = "tbPos";
            this.tbPos.Size = new System.Drawing.Size(262, 20);
            this.tbPos.TabIndex = 1;
            // 
            // tbSurname
            // 
            this.tbSurname.Location = new System.Drawing.Point(18, 43);
            this.tbSurname.Name = "tbSurname";
            this.tbSurname.Size = new System.Drawing.Size(182, 20);
            this.tbSurname.TabIndex = 2;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(17, 69);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(182, 20);
            this.tbName.TabIndex = 3;
            // 
            // tbPatronimic
            // 
            this.tbPatronimic.Location = new System.Drawing.Point(17, 95);
            this.tbPatronimic.Name = "tbPatronimic";
            this.tbPatronimic.Size = new System.Drawing.Size(182, 20);
            this.tbPatronimic.TabIndex = 4;
            // 
            // tbCity
            // 
            this.tbCity.Location = new System.Drawing.Point(269, 49);
            this.tbCity.Name = "tbCity";
            this.tbCity.Size = new System.Drawing.Size(181, 20);
            this.tbCity.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Фамилия";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(207, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Имя";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Отчество";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(458, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Город";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(17, 149);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEditSubmit
            // 
            this.btnEditSubmit.Location = new System.Drawing.Point(98, 149);
            this.btnEditSubmit.Name = "btnEditSubmit";
            this.btnEditSubmit.Size = new System.Drawing.Size(94, 23);
            this.btnEditSubmit.TabIndex = 8;
            this.btnEditSubmit.Text = "Правка";
            this.btnEditSubmit.UseVisualStyleBackColor = true;
            this.btnEditSubmit.Click += new System.EventHandler(this.btnEditSubmit_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(198, 149);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(93, 23);
            this.btnDel.TabIndex = 9;
            this.btnDel.Text = "Удалить";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dg);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(920, 613);
            this.splitContainer1.SplitterDistance = 359;
            this.splitContainer1.TabIndex = 6;
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            this.dg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.Size = new System.Drawing.Size(920, 359);
            this.dg.TabIndex = 10;
            this.dg.SelectionChanged += new System.EventHandler(this.dg_SelectionChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnPosListEdit);
            this.splitContainer2.Panel1.Controls.Add(this.btnLoadFRinet);
            this.splitContainer2.Panel1.Controls.Add(this.btnLoadToInet);
            this.splitContainer2.Panel1.Controls.Add(this.btnDelPhoto);
            this.splitContainer2.Panel1.Controls.Add(this.btnLoadPhoto);
            this.splitContainer2.Panel1.Controls.Add(this.pbPhoto);
            this.splitContainer2.Panel1.Controls.Add(this.rbGroupPos);
            this.splitContainer2.Panel1.Controls.Add(this.rbGroupJudge);
            this.splitContainer2.Panel1.Controls.Add(this.btnPrintOneB);
            this.splitContainer2.Panel1.Controls.Add(this.btnPrintBadges);
            this.splitContainer2.Panel1.Controls.Add(this.btnExcelExport);
            this.splitContainer2.Panel1.Controls.Add(this.lblPosID);
            this.splitContainer2.Panel1.Controls.Add(this.btnDelPos);
            this.splitContainer2.Panel1.Controls.Add(this.btnEditPos);
            this.splitContainer2.Panel1.Controls.Add(this.btnAddPos);
            this.splitContainer2.Panel1.Controls.Add(this.cbPos);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.tbCat);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.lblIID);
            this.splitContainer2.Panel1.Controls.Add(this.tbCity);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.tbPatronimic);
            this.splitContainer2.Panel1.Controls.Add(this.btnDel);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.tbPos);
            this.splitContainer2.Panel1.Controls.Add(this.tbName);
            this.splitContainer2.Panel1.Controls.Add(this.btnEditSubmit);
            this.splitContainer2.Panel1.Controls.Add(this.btnAdd);
            this.splitContainer2.Panel1.Controls.Add(this.tbSurname);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgPos);
            this.splitContainer2.Size = new System.Drawing.Size(920, 250);
            this.splitContainer2.SplitterDistance = 602;
            this.splitContainer2.TabIndex = 12;
            // 
            // btnPosListEdit
            // 
            this.btnPosListEdit.Location = new System.Drawing.Point(98, 224);
            this.btnPosListEdit.Name = "btnPosListEdit";
            this.btnPosListEdit.Size = new System.Drawing.Size(352, 23);
            this.btnPosListEdit.TabIndex = 19;
            this.btnPosListEdit.Text = "Настройка списка должностей для печати бейджиков";
            this.btnPosListEdit.UseVisualStyleBackColor = true;
            this.btnPosListEdit.Click += new System.EventHandler(this.btnPosListEdit_Click);
            // 
            // btnLoadFRinet
            // 
            this.btnLoadFRinet.Enabled = false;
            this.btnLoadFRinet.Location = new System.Drawing.Point(457, 95);
            this.btnLoadFRinet.Name = "btnLoadFRinet";
            this.btnLoadFRinet.Size = new System.Drawing.Size(118, 40);
            this.btnLoadFRinet.TabIndex = 18;
            this.btnLoadFRinet.Text = "Загрузить список судей ИЗ интернета";
            this.btnLoadFRinet.UseVisualStyleBackColor = true;
            this.btnLoadFRinet.Visible = false;
            this.btnLoadFRinet.Click += new System.EventHandler(this.btnLoadFRinet_Click);
            // 
            // btnLoadToInet
            // 
            this.btnLoadToInet.Enabled = false;
            this.btnLoadToInet.Location = new System.Drawing.Point(457, 196);
            this.btnLoadToInet.Name = "btnLoadToInet";
            this.btnLoadToInet.Size = new System.Drawing.Size(142, 41);
            this.btnLoadToInet.TabIndex = 17;
            this.btnLoadToInet.Text = "Загрузить список судей в интернет";
            this.btnLoadToInet.UseVisualStyleBackColor = true;
            this.btnLoadToInet.Visible = false;
            this.btnLoadToInet.Click += new System.EventHandler(this.btnLoadToInet_Click);
            // 
            // btnDelPhoto
            // 
            this.btnDelPhoto.Location = new System.Drawing.Point(342, 120);
            this.btnDelPhoto.Name = "btnDelPhoto";
            this.btnDelPhoto.Size = new System.Drawing.Size(108, 23);
            this.btnDelPhoto.TabIndex = 16;
            this.btnDelPhoto.Text = "Удалить фото";
            this.btnDelPhoto.UseVisualStyleBackColor = true;
            this.btnDelPhoto.Click += new System.EventHandler(this.btnDelPhoto_Click);
            // 
            // btnLoadPhoto
            // 
            this.btnLoadPhoto.Location = new System.Drawing.Point(342, 95);
            this.btnLoadPhoto.Name = "btnLoadPhoto";
            this.btnLoadPhoto.Size = new System.Drawing.Size(108, 23);
            this.btnLoadPhoto.TabIndex = 16;
            this.btnLoadPhoto.Text = "Загрузить фото";
            this.btnLoadPhoto.UseVisualStyleBackColor = true;
            this.btnLoadPhoto.Click += new System.EventHandler(this.btnLoadPhoto_Click);
            // 
            // pbPhoto
            // 
            this.pbPhoto.Location = new System.Drawing.Point(269, 98);
            this.pbPhoto.Name = "pbPhoto";
            this.pbPhoto.Size = new System.Drawing.Size(67, 45);
            this.pbPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPhoto.TabIndex = 15;
            this.pbPhoto.TabStop = false;
            // 
            // rbGroupPos
            // 
            this.rbGroupPos.AutoSize = true;
            this.rbGroupPos.Checked = true;
            this.rbGroupPos.Location = new System.Drawing.Point(430, 172);
            this.rbGroupPos.Name = "rbGroupPos";
            this.rbGroupPos.Size = new System.Drawing.Size(171, 17);
            this.rbGroupPos.TabIndex = 14;
            this.rbGroupPos.TabStop = true;
            this.rbGroupPos.Text = "Группировка по должностям";
            this.rbGroupPos.UseVisualStyleBackColor = true;
            // 
            // rbGroupJudge
            // 
            this.rbGroupJudge.AutoSize = true;
            this.rbGroupJudge.Location = new System.Drawing.Point(430, 149);
            this.rbGroupJudge.Name = "rbGroupJudge";
            this.rbGroupJudge.Size = new System.Drawing.Size(145, 17);
            this.rbGroupJudge.TabIndex = 14;
            this.rbGroupJudge.TabStop = true;
            this.rbGroupJudge.Text = "Группировка по судьям";
            this.rbGroupJudge.UseVisualStyleBackColor = true;
            // 
            // btnPrintOneB
            // 
            this.btnPrintOneB.Location = new System.Drawing.Point(297, 201);
            this.btnPrintOneB.Name = "btnPrintOneB";
            this.btnPrintOneB.Size = new System.Drawing.Size(153, 23);
            this.btnPrintOneB.TabIndex = 13;
            this.btnPrintOneB.Text = "Печать одного бейджика";
            this.btnPrintOneB.UseVisualStyleBackColor = true;
            this.btnPrintOneB.Click += new System.EventHandler(this.btnPrintBadges_Click);
            // 
            // btnPrintBadges
            // 
            this.btnPrintBadges.Location = new System.Drawing.Point(297, 178);
            this.btnPrintBadges.Name = "btnPrintBadges";
            this.btnPrintBadges.Size = new System.Drawing.Size(127, 23);
            this.btnPrintBadges.TabIndex = 13;
            this.btnPrintBadges.Text = "Печать бейджиков";
            this.btnPrintBadges.UseVisualStyleBackColor = true;
            this.btnPrintBadges.Click += new System.EventHandler(this.btnPrintBadges_Click);
            // 
            // btnExcelExport
            // 
            this.btnExcelExport.Location = new System.Drawing.Point(297, 149);
            this.btnExcelExport.Name = "btnExcelExport";
            this.btnExcelExport.Size = new System.Drawing.Size(127, 23);
            this.btnExcelExport.TabIndex = 12;
            this.btnExcelExport.Text = "Вывод списка в Excel";
            this.btnExcelExport.UseVisualStyleBackColor = true;
            this.btnExcelExport.Click += new System.EventHandler(this.btnExcelExport_Click);
            // 
            // lblPosID
            // 
            this.lblPosID.AutoSize = true;
            this.lblPosID.Location = new System.Drawing.Point(326, 211);
            this.lblPosID.Name = "lblPosID";
            this.lblPosID.Size = new System.Drawing.Size(0, 13);
            this.lblPosID.TabIndex = 11;
            this.lblPosID.Visible = false;
            // 
            // btnDelPos
            // 
            this.btnDelPos.Location = new System.Drawing.Point(198, 178);
            this.btnDelPos.Name = "btnDelPos";
            this.btnDelPos.Size = new System.Drawing.Size(94, 47);
            this.btnDelPos.TabIndex = 10;
            this.btnDelPos.Text = "Удалить должность";
            this.btnDelPos.UseVisualStyleBackColor = true;
            this.btnDelPos.Click += new System.EventHandler(this.btnDelPos_Click);
            // 
            // btnEditPos
            // 
            this.btnEditPos.Location = new System.Drawing.Point(98, 178);
            this.btnEditPos.Name = "btnEditPos";
            this.btnEditPos.Size = new System.Drawing.Size(94, 47);
            this.btnEditPos.TabIndex = 10;
            this.btnEditPos.Text = "Правка должности";
            this.btnEditPos.UseVisualStyleBackColor = true;
            this.btnEditPos.Click += new System.EventHandler(this.btnEditPos_Click);
            // 
            // btnAddPos
            // 
            this.btnAddPos.Location = new System.Drawing.Point(17, 178);
            this.btnAddPos.Name = "btnAddPos";
            this.btnAddPos.Size = new System.Drawing.Size(75, 47);
            this.btnAddPos.TabIndex = 10;
            this.btnAddPos.Text = "Добавить должность";
            this.btnAddPos.UseVisualStyleBackColor = true;
            this.btnAddPos.Click += new System.EventHandler(this.btnAddPos_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(458, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Категория";
            // 
            // tbCat
            // 
            this.tbCat.Location = new System.Drawing.Point(269, 75);
            this.tbCat.Name = "tbCat";
            this.tbCat.Size = new System.Drawing.Size(181, 20);
            this.tbCat.TabIndex = 6;
            // 
            // lblIID
            // 
            this.lblIID.AutoSize = true;
            this.lblIID.Location = new System.Drawing.Point(53, 118);
            this.lblIID.Name = "lblIID";
            this.lblIID.Size = new System.Drawing.Size(0, 13);
            this.lblIID.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "iid:";
            // 
            // dgPos
            // 
            this.dgPos.AllowUserToAddRows = false;
            this.dgPos.AllowUserToDeleteRows = false;
            this.dgPos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPos.Location = new System.Drawing.Point(0, 0);
            this.dgPos.Name = "dgPos";
            this.dgPos.ReadOnly = true;
            this.dgPos.Size = new System.Drawing.Size(314, 250);
            this.dgPos.TabIndex = 11;
            this.dgPos.SelectionChanged += new System.EventHandler(this.dgPos_SelectionChanged);
            // 
            // Judges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(920, 613);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Judges";
            this.Text = "Судьи";
            this.Load += new System.EventHandler(this.Judges_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPos;
        private System.Windows.Forms.TextBox tbPos;
        private System.Windows.Forms.TextBox tbSurname;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbPatronimic;
        private System.Windows.Forms.TextBox tbCity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEditSubmit;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Label lblIID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbCat;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgPos;
        private System.Windows.Forms.Button btnAddPos;
        private System.Windows.Forms.Button btnDelPos;
        private System.Windows.Forms.Button btnEditPos;
        private System.Windows.Forms.Label lblPosID;
        private System.Windows.Forms.Button btnExcelExport;
        private System.Windows.Forms.Button btnPrintBadges;
        private System.Windows.Forms.RadioButton rbGroupPos;
        private System.Windows.Forms.RadioButton rbGroupJudge;
        private System.Windows.Forms.Button btnPrintOneB;
        private System.Windows.Forms.PictureBox pbPhoto;
        private System.Windows.Forms.Button btnDelPhoto;
        private System.Windows.Forms.Button btnLoadPhoto;
        private System.Windows.Forms.Button btnLoadToInet;
        private System.Windows.Forms.Button btnLoadFRinet;
        private System.Windows.Forms.Button btnPosListEdit;
    }
}