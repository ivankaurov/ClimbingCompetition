using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing.Models;
using XmlApiData;
using ClimbingCompetition;
using System.Data;

namespace WebClimbing.Controllers
{
    [AllowAnonymous]
    public class DataController : Controller
    {
        ClimbingContext db = new ClimbingContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        //
        // GET: /Data/

        public ActionResult Index(long id = 0)
        {
            var comp = db.Competitions.Find(id);
            if (comp == null || comp.Lists.Count < 1)
                return RedirectToAction("Index", "Home");

            var availableStylesL = comp.Lists
                                    .Select(l => l.Style)
                                    .Distinct()
                                    .ToList()
                                    .Select(s => new
                                    {
                                        Name = s,
                                        maxP = comp.Lists
                                             .Where(l => l.Style.Equals(s, StringComparison.OrdinalIgnoreCase))
                                             .Max(l => l.LocalIid)
                                    })
                                    .OrderByDescending(l => l.maxP);
            var availableStyles = availableStylesL.Select(l => l.Name);
            String selectedStyle = availableStyles.FirstOrDefault();
            var selectListStyles = new SelectList(availableStyles, selectedStyle);
            ViewBag.Styles = selectListStyles;
            ViewBag.SelectedStyle = selectedStyle;
            return View(comp);
        }

        public ActionResult GetGroups(long id, String style, String divId)
        {
            ViewBag.DivID = divId;
            var comp = db.Competitions.Find(id);
            if (comp == null || style == null)
                return new HttpNotFoundResult();
            var availableGroups = comp.Lists
                                  .Where(l => l.Style.Equals(style, StringComparison.OrdinalIgnoreCase))
                                  .Select(l => l.Group)
                                  .Distinct()
                                  .ToList();
            availableGroups.Sort((a, b) =>
            {
                if (a == null && b == null)
                    return 0;
                if (a == null)
                    return -1;
                if (b == null)
                    return 1;
                int n = (b.AgeGroup.MinAge ?? 0).CompareTo(a.AgeGroup.MinAge ?? 0);
                if (n != 0)
                    return n;
                return (a.AgeGroup.GenderCode.CompareTo(b.AgeGroup.GenderCode));
            });
            var grpToSelect = availableGroups.Select(gr => gr != null ?
                new { Id = gr.Iid, Name = gr.AgeGroup.SecretaryName, Value = gr } :
                new { Id = (long)0, Name = "-", Value = (Comp_AgeGroupModel)null }).ToList();
            
            var fGroup = grpToSelect.FirstOrDefault();
            object selectedGroup = (fGroup == null ? null : fGroup.Id.ToString());
            var selectListGroups = new SelectList(grpToSelect, "Id", "Name", selectedGroup);

            ViewBag.Groups = selectListGroups;
            ViewBag.SelectedGroup = selectedGroup;
            ViewBag.SelectedStyle = style;
            ViewBag.Id = id;
            return PartialView();
        }

        public ActionResult GetRounds(long id, long groupId, String style, String divId)
        {
            ViewBag.DivID = divId;
            var comp = db.Competitions.Find(id);
            if (comp == null)
                return new HttpNotFoundResult();
            var availableLists = comp.Lists
                                 .Where(l => l.Style.Equals(style, StringComparison.OrdinalIgnoreCase)
                                           && l.GroupId == (id == 0 ? null : new long?(groupId)))
                                 .OrderByDescending(l => l.LocalIid).ToList();
            var firstList = availableLists.FirstOrDefault();
            object selectedList = firstList == null ? null : firstList.Iid.ToString();
            var selectListRounds = new SelectList(availableLists, "Iid", "Round", selectedList);

            ViewBag.Rounds = selectListRounds;
            ViewBag.SelectedRound = selectedList;
            return PartialView();
        }

        public ActionResult GetList(long id, String divId)
        {
            ViewBag.DivID = divId;
            var header = db.Lists.Find(id);
            if (header == null)
                return new HttpNotFoundResult();
            ViewBag.Header = header;
            switch (header.ListType)
            {
                case ListTypeEnum.BoulderGroups:
                case ListTypeEnum.LeadGroups:
                    return RenderResultListSeveralRoutes(header);
                case ListTypeEnum.BoulderSimple:
                    return RenderSimpleResultList(header, h => h.ResultsBoulder, "RenderResultListBoulder");
                case ListTypeEnum.BoulderSuper:
                case ListTypeEnum.LeadSimple:
                    return RenderSimpleResultList(header, h => h.ResultsLead, "RenderResultListLead");
                case ListTypeEnum.LeadFlash:
                    return RenderResultListFlash(header);
                case ListTypeEnum.SpeedFinal:
                case ListTypeEnum.SpeedQualy:
                case ListTypeEnum.SpeedQualy2:
                    return RenderSimpleResultList(header, h => h.ResultsSpeed, "RenderResultListSpeed");
                default:
                    return new HttpNotFoundResult("Какой-то странный протокол");
            }
        }

        private ActionResult RenderResultListSeveralRoutes(ListHeaderModel header)
        {
            return new HttpNotFoundResult();
        }

        private static int ResultSorter(ListLineModel a, ListLineModel b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return 1;
            if (b == null)
                return -1;
            int n = (a.Position ?? int.MaxValue).CompareTo(b.Position ?? int.MaxValue);
            if (n != 0)
                return n;
            return a.StartNumber.CompareTo(b.StartNumber);
        }

        private ActionResult RenderSimpleResultList<T>(ListHeaderModel header, Func<ListHeaderModel, IEnumerable<T>> selector, String viewName)
            where T : ListLineModel
        {
            var model = selector(header).ToList();
            model.Sort(ResultSorter);
            return PartialView(viewName, model);
        }

        private ActionResult RenderResultListGeneral(ListHeaderModel header)
        {
            return new HttpNotFoundResult();
        }


        private ActionResult RenderResultListFlash(ListHeaderModel header)
        {
            var results = ResultsController.GetFlashResult(header);
            var dataTable = SortingClass.CreateStructure();
            dataTable.Columns.Add("pos0", typeof(int));
            //dt.Rows.Add(res.Climber.Vk, 0, String.Empty, res.Climber.SecretaryId.Value, String.Empty,
            //                0.0, String.Empty, sp, curPos);
            //climbersWithResult.Add(res.Climber.SecretaryId.Value);
            foreach (var r in results.Values.OfType<ResultsController.FlashRoundResult>().Where(r=>r.RouteNum==1))
            {
                var climber = header.Competition.Climbers.FirstOrDefault(c => c.SecretaryId == r.ClimberId);
                if (climber == null)
                    continue;
                int sp;
                bool allDSQ = true, allDNS = true;
                foreach (var route in r.Routes)
                {
                    if (allDNS && !route.ResText.ToLowerInvariant().Contains("н/я"))
                        allDNS = false;
                    if (allDSQ && !route.ResText.ToLowerInvariant().Contains("дискв"))
                        allDSQ = false;
                    if (!allDNS || allDSQ)
                        break;
                }
                if (allDNS && allDSQ)
                    continue;
                if (allDNS)
                    sp = 2;
                else if (allDSQ)
                    sp = 1;
                else
                    sp = 0;
                dataTable.Rows.Add(climber.Vk, 0, String.Empty, r.ClimberId, String.Empty, 0.0, String.Empty, sp, r.Pos);
            }
            if(dataTable.Rows.Count>0)
            SortingClass.SortResults(dataTable, header.Quota, true, false, header.CompetitionRules, false);
            List<FlashResult> model = new List<FlashResult>();
            foreach (DataRow row in dataTable.Rows)
            {
                FlashResult flshRes = new FlashResult();
                flshRes.Climber = header.Competition.Climbers.First(r => r.SecretaryId == (int)row[3]);
                flshRes.PointsText = (string)row["ptsText"];
                flshRes.PosText = (string)row["posText"];
                flshRes.PreQf = false;
                flshRes.Qf = (string)row["qf"];
                flshRes.ResText = results[flshRes.Climber.SecretaryId.Value].Pts.ToString("0.00");
                foreach (var list in header.Children.OrderBy(r => r.LocalIid))
                {
                    var listRes = list.ResultsLead.FirstOrDefault(r => r.ClimberId == flshRes.Climber.Iid);
                    if (listRes == null)
                        flshRes.Routes.Add(new LeadResultLine
                        {
                            ResText = String.Empty,
                            PointsText = String.Empty,
                            PosText = String.Empty
                        });
                    else
                        flshRes.Routes.Add(listRes);
                }
                model.Add(flshRes);
            }

            return PartialView("RenderResultListFlash", model);
        }
    }
}
