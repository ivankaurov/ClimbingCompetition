using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DbAccessCore.Log
{
    [Table("log_logic_transaction_objects")]
    public partial class LogicTransactionObject : IIIDObject
    {
        const String INDEX_NAME = "ltr_objects_UX1";

        protected LogicTransactionObject() { }

        internal LogicTransactionObject(LogAction action, IBaseObject obj, BaseContext context)
        {
            this.Iid = context.CreateNewIid();
            this.Action = action;
            this.ObjectIid = obj.Iid;
            this.ObjectClass = obj.ObjectClass;
        }

        [Key, Column("iid"), MaxLength(BaseObject.IID_SIZE), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Iid { get; set; }

        [Column("ltr_iid"), MaxLength(BaseObject.IID_SIZE), Index(INDEX_NAME, IsUnique = true, Order = 3)]
        public String LtrIid { get; protected set; }

        [ForeignKey("LtrIid")]
        public virtual LogicTransaction Ltr { get; protected set; }

        [Column("add_order")]
        public int AddOrder { get; set; }

        [Column("iid_obj"), MaxLength(BaseObject.IID_SIZE), Required, Index(INDEX_NAME, IsUnique = true, Order = 1)]
        public String ObjectIid { get; protected set; }

        [Column("object_class"), MaxLength(BaseObject.TYPE_SIZE), Required]
        public String ObjectClass { get; protected set; }

        [Column("action"), /*Required,*/ Index(INDEX_NAME, IsUnique = true, Order = 2), MaxLength(1)]
        public String ActionChar { get; protected set; }

        [NotMapped]
        public LogAction Action
        {
            get { return ActionChar.GetByFirstLetter<LogAction>(); }
            protected set { ActionChar = value.GetFirstLetter(); }
        }

        public virtual ICollection<LogicTransactionObjectParam> Params { get; set; }


        public void RemoveObject(BaseContext context, LogicTransaction ltr = null)
        {
            if (context == null)
                return;
            if (Params != null)
                Params.ToList().ForEach(p => p.RemoveObject(context, ltr));
            context.LogicTransactionObjects1.Remove(this);
        }
    }
}