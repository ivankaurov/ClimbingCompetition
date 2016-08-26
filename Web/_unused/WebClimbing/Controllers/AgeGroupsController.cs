﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing.Models;
using WebClimbing.ServiceClasses;

namespace WebClimbing.Controllers
{
    [Authorize]
    public class AgeGroupsController : Controller
    {
        //
        // GET: /AgeGroups/

        private ClimbingContext db = new ClimbingContext();

        [HttpGet]
        public ActionResult Index(bool scs = false)
        {
            var model = db.AgeGroups.Select(g =>
                new AgeGroupModelWrapper()
                {
                    Value = g,
                    IsNew = false,
                    ReadOnly = (g.CompetitionGroups.Count > 0)
                }).ToList();
            model.Sort((a, b) => a.Value.CompareTo(b.Value));
            ViewBag.Scs = scs;
            return View(model);
        }

        public const string IndexPostArgName = "items";

        [HttpPost]
        public ActionResult Index(IEnumerable<AgeGroupModelWrapper> items)
        {
            ViewBag.Scs = false;
            if (items == null)
            {
                ModelState.AddModelError(String.Empty, "Нет данных");
                return View(new List<AgeGroupModelWrapper>());
            }
            if (!ModelState.IsValid)
                return View(items);

            List<AgeGroupModelWrapper> newModel = new List<AgeGroupModelWrapper>();
            foreach (var m in items.Select(mq => new AgeGroupModelWrapper(mq.Value, mq.IsNew, mq.ReadOnly)
            {
                ToDelete = mq.ToDelete
            }))
            {
                if (m.ToDelete)
                {
                    var delModel = db.AgeGroups.Find(m.Value.Iid);
                    if (delModel != null)
                    {
                        if (delModel.IsReadOnly)
                        {
                            ModelState.AddModelError(String.Empty, String.Format("Нельзя удалить группу {0}", m.Value.FullName));
                            return View(items);
                        }
                    }
                    newModel.Add(m);
                }
                else if (m.IsNew)
                    newModel.Add(m);
                else if (m.Value.Iid < 1)
                {
                    m.IsNew = true;
                    newModel.Add(m);
                }
                else
                {
                    var oldModel = db.AgeGroups.Find(m.Value.Iid);
                    if (oldModel == null)
                    {
                        m.IsNew = true;
                        newModel.Add(m);
                    }
                    bool modelReadOnly = oldModel.IsReadOnly;
                    bool change = false;
                    if (!m.Value.FullName.Equals(oldModel.FullName, StringComparison.Ordinal))
                    {
                        m.FullNameChange = String.Format("{0} => {1}", oldModel.FullName, m.Value.FullName);
                        change = true;
                    }

                    if (!m.Value.SecretaryName.Equals(oldModel.SecretaryName, StringComparison.Ordinal))
                    {
                        m.SecretaryNameChange = String.Format("{0} => {1}", oldModel.SecretaryName, m.Value.SecretaryName);
                        change = true;
                    }

                    if (!modelReadOnly && !m.Value.GenderProperty.Equals(oldModel.GenderProperty))
                    {
                        m.GenderChange = String.Format("{0} => {1}", oldModel.GenderProperty.GetFriendlyValue(), m.Value.GenderProperty.GetFriendlyValue());
                        change = true;
                    }

                    if (!modelReadOnly && m.Value.MaxAge != oldModel.MaxAge)
                    {
                        m.MaxAgeChange = String.Format("{0} => {1}", oldModel.MaxAge.GetStringValue(), m.Value.MaxAge.GetStringValue());
                        change = true;
                    }

                    if (!modelReadOnly && m.Value.MinAge != oldModel.MinAge)
                    {
                        m.MinAgeChange = String.Format("{0} => {1}", oldModel.MinAge.GetStringValue(), m.Value.MinAge.GetStringValue());
                        change = true;
                    }
                    if (change)
                        newModel.Add(m);
                }
            }
            if (newModel.Count < 1)
            {
                ModelState.AddModelError(String.Empty, "Изменений нет");
                return View(items);
            }
            else
            {
                newModel.ForEach(a => a.Confirmed = true);
                return View("Confirm", newModel);
            }
        }

        public ActionResult GroupEdit(int prefix, AgeGroupModelWrapper model = null)
        {
            if (model == null)
                model = new AgeGroupModelWrapper();
            if (model.Value == null)
            {
                model.Value = new AgeGroupModel();
                model.IsNew = true;
                model.ReadOnly = false;
            }
            ViewBag.Prefix = prefix;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Confirm(IEnumerable<AgeGroupModelWrapper> listModel)
        {
            if (Request.Form.AllKeys.Contains("Back"))
            {
                ViewBag.Scs = false;
                return View("Index", listModel);
            }
            try
            {
                foreach (var m in listModel.Where(q => q.Confirmed))
                {
                    if (m.IsNew)
                        db.AgeGroups.Add(m.Value);
                    else if (m.ToDelete)
                    {
                        var delModel = db.AgeGroups.Find(m.Value.Iid);
                        if (delModel != null && !delModel.IsReadOnly)
                            db.AgeGroups.Remove(delModel);
                    }
                    else
                    {
                        var oldModel = db.AgeGroups.Find(m.Value.Iid);
                        if (oldModel != null)
                        {
                            oldModel.SecretaryName = m.Value.SecretaryName;
                            oldModel.FullName = m.Value.FullName;
                            if (!oldModel.IsReadOnly)
                            {
                                oldModel.GenderProperty = m.Value.GenderProperty;
                                oldModel.MaxAge = m.Value.MaxAge;
                                oldModel.MinAge = m.Value.MinAge;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, String.Format("Группа {0} (compId={1}) не найдена", m.Value.FullName, m.Value.Iid));
                            return View(listModel);
                        }
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex);
                return View(listModel);
            }
            return RedirectToAction("Index", new { scs = true });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
