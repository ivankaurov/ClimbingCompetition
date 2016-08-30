// <copyright file="ApiListLine.cs">
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
    [XmlType("ResultLabel", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public enum ResultLabel : short
    {
        RES = 0,
        DNS = 1,
        DSQ = 2
    }

    [XmlType("ResultListData", IncludeInSchema = true, Namespace=XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineCollection :APIBaseRequest, IAPICollection<ApiListLine>
    {
        [XmlElement(ElementName = "ClimberResult")]
        public ApiListLine[] Data { get; set; }

        public ApiListLineCollection(IEnumerable<ApiListLine> data) { this.Data = XmlApiDataConstants.ToArray(data); }
        public ApiListLineCollection() { this.Data = new ApiListLine[0]; }
    }

    [XmlType("ResultListLine", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlInclude(typeof(ApiListLineLead))]
    [XmlInclude(typeof(ApiListLineSpeed))]
    [XmlInclude(typeof(ApiListLineBoulder))]
    public abstract class ApiListLine : APIBaseRequest
    {
        [XmlElement("ListID")]
        public int ListID { get; set; }

        [XmlElement("Climber")]
        public int ClimberID { get; set; }

        [XmlElement("StartNumber")]
        public int StartNumber { get; set; }

        [XmlElement("Result")]
        public long? Result { get; set; }

        [XmlElement("TextResult")]
        public String ResText { get; set; }

        [XmlElement("PreQf")]
        public bool PreQf { get; set; }

        [XmlIgnore]
        public int ResultID { get; set; }
    }

    [XmlType("ResultListLineLead", IncludeInSchema=true, Namespace=XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineLead : ApiListLine {
        [XmlElement("Time")]
        public int? Time { get; set; }

        [XmlElement("TimeText")]
        public String TimeText { get; set; }
    }

    [XmlType("ResultListLineSpeed", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineSpeed : ApiListLine
    {
        [XmlElement("Route1")]
        public long? Route1Data { get; set; }

        [XmlElement("Route1Text")]
        public String Route1Text { get; set; }

        [XmlElement("Route2")]
        public long? Route2Data { get; set; }
        [XmlElement("Route2Text")]
        public String Route2Text { get; set; }

        [XmlElement("Place")]
        public int? Pos { get; set; }

        private String posText = String.Empty;
        [XmlElement("PlaceText")]
        public String PosText { get { return posText; } set { posText = value ?? String.Empty; } }

        private String qf = String.Empty;
        [XmlElement("Qf")]
        public String Qf { get { return qf; } set { qf = (String.IsNullOrEmpty(value) ? String.Empty : value.Trim()); } }
    }

    [XmlType("ResultListLineBoudler", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiListLineBoulder: ApiListLine
    {
        [XmlElement("ResultCode")]
        public ResultLabel ResultCode { get; set; }

        [XmlArray("Routes")]
        [XmlArrayItem("Route")]
        public ApiBoulderResultRoute[] Routes { get; set; }

        public ApiListLineBoulder() { this.Routes = new ApiBoulderResultRoute[0]; }
    }

    [XmlType("ResultListLineBoudlerRoute", IncludeInSchema = true, Namespace = XmlApiDataConstants.NAMESPACE)]
    public sealed class ApiBoulderResultRoute
    {
        [XmlElement("RouteNumber")]
        public int Route { get; set; }

        [XmlElement("Top")]
        public int? Top { get; set; }

        [XmlElement("Bonus")]
        public int? Bonus { get; set; }
    }
}
