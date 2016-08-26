using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;
using Extensions;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_climbers")]
    public class ClimberOnCompetition : ClimbingBaseObject
    {
        protected ClimberOnCompetition() { }
        public ClimberOnCompetition(ClimbingContext2 context) : base(context)
        {
            this.Qf = ClimberQf.Empty;
            this.Styles = 0;
        }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ClimbersOnCompetition.Remove(this);
        }

        [Column("person_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_Person", IsUnique = true, Order = 0)]
        public String PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual People.Person Person { get; set; }

        [Column("comp_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_Person", IsUnique = true, Order = 1)]
        [Index("UX_Comp_Bib", IsUnique = false, Order = 1)]
        [Index("UX_Comp_IntBib", IsUnique = false, Order = 1)]
        public String CompId { get; set; }
        [ForeignKey("CompId")]
        public virtual Competition Competition { get; set; }

        [Column("group_id"), MaxLength(IID_SIZE)]
        public String AgeGroupId { get; set; }

        [ForeignKey("AgeGroupId")]
        public virtual AgeGroupOnCompetition AgeGroup { get; set; }
        

        [NotMapped]
        public ClimberQf Qf { get; set; }

        [Column("qf"), MaxLength(IID_SIZE)]
        public String QfStr { get { return Qf.GetEnumValueString(); } set { Qf = value.GetEnumValue<ClimberQf>() ?? ClimberQf.Empty; } }

        [Column("bib")]
        [Index("UX_Comp_Bib", IsUnique = false, Order = 0)]
        [MaxLength(IID_SIZE)]
        public String Bib { get; set; }

        [Column("old_comp_id")]
        [Index("UX_Comp_IntBib", IsUnique = false, Order = 0)]
        public int? SecretaryBib { get; set; }

        [NotMapped]
        public String FriendlyQf { get { return Qf.EnumFriendlyValue(); } }

        [NotMapped]
        public ClimbingStyles Styles { get; set; }

        [Column("styles"), MaxLength(IID_SIZE)]
        protected String StylesStr
        {
            get { return Styles.GetSerializedValue(); }
            set { Styles = ClimbingStyleExtensions.ParseSerializedValue(value); }
        }

        [Column("vk")]
        public Boolean VK { get; set; }

        public virtual ICollection<ClimberTeamOnCompetition> Teams { get; set; }
        public virtual ICollection<Lists.ListLine> Results { get; set; }

        [NotMapped]
        public String Team
        {
            get
            {
                if (Teams == null)
                    return string.Empty;
                StringBuilder sb = new StringBuilder();
                foreach (var t in Teams.OrderBy(t => string.Format("{0:000} {1}", t.TeamOrder, t.Team.Name))
                                      .Select(t => t.Team.Name)
                                      .Distinct())
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(t);
                }
                return sb.ToString();
            }
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            RemoveChildCollection(context, Teams, ltr);
        }
    }
}