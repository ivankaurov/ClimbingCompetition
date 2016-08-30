// <copyright file="AgeGroup.cs">
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

﻿using ClimbingCompetition.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore;
using DbAccessCore.Log;

namespace ClimbingEntities.AgeGroups
{
    [Table("cl_ag_age_group")]
    public class AgeGroup : ClimbingBaseObject
    {
        protected AgeGroup() { }
        public AgeGroup(ClimbingContext2 context) : base(context) { this.Gender = Gender.Male; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.AgeGroups.Remove(this);
        }

        [Column("full_name"), MaxLength(TYPE_SIZE), Required]
        [Index("UX_GR_FullName", IsUnique = true)]
        public string FullName { get; set; }

        [Column("short_name"), MaxLength(2 * IID_SIZE), Required]
        public string ShortName { get; set; }

        [NotMapped]
        public Gender Gender { get; set; }

        [Column("gender"), MaxLength(1)]
        [Index("UX_GR_Age", IsUnique = true, Order = 2)]
        public String GenderC
        {
            get { return Gender.GetFirstLetter(); }
            protected set { Gender = value.GetByFirstLetter<Gender>(); }
        }

        [Column("age_young")]
        [Index("UX_GR_Age", IsUnique = true, Order = 0)]
        public int AgeYoung { get; set; }

        [Column("age_old")]
        [Index("UX_GR_Age", IsUnique = true, Order = 1)]
        public int AgeOld { get; set; }

        public virtual ICollection<Competitions.AgeGroupOnCompetition> AgeGroupAppearances { get; set; }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (AgeGroupAppearances != null && AgeGroupAppearances.Count > 0)
                throw new InvalidOperationException("Can\'t delete age group. It is used at one or more competitions");
        }
    }
}
