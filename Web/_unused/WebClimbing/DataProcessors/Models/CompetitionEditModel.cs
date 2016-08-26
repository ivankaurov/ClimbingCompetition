using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebClimbing.Models;
using WebClimbing.ServiceClasses;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class CompetitionEditModel : IValidatableObject
    {
        public long Iid { get; set; }

        [Display(Name = "Краткое наименование соревнований")]
        [Required(ErrorMessage = "Введите краткое название соревнований", AllowEmptyStrings = false)]
        public String ShortName { get; set; }

        [Display(Name = "Наименование соревнований")]
        [Required(ErrorMessage = "Введите название соревнований", AllowEmptyStrings = false)]
        public String FullName { get; set; }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get { return this.startDate; }
            set
            {
                this.startDate = value;
                this.StartDateText = value.GetDateString();
            }
        }
        [Display(Name = "Начало соревнований")]
        [Required(ErrorMessage = "Введите дату начала соревнований", AllowEmptyStrings = false)]
        public String StartDateText { get; set; }

        private DateTime? stopDate;
        public DateTime? StopDate
        {
            get { return this.stopDate; }
            set
            {
                this.stopDate = value;
                this.StopDateText = value.GetDateString();
            }
        }
        [Display(Name = "Окончание соревнований")]
        [Required(ErrorMessage = "Введите дату окончания соревнований", AllowEmptyStrings = false)]
        public String StopDateText { get; set; }

        private DateTime? appEndDate;
        public DateTime? ApplicationsEndDate
        {
            get { return this.appEndDate; }
            set
            {
                this.appEndDate = value;
                this.ApplicationsEndDateString = value.GetDateString();
            }
        }

        [Display(Name = "Окончание приёма заявок")]
        [Required(ErrorMessage = "Введите дату окончания заявок", AllowEmptyStrings = false)]
        public String ApplicationsEndDateString { get; set; }

        private DateTime? appEndEditDate;
        public DateTime? ApplicationsEditEndDate
        {
            get { return this.appEndEditDate; }
            set
            {
                this.appEndEditDate = value;
                this.ApplicationsEditEndDateString = value.GetDateString();
            }
        }
        
        [Display(Name = "Окончание корректировки заявок")]
        [Required(ErrorMessage = "Введите дату окончания корректировки заявок", AllowEmptyStrings = false)]
        public String ApplicationsEditEndDateString { get; set; }

        [Display(Name = "Разрешить поздние дозаявки")]
        public bool AllowLateApps { get; set; }

        [Display(Name = "Требовать ЭЦП заявок")]
        public bool RequireSignature { get; set; }

        [Display(Name = "Участники могут выступать за несколько команд")]
        public bool AllowMultipleTeams { get; set; }

        public RegionModel Region { get; set; }
        [Required(ErrorMessage = "Выберите регион проведения")]
        [Display(Name = "Регион")]
        public long? RegionId { get; set; }

        [Display(Name = "Трудность")]
        public bool Lead { get; set; }
        [Display(Name = "Скорость")]
        public bool Speed { get; set; }
        [Display(Name = "Боулдеринг")]
        public bool Boulder { get; set; }

        [Display(Name = "Возрастные группы")]
        public List<Comp_AgeGroupEdit> ageGroups { get; set; }

        public CompetitionEditModel()
        {
            this.Iid = 0;
            this.Lead = this.Speed = this.Boulder = false;
            this.ShortName = this.FullName = String.Empty;
            this.StartDate = this.StopDate = this.ApplicationsEndDate = null;
            this.Region = null;
            this.RegionId = null;
            this.ageGroups = new List<Comp_AgeGroupEdit>();
        }

        public CompetitionEditModel(ClimbingContext db)
            : this()
        {
            var agr = db.AgeGroups.ToList();
            agr.Sort();
            foreach (var a in agr)
                this.ageGroups.Add(new Comp_AgeGroupEdit(a, false));
        }

        public CompetitionEditModel(CompetitionModel model, ClimbingContext db)
        {
            this.Iid = model.Iid;
            this.ShortName = model.ShortName;
            this.FullName = model.Name;
            this.StartDate = model.Start;
            this.StopDate = model.End;
            this.ApplicationsEndDate = model.ApplicationsEnd;

            this.Region = model.Region;
            this.RegionId = model.RegionId;
            this.Lead = model.Lead;
            this.Speed = model.Speed;
            this.Boulder = model.Boulder;
            this.ageGroups = new List<Comp_AgeGroupEdit>();
            this.AllowLateApps = model.AllowLateAppl;
            this.ApplicationsEditEndDate = model.ApplicationsEditEnd;
            this.RequireSignature = model.SignApplications;
            this.AllowMultipleTeams = model.AllowMultipleTeams;
            var grpList = db.AgeGroups.ToList();
            grpList.Sort();
            foreach (var agr in grpList)
                ageGroups.Add(new Comp_AgeGroupEdit(agr, (agr.CompetitionGroups.Count(cg => cg.CompetitionId == model.Iid) > 0)));
        }

        private ValidationResult validateExpression(Expression<Func<CompetitionEditModel, String>> sourceExpression, out DateTime? value)
        {
            var retVal = sourceExpression.Compile().Invoke(this);
            try
            {
                value = retVal.GetDateValue();
                return null;
            }
            catch (FormatException)
            {
                value = null;
                string gData = null;
                try
                {
                    var property = sourceExpression.Body as System.Linq.Expressions.MemberExpression;
                    if (property != null)
                        gData = property.Member.Name;
                }
                catch { }
                String[] oData;
                if (String.IsNullOrEmpty(gData))
                    oData = null;
                else
                    oData = new String[] { gData };

                return new ValidationResult("Пожалуйста, введите дату в формате ДД.ММ.ГГГГ", oData);
            }
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationResult vRes;
            DateTime? val;
            vRes = validateExpression(m => m.StartDateText, out val);
            if (vRes == null)
                this.StartDate = val;
            else
                yield return vRes;

            vRes = validateExpression(m => m.StopDateText, out val);
            if (vRes == null)
                this.StopDate = val;
            else
                yield return vRes;

            vRes = validateExpression(m => m.ApplicationsEndDateString, out val);
            if (vRes == null)
                this.ApplicationsEndDate = val;
            else
                yield return vRes;

            vRes = validateExpression(m => m.ApplicationsEditEndDateString, out val);
            if (vRes == null)
                this.ApplicationsEditEndDate = val;
            else
                yield return vRes;

            DateTime startDate = this.StartDate ?? DateTime.MinValue;
            DateTime stopDate = this.StopDate ?? DateTime.MaxValue;
            DateTime appEnd = this.ApplicationsEndDate ?? DateTime.MinValue;
            DateTime appEditEnd = this.ApplicationsEditEndDate ?? DateTime.MinValue;
            if (stopDate < startDate)
                yield return new ValidationResult("Дата окончания должна быть больше или равной дате начала", new String[] { "StopDate" });
            if (appEditEnd < appEnd)
                yield return new ValidationResult("Дата окончания корректировок заявок должна быть больше или равной дате окончания заявок", new String[] { "ApplicationsEditEndDate" });
            if (appEditEnd > startDate)
                yield return new ValidationResult("Дата окончания корректировки заявок должан быть меньше или равной дате начала", new String[] { "ApplicationsEditEndDate" });
            if (!(Lead || Speed || Boulder))
                yield return new ValidationResult("Виды не выбраны");
            if (ageGroups == null || ageGroups.Count(g => g.Confirmed) < 1)
                yield return new ValidationResult("Группы не выбраны");
            if (ageGroups != null)
            {
                ClimbingContext db = new ClimbingContext();
                Dictionary<int, AgeGroupModel> groups = new Dictionary<int, AgeGroupModel>();
                foreach (var agr in ageGroups.Where(g => g.Confirmed))
                {
                    AgeGroupModel a = GetAgeGroup(db, groups, agr);
                    if (a == null)
                    {
                        yield return new ValidationResult(String.Format("Группа с ID={0} не найдена.", agr.GroupId));
                        continue;
                    }
                    int minAge = a.MinAge ?? int.MinValue;
                    int maxAge = a.MaxAge ?? int.MaxValue;
                    var crossGroups = ageGroups
                                        .Where(g => g.Confirmed)
                                        .Select(g => GetAgeGroup(db, groups, g))
                                        .Where(g => g != null)
                                        .Where(g => g.Iid != a.Iid)
                                        .Where(g => g.GenderCode == a.GenderCode
                                            && (minAge >= (g.MinAge ?? int.MinValue) && minAge <= (g.MaxAge ?? int.MaxValue)
                                              || maxAge >= (g.MinAge ?? int.MinValue) && maxAge <= (g.MaxAge ?? int.MaxValue)
                                              )).ToList();
                    if (crossGroups.Count < 1)
                        continue;
                    crossGroups.Sort();
                    StringBuilder sb = new StringBuilder();
                    foreach (var g in crossGroups)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(g.FullName);
                    }
                    yield return new ValidationResult(String.Format("Группа {0} пересекается с группами {1}", a.FullName, sb.ToString()));
                }
            }
        }

        private static AgeGroupModel GetAgeGroup(ClimbingContext db, Dictionary<int, AgeGroupModel> groups, Comp_AgeGroupEdit agr)
        {
            AgeGroupModel a;
            if (groups.ContainsKey(agr.GroupId))
                a = groups[agr.GroupId];
            else
            {
                a = db.AgeGroups.Find(agr.GroupId);
                groups.Add(agr.GroupId, a);
            }
            return a;
        }
    }
}