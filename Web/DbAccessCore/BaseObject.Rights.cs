// <copyright file="BaseObject.Rights.cs">
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

﻿using DbAccessCore.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore
{
    partial class BaseObject
    {
        private RightsEnum? GetRightsFromCollection(RightsActionEnum action, IEnumerable<DbRights> rights)
        {
            if (rights == null)
                return null;
            RightsEnum? result = null;
            foreach (var right in rights.Where(a => a.Action >= action).OrderByDescending(a => a.Action))
            {
                if (!result.HasValue || result.Value < right.Rights)
                    result = right.Rights;
            }

            return result;
        }

        public virtual RightsEnum? GetRights(String securityEntityID, RightsActionEnum action, BaseContext context, out Boolean isInherited)
        {
            if (context.AdminAllowAll && context.UserIsAdmin(securityEntityID))
            {
                isInherited = false;
                return RightsEnum.Allow;
            }
            var directRights = this.GetRightsFromCollection(action, context.AllRights
                                                                           .Where(r => r.SubjectId == this.Iid && r.ObjectId == securityEntityID)
                                                                           .ToList());

            if (directRights.HasValue)
            {
                isInherited = false;
                return directRights;
            }
            isInherited = true;

            var groupRightsForThisUser = (from gm in context.GroupMembers
                                          join gr in context.UserGroups on gm.GroupId equals gr.Iid
                                          join or in context.AllRights on gr.Iid equals or.ObjectId
                                          where or.SubjectId == this.Iid
                                          select or).ToList();
            switch (groupRightsForThisUser.Count)
            {
                case 0: return null;
                case 1: return groupRightsForThisUser[0].Rights;
            }

            var byGroup = groupRightsForThisUser.ToLookup(g => g.ObjectId);
            foreach(var e in byGroup)
            {
                var rights = this.GetRightsFromCollection(action, e);
                if (!directRights.HasValue || directRights.Value > rights)
                    directRights = rights;
            }

            return directRights;
        }

        public RightsEnum? GetRights(String securityEntityID, RightsActionEnum action, BaseContext context)
        {
            Boolean isInherited;
            return GetRights(securityEntityID, action, context, out isInherited);
        }

        public RightsEnum GetNotNullableRigths(string securityEntityId, RightsActionEnum action, BaseContext context)
        {
            return this.GetRights(securityEntityId, action, context) ?? RightsEnum.Deny;
        }

        public void SetRights(String securityEntityID, RightsActionEnum action,RightsEnum? rightsValue, BaseContext context, Log.LogicTransaction ltr = null)
        {
            var ace = context.AllRights.Where(r => r.SubjectId.Equals(this.Iid, StringComparison.OrdinalIgnoreCase) && r.ObjectId.Equals(securityEntityID, StringComparison.OrdinalIgnoreCase))
                                       .ToList()
                                       .FirstOrDefault(r => r.Action == action);
            if (rightsValue == null)
            {
                if (ace != null)
                    ace.RemoveObject(context, ltr);
                return;
            }
            if (ace == null)
            {
                ace = context.AllRights.Add(new Users.DbRights(context)
                {
                    Subject = this,
                    SubjectId = this.Iid,
                    ObjectId = securityEntityID,
                    Rights = rightsValue.Value,
                    Action = action
                });
                if (ltr != null)
                    ltr.AddCreatedObject(ace, context);
                return;
            }

            if (ace.Rights != rightsValue.Value)
            {
                if (ltr != null)
                    ltr.AddUpdatedObjectBefore(ace, context);
                ace.Rights = rightsValue.Value;
                if (ltr != null)
                    ltr.AddUpatedObjectAfter(ace, context);
            }
        }
    }
}
