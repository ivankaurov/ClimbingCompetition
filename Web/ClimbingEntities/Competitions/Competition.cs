// <copyright file="Competition.cs">
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
using DbAccessCore;
using DbAccessCore.Log;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_competition")]
    public partial class Competition : ClimbingBaseObject
    {
        protected Competition() { }

        public Competition(ClimbingContext2 context) : base(context) { }

        [Column("name"), MaxLength(TYPE_SIZE), Required]
        public String Name { get; set; }

        [Column("short_name"),MaxLength(50), Required]
        public String ShortName { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("organizer_id")]
        public String OrganizerId { get; set; }

        [ForeignKey("OrganizerId")]
        public virtual Teams.Team Organizer { get; set; }

        [NotMapped]
        public Teams.Team CompetitionZone { get { return Organizer.ParentTeam; } }

        [NotMapped]
        public String CompetitionZoneId { get { return CompetitionZone == null ? null : CompetitionZone.Iid; } }

        [NotMapped]
        public int CompetitionYear { get { return StartDate.Year; } }

        public virtual ICollection<ClimberOnCompetition> Competitors { get; set; }
        public virtual ICollection<AgeGroupOnCompetition> AgeGroups { get; set; }
        public virtual ICollection<Lists.ListHeader> ResultLists { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.Competitions.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (this.ResultLists != null)
                this.RemoveChildCollection(context, ResultLists.OrderByDescending(r => r.Iid), ltr);
            this.RemoveChildCollection(context, AgeGroups, ltr);
            this.RemoveChildCollection(context, Parameters, ltr);
        }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);

            if (result.HasValue)
                return result;

            var organizer = this.Organizer;
            if (organizer == null)
                organizer = ((ClimbingContext2)context).Teams.FirstOrDefault(t => t.Iid == this.OrganizerId);
            if (organizer == null)
                return null;
            if (organizer.ParentTeam == null)
                return context.UserIsAdmin(securityEntityID) ? new RightsEnum?(RightsEnum.Allow) : null;
            else
                return organizer.ParentTeam.GetRights(securityEntityID, action, context, out isInherited);
        }
    }
}
