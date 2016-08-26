using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ListShow
{
    public partial class LeadPhotoControl : ListShow.BasePhotoControl
    {
        public LeadPhotoControl()
        {
            InitializeComponent();
        }

        protected override void DoLayout()
        {
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();


            this.Controls.Remove(pbPhoto);
            this.splitContainer1.Panel2.Controls.Add(pbPhoto);
            this.pbPhoto.Dock = DockStyle.Fill;

            this.Controls.Remove(namesPanel);
            this.splitContainer2.Panel1.Controls.Add(namesPanel);
            this.namesPanel.Dock = DockStyle.Fill;

            this.Controls.Remove(listBox1);
            this.splitContainer2.Panel2.Controls.Add(listBox1);
            this.listBox1.Dock = DockStyle.Fill;

            this.splitContainer1.Dock = DockStyle.Fill;

            this.splitContainer2.ResumeLayout(false);
            this.splitContainer2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer1.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
