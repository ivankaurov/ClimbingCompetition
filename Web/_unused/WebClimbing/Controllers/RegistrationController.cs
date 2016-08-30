// <copyright file="RegistrationController.cs">
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
using System.Web;
using System.Web.Mvc;
using WebClimbing.DataProcessors;
using WebClimbing.Models;
using WebClimbing.DataProcessors.Models;

namespace WebClimbing.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        ClimbingContext db = new ClimbingContext();
        ApplicationsProcessor proc = new ApplicationsProcessor();

        //
        // GET: /Registration/
        public ActionResult Index(long id, bool newClimbersAdded = false, long? selectedRegion = null)
        {
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            var regList = proc.GetRegionsForApps(User.GetDbUser(db), id);
            if (regList.Count < 1)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Registration", new { id = id }) });
            var groupList = comp.AgeGroups.Select(grp => grp.AgeGroup).Distinct().ToList();
            groupList.Sort();
            ViewBag.Groups = groupList;
            ViewBag.Comp = comp;
            ViewBag.NewClimbersAdded = newClimbersAdded;
            ViewBag.SelectedRegionId = selectedRegion;
            return View(regList);
        }

        public ActionResult ClimbersList(long compId, long? groupId = null, long? regionId = null)
        {
            var regList = proc.GetRegionsForApps(User.GetDbUser(db), compId);
            if (regList.Count < 1)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Registration", new { id = compId }) });
            if (regionId != null && regList.Count(r => r.Iid == regionId) > 0)
                regList.RemoveAll(r => r.Iid != regionId.Value);

            var comp = db.Competitions.Find(compId);

            var sortList = from clmt in db.CompetitionClimberTeams
                           join clm in db.CompetitionClimbers on clmt.ClimberRegId equals clm.Iid
                           join cg in db.CompetitionAgeGroups on clm.GroupId equals cg.Iid
                           join ag in db.AgeGroups on cg.AgeGroupId equals ag.Iid
                           join ppl in db.People on clm.PersonId equals ppl.Iid
                           join preg in db.Regions on clmt.RegionId equals preg.Iid
                           where clm.CompId == compId
                           select new
                           {
                               Iid = clmt.Iid,
                               Value = clmt,
                               DisplayName = ppl.Surname + " " + ppl.Name,
                               AgeGroup = ag,
                               AgeGroupId=ag.Iid,
                               TeamId = preg.Iid,
                               TeamName = preg.Name
                           };
            if (groupId != null)
                sortList = sortList.Where(n => n.AgeGroupId == groupId);
            if (regionId != null)
                sortList = sortList.Where(n => n.TeamId == regionId);
            var newLst = sortList.ToList();

            //var sortList = compClimbers.Select(clm => new { Value = clm, Group = clm.CompAgeGroup.AgeGroup, DisplayName = clm.Person.Surname + "_" + clm.Person.Name }).ToList();
            newLst.Sort((a, b) =>
            {
                if (a.Iid==b.Iid)
                    return 0;
                if (a.AgeGroupId != b.AgeGroupId)
                    return (a.AgeGroup.CompareTo(b.AgeGroup));
                if (a.TeamId != b.TeamId)
                    return a.TeamName.CompareTo(b.TeamName);
                return a.DisplayName.CompareTo(b.DisplayName);
            });
            var iList = newLst.Select(al => al.Value).ToList();
            ViewBag.Comp = comp;
            return PartialView(iList);
        }


        private bool PrepareNewClimberView(long compId, long? regionId)
        {
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return false;
            var regList = proc.GetRegionsForApps(User.GetDbUser(db), compId);
            if (regList.Count < 1)
                return false;
            RegionModel defaultRegion;
            if (regList.Count == 1)
                defaultRegion = regList[0];
            else if (regionId != null)
                defaultRegion = (regList.FirstOrDefault(r => r.Iid == regionId));
            else
                defaultRegion = null;
            ViewBag.DefaultRegion = defaultRegion;
            ViewBag.Comp = comp;
            return true;
        }

        [HttpGet]
        public ActionResult NewClimber(long id, long? regionId = null)
        {
            if (!PrepareNewClimberView(id, regionId))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("NewClimber", "Registration", new { id = id }) });
            return View();
        }

        [HttpPost]
        public ActionResult NewClimber(long compId, long? regionId, IEnumerable<ClimberApplication> data)
        {
            if (!PrepareNewClimberView(compId, regionId))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("NewClimber", "Registration", new { id = compId }) });
            if (data == null || data.Count() < 1)
                ModelState.AddModelError(String.Empty, "Участники не введены");
            if (!ModelState.IsValid)
                return View(data);
            var usr = User.GetDbUser(db);
            var comp = db.Competitions.Find(compId);
            foreach (var error in data.SelectMany(d => d.Validate(usr, db, comp)))
                ModelState.AddModelError(String.Empty, error);
            if (!ModelState.IsValid)
                return View(data);
            var dataList = data.ToList();
            dataList.ForEach(a => a.Confirmed = true);
            return View("ConfirmClimbers", dataList);
        }

        [HttpPost]
        public ActionResult ConfirmClimbers(long compId, long? regionId, bool goback, IEnumerable<ClimberApplication> data)
        {
            if(!PrepareNewClimberView(compId, regionId))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("NewClimber", "Registration", new { id = compId }) });
            if (goback)
                return View("NewClimber", data);
            if (data != null)
                foreach (var d in data)
                    if (d.Team == null && d.TeamId != null)
                        d.Team = db.Regions.Find(d.TeamId);
            if (!ModelState.IsValid)
                return View(data);
            foreach (var err in proc.SaveClimberApps(User.GetDbUser(db), compId, data))
                ModelState.AddModelError(String.Empty, err);
            if (!ModelState.IsValid)
                return View(data);
            return RedirectToAction("Index", new { id = compId, newClimbersAdded = true, selectedRegion = regionId });
        }

        private void PrepareEditorForm(long compId, long? regionId)
        {
            ViewBag.Comp = db.Competitions.Find(compId);
            var regionList = proc.GetRegionsForApps(User.GetDbUser(db), compId);
            RegionModel defaultRegion;
            if (regionList.Count == 1)
                defaultRegion = regionList[0];
            else if (regionId != null)
                defaultRegion = regionList.FirstOrDefault(r => r.Iid == regionId);
            else
                defaultRegion = null;
            long? defaultRegionId = defaultRegion == null ? null : new long?(defaultRegion.Iid);
            ViewBag.DefaultRegionId = defaultRegionId;
            ViewBag.RegionList = regionList;
        }

        public ActionResult EditorForm(long compId, int index, long? selectedRegion = null, ClimberApplication model = null, long? appId = null)
        {
            ViewBag.Index = index;
            PrepareEditorForm(compId, model == null ? selectedRegion : (model.TeamId ?? selectedRegion));
            return PartialView(model ?? new ClimberApplication());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                proc.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
