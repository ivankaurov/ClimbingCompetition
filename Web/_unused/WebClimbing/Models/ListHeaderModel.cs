// <copyright file="ListHeaderModel.cs">
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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using XmlApiData;

namespace WebClimbing.Models
{
    [Table("MVCListHeaders")]
    public class ListHeaderModel
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("comp_id"), ForeignKey("Competition")]
        public long CompId { get; set; }
        public virtual CompetitionModel Competition { get; set; }

        [Column("secretary_id")]
        public int LocalIid { get; set; }

        [Column("iid_parent")]
        public int? IidParent { get; set; }
        [NotMapped]
        public ListHeaderModel Parent
        {
            get
            {
                if (IidParent == null)
                    return null;
                return this.Competition.Lists.FirstOrDefault(l => l.LocalIid == this.IidParent.Value);
            }
            set
            {
                if (value == null)
                    this.IidParent = null;
                else
                    this.IidParent = value.LocalIid;
            }
        }

        [NotMapped]
        public IEnumerable<ListHeaderModel> Children
        {
            get
            {
                return this.Competition.Lists.Where(l => l.IidParent == this.LocalIid);
            }
        }

        [NotMapped]
        public IEnumerable<ListHeaderModel> NextRounds
        {
            get
            {
                return this.Competition.Lists.Where(l => l.PreviousRoundId == this.LocalIid);
            }
        }

        [Column("prev_round")]
        public int? PreviousRoundId { get; set; }
        [NotMapped]
        public ListHeaderModel PreviousRound
        {
            get
            {
                if (PreviousRoundId == null)
                    return null;
                return this.Competition.Lists.FirstOrDefault(l => l.LocalIid == this.PreviousRoundId.Value);
            }
            set
            {
                if (value == null)
                    this.PreviousRoundId = null;
                else
                    this.PreviousRoundId = value.LocalIid;
            }
        }

        [Column("group_id"), ForeignKey("Group")]
        public long? GroupId { get; set; }
        public virtual Comp_AgeGroupModel Group { get; set; }

        [Column("list_type")]
        public String ListTypeCode { get; set; }
        [NotMapped]
        public ListTypeEnum ListType
        {
            get { return (ListTypeEnum)Enum.Parse(typeof(ListTypeEnum), ListTypeCode, true); }
            set { ListTypeCode = value.ToString(); }
        }

        [Column("style")]
        public String Style { get; set; }

        [Column("round_name")]
        public String Round { get; set; }

        [Column("quota")]
        public int Quota { get; set; }

        [Column("route_num")]
        public int? RouteQuantity { get; set; }

        [Column("live")]
        public bool Live { get; set; }

        [Column("start_time")]
        public String StartTime { get; set; }

        [Column("last_update")]
        public DateTime LastRefresh { get; set; }

        [Column("CompetitionRules")]
        public int CompetitionRulesCode { get; set; }

        [Column("BestResultInQf")]
        public bool BestResultInQf { get; set; }

        [NotMapped]
        public CompetitionRules CompetitionRules
        {
            get { return (CompetitionRules)this.CompetitionRulesCode; }
            set { this.CompetitionRulesCode = (int)value; }
        }

        [NotMapped]
        public bool SecondQfWithBestFirst
        {
            get
            {
                if (this.ListType == ListTypeEnum.SpeedQualy2 && this.BestResultInQf)
                {
                    return (this.PreviousRoundId != null && this.PreviousRound != null);
                }
                else
                    return false;
            }
        }

        public virtual ICollection<ListLineModel> Results { get; set; }

        private IEnumerable<T> GetResults<T>() where T : ListLineModel
        {
            if (this.Results == null)
                return new T[0];
            return this.Results.OfType<T>();
        }

        [NotMapped]
        public IEnumerable<LeadResultLine> ResultsLead { get { return GetResults<LeadResultLine>(); } }

        [NotMapped]
        public IEnumerable<SpeedResultLine> ResultsSpeed { get { return GetResults<SpeedResultLine>(); } }

        [NotMapped]
        public IEnumerable<BoulderResultLine> ResultsBoulder { get { return GetResults<BoulderResultLine>(); } }
    }
}