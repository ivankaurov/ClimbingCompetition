#if FULL
namespace ClimbingCompetition
{
    partial class ResultListBoulder
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgStart = new System.Windows.Forms.DataGridView();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.xlExport = new System.Windows.Forms.Button();
            this.btnNxtRound = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dgRes = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnStartP = new System.Windows.Forms.Button();
            this.tbPocketPort = new System.Windows.Forms.TextBox();
            this.cbUseSystem = new System.Windows.Forms.CheckBox();
            this.lblUseSystem = new System.Windows.Forms.Label();
            this.tbCOMportSys = new System.Windows.Forms.TextBox();
            this.tbP2 = new System.Windows.Forms.TextBox();
            this.tbP1 = new System.Windows.Forms.TextBox();
            this.btnAddP2 = new System.Windows.Forms.Button();
            this.btnAddP1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb1stRoute = new System.Windows.Forms.TextBox();
            this.btnCorrect = new System.Windows.Forms.Button();
            this.btnStartListening = new System.Windows.Forms.Button();
            this.lblComPort = new System.Windows.Forms.Label();
            this.tbComPortNum = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgStart
            // 
            this.dgStart.AllowUserToAddRows = false;
            this.dgStart.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            this.dgStart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgStart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgStart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgStart.Location = new System.Drawing.Point(0, 0);
            this.dgStart.Name = "dgStart";
            this.dgStart.Size = new System.Drawing.Size(865, 140);
            this.dgStart.TabIndex = 3;
            this.dgStart.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgStart_CellValueChanged);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStart.Location = new System.Drawing.Point(497, 19);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 39);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Arial", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTime.Location = new System.Drawing.Point(663, 5);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(197, 75);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "00:00";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 46);
            this.button1.TabIndex = 11;
            this.button1.Text = "Ввод результатов";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // xlExport
            // 
            this.xlExport.Location = new System.Drawing.Point(154, 15);
            this.xlExport.Name = "xlExport";
            this.xlExport.Size = new System.Drawing.Size(91, 43);
            this.xlExport.TabIndex = 12;
            this.xlExport.Text = "Экспорт в Excel";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
            // 
            // btnNxtRound
            // 
            this.btnNxtRound.Location = new System.Drawing.Point(251, 15);
            this.btnNxtRound.Name = "btnNxtRound";
            this.btnNxtRound.Size = new System.Drawing.Size(91, 43);
            this.btnNxtRound.TabIndex = 13;
            this.btnNxtRound.Text = "Стартовый на след. раунд";
            this.btnNxtRound.UseVisualStyleBackColor = true;
            this.btnNxtRound.Click += new System.EventHandler(this.btnNxtRound_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(348, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(142, 31);
            this.button3.TabIndex = 11;
            this.button3.Text = "Старт 1ого участника";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dgRes
            // 
            this.dgRes.AllowUserToAddRows = false;
            this.dgRes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            this.dgRes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgRes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgRes.Location = new System.Drawing.Point(0, 0);
            this.dgRes.Name = "dgRes";
            this.dgRes.Size = new System.Drawing.Size(865, 180);
            this.dgRes.TabIndex = 16;
            this.dgRes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRes_CellValueChanged);
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
            this.splitContainer1.Panel2.Controls.Add(this.dgStart);
            this.splitContainer1.Size = new System.Drawing.Size(865, 324);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 17;
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
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnStartP);
            this.splitContainer2.Panel2.Controls.Add(this.tbPocketPort);
            this.splitContainer2.Panel2.Controls.Add(this.cbUseSystem);
            this.splitContainer2.Panel2.Controls.Add(this.lblUseSystem);
            this.splitContainer2.Panel2.Controls.Add(this.tbCOMportSys);
            this.splitContainer2.Panel2.Controls.Add(this.tbP2);
            this.splitContainer2.Panel2.Controls.Add(this.tbP1);
            this.splitContainer2.Panel2.Controls.Add(this.btnAddP2);
            this.splitContainer2.Panel2.Controls.Add(this.btnAddP1);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.tb1stRoute);
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Panel2.Controls.Add(this.lblTime);
            this.splitContainer2.Panel2.Controls.Add(this.btnNxtRound);
            this.splitContainer2.Panel2.Controls.Add(this.btnStart);
            this.splitContainer2.Panel2.Controls.Add(this.xlExport);
            this.splitContainer2.Panel2.Controls.Add(this.btnCorrect);
            this.splitContainer2.Panel2.Controls.Add(this.button3);
            this.splitContainer2.Panel2.Controls.Add(this.btnStartListening);
            this.splitContainer2.Panel2.Controls.Add(this.lblComPort);
            this.splitContainer2.Panel2.Controls.Add(this.tbComPortNum);
            this.splitContainer2.Size = new System.Drawing.Size(865, 524);
            this.splitContainer2.SplitterDistance = 324;
            this.splitContainer2.TabIndex = 18;
            // 
            // btnStartP
            // 
            this.btnStartP.Location = new System.Drawing.Point(194, 161);
            this.btnStartP.Name = "btnStartP";
            this.btnStartP.Size = new System.Drawing.Size(75, 23);
            this.btnStartP.TabIndex = 25;
            this.btnStartP.Text = "Start";
            this.btnStartP.UseVisualStyleBackColor = true;
            this.btnStartP.Click += new System.EventHandler(this.btnStartP_Click);
            // 
            // tbPocketPort
            // 
            this.tbPocketPort.Location = new System.Drawing.Point(295, 164);
            this.tbPocketPort.Name = "tbPocketPort";
            this.tbPocketPort.Size = new System.Drawing.Size(40, 20);
            this.tbPocketPort.TabIndex = 24;
            // 
            // cbUseSystem
            // 
            this.cbUseSystem.AutoSize = true;
            this.cbUseSystem.Location = new System.Drawing.Point(394, 160);
            this.cbUseSystem.Name = "cbUseSystem";
            this.cbUseSystem.Size = new System.Drawing.Size(169, 17);
            this.cbUseSystem.TabIndex = 23;
            this.cbUseSystem.Text = "Связать таймер с системой";
            this.cbUseSystem.UseVisualStyleBackColor = true;
            this.cbUseSystem.CheckedChanged += new System.EventHandler(this.cbUseSystem_CheckedChanged);
            // 
            // lblUseSystem
            // 
            this.lblUseSystem.AutoSize = true;
            this.lblUseSystem.Location = new System.Drawing.Point(569, 160);
            this.lblUseSystem.Name = "lblUseSystem";
            this.lblUseSystem.Size = new System.Drawing.Size(172, 13);
            this.lblUseSystem.TabIndex = 22;
            this.lblUseSystem.Text = "СОМ порт для связи с системой";
            // 
            // tbCOMportSys
            // 
            this.tbCOMportSys.Location = new System.Drawing.Point(747, 160);
            this.tbCOMportSys.Name = "tbCOMportSys";
            this.tbCOMportSys.Size = new System.Drawing.Size(100, 20);
            this.tbCOMportSys.TabIndex = 21;
            // 
            // tbP2
            // 
            this.tbP2.Location = new System.Drawing.Point(154, 115);
            this.tbP2.Name = "tbP2";
            this.tbP2.Size = new System.Drawing.Size(116, 20);
            this.tbP2.TabIndex = 20;
            // 
            // tbP1
            // 
            this.tbP1.Location = new System.Drawing.Point(154, 77);
            this.tbP1.Name = "tbP1";
            this.tbP1.Size = new System.Drawing.Size(116, 20);
            this.tbP1.TabIndex = 20;
            // 
            // btnAddP2
            // 
            this.btnAddP2.Location = new System.Drawing.Point(12, 104);
            this.btnAddP2.Name = "btnAddP2";
            this.btnAddP2.Size = new System.Drawing.Size(135, 40);
            this.btnAddP2.TabIndex = 19;
            this.btnAddP2.Text = "Запустить скользящий переход 2 на трассе:";
            this.btnAddP2.UseVisualStyleBackColor = true;
            this.btnAddP2.Click += new System.EventHandler(this.btnAddP1_Click);
            // 
            // btnAddP1
            // 
            this.btnAddP1.Location = new System.Drawing.Point(12, 64);
            this.btnAddP1.Name = "btnAddP1";
            this.btnAddP1.Size = new System.Drawing.Size(135, 40);
            this.btnAddP1.TabIndex = 19;
            this.btnAddP1.Text = "Запустить скользящий переход 1 на трассе:";
            this.btnAddP1.UseVisualStyleBackColor = true;
            this.btnAddP1.Click += new System.EventHandler(this.btnAddP1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(478, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(357, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Введите через пробел номер любой трассы и ст.№ участника на ней";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(363, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Сейчас на 1ой трассе:";
            // 
            // tb1stRoute
            // 
            this.tb1stRoute.Location = new System.Drawing.Point(366, 92);
            this.tb1stRoute.Name = "tb1stRoute";
            this.tb1stRoute.Size = new System.Drawing.Size(496, 20);
            this.tb1stRoute.TabIndex = 15;
            // 
            // btnCorrect
            // 
            this.btnCorrect.Location = new System.Drawing.Point(366, 116);
            this.btnCorrect.Name = "btnCorrect";
            this.btnCorrect.Size = new System.Drawing.Size(106, 31);
            this.btnCorrect.TabIndex = 11;
            this.btnCorrect.Text = "Коррекция";
            this.btnCorrect.UseVisualStyleBackColor = true;
            this.btnCorrect.Click += new System.EventHandler(this.btnCorrect_Click);
            // 
            // btnStartListening
            // 
            this.btnStartListening.Location = new System.Drawing.Point(12, 147);
            this.btnStartListening.Name = "btnStartListening";
            this.btnStartListening.Size = new System.Drawing.Size(135, 35);
            this.btnStartListening.TabIndex = 12;
            this.btnStartListening.Text = "Запустить сервер";
            this.btnStartListening.UseVisualStyleBackColor = true;
            this.btnStartListening.Visible = false;
            this.btnStartListening.Click += new System.EventHandler(this.btnStartListening_Click);
            // 
            // lblComPort
            // 
            this.lblComPort.AutoSize = true;
            this.lblComPort.Location = new System.Drawing.Point(151, 147);
            this.lblComPort.Name = "lblComPort";
            this.lblComPort.Size = new System.Drawing.Size(138, 13);
            this.lblComPort.TabIndex = 16;
            this.lblComPort.Text = "Bluetooth COMport number:";
            this.lblComPort.Visible = false;
            // 
            // tbComPortNum
            // 
            this.tbComPortNum.Location = new System.Drawing.Point(295, 147);
            this.tbComPortNum.Name = "tbComPortNum";
            this.tbComPortNum.Size = new System.Drawing.Size(40, 20);
            this.tbComPortNum.TabIndex = 15;
            this.tbComPortNum.Visible = false;
            // 
            // ResultListBoulder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 524);
            this.Controls.Add(this.splitContainer2);
            this.Name = "ResultListBoulder";
            this.Text = "Боулдеринг";
            this.Load += new System.EventHandler(this.ResultListBoulder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgStart;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.Button btnNxtRound;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dgRes;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb1stRoute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCorrect;
        private System.Windows.Forms.Label lblComPort;
        private System.Windows.Forms.TextBox tbComPortNum;
        private System.Windows.Forms.Button btnStartListening;
        private System.Windows.Forms.Button btnAddP1;
        private System.Windows.Forms.TextBox tbP2;
        private System.Windows.Forms.TextBox tbP1;
        private System.Windows.Forms.Button btnAddP2;
        private System.Windows.Forms.CheckBox cbUseSystem;
        private System.Windows.Forms.Label lblUseSystem;
        private System.Windows.Forms.TextBox tbCOMportSys;
        private System.Windows.Forms.Button btnStartP;
        private System.Windows.Forms.TextBox tbPocketPort;
    }
}
#endif