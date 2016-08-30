// <copyright file="WebAppl.Master.cs">
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

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class _MPWebAppl : System.Web.UI.MasterPage
    {
        //Разница во времени между местом проведения сорев и UTC
        private static TimeSpan TTS_DIFF = new TimeSpan(3, 0, 0);
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            //тут задать время начала соревнований (для бегущей строки)
            DateTime dtStart = new DateTime(2011, 01, 05, 10, 0, 0);
            TimeSpan ts = dtStart - DateTime.Now.ToUniversalTime().Add(TTS_DIFF);
            string res;
            if (ts < new TimeSpan(0))
            {
                res = "От начала соревнований прошло ";
                ts = -ts;
            }
            else
                res = "До начала соревнований осталось ";
            res += ts.Days.ToString() + " ";
            string tmp;
            switch (GetPadeg(ts.Days))
            {
                case Padeg.SINGLE:
                    tmp = "день";
                    break;
                case Padeg.PLURAL_SMALL:
                    tmp = "дня";
                    break;
                default:
                    tmp = "дней";
                    break;
            }
            res += tmp + " " + ts.Hours.ToString() + " ";

            switch (GetPadeg(ts.Hours))
            {
                case Padeg.SINGLE:
                    tmp = "час";
                    break;
                case Padeg.PLURAL_SMALL:
                    tmp = "часа";
                    break;
                default:
                    tmp = "часов";
                    break;
            }
            res += tmp + " " + ts.Minutes.ToString() + " ";

            switch (GetPadeg(ts.Minutes))
            {
                case Padeg.SINGLE:
                    tmp = "минута";
                    break;
                case Padeg.PLURAL_SMALL:
                    tmp = "минуты";
                    break;
                default:
                    tmp = "минут";
                    break;
            }
            res += tmp + " " + ts.Seconds.ToString() + " ";

            switch (GetPadeg(ts.Seconds))
            {
                case Padeg.SINGLE:
                    tmp = "секунда";
                    break;
                case Padeg.PLURAL_SMALL:
                    tmp = "секунды";
                    break;
                default:
                    tmp = "секунд";
                    break;
            }
            res += tmp;

            return res;
        }

        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        static Padeg GetPadeg(int num)
        {
            if (num < 0)
                throw new ArgumentException("Неверное число");
            if (num == 0)
                return Padeg.PLURAL_BIG;
            if (num < 10)
                return GetPadegOne(num);
            if (num < 20)
                return Padeg.PLURAL_BIG;
            string sTmp = num.ToString();
            num = Convert.ToInt32(sTmp[sTmp.Length - 1].ToString());
            return GetPadegOne(num);
        }

        private static Padeg GetPadegOne(int num)
        {
            if (num == 0)
                return Padeg.PLURAL_BIG;
            if (num == 1)
                return Padeg.SINGLE;
            if (num < 5)
                return Padeg.PLURAL_SMALL;
            return Padeg.PLURAL_BIG;
        }
        enum Padeg { SINGLE, PLURAL_SMALL, PLURAL_BIG }

        

        private Entities _dc = null;
        protected Entities dc
        {
            get
            {
                if (_dc == null)
                {
                    string cString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString;
                    _dc = new Entities(cString);
                }
                if (_dc.Connection.State != ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }
        long compID;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                compID = this.GetCompID();
                
                panelCompAdminLinks.Visible = Page.User.IsInRole(Constants.ROLE_ADMIN, compID);
                if (!IsPostBack)
                {
                

                    //if (compID < 0)
                    //{
                    //    Response.Redirect(ExtensionMethods.DEFAULT_REDIRECT);
                    //    return;
                    //}

                    lbl.Text = "Copyright (c) Ivan Kaurov 2006-" + DateTime.Now.Year;

                    if (compID > 0)
                    {
                        //lblCountback.Text = GetTimeSpan();

                        var comp = (from c in dc.ONLCompetitions
                                    where c.iid == compID
                                    select c).First();
                        this.lblH.Text = comp.name;
                    }
                    else
                        this.lblH.Text = "ClimbingCompetition Web!";

                    SetExistingLinks();
                }
            }
            catch { }
        }

        private static void SetImageLinkVisible(HyperLink link, string paramName, ONLCompetition comp)
        {
            if (comp == null)
            {
                link.Visible = false;
                return;
            }
            string srcToSet = comp.GetBinaryParamLink(paramName);
            if (String.IsNullOrEmpty(srcToSet))
            {
                link.Visible = false;
                return;
            }
            link.ImageUrl = srcToSet;
            string strNavigate = comp.GetStringParam(paramName + Constants.PDB_PARAM_ADD_INFO);
            if (strNavigate == null)
                strNavigate = String.Empty;
            link.NavigateUrl = strNavigate;
            link.Enabled = !strNavigate.Equals(String.Empty);
        }

        protected void SetExistingLinks()
        {
            ONLCompetition comp;
            try { comp = dc.ONLCompetitions.Where(c => c.iid == compID).First(); }
            catch { comp = null; }

            hlInfos.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_POLOGENIE);
            titul.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_POLOGENIE_TITUL);
            hlRegl.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_REGLAMENT);
            hlReglTitul.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_REGLAMENT_TITUL);
            hlRasp.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_RASP);
            hlAccom.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_ACCOMODATION);
            hlRes.SetHyperlinkVisible(comp, Constants.PDB_COMP_BIN_RESULTS);

            if (comp == null)
                hlVideo.Visible = false;
            else
                hlVideo.Visible = comp.GetBooleanParam(Constants.PDB_COMP_VIDEO);

            SetImageLinkVisible(hlOrgLeft, Constants.PDB_COMP_BIN_LOGO_LEFT, comp);
            SetImageLinkVisible(hlOrgRight, Constants.PDB_COMP_BIN_LOGO_RIGHT, comp);

            LoadSponsors(comp, panelSponsors, gvSponsors, Constants.PDB_COMP_SPONSORS);
            LoadSponsors(comp, panelPartners, gvPartners, Constants.PDB_COMP_PARTNERS);
            LoadAdditionalLinks(comp);
        }

        void LoadAdditionalLinks(ONLCompetition comp)
        {
            try
            {
                if (comp == null)
                {
                    panelAdditionalFiles.Visible = false;
                    return;
                }
                var pList = comp.GetBinaryParamLinks(p => p.ParamName.IndexOf(Constants.PDB_COMP_BIN_ADD_FILE) == 0).Where(d => !String.IsNullOrEmpty(d.Value)).ToArray();
                if (pList.Length < 1)
                {
                    panelAdditionalFiles.Visible = false;
                    return;
                }
                var src = from l in pList
                          select new
                          {
                              link = l.Value,
                              linktext = (from p in comp.ONLCompetitionParams
                                          where p.ParamName.Equals(l.Key + Constants.PDB_PARAM_ADD_INFO)
                                          select p.ParamValue).FirstOrDefault()
                          };
                panelAdditionalFiles.Visible = true;
                gvAdditionalFiles.DataSource = src;
                gvAdditionalFiles.DataBind();
            }
            catch { panelAdditionalFiles.Visible = false; }
        }

        static void LoadSponsors(ONLCompetition comp, Panel panel, GridView gridView, string paramName)
        {
            try
            {
                if (comp == null)
                {
                    panel.Visible = false;
                    return;
                }
                var parList = comp.GetBinaryParamLinks(p => p.ParamName.IndexOf(paramName) == 0).Where(d => !String.IsNullOrEmpty(d.Value)).ToArray();
                if (parList.Length < 1)
                {
                    panel.Visible = false;
                    return;
                }
                var src = from l in parList
                          select new { img_src = l.Value };
                panel.Visible = true;
                gridView.DataSource = src;
                gridView.DataBind();
            }
            catch { panel.Visible = false; }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            //lblCountback.Text = GetTimeSpan();
        }

    }
}
