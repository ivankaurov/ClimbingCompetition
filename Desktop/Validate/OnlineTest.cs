using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Validate
{
    public partial class OnlineTest : Form
    {
        SqlConnection cn;
        public OnlineTest(string style, SqlConnection local)
        {
            InitializeComponent();
            //try
            //{
            string rc;
            if (local.State != ConnectionState.Open)
                local.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = local;
            cmd.CommandText = "SELECT remoteString FROM CompetitionData(NOLOCK)";
            rc = cmd.ExecuteScalar().ToString();
            this.cn = new SqlConnection(rc/*"server=ivank\\sql2000;user=sa;password=sa;initial catalog=ONLclimbing"*/);
            if (this.cn.State != ConnectionState.Open)
                this.cn.Open();
            DataTable dt = ClimbingCompetition.Online.ListCreator.GetResultList(ClimbingCompetition.GetIid.GetInt(), this.cn, 1);
            if (dt == null)
                dt = new DataTable();
            dataGridView1.DataSource = dt;
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try { cn.Close(); }
            catch { }
            base.OnClosing(e);
        }
    }
}