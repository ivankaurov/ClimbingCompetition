// <copyright file="Team.cs">
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

﻿using DbAccessCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;

namespace ClimbingEntities.Teams
{
    [Table("cl_tm_teams")]
    public class Team : ClimbingBaseObject
    {
        protected Team() { }

        public Team(ClimbingContext2 context) : base(context) { }
        
        [Column("code")]
        [Index("UX_Team", Order = 0, IsUnique = true)]
        public int Code { get; set; }

        [Column("iid_parent")]
        [MaxLength(IID_SIZE)]
        [Index("UX_Team", Order = 1, IsUnique = true)]
        [Index("UX_TeamName", Order = 1, IsUnique = true)]
        public string IidParent { get; set; }

        [ForeignKey("IidParent")]
        public virtual Team ParentTeam { get; set; }

        public virtual ICollection<Team> ChildTeams { get; set; }

        public virtual ICollection<UserWithTeam> TeamMainUsers { get; set; }

        [Column("name")]
        [Required(AllowEmptyStrings =false)]
        [MaxLength(TYPE_SIZE)]
        [Index("UX_TeamName", Order = 0, IsUnique = true)]
        public String Name { get; set; }

        [Column("full_code")]
        [MaxLength(TYPE_SIZE)]
        [Required(AllowEmptyStrings = false)]
        [Index("UX_Team_FullCode", IsUnique = true)]
        public String FullCode { get; protected set; }

        public String CalculateFullCode(String formatString = "000", String separator = "-")
        {
            var teamList = new LinkedList<Team>();
            teamList.AddFirst(this);
            while (teamList.First.Value.ParentTeam != null)
                teamList.AddFirst(teamList.First.Value.ParentTeam);
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(formatString))
                formatString = string.Format("{{0:{0}}}", formatString);
            foreach (var team in teamList)
            {
                if (result.Length > 0)
                    result.Append(separator);
                if (String.IsNullOrEmpty(formatString))
                    result.Append(team.Code);
                else
                    result.AppendFormat(formatString, team.Code);
            }

            return this.FullCode = result.ToString();
        }

        public int GetNextCode(ClimbingContext2 context)
        {
            Team parentTeam = this.ParentTeam;
            if (parentTeam == null && this.IidParent != null)
                parentTeam = context.Teams.FirstOrDefault(t => t.Iid.Equals(this.IidParent, StringComparison.Ordinal));

            int result;
            IEnumerable<Team> enumer;
            if (parentTeam == null)
                enumer = context.Teams.Where(t => t.IidParent == null);
            else
                enumer = parentTeam.ChildTeams;

            result = (enumer == null || enumer.Count() < 1) ? 1 : (enumer.Max(t => t.Code) + 1);

            this.Code = result;
            CalculateFullCode();
            return result;
        }

        public virtual ICollection<Competitions.Competition> OrganizedCompetitions { get; set; }
        public virtual ICollection<People.Climber> TeamClimbers { get; set; }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            if (OrganizedCompetitions != null && OrganizedCompetitions.Count > 0)
                throw new InvalidOperationException("Can\'t remove team. There\'re one or more competitions organized by this team");
            RemoveChildCollection(context, TeamClimbers, ltr);
            RemoveChildCollection(context, ChildTeams, ltr);

            if (this.TeamMainUsers != null)
            {
                foreach (var u in this.TeamMainUsers)
                {
                    if (ltr != null)
                        ltr.AddUpdatedObjectBefore(u, context);
                    u.Team = null;
                    u.TeamId = null;
                    if (ltr != null)
                        ltr.AddUpatedObjectAfter(u, context);
                }

                this.TeamMainUsers.Clear();
            }
        }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.Teams.Remove(this);
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", FullCode, Name);
        }
    }
}
