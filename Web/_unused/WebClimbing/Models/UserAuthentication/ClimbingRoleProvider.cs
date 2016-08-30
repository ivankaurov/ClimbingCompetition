// <copyright file="ClimbingRoleProvider.cs">
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
using System.Data.Entity;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;

namespace WebClimbing.Models.UserAuthentication
{
    public class ClimbingRoleProvider : RoleProvider
    {
        //private ClimbingContext db = new ClimbingContext();

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get { return "WebClimbing"; } set { } }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        private string[] GetRolesForUser(string username, long? compID, long? regionID)
        {
            using (var db = new ClimbingContext())
            {
                var user = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
                if (user == null)
                    return new string[0];
                IEnumerable<UserRoleModel> roles = user.Roles.ToArray();
                if (compID != null && compID.HasValue)
                    roles = roles.Where(r => r.CompID != null || r.CompID.Value == compID.Value);
                else
                    roles = roles.Where(r => r.CompID == null);

                if (regionID != null && regionID.HasValue)
                    roles = roles.Where(r => r.RegionID != null && r.RegionID.Value == regionID.Value);
                else
                    roles = roles.Where(r => r.RegionID == null);

                var res = roles.Select(r => ((RoleEnum)r.RoleId).ToString()).ToArray();
                return res;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            return GetRolesForUser(username, null, null);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetRolesForUser(username).Select(r=>r.ToUpperInvariant()).Contains(roleName.ToUpperInvariant());
        }

        public bool IsUserInRole(string username, RoleEnum role, long? compID = null, long? regionID = null)
        {
            return GetRolesForUser(username, compID, regionID).Select(rs => rs.ToUpperInvariant()).Contains(role.ToString().ToUpperInvariant());
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}