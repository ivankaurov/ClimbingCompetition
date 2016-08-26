namespace ClimbingCompetition
{
    partial class PositionsEditForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.dgPosView = new System.Windows.Forms.DataGridView();
            this.iidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.должностьDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.positionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsClimbing = new ClimbingCompetition.dsClimbing();
            this.positionsTableAdapter = new ClimbingCompetition.dsClimbingTableAdapters.positionsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dgPosView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsClimbing)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 387);
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
            this.btnCancel.Location = new System.Drawing.Point(570, 387);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(218, 387);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(184, 23);
            this.btnDel.TabIndex = 3;
            this.btnDel.Text = "Удалить выбранную должность";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // dgPosView
            // 
            this.dgPosView.AllowUserToDeleteRows = false;
            this.dgPosView.AutoGenerateColumns = false;
            this.dgPosView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgPosView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPosView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iidDataGridViewTextBoxColumn,
            this.должностьDataGridViewTextBoxColumn,
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn});
            this.dgPosView.DataSource = this.positionsBindingSource;
            this.dgPosView.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgPosView.Location = new System.Drawing.Point(0, 0);
            this.dgPosView.Name = "dgPosView";
            this.dgPosView.Size = new System.Drawing.Size(657, 381);
            this.dgPosView.TabIndex = 4;
            // 
            // iidDataGridViewTextBoxColumn
            // 
            this.iidDataGridViewTextBoxColumn.DataPropertyName = "iid";
            this.iidDataGridViewTextBoxColumn.HeaderText = "iid";
            this.iidDataGridViewTextBoxColumn.Name = "iidDataGridViewTextBoxColumn";
            this.iidDataGridViewTextBoxColumn.Width = 42;
            // 
            // должностьDataGridViewTextBoxColumn
            // 
            this.должностьDataGridViewTextBoxColumn.DataPropertyName = "Должность";
            this.должностьDataGridViewTextBoxColumn.HeaderText = "Должность";
            this.должностьDataGridViewTextBoxColumn.Name = "должностьDataGridViewTextBoxColumn";
            this.должностьDataGridViewTextBoxColumn.Width = 90;
            // 
            // дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn
            // 
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn.DataPropertyName = "Для впечатывания в бейджики";
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn.HeaderText = "Для впечатывания в бейджики";
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn.Name = "дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn";
            this.дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn.Width = 109;
            // 
            // positionsBindingSource
            // 
            this.positionsBindingSource.DataMember = "positions";
            this.positionsBindingSource.DataSource = this.dsClimbing;
            // 
            // dsClimbing
            // 
            this.dsClimbing.DataSetName = "dsClimbing";
            this.dsClimbing.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // positionsTableAdapter
            // 
            this.positionsTableAdapter.ClearBeforeFill = true;
            // 
            // PositionsEditForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(657, 422);
            this.ControlBox = false;
            this.Controls.Add(this.dgPosView);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "PositionsEditForm";
            this.Text = "Настройка судейских должностей";
            this.Load += new System.EventHandler(this.PositionsEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgPosView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsClimbing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.DataGridView dgPosView;
        private dsClimbing dsClimbing;
        private System.Windows.Forms.BindingSource positionsBindingSource;
        private ClimbingCompetition.dsClimbingTableAdapters.positionsTableAdapter positionsTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn iidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn должностьDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn дляВпечатыванияВБейджикиDataGridViewCheckBoxColumn;
    }
}