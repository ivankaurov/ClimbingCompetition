using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;

namespace ClimbingEntities.People
{
    [Table("cl_pr_climbers")]
    public class Climber : ClimbingBaseObject
    {
        protected Climber() { }
        public Climber(ClimbingContext2 context) : base(context) { }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.Climbers.Remove(this);
        }

        [Column("person_id")]
        [MaxLength(IID_SIZE)]

        [Index("UX_Climber_Team", IsUnique = true, Order = 1)]
        public String PersonId { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        [Column("team_id")]
        [MaxLength(IID_SIZE)]
        [Index("UX_Climber_Team", IsUnique = true, Order = 0)]
        public string TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Teams.Team Team { get; set; }

        public virtual ICollection<Competitions.ClimberTeamOnCompetition> CompetitionAppearances { get; set; }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (CompetitionAppearances != null && CompetitionAppearances.Count > 0)
                throw new InvalidOperationException("This climber has appeared on several competitions");
        }
    }
}
