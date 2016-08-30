// <copyright file="BaseContext.LoginControl.cs">
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
using DbAccessCore.Users;

namespace DbAccessCore
{
    partial class BaseContext
    {
        DbUser currentUser = null;
        public DbUser CurrentUser { get { return currentUser; } }

        Lazy<Boolean> userIsAdmin;

        void resetUserIsAdmin()
        {
            userIsAdmin = new Lazy<bool>(() =>
            {
                if (CurrentUser == null)
                    return false;
                return UserIsAdmin(CurrentUserID);
            });
        }

        public String CurrentUserID
        {
            get
            {
                if (CurrentUser != null)
                    return CurrentUser.Iid;
                if (SqlHostName.Length > BaseObject.IID_SIZE)
                    return SqlHostName.Substring(0, BaseObject.IID_SIZE);
                return SqlHostName;
            }
        }

        public Boolean CurrentUserIsAdmin { get { return userIsAdmin == null ? false : userIsAdmin.Value; } }

        public Boolean UserIsAdmin(String userId)
        {
            if (String.IsNullOrEmpty(userId))
                return false;
            return this.GroupMembers.FirstOrDefault(gm => gm.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && gm.GroupId.Equals(ADMIN_GROUP, StringComparison.OrdinalIgnoreCase)) != null;
        }

        void SetCurrentUser(DbUser user)
        {
            if (user == null)
                ClearCurrentUser(true);
            else
            {
                var deadLogins = ActiveUsers.Where(u => u.Spid == SPID).ToArray();
                if (deadLogins.Length > 0)
                    ActiveUsers.RemoveRange(deadLogins);
                
                if (IsWebContext)
                    user.LastOnlineWeb = this.Now;
                else
                    user.LastOnlineDesktop = this.Now;

                if (!IsWebContext)
                    ActiveUsers.Add(new DbActiveUser(user, this));

                currentUser = user;

                SetCurrentUserTimeStamp(true);
                
                resetUserIsAdmin();
            }
        }

        void ClearCurrentUser(bool saveChanges)
        {
            if (currentUser != null)
            {
                var toRemove = ActiveUsers.Where(u => u.Spid == SPID).ToArray();
                if (toRemove.Length > 0)
                    ActiveUsers.RemoveRange(toRemove);
                SetCurrentUserTimeStamp(saveChanges);
                currentUser = null;
                resetUserIsAdmin();
            }
        }

        Boolean IsUserOnline(String userId)
        {
            var userLogins = this.ActiveUsers.Where(u => u.UserId.Equals(userId, StringComparison.Ordinal))
                                             .ToList();
            var deadLogins = userLogins.Where(l =>
            {
                var sqlLoginTime = this.GetLoginTime(l.Spid);
                return sqlLoginTime.HasValue && sqlLoginTime.Value != l.LoginTime || !sqlLoginTime.HasValue;
            })
            .ToList();
            if (deadLogins.Count > 0)
            {
                this.ActiveUsers.RemoveRange(deadLogins);
                this.SaveChanges();
                return this.ActiveUsers.FirstOrDefault(u => u.UserId.Equals(userId, StringComparison.Ordinal) && !u.LoginInBrowser) != null;
            }
            else
                return userLogins.FirstOrDefault(u => !u.LoginInBrowser) != null;
        }

        public void LogOut()
        {
            LogOut(true);
        }

        public void SetCurrentUserTimeStamp(Boolean saveChanges)
        {
            if(this.currentUser != null)
            {
                if (this.IsWebContext)
                    this.currentUser.LastOnlineWeb = this.Now;
                else
                    this.currentUser.LastOnlineDesktop = this.Now;
                if (saveChanges)
                    this.SaveChanges();
            }
        }

        void LogOut(Boolean withAudit)
        {
            if (withAudit && this.currentUser != null)
            {
                Audit.Add(new DbAudit(DbAudit.AuditDataType.Logout, this)
                {
                    ResultSuccess = true,
                    User = this.CurrentUser
                });
            }
            ClearCurrentUser(true);
        }

        public void LogIn(String login, String password)
        {
            ClearCurrentUser(true);
            var auditMesage = Audit.Add(new DbAudit(DbAudit.AuditDataType.Login, this));
            try
            {
                var user = Users.FirstOrDefault(u => u.UserName.Equals(login, StringComparison.OrdinalIgnoreCase));
                if (user == null && login.IndexOf('@') > 0)
                {
                    var usersByEmail = Users.Where(u => u.Email.Equals(login, StringComparison.OrdinalIgnoreCase) && !u.LogicallyDeleted).ToArray();
                    if (usersByEmail.Length > 1)
                        throw new LoginException(CoreTranslations.UserCantLoginByEmail);
                    else if (usersByEmail.Length == 1)
                        user = usersByEmail[0];
                }
                if (user == null)
                    throw new LoginException(CoreTranslations.InvalidUser);
                auditMesage.User = user;

                if (user.LogicallyDeleted)
                    throw new LoginException(CoreTranslations.UserIsLocked);

                if (!(IsWebContext || user.AllowMultipleLogins) && IsUserOnline(user.Iid))
                    throw new LoginException(CoreTranslations.UserAlreadyOnline);

                if (!user.CheckPassword(password))
                    throw new LoginException(CoreTranslations.InvalidPassword);

                auditMesage.ResultSuccess = true;
                SetCurrentUser(user);
                this.SaveChanges();
            }
            catch (LoginException loginException)
            {
                auditMesage.ResultSuccess = false;
                auditMesage.Message = loginException.Message;
                this.SaveChanges();
                throw;
            }

            
        }
    }
}
