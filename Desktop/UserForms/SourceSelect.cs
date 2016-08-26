using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки печати бейджиков
    /// </summary>
    public partial class SourceSelect : Form
    {
        public const string NOLOGO = "Без логотипа";
        public const string PHOTO = "Фотография";
        public const string CANCEL = "CANCEL";

        private string retVal = "";

        private SourceSelect()
        {
            InitializeComponent();
        }

        public static string GetLogo(string where)
        {
            SourceSelect sc = new SourceSelect();
            sc.Text += where;
            sc.ShowDialog();
            return sc.retVal;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            retVal = CANCEL;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbNothing.Checked)
                retVal = NOLOGO;
            else if (rbPhoto.Checked)
                retVal = PHOTO;
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.Title = this.Text;
                if (ofd.ShowDialog(this) == DialogResult.Cancel)
                    return;
                retVal = ofd.FileName;
            }
            this.Close();
        }
    }
}