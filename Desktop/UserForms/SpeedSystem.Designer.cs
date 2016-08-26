namespace ClimbingCompetition
{
    partial class SpeedSystem
    {
        private void InitializeComponent()
        {
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbSystem = new System.Windows.Forms.RadioButton();
            this.rbRoute1 = new System.Windows.Forms.RadioButton();
            this.rbBothRoutes = new System.Windows.Forms.RadioButton();
            this.rbRoute2 = new System.Windows.Forms.RadioButton();
            this.btnNext = new System.Windows.Forms.Button();
            this.cbNxtRound = new System.Windows.Forms.ComboBox();
            this.btnRestoreSecond = new System.Windows.Forms.Button();
            this.btnRollBackSecond = new System.Windows.Forms.Button();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbWorkMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRes)).BeginInit();
            this.gbMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(397, 130);
            // 
            // splitContainer2
            // 
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnRestoreSecond);
            this.splitContainer2.Panel2.Controls.Add(this.btnRollBackSecond);
            this.splitContainer2.Panel2.Controls.Add(this.cbNxtRound);
            this.splitContainer2.Panel2.Controls.Add(this.btnNext);
            this.splitContainer2.Panel2.Controls.Add(this.gbMode);
            this.splitContainer2.Size = new System.Drawing.Size(935, 379);
            this.splitContainer2.SplitterDistance = 60;
            // 
            // splitContainer1
            // 
            this.splitContainer1.SplitterDistance = 195;
            // 
            // leftSum
            // 
            this.leftSum.Enabled = false;
            // 
            // leftRoute2
            // 
            this.leftRoute2.Enabled = false;
            // 
            // cbPrevRound
            // 
            this.cbPrevRound.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(397, 165);
            this.btnCancel.Size = new System.Drawing.Size(98, 33);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(651, 76);
            // 
            // rightSum
            // 
            this.rightSum.Location = new System.Drawing.Point(714, 72);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(651, 28);
            // 
            // rightRoute2
            // 
            this.rightRoute2.Location = new System.Drawing.Point(714, 24);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(651, 9);
            // 
            // rightRoute1
            // 
            this.rightRoute1.Enabled = false;
            this.rightRoute1.Location = new System.Drawing.Point(714, 5);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(298, 117);
            // 
            // btnRollBack
            // 
            this.btnRollBack.Location = new System.Drawing.Point(268, 117);
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbSystem);
            this.gbMode.Controls.Add(this.rbRoute1);
            this.gbMode.Controls.Add(this.rbBothRoutes);
            this.gbMode.Controls.Add(this.rbRoute2);
            this.gbMode.Location = new System.Drawing.Point(738, 136);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(158, 120);
            this.gbMode.TabIndex = 35;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "Режим ввода результатов";
            // 
            // rbSystem
            // 
            this.rbSystem.AutoSize = true;
            this.rbSystem.Checked = true;
            this.rbSystem.Location = new System.Drawing.Point(6, 88);
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
            this.rbRoute1.Location = new System.Drawing.Point(6, 19);
            this.rbRoute1.Name = "rbRoute1";
            this.rbRoute1.Size = new System.Drawing.Size(71, 17);
            this.rbRoute1.TabIndex = 0;
            this.rbRoute1.Text = "Трасса 1";
            this.rbRoute1.UseVisualStyleBackColor = true;
            // 
            // rbBothRoutes
            // 
            this.rbBothRoutes.AutoSize = true;
            this.rbBothRoutes.Location = new System.Drawing.Point(6, 65);
            this.rbBothRoutes.Name = "rbBothRoutes";
            this.rbBothRoutes.Size = new System.Drawing.Size(132, 17);
            this.rbBothRoutes.TabIndex = 0;
            this.rbBothRoutes.Text = "Обе трассы / правка";
            this.rbBothRoutes.UseVisualStyleBackColor = true;
            // 
            // rbRoute2
            // 
            this.rbRoute2.AutoSize = true;
            this.rbRoute2.Location = new System.Drawing.Point(6, 42);
            this.rbRoute2.Name = "rbRoute2";
            this.rbRoute2.Size = new System.Drawing.Size(71, 17);
            this.rbRoute2.TabIndex = 0;
            this.rbRoute2.Text = "Трасса 2";
            this.rbRoute2.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(501, 130);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(128, 31);
            this.btnNext.TabIndex = 36;
            this.btnNext.Text = "Следующий участник";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // cbNxtRound
            // 
            this.cbNxtRound.FormattingEnabled = true;
            this.cbNxtRound.Items.AddRange(new object[] {
            "Квалификация 2",
            "Парная гонка"});
            this.cbNxtRound.Location = new System.Drawing.Point(109, 47);
            this.cbNxtRound.Name = "cbNxtRound";
            this.cbNxtRound.Size = new System.Drawing.Size(121, 21);
            this.cbNxtRound.TabIndex = 52;
            this.cbNxtRound.Text = "Выберите раунд";
            this.cbNxtRound.SelectedIndexChanged += new System.EventHandler(this.cbNxtRound_SelectedIndexChanged);
            // 
            // btnRestoreSecond
            // 
            this.btnRestoreSecond.BackColor = System.Drawing.SystemColors.Window;
            this.btnRestoreSecond.Enabled = false;
            this.btnRestoreSecond.Image = global::ClimbingCompetition.Properties.Resources.restore;
            this.btnRestoreSecond.Location = new System.Drawing.Point(744, 100);
            this.btnRestoreSecond.Name = "btnRestoreSecond";
            this.btnRestoreSecond.Size = new System.Drawing.Size(24, 23);
            this.btnRestoreSecond.TabIndex = 58;
            this.btnRestoreSecond.UseVisualStyleBackColor = false;
            this.btnRestoreSecond.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // btnRollBackSecond
            // 
            this.btnRollBackSecond.BackColor = System.Drawing.SystemColors.Window;
            this.btnRollBackSecond.Enabled = false;
            this.btnRollBackSecond.Image = global::ClimbingCompetition.Properties.Resources.rollback;
            this.btnRollBackSecond.Location = new System.Drawing.Point(714, 100);
            this.btnRollBackSecond.Name = "btnRollBackSecond";
            this.btnRollBackSecond.Size = new System.Drawing.Size(24, 23);
            this.btnRollBackSecond.TabIndex = 57;
            this.btnRollBackSecond.UseVisualStyleBackColor = false;
            this.btnRollBackSecond.Click += new System.EventHandler(this.btnRollBack_Click);
            // 
            // SpeedSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(935, 578);
            this.Name = "SpeedSystem";
            this.Load += new System.EventHandler(this.SpeedSystem_Load);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gbWorkMode.ResumeLayout(false);
            this.gbWorkMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRes)).EndInit();
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox gbMode;
        private System.Windows.Forms.RadioButton rbSystem;
        private System.Windows.Forms.RadioButton rbRoute1;
        private System.Windows.Forms.RadioButton rbBothRoutes;
        private System.Windows.Forms.RadioButton rbRoute2;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ComboBox cbNxtRound;
        protected System.Windows.Forms.Button btnRestoreSecond;
        protected System.Windows.Forms.Button btnRollBackSecond;
    }
}