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
using System.Timers;
using System.Windows.Threading;
using System.Threading;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const double REFRESH_TIME = 10000.0;
        private const int ITER_COUNT = 3;

        private const string LEAD_PHOTO = "LEAD_PHOTO";
        private const string SPEED_PHOTO = "SPEED_PHOTO";

        private int currentIterCount = ITER_COUNT;

        System.Timers.Timer t = new System.Timers.Timer(3000);
        private int EventCounter = 3;
        private Mutex eventMutex = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
            t.AutoReset = true;
            if (t.Enabled)
                t.Stop();
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);

            uiElems.Add(lstResView);
            uiElems.Add(leadPhoto);
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            NextEvent(false);
        }

        bool shortShow = false;
        void NextEvent(bool resetCounter)
        {
            eventMutex.WaitOne();
            try
            {
                t.Stop();
                try
                {
                    if (resetCounter || shortShow)
                    {
                        EventCounter = currentIterCount;
                        if (shortShow)
                            shortShow = false;
                    }
                    else
                    {
                        EventCounter++;
                        if (EventCounter > currentIterCount)
                            EventCounter = 0;
                    }
                    if (EventCounter == currentIterCount)
                    {
                        if (!ScrollNext())
                        {
                            if (lstResView.Dispatcher.CheckAccess())
                            {
                                lstResView.ScrollToTop();
                                if (MoveToNextElem())
                                    ReloadStack();
                            }
                            else
                                lstResView.Dispatcher.Invoke(DispatcherPriority.Normal,
                                    (ThreadStart)delegate
                                {
                                    lstResView.ScrollToTop();
                                    if (MoveToNextElem())
                                        ReloadStack();
                                });
                        }

                    }
                    else
                    {
                        if (lstResView.Dispatcher.CheckAccess())
                            RefreshCurrent();
                        else
                            lstResView.Dispatcher.Invoke(DispatcherPriority.Normal,
                                (ThreadStart)delegate
                            {
                                RefreshCurrent();
                            });
                    }
                }
                finally { t.Start(); }
            }
            catch (ThreadAbortException exT) { throw exT; }
            catch (Exception ex) { ProcessException(ex); }
            finally { eventMutex.ReleaseMutex(); }
        }

        //private delegate void ScrollLineDelegate(ScrollElem scv);
        //private delegate double GetNextScroll(ScrollViewer scv);
        //private delegate void MoveToPos(ScrollViewer scv, double pos);

        private class ScrollElem
        {
            public ScrollViewer Viewer { get; private set; }
            public double MaxToScroll { get; private set; }
            public ScrollElem(ScrollViewer viewer, double maxToScroll)
            {
                this.Viewer = viewer;
                this.MaxToScroll = maxToScroll;
            }
        }

        private void ScrollLine(ScrollElem sel)
        {
            sel.Viewer.Dispatcher.Invoke(DispatcherPriority.Normal,
                (ParameterizedThreadStart)delegate(object objP)
            {
                ScrollElem e = objP as ScrollElem;
                if (e == null || e.Viewer.VerticalOffset >= e.MaxToScroll)
                    return;
                e.Viewer.LineDown();
                Thread thr = new Thread(new ParameterizedThreadStart(delegate(object obj)
                {
                    ScrollElem s = (ScrollElem)obj;
                    Thread.Sleep(30);
                    ScrollLine(s);
                }));
                thr.Start(e);
            }, sel);
        }

        List<FrameworkElement> uiElems = new List<FrameworkElement>();

        private bool ScrollNext()
        {
            FrameworkElement elemToScroll;
            if (Dispatcher.CheckAccess())
            {
                elemToScroll = (from q in uiElems
                                where q.Visibility == System.Windows.Visibility.Visible
                                select q).First();
            }
            else
            {
                elemToScroll = (FrameworkElement)Dispatcher.Invoke(DispatcherPriority.Normal,
                    (Func<FrameworkElement>)delegate
                {
                    return (from q in uiElems
                            where q.Visibility == System.Windows.Visibility.Visible
                            select q).First();
                });
            }
            return ScrollNext(elemToScroll);
        }

        private bool ScrollNext(FrameworkElement fel)
        {
            int nMx = 0;
            Visibility vis;
            if (fel.Dispatcher.CheckAccess())
                vis = fel.Visibility;
            else
                vis = (Visibility)fel.Dispatcher.Invoke(DispatcherPriority.Send,
                (Func<FrameworkElement, Visibility>)(f => f.Visibility), fel);

            if (vis != System.Windows.Visibility.Visible)
                return false;

            PhotoControl pc = fel as PhotoControl;
            if (pc != null)
            {
                if (pc.Dispatcher.CheckAccess())
                    return pc.ScrollToNext();
                else
                    return (bool)pc.Dispatcher.Invoke(DispatcherPriority.Send,
                        (Func<PhotoControl, bool>)(p => p.ScrollToNext()), pc);
            }
            ScrollViewer scv = fel as ScrollViewer;
            if (scv == null)
                return false;
            double curScr;
            if (scv.Dispatcher.CheckAccess())
                curScr = scv.VerticalOffset;
            else
                curScr = (double)scv.Dispatcher.Invoke(DispatcherPriority.Send,
                    (Func<ScrollViewer, double>)(lamArg => lamArg.VerticalOffset), scv);




            var q = scv.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                (Func<ScrollViewer, double, bool>)delegate(ScrollViewer sv, double pos)
            {
                sv.BeginInit();
                sv.PageDown();
                sv.EndInit();
                return true;
            }, scv, new object[] { 0.0 });

            nMx = 0;
            while (q.Status != DispatcherOperationStatus.Completed && nMx < 1000)
            {
                nMx++;
                Thread.Sleep(2);
            }
            if (q.Status != DispatcherOperationStatus.Completed)
            {
                q.Abort();
                return false;
            }

            double maxToScroll = (double)scv.Dispatcher.Invoke(
                DispatcherPriority.Send,
                (Func<ScrollViewer, double>)(lamArg => lamArg.VerticalOffset), scv);

            if (curScr >= maxToScroll)
                return false;

            q = scv.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                (Func<ScrollViewer, double, bool>)delegate(ScrollViewer sv, double pos)
            {
                sv.ScrollToVerticalOffset(pos);
                return true;
            }, scv, new object[] { curScr });

            nMx = 0;
            while (q.Status != DispatcherOperationStatus.Completed && nMx < 1000)
            {
                Thread.Sleep(2);
                nMx++;
            }
            if (q.Status != DispatcherOperationStatus.Completed)
            {
                q.Abort();
                return false;
            }

            var se = new ScrollElem(scv, maxToScroll);
            ScrollLine(se);
            return true;
        }

        public static SqlConnection cn { get; private set; }
        private int pNum;
        public int ProjNum
        {
            get { return pNum; }
            private set
            {
                pNum = value;
                leadPhoto.ProjNum = pNum;
            }
        }

        List<object> controlStack = new List<object>();
        private int currentElem = 0;

        private Brush bgr = Brushes.Black;

        string curStringRound = string.Empty;

        private void RefreshCurrent()
        {
            RefreshCurrent(-1);
        }

        private void RefreshCurrent(int elem)
        {
            if (controlStack.Count < 0)
                return;
            if (elem < 0)
                elem = currentElem;
            if (elem >= controlStack.Count)
                elem = controlStack.Count - 1;
            currentElem = elem;
            SetListView(elem);
        }

        private bool MoveToNextElem()
        {
            if (controlStack.Count < 1)
            {
                if (currentElem < 0)
                    currentElem = 0;
                return true;
            }
            bool needToReload = false;
            if (currentElem < 0)
                currentElem = controlStack.Count - 1;
            int elemToSet = currentElem;
            do
            {
                elemToSet++;
                if (elemToSet >= controlStack.Count)
                {
                    elemToSet = 0;
                    needToReload = true;
                }
                if (CheckElemValidity(elemToSet))
                {
                    currentElem = elemToSet;
                }
            } while (elemToSet != currentElem);
            SetListView(elemToSet);
            return needToReload;
        }

        private void SetListView(int elemToSet)
        {
            if (controlStack[elemToSet] is ListShow.ListShowControl.ListForShow)
            {
                SetElemVisible(lstResView);
                //leadPhoto.Visibility = System.Windows.Visibility.Collapsed;
                //lstResView.Visibility = System.Windows.Visibility.Visible;


                var lfs = (ListShow.ListShowControl.ListForShow)controlStack[elemToSet];

                string rnd = lfs.Round.ToLower();

                QualifyHighlighterConverter.HighlightMedals = (rnd == "финал" || rnd == "суперфинал");

                if (cn.State != ConnectionState.Open)
                    cn.Open();
                lfs.Refresh(cn);

                lstRes.ItemsSource = lfs.Data.DefaultView;

                //if (curStringRound != lfs.GetStyleRoundString())
                //{
                curStringRound = lfs.GetStyleRoundString();
                if (lstRes.View is GridView)
                {
                    lstRes.BeginInit();
                    try
                    {
                        var gV = (GridView)lstRes.View;
                        gV.Columns.Clear();
                        for (int i = 0; i < lfs.Data.Columns.Count; i++)
                        {
                            DataColumn col = lfs.Data.Columns[i];
                            if (col.ColumnName.ToLower() == "кв.")
                                continue;
                            GridViewColumn gc = new GridViewColumn();
                            gc.Header = col.ColumnName;
                            gc.DisplayMemberBinding = new Binding("[" + i.ToString() + "]");
                            gV.Columns.Add(gc);
                        }
                    }
                    finally { lstRes.EndInit(); }
                }
                Dispatcher.Invoke((Action)(() =>
                {
                    try { this.Title = lfs.GetListHeader(); }
                    catch { }
                }));
                //}
            }
            else if (controlStack[elemToSet] is string)
            {
                switch (controlStack[elemToSet] as string)
                {
                    case LEAD_PHOTO:
                        SetElemVisible(leadPhoto);
                        //if (leadPhoto.Visibility == System.Windows.Visibility.Collapsed)
                        //{
                        //    lstResView.Visibility = System.Windows.Visibility.Collapsed;
                        //    leadPhoto.Visibility = System.Windows.Visibility.Visible;
                        //}
                        break;
                }
            }
        }

        private bool CheckElemValidity(int elemIndex)
        {
            if (elemIndex < 0 || elemIndex >= controlStack.Count)
                return false;
            if (controlStack[elemIndex] as string == LEAD_PHOTO)
            {
                return leadPhoto.LoadData();
            }
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool? res = null;
            while (res == null)
            {
                ConnectionSettings csForm = new ConnectionSettings();
                cn = null;
                res = csForm.ShowDialog();
                if (res.Value)
                {
                    cn = csForm.CreatedConnection;
                    try
                    {
                        cn.Open();
                        leadPhoto.Connection = cn;
                        ReloadStack();
                        t.Start();
                        textLine.ConnectionString = cn.ConnectionString;
                        textLine.IsRunning = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка открытия соединения:\r\n" +
                            ex.Message);
                        res = null;
                    }
                }
                else
                    this.Close();
            }

        }

        private void ReloadStack()
        {
            try
            {
                LoadRefreshSpeed();
                controlStack.Clear();
                ProjNum = ClimbingPresentation.Properties.Settings.Default.sqlCurPrj;
                foreach (var o in ListShow.ListShowControl.ListForShow.GetAllLists(cn, ProjNum))
                    controlStack.Add(o);
                controlStack.Add(LEAD_PHOTO);
                currentElem = -1;
                MoveToNextElem();
            }
            catch (Exception ex) { ProcessException(ex); }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
#if !DEBUG
            var dgRes = MessageBox.Show(this, "Вы уверены, что хотите выйти из приложения?","Выйти?",
                 MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dgRes == MessageBoxResult.No || dgRes == MessageBoxResult.None || dgRes == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
#endif
            if (thrRefr != null && thrRefr.IsAlive)
                thrRefr.Abort();
            if(cn!= null)
                try
                {
                    if (cn.State != ConnectionState.Closed)
                        cn.Close();
                }
                catch { }
            textLine.DisposeConnection();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool handledBak = e.Handled;
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Down:
                    InvokeScrollJob(lstResView.LineDown);
                    return;
                case Key.Up:
                    InvokeScrollJob(lstResView.LineUp);
                    return;
                case Key.PageDown:
                    InvokeScrollJob(lstResView.PageDown);
                    return;
                case Key.PageUp:
                    InvokeScrollJob(lstResView.PageUp);
                    return;
                case Key.Home:
                    InvokeScrollJob(lstResView.ScrollToTop);
                    return;
                case Key.End:
                    InvokeScrollJob(lstResView.ScrollToBottom);
                    return;
                case Key.Left:
                    InvokeScrollJob(lstResView.LineLeft);
                    return;
                case Key.Right:
                    InvokeScrollJob(lstResView.LineRight);
                    return;

                case Key.Space:
                    if (thrRefr != null && thrRefr.IsAlive)
                        return;
                    thrRefr = new Thread(new ThreadStart(delegate { NextEvent(true); }));
                    thrRefr.Priority = ThreadPriority.Highest;
                    thrRefr.Start();
                    return;
            }
            e.Handled = handledBak;
        }

        private Thread thrRefr = null;

        private delegate void ScrollJob();

        private void InvokeScrollJob(ScrollJob job)
        {
            eventMutex.WaitOne();
            try
            {
                bool hBak = t.Enabled;
                if (hBak)
                    t.Stop();
                if (lstResView.Dispatcher.CheckAccess())
                    job();
                else
                    lstResView.Dispatcher.Invoke(DispatcherPriority.Send, job);
                if (hBak)
                    t.Start();
            }
            finally { eventMutex.ReleaseMutex(); }
        }

        private void LoadRefreshSpeed()
        {
            bool tRunning = t.Enabled;
            if (tRunning)
                t.Stop();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                t.Interval = (double)ClimbingCompetition.SettingsForm.GetRefreshSpeed(cn) * 1000;
                currentIterCount = ClimbingCompetition.SettingsForm.GetMoveNext(cn) - 1;
            }
            catch (Exception ex) { ProcessException(ex); }
            finally
            {
                if (tRunning)
                    t.Start();
            }
        }

        private void ProcessException(Exception ex)
        {
            if (ex is ThreadAbortException)
                throw ex;
#if DEBUG
            throw ex;
#else
            
#endif
        }


        int? backupRow = null;
        private void SetElemVisible(FrameworkElement elem)
        {


            if (elem.Visibility == System.Windows.Visibility.Visible)
                return;
            mainGrid.BeginInit();
            List<FrameworkElement> toInvis = new List<FrameworkElement>();
            if (elem == lstResView)
            {
                toInvis.Add(leadPhoto);
            }
            else if (elem == leadPhoto)
            {
                toInvis.Add(lstResView);
            }
            foreach (var e in toInvis)
            {
                e.BeginInit();
                if (e.Visibility == System.Windows.Visibility.Visible && backupRow != null)
                {
                    Grid.SetRow(e, backupRow.Value);
                    backupRow = null;
                }
                e.Visibility = System.Windows.Visibility.Collapsed;
                e.EndInit();
            }
            elem.BeginInit();
            backupRow = Grid.GetRow(elem);
            Grid.SetRow(elem, 1);
            elem.Visibility = System.Windows.Visibility.Visible;
            elem.EndInit();
            mainGrid.EndInit();
        }
    }

    public sealed class QualifyHighlighterConverter : IValueConverter
    {
        private static bool highlightMedals = true;
        public static bool HighlightMedals { get { return highlightMedals; } set { highlightMedals = value; } }
        public Brush HighlightBrush { get; set; }
        public Brush DefaultBrush { get; set; }
        public Brush MedalsBrush { get; set; }
        public Brush GoldenBrush { get; set; }
        public Brush SilverBrush { get; set; }
        public Brush BronzeBrush { get; set; }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DataRowView))
                return DefaultBrush;
            DataRow row = ((DataRowView)value).Row;
            int qfCol = row.Table.Columns.IndexOf("Кв.");
            if (qfCol < 0)
                return DefaultBrush;
            object o = row[qfCol];
            if (o == null)
                return DefaultBrush;
            var s = o.ToString().ToLower();
            if (s.IndexOf("q") > -1)
                return HighlightBrush;
            if (MedalsBrush == null || !highlightMedals)
                return DefaultBrush;
            var colCl = from DataColumn c in row.Table.Columns
                        orderby c.Ordinal
                        select c;
            var cl = colCl.First(c => c.ColumnName.ToLower().IndexOf("мест") > -1
                                || c.ColumnName.ToLower().IndexOf("м.") == 0);
            if (cl == null)
                return DefaultBrush;
            o = row[cl];
            if (o == null || o == DBNull.Value)
                return DefaultBrush;
            int pl;
            if (int.TryParse(o.ToString(), out pl))
            {
                switch (pl)
                {
                    case 1:
                        return GoldenBrush;
                    case 2:
                        return SilverBrush;
                    case 3:
                        return BronzeBrush;
                    default:
                        return DefaultBrush;
                }
            }
            else
                return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
