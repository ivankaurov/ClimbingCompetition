using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;

namespace ClimbingCompetition.Common
{
    [Flags]
    public enum ClimbingStyles : byte
    {
        [EnumDisplay("Lead", typeof(CommonTranslations))]
        Lead = 0x01,

        [EnumDisplay("Speed", typeof(CommonTranslations))]
        Speed = 0x02,

        [EnumDisplay("Bouldering", typeof(CommonTranslations))]
        Bouldering = 0x04,
        
        /*
        [EnumDisplay("Combined", typeof(CommonTranslations))]
        Combined = 0x08*/
    }

    public static class ClimbingStyleExtensions
    {
        public static String GetSerializedValue(this ClimbingStyles styles)
        {
            StringBuilder result = new StringBuilder();
            foreach (var climbingStyle in (ClimbingStyles[])Enum.GetValues(typeof(ClimbingStyles)))
                if ((styles & climbingStyle) == climbingStyle)
                    result.Append(climbingStyle.ToString("G").First());
            return result.ToString();
        }

        public static ClimbingStyles ParseSerializedValue(String serializedValue)
        {
            if (string.IsNullOrEmpty(serializedValue))
                return 0;
            ClimbingStyles result = 0;
            foreach (var style in (ClimbingStyles[])Enum.GetValues(typeof(ClimbingStyles)))
                if (serializedValue.Contains(style.ToString("G").Substring(0, 1)))
                    result = result | style;
            return result;
        }

    }
}
