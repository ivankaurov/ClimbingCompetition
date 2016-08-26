using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models;
using System.ComponentModel.DataAnnotations;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.DataProcessors.Models;
using System.Threading.Tasks;
using System.Threading;
using WebClimbing.ServiceClasses;

namespace WebClimbing.DataProcessors
{
    public sealed class CompetitionProcessor : IDisposable
    {
        private ClimbingContext db = new ClimbingContext();

        public List<RegionModel> GetCompetitionRegions()
        {
            var res = db.Competitions.Where(c => c.Region.IidParent != null)
                .Select(c => c.Region.Parent).Distinct().ToList();
            res.Sort();
            return res;
        }

        public List<RegionModel> GetRegionsToCreateCompetition(UserProfileModel user)
        {
            List<RegionModel> result = new List<RegionModel>();
            var adminZones = user.Roles.Where(r => r.RoleId >= (int)RoleEnum.Admin && r.CompID == null)
                .Select(r => r.RegionID).Distinct();
            foreach (var zoneId in adminZones)
                result.AddRange(db.Regions.Where(r => (r.IidParent ?? 0) == (zoneId ?? 0)));
            result.Sort();
            return result;
        }

        public List<int> GetCompetitionYears()
        {
            var resList= db.Competitions.Select(c => c.Start.Year).Distinct().OrderBy(n => n).ToList();
            if (resList.Count < 1)
                resList.Add(DateTime.Now.Year);
            return resList;
        }

        public String[] SaveCompetition(CompetitionEditModel model, out long newIid)
        {
            newIid = model.Iid;

            List<String> errors = new List<string>();

            CompetitionModel comp;
            if (model.Iid > 0)
            {
                comp = db.Competitions.Find(model.Iid);
                foreach (var agr in model.ageGroups.Where(g => !g.Confirmed))
                {
                    var oldGRREg = comp.AgeGroups.FirstOrDefault(g => g.AgeGroupId == agr.GroupId);
                    if (oldGRREg == null)
                        continue;
                    if (oldGRREg.Competitiors.Count > 0)
                        errors.Add(String.Format("Нельзя удалить группу {0}. В ней есть заявленные участники", oldGRREg.AgeGroup.FullName));
                    else
                        db.CompetitionAgeGroups.Remove(oldGRREg);
                }
            }
            else
            {
                comp = new CompetitionModel
                {
                    AgeGroups = new List<Comp_AgeGroupModel>(),
                    Parameters = new List<CompetitionParameterModel>()
                };
                if (comp.AgeGroups == null)
                    comp.AgeGroups = new List<Comp_AgeGroupModel>();
                db.Competitions.Add(comp);
            }
            var reg = db.Regions.Find(model.RegionId);
            if (reg == null)
                errors.Add(String.Format("Регион с ID={0} не найден", model.RegionId));
            
            comp.ApplicationsEnd = model.ApplicationsEndDate.Value;
            comp.ApplicationsEditEnd = model.ApplicationsEditEndDate.Value;
            comp.AllowLateAppl = model.AllowLateApps;
            comp.SignApplications = model.RequireSignature;
            comp.AllowMultipleTeams = model.AllowMultipleTeams;
            comp.Boulder = model.Boulder;
            comp.End = model.StopDate.Value;
            comp.Lead = model.Lead;
            comp.Name = model.FullName;
            comp.RegionId = model.RegionId.Value;
            comp.ShortName = model.ShortName;
            comp.Speed = model.Speed;
            comp.Start = model.StartDate.Value;
            comp.Rank = (reg.IidParent == null) ? CompetitionLevel.National : CompetitionLevel.Regional;

            foreach (var agr in model.ageGroups.Where(a => a.Confirmed))
            {
                if (comp.AgeGroups.FirstOrDefault(g => g.AgeGroupId == agr.GroupId) == null)
                    comp.AgeGroups.Add(new Comp_AgeGroupModel()
                    {
                        AgeGroupId = agr.GroupId
                    });
            }
            if (errors.Count > 0)
                return errors.ToArray();
            try
            {
                db.SaveChanges();
                newIid = comp.Iid;
            }
            catch (Exception ex)
            {
                return new String[] { String.Format("Ошибка сохранения:{0}{1}", Environment.NewLine, ex.ToString()) };
            }
            return new String[0];
        }

        public List<RoleSelectorModel> GetCompetitonRoles(long id)
        {
            var comp = db.Competitions.Find(id);
            List<RoleSelectorModel> res = new List<RoleSelectorModel>();
            foreach (var user in db.UserRoles.Where(r => r.CompID == id).ToList())
            {
                if (res.Count(r => r.UserId == user.UserId) < 1)
                    res.Add(new RoleSelectorModel
                    {
                        UserId = user.UserId,
                        UserName = user.User.Name,
                        RegionName = (user.User.RegionId == null) ? String.Empty : user.User.Region.Name,
                        ReadOnly = false,
                        Role = user.Role.RoleCode
                    });
            }

            foreach (var user in db.UserProfiles
                .Where(u => u.Region != null && (u.Region.IidParent ?? 0) == (comp.Region.IidParent ?? 0))
                .ToList()
                .Where(r => res.Count(qr => qr.UserId == r.Iid) < 1))
            {
                res.Add(new RoleSelectorModel
                {
                    UserId = user.Iid,
                    UserName = user.Name,
                    RegionName = (user.RegionId == null) ? String.Empty : user.Region.Name,
                    ReadOnly = false,
                    Role = null
                });
            }

            foreach (var user in db.UserRoles
                .Where(r => r.CompID == null
                    && (r.RegionID ?? 0) == (comp.Region.IidParent ?? 0)
                    && r.RoleId >= (int)RoleEnum.Admin)
                    .ToList()
                    .Select(r => r.User)
                    .Distinct())
            {
                var updModel = res.FirstOrDefault(r => r.UserId == user.Iid);
                if (updModel == null)
                {
                    updModel = new RoleSelectorModel
                    {
                        UserId = user.Iid,
                        UserName = user.Name,
                        RegionName = (user.Region==null)?String.Empty:user.Region.Name
                    };
                    res.Add(updModel);
                }
                updModel.Role = RoleEnum.Admin;
                updModel.ReadOnly = true;
            }

            res.Sort((a, b) =>
            {
                var n = a.RegionName.CompareTo(b.RegionName);
                if (n != 0)
                    return n;
                return a.UserName.CompareTo(b.UserName);
            });
            return res;
        }

        public List<UserSendEmail> GetCompUsersForEmail(CompetitionModel comp)
        {
            var res = comp.Users
                      .Select(u => u.User)
                      .Distinct()
                      .Where(u => !String.IsNullOrEmpty(u.Email))
                      .Select(u => new UserSendEmail(u) { Confirmed = true })
                      .ToList();
            foreach (var user in db.UserProfiles.Where(u => !String.IsNullOrEmpty(u.Email)
                                                          && u.Roles.Count(r => r.CompID == null
                                                                      && (r.RegionID ?? 0) == (comp.Region.IidParent ?? 0)
                                                                      && r.RoleId >= (int)RoleEnum.Admin) > 0
                                                       ).ToList()
                    )
            {
                if (res.Count(r => r.UserId == user.Iid) < 1)
                    res.Add(new UserSendEmail(user) { Confirmed = true });
            }
            res.Sort((a, b) =>
            {
                int n = a.TeamName.CompareTo(b.TeamName);
                if (n != 0)
                    return n;
                return a.UserName.CompareTo(b.UserName);
            });
            return res;
        }

        public String[] SendEmail(long compId, IEnumerable<UserSendEmail> data, String subj, String body, out int messagesSent)
        {
            messagesSent = 0;
            var comp = db.Competitions.Find(compId);
            if (comp == null)
                return new String[] { "Не удалось найти нужные соревнования" };
            var allowedUsers = GetCompUsersForEmail(comp).Select(u => u.UserId).ToList();
            List<String> errors = new List<string>();
            try
            {
                int msgCount = 0;
                Parallel.ForEach(data,
                    (u) =>
                    {
                        bool hasLock = false;
                        try
                        {
                            Monitor.Enter(allowedUsers, ref hasLock);
                            if (!allowedUsers.Contains(u.UserId))
                                throw new InvalidUserException(u.UserName, u.Email);
                        }
                        finally
                        {
                            if (hasLock)
                                Monitor.Exit(allowedUsers);
                        }
                        String errorMessage;
                        if (!MailService.SendMessage(u.Email, subj, body, out errorMessage, false))
                            throw new MailSendException(u.UserName, u.Email, errorMessage);
                        Interlocked.Add(ref msgCount, 1);
                        u.Confirmed = false;
                    });
                messagesSent = msgCount;
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(e =>
                {
                    var pmex = e as ParallelMailException;
                    if (pmex != null)
                        errors.Add(pmex.Message);
                    return pmex != null;
                });
            }
            return errors.ToArray();
        }

        abstract class ParallelMailException : ApplicationException
        {
            protected ParallelMailException(String message) : base(message) { }
        }

        [Serializable]
        private sealed class InvalidUserException : ParallelMailException
        {
            public InvalidUserException(String userName, String email)
                : base(String.Format("У вас нет прав для отправки сообщения пользователю {0} по адресу {1}", userName, email))
            { }
        }

        [Serializable]
        private sealed class MailSendException : ParallelMailException
        {
            public MailSendException(String userName, String email, String error)
                : base(String.Format("Ошибка отправки сообщения пользователю {0} по адресу {1}: {2}", userName, email, error))
            { }
        }

        public String[] SaveRoles(long compId, IEnumerable<RoleSelectorModel> roles)
        {
            var comp = db.Competitions.Find(compId);

            var updatableRoles = roles.Where(r => !r.ReadOnly);
            foreach (var ur in updatableRoles.Where(r => r.Role == null))
            {
                var rolesDel = db.UserRoles.Where(r => r.CompID == compId && r.UserId == ur.UserId && r.RegionID == null).ToList();
                foreach (var role in rolesDel)
                    db.UserRoles.Remove(role);
            }
            foreach (var ur in updatableRoles.Where(r => r.Role != null))
            {
                var user = db.UserProfiles.Find(ur.UserId);
                if (user == null || user.IsInRoleRegion(RoleEnum.Admin, comp.Region.IidParent) || user.Region == null)
                    continue;
                if ((user.Region.IidParent ?? 0) != (comp.Region.IidParent ?? 0))
                    continue;
                var role = user.Roles.FirstOrDefault(r => r.CompID == compId && r.RegionID == null);
                if (role == null)
                {
                    role = new UserRoleModel { CompID = compId, UserId = user.Iid, RegionID = null };
                    user.Roles.Add(role);
                }
                role.RoleId = (int)ur.Role;
            }
            db.SaveChanges();
            return new String[0];
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
        }

        ~CompetitionProcessor()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}