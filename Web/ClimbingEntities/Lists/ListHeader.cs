using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;
using Extensions;
using DbAccessCore;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_headers")]
    public partial class ListHeader : ClimbingBaseObject
    {
        protected ListHeader() { }
        public ListHeader(ClimbingContext2 context) : base(context)
        {
            this.ListType = ListType.Unknown;
            this.Style = ClimbingStyles.Lead;
            this.Rules = Rules.Russian;
        }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultLists.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.NextRounds != null)
            {
                foreach (var nr in this.NextRounds)
                {
                    nr.PreviousRound = null;
                    nr.PrevRoundIid = null;
                }
                this.NextRounds.Clear();
            }
            this.RemoveChildCollection(context, Children, ltr);
            this.RemoveChildCollection(context, Results, ltr);
        }

        [Column("comp_id"), MaxLength(IID_SIZE)]
        public String CompId { get; set; }
        [ForeignKey("CompId")]
        public virtual Competitions.Competition Competition { get; set; }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        [ForeignKey("Parent")]
        public String IidParent { get; set; }

        public virtual ListHeader Parent { get; set; }
        public virtual ICollection<ListHeader> Children { get; set; }

        [Column("prev_round"), MaxLength(IID_SIZE)]
        [ForeignKey("PreviousRound")]
        public String PrevRoundIid { get; set; }
        
        public virtual ListHeader PreviousRound { get; set; }
        public virtual ICollection<ListHeader> NextRounds { get; set; }
        [NotMapped]
        public ListHeader NextRound
        {
            get
            {
                if (NextRounds == null)
                    return null;
                return NextRounds.FirstOrDefault();
            }
        }

        [Column("age_group"), MaxLength(IID_SIZE)]
        public String GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Competitions.AgeGroupOnCompetition AgeGroup { get; set; }

        [NotMapped]
        public ListType ListType { get; set; }

        [Column("list_type"), MaxLength(2 * IID_SIZE)]
        public String ListTypeStr
        {
            get
            {
                return this.ListType.GetEnumValueString();
            }
            protected set { this.ListType = value.GetEnumValue<ListType>() ?? ListType.Unknown; }
        }

        [NotMapped]
        public ClimbingStyles Style { get; set; }
        [Column("style"), MaxLength(IID_SIZE)]
        public String StyleStr
        {
            get { return this.Style.GetEnumValueString(); }
            set { this.Style = value.GetEnumValue<ClimbingStyles>() ?? ClimbingStyles.Lead; }
        }
        [NotMapped]
        public String StyleFriendlyName { get { return this.Style.EnumFriendlyValue(); } }

        [Column("round")]
        public ClimbingRound Round { get; set; }

        [Column("routeNum")]
        public int RouteNumber { get; set; }

        [NotMapped]
        public String RoundName { get { return Round.GetLocalizedValue(this.RouteNumber); } }

        [Column("live")]
        public Boolean Live { get; set; }

        [Column("rules")]
        public Rules Rules { get; set; }

        [NotMapped]
        public Boolean BestRouteInQf
        {
            get { return ((Rules & Rules.BestRouteInQf) == Rules.BestRouteInQf); }
            set
            {
                if (value)
                    Rules = (Rules | Rules.BestRouteInQf);
                else
                    Rules = Rules & (~Rules.BestRouteInQf);
            }
        }

        [Column("quota")]
        public int Quota { get; set; }

        public virtual ICollection<ListLine> Results { get; set; }
    }
}
