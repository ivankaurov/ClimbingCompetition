// <copyright file="CompetitionViewModel.cs">
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

﻿using ClimbingCompetition.Common;
using ClimbingEntities;
using ClimbingEntities.Competitions;
using DbAccessCore.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebClimbing2.Models.CompetitionManagement
{
    public class CompetitionViewModel : IValidatableObject
    {
        public string Iid { get; set; }

        [Display(Name = "Полное наименование"), Required]
        public string FullName { get; set; }

        [Display(Name = "Краткое наименование"), Required]
        public string ShortName { get; set; }

        [Display(Name = "Дата начала"), Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата окончания"), Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Окончание заявок"), Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ApplicationEndDate { get; set; }

        [Display(Name = "Окончание коректировки заявок"), Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AppCorrEndDate { get; set; }

        [Display(Name = "Участник может выступать за несколько команд")]
        public Boolean AllowMultipleTeams { get; set; }

        [Display(Name = "Виды соревнований")]
        public ClimbingStyles Styles { get; set; }

        [Display(Name ="Пароль для онлайн-трансляции")]
        public string Password { get; set; }

        [Display(Name = "Место проведения"), Required]
        public string OrganizerId { get; set; }
        public CompetitionViewModel() { }

        public CompetitionViewModel(Competition comp)
        {
            this.AllowMultipleTeams = comp.GetBooleanValue(CompetitionParamId.AllowMultipleTeams);
            this.AppCorrEndDate = comp.GetDatetimeValue(CompetitionParamId.CorrectionsEndDate, comp.StartDate);
            this.ApplicationEndDate = comp.GetDatetimeValue(CompetitionParamId.ApplicationsEndDate, comp.StartDate);
            this.EndDate = comp.GetDatetimeValue(CompetitionParamId.EndDate, comp.StartDate);
            this.FullName = comp.Name;
            this.Iid = comp.Iid;
            this.ShortName = comp.ShortName;
            this.StartDate = comp.StartDate;
            this.Styles = comp.GetStyles();
            this.Password = comp.GetStringParameterValue(CompetitionParamId.UpdatePassword, string.Empty);
        }

        public Competition Persist(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            
            context.BeginPhysicalTransaction();
            try
            {
                var _ltr = ltr ?? context.BeginLtr();

                Competition entity = null;
                if (!string.IsNullOrEmpty(this.Iid))
                {
                    entity = context.Competitions.FirstOrDefault(c => c.Iid == this.Iid);
                    if (entity == null)
                        throw new InvalidOperationException("Invalid competition");
                    if (entity.GetNotNullableRigths(context.CurrentUserID, DbAccessCore.RightsActionEnum.Edit, context) < DbAccessCore.RightsEnum.Allow)
                        throw new InvalidOperationException("Not allowed");

                    _ltr.AddUpdatedObjectBefore(entity, context);
                }
                else
                {
                    var region = context.Teams.FirstOrDefault(t => t.Iid == this.OrganizerId);
                    if (region == null)
                        throw new InvalidOperationException("Invalid region");
                    if (region.ParentTeam == null)
                    {
                        if (!context.CurrentUserIsAdmin)
                            throw new InvalidOperationException("Not allowed");
                    }
                    else if (region.ParentTeam.GetNotNullableRigths(context.CurrentUserID, DbAccessCore.RightsActionEnum.Edit, context) < DbAccessCore.RightsEnum.Allow)
                        throw new InvalidOperationException("Not allowed");

                    entity = context.Competitions.Add(new Competition(context));
                }

                entity.Name = this.FullName;
                entity.ShortName = this.ShortName;
                entity.StartDate = this.StartDate;
                entity.OrganizerId = this.OrganizerId;
                entity.Organizer = context.Teams.FirstOrDefault(t => t.Iid == this.OrganizerId);

                if (string.IsNullOrEmpty(this.Iid))
                    _ltr.AddCreatedObject(entity, context);
                else
                    _ltr.AddUpatedObjectAfter(entity, context);

                context.SaveChanges();

                entity.SetDateTimeValue(CompetitionParamId.ApplicationsEndDate, this.ApplicationEndDate, context, _ltr);
                entity.SetDateTimeValue(CompetitionParamId.CorrectionsEndDate, this.AppCorrEndDate, context, _ltr);
                entity.SetDateTimeValue(CompetitionParamId.EndDate, this.EndDate, context, _ltr);
                entity.SetStringParameterValue(CompetitionParamId.UpdatePassword, this.Password, context, _ltr);
                entity.SetStyles(this.Styles, context, _ltr);

                if (ltr == null)
                    _ltr.Commit(context);

                context.SaveChanges();
                context.CommitPhysicalTransaction();
                return entity;
            }
            catch
            {
                context.RollbackPhysicalTransaction();
                throw;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
                yield return new ValidationResult("Дата окончания должна быть позже даты начала", new string[] { "EndDate", "StartDate" });
            if (StartDate < AppCorrEndDate)
                yield return new ValidationResult("Дата окончания корректировок должна быть меньше даты начала", new string[] { "StartDate", "AppCorrEndDate" });
            if (AppCorrEndDate < ApplicationEndDate)
                yield return new ValidationResult("Дата окончания заявок должна быть меньше даты окончания корректировок", new string[] { "AppCorrEndDate", "ApplicationEndDate" });
        }
    }
}