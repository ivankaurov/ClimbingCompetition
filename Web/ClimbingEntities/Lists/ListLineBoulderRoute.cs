// <copyright file="ListLineBoulderRoute.cs">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_boulder_routes")]
    public class ListLineBoulderRoute : ClimbingBaseObject
    {
        protected ListLineBoulderRoute() { }
        public ListLineBoulderRoute(ClimbingContext2 context) : base(context) { }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsBoulderRoutes.Remove(this);
        }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        [Index("UX_BoulderRoute", IsUnique = true, Order = 1)]
        public String ResultIid { get; set; }

        [ForeignKey("ResultIid")]
        public virtual ListLineBoulder Result { get; set; }

        [Column("route_number")]
        [Index("UX_BoulderRoute", IsUnique = true, Order = 2)]
        public int RouteNumber { get; set; }

        [Column("top_attempt")]
        public int TopAttempt { get; set; }

        [Column("bonus_attempt")]
        public int BonusAttempt { get; set; }
    }
}
