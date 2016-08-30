// <copyright file="ResultService.aspx.cs">
// Copyright © 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

﻿//#define NO_LISTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ClimbingCompetition.Online;
using WebClimbing.src;
using System.Web.Configuration;

namespace WebClimbing
{
    public partial class _PResultService : BasePage
    {
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int listID = -1;
                long curCompIdP = -1;
                try
                {
                    if (!IsPostBack)
                    {
                        listID = (int)Request.GetLongParam(Constants.PARAM_IID);
                        if (listID > 0)
                        {
                            var compLst = (from l in dc.ONLlists
                                           where l.iid == listID
                                           select l.comp_id).ToArray();
                            if (compLst.Length > 0)
                            {
                                curCompIdP = compLst[0];
                                this.SetCompID(curCompIdP);
                            }
                        }
                    }
                }
                catch { }
                base.Page_Load(sender, e, curCompIdP < 1);
                if (IsPostBack)
                    return;
#if !DEBUG
            Tasks.CheckDDL(compID);
#endif
                //compID = this.GetCompID();
                //if (compID < 0)
                //{
                //    Response.Redirect(ExtensionMethods.DEFAULT_REDIRECT);
                //    return;
                //}

                //Tasks.CheckAndSend();

                int nnC = (from ppq in dc.ONLlists
                           where ppq.comp_id == compID
                           && ppq.group_id != null
                           select ppq).Count();

                if (nnC < 1)
                {

                    Response.Redirect("~/ClimbersList.aspx?" + Constants.PARAM_COMP_ID + "=" + compID.ToString());
                    return;

                }


                GetStyles();

                if (listID > 0)
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();

                    var stGroup = from p in dc.ONLlists
                                  where p.iid == listID
                                  && p.group_id != null
                                  select new { p.style, group_id = p.group_id.Value };
                    if (stGroup.Count() < 1)
                        return;
                    var sg = stGroup.First();
                    for (int i = 0; i < styles.Items.Count; i++)
                        if (styles.Items[i].Value == sg.style)
                        {
                            styles.SelectedIndex = i;
                            GetGroups();
                            for (int j = 0; j < groups.Items.Count; j++)
                                if (groups.Items[j].Value == sg.group_id.ToString())
                                {
                                    groups.SelectedIndex = j;
                                    GetRounds();
                                    for (int k = 0; k < rounds.Items.Count; k++)
                                        if (rounds.Items[k].Value == listID.ToString())
                                        {
                                            rounds.SelectedIndex = k;
                                            GetList();
                                            break;
                                        }
                                    break;
                                }
                            break;
                        }
                }
            }
            catch (Exception ex) { lblTitle.Text = ex.Message; }
        }

        string Time
        {
            get
            {
                DateTime now = DateTime.Now.ToUniversalTime();
                return now.ToLongTimeString();
            }
        }

        void GetStyles()
        {
            styles.Items.Clear();

            //                SqlCommand cmd = new SqlCommand();
            //                cmd.Connection = cn;
            //                cmd.CommandText = @"
            //                  select isnull(l.style,'') style
            //                    from ONLlists l(nolock)
            //                order by l.live desc, l.lastUpd desc, l.iid desc";
            //                List<string> lst = new List<string>();
            //                var rdr = cmd.ExecuteReader();
            //                try
            //                {
            //                    while (rdr.Read())
            //                    {
            //                        var s = rdr[0].ToString();
            //                        if (lst.IndexOf(s) < 0)
            //                            lst.Add(s);
            //                    }
            //                }
            //                finally { rdr.Close(); }

            
            List<string> lst = new List<string>();
            try
            {
                var l1 = CurrentCompetition.ONLlists.Where(l => l.live).OrderByDescending(l => l.lastUpd);
                foreach (var s in l1)
                    if (!lst.Contains(s.style))
                        lst.Add(s.style);
            }
            catch { }
            try
            {
                var l1 = CurrentCompetition.ONLlists.Where(l => !l.live).OrderByDescending(l => l.lastUpd);
                foreach (var s in l1)
                    if (!lst.Contains(s.style))
                        lst.Add(s.style);
            }
            catch { }

            foreach (var s in lst)
                styles.Items.Add(new ListItem(s, s));
            if (styles.Items.Count > 0)
            {
                styles.SelectedIndex = 0;
                GetGroups();
            }
        }
        
        protected void styles_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetGroups();
        }

        class GroupEqualityComparer : IEqualityComparer<ONLGroup>
        {
            public bool Equals(ONLGroup x, ONLGroup y)
            {
                return (x.iid == y.iid);
            }

            public int GetHashCode(ONLGroup obj)
            {
                return obj.iid.GetHashCode();
            }
        }


        void GetGroups()
        {
            groups.Items.Clear();
            if (styles.SelectedIndex < 0)
                return;

//            SqlCommand cmd = new SqlCommand();
//            cmd.Connection = cn;
//            cmd.CommandText = @"
//                     select g.iid, g.name
//                       from ONLlists l(nolock)
//                       join ONLgroups g(nolock) on g.iid = l.group_id
//                   order by l.live desc, l.lastUpd desc, l.iid desc";

            var groupList = from l in dc.ONLlists
                            where l.comp_id == compID
                                   && l.ONLGroup != null
                            orderby l.live descending, l.lastUpd descending
                            select l.ONLGroup;


            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (var g in groupList)
                if (!dict.ContainsKey(g.iid))
                    dict.Add(g.iid, g.name);

            //var rdr = cmd.ExecuteReader();
            //try
            //{
            //    while (rdr.Read())
            //    {
            //        int nIid = Convert.ToInt32(rdr[0]);
            //        if (!dict.ContainsKey(nIid))
            //            dict.Add(nIid, rdr[1].ToString());
            //    }
            //}
            //finally { rdr.Close(); }

            //var res = (from l in dc.ONLlists
            //           orderby l.live descending,
            //           l.lastUpd descending, l.iid descending
            //           select l.ONLGroup).Distinct(new GroupEqualityComparer());

            //var res = (from l in dc.ONLlists
            //           orderby (from l2 in dc.ONLlists
            //                    where l2.style == l.style && l2.group_id == l.group_id
            //                    select Convert.ToInt32(l2.live)).Max() descending,
            //                    (from l2 in dc.ONLlists
            //                     where l2.style == l.style && l2.group_id == l.group_id
            //                     select l2.iid).Max() descending
            //           select new { l.ONLGroup.name, iid = l.ONLGroup.iid.ToString() }).Distinct();

            foreach (var s in dict)
                groups.Items.Add(new ListItem(s.Value, s.Key.ToString()));
            if (groups.Items.Count > 0)
            {
                groups.SelectedIndex = 0;
                GetRounds();
            }
        }

        protected void groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetRounds();
        }

        void GetRounds()
        {
            rounds.Items.Clear();
            if (groups.SelectedIndex < 0)
                return;
            int n;

//            SqlCommand cmd = new SqlCommand();
//            cmd.Connection = cn;
//            cmd.CommandText = @"
//                     select l.iid, case l.live when 1 then 'Live! ' else '' end + l.round round
//                       from ONLlists l(nolock)
//                      where l.style = @s
//                        and l.group_id = @g
//                   order by l.live desc,l.lastUpd desc, l.iid desc";
//            cmd.Parameters.Add("@s", SqlDbType.VarChar, 255);
//            cmd.Parameters[0].Value = styles.SelectedValue;

//            cmd.Parameters.Add("@g", SqlDbType.Int);
//            cmd.Parameters[1].Value = (int.TryParse(groups.SelectedValue, out n) ? (object)n : DBNull.Value);

            int? groupID = int.TryParse(groups.SelectedValue, out n)?(new int?(n)):null;

            var roundsList = from l in dc.ONLlists
                             where l.comp_id == compID
                             && l.style == styles.SelectedValue
                             && l.group_id == groupID
                             orderby l.live descending, l.lastUpd descending
                             select new
                             {
                                 iid = l.iid,
                                 name = (l.live ? "Live! " : String.Empty) + l.round
                             };

            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (var r in roundsList)
                dict.Add(r.iid, r.name);

            //var rdr = cmd.ExecuteReader();
            //try
            //{
            //    while (rdr.Read())
            //        dict.Add(Convert.ToInt32(rdr["iid"]), rdr["round"].ToString());
            //}
            //finally { rdr.Close(); }
            //var res = from l in dc.ONLlists
            //          where l.style == styles.SelectedValue && l.group_id == int.Parse(groups.SelectedValue)
            //          orderby l.live descending, l.lastUpd descending, l.iid descending
            //          select new { iid = l.iid.ToString(), rnd = (l.live ? "Live! " : "") + l.round };
            foreach (var s in dict)
                rounds.Items.Add(new ListItem(s.Value, s.Key.ToString()));

            if (rounds.Items.Count > 0)
            {
                rounds.SelectedIndex = 0;
                GetList();
            }
        }



        DataTable dt = new DataTable();


        bool GetList()
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                int iid;
                if (!int.TryParse(rounds.SelectedValue, out iid))
                    iid = -1;
                bool retVal = false;
                bool roundFinished = false;
                string start = "";
                try
                {
                    var res = (from l in dc.ONLlists
                               where l.iid == iid
                               select new
                               {
                                   l.roundFinished,
                                   start = (l.roundFinished || l.live) ? "" : l.start_time
                               }).First();
                    if (res != null)
                    {
                        roundFinished = res.roundFinished;
                        start = res.start;
                    }
                }
                catch { }
                try
                {
                    try
                    {
                        var lastUpd = (from ld in dc.ONLlists
                                       where ld.iid == iid
                                       select ld.lastUpd).First();
                        if (lastUpd == null)
                            lblList.Text = "";
                        else
                            lblList.Text = "Протокол обновлён " + lastUpd.Value.ToShortDateString() + " в " + lastUpd.Value.ToLongTimeString() + " UTC";
                    }
                    catch { lblList.Text = String.Empty; }
                }
                catch { lblList.Text = ""; }
                if (rounds.SelectedValue != null && rounds.SelectedValue != "")
                {
                    lblTMP.Visible = Timer1.Enabled = (rounds.SelectedItem.Text.IndexOf("Live") > -1);
                    lblTitle.Text = styles.SelectedItem + " " + groups.SelectedItem + " " + rounds.SelectedItem;
                    if (start.Length > 0)
                        lblTitle.Text += "; Время старта: " + start;


                    dt.Clear();
                    dt.Columns.Clear();

                    try
                    {
                        if (cn.State != ConnectionState.Open)
                            cn.Open();
                        dt = ListCreator.GetResultList(iid, cn, compID);
                    }
                    finally { cn.Close(); }
                    if (dt == null)
                        dt = new DataTable();
                    foreach (DataRow dr in dt.Rows)
                        foreach (DataColumn dCol in dt.Columns)
                            try
                            {
                                if (dr[dCol].ToString().ToLower().IndexOf("протокол") > -1)
                                {
                                    retVal = true;
                                    break;
                                }
                            }
                            catch { }
                    if (!retVal && !roundFinished)
                    {
                        try
                        {
                            if (cmd.Connection == null)
                                cmd.Connection = cn;
                            if (cn.State != ConnectionState.Open)
                                cn.Open();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "UPDATE ONLlists SET roundFinished = 1 WHERE iid=" + iid.ToString();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    GridView1.AutoGenerateColumns = false;
                    GridView1.Columns.Clear();
                    bool isID = dt.Columns.IndexOf("iid") > -1;
                    bool isN = dt.Columns.IndexOf("№") > -1;
                    bool HasIid = (isID || isN);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn dCol = dt.Columns[i];
                        DataControlField bf;

                        if (i != 2 || (!HasIid))
                        {
                            BoundField bff = new BoundField();
                            bff.DataField = dCol.ColumnName;
                            bff.HeaderText = dCol.ColumnName;
                            bf = bff;
                        }
                        else
                        {
                            HyperLinkField hf = new HyperLinkField();
                            hf.ControlStyle.ForeColor = System.Drawing.Color.Blue;
                            hf.DataTextField = dCol.ColumnName;
                            hf.HeaderText = dCol.ColumnName;
                            hf.ItemStyle.Font.Underline = false;
                            if (isID)
                                hf.DataNavigateUrlFields = new string[] { "iid" };
                            else if (isN)
                                hf.DataNavigateUrlFields = new string[] { "№" };
                            hf.DataNavigateUrlFormatString = "~/climber.aspx?" + Constants.PARAM_SECRETARY_ID + "={0}&" + Constants.PARAM_COMP_ID + "=" + compID.ToString();
                            bf = hf;
                        }
                        GridView1.Columns.Add(bf);
                        if (dCol.ColumnName == "iid" || dCol.ColumnName == "№")
                            GridView1.Columns[i].Visible = false;
                        if (i != 2 && i != 4)
                            GridView1.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        else
                            GridView1.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    }

                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
                return retVal;
            }
        }

        protected void UpdateList_Load(object sender, EventArgs e)
        {
            
        }

        protected void rounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetList();
        }
        protected void timerEv(object sender, EventArgs e)
        {
            GetList();
        }
    }
}