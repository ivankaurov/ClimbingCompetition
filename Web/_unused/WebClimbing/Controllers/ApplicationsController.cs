// <copyright file="ApplicationsController.cs">
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
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.ServiceClasses;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using WebClimbing.DataProcessors.Models;
using WebClimbing.DataProcessors;
using System.Threading.Tasks;

namespace WebClimbing.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {

        public const string CLS_HIDE_WHEN_EDIT = "hide-when-edit";
        ClimbingContext db = new ClimbingContext();
        ApplicationsProcessor proc = new ApplicationsProcessor();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                proc.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index(long id)
        {
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            var regList = proc.GetRegionsForApps(User.GetDbUser(db, false), comp.Iid);
            if (regList.Count < 1)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Applications", new { id = id }) });
            ViewBag.RegionName = regList[0].Name;
            ViewBag.Regions = new SelectList(regList, "Iid", "Name", regList.Count > 1 ? null : (object)regList[0].Iid);
            ViewBag.Groups = new SelectList(comp.AgeGroupsList(), "Iid", "SecretaryName", null);
            ViewBag.Comp = comp;
            return View();
        }

        [HttpGet]
        public ActionResult Applications(long id, int? groupId, long? regionId, String divId = null)
        {
            var user = User.GetDbUser(db, false);
            var comp = db.Competitions.Find(id);
            ViewBag.DivId = divId;
            ViewBag.Comp = comp;
            ViewBag.Region = db.Regions.Find(regionId);

            ViewBag.AllowEdit = comp.AllowedToEdit(user);
            ViewBag.AllowAdd = comp.AllowedToAdd(user);
            ViewBag.GroupId = groupId;
            if (regionId == null)
                return PartialView();
            else
                return PartialView(proc.GetAppsForUser(User.GetDbUser(db, false), id, groupId, regionId.Value));
        }
        
        private static Random random = new Random();

        public ActionResult Climber(long compId, String divId, long regionId, ClimberApplication model = null, long? climberId = null)
        {
            var comp = db.Competitions.Find(compId);
            ViewBag.Comp = comp;
            ViewBag.DivId = divId;
            ViewBag.RegionId = regionId;

            if (climberId != null)
            {
                var app = db.CompetitionClimberTeams.Find(climberId.Value);
                if (app.Climber.CompId != compId || app.RegionId != regionId || !app.AllowedEdit(User.GetDbUser(db, false)))
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
                model = new ClimberApplication(app);
            }
            ViewBag.DisplayValidation = false;
            ViewBag.AllowEditName = comp.AllowedToAdd(User.GetDbUser(db, false));
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeleteClimber(long climberId)
        {
            var hasLock = false;
            try
            {
                Monitor.Enter(APPLICATIONS_LOCKER, ref hasLock);
                String error = proc.DeleteApp(User.GetDbUser(db, false), climberId);
                return Json(new { Error = error, Success = String.IsNullOrEmpty(error) });
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(APPLICATIONS_LOCKER);
            }
        }

        [HttpPost]
        public ActionResult SaveClimber(long compId, long regionId, String divId, ClimberApplication model)
        {
            var comp = db.Competitions.Find(compId);
            ViewBag.Comp = comp;
            ViewBag.DivId = divId;
            ViewBag.RegionId = regionId;
            ViewBag.DisplayValidation = true;
            ViewBag.AllowEditName = comp.AllowedToAdd(User.GetDbUser(db, false));
            if (!ModelState.IsValid)
                return PartialView("Climber", model);
            foreach (var s in model.Validate(User.GetDbUser(db, false), db, ViewBag.Comp))
                ModelState.AddModelError(String.Empty, s);
            if (!ModelState.IsValid)
                return PartialView("Climber", model);
            return PartialView("ConfirmClimber", model);
        }

        public static readonly object APPLICATIONS_LOCKER = new object();

        [HttpPost]
        public ActionResult ConfirmClimber(long compId, long regionId, String divId, ClimberApplication model)
        {
            ViewBag.DisplayValidation = true;
            bool hasLock = false;
            try
            {
                Monitor.Enter(APPLICATIONS_LOCKER, ref hasLock);
                var comp = db.Competitions.Find(compId);
                var user = User.GetDbUser(db, false);
                ViewBag.Comp = comp;
                ViewBag.AllowEditName = comp.AllowedToAdd(user);
                ViewBag.DivId = divId;
                ViewBag.RegionId = regionId;
                if (!ModelState.IsValid)
                    return PartialView("Climber", model);
                foreach (var s in proc.SaveClimberApp(user, compId, model))
                    ModelState.AddModelError(String.Empty, s);
                if (!ModelState.IsValid)
                    return PartialView("Climber", model);
                return PartialView(model);
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(APPLICATIONS_LOCKER);
            }
        }


        [ChildActionOnly]
        public ActionResult EditClimberFromModel(uint index, long compId, ClimberApplication model)
        {
            CompetitionModel comp;
            ViewBag.Index = index;
            var user = User.GetDbUser(db, false);
            if (model.ApplicationId != null)
            {
                var app = db.CompetitionClimberTeams.Find(model.ApplicationId);
                if (app == null)
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
                if (!app.AllowedEdit(user))
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
                comp = app.Climber.Competition;
                model.Surname = app.Climber.Person.Surname;
                model.Name = app.Climber.Person.Name;
                model.YearOfBirth = app.Climber.Person.DateOfBirth.Year;
                model.GenderP = app.Climber.Person.GenderProperty;
                ViewBag.AllowEditName = comp.AllowedToAdd(user);
            }
            else
            {
                comp = db.Competitions.Find(compId);
                if (comp == null)
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
                if (!comp.AllowedToAdd(user))
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
                ViewBag.AllowEditName = true;
            }
            ViewBag.Comp = comp;
            return PartialView("EditClimber", model);
        }

        public ActionResult EditClimber(uint index, long? climberId = null, long? compId = null)
        {
            ClimberApplication model;
            CompetitionModel comp = null;
            var user = User.GetDbUser(db, false);
            if (climberId != null)
            {
                var app = db.CompetitionClimberTeams.Find(climberId);
                if (app == null)
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
                if (!app.AllowedEdit(user))
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
                model = new ClimberApplication(app);
                comp = app.Climber.Competition;
                ViewBag.AllowEditName = comp.AllowedToAdd(user);
            }
            else
            {
                if (compId != null)
                    comp = db.Competitions.Find(compId);
                if (comp == null)
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
                if (!comp.AllowedToAdd(user))
                    throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
                ViewBag.AllowEditName = true;
                model = new ClimberApplication();
            }
            ViewBag.Comp = comp;
            ViewBag.Index = index;
            return PartialView(model);
        }

        public ActionResult NewClimbers(long compId, long regionId)
        {
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return HttpNotFound();
            if (!comp.AllowedToAdd(User.GetDbUser(db, false)))
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            ViewBag.CompId = compId;
            ViewBag.RegionId = regionId;
            return PartialView();
        }

        [HttpPost]
        public ActionResult SaveNewClimbers(long compId, long regionId, String divId, IEnumerable<ClimberApplication> items)
        {
            var user = User.GetDbUser(db, false);
            ViewBag.CompId = compId;
            ViewBag.RegionId = regionId;
            ViewBag.DivId = divId;
            var comp = db.Competitions.Find(compId);
            if (comp == null || comp != null && !comp.AllowedToAdd(user))
                ModelState.AddModelError(String.Empty, "У вас нет прав для заявки на данные соревнования");
            if (items == null || items != null && items.Count() < 1)
                ModelState.AddModelError(String.Empty, "Участники не введены");
            if (items != null && comp != null && user != null)
                foreach (var app in items)
                    foreach (var s in app.Validate(user, db, comp))
                        ModelState.AddModelError(String.Empty, s);
            var viewName = ModelState.IsValid ? "ConfirmNewClimbers" : "NewClimbers";
            if (ModelState.IsValid)
                Parallel.ForEach(items, i => i.Confirmed = true);
            return PartialView(viewName, items);
        }

        [HttpPost]
        public ActionResult ConfirmNewClimbers(long compId, long regionId, IEnumerable<ClimberApplication> items)
        {
            var user = User.GetDbUser(db, false);
            var errors = (from mv in ModelState.Values
                         from ve in mv.Errors
                         select ve.ErrorMessage).ToList();
            if (errors.Count < 1)
                foreach (var err in proc.SaveClimberApps(user, compId, items))
                    errors.Add(err);
            return Json(new { Status = errors.Count, Errors = errors.ToArray() });
        }
    }
}
