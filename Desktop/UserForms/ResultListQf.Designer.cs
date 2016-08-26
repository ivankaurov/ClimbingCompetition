#if FULL
namespace ClimbingCompetition
{
    partial class ResultListQf
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
            this.button1 = new System.Windows.Forms.Button();
            this.xlExport = new System.Windows.Forms.Button();
            this.btnNxtRound = new System.Windows.Forms.Button();
            this.cbRound = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
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
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            this.dg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.Size = new System.Drawing.Size(822, 350);
            this.dg.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 43);
            this.button1.TabIndex = 1;
            this.button1.Text = "Обновить данные";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // xlExport
            // 
            this.xlExport.Location = new System.Drawing.Point(101, 3);
            this.xlExport.Name = "xlExport";
            this.xlExport.Size = new System.Drawing.Size(83, 43);
            this.xlExport.TabIndex = 1;
            this.xlExport.Text = "Экспорт в Excel";
            this.xlExport.UseVisualStyleBackColor = true;
            this.xlExport.Click += new System.EventHandler(this.xlExport_Click);
            // 
            // btnNxtRound
            // 
            this.btnNxtRound.Location = new System.Drawing.Point(190, 3);
            this.btnNxtRound.Name = "btnNxtRound";
            this.btnNxtRound.Size = new System.Drawing.Size(88, 43);
            this.btnNxtRound.TabIndex = 10;
            this.btnNxtRound.Text = "Стартовый на след. раунд";
            this.btnNxtRound.UseVisualStyleBackColor = true;
            this.btnNxtRound.Click += new System.EventHandler(this.btnNxtRound_Click);
            // 
            // cbRound
            // 
            this.cbRound.FormattingEnabled = true;
            this.cbRound.Items.AddRange(new object[] {
            "1/4 финала",
            "1/2 финала",
            "Финал",
            "Суперфинал"});
            this.cbRound.Location = new System.Drawing.Point(293, 7);
            this.cbRound.Name = "cbRound";
            this.cbRound.Size = new System.Drawing.Size(142, 21);
            this.cbRound.TabIndex = 11;
            this.cbRound.Text = "Раунд";
            this.cbRound.SelectedIndexChanged += new System.EventHandler(this.cbRound_SelectedIndexChanged);
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
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.cbRound);
            this.splitContainer1.Panel2.Controls.Add(this.xlExport);
            this.splitContainer1.Panel2.Controls.Add(this.btnNxtRound);
            this.splitContainer1.Size = new System.Drawing.Size(822, 410);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.TabIndex = 12;
            // 
            // ResultListQf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 410);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ResultListQf";
            this.Text = "Результаты квалификации";
            this.Load += new System.EventHandler(this.ResultListQf_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button xlExport;
        private System.Windows.Forms.Button btnNxtRound;
        private System.Windows.Forms.ComboBox cbRound;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
#endif