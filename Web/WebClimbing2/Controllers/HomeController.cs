// <copyright file="HomeController.cs">
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

﻿using ClimbingEntities.Competitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebClimbing2.Controllers
{
    [AllowAnonymous]
    public class HomeController : __BaseController
    {
        void PrepareViewBagCalendar(int year, String zone)
        {
            ViewBag.Years = Context.Competitions.Select(c => c.StartDate.Year)
                                                .Distinct()
                                                .OrderBy(n => n)
                                                .Select(n => new SelectListItem
                                                {
                                                    Text = n.ToString(),
                                                    Value = n.ToString(),
                                                    Selected = (n == year)
                                                })
                                                .ToArray();
            var teamList = Context.Teams
                                     .Select(t => t.ParentTeam)
                                     .Distinct()
                                     .ToArray()
                                     .Select(t => new Tuple<String, String>(t == null ? String.Empty : t.Iid, t == null ? "Россия" : t.Name))
                                     .OrderBy(t => t.Item1)
                                     .Select(t => new SelectListItem { Text = t.Item2, Value = t.Item1 })
                                     .ToList();
            teamList.ForEach(t => t.Selected = (t.Value == (zone ?? string.Empty)));
            ViewBag.Teams = teamList;

            bool canCreate = false;
            string[] editableComp = null;
            if (Context.CurrentUser != null)
            {
                if (string.IsNullOrEmpty(zone))
                    canCreate = Context.CurrentUserIsAdmin;
                else
                {
                    var t = Context.Teams.FirstOrDefault(tcc => tcc.Iid == zone);
                    if (t != null)
                        canCreate = t.GetNotNullableRigths(Context.CurrentUserID, DbAccessCore.RightsActionEnum.Edit, Context) >= DbAccessCore.RightsEnum.Allow;
                }

                if (!canCreate)
                    editableComp = (string.IsNullOrEmpty(zone)
                        ? Context.Competitions.Where(c => c.Organizer.ParentTeam == null)
                        : Context.Competitions.Where(c => c.Organizer.IidParent == zone))
                        .Where(c => c.StartDate.Year == year)
                        .ToArray()
                        .Where(c => c.GetNotNullableRigths(Context.CurrentUserID, DbAccessCore.RightsActionEnum.Edit, Context) >= DbAccessCore.RightsEnum.Allow)
                        .Select(c => c.Iid)
                        .ToArray();
            }

            ViewBag.CanCreate = canCreate;
            ViewBag.EditableComp = editableComp ?? new string[0];
            ViewBag.Zone = zone ?? string.Empty;
        }

        public ActionResult Index(int? year = null, String zone = null)
        {
            if (!year.HasValue || year.Value <= 0)
                year = DateTime.Now.Year;
            IEnumerable<Competition> model = Context.Competitions.Where(c => c.StartDate.Year == year).ToArray();
            if (String.IsNullOrEmpty(zone))
                model = model.Where(c => c.Organizer.ParentTeam == null);
            else
                model = model.Where(c => c.Organizer.IidParent == zone);
            model = model.OrderBy(m => m.StartDate)
                         .ToArray();
            PrepareViewBagCalendar(year.Value, zone);
            return View(model);                                
        }

        void PrepareViewBagList(String id, String groupId, String teamId)
        {
            ViewBag.CompId = id;

            ViewBag.Groups = Context.AgeGroupsOnCompetition.Where(g => g.CompId == id)
                                    .ToArray()
                                    .OrderBy(g => string.Format("{0:0000}_{1}", g.AgeGroup.AgeOld, g.AgeGroup.GenderC))
                                    .Select(g => new SelectListItem
                                    {
                                        Text = g.AgeGroup.ShortName,
                                        Value = g.Iid,
                                        Selected = g.Iid == groupId
                                    });

            ViewBag.Teams = Context.ClimbersOnCompetition
                                   .Where(c => c.CompId == id)
                                   .SelectMany(c => c.Teams)
                                   .Select(t => t.TeamLicense.Team)
                                   .Distinct()
                                   .OrderBy(t => t.Name)
                                   .ToArray()
                                   .Select(t => new SelectListItem
                                   {
                                       Text = t.Name,
                                       Value = t.Iid,
                                       Selected = t.Iid == teamId
                                   });
        }
        public ActionResult ParticipantList(String id, String groupId = null, String teamId = null)
        {
            var participants = Context.ClimbersOnCompetition.Where(c => c.CompId == id);
            if (!string.IsNullOrEmpty(groupId))
                participants = participants.Where(p => p.AgeGroupId == groupId);
            if (!string.IsNullOrEmpty(teamId))
                participants = participants.Where(p => p.Teams.Count(t => t.TeamLicense.TeamId == teamId) > 0);
            var model = participants.ToArray()
                                    .OrderBy(p => string.Format("{0} {1:0000} {2} {3} {4}", p.Team, p.AgeGroup.AgeGroup.AgeOld, p.Person.Gender, p.Person.Surname, p.Person.Name));
            PrepareViewBagList(id, groupId ?? string.Empty, teamId ?? string.Empty);
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}