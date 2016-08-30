// <copyright file="DbUserGroup.cs">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Users
{
    [Table("usr_groups")]
    public class DbUserGroup : DbSecurityEntity
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.UserGroups.Remove(this);
        }

        protected override void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr)
        {
            RemoveChildCollection(context, this.Users, ltr);
        }

        protected DbUserGroup() { }

        internal DbUserGroup(BaseContext context, String iid)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            this.Iid = String.IsNullOrWhiteSpace(iid) ? context.CreateNewIid() : (iid.Length > IID_SIZE ? iid.Substring(0, IID_SIZE) : iid);
            this.CreateDate = this.UpdateDate = context.Now;
            this.UserCreated = this.UserUpdated = context.CurrentUserID;
        }

        public DbUserGroup(BaseContext context)
            : base(context) { }

        String name;

        [Column("name"), Required, MaxLength(IID_SIZE * 2), Index("usr_groups_name", IsUnique = true)]
        public String Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    name = null;
                else
                    name = value.Length > 2 * IID_SIZE ? value.Substring(0, IID_SIZE * 2) : value;
            }
        }

        public virtual ICollection<DbUserGroupMember> Users { get; set; }

        public void ResetMembersExplicitRights(BaseContext context, Log.LogicTransaction ltr = null, String subjectId = null)
        {
            IEnumerable<DbRights> rightsToDelete = from gm in context.GroupMembers
                                                   join us in context.Users on gm.UserId equals us.Iid
                                                   join ace in context.AllRights on us.Iid equals ace.ObjectId
                                                   where gm.GroupId.Equals(this.Iid, StringComparison.OrdinalIgnoreCase)
                                                   select ace;
            if (!String.IsNullOrEmpty(subjectId))
                rightsToDelete = rightsToDelete.Where(ace => ace.SubjectId.Equals(subjectId, StringComparison.OrdinalIgnoreCase));
            var aceToDel = rightsToDelete.ToList();
            if (aceToDel.Count > 0)
            {
                var _ltr = ltr ?? context.BeginLtr("RESET_RIGHTS");
                aceToDel.ForEach(ace => ace.RemoveObject(context, ltr));
                if (ltr == null)
                    _ltr.Commit(context);
            }
        }

        public void AddUser(DbUser user, BaseContext context, Log.LogicTransaction ltr = null)
        {
            if (Users == null)
                Users = new List<DbUserGroupMember>();
            if(Users.FirstOrDefault(u=>u.User == user || String.Equals(u.UserId, user.Iid, StringComparison.OrdinalIgnoreCase)) != null)
                return;
            var map = new DbUserGroupMember(context)
            {
                User = user,
                Group = this
            };
            this.Users.Add(map);
            if (ltr != null)
                ltr.AddCreatedObject(map, context);
        }
    }
}