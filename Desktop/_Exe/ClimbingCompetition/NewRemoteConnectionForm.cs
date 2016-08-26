using ClimbingCompetition.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public partial class NewRemoteConnectionForm : BaseForm
    {
        readonly WPF.NewRemoteConnectionControl control1;

        public NewRemoteConnectionForm(string competitionTitle, SqlConnection cn)
            : base(cn, competitionTitle)
        {
            InitializeComponent();

            this.control1 = new NewRemoteConnectionControl(cn);
            this.elementHost1.Child = this.control1;
            this.control1.DialogClosed += this.WpfControl_DialogClosed;
        }

        private void WpfControl_DialogClosed(object sender, DialogRoutedEventArgs e)
        {
            this.DialogResult = e.DialogResult;
            this.Close();
        }
    }
}
