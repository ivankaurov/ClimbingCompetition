// <copyright file="ListLineBoulder.cs">
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
using Extensions;
using DbAccessCore.Log;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_boulder")]
    public class ListLineBoulder : ListLine
    {
        protected ListLineBoulder() { }
        public ListLineBoulder(ClimbingContext2 context) : base(context) { this.ResultType = ResultType.RES; }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsBoulder.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            this.RemoveChildCollection(context, Routes, ltr);
        }

        [NotMapped]
        public ResultType ResultType { get; set; }
        [Column("result_type"), MaxLength(IID_SIZE)]
        public String ResultTypeStr
        {
            get { return ResultType.GetEnumValueString(); }
            set { this.ResultType = value.GetEnumValue<ResultType>() ?? ResultType.RES; }
        }

        public virtual ICollection<ListLineBoulderRoute> Routes { get; set; }

        [NotMapped]
        public int Tops
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Count(r => r.TopAttempt > 0);
            }
        }

        [NotMapped]
        public int TopAttempts
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Sum(r => r.TopAttempt > 0 ? r.TopAttempt : 0);
            }
        }

        [NotMapped]
        public int Bonuses
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Count(r => r.BonusAttempt > 0);
            }
        }

        [NotMapped]
        public int BonusAttempts
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Sum(r => r.BonusAttempt > 0 ? r.BonusAttempt : 0);
            }
        }

        [NotMapped]
        public ListLineBoulderRoute this[int route]
        {
            get
            {
                if (this.Routes == null)
                    return null;
                return this.Routes.FirstOrDefault(r => r.RouteNumber == route);
            }
        }

        protected override int CompareResult(ListLine other)
        {
            var br = (ListLineBoulder)other;
            int n = br.Tops.CompareTo(this.Tops);
            if (n != 0)
                return n;

            n = br.Bonuses.CompareTo(this.Bonuses);
            if (n != 0)
                return n;

            n = this.TopAttempts.CompareTo(br.TopAttempts);
            if (n != 0)
                return n;

            return this.BonusAttempts.CompareTo(br.BonusAttempts);
        }

        public override bool HasResult()
        {
            return this.Routes != null && this.Routes.Count > 0;
        }

        protected override bool IsNilResult()
        {
            return this.BonusAttempts == 0;
        }

        protected override bool IsDns()
        {
            return this.ResultType == ResultType.DNS;
        }

        protected override bool IsDsq()
        {
            return this.ResultType == ResultType.DSQ;
        }
    }
}
