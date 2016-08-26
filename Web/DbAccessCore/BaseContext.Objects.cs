using System;
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