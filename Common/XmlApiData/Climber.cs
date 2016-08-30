// <copyright file="Climber.cs">
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
    public enum ApplicationType : short
    {
        NotStart = 0,
        Start = 1,
        NoPoints = 2
    }

    [XmlRoot("Climbers", Namespace = XmlApiDataConstants.NAMESPACE)]
    [XmlType("ClimbersCollection", IncludeInSchema = true)]
    public sealed class API_ClimbersCollection : APIBaseRequest, IAPICollection<Comp_CompetitorRegistrationApiModel>
    {
        [XmlElement("Climber")]
        public Comp_CompetitorRegistrationApiModel[] Data { get; set; }
        public API_ClimbersCollection() { }
        public API_ClimbersCollection(IEnumerable<Comp_CompetitorRegistrationApiModel> data) { this.Data = XmlApiDataConstants.ToArray(data); }
    }

    [XmlInclude(typeof(Comp_MultipleTeamsClimber))]
    [XmlType("Climber", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public class Comp_CompetitorRegistrationApiModel : APIBaseRequest
    {
        [XmlElement("License")]
        public long License { get; set; }

        [XmlElement("Bib", IsNullable = true)]
        public int? Bib { get; set; }

        [XmlElement("Surname")]
        public String Surname { get; set; }

        [XmlElement("Name")]
        public String Name { get; set; }

        [XmlElement("Female")]
        public bool Female { get; set; }

        [XmlElement("TeamID")]
        public long TeamID { get; set; }

        [XmlElement("GroupID")]
        public int GroupID { get; set; }

        [XmlElement("YearOfBirth")]
        public int YearOfBirth { get; set; }

        [XmlElement("Razryad")]
        public String Razr { get; set; }

        [XmlIgnore]
        public ApplicationType Lead
        {
            get { return (ApplicationType)LeadN; }
            set { LeadN = (short)value; }
        }
        [XmlIgnore]
        public ApplicationType Speed
        {
            get { return (ApplicationType)SpeedN; }
            set { SpeedN = (short)value; }
        }
        [XmlIgnore]
        public ApplicationType Boulder
        {
            get { return (ApplicationType)BoulderN; }
            set { BoulderN = (short)value; }
        }

        [XmlElement("Lead")]
        public short LeadN { get; set; }

        [XmlElement("Speed")]
        public short SpeedN { get; set; }

        [XmlElement("Boulder")]
        public short BoulderN { get; set; }

        [XmlElement("RankingLead", IsNullable = true)]
        public int? RankingLead { get; set; }

        [XmlElement("RankingSpeed", IsNullable = true)]
        public int? RankingSpeed { get; set; }

        [XmlElement("RankingBoulder", IsNullable = true)]
        public int? RankingBoulder { get; set; }

        public Comp_CompetitorRegistrationApiModel()
        {
            this.License = this.TeamID = this.YearOfBirth = 0;
            this.Female = false;
            this.Surname = this.Name = this.Razr = String.Empty;
            this.Bib = null;
        }

    }

    [Serializable]
    [XmlType("MultipleTeamsClimber", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    public sealed class Comp_MultipleTeamsClimber : Comp_CompetitorRegistrationApiModel
    {
        [XmlArray(ElementName = "Teams")]
        [XmlArrayItem(ElementName = "Team")]
        public long[] Teams { get; set; }
    }

    [Serializable]
    [XmlType("ClimberPicture", Namespace = XmlApiDataConstants.NAMESPACE, IncludeInSchema = true)]
    [XmlRoot("ClimberPicture")]
    public sealed class Comp_ClimberPicture
    {
        [XmlElement("License")]
        public long ClimberId { get; set; }

        [XmlElement("Picture")]
        public byte[] Picture { get; set; }

        [XmlElement("PictureDate")]
        public DateTime PictureDate { get; set; }
    }
}