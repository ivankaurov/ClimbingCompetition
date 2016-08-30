// <copyright file="__ApiBaseController.cs">
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

﻿using ClimbingCompetition.Common;
using ClimbingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace WebClimbing2.Controllers
{
    public abstract class __ApiBaseController : ApiController
    {
        public __ApiBaseController() { climbingContext = new Lazy<ClimbingContext2>(() => LoadContext()); }

        Lazy<ClimbingContext2> climbingContext;

        private ClimbingContext2 LoadContext()
        {
            return ClimbingContext2.CreateContextForBrowser(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["RemoteDbConnection"].ConnectionString,
                                                            null,
                                                            null,
                                                            "API_CALL");
        }

        public ClimbingContext2 Context { get { return climbingContext.Value; } }

        protected bool CheckCompetitionsPassword(string compId, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(compId))
                return false;
            var p = Context.CompetitionParameters.FirstOrDefault(pr => pr.CompId == compId && pr.ParamIdString == CompetitionParamId.UpdatePassword.ToString());
            if (p == null)
                return false;
            return string.Equals(p.StringValue, password, StringComparison.Ordinal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && climbingContext.IsValueCreated)
                climbingContext.Value.Dispose();
            base.Dispose(disposing);
        }
    }
}