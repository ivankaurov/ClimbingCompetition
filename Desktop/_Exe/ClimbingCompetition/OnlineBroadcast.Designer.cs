namespace ClimbingCompetition
{
    partial class OnlineBroadcast
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dg = new System.Windows.Forms.DataGridView();
            this.cbLoadJudges = new System.Windows.Forms.CheckBox();
            this.cbLoadInDiffThread = new System.Windows.Forms.CheckBox();
            this.cbLoadPhoto = new System.Windows.Forms.CheckBox();
            this.btnLoadOneList = new System.Windows.Forms.Button();
            this.btnReloadLists = new System.Windows.Forms.Button();
            this.btnLoadClimbers = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.dg);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnLoadData);
            this.splitContainer1.Panel2.Controls.Add(this.cbLoadJudges);
            this.splitContainer1.Panel2.Controls.Add(this.cbLoadInDiffThread);
            this.splitContainer1.Panel2.Controls.Add(this.cbLoadPhoto);
            this.splitContainer1.Panel2.Controls.Add(this.btnLoadOneList);
            this.splitContainer1.Panel2.Controls.Add(this.btnReloadLists);
            this.splitContainer1.Panel2.Controls.Add(this.btnLoadClimbers);
            this.splitContainer1.Panel2.Controls.Add(this.btnRefresh);
            this.splitContainer1.Size = new System.Drawing.Size(688, 372);
            this.splitContainer1.SplitterDistance = 265;
            this.splitContainer1.TabIndex = 0;
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            this.dg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.Size = new System.Drawing.Size(688, 265);
            this.dg.TabIndex = 0;
            // 
            // cbLoadJudges
            // 
            this.cbLoadJudges.AutoSize = true;
            this.cbLoadJudges.Location = new System.Drawing.Point(492, 68);
            this.cbLoadJudges.Name = "cbLoadJudges";
            this.cbLoadJudges.Size = new System.Drawing.Size(184, 17);
            this.cbLoadJudges.TabIndex = 4;
            this.cbLoadJudges.Text = "Также загрузить список судей";
            this.cbLoadJudges.UseVisualStyleBackColor = true;
            // 
            // cbLoadInDiffThread
            // 
            this.cbLoadInDiffThread.AutoSize = true;
            this.cbLoadInDiffThread.Location = new System.Drawing.Point(256, 67);
            this.cbLoadInDiffThread.Name = "cbLoadInDiffThread";
            this.cbLoadInDiffThread.Size = new System.Drawing.Size(234, 17);
            this.cbLoadInDiffThread.TabIndex = 3;
            this.cbLoadInDiffThread.Text = "Загрузка участников в фоновом режиме";
            this.cbLoadInDiffThread.UseVisualStyleBackColor = true;
            // 
            // cbLoadPhoto
            // 
            this.cbLoadPhoto.AutoSize = true;
            this.cbLoadPhoto.Location = new System.Drawing.Point(12, 68);
            this.cbLoadPhoto.Name = "cbLoadPhoto";
            this.cbLoadPhoto.Size = new System.Drawing.Size(238, 17);
            this.cbLoadPhoto.TabIndex = 3;
            this.cbLoadPhoto.Text = "Также загрузить фотографии участников";
            this.cbLoadPhoto.UseVisualStyleBackColor = true;
            // 
            // btnLoadOneList
            // 
            this.btnLoadOneList.Location = new System.Drawing.Point(363, 4);
            this.btnLoadOneList.Name = "btnLoadOneList";
            this.btnLoadOneList.Size = new System.Drawing.Size(102, 57);
            this.btnLoadOneList.TabIndex = 2;
            this.btnLoadOneList.Text = "Загрузить протокол";
            this.btnLoadOneList.UseVisualStyleBackColor = true;
            this.btnLoadOneList.Click += new System.EventHandler(this.btnLoadOneList_Click);
            // 
            // btnReloadLists
            // 
            this.btnReloadLists.Location = new System.Drawing.Point(248, 2);
            this.btnReloadLists.Name = "btnReloadLists";
            this.btnReloadLists.Size = new System.Drawing.Size(108, 59);
            this.btnReloadLists.TabIndex = 1;
            this.btnReloadLists.Text = "Перегрузить протоколы на сайте";
            this.btnReloadLists.UseVisualStyleBackColor = true;
            this.btnReloadLists.Click += new System.EventHandler(this.btnReloadLists_Click);
            // 
            // btnLoadClimbers
            // 
            this.btnLoadClimbers.Location = new System.Drawing.Point(134, 2);
            this.btnLoadClimbers.Name = "btnLoadClimbers";
            this.btnLoadClimbers.Size = new System.Drawing.Size(108, 59);
            this.btnLoadClimbers.TabIndex = 1;
            this.btnLoadClimbers.Text = "Загрузить список участников";
            this.btnLoadClimbers.UseVisualStyleBackColor = true;
            this.btnLoadClimbers.Click += new System.EventHandler(this.btnLoadClimbers_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(116, 59);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Загрузить протоколы для прямой трансляции";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(472, 4);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(116, 57);
            this.btnLoadData.TabIndex = 5;
            this.btnLoadData.Text = "Загрузка файлов (положение и т.п.)";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // OnlineBroadcast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 372);
            this.Controls.Add(this.splitContainer1);
            this.Name = "OnlineBroadcast";
            this.Text = "Online трансляция";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnLoadClimbers;
        private System.Windows.Forms.Button btnReloadLists;
        private System.Windows.Forms.Button btnLoadOneList;
        private System.Windows.Forms.CheckBox cbLoadInDiffThread;
        private System.Windows.Forms.CheckBox cbLoadPhoto;
        private System.Windows.Forms.CheckBox cbLoadJudges;
        private System.Windows.Forms.Button btnLoadData;
    }
}