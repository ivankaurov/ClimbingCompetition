// <copyright file="Comp_CompetitiorRegistrationModel.cs">
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
using WebClimbing.ServiceClasses;
using System.Xml.Serialization;
using XmlApiData;
using System.Text;
using WebClimbing.Models.UserAuthentication;

namespace WebClimbing.Models
{
    [EnumCustomDisplay]
    public enum Razryad
    {
        [Display(Name="ЗМС")]
        HMS = 0,
        [Display(Name = "МСМК")]
        MSIC = 1,
        [Display(Name = "МС")]
        MS = 2,
        [Display(Name = "КМС")]
        CMS = 3,
        [Display(Name = "1")]
        First = 4,
        [Display(Name = "2")]
        Second = 5,
        [Display(Name = "3")]
        Third = 6,
        [Display(Name = "1ю")]
        FirstYouth = 7,
        [Display(Name = "2ю")]
        SecondYouth = 8,
        [Display(Name = "3ю")]
        ThirdYouth = 9,
        [Display(Name = "б/р")]
        No = 10,
        [Display(Name = "")]
        Empty = 11
    }

    [EnumCustomDisplay]
    public enum ApplicationDisplayEnum : short
    {
        [Display(Name="-")]
        NotStarter = 0,
        [Display(Name="+")]
        Starter = 1,
        [Display(Name="Лично")]
        NoPoints = 2
    }

    [Table("MVC_Comp_Competitiors")]
    public class Comp_CompetitiorRegistrationModel
    {
        public Comp_CompetitiorRegistrationModel()
        {
            this.Qf = Razryad.Empty;
        }

        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [ForeignKey("Competition"), Display(Name = "Соревнование"), Column("comp_id")]
        public long CompId { get; set; }
        public virtual CompetitionModel Competition { get; set; }

        [ForeignKey("Person"), Display(Name = "Участник"), Column("person_id")]
        public long PersonId { get; set; }
        public virtual PersonModel Person { get; set; }

        [Column("secretary_id"), Display(Name = "Инд.№")]
        public int? SecretaryId { get; set; }

        [Column("qf")]
        public int QfCode { get; set; }

        [NotMapped, Display(Name = "Разряд")]
        public Razryad Qf { get { return (Razryad)QfCode; } set { QfCode = (int)value; } }

        [ForeignKey("CompAgeGroup"), Column("group_id"), Required(ErrorMessage = "Группа не указана")]
        public long GroupId { get; set; }
        [Display(Name="Группа")]
        public virtual Comp_AgeGroupModel CompAgeGroup { get; set; }

        [Column("lead")]
        public short LeadN { get; set; }
        [NotMapped, Display(Name="Трудность")]
        public ApplicationDisplayEnum Lead
        {
            get { return (ApplicationDisplayEnum)this.LeadN; }
            set { this.LeadN = (short)value; }
        }

        [Column("speed")]
        public short SpeedN { get; set; }
        [NotMapped, Display(Name = "Скорость")]
        public ApplicationDisplayEnum Speed
        {
            get { return (ApplicationDisplayEnum)this.SpeedN; }
            set { this.SpeedN = (short)value; }
        }

        [Column("boulder")]
        public short BoulderN { get; set; }
        [NotMapped, Display(Name = "Боулдеринг")]
        public ApplicationDisplayEnum Boulder
        {
            get { return (ApplicationDisplayEnum)this.BoulderN; }
            set { this.BoulderN = (short)value; }
        }
        
        
        [Column("ranking_lead"), Display(Name="Рейтинг (Тр.)")]
        public int? RankingLead { get; set; }

        [Column("ranking_speed"), Display(Name = "Рейтинг (Тр.)")]
        public int? RankingiSpeed { get; set; }

        [Column("ranking_boudler"), Display(Name = "Рейтинг (Тр.)")]
        public int? RankingBoulder { get; set; }

        [Column("vk"), Display(Name = "в/к")]
        public bool Vk { get; set; }
        
        public virtual ICollection<Comp_ClimberTeam> Teams { get; set; }

        public virtual ICollection<ListLineModel> Results { get; set; }

        [NotMapped, Display(Name="Команда")]
        public String TeamList
        {
            get
            {
                if (Teams == null)
                    return String.Empty;
                var tList = Teams.Select(t => new { Order = t.RegionOrder, Team = t.Region }).ToList();
                tList.Sort((a, b) =>
                {
                    int n = a.Order.CompareTo(b.Order);
                    if (n != 0)
                        return n;
                    return a.Team.CompareTo(b.Team);
                });
                StringBuilder sb = new StringBuilder();
                foreach (var t in tList)
                {
                    if (sb.Length > 0)
                        sb.Append("-");
                    sb.Append(t.Team.Name);
                }
                return sb.ToString();
            }
        }

        [NotMapped, Display(Name = "Фамилия, Имя")]
        public String ClimberName { get { return String.Format("{0} {1}", this.Person.Surname, this.Person.Name); } }

        [NotMapped, Display(Name = "Г.р.")]
        public int Age { get { return this.Person.YearOfBirth; } }
    }

    [Table("MVC_Comp_Teams")]
    public class Comp_ClimberTeam
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("climber_id"), ForeignKey("Climber")]
        public long ClimberRegId { get; set; }
        public virtual Comp_CompetitiorRegistrationModel Climber { get; set; }

        [Column("region_id"), ForeignKey("Region")]
        public long RegionId { get; set; }
        public virtual RegionModel Region { get; set; }

        [Column("region_order")]
        public int RegionOrder { get; set; }

        public bool AllowedEdit(UserProfileModel user, DateTime? dateTime = null)
        {
            if (!this.Climber.Competition.AllowedToEdit(user, dateTime))
                return false;
            if (user.IsInRoleComp(RoleEnum.Admin, this.Climber.Competition))
                return true;
            return (user.RegionId != null && user.RegionId.Value == this.RegionId);
        }
    }
}