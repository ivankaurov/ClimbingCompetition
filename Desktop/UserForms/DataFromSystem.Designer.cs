namespace UserForms
{
    partial class DataFromSystem
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
            this.gbR1 = new System.Windows.Forms.GroupBox();
            this.btnEnterAndSetNext = new System.Windows.Forms.Button();
            this.rbNya1 = new System.Windows.Forms.RadioButton();
            this.gbRT1 = new System.Windows.Forms.GroupBox();
            this.rbS1 = new System.Windows.Forms.RadioButton();
            this.rbR1 = new System.Windows.Forms.RadioButton();
            this.rbDisq1 = new System.Windows.Forms.RadioButton();
            this.rbFall1 = new System.Windows.Forms.RadioButton();
            this.rbTime1 = new System.Windows.Forms.RadioButton();
            this.lblClimber1 = new System.Windows.Forms.Label();
            this.gbR2 = new System.Windows.Forms.GroupBox();
            this.btnSetNext2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbS2 = new System.Windows.Forms.RadioButton();
            this.rbR2 = new System.Windows.Forms.RadioButton();
            this.rbNya2 = new System.Windows.Forms.RadioButton();
            this.rbDisq2 = new System.Windows.Forms.RadioButton();
            this.rbFall2 = new System.Windows.Forms.RadioButton();
            this.rbTime2 = new System.Windows.Forms.RadioButton();
            this.lblClimber2 = new System.Windows.Forms.Label();
            this.btnSaveRes = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTimer = new System.Windows.Forms.Label();
            this.autoCloseTimer = new System.Windows.Forms.Timer(this.components);
            this.btnStopAuto = new System.Windows.Forms.Button();
            this.btnExchange = new System.Windows.Forms.Button();
            this.gbR1.SuspendLayout();
            this.gbRT1.SuspendLayout();
            this.gbR2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbR1
            // 
            this.gbR1.Controls.Add(this.btnEnterAndSetNext);
            this.gbR1.Controls.Add(this.rbNya1);
            this.gbR1.Controls.Add(this.gbRT1);
            this.gbR1.Controls.Add(this.rbDisq1);
            this.gbR1.Controls.Add(this.rbFall1);
            this.gbR1.Controls.Add(this.rbTime1);
            this.gbR1.Controls.Add(this.lblClimber1);
            this.gbR1.Location = new System.Drawing.Point(12, 26);
            this.gbR1.Name = "gbR1";
            this.gbR1.Size = new System.Drawing.Size(201, 323);
            this.gbR1.TabIndex = 0;
            this.gbR1.TabStop = false;
            this.gbR1.Text = "Трасса 1";
            // 
            // btnEnterAndSetNext
            // 
            this.btnEnterAndSetNext.Location = new System.Drawing.Point(104, 110);
            this.btnEnterAndSetNext.Name = "btnEnterAndSetNext";
            this.btnEnterAndSetNext.Size = new System.Drawing.Size(89, 84);
            this.btnEnterAndSetNext.TabIndex = 5;
            this.btnEnterAndSetNext.Text = "Ввести результат и установить следующего участника";
            this.btnEnterAndSetNext.UseVisualStyleBackColor = true;
            this.btnEnterAndSetNext.Click += new System.EventHandler(this.btnEnterAndSetNext_Click);
            // 
            // rbNya1
            // 
            this.rbNya1.AutoSize = true;
            this.rbNya1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbNya1.Location = new System.Drawing.Point(9, 161);
            this.rbNya1.Name = "rbNya1";
            this.rbNya1.Size = new System.Drawing.Size(67, 33);
            this.rbNya1.TabIndex = 3;
            this.rbNya1.Text = "н/я";
            this.rbNya1.UseVisualStyleBackColor = true;
            // 
            // gbRT1
            // 
            this.gbRT1.Controls.Add(this.rbS1);
            this.gbRT1.Controls.Add(this.rbR1);
            this.gbRT1.Location = new System.Drawing.Point(9, 200);
            this.gbRT1.Name = "gbRT1";
            this.gbRT1.Size = new System.Drawing.Size(168, 110);
            this.gbRT1.TabIndex = 4;
            this.gbRT1.TabStop = false;
            // 
            // rbS1
            // 
            this.rbS1.AutoSize = true;
            this.rbS1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbS1.Location = new System.Drawing.Point(6, 58);
            this.rbS1.Name = "rbS1";
            this.rbS1.Size = new System.Drawing.Size(108, 33);
            this.rbS1.TabIndex = 4;
            this.rbS1.Text = "Сумма";
            this.rbS1.UseVisualStyleBackColor = true;
            // 
            // rbR1
            // 
            this.rbR1.AutoSize = true;
            this.rbR1.Checked = true;
            this.rbR1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbR1.Location = new System.Drawing.Point(6, 19);
            this.rbR1.Name = "rbR1";
            this.rbR1.Size = new System.Drawing.Size(130, 33);
            this.rbR1.TabIndex = 4;
            this.rbR1.TabStop = true;
            this.rbR1.Text = "Трасса 1";
            this.rbR1.UseVisualStyleBackColor = true;
            // 
            // rbDisq1
            // 
            this.rbDisq1.AutoSize = true;
            this.rbDisq1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbDisq1.Location = new System.Drawing.Point(9, 122);
            this.rbDisq1.Name = "rbDisq1";
            this.rbDisq1.Size = new System.Drawing.Size(103, 33);
            this.rbDisq1.TabIndex = 3;
            this.rbDisq1.Text = "дискв.";
            this.rbDisq1.UseVisualStyleBackColor = true;
            // 
            // rbFall1
            // 
            this.rbFall1.AutoSize = true;
            this.rbFall1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbFall1.Location = new System.Drawing.Point(9, 83);
            this.rbFall1.Name = "rbFall1";
            this.rbFall1.Size = new System.Drawing.Size(87, 33);
            this.rbFall1.TabIndex = 3;
            this.rbFall1.Text = "срыв";
            this.rbFall1.UseVisualStyleBackColor = true;
            // 
            // rbTime1
            // 
            this.rbTime1.AutoSize = true;
            this.rbTime1.Checked = true;
            this.rbTime1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbTime1.Location = new System.Drawing.Point(9, 36);
            this.rbTime1.Name = "rbTime1";
            this.rbTime1.Size = new System.Drawing.Size(168, 46);
            this.rbTime1.TabIndex = 2;
            this.rbTime1.TabStop = true;
            this.rbTime1.Text = "0:00.00";
            this.rbTime1.UseVisualStyleBackColor = true;
            // 
            // lblClimber1
            // 
            this.lblClimber1.AutoSize = true;
            this.lblClimber1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblClimber1.Location = new System.Drawing.Point(6, 16);
            this.lblClimber1.Name = "lblClimber1";
            this.lblClimber1.Size = new System.Drawing.Size(105, 16);
            this.lblClimber1.TabIndex = 0;
            this.lblClimber1.Text = "Нет участника";
            // 
            // gbR2
            // 
            this.gbR2.Controls.Add(this.btnSetNext2);
            this.gbR2.Controls.Add(this.groupBox1);
            this.gbR2.Controls.Add(this.rbNya2);
            this.gbR2.Controls.Add(this.rbDisq2);
            this.gbR2.Controls.Add(this.rbFall2);
            this.gbR2.Controls.Add(this.rbTime2);
            this.gbR2.Controls.Add(this.lblClimber2);
            this.gbR2.Location = new System.Drawing.Point(232, 26);
            this.gbR2.Name = "gbR2";
            this.gbR2.Size = new System.Drawing.Size(217, 323);
            this.gbR2.TabIndex = 1;
            this.gbR2.TabStop = false;
            this.gbR2.Text = "Трасса 2";
            // 
            // btnSetNext2
            // 
            this.btnSetNext2.Location = new System.Drawing.Point(118, 110);
            this.btnSetNext2.Name = "btnSetNext2";
            this.btnSetNext2.Size = new System.Drawing.Size(89, 84);
            this.btnSetNext2.TabIndex = 6;
            this.btnSetNext2.Text = "Ввести результат и установить следующего участника";
            this.btnSetNext2.UseVisualStyleBackColor = true;
            this.btnSetNext2.Click += new System.EventHandler(this.btnEnterAndSetNext_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbS2);
            this.groupBox1.Controls.Add(this.rbR2);
            this.groupBox1.Location = new System.Drawing.Point(9, 200);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 110);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // rbS2
            // 
            this.rbS2.AutoSize = true;
            this.rbS2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbS2.Location = new System.Drawing.Point(9, 58);
            this.rbS2.Name = "rbS2";
            this.rbS2.Size = new System.Drawing.Size(108, 33);
            this.rbS2.TabIndex = 4;
            this.rbS2.Text = "Сумма";
            this.rbS2.UseVisualStyleBackColor = true;
            // 
            // rbR2
            // 
            this.rbR2.AutoSize = true;
            this.rbR2.Checked = true;
            this.rbR2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbR2.Location = new System.Drawing.Point(9, 19);
            this.rbR2.Name = "rbR2";
            this.rbR2.Size = new System.Drawing.Size(130, 33);
            this.rbR2.TabIndex = 4;
            this.rbR2.TabStop = true;
            this.rbR2.Text = "Трасса 2";
            this.rbR2.UseVisualStyleBackColor = true;
            // 
            // rbNya2
            // 
            this.rbNya2.AutoSize = true;
            this.rbNya2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbNya2.Location = new System.Drawing.Point(9, 161);
            this.rbNya2.Name = "rbNya2";
            this.rbNya2.Size = new System.Drawing.Size(67, 33);
            this.rbNya2.TabIndex = 3;
            this.rbNya2.Text = "н/я";
            this.rbNya2.UseVisualStyleBackColor = true;
            // 
            // rbDisq2
            // 
            this.rbDisq2.AutoSize = true;
            this.rbDisq2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbDisq2.Location = new System.Drawing.Point(9, 122);
            this.rbDisq2.Name = "rbDisq2";
            this.rbDisq2.Size = new System.Drawing.Size(103, 33);
            this.rbDisq2.TabIndex = 3;
            this.rbDisq2.Text = "дискв.";
            this.rbDisq2.UseVisualStyleBackColor = true;
            // 
            // rbFall2
            // 
            this.rbFall2.AutoSize = true;
            this.rbFall2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbFall2.Location = new System.Drawing.Point(9, 83);
            this.rbFall2.Name = "rbFall2";
            this.rbFall2.Size = new System.Drawing.Size(87, 33);
            this.rbFall2.TabIndex = 3;
            this.rbFall2.Text = "срыв";
            this.rbFall2.UseVisualStyleBackColor = true;
            // 
            // rbTime2
            // 
            this.rbTime2.AutoSize = true;
            this.rbTime2.Checked = true;
            this.rbTime2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbTime2.Location = new System.Drawing.Point(9, 36);
            this.rbTime2.Name = "rbTime2";
            this.rbTime2.Size = new System.Drawing.Size(168, 46);
            this.rbTime2.TabIndex = 2;
            this.rbTime2.TabStop = true;
            this.rbTime2.Text = "0:00.00";
            this.rbTime2.UseVisualStyleBackColor = true;
            // 
            // lblClimber2
            // 
            this.lblClimber2.AutoSize = true;
            this.lblClimber2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblClimber2.Location = new System.Drawing.Point(6, 16);
            this.lblClimber2.Name = "lblClimber2";
            this.lblClimber2.Size = new System.Drawing.Size(105, 16);
            this.lblClimber2.TabIndex = 0;
            this.lblClimber2.Text = "Нет участника";
            // 
            // btnSaveRes
            // 
            this.btnSaveRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveRes.Location = new System.Drawing.Point(12, 355);
            this.btnSaveRes.Name = "btnSaveRes";
            this.btnSaveRes.Size = new System.Drawing.Size(132, 59);
            this.btnSaveRes.TabIndex = 2;
            this.btnSaveRes.Text = "Ввести результат";
            this.btnSaveRes.UseVisualStyleBackColor = true;
            this.btnSaveRes.Click += new System.EventHandler(this.btnSaveRes_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(317, 355);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(132, 59);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(13, 7);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(35, 13);
            this.lblTimer.TabIndex = 4;
            this.lblTimer.Text = "label1";
            // 
            // autoCloseTimer
            // 
            this.autoCloseTimer.Interval = 1000;
            this.autoCloseTimer.Tick += new System.EventHandler(this.autoCloseTimer_Tick);
            // 
            // btnStopAuto
            // 
            this.btnStopAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStopAuto.Location = new System.Drawing.Point(150, 355);
            this.btnStopAuto.Name = "btnStopAuto";
            this.btnStopAuto.Size = new System.Drawing.Size(161, 59);
            this.btnStopAuto.TabIndex = 3;
            this.btnStopAuto.Text = "Выход из автоматического режима";
            this.btnStopAuto.UseVisualStyleBackColor = true;
            this.btnStopAuto.Click += new System.EventHandler(this.btnStopAuto_Click);
            // 
            // btnExchange
            // 
            this.btnExchange.BackColor = System.Drawing.SystemColors.Window;
            this.btnExchange.Image = global::ClimbingCompetition.Properties.Resources.Exchange;
            this.btnExchange.Location = new System.Drawing.Point(207, 12);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(28, 51);
            this.btnExchange.TabIndex = 5;
            this.btnExchange.UseVisualStyleBackColor = false;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // DataFromSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 424);
            this.ControlBox = false;
            this.Controls.Add(this.btnExchange);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.btnStopAuto);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSaveRes);
            this.Controls.Add(this.gbR2);
            this.Controls.Add(this.gbR1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataFromSystem";
            this.Text = "Считывание данных...";
            this.Load += new System.EventHandler(this.DataFromSystem_Load);
            this.gbR1.ResumeLayout(false);
            this.gbR1.PerformLayout();
            this.gbRT1.ResumeLayout(false);
            this.gbRT1.PerformLayout();
            this.gbR2.ResumeLayout(false);
            this.gbR2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbR1;
        private System.Windows.Forms.Label lblClimber1;
        private System.Windows.Forms.RadioButton rbDisq1;
        private System.Windows.Forms.RadioButton rbFall1;
        private System.Windows.Forms.RadioButton rbTime1;
        private System.Windows.Forms.RadioButton rbNya1;
        private System.Windows.Forms.GroupBox gbR2;
        private System.Windows.Forms.RadioButton rbNya2;
        private System.Windows.Forms.RadioButton rbDisq2;
        private System.Windows.Forms.RadioButton rbFall2;
        private System.Windows.Forms.RadioButton rbTime2;
        private System.Windows.Forms.Label lblClimber2;
        private System.Windows.Forms.Button btnSaveRes;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox gbRT1;
        private System.Windows.Forms.RadioButton rbS1;
        private System.Windows.Forms.RadioButton rbR1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbS2;
        private System.Windows.Forms.RadioButton rbR2;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Timer autoCloseTimer;
        private System.Windows.Forms.Button btnStopAuto;
        private System.Windows.Forms.Button btnEnterAndSetNext;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.Button btnSetNext2;
    }
}