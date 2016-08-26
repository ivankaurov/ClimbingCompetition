using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Users
{
    [Table("usr_security_entities")]
    public abstract class DbSecurityEntity : BaseObject
    {
        public override bool NeedLtrForDelete { get { return true; } }

        protected DbSecurityEntity() { }

        public DbSecurityEntity(BaseContext context) : base(context) { }

        internal virtual ICollection<DbRights> Rights { get; set; }
    }
}