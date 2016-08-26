using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingCompetition
{
    internal static class SpeedSystemHelper
    {
        public static bool ShowErrors
        {
            get
            {
                return ClimbingCompetition.appSettings.Default.cbShowInfo;
            }
        }

        public static void PersistShowErrors(bool showErrors)
        {
            ClimbingCompetition.appSettings aSet = ClimbingCompetition.appSettings.Default;
            aSet.cbShowInfo = showErrors;
            aSet.Save();
        }
    }
}
