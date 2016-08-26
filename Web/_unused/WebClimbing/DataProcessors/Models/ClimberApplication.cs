using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models;
using System.ComponentModel.DataAnnotations;
using WebClimbing.ServiceClasses;
using WebClimbing.Models.UserAuthentication;
using System.Text;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class ClimberApplication : IValidatableObject
    {
        public override string ToString()
        {
            return this.DisplayName;
        }
        public ClimberApplication()
        {
            this.IsDel = false;
            this.ApplicationId = null;
            this.ModelHeader = "Новый участник";
        }

        public String ModelHeader { get; set; }

        public ClimberApplication(Comp_ClimberTeam clmTeamModel)
        {
            var model = clmTeamModel.Climber;
            this.IsDel = false;
            this.Surname = model.Person.Surname;
            this.Name = model.Person.Name;
            this.YearOfBirth = model.Person.YearOfBirth;
            this.GenderP = model.Person.GenderProperty;
            this.Qf = model.Qf;
            this.Lead = model.Lead;
            this.Speed = model.Speed;
            this.Boulder = model.Boulder;
            this.GroupId = model.GroupId;
            this.GroupName = model.CompAgeGroup.AgeGroup.FullName;
            this.ModelHeader = String.Format("Правка участника {0} {1}", Surname, Name);
            this.ApplicationId = clmTeamModel.Iid;
        }

        public long? ApplicationId { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия не введена", AllowEmptyStrings = false)]
        public String Surname { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя не введено", AllowEmptyStrings = false)]
        public String Name { get; set; }

        [Display(Name = "Пол")]
        [Required(ErrorMessage = "Пол не выбран")]
        public Gender? GenderP { get; set; }

        [Display(Name = "Год рождения")]
        [Required(ErrorMessage = "Год рождения не введён")]
        public int? YearOfBirth { get; set; }

        [Display(Name = "Разряд")]
        [Required(ErrorMessage = "Разряд не выбран")]
        public Razryad? Qf { get; set; }

        [Display(Name = "Трудность")]
        public ApplicationDisplayEnum Lead { get; set; }

        [Display(Name = "Скорость")]
        public ApplicationDisplayEnum Speed { get; set; }

        [Display(Name = "Боулдеринг")]
        public ApplicationDisplayEnum Boulder { get; set; }

        [Display(Name = "Команда")]
        public RegionModel Team { get; set; }
        [Required(ErrorMessage="Команда не выбрана")]
        public long? TeamId { get; set; }


        [Display(Name = "Подтвердить")]
        public bool Confirmed { get; set; }

        public bool IsDel { get; set; }
        public bool IsNew { get { return this.ApplicationId == null; } }
        public bool Changed { get; set; }
        public bool IsEmpty()
        {
            return ((Lead == ApplicationDisplayEnum.NotStarter) &&
                (Speed == ApplicationDisplayEnum.NotStarter) && (Boulder == ApplicationDisplayEnum.NotStarter));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsEmpty())
                yield return new ValidationResult("Виды не выбраны", new String[0]);
            if (YearOfBirth == null)
                yield return new ValidationResult("Год рождения не введен", new String[] { "YearOfBirth" });
            else if (YearOfBirth.Value < 0 || YearOfBirth.Value > DateTime.Now.Year)
                yield return new ValidationResult("Неверный год рождения");
            else if (YearOfBirth < 20)
                YearOfBirth += 2000;
            else if (YearOfBirth < 100)
                YearOfBirth += 1900;
        }

        public String DisplayHeader { get; set; }

        [Display(Name = "Фамилия, Имя")]
        public String DisplayName
        {
            get
            {
                return (Surname == null ? String.Empty : Surname) + " " + (Name == null ? String.Empty : Name);
            }
        }

        [Display(Name="Г.р.")]
        public String AgeChange { get; set; }

        [Display(Name = "Пол")]
        public String GenderChange { get; set; }

        [Display(Name = "Возрастная группа")]
        public String GroupName { get; set; }
        public long GroupId { get; set; }

        private void PrepareName()
        {
            this.Surname = Surname.GetUnifiedName();
            this.Name = Name.GetUnifiedName();
        }

        #region ValidateComp

        private PersonModel FindSuitablePerson(ClimbingContext db)
        {
            var nameList = db.People.Where(p => p.Surname.Equals(Surname, StringComparison.OrdinalIgnoreCase)
                && p.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)).ToList();
            return nameList.FirstOrDefault(p => p.GenderProperty == this.GenderP.Value
                && p.YearOfBirth == this.YearOfBirth.Value);
        }
        public String[] Validate(UserProfileModel user, ClimbingContext db, CompetitionModel comp)
        {
            PersonModel p;
            Comp_ClimberTeam e;
            return Validate(user, db, comp, out p, out e);
        }

        public String[] Validate(UserProfileModel user, ClimbingContext db, CompetitionModel comp, out PersonModel foundPerson, out Comp_ClimberTeam existingApp)
        {
            foundPerson = null;
            existingApp = null;
            this.Team = (this.TeamId == null) ? null : db.Regions.Find(this.TeamId);
            if (this.Team == null)
                return new String[] { "Команда не выбрана" };
            long regionId = this.TeamId.Value;
            if (!comp.AllowedToEdit(user))
                return new String[] { "У вас нет прав на изменение заявок" };
            PrepareName();
            this.AgeChange = this.YearOfBirth.Value.ToString();
            this.GenderChange = this.GenderP.GetFriendlyValue();
            List<String> errors = new List<string>();
            int age = comp.Start.Year - this.YearOfBirth.Value;
            var agrGroup = comp.AgeGroups.ToList().FirstOrDefault(gr =>
                gr.AgeGroup.GenderProperty == this.GenderP.Value
                && (gr.AgeGroup.MaxAge ?? int.MaxValue) >= age
                && (gr.AgeGroup.MinAge ?? 0) <= age);
            if (agrGroup == null)
                errors.Add(String.Format("Участник {0} {1} не входит ни в одну возрастную группу", Surname, Name));
            else
            {
                this.GroupId = agrGroup.Iid;
                this.GroupName = agrGroup.AgeGroup.FullName;
            }
            Comp_ClimberTeam exApp = null;
            if (this.ApplicationId != null)
            {
                exApp = db.CompetitionClimberTeams.Find(this.ApplicationId);
                if (exApp == null)
                    this.ApplicationId = null;
                else
                {
                    if (exApp.RegionId != regionId || exApp.Climber.CompId != comp.Iid)
                        errors.Add(String.Format("У вас нет прав для редактирования заявки {0} {1}", Surname, Name));
                }
            }
            if (exApp == null && !comp.AllowedToAdd(user))
                errors.Add("У вас нет прав для заявки");
            else if (exApp != null && !exApp.AllowedEdit(user))
                errors.Add("У вас нет прав для корректировки заявки");
            if (errors.Count > 0)
                return errors.ToArray();
            var climber = this.FindSuitablePerson(db);

            if (climber != null && !comp.AllowMultipleTeams)
            {
                var curCompReg = climber.Competitions.FirstOrDefault(c => c.CompId == comp.Iid);
                if (curCompReg != null)
                {
                    StringBuilder exTeams = new StringBuilder();
                    foreach (var ct in curCompReg.Teams.Where(ct => ct.RegionId != regionId))
                    {
                        if (exTeams.Length > 0)
                            exTeams.Append(", ");
                        exTeams.Append(ct.Region.Name);
                    }
                    if (exTeams.Length > 0)
                        return new String[] { "Участник уже заявлен от других команд: " + exTeams.ToString() };
                }
            }

            if (exApp != null && (climber == null || climber.Iid != exApp.Climber.PersonId))
            {
                var oldClimber = exApp.Climber.Person;
                bool changed = false;
                if (!oldClimber.Surname.Equals(Surname, StringComparison.Ordinal) ||
                    !oldClimber.Name.Equals(Name, StringComparison.Ordinal))
                {
                    //this.DisplayName = String.Format("{0} {1} => {2}", oldClimber.Surname, oldClimber.Name, DisplayName);
                    changed = true;
                }
                if (this.YearOfBirth.Value != oldClimber.DateOfBirth.Year)
                {
                    this.AgeChange = String.Format("{0} => {1}", oldClimber.DateOfBirth.Year, AgeChange);
                    if (!changed)
                        changed = true;
                }
                if (this.GenderP.Value != oldClimber.GenderProperty)
                {
                    this.GenderChange = String.Format("{0} => {1}", oldClimber.GenderProperty.GetFriendlyValue(), GenderChange);
                    if (!changed)
                        changed = true;
                }
                if (changed && !comp.AllowedToAdd(user))
                    errors.Add("Замена запрещена.");
            }
            if (climber != null)
            {
                var compApply = climber.Competitions.Where(c => c.CompId == comp.Iid).ToList();
                if (compApply.Count(ca => ca.Teams.Count(ct => ct.RegionId == regionId && (exApp == null || exApp != null && exApp.Iid != ct.Iid)) > 0) > 0)
                    errors.Add(String.Format("Участник {0} {1} уже заявлен от Вашего региона", Surname, Name));
            }
            foundPerson = climber;
            existingApp = exApp;
            return errors.ToArray();
        }

        #endregion
    }
}