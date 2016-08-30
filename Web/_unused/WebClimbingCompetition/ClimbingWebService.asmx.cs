// <copyright file="ClimbingWebService.asmx.cs">
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
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Services;
using ClimbingCompetition;
using ClimbingCompetition.Online;
using WebClimbing.src;
using XmlApiData;

namespace WebClimbing
{
    /// <summary>
    /// Summary description for ClimbingWebService
    /// </summary>
    [WebService(Namespace = "http://climbing-competition.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ClimbingWebService : System.Web.Services.WebService
    {
        private Entities _dc;
        protected Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != System.Data.ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        [WebMethod]
        public bool ValidateAdminPassword(byte[] password, long compID)
        {
            if (password == null || password.Length < 1)
                return false;
            string passwordToCheck = PasswordWorkingClass.DecryptPassword(password);
            var n = (from uR in dc.ONLuserRoles
                     where uR.comp_id == compID
                     && uR.role_id == Constants.ROLE_ADMIN
                     && uR.ONLuser.password == passwordToCheck
                     select uR).Count();
            if (n > 0)
                return true;

            n = (from uR in dc.ONLuserRoles
                 where uR.role_id == Constants.ROLE_ADMIN_ROOT
                 && uR.ONLuser.password == passwordToCheck
                 select uR).Count();
            return (n > 0);
        }

        #region GET

        [WebMethod]
        public string GetParamValue(string paramName, long compID)
        {
            try
            {
                var pL = dc.ONLCompetitionParams.Where(p => p.comp_id == compID && p.ParamName.Equals(paramName));
                if (pL.Count() > 0)
                    return pL.First().ParamValue;
                else
                    return String.Empty;
            }
            catch { return String.Empty; }
        }



        [WebMethod]
        public PairingValue<string,string>[] GetAllBinaryParamsStringParamValues(long compID)
        {
            Dictionary<string, string> pVals = new Dictionary<string, string>();
            try
            {
                var pList = dc.ONLCompetitionParams.Where(p => p.comp_id == compID && p.ONLCompetitionBinaryParam != null).ToArray();
                foreach (var v in pList)
                    if (!pVals.ContainsKey(v.ParamName))
                        pVals.Add(v.ParamName, v.ParamValue);
            }
            catch { }
            PairingValue<string, string>[] ret = new PairingValue<string, string>[pVals.Count];
            int i = 0;
            foreach (var v in pVals)
                ret[i++] = new PairingValue<string, string>(v.Key, v.Value);
            return ret;
        }

        [WebMethod]
        public byte[] GetBinaryParamValue(string paramName, long compID)
        {
            try
            {
                var pL = dc.ONLCompetitionBinaryParams.Where(p => p.ONLCompetitionParam.comp_id == compID &&
                                                                p.ONLCompetitionParam.ParamName.Equals(paramName));
                if (pL.Count() > 0)
                    return pL.First().paramValue;
                else
                    return null;
            }
            catch { return null; }
        }

        [WebMethod]
        public ONLCompetitionSerializable[] GetAllCompetitions()
        {
            List<ONLCompetition> cLToSort = new List<ONLCompetition>();
            foreach (var v in dc.ONLCompetitions)
                cLToSort.Add(v);
            cLToSort.Sort(new Comparison<ONLCompetition>(delegate(ONLCompetition a, ONLCompetition b)
            {
                if (a.iid == b.iid)
                    return 0;
                var o = b.GetDateParam(Constants.PDB_COMP_START_DATE).CompareTo(
                    a.GetDateParam(Constants.PDB_COMP_START_DATE));
                if (o != 0)
                    return 0;
                return b.iid.CompareTo(a.iid);
            }));
            List<ONLCompetitionSerializable> lst = new List<ONLCompetitionSerializable>();
            foreach (var v in cLToSort)
                lst.Add(new ONLCompetitionSerializable(v));
            return lst.ToArray();
        }

        [WebMethod]
        public ONLTeamSerializable[] GetTeamsForCompetition(long compID)
        {
            var tList = dc.ONLTeamsCompLinks.Where(l => l.comp_id == compID).ToArray();
            ONLTeamSerializable[] t = new ONLTeamSerializable[tList.Length];
            for (int i = 0; i < t.Length; i++)
                t[i] = new ONLTeamSerializable(tList[i]);
            return t;
        }

        [WebMethod]
        public ONLGroupSerializable[] GetGroupsForCompetition(long compID)
        {
            var gList = dc.ONLGroupsCompLinks.Where(g => g.comp_id == compID).ToArray();
            ONLGroupSerializable[] grl = new ONLGroupSerializable[gList.Length];
            for (int i = 0; i < grl.Length; i++)
                grl[i] = new ONLGroupSerializable(gList[i]);
            return grl;
        }

        [WebMethod]
        public ONLClimberLinkSerializable GetClimber(int climberID, long compID, bool GetPhoto = true)
        {
            try
            {
                return new ONLClimberLinkSerializable(
                    dc.ONLClimberCompLinks.First(l => (l.comp_id == compID && l.secretary_id == climberID)), GetPhoto, dc);
            }
            catch { return null; }
        }

        [WebMethod]
        public ONLClimberLinkSerializable[] GetClimbersForCompetition(
            long compID, bool GetConfirmedOnly = true, bool GetPhoto = true,int? groupID = null, int? teamID = null)
        {
            var lnkList = dc.ONLClimberCompLinks.Where(l => l.comp_id == compID);
            if (GetConfirmedOnly)
                lnkList = lnkList.Where(l => l.state == Constants.CLIMBER_CONFIRMED);
            if (groupID != null && groupID.Value > 0)
                lnkList = lnkList.Where(l => l.group_id == groupID.Value);
            if (teamID != null && teamID.Value > 0)
                lnkList = lnkList.Where(l => l.team_id == teamID.Value);
            List<ONLClimberLinkSerializable> lstS = new List<ONLClimberLinkSerializable>();
            foreach (var v in lnkList)
                lstS.Add(new ONLClimberLinkSerializable(v, GetPhoto, dc));
            return lstS.ToArray();
        }

        #endregion

        #region POST

        #region LISTS

        private int? GetListBySercretaryIid(int secretary_id, long compID)
        {
            try { return dc.ONLlists.First(l => l.comp_id == compID && l.secretary_id == secretary_id).iid; }
            catch { return null; }
        }

        [WebMethod]
        public ListHeader GetList(int secretary_id, long compID)
        {
            try
            {
                var lst = dc.ONLlists.First(l => l.comp_id == compID && l.secretary_id == secretary_id);
                ListHeader lh = new ListHeader();
                lh.GroupID = lst.group_id;
                lh.LastUpdate = lst.lastUpd;
                lh.ListType = lst.listType;
                lh.Live = lst.live;
                try
                {
                    lh.PrevRound_SecretaryID = lst.prevRound == null ? null :
                        new int?(dc.ONLlists.First(l => l.iid == lst.prevRound.Value).secretary_id);
                }
                catch { lh.PrevRound_SecretaryID = null; }
                lh.Quote = lst.quote;
                lh.Round = lst.round;
                lh.RoundFinished = lst.roundFinished;
                lh.RouteNumber = lst.routeNumber;
                lh.SecretaryID = lst.secretary_id;
                try
                {
                    lh.SecretaryID_Parent = lst.iid_parent == null ? null :
                        new int?(dc.ONLlists.First(l => l.iid == lst.iid_parent.Value).secretary_id);
                }
                catch { lh.SecretaryID_Parent = null; }
                lh.StartTime = lst.start_time;
                lh.Style = lst.style;
                return lh;
            }
            catch { return null; }
        }

        [WebMethod]
        public ListHeader[] GetLists(int[] secretary_ids, long compID)
        {
            List<ListHeader> lst = new List<ListHeader>();
            foreach (var v in secretary_ids)
            {
                ListHeader lTmp = GetList(v, compID);
                if (lTmp != null)
                    lst.Add(lTmp);
            }
            return lst.ToArray();
        }

        [WebMethod]
        public bool PostList(ListHeader list, long compID)
        {
            try
            {
                var wWH = dc.ONLlists.Where(l => l.comp_id == compID && l.secretary_id == list.SecretaryID);
                ONLlist listToSet;
                if (wWH.Count() < 1)
                {
                    int iidNew;
                    try { iidNew = dc.ONLlists.OrderByDescending(l => l.iid).First().iid + 1; }
                    catch { iidNew = 1; }
                    listToSet = ONLlist.CreateONLlist(iidNew, list.SecretaryID, compID, list.ListType, list.Live,
                        list.Quote, list.StartTime, list.RoundFinished);
                    dc.ONLlists.AddObject(listToSet);
                }
                else
                {
                    listToSet = dc.ONLlists.First(l => l.comp_id == compID && l.secretary_id == list.SecretaryID);
                    listToSet.listType = list.ListType;
                    listToSet.live = list.Live;
                    listToSet.quote = list.Quote;
                    listToSet.start_time = list.StartTime;
                    listToSet.roundFinished = list.RoundFinished;
                }
                listToSet.group_id = list.GroupID;
                if (list.SecretaryID_Parent == null)
                    listToSet.iid_parent = null;
                else
                {
                    listToSet.iid_parent = GetListBySercretaryIid(list.SecretaryID_Parent.Value, compID);
                }
                listToSet.lastUpd = (list.LastUpdate != null && list.LastUpdate.HasValue ? list.LastUpdate.Value : DateTime.UtcNow);
                if (list.PrevRound_SecretaryID == null)
                    listToSet.prevRound = null;
                else
                {
                    listToSet.prevRound = GetListBySercretaryIid(list.PrevRound_SecretaryID.Value, compID);
                }
                listToSet.round = list.Round;
                listToSet.routeNumber = list.RouteNumber;
                listToSet.style = list.Style;
                dc.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        [WebMethod]
        public int[] PostLists(ListHeader[] lists, long compID)
        {
            List<int> scs = new List<int>();
            foreach (var list in lists)
            {
                try
                {
                    if (PostList(list, compID))
                        scs.Add(list.SecretaryID);
                }
                catch { }
            }
            return scs.ToArray();
        }

        private int DeleteListByFunction(Func<ONLlist, bool> selector)
        {
            try
            {
                var vToDel = dc.ONLlists.Where(selector).ToArray();
                if (vToDel.Length > 0)
                {
                    foreach (var v in vToDel)
                        dc.ONLlists.DeleteObject(v);
                    dc.SaveChanges();
                }
                return vToDel.Length;
            }
            catch (EntitySqlException) { return SQL_EXCEPTION; }
            catch (SqlException) { return SQL_EXCEPTION; }
            catch (Exception) { return GENERAL_EXCEPTION; }
        }

        [WebMethod]
        public int DeleteListsWhereSecretaryIDIn(int[] idToDel, long compID)
        {
            return DeleteListByFunction(l => l.comp_id == compID && idToDel.Contains(l.secretary_id));
        }

        [WebMethod]
        public int DeleteListsNotInSecretaryID(int[] idNotToDelete, long compID)
        {
            return DeleteListByFunction(l => l.comp_id == compID && !idNotToDelete.Contains(l.secretary_id));
        }

        private const int NO_CLIMBER = -100;
        private const int NO_LIST = -200;
        private const int SQL_EXCEPTION = -1;
        private const int GENERAL_EXCEPTION = -2;

        private long PostGeneralResult(ClimbersResult res, long compID)
        {
            var vWh = dc.ONLlistdatas.Where(ld => ld.ONLClimberCompLink.comp_id == compID &&
                ld.ONLClimberCompLink.secretary_id == res.ClimberID_Secretary
                && ld.ONLlist.secretary_id == res.ListID_Secretary);

            ONLlistdata ldToSet;
            if (vWh.Count() < 1)
            {
                long climberID;
                try { climberID = dc.ONLClimberCompLinks.First(l => l.comp_id == compID && l.secretary_id == res.ClimberID_Secretary).iid; }
                catch { return NO_CLIMBER; }

                int listID;
                try { listID = dc.ONLlists.First(l => l.comp_id == compID && l.secretary_id == res.ListID_Secretary).iid; }
                catch { return NO_LIST; }

                long nextIdLine;
                try { nextIdLine = dc.ONLlistdatas.OrderByDescending(l => l.iid_line).First().iid_line + 1; }
                catch { nextIdLine = 1; }

                ldToSet = ONLlistdata.CreateONLlistdata(listID, nextIdLine, climberID, res.PreQf);
                dc.ONLlistdatas.AddObject(ldToSet);
            }
            else
            {
                ldToSet = vWh.First();
                ldToSet.preQf = res.PreQf;
            }
            ldToSet.pos = res.Pos;
            ldToSet.res = res.Res;
            ldToSet.start = res.Start;
            dc.SaveChanges();

            ldToSet.ONLlist.lastUpd = DateTime.UtcNow;
            dc.SaveChanges();

            var vToDel = dc.ONLlistdatas.Where(l => l.climber_id == ldToSet.climber_id
                && l.iid == ldToSet.iid && l.iid_line != ldToSet.iid_line).ToArray();
            if (vToDel.Length > 0)
            {
                foreach (var vtd in vToDel)
                    dc.ONLlistdatas.DeleteObject(vtd);
                dc.SaveChanges();
            }

            return ldToSet.iid_line;
        }

        private delegate long PostResult<T>(T res, long compID, out long listDataID) where T : ClimbersResult;

        [WebMethod]
        public long PostLeadResult(LeadResult res, long compID, out long listDataID)
        {
            listDataID = GENERAL_EXCEPTION;
            try
            {
                listDataID = PostGeneralResult(res, compID);
                if (listDataID < 1)
                    return listDataID;
                var lTmp = listDataID;
                var vWh = dc.ONLleads.Where(ll => ll.iid_line == lTmp);
                ONLlead ld;
                if (vWh.Count() < 1)
                {
                    long nextIdPK;
                    try { nextIdPK = dc.ONLleads.OrderByDescending(l => l.idPK).First().idPK + 1; }
                    catch { nextIdPK = 1; }
                    ld = ONLlead.CreateONLlead(nextIdPK, listDataID);
                    dc.ONLleads.AddObject(ld);
                }
                else
                    ld = vWh.First();
                ld.res = res.TextRes;
                dc.SaveChanges();

                var vToDel = dc.ONLleads.Where(l => l.iid_line == ld.iid_line && l.idPK != ld.idPK).ToArray();
                if (vToDel.Length > 0)
                {
                    foreach (var v in vToDel)
                        dc.ONLleads.DeleteObject(v);
                    dc.SaveChanges();
                }

                try
                {
                    SortList(ld.ONLlistdata.iid, new Comparison<ONLlistdata>(delegate(ONLlistdata a, ONLlistdata b)
                    {
                        if (a.iid_line == b.iid_line)
                            return 0;
                        long resA = a.res != null && a.res.HasValue ? a.res.Value : -1;
                        long resB = b.res != null && b.res.HasValue ? b.res.Value : -1;
                        return resB.CompareTo(resA);
                    }));
                }
                catch { }

                return ld.idPK;
            }
            catch (EntitySqlException) { return SQL_EXCEPTION; }
            catch (SqlException) { return SQL_EXCEPTION; }
            catch (Exception) { return GENERAL_EXCEPTION; }
        }

        [WebMethod]
        public long PostSpeedResult(SpeedResult res, long compID, out long listDataID)
        {
            listDataID = GENERAL_EXCEPTION;
            
            try
            {
                ONLCompetition comp = dc.ONLCompetitions.First(c => c.iid == compID);
                if (comp == null)
                    return listDataID;
                var rules = comp.GetEnumParam<SpeedRules>(Constants.PDB_COMP_RULES, SpeedRules.DefaultAll);
                listDataID = PostGeneralResult(res, compID);
                if (listDataID < 1)
                    return listDataID;
                long lTmp = listDataID;
                var vWh = dc.ONLspeeds.Where(l => l.iid_line == lTmp);
                ONLspeed sd;
                if (vWh.Count() < 1)
                {
                    long nextIdPK;
                    try { nextIdPK = dc.ONLspeeds.OrderByDescending(l => l.idPK).First().idPK + 1; }
                    catch { nextIdPK = 1; }
                    sd = ONLspeed.CreateONLspeed(nextIdPK, listDataID, res.Qf);
                    dc.ONLspeeds.AddObject(sd);
                }
                else
                {
                    sd = vWh.First();
                    sd.qf = res.Qf;
                }
                sd.r1 = res.Route1;
                sd.r2 = res.Route2;
                sd.res = res.TextRes;
                dc.SaveChanges();

                bool needCheck1stQf;
                try
                {
                    needCheck1stQf = sd.ONLlistdata.ONLlist.listType.Equals(ListTypeEnum.SpeedQualy2.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    if (needCheck1stQf)
                    {
                        var v = dc.ONLCompetitions.First(l => l.iid == compID);
                        needCheck1stQf = (rules & SpeedRules.BestResultFromTwoQfRounds) == SpeedRules.BestResultFromTwoQfRounds;
                    }
                }
                catch { needCheck1stQf = false; }
                if(needCheck1stQf && sd.ONLlistdata.ONLlist.prevRound!= null)
                    try
                    {
                        var prevRes = (from l in dc.ONLlistdatas
                                       where l.iid == sd.ONLlistdata.ONLlist.prevRound.Value
                                       && l.climber_id == sd.ONLlistdata.climber_id
                                       select l.res).First();
                        long thisRes = res.Res != null && res.Res.HasValue ? res.Res.Value : long.MaxValue;
                        long prevResS = prevRes != null && prevRes.HasValue ? prevRes.Value : long.MaxValue;
                        if (prevResS < thisRes)
                            sd.ONLlistdata.res = prevResS;
                    }
                    catch { }


                var vToDel = dc.ONLspeeds.Where(l => l.iid_line == sd.iid_line && l.idPK != sd.idPK).ToArray();

                if (vToDel.Length > 0)
                {
                    foreach (var v in vToDel)
                        dc.ONLspeeds.DeleteObject(v);
                    dc.SaveChanges();
                }

                try
                {
                    bool isFinal = sd.ONLlistdata.ONLlist.listType.Equals(ListTypeEnum.SpeedFinal.ToString());
                    SortList(sd.ONLlistdata.iid, new Comparison<ONLlistdata>(delegate(ONLlistdata a, ONLlistdata b)
                    {
                        if (a.iid_line == b.iid_line)
                            return 0;
                        if (isFinal)
                        {
                            bool aQf = a.ONLspeeds.Count > 0 && !string.IsNullOrWhiteSpace(a.ONLspeeds.First().qf);
                            bool bQf = b.ONLspeeds.Count > 0 && !string.IsNullOrWhiteSpace(b.ONLspeeds.First().qf);
                            if (!aQf.Equals(bQf))
                                return bQf.CompareTo(aQf);
                        }
                        long resA = a.res != null && a.res.HasValue ? a.res.Value : long.MaxValue;
                        long resB = b.res != null && b.res.HasValue ? b.res.Value : long.MaxValue;
                        return resA.CompareTo(resB);
                    }));
                }
                catch { }
                return sd.idPK;
            }
            catch (EntitySqlException) { return SQL_EXCEPTION; }
            catch (SqlException) { return SQL_EXCEPTION; }
            catch (Exception) { return GENERAL_EXCEPTION; }
        }

        private void SortList(int listID, Comparison<ONLlistdata> comparison)
        {
            var resListWhere = dc.ONLlistdatas.Where(l => l.iid == listID);
            List<ONLlistdata> resultList = new List<ONLlistdata>();
            foreach (var v in resListWhere)
                resultList.Add(v);
            if (resultList.Count < 1)
                return;
            resultList.Sort(comparison);
            int curPos = 1;
            var res = resultList.ToArray();

            res[0].pos = curPos;
            for (int i = 1; i < res.Length; i++)
            {
                if (comparison(res[curPos - 1], res[i]) != 0)
                    curPos = (i + 1);
                res[i].pos = curPos;
            }
            dc.SaveChanges();
        }

        private long PostBoulderRoute(BoulderRouteResult res, long iid_parent)
        {
            var bw = dc.ONLboulderRoutes.Where(l => l.iid_parent == iid_parent && l.routeN == res.RouteNumber);
            ONLboulderRoute r;
            if (bw.Count() < 1)
            {
                long nextID;
                try { nextID = dc.ONLboulderRoutes.OrderByDescending(l => l.iid_line).First().iid_line + 1; }
                catch { nextID = 1; }
                r = ONLboulderRoute.CreateONLboulderRoute(nextID, iid_parent, res.RouteNumber);
                dc.ONLboulderRoutes.AddObject(r);
            }
            else
                r = bw.First();
            r.topA = res.TopAttempt;
            r.bonusA = res.BonusAttempt;
            dc.SaveChanges();

            var toDel = dc.ONLboulderRoutes.Where(l => l.iid_parent == iid_parent && l.routeN == res.RouteNumber && l.iid_line != r.iid_line).ToArray();
            if (toDel.Length > 0)
            {
                foreach (var rr in toDel)
                    dc.ONLboulderRoutes.DeleteObject(rr);
                dc.SaveChanges();
            }
            return r.iid_line;
        }

        [WebMethod]
        public long PostBoulderResult(BoulderResult res, long compID, out long listDataID)
        {
            listDataID = GENERAL_EXCEPTION;
            try
            {
                
                listDataID = PostGeneralResult(res, compID);
                if (listDataID < 1)
                    return listDataID;
                long lTmp = listDataID;
                var vWh = dc.ONLBoulders.Where(l => l.iid_line == lTmp);
                ONLBoulder br;
                if (vWh.Count() < 1)
                {
                    long nextIdPK;
                    try { nextIdPK = dc.ONLBoulders.OrderByDescending(l => l.idPK).First().idPK + 1; }
                    catch { nextIdPK = 1; }
                    br = ONLBoulder.CreateONLBoulder(nextIdPK, listDataID, res.DNS, res.DSQ);
                    dc.ONLBoulders.AddObject(br);
                }
                else
                {
                    br = vWh.First();
                    br.disq = res.DSQ;
                    br.nya = res.DNS;
                }

                br.B = res.Bonuses;
                br.Ba = res.BonusAttempts;
                br.T = res.Tops;
                br.Ta = res.TopAttempts;
                dc.SaveChanges();

                List<long> insertedRoutes = new List<long>();
                foreach (var vRes in res.RouteData)
                    insertedRoutes.Add(PostBoulderRoute(vRes, br.idPK));
                try
                {
                    var rToDel = dc.ONLboulderRoutes.Where(l => l.iid_parent == br.iid_line && !insertedRoutes.Contains(l.iid_line)).ToArray();
                    if (rToDel.Length > 0)
                    {
                        foreach (var v in rToDel)
                            dc.ONLboulderRoutes.DeleteObject(v);
                        dc.SaveChanges();
                    }
                }
                catch { }


                var vToDel = dc.ONLBoulders.Where(l => l.iid_line == br.iid_line && l.idPK != br.idPK).ToArray();

                if (vToDel.Length > 0)
                {
                    foreach (var v in vToDel)
                        dc.ONLBoulders.DeleteObject(v);
                    dc.SaveChanges();
                }

                try
                {
                    SortList(br.ONLlistdata.iid, new Comparison<ONLlistdata>(delegate(ONLlistdata a, ONLlistdata b)
                    {
                        if (a.iid_line == b.iid_line)
                            return 0;
                        long resA = a.res != null && a.res.HasValue ? a.res.Value : -1;
                        long resB = b.res != null && b.res.HasValue ? b.res.Value : -1;
                        return resB.CompareTo(resA);
                    }));
                }
                catch { }

                return br.idPK;
            }
            catch (EntitySqlException) { return SQL_EXCEPTION; }
            catch (SqlException) { return SQL_EXCEPTION; }
            catch (Exception) { return GENERAL_EXCEPTION; }
        }

        [WebMethod]
        public int PostBoulderResultList(BoulderResult[] results, long compID, bool clearOtherRes,out int errCode)
        {
            return PostResultList<BoulderResult>(results, compID, PostBoulderResult, clearOtherRes, out errCode);
        }

        [WebMethod]
        public int PostSpeedResultList(SpeedResult[] results, long compID, bool clearOtherRes, out int errCode)
        {
            return PostResultList<SpeedResult>(results, compID, PostSpeedResult, clearOtherRes, out errCode);
        }

        [WebMethod]
        public int PostLeadResultList(LeadResult[] results, long compID, bool clearOtherRes,out int errCode)
        {
            return PostResultList<LeadResult>(results, compID, PostLeadResult, clearOtherRes, out errCode);
        }

        private int ClearOtherResults(long[] otherRes, int listID)
        {
            var vToCLear = dc.ONLlistdatas.Where(l => l.iid == listID && !otherRes.Contains(l.iid_line)).ToArray();
            if (vToCLear.Length > 0)
            {
                foreach (var v in vToCLear)
                    dc.ONLlistdatas.DeleteObject(v);
                dc.SaveChanges();
            }
            return vToCLear.Length;
        }

        private int PostResultList<T>(T[] results, long compID, PostResult<T> postOneResFunc, bool clearOtherRes, out int errCode) where T : ClimbersResult
        {
            int cnt;
            long lTmp, lResID;
            errCode = 0;
            List<long> insertedResults = new List<long>();
            for (cnt = 0; cnt < results.Length; cnt++)
            {
                lTmp = postOneResFunc(results[cnt], compID, out lResID);
                if (!insertedResults.Contains(lResID))
                    insertedResults.Add(lResID);
                if (lTmp < 0)
                {
                    errCode = (int)lTmp;
                    return cnt;
                }
            }
            if (clearOtherRes && insertedResults.Count > 0)
            {
                try
                {
                    long someIidLine = insertedResults[0];
                    int listID = dc.ONLlistdatas.Where(l => l.iid_line == someIidLine).First().iid;
                    ClearOtherResults(insertedResults.ToArray(), listID);
                }
                catch { }
            }
            return cnt;
        }

        private ClimbersResult GetClimberResultFor(int listID_Secretary, int climberID_Secretary, long compID, out long resID)
        {
            resID = -1;
            var WH = from l in dc.ONLlistdatas
                     where l.ONLlist.comp_id == compID
                     && l.ONLlist.secretary_id == listID_Secretary
                     && l.ONLClimberCompLink.secretary_id == climberID_Secretary
                     select l;
            if (WH.Count() < 1)
                return null;
            ClimbersResult cRes = new ClimbersResult(listID_Secretary, climberID_Secretary);
            var w = WH.First();
            cRes.Pos = w.pos;
            cRes.PreQf = w.preQf;
            cRes.Res = w.res;
            cRes.Start = w.start;
            resID = w.iid_line;
            return cRes;
        }

        private LeadResult GetLeadResultForClimber(int listID_Secretary, int climberID_Secretary, long compID)
        {
            
            try
            {
                long resID;
                var cRes = GetClimberResultFor(listID_Secretary, climberID_Secretary, compID, out resID);
                if (cRes == null)
                    return null;
                var wH = from l in dc.ONLleads
                         where l.iid_line == resID
                         select l;
                if (wH.Count() < 1)
                    return null;
                var lrD = wH.First();
                LeadResult lr = new LeadResult(cRes);
                lr.TextRes = lrD.res;
                return lr;
            }
            catch { return null; }
        }

        private SpeedResult GetSpeedResultForClimber(int listID_Secretary, int climberID_Secretary, long compID)
        {
            try
            {
                long resID;
                var cRes = GetClimberResultFor(listID_Secretary, climberID_Secretary, compID, out resID);
                if (cRes == null)
                    return null;
                var wH = from l in dc.ONLspeeds
                         where l.iid_line == resID
                         select l;
                if (wH.Count() < 1)
                    return null;
                var lrD = wH.First();
                SpeedResult sr = new SpeedResult(cRes);
                sr.Route1 = lrD.r1;
                sr.Route2 = lrD.r2;
                sr.TextRes = lrD.res;
                sr.Qf = lrD.qf;
                return sr;
            }
            catch { return null; }
        }

        private BoulderResult GetBoulderResultForClimber(int listID_Secretary, int climberID_Secretary, long compID)
        {
            try
            {
                long resID;
                var cRes = GetClimberResultFor(listID_Secretary, climberID_Secretary, compID, out resID);
                if (cRes == null)
                    return null;
                var wH = from l in dc.ONLBoulders
                         where l.iid_line == resID
                         select l;
                if (wH.Count() < 1)
                    return null;
                var lrD = wH.First();
                BoulderResult sr = new BoulderResult(cRes);
                sr.Bonuses = lrD.B;
                sr.BonusAttempts = lrD.Ba;
                sr.DSQ = lrD.disq;
                sr.DNS = lrD.nya;
                sr.Tops = lrD.T;
                sr.TopAttempts = lrD.Ta;

                List<BoulderRouteResult> brList = new List<BoulderRouteResult>();
                try
                {
                    foreach (var v in lrD.ONLboulderRoutes)
                    {
                        BoulderRouteResult br = new BoulderRouteResult();
                        br.BonusAttempt = v.bonusA;
                        br.TopAttempt = v.topA;
                        br.RouteNumber = v.routeN;
                        brList.Add(br);
                    }
                }
                catch { }
                sr.RouteData = brList.ToArray();

                return sr;
            }
            catch { return null; }
        }

        [WebMethod]
        public LeadResult[] GetLeadResultsForClimber(int listID_Secretary, int[] climbers, long compID)
        {
            return GetResultsForClimbers<LeadResult>(listID_Secretary, climbers, compID, GetLeadResultForClimber);
        }

        [WebMethod]
        public SpeedResult[] GetSpeedResultsForClimber(int listID_Secretary, int[] climbers, long compID)
        {
            return GetResultsForClimbers<SpeedResult>(listID_Secretary, climbers, compID, GetSpeedResultForClimber);
        }

        [WebMethod]
        public BoulderResult[] GetBoulderResultsForClimber(int listID_Secretary, int[] climbers, long compID)
        {
            return GetResultsForClimbers<BoulderResult>(listID_Secretary, climbers, compID, GetBoulderResultForClimber);
        }

        private delegate T GetResForClimber<T>(int listID_Secretary, int climberID_Secretary, long compID) where T : ClimbersResult;
        private T[] GetResultsForClimbers<T>(int listID_Secretary, int[] climbers, long compID, GetResForClimber<T> selector) where T : ClimbersResult
        {
            List<T> reL = new List<T>();

            foreach (int n in climbers)
            {
                var v = selector(listID_Secretary, n, compID);
                if (v != null)
                    reL.Add(v);
            }
            return reL.ToArray();
        }

        #endregion

        #region COMP_PARAMS

        [WebMethod]
        public int RemoveCompetitionParam(string paramName, long compID)
        {
            try
            {
                var pL = dc.ONLCompetitionParams.Where(p => p.comp_id == compID && p.ParamName == paramName).ToArray();
                if (pL.Length > 0)
                {
                    for (int i = 0; i < pL.Length; i++)
                        dc.DeleteObject(pL[i]);
                    dc.SaveChanges();
                }
                if (paramName.IndexOf(Constants.PDB_PARAM_ADD_INFO) < 0)
                    RemoveCompetitionParam(paramName + Constants.PDB_PARAM_ADD_INFO, compID);
                return pL.Length;
            }
            catch { return -1; }
        }

        [WebMethod]
        public long PostCompetitionParam(string paramName, string paramValue, long compID)
        {
            try
            {
                var curParamL = dc.ONLCompetitionParams.Where(p => p.comp_id == compID && p.ParamName.Equals(paramName));
                ONLCompetitionParam curParam;
                if (curParamL.Count() < 1)
                {
                    long nextPID;
                    try { nextPID = dc.ONLCompetitionParams.OrderByDescending(p => p.iid).First().iid + 1; }
                    catch { nextPID = 1; }
                    curParam = ONLCompetitionParam.CreateONLCompetitionParam(nextPID, compID, paramName);
                    dc.ONLCompetitionParams.AddObject(curParam);
                }
                else
                    curParam = curParamL.First();
                curParam.ParamValue = paramValue;
                dc.SaveChanges();
                return curParam.iid;
            }
            catch { return -1; }
        }

        [WebMethod]
        public long PostCompetitionBinaryParam(string paramName, string paramValue, byte[] binaryValue, long compID)
        {
            try
            {
                long pIid = PostCompetitionParam(paramName, paramValue, compID);
                if (pIid < 0)
                    return pIid;
                var bpL = dc.ONLCompetitionBinaryParams.Where(p => p.iid == pIid);
                ONLCompetitionBinaryParam bp;
                if (bpL.Count() < 1)
                {
                    bp = ONLCompetitionBinaryParam.CreateONLCompetitionBinaryParam(pIid, binaryValue);
                    dc.ONLCompetitionBinaryParams.AddObject(bp);
                }
                else
                {
                    bp = bpL.First();
                    bp.paramValue = binaryValue;
                }
                dc.SaveChanges();

                var curPL = dc.ONLCompetitionParams.Where(p => p.comp_id == compID &&
                                                             p.ParamName.Equals(Constants.PDB_BINARY_UPDATED) &&
                                                             p.ParamValue.Equals(paramName));
                if (curPL.Count() < 1)
                {
                    long nextPID = dc.ONLCompetitionParams.OrderByDescending(p => p.iid).First().iid + 1;
                    var nextP = ONLCompetitionParam.CreateONLCompetitionParam(nextPID, compID, Constants.PDB_BINARY_UPDATED);
                    nextP.ParamValue = paramName;
                    dc.ONLCompetitionParams.AddObject(nextP);
                    dc.SaveChanges();
                }

                return bp.iid;
            }
            catch { return -1; }
        }

        #endregion

        [WebMethod]
        public bool PostPhotoForClimber(byte[] photo, int climberID, long compID)
        {
            try
            {
                var c = from cLNK in dc.ONLClimberCompLinks
                        where cLNK.comp_id == compID
                        && cLNK.secretary_id == climberID
                        select cLNK.ONLclimber;
                if (c.Count() < 1)
                    return false;
                var cToSet = c.First();
                if (photo == null)
                    cToSet.photo = null;
                else
                    cToSet.photo = (photo.Length > 0 ? photo : null);
                if (cToSet.EntityState == EntityState.Detached)
                    dc.ONLclimbers.Attach(cToSet);
                dc.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        [WebMethod]
        int PostTeamGroup(string name)
        {
            try
            {
                string nameToCheck = name.Trim();
                var gToUL = dc.ONLTeamGroups.Where(g => g.name == nameToCheck);
                ONLTeamGroup gt;
                if (gToUL.Count() < 1)
                {
                    int newID;
                    try { newID = dc.ONLTeamGroups.OrderByDescending(g => g.iid).First().iid + 1; }
                    catch { newID = 1; }
                    gt = ONLTeamGroup.CreateONLTeamGroup(newID, nameToCheck);
                    dc.ONLTeamGroups.AddObject(gt);
                }
                else
                {
                    gt = gToUL.First();
                    gt.name = nameToCheck;
                }
                dc.SaveChanges();
                return gt.iid;
            }
            catch { return -1; }
        }

        private int PostTeamBase(string name)
        {
            var tl = dc.ONLteams.Where(t => t.name.Equals(name)).ToArray();
            if (tl.Length > 0)
                return tl[0].iid;
            int idToSet;
            try { idToSet = dc.ONLteams.OrderByDescending(t => t.iid).First().iid + 1; }
            catch { idToSet = 1; }
            ONLteam nT = ONLteam.CreateONLteam(idToSet);
            nT.name = name;

            dc.ONLteams.AddObject(nT);
            dc.SaveChanges();
            return idToSet;
        }

        [WebMethod]
        public int[] PostTeams(ONLTeamSerializable[] teams, bool fullRefresh, long compID)
        {
            List<int> inserted = new List<int>();
            foreach (var v in teams)
                if (PostTeam(v) > 0)
                    inserted.Add(v.SecretaryId);
            try
            {
                if (fullRefresh && inserted.Count == teams.Length)
                {
                    var toDel = dc.ONLTeamsCompLinks.Where(l => l.comp_id == compID && !inserted.Contains(l.secretary_id)).ToArray();
                    if (toDel.Length > 0)
                    {
                        foreach (var vtd in toDel)
                            dc.ONLTeamsCompLinks.DeleteObject(vtd);
                        dc.SaveChanges();
                    }
                    var toDel3 = dc.ONLteams.Where(t => t.ONLClimberCompLinks.Count == 0 && t.ONLrankings.Count == 0
                        && t.ONLTeamsCompLinks.Count == 0 && t.ONLusers.Count == 0).ToArray();
                    if (toDel3.Length > 0)
                    {
                        foreach (var vtd in toDel3)
                            dc.ONLteams.DeleteObject(vtd);
                        dc.SaveChanges();
                    }
                }
            }
            catch { }
            return inserted.ToArray();
        }

        [WebMethod]
        public int PostTeam(ONLTeamSerializable team)
        {
            try
            {
                var teamID = PostTeamBase(team.Name);
                var oldLinkList = dc.ONLTeamsCompLinks.Where(l => l.comp_id == team.CompID && l.secretary_id == team.SecretaryId).ToArray();

                ONLTeamsCompLink newLink;
                if (oldLinkList.Length > 0)
                {
                    newLink = oldLinkList[0];
                    //если команда изменилась, значит надо перепривязать участников к новой команде
                    if (teamID != newLink.team_id)
                    {
                        var clmToUpdate = newLink.ONLteam.ONLClimberCompLinks.ToArray();
                        foreach (var c in clmToUpdate)
                            c.team_id = teamID;
                        newLink.team_id = teamID;
                        dc.SaveChanges();
                    }
                    newLink.ranking_pos = team.RankingPos;
                }
                else
                {
                    long nextLinkID;
                    try { nextLinkID = dc.ONLTeamsCompLinks.OrderByDescending(l => l.iid).First().iid + 1; }
                    catch { nextLinkID = 1; }
                    newLink = ONLTeamsCompLink.CreateONLTeamsCompLink(nextLinkID, teamID, team.CompID, team.SecretaryId, team.RankingPos);
                    dc.ONLTeamsCompLinks.AddObject(newLink);
                }
                dc.SaveChanges();
                return newLink.team_id;
            }
            catch { return -1; }
        }

        [WebMethod]
        public int PostGroup(ONLGroupSerializable grp)
        {
            try
            {
                int compYear = dc.ONLCompetitions.Where(c => c.iid == grp.CompID).First().GetCompYear();
                int oldYear = compYear - grp.yearOld;
                int youngYear = compYear - grp.yearYoung;

                var GroupListToUpdate = from g in dc.ONLGroups
                                       where g.genderFemale == grp.GenderFemale
                                       && g.youngYear == youngYear
                                       && g.oldYear == oldYear
                                       select g;
                ONLGroup groupToUpdate;
                if (GroupListToUpdate.Count() < 1)
                {
                    int newId;
                    try { newId = dc.ONLGroups.OrderByDescending(t => t.iid).First().iid + 1; }
                    catch { newId = 1; }
                    groupToUpdate = ONLGroup.CreateONLGroup(newId, oldYear, youngYear, grp.GenderFemale);
                    dc.ONLGroups.AddObject(groupToUpdate);
                }
                else
                    groupToUpdate = GroupListToUpdate.First();
                groupToUpdate.name = grp.Name;

                ONLGroupsCompLink lnk;
                if (groupToUpdate.ONLGroupsCompLinks.Count(l => l.comp_id == grp.CompID) < 1)
                {
                    long newL;
                    try { newL = dc.ONLGroupsCompLinks.OrderByDescending(g => g.iid).First().iid + 1; }
                    catch { newL = 1; }
                    lnk = ONLGroupsCompLink.CreateONLGroupsCompLink(newL, groupToUpdate.iid,
                        grp.CompID, grp.minQf);
                    dc.ONLGroupsCompLinks.AddObject(lnk);
                }
                else
                {
                    lnk = groupToUpdate.ONLGroupsCompLinks.First(l => l.comp_id == grp.CompID);
                    lnk.minQf = grp.minQf;
                }
                dc.SaveChanges();
                return groupToUpdate.iid;
            }
            catch { return -1; }
        }

        [WebMethod]
        public long PostClimber(ONLClimberLinkSerializable serClm)
        {
            try
            {
                var clmListToUpdate = from cl in dc.ONLclimbers
                                      where cl.name == serClm.Climber.name
                                      && cl.surname == serClm.Climber.surname
                                      && cl.age == serClm.Climber.age
                                      select cl;
                ONLclimber climberToSet;
                if (clmListToUpdate.Count() < 1)
                {
                    int nextIid;
                    try { nextIid = dc.ONLclimbers.OrderByDescending(p => p.iid).First().iid + 1; }
                    catch { nextIid = 1; }
                    climberToSet = ONLclimber.CreateONLclimber(nextIid,
                        serClm.Climber.genderFemale,
                        serClm.Climber.birthdate,
                        false, false);
                    dc.ONLclimbers.AddObject(climberToSet);
                }
                else
                    climberToSet = clmListToUpdate.First();

                climberToSet.surname = serClm.Climber.surname;
                climberToSet.name = serClm.Climber.name;
                climberToSet.birthdate = serClm.Climber.birthdate;

                if (serClm.ContainsPhoto)
                    climberToSet.photo = serClm.Climber.photo;
                dc.SaveChanges();

                int newTeamID = PostTeam(serClm.Team);
                if (newTeamID < 1)
                    return newTeamID;
                int newGroupID = PostGroup(serClm.Group);
                if (newGroupID < 1)
                    return newGroupID;

                var lstL = from l in dc.ONLClimberCompLinks
                           where l.comp_id == serClm.Link.comp_id
                           && l.secretary_id == serClm.Link.secretary_id
                           select l;
                ONLClimberCompLink lnkToUpdate;
                if (lstL.Count() < 1)
                {
                    long newId;
                    try { newId = dc.ONLClimberCompLinks.OrderByDescending(l => l.iid).First().iid + 1; }
                    catch { newId = 1; }
                    lnkToUpdate = new ONLClimberCompLink();
                    lnkToUpdate.iid = newId;
                    lnkToUpdate.comp_id = serClm.Link.comp_id;
                    
                    lnkToUpdate.appl_type = String.Empty;
                    lnkToUpdate.is_changeble = false;
                    lnkToUpdate.sys_date_create = DateTime.UtcNow;
                    dc.ONLClimberCompLinks.AddObject(lnkToUpdate);
                }
                else
                    lnkToUpdate = lstL.First();
                lnkToUpdate.boulder = serClm.Link.boulder;
                lnkToUpdate.climber_id = climberToSet.iid;
                lnkToUpdate.group_id = newGroupID;
                lnkToUpdate.lead = serClm.Link.lead;
                lnkToUpdate.nopoints = serClm.Link.nopoints;
                lnkToUpdate.qf = serClm.Link.qf;
                lnkToUpdate.queue_pos = 0;
                lnkToUpdate.rankingBoulder = serClm.Link.rankingBoulder;
                lnkToUpdate.rankingLead = serClm.Link.rankingLead;
                lnkToUpdate.rankingSpeed = serClm.Link.rankingSpeed;
                lnkToUpdate.replacementID = null;
                lnkToUpdate.secretary_id = serClm.Link.secretary_id;
                lnkToUpdate.speed = serClm.Link.speed;
                lnkToUpdate.state = Constants.CLIMBER_CONFIRMED;
                lnkToUpdate.sys_date_update = DateTime.UtcNow;
                lnkToUpdate.team_id = newTeamID;
                lnkToUpdate.vk = serClm.Link.vk;
                dc.SaveChanges();

                ClimbingConfirmations.DeleteDeadClimbers(dc);

                return lnkToUpdate.iid;
            }
            catch { return -1; }
        }

        private int DeleteClimbersWhere(int[] iids, Func<ONLClimberCompLink, bool> where)
        {
            try
            {
                var v = dc.ONLClimberCompLinks.Where(where);
                if (v.Count() < 1)
                    return 0;
                var toDelAr = v.ToArray();
                for (int i = 0; i < toDelAr.Length; i++)
                    dc.ONLClimberCompLinks.DeleteObject(toDelAr[i]);
                dc.SaveChanges();
                ClimbingConfirmations.DeleteDeadClimbers(dc);
                return toDelAr.Length;
            }
            catch { return -1; }
        }

        [WebMethod]
        public int DeleteClimbersNotInIid(int[] iids, long compID)
        {
            return DeleteClimbersWhere(iids, l => l.comp_id == compID && !iids.Contains(l.secretary_id));
        }

        [WebMethod]
        public int DeleteClimbersBySecretaryID(int[] iids, long compID)
        {
            return DeleteClimbersWhere(iids, l => l.comp_id == compID && iids.Contains(l.secretary_id));
        }

        [WebMethod]
        public int DeleteGroupsNotInIid(int[] iids, long compID)
        {
            try
            {
                var v = dc.ONLGroupsCompLinks.Where(l => l.comp_id==compID&& !iids.Contains(l.group_id)).ToArray();
                foreach (var l in v)
                    dc.ONLGroupsCompLinks.DeleteObject(l);
                dc.SaveChanges();
                return v.Length;
            }
            catch { return -1; }
        }

        [WebMethod]
        public int DeleteTeamsNotInSecretaryId(int[] iids, long compID)
        {
            try
            {
                var v = dc.ONLTeamsCompLinks.Where(lnk => lnk.comp_id == compID && !iids.Contains(lnk.secretary_id)).ToArray();
                foreach (var l in v)
                    dc.ONLTeamsCompLinks.DeleteObject(l);
                dc.SaveChanges();
                return v.Length;
            }
            catch { return -1; }
        }

        #endregion
    }

    #region CLASSES

    [Serializable]
    public sealed class PairingValue<T1, T2>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public PairingValue() { Value1 = default(T1); Value2 = default(T2); }

        public PairingValue(T1 value1, T2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }

    [Serializable]
    public sealed class ONLGroupSerializable
    {
        public int Iid { get; set; }
        public long CompID { get; set; }
        public string Name { get; set; }
        public int minQf { get; set; }
        public int yearOld { get; set; }
        public int yearYoung { get; set; }
        public bool GenderFemale { get; set; }

        public ONLGroupSerializable() { }
        public ONLGroupSerializable(ONLGroupsCompLink lnk)
        {
            this.Iid = lnk.group_id;
            this.CompID = lnk.comp_id;
            this.Name = lnk.ONLGroup.name;
            this.minQf = lnk.minQf;
            int compYear = lnk.ONLCompetition.GetCompYear();
            this.yearOld = compYear - lnk.ONLGroup.oldYear;
            this.yearYoung = compYear - lnk.ONLGroup.youngYear;
            this.GenderFemale = lnk.ONLGroup.genderFemale;
        }
    }

    [Serializable]
    public sealed class ONLTeamSerializable
    {
        public int Iid { get; set; }
        public long CompID { get; set; }
        public int SecretaryId { get; set; }
        public string Name { get; set; }
        public int RankingPos { get; set; }

        public string GroupName { get; set; }
        public int? GroupID { get; set; }
        public bool GroupSet { get; set; }
        public ONLTeamSerializable() { }
        public ONLTeamSerializable(ONLTeamsCompLink lnk)
        {
            this.Iid = lnk.team_id;
            this.CompID = lnk.comp_id;
            this.SecretaryId = lnk.secretary_id;
            this.Name = lnk.ONLteam.name;
            this.RankingPos = lnk.ranking_pos;

            this.GroupSet = (lnk.ONLteam.group_id != null);
            this.GroupID = this.GroupSet ? lnk.ONLteam.group_id : null;
            this.GroupName = this.GroupSet ? lnk.ONLteam.ONLTeamGroup.name : String.Empty;
        }
    }

    [Serializable]
    public sealed class ONLCompetitionSerializable
    {
        public ONLCompetitionParamSerializable[] Params { get; set; }
        public ONLCompetition Competition { get; set; }
        public ONLCompetitionSerializable() { }
        public ONLCompetitionSerializable(ONLCompetition comp)
        {
            this.Competition = comp;
            List<ONLCompetitionParamSerializable> pList = new List<ONLCompetitionParamSerializable>();
            foreach (var p in comp.ONLCompetitionParams)
                pList.Add(new ONLCompetitionParamSerializable(p));
            Params = pList.ToArray();
        }
    }

    [Serializable]
    public sealed class ONLCompetitionParamSerializable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public byte[] BinaryValue { get; set; }
        public ONLCompetitionParamSerializable() { }
        public ONLCompetitionParamSerializable(ONLCompetitionParam p)
        {
            this.Name = p.ParamName;
            this.Value = p.ParamValue;
            if (p.ONLCompetitionBinaryParam != null)
                this.BinaryValue = p.ONLCompetitionBinaryParam.paramValue;
            else
                this.BinaryValue = null;
        }
    }

    [Serializable]
    public sealed class ONLClimberLinkSerializable
    {
        public bool ContainsPhoto { get; set; }
        public ONLClimberCompLink Link { get; set; }
        public ONLclimber Climber { get; set; }
        public ONLGroupSerializable Group { get; set; }
        public ONLTeamSerializable Team { get; set; }
        public ONLClimberLinkSerializable() { this.ContainsPhoto = false; }
        public ONLClimberLinkSerializable(ONLClimberCompLink lnkBase, bool GetPhoto = true, Entities dc = null)
        {
            this.Climber = lnkBase.ONLclimber;
            this.Link = lnkBase;
            this.ContainsPhoto = GetPhoto;

            ONLTeamsCompLink lnkToSet;
            try { lnkToSet = lnkBase.ONLteam.ONLTeamsCompLinks.First(l => l.comp_id == lnkBase.comp_id); }
            catch { lnkToSet = null; }
            this.Team = (lnkToSet == null) ? null : new ONLTeamSerializable(lnkToSet);

            ONLGroupsCompLink lnkG;
            try { lnkG = lnkBase.ONLGroup.ONLGroupsCompLinks.First(gl => gl.comp_id == lnkBase.comp_id); }
            catch { lnkG = null; }
            this.Group = (lnkG == null) ? null : new ONLGroupSerializable(lnkG);

            if (!this.ContainsPhoto && dc != null && this.Climber.photo != null)
            {
                if (this.Climber.EntityState != EntityState.Detached)
                    dc.Detach(this.Climber);
                this.Climber.photo = null;
            }
        }
    }

    [Serializable]
    public class ClimbersResult
    {
        public int ListID_Secretary { get; set; }
        public int ClimberID_Secretary { get; set; }
        public int? Start { get; set; }
        public long? Res { get; set; }
        public bool PreQf { get; set; }
        public int? Pos { get; set; }

        public ClimbersResult(int listID, int climberID)
            : this()
        {
            this.ListID_Secretary = listID;
            this.ClimberID_Secretary = climberID;
        }

        public ClimbersResult()
        {
            this.ListID_Secretary = this.ClimberID_Secretary = -1;
            this.PreQf = false;
            this.Start = null;
            this.Res = null;
            this.Pos = null;
        }

        public ClimbersResult(ClimbersResult source)
        {
            this.ListID_Secretary = source.ListID_Secretary;
            this.ClimberID_Secretary = source.ClimberID_Secretary;
            this.Pos = source.Pos;
            this.PreQf = source.PreQf;
            this.Res = source.Res;
            this.Start = source.Start;
        }
    }

    [Serializable]
    public sealed class LeadResult : ClimbersResult
    {
        public string TextRes { get; set; }
        public LeadResult(int listID, int climberID)
            : base(listID, climberID)
        {
            this.TextRes = String.Empty;
        }

        public LeadResult() : base() { this.TextRes = String.Empty; }

        public LeadResult(ClimbersResult source)
            : base(source)
        {
            LeadResult src = source as LeadResult;
            if (src != null)
                this.TextRes = src.TextRes;
        }
    }

    [Serializable]
    public sealed class ListHeader
    {
        public int SecretaryID { get; set; }
        public string Round { get; set; }
        public string Style { get; set; }
        public int? SecretaryID_Parent { get; set; }
        public string ListType { get; set; }
        public int? GroupID { get; set; }
        public int? RouteNumber { get; set; }
        public bool Live { get; set; }
        public int Quote { get; set; }
        public int? PrevRound_SecretaryID { get; set; }
        public string StartTime { get; set; }
        public bool RoundFinished { get; set; }
        public DateTime? LastUpdate { get; set; }

        public ListHeader()
        {
            this.SecretaryID = 0;
            this.Round = this.Style = String.Empty;
            this.SecretaryID_Parent = null;
            this.ListType = ListTypeEnum.Unknown.ToString();
            this.GroupID = null;
            this.RouteNumber = null;
            this.Live = false;
            this.Quote = 0;
            this.PrevRound_SecretaryID = null;
            this.StartTime = String.Empty;
            this.RoundFinished = false;
            this.LastUpdate = null;
        }
    }

    [Serializable]
    public sealed class SpeedResult : ClimbersResult
    {
        public string Route1 { get; set; }
        public string Route2 { get; set; }
        public string TextRes { get; set; }
        public string Qf { get; set; }

        public SpeedResult()
            : base()
        {
            this.Route1 = this.Route2 = this.TextRes = this.Qf = String.Empty;
        }

        public SpeedResult(ClimbersResult source)
            : base(source)
        {
            SpeedResult src = source as SpeedResult;
            if (src != null)
            {
                this.Route1 = src.Route1;
                this.Route2 = src.Route2;
                this.TextRes = src.TextRes;
                this.Qf = src.Qf;
            }
        }
    }

    [Serializable]
    public sealed class BoulderRouteResult
    {
        public int RouteNumber { get; set; }
        public int? TopAttempt { get; set; }
        public int? BonusAttempt { get; set; }
        public BoulderRouteResult()
        {
            this.RouteNumber = 0;
            this.TopAttempt = null;
            this.BonusAttempt = null;
        }

        public BoulderRouteResult(BoulderRouteResult source)
        {
            this.RouteNumber = source.RouteNumber;
            this.TopAttempt = source.TopAttempt;
            this.BonusAttempt = source.BonusAttempt;
        }
    }

    [Serializable]
    public sealed class BoulderResult : ClimbersResult
    {
        public int? Tops { get; set; }
        public int? TopAttempts { get; set; }
        public int? Bonuses { get; set; }
        public int? BonusAttempts { get; set; }
        public bool DNS { get; set; }
        public bool DSQ { get; set; }

        private BoulderRouteResult[] routes = null;
        public BoulderRouteResult[] RouteData
        {
            get
            {
                if (routes == null)
                    routes = new BoulderRouteResult[0];
                return routes;
            }
            set { routes = value; }
        }

        public BoulderResult()
        {
            this.Tops = this.TopAttempts = this.Bonuses = this.BonusAttempts = null;
            this.DNS = this.DSQ = false;
        }

        public BoulderResult(ClimbersResult source)
            : base(source)
        {
            BoulderResult src = source as BoulderResult;
            if (src != null)
            {
                this.Tops = src.Tops;
                this.Bonuses = src.Bonuses;
                this.TopAttempts = src.TopAttempts;
                this.BonusAttempts = src.BonusAttempts;
                this.DNS = src.DNS;
                this.DSQ = src.DSQ;
                if (src.routes == null)
                    this.routes = new BoulderRouteResult[0];
                else
                {
                    this.routes = new BoulderRouteResult[src.routes.Length];
                    for (int i = 0; i < this.routes.Length; i++)
                        this.routes[i] = new BoulderRouteResult(src.routes[i]);
                }
            }
        }
    }
    /*
    [Serializable]
    public sealed class ONLteamSerializable
    {
        public ONLteam Team { get; set; }
        public ONLteamSerializable(){this.Team = null;}
        public ONLteamSerializable(ONLteam t)
        {
            this.Team = t;
        }
    }

    [Serializable]
    public sealed class ONLGroupSerializable
    {
        public ONLGroup Group { get; set; }
        public ONLGroupSerializable() { this.Group = null; }
        public ONLGroupSerializable(ONLGroup g)
        {
            this.Group = g;
        }
    }
     */
    #endregion
}
