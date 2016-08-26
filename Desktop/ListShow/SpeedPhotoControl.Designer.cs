namespace ListShow
{
    partial class SpeedPhotoControl
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
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.spcLeft = new System.Windows.Forms.SplitContainer();
            this.spcLeftBottom = new System.Windows.Forms.SplitContainer();
            this.spcRight = new System.Windows.Forms.SplitContainer();
            this.namesPanelRight = new System.Windows.Forms.Panel();
            this.tbRightGroup = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRightAge = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbRightSurname = new System.Windows.Forms.Label();
            this.tbRightNum = new System.Windows.Forms.Label();
            this.tbRightQf = new System.Windows.Forms.Label();
            this.tbRightTeam = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.spcRightBottom = new System.Windows.Forms.SplitContainer();
            this.listBoxRight = new System.Windows.Forms.ListView();
            this.pbRightPhoto = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
            this.namesPanel.SuspendLayout();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.spcLeft.Panel2.SuspendLayout();
            this.spcLeft.SuspendLayout();
            this.spcLeftBottom.SuspendLayout();
            this.spcRight.Panel1.SuspendLayout();
            this.spcRight.Panel2.SuspendLayout();
            this.spcRight.SuspendLayout();
            this.namesPanelRight.SuspendLayout();
            this.spcRightBottom.Panel1.SuspendLayout();
            this.spcRightBottom.Panel2.SuspendLayout();
            this.spcRightBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPhoto
            // 
            this.pbPhoto.BackColor = System.Drawing.SystemColors.Control;
            // 
            // namesPanel
            // 
            this.namesPanel.BackColor = System.Drawing.SystemColors.Control;
            // 
            // spcMain
            // 
            this.spcMain.Location = new System.Drawing.Point(361, 3);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.spcLeft);
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.spcRight);
            this.spcMain.Size = new System.Drawing.Size(517, 602);
            this.spcMain.SplitterDistance = 255;
            this.spcMain.TabIndex = 42;
            this.spcMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcMain_SplitterMoved);
            // 
            // spcLeft
            // 
            this.spcLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcLeft.Location = new System.Drawing.Point(0, 0);
            this.spcLeft.Name = "spcLeft";
            this.spcLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcLeft.Panel2
            // 
            this.spcLeft.Panel2.Controls.Add(this.spcLeftBottom);
            this.spcLeft.Size = new System.Drawing.Size(255, 602);
            this.spcLeft.SplitterDistance = 225;
            this.spcLeft.TabIndex = 0;
            this.spcLeft.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcLeft_SplitterMoved);
            // 
            // spcLeftBottom
            // 
            this.spcLeftBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcLeftBottom.Location = new System.Drawing.Point(0, 0);
            this.spcLeftBottom.Name = "spcLeftBottom";
            this.spcLeftBottom.Size = new System.Drawing.Size(255, 373);
            this.spcLeftBottom.SplitterDistance = 125;
            this.spcLeftBottom.TabIndex = 0;
            this.spcLeftBottom.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcLeftBottom_SplitterMoved);
            // 
            // spcRight
            // 
            this.spcRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcRight.Location = new System.Drawing.Point(0, 0);
            this.spcRight.Name = "spcRight";
            this.spcRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcRight.Panel1
            // 
            this.spcRight.Panel1.Controls.Add(this.namesPanelRight);
            // 
            // spcRight.Panel2
            // 
            this.spcRight.Panel2.Controls.Add(this.spcRightBottom);
            this.spcRight.Size = new System.Drawing.Size(258, 602);
            this.spcRight.SplitterDistance = 225;
            this.spcRight.TabIndex = 1;
            this.spcRight.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcLeft_SplitterMoved);
            // 
            // namesPanelRight
            // 
            this.namesPanelRight.BackColor = System.Drawing.SystemColors.Control;
            this.namesPanelRight.Controls.Add(this.tbRightGroup);
            this.namesPanelRight.Controls.Add(this.label2);
            this.namesPanelRight.Controls.Add(this.label3);
            this.namesPanelRight.Controls.Add(this.tbRightAge);
            this.namesPanelRight.Controls.Add(this.label8);
            this.namesPanelRight.Controls.Add(this.label9);
            this.namesPanelRight.Controls.Add(this.label11);
            this.namesPanelRight.Controls.Add(this.tbRightSurname);
            this.namesPanelRight.Controls.Add(this.tbRightNum);
            this.namesPanelRight.Controls.Add(this.tbRightQf);
            this.namesPanelRight.Controls.Add(this.tbRightTeam);
            this.namesPanelRight.Controls.Add(this.label18);
            this.namesPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.namesPanelRight.Location = new System.Drawing.Point(0, 0);
            this.namesPanelRight.Name = "namesPanelRight";
            this.namesPanelRight.Size = new System.Drawing.Size(258, 225);
            this.namesPanelRight.TabIndex = 42;
            // 
            // tbRightGroup
            // 
            this.tbRightGroup.AutoSize = true;
            this.tbRightGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightGroup.Location = new System.Drawing.Point(126, 46);
            this.tbRightGroup.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightGroup.Name = "tbRightGroup";
            this.tbRightGroup.Size = new System.Drawing.Size(60, 24);
            this.tbRightGroup.TabIndex = 29;
            this.tbRightGroup.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(63, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 24);
            this.label2.TabIndex = 35;
            this.label2.Text = "Имя:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(36, 46);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 24);
            this.label3.TabIndex = 38;
            this.label3.Text = "Группа:";
            // 
            // tbRightAge
            // 
            this.tbRightAge.AutoSize = true;
            this.tbRightAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightAge.Location = new System.Drawing.Point(123, 113);
            this.tbRightAge.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightAge.Name = "tbRightAge";
            this.tbRightAge.Size = new System.Drawing.Size(48, 24);
            this.tbRightAge.TabIndex = 30;
            this.tbRightAge.Text = "num";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(67, 113);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 24);
            this.label8.TabIndex = 33;
            this.label8.Text = "Г.р.:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(20, 186);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 24);
            this.label9.TabIndex = 36;
            this.label9.Text = "Команда:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(40, 79);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(74, 24);
            this.label11.TabIndex = 32;
            this.label11.Text = "Номер:";
            // 
            // tbRightSurname
            // 
            this.tbRightSurname.AutoSize = true;
            this.tbRightSurname.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightSurname.Location = new System.Drawing.Point(126, 13);
            this.tbRightSurname.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightSurname.Name = "tbRightSurname";
            this.tbRightSurname.Size = new System.Drawing.Size(48, 24);
            this.tbRightSurname.TabIndex = 30;
            this.tbRightSurname.Text = "num";
            // 
            // tbRightNum
            // 
            this.tbRightNum.AutoSize = true;
            this.tbRightNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightNum.Location = new System.Drawing.Point(126, 79);
            this.tbRightNum.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightNum.Name = "tbRightNum";
            this.tbRightNum.Size = new System.Drawing.Size(48, 24);
            this.tbRightNum.TabIndex = 30;
            this.tbRightNum.Text = "num";
            // 
            // tbRightQf
            // 
            this.tbRightQf.AutoSize = true;
            this.tbRightQf.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightQf.Location = new System.Drawing.Point(126, 151);
            this.tbRightQf.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightQf.Name = "tbRightQf";
            this.tbRightQf.Size = new System.Drawing.Size(48, 24);
            this.tbRightQf.TabIndex = 30;
            this.tbRightQf.Text = "num";
            // 
            // tbRightTeam
            // 
            this.tbRightTeam.AutoSize = true;
            this.tbRightTeam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbRightTeam.Location = new System.Drawing.Point(123, 186);
            this.tbRightTeam.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tbRightTeam.Name = "tbRightTeam";
            this.tbRightTeam.Size = new System.Drawing.Size(51, 24);
            this.tbRightTeam.TabIndex = 28;
            this.tbRightTeam.Text = "team";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new System.Drawing.Point(34, 151);
            this.label18.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(80, 24);
            this.label18.TabIndex = 34;
            this.label18.Text = "Разряд:";
            // 
            // spcRightBottom
            // 
            this.spcRightBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcRightBottom.Location = new System.Drawing.Point(0, 0);
            this.spcRightBottom.Name = "spcRightBottom";
            // 
            // spcRightBottom.Panel1
            // 
            this.spcRightBottom.Panel1.Controls.Add(this.listBoxRight);
            // 
            // spcRightBottom.Panel2
            // 
            this.spcRightBottom.Panel2.Controls.Add(this.pbRightPhoto);
            this.spcRightBottom.Size = new System.Drawing.Size(258, 373);
            this.spcRightBottom.SplitterDistance = 125;
            this.spcRightBottom.TabIndex = 0;
            this.spcRightBottom.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcLeftBottom_SplitterMoved);
            // 
            // listBoxRight
            // 
            this.listBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxRight.GridLines = true;
            this.listBoxRight.Location = new System.Drawing.Point(0, 0);
            this.listBoxRight.Name = "listBoxRight";
            this.listBoxRight.Size = new System.Drawing.Size(125, 373);
            this.listBoxRight.TabIndex = 1;
            this.listBoxRight.UseCompatibleStateImageBehavior = false;
            // 
            // pbRightPhoto
            // 
            this.pbRightPhoto.BackColor = System.Drawing.SystemColors.Control;
            this.pbRightPhoto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbRightPhoto.Location = new System.Drawing.Point(0, 0);
            this.pbRightPhoto.Name = "pbRightPhoto";
            this.pbRightPhoto.Size = new System.Drawing.Size(129, 373);
            this.pbRightPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRightPhoto.TabIndex = 1;
            this.pbRightPhoto.TabStop = false;
            // 
            // SpeedPhotoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClimbingStyle = ListShow.BasePhotoControl.ClimbingStyleType.Скорость;
            this.Controls.Add(this.spcMain);
            this.Name = "SpeedPhotoControl";
            this.Controls.SetChildIndex(this.spcMain, 0);
            this.Controls.SetChildIndex(this.pbPhoto, 0);
            this.Controls.SetChildIndex(this.listBox1, 0);
            this.Controls.SetChildIndex(this.namesPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
            this.namesPanel.ResumeLayout(false);
            this.namesPanel.PerformLayout();
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            this.spcMain.ResumeLayout(false);
            this.spcLeft.Panel2.ResumeLayout(false);
            this.spcLeft.ResumeLayout(false);
            this.spcLeftBottom.ResumeLayout(false);
            this.spcRight.Panel1.ResumeLayout(false);
            this.spcRight.Panel2.ResumeLayout(false);
            this.spcRight.ResumeLayout(false);
            this.namesPanelRight.ResumeLayout(false);
            this.namesPanelRight.PerformLayout();
            this.spcRightBottom.Panel1.ResumeLayout(false);
            this.spcRightBottom.Panel2.ResumeLayout(false);
            this.spcRightBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRightPhoto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.SplitContainer spcLeft;
        private System.Windows.Forms.SplitContainer spcLeftBottom;
        private System.Windows.Forms.SplitContainer spcRight;
        private System.Windows.Forms.SplitContainer spcRightBottom;
        protected System.Windows.Forms.Panel namesPanelRight;
        protected System.Windows.Forms.Label tbRightGroup;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.Label tbRightAge;
        protected System.Windows.Forms.Label label8;
        protected System.Windows.Forms.Label label9;
        protected System.Windows.Forms.Label label11;
        protected System.Windows.Forms.Label tbRightSurname;
        protected System.Windows.Forms.Label tbRightNum;
        protected System.Windows.Forms.Label tbRightQf;
        protected System.Windows.Forms.Label tbRightTeam;
        protected System.Windows.Forms.Label label18;
        protected System.Windows.Forms.PictureBox pbRightPhoto;
        protected System.Windows.Forms.ListView listBoxRight;
    }
}
