using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Users
{
    [Table("usr_group_members")]
    public class DbUserGroupMember : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.GroupMembers.Remove(this);
        }

        protected DbUserGroupMember() { }
        public DbUserGroupMember(BaseContext context) : base(context) { }

        [Column("user_id"), MaxLength(IID_SIZE), Index("usr_group_members_UX1", IsUnique = true, Order = 1)]
        public String UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual DbUser User { get; set; }

        [Column("group_id"), MaxLength(IID_SIZE), Index("usr_group_members_UX1", IsUnique = true, Order = 2)]
        public String GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual DbUserGroup Group { get; set; }
    }
}