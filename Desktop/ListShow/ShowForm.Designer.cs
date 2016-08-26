namespace ListShow
{
    partial class ShowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.обновитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.следующийЭкранToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.журналОшибокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настроитьФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.установитьФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.установитьВыводОшибокНаЭкранToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.очиститьЖурналОшибокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.таймерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запуститьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.остановитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаСигналовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сброситьОчередьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leadPhotoControl = new ListShow.LeadPhotoControl();
            this.speedPhotoControl = new ListShow.SpeedPhotoControl();
            this.listShowControl = new ListShow.ListShowControl();
            this.textLine = new ListShow.TextLine();
            this.timerControl = new ListShow.TimerControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(815, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.обновитьToolStripMenuItem,
            this.следующийЭкранToolStripMenuItem,
            this.connectToolStripMenuItem,
            this.журналОшибокToolStripMenuItem,
            this.таймерToolStripMenuItem,
            this.выходToolStripMenuItem,
            this.сброситьОчередьToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "&Файл";
            // 
            // обновитьToolStripMenuItem
            // 
            this.обновитьToolStripMenuItem.Name = "обновитьToolStripMenuItem";
            this.обновитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.обновитьToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.обновитьToolStripMenuItem.Text = "Обновить";
            this.обновитьToolStripMenuItem.Click += new System.EventHandler(this.обновитьToolStripMenuItem_Click);
            // 
            // следующийЭкранToolStripMenuItem
            // 
            this.следующийЭкранToolStripMenuItem.Name = "следующийЭкранToolStripMenuItem";
            this.следующийЭкранToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.следующийЭкранToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.следующийЭкранToolStripMenuItem.Text = "Следующий экран";
            this.следующийЭкранToolStripMenuItem.Click += new System.EventHandler(this.следующийЭкранToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.connectToolStripMenuItem.Text = "&Подключится к БД";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // журналОшибокToolStripMenuItem
            // 
            this.журналОшибокToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настроитьФайлToolStripMenuItem,
            this.очиститьЖурналОшибокToolStripMenuItem});
            this.журналОшибокToolStripMenuItem.Name = "журналОшибокToolStripMenuItem";
            this.журналОшибокToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.журналОшибокToolStripMenuItem.Text = "Журнал ошибок";
            // 
            // настроитьФайлToolStripMenuItem
            // 
            this.настроитьФайлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.установитьФайлToolStripMenuItem,
            this.установитьВыводОшибокНаЭкранToolStripMenuItem});
            this.настроитьФайлToolStripMenuItem.Name = "настроитьФайлToolStripMenuItem";
            this.настроитьФайлToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.настроитьФайлToolStripMenuItem.Text = "Настроить файл";
            // 
            // установитьФайлToolStripMenuItem
            // 
            this.установитьФайлToolStripMenuItem.Name = "установитьФайлToolStripMenuItem";
            this.установитьФайлToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.установитьФайлToolStripMenuItem.Text = "Установить файл";
            this.установитьФайлToolStripMenuItem.Click += new System.EventHandler(this.файлЖурналаОшибокToolStripMenuItem_Click);
            // 
            // установитьВыводОшибокНаЭкранToolStripMenuItem
            // 
            this.установитьВыводОшибокНаЭкранToolStripMenuItem.Name = "установитьВыводОшибокНаЭкранToolStripMenuItem";
            this.установитьВыводОшибокНаЭкранToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.установитьВыводОшибокНаЭкранToolStripMenuItem.Text = "Установить вывод ошибок на экран";
            this.установитьВыводОшибокНаЭкранToolStripMenuItem.Click += new System.EventHandler(this.установитьВыводОшибокНаЭкранToolStripMenuItem_Click);
            // 
            // очиститьЖурналОшибокToolStripMenuItem
            // 
            this.очиститьЖурналОшибокToolStripMenuItem.Name = "очиститьЖурналОшибокToolStripMenuItem";
            this.очиститьЖурналОшибокToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.очиститьЖурналОшибокToolStripMenuItem.Text = "Очистить журнал ошибок";
            this.очиститьЖурналОшибокToolStripMenuItem.Click += new System.EventHandler(this.очиститьЖурналОшибокToolStripMenuItem_Click);
            // 
            // таймерToolStripMenuItem
            // 
            this.таймерToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.запуститьToolStripMenuItem,
            this.остановитьToolStripMenuItem,
            this.настройкаСигналовToolStripMenuItem});
            this.таймерToolStripMenuItem.Name = "таймерToolStripMenuItem";
            this.таймерToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.таймерToolStripMenuItem.Text = "Таймер";
            // 
            // запуститьToolStripMenuItem
            // 
            this.запуститьToolStripMenuItem.Name = "запуститьToolStripMenuItem";
            this.запуститьToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.запуститьToolStripMenuItem.Text = "Запустить";
            this.запуститьToolStripMenuItem.Click += new System.EventHandler(this.запуститьToolStripMenuItem_Click);
            // 
            // остановитьToolStripMenuItem
            // 
            this.остановитьToolStripMenuItem.Name = "остановитьToolStripMenuItem";
            this.остановитьToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.остановитьToolStripMenuItem.Text = "Остановить";
            this.остановитьToolStripMenuItem.Click += new System.EventHandler(this.остановитьToolStripMenuItem_Click);
            // 
            // настройкаСигналовToolStripMenuItem
            // 
            this.настройкаСигналовToolStripMenuItem.Name = "настройкаСигналовToolStripMenuItem";
            this.настройкаСигналовToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.настройкаСигналовToolStripMenuItem.Text = "Настройка сигналов";
            this.настройкаСигналовToolStripMenuItem.Click += new System.EventHandler(this.настройкаСигналовToolStripMenuItem_Click);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.выходToolStripMenuItem.Text = "В&ыход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // сброситьОчередьToolStripMenuItem
            // 
            this.сброситьОчередьToolStripMenuItem.Name = "сброситьОчередьToolStripMenuItem";
            this.сброситьОчередьToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.сброситьОчередьToolStripMenuItem.Text = "Сбросить очередь";
            this.сброситьОчередьToolStripMenuItem.Click += new System.EventHandler(this.сброситьОчередьToolStripMenuItem_Click);
            // 
            // leadPhotoControl
            // 
            this.leadPhotoControl.AllowEvent = false;
            this.leadPhotoControl.ClimbingStyle = ListShow.BasePhotoControl.ClimbingStyleType.Трудность;
            this.leadPhotoControl.cn = null;
            this.leadPhotoControl.Location = new System.Drawing.Point(53, 19);
            this.leadPhotoControl.Name = "leadPhotoControl";
            this.leadPhotoControl.ProjNum = 0;
            this.leadPhotoControl.Size = new System.Drawing.Size(196, 91);
            this.leadPhotoControl.TabIndex = 2;
            // 
            // speedPhotoControl
            // 
            this.speedPhotoControl.AllowEvent = false;
            this.speedPhotoControl.ClimbingStyle = ListShow.BasePhotoControl.ClimbingStyleType.Скорость;
            this.speedPhotoControl.cn = null;
            this.speedPhotoControl.Location = new System.Drawing.Point(149, 131);
            this.speedPhotoControl.Name = "speedPhotoControl";
            this.speedPhotoControl.ProjNum = 0;
            this.speedPhotoControl.Size = new System.Drawing.Size(100, 100);
            this.speedPhotoControl.TabIndex = 3;
            // 
            // listShowControl
            // 
            this.listShowControl.AllowEvent = false;
            this.listShowControl.cn = null;
            this.listShowControl.Location = new System.Drawing.Point(77, 241);
            this.listShowControl.Name = "listShowControl";
            this.listShowControl.ProjNum = 0;
            this.listShowControl.Size = new System.Drawing.Size(64, 71);
            this.listShowControl.TabIndex = 0;
            // 
            // textLine
            // 
            this.textLine.Connection = null;
            this.textLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.textLine.Location = new System.Drawing.Point(0, 24);
            this.textLine.Name = "textLine";
            this.textLine.Size = new System.Drawing.Size(815, 39);
            this.textLine.TabIndex = 4;
            this.textLine.TextSpeed = 11;
            // 
            // timerControl
            // 
            this.timerControl.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.timerControl.ConnectionString = "";
            this.timerControl.ContextMenuEnabled = false;
            this.timerControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.timerControl.Location = new System.Drawing.Point(0, 404);
            this.timerControl.Name = "timerControl";
            this.timerControl.Size = new System.Drawing.Size(815, 108);
            this.timerControl.TabIndex = 5;
            this.timerControl.TabStop = false;
            this.timerControl.TimerStateChanged += new System.EventHandler<ListShow.TimerControl.TimerStateChangedEventArgs>(this.timerControl_TimerStateChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(815, 29);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(183, 24);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.timerControl);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(815, 512);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 63);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(815, 566);
            this.toolStripContainer1.TabIndex = 8;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.speedPhotoControl);
            this.panel1.Controls.Add(this.leadPhotoControl);
            this.panel1.Controls.Add(this.listShowControl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 404);
            this.panel1.TabIndex = 6;
            // 
            // ShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 629);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.textLine);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ShowForm";
            this.Text = "ShowForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShowForm_FormClosing);
            this.Load += new System.EventHandler(this.ShowForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ShowForm_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListShowControl listShowControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem обновитьToolStripMenuItem;
        private LeadPhotoControl leadPhotoControl;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private SpeedPhotoControl speedPhotoControl;
        private TextLine textLine;
        private System.Windows.Forms.ToolStripMenuItem следующийЭкранToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem журналОшибокToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настроитьФайлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem установитьФайлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem установитьВыводОшибокНаЭкранToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem очиститьЖурналОшибокToolStripMenuItem;
        private TimerControl timerControl;
        private System.Windows.Forms.ToolStripMenuItem таймерToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem запуститьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem остановитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкаСигналовToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem сброситьОчередьToolStripMenuItem;
    }
}