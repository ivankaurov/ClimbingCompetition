using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DbAccessCore.Log
{
    [Table("log_logic_transactions")]
    public partial class LogicTransaction : IIIDObject
    {
        protected LogicTransaction() { }

        internal LogicTransaction(BaseContext context) : this("-", context) { }

        internal LogicTransaction(String name, BaseContext context)
        {
            String iid;
            DateTime beginDate;
            context.GetObjectCreationValues(out iid, out beginDate);
            this.Iid = iid;
            this.BeginDate = beginDate;

            if (String.IsNullOrEmpty(name))
                this.Name = "-";
            else if (name.Length > BaseObject.IID_SIZE)
                this.Name = name.Substring(0, BaseObject.IID_SIZE);
            else
                this.Name = name;

            this.LtrBeginSpid = context.SPID;
            this.LtrBeginSpidLoginDate = context.LoginTime;
            this.LtrUserCreate = context.CurrentUserID;
            this.LtrCreatedInWeb = context.IsWebContext;
            
            this.State = LtrState.Begin;
        }

        [Key, Column("iid"), MaxLength(BaseObject.IID_SIZE), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Iid { get; protected set; }

        [Column("iid_parent"), MaxLength(BaseObject.IID_SIZE)]
        public String ParentTransactionIid { get; set; }

        [ForeignKey("ParentTransactionIid")]
        public virtual LogicTransaction ParentTransaction { get; set; }

        [Column("name"), MaxLength(BaseObject.IID_SIZE), Required]
        public String Name { get; protected set; }

        [Column("state"), MaxLength(1)]
        public String StateChar { get; protected set; }

        [NotMapped]
        public LtrState State
        {
            get { return StateChar.GetByFirstLetter<LtrState>(); }
            set { StateChar = value.GetFirstLetter(); }
        }

        [Column("begin_date")]
        public DateTime BeginDate { get; protected set; }

        [Column("commit_date")]
        public DateTime? CommitDate { get; protected set; }

        [Column("rollback_date")]
        public DateTime? RollbackDate { get; protected set; }

        [Column("ltr_begin_spid")]
        public int LtrBeginSpid { get; protected set; }

        [Column("ltr_begin_spid_login_date")]
        public DateTime LtrBeginSpidLoginDate { get; protected set; }

        [Column("user_create"), MaxLength(BaseObject.IID_SIZE), Required]
        public String LtrUserCreate { get; protected set; }

        [Column("user_rollback"), MaxLength(BaseObject.IID_SIZE)]
        public String LtrUserRollback { get; protected set; }

        [Column("web_created")]
        public Boolean LtrCreatedInWeb { get; set; }

        public virtual ICollection<LogicTransactionObject> Objects { get; set; }
        public virtual ICollection<LogicTransaction> Children { get; set; }


        public void RemoveObject(BaseContext context, LogicTransaction ltr = null)
        {
            if (context == null)
                return;

            if (Children != null)
                Children.ToList().ForEach(c => c.RemoveObject(context, ltr));
            if (Objects != null)
                Objects.ToList().ForEach(o => o.RemoveObject(context, ltr));
        }
    }
}