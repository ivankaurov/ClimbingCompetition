using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common
{
    public enum NextRoundQf : byte
    {
        [EnumDisplay("")]
        NotQf = 0x00,

        [EnumDisplay("q")]
        LuckyLooser = 0x01,

        [EnumDisplay("Q")]
        Qualified = 0x02
    }
}
