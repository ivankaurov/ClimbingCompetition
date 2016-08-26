using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DbAccessCore.Attributes;

namespace DbAccessCore
{
    [Table("objects")]
    public abstract partial class BaseObject : IBaseObject
    {
        [Key, Column("iid"), Required, MaxLength(IID_SIZE),
        DatabaseGenerated(DatabaseGeneratedOption.None),
        Loggable(false)]
        public String Iid { get; protected set; }

        [Column("object_type"), Required, MaxLength(TYPE_SIZE), Loggable(false)]
        public string ObjectClass { get; protected set; }

        [Column("logically_deleted")]
        public Boolean LogicallyDeleted { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; protected set; }

        [Column("update_date")]
        public DateTime UpdateDate { get; protected set; }

        [Column("created_by"), MaxLength(IID_SIZE)]
        public String UserCreated { get; protected set; }

        [Column("updated_by"), MaxLength(IID_SIZE)]
        public String UserUpdated { get; protected set; }

        [Column("last_ltr"), MaxLength(IID_SIZE)]
        public String LtrIid { get; internal set; }

        internal virtual ICollection<Users.DbRights> RightsForThisObject { get; set; }
    }
}