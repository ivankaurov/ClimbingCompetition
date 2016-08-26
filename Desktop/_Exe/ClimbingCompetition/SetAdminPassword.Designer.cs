namespace ClimbingCompetition
{
    partial class SetAdminPassword
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
            this.lblOld = new System.Windows.Forms.Label();
            this.tbOld = new System.Windows.Forms.TextBox();
            this.lblNew = new System.Windows.Forms.Label();
            this.tbNew = new System.Windows.Forms.TextBox();
            this.lblConf = new System.Windows.Forms.Label();
            this.tbConf = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblOld
            // 
            this.lblOld.AutoSize = true;
            this.lblOld.Location = new System.Drawing.Point(13, 13);
            this.lblOld.Name = "lblOld";
            this.lblOld.Size = new System.Drawing.Size(87, 13);
            this.lblOld.TabIndex = 0;
            this.lblOld.Text = "Старый пароль:";
            // 
            // tbOld
            // 
            this.tbOld.Location = new System.Drawing.Point(107, 13);
            this.tbOld.Name = "tbOld";
            this.tbOld.PasswordChar = '*';
            this.tbOld.Size = new System.Drawing.Size(173, 20);
            this.tbOld.TabIndex = 1;
            // 
            // lblNew
            // 
            this.lblNew.AutoSize = true;
            this.lblNew.Location = new System.Drawing.Point(18, 42);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(83, 13);
            this.lblNew.TabIndex = 0;
            this.lblNew.Text = "Новый пароль:";
            // 
            // tbNew
            // 
            this.tbNew.Location = new System.Drawing.Point(107, 42);
            this.tbNew.Name = "tbNew";
            this.tbNew.PasswordChar = '*';
            this.tbNew.Size = new System.Drawing.Size(173, 20);
            this.tbNew.TabIndex = 2;
            // 
            // lblConf
            // 
            this.lblConf.AutoSize = true;
            this.lblConf.Location = new System.Drawing.Point(13, 69);
            this.lblConf.Name = "lblConf";
            this.lblConf.Size = new System.Drawing.Size(91, 13);
            this.lblConf.TabIndex = 0;
            this.lblConf.Text = "Подтверждение:";
            // 
            // tbConf
            // 
            this.tbConf.Location = new System.Drawing.Point(107, 69);
            this.tbConf.Name = "tbConf";
            this.tbConf.PasswordChar = '*';
            this.tbConf.Size = new System.Drawing.Size(173, 20);
            this.tbConf.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(21, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(209, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SetAdminPassword
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 126);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbConf);
            this.Controls.Add(this.tbNew);
            this.Controls.Add(this.tbOld);
            this.Controls.Add(this.lblConf);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.lblOld);
            this.Name = "SetAdminPassword";
            this.Text = "SetAdminPassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOld;
        private System.Windows.Forms.TextBox tbOld;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.TextBox tbNew;
        private System.Windows.Forms.Label lblConf;
        private System.Windows.Forms.TextBox tbConf;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}