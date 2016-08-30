// <copyright file="ListLineSpeed.cs">
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
    [Table("cl_ls_list_lines_speed")]
    public class ListLineSpeed : ListLine
    {
        public const long FALL = long.MaxValue / 3,
                         DSQ = long.MaxValue / 3 + 100,
                         DNS = long.MaxValue / 3 + 500;
                         

        protected ListLineSpeed() { }
        public ListLineSpeed(ClimbingContext2 context) : base(context) { }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsSpeed.Remove(this);
        }

        [Column("route1")]
        public long Route1 { get; set; }

        [Column("route1_text"), MaxLength(IID_SIZE)]
        public String Route1Text { get; set; }

        [Column("route2")]
        public long Route2 { get; set; }

        [Column("route2_text"), MaxLength(IID_SIZE)]
        public String Route2Text { get; set; }

        protected override bool IsDns()
        {
            return DNS == this.Result;
        }

        protected override bool IsDsq()
        {
            return DSQ == this.Result;
        }

        protected override bool IsNilResult()
        {
            return FALL == this.Result;
        }

        public override bool HasResult()
        {
            return this.Result.HasValue && !string.IsNullOrEmpty(this.ResText);
        }

        [NotMapped]
        public bool Failed
        {
            get { return this.HasResult() && this.Result.Value >= FALL; }
        }
        
        public ListLineSpeed PreviousRoundResult()
        {
                if (this.Header == null)
                    return null;
                if (this.Header.PreviousRound == null)
                    return null;
                if (this.Header.PreviousRound.Results == null)
                    return null;
                return this.Header.PreviousRound.Results.OfType<ListLineSpeed>().FirstOrDefault(s => s.ClimberId == this.ClimberId);
        }

        [NotMapped]
        public ListLineSpeed BestRecord
        {
            get
            {
                var prevResult = this.PreviousRoundResult();
                if (prevResult == null)
                    return this;
                if ((prevResult.Result ?? long.MaxValue) < (this.Result ?? long.MaxValue))
                    return prevResult;
                return this;
            }
        }

        private long GetResult()
        {
            if (this.Header != null && this.Header.ListType == ClimbingCompetition.Common.ListType.SpeedQualy2 &&
                (this.Header.Rules & ClimbingCompetition.Common.Rules.BestRouteInQf) == ClimbingCompetition.Common.Rules.BestRouteInQf)
                return this.BestRecord.Result ?? 0;
            else
                return this.Result ?? 0;
        }

        public int CompareResult(ListLineSpeed other)
        {
            return this.GetResult().CompareTo(other.GetResult());
        }

        protected override int CompareResult(ListLine other)
        {
            return this.CompareResult((ListLineSpeed)other);
        }
    }
}
