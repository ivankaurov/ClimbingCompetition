// <copyright file="ClimbingRound.cs">
// Copyright © 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

﻿using Extensions;
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
