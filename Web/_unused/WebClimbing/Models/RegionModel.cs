// <copyright file="RegionModel.cs">
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
using WebClimbing.Models.UserAuthentication;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using XmlApiData;

namespace WebClimbing.Models
{

    [Table("MVCRegions"), DisplayColumn("Name")]
    public class RegionModel : IComparable<RegionModel>
    {
        public const int CODE_LENGTH = 50;
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("sym_code"), Required(AllowEmptyStrings = false), StringLength(CODE_LENGTH, MinimumLength = 1)]
        public String SymCode { get; set; }

        [Column("iid_parent"), ForeignKey("Parent")]
        public long? IidParent { get; set; }

        [Display(Name = "Регион - владелец")]
        public virtual RegionModel Parent { get; set; }

        [Column("name"), Required(AllowEmptyStrings = false), MaxLength(255), Display(Name = "Наименование")]
        public string Name { get; set; }

        public virtual ICollection<CompetitionModel> CompetitionsHold { get; set; }

        public virtual ICollection<Comp_ClimberTeam> PeopleCompetitions { get; set; }

        public virtual ICollection<UserProfileModel> Users { get; set; }

        public virtual ICollection<UserRoleModel> UserRoles { get; set; }

        public virtual ICollection<RegionModel> Children { get; set; }

        public PersonModel[] Climbers
        {
            get
            {
                if (PeopleCompetitions == null)
                    return new PersonModel[0];
                var lst = PeopleCompetitions
                           .Select(pc => pc.Climber)
                           .Select(clm => clm.Person)
                           .Distinct()
                           .ToList();
                lst.Sort();
                return lst.ToArray();
            }
        }

        public override bool Equals(object obj)
        {
            RegionModel other = obj as RegionModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }

        public int CompareTo(RegionModel other)
        {
            if (other == null)
                return 1;
            if (this.Parent == null && other.Parent != null)
                return -1;
            if (this.Parent != null && other.Parent == null)
                return 1;
            if (this.Parent != null && other.Parent != null)
            {
                int n = this.Parent.CompareTo(other.Parent);
                if (n != 0)
                    return n;
            }
            return this.Name.CompareTo(other.Name);
        }
    }
}