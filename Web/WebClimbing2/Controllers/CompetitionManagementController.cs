using ClimbingEntities.Teams;
using DbAccessCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing2.Models.CompetitionManagement;

namespace WebClimbing2.Controllers
{
    [Authorize]
    public class CompetitionManagementController : __BaseController
    {
        private void GetAvailableOrganizers(string zone, string current)
        {
            var org = (string.IsNullOrEmpty(zone) ? this.Context.Teams.Where(t => t.ParentTeam == null) : this.Context.Teams.Where(t => t.IidParent == zone))
                     .OrderBy(t => t.Name)
                     .ToList()
                     .Select(t => new SelectListItem
                     {
                         Text = t.Name,
                         Value = t.Iid,
                         Selected = string.Equals(current, t.Iid, StringComparison.OrdinalIgnoreCase)
                     })
                     .ToList();
            ViewBag.Organizers = org;
            ViewBag.Zone = zone;
        }

        public ActionResult Edit(string id)
        {
            var comp = this.Context.Competitions.FirstOrDefault(c => c.Iid == id);
            if (comp == null)
                return this.HttpNotFound();
            var rigths = comp.GetNotNullableRigths(this.Context.CurrentUserID, RightsActionEnum.Edit, this.Context);
            if (rigths < RightsEnum.Allow)
                return this.HttpNotFound();
            this.GetAvailableOrganizers(comp.CompetitionZoneId, comp.OrganizerId);
            return View(new CompetitionViewModel(comp));
        }

        public ActionResult CreateNew(string region)
        {
            if (string.IsNullOrEmpty(region))
            {
                if (!this.Context.CurrentUserIsAdmin)
                    return this.HttpNotFound();
            }
            else
            {
                var regionForComp = this.Context.Teams.FirstOrDefault(t => t.Iid == region);
                if (regionForComp == null || regionForComp.GetNotNullableRigths(this.Context.CurrentUserID, RightsActionEnum.Edit, this.Context) < RightsEnum.Allow)
                    return this.HttpNotFound();
            }
            this.GetAvailableOrganizers(region, null);

            return View("Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(CompetitionViewModel model, string zone)
        {
            this.GetAvailableOrganizers(zone, model.OrganizerId);
            if (!ModelState.IsValid)
                return View("Edit");

            ClimbingEntities.Competitions.Competition comp;
            try { comp = model.Persist(Context, null); }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex);
                return View("Edit");
            }

            return this.RedirectToAction("Index", "Home", new { zone = comp.CompetitionZoneId, year = comp.CompetitionYear });
        }
        
        public PartialViewResult GeneratePassword(string elementName)
        {
            ViewBag.ElementName = elementName;
            ViewBag.Password = Crypto.PasswordHelper.GeneratePassword();
            return PartialView();
        }
    }
}