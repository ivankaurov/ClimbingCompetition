using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class GetIid : Form
    {
        private int n;
        private GetIid()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out n))
                this.Close();
        }

        public static int GetInt()
        {
            GetIid gtId = new GetIid();
            gtId.pictureBox.Visible = gtId.btnShowPhoto.Visible = false;
            gtId.ShowDialog();
            return gtId.n;
        }

        private SqlConnection cn = null;

        public GetIid(SqlConnection baseCn)
            : this()
        {
            cn = new SqlConnection(baseCn.ConnectionString);
            cn.Open();
        }

        private void GetIid_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (cn != null)
                    if (cn.State != ConnectionState.Closed)
                        cn.Close();
            }
            catch { }
        }

        private void btnShowPhoto_Click(object sender, EventArgs e)
        {
            int iid;
            if (!int.TryParse(textBox1.Text, out iid))
                return;
            bool judge = (sender == btnShowPhoto);
                
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT remoteString FROM CompetitionData(NOLOCK)";
            string remS = cmd.ExecuteScalar().ToString();
            if (remS.Length < 1)
                return;
            SqlConnection remote = new SqlConnection(remS);
            try
            {
                string path = ImageWorker.GetOnlineImg(remote, iid,judge);
                if (path.Length < 1)
                {
                    pictureBox.Image = null;
                    return;
                }
                Image img = Image.FromFile(System.Threading.Thread.GetDomain().BaseDirectory+ path);
                pictureBox.Image = img;
            }
            finally { remote.Close(); }
        }
        
    }
}