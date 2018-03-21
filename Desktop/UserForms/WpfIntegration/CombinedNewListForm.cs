using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClimbingCompetition.UserForms.Wpf;
using System.Data.SqlClient;
using ClimbingCompetition.WPF;

namespace ClimbingCompetition.UserForms.WpfIntegration
{
    public partial class CombinedNewListForm : WpfContainerForm
    {
        public CombinedNewListForm(SqlConnection cn, string competitionTitle)
            : base(cn, competitionTitle, SimpleWpfControlFactory<CombinedNewListWpf>.Instance)
        {
            InitializeComponent();
        }
    }
}
