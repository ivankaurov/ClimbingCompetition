// <copyright file="DbUser.cs">
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
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Crypto;

namespace DbAccessCore.Users
{
    [Table("usr_users")]
    public class DbUser : DbSecurityEntity
    {
        protected override void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr)
        {
            RemoveChildCollection(context, ActiveLogins, ltr);
            RemoveChildCollection(context, Audit, ltr);
            RemoveChildCollection(context, Groups, ltr);
        }

        protected override void RemoveEntity(BaseContext context)
        {
            context.Users.Remove(this);
        }

        protected DbUser() { }

        public DbUser(BaseContext context) : base(context) { }

        [Column("email"), MaxLength(100), Index("ux_usr_eml", IsUnique = false)]
        public String Email { get; set; }

        [Column("username"), MaxLength(4 * IID_SIZE), Required, Index(IsUnique = true)]
        public String UserName { get; set; }

        [Column("password")]
        public String Password { get; set; }

        [Column("salt")]
        public Byte[] Salt { get; set; }

        [Column("need_change_password")]
        public Boolean NeedChangePassword { get; set; }

        [Column("allow_multiple_login")]
        public Boolean AllowMultipleLogins { get; set; }

        [Column("last_online_web")]
        public DateTime? LastOnlineWeb { get; set; }

        [Column("last_online_desktop")]
        public DateTime? LastOnlineDesktop { get; set; }

        public virtual ICollection<DbActiveUser> ActiveLogins { get; set; }
        public virtual ICollection<DbAudit> Audit { get; set; }
        public virtual ICollection<DbUserGroupMember> Groups { get; set; }

        public void SetPassword(String password)
        {
            Byte[] salt;
            this.Salt = salt = Crypto.Hashing.GenerateSalt(IID_SIZE);
            this.Password = Crypto.Hashing.ComputeHashString(password ?? String.Empty, salt);
        }

        public Boolean CheckPassword(String password)
        {
            return HasPassword ? string.Equals(this.Password, password.ComputeHashString(this.Salt), StringComparison.Ordinal) : false;
        }

        [NotMapped]
        public Boolean HasPassword
        {
            get
            {
                return this.Password != null && this.Password.Length > 0
                    && this.Salt != null && this.Salt.Length > 0;
            }
        }
    }
}
