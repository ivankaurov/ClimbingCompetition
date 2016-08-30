// <copyright file="CompetitionParameter.cs">
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
using Extensions;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_comp_parameters")]
    public class CompetitionParameter : ClimbingBaseObject
    {
        protected CompetitionParameter() { }

        public CompetitionParameter(ClimbingContext2 context) : base(context) { }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        public string CompId { get; set; }

        [ForeignKey("CompId")]
        public virtual Competition Competition { get; set; }

        [Column("param_id")]
        [Required]
        [MaxLength(2 * IID_SIZE)]
        public String ParamIdString { get; protected set; }

        [NotMapped]
        public CompetitionParamId ParamId
        {
            get
            {
                return ParamIdString.GetEnumValue<CompetitionParamId>() ?? default(CompetitionParamId);
            }
            set { this.ParamIdString = value.GetEnumValueString(); }
        }

        [Column("string_value")]
        public String StringValue { get; set; }

        [NotMapped]
        public DateTime? DateTimeValue
        {
            get { return StringValue.GetFormalizedDate(); }
            set { StringValue = value.GetFormalizedDateString(); }
        }

        [NotMapped]
        public Double NumericValue
        {
            get { return StringValue.GetNumericValue() ?? 0.0; }
            set { value.GetNumericValueString(); }
        }

        [Column("binary_value", TypeName = "varbinary(max)")]
        public byte[] BinaryValue { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.CompetitionParameters.Remove(this);
        }
    }
}
