namespace ClimbingCompetition
{
    partial class ONLCompGrid
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gvParams = new System.Windows.Forms.DataGridView();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.paramID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirendlyName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ParamValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.linkText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.setValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clearVal = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvParams)).BeginInit();
            this.SuspendLayout();
            // 
            // gvParams
            // 
            this.gvParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramID,
            this.FirendlyName,
            this.ParamValue,
            this.linkText,
            this.setValue,
            this.clearVal});
            this.gvParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvParams.Location = new System.Drawing.Point(0, 0);
            this.gvParams.Name = "gvParams";
            this.gvParams.Size = new System.Drawing.Size(499, 240);
            this.gvParams.TabIndex = 0;
            this.gvParams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvParams_CellContentClick);
            this.gvParams.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gvParams_RowValidating);
            // 
            // ofd
            // 
            this.ofd.Title = "Выберите файл";
            // 
            // paramID
            // 
            this.paramID.HeaderText = "paramID";
            this.paramID.Name = "paramID";
            this.paramID.ReadOnly = true;
            this.paramID.Visible = false;
            this.paramID.Width = 72;
            // 
            // FirendlyName
            // 
            this.FirendlyName.HeaderText = "Параметр";
            this.FirendlyName.Items.AddRange(new object[] {
            "Положение",
            "Титул положения",
            "Регламент",
            "Титул регламента",
            "Расписание соревнований",
            "Информация по проживанию",
            "Результаты",
            "Логотип на сайте слева",
            "Логотип на сайте справа",
            "Лого спонсора",
            "Лого партнера ФСР",
            "Доп.инфо"});
            this.FirendlyName.Name = "FirendlyName";
            this.FirendlyName.Width = 64;
            // 
            // ParamValue
            // 
            this.ParamValue.HeaderText = "Значение";
            this.ParamValue.Name = "ParamValue";
            this.ParamValue.ReadOnly = true;
            this.ParamValue.Width = 80;
            // 
            // linkText
            // 
            this.linkText.HeaderText = "Текст для ссылки (для доп.информации)";
            this.linkText.Name = "linkText";
            this.linkText.Width = 217;
            // 
            // setValue
            // 
            this.setValue.HeaderText = "Установить значение";
            this.setValue.Name = "setValue";
            this.setValue.Text = "Установить значение";
            this.setValue.ToolTipText = "Установить значение";
            this.setValue.UseColumnTextForButtonValue = true;
            this.setValue.Width = 111;
            // 
            // clearVal
            // 
            this.clearVal.HeaderText = "Очистить значение";
            this.clearVal.Name = "clearVal";
            this.clearVal.Text = "Очистить значение";
            this.clearVal.ToolTipText = "Установить значение";
            this.clearVal.UseColumnTextForButtonValue = true;
            this.clearVal.Width = 99;
            // 
            // ONLCompGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvParams);
            this.Name = "ONLCompGrid";
            this.Size = new System.Drawing.Size(499, 240);
            ((System.ComponentModel.ISupportInitialize)(this.gvParams)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvParams;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramID;
        private System.Windows.Forms.DataGridViewComboBoxColumn FirendlyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn linkText;
        private System.Windows.Forms.DataGridViewButtonColumn setValue;
        private System.Windows.Forms.DataGridViewButtonColumn clearVal;
    }
}
