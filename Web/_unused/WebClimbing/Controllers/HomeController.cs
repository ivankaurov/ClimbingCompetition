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

﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebClimbing.Models;
using System.Data;
using WebClimbing.Models.UserAuthentication;
using System.Net.Mime;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WebClimbing.DataProcessors;
using WebClimbing.DataProcessors.Models;
using WebClimbing.ServiceClasses;

namespace WebClimbing.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ClimbingContext db = new ClimbingContext();
        private CompetitionProcessor proc = new CompetitionProcessor();

        [AllowAnonymous]
        public ActionResult Index(int? comp_year = null, long? comp_region = null)
        {
            ViewBag.Years = new SelectList(proc.GetCompetitionYears(), comp_year ?? DateTime.UtcNow.Year);
            var regList = proc.GetCompetitionRegions();
            var user = User.GetDbUser(db, false);
            if (user != null)
            {
                foreach (var reg in (new RegionsProcessor()).GetAdminRegions(user))
                    if (regList.Count(r => r != null && r.Iid == reg.Iid) < 1)
                        regList.Add(reg);
                regList.Sort();
            }
            if (user!= null && comp_region == null && user.RegionId != null)
                comp_region = user.Region.IidParent;
            ViewBag.Regions = new SelectList(regList, "Iid", "Name", comp_region);
            ViewBag.Region = comp_region;
            ViewBag.Year = comp_year ?? DateTime.UtcNow.Year;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Calendar(int comp_year, long? comp_region, String divId)
        {
            var compList = db.Competitions.Where(c => c.Start.Year == comp_year);
            if ((comp_region ?? 0) == 0)
                compList = compList.Where(c => c.Region.IidParent == null);
            else
                compList = compList.Where(c => c.Region.IidParent == comp_region);
            compList = compList.OrderBy(c => c.Start);
            ViewBag.DivID = divId;
            ViewBag.Db = db;
            ViewBag.Year = comp_year;
            ViewBag.Region = comp_region;
            return PartialView(compList.ToArray());
        }

        public ActionResult Create()
        {
            ViewBag.Message = String.Empty;
            var regList = proc.GetRegionsToCreateCompetition(User.GetDbUser(db));
            if (regList.Count < 1)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Home") });
            ViewBag.Regions = new SelectList(regList, "Iid", "Name");
            return View("Edit", new CompetitionEditModel(db));
        }

        public ActionResult Edit(long id, bool scs = false)
        {
            ViewBag.Message = scs ? "Данные сохранены" : String.Empty;
            CompetitionModel competitionmodel = db.Competitions.Find(id);
            if (competitionmodel == null)
                return HttpNotFound();
            var regList = proc.GetRegionsToCreateCompetition(User.GetDbUser(db));
            if (regList.Count(r => r.Iid == competitionmodel.RegionId) < 1)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Edit", "Home", new { id = id }) });
            ViewBag.Regions = new SelectList(regList, "Iid", "Name", competitionmodel.RegionId);
            
            return View(new CompetitionEditModel(competitionmodel, db));
        }

        [HttpPost]
        public ActionResult SaveData(CompetitionEditModel model)
        {
            var dbUser = User.GetDbUser(db);
            ViewBag.Regions = new SelectList(proc.GetRegionsToCreateCompetition(dbUser), "Iid", "Name", model.RegionId);
            if (model.Iid > 0)
            {
                var oldComp = db.Competitions.Find(model.Iid);
                if (oldComp == null || !dbUser.IsInRoleRegion(RoleEnum.Admin, oldComp.Region.IidParent))
                    ModelState.AddModelError(String.Empty, "У вас нет прав на редактирование этих соревнований");
            }
            if (!ModelState.IsValid)
                return View("Edit", model);
            long newIid;
            var saveResults = proc.SaveCompetition(model, out newIid);
            if (saveResults.Length > 0)
            {
                foreach (var s in saveResults)
                    ModelState.AddModelError(String.Empty, s);
                return View("Edit", model);
            }
            else
            {
                ViewBag.Message = "Данные сохранены";
                return RedirectToAction("Edit", new { id = newIid, scs = true });
            }
        }

        [HttpGet]
        public ActionResult Delete(long id = 0)
        {
            CompetitionModel competitionmodel = db.Competitions.Find(id);
            if (competitionmodel == null)
                return HttpNotFound();
            if (!User.IsInRole(db, RoleEnum.Admin, competitionmodel.Region.IidParent))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Delete", "Home", new { id = id }) });
            var cModel = new CompetitionDeleteModel(competitionmodel);
            foreach (var err in cModel.Validate(null))
                ModelState.AddModelError(String.Empty, err.ErrorMessage);
            return View(new CompetitionDeleteModel(competitionmodel));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(CompetitionDeleteModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var comp = db.Competitions.Find(model.Iid);
            if (comp == null)
                return HttpNotFound();
            if (!User.IsInRole(db, RoleEnum.Admin, comp.Region.IidParent))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Delete", "Home", new { id = model.Iid }) });
            int year = comp.Start.Year;
            var region = comp.Region.IidParent;
            db.Competitions.Remove(comp);
            db.SaveChanges();
            return RedirectToAction("Index", new { comp_year = year, comp_region = region });
        }
        /*
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your itemsConfimed description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //
        // GET: /Home/Details/5
        [AllowAnonymous]
        public ActionResult Details(long id = 0)
        {
            CompetitionModel competitionmodel = db.Competitions.Find(id);
            if (competitionmodel == null)
            {
                return HttpNotFound();
            }
            return View(competitionmodel);
        }
        */
        
        [HttpGet]
        public ActionResult PrivateKey(long id)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Forbidden);
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            byte[] key = comp[CompetitionParamType.SignatureKey].BinaryValue;
            ViewBag.HasKey = (key != null && key.Length > 0);
            db.SaveChanges();
            return View(db.Competitions.Find(id));
        }

        [HttpGet]
        public FileResult GetNewKey(long id)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Forbidden);
            var comp = db.Competitions.Find(id);
            if (comp == null)
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.NotFound);
            byte[] privateKey = comp.GenerateKeyPair();
            db.SaveChanges();
            return this.File(privateKey, MediaTypeNames.Application.Octet, String.Format("COMP_{0}.key", comp.Iid));
        }

        public class ClimberListViewModel
        {
            public SelectList RegionsList { get; set; }
            public SelectList GroupsList { get; set; }
            public CompetitionModel Comp { get; set; }
        }
        [AllowAnonymous]
        public ActionResult Climbers(long id)
        {
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            ClimberListViewModel model = new ClimberListViewModel();
            model.Comp = comp;
            var regionsLst = comp.Climbers
                                .SelectMany(clm => clm.Teams)
                                .Select(clt => clt.Region)
                                .Distinct()
                                .ToList();
            regionsLst.Sort();
            model.RegionsList = new SelectList(regionsLst, "Iid", "Name");

            var groupLst = comp.AgeGroups
                               .Select(g => g.AgeGroup)
                               .Distinct()
                               .ToList();
            groupLst.Sort();
            model.GroupsList = new SelectList(groupLst, "Iid", "FullName");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ClimbersList(long id, String divId, long? regionId = null, long? groupId = null)
        {
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            ViewBag.Comp = comp;
            IEnumerable<Comp_CompetitiorRegistrationModel> climbersList = comp.Climbers;
            if (groupId != null)
                climbersList = climbersList.Where(c => c.CompAgeGroup.AgeGroupId == groupId);
            if (regionId != null)
                climbersList = climbersList.Where(c => c.Teams.Count(t => t.RegionId == regionId) > 0);
            var lst = climbersList.ToList();
            lst.Sort((a, b) =>
            {
                int n = a.CompAgeGroup.AgeGroup.CompareTo(b.CompAgeGroup.AgeGroup);
                if (n != 0)
                    return n;
                n = a.TeamList.CompareTo(b.TeamList);
                if (n != 0)
                    return n;
                return a.Person.CompareTo(b.Person);
            });
            ViewBag.DivId = divId;
            return PartialView(lst);
        }

        [HttpGet]
        public ActionResult Roles(long id)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Roles", "Home", new { id = id }) });
            
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return HttpNotFound();
            ViewBag.Comp = comp;
            ViewBag.Message = String.Empty;
            return View(proc.GetCompetitonRoles(id));
        }

        [HttpPost]
        public ActionResult Roles(long id, IEnumerable<RoleSelectorModel> items)
        {
            if(!User.IsInRoleComp(db,RoleEnum.Admin, id))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Roles", "Home", new { id = id }) });
            var comp = db.Competitions.Find(id);
            ViewBag.Comp = comp;
            if (!ModelState.IsValid)
            {
                ViewBag.Message = String.Empty;
                return View(items);
            }
            foreach (var s in proc.SaveRoles(id, items))
                ModelState.AddModelError(String.Empty, s);
            if (ModelState.IsValid)
                ViewBag.Message = "Данные сохранены";
            return View(items);
        }

        [HttpGet]
        public ActionResult SendEmail(long id)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("SendEmail", "Home", new { id = id }) });
            var comp = db.Competitions.Find(id);
            ViewBag.Comp = comp;
            return View();
        }
        
        [HttpGet]
        public ActionResult MailList(long id, String divId)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            ViewBag.DivId = divId;
            return PartialView(proc.GetCompUsersForEmail(db.Competitions.Find(id)));
        }

        [HttpPost]
        public ActionResult MailList(long id, String divId, String subject, String body, IEnumerable<UserSendEmail> items)
        {
            if (!User.IsInRoleComp(db, RoleEnum.Admin, id))
                ModelState.AddModelError(String.Empty, "У вас нет прав для рассылки сообщений");
            if (items == null || items.Count(i => i.Confirmed) < 1)
                ModelState.AddModelError(String.Empty, "Пользователи не выбраны");
            if (String.IsNullOrEmpty(subject))
                ModelState.AddModelError(String.Empty, "Тема не указана");
            if (String.IsNullOrEmpty(body))
                ModelState.AddModelError(String.Empty, "Тело сообщения не введено");
            ViewBag.DivId = divId;
            ViewData["subject"] = subject;
            ViewData["body"] = body;
            int messageSent = 0;
            if (ModelState.IsValid)
                foreach (var s in proc.SendEmail(id, items.Where(u => u.Confirmed), subject, body, out messageSent))
                    ModelState.AddModelError(String.Empty, s);
            ViewBag.MessageSent = new int?(messageSent);
            if (ModelState.IsValid)
            {
                ViewData["subject"] = String.Empty;
                ViewData["body"] = String.Empty;
            }
            return PartialView(items ?? new UserSendEmail[0]);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                proc.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
