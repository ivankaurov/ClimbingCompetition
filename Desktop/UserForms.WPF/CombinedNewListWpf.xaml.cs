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

using WF = System.Windows.Forms;

namespace ClimbingCompetition.UserForms.Wpf
{
    /// <summary>
    /// Interaction logic for CombinedNewListWpf.xaml
    /// </summary>
    public partial class CombinedNewListWpf : BaseWpfControl
    {
        public enum CombinedNewListType { NewList, BasedOnExisting, }

        public CombinedNewListWpf()
        {
            InitializeComponent();
        }

        public CombinedNewListType? Result { get; private set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.radioButtonBasedOnExisting.IsChecked.GetValueOrDefault(false))
            {
                this.Result = CombinedNewListType.BasedOnExisting;
                this.RaiseDialogClosedEvent(WF.DialogResult.OK);
            }
            else if (this.radioButtonNewList.IsChecked.GetValueOrDefault(false))
            {
                this.Result = CombinedNewListType.NewList;
                this.RaiseDialogClosedEvent(WF.DialogResult.OK);
            }
            else
            {
                MessageBox.Show("Способ создания протокола не выбран.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseDialogClosedEvent(WF.DialogResult.Cancel);
        }
    }
}
