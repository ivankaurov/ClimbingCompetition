using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Users
{
    [Table("usr_object_rights")]
    class DbRights : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.AllRights.Remove(this);
        }

        protected DbRights() { }

        public DbRights(BaseContext context) : base(context) { }

        [Column("subject_id"), MaxLength(IID_SIZE), Index("usr_obj_rights_UX1", IsUnique = true, Order = 2)]
        public String SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public virtual BaseObject Subject { get; set; }

        [Column("object_id"), MaxLength(IID_SIZE), Index("usr_obj_rights_UX1", IsUnique = true, Order = 1)]
        public String ObjectId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual DbSecurityEntity Object { get; set; }

        [Column("right_data"), MaxLength(1)]
        protected String RightData { get; set; }

        [Column("action_data"), MaxLength(1)]
        protected string ActionData { get; set; }

        [NotMapped]
        public RightsEnum Rights
        {
            get { return RightData.GetByFirstLetter<RightsEnum>(); }
            set { RightData = value.GetFirstLetter(); }
        }

        [NotMapped]
        public RightsActionEnum Action
        {
            get { return ActionData.GetByFirstLetter<RightsActionEnum>(); }
            set { ActionData = value.GetFirstLetter(); }
        }

        [Column("is_inherited")]
        public Boolean IsInherited { get; protected internal set; }
    }
}