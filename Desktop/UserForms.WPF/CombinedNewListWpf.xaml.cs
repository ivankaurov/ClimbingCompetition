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
using System.Globalization;
using ClimbingCompetition.WPF;

namespace ClimbingCompetition.UserForms.Wpf
{
    /// <summary>
    /// Interaction logic for CombinedNewListWpf.xaml
    /// </summary>
    public partial class CombinedNewListWpf : BaseWpfControl
    {
        public enum CombinedNewListType { NewList, BasedOnExisting, Cancel }

        public CombinedNewListWpf()
        {
            InitializeComponent();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
