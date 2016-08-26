using Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClimbingCompetition.Common
{
    [Flags]
    public enum ClimbingRound
    {
        [EnumDisplay("GeneralResults", typeof(CommonTranslations))]
        GeneralResults = 0x01,

        [EnumDisplay("Final", typeof(CommonTranslations))]
        Final = 0x02,

        [EnumDisplay("Semifinal", typeof(CommonTranslations))]
        Semifinal = 0x03,

        [EnumDisplay("RoundOf8", typeof(CommonTranslations))]
        Quarterfinal = 0x04,

        [EnumDisplay("RoundOf16", typeof(CommonTranslations))]
        RoundOf16 = 0x05,

        [EnumDisplay("Qualification", typeof(CommonTranslations))]
        Qualification = 0x06,

        [EnumDisplay("Superfinal", typeof(CommonTranslations))]
        Superfinal = 0x07,

        [EnumDisplay("Route", typeof(CommonTranslations))]
        Route = 0x0100
    }

    public static class ClimbingRoundExtensions
    {
        public static String GetLocalizedValue(this ClimbingRound round, int routeNumber = 0, CultureInfo language = null)
        {
            if ((round & ClimbingRound.Route) == ClimbingRound.Route)
                return String.Format("{0} {1} {2}", (round & (~ClimbingRound.Route)).EnumFriendlyValue(language), ClimbingRound.Route.EnumFriendlyValue(language), routeNumber);
            else
                return round.EnumFriendlyValue(language);
        }

        public static ClimbingRound GetByLocalizedValue(string roundName, out int routeNumber, CultureInfo language = null)
        {
            if (string.IsNullOrEmpty(roundName))
                throw new ArgumentNullException("roundName");
            var qualiName = CommonTranslations.ResourceManager.GetString("Qualification", language);
            if (roundName.Contains(qualiName))
            {
                ClimbingRound result = ClimbingRound.Qualification;
                roundName = roundName.Replace(qualiName, string.Empty).Trim();

                if (int.TryParse(roundName, out routeNumber))
                {
                    result = result | ClimbingRound.Route;
                }
                else
                    routeNumber = 0;
                return result;
            }

            routeNumber = 0;
            return Extensions.StringExtensions.GetEnumByStringValue(roundName, ClimbingRound.Final, language);
        }
    }
}
