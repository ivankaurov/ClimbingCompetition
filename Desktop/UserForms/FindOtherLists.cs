using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ClimbingCompetition
{
    public sealed partial class FindOtherLists : Form
    {
        private FindOtherLists(SqlConnection cn, SqlTransaction tran, int listId)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT G.name + ' ' + L.round list_name, L.iid" +
                              "  FROM lists L(nolock)" +
                              "  JOIN groups G(nolock) on G.iid = L.group_id" +
                              " WHERE L.iid <> @iid" +
                              "   AND L.topo IS NOT NULL" +
                           " ORDER BY G.name, L.round";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
            comboBox1.Items.Clear();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    comboBox1.Items.Add(new ListTopo { Iid = Convert.ToInt32(rdr["iid"]), Tag = (string)rdr["list_name"] });
                }
            }
            comboBox1.Text = "Выбрать протокол";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show(this, "Протокол не выбран");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public static ListTopo SelectList(IWin32Window owner, int listOwner, SqlConnection cn, SqlTransaction tran)
        {
            var window = new FindOtherLists(cn, tran, listOwner);
            var res = window.ShowDialog(owner);
            if (res == DialogResult.OK)
                return (ListTopo)window.comboBox1.SelectedItem;
            else
                return null;
        }

    }

    public class ListTopo
    {
        public int Iid { get; set; }
        public String Tag { get; set; }

        public override string ToString()
        {
            return (Tag ?? String.Empty);
        }
    }
}
