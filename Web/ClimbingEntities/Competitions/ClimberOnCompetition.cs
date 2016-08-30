// <copyright file="ClimberOnCompetition.cs">
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
using ClimbingCompetition.Common;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_climbers")]
    public class ClimberOnCompetition : ClimbingBaseObject
    {
        protected ClimberOnCompetition() { }
        public ClimberOnCompetition(ClimbingContext2 context) : base(context)
        {
            this.Qf = ClimberQf.Empty;
            this.Styles = 0;
        }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ClimbersOnCompetition.Remove(this);
        }

        [Column("person_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_Person", IsUnique = true, Order = 0)]
        public String PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual People.Person Person { get; set; }

        [Column("comp_id"), MaxLength(IID_SIZE)]
        [Index("UX_Comp_Person", IsUnique = true, Order = 1)]
        [Index("UX_Comp_Bib", IsUnique = false, Order = 1)]
        [Index("UX_Comp_IntBib", IsUnique = false, Order = 1)]
        public String CompId { get; set; }
        [ForeignKey("CompId")]
        public virtual Competition Competition { get; set; }

        [Column("group_id"), MaxLength(IID_SIZE)]
        public String AgeGroupId { get; set; }

        [ForeignKey("AgeGroupId")]
        public virtual AgeGroupOnCompetition AgeGroup { get; set; }
        

        [NotMapped]
        public ClimberQf Qf { get; set; }

        [Column("qf"), MaxLength(IID_SIZE)]
        public String QfStr { get { return Qf.GetEnumValueString(); } set { Qf = value.GetEnumValue<ClimberQf>() ?? ClimberQf.Empty; } }

        [Column("bib")]
        [Index("UX_Comp_Bib", IsUnique = false, Order = 0)]
        [MaxLength(IID_SIZE)]
        public String Bib { get; set; }

        [Column("old_comp_id")]
        [Index("UX_Comp_IntBib", IsUnique = false, Order = 0)]
        public int? SecretaryBib { get; set; }

        [NotMapped]
        public String FriendlyQf { get { return Qf.EnumFriendlyValue(); } }

        [NotMapped]
        public ClimbingStyles Styles { get; set; }

        [Column("styles"), MaxLength(IID_SIZE)]
        protected String StylesStr
        {
            get { return Styles.GetSerializedValue(); }
            set { Styles = ClimbingStyleExtensions.ParseSerializedValue(value); }
        }

        [Column("vk")]
        public Boolean VK { get; set; }

        public virtual ICollection<ClimberTeamOnCompetition> Teams { get; set; }
        public virtual ICollection<Lists.ListLine> Results { get; set; }

        [NotMapped]
        public String Team
        {
            get
            {
                if (Teams == null)
                    return string.Empty;
                StringBuilder sb = new StringBuilder();
                foreach (var t in Teams.OrderBy(t => string.Format("{0:000} {1}", t.TeamOrder, t.Team.Name))
                                      .Select(t => t.Team.Name)
                                      .Distinct())
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(t);
                }
                return sb.ToString();
            }
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            RemoveChildCollection(context, Teams, ltr);
        }
    }
}