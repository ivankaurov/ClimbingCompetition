// <copyright file="ApiListLine.cs">
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

namespace ClimbingCompetition.Common.API
{
    public abstract class ApiListLine
    {
        public string Iid { get; set; }
        public long SecretaryId { get; set; }
        public string ListId { get; set; }
        public string ClimberId { get; set; }
        public string ResText { get; set; }
        public long Result { get; set; }
        public int Start { get; set; }
    }

    public class ApiListLineLead : ApiListLine
    {
        public string TimeText { get; set; }
        public long TimeValue { get; set; }
    }

    public class ApiListLineSpeed : ApiListLine
    {
        public enum ResultType { Time, Fall, Dsq, Dns}

        public string Route1Text { get; set; }
        public string Route2Text { get; set; }
        public long Route1Res { get; set; }
        public long Route2Res { get; set; }
        public ResultType TotalResType { get; set; }
    }

    public class ApiListLineBoulder : ApiListLine
    {
        public ResultType ResultType { get; set; }

        public ApiListLineBoulderRoute[] Routes { get; set; }
    }

    public class ApiListLineBoulderRoute
    {
        public int RouteNumber { get; set; }
        public int TopAttempt { get; set; }
        public int BonusAttempt { get; set; }
    }
}
