namespace ClimbingCompetition
{
    partial class EditTopo
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
            if (disposing)
            {
                if (tran != null)
                    try { tran.Rollback(); }
                    catch { }
                    finally { tran = null; }
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTopo));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbTopoColors = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnColorSelectedHold = new System.Windows.Forms.Button();
            this.btnColorHold = new System.Windows.Forms.Button();
            this.gbEditClimber = new System.Windows.Forms.GroupBox();
            this.tbHeader = new System.Windows.Forms.TextBox();
            this.lblTimer = new System.Windows.Forms.Label();
            this.cbModifier = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.gbSaveCancel = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.gbEditTopo = new System.Windows.Forms.GroupBox();
            this.btnLoadOtherTopo = new System.Windows.Forms.Button();
            this.btnRollback = new System.Windows.Forms.Button();
            this.lblHeight = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.btnDelTopo = new System.Windows.Forms.Button();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            this.btnDelHold = new System.Windows.Forms.Button();
            this.btnCancelEdit = new System.Windows.Forms.Button();
            this.btnNumberHolds = new System.Windows.Forms.Button();
            this.btnUploadTopo = new System.Windows.Forms.Button();
            this.topoBox = new System.Windows.Forms.PictureBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.saveTopo = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbTopoColors.SuspendLayout();
            this.gbEditClimber.SuspendLayout();
            this.gbSaveCancel.SuspendLayout();
            this.gbEditTopo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbTopoColors);
            this.splitContainer1.Panel1.Controls.Add(this.gbEditClimber);
            this.splitContainer1.Panel1.Controls.Add(this.gbSaveCancel);
            this.splitContainer1.Panel1.Controls.Add(this.gbEditTopo);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.topoBox);
            // 
            // gbTopoColors
            // 
            this.gbTopoColors.Controls.Add(this.label5);
            this.gbTopoColors.Controls.Add(this.label4);
            this.gbTopoColors.Controls.Add(this.btnColorSelectedHold);
            this.gbTopoColors.Controls.Add(this.btnColorHold);
            resources.ApplyResources(this.gbTopoColors, "gbTopoColors");
            this.gbTopoColors.Name = "gbTopoColors";
            this.gbTopoColors.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnColorSelectedHold
            // 
            this.btnColorSelectedHold.BackColor = System.Drawing.Color.Red;
            this.btnColorSelectedHold.ForeColor = System.Drawing.Color.DarkOrange;
            resources.ApplyResources(this.btnColorSelectedHold, "btnColorSelectedHold");
            this.btnColorSelectedHold.Name = "btnColorSelectedHold";
            this.btnColorSelectedHold.UseVisualStyleBackColor = false;
            this.btnColorSelectedHold.Click += new System.EventHandler(this.btnColorHold_Click);
            // 
            // btnColorHold
            // 
            this.btnColorHold.BackColor = System.Drawing.Color.Red;
            this.btnColorHold.ForeColor = System.Drawing.Color.DarkOrange;
            resources.ApplyResources(this.btnColorHold, "btnColorHold");
            this.btnColorHold.Name = "btnColorHold";
            this.btnColorHold.UseVisualStyleBackColor = false;
            this.btnColorHold.Click += new System.EventHandler(this.btnColorHold_Click);
            // 
            // gbEditClimber
            // 
            this.gbEditClimber.Controls.Add(this.tbHeader);
            this.gbEditClimber.Controls.Add(this.lblTimer);
            this.gbEditClimber.Controls.Add(this.cbModifier);
            this.gbEditClimber.Controls.Add(this.label1);
            this.gbEditClimber.Controls.Add(this.lblResult);
            this.gbEditClimber.Controls.Add(this.tbTime);
            this.gbEditClimber.Controls.Add(this.tbResult);
            resources.ApplyResources(this.gbEditClimber, "gbEditClimber");
            this.gbEditClimber.Name = "gbEditClimber";
            this.gbEditClimber.TabStop = false;
            // 
            // tbHeader
            // 
            resources.ApplyResources(this.tbHeader, "tbHeader");
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.ReadOnly = true;
            // 
            // lblTimer
            // 
            resources.ApplyResources(this.lblTimer, "lblTimer");
            this.lblTimer.Name = "lblTimer";
            // 
            // cbModifier
            // 
            this.cbModifier.FormattingEnabled = true;
            this.cbModifier.Items.AddRange(new object[] {
            resources.GetString("cbModifier.Items"),
            resources.GetString("cbModifier.Items1"),
            resources.GetString("cbModifier.Items2")});
            resources.ApplyResources(this.cbModifier, "cbModifier");
            this.cbModifier.Name = "cbModifier";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblResult
            // 
            resources.ApplyResources(this.lblResult, "lblResult");
            this.lblResult.Name = "lblResult";
            // 
            // tbTime
            // 
            resources.ApplyResources(this.tbTime, "tbTime");
            this.tbTime.Name = "tbTime";
            // 
            // tbResult
            // 
            resources.ApplyResources(this.tbResult, "tbResult");
            this.tbResult.Name = "tbResult";
            // 
            // gbSaveCancel
            // 
            this.gbSaveCancel.Controls.Add(this.btnCancel);
            this.gbSaveCancel.Controls.Add(this.btnSave);
            resources.ApplyResources(this.gbSaveCancel, "gbSaveCancel");
            this.gbSaveCancel.Name = "gbSaveCancel";
            this.gbSaveCancel.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gbEditTopo
            // 
            this.gbEditTopo.Controls.Add(this.btnLoadOtherTopo);
            this.gbEditTopo.Controls.Add(this.btnRollback);
            this.gbEditTopo.Controls.Add(this.lblHeight);
            this.gbEditTopo.Controls.Add(this.tbHeight);
            this.gbEditTopo.Controls.Add(this.btnDelTopo);
            this.gbEditTopo.Controls.Add(this.btnSaveToFile);
            this.gbEditTopo.Controls.Add(this.btnDelHold);
            this.gbEditTopo.Controls.Add(this.btnCancelEdit);
            this.gbEditTopo.Controls.Add(this.btnNumberHolds);
            this.gbEditTopo.Controls.Add(this.btnUploadTopo);
            resources.ApplyResources(this.gbEditTopo, "gbEditTopo");
            this.gbEditTopo.Name = "gbEditTopo";
            this.gbEditTopo.TabStop = false;
            // 
            // btnLoadOtherTopo
            // 
            resources.ApplyResources(this.btnLoadOtherTopo, "btnLoadOtherTopo");
            this.btnLoadOtherTopo.Name = "btnLoadOtherTopo";
            this.btnLoadOtherTopo.UseVisualStyleBackColor = true;
            this.btnLoadOtherTopo.Click += new System.EventHandler(this.btnLoadOtherTopo_Click);
            // 
            // btnRollback
            // 
            this.btnRollback.BackgroundImage = global::ClimbingCompetition.Properties.Resources.rollback;
            resources.ApplyResources(this.btnRollback, "btnRollback");
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.UseVisualStyleBackColor = true;
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            // 
            // lblHeight
            // 
            resources.ApplyResources(this.lblHeight, "lblHeight");
            this.lblHeight.Name = "lblHeight";
            // 
            // tbHeight
            // 
            resources.ApplyResources(this.tbHeight, "tbHeight");
            this.tbHeight.Name = "tbHeight";
            // 
            // btnDelTopo
            // 
            resources.ApplyResources(this.btnDelTopo, "btnDelTopo");
            this.btnDelTopo.Name = "btnDelTopo";
            this.btnDelTopo.UseVisualStyleBackColor = true;
            this.btnDelTopo.Click += new System.EventHandler(this.btnDelTopo_Click);
            // 
            // btnSaveToFile
            // 
            resources.ApplyResources(this.btnSaveToFile, "btnSaveToFile");
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.UseVisualStyleBackColor = true;
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // btnDelHold
            // 
            resources.ApplyResources(this.btnDelHold, "btnDelHold");
            this.btnDelHold.Name = "btnDelHold";
            this.btnDelHold.UseVisualStyleBackColor = true;
            this.btnDelHold.Click += new System.EventHandler(this.btnDelHold_Click);
            // 
            // btnCancelEdit
            // 
            resources.ApplyResources(this.btnCancelEdit, "btnCancelEdit");
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.UseVisualStyleBackColor = true;
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // btnNumberHolds
            // 
            resources.ApplyResources(this.btnNumberHolds, "btnNumberHolds");
            this.btnNumberHolds.Name = "btnNumberHolds";
            this.btnNumberHolds.UseVisualStyleBackColor = true;
            this.btnNumberHolds.Click += new System.EventHandler(this.btnNumberHolds_Click);
            // 
            // btnUploadTopo
            // 
            resources.ApplyResources(this.btnUploadTopo, "btnUploadTopo");
            this.btnUploadTopo.Name = "btnUploadTopo";
            this.btnUploadTopo.UseVisualStyleBackColor = true;
            this.btnUploadTopo.Click += new System.EventHandler(this.btnUploadTopo_Click);
            // 
            // topoBox
            // 
            resources.ApplyResources(this.topoBox, "topoBox");
            this.topoBox.Name = "topoBox";
            this.topoBox.TabStop = false;
            this.topoBox.SizeChanged += new System.EventHandler(this.topoBox_Resize);
            this.topoBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.topoBox_MouseClick);
            // 
            // ofd
            // 
            resources.ApplyResources(this.ofd, "ofd");
            // 
            // saveTopo
            // 
            resources.ApplyResources(this.saveTopo, "saveTopo");
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // EditTopo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "EditTopo";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTopo_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditTopo_FormClosed);
            this.Load += new System.EventHandler(this.EditTopo_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gbTopoColors.ResumeLayout(false);
            this.gbTopoColors.PerformLayout();
            this.gbEditClimber.ResumeLayout(false);
            this.gbEditClimber.PerformLayout();
            this.gbSaveCancel.ResumeLayout(false);
            this.gbEditTopo.ResumeLayout(false);
            this.gbEditTopo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbEditTopo;
        private System.Windows.Forms.Button btnUploadTopo;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNumberHolds;
        private System.Windows.Forms.GroupBox gbSaveCancel;
        private System.Windows.Forms.Button btnCancelEdit;
        private System.Windows.Forms.Button btnSaveToFile;
        private System.Windows.Forms.Button btnDelTopo;
        private System.Windows.Forms.SaveFileDialog saveTopo;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.GroupBox gbEditClimber;
        private System.Windows.Forms.ComboBox cbModifier;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.TextBox tbHeader;
        private System.Windows.Forms.GroupBox gbTopoColors;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnColorSelectedHold;
        private System.Windows.Forms.Button btnColorHold;
        private System.Windows.Forms.Button btnDelHold;
        private System.Windows.Forms.Button btnRollback;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTime;
        private System.Windows.Forms.Button btnLoadOtherTopo;
        private System.Windows.Forms.PictureBox topoBox;
    }
}