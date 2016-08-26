using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using ClimbingCompetition;

namespace BoulderTimer
{
    public partial class TimerShowForm : Form
    {
        private SqlConnection cn = null;

        public TimerShowForm()
        {
            InitializeComponent();
        }

        private void TimerShowForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    CloseAll();
                AccountForm af = new AccountForm(String.Empty, true);
                af.ShowDialog();
                if (af.ExitPressed || String.IsNullOrEmpty(af.ConnectionString))
                    this.Close();
                cn = new SqlConnection(af.ConnectionString);
                cn.Open();
                this.timerControl.ConnectionString = cn.ConnectionString;
                try
                {
                    BoulderTimer.Properties.Settings set = BoulderTimer.Properties.Settings.Default;
                    this.timerControl.Font = new Font(set.FontFamily, set.FontSize);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка соединения:\r\n" + ex.Message);
                this.Close();
            }
        }

        private void CloseAll()
        {
            try
            {
                if (cn != null && cn.State != ConnectionState.Closed)
                    cn.Close();
                if (cn != null)
                    cn = null;
                GC.Collect();
            }
            catch { }
        }

        private void TimerShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                BoulderTimer.Properties.Settings st = BoulderTimer.Properties.Settings.Default;
                st.FontFamily = this.timerControl.Font.FontFamily.Name;
                st.FontSize = this.timerControl.Font.Size;
                st.Save();
            }
            catch { }
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var r = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                    "Выйти из приложения?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
