using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common
{
    public enum ResultType : short
    {
        [EnumDisplay("")]
        RES = 0,

        [EnumDisplay("DNS", typeof(CommonTranslations))]
        DNS = 1,

        [EnumDisplay("DSQ", typeof(CommonTranslations))]
        DSQ = 2
    }
}
