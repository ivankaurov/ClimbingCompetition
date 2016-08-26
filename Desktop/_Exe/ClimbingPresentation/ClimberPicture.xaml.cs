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
using System.Data;
using ListShow;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ClimberPicture.xaml
    /// </summary>
    public partial class ClimberPicture : UserControl
    {
        public ClimberPicture()
        {
            InitializeComponent();
        }

        private SqlConnection cn;
        public SqlConnection Connection { get { return cn; } set { cn = value; } }

        private ClimberData current = null;

        public ClimberData CurrentClimber
        {
            get { return current; }
            set
            {
                current = value;
                SafeLoad();
            }
        }
        private bool empty = true;
        public bool IsEmpty { get { return empty; } }

        public int CurrentClimberIid
        {
            get { return (current == null ? -1 : current.Iid); }
            set
            {
                if (value < 1)
                    CurrentClimber = null;
                else
                    CurrentClimber = new ClimberData(value);
            }
        }

        private void SafeLoad()
        {
            if (img.Dispatcher.CheckAccess())
                LoadToShow();
            else
                img.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { LoadToShow(); });
        }

        private void LoadToShow()
        {
            MemoryStream imageSource;
            if (current == null || current.Iid < 1 || current.NoPhoto)
            {
                empty = true;
                imageSource = ListShow.ClimberData.GetEmptyImage();
            }
            else
            {
                if (!current.Loaded)
                {
                    if (cn == null)
                        return;
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    current.LoadData(cn);
                }
                imageSource = current.GetImageStream();
                empty = false;
            }
            this.BeginInit();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = imageSource;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            imageSource.Close();
            img.Source = bmp;

            this.Visibility = (empty ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);

            this.EndInit();
        }
    }
}
