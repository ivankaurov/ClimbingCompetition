using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common.API
{
    public sealed class ApiListHeader : _BaseApiClass
    {
        public int SecretaryId { get; set; }
        public string AgeGroupInCompId { get; set; }

        public string PreviousRoundId { get; set; }

        public string IidParent { get; set; }

        public string Iid { get; set; }

        public ListType ListTypeV { get; set; }

        public ClimbingStyles Style { get; set; }

        public ClimbingRound Round { get; set; }

        public int RouteNumber { get; set; }

        public bool Live { get; set; }
        
        public Rules ClimbingRules { get; set; }

        public int Quota { get; set; }
    }
}
