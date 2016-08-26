using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Crypto;

namespace DbAccessCore.Users
{
    [Table("usr_users")]
    public class DbUser : DbSecurityEntity
    {
        protected override void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr)
        {
            RemoveChildCollection(context, ActiveLogins, ltr);
            RemoveChildCollection(context, Audit, ltr);
            RemoveChildCollection(context, Groups, ltr);
        }

        protected override void RemoveEntity(BaseContext context)
        {
            context.Users.Remove(this);
        }

        protected DbUser() { }

        public DbUser(BaseContext context) : base(context) { }

        [Column("email"), MaxLength(100), Index("ux_usr_eml", IsUnique = false)]
        public String Email { get; set; }

        [Column("username"), MaxLength(4 * IID_SIZE), Required, Index(IsUnique = true)]
        public String UserName { get; set; }

        [Column("password")]
        public String Password { get; set; }

        [Column("salt")]
        public Byte[] Salt { get; set; }

        [Column("need_change_password")]
        public Boolean NeedChangePassword { get; set; }

        [Column("allow_multiple_login")]
        public Boolean AllowMultipleLogins { get; set; }

        [Column("last_online_web")]
        public DateTime? LastOnlineWeb { get; set; }

        [Column("last_online_desktop")]
        public DateTime? LastOnlineDesktop { get; set; }

        public virtual ICollection<DbActiveUser> ActiveLogins { get; set; }
        public virtual ICollection<DbAudit> Audit { get; set; }
        public virtual ICollection<DbUserGroupMember> Groups { get; set; }

        public void SetPassword(String password)
        {
            Byte[] salt;
            this.Salt = salt = Crypto.Hashing.GenerateSalt(IID_SIZE);
            this.Password = Crypto.Hashing.ComputeHashString(password ?? String.Empty, salt);
        }

        public Boolean CheckPassword(String password)
        {
            return HasPassword ? string.Equals(this.Password, password.ComputeHashString(this.Salt), StringComparison.Ordinal) : false;
        }

        [NotMapped]
        public Boolean HasPassword
        {
            get
            {
                return this.Password != null && this.Password.Length > 0
                    && this.Salt != null && this.Salt.Length > 0;
            }
        }
    }
}
