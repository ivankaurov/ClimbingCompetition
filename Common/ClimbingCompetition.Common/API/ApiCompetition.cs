using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common.API
{
    [Serializable]
    public class ApiCompetition : _BaseApiClass
    {
        public String CompId { get; set; }
        public String FullName { get; set; }
        public String ShortName { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.ShortName) ? this.FullName : this.ShortName;
        }
    }
}
