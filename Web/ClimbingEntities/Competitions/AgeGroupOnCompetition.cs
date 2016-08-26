using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_age_groups")]
    public class AgeGroupOnCompetition : ClimbingBaseObject
    {
        protected AgeGroupOnCompetition() { }
        public AgeGroupOnCompetition(ClimbingContext2 context) : base(context)
        {
            this.StylesForGroup = 0;
        }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.AgeGroupsOnCompetition.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            RemoveChildCollection(context, Climbers, ltr);
        }

        [Column("comp_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_AgeGroup", IsUnique = true, Order = 0)]
        public String CompId { get; set; }

        [ForeignKey("CompId")]
        public virtual Competition Competition { get; set; }

        [Column("age_group_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_AgeGroup", IsUnique = true, Order = 1)]
        public String AgeGroupId { get; set; }

        [ForeignKey("AgeGroupId")]
        public virtual AgeGroups.AgeGroup AgeGroup { get; set; }

        public virtual ICollection<ClimberOnCompetition> Climbers { get; set; }
        public virtual ICollection<Lists.ListHeader> ResultLists { get; set; }

        [NotMapped]
        public ClimbingStyles StylesForGroup { get; set; }

        [Column("styles_for_group"), MaxLength(IID_SIZE)]
        public String StylesStr
        {
            get { return StylesForGroup.GetSerializedValue(); }
            protected set { StylesForGroup = ClimbingStyleExtensions.ParseSerializedValue(value); }
        }
    }
}
