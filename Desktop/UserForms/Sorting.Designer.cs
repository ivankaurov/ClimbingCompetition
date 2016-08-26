namespace ClimbingCompetition
{
    partial class Sorting
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
            this.rankings = new System.Windows.Forms.GroupBox();
            this.tb12quan = new System.Windows.Forms.TextBox();
            this.rb132Some = new System.Windows.Forms.RadioButton();
            this.rb131All = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rb123Reverse = new System.Windows.Forms.RadioButton();
            this.rb122Direct = new System.Windows.Forms.RadioButton();
            this.rb121Random = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb11stNum = new System.Windows.Forms.TextBox();
            this.rb112Number = new System.Windows.Forms.RadioButton();
            this.rb113End = new System.Windows.Forms.RadioButton();
            this.rb114Beg = new System.Windows.Forms.RadioButton();
            this.rb111middle = new System.Windows.Forms.RadioButton();
            this.lateAppl = new System.Windows.Forms.GroupBox();
            this.rb22Beginning = new System.Windows.Forms.RadioButton();
            this.rb21End = new System.Windows.Forms.RadioButton();
            this.secondRoute = new System.Windows.Forms.GroupBox();
            this.rb34City = new System.Windows.Forms.RadioButton();
            this.rb33Intl = new System.Windows.Forms.RadioButton();
            this.rb32SameOrder = new System.Windows.Forms.RadioButton();
            this.rb31Reverse = new System.Windows.Forms.RadioButton();
            this.cbRanking = new System.Windows.Forms.CheckBox();
            this.cbLateAppl = new System.Windows.Forms.CheckBox();
            this.random = new System.Windows.Forms.GroupBox();
            this.prevRound = new System.Windows.Forms.GroupBox();
            this.rb_24sameStart = new System.Windows.Forms.RadioButton();
            this.rb_23reverseStart = new System.Windows.Forms.RadioButton();
            this.rb_22SameOrder = new System.Windows.Forms.RadioButton();
            this.rb_21reverse = new System.Windows.Forms.RadioButton();
            this.rbPrev = new System.Windows.Forms.RadioButton();
            this.rbRandom = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbRoundFlash = new System.Windows.Forms.GroupBox();
            this.lblFlashRoutes = new System.Windows.Forms.Label();
            this.tbFlashRoutes = new System.Windows.Forms.TextBox();
            this.cbRoundFlash = new System.Windows.Forms.CheckBox();
            this.rankings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.lateAppl.SuspendLayout();
            this.secondRoute.SuspendLayout();
            this.random.SuspendLayout();
            this.prevRound.SuspendLayout();
            this.gbRoundFlash.SuspendLayout();
            this.SuspendLayout();
            // 
            // rankings
            // 
            this.rankings.Controls.Add(this.tb12quan);
            this.rankings.Controls.Add(this.rb132Some);
            this.rankings.Controls.Add(this.rb131All);
            this.rankings.Controls.Add(this.groupBox2);
            this.rankings.Controls.Add(this.groupBox1);
            this.rankings.Enabled = false;
            this.rankings.Location = new System.Drawing.Point(25, 19);
            this.rankings.Name = "rankings";
            this.rankings.Size = new System.Drawing.Size(477, 106);
            this.rankings.TabIndex = 0;
            this.rankings.TabStop = false;
            this.rankings.Text = "Рейтинговые";
            // 
            // tb12quan
            // 
            this.tb12quan.Location = new System.Drawing.Point(423, 55);
            this.tb12quan.Name = "tb12quan";
            this.tb12quan.Size = new System.Drawing.Size(48, 20);
            this.tb12quan.TabIndex = 1;
            this.tb12quan.Text = "20";
            // 
            // rb132Some
            // 
            this.rb132Some.AutoSize = true;
            this.rb132Some.Checked = true;
            this.rb132Some.Location = new System.Drawing.Point(360, 55);
            this.rb132Some.Name = "rb132Some";
            this.rb132Some.Size = new System.Drawing.Size(65, 17);
            this.rb132Some.TabIndex = 2;
            this.rb132Some.TabStop = true;
            this.rb132Some.Text = "первых:";
            this.rb132Some.UseVisualStyleBackColor = true;
            // 
            // rb131All
            // 
            this.rb131All.AutoSize = true;
            this.rb131All.Location = new System.Drawing.Point(360, 31);
            this.rb131All.Name = "rb131All";
            this.rb131All.Size = new System.Drawing.Size(112, 17);
            this.rb131All.TabIndex = 2;
            this.rb131All.Text = "всех из рейтинга";
            this.rb131All.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rb123Reverse);
            this.groupBox2.Controls.Add(this.rb122Direct);
            this.groupBox2.Controls.Add(this.rb121Random);
            this.groupBox2.Location = new System.Drawing.Point(207, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(146, 81);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // rb123Reverse
            // 
            this.rb123Reverse.AutoSize = true;
            this.rb123Reverse.Checked = true;
            this.rb123Reverse.Location = new System.Drawing.Point(6, 58);
            this.rb123Reverse.Name = "rb123Reverse";
            this.rb123Reverse.Size = new System.Drawing.Size(128, 17);
            this.rb123Reverse.TabIndex = 0;
            this.rb123Reverse.TabStop = true;
            this.rb123Reverse.Text = "в обратном порядке";
            this.rb123Reverse.UseVisualStyleBackColor = true;
            // 
            // rb122Direct
            // 
            this.rb122Direct.AutoSize = true;
            this.rb122Direct.Location = new System.Drawing.Point(6, 35);
            this.rb122Direct.Name = "rb122Direct";
            this.rb122Direct.Size = new System.Drawing.Size(119, 17);
            this.rb122Direct.TabIndex = 0;
            this.rb122Direct.Text = "в прямом порядке";
            this.rb122Direct.UseVisualStyleBackColor = true;
            // 
            // rb121Random
            // 
            this.rb121Random.AutoSize = true;
            this.rb121Random.Location = new System.Drawing.Point(6, 12);
            this.rb121Random.Name = "rb121Random";
            this.rb121Random.Size = new System.Drawing.Size(133, 17);
            this.rb121Random.TabIndex = 0;
            this.rb121Random.Text = "в случайном порядке";
            this.rb121Random.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb11stNum);
            this.groupBox1.Controls.Add(this.rb112Number);
            this.groupBox1.Controls.Add(this.rb113End);
            this.groupBox1.Controls.Add(this.rb114Beg);
            this.groupBox1.Controls.Add(this.rb111middle);
            this.groupBox1.Location = new System.Drawing.Point(7, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // tb11stNum
            // 
            this.tb11stNum.Location = new System.Drawing.Point(134, 34);
            this.tb11stNum.Name = "tb11stNum";
            this.tb11stNum.Size = new System.Drawing.Size(54, 20);
            this.tb11stNum.TabIndex = 1;
            this.tb11stNum.Text = "6";
            // 
            // rb112Number
            // 
            this.rb112Number.AutoSize = true;
            this.rb112Number.Checked = true;
            this.rb112Number.Location = new System.Drawing.Point(6, 37);
            this.rb112Number.Name = "rb112Number";
            this.rb112Number.Size = new System.Drawing.Size(121, 17);
            this.rb112Number.TabIndex = 0;
            this.rb112Number.TabStop = true;
            this.rb112Number.Text = "Начиная с номера:";
            this.rb112Number.UseVisualStyleBackColor = true;
            // 
            // rb113End
            // 
            this.rb113End.AutoSize = true;
            this.rb113End.Location = new System.Drawing.Point(6, 57);
            this.rb113End.Name = "rb113End";
            this.rb113End.Size = new System.Drawing.Size(64, 17);
            this.rb113End.TabIndex = 0;
            this.rb113End.Text = "в конец";
            this.rb113End.UseVisualStyleBackColor = true;
            // 
            // rb114Beg
            // 
            this.rb114Beg.AutoSize = true;
            this.rb114Beg.Location = new System.Drawing.Point(98, 11);
            this.rb114Beg.Name = "rb114Beg";
            this.rb114Beg.Size = new System.Drawing.Size(69, 17);
            this.rb114Beg.TabIndex = 0;
            this.rb114Beg.Text = "с начала";
            this.rb114Beg.UseVisualStyleBackColor = true;
            // 
            // rb111middle
            // 
            this.rb111middle.AutoSize = true;
            this.rb111middle.Location = new System.Drawing.Point(7, 11);
            this.rb111middle.Name = "rb111middle";
            this.rb111middle.Size = new System.Drawing.Size(90, 17);
            this.rb111middle.TabIndex = 0;
            this.rb111middle.Text = "из середины";
            this.rb111middle.UseVisualStyleBackColor = true;
            // 
            // lateAppl
            // 
            this.lateAppl.Controls.Add(this.rb22Beginning);
            this.lateAppl.Controls.Add(this.rb21End);
            this.lateAppl.Enabled = false;
            this.lateAppl.Location = new System.Drawing.Point(25, 215);
            this.lateAppl.Name = "lateAppl";
            this.lateAppl.Size = new System.Drawing.Size(422, 78);
            this.lateAppl.TabIndex = 0;
            this.lateAppl.TabStop = false;
            this.lateAppl.Text = "Поздние заявки";
            this.lateAppl.Enter += new System.EventHandler(this.lateAppl_Enter);
            // 
            // rb22Beginning
            // 
            this.rb22Beginning.AutoSize = true;
            this.rb22Beginning.Location = new System.Drawing.Point(14, 43);
            this.rb22Beginning.Name = "rb22Beginning";
            this.rb22Beginning.Size = new System.Drawing.Size(70, 17);
            this.rb22Beginning.TabIndex = 0;
            this.rb22Beginning.Text = "В начало";
            this.rb22Beginning.UseVisualStyleBackColor = true;
            // 
            // rb21End
            // 
            this.rb21End.AutoSize = true;
            this.rb21End.Checked = true;
            this.rb21End.Location = new System.Drawing.Point(14, 20);
            this.rb21End.Name = "rb21End";
            this.rb21End.Size = new System.Drawing.Size(65, 17);
            this.rb21End.TabIndex = 0;
            this.rb21End.TabStop = true;
            this.rb21End.Text = "В конец";
            this.rb21End.UseVisualStyleBackColor = true;
            // 
            // secondRoute
            // 
            this.secondRoute.Controls.Add(this.rb34City);
            this.secondRoute.Controls.Add(this.rb33Intl);
            this.secondRoute.Controls.Add(this.rb32SameOrder);
            this.secondRoute.Controls.Add(this.rb31Reverse);
            this.secondRoute.Location = new System.Drawing.Point(25, 131);
            this.secondRoute.Name = "secondRoute";
            this.secondRoute.Size = new System.Drawing.Size(422, 78);
            this.secondRoute.TabIndex = 0;
            this.secondRoute.TabStop = false;
            this.secondRoute.Text = "Жеребьёка на 2ую трассу";
            this.secondRoute.Enter += new System.EventHandler(this.lateAppl_Enter);
            // 
            // rb34City
            // 
            this.rb34City.AutoSize = true;
            this.rb34City.Location = new System.Drawing.Point(149, 43);
            this.rb34City.Name = "rb34City";
            this.rb34City.Size = new System.Drawing.Size(248, 17);
            this.rb34City.TabIndex = 1;
            this.rb34City.Text = "Меняются местами половинки половинок :)";
            this.rb34City.UseVisualStyleBackColor = true;
            // 
            // rb33Intl
            // 
            this.rb33Intl.AutoSize = true;
            this.rb33Intl.Location = new System.Drawing.Point(149, 20);
            this.rb33Intl.Name = "rb33Intl";
            this.rb33Intl.Size = new System.Drawing.Size(182, 17);
            this.rb33Intl.TabIndex = 1;
            this.rb33Intl.Text = "Меняются местами половинки";
            this.rb33Intl.UseVisualStyleBackColor = true;
            // 
            // rb32SameOrder
            // 
            this.rb32SameOrder.AutoSize = true;
            this.rb32SameOrder.Location = new System.Drawing.Point(14, 43);
            this.rb32SameOrder.Name = "rb32SameOrder";
            this.rb32SameOrder.Size = new System.Drawing.Size(116, 17);
            this.rb32SameOrder.TabIndex = 0;
            this.rb32SameOrder.Text = "В том же порядке";
            this.rb32SameOrder.UseVisualStyleBackColor = true;
            // 
            // rb31Reverse
            // 
            this.rb31Reverse.AutoSize = true;
            this.rb31Reverse.Checked = true;
            this.rb31Reverse.Location = new System.Drawing.Point(14, 20);
            this.rb31Reverse.Name = "rb31Reverse";
            this.rb31Reverse.Size = new System.Drawing.Size(129, 17);
            this.rb31Reverse.TabIndex = 0;
            this.rb31Reverse.TabStop = true;
            this.rb31Reverse.Text = "В обратном порядке";
            this.rb31Reverse.UseVisualStyleBackColor = true;
            // 
            // cbRanking
            // 
            this.cbRanking.AutoSize = true;
            this.cbRanking.Location = new System.Drawing.Point(4, 19);
            this.cbRanking.Name = "cbRanking";
            this.cbRanking.Size = new System.Drawing.Size(15, 14);
            this.cbRanking.TabIndex = 1;
            this.cbRanking.UseVisualStyleBackColor = true;
            this.cbRanking.CheckedChanged += new System.EventHandler(this.cbRanking_CheckedChanged);
            // 
            // cbLateAppl
            // 
            this.cbLateAppl.AutoSize = true;
            this.cbLateAppl.Location = new System.Drawing.Point(6, 215);
            this.cbLateAppl.Name = "cbLateAppl";
            this.cbLateAppl.Size = new System.Drawing.Size(15, 14);
            this.cbLateAppl.TabIndex = 1;
            this.cbLateAppl.UseVisualStyleBackColor = true;
            this.cbLateAppl.CheckedChanged += new System.EventHandler(this.cbLateAppl_CheckedChanged);
            // 
            // random
            // 
            this.random.Controls.Add(this.rankings);
            this.random.Controls.Add(this.cbLateAppl);
            this.random.Controls.Add(this.lateAppl);
            this.random.Controls.Add(this.secondRoute);
            this.random.Controls.Add(this.cbRanking);
            this.random.Location = new System.Drawing.Point(32, 130);
            this.random.Name = "random";
            this.random.Size = new System.Drawing.Size(535, 309);
            this.random.TabIndex = 2;
            this.random.TabStop = false;
            this.random.Text = "В случайном порядке";
            // 
            // prevRound
            // 
            this.prevRound.Controls.Add(this.rb_24sameStart);
            this.prevRound.Controls.Add(this.rb_23reverseStart);
            this.prevRound.Controls.Add(this.rb_22SameOrder);
            this.prevRound.Controls.Add(this.rb_21reverse);
            this.prevRound.Location = new System.Drawing.Point(32, 52);
            this.prevRound.Name = "prevRound";
            this.prevRound.Size = new System.Drawing.Size(535, 72);
            this.prevRound.TabIndex = 4;
            this.prevRound.TabStop = false;
            this.prevRound.Text = "Порядок основан на предыдущем раунде";
            this.prevRound.Enter += new System.EventHandler(this.prevRound_Enter);
            // 
            // rb_24sameStart
            // 
            this.rb_24sameStart.AutoSize = true;
            this.rb_24sameStart.Location = new System.Drawing.Point(214, 42);
            this.rb_24sameStart.Name = "rb_24sameStart";
            this.rb_24sameStart.Size = new System.Drawing.Size(116, 17);
            this.rb_24sameStart.TabIndex = 4;
            this.rb_24sameStart.Text = "В том же порядке";
            this.rb_24sameStart.UseVisualStyleBackColor = true;
            // 
            // rb_23reverseStart
            // 
            this.rb_23reverseStart.AutoSize = true;
            this.rb_23reverseStart.Location = new System.Drawing.Point(214, 19);
            this.rb_23reverseStart.Name = "rb_23reverseStart";
            this.rb_23reverseStart.Size = new System.Drawing.Size(129, 17);
            this.rb_23reverseStart.TabIndex = 3;
            this.rb_23reverseStart.Text = "В обратном порядке";
            this.rb_23reverseStart.UseVisualStyleBackColor = true;
            // 
            // rb_22SameOrder
            // 
            this.rb_22SameOrder.AutoSize = true;
            this.rb_22SameOrder.Location = new System.Drawing.Point(6, 42);
            this.rb_22SameOrder.Name = "rb_22SameOrder";
            this.rb_22SameOrder.Size = new System.Drawing.Size(150, 17);
            this.rb_22SameOrder.TabIndex = 2;
            this.rb_22SameOrder.Text = "В порядке занятых мест";
            this.rb_22SameOrder.UseVisualStyleBackColor = true;
            this.rb_22SameOrder.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            // 
            // rb_21reverse
            // 
            this.rb_21reverse.AutoSize = true;
            this.rb_21reverse.Checked = true;
            this.rb_21reverse.Location = new System.Drawing.Point(6, 19);
            this.rb_21reverse.Name = "rb_21reverse";
            this.rb_21reverse.Size = new System.Drawing.Size(202, 17);
            this.rb_21reverse.TabIndex = 1;
            this.rb_21reverse.TabStop = true;
            this.rb_21reverse.Text = "В обратном порядке занятых мест";
            this.rb_21reverse.UseVisualStyleBackColor = true;
            // 
            // rbPrev
            // 
            this.rbPrev.AutoSize = true;
            this.rbPrev.Location = new System.Drawing.Point(12, 52);
            this.rbPrev.Name = "rbPrev";
            this.rbPrev.Size = new System.Drawing.Size(14, 13);
            this.rbPrev.TabIndex = 5;
            this.rbPrev.TabStop = true;
            this.rbPrev.UseVisualStyleBackColor = true;
            this.rbPrev.CheckedChanged += new System.EventHandler(this.rbPrev_CheckedChanged);
            // 
            // rbRandom
            // 
            this.rbRandom.AutoSize = true;
            this.rbRandom.Location = new System.Drawing.Point(12, 130);
            this.rbRandom.Name = "rbRandom";
            this.rbRandom.Size = new System.Drawing.Size(14, 13);
            this.rbRandom.TabIndex = 5;
            this.rbRandom.TabStop = true;
            this.rbRandom.UseVisualStyleBackColor = true;
            this.rbRandom.CheckedChanged += new System.EventHandler(this.rbPrev_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(574, 52);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(133, 59);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(574, 117);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(133, 59);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbRoundFlash
            // 
            this.gbRoundFlash.Controls.Add(this.lblFlashRoutes);
            this.gbRoundFlash.Controls.Add(this.tbFlashRoutes);
            this.gbRoundFlash.Controls.Add(this.cbRoundFlash);
            this.gbRoundFlash.Location = new System.Drawing.Point(574, 182);
            this.gbRoundFlash.Name = "gbRoundFlash";
            this.gbRoundFlash.Size = new System.Drawing.Size(161, 87);
            this.gbRoundFlash.TabIndex = 7;
            this.gbRoundFlash.TabStop = false;
            this.gbRoundFlash.Text = "Настройки flash";
            this.gbRoundFlash.Visible = false;
            // 
            // lblFlashRoutes
            // 
            this.lblFlashRoutes.AutoSize = true;
            this.lblFlashRoutes.Location = new System.Drawing.Point(41, 48);
            this.lblFlashRoutes.Name = "lblFlashRoutes";
            this.lblFlashRoutes.Size = new System.Drawing.Size(71, 13);
            this.lblFlashRoutes.TabIndex = 2;
            this.lblFlashRoutes.Text = "Число трасс";
            // 
            // tbFlashRoutes
            // 
            this.tbFlashRoutes.Enabled = false;
            this.tbFlashRoutes.Location = new System.Drawing.Point(7, 46);
            this.tbFlashRoutes.Name = "tbFlashRoutes";
            this.tbFlashRoutes.Size = new System.Drawing.Size(28, 20);
            this.tbFlashRoutes.TabIndex = 1;
            this.tbFlashRoutes.TextChanged += new System.EventHandler(this.tbFlashRoutes_TextChanged);
            // 
            // cbRoundFlash
            // 
            this.cbRoundFlash.AutoSize = true;
            this.cbRoundFlash.Location = new System.Drawing.Point(7, 21);
            this.cbRoundFlash.Name = "cbRoundFlash";
            this.cbRoundFlash.Size = new System.Drawing.Size(143, 17);
            this.cbRoundFlash.TabIndex = 0;
            this.cbRoundFlash.Text = "Раунд проводится flash";
            this.cbRoundFlash.UseVisualStyleBackColor = true;
            this.cbRoundFlash.CheckedChanged += new System.EventHandler(this.cbRoundFlash_CheckedChanged);
            // 
            // Sorting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 452);
            this.Controls.Add(this.gbRoundFlash);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbRandom);
            this.Controls.Add(this.rbPrev);
            this.Controls.Add(this.prevRound);
            this.Controls.Add(this.random);
            this.Name = "Sorting";
            this.ShowIcon = false;
            this.Text = "Жеребьёвка";
            this.rankings.ResumeLayout(false);
            this.rankings.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.lateAppl.ResumeLayout(false);
            this.lateAppl.PerformLayout();
            this.secondRoute.ResumeLayout(false);
            this.secondRoute.PerformLayout();
            this.random.ResumeLayout(false);
            this.random.PerformLayout();
            this.prevRound.ResumeLayout(false);
            this.prevRound.PerformLayout();
            this.gbRoundFlash.ResumeLayout(false);
            this.gbRoundFlash.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox rankings;
        private System.Windows.Forms.GroupBox lateAppl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb11stNum;
        private System.Windows.Forms.RadioButton rb112Number;
        private System.Windows.Forms.RadioButton rb111middle;
        private System.Windows.Forms.RadioButton rb121Random;
        private System.Windows.Forms.RadioButton rb123Reverse;
        private System.Windows.Forms.RadioButton rb122Direct;
        private System.Windows.Forms.RadioButton rb131All;
        private System.Windows.Forms.RadioButton rb132Some;
        private System.Windows.Forms.TextBox tb12quan;
        private System.Windows.Forms.RadioButton rb22Beginning;
        private System.Windows.Forms.RadioButton rb21End;
        private System.Windows.Forms.GroupBox secondRoute;
        private System.Windows.Forms.RadioButton rb32SameOrder;
        private System.Windows.Forms.RadioButton rb31Reverse;
        private System.Windows.Forms.CheckBox cbRanking;
        private System.Windows.Forms.CheckBox cbLateAppl;
        private System.Windows.Forms.RadioButton rb34City;
        private System.Windows.Forms.RadioButton rb33Intl;
        private System.Windows.Forms.GroupBox random;
        private System.Windows.Forms.GroupBox prevRound;
        private System.Windows.Forms.RadioButton rbPrev;
        private System.Windows.Forms.RadioButton rbRandom;
        private System.Windows.Forms.RadioButton rb_22SameOrder;
        private System.Windows.Forms.RadioButton rb_21reverse;
        private System.Windows.Forms.RadioButton rb113End;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rb_24sameStart;
        private System.Windows.Forms.RadioButton rb_23reverseStart;
        private System.Windows.Forms.RadioButton rb114Beg;
        private System.Windows.Forms.GroupBox gbRoundFlash;
        private System.Windows.Forms.Label lblFlashRoutes;
        private System.Windows.Forms.TextBox tbFlashRoutes;
        private System.Windows.Forms.CheckBox cbRoundFlash;
    }
}