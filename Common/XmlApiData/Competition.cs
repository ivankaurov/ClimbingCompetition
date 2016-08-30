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
using System.Text;
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlType("CompetitionRules", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public enum CompetitionRules
    {
        Russian, International
    }

    [XmlRoot("Competitions", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("CompetitionCollection", IncludeInSchema = true)]
    public sealed class API_CompetitionCollection :APIBaseRequest, IAPICollection<CompetitionApiModel>
    {
        [XmlElement("Competition")]
        public CompetitionApiModel[] Data { get; set; }

        public API_CompetitionCollection() { this.Data = new CompetitionApiModel[0]; }
        public API_CompetitionCollection(IEnumerable<CompetitionApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("Competition", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class CompetitionApiModel : APIBaseRequest
    {
        [XmlElement("Iid")]
        public long Iid { get; set; }

        [XmlElement("FullName")]
        public String FullName { get; set; }

        [XmlElement("ShortName")]
        public String ShortName { get; set; }

        [XmlElement("StartDate")]
        public DateTime DateStart { get; set; }

        [XmlElement("EndDate")]
        public DateTime DateEnd { get; set; }

        [XmlElement("Rules")]
        public CompetitionRules Rules { get; set; }

        public override string ToString()
        {
            return this.FullName;
        }

        public CompetitionApiModel()
        {
            this.Iid = 0;
            this.DateStart = this.DateEnd = DateTime.MinValue;
            this.FullName = this.ShortName = String.Empty;
            this.Rules = CompetitionRules.International;
        }
    }
}
