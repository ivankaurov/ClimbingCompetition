using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition.WPF
{
    public class DialogRoutedEventArgs : System.Windows.RoutedEventArgs
    {
        readonly DialogResult dialogResult;

        public DialogRoutedEventArgs(DialogResult dialogResult) { this.dialogResult = dialogResult; }

        public DialogRoutedEventArgs(DialogResult dialogResult, System.Windows.RoutedEvent routedEvent)
            : base(routedEvent)
        { this.dialogResult = dialogResult; }

        public DialogRoutedEventArgs(DialogResult dialogResult, System.Windows.RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        { this.dialogResult = dialogResult; }

        public DialogResult DialogResult
        {
            get
            {
                return this.dialogResult;
            }
        }
    }

    public delegate void DialogRoutedEventHandler(object sender, DialogRoutedEventArgs e);
}
