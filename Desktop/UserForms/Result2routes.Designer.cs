#if FULL
namespace ClimbingCompetition
{
    partial class Result2routes
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dg = new System.Windows.Forms.DataGridView();
            this.xlExport = new System.Windows.Forms.Button();
            this.cbRound = new System.Windows.Forms.ComboBox();
            this.btnNxtRound = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rbPts = new System.Windows.Forms.RadioButton();
            this.rbPl = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            this.dg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.Size = new System.Drawing.Size(824, 423);
            this.dg.TabIndex = 0;
            // 
            // xlExport
            // 
            this.xlExport.Location = new System.Drawing.Point(0, 3);
            this.xlExport.Name = "xlExport";
            this.xlExport.Size = new System.Drawing.Size(97, 43);
            this.xlExport.TabIndex = 1;
            this.xlExport.Text = "Экспорт в Excel";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
            // 
            // cbRound
            // 
            this.cbRound.FormattingEnabled = true;
            this.cbRound.Items.AddRange(new object[] {
            "1/4 финала",
            "1/2 финала",
            "Финал",
            "Суперфинал"});
            this.cbRound.Location = new System.Drawing.Point(206, 7);
            this.cbRound.Name = "cbRound";
            this.cbRound.Size = new System.Drawing.Size(142, 21);
            this.cbRound.TabIndex = 13;
            this.cbRound.Text = "Раунд";
            // 
            // btnNxtRound
            // 
            this.btnNxtRound.Location = new System.Drawing.Point(103, 3);
            this.btnNxtRound.Name = "btnNxtRound";
            this.btnNxtRound.Size = new System.Drawing.Size(88, 43);
            this.btnNxtRound.TabIndex = 12;
            this.btnNxtRound.Text = "Стартовый на след. раунд";
            this.btnNxtRound.UseVisualStyleBackColor = true;
            this.btnNxtRound.Click += new System.EventHandler(this.btnNxtRound_Click);
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
            this.splitContainer1.Panel2.Controls.Add(this.rbPts);
            this.splitContainer1.Panel2.Controls.Add(this.rbPl);
            this.splitContainer1.Panel2.Controls.Add(this.xlExport);
            this.splitContainer1.Panel2.Controls.Add(this.cbRound);
            this.splitContainer1.Panel2.Controls.Add(this.btnNxtRound);
            this.splitContainer1.Size = new System.Drawing.Size(824, 479);
            this.splitContainer1.SplitterDistance = 423;
            this.splitContainer1.TabIndex = 14;
            // 
            // rbPts
            // 
            this.rbPts.AutoSize = true;
            this.rbPts.Checked = true;
            this.rbPts.Location = new System.Drawing.Point(509, 11);
            this.rbPts.Name = "rbPts";
            this.rbPts.Size = new System.Drawing.Size(265, 17);
            this.rbPts.TabIndex = 14;
            this.rbPts.TabStop = true;
            this.rbPts.Text = "Сведение по среднему арифметическому мест";
            this.rbPts.UseVisualStyleBackColor = true;
            // 
            // rbPl
            // 
            this.rbPl.AutoSize = true;
            this.rbPl.Location = new System.Drawing.Point(372, 10);
            this.rbPl.Name = "rbPl";
            this.rbPl.Size = new System.Drawing.Size(131, 17);
            this.rbPl.TabIndex = 14;
            this.rbPl.Text = "Сведение по местам";
            this.rbPl.UseVisualStyleBackColor = true;
            this.rbPl.CheckedChanged += new System.EventHandler(this.rbPl_CheckedChanged);
            // 
            // Result2routes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 479);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Result2routes";
            this.Text = "Результаты квалификации";
            this.Load += new System.EventHandler(this.Result2routes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.ComboBox cbRound;
        private System.Windows.Forms.Button btnNxtRound;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton rbPl;
        private System.Windows.Forms.RadioButton rbPts;
    }
}
#endif