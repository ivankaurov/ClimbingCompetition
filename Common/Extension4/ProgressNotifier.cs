using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    public class ProgressNotifier
    {
        public event EventHandler<DataEventArgs<int, String>> InitProgress;

        public void OnInitProgress(int percentage, String message)
        {
            var evT = Interlocked.CompareExchange(ref InitProgress, null, null);
            if (evT != null)
                Task.Factory.StartNew(() => InitProgress(null, new DataEventArgs<int, string>(percentage, message)));
        }
    }
}
