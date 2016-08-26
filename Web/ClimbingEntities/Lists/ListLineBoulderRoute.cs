using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_boulder_routes")]
    public class ListLineBoulderRoute : ClimbingBaseObject
    {
        protected ListLineBoulderRoute() { }
        public ListLineBoulderRoute(ClimbingContext2 context) : base(context) { }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsBoulderRoutes.Remove(this);
        }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        [Index("UX_BoulderRoute", IsUnique = true, Order = 1)]
        public String ResultIid { get; set; }

        [ForeignKey("ResultIid")]
        public virtual ListLineBoulder Result { get; set; }

        [Column("route_number")]
        [Index("UX_BoulderRoute", IsUnique = true, Order = 2)]
        public int RouteNumber { get; set; }

        [Column("top_attempt")]
        public int TopAttempt { get; set; }

        [Column("bonus_attempt")]
        public int BonusAttempt { get; set; }
    }
}
