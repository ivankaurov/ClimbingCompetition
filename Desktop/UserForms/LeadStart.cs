using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class LeadStart : Form
    {
        protected LeadStart()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        public static bool StartClimber(string climberName)
        {
            LeadStart ls = new LeadStart();
            ls.Text = climberName;
            return (ls.ShowDialog() == DialogResult.OK);
        }

        System.Timers.Timer t = null;

        int remainingSeconds = 0;

        private void LeadStart_Load(object sender, EventArgs e)
        {
            t = new System.Timers.Timer(1000);
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            remainingSeconds = 40;
            updateLabel();
            t.Start();
        }

        private void updateLabel()
        {
            string sToSet = "00:" + remainingSeconds.ToString("00");
            if (label1.InvokeRequired)
                label1.Invoke(new EventHandler(delegate { label1.Text = sToSet; }));
            else
                label1.Text = sToSet;
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (remainingSeconds > 0)
            {
                remainingSeconds--;
                updateLabel();
            }
            else
            {
                System.Timers.Timer _t = sender as System.Timers.Timer;
                if (_t != null)
                    _t.Stop();
                MessageBox.Show("Время истекло", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            t.Stop();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            t.Stop();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
