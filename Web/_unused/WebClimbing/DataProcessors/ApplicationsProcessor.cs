// <copyright file="ApplicationsProcessor.cs">
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
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.DataProcessors.Models;
using System.Text;

namespace WebClimbing.DataProcessors
{
    public sealed class ApplicationsProcessor : IDisposable
    {
        private ClimbingContext db = new ClimbingContext();

        #region DisposingPattern
        private void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~ApplicationsProcessor()
        {
            Dispose(false);
        }
        #endregion

        public List<RegionModel> GetRegionsForApps(UserProfileModel user, long compId)
        {
            var comp = db.Competitions.Find(compId);
            if(comp==null)
                return new List<RegionModel>();
            List<RegionModel> regList = new List<RegionModel>();
            if (user.IsInRoleComp(RoleEnum.Admin, comp))
                regList = db.Regions.Where(r => (r.IidParent ?? 0) == (comp.Region.IidParent ?? 0)).ToList();
            else if (user.IsInRoleComp(RoleEnum.User, comp) && user.RegionId != null)
                regList = new List<RegionModel> { user.Region };
            regList.Sort();
            return regList;
        }

        public ClimberApplication[] GetAppsForUser(UserProfileModel user, long compId, int? groupId, long regionId)
        {
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return new ClimberApplication[0];
            if (!user.IsInRoleComp(RoleEnum.Admin, comp))
            {
                if (regionId != user.RegionId || !user.IsInRoleComp(RoleEnum.User, comp))
                    return new ClimberApplication[0];
            }
            IEnumerable<Comp_CompetitiorRegistrationModel> climbers = comp.Climbers.Where(c => c.Teams.Count(ct => ct.RegionId == regionId) > 0);

            if (groupId != null)
                climbers = climbers.Where(c => c.CompAgeGroup.AgeGroupId == groupId);
            var clmList = from r in comp.Climbers
                          join d in db.CompetitionClimberTeams on r.Iid equals d.ClimberRegId
                          where d.RegionId == regionId
                          select new { CLM = r, REG = d };
            if (groupId != null)
                clmList = clmList.Where(a => a.CLM.CompAgeGroup.AgeGroupId == groupId);
            var resList = clmList.ToList()
                          .Select(a => new { DB = a.CLM, UPD = new ClimberApplication(a.REG) })
                          .ToList();
            resList.Sort((a, b) =>
            {
                if (a.DB.Iid == b.DB.Iid)
                    return 0;
                var n = (b.DB.CompAgeGroup.AgeGroup.MinAge ?? 0).CompareTo(a.DB.CompAgeGroup.AgeGroup.MinAge ?? 0);
                if (n != 0)
                    return n;
                n = a.DB.Person.GenderFemale.CompareTo(b.DB.Person.GenderFemale);
                if (n != 0)
                    return n;
                return a.DB.Person.Fullname.CompareTo(b.DB.Person.Fullname);
            });
            return resList.Select(r => r.UPD).ToArray();
        }

        public String DeleteApp(UserProfileModel user, long appId)
        {
            var appToDel = db.CompetitionClimberTeams.Find(appId);
            if (appToDel == null)
                return "Заявка не найдена";
            if (!appToDel.AllowedEdit(user))
                return "У вас нет прав для удаления данной заявки";
            RemoveApp(appToDel);
            db.SaveChanges();
            return String.Empty;
        }

        public long? GenerateLicenseId(long regionId)
        {
            if (db.People.Count() < 1)
                return 1;
            return (db.People.Max(p => p.Iid) + 1);
            //var reg = db.Regions.Find(regionId);
            //if (reg == null)
            //    return null;
            //StringBuilder codeBuilder = new StringBuilder();
            //foreach (var c in (reg.SymCode ?? String.Empty))
            //    if (Char.IsDigit(c))
            //        codeBuilder.Append(c);
            //string startPrefix = codeBuilder.ToString();
            //long result, regionalIndex = 0;
            //do
            //{
            //    result = long.Parse(String.Format("{0}{1:4}", startPrefix, ++regionalIndex));
            //} while (db.People.Count(p => p.Iid == result) > 0);
            //return result;
        }

        public String[] SaveClimberApps(UserProfileModel user, long compId, IEnumerable<ClimberApplication> models)
        {
            if (models == null || models != null && models.Count(m => m.Confirmed) < 1)
                return new String[] { "Нет ни одной подтвержденной заявки" };
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return new String[] { "Не удалось найти сорвенования для заявки" };
            if (!comp.AllowedToAdd(user))
                return new String[] { "У вас нет прав для подачи заявок" };
            var errors = (from item in models.Where(m => m.Confirmed)
                          from error in SaveClimberApp(user, compId, item, false)
                          select error).ToArray();
            if (errors.Length < 1)
                db.SaveChanges();
            return errors;

        }

        public String[] SaveClimberApp(UserProfileModel user, long compId, ClimberApplication model)
        {
            return SaveClimberApp(user, compId, model, true);
        }

        private String[] SaveClimberApp(UserProfileModel user, long compId, ClimberApplication model, bool saveChanges)
        {
            if (model == null)
                return new String[] { "Модель не загружена" };
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return new String[] { "Неверные соревнования" };
            String[] empty = new String[0];
            PersonModel personToApply;
            Comp_ClimberTeam existingApp;
            var retVal = model.Validate(user, db, comp, out personToApply, out existingApp);
            if (retVal.Length > 0)
                return retVal;

            //если надо улалить заявку или заменить участника
            if (model.IsDel || (existingApp != null && (personToApply == null || personToApply.Iid != existingApp.Climber.PersonId)))
            {
                if (model.IsDel && existingApp == null)
                    return empty;

                //удаляем текущую заявку
                RemoveApp(existingApp);
                if (model.IsDel)
                {
                    //если надо только удалить  то выйдем из функции
                    if (saveChanges)
                        db.SaveChanges();
                    return empty;
                }
                existingApp = null;
            }

            //создадим заявку
            var climber = existingApp != null ? existingApp.Climber : null;
            if (climber == null)
            {
                //добавим человека
                if (personToApply == null)
                {
                    var newIid = GenerateLicenseId(model.TeamId.Value);
                    personToApply = new PersonModel
                    {
                        Surname = model.Surname,
                        Name = model.Name,
                        GenderProperty = model.GenderP.Value,
                        DateOfBirth = new DateTime(model.YearOfBirth.Value, 01, 05),
                        Competitions = new List<Comp_CompetitiorRegistrationModel>(),
                        Coach = String.Empty,
                        Email = String.Empty,
                        HomeAddress = String.Empty,
                        Patronymic = String.Empty
                    };
                    if (newIid != null)
                        personToApply.Iid = newIid.Value;
                    db.People.Add(personToApply);
                }
                //найдем его заявку на данные соревы (если заявки нет - то создадим)
                climber = personToApply.Competitions.FirstOrDefault(c => c.CompId == comp.Iid);
                if (climber == null)
                {
                    climber = new Comp_CompetitiorRegistrationModel
                    {
                        CompId = comp.Iid,
                        Teams = new List<Comp_ClimberTeam>()
                    };
                    personToApply.Competitions.Add(climber);
                }
            }
            //обновим заявку
            climber.Boulder = model.Boulder;
            climber.GroupId = model.GroupId;
            climber.Lead = model.Lead;
            climber.Speed = model.Speed;
            climber.Qf = model.Qf.Value;
            //добавим команды
            if (existingApp == null)
            {
                int pos = 1;
                for (; climber.Teams.Count(pt => pt.RegionOrder == pos) > 0; pos++) ;
                climber.Teams.Add(new Comp_ClimberTeam { RegionId = model.TeamId.Value, RegionOrder = pos });
            }
            if (saveChanges)
                db.SaveChanges();
            return empty;
        }

        private void RemoveApp(Comp_ClimberTeam existingApp)
        {
            var climberReg = existingApp.Climber;
            db.CompetitionClimberTeams.Remove(existingApp);
            if (climberReg.Teams.Count < 1)
            {
                var person = climberReg.Person;
                db.CompetitionClimbers.Remove(climberReg);
                if (person.Competitions.Count < 1)
                    db.People.Remove(person);
            }
        }
    }
}