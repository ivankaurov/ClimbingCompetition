using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common
{
    [Flags]
    public enum Rules
    {
        Russian = 0x0,
        International = 0x1,
        BestRouteInQf = 0x2
    }
}
