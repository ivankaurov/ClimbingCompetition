using ClimbingCompetition.Common;
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