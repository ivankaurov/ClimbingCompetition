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
    /// Interaction logic for PhotoControl.xaml
    /// </summary>
    public partial class PhotoControl : UserControl
    {
        public PhotoControl()
        {
            InitializeComponent();
            ProjNum = 1;
        }
        private SqlConnection cn=null;
        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                if (value != null && value.State != System.Data.ConnectionState.Open)
                    value.Open();
                cn = value;
                clmResults.Connection = clmPhoto.Connection = clmPresentation.Connection = cn;
            }
        }
        private ListShow.ClimberData cClimber = null;

        private int CurrentClimber
        {
            get
            {
                if (cClimber == null)
                    return -1;
                return cClimber.Iid;
            }
            set
            {
                cClimber = new ListShow.ClimberData(value);
                if (cn != null)
                    cClimber.LoadData(cn);
                clmResults.CurrentClimber = clmPhoto.CurrentClimber = clmPresentation.CurrentClimber = cClimber;
            }
        }

        public int ProjNum { get; set; }
        public List<ListShow.BasePhotoControl.ListStruct> showingLists = new List<ListShow.BasePhotoControl.ListStruct>();
        int currentList = -1;
        public bool LoadData()
        {
            if (cn == null)
                return false;
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            showingLists = ListShow.BasePhotoControl.LoadData(cn, ListShow.BasePhotoControl.ClimbingStyleType.Трудность, ProjNum);
            currentList = -1;
            return ScrollToNext();
        }

        public bool ScrollToNext()
        {
            currentList++;
            if (currentList >= showingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing FROM lists(NOLOCK) WHERE iid=" + showingLists[currentList].iid.ToString();
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return ScrollToNext();
            CurrentClimber = Convert.ToInt32(oTmp);
            return true;
        }
    }
}
