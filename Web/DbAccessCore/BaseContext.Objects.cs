// <copyright file="BaseContext.Objects.cs">
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
using System.Data.Entity;
using DbAccessCore.Users;
using DbAccessCore.Log;
using System.Collections.Concurrent;
using System.Reflection;

namespace DbAccessCore
{
    partial class BaseContext
    {
        public DbSet<BaseObject> AllObjects { get; set; }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbActiveUser> ActiveUsers { get; set; }
        public DbSet<DbAudit> Audit { get; set; }
        public DbSet<DbSecurityEntity> SecurityEntities { get; set; }
        public DbSet<DbUserGroup> UserGroups { get; set; }
        public DbSet<DbUserGroupMember> GroupMembers { get; set; }
        internal DbSet<DbRights> AllRights { get; set; }

        public DbSet<LogicTransaction> LogicTransactions1 { get; set; }
        public DbSet<LogicTransactionObject> LogicTransactionObjects1 { get; set; }
        public DbSet<LogicTransactionObjectParam> LogicTransactionObjectParams1 { get; set; }

        public DbSet<Windows.WindowDescriptor> Windows { get; set; }
        public DbSet<Windows.ActionDescriptor> Actions { get; set; }
    }
}