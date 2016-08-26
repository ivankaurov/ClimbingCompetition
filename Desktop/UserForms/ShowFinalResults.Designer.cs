namespace ClimbingCompetition
{
    partial class ShowFinalResults
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
            this.dg = new System.Windows.Forms.DataGridView();
            this.xlExport = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCalcQfs = new System.Windows.Forms.Button();
            this.gbParallel = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbGName = new System.Windows.Forms.TextBox();
            this.btnCount = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbYoung = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbOld = new System.Windows.Forms.TextBox();
            this.cbParallel = new System.Windows.Forms.CheckBox();
            this.cbRestrict = new System.Windows.Forms.CheckBox();
            this.btnRecalc = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbParallel.SuspendLayout();
            this.SuspendLayout();
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
            this.dg.Size = new System.Drawing.Size(912, 433);
            this.dg.TabIndex = 0;
            this.dg.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_CellEndEdit);
            // 
            // xlExport
            // 
            this.xlExport.Location = new System.Drawing.Point(3, 3);
            this.xlExport.Name = "xlExport";
            this.xlExport.Size = new System.Drawing.Size(91, 43);
            this.xlExport.TabIndex = 13;
            this.xlExport.Text = "Экспорт в Excel";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
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
            this.splitContainer1.Panel2.Controls.Add(this.btnRecalc);
            this.splitContainer1.Panel2.Controls.Add(this.btnCalcQfs);
            this.splitContainer1.Panel2.Controls.Add(this.gbParallel);
            this.splitContainer1.Panel2.Controls.Add(this.cbParallel);
            this.splitContainer1.Panel2.Controls.Add(this.cbRestrict);
            this.splitContainer1.Panel2.Controls.Add(this.xlExport);
            this.splitContainer1.Size = new System.Drawing.Size(912, 529);
            this.splitContainer1.SplitterDistance = 433;
            this.splitContainer1.TabIndex = 14;
            // 
            // btnCalcQfs
            // 
            this.btnCalcQfs.Location = new System.Drawing.Point(3, 52);
            this.btnCalcQfs.Name = "btnCalcQfs";
            this.btnCalcQfs.Size = new System.Drawing.Size(91, 34);
            this.btnCalcQfs.TabIndex = 19;
            this.btnCalcQfs.Text = "Посчитать разрядников";
            this.btnCalcQfs.UseVisualStyleBackColor = true;
            this.btnCalcQfs.Click += new System.EventHandler(this.btnCalcQfs_Click);
            // 
            // gbParallel
            // 
            this.gbParallel.Controls.Add(this.label3);
            this.gbParallel.Controls.Add(this.tbGName);
            this.gbParallel.Controls.Add(this.btnCount);
            this.gbParallel.Controls.Add(this.label2);
            this.gbParallel.Controls.Add(this.tbYoung);
            this.gbParallel.Controls.Add(this.label1);
            this.gbParallel.Controls.Add(this.tbOld);
            this.gbParallel.Enabled = false;
            this.gbParallel.Location = new System.Drawing.Point(264, 44);
            this.gbParallel.Name = "gbParallel";
            this.gbParallel.Size = new System.Drawing.Size(636, 42);
            this.gbParallel.TabIndex = 18;
            this.gbParallel.TabStop = false;
            this.gbParallel.Text = "Параллельный зачёт";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Название группы:";
            // 
            // tbGName
            // 
            this.tbGName.Location = new System.Drawing.Point(404, 14);
            this.tbGName.Name = "tbGName";
            this.tbGName.Size = new System.Drawing.Size(141, 20);
            this.tbGName.TabIndex = 5;
            // 
            // btnCount
            // 
            this.btnCount.Location = new System.Drawing.Point(551, 14);
            this.btnCount.Name = "btnCount";
            this.btnCount.Size = new System.Drawing.Size(79, 23);
            this.btnCount.TabIndex = 4;
            this.btnCount.Text = "Подсчитать";
            this.btnCount.UseVisualStyleBackColor = true;
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Младший г.р.";
            // 
            // tbYoung
            // 
            this.tbYoung.Location = new System.Drawing.Point(236, 16);
            this.tbYoung.Name = "tbYoung";
            this.tbYoung.Size = new System.Drawing.Size(62, 20);
            this.tbYoung.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Старший г.р.";
            // 
            // tbOld
            // 
            this.tbOld.Location = new System.Drawing.Point(79, 16);
            this.tbOld.Name = "tbOld";
            this.tbOld.Size = new System.Drawing.Size(62, 20);
            this.tbOld.TabIndex = 0;
            // 
            // cbParallel
            // 
            this.cbParallel.AutoSize = true;
            this.cbParallel.Location = new System.Drawing.Point(125, 45);
            this.cbParallel.Name = "cbParallel";
            this.cbParallel.Size = new System.Drawing.Size(133, 17);
            this.cbParallel.TabIndex = 17;
            this.cbParallel.Text = "Параллельный зачёт";
            this.cbParallel.UseVisualStyleBackColor = true;
            this.cbParallel.CheckedChanged += new System.EventHandler(this.cbParallel_CheckedChanged);
            // 
            // cbRestrict
            // 
            this.cbRestrict.AutoSize = true;
            this.cbRestrict.Checked = true;
            this.cbRestrict.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRestrict.Location = new System.Drawing.Point(125, 21);
            this.cbRestrict.Name = "cbRestrict";
            this.cbRestrict.Size = new System.Drawing.Size(170, 17);
            this.cbRestrict.TabIndex = 14;
            this.cbRestrict.Text = "Баллы получают только 75%";
            this.cbRestrict.UseVisualStyleBackColor = true;
            this.cbRestrict.CheckedChanged += new System.EventHandler(this.cbRestrict_CheckedChanged);
            // 
            // btnRecalc
            // 
            this.btnRecalc.Location = new System.Drawing.Point(798, 13);
            this.btnRecalc.Name = "btnRecalc";
            this.btnRecalc.Size = new System.Drawing.Size(92, 23);
            this.btnRecalc.TabIndex = 20;
            this.btnRecalc.Text = "Пересчитать";
            this.btnRecalc.UseVisualStyleBackColor = true;
            this.btnRecalc.Click += new System.EventHandler(this.btnRecalc_Click);
            // 
            // ShowFinalResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 529);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ShowFinalResults";
            this.Text = "ShowFinalResults";
            this.Load += new System.EventHandler(this.ShowFinalResults_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.gbParallel.ResumeLayout(false);
            this.gbParallel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox cbRestrict;
        private System.Windows.Forms.GroupBox gbParallel;
        private System.Windows.Forms.CheckBox cbParallel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbYoung;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbOld;
        private System.Windows.Forms.Button btnCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbGName;
        private System.Windows.Forms.Button btnCalcQfs;
        private System.Windows.Forms.Button btnRecalc;
    }
}