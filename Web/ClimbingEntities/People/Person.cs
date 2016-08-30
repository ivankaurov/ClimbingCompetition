// <copyright file="Person.cs">
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
using DbAccessCore.Log;
using ClimbingCompetition.Common;
using DbAccessCore;

namespace ClimbingEntities.People
{
    [Table("cl_pr_people")]
    public class Person : ClimbingBaseObject
    {
        protected Person() { }

        public Person(ClimbingContext2 context) : base(context)
        {
            this.Name = this.Patronymic = string.Empty;
            this.Gender = Gender.Male;
        }

        [Column("surname"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = false)]
        public String Surname { get; set; }

        [Column("name"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = true)]
        public String Name { get; set; }

        [Column("patronymic"), MaxLength(TYPE_SIZE), Required(AllowEmptyStrings = true)]
        public String Patronymic { get; set; }

        [NotMapped]
        public String FullName
        {
            get { return string.Format("{0} {1}", this.Surname, this.Name); }
        }

        [Column("gender"), MaxLength(1)]
        public String GenderChar { get; protected set; }

        [NotMapped]
        public Gender Gender
        {
            get { return GenderChar.GetByFirstLetter<Gender>(); }
            set { GenderChar = value.GetFirstLetter(); }
        }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int YearOfBirth { get { return DateOfBirth.Year; } }

        public DateTime SetDateOfBirthByYear(int year)
        {
            if (year < 0)
                throw new ArgumentOutOfRangeException("year");
            else if (year < 20)
                year += 2000;
            else if (year < 100)
                year += 1900;
            return this.DateOfBirth = new DateTime(year, 1, 2);
        }

        public virtual ICollection<Climber> ClimbersLicenses { get; set; }
        public virtual ICollection<Competitions.ClimberOnCompetition> CompetitionAppearances { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.People.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.CompetitionAppearances != null && this.CompetitionAppearances.Count > 0)
                throw new InvalidOperationException("This climber has appeared on several competitions");
            RemoveChildCollection(context, ClimbersLicenses, ltr);
        }
    }
}
