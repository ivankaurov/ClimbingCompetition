namespace ClimbingCompetition
{
    partial class StartList
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbPreQf = new System.Windows.Forms.CheckBox();
            this.cbPrintTime = new System.Windows.Forms.CheckBox();
            this.cbTechnical = new System.Windows.Forms.CheckBox();
            this.cbPrev = new System.Windows.Forms.CheckBox();
            this.cbRanking = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsPushFront = new System.Windows.Forms.ToolStripButton();
            this.tsPushMiddle = new System.Windows.Forms.ToolStripButton();
            this.tsPushBack = new System.Windows.Forms.ToolStripButton();
            this.tsDelClimber = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsRefreshStartNumbers = new System.Windows.Forms.ToolStripButton();
            this.tsReCreate2ndRoute = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsExcelExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
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
            this.dg.Size = new System.Drawing.Size(601, 433);
            this.dg.TabIndex = 0;
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
            this.splitContainer1.Panel2.Controls.Add(this.cbPreQf);
            this.splitContainer1.Panel2.Controls.Add(this.cbPrintTime);
            this.splitContainer1.Panel2.Controls.Add(this.cbTechnical);
            this.splitContainer1.Panel2.Controls.Add(this.cbPrev);
            this.splitContainer1.Panel2.Controls.Add(this.cbRanking);
            this.splitContainer1.Size = new System.Drawing.Size(601, 484);
            this.splitContainer1.SplitterDistance = 433;
            this.splitContainer1.TabIndex = 3;
            // 
            // cbPreQf
            // 
            this.cbPreQf.AutoSize = true;
            this.cbPreQf.Location = new System.Drawing.Point(3, 26);
            this.cbPreQf.Name = "cbPreQf";
            this.cbPreQf.Size = new System.Drawing.Size(275, 17);
            this.cbPreQf.TabIndex = 7;
            this.cbPreQf.Text = "Показывать преквалифицированных участников";
            this.cbPreQf.UseVisualStyleBackColor = true;
            this.cbPreQf.CheckedChanged += new System.EventHandler(this.cbPrev_CheckedChanged);
            // 
            // cbPrintTime
            // 
            this.cbPrintTime.AutoSize = true;
            this.cbPrintTime.Location = new System.Drawing.Point(322, 26);
            this.cbPrintTime.Name = "cbPrintTime";
            this.cbPrintTime.Size = new System.Drawing.Size(189, 17);
            this.cbPrintTime.TabIndex = 6;
            this.cbPrintTime.Text = "Печать данных о зоне изоляции";
            this.cbPrintTime.UseVisualStyleBackColor = true;
            // 
            // cbTechnical
            // 
            this.cbTechnical.AutoSize = true;
            this.cbTechnical.Location = new System.Drawing.Point(322, 3);
            this.cbTechnical.Name = "cbTechnical";
            this.cbTechnical.Size = new System.Drawing.Size(166, 17);
            this.cbTechnical.TabIndex = 5;
            this.cbTechnical.Text = "Печать служебных колонок";
            this.cbTechnical.UseVisualStyleBackColor = true;
            // 
            // cbPrev
            // 
            this.cbPrev.AutoSize = true;
            this.cbPrev.Location = new System.Drawing.Point(141, 3);
            this.cbPrev.Name = "cbPrev";
            this.cbPrev.Size = new System.Drawing.Size(175, 17);
            this.cbPrev.TabIndex = 4;
            this.cbPrev.Text = "Показывать предыдущий тур";
            this.cbPrev.UseVisualStyleBackColor = true;
            this.cbPrev.CheckedChanged += new System.EventHandler(this.cbPrev_CheckedChanged);
            // 
            // cbRanking
            // 
            this.cbRanking.AutoSize = true;
            this.cbRanking.Location = new System.Drawing.Point(3, 3);
            this.cbRanking.Name = "cbRanking";
            this.cbRanking.Size = new System.Drawing.Size(132, 17);
            this.cbRanking.TabIndex = 3;
            this.cbRanking.Text = "Показывать рейтинг";
            this.cbRanking.UseVisualStyleBackColor = true;
            this.cbRanking.CheckedChanged += new System.EventHandler(this.cbPrev_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPushFront,
            this.tsPushMiddle,
            this.tsPushBack,
            this.tsDelClimber,
            this.toolStripSeparator1,
            this.tsRefreshStartNumbers,
            this.tsReCreate2ndRoute,
            this.toolStripSeparator2,
            this.tsExcelExport});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(274, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsPushFront
            // 
            this.tsPushFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPushFront.Image = global::ClimbingCompetition.Properties.Resources.PushFront;
            this.tsPushFront.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPushFront.Name = "tsPushFront";
            this.tsPushFront.Size = new System.Drawing.Size(36, 36);
            this.tsPushFront.Text = "Добавить участника в начало";
            this.tsPushFront.Click += new System.EventHandler(this.pushFrontToolStripMenuItem_Click);
            // 
            // tsPushMiddle
            // 
            this.tsPushMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPushMiddle.Image = global::ClimbingCompetition.Properties.Resources.PushMiddle;
            this.tsPushMiddle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPushMiddle.Name = "tsPushMiddle";
            this.tsPushMiddle.Size = new System.Drawing.Size(36, 36);
            this.tsPushMiddle.Text = "Добавить участника на заданную позицию";
            this.tsPushMiddle.Click += new System.EventHandler(this.addToPosToolStripMenuItem_Click);
            // 
            // tsPushBack
            // 
            this.tsPushBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPushBack.Image = global::ClimbingCompetition.Properties.Resources.PushBack;
            this.tsPushBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPushBack.Name = "tsPushBack";
            this.tsPushBack.Size = new System.Drawing.Size(36, 36);
            this.tsPushBack.Text = "Добавить участника в конец протокола";
            this.tsPushBack.Click += new System.EventHandler(this.pushBackToolStripMenuItem_Click);
            // 
            // tsDelClimber
            // 
            this.tsDelClimber.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDelClimber.Image = global::ClimbingCompetition.Properties.Resources.DelImage;
            this.tsDelClimber.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDelClimber.Name = "tsDelClimber";
            this.tsDelClimber.Size = new System.Drawing.Size(36, 36);
            this.tsDelClimber.Text = "Удалить участника";
            this.tsDelClimber.Click += new System.EventHandler(this.btnDelClimber_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // tsRefreshStartNumbers
            // 
            this.tsRefreshStartNumbers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRefreshStartNumbers.Image = global::ClimbingCompetition.Properties.Resources.FileSave;
            this.tsRefreshStartNumbers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRefreshStartNumbers.Name = "tsRefreshStartNumbers";
            this.tsRefreshStartNumbers.Size = new System.Drawing.Size(36, 36);
            this.tsRefreshStartNumbers.Text = "Сохранить и упорядочить стартовые номера";
            this.tsRefreshStartNumbers.Click += new System.EventHandler(this.btnRefreshStartNumbers_Click);
            // 
            // tsReCreate2ndRoute
            // 
            this.tsReCreate2ndRoute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsReCreate2ndRoute.Image = global::ClimbingCompetition.Properties.Resources.SecondRoute;
            this.tsReCreate2ndRoute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsReCreate2ndRoute.Name = "tsReCreate2ndRoute";
            this.tsReCreate2ndRoute.Size = new System.Drawing.Size(36, 36);
            this.tsReCreate2ndRoute.Text = "Переформировать стартовый протокол на другую трассу квалификации";
            this.tsReCreate2ndRoute.Click += new System.EventHandler(this.btnReCreate2ndRoute_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // tsExcelExport
            // 
            this.tsExcelExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsExcelExport.Image = global::ClimbingCompetition.Properties.Resources.ExcelImage;
            this.tsExcelExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsExcelExport.Name = "tsExcelExport";
            this.tsExcelExport.Size = new System.Drawing.Size(36, 36);
            this.tsExcelExport.Text = "Экспорт в Excel";
            this.tsExcelExport.Click += new System.EventHandler(this.btnExcelExport_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(601, 484);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(601, 523);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // StartList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 523);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "StartList";
            this.Text = "StartList";
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox cbPrev;
        private System.Windows.Forms.CheckBox cbRanking;
        private System.Windows.Forms.CheckBox cbTechnical;
        private System.Windows.Forms.CheckBox cbPrintTime;
        private System.Windows.Forms.CheckBox cbPreQf;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsPushFront;
        private System.Windows.Forms.ToolStripButton tsPushMiddle;
        private System.Windows.Forms.ToolStripButton tsPushBack;
        private System.Windows.Forms.ToolStripButton tsDelClimber;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsRefreshStartNumbers;
        private System.Windows.Forms.ToolStripButton tsReCreate2ndRoute;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsExcelExport;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    }
}