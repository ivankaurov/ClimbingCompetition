using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ClimberPres.xaml
    /// </summary>
    public partial class ClimberPres : UserControl
    {
        private SqlConnection cn;
        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                if (value != null && value.State != System.Data.ConnectionState.Open)
                    value.Open();
                cn = value;
            }
        }
        public ClimberPres()
        {
            InitializeComponent();
        }

        public ListShow.ClimberData CurrentClimber
        {
            get { return (mainPanel.DataContext as ListShow.ClimberData); }
            set
            {
                if (value != null && cn != null && !value.Loaded)
                {
                    if (cn.State != System.Data.ConnectionState.Open)
                        cn.Open();
                    value.LoadData(cn);
                }
                mainPanel.DataContext = value;
            }
        }
    }
}
