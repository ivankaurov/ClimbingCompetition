using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class NextRoundForm : Form
    {
        private readonly string[] rounds = new string[] { "1/2 финала", "Финал", "Суперфинал" };

        private string round = "";
        private int routeNumber = -1;
        private bool cancel = false;

        private NextRoundForm(string prevRound)
        {
            InitializeComponent();
            int startInd;
            switch (prevRound.ToLower())
            {
                case "1/2 финала":
                    startInd = 1;
                    break;
                case "финал":
                    startInd = 2;
                    break;
                default:
                    startInd = 0;
                    break;
            }
            for (int i = startInd; i < rounds.Length; i++)
                cbRound.Items.Add(rounds[i]);
        }
        private bool validateData()
        {
            if (cbRound.SelectedIndex < 0 || cbRound.SelectedItem == null)
            {
                MessageBox.Show(this, "Раунд выбран неверно");
                return false;
            }
            round = cbRound.SelectedItem.ToString();
            if (cbRound.SelectedItem.ToString() != rounds[rounds.Length - 1])
            {
                if (!int.TryParse(textBox1.Text, out routeNumber))
                {
                    MessageBox.Show(this, "Число трасс введено неверно");
                    return false;
                }
            }
            else
                routeNumber = -1;
            return true;
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            if (validateData())
                this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cancel = true;
            this.Close();
        }

        public static bool GetNextRoundData(string prevRound, IWin32Window owner, out string round, out int routeNumber)
        {
            NextRoundForm nf = new NextRoundForm(prevRound);
            if (owner == null)
                nf.ShowDialog();
            else
                nf.ShowDialog(owner);
            round = nf.round;
            routeNumber = nf.routeNumber;
            return nf.cancel;
        }
    }
}
