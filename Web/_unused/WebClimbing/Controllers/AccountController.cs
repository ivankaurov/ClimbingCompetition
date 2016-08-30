// <copyright file="AccountController.cs">
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
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using WebClimbing.ServiceClasses;
using WebMatrix.WebData;
using System.Threading;

namespace WebClimbing.Controllers
{
    [Authorize]
    /*[InitializeSimpleMembership]*/
    public class AccountController : Controller
    {
        ClimbingContext db = new ClimbingContext();

        private static object locker = new object();

        //
        // GET: /Account/Login

        public ActionResult Index(long? id = null, DbResult? creationResult = null)
        {
            var usr = User.GetDbUser(db);
            var regionList = usr.AdminRegions()
                .Select(r => r == null ? new { Iid = (long)0, Name = "Россия" } : new { Iid = r.Iid, Name = r.Name }).ToList();
            if (regionList.Count < 1)
                return RedirectToAction("Login", new { returnUrl = Url.Action("Index", new { id = id }) });
            regionList.Sort((a, b) =>
            {
                if (a.Iid == 0 || b.Iid == 0)
                    return a.Iid.CompareTo(b.Iid);
                else
                    return a.Name.CompareTo(b.Name);
            });
            long? selectedRegion;
            var regToCheck = (id ?? (usr.RegionId ?? 0));
            if (regionList.Count(r => r.Iid == regToCheck) > 0)
                selectedRegion = regToCheck;
            else
                selectedRegion = null;
            ViewBag.Regions = new SelectList(regionList, "Iid", "Name", selectedRegion);
            ViewBag.Db = db;
            ViewBag.Region = selectedRegion;
            ViewBag.Result = creationResult;
            return View();
        }

        public ActionResult UserList(long? parentRegion, String divId)
        {
            List<UserProfileModel> data;
            bool allowNew;
            long? region = (parentRegion ?? 0) < 1 ? null : parentRegion;
            if (User.IsInRole(db, RoleEnum.Admin, (parentRegion ?? 0) < 1 ? null : parentRegion))
            {
                if (region == null)
                    data = db.UserProfiles.Where(u => u.RegionId == null || u.Region.IidParent == null).ToList();
                else
                    data = db.UserProfiles.Where(u => u.RegionId == region || u.Region.IidParent == region).ToList();
                allowNew = true;
            }
            else
            {
                data = new List<UserProfileModel>();
                allowNew = false;
            }
            ViewBag.AllowNew = allowNew;
            ViewBag.DivID = divId;
            ViewBag.Db = db;
            return PartialView(data);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, DbResult? result = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Result = result;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);
            var userName = (model.UserName ?? String.Empty).Trim();
            var dbUser = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (dbUser == null)
            {
                var userEmailList = db.UserProfiles.Where(u => (u.Email ?? String.Empty).Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
                if (userEmailList.Count > 1)
                    ModelState.AddModelError(String.Empty, "Вход в систему по данному E-Mail невозможен");
                else if (userEmailList.Count == 1)
                    dbUser = userEmailList[0];
            }
            if (dbUser != null && WebSecurity.Login(dbUser.Name, model.Password, persistCookie: model.RememberMe))
                return RedirectToLocal(returnUrl);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Неправильное имя пользователя или пароль.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            List<RegionModel> regions = SetRegionsViewBag();
            if (regions.Count < 1)
                return RedirectToAction("Login", new { returnUrl = Url.Action("Register") });
            return View();
        }

        private List<RegionModel> SetRegionsViewBag()
        {
            List<RegionModel> regions = new List<RegionModel>();
            foreach (var reg in User.AdminRegions(db))
            {
                if (reg == null)
                {
                    regions.Add(new RegionModel { Iid = 0, Name = String.Empty });
                    regions.AddRange(db.Regions.Where(r => r.IidParent == null).OrderBy(r => r.Name));
                }
                else
                    regions.AddRange(reg.Children.OrderBy(r => r.Name));
            }
            ViewBag.Regions = new SelectList(regions, "Iid", "Name");
            return regions;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegistrationModel model)
        {
            bool hasLocker = false;
            try
            {
                Monitor.Enter(locker, ref hasLocker);
                var regions = SetRegionsViewBag();
                if (regions.Count(r => r.Iid == (model.Region ?? 0)) < 1)
                    ModelState.AddModelError(String.Empty, "У вас нет прав для создания пользователей от выбранного региона");
                if (ModelState.IsValid && db.UserProfiles.Count(u => u.Name.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)) > 0)
                    ModelState.AddModelError(String.Empty, "Пользователь с таким именем существует");
                if (ModelState.IsValid)
                {
                    var newUser = new UserProfileModel
                    {
                        Email = model.Email,
                        Inactive = true,
                        RegionId = model.Region,
                        Name = model.UserName,
                        Token = GenerateToken()
                    };
                    newUser.SetPassword(String.Empty);

                    MailService.SendMessage(newUser.Email, "Активация пользователя",
                        String.Format("На Ваше имя зарегистрирован пользователь.{0}" +
                        "Имя пользователя: {1}{0}" +
                        "Для активации учетной записи и создания пароля, пожалуйста, перейдите по ссылке {2}",
                        Environment.NewLine, newUser.Name,
                        Url.Action("Activate", "Account", new { id = newUser.Token }, "http")), false);
                    if (newUser.RegionId < 1)
                        newUser.RegionId = null;
                    db.UserProfiles.Add(newUser);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = newUser.RegionId == null ? null : newUser.Region.IidParent, creationResult = DbResult.Created });
                }
                else
                    return View(model);
            }
            finally
            {
                if (hasLocker)
                    Monitor.Exit(locker);
            }
        }

        private String GenerateToken(int length = UserProfileModel.TOKEN_LENGTH)
        {
            String token;
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Clear();
                for (int i = 0; i < length; i++)
                    sb.Append((char)rnd.Next('a', 'z' + 1));
                token = sb.ToString();
            } while (db.UserProfiles.Count(u => u.Token.Equals(token, StringComparison.OrdinalIgnoreCase)) > 0);
            return token;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Activate(String id)
        {
            var userToActivate = db.UserProfiles.FirstOrDefault(u => u.Token.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (userToActivate == null)
                return new HttpNotFoundResult("Пользователь не найден");
            //public ActionResult ForgottenPassword(string initialName = null, ValidateStatusMessage message = ValidateStatusMessage.Nothing)
            if (!userToActivate.Inactive && userToActivate.PasswordTokenExpirationTime != null && userToActivate.PasswordTokenExpirationTime < DateTime.UtcNow)
            {
                return RedirectToAction("ForgottenPassword", new { initialName = userToActivate.Name, message = ValidateStatusMessage.Expired });
            }
            UserActivationModel model = new UserActivationModel { 
                UserName = userToActivate.Name,
                Password = String.Empty, 
                PasswordConfirm = String.Empty,
                Token=userToActivate.Token
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(UserActivationModel model)
        {
            bool hasLocker = false;
            try
            {
                Monitor.Enter(locker, ref hasLocker);
                UserProfileModel user = null;
                if (ModelState.IsValid)
                    user = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(model.UserName, StringComparison.OrdinalIgnoreCase) &&
                                                             u.Token.Equals(model.Token, StringComparison.OrdinalIgnoreCase));
                if (user == null)
                    ModelState.AddModelError(String.Empty, "Пользователь не найден");
                if (ModelState.IsValid)
                {
                    user.SetPassword(model.Password);
                    user.PasswordTokenExpirationTime = null;
                    user.Token = String.Empty;
                    user.Inactive = false;
                    db.SaveChanges();
                    return RedirectToAction("Login", new { returnUrl = String.Empty, result = DbResult.Updated });
                }
                else
                    return View(model);
            }
            finally
            {
                if (hasLocker)
                    Monitor.Exit(locker);
            }
        }
        /*
        [HttpGet]
        public ActionResult Delete(int id, bool lastAdmin = false)
        {
            var dbUser = db.UserProfiles.Find(id);
            if (dbUser == null)
                return HttpNotFound();
            ViewBag.Title = "Удаление пользователя";
            DeleteModel items = new DeleteModel();
            items.UserName = dbUser.Name;
            items.RegionName = dbUser.RegionId == null ? String.Empty : dbUser.Region.Name;
            items.Iid = id;
            ViewBag.ListId = GetListRegion(dbUser);
            if (lastAdmin)
                ViewBag.ErrorMessage = "Нельзя удалить последнего администратора региона";
            else
                ViewBag.ErrorMessage = String.Empty;
            return View(items);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id = 0)
        {
            var dbUser = db.UserProfiles.Find(id);
            if (dbUser == null)
                return HttpNotFound();
            IEnumerable<UserRoleModel> regionAdmins = db.UserRoles;
            if (dbUser.RegionId == null)
                regionAdmins = regionAdmins.Where(r => r.RegionID == null);
            else
                regionAdmins = regionAdmins.Where(r => r.RegionID == dbUser.RegionId);
            var pReg = GetListRegion(dbUser);
            db.UserProfiles.Remove(dbUser);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = pReg });
        }
        */
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var dbUser = db.UserProfiles.Find(id);
            if (dbUser == null)
                return HttpNotFound();
            long? userRegion;
            if (dbUser.RegionId == null)
                userRegion = null;
            else
                userRegion = dbUser.Region.IidParent;
            if (!User.IsInRole(db, RoleEnum.Admin, userRegion))
                return RedirectToAction("Login", new { returnUrl = Url.Action("Delete", new { id = id }) });
            return View(dbUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            bool hasLocker = false;
            try
            {
                Monitor.Enter(locker, ref hasLocker);
                var todel = db.UserProfiles.Find(id);
                if (todel == null)
                    return HttpNotFound();
                var userArea = todel.RegionId == null ? null : todel.Region.IidParent;
                if (!User.IsInRole(db, RoleEnum.Admin, userArea))
                    return RedirectToAction("Login", new { returnUrl = Url.Action("DeleteConfirmed", new { id = id }) });
                if (todel.RegionId == null &&
                    (todel.Roles.Count(r => r.CompID == null && r.RoleId == (int)RoleEnum.Admin && r.RegionID == null) > 0)
                    && (db.UserRoles.Count(r => r.CompID == null && r.RegionID == null && r.RoleId == (int)RoleEnum.Admin &&
                                          r.UserId != todel.Iid) < 1))
                {
                    ModelState.AddModelError(String.Empty, "Нельзя удалять последнего администратора БД");
                    return View("Delete", todel);
                }
                foreach (var role in todel.Roles.ToArray())
                    db.UserRoles.Remove(role);
                db.UserProfiles.Remove(todel);
                db.SaveChanges();
                return RedirectToAction("Index", new { creationResult = DbResult.Deleted });
            }
            finally
            {
                if (hasLocker)
                    Monitor.Exit(locker);
            }
        }

        
        [HttpGet]
        public ActionResult AdminEdit(int id)
        {
            var dbUser = db.UserProfiles.Find(id);
            if (dbUser == null)
                return HttpNotFound();
            var regList = SetRegionsViewBag();
            if (regList.Count(r => r.Iid == (dbUser.RegionId ?? 0)) < 1)
                return RedirectToAction("Login", new { returnUrl = Url.Action("AdminEdit", new { id = id }) });
            UserEditModel model = new UserEditModel
            {
                Email = dbUser.Email,
                RegionId = (dbUser.RegionId ?? 0),
                UserName = dbUser.Name,
                Iid = dbUser.Iid
            };
            ViewBag.Action = "AdminEdit";
            ViewBag.Password = false;
            ViewBag.Success = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult AdminEdit(UserEditModel model)
        {
            bool hasLocker = false;
            try
            {
                Monitor.Enter(locker, ref hasLocker);
                var regList = SetRegionsViewBag();
                ViewBag.Action = "AdminEdit";
                ViewBag.Password = false;
                ViewBag.Success = false;
                if (!ModelState.IsValid)
                    return View(model);
                if (regList.Count(r => r.Iid == (model.RegionId ?? 0)) < 1)
                {
                    ModelState.AddModelError(String.Empty, "У вас нет прав для данного региона");
                    return View(model);
                }
                var dbUser = db.UserProfiles.Find(model.Iid);
                if (dbUser == null)
                {
                    ModelState.AddModelError(String.Empty, "Пользователь не найден");
                    return View(model);
                }
                if (regList.Count(r => r.Iid == (dbUser.RegionId ?? 0)) < 1)
                {
                    ModelState.AddModelError(String.Empty, "У вас нет прав для правки данного пользователя");
                    return View(model);
                }
                if (db.UserProfiles.Count(u => u.Iid != dbUser.Iid && u.Name.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    ModelState.AddModelError(String.Empty, "Пользователь с таким логином уже существует");
                    return View(model);
                }
                dbUser.Name = model.UserName;
                dbUser.RegionId = (model.RegionId ?? 0) < 1 ? null : model.RegionId;
                dbUser.Email = model.Email;
                db.SaveChanges();
            }
            finally
            {
                if (hasLocker)
                    Monitor.Exit(locker);
            }
            return RedirectToAction("Index", new { creationResult = DbResult.Updated });

        }

        [HttpGet]
        public ActionResult SelfEdit(bool scs = false)
        {
            var dbUser = User.GetDbUser(db, false);
            if (dbUser == null)
                return HttpNotFound();
            UserEditModel model = new UserEditModel
            {
                Email = dbUser.Email,
                Iid = dbUser.Iid,
                UserName = dbUser.Name
            };
            ViewBag.Action = "SelfEdit";
            ViewBag.Password = true;
            ViewBag.Success = scs;
            return View("AdminEdit", model);
        }

        [HttpPost]
        public ActionResult SelfEdit(UserEditModel model)
        {
            bool kasLocker = false;
            try
            {
                Monitor.Enter(locker, ref kasLocker);
                ViewBag.Action = "SelfEdit";
                ViewBag.Password = true;
                ViewBag.Success = false;
                if (!ModelState.IsValid)
                    return View("AdminEdit", model);
                var dbUser = User.GetDbUser(db, false);
                if (dbUser == null)
                {
                    ModelState.AddModelError(String.Empty, "Пользователь не найден");
                    return View("AdminEdit", model);
                }
                if (dbUser.Iid != model.Iid)
                {
                    ModelState.AddModelError(String.Empty, "Обманывать не хорошо");
                    return View("AdminEdit", model);
                }
                if (!dbUser.CheckPassword(model.Password ?? String.Empty))
                {
                    ModelState.AddModelError(String.Empty, "Неверный пароль");
                    return View("AdminEdit", model);
                }
                if (db.UserProfiles.Count(u => u.Iid != dbUser.Iid && u.Name.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    ModelState.AddModelError(String.Empty, "Пользователь с таким именем существует");
                    return View("AdminEdit", model);
                }
                if (!String.IsNullOrEmpty(model.NewPassword))
                    dbUser.SetPassword(model.NewPassword);
                dbUser.Name = model.UserName;
                dbUser.Email = model.Email;
                db.SaveChanges();
            }
            finally
            {
                if (kasLocker)
                    Monitor.Exit(locker);
            }
            WebSecurity.Login(model.UserName, String.IsNullOrEmpty(model.NewPassword) ? model.Password : model.NewPassword);
            return RedirectToAction("SelfEdit", new { scs = true });
        }

        //private void SetRegionsData()
        //{
        //    List<RegionModel> ap;
        //    SetRegionsData(out ap, true);
        //}

        //private bool SetRegionsData(out List<RegionModel> allowedParents, bool set = false)
        //{
        //    allowedParents = null;
        //    var user = User.GetDbUser(db);
        //    HashSet<RegionModel> allowed = new HashSet<RegionModel>();
        //    bool hasEmptyParent = false;
        //    //foreach (var r in user.Roles.Where(r => r.CompID == null).ToList().Where(r => r.Role.HasFlags(RightsType.Database, RightsEnum.Edit)))
        //    //{
        //    //    if (r.RegionID == null)
        //    //        hasEmptyParent = true;
        //    //    else if (!allowed.Contains(r.Region))
        //    //        allowed.Add(r.Region);
        //    //}
        //    if (allowed.Count() < 1 && !hasEmptyParent)
        //        return false;
        //    allowedParents = new List<RegionModel>();
        //    if (hasEmptyParent)
        //    {
        //        ViewBag.DefaultRegion = "Без региона";
        //        allowedParents.AddRange(db.Regions.Where(r => r.IidParent == null));
        //    }
        //    else
        //        ViewBag.DefaultRegion = null;

        //    allowedParents.AddRange(allowed);

        //    foreach (var reg in allowed)
        //        allowedParents.AddRange(reg.Children);
        //    allowedParents.Sort();
        //    if(set)
        //        ViewBag.RegionId = new SelectList(allowedParents, "Iid", "Name");
        //    return true;
        //    //return allowedParents;
        //}

        ////
        //// GET: /Account/Edit/1
        //public ActionResult Edit(int id)
        //{
        //    var userToEdit = db.UserProfiles.Find(id);
        //    if (userToEdit == null)
        //        return HttpNotFound();
        //    var dbUser = User.GetDbUser(db);
        //    /*if (!User.AllowedToEditUser(id, db))
        //        return RedirectToAction("Login", new { returnUrl = Url.Action("Edit", new { id = id }) });
        //     * */
        //    List<RegionModel> allowedRegions;
        //    if (!SetRegionsData(out allowedRegions))
        //        return RedirectToAction("Login", new { returnUrl = Url.Action("Edit", new { id = id }) });
        //    var defaultRegion = allowedRegions.FirstOrDefault(r => r.Iid == userToEdit.RegionId);
        //    var dr = (defaultRegion == null ? null : new long?(defaultRegion.Iid));
        //    ViewBag.RegionId = new SelectList(allowedRegions, "Iid", "Name", dr);
        //    RegisterModel items = new RegisterModel(userToEdit);
        //    ViewBag.Title = "Правка пользователя " + userToEdit.Name;
        //    ViewBag.SaveButtonLabel = "Сохранить";
        //    return View("Register", items);
        //}

        public enum ValidateStatusMessage
        {
            IncorrectUser,
            PasswordError,
            Nothing,
            ManyEmails,
            ProviderError,
            Success,
            Expired
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult Validate(string token, ValidateStatusMessage message = ValidateStatusMessage.Nothing)
        //{
        //    if (WebSecurity.GetUserIdFromPasswordResetToken(token) < 1)
        //        return HttpNotFound();
        //    ValidationModel items = new ValidationModel();
        //    items.Token = token;

        //    switch (message)
        //    {
        //        case ValidateStatusMessage.IncorrectUser:
        //            ViewBag.StatusMessage = "Неверное имя пользователя";
        //            break;
        //        case ValidateStatusMessage.PasswordError:
        //            ViewBag.StatusMessage = "Ошибка установки пароля";
        //            break;
        //        default:
        //            ViewBag.StatusMessage = String.Empty;
        //            break;
        //    }

        //    return View(items);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult Validate(ValidationModel items)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var userId = WebSecurity.GetUserIdFromPasswordResetToken(items.Token);
        //        var user = db.UserProfiles.Find(userId);
        //        if (user == null)
        //            return HttpNotFound();
        //        if (!user.Name.Equals(items.UserName, StringComparison.OrdinalIgnoreCase))
        //            return RedirectToAction("Validate", new { token = items.Token, message = ValidateStatusMessage.IncorrectUser });
        //        if(!WebSecurity.ResetPassword(items.Token, items.Password))
        //            return RedirectToAction("Validate", new { token = items.Token, message = ValidateStatusMessage.PasswordError });
        //        WebSecurity.Login(user.Name, items.Password, false);
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View(items);
        //}

        ////
        //// POST: /Account/Register

        //private ActionResult EditUser(RegisterModel items, int userId)
        //{
        //    /*if (!User.AllowedToEditUser(userId, db))
        //        return RedirectToAction("Login", new { returnUrl = Url.Action("Edit", new { id = userId }) });*/
        //    var uswer = db.UserProfiles.Find(userId);
        //    if (uswer == null)
        //        return HttpNotFound();
        //    if (db.UserProfiles.Count(u => u.Iid != userId && u.Name.Equals(items.UserName, StringComparison.OrdinalIgnoreCase)) > 0)
        //        throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
        //    StringBuilder emailText = new StringBuilder();
        //    if (!uswer.Name.Equals(items.UserName, StringComparison.OrdinalIgnoreCase))
        //        emailText.AppendFormat("Изменилось имя пользователя.{0}Было: {1}{0}Стало: {2}{0}", Environment.NewLine, uswer.Name, items.UserName);
        //    string oldEmail = null;
        //    if (!uswer.Email.Equals(items.Email, StringComparison.OrdinalIgnoreCase))
        //    {
        //        emailText.AppendFormat("Изменился e-mail.{0}Был: {1}{0}Стал: {2}", Environment.NewLine, uswer.Email, items.Email);
        //        oldEmail = uswer.Email;
        //    }
        //    uswer.Name = items.UserName;
        //    uswer.RegionId = items.RegionId;
        //    uswer.Email = items.Email;
        //    db.SaveChanges();
        //    long? regionP = GetListRegion(uswer);
        //    if (emailText.Length > 0)
        //    {
        //        MailService.SendMessage(uswer.Email, "Изменение персональных данных", emailText.ToString(), true);
        //        if (oldEmail != null)
        //            MailService.SendMessage(oldEmail, "Изменение персональных данных", emailText.ToString(), true);
        //    }
        //    return RedirectToAction("Index", new { id = regionP });
        //}

        //private long? GetListRegion(UserProfileModel uswer)
        //{
        //    long? regionP;
        //    if (uswer.RegionId == null)
        //        regionP = null;
        //    else if (uswer.RegionId == User.GetDbUser(db).RegionId)
        //        regionP = uswer.RegionId;
        //    else
        //        regionP = db.Regions.First(r => r.Iid == uswer.RegionId).IidParent;
        //    return regionP;
        //}

        

        ////
        //// POST: /Account/Disassociate

        //private string GeneratePasswordResetLink(string token)
        //{
        //    return String.Format("http://{0}{1}", Request.Url.Authority, Url.Action("Validate", new { token = token }));
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgottenPassword(string initialName = null, ValidateStatusMessage message = ValidateStatusMessage.Nothing)
        {
            ForgottenPasswordModel model = new ForgottenPasswordModel();
            if (initialName != null)
                model.UserName = initialName;
            string msg;
            switch (message)
            {
                case ValidateStatusMessage.IncorrectUser:
                    msg = "Неправильное имя пользователя или E-mail";
                    break;
                case ValidateStatusMessage.ManyEmails:
                    msg = "Для данного адреса электронной почты восстановление пароля невозможно";
                    break;
                case ValidateStatusMessage.Expired:
                    msg = "Истек срок действия кода подтверждения смены пароля. Запросите новый код";
                    break;
                case ValidateStatusMessage.ProviderError:
                    msg = "Ошибка отправки кода сброса пароля. Пожалуйста, повторите попытку";
                    break;
                case ValidateStatusMessage.Success:
                    msg = "Код сброса пароля отправлен на Ваш e-mail. Срок действия кода 24 часа";
                    model.ModelSuccess = true;
                    break;
                default:
                    msg = String.Empty;
                    break;
            }
            ViewBag.StatusMessage = msg;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgottenPassword(ForgottenPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            string userName = model.UserName;
            var dbUser = db.UserProfiles.FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (dbUser == null)
            {
                var userEmailList = db.UserProfiles.Where(u => u.Email.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
                int n = userEmailList.Count;
                if (n > 1)
                    return RedirectToAction("ForgottenPassword", new { initialName = userName, message = ValidateStatusMessage.ManyEmails });
                else if (n <= 0)
                    return RedirectToAction("ForgottenPassword", new { initialName = userName, message = ValidateStatusMessage.IncorrectUser });
                dbUser = userEmailList[0];
            }
            dbUser.Token = GenerateToken();
            dbUser.PasswordTokenExpirationTime = DateTime.UtcNow.AddDays(1.0);
            db.SaveChanges();
            ValidateStatusMessage result;
            if (!MailService.SendMessage(dbUser.Email, "Сброс пароля",
                String.Format("Для установки нового пароля для Вашего пользователя {1},{0}пожалуйста, перейдите по ссылке {2}",
                Environment.NewLine, dbUser.Name,
                Url.Action("Activate", "Account", new { id = dbUser.Token }, "http")
                )))
                result = ValidateStatusMessage.ProviderError;
            else
                result = ValidateStatusMessage.Success;
            return RedirectToAction("ForgottenPassword", new { initialName = userName, message = result });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in dbUser is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the dbUser from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Аккаунт обновлен."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    LocalPasswordModel items = new LocalPasswordModel();
        //    var mu = Membership.GetUser();
        //    items.Email = mu.Email;
        //    items.UserName = User.Identity.Name;
        //    return View(items);
        //}

        ////
        //// POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel items)
        //{
        //    var id = WebSecurity.GetUserId(User.Identity.Name);
        //    var db_u = db.UserProfiles.Find(id);
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(id);
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    //if (hasLocalAccount)
        //    //{
        //    if (ModelState.IsValid)
        //    {
        //        // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //        try
        //        {
        //            if (!Membership.ValidateUser(User.Identity.Name, items.OldPassword))
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect.");
        //                return View(items);
        //            }

        //            var user = Membership.GetUser(User.Identity.Name);
        //            if (db_u == null)
        //                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);

        //            user.Email = items.Email;
        //            Membership.UpdateUser(user);
        //            db_u = db.UserProfiles.Find(user.ProviderUserKey);

        //            if (db.UserProfiles.Count(up => up.Iid != db_u.Iid && up.Name.Equals(items.UserName, StringComparison.OrdinalIgnoreCase)) > 0)
        //                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
        //            db_u.Name = items.UserName;

        //            string pswToCheck;
        //            if (String.IsNullOrEmpty(items.NewPassword))
        //                pswToCheck = items.OldPassword;
        //            else
        //            {
        //                db_u.SetPassword(items.NewPassword);
        //                pswToCheck = items.NewPassword;
        //                user.ChangePassword(items.OldPassword, items.NewPassword);
        //            }
        //            db.SaveChanges();

        //            if (!items.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
        //            {
        //                WebSecurity.Logout();
        //                WebSecurity.Login(items.UserName, pswToCheck);
        //            }

        //            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //        }
        //        catch (MembershipCreateUserException e)
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        //        }
        //        //ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //    }
        //    //}
        //    //else
        //    //{
        //    //    // User does not have a local password so remove any validation errors caused by a missing
        //    //    // OldPassword field
        //    //    ModelState state = ModelState["OldPassword"];
        //    //    if (state != null)
        //    //    {
        //    //        state.Errors.Clear();
        //    //    }

        //    //    if (ModelState.IsValid)
        //    //    {
        //    //        try
        //    //        {
        //    //            WebSecurity.CreateAccount(User.Identity.Name, grList.NewPassword);
        //    //            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //    //        }
        //    //        catch (Exception e)
        //    //        {
        //    //            ModelState.AddModelError("", e);
        //    //        }
        //    //    }
        //    //}

        //    // If we got this far, something failed, redisplay form
        //    return View(items);
        //}

        ////
        //// POST: /Account/ExternalLogin

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current dbUser is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel items, string returnUrl)
        //{
        //    string provider = null;
        //    string providerUserId = null;

        //    if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(items.ExternalLoginData, out provider, out providerUserId))
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Insert a new dbUser into the database
        //        using (ClimbingContext db = new ClimbingContext())
        //        {
        //            UserProfileModel user = db.UserProfiles.FirstOrDefault(u => u.Name.ToLower() == items.UserName.ToLower());
        //            // Check if dbUser already exists
        //            if (user == null)
        //            {
        //                // Insert name into the profile table
        //                db.UserProfiles.Add(new UserProfileModel { Name = items.UserName });
        //                db.SaveChanges();

        //                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, items.UserName);
        //                OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

        //                return RedirectToLocal(returnUrl);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("UserName", "User name already exists. Please enter a different dbUser name.");
        //            }
        //        }
        //    }

        //    ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(items);
        //}

        ////
        //// GET: /Account/ExternalLoginFailure

        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult ExternalLoginsList(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveExternalLogins()
        //{
        //    ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
        //    List<ExternalLogin> externalLogins = new List<ExternalLogin>();
        //    foreach (OAuthAccount account in accounts)
        //    {
        //        AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

        //        externalLogins.Add(new ExternalLogin
        //        {
        //            Provider = account.Provider,
        //            ProviderDisplayName = clientData.DisplayName,
        //            ProviderUserId = account.ProviderUserId,
        //        });
        //    }

        //    ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        //}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different dbUser name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A dbUser name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The dbUser name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The dbUser creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
