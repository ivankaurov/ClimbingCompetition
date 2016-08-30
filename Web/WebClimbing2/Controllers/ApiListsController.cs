// <copyright file="ApiListsController.cs">
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

﻿using ClimbingCompetition.Common.API;
using ClimbingEntities.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace WebClimbing2.Controllers
{
    public class ApiListsController : __ApiBaseController
    {
        private void SortChangedLists(HashSet<string> changedLists)
        {
            if (changedLists == null || changedLists.Count < 1)
                return;

            foreach (var list in Context.ResultLists.Where(l => changedLists.Contains(l.Iid)).ToArray())
                list.Sort(Context);
            Context.SaveChanges();
        }

        [HttpPost]
        public JsonResult<ApiListHeader> PostListHeader(string id, string password, ApiListHeader listHeader)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return Json(listHeader);
            var list = Context.ResultLists.FirstOrDefault(l => l.Iid == listHeader.Iid && l.CompId == id);
            bool newList = list == null;

            if (newList)
            {
                list = Context.ResultLists.Add(new ListHeader(Context)
                {
                    CompId = id
                });
            }

            if (string.IsNullOrEmpty(listHeader.AgeGroupInCompId))
            {
                list.AgeGroup = null;
                list.GroupId = null;
            }
            else
            {
                list.AgeGroup = Context.AgeGroupsOnCompetition.FirstOrDefault(g => g.Iid == listHeader.AgeGroupInCompId);
                list.GroupId = listHeader.AgeGroupInCompId;
            }

            if (string.IsNullOrEmpty(listHeader.IidParent))
            {
                list.Parent = null;
                list.IidParent = null;
            }
            else
            {
                list.Parent = Context.ResultLists.FirstOrDefault(l => l.Iid == listHeader.IidParent);
                list.IidParent = listHeader.IidParent;
            }

            list.ListType = listHeader.ListTypeV;
            list.Live = listHeader.Live;

            if (string.IsNullOrEmpty(listHeader.PreviousRoundId))
            {
                list.PreviousRound = null;
                list.PrevRoundIid = null;
            }
            else
            {
                list.PreviousRound = Context.ResultLists.FirstOrDefault(r => r.Iid == listHeader.PreviousRoundId);
                list.PrevRoundIid = listHeader.PreviousRoundId;
            }

            list.Quota = listHeader.Quota;
            list.Round = listHeader.Round;
            list.RouteNumber = listHeader.RouteNumber;
            list.Rules = listHeader.ClimbingRules;
            list.Style = listHeader.Style;

            Context.SaveChanges();

            listHeader.Iid = list.Iid;
            return Json(listHeader);
        }

        [HttpPost]
        public void RemoveListsExceptFor(string id, string password, IEnumerable<string> listsToLeave)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return;

            var toLeave = new HashSet<string>(listsToLeave ?? new string[0]);
            

            foreach(var l in Context.ResultLists.Where(c=>c.CompId == id && !listsToLeave.Contains(c.Iid)).OrderByDescending(c=>c.Iid).ToArray())
            {
                l.RemoveObject(Context);
            }
            

            Context.SaveChanges();
        }

        [HttpPost]
        public void RemoveDeadResults(string id, string password, string listId, IEnumerable<string> resultsToLeave)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return;
            var toLeave = new HashSet<string>(resultsToLeave ?? new string[0]);
            foreach(var rtd in Context.AllResultLines.Where(r=>r.ListId == listId && !toLeave.Contains(r.Iid)).ToArray())
            {
                rtd.RemoveObject(Context);
            }
            
            Context.SaveChanges();
        }

        [HttpPost]
        public JsonResult<ApiListLineLead[]> PostLeadResults(string id, string password, IEnumerable<ApiListLineLead> data)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return Json(new ApiListLineLead[0]);

            var changedLists = new HashSet<string>();
            
            foreach(var l in data)
            {
                var res = Context.ResultsLead.FirstOrDefault(r => r.ListId == l.ListId && r.ClimberId == l.ClimberId);
                var isNew = res == null;
                if (isNew)
                {
                    res = Context.ResultsLead.Add(new ListLineLead(Context)
                    {
                        ListId = l.ListId,
                        ClimberId = l.ClimberId,
                        Climber = Context.ClimbersOnCompetition.FirstOrDefault(c => c.Iid == l.ClimberId)
                    });
                }
                
                res.ResText = l.ResText;
                res.Result = l.Result;
                res.Start = l.Start;
                res.TimeText = l.TimeText;
                res.TimeValue = l.TimeValue;

                l.Iid = res.Iid;
                Context.SaveChanges();

                if (!changedLists.Contains(l.ListId))
                    changedLists.Add(l.ListId);
            }

            this.SortChangedLists(changedLists);

            return Json(data.ToArray());
        }

        [HttpPost]
        public JsonResult<ApiListLineSpeed[]> PostSpeedResults(string id, string password, IEnumerable<ApiListLineSpeed> data)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return Json(new ApiListLineSpeed[0]);

            var changedLists = new HashSet<string>();

            foreach(var l in data)
            {
                var res = Context.ResultsSpeed.FirstOrDefault(r => r.ListId == l.ListId && r.ClimberId == l.ClimberId);
                if (res == null)
                    res = Context.ResultsSpeed.Add(new ListLineSpeed(Context)
                    {
                        ClimberId = l.ClimberId,
                        ListId = l.ListId,
                        Header = Context.ResultLists.FirstOrDefault(lh => lh.Iid == l.ListId),
                        Climber = Context.ClimbersOnCompetition.FirstOrDefault(c => c.Iid == l.ClimberId)
                    });
                switch (l.TotalResType)
                {
                    case ApiListLineSpeed.ResultType.Dns:
                        res.Result = ListLineSpeed.DNS;
                        break;
                    case ApiListLineSpeed.ResultType.Dsq:
                        res.Result = ListLineSpeed.DSQ;
                        break;
                    case ApiListLineSpeed.ResultType.Fall:
                        res.Result = ListLineSpeed.FALL;
                        break;
                    default:
                        res.Result = l.Result;
                        break;
                }
                res.ResText = l.ResText;
                res.Route1 = l.Route1Res;
                res.Route1Text = l.Route1Text;
                res.Route2 = l.Route2Res;
                res.Route2Text = l.Route2Text;
                res.Start = l.Start;

                if (!changedLists.Contains(res.ListId))
                    changedLists.Add(res.ListId);

                l.Iid = res.Iid;
                Context.SaveChanges();
            }

            this.SortChangedLists(changedLists);

            return Json(data.ToArray());
        }

        [HttpPost]
        public JsonResult<ApiListLineBoulder[]> PostBoulderResults(string id, string password, IEnumerable<ApiListLineBoulder> data)
        {
            if(!this.CheckCompetitionsPassword(id, password))
            {
                return Json(new ApiListLineBoulder[0]);
            }

            var changedLists = new HashSet<string>();

            foreach(var l in data)
            {
                if (!changedLists.Contains(l.ListId))
                    changedLists.Add(l.ListId);

                var res = this.Context.ResultsBoulder.FirstOrDefault(r => r.ListId == l.ListId && r.ClimberId == l.ClimberId);
                if(res == null)
                {
                    res = this.Context.ResultsBoulder.Add(new ListLineBoulder(this.Context)
                    {
                        ClimberId = l.ClimberId,
                        Climber = this.Context.ClimbersOnCompetition.FirstOrDefault(c => c.Iid == l.ClimberId),

                        ListId = l.ListId,
                        Header = this.Context.ResultLists.FirstOrDefault(h => h.Iid == l.ListId),

                        Start = l.Start,

                        Routes = new List<ListLineBoulderRoute>()
                    });
                }

                res.ResultType = l.ResultType;
                if (l.ResultType == ClimbingCompetition.Common.ResultType.RES)
                {
                    this.PostRoutes(res, l.Routes);
                }
                else if (res.Routes.Any())
                {
                    foreach (var r in res.Routes.ToArray())
                    {
                        r.RemoveObject(this.Context);
                    }

                    res.Routes.Clear();
                }

                this.Context.SaveChanges();

                l.Iid = res.Iid;
            }

            this.SortChangedLists(changedLists);

            return Json(data.ToArray());
        }

        private void PostRoutes(ListLineBoulder res, IEnumerable<ApiListLineBoulderRoute> routes)
        {
            var savedRoutes = new HashSet<string>();
            if(routes != null)
            {
                foreach(var route in routes)
                {
                    var existingRoute = res.Routes.FirstOrDefault(r => r.RouteNumber == route.RouteNumber);
                    if(existingRoute == null)
                    {
                        existingRoute = this.Context.ResultsBoulderRoutes.Add(new ListLineBoulderRoute(this.Context)
                        {
                            Result = res,
                            ResultIid = res.Iid,
                            RouteNumber = route.RouteNumber
                        });

                        res.Routes.Add(existingRoute);
                    }

                    existingRoute.TopAttempt = route.TopAttempt;
                    existingRoute.BonusAttempt = route.BonusAttempt;
                    
                    savedRoutes.Add(existingRoute.Iid);
                }
            }

            var deadRoutes = res.Routes.Where(r => !savedRoutes.Contains(r.Iid)).ToList();
            if (deadRoutes.Count > 0)
            {
                deadRoutes.ForEach(r =>
                {
                    res.Routes.Remove(r);
                    r.RemoveObject(this.Context);
                });
            }
        }
    }
}