namespace ClimbingCompetition
{
    partial class SourceSelect
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
            this.rbLogo = new System.Windows.Forms.RadioButton();
            this.rbPhoto = new System.Windows.Forms.RadioButton();
            this.rbNothing = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbLogo
            // 
            this.rbLogo.AutoSize = true;
            this.rbLogo.Checked = true;
            this.rbLogo.Location = new System.Drawing.Point(13, 13);
            this.rbLogo.Name = "rbLogo";
            this.rbLogo.Size = new System.Drawing.Size(67, 17);
            this.rbLogo.TabIndex = 0;
            this.rbLogo.TabStop = true;
            this.rbLogo.Text = "Логотип";
            this.rbLogo.UseVisualStyleBackColor = true;
            // 
            // rbPhoto
            // 
            this.rbPhoto.AutoSize = true;
            this.rbPhoto.Location = new System.Drawing.Point(13, 36);
            this.rbPhoto.Name = "rbPhoto";
            this.rbPhoto.Size = new System.Drawing.Size(90, 17);
            this.rbPhoto.TabIndex = 0;
            this.rbPhoto.Text = "Фотография";
            this.rbPhoto.UseVisualStyleBackColor = true;
            // 
            // rbNothing
            // 
            this.rbNothing.AutoSize = true;
            this.rbNothing.Location = new System.Drawing.Point(13, 59);
            this.rbNothing.Name = "rbNothing";
            this.rbNothing.Size = new System.Drawing.Size(61, 17);
            this.rbNothing.TabIndex = 0;
            this.rbNothing.Text = "Ничего";
            this.rbNothing.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(13, 106);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(94, 106);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SourceSelect
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(195, 155);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbNothing);
            this.Controls.Add(this.rbPhoto);
            this.Controls.Add(this.rbLogo);
            this.Name = "SourceSelect";
            this.Text = "Выберите логотип ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbLogo;
        private System.Windows.Forms.RadioButton rbPhoto;
        private System.Windows.Forms.RadioButton rbNothing;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}