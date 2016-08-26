using System;
using System.Windows.Forms;
namespace ClimbingCompetition
{
    partial class ResultListSpeed
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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch  { /*MessageBox.Show("Ошибка закрытия формы\r\n" + ex.Message);*/ }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgRes = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgStart = new System.Windows.Forms.DataGridView();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnRollBack = new System.Windows.Forms.Button();
            this.btnPrintPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbQf2 = new System.Windows.Forms.RadioButton();
            this.rdPairs = new System.Windows.Forms.RadioButton();
            this.btnNextRound = new System.Windows.Forms.Button();
            this.xlExport = new System.Windows.Forms.Button();
            this.lblPos = new System.Windows.Forms.Label();
            this.lblIID = new System.Windows.Forms.Label();
            this.lblQf = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblTeam = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbRoute2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRoute1 = new System.Windows.Forms.TextBox();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbSystem = new System.Windows.Forms.RadioButton();
            this.rbRoute1 = new System.Windows.Forms.RadioButton();
            this.rbBothRoutes = new System.Windows.Forms.RadioButton();
            this.rbRoute2 = new System.Windows.Forms.RadioButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.gbMode.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.dgRes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(915, 578);
            this.splitContainer1.SplitterDistance = 233;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgRes
            // 
            this.dgRes.AllowUserToAddRows = false;
            this.dgRes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            this.dgRes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgRes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgRes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgRes.Location = new System.Drawing.Point(0, 0);
            this.dgRes.Name = "dgRes";
            this.dgRes.Size = new System.Drawing.Size(915, 233);
            this.dgRes.TabIndex = 0;
            this.dgRes.SelectionChanged += new System.EventHandler(this.dgRes_DoubleClick);
            this.dgRes.DoubleClick += new System.EventHandler(this.dgRes_DoubleClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgStart);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnRestore);
            this.splitContainer2.Panel2.Controls.Add(this.btnRefresh);
            this.splitContainer2.Panel2.Controls.Add(this.btnRollBack);
            this.splitContainer2.Panel2.Controls.Add(this.btnPrintPreview);
            this.splitContainer2.Panel2.Controls.Add(this.btnPrint);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel2.Controls.Add(this.xlExport);
            this.splitContainer2.Panel2.Controls.Add(this.lblPos);
            this.splitContainer2.Panel2.Controls.Add(this.lblIID);
            this.splitContainer2.Panel2.Controls.Add(this.lblQf);
            this.splitContainer2.Panel2.Controls.Add(this.lblAge);
            this.splitContainer2.Panel2.Controls.Add(this.lblTeam);
            this.splitContainer2.Panel2.Controls.Add(this.lblName);
            this.splitContainer2.Panel2.Controls.Add(this.lblStart);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.label11);
            this.splitContainer2.Panel2.Controls.Add(this.label9);
            this.splitContainer2.Panel2.Controls.Add(this.label7);
            this.splitContainer2.Panel2.Controls.Add(this.label8);
            this.splitContainer2.Panel2.Controls.Add(this.label10);
            this.splitContainer2.Panel2.Controls.Add(this.btnNext);
            this.splitContainer2.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer2.Panel2.Controls.Add(this.btnEdit);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.tbSum);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.tbRoute2);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.tbRoute1);
            this.splitContainer2.Panel2.Controls.Add(this.gbMode);
            this.splitContainer2.Size = new System.Drawing.Size(915, 341);
            this.splitContainer2.SplitterDistance = 159;
            this.splitContainer2.TabIndex = 0;
            // 
            // dgStart
            // 
            this.dgStart.AllowUserToAddRows = false;
            this.dgStart.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            this.dgStart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgStart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgStart.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgStart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgStart.Location = new System.Drawing.Point(0, 0);
            this.dgStart.Name = "dgStart";
            this.dgStart.Size = new System.Drawing.Size(915, 159);
            this.dgStart.TabIndex = 0;
            this.dgStart.SelectionChanged += new System.EventHandler(this.dgStart_SelectionChanged);
            this.dgStart.DoubleClick += new System.EventHandler(this.dgStart_SelectionChanged);
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.SystemColors.Window;
            this.btnRestore.Enabled = false;
            this.btnRestore.Image = global::ClimbingCompetition.Properties.Resources.restore;
            this.btnRestore.Location = new System.Drawing.Point(461, 76);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(24, 23);
            this.btnRestore.TabIndex = 16;
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(498, 127);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(129, 37);
            this.btnRefresh.TabIndex = 36;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnRollBack
            // 
            this.btnRollBack.BackColor = System.Drawing.SystemColors.Window;
            this.btnRollBack.Enabled = false;
            this.btnRollBack.Image = global::ClimbingCompetition.Properties.Resources.rollback;
            this.btnRollBack.Location = new System.Drawing.Point(431, 76);
            this.btnRollBack.Name = "btnRollBack";
            this.btnRollBack.Size = new System.Drawing.Size(24, 23);
            this.btnRollBack.TabIndex = 15;
            this.btnRollBack.UseVisualStyleBackColor = false;
            this.btnRollBack.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // btnPrintPreview
            // 
            this.btnPrintPreview.Location = new System.Drawing.Point(701, 137);
            this.btnPrintPreview.Name = "btnPrintPreview";
            this.btnPrintPreview.Size = new System.Drawing.Size(114, 40);
            this.btnPrintPreview.TabIndex = 35;
            this.btnPrintPreview.Text = "Предварительный просмотр";
            this.btnPrintPreview.UseVisualStyleBackColor = true;
            this.btnPrintPreview.Click += new System.EventHandler(this.btnPrintPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(821, 137);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(91, 38);
            this.btnPrint.TabIndex = 34;
            this.btnPrint.Text = "Печать";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbQf2);
            this.groupBox2.Controls.Add(this.rdPairs);
            this.groupBox2.Controls.Add(this.btnNextRound);
            this.groupBox2.Location = new System.Drawing.Point(151, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 69);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Следующий раунд";
            // 
            // rbQf2
            // 
            this.rbQf2.AutoSize = true;
            this.rbQf2.Location = new System.Drawing.Point(19, 19);
            this.rbQf2.Name = "rbQf2";
            this.rbQf2.Size = new System.Drawing.Size(109, 17);
            this.rbQf2.TabIndex = 0;
            this.rbQf2.TabStop = true;
            this.rbQf2.Text = "Квалификация 2";
            this.rbQf2.UseVisualStyleBackColor = true;
            this.rbQf2.CheckedChanged += new System.EventHandler(this.rbRoute1_CheckedChanged);
            // 
            // rdPairs
            // 
            this.rdPairs.AutoSize = true;
            this.rdPairs.Location = new System.Drawing.Point(19, 42);
            this.rdPairs.Name = "rdPairs";
            this.rdPairs.Size = new System.Drawing.Size(95, 17);
            this.rdPairs.TabIndex = 0;
            this.rdPairs.TabStop = true;
            this.rdPairs.Text = "Парная гонка";
            this.rdPairs.UseVisualStyleBackColor = true;
            this.rdPairs.CheckedChanged += new System.EventHandler(this.rbRoute1_CheckedChanged);
            // 
            // btnNextRound
            // 
            this.btnNextRound.Location = new System.Drawing.Point(179, 12);
            this.btnNextRound.Name = "btnNextRound";
            this.btnNextRound.Size = new System.Drawing.Size(98, 47);
            this.btnNextRound.TabIndex = 4;
            this.btnNextRound.Text = "Следующий раунд";
            this.btnNextRound.UseVisualStyleBackColor = true;
            this.btnNextRound.Click += new System.EventHandler(this.btnNextRound_Click);
            // 
            // xlExport
            // 
            this.xlExport.Location = new System.Drawing.Point(821, 88);
            this.xlExport.Name = "xlExport";
            this.xlExport.Size = new System.Drawing.Size(91, 43);
            this.xlExport.TabIndex = 32;
            this.xlExport.Text = "Экспорт в Excel";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
            // 
            // lblPos
            // 
            this.lblPos.AutoSize = true;
            this.lblPos.Location = new System.Drawing.Point(428, 10);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(0, 13);
            this.lblPos.TabIndex = 24;
            this.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIID
            // 
            this.lblIID.AutoSize = true;
            this.lblIID.Location = new System.Drawing.Point(335, 10);
            this.lblIID.Name = "lblIID";
            this.lblIID.Size = new System.Drawing.Size(0, 13);
            this.lblIID.TabIndex = 23;
            this.lblIID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblQf
            // 
            this.lblQf.AutoSize = true;
            this.lblQf.Location = new System.Drawing.Point(335, 76);
            this.lblQf.Name = "lblQf";
            this.lblQf.Size = new System.Drawing.Size(0, 13);
            this.lblQf.TabIndex = 26;
            this.lblQf.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Location = new System.Drawing.Point(241, 76);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(0, 13);
            this.lblAge.TabIndex = 25;
            this.lblAge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTeam
            // 
            this.lblTeam.AutoSize = true;
            this.lblTeam.Location = new System.Drawing.Point(241, 55);
            this.lblTeam.Name = "lblTeam";
            this.lblTeam.Size = new System.Drawing.Size(0, 13);
            this.lblTeam.TabIndex = 22;
            this.lblTeam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(241, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 13);
            this.lblName.TabIndex = 20;
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(241, 10);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(0, 13);
            this.lblStart.TabIndex = 21;
            this.lblStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(380, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Место:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(308, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "№:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(282, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Разряд:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(207, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Г.р.:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(180, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Команда:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(148, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Фамилия, Имя:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(196, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Ст. №:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(498, 86);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(129, 34);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "Следующий участник";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(741, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(631, 87);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(104, 33);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "Правка";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Сумма:";
            // 
            // tbSum
            // 
            this.tbSum.Location = new System.Drawing.Point(604, 61);
            this.tbSum.Name = "tbSum";
            this.tbSum.Size = new System.Drawing.Size(199, 20);
            this.tbSum.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(541, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Трасса 2:";
            // 
            // tbRoute2
            // 
            this.tbRoute2.Location = new System.Drawing.Point(604, 42);
            this.tbRoute2.Name = "tbRoute2";
            this.tbRoute2.Size = new System.Drawing.Size(199, 20);
            this.tbRoute2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(541, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Трасса 1:";
            // 
            // tbRoute1
            // 
            this.tbRoute1.Location = new System.Drawing.Point(604, 23);
            this.tbRoute1.Name = "tbRoute1";
            this.tbRoute1.Size = new System.Drawing.Size(199, 20);
            this.tbRoute1.TabIndex = 2;
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbSystem);
            this.gbMode.Controls.Add(this.rbRoute1);
            this.gbMode.Controls.Add(this.rbBothRoutes);
            this.gbMode.Controls.Add(this.rbRoute2);
            this.gbMode.Location = new System.Drawing.Point(3, 10);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(142, 131);
            this.gbMode.TabIndex = 1;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "Режим ввода результатов";
            // 
            // rbSystem
            // 
            this.rbSystem.AutoSize = true;
            this.rbSystem.Location = new System.Drawing.Point(6, 101);
            this.rbSystem.Name = "rbSystem";
            this.rbSystem.Size = new System.Drawing.Size(107, 17);
            this.rbSystem.TabIndex = 1;
            this.rbSystem.TabStop = true;
            this.rbSystem.Text = "Ввод с системы";
            this.rbSystem.UseVisualStyleBackColor = true;
            this.rbSystem.CheckedChanged += new System.EventHandler(this.rbSystem_CheckedChanged);
            // 
            // rbRoute1
            // 
            this.rbRoute1.AutoSize = true;
            this.rbRoute1.Location = new System.Drawing.Point(6, 32);
            this.rbRoute1.Name = "rbRoute1";
            this.rbRoute1.Size = new System.Drawing.Size(71, 17);
            this.rbRoute1.TabIndex = 0;
            this.rbRoute1.TabStop = true;
            this.rbRoute1.Text = "Трасса 1";
            this.rbRoute1.UseVisualStyleBackColor = true;
            this.rbRoute1.CheckedChanged += new System.EventHandler(this.rbRoute1_CheckedChanged);
            // 
            // rbBothRoutes
            // 
            this.rbBothRoutes.AutoSize = true;
            this.rbBothRoutes.Location = new System.Drawing.Point(6, 78);
            this.rbBothRoutes.Name = "rbBothRoutes";
            this.rbBothRoutes.Size = new System.Drawing.Size(132, 17);
            this.rbBothRoutes.TabIndex = 0;
            this.rbBothRoutes.TabStop = true;
            this.rbBothRoutes.Text = "Обе трассы / правка";
            this.rbBothRoutes.UseVisualStyleBackColor = true;
            this.rbBothRoutes.CheckedChanged += new System.EventHandler(this.rbRoute1_CheckedChanged);
            // 
            // rbRoute2
            // 
            this.rbRoute2.AutoSize = true;
            this.rbRoute2.Location = new System.Drawing.Point(6, 55);
            this.rbRoute2.Name = "rbRoute2";
            this.rbRoute2.Size = new System.Drawing.Size(71, 17);
            this.rbRoute2.TabIndex = 0;
            this.rbRoute2.TabStop = true;
            this.rbRoute2.Text = "Трасса 2";
            this.rbRoute2.UseVisualStyleBackColor = true;
            this.rbRoute2.CheckedChanged += new System.EventHandler(this.rbRoute1_CheckedChanged);
            // 
            // ResultListSpeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 578);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ResultListSpeed";
            this.Text = "Скорость";
            this.Load += new System.EventHandler(this.ResultListSpeed_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgRes;
        private System.Windows.Forms.DataGridView dgStart;
        private System.Windows.Forms.RadioButton rbBothRoutes;
        private System.Windows.Forms.RadioButton rbRoute2;
        private System.Windows.Forms.RadioButton rbRoute1;
        private System.Windows.Forms.GroupBox gbMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbRoute2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRoute1;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblPos;
        private System.Windows.Forms.Label lblIID;
        private System.Windows.Forms.Label lblQf;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.Label lblTeam;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbQf2;
        private System.Windows.Forms.RadioButton rdPairs;
        private System.Windows.Forms.Button btnNextRound;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrintPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RadioButton rbSystem;
        private Button btnRestore;
        private Button btnRollBack;
    }
}