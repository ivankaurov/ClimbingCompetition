// <copyright file="ClimberQf.cs">
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

﻿using System;
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
