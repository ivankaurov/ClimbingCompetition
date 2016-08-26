using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class GetSerial : Form
    {
        public GetSerial()
        {
            InitializeComponent();
        }

        public string Serial { get { return textBox1.Text; } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            this.Close();
        }
    }
}