using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models.UserAuthentication;
using System.Security.Principal;

namespace WebClimbing.Models
{
    public static class ExtensionMethods
    {
        //private static ClimbingContext db = new ClimbingContext();

        private static bool IsInRole(IPrincipal user, ClimbingContext db,
                      Func<UserRoleModel, bool> roleSelector)
        {
            if (!user.Identity.IsAuthenticated)
                return false;
            return IsInRole(user.GetDbUser(db, false), roleSelector);
        }

        private static bool IsInRole(UserProfileModel user,
                      Func<UserRoleModel, bool> roleSelector)
        {
            if (user == null)
                return false;
            return (user.Roles.ToArray().Count(roleSelector) > 0);
        }

        public static bool IsInRole(this IPrincipal user, ClimbingContext db, RoleEnum role, long? RegionId)
        {
            return IsInRole(user, db, (ur => ur.CompID == null && ((ur.RegionID ?? 0) == (RegionId ?? 0)) && ur.RoleId >= (int)role));
        }

        public static bool IsInRoleRegion(this UserProfileModel user, RoleEnum role, long? regionId)
        {
            return user.Roles.ToArray().Count(ur => ur.CompID == null && (ur.RegionID ?? 0) == (regionId ?? 0) && ur.RoleId >= (int)role) > 0;
        }

        public static bool IsInRoleComp(this UserProfileModel user, RoleEnum role, CompetitionModel comp)
        {
            if (user == null || comp == null)
                return false;
            if (IsInRole(user, (u => u.RoleId >= (int)role && u.CompID == comp.Iid)))
                return true;
            if (((int)role) >= ((int)RoleEnum.Admin))
                return IsInRole(user, u => (u.RoleId >= (int)role) && u.CompID == null && (u.RegionID == null && comp.Region.IidParent == null || u.RegionID != null && comp.Region.IidParent != null && comp.Region.IidParent == u.RegionID));
            return false;
        }

        public static bool IsInRoleComp(this IPrincipal user, ClimbingContext db, RoleEnum role, long CompId, bool checkRegion = true)
        {
            if (IsInRole(user, db, (ur => ur.RegionID == null && ur.CompID == CompId && ur.RoleId >= (int)role)))
                return true;
            if (!checkRegion)
                return false;
            var comp = db.Competitions.Find(CompId);
            if (comp == null)
                return false;
            return user.IsInRole(db, role, comp.Region.IidParent);
        }

        public static UserProfileModel GetDbUser(this System.Security.Principal.IPrincipal user, ClimbingContext db, bool selectAlways = true)
        {
            if (!selectAlways && !user.Identity.IsAuthenticated)
                return null;
            var users = db.UserProfiles.Where(u => u.Name.Equals(user.Identity.Name, StringComparison.OrdinalIgnoreCase));
            if (selectAlways)
                return users.First();
            else
                return users.FirstOrDefault();
        }

        public static bool AllowRegistration(this IPrincipal user, ClimbingContext db)
        {
            if (!user.Identity.IsAuthenticated)
                return false;
            var dbUser = user.GetDbUser(db, false);
            if (dbUser == null)
                return false;
            return (dbUser.Roles.Where(r => r.CompID == null).Count(r => r.RoleId >= (int)RoleEnum.Admin) > 0);
        }

        public static bool AllowApplications(this IPrincipal user, ClimbingContext db, long compID)
        {
            if (!user.Identity.IsAuthenticated)
                return false;
            if (user.IsInRoleComp(db, RoleEnum.Admin, compID))
                return true;
            if (user.IsInRoleComp(db, RoleEnum.User, compID, false))
                return true;
            var comp = db.Competitions.Find(compID);
            if (comp == null)
                return false;
            var dbUser = user.GetDbUser(db, false);
            if (dbUser == null)
                return false;
            if (dbUser.RegionId == null)
                return false;
            return (dbUser.Region.IidParent ?? 0) == (comp.Region.IidParent ?? 0);
        }

        public static IEnumerable<RegionModel> AdminRegions(this IPrincipal user, ClimbingContext db)
        {
            if (!user.Identity.IsAuthenticated)
                return new RegionModel[0];
            return user.GetDbUser(db).AdminRegions();
        }

        public static IEnumerable<RegionModel> AdminRegions(this UserProfileModel user)
        {
            foreach (var urm in user.Roles.Where(r => r.CompID == null && r.RoleId >= (int)RoleEnum.Admin).OrderBy(r=>r.RegionID))
                if (urm.RegionID == null)
                    yield return null;
                else
                    yield return urm.Region;
        }

        /*
        public static RightsEnum GetUserRights(this System.Security.Principal.IPrincipal user, RightsType rightsType, ClimbingContext db, long? regionId = null, long? competitionId = null)
        {
            var dbUser = user.GetDbUser(db, false);
            if (dbUser == null)
                return RightsEnum.None;
            return dbUser.GetUserRights(rightsType, regionId, competitionId);
        }

        public static RightsEnum GetUserRights(this UserProfileModel user, RightsType rightsType, long? regionId = null, long? competitionId = null)
        {
            RightsEnum res = RightsEnum.None;
            IEnumerable<UserRoleModel> roleList = user.Roles.Where(r => r.Role.Rights.Count(rt => rt.Type == rightsType) > 0);
            if (regionId == null)
                roleList = roleList.Where(r => r.RegionID == null);
            else
                roleList = roleList.Where(r => r.RegionID == regionId);
            if (competitionId == null)
                roleList = roleList.Where(r => r.CompID == null);
            else
                roleList = roleList.Where(r => r.CompID == competitionId);

            foreach (var role in roleList.Select(r => r.Role).Distinct().ToList())
                foreach (var right in role.Rights.Where(rt => rt.Type == rightsType))
                    res = (res | right.Rights);
            return res;
        }

        public static bool CheckUserRights(this UserProfileModel user, RightsEnum rights, RightsType rightsType, long? regionId = null, long? competitionId = null)
        {
            IEnumerable<UserRoleModel> rolesList = user.Roles.Where(r => r.Role.Rights.Count(rt => rt.Type == rightsType) > 0);
            if (regionId == null)
                rolesList = rolesList.Where(r => r.RegionID == null);
            else
                rolesList = rolesList.Where(r => r.RegionID == regionId);
            if (competitionId == null)
                rolesList = rolesList.Where(r => r.CompID == null);
            else
                rolesList = rolesList.Where(r => r.CompID == competitionId);
            foreach (var role in rolesList.Select(r => r.Role).Distinct().ToList())
                if (role.HasFlags(rightsType, rights))
                    return true;
            return false;
        }

        public static bool HasFlags(this RoleModel role, RightsType rightsType, RightsEnum rightsEnum)
        {
            if (rightsEnum == RightsEnum.None)
                return true;
            var rightsRecord = role.Rights.FirstOrDefault(r => r.Type == rightsType);
            if (rightsRecord == null)
                return false;
            return ((rightsRecord.Rights & rightsEnum) == rightsEnum);
        }

        public static bool AllowedToEditCompetition(this System.Security.Principal.IPrincipal user, long compID, ClimbingContext db, RightsEnum rights = RightsEnum.Edit)
        {
            var dbUser = user.GetDbUser(db, false);
            if (dbUser == null)
                return false;
            var dbComp = db.Competitions.FirstOrDefault(c => c.Iid == compID);
            if (dbComp == null)
                return false;
            if (dbUser.CheckUserRights(rights, regionId: dbComp.Region.IidParent, rightsType: RightsType.Competition))
                return true;
            return dbUser.CheckUserRights(rights, RightsType.Competition, competitionId: dbComp.Iid);
        }


        public static bool AllowedToEditUser(this System.Security.Principal.IPrincipal user, int userId, ClimbingContext db)
        {
            var curUser = user.GetDbUser(db, false);
            if (curUser == null)
                return false;
            var checkUser = db.UserProfiles.Find(userId);
            if (checkUser == null)
                return false;
            long? userRegion = checkUser.RegionId;

            if (userRegion == null)
                return (curUser.Roles.Where(r => r.RegionID == null).ToList().Count(r => r.Role.HasFlags(RightsType.Database, RightsEnum.Edit)) > 0);

            return curUser.CheckUserRights(RightsEnum.Edit, RightsType.Database, checkUser.Region.IidParent);
        }
        
        public static bool AllowApplications(this IPrincipal user, CompetitionModel comp, ClimbingContext db)
        {
            if (comp == null || user == null || !user.Identity.IsAuthenticated)
                return false;
            var dbUser = user.GetDbUser(db, false);
            if (dbUser == null)
                return false;
            return AllowApplications(comp, dbUser);
        }

        private static bool AllowApplications(CompetitionModel comp, UserProfileModel dbUser)
        {
            var dtNow = DateTime.Now.Date;
            if (dtNow > comp.End.Date)
                return false;
            if (dbUser.GetUserRights(RightsType.Competition, competitionId: comp.Iid) == RightsEnum.Edit)
                return true;
            if (dbUser.GetUserRights(RightsType.Database, regionId: comp.Region.IidParent) == RightsEnum.Edit)
                return true;
            if (dtNow > comp.ApplicationsEnd.Date)
                return false;
            if (dbUser.RegionId == null || dbUser.Region == null)
                return false;
            if (dbUser.Region.IidParent == null)
                return (comp.Region.IidParent == null);
            else
                return (comp.Region.IidParent == dbUser.Region.IidParent);
        }

        public static bool AllowApplications(this IPrincipal user, CompetitionModel comp, long teamId, ClimbingContext db)
        {
            bool result = user.AllowApplications(comp, db);
            if (!result)
                return false;
            var dbUser = user.GetDbUser(db);
            HashSet<long> allowedTeamList;
            if (comp.Region.IidParent == null)
                allowedTeamList = new HashSet<long>(db.Regions.Where(r => r.IidParent == null).Select(r => r.Iid));
            else
                allowedTeamList = new HashSet<long>(db.Regions.Where(r => r.IidParent == comp.Region.IidParent).Select(r => r.Iid));
            if (!allowedTeamList.Contains(teamId))
                return false;
            if (dbUser.RegionId == teamId)
                return true;
            if (dbUser.GetUserRights(RightsType.Competition, competitionId: comp.Iid) == RightsEnum.Edit)
                return true;
            if (dbUser.GetUserRights(RightsType.Database, regionId: comp.Region.IidParent) == RightsEnum.Edit)
                return true;
            return false;
        }

        public static RegionModel[] GetAllowedRegions(this CompetitionModel comp, UserProfileModel user, ClimbingContext db)
        {
            if (!AllowApplications(comp, user))
                return new RegionModel[0];
            List<RegionModel> res;
            if (comp.Region.IidParent == null)
                res = db.Regions.Where(r => r.IidParent == null).ToList();
            else
                res = db.Regions.Where(r => r.IidParent == comp.Region.IidParent).ToList();

            if (user.GetUserRights(RightsType.Competition, competitionId: comp.Iid) == RightsEnum.Edit ||
                user.GetUserRights(RightsType.Database, regionId: comp.Region.IidParent) == RightsEnum.Edit)
            {
                res.Sort();
                return res.ToArray();
            }
            else
            {
                if (user.RegionId == null || !res.Contains(user.Region))
                    return new RegionModel[0];
                else
                    return new RegionModel[] { user.Region };
            }
        }*/
    }
}