namespace ClimbingCompetition
{
    partial class SafetyRepParams
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rbOnlyOne = new System.Windows.Forms.RadioButton();
            this.rbSelected = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.dateToPrint = new System.Windows.Forms.DateTimePicker();
            this.cbNoDate = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgTeams = new System.Windows.Forms.DataGridView();
            this.btnPrintAppls = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTeams)).BeginInit();
            this.SuspendLayout();
            // 
            // rbOnlyOne
            // 
            this.rbOnlyOne.AutoSize = true;
            this.rbOnlyOne.Location = new System.Drawing.Point(3, 3);
            this.rbOnlyOne.Name = "rbOnlyOne";
            this.rbOnlyOne.Size = new System.Drawing.Size(14, 13);
            this.rbOnlyOne.TabIndex = 0;
            this.rbOnlyOne.TabStop = true;
            this.rbOnlyOne.UseVisualStyleBackColor = true;
            // 
            // rbSelected
            // 
            this.rbSelected.AutoSize = true;
            this.rbSelected.Location = new System.Drawing.Point(3, 22);
            this.rbSelected.Name = "rbSelected";
            this.rbSelected.Size = new System.Drawing.Size(176, 17);
            this.rbSelected.TabIndex = 1;
            this.rbSelected.TabStop = true;
            this.rbSelected.Text = "Только отмеченные команды";
            this.rbSelected.UseVisualStyleBackColor = true;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(3, 45);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(93, 17);
            this.rbAll.TabIndex = 2;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "Все команды";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // dateToPrint
            // 
            this.dateToPrint.Location = new System.Drawing.Point(2, 97);
            this.dateToPrint.Name = "dateToPrint";
            this.dateToPrint.Size = new System.Drawing.Size(200, 20);
            this.dateToPrint.TabIndex = 3;
            // 
            // cbNoDate
            // 
            this.cbNoDate.AutoSize = true;
            this.cbNoDate.Location = new System.Drawing.Point(2, 124);
            this.cbNoDate.Name = "cbNoDate";
            this.cbNoDate.Size = new System.Drawing.Size(113, 17);
            this.cbNoDate.TabIndex = 4;
            this.cbNoDate.Text = "Не печатать дату";
            this.cbNoDate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Дата для печати в отчет:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(4, 231);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 47);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Напечатать ТБ";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(189, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 47);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnPrintAppls);
            this.splitContainer1.Panel1.Controls.Add(this.rbOnlyOne);
            this.splitContainer1.Panel1.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel1.Controls.Add(this.rbSelected);
            this.splitContainer1.Panel1.Controls.Add(this.btnOK);
            this.splitContainer1.Panel1.Controls.Add(this.rbAll);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.dateToPrint);
            this.splitContainer1.Panel1.Controls.Add(this.cbNoDate);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgTeams);
            this.splitContainer1.Size = new System.Drawing.Size(654, 290);
            this.splitContainer1.SplitterDistance = 268;
            this.splitContainer1.TabIndex = 5;
            // 
            // dgTeams
            // 
            this.dgTeams.AllowUserToAddRows = false;
            this.dgTeams.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            this.dgTeams.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgTeams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTeams.Location = new System.Drawing.Point(0, 0);
            this.dgTeams.Name = "dgTeams";
            this.dgTeams.Size = new System.Drawing.Size(382, 290);
            this.dgTeams.TabIndex = 5;
            this.dgTeams.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgTeams_CellBeginEdit);
            // 
            // btnPrintAppls
            // 
            this.btnPrintAppls.Location = new System.Drawing.Point(87, 231);
            this.btnPrintAppls.Name = "btnPrintAppls";
            this.btnPrintAppls.Size = new System.Drawing.Size(92, 47);
            this.btnPrintAppls.TabIndex = 8;
            this.btnPrintAppls.Text = "Печать данных для мандатной комиссии";
            this.btnPrintAppls.UseVisualStyleBackColor = true;
            this.btnPrintAppls.Click += new System.EventHandler(this.btnPrintAppls_Click);
            // 
            // SafetyRepParams
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(654, 290);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SafetyRepParams";
            this.Text = "Печать \"Техник безопасности\"";
            this.Load += new System.EventHandler(this.SafetyRepParams_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTeams)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbOnlyOne;
        private System.Windows.Forms.RadioButton rbSelected;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.DateTimePicker dateToPrint;
        private System.Windows.Forms.CheckBox cbNoDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgTeams;
        private System.Windows.Forms.Button btnPrintAppls;
    }
}
