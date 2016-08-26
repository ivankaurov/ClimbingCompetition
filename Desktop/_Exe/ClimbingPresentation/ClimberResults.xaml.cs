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
using System.Data;
using System.Data.SqlClient;
using ListShow;
using ClimbingCompetition;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ClimberResults.xaml
    /// </summary>
    public partial class ClimberResults : UserControl
    {
        private SqlConnection cn;
        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                cn = value;
                if (cn != null && cn.State != ConnectionState.Open)
                    cn.Open();
            }
        }
        public ClimberResults()
        {
            InitializeComponent();
        }

        private ClimberData climber = null;

        public ClimberData CurrentClimber
        {
            get { return climber; }
            set
            {
                climber = value;
                if (cn != null && value != null && !value.Loaded)
                    value.LoadData(cn);

                ShowResults();
            }
        }
        private void ShowResults()
        {
            lstResView.BeginInit();
            try
            {
                if (climber == null)
                {
                    lstResView.ItemsSource = null;
                    return;
                }
                DataTable dt = climber.GetResultsTable(String.Empty);
                lstResView.ItemsSource = dt.DefaultView;
            }
            finally { lstResView.EndInit(); }
        }
    }

    public sealed class FontStyleConverter : IValueConverter
    {
        public FontWeight HeaderW { get; set; }
        public FontWeight DefaultW { get; set; }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataRowView v = value as DataRowView;
            if (v == null)
                return DefaultW;
            string s = v[0] as string;
            if (s == null)
                return DefaultW;
            if (s.LastIndexOf(':') == s.Length - 1)
                return HeaderW;
            else
                return DefaultW;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
