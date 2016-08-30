// <copyright file="CompetitionDeleteModel.cs">
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
using System.Web;
using WebClimbing.Models;
using System.ComponentModel.DataAnnotations;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class CompetitionDeleteModel : IValidatableObject
    {
        public long Iid { get; set; }

        [Display(Name="Наименование")]
        public String Name { get; set; }

        [Display(Name="Дата начала")]
        public DateTime From { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime To { get; set; }

        [Display(Name = "Регион проведения")]
        public String Region { get; set; }

        public CompetitionDeleteModel() { }

        public CompetitionDeleteModel(CompetitionModel model)
        {
            this.Iid = model.Iid;
            this.Name = model.Name;
            this.From = model.Start;
            this.To = model.End;
            this.Region = model.Region.Name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ClimbingContext db = new ClimbingContext();
            var comp = db.Competitions.Find(this.Iid);
            if (comp == null)
                yield return new ValidationResult(String.Format("Invalid ID={0}", Iid));
            else
            {
                if (comp.Climbers.Count > 0)
                    yield return new ValidationResult("На соревнования есть заявленные участнки. Сначала удалите их");
            }
        }
    }
}