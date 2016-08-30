// <copyright file="ListLine.cs">
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

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines")]
    public abstract class ListLine : ClimbingBaseObject, IListLine
    {
        protected ListLine() { }
        public ListLine(ClimbingContext2 context) : base(context) { }

        [Column("list_id"), MaxLength(IID_SIZE)]
        [Index("UX_ListLine", IsUnique = true, Order = 1)]
        public String ListId { get; set; }
        [ForeignKey("ListId")]
        public virtual ListHeader Header { get; set; }

        [Column("climber_id"), MaxLength(IID_SIZE)]
        [Index("UX_ListLine", IsUnique = true, Order = 2)]
        public String ClimberId { get; set; }
        [ForeignKey("ClimberId")]
        public virtual Competitions.ClimberOnCompetition Climber { get; set; }

        [Column("start_num")]
        public int Start { get; set; }

        [Column("pos")]
        public int Pos { get; set; }

        [Column("pos_text"), MaxLength(IID_SIZE)]
        public String PosText { get; set; }

        [Column("result")]
        public long? Result { get; set; }

        [Column("res_text"), MaxLength(IID_SIZE)]
        public String ResText { get; set; }

        [Column("points")]
        public Double? Points { get; set; }

        [Column("pts_text"), MaxLength(IID_SIZE)]
        public String PtsText { get; set; }

        [NotMapped]
        public NextRoundQf Qf { get; set; }

        [Column("qf"), MaxLength(IID_SIZE)]
        public String QfStr
        {
            get { return this.Qf.GetEnumValueString(); }
            set { this.Qf = value.GetEnumValue<NextRoundQf>() ?? NextRoundQf.NotQf; }
        }

        [Column("pre_qf")]
        public Boolean PreQf { get; set; }
        

        public int CompareTo(IListLine o, bool hasPrevRound)
        {
            var other = o as ListLine;
            if (other == null)
                throw new ArgumentException("o");

            var thisRT = this.SortRT(hasPrevRound);
            var otherRT = other.SortRT(hasPrevRound);

            int n = thisRT.CompareTo(otherRT);
            if (n != 0)
                return n;
            if (this.HasResult())
            {
                n = this.CompareResult(other);
                if (n != 0)
                    return n;
                n = this.Climber.VK.CompareTo(other.Climber.VK);
                if (n != 0)
                    return n;
                n = this.Climber.Team.CompareTo(other.Climber.Team);
                if (n != 0)
                    return n;
                n = this.Climber.Person.FullName.CompareTo(other.Climber.Person.FullName);
                if (n != 0)
                    return n;
            }
            return this.Start.CompareTo(other.Start);
        }

        private int SortRT(bool hasPrevRound)
        {
            if (this.HasResult())
            {
                if (this.IsDsq())
                    return 15;
                if (this.IsDns())
                    return hasPrevRound ? 15 : 16;
                return 10;
            }
            return 25;
        }

        protected abstract int CompareResult(ListLine other);

        public abstract bool HasResult();

        public bool NilResult() { return this.HasResult() ? this.IsNilResult() : false; }

        protected virtual bool IsNilResult() { return false; }

        [NotMapped]
        public bool Dns { get { return this.HasResult() ? this.IsDns() : false; } }

        [NotMapped]
        public bool Dsq { get { return this.HasResult() ? this.IsDns() : false; } }

        public bool EqualResults(IListLine o)
        {
            var other = o as ListLine;
            if (other == null)
                throw new ArgumentException("o");
            if (!(this.HasResult() && other.HasResult()))
                return false;
            return this.CompareResult(other) == 0;
        }

        protected abstract bool IsDsq();
        protected abstract bool IsDns();
    }
}
