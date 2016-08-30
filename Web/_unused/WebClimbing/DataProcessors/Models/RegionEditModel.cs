// <copyright file="RegionEditModel.cs">
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
using System.ComponentModel.DataAnnotations;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class RegionEditModel : IValidatableObject
    {
        public long? Iid { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(255), Display(Name = "Наименование")]
        public String Name { get; set; }

        [Required(AllowEmptyStrings=false)]
        [StringLength(RegionModel.CODE_LENGTH, MinimumLength=1)]
        [Display(Name="Код региона")]
        [RegularExpression("[0-9]*", ErrorMessage="Введите цифровой код")]
        public String SymCode { get; set; }

        [Display(Name = "Регион-родитель")]
        public long? IidParent { get; set; }

        public String PrefixCode { get; set; }

        public RegionEditModel()
        {
            Iid = null;
            SymCode = String.Empty;
            Name = String.Empty;
            PrefixCode = String.Empty;
            IidParent = null;
        }

        [Display(Name="Пользователи")]
        public List<RegionAdminModel> Users { get; set; }

        public RegionEditModel(RegionModel model)
        {
            this.Iid = model.Iid;
            this.IidParent = model.IidParent;
            this.PrefixCode = model.IidParent == null ? String.Empty : model.Parent.SymCode;
            this.Name = model.Name;
            if (String.IsNullOrEmpty(PrefixCode) || !model.SymCode.StartsWith(PrefixCode))
                this.SymCode = model.SymCode;
            else
                this.SymCode = model.SymCode.Substring(PrefixCode.Length);
        }

        public void LoadUsers(ClimbingContext db)
        {
            this.Users = new List<RegionAdminModel>();
            if (this.Iid != null)
            {
                //Админы региона
                Users.AddRange(
                    db.UserRoles
                      .Where(r => r.RegionID == this.Iid
                               && r.CompID == null
                               && r.RoleId >= (int)RoleEnum.Admin)
                      .Select(r => r.User)
                      .ToList()
                      .Distinct()
                      .Select(u => new RegionAdminModel
                      {
                          UserId = u.Iid,
                          UserName = u.Name,
                          UserEmail = u.Email ?? String.Empty,
                          UserRegion = (u.RegionId == null) ? String.Empty : u.Region.Name,
                          IsAdmin = true
                      }));

                //пользователи региона
                Users.AddRange(
                    db.UserProfiles
                      .Where(u => u.RegionId == this.Iid)
                      .ToList()
                      .Where(u => Users.Count(au => au.UserId == u.Iid) < 1)
                      .Select(u => new RegionAdminModel
                      {
                          UserId = u.Iid,
                          UserName = u.Name,
                          UserEmail = u.Email ?? String.Empty,
                          UserRegion = u.Region.Name,
                          IsAdmin = false
                      }));
            }

            this.Users.AddRange(
                db.UserRoles
                  .Where(r => (r.RegionID ?? 0) == (IidParent ?? 0)
                          && r.CompID == null
                          && r.RoleId >= (int)RoleEnum.Admin)
                  .Select(r => r.User)
                  .ToList()
                  .Distinct()
                  .Where(u => Users.Count(au => au.UserId == u.Iid) < 1)
                  .Select(u => new RegionAdminModel
                  {
                      UserId = u.Iid,
                      UserName = u.Name,
                      UserEmail = u.Email ?? String.Empty,
                      UserRegion = u.RegionId == null ? String.Empty : u.Region.Name,
                      IsAdmin = false
                  }));
            this.Users.Sort((a, b) =>
            {
                var n = a.UserRegion.CompareTo(b.UserRegion);
                if (n != 0)
                    return n;
                return a.UserName.CompareTo(b.UserName);
            });

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ClimbingContext db = new ClimbingContext();
            String prefixCode;
            if (this.IidParent == null)
                prefixCode = String.Empty;
            else
            {
                var pReg = db.Regions.Find(IidParent);
                prefixCode = (pReg == null) ? String.Empty : (pReg.SymCode ?? String.Empty);
            }
            var fullCode = prefixCode + SymCode;
            if (fullCode.Length > RegionModel.CODE_LENGTH)
                yield return new ValidationResult(
                    String.Format("Длина кода должна быть не более {0} символов", RegionModel.CODE_LENGTH - prefixCode.Length),
                    new String[] { "SymCode" });
            if (!String.IsNullOrEmpty(fullCode))
            {
                
                if (db.Regions.Count(r => r.Iid != (this.Iid ?? 0) && r.SymCode.Equals(fullCode, StringComparison.Ordinal)) > 0)
                    yield return new ValidationResult("Регион с таким кодом уже сществует", new String[] { "SymCode" });
            }
        }
    }
}