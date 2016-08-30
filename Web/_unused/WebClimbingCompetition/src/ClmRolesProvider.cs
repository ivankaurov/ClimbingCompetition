// <copyright file="ClmRolesProvider.cs">
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
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using System.Collections.Generic;

namespace WebClimbing.src
{
    public class ClmRoleProvider : RoleProvider
    {
        public const string PROVIDER_NAME = "ClmRoleProvider";
        private string connectionString;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (String.IsNullOrEmpty(name))
                name = PROVIDER_NAME;
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "ClimbingCompetition membership provider");
            }

            base.Initialize(name, config);
            connectionString = WebConfigurationManager.ConnectionStrings["db"].ConnectionString;
            //connectionString = ConfigurationManager.ConnectionStrings[config["db"]].ConnectionString;

        }

        private Entities _dc = null;
        protected Entities dc
        {
            get
            {
                if (_dc == null)
                    _dc = new Entities(WebConfigurationManager.ConnectionStrings["db_Entities"].ConnectionString);
                if (_dc.Connection.State != ConnectionState.Open)
                    _dc.Connection.Open();
                return _dc;
            }
        }

        private long GetNextIid()
        {
            //SqlConnection cn = GetConnection();
            try
            {
                var iid = (from rp in dc.ONLuserRoles
                           orderby rp.iid descending
                           select rp.iid).First();
                return (iid + 1);
            }
            catch { return 1; }
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var uN in usernames)
            {
                var u = dc.ONLusers.First(q => q.iid == uN);
                if (u == null)
                    continue;
                foreach(var r in roleNames)
                    if (u.ONLuserRoles.Count(q => q.role_id == r) < 1)
                    {
                        long l = ClimbingCompetition.SortingClass.GetNextIID("ONLuserRoles", "iid", new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString), null);
                        ONLuserRole rN = ONLuserRole.CreateONLuserRole(l, uN, r);
                        rN.notifSent = String.Empty;
                        //ONLuserRole rN = ONLuserRole.CreateONLuserRole(l, uN, r, String.Empty);
                        u.ONLuserRoles.Add(rN);
                        dc.SaveChanges();
                    }
            }
        }

        private string applicationName = "WebApatity";
        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

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
            var uList = from r in dc.ONLuserRoles
                        where r.role_id == roleName &&
                        (String.IsNullOrEmpty(usernameToMatch) ? true :
                        (r.user_id.IndexOf(usernameToMatch) > -1))
                        select r.user_id;
            return uList.ToArray();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                var rL = from u in dc.ONLuserRoles
                         where u.ONLuser.iid == username
                         orderby u.role_id
                         select u.role_id;
                return rL.ToArray();
            }
            catch { return new string[0]; }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            try
            {
                var rL = from u in dc.ONLuserRoles
                         where u.role_id == roleName
                         orderby u.ONLuser.iid ascending
                         select u.ONLuser.iid;
                return rL.ToArray();
            }
            catch { return new string[0]; }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var q = from ur in dc.ONLuserRoles
                    where ur.user_id == username &&
                          ur.role_id == roleName
                    select ur;
            return (q.Count() > 0);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["db"].ConnectionString);
            cmd.Connection.Open();
            try
            {
                cmd.CommandText = "DELETE FROM ONLuserRoles WHERE user_id = @uid AND role_id = @rid";
                cmd.Parameters.Add("@uid", SqlDbType.VarChar, 3);
                cmd.Parameters.Add("@rid", SqlDbType.VarChar, 3);
                cmd.Transaction = cmd.Connection.BeginTransaction();
                try
                {
                    foreach (var uN in usernames)
                    {
                        cmd.Parameters[0].Value = uN;
                        foreach (var rN in roleNames)
                        {
                            cmd.Parameters[1].Value = rN;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
            finally { cmd.Connection.Close(); }
        }

        public override bool RoleExists(string roleName)
        {
            var q = from ur in dc.ONLuserRoles
                    where ur.role_id == roleName
                    select ur;
            return (q.Count() > 0);
        }
    }
}
