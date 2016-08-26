using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class ExcelWorkingForm : Form
    {
        public ExcelWorkingForm()
        {
            InitializeComponent();
        }
        private bool directionUp = true;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (directionUp)
            {
                if (progressBar1.Value == 99)
                    directionUp = false;
                progressBar1.Value++;
            }
            else
            {
                if (progressBar1.Value == 1)
                    directionUp = true;
                progressBar1.Value--;
            }
        }

        private void ExcelWorkingForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            StaticClass.ExecutionFinished += new StaticClass.ExecutionFinishedEventHandler(StaticClass_ExecutionFinished);
        }

        void StaticClass_ExecutionFinished(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(delegate(object o, EventArgs ec)
            {
                this.Close();
            }));
        }

        private void ExcelWorkingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            StaticClass.ExecutionFinished -= this.StaticClass_ExecutionFinished;
        }
    }
}
