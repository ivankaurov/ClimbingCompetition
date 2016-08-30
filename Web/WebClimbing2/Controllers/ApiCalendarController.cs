// <copyright file="ApiCalendarController.cs">
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

﻿using ClimbingCompetition.Common.API;
using ClimbingEntities.Competitions;
using ClimbingEntities.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Extensions;
using DbAccessCore;
using DbAccessCore.Log;
using ClimbingEntities.People;
using ClimbingCompetition.Common;

namespace WebClimbing2.Controllers
{
    [AllowAnonymous]
    public class ApiCalendarController : __ApiBaseController
    {
        [HttpGet]
        public JsonResult<IEnumerable<ApiCompetition>> GetCompetitions(string password)
        {
            var result = from c in Context.Competitions
                         join cp in Context.CompetitionParameters on c.Iid equals cp.CompId
                         where cp.ParamIdString == CompetitionParamId.UpdatePassword.ToString()
                           && cp.StringValue == password
                         select c;
            return Json(result.ToList()
                         .Distinct()
                         .OrderByDescending(c => c.StartDate)
                         .Select(c => new ApiCompetition
                         {
                             FullName = c.Name,
                             ShortName = c.ShortName,
                             CompId = c.Iid
                         }));
        }

        [HttpGet]
        public JsonResult<bool> CheckPassword(string compId, string password)
        {
            return Json(this.CheckCompetitionsPassword(compId, password));
        }

        static ApiAgeGroup GetApiAgeGroup(AgeGroupOnCompetition g)
        {
            return new ApiAgeGroup
            {
                AgeGroupInCompId = g.Iid,
                FullName = g.AgeGroup.FullName,
                Name = g.AgeGroup.ShortName,
                YearOld = g.Competition.CompetitionYear - g.AgeGroup.AgeOld,
                YearYoung = g.Competition.CompetitionYear - g.AgeGroup.AgeYoung,
                Gender = g.AgeGroup.Gender
            };
        }

        [HttpGet]
        public JsonResult<IEnumerable<ApiAgeGroup>> GetAgeGroups(String id)
        {
            var r = Context.AgeGroupsOnCompetition
                          .Where(g => g.CompId == id)
                          .ToArray();
            return Json(r.Select(g => GetApiAgeGroup(g)));
        }

        static IEnumerable<ApiTeam> GetApiTeams(IEnumerable<Team> src)
        {
            return src.OrderBy(t => t.Name)
                      .ToArray()
                      .Select(t => new ApiTeam
                      {
                          Iid = t.Iid,
                          Name = t.Name
                      });
        }

        [HttpGet]
        public JsonResult<IEnumerable<ApiTeam>> GetTeams(String id)
        {
            var r = Context.ClimbersOnCompetition
                           .Where(c => c.CompId == id)
                           .SelectMany(c => c.Teams)
                           .Select(t => t.TeamLicense.Team)
                           .Distinct();

            return Json(GetApiTeams(r));
        }

        [HttpGet]
        public JsonResult<IEnumerable<ApiParticipant>> GetParticipants(String id)
        {
            var r = Context.ClimbersOnCompetition.Where(c => c.CompId == id)
                           .ToArray()
                           .Select(c => new ApiParticipant
                           {
                               AgeGroupInCompId = c.AgeGroupId,
                               Gender = c.Person.Gender,
                               Name = c.Person.Name,
                               OldBib = c.SecretaryBib,
                               PersonId = c.PersonId,
                               Qf = c.Qf,
                               RecordUniqueId = c.Iid,
                               Styles = c.Styles,
                               Surname = c.Person.Surname,
                               TeamsIDs = c.Teams.OrderBy(t => t.TeamOrder).Select(t => t.Team.Iid).ToArray(),
                               YearOfBirth = c.Person.YearOfBirth
                           });
            return Json(r);
        }
        
        [HttpPost]
        public JsonResult<ApiAgeGroup[]> SaveAgeGroups(String id, string password, Boolean refreshAll, IEnumerable<ApiAgeGroup> ageGroupsToSave)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return Json(new ApiAgeGroup[0]);

            var comp = Context.Competitions.FirstOrDefault(c => c.Iid == id);
            if (ageGroupsToSave == null || comp == null || ageGroupsToSave.Count() < 1)
                return Json(new ApiAgeGroup[0]);

            HashSet<String> passed = new HashSet<string>();

            foreach(var ag in ageGroupsToSave)
            {
                var localAG = string.IsNullOrEmpty(ag.AgeGroupInCompId) ? null : Context.AgeGroupsOnCompetition.FirstOrDefault(g => g.Iid == ag.AgeGroupInCompId && g.CompId == id);
                if (localAG != null)
                {
                    localAG.AgeGroup.ShortName = ag.Name ?? string.Empty;
                    passed.Add(localAG.Iid);
                    continue;
                }

                int ageOld = comp.CompetitionYear - ag.YearOld;
                int ageYoung = comp.CompetitionYear - ag.YearYoung;
                var genderC = ag.Gender.GetFirstLetter();
                var globalAG = Context.AgeGroups.FirstOrDefault(g => g.AgeOld == ageOld && g.AgeYoung == ageYoung && g.GenderC == genderC);
                if(globalAG == null)
                {
                    globalAG = Context.AgeGroups.FirstOrDefault(g => g.FullName == (string.IsNullOrEmpty(ag.FullName) ? ag.Name : ag.FullName));
                }
                if (globalAG == null)
                {
                    globalAG = Context.AgeGroups.Add(new ClimbingEntities.AgeGroups.AgeGroup(Context)
                    {
                        AgeOld = ageOld,
                        AgeYoung = ageYoung,
                        Gender = ag.Gender,
                        ShortName = ag.Name,
                        FullName = string.IsNullOrEmpty(ag.FullName) ? ag.Name : ag.FullName,
                        AgeGroupAppearances = new List<AgeGroupOnCompetition>()
                    });
                }
                else
                {
                    globalAG.ShortName = ag.Name ?? string.Empty;
                }

                localAG = globalAG.AgeGroupAppearances.FirstOrDefault(g => g.CompId == id);
                if (localAG == null)
                {
                    globalAG.AgeGroupAppearances.Add(localAG = new AgeGroupOnCompetition(Context)
                    {
                        AgeGroup = globalAG,
                        Competition = comp
                    });
                }
                ag.AgeGroupInCompId = localAG.Iid;
                passed.Add(localAG.Iid);
            }
            
            if(refreshAll)
            {
                foreach (var lAG in Context.AgeGroupsOnCompetition.Where(g => g.CompId == id && !passed.Contains(g.Iid)).ToList())
                    lAG.RemoveObject(Context);
            }
            
            Context.SaveChanges();
            return Json(ageGroupsToSave.ToArray());
        }

        Team ProcessTeam(String name, Competition comp)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            var tm = Context.Teams.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                                  && (comp.Organizer.ParentTeam == null && t.ParentTeam == null
                                                     || comp.Organizer.IidParent != null && comp.Organizer.IidParent == t.IidParent));
            if (tm != null)
                return tm;
            tm = Context.Teams.Add(new Team(Context)
            {
                Name = name,
                ParentTeam = comp.Organizer.ParentTeam,
                TeamClimbers = new List<Climber>()
            });
            tm.GetNextCode(Context);

            Context.SaveChanges();
            return Context.Teams.FirstOrDefault(t => t.Iid == tm.Iid);
        }

        Person ProcessPerson(String iid, String surname, String name, Gender gender, int yearOfBirth)
        {
            if (String.IsNullOrEmpty(surname))
                throw new ArgumentNullException("surname");
            var surnameCheck = (surname.Replace('ё', 'е') + " " + (name ?? string.Empty).Replace('ё', 'е'));
            var gChar = gender.GetFirstLetter();
            Person person;
            if (!string.IsNullOrEmpty(iid))
            {
                person = Context.People.FirstOrDefault(p => p.Iid == iid);
                if (person == null)
                    person = Context.ClimbersOnCompetition.Where(p => p.Iid == iid).Select(p => p.Person).FirstOrDefault();
                if(person != null)
                {
                    person.Surname = surname;
                    person.Name = name;
                    person.Gender = gender;
                    if (person.YearOfBirth != yearOfBirth)
                        person.SetDateOfBirthByYear(yearOfBirth);
                    Context.SaveChanges();
                    return person;
                }
            }

            person = Context.People.FirstOrDefault(p => (p.Surname + " " + p.Name) == surnameCheck
                                                        && p.GenderChar == gChar
                                                        && p.DateOfBirth.Year == yearOfBirth);
            if (person != null)
                return person;
            person = Context.People.Add(new Person(Context)
            {
                ClimbersLicenses = new List<Climber>(),
                CompetitionAppearances = new List<ClimberOnCompetition>(),
                Gender = gender,
                Name = (name ?? string.Empty).Replace('ё', 'е'),
                Surname = surname.Replace('ё', 'е')
            });
            person.SetDateOfBirthByYear(yearOfBirth);

            Context.SaveChanges();

            return Context.People.First(p => p.Iid == person.Iid);
        }

        Climber ProcessLicense(Person person, Team team)
        {
            if (person.ClimbersLicenses == null)
                person.ClimbersLicenses = new List<Climber>();
            var license = person.ClimbersLicenses.FirstOrDefault(l => l.TeamId == team.Iid);
            if (license != null)
                return license;
            person.ClimbersLicenses.Add(license = new Climber(Context)
            {
                CompetitionAppearances = new List<ClimberTeamOnCompetition>(),
                Person = person,
                PersonId = person.Iid,
                Team = team,
                TeamId = team.Iid
            });
            Context.SaveChanges();
            return Context.Climbers.First(l => l.Iid == license.Iid);
        }

        ClimberTeamOnCompetition ProcessLicenseOnCompetition(ClimberOnCompetition climber, Climber license)
        {
            if (climber.Teams == null)
                climber.Teams = new List<ClimberTeamOnCompetition>();
            var res = climber.Teams.FirstOrDefault(t => t.TeamLicenseId == license.Iid);
            if (res != null)
                return res;
            climber.Teams.Add(res = new ClimberTeamOnCompetition(Context)
            {
                Climber = climber,
                TeamLicense = license,
                TeamLicenseId = license.Iid,
                TeamOrder = climber.Teams.Count + 1
            });
            Context.SaveChanges();
            return Context.ClimberTeamsOnCompetition.First(c => c.Iid == res.Iid);
        }

        ClimberOnCompetition ProcessAppearance(Person person, ClimbingStyles styles, int? bib, ClimberQf qf, String ageGroupId,Competition comp)
        {
            if (person.CompetitionAppearances == null)
                person.CompetitionAppearances = new List<ClimberOnCompetition>();
            var app = person.CompetitionAppearances.FirstOrDefault(c => c.CompId == comp.Iid);
            bool isNew = true;
            if (app == null)
                person.CompetitionAppearances.Add(app = new ClimberOnCompetition(Context)
                {
                    Competition = comp,
                    CompId = comp.Iid,
                    Person = person,
                    PersonId = person.Iid,
                    Teams = new List<ClimberTeamOnCompetition>()
                });
            else
            {
                isNew = false;
            }
            app.AgeGroupId = ageGroupId;
            app.Qf = qf;
            app.SecretaryBib = bib;
            app.Styles = styles;
            Context.SaveChanges();
            return Context.ClimbersOnCompetition.First(c => c.Iid == app.Iid);
        }

        [HttpPost]
        public void RemoveAllClimbersExceptFor(string id, string password, IEnumerable<string> climbersForComp)
        {
            if(!this.CheckCompetitionsPassword(id, password))
            {
                return;
            }

            var comp = Context.Competitions.FirstOrDefault(c => c.Iid == id);
            if (comp == null)
                return;

            var passed = new HashSet<string>(climbersForComp ?? new string[0]);

            var ltr = Context.BeginLtr("REMOVE_OTHER_CLIMBERS");

            foreach (var a in comp.Competitors.Where(c => !passed.Contains(c.Iid)).ToList())
                a.RemoveObject(Context, ltr);

            ltr.Commit(Context);
            Context.SaveChanges();
        }

        [HttpPost]
        public JsonResult<ApiParticipant[]> SaveParticipants(String id, String password, Boolean refreshAll, IEnumerable<ApiParticipant> participantsToSave)
        {
            if (!this.CheckCompetitionsPassword(id, password))
                return Json(new ApiParticipant[0]);

            var comp = Context.Competitions.FirstOrDefault(c => c.Iid == id);
            if (participantsToSave == null || participantsToSave.Count() < 1 || comp == null)
                return Json(new ApiParticipant[0]);

            HashSet<String> passed = new HashSet<string>();

            foreach(var clm in participantsToSave)
            {
                var person = ProcessPerson(clm.RecordUniqueId, clm.Surname, clm.Name, clm.Gender, clm.YearOfBirth);
                clm.PersonId = person.Iid;

                var app = ProcessAppearance(person, clm.Styles, clm.OldBib, clm.Qf, clm.AgeGroupInCompId, comp);
                List<String> teamIDs = new List<string>();
                foreach(var tName in clm.TeamNames ?? new String[0])
                {
                    var team = ProcessTeam(tName,  comp);
                    teamIDs.Add(team.Iid);

                    var license = ProcessLicense(person, team);
                    var teamAPp = ProcessLicenseOnCompetition(app, license);
                }
                foreach (var c in app.Teams.Where(t => !teamIDs.Contains(t.TeamLicense.TeamId)).ToList())
                    c.RemoveObject(Context);
                clm.RecordUniqueId = app.Iid;
                clm.TeamsIDs = teamIDs.ToArray();

                Context.SaveChanges();

                passed.Add(app.Iid);
            }

            if (refreshAll)
                foreach (var a in comp.Competitors.Where(c => !passed.Contains(c.Iid)).ToList())
                    a.RemoveObject(Context);
            
            Context.SaveChanges();
            return Json(participantsToSave.ToArray());
        }
    }
}