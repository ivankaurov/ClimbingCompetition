using ClimbingCompetition.Common;
using ClimbingEntities.Competitions;
using ClimbingEntities.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Extensions;

namespace WebClimbing2.Controllers
{
    [AllowAnonymous]
    public class ResultsController : __BaseController
    {
        void PrepareViewBag(ListHeader list)
        {
            var styles = list.Competition
                             .ResultLists
                             .Where(r => r.AgeGroup != null)
                             .Select(r => r.StyleStr)
                             .Distinct()
                             .ToList()
                             .Select(r => new SelectListItem
                             {
                                 Text = r.GetEnumValue<ClimbingStyles>().EnumFriendlyValue(),
                                 Value = r,
                                 Selected = (r == list.StyleStr)
                             })
                             .OrderBy(s => s.Text);
            ViewBag.Styles = styles;

            var groups = Context.ResultLists
                                .Where(l => l.CompId == list.CompId && l.StyleStr == list.StyleStr && l.AgeGroup != null)
                                .Select(l => l.AgeGroup)
                                .Distinct()
                                .OrderBy(g => g.AgeGroup.ShortName)
                                .ToList()
                                .Select(r => new SelectListItem
                                {
                                    Text = r.AgeGroup.ShortName,
                                    Value = r.Iid,
                                    Selected = r.Iid == list.GroupId
                                });
            ViewBag.Groups = groups;

            var lists = Context.ResultLists
                               .Where(r => r.GroupId == list.GroupId && r.StyleStr == list.StyleStr)
                               .OrderBy(r => r.CreateDate)
                               .ToList()
                               .Select(r => new SelectListItem
                               {
                                   Text = r.RoundName,
                                   Value = r.Iid,
                                   Selected = r.Iid == list.Iid
                               });
            ViewBag.Lists = lists;
        }

        static IQueryable<T> TryApplyFilter<T>(IQueryable<T> source, System.Linq.Expressions.Expression<Func<T,Boolean>> filter)
        {
            var s = source.Where(filter);
            return s.Count() > 0 ? s : source;
        }

        public ActionResult Index(String compId = null, String style = null, String groupId = null, String listId = null)
        {
            var listToShow = String.IsNullOrEmpty(listId) ? null : Context.ResultLists.FirstOrDefault(r => r.Iid == listId);
            if (listToShow == null)
            {
                if (Context.ResultLists.Count(c => c.CompId == compId) < 1)
                    return HttpNotFound();
                var lists = Context.ResultLists.Where(c => c.CompId == compId && c.AgeGroup != null);
                if(!string.IsNullOrEmpty(style))
                    lists = TryApplyFilter(lists, l => l.StyleStr == style.ToString());
                if (!string.IsNullOrEmpty(groupId))
                    lists = TryApplyFilter(lists, l => l.GroupId == groupId);
                listToShow = lists.OrderByDescending(l => l.CreateDate).FirstOrDefault();
                if (listToShow == null)
                    return HttpNotFound();
            }
            PrepareViewBag(listToShow);
            return View(listToShow);
        }

        private ListHeader FindList(string id)
        {
            var list = Context.ResultLists.FirstOrDefault(l => l.Iid == id);
            if (list == null)
                throw new HttpException(404, "List not found");

#if DEBUG
            list.Sort(Context);
#endif
            list.LoadSortedResults(Context);
            return list;
        }

        [ChildActionOnly]
        public PartialViewResult RenderLead(String id)
        {
            var list = FindList(id);
            string viewName;
            switch (list.ListType)
            {
                case ListType.LeadFlash:
                    viewName = "RenderFlash";
                    break;
                default:
                    viewName = "RenderLead";
                    break;
            }
            return PartialView(viewName, list);
        }

        [ChildActionOnly]
        public PartialViewResult RenderBoulder(String id)
        {
            var list = FindList(id);
            string viewName;
            switch (list.ListType)
            {
                case ListType.BoulderSuper:
                    viewName = "RenderLead";
                    break;
                default:
                    viewName = "RenderBoulder";
                    break;
            }

            return PartialView(viewName, list);
        }

        [ChildActionOnly]
        public PartialViewResult RenderSpeed(String id)
        {
            return PartialView(this.FindList(id));
        }
    }
}