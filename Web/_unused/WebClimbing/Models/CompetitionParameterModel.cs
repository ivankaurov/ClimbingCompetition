// <copyright file="CompetitionParameterModel.cs">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace WebClimbing.Models
{

    public enum CompetitionParamType
    {
        SignatureKey,
        SignApplications,
        AllowMultipleTeams
    }

    [Table("MVCCompetitionParams")]
    public class CompetitionParameterModel
    {
        private static CultureInfo DefaultCulture = new CultureInfo("en-US");

        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("param_name"), Display(Name = "Параметр"), Required(AllowEmptyStrings = false), MaxLength(50)]
        public String Name { get; set; }

        public CompetitionParamType ParamType
        {
            get { return (CompetitionParamType)Enum.Parse(typeof(CompetitionParamType), Name, true); }
            set { Name = value.ToString("G"); }
        }

        [Required, ForeignKey("Competition"), Column("comp_id")]
        public long CompetitionIid { get; set; }

        public virtual CompetitionModel Competition { get; set; }

        [MaxLength(4000, ErrorMessage = "Разрешено не более 8000 символов"), Column("value")]
        public string StringValue { get; set; }

        [Column("binary_value", TypeName="image")]
        public byte[] BinaryValue { get; set; }

        [NotMapped]
        public long Int64Value
        {
            get { return Int64.Parse(StringValue); }
            set { StringValue = value.ToString(); }
        }

        [NotMapped]
        public bool BooleanValue
        {
            get { return String.IsNullOrEmpty(StringValue) ? false : !StringValue.Equals("0", StringComparison.Ordinal); }
            set { StringValue = (value ? "1" : String.Empty); }
        }

        [NotMapped]
        public int Int32Value
        {
            get { unchecked { return (int)Int64Value; } }
            set { Int64Value = value; }
        }

        [NotMapped]
        public DateTime DateTimeValue
        {
            get { return DateTime.Parse(StringValue, DefaultCulture); }
            set { StringValue = value.ToString(DefaultCulture); }
        }

        [NotMapped]
        public DateTime DateValue
        {
            get { return DateTimeValue.Date; }
            set { DateTimeValue = value; }
        }

        [NotMapped]
        public double DoubleValue
        {
            get { return double.Parse(StringValue, DefaultCulture); }
            set { StringValue = value.ToString(DefaultCulture); }
        }
    }
}