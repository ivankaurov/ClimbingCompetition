using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ClimbingCompetition;
using ListShow.DsListShowTableAdapters;
using System.Threading;
using XmlApiData;

namespace ListShow
{
    public sealed partial class ListShowControl : BaseControl
    {
        private Mutex mOp = new Mutex();
        private static int refreshCount = 0;
        private int screenSize = 35;

        private const int ROW_SIZE = 25;

        private int currentList = 0;
        private List<ListForShow> lst = new List<ListForShow>();

        public ListShowControl()
        {
            InitializeComponent();
        }

        public override bool LoadData()
        {
            lst = ListForShow.GetAllLists(cn, projNum);
            currentList = 0;
            return (lst.Count > 0);
        }

        public override bool MoveNext()
        {
            if (currentList < 0)
                currentList = 0;
            if (lst.Count == 0 || currentList >= lst.Count)
                if (!LoadData())
                    return false;
            while (currentList < lst.Count)
            {
                if (SetNextView())
                    return true;
                currentList++;
            }
            return false;
        }

        public override bool RefreshCurrent()
        {
            mOp.WaitOne();
            try
            {
                if (lst.Count < 1)
                    return false;
                else if (currentList < 0)
                    currentList = 0;
                else if (currentList >= lst.Count)
                    return false;
                if (lst[currentList].Refresh(cn))
                    return RefreshCurrentScreen();
                else
                    return false;
            }
            finally { mOp.ReleaseMutex(); }
        }

        ListForShow.ListType currentListConfig = ListForShow.ListType.UNKNOWN;

        private bool SetNextView()
        {
            ListForShow lstC = lst[currentList];
            if (lstC.Current < 0)
                lstC.Current = 0;
            else
                lstC.Current += screenSize;

            if (!RefreshCurrent())
                return false;
            if (lstC.Current >= lstC.Length)
            {
                lstC.Current = -1;
                return false;
            }
            bool bRes = true;
            if (listView.InvokeRequired)
                listView.Invoke(new EventHandler(delegate { bRes = RefreshListView(lstC); }));
            else
                bRes = RefreshListView(lstC);

            return bRes;
        }

        private bool RefreshListView(ListForShow ls)
        {
            if (ls == null)
                return false;
            if (ls.Current == 0)
            {
                try
                {
                    if (this.ParentF != null)
                    {
                        if (this.ParentF.InvokeRequired)
                            this.ParentF.Invoke(new EventHandler(delegate
                            {
                                this.ParentF.Text = ls.Group + " " + ls.Round + " " + ls.Style;
                            }));
                        else
                            this.ParentF.Text = ls.Group + " " + ls.Round + " " + ls.Style;
                    }
                }
                catch { }
                var lstp = ls.GetListType(cn);
                if (lstp != currentListConfig || refreshCount < 2)
                {
                    listView.Items.Clear();
                    listView.Columns.Clear();
                    foreach (DataColumn dc in ls.Data.Columns)
                        listView.Columns.Add(dc.ColumnName, 40, HorizontalAlignment.Center);

                    listView.Columns[0].Width = 45;
                    listView.Columns[1].Width = 240;
                    listView.Columns[1].TextAlign = HorizontalAlignment.Left;
                    listView.Columns[2].Width = 57;
                    listView.Columns[3].Width = 70;
                    listView.Columns[4].Width = 200;
                    listView.Columns[ls.Data.Columns.Count - 1].Width = 50;

                    switch (lstp)
                    {
                        case ListForShow.ListType.BOULDER:
                            break;
                        case ListForShow.ListType.LEAD:
                            listView.Columns[5].Width = 74;
                            break;
                        case ListForShow.ListType.FLASH:
                            listView.Columns[5].Width = 74;
                            for (int i = 6; i < ls.Data.Columns.Count - 1; i++)
                                listView.Columns[i].Width = 74;
                            break;
                        case ListForShow.ListType.SPEED:
                            for (int ii = 5; ii < ls.Data.Columns.Count - 1; ii++)
                                listView.Columns[ii].Width = 100;
                            break;
                        case ListForShow.ListType.SPEED_2:
                            goto case ListForShow.ListType.SPEED;
                        default:
                            return false;
                    }
                    listView.View = View.Details;
                    currentListConfig = lstp;
                    if (refreshCount < 2)
                        refreshCount++;
                }
            }
            RefreshCurrentScreen();
            return true;
        }

        private bool RefreshCurrentScreen()
        {
            if (currentList < 0 || currentList >= lst.Count)
                return false;
            ListForShow ls = lst[currentList];
            bool res = false;
            if (listView.InvokeRequired)
                listView.Invoke(new EventHandler(delegate
                {
                    listView.Items.Clear();
                    for (int i = ls.Current; i < (ls.Current + screenSize) && i < ls.Data.Rows.Count; i++)
                    {
                        listView.Items.Add(ls.Data.Rows[i][0].ToString());
                        for (int k = 1; k < ls.Data.Columns.Count; k++)
                            try { listView.Items[i - ls.Current].SubItems.Add(ls.Data.Rows[i][k].ToString()); }
                            catch { }
                        if (!res)
                            res = true;
                    }
                }));
            else
            {
                listView.Items.Clear();
                for (int i = ls.Current; i < (ls.Current + screenSize) && i < ls.Data.Rows.Count; i++)
                {
                    listView.Items.Add(ls.Data.Rows[i][0].ToString());
                    for (int k = 1; k < ls.Data.Columns.Count; k++)
                        try { listView.Items[i - ls.Current].SubItems.Add(ls.Data.Rows[i][k].ToString()); }
                        catch { }
                        if (!res)
                            res = true;
                }
            }
            return res;
        }

        public sealed class ListForShow
        {
            private SqlConnection _cn = null;
            public static List<ListForShow> GetAllLists(SqlConnection cn, int projNum = 1)
            {
                if (cn == null)
                    throw new ArgumentNullException("cn");
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                ListsForShowTableAdapter lta = new ListsForShowTableAdapter();
                lta.Connection = cn;
                List<ListForShow> lst = new List<ListForShow>();
                foreach (DsListShow.ListsForShowRow row in lta.GetData(projNum))
                    lst.Add(new ListForShow(row.iid, row.grp, row.style, row.round, row.routeNumber, cn));
                return lst;
            }
            private int iid, routeNumber, current = -1;
            public int Iid { get { return iid; } }
            public int RouteNumber { get { return routeNumber; } }
            public int Current { get { return current; } set { current = value; } }
            public int Length { get { return (data == null) ? 0 : data.Rows.Count; } }

            private string group, style, round;
            public string Group { get { return group; } }
            public string Style { get { return style; } }
            public string Round { get { return round; } }

            private DataTable data = null;
            public DataTable Data { get { return data; } set { data = value; } }

            public ListTypeEnum ListTypeDB { get; private set; }

            public enum ListType { FLASH, LEAD, SPEED, SPEED_2, BOULDER, UNKNOWN }

            public ListType GetListType(SqlConnection _cn = null)
            {
                SqlConnection cn = _cn == null ? this._cn : _cn;
                try
                {
                    ListTypeDB = StaticClass.GetListType(iid, cn, null);
                }
                catch { ListTypeDB = ListTypeEnum.Unknown; }
                switch (ListTypeDB)
                {
                    case ListTypeEnum.LeadFlash:
                        return ListType.FLASH;
                    case ListTypeEnum.LeadSimple:
                        return ListType.LEAD;
                    case ListTypeEnum.SpeedFinal:
                        return ListType.SPEED;
                    case ListTypeEnum.SpeedQualy:
                        return ListType.SPEED;
                    case ListTypeEnum.SpeedQualy2:
                        if (cn != null && ((SettingsForm.GetSpeedRules(cn) & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds))
                            return ListType.SPEED_2;
                        else
                            return ListType.SPEED;
                    case ListTypeEnum.BoulderSuper:
                        return ListType.LEAD;
                    case ListTypeEnum.BoulderSimple:
                        return ListType.BOULDER;
                    default:
                        return ListType.UNKNOWN;
                }
            }
#if FULL
            private MultiRouteBoulder mrb = null;
#endif
            public ListForShow(int iid, string group, string style, string round, int routeNumber, SqlConnection cn)
            {
                this._cn = cn;
                this.iid = iid;
                this.group = group;
                this.style = style;
                this.round = round;
                this.routeNumber = routeNumber;
#if FULL
                if (this.GetListType(this._cn) == ListType.BOULDER)
                    mrb = new MultiRouteBoulder(iid, routeNumber);
#endif
            }

            public bool Refresh(SqlConnection _cn = null)
            {
                SqlConnection cn = _cn == null ? this._cn : _cn;
                SqlDataAdapter da = new SqlDataAdapter(new SqlCommand());
                da.SelectCommand.Connection = cn;
                da.SelectCommand.Parameters.Add("@iid", SqlDbType.Int);
                da.SelectCommand.Parameters[0].Value = iid;
                Boolean preQfClimb = SettingsForm.GetPreQfClimb(da.SelectCommand.Connection, iid, da.SelectCommand.Transaction);
                switch (GetListType(cn))
                {
#if FULL
                    case ListType.FLASH:
                        bool showEverybody = SettingsForm.GetFlashShowMode(cn) == SettingsForm.FlashShowMode.BTH;
                        int timeRoutes;
                        KeyValuePair<int, bool>[] lstRRR;
                        data = StaticClass.FillResFlash(iid, cn, false, out routeNumber, showEverybody, out timeRoutes, out lstRRR);
                        try
                        {
                            if (data.Columns.IndexOf("pos") > -1)
                                data.Columns.Remove("pos");
                            if (data.Columns.IndexOf("№") > -1)
                                data.Columns.Remove("№");
                        }
                        catch { }
                        return true;
                    case ListType.LEAD:
                        da.SelectCommand.CommandText = @"
                          SELECT r.posText AS Место, p.surname+' '+p.name AS [Фамилия, Имя], 
                                 p.age AS [Г.р.], p.qf AS Разряд, dbo.fn_getTeamName(p.iid, r.list_id) AS Команда, r.resText AS [Рез-т],
                                 r.qf AS [Кв.] 
                            FROM routeResults r(NOLOCK)
                            JOIN Participants p(NOLOCK) ON r.climber_id = p.iid
                            JOIN Teams t(NOLOCK)ON p.team_id = t.iid
                           WHERE r.list_id = @iid
                             AND r.preQf = 0
                        ORDER BY r.pos, r.res DESC, r.start";
                        break;
                    case ListType.BOULDER:
                        if (mrb == null)
                            mrb = new MultiRouteBoulder(iid, cn);
                        else
                            mrb.cn = cn;
                        mrb.FillData();
                        data = mrb.GetListFiltered(MultiRouteBoulder.ListType.Full);
                        foreach (DataColumn dc in data.Columns)
                        {
                            string sCheck = dc.ColumnName.ToLower();
                            if (sCheck.IndexOf("поп") > -1)
                            {
                                if (sCheck.IndexOf("тр") > -1)
                                    dc.ColumnName = "П.Тр.";
                                else if (sCheck.IndexOf("бон") > -1)
                                    dc.ColumnName = "П.Б.";
                            }
                        }
                        return true;
                    //da.SelectCommand.CommandText = 
                    //    "  SELECT CASE WHEN r.pos = " + SortingClass.DSQ_POS.ToString() + " THEN 'дискв.'" +
                    //    "              WHEN r.pos = " + SortingClass.DNS_POS.ToString() + " THEN 'н/я'" +
                    //    "              ELSE r.posText END AS Место, " +
                    //    "         p.surname+' '+p.name AS [Фамилия, Имя], " +
                    //    "         p.age AS [Г.р.], p.qf AS Разряд, t.name AS Команда, ";
                    //for (int i = 1; i <= routeNumber; i++)
                    //    da.SelectCommand.CommandText +=
                    //        "     r.top" + i.ToString() + " AS T" + i.ToString() + ", " +
                    //        "     r.bonus" + i.ToString() + " AS B" + i.ToString() + ", ";
                    //da.SelectCommand.CommandText += 
                    //    "         r.tops AS [Т], r.topAttempts AS [Тп], r.bonuses AS [Б], " +
                    //    "         r.bonusAttempts AS [Бп], r.Qf AS [Кв.] " +
                    //    "    FROM boulderResults r(NOLOCK) " +
                    //    "    JOIN Participants p(NOLOCK) ON r.climber_id=p.iid " +
                    //    "    JOIN Teams t(NOLOCK) ON p.team_id=t.iid " +
                    //    "   WHERE r.list_id=" + iid.ToString() +
                    //    "     AND r.preQf = 0 " +
                    //    "ORDER BY r.pos, r.res DESC, r.start";
                    //break;
#endif
                    case ListType.SPEED_2:
                        da.SelectCommand.CommandText =
                                "  SELECT r.posText AS Место, p.surname+' '+p.name AS [Фамилия, Имя], " +
                                "         p.age AS [Г.р.], p.qf AS Разряд, dbo.fn_getTeamName(p.iid, r.list_id) AS Команда, " +
                                "         dbo.fn_get1st(p.iid, r.list_id) AS [Квал.1]," +
                                "         r.route1_text AS [Трасса 1],r.route2_text AS[Трасса 2], r.resText AS [Квал.2]," +
                                "         dbo.fn_getBest(r.iid) AS [Итог]," +
                                "         r.qf AS [Кв.] " +
                                "    FROM speedResults r(NOLOCK) " +
                                "    JOIN Participants p(NOLOCK) ON r.climber_id=p.iid " +
                                "    JOIN Teams t(NOLOCK) ON p.team_id=t.iid " +
                                "   WHERE r.list_id=" + iid.ToString() +
                                (preQfClimb ? String.Empty : "     AND r.preQf = 0 ") +
                                "ORDER BY (CASE WHEN r.qf IS NULL THEN 1 WHEN r.qf = '' THEN 1 ELSE 0 END), " +
                                "         r.pos, dbo.fn_getBestRes(r.iid), r.start";
                        break;
                    case ListType.SPEED:
                        da.SelectCommand.CommandText =
                            "  SELECT r.posText AS Место, p.surname+' '+p.name AS [Фамилия, Имя], " +
                            "         p.age AS [Г.р.], p.qf AS Разряд, dbo.fn_getTeamName(p.iid, r.list_id) AS Команда, " +
                            "         r.route1_text AS [Трасса 1],r.route2_text AS[Трасса 2], r.resText AS [Сумма]," +
                            "         r.qf AS [Кв.] " +
                            "    FROM speedResults r(NOLOCK) " +
                            "    JOIN Participants p(NOLOCK) ON r.climber_id=p.iid " +
                            "    JOIN Teams t(NOLOCK) ON p.team_id=t.iid " +
                            "   WHERE r.list_id=" + iid.ToString() +
                            (preQfClimb ? String.Empty : "     AND r.preQf = 0 ") +
                            "ORDER BY (CASE WHEN r.qf IS NULL THEN 1 WHEN r.qf = '' THEN 1 ELSE 0 END), " +
                            "         r.pos, r.res,r.start";
                        break;
                    default:
                        data = null;
                        GC.Collect();
                        return false;
                }
                data = new DataTable();
                da.Fill(data);
                return true;
            }
        }

        protected override void BaseControl_Resize(object sender, EventArgs e)
        {
            try
            {
                screenSize = ((this.ClientSize.Height - ROW_SIZE) / ROW_SIZE);
                if (screenSize < 1)
                    screenSize = 1;
                RefreshCurrent();
            }
            catch (Exception ex)
            {
                if (this.ParentF != null)
                    this.ParentF.WriteLog(ex, false);
                else
                    ShowForm.WriteLogStatic(ex.ToString(), null, false);
            }
        }

    }
}
