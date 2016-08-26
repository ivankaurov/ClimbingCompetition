namespace BoulderTimer
{
    partial class TimerShowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimerShowForm));
            this.timerControl = new ListShow.TimerControl();
            this.SuspendLayout();
            // 
            // timerControl
            // 
            this.timerControl.ConnectionString = "";
            this.timerControl.ContextMenuEnabled = true;
            this.timerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timerControl.Location = new System.Drawing.Point(0, 0);
            this.timerControl.Name = "timerControl";
            this.timerControl.Size = new System.Drawing.Size(885, 485);
            this.timerControl.TabIndex = 0;
            // 
            // TimerShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 485);
            this.Controls.Add(this.timerControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TimerShowForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TimerShowForm_FormClosing);
            this.Load += new System.EventHandler(this.TimerShowForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListShow.TimerControl timerControl;
    }
}

