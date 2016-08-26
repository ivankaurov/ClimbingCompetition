#if FULL
namespace ClimbingCompetition
{
    partial class ResultListLead
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultListLead));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgRes = new System.Windows.Forms.DataGridView();
            this.dgStart = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblIID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTeam = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblQf = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tbRes = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblPos = new System.Windows.Forms.Label();
            this.xlExport = new System.Windows.Forms.Button();
            this.btnNxtRound = new System.Windows.Forms.Button();
            this.cbRound = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lblTimeClimb = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnRollBack = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.btnEditTopo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgRes
            // 
            this.dgRes.AllowUserToAddRows = false;
            this.dgRes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Silver;
            this.dgRes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgRes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgRes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgRes, "dgRes");
            this.dgRes.Name = "dgRes";
            this.dgRes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRes_CellClick);
            this.dgRes.DoubleClick += new System.EventHandler(this.dgRes_DoubleClick);
            // 
            // dgStart
            // 
            this.dgStart.AllowUserToAddRows = false;
            this.dgStart.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Silver;
            this.dgStart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgStart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgStart.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgStart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgStart, "dgStart");
            this.dgStart.Name = "dgStart";
            this.dgStart.ReadOnly = true;
            this.dgStart.SelectionChanged += new System.EventHandler(this.dgStart_SelectionChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblStart
            // 
            resources.ApplyResources(this.lblStart, "lblStart");
            this.lblStart.Name = "lblStart";
            this.lblStart.TextChanged += new System.EventHandler(this.lblStart_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblIID
            // 
            resources.ApplyResources(this.lblIID, "lblIID");
            this.lblIID.Name = "lblIID";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // lblTeam
            // 
            resources.ApplyResources(this.lblTeam, "lblTeam");
            this.lblTeam.Name = "lblTeam";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // lblAge
            // 
            resources.ApplyResources(this.lblAge, "lblAge");
            this.lblAge.Name = "lblAge";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // lblQf
            // 
            resources.ApplyResources(this.lblQf, "lblQf");
            this.lblQf.Name = "lblQf";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tbRes
            // 
            resources.ApplyResources(this.tbRes, "tbRes");
            this.tbRes.Name = "tbRes";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblTime
            // 
            resources.ApplyResources(this.lblTime, "lblTime");
            this.lblTime.Name = "lblTime";
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblPos
            // 
            resources.ApplyResources(this.lblPos, "lblPos");
            this.lblPos.Name = "lblPos";
            // 
            // xlExport
            // 
            resources.ApplyResources(this.xlExport, "xlExport");
            this.xlExport.Name = "xlExport";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
            // 
            // btnNxtRound
            // 
            resources.ApplyResources(this.btnNxtRound, "btnNxtRound");
            this.btnNxtRound.Name = "btnNxtRound";
            this.btnNxtRound.UseVisualStyleBackColor = true;
            this.btnNxtRound.Click += new System.EventHandler(this.btnNxtRound_Click);
            // 
            // cbRound
            // 
            this.cbRound.FormattingEnabled = true;
            this.cbRound.Items.AddRange(new object[] {
            resources.GetString("cbRound.Items"),
            resources.GetString("cbRound.Items1"),
            resources.GetString("cbRound.Items2"),
            resources.GetString("cbRound.Items3")});
            resources.ApplyResources(this.cbRound, "cbRound");
            this.cbRound.Name = "cbRound";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgRes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgStart);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnEditTopo);
            this.splitContainer2.Panel2.Controls.Add(this.lblTimeClimb);
            this.splitContainer2.Panel2.Controls.Add(this.btnRestore);
            this.splitContainer2.Panel2.Controls.Add(this.btnRollBack);
            this.splitContainer2.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Panel2.Controls.Add(this.cbRound);
            this.splitContainer2.Panel2.Controls.Add(this.xlExport);
            this.splitContainer2.Panel2.Controls.Add(this.btnNxtRound);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.btnEdit);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.lblTime);
            this.splitContainer2.Panel2.Controls.Add(this.label7);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.label9);
            this.splitContainer2.Panel2.Controls.Add(this.tbTime);
            this.splitContainer2.Panel2.Controls.Add(this.tbRes);
            this.splitContainer2.Panel2.Controls.Add(this.label11);
            this.splitContainer2.Panel2.Controls.Add(this.lblPos);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.lblIID);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.lblQf);
            this.splitContainer2.Panel2.Controls.Add(this.lblStart);
            this.splitContainer2.Panel2.Controls.Add(this.lblAge);
            this.splitContainer2.Panel2.Controls.Add(this.lblName);
            this.splitContainer2.Panel2.Controls.Add(this.lblTeam);
            // 
            // lblTimeClimb
            // 
            resources.ApplyResources(this.lblTimeClimb, "lblTimeClimb");
            this.lblTimeClimb.Name = "lblTimeClimb";
            // 
            // btnRestore
            // 
            this.btnRestore.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.btnRestore, "btnRestore");
            this.btnRestore.Image = global::ClimbingCompetition.Properties.Resources.restore;
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // btnRollBack
            // 
            this.btnRollBack.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.btnRollBack, "btnRollBack");
            this.btnRollBack.Image = global::ClimbingCompetition.Properties.Resources.rollback;
            this.btnRollBack.Name = "btnRollBack";
            this.btnRollBack.UseVisualStyleBackColor = false;
            this.btnRollBack.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbTime
            // 
            resources.ApplyResources(this.tbTime, "tbTime");
            this.tbTime.Name = "tbTime";
            this.tbTime.ReadOnly = true;
            // 
            // btnEditTopo
            // 
            resources.ApplyResources(this.btnEditTopo, "btnEditTopo");
            this.btnEditTopo.Name = "btnEditTopo";
            this.btnEditTopo.UseVisualStyleBackColor = true;
            this.btnEditTopo.Click += new System.EventHandler(this.btnEditTopo_Click);
            // 
            // ResultListLead
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ResultListLead";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResultListLead_FormClosing);
            this.Load += new System.EventHandler(this.ResultListLead_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgStart)).EndInit();
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

        private System.Windows.Forms.DataGridView dgRes;
        private System.Windows.Forms.DataGridView dgStart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblIID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTeam;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblQf;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox tbRes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblPos;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.Button btnNxtRound;
        private System.Windows.Forms.ComboBox cbRound;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRollBack;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.TextBox tbTime;
        private System.Windows.Forms.Label lblTimeClimb;
        private System.Windows.Forms.Button btnEditTopo;
    }
}
#endif