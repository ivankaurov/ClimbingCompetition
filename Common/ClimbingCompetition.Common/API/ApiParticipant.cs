using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common.API
{
    public class ApiParticipant : _BaseApiClass
    {
        public String Surname { get; set; }
        public String Name { get; set; }
        public ClimberQf Qf { get; set; }
        public String PersonId { get; set; }
        public String RecordUniqueId { get; set; }
        public int? OldBib { get; set; }
        public Gender Gender { get; set; }
        public int YearOfBirth { get; set; }
        public  ClimbingStyles Styles { get; set; }

        public String AgeGroupInCompId { get; set; }

        public String[] TeamsIDs { get; set; }

        public String[] TeamNames { get; set; }
    }
}
