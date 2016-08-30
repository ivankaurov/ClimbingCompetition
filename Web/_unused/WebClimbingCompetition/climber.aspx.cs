// <copyright file="climber.aspx.cs">
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
using System.Web.UI.WebControls;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using WebClimbing.src;

namespace WebClimbing
{
    public partial class _Pclimber : BasePage
    {
        private MutexClsCollection mutexes = new MutexClsCollection();

        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string GetTimeSpan()
        {
            return _MPWebAppl.GetTimeSpan();
        }

        string name = "";
        string team = "";
        string age = "";
        string group = "";
        string qf = "";
        string rLead = "";
        string rSpeed = "";
        string rBoulder = "";

        ONLClimberCompLink link = null;

        

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;

            try
            {
                long lnkIid = Request.GetLongParam("iid");
                if (lnkIid > 0)
                    link = dc.ONLClimberCompLinks.First(l => l.iid == lnkIid);
                else
                {
                    long compIDC = Request.GetLongParam(Constants.PARAM_COMP_ID);
                    int secrID = (int)Request.GetLongParam(Constants.PARAM_SECRETARY_ID);
                    link = dc.ONLClimberCompLinks.First(l => l.comp_id == compIDC && l.secretary_id == secrID);
                }
            }
            catch
            {
                link = null;
                Response.Redirect("~/ClimbersList.aspx");
            }



            name = link.ONLclimber.surname.ToUpper() + " " + link.ONLclimber.name;
            team = link.ONLteam.name;
            age = link.ONLclimber.age.ToString();
            group = link.ONLGroup.name;
            qf = link.qf;
            
            rLead = link.rankingLead == null ? String.Empty : link.rankingLead.Value.ToString();
            rSpeed = link.rankingSpeed == null ? String.Empty : link.rankingSpeed.Value.ToString();
            rBoulder = link.rankingBoulder == null ? String.Empty : link.rankingBoulder.Value.ToString();
            Label3.Text = link.vk ? ("Участни" + (link.ONLclimber.genderFemale ? "ца" : "к")) : String.Empty;
            try { FillRes(); }
            catch { }
            finally { getPhoto(); }


            lblName.Text = name;
            lblGroup.Text = group;
            lblTeam.Text = team;
            lblIid.Text = link.secretary_id.ToString();
            lblAge.Text = age;
            lblQf.Text = qf;

            if (rLead == "")
                lblRLead.Visible = false;
            else
            {
                lblRLead.Text = "<br \\>Трудность: " + rLead + " место";
                lblRLead.Visible = true;
            }

            if (rSpeed == "")
                lblRSpeed.Visible = false;
            else
            {
                lblRSpeed.Text = "<br \\>Скорость: " + rSpeed + " место";
                lblRSpeed.Visible = true;
            }

            if (rBoulder == "")
                lblRBoulder.Visible = false;
            else
            {
                lblRBoulder.Text = "<br \\>Боулдеринг: " + rBoulder + " место";
                lblRBoulder.Visible = true;
            }
        }

        List<ListEl> GetLists()
        {
            var lst = dc.ONLlists.Where(l => l.group_id != null && l.comp_id == link.comp_id &&
                l.ONLlistdatas.Count(ld => ld.climber_id == link.iid) > 0);
            
            List<ListEl> res = new List<ListEl>();
            int rInd = -1;

            foreach(var l in lst)
            {
                ListEl le = new ListEl(l.iid, l.style, l.round, l.group_id.Value);
                res.Add(le);
                if (rInd < 0 && le.Style.ToLower() == "трудность" && le.Round.ToLower().IndexOf("квалификация 2") == 0)
                    rInd = res.Count - 1;
            }
            if (rInd >= 0)
            {
                try
                {
                    List<ListEl> r2 = new List<ListEl>();
                    for (int i = 0; i <= rInd; i++)
                        r2.Add(res[i]);
                    var lq = (from l in dc.ONLlists
                              where l.round == "Квалификация"
                                 && l.style == "Трудность"
                                 && l.group_id == res[rInd].GroupID
                              select l.iid).First();
                    r2.Add(new ListEl(lq, res[rInd].Style,
                        "Квалификация", res[rInd].GroupID));
                    for (int i = rInd + 1; i < res.Count; i++)
                        r2.Add(res[i]);
                    res.Clear();
                    res = r2;
                }
                catch { }
            }
            return res;
        }

        void FillRes()
        {
            List<ListEl> lst = GetLists();
            listLead.Items.Clear();
            listBoulder.Items.Clear();
            listSpeed.Items.Clear();
            foreach (ListEl le in lst)
            {
                DataTable dtRes = ClimbingCompetition.Online.ListCreator.GetResultList(le.Iid, cn, link.comp_id);
                DataRow dr = null;
                foreach(DataRow drr in dtRes.Rows)
                    if (drr["№"].ToString() == link.secretary_id.ToString())
                    {
                        dr = drr;
                        break;
                    }
                if (dr == null)
                    continue;
                switch (le.Style)
                {
                    case "Трудность":
                        FillBulletedList(dr["Место"].ToString(), dr["Рез-т"].ToString(),
                            listLead, lblLead, le);
                        break;
                    case "Скорость":
                        FillBulletedList(dr["Место"].ToString(), dr["Сумма"].ToString(),
                            listSpeed, lblSpeed, le);
                        break;
                    case "Боулдеринг":
                        if (le.Round == "Суперфинал")
                            goto case "Трудность";
                        string res, pos = dr["Место"].ToString();
                        int nTmp;
                        if (pos == "в/к" || int.TryParse(pos, out nTmp))
                            res = dr["Тр."].ToString() + "/" + dr["Поп.на тр."].ToString() + " " +
                                dr["Бон."].ToString() + "/" + dr["Поп.на бон."].ToString();
                        else
                        {
                            res = pos.ToString();
                            pos = "";
                        }
                        FillBulletedList(pos, res, listBoulder, lblBoulder, le);
                        break;
                }
            }
        }

        void getPhoto()
        {
            int id = link.climber_id;
            string path = ImageWorker.GetOnlineImg(cn, id, false);
            if (path.Length < 1)
                photoBox.Visible = false;
            else
            {
                photoBox.Visible = true;
                photoBox.ImageUrl = path;
            }
        }

        private void FillBulletedList(string pos, string res, BulletedList dl, Label lb, ListEl le)
        {
            string ps = "", rs = "";
            if (pos.Length > 0)
                ps = pos + " место";
            if (res.Length > 0)
                rs = "Результат: " + res;
            if (ps != "" && rs != "")
                ps += ", ";

            ListItem li = new ListItem(le.Round + ": " + ps + rs,
                    "~/ResultService.aspx?" + Constants.PARAM_IID + "=" + le.Iid.ToString());
            string imageID = Label2.ID;
            string js = "loadS()";
            li.Attributes.Add("onclick", js);
            dl.Items.Add(li);
            dl.Visible = true;
            lb.Visible = true;
        }


    }

    public class MutexCls
    {
        private System.Threading.Mutex m;
        public System.Threading.Mutex M { get { return m; } }
        private int iid;
        public int Iid { get { return iid; } }
        public MutexCls(int iid)
        {
            this.iid = iid;
            this.m = new System.Threading.Mutex();
        }
    }

    public class MutexClsCollection : List<MutexCls>
    {
        public new MutexCls this[int iid]{
            get
            {
                foreach (MutexCls m in this)
                    if (m.Iid == iid)
                        return m;
                return null;
            }
            set
            {
                base[iid] = value;
            }
        }
    }

    class ListEl
    {
        public int Iid { get; set; }
        public string Style { get; set; }
        public string Round { get; set; }
        public string TableName { get; private set; }
        public int GroupID { get; set; }
        public ListEl(int iid, string style, string round, int group_id)
        {
            GroupID = group_id;
            Iid = iid;
            Style = style;
            Round = round;
            switch (Style)
            {
                case "Трудность":
                    switch (Round)
                    {
                        case "Квалификация":
                            TableName = "ONLflash";
                            break;
                        default:
                            TableName = "ONLlead";
                            break;
                    }
                    break;
                case "Скорость":
                    TableName = "ONLspeed";
                    break;
                case "Боулдеринг":
                    TableName = "ONLboulder";
                    break;
            }
        }
    }
}