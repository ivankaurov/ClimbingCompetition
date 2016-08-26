using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

namespace ClimbingCompetition.Common
{
    public enum Gender
    {
        [EnumDisplay("Male", typeof(CommonTranslations))]
        Male = 0,

        [EnumDisplay("Felame", typeof(CommonTranslations))]
        Female = 1
    }
}
