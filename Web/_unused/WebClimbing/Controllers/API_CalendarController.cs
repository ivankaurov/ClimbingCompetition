using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebClimbing.Models;
using XmlApiData;
using WebClimbing.ServiceClasses;
using System.Threading;

namespace WebClimbing.Controllers
{
    [AllowAnonymous]
    public class CalendarController : ApiController
    {
        private ClimbingContext db = new ClimbingContext();

        [HttpGet]
        [ActionName("Competitions")]
        public API_CompetitionCollection GetCompetitions(int id = -1)
        {
            if (id < 0)
                id = DateTime.Now.Year;
            var cList = db.Competitions.Where(c => c.Start.Year == id).ToList().Select(c => c.ToApi());
            return new API_CompetitionCollection(cList);
        }

        [HttpGet]
        [ActionName("Climbers")]
        public API_ClimbersCollection GetCompetitiors(long id)
        {
            var cList = db.CompetitionClimbers.Where(c => c.CompId == id).ToList().Select(c => c.ToApi(true));
            return new API_ClimbersCollection(cList);
        }

        [HttpGet]
        [ActionName("Groups")]
        public API_AgeGroupCollection GetGroups(long id)
        {
            var cList = db.CompetitionAgeGroups
                          .Where(g => g.CompetitionId == id)
                          .ToList()
                          .Select(g => g.ToApi());
            return new API_AgeGroupCollection(cList);
        }

        [HttpGet]
        [ActionName("Teams")]
        public API_RegionCollection GetRegions(long id)
        {
            var comp = db.Competitions.Find(id);
            List<RegionApiModel> lst = new List<RegionApiModel>();
            if (comp != null)
            {
                IEnumerable<RegionModel> list;
                if (comp.Region.IidParent == null)
                    list = db.Regions.Where(r => r.IidParent == null);
                else
                    list = db.Regions.Where(r => r.IidParent == comp.Region.IidParent);
                foreach (var r in list)
                    lst.Add(r.ToApi());
            }
            return new API_RegionCollection(lst);
        }

        private T GetRequestParameter<T>(APISignedRequest request, out CompetitionModel comp) where T : APIBaseRequest
        {
            return request.GetRequestParameter<T>(db, out comp);
        }

        [HttpPost]
        [ActionName("TestDSA")]
        public HttpResponseMessage TestDSA(APISignedRequest request)
        {
            CompetitionModel comp;
            var testMessage = GetRequestParameter<APISimpleRequest>(request, out comp);
            var errorCode = (testMessage != null) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
            return Request.CreateResponse(errorCode);
        }

        [HttpPost]
        [ActionName("PostClimber")]
        public HttpResponseMessage PostClimber(APISignedRequest request)
        {
            CompetitionModel comp;
            var climber = GetRequestParameter<Comp_CompetitorRegistrationApiModel>(request, out comp);
            var res = SaveOneClimber(climber, comp);
            db.SaveChanges();
            var response = Request.CreateResponse<Comp_CompetitorRegistrationApiModel>(HttpStatusCode.Created, res.ToApi(true));
            return response;
        }

        private Comp_CompetitiorRegistrationModel SaveOneClimber(Comp_CompetitorRegistrationApiModel climber, CompetitionModel comp)
        {
            bool hasLock = false;
            try
            {
                Monitor.Enter(ApplicationsController.APPLICATIONS_LOCKER, ref hasLock);
                return SaveOneClimberSynchronized(climber, comp);
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(ApplicationsController.APPLICATIONS_LOCKER);
            }
        }

        private Comp_CompetitiorRegistrationModel SaveOneClimberSynchronized(Comp_CompetitorRegistrationApiModel climber, CompetitionModel comp)
        {
            var group = comp.AgeGroups.FirstOrDefault(g => g.AgeGroupId == climber.GroupID);
            if (group == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            String surname = climber.Surname.GetUnifiedName();
            String name = climber.Name.GetUnifiedName();
            if (climber.YearOfBirth < 20)
                climber.YearOfBirth += 2000;
            else if (climber.YearOfBirth < 100)
                climber.YearOfBirth += 1900;
            PersonModel person;
            if (climber.License > 0)
            {
                person = db.People.Find(climber.License);
                if (person != null && !(
                        person.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase) 
                     && person.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                     && person.YearOfBirth == climber.YearOfBirth
                     && person.GenderFemale == climber.Female
                                       )
                   )
                {
                    person.Surname = surname;
                    person.Name = name;
                    if (person.YearOfBirth != climber.YearOfBirth)
                        person.DateOfBirth = new DateTime(climber.YearOfBirth, person.DateOfBirth.Month, person.DateOfBirth.Day);
                    person.GenderFemale = climber.Female;
                }
            }
            else
                person = db.People.FirstOrDefault(p => p.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase)
                    && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    && p.GenderFemale == climber.Female
                    && p.DateOfBirth.Year == climber.YearOfBirth);
            if (person == null)
            {
                person = new PersonModel
                {
                    Surname = surname,
                    Name = name,
                    GenderFemale = climber.Female,
                    DateOfBirth = new DateTime(climber.YearOfBirth, 1, 1),
                    Patronymic = String.Empty,
                    Coach = String.Empty,
                    HomeAddress = String.Empty,
                    Email = String.Empty,
                    Competitions = new List<Comp_CompetitiorRegistrationModel>()
                };
                db.People.Add(person);
            }
            var compReg = person.Competitions.FirstOrDefault(r => r.CompId == comp.Iid);
            if (compReg == null)
            {
                compReg = new Comp_CompetitiorRegistrationModel
                {
                    CompId = comp.Iid,
                    Person = person,
                    GroupId = group.Iid,
                    Teams = new List<Comp_ClimberTeam>()
                };
                person.Competitions.Add(compReg);
            }
            compReg.Lead = (ApplicationDisplayEnum)climber.Lead;
            compReg.Speed = (ApplicationDisplayEnum)climber.Speed;
            compReg.Boulder = (ApplicationDisplayEnum)climber.Boulder;
            compReg.Qf = climber.Razr.GetEnumValue<Razryad>();
            compReg.RankingLead = climber.RankingLead;
            compReg.RankingiSpeed = climber.RankingSpeed;
            compReg.RankingBoulder = climber.RankingBoulder;
            compReg.SecretaryId = climber.Bib;

            List<long> teamList = new List<long>();
            var mtClimber = climber as Comp_MultipleTeamsClimber;
            if (mtClimber == null)
                teamList.Add(climber.TeamID);
            else
                teamList.AddRange(mtClimber.Teams);
            int i = 0;
            foreach (var teamId in teamList)
            {
                if (db.Regions.Count(r => r.Iid == teamId) < 1)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                var clmteam = compReg.Teams.FirstOrDefault(t => t.RegionId == teamId);
                if (clmteam == null)
                    compReg.Teams.Add(new Comp_ClimberTeam { RegionId = teamId, RegionOrder = (++i) });
                else
                    clmteam.RegionOrder = (++i);
            }
            var delList = compReg.Teams.ToList().Where(t => !teamList.Contains(t.RegionId)).ToArray();
            for (i = 0; i < delList.Length; i++)
                db.CompetitionClimberTeams.Remove(delList[i]);
            return compReg;
        }

        [HttpPost]
        [ActionName("PostClimberCollection")]
        public HttpResponseMessage PostClimberCollection(APISignedRequest request)
        {
            CompetitionModel comp;
            var climberCollection = GetRequestParameter<API_ClimbersCollection>(request, out comp);
            List<Comp_CompetitiorRegistrationModel> addedClimbers = new List<Comp_CompetitiorRegistrationModel>();
            foreach (var newClm in climberCollection.Data)
                addedClimbers.Add(SaveOneClimber(newClm, comp));
            db.SaveChanges();
            bool hasLock = false;
            try
            {
                Monitor.Enter(ApplicationsController.APPLICATIONS_LOCKER, ref hasLock);
                HashSet<long> newClimbers = new HashSet<long>(addedClimbers.Select(c => c.Iid));
                var toRemove = comp.Climbers.ToList().Where(c => !newClimbers.Contains(c.Iid)).ToArray();
                for (int i = 0; i < toRemove.Length; i++)
                {
                    foreach (var result in toRemove[i].Results.ToArray())
                        db.Results.Remove(result);
                    db.CompetitionClimbers.Remove(toRemove[i]);
                }
                db.SaveChanges();
            }
            finally
            {
                if (hasLock)
                    Monitor.Exit(ApplicationsController.APPLICATIONS_LOCKER);
            }
            var res = new API_ClimbersCollection { Data = addedClimbers.Select(c => c.ToApi(true)).ToArray() };
            return Request.CreateResponse<API_ClimbersCollection>(HttpStatusCode.Created, res);
        }

        [HttpPost]
        [ActionName("PostGroup")]
        public HttpResponseMessage PostGroup(APISignedRequest request)
        {
            CompetitionModel comp;
            var group = GetRequestParameter<Comp_AgeGroupApiModel>(request, out comp);
            Comp_AgeGroupModel groupComp = SaveOneAgeGroup(comp, group);

            db.SaveChanges();

            var response = Request.CreateResponse<Comp_AgeGroupApiModel>(HttpStatusCode.Created, groupComp.ToApi());
            return response;
        }

        private Comp_AgeGroupModel SaveOneAgeGroup(CompetitionModel comp, Comp_AgeGroupApiModel group)
        {
            var compYear = comp.Start.Year;
            var minAge = compYear - group.YearYoung;
            var maxAge = compYear - group.YearOld;

            bool noMaxLimit = (maxAge > 80);
            bool noMinLimit = (minAge < 3);

            Gender gender = group.Female ? Gender.Female : Gender.Male;
            IEnumerable<AgeGroupModel> exGroupList = db.AgeGroups.Where(g => g.GenderCode == (int)gender);
            if (noMaxLimit)
                exGroupList = exGroupList.Where(g => (g.MaxAge ?? 100) > 80);
            else if (noMinLimit)
                exGroupList = exGroupList.Where(g => (g.MinAge ?? 0) < 4);
            else
                exGroupList = exGroupList.Where(g => g.MaxAge == maxAge && g.MinAge == minAge);

            AgeGroupModel exGroup = exGroupList.FirstOrDefault();
            if (exGroup == null)
            {
                exGroup = new AgeGroupModel
                {
                    FullName = group.Name,
                    SecretaryName = group.Name,
                    MaxAge = noMaxLimit ? null : new int?(maxAge),
                    MinAge = (noMaxLimit || noMinLimit) ? null : new int?(minAge),
                    GenderProperty = gender,
                    CompetitionGroups = new List<Comp_AgeGroupModel>()
                };
                db.AgeGroups.Add(exGroup);
            }
            Comp_AgeGroupModel groupComp = exGroup.CompetitionGroups.FirstOrDefault(cg => cg.CompetitionId == comp.Iid);
            if (groupComp == null)
            {
                groupComp = new Comp_AgeGroupModel { CompetitionId = comp.Iid};
                exGroup.CompetitionGroups.Add(groupComp);
            }
            return groupComp;
        }

        [HttpPost]
        [ActionName("PostGroupCollection")]
        public HttpResponseMessage PostGroupCollection(APISignedRequest request)
        {
            CompetitionModel comp;
            var groupCollection = GetRequestParameter<API_AgeGroupCollection>(request, out comp);

            List<Comp_AgeGroupModel> addedGroups = new List<Comp_AgeGroupModel>();
            foreach (var agApi in groupCollection.Data)
                addedGroups.Add(SaveOneAgeGroup(comp, agApi));
            db.SaveChanges();
            HashSet<int> savedGroups = new HashSet<int>(addedGroups.Select(g => g.AgeGroupId));
            //comp = db.Competitions.Find(id);
            var toRemove = comp.AgeGroups.ToList().Where(g => !savedGroups.Contains(g.AgeGroupId)).ToArray();
            for (int i = 0; i < toRemove.Length; i++)
                db.CompetitionAgeGroups.Remove(toRemove[i]);
            db.SaveChanges();
            var res = new API_AgeGroupCollection { Data = addedGroups.Select(gr => gr.ToApi()).ToArray() };
            return Request.CreateResponse<API_AgeGroupCollection>(HttpStatusCode.Created, res);
        }

        [HttpPost]
        [ActionName("PostTeam")]
        public HttpResponseMessage PostRegion(APISignedRequest request)
        {
            CompetitionModel comp;
            var region = GetRequestParameter<RegionApiModel>(request, out comp);
            var reg = SaveOneRegion(region, comp);
            db.SaveChanges();
            return Request.CreateResponse<RegionApiModel>(HttpStatusCode.Created, reg.ToApi());
        }

        private RegionModel SaveOneRegion(RegionApiModel region, CompetitionModel comp)
        {
            
            var reg = db.Regions.FirstOrDefault(r => r.Name.Equals(region.Name, StringComparison.OrdinalIgnoreCase));
            if (reg != null)
                return reg;
            String regName = region.Name.ToLowerInvariant();
            if (regName.Contains("респ"))
                regName = regName.Replace("республика", String.Empty).Replace("респ.", String.Empty).Replace("респ", String.Empty);
            else if (regName.Contains("обл."))
                regName = regName.Replace("обл.", String.Empty);
            else if (regName.Contains("область"))
                regName = regName.Replace("область", String.Empty);
            else if (regName.Contains("край"))
                regName = regName.Replace("край", String.Empty);
            else if (regName.Contains("кр."))
                regName = regName.Replace("кр.", String.Empty);
            regName = regName.Trim();
            reg = db.Regions.ToList().FirstOrDefault(r => r.Name.ToLower().Contains(regName));
            if (reg != null)
                return reg;
            reg = new RegionModel
            {
                IidParent = comp.Region.IidParent,
                Name = region.Name,
                SymCode = (comp.Region.IidParent == null ? String.Empty : comp.Region.Parent.SymCode) + (db.Regions.Max(r => r.Iid) + 1).ToString("0000")
            };
            db.Regions.Add(reg);
            return reg;
        }

        [HttpPost]
        [ActionName("PostTeamCollection")]
        public HttpResponseMessage PostRegionCollection(APISignedRequest request)
        {
            CompetitionModel comp;
            var regionCollection = GetRequestParameter<API_RegionCollection>(request, out comp);
            List<RegionModel> savedRegions = new List<RegionModel>();
            foreach (var arm in regionCollection.Data)
                savedRegions.Add(SaveOneRegion(arm, comp));
            db.SaveChanges();
            API_RegionCollection res = new API_RegionCollection { Data = savedRegions.Select(r => r.ToApi()).ToArray() };
            return Request.CreateResponse<API_RegionCollection>(HttpStatusCode.Created, res);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
