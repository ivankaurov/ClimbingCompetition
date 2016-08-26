using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Competitions
{
    [Table("cr_cm_climber_teams")]
    public class ClimberTeamOnCompetition : ClimbingBaseObject
    {
        protected ClimberTeamOnCompetition() { }
        public ClimberTeamOnCompetition(ClimbingContext2 context) : base(context) { }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ClimberTeamsOnCompetition.Remove(this);
        }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        [Index("UX_ClimberTeamT", IsUnique = true, Order = 1)]
        public String ClimberOnCompId { get; set; }

        [ForeignKey("ClimberOnCompId")]
        public virtual ClimberOnCompetition Climber { get; set; }

        [Column("team_license_id"), MaxLength(IID_SIZE)]
        [Index("UX_ClimberTeamT", IsUnique = true, Order = 2)]
        public String TeamLicenseId { get; set; }

        [ForeignKey("TeamLicenseId")]
        public virtual People.Climber TeamLicense { get; set; }

        [NotMapped]
        public Teams.Team Team { get { return this.TeamLicense.Team; } }

        public int TeamOrder { get; set; }
    }
}
