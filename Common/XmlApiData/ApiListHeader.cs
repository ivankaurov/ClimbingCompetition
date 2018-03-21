// <copyright file="ApiListHeader.cs">
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
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlType("ListTypes", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public enum ListTypeEnum
    {
        SpeedQualy, SpeedQualy2, SpeedFinal,
        LeadFlash, LeadGroups, LeadSimple,
        BoulderGroups, BoulderSimple, BoulderSuper,
        TeamLead, TeamSpeed, TeamBoulder, TeamGeneral,
        Combined,
        General, Unknown, OlympicCombined
    }

    [XmlType("ListHeaderCollection", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListHeaderCollection : APIBaseRequest, IAPICollection<ApiListHeader>
    {
        [XmlElement(ElementName = "List")]
        public  ApiListHeader[] Data { get; set; }
        public ApiListHeaderCollection() { this.Data = new ApiListHeader[0]; }
        public ApiListHeaderCollection(IEnumerable<ApiListHeader> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("ListHeader", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class ApiListHeader : APIBaseRequest
    {
        [XmlElement("Id")]
        public int Iid { get; set; }

        [XmlElement("ParentList")]
        public int? ParentList { get; set; }

        [XmlElement("PreviousRound")]
        public int? PreviousRound { get; set; }

        [XmlElement("GroupId")]
        public int? GroupId { get; set; }

        [XmlElement("Type")]
        public ListTypeEnum ListType { get; set; }

        [XmlElement("Discipline")]
        public String Style { get; set; }

        [XmlElement("RoundName")]
        public String Round { get; set; }

        [XmlElement("Quota")]
        public int Quota { get; set; }

        [XmlElement("RouteQuantity")]
        public int? RouteQuantity { get; set; }

        [XmlElement("CurrentlyOn")]
        public bool Live { get; set; }

        [XmlElement("StartTime")]
        public String StartTime { get; set; }

        [XmlElement("LastRefresh")]
        public DateTime LastRefresh { get; set; }

        [XmlElement("CompetitionRules")]
        public CompetitionRules Rules { get; set; }

        [XmlElement("BestResultInQf")]
        public bool BestQf { get; set; }
    }
}