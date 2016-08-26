using ClimbingCompetition.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore;
using DbAccessCore.Log;

namespace ClimbingEntities.AgeGroups
{
    [Table("cl_ag_age_group")]
    public class AgeGroup : ClimbingBaseObject
    {
        protected AgeGroup() { }
        public AgeGroup(ClimbingContext2 context) : base(context) { this.Gender = Gender.Male; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.AgeGroups.Remove(this);
        }

        [Column("full_name"), MaxLength(TYPE_SIZE), Required]
        [Index("UX_GR_FullName", IsUnique = true)]
        public string FullName { get; set; }

        [Column("short_name"), MaxLength(2 * IID_SIZE), Required]
        public string ShortName { get; set; }

        [NotMapped]
        public Gender Gender { get; set; }

        [Column("gender"), MaxLength(1)]
        [Index("UX_GR_Age", IsUnique = true, Order = 2)]
        public String GenderC
        {
            get { return Gender.GetFirstLetter(); }
            protected set { Gender = value.GetByFirstLetter<Gender>(); }
        }

        [Column("age_young")]
        [Index("UX_GR_Age", IsUnique = true, Order = 0)]
        public int AgeYoung { get; set; }

        [Column("age_old")]
        [Index("UX_GR_Age", IsUnique = true, Order = 1)]
        public int AgeOld { get; set; }

        public virtual ICollection<Competitions.AgeGroupOnCompetition> AgeGroupAppearances { get; set; }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (AgeGroupAppearances != null && AgeGroupAppearances.Count > 0)
                throw new InvalidOperationException("Can\'t delete age group. It is used at one or more competitions");
        }
    }
}
