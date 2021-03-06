// <copyright file="Default.aspx.cs">
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

﻿//#define NO_REDIR
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class Default : System.Web.UI.Page
    {
        private SqlConnection _cn;
        protected SqlConnection cn
        {
            get
            {
                if (_cn == null)
                    _cn = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
                if (_cn.State != ConnectionState.Open)
                    _cn.Open();
                return _cn;
            }
        }

        private Entities _dc;
        protected Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                return;
            Session.Clear();
#if !NO_REDIR
            bool redirect;
            try
            {
                string s = Request.QueryString[Constants.PARAM_NO_REDIR];
                if (String.IsNullOrEmpty(s))
                    redirect = true;
                else
                    redirect = !(s.Equals("1") || s.Equals("true", StringComparison.InvariantCultureIgnoreCase));
            }
            catch { redirect = true; }
            if (redirect)
            {
                var l = GetCurrentCompetition();
                if (l != null)
                {
                    try { Session[Constants.PARAM_COMP_ID] = l.Value; }
                    catch { Session.Add(Constants.PARAM_COMP_ID, l.Value); }
                    Response.Redirect("~/ResultService.aspx?" + Constants.PARAM_COMP_ID + "=" + l.Value.ToString());
                    return;
                }
            }
#endif
            LoadCompList();
        }

        private long? GetCurrentCompetition()
        {
            DateTime current = DateTime.Now;
            var compList = from c in dc.ONLCompetitions
                           select c;
            List<ONLCompetition> cList = new List<ONLCompetition>();
            foreach (var c in compList)
                if (c.GetDateParam(Constants.PDB_COMP_START_DATE) <= current && c.GetDateParam(Constants.PDB_COMP_END_DATE) >= current)
                    cList.Add(c);
            switch (cList.Count)
            {
                case 0:
                    return null;
                //case 1:
                //    return cList[0].iid;
                default:
                    cList.Sort((a, b) => a.GetDateParam(Constants.PDB_COMP_START_DATE).CompareTo(b.GetDateParam(Constants.PDB_COMP_START_DATE)));
                    return cList[0].iid;
            }
            //foreach(var v in compList)
        }

        private void LoadCompList()
        {
            List<ONLCompetition> compList = new List<ONLCompetition>();
            foreach (var c in (from q in dc.ONLCompetitions
                               select q))
                compList.Add(c);

            compList.Sort((a, b) => b.GetDateParam(Constants.PDB_COMP_START_DATE).CompareTo(a.GetDateParam(Constants.PDB_COMP_START_DATE)));

            cbSelectComp.Items.Clear();
            foreach (var c in compList)
                cbSelectComp.Items.Add(new ListItem(c.short_name, c.iid.ToString()));
            if (compList.Count > 0)
            {
                cbSelectComp.SelectedIndex = 0;
                LoadCompParams(compList[0].iid);
                btnProceed.Enabled = true;
            }
            else
                btnProceed.Enabled = false;
        }

        private void LoadCompParams(long curId)
        {
            var cList = from c in dc.ONLCompetitions
                        where c.iid == curId
                        select c;
            if (cList.Count() < 1)
                return;
            var curComp = cList.First();
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.AddString("Наименование", curComp.name);
            p.AddString("Место проведения", curComp.place);
            p.AddString("Сроки проведения", curComp.date);

            p.AddDateTime("Заявки до", curComp.GetDateParamNullable(Constants.PDB_COMP_DEADLINE));
            p.AddDateTime("Коррекция заявок до", curComp.GetDateParamNullable(Constants.PDB_COMP_DEADLINE_CHANGE));

            competitionParamsGrid.DataSource = p;
            competitionParamsGrid.ShowHeader = false;
            competitionParamsGrid.DataBind();
        }
        

        protected void cbSelectComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bnVis = false;
            try
            {
                if (cbSelectComp.SelectedValue == null)
                    return;
                long curId;
                if (!long.TryParse(cbSelectComp.SelectedValue, out curId))
                    return;
                LoadCompParams(curId);
                bnVis = true;
            }
            finally { btnProceed.Enabled = bnVis; }
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            string curId = cbSelectComp.SelectedValue;
            if (curId == null)
                curId = String.Empty;
            long l;
            if (long.TryParse(curId, out l))
                try { Session[Constants.PARAM_COMP_ID] = l; }
                catch { Session.Add(Constants.PARAM_COMP_ID, l); }
            Response.Redirect("~/ResultService.aspx?" + Constants.PARAM_COMP_ID + "=" + curId);
        }
    }
}