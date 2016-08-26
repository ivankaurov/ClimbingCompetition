using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.DataProcessors;
using WebClimbing.DataProcessors.Models;

namespace WebClimbing.Controllers
{
    [Authorize]
    public class RegionsController : Controller
    {
        private ClimbingContext db = new ClimbingContext();
        private RegionsProcessor proc = new RegionsProcessor();

        //
        // GET: /Regions/
        [AllowAnonymous]
        public ActionResult Index(long? id = null)
        {
            var parentList = db.Regions
                             .Where(r => r.IidParent != null)
                             .Select(r => r.Parent)
                             .Distinct()
                             .ToList();
            var user = User.GetDbUser(db, false);
            if (user != null)
                foreach (var reg in proc.GetAdminRegions(user))
                    if (parentList.Count(pR => pR != null && pR.Iid == reg.Iid) < 1)
                        parentList.Add(reg);
            parentList.Sort();
            ViewBag.Regions = new SelectList(parentList, "Iid", "Name", id);
            ViewBag.AllowGlobal = User.IsInRole(db, RoleEnum.Admin, null);
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegList(long? region, String divId)
        {
            ViewBag.AllowEdit = User.IsInRole(db, RoleEnum.Admin, region);
            ViewBag.DivID = divId;
            ViewBag.Selected = region == null ? null : db.Regions.Find(region);
            var regList = db.Regions
                            .Where(r => (r.IidParent ?? 0) == (region ?? 0))
                            .OrderBy(r => r.Name)
                            .ToList();
            return PartialView(regList);
        }

        //
        // GET: /Regions/Create
        public ActionResult Create(long? parent)
        {
            if(!User.IsInRole(db, RoleEnum.Admin, parent))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Regions", new { parent = parent }) });
            
            var regList = proc.GetAdministartorRegion(User.GetDbUser(db), parent);
            
            ViewBag.Regions = regList;
            var model = new RegionEditModel
            {
                IidParent = parent
            };
            model.LoadUsers(db);
            return View("Edit", model);
        }
        
        
        public ActionResult Edit(long id)
        {
            var model = db.Regions.Find(id);
            if (model == null)
                return HttpNotFound();
            if (!User.IsInRole(db, RoleEnum.Admin, model.IidParent))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Edit", "Regions", new { id = id }) });
            ViewBag.Regions = proc.GetAdministartorRegion(User.GetDbUser(db), model.IidParent);

            var editModel = new RegionEditModel(model);
            
            editModel.LoadUsers(db);
            return View(editModel);
        }

        [HttpPost]
        public ActionResult Save(RegionEditModel model)
        {
            ViewBag.Message = String.Empty;
            ViewBag.Regions = proc.GetAdministartorRegion(User.GetDbUser(db), model.IidParent);
            if (!ModelState.IsValid)
                return View("Edit", model);
            var errors = proc.SaveRegion(User.GetDbUser(db), model);
            if (errors.Length > 0)
            {
                foreach (var err in errors)
                    ModelState.AddModelError(err.Item1, err.Item2);
                return View("Edit", model);
            }
            return RedirectToAction("Index", new { id = model.IidParent });
        }

        //
        // GET: /Regions/Delete/5
        [HttpGet]
        public ActionResult Delete(long id = 0)
        {
            RegionModel regionmodel = db.Regions.Find(id);
            if (regionmodel == null || !User.IsInRole(db, RoleEnum.Admin, regionmodel.IidParent))
                return HttpNotFound();
            return View(new RegionEditModel(regionmodel));
        }

        //
        // POST: /Regions/Delete/5

        [HttpPost]
        public ActionResult Delete(RegionEditModel model)
        {
            String[] errors = proc.DeleteRegion(User.GetDbUser(db), model.Iid.Value);
            if (errors.Length > 0)
            {
                foreach (var s in errors)
                    ModelState.AddModelError(String.Empty, s);
                return View(model);
            }
            else
                return RedirectToAction("Index", model.IidParent);
        }

        [HttpGet]
        public ActionResult GlobalAdmins()
        {
            if (!User.IsInRole(db, RoleEnum.Admin, null))
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("GlobalAdmins", "Regions") });
            var res = db.UserProfiles
                        .Where(r => r.RegionId == null)
                        .Select(r => new RegionAdminModel
                        {
                            UserId = r.Iid,
                            UserEmail = r.Email ?? String.Empty,
                            UserName = r.Name,
                            UserRegion = String.Empty,
                            IsAdmin = r.Roles.Count(ur => ur.RegionID == null && ur.CompID == null && ur.RoleId >= (int)RoleEnum.Admin) > 0
                        })
                        .ToList();
            res.Sort((a, b) => a.UserName.CompareTo(b.UserName));
            ViewBag.Message = String.Empty;
            return View(res);
        }

        [HttpPost]
        public ActionResult GlobalAdmins(IEnumerable<RegionAdminModel> data)
        {
            var res = proc.SaveGlobalAdmins(User.GetDbUser(db), data);
            foreach (var s in res)
                ModelState.AddModelError(String.Empty, s);
            ViewBag.Message = ModelState.IsValid ? "Данные сохранены" : String.Empty;
            return View(data);
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