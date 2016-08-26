using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore;
using DbAccessCore.Log;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_competition")]
    public partial class Competition : ClimbingBaseObject
    {
        protected Competition() { }

        public Competition(ClimbingContext2 context) : base(context) { }

        [Column("name"), MaxLength(TYPE_SIZE), Required]
        public String Name { get; set; }

        [Column("short_name"),MaxLength(50), Required]
        public String ShortName { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("organizer_id")]
        public String OrganizerId { get; set; }

        [ForeignKey("OrganizerId")]
        public virtual Teams.Team Organizer { get; set; }

        [NotMapped]
        public Teams.Team CompetitionZone { get { return Organizer.ParentTeam; } }

        [NotMapped]
        public String CompetitionZoneId { get { return CompetitionZone == null ? null : CompetitionZone.Iid; } }

        [NotMapped]
        public int CompetitionYear { get { return StartDate.Year; } }

        public virtual ICollection<ClimberOnCompetition> Competitors { get; set; }
        public virtual ICollection<AgeGroupOnCompetition> AgeGroups { get; set; }
        public virtual ICollection<Lists.ListHeader> ResultLists { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.Competitions.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.ResultLists != null)
                this.RemoveChildCollection(context, ResultLists.OrderByDescending(r => r.Iid), ltr);
            this.RemoveChildCollection(context, AgeGroups, ltr);
            this.RemoveChildCollection(context, Parameters, ltr);
        }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);

            if (result.HasValue)
                return result;

            var organizer = this.Organizer;
            if (organizer == null)
                organizer = ((ClimbingContext2)context).Teams.FirstOrDefault(t => t.Iid == this.OrganizerId);
            if (organizer == null)
                return null;
            if (organizer.ParentTeam == null)
                return context.UserIsAdmin(securityEntityID) ? new RightsEnum?(RightsEnum.Allow) : null;
            else
                return organizer.ParentTeam.GetRights(securityEntityID, action, context, out isInherited);
        }
    }
}
