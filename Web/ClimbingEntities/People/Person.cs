using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;
using ClimbingCompetition.Common;
using DbAccessCore;

namespace ClimbingEntities.People
{
    [Table("cl_pr_people")]
    public class Person : ClimbingBaseObject
    {
        protected Person() { }

        public Person(ClimbingContext2 context) : base(context)
        {
            this.Name = this.Patronymic = string.Empty;
            this.Gender = Gender.Male;
        }

        [Column("surname"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = false)]
        public String Surname { get; set; }

        [Column("name"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = true)]
        public String Name { get; set; }

        [Column("patronymic"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = true)]
        public String Patronymic { get; set; }

        [NotMapped]
        public String FullName
        {
            get { return string.Format("{0} {1}", this.Surname, this.Name); }
        }

        [Column("gender"), MaxLength(1)]
        public String GenderChar { get; protected set; }

        [NotMapped]
        public Gender Gender
        {
            get { return GenderChar.GetByFirstLetter<Gender>(); }
            set { GenderChar = value.GetFirstLetter(); }
        }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int YearOfBirth { get { return DateOfBirth.Year; } }

        public DateTime SetDateOfBirthByYear(int year)
        {
            if (year < 0)
                throw new ArgumentOutOfRangeException("year");
            else if (year < 20)
                year += 2000;
            else if (year < 100)
                year += 1900;
            return this.DateOfBirth = new DateTime(year, 1, 2);
        }

        public virtual ICollection<Climber> ClimbersLicenses { get; set; }
        public virtual ICollection<Competitions.ClimberOnCompetition> CompetitionAppearances { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.People.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.CompetitionAppearances != null && this.CompetitionAppearances.Count > 0)
                throw new InvalidOperationException("This climber has appeared on several competitions");
            RemoveChildCollection(context, ClimbersLicenses, ltr);
        }
    }
}
