using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.DataProcessors.Models;
using ErrorTuple = System.Tuple<string, string>;
using System.Web.Mvc;

namespace WebClimbing.DataProcessors
{
    public sealed class RegionsProcessor : IDisposable
    {
        private ClimbingContext db = new ClimbingContext();

        #region DisposingPattern
        private void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~RegionsProcessor()
        {
            Dispose(false);
        }
        #endregion

        public SelectList GetAdministartorRegion(UserProfileModel user, long? setSelectedParent)
        {
            var userAdmList = user.Roles.Where(r => r.CompID == null && r.RoleId >= (int)RoleEnum.Admin).ToList();
            if (userAdmList.Count(us => (us.RegionID ?? 0) == (setSelectedParent ?? 0)) < 1)
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            userAdmList.Sort((a,b)=>{
                if(a.RegionID==null && b.RegionID==null)
                    return 0;
                if(a.RegionID==null)
                    return -1;
                if(b.RegionID==null)
                    return 1;
                return a.Region.CompareTo(b.Region);
            });
            return new SelectList(userAdmList.Select(ur => new { Iid = ur.RegionID, Name = (ur.Region == null) ? String.Empty : ur.Region.Name }),
                "Iid", "Name", setSelectedParent);
        }

        public IEnumerable<RegionModel> GetAdminRegions(UserProfileModel user)
        {
            if (user == null)
                return new RegionModel[0];
            return user.Roles.Where(r => r.CompID == null && r.RegionID != null && r.RoleId >= (int)RoleEnum.Admin).Select(r => r.Region);
        }

        public ErrorTuple[] SaveRegion(UserProfileModel user, RegionEditModel model)
        {
            List<ErrorTuple> res = new List<ErrorTuple>();
            if (!user.IsInRoleRegion(RoleEnum.Admin, model.IidParent))
                res.Add(new ErrorTuple("IidParent", "У вас нет прав для создания команд в выбранном регионе"));
            RegionModel saveModel;
            if ((model.Iid ?? 0) > 0)
            {
                saveModel = db.Regions.Find(model.Iid);
                if (saveModel == null)
                    res.Add(new ErrorTuple(String.Empty, "Корректируемый регион был удален"));
                else if (!user.IsInRoleRegion(RoleEnum.Admin, saveModel.IidParent))
                    res.Add(new ErrorTuple(String.Empty, "У вас нет прав для корректировки выбранного региона"));
                if (res.Count > 0)
                    return res.ToArray();
            }
            else
            {
                if (res.Count > 0)
                    return res.ToArray();
                saveModel = new RegionModel() { UserRoles = new List<UserRoleModel>() };
                db.Regions.Add(saveModel);
            }
            saveModel.IidParent = model.IidParent;
            saveModel.Name = model.Name;
            String prefixCode;
            if (model.IidParent == null)
                prefixCode = String.Empty;
            else
            {
                var preg = db.Regions.Find(model.IidParent);
                prefixCode = preg == null ? String.Empty : (preg.SymCode ?? String.Empty);
            }
            saveModel.SymCode = prefixCode + model.SymCode;
            if (model.Users == null)
                saveModel.UserRoles.Clear();
            else
            {
                var existingRoles = saveModel.UserRoles.ToList();
                var rolesToDel = existingRoles.Where(r => r.CompID != null || r.RoleId < (int)RoleEnum.Admin).ToList();
                foreach (var r in rolesToDel)
                    existingRoles.Remove(r);
                rolesToDel.AddRange(existingRoles.Where(r => model.Users.Count(um => um.UserId == r.UserId && um.IsAdmin) < 1));
                foreach (var r in rolesToDel)
                    existingRoles.Remove(r);
                foreach (var um in model.Users.Where(um => um.IsAdmin && existingRoles.Count(r => r.UserId == um.UserId) < 1))
                    saveModel.UserRoles.Add(new UserRoleModel { UserId = um.UserId, CompID = null, RoleId = (int)RoleEnum.Admin });
                foreach (var r in rolesToDel)
                    db.UserRoles.Remove(r);
            }
            db.SaveChanges();
            model.Iid = saveModel.Iid;
            return new ErrorTuple[0];
        }

        public String[] DeleteRegion(UserProfileModel user, long regionId)
        {
            var region = db.Regions.Find(regionId);
            if (region == null)
                return new String[] { "Регион был удален" };
            if (!user.IsInRoleRegion(RoleEnum.Admin, region.IidParent))
                return new String[] { "У вас нет прав для удаления региона" };
            List<String> errors = new List<string>();
            if (region.CompetitionsHold.Count > 0)
                errors.Add("Данный регион проводил соревнования. Сначала надо удалить их");
            if (region.PeopleCompetitions.Count > 0)
                errors.Add("От данного региона выступали участники. Сначала удалите их");
            if (errors.Count > 0)
                return errors.ToArray();
            db.Regions.Remove(region);
            db.SaveChanges();
            return new String[0];
        }

        public String[] SaveGlobalAdmins(UserProfileModel user, IEnumerable<RegionAdminModel> model)
        {
            List<String> errors = new List<string>();
            if (!user.IsInRoleRegion(RoleEnum.Admin, null))
                errors.Add("У вас нет прав на корректировку списка");
            if (model == null)
                model = new RegionAdminModel[0];
            if (model.Count(a => a.IsAdmin) < 1)
                errors.Add("Невозможно удалить последнего администратора");
            if (model.Count(a => a.IsAdmin && a.UserId == user.Iid) < 1)
                errors.Add("Нельзя удалить себя из списка администраторов");
            if (errors.Count > 0)
                return errors.ToArray();
            var existingRoles = db.UserRoles.Where(r => r.RegionID == null && r.CompID == null).ToList();
            var rolesToDel = existingRoles.Where(r => r.RoleId < (int)RoleEnum.Admin).ToList();
            foreach (var r in rolesToDel)
                existingRoles.Remove(r);
            rolesToDel.AddRange(existingRoles.Where(r => model.Count(m => m.UserId == r.UserId && m.IsAdmin) < 1));
            foreach (var r in rolesToDel)
                existingRoles.Remove(r);
            foreach (var em in model.Where(m => m.IsAdmin && existingRoles.Count(er => er.UserId == m.UserId) < 1))
                db.UserRoles.Add(new UserRoleModel
                {
                    RegionID = null,
                    CompID = null,
                    RoleId = (int)RoleEnum.Admin,
                    UserId = em.UserId
                });
            foreach (var r in rolesToDel)
                db.UserRoles.Remove(r);
            db.SaveChanges();
            return new String[0];
        }
    }
}