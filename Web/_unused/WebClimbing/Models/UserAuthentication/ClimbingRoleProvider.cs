using System;
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