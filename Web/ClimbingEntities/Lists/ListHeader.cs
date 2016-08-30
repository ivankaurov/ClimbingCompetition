// <copyright file="ListHeader.cs">
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
using Extensions;
using DbAccessCore;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_headers")]
    public partial class ListHeader : ClimbingBaseObject
    {
        protected ListHeader() { }
        public ListHeader(ClimbingContext2 context) : base(context)
        {
            this.ListType = ListType.Unknown;
            this.Style = ClimbingStyles.Lead;
            this.Rules = Rules.Russian;
        }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultLists.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.NextRounds != null)
            {
                foreach (var nr in this.NextRounds)
                {
                    nr.PreviousRound = null;
                    nr.PrevRoundIid = null;
                }
                this.NextRounds.Clear();
            }
            this.RemoveChildCollection(context, Children, ltr);
            this.RemoveChildCollection(context, Results, ltr);
        }

        [Column("comp_id"), MaxLength(IID_SIZE)]
        public String CompId { get; set; }
        [ForeignKey("CompId")]
        public virtual Competitions.Competition Competition { get; set; }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        [ForeignKey("Parent")]
        public String IidParent { get; set; }

        public virtual ListHeader Parent { get; set; }
        public virtual ICollection<ListHeader> Children { get; set; }

        [Column("prev_round"), MaxLength(IID_SIZE)]
        [ForeignKey("PreviousRound")]
        public String PrevRoundIid { get; set; }
        
        public virtual ListHeader PreviousRound { get; set; }
        public virtual ICollection<ListHeader> NextRounds { get; set; }
        [NotMapped]
        public ListHeader NextRound
        {
            get
            {
                if (NextRounds == null)
                    return null;
                return NextRounds.FirstOrDefault();
            }
        }

        [Column("age_group"), MaxLength(IID_SIZE)]
        public String GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Competitions.AgeGroupOnCompetition AgeGroup { get; set; }

        [NotMapped]
        public ListType ListType { get; set; }

        [Column("list_type"), MaxLength(2 * IID_SIZE)]
        public String ListTypeStr
        {
            get
            {
                return this.ListType.GetEnumValueString();
            }
            protected set { this.ListType = value.GetEnumValue<ListType>() ?? ListType.Unknown; }
        }

        [NotMapped]
        public ClimbingStyles Style { get; set; }
        [Column("style"), MaxLength(IID_SIZE)]
        public String StyleStr
        {
            get { return this.Style.GetEnumValueString(); }
            set { this.Style = value.GetEnumValue<ClimbingStyles>() ?? ClimbingStyles.Lead; }
        }
        [NotMapped]
        public String StyleFriendlyName { get { return this.Style.EnumFriendlyValue(); } }

        [Column("round")]
        public ClimbingRound Round { get; set; }

        [Column("routeNum")]
        public int RouteNumber { get; set; }

        [NotMapped]
        public String RoundName { get { return Round.GetLocalizedValue(this.RouteNumber); } }

        [Column("live")]
        public Boolean Live { get; set; }

        [Column("rules")]
        public Rules Rules { get; set; }

        [NotMapped]
        public Boolean BestRouteInQf
        {
            get { return ((Rules & Rules.BestRouteInQf) == Rules.BestRouteInQf); }
            set
            {
                if (value)
                    Rules = (Rules | Rules.BestRouteInQf);
                else
                    Rules = Rules & (~Rules.BestRouteInQf);
            }
        }

        [Column("quota")]
        public int Quota { get; set; }

        public virtual ICollection<ListLine> Results { get; set; }
    }
}
