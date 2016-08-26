using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ListShow.DsListShowTableAdapters;

namespace ListShow
{
    public partial class BasePhotoControl : BaseControl
    {
        public struct ListStruct
        {
            public int iid;
            public string name;
        }
        protected List<ListStruct> ShowingLists = new List<ListStruct>();
        protected int currentList = 0;
        public enum ClimbingStyleType { Трудность, Скорость }
        public ClimbingStyleType ClimbingStyle { get; set; }

        public BasePhotoControl()
        {
            InitializeComponent();
        }

        protected virtual void DoLayout() { }

        public override bool LoadData()
        {
            ShowingLists = LoadData(cn, ClimbingStyle, projNum);
            currentList = 0;
            return (ShowingLists.Count > 0);
        }

        public static List<ListStruct> LoadData(SqlConnection cn, ClimbingStyleType style, int projNum)
        {
            List<ListStruct> rV = new List<ListStruct>();
            ListsForShowTableAdapter lta = new ListsForShowTableAdapter();
            lta.Connection = cn;
            DsListShow.ListsForShowDataTable dt = lta.GetData(projNum);
            foreach (DsListShow.ListsForShowRow row in dt.Rows)
                if (row.showPhoto && row.style == style.ToString())
                {
                    ListStruct lstr;
                    lstr.iid = row.iid;
                    lstr.name = row.grp + " " + row.round + " " + row.style;
                    rV.Add(lstr);
                }
            return rV;
        }

        protected bool lastPassed = false;

        public override bool MoveNext()
        {
            if (lastPassed)
            {
                lastPassed = false;
                return false;
            }
            if (ShowingLists.Count == 0 || currentList >= ShowingLists.Count)
                if (!LoadData())
                    return false;

            while (currentList < ShowingLists.Count)
                if (SetCurrentList())
                {
                    if (currentList == (ShowingLists.Count - 1))
                        lastPassed = true;
                    currentList++;
                    return true;
                }
                else
                    currentList++;
            return false;


        }

        protected ClimberData currentClimber = ClimberData.Empty;
        protected virtual bool SetCurrentList()
        {
            if (currentList < 0 || currentList >= ShowingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing FROM lists(NOLOCK) WHERE iid=" + ShowingLists[currentList].iid.ToString();
            object oTmp = cmd.ExecuteScalar();
            if (oTmp == null || oTmp == DBNull.Value)
                return false;
            int curClm = Convert.ToInt32(oTmp);
            if (currentClimber != null && curClm == currentClimber.Iid)
                return true;
            ClimberData c = new ClimberData(curClm);
            if (!c.LoadData(cn))
                return false;
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate { SetClimberData(c); }));
            else
                SetClimberData(c);
            currentClimber = c;
            return true;
        }

        protected virtual void SetClimberData(ClimberData clm)
        {
            ClimberData c;
            if (clm == null)
                c = ClimberData.Empty;
            else
                c = clm;
            tbPartAge.Text = c.AgeStr;
            tbPartGroup.Text = c.Group;
            tbPartNumber.Text = c.IidStr;
            tbPArtQf.Text = c.Qf;
            tbPartSurname.Text = c.Name;
            tbPartTeam.Text = c.Team;
            pbPhoto.Image = c.Photo;
            c.SetToListBox(listBox1);
        }

        public override bool RefreshCurrent()
        {
            return true;
        }

        private void BasePhotoControl_Load(object sender, EventArgs e)
        {
            DoLayout();
        }
    }
}
