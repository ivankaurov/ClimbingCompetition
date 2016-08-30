// <copyright file="ExceptionHandlingController.cs">
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
using System.Web;
using System.Web.Mvc;
using WebClimbing.Models;
using WebClimbing.Models.UserAuthentication;
using System.Threading.Tasks;
using System.Text;

namespace WebClimbing.ServiceClasses
{
    public class ExceptionLoggerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.ExceptionHandled || (filterContext.Exception is System.Web.Http.HttpResponseException)))
            {
                try
                {
                    DateTime dt = DateTime.UtcNow;
                    StringBuilder exData = new StringBuilder();
                    exData.AppendFormat("Date:           {0}{1}", dt.ToShortDateString(), Environment.NewLine);
                    exData.AppendFormat("Time:           {0}{1}", dt.ToLongTimeString(), Environment.NewLine);
                    exData.AppendFormat("Controller:     {0}{1}", filterContext.Controller.GetType(), Environment.NewLine);
                    exData.AppendFormat("IsChildAction:  {0}{1}", filterContext.IsChildAction, Environment.NewLine);
                    exData.AppendFormat("Exception Data: {1}{0}", GetExceptionData(filterContext.Exception, 0), Environment.NewLine);

                    ClimbingContext db = new ClimbingContext();
                    IEnumerable<String> emailList;
                    try { emailList = db.UserRoles.Where(r => r.CompID == null && r.RegionID == null && r.RoleId >= (int)RoleEnum.Admin).Select(r => r.User.Email ?? String.Empty).ToList().Distinct(); }
                    catch { emailList = new List<string> { "ivan.kaurov@gmail.com" }; }
                    Parallel.ForEach(emailList.Where(e => !String.IsNullOrEmpty(e)),
                        u => MailService.SendMessage(u, "Unhandled Exception",
                            exData.ToString(), true));
                }
                catch { }
            }
            base.OnException(filterContext);
        }

        private static String GetExceptionData(Exception ex, int level)
        {
            StringBuilder exceptionData = new StringBuilder();
            StringBuilder sPrefixBuilder = new StringBuilder();
            for (int i = 0; i < level; i++)
                sPrefixBuilder.Append("  ");
            String sPrefix = sPrefixBuilder.ToString();
            exceptionData.AppendFormat("{2}Exception:      {0}{1}", ex.GetType(), Environment.NewLine, sPrefix);
            exceptionData.AppendFormat("{2}Message:        {0}{1}", ex.Message, Environment.NewLine, sPrefix);
            exceptionData.AppendFormat("{1}Stack Trace:    {0}", ex.StackTrace, sPrefix);
            if (ex.InnerException != null)
                exceptionData.AppendFormat("{0}{2}InnerException:{0}{1}", Environment.NewLine, GetExceptionData(ex.InnerException, level + 1), sPrefix);
            return exceptionData.ToString();
        }
    }
}