using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore;
using System.ComponentModel.DataAnnotations;

namespace ClimbingEntities.Teams
{
    [Table("cl_tm_users_with_team")]
    public class UserWithTeam : DbAccessCore.Users.DbUser
    {
        protected UserWithTeam() { }
        public UserWithTeam(ClimbingContext2 context) : base(context) { }

        [MaxLength(IID_SIZE)]
        public string TeamId { get; set; }
        public virtual Team Team { get; set; }
    }
}
