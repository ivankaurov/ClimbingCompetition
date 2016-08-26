using System;
namespace ListShow
{
    partial class TimerControl
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
            if (disposing && bt != null)
                ((IDisposable)bt).Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTime = new System.Windows.Forms.Label();
            this.timerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.таймерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSignalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настроитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSynchroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.сброситьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.ContextMenuStrip = this.timerContextMenuStrip;
            this.lblTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTime.Location = new System.Drawing.Point(0, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(289, 108);
            this.lblTime.TabIndex = 1;
            this.lblTime.Text = "00:00";
            // 
            // timerContextMenuStrip
            // 
            this.timerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.таймерToolStripMenuItem,
            this.setSignalsToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.autoresetToolStripMenuItem,
            this.autoSynchroToolStripMenuItem});
            this.timerContextMenuStrip.Name = "timerContextMenuStrip";
            this.timerContextMenuStrip.Size = new System.Drawing.Size(185, 136);
            // 
            // таймерToolStripMenuItem
            // 
            this.таймерToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTimerToolStripMenuItem,
            this.stopTimerToolStripMenuItem,
            this.сброситьToolStripMenuItem});
            this.таймерToolStripMenuItem.Name = "таймерToolStripMenuItem";
            this.таймерToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.таймерToolStripMenuItem.Text = "Таймер";
            // 
            // startTimerToolStripMenuItem
            // 
            this.startTimerToolStripMenuItem.Name = "startTimerToolStripMenuItem";
            this.startTimerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startTimerToolStripMenuItem.Text = "Запустить";
            this.startTimerToolStripMenuItem.Click += new System.EventHandler(this.startTimerToolStripMenuItem_Click);
            // 
            // stopTimerToolStripMenuItem
            // 
            this.stopTimerToolStripMenuItem.Name = "stopTimerToolStripMenuItem";
            this.stopTimerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopTimerToolStripMenuItem.Text = "Остановить";
            this.stopTimerToolStripMenuItem.Click += new System.EventHandler(this.stopTimerToolStripMenuItem_Click);
            // 
            // setSignalsToolStripMenuItem
            // 
            this.setSignalsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настроитьToolStripMenuItem});
            this.setSignalsToolStripMenuItem.Name = "setSignalsToolStripMenuItem";
            this.setSignalsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.setSignalsToolStripMenuItem.Text = "Сигналы";
            // 
            // настроитьToolStripMenuItem
            // 
            this.настроитьToolStripMenuItem.Name = "настроитьToolStripMenuItem";
            this.настроитьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.настроитьToolStripMenuItem.Text = "Настроить";
            this.настроитьToolStripMenuItem.Click += new System.EventHandler(this.настроитьToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fontToolStripMenuItem.Text = "Шрифт";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // autoresetToolStripMenuItem
            // 
            this.autoresetToolStripMenuItem.Checked = true;
            this.autoresetToolStripMenuItem.CheckOnClick = true;
            this.autoresetToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoresetToolStripMenuItem.Name = "autoresetToolStripMenuItem";
            this.autoresetToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.autoresetToolStripMenuItem.Text = "Автосброс";
            this.autoresetToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autoresetToolStripMenuItem_CheckedChanged);
            // 
            // autoSynchroToolStripMenuItem
            // 
            this.autoSynchroToolStripMenuItem.Checked = true;
            this.autoSynchroToolStripMenuItem.CheckOnClick = true;
            this.autoSynchroToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSynchroToolStripMenuItem.Name = "autoSynchroToolStripMenuItem";
            this.autoSynchroToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.autoSynchroToolStripMenuItem.Text = "Автосинхронизация";
            this.autoSynchroToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autoSynchroToolStripMenuItem_CheckedChanged);
            // 
            // fontDialog
            // 
            this.fontDialog.FontMustExist = true;
            this.fontDialog.ShowColor = true;
            // 
            // сброситьToolStripMenuItem
            // 
            this.сброситьToolStripMenuItem.Name = "сброситьToolStripMenuItem";
            this.сброситьToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.сброситьToolStripMenuItem.Text = "Сбросить";
            this.сброситьToolStripMenuItem.Click += new System.EventHandler(this.сброситьToolStripMenuItem_Click);
            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTime);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(289, 108);
            this.timerContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ContextMenuStrip timerContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem таймерToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTimerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopTimerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setSignalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настроитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.ToolStripMenuItem autoresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSynchroToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сброситьToolStripMenuItem;
    }
}
