namespace ClimbingCompetition
{
    partial class All_Lists
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
            this.dgLists = new System.Windows.Forms.DataGridView();
            this.cbStyle = new System.Windows.Forms.ComboBox();
            this.cbGroup = new System.Windows.Forms.ComboBox();
            this.cbRound = new System.Windows.Forms.ComboBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIid = new System.Windows.Forms.Label();
            this.viewStartList = new System.Windows.Forms.Button();
            this.viewResultList = new System.Windows.Forms.Button();
            this.btnSubmitChanges = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbJudge = new System.Windows.Forms.ComboBox();
            this.tbIsoOpen = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIsoClose = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbObserv = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbStart = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPrintAct = new System.Windows.Forms.Button();
            this.cbRouteSetter = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbProjNum = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRouteNumber = new System.Windows.Forms.TextBox();
            this.lblRouteNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgLists)).BeginInit();
            this.SuspendLayout();
            // 
            // dgLists
            // 
            this.dgLists.AllowUserToAddRows = false;
            this.dgLists.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            this.dgLists.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgLists.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgLists.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLists.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgLists.Location = new System.Drawing.Point(0, 0);
            this.dgLists.Name = "dgLists";
            this.dgLists.Size = new System.Drawing.Size(923, 340);
            this.dgLists.TabIndex = 1;
            this.dgLists.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgLists_CellValueChanged);
            this.dgLists.DoubleClick += new System.EventHandler(this.viewResultList_Click);
            this.dgLists.SelectionChanged += new System.EventHandler(this.dg_lead_SelectionChanged);
            // 
            // cbStyle
            // 
            this.cbStyle.FormattingEnabled = true;
            this.cbStyle.Items.AddRange(new object[] {
            "-- Все виды --",
            "Трудность",
            "Скорость",
            "Боулдеринг",
            "Многоборье",
            "Командные - Тр",
            "Командные - Ск",
            "Командные - Б",
            "Командные - Мн"});
            this.cbStyle.Location = new System.Drawing.Point(16, 346);
            this.cbStyle.Name = "cbStyle";
            this.cbStyle.Size = new System.Drawing.Size(150, 21);
            this.cbStyle.TabIndex = 2;
            this.cbStyle.Text = "Вид";
            this.cbStyle.SelectedIndexChanged += new System.EventHandler(this.cbStyle_SelectedIndexChanged);
            // 
            // cbGroup
            // 
            this.cbGroup.FormattingEnabled = true;
            this.cbGroup.Location = new System.Drawing.Point(172, 347);
            this.cbGroup.Name = "cbGroup";
            this.cbGroup.Size = new System.Drawing.Size(156, 21);
            this.cbGroup.TabIndex = 3;
            this.cbGroup.Text = "Выберите группу";
            this.cbGroup.SelectedIndexChanged += new System.EventHandler(this.cbGroup_SelectedIndexChanged);
            // 
            // cbRound
            // 
            this.cbRound.FormattingEnabled = true;
            this.cbRound.Location = new System.Drawing.Point(172, 376);
            this.cbRound.Name = "cbRound";
            this.cbRound.Size = new System.Drawing.Size(156, 21);
            this.cbRound.TabIndex = 4;
            this.cbRound.Text = "Раунд";
            this.cbRound.SelectedIndexChanged += new System.EventHandler(this.cbRound_SelectedIndexChanged);
            this.cbRound.DropDown += new System.EventHandler(this.cbRound_DropDown);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(228, 458);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(100, 36);
            this.btnDel.TabIndex = 5;
            this.btnDel.Text = "Удалить";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(122, 458);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 36);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(16, 458);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 36);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Правка";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 405);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Номер протокола:";
            // 
            // tbIid
            // 
            this.tbIid.AutoSize = true;
            this.tbIid.Location = new System.Drawing.Point(318, 405);
            this.tbIid.Name = "tbIid";
            this.tbIid.Size = new System.Drawing.Size(0, 13);
            this.tbIid.TabIndex = 10;
            // 
            // viewStartList
            // 
            this.viewStartList.Location = new System.Drawing.Point(355, 346);
            this.viewStartList.Name = "viewStartList";
            this.viewStartList.Size = new System.Drawing.Size(102, 51);
            this.viewStartList.TabIndex = 11;
            this.viewStartList.Text = "Просмотр стартового протокола";
            this.viewStartList.UseVisualStyleBackColor = true;
            this.viewStartList.Click += new System.EventHandler(this.viewStartList_Click);
            // 
            // viewResultList
            // 
            this.viewResultList.Location = new System.Drawing.Point(463, 346);
            this.viewResultList.Name = "viewResultList";
            this.viewResultList.Size = new System.Drawing.Size(102, 51);
            this.viewResultList.TabIndex = 11;
            this.viewResultList.Text = "Просмотр протокола результатов";
            this.viewResultList.UseVisualStyleBackColor = true;
            this.viewResultList.Click += new System.EventHandler(this.viewResultList_Click);
            // 
            // btnSubmitChanges
            // 
            this.btnSubmitChanges.Location = new System.Drawing.Point(571, 347);
            this.btnSubmitChanges.Name = "btnSubmitChanges";
            this.btnSubmitChanges.Size = new System.Drawing.Size(100, 50);
            this.btnSubmitChanges.TabIndex = 5;
            this.btnSubmitChanges.Text = "Подтвердить изменения";
            this.btnSubmitChanges.UseVisualStyleBackColor = true;
            this.btnSubmitChanges.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(677, 346);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(114, 50);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbJudge
            // 
            this.cbJudge.FormattingEnabled = true;
            this.cbJudge.Location = new System.Drawing.Point(16, 397);
            this.cbJudge.Name = "cbJudge";
            this.cbJudge.Size = new System.Drawing.Size(150, 21);
            this.cbJudge.TabIndex = 13;
            this.cbJudge.Text = "Зам. по виду";
            // 
            // tbIsoOpen
            // 
            this.tbIsoOpen.Location = new System.Drawing.Point(355, 433);
            this.tbIsoOpen.Name = "tbIsoOpen";
            this.tbIsoOpen.Size = new System.Drawing.Size(102, 20);
            this.tbIsoOpen.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(464, 439);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Открытие зоны изоляции";
            // 
            // tbIsoClose
            // 
            this.tbIsoClose.Location = new System.Drawing.Point(355, 459);
            this.tbIsoClose.Name = "tbIsoClose";
            this.tbIsoClose.Size = new System.Drawing.Size(102, 20);
            this.tbIsoClose.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(464, 465);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Закрытие зоны изоляции";
            // 
            // tbObserv
            // 
            this.tbObserv.Location = new System.Drawing.Point(607, 433);
            this.tbObserv.Name = "tbObserv";
            this.tbObserv.Size = new System.Drawing.Size(102, 20);
            this.tbObserv.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(716, 439);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Просмотр";
            // 
            // tbStart
            // 
            this.tbStart.Location = new System.Drawing.Point(607, 458);
            this.tbStart.Name = "tbStart";
            this.tbStart.Size = new System.Drawing.Size(102, 20);
            this.tbStart.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(716, 464);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Время старта";
            // 
            // btnPrintAct
            // 
            this.btnPrintAct.Location = new System.Drawing.Point(797, 347);
            this.btnPrintAct.Name = "btnPrintAct";
            this.btnPrintAct.Size = new System.Drawing.Size(114, 51);
            this.btnPrintAct.TabIndex = 16;
            this.btnPrintAct.Text = "Печать акта готовности трассы";
            this.btnPrintAct.UseVisualStyleBackColor = true;
            this.btnPrintAct.Click += new System.EventHandler(this.btnPrintAct_Click);
            // 
            // cbRouteSetter
            // 
            this.cbRouteSetter.FormattingEnabled = true;
            this.cbRouteSetter.Location = new System.Drawing.Point(463, 405);
            this.cbRouteSetter.Name = "cbRouteSetter";
            this.cbRouteSetter.Size = new System.Drawing.Size(246, 21);
            this.cbRouteSetter.TabIndex = 17;
            this.cbRouteSetter.Text = "Начальник трассы";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 379);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Зам. по виду";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(352, 405);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Начальник трассы:";
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
            this.cbProjNum.Location = new System.Drawing.Point(865, 402);
            this.cbProjNum.Name = "cbProjNum";
            this.cbProjNum.Size = new System.Drawing.Size(46, 21);
            this.cbProjNum.TabIndex = 20;
            this.cbProjNum.Text = "1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(800, 405);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Проектор:";
            // 
            // tbRouteNumber
            // 
            this.tbRouteNumber.Location = new System.Drawing.Point(280, 433);
            this.tbRouteNumber.Name = "tbRouteNumber";
            this.tbRouteNumber.Size = new System.Drawing.Size(47, 20);
            this.tbRouteNumber.TabIndex = 21;
            // 
            // lblRouteNumber
            // 
            this.lblRouteNumber.AutoSize = true;
            this.lblRouteNumber.Location = new System.Drawing.Point(176, 433);
            this.lblRouteNumber.Name = "lblRouteNumber";
            this.lblRouteNumber.Size = new System.Drawing.Size(101, 13);
            this.lblRouteNumber.TabIndex = 22;
            this.lblRouteNumber.Text = "Количество трасс:";
            // 
            // All_Lists
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 504);
            this.Controls.Add(this.lblRouteNumber);
            this.Controls.Add(this.tbRouteNumber);
            this.Controls.Add(this.cbProjNum);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbRouteSetter);
            this.Controls.Add(this.btnPrintAct);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStart);
            this.Controls.Add(this.tbObserv);
            this.Controls.Add(this.tbIsoClose);
            this.Controls.Add(this.tbIsoOpen);
            this.Controls.Add(this.cbJudge);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.viewResultList);
            this.Controls.Add(this.viewStartList);
            this.Controls.Add(this.tbIid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSubmitChanges);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.cbRound);
            this.Controls.Add(this.cbGroup);
            this.Controls.Add(this.cbStyle);
            this.Controls.Add(this.dgLists);
            this.Name = "All_Lists";
            this.Text = "Протоколы";
            ((System.ComponentModel.ISupportInitialize)(this.dgLists)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgLists;
        private System.Windows.Forms.ComboBox cbStyle;
        private System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.ComboBox cbRound;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label tbIid;
        private System.Windows.Forms.Button viewStartList;
        private System.Windows.Forms.Button viewResultList;
        private System.Windows.Forms.Button btnSubmitChanges;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbJudge;
        private System.Windows.Forms.TextBox tbIsoOpen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIsoClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbObserv;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnPrintAct;
        private System.Windows.Forms.ComboBox cbRouteSetter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbProjNum;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRouteNumber;
        private System.Windows.Forms.Label lblRouteNumber;

    }
}