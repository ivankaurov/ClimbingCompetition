// <copyright file="AgeGroup.cs">
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
using System.Xml.Serialization;

namespace XmlApiData
{
    [XmlRoot("Groups", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("AgeGroupCollection", IncludeInSchema = true)]
    public sealed class API_AgeGroupCollection : APIBaseRequest, IAPICollection<Comp_AgeGroupApiModel>
    {
        [XmlElement("AgeGroup")]
        public Comp_AgeGroupApiModel[] Data { get; set; }

        public API_AgeGroupCollection() { this.Data = new Comp_AgeGroupApiModel[0]; }
        public API_AgeGroupCollection(IEnumerable<Comp_AgeGroupApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlType("AgeGroup", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class Comp_AgeGroupApiModel : APIBaseRequest
    {
        [XmlElement(ElementName = "Iid")]
        public int Iid { get; set; }

        [XmlElement(ElementName = "Name")]
        public String Name { get; set; }

        [XmlElement(ElementName = "YearOld")]
        public int YearOld { get; set; }

        [XmlElement(ElementName = "YearYoung")]
        public int YearYoung { get; set; }

        [XmlElement(ElementName = "Female")]
        public bool Female { get; set; }

        public Comp_AgeGroupApiModel()
        {
            this.Iid = this.YearOld = this.YearYoung = 0;
            this.Female = false;
            this.Name = String.Empty;
        }
    }
}