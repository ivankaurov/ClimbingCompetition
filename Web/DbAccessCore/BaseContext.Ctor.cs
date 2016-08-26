using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DbAccessCore.Users;
using Extensions;
using System.Threading;
using System.Threading.Tasks;
using DbAccessCore.Log;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace DbAccessCore
{
    partial class BaseContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbActiveUser>()
                .HasRequired(au => au.User)
                .WithMany(u => u.ActiveLogins)
                .HasForeignKey(au => au.UserId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DbAudit>()
                .HasOptional(a => a.User)
                .WithMany(u => u.Audit)
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DbUserGroup>()
                .HasMany(gr => gr.Users)
                .WithRequired(u => u.Group)
                .HasForeignKey(u => u.GroupId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DbUser>()
                .HasMany(u => u.Groups)
                .WithRequired(g => g.User)
                .HasForeignKey(g => g.UserId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DbSecurityEntity>()
                .HasMany(se => se.Rights)
                .WithRequired(sbj => sbj.Object)
                .HasForeignKey(sbj => sbj.ObjectId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<BaseObject>()
                .HasMany(sbj => sbj.RightsForThisObject)
                .WithRequired(ace => ace.Subject)
                .HasForeignKey(ace => ace.SubjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LogicTransaction>()
                .HasMany(lt => lt.Objects)
                .WithRequired(lo => lo.Ltr)
                .HasForeignKey(lo => lo.LtrIid)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<LogicTransaction>()
                .HasMany(lt => lt.Children)
                .WithOptional(ct => ct.ParentTransaction)
                .HasForeignKey(ct => ct.ParentTransactionIid)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<LogicTransactionObject>()
                .HasMany(lt => lt.Params)
                .WithRequired(lt => lt.LtrObj)
                .HasForeignKey(lt => lt.LtrObjIid)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Windows.WindowDescriptor>()
                .HasMany(wnd => wnd.ChildWindows)
                .WithOptional(wndC => wndC.ParentWindow)
                .HasForeignKey(wndC => wndC.ParentWindowId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Windows.WindowDescriptor>()
                .HasMany(wnd => wnd.ChildActions)
                .WithRequired(act => act.ParentWindow)
                .HasForeignKey(act => act.ParentWindowId)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        readonly bool createdForInitOnly;
        protected BaseContext(String connectionString)
            : base(connectionString)
        { this.createdForInitOnly = true; }

        protected BaseContext(Boolean isClone, Boolean isWeb, SqlConnection cn, Boolean contextOwnsConnection,
                             IPAddress clientIp, String clientHostName)
            : base(cn, false)
        {
            this.createdForInitOnly = false;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            this.connection = cn;
            this.ownConnection = contextOwnsConnection;
            this.isClone = isClone;
            this.web = isWeb;

            this.clrIP = Dns.GetHostEntry(IPAddress.Loopback)
                            .AddressList
                            .Where(adr => adr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !adr.Equals(IPAddress.Loopback))
                            .FirstOrDefault() ?? IPAddress.Loopback;



            this.clrHostNameForSQL = new Lazy<string>(() => clrHostName.Cut(255));
            this.clrIpAddressForSQL = new Lazy<string>(() => clrIP.CutString(15));

            spid = new Lazy<int>(() => Convert.ToInt32(SqlExecuteScalar("select @@spid")));
            dbHash = new Lazy<string>(() => SqlExecuteScalar(@"select left(@@SERVERNAME+'::'+DB_NAME(), 255)")
                                             .ToString().XORString());

            var serverDT = Convert.ToDateTime(SqlExecuteScalar("select GetDate()"));
            timeDiff = DateTime.Now - serverDT;

            loginTime = new Lazy<DateTime>(() => GetLoginTime(SPID).Value);
            sqlHostName = new Lazy<string>(() => SqlExecuteScalar("select host_name()").ToString());

            if (clientIp == null)
            {
                this.clientIp = this.clrIP;
                this.clientIpForSql = this.clrIpAddressForSQL;
                this.clientHostName = this.clrHostName;
                this.clientHostNameForSQL = this.clrHostNameForSQL;
            }
            else
            {
                this.clientIp = clientIp;
                this.clientIpForSql = new Lazy<string>(() => this.clientIp.CutString(15));
                this.clientHostName = (clientHostName ?? String.Empty);
                this.clientHostNameForSQL = new Lazy<string>(() => this.clientHostName.Cut(255));
            }
        }

        protected BaseContext(String connectionString, Boolean isClone, Boolean isWeb, IPAddress clientIP, String clientHostName) :
            this(isClone, isWeb, new SqlConnection(connectionString), true, clientIP, clientHostName)
        { }

        protected static BaseContext LoginOnDesktop(String server, String database, String userName, String password,
                                          Func<String, BaseContext> constructorDesktop)
        {
            BaseContext result = null;
            try
            {
                result = constructorDesktop(SqlCore.Connector.CreateInternalConnectionString(server, database, userName));
                result.SetInitializer(WhatToDo.LeaveAsIs);
                result.LogIn(userName, password);
                return result;
            }
            catch
            {
                if (result != null)
                    result.Dispose();
                throw;
            }
        }

        static readonly TimeSpan onlineGap =
#if DEBUG
            new TimeSpan(0, 0, 30)
#else
            new TimeSpan(0,30,0)
#endif
            ;
        public virtual TimeSpan OnlineGap { get { return onlineGap; } }

        protected static BaseContext CreateContextForBrowser(String userIidNameOrEmail, Func<BaseContext> constructorBrowser)
        {
            BaseContext result = null;
            try
            {
                result = constructorBrowser();
                result.SetInitializer(WhatToDo.LeaveAsIs);
                if (!String.IsNullOrEmpty(userIidNameOrEmail))
                {
                    var user = result.Users.FirstOrDefault(u => u.Iid.Equals(userIidNameOrEmail, StringComparison.Ordinal));
                    if (user == null)
                        user = result.Users.FirstOrDefault(u => u.UserName.Equals(userIidNameOrEmail, StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        var usersByEmail = result.Users.Where(u => u.Email.Equals(userIidNameOrEmail, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (usersByEmail.Count == 1)
                            user = usersByEmail[0];
                    }
                    if (user != null)
                    {
                        if (!user.LastOnlineWeb.HasValue || (result.Now - user.LastOnlineWeb.Value) > result.OnlineGap)
                        {
                            result.Audit.Add(new DbAudit(DbAudit.AuditDataType.AutoLoginWeb, result)
                            {
                                User = user,
                                ResultSuccess = true
                            });
                            result.SaveChanges();
                        }
                        result.SetCurrentUser(user);
                    }
                }
                return result;
            }
            catch
            {
                if (result != null)
                    result.Dispose();
                throw;
            }
        }

    }
}
