using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common.API
{
    public class ApiAgeGroup : _BaseApiClass
    {
        public int YearOld { get; set; }
        public int YearYoung { get; set; }
        public String Name { get; set; }
        public String FullName { get; set; }
        public String AgeGroupInCompId { get; set; }
        public Gender Gender { get; set; }
    }
}
