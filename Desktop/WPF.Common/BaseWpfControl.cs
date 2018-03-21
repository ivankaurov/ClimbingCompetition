using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using WF = System.Windows.Forms;

namespace ClimbingCompetition.WPF
{
    public class BaseWpfControl : UserControl
    {
        private static readonly RoutedEvent DialogClosedRoutedEvent =
            EventManager.RegisterRoutedEvent("DialogClosed", RoutingStrategy.Bubble, typeof(DataRoutedEventHandler<WF.DialogResult>), typeof(BaseWpfControl));

        public event DataRoutedEventHandler<WF.DialogResult> DialogClosed
        {
            add { AddHandler(DialogClosedRoutedEvent, value); }
            remove { RemoveHandler(DialogClosedRoutedEvent, value); }
        }

        protected void RaiseDialogClosedEvent(WF.DialogResult dialogResult)
        {
            var eventArgs = new DataRoutedEventArgs<WF.DialogResult>(dialogResult, DialogClosedRoutedEvent);
            RaiseEvent(eventArgs);
        }
    }
}
