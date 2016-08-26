using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

namespace ClimbingCompetition.Common
{
    [EnumDisplay("")]
    public enum ClimberQf : byte
    {
        [EnumDisplay("")]
        Empty = 11,

        [EnumDisplay("б/р")]
        NoQf = 10,

        [EnumDisplay("3ю")]
        Youth3 = 9,

        [EnumDisplay("2ю")]
        Youth2 = 8,

        [EnumDisplay("1ю")]
        Youth1 = 7,

        [EnumDisplay("3")]
        Adult3 = 6,

        [EnumDisplay("2")]
        Adult2 = 5,

        [EnumDisplay("1")]
        Adult1 = 4,

        [EnumDisplay("КМС")]
        KMC = 3,

        [EnumDisplay("МС")]
        MC = 2,

        [EnumDisplay("МСМК")]
        MCMK = 1,

        [EnumDisplay("ЗМС")]
        ZMS = 0
    }
}
