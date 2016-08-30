// <copyright file="API_ResultsController.cs">
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebClimbing.Models;
using XmlApiData;
using WebClimbing.ServiceClasses;
using System.Threading;
using System.Runtime.CompilerServices;
using ClimbingCompetition;
using System.Data;

namespace WebClimbing.Controllers
{
    [AllowAnonymous]
    public class ResultsController : ApiController
    {
        private ClimbingContext db = new ClimbingContext();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        private static object listLocker = new object();
        private ListHeaderModel UpdateListHeader(CompetitionModel comp, ApiListHeader header)
        {
            ListHeaderModel saveList = comp.Lists.FirstOrDefault(l => l.LocalIid == header.Iid);
            if (saveList == null)
            {
                    saveList = new ListHeaderModel { LocalIid = header.Iid };
                comp.Lists.Add(saveList);
            }
            saveList.BestResultInQf = header.BestQf;
            saveList.CompetitionRules = header.Rules;
            if (header.GroupId == null)
                saveList.GroupId = null;
            else
            {
                var agr = comp.AgeGroups.FirstOrDefault(g => g.AgeGroupId == header.GroupId.Value);
                if (agr == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                saveList.GroupId = agr.Iid;
            }
            saveList.IidParent = header.ParentList;
            saveList.LastRefresh = header.LastRefresh;
            saveList.ListType = header.ListType;
            saveList.Live = header.Live;
            saveList.PreviousRoundId = header.PreviousRound;
            saveList.Quota = header.Quota;
            saveList.Round = header.Round;
            saveList.RouteQuantity = header.RouteQuantity;
            saveList.StartTime = header.StartTime;
            saveList.Style = header.Style;
            return saveList;
        }

        [HttpPost]
        [ActionName("PostListHeader")]
        public HttpResponseMessage PostListHeader(APISignedRequest request)
        {
            CompetitionModel comp;
            var listHeader = request.GetRequestParameter<ApiListHeader>(db, out comp);
            bool hasMutex = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.Enter(listLocker, ref hasMutex);
                UpdateListHeader(comp, listHeader);
                db.SaveChanges();
            }
            finally
            {
                if (hasMutex)
                    Monitor.Exit(listLocker);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("ReloadAllLists")]
        public HttpResponseMessage ReloadAllLists(APISignedRequest request)
        {
            CompetitionModel comp;
            var listCollection = request.GetRequestParameter<ApiListHeaderCollection>(db, out comp);
            bool hasLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.Enter(listLocker, ref hasLock);
                foreach (var l in comp.Lists.ToArray())
                {
                    foreach (var r in l.ResultsBoulder.ToArray())
                    {
                        foreach (var br in r.Routes.ToArray())
                            db.BoulderRoutes.Remove(br);
                        db.Results.Remove(r);
                    }
                    foreach (var r in l.Results.ToArray())
                        db.Results.Remove(r);

                    db.Lists.Remove(l);
                }
                comp.Lists.Clear();
                foreach (var l in listCollection.Data)
                    UpdateListHeader(comp, l);
                db.SaveChanges();
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(listLocker);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("PostListLine")]
        public HttpResponseMessage PostListLine(APISignedRequest request)
        {
            CompetitionModel comp;
            var resultListElem = request.GetRequestParameter<ApiListLine>(db, out comp);
            bool hasLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.Enter(listLocker, ref hasLock);
                db.SaveChanges();
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(listLocker);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("ReloadResultList")]
        public HttpResponseMessage ReloadListPackage(APISignedRequest request)
        {
            return LoadListPackage(request, true);
        }

        [HttpPost]
        [ActionName("LoadResultsPackage")]
        public HttpResponseMessage LoadResultsPackage(APISignedRequest request)
        {
            return LoadListPackage(request, false);
        }

        private HttpResponseMessage LoadListPackage(APISignedRequest request, bool clearLists)
        {
            CompetitionModel comp;
            var resultListElem = request.GetRequestParameter<ApiListLineCollection>(db, out comp);
            bool hasLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.Enter(listLocker, ref hasLock);

                Dictionary<int, ListHeaderModel> lists = new Dictionary<int, ListHeaderModel>();
                if (resultListElem.Data != null)
                {
                    foreach (var n in resultListElem.Data.Select(d => d.ListID).Distinct())
                    {
                        var cList = comp.Lists.FirstOrDefault(l => l.LocalIid == n);
                        if (cList != null)
                            lists.Add(n, cList);
                    }

                    if (clearLists)
                    {
                        foreach (var cList in lists.Values)
                        {
                            foreach (var result in cList.Results.ToArray())
                                db.Results.Remove(result);
                        }
                    }

                    foreach (var res in resultListElem.Data.OfType<ApiListLineLead>())
                    {
                        if (!lists.ContainsKey(res.ListID))
                            continue;
                        var climber = comp.Climbers.FirstOrDefault(c => c.SecretaryId == res.ClimberID);
                        if (climber == null)
                            continue;
                        var model = lists[res.ListID].ResultsLead.FirstOrDefault(r => r.ClimberId == climber.Iid);
                        if (model == null)
                        {
                            var otherStyle = lists[res.ListID].Results.FirstOrDefault(r => r.ClimberId == climber.Iid);
                            if (otherStyle != null)
                                db.Results.Remove(otherStyle);
                            model = new LeadResultLine { Climber = climber };
                            lists[res.ListID].Results.Add(model);
                        }
                        model.ResText = String.IsNullOrWhiteSpace(res.ResText) ? String.Empty : res.ResText.Trim();
                        model.Result = res.Result;
                        model.StartNumber = res.StartNumber;
                        model.Time = res.Time ?? int.MaxValue;
                        if (model.Time < 1)
                            model.Time = int.MaxValue;
                        model.TimeText = String.IsNullOrWhiteSpace(res.TimeText) ? String.Empty : res.TimeText.Trim();
                        model.PreQf = res.PreQf;
                    }

                    foreach (var res in resultListElem.Data.OfType<ApiListLineSpeed>())
                    {
                        if (!lists.ContainsKey(res.ListID))
                            continue;
                        var climber = comp.Climbers.FirstOrDefault(c => c.SecretaryId == res.ClimberID);
                        if (climber == null)
                            continue;
                        var model = lists[res.ListID].ResultsSpeed.FirstOrDefault(r => r.ClimberId == climber.Iid);
                        if (model == null)
                        {
                            var otherStyle = lists[res.ListID].Results.FirstOrDefault(r => r.ClimberId == climber.Iid);
                            if (otherStyle != null)
                                db.Results.Remove(otherStyle);
                            model = new SpeedResultLine { Climber = climber };
                            lists[res.ListID].Results.Add(model);
                        }
                        model.ResText = String.IsNullOrWhiteSpace(res.ResText) ? String.Empty : res.ResText.Trim();
                        model.Result = res.Result;
                        model.Route1Data = res.Route1Data;
                        model.Route1Text = String.IsNullOrWhiteSpace(res.Route1Text) ? String.Empty : res.Route1Text.Trim();
                        model.Route2Data = res.Route2Data;
                        model.Route2Text = String.IsNullOrWhiteSpace(res.Route2Text) ? String.Empty : res.Route2Text.Trim();
                        model.StartNumber = res.StartNumber;
                        model.Position = res.Pos;
                        model.PosText = res.PosText;
                        model.PreQf = res.PreQf;
                        model.Qf = res.Qf;
                    }

                    foreach (var res in resultListElem.Data.OfType<ApiListLineBoulder>())
                    {
                        if (!lists.ContainsKey(res.ListID))
                            continue;
                        var climber = comp.Climbers.FirstOrDefault(c => c.SecretaryId == res.ClimberID);
                        if (climber == null)
                            continue;
                        var model = lists[res.ListID].ResultsBoulder.FirstOrDefault(r => r.ClimberId == climber.Iid);
                        if (model == null)
                        {
                            var otherStyle = lists[res.ListID].Results.FirstOrDefault(r => r.ClimberId == climber.Iid);
                            if (otherStyle != null)
                                db.Results.Remove(otherStyle);
                            model = new BoulderResultLine { Climber = climber, Routes = new List<BoulderResultRoute>() };
                            lists[res.ListID].Results.Add(model);
                        }
                        model.PreQf = res.PreQf;
                        model.StartNumber = res.StartNumber;
                        switch (res.ResultCode)
                        {
                            case ResultLabel.DNS:
                                model.ResText = "н/я";
                                model.Routes.Clear();
                                model.ResultTypeValue = ResultType.DidNotStarted;
                                break;
                            case ResultLabel.DSQ:
                                model.ResText = "дискв.";
                                model.Routes.Clear();
                                model.ResultTypeValue = ResultType.Disqualified;
                                break;
                            default:
                                model.ResText = String.Empty;
                                model.ResultTypeValue = ResultType.HasResult;
                                var updatedRoutes = res.Routes.Where(r => r.Route > 0 && r.Route <= (lists[res.ListID].RouteQuantity ?? 0)).ToArray();
                                foreach (var routeData in updatedRoutes)
                                {
                                    var modelRoute = model.Routes.FirstOrDefault(r => r.Route == routeData.Route);
                                    if (modelRoute == null)
                                    {
                                        modelRoute = new BoulderResultRoute { Route = routeData.Route };
                                        model.Routes.Add(modelRoute);
                                    }
                                    modelRoute.Top = routeData.Top;
                                    modelRoute.Bonus = routeData.Bonus;
                                }
                                var routesToDelete = model.Routes
                                                    .ToArray()
                                                    .Where(r => updatedRoutes.Count(ur => ur.Route == r.Route) < 1)
                                                    .ToArray();
                                foreach (var r in routesToDelete)
                                    db.BoulderRoutes.Remove(r);
                                break;
                        }

                    }
                }

                foreach (var lst in lists.Values)
                    FinalizeList(lst);

                db.SaveChanges();
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(listLocker);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void FinalizeList(ListHeaderModel model)
        {
            var speedRes = model.ResultsSpeed;
            if(speedRes.Count() > 0)
                FinalizeListSpeed(model, speedRes);
            else
            {
                var leadRes = model.ResultsLead;
                if (leadRes.Count() > 0)
                    FinalizeListLead(model, leadRes);
                else
                {
                    var boulderRes = model.ResultsBoulder;
                    if (boulderRes.Count() > 0)
                        FinalizeListBoulder(model, boulderRes);
                }

            }
            var chainList = (model.IidParent == null) ? model : model.Parent;
            foreach (var list in chainList.NextRounds)
                FinalizeList(list);
        }

        private void FinalizeListLead(ListHeaderModel model, IEnumerable<LeadResultLine> results)
        {
            FinalizeList(model, results, (r => r.Result != null && !r.ResText.Equals(String.Empty, StringComparison.Ordinal)),
                (a, b) => b.Result.Value.CompareTo(a.Result.Value),
                r => r.Result.Value,
                r => r.ResText.ToLowerInvariant().Contains("дискв"),
                r => r.ResText.ToLowerInvariant().Contains("н/я"),
                model.IidParent != null && model.Parent.ListType == ListTypeEnum.LeadFlash,
                false,
                l =>
                {
                    if (l.Time == null || l.Time.Value == int.MaxValue || l.Time.Value <= 0)
                        return -1;
                    else
                        return l.Time.Value;
                }
                );
        }

        

        private void FinalizeListSpeed(ListHeaderModel model, IEnumerable<SpeedResultLine> results)
        {
            //throw new NotImplementedException();
            Func<SpeedResultLine, long> resultSelector;
            Func<SpeedResultLine, bool> selector;
            switch (model.ListType)
            {
                case ListTypeEnum.SpeedQualy:
                    resultSelector = (r=>r.Result.Value);
                    selector = (r => r.Result != null && r.Result.Value > 0);
                    break;
                case ListTypeEnum.SpeedQualy2:
                    if (model.BestResultInQf)
                    {
                        List<ListHeaderModel> qfChain = new List<ListHeaderModel>();
                        qfChain.Add(model);
                        ListHeaderModel curLst = model;
                        while (curLst.PreviousRoundId != null)
                        {
                            curLst = curLst.PreviousRound;
                            qfChain.Add(curLst);
                        }
                        resultSelector = (r =>
                        {
                            long? bestRes = null;
                            foreach (var list in qfChain)
                            {
                                var clmRes = list.ResultsSpeed.FirstOrDefault(qr => qr.Climber.SecretaryId == r.Climber.SecretaryId);
                                if (clmRes == null || clmRes.Result == null || clmRes.Result.Value <= 0)
                                    continue;
                                if (bestRes == null || clmRes.Result.Value < bestRes.Value)
                                    bestRes = clmRes.Result.Value;
                            }
                            return bestRes ?? -1;
                        });
                        selector = (r => resultSelector(r) > 0);
                        break;
                    }
                    else
                        goto case ListTypeEnum.SpeedQualy;
                default:
                    return;
            }
            FinalizeList(model, results, selector,
                (a, b) => resultSelector(a).CompareTo(resultSelector(b)),
                resultSelector,
                r => r.ResText.ToLowerInvariant().Contains("дискв"),
                r => r.ResText.ToLowerInvariant().Contains("н/я"),
                false,
                model.ListType== ListTypeEnum.SpeedQualy && model.BestResultInQf,
                null);
        }

        private static long getResult(BoulderResultLine line)
        {
            if (line == null)
                return long.MaxValue;
            switch (line.ResultTypeValue)
            {
                case ResultType.DidNotStarted:
                    return long.MaxValue - 1;
                case ResultType.Disqualified:
                    return long.MaxValue - 2;
                default:
                    long ba = 1000 - (line.BonusAttempts ?? 1000);
                    long b = line.Bonus ?? 0;
                    long ta = 1000 - (line.TopAttempts ?? 1000);
                    long t = line.Top ?? 0;
                    long result = ba + b * 1000;
                    result += ta * 1000 * 1000;
                    result += t * 1000 * 1000 * 1000;
                    return result;
            }
        }

        private void FinalizeListBoulder(ListHeaderModel model, IEnumerable<BoulderResultLine> results)
        {
            FinalizeList(model, results,
                r => r.Routes.Count(br => br.Bonus != null) > 0,
                (a, b) =>
                {
                    var aRes = getResult(a);
                    var bRes = getResult(b);
                    if (aRes != bRes)
                        return bRes.CompareTo(aRes);
                    return a.Climber.Vk.CompareTo(b.Climber.Vk);
                },
                getResult,
                //r => (1000 - r.BonusAttempts.Value) + r.Bonus.Value * 1000 + (1000 - r.TopAttempts.Value) * 1000 * 1000 + 1000 * 1000 * 1000 * r.Top.Value,
                    r => r.ResultTypeValue == ResultType.Disqualified,
                    r => r.ResultTypeValue == ResultType.DidNotStarted,
                    false, false, null);
        }



        private void FinalizeList<T>(ListHeaderModel model, IEnumerable<T> results,
            Func<T, bool> selectorWithResult,
            Comparison<T> sortingComparison,
            Func<T, long> resultSelector,
            Func<T, bool> isDSQ,
            Func<T, bool> isDNS,
            bool isLeadQualy,
            bool forceQf,
            Func<T, int> leadTimeSelector
            )
            where T : ListLineModel
        {
            var dt = SortingClass.CreateStructure();
            
            var posCol = dt.Columns.Add("pos0", typeof(int));
            DataColumn rNumCol;
            long curRes = -1;
            int curPos = 1;
            int i = 0;
            HashSet<long> climbersWithResult = new HashSet<long>();
            var resultList = results.Where(r => selectorWithResult(r)).ToList();
            resultList.Sort(sortingComparison);
            foreach (var res in resultList)
            {
                i++;
                long rowRes = resultSelector(res);
                if (curRes < 0 || rowRes != curRes)
                {
                    curRes = rowRes;
                    curPos = i;
                }
                int sp;
                if (isDSQ(res))
                    sp = 1;
                else if (isDNS(res))
                    sp = 2;
                else
                    sp = 0;
                dt.Rows.Add(res.Climber.Vk, 0, String.Empty, res.Climber.SecretaryId.Value, String.Empty,
                            0.0, String.Empty, sp, curPos);
                climbersWithResult.Add(res.Climber.SecretaryId.Value);
            }
            
            if (dt.Rows.Count > 0)
            {
                var currentList = model;
                i = 0;
                while (currentList.PreviousRoundId != null)
                {
                    currentList = currentList.PreviousRound;
                    rNumCol = dt.Columns.Add(String.Format("rNum{0}", ++i), typeof(int));
                    posCol = dt.Columns.Add(String.Format("pos{0}", i), typeof(int));
                    var roundRes = GetResultsByRound(currentList);
                    foreach (DataRow row in dt.Rows)
                    {
                        int iid = (int)row["iid"];
                        if (roundRes == null || !roundRes.ContainsKey(iid))
                        {
                            row[rNumCol] = 0;
                            row[posCol] = 1;
                        }
                        else
                        {
                            var r = roundRes[iid];
                            row[rNumCol] = r.RouteNum;
                            row[posCol] = r.Pos;
                        }
                    }
                }

                SortingClass.SortResults(dt, model.Quota, i == 0, isLeadQualy, model.CompetitionRules, forceQf);
                if (leadTimeSelector != null)
                {
                    var dtWithTime = SortingClass.CreateStructure();
                    posCol = dtWithTime.Columns.Add("pos0");
                    rNumCol = dtWithTime.Columns.Add("rNum1");
                    var timeCol = dtWithTime.Columns.Add("pos1");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var clmResult = resultList.FirstOrDefault(r => r.Climber.SecretaryId == (int)dr[3]);
                        int time = clmResult == null ? -1 : leadTimeSelector(clmResult);
                        dtWithTime.Rows.Add(dr[0], 0, String.Empty, dr[3], String.Empty, 0.0, String.Empty, dr[7],
                            dr[1], time < 0 ? 0 : 1, time < 0 ? 1 : time);
                    }
                    SortingClass.SortResults(dtWithTime, model.Quota, i == 0, isLeadQualy, model.CompetitionRules, forceQf);
                    dt = dtWithTime;
                }
                foreach (DataRow row in dt.Rows)
                {
                    var climberRes = model.Results.First(r => r.Climber.SecretaryId == (int)row["iid"]);
                    climberRes.Position = (int)row["pos"];
                    climberRes.PosText = (string)row["posText"];
                    climberRes.Qf = (string)row["qf"];
                    climberRes.Points = (double)row["pts"];
                    climberRes.PointsText = (string)row["ptsText"];
                }
            }
            foreach (var clmRes in model.Results.ToArray().Where(r => !climbersWithResult.Contains(r.Climber.SecretaryId.Value)))
            {
                clmRes.Position = null;
                clmRes.PosText = String.Empty;
                clmRes.Qf = String.Empty;
                clmRes.Points = null;
                clmRes.PointsText = String.Empty;
            }
        }

        private Dictionary<int, RoundResult> GetResultsByRound(ListHeaderModel model)
        {
            switch (model.ListType)
            {
                case ListTypeEnum.LeadFlash:
                    return GetFlashResult(model);
                case ListTypeEnum.LeadGroups:
                case ListTypeEnum.BoulderGroups:
                    return GetSeveralGroupsResult(model);
                default:
                    return GetSimpleResult(model);
            }
        }

        private static Dictionary<int, RoundResult> GetSimpleResult(ListHeaderModel model)
        {
            Dictionary<int, RoundResult> result = new Dictionary<int, RoundResult>();
            foreach (var res in model.Results)
            {
                var rr = new RoundResult
                {
                    ClimberId = res.Climber.SecretaryId.Value,
                    Pos = res.Position ?? 0,
                    Pts = res.Points ?? 0.0,
                    RouteNum = res.PreQf ? 0 : ((res.Position == null) ? int.MaxValue : 1),
                    ResText = res.ResText??String.Empty
                };
                result.Add(rr.ClimberId, rr);
            }
            return result;
        }

        public static Dictionary<int, RoundResult> GetFlashResult(ListHeaderModel model)
        {
            List<Dictionary<int, RoundResult>> routes = new List<Dictionary<int, RoundResult>>();
            foreach (var route in model.Children.OrderBy(r => r.Round).ToArray())
                routes.Add(GetSimpleResult(route));
            if (routes.Count < 1)
                return null;
            int peopleCount = routes.Max(rd => rd.Count);
            if (peopleCount < 1)
                return null;
            Dictionary<int, RoundResult> results = new Dictionary<int, RoundResult>();
            for (int i = 0; i < routes.Count; i++)
            {
                foreach (var climber in routes[i].Values)
                {
                    FlashRoundResult fRes;
                    if (results.ContainsKey(climber.ClimberId))
                        fRes = (FlashRoundResult)results[climber.ClimberId];
                    else
                    {
                        fRes = new FlashRoundResult { ClimberId = climber.ClimberId };
                        results.Add(fRes.ClimberId, fRes);
                    }
                    if (fRes.Routes.Count < i)
                        for (int k = fRes.Routes.Count; k < i; k++)
                            fRes.Routes.Add(new RoundResult { ClimberId = fRes.ClimberId, Pos = peopleCount, Pts = peopleCount, RouteNum = 1, ResText = String.Empty });
                    fRes.Routes.Add(new RoundResult
                    {
                        ClimberId = fRes.ClimberId,
                        Pos = (climber.Pos <= 0) ? peopleCount : climber.Pos,
                        Pts = (climber.Pts <= 0.0) ? (double)peopleCount : climber.Pts,
                        RouteNum = (climber.RouteNum == 0) ? 0 : 1,
                        ResText = climber.ResText
                    });
                }
            }
            foreach (var fRes in results.Values.OfType<FlashRoundResult>().Where(r => r.Routes.Count < routes.Count))
                for (int i = fRes.Routes.Count; i < routes.Count; i++)
                    fRes.Routes.Add(new RoundResult { ClimberId = fRes.ClimberId, Pos = peopleCount, Pts = peopleCount, RouteNum = 1 });
            var resultList = results.Values.ToList();
            resultList.Sort((a, b) => a.Pts.CompareTo(b.Pts));
            double curPts = -1.0;
            int curPos = 1;
            for (int i = 0; i < resultList.Count; i++)
            {
                var curClm = resultList[i];
                var rowPts = curClm.Pts;
                if (i == 0 || rowPts > curPts)
                {
                    curPos = (i + 1);
                    curPts = rowPts;
                }
                curClm.Pos = curPos;
            }
            return results;
        }

        private Dictionary<int, RoundResult> GetSeveralGroupsResult(ListHeaderModel model)
        {
            Dictionary<int, RoundResult> result = new Dictionary<int, RoundResult>();
            int i=0;
            foreach (var list in model.Children)
            {
                i++;
                foreach (var res in list.Results)
                    result.Add(res.Climber.SecretaryId.Value,
                        new RoundResult
                        {
                            ClimberId = res.Climber.SecretaryId.Value,
                            Pos = res.Position ?? int.MaxValue,
                            Pts = res.Points ?? double.MaxValue,
                            RouteNum = res.PreQf ? 0 : i
                        });
            }
            return result;
        }

        public class RoundResult
        {
            public int ClimberId { get; set; }
            public virtual int RouteNum { get; set; }
            public int Pos { get; set; }
            public virtual double Pts { get; set; }
            public String ResText { get; set; }
        }
        public class FlashRoundResult : RoundResult
        {
            public List<RoundResult> Routes { get; private set; }
            public FlashRoundResult()
            {
                this.Routes = new List<RoundResult>();
            }

            public override int RouteNum
            {
                get
                {
                    if (Routes.Count(r => r.RouteNum == 0) > 0)
                        return 0;
                    else
                        return 1;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override double Pts
            {
                get
                {
                    if (Routes.Count < 1 || this.RouteNum == 0)
                        return 0.0;
                    double value = Routes[0].Pts;
                    for (int i = 1; i < Routes.Count; i++)
                        value *= Routes[i].Pts;
                    int nValue = (int)(100.0 * Math.Pow(value, 1.0 / ((double)Routes.Count)));
                    return ((double)nValue) / 100.0;
                }
                set { throw new NotImplementedException(); }
            }
        }
    }
}
