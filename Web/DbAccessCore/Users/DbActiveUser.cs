using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DbAccessCore.Users
{
    [Table("usr_active_users")]
    public class DbActiveUser : IIIDObject
    {
        protected DbActiveUser() { }
        public DbActiveUser(DbUser user, BaseContext context)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            this.Iid = context.CreateNewIid();
            this.ClrHostName = context.CLRHostNameForSQL;
            this.ClrIP = context.CLRIPAddressForSQL;
            this.LoginTime = context.LoginTime;
            this.Spid = context.SPID;
            this.User = user;
        }

        [Key, Column("iid"), Required, MaxLength(BaseObject.IID_SIZE), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Iid { get; set; }

        [Column("spid")]
        public int Spid { get; protected set; }

        [Column("login_time")]
        public DateTime LoginTime { get; protected set; }

        [Column("user_id"), MaxLength(BaseObject.IID_SIZE)]
        public String UserId { get; protected set; }

        [ForeignKey("UserId")]
        public virtual DbUser User { get; protected set; }

        [Column("web_login")]
        public Boolean LoginInBrowser { get; protected set; }

        [Column("client_host_name"), MaxLength(255)]
        public String ClientHostName { get; protected set; }

        [Column("clr_host_name"), MaxLength(255)]
        public String ClrHostName { get; protected set; }

        [Column("client_ip"), MaxLength(15)]
        public String ClientIP { get; protected set; }

        [Column("clr_ip"), MaxLength(15)]
        public String ClrIP { get; protected set; }


        public void RemoveObject(BaseContext context, Log.LogicTransaction ltr = null)
        {
            if (context == null)
                return;
            context.ActiveUsers.Remove(this);
        }
    }
}
