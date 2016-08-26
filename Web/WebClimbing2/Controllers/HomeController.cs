using ClimbingEntities.Competitions;
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